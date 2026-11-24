using System;
using System.Buffers.Binary;

namespace Wc3.Model;

/// <summary>
/// Clears hash table entries that name a block the block table does not hold.
///
/// A rebuild that drops files can leave the hash entry behind marked deleted (0xFFFFFFFE), and
/// War3Net's <c>MpqArchive.AddFileName</c> dereferences that value instead of skipping it, so the
/// whole archive refuses to open over one inert slot. Writing such an entry as the empty entry,
/// all ones, is what the format says a slot with no file is, and the game already ignores them.
/// Measured on the 350 MB BWOS map, three deleted entries, and clearing them turned a
/// MAP_UNREADABLE into a loaded document.
/// </summary>
public static class MpqTableRepair
{
    private const uint Deleted = 0xFFFFFFFE;
    private const uint Empty = 0xFFFFFFFF;
    private const int EntrySize = 16;
    private static readonly uint[] Crypt = BuildCryptTable();

    /// <summary>
    /// Clears every entry that is deleted or that points past the block table, in place, and
    /// returns how many were cleared. Zero means the archive was left byte for byte alone, which
    /// is the case for a healthy archive.
    /// </summary>
    public static int TryNormalize(byte[] fileBytes)
    {
        if (fileBytes is null || fileBytes.Length < 32) return 0;
        int at = MpqHeader.FindArchiveOffset(fileBytes);
        if (at < 0 || at + 32 > fileBytes.Length) return 0;
        Span<byte> head = fileBytes.AsSpan(at, 32);
        if (head[0] != (byte)'M' || head[1] != (byte)'P') return 0;

        uint hashPos = BinaryPrimitives.ReadUInt32LittleEndian(head[16..]);
        uint hashCount = BinaryPrimitives.ReadUInt32LittleEndian(head[24..]);
        uint blockCount = BinaryPrimitives.ReadUInt32LittleEndian(head[28..]);
        if (hashCount == 0 || hashCount > 1 << 20) return 0;
        long start = at + (long)hashPos;
        long end = start + (long)hashCount * EntrySize;
        if (start < 0 || end > fileBytes.Length) return 0;

        int words = (int)hashCount * 4;
        var table = new uint[words];
        Buffer.BlockCopy(fileBytes, (int)start, table, 0, words * 4);
        Decrypt(table, HashString("(hash table)", 3));

        int cleared = Normalize(table, blockCount);
        if (cleared == 0) return 0;

        Encrypt(table, HashString("(hash table)", 3));
        Buffer.BlockCopy(table, 0, fileBytes, (int)start, words * 4);
        return cleared;
    }

    /// <summary>
    /// The decision itself, on the decrypted words. One entry is four words, name1, name2, the
    /// locale and platform word, and the block index. A valid index is left alone, and an empty
    /// slot is left alone, so the method is safe to run twice.
    /// </summary>
    internal static int Normalize(uint[] words, uint blockCount)
    {
        if (words is null) return 0;
        int cleared = 0;
        for (int entry = 0; entry * 4 + 3 < words.Length; entry++)
        {
            int at = entry * 4;
            uint index = words[at + 3];
            if (index == Empty) continue;
            if (index != Deleted && index < blockCount) continue;
            words[at] = Empty;
            words[at + 1] = Empty;
            words[at + 2] = Empty;
            words[at + 3] = Empty;
            cleared++;
        }

        return cleared;
    }

    private static void Decrypt(uint[] data, uint key)
    {
        uint seed = 0xEEEEEEEE;
        for (int i = 0; i < data.Length; i++)
        {
            seed += Crypt[0x400 + (key & 0xFF)];
            uint plain = data[i] ^ (key + seed);
            data[i] = plain;
            key = ((~key << 0x15) + 0x11111111) | (key >> 0x0B);
            seed = plain + seed + (seed << 5) + 3;
        }
    }

    private static void Encrypt(uint[] data, uint key)
    {
        uint seed = 0xEEEEEEEE;
        for (int i = 0; i < data.Length; i++)
        {
            uint plain = data[i];
            data[i] = plain ^ (key + seed);
            key = ((~key << 0x15) + 0x11111111) | (key >> 0x0B);
            seed = plain + seed + (seed << 5) + 3;
        }
    }

    private static uint HashString(string name, uint hashType)
    {
        uint seed1 = 0x7FED7FED;
        uint seed2 = 0xEEEEEEEE;
        foreach (char raw in name)
        {
            char c = char.ToUpperInvariant(raw) == '/' ? '\\' : char.ToUpperInvariant(raw);
            seed1 = Crypt[(hashType << 8) + (byte)c] ^ (seed1 + seed2);
            seed2 = (uint)c + seed1 + seed2 + (seed2 << 5) + 3;
        }

        return seed1;
    }

    private static uint[] BuildCryptTable()
    {
        var table = new uint[0x500];
        uint seed = 0x00100001;
        for (uint i = 0; i < 0x100; i++)
        {
            uint index = i;
            for (int round = 0; round < 5; round++)
            {
                seed = seed * 125 + 3;
                seed %= 0x2AAAAB;
                uint high = (seed & 0xFFFF) << 16;
                seed = seed * 125 + 3;
                seed %= 0x2AAAAB;
                table[index] = high | (seed & 0xFFFF);
                index += 0x100;
            }
        }

        return table;
    }
}
