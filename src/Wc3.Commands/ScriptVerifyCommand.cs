using System.Text.RegularExpressions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>Severity of a finding. High is a state change that likely never restores, a real
/// defect. Medium is a pattern that is often intentional, an ability removed after use or hidden
/// during a morph. The grade lets a reader weight the list instead of trusting the count.</summary>
public sealed record ScriptVerifyFinding(string Severity, string Rule, string Function, string Detail);
public sealed record ScriptVerifyResult(
    bool Ok, string Message, string File, int Functions, int PeriodFunctions,
    IReadOnlyList<ScriptVerifyFinding> Findings);

/// <summary>
/// Static verifier for a compiled JASS script. It models the one engine behaviour a per-function
/// grep cannot: reachability. A state change with a restore, PauseUnit, EnableUserControl,
/// SetUnitTimeScale(0), UnitRemoveAbility, CreateGroup, Location, is correct exactly when its
/// restore is reachable from the function that starts it, following direct calls AND
/// TimerStart(..., function X) callback edges. That is why a channel ability with the pause in
/// _Start and the resume in _Loop is seen as balanced, where a grep flags all of them.
///
/// It does not model the engine's own ability-field or buff semantics, so a rule it does not
/// have cannot be assumed clean. It says so in its own result rather than implying coverage.
/// </summary>
public static class ScriptVerifyCommand
{
    public static ScriptVerifyResult Execute(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var (file, source, isLua) = ScriptAnalysis.Read(doc);
        if (isLua)
            return new(false, "the script is Lua; the verifier reads JASS only", file, 0, 0,
                Array.Empty<ScriptVerifyFinding>());

        var functions = ScriptAnalysis.Functions(source, false);
        var lineStarts = LineStarts(source);
        var bodies = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var f in functions)
        {
            // Analyse live code only. A commented-out SetPlayerAbilityAvailable or
            // UnitRemoveAbility is not a defect; SusanoMadaraCast's hide and its partner are both
            // commented, which is why ability-hide and ability-strip each read 40 before this.
            var slice = ScriptCommand.SliceFunction(source, lineStarts, f.StartLine, f.EndLine);
            bodies[f.Name] = string.Join('\n'.ToString(), ScriptAnalysis.CodeLines(slice, false));
        }

        // edges: direct calls to other functions, and timer callbacks
        var edges = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
        var timers = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
        foreach (var (name, body) in bodies)
        {
            var called = new HashSet<string>(
                Regex.Matches(body, @"function\s+(\w+)").Select(m => m.Groups[1].Value), StringComparer.Ordinal);
            called.Remove(name);
            edges[name] = called;
            var cbs = new HashSet<string>(
                Regex.Matches(body, @"TimerStart\([^,]+,\s*[0-9.]+\s*,\s*true\s*,\s*function\s+(\w+)")
                    .Select(m => m.Groups[1].Value), StringComparer.Ordinal);
            if (cbs.Count > 0) timers[name] = cbs;
        }

        HashSet<string> Reachable(string start)
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var stack = new Stack<string>();
            stack.Push(start);
            while (stack.Count > 0)
            {
                var n = stack.Pop();
                if (!seen.Add(n) || !bodies.ContainsKey(n)) continue;
                foreach (var e in edges[n]) stack.Push(e);
                if (timers.TryGetValue(n, out var t)) foreach (var e in t) stack.Push(e);
            }
            return seen;
        }

        bool AnyReachable(string src, string pattern)
        {
            foreach (var r in Reachable(src))
                if (bodies.TryGetValue(r, out var b) && Regex.IsMatch(b, pattern))
                    return true;
            return false;
        }

        // A function that IS a timer or periodic-event callback, as opposed to one that merely
        // starts a timer. GetTriggerUnit and sleeps are only wrong in the callback.
        var callbacks = new HashSet<string>(StringComparer.Ordinal);
        foreach (var cbs in timers.Values) foreach (var cb in cbs) callbacks.Add(cb);
        foreach (var (n, b) in bodies)
            if (Regex.IsMatch(b, @"TriggerRegisterTimerEventPeriodic\(")) callbacks.Add(n);

        // Abilities added anywhere and abilities shown-available anywhere. A hero ability is
        // granted in an init far from the code that removes it, so a per-path check cannot see it
        // and would flag legitimate choreography 460 times.
        var addedAbilities = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var shownAbilities = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var b in bodies.Values)
        {
            foreach (Match m in Regex.Matches(b, @"UnitAddAbility\(\s*\w+\s*,\s*'([A-Za-z0-9]{4})'"))
                addedAbilities.Add(m.Groups[1].Value);
            foreach (Match m in Regex.Matches(b,
                         @"SetPlayerAbilityAvailable\(\s*\w+\s*,\s*'([A-Za-z0-9]{4})'\s*,\s*true\s*\)"))
                shownAbilities.Add(m.Groups[1].Value);
        }

        // REA: resolve each hidden or removed ability against the map's own object data. An
        // ability on a hero's uabi list is a real hero ability, and losing it is a real defect.
        // One that is on no uabi is a temporary or dummy ability and its removal is choreography.
        var heroAbilities = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        try
        {
            foreach (var e in ObjectKinds.MergedEntries(doc, ObjectKinds.Info(ObjectKind.Unit)))
                foreach (var mod in e.Mods)
                    if (mod.Key.StartsWith("uabi", StringComparison.OrdinalIgnoreCase))
                        foreach (var code in mod.Value.Split(',', StringSplitOptions.RemoveEmptyEntries))
                            heroAbilities.Add(code.Trim());
        }
        catch
        {
            // object data unreadable, fall back to the text-only grading rather than failing.
        }

        var findings = new List<ScriptVerifyFinding>();
        var mediumRules = new HashSet<string>(StringComparer.Ordinal)
        { "ability-strip", "ability-hide", "bj-last" };
        void Flag(string rule, string fn, string detail, bool forceHigh = false) =>
            findings.Add(new(forceHigh || !mediumRules.Contains(rule) ? "high" : "medium", rule, fn, detail));

        foreach (var (name, body) in bodies)
        {
            // state change with a restore elsewhere on a reachable path
            if (Regex.IsMatch(body, @"PauseUnit\(\s*\w+\s*,\s*true\s*\)")
                && !AnyReachable(name, @"PauseUnit\(\s*\w+\s*,\s*false\s*\)"))
                Flag("pause", name, "PauseUnit(true) with no restore reachable through any call or timer");

            if (Regex.IsMatch(body, @"EnableUserControl\(\s*false\s*\)")
                && !AnyReachable(name, @"EnableUserControl\(\s*true\s*\)"))
                Flag("ucontrol", name, "EnableUserControl(false) with no enable reachable");

            if (Regex.IsMatch(body, @"SetUnitTimeScale\(\s*(\w+)\s*,\s*0(?:\.0*)?\s*\)")
                && !AnyReachable(name, @"SetUnitTimeScale\(\s*\w+\s*,\s*[1-9]"))
                Flag("timescale", name, "SetUnitTimeScale(u,0) with no non-zero restore reachable");

            // periodic timer started but never stopped on a reachable path
            if (timers.TryGetValue(name, out var cbs) && !AnyReachable(name, @"PauseTimer\(|DestroyTimer\("))
                foreach (var cb in cbs)
                    Flag("timer", name, $"periodic timer -> {cb}, never paused or destroyed on a reachable path");

            // groups and locations
            foreach (Match m in Regex.Matches(body, @"local group (\w+)\s*=\s*CreateGroup\(\)"))
                if (!Regex.IsMatch(body, @"DestroyGroup\(\s*" + Regex.Escape(m.Groups[1].Value) + @"\s*\)")
                    && !AnyReachable(name, @"DestroyGroup\("))
                    Flag("group-leak", name, $"local group {m.Groups[1].Value} never destroyed here or on a reachable path");
            int locs = Regex.Matches(body, @"Location\(").Count;
            if (locs > 0 && !AnyReachable(name, @"RemoveLocation\("))
                Flag("location-leak", name, $"{locs} Location, no RemoveLocation reachable");

            // ability removed and never re-added. Only a real ability, a rawcode that is not a
            // buff, counts: UnitRemoveAbility(u,'B039') clears a debuff and needs no add, which
            // is why this rule reported 505 when it did not distinguish the two.
            var localDummies = new HashSet<string>(StringComparer.Ordinal);
            foreach (Match m in Regex.Matches(body, @"local unit (\w+)\s*=\s*CreateUnit"))
                localDummies.Add(m.Groups[1].Value);
            foreach (Match m in Regex.Matches(body, @"UnitRemoveAbility\(\s*(\w+)\s*,\s*'([A-Za-z0-9]{4})'"))
            {
                var unit = Regex.Escape(m.Groups[1].Value);
                var raw = m.Groups[2].Value;
                if (raw.StartsWith("B", StringComparison.OrdinalIgnoreCase)) continue;
                if (localDummies.Contains(m.Groups[1].Value)) continue;
                if (addedAbilities.Contains(raw)) continue;
                if (!AnyReachable(name, @"UnitAddAbility\(\s*" + unit + @"\s*,\s*'" + raw + @"'\s*\)"))
                    Flag("ability-strip", name, $"UnitRemoveAbility({unit},'{raw}') with no matching UnitAddAbility reachable",
                        forceHigh: heroAbilities.Contains(raw));
            }
            foreach (Match m in Regex.Matches(body,
                         @"SetPlayerAbilityAvailable\(\s*(\w+)\s*,\s*('[^']+'),\s*false\s*\)"))
            {
                var pat = @"SetPlayerAbilityAvailable\(\s*" + Regex.Escape(m.Groups[1].Value)
                          + @"\s*,\s*" + Regex.Escape(m.Groups[2].Value) + @"\s*,\s*true\s*\)";
                if (shownAbilities.Contains(m.Groups[2].Value)) continue;
                // Morph teardown: the show for a hidden ability lives in the ability's own
                // Stop/End/Off/Deactivate function, which is not called from the cast. Pair by the
                // longest shared name stem, per the community morph pattern.
                // Morph teardown: the show for a hidden ability lives in the ability's own
                // Stop/End/Off/Deactivate function, not in the cast, so reachability misses it.
                // Pair by shared name prefix, which is how these functions are named
                // (SusanoMadaraCast hides, SusanoMadaraDeactivate shows).
                static int CommonPrefix(string a, string b)
                {
                    int i = 0;
                    while (i < a.Length && i < b.Length && a[i] == b[i]) i++;
                    return i;
                }
                bool shownInTeardown = bodies.Any(kv =>
                    kv.Key != name && CommonPrefix(kv.Key, name) >= 8
                    && Regex.IsMatch(kv.Value, @"SetPlayerAbilityAvailable\([^,]+,\s*'" + Regex.Escape(m.Groups[2].Value) + @"'\s*,\s*true\s*\)"));
                if (shownInTeardown) continue;
                if (!AnyReachable(name, pat))
                    Flag("ability-hide", name, $"hides {m.Groups[2].Value} with no matching true reachable",
                        forceHigh: heroAbilities.Contains(m.Groups[2].Value.Replace("'", "")));
            }

            // GetLocalPlayer near a game-state call, a desync
            foreach (var line in body.Split('\n'))
            {
                if (!line.Contains("GetLocalPlayer")) continue;
                if (Regex.IsMatch(line, @"CreateUnit|IssueOrder|SetUnitX|SetUnitY|UnitDamageTarget|KillUnit"))
                    Flag("desync", name, "GetLocalPlayer on the same line as a game-state call");
            }

            // constant-exit loop
            if (Regex.IsMatch(body, @"exitwhen\s+false\b", RegexOptions.IgnoreCase))
                Flag("loop", name, "exitwhen false, the loop can never exit");

            // a sleep inside a periodic callback stalls the whole map for that duration
            if (callbacks.Contains(name)
                && Regex.IsMatch(body, @"PolledWait\(|TriggerSleepAction\("))
                Flag("sleep-in-loop", name, "PolledWait or TriggerSleepAction inside a periodic callback");

            // bj_lastCreatedUnit is overwritten by any subsequent unit creation, a fragile read
            if (Regex.IsMatch(body, @"bj_lastCreatedUnit"))
                Flag("bj-last", name, "reads bj_lastCreatedUnit, which any later unit creation overwrites");

            // GetTriggerUnit inside a timer callback is null, reads as no unit
            if (callbacks.Contains(name) && Regex.IsMatch(body, @"GetTriggerUnit\(\)"))
                Flag("triggerunit-in-timer", name, "GetTriggerUnit() inside a periodic callback returns null");

            // a dummy given an ability but never given a timed life or removed
            foreach (var d in localDummies)
                if (Regex.IsMatch(body, @"UnitAddAbility\(\s*" + Regex.Escape(d) + @"\s*,")
                    && !Regex.IsMatch(body, @"UnitApplyTimedLife\(\s*" + Regex.Escape(d) + @"\s*,")
                    && !Regex.IsMatch(body, @"RemoveUnit\(\s*" + Regex.Escape(d) + @"\s*\)"))
                    Flag("dummy-no-life", name, $"dummy {d} given an ability with no timed life or remove in this function");
        }

        int periodic = functions.Count(f => timers.ContainsKey(f.Name)
            || Regex.IsMatch(bodies[f.Name], @"TriggerRegisterTimerEventPeriodic\("));
        bool ok = findings.Count == 0;
        string message = ok
            ? $"{bodies.Count} function(s) verified, no finding"
            : $"{findings.Count} finding(s) across {bodies.Count} function(s)";
        return new(ok, message, file, bodies.Count, periodic, findings);
    }

    private static int[] LineStarts(string source)
    {
        var starts = new List<int> { 0 };
        for (int i = 0; i < source.Length; i++)
            if (source[i] == '\n') starts.Add(i + 1);
        return starts.ToArray();
    }
}
