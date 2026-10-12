// src/Wc3.Commands/CiAllCommand.cs
namespace Wc3.Commands;

/// <summary>
/// Outcome of the combined gate. Passed is true when every gate passed and ExitCode is 0 or 1.
/// Gates holds one line per gate in the order lint, validate, compat. Lint, Validate and Compat are
/// the underlying results, Compat null when no target was given.
/// </summary>
public sealed record CiAllResult(
    bool Passed,
    int ExitCode,
    IReadOnlyList<CiGateLine> Gates,
    LintResult Lint,
    ValidateResult Validate,
    CompatResult? Compat);

/// <summary>
/// The combined CI gate. It runs the lint, the plain validation and, when a target is given, the
/// compat check in one pass and folds them into a single verdict, so a pipeline gets the whole map
/// picture from one command and one exit code. It reuses <see cref="CiLintCommand"/> and
/// <see cref="CiValidateCommand"/>, so a gate means here exactly what it means on its own. Read-only.
/// </summary>
public static class CiAllCommand
{
    /// <summary>
    /// Run lint, validate and, when target is given, compat. schemasDir overrides where the schemas
    /// are found. originalPath is optional and adds the before/after lint checks.
    /// </summary>
    public static CiAllResult Execute(
        string mapPath,
        string? target = null,
        string? schemasDir = null,
        string? originalPath = null)
    {
        var lint = CiLintCommand.Execute(mapPath, baselinePath: null, target, schemasDir, originalPath);
        var validate = CiValidateCommand.Execute(mapPath, strict: false);

        var gates = new List<CiGateLine>();
        gates.AddRange(lint.Gates.Where(g => g.Name != "compat"));
        gates.AddRange(validate.Gates);
        var compatGates = lint.Gates.Where(g => g.Name == "compat").ToList();
        if (compatGates.Count > 0) gates.AddRange(compatGates);
        else gates.Add(new CiGateLine("compat", true, "skipped, no --target given", Array.Empty<string>()));

        bool passed = gates.All(g => g.Passed);
        return new CiAllResult(passed, passed ? 0 : 1, gates, lint.Lint, validate.Validate, lint.Compat);
    }
}
