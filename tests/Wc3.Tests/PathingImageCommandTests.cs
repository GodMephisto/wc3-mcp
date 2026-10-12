// tests/Wc3.Tests/PathingImageCommandTests.cs
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using War3Net.Build.Environment; // MapPathingMap, PathingType
using Wc3.Commands;
using Wc3.Model;
using Wc3.Render;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// The pathing PNG pair. Export renders the war3map.wpm grid colour-coded with a legend,
/// and import reads a colour-coded image back, refusing an image smaller than the grid and
/// any colour the palette does not produce.
/// </summary>
public class PathingImageCommandTests
{
    private const int TileEdge = 8;   // blank wpm is TileEdge*4 = 32 cells per side

    private static MapDocument Blank() => BlankMap.Create(new BlankMapOptions { TileEdge = TileEdge });
    private static MapPathingMap Map(MapDocument d) => (MapPathingMap)d.GetFile(PathingCommand.PathingFile)!.Model!;

    [Fact]
    public void Export_writes_a_png_with_a_legend_of_the_flags()
    {
        var doc = Blank();
        var r = PathingExportPngCommand.Export(doc);

        Assert.True(r.Ok, r.Message);
        Assert.NotNull(r.Png);
        Assert.Equal((int)Map(doc).Width, r.Width);
        Assert.Equal((int)Map(doc).Height, r.Height);

        Assert.NotNull(r.Legend);
        var names = r.Legend!.Select(l => l.Name).ToList();
        Assert.Contains("none", names);
        Assert.Contains("Walk", names);
        Assert.Contains("Water", names);
    }

    [Fact]
    public void Export_is_read_only()
    {
        var doc = Blank();
        PathingCommand.Paint(doc, 8, 8, 2, PathingType.Walk);
        var before = Map(doc).Cells.ToList();

        var r = PathingExportPngCommand.Export(doc);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(before, Map(doc).Cells);
    }

    [Fact]
    public void The_exported_map_region_decodes_to_the_same_cells()
    {
        var doc = Blank();
        PathingCommand.Paint(doc, 8, 8, 2, PathingType.Walk | PathingType.Build);
        PathingCommand.Paint(doc, 20, 20, 1, PathingType.Water);

        var r = PathingExportPngCommand.Export(doc);
        Assert.True(r.Ok, r.Message);

        var map = Map(doc);
        int w = (int)map.Width, h = (int)map.Height;
        Assert.True(PathingPng.TryDecodeRegion(r.Png!, w, h, out var cells, out var error), error);
        Assert.Equal(map.Cells, cells);
    }

    [Fact]
    public void Import_round_trips_the_flags_through_a_png()
    {
        var src = Blank();
        PathingCommand.Paint(src, 8, 8, 2, PathingType.Walk | PathingType.Build);
        PathingCommand.Paint(src, 20, 20, 1, PathingType.Water);
        var png = PathingExportPngCommand.Export(src).Png!;

        var dst = Blank();
        var r = PathingImportPngCommand.Import(dst, png);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(Map(src).Cells, Map(dst).Cells);
    }

    [Fact]
    public void Import_refuses_an_unknown_colour()
    {
        var doc = Blank();
        int w = (int)Map(doc).Width, h = (int)Map(doc).Height;
        using var image = new Image<Rgba32>(w, h);
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                image[x, y] = new Rgba32(0, 0, 0);
        image[1, 1] = new Rgba32(255, 0, 255);   // not a palette colour

        using var ms = new MemoryStream();
        image.SaveAsPng(ms);

        var r = PathingImportPngCommand.Import(doc, ms.ToArray());
        Assert.False(r.Ok);
        Assert.Contains("unknown pathing colour", r.Message);
    }

    [Fact]
    public void Import_refuses_an_image_smaller_than_the_grid()
    {
        var doc = Blank();
        using var image = new Image<Rgba32>(1, 1);
        image[0, 0] = new Rgba32(0, 0, 0);
        using var ms = new MemoryStream();
        image.SaveAsPng(ms);

        var r = PathingImportPngCommand.Import(doc, ms.ToArray());
        Assert.False(r.Ok);
    }

    [Fact]
    public void Export_refuses_a_map_with_no_pathing_map()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = new byte[] { 0x2F, 0x2F },
        }));
        var r = PathingExportPngCommand.Export(doc);
        Assert.False(r.Ok);
    }
}
