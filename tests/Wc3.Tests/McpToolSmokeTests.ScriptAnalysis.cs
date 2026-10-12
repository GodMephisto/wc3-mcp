// tests/Wc3.Tests/McpToolSmokeTests.ScriptAnalysis.cs
// The five script analysis tools, driven through the MCP protocol like an AI client. Synthetic
// maps with hand-written war3map.j are built in memory, so every tool answers for real with no
// game install. The four read tools answer with the shape they name, and modernize writes a map
// that reads back with the rewrite in it while the input map stays byte for byte unchanged.
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public sealed class McpSmokeScriptAnalysisTests : McpSmokeBase
{
    public McpSmokeScriptAnalysisTests(SmokeFixture f) : base(f) { }

    private string ScriptMap(string jass, string label)
    {
        var path = NewOut(label);
        var doc = BlankMap.Create();
        FileEditCommand.WriteText(doc, "war3map.j", jass);
        doc.Save(path);
        return path;
    }

    private static readonly string ReachScript = string.Join("\n",
        "function main takes nothing returns nothing",
        "    call Helper()",
        "endfunction",
        "function Helper takes nothing returns nothing",
        "endfunction",
        "function Orphan takes nothing returns nothing",
        "endfunction");

    [Fact, Covers("script_reachable")]
    public async Task Script_reachable_reports_the_dead_function()
    {
        var map = ScriptMap(ReachScript, "reach");
        var r = await Call("script_reachable", Args(("map", map)));
        Assert.Equal(2, Prop(r, "reachable").GetInt32());
        Assert.Equal(1, Prop(r, "dead").GetInt32());
        var dead = Rows(Prop(r, "deadFunctions"));
        Assert.Contains(dead, d => Prop(d, "name").GetString() == "Orphan");
    }

    [Fact, Covers("script_api_usage")]
    public async Task Script_api_usage_reports_only_what_the_target_lacks()
    {
        var schemas = Path.Combine(_f.Dir, "schemas-api");
        ScriptTestSchema.Save(Path.Combine(schemas, "old"), "TFT-v1.30.1.10211", new[] { "KnownNative" });
        ScriptTestSchema.Save(Path.Combine(schemas, "new"), "TFT-v1.31.0.10420",
            new[] { "KnownNative", "NewNative" });

        var map = ScriptMap(
            "function InitTriggers takes nothing returns nothing\n"
            + "    call KnownNative()\n"
            + "    call NewNative()\n"
            + "endfunction\n", "api");

        var r = await Call("script_api_usage",
            Args(("map", map), ("target", "TFT-v1.30.1.10211"), ("schemas_dir", schemas)));

        Assert.Equal(1, Prop(r, "missing").GetInt32());
        var usages = Rows(Prop(r, "usages"));
        var one = Assert.Single(usages);
        Assert.Equal("NewNative", Prop(one, "name").GetString());
    }

    [Fact, Covers("script_semantic_diff")]
    public async Task Script_semantic_diff_ignores_a_local_rename()
    {
        var a = ScriptMap(
            "function Foo takes nothing returns nothing\n"
            + "    local integer i = 0\n"
            + "    call BJDebugMsg(\"hi\")\n"
            + "endfunction\n", "diffa");
        var b = ScriptMap(
            "function Foo takes nothing returns nothing\n"
            + "    local integer j = 0\n"
            + "    call BJDebugMsg(\"bye\")\n"
            + "endfunction\n", "diffb");

        var r = await Call("script_semantic_diff", Args(("map", a), ("other", b)));
        Assert.Equal(1, Prop(r, "changes").GetInt32());
    }

    [Fact, Covers("script_duplicate_bodies")]
    public async Task Script_duplicate_bodies_groups_the_copied_pair()
    {
        var map = ScriptMap(
            "function One takes nothing returns nothing\n"
            + "    local integer i = 0\n"
            + "    call BJDebugMsg(\"x\")\n"
            + "endfunction\n"
            + "function Two takes nothing returns nothing\n"
            + "    local integer j = 0\n"
            + "    call BJDebugMsg(\"x\")\n"
            + "endfunction\n"
            + "function Three takes nothing returns nothing\n"
            + "    call BJDebugMsg(\"y\")\n"
            + "endfunction\n", "dup");

        var r = await Call("script_duplicate_bodies", Args(("map", map)));
        Assert.Equal(1, Prop(r, "duplicateGroups").GetInt32());
        Assert.Equal(2, Prop(r, "duplicateFunctions").GetInt32());
    }

    [Fact, Covers("script_modernize")]
    public async Task Script_modernize_rewrites_a_known_native_and_leaves_the_input_alone()
    {
        var schemas = Path.Combine(_f.Dir, "schemas-modernize");
        ScriptTestSchema.Save(Path.Combine(schemas, "new"), "TFT-v1.31.0.10420",
            new[] { "BlzSetUnitBaseDamage" });

        var map = ScriptMap(
            "function main takes nothing returns nothing\n"
            + "    call SetUnitBaseDamage(udg_u, 10, 0)\n"
            + "endfunction\n", "modernize");

        var before = Sha(map);
        var outPath = NewOut("modernized");
        var r = await Call("script_modernize", Args(
            ("map", map), ("out_path", outPath),
            ("target", "TFT-v1.31.0.10420"), ("schemas_dir", schemas)));

        Assert.Equal(1, Prop(r, "rewrites").GetInt32());
        Assert.Equal(before, Sha(map));
        Assert.True(File.Exists(outPath), "script_modernize wrote nothing");

        var text = Prop(await Call("file_get_text",
            Args(("map", outPath), ("internal_path", "war3map.j"))), "text").GetString();
        Assert.Contains("call BlzSetUnitBaseDamage(udg_u, 10, 0)", text);
    }
}
