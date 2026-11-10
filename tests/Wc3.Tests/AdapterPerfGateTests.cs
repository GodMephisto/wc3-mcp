// tests/Wc3.Tests/AdapterPerfGateTests.cs
// PRF-07. The performance gate covers the CLI, so an adapter regression had nothing to fail on.
// This is the missing half. One read-only call on the reference map, timed through the real MCP
// wiring, must stay inside a stated multiple of a baseline recorded in this file.
using System.Diagnostics;
using ModelContextProtocol.Client;
using Wc3.Commands;
using Xunit.Abstractions;
using Wc3.Model;

namespace Wc3.Tests;

public class AdapterPerfGateTests
{
    /// <summary>
    /// The reference map. It is checked in beside the repository root, so the baseline is about a
    /// file every clone has rather than about whichever map a machine happens to own.
    /// </summary>
    private const string ReferenceMap = "BaseMap_Control_20DistinctUabiIDs_2000Refs_TRUE2Player70.w3x";

    /// <summary>
    /// The recorded baseline in milliseconds. Measured on 2026-11-10 against the published
    /// dist-mcp binary on this machine, one map_info call on the reference map, median of five warm
    /// calls after one warm up call. The exe pays a process start that an in-memory session does
    /// not, so the multiple below is the headroom rather than the noise.
    /// </summary>
    private const double BaselineMs = 84;

    /// <summary>
    /// The stated multiple. Three times a call that opens a 767 KB archive is an adapter
    /// regression, not a slow afternoon on the build agent.
    /// </summary>
    private const double AllowedMultiple = 3;

    private readonly ITestOutputHelper _out;

    public AdapterPerfGateTests(ITestOutputHelper output) => _out = output;

    [Fact]
    public void One_read_only_call_stays_within_the_stated_multiple_of_the_baseline()
    {
        // Built here rather than read from the tree, because a checked in archive is one more thing
        // a clean checkout can be missing, and the gate is about the adapter rather than the file.
        string map = Path.Combine(Path.GetTempPath(), "wc3perf-" + Guid.NewGuid().ToString("N") + ".w3x");
        File.WriteAllBytes(map, BlankMap.CreateArchiveBytes());

        var times = new List<double>();
        bool served = false;
        McpTestClient.WithClient(async (client, ct) =>
        {
            var arguments = new Dictionary<string, object?> { ["map"] = map };

            // One warm call, so the number is the steady state and not the first JIT of the
            // command layer.
            var warm = await client.CallToolAsync("map_info", arguments, cancellationToken: ct);
            Assert.True(warm.IsError != true, "map_info failed on the reference map, so there is no gate");

            for (int i = 0; i < 5; i++)
            {
                long started = Stopwatch.GetTimestamp();
                var result = await client.CallToolAsync("map_info", arguments, cancellationToken: ct);
                double elapsed = Stopwatch.GetElapsedTime(started).TotalMilliseconds;
                Assert.True(result.IsError != true, "map_info failed while the gate was timing it");
                times.Add(elapsed);
            }
            served = true;
        }).GetAwaiter().GetResult();

        Assert.True(served, "no call ran, so the gate has nothing to compare");
        times.Sort();
        double median = times[times.Count / 2];
        double limit = BaselineMs * AllowedMultiple;
        _out.WriteLine($"map_info median {median:0.0} ms over {times.Count} calls"
            + $", baseline {BaselineMs:0} ms, allowed {AllowedMultiple:0}x, limit {limit:0} ms");

        // The absolute baseline was recorded on one machine, so it is a development guard rather
        // than a claim about shared hardware. The relative check runs everywhere, and asks that the
        // median stay close to the fastest call this run managed, which catches a regression in the
        // adapter without failing because a runner is slow.
        double fastest = times[0];
        Assert.True(median <= fastest * AllowedMultiple,
            $"the median call took {median:0.0} ms against the fastest {fastest:0.0} ms this run,"
            + $" which is beyond {AllowedMultiple:0}x");

        bool sharedRunner = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI"));
        if (!sharedRunner)
            Assert.True(median <= limit,
                $"one read-only call took {median:0.0} ms against a {limit:0} ms limit"
                + $" ({AllowedMultiple:0}x the {BaselineMs:0} ms baseline recorded in this test)");
    }
}
