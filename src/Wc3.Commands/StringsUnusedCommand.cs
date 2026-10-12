// src/Wc3.Commands/StringsUnusedCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One war3map.wts entry nothing in the map references, with its text.</summary>
public sealed record UnusedString(int Id, string Text);

/// <summary>
/// The war3map.wts entries no object-data field value, no map-info field, no GUI trigger and no
/// script line references. Read-only.
///
/// The reference set is every source the toolkit trusts (<see cref="StringReferences.All"/>), so
/// an entry named only by a map-info title or a trigger reads as used and is not reported. An
/// unreferenced entry is dead weight rather than a fault, a lead to review and not an
/// instruction to delete, since a tool this side of the game cannot see every consumer.
/// </summary>
public sealed record StringsUnusedResult(
    int DefinedEntries, int ReferencedIds, IReadOnlyList<UnusedString> Unused);

public static class StringsUnusedCommand
{
    public static StringsUnusedResult Execute(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var entries = StringsCommand.List(doc).Entries;
        var referenced = StringReferences.All(doc);
        var unused = entries
            .Where(e => !referenced.Contains(e.Id))
            .OrderBy(e => e.Id)
            .Select(e => new UnusedString(e.Id, e.Text))
            .ToList();
        return new StringsUnusedResult(entries.Count, referenced.Count, unused);
    }
}
