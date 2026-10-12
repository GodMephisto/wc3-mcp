// tests/Wc3.Tests/McpToolSmokeTests.TerrainImages.cs
// The terrain heightmap PNG pair, map-wide smoothing, and the pathing PNG pair, exercised
// through the protocol. Export tools write an image, so they cannot use the map-opening
// Write helper, while import tools write a map and do.
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public sealed class McpSmokeTerrainImageTests : McpSmokeBase
{
    public McpSmokeTerrainImageTests(SmokeFixture f) : base(f) { }

    [Fact, Covers("terrain_export_heightmap", "terrain_import_heightmap")]
    public async Task Heightmap_round_trips_through_mcp()
    {
        var spiked = await Write("terrain_deform",
            Args(("center_x", 16), ("center_y", 16), ("radius", 0), ("op", "set"), ("amount", 40f)));

        var png = Path.Combine(_f.Dir, $"hm-{Guid.NewGuid():N}.png");
        var export = await Call("terrain_export_heightmap", Args(("map", spiked), ("out_path", png)));
        Assert.True(File.Exists(png));
        Assert.True(Prop(export, "sizeBytes").GetInt32() > 0);

        var imported = await Write("terrain_import_heightmap", Args(("png", png)));
        var from = await Call("terrain_corner_get", Args(("map", spiked), ("col", 16), ("row", 16)));
        var to = await Call("terrain_corner_get", Args(("map", imported), ("col", 16), ("row", 16)));
        Assert.Equal(Prop(from, "groundHeight").GetSingle(), Prop(to, "groundHeight").GetSingle(), 2);
    }

    [Fact, Covers("terrain_import_heightmap")]
    public async Task Heightmap_import_refuses_a_mismatched_png()
    {
        // A 1x1 PNG cannot match the fixture's 33x33 corner grid.
        var tiny = Path.Combine(_f.Dir, $"tiny-{Guid.NewGuid():N}.png");
        using (var image = new Image<Rgba32>(1, 1))
            image.SaveAsPng(tiny);

        await CallError("terrain_import_heightmap",
            Args(("map", Fixture), ("out_path", NewOut("hm")), ("png", tiny)));
    }

    [Fact, Covers("terrain_smooth")]
    public async Task Terrain_smooth_lowers_a_spike()
    {
        var spiked = await Write("terrain_deform",
            Args(("center_x", 16), ("center_y", 16), ("radius", 0), ("op", "set"), ("amount", 40f)));
        var smoothed = await Write("terrain_smooth", Args(("passes", 1)), map: spiked);

        var s = await Call("terrain_corner_get", Args(("map", smoothed), ("col", 16), ("row", 16)));
        Assert.True(Prop(s, "groundHeight").GetSingle() < 40f);
    }

    [Fact, Covers("pathing_export_png", "pathing_import_png")]
    public async Task Pathing_png_round_trips_through_mcp()
    {
        var painted = await Write("pathing_paint",
            Args(("center_x", 20), ("center_y", 20), ("radius", 3), ("flags", "Walk")));

        var png = Path.Combine(_f.Dir, $"wpm-{Guid.NewGuid():N}.png");
        var export = await Call("pathing_export_png", Args(("map", painted), ("out_path", png)));
        Assert.True(File.Exists(png));
        Assert.True(Prop(export, "legend").GetArrayLength() > 0);

        var imported = await Write("pathing_import_png", Args(("png", png)), map: Fixture);
        Assert.True(MapDocument.Load(painted).TryReadFileByName(PathingCommand.PathingFile, out var a));
        Assert.True(MapDocument.Load(imported).TryReadFileByName(PathingCommand.PathingFile, out var b));
        Assert.Equal(a, b);
    }

    [Fact, Covers("pathing_import_png")]
    public async Task Pathing_import_refuses_an_unknown_colour()
    {
        int w = (int)((War3Net.Build.Environment.MapPathingMap)MapDocument
            .Load(Fixture).GetFile(PathingCommand.PathingFile)!.Model!).Width;
        var bad = Path.Combine(_f.Dir, $"bad-{Guid.NewGuid():N}.png");
        using (var image = new Image<Rgba32>(w, w))
        {
            for (int y = 0; y < w; y++)
                for (int x = 0; x < w; x++)
                    image[x, y] = new Rgba32(0, 0, 0);
            image[0, 0] = new Rgba32(255, 0, 255);
            image.SaveAsPng(bad);
        }

        await CallError("pathing_import_png",
            Args(("map", Fixture), ("out_path", NewOut("wpm")), ("png", bad)));
    }
}
