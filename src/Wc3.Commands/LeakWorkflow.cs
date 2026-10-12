// src/Wc3.Commands/LeakWorkflow.cs
using System.Globalization;
using System.Text;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One hot leak candidate, aggregated by the function that leaks and the handle kind.
/// This is the unit leak harness drives and the unit a runtime slope is attributed to.</summary>
public sealed record LeakTarget(string Function, string Handle, int Count, int Line, string Sample);

/// <summary>A periodic driver with the handles-per-minute a leak in its call tree predicts.
/// PredictedPerMin follows from the static list alone, so it is an inference, not a measurement.</summary>
public sealed record LeakDriver(string Function, double? Period, int HotLeaks, double PredictedPerMin, int Rank);

/// <summary>One second of a wc3watch session, reduced to what the leak maths needs.</summary>
public sealed record LeakSample(int Index, DateTime Time, double? Handles, double? MemoryMb);

/// <summary>The measured slopes of one session. A null slope means too few samples to fit.</summary>
public sealed record SessionSlope(
    string Session, int Samples, string Span,
    double? HandlesPerMin, double? HandleCorrelation,
    double? MemoryPerMin, double? MemoryCorrelation,
    string? Error);

/// <summary>
/// Shared maths for the leak workflow. The static half is ScriptLeaksCommand, the runtime half is a
/// wc3watch session's samples.csv. This file holds the join between them, the handle and memory
/// slope fits, and the samples reader, so the five leak commands do not each re-implement them.
///
/// A handle slope here is a least-squares fit in units per minute over (time, value) pairs, the same
/// fit analyze.py uses, so a number from this tool and a number from the wc3watch report can be
/// compared directly. It is a measurement of the samples it was given and nothing more.
/// </summary>
internal static class LeakWorkflow
{
    /// <summary>Hot leak candidates grouped by function and handle kind, in read order.</summary>
    public static IReadOnlyList<LeakTarget> Targets(ScriptLeaksResult r) =>
        r.Leaks.Where(l => l.Heat == "hot")
            .GroupBy(l => (l.Function, l.Handle))
            .Select(g => new LeakTarget(g.Key.Function, g.Key.Handle, g.Count(), g.Min(x => x.Line), g.First().Code))
            .OrderBy(t => t.Function, StringComparer.Ordinal).ThenBy(t => t.Handle, StringComparer.Ordinal)
            .ToList();

    /// <summary>
    /// Periodic drivers ranked by the handle rate their call tree predicts. A driver firing every P
    /// seconds with N hot leaks in its reach allocates N * (60 / P) handles a minute if every firing
    /// leaks, an upper bound the runtime slope is checked against.
    /// </summary>
    public static IReadOnlyList<LeakDriver> RankDrivers(ScriptLeaksResult r)
    {
        var ranked = r.Periodic
            .GroupBy(p => p.Function, StringComparer.Ordinal)
            .Select(g =>
            {
                var first = g.OrderBy(p => p.Period ?? double.MaxValue).First();
                double? period = first.Period;
                int hotLeaks = g.Max(p => p.HotLeaks);
                double predicted = period is > 0 ? hotLeaks * (60.0 / period.Value) : 0.0;
                return new LeakDriver(first.Function, period, hotLeaks, predicted, 0);
            })
            .OrderByDescending(d => d.PredictedPerMin).ThenBy(d => d.Function, StringComparer.Ordinal)
            .ToList();
        return ranked.Select((d, i) => d with { Rank = i + 1 }).ToList();
    }

    /// <summary>
    /// Reads samples.csv from a session folder. Returns an empty list and sets error when the file is
    /// absent, empty, or lacks the columns the leak maths needs. A header is required, so a file with
    /// rows but no names is reported rather than guessed at.
    /// </summary>
    public static IReadOnlyList<LeakSample> ReadSamples(string sessionFolder, out string? error)
    {
        error = null;
        string path = Path.Combine(sessionFolder, "samples.csv");
        if (!File.Exists(path))
        {
            error = $"no samples.csv in {sessionFolder}";
            return Array.Empty<LeakSample>();
        }
        var lines = File.ReadAllLines(path);
        if (lines.Length < 2)
        {
            error = $"samples.csv in {sessionFolder} has no data rows";
            return Array.Empty<LeakSample>();
        }
        var header = SplitCsv(lines[0]);
        int timeCol = IndexOf(header, "time");
        int handleCol = IndexOf(header, "wc3_handles");
        int memCol = IndexOf(header, "wc3_priv_mb");
        if (timeCol < 0 || handleCol < 0)
        {
            error = "samples.csv has no time and wc3_handles columns";
            return Array.Empty<LeakSample>();
        }

        var samples = new List<LeakSample>();
        for (int i = 1; i < lines.Length; i++)
        {
            if (lines[i].Length == 0) continue;
            var cells = SplitCsv(lines[i]);
            if (timeCol >= cells.Count) continue;
            if (!TryParseTime(cells[timeCol], out var t)) continue;
            double? handles = handleCol < cells.Count ? Num(cells[handleCol]) : null;
            double? mem = memCol >= 0 && memCol < cells.Count ? Num(cells[memCol]) : null;
            samples.Add(new LeakSample(samples.Count, t, handles, mem));
        }
        if (samples.Count == 0)
        {
            error = "samples.csv has no rows with a parseable time";
            return Array.Empty<LeakSample>();
        }
        return samples;
    }

    /// <summary>Least-squares slope in units per minute plus the Pearson correlation, over one field.</summary>
    public static (double? Slope, double? Correlation) Slope(
        IReadOnlyList<LeakSample> samples, Func<LeakSample, double?> selector)
    {
        var pts = samples.Select(s => (s.Time, V: selector(s))).Where(p => p.V is not null)
            .Select(p => (p.Time, V: p.V!.Value)).ToList();
        if (pts.Count < 3) return (null, null);
        var t0 = pts[0].Time;
        var xs = pts.Select(p => (p.Time - t0).TotalMinutes).ToList();
        var ys = pts.Select(p => p.V).ToList();
        double mx = xs.Average(), my = ys.Average();
        double sxx = xs.Sum(x => (x - mx) * (x - mx));
        double syy = ys.Sum(y => (y - my) * (y - my));
        double sxy = xs.Zip(ys, (x, y) => (x - mx) * (y - my)).Sum();
        if (sxx <= 0) return (0.0, syy <= 0 ? null : 0.0);
        double slope = sxy / sxx;
        double? r = syy <= 0 ? null : sxy / Math.Sqrt(sxx * syy);
        return (slope, r);
    }

    /// <summary>The two slopes and the span of one session, with a per-session error when unreadable.</summary>
    public static SessionSlope Summarize(string session)
    {
        var samples = ReadSamples(session, out var error);
        if (error is not null)
            return new SessionSlope(session, 0, "", null, null, null, null, error);
        var (hs, hr) = Slope(samples, s => s.Handles);
        var (ms, mr) = Slope(samples, s => s.MemoryMb);
        string span = samples.Count == 0 ? ""
            : $"{samples[0].Time:HH:mm:ss} to {samples[^1].Time:HH:mm:ss}";
        return new SessionSlope(session, samples.Count, span, hs, hr, ms, mr, null);
    }

    /// <summary>
    /// Segments a session into fixed windows and reports the ones whose handle count rose, each with
    /// the top-ranked periodic driver as the likely cause. This localizes a rising slope in time; the
    /// driver is from the static list and is a candidate, not a proven attribution.
    /// </summary>
    public static IReadOnlyList<LeakWindow> RisingWindows(
        IReadOnlyList<LeakSample> samples, IReadOnlyList<LeakDriver> drivers, double windowSeconds = 30.0)
    {
        var withHandles = samples.Where(s => s.Handles is not null).ToList();
        if (withHandles.Count < 2) return Array.Empty<LeakWindow>();
        string? top = drivers.Count > 0 ? drivers[0].Function : null;
        var t0 = withHandles[0].Time;
        var windows = new List<LeakWindow>();
        foreach (var bucket in withHandles.GroupBy(s => (int)((s.Time - t0).TotalSeconds / windowSeconds)))
        {
            var ordered = bucket.OrderBy(s => s.Time).ToList();
            if (ordered.Count < 2) continue;
            double minutes = (ordered[^1].Time - ordered[0].Time).TotalMinutes;
            if (minutes <= 0) continue;
            double rate = (ordered[^1].Handles!.Value - ordered[0].Handles!.Value) / minutes;
            if (rate <= 0) continue;
            windows.Add(new LeakWindow(
                ordered[0].Time.ToString("HH:mm:ss", CultureInfo.InvariantCulture),
                ordered[^1].Time.ToString("HH:mm:ss", CultureInfo.InvariantCulture),
                ordered.Count, rate, top));
        }
        return windows.OrderByDescending(w => w.HandlesPerMin).Take(8).ToList();
    }

    private static int IndexOf(IReadOnlyList<string> header, string name)
    {
        for (int i = 0; i < header.Count; i++)
            if (string.Equals(header[i].Trim(), name, StringComparison.Ordinal)) return i;
        return -1;
    }

    private static double? Num(string s)
    {
        s = s.Trim();
        return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : null;
    }

    private static bool TryParseTime(string s, out DateTime t)
    {
        s = s.Trim();
        string[] formats =
        {
            "yyyy-MM-dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss.ff", "yyyy-MM-dd HH:mm:ss.f",
            "yyyy-MM-dd HH:mm:ss", "yyyy-MM-ddTHH:mm:ss.fff", "yyyy-MM-ddTHH:mm:ss",
        };
        return DateTime.TryParseExact(s, formats, CultureInfo.InvariantCulture,
            DateTimeStyles.None, out t)
            || DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out t);
    }

    /// <summary>A minimal quote-aware CSV line split. wc3watch writes no commas inside fields except
    /// inside the quoted top_cpu and top_gpu columns, so this only has to honor double quotes.</summary>
    private static List<string> SplitCsv(string line)
    {
        var cells = new List<string>();
        var sb = new StringBuilder();
        bool quoted = false;
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (quoted)
            {
                if (c == '"')
                {
                    if (i + 1 < line.Length && line[i + 1] == '"') { sb.Append('"'); i++; }
                    else quoted = false;
                }
                else sb.Append(c);
            }
            else if (c == '"') quoted = true;
            else if (c == ',') { cells.Add(sb.ToString()); sb.Clear(); }
            else sb.Append(c);
        }
        cells.Add(sb.ToString());
        return cells;
    }
}

/// <summary>A stretch of a session where the handle count rose, with the top static candidate.</summary>
public sealed record LeakWindow(string Start, string End, int Samples, double HandlesPerMin, string? Driver);
