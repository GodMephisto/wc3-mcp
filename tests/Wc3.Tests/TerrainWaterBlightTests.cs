using War3Net.Build.Environment; // MapEnvironment
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// Round-trip tests for the water-height and blight area brushes
/// (<see cref="TerrainCommand.Water"/> / <see cref="TerrainCommand.Blight"/>).
/// Uses the same 8×8-tile (9×9 corner) blank map as the height-brush tests.
/// </summary>
public class TerrainWaterBlightTests
{
    private const int TileEdge = 8;
    private const int W = TileEdge + 1;                 // 9 corner tiles per side

    private static int Idx(int x, int y) => y * W + x;
    private static MapDocument BlankTerrain() => BlankMap.Create(new BlankMapOptions { TileEdge = TileEdge });
    private static MapEnvironment Env(MapDocument doc)
        => (MapEnvironment)doc.GetFile(TerrainCommand.TerrainFile)!.Model!;

    // ---- Water --------------------------------------------------------------

    [Fact]
    public void Water_Set_flags_water_and_sets_absolute_height()
    {
        var doc = BlankTerrain();
        var r = TerrainCommand.Water(doc, 4, 4, radius: 2, TerrainCommand.WaterOp.Set, amount: 5f);
        Assert.True(r.Ok, r.Message);
        Assert.True(r.TilesChanged > 0);

        var env = Env(doc);
        Assert.True(env.TerrainTiles[Idx(4, 4)].IsWater);
        Assert.Equal(5f, env.TerrainTiles[Idx(4, 4)].WaterHeight);

        // Far corner untouched.
        Assert.False(env.TerrainTiles[Idx(0, 0)].IsWater);
    }

    [Fact]
    public void Water_Set_Raise_Lower_round_trip_is_exact()
    {
        var doc = BlankTerrain();
        TerrainCommand.Water(doc, 4, 4, 0, TerrainCommand.WaterOp.Set, 4f);   // -> 4
        TerrainCommand.Water(doc, 4, 4, 0, TerrainCommand.WaterOp.Raise, 3f); // -> 7
        TerrainCommand.Water(doc, 4, 4, 0, TerrainCommand.WaterOp.Lower, 7f); // -> 0

        var env = Env(doc);
        Assert.Equal(0f, env.TerrainTiles[Idx(4, 4)].WaterHeight);
        Assert.True(env.TerrainTiles[Idx(4, 4)].IsWater); // Lower keeps the water flag
    }

    [Fact]
    public void Water_Remove_clears_flag_and_preserves_height()
    {
        var doc = BlankTerrain();
        TerrainCommand.Water(doc, 4, 4, 0, TerrainCommand.WaterOp.Set, 6f);
        var r = TerrainCommand.Water(doc, 4, 4, 0, TerrainCommand.WaterOp.Remove);
        Assert.True(r.Ok, r.Message);

        var t = Env(doc).TerrainTiles[Idx(4, 4)];
        Assert.False(t.IsWater);
        Assert.Equal(6f, t.WaterHeight); // stored height left untouched
    }

    [Fact]
    public void Water_circle_footprint_excludes_diagonal_corner()
    {
        var doc = BlankTerrain();
        TerrainCommand.Water(doc, 4, 4, radius: 2, TerrainCommand.WaterOp.Set, 3f,
            TerrainCommand.BrushShape.Circle);

        var env = Env(doc);
        Assert.True(env.TerrainTiles[Idx(4, 6)].IsWater);   // straight edge, dist 2 -> in
        Assert.False(env.TerrainTiles[Idx(2, 2)].IsWater);  // diagonal, dist ~2.83 -> out
    }

    [Fact]
    public void Water_off_grid_reports_zero_changed()
    {
        var doc = BlankTerrain();
        var r = TerrainCommand.Water(doc, 999, 999, 1, TerrainCommand.WaterOp.Set, 1f);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(0, r.TilesChanged);
    }

    [Fact]
    public void Water_negative_radius_rejected()
        => Assert.False(TerrainCommand.Water(BlankTerrain(), 4, 4, -1, TerrainCommand.WaterOp.Set, 1f).Ok);

    // ---- Blight -------------------------------------------------------------

    [Fact]
    public void Blight_on_sets_flag_and_off_clears_it()
    {
        var doc = BlankTerrain();
        var on = TerrainCommand.Blight(doc, 4, 4, radius: 2, on: true);
        Assert.True(on.Ok, on.Message);
        Assert.True(on.TilesChanged > 0);
        Assert.True(Env(doc).TerrainTiles[Idx(4, 4)].IsBlighted);

        var off = TerrainCommand.Blight(doc, 4, 4, radius: 2, on: false);
        Assert.True(off.Ok, off.Message);
        Assert.False(Env(doc).TerrainTiles[Idx(4, 4)].IsBlighted);
    }

    [Fact]
    public void Blight_rect_covers_region_and_leaves_outside_clear()
    {
        var doc = BlankTerrain();
        TerrainCommand.BlightRect(doc, 0, 0, 2, 2, on: true);

        var env = Env(doc);
        Assert.True(env.TerrainTiles[Idx(1, 1)].IsBlighted);  // inside
        Assert.False(env.TerrainTiles[Idx(5, 5)].IsBlighted); // outside
    }

    [Fact]
    public void Blight_circle_footprint_excludes_diagonal_corner()
    {
        var doc = BlankTerrain();
        TerrainCommand.Blight(doc, 4, 4, radius: 2, on: true, TerrainCommand.BrushShape.Circle);

        var env = Env(doc);
        Assert.True(env.TerrainTiles[Idx(6, 4)].IsBlighted);  // straight edge -> in
        Assert.False(env.TerrainTiles[Idx(2, 2)].IsBlighted); // diagonal -> out
    }

    [Fact]
    public void Blight_re_applying_same_state_changes_nothing()
    {
        var doc = BlankTerrain();
        TerrainCommand.Blight(doc, 4, 4, 2, on: true);
        var again = TerrainCommand.Blight(doc, 4, 4, 2, on: true);
        Assert.True(again.Ok, again.Message);
        Assert.Equal(0, again.TilesChanged); // already blighted -> idempotent
    }

    [Fact]
    public void Blight_negative_radius_rejected()
        => Assert.False(TerrainCommand.Blight(BlankTerrain(), 4, 4, -1, on: true).Ok);
}
