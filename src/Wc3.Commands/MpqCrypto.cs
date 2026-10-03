// src/Wc3.Commands/MpqCrypto.cs
using System.Buffers.Binary;

namespace Wc3.Commands;

/// <summary>
/// MPQ's hash/decrypt primitives, needed because War3Net keeps its own <c>HashTable</c> internal
/// and the raw hash table is the only way to measure a table's real capacity and crowding
/// (<c>MpqArchive.EnumerateHashes</c> yields occupied entries, so it reports occupancy as if it
/// were capacity).
///
/// Transcribed from StormLib's reference implementation. One detail is easy to get wrong and
/// silently yields plausible-looking noise instead of an error: each crypt-table word is built
/// from the LOW 16 bits of two successive seeds, <c>(s1 &amp; 0xFFFF) &lt;&lt; 16 | (s2 &amp; 0xFFFF)</c>,
/// NOT from the high bits. A hand-rolled version that shifted right produced flag counts near
/// half the table for every mutually-exclusive flag, which is the signature of decrypting to
/// random data. <see cref="SelfCheck"/> exists so that failure can never pass silently again.
/// </summary>
internal static class MpqCrypto
{
    private const int TableWords = 0x500;
    private const int Key2MixOffset = 0x400;

    private static readonly uint[] CryptTable = BuildCryptTable();

    private static uint[] BuildCryptTable()
    {
        var t = new uint[TableWords];
        uint seed = 0x00100001;
        for (uint index1 = 0; index1 < 0x100; index1++)
        {
            for (uint index2 = index1, i = 0; i < 5; i++, index2 += 0x100)
            {
                seed = (seed * 125 + 3) % 0x2AAAAB;
                uint hi = (seed & 0xFFFF) << 0x10;
                seed = (seed * 125 + 3) % 0x2AAAAB;
                uint lo = seed & 0xFFFF;
                t[index2] = hi | lo;
            }
        }
        return t;
    }

    /// <summary>
    /// <paramref name="hashType"/> selects the sub-table: 0 table-offset, 1 name-A, 2 name-B,
    /// 3 file-key. Names are upper-cased and backslash-normalised, as the format requires.
    /// </summary>
    public static uint HashString(string text, int hashType)
    {
        uint seed1 = 0x7FED7FED, seed2 = 0xEEEEEEEE;
        foreach (char raw in text)
        {
            uint ch = char.ToUpperInvariant(raw == '/' ? '\\' : raw);
            seed1 = CryptTable[(hashType << 8) + (int)(ch & 0xFF)] ^ (seed1 + seed2);
            seed2 = ch + seed1 + seed2 + (seed2 << 5) + 3;
        }
        return seed1;
    }

    /// <summary>Decrypts whole 4-byte words in place; a trailing partial word is left alone.</summary>
    public static void DecryptInPlace(byte[] data, uint key)
    {
        uint key2 = 0xEEEEEEEE;
        int words = data.Length / 4;
        for (int i = 0; i < words; i++)
        {
            key2 += CryptTable[Key2MixOffset + (int)(key & 0xFF)];
            var span = data.AsSpan(i * 4, 4);
            uint value = BinaryPrimitives.ReadUInt32LittleEndian(span) ^ (key + key2);
            BinaryPrimitives.WriteUInt32LittleEndian(span, value);
            key = ((~key << 0x15) + 0x11111111) | (key >> 0x0B);
            key2 = value + key2 + (key2 << 5) + 3;
        }
    }

    /// <summary>
    /// Known-answer check on the crypt table. These are the canonical StormLib table keys, so a
    /// mismatch means the table was built wrongly and every decrypt would return noise.
    /// </summary>
    public static bool SelfCheck() =>
        HashString("(hash table)", 3) == 0xC3AF3770
        && HashString("(block table)", 3) == 0xEC83B3A3;
}
