// src/Wc3.Commands/MpqRebuildCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One entry the rebuild could not carry, and why it was left out.</summary>
public sealed record MpqRebuildDrop(string Name, string Reason);

/// <summary>The rebuilt copy, its entry counts before and after, and what was dropped.</summary>
public sealed record MpqRebuildResult(
    string Path,
    string SavedTo,
    int InputEntries,
    int OutputEntries,
    int DroppedEntries,
    IReadOnlyList<MpqRebuildDrop> Dropped,
    uint InputHashSlots,
    uint OutputHashSlots,
    long OutputBytes,
    bool Reopens,
    string? ReopenError);

/// <summary>
/// Rebuilds a map's archive from scratch into a healthy one.
///
/// A protected map ships block-table rows that are deliberate nonsense, and War3Net's
/// <c>MpqArchiveBuilder(archive)</c> reads every entry's header up front, so that constructor
/// throws before a single byte is written. This rebuild never constructs from the archive. It
/// reads each entry through <see cref="MapDocument.TryReadFileByName"/> and adds the payloads it
/// actually got, so the archive is re-placed whole and the entries protection made unreadable, or
/// left with no name at all, are simply not carried over. Those are the blocks the drop count
/// names.
///
/// It is a re-encode, not a byte-for-byte copy. Every carried entry is recompressed, which is the
/// price of a healthy container. Use <c>bytes-diff</c> afterwards to confirm the payloads still
/// match the source by content.
/// </summary>
public static class MpqRebuildCommand
{
    public static MpqRebuildResult Execute(string path, string? outPath, bool growHash)
    {
        string dest = MpqStripBookkeepingCommand.ResolveOutput(path, outPath);
        var doc = MapDocument.Load(path);
        int inputEntries = doc.Files.Count;

        uint inputSlots = 0;
        try { inputSlots = MpqHashTableCommand.Read(path).Slots; }
        catch (InvalidDataException) { /* no readable header, reported as zero */ }

        var entries = new List<MpqWriteEntry>();
        var dropped = new List<MpqRebuildDrop>();
        foreach (var file in doc.Files)
        {
            // A file the map's own object data and script name, or a standard name the probe
            // recovered, is re-addable. A nameless block is not, an MPQ locates a file by the
            // hash of its name and this one has none.
            string? name = file.FileName ?? file.RecoveredFileName;
            if (name is null)
            {
                dropped.Add(new MpqRebuildDrop($"(unnamed block {file.BlockIndex})", "no name to re-add it under"));
                continue;
            }
            // The read that drops the protection blocks. A payload that will not come back, or an
            // entry protection stuffed with a declared size but no readable bytes, reads empty and
            // is left out rather than written as an empty file.
            if (!doc.TryReadFileByName(name, out var bytes) || bytes.Length == 0)
            {
                dropped.Add(new MpqRebuildDrop(name, "no readable payload"));
                continue;
            }
            entries.Add(new MpqWriteEntry(name, bytes));
        }

        ushort? tableSize = growHash ? MpqArchiveWriter.GrowFor(inputSlots, entries.Count) : null;
        byte[] rebuilt = MpqArchiveWriter.Write(entries, doc.PreArchiveData, tableSize);

        string? dir = Path.GetDirectoryName(dest);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        File.WriteAllBytes(dest, rebuilt);

        bool reopens = true;
        string? reopenError = null;
        int outputEntries = 0;
        uint outputSlots = 0;
        try
        {
            outputEntries = MapDocument.Load(rebuilt).Files.Count;
            outputSlots = MpqHashTableCommand.Read(dest).Slots;
        }
        catch (Exception ex)
        {
            reopens = false;
            reopenError = ex.Message;
        }

        return new MpqRebuildResult(path, dest, inputEntries, outputEntries, dropped.Count,
            dropped, inputSlots, outputSlots, rebuilt.LongLength, reopens, reopenError);
    }
}
