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
    public void A_tool_that_leaves_one_hint_unset_is_reported()
    {
        var audit = McpToolAudit.Inspect(new[]
        {
            Tool("model_squish", readOnly: false, destructive: null, idempotent: false, openWorld: false),
        });
        Assert.Equal(1, audit.Count);
        Assert.Contains("destructiveHint", audit.Problems[0].Problem);
    }

    [Fact]
    public void A_writing_tool_that_says_read_only_in_its_description_is_reported()
    {
        var tool = new McpToolInfo(
            "terrain_export_heightmap", "Export the heights. Read-only, the map is not modified.",
            false, false, true, false);
        var audit = McpToolAudit.Inspect(new[] { tool });
        Assert.Equal(1, audit.Count);
        Assert.Contains("read-only", audit.Problems[0].Problem);
    }

    [Fact]
    public void A_reading_tool_may_say_read_only()
    {
        var tool = new McpToolInfo(
            "map_info", "Read a map's metadata. Read-only.", true, false, true, false);
        Assert.Equal(0, McpToolAudit.Inspect(new[] { tool }).Count);
    }

    [Fact]
    public void A_tool_that_is_both_read_only_and_destructive_is_reported()
    {
        var audit = McpToolAudit.Inspect(new[]
        {
            Tool("map_info", readOnly: true, destructive: true, idempotent: true, openWorld: false),
        });
        Assert.Equal(1, audit.Count);
        Assert.Contains("both", audit.Problems[0].Problem);
    }

    [Fact]
    public void A_tool_whose_only_effect_is_to_remove_something_must_declare_destructive()
    {
        var audit = McpToolAudit.Inspect(new[]
        {
            Tool("mpq_remove", readOnly: false, destructive: false, idempotent: false, openWorld: false),
        });
        Assert.Equal(1, audit.Count);
        Assert.Contains("looks destructive", audit.Problems[0].Problem);
    }

    [Fact]
    public void A_removal_that_declares_destructive_is_clean()
    {
        var audit = McpToolAudit.Inspect(new[]
        {
            Tool("mpq_remove", readOnly: false, destructive: true, idempotent: false, openWorld: false),
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
    public void Every_tool_belongs_to_exactly_one_family()
    {
        // SUR-06. A tool that is not in the inventory table has no family, and this is the only
        // thing that fails when one is added without one.
        var infos = McpToolCatalog.Describe();
        var missing = infos.Where(i => string.IsNullOrEmpty(i.Family)).Select(i => i.Name).ToList();
        Assert.True(missing.Count == 0, "tools with no family: " + string.Join(", ", missing));
        Assert.Equal(277, infos.Count);
        Assert.Equal(11, infos.Select(i => i.Family).Distinct().Count());
        Assert.Equal(277, McpToolFamilies.All.Count);
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
