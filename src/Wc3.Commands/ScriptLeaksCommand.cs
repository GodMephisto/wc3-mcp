// src/Wc3.Commands/ScriptLeaksCommand.cs
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One leak finding. Line is 1-based in war3map.j.</summary>
public sealed record ScriptLeak(string Rule, string Heat, string Function, int Line, string Handle, string Code, string Why);

/// <summary>A timer or trigger that fires on a period, and the function it runs.</summary>
public sealed record ScriptPeriodic(string Function, double? Period, int Line, int Reached);

public sealed record ScriptLeaksResult(
    bool Ok,
    string Message,
    int Functions,
    int HotFunctions,
    int RepeatFunctions,
    IReadOnlyList<ScriptLeak> Leaks,
    IReadOnlyList<ScriptPeriodic> Periodic,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Finds handle leaks in a JASS map script, the kind that make a long game grow in memory and
/// slow down, and says how often each one runs.
///
/// JASS has no garbage collector for handles. A location, group, force, effect, timer, text tag,
/// lightning or trigger lives until the script destroys it, so one created on every tick and never
/// destroyed is memory the game never gets back. Only the shapes that are unambiguous in the text
/// are reported, each as its own rule.
///
/// discarded. A creator called as a statement, so its handle is thrown away the moment it exists
/// and nothing can ever destroy it.
/// inline. A location, group or force creator used as an argument to anything but its destroyer,
/// so the temporary is never destroyed. A group passed to ForGroupBJ after bj_wantDestroyGroup is
/// set true is destroyed by Blizzard.j and is not reported.
/// never-destroyed. A local assigned from a creator that the function never destroys, returns,
/// stores anywhere, or hands to a function of the map (which might destroy it). What the map's
/// own code does with a handle is not followed, so this rule stays silent rather than guess.
///
/// Heat says how often the code runs. hot is reachable from a timer or trigger on a period, repeat
/// from any callback (events, timers, ForGroup), once from main and init only. A leak that runs once
/// costs a few bytes and is listed last. The same finding in hot code is what a long game feels.
/// </summary>
public static class ScriptLeaksCommand
{
    private static readonly Dictionary<string, string> Creators = new(StringComparer.Ordinal)
    {
        ["Location"] = "location", ["GetUnitLoc"] = "location", ["GetRectCenter"] = "location",
        ["GetSpellTargetLoc"] = "location", ["PolarProjectionBJ"] = "location", ["OffsetLocation"] = "location",
        ["GetOrderPointLoc"] = "location", ["GetUnitRallyPoint"] = "location", ["GetRandomLocInRect"] = "location",
        ["GetPlayerStartLocationLoc"] = "location", ["GetCameraTargetPositionLoc"] = "location",
        ["GetCameraEyePositionLoc"] = "location", ["CameraSetupGetDestPositionLoc"] = "location",
        ["GetDestructableLoc"] = "location", ["GetItemLoc"] = "location", ["GetRectCenterLoc"] = "location",
        ["CreateGroup"] = "group", ["GetUnitsInRectAll"] = "group", ["GetUnitsInRectOfPlayer"] = "group",
        ["GetUnitsInRectMatching"] = "group", ["GetUnitsInRangeOfLocAll"] = "group",
        ["GetUnitsInRangeOfLocMatching"] = "group", ["GetUnitsOfPlayerAll"] = "group",
        ["GetUnitsOfPlayerMatching"] = "group", ["GetUnitsOfPlayerAndTypeId"] = "group",
        ["GetUnitsOfTypeIdAll"] = "group", ["GetUnitsSelectedAll"] = "group", ["GetRandomSubGroup"] = "group",
        ["CreateForce"] = "force", ["GetPlayersAllies"] = "force", ["GetPlayersEnemies"] = "force",
        ["GetPlayersMatching"] = "force", ["GetPlayersByMapControl"] = "force", ["GetForceOfPlayer"] = "force",
        ["AddSpecialEffect"] = "effect", ["AddSpecialEffectLoc"] = "effect", ["AddSpecialEffectTarget"] = "effect",
        ["AddSpecialEffectLocBJ"] = "effect", ["AddSpecialEffectTargetUnitBJ"] = "effect",
        ["AddSpellEffect"] = "effect", ["AddSpellEffectById"] = "effect", ["AddSpellEffectLoc"] = "effect",
        ["AddSpellEffectByIdLoc"] = "effect", ["AddSpellEffectTarget"] = "effect",
        ["AddSpellEffectTargetById"] = "effect",
        ["CreateTimer"] = "timer", ["CreateTextTag"] = "texttag", ["AddLightning"] = "lightning",
        ["AddLightningEx"] = "lightning", ["CreateTrigger"] = "trigger",
    };

    private static readonly Dictionary<string, string[]> Destroyers = new(StringComparer.Ordinal)
    {
        ["location"] = new[] { "RemoveLocation" },
        ["group"] = new[] { "DestroyGroup" },
        ["force"] = new[] { "DestroyForce" },
        ["effect"] = new[] { "DestroyEffect", "DestroyEffectBJ" },
        ["timer"] = new[] { "DestroyTimer", "PauseTimer" },
        ["texttag"] = new[] { "DestroyTextTag", "SetTextTagPermanent", "SetTextTagLifespan" },
        ["lightning"] = new[] { "DestroyLightning", "DestroyLightningBJ" },
        ["trigger"] = new[] { "DestroyTrigger" },
    };

    private static readonly Regex Call = new(@"\b([A-Za-z_]\w*)\s*\(", RegexOptions.Compiled);
    private static readonly Regex FunctionRef = new(@"\bfunction\s+([A-Za-z_]\w*)", RegexOptions.Compiled);
    private static readonly Regex LocalDecl = new(@"^\s*local\s+(\w+)\s+(?:array\s+)?(\w+)\s*(?:=\s*(.*))?$", RegexOptions.Compiled);
    private static readonly Regex SetStmt = new(@"^\s*set\s+(\w+)\s*=\s*(.*)$", RegexOptions.Compiled);
    private static readonly Regex CallStmt = new(@"^\s*call\s+([A-Za-z_]\w*)\s*\(", RegexOptions.Compiled);
    private static readonly Regex ReturnStmt = new(@"^\s*return\b\s*(.*)$", RegexOptions.Compiled);
    private static readonly Regex TimerStartPeriodic = new(
        @"\bTimerStart\s*\(\s*[^,]+,\s*([^,]+?)\s*,\s*true\s*,\s*function\s+(\w+)\s*\)", RegexOptions.Compiled);
    private static readonly Regex TriggerPeriodic = new(
        @"\bTriggerRegisterTimerEvent(?:Periodic)?\s*\(\s*(\w+)\s*,\s*([^,\)]+?)\s*(?:,\s*true\s*)?\)", RegexOptions.Compiled);
    private static readonly Regex TriggerCallback = new(
        @"\bTriggerAdd(?:Action|Condition)\s*\(\s*(\w+)\s*,\s*(?:Condition\s*\(\s*|Filter\s*\(\s*)?function\s+(\w+)", RegexOptions.Compiled);
    private static readonly Regex RealGlobal = new(@"^\s*(?:constant\s+)?real\s+(\w+)\s*=\s*([0-9]*\.?[0-9]+)\s*$", RegexOptions.Compiled);
    private static readonly Regex WantDestroy = new(@"\bset\s+bj_wantDestroyGroup\s*=\s*true", RegexOptions.Compiled);

    public static ScriptLeaksResult Execute(MapDocument doc)
    {
        var diagnostics = new List<string>();
        if (!UabiRuntimeRepairCommand.TryReadScript(doc, out _, out var script, out var why))
            return new(false, why, 0, 0, 0, Array.Empty<ScriptLeak>(), Array.Empty<ScriptPeriodic>(), diagnostics);
        return Analyze(script, diagnostics);
    }

    internal static ScriptLeaksResult Analyze(string script, List<string>? diagnostics = null)
    {
        diagnostics ??= new List<string>();
        var lines = script.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        var functions = JassFunctionIndex.Parse(string.Join('\n', lines));
        var byName = new Dictionary<string, JassFunction>(StringComparer.Ordinal);
        foreach (var f in functions) byName.TryAdd(f.Name, f);

        var reals = new Dictionary<string, double>(StringComparer.Ordinal);
        foreach (var l in lines)
            if (RealGlobal.Match(l) is { Success: true } g
                && double.TryParse(g.Groups[2].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
                reals.TryAdd(g.Groups[1].Value, v);
        double? Period(string expr)
        {
            expr = expr.Trim();
            if (double.TryParse(expr, NumberStyles.Float, CultureInfo.InvariantCulture, out var p)) return p;
            return reals.TryGetValue(expr, out var r) ? r : null;
        }

        // Call graph. A call edge runs the callee now, a function reference hands it to the
        // engine to run later, which is what makes code repeat.
        var calls = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
        var callbacks = new HashSet<string>(StringComparer.Ordinal);
        var periodic = new List<(string Fn, double? P, int Line)>();
        var triggerActions = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        foreach (var f in functions)
        {
            var edges = calls[f.Name] = new HashSet<string>(StringComparer.Ordinal);
            for (int i = f.StartLine; i < f.EndLine - 1; i++)
            {
                string code = StripComment(lines[i]);
                foreach (Match c in Call.Matches(code))
                    if (byName.ContainsKey(c.Groups[1].Value)) edges.Add(c.Groups[1].Value);
                foreach (Match r in FunctionRef.Matches(code))
                    if (byName.ContainsKey(r.Groups[1].Value)) { callbacks.Add(r.Groups[1].Value); edges.Add(r.Groups[1].Value); }
                foreach (Match t in TimerStartPeriodic.Matches(code))
                    periodic.Add((t.Groups[2].Value, Period(t.Groups[1].Value), i + 1));
                foreach (Match a in TriggerCallback.Matches(code))
                    (triggerActions.TryGetValue(a.Groups[1].Value, out var l) ? l : triggerActions[a.Groups[1].Value] = new()).Add(a.Groups[2].Value);
            }
        }
        for (int i = 0; i < lines.Length; i++)
            foreach (Match t in TriggerPeriodic.Matches(StripComment(lines[i])))
                if (t.Value.Contains("Periodic", StringComparison.Ordinal) || t.Value.Contains("true", StringComparison.Ordinal))
                    foreach (var fn in triggerActions.GetValueOrDefault(t.Groups[1].Value) ?? new())
                        periodic.Add((fn, Period(t.Groups[2].Value), i + 1));

        HashSet<string> Reach(IEnumerable<string> roots)
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var q = new Queue<string>(roots);
            while (q.Count > 0)
            {
                var n = q.Dequeue();
                if (!seen.Add(n)) continue;
                if (calls.TryGetValue(n, out var e)) foreach (var m in e) q.Enqueue(m);
            }
            return seen;
        }
        var hot = Reach(periodic.Select(p => p.Fn));
        var repeat = Reach(callbacks);
        string Heat(string fn) => hot.Contains(fn) ? "hot" : repeat.Contains(fn) ? "repeat" : "once";

        var leaks = new List<ScriptLeak>();
        foreach (var f in functions) Scan(f, lines, byName, Heat(f.Name), leaks);

        int Rank(string h) => h == "hot" ? 0 : h == "repeat" ? 1 : 2;
        var ordered = leaks.OrderBy(l => Rank(l.Heat)).ThenBy(l => l.Rule).ThenBy(l => l.Line).ToList();
        var periodicOut = periodic
            .Select(p => new ScriptPeriodic(p.Fn, p.P, p.Line, Reach(new[] { p.Fn }).Count))
            .OrderBy(p => p.Period ?? double.MaxValue).ThenBy(p => p.Line).ToList();

        int inHot = ordered.Count(l => l.Heat == "hot"), inRepeat = ordered.Count(l => l.Heat == "repeat");
        diagnostics.Add($"{functions.Count} functions, {hot.Count(byName.ContainsKey)} reachable from a periodic "
            + $"timer or trigger, {repeat.Count(byName.ContainsKey)} from any callback");
        string message = $"{ordered.Count} leak(s), {inHot} in hot code, {inRepeat} in repeated code, "
            + $"{ordered.Count - inHot - inRepeat} run once. {periodicOut.Count} periodic timer(s) or trigger(s)";
        return new(true, message, functions.Count, hot.Count(byName.ContainsKey), repeat.Count(byName.ContainsKey),
            ordered, periodicOut, diagnostics);
    }

    private static void Scan(JassFunction f, string[] lines, Dictionary<string, JassFunction> mapFunctions,
        string heat, List<ScriptLeak> leaks)
    {
        var body = new List<(int Line, string Code)>();
        for (int i = f.StartLine; i < f.EndLine - 1; i++)
        {
            string code = StripComment(lines[i]).Trim();
            if (code.Length > 0) body.Add((i + 1, code));
        }
        var locals = new Dictionary<string, (string Type, int Line, string Creator, string Code)>(StringComparer.Ordinal);
        bool wantDestroy = false;

        foreach (var (line, code) in body)
        {
            if (WantDestroy.IsMatch(code)) wantDestroy = true;

            // discarded
            if (CallStmt.Match(code) is { Success: true } cs && Creators.TryGetValue(cs.Groups[1].Value, out var dType))
            {
                leaks.Add(new("discarded", heat, f.Name, line, dType, code,
                    $"{cs.Groups[1].Value} is called as a statement, so its {dType} is thrown away and can never be destroyed"));
                continue;
            }

            // assignment targets, and the expression each one takes
            string? target = null, expr = code;
            if (LocalDecl.Match(code) is { Success: true } ld)
            {
                expr = ld.Groups[3].Success ? ld.Groups[3].Value : "";
                target = ld.Groups[2].Value;
                if (Creators.ContainsKey(Head(expr)))
                    locals[target] = (Creators[Head(expr)], line, Head(expr), code);
            }
            else if (SetStmt.Match(code) is { Success: true } st)
            {
                target = st.Groups[1].Value;
                expr = st.Groups[2].Value;
                if (Creators.TryGetValue(Head(expr), out var t))
                {
                    if (locals.ContainsKey(target) || IsDeclaredLocal(body, target))
                        locals[target] = (t, line, Head(expr), code);
                }
            }

            // inline, a location, group or force creator nested inside another call
            foreach (var (creator, type, outer) in Nested(expr))
            {
                if (type is not ("location" or "group" or "force")) continue;
                if (Destroyers[type].Contains(outer) || outer.StartsWith("Save", StringComparison.Ordinal)) continue;
                if (type == "group" && outer == "ForGroupBJ" && wantDestroy) continue;
                leaks.Add(new("inline", heat, f.Name, line, type, code,
                    $"{creator} makes a temporary {type} passed to {outer}, and nothing destroys it"));
            }
        }

        // never-destroyed
        foreach (var (name, info) in locals)
        {
            bool handled = false;
            var word = new Regex($@"\b{Regex.Escape(name)}\b");
            foreach (var (line, code) in body)
            {
                if (line == info.Line || !word.IsMatch(code)) continue;
                if (Destroyers[info.Type].Any(d => Regex.IsMatch(code, $@"\b{d}\s*\(\s*{Regex.Escape(name)}\b"))) { handled = true; break; }
                if (ReturnStmt.Match(code) is { Success: true } rt && word.IsMatch(rt.Groups[1].Value)) { handled = true; break; }
                // A timer started with a callback, or a trigger given an action, belongs to that
                // callback now, which is where GetExpiredTimer or GetTriggeringTrigger destroys it.
                if (Regex.IsMatch(code, $@"\b(TimerStart|TriggerAddAction|TriggerAddCondition)\s*\(\s*{Regex.Escape(name)}\b"))
                { handled = true; break; }
                if (SetStmt.Match(code) is { Success: true } st && st.Groups[1].Value != name && word.IsMatch(st.Groups[2].Value)
                    && !Regex.IsMatch(st.Groups[2].Value, $@"\w\s*\(\s*[^)]*\b{Regex.Escape(name)}\b"))
                { handled = true; break; }   // stored in another variable
                foreach (Match c in Call.Matches(code))
                {
                    string callee = c.Groups[1].Value;
                    if (!mapFunctions.ContainsKey(callee) && !callee.StartsWith("Save", StringComparison.Ordinal)) continue;
                    int open = c.Index + c.Length;
                    int close = MatchParen(code, open - 1);
                    if (close > open && word.IsMatch(code[open..close])) { handled = true; break; }
                }
                if (handled) break;
            }
            if (handled) continue;
            leaks.Add(new("never-destroyed", heat, f.Name, info.Line, info.Type, info.Code,
                $"local {name} holds a {info.Type} from {info.Creator}, and this function never destroys, returns or stores it"));
        }
    }

    private static bool IsDeclaredLocal(List<(int Line, string Code)> body, string name) =>
        body.Any(b => LocalDecl.Match(b.Code) is { Success: true } m && m.Groups[2].Value == name);

    private static string Head(string expr)
    {
        var m = Regex.Match(expr.Trim(), @"^([A-Za-z_]\w*)\s*\(");
        return m.Success ? m.Groups[1].Value : "";
    }

    /// <summary>Every creator call nested inside another call, with the call that receives it.</summary>
    private static IEnumerable<(string Creator, string Type, string Outer)> Nested(string expr)
    {
        var stack = new Stack<string>();
        string s = MaskStrings(expr);
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == '(')
            {
                int j = i - 1;
                while (j >= 0 && s[j] == ' ') j--;
                int k = j;
                while (k >= 0 && (char.IsLetterOrDigit(s[k]) || s[k] == '_')) k--;
                string name = j >= 0 ? s[(k + 1)..(j + 1)] : "";
                if (name.Length > 0 && Creators.TryGetValue(name, out var type) && stack.Count > 0 && stack.Peek().Length > 0)
                    yield return (name, type, stack.Peek());
                stack.Push(name);
            }
            else if (s[i] == ')' && stack.Count > 0) stack.Pop();
        }
    }

    private static int MatchParen(string s, int open)
    {
        int depth = 0;
        for (int i = open; i < s.Length; i++)
        {
            if (s[i] == '(') depth++;
            else if (s[i] == ')' && --depth == 0) return i;
        }
        return -1;
    }

    private static string MaskStrings(string s)
    {
        var sb = new StringBuilder(s);
        bool inString = false;
        for (int i = 0; i < sb.Length; i++)
        {
            if (inString && sb[i] == '\\') { sb[i] = ' '; if (i + 1 < sb.Length) sb[++i] = ' '; continue; }
            if (sb[i] == '"') { inString = !inString; continue; }
            if (inString) sb[i] = ' ';
        }
        return sb.ToString();
    }

    private static string StripComment(string line)
    {
        bool inString = false;
        for (int i = 0; i < line.Length - 1; i++)
        {
            char c = line[i];
            if (c == '\\' && inString) { i++; continue; }
            if (c == '"') inString = !inString;
            else if (!inString && c == '/' && line[i + 1] == '/') return line[..i];
        }
        return line;
    }
}
