// tests/Wc3.Tests/TerrainPerspectiveProbeTests.cs
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Wc3.Commands;
using Wc3.Model;
using Wc3.Render;

namespace Wc3.Tests;

/// <summary>
/// The 3D perspective terrain renderer (the "rendered in game" view) must produce a
/// valid, non-degenerate PNG: correct dimensions, PNG signature, actual lit terrain
/// pixels (not pure sky), and a genuine perspective camera — tilting the pitch must
/// change the projection (a flat 2D blit would be pitch-invariant).
/// </summary>
public class TerrainPerspectiveProbeTests
{
    [Fact]
    public void RenderPerspective_produces_a_valid_lit_terrain_image()
    {
        var doc = BlankMap.Create(new BlankMapOptions { TileEdge = 8 });

        byte[] png = TerrainRenderer.RenderPerspectivePng(
            doc, width: 320, height: 240, yawDegrees: 45f, pitchDegrees: 30f, zoom: 1f);

        Assert.True(png.Length > 200, $"expected a real PNG, got {png.Length} bytes");
        Assert.Equal(0x89, png[0]);          // PNG signature
        Assert.Equal((byte)'P', png[1]);

        using var img = Image.Load<Rgba32>(png);
        Assert.Equal(320, img.Width);
        Assert.Equal(240, img.Height);

        // The mesh must actually fill pixels — count ones that aren't the sky clear colour.
        int nonSky = 0;
        var seen = new HashSet<int>();
        img.ProcessPixelRows(rows =>
        {
            for (int y = 0; y < rows.Height; y++)
            {
                var row = rows.GetRowSpan(y);
                for (int x = 0; x < row.Length; x++)
                {
                    var p = row[x];
                    if (!(p.R == 28 && p.G == 32 && p.B == 38)) nonSky++;
                    seen.Add((p.R << 16) | (p.G << 8) | p.B);
                }
            }
        });

        Assert.True(nonSky > 1000, $"expected lit terrain pixels, got {nonSky} non-sky");
        Assert.True(seen.Count >= 2, $"expected sky + terrain colours, got {seen.Count}");
    }

    [Fact]
    public void RenderPerspective_camera_pitch_changes_the_projection()
    {
        var doc = BlankMap.Create(new BlankMapOptions { TileEdge = 8 });
        var low = TerrainRenderer.RenderPerspectivePng(doc, 200, 160, pitchDegrees: 15f);
        var high = TerrainRenderer.RenderPerspectivePng(doc, 200, 160, pitchDegrees: 80f);
        Assert.False(low.AsSpan().SequenceEqual(high), "pitch should change the perspective projection");
    }

    [Fact]
    public void RenderPerspective_rejects_a_map_with_no_terrain()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        { ["war3map.j"] = new byte[] { 1 } }));
        var ex = Assert.Throws<InvalidDataException>(() =>
            TerrainRenderer.RenderPerspectivePng(doc, 64, 64));
        Assert.Contains("no terrain", ex.Message);
    }
}
