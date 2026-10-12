// src/Wc3.Commands/LintBaselineCommand.cs
using System.Text;
using System.Text.Json;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One non-clean finding the lint reported, in the shape a baseline stores it.</summary>
public sealed record LintBaselineFinding(
    string Check,
    string Severity,
    string Summary,
    IReadOnlyList<string> Detail);

/// <summary>A recorded set of lint findings for one map, written by the baseline mode.</summary>
public sealed record LintBaseline(string Map, IReadOnlyList<LintBaselineFinding> Findings);

/// <summary>Outcome of writing a baseline file.</summary>
public sealed record LintBaselineWriteResult(string OutPath, string Map, int Findings);

/// <summary>
/// Outcome of the gate mode. Ok is true when every current finding is already in the baseline,
/// so only a finding the baseline never recorded fails. Added are the new findings, Resolved are
/// the baseline findings this run no longer reports, and Current is the full lint result.
/// </summary>
public sealed record LintGateResult(
    bool Ok,
    IReadOnlyList<LintBaselineFinding> Added,
    IReadOnlyList<LintBaselineFinding> Resolved,
    LintResult Current);

/// <summary>
/// A lint baseline for CI. The write mode records the map's current non-clean findings to a file.
/// The gate mode runs the lint again and fails only on findings that file does not already hold,
/// so a map with known and accepted warnings can still be gated on regressions. Findings are
/// matched by check name, severity and the detail lines, so a reordered or reformatted summary
/// alone does not read as a new finding.
/// </summary>
public static class LintBaselineCommand
{
    private static readonly JsonSerializerOptions Json = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    /// <summary>Run the lint and write its non-clean findings to a baseline file.</summary>
    public static LintBaselineWriteResult Write(string mapPath, string outPath, string? originalPath = null)
    {
        if (string.IsNullOrWhiteSpace(outPath))
            throw new ArgumentException("an output path is required", nameof(outPath));

        var result = LintCommand.Run(Load(mapPath), LoadOptional(originalPath));
        var baseline = new LintBaseline(Path.GetFileName(mapPath), FindingsOf(result));

        string full = Path.GetFullPath(outPath);
        if (Path.GetDirectoryName(full) is { Length: > 0 } dir) Directory.CreateDirectory(dir);
        File.WriteAllText(full, JsonSerializer.Serialize(baseline, Json) + "\n", new UTF8Encoding(false));
        return new LintBaselineWriteResult(full, baseline.Map, baseline.Findings.Count);
    }

    /// <summary>
    /// Run the lint and compare it to a baseline. Ok is true when nothing was added. The baseline
    /// read is by exact identity, so a summary word change is not a new finding while a new
    /// offending item is.
    /// </summary>
    public static LintGateResult Gate(string mapPath, string baselinePath, string? originalPath = null)
    {
        if (string.IsNullOrWhiteSpace(baselinePath))
            throw new ArgumentException("a baseline path is required", nameof(baselinePath));
        if (!File.Exists(baselinePath))
            throw new FileNotFoundException("no baseline at " + baselinePath, baselinePath);

        var baseline = JsonSerializer.Deserialize<LintBaseline>(File.ReadAllText(baselinePath), Json)
            ?? throw new InvalidOperationException("baseline file is empty or not valid " + baselinePath);

        var result = LintCommand.Run(Load(mapPath), LoadOptional(originalPath));
        var current = FindingsOf(result);

        var baselineKeys = baseline.Findings.Select(Key).ToHashSet(StringComparer.Ordinal);
        var currentKeys = current.Select(Key).ToHashSet(StringComparer.Ordinal);

        var added = current.Where(f => !baselineKeys.Contains(Key(f))).ToList();
        var resolved = baseline.Findings.Where(f => !currentKeys.Contains(Key(f))).ToList();

        return new LintGateResult(added.Count == 0, added, resolved, result);
    }

    private static MapDocument Load(string mapPath)
    {
        if (!File.Exists(mapPath))
            throw new FileNotFoundException("no map at " + mapPath, mapPath);
        return MapDocument.Load(mapPath);
    }

    private static MapDocument? LoadOptional(string? originalPath) =>
        originalPath is null ? null : Load(originalPath);

    /// <summary>The non-clean checks, one finding each, in the order the lint reported them.</summary>
    private static IReadOnlyList<LintBaselineFinding> FindingsOf(LintResult result) =>
        result.Checks
            .Where(c => c.Severity != LintSeverity.Ok)
            .Select(c => new LintBaselineFinding(c.Name, c.Severity.ToString(), c.Summary, c.Detail))
            .ToList();

    /// <summary>Identity of a finding for matching. Check, severity and every detail line, in order.</summary>
    private static string Key(LintBaselineFinding f) =>
        f.Check + "\u0000" + f.Severity + "\u0000" + string.Join("\u0001", f.Detail);
}
