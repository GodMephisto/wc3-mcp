// tests/Wc3.Tests/McpToolSmokeTests.Strings.cs
// Strings family. Covers strings_missing, strings_unused, strings_diff, strings_find, strings_set.
using System.Text;

namespace Wc3.Tests;

public sealed class McpSmokeStringsTests : McpSmokeBase
{
    public McpSmokeStringsTests(SmokeFixture f) : base(f) { }

    private const string Wts =
        "STRING 1\r\n{\r\nused by script\r\n}\r\n\r\n" +
        "STRING 2\r\n{\r\nunused entry\r\n}\r\n\r\n" +
        "STRING 3\r\n{\r\nfind me needle\r\n}\r\n";

    private const string Script =
        "function main takes nothing returns nothing\n" +
        "    call DisplayTextToForce(bj_FORCE_ALL_PLAYERS, \"TRIGSTR_1\")\n" +
        "    call DisplayTextToForce(bj_FORCE_ALL_PLAYERS, \"TRIGSTR_999\")\n" +
        "endfunction\n";

    private string WriteMap(string label, string wts, string? script = null)
    {
        var files = new Dictionary<string, byte[]> { ["war3map.wts"] = Encoding.UTF8.GetBytes(wts) };
        if (script is not null) files["war3map.j"] = Encoding.UTF8.GetBytes(script);
        var path = Path.Combine(_f.Dir, $"{label}-{Guid.NewGuid():N}.w3x");
        File.WriteAllBytes(path, SyntheticMap.Build(files));
        return path;
    }

    [Fact, Covers("strings_missing")]
    public async Task Strings_missing_reports_a_script_reference_with_no_entry()
    {
        var map = WriteMap("missing", Wts, Script);
        var r = await Call("strings_missing", Args(("map", map)));
        Assert.Contains(999, Rows(Prop(r, "missingIds")).Select(e => e.GetInt32()));
    }

    [Fact, Covers("strings_unused")]
    public async Task Strings_unused_reports_an_entry_nothing_references()
    {
        var map = WriteMap("unused", Wts, Script);
        var r = await Call("strings_unused", Args(("map", map)));
        var ids = Rows(Prop(r, "unused")).Select(e => Prop(e, "id").GetInt32()).ToList();
        Assert.Contains(2, ids);
        Assert.Contains(3, ids);
    }

    [Fact, Covers("strings_diff")]
    public async Task Strings_diff_reports_an_added_entry()
    {
        var a = WriteMap("diff-a", Wts);
        var b = WriteMap("diff-b", Wts + "STRING 4\r\n{\r\nadded\r\n}\r\n");
        var r = await Call("strings_diff", Args(("map_a", a), ("map_b", b)));
        Assert.Contains(4, Rows(Prop(r, "added")).Select(e => Prop(e, "id").GetInt32()));
    }

    [Fact, Covers("strings_find")]
    public async Task Strings_find_matches_case_insensitively()
    {
        var map = WriteMap("find", Wts);
        var r = await Call("strings_find", Args(("map", map), ("text", "NEEDLE")));
        var hit = Assert.Single(Rows(Prop(r, "matches")));
        Assert.Equal(3, Prop(hit, "id").GetInt32());
    }

    [Fact, Covers("strings_set", "strings_list")]
    public async Task Strings_set_writes_a_copy_and_leaves_the_input_alone()
    {
        var map = WriteMap("set", Wts);
        var outPath = NewOut("strings-set");
        var before = Sha(map);

        var r = await Call("strings_set", Args(("map", map), ("id", 3), ("text", "edited"), ("out_path", outPath)));

        Assert.Equal(before, Sha(map));
        Assert.True(File.Exists(outPath));
        var listed = await Call("strings_list", Args(("map", outPath)));
        var entry = Rows(listed).Single(e => Prop(e, "id").GetInt32() == 3);
        Assert.Equal("edited", Prop(entry, "text").GetString());
    }
}
