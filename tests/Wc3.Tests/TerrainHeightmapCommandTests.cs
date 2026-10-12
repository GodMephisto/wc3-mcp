// tests/Wc3.Tests/TerrainHeightmapCommandTests.cs
using War3Net.Build.Environment; // MapEnvironment
using Wc3.Commands;
using Wc3.Model;
using Wc3.Render;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// Export writes the terrain heights as a 16-bit grayscale PNG and import reads one back.
/// The pair has to be an exact inverse of the map's corner grid, so these pin the round
/// trip, the dimension guard and the fact that only ground height is written.
/// </summary>
public class TerrainHeightmapCommandTests
{
    private const int TileEdge = 8;
    private const int W = TileEdge + 1;   // corners per side
    private static int Idx(int x, int y) => y * W + x;

    private static MapDocument Blank() => BlankMap.Create(new BlankMapOptions { TileEdge = TileEdge });
    private static MapEnvironment Env(MapDocument d) => (MapEnvironment)d.GetFile(TerrainCommand.TerrainFile)!.Model!;

    [Fact]
    public void Export_writes_a_png_the_size_of_the_corner_grid()
    {
        var r = TerrainExportHeightmapCommand.Export(Blank());
        Assert.True(r.Ok, r.Message);
        Assert.NotNull(r.Png);
        Assert.Equal(W, r.Width);
        Assert.Equal(W, r.Height);

        var decoded = HeightmapPng.Decode(r.Png!);
        Assert.Equal(W, decoded.Width);
        Assert.Equal(W, decoded.Height);
    }

    [Fact]
    public void Export_is_read_only()
    {
        var doc = Blank();
        TerrainCommand.Deform(doc, 4, 4, 1, TerrainCommand.HeightOp.Set, 7f, TerrainCommand.BrushShape.Square);
        float before = Env(doc).TerrainTiles[Idx(4, 4)].Height;

        var r = TerrainExportHeightmapCommand.Export(doc);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(before, Env(doc).TerrainTiles[Idx(4, 4)].Height);
    }

    [Fact]
    public void Import_round_trips_the_heights_through_a_png()
    {
        var src = Blank();
        TerrainCommand.Deform(src, 4, 4, 2, TerrainCommand.HeightOp.Set, 5f, TerrainCommand.BrushShape.Square);

        var exported = TerrainExportHeightmapCommand.Export(src);
        Assert.True(exported.Ok, exported.Message);

        var dst = Blank();
        var imported = TerrainImportHeightmapCommand.Import(dst, exported.Png!);
        Assert.True(imported.Ok, imported.Message);

        var srcTiles = Env(src).TerrainTiles;
        var dstTiles = Env(dst).TerrainTiles;
        for (int i = 0; i < srcTiles.Count; i++)
            Assert.True(Math.Abs(srcTiles[i].Height - dstTiles[i].Height) <= 1f / 256f + 1e-4f,
                $"corner {i}: {srcTiles[i].Height} vs {dstTiles[i].Height}");
    }

    [Fact]
    public void Import_refuses_a_png_whose_dimensions_do_not_match_the_grid()
    {
        // A heightmap exported from a differently sized map is W+2 per side here.
        var other = BlankMap.Create(new BlankMapOptions { TileEdge = TileEdge + 2 });
        var png = HeightmapPng.Encode(Env(other));

        var r = TerrainImportHeightmapCommand.Import(Blank(), png);
        Assert.False(r.Ok);
        Assert.Contains("grid", r.Message);
    }

    [Fact]
    public void Import_writes_only_the_ground_height()
    {
        var src = Blank();
        TerrainCommand.Cliff(src, 4, 4, 0, TerrainCommand.CliffOp.Set, 3, TerrainCommand.BrushShape.Square);
        var png = HeightmapPng.Encode(Env(src));

        var dst = Blank();
        var r = TerrainImportHeightmapCommand.Import(dst, png);
        Assert.True(r.Ok, r.Message);

        // Cliff level is not carried by a heightmap, so the target keeps its own.
        Assert.Equal(0, Env(dst).TerrainTiles[Idx(4, 4)].CliffLevel);
    }

    [Fact]
    public void Import_of_an_identical_grid_changes_nothing()
    {
        var doc = Blank();
        var png = HeightmapPng.Encode(Env(doc));
        var r = TerrainImportHeightmapCommand.Import(doc, png);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(0, r.TilesChanged);
    }

    [Fact]
    public void Export_refuses_a_map_with_no_terrain()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = new byte[] { 0x2F, 0x2F },
        }));
        var r = TerrainExportHeightmapCommand.Export(doc);
        Assert.False(r.Ok);
    }
}
