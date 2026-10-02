// tests/Wc3.Tests/TerrainRendererTests.cs
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Wc3.Model;
using Wc3.Render;

namespace Wc3.Tests;

public class TerrainRendererTests
{
    private static readonly byte[] PngSignature = { 0x89, (byte)'P', (byte)'N', (byte)'G', 0x0D, 0x0A, 0x1A, 0x0A };

    [Fact]
    public void Throws_when_map_has_no_terrain()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = new byte[] { 1, 2 },
        }));
        var ex = Assert.Throws<InvalidDataException>(() => TerrainRenderer.RenderTerrainPng(doc));
        Assert.Contains("war3map.w3e", ex.Message);
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Renders_real_map_terrain_to_png()
    {
        string path = TestCorpus.Map(@"ggg_en_1.11r_slk.w3x");
        if (!File.Exists(path)) return;

        var png = TerrainRenderer.RenderTerrainPng(MapDocument.Load(path));

        Assert.True(png.Length > PngSignature.Length);
        Assert.Equal(PngSignature, png[..8]);

        // IHDR width/height (big-endian) sit at bytes 16..23 of any valid PNG.
        int width = (png[16] << 24) | (png[17] << 16) | (png[18] << 8) | png[19];
        int height = (png[20] << 24) | (png[21] << 16) | (png[22] << 8) | png[23];
        Assert.True(width > 0 && height > 0, $"IHDR dims {width}x{height} should be positive");
        // ggg is a square 257x257-tilepoint map, so the render must be square too.
        Assert.Equal(width, height);

        // Not grayscale: decode the render and require chromatic variety.
        // A grayscale image has R==G==B for every pixel; a colored terrain
        // render (grass/water/dirt/cliffs) must have many pixels whose channels
        // differ, plus several distinct colors covering the ground types.
        using var img = Image.Load<Rgba32>(png);
        long chromatic = 0;
        var distinct = new HashSet<int>();
        for (int y = 0; y < img.Height; y++)
            for (int x = 0; x < img.Width; x++)
            {
                Rgba32 p = img[x, y];
                if (p.R != p.G || p.G != p.B) chromatic++;
                distinct.Add((p.R << 16) | (p.G << 8) | p.B);
            }

        long total = (long)img.Width * img.Height;
        Assert.True(chromatic > total / 20,
            $"expected a colored render, but only {chromatic} of {total} pixels were chromatic (grayscale regression?)");
        Assert.True(distinct.Count >= 8,
            $"expected varied terrain colors, but got only {distinct.Count} distinct colors");
    }
}
