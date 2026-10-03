// tests/Wc3.Tests/ModelRendererTests.cs
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Wc3.Modeling;
using Wc3.Render;

namespace Wc3.Tests;

public class ModelRendererTests
{
    private static readonly byte[] PngSignature = { 0x89, (byte)'P', (byte)'N', (byte)'G', 0x0D, 0x0A, 0x1A, 0x0A };

    private static readonly IReadOnlyDictionary<int, TextureImage> NoTextures =
        new Dictionary<int, TextureImage>();

    /// <summary>A unit tetrahedron: four triangles, so every view angle sees geometry.</summary>
    private static Model3D Tetrahedron()
    {
        var vertices = new float[]
        {
            0f, 0f, 1f,
            1f, 0f, -1f,
            -1f, 1f, -1f,
            -1f, -1f, -1f,
        };
        // Radial normals are good enough for shading coverage.
        var normals = new float[vertices.Length];
        for (int i = 0; i < vertices.Length; i += 3)
        {
            float len = MathF.Sqrt(
                vertices[i] * vertices[i] + vertices[i + 1] * vertices[i + 1] + vertices[i + 2] * vertices[i + 2]);
            normals[i] = vertices[i] / len;
            normals[i + 1] = vertices[i + 1] / len;
            normals[i + 2] = vertices[i + 2] / len;
        }
        var uvs = new float[] { 0.5f, 0.5f, 0f, 1f, 1f, 1f, 0.5f, 0f };
        var indices = new[] { 0, 1, 2, 0, 2, 3, 0, 3, 1, 1, 3, 2 };
        return new Model3D(
            new[] { new Geoset(vertices, normals, uvs, indices, 0) },
            new[] { "some/texture.blp" });
    }

    [Fact]
    public void Renders_untextured_geometry_to_visible_pixels()
    {
        var png = ModelRenderer.RenderPng(Tetrahedron(), NoTextures, 128, 128);

        Assert.Equal(PngSignature, png[..8]);
        using var image = Image.Load<Rgba32>(png);
        Assert.Equal(128, image.Width);
        Assert.Equal(128, image.Height);

        int opaque = 0, transparent = 0;
        for (int y = 0; y < image.Height; y++)
            for (int x = 0; x < image.Width; x++)
            {
                if (image[x, y].A == 255) opaque++;
                else if (image[x, y].A == 0) transparent++;
            }
        Assert.True(opaque > 100, $"expected a visible model, got {opaque} opaque pixels");
        Assert.True(transparent > 100, $"expected a transparent background, got {transparent} transparent pixels");
    }

    [Fact]
    public void Samples_texture_colors_when_texture_resolves()
    {
        // A solid red 2x2 texture: every lit pixel must carry only red.
        var red = new byte[2 * 2 * 4];
        for (int i = 0; i < red.Length; i += 4) { red[i] = 255; red[i + 3] = 255; }
        var textures = new Dictionary<int, TextureImage> { [0] = new TextureImage(2, 2, red) };

        using var image = Image.Load<Rgba32>(ModelRenderer.RenderPng(Tetrahedron(), textures, 64, 64));

        bool sawRed = false;
        for (int y = 0; y < image.Height; y++)
            for (int x = 0; x < image.Width; x++)
            {
                var p = image[x, y];
                if (p.A == 0) continue;
                Assert.True(p.G == 0 && p.B == 0, $"({x},{y}) has non-red channels {p}");
                if (p.R > 0) sawRed = true;
            }
        Assert.True(sawRed, "no red-textured pixel rendered");
    }

    [Fact]
    public void Throws_on_model_without_geometry()
    {
        var empty = new Model3D(Array.Empty<Geoset>(), Array.Empty<string>());
        var ex = Assert.Throws<InvalidDataException>(() => ModelRenderer.RenderPng(empty, NoTextures));
        Assert.Contains("geometry", ex.Message);
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Renders_map_imported_model_end_to_end()
    {
        string map = CorpusMap.PathOrEmpty;
        if (!File.Exists(map)) return; // corpus-optional

        // Find a model in whatever map resolved, rather than naming one. Naming one tied this
        // test to a single file, and when that file went away the test stopped running instead of
        // failing, which is how it stayed green while proving nothing.
        var doc = Wc3.Model.MapDocument.Load(map);
        // Asked for as .mdl though the archive stores .mdx, which is how object data names a
        // model and therefore exercises the extension-swap lookup as well as geometry and textures.
        var asMdl = CorpusSubject.LargestModelAsMdl(doc);
        if (asMdl is null) return;   // a map with no imported models proves nothing here

        var png = Wc3.Commands.RenderModelCommand.Execute(doc, asMdl);

        Assert.Equal(PngSignature, png[..8]);
        Assert.True(png.Length > 10_000, $"suspiciously small render ({png.Length} bytes)");

        using var image = Image.Load<Rgba32>(png);
        Assert.Equal(512, image.Width);
        Assert.Equal(512, image.Height);
        int opaque = 0;
        for (int y = 0; y < image.Height; y++)
            for (int x = 0; x < image.Width; x++)
                if (image[x, y].A == 255) opaque++;
        Assert.True(opaque > 5_000, $"expected a substantial model silhouette, got {opaque} opaque pixels");
    }
}
