// src/Wc3.Commands/TestLoadCommand.cs
using System.Diagnostics;
using System.Text;
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
/// Two mechanisms make this work, both verified on this install:
/// <list type="bullet">
/// <item>The game accepts <c>-launch -window -nowfpause -loadfile &lt;map&gt;</c> and will start a
/// second instance without Battle.net driving it.</item>
/// <item>A map can write a file from JASS via <c>PreloadGenEnd</c>, the same trick every save-code
/// system uses. A marker written at the END of <c>main()</c> proves initialisation completed.</item>
/// </list>
///
/// Marker present means loaded. Marker absent with a core pinned means an infinite loop, and the
/// instruction-pointer histogram says where. Marker absent while idle means blocked rather than
/// looping. Those are three different bugs that all look identical to a human watching a loading
/// screen.
///
/// The injected copy is always written beside the original and never overwrites it. Note that on
/// a map where ANY modification matters, a run should be compared against an injected copy of the
/// known-good original, never against the pristine file.
/// </summary>
public static class TestLoadCommand
{
    private static readonly string[] ExeRelativePaths =
    {
        Path.Combine("_retail_", "x86_64", "Warcraft III.exe"),
        Path.Combine("x86_64", "Warcraft III.exe"),
        "Warcraft III.exe",
    };

    public static TestLoadResult Run(string mapPath, string gameDir, string documentsDir,
        double timeoutSeconds = 120, string? runId = null)
    {
        var exe = ExeRelativePaths.Select(p => Path.Combine(gameDir, p)).FirstOrDefault(File.Exists)
            ?? throw new FileNotFoundException($"Warcraft III.exe not found under '{gameDir}'.");

        runId ??= Path.GetFileNameWithoutExtension(mapPath).Replace(' ', '_');
        // PreloadGenEnd's path is relative to <documents>\Warcraft III\CustomMapData, NOT to the
        // documents root. Prefixing "CustomMapData" here as well produced
        // CustomMapData\CustomMapData\... and made a successful load look like a hang, because the
        // harness polled a path the game never wrote. The launch was correct all along.
        var markerRelative = Path.Combine("wc3ctl", runId + ".txt");
        var markerFull = Path.Combine(documentsDir, "CustomMapData", markerRelative);
        Directory.CreateDirectory(Path.GetDirectoryName(markerFull)!);
        if (File.Exists(markerFull)) File.Delete(markerFull);

        // The World Editor does not pass an arbitrary path. Its own preferences say how:
        //   [Test Map]  Copy Location=WorldEditTestMap  Player Profile=WorldEdit
        // It copies the map to Maps\Test under the documents folder and names the profile.
        // Passing an absolute path instead merely starts the game and leaves it at the menu, which
        // is what an earlier guess at this produced.
        var testDir = Path.Combine(documentsDir, "Maps", "Test");
        Directory.CreateDirectory(testDir);
        var testMap = Path.Combine(testDir, "WorldEditTestMap" + Path.GetExtension(mapPath));
        var testMapRelative = Path.Combine("Maps", "Test", Path.GetFileName(testMap));

        var doc = MapDocument.Load(mapPath);
        var script = doc.GetFile("war3map.j") ?? doc.GetFile("scripts\\war3map.j")
            ?? throw new InvalidOperationException("Map has no war3map.j to instrument.");
        var text = ScriptText.GetString(script.CurrentBytes);
        FileEditCommand.WriteText(doc, script.FileName!, InjectMarker(text, markerRelative));
        doc.Save(testMap);

        var psi = new ProcessStartInfo(exe)
        {
            UseShellExecute = false,
            WorkingDirectory = Path.GetDirectoryName(exe)!,
        };
        foreach (var a in new[] { "-launch", "-uid", "w3", "-window", "-nowfpause",
                                  "-loadfile", testMapRelative, "-testmapprofile", "WorldEdit" })
            psi.ArgumentList.Add(a);

        var sw = Stopwatch.StartNew();
        using var proc = Process.Start(psi)
            ?? throw new InvalidOperationException("Could not start Warcraft III.");
        try
        {
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

            // Timed out. Whether it is spinning or blocked is the whole diagnosis, so measure it
            // rather than reporting a bare timeout.
            var sample = GameHangCommand.Sample(seconds: 4, topThreads: 3, ipSamples: 60);
            string verdict = sample.CpuCoresBusy >= 0.75 ? "HANG_SPINNING" : "HANG_WAITING";
            string detail = sample.CpuCoresBusy >= 0.75
                ? $"no marker after {timeoutSeconds:0}s and {sample.CpuCoresBusy:0.00} core(s) pinned: "
                  + "an infinite loop, see the hot addresses"
                : $"no marker after {timeoutSeconds:0}s at {sample.CpuCoresBusy:0.00} core(s): "
                  + "blocked rather than looping";
            return new(verdict, detail, Math.Round(sw.Elapsed.TotalSeconds, 1),
                sample.CpuCoresBusy, testMap, markerFull, sample.HotAddresses);
        }
        finally
        {
            // Always clean up the instance we started. Leaving a stray game running after an
            // unattended run would make the next run's process sampling meaningless.
            try { if (!proc.HasExited) { proc.Kill(entireProcessTree: true); proc.WaitForExit(5000); } }
            catch { }
        }
    }

    /// <summary>
    /// Writes the marker at the END of <c>main()</c>, so its presence proves initialisation
    /// completed rather than merely started.
    /// </summary>
    internal static string InjectMarker(string script, string markerRelativePath)
    {
        var lines = script.Split('\n');
        int mainStart = -1;
        for (int i = 0; i < lines.Length; i++)
            if (lines[i].StartsWith("function main ", StringComparison.Ordinal)
                || lines[i].TrimEnd('\r', ' ') == "function main takes nothing returns nothing")
            { mainStart = i; break; }
        if (mainStart < 0)
            throw new InvalidOperationException("Could not find 'function main' to instrument.");

        int mainEnd = -1;
        for (int i = mainStart + 1; i < lines.Length; i++)
            if (lines[i].StartsWith("endfunction", StringComparison.Ordinal)) { mainEnd = i; break; }
        if (mainEnd < 0)
            throw new InvalidOperationException("'function main' has no endfunction.");

        var esc = markerRelativePath.Replace("\\", "\\\\");
        var probe = new[]
        {
            "    call PreloadGenClear()",
            "    call PreloadGenStart()",
            "    call Preload(\"wc3ctl test-load marker\")",
            $"    call PreloadGenEnd(\"{esc}\")",
        };
        return string.Join("\n", lines.Take(mainEnd).Concat(probe).Concat(lines.Skip(mainEnd)));
    }
}
