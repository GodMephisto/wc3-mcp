// src/Wc3.Mcp/Program.cs
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Wc3.Mcp.Setup;

namespace Wc3.Mcp;

/// <summary>
/// With no arguments, the MCP server over stdio (newline-delimited JSON-RPC on stdin/stdout).
/// With a setup verb (install, uninstall, config, clients, doctor, help, version), the setup commands.
/// </summary>
public static class Program
{
    /// <summary>The one target this binary is published for. Anything else gets a refusal, not the
    /// operating system's message about a Windows executable.</summary>
    public const string SupportedTarget = "win-x64";

    public static async Task<int> Main(string[] args)
    {
        // PRF-01. The first touch of the clock is the first statement of Main, so the cold start
        // line carries the runtime and the JIT rather than only the work this file does.
        ServerRuntime.ColdStart.Enable();

        OSPlatform os = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? OSPlatform.Windows
            : RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? OSPlatform.Linux
            : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? OSPlatform.OSX
            : OSPlatform.Create("unknown");

        // Before any MCP output and before any setup verb, so stdout stays clean and the caller
        // sees one sentence instead of a loader error. Exit 3 is distinct from a command failure.
        if (PlatformRefusal(os, RuntimeInformation.ProcessArchitecture) is { } refusal)
        {
            Console.Error.WriteLine(refusal);
            return 3;
        }

        if (args.Length > 0)
            return SetupCli.Run(args, SetupEnvironment.Current, Console.Out, SetupProduct.Standalone);

        await RunServerAsync();
        return 0;
    }

    /// <summary>
    /// Why this build cannot run on the given platform, or null when it can. A function of the
    /// measured platform rather than of the running process, so the guard is testable for a
    /// platform this machine is not.
    /// </summary>
    internal static string? PlatformRefusal(OSPlatform os, Architecture arch)
    {
        if (os == OSPlatform.Windows && arch == Architecture.X64) return null;
        string osName = os == OSPlatform.Windows ? "Windows"
            : os == OSPlatform.Linux ? "Linux"
            : os == OSPlatform.OSX ? "macOS"
            : os.ToString();
        return "This build supports " + SupportedTarget + " only, so it cannot run on "
            + osName + " " + arch.ToString().ToLowerInvariant() + ".";
    }

    /// <summary>Runs the MCP server on stdio until the client closes the stream. Shared with 'wc3ctl mcp serve'.</summary>
    public static async Task RunServerAsync()
    {
        // PRF-01. The surface, the host and the start line are three independent pieces of work, so
        // they run together rather than one after the other. The surface still has to exist before
        // the host resolves its options, so it is started first and the host build is handed to the
        // pool before the line is written. What used to be startup plus surface plus host is now the
        // longest of the three, and the line still reaches stderr before the protocol starts.
        Task<McpServerOptions> surface = Task.Run(() => Wc3McpServer.CreateOptions());

        var builder = Host.CreateApplicationBuilder(Array.Empty<string>());

        // stdout carries the MCP protocol, so every log line must go to stderr.
        builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);
        builder.Logging.SetMinimumLevel(McpLogLevel.FromEnvironment());
        // OBS-02 logs a slow call at information. The category keeps that level even when the
        // server runs at the default warn, so the line is there without turning on everything.
        builder.Logging.AddFilter(ServerRuntime.Category, LogLevel.Information);

        // The surface is awaited inside the options callback, so the host build joins the surface
        // task rather than reading a result that may not be there yet.
        builder.Services
            .AddMcpServer(o => Wc3McpServer.ApplyTo(o, surface.GetAwaiter().GetResult()))
            .WithStdioServerTransport();

        // TRN-03. The stdio registration above opens its own streams, so ours goes in last and the
        // container resolves it. The gate is attached to the transport's pending queue, so the read
        // side holds end of stream until every message already read has been dispatched and every
        // response has been written, including a call that is still queued in the transport at the
        // moment the client closes the pipe.
        var drain = new ServerRuntime.DrainGate();
        builder.Services.AddSingleton<ITransport>(sp =>
        {
            var transport = new StreamServerTransport(
                new ServerRuntime.DrainingStdinStream(Console.OpenStandardInput(), drain),
                new BufferedStream(Console.OpenStandardOutput()),
                ServerIdentity.ProductId,
                sp.GetService<ILoggerFactory>());
            drain.Attach(() => transport.MessageReader.Count);
            return transport;
        });

        // PRF-01. The host build is started here, before the line, so its cost overlaps the start
        // line instead of following it. The host touches no console stream until it runs, which is
        // after the line has been written.
        Task<IHost> hosting = Task.Run(() => builder.Build());

        // One line on stderr before the protocol starts, so a bug report can quote the build it
        // came from (ENV-04). stdout stays clean for JSON-RPC.
        Console.Error.WriteLine(Wc3McpServer.BuildStartLine());
        ServerRuntime.ColdStart.StartLineWritten();

        using IHost host = hosting.GetAwaiter().GetResult();
        ServerRuntime.ColdStart.HostBuilt();

        // Joins the parallel surface build, so a throw there is the start failure rather than a
        // hang inside the first request.
        _ = await surface.ConfigureAwait(false);

        await host.RunAsync();

        // TRN-03. The read side already held the end of stream until the queue was drained, so this
        // is the last look rather than the wait. A short bound keeps a leaked record from delaying
        // the exit. Exit stays 0 either way.
        await drain.WaitAsync(ServerRuntime.ResponseGrace);
    }
}
