// src/Wc3.Commands/TraceLoadCommand.cs
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One instrumented call site, and whether its marker was written.</summary>
public sealed record TraceStep(int Index, string Label, bool Reached);

/// <summary>
/// One instrumented function, with the time its entry marker and its exit marker were last written.
/// A frame is OPEN when the entry is newer than the exit, which is exactly the signature of a
/// function that was entered and never left. Comparing times rather than mere existence is what keeps
/// that verdict correct for a function called many times that only hangs on a later call, where both
/// marker files exist yet the entry is the newer write.
/// </summary>
public sealed record TraceFrame(int Index, string Function, DateTime? EnteredAt, DateTime? ExitedAt)
{
    public bool Entered => EnteredAt is not null;
    public bool Exited => ExitedAt is not null;
    public bool Open => EnteredAt is not null && (ExitedAt is null || ExitedAt < EnteredAt);
}

public sealed record TraceLoadResult(
    string Verdict,
    string Detail,
    int StepsInstrumented,
    int StepsReached,
    string? LastReached,
    string? FirstMissed,
    double SecondsElapsed,
    string TestMapPath,
    IReadOnlyList<TraceStep> Steps,
    int FunctionsInstrumented,
    int FunctionsEntered,
    int FunctionsSkippedByCap,
    int CallSitesSkippedByCap,
    int FunctionCap,
    int CallSiteCap,
    string? HangFunction,
    IReadOnlyList<string> OpenFrames,
    IReadOnlyList<TraceFrame> Frames);

/// <summary>
/// Records HOW FAR a map gets through initialisation, instead of only whether it finished.
///
/// A map that hangs on the loading screen gives a human nothing, no error, no log entry, no crash
/// dump, and Windows shows "Not Responding" whether the game is looping or waiting. Diagnosing that
/// from the outside consumed an entire session and produced nine dead theories.
///
/// The way in is that JASS can write a file (<c>PreloadGenEnd</c>, the same trick every save-code
/// system uses). So this drops NUMBERED markers through initialisation. Every marker that executed
/// leaves its own file behind, so the files present name exactly how far the script got, and the file
/// write times put those events in the order they really happened.
///
/// A marker per event rather than one accumulating log is deliberate, the preload buffer is cleared
/// on each flush, so per-event files are the only form that survives a hang mid-sequence.
///
/// Four things were learned the hard way and are the reason for the current shape.
///
/// 1. Marker index order is SOURCE order, not execution order. <c>config</c> is declared last but the
///    engine runs it FIRST. The first version reported "COMPLETED" because the highest-numbered
///    marker existed, and so declared victory the moment config finished, on a map that had not even
///    started main and went on to hang. Order now comes from the file write times.
/// 2. A marker after a call cannot tell "the call returned" from "the call never happened". Each
///    instrumented function now writes a marker on ENTRY and another before every exit, so a function
///    that was entered and never left is directly identifiable, which is the whole point.
/// 3. Completion is "main returned", not "all markers present". A call site inside a branch that was
///    not taken legitimately never runs, so demanding every marker would call a healthy map broken.
/// 4. The marker payload must be a string LITERAL. An attempt to stamp a sequence number into the
///    file with <c>Preload(I2S(counter))</c> produced marker files with no payload line at all on
///    Warcraft III 2.0.4, so every marker read back as unrecognised and a map that clearly ran was
///    reported as NOTHING_RAN. Existence of the file, plus its write time, is the whole signal now.
///
/// Known limitation. A marker clears the preload buffer, and a save-code map builds its save file in
/// that same buffer, so a marker that fires between the map's own PreloadGenStart and PreloadGenEnd
/// would truncate that save file. Init code does not write save codes, so this does not bite during
/// the load this command measures, but it is the reason the instrumented copy is a throwaway test map
/// and never the map the user keeps.
/// </summary>
public static class TraceLoadCommand
{
    /// <summary>
    /// Instrumentation is not free. Every marker is a synchronous file write inside the loading
    /// thread, and a 15MB script has already exhausted pjass once in this project, so coverage is
    /// capped and the caps are reported rather than silently swallowing the excess.
    /// </summary>
    public const int DefaultFunctionCap = 500;

    /// <summary>Ceiling on statement-level markers. See <see cref="DefaultFunctionCap"/>.</summary>
    public const int DefaultCallSiteCap = 1500;

    private static readonly string[] ExeRelativePaths =
    {
        Path.Combine("_retail_", "x86_64", "Warcraft III.exe"),
        Path.Combine("x86_64", "Warcraft III.exe"),
        "Warcraft III.exe",
    };

    /// <summary>
    /// Functions the engine or the generated preamble calls by name. These are the roots of the init
    /// graph, so they are selected first and are never the ones the cap drops.
    /// </summary>
    private static readonly string[] EngineEntryPoints =
    {
        "main", "config", "InitGlobals", "InitCustomTriggers", "RunInitializationTriggers",
        "InitCustomPlayerSlots", "InitCustomTeams", "InitAllyPriorities",
        "CreateAllUnits", "CreateAllItems", "CreateAllDestructables", "CreateRegions",
        "CreateNeutralPassiveBuildings", "CreateNeutralHostileBuildings",
        "CreatePlayerBuildings", "CreatePlayerUnits",
        "InitSounds", "InitUpgrades", "InitTechTree",
    };

    /// <summary>A statement worth marking, a call at the top level of an instrumented function.</summary>
    private static readonly Regex CallStatement =
        new(@"^\s*call\s+([A-Za-z_][A-Za-z0-9_]*)\s*\(", RegexOptions.Compiled);

    /// <summary>vJASS module initialisers reach main only as a string, never as a direct call.</summary>
    private static readonly Regex ExecuteFuncTarget =
        new(@"ExecuteFunc\s*\(\s*""([A-Za-z_][A-Za-z0-9_]*)""", RegexOptions.Compiled);

    /// <summary>A callback handed to a trigger, timer or enumeration, "function Foo".</summary>
    private static readonly Regex FunctionReference =
        new(@"\bfunction\s+([A-Za-z_][A-Za-z0-9_]*)", RegexOptions.Compiled);

    private static readonly Regex FirstToken =
        new(@"^\s*([A-Za-z_][A-Za-z0-9_]*)", RegexOptions.Compiled);

    private sealed record Plan(
        string Script,
        List<string> CallLabels,
        List<string> Functions,
        int FunctionsSkipped,
        int CallSitesSkipped,
        int FunctionCap,
        int CallSiteCap);

    public static TraceLoadResult Run(string mapPath, string gameDir, string documentsDir,
        double timeoutSeconds = 180, int functionCap = DefaultFunctionCap,
        int callSiteCap = DefaultCallSiteCap)
    {
        var exe = ExeRelativePaths.Select(p => Path.Combine(gameDir, p)).FirstOrDefault(File.Exists)
            ?? throw new FileNotFoundException($"Warcraft III.exe not found under '{gameDir}'.");

        var traceDir = Path.Combine(documentsDir, "CustomMapData", "wc3trace");
        if (Directory.Exists(traceDir)) Directory.Delete(traceDir, recursive: true);
        Directory.CreateDirectory(traceDir);

        var doc = MapDocument.Load(mapPath);
        var script = doc.GetFile("war3map.j") ?? doc.GetFile("scripts\\war3map.j")
            ?? throw new InvalidOperationException("Map has no war3map.j to instrument.");
        // Latin-1 so a script that is not valid UTF-8 round-trips unchanged. A UTF-8 round-trip
        // replaced WOS2's high bytes with '?' and broke the map, so this is not a free choice.
        var text = Encoding.Latin1.GetString(script.CurrentBytes);
        var plan = Instrument(text, functionCap, callSiteCap);
        FileEditCommand.AddOrReplace(doc, script.FileName!, Encoding.Latin1.GetBytes(plan.Script));

        var testDir = Path.Combine(documentsDir, "Maps", "Test");
        Directory.CreateDirectory(testDir);
        var testMap = Path.Combine(testDir, "WorldEditTestMap" + Path.GetExtension(mapPath));
        doc.Save(testMap);

        var psi = new ProcessStartInfo(exe) { UseShellExecute = false, WorkingDirectory = Path.GetDirectoryName(exe)! };
        foreach (var a in new[] { "-launch", "-uid", "w3", "-window", "-nowfpause",
                                  "-loadfile", Path.Combine("Maps", "Test", Path.GetFileName(testMap)),
                                  "-testmapprofile", "WorldEdit" })
            psi.ArgumentList.Add(a);

        int mainIndex = plan.Functions.IndexOf("main");
        var launchedAt = DateTime.Now;
        var sw = Stopwatch.StartNew();
        using var proc = Process.Start(psi) ?? throw new InvalidOperationException("Could not start Warcraft III.");
        try
        {
            // Silence means two different things, and treating them alike is what let the first
            // version stop early. Before main runs, the engine is still unpacking terrain, object
            // data and imports, and a large map is legitimately quiet for a long time. Once main is
            // running the script itself is driving, so a short silence already means it stopped.
            double preMainStall = Math.Clamp(timeoutSeconds * 0.35, 30, 90);
            const double runningStall = 20;
            const double finishedQuiet = 8;

            bool gameExited = false;
            int markerCount = -1;
            double quietSince = 0;
            while (sw.Elapsed.TotalSeconds < timeoutSeconds)
            {
                if (proc.HasExited) { gameExited = true; break; }
                Thread.Sleep(500);
                var snap = ReadMarkers(traceDir);
                if (snap.Count != markerCount) { markerCount = snap.Count; quietSince = sw.Elapsed.TotalSeconds; }
                double quiet = sw.Elapsed.TotalSeconds - quietSince;

                var mainFrame = Frame(snap, plan.Functions, mainIndex);
                if (mainFrame is { Entered: true, Open: false } && quiet > finishedQuiet) break;
                if (mainFrame is { Open: true } && quiet > runningStall) break;
                if (mainFrame is null or { Entered: false } && snap.Count > 0 && quiet > preMainStall) break;
            }

            // A periodic callback caught between its own entry and exit marker looks exactly like a
            // hang for one instant. Two reads a moment apart tell them apart, a real hang keeps the
            // same innermost frame with the same entry time, a callback in flight does not.
            var firstRead = ReadMarkers(traceDir);
            var openFirst = OpenFrames(firstRead, plan.Functions);
            Thread.Sleep(900);
            var snapshot = ReadMarkers(traceDir);
            var open = OpenFrames(snapshot, plan.Functions);
            bool openStable = open.Count > 0 && openFirst.Count > 0
                && open[0].Index == openFirst[0].Index && open[0].EnteredAt == openFirst[0].EnteredAt;

            return Report(plan, snapshot, open, openStable, mainIndex, gameExited,
                Math.Round(sw.Elapsed.TotalSeconds, 1), testMap);
        }
        finally
        {
            try { if (!proc.HasExited) { proc.Kill(entireProcessTree: true); proc.WaitForExit(5000); } }
            catch { }
            CloseStrays(exe, launchedAt);
        }
    }

    /// <summary>
    /// Kills any game process this run left behind. Killing the started process is not enough, the
    /// launcher re-spawns the game and the child outlives the tree kill, and a surviving instance
    /// keeps writing into the marker folder and poisons the NEXT run's verdict. Only instances of the
    /// exe this run started, and only ones started after it, are touched, so a game the user is
    /// playing from another install or from before this run is left alone.
    /// </summary>
    private static void CloseStrays(string exe, DateTime launchedAt)
    {
        foreach (var p in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(exe)))
        {
            try
            {
                if (p.HasExited || p.StartTime < launchedAt.AddSeconds(-2)) continue;
                if (!string.Equals(p.MainModule?.FileName, exe, StringComparison.OrdinalIgnoreCase)) continue;
                p.Kill(entireProcessTree: true);
                p.WaitForExit(5000);
            }
            catch { }
            finally { p.Dispose(); }
        }
    }

    private static TraceLoadResult Report(Plan plan, Dictionary<string, DateTime> snapshot,
        List<TraceFrame> open, bool openStable, int mainIndex, bool gameExited,
        double seconds, string testMap)
    {
        var frames = plan.Functions
            .Select((n, i) => new TraceFrame(i, n, At(snapshot, "e" + i), At(snapshot, "x" + i)))
            .ToList();
        var steps = plan.CallLabels
            .Select((l, i) => new TraceStep(i, l, snapshot.ContainsKey("s" + i)))
            .ToList();
        int reached = steps.Count(s => s.Reached);
        int entered = frames.Count(f => f.Entered);
        var mainFrame = mainIndex >= 0 ? frames[mainIndex] : null;
        var openNames = open.Select(f => f.Function).ToList();
        string? lastReached = Describe(plan, snapshot);
        string? firstMissed = steps.FirstOrDefault(s => !s.Reached)?.Label;

        // A game that quit by itself crashed or was closed, which is a different fault from a hang and
        // the user cannot tell them apart from the loading screen either.
        string exitNote = gameExited
            ? " The game process ended by itself rather than sitting there, so this run was a crash or "
              + "a close, not a freeze."
            : "";
        string capNote = plan.FunctionsSkipped > 0 || plan.CallSitesSkipped > 0
            ? $" Coverage hit its cap ({plan.FunctionCap} functions, {plan.CallSiteCap} call sites), "
              + $"so {plan.FunctionsSkipped} further function(s) and {plan.CallSitesSkipped} further "
              + "call site(s) carry no marker and a hang in one of those is invisible here."
            : "";

        string verdict, detail;
        if (snapshot.Count == 0)
        {
            verdict = "NOTHING_RAN";
            detail = "no marker at all, so the script never executed, which is what a script that "
                   + "does not compile looks like";
        }
        else if (openStable)
        {
            verdict = "ENTERED_NEVER_RETURNED";
            detail = $"{openNames[0]} was entered and never returned, so the load stopped inside it. "
                   + $"Frames still open, innermost first, are {string.Join(", then ", openNames)}."
                   + capNote;
        }
        else if (mainFrame is { Entered: false })
        {
            // The engine runs config first and main later, so "config done, main never started" places
            // the stall outside every function this pass can reach.
            var configFrame = frames.FirstOrDefault(f => f.Function == "config");
            string opening = configFrame is { Exited: true }
                ? "config ran to its end but main never started"
                : "main never started";
            verdict = "STOPPED";
            detail = $"{opening}, so the stall sits in the engine's own load phase between config and "
                   + "main (object data, terrain, imported assets) and not inside a script function. "
                   + $"The last thing the script did was '{lastReached ?? "nothing"}'." + capNote;
        }
        else if (mainFrame is { Entered: true, Exited: true, Open: false })
        {
            verdict = "COMPLETED";
            detail = "main returned and every function that was entered also returned. "
                   + $"{entered} of {plan.Functions.Count} instrumented function(s) ran, and "
                   + $"{reached} of {steps.Count} instrumented call site(s) ran (a call site inside a "
                   + "branch that was not taken is expected to be missing)." + capNote;
        }
        else if (mainIndex < 0)
        {
            // No main at all is not a real map, so fall back to the plain marker count.
            verdict = reached >= steps.Count && steps.Count > 0 ? "COMPLETED" : "STOPPED";
            detail = $"the script declares no main, and {reached} of {steps.Count} instrumented call "
                   + "site(s) ran" + capNote;
        }
        else
        {
            verdict = "STOPPED";
            detail = $"the last thing that ran was '{lastReached}', and {steps.Count - reached} "
                   + $"instrumented call site(s) plus {plan.Functions.Count - entered} instrumented "
                   + "function(s) never ran."
                   + (open.Count > 0
                        ? $" {open.Count} frame(s) looked open ({string.Join(", ", openNames.Take(3))}) "
                          + "but did not hold still across two reads, so they were still moving rather "
                          + "than hung."
                        : "")
                   + capNote;
        }

        return new(verdict, detail + exitNote, steps.Count, reached, lastReached, firstMissed,
            seconds, testMap, steps, plan.Functions.Count, entered,
            plan.FunctionsSkipped, plan.CallSitesSkipped, plan.FunctionCap, plan.CallSiteCap,
            openStable ? openNames[0] : null, openNames, frames);
    }

    /// <summary>Names the newest event on disk, which is the true "how far did it get".</summary>
    private static string? Describe(Plan plan, Dictionary<string, DateTime> snapshot)
    {
        string? best = null;
        var high = DateTime.MinValue;
        foreach (var key in snapshot.Keys.OrderBy(k => k, StringComparer.Ordinal))
        {
            if (snapshot[key] <= high) continue;
            var text = KeyLabel(plan, key);
            if (text is null) continue;
            high = snapshot[key];
            best = text;
        }
        return best;
    }

    private static string? KeyLabel(Plan plan, string key)
    {
        if (key.Length < 2 || !int.TryParse(key[1..], out var n) || n < 0) return null;
        return key[0] switch
        {
            'e' when n < plan.Functions.Count => $"entered {plan.Functions[n]}",
            'x' when n < plan.Functions.Count => $"returned from {plan.Functions[n]}",
            's' when n < plan.CallLabels.Count => plan.CallLabels[n],
            _ => null,
        };
    }

    private static DateTime? At(Dictionary<string, DateTime> snapshot, string key) =>
        snapshot.TryGetValue(key, out var v) ? v : null;

    private static TraceFrame? Frame(Dictionary<string, DateTime> snapshot, List<string> functions, int index) =>
        index < 0 ? null
        : new TraceFrame(index, functions[index], At(snapshot, "e" + index), At(snapshot, "x" + index));

    /// <summary>Open frames, innermost first. The newest entry is by definition the deepest frame.</summary>
    private static List<TraceFrame> OpenFrames(Dictionary<string, DateTime> snapshot, List<string> functions)
    {
        var result = new List<TraceFrame>();
        for (int i = 0; i < functions.Count; i++)
        {
            var f = Frame(snapshot, functions, i)!;
            if (f.Open) result.Add(f);
        }
        result.Sort((a, b) => Nullable.Compare(b.EnteredAt, a.EnteredAt));
        return result;
    }

    /// <summary>
    /// Lists every marker present with the time it was last written. Existence is the signal and the
    /// write time is the ordering, because the game refuses to record a computed payload string, so
    /// nothing about the file's CONTENT may be required for a marker to count.
    /// </summary>
    private static Dictionary<string, DateTime> ReadMarkers(string dir)
    {
        var map = new Dictionary<string, DateTime>(StringComparer.Ordinal);
        if (!Directory.Exists(dir)) return map;
        foreach (var f in Directory.EnumerateFiles(dir, "*.txt"))
        {
            try
            {
                var info = new FileInfo(f);
                if (info.Length == 0) continue;
                map[Path.GetFileNameWithoutExtension(f)] = info.LastWriteTimeUtc;
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
        return map;
    }

    /// <summary>
    /// Adds an entry marker and an exit marker to every function on the init path, plus a
    /// statement-level marker after each call inside the engine's own entry points and inside every
    /// InitTrig_ body. Function selection uses the shared <see cref="JassFunctionIndex"/> span parser
    /// rather than a local pattern, because a hand-rolled matcher that demanded "function" at column
    /// zero has already missed indented declarations elsewhere in this codebase.
    /// </summary>
    private static Plan Instrument(string text, int functionCap, int callSiteCap)
    {
        var lines = text.Split('\n');
        var stripped = JassComments.Strip(lines);
        // Match the file's own line ending so a CRLF script does not come back half converted.
        string eol = lines.Length > 0 && lines[0].EndsWith('\r') ? "\r" : "";

        var byName = new Dictionary<string, JassFunction>(StringComparer.Ordinal);
        foreach (var f in JassFunctionIndex.Parse(text))
        {
            // A constant function may not call a non-constant one, so marking one would stop the
            // script compiling.
            if (f.Signature.StartsWith("constant", StringComparison.Ordinal)) continue;
            byName.TryAdd(f.Name, f);
        }

        var selected = SelectFunctions(byName, stripped, functionCap, out int functionsSkipped);
        var detailed = new HashSet<string>(
            selected.Where(n => EngineEntryPoints.Contains(n, StringComparer.Ordinal)
                                || n.StartsWith("InitTrig_", StringComparison.Ordinal)),
            StringComparer.Ordinal);

        var before = new Dictionary<int, List<string>>();
        var after = new Dictionary<int, List<string>>();
        var callLabels = new List<string>();
        int callSitesSkipped = 0;

        for (int index = 0; index < selected.Count; index++)
        {
            var fn = byName[selected[index]];
            int firstBody = fn.StartLine;              // 0-based line just after the declaration
            int lastBody = fn.EndLine - 2;             // 0-based line just before endfunction

            // Locals must precede every statement in JASS, so the entry marker goes after the last
            // one. Valid JASS keeps locals at the top of the body, so this is still the top.
            int entryAt = fn.StartLine - 1;
            for (int i = firstBody; i <= lastBody; i++)
                if (Token(stripped[i]) == "local") entryAt = i;
            Insert(after, entryAt).AddRange(Marker($"e{index}", eol));

            string? lastToken = null;
            for (int i = firstBody; i <= lastBody; i++)
            {
                var token = Token(stripped[i]);
                if (token is null) continue;
                lastToken = token;
                if (token == "return") Insert(before, i).AddRange(Marker($"x{index}", eol));
                if (token != "call" || !detailed.Contains(fn.Name)) continue;
                var call = CallStatement.Match(stripped[i]);
                if (!call.Success) continue;
                if (callLabels.Count >= callSiteCap) { callSitesSkipped++; continue; }
                Insert(after, i).AddRange(Marker($"s{callLabels.Count}", eol));
                callLabels.Add($"{fn.Name} -> {call.Groups[1].Value}");
            }

            // Skip the marker at endfunction when the body already ends in a return, so no unreachable
            // statement is introduced.
            if (lastToken != "return")
                Insert(before, fn.EndLine - 1).AddRange(Marker($"x{index}", eol));
        }

        var output = new List<string>(lines.Length + 16384);
        for (int i = 0; i < lines.Length; i++)
        {
            if (before.TryGetValue(i, out var pre)) output.AddRange(pre);
            output.Add(lines[i]);
            if (after.TryGetValue(i, out var post)) output.AddRange(post);
        }
        return new Plan(string.Join("\n", output), callLabels, selected,
            functionsSkipped, callSitesSkipped, functionCap, callSiteCap);
    }

    /// <summary>
    /// Picks the functions worth instrumenting, breadth first so the cap trims the outer edge and
    /// never the roots. Two tiers, first everything synchronously reachable by call or by ExecuteFunc,
    /// then the callbacks those functions hand to triggers, timers and enumerations, because ForGroup
    /// and TriggerEvaluate callbacks run inside init and can hang it. The callbacks' own callees are
    /// deliberately left out, they are gameplay code and instrumenting them turns every periodic timer
    /// tick into two disk writes.
    /// </summary>
    private static List<string> SelectFunctions(Dictionary<string, JassFunction> byName,
        string[] stripped, int functionCap, out int skipped)
    {
        var order = new List<string>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        void Add(string name)
        {
            if (byName.ContainsKey(name) && seen.Add(name)) order.Add(name);
        }

        foreach (var name in EngineEntryPoints) Add(name);
        foreach (var f in byName.Values.OrderBy(f => f.StartLine))
            if (f.Name.StartsWith("InitTrig_", StringComparison.Ordinal)) Add(f.Name);

        for (int i = 0; i < order.Count; i++)
        {
            var body = Body(stripped, byName[order[i]]);
            foreach (Match m in CallStatement.Matches(body)) Add(m.Groups[1].Value);
            foreach (Match m in ExecuteFuncTarget.Matches(body)) Add(m.Groups[1].Value);
        }

        int reachable = order.Count;
        for (int i = 0; i < reachable; i++)
        {
            var body = Body(stripped, byName[order[i]]);
            foreach (Match m in FunctionReference.Matches(body)) Add(m.Groups[1].Value);
        }

        skipped = Math.Max(0, order.Count - functionCap);
        if (skipped > 0) order.RemoveRange(functionCap, order.Count - functionCap);
        return order;
    }

    private static string Body(string[] stripped, JassFunction f)
    {
        var sb = new StringBuilder();
        for (int i = f.StartLine; i <= f.EndLine - 2; i++) sb.Append(stripped[i]).Append('\n');
        return sb.ToString();
    }

    /// <summary>
    /// The four statements that record one event. The payload is a literal because a computed one is
    /// silently dropped, see point 4 on the class. It is written even though nothing reads it, so a
    /// human opening the folder can see which marker a file belongs to.
    /// </summary>
    private static IEnumerable<string> Marker(string file, string eol)
    {
        yield return $"    call PreloadGenClear(){eol}";
        yield return $"    call PreloadGenStart(){eol}";
        yield return $"    call Preload(\"{file}\"){eol}";
        yield return $"    call PreloadGenEnd(\"wc3trace\\\\{file}.txt\"){eol}";
    }

    private static List<string> Insert(Dictionary<int, List<string>> at, int line)
    {
        if (!at.TryGetValue(line, out var list)) at[line] = list = new List<string>();
        return list;
    }

    private static string? Token(string line)
    {
        var m = FirstToken.Match(line);
        return m.Success ? m.Groups[1].Value : null;
    }
}
