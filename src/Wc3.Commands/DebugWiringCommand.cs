// src/Wc3.Commands/DebugWiringCommand.cs
using System.Text;
using System.Text.RegularExpressions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One dispatch function this pass rewrote, and which checkpoints it added to it.</summary>
public sealed record DebugWiringTarget(string Trigger, string Function, IReadOnlyList<string> Checkpoints);

public sealed record DebugWiringResult(
    bool Ok, string Message, IReadOnlyList<DebugWiringTarget> Targets, IReadOnlyList<string> Diagnostics);

/// <summary>
/// Instruments an already-ported map so the running GAME reports, at cast time, exactly where its
/// spell-dispatch chain stops.
///
/// This exists because <see cref="HeroWiringAudit"/> and <see cref="RuntimeReadinessCommand"/> can
/// both read a hero clean, every ability wired to a trigger, every global assigned, and the hero can
/// still never cast in game. Neither check can see what actually happens at runtime, whether the
/// dispatcher's own gating condition passes for this caster, whether its own per-hero branch is ever
/// entered, or whether the deferred trigger registration a placed hero depends on ran before a
/// player tried to cast. Those are exactly the places static analysis goes blind, so this makes the
/// running map print them instead of adding a fourth static guess.
///
/// Opt in only, meant to be run once on a disposable copy of a map to observe in game. It finds the
/// arena's per-player cast-dispatch trigger the same way <see cref="PreplacedUnitsScript"/> already
/// does (a TriggerRegisterPlayerUnitEvent registering EVENT_PLAYER_UNIT_SPELL_EFFECT per player),
/// then rewrites its condition/action function and, best-effort, the generated
/// wc3ctl_WirePlacedHeroSpells init, with BJDebugMsg calls. Never run as part of an ordinary port,
/// and never touches a map that has none of this shape (a normal port is unaffected).
/// </summary>
public static class DebugWiringCommand
{
    private const string Tag = "[wc3ctl-debug]";

    public static DebugWiringResult Instrument(MapDocument doc, string? heroRawcode = null)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var diagnostics = new List<string>();
        var noTargets = Array.Empty<DebugWiringTarget>();

        var entry = doc.GetFile(PreplacedUnitsScript.ScriptFile) ?? doc.GetFile(@"scripts\war3map.j");
        byte[]? bytes = entry?.CurrentBytes;
        if (entry?.FileName is null || bytes is null || bytes.Length == 0)
            return new(false, "map contains no war3map.j script", noTargets, diagnostics);

        // Latin1 round-trips every byte, matching every other pass that touches this script
        // (PreplacedUnitsScript, HeroWiringAudit, RuntimeReadinessCommand).
        var enc = Encoding.Latin1;
        string jass = enc.GetString(bytes);
        string nl = jass.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";

        var triggers = PreplacedUnitsScript.DetectPerPlayerSpellTriggers(jass);
        if (triggers.Count == 0)
            return new(false,
                "no per-player spell-dispatch trigger found (a TriggerRegisterPlayerUnitEvent registering "
                + "EVENT_PLAYER_UNIT_SPELL_EFFECT per player), nothing here to instrument",
                noTargets, diagnostics);

        string? heroIdGlobal = null;
        if (heroRawcode is not null)
        {
            var aliases = JassRawcodeAliases.Parse(jass);
            heroIdGlobal = aliases.FirstOrDefault(kv => string.Equals(kv.Value, heroRawcode, StringComparison.Ordinal)).Key;
            if (heroIdGlobal is null)
                diagnostics.Add($"no id-global alias resolves to {heroRawcode}, the per-hero branch checkpoint is skipped");
        }

        var targets = new List<DebugWiringTarget>();
        foreach (var trigger in triggers)
        {
            string codeText = string.Join("\n", JassComments.Strip(jass.Replace("\r\n", "\n").Split('\n')));
            string? dispatchFn = FindDispatchFunction(codeText, jass, trigger, diagnostics);
            if (dispatchFn is null) continue;

            var (updated, checkpoints) = InstrumentDispatchFunction(jass, dispatchFn, heroRawcode, heroIdGlobal, diagnostics);
            jass = updated;
            if (checkpoints.Count > 0)
                targets.Add(new DebugWiringTarget(trigger, dispatchFn, checkpoints));
        }

        var (afterInit, initWired) = InstrumentDeferredRegistration(jass, triggers, diagnostics);
        jass = afterInit;
        if (initWired)
            targets.Add(new DebugWiringTarget(
                string.Join(", ", triggers), PreplacedUnitsScript.WireSpellsFunc,
                new[] { "trigger non-null when the deferred timer fires", "each per-player registration confirmed as it runs" }));

        if (targets.Count == 0)
            return new(false,
                "found per-player spell-dispatch trigger(s) but could not instrument anything, see diagnostics",
                targets, diagnostics);

        // Back to the entry it was read from, see ScriptCommand.Write.
        ScriptCommand.Write(doc, enc.GetBytes(NormalizeNewlines(jass, nl)));
        return new(true,
            $"instrumented {targets.Count} function(s) for {triggers.Count} dispatch trigger(s)",
            targets, diagnostics);
    }

    private static string NormalizeNewlines(string jass, string nl) =>
        nl == "\r\n" ? jass.Replace("\r\n", "\n").Replace("\n", "\r\n") : jass.Replace("\r\n", "\n");

    // ---- locating the dispatch function --------------------------------------

    /// <summary>The function attached to <paramref name="trigger"/> that reads GetSpellAbilityId(),
    /// directly or one call away (an inline condition helper is common, the same shallow hop
    /// <see cref="HeroWiringAudit"/> already walks). Null, with a diagnostic, when nothing here
    /// looks like a cast dispatcher.</summary>
    private static string? FindDispatchFunction(string codeText, string jass, string trigger, List<string> diagnostics)
    {
        var attach = Regex.Match(codeText,
            @"TriggerAdd(?:Condition|Action)\s*\(\s*" + Regex.Escape(trigger) + @"\s*,[^)]*\bfunction\s+([A-Za-z_]\w*)");
        if (!attach.Success)
        {
            diagnostics.Add($"trigger {trigger} registers a per-player spell event but nothing attaches a "
                + "condition or action to it, skipped");
            return null;
        }
        string attached = attach.Groups[1].Value;

        var functions = JassFunctionIndex.Parse(jass);
        var lineStarts = ScriptCommand.ComputeLineStarts(jass);
        string? BodyOf(string name)
        {
            var f = functions.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.Ordinal));
            return f is null ? null : ScriptCommand.SliceFunction(jass, lineStarts, f.StartLine, f.EndLine);
        }

        string? body = BodyOf(attached);
        if (body is null)
        {
            diagnostics.Add($"trigger {trigger} attaches {attached}, but no such function is defined, skipped");
            return null;
        }
        if (body.Contains("GetSpellAbilityId", StringComparison.Ordinal))
            return attached;

        foreach (Match call in Regex.Matches(body, @"\b([A-Za-z_]\w*)\s*\("))
        {
            if (string.Equals(call.Groups[1].Value, attached, StringComparison.Ordinal)) continue;
            string? calleeBody = BodyOf(call.Groups[1].Value);
            if (calleeBody is not null && calleeBody.Contains("GetSpellAbilityId", StringComparison.Ordinal))
            {
                diagnostics.Add($"trigger {trigger} attaches {attached}, which calls the real dispatcher "
                    + $"{call.Groups[1].Value}, instrumented that one instead");
                return call.Groups[1].Value;
            }
        }

        diagnostics.Add($"trigger {trigger} attaches {attached}, which does not read GetSpellAbilityId() "
            + "directly or one call away, skipped, nothing here looks like a cast dispatcher");
        return null;
    }

    // ---- instrumenting the dispatch function -----------------------------------

    private static (string Jass, List<string> Checkpoints) InstrumentDispatchFunction(
        string jass, string functionName, string? heroRawcode, string? heroIdGlobal, List<string> diagnostics)
    {
        var checkpoints = new List<string>();
        var functions = JassFunctionIndex.Parse(jass);
        var fn = functions.FirstOrDefault(f => string.Equals(f.Name, functionName, StringComparison.Ordinal));
        if (fn is null) return (jass, checkpoints);

        var lineStarts = ScriptCommand.ComputeLineStarts(jass);
        string body = ScriptCommand.SliceFunction(jass, lineStarts, fn.StartLine, fn.EndLine);
        var lines = body.Replace("\r\n", "\n").Split('\n').ToList();
        var stripped = JassComments.Strip(lines.ToArray());

        // The caster local, "local unit X= GetSpellAbilityUnit()", the idiom every dispatcher in
        // this map family declares first. Absent, the other checkpoints still work, just without
        // a caster-type print at entry.
        string? caster = null;
        int localsEnd = LastLeadingLocalIndex(stripped);
        for (int i = 1; i <= localsEnd; i++)
        {
            var m = Regex.Match(stripped[i].Trim(), @"^local\s+unit\s+(\w+)\s*=\s*GetSpellAbilityUnit\s*\(\s*\)");
            if (m.Success) caster = m.Groups[1].Value;
        }

        string IndentOf(int i) => lines[i][..(lines[i].Length - lines[i].TrimStart().Length)];
        var edits = new List<(int At, List<string> Text)>();

        // 1. Entry, reached before anything else, so a silent hero shows up here first as never
        //    printed at all.
        edits.Add((localsEnd + 1, BuildEntryPrint(functionName, caster)));
        checkpoints.Add("entry (function reached, ability id, caster type)");

        // 2. Hero gate, "if IsUnitType(x, UNIT_TYPE_HERO) and b then".
        var gate = FindFirst(stripped, @"^\s*if\s+IsUnitType\(\s*(\w+)\s*,\s*UNIT_TYPE_HERO\s*\)\s+and\s+(\w+)\s+then\b");
        if (gate is { } g)
        {
            string ind = IndentOf(g.Index);
            string c = g.Match.Groups[1].Value, b = g.Match.Groups[2].Value;
            edits.Add((g.Index, new List<string>
            {
                $"{ind}if IsUnitType({c}, UNIT_TYPE_HERO) then",
                $"{ind}    call BJDebugMsg(\"{Tag} IsUnitType(HERO)=true\")",
                $"{ind}else",
                $"{ind}    call BJDebugMsg(\"{Tag} IsUnitType(HERO)=false\")",
                $"{ind}endif",
                $"{ind}if {b} then",
                $"{ind}    call BJDebugMsg(\"{Tag} {b}=true\")",
                $"{ind}else",
                $"{ind}    call BJDebugMsg(\"{Tag} {b}=false\")",
                $"{ind}endif",
            }));
            checkpoints.Add($"hero gate (IsUnitType HERO, {b})");
        }
        else
        {
            diagnostics.Add($"{functionName}, no 'if IsUnitType(x, UNIT_TYPE_HERO) and b then' gate found, "
                + "that checkpoint is skipped");
        }

        // 3. Area exemption, a CheckCoordsInRect / SpellExtension condition.
        int areaIdx = -1;
        Match? areaMatch = null;
        for (int i = 0; i < stripped.Length; i++)
        {
            if (!stripped[i].Contains("CheckCoordsInRect", StringComparison.Ordinal)) continue;
            var m = Regex.Match(stripped[i], @"^(\s*)if\s+(.*)\bthen\b");
            if (!m.Success) continue;
            areaIdx = i; areaMatch = m; break;
        }
        if (areaIdx >= 0 && areaMatch is not null)
        {
            string ind = IndentOf(areaIdx);
            string cond = areaMatch.Groups[2].Value.TrimEnd();
            var sub = new List<string>();
            // One level of nested parens allowed, CheckCoordsInRect's own arguments are commonly
            // calls themselves (GetUnitX(c), GetUnitY(c)).
            foreach (Match part in Regex.Matches(cond,
                @"\b(?:CheckCoordsInRect|SpellExtension)\s*\((?:[^()]|\([^()]*\))*\)"))
            {
                string expr = part.Value;
                sub.Add($"{ind}if {expr} then");
                sub.Add($"{ind}    call BJDebugMsg(\"{Tag} {expr}=true\")");
                sub.Add($"{ind}else");
                sub.Add($"{ind}    call BJDebugMsg(\"{Tag} {expr}=false\")");
                sub.Add($"{ind}endif");
            }
            sub.Add($"{ind}if {cond} then");
            sub.Add($"{ind}    call BJDebugMsg(\"{Tag} area/extension gate=true\")");
            sub.Add($"{ind}else");
            sub.Add($"{ind}    call BJDebugMsg(\"{Tag} area/extension gate=false\")");
            sub.Add($"{ind}endif");
            edits.Add((areaIdx, sub));
            checkpoints.Add("area exemption (CheckCoordsInRect, SpellExtension)");
        }
        else
        {
            diagnostics.Add($"{functionName}, no CheckCoordsInRect condition found, the area-exemption "
                + "checkpoint is skipped");
        }

        // 4. The requested hero's own dispatch branch, "if GetUnitTypeId(x) == HeroX_ID then".
        if (heroIdGlobal is not null)
        {
            var hero = FindFirst(stripped,
                @"^(\s*)if\s+GetUnitTypeId\(\s*(\w+)\s*\)\s*==\s*" + Regex.Escape(heroIdGlobal) + @"\s+then\b");
            if (hero is { } h)
            {
                string ind = IndentOf(h.Index) + "    ";
                edits.Add((h.Index + 1, new List<string>
                {
                    $"{ind}call BJDebugMsg(\"{Tag} entered {heroIdGlobal} branch for {heroRawcode}, "
                        + $"ability=\" + I2S(GetSpellAbilityId()))",
                }));
                checkpoints.Add($"{heroIdGlobal} branch entered ({heroRawcode})");
            }
            else
            {
                diagnostics.Add($"{functionName}, no 'if GetUnitTypeId(x) == {heroIdGlobal} then' branch found, "
                    + $"{heroRawcode} has no dispatch branch in this function at all");
            }
        }

        // 5. The final dispatch flag, "local integer check= 0" declared first and read at the end.
        string? flag = null;
        if (stripped.Length > 1)
        {
            var f = Regex.Match(stripped[1].Trim(), @"^local\s+integer\s+(\w+)\s*=\s*0\s*$");
            if (f.Success) flag = f.Groups[1].Value;
        }
        if (flag is not null)
        {
            int cleanup = caster is not null
                ? LastIndex(stripped, @"^\s*set\s+" + Regex.Escape(caster) + @"\s*=\s*null\b")
                : -1;
            if (cleanup < 0) cleanup = LastIndex(stripped, @"^\s*return\b");
            if (cleanup >= 0)
            {
                string ind = IndentOf(cleanup);
                edits.Add((cleanup, new List<string>
                {
                    $"{ind}call BJDebugMsg(\"{Tag} {functionName} finished, {flag}=\" + I2S({flag}))",
                }));
                checkpoints.Add($"final dispatch flag ({flag})");
            }
            else
            {
                diagnostics.Add($"{functionName}, no cleanup or return line found to print the final "
                    + $"{flag} value, skipped");
            }
        }
        else
        {
            diagnostics.Add($"{functionName}, first local is not 'local integer x= 0', the final "
                + "dispatch-flag checkpoint is skipped");
        }

        foreach (var (at, text) in edits.OrderByDescending(e => e.At))
            lines.InsertRange(Math.Min(at, lines.Count), text);

        string newJass = ScriptCommand.ReplaceFunction(jass, lineStarts, fn, string.Join("\n", lines));
        return (newJass, checkpoints);
    }

    private static List<string> BuildEntryPrint(string functionName, string? caster) => new()
    {
        caster is null
            ? $"    call BJDebugMsg(\"{Tag} {functionName} entered, ability=\" + I2S(GetSpellAbilityId()))"
            : $"    call BJDebugMsg(\"{Tag} {functionName} entered, ability=\" + I2S(GetSpellAbilityId()) "
              + $"+ \", casterType=\" + I2S(GetUnitTypeId({caster})))",
    };

    // ---- instrumenting the deferred registration -------------------------------

    /// <summary>Prints, from inside the generated wc3ctl_WirePlacedHeroSpells (see
    /// <see cref="PreplacedUnitsScript"/>), whether each dispatch trigger already exists by the
    /// time its 0-second timer fires, and confirms each per-player registration as it runs. A
    /// map with no such generated block (a hand-wired map, or one that never needed it) simply
    /// has nothing here to instrument, which is reported, not treated as a failure.</summary>
    private static (string Jass, bool Wired) InstrumentDeferredRegistration(
        string jass, IReadOnlyList<string> triggers, List<string> diagnostics)
    {
        var functions = JassFunctionIndex.Parse(jass);
        var wireFn = functions.FirstOrDefault(f =>
            string.Equals(f.Name, PreplacedUnitsScript.WireSpellsFunc, StringComparison.Ordinal));
        if (wireFn is null)
        {
            diagnostics.Add($"no {PreplacedUnitsScript.WireSpellsFunc} function found, the deferred "
                + "registration init print is skipped (only a wc3ctl-generated preplaced-widget block has one)");
            return (jass, false);
        }

        var lineStarts = ScriptCommand.ComputeLineStarts(jass);
        string body = ScriptCommand.SliceFunction(jass, lineStarts, wireFn.StartLine, wireFn.EndLine);
        var lines = body.Replace("\r\n", "\n").Split('\n').ToList();
        var stripped = JassComments.Strip(lines.ToArray());

        var prelude = new List<string>();
        foreach (var trg in triggers)
        {
            prelude.Add($"    if {trg} != null then");
            prelude.Add($"        call BJDebugMsg(\"{Tag} {PreplacedUnitsScript.WireSpellsFunc} fired, "
                + $"{trg} is non-null\")");
            prelude.Add("    else");
            prelude.Add($"        call BJDebugMsg(\"{Tag} {PreplacedUnitsScript.WireSpellsFunc} fired, "
                + $"{trg} is NULL\")");
            prelude.Add("    endif");
        }
        int afterLocals = LastLeadingLocalIndex(stripped);
        lines.InsertRange(afterLocals + 1, prelude);

        var reg = new Regex(@"^(\s*)call\s+TriggerRegisterPlayerUnitEvent\s*\(\s*("
            + string.Join("|", triggers.Select(Regex.Escape)) + @")\s*,\s*([^,]+),\s*EVENT_PLAYER_UNIT_SPELL_EFFECT");
        for (int i = lines.Count - 1; i >= 0; i--)
        {
            var m = reg.Match(lines[i]);
            if (!m.Success) continue;
            string ind = m.Groups[1].Value;
            string trg = m.Groups[2].Value;
            string owner = m.Groups[3].Value.Trim().Replace("\"", "'");
            lines.Insert(i + 1,
                $"{ind}call BJDebugMsg(\"{Tag} registered {trg} EVENT_PLAYER_UNIT_SPELL_EFFECT for {owner}\")");
        }

        return (ScriptCommand.ReplaceFunction(jass, lineStarts, wireFn, string.Join("\n", lines)), true);
    }

    // ---- small line-scan helpers ------------------------------------------------

    /// <summary>The last index, among the leading lines of a function body (index 0 is the
    /// signature line itself), that is still a "local ..." declaration. JASS requires every local
    /// declared before the first statement, so this is exactly where a print can be inserted
    /// without moving any locals after a statement.</summary>
    private static int LastLeadingLocalIndex(IReadOnlyList<string> strippedLines)
    {
        int last = 0;
        for (int i = 1; i < strippedLines.Count; i++)
        {
            var t = strippedLines[i].Trim();
            if (t.Length == 0) continue;
            if (!t.StartsWith("local ", StringComparison.Ordinal)) break;
            last = i;
        }
        return last;
    }

    private static (int Index, Match Match)? FindFirst(IReadOnlyList<string> lines, string pattern)
    {
        var re = new Regex(pattern);
        for (int i = 0; i < lines.Count; i++)
        {
            var m = re.Match(lines[i]);
            if (m.Success) return (i, m);
        }
        return null;
    }

    private static int LastIndex(IReadOnlyList<string> lines, string pattern)
    {
        var re = new Regex(pattern);
        for (int i = lines.Count - 1; i >= 0; i--)
            if (re.IsMatch(lines[i])) return i;
        return -1;
    }
}
