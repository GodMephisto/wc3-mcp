// tests/Wc3.Tests/McpToolSmokeTests.Release.cs
// Release family. version_info reports what a build is, changelog turns git history into release
// notes grouped by Conventional Commit type, and mcp_export_verify checks a wc3-mcp checkout stays
// in step with this repository. All three are read-only.
using System.Text.Json;

namespace Wc3.Tests;

public sealed class McpSmokeReleaseTests : McpSmokeBase
{
    public McpSmokeReleaseTests(SmokeFixture f) : base(f) { }

    [Fact, Covers("version_info")]
    public async Task Version_info_reports_the_build_and_mcp_identity()
    {
        var r = await Call("version_info", Args());
        Assert.False(string.IsNullOrWhiteSpace(Prop(r, "productVersion").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(Prop(r, "gitHash").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(Prop(r, "buildTimeUtc").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(Prop(r, "mcpServerName").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(Prop(r, "mcpVersion").GetString()));
    }

    [Fact, Covers("changelog")]
    public async Task Changelog_groups_a_temp_repo_by_conventional_type()
    {
        using var repo = new TempGitRepo();
        repo.Write("a.txt", "1");
        repo.Commit("feat: add alpha");
        repo.Write("b.txt", "2");
        repo.Commit("fix: correct beta");

        var r = await Call("changelog", Args(("repo_dir", repo.Dir)));
        Assert.True(Prop(r, "ok").GetBoolean());
        var groups = Prop(r, "groups").EnumerateArray()
            .Select(g => Prop(g, "type").GetString()).ToList();
        Assert.Equal(new[] { "feat", "fix" }, groups);
        Assert.Contains("## feat", Prop(r, "markdown").GetString());
    }

    [Fact, Covers("mcp_export_verify")]
    public async Task Mcp_export_verify_is_in_step_for_identical_trees()
    {
        var source = Path.Combine(_f.Dir, "export-source");
        var target = Path.Combine(_f.Dir, "export-target");
        // One shared file is enough. Both trees hold it byte for byte, so the export is in step.
        Directory.CreateDirectory(Path.Combine(source, "src", "Wc3.Commands"));
        Directory.CreateDirectory(Path.Combine(target, "src", "Wc3.Commands"));
        File.WriteAllText(Path.Combine(source, "src", "Wc3.Commands", "A.cs"), "namespace A;\n");
        File.WriteAllText(Path.Combine(target, "src", "Wc3.Commands", "A.cs"), "namespace A;\n");

        var r = await Call("mcp_export_verify", Args(("target_dir", target), ("source_dir", source)));
        Assert.True(Prop(r, "inStep").GetBoolean());
        Assert.Equal(1, Prop(r, "checkedFiles").GetInt32());
        Assert.Equal(JsonValueKind.Array, Prop(r, "differences").ValueKind);
    }
}
