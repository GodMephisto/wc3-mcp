// src/Wc3.Mcp/Program.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Wc3.Mcp;

/// <summary>MCP server over stdio: newline-delimited JSON-RPC on stdin/stdout.</summary>
public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        // stdout carries the MCP protocol - every log line must go to stderr.
        builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

        builder.Services
            .AddMcpServer(options =>
            {
                var configured = Wc3McpServer.CreateOptions();
                options.ServerInfo = configured.ServerInfo;
                options.ToolCollection = configured.ToolCollection;
            })
            .WithStdioServerTransport();

        await builder.Build().RunAsync();
    }
}
