// src/Wc3.Commands/ReleaseVersionCommand.cs
using System.Reflection;

namespace Wc3.Commands;

/// <summary>
/// The version block for a build. The product version and the git short hash and UTC build time
/// come from the assembly's own metadata, so a build reports exactly what it is, and the MCP server
/// name and version come from the server identity carried inside the same build. Read-only.
/// </summary>
public sealed record ReleaseVersion(
    string ProductVersion,
    string GitHash,
    string BuildTimeUtc,
    string McpServerName,
    string McpVersion);

public static class ReleaseVersionCommand
{
    public const string GitHashKey = "GitHash";
    public const string BuildTimeKey = "BuildTimeUtc";
    public const string NoGit = "nogit";
    public const string NoBuildTime = "unknown";

    /// <summary>Read the version block from an assembly plus the MCP server identity it carries.</summary>
    public static ReleaseVersion Read(Assembly productAssembly, string mcpServerName, string mcpVersion)
    {
        var metadata = productAssembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .GroupBy(a => a.Key, StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => g.First().Value ?? string.Empty, StringComparer.Ordinal);
        return new ReleaseVersion(
            VersionOf(productAssembly),
            Value(metadata, GitHashKey, NoGit),
            Value(metadata, BuildTimeKey, NoBuildTime),
            mcpServerName,
            mcpVersion);
    }

    /// <summary>The assembly's three-part version, or 0.0.0 when the build set none.</summary>
    public static string VersionOf(Assembly assembly) =>
        assembly.GetName().Version?.ToString(3) ?? "0.0.0";

    private static string Value(IReadOnlyDictionary<string, string> metadata, string key, string fallback) =>
        metadata.TryGetValue(key, out var value) && value.Length > 0 ? value : fallback;
}
