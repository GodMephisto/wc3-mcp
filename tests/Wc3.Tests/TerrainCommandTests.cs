// tests/Wc3.Tests/TerrainCommandTests.cs
using War3Net.Build.Environment; // MapEnvironment
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

public class TerrainCommandTests
{
    private const int TileEdge = 8;         // 8×8 tiles → 9×9 = 81 corner tiles
    private const int W = TileEdge + 1;     // grid width in corners
    private static int Idx(int x, int y) => y * W + x;

    private static MapDocument BlankTerrain() => BlankMap.Create(new BlankMapOptions { TileEdge = TileEdge });

    private static MapEnvironment Env(MapDocument doc)
        => (MapEnvironment)doc.GetFile(TerrainCommand.TerrainFile)!.Model!;

    private static MapEnvironment Reload(MapDocument doc)
        => Env(MapDocument.Load(doc.SaveToBytes()));

    // BlankMap.Create emits a flat grid at a fixed default height (not necessarily 0),
    // so tests read the baseline from an untouched tile instead of hardcoding a constant.
    private static float Baseline() => Env(BlankTerrain()).TerrainTiles[Idx(0, 0)].Height;

    // --- raise / lower round-trips ------------------------------------------

    [Fact]
    public void Raise_persists_through_save_and_leaves_outside_untouched()
    {
        var doc = BlankTerrain();
        var r = TerrainCommand.Deform(doc, 4, 4, 2, TerrainCommand.HeightOp.Raise, 3f, TerrainCommand.BrushShape.Circle);
        Assert.True(r.Ok, r.Message);
        Assert.True(r.TilesChanged > 0);

        var env = Reload(doc);
        var baseline = Baseline();
        Assert.Equal(baseline + 3f, env.TerrainTiles[Idx(4, 4)].Height); // centre raised
        Assert.Equal(baseline, env.TerrainTiles[Idx(0, 0)].Height);      // far corner untouched
    }

    [Fact]
    public void Lower_subtracts_from_height()
    {
        var doc = BlankTerrain();
        TerrainCommand.Deform(doc, 4, 4, 1, TerrainCommand.HeightOp.Set, 10f, TerrainCommand.BrushShape.Square);
        var r = TerrainCommand.Deform(doc, 4, 4, 0, TerrainCommand.HeightOp.Lower, 4f, TerrainCommand.BrushShape.Square);
        Assert.True(r.Ok, r.Message);

        var env = Reload(doc);
        Assert.Equal(6f, env.TerrainTiles[Idx(4, 4)].Height);   // 10 - 4
    }

    // --- set / footprints ---------------------------------------------------

    [Fact]
    public void Set_square_footprint_covers_the_expected_tile_count()
    {
        var doc = BlankTerrain();
        var r = TerrainCommand.Deform(doc, 4, 4, 1, TerrainCommand.HeightOp.Set, 5f, TerrainCommand.BrushShape.Square);
        Assert.Equal(9, r.TilesChanged);   // (2*1+1)² = 3×3

        var env = Reload(doc);
        Assert.Equal(5f, env.TerrainTiles[Idx(3, 3)].Height);
        Assert.Equal(5f, env.TerrainTiles[Idx(5, 5)].Height);
        Assert.Equal(Baseline(), env.TerrainTiles[Idx(2, 2)].Height); // just outside the 3×3
    }

    [Fact]
    public void Circle_footprint_is_smaller_than_the_bounding_square()
    {
        var square = TerrainCommand.Deform(BlankTerrain(), 4, 4, 2, TerrainCommand.HeightOp.Raise, 1f, TerrainCommand.BrushShape.Square);
        var circle = TerrainCommand.Deform(BlankTerrain(), 4, 4, 2, TerrainCommand.HeightOp.Raise, 1f, TerrainCommand.BrushShape.Circle);
        Assert.Equal(25, square.TilesChanged);  // 5×5
        Assert.Equal(13, circle.TilesChanged);  // dx²+dy² ≤ 4 disc
        Assert.True(circle.TilesChanged < square.TilesChanged);
    }

    // --- flatten / smooth ---------------------------------------------------

    [Fact]
    public void Flatten_levels_a_region_to_its_own_mean()
    {
        var doc = BlankTerrain();
        TerrainCommand.DeformRect(doc, 1, 1, 3, 3, TerrainCommand.HeightOp.Set, 0f); // known flat field
        TerrainCommand.Deform(doc, 2, 2, 0, TerrainCommand.HeightOp.Set, 9f, TerrainCommand.BrushShape.Square); // one spike of 9
        var r = TerrainCommand.DeformRect(doc, 1, 1, 3, 3, TerrainCommand.HeightOp.Flatten);
        Assert.True(r.Ok, r.Message);

        var env = Reload(doc);
        float expected = 9f / 9f; // one tile at 9, eight at 0, over a 3×3 rect
        for (int y = 1; y <= 3; y++)
            for (int x = 1; x <= 3; x++)
                Assert.Equal(expected, env.TerrainTiles[Idx(x, y)].Height, 3);
    }

    [Fact]
    public void Smooth_averages_a_tile_toward_its_neighbours()
    {
        var doc = BlankTerrain();
        TerrainCommand.DeformRect(doc, 3, 3, 5, 5, TerrainCommand.HeightOp.Set, 0f); // known flat neighbourhood
        TerrainCommand.Deform(doc, 4, 4, 0, TerrainCommand.HeightOp.Set, 10f, TerrainCommand.BrushShape.Square); // spike at (4,4)
        var r = TerrainCommand.Deform(doc, 4, 4, 0, TerrainCommand.HeightOp.Smooth, 0f, TerrainCommand.BrushShape.Square);
        Assert.True(r.Ok, r.Message);

        var env = Reload(doc);
        // self(10) + four in-grid neighbours(0) → 10/5 = 2
        Assert.Equal(2f, env.TerrainTiles[Idx(4, 4)].Height, 3);
    }

    // --- rect / bounds / guards ---------------------------------------------

    [Fact]
    public void DeformRect_spanning_the_grid_is_a_full_map_fill()
    {
        var doc = BlankTerrain();
        var r = TerrainCommand.DeformRect(doc, 0, 0, 8, 8, TerrainCommand.HeightOp.Set, 4f);
        Assert.Equal(W * W, r.TilesChanged);
    }

    [Fact]
    public void Region_entirely_off_grid_changes_nothing()
    {
        var r = TerrainCommand.Deform(BlankTerrain(), 100, 100, 2, TerrainCommand.HeightOp.Raise, 1f);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(0, r.TilesChanged);
    }

    [Fact]
    public void Raising_by_zero_changes_nothing()
    {
        var r = TerrainCommand.Deform(BlankTerrain(), 4, 4, 3, TerrainCommand.HeightOp.Raise, 0f);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(0, r.TilesChanged);
    }

    [Fact]
    public void Negative_radius_is_rejected()
    {
        var r = TerrainCommand.Deform(BlankTerrain(), 4, 4, -1, TerrainCommand.HeightOp.Raise, 1f);
        Assert.False(r.Ok);
    }

    // --- stats --------------------------------------------------------------

    [Fact]
    public void Stats_on_a_blank_map_reports_a_flat_grid()
    {
        var s = TerrainCommand.Stats(BlankTerrain());
        Assert.True(s.Ok, s.Message);
        Assert.Equal(W * W, s.TileCount);
        var baseline = Baseline();
        Assert.Equal(baseline, s.MinHeight);   // flat grid: min == max == mean == baseline
        Assert.Equal(baseline, s.MaxHeight);
        Assert.Equal(baseline, s.MeanHeight);
        Assert.Equal(0, s.MinCliff);
        Assert.Equal(0, s.MaxCliff);
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Stats_reads_a_real_map_terrain_range()
    {
        string path = TestCorpus.Map(@"ggg_en_1.11r_slk.w3x");
        if (!File.Exists(path)) return;

        var s = TerrainCommand.Stats(MapDocument.Load(path));
        Assert.True(s.Ok, s.Message);
        // ggg is a 257×257-tilepoint map.
        Assert.Equal(257 * 257, s.TileCount);
        Assert.True(s.MaxHeight >= s.MinHeight, s.Message);
        Assert.True(s.MaxCliff >= s.MinCliff, s.Message);
    }

    // --- texture / tile paint -----------------------------------------------

    // Seed EXACTLY `groundTypes` ground types (clearing the blank map's default Lordaeron
    // palette first) so paint validation, which mirrors the renderer's index-into-TerrainTypes
    // semantics, has a known count; the enum values are arbitrary (any valid TerrainType passes
    // validation and the tile stores a plain byte index).
    private static MapDocument SeededTerrain(int groundTypes = 4)
    {
        var doc = BlankTerrain();
        var env = Env(doc);
        env.TerrainTypes.Clear();
        var all = Enum.GetValues<TerrainType>();
        for (int i = 0; i < groundTypes && i < all.Length; i++)
            env.TerrainTypes.Add(all[i]);
        return doc;
    }

    [Fact]
    public void Paint_with_no_ground_types_is_rejected()
    {
        // No TerrainTypes → a texture index has nothing to reference, matching the renderer's
        // "fall back to neutral colour" behaviour rather than writing garbage. (A default blank
        // map now ships a ground palette, so clear it to exercise the no-types guard.)
        var doc = BlankTerrain();
        Env(doc).TerrainTypes.Clear();
        var r = TerrainCommand.Paint(doc, 4, 4, 1, textureIndex: 0);
        Assert.False(r.Ok);
        Assert.Contains("ground tile-types", r.Message);
    }

    [Fact]
    public void Paint_persists_texture_through_save()
    {
        var doc = SeededTerrain(4);
        var r = TerrainCommand.Paint(doc, 4, 4, 0, textureIndex: 2, shape: TerrainCommand.BrushShape.Square);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(1, r.TilesChanged);

        var env = Reload(doc);
        Assert.Equal(2, env.TerrainTiles[Idx(4, 4)].Texture);
    }

    [Fact]
    public void Paint_square_footprint_covers_the_expected_tile_count()
    {
        var doc = SeededTerrain(4);
        var r = TerrainCommand.Paint(doc, 4, 4, 1, textureIndex: 1, shape: TerrainCommand.BrushShape.Square);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(9, r.TilesChanged);   // (2*1+1)² = 3×3

        var env = Reload(doc);
        Assert.Equal(1, env.TerrainTiles[Idx(3, 3)].Texture);
        Assert.Equal(1, env.TerrainTiles[Idx(5, 5)].Texture);
        Assert.Equal(0, env.TerrainTiles[Idx(2, 2)].Texture); // just outside the 3×3
    }

    [Fact]
    public void Paint_also_sets_variation_when_provided()
    {
        var doc = SeededTerrain(4);
        var r = TerrainCommand.Paint(doc, 4, 4, 0, textureIndex: 1, variation: 3, shape: TerrainCommand.BrushShape.Square);
        Assert.True(r.Ok, r.Message);

        var env = Reload(doc);
        var tile = env.TerrainTiles[Idx(4, 4)];
        Assert.Equal(1, tile.Texture);
        Assert.Equal(3, tile.Variation);
    }

    [Fact]
    public void Painting_the_same_texture_again_changes_nothing()
    {
        var doc = SeededTerrain(4);
        TerrainCommand.Paint(doc, 4, 4, 1, textureIndex: 2, shape: TerrainCommand.BrushShape.Square);
        var r = TerrainCommand.Paint(doc, 4, 4, 1, textureIndex: 2, shape: TerrainCommand.BrushShape.Square);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(0, r.TilesChanged);
    }

    [Fact]
    public void PaintRect_repaints_the_rectangle_and_leaves_outside_untouched()
    {
        var doc = SeededTerrain(4);
        var r = TerrainCommand.PaintRect(doc, 2, 2, 4, 4, textureIndex: 3);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(9, r.TilesChanged);

        var env = Reload(doc);
        Assert.Equal(3, env.TerrainTiles[Idx(2, 2)].Texture);
        Assert.Equal(3, env.TerrainTiles[Idx(4, 4)].Texture);
        Assert.Equal(0, env.TerrainTiles[Idx(0, 0)].Texture); // outside the rect
    }

    [Fact]
    public void Paint_out_of_range_texture_index_is_rejected()
    {
        var doc = SeededTerrain(4);            // valid indices 0..3
        var r = TerrainCommand.Paint(doc, 4, 4, 1, textureIndex: 9);
        Assert.False(r.Ok);
        Assert.Contains("out of range", r.Message);
    }

    [Fact]
    public void Paint_negative_texture_index_is_rejected()
    {
        var r = TerrainCommand.Paint(SeededTerrain(4), 4, 4, 1, textureIndex: -1);
        Assert.False(r.Ok);
    }

    [Fact]
    public void Paint_negative_variation_is_rejected()
    {
        var r = TerrainCommand.Paint(SeededTerrain(4), 4, 4, 1, textureIndex: 0, variation: -1);
        Assert.False(r.Ok);
    }

    [Fact]
    public void Paint_negative_radius_is_rejected()
    {
        var r = TerrainCommand.Paint(SeededTerrain(4), 4, 4, -1, textureIndex: 0);
        Assert.False(r.Ok);
    }

    // --- cliff level / ramp -------------------------------------------------

    [Fact]
    public void Cliff_raise_lifts_the_centre_and_leaves_the_corner()
    {
        var doc = BlankTerrain();
        var r = TerrainCommand.Cliff(doc, 4, 4, 1, TerrainCommand.CliffOp.Raise, 2, TerrainCommand.BrushShape.Square);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(9, r.TilesChanged); // (2*1+1)²

        var env = Reload(doc);
        Assert.Equal(2, env.TerrainTiles[Idx(4, 4)].CliffLevel);  // base 0 + 2
        Assert.Equal(0, env.TerrainTiles[Idx(0, 0)].CliffLevel);  // untouched corner
    }

    [Fact]
    public void Cliff_set_writes_the_absolute_level_through_save()
    {
        var doc = BlankTerrain();
        var r = TerrainCommand.Cliff(doc, 3, 3, 0, TerrainCommand.CliffOp.Set, 5, TerrainCommand.BrushShape.Square);
        Assert.True(r.Ok, r.Message);

        Assert.Equal(5, Reload(doc).TerrainTiles[Idx(3, 3)].CliffLevel);
    }

    [Fact]
    public void Cliff_lower_floors_at_the_base_layer()
    {
        var doc = BlankTerrain();
        TerrainCommand.CliffRect(doc, 0, 0, 8, 8, TerrainCommand.CliffOp.Set, 3); // whole grid at layer 3
        var r = TerrainCommand.Cliff(doc, 4, 4, 2, TerrainCommand.CliffOp.Lower, 5, TerrainCommand.BrushShape.Square);
        Assert.True(r.Ok, r.Message);

        var env = Reload(doc);
        Assert.Equal(0, env.TerrainTiles[Idx(4, 4)].CliffLevel);  // 3 - 5 floored to 0, not -2
        Assert.Equal(3, env.TerrainTiles[Idx(0, 0)].CliffLevel);  // outside the brush
    }

    [Fact]
    public void Cliff_circle_footprint_is_smaller_than_the_bounding_square()
    {
        var square = TerrainCommand.Cliff(BlankTerrain(), 4, 4, 2, TerrainCommand.CliffOp.Raise, 1, TerrainCommand.BrushShape.Square);
        var circle = TerrainCommand.Cliff(BlankTerrain(), 4, 4, 2, TerrainCommand.CliffOp.Raise, 1, TerrainCommand.BrushShape.Circle);
        Assert.Equal(25, square.TilesChanged);  // 5×5
        Assert.Equal(13, circle.TilesChanged);  // dx²+dy² ≤ 4 disc
    }

    [Fact]
    public void CliffRect_spanning_the_grid_is_a_full_map_fill()
    {
        var r = TerrainCommand.CliffRect(BlankTerrain(), 0, 0, 8, 8, TerrainCommand.CliffOp.Set, 2);
        Assert.Equal(W * W, r.TilesChanged);
    }

    [Fact]
    public void Cliff_negative_level_is_rejected()
    {
        var r = TerrainCommand.Cliff(BlankTerrain(), 4, 4, 1, TerrainCommand.CliffOp.Set, -1);
        Assert.False(r.Ok);
    }

    [Fact]
    public void Cliff_negative_radius_is_rejected()
    {
        var r = TerrainCommand.Cliff(BlankTerrain(), 4, 4, -1, TerrainCommand.CliffOp.Raise, 1);
        Assert.False(r.Ok);
    }

    [Fact]
    public void Cliff_region_entirely_off_grid_changes_nothing()
    {
        var r = TerrainCommand.Cliff(BlankTerrain(), 100, 100, 2, TerrainCommand.CliffOp.Raise, 1);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(0, r.TilesChanged);
    }

    [Fact]
    public void Ramp_on_sets_the_flag_through_save()
    {
        var doc = BlankTerrain();
        Assert.False(Env(doc).TerrainTiles[Idx(4, 4)].IsRamp); // blank maps start un-ramped
        var r = TerrainCommand.Ramp(doc, 4, 4, 1, on: true, TerrainCommand.BrushShape.Square);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(9, r.TilesChanged);

        var env = Reload(doc);
        Assert.True(env.TerrainTiles[Idx(4, 4)].IsRamp);
        Assert.False(env.TerrainTiles[Idx(0, 0)].IsRamp); // untouched corner
    }

    [Fact]
    public void Ramp_off_clears_a_previously_set_flag()
    {
        var doc = BlankTerrain();
        TerrainCommand.RampRect(doc, 2, 2, 4, 4, on: true);
        var r = TerrainCommand.RampRect(doc, 2, 2, 4, 4, on: false);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(9, r.TilesChanged);

        Assert.False(Reload(doc).TerrainTiles[Idx(3, 3)].IsRamp);
    }

    [Fact]
    public void Ramp_already_in_the_target_state_changes_nothing()
    {
        var doc = BlankTerrain();
        TerrainCommand.Ramp(doc, 4, 4, 1, on: true, TerrainCommand.BrushShape.Square);
        var r = TerrainCommand.Ramp(doc, 4, 4, 1, on: true, TerrainCommand.BrushShape.Square);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(0, r.TilesChanged);
    }

    [Fact]
    public void Ramp_negative_radius_is_rejected()
    {
        var r = TerrainCommand.Ramp(BlankTerrain(), 4, 4, -1, on: true);
        Assert.False(r.Ok);
    }
}
