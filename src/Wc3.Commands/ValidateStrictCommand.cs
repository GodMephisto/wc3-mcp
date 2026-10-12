// src/Wc3.Commands/ValidateStrictCommand.cs
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Result of the strict validation. Base is the plain validate result, StrictIssues are the
/// additional findings, and Issues is the merged list the verdict is computed over.
/// Diagnostics names any check that could not run, so a skipped check is visible rather than
/// reading as a clean one.
/// </summary>
public sealed record ValidateStrictResult(
    ValidateResult Base,
    IReadOnlyList<ValidationIssue> StrictIssues,
    bool Valid,
    int Errors,
    int Warnings,
    IReadOnlyList<ValidationIssue> Issues,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// A stricter validation than the default. It runs the plain validate first and adds the checks
/// that are too strict to be a default, a map with no (listfile), a script that declares no entry
/// point function, and a placement whose type neither the map nor the installed game defines. The
/// default <see cref="ValidateCommand"/> is untouched, so a caller only pays for these when it
/// asks for them.
/// </summary>
public static class ValidateStrictCommand
{
    public static ValidateStrictResult Execute(MapDocument doc, string? gameDirOverride = null)
    {
        ArgumentNullException.ThrowIfNull(doc);

        var baseResult = ValidateCommand.Execute(doc);
        var strict = new List<ValidationIssue>();
        var diagnostics = new List<string>();

        MissingListfile(doc, strict);
        ScriptEntryPoint(doc, strict);
        UndefinedPlacementTypes(doc, gameDirOverride, strict, diagnostics);

        var issues = baseResult.Issues.Concat(strict).ToList();
        int errors = issues.Count(i => i.Severity == DiagnosticSeverity.Error);
        int warnings = issues.Count(i => i.Severity == DiagnosticSeverity.Warning);

        return new ValidateStrictResult(
            baseResult, strict, errors == 0, errors, warnings, issues, diagnostics);
    }

    /// <summary>
    /// A map with no (listfile) entry. A protected map ships without one, and the names of its
    /// entries are then unavailable until they are recovered, so a strict check names it rather
    /// than letting the map look clean.
    /// </summary>
    private static void MissingListfile(MapDocument doc, List<ValidationIssue> issues)
    {
        if (doc.Files.Any(f => f.FileName == "(listfile)")) return;
        issues.Add(new ValidationIssue(DiagnosticSeverity.Warning, "missing-listfile", "(listfile)",
            "the map has no (listfile), so its archive entries carry no stored names until they "
            + "are recovered, and a tool that reads by name will see fewer files than exist"));
    }

    /// <summary>
    /// A JASS script that is present and non-empty but declares no function at all. The default
    /// validate reports a script that is missing its main or config function only once it declares
    /// at least one function, on the ground that a payload with none is not JASS, so a script that
    /// declares nothing at all passes it. This is the gap that check leaves.
    /// </summary>
    private static void ScriptEntryPoint(MapDocument doc, List<ValidationIssue> issues)
    {
        var entry = doc.GetFile("war3map.j") ?? doc.GetFile(@"scripts\war3map.j");
        if (entry is null || entry.CurrentBytes.Length == 0) return;
        // A Lua script is indexed by a different parser and has no main in the JASS sense.
        if ((entry.FileName ?? "").EndsWith(".lua", StringComparison.OrdinalIgnoreCase)) return;

        var source = ScriptCommand.ScriptEncoding.GetString(entry.CurrentBytes);
        var api = JassApiIndex.Parse(source);
        if (api.Functions.Count == 0)
            issues.Add(new ValidationIssue(DiagnosticSeverity.Error, "script-entry-point",
                entry.FileName ?? "war3map.j",
                "the script declares no function at all, so it has no main entry point and the "
                + "map cannot run"));
    }

    /// <summary>
    /// A placed unit or doodad whose type rawcode neither the map's own object data nor the
    /// installed game defines. Such an object resolves to nothing and never appears in the game.
    /// Needs the installed game, and says so in a diagnostic when it is unavailable, because a
    /// base-game type the map never redefines is entirely correct and cannot be told apart from a
    /// broken one without the install.
    /// </summary>
    private static void UndefinedPlacementTypes(
        MapDocument doc, string? gameDirOverride, List<ValidationIssue> issues, List<string> diagnostics)
    {
        if (!Wc3.GameData.GameData.TryOpen(gameDirOverride, out var ctx, out var diagnostic))
        {
            diagnostics.Add("undefined-type check skipped, game data unavailable (" + diagnostic + ")");
            return;
        }

        var defined = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kind in ObjectKinds.All)
            foreach (var entry in ObjectKinds.MergedEntries(doc, ObjectKinds.Info(kind)))
                defined.Add(entry.Id.ToRawcode());

        foreach (var unit in UnitInstanceCommand.List(doc))
        {
            // A start location is not a unit type the object data defines.
            if (unit.TypeRawcode == PlacementCommand.StartLocationRawcode) continue;
            if (defined.Contains(unit.TypeRawcode)) continue;
            if (ctx!.Units.TryGetUnit(unit.TypeRawcode, out _)) continue;
            issues.Add(new ValidationIssue(DiagnosticSeverity.Warning, "undefined-type",
                PlacementCommand.UnitsFile,
                "placed unit #" + unit.CreationNumber + " is type '" + unit.TypeRawcode
                + "', which neither the map nor the installed game defines, so it resolves to "
                + "nothing at runtime"));
        }

        foreach (var doodad in DoodadInstanceCommand.List(doc))
        {
            if (defined.Contains(doodad.TypeRawcode)) continue;
            if (ctx!.Doodads.TryGet(doodad.TypeRawcode, out _)) continue;
            if (ctx.Destructables.TryGet(doodad.TypeRawcode, out _)) continue;
            issues.Add(new ValidationIssue(DiagnosticSeverity.Warning, "undefined-type",
                PlacementCommand.DoodadsFile,
                "placed doodad #" + doodad.CreationNumber + " is type '" + doodad.TypeRawcode
                + "', which neither the map nor the installed game defines, so it resolves to "
                + "nothing at runtime"));
        }
    }
}
