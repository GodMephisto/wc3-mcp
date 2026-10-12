// src/Wc3.Commands/CiCompatCommand.cs
namespace Wc3.Commands;

/// <summary>
/// Outcome of the compat gate. Passed is true when the target build raised no error-level finding,
/// ExitCode is 0 or 1, Gates names what was checked, and Compat is the full compat result.
/// </summary>
public sealed record CiCompatResult(
    bool Passed,
    int ExitCode,
    IReadOnlyList<CiGateLine> Gates,
    CompatResult Compat);

/// <summary>
/// The compat CI gate. It checks a map against a target build and fails on any error-level finding,
/// a native, global, type or function the map uses that the target does not declare. Warnings about
/// object fields the target no longer defines do not fail the gate, matching the compat command.
/// Read-only.
/// </summary>
public static class CiCompatCommand
{
    /// <summary>
    /// Run the compat gate. target is a schema folder path or a build name looked up in schemasDir.
    /// schemasDir overrides where the schemas are found.
    /// </summary>
    public static CiCompatResult Execute(string mapPath, string target, string? schemasDir = null)
    {
        if (!File.Exists(mapPath))
            throw new FileNotFoundException("no map at " + mapPath, mapPath);

        var compat = TargetCompatCommand.Execute(mapPath, target, schemasDir);
        var errors = compat.Findings.Where(f => f.Severity == "error").ToList();
        bool passed = errors.Count == 0;

        var gates = new List<CiGateLine>
        {
            new(
                "compat",
                passed,
                passed
                    ? $"{compat.TargetBuild}, no error-level finding, "
                      + $"identifiers checked {compat.IdentifiersChecked}, fields checked {compat.FieldsChecked}"
                    : $"{errors.Count} error-level finding(s) against {compat.TargetBuild}, "
                      + $"identifiers checked {compat.IdentifiersChecked}, fields checked {compat.FieldsChecked}",
                errors.Select(f => f.Kind + " " + f.Name + "  " + f.Where + "  " + f.Detail).ToList()),
        };

        return new CiCompatResult(passed, passed ? 0 : 1, gates, compat);
    }
}
