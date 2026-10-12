// src/Wc3.Commands/ReplayDiffCommand.cs
namespace Wc3.Commands;

/// <summary>How one player's leave time differs between two games. <see cref="Delta"/> is B minus A
/// and is null unless the player left in both.</summary>
public sealed record ReplayPlayerLeaveDelta(
    string? Name, int PlayerId, TimeSpan? AtA, TimeSpan? AtB, TimeSpan? Delta);

/// <summary>The comparison of two recorded games. <see cref="LengthDelta"/> is B minus A.</summary>
public sealed record ReplayDiffReport(
    string FileA, string FileB,
    string? MapA, string? MapB, bool SameMap,
    TimeSpan LengthA, TimeSpan LengthB, TimeSpan LengthDelta,
    IReadOnlyList<ReplayPlayerLeaveDelta> LeaveDeltas,
    string? ProblemA, string? ProblemB);

/// <summary>
/// Compare two recorded games. Reports whether they played the same map, the length difference,
/// and the per player leave time difference. Players are keyed by slot, so a slot that left in both
/// games gets a delta and a slot that left in only one reports the one time it has. Read only.
/// </summary>
public static class ReplayDiffCommand
{
    public static ReplayDiffReport Execute(string a, string b) =>
        Compare(ReplayCommand.ReadGame(a), ReplayCommand.ReadGame(b));

    internal static ReplayDiffReport Compare(ReplayGame a, ReplayGame b)
    {
        string? mapA = ReplayCommand.CleanMapPath(a.Map);
        string? mapB = ReplayCommand.CleanMapPath(b.Map);
        bool same = mapA is not null && mapB is not null
            && string.Equals(mapA, mapB, StringComparison.OrdinalIgnoreCase);

        var slots = a.Players.Select(p => p.Id).Concat(b.Players.Select(p => p.Id))
            .Distinct().OrderBy(id => id);
        var deltas = slots.Select(id =>
        {
            var leaveA = a.Leaves.LastOrDefault(l => l.PlayerId == id);
            var leaveB = b.Leaves.LastOrDefault(l => l.PlayerId == id);
            string? name = a.Players.FirstOrDefault(p => p.Id == id)?.Name
                ?? b.Players.FirstOrDefault(p => p.Id == id)?.Name;
            return new ReplayPlayerLeaveDelta(name, id, leaveA?.At, leaveB?.At,
                leaveA is not null && leaveB is not null ? leaveB.At - leaveA.At : null);
        }).ToList();

        return new(a.File, b.File, mapA, mapB, same, a.Length, b.Length, b.Length - a.Length,
            deltas, a.Problem, b.Problem);
    }
}
