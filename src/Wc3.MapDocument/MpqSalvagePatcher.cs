// src/Wc3.MapDocument/MpqSalvagePatcher.cs
using System.Buffers.Binary;

namespace Wc3.Model;

/// <summary>
/// Saves a map whose archive cannot be rebuilt, by patching the original bytes in place
/// instead of re-encoding the archive. Every original byte is kept. Edited files are
/// appended after the archive's current end, and only the 16-byte block table rows of the
/// edited entries change (plus, for added files, one hash table slot).
/// </summary>
/// <remarks>
/// Why a patcher exists at all, measured on the two protected maps in the library
/// (ORDR_S2_2.305[R] and PumpkinTD_v2.3b, 65,534 live hash entries each). A rebuild via
/// <c>MpqArchiveBuilder(originalArchive)</c> dies eagerly on a handful of entries whose
/// block rows protection has corrupted (22 and 8). Rebuilding from scratch and carrying
/// entries one at a time is no fix either, and that is worth recording so nobody retries
/// it. The stuffed entries alias each other's byte ranges, so the per-entry compressed
/// sizes sum to 686 GB in a 134 MB file, and even the entries War3Net reports as readable
/// claim 9.3 GB. Any rebuild that copies per-entry ranges either explodes or has to drop
/// ~49,000 entries, and dropping them punches holes in the hash probe chains the game
/// walks when it resolves the map's REAL files by name, a breakage no offline check can
/// bound. Patching in place sidesteps all of it, the stuffing survives byte for byte and
/// the probe chains stay intact.
/// </remarks>
internal static class MpqSalvagePatcher
{
    internal sealed record Result(
        byte[] Bytes, int ReplacedEntries, int AddedEntries, int NamedEntries, long AppendedBytes);

    // MPQ v0 header field offsets, relative to the archive header start.
    private const int HashTablePosOffset = 0x10;
    private const int BlockTablePosOffset = 0x14;
    private const int HashTableSizeOffset = 0x18;
    private const int BlockTableSizeOffset = 0x1C;

    // Bytes per row in either table (hash row and block row are both 16 bytes).
    private const int TableRowSize = 16;

    // The format's ceiling for either table. War3Net's BlockTable constructor refuses
    // anything larger on open, so growing past it would produce a map nothing can reload.
    private const uint MaxTableSize = 0x10000;

    // A block row's flags for a patched payload. Exists only, stored raw. Compression or
    // encryption would buy nothing here and every extra transform is another way for a
    // salvaged save to diverge from what was verified.
    private const uint FlagsExistsOnly = 0x80000000;

    // Hash slot sentinels, from the MPQ format. An EMPTY slot terminates the game's probe
    // walk, a DELETED slot does not, which is why an insertion may claim either.
    private const uint HashSlotEmpty = 0xFFFFFFFF;
    private const uint HashSlotDeleted = 0xFFFFFFFE;

    /// <summary>
    /// Produces the patched map bytes. Dirty entries whose <paramref name="dirtyEntries"/>
    /// BlockIndex lies inside the archive's block table are replaced in place. Entries
    /// beyond it (files added after load) become new hash and block rows, capacity
    /// permitting. Throws <see cref="NotSupportedException"/> when the archive has no room
    /// for an addition, and <see cref="InvalidDataException"/> when the tables cannot be
    /// located, both of which the caller reports rather than hides.
    /// </summary>
    internal static Result PatchSave(
        byte[] originalBytes,
        int headerOffset,
        IReadOnlyList<(int BlockIndex, string? FileName, byte[]? Payload)> dirtyEntries)
    {
        var header = ReadHeader(originalBytes, headerOffset);

        var blockTable = new byte[header.BlockTableSize * TableRowSize];
        Array.Copy(originalBytes, header.BlockTableAbsolute, blockTable, 0, blockTable.Length);
        DecryptBlock(blockTable, BlockTableKey);

        var replacements = new List<(int BlockIndex, byte[] Payload)>();
        var additions = new List<(string FileName, byte[] Payload)>();
        // A recovered name for an entry that already has a block row, a deprotect pass naming a
        // file the archive lists without one. The payload is null because the bytes do not change,
        // only the hash slot that resolves the name to the existing row. Rewriting the payload
        // instead would append the whole file again for every recovered name.
        var nameOnly = new List<(int BlockIndex, string FileName)>();
        foreach (var (blockIndex, fileName, payload) in dirtyEntries)
        {
            if (blockIndex >= 0 && blockIndex < header.BlockTableSize)
            {
                if (payload is null)
                {
                    if (fileName is null)
                        throw new NotSupportedException(
                            "A name-only entry carries no name to write into the hash table.");
                    nameOnly.Add((blockIndex, fileName));
                }
                else
                {
                    replacements.Add((blockIndex, payload));
                }
            }
            else if (fileName is not null && payload is not null)
                additions.Add((fileName, payload));
            else
                throw new NotSupportedException(
                    "A dirty entry has no name and no existing block row, so the patch cannot place it.");
        }

        // Additions need a free hash slot on the file's own probe chain, and the chain may
        // instead already CONTAIN this name (an entry that is live in the archive but was
        // never named on load). That case is a replacement wearing an addition's clothes,
        // so it is converted rather than inserted twice, a second row with the same name
        // would shadow forever behind the first.
        byte[]? hashTable = null;
        uint newBlockCount = header.BlockTableSize;
        var resolvedAdditions = new List<(uint HashSlotOffset, ulong HashedName, uint BlockIndex, byte[] Payload)>();
        if (additions.Count > 0 || nameOnly.Count > 0)
        {
            if (header.BlockTableSize + (uint)additions.Count > MaxTableSize)
                throw new NotSupportedException(
                    $"The block table already holds {header.BlockTableSize:N0} of the format's "
                    + $"{MaxTableSize:N0} maximum rows, so no file can be added to this map. "
                    + "Existing files can still be edited.");

            hashTable = new byte[header.HashTableSize * TableRowSize];
            Array.Copy(originalBytes, header.HashTableAbsolute, hashTable, 0, hashTable.Length);
            DecryptBlock(hashTable, HashTableKey);

            // Name an existing row. The name may already resolve, from a curated listfile or a
            // second deprotect pass, in which case the slot is left alone.
            foreach (var (blockIndex, fileName) in nameOnly)
            {
                var slot = FindHashSlot(hashTable, header.HashTableSize, fileName);
                if (slot.ExistingBlockIndex is null)
                    WriteHashSlot(hashTable, slot.SlotOffset, HashedFileName(fileName), (uint)blockIndex);
            }

            foreach (var (fileName, payload) in additions)
            {
                var slot = FindHashSlot(hashTable, header.HashTableSize, fileName);
                if (slot.ExistingBlockIndex is { } existing)
                    replacements.Add(((int)existing, payload));
                else
                {
                    resolvedAdditions.Add((slot.SlotOffset, HashedFileName(fileName), newBlockCount, payload));
                    newBlockCount++;
                }
            }
        }

        // A replacement that fits in its original slot overwrites in place. That keeps the
        // archive's offsets, and therefore the offset-derived encryption keys of every entry
        // that is not being replaced (notably the encrypted (listfile) and (attributes)). Only
        // a payload that outgrew its slot is appended.
        var inPlace = new List<(int BlockIndex, uint Offset, byte[] Payload)>();
        var appended = new List<(int BlockIndex, byte[] Payload)>();
        foreach (var (blockIndex, payload) in replacements)
        {
            var row = blockTable.AsSpan(blockIndex * TableRowSize, TableRowSize);
            uint origOffset = BinaryPrimitives.ReadUInt32LittleEndian(row[..4]);
            uint origStored = BinaryPrimitives.ReadUInt32LittleEndian(row[4..8]);
            if (payload.Length <= origStored)
                inPlace.Add((blockIndex, origOffset, payload));
            else
                appended.Add((blockIndex, payload));
        }

        // Appended payloads land after the archive's current end. Offsets in a block row are
        // 32-bit and relative to the header, so the growth has a hard ceiling worth checking
        // rather than letting a wrapped offset corrupt the map quietly.
        long appendAt = originalBytes.Length;
        long appendedBytes = appended.Sum(r => (long)r.Payload.Length)
                           + resolvedAdditions.Sum(a => (long)a.Payload.Length);
        long lastRelative = appendAt + appendedBytes - headerOffset;
        if (lastRelative > uint.MaxValue)
            throw new NotSupportedException(
                "The patched payloads would push the archive past the 4 GB the MPQ format can address.");

        if (resolvedAdditions.Count > 0)
        {
            var grown = new byte[newBlockCount * TableRowSize];
            blockTable.CopyTo(grown, 0);
            blockTable = grown;
        }

        foreach (var (blockIndex, offset, payload) in inPlace)
            WriteBlockRow(blockTable, blockIndex, offset, payload);

        long cursor = appendAt;
        foreach (var (blockIndex, payload) in appended)
        {
            WriteBlockRow(blockTable, blockIndex, (uint)(cursor - headerOffset), payload);
            cursor += payload.Length;
        }
        foreach (var (slotOffset, hashedName, blockIndex, payload) in resolvedAdditions)
        {
            WriteBlockRow(blockTable, (int)blockIndex, (uint)(cursor - headerOffset), payload);
            cursor += payload.Length;
            WriteHashSlot(hashTable!, slotOffset, hashedName, blockIndex);
        }

        EncryptBlock(blockTable, BlockTableKey);

        // Without additions the block table keeps its size, so it goes back over its own
        // region and the header stays untouched. With additions it has grown, so it moves
        // to the end and the header points there. Table encryption chains every row into
        // the next, which is why the whole table re-encrypts even for one patched row.
        bool relocateBlockTable = resolvedAdditions.Count > 0;
        long tail = relocateBlockTable ? blockTable.Length : 0;
        var output = new byte[cursor + tail];
        originalBytes.CopyTo(output, 0);
        foreach (var (_, offset, payload) in inPlace)
            payload.CopyTo(output, headerOffset + offset);
        long copyCursor = appendAt;
        foreach (var (_, payload) in appended)
        {
            payload.CopyTo(output, copyCursor);
            copyCursor += payload.Length;
        }
        foreach (var (_, _, _, payload) in resolvedAdditions)
        {
            payload.CopyTo(output, copyCursor);
            copyCursor += payload.Length;
        }

        if (relocateBlockTable)
        {
            blockTable.CopyTo(output, cursor);
            BinaryPrimitives.WriteUInt32LittleEndian(
                output.AsSpan(headerOffset + BlockTablePosOffset, 4), (uint)(cursor - headerOffset));
            BinaryPrimitives.WriteUInt32LittleEndian(
                output.AsSpan(headerOffset + BlockTableSizeOffset, 4), newBlockCount);
        }
        else
        {
            blockTable.CopyTo(output, header.BlockTableAbsolute);
        }

        if (hashTable is not null)
        {
            EncryptBlock(hashTable, HashTableKey);
            hashTable.CopyTo(output, header.HashTableAbsolute);
        }

        return new Result(output, replacements.Count, resolvedAdditions.Count, nameOnly.Count, appendedBytes + tail);
    }

    /// <summary>
    /// Compacting save. Writes every entry's stored payload back to back in block order, copies
    /// the source hash table verbatim so every probe chain stays exactly as the author placed it,
    /// and rebuilds only the block table. A rebuild re-places every hash slot, and on a map whose
    /// new chains are long the game hosts fine then stalls forever on the loading screen while
    /// every content check passes. DevoProtect writes maps this way for the same reason
    /// (shrink.py, <c>_write</c>). Only dirty entries change their bytes, unchanged entries keep
    /// their original stored bytes and flags.
    /// </summary>
    internal static Result CompactSave(
        byte[] originalBytes,
        int headerOffset,
        IReadOnlyList<(int BlockIndex, byte[] Payload)> dirty)
    {
        var header = ReadHeader(originalBytes, headerOffset);
        int rows = (int)header.BlockTableSize;
        var blockTable = new byte[rows * TableRowSize];
        Array.Copy(originalBytes, header.BlockTableAbsolute, blockTable, 0, blockTable.Length);
        DecryptBlock(blockTable, BlockTableKey);

        var dirtyByIndex = new Dictionary<int, byte[]>(dirty.Count);
        foreach (var (blockIndex, payload) in dirty)
            dirtyByIndex[blockIndex] = payload;

        var payloads = new byte[rows][];
        var flags = new uint[rows];
        var fileSizes = new uint[rows];
        var offsets = new uint[rows];
        // Block offsets are relative to the header, and the first payload sits just past the
        // 32-byte header, so the running offset starts at 32, not 0.
        uint rel = 32;
        for (int bi = 0; bi < rows; bi++)
        {
            var row = blockTable.AsSpan(bi * TableRowSize, TableRowSize);
            uint rowFlags = BinaryPrimitives.ReadUInt32LittleEndian(row[12..]);
            uint storedSize = BinaryPrimitives.ReadUInt32LittleEndian(row[4..]);
            uint origOffset = BinaryPrimitives.ReadUInt32LittleEndian(row[..4]);
            uint origFileSize = BinaryPrimitives.ReadUInt32LittleEndian(row[8..]);

            byte[] payload;
            if (dirtyByIndex.TryGetValue(bi, out var replacement))
            {
                payload = replacement;
                flags[bi] = FlagsExistsOnly;
                fileSizes[bi] = (uint)replacement.Length;
            }
            else
            {
                long abs = headerOffset + (long)origOffset;
                if (abs < 0 || abs + storedSize > originalBytes.Length)
                    throw new InvalidDataException($"Block {bi} lies outside the file.");
                payload = originalBytes.AsSpan((int)abs, (int)storedSize).ToArray();
                flags[bi] = rowFlags;
                fileSizes[bi] = origFileSize;
            }

            payloads[bi] = payload;
            offsets[bi] = rel;
            rel += (uint)payload.Length;
        }

        // Lay the archive out the same way _write does: prefix, header placeholder, payloads,
        // the untouched hash table, then a fresh block table.
        long hashSizeBytes = (long)header.HashTableSize * TableRowSize;
        long blockSizeBytes = (long)rows * TableRowSize;
        long total = headerOffset + 32 + rel + hashSizeBytes + blockSizeBytes;
        if (total > uint.MaxValue)
            throw new NotSupportedException(
                "The compacted archive would exceed the 4 GB the MPQ format can address.");
        if (total > int.MaxValue)
            throw new NotSupportedException("The compacted archive is too large for a managed byte array.");

        var output = new byte[(int)total];
        Array.Copy(originalBytes, 0, output, 0, headerOffset);

        long cursor = headerOffset + 32;
        for (int bi = 0; bi < rows; bi++)
        {
            payloads[bi].CopyTo(output, (int)cursor);
            cursor += payloads[bi].Length;
        }

        long hashAbs = cursor;
        Array.Copy(originalBytes, header.HashTableAbsolute, output, hashAbs, hashSizeBytes);
        cursor += hashSizeBytes;
        long blockAbs = cursor;

        for (int bi = 0; bi < rows; bi++)
        {
            var row = blockTable.AsSpan(bi * TableRowSize, TableRowSize);
            BinaryPrimitives.WriteUInt32LittleEndian(row[..4], offsets[bi]);
            BinaryPrimitives.WriteUInt32LittleEndian(row[4..8], (uint)payloads[bi].Length);
            BinaryPrimitives.WriteUInt32LittleEndian(row[8..12], fileSizes[bi]);
            BinaryPrimitives.WriteUInt32LittleEndian(row[12..16], flags[bi]);
        }
        EncryptBlock(blockTable, BlockTableKey);
        blockTable.CopyTo(output, blockAbs);

        var hdr = output.AsSpan(headerOffset, 32);
        BinaryPrimitives.WriteUInt32LittleEndian(hdr, 0x1A51504D);
        BinaryPrimitives.WriteUInt32LittleEndian(hdr[4..], 32);
        BinaryPrimitives.WriteUInt32LittleEndian(hdr[8..], (uint)(total - headerOffset));
        BinaryPrimitives.WriteUInt16LittleEndian(hdr[12..], 0);
        BinaryPrimitives.WriteUInt16LittleEndian(hdr[14..], SourceBlockShift(originalBytes, headerOffset));
        BinaryPrimitives.WriteUInt32LittleEndian(hdr[16..], (uint)(hashAbs - headerOffset));
        BinaryPrimitives.WriteUInt32LittleEndian(hdr[20..], (uint)(blockAbs - headerOffset));
        BinaryPrimitives.WriteUInt32LittleEndian(hdr[24..], header.HashTableSize);
        BinaryPrimitives.WriteUInt32LittleEndian(hdr[28..], (uint)rows);

        return new Result(output, dirtyByIndex.Count, 0, 0, 0);
    }

    /// <summary>The source header's sector shift, or the 3 (4,096-byte sectors?) the format defaults to.</summary>
    private static ushort SourceBlockShift(byte[] bytes, int headerOffset)
    {
        ushort shift = BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan(headerOffset + 14, 2));
        return shift is >= 1 and <= 15 ? shift : (ushort)3;
    }

    /// <summary>Writes one hash-table row, the hash of a name and the block row it resolves to,
    /// under a neutral locale.</summary>
    private static void WriteHashSlot(byte[] hashTable, uint slotOffset, ulong hashedName, uint blockIndex)
    {
        var slot = hashTable.AsSpan((int)slotOffset, TableRowSize);
        BinaryPrimitives.WriteUInt64LittleEndian(slot, hashedName);
        BinaryPrimitives.WriteUInt32LittleEndian(slot[8..], 0); // neutral locale
        BinaryPrimitives.WriteUInt32LittleEndian(slot[12..], blockIndex);
    }

    private static void WriteBlockRow(byte[] blockTable, int blockIndex, uint relativeOffset, byte[] payload)
    {
        var row = blockTable.AsSpan(blockIndex * TableRowSize, TableRowSize);
        BinaryPrimitives.WriteUInt32LittleEndian(row[..4], relativeOffset);
        BinaryPrimitives.WriteUInt32LittleEndian(row[4..8], (uint)payload.Length);
        BinaryPrimitives.WriteUInt32LittleEndian(row[8..12], (uint)payload.Length);
        BinaryPrimitives.WriteUInt32LittleEndian(row[12..16], FlagsExistsOnly);
    }

    /// <summary>
    /// Walks the file's probe chain the way the game does, from the name's home slot
    /// forward, and reports either the slot already holding this name or the first slot an
    /// insertion may claim. EMPTY and DELETED both accept an insertion, but only EMPTY
    /// ends a lookup, so an existing name deeper in the chain can never be shadowed by
    /// claiming an earlier DELETED slot before checking the whole chain for the name.
    /// </summary>
    private static (uint SlotOffset, uint? ExistingBlockIndex) FindHashSlot(
        byte[] hashTable, uint tableSize, string fileName)
    {
        uint mask = tableSize - 1;
        ulong hashedName = HashedFileName(fileName);
        uint home = HashString(fileName, 0x000) & mask;

        uint? firstFree = null;
        for (uint step = 0; step < tableSize; step++)
        {
            uint slot = (home + step) & mask;
            uint offset = slot * TableRowSize;
            uint blockIndex = BinaryPrimitives.ReadUInt32LittleEndian(hashTable.AsSpan((int)offset + 12, 4));

            if (blockIndex == HashSlotEmpty)
                return (firstFree ?? offset, null);
            if (blockIndex == HashSlotDeleted)
            {
                firstFree ??= offset;
                continue;
            }
            if (BinaryPrimitives.ReadUInt64LittleEndian(hashTable.AsSpan((int)offset, 8)) == hashedName)
                return (offset, blockIndex);
        }

        if (firstFree is { } free)
            return (free, null);
        throw new NotSupportedException(
            $"The hash table's {tableSize:N0} slots are all occupied, so no file can be "
            + "added to this map. Existing files can still be edited.");
    }

    private readonly record struct Header(
        uint HashTableSize, uint BlockTableSize, long HashTableAbsolute, long BlockTableAbsolute);

    private static Header ReadHeader(byte[] bytes, int headerOffset)
    {
        if (headerOffset < 0 || headerOffset + 0x20 > bytes.Length)
            throw new InvalidDataException("The MPQ header lies outside the file.");

        uint hashTablePos = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(headerOffset + HashTablePosOffset, 4));
        uint blockTablePos = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(headerOffset + BlockTablePosOffset, 4));
        uint hashTableSize = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(headerOffset + HashTableSizeOffset, 4));
        uint blockTableSize = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(headerOffset + BlockTableSizeOffset, 4));

        long hashAbs = (long)headerOffset + hashTablePos;
        long blockAbs = (long)headerOffset + blockTablePos;
        if (hashTableSize == 0 || hashTableSize > MaxTableSize || (hashTableSize & (hashTableSize - 1)) != 0)
            throw new InvalidDataException($"Hash table size {hashTableSize} is not a valid power of two.");
        if (blockTableSize > MaxTableSize)
            throw new InvalidDataException($"Block table size {blockTableSize} exceeds the format maximum.");
        if (hashAbs + (long)hashTableSize * TableRowSize > bytes.Length)
            throw new InvalidDataException("The hash table extends past the end of the file.");
        if (blockAbs + (long)blockTableSize * TableRowSize > bytes.Length)
            throw new InvalidDataException("The block table extends past the end of the file.");

        return new Header(hashTableSize, blockTableSize, hashAbs, blockAbs);
    }

    // ---- MPQ table crypto ----
    // War3Net has all of this in StormBuffer, which is internal, so the primitives live
    // here as well. They are the fixed, format-defined algorithm (the StormLib crypt
    // table), verified against War3Net by parsing the library's own maps' hash tables and
    // comparing the result to MpqArchive.EnumerateHashes slot for slot.

    /// <summary>Key that encrypts every MPQ hash table, fixed by the format.</summary>
    internal static uint HashTableKey => HashString("(hash table)", 0x300);

    /// <summary>Key that encrypts every MPQ block table, fixed by the format.</summary>
    internal static uint BlockTableKey => HashString("(block table)", 0x300);

    /// <summary>The 64-bit name identity an MPQ hash slot stores for a file.</summary>
    internal static ulong HashedFileName(string fileName) =>
        HashString(fileName, 0x100) | ((ulong)HashString(fileName, 0x200) << 32);

    internal static uint HashString(string input, int offset)
    {
        var table = CryptTable;
        uint seed1 = 0x7FED7FED, seed2 = 0xEEEEEEEE;
        foreach (int c in input.ToUpperInvariant())
        {
            if (c >= 0x200)
                throw new NotSupportedException(
                    $"'{input}' contains a character outside the range MPQ name hashing supports.");
            seed1 = table[offset + c] ^ (seed1 + seed2);
            seed2 = (uint)(c + (int)seed1 + (int)seed2 + (int)(seed2 << 5) + 3);
        }
        return seed1;
    }

    /// <summary>
    /// Decrypts a whole table buffer in place. The cipher chains each 32-bit word's
    /// plaintext into the next word's key, which is why a single row can never be patched
    /// without re-encrypting everything after it, and why the patcher always rewrites a
    /// table whole.
    /// </summary>
    internal static void DecryptBlock(byte[] data, uint seed1)
    {
        var table = CryptTable;
        uint seed2 = 0xEEEEEEEE;
        for (int i = 0; i + 4 <= data.Length; i += 4)
        {
            seed2 += table[0x400 + (seed1 & 0xFF)];
            uint value = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(i, 4)) ^ (seed1 + seed2);
            seed1 = ((~seed1 << 0x15) + 0x11111111) | (seed1 >> 0x0B);
            seed2 = value + seed2 + (seed2 << 5) + 3;
            BinaryPrimitives.WriteUInt32LittleEndian(data.AsSpan(i, 4), value);
        }
    }

    /// <summary>Inverse of <see cref="DecryptBlock"/>, chaining on the plaintext.</summary>
    internal static void EncryptBlock(byte[] data, uint seed1)
    {
        var table = CryptTable;
        uint seed2 = 0xEEEEEEEE;
        for (int i = 0; i + 4 <= data.Length; i += 4)
        {
            seed2 += table[0x400 + (seed1 & 0xFF)];
            uint plain = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(i, 4));
            uint cipher = plain ^ (seed1 + seed2);
            seed1 = ((~seed1 << 0x15) + 0x11111111) | (seed1 >> 0x0B);
            seed2 = plain + seed2 + (seed2 << 5) + 3;
            BinaryPrimitives.WriteUInt32LittleEndian(data.AsSpan(i, 4), cipher);
        }
    }

    private static readonly uint[] CryptTable = BuildCryptTable();

    private static uint[] BuildCryptTable()
    {
        var table = new uint[0x500];
        uint seed = 0x00100001;
        for (uint index1 = 0; index1 < 0x100; index1++)
        {
            uint index2 = index1;
            for (int i = 0; i < 5; i++, index2 += 0x100)
            {
                seed = (seed * 125 + 3) % 0x2AAAAB;
                uint high = (seed & 0xFFFF) << 0x10;
                seed = (seed * 125 + 3) % 0x2AAAAB;
                table[index2] = high | (seed & 0xFFFF);
            }
        }
        return table;
    }
}
