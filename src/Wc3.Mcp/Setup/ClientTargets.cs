// src/Wc3.Mcp/Setup/ClientTargets.cs
namespace Wc3.Mcp.Setup;

/// <summary>How a client stores its MCP server list.</summary>
public enum ConfigStyle
{
    /// <summary>A JSON file with a map of servers under one top-level key.</summary>
    Json,
    /// <summary>Codex's TOML file, one [mcp_servers.NAME] table per server.</summary>
    CodexToml,
    /// <summary>A client with its own add and remove commands (Claude Code).</summary>
    ClaudeCli,
}

/// <summary>
/// One MCP client the setup commands know. Paths are worked out from <see cref="SetupEnvironment"/>
/// so tests can point every client at a temp folder.
/// </summary>
/// <param name="Id">The name typed on the command line, for example <c>cursor</c>.</param>
/// <param name="DisplayName">The product name shown to people.</param>
/// <param name="Style">How the config is written.</param>
/// <param name="ServersKey">Top-level key holding the server map (Json style only).</param>
/// <param name="WriteType">Whether entries carry <c>"type": "stdio"</c>.</param>
/// <param name="Candidates">Config files to use, each paired with the folder whose presence means the client is installed.</param>
/// <param name="Docs">Where the format was confirmed.</param>
public sealed record ClientTarget(
    string Id,
    string DisplayName,
    ConfigStyle Style,
    string ServersKey,
    bool WriteType,
    Func<SetupEnvironment, IReadOnlyList<(string ConfigFile, string MarkerDir)>> Candidates,
    string Docs)
{
    /// <summary>The config files whose client folder exists on this machine.</summary>
    public IReadOnlyList<string> PresentFiles(SetupEnvironment env) =>
        Candidates(env).Where(c => Directory.Exists(c.MarkerDir)).Select(c => c.ConfigFile).Distinct().ToList();

    /// <summary>The file to write when the client is installed, or the first candidate otherwise.</summary>
    public string PrimaryFile(SetupEnvironment env) =>
        PresentFiles(env).FirstOrDefault() ?? Candidates(env)[0].ConfigFile;

    /// <summary>Whether this client looks installed.</summary>
    public bool IsDetected(SetupEnvironment env) => Style == ConfigStyle.ClaudeCli
        ? env.FindOnPath("claude") is not null
        : PresentFiles(env).Count > 0;
}

/// <summary>Every client the setup commands support, with formats checked against each client's docs on 2026-10-03.</summary>
public static class ClientTargets
{
    public static readonly IReadOnlyList<ClientTarget> All = new[]
    {
        new ClientTarget("claude-code", "Claude Code", ConfigStyle.ClaudeCli, "", false,
            e => new[] { (Path.Combine(e.Home, ".claude.json"), Path.Combine(e.Home, ".claude")) },
            "https://code.claude.com/docs/en/mcp"),
        new ClientTarget("claude-desktop", "Claude Desktop", ConfigStyle.Json, "mcpServers", false,
            e => ClaudeDesktopCandidates(e),
            "https://modelcontextprotocol.io/docs/develop/connect-local-servers"),
        new ClientTarget("cursor", "Cursor", ConfigStyle.Json, "mcpServers", true,
            e => new[] { (Path.Combine(e.Home, ".cursor", "mcp.json"), Path.Combine(e.Home, ".cursor")) },
            "https://cursor.com/docs/context/mcp"),
        new ClientTarget("vscode", "VS Code (GitHub Copilot)", ConfigStyle.Json, "servers", true,
            e => new[] { (Path.Combine(e.AppData, "Code", "User", "mcp.json"), Path.Combine(e.AppData, "Code", "User")) },
            "https://code.visualstudio.com/docs/copilot/customization/mcp-servers"),
        new ClientTarget("windsurf", "Windsurf", ConfigStyle.Json, "mcpServers", false,
            e => new[]
            {
                (Path.Combine(e.Home, ".codeium", "windsurf", "mcp_config.json"), Path.Combine(e.Home, ".codeium", "windsurf")),
                (Path.Combine(e.AppData, "devin", "mcp_config.json"), Path.Combine(e.AppData, "devin")),
            },
            "https://docs.devin.ai/desktop/cascade/mcp"),
        new ClientTarget("gemini", "Gemini CLI", ConfigStyle.Json, "mcpServers", false,
            e => new[] { (Path.Combine(e.Home, ".gemini", "settings.json"), Path.Combine(e.Home, ".gemini")) },
            "https://github.com/google-gemini/gemini-cli/blob/main/docs/tools/mcp-server.md"),
        new ClientTarget("codex", "Codex CLI", ConfigStyle.CodexToml, "mcp_servers", false,
            e => new[] { (Path.Combine(e.CodexHome, "config.toml"), e.CodexHome) },
            "https://developers.openai.com/codex/mcp"),
        new ClientTarget("cline", "Cline", ConfigStyle.Json, "mcpServers", false,
            e => new[]
            {
                (Path.Combine(e.AppData, "Code", "User", "globalStorage", "saoudrizwan.claude-dev", "settings", "cline_mcp_settings.json"),
                 Path.Combine(e.AppData, "Code", "User", "globalStorage", "saoudrizwan.claude-dev")),
            },
            "https://docs.cline.bot/mcp/configuring-mcp-servers"),
        new ClientTarget("lmstudio", "LM Studio", ConfigStyle.Json, "mcpServers", false,
            e => new[] { (Path.Combine(e.Home, ".lmstudio", "mcp.json"), Path.Combine(e.Home, ".lmstudio")) },
            "https://lmstudio.ai/docs/app/mcp"),
        new ClientTarget("zed", "Zed", ConfigStyle.Json, "context_servers", false,
            e => new[] { (Path.Combine(e.AppData, "Zed", "settings.json"), Path.Combine(e.AppData, "Zed")) },
            "https://zed.dev/docs/ai/mcp"),
    };

    /// <summary>Finds a client by id, ignoring case.</summary>
    public static ClientTarget? Find(string id) =>
        All.FirstOrDefault(t => string.Equals(t.Id, id, StringComparison.OrdinalIgnoreCase));

    // The Microsoft Store build of Claude Desktop keeps its config inside its package folder.
    private static IReadOnlyList<(string, string)> ClaudeDesktopCandidates(SetupEnvironment e)
    {
        var list = new List<(string, string)>
        {
            (Path.Combine(e.AppData, "Claude", "claude_desktop_config.json"), Path.Combine(e.AppData, "Claude")),
        };
        var packages = Path.Combine(e.LocalAppData, "Packages");
        if (Directory.Exists(packages))
        {
            foreach (var pkg in Directory.EnumerateDirectories(packages, "Claude_*"))
            {
                var dir = Path.Combine(pkg, "LocalCache", "Roaming", "Claude");
                list.Add((Path.Combine(dir, "claude_desktop_config.json"), dir));
            }
        }
        return list;
    }
}
