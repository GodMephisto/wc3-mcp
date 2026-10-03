// src/Wc3.Mcp/ServerIdentity.cs
using System.Reflection;

namespace Wc3.Mcp;

/// <summary>
/// Who this build of the server says it is. The values come from the build (the McpProductId and
/// McpServerName properties in Wc3.Mcp.csproj, written into the assembly as metadata), so the same
/// source ships as wc3ctl here and as wc3-mcp from the public repo, which sets them in its own
/// Directory.Build.props.
/// </summary>
public static class ServerIdentity
{
    private static readonly Assembly Self = typeof(ServerIdentity).Assembly;

    private static string Metadata(string key, string fallback) =>
        Self.GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(a => a.Key == key)?.Value is { Length: > 0 } value ? value : fallback;

    /// <summary>The product name reported to MCP clients and used in backup file names.</summary>
    public static string ProductId { get; } = Metadata("McpProductId", "wc3ctl");

    /// <summary>The name the server is registered under in each AI app.</summary>
    public static string ServerName { get; } = Metadata("McpServerName", "wc3ctl");

    /// <summary>The exe's own name without extension, what a person types to run it.</summary>
    public static string ExeName { get; } = Self.GetName().Name ?? "Wc3.Mcp";

    /// <summary>The build's version, three parts.</summary>
    public static string Version { get; } = Self.GetName().Version?.ToString(3) ?? "0.0.0";
}
