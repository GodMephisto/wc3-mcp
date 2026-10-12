// src/Wc3.Commands/BenchPerfGateCommand.cs
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wc3.Commands;

/// <summary>
/// One recorded benchmark run. Metrics maps a metric name to its measured value, and
/// HigherIsBetter names the metrics for which an increase is an improvement. Every metric not in
/// that list is treated as lower-is-better, which fits latency, time, memory, bytes and error
/// counts, the costs a gate usually protects.
/// </summary>
public sealed record BenchRun(
    string Name,
    string? Generated,
    IReadOnlyDictionary<string, double> Metrics,
    IReadOnlyList<string> HigherIsBetter);

/// <summary>
/// One metric's comparison. ChangePct is the raw percent change and RegressionPct is the same
/// figure signed so positive means worse. Both are null when the baseline was 0, where a percent
/// change is undefined. The Regressed flag still carries the verdict even then.
/// </summary>
public sealed record PerfGateMetric(
    string Metric, double Baseline, double Run, double? ChangePct, double? RegressionPct, bool Regressed);

/// <summary>
/// The gate outcome. Pass is false when any compared metric regressed by more than the threshold.
/// Metrics are compared name by name and ordered by name. MissingInRun and AddedInRun list the
/// metrics only one side carried, so a renamed metric cannot silently pass.
/// </summary>
public sealed record PerfGateResult(
    bool Pass,
    double MaxRegressionPct,
    int MetricsCompared,
    int Regressed,
    IReadOnlyList<PerfGateMetric> Metrics,
    IReadOnlyList<string> MissingInRun,
    IReadOnlyList<string> AddedInRun);

/// <summary>
/// Compare two recorded benchmark results and say whether any metric regressed by more than a
/// threshold, so a CI pipeline can fail a build that made the product slower or heavier. A metric
/// present in only one run is reported rather than counted, because absence is not a regression. It
/// is a changed measurement. The comparison is pure data, it runs no benchmark itself.
/// </summary>
public static class BenchPerfGateCommand
{
    private static readonly JsonSerializerOptions Read = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    /// <summary>Load a recorded run from a JSON file. Throws a clean message for a malformed file.</summary>
    public static BenchRun Load(string path)
    {
        if (!File.Exists(path)) throw new FileNotFoundException("benchmark file not found " + path, path);
        RunJson? json;
        try
        {
            json = JsonSerializer.Deserialize<RunJson>(File.ReadAllText(path), Read);
        }
        catch (JsonException e)
        {
            throw new InvalidDataException(path + " is not a benchmark JSON file, " + e.Message);
        }
        if (json?.Metrics is null)
            throw new InvalidDataException(path + " has no metrics object");
        return new BenchRun(
            string.IsNullOrWhiteSpace(json.Name) ? Path.GetFileNameWithoutExtension(path) : json.Name!,
            json.Generated,
            json.Metrics,
            json.HigherIsBetter ?? (IReadOnlyList<string>)Array.Empty<string>());
    }

    /// <summary>Load both runs and compare them at the given threshold.</summary>
    public static PerfGateResult Execute(string baselinePath, string runPath, double maxRegressionPct = 10) =>
        Compare(Load(baselinePath), Load(runPath), maxRegressionPct);

    /// <summary>
    /// Compare two in-memory runs. A metric regresses when its signed RegressionPct is strictly
    /// greater than the threshold, so a change exactly at the threshold passes. Pure.
    /// </summary>
    public static PerfGateResult Compare(BenchRun baseline, BenchRun run, double maxRegressionPct = 10)
    {
        if (maxRegressionPct < 0)
            throw new ArgumentOutOfRangeException(nameof(maxRegressionPct), "threshold percent cannot be negative");

        var higher = new HashSet<string>(baseline.HigherIsBetter, StringComparer.Ordinal);
        higher.UnionWith(run.HigherIsBetter);

        var metrics = new List<PerfGateMetric>();
        var missing = new List<string>();
        foreach (var name in baseline.Metrics.Keys.OrderBy(k => k, StringComparer.Ordinal))
        {
            if (!run.Metrics.TryGetValue(name, out var value))
            {
                missing.Add(name);
                continue;
            }

            double b = baseline.Metrics[name];
            bool lowerIsBetter = !higher.Contains(name);
            double? change = b == 0 ? null : (value - b) / b * 100.0;
            double? regression = change is null ? null : (lowerIsBetter ? change.Value : -change.Value);

            bool regressed = regression is { } r
                ? r > maxRegressionPct
                : b == 0 && value != 0 && lowerIsBetter;

            metrics.Add(new PerfGateMetric(name, b, value, change, regression, regressed));
        }

        var added = run.Metrics.Keys.Except(baseline.Metrics.Keys, StringComparer.Ordinal)
            .OrderBy(k => k, StringComparer.Ordinal).ToList();

        int regressedCount = metrics.Count(m => m.Regressed);
        return new PerfGateResult(
            regressedCount == 0, maxRegressionPct, metrics.Count, regressedCount, metrics, missing, added);
    }

    /// <summary>The on-disk shape, read defensively so a missing field is a clean error, not a crash.</summary>
    private sealed class RunJson
    {
        public string? Name { get; set; }
        public string? Generated { get; set; }
        public Dictionary<string, double>? Metrics { get; set; }
        public List<string>? HigherIsBetter { get; set; }
    }
}
