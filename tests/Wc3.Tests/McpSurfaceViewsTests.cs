// tests/Wc3.Tests/McpSurfaceViewsTests.cs
// DSC-04. The surface is published four ways, as the catalog JSON, as the live tools list, as the
// family inventory and as the annotation audit. This test reads all four in one session and fails
// on a single name, because a client that trusts one view must not be offered a different surface
// from the one the other views describe.
using System.Text.Json;
using ModelContextProtocol.Protocol;
using Wc3.Mcp;

namespace Wc3.Tests;

public class McpSurfaceViewsTests
{
    [Fact]
    public async Task The_four_surface_views_agree()
    {
        // The catalog JSON, serialized from the same objects the catalog resource is built from,
        // so this view is the one a client pulls in a single read.
        string catalogJson = JsonSerializer.Serialize(
            new McpCatalog(McpToolCatalog.Describe(), McpToolCatalog.DescribePrompts()),
            Wc3McpServer.JsonOptions);
        var catalog = CatalogNames(catalogJson);
        var familyNames = McpToolFamilies.All.Keys.ToHashSet(StringComparer.Ordinal);

        // The live tools list and the audit verdict, both over the real in-memory session.
        var live = new HashSet<string>(StringComparer.Ordinal);
        var problems = new List<string>();
        int auditCount = -1;

        await McpTestClient.WithClient(async (client, ct) =>
        {
            foreach (var tool in await client.ListToolsAsync(cancellationToken: ct))
                live.Add(tool.Name);

            var audit = await client.CallToolAsync("mcp_audit_annotations",
                new Dictionary<string, object?>(), cancellationToken: ct);
            string text = audit.Content.OfType<TextContentBlock>().FirstOrDefault()?.Text ?? "";
            Assert.True(audit.IsError != true, "the audit tool failed. " + text);

            using var doc = JsonDocument.Parse(text);
            var root = doc.RootElement;
            auditCount = root.GetProperty("count").GetInt32();
            foreach (var p in root.GetProperty("problems").EnumerateArray())
                problems.Add(p.GetProperty("tool").GetString() + " " + p.GetProperty("problem").GetString());
        });

        AssertSame("the catalog JSON", catalog, "the live tools list", live);
        AssertSame("the catalog JSON", catalog, "the family inventory", familyNames);

        Assert.True(auditCount == 0,
            "the annotation audit reports " + auditCount + " problem(s). " + string.Join(", ", problems));
    }

    /// <summary>The tool names in the catalog JSON.</summary>
    private static HashSet<string> CatalogNames(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var names = doc.RootElement.GetProperty("tools").EnumerateArray()
            .Select(t => t.GetProperty("name").GetString()!)
            .ToHashSet(StringComparer.Ordinal);
        Assert.NotEmpty(names);
        return names;
    }

    /// <summary>Fails with the symmetric difference when two views name different tools.</summary>
    private static void AssertSame(string firstName, HashSet<string> first, string secondName, HashSet<string> second)
    {
        var onlyFirst = first.Except(second).OrderBy(n => n, StringComparer.Ordinal).ToList();
        var onlySecond = second.Except(first).OrderBy(n => n, StringComparer.Ordinal).ToList();

        Assert.True(onlyFirst.Count == 0 && onlySecond.Count == 0,
            firstName + " and " + secondName + " disagree. Only in " + firstName + ", "
            + string.Join(", ", onlyFirst) + ". Only in " + secondName + ", "
            + string.Join(", ", onlySecond) + ".");
        Assert.Equal(first.Count, second.Count);
    }
}
