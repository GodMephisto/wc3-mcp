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
}
