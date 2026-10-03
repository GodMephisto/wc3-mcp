// tests/Wc3.Tests/McpTestClient.cs
// The real server wiring (Wc3McpServer.CreateOptions, exactly what the stdio exe serves)
// driven by the SDK's own client over in-memory pipes. No processes, no network.
using System.IO.Pipelines;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Wc3.Mcp;

namespace Wc3.Tests;

internal static class McpTestClient
{
    /// <summary>Runs the body against a live in-memory client and server session.</summary>
    public static async Task WithClient(Func<McpClient, CancellationToken, Task> body)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(120));
        Pipe clientToServer = new(), serverToClient = new();
        await using var server = McpServer.Create(
            new StreamServerTransport(clientToServer.Reader.AsStream(), serverToClient.Writer.AsStream()),
            Wc3McpServer.CreateOptions());
        Task run = server.RunAsync(timeout.Token);
        try
        {
            await using var client = await McpClient.CreateAsync(
                new StreamClientTransport(clientToServer.Writer.AsStream(), serverToClient.Reader.AsStream()),
                cancellationToken: timeout.Token);
            await body(client, timeout.Token);
        }
        finally
        {
            timeout.Cancel();
            try { await run; } catch (OperationCanceledException) { }
        }
    }
}
