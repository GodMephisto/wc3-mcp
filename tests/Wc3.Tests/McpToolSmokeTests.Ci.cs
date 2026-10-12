// tests/Wc3.Tests/McpToolSmokeTests.Ci.cs
// The ci gate MCP tools, exercised through the protocol against the fixture blank map and
// temporary schema folders, so the coverage check has a smoke test for each and no test needs
// data/schemas or a real map on disk.
using System.Text;

namespace Wc3.Tests;

public sealed class McpSmokeCiTests : McpSmokeBase
{
    public McpSmokeCiTests(SmokeFixture f) : base(f) { }

    /// <summary>Two schemas whose second declares a native the first does not, so a map using it
    /// has a compat finding against the first.</summary>
    private string SchemaRootWithExtraNative()
    {
        var root = Path.Combine(_f.Dir, "ci-schemas-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        GamedataFixtures.Save(root, "CI-v1.30.1.10211", natives: new[] { "CiSmokeNative" });
        GamedataFixtures.Save(root, "CI-v1.31.0.10420", natives: new[] { "CiSmokeNative", "CiExtraNative" });
        return root;
    }

    /// <summary>A synthetic map whose script calls CiExtraNative, which CI-v1.30.1.10211 lacks.</summary>
    private string MapCallingExtraNative()
    {
        var path = Path.Combine(_f.Dir, "ci-caller-" + Guid.NewGuid().ToString("N") + ".w3x");
        File.WriteAllBytes(path, SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = Encoding.UTF8.GetBytes(
                "function main takes nothing returns nothing\n    call CiExtraNative()\nendfunction\n"),
        }));
        return path;
    }

    [Fact, Covers("ci_lint")]
    public async Task Ci_lint_gate_answers_on_the_fixture()
    {
        var r = await Call("ci_lint", Args(("map", Fixture)));

        Assert.True(Prop(r, "passed").GetBoolean());
        Assert.Equal(0, Prop(r, "exitCode").GetInt32());
        Assert.Single(Rows(Prop(r, "gates")));
    }

    [Fact, Covers("ci_validate")]
    public async Task Ci_validate_gate_answers_on_the_fixture()
    {
        var r = await Call("ci_validate", Args(("map", Fixture)));

        Assert.True(Prop(r, "passed").GetBoolean());
        Assert.Equal(0, Prop(r, "exitCode").GetInt32());
        Assert.Equal("validate", Prop(Assert.Single(Rows(Prop(r, "gates"))), "name").GetString());
    }

    [Fact, Covers("ci_compat")]
    public async Task Ci_compat_gate_fails_on_a_missing_native()
    {
        var root = SchemaRootWithExtraNative();
        var map = MapCallingExtraNative();

        var r = await Call("ci_compat", Args(
            ("map", map), ("target", "CI-v1.30.1.10211"), ("schemas_dir", root)));

        Assert.False(Prop(r, "passed").GetBoolean());
        Assert.Equal(1, Prop(r, "exitCode").GetInt32());
    }

    [Fact, Covers("ci_schema")]
    public async Task Ci_schema_gate_checks_the_dataset()
    {
        var root = SchemaRootWithExtraNative();

        var r = await Call("ci_schema", Args(("schemas_dir", root), ("min_real", 0)));

        Assert.True(Prop(r, "passed").GetBoolean());
        Assert.Equal(0, Prop(r, "exitCode").GetInt32());
        Assert.Equal(2, Prop(r, "builds").EnumerateArray().Count());
    }

    [Fact, Covers("ci_all")]
    public async Task Ci_all_gate_summarizes_every_check()
    {
        var r = await Call("ci_all", Args(("map", Fixture)));

        Assert.True(Prop(r, "passed").GetBoolean());
        Assert.Equal(new[] { "lint", "validate", "compat" },
            Rows(Prop(r, "gates")).Select(g => Prop(g, "name").GetString()).ToArray());
    }
}
