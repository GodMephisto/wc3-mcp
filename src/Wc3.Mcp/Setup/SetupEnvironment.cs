// src/Wc3.Mcp/Setup/SetupEnvironment.cs
using System.Diagnostics;

namespace Wc3.Mcp.Setup;

/// <summary>
/// Where the setup commands look and how they run other programs. <see cref="Current"/> is the real
/// machine. Tests build one over a temp folder so no real client config is ever touched.
/// </summary>
public sealed record SetupEnvironment(
    string Home,
    string AppData,
    string LocalAppData,
    string CodexHome,
    Func<string, string?> FindOnPath,
    Func<string, IReadOnlyList<string>, (int ExitCode, string Output)> Run,
    Func<string?> GetUserPath,
    Action<string> SetUserPath)
{
    /// <summary>
    /// The real user profile, PATH and process runner. USERPROFILE, APPDATA and LOCALAPPDATA are
    /// honoured when set, as most tools do, which also lets the exe be run against a scratch profile.
    /// </summary>
    public static SetupEnvironment Current
    {
        get
        {
            static string Folder(string variable, Environment.SpecialFolder fallback) =>
                Environment.GetEnvironmentVariable(variable) is { Length: > 0 } v ? v : Environment.GetFolderPath(fallback);
            var home = Folder("USERPROFILE", Environment.SpecialFolder.UserProfile);
            var codex = Environment.GetEnvironmentVariable("CODEX_HOME");
            return new SetupEnvironment(
                home,
                Folder("APPDATA", Environment.SpecialFolder.ApplicationData),
                Folder("LOCALAPPDATA", Environment.SpecialFolder.LocalApplicationData),
                string.IsNullOrWhiteSpace(codex) ? Path.Combine(home, ".codex") : codex,
                SearchPath,
                RunProcess,
                ReadUserPath,
                WriteUserPath);
        }
    }

    // The user PATH is read and written raw. Environment.GetEnvironmentVariable(.., User) expands
    // %USERPROFILE% and friends, and writing that back turned every such entry into a fixed path
    // and the value from REG_EXPAND_SZ into REG_SZ. Measured on this machine on 2026-10-03.
    private const string EnvironmentKey = "Environment";

    private static string? ReadUserPath()
    {
        if (!OperatingSystem.IsWindows()) return Environment.GetEnvironmentVariable("PATH");
        using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(EnvironmentKey);
        return key?.GetValue("Path", null, Microsoft.Win32.RegistryValueOptions.DoNotExpandEnvironmentNames) as string;
    }

    private static void WriteUserPath(string value)
    {
        if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException("the user PATH is only edited on Windows");
        using (var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(EnvironmentKey))
        {
            var kind = key.GetValueNames().Contains("Path", StringComparer.OrdinalIgnoreCase)
                ? key.GetValueKind("Path")
                : Microsoft.Win32.RegistryValueKind.ExpandString;
            if (kind != Microsoft.Win32.RegistryValueKind.String) kind = Microsoft.Win32.RegistryValueKind.ExpandString;
            key.SetValue("Path", value, kind);
        }
        // Tell Explorer the environment changed, so terminals opened from now on see the new PATH.
        SendMessageTimeout(new IntPtr(0xFFFF), 0x001A, UIntPtr.Zero, EnvironmentKey, 0x0002, 5000, out _);
    }

    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    private static extern IntPtr SendMessageTimeout(
        IntPtr hWnd, uint msg, UIntPtr wParam, string lParam, uint flags, uint timeout, out UIntPtr result);

    /// <summary>
    /// Whether a folder is already one of the PATH entries. Entries are compared after expanding
    /// %VARIABLES%, ignoring case and a trailing slash, so %USERPROFILE%\x matches C:\Users\me\x.
    /// </summary>
    public static bool PathContains(string? path, string folder) =>
        (path ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries).Any(p => SameFolder(p, folder));

    /// <summary>Two PATH entries naming the same folder.</summary>
    public static bool SameFolder(string entry, string folder) =>
        string.Equals(
            Environment.ExpandEnvironmentVariables(entry.Trim()).TrimEnd('\\', '/'),
            Environment.ExpandEnvironmentVariables(folder.Trim()).TrimEnd('\\', '/'),
            StringComparison.OrdinalIgnoreCase);

    /// <summary>Finds a program on PATH the way a shell would, trying each PATHEXT extension.</summary>
    public static string? SearchPath(string name)
    {
        var exts = (Environment.GetEnvironmentVariable("PATHEXT") ?? ".COM;.EXE;.BAT;.CMD")
            .Split(';', StringSplitOptions.RemoveEmptyEntries);
        foreach (var dir in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
        {
            foreach (var ext in exts)
            {
                string candidate;
                try { candidate = Path.Combine(dir.Trim('"'), name + ext.ToLowerInvariant()); }
                catch (ArgumentException) { continue; }
                if (File.Exists(candidate)) return candidate;
            }
        }
        return null;
    }

    /// <summary>
    /// Runs a program and returns its exit code with stdout and stderr together. A .cmd or .bat shim
    /// (how npm installs CLIs) is run through cmd.exe, since it cannot be started directly.
    /// </summary>
    public static (int, string) RunProcess(string program, IReadOnlyList<string> args)
    {
        // stdin is redirected and closed at once, because some CLIs wait on it when it stays attached.
        var psi = new ProcessStartInfo
        {
            RedirectStandardInput = true, RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false,
        };
        var ext = Path.GetExtension(program);
        if (ext.Equals(".cmd", StringComparison.OrdinalIgnoreCase) || ext.Equals(".bat", StringComparison.OrdinalIgnoreCase))
        {
            psi.FileName = Environment.GetEnvironmentVariable("ComSpec") ?? "cmd.exe";
            // /s strips the outer pair of quotes, leaving each quoted token intact.
            psi.Arguments = "/d /s /c \"" + string.Join(" ", new[] { program }.Concat(args).Select(Quote)) + "\"";
        }
        else
        {
            psi.FileName = program;
            foreach (var a in args) psi.ArgumentList.Add(a);
        }
        using var p = Process.Start(psi) ?? throw new InvalidOperationException($"could not start {program}");
        p.StandardInput.Close();
        var stdout = p.StandardOutput.ReadToEndAsync();
        var stderr = p.StandardError.ReadToEndAsync();
        if (!p.WaitForExit(60_000))
        {
            try { p.Kill(entireProcessTree: true); } catch { /* already gone */ }
            return (-1, $"{program} did not finish within 60 s");
        }
        return (p.ExitCode, (stdout.Result + stderr.Result).Trim());
    }

    private static string Quote(string s) => "\"" + s.Replace("\"", "\\\"") + "\"";
}
