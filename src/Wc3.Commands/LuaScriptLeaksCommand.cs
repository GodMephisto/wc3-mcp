// src/Wc3.Commands/LuaScriptLeaksCommand.cs
using System.Globalization;
using Wc3.Commands.Lua;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One Lua leak finding. Line is 1-based in the Lua script.</summary>
public sealed record LuaScriptLeak(string Rule, string Heat, string Function, int Line, string Handle, string Code, string Why);

/// <summary>A periodic driver of the Lua script, its period when it could be read statically, how
/// many functions its call tree reaches, and how many hot findings that tree contains.</summary>
public sealed record LuaPeriodic(string Function, double? Period, int Line, int Reached, int HotLeaks);

/// <summary>What the Lua leak pass found.</summary>
public sealed record LuaScriptLeaksResult(
    bool Ok,
    string Message,
    int Functions,
    int HotFunctions,
    int RepeatFunctions,
    IReadOnlyList<LuaScriptLeak> Leaks,
    IReadOnlyList<LuaPeriodic> Periodic,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Finds handle leaks in a Lua map script.
///
/// The rule is not the JASS rule, because Lua has a garbage collector. A discarded Lua handle
/// (a created table or userdata nothing references) is collected, so the classic JASS leak of a
/// local never nulled does not exist here. What still leaks is the engine side. A Warcraft III
/// handle is a real object the engine owns, and the Lua wrapper holding it is all the collector
/// frees, so a handle created on every tick without being released still grows memory, and a table
/// that keeps handles forever is the Lua form of the leak.
///
/// Three unambiguous shapes are reported, each as its own rule.
/// discarded. A creator called as a statement, so its handle is dropped the moment it exists and
/// nothing can release it. The same shape the JASS pass reports.
/// hot-creator. A creator assigned in a function reachable from a periodic timer or trigger, where
/// the function never calls that handle type's release function. One per tick adds up.
/// table-retains. A created handle stored into a table the script never clears, the Lua way to keep
/// a handle alive for the life of the game.
///
/// The creator table and the heat ranking are shared with the JASS pass
/// (<see cref="ScriptLeaksCommand"/>), so both languages agree on what a creator is and on which
/// code is hot. The call graph comes from <see cref="LuaScriptIndex"/>, the real parser.
/// </summary>
public static class LuaScriptLeaksCommand
{
    public static LuaScriptLeaksResult Execute(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var (file, source, isLua) = LuaScriptReader.Read(doc);
        if (!isLua)
            return Empty($"{file} is a JASS script, not Lua.");
        var chunk = LuaScriptReader.TryParse(source, out var error);
        if (chunk is null)
            return Empty($"{file} does not parse, line {error!.Line}. {error.Detail}");
        return Analyze(source, chunk);
    }

    private static LuaScriptLeaksResult Empty(string message) =>
        new(false, message, 0, 0, 0, Array.Empty<LuaScriptLeak>(), Array.Empty<LuaPeriodic>(), Array.Empty<string>());

    internal static LuaScriptLeaksResult Analyze(string source, LuaChunk chunk)
    {
        var diagnostics = new List<string>();
        var functions = LuaScriptIndex.Functions(chunk, source);
        var declared = new HashSet<string>(functions.Select(f => f.Name), StringComparer.Ordinal);

        // Walk every statement with the function it sits in, collecting calls, edges, periodic
        // drivers and every function passed as a value.
        var callsOf = new Dictionary<string, List<CallExpr>>(StringComparer.Ordinal);
        var edges = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
        var triggerActions = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        var periodicTriggers = new List<(string Trigger, double? Period, int Line)>();
        var timerPeriodic = new List<(string Fn, double? Period, int Line)>();
        var callbacks = new HashSet<string>(StringComparer.Ordinal);
        var numbers = new Dictionary<string, double>(StringComparer.Ordinal);

        LuaScriptIndex.Walk(chunk.Body, null, (stmt, fn) =>
        {
            foreach (var e in LuaScriptIndex.TopExprs(stmt))
                foreach (var call in LuaScriptReader.Calls(e))
                {
                    string? callee = LuaScriptReader.CalleeName(call.Func);
                    if (callee is null) continue;
                    if (fn is not null && declared.Contains(fn))
                    {
                        if (!callsOf.TryGetValue(fn, out var list)) callsOf[fn] = list = new List<CallExpr>();
                        list.Add(call);
                        if (!edges.TryGetValue(fn, out var set)) edges[fn] = set = new HashSet<string>(StringComparer.Ordinal);
                        if (declared.Contains(callee)) set.Add(callee);
                    }
                    HandleKnownCall(callee, call, triggerActions, periodicTriggers, timerPeriodic, numbers);
                    foreach (var arg in call.Args)
                        if (LuaScriptReader.CalleeName(arg) is { } argName && declared.Contains(argName)) callbacks.Add(argName);
                }

            // A global numeric literal, so a timer period given by name can be read.
            if (stmt is AssignStmt { Targets.Count: 1, Values.Count: 1 } a && a.Targets[0] is NameExpr n
                && a.Values[0] is LiteralExpr { Kind: "number", Value: not null } lit)
                numbers[n.Name] = ToDouble(lit.Value!);
            if (stmt is LocalStmt { Names.Count: 1, Values.Count: 1 } l && l.Values[0] is LiteralExpr { Kind: "number", Value: not null } ln)
                numbers[l.Names[0]] = ToDouble(ln.Value!);
        });

        foreach (var (trigger, period, line) in periodicTriggers)
            foreach (var handler in triggerActions.GetValueOrDefault(trigger) ?? new List<string>())
                timerPeriodic.Add((handler, period, line));

        HashSet<string> Reach(IEnumerable<string> roots)
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var q = new Queue<string>(roots);
            while (q.Count > 0)
            {
                var n = q.Dequeue();
                if (!seen.Add(n)) continue;
                if (edges.TryGetValue(n, out var e))
                    foreach (var m in e) q.Enqueue(m);
            }
            return seen;
        }

        var hot = Reach(timerPeriodic.Select(p => p.Fn));
        var repeat = Reach(callbacks);
        string Heat(string fn) => hot.Contains(fn) ? "hot" : repeat.Contains(fn) ? "repeat" : "once";

        // Table names cleared anywhere in the file, so a table that keeps handles is not one that
        // the script empties. Only the whole-file view can tell, since the clear can be far away.
        var cleared = FindClearedTables(chunk);

        var leaks = new List<LuaScriptLeak>();
        foreach (var f in functions)
        {
            if (!callsOf.TryGetValue(f.Name, out var calls)) calls = new List<CallExpr>();
            ScanFunction(source, chunk, f, calls, Heat(f.Name), cleared, leaks);
        }

        var ordered = leaks
            .OrderBy(l => ScriptLeaksCommand.HeatRank(l.Heat))
            .ThenBy(l => l.Rule, StringComparer.Ordinal)
            .ThenBy(l => l.Line)
            .ToList();

        var periodicOut = timerPeriodic
            .Select(p =>
            {
                var reach = Reach(new[] { p.Fn });
                int hotLeaks = ordered.Count(l => l.Heat == "hot" && reach.Contains(l.Function));
                return new LuaPeriodic(p.Fn, p.Period, p.Line, reach.Count, hotLeaks);
            })
            .OrderBy(p => p.Period ?? double.MaxValue)
            .ThenBy(p => p.Line)
            .DistinctBy(p => (p.Function, p.Line))
            .ToList();

        int inHot = ordered.Count(l => l.Heat == "hot"), inRepeat = ordered.Count(l => l.Heat == "repeat");
        diagnostics.Add($"{functions.Count} functions, {hot.Count(declared.Contains)} reachable from a periodic "
            + $"timer or trigger, {repeat.Count(declared.Contains)} from any callback");
        string message = $"{ordered.Count} leak(s), {inHot} in hot code, {inRepeat} in repeated code, "
            + $"{ordered.Count - inHot - inRepeat} run once. {periodicOut.Count} periodic driver(s)";
        return new(true, message, functions.Count, hot.Count(declared.Contains), repeat.Count(declared.Contains),
            ordered, periodicOut, diagnostics);
    }

    private static void HandleKnownCall(
        string callee, CallExpr call,
        Dictionary<string, List<string>> triggerActions,
        List<(string, double?, int)> periodicTriggers,
        List<(string, double?, int)> timerPeriodic,
        Dictionary<string, double> numbers)
    {
        switch (callee)
        {
            case "TimerStart":
                if (call.Args.Count >= 4 && IsTrue(call.Args[2]) && LuaScriptReader.CalleeName(call.Args[3]) is { } handler)
                    timerPeriodic.Add((handler, PeriodOf(call.Args[1], numbers), call.Line));
                break;
            case "TriggerRegisterTimerEventPeriodic":
                if (call.Args.Count >= 1 && LuaScriptReader.CalleeName(call.Args[0]) is { } trig)
                    periodicTriggers.Add((trig, call.Args.Count >= 2 ? PeriodOf(call.Args[1], numbers) : null, call.Line));
                break;
            case "TriggerRegisterTimerEvent":
                if (call.Args.Count >= 3 && IsTrue(call.Args[2]) && LuaScriptReader.CalleeName(call.Args[0]) is { } trig2)
                    periodicTriggers.Add((trig2, PeriodOf(call.Args[1], numbers), call.Line));
                break;
            case "TriggerAddAction":
            case "TriggerAddCondition":
                if (call.Args.Count >= 2 && LuaScriptReader.CalleeName(call.Args[0]) is { } t && LuaScriptReader.CalleeName(call.Args[1]) is { } h)
                {
                    if (!triggerActions.TryGetValue(t, out var list)) triggerActions[t] = list = new List<string>();
                    list.Add(h);
                }
                break;
        }
    }

    private static void ScanFunction(
        string source, LuaChunk chunk, JassFunction f, List<CallExpr> calls, string heat,
        IReadOnlyDictionary<string, int> cleared, List<LuaScriptLeak> leaks)
    {
        // Release functions the function calls at all, so a handle released through one is quiet.
        var releases = calls.Select(c => LuaScriptReader.CalleeName(c.Func)).Where(c => c is not null).ToHashSet(StringComparer.Ordinal)!;

        LuaBlock? body = FindBody(chunk.Body, f.Name);
        if (body is null) return;

        foreach (var stmt in LuaScriptReader.StatementsOf(body))
        {
            var topLevelCall = (stmt as CallStmt)?.Call;
            foreach (var e in LuaScriptIndex.TopExprs(stmt))
                foreach (var call in LuaScriptReader.Calls(e))
                {
                    if (LuaScriptReader.CalleeName(call.Func) is not { } creator || !ScriptLeaksCommand.Creators.TryGetValue(creator, out var type))
                        continue;
                    bool discarded = ReferenceEquals(call, topLevelCall);
                    if (discarded)
                    {
                        leaks.Add(new LuaScriptLeak("discarded", heat, f.Name, call.Line, type,
                            LuaScriptReader.LineText(source, call.Line).Trim(),
                            $"{creator} is called as a statement, so its {type} is dropped and the engine handle is never released"));
                        continue;
                    }
                    if (heat == "hot" && !ScriptLeaksCommand.Destroyers[type].Any(releases.Contains))
                    {
                        leaks.Add(new LuaScriptLeak("hot-creator", heat, f.Name, call.Line, type,
                            LuaScriptReader.LineText(source, call.Line).Trim(),
                            $"{creator} runs on a period and this function never calls {string.Join(" or ", ScriptLeaksCommand.Destroyers[type])}, "
                            + "so the engine handle grows each tick"));
                    }
                }

            // table-retains. A handle stored into a table the script never clears.
            foreach (var (table, creatorCall) in TableStores(stmt))
            {
                if (cleared.TryGetValue(table, out var clearLine) && clearLine > creatorCall.Line) continue;
                leaks.Add(new LuaScriptLeak("table-retains", heat, f.Name, creatorCall.Line,
                    ScriptLeaksCommand.Creators[LuaScriptReader.CalleeName(creatorCall.Func)!],
                    LuaScriptReader.LineText(source, creatorCall.Line).Trim(),
                    $"the handle from {LuaScriptReader.CalleeName(creatorCall.Func)} is stored in table '{table}', which the script never clears"));
            }
        }
    }

    private static LuaBlock? FindBody(LuaBlock block, string name)
    {
        foreach (var s in block)
        {
            if (s is FunctionStmt fs && fs.Name == name) return fs.Body;
            if (s is AssignStmt a && a.Targets.Count == 1 && NameOf(a.Targets[0]) == name && a.Values.Count == 1
                && a.Values[0] is FunctionExpr fe) return fe.Body;
            if (s is LocalStmt l && l.Names.Count == 1 && l.Names[0] == name && l.Values.Count == 1
                && l.Values[0] is FunctionExpr lfe) return lfe.Body;
        }
        return null;
    }

    /// <summary>Tables that receive a created handle, with the creator call, for a statement.</summary>
    private static IEnumerable<(string Table, CallExpr Creator)> TableStores(LuaStmt stmt)
    {
        if (stmt is AssignStmt a)
            foreach (var t in a.Targets)
            {
                if (t is not IndexExpr { Base: NameExpr n }) continue;
                foreach (var v in a.Values)
                    foreach (var call in LuaScriptReader.Calls(v))
                        if (LuaScriptReader.CalleeName(call.Func) is { } c && ScriptLeaksCommand.Creators.ContainsKey(c))
                            yield return (n.Name, call);
            }
        if (stmt is CallStmt cs && LuaScriptReader.CalleeName(cs.Call.Func) == "table.insert" && cs.Call.Args.Count >= 2
            && LuaScriptReader.CalleeName(cs.Call.Args[0]) is { } tbl)
            foreach (var call in LuaScriptReader.Calls(cs.Call.Args[1]))
                if (LuaScriptReader.CalleeName(call.Func) is { } c && ScriptLeaksCommand.Creators.ContainsKey(c))
                    yield return (tbl, call);
    }

    private static Dictionary<string, int> FindClearedTables(LuaChunk chunk)
    {
        var cleared = new Dictionary<string, int>(StringComparer.Ordinal);
        void Mark(string name, int line)
        {
            if (!cleared.TryGetValue(name, out var prev) || line > prev) cleared[name] = line;
        }
        LuaScriptIndex.Walk(chunk.Body, null, (stmt, _) =>
        {
            if (stmt is AssignStmt { Targets.Count: 1, Values.Count: 1 } a && a.Targets[0] is NameExpr n)
            {
                if (a.Values[0] is LiteralExpr { Kind: "nil" }
                    || a.Values[0] is TableExpr { Fields.Count: 0 })
                    Mark(n.Name, a.Line);
            }
            if (stmt is CallStmt cs && LuaScriptReader.CalleeName(cs.Call.Func) is "table.remove" or "table.clear"
                && cs.Call.Args.Count >= 1 && LuaScriptReader.CalleeName(cs.Call.Args[0]) is { } t)
                Mark(t, cs.Line);
        });
        return cleared;
    }

    private static string? NameOf(LuaExpr e) => e is NameExpr n ? n.Name : null;

    private static bool IsTrue(LuaExpr e) => e is LiteralExpr { Kind: "boolean", Value: true };

    private static double? PeriodOf(LuaExpr e, IReadOnlyDictionary<string, double> numbers) => e switch
    {
        LiteralExpr { Kind: "number", Value: not null } lit => ToDouble(lit.Value!),
        NameExpr n when numbers.TryGetValue(n.Name, out var v) => v,
        _ => null,
    };

    private static double ToDouble(object v) => v switch
    {
        long l => l,
        double d => d,
        _ => Convert.ToDouble(v, CultureInfo.InvariantCulture),
    };
}
