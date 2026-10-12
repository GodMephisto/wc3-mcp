// tests/Wc3.Tests/McpToolSmokeTests.Placement.cs
// Placement, pathing and terrain. No MCP tool lists placed objects or tiles, so each change is read
// back by opening the saved map with the library and finding the exact unit, doodad, region or tile.
using War3Net.Build.Environment;
using War3Net.Build.Widget;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public sealed class McpSmokePlacementTests : McpSmokeBase
{
    public McpSmokePlacementTests(SmokeFixture f) : base(f) { }

    private static T Model<T>(string map, string file) where T : class =>
        MapDocument.Load(map).GetFile(file)?.Model as T ?? throw new Xunit.Sdk.XunitException($"{file} missing in {map}");

    private static int CreationNumber(System.Text.Json.JsonElement r) => Prop(r, "creationNumber").GetInt32();

    [Fact, Covers("place_unit")]
    public async Task Place_unit_puts_the_unit_in_war3mapUnits_doo()
    {
        var outPath = NewOut("place_unit");
        var r = await Call("place_unit", Args(("map", Fixture), ("out_path", outPath),
            ("type_rawcode", "hfoo"), ("owner_id", 0), ("x", 64f), ("y", -128f)));
        var units = Model<MapUnits>(outPath, PlacementCommand.UnitsFile);
        var u = Assert.Single(units.Units, x => x.CreationNumber == CreationNumber(r));
        Assert.Equal("hfoo", u.TypeId.ToRawcode());
        Assert.Equal(0, u.OwnerId);
        Assert.Equal(64f, u.Position.X);
        Assert.Equal(-128f, u.Position.Y);
    }

    [Fact, Covers("place_item")]
    public async Task Place_item_puts_the_item_in_war3mapUnits_doo()
    {
        var outPath = NewOut("place_item");
        var r = await Call("place_item", Args(("map", Fixture), ("out_path", outPath),
            ("type_rawcode", "ratf"), ("x", 32f), ("y", 32f)));
        var item = Assert.Single(Model<MapUnits>(outPath, PlacementCommand.UnitsFile).Units,
            x => x.CreationNumber == CreationNumber(r));
        Assert.Equal("ratf", item.TypeId.ToRawcode());
    }

    [Fact, Covers("place_doodad")]
    public async Task Place_doodad_puts_the_doodad_in_war3map_doo()
    {
        var outPath = NewOut("place_doodad");
        var r = await Call("place_doodad", Args(("map", Fixture), ("out_path", outPath),
            ("type_rawcode", "LTlt"), ("x", -64f), ("y", 64f), ("variation", 2)));
        var d = Assert.Single(Model<MapDoodads>(outPath, PlacementCommand.DoodadsFile).Doodads,
            x => x.CreationNumber == CreationNumber(r));
        Assert.Equal("LTlt", d.TypeId.ToRawcode());
        Assert.Equal(2, d.Variation);
        Assert.Equal(-64f, d.Position.X);
    }

    [Fact, Covers("place_region")]
    public async Task Place_region_writes_the_region_with_its_bounds()
    {
        var outPath = NewOut("place_region");
        await Call("place_region", Args(("map", Fixture), ("out_path", outPath),
            ("name", "Spawn"), ("left", -256f), ("bottom", -128f), ("right", 256f), ("top", 128f)));
        var region = Assert.Single(Model<MapRegions>(outPath, PlacementCommand.RegionsFile).Regions, x => x.Name == "Spawn");
        Assert.Equal(-256f, region.Left);
        Assert.Equal(128f, region.Top);
    }

    [Fact, Covers("place_start_location")]
    public async Task Place_start_location_adds_a_sloc_for_the_player()
    {
        var outPath = NewOut("place_start");
        await Call("place_start_location", Args(("map", Fixture), ("out_path", outPath),
            ("player", 0), ("x", 96f), ("y", 96f)));
        var sloc = Assert.Single(Model<MapUnits>(outPath, PlacementCommand.UnitsFile).Units,
            x => x.TypeId.ToRawcode() == PlacementCommand.StartLocationRawcode);
        Assert.Equal(0, sloc.OwnerId);
        Assert.Equal(96f, sloc.Position.X);
    }

    [Fact, Covers("place_unit")]
    public async Task Place_unit_onto_its_own_input_is_refused()
    {
        await CallError("place_unit", Args(("map", Fixture), ("out_path", Fixture),
            ("type_rawcode", "hfoo"), ("owner_id", 0), ("x", 0f), ("y", 0f)));
    }

    [Fact, Covers("place_grid")]
    public async Task Place_grid_lays_out_a_rows_by_cols_block()
    {
        var outPath = NewOut("place_grid");
        var r = await Call("place_grid", Args(("map", Fixture), ("out_path", outPath),
            ("type_rawcode", "hfoo"), ("rows", 2), ("cols", 3)));
        Assert.Equal(6, Prop(r, "placed").GetInt32());
        Assert.Equal(6, Rows(await Call("placed_units_list", Args(("map", outPath)))).Count);
    }

    [Fact, Covers("place_scatter")]
    public async Task Place_scatter_places_the_requested_count()
    {
        var outPath = NewOut("place_scatter");
        var r = await Call("place_scatter", Args(("map", Fixture), ("out_path", outPath),
            ("type_rawcode", "hfoo"), ("count", 5), ("seed", 3)));
        Assert.Equal(5, Prop(r, "placed").GetInt32());
        Assert.Equal(5, Rows(await Call("placed_units_list", Args(("map", outPath)))).Count);
    }

    [Fact, Covers("placed_stats")]
    public async Task Placed_stats_counts_the_placed_objects()
    {
        var map = await PopulatedMap();
        var r = await Call("placed_stats", Args(("map", map)));
        Assert.Equal(1, Prop(r, "unitTotal").GetInt32());
        Assert.Equal(1, Prop(r, "doodadTotal").GetInt32());
    }

    [Fact, Covers("placed_missing_refs")]
    public async Task Placed_missing_refs_flags_a_stranger_rawcode()
    {
        var map = await Write("place_unit", Args(("type_rawcode", "zX9q"), ("owner_id", 0), ("x", 0f), ("y", 0f)));
        var r = await Call("placed_missing_refs", Args(("map", map)));
        Assert.True(Prop(r, "count").GetInt32() >= 1);
        Assert.Contains(Rows(r), m => Prop(m, "rawcode").GetString() == "zX9q");
    }

    [Fact, Covers("placed_fix_refs")]
    public async Task Placed_fix_refs_drops_a_stranger_placement()
    {
        var map = await Write("place_unit", Args(("type_rawcode", "zX9q"), ("owner_id", 0), ("x", 0f), ("y", 0f)));
        var fixedMap = await Write("placed_fix_refs", Args(), map);
        Assert.Empty(Rows(await Call("placed_units_list", Args(("map", fixedMap)))));
    }

    [Fact, Covers("pathing_paint")]
    public async Task Pathing_paint_changes_war3map_wpm()
    {
        var outPath = await Write("pathing_paint", Args(("center_x", 20), ("center_y", 20), ("radius", 3), ("flags", "Walk")));
        Assert.True(MapDocument.Load(Fixture).TryReadFileByName(PathingCommand.PathingFile, out var before));
        Assert.True(MapDocument.Load(outPath).TryReadFileByName(PathingCommand.PathingFile, out var after));
        Assert.Equal(before.Length, after.Length);
        Assert.NotEqual(before, after);
    }

    [Fact, Covers("region_create")]
    public async Task Region_create_writes_the_region_from_corner_and_size()
    {
        var outPath = await Write("region_create", Args(("name", "Box"),
            ("x", -128f), ("y", -64f), ("w", 256f), ("h", 128f)));
        var region = Assert.Single(Model<MapRegions>(outPath, PlacementCommand.RegionsFile).Regions,
            x => x.Name == "Box");
        Assert.Equal(-128f, region.Left);
        Assert.Equal(-64f, region.Bottom);
        Assert.Equal(128f, region.Right);
        Assert.Equal(64f, region.Top);
    }

    [Fact, Covers("region_to_rect")]
    public async Task Region_to_rect_returns_the_corner_values()
    {
        var map = await PopulatedMap();
        var r = await Call("region_to_rect", Args(("map", map), ("region", "Spawn")));
        Assert.Equal(-256f, Prop(r, "left").GetSingle());
        Assert.Equal(256f, Prop(r, "right").GetSingle());
        Assert.Contains("Rect(", Prop(r, "jass").GetString());
    }

    [Fact, Covers("region_validate")]
    public async Task Region_validate_is_clean_on_a_single_region()
    {
        var map = await PopulatedMap();
        var r = await Call("region_validate", Args(("map", map)));
        Assert.Equal(0, Prop(r, "issues").GetArrayLength());
    }

    [Fact, Covers("pathing_validate")]
    public async Task Pathing_validate_is_clean_on_a_blank_map()
    {
        var r = await Call("pathing_validate", Args(("map", Fixture)));
        Assert.Equal(0, Prop(r, "issues").GetArrayLength());
    }

    private static TerrainTile Tile(string map, int x, int y)
    {
        var env = Model<MapEnvironment>(map, TerrainCommand.TerrainFile);
        return env.TerrainTiles[y * ((int)env.Width + 1) + x];
    }

    [Fact, Covers("terrain_stats", "terrain_deform")]
    public async Task Terrain_deform_raise_shows_in_terrain_stats_and_the_tile()
    {
        var before = await Call("terrain_stats", Args(("map", Fixture)));
        Assert.True(Prop(before, "tileCount").GetInt32() > 0);
        var outPath = await Write("terrain_deform", Args(("center_x", 16), ("center_y", 16), ("radius", 2),
            ("op", "raise"), ("amount", 3f)));
        var after = await Call("terrain_stats", Args(("map", outPath)));
        Assert.True(Prop(after, "maxHeight").GetSingle() > Prop(before, "maxHeight").GetSingle());
        Assert.True(Tile(outPath, 16, 16).Height > Tile(Fixture, 16, 16).Height);
    }

    [Fact, Covers("terrain_cliff")]
    public async Task Terrain_cliff_raises_the_cliff_level()
    {
        var outPath = await Write("terrain_cliff", Args(("center_x", 10), ("center_y", 10), ("radius", 1), ("op", "raise"), ("level", 1)));
        Assert.Equal(Tile(Fixture, 10, 10).CliffLevel + 1, Tile(outPath, 10, 10).CliffLevel);
        var stats = await Call("terrain_stats", Args(("map", outPath)));
        Assert.True(Prop(stats, "maxCliff").GetInt32() > Prop(await Call("terrain_stats", Args(("map", Fixture))), "maxCliff").GetInt32());
    }

    [Fact, Covers("terrain_ramp")]
    public async Task Terrain_ramp_marks_the_tile_as_a_ramp()
    {
        Assert.False(Tile(Fixture, 12, 12).IsRamp);
        var outPath = await Write("terrain_ramp", Args(("center_x", 12), ("center_y", 12), ("radius", 1), ("on", true)));
        Assert.True(Tile(outPath, 12, 12).IsRamp);
    }

    [Fact, Covers("terrain_paint")]
    public async Task Terrain_paint_sets_the_texture_index()
    {
        int start = Tile(Fixture, 8, 8).Texture;
        int target = start == 1 ? 2 : 1;
        var outPath = await Write("terrain_paint", Args(("center_x", 8), ("center_y", 8), ("radius", 1), ("texture_index", target)));
        Assert.Equal(target, Tile(outPath, 8, 8).Texture);
    }

    [Fact, Covers("terrain_water")]
    public async Task Terrain_water_sets_the_water_height()
    {
        var outPath = await Write("terrain_water", Args(("center_x", 14), ("center_y", 14), ("radius", 1), ("op", "set"), ("amount", 1.5f)));
        Assert.Equal(1.5f, Tile(outPath, 14, 14).WaterHeight, 3);
        Assert.NotEqual(Tile(Fixture, 14, 14).WaterHeight, Tile(outPath, 14, 14).WaterHeight);
    }

    [Fact, Covers("terrain_blight")]
    public async Task Terrain_blight_marks_the_tile_blighted()
    {
        Assert.False(Tile(Fixture, 18, 18).IsBlighted);
        var outPath = await Write("terrain_blight", Args(("center_x", 18), ("center_y", 18), ("radius", 1), ("on", true)));
        Assert.True(Tile(outPath, 18, 18).IsBlighted);
    }
}
