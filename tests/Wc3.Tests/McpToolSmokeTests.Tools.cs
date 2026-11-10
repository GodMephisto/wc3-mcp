// tests/Wc3.Tests/McpToolSmokeTests.Tools.cs
// Surface-inspection family. mcp_audit_annotations reports annotation drift on the tool
// surface itself, so an agent can check the tools it was handed are labelled honestly.
using System.Text.Json;

namespace Wc3.Tests;

public sealed class McpSmokeToolsTests : McpSmokeBase
{
    public McpSmokeToolsTests(SmokeFixture f) : base(f) { }

    [Fact, Covers("mcp_audit_annotations")]
    public async Task Mcp_audit_annotations_reports_the_surface_clean()
    {
        var r = await Call("mcp_audit_annotations", Args());
        Assert.Equal(0, Prop(r, "count").GetInt32());
        var problems = Prop(r, "problems");
        Assert.Equal(JsonValueKind.Array, problems.ValueKind);
        Assert.Empty(problems.EnumerateArray());
    }

    [Fact, Covers("quickstart")]
    public async Task Quickstart_lists_the_core_workflows_and_narrows_by_goal()
    {
        var page = await Call("quickstart", Args());
        var all = Rows(Prop(page, "items"));
        Assert.Equal(12, all.Count);
        Assert.Equal(12, Prop(page, "totalCount").GetInt32());
        Assert.Equal(12, Prop(page, "returnedCount").GetInt32());
        Assert.False(Prop(page, "hasMore").GetBoolean());
        Assert.All(all, w => Assert.False(string.IsNullOrWhiteSpace(Prop(w, "id").GetString())));
        Assert.All(all, w => Assert.False(string.IsNullOrWhiteSpace(Prop(w, "title").GetString())));
        Assert.All(all, w => Assert.False(string.IsNullOrWhiteSpace(Prop(w, "prompt").GetString())));

        var narrowed = Rows(Prop(await Call("quickstart", Args(("goal", "portrait"))), "items"));
        Assert.NotEmpty(narrowed);
        Assert.Contains(narrowed, w => Prop(w, "id").GetString()!.Contains("portrait", StringComparison.OrdinalIgnoreCase));
    }
}
