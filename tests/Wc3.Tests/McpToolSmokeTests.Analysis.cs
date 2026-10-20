// tests/Wc3.Tests/McpToolSmokeTests.Analysis.cs
// Analysis tools. A richer map is built through MCP itself (a custom unit with a custom ability, a
// second ability the script hands out, and a script with a leak), so each tool has something real
// to find. Tools that read the Warcraft III install are tagged GameData.
using System.Text.Json;
using ModelContextProtocol.Protocol;

namespace Wc3.Tests;

public sealed class McpSmokeAnalysisTests : McpSmokeBase
{
    public McpSmokeAnalysisTests(SmokeFixture f) : base(f) { }

    private const string GameDir = @"C:\Warcraft III";

    /// <summary>The rich map and the rawcodes it defines.</summary>
    private sealed record RichMap(string Path, string Unit, string OwnAbility, string ScriptAbility);

    private async Task<RichMap> BuildRichMap()
    {
        async Task<(string Out, string Raw)> New(string map, string baseRaw, string kind)
        {
            var o = NewOut("rich");
            var r = await Call("object_new", Args(("map", map), ("out_path", o), ("base_rawcode", baseRaw), ("kind", kind)));
            return (o, Prop(r, "newRawcode").GetString()!);
        }

        var (m1, unit) = await New(Fixture, "hfoo", "unit");
        var (m2, own) = await New(m1, "AHbz", "ability");
        var (m3, given) = await New(m2, "AHtb", "ability");
        var m4 = NewOut("rich");
        await Call("object_set", Args(("map", m3), ("out_path", m4), ("rawcode", unit), ("field", "uabi"), ("value", own), ("kind", "unit")));

        var script = Prop(await Call("file_get_text", Args(("map", m4), ("internal_path", "war3map.j"))), "text").GetString()!;
        string extra = string.Join("\n",
            "function SmokeLeak takes nothing returns nothing",
            "    local location l = GetRectCenter(GetPlayableMapRect())",
            $"    call CreateUnitAtLoc(Player(0), '{unit}', l, 0)",
            "endfunction",
            "function SmokeGive takes unit u returns nothing",
            $"    if GetUnitTypeId(u) == '{unit}' then",
            $"        call UnitAddAbility(u, '{given}')",
            "    endif",
            "endfunction",
            "");
        int main = script.IndexOf("function main takes nothing", StringComparison.Ordinal);
        Assert.True(main > 0, "blank script has no main");
        var scriptFile = NewOut("script", ".j");
        File.WriteAllText(scriptFile, script[..main] + extra + script[main..]);
        var m5 = NewOut("rich");
        await Call("file_set", Args(("map", m4), ("out_path", m5), ("internal_path", "war3map.j"), ("from", scriptFile)));
        return new RichMap(m5, unit, own, given);
    }

    [Fact, Covers("unit_abilities")]
    public async Task Unit_abilities_lists_the_object_ability_and_the_script_given_one()
    {
        var rich = await BuildRichMap();
        var r = await Call("unit_abilities", Args(("map", rich.Path), ("rawcode", rich.Unit)));
        Assert.True(Prop(r, "found").GetBoolean());
        var abilities = Rows(Prop(r, "abilities"));

        var own = Assert.Single(abilities, a => Prop(a, "rawcode").GetString() == rich.OwnAbility);
        Assert.Equal("ObjectNormal", Prop(own, "source").GetString());
        Assert.False(Prop(own, "inferred").GetBoolean());

        var given = Assert.Single(abilities, a => Prop(a, "rawcode").GetString() == rich.ScriptAbility);
        Assert.Equal("Script", Prop(given, "source").GetString());
        Assert.True(Prop(given, "inferred").GetBoolean());
        Assert.StartsWith("war3map.j:", Prop(given, "detail").GetString());
    }

    [Fact, Covers("unit_abilities")]
    public async Task Unit_abilities_for_a_missing_placed_unit_reports_not_found()
    {
        var r = await Call("unit_abilities", Args(("map", Fixture), ("rawcode", "hfoo"), ("creation_number", 999)));
        Assert.False(Prop(r, "found").GetBoolean());
    }

    [Fact, Covers("script_leaks")]
    public async Task Script_leaks_finds_the_location_that_is_never_removed()
    {
        var rich = await BuildRichMap();
        var r = await Call("script_leaks", Args(("map", rich.Path)));
        Assert.True(Prop(r, "ok").GetBoolean());
        var leaks = Rows(Prop(r, "leaks"));
        Assert.Contains(leaks, l => Prop(l, "function").GetString() == "SmokeLeak");
    }

    [Fact, Covers("bundle_unit")]
    public async Task Bundle_unit_includes_the_units_own_ability()
    {
        var rich = await BuildRichMap();
        var r = await Call("bundle_unit", Args(("map", rich.Path), ("rawcode", rich.Unit)));
        Assert.Equal(rich.Unit, Prop(r, "rootRawcode").GetString());
        var objects = Rows(Prop(r, "objects")).Select(o => Prop(o, "rawcode").GetString()).ToList();
        Assert.Contains(rich.OwnAbility, objects);
    }

    [Fact, Covers("port_unit")]
    public async Task Port_unit_copies_the_unit_into_a_new_target_file()
    {
        var rich = await BuildRichMap();
        var targetBefore = Sha(Fixture);
        var r = await Call("port_unit", Args(("source_map", rich.Path), ("rawcode", rich.Unit), ("target_map", Fixture)));
        Assert.Equal(targetBefore, Sha(Fixture));
        var saved = Prop(r, "savedTo").GetString()!;
        try
        {
            var ported = Prop(Prop(r, "report"), "rootPortedTo").GetString()!;
            var got = await Call("object_get", Args(("map", saved), ("rawcode", ported), ("kind", "unit")));
            Assert.True(Prop(got, "found").GetBoolean());
        }
        finally { File.Delete(saved); }
    }

    [Fact, Covers("audit_map")]
    public async Task Audit_map_checks_the_custom_objects()
    {
        var rich = await BuildRichMap();
        var r = await Call("audit_map", Args(("map", rich.Path)));
        Assert.True(Prop(r, "objectsChecked").GetInt32() >= 1);
    }

    [Fact, Covers("uabi_profile")]
    public async Task Uabi_profile_counts_the_unit_with_an_ability_list()
    {
        var rich = await BuildRichMap();
        var r = await Call("uabi_profile", Args(("paths", new[] { rich.Path })));
        var map = Assert.Single(Rows(Prop(r, "maps")));
        Assert.True(Prop(map, "withUabi").GetInt32() >= 1);
    }

    [Fact, Covers("deprotect_map")]
    public async Task Deprotect_map_reports_the_blank_maps_blocks_without_writing()
    {
        var r = await Call("deprotect_map", Args(("map", Fixture)));
        Assert.True(Prop(r, "totalBlocks").GetInt32() > 0);
        Assert.True(Prop(r, "namedAfter").GetInt32() >= Prop(r, "namedBefore").GetInt32());
    }

    [Fact, Covers("replay_summary")]
    public async Task Replay_summary_refuses_a_path_that_does_not_exist()
    {
        var message = await CallError("replay_summary", Args(("path", Path.Combine(_f.Dir, "no-such.w3g"))));
        Assert.Contains("no-such.w3g", message);
    }

    [Fact, Covers("replay_summary")]
    public async Task Replay_summary_of_a_folder_with_no_replays_is_empty()
    {
        var empty = Directory.CreateDirectory(Path.Combine(_f.Dir, "replays-" + Guid.NewGuid().ToString("N"))).FullName;
        var r = await Call("replay_summary", Args(("path", empty)));
        Assert.Empty(Rows(Prop(r, "games")));
    }

    /// <summary>A synthetic .w3g in the fixture folder, so the replay tools need no real game.</summary>
    private string SyntheticReplay(params ReplayFixtures.Event[] events)
    {
        string path = Path.Combine(_f.Dir, $"replay-{Guid.NewGuid():N}.w3g");
        File.WriteAllBytes(path, ReplayFixtures.Build(events: events));
        return path;
    }

    [Fact, Covers("replay_players")]
    public async Task Replay_players_lists_each_player_and_their_leave()
    {
        var replay = SyntheticReplay(new ReplayFixtures.Leave(1000, 2, 0x0C, 0x07));
        var r = await Call("replay_players", Args(("replay", replay)));
        var players = Rows(Prop(r, "players"));
        Assert.Equal(2, players.Count);
        var guest = Assert.Single(players, p => Prop(p, "slot").GetInt32() == 2);
        Assert.Equal("Guest#2", Prop(guest, "name").GetString());
        Assert.Equal("left", Prop(guest, "leaveMeaning").GetString());
    }

    [Fact, Covers("replay_timeline")]
    public async Task Replay_timeline_orders_start_chat_leave_and_end()
    {
        var replay = SyntheticReplay(
            new ReplayFixtures.Chat(500, 1, "hello"),
            new ReplayFixtures.Leave(1000, 2, 0x0C, 0x07));
        var r = await Call("replay_timeline", Args(("replay", replay)));
        var events = Rows(Prop(r, "events"));
        Assert.Equal(new[] { "game start", "chat", "leave", "game end" },
            events.Select(e => Prop(e, "kind").GetString()).ToArray());
    }

    [Fact, Covers("replay_diff")]
    public async Task Replay_diff_reports_a_length_delta_between_two_games()
    {
        var a = SyntheticReplay(new ReplayFixtures.Leave(500, 2, 0x0C, 0x07));
        var b = SyntheticReplay(new ReplayFixtures.Leave(900, 2, 0x0C, 0x07));
        var r = await Call("replay_diff", Args(("a", a), ("b", b)));
        Assert.True(Prop(r, "sameMap").GetBoolean());
        var delta = Prop(r, "leaveDeltas").EnumerateArray()
            .Single(d => Prop(d, "playerId").GetInt32() == 2).GetProperty("delta").GetString();
        Assert.Equal(TimeSpan.FromMilliseconds(400), TimeSpan.Parse(delta!));
    }

    [Fact, Covers("replay_find_map")]
    public async Task Replay_find_map_resolves_the_recorded_map_in_a_folder()
    {
        var folder = Directory.CreateDirectory(Path.Combine(_f.Dir, "maps-" + Guid.NewGuid().ToString("N"))).FullName;
        Directory.CreateDirectory(Path.Combine(folder, "Download"));
        File.WriteAllBytes(Path.Combine(folder, "Download", "BaseMap_Repro.w3x"), new byte[] { 1 });
        var replay = SyntheticReplay();
        var r = await Call("replay_find_map", Args(("replay", replay), ("maps", folder)));
        Assert.True(Prop(r, "found").GetBoolean());
    }

    [Fact, Covers("replay_stats")]
    public async Task Replay_stats_aggregates_duration_players_and_disconnects()
    {
        var replay = SyntheticReplay(new ReplayFixtures.Leave(3000, 2, 0x0C, 0x01));
        var r = await Call("replay_stats", Args(("replay", replay)));
        Assert.Equal(2, Prop(r, "playerCount").GetInt32());
        Assert.Equal(1, Prop(r, "disconnects").GetInt32());
    }

    // The trigger catalog is read from the install's ui\triggerdata.txt.
    [Fact, Trait("Category", "GameData"), Covers("trigger_catalog_list", "trigger_catalog_describe")]
    public async Task Trigger_catalog_lists_and_describes_a_known_action()
    {
        var list = await Call("trigger_catalog_list", Args(("game_dir", GameDir), ("search", "CreateNUnitsAtLoc")));
        Assert.Contains(Rows(Prop(list, "functions")), f => Prop(f, "name").GetString() == "CreateNUnitsAtLoc");
        var one = await Call("trigger_catalog_describe", Args(("name", "CreateNUnitsAtLoc"), ("game_dir", GameDir)));
        Assert.Equal("CreateNUnitsAtLoc", Prop(one, "name").GetString());
    }

    // The doodad palette is the base game's doodad catalog.
    [Fact, Trait("Category", "GameData"), Covers("palette_doodad")]
    public async Task Palette_doodad_lists_base_game_doodads()
    {
        var r = await Call("palette_doodad", Args(("map", Fixture), ("game_dir", GameDir)));
        Assert.True(Prop(r, "ok").GetBoolean());
        Assert.NotEmpty(Rows(Prop(r, "entries")));
    }

    // A footman's model lives in the install, and the picture must come back as a real PNG.
    [Fact, Trait("Category", "GameData"), Covers("render_model")]
    public async Task Render_model_writes_a_png_and_returns_it_inline()
    {
        var png = NewOut("render", ".png");
        byte[]? inline = null;
        string? text = null;
        await McpTestClient.WithClient(async (client, ct) =>
        {
            var r = await client.CallToolAsync("render_model",
                Args(("map", Fixture), ("rawcode", "hfoo"), ("out_path", png), ("game_dir", GameDir)), cancellationToken: ct);
            text = r.Content.OfType<TextContentBlock>().FirstOrDefault()?.Text;
            Assert.True(r.IsError != true, text);
            inline = r.Content.OfType<ImageContentBlock>().FirstOrDefault()?.DecodedData.ToArray();
        });
        byte[] signature = { 0x89, (byte)'P', (byte)'N', (byte)'G' };
        Assert.Equal(signature, File.ReadAllBytes(png)[..4]);
        Assert.NotNull(inline);
        Assert.Equal(signature, inline![..4]);
        Assert.Equal(File.ReadAllBytes(png), inline);
    }

    [Fact, Covers("gamedata_schema_diff")]
    public async Task Gamedata_schema_diff_returns_zero_changes_for_identical_schema_folders()
    {
        var schemaA = NewOut("schema-a");
        var schemaB = NewOut("schema-b");
        Directory.CreateDirectory(schemaA);
        Directory.CreateDirectory(schemaB);
        // Both folders get the same minimal manifest so the tool has something to load.
        File.WriteAllText(Path.Combine(schemaA, "manifest.json"), @"{""schemaVersion"":1,""build"":""3.0.0.24268"",""source"":""test""}");
        File.WriteAllText(Path.Combine(schemaB, "manifest.json"), @"{""schemaVersion"":1,""build"":""3.0.0.24268"",""source"":""test""}");
        var r = await Call("gamedata_schema_diff", Args(("a", schemaA), ("b", schemaB)));
        Assert.Equal(0, Prop(r, "changes").GetArrayLength());
    }

    [Fact, Covers("gamedata_schema_diff")]
    public async Task Gamedata_schema_diff_filters_by_section()
    {
        var schemaA = NewOut("schema-section");
        var schemaB = NewOut("schema-section");
        Directory.CreateDirectory(schemaA);
        Directory.CreateDirectory(schemaB);
        File.WriteAllText(Path.Combine(schemaA, "manifest.json"), @"{""schemaVersion"":1,""build"":""3.0.0.24268"",""source"":""test""}");
        File.WriteAllText(Path.Combine(schemaB, "manifest.json"), @"{""schemaVersion"":1,""build"":""3.0.0.24269"",""source"":""test""}");
        // With a section filter, objects changes should be excluded when both have no objects.
        var r = await Call("gamedata_schema_diff", Args(("a", schemaA), ("b", schemaB), ("section", "objects")));
        Assert.Equal(0, Prop(r, "changes").GetArrayLength());
    }

    /// <summary>A map whose war3map.j carries the shapes the script toolkit commands look for.</summary>
    private async Task<string> ScriptedMap()
    {
        var script = Prop(await Call("file_get_text", Args(("map", Fixture), ("internal_path", "war3map.j"))), "text").GetString()!;
        int g = script.IndexOf("endglobals", StringComparison.Ordinal);
        string globalsBlock = "    integer udg_NeverRead = 0\n    integer udg_ReadOnce = 0\n";
        script = script[..g] + globalsBlock + script[g..];
        int main = script.IndexOf("function main takes nothing", StringComparison.Ordinal);
        string extra = string.Join("\n",
            "function SmokeNullLocal takes nothing returns nothing",
            "    local group g = CreateGroup()",
            "    call DestroyGroup(g)",
            "endfunction",
            "function SmokeComplex takes integer n returns integer",
            "    if n > 0 and n < 10 then",
            "        return 1",
            "    endif",
            "    return 0",
            "endfunction",
            "function SmokeUnused takes nothing returns nothing",
            "endfunction",
            "function SmokeReadsGlobal takes nothing returns nothing",
            "    set udg_ReadOnce = GetRandomInt(1, 2)",
            "    call BJDebugMsg(I2S(udg_ReadOnce))",
            "endfunction",
            "");
        script = script[..main] + extra + script[main..];
        var file = NewOut("scripted", ".j");
        File.WriteAllText(file, script);
        var outMap = NewOut("scripted");
        await Call("file_set", Args(("map", Fixture), ("out_path", outMap), ("internal_path", "war3map.j"), ("from", file)));
        return outMap;
    }

    [Fact, Covers("script_unused_globals")]
    public async Task Script_unused_globals_reports_the_global_never_read()
    {
        var map = await ScriptedMap();
        var r = await Call("script_unused_globals", Args(("map", map)));
        var names = Rows(Prop(r, "globals")).Select(g => Prop(g, "name").GetString()).ToList();
        Assert.Contains("udg_NeverRead", names);
        Assert.DoesNotContain("udg_ReadOnce", names);
    }

    [Fact, Covers("script_unused_functions")]
    public async Task Script_unused_functions_reports_the_uncalled_function()
    {
        var map = await ScriptedMap();
        var r = await Call("script_unused_functions", Args(("map", map)));
        var names = Rows(Prop(r, "functions")).Select(f => Prop(f, "name").GetString()).ToList();
        Assert.Contains("SmokeUnused", names);
        Assert.DoesNotContain("main", names);
    }

    [Fact, Covers("script_null_locals")]
    public async Task Script_null_locals_reports_the_destroyed_but_unset_local()
    {
        var map = await ScriptedMap();
        var r = await Call("script_null_locals", Args(("map", map)));
        var rows = Rows(Prop(r, "locals"));
        Assert.Contains(rows, l => Prop(l, "function").GetString() == "SmokeNullLocal");
    }

    [Fact, Covers("script_complexity")]
    public async Task Script_complexity_ranks_the_branching_function()
    {
        var map = await ScriptedMap();
        var r = await Call("script_complexity", Args(("map", map)));
        var rows = Rows(Prop(r, "functions"));
        Assert.Contains(rows, f => Prop(f, "function").GetString() == "SmokeComplex"
            && Prop(f, "complexity").GetInt32() >= 2);
    }

    [Fact, Covers("script_callgraph")]
    public async Task Script_callgraph_lists_edges_between_declared_functions()
    {
        var map = await ScriptedMap();
        var r = await Call("script_callgraph", Args(("map", map)));
        Assert.True(Prop(r, "edges").GetInt32() >= 1);
    }

    [Fact, Covers("script_verify", "script_bench")]
    public async Task Verify_and_bench_answer_on_the_scripted_map()
    {
        var map = await ScriptedMap();
        var verify = await Call("script_verify", Args(("map", map)));
        Assert.NotNull(Prop(verify, "findings"));
        var bench = await Call("script_bench", Args(("map", map)));
        Assert.NotNull(Prop(bench, "functions"));
    }

    [Fact, Trait("Category", "GameData"), Covers("repair_audit_errors", "repair_portraits")]
    public async Task Repairs_write_a_copy_and_leave_the_input_alone()
    {
        var audit = await Call("repair_audit_errors", Args(("map", Fixture), ("out_path", NewOut("audit-fix"))));
        Assert.NotNull(audit);
        var portraits = await Call("repair_portraits", Args(("map", Fixture), ("out_path", NewOut("portrait-fix"))));
        Assert.NotNull(portraits);
    }
}
