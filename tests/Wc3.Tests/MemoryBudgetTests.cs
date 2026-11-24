// tests/Wc3.Tests/MemoryBudgetTests.cs
// PRF-09. The budget is stated in ServerRuntime.MemoryBudget with the measurement it came from,
// and a call that would exceed it is refused rather than attempted. Every size here is worked out
// from the budget the code reports, so the suite says the same thing on a small host and a large
// one. The largest supported input is admitted, one byte past it is refused, and the end to end
// case uses a sparse file sized just past that line so the archive never costs its length on disk.
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
    public void The_line_the_budget_reports_is_admitted_and_one_byte_more_is_refused()
    {
        // No variable in force, so this is the budget the host derives.
        RunWith(null, () =>
        {
            long budget = ServerRuntime.MemoryBudget.BudgetBytes;
            long largest = ServerRuntime.MemoryBudget.LargestSupportedInputBytes;

            // The line is the budget less the fixed surface, at the measured growth rate.
            Assert.Equal(
                (budget - ServerRuntime.MemoryBudget.FixedBytes) / ServerRuntime.MemoryBudget.BytesPerInputByte,
                largest);
            Assert.True(ServerRuntime.MemoryBudget.ProjectedBytes(largest) <= budget);
            Assert.False(ServerRuntime.MemoryBudget.WouldExceed(largest));
            Assert.True(ServerRuntime.MemoryBudget.WouldExceed(largest + 1));

            // The measured 38 MB archive peaked at an eighth of the reference line, so an archive of
            // that class is admitted on any host, and the 767 KB reference map is far below it.
            Assert.False(ServerRuntime.MemoryBudget.WouldExceed(
                largest / ServerRuntime.MemoryBudget.MemoryShare));
            Assert.False(ServerRuntime.MemoryBudget.WouldExceed(largest / 1024));

            // The 331 MB map the budget was raised for sat at about seven tenths of the line.
            Assert.False(ServerRuntime.MemoryBudget.WouldExceed(largest / 10 * 7));
        });
    }

    [Fact]
    public void On_a_host_at_the_floor_the_measured_archive_is_admitted_and_a_larger_one_refused()
    {
        // The floor is what a host that reports 0 bytes of memory gets, so pinning the variable to
        // it exercises the small-host case on any machine.
        RunWith(ServerRuntime.MemoryBudget.FloorBytes.ToString(), () =>
        {
            long largest = ServerRuntime.MemoryBudget.LargestSupportedInputBytes;
            Assert.Equal(ServerRuntime.MemoryBudget.FloorBytes, ServerRuntime.MemoryBudget.BudgetBytes);

            // The 38 MB archive is inside the floor's line, and the 495 MB corpus maximum is
            // several times that line and is refused.
            Assert.False(ServerRuntime.MemoryBudget.WouldExceed(
                largest / ServerRuntime.MemoryBudget.MemoryShare));
            Assert.False(ServerRuntime.MemoryBudget.WouldExceed(largest));
            Assert.True(ServerRuntime.MemoryBudget.WouldExceed(largest + 1));
            Assert.True(ServerRuntime.MemoryBudget.WouldExceed(
                largest * ServerRuntime.MemoryBudget.MemoryShare));
        });
    }

    [Fact]
    public void An_input_past_the_budget_is_refused_before_it_is_read()
    {
        // One eighth past the line, so the refusal comes from the budget and not from the file
        // ending early.
        long largest = ServerRuntime.MemoryBudget.LargestSupportedInputBytes;
        long oversized = largest + largest / ServerRuntime.MemoryBudget.MemoryShare;
        string map = Path.Combine(_dir, "oversized.w3x");
        using (var file = new FileStream(map, FileMode.CreateNew, FileAccess.Write))
            file.SetLength(oversized);

        CallToolResult? result = null;
        RunWith(null, () =>
            McpTestClient.WithClient(async (client, ct) =>
            {
                result = await client.CallToolAsync(
                    "map_info",
                    new Dictionary<string, object?> { ["map"] = map },
                    cancellationToken: ct);
            }).GetAwaiter().GetResult());

        Assert.NotNull(result);
        Assert.True(result!.IsError == true, "an input past the largest supported size was attempted");
        string text = result.Content?.OfType<TextContentBlock>().FirstOrDefault()?.Text ?? "";
        Assert.Contains("PRF09_MEMORY_BUDGET", text);

        // The refusal names the budget, the largest input it supports, and where the figure came
        // from. That is this host and the memory the runtime reports, not a constant in the build.
        Assert.Contains(ServerRuntime.MemoryBudget.BudgetBytes.ToString(), text);
        Assert.Contains(largest.ToString(), text);
        Assert.Contains(ServerRuntime.MemoryBudget.TotalMemoryBytes().ToString(), text);
    }

    [Fact]
    public void The_variable_moves_the_budget_in_both_directions()
    {
        RunWith(null, () =>
        {
            long derived = ServerRuntime.MemoryBudget.BudgetBytes;
            long derivedLargest = ServerRuntime.MemoryBudget.LargestSupportedInputBytes;

            // Raised to 8 GiB, the line moves up with it and the basis says the variable set it.
            RunWith("8G", () =>
            {
                long raised = ServerRuntime.MemoryBudget.FloorBytes * 8;
                Assert.Equal(raised, ServerRuntime.MemoryBudget.BudgetBytes);
                Assert.False(ServerRuntime.MemoryBudget.WouldExceed(
                    ServerRuntime.MemoryBudget.LargestSupportedInputBytes));
                Assert.True(ServerRuntime.MemoryBudget.WouldExceed(
                    ServerRuntime.MemoryBudget.LargestSupportedInputBytes + 1));
                Assert.Contains(
                    ServerRuntime.MemoryBudget.BudgetEnvVar,
                    ServerRuntime.MemoryBudget.Explain(derivedLargest + 1));
            });

            // Halved to 512 MiB, which is below the floor and so below any derived budget, the
            // 331 MB map is refused again and a small archive still opens.
            RunWith("512M", () =>
            {
                Assert.Equal(ServerRuntime.MemoryBudget.FloorBytes / 2, ServerRuntime.MemoryBudget.BudgetBytes);
                Assert.True(ServerRuntime.MemoryBudget.WouldExceed(derivedLargest));
                Assert.False(ServerRuntime.MemoryBudget.WouldExceed(
                    ServerRuntime.MemoryBudget.LargestSupportedInputBytes
                        / ServerRuntime.MemoryBudget.MemoryShare));
            });

            // The guard is back to the derived budget once the variable is gone, so one raised
            // process does not leave the ceiling open for the next call.
            Assert.Equal(ServerRuntime.MemoryBudget.BudgetFor(
                ServerRuntime.MemoryBudget.TotalMemoryBytes()), ServerRuntime.MemoryBudget.BudgetBytes);
            Assert.Equal(derived, ServerRuntime.MemoryBudget.BudgetBytes);
            Assert.False(ServerRuntime.MemoryBudget.WouldExceed(derivedLargest));
        });
    }

    [Theory]
    [InlineData("banana")]
    [InlineData("0")]
    [InlineData("-2048")]
    [InlineData("99999999999999999999")]
    [InlineData("")]
    public void A_value_that_does_not_parse_leaves_the_derived_budget_in_force(string value)
    {
        RunWith(value, () =>
        {
            Assert.Equal(ServerRuntime.MemoryBudget.BudgetFor(
                ServerRuntime.MemoryBudget.TotalMemoryBytes()), ServerRuntime.MemoryBudget.BudgetBytes);

            // The refusal still names the budget and the host memory it came from, not the variable.
            string text = ServerRuntime.MemoryBudget.Explain(
                ServerRuntime.MemoryBudget.LargestSupportedInputBytes + 1);
            Assert.Contains(ServerRuntime.MemoryBudget.BudgetBytes.ToString(), text);
            Assert.Contains(ServerRuntime.MemoryBudget.TotalMemoryBytes().ToString(), text);
            Assert.DoesNotContain(ServerRuntime.MemoryBudget.BudgetEnvVar, text);
        });
    }

    [Fact]
    public void A_bigger_host_gets_an_eighth_of_its_memory_and_a_small_one_the_floor()
    {
        // Eight gibibytes of memory is eight times the floor and gets one gibibyte back, the floor
        // itself. Thirty-two gibibytes gets four gibibytes, the reference host's budget.
        long eight = ServerRuntime.MemoryBudget.FloorBytes * ServerRuntime.MemoryBudget.MemoryShare;
        Assert.Equal(
            eight / ServerRuntime.MemoryBudget.MemoryShare,
            ServerRuntime.MemoryBudget.BudgetFor(eight));
        Assert.Equal(
            ServerRuntime.MemoryBudget.FloorBytes * 4,
            ServerRuntime.MemoryBudget.BudgetFor(eight * 4));

        // Below the line the floor holds, including a host that reports no memory at all.
        Assert.Equal(
            ServerRuntime.MemoryBudget.FloorBytes,
            ServerRuntime.MemoryBudget.BudgetFor(ServerRuntime.MemoryBudget.FloorBytes / 2));
        Assert.Equal(
            ServerRuntime.MemoryBudget.FloorBytes,
            ServerRuntime.MemoryBudget.BudgetFor(0));
    }

    [Theory]
    [InlineData("1G", 1L)]
    [InlineData("1024M", 1L)]
    [InlineData("8G", 8L)]
    public void A_suffix_value_is_read_as_bytes(string value, long floors) =>
        RunWith(value, () => Assert.Equal(
            ServerRuntime.MemoryBudget.FloorBytes * floors, ServerRuntime.MemoryBudget.BudgetBytes));

    /// <summary>Set the variable for one test body and restore what was there before.</summary>
    private static void RunWith(string? value, Action body)
    {
        string variable = ServerRuntime.MemoryBudget.BudgetEnvVar;
        string? previous = Environment.GetEnvironmentVariable(variable);
        try
        {
            Environment.SetEnvironmentVariable(variable, value);
            body();
        }
        finally
        {
            Environment.SetEnvironmentVariable(variable, previous);
        }
    }
}
