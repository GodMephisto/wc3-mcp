// src/Wc3.Commands/LeakJoinCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>The runtime join. Measures the handle and memory slope of one wc3watch session, names
/// the periodic drivers whose predicted rate matches, and localizes the rise in time. Every slope is
/// a measurement of the samples given; every driver ranking is an inference from the static list.</summary>
public sealed record LeakJoinResult(
    bool Ok,
    string Message,
    string? Session,
    int SampleCount,
    string Span,
    double? HandleSlopePerMin,
    double? HandleCorrelation,
    double? MemorySlopePerMin,
    double? MemoryCorrelation,
    IReadOnlyList<LeakDriver> Drivers,
    IReadOnlyList<LeakWindow> Windows,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Joins the static ranked leak list to a recorded wc3watch session. Read-only. It reads samples.csv
/// for the game's handle and private memory columns, fits each slope, and aligns a rising handle slope
/// with the periodic candidates from the static list. A session folder with no samples.csv is
/// reported as such rather than guessed around.
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
            return new(false, error, sessionFolder, 0, "", null, null, null, null,
                Array.Empty<LeakDriver>(), Array.Empty<LeakWindow>(), diagnostics);

        var (handles, hr) = LeakWorkflow.Slope(samples, s => s.Handles);
        var (memory, mr) = LeakWorkflow.Slope(samples, s => s.MemoryMb);
        var drivers = LeakWorkflow.RankDrivers(staticLeaks);
        var windows = LeakWorkflow.RisingWindows(samples, drivers);
        string span = $"{samples[0].Time:HH:mm:ss} to {samples[^1].Time:HH:mm:ss}";

        if (samples.Count < 3)
            diagnostics.Add($"{samples.Count} sample(s) is too few for a slope, the fit needs at least 3");
        double predicted = drivers.Sum(d => d.PredictedPerMin);
        diagnostics.Add($"{staticLeaks.Leaks.Count(l => l.Heat == "hot")} hot leak(s) across "
            + $"{drivers.Count} periodic driver(s), an upper bound of {predicted:0.#} handles a minute");
        if (handles is null)
            diagnostics.Add("samples.csv has no usable wc3_handles column to fit");
        if (memory is null)
            diagnostics.Add("samples.csv has no usable wc3_priv_mb column to fit");

        string message = handles is null
            ? $"{samples.Count} sample(s), no handle slope to fit"
            : $"handle slope {handles:+0.#;-0.#;0} per minute over {samples.Count} sample(s), "
              + $"{drivers.Count} periodic driver(s) ranked";

        return new(true, message, sessionFolder, samples.Count, span, handles, hr, memory, mr,
            drivers, windows, diagnostics);
    }
}
