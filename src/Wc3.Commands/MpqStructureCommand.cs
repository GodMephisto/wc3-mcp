// src/Wc3.Commands/MpqStructureCommand.cs
using System.Buffers.Binary;
using War3Net.IO.Mpq;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>How one file is physically stored, as opposed to what it contains.</summary>
public sealed record EntryEncoding(
    string? Name,
    uint Flags,
    uint CompressedSize,
    uint FileSize,
    uint FilePos);

public sealed record ArchiveStructure(
    string Path,
    long FileBytes,
    int ArchiveOffset,
    ushort FormatVersion,
    ushort SectorShift,
    uint HashTableSize,
    uint BlockTableSize,
    int OccupiedHashSlots,
    int MaxProbeDistance,
    IReadOnlyList<EntryEncoding> Entries);

public sealed record StructureDiff(
    ArchiveStructure A,
    ArchiveStructure B,
    IReadOnlyList<string> HeaderDifferences,
    IReadOnlyList<string> EncodingDifferences,
    IReadOnlyList<string> OnlyInA,
    IReadOnlyList<string> OnlyInB)
{
    public bool Identical => HeaderDifferences.Count == 0 && EncodingDifferences.Count == 0
        && OnlyInA.Count == 0 && OnlyInB.Count == 0;
}

/// <summary>
/// Reads and compares MPQ archives at the CONTAINER level, which is the blind spot that cost
/// this project days. <c>diff</c> compares file CONTENTS, <c>roundtrip</c> excludes MPQ
/// bookkeeping by design, and <c>validate</c>/pjass only look at the script. So a rebuilt
/// archive whose every byte of every file was identical still failed to load, and no tool we
/// owned could say why: the difference was in HOW files were stored, not WHAT was stored.
///
/// This reports the physical facts a loader actually consumes - format version, sector size,
/// hash table capacity and crowding, and per-file storage flags, compressed size and offset -
/// so "our rebuild differs from the original in this specific way" becomes an observation
/// instead of a guess.
/// </summary>
public static class MpqStructureCommand
{
    /// <summary>An MPQ hash table entry is 16 bytes: nameA, nameB, locale+platform, block index.</summary>
    private const int HashEntryBytes = 16;

    /// <summary>The archive begins after any prefix block (a .w3x carries a 512-byte HM3W header,
    /// a protected map often carries none), so locate the magic rather than assume an offset.</summary>
    private static int FindArchiveOffset(byte[] bytes)
    {
        for (int i = 0; i + 4 <= bytes.Length; i += 512)
            if (bytes[i] == 'M' && bytes[i + 1] == 'P' && bytes[i + 2] == 'Q' && bytes[i + 3] == 0x1A)
                return i;
        return -1;
    }

    public static ArchiveStructure Read(string path)
    {
        var bytes = File.ReadAllBytes(path);
        int offset = FindArchiveOffset(bytes);
        if (offset < 0) throw new InvalidDataException($"No MPQ archive magic found in {path}.");

        var h = bytes.AsSpan(offset);
        ushort version = BinaryPrimitives.ReadUInt16LittleEndian(h[12..]);
        ushort sectorShift = BinaryPrimitives.ReadUInt16LittleEndian(h[14..]);
        uint hashSize = BinaryPrimitives.ReadUInt32LittleEndian(h[24..]);
        uint blockSize = BinaryPrimitives.ReadUInt32LittleEndian(h[28..]);

        using var fs = File.OpenRead(path);
        using var archive = MpqArchive.Open(fs, loadListFile: true);
        archive.AddFileNames(StandardMapFileNames.All);

        var entries = archive
            .Select(e => new EntryEncoding(e.FileName, (uint)e.Flags, e.CompressedSize,
                e.FileSize, e.FilePosition))
            .ToList();

        // Hash-table crowding, read from the RAW table rather than via MpqArchive.EnumerateHashes,
        // which yields only the occupied entries and so reports occupancy as if it were capacity.
        // Getting that wrong once already produced a nonsense metric (occupied == capacity, and a
        // "longest run" equal to the whole table), so decrypt the real table via War3Net.
        var slots = ReadHashSlots(path, offset, hashSize);
        int occupied = slots.Count(x => x);
        int maxProbe = LongestOccupiedRun(slots);

        return new ArchiveStructure(path, new FileInfo(path).Length, offset, version, sectorShift,
            hashSize, blockSize, occupied, maxProbe, entries);
    }

    /// <summary>
    /// One flag per hash slot, true when occupied. The table is encrypted with a well-known key,
    /// so this hands the raw bytes to War3Net's own <see cref="HashTable"/> rather than
    /// reimplementing MPQ's cipher (an earlier hand-rolled attempt silently produced noise).
    /// </summary>
    private static List<bool> ReadHashSlots(string path, int archiveOffset, uint hashTableSize)
    {
        try
        {
            using var fs = File.OpenRead(path);
            using var br = new BinaryReader(fs);
            fs.Position = archiveOffset;
            var header = new byte[32];
            _ = fs.Read(header, 0, header.Length);
            uint hashTablePos = BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(16));

            fs.Position = archiveOffset + hashTablePos;
            var raw = br.ReadBytes(checked((int)(hashTableSize * HashEntryBytes)));
            if (raw.Length != hashTableSize * HashEntryBytes) return new List<bool>();
            MpqCrypto.DecryptInPlace(raw, MpqCrypto.HashString("(hash table)", 3));

            var slots = new List<bool>((int)hashTableSize);
            for (int i = 0; i < hashTableSize; i++)
            {
                // A slot's last uint32 is the block index. 0xFFFFFFFF is never-used and
                // 0xFFFFFFFE is deleted; both are free for probing purposes.
                uint blockIndex = BinaryPrimitives.ReadUInt32LittleEndian(
                    raw.AsSpan(i * HashEntryBytes + 12));
                slots.Add(blockIndex < 0xFFFFFFFE);
            }
            return slots;
        }
        catch
        {
            // A protected archive can point its table somewhere unreadable. Report nothing rather
            // than a fabricated number, so a caller can tell "unknown" from "not crowded".
            return new List<bool>();
        }
    }

    /// <summary>
    /// The longest unbroken run of occupied slots, treating the table as circular. This IS the
    /// worst-case probe length for a lookup that misses, which is what the game does thousands of
    /// times while loading a map's assets.
    /// </summary>
    private static int LongestOccupiedRun(List<bool> occupied)
    {
        int n = occupied.Count;
        if (n == 0 || occupied.All(x => x)) return n;
        int best = 0, run = 0;
        for (int i = 0; i < n * 2; i++)
        {
            if (occupied[i % n]) { run++; best = Math.Max(best, run); }
            else run = 0;
        }
        return Math.Min(best, n);
    }

    public static StructureDiff Diff(string pathA, string pathB)
    {
        var a = Read(pathA);
        var b = Read(pathB);

        var header = new List<string>();
        void Cmp(string label, object x, object y)
        {
            if (!Equals(x, y)) header.Add($"{label}: {x} -> {y}");
        }
        Cmp("file bytes", a.FileBytes, b.FileBytes);
        Cmp("archive offset", a.ArchiveOffset, b.ArchiveOffset);
        Cmp("format version", a.FormatVersion, b.FormatVersion);
        Cmp("sector size", 512 << a.SectorShift, 512 << b.SectorShift);
        Cmp("hash table size", a.HashTableSize, b.HashTableSize);
        Cmp("block table size", a.BlockTableSize, b.BlockTableSize);
        Cmp("occupied hash slots", a.OccupiedHashSlots, b.OccupiedHashSlots);
        Cmp("longest probe run", a.MaxProbeDistance, b.MaxProbeDistance);

        var byNameA = NamedMap(a);
        var byNameB = NamedMap(b);

        var encoding = new List<string>();
        foreach (var (name, ea) in byNameA)
        {
            if (!byNameB.TryGetValue(name, out var eb)) continue;
            var bits = new List<string>();
            if (ea.Flags != eb.Flags) bits.Add($"flags 0x{ea.Flags:X8} -> 0x{eb.Flags:X8}");
            if (ea.CompressedSize != eb.CompressedSize)
                bits.Add($"stored {ea.CompressedSize} -> {eb.CompressedSize} bytes");
            if (ea.FileSize != eb.FileSize) bits.Add($"size {ea.FileSize} -> {eb.FileSize}");
            if (bits.Count > 0) encoding.Add($"{name}: {string.Join(", ", bits)}");
        }

        return new StructureDiff(a, b, header, Cap(encoding),
            Cap(byNameA.Keys.Except(byNameB.Keys).ToList()),
            Cap(byNameB.Keys.Except(byNameA.Keys).ToList()));
    }

    private static Dictionary<string, EntryEncoding> NamedMap(ArchiveStructure s)
    {
        var d = new Dictionary<string, EntryEncoding>(StringComparer.OrdinalIgnoreCase);
        foreach (var e in s.Entries)
            if (e.Name is not null) d[e.Name] = e;
        return d;
    }

    private static IReadOnlyList<string> Cap(List<string> items) =>
        items.Count <= 40 ? items : items.Take(40).Append($"... and {items.Count - 40} more").ToList();
}
