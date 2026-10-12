// src/Wc3.Commands/CompatMatrixCommand.cs
namespace Wc3.Commands;

/// <summary>
/// One map checked against one target build. Pass is true when the target build raised no
/// error-level finding, so a map that only warns still passes. Errors, Warnings and Findings
/// are the counts, so a reader can tell one failing check from a map that is broken throughout.
/// </summary>
public sealed record CompatMatrixCell(
    string Map,
    string Target,
    bool Pass,
    int Errors,
    int Warnings,
    int Findings);

/// <summary>A grid of maps against target builds. Maps and Targets are the row and column labels.</summary>
public sealed record CompatMatrixResult(
    IReadOnlyList<string> Maps,
    IReadOnlyList<string> Targets,
    IReadOnlyList<CompatMatrixCell> Cells);

/// <summary>
/// Check several maps against several target builds and report one pass or fail per cell, with
/// the finding count. This is the whole-map by whole-build view the single compat command cannot
/// give, so an operator can see at a glance which of their maps still compile on which builds.
/// </summary>
public static class CompatMatrixCommand
{
    public static CompatMatrixResult Execute(
        IReadOnlyList<string> maps, IReadOnlyList<string> targets, string? schemasDir = null)
    {
        if (maps.Count == 0) throw new ArgumentException("at least one map is required", nameof(maps));
        if (targets.Count == 0) throw new ArgumentException("at least one target build is required", nameof(targets));

        var cells = new List<CompatMatrixCell>();
        foreach (var map in maps)
            foreach (var target in targets)
            {
                var r = TargetCompatCommand.Execute(map, target, schemasDir);
                int errors = r.Findings.Count(f => f.Severity == "error");
                int warnings = r.Findings.Count(f => f.Severity == "warning");
                cells.Add(new CompatMatrixCell(
                    Path.GetFileName(map), r.TargetBuild, errors == 0, errors, warnings, r.Findings.Count));
            }

        return new CompatMatrixResult(
            maps.Select(m => Path.GetFileName(m)!).ToList(), targets.ToList(), cells);
    }
}
