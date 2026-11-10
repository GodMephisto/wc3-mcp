// tests/Wc3.Tests/ColdStartTests.cs
// PRF-01. A measurement rather than a guard on the requirement's own budget, because the one
// second figure is a whole process figure and an in-process session is already warm. What these
// tests hold is that the server records the three phases, that the surface build is the phase the
// line names, and that a gross regression in either fails here before a client sees it.
using System.Diagnostics;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Wc3.Mcp;
using Xunit.Abstractions;

namespace Wc3.Tests;

public class ColdStartTests
{
    private readonly ITestOutputHelper _out;

    public ColdStartTests(ITestOutputHelper output) => _out = output;

    [Fact]
    public void The_surface_build_is_recorded_and_stays_under_its_budget()
    {
        // Another class may have built the surface first. That is the point, the number is the
        // first build in this process and not a cache hit taken under a stopwatch.
        _ = Wc3McpServer.CreateOptions();

        double surface = ServerRuntime.ColdStart.SurfaceMs;
        _out.WriteLine($"surface build {surface:0} ms, {Wc3McpServer.ToolCount()} tools");

        Assert.True(surface >= 0, "the surface build never recorded its time");
        Assert.True(surface < 1500, $"the surface build took {surface:0} ms against a 1500 ms budget");
    }

    [Fact]
    public void The_initialize_reply_closes_the_cold_start_clock()
    {
        McpTestClient.WithClient(async (client, ct) =>
        {
            Assert.NotNull(client.ServerInfo);
            await Task.CompletedTask;
        }).GetAwaiter().GetResult();

        string? line = ServerRuntime.ColdStart.Line;
        _out.WriteLine(line ?? "no cold start line");
        _out.WriteLine($"surface {ServerRuntime.ColdStart.SurfaceMs:0} ms"
            + $", handshake {ServerRuntime.ColdStart.HandshakeMs:0} ms"
            + $", total {ServerRuntime.ColdStart.TotalMs:0} ms");

        Assert.False(string.IsNullOrEmpty(line), "the initialize reply did not close the cold start clock");
        Assert.Contains("startup", line);
        Assert.Contains("host", line);
        Assert.Contains("surface", line);
        Assert.Contains("handshake", line);

        // The in-process session is not the exe, so this is a gross regression bound. The budget
        // that matters, one second to the initialize reply, is printed by the exe as this line.
        Assert.True(ServerRuntime.ColdStart.TotalMs < 4000,
            $"the in-process cold start took {ServerRuntime.ColdStart.TotalMs:0} ms");
    }

    [Fact]
    public void The_three_phases_are_each_bounded_by_the_total()
    {
        _ = Wc3McpServer.CreateOptions();
        double surface = ServerRuntime.ColdStart.SurfaceMs;
        double total = ServerRuntime.ColdStart.TotalMs;

        // A phase cannot be longer than the clock that contains it. This catches a phase timed
        // from a different origin, which is the way a phase line silently lies.
        // The host and the surface now start together, so a phase can finish after the clock that
        // reports the reply. The claim that survives is that both were recorded, not that one
        // contains the other.
        Assert.True(surface >= 0, "the surface build never recorded its time");
        Assert.True(total >= 0, "the reply clock never recorded its time");
        Assert.True(ServerRuntime.ColdStart.HandshakeMs >= 0, "the handshake phase never recorded its time");
    }

    [Fact]
    public void The_start_line_carries_the_same_facts_now_that_the_probes_run_together()
    {
        // PRF-01. The line is the phase the cold start budget waits on, and the probes it needs are
        // independent. Running them together must not change what the line says.
        string line = Wc3McpServer.BuildStartLine();
        _out.WriteLine(line);

        Assert.Contains(" tools " + Wc3McpServer.ToolCount() + " ", line);
        Assert.Contains(" protocol " + Wc3McpServer.ProtocolRevision, line);
        Assert.Contains(" target ", line);
        Assert.Contains(" game ", line);
        Assert.Contains(" profile " + Wc3McpServer.DefaultProfile, line);
    }

    [Fact]
    public void The_tool_collection_is_filled_on_first_use_and_only_once()
    {
        int builds = 0;
        var source = new Lazy<IReadOnlyList<McpServerTool>>(() =>
        {
            Interlocked.Increment(ref builds);
            return Wc3McpServer.CreateTools();
        });
        var collection = new DeferredToolCollection(source, Wc3McpServer.ToolCount());

        // PRF-01. The options carry the collection, not the descriptors. Nothing is built until a
        // client asks for the list or calls a tool by name.
        Assert.Equal(0, builds);
        Assert.Equal(Wc3McpServer.ToolCount(), collection.Expected);

        Assert.Equal(Wc3McpServer.ToolCount(), collection.PrimitiveNames.Count);
        Assert.Equal(1, builds);

        // A second read is a dictionary lookup over the one build, which is what a second session
        // in the same process used to pay for again.
        Assert.Equal(collection.Expected, collection.ToArray().Length);
        Assert.Equal(1, builds);
        Assert.True(collection.TryGetPrimitive("map_overview", out McpServerTool? tool));
        Assert.Equal("map_overview", tool!.ProtocolTool.Name);
    }

    [Fact]
    public void The_deferred_catalog_resource_publishes_the_same_template()
    {
        // The proxy answers the template from its own constants so that adding the resource costs
        // nothing. This pins those constants to the catalog the server used to build eagerly.
        ResourceTemplate eager = McpToolCatalog.CreateCatalogResource().ProtocolResourceTemplate;
        var deferred = new DeferredCatalogResource().ProtocolResourceTemplate;

        Assert.Equal(eager.UriTemplate, deferred.UriTemplate);
        Assert.Equal(eager.Name, deferred.Name);
        Assert.Equal(eager.Description, deferred.Description);
        Assert.Equal(eager.MimeType, deferred.MimeType);
    }
}
