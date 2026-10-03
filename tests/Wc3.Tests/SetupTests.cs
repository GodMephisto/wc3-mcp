// tests/Wc3.Tests/SetupTests.cs
// The client setup commands, run against a fake user profile in a temp folder so no real
// client config is read or written.
using System.Text.Json.Nodes;
using Wc3.Mcp.Setup;

namespace Wc3.Tests;

public sealed class SetupTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "wc3ctl-setup-" + Guid.NewGuid().ToString("N"));
    private readonly List<(string Program, string[] Args)> _runs = new();
    private readonly Dictionary<string, string> _onPath = new(StringComparer.OrdinalIgnoreCase);
    private static readonly SetupProduct Product = SetupProduct.Standalone;
    private static readonly string N = Product.DefaultServerName;
    private static readonly ServerEntry Entry = new(N, @"C:\Tools\wc3ctl\Wc3.Mcp.exe", Array.Empty<string>(), null);

    public SetupTests() => Directory.CreateDirectory(_root);

    public void Dispose()
    {
        try { Directory.Delete(_root, recursive: true); } catch (IOException) { }
    }

    private string Home => Path.Combine(_root, "home");
    private string AppData => Path.Combine(_root, "appdata");

    private SetupEnvironment Env => new(
        Home, AppData, Path.Combine(_root, "local"), Path.Combine(Home, ".codex"),
        name => _onPath.TryGetValue(name, out var p) ? p : null,
        (program, args) => { _runs.Add((program, args.ToArray())); return (0, ""); },
        () => _userPath,
        value => _userPath = value);

    private string? _userPath = @"C:\Existing;C:\Other";

    private (int Code, string Output) Cli(params string[] args) => Cli(Product, args);

    private (int Code, string Output) Cli(SetupProduct product, params string[] args)
    {
        var sw = new StringWriter();
        int code = SetupCli.Run(args, Env, sw, product, Entry);
        return (code, sw.ToString());
    }

    private static string Write(string path, string text)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, text);
        return path;
    }

    [Fact]
    public void Install_adds_the_server_and_keeps_everything_else_in_the_file()
    {
        var file = Write(Path.Combine(Home, ".cursor", "mcp.json"),
            """{ "theme": "dark", "mcpServers": { "other": { "command": "other.exe" } } }""");

        var (code, output) = Cli("install", "cursor");

        Assert.Equal(0, code);
        Assert.Contains("added", output);
        var root = JsonNode.Parse(File.ReadAllText(file))!;
        Assert.Equal("dark", (string?)root["theme"]);
        Assert.Equal("other.exe", (string?)root["mcpServers"]!["other"]!["command"]);
        Assert.Equal(Entry.Command, (string?)root["mcpServers"]![N]!["command"]);
        Assert.Equal("stdio", (string?)root["mcpServers"]![N]!["type"]);
        Assert.True(File.Exists(file + ConfigFile.BackupSuffix), "the original was not backed up");
    }

    [Fact]
    public void Installing_twice_changes_nothing_the_second_time()
    {
        Directory.CreateDirectory(Path.Combine(Home, ".cursor"));
        Cli("install", "cursor");
        var file = Path.Combine(Home, ".cursor", "mcp.json");
        var first = File.ReadAllText(file);

        var (code, output) = Cli("install", "cursor");

        Assert.Equal(0, code);
        Assert.Contains("already set up", output);
        Assert.Equal(first, File.ReadAllText(file));
    }

    [Fact]
    public void A_file_with_comments_is_left_byte_for_byte_and_the_snippet_is_printed()
    {
        var text = "{\n  // my servers\n  \"context_servers\": {}\n}\n";
        var file = Write(Path.Combine(AppData, "Zed", "settings.json"), text);

        var (code, output) = Cli("install", "zed");

        Assert.Equal(1, code);
        Assert.Equal(text, File.ReadAllText(file));
        Assert.False(File.Exists(file + ConfigFile.BackupSuffix));
        Assert.Contains("comments", output);
        Assert.Contains("\"context_servers\"", output);
        Assert.Contains(Entry.Command.Replace(@"\", @"\\"), output);
    }

    [Fact]
    public void A_broken_json_file_is_left_alone()
    {
        var file = Write(Path.Combine(Home, ".cursor", "mcp.json"), "{ \"mcpServers\": ");

        var (code, output) = Cli("install", "cursor");

        Assert.Equal(1, code);
        Assert.Equal("{ \"mcpServers\": ", File.ReadAllText(file));
        Assert.Contains("not valid JSON", output);
    }

    [Theory]
    [InlineData("vscode", "servers", true)]
    [InlineData("claude-desktop", "mcpServers", false)]
    [InlineData("zed", "context_servers", false)]
    [InlineData("gemini", "mcpServers", false)]
    public void Each_client_gets_its_own_key_and_shape(string id, string key, bool hasType)
    {
        var target = ClientTargets.Find(id)!;
        var file = target.Candidates(Env)[0].ConfigFile;
        Directory.CreateDirectory(target.Candidates(Env)[0].MarkerDir);

        Assert.Equal(0, Cli("install", id).Code);

        var server = JsonNode.Parse(File.ReadAllText(file))![key]![N]!;
        Assert.Equal(Entry.Command, (string?)server["command"]);
        Assert.Equal(hasType, server["type"] is not null);
    }

    [Fact]
    public void Uninstall_removes_only_this_server()
    {
        var file = Write(Path.Combine(Home, ".cursor", "mcp.json"),
            """{ "mcpServers": { "other": { "command": "other.exe" } } }""");
        Cli("install", "cursor");

        var (code, _) = Cli("uninstall", "cursor");

        Assert.Equal(0, code);
        var servers = JsonNode.Parse(File.ReadAllText(file))!["mcpServers"]!.AsObject();
        Assert.False(servers.ContainsKey(N));
        Assert.True(servers.ContainsKey("other"));
    }

    [Fact]
    public void Codex_toml_keeps_comments_and_other_servers_and_never_duplicates_the_table()
    {
        var file = Write(Path.Combine(Home, ".codex", "config.toml"),
            "# my settings\nmodel = \"o3\"\n\n[mcp_servers.other]\ncommand = \"other\"\n");

        Assert.Equal(0, Cli("install", "codex", "--game-dir", _root).Code);
        Assert.Equal(0, Cli("install", "codex").Code);
        var text = File.ReadAllText(file);

        Assert.Contains("# my settings", text);
        Assert.Contains("[mcp_servers.other]", text);
        Assert.Single(text.Split('\n'), l => l.Trim() == $"[mcp_servers.{N}]");
        Assert.Contains($"command = '{Entry.Command}'", text);
        // The second install had no --game-dir, so the env table from the first is gone.
        Assert.DoesNotContain($"[mcp_servers.{N}.env]", text);

        Assert.Equal(0, Cli("uninstall", "codex").Code);
        text = File.ReadAllText(file);
        Assert.DoesNotContain($"mcp_servers.{N}", text);
        Assert.Contains("[mcp_servers.other]", text);
        Assert.Contains("model = \"o3\"", text);
    }

    [Fact]
    public void Install_all_touches_only_clients_that_are_present()
    {
        Directory.CreateDirectory(Path.Combine(Home, ".cursor"));

        var (code, output) = Cli("install", "--all");

        Assert.Equal(0, code);
        Assert.Contains("Cursor", output);
        Assert.True(File.Exists(Path.Combine(Home, ".cursor", "mcp.json")));
        Assert.False(Directory.Exists(Path.Combine(AppData, "Code")));
        Assert.False(Directory.Exists(Path.Combine(Home, ".gemini")));
        Assert.Empty(_runs);
    }

    [Fact]
    public void Install_all_with_no_client_present_says_so_and_writes_nothing()
    {
        var (code, output) = Cli("install", "--all");

        Assert.Equal(1, code);
        Assert.Contains("No supported MCP client", output);
        Assert.False(Directory.Exists(Home));
    }

    [Fact]
    public void Claude_code_is_set_up_through_its_own_cli_at_user_scope()
    {
        _onPath["claude"] = @"C:\bin\claude.exe";

        var (code, _) = Cli("install", "claude-code", "--game-dir", _root);

        Assert.Equal(0, code);
        Assert.Equal(2, _runs.Count);
        Assert.Equal(new[] { "mcp", "remove", N, "--scope", "user" }, _runs[0].Args);
        var add = _runs[1].Args;
        Assert.Equal(new[] { "mcp", "add", "--transport", "stdio", "--scope", "user" }, add.Take(6));
        Assert.Contains($"WC3_GAME_DIR={Path.GetFullPath(_root)}", add);
        Assert.Equal(new[] { N, "--", Entry.Command }, add.Skip(add.Length - 3));
        Assert.False(File.Exists(Path.Combine(Home, ".claude.json")), "the Claude Code state file must never be edited directly");
    }

    [Fact]
    public void Game_dir_option_becomes_the_env_variable()
    {
        Directory.CreateDirectory(Path.Combine(Home, ".cursor"));

        Cli("install", "cursor", "--game-dir", _root);

        var env = JsonNode.Parse(File.ReadAllText(Path.Combine(Home, ".cursor", "mcp.json")))!["mcpServers"]![N]!["env"]!;
        Assert.Equal(Path.GetFullPath(_root), (string?)env[Wc3.Mcp.Wc3Tools.GameDirVariable]);
    }

    [Fact]
    public void Unknown_client_and_unknown_option_are_usage_errors()
    {
        Assert.Equal(2, Cli("install", "notepad").Code);
        Assert.Equal(2, Cli("install", "cursor", "--bogus").Code);
        Assert.Equal(2, Cli("install").Code);
    }

    [Fact]
    public void Every_documented_client_id_resolves()
    {
        foreach (var id in new[] { "claude-code", "claude-desktop", "cursor", "vscode", "windsurf", "gemini", "codex", "cline", "lmstudio", "zed" })
            Assert.NotNull(ClientTargets.Find(id));
        Assert.Equal(10, ClientTargets.All.Count);
    }

    [Fact]
    public void Install_puts_the_exe_folder_on_the_user_path_once_and_uninstall_all_takes_it_off()
    {
        Directory.CreateDirectory(Path.Combine(Home, ".cursor"));
        var folder = Path.GetDirectoryName(Entry.Command)!;

        var (_, output) = Cli("install", "cursor");
        Assert.Contains("PATH", output);
        Assert.Equal(@"C:\Existing;C:\Other;" + folder, _userPath);

        Cli("install", "cursor");
        Assert.Equal(1, _userPath!.Split(';').Count(p => p == folder));

        Cli("uninstall", "--all");
        Assert.Equal(@"C:\Existing;C:\Other", _userPath);
    }

    [Fact]
    public void Path_entries_written_with_variables_stay_unexpanded()
    {
        _userPath = @"%USERPROFILE%\go\bin;%SystemRoot%\system32";
        Directory.CreateDirectory(Path.Combine(Home, ".cursor"));
        var folder = Path.GetDirectoryName(Entry.Command)!;

        Cli("install", "cursor");
        Assert.Equal(@"%USERPROFILE%\go\bin;%SystemRoot%\system32;" + folder, _userPath);
        Cli("uninstall", "--all");
        Assert.Equal(@"%USERPROFILE%\go\bin;%SystemRoot%\system32", _userPath);
    }

    [Fact]
    public void A_folder_already_on_path_through_a_variable_is_not_added_twice()
    {
        var profile = Environment.GetEnvironmentVariable("USERPROFILE")!;
        Assert.True(SetupEnvironment.PathContains(@"C:\x;%USERPROFILE%\Tools\", Path.Combine(profile, "Tools")));
        Assert.False(SetupEnvironment.PathContains(@"C:\x", Path.Combine(profile, "Tools")));
    }

    [Fact]
    public void No_path_leaves_the_user_path_alone()
    {
        Directory.CreateDirectory(Path.Combine(Home, ".cursor"));
        Cli("install", "cursor", "--no-path");
        Assert.Equal(@"C:\Existing;C:\Other", _userPath);
    }

    [Fact]
    public void The_cli_entry_starts_the_server_with_mcp_serve()
    {
        var entry = ServerEntry.ForThisProcess(N, null, SetupProduct.Wc3ctl.ServeArgs);
        Assert.Equal(new[] { "mcp", "serve" }, entry.Args.TakeLast(2));
        Assert.Empty(ServerEntry.ForThisProcess(N, null).Args.Where(a => a == "serve"));
    }

    [Fact]
    public void Help_and_version_name_the_command_that_was_typed()
    {
        var (code, help) = Cli(SetupProduct.Wc3ctl, "help");
        Assert.Equal(0, code);
        Assert.Contains("wc3ctl mcp install --all", help);
        Assert.Contains("'wc3ctl mcp serve' is the MCP server", help);
        Assert.Contains(ConfigFile.BackupSuffix, help);
        // No hard-coded product name. The backup suffix is the build's own (wc3-mcp in the public build).
        Assert.DoesNotContain("wc3-mcp", help.Replace(ConfigFile.BackupSuffix, ""));

        Assert.Contains("Run with no arguments, it is the MCP server", Cli("help").Output);
        Assert.StartsWith("wc3ctl mcp ", Cli(SetupProduct.Wc3ctl, "version").Output);
        Assert.Equal(Cli(SetupProduct.Wc3ctl, "help").Output, Cli(SetupProduct.Wc3ctl).Output);
    }
}
