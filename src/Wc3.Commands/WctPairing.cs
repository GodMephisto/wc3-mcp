// src/Wc3.Commands/WctPairing.cs
using War3Net.Build.Script;

namespace Wc3.Commands;

/// <summary>
/// The rule that pairs war3map.wct code bodies with war3map.wtg trigger definitions.
///
/// The two files carry no cross-reference at all. A body is found by POSITION, so getting the
/// position rule wrong shows one trigger's script under another trigger's name, and nothing about
/// the result looks broken. That makes this the single most load-bearing arithmetic in the trigger
/// panel, which is why it lives here once instead of being spelled out at each call site.
///
/// The rule is conditional on the wtg sub-version, and both branches were measured across the
/// whole map library rather than reasoned about (WctPairingRuleSweep, 9 decisive maps and no
/// counter-example, 5 more consistent but unable to distinguish the branches because they have no
/// GUI triggers at all):
///
///   sub-version ABSENT  one slot per TriggerDefinition, and a GUI trigger holds an EMPTY slot.
///                       RATankD_516.2 has 606 slots for 606 definitions with exactly 42 empty
///                       for its 42 GUI triggers.
///
///   sub-version PRESENT one slot per CUSTOM-TEXT definition, and a GUI trigger holds no slot.
///                       Anime_WOS2_0.30d has 109 slots for 115 definitions, 6 GUI triggers and
///                       zero empty slots.
///
/// The reader used to apply the absent-branch rule unconditionally, which mispaired every
/// custom-text body after the first GUI trigger on every sub-version map. In this library that was
/// six versions of Anime_WOS2, the map used for day-to-day testing.
/// </summary>
public static class WctPairing
{
    /// <summary>True when this definition holds its body as text in war3map.wct. The sub-version
    /// format marks that by item type (Script), the classic format by the flag. Honour either.</summary>
    public static bool IsCustomText(TriggerDefinition td) =>
        td.IsCustomTextTrigger || td.Type == TriggerItemType.Script;

    /// <summary>True when GUI definitions occupy a slot of their own (an empty one). This is the
    /// whole of the conditional, expressed once.</summary>
    public static bool GuiTriggersHoldASlot(MapTriggers wtg) => wtg.SubVersion is null;

    /// <summary>
    /// The wct slot index for every <see cref="TriggerDefinition"/> in <paramref name="wtg"/>,
    /// keyed by the definition instance, or -1 for a definition that holds no slot.
    ///
    /// Returned as a map rather than computed per definition on demand, because the correct index
    /// depends on every definition that precedes it, and a caller that recomputes it in a loop
    /// invites the quadratic-cost-plus-subtle-drift pair this file exists to prevent.
    /// </summary>
    public static Dictionary<TriggerDefinition, int> SlotIndices(MapTriggers wtg)
    {
        bool guiHoldsSlot = GuiTriggersHoldASlot(wtg);

        // Keyed by reference on purpose. Two distinct trigger definitions can carry the same Id
        // (measured: Anime_WOS2_0.30d has 24 duplicate ids, because the sub-version format
        // namespaces ids by item type), and a value-equality key would collapse them onto one
        // slot. ReferenceEqualityComparer is IEqualityComparer<object>, so it needs wrapping to
        // be used as a typed comparer rather than cast, which yields null and silently reverts to
        // default equality.
        var map = new Dictionary<TriggerDefinition, int>(ByReference.Instance);

        int next = 0;
        foreach (var td in wtg.TriggerItems.OfType<TriggerDefinition>())
        {
            bool holdsSlot = guiHoldsSlot || IsCustomText(td);
            map[td] = holdsSlot ? next : -1;
            if (holdsSlot) next++;
        }
        return map;
    }

    /// <summary>How many slots the pairing rule expects this tree to need. Compare against the
    /// actual slot count to detect a file whose two halves disagree.</summary>
    public static int ExpectedSlotCount(MapTriggers wtg) =>
        GuiTriggersHoldASlot(wtg)
            ? wtg.TriggerItems.OfType<TriggerDefinition>().Count()
            : wtg.TriggerItems.OfType<TriggerDefinition>().Count(IsCustomText);

    /// <summary>The code body for one definition, or null when it holds no slot, the map has no
    /// wct, or the slot list is shorter than the rule expects. Bodies are stored
    /// null-terminated, so the terminator is trimmed for consumers.</summary>
    public static string? BodyFor(
        MapTriggers wtg, MapCustomTextTriggers? wct, TriggerDefinition td,
        IReadOnlyDictionary<TriggerDefinition, int>? slots = null)
    {
        if (wct is null || !IsCustomText(td)) return null;
        slots ??= SlotIndices(wtg);
        if (!slots.TryGetValue(td, out int i) || i < 0 || i >= wct.CustomTextTriggers.Count)
            return null;
        return wct.CustomTextTriggers[i].Code?.TrimEnd('\0');
    }

    // ---- byte level war3map.wct surgery ------------------------------------------------------

    /// <summary>
    /// Replace one slot's code body in a war3map.wct byte array, copying every other byte verbatim.
    ///
    /// war3map.wct has never been written back through its model, because its decoder is lossy for
    /// bytes that are not valid UTF-8 (measured, one byte of script text destroyed per undecodable
    /// character on a real map, invisible to a round trip because the replacement character
    /// re-encodes to itself). So the safe direction is to leave every byte we do not mean to change
    /// exactly where it is, and touch only the one slot being set. The header, the global block and
    /// every other slot's raw bytes are copied through unchanged.
    ///
    /// The layout walked here is the one Warcraft III and the World Editor use, and it is what
    /// War3Net's reader follows. A little-endian int32 format version (0 for RoC, 1 for TFT and
    /// Reforged). When the version is not 0, a null-terminated comment string, then an int32 global
    /// code length followed by that many bytes. Then an int32 slot count, then that many slots, each
    /// an int32 code length followed by that many bytes. A non-empty length includes the trailing
    /// null byte. Returns false without touching anything when the bytes do not walk cleanly or the
    /// index is out of range.
    /// </summary>
    public static bool TryReplaceSlotBytes(
        byte[] wctBytes, int slotIndex, string newCode, out byte[] updatedWctBytes)
    {
        updatedWctBytes = wctBytes;
        if (!TryParseLayout(wctBytes, out var layout)) return false;
        if (slotIndex < 0 || slotIndex >= layout.Slots.Count) return false;

        var (start, length) = layout.Slots[slotIndex];
        byte[] payload = EncodeCode(newCode);

        byte[] result = new byte[wctBytes.Length - length + 4 + payload.Length];
        Buffer.BlockCopy(wctBytes, 0, result, 0, start);
        int at = start + WriteInt32(result, start, payload.Length);
        Buffer.BlockCopy(payload, 0, result, at, payload.Length);
        at += payload.Length;
        Buffer.BlockCopy(wctBytes, start + length, result, at, wctBytes.Length - (start + length));

        updatedWctBytes = result;
        return true;
    }

    /// <summary>
    /// Remove one slot from a war3map.wct byte array and decrement the slot count, copying every
    /// surviving slot's raw bytes verbatim. This does not decide whether the removal is safe for the
    /// pairing rule (dropping a slot shifts every later slot onto an earlier trigger). That decision
    /// belongs to the caller, see TriggerCommand.Remove. Returns false when the bytes do not walk
    /// cleanly or the index is out of range.
    /// </summary>
    public static bool TryRemoveSlotBytes(byte[] wctBytes, int slotIndex, out byte[] updatedWctBytes)
    {
        updatedWctBytes = wctBytes;
        if (!TryParseLayout(wctBytes, out var layout)) return false;
        if (slotIndex < 0 || slotIndex >= layout.Slots.Count) return false;

        var (start, length) = layout.Slots[slotIndex];
        int newCount = layout.Slots.Count - 1;
        int prefix = layout.CountOffset;
        int suffixStart = start + length;

        byte[] result = new byte[wctBytes.Length - length];
        Buffer.BlockCopy(wctBytes, 0, result, 0, prefix);
        int at = prefix + WriteInt32(result, prefix, newCount);
        Buffer.BlockCopy(wctBytes, prefix + 4, result, at, start - (prefix + 4));
        at += start - (prefix + 4);
        Buffer.BlockCopy(wctBytes, suffixStart, result, at, wctBytes.Length - suffixStart);

        updatedWctBytes = result;
        return true;
    }

    /// <summary>One parsed slot, its byte offset and its full length (the int32 length field plus
    /// the code bytes it announces).</summary>
    private readonly record struct SlotRange(int Start, int Length);

    /// <summary>Where the slot count sits and where each slot's bytes start and end.</summary>
    private sealed record WctLayout(int CountOffset, IReadOnlyList<SlotRange> Slots);

    /// <summary>
    /// Walks the war3map.wct header and slot table, recording offsets only. Returns false rather
    /// than throwing when a length would run past the end of the buffer, so a malformed or truncated
    /// file refuses cleanly instead of producing a wrong splice.
    /// </summary>
    private static bool TryParseLayout(byte[] bytes, out WctLayout layout)
    {
        layout = null!;
        if (bytes is null || bytes.Length < 4) return false;

        int pos = 4;
        int version = ReadInt32(bytes, 0);
        if (version != 0)
        {
            int nul = Array.IndexOf(bytes, (byte)0, pos);
            if (nul < 0) return false;
            pos = nul + 1;
            if (pos + 4 > bytes.Length) return false;
            int globalLength = ReadInt32(bytes, pos);
            pos += 4;
            if (globalLength < 0 || pos + globalLength > bytes.Length) return false;
            pos += globalLength;
        }

        if (pos + 4 > bytes.Length) return false;
        int countOffset = pos;
        int count = ReadInt32(bytes, pos);
        pos += 4;
        if (count < 0) return false;

        // Each slot needs at least its 4-byte length field, so the remaining buffer bounds how many
        // can follow. Pre-sizing to a raw count from a malformed file would let a hostile header
        // allocate arbitrarily before the loop's own bound check ever runs.
        var slots = new List<SlotRange>(Math.Min(count, (bytes.Length - pos) / 4));
        for (int i = 0; i < count; i++)
        {
            int start = pos;
            if (pos + 4 > bytes.Length) return false;
            int codeLength = ReadInt32(bytes, pos);
            pos += 4;
            if (codeLength < 0 || pos + codeLength > bytes.Length) return false;
            pos += codeLength;
            slots.Add(new SlotRange(start, pos - start));
        }

        layout = new WctLayout(countOffset, slots);
        return true;
    }

    /// <summary>UTF-8 bytes for a code body, with a single trailing null when non-empty, matching
    /// the World Editor's own encoding for a slot. An empty body is a zero length and no bytes.</summary>
    private static byte[] EncodeCode(string? code)
    {
        byte[] raw = System.Text.Encoding.UTF8.GetBytes(code ?? string.Empty);
        if (raw.Length == 0) return raw;
        if (raw[^1] == 0) return raw;
        var withNull = new byte[raw.Length + 1];
        Buffer.BlockCopy(raw, 0, withNull, 0, raw.Length);
        return withNull;
    }

    private static int ReadInt32(byte[] bytes, int offset) =>
        bytes[offset] | (bytes[offset + 1] << 8) | (bytes[offset + 2] << 16) | (bytes[offset + 3] << 24);

    private static int WriteInt32(byte[] target, int offset, int value)
    {
        target[offset] = (byte)value;
        target[offset + 1] = (byte)(value >> 8);
        target[offset + 2] = (byte)(value >> 16);
        target[offset + 3] = (byte)(value >> 24);
        return 4;
    }

    private sealed class ByReference : IEqualityComparer<TriggerDefinition>
    {
        public static readonly ByReference Instance = new();
        public bool Equals(TriggerDefinition? a, TriggerDefinition? b) => ReferenceEquals(a, b);
        public int GetHashCode(TriggerDefinition d) =>
            System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(d);
    }
}
