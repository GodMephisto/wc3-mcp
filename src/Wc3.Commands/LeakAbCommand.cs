// src/Wc3.Commands/LeakAbCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>The A/B diff of two sessions. HandlesFell and MemoryFell are measured from the two
/// slopes. LikelyDisabled, when set, is the static periodic candidate whose predicted rate is
/// closest to the measured handle drop, an inference and named as one.</summary>
public sealed record LeakAbResult(
    bool Ok,
    string Message,
    SessionSlope A,
    SessionSlope B,
    double? HandleDropPerMin,
    double? MemoryDropPerMin,
    bool HandlesFell,
    bool MemoryFell,
    IReadOnlyList<LeakDriver> Candidates,
    string? LikelyDisabled,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Diffs two wc3watch sessions and says whether the handle and memory slope fell. Read-only. The
/// usual shape is one session with a trigger live and one with it disabled, so a fall on the handle
/// slope is the leak being removed. When a map is supplied the drop is matched to the periodic
/// candidate whose predicted rate is nearest, and that candidate is reported as the likely disabled
/// one, labelled an inference because it comes from the static list and not from the session.
/// </summary>
public static class LeakAbCommand
{
    public static LeakAbResult Execute(string sessionA, string sessionB, MapDocument? map = null)
        => From(sessionA, sessionB, map is null ? null : ScriptLeaksCommand.Execute(map));

    public static LeakAbResult From(string sessionA, string sessionB, ScriptLeaksResult? staticLeaks)
    {
        var diagnostics = new List<string>();
        var a = LeakWorkflow.Summarize(sessionA);
        var b = LeakWorkflow.Summarize(sessionB);
        if (a.Error is not null) diagnostics.Add($"session A, {a.Error}");
        if (b.Error is not null) diagnostics.Add($"session B, {b.Error}");

        double? handleDrop = a.HandlesPerMin is { } ha && b.HandlesPerMin is { } hb ? ha - hb : null;
        double? memoryDrop = a.MemoryPerMin is { } ma && b.MemoryPerMin is { } mb ? ma - mb : null;
        bool handlesFell = handleDrop is > 0;
        bool memoryFell = memoryDrop is > 0;

        var candidates = staticLeaks is null
            ? Array.Empty<LeakDriver>()
            : LeakWorkflow.RankDrivers(staticLeaks);
        string? likely = null;
        if (handleDrop is > 0 && candidates.Count > 0)
        {
            var best = candidates.OrderBy(c => Math.Abs(c.PredictedPerMin - handleDrop.Value)).First();
            likely = best.Function;
            double delta = Math.Abs(best.PredictedPerMin - handleDrop.Value);
            diagnostics.Add($"closest candidate {best.Function} predicts {best.PredictedPerMin:0.#} handles "
                + $"a minute against a measured drop of {handleDrop.Value:0.#}, a gap of {delta:0.#}");
        }
        else if (handleDrop is > 0 && staticLeaks is null)
        {
            diagnostics.Add("no map was given, so the drop is not attributed to a named candidate");
        }

        string message = a.Error is not null || b.Error is not null
            ? "one or both sessions could not be read"
            : $"handles {Describe(handleDrop, handlesFell)}, memory {Describe(memoryDrop, memoryFell)}"
              + (likely is null ? "" : $", likely disabled {likely}");

        return new(a.Error is null && b.Error is null, message, a, b, handleDrop, memoryDrop,
            handlesFell, memoryFell, candidates, likely, diagnostics);
    }

    private static string Describe(double? drop, bool fell)
    {
        if (drop is null) return "no slope";
        if (!fell) return $"did not fall ({drop:+0.#;-0.#;0} per minute)";
        return $"fell {drop:0.#} per minute";
    }
}
