// src/Wc3.Mcp/Setup/SetupProduct.cs
namespace Wc3.Mcp.Setup;

/// <summary>
/// Which program the setup commands belong to, so one setup code path serves both ways of
/// starting the server. <see cref="Command"/> is what a person types to reach setup,
/// <see cref="DefaultServerName"/> is the name written into each AI app, and
/// <see cref="ServeArgs"/> are the arguments the app passes to start the server.
/// </summary>
public sealed record SetupProduct(string Command, string DefaultServerName, IReadOnlyList<string> ServeArgs, string Version)
{
    /// <summary>
    /// The MCP server run as its own exe, which is the server when started with no arguments.
    /// Wc3.Mcp in this repository, wc3-mcp (registered as "wc3") from the public one.
    /// </summary>
    public static SetupProduct Standalone { get; } =
        new(ServerIdentity.ExeName, ServerIdentity.ServerName, Array.Empty<string>(), ServerIdentity.Version);

    /// <summary>The server reached through the CLI, as 'wc3ctl mcp serve'.</summary>
    public static SetupProduct Wc3ctl { get; } =
        new("wc3ctl mcp", "wc3ctl", new[] { "mcp", "serve" }, ServerIdentity.Version);
}
