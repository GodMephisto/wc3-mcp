// tests/Wc3.Tests/ObservabilityLogContentTests.cs
// OBS-03. A log line may carry a path, a size, a digest and a duration. It may not carry map
// contents, script text, a string-table value or a file body. The records observed before this
// test carried the tool name and a code and nothing else, but no test failed when a line carried
// content it should not, so the rule was a description rather than a check.
//
// The capture is the real host filter (ServerRuntime.ObserveToolCalls) reached through the shipped
// server options, driven over in-memory pipes with a logger factory the test owns. The map is built
// here and carries two markers, one in the script and one in the string table, so a line that
// leaked either is caught by name rather than by eye.
using System.IO.Pipelines;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Wc3.Commands;
using Wc3.Mcp;
using Wc3.Model;

namespace Wc3.Tests;

public sealed class ObservabilityLogContentTests : IDisposable
{
    private const string ScriptCanary = "OBS03SCRIPTTEXTMARKER";
    private const string StringCanary = "OBS03STRINGVALUEMARKER";

    private readonly string _dir;
    private readonly string _map;

    public ObservabilityLogContentTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "wc3obs-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);

        var doc = BlankMap.Create();
        FileEditCommand.WriteText(doc, "war3map.j",
            "function OBS03SCRIPTTEXTMARKER takes nothing returns nothing\n"
            + "    call BJDebugMsg(\"" + ScriptCanary + "\")\n"
            + "endfunction\n"
            + "function main takes nothing returns nothing\nendfunction\n");
        doc.AddOrReplaceRawFile("war3map.wts",
            Encoding.UTF8.GetBytes("STRING 1\r\n{\r\n" + StringCanary + "\r\n}\r\n"));

        _map = Path.Combine(_dir, "log.w3x");
        File.WriteAllBytes(_map, doc.SaveToBytes());
    }

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch (IOException) { }
    }

    [Fact]
    public async Task A_failed_call_names_the_tool_and_carries_no_map_content()
    {
        var capture = new CapturingLoggerProvider();
        var services = new ServiceCollection();
        services.AddLogging(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddProvider(capture);
        });
        await using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<ILoggerFactory>();

        var options = new McpServerOptions();
        Wc3McpServer.ApplyTo(options, Wc3McpServer.CreateOptions());

        using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(2));
        Pipe clientToServer = new(), serverToClient = new();
        await using var server = McpServer.Create(
            new StreamServerTransport(clientToServer.Reader.AsStream(), serverToClient.Writer.AsStream()),
            options, loggerFactory: factory, serviceProvider: provider);
        Task run = server.RunAsync(timeout.Token);
        try
        {
            await using var client = await McpClient.CreateAsync(
                new StreamClientTransport(clientToServer.Writer.AsStream(), serverToClient.Reader.AsStream()),
                cancellationToken: timeout.Token);

            // The map loads, so its script and its string table are in memory when the call fails,
            // which is the state a leaky log line would be written from.
            var result = await client.CallToolAsync("placed_unit_get",
                new Dictionary<string, object?> { ["map"] = _map, ["creation_number"] = 424242 },
                cancellationToken: timeout.Token);

            Assert.True(result.IsError, "the call was expected to refuse the missing creation number");
        }
        finally
        {
            timeout.Cancel();
            try { await run; } catch (OperationCanceledException) { }
        }

        string[] lines = capture.Lines.ToArray();
        Assert.NotEmpty(lines);

        // OBS-03. The rule is about what a line may carry, so the check is on every line, including
        // the ones the SDK writes.
        foreach (string line in lines)
        {
            Assert.DoesNotContain(ScriptCanary, line, StringComparison.Ordinal);
            Assert.DoesNotContain(StringCanary, line, StringComparison.Ordinal);
            Assert.DoesNotContain("endfunction", line, StringComparison.Ordinal);
        }

        // The line that is there carries only the allowed fields, the tool and the code and the time.
        string failure = Assert.Single(lines, l =>
            l.Contains("placed_unit_get", StringComparison.Ordinal)
            && l.Contains("failed with code", StringComparison.Ordinal));
        Assert.Contains("ms", failure, StringComparison.Ordinal);
    }

    /// <summary>A logger provider that keeps the rendered line, which is exactly what reaches a
    /// terminal or a log file.</summary>
    private sealed class CapturingLoggerProvider : ILoggerProvider, ILogger
    {
        private readonly System.Collections.Concurrent.ConcurrentQueue<string> _lines = new();

        public IReadOnlyCollection<string> Lines => _lines;

        public ILogger CreateLogger(string categoryName) => this;

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            ArgumentNullException.ThrowIfNull(formatter);
            _lines.Enqueue(formatter(state, exception));
        }

        public void Dispose() { }
    }
}
