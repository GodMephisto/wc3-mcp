// src/Wc3.Commands/ReplayStatsCommand.cs
namespace Wc3.Commands;

/// <summary>One player's aggregate line. <see cref="ActionsPerMinute"/> is null when the record
/// does not carry an action count, which is reported as unavailable rather than as zero.</summary>
public sealed record ReplayPlayerStat(
    string? Name, int Slot, int? Actions, double? ActionsPerMinute,
    TimeSpan? LeftAt, string? LeaveMeaning);

/// <summary>Aggregate stats for one recorded game. <see cref="ActionsAvailable"/> is true only when
/// at least one player carried an action count, so a zero count and an absent count are told
/// apart. <see cref="Disconnects"/> counts leaves with the disconnect result code.</summary>
public sealed record ReplayStatsReport(
    string File, TimeSpan Duration, int PlayerCount, int Disconnects,
    IReadOnlyList<ReplayPlayerStat> Players, bool ActionsAvailable, string? Problem);

/// <summary>
/// Aggregate stats for one recorded game. Reports duration, player count, disconnects, and a per
/// player action rate when the record carries action counts. This parser does not read the action
/// stream into per player counts, so a real replay reports the rate as unavailable and the per
/// player table carries the leave instead. Read only.
/// </summary>
public static class ReplayStatsCommand
{
    public static ReplayStatsReport Execute(string replay) =>
        Summarize(ReplayCommand.ReadGame(replay));

    internal static ReplayStatsReport Summarize(ReplayGame g)
    {
        double minutes = g.Length.TotalMinutes;
        bool actions = g.Players.Any(p => p.Actions.HasValue);
        var stats = g.Players.Select(p =>
        {
            var leave = g.Leaves.LastOrDefault(l => l.PlayerId == p.Id);
            double? perMinute = p.Actions.HasValue && minutes > 0
                ? p.Actions.Value / minutes
                : null;
            return new ReplayPlayerStat(p.Name, p.Id, p.Actions, perMinute, leave?.At, leave?.Meaning);
        }).ToList();
        int disconnects = g.Leaves.Count(l => l.Result == 0x01);
        return new(g.File, g.Length, g.Players.Count, disconnects, stats, actions, g.Problem);
    }
}
