// src/Wc3.Commands/StringsDiffCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One key present in both maps whose text differs.</summary>
public sealed record StringChange(int Id, string? OldText, string? NewText);

/// <summary>
/// The entries added, removed and changed between two maps' war3map.wts tables. Read-only.
/// Entries are matched by id, so a text change on the same key reads as a change rather than a
/// remove plus an add. A malformed table carrying duplicate ids keeps its first entry per id,
/// matching <see cref="StringsCommand.Get"/>.
/// </summary>
public sealed record StringsDiffResult(
    IReadOnlyList<WtsEntry> Added,
    IReadOnlyList<WtsEntry> Removed,
    IReadOnlyList<StringChange> Changed);

public static class StringsDiffCommand
{
    public static StringsDiffResult Execute(MapDocument mapA, MapDocument mapB)
    {
        ArgumentNullException.ThrowIfNull(mapA);
        ArgumentNullException.ThrowIfNull(mapB);
        var a = FirstById(StringsCommand.List(mapA).Entries);
        var b = FirstById(StringsCommand.List(mapB).Entries);

        var added = b.Values.Where(e => !a.ContainsKey(e.Id)).OrderBy(e => e.Id).ToList();
        var removed = a.Values.Where(e => !b.ContainsKey(e.Id)).OrderBy(e => e.Id).ToList();
        var changed = a.Values
            .Where(e => b.TryGetValue(e.Id, out var other)
                        && !string.Equals(e.Text, other.Text, StringComparison.Ordinal))
            .OrderBy(e => e.Id)
            .Select(e => new StringChange(e.Id, e.Text, b[e.Id].Text))
            .ToList();

        return new StringsDiffResult(added, removed, changed);
    }

    private static Dictionary<int, WtsEntry> FirstById(IReadOnlyList<WtsEntry> entries)
    {
        var byId = new Dictionary<int, WtsEntry>();
        foreach (var entry in entries) byId.TryAdd(entry.Id, entry);
        return byId;
    }
}
