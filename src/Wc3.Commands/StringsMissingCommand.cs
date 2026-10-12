// src/Wc3.Commands/StringsMissingCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// The TRIGSTR_ keys the map script and the GUI trigger tree reference that have no
/// war3map.wts entry behind them. Read-only.
///
/// A key in this list resolves to the literal token "TRIGSTR_nnn" in game, so a missing entry
/// shows that text where a name or a message was meant. This is the reference side of the
/// strings check, the counterpart of <see cref="StringsUnusedCommand"/> on the entry side.
/// </summary>
public sealed record StringsMissingResult(
    int ReferencedIds, int DefinedEntries, IReadOnlyList<int> MissingIds);

public static class StringsMissingCommand
{
    public static StringsMissingResult Execute(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var defined = StringsCommand.List(doc).Entries.Select(e => e.Id).ToHashSet();
        var referenced = new HashSet<int>(StringReferences.FromScript(doc));
        referenced.UnionWith(StringReferences.FromTriggers(doc));
        var missing = referenced.Where(id => !defined.Contains(id)).OrderBy(id => id).ToList();
        return new StringsMissingResult(referenced.Count, defined.Count, missing);
    }
}
