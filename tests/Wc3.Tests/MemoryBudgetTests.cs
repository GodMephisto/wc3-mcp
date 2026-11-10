// tests/Wc3.Tests/MemoryBudgetTests.cs
// PRF-09. The budget is stated in ServerRuntime.MemoryBudget with the measurement it came from,
// and a call that would exceed it is refused rather than attempted. The end to end case uses a
// sparse file, so the archive is 300 MB by length without 300 MB of disk or of reading.
using ModelContextProtocol.Protocol;
using Wc3.Mcp;

namespace Wc3.Tests;

public sealed class MemoryBudgetTests : IDisposable
{
    private readonly string _dir = Path.Combine(
        Path.GetTempPath(), "wc3ctl-memory-budget", Guid.NewGuid().ToString("N"));

    public MemoryBudgetTests() => Directory.CreateDirectory(_dir);

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch (IOException) { }
    }

    [Fact]
    public void The_projection_admits_the_measured_map_and_refuses_the_largest_one_on_disk()
    {
        // Measured on the reference machine. A 38 MB archive peaked at 201 MB of working set.
        Assert.False(ServerRuntime.MemoryBudget.WouldExceed(38L * 1024 * 1024));

        // The largest map in the reference corpus is 495 MB and did not answer in seven minutes.
        Assert.True(ServerRuntime.MemoryBudget.WouldExceed(495L * 1024 * 1024));

        // The reference map itself is 767 KB and is never near the line.
        Assert.False(ServerRuntime.MemoryBudget.WouldExceed(767L * 1024));

        Assert.Equal(92_274_688, ServerRuntime.MemoryBudget.LargestSupportedInputBytes);
    }

    [Fact]
    public void An_input_past_the_budget_is_refused_before_it_is_read()
    {
        string map = Path.Combine(_dir, "oversized.w3x");
        using (var file = new FileStream(map, FileMode.CreateNew, FileAccess.Write))
            file.SetLength(300L * 1024 * 1024);

        CallToolResult? result = null;
        McpTestClient.WithClient(async (client, ct) =>
        {
            result = await client.CallToolAsync(
                "map_info",
                new Dictionary<string, object?> { ["map"] = map },
                cancellationToken: ct);
        }).GetAwaiter().GetResult();

        Assert.NotNull(result);
        Assert.True(result!.IsError == true, "a 300 MB input was attempted");
        string text = result.Content?.OfType<TextContentBlock>().FirstOrDefault()?.Text ?? "";
        Assert.Contains("PRF09_MEMORY_BUDGET", text);
        Assert.Contains(ServerRuntime.MemoryBudget.BudgetBytes.ToString(), text);
        Assert.Contains("Windows 10 x64", text);
    }
}
