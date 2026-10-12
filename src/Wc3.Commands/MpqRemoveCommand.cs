// src/Wc3.Commands/MpqRemoveCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>The edited copy, whether the named file was there, and whether the rebuild dropped it.</summary>
public sealed record MpqRemoveResult(
    string Path,
    string SavedTo,
    string Name,
    bool Found,
    int InputEntries,
    int OutputEntries,
    bool DroppedOnRebuild,
    long OutputBytes,
    bool Reopens,
    string? ReopenError);

/// <summary>
/// Removes one internal file from an archive, by rebuilding the container without it.
///
/// <see cref="MapDocument"/> has no removal API on purpose. Its save rebuilds the original archive
/// and only ADDS dirty entries, and <c>MpqArchiveBuilder.RemoveFile</c> followed by an
/// <c>AddFile</c> of the same name silently drops the file through the removal set. So removal is
/// done here, at the container level, by building a fresh archive that carries every other readable
/// named entry and omits the one asked for. <see cref="MpqRemoveResult.DroppedOnRebuild"/> is the
/// measured answer to "did the rebuild actually leave the name out", checked by reopening the copy
/// rather than assumed.
///
/// A copy is written, and the input map is never modified in place.
/// </summary>
public static class MpqRemoveCommand
{
    public static MpqRemoveResult Execute(string path, string name, string? outPath)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("an internal file name is required", nameof(name));
        string dest = MpqStripBookkeepingCommand.ResolveOutput(path, outPath);

        var doc = MapDocument.Load(path);
        int inputEntries = doc.Files.Count;
        bool found = doc.GetFile(name) is not null;

        var entries = new List<MpqWriteEntry>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var file in doc.Files)
        {
            string? entryName = file.FileName ?? file.RecoveredFileName;
            if (entryName is null) continue;
            if (string.Equals(entryName, name, StringComparison.OrdinalIgnoreCase)) continue;
            if (!doc.TryReadFileByName(entryName, out var bytes) || bytes.Length == 0) continue;
            if (!seen.Add(entryName)) continue;   // one payload per name, first wins
            entries.Add(new MpqWriteEntry(entryName, bytes));
        }

        byte[] rebuilt = MpqArchiveWriter.Write(entries, doc.PreArchiveData, hashTableSize: null);

        string? dir = Path.GetDirectoryName(dest);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        File.WriteAllBytes(dest, rebuilt);

        bool reopens = true;
        string? reopenError = null;
        int outputEntries = 0;
        bool dropped = false;
        try
        {
            var reopened = MapDocument.Load(dest);
            outputEntries = reopened.Files.Count;
            dropped = !reopened.HasFileByName(name);
        }
        catch (Exception ex)
        {
            reopens = false;
            reopenError = ex.Message;
        }

        return new MpqRemoveResult(path, dest, name, found, inputEntries, outputEntries,
            dropped, rebuilt.LongLength, reopens, reopenError);
    }
}
