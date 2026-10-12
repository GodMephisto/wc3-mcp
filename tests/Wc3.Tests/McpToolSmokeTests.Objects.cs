// tests/Wc3.Tests/McpToolSmokeTests.Objects.cs
// Objects family. object_list, object_get, object_new, object_set, object_search,
// object_noop_fields, object_deprecated, object_rawcode_suggest, object_clone,
// player_list, player_set_force, player_set_name, player_set_color, player_set_race,
// force_list, force_set_flags, force_rename, force_set_players.
// object_new and object_set open game data when the map references
// standard objects, so they need a game install directory.
using System.Text.Json;
using ModelContextProtocol.Protocol;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;

namespace Wc3.Tests;

public sealed class McpSmokeObjectsTests : McpSmokeBase
{
    public McpSmokeObjectsTests(SmokeFixture f) : base(f) { }

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

    [Fact, Covers("player_set_name")]
    public async Task Player_set_name_changes_the_display_name()
    {
        var updated = await Write("player_set_name", Args(("player_id", 0), ("name", "Grom Hellscream")));
        var players = Rows(await Call("player_list", Args(("map", updated))));
        var p0 = players.Single(p => Prop(p, "id").GetInt32() == 0);
        Assert.Equal("Grom Hellscream", Prop(p0, "name").GetString());
    }

    [Fact, Covers("player_set_name")]
    public async Task Player_set_name_on_a_missing_player_is_refused()
    {
        await CallError("player_set_name", Args(("map", Fixture), ("out_path", NewOut("psn")),
            ("player_id", 999), ("name", "Nobody")));
    }

    [Fact, Covers("player_set_race")]
    public async Task Player_set_race_changes_the_race()
    {
        var updated = await Write("player_set_race", Args(("player_id", 0), ("race", "orc")));
        var players = Rows(await Call("player_list", Args(("map", updated))));
        var p0 = players.Single(p => Prop(p, "id").GetInt32() == 0);
        Assert.Equal("Orc", Prop(p0, "race").GetString());
    }

    [Fact, Covers("player_set_race")]
    public async Task Player_set_race_on_an_unknown_race_is_refused()
    {
        await CallError("player_set_race", Args(("map", Fixture), ("out_path", NewOut("psr")),
            ("player_id", 0), ("race", "dragon")));
    }

    [Fact, Covers("player_set_color")]
    public async Task Player_set_color_rewrites_the_script_color_call()
    {
        // war3map.w3i has no color field, so the edit lands in the script's SetPlayerColor call.
        var updated = await Write("player_set_color", Args(("player_id", 0), ("color", "blue")));
        var text = Prop(await Call("file_get_text",
            Args(("map", updated), ("internal_path", "war3map.j"))), "text").GetString();
        Assert.Contains("SetPlayerColor( Player(0), ConvertPlayerColor(1) )", text);
    }

    [Fact, Covers("player_set_color")]
    public async Task Player_set_color_on_an_unknown_color_is_refused()
    {
        await CallError("player_set_color", Args(("map", Fixture), ("out_path", NewOut("psc")),
            ("player_id", 0), ("color", "chartreuse")));
    }

    [Fact, Covers("force_rename")]
    public async Task Force_rename_changes_the_force_name()
    {
        var updated = await Write("force_rename", Args(("force_index", 0), ("name", "The Horde")));
        var forces = Rows(await Call("force_list", Args(("map", updated))));
        Assert.Equal("The Horde", Prop(forces[0], "name").GetString());
    }

    [Fact, Covers("force_rename")]
    public async Task Force_rename_on_a_missing_force_is_refused()
    {
        await CallError("force_rename", Args(("map", Fixture), ("out_path", NewOut("frn")),
            ("force_index", 9), ("name", "Nope")));
    }

    [Fact, Covers("force_set_players")]
    public async Task Force_set_players_remove_then_add_round_trips_through_force_list()
    {
        // A blank map puts every slot in force 0, so remove slot 0 and put it back.
        var removed = await Write("force_set_players", Args(("force_index", 0), ("remove", 0)));
        var afterRemove = Rows(await Call("force_list", Args(("map", removed))));
        var pids = Prop(afterRemove[0], "playerIds").EnumerateArray().Select(e => e.GetInt32()).ToList();
        Assert.DoesNotContain(0, pids);

        var added = await Write("force_set_players", Args(("force_index", 0), ("add", 0)), removed);
        var afterAdd = Rows(await Call("force_list", Args(("map", added))));
        var pids2 = Prop(afterAdd[0], "playerIds").EnumerateArray().Select(e => e.GetInt32()).ToList();
        Assert.Contains(0, pids2);
    }

    [Fact, Covers("force_set_players")]
    public async Task Force_set_players_needs_exactly_one_of_add_or_remove()
    {
        await CallError("force_set_players", Args(("map", Fixture), ("out_path", NewOut("fsp")),
            ("force_index", 0)));
    }

    [Fact, Covers("object_search")]
    public async Task Object_search_finds_a_map_field_value()
    {
        var map = UnitMap("uhpm", "999");
        var result = await Call("object_search", Args(("map", map), ("query", "999")));
        Assert.True(Prop(result, "count").GetInt32() >= 1, "expected at least one hit for 999");
        var hits = Prop(result, "hits");
        Assert.Equal(JsonValueKind.Array, hits.ValueKind);
        Assert.Contains(hits.EnumerateArray(),
            h => string.Equals(Prop(h, "rawcode").GetString(), "u000", StringComparison.Ordinal));
    }

    [Fact, Covers("object_noop_fields")]
    public async Task Object_noop_fields_without_game_data_says_so()
    {
        // A deliberately absent game_dir keeps this hermetic on a machine that does have
        // a WC3 install, since an explicit override never falls back to auto-detection.
        var map = UnitMap("uhpm", "999");
        var result = await Call("object_noop_fields",
            Args(("map", map), ("game_dir", "Z:\\no_such_install")));
        Assert.Equal(0, Prop(result, "count").GetInt32());
        Assert.NotEmpty(Prop(result, "diagnostics").EnumerateArray());
    }

    [Fact, Covers("object_deprecated")]
    public async Task Object_deprecated_reports_a_field_the_target_lacks()
    {
        var map = UnitMap("uhpm", "999");
        var schemaDir = Path.Combine(_f.Dir, $"schema-{Guid.NewGuid():N}");
        var schema = new GameSchema(1, "smoke-build", "test",
            Array.Empty<SchemaInput>(), Array.Empty<string>(),
            new Dictionary<string, IReadOnlyList<SchemaField>>
            { ["units/unitmetadata.slk"] = new[] { SchemaFieldOf("unam") } },
            null, null);
        GameSchemaCommand.Save(schema, schemaDir);

        var result = await Call("object_deprecated", Args(("map", map), ("target", schemaDir)));
        Assert.True(Prop(result, "fieldsChecked").GetInt32() >= 1);
        var fields = Prop(result, "fields");
        Assert.Contains(fields.EnumerateArray(),
            f => string.Equals(Prop(f, "fieldCode").GetString(), "uhpm", StringComparison.Ordinal));
    }

    [Fact, Covers("object_rawcode_suggest")]
    public async Task Object_rawcode_suggest_avoids_used_codes()
    {
        var map = UnitMap("uhpm", "999");
        var result = await Call("object_rawcode_suggest", Args(("map", map), ("kind", "unit"), ("count", 3)));
        var codes = Prop(result, "rawcodes").EnumerateArray().Select(c => c.GetString()).ToList();
        Assert.Equal(3, codes.Count);
        Assert.DoesNotContain("u000", codes);
    }

    [Fact, Covers("object_clone")]
    public async Task Object_clone_writes_a_copy_the_read_tools_confirm()
    {
        var map = UnitMap("uhpm", "999");
        var cloned = await Write("object_clone",
            Args(("rawcode", "u000"), ("new_rawcode", "u001"), ("kind", "unit")), map: map);

        var got = await Call("object_get", Args(("map", cloned), ("rawcode", "u001"), ("kind", "unit")));
        Assert.True(Prop(got, "found").GetBoolean());
    }

    [Fact, Covers("object_migrate")]
    public async Task Object_migrate_drops_a_field_the_target_lacks()
    {
        var schemaDir = Path.Combine(_f.Dir, $"schema-{Guid.NewGuid():N}");
        GameSchemaCommand.Save(new GameSchema(1, "smoke-build", "test",
            Array.Empty<SchemaInput>(), Array.Empty<string>(),
            new Dictionary<string, IReadOnlyList<SchemaField>>
            { ["units/unitmetadata.slk"] = new[] { SchemaFieldOf("unam") } },
            null, null), schemaDir);

        var map = UnitMap("uhpm", "999");
        var migrated = await Write("object_migrate", Args(("to", schemaDir), ("kind", "unit")), map: map);

        var got = await Call("object_get", Args(("map", migrated), ("rawcode", "u000"), ("kind", "unit")));
        var fields = Prop(got, "fields").EnumerateArray();
        // The map delta is gone; a base-game value for the same code may still show with source "base".
        Assert.DoesNotContain(fields, f =>
            string.Equals(Prop(f, "code").GetString(), "uhpm", StringComparison.OrdinalIgnoreCase)
            && string.Equals(Prop(f, "source").GetString(), "map", StringComparison.OrdinalIgnoreCase));
    }

    [Fact, Covers("object_bulk_set")]
    public async Task Object_bulk_set_writes_only_matching_objects()
    {
        var map = UnitMap("uhpm", "999");
        var edited = await Write("object_bulk_set",
            Args(("set", "uhpm=1000"), ("kind", "unit"), ("where", new[] { "uhpm=999" })), map: map);

        var got = await Call("object_get", Args(("map", edited), ("rawcode", "u000"), ("kind", "unit")));
        var uhpm = Prop(got, "fields").EnumerateArray()
            .First(f => string.Equals(Prop(f, "code").GetString(), "uhpm", StringComparison.OrdinalIgnoreCase));
        Assert.Equal("1000", Prop(uhpm, "value").GetString());
    }

    [Fact, Covers("object_export")]
    public async Task Object_export_writes_a_csv_with_a_rawcode_header()
    {
        var map = UnitMap("uhpm", "999");
        var csv = NewOut("export", ".csv");
        var result = await Call("object_export", Args(("map", map), ("out_path", csv), ("kind", "unit")));

        Assert.True(File.Exists(csv), "object_export wrote no CSV");
        Assert.Contains("rawcode", File.ReadAllText(csv));
        Assert.Equal(1, Prop(result, "objectCount").GetInt32());
    }

    [Fact, Covers("object_import")]
    public async Task Object_import_adds_a_row_and_the_read_tools_confirm()
    {
        var csv = Path.Combine(_f.Dir, $"import-{Guid.NewGuid():N}.csv");
        File.WriteAllText(csv, "rawcode,uhpm\nhfoo,777\n");

        var map = UnitMap("uhpm", "999");
        var edited = await Write("object_import", Args(("csv_path", csv), ("kind", "unit")), map: map);

        var got = await Call("object_get", Args(("map", edited), ("rawcode", "hfoo"), ("kind", "unit")));
        Assert.True(Prop(got, "found").GetBoolean());
    }

    [Fact, Covers("object_rename")]
    public async Task Object_rename_changes_the_rawcode_the_read_tools_confirm()
    {
        var map = UnitMap("uhpm", "999");
        var edited = await Write("object_rename",
            Args(("old_rawcode", "u000"), ("new_rawcode", "u001"), ("kind", "unit"), ("rewrite_refs", true)),
            map: map);

        var got = await Call("object_get", Args(("map", edited), ("rawcode", "u001"), ("kind", "unit")));
        Assert.True(Prop(got, "found").GetBoolean());
    }

    /// <summary>A map holding one custom unit u000 (base hfoo) with a single int field.</summary>
    private string UnitMap(string field, string value)
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var unit = new SimpleObjectModification
        { OldId = "hfoo".FromRawcode(), NewId = "u000".FromRawcode() };
        unit.Modifications.Add(new SimpleObjectDataModification
        { Id = field.FromRawcode(), Type = ObjectDataType.Int, Value = int.Parse(value) });
        w3u.NewUnits.Add(unit);

        var map = Path.Combine(_f.Dir, $"obj-{Guid.NewGuid():N}.w3x");
        File.WriteAllBytes(map, SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Ser(w => War3Net.Build.Extensions.BinaryWriterExtensions.Write(w, w3u)),
        }));
        return map;
    }

    private static byte[] Ser(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }

    /// <summary>A schema field row carrying only the field code, enough for a deprecated check.</summary>
    private static SchemaField SchemaFieldOf(string code) =>
        new(code, "UnitBalance", "HP", "int", "WESTRING", code, "data", 0, 0, 0,
            "", "", "", "", true, true, true, true, true);
}
