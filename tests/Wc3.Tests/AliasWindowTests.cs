// tests/Wc3.Tests/AliasWindowTests.cs
// SUR-02 and SUR-03. The alias window is proved at the call path rather than in a tool body, so
// every test here drives a real client at a real server over the shipped filter and the shipped
// dispatch. The table is supplied because this tree has not renamed a tool yet, and the shipped
// table is empty for the same reason. What the tests hold is the mechanism, so the first rename
// lands in the table and nowhere else.
using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using Wc3.Commands;
using Wc3.Mcp;

namespace Wc3.Tests;

public class AliasWindowTests
{
    /// <summary>A rename the tests can open and close, shaped like a real entry.</summary>
    private static IReadOnlyDictionary<string, ToolAlias> OpenWindow() =>
        new Dictionary<string, ToolAlias>(StringComparer.Ordinal)
        {
            ["old_list_files"] = new("old_list_files", "list_files", "9.0.0"),
            ["old_version_info"] = new("old_version_info", "version_info", "9.0.0"),
        };

    private static string WriteMap()
    {
        string path = Path.Combine(Path.GetTempPath(), $"wc3alias-{Guid.NewGuid():N}.w3x");
        File.WriteAllBytes(path, SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = new byte[] { 1, 2, 3 },
            ["mystery.bin"] = new byte[] { 9 },
        }));
        return path;
    }

    [Fact]
    public async Task An_old_name_reaches_the_current_tool_and_the_reply_names_both()
    {
        // version_info takes no argument, so this proves the rewrite reaches a real tool and the
        // reply says which name was called, without depending on a fixture the tool might refuse.
        await McpTestClient.WithClient(async (client, ct) =>
        {
            var result = await client.CallToolAsync("old_version_info",
                new Dictionary<string, object?>(), cancellationToken: ct);

            var blocks = result.Content!.OfType<TextContentBlock>().Select(b => b.Text ?? "").ToList();
            Assert.True(result.IsError != true, string.Join(" | ", blocks));
            Assert.True(blocks.Count >= 2, "the tool output and the alias note are two blocks");

            string all = string.Join("\n", blocks);
            Assert.Contains("old_version_info", all);
            Assert.Contains("version_info", all);
            Assert.Contains("9.0.0", all);
        }, OpenWindow());
    }

    [Fact]
    public async Task An_old_name_is_absent_from_tools_list_and_the_current_name_appears_once()
    {
        await McpTestClient.WithClient(async (client, ct) =>
        {
            var tools = await client.ListToolsAsync(cancellationToken: ct);
            var names = tools.Select(t => t.Name).ToList();

            Assert.DoesNotContain("old_list_files", names);
            Assert.DoesNotContain("old_version_info", names);
            Assert.Equal(1, names.Count(n => n == "list_files"));
            Assert.Equal(1, names.Count(n => n == "version_info"));

            // One row per tool, exactly as the table without a rename gives.
            Assert.Equal(Wc3McpServer.ToolCount(), names.Count);
        }, OpenWindow());
    }

    [Fact]
    public async Task An_expired_alias_is_refused_with_a_code_naming_the_removal_version()
    {
        // 0.0.1 is behind every build, so the window this entry states has closed.
        var closed = new Dictionary<string, ToolAlias>(StringComparer.Ordinal)
        {
            ["ancient_name"] = new("ancient_name", "version_info", "0.0.1"),
        };

        await McpTestClient.WithClient(async (client, ct) =>
        {
            var result = await client.CallToolAsync(
                "ancient_name", new Dictionary<string, object?>(), cancellationToken: ct);

            Assert.True(result.IsError);
            string text = Assert.IsType<TextContentBlock>(Assert.Single(result.Content!)).Text ?? "";
            Assert.StartsWith("[ALIAS_EXPIRED]", text, StringComparison.Ordinal);
            Assert.Contains("0.0.1", text);
            Assert.Contains("version_info", text);

            // The tool never ran, so its own reply is not in the refusal.
            Assert.DoesNotContain("productVersion", text);
        }, closed);
    }

    [Fact]
    public async Task An_unknown_name_still_fails_at_the_protocol_layer()
    {
        // The window changes nothing for a name nobody ever served, so the refusal is the one the
        // SDK gives and not a tool error this server invents.
        await McpTestClient.WithClient(async (client, ct) =>
        {
            McpException error = await Assert.ThrowsAnyAsync<McpException>(() =>
                client.CallToolAsync("definitely_not_a_tool",
                    new Dictionary<string, object?>(), cancellationToken: ct).AsTask());

            Assert.Contains("Unknown tool", error.Message);
            Assert.Contains("definitely_not_a_tool", error.Message);
        });
    }

    [Fact]
    public void The_shipped_window_names_only_live_tools()
    {
        var live = Wc3McpServer.CreateTools().Select(t => t.ProtocolTool.Name).ToList();

        // Empty today, and the same call is the gate on the day the first entry appears.
        Assert.Empty(McpToolCatalog.InspectRenames(McpToolCatalog.Renames, live));
        foreach (ToolAlias alias in McpToolCatalog.Renames.Values)
        {
            Assert.Contains(alias.CurrentName, live);
            Assert.DoesNotContain(alias.OldName, live);
        }
    }

    [Fact]
    public void The_catalog_lists_the_old_name_on_the_current_tool_only()
    {
        var listed = McpToolCatalog.Describe(OpenWindow());

        Assert.DoesNotContain(listed, i => i.Name == "old_list_files");
        McpToolInfo current = Assert.Single(listed.Where(i => i.Name == "list_files"));
        Assert.Contains("old_list_files", current.Aliases);
        Assert.Equal(Wc3McpServer.ToolCount(), listed.Count);

        // The shipped table adds no alias to any entry, which is the catalog the specification
        // measured.
        Assert.All(McpToolCatalog.Describe(), i => Assert.Empty(i.Aliases));
    }

    [Theory]
    [InlineData("list_files", "list_files", "0.5.0")]
    [InlineData("old_name", "list_files", "next release")]
    [InlineData("old_name", "", "0.5.0")]
    public void A_broken_window_fails_before_the_server_starts(string oldName, string currentName, string removal)
    {
        var table = new Dictionary<string, ToolAlias>(StringComparer.Ordinal)
        {
            [oldName] = new(oldName, currentName, removal),
        };

        var error = Assert.Throws<InvalidOperationException>(() => { Wc3McpServer.CreateOptions(table); });
        Assert.Contains("alias window", error.Message);
    }

    [Theory]
    [InlineData("0.5.0", "0.4.13", false)]
    [InlineData("0.4.14", "0.4.13", false)]
    [InlineData("0.4.13", "0.4.13", true)]
    [InlineData("0.4.12", "0.4.13", true)]
    [InlineData("1.0.0", "0.13.0", false)]
    [InlineData("0.0.1", "0.4.13", true)]
    [InlineData("not a version", "0.4.13", true)]
    public void The_window_closes_on_the_removal_version(string removal, string current, bool closed) =>
        Assert.Equal(closed, McpToolCatalog.RemovalReached(removal, current));

    /// <summary>
    /// CMP-03. The alias window is not the whole of a rename. The release note that closes the
    /// window has to name the old name, or a client that reads the notes has no way to learn that
    /// the name it still sends has been retired. Every shipped entry is checked against the
    /// changelog file for the version that removes it, and the checker itself is proved on a
    /// window whose removal version has no note, so an empty table cannot make this pass by
    /// accident.
    /// </summary>
    [Fact]
    public void A_removed_name_is_listed_in_the_release_note_that_removes_it()
    {
        if (RepoLayout.FindRoot() is not { } root) return;   // the wc3-mcp mirror has no changelog

        var live = Wc3McpServer.CreateTools().Select(t => t.ProtocolTool.Name).ToList();
        Assert.Empty(McpToolCatalog.InspectRenames(McpToolCatalog.Renames, live));

        Assert.Empty(UnlistedRenames(root, McpToolCatalog.Renames));

        // The checker bites. 9.0.0 has no changelog file in this tree, so the window the alias
        // tests open is reported rather than silently accepted.
        Assert.NotEmpty(UnlistedRenames(root, OpenWindow()));
    }

    /// <summary>
    /// CMP-03. One problem per window entry whose release note does not name the removed name, in
    /// the order the table is read. The note is the file <c>changelog/&lt;removal&gt;.md</c>, the
    /// same folder the changelog guard reads, so a version that removes a name has to be a version
    /// this tree publishes notes for.
    /// </summary>
    private static IReadOnlyList<string> UnlistedRenames(
        string root, IReadOnlyDictionary<string, ToolAlias> table)
    {
        var problems = new List<string>();
        foreach (ToolAlias alias in table.Values.OrderBy(a => a.OldName, StringComparer.Ordinal))
        {
            string note = Path.Combine(root, "changelog", alias.RemovalVersion + ".md");
            if (!File.Exists(note))
            {
                problems.Add(alias.OldName + " is removed in " + alias.RemovalVersion
                    + " and there is no changelog/" + alias.RemovalVersion + ".md");
                continue;
            }
            if (!File.ReadAllText(note).Contains(alias.OldName, StringComparison.Ordinal))
                problems.Add(alias.OldName + " is removed in " + alias.RemovalVersion
                    + " and that release note never names it");
        }
        return problems;
    }
}
