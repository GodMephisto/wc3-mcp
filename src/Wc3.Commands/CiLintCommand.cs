// src/Wc3.Commands/CiLintCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Outcome of the lint gate. Passed is true when every gate passed, ExitCode is 0 or 1 for a
/// caller to hand straight to a pipeline, Gates names each check with what it examined, and the
/// remaining fields carry the underlying results so a caller can render or reason about them.
/// Compat is null when no target was given. Baseline is the path compared against, when one was.
/// Added and Resolved are the baseline diff, empty when there was no baseline.
/// </summary>
public sealed record CiLintResult(
    bool Passed,
    int ExitCode,
    IReadOnlyList<CiGateLine> Gates,
    LintResult Lint,
    string? Baseline,
    IReadOnlyList<LintBaselineFinding> Added,
    IReadOnlyList<LintBaselineFinding> Resolved,
    CompatResult? Compat);

/// <summary>
/// The lint CI gate. It runs the lint and fails when a lint rule reports an error. With a baseline
/// it fails only on a finding the baseline does not already hold, so a map carrying known and
/// accepted warnings can still be gated on regressions. With a target it also runs the compat check
/// and fails on an error-level finding, the same behaviour <c>lint --target</c> has. Read-only, it
/// never writes the map or the baseline.
/// </summary>
public static class CiLintCommand
{
    /// <summary>
    /// Run the lint gate. baselinePath is optional and selects the regression mode. target is
    /// optional and adds a compat gate. schemasDir overrides where the schemas are found.
    /// originalPath is optional and adds the before/after lint checks, matching <c>lint --against</c>.
    /// </summary>
    public static CiLintResult Execute(
        string mapPath,
        string? baselinePath = null,
        string? target = null,
        string? schemasDir = null,
        string? originalPath = null)
    {
        if (!File.Exists(mapPath))
            throw new FileNotFoundException("no map at " + mapPath, mapPath);

        var gates = new List<CiGateLine>();
        var doc = MapDocument.Load(mapPath);
        var original = originalPath is null ? null : MapDocument.Load(originalPath);
        var lint = LintCommand.Run(doc, original);

        IReadOnlyList<LintBaselineFinding> added = Array.Empty<LintBaselineFinding>();
        IReadOnlyList<LintBaselineFinding> resolved = Array.Empty<LintBaselineFinding>();

        if (baselinePath is not null)
        {
            var gate = LintBaselineCommand.Gate(mapPath, baselinePath, originalPath);
            added = gate.Added;
            resolved = gate.Resolved;
            string name = Path.GetFileName(baselinePath);
            gates.Add(new CiGateLine(
                "lint-baseline",
                gate.Ok,
                gate.Ok
                    ? $"{lint.Checks.Count} check(s), no finding outside {name}"
                    : $"{gate.Added.Count} finding(s) not in {name}",
                gate.Added
                    .Select(f => f.Check + "  " + f.Severity + "  " + f.Summary)
                    .ToList()));
        }
        else
        {
            var errors = lint.Checks.Where(c => c.Severity == LintSeverity.Error).ToList();
            gates.Add(new CiGateLine(
                "lint",
                lint.Ok,
                lint.Ok
                    ? $"{lint.Checks.Count} check(s), {lint.Warnings} warning(s), no error"
                    : $"{errors.Count} error-level finding(s)",
                errors.Select(c => c.Name + "  " + c.Summary).ToList()));
        }

        CompatResult? compat = null;
        if (target is not null)
        {
            compat = TargetCompatCommand.Execute(mapPath, target, schemasDir);
            var compatErrors = compat.Findings.Where(f => f.Severity == "error").ToList();
            gates.Add(new CiGateLine(
                "compat",
                compatErrors.Count == 0,
                compatErrors.Count == 0
                    ? $"{compat.TargetBuild}, no error-level finding, "
                      + $"identifiers checked {compat.IdentifiersChecked}, fields checked {compat.FieldsChecked}"
                    : $"{compatErrors.Count} error-level finding(s) against {compat.TargetBuild}, "
                      + $"identifiers checked {compat.IdentifiersChecked}, fields checked {compat.FieldsChecked}",
                compatErrors.Select(f => f.Kind + " " + f.Name + "  " + f.Where + "  " + f.Detail).ToList()));
        }

        bool passed = gates.All(g => g.Passed);
        return new CiLintResult(passed, passed ? 0 : 1, gates, lint, baselinePath, added, resolved, compat);
    }
}
