// src/Wc3.Commands/ReplayTimelineCommand.cs
namespace Wc3.Commands;

/// <summary>One event in a recorded game's ordered timeline. <see cref="Kind"/> is one of
/// "game start", "chat", "leave" and "game end". <see cref="PlayerId"/> and <see cref="Name"/> are
/// null for the two game level events.</summary>
public sealed record ReplayTimelineEvent(
    TimeSpan At, string Kind, int? PlayerId, string? Name, string Text);

public sealed record ReplayTimelineReport(
    string File, TimeSpan Length, IReadOnlyList<ReplayTimelineEvent> Events, string? Problem);

/// <summary>
/// The ordered event list for one recorded game. The game start is at zero, every chat line and
/// player leave at the game time it happened, and the game end at the recorded length. Ties keep
/// the game start first, then chats, then leaves, then the game end. Read only.
/// </summary>
public static class ReplayTimelineCommand
{
    public static ReplayTimelineReport Execute(string replay) =>
        From(ReplayCommand.ReadGame(replay));

    internal static ReplayTimelineReport From(ReplayGame g)
    {
        var events = new List<ReplayTimelineEvent>();
        if (g.Problem is null)
        {
            events.Add(new(TimeSpan.Zero, "game start", null, null,
                ReplayCommand.CleanMapPath(g.Map) ?? ""));
            events.AddRange(g.Chats.Select(c =>
                new ReplayTimelineEvent(c.At, "chat", c.PlayerId, c.Name, c.Text)));
            events.AddRange(g.Leaves.Select(l =>
                new ReplayTimelineEvent(l.At, "leave", l.PlayerId, l.Name, l.Meaning)));
            events.Add(new(g.Length, "game end", null, null, ""));
            events = events.OrderBy(e => e.At).ThenBy(e => Rank(e.Kind)).ToList();
        }
        return new(g.File, g.Length, events, g.Problem);
    }

    private static int Rank(string kind) => kind switch
    {
        "game start" => 0,
        "chat" => 1,
        "leave" => 2,
        _ => 3,
    };
}
