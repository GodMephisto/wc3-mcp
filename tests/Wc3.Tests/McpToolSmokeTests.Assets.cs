// tests/Wc3.Tests/McpToolSmokeTests.Assets.cs
// The asset inspection and replacement tools. Unused, duplicates, audit and size-report read a
// map's imported files. Asset_replace rewrites a path reference into an edited copy. Every read
// is checked against an import put there through MCP first, so an empty answer cannot pass.
namespace Wc3.Tests;

public sealed class McpSmokeAssetTests : McpSmokeBase
{
    public McpSmokeAssetTests(SmokeFixture f) : base(f) { }

    [Fact, Covers("asset_unused", "asset_duplicates", "asset_audit", "asset_size_report")]
    public async Task Asset_scans_see_an_import_and_a_blank_map_has_no_duplicates()
    {
        var asset = NewOut("smoke", ".blp");
        File.WriteAllBytes(asset, new byte[] { 0x42, 0x4C, 0x50, 0x31 });
        var map = await Write("file_set",
            Args(("internal_path", @"war3mapImported\Smoke.blp"), ("from", asset)));

        var unused = await Call("asset_unused", Args(("map", map)));
        Assert.Contains(Rows(Prop(unused, "unused")),
            u => Prop(u, "path").GetString()!.EndsWith("Smoke.blp", StringComparison.OrdinalIgnoreCase));

        var duplicates = await Call("asset_duplicates", Args(("map", map)));
        Assert.Empty(Prop(duplicates, "groups").EnumerateArray());

        var audit = await Call("asset_audit", Args(("map", map)));
        Assert.Equal(1, Prop(audit, "scanned").GetInt32());
        // Four bytes of a BLP header cannot decode, so it is reported as a diagnostic, not a finding.
        Assert.Empty(Prop(audit, "findings").EnumerateArray());
        Assert.NotEmpty(Prop(audit, "diagnostics").EnumerateArray());

        var sizes = await Call("asset_size_report", Args(("map", map)));
        var folder = Assert.Single(Prop(sizes, "folders").EnumerateArray());
        Assert.Contains("war3mapImported", Prop(folder, "folder").GetString());
        Assert.Equal(4, Prop(folder, "bytes").GetInt64());
    }

    [Fact, Covers("asset_replace")]
    public async Task Asset_replace_rewrites_a_script_reference_into_a_copy()
    {
        var asset = NewOut("old", ".blp");
        File.WriteAllBytes(asset, new byte[] { 0x42, 0x4C, 0x50, 0x31 });
        var withImport = await Write("file_set",
            Args(("internal_path", @"war3mapImported\Old.blp"), ("from", asset)));

        var script = NewOut("script", ".j");
        File.WriteAllText(script, "function main takes nothing returns nothing\n"
            + "  call AddSpecialEffect(\"war3mapImported\\\\Old.blp\", 0, 0)\n"
            + "endfunction\n");
        var withScript = await Write("file_set",
            Args(("internal_path", "war3map.j"), ("from", script)), withImport);

        var outPath = NewOut("replaced");
        var before = Sha(withScript);
        var result = await Call("asset_replace", Args(
            ("map", withScript), ("out_path", outPath),
            ("old_path", @"war3mapImported\Old.blp"), ("new_path", @"war3mapImported\New.blp")));

        Assert.Equal(before, Sha(withScript));
        Assert.True(Prop(result, "ok").GetBoolean());
        Assert.Equal(1, Prop(result, "scriptLiteralsChanged").GetInt32());
        Assert.True(File.Exists(outPath));
        await Call("map_info", Args(("map", outPath)));
    }
}
