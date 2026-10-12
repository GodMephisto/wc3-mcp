// src/Wc3.Commands/CiValidateCommand.cs
using War3Net.Build.Info;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Outcome of the validate gate. Passed is true when the map validated, ExitCode is 0 or 1, Gates
/// names what was checked, Validate is the plain result, and Strict is the strict result when
/// strict was requested. A map that cannot load throws, which the CLI turns into exit code 1.
/// </summary>
public sealed record CiValidateResult(
    bool Passed,
    int ExitCode,
    IReadOnlyList<CiGateLine> Gates,
    ValidateResult Validate,
    ValidateStrictResult? Strict);

/// <summary>
/// The validate CI gate. It runs the plain validation and fails when the map has an error-level
/// issue. With strict it runs the stricter validation instead and fails on any error it reports.
/// Read-only.
/// </summary>
public static class CiValidateCommand
{
    /// <summary>
    /// Run the validate gate. strict selects the stricter validation, whose extra checks are the
    /// ones too strict to be a default. gameDir overrides where the installed game is found for the
    /// strict undefined-type check.
    /// </summary>
    public static CiValidateResult Execute(string mapPath, bool strict = false, string? gameDir = null)
    {
        if (!File.Exists(mapPath))
            throw new FileNotFoundException("no map at " + mapPath, mapPath);

        var doc = MapDocument.Load(mapPath);
        var gates = new List<CiGateLine>();

        if (strict)
        {
            var strictResult = ValidateStrictCommand.Execute(doc, gameDir);
            var errors = strictResult.Issues.Where(i => i.Severity == DiagnosticSeverity.Error).ToList();
            var detail = errors
                .Select(i => i.Category + "  " + i.FileName + "  " + i.Message)
                .ToList();
            detail.AddRange(strictResult.Diagnostics.Select(d => "note  " + d));
            gates.Add(new CiGateLine(
                "validate-strict",
                strictResult.Valid,
                strictResult.Valid
                    ? $"valid, {strictResult.Errors} error(s), {strictResult.Warnings} warning(s), "
                      + $"{strictResult.StrictIssues.Count} strict-only issue(s)"
                    : $"invalid, {strictResult.Errors} error(s), {strictResult.Warnings} warning(s)",
                detail));
            return new CiValidateResult(strictResult.Valid, strictResult.Valid ? 0 : 1, gates,
                strictResult.Base, strictResult);
        }

        var result = ValidateCommand.Execute(doc);
        var plainErrors = result.Issues.Where(i => i.Severity == DiagnosticSeverity.Error).ToList();
        gates.Add(new CiGateLine(
            "validate",
            result.Valid,
            result.Valid
                ? $"valid, {result.Errors} error(s), {result.Warnings} warning(s), {result.Issues.Count} issue(s)"
                : $"invalid, {result.Errors} error(s), {result.Warnings} warning(s)",
            plainErrors.Select(i => i.Category + "  " + i.FileName + "  " + i.Message).ToList()));
        return new CiValidateResult(result.Valid, result.Valid ? 0 : 1, gates, result, null);
    }
}
