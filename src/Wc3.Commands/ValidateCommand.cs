using Wc3.Model;
using War3Net.Build.Info;

namespace Wc3.Commands;

/// <summary>
/// Static, read-only sanity check ("lint") over a loaded map. Surfaces the loader's
/// own diagnostics plus missing/empty required files and obviously-wrong map-info
/// values. A map is "valid" when it has zero Error-severity issues; warnings never
/// flip the verdict. Pure over <see cref="MapDocument"/> — writes nothing.
/// </summary>
public static class ValidateCommand
{
    // A runnable, playable map is expected to contain these:
    //   war3map.w3i - map info header (name, players, bounds)
    //   war3map.w3e - terrain / environment
    private static readonly string[] RequiredFiles = { "war3map.w3i", "war3map.w3e" };

    // A map needs at least one of these to run at all.
    private static readonly string[] ScriptFiles = { "war3map.j", "war3map.lua" };

    // Files the map cannot run without — an empty one of these is fatal, not cosmetic.
    private static bool IsCritical(string fileName) =>
        RequiredFiles.Contains(fileName, StringComparer.OrdinalIgnoreCase)
        || ScriptFiles.Contains(fileName, StringComparer.OrdinalIgnoreCase);

    public static ValidateResult Execute(MapDocument doc)
    {
        var issues = new List<ValidationIssue>();

        // 1. Surface the loader's own diagnostics (unreadable / unparseable entries).
        foreach (var d in doc.Diagnostics)
            issues.Add(new ValidationIssue(d.Severity, "loader", d.FileName, d.Message));

        // 2. Required files must be present.
        foreach (var f in RequiredFiles)
            if (doc.GetFile(f) is null)
                issues.Add(new ValidationIssue(
                    DiagnosticSeverity.Error, "missing-file", f, $"required file '{f}' is missing"));

        // 3. The map must carry at least one script.
        if (!ScriptFiles.Any(s => doc.GetFile(s) is not null))
            issues.Add(new ValidationIssue(
                DiagnosticSeverity.Error, "missing-file", "war3map.j",
                "map has no script file (war3map.j or war3map.lua); it cannot run"));

        // 4. Empty named files are almost always a packaging mistake — and fatal when the
        //    empty entry is one the map needs to run (a required file or its only script).
        // RawSize rather than RawBytes.Length. "Is this entry empty" is a question the archive's
        // block table already answers, and reading the bytes to find out decompresses every entry
        // in the map for nothing.
        foreach (var e in doc.Files)
            if (e.FileName is not null && e.RawSize == 0)
            {
                bool critical = IsCritical(e.FileName);
                issues.Add(new ValidationIssue(
                    critical ? DiagnosticSeverity.Error : DiagnosticSeverity.Warning,
                    "empty-file", e.FileName,
                    critical
                        ? $"critical file '{e.FileName}' is present but empty (0 bytes); the map cannot run"
                        : "internal file is empty (0 bytes)"));
            }

        // 5. Map-info sanity (only when war3map.w3i parsed successfully).
        if (doc.GetFile("war3map.w3i")?.Model is MapInfo info)
        {
            if (string.IsNullOrWhiteSpace(info.MapName))
                issues.Add(new ValidationIssue(
                    DiagnosticSeverity.Warning, "map-info", "war3map.w3i", "map name is blank"));

            if ((info.Players?.Count ?? 0) == 0)
                issues.Add(new ValidationIssue(
                    DiagnosticSeverity.Warning, "map-info", "war3map.w3i", "map declares no players"));

            // Duplicate player slot ids confuse the engine's player table.
            if (info.Players is { } players)
                foreach (var dup in players.GroupBy(p => p.Id).Where(g => g.Count() > 1))
                    issues.Add(new ValidationIssue(
                        DiagnosticSeverity.Warning, "map-info", "war3map.w3i",
                        $"duplicate player id {dup.Key} ({dup.Count()} entries)"));

            if (info.PlayableMapAreaWidth <= 0 || info.PlayableMapAreaHeight <= 0)
                issues.Add(new ValidationIssue(
                    DiagnosticSeverity.Warning, "map-info", "war3map.w3i",
                    $"non-positive playable area ({info.PlayableMapAreaWidth}x{info.PlayableMapAreaHeight})"));
        }

        // 6. The script must still compile. A single undeclared variable fails the whole war3map.j,
        //    so config() never runs and a hosted map shows no player slots. That is the failure a
        //    port can introduce silently, so it is checked here rather than discovered in a lobby.
        issues.AddRange(ScriptIssues(doc));

        // 7. The generated spawn block must still carry the behaviour this build knows about. A block
        //    left behind by an older wc3ctl still compiles and still hosts, so nothing above can see
        //    it, yet its placed heroes cannot cast a single dispatched spell.
        issues.AddRange(GeneratedBlockIssues(doc));

        // 8. A placed hero can wire every ability end to end and still do nothing worth playing if
        //    the values its handlers read were never initialized, InitGlobals or
        //    RunInitializationTriggers dropped by a port, or a single global carried across without
        //    the assignment that used to set it. Nothing above reads VALUES, only structure, so this
        //    is the only check that would have caught a real map that shipped exactly this broken.
        issues.AddRange(ReadinessIssues(doc));

        return Summarize(issues);
    }

    /// <summary>
    /// Folds a pjass run into the verdict. pjass is the game's own parser, so a script it rejects
    /// cannot load, and that has to reach the VERDICT and not only the exit code. It did not, and
    /// the result was validate printing "OK, valid (0 error(s), 0 warning(s))" one line above
    /// eleven undefined-function errors, on a ported map that could never host. Anyone who read the
    /// headline and stopped was told the exact opposite of the truth.
    ///
    /// A run that could not happen (no install, no pjass, a timeout) is not a failure. That is what
    /// <see cref="PjassResult.Ran"/> is for, and <see cref="JassScriptCheck"/> stays the guaranteed
    /// line of defense in that case.
    /// </summary>
    public static ValidateResult WithPjass(ValidateResult result, PjassResult? pjass)
    {
        if (pjass is null || !pjass.Ran) return result;

        var issues = new List<ValidationIssue>(result.Issues);
        foreach (var e in pjass.Errors)
            issues.Add(new ValidationIssue(DiagnosticSeverity.Error, "pjass", "war3map.j", e));
        foreach (var w in pjass.Warnings)
            issues.Add(new ValidationIssue(DiagnosticSeverity.Warning, "pjass", "war3map.j", w));
        return Summarize(issues);
    }

    private static ValidateResult Summarize(List<ValidationIssue> issues)
    {
        int errors = issues.Count(i => i.Severity == DiagnosticSeverity.Error);
        int warnings = issues.Count(i => i.Severity == DiagnosticSeverity.Warning);
        return new ValidateResult(errors == 0, errors, warnings, issues);
    }

    /// <summary>Reports a generated spawn block that an older wc3ctl downgraded. The missing
    /// spell wiring is an Error because its effect is total (no placed hero can cast) while the map
    /// looks perfectly healthy otherwise. A merely older block that does not need the wiring is a
    /// Warning, since it still behaves correctly.</summary>
    private static IEnumerable<ValidationIssue> GeneratedBlockIssues(MapDocument doc)
    {
        var audit = PreplacedUnitsScript.Audit(doc);
        if (!audit.HasBlock) yield break;

        if (audit.IsMissingSpellWiring)
            yield return new ValidationIssue(DiagnosticSeverity.Error, "generated-block",
                PreplacedUnitsScript.ScriptFile,
                "the generated spawn block does not register this map's spell-dispatch triggers, so "
                + "placed heroes cannot cast. It was written by an older wc3ctl "
                + $"(gen v{audit.BlockVersion} vs v{audit.CurrentVersion}). Run 'wc3ctl place sync' to regenerate it.");
        else if (audit.IsStale)
            yield return new ValidationIssue(DiagnosticSeverity.Warning, "generated-block",
                PreplacedUnitsScript.ScriptFile,
                $"the generated spawn block is from an older wc3ctl (gen v{audit.BlockVersion} vs "
                + $"v{audit.CurrentVersion}). Run 'wc3ctl place sync' to bring it up to date.");
    }

    /// <summary>Runs <see cref="RuntimeReadinessCommand"/> over every placed hero, mapped onto the
    /// shared validation shape. The two script-wide findings (InitGlobals, RunInitializationTriggers)
    /// are the same fact for every hero on the map, so each is only reported once here rather than
    /// once per hero, the per-hero findings (an unassigned global) are reported per hero since a
    /// dropped value can differ hero to hero.</summary>
    private static IEnumerable<ValidationIssue> ReadinessIssues(MapDocument doc)
    {
        var entry = doc.GetFile("war3map.j") ?? doc.GetFile("scripts\\war3map.j");
        string fileName = entry?.FileName ?? "war3map.j";
        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (var r in RuntimeReadinessCommand.CheckPlacedHeroes(doc))
            foreach (var f in r.Findings)
            {
                string dedupeKey = f.Global is null ? $"{f.Issue}" : $"{f.Issue}|{r.Hero}|{f.Global}";
                if (!seen.Add(dedupeKey)) continue;
                yield return new ValidationIssue(f.Severity, "runtime-readiness", fileName,
                    f.Global is null ? f.Detail : $"{r.Hero} \"{r.Name}\", {f.Detail}");
            }
    }

    /// <summary>Runs <see cref="JassScriptCheck"/> over the map's JASS, mapped onto the shared
    /// validation shape. Lua maps are not analyzed (no Lua checker yet), which reads as no issues.</summary>
    private static IEnumerable<ValidationIssue> ScriptIssues(MapDocument doc)
    {
        var entry = doc.GetFile("war3map.j") ?? doc.GetFile("scripts\\war3map.j");
        byte[]? bytes = entry?.CurrentBytes;
        if (entry?.FileName is null || bytes is null || bytes.Length == 0) yield break;

        // Latin1 round-trips every byte, matching how ScriptPorter reads and writes the script.
        string jass = System.Text.Encoding.Latin1.GetString(bytes);
        foreach (var i in JassScriptCheck.Check(jass))
            yield return new ValidationIssue(i.Severity, "script", entry.FileName,
                i.Line > 0 ? $"line {i.Line}: {i.Message}" : i.Message);
    }
}
