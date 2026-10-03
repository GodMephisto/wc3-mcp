// src/Wc3.Commands/AbilityAuditCommand.cs
using System.Text;
using System.Text.RegularExpressions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>Which link of an ability's RUNTIME chain this check covers. <see cref="HeroWiringAudit"/>
/// only proves a cast reaches SOME live trigger, none of the other seven, whether the handler it
/// reaches still has a body, whether a follow-up ability's loop actually ticks, whether anything in
/// the chain deals damage, whether a pause is ever matched by an unpause, whether a timer callback
/// still points at a declared function, whether a global the chain reads is ever assigned, whether a
/// visual effect's asset still exists. A hero can be "8 of 8 wired" by <see cref="HeroWiringAudit"/>
/// and still be unplayable, which is exactly what happened on a real map and is why this exists.</summary>
public enum AbilityCheck { Dispatch, Handler, Loop, Damage, PauseBalance, TimerCallbacks, State, Effects }

/// <summary><see cref="CheckVerdict.NotApplicable"/> is not a soft PASS, it means this tool could not
/// determine an answer (an unsupported dispatch shape, or a passive with no runtime chain at all),
/// and must never be rendered or counted as verified.</summary>
public enum CheckVerdict { Pass, Fail, NotApplicable }

public sealed record AbilityCheckResult(AbilityCheck Check, CheckVerdict Verdict, string Detail);

public sealed record AbilityAuditRow(string Ability, string? Name, IReadOnlyList<AbilityCheckResult> Checks)
{
    public bool Pass => Checks.All(c => c.Verdict != CheckVerdict.Fail);
    public IReadOnlyList<AbilityCheckResult> Failures =>
        Checks.Where(c => c.Verdict == CheckVerdict.Fail).ToList();
}

public sealed record AbilityAuditResult(string Hero, string? Name, int OwnerId, IReadOnlyList<AbilityAuditRow> Abilities)
{
    public int Passed => Abilities.Count(a => a.Pass);
    public int Total => Abilities.Count;
}

/// <summary>
/// Per-ability verdicts, not just per-ability dispatch. <see cref="HeroWiringAudit"/> answers "does a
/// cast reach a live trigger", which is necessary but nowhere near sufficient, a real map audited
/// 8 of 8 wired there and still did nothing worth playing, because the handler it reached was gutted,
/// its follow-up loop never started, its damage call was trimmed, or a global it read was never
/// assigned. This walks each ability's own runtime chain (never the whole hero's carried closure,
/// which routinely includes shared framework other abilities reach too) and reports PASS or the exact
/// broken link for eight checks: <see cref="AbilityCheck.Dispatch"/>, <see cref="AbilityCheck.Handler"/>,
/// <see cref="AbilityCheck.Loop"/>, <see cref="AbilityCheck.Damage"/>, <see cref="AbilityCheck.PauseBalance"/>,
/// <see cref="AbilityCheck.TimerCallbacks"/>, <see cref="AbilityCheck.State"/>, <see cref="AbilityCheck.Effects"/>.
///
/// Reuses rather than re-parses: the ability set and its dispatch verdict come straight from
/// <see cref="HeroWiringAudit"/> (extended here with <see cref="WiringStatus.MultipleLiveDispatchers"/>,
/// two independently live triggers for the same ability, exactly how the --synth-dispatch double
/// registration bug would have been caught automatically). The Shape A/B "never assigned" state check
/// comes from <see cref="RuntimeReadinessCommand"/>, scoped down from the whole hero to this one
/// ability. The ability's own runtime chain, which handler its dispatch calls, comes from
/// <see cref="SynthDispatchBuilder"/>'s branch reader, either <see cref="SynthDispatchBuilder.ExtractHeroCastBranches"/>
/// (a plain port still carries the source's hero-guarded shared dispatcher) or
/// <see cref="SynthDispatchBuilder.ExtractBranchesFromDispatchFunction"/> (a --synth-dispatch port's
/// own bare dispatcher, already scoped to one hero). Everything reachable from there is walked with
/// <see cref="ScriptPorter.ForwardClosure"/>, the exact reachability <see cref="RuntimeReadinessCommand"/>'s
/// Shape B check and the synth-dispatch framework prune already use, so this can never disagree with
/// them about what "reachable" means.
///
/// Works on any ported map, any hero, any dispatch shape this project's other tools already
/// recognise, nothing here is Asta- or WOS2-specific.
/// </summary>
public static class AbilityAuditCommand
{
    public static IReadOnlyList<AbilityAuditResult> AuditPlacedHeroes(MapDocument doc) =>
        HeroWiringAudit.AuditPlacedHeroes(doc).Select(h => Audit(doc, h.Hero, h.OwnerId)).ToList();

    public static AbilityAuditResult Audit(MapDocument doc, string heroRawcode, int ownerId)
    {
        var wiring = HeroWiringAudit.Audit(doc, heroRawcode, ownerId);
        var readiness = RuntimeReadinessCommand.Check(doc, heroRawcode, ownerId);
        var readinessByGlobal = new Dictionary<string, ReadinessFinding>(StringComparer.Ordinal);
        foreach (var f in readiness.Findings)
            if (f.Global is not null) readinessByGlobal.TryAdd(f.Global, f);

        var entry = doc.GetFile(PreplacedUnitsScript.ScriptFile);
        byte[]? bytes = entry?.CurrentBytes;
        string jass = bytes is { Length: > 0 } ? Encoding.Latin1.GetString(bytes) : "";
        var sv = new ScriptView(jass);
        var branches = ResolveBranches(sv, heroRawcode);

        var rows = new List<AbilityAuditRow>();
        foreach (var a in wiring.Abilities)
        {
            rows.Add(a.Status == WiringStatus.NotCastDispatched
                ? NotApplicableRow(a, "passive/aura, not cast-dispatched, no runtime chain to audit")
                : BuildRow(a, branches.GetValueOrDefault(a.Ability), sv, readinessByGlobal, doc));
        }
        return new(heroRawcode, wiring.Name, ownerId, rows);
    }

    // ---- per-ability wiring --------------------------------------------------

    private static readonly AbilityCheck[] AllChecks =
        { AbilityCheck.Dispatch, AbilityCheck.Handler, AbilityCheck.Loop, AbilityCheck.Damage,
          AbilityCheck.PauseBalance, AbilityCheck.TimerCallbacks, AbilityCheck.State, AbilityCheck.Effects };

    private static AbilityAuditRow NotApplicableRow(AbilityWiring a, string reason) =>
        new(a.Ability, a.Name, AllChecks.Select(c => new AbilityCheckResult(c, CheckVerdict.NotApplicable, reason)).ToList());

    private static AbilityAuditRow BuildRow(
        AbilityWiring a, SynthDispatchBuilder.CastBranch? branch, ScriptView sv,
        IReadOnlyDictionary<string, ReadinessFinding> readinessByGlobal, MapDocument doc)
    {
        var checks = new List<AbilityCheckResult>
        {
            // 1. Dispatch: HeroWiringAudit's own verdict, unmodified. Ok is the only PASS, every other
            //    status (including the new MultipleLiveDispatchers) is a real broken or doubled link.
            new(AbilityCheck.Dispatch,
                a.Status == WiringStatus.Ok ? CheckVerdict.Pass : CheckVerdict.Fail,
                a.Status == WiringStatus.Ok ? a.Detail : $"{a.Status}: {a.Detail}"),
        };

        if (branch is null)
        {
            const string reason = "this ability's own cast branch could not be isolated in the ported "
                + "script (unsupported dispatch shape), so its handler/loop/damage/pause/callback/state/"
                + "effect chain cannot be verified";
            foreach (var c in AllChecks.Where(c => c != AbilityCheck.Dispatch))
                checks.Add(new(c, CheckVerdict.NotApplicable, reason));
            return new(a.Ability, a.Name, checks);
        }

        var reachable = ScriptPorter.ForwardClosure(branch.Callees, sv.Bodies, sv.ByName);

        checks.Add(HandlerCheck(branch, sv));
        checks.Add(LoopCheck(branch, sv));
        checks.Add(DamageCheck(reachable, sv));
        checks.Add(PauseBalanceCheck(reachable, sv));
        checks.Add(TimerCallbackCheck(reachable, sv));
        checks.Add(StateCheck(reachable, readinessByGlobal, sv));
        checks.Add(EffectsCheck(reachable, sv, doc));

        return new(a.Ability, a.Name, checks);
    }

    /// <summary>Every ability branch the ported script's dispatcher(s) name, however they carried
    /// forward: <see cref="SynthDispatchBuilder.ExtractHeroCastBranches"/> for a plain port (still
    /// carrying the source's hero-guarded shared dispatcher), plus every <c>wc3ctl_SynthCast_*</c>
    /// dispatch function for a --synth-dispatch one (bare, already scoped to one hero, so no guard
    /// search applies). First writer wins on a rawcode collision between the two, which only matters
    /// when a map somehow carries both shapes for the same ability.</summary>
    private static Dictionary<string, SynthDispatchBuilder.CastBranch> ResolveBranches(ScriptView sv, string heroRawcode)
    {
        var map = new Dictionary<string, SynthDispatchBuilder.CastBranch>(StringComparer.Ordinal);
        if (SynthDispatchBuilder.ExtractHeroCastBranches(sv.Jass, heroRawcode) is { } guarded)
            foreach (var b in guarded) map.TryAdd(b.AbilityRawcode, b);

        foreach (var name in sv.ByName.Keys
                     .Where(n => n.StartsWith("wc3ctl_SynthCast_", StringComparison.Ordinal)
                              && !n.EndsWith("Init", StringComparison.Ordinal)))
            if (SynthDispatchBuilder.ExtractBranchesFromDispatchFunction(sv.Jass, name) is { } synth)
                foreach (var b in synth) map.TryAdd(b.AbilityRawcode, b);

        return map;
    }

    // ---- 2. handler carried and not gutted -----------------------------------

    private static AbilityCheckResult HandlerCheck(SynthDispatchBuilder.CastBranch branch, ScriptView sv)
    {
        if (branch.Callees.Count == 0)
            return new(AbilityCheck.Handler, CheckVerdict.Fail,
                "the dispatch branch calls nothing, there is no handler to carry");

        var missing = branch.Callees.Where(c => !sv.ByName.ContainsKey(c)).ToList();
        if (missing.Count > 0)
            return new(AbilityCheck.Handler, CheckVerdict.Fail,
                $"handler(s) not carried: {string.Join(", ", missing)}");

        var gutted = branch.Callees
            .Select(c => (Name: c, Trimmed: CountTrimMarkers(sv.Bodies[c])))
            .Where(x => x.Trimmed > 0).ToList();
        if (gutted.Count > 0)
            return new(AbilityCheck.Handler, CheckVerdict.Fail,
                "handler(s) gutted: " + string.Join(", ", gutted.Select(g => $"{g.Name} ({g.Trimmed} trimmed line(s))")));

        return new(AbilityCheck.Handler, CheckVerdict.Pass,
            $"handler(s) {string.Join(", ", branch.Callees)} carried, no trimmed lines");
    }

    // ---- 3. follow-up loop actually started -----------------------------------

    private static readonly Regex LoopStart = new(
        @"TimerStart\s*\([^,]+,[^,]+,\s*true\s*,\s*function\s+([A-Za-z_][A-Za-z0-9_]*)\s*\)",
        RegexOptions.Compiled);

    private static AbilityCheckResult LoopCheck(SynthDispatchBuilder.CastBranch branch, ScriptView sv)
    {
        var found = new List<(string Loop, bool LineTrimmed, bool Declared)>();
        foreach (var callee in branch.Callees.Where(sv.ByName.ContainsKey))
            foreach (var rawLine in sv.Bodies[callee].Split('\n'))
            {
                bool trimmed = rawLine.StartsWith(JassScriptCheck.TrimMarker, StringComparison.Ordinal);
                string probe = trimmed ? rawLine[JassScriptCheck.TrimMarker.Length..] : rawLine;
                var m = LoopStart.Match(probe);
                if (!m.Success) continue;
                string loopFn = m.Groups[1].Value;
                found.Add((loopFn, trimmed, sv.ByName.ContainsKey(loopFn)));
            }

        if (found.Count == 0)
            return new(AbilityCheck.Loop, CheckVerdict.Pass, "no follow-up loop in this ability's own handler(s)");

        var broken = found.Where(f => f.LineTrimmed || !f.Declared).ToList();
        if (broken.Count > 0)
            return new(AbilityCheck.Loop, CheckVerdict.Fail, string.Join("; ", broken.Select(b =>
                b.LineTrimmed
                    ? $"the TimerStart that would start {b.Loop} was trimmed (its callback was not carried)"
                    : $"{b.Loop} is never declared, the loop handler was not carried")));

        return new(AbilityCheck.Loop, CheckVerdict.Pass,
            $"loop(s) {string.Join(", ", found.Select(f => f.Loop).Distinct())} started live and carried");
    }

    // ---- 4. damage --------------------------------------------------------------

    private static readonly Regex DamageCall = new(
        @"\b(dmgphys|dmgatk|UnitDamageTarget|DelayedDamage)\s*\(", RegexOptions.Compiled);

    private static AbilityCheckResult DamageCheck(HashSet<string> reachable, ScriptView sv)
    {
        string? guttedCallee = null;
        foreach (var fn in reachable.Where(sv.Bodies.ContainsKey))
            foreach (var code in ScriptPorter.LiveCode(sv.Bodies[fn]))
            {
                var m = DamageCall.Match(code);
                if (!m.Success) continue;
                string callee = m.Groups[1].Value;
                bool usable = !sv.ByName.ContainsKey(callee) || HasAnyLiveStatement(sv.Bodies[callee]);
                if (usable)
                    return new(AbilityCheck.Damage, CheckVerdict.Pass, $"DAMAGE YES, {fn} calls {callee}(...) live");
                guttedCallee ??= callee;
            }

        if (guttedCallee is not null)
            return new(AbilityCheck.Damage, CheckVerdict.Fail,
                $"DAMAGE NO, the call reaches {guttedCallee} but its own body is gutted (no live statements)");

        bool trimmedCallSeen = AnyTrimmedLineMatches(reachable, sv, DamageCall);
        return new(AbilityCheck.Damage, CheckVerdict.Fail, trimmedCallSeen
            ? "DAMAGE NO, the call to a damage path in this ability's own code was trimmed (its callee was not carried)"
            : "DAMAGE NO, no damage call (dmgphys/dmgatk/UnitDamageTarget/DelayedDamage) is reachable "
              + "from this ability's own handler(s)");
    }

    // ---- 5. pause balance ---------------------------------------------------

    private static readonly Regex StartPause = new(@"\bStartSpellUnit2?\s*\(", RegexOptions.Compiled);
    private static readonly Regex StopPause = new(@"\bStopSpellUnit2?\s*\(", RegexOptions.Compiled);

    private static AbilityCheckResult PauseBalanceCheck(HashSet<string> reachable, ScriptView sv)
    {
        int starts = 0, stops = 0;
        foreach (var fn in reachable.Where(sv.Bodies.ContainsKey))
            foreach (var code in ScriptPorter.LiveCode(sv.Bodies[fn]))
            {
                starts += StartPause.Matches(code).Count;
                stops += StopPause.Matches(code).Count;
            }

        if (starts == 0 && stops == 0)
            return new(AbilityCheck.PauseBalance, CheckVerdict.Pass,
                "no StartSpellUnit/StopSpellUnit in this ability's own code");
        if (starts == stops)
            return new(AbilityCheck.PauseBalance, CheckVerdict.Pass, $"{starts} pause(s), {stops} unpause(s), balanced");

        return new(AbilityCheck.PauseBalance, CheckVerdict.Fail,
            starts > stops
                ? $"{starts} pause(s) but only {stops} live unpause(s), the caster can be left paused "
                  + "forever (a StopSpellUnit likely sits in a loop or callback this ability's own live "
                  + "code never actually reaches, see the Loop check)"
                : $"{stops} unpause(s) but only {starts} live pause(s), an unpause with no matching pause");
    }

    // ---- 6. timer / callback registrations ------------------------------------

    private static readonly Regex SetCallback = new(
        @"\bset\s+[A-Za-z_][A-Za-z0-9_]*\s*=\s*function\s+([A-Za-z_][A-Za-z0-9_]*)", RegexOptions.Compiled);
    private static readonly Regex TimerStartCallback = new(
        @"\bTimerStart\s*\([^)]*function\s+([A-Za-z_][A-Za-z0-9_]*)\s*\)", RegexOptions.Compiled);

    private static AbilityCheckResult TimerCallbackCheck(HashSet<string> reachable, ScriptView sv)
    {
        var problems = new List<string>();
        var ok = new List<string>();
        foreach (var fn in reachable.Where(sv.Bodies.ContainsKey))
            foreach (var rawLine in sv.Bodies[fn].Split('\n'))
            {
                bool trimmed = rawLine.StartsWith(JassScriptCheck.TrimMarker, StringComparison.Ordinal);
                string probe = trimmed ? rawLine[JassScriptCheck.TrimMarker.Length..] : rawLine;
                foreach (var callback in SetCallback.Matches(probe).Cast<Match>()
                             .Concat(TimerStartCallback.Matches(probe).Cast<Match>())
                             .Select(m => m.Groups[1].Value))
                {
                    if (trimmed) { problems.Add($"{callback} (assignment trimmed)"); continue; }
                    if (!sv.ByName.ContainsKey(callback)) { problems.Add($"{callback} (undeclared)"); continue; }
                    ok.Add(callback);
                }
            }

        if (problems.Count > 0)
            return new(AbilityCheck.TimerCallbacks, CheckVerdict.Fail,
                "callback(s) broken: " + string.Join(", ", problems.Distinct()));
        return ok.Count > 0
            ? new(AbilityCheck.TimerCallbacks, CheckVerdict.Pass,
                $"callback(s) {string.Join(", ", ok.Distinct())} declared and live")
            : new(AbilityCheck.TimerCallbacks, CheckVerdict.Pass, "no timer-callback assignment in this ability's own code");
    }

    // ---- 7. state (globals read but never really assigned, Shape A/B) --------

    private static AbilityCheckResult StateCheck(
        HashSet<string> reachable, IReadOnlyDictionary<string, ReadinessFinding> readinessByGlobal, ScriptView sv)
    {
        var idents = new HashSet<string>(StringComparer.Ordinal);
        foreach (var fn in reachable.Where(sv.Bodies.ContainsKey))
            foreach (var code in ScriptPorter.LiveCode(sv.Bodies[fn]))
                foreach (Match m in Identifier.Matches(code))
                    idents.Add(m.Value);

        var mine = readinessByGlobal.Values.Where(f => idents.Contains(f.Global!))
            .OrderBy(f => f.Global, StringComparer.Ordinal).ToList();
        if (mine.Count == 0)
            return new(AbilityCheck.State, CheckVerdict.Pass, "no unassigned global read by this ability's own code");

        // Names only, not RuntimeReadinessCommand's full per-global essay (repeated per ability, that
        // reads as noise): run 'audit readiness' for the full Shape A/B reasoning behind each one.
        return new(AbilityCheck.State, CheckVerdict.Fail,
            $"{mine.Count} unassigned global(s) reachable from this ability's own code: "
            + string.Join(", ", mine.Select(f => f.Global)));
    }

    // ---- 8. special effects -----------------------------------------------------

    private static readonly Regex EffectCall = new(
        @"\b(AddSpecialEffect|AddSpecialEffectTarget|AddSpecialEffectLoc|EffectSpawn[A-Za-z]*)\s*\(",
        RegexOptions.Compiled);
    private static readonly Regex StringLiteral = new("\"([^\"]*)\"", RegexOptions.Compiled);

    private static AbilityCheckResult EffectsCheck(HashSet<string> reachable, ScriptView sv, MapDocument doc)
    {
        string? absentAsset = null;
        foreach (var fn in reachable.Where(sv.Bodies.ContainsKey))
            foreach (var code in ScriptPorter.LiveCode(sv.Bodies[fn]))
            {
                if (!EffectCall.IsMatch(code)) continue;
                var pathMatch = StringLiteral.Match(code);
                if (!pathMatch.Success) continue; // path built dynamically, cannot check statically
                // Un-escape the doubled backslash a JASS literal always writes for one real path
                // separator, or a genuinely present asset looks absent because the raw source
                // spelling never matches any file name the map actually stores.
                string path = AssetPathCandidates.Unescape(pathMatch.Groups[1].Value);

                // Only a path shaped like this project's own custom-import convention is checked
                // against the map's files, a base-game reference (Doodads\, Abilities\, ...) never
                // lives in the map and absence there means nothing, see PortCommand's own
                // "PresentInMap" convention, which this mirrors rather than re-deriving.
                bool looksCustom = path.Contains("war3mapImported", StringComparison.OrdinalIgnoreCase);
                if (!looksCustom)
                    return new(AbilityCheck.Effects, CheckVerdict.Pass, $"effect asset '{path}' is a base-game reference");
                if (RenderModelCommand.FindAssetEntry(doc, path) is not null)
                    return new(AbilityCheck.Effects, CheckVerdict.Pass, $"effect asset '{path}' present in the map");
                absentAsset ??= path;
            }

        if (absentAsset is not null)
            return new(AbilityCheck.Effects, CheckVerdict.Fail, $"effect asset '{absentAsset}' is absent from the map");

        bool trimmedCallSeen = AnyTrimmedLineMatches(reachable, sv, EffectCall);
        return trimmedCallSeen
            ? new(AbilityCheck.Effects, CheckVerdict.Fail,
                "an effect call in this ability's own code was trimmed (its context was not carried)")
            : new(AbilityCheck.Effects, CheckVerdict.Pass, "no special-effect call in this ability's own code");
    }

    // ---- shared helpers -------------------------------------------------------

    private static readonly Regex Identifier = new(@"[A-Za-z_][A-Za-z0-9_]*", RegexOptions.Compiled);

    private static int CountTrimMarkers(string body)
    {
        int count = 0, i = 0;
        while ((i = body.IndexOf(JassScriptCheck.TrimMarker, i, StringComparison.Ordinal)) >= 0)
        { count++; i += JassScriptCheck.TrimMarker.Length; }
        return count;
    }

    /// <summary>True once a body has any statement beyond a bare local declaration, the same "is
    /// this handler actually doing something" test <see cref="HeroWiringAudit"/> runs on a whole
    /// trigger, reused here on one callee.</summary>
    private static bool HasAnyLiveStatement(string body)
    {
        foreach (var code in ScriptPorter.LiveCode(body))
        {
            var t = code.Trim();
            if (t.Length == 0 || t.StartsWith("local ", StringComparison.Ordinal)) continue;
            return true;
        }
        return false;
    }

    private static bool AnyTrimmedLineMatches(HashSet<string> reachable, ScriptView sv, Regex pattern) =>
        reachable.Where(sv.Bodies.ContainsKey)
            .SelectMany(fn => sv.Bodies[fn].Split('\n'))
            .Where(l => l.StartsWith(JassScriptCheck.TrimMarker, StringComparison.Ordinal))
            .Any(l => pattern.IsMatch(l[JassScriptCheck.TrimMarker.Length..]));

    /// <summary>The ported script parsed once per hero: every declared function's RAW body (trim
    /// markers still visible, unlike <see cref="HeroWiringAudit"/>'s own comment-stripped view, this
    /// tool needs to SEE what was trimmed, not just skip past it).</summary>
    private sealed class ScriptView
    {
        public string Jass { get; }
        public Dictionary<string, JassFunction> ByName { get; }
        public Dictionary<string, string> Bodies { get; }

        public ScriptView(string jass)
        {
            Jass = jass;
            var lines = jass.Replace("\r\n", "\n").Split('\n');
            ByName = new(StringComparer.Ordinal);
            foreach (var f in JassFunctionIndex.Parse(jass)) ByName.TryAdd(f.Name, f);
            Bodies = new(StringComparer.Ordinal);
            foreach (var f in ByName.Values) Bodies[f.Name] = ScriptPorter.BodyText(lines, f);
        }
    }
}
