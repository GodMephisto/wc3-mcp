// src/Wc3.Mcp/Program.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wc3.Mcp.Setup;

namespace Wc3.Mcp;

/// <summary>
/// With no arguments, the MCP server over stdio (newline-delimited JSON-RPC on stdin/stdout).
/// With a setup verb (install, uninstall, config, clients, doctor, help, version), the setup commands.
/// </summary>
public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        if (args.Length > 0)
            return SetupCli.Run(args, SetupEnvironment.Current, Console.Out, SetupProduct.Standalone);

        await RunServerAsync();
        return 0;
    }

    /// <summary>Runs the MCP server on stdio until the client closes the stream. Shared with 'wc3ctl mcp serve'.</summary>
    public static async Task RunServerAsync()
    {
        var builder = Host.CreateApplicationBuilder(Array.Empty<string>());

        // stdout carries the MCP protocol, so every log line must go to stderr.
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
