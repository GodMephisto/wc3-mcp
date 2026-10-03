// src/Wc3.Commands/RuntimeReadinessCommand.cs
using System.Text;
using System.Text.RegularExpressions;
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>Which readiness check found the problem.</summary>
public enum ReadinessIssue
{
    /// <summary>The script carries GUI globals and GUI trigger handlers, but InitGlobals is
    /// missing, or defined and never called, so every custom starting value it would have set
    /// (a damage coefficient, a range, a duration, a dummy id) silently reads as its type
    /// default instead.</summary>
    GlobalInitMissing,
    /// <summary>A global (udg_ or otherwise, a gg_rct_* region or a hand-written system's own
    /// timer/group/hashtable) that appears, unassigned, somewhere in the script text this hero's
    /// port would carry. Warning severity, not Error, on purpose, the carried text for a hero on a
    /// tightly-coupled arena routinely includes shared framework code every other hero's closure
    /// reaches the same way (a common UI frame system, a mouse handler, a pick-phase state
    /// machine), so a global found this way is not provably THIS hero's own bug, only a lead worth
    /// checking. It found the real gg_rct_Base and GearTimer05 bugs this feature was built for, and
    /// it also found dozens of other heroes' own unrelated frame globals on a real corpus map, at
    /// Error severity that made a script that compiles and round trips byte faithful come out
    /// INVALID, which is worse than under-reporting. The general form of GlobalInitMissing, it also
    /// catches a loss narrower than the whole InitGlobals function, for instance a single value a
    /// port dropped while carrying the rest.</summary>
    GlobalNeverAssigned,
    /// <summary>The script carries GUI trigger handlers, but RunInitializationTriggers is
    /// missing, or defined and never called, so a trigger whose only event is Map Initialization
    /// (it registers no event of its own) never runs.</summary>
    RunInitializationTriggersMissing,
}

/// <summary>One readiness problem. <see cref="Global"/> names the specific variable for
/// <see cref="ReadinessIssue.GlobalNeverAssigned"/>, null for the two script-wide findings.</summary>
public sealed record ReadinessFinding(ReadinessIssue Issue, DiagnosticSeverity Severity, string? Global, string Detail);

public sealed record RuntimeReadinessResult(
    string Hero, string? Name, int OwnerId,
    IReadOnlyList<ReadinessFinding> Findings, IReadOnlyList<string> Diagnostics)
{
    public int Errors => Findings.Count(f => f.Severity == DiagnosticSeverity.Error);
    public bool Ready => Errors == 0;
}

/// <summary>
/// Checks whether a placed hero's script would actually RUN correctly, as opposed to merely being
/// wired up. <see cref="HeroWiringAudit"/> proves a cast reaches a handler, it says nothing about
/// what that handler computes once it runs. A hero can audit at zero wiring problems and still be
/// useless in game if the values its handlers read (damage, range, duration, a dummy id) were
/// never initialized because InitGlobals was not carried by a port. This exists because exactly
/// that happened on a real map and nothing caught it, the hero audited eight of eight abilities
/// wired and still did nothing worth playing.
///
/// Two of the three checks are whole-script facts. InitGlobals and RunInitializationTriggers are
/// generated exactly once per map, under those exact names, the same fixed World Editor
/// convention <see cref="JassScriptCheck"/> already relies on for config and main, both stay Error
/// severity, neither depends on which hero is being checked. The third is scoped to the hero's
/// own dependency closure, the same one <see cref="BundleCommand"/> computes for porting, which
/// narrows a whole-map scan down to what this hero's port would actually carry, cutting a real
/// map's noise down enormously (a prototype that skipped this scoping entirely found problems
/// purely from other heroes' own unrelated leftovers, on a map the user plays without issue). It
/// does not prove exclusivity though, a tightly-coupled arena's closure routinely pulls in shared
/// framework code that every hero's own closure reaches the same way, so this one finding stays at
/// Warning severity, informative rather than verdict-flipping, see
/// <see cref="ReadinessIssue.GlobalNeverAssigned"/> for the real map evidence behind that choice.
/// </summary>
public static class RuntimeReadinessCommand
{
    /// <summary>Checks every distinct hero type placed on the map, reusing exactly the placement
    /// enumeration <see cref="HeroWiringAudit"/> uses, so the two audits can never disagree about
    /// what counts as a placed hero.</summary>
    public static IReadOnlyList<RuntimeReadinessResult> CheckPlacedHeroes(MapDocument doc) =>
        HeroWiringAudit.AuditPlacedHeroes(doc)
            .Select(h => Check(doc, h.Hero, h.OwnerId))
            .ToList();

    /// <summary>Checks one hero type as owned by <paramref name="ownerId"/>.</summary>
    public static RuntimeReadinessResult Check(MapDocument doc, string heroRawcode, int ownerId)
    {
        var diagnostics = new List<string>();
        var findings = new List<ReadinessFinding>();
        string? heroName = ObjectName(doc, ObjectKind.Unit, heroRawcode);

        var entry = doc.GetFile("war3map.j") ?? doc.GetFile("scripts\\war3map.j");
        byte[]? bytes = entry?.CurrentBytes;
        if (entry?.FileName is null || bytes is null || bytes.Length == 0)
        {
            diagnostics.Add(doc.GetFile("war3map.lua") is not null
                ? "map ships war3map.lua, not war3map.j, this check only understands JASS"
                : "map has no war3map.j to check");
            return new(heroRawcode, heroName, ownerId, findings, diagnostics);
        }

        // Latin1 round-trips every byte, matching how ScriptPorter reads and writes the script.
        string jass = Encoding.Latin1.GetString(bytes);
        var lines = jass.Replace("\r\n", "\n").Split('\n');
        var code = JassComments.Strip(lines);
        string codeText = string.Join("\n", code);
        var functions = JassFunctionIndex.Parse(jass);
        var (globalsByName, _) = JassGlobals.Parse(lines);
        var udgGlobals = globalsByName.Keys
            .Where(n => n.StartsWith("udg_", StringComparison.Ordinal)).ToList();
        bool hasGuiHandlers = functions.Any(f => f.Name.StartsWith("InitTrig_", StringComparison.Ordinal));

        // 1. InitGlobals, the fixed name World Editor gives "set every custom starting value".
        //    Only meaningful when the script actually has GUI globals and GUI handlers to init, a
        //    hand-written or vanilla script with neither has nothing for InitGlobals to do, so
        //    a freshly created blank map (which calls neither) never reads as broken.
        if (udgGlobals.Count > 0 && hasGuiHandlers)
        {
            bool defined = functions.Any(f => f.Name == "InitGlobals");
            bool called = Regex.IsMatch(codeText, @"\bcall\s+InitGlobals\s*\(");
            if (!defined || !called)
                findings.Add(new(ReadinessIssue.GlobalInitMissing, DiagnosticSeverity.Error, null,
                    defined
                        ? "InitGlobals is defined but nothing ever calls it, so its custom starting "
                          + $"values never run ({udgGlobals.Count} udg_ global(s) declared)"
                        : $"no InitGlobals function at all, though {udgGlobals.Count} udg_ global(s) "
                          + "and carried GUI trigger handlers are present, so every custom starting "
                          + "value (a damage coefficient, a range, a duration, a dummy id) silently "
                          + "reads as its type default"));
        }

        // 2. RunInitializationTriggers, the other fixed name, the one that actually executes any
        //    GUI trigger whose only event is Map Initialization (such a trigger registers no event
        //    of its own, so nothing else would ever run it).
        if (hasGuiHandlers)
        {
            bool defined = functions.Any(f => f.Name == "RunInitializationTriggers");
            bool called = Regex.IsMatch(codeText, @"\bcall\s+RunInitializationTriggers\s*\(");
            if (!defined || !called)
                findings.Add(new(ReadinessIssue.RunInitializationTriggersMissing, DiagnosticSeverity.Error, null,
                    defined
                        ? "RunInitializationTriggers is defined but nothing ever calls it, so a Map "
                          + "Initialization trigger, which registers no event of its own, never runs"
                        : "no RunInitializationTriggers function at all, so a Map Initialization "
                          + "trigger, which registers no event of its own, never runs"));
        }

        // 3. A global that appears, unassigned, somewhere in the script text this hero's port
        //    would carry. Not scoped to udg_: gg_rct_* (a region CreateRegions never carried) and a
        //    hand-written system's own timer/group/hashtable globals broke exactly this way on a
        //    real map, and nothing but the name distinguishes them from a udg_ one. Scoped to the
        //    hero's OWN dependency closure, which cuts a whole-map scan down a great deal, but a
        //    tightly-coupled arena's closure still routinely pulls in shared framework code every
        //    other hero's closure reaches too (a UI frame system, a mouse handler), so this can
        //    never prove the global is exclusively this hero's, Warning severity reflects exactly
        //    that (see the finding text below for the honest claim). "Assigned" still looks at the
        //    WHOLE script, because the natural home for the assignment, InitGlobals (or a
        //    hand-written system's own Init), is never itself part of a hero's own closure. Shared
        //    with the porter's bootstrap-state feature, see JassGlobals.Assigned, so the two can
        //    never disagree about what counts as an assignment.
        var assigned = JassGlobals.Assigned(globalsByName, codeText);

        IReadOnlyList<BundleFunction> closure;
        try
        {
            closure = BundleCommand.ResolveObject(doc, ObjectKind.Unit, heroRawcode,
                ctx: null, preDiagnostics: Array.Empty<string>()).Functions;
        }
        catch (Exception ex)
        {
            diagnostics.Add($"could not resolve '{heroRawcode}' script closure ({ex.Message}), "
                + "the global-assignment check is skipped");
            closure = Array.Empty<BundleFunction>();
        }

        if (closure.Count > 0)
        {
            var usedByHero = new HashSet<string>(StringComparer.Ordinal);
            foreach (var f in closure)
                for (int i = f.StartLine - 1; i < f.EndLine && i < code.Length; i++)
                    foreach (Match m in Regex.Matches(code[i], @"[A-Za-z_][A-Za-z0-9_]*"))
                        if (globalsByName.ContainsKey(m.Value)) usedByHero.Add(m.Value);

            foreach (var g in usedByHero.Where(g => !assigned.Contains(g)).OrderBy(g => g, StringComparer.Ordinal))
                findings.Add(new(ReadinessIssue.GlobalNeverAssigned, DiagnosticSeverity.Warning, g,
                    $"'{g}' appears, unassigned, in the script text carried for this hero (which can "
                    + "include shared framework code other heroes' closures reach too, so this is "
                    + "not proven to be this hero's own bug) and is assigned nowhere in the whole "
                    + "script, so it can only ever hold its type default (0, false or null), worth "
                    + "checking rather than a confirmed problem"));

            // A second, subtler shape of the same problem. A "set NAME=" statement DOES appear
            // somewhere in the script (so the scan above found it and stayed silent), but its
            // ENCLOSING FUNCTION is never called anywhere, not from main, not from config, not
            // through any chain of calls or callback registrations reachable from either. The
            // assignment is live in the TEXT and dead at RUNTIME, a plain "is this assigned
            // anywhere" scan cannot tell the two apart, it only sees the statement, never whether
            // the function around it ever runs. Measured on the Asta corpus port, this hid three
            // real bugs the whole-script scan missed entirely. GearSystems' own hand-written Init
            // (the only thing that ever assigns GearTimer03/05/10) is reached solely through
            // ExecuteFunc("Init") in the source's main, never carried into the target's own entry
            // points by a synth-dispatch framework prune that correctly judged Init wires nothing
            // the hero calls. NoDecor_Cond, a filter several heroes' own AoE abilities read
            // directly, is assigned only inside the map's mode-selection dialog setup, itself
            // reached only through an 8 second timer a different, equally unwired initializer
            // registers.
            //
            // Reachability is rooted at the script's two fixed World Editor entry points, main and
            // config, the same forward call-graph closure ScriptPorter's synth-dispatch prune uses
            // to decide what the HERO reaches, shared here (ForwardClosure) so the two can never
            // disagree about what "reachable" means. This is a whole-script fact, independent of
            // which hero is being checked, unlike the closure-scoped check above it stays exact,
            // main and config either call something or they do not.
            var allByNameWhole = new Dictionary<string, JassFunction>(StringComparer.Ordinal);
            foreach (var f in functions) allByNameWhole.TryAdd(f.Name, f);
            var bodiesWhole = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var f in allByNameWhole.Values)
            {
                int end = Math.Min(f.EndLine, code.Length);
                bodiesWhole[f.Name] = string.Join('\n', code[(f.StartLine - 1)..end]);
            }
            var reachableFromMain = ScriptPorter.ForwardClosure(
                new[] { "main", "config" }, bodiesWhole, allByNameWhole);

            // One pass over every function's body, not one pass per candidate global, so this
            // stays linear in script size regardless of how many globals need checking.
            var assigningFunctions = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            foreach (var f in allByNameWhole.Values)
                foreach (var line in bodiesWhole[f.Name].Split('\n'))
                {
                    var m = Regex.Match(line.TrimStart(), @"^set\s+([A-Za-z_][A-Za-z0-9_]*)\b");
                    if (!m.Success || !globalsByName.ContainsKey(m.Groups[1].Value)) continue;
                    if (!assigningFunctions.TryGetValue(m.Groups[1].Value, out var list))
                        assigningFunctions[m.Groups[1].Value] = list = new List<string>();
                    list.Add(f.Name);
                }

            foreach (var g in usedByHero.Where(g => assigned.Contains(g))
                .OrderBy(g => g, StringComparer.Ordinal))
            {
                // A real declaration initializer ("hashtable hs= InitHashtable()") always runs, it
                // is not inside any function to be unreachable, nothing further to check.
                if (JassGlobals.HasRealInitializer(globalsByName[g])) continue;
                if (!assigningFunctions.TryGetValue(g, out var fns) || fns.Count == 0) continue;
                if (fns.Any(reachableFromMain.Contains)) continue; // at least one assignment is live
                findings.Add(new(ReadinessIssue.GlobalNeverAssigned, DiagnosticSeverity.Warning, g,
                    $"'{g}' is assigned only inside {string.Join(", ", fns.Distinct(StringComparer.Ordinal))}, "
                    + "which this script never calls (not from main, not from config, not through any "
                    + "chain of calls or callback registrations reachable from either), so the "
                    + "assignment never runs and it can only ever hold its type default (0, false or "
                    + "null), worth checking rather than a confirmed problem"));
            }
        }

        return new(heroRawcode, heroName, ownerId, findings, diagnostics);
    }

    private static string? ObjectName(MapDocument doc, ObjectKind kind, string rawcode)
    {
        var info = ObjectKinds.Info(kind);
        var strings = MapStrings.From(doc);
        int id = rawcode.FromRawcode();
        foreach (var e in ObjectKinds.MergedEntries(doc, info))
            if (e.Id == id && ObjectKinds.DeltaName(ObjectKinds.ModsToDict(e.Mods), info, strings) is { } n)
                return n;
        return null;
    }
}
