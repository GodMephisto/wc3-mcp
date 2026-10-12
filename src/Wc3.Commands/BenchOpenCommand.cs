// src/Wc3.Commands/BenchOpenCommand.cs
using System.Diagnostics;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One map's open measurement. Milliseconds is wall-clock time for Load.</summary>
public sealed record BenchOpenMap(string Map, long Milliseconds, int Entries, long Bytes);

/// <summary>
/// A folder open sweep holding every map's measurement, the slowest map, the median open time and
/// the total entry bytes. MedianMs is 0 when nothing could be measured, and Failures names the
/// maps that could not be opened so one unreadable file does not abort the sweep.
/// </summary>
public sealed record BenchOpenResult(
    IReadOnlyList<BenchOpenMap> Maps,
    BenchOpenMap? Slowest,
    double MedianMs,
    long TotalBytes,
    IReadOnlyList<string> Failures);

/// <summary>
/// Open every map in a folder through <see cref="MapDocument.Load"/> and record the per-map open
/// time, entry count and total decompressed bytes. Reports the slowest map and the median open
/// time, so a folder that loads slowly can be found by measurement rather than by guess. The load
/// is read-only and never writes the map back.
/// </summary>
public static class BenchOpenCommand
{
    public static BenchOpenResult Execute(string folder)
    {
        var maps = new List<BenchOpenMap>();
        var failures = new List<string>();
        foreach (var path in BenchMapFolder.Enumerate(folder))
        {
            try
            {
                var sw = Stopwatch.StartNew();
                var doc = MapDocument.Load(path);
                sw.Stop();
                long bytes = doc.Files.Sum(f => (long)f.RawSize);
                maps.Add(new BenchOpenMap(
                    Path.GetFileName(path), sw.ElapsedMilliseconds, doc.Files.Count, bytes));
            }
            catch (Exception e) when (e is IOException or InvalidDataException or UnauthorizedAccessException)
            {
                failures.Add(Path.GetFileName(path) + " " + e.Message);
            }
        }
        return Summarize(maps, failures);
    }

    /// <summary>
    /// Compute the slowest map and the median open time from an in-memory result set. Pure and
    /// deterministic, so it can be tested without measuring real wall-clock time. The median of an
    /// even count is the mean of the two middle values, and an empty set has no slowest map and a
    /// median of 0.
    /// </summary>
    public static BenchOpenResult Summarize(
        IReadOnlyList<BenchOpenMap> maps, IReadOnlyList<string>? failures = null)
    {
        var ordered = maps.OrderBy(m => m.Milliseconds).ThenBy(m => m.Map, StringComparer.Ordinal).ToList();
        BenchOpenMap? slowest = ordered.Count == 0 ? null : ordered[^1];

        double median = 0;
        if (ordered.Count > 0)
        {
            int mid = ordered.Count / 2;
            median = ordered.Count % 2 == 1
                ? ordered[mid].Milliseconds
                : (ordered[mid - 1].Milliseconds + ordered[mid].Milliseconds) / 2.0;
        }

        return new BenchOpenResult(
            maps, slowest, median, maps.Sum(m => m.Bytes),
            failures ?? Array.Empty<string>());
    }
}
