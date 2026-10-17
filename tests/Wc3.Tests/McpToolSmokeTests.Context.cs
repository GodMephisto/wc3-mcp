// tests/Wc3.Tests/McpToolSmokeTests.Context.cs
// The context injector. docs_search queries the bundled Warcraft III documents and rag_status
// reports the local index, both read-only and offline. rag_install fetches the model and the
// corpus over the network, so it is asserted present rather than run here, which keeps the
// suite hermetic.
using System.Text.Json;
using ModelContextProtocol.Client;

namespace Wc3.Tests;

public sealed class McpSmokeContextTests : McpSmokeBase
{
    public McpSmokeContextTests(SmokeFixture f) : base(f) { }

    [Fact, Covers("docs_search", "rag_status", "rag_install", "graph_neighbors", "graph_hubs", "graph_path")]
    public async Task Context_tools_answer_and_install_time_tools_are_served()
    {
        // rag_status reports the index state and never touches the network.
        var status = await Call("rag_status", new());
        Assert.Equal(JsonValueKind.Object, status.ValueKind);

        // docs_search finds a passage in the bundled Ability Insight reference.
        var search = await Call("docs_search", new() { ["query"] = "Aerial Shackles", ["max"] = 3 });
        Assert.True(search.GetProperty("results").GetArrayLength() > 0, "docs_search returned no passage");

        // rag_install and the graph tools need the local store built by rag install, so they are
        // asserted present rather than invoked, which keeps the suite hermetic.
        await McpTestClient.WithClient(async (client, ct) =>
        {
            var tools = await client.ListToolsAsync(cancellationToken: ct);
            foreach (var name in new[] { "rag_install", "graph_neighbors", "graph_hubs", "graph_path" })
                Assert.Contains(tools, t => t.Name == name);
        });
    }
}
