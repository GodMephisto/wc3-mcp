// src/Wc3.Mcp/Wc3McpServer.cs
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace Wc3.Mcp;

/// <summary>
/// Single source of truth for the server identity and tool set, shared by the
/// stdio entry point (Program) and the in-memory protocol tests, so what the
/// tests verify is exactly what a real MCP client sees.
/// </summary>
public static class Wc3McpServer
{
    /// <summary>Result POCOs serialize like the CLI's --json, enums as names ("Unit"), not numbers.</summary>
    public static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions(McpJsonUtilities.DefaultOptions);
        options.Converters.Add(new JsonStringEnumConverter());
        // A placed unit's or doodad's scale is a tuple, which printed as {} (see the converter).
        options.Converters.Add(new Wc3.Commands.ScaleJsonConverter());
        return options;
    }

    /// <summary>Server options with every wc3ctl tool registered.</summary>
    public static McpServerOptions CreateOptions()
    {
        var tools = new McpServerPrimitiveCollection<McpServerTool>();
        foreach (var tool in CreateTools())
            tools.Add(tool);

        var resources = new McpServerResourceCollection();
        resources.Add(McpToolCatalog.CreateCatalogResource());

        var prompts = new McpServerPrimitiveCollection<McpServerPrompt>();
        foreach (var prompt in CreatePrompts())
            prompts.Add(prompt);

        return new McpServerOptions
        {
            ServerInfo = new Implementation { Name = ServerIdentity.ProductId, Version = ServerIdentity.Version },
            ToolCollection = tools,
            PromptCollection = prompts,
            ResourceCollection = resources,
            ServerInstructions = Instructions.Value,
        };
    }

    /// <summary>
    /// The initialize instructions, computed once per process. <see cref="BuildInstructions"/>
    /// resolves the target, which walks the schemas folder, so recomputing it for every session
    /// made each in-memory MCP test pay for a full scan and the suite took minutes instead of
    /// seconds. The instructions are fixed at start, which matches the reply being sent once.
    /// </summary>
    private static readonly Lazy<string> Instructions =
        new(BuildInstructions, LazyThreadSafetyMode.ExecutionAndPublication);

    private static string BuildInstructions()
    {
        try
        {
            var settings = Wc3.Commands.TargetBuildSettings.Get();
            string source = settings.Source switch
            {
                Wc3.Commands.TargetSource.Option => "option",
                Wc3.Commands.TargetSource.Environment => "environment variable",
                Wc3.Commands.TargetSource.Settings => "settings",
                Wc3.Commands.TargetSource.Default => "default",
                _ => "unknown",
            };
            string? installDir = null;
            try { installDir = Wc3.GameData.GameInstall.Locate(); } catch { /* ignore */ }
            string? installBuild = null;
            if (installDir is not null)
            {
                try { installBuild = Wc3.GameData.GameInstall.ReadBuildVersion(installDir); } catch { /* ignore */ }
            }
            return "target  " + settings.Build + "  (" + source + ")"
                + (installBuild is not null ? ", game install " + installBuild : "");
        }
        catch
        {
            return "target resolution unavailable (no schemas directory configured)";
        }
    }

    /// <summary>Every [McpServerTool] method on <see cref="Wc3Tools"/>, bound with our JSON options.</summary>
    public static IReadOnlyList<McpServerTool> CreateTools() =>
        typeof(Wc3Tools)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.GetCustomAttribute<McpServerToolAttribute>() is not null)
            .Select(m => McpServerTool.Create(m, target: null,
                new McpServerToolCreateOptions { SerializerOptions = JsonOptions }))
            .ToList();

    /// <summary>Every [McpServerPrompt] method on <see cref="Wc3Prompts"/>, bound with our JSON options.</summary>
    public static IReadOnlyList<McpServerPrompt> CreatePrompts() =>
        typeof(Wc3Prompts)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.GetCustomAttribute<McpServerPromptAttribute>() is not null)
            .Select(m => McpServerPrompt.Create(m, target: null,
                new McpServerPromptCreateOptions { SerializerOptions = JsonOptions }))
            .ToList();

    /// <summary>
    /// Populates a host's options from <see cref="CreateOptions"/>. Both the stdio entry point and
    /// the in-memory protocol tests call this one method, so what the tests verify is exactly what a
    /// client receives, ServerInstructions included. Copying the fields by hand instead is what left
    /// the instructions off the initialize reply.
    /// </summary>
    public static void ApplyTo(McpServerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        var configured = CreateOptions();
        options.ServerInfo = configured.ServerInfo;
        options.Capabilities = configured.Capabilities;
        options.ServerInstructions = configured.ServerInstructions;
        options.ToolCollection = configured.ToolCollection;
        options.PromptCollection = configured.PromptCollection;
        options.ResourceCollection = configured.ResourceCollection;
    }
}
