// tests/Wc3.Tests/ModelPreviewTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Modeling;

namespace Wc3.Tests;

/// <summary>
/// Hermetic tests for ModelPreviewCommand. Synthetic in-memory meshes (the public
/// Model3D/Geoset records) drive the internal render seam; the pure helpers
/// (FlattenGeosets / ProjectOrthographic / ScaleToFit) are asserted directly; and
/// the public byte[] entry point is exercised end-to-end with a minimal text-MDL
/// document plus empty/garbage input. No real .mdx from disk is needed.
/// </summary>
public class ModelPreviewTests
{
    // ---- fixtures --------------------------------------------------------

    /// <summary>Two triangles forming a unit quad in the XZ plane (Y = 0).</summary>
    private static Model3D TwoTriangleModel()
    {
        float[] verts =
        {
            0f, 0f, 0f,
            1f, 0f, 0f,
            1f, 0f, 1f,
            0f, 0f, 1f,
        };
        int[] indices = { 0, 1, 2, 0, 2, 3 };
        var geoset = new Geoset(verts, new float[verts.Length], new float[8], indices, -1);
        return new Model3D(new[] { geoset }, Array.Empty<string>());
    }

    /// <summary>The same quad as a minimal text-MDL document (public entry path).</summary>
    private const string QuadMdl = """
        // synthetic preview fixture
        Geoset {
            Vertices 4 {
                { 0, 0, 0 },
                { 1, 0, 0 },
                { 1, 0, 1 },
                { 0, 0, 1 },
            }
            Faces 1 6 {
                Triangles {
                    { 0, 1, 2, 0, 2, 3 },
                }
            }
        }
        """;

    private static void AssertPngSignature(byte[] png)
    {
        byte[] sig = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        Assert.True(png.Length >= 8, $"expected at least 8 bytes, got {png.Length}");
        Assert.Equal(sig, png[..8]);
    }

    /// <summary>Reads width/height from the IHDR chunk (big-endian, right after the signature).</summary>
    private static (int Width, int Height) PngSize(byte[] png)
    {
        Assert.True(png.Length > 24, "png too small to hold IHDR");
        Assert.Equal("IHDR", Encoding.ASCII.GetString(png, 12, 4));
        int w = (png[16] << 24) | (png[17] << 16) | (png[18] << 8) | png[19];
        int h = (png[20] << 24) | (png[21] << 16) | (png[22] << 8) | png[23];
        return (w, h);
    }

    // ---- rendering (internal Model3D seam) --------------------------------

    [Fact]
    public void Synthetic_mesh_renders_valid_png_of_requested_dimensions()
    {
        var png = ModelPreviewCommand.RenderPreview(TwoTriangleModel(), 64, 48);
        AssertPngSignature(png);
        Assert.Equal((64, 48), PngSize(png));
    }

    [Fact]
    public void Synthetic_mesh_render_differs_from_placeholder()
    {
        // Same size, one has geometry: the wireframe must actually draw something.
        var mesh = ModelPreviewCommand.RenderPreview(TwoTriangleModel(), 64, 64);
        var placeholder = ModelPreviewCommand.RenderPreview(Array.Empty<byte>(), 64, 64);
        Assert.NotEqual(placeholder, mesh);
    }

    [Fact]
    public void Rendering_is_deterministic()
    {
        var a = ModelPreviewCommand.RenderPreview(TwoTriangleModel(), 96, 96);
        var b = ModelPreviewCommand.RenderPreview(TwoTriangleModel(), 96, 96);
        Assert.Equal(a, b);
    }

    [Fact]
    public void Model_with_no_triangles_returns_placeholder_not_throw()
    {
        var empty = new Model3D(Array.Empty<Geoset>(), Array.Empty<string>());
        var png = ModelPreviewCommand.RenderPreview(empty, 32, 32);
        AssertPngSignature(png);
        Assert.Equal((32, 32), PngSize(png));
    }

    [Fact]
    public void Non_finite_vertices_and_bad_indices_never_throw()
    {
        float[] verts =
        {
            float.NaN, 0f, 0f,
            1f, float.PositiveInfinity, 0f,
            0f, 0f, 1f,
        };
        int[] indices = { 0, 1, 2, 0, 1, 99, -1, 1, 2 }; // NaN/Inf verts + out-of-range indices
        var model = new Model3D(
            new[] { new Geoset(verts, new float[verts.Length], new float[6], indices, -1) },
            Array.Empty<string>());
        var png = ModelPreviewCommand.RenderPreview(model, 32, 32);
        AssertPngSignature(png);
        Assert.Equal((32, 32), PngSize(png));
    }

    // ---- public byte[] entry point ----------------------------------------

    [Fact]
    public void Mdl_text_bytes_render_end_to_end()
    {
        var png = ModelPreviewCommand.RenderPreview(Encoding.UTF8.GetBytes(QuadMdl), 64, 64);
        AssertPngSignature(png);
        Assert.Equal((64, 64), PngSize(png));
        // Geometry parsed: not the placeholder.
        Assert.NotEqual(ModelPreviewCommand.RenderPreview(Array.Empty<byte>(), 64, 64), png);
        // Determinism through the full parse+render path.
        Assert.Equal(png, ModelPreviewCommand.RenderPreview(Encoding.UTF8.GetBytes(QuadMdl), 64, 64));
    }

    [Fact]
    public void Empty_input_returns_valid_placeholder_png()
    {
        var png = ModelPreviewCommand.RenderPreview(Array.Empty<byte>(), 32, 32);
        AssertPngSignature(png);
        Assert.Equal((32, 32), PngSize(png));
    }

    [Fact]
    public void Default_dimensions_are_512()
    {
        var png = ModelPreviewCommand.RenderPreview(Array.Empty<byte>());
        Assert.Equal((512, 512), PngSize(png));
    }

    [Fact]
    public void Garbage_input_returns_valid_placeholder_png()
    {
        // Truncated MDLX magic (binary parser path) and arbitrary text (MDL path).
        foreach (var bytes in new[]
                 {
                     Encoding.ASCII.GetBytes("MDLX"),
                     new byte[] { (byte)'M', (byte)'D', (byte)'L', (byte)'X', 0x01, 0x02, 0x03 },
                     Encoding.ASCII.GetBytes("this is not a model { } } {"),
                 })
        {
            var png = ModelPreviewCommand.RenderPreview(bytes, 32, 32);
            AssertPngSignature(png);
            Assert.Equal((32, 32), PngSize(png));
        }
    }

    [Fact]
    public void Degenerate_dimensions_are_clamped_not_thrown()
    {
        var png = ModelPreviewCommand.RenderPreview(Array.Empty<byte>(), 0, -5);
        AssertPngSignature(png);
        Assert.Equal((1, 1), PngSize(png));
    }

    // ---- pure helpers ------------------------------------------------------

    [Fact]
    public void FlattenGeosets_offsets_indices_and_drops_bad_triangles()
    {
        var a = new Geoset(
            new float[] { 0, 0, 0, 1, 0, 0, 0, 1, 0 }, new float[9], new float[6],
            new[] { 0, 1, 2, 0, 1, 99 }, -1); // second triangle out of range
        var b = new Geoset(
            new float[] { 0, 0, 5, 1, 0, 5, 0, 1, 5 }, new float[9], new float[6],
            new[] { 0, 1, 2 }, -1);
        var (verts, indices) = ModelPreviewCommand.FlattenGeosets(
            new Model3D(new[] { a, b }, Array.Empty<string>()));

        Assert.Equal(18, verts.Length);                    // 6 vertices packed
        Assert.Equal(new[] { 0, 1, 2, 3, 4, 5 }, indices); // b's indices offset by 3
    }

    [Fact]
    public void FlattenGeosets_drops_triangles_touching_non_finite_vertices()
    {
        var g = new Geoset(
            new float[] { 0, 0, 0, 1, 0, 0, float.NaN, 1, 0, 0, 0, 1 }, new float[12], new float[8],
            new[] { 0, 1, 2, 0, 1, 3 }, -1); // first triangle touches the NaN vertex
        var (_, indices) = ModelPreviewCommand.FlattenGeosets(
            new Model3D(new[] { g }, Array.Empty<string>()));
        Assert.Equal(new[] { 0, 1, 3 }, indices);
    }

    [Fact]
    public void ProjectOrthographic_matches_fixed_34_view()
    {
        // Up axis (+Z): unaffected by yaw, scaled by cos(pitch) on screen-up.
        // +X: yawed 45° into (cos45, sin45), then screen-up picks up -sin45·sin30.
        var xy = ModelPreviewCommand.ProjectOrthographic(new float[]
        {
            0f, 0f, 0f,
            0f, 0f, 1f,
            1f, 0f, 0f,
        });
        float c45 = MathF.Cos(45f * MathF.PI / 180f);
        Assert.Equal(0f, xy[0], 4f);                    // origin → (0, 0)
        Assert.Equal(0f, xy[1], 4f);
        Assert.Equal(0f, xy[2], 4f);                    // +Z → (0, cos30)
        Assert.Equal(MathF.Cos(30f * MathF.PI / 180f), xy[3], 4f);
        Assert.Equal(c45, xy[4], 4f);                   // +X → (cos45, -sin45·sin30)
        Assert.Equal(-c45 * 0.5f, xy[5], 4f);
    }

    [Fact]
    public void ProjectOrthographic_is_linear_no_perspective()
    {
        var one = ModelPreviewCommand.ProjectOrthographic(new float[] { 1f, 2f, 3f });
        var two = ModelPreviewCommand.ProjectOrthographic(new float[] { 2f, 4f, 6f });
        Assert.Equal(one[0] * 2f, two[0], 4f);
        Assert.Equal(one[1] * 2f, two[1], 4f);
    }

    [Fact]
    public void ScaleToFit_maps_bounds_into_padded_canvas_preserving_aspect()
    {
        var xy = new float[] { 0f, 0f, 10f, 5f };
        Assert.True(ModelPreviewCommand.ScaleToFit(xy, 100, 100, 0.05f));
        // pad = 5, avail = 90; extents (10, 5) → uniform scale 9; center (5, 2.5) → (50, 50).
        Assert.Equal(5f, xy[0], 3f);     // (0,0) → x = 50 + (0-5)·9
        Assert.Equal(72.5f, xy[1], 3f);  //          y = 50 - (0-2.5)·9 (Y flipped)
        Assert.Equal(95f, xy[2], 3f);    // (10,5) → x = 50 + (10-5)·9
        Assert.Equal(27.5f, xy[3], 3f);  //          y = 50 - (5-2.5)·9
    }

    [Fact]
    public void ScaleToFit_zero_extent_collapses_to_center()
    {
        var xy = new float[] { 3f, 7f, 3f, 7f };
        Assert.True(ModelPreviewCommand.ScaleToFit(xy, 100, 60, 0.05f));
        Assert.Equal(50f, xy[0], 3f);
        Assert.Equal(30f, xy[1], 3f);
        Assert.Equal(50f, xy[2], 3f);
        Assert.Equal(30f, xy[3], 3f);
    }

    [Fact]
    public void ScaleToFit_returns_false_when_nothing_is_finite()
    {
        var xy = new float[] { float.NaN, 1f, float.PositiveInfinity, float.NegativeInfinity };
        Assert.False(ModelPreviewCommand.ScaleToFit(xy, 100, 100, 0.05f));
    }
}
