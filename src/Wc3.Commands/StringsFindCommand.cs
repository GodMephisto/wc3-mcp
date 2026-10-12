// src/Wc3.Commands/StringsFindCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// The war3map.wts entries whose text contains the given text, case-insensitive, each with its
/// id. Read-only. An empty query matches every entry, which is a way to list the table through
/// the same shape.
/// </summary>
public sealed record StringsFindResult(
    string Query, int MatchCount, IReadOnlyList<WtsEntry> Matches);

public static class StringsFindCommand
{
    public static StringsFindResult Execute(MapDocument doc, string text)
    {
        ArgumentNullException.ThrowIfNull(doc);
        ArgumentNullException.ThrowIfNull(text);
        var matches = StringsCommand.List(doc).Entries
            .Where(e => e.Text.Contains(text, StringComparison.OrdinalIgnoreCase))
            .OrderBy(e => e.Id)
            .ToList();
        return new StringsFindResult(text, matches.Count, matches);
    }
}
