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

/// <summary>One line of a wc3watch events.csv. Kind is game_start, game_exit, focus_lost,
/// focus_gained, match_end, gc_probe or gamelog, and Detail is its text. The gc_probe marks a
/// forced garbage collection, the point analyze.py measures a Lua map's slope from.</summary>
public sealed record LeakEvent(int Index, DateTime Time, string Kind, string Detail);

/// <summary>The measured slopes of one session. A null slope means too few samples to fit.
/// Baseline is the forced garbage collection the slopes were measured from, or null when the
/// session recorded none and the whole span was used.</summary>
public sealed record SessionSlope(
    string Session, int Samples, string Span,
    double? HandlesPerMin, double? HandleCorrelation,
    double? MemoryPerMin, double? MemoryCorrelation,
    DateTime? Baseline, int Events, string? Error);

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
    /// <summary>The event kind a recorder writes when a forced garbage collection was marked.</summary>
    public const string GcProbeKind = "gc_probe";

    /// <summary>The handle-rise window size, matching analyze.py's HANDLE_WINDOW_S.</summary>
    private const double WindowSeconds = 30.0;

    /// <summary>How far around a rising window an event or game log line still counts as near,
    /// matching analyze.py's handle_suspects.</summary>
    private const double NearSeconds = 3.0;

    private const int MaxWindows = 8;

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

    /// <summary>
    /// Reads events.csv from a session folder. A missing or headerless file is not an error, since
    /// events are evidence rather than a requirement, so an empty list comes back and the join
    /// carries on with the samples alone. Returns one LeakEvent per parseable row in file order.
    /// </summary>
    public static IReadOnlyList<LeakEvent> ReadEvents(string sessionFolder)
    {
        string path = Path.Combine(sessionFolder, "events.csv");
        if (!File.Exists(path)) return Array.Empty<LeakEvent>();
        var lines = File.ReadAllLines(path);
        if (lines.Length < 2) return Array.Empty<LeakEvent>();
        var header = SplitCsv(lines[0]);
        int timeCol = IndexOf(header, "time");
        int kindCol = IndexOf(header, "kind");
        int detailCol = IndexOf(header, "detail");
        if (timeCol < 0 || kindCol < 0) return Array.Empty<LeakEvent>();

        var events = new List<LeakEvent>();
        for (int i = 1; i < lines.Length; i++)
        {
            if (lines[i].Length == 0) continue;
            var cells = SplitCsv(lines[i]);
            if (timeCol >= cells.Count || kindCol >= cells.Count) continue;
            if (!TryParseTime(cells[timeCol], out var t)) continue;
            string kind = cells[kindCol].Trim();
            string detail = detailCol >= 0 && detailCol < cells.Count ? cells[detailCol].Trim() : "";
            events.Add(new LeakEvent(events.Count, t, kind, detail));
        }
        return events;
    }

    /// <summary>The time of the last forced garbage collection in a session, or null. Lua frees
    /// memory lazily, so the slope is measured from here when a recording marked one, the same rule
    /// analyze.py's last_gc_probe applies. A null baseline means the whole span is used.</summary>
    public static DateTime? LastGcProbe(IReadOnlyList<LeakEvent> events)
    {
        DateTime? last = null;
        foreach (var e in events)
            if (e.Kind == GcProbeKind && (last is null || e.Time > last)) last = e.Time;
        return last;
    }

    /// <summary>Least-squares slope in units per minute plus the Pearson correlation, over one field.
    /// Points before <paramref name="baseline"/> are dropped, so a session that forced a garbage
    /// collection is measured from that point the way analyze.py measures it.</summary>
    public static (double? Slope, double? Correlation) Slope(
        IReadOnlyList<LeakSample> samples, Func<LeakSample, double?> selector, DateTime? baseline = null)
    {
        var pts = samples.Where(s => baseline is null || s.Time >= baseline)
            .Select(s => (s.Time, V: selector(s))).Where(p => p.V is not null)
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

    /// <summary>The two slopes and the span of one session, with a per-session error when unreadable.
    /// When the session marked a forced garbage collection the slopes are measured from it, so a Lua
    /// map's lazy collection does not overstate the leak.</summary>
    public static SessionSlope Summarize(string session)
    {
        var samples = ReadSamples(session, out var error);
        if (error is not null)
            return new SessionSlope(session, 0, "", null, null, null, null, null, 0, error);
        var events = ReadEvents(session);
        var baseline = LastGcProbe(events);
        var (hs, hr) = Slope(samples, s => s.Handles, baseline);
        var (ms, mr) = Slope(samples, s => s.MemoryMb, baseline);
        string span = samples.Count == 0 ? ""
            : $"{samples[0].Time:HH:mm:ss} to {samples[^1].Time:HH:mm:ss}";
        return new SessionSlope(session, samples.Count, span, hs, hr, ms, mr, baseline, events.Count, null);
    }

    /// <summary>
    /// Segments a session into fixed windows and reports the ones whose handle count rose. Each
    /// window carries the periodic driver whose predicted handle rate is nearest the measured rise
    /// and the session events or game log lines within three seconds of it, the evidence that ties
    /// the rise to something that actually happened. Points before <paramref name="baseline"/> are
    /// dropped first, the same baseline the slopes use. The driver is from the static list and is a
    /// candidate, not a proven attribution.
    /// </summary>
    public static IReadOnlyList<LeakWindow> RisingWindows(
        IReadOnlyList<LeakSample> samples, IReadOnlyList<LeakDriver> drivers,
        IReadOnlyList<LeakEvent> events, DateTime? baseline = null, double windowSeconds = WindowSeconds)
    {
        var withHandles = samples
            .Where(s => s.Handles is not null && (baseline is null || s.Time >= baseline))
            .ToList();
        if (withHandles.Count < 2) return Array.Empty<LeakWindow>();
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
            string? driver = drivers.Count == 0 ? null
                : drivers.OrderBy(d => Math.Abs(d.PredictedPerMin - rate)).First().Function;
            var lo = ordered[0].Time.AddSeconds(-NearSeconds);
            var hi = ordered[^1].Time.AddSeconds(NearSeconds);
            windows.Add(new LeakWindow(
                ordered[0].Time.ToString("HH:mm:ss", CultureInfo.InvariantCulture),
                ordered[^1].Time.ToString("HH:mm:ss", CultureInfo.InvariantCulture),
                ordered.Count, rate, driver, NearText(lo, hi, events)));
        }
        return windows.OrderByDescending(w => w.HandlesPerMin).Take(MaxWindows).ToList();
    }

    /// <summary>The session events and game log lines within a window, as one line, matching the
    /// evidence analyze.py prints beside a handle-growth window. An empty area reads as such.</summary>
    private static string NearText(DateTime lo, DateTime hi, IReadOnlyList<LeakEvent> events)
    {
        var parts = new List<string>();
        var logs = new List<string>();
        foreach (var e in events)
        {
            if (e.Time < lo || e.Time > hi) continue;
            if (e.Kind == "gamelog") { if (e.Detail.Length > 0) logs.Add(e.Detail); }
            else parts.Add($"{e.Kind} {e.Time:HH:mm:ss}");
        }
        var top = logs.GroupBy(NormalizeLog, StringComparer.Ordinal)
            .Select(g => (Message: g.Key, Count: g.Count()))
            .OrderByDescending(x => x.Count).ThenBy(x => x.Message, StringComparer.Ordinal).Take(3);
        parts.AddRange(top.Select(x => $"{x.Count} x {x.Message}"));
        return parts.Count == 0 ? "no event nearby" : string.Join(", ", parts);
    }

    /// <summary>A log line with its digits replaced by N, so repeated lines of the same shape group,
    /// matching analyze.py's norm_log. Truncated the same way analyze.py truncates.</summary>
    private static string NormalizeLog(string message)
    {
        var sb = new StringBuilder(message.Length);
        foreach (char c in message) sb.Append(char.IsDigit(c) ? 'N' : c);
        string s = sb.ToString();
        return s.Length > 110 ? s[..110] : s;
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

/// <summary>A stretch of a session where the handle count rose, the periodic driver whose predicted
/// rate is nearest the rise, and the session events or game log lines within three seconds of it.</summary>
public sealed record LeakWindow(
    string Start, string End, int Samples, double HandlesPerMin, string? Driver, string Near);
