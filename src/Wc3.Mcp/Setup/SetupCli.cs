// src/Wc3.Mcp/Setup/SetupCli.cs
using Wc3.GameData;

namespace Wc3.Mcp.Setup;

/// <summary>
/// The commands that set up MCP clients. With no arguments the exe is the MCP server, so these run
/// only when the first argument is one of <see cref="Verbs"/>.
/// </summary>
public static class SetupCli
{
    public static readonly IReadOnlySet<string> Verbs = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "install", "uninstall", "config", "doctor", "clients", "help", "--help", "-h", "/?", "version", "--version",
    };

    /// <summary>Runs a setup command. Exit 0 succeeded, 1 something was refused or failed, 2 bad usage.</summary>
    public static int Run(string[] args, SetupEnvironment env, TextWriter output, SetupProduct product, ServerEntry? entryOverride = null)
    {
        if (args.Length == 0) args = new[] { "help" };
        var verb = args[0].ToLowerInvariant();
        var rest = args.Skip(1).ToList();
        string name = TakeOption(rest, "--name") ?? product.DefaultServerName;
        string? gameDir = TakeOption(rest, "--game-dir");
        bool all = rest.Remove("--all");
        bool noPath = rest.Remove("--no-path");
        if (gameDir is not null) gameDir = Path.GetFullPath(gameDir);

        switch (verb)
        {
            case "help" or "--help" or "-h" or "/?":
                output.Write(Help(product));
                return 0;
            case "version" or "--version":
                output.WriteLine($"{product.Command} {product.Version}");
                return 0;
            case "clients":
                return Clients(env, output, name);
            case "doctor":
                return Doctor(env, output, name, product);
        }

        if (rest.Any(r => r.StartsWith('-')))
        {
            output.WriteLine($"unknown option {rest.First(r => r.StartsWith('-'))}. Run {product.Command} help.");
            return 2;
        }

        var entry = entryOverride is null
            ? ServerEntry.ForThisProcess(name, gameDir, product.ServeArgs)
            : entryOverride with { Name = name, GameDir = gameDir ?? entryOverride.GameDir };

        List<ClientTarget> targets;
        if (all)
        {
            targets = ClientTargets.All.Where(t => t.IsDetected(env)).ToList();
            if (targets.Count == 0)
            {
                if (verb == "install" && !noPath) AddToPath(entry, env, output, product);
                if (verb == "uninstall") RemoveFromPath(entry, env, output);
                output.WriteLine("No supported MCP client was found on this PC. Run {product.Command} clients to see the list.");
                return 1;
            }
        }
        else if (rest.Count == 0)
        {
            output.WriteLine($"Name a client or pass --all. For example, {product.Command} {verb} cursor");
            output.WriteLine();
            Clients(env, output, name);
            return 2;
        }
        else
        {
            targets = new List<ClientTarget>();
            foreach (var id in rest)
            {
                if (ClientTargets.Find(id) is not { } t)
                {
                    output.WriteLine($"unknown client {id}. Known clients are {string.Join(", ", ClientTargets.All.Select(c => c.Id))}.");
                    return 2;
                }
                targets.Add(t);
            }
        }

        // So that the program works by name in a new terminal however the exe got here.
        if (verb == "install" && !noPath) AddToPath(entry, env, output, product);
        if (verb == "uninstall" && all) RemoveFromPath(entry, env, output);

        int failures = 0;
        foreach (var t in targets)
        {
            failures += verb switch
            {
                "install" => Install(t, entry, env, output),
                "uninstall" => Uninstall(t, name, env, output),
                "config" => PrintConfig(t, entry, env, output),
                _ => throw new InvalidOperationException(verb),
            };
        }
        if (verb == "install" && failures < targets.Count)
            output.WriteLine("Restart each client so it starts the server.");
        return failures == 0 ? 0 : 1;
    }

    /// <summary>The folder to put on PATH, or null when this runs under dotnet rather than as the exe.</summary>
    private static string? ExeFolder(ServerEntry entry) =>
        Path.GetFileNameWithoutExtension(entry.Command).Equals("dotnet", StringComparison.OrdinalIgnoreCase)
            ? null
            : Path.GetDirectoryName(entry.Command);

    private static void AddToPath(ServerEntry entry, SetupEnvironment env, TextWriter output, SetupProduct product)
    {
        if (ExeFolder(entry) is not { } folder) return;
        var path = env.GetUserPath();
        if (SetupEnvironment.PathContains(path, folder)) return;
        env.SetUserPath(string.IsNullOrEmpty(path) ? folder : path.TrimEnd(';') + ";" + folder);
        output.WriteLine($"Added {folder} to your user PATH, so {product.Command} works in any new terminal. (--no-path skips this.)");
    }

    private static void RemoveFromPath(ServerEntry entry, SetupEnvironment env, TextWriter output)
    {
        if (ExeFolder(entry) is not { } folder) return;
        var path = env.GetUserPath();
        if (!SetupEnvironment.PathContains(path, folder)) return;
        // Every other entry is kept exactly as written, %VARIABLES% included.
        var kept = (path ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Where(p => !SetupEnvironment.SameFolder(p, folder));
        env.SetUserPath(string.Join(";", kept));
        output.WriteLine($"Removed {folder} from your user PATH.");
    }

    private static int Install(ClientTarget t, ServerEntry entry, SetupEnvironment env, TextWriter output)
    {
        if (t.Style == ConfigStyle.ClaudeCli)
        {
            if (env.FindOnPath("claude") is not { } claude)
            {
                output.WriteLine($"{t.DisplayName}, the claude command is not on PATH. Run this once it is.");
                output.WriteLine("  " + ClaudeAddCommand(entry));
                return 1;
            }
            env.Run(claude, new[] { "mcp", "remove", entry.Name, "--scope", "user" });
            var (code, text) = env.Run(claude, ClaudeAddArgs(entry));
            output.WriteLine(code == 0
                ? $"{t.DisplayName}, added for your user account (claude mcp add --scope user)."
                : $"{t.DisplayName}, claude mcp add failed with exit {code}. {text}");
            return code == 0 ? 0 : 1;
        }

        var files = t.PresentFiles(env);
        if (files.Count == 0) files = new[] { t.PrimaryFile(env) };
        int failed = 0;
        foreach (var file in files)
        {
            var r = t.Style == ConfigStyle.CodexToml
                ? CodexToml.Upsert(file, entry)
                : JsonConfig.Upsert(file, t.ServersKey, entry, t.WriteType);
            output.WriteLine($"{t.DisplayName}, {r.Message}. {r.Path}");
            if (r.Outcome == WriteOutcome.Refused)
            {
                failed = 1;
                output.WriteLine(Indent(Snippet(t, entry)));
            }
        }
        return failed;
    }

    private static int Uninstall(ClientTarget t, string name, SetupEnvironment env, TextWriter output)
    {
        if (t.Style == ConfigStyle.ClaudeCli)
        {
            if (env.FindOnPath("claude") is not { } claude)
            {
                output.WriteLine($"{t.DisplayName}, the claude command is not on PATH. Run claude mcp remove {name} --scope user");
                return 1;
            }
            var (code, text) = env.Run(claude, new[] { "mcp", "remove", name, "--scope", "user" });
            output.WriteLine(code == 0 ? $"{t.DisplayName}, removed." : $"{t.DisplayName}, nothing removed. {text}");
            return 0;
        }

        int failed = 0, reported = 0;
        foreach (var file in t.Candidates(env).Select(c => c.ConfigFile).Distinct())
        {
            var r = t.Style == ConfigStyle.CodexToml ? CodexToml.Remove(file, name) : JsonConfig.Remove(file, t.ServersKey, name);
            if (r.Outcome == WriteOutcome.NotPresent && !File.Exists(file)) continue;
            output.WriteLine($"{t.DisplayName}, {r.Message}. {r.Path}");
            reported++;
            if (r.Outcome == WriteOutcome.Refused) failed = 1;
        }
        if (reported == 0) output.WriteLine($"{t.DisplayName}, was not set up.");
        return failed;
    }

    private static int PrintConfig(ClientTarget t, ServerEntry entry, SetupEnvironment env, TextWriter output)
    {
        output.WriteLine($"{t.DisplayName}");
        if (t.Style == ConfigStyle.ClaudeCli)
        {
            output.WriteLine("  Run");
            output.WriteLine("  " + ClaudeAddCommand(entry));
        }
        else
        {
            output.WriteLine($"  Add this to {t.PrimaryFile(env)}");
            output.WriteLine(Indent(Snippet(t, entry)));
        }
        output.WriteLine();
        return 0;
    }

    private static int Clients(SetupEnvironment env, TextWriter output, string name)
    {
        output.WriteLine("Supported MCP clients");
        foreach (var t in ClientTargets.All)
        {
            var state = !t.IsDetected(env) ? "not found"
                : IsRegistered(t, env, name) ? "found, set up"
                : t.Style == ConfigStyle.ClaudeCli ? "found"
                : "found, not set up";
            output.WriteLine($"  {t.Id,-15}{t.DisplayName,-26}{state}");
        }
        return 0;
    }

    private static int Doctor(SetupEnvironment env, TextWriter output, string name, SetupProduct product)
    {
        int problems = 0;
        var exe = Environment.ProcessPath ?? "(unknown)";
        output.WriteLine($"{product.Command} {product.Version}");
        output.WriteLine($"  program      {exe}");

        var casc = Path.Combine(AppContext.BaseDirectory, "CascLib.dll");
        bool hasCasc = File.Exists(casc);
        output.WriteLine($"  CascLib.dll  {(hasCasc ? "present" : "MISSING, keep it beside the exe or game data cannot be read")}");
        if (!hasCasc) problems++;

        var envDir = Environment.GetEnvironmentVariable(Wc3Tools.GameDirVariable);
        var game = GameInstall.Locate(Wc3Tools.ResolveGameDir(null));
        output.WriteLine(game is not null
            ? $"  Warcraft III {game}{(string.IsNullOrWhiteSpace(envDir) ? " (found automatically)" : $" (from {Wc3Tools.GameDirVariable})")}"
            : $"  Warcraft III not found. Map tools still work. Tools that need base game data need --game-dir or {Wc3Tools.GameDirVariable}.");

        output.WriteLine();
        Clients(env, output, name);
        return problems == 0 ? 0 : 1;
    }

    private static bool IsRegistered(ClientTarget t, SetupEnvironment env, string name) => t.Style switch
    {
        ConfigStyle.CodexToml => t.PresentFiles(env).Any(f => CodexToml.Contains(f, name)),
        ConfigStyle.Json => t.PresentFiles(env).Any(f => JsonConfig.Contains(f, t.ServersKey, name)),
        // Claude Code keeps its list inside a large state file, so only its own CLI is asked.
        _ => false,
    };

    private static string Snippet(ClientTarget t, ServerEntry entry) => t.Style == ConfigStyle.CodexToml
        ? CodexToml.Block(entry).TrimEnd()
        : JsonConfig.Snippet(t.ServersKey, entry, t.WriteType);

    private static IReadOnlyList<string> ClaudeAddArgs(ServerEntry e)
    {
        var a = new List<string> { "mcp", "add", "--transport", "stdio", "--scope", "user" };
        if (e.GameDir is not null) { a.Add("--env"); a.Add($"{Wc3Tools.GameDirVariable}={e.GameDir}"); }
        a.Add(e.Name);
        a.Add("--");
        a.Add(e.Command);
        a.AddRange(e.Args);
        return a;
    }

    private static string ClaudeAddCommand(ServerEntry e) =>
        "claude " + string.Join(" ", ClaudeAddArgs(e).Select(s => s.Contains(' ') ? $"\"{s}\"" : s));

    private static string? TakeOption(List<string> args, string option)
    {
        int i = args.FindIndex(a => a.Equals(option, StringComparison.OrdinalIgnoreCase));
        if (i < 0 || i + 1 >= args.Count) return null;
        var value = args[i + 1];
        args.RemoveRange(i, 2);
        return value;
    }

    private static string Indent(string text) =>
        string.Join(Environment.NewLine, text.Replace("\r\n", "\n").Split('\n').Select(l => "    " + l));

    private static string Help(SetupProduct p) => $"""
        {p.Command}, setup for the Warcraft III map MCP server.

        {(p.ServeArgs.Count == 0 ? "Run with no arguments, it is the MCP server (stdio)." : $"'{p.Command} serve' is the MCP server (stdio).")} Your AI app starts it this way.

        Setting up an AI app
          {p.Command} install --all            set up every supported app found on this PC
          {p.Command} install cursor vscode    set up the apps named
          {p.Command} uninstall --all          remove it from every app and from PATH
          {p.Command} config <app>             print the settings to paste by hand
          {p.Command} clients                  list supported apps and whether each is set up
          {p.Command} doctor                   check the install, the game folder and the apps

        Options
          --game-dir <folder>   the Warcraft III folder, when it is not found automatically
          --name <name>         the server name in the app (default {p.DefaultServerName})
          --no-path             install leaves your user PATH alone (by default it adds
                                this folder once, so the program works in new terminals)

        Apps
          claude-code, claude-desktop, cursor, vscode, windsurf, gemini, codex, cline, lmstudio, zed

        Every edited settings file is first copied to NAME{ConfigFile.BackupSuffix} beside it. A file with
        comments is never rewritten. The settings to paste are printed instead.

        """;
}
