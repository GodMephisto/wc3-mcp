// tests/Wc3.Tests/McpToolSmokeTests.Gamedata.cs
// The read-only gamedata MCP tools, exercised through the protocol against temporary schema folders
// built on disk, so the coverage check has a smoke test for each and no test needs data/schemas.
namespace Wc3.Tests;

public sealed class McpSmokeGameDataTests : McpSmokeBase
{
    public McpSmokeGameDataTests(SmokeFixture f) : base(f) { }

    /// <summary>Two builds, an object field code shared by both and one identifier in each.</summary>
    private string BuildSchemas()
    {
        var root = Path.Combine(_f.Dir, "gamedata-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        GamedataFixtures.Save(root, "TFT-v1.30.1.10211", "units/unitmetadata.slk",
            new[] { GamedataFixtures.Field("Cool1", "int", "0", "100", true) },
            natives: new[] { "NativeA" });
        GamedataFixtures.Save(root, "TFT-v1.31.1.12173", "units/unitmetadata.slk",
            new[] { GamedataFixtures.Field("Cool1", "real", "0", "100", true) },
            functions: new[] { "FuncB" });
        return root;
    }

    [Fact, Covers("gamedata_history")]
    public async Task Gamedata_history_reports_a_field_code()
    {
        var root = BuildSchemas();
        var r = await Call("gamedata_history", Args(("name", "Cool1"), ("schemas_dir", root)));
        Assert.True(Prop(r, "found").GetBoolean());
        var kinds = Rows(Prop(r, "kinds"));
        var field = Assert.Single(kinds);
        Assert.Equal("field", Prop(field, "kind").GetString());
        Assert.Equal(2, Prop(field, "builds").GetArrayLength());
    }

    [Fact, Covers("gamedata_changelog")]
    public async Task Gamedata_changelog_reports_a_retype()
    {
        var root = BuildSchemas();
        var r = await Call("gamedata_changelog",
            Args(("build_a", "TFT-v1.30.1.10211"), ("build_b", "TFT-v1.31.1.12173"), ("schemas_dir", root)));
        var groups = Rows(Prop(r, "groups"));
        var group = Assert.Single(groups);
        var changes = Rows(Prop(group, "changes"));
        var change = Assert.Single(changes);
        Assert.Equal("retyped", Prop(change, "change").GetString());
        Assert.Equal(1, Prop(r, "retyped").GetInt32());
    }

    [Fact, Covers("gamedata_field_lookup")]
    public async Task Gamedata_field_lookup_matches_a_display_name()
    {
        var root = BuildSchemas();
        var r = await Call("gamedata_field_lookup", Args(("query", "cool"), ("schemas_dir", root)));
        var matches = Rows(Prop(r, "matches"));
        Assert.Equal(2, matches.Count);
        Assert.All(matches, m => Assert.Equal("Cool1", Prop(m, "code").GetString()));
    }

    [Fact, Covers("gamedata_bounds")]
    public async Task Gamedata_bounds_reports_each_build()
    {
        var root = BuildSchemas();
        var r = await Call("gamedata_bounds", Args(("code", "Cool1"), ("schemas_dir", root)));
        var entries = Rows(Prop(r, "entries"));
        Assert.Equal(2, entries.Count);
        Assert.Equal("real", Prop(entries[1], "type").GetString());
    }

    [Fact, Covers("gamedata_coverage")]
    public async Task Gamedata_coverage_lists_every_build()
    {
        var root = BuildSchemas();
        var r = await Call("gamedata_coverage", Args(("schemas_dir", root)));
        var rows = Rows(Prop(r, "rows"));
        Assert.Equal(2, rows.Count);
        Assert.All(rows, row => Assert.Contains("script", Prop(row, "sections").EnumerateArray()
            .Select(s => s.GetString())));
    }

    [Fact, Covers("gamedata_sources")]
    public async Task Gamedata_sources_reports_a_hash_per_section_file()
    {
        var root = BuildSchemas();
        var r = await Call("gamedata_sources", Args(("schemas_dir", root)));
        var sources = Rows(Prop(r, "sources"));
        Assert.Equal(2, sources.Count);
        var full = sources.Single(s => Prop(s, "build").GetString() == "TFT-v1.30.1.10211");
        var hashes = Rows(Prop(full, "sectionHashes"));
        Assert.Contains(hashes, h => Prop(h, "section").GetString() == "objects");
        Assert.Contains(hashes, h => Prop(h, "section").GetString() == "script");
    }

    [Fact, Covers("gamedata_export_schemas")]
    public async Task Gamedata_export_schemas_packs_a_zip()
    {
        var root = BuildSchemas();
        var zip = Path.Combine(_f.Dir, "export-" + Guid.NewGuid().ToString("N") + ".zip");
        var r = await Call("gamedata_export_schemas", Args(("out_path", zip), ("schemas_dir", root)));
        Assert.True(Prop(r, "zipped").GetBoolean());
        Assert.True(File.Exists(zip));
        Assert.Equal(2, Prop(r, "builds").GetArrayLength());
    }
}
