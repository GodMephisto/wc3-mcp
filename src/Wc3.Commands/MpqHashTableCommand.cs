// src/Wc3.Commands/MpqHashTableCommand.cs
using System.Buffers.Binary;

namespace Wc3.Commands;

/// <summary>One hash table slot, classified the way the game's own lookup treats it.</summary>
public sealed record HashSlot(int Index, uint NameA, uint NameB, uint Locale, uint BlockIndex, string Kind);

public sealed record HashTableView(
    string Path,
    uint Slots,
    int Occupied,
    int Deleted,
    int NeverUsed,
    int LongestRunWithoutNeverUsed,
    bool LookupCanLoopForever,
    IReadOnlyList<HashSlot> Sample);

/// <summary>
/// Renders an MPQ hash table so a rebuild's placement can be inspected rather than guessed at.
///
/// The distinction this exists for: MPQ resolves a name by hashing it to a home slot and probing
/// FORWARD until it either matches or reaches a slot that was NEVER USED (0xFFFFFFFF). A DELETED
/// slot (0xFFFFFFFE) does NOT stop that scan, because a deleted entry may have displaced the one
/// being searched for. So an archive with zero never-used slots gives a lookup that misses no
/// terminator at all, and the probe runs forever.
///
/// That is a real infinite loop, inside the game's own executable, with no crash and no log entry,
/// which matches a measured hang exactly: one core pinned, ~80% of instruction-pointer samples
/// inside a 30-byte window. Earlier tooling here collapsed deleted and never-used into a single
/// "free" bucket and so could not see it.
/// </summary>
public static class MpqHashTableCommand
{
    private const int SlotBytes = 16;
    private const uint SlotEmpty = 0xFFFFFFFF;      // never used - terminates a probe
    private const uint SlotDeleted = 0xFFFFFFFE;    // deleted - does NOT terminate a probe

    public static HashTableView Read(string path, int sampleLimit = 0)
    {
        var bytes = File.ReadAllBytes(path);
        int archiveOffset = -1;
        for (int i = 0; i + 4 <= bytes.Length; i += 512)
            if (bytes[i] == 'M' && bytes[i + 1] == 'P' && bytes[i + 2] == 'Q' && bytes[i + 3] == 0x1A)
            { archiveOffset = i; break; }
        if (archiveOffset < 0) throw new InvalidDataException($"No MPQ archive magic found in {path}.");

        var h = bytes.AsSpan(archiveOffset);
        uint tablePos = BinaryPrimitives.ReadUInt32LittleEndian(h[16..]);
        uint slots = BinaryPrimitives.ReadUInt32LittleEndian(h[24..]);

        long start = archiveOffset + tablePos;
        long need = (long)slots * SlotBytes;
        if (start < 0 || start + need > bytes.Length)
            throw new InvalidDataException("Hash table lies outside the file.");

        var raw = new byte[need];
        Array.Copy(bytes, start, raw, 0, need);
        MpqCrypto.DecryptInPlace(raw, MpqCrypto.HashString("(hash table)", 3));

        var all = new List<HashSlot>((int)slots);
        int occupied = 0, deleted = 0, never = 0;
        for (int i = 0; i < slots; i++)
        {
            var s = raw.AsSpan(i * SlotBytes);
            uint nameA = BinaryPrimitives.ReadUInt32LittleEndian(s);
            uint nameB = BinaryPrimitives.ReadUInt32LittleEndian(s[4..]);
            uint locale = BinaryPrimitives.ReadUInt32LittleEndian(s[8..]);
            uint block = BinaryPrimitives.ReadUInt32LittleEndian(s[12..]);

            string kind;
            if (block == SlotEmpty) { kind = "never-used"; never++; }
            else if (block == SlotDeleted) { kind = "deleted"; deleted++; }
            else { kind = "occupied"; occupied++; }
            all.Add(new HashSlot(i, nameA, nameB, locale, block, kind));
        }

        // Worst-case probe for a name that is NOT present: how far a scan runs before it meets a
        // never-used slot. If there are none, that scan never ends.
        int longest = LongestRunWithoutTerminator(all);
        bool canLoop = never == 0;

        var sample = sampleLimit <= 0
            ? new List<HashSlot>()
            : all.Take(sampleLimit).ToList();

        return new HashTableView(path, slots, occupied, deleted, never, longest, canLoop, sample);
    }

    private static int LongestRunWithoutTerminator(List<HashSlot> all)
    {
        int n = all.Count;
        if (n == 0) return 0;
        if (all.All(s => s.Kind != "never-used")) return n;
        int best = 0, run = 0;
        for (int i = 0; i < n * 2; i++)
        {
            if (all[i % n].Kind == "never-used") run = 0;
            else { run++; best = Math.Max(best, run); }
        }
        return Math.Min(best, n);
    }

    /// <summary>Slots whose contents differ between two archives, by index.</summary>
    public static IReadOnlyList<string> CompareSlots(HashTableView a, HashTableView b, int limit = 30)
    {
        if (a.Slots != b.Slots)
            return new[] { $"table sizes differ ({a.Slots} vs {b.Slots}), so slots are not comparable" };
        var diffs = new List<string>();
        for (int i = 0; i < a.Sample.Count && i < b.Sample.Count && diffs.Count < limit; i++)
        {
            var x = a.Sample[i]; var y = b.Sample[i];
            if (x.NameA == y.NameA && x.NameB == y.NameB && x.BlockIndex == y.BlockIndex) continue;
            diffs.Add($"slot {i}: {x.Kind} block={Fmt(x.BlockIndex)} -> {y.Kind} block={Fmt(y.BlockIndex)}");
        }
        return diffs;
    }

    private static string Fmt(uint b) =>
        b == SlotEmpty ? "empty" : b == SlotDeleted ? "deleted" : b.ToString();
}
