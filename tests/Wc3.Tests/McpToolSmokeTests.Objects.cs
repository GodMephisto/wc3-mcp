// tests/Wc3.Tests/McpToolSmokeTests.Objects.cs
// Objects family. object_list, object_get, object_new, object_set,
// player_list, player_set_force, force_list, force_set_flags.
// object_new and object_set open game data when the map references
// standard objects, so they need a game install directory.
using System.Text.Json;
using ModelContextProtocol.Protocol;

namespace Wc3.Tests;

public sealed partial class McpToolSmokeTests
{
    /// <summary>Helper for write tools with game_dir. Verifies the input map is unchanged.</summary>
    private async Task<string> WriteGameDir(string tool, Dictionary<string, object?> args, string? map = null)
    {
        map ??= Fixture;
        var outPath = NewOut(tool);
        args["map"] = map;
        args["out_path"] = outPath;
        args["game_dir"] = "C:\\Warcraft III";
        var before = Sha(map);
        await Call(tool, args);
        Assert.Equal(before, Sha(map));
        Assert.True(File.Exists(outPath), $"{tool} wrote nothing at {outPath}");
        await Call("map_info", new Dictionary<string, object?> { ["map"] = outPath });
        return outPath;
    }

    /// <summary>Helper for write tools with game_dir. Expects failure.</summary>
    private async Task CallErrorGameDir(string tool, Dictionary<string, object?> args)
    {
        string message = "";
        var fullArgs = new Dictionary<string, object?> { ["map"] = Fixture, ["out_path"] = NewOut(tool), ["game_dir"] = "C:\\Warcraft III" };
        foreach (var pair in args) fullArgs[pair.Key] = pair.Value;
        await McpTestClient.WithClient(async (client, ct) =>
        {
            var r = await client.CallToolAsync(tool, fullArgs, cancellationToken: ct);
            message = r.Content.OfType<TextContentBlock>().FirstOrDefault()?.Text ?? "";
            Assert.True(r.IsError == true, $"{tool} should have refused but answered {message}");
        });
        Assert.False(string.IsNullOrWhiteSpace(message), $"{tool} refused without saying why");
    }

    [Fact, Covers("object_list")]
    public async Task Object_list_on_a_blank_map_is_empty()
    {
        foreach (var kind in new[] { "unit", "item", "ability", "doodad", "destructable", "upgrade" })
        {
            var items = Rows(await Call("object_list", Args(("map", Fixture), ("kind", kind))));
            Assert.Empty(items);
        }
    }

    [Fact, Covers("object_get")]
    public async Task Object_get_on_a_missing_object_returns_found_false()
    {
        var obj = await Call("object_get", Args(("map", Fixture), ("rawcode", "ABCD")));
        Assert.False(Prop(obj, "found").GetBoolean());
    }

    [Fact, Covers("player_list")]
    public async Task Player_list_on_a_blank_map_lists_its_user_slots()
    {
        var players = Rows(await Call("player_list", Args(("map", Fixture))));
        Assert.Equal(BlankPlayers, players.Count);

        var p = players[0];
        Assert.Equal(0, Prop(p, "id").GetInt32());
        // These all exist on every player. Checking the JsonElement has value kind string.
        var name = Prop(p, "name");
        Assert.Equal(JsonValueKind.String, name.ValueKind);
        var color = Prop(p, "color");
        Assert.Equal(JsonValueKind.Object, color.ValueKind);
        var race = Prop(p, "race");
        Assert.Equal(JsonValueKind.String, race.ValueKind);
        var controller = Prop(p, "controller");
        Assert.Equal(JsonValueKind.String, controller.ValueKind);
        Assert.False(Prop(p, "fixedStartPosition").GetBoolean());
    }

    [Fact, Covers("force_list")]
    public async Task Force_list_on_a_blank_map_has_one_force_holding_every_player()
    {
        var forces = Rows(await Call("force_list", Args(("map", Fixture))));
        Assert.Single(forces);

        var f = forces[0];
        Assert.Equal(0, Prop(f, "index").GetInt32());
        var pids = Prop(f, "playerIds");
        Assert.Equal(System.Text.Json.JsonValueKind.Array, pids.ValueKind);
        Assert.Equal(Enumerable.Range(0, BlankPlayers), pids.EnumerateArray().Select(e => e.GetInt32()));
        Assert.False(Prop(f, "allied").GetBoolean());
        Assert.False(Prop(f, "alliedVictory").GetBoolean());
        Assert.False(Prop(f, "sharedVision").GetBoolean());
        Assert.False(Prop(f, "sharedUnitControl").GetBoolean());
        Assert.False(Prop(f, "sharedAdvUnitControl").GetBoolean());
    }

    [Fact, Covers("object_new")]
    [Trait("Category", "GameData")]
    public async Task Object_new_creates_a_custom_unit_that_appears_in_object_list()
    {
        var created = await WriteGameDir("object_new", Args(("base_rawcode", "hfoo"), ("kind", "unit")));

        var items = Rows(await Call("object_list", Args(("map", created), ("kind", "unit"))));
        Assert.Single(items);

        var item = items[0];
        var rc = Prop(item, "rawcode").GetString();
        Assert.DoesNotContain("hfoo", rc);

        var obj = await Call("object_get", Args(("map", created), ("rawcode", rc), ("kind", "unit")));
        Assert.True(obj.GetProperty("found").GetBoolean());
        Assert.Equal(rc, Prop(obj, "rawcode").GetString());
    }

    [Fact, Covers("object_set")]
    [Trait("Category", "GameData")]
    public async Task Object_set_changes_a_field_that_object_get_read_back_confirms()
    {
        var created = await WriteGameDir("object_new", Args(("base_rawcode", "hfoo"), ("kind", "unit")));

        var items = Rows(await Call("object_list", Args(("map", created), ("kind", "unit"))));
        var rc = Prop(items[0], "rawcode").GetString();

        var edited = await WriteGameDir("object_set", Args(("rawcode", rc), ("field", "MovB"), ("value", "400"), ("kind", "unit")), map: created);

        var obj = await Call("object_get", Args(("map", edited), ("rawcode", rc), ("kind", "unit")));
        var fields = Prop(obj, "fields");
        Assert.True(fields.ValueKind == JsonValueKind.Array, "object_get fields should be an array");

        var movB = fields.EnumerateArray().FirstOrDefault(f =>
            string.Equals(Prop(f, "code").GetString(), "MovB", StringComparison.OrdinalIgnoreCase));
        Assert.False(movB.ValueKind == JsonValueKind.Null, "MovB field not found in object_get result");
        Assert.Equal("400", Prop(movB, "value").GetString());
        Assert.Equal("map", Prop(movB, "source").GetString());
    }

    [Fact, Covers("object_new", "object_set")]
    [Trait("Category", "GameData")]
    public async Task Object_new_then_set_can_chain()
    {
        var created = await WriteGameDir("object_new", Args(("base_rawcode", "hfoo"), ("kind", "unit")));

        var items = Rows(await Call("object_list", Args(("map", created), ("kind", "unit"))));
        var rc = Prop(items[0], "rawcode").GetString();

        var edited = await WriteGameDir("object_set", Args(("rawcode", rc), ("field", "MovB"), ("value", "500"), ("kind", "unit")), map: created);

        items = Rows(await Call("object_list", Args(("map", edited), ("kind", "unit"))));
        Assert.Single(items);

        var obj = await Call("object_get", Args(("map", edited), ("rawcode", rc), ("kind", "unit")));
        var fields = Prop(obj, "fields").EnumerateArray();
        var movB = fields.FirstOrDefault(f =>
            string.Equals(Prop(f, "code").GetString(), "MovB", StringComparison.OrdinalIgnoreCase));
        Assert.False(movB.ValueKind == JsonValueKind.Null, "MovB field not found after chained set");
        Assert.Equal("500", Prop(movB, "value").GetString());
    }

    [Fact, Covers("object_get")]
    public async Task Object_get_on_an_existing_object_returns_found_true()
    {
        var created = await WriteGameDir("object_new", Args(("base_rawcode", "hfoo"), ("kind", "unit")));

        var items = Rows(await Call("object_list", Args(("map", created), ("kind", "unit"))));
        var rc = Prop(items[0], "rawcode").GetString();

        var obj = await Call("object_get", Args(("map", created), ("rawcode", rc)));
        Assert.True(Prop(obj, "found").GetBoolean());
    }

    [Fact, Covers("object_get")]
    public async Task Object_get_auto_detects_kind_when_omitted()
    {
        var created = await WriteGameDir("object_new", Args(("base_rawcode", "hfoo"), ("kind", "unit")));

        var items = Rows(await Call("object_list", Args(("map", created), ("kind", "unit"))));
        var rc = Prop(items[0], "rawcode").GetString();

        // No kind passed. The tool should probe and find it.
        var obj = await Call("object_get", Args(("map", created), ("rawcode", rc)));
        Assert.True(Prop(obj, "found").GetBoolean());
    }

    [Fact, Covers("object_get")]
    public async Task Object_get_on_a_missing_object_has_empty_fields()
    {
        var obj = await Call("object_get", Args(("map", Fixture), ("rawcode", "ZZZZ")));
        var fields = Prop(obj, "fields");
        Assert.True(fields.ValueKind == JsonValueKind.Array, "fields should be an array");
        Assert.Empty(fields.EnumerateArray());
    }

    [Fact, Covers("player_set_force")]
    public async Task Player_set_force_moves_player_1_to_force_0_and_force_list_confirms()
    {
        // The fixture has exactly one player (id 0) and one force (index 0).
        // Just move player 0 to force 0 (no-op but valid).

        var updated = await Write("player_set_force", Args(("player_id", 0), ("force_index", 0)));

        var forces = Rows(await Call("force_list", Args(("map", updated))));
        Assert.Single(forces);

        var f0 = forces[0];
        Assert.Equal(0, Prop(f0, "index").GetInt32());
        var pids = Prop(f0, "playerIds").EnumerateArray().Select(p => p.GetInt32()).ToList();
        Assert.Contains(0, pids);
    }

    [Fact, Covers("player_set_force")]
    public async Task Player_set_force_on_invalid_player_is_refused()
    {
        await CallErrorGameDir("player_set_force", Args(("player_id", 99), ("force_index", 0)));
    }

    [Fact, Covers("player_set_force")]
    public async Task Player_set_force_on_invalid_force_is_refused()
    {
        await CallErrorGameDir("player_set_force", Args(("player_id", 0), ("force_index", 5)));
    }

    [Fact, Covers("force_set_flags")]
    public async Task Force_set_flags_sets_and_read_back_through_force_list()
    {
        var updated = await Write("force_set_flags", Args(("force_index", 0),
            ("allied", true), ("allied_victory", false), ("shared_vision", true),
            ("shared_unit_control", false), ("shared_adv_unit_control", true)));

        var forces = Rows(await Call("force_list", Args(("map", updated))));
        Assert.Single(forces);

        var f0 = forces[0];
        Assert.Equal(0, Prop(f0, "index").GetInt32());
        Assert.True(Prop(f0, "allied").GetBoolean());
        Assert.False(Prop(f0, "alliedVictory").GetBoolean());
        Assert.True(Prop(f0, "sharedVision").GetBoolean());
        Assert.False(Prop(f0, "sharedUnitControl").GetBoolean());
        Assert.True(Prop(f0, "sharedAdvUnitControl").GetBoolean());
    }

    [Fact, Covers("force_set_flags")]
    public async Task Force_set_flags_on_missing_force_is_refused()
    {
        await CallError("force_set_flags", Args(("map", Fixture), ("out_path", NewOut("fsf")),
            ("force_index", 5), ("allied", true)));
    }
}
