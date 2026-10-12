// src/Wc3.Commands/AssetSizeReportCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>Total bytes and entry count for one parent folder of the archive.</summary>
public sealed record AssetFolderSize(string Folder, long Bytes, int Entries);

/// <summary>Per-folder totals of the imported entries, largest first.</summary>
public sealed record AssetSizeReportResult(
    IReadOnlyList<AssetFolderSize> Folders,
    long TotalBytes,
    int TotalEntries);

/// <summary>
/// Breaks the archive's imported bytes down by parent folder, largest total first, so the one
/// or two folders that dominate a map's weight are obvious. Files at the archive top are their
/// own "(root)" row. Read-only.
/// </summary>
public static class AssetSizeReportCommand
{
    public static AssetSizeReportResult Execute(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var entries = AssetReferences.ImportedEntries(doc);

        var byFolder = new Dictionary<string, (long Bytes, int Count)>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in entries)
        {
            var folder = AssetReferences.FolderOf(entry.FileName!);
            var current = byFolder.TryGetValue(folder, out var value) ? value : (Bytes: 0L, Count: 0);
            byFolder[folder] = (current.Bytes + entry.CurrentSize, current.Count + 1);
        }

        var folders = byFolder
            .Select(kv => new AssetFolderSize(kv.Key, kv.Value.Bytes, kv.Value.Count))
            .OrderByDescending(f => f.Bytes)
            .ThenBy(f => f.Folder, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return new AssetSizeReportResult(folders, folders.Sum(f => f.Bytes), entries.Count);
    }
}
