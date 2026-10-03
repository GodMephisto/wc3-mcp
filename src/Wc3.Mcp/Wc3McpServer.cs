// src/Wc3.Mcp/Wc3McpServer.cs
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace Wc3.Mcp;

/// <summary>
/// Single source of truth for the server identity and tool set — shared by the
/// stdio entry point (Program) and the in-memory protocol tests, so what the
/// tests verify is exactly what a real MCP client sees.
/// </summary>
public static class Wc3McpServer
{
    /// <summary>Result POCOs serialize like the CLI's --json: enums as names ("Unit"), not numbers.</summary>
    public static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions(McpJsonUtilities.DefaultOptions);
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }

    /// <summary>The release version, reported to clients and by wc3-mcp version.</summary>
    public const string Version = "0.1.3";

    /// <summary>Server options with every tool registered.</summary>
    public static McpServerOptions CreateOptions()
    {
        var tools = new McpServerPrimitiveCollection<McpServerTool>();
        foreach (var tool in CreateTools())
            tools.Add(tool);
        return new McpServerOptions
        {
            ServerInfo = new Implementation { Name = "wc3-mcp", Version = Version },
            ToolCollection = tools,
        };
    }

    /// <summary>Every [McpServerTool] method on <see cref="Wc3Tools"/>, bound with our JSON options.</summary>
    public static IReadOnlyList<McpServerTool> CreateTools() =>
        typeof(Wc3Tools)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.GetCustomAttribute<McpServerToolAttribute>() is not null)
            .Select(m => McpServerTool.Create(m, target: null,
                new McpServerToolCreateOptions { SerializerOptions = JsonOptions }))
            .ToList();
}
