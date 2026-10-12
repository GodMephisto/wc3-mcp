// src/Wc3.Commands/ReplayPlayersCommand.cs
namespace Wc3.Commands;

/// <summary>One row of the per player table. <see cref="Result"/> and <see cref="LeaveReason"/> are
/// the raw codes the record carries, <see cref="LeaveMeaning"/> is the resolved word and
/// <see cref="LeftAt"/> is the game time the player left at. A player still in the game when the
/// record ends has null leave fields.</summary>
public sealed record ReplayPlayerRow(
    int Slot, string? Name, int? Team, string? Race,
    int? Result, string? LeaveMeaning, TimeSpan? LeftAt, int? LeaveReason);

public sealed record ReplayPlayersReport(
    string File, string? Map, TimeSpan Length,
    IReadOnlyList<ReplayPlayerRow> Players, string? Problem);

/// <summary>
/// The per player table for one recorded game. Name and slot come from the recorded player list,
/// team and race from the player record when a source carries them (this parser does not, so they
/// are reported as unknown rather than guessed), and the result and leave time from the leave
/// events. Read only.
/// </summary>
public static class ReplayPlayersCommand
{
    public static ReplayPlayersReport Execute(string replay) =>
        From(ReplayCommand.ReadGame(replay));

    internal static ReplayPlayersReport From(ReplayGame g) => new(
        g.File, ReplayCommand.CleanMapPath(g.Map), g.Length, Resolve(g), g.Problem);

    /// <summary>Pure resolution of one row per recorded player, in slot order. A player's leave is
    /// the last leave event recorded for its slot, since a slot can be re-listed at game end.</summary>
    internal static IReadOnlyList<ReplayPlayerRow> Resolve(ReplayGame g) =>
        g.Players.Select(p =>
        {
            var leave = g.Leaves.LastOrDefault(l => l.PlayerId == p.Id);
            return new ReplayPlayerRow(p.Id, p.Name, p.Team, p.Race,
                leave?.Result, leave?.Meaning, leave?.At, leave?.Reason);
        }).ToList();
}
