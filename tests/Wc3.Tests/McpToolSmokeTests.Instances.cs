// tests/Wc3.Tests/McpToolSmokeTests.Instances.cs
// Placed units and doodads, regions, and single terrain corners. Each map is built through the
// placement tools first, so the tool under test reads or edits something real, and every edit is
// read back through the matching get or list tool.
using System.Text.Json;

namespace Wc3.Tests;

public sealed partial class McpToolSmokeTests
{
    /// <summary>The fixture plus one footman, one tree and one region, all placed through MCP.</summary>
    private async Task<string> PopulatedMap()
    {
        var withUnit = await Write("place_unit", Args(("type_rawcode", "hfoo"), ("owner_id", 0), ("x", 64f), ("y", -64f)));
        var withDoodad = await Write("place_doodad", Args(("type_rawcode", "LTlt"), ("x", 128f), ("y", 128f)), withUnit);
        return await Write("place_region", Args(("name", "Spawn"), ("left", -256f), ("bottom", -256f), ("right", 256f), ("top", 256f)), withDoodad);
    }

    [Fact, Covers("placed_units_list", "placed_unit_get")]
    public async Task Placed_unit_reads_back_with_its_scale_named()
    {
        var map = await PopulatedMap();
        var unit = Assert.Single(Rows(await Call("placed_units_list", Args(("map", map)))));
        Assert.Equal("hfoo", Prop(unit, "typeRawcode").GetString());
        Assert.Equal(64f, Prop(unit, "x").GetSingle());
        Assert.Equal(-64f, Prop(unit, "y").GetSingle());

        var one = await Call("placed_unit_get", Args(("map", map), ("creation_number", Prop(unit, "creationNumber").GetInt32())));
        Assert.Equal("hfoo", Prop(one, "typeRawcode").GetString());
        // The scale used to serialize as {} because it is a tuple. It must carry real numbers.
        var scale = Prop(one, "scale");
        Assert.Equal(1f, Prop(scale, "sx").GetSingle());
        Assert.Equal(1f, Prop(scale, "sz").GetSingle());
    }

    [Fact, Covers("placed_unit_set")]
    public async Task Placed_unit_set_changes_the_one_field()
    {
        var map = await PopulatedMap();
        var set = await Write("placed_unit_set", Args(("creation_number", 0), ("field", "Facing"), ("value", "1.5")), map);
        var after = await Call("placed_unit_get", Args(("map", set), ("creation_number", 0)));
        Assert.Equal(1.5f, Prop(after, "rotation").GetSingle(), 3);
        Assert.Equal(64f, Prop(after, "x").GetSingle());
    }

    [Fact, Covers("placed_unit_remove")]
    public async Task Placed_unit_remove_leaves_no_unit()
    {
        var map = await PopulatedMap();
        var removed = await Write("placed_unit_remove", Args(("creation_number", 0)), map);
        Assert.Empty(Rows(await Call("placed_units_list", Args(("map", removed)))));
        // The doodad on the same map is untouched.
        Assert.Single(Rows(await Call("placed_doodads_list", Args(("map", removed)))));
    }

    [Fact, Covers("placed_doodads_list", "placed_doodad_get")]
    public async Task Placed_doodad_reads_back()
    {
        var map = await PopulatedMap();
        var tree = Assert.Single(Rows(await Call("placed_doodads_list", Args(("map", map)))));
        Assert.Equal("LTlt", Prop(tree, "typeRawcode").GetString());
        var one = await Call("placed_doodad_get", Args(("map", map), ("creation_number", Prop(tree, "creationNumber").GetInt32())));
        Assert.Equal(128f, Prop(one, "x").GetSingle());
        Assert.Equal(1f, Prop(Prop(one, "scale"), "sy").GetSingle());
    }

    [Fact, Covers("placed_doodad_set")]
    public async Task Placed_doodad_set_changes_the_variation()
    {
        var map = await PopulatedMap();
        var set = await Write("placed_doodad_set", Args(("creation_number", 0), ("field", "Variation"), ("value", "2")), map);
        Assert.Equal(2, Prop(await Call("placed_doodad_get", Args(("map", set), ("creation_number", 0))), "variation").GetInt32());
    }

    [Fact, Covers("placed_doodad_remove")]
    public async Task Placed_doodad_remove_leaves_no_doodad()
    {
        var map = await PopulatedMap();
        var removed = await Write("placed_doodad_remove", Args(("creation_number", 0)), map);
        Assert.Empty(Rows(await Call("placed_doodads_list", Args(("map", removed)))));
        Assert.Single(Rows(await Call("placed_units_list", Args(("map", removed)))));
    }

    [Fact, Covers("region_list", "region_remove")]
    public async Task Region_lists_then_removes()
    {
        var map = await PopulatedMap();
        var region = Assert.Single(Rows(await Call("region_list", Args(("map", map)))));
        Assert.Equal("Spawn", Prop(region, "name").GetString());
        Assert.Equal(256f, Prop(region, "right").GetSingle());

        var removed = await Write("region_remove", Args(("name", "Spawn")), map);
        Assert.Empty(Rows(await Call("region_list", Args(("map", removed)))));
        var missing = await CallError("region_remove", Args(("map", removed), ("out_path", NewOut("again")), ("name", "Spawn")));
        Assert.Contains("Spawn", missing);
    }

    [Fact, Covers("terrain_info", "terrain_corner_get")]
    public async Task Terrain_info_and_a_corner_describe_the_blank_map()
    {
        var info = await Call("terrain_info", Args(("map", Fixture)));
        // A 32 tile map has 33 corners a side.
        Assert.Equal(33, Prop(info, "width").GetInt32());
        Assert.Equal(33, Prop(info, "height").GetInt32());
        Assert.NotEmpty(Prop(info, "groundTiles").EnumerateArray());

        var corner = await Call("terrain_corner_get", Args(("map", Fixture), ("col", 3), ("row", 4)));
        Assert.Equal(3, Prop(corner, "col").GetInt32());
        Assert.Equal(0, Prop(corner, "cliffLevel").GetInt32());
        Assert.False(Prop(corner, "water").GetBoolean());
    }

    [Fact, Covers("terrain_corner_set")]
    public async Task Terrain_corner_set_changes_that_corner_only()
    {
        var set = await Write("terrain_corner_set", Args(("col", 3), ("row", 4), ("field", "CliffLevel"), ("value", "3")));
        Assert.Equal(3, Prop(await Call("terrain_corner_get", Args(("map", set), ("col", 3), ("row", 4))), "cliffLevel").GetInt32());
        Assert.Equal(0, Prop(await Call("terrain_corner_get", Args(("map", set), ("col", 4), ("row", 4))), "cliffLevel").GetInt32());
    }

    [Fact, Covers("terrain_fill")]
    public async Task Terrain_fill_sets_the_rectangle_and_stops_at_its_edge()
    {
        var filled = await Write("terrain_fill", Args(("x0", 2), ("y0", 2), ("x1", 4), ("y1", 5), ("tool", "SetHeight"), ("value", 40f)));
        Assert.Equal(40f, Prop(await Call("terrain_corner_get", Args(("map", filled), ("col", 4), ("row", 5))), "groundHeight").GetSingle());
        Assert.Equal(0f, Prop(await Call("terrain_corner_get", Args(("map", filled), ("col", 5), ("row", 5))), "groundHeight").GetSingle());
        await CallError("terrain_fill", Args(("map", Fixture), ("out_path", NewOut("bad")), ("x0", 0), ("y0", 0), ("x1", 1), ("y1", 1), ("tool", "NoSuchTool")));
    }
}
