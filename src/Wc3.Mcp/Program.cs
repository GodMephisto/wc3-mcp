// src/Wc3.Mcp/Program.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Wc3.Mcp;

/// <summary>The MCP server over stdio (newline-delimited JSON-RPC), or a setup command when one is named.</summary>
public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        // wc3-mcp install, doctor and the rest set up AI apps. With no such verb this is the server.
        if (args.Length > 0 && Setup.SetupCli.Verbs.Contains(args[0]))
            return Setup.SetupCli.Run(args, Setup.SetupEnvironment.Current, Console.Out);

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
        return 0;
    }
}
