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
    Func<string, IReadOnlyList<string>, (int ExitCode, string Output)> Run)
{
    /// <summary>The real user profile, PATH and process runner.</summary>
    public static SetupEnvironment Current
    {
        get
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var codex = Environment.GetEnvironmentVariable("CODEX_HOME");
            return new SetupEnvironment(
                home,
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                string.IsNullOrWhiteSpace(codex) ? Path.Combine(home, ".codex") : codex,
                SearchPath,
                RunProcess);
        }
    }

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
