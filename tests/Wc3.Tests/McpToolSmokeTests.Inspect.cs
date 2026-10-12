// tests/Wc3.Tests/McpToolSmokeTests.Inspect.cs
// Read-only analysis (validate, lint, diff, search, script and string listings, the audits) and
// the hero definition tools. Each read is checked against content put there through MCP first,
// so an empty answer cannot pass for a correct one.
using System.Text;
using System.Text.Json;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public sealed class McpSmokeInspectTests : McpSmokeBase
{
    public McpSmokeInspectTests(SmokeFixture f) : base(f) { }

    [Fact, Covers("validate", "lint", "roundtrip")]
    public async Task A_blank_map_validates_lints_clean_and_round_trips()
    {
        var v = await Call("validate", Args(("map", Fixture)));
        Assert.True(Prop(v, "valid").GetBoolean());
        Assert.Equal(0, Prop(v, "errors").GetInt32());

        var lint = await Call("lint", Args(("map", Fixture)));
        Assert.True(Prop(lint, "ok").GetBoolean());
        // pjass needs the game's common.j, so with no install (CI) the check is skipped and says so.
        // Anywhere else the blank map's script has to compile.
        var compiles = Rows(Prop(lint, "checks")).Single(c => Prop(c, "name").GetString() == "script-compiles");
        var severity = Prop(compiles, "severity").GetString();
        Assert.True(severity == "Ok"
                    || (severity == "Warning" && Prop(compiles, "summary").GetString()!.Contains("did not run")),
            $"script-compiles was {severity}: {Prop(compiles, "summary").GetString()}");

        Assert.True(Prop(await Call("roundtrip", Args(("map", Fixture))), "faithful").GetBoolean());
    }

    [Fact, Covers("map_compat")]
    public async Task Map_compat_reports_missing_targets_and_checks_counts()
    {
        // Create a temp schema that declares MapCompatTestNative, plus copy the real target.
        // MapCompatTestNative is not in data/schemas, so it will be a finding when targeting
        // any build that does not declare it.
        var tempSchemas = Path.Combine(Path.GetTempPath(), "wc3ctl-mcp-compat-test");
        Directory.CreateDirectory(tempSchemas);
        try
        {
            var jass = new JassApi(
                Array.Empty<JassTypeDecl>(),
                new[]
                {
                    new JassNativeDecl("MapCompatTestNative", Array.Empty<JassParameter>(), "boolean", false),
                },
                Array.Empty<JassGlobalDecl>(),
                Array.Empty<JassFunctionDecl>());

            var schema = new GameSchema(1, "MyCompatBuild", "MyCompatBuild",
                Array.Empty<SchemaInput>(), new[] { "script" },
                null, (SchemaTriggers?)null,
                new Dictionary<string, JassApi>(StringComparer.Ordinal)
                {
                    ["scripts/common.j"] = jass,
                });

            var schemaDir = Path.Combine(tempSchemas, "MyCompatBuild");
            Directory.CreateDirectory(schemaDir);
            GameSchemaCommand.Save(schema, schemaDir);

            // Also add a target schema that does NOT declare MapCompatTestNative.
            var targetJass = new JassApi(
                Array.Empty<JassTypeDecl>(),
                Array.Empty<JassNativeDecl>(),
                Array.Empty<JassGlobalDecl>(),
                Array.Empty<JassFunctionDecl>());

            var targetSchema = new GameSchema(1, "TFT-v1.30.1.10211", "TFT-v1.30.1.10211",
                Array.Empty<SchemaInput>(), new[] { "script" },
                null, (SchemaTriggers?)null,
                new Dictionary<string, JassApi>(StringComparer.Ordinal)
                {
                    ["scripts/common.j"] = targetJass,
                });

            var targetSchemaDir = Path.Combine(tempSchemas, "TFT-v1.30.1.10211");
            Directory.CreateDirectory(targetSchemaDir);
            GameSchemaCommand.Save(targetSchema, targetSchemaDir);

            // Write a script that calls MapCompatTestNative.
            var scriptText = "function InitTriggers takes nothing returns nothing\n"
                + "    call MapCompatTestNative()\n"
                + "endfunction\n";
            var mapPath = Fixture + ".test.w3x";
            var bytes = SyntheticMap.Build(new Dictionary<string, byte[]>
            {
                ["war3map.j"] = Encoding.UTF8.GetBytes(scriptText),
            });
            await File.WriteAllBytesAsync(mapPath, bytes);
            try
            {
                var compat = await Call("map_compat", Args(("map", mapPath), ("target", "TFT-v1.30.1.10211"), ("schemas_dir", tempSchemas)));
                var findings = compat.EnumerateObject().Single(p => p.Name == "findings").Value.EnumerateArray().ToList();
                Assert.Single(findings);
                var el = findings[0];
                Assert.Equal("error", Prop(el, "severity").GetString());
                Assert.Equal("MapCompatTestNative", Prop(el, "name").GetString());
                Assert.True(Prop(compat, "identifiersChecked").GetInt32() > 0);
            }
            finally
            {
                if (File.Exists(mapPath)) File.Delete(mapPath);
            }
        }
        finally
        {
            if (Directory.Exists(tempSchemas)) Directory.Delete(tempSchemas, true);
        }
    }

    [Fact, Covers("diff")]
    public async Task Diff_reports_the_files_placement_added()
    {
        var populated = await PopulatedMap();
        var entries = Rows(await Call("diff", Args(("map", Fixture), ("other", populated))));
        string Change(string name) => Prop(entries.Single(e => Prop(e, "name").GetString() == name), "change").GetString()!;
        Assert.Equal("added", Change("war3mapUnits.doo"));
        Assert.Equal("added", Change("war3map.w3r"));
        Assert.Empty(Rows(await Call("diff", Args(("map", Fixture), ("other", Fixture)))));
    }

    [Fact, Covers("search")]
    public async Task Search_finds_script_lines_object_data_and_file_names()
    {
        var created = await Call("object_new", Args(("map", Fixture), ("out_path", NewOut("search")), ("base_rawcode", "hfoo"), ("kind", "unit")));
        string raw = Prop(created, "newRawcode").GetString()!;
        var map = Prop(created, "savedTo").GetString()!;
        var placed = await Write("place_unit", Args(("type_rawcode", raw), ("owner_id", 0), ("x", 0f), ("y", 0f)), map);

        var hits = Rows(await Call("search", Args(("map", placed), ("query", raw))));
        // The placement script names the new unit, and the object data holds it.
        Assert.Contains(hits, h => Prop(h, "fileName").GetString() == "war3map.j" && Prop(h, "context").GetString()!.StartsWith("line "));
        Assert.Contains(hits, h => Prop(h, "fileName").GetString() == "war3map.w3u" && Prop(h, "context").GetString()!.Contains("based on hfoo"));

        var byName = Rows(await Call("search", Args(("map", Fixture), ("query", "w3e"))));
        Assert.Contains(byName, h => Prop(h, "context").GetString() == "filename");
        Assert.Empty(Rows(await Call("search", Args(("map", Fixture), ("query", "zzqqxx-not-anywhere")))));
    }

    [Fact, Covers("script_functions", "script_references")]
    public async Task Script_functions_and_references_read_the_blank_script()
    {
        var fns = Rows(Prop(await Call("script_functions", Args(("map", Fixture))), "functions"));
        var main = fns.Single(f => Prop(f, "name").GetString() == "main");
        Assert.StartsWith("function main takes nothing", Prop(main, "signature").GetString());

        var refs = await Call("script_references", Args(("map", Fixture), ("name", "InitCustomPlayerSlots")));
        var call = Assert.Single(Rows(Prop(refs, "references")));
        Assert.Equal("config", Prop(call, "inFunction").GetString());
        Assert.Equal("Call", Prop(call, "kind").GetString());
    }

    [Fact, Covers("strings_list", "imports_list")]
    public async Task String_and_import_listings_see_what_the_map_holds()
    {
        // A blank map has no string table, and says so with an empty list rather than an error.
        Assert.Empty(Rows(await Call("strings_list", Args(("map", Fixture)))));

        var asset = NewOut("icon", ".blp");
        File.WriteAllBytes(asset, new byte[] { 0x42, 0x4C, 0x50, 0x31 });
        var withImport = await Write("file_set", Args(("internal_path", @"war3mapImported\Smoke.blp"), ("from", asset)));
        var imports = await Call("imports_list", Args(("map", withImport)));
        Assert.False(Prop(imports, "hasManifest").GetBoolean());
        Assert.Contains(Rows(Prop(imports, "entries")), e => Prop(e, "path").GetString()!.EndsWith("Smoke.blp"));
    }

    [Fact, Covers("hero_roster", "repair_generated")]
    public async Task Roster_and_generated_repair_say_why_a_blank_map_has_nothing()
    {
        var roster = await Call("hero_roster", Args(("map", Fixture)));
        Assert.False(Prop(roster, "ok").GetBoolean());
        Assert.Empty(Rows(Prop(roster, "heroes")));
        Assert.Contains("roster", Prop(roster, "message").GetString());

        var refusal = await CallError("repair_generated", Args(("map", Fixture), ("out_path", NewOut("repair"))));
        Assert.Contains("helper block", refusal);
    }

    /// <summary>A custom hero derived from the Paladin, placed once, made through MCP.</summary>
    private async Task<(string Map, string Hero)> HeroMap()
    {
        var created = await Call("object_new", Args(("map", Fixture), ("out_path", NewOut("hero")), ("base_rawcode", "Hpal"), ("kind", "unit")));
        string hero = Prop(created, "newRawcode").GetString()!;
        var placed = await Write("place_unit", Args(("type_rawcode", hero), ("owner_id", 0), ("x", 0f), ("y", 0f)), Prop(created, "savedTo").GetString());
        return (placed, hero);
    }

    [Fact, Trait("Category", "GameData"), Covers("audit_hero", "audit_ability", "audit_readiness")]
    public async Task The_hero_audits_find_the_placed_hero()
    {
        var (map, hero) = await HeroMap();
        var wiring = Assert.Single(Rows(await Call("audit_hero", Args(("map", map)))));
        Assert.Equal(hero, Prop(wiring, "hero").GetString());
        Assert.Empty(Rows(Prop(wiring, "problems")));

        var ability = Assert.Single(Rows(await Call("audit_ability", Args(("map", map), ("hero", hero)))));
        Assert.Equal(hero, Prop(ability, "hero").GetString());

        var ready = Assert.Single(Rows(await Call("audit_readiness", Args(("map", map)))));
        Assert.True(Prop(ready, "ready").GetBoolean());
    }

    [Fact, Trait("Category", "GameData"), Covers("hero_lint", "hero_install", "audit_fidelity")]
    public async Task A_hero_exports_lints_installs_and_survives_the_port()
    {
        var (map, hero) = await HeroMap();
        // Export is CLI-only by design (it writes a folder), so it runs in process as setup.
        var definition = Path.Combine(_f.Dir, "hero-" + Guid.NewGuid().ToString("N"));
        HeroExportCommand.Run(MapDocument.Load(map), hero, map, definition);

        var lint = await Call("hero_lint", Args(("definition", definition)));
        Assert.True(Prop(lint, "ok").GetBoolean(), lint.ToString());
        Assert.Equal(0, Prop(lint, "errors").GetInt32());

        var installed = await Write("hero_install", Args(("definition", definition)));
        var units = Rows(await Call("object_list", Args(("map", installed), ("kind", "unit"))));
        Assert.Contains(units, u => Prop(u, "rawcode").GetString() == hero && Prop(u, "baseRawcode").GetString() == "Hpal");

        var fidelity = await Call("audit_fidelity", Args(("source_map", map), ("target_map", installed), ("rawcode", hero)));
        Assert.True(Prop(fidelity, "faithful").GetBoolean());
        Assert.Equal(0, Prop(fidelity, "errors").GetInt32());

        var empty = await Call("hero_lint", Args(("definition", _f.Dir)));
        Assert.False(Prop(empty, "ok").GetBoolean());
    }

    [Fact, Trait("Category", "GameData"), Covers("object_form", "object_field_options")]
    public async Task Object_form_and_field_options_come_from_the_game_data()
    {
        var form = await Call("object_form", Args(("map", Fixture), ("rawcode", "hfoo")));
        Assert.Equal("Footman", Prop(form, "name").GetString());
        Assert.NotEmpty(Rows(Prop(form, "groups")));

        var options = Rows(Prop(await Call("object_field_options", Args(("kind", "unit"), ("field", "utyp"))), "options"));
        Assert.Contains(options, o => Prop(o, "value").GetString() == "undead");
    }

    [Fact, Trait("Category", "GameData"), Covers("asset_list", "editor_catalog_list", "editor_catalog_get")]
    public async Task Asset_and_editor_catalogs_list_the_installs_content()
    {
        var icons = await Call("asset_list", Args(("family", "icon")));
        Assert.Contains(Prop(icons, "gamePaths").EnumerateArray(), p => p.GetString()!.Contains("btn", StringComparison.OrdinalIgnoreCase));

        var catalogs = Rows(Prop(await Call("editor_catalog_list", Args()), "catalogs"));
        Assert.Contains(catalogs, c => Prop(c, "name").GetString() == "TileSets");

        var tilesets = Rows(Prop(await Call("editor_catalog_get", Args(("name", "TileSets"))), "entries"));
        Assert.Contains(tilesets, t => Prop(t, "displayName").GetString() == "Ashenvale");
        await CallError("editor_catalog_get", Args(("name", "NoSuchCatalog")));
    }

    [Fact, Trait("Category", "GameData"), Covers("target_get", "target_set", "target_list")]
    public async Task Target_tools_view_and_set_the_current_target()
    {
        // Determine schemas directory: env override > test output > default location.
        string? schemasDir = Environment.GetEnvironmentVariable("WC3_SCHEMAS_DIR");
        if (string.IsNullOrWhiteSpace(schemasDir))
        {
            var schemasPath = Path.Combine(AppContext.BaseDirectory, "data", "schemas");
            if (Directory.Exists(schemasPath))
                schemasDir = schemasPath;
        }

        // If no schemas, this test cannot validate the feature.
        if (schemasDir is null)
        {
            throw new Xunit.Sdk.XunitException("no schemas directory available");
        }

        var args = new Dictionary<string, object?> { ["schemas_dir"] = schemasDir };

        // target_get returns the build and source.
        var get = await Call("target_get", args);
        var build = Prop(get, "build").GetString();
        Assert.False(string.IsNullOrEmpty(build));
        var source = Prop(get, "source").GetString();
        Assert.True(source == "default" || source == "settings" || source == "environment",
            "source was " + source);

        // target_list returns an array of builds.
        var list = await Call("target_list", args);
        var builds = list.EnumerateArray().ToList();
        Assert.True(builds.Any());

        // target_set with an invalid build fails.
        var badSet = await Call("target_set", Args(("build", "NoSuchBuild12345"), ("schemas_dir", schemasDir)));
        Assert.False(Prop(badSet, "ok").GetBoolean());

        // target_set with a valid build succeeds.
        // Use the first build from target_list.
        string firstBuild = builds.First().GetProperty("build").GetString()!;
        var goodSet = await Call("target_set", Args(("build", firstBuild), ("schemas_dir", schemasDir)));
        Assert.True(Prop(goodSet, "ok").GetBoolean());

        // Verify target_get now shows settings as source.
        var get2 = await Call("target_get", args);
        Assert.Equal("settings", Prop(get2, "source").GetString());
    }

    [Fact, Covers("validate_strict")]
    public async Task Validate_strict_answers_on_the_fixture()
    {
        var r = await Call("validate_strict", Args(("map", Fixture)));
        Assert.True(Prop(r, "valid").GetBoolean());
        Assert.Equal(0, Prop(r, "errors").GetInt32());
    }

    [Fact, Covers("lint_baseline")]
    public async Task Lint_baseline_writes_a_file_the_gate_accepts()
    {
        var outPath = Path.Combine(_f.Dir, "baseline-" + Guid.NewGuid().ToString("N") + ".json");
        var r = await Call("lint_baseline", Args(("map", Fixture), ("out_path", outPath)));
        Assert.True(Prop(r, "findings").GetInt32() >= 0);
        Assert.True(File.Exists(outPath));
        Assert.True(LintBaselineCommand.Gate(Fixture, outPath).Ok);
    }

    [Fact, Covers("audit_trend")]
    public async Task Audit_trend_of_a_map_against_itself_changes_nothing()
    {
        var r = await Call("audit_trend", Args(("map", Fixture), ("other", Fixture)));
        Assert.Equal(0, Prop(r, "added").GetInt32());
        Assert.Equal(0, Prop(r, "removed").GetInt32());
    }

    [Fact, Covers("compat_matrix", "compat_explain")]
    public async Task Compat_matrix_and_explain_use_the_temp_schema()
    {
        var tempSchemas = Path.Combine(_f.Dir, "schemas-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempSchemas);
        try
        {
            var jass = new JassApi(
                Array.Empty<JassTypeDecl>(),
                new[] { new JassNativeDecl("SmokeExplainNative", Array.Empty<JassParameter>(), "boolean", false) },
                Array.Empty<JassGlobalDecl>(),
                Array.Empty<JassFunctionDecl>());
            var schema = new GameSchema(1, "SmokeBuild", "SmokeBuild",
                Array.Empty<SchemaInput>(), new[] { "script" },
                null, (SchemaTriggers?)null,
                new Dictionary<string, JassApi>(StringComparer.Ordinal) { ["scripts/common.j"] = jass });
            var dir = Path.Combine(tempSchemas, "SmokeBuild");
            Directory.CreateDirectory(dir);
            GameSchemaCommand.Save(schema, dir);

            var scriptText = "function main takes nothing returns nothing\n"
                + "    call SmokeExplainNative()\nendfunction\n";
            var mapPath = Path.Combine(_f.Dir, "compat-matrix-" + Guid.NewGuid().ToString("N") + ".w3x");
            await File.WriteAllBytesAsync(mapPath, SyntheticMap.Build(new Dictionary<string, byte[]>
            {
                ["war3map.j"] = Encoding.UTF8.GetBytes(scriptText),
            }));
            try
            {
                var matrix = await Call("compat_matrix", Args(
                    ("maps", new[] { mapPath }),
                    ("targets", new[] { "SmokeBuild" }),
                    ("schemas_dir", tempSchemas)));
                var cells = matrix.EnumerateObject().Single(p => p.Name == "cells").Value.EnumerateArray().ToList();
                Assert.True(Prop(Assert.Single(cells), "pass").GetBoolean());

                var explain = await Call("compat_explain", Args(
                    ("name", "SmokeExplainNative"),
                    ("target", "SmokeBuild"),
                    ("schemas_dir", tempSchemas)));
                Assert.True(Prop(explain, "isIdentifier").GetBoolean());
                Assert.True(Prop(explain, "declaredInTarget").GetBoolean());
            }
            finally
            {
                if (File.Exists(mapPath)) File.Delete(mapPath);
            }
        }
        finally
        {
            if (Directory.Exists(tempSchemas)) Directory.Delete(tempSchemas, true);
        }
    }
}
