// src/Wc3.Mcp/McpLogLevel.cs
using Microsoft.Extensions.Logging;

namespace Wc3.Mcp;

/// <summary>
/// The log level the server runs at, from <c>WC3_MCP_LOG</c> (ENV-05). The default is warn, an
/// unknown value falls back to warn rather than to a guess, and no value enables a per-call
/// payload log because no such log exists.
/// </summary>
public static class McpLogLevel
{
    /// <summary>The environment variable, named in section 6.8.</summary>
    public const string Variable = "WC3_MCP_LOG";

    /// <summary>Maps one value to a level. Case and surrounding space do not matter.</summary>
    public static LogLevel Parse(string? value) => value?.Trim().ToLowerInvariant() switch
    {
        "error" => LogLevel.Error,
        "info" or "information" => LogLevel.Information,
        "debug" => LogLevel.Debug,
        "warn" or "warning" => LogLevel.Warning,
        _ => LogLevel.Warning,
    };

    /// <summary>The level the current process environment asks for.</summary>
    public static LogLevel FromEnvironment() => Parse(Environment.GetEnvironmentVariable(Variable));
}
