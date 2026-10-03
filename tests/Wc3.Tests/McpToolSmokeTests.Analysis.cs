// tests/Wc3.Tests/McpToolSmokeTests.Analysis.cs
// Analysis tools. A richer map is built through MCP itself (a custom unit with a custom ability, a
// second ability the script hands out, and a script with a leak), so each tool has something real
// to find. Tools that read the Warcraft III install are tagged GameData.
using System.Text.Json;
using ModelContextProtocol.Protocol;

namespace Wc3.Tests;

public sealed partial class McpToolSmokeTests
{
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
}
