// tests/Wc3.Tests/TerrainEditCommandTests.cs
using War3Net.Build.Environment; // MapEnvironment, TerrainType
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

public class TerrainEditCommandTests
{
    private const int TileEdge = 8;         // 8×8 tiles → 9×9 = 81 corner tiles
    private const int W = TileEdge + 1;     // grid width in corners
    private static int Idx(int x, int y) => y * W + x;

    private static MapDocument BlankTerrain() => BlankMap.Create(new BlankMapOptions { TileEdge = TileEdge });

    private static MapEnvironment Env(MapDocument doc)
        => (MapEnvironment)doc.GetFile(TerrainEditCommand.TerrainFile)!.Model!;

    private static MapEnvironment Reload(MapDocument doc)
        => Env(MapDocument.Load(doc.SaveToBytes()));

    // BlankMap.Create emits a flat grid at a fixed default height (not necessarily 0),
    // so tests read the baseline from an untouched corner instead of hardcoding a constant.
    private static float Baseline() => Env(BlankTerrain()).TerrainTiles[Idx(0, 0)].Height;

    // Seed EXACTLY `groundTypes` ground tile-types (clearing the blank map's default
    // Lordaeron palette first) so texture-index tests validate against a known count.
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

    // --- info -----------------------------------------------------------------

    [Fact]
    public void GetInfo_reports_corner_grid_dimensions()
    {
        var info = TerrainEditCommand.GetInfo(BlankTerrain());
        Assert.NotNull(info);
        Assert.Equal(W, info!.Width);
        Assert.Equal(W, info.Height);
        Assert.NotEmpty(info.GroundTiles);   // blank maps ship the tileset's ground palette
        Assert.Equal("Lgrs", info.GroundTiles[0]); // grass at index 0
        Assert.Empty(info.CliffTiles);       // flat blank map has no cliff tiles
    }

    [Fact]
    public void GetInfo_lists_ground_tiles_as_fourcc_ids()
    {
        // The blank map's Lordaeron Summer ground palette surfaces as 4CC tile ids.
        var info = TerrainEditCommand.GetInfo(BlankTerrain())!;
        Assert.Equal(new[] { "Lgrs", "Lgrd", "Ldrt", "Ldro", "Lrok" }, info.GroundTiles);
    }

    // --- corner reads -----------------------------------------------------------

    [Fact]
    public void GetCorner_returns_the_blank_map_baseline()
    {
        var c = TerrainEditCommand.GetCorner(BlankTerrain(), 4, 4);
        Assert.NotNull(c);
        Assert.Equal(4, c!.Col);
        Assert.Equal(4, c.Row);
        Assert.Equal(Baseline(), c.GroundHeight);
        Assert.Equal(0, c.CliffLevel);
        Assert.False(c.Water);
        Assert.False(c.Ramp);
        Assert.False(c.Boundary);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(W, 0)]
    [InlineData(0, W)]
    public void GetCorner_off_grid_returns_null(int col, int row)
    {
        Assert.Null(TerrainEditCommand.GetCorner(BlankTerrain(), col, row));
    }

    // --- ground height ----------------------------------------------------------

    [Fact]
    public void SetGroundHeight_is_visible_through_GetCorner()
    {
        var doc = BlankTerrain();
        var before = TerrainEditCommand.GetCorner(doc, 3, 5)!;
        var r = TerrainEditCommand.SetGroundHeight(doc, 3, 5, before.GroundHeight + 2.5f);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(1, r.CornersChanged);

        var after = TerrainEditCommand.GetCorner(doc, 3, 5)!;
        Assert.Equal(before.GroundHeight + 2.5f, after.GroundHeight);
    }

    [Fact]
    public void SetGroundHeight_persists_through_save_and_reload()
    {
        var doc = BlankTerrain();
        float target = Baseline() + 2.5f;   // exact multiple of 1/512 → faithful round-trip
        var r = TerrainEditCommand.SetGroundHeight(doc, 4, 4, target);
        Assert.True(r.Ok, r.Message);

        var env = Reload(doc);
        Assert.Equal(target, env.TerrainTiles[Idx(4, 4)].Height);
        Assert.Equal(Baseline(), env.TerrainTiles[Idx(0, 0)].Height); // far corner untouched
    }

    [Fact]
    public void SetGroundHeight_quantizes_to_the_file_lattice_up_front()
    {
        var doc = BlankTerrain();
        TerrainEditCommand.SetGroundHeight(doc, 2, 2, 0.3f);   // not a 1/512 multiple

        // The in-memory value is already on the 1/512 lattice, so it equals the
        // reloaded value exactly — no surprise drift between edit and save.
        float inMemory = TerrainEditCommand.GetCorner(doc, 2, 2)!.GroundHeight;
        Assert.Equal(MathF.Round(0.3f * 512f) / 512f, inMemory);
        Assert.Equal(inMemory, Reload(doc).TerrainTiles[Idx(2, 2)].Height);
    }

    [Fact]
    public void SetGroundHeight_clamps_to_the_storable_range()
    {
        var doc = BlankTerrain();
        TerrainEditCommand.SetGroundHeight(doc, 1, 1, 9999f);
        Assert.Equal(TerrainEditCommand.MaxGroundHeight, TerrainEditCommand.GetCorner(doc, 1, 1)!.GroundHeight);
        Assert.Equal(TerrainEditCommand.MaxGroundHeight, Reload(doc).TerrainTiles[Idx(1, 1)].Height);

        TerrainEditCommand.SetGroundHeight(doc, 1, 1, -9999f);
        Assert.Equal(TerrainEditCommand.MinGroundHeight, TerrainEditCommand.GetCorner(doc, 1, 1)!.GroundHeight);
    }

    [Fact]
    public void AddGroundHeight_accumulates()
    {
        var doc = BlankTerrain();
        TerrainEditCommand.AddGroundHeight(doc, 4, 4, 1.5f);
        var r = TerrainEditCommand.AddGroundHeight(doc, 4, 4, 0.5f);
        Assert.True(r.Ok, r.Message);

        Assert.Equal(Baseline() + 2f, TerrainEditCommand.GetCorner(doc, 4, 4)!.GroundHeight);
        Assert.Equal(Baseline() + 2f, Reload(doc).TerrainTiles[Idx(4, 4)].Height);
    }

    [Fact]
    public void Setting_the_same_height_again_changes_nothing()
    {
        var doc = BlankTerrain();
        TerrainEditCommand.SetGroundHeight(doc, 4, 4, 3f);
        var r = TerrainEditCommand.SetGroundHeight(doc, 4, 4, 3f);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(0, r.CornersChanged);
    }

    // --- water ------------------------------------------------------------------

    [Fact]
    public void SetWaterHeight_sets_the_level_and_flags_water()
    {
        var doc = BlankTerrain();
        var r = TerrainEditCommand.SetWaterHeight(doc, 4, 4, 1.25f);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(1, r.CornersChanged);

        var c = TerrainEditCommand.GetCorner(doc, 4, 4)!;
        Assert.Equal(1.25f, c.WaterHeight);
        Assert.True(c.Water);

        var tile = Reload(doc).TerrainTiles[Idx(4, 4)];
        Assert.Equal(1.25f, tile.WaterHeight);
        Assert.True(tile.IsWater);
    }

    [Fact]
    public void SetWaterHeight_clamps_to_the_14bit_water_range()
    {
        var doc = BlankTerrain();
        TerrainEditCommand.SetWaterHeight(doc, 2, 2, 9999f);
        Assert.Equal(TerrainEditCommand.MaxWaterHeight, TerrainEditCommand.GetCorner(doc, 2, 2)!.WaterHeight);
        Assert.Equal(TerrainEditCommand.MaxWaterHeight, Reload(doc).TerrainTiles[Idx(2, 2)].WaterHeight);
    }

    // --- texture ----------------------------------------------------------------

    [Fact]
    public void SetGroundTexture_changes_the_index_and_persists()
    {
        var doc = SeededTerrain(4);
        var r = TerrainEditCommand.SetGroundTexture(doc, 4, 4, 2);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(1, r.CornersChanged);

        Assert.Equal(2, TerrainEditCommand.GetCorner(doc, 4, 4)!.GroundTexture);
        Assert.Equal(2, Reload(doc).TerrainTiles[Idx(4, 4)].Texture);
        Assert.Equal(0, Reload(doc).TerrainTiles[Idx(0, 0)].Texture); // untouched corner
    }

    [Fact]
    public void SetGroundTexture_with_no_ground_types_is_rejected()
    {
        var doc = BlankTerrain();
        Env(doc).TerrainTypes.Clear();   // a map with no ground palette can't index a texture
        var r = TerrainEditCommand.SetGroundTexture(doc, 4, 4, 0);
        Assert.False(r.Ok);
        Assert.Contains("ground tile-types", r.Message);
    }

    [Fact]
    public void SetGroundTexture_out_of_range_index_is_rejected()
    {
        var doc = SeededTerrain(4);            // valid indices 0..3
        Assert.False(TerrainEditCommand.SetGroundTexture(doc, 4, 4, 9).Ok);
        Assert.False(TerrainEditCommand.SetGroundTexture(doc, 4, 4, -1).Ok);
    }

    // --- cliff level ------------------------------------------------------------

    [Fact]
    public void SetCliffLevel_persists_through_save()
    {
        var doc = BlankTerrain();
        var r = TerrainEditCommand.SetCliffLevel(doc, 3, 3, 5);
        Assert.True(r.Ok, r.Message);

        Assert.Equal(5, TerrainEditCommand.GetCorner(doc, 3, 3)!.CliffLevel);
        Assert.Equal(5, Reload(doc).TerrainTiles[Idx(3, 3)].CliffLevel);
    }

    [Fact]
    public void SetCliffLevel_outside_the_4bit_range_is_rejected()
    {
        Assert.False(TerrainEditCommand.SetCliffLevel(BlankTerrain(), 3, 3, 16).Ok);
        Assert.False(TerrainEditCommand.SetCliffLevel(BlankTerrain(), 3, 3, -1).Ok);
    }

    // --- bounds -----------------------------------------------------------------

    [Theory]
    [InlineData(-1, 4)]
    [InlineData(4, -1)]
    [InlineData(W, 4)]
    [InlineData(4, W)]
    public void Edits_off_the_grid_are_rejected(int col, int row)
    {
        var doc = BlankTerrain();
        Assert.False(TerrainEditCommand.SetGroundHeight(doc, col, row, 1f).Ok);
        Assert.False(TerrainEditCommand.AddGroundHeight(doc, col, row, 1f).Ok);
        Assert.False(TerrainEditCommand.SetWaterHeight(doc, col, row, 1f).Ok);
        Assert.False(TerrainEditCommand.SetCliffLevel(doc, col, row, 1).Ok);
    }

    // --- area raise (brush primitive) --------------------------------------------

    [Fact]
    public void AddGroundHeightArea_without_falloff_raises_the_disc_uniformly()
    {
        var doc = BlankTerrain();
        var r = TerrainEditCommand.AddGroundHeightArea(doc, 4, 4, 2, 1f, falloff: false);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(13, r.CornersChanged);   // dx²+dy² ≤ 4 disc

        var env = Reload(doc);
        var baseline = Baseline();
        Assert.Equal(baseline + 1f, env.TerrainTiles[Idx(4, 4)].Height); // centre
        Assert.Equal(baseline + 1f, env.TerrainTiles[Idx(6, 4)].Height); // rim (dist 2)
        Assert.Equal(baseline, env.TerrainTiles[Idx(6, 6)].Height);      // outside the disc
        Assert.Equal(baseline, env.TerrainTiles[Idx(0, 0)].Height);      // far corner
    }

    [Fact]
    public void AddGroundHeightArea_with_falloff_gives_the_centre_the_full_delta()
    {
        var doc = BlankTerrain();
        var r = TerrainEditCommand.AddGroundHeightArea(doc, 4, 4, 2, 3f, falloff: true);
        Assert.True(r.Ok, r.Message);

        var baseline = Baseline();
        float centre = TerrainEditCommand.GetCorner(doc, 4, 4)!.GroundHeight;
        float rim = TerrainEditCommand.GetCorner(doc, 6, 4)!.GroundHeight;
        Assert.Equal(baseline + 3f, centre);            // weight 1 at dist 0
        Assert.True(rim > baseline, "rim should still rise");
        Assert.True(rim < centre, "rim should rise less than the centre");
        // weight at dist 2 with r=2 is 1 - 2/3; quantized to the 1/512 lattice.
        float expectedRim = baseline + MathF.Round(3f * (1f - 2f / 3f) * 512f) / 512f;
        Assert.Equal(expectedRim, rim, 3);
    }

    [Fact]
    public void AddGroundHeightArea_footprint_clips_at_the_grid_edge()
    {
        var doc = BlankTerrain();
        var r = TerrainEditCommand.AddGroundHeightArea(doc, 0, 0, 1, 1f, falloff: false);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(3, r.CornersChanged);   // (0,0), (1,0), (0,1) — the rest is off-grid
    }

    [Fact]
    public void AddGroundHeightArea_entirely_off_grid_is_a_noop()
    {
        var r = TerrainEditCommand.AddGroundHeightArea(BlankTerrain(), 100, 100, 2, 1f, falloff: false);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(0, r.CornersChanged);
    }

    [Fact]
    public void AddGroundHeightArea_negative_radius_is_rejected()
    {
        Assert.False(TerrainEditCommand.AddGroundHeightArea(BlankTerrain(), 4, 4, -1, 1f, falloff: false).Ok);
    }

    [Fact]
    public void AddGroundHeightArea_persists_through_save()
    {
        var doc = BlankTerrain();
        TerrainEditCommand.AddGroundHeightArea(doc, 4, 4, 1, 2.5f, falloff: false);

        Assert.Equal(Baseline() + 2.5f, Reload(doc).TerrainTiles[Idx(4, 4)].Height);
    }

    [Fact]
    public void AddGroundHeightArea_lowering_saturates_at_the_floor_instead_of_wrapping()
    {
        // Lowering the ground far past the floor must saturate at MinGroundHeight (-16),
        // never wrap the raw ushort (which would teleport the ground to +top). A blank map
        // now starts flat at the standard datum (Height 0), so drive well below the floor.
        var doc = BlankTerrain();
        var r = TerrainEditCommand.AddGroundHeightArea(doc, 4, 4, 1, -100f, falloff: false);
        Assert.True(r.Ok, r.Message);

        Assert.Equal(TerrainEditCommand.MinGroundHeight, Reload(doc).TerrainTiles[Idx(4, 4)].Height);
    }
}
