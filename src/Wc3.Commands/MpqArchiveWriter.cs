// src/Wc3.Commands/MpqArchiveWriter.cs
using War3Net.IO.Mpq;

namespace Wc3.Commands;

/// <summary>One named payload destined for a freshly built archive.</summary>
internal sealed record MpqWriteEntry(string Name, byte[] Bytes);

/// <summary>
/// Builds a fresh MPQ archive from named payloads. Shared by rebuild, merge and remove.
///
/// The reason it builds from payloads rather than from the source archive is the whole point of
/// those commands. An <c>MpqArchiveBuilder</c> constructed FROM an archive reads every entry's
/// stream header up front, and a protected map ships block-table rows that are deliberate
/// nonsense, so that constructor dies with War3Net's bare "stream cannot be read". Adding
/// payloads we have ALREADY read one at a time never touches those rows, so the rebuild
/// succeeds and the unreproducible blocks are simply not carried over.
/// </summary>
internal static class MpqArchiveWriter
{
    // War3Net types HashTableSize as ushort, so 65536 is unrepresentable and 2^15 is the ceiling.
    internal const ushort MaxHashTableSize = 32768;

    // A rebuild is unsafe above 3/4 occupancy, MPQ resolves names by linear probing, so a
    // near-full table turns every lookup miss into a long walk. Same threshold MapDocument uses.
    private const int OccupancyNumerator = 3;
    private const int OccupancyDenominator = 4;

    /// <summary>
    /// Writes the entries into a new archive, prefixed by <paramref name="preArchive"/> (the WC3
    /// "HM3W" header a map carries before the MPQ), and returns the file bytes. A null
    /// <paramref name="hashTableSize"/> takes the writer's own default table, which is what a
    /// plain rebuild wants, and a value is passed only when a caller asked for a specific size.
    /// </summary>
    internal static byte[] Write(
        IReadOnlyList<MpqWriteEntry> entries, byte[] preArchive, ushort? hashTableSize)
    {
        var builder = new MpqArchiveBuilder();
        foreach (var e in entries)
            builder.AddFile(MpqFile.New(new MemoryStream(e.Bytes), e.Name));

        using var mpq = new MemoryStream();
        if (hashTableSize is null)
            builder.SaveTo(mpq, leaveOpen: true);
        else
            builder.SaveTo(mpq, new MpqArchiveCreateOptions
            {
                HashTableSize = hashTableSize.Value,
            }, leaveOpen: true);

        using var outStream = new MemoryStream();
        outStream.Write(preArchive, 0, preArchive.Length);
        mpq.Position = 0;
        mpq.CopyTo(outStream);
        return outStream.ToArray();
    }

    /// <summary>
    /// The hash table size for a forced growth, which is the <c>--grow-hash</c> path.
    ///
    /// It starts from double the source table's own slot count and raises it, by powers of two,
    /// until 3/4 occupancy still fits the entry count. Doubling the source guarantees the result
    /// is strictly larger than what the input already had, which is what "grow" has to mean, and
    /// the occupancy loop then makes it large enough for the rebuilt entry set. Capped at the
    /// format ceiling.
    /// </summary>
    internal static ushort GrowFor(uint originalSlots, int entryCount)
    {
        static bool Fits(long entries, long slots) =>
            entries * OccupancyDenominator <= slots * OccupancyNumerator;

        long size = originalSlots == 0 ? 16 : (long)originalSlots * 2;
        while (size < MaxHashTableSize && !Fits(entryCount, size))
            size <<= 1;
        if (size > MaxHashTableSize) size = MaxHashTableSize;
        return (ushort)size;
    }
}
