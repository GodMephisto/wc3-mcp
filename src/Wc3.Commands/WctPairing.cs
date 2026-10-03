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

    private sealed class ByReference : IEqualityComparer<TriggerDefinition>
    {
        public static readonly ByReference Instance = new();
        public bool Equals(TriggerDefinition? a, TriggerDefinition? b) => ReferenceEquals(a, b);
        public int GetHashCode(TriggerDefinition d) =>
            System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(d);
    }
}
