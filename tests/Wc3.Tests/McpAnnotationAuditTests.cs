// tests/Wc3.Tests/McpAnnotationAuditTests.cs
// Hermetic tests for the annotation audit itself. The audit walks the tool surface by name and
// annotation, so these feed it synthetic tools to prove the two rules fire, and then check the
// real server is clean.
using Wc3.Mcp;

namespace Wc3.Tests;

public class McpAnnotationAuditTests
{
    private static McpToolInfo Tool(
        string name, bool? readOnly, bool? destructive = null, bool? idempotent = null, bool? openWorld = null) =>
        new(name, "test tool", readOnly, destructive, idempotent, openWorld);

    [Fact]
    public void A_mutating_name_marked_read_only_is_reported()
    {
        var audit = McpToolAudit.Inspect(new[]
        {
            Tool("object_set", readOnly: true, destructive: false, idempotent: true, openWorld: false),
        });
        Assert.Equal(1, audit.Count);
        Assert.Contains(audit.Problems, p => p.Tool == "object_set");
    }

    [Theory]
    [InlineData("place_unit")]
    [InlineData("port_unit")]
    [InlineData("new_map")]
    [InlineData("hero_install")]
    [InlineData("file_set")]
    [InlineData("strings_set")]
    public void Every_example_mutating_prefix_is_checked(string name)
    {
        var audit = McpToolAudit.Inspect(new[]
        {
            Tool(name, readOnly: true, destructive: false, idempotent: true, openWorld: false),
        });
        Assert.Equal(1, audit.Count);
        Assert.Equal(name, audit.Problems[0].Tool);
    }

    [Fact]
    public void A_mutating_name_marked_not_read_only_is_clean()
    {
        var audit = McpToolAudit.Inspect(new[]
        {
            Tool("place_unit", readOnly: false, destructive: false, idempotent: false, openWorld: false),
        });
        Assert.Equal(0, audit.Count);
    }

    [Fact]
    public void A_tool_with_no_annotations_at_all_is_reported()
    {
        var audit = McpToolAudit.Inspect(new[] { Tool("mystery", readOnly: null) });
        Assert.Equal(1, audit.Count);
        Assert.Contains(audit.Problems, p => p.Tool == "mystery");
    }

    [Fact]
    public void A_fully_annotated_read_only_tool_is_clean()
    {
        var audit = McpToolAudit.Inspect(new[]
        {
            Tool("map_info", readOnly: true, destructive: false, idempotent: true, openWorld: false),
        });
        Assert.Equal(0, audit.Count);
    }

    [Fact]
    public void The_real_server_surface_is_clean()
    {
        // Every tool the server builds must agree with its name and declare an annotation.
        var audit = McpToolAudit.InspectServer();
        Assert.True(audit.Count == 0,
            "annotation problems: " + string.Join(", ", audit.Problems.Select(p => p.Tool + " " + p.Problem)));
    }
}
