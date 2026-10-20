// src/Wc3.Commands/TestLoadCommand.cs
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Wc3.Model;

namespace Wc3.Commands;

public sealed record TestLoadResult(
    string Verdict,
    string Detail,
    double SecondsElapsed,
    double CpuCoresBusyAtEnd,
    string TestMapPath,
    string MarkerPath,
    IReadOnlyList<IpAddress> HotAddresses);

/// <summary>
/// Loads a map in Warcraft III unattended and reports whether it actually reached
/// <c>main()</c>, closing the feedback loop that made every earlier diagnosis depend on a human
/// launching a 250 MB map and describing what they saw.
///
/// The script may be <c>war3map.j</c> or <c>war3map.lua</c>. A marker written at the END of
/// <c>main()</c> proves initialisation completed. The instrumented copy is always a separate file.
///
/// A game started on its own with <c>-launch -loadfile</c> has no Battle.net session, and signing
/// the Battle.net app in does not give it one, so Reforged shows an email and password form every
/// run. <c>--via-launcher</c> has the app start the game the way its Play button does, with the map
/// in the app's own launch options, and the app hands the game its session. The map has to sit at
/// one fixed path so the stored arguments never change, and the app ignores the launch command
/// until it has finished starting, so the command is repeated. This mirrors the proven handoff in
/// BenqsXd/wc3-mcp (desktop/battlenet.py).
///
/// A custom loading screen can wait for a key before the game starts the map, so a space is sent
/// to the game window while the marker is awaited. Sending it a few times during loading is
/// harmless, and it stops as soon as the marker appears.
/// </summary>
public static class TestLoadCommand
{
    private static readonly string[] ExeRelativePaths =
    {
        Path.Combine("_retail_", "x86_64", "Warcraft III.exe"),
        Path.Combine("x86_64", "Warcraft III.exe"),
        "Warcraft III.exe",
    };

    private static readonly string[] BattleNetPaths =
    {
        @"C:\Program Files (x86)\Battle.net\Battle.net.exe",
        @"C:\Program Files\Battle.net\Battle.net.exe",
    };

    private const string AppName = "Battle.net.exe";
    private const string GameName = "Warcraft III";
    private const double LaunchTimeout = 180;

    public static TestLoadResult Run(string mapPath, string gameDir, string documentsDir,
        double timeoutSeconds = 120, string? runId = null, bool viaLauncher = false,
        string? battleNetExe = null)
    {
        var exe = ExeRelativePaths.Select(p => Path.Combine(gameDir, p)).FirstOrDefault(File.Exists)
            ?? throw new FileNotFoundException($"Warcraft III.exe not found under '{gameDir}'.");

        runId ??= Path.GetFileNameWithoutExtension(mapPath).Replace(' ', '_');
        var markerRelative = Path.Combine("wc3ctl", runId + ".txt");
        var markerFull = Path.Combine(documentsDir, "CustomMapData", markerRelative);
        Directory.CreateDirectory(Path.GetDirectoryName(markerFull)!);
        if (File.Exists(markerFull)) File.Delete(markerFull);

        var home = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "wc3ctl");
        Directory.CreateDirectory(home);

        // The app's stored arguments must never change between runs, so the map always lands on
        // this one path.
        var launchCopy = Path.Combine(home, "launch", "map" + Path.GetExtension(mapPath));
        Directory.CreateDirectory(Path.GetDirectoryName(launchCopy)!);

        var testMap = viaLauncher
            ? launchCopy
            : Path.Combine(documentsDir, "Maps", "Test", "WorldEditTestMap" + Path.GetExtension(mapPath));
        Directory.CreateDirectory(Path.GetDirectoryName(testMap)!);

        var doc = MapDocument.Load(mapPath);
        var script = doc.GetFile("war3map.lua") ?? doc.GetFile("scripts\\war3map.lua")
            ?? doc.GetFile("war3map.j") ?? doc.GetFile("scripts\\war3map.j")
            ?? throw new InvalidOperationException("Map has no war3map.j or war3map.lua to instrument.");
        var text = ScriptText.GetString(script.CurrentBytes);
        FileEditCommand.WriteText(doc, script.FileName!, InjectMarker(text, markerRelative));
        doc.Save(testMap);

        var sw = Stopwatch.StartNew();
        Process? proc = null;
        bool appRoute = viaLauncher;
        string? bnet = null;
        string? configPath = null;
        string? previousArgs = null;
        bool appWasRunning = false;
        bool mustRestore = false;
        int? gamePid = null;

        try
        {
            if (appRoute)
            {
                bnet = battleNetExe
                    ?? Environment.GetEnvironmentVariable("WC3CTL_BATTLENET")
                    ?? BattleNetPaths.FirstOrDefault(File.Exists)
                    ?? throw new FileNotFoundException("Battle.net.exe not found; pass --battlenet.");
                configPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Battle.net", "Battle.net.config");
                var wanted = $"-loadfile \"{launchCopy}\" -windowmode windowed -nowfpause";
                var stored = StoredArgs(configPath);
                var kept = Path.Combine(home, "battlenet-previous-launch-arguments.txt");
                appWasRunning = Process.GetProcessesByName("Battle.net").Length > 0;
                if (stored != wanted)
                {
                    // Keep the user's own arguments aside, then restart the app so it reads them.
                    File.WriteAllText(kept, stored);
                    previousArgs = stored;
                    mustRestore = true;
                    ShutdownApp();
                    SetLaunchArgs(configPath, wanted);
                    StartApp(bnet);
                    MinimizeAppWindows();
                }
                else if (File.Exists(kept))
                {
                    // A previous run was interrupted and left the app pointing at the launch copy.
                    previousArgs = File.ReadAllText(kept);
                    mustRestore = true;
                }

                gamePid = LaunchViaApp(bnet, sw, timeoutSeconds);
                if (gamePid is null)
                    return new("EXITED",
                        $"the Battle.net app did not start Warcraft III within {LaunchTimeout:0}s. "
                        + "Ask the user to open the app and log in once with keep me logged in.",
                        Math.Round(sw.Elapsed.TotalSeconds, 1), 0, testMap, markerFull,
                        Array.Empty<IpAddress>());

                // A custom loading screen can wait for a key. Send space periodically until the
                // marker appears, but never after it.
                var deadline = DateTime.UtcNow.AddSeconds(Math.Max(5, timeoutSeconds));
                int keys = 0;
                while (DateTime.UtcNow < deadline)
                {
                    if (File.Exists(markerFull))
                        return new("LOADED",
                            $"the map reached the end of main() after {sw.Elapsed.TotalSeconds:0.0}s",
                            Math.Round(sw.Elapsed.TotalSeconds, 1), 0, testMap, markerFull,
                            Array.Empty<IpAddress>());
                    if (keys < 30)
                    {
                        MinimizeWindow(gamePid.Value);
                        SendSpace(gamePid.Value);
                        keys++;
                    }
                    Thread.Sleep(1000);
                }

                var sample = GameHangCommand.Sample(seconds: 4, topThreads: 3, ipSamples: 60);
                bool up = Process.GetProcessesByName(GameName).Length > 0;
                string verdict = !up ? "EXITED"
                    : sample.CpuCoresBusy >= 0.75 ? "HANG_SPINNING" : "HANG_WAITING";
                string detail = verdict == "EXITED"
                    ? $"no Warcraft III process and no marker after {timeoutSeconds:0}s; the game closed "
                      + "or crashed, check the Warcraft III Errors folder"
                    : verdict == "HANG_SPINNING"
                        ? $"no marker after {timeoutSeconds:0}s and {sample.CpuCoresBusy:0.00} core(s) pinned"
                        : $"no marker after {timeoutSeconds:0}s at {sample.CpuCoresBusy:0.00} core(s)";
                return new(verdict, detail, Math.Round(sw.Elapsed.TotalSeconds, 1),
                    sample.CpuCoresBusy, testMap, markerFull, sample.HotAddresses);
            }

            // Direct launch. Login-free only when a cached session already exists.
            var psi = new ProcessStartInfo(exe)
            {
                UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(exe)!,
            };
            foreach (var a in new[] { "-launch", "-window", "-nowfpause", "-loadfile", testMap })
                psi.ArgumentList.Add(a);
            proc = Process.Start(psi)
                ?? throw new InvalidOperationException("Could not start Warcraft III.");

            while (sw.Elapsed.TotalSeconds < timeoutSeconds)
            {
                if (File.Exists(markerFull))
                    return new("LOADED",
                        $"the map reached the end of main() after {sw.Elapsed.TotalSeconds:0.0}s",
                        Math.Round(sw.Elapsed.TotalSeconds, 1), 0, testMap, markerFull,
                        Array.Empty<IpAddress>());
                if (proc.HasExited)
                    return new("EXITED",
                        $"the game exited after {sw.Elapsed.TotalSeconds:0.0}s without writing the "
                        + "marker; check the Warcraft III Errors folder for a crash dump",
                        Math.Round(sw.Elapsed.TotalSeconds, 1), 0, testMap, markerFull,
                        Array.Empty<IpAddress>());
                Thread.Sleep(500);
            }
            var s2 = GameHangCommand.Sample(seconds: 4, topThreads: 3, ipSamples: 60);
            return new(s2.CpuCoresBusy >= 0.75 ? "HANG_SPINNING" : "HANG_WAITING",
                $"no marker after {timeoutSeconds:0}s at {s2.CpuCoresBusy:0.00} core(s)",
                Math.Round(sw.Elapsed.TotalSeconds, 1), s2.CpuCoresBusy, testMap, markerFull,
                s2.HotAddresses);
        }
        finally
        {
            try { if (!appRoute && proc is { HasExited: false }) proc.Kill(entireProcessTree: true); }
            catch { }

            if (appRoute)
            {
                // Kill the game this run started, then put the app back exactly as it was.
                try
                {
                    if (gamePid is int pid)
                    {
                        var gp = Process.GetProcessById(pid);
                        if (!gp.HasExited) gp.Kill(entireProcessTree: true);
                    }
                }
                catch { }
                try
                {
                    if (bnet is not null && configPath is not null && mustRestore && previousArgs is not null)
                    {
                        ShutdownApp();
                        if (previousArgs.Length == 0) RemoveLaunchArgs(configPath);
                        else SetLaunchArgs(configPath, previousArgs);
                        if (appWasRunning) { StartApp(bnet); MinimizeAppWindows(); }
                        File.Delete(Path.Combine(home, "battlenet-previous-launch-arguments.txt"));
                    }
                }
                catch { }
                try { File.Delete(launchCopy); } catch { }
            }
        }
    }

    private static int? LaunchViaApp(string bnet, Stopwatch sw, double timeoutSeconds)
    {
        var before = Process.GetProcessesByName(GameName).Select(p => p.Id).ToHashSet();
        double limit = Math.Min(LaunchTimeout, Math.Max(LaunchTimeout, timeoutSeconds));
        DateTime deadline = DateTime.UtcNow.AddSeconds(limit);
        DateTime nextAsk = DateTime.MinValue;
        while (DateTime.UtcNow < deadline)
        {
            if (DateTime.UtcNow >= nextAsk)
            {
                nextAsk = DateTime.UtcNow.AddSeconds(25);
                try
                {
                    Process.Start(new ProcessStartInfo(bnet)
                    {
                        UseShellExecute = false,
                        ArgumentList = { "--exec=launch W3" },
                    });
                }
                catch { }
            }
            foreach (var p in Process.GetProcessesByName(GameName))
                if (!before.Contains(p.Id)) return p.Id;
            Thread.Sleep(1000);
        }
        return null;
    }

    private static void ShutdownApp()
    {
        try
        {
            using var p = Process.Start(new ProcessStartInfo("taskkill")
            {
                UseShellExecute = false,
                ArgumentList = { "/IM", AppName, "/F" },
            });
            p?.WaitForExit(5000);
        }
        catch { }
        for (int i = 0; i < 20; i++)
        {
            if (Process.GetProcessesByName("Battle.net").Length == 0) return;
            Thread.Sleep(1000);
        }
    }

    private static void StartApp(string bnet)
    {
        try { Process.Start(new ProcessStartInfo(bnet) { UseShellExecute = true }); } catch { }
        for (int i = 0; i < 30 && Process.GetProcessesByName("Battle.net").Length == 0; i++)
            Thread.Sleep(1000);
    }

    private static string StoredArgs(string configPath)
    {
        try
        {
            var node = JsonNode.Parse(File.ReadAllText(configPath));
            return node?["Games"]?["w3"]?["AdditionalLaunchArguments"]?.GetValue<string>() ?? "";
        }
        catch { return ""; }
    }

    private static void SetLaunchArgs(string configPath, string value)
    {
        var node = JsonNode.Parse(File.ReadAllText(configPath)) as JsonObject ?? new JsonObject();
        if (node["Games"] is not JsonObject games) { games = new JsonObject(); node["Games"] = games; }
        if (games["w3"] is not JsonObject w3) { w3 = new JsonObject(); games["w3"] = w3; }
        w3["AdditionalLaunchArguments"] = value;
        File.WriteAllText(configPath, node.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
    }

    private static void RemoveLaunchArgs(string configPath)
    {
        if (!File.Exists(configPath)) return;
        if (JsonNode.Parse(File.ReadAllText(configPath)) is not JsonObject node) return;
        if (node["Games"] is JsonObject games && games["w3"] is JsonObject w3)
        {
            w3.Remove("AdditionalLaunchArguments");
            File.WriteAllText(configPath, node.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        }
    }

    private static void SendSpace(int pid)
    {
        try
        {
            var p = Process.GetProcessById(pid);
            p.Refresh();
            var h = p.MainWindowHandle;
            if (h == IntPtr.Zero) return;
            // Post to the game window. Never call SetForegroundWindow or keybd_event here: those
            // take focus away from whatever the user is doing, which made an unattended run
            // impossible to work through.
            PostMessage(h, 0x0100, (IntPtr)0x20, IntPtr.Zero);
            PostMessage(h, 0x0101, (IntPtr)0x20, IntPtr.Zero);
        }
        catch { }
    }

    private static void MinimizeWindow(int pid)
    {
        try
        {
            var p = Process.GetProcessById(pid);
            p.Refresh();
            if (p.MainWindowHandle != IntPtr.Zero) ShowWindow(p.MainWindowHandle, 6);
        }
        catch { }
    }

    private static void MinimizeAppWindows()
    {
        try
        {
            foreach (var p in Process.GetProcessesByName("Battle.net"))
            {
                p.Refresh();
                if (p.MainWindowHandle != IntPtr.Zero) ShowWindow(p.MainWindowHandle, 6);
            }
        }
        catch { }
    }

    [DllImport("user32.dll")]
    private static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    /// <summary>
    /// Writes the marker at the END of <c>main()</c>, so its presence proves initialisation
    /// completed rather than merely started. Handles both a JASS <c>function main takes</c> ...
    /// <c>endfunction</c> and a Lua <c>function main()</c> ... <c>end</c>.
    /// </summary>
    internal static string InjectMarker(string script, string markerRelativePath)
    {
        var lines = script.Split('\n');
        int mainStart = -1;
        bool lua = false;
        for (int i = 0; i < lines.Length; i++)
        {
            var trimmed = lines[i].TrimEnd('\r', ' ');
            if (trimmed == "function main()" || trimmed == "function main ()")
            { mainStart = i; lua = true; break; }
            if (trimmed.StartsWith("function main ", StringComparison.Ordinal)
                || trimmed == "function main takes nothing returns nothing")
            { mainStart = i; lua = false; break; }
        }
        if (mainStart < 0)
            throw new InvalidOperationException("Could not find 'function main' to instrument.");

        int mainEnd = -1;
        if (lua)
        {
            for (int i = mainStart + 1; i < lines.Length; i++)
                if (lines[i].TrimEnd('\r', ' ') == "end") { mainEnd = i; break; }
            if (mainEnd < 0)
                throw new InvalidOperationException("'function main' has no end.");
        }
        else
        {
            for (int i = mainStart + 1; i < lines.Length; i++)
                if (lines[i].StartsWith("endfunction", StringComparison.Ordinal)) { mainEnd = i; break; }
            if (mainEnd < 0)
                throw new InvalidOperationException("'function main' has no endfunction.");
        }

        var esc = markerRelativePath.Replace("\\", "\\\\");
        var probe = lua
            ? new[]
            {
                "  PreloadGenClear()",
                "  PreloadGenStart()",
                "  Preload(\"wc3ctl test-load marker\")",
                $"  PreloadGenEnd(\"{esc}\")",
            }
            : new[]
            {
                "    call PreloadGenClear()",
                "    call PreloadGenStart()",
                "    call Preload(\"wc3ctl test-load marker\")",
                $"    call PreloadGenEnd(\"{esc}\")",
            };
        return string.Join("\n", lines.Take(mainEnd).Concat(probe).Concat(lines.Skip(mainEnd)));
    }
}
