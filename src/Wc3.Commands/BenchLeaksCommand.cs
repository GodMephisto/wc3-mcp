// src/Wc3.Commands/BenchLeaksCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One map's leak counts from a folder sweep.</summary>
public sealed record BenchLeakMap(string Map, bool Ok, string Message, int Leaks, int Hot, int Repeat);

/// <summary>How many findings of one leak kind the whole sweep produced.</summary>
public sealed record BenchLeakKind(string Rule, int Count);

/// <summary>A folder sweep holding every map's counts, findings grouped by kind, and the totals.</summary>
public sealed record BenchLeaksResult(
    IReadOnlyList<BenchLeakMap> Maps,
    IReadOnlyList<BenchLeakKind> Kinds,
    int TotalLeaks,
    int Hot,
    int Repeat);

/// <summary>
/// Run the static leak detector over every map in a folder and report the counts per map and per
/// leak kind, a sweep rather than one map. Each map goes through
/// <see cref="ScriptLeaksCommand.Execute"/>, so the rules are the same ones the single-map command
/// applies. This command only sums them across the folder and groups them by kind.
/// </summary>
public static class BenchLeaksCommand
{
    /// <summary>Every map under <paramref name="folder"/>, each measured for leaks.</summary>
    public static BenchLeaksResult Execute(string folder)
    {
        var results = new List<(string Map, ScriptLeaksResult Result)>();
        foreach (var path in BenchMapFolder.Enumerate(folder))
        {
            string name = Path.GetFileName(path);
            try
            {
                results.Add((name, ScriptLeaksCommand.Execute(MapDocument.Load(path))));
            }
            catch (Exception e) when (e is IOException or InvalidDataException or UnauthorizedAccessException)
            {
                results.Add((name, new ScriptLeaksResult(false, e.Message, 0, 0, 0,
                    Array.Empty<ScriptLeak>(), Array.Empty<ScriptPeriodic>(), Array.Empty<string>())));
            }
        }
        return Summarize(results);
    }

    /// <summary>
    /// Fold per-map leak results into the sweep totals and the per-kind counts. Pure, so a caller
    /// can summarize a result set in memory without loading any map. Kinds are ordered by count
    /// then rule, so the biggest source of leaks reads first.
    /// </summary>
    public static BenchLeaksResult Summarize(IReadOnlyList<(string Map, ScriptLeaksResult Result)> results)
    {
        var maps = results.Select(r => new BenchLeakMap(
            r.Map, r.Result.Ok, r.Result.Message,
            r.Result.Leaks.Count,
            r.Result.Leaks.Count(l => l.Heat == "hot"),
            r.Result.Leaks.Count(l => l.Heat == "repeat"))).ToList();

        var all = results.SelectMany(r => r.Result.Leaks).ToList();
        var kinds = all.GroupBy(l => l.Rule, StringComparer.Ordinal)
            .Select(g => new BenchLeakKind(g.Key, g.Count()))
            .OrderByDescending(k => k.Count).ThenBy(k => k.Rule, StringComparer.Ordinal)
            .ToList();

        return new BenchLeaksResult(
            maps, kinds,
            all.Count,
            all.Count(l => l.Heat == "hot"),
            all.Count(l => l.Heat == "repeat"));
    }
}
