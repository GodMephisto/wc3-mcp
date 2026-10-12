// src/Wc3.Commands/MpqMergeCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One name both archives carry, and which side's payload the merge kept.</summary>
public sealed record MpqMergeConflict(string Name, int SizeKept, int SizeOther, string KeptFrom, bool Identical);

/// <summary>The merged copy, the entries taken from each side, and the names they disagreed on.</summary>
public sealed record MpqMergeResult(
    string PathA,
    string PathB,
    string SavedTo,
    string Preferred,
    int EntriesA,
    int EntriesB,
    int MergedEntries,
    int FromA,
    int FromB,
    int Conflicts,
    IReadOnlyList<MpqMergeConflict> ConflictList,
    IReadOnlyList<string> OnlyInA,
    IReadOnlyList<string> OnlyInB,
    int UnnamedDroppedA,
    int UnnamedDroppedB,
    long OutputBytes,
    bool Reopens,
    string? ReopenError);

/// <summary>
/// Merges two map archives into one, taking every named entry from both.
///
/// A name present in both is a conflict. The merge keeps the preferred side's payload and reports
/// the pair, so a caller can see exactly which files the two maps disagreed on and which one won.
/// Every other entry is carried from whichever side has it. Entries with no name cannot be paired
/// across the two archives (a block index means a different thing in each), so they are counted
/// and left out rather than guessed at.
///
/// The output is a fresh archive, the same healthy rebuild as <c>mpq rebuild</c>, so a protected
/// input whose unreadable blocks are dropped is handled the same way on either side.
/// </summary>
public static class MpqMergeCommand
{
    public static MpqMergeResult Execute(string pathA, string pathB, string? outPath, string prefer)
    {
        string dest = MpqStripBookkeepingCommand.ResolveOutput(pathA, outPath);
        // The second input is an original too, so refuse a destination that is it or sits inside it.
        string fullDest = Path.GetFullPath(dest), fullB = Path.GetFullPath(pathB);
        if (fullDest.Equals(fullB, StringComparison.OrdinalIgnoreCase)
            || (Directory.Exists(fullB)
                && fullDest.StartsWith(fullB + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)))
            throw new ArgumentException(
                "output must differ from both input maps, the originals are never written.");
        bool preferB = string.Equals(prefer, "b", StringComparison.OrdinalIgnoreCase);
        if (!preferB && !string.Equals(prefer, "a", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException($"prefer must be 'a' or 'b', not '{prefer}'.", nameof(prefer));

        var docA = MapDocument.Load(pathA);
        var docB = MapDocument.Load(pathB);
        var (namedA, droppedA) = NamedPayloads(docA);
        var (namedB, droppedB) = NamedPayloads(docB);

        // The preferred side is laid down first, then the other side supplies only the names it
        // alone carries, so a shared name keeps the preferred payload without a second write.
        var baseMap = preferB ? namedB : namedA;
        var otherMap = preferB ? namedA : namedB;
        string baseSide = preferB ? "b" : "a";
        string otherSide = preferB ? "a" : "b";

        var entries = new List<MpqWriteEntry>();
        foreach (var (name, bytes) in baseMap)
            entries.Add(new MpqWriteEntry(name, bytes));

        var conflicts = new List<MpqMergeConflict>();
        var onlyOther = new List<string>();
        foreach (var (name, bytes) in otherMap)
        {
            if (baseMap.TryGetValue(name, out var kept))
            {
                conflicts.Add(new MpqMergeConflict(
                    name, kept.Length, bytes.Length, baseSide, kept.AsSpan().SequenceEqual(bytes)));
                continue;
            }
            entries.Add(new MpqWriteEntry(name, bytes));
            onlyOther.Add(name);
        }

        // The names only one side carried, split by which side that was.
        var onlyA = namedA.Keys.Where(k => !namedB.ContainsKey(k))
            .OrderBy(k => k, StringComparer.OrdinalIgnoreCase).ToList();
        var onlyB = namedB.Keys.Where(k => !namedA.ContainsKey(k))
            .OrderBy(k => k, StringComparer.OrdinalIgnoreCase).ToList();
        conflicts = conflicts.OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase).ToList();

        // Pre-archive header, prefer A's, which is the map the output sits beside. A map header is
        // the same 512-byte "HM3W" block on both sides in practice.
        byte[] rebuilt = MpqArchiveWriter.Write(entries, docA.PreArchiveData, hashTableSize: null);

        string? dir = Path.GetDirectoryName(dest);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        File.WriteAllBytes(dest, rebuilt);

        bool reopens = true;
        string? reopenError = null;
        int mergedEntries = 0;
        try { mergedEntries = MapDocument.Load(rebuilt).Files.Count; }
        catch (Exception ex) { reopens = false; reopenError = ex.Message; }

        int fromBase = baseMap.Count;
        int fromOther = onlyOther.Count;
        return new MpqMergeResult(pathA, pathB, dest, preferB ? "b" : "a",
            namedA.Count, namedB.Count, mergedEntries,
            preferB ? fromOther : fromBase, preferB ? fromBase : fromOther,
            conflicts.Count, conflicts, onlyA, onlyB, droppedA, droppedB,
            rebuilt.LongLength, reopens, reopenError);
    }

    /// <summary>The readable named payloads of one archive, keyed by name. The first entry of any
    /// duplicate name wins, matching how the archive resolves a name. Returns the count left out
    /// because the entry has no name or no readable bytes.
    ///
    /// MPQ bookkeeping ((listfile), (attributes), (signature)) is skipped. It is per-archive
    /// metadata the writer regenerates, so two maps almost always disagree on it for reasons that
    /// are not a content conflict, and merging one side's stale copy would only shadow the copy
    /// the rebuild writes.</summary>
    private static (Dictionary<string, byte[]> Named, int Dropped) NamedPayloads(MapDocument doc)
    {
        var named = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
        int dropped = 0;
        foreach (var file in doc.Files)
        {
            string? name = file.FileName ?? file.RecoveredFileName;
            if (name is null) { dropped++; continue; }
            if (RoundtripCommand.MpqSpecialFiles.Contains(name)) continue;
            if (!doc.TryReadFileByName(name, out var bytes) || bytes.Length == 0) { dropped++; continue; }
            named.TryAdd(name, bytes);
        }
        return (named, dropped);
    }
}
