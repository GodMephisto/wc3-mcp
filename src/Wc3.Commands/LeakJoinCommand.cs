// src/Wc3.Commands/LeakJoinCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>The runtime join. Measures the handle and memory slope of one wc3watch session from its
/// forced garbage collection when it marked one, joins each rising window to the session events and
/// game log lines near it, and names the periodic driver whose predicted rate is nearest the rise.
/// Every slope is a measurement of the samples given, every driver ranking is an inference from the
/// static list, and every nearby event is a recorded fact.</summary>
public sealed record LeakJoinResult(
    bool Ok,
    string Message,
    string? Session,
    int SampleCount,
    string Span,
    DateTime? GcProbe,
    int EventsJoined,
    double? HandleSlopePerMin,
    double? HandleCorrelation,
    double? MemorySlopePerMin,
    double? MemoryCorrelation,
    IReadOnlyList<LeakDriver> Drivers,
    IReadOnlyList<LeakWindow> Windows,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Joins the static ranked leak list to a recorded wc3watch session. Read-only. It reads samples.csv
/// for the game's handle and private memory columns and events.csv for the forced garbage collection,
/// the session events and the game log. The slopes are measured from that garbage collection when one
/// was marked, the same baseline analyze.py uses, so a Lua map's lazy collection does not overstate a
/// leak. Each rising window carries the periodic candidate nearest its measured rise and the events
/// and log lines near it in time, which is what names the likely leaking trigger rather than echoing
/// the top static candidate. A session folder with no samples.csv is reported as such.
/// </summary>
public static class LeakJoinCommand
{
    public static LeakJoinResult Execute(MapDocument doc, string sessionFolder)
        => From(ScriptLeaksCommand.Execute(doc), sessionFolder);

    public static LeakJoinResult From(ScriptLeaksResult staticLeaks, string sessionFolder)
    {
        var diagnostics = new List<string>();
        var samples = LeakWorkflow.ReadSamples(sessionFolder, out var error);
        if (error is not null)
            return new(false, error, sessionFolder, 0, "", null, 0, null, null, null, null,
                Array.Empty<LeakDriver>(), Array.Empty<LeakWindow>(), diagnostics);

        var events = LeakWorkflow.ReadEvents(sessionFolder);
        var gcProbe = LeakWorkflow.LastGcProbe(events);
        var (handles, hr) = LeakWorkflow.Slope(samples, s => s.Handles, gcProbe);
        var (memory, mr) = LeakWorkflow.Slope(samples, s => s.MemoryMb, gcProbe);
        var drivers = LeakWorkflow.RankDrivers(staticLeaks);
        var windows = LeakWorkflow.RisingWindows(samples, drivers, events, gcProbe);
        string span = $"{samples[0].Time:HH:mm:ss} to {samples[^1].Time:HH:mm:ss}";

        if (samples.Count < 3)
            diagnostics.Add($"{samples.Count} sample(s) is too few for a slope, the fit needs at least 3");
        double predicted = drivers.Sum(d => d.PredictedPerMin);
        diagnostics.Add($"{staticLeaks.Leaks.Count(l => l.Heat == "hot")} hot leak(s) across "
            + $"{drivers.Count} periodic driver(s), an upper bound of {predicted:0.#} handles a minute");
        if (gcProbe is { } g)
            diagnostics.Add($"slopes measured from the forced garbage collection at {g:HH:mm:ss}, the "
                + "same baseline analyze.py uses, so a Lua map's lazy collection does not overstate a leak");
        else if (events.Count > 0)
            diagnostics.Add($"no gc_probe among {events.Count} session event(s), so the slopes cover the whole span");
        if (events.Count > 0)
        {
            int evidenced = windows.Count(w => w.Near != "no event nearby");
            diagnostics.Add($"{events.Count} session event(s) read, {evidenced} of {windows.Count} rising "
                + "window(s) carry a nearby session event or game log line");
        }
        if (handles is null)
            diagnostics.Add("samples.csv has no usable wc3_handles column to fit");
        if (memory is null)
            diagnostics.Add("samples.csv has no usable wc3_priv_mb column to fit");

        string baselineNote = gcProbe is { } b ? $", measured from gc_probe {b:HH:mm:ss}" : "";
        string message = handles is null
            ? $"{samples.Count} sample(s), no handle slope to fit"
            : $"handle slope {handles:+0.#;-0.#;0} per minute over {samples.Count} sample(s){baselineNote}, "
              + $"{drivers.Count} periodic driver(s) ranked";

        return new(true, message, sessionFolder, samples.Count, span, gcProbe, events.Count,
            handles, hr, memory, mr, drivers, windows, diagnostics);
    }
}
