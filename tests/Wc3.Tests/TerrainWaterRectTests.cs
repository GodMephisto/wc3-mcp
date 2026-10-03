// tests/Wc3.Tests/TerrainWaterRectTests.cs
using War3Net.Build.Environment;
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// WaterRect was the one bulk terrain operation with ZERO test references and ZERO callers
/// outside Wc3.Commands. Its six siblings all carry coverage, so it is the only one of the
/// eight that is both unreachable and unverified, and exposing it through a front-end
/// without a test would be shipping unmeasured code to a user's map.
///
/// These pin the behaviour its siblings already promise: the rectangle is inclusive, corners
/// may be given in any order, tiles outside are untouched, an off-grid region is a refusal
/// rather than a throw, and the change survives a save and reload.
/// </summary>
public class TerrainWaterRectTests
{
    private const int TileEdge = 8;
    private const int W = TileEdge + 1;
    private static int Idx(int x, int y) => y * W + x;

    private static MapDocument BlankTerrain() => BlankMap.Create(new BlankMapOptions { TileEdge = TileEdge });

    private static MapEnvironment Env(MapDocument doc)
        => (MapEnvironment)doc.GetFile(TerrainCommand.TerrainFile)!.Model!;

    private static MapEnvironment Reload(MapDocument doc)
        => Env(MapDocument.Load(doc.SaveToBytes()));

    [Fact]
    public void Set_flags_every_tile_in_the_inclusive_rectangle_and_nothing_outside()
    {
        var doc = BlankTerrain();
        var r = TerrainCommand.WaterRect(doc, 2, 2, 4, 4, TerrainCommand.WaterOp.Set, 1.5f);

        Assert.True(r.Ok, r.Message);
        // Inclusive on both corners, so 3x3 corners, not 2x2.
        Assert.Equal(9, r.TilesChanged);

        var env = Env(doc);
        for (int y = 2; y <= 4; y++)
            for (int x = 2; x <= 4; x++)
                Assert.True(env.TerrainTiles[Idx(x, y)].IsWater, $"({x},{y}) should be water");

        Assert.False(env.TerrainTiles[Idx(1, 2)].IsWater);
        Assert.False(env.TerrainTiles[Idx(5, 4)].IsWater);
        Assert.False(env.TerrainTiles[Idx(2, 1)].IsWater);
        Assert.False(env.TerrainTiles[Idx(4, 5)].IsWater);
    }

    [Fact]
    public void Corners_may_be_given_in_any_order()
    {
        var forward = BlankTerrain();
        var reversed = BlankTerrain();

        var a = TerrainCommand.WaterRect(forward, 1, 1, 5, 3, TerrainCommand.WaterOp.Set, 2f);
        var b = TerrainCommand.WaterRect(reversed, 5, 3, 1, 1, TerrainCommand.WaterOp.Set, 2f);

        Assert.True(a.Ok);
        Assert.True(b.Ok);
        Assert.Equal(a.TilesChanged, b.TilesChanged);

        var ea = Env(forward);
        var eb = Env(reversed);
        for (int i = 0; i < ea.TerrainTiles.Count; i++)
            Assert.Equal(ea.TerrainTiles[i].IsWater, eb.TerrainTiles[i].IsWater);
    }

    [Fact]
    public void Remove_clears_the_water_flag_it_previously_set()
    {
        var doc = BlankTerrain();
        TerrainCommand.WaterRect(doc, 0, 0, 3, 3, TerrainCommand.WaterOp.Set, 1f);
        Assert.True(Env(doc).TerrainTiles[Idx(1, 1)].IsWater);

        var r = TerrainCommand.WaterRect(doc, 0, 0, 3, 3, TerrainCommand.WaterOp.Remove);
        Assert.True(r.Ok, r.Message);
        Assert.False(Env(doc).TerrainTiles[Idx(1, 1)].IsWater);
    }

    [Fact]
    public void A_region_entirely_off_grid_reports_zero_changed_rather_than_throwing()
    {
        var doc = BlankTerrain();
        var r = TerrainCommand.WaterRect(doc, 50, 50, 60, 60, TerrainCommand.WaterOp.Set, 1f);

        // Ok, because asking to flood nothing is not an error, it just does nothing.
        Assert.True(r.Ok, r.Message);
        Assert.Equal(0, r.TilesChanged);
        Assert.Contains("off-grid", r.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void A_region_straddling_the_edge_is_clipped_to_the_grid()
    {
        var doc = BlankTerrain();
        // x from 6 to 20 clips to 6..8, y from 6 to 20 clips to 6..8, so 3x3.
        var r = TerrainCommand.WaterRect(doc, 6, 6, 20, 20, TerrainCommand.WaterOp.Set, 1f);

        Assert.True(r.Ok, r.Message);
        Assert.Equal(9, r.TilesChanged);
        Assert.True(Env(doc).TerrainTiles[Idx(8, 8)].IsWater);
    }

    [Fact]
    public void The_water_flag_survives_a_save_and_reload()
    {
        var doc = BlankTerrain();
        TerrainCommand.WaterRect(doc, 3, 3, 5, 5, TerrainCommand.WaterOp.Set, 2.25f);

        var reloaded = Reload(doc);
        for (int y = 3; y <= 5; y++)
            for (int x = 3; x <= 5; x++)
                Assert.True(reloaded.TerrainTiles[Idx(x, y)].IsWater,
                    $"({x},{y}) lost its water flag through save and reload");
        Assert.False(reloaded.TerrainTiles[Idx(2, 3)].IsWater);
    }
}
