// src/Wc3.Commands/AssetUnusedCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One imported file and its size in bytes, without its content.</summary>
public sealed record AssetEntryInfo(string Path, int SizeBytes);

/// <summary>
/// The imported files no object-data field value and no script string literal names, each with
/// the size that freeing it would recover. Read-only.
/// </summary>
public sealed record AssetUnusedResult(IReadOnlyList<AssetEntryInfo> Unused, int ImportedEntries);

/// <summary>
/// Lists imported files nothing in the map references. The reference set is exactly the two
/// sources the harvester trusts, object-data field values and script string literals, expanded
/// the way the engine loads them, so an import named as an extensionless path or under its
/// ".mdl" spelling still reads as used.
///
/// Not every unreferenced file is safe to delete. A model or texture can be reached only by a
/// path string, which this finds, but the base game can also load an import by a name no field
/// or literal carries, and a preload file may name one. This is a lead to review, not an
/// instruction to delete.
/// </summary>
public static class AssetUnusedCommand
{
    public static AssetUnusedResult Execute(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var referenced = AssetReferences.ReferencedSpellings(doc);
        var entries = AssetReferences.ImportedEntries(doc);

        var unused = new List<AssetEntryInfo>();
        foreach (var entry in entries)
        {
            var name = entry.FileName!;
            if (!referenced.Contains(AssetReferences.Normalize(name)))
                unused.Add(new AssetEntryInfo(name, entry.CurrentSize));
        }
        unused.Sort((a, b) => string.Compare(a.Path, b.Path, StringComparison.OrdinalIgnoreCase));
        return new AssetUnusedResult(unused, entries.Count);
    }
}
