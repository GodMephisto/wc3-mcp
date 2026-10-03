// src/Wc3.MapDocument/ObjectDataSets.cs
using War3Net.Build.Object;

namespace Wc3.Model;

/// <summary>
/// The Reforged per-object "modification set" prefix in object data, and the one value every
/// real map uses for it.
///
/// In a format version 3 file (war3map.w3u/w3a/w3t/w3b/w3d/w3h/w3q and their war3mapSkin
/// twins) each object is laid out as
///
///     oldId, newId, setCount, then per set { setFlags, modCount, modifications... }
///
/// Format versions 1 and 2 have no setCount and no setFlags at all, just oldId, newId,
/// modCount, modifications. War3Net models the v3 prefix as the untyped list
/// <c>Unk</c> on each modification group, reading a count and that many int32s into the
/// list and writing the list's length back out followed by its contents. Surveying five
/// unrelated Reforged maps across all seven kinds found exactly one set with flags 0 on
/// every single object, so the canonical prefix is the one element list { 0 }.
///
/// WHY this class exists. A modification group built in memory (a newly added custom
/// object) starts with an EMPTY <c>Unk</c>, so War3Net wrote setCount 0 and then went
/// straight to the modification count, one int32 short of the layout every other object in
/// the same file uses. Our own reader took those bytes back symmetrically (count 0, no
/// flags), so validate, lint and roundtrip all stayed green. The GAME instead reads
/// setCount 0 as "this object has no modification sets" and resumes parsing the next object
/// from the middle of this one's data, so everything after the first added object is
/// garbage. That crashed Warcraft III before main() ever ran. Proven by bisection with
/// wc3ctl debug trace-load against Anime_WOS2_0.28a2.w3x, where the variant carrying only
/// our rewritten war3map.w3u/w3a/w3h was the single failing one, and confirmed at byte
/// level (the appended objects were the exact point where an independent parser derailed).
/// </summary>
public static class ObjectDataSets
{
    /// <summary>The set flags every object in every real version 3 map carries.</summary>
    public const int DefaultSetFlags = 0;

    /// <summary>
    /// Gives every modification group of a version 3 object-data model the canonical
    /// one-set prefix when it has none yet, and reports how many groups needed one.
    /// Does nothing for a version 1 or 2 model (those formats have no prefix) or for a
    /// model that is not object data, so a caller may hand it any model unconditionally.
    /// Idempotent, a group read from a real map already carries its prefix and is left
    /// exactly as it was, which is what keeps untouched object data byte-faithful.
    /// </summary>
    public static int EnsureSetPrefixes(object? model)
    {
        if (FormatVersionOf(model) is not { } version || version < ObjectDataFormatVersion.v3)
            return 0;

        int seeded = 0;
        foreach (var prefix in SetPrefixes(model!))
            if (prefix.Count == 0)
            {
                prefix.Add(DefaultSetFlags);
                seeded++;
            }
        return seeded;
    }

    /// <summary>
    /// The object-data format version of any of the seven kinds' models, or null when the
    /// model is not object data. Shared so the write-side adapters do not each re-declare
    /// this switch.
    /// </summary>
    public static ObjectDataFormatVersion? FormatVersionOf(object? model) => model switch
    {
        UnitObjectData m => m.FormatVersion,
        ItemObjectData m => m.FormatVersion,
        AbilityObjectData m => m.FormatVersion,
        DestructableObjectData m => m.FormatVersion,
        DoodadObjectData m => m.FormatVersion,
        BuffObjectData m => m.FormatVersion,
        UpgradeObjectData m => m.FormatVersion,
        _ => null,
    };

    // The three War3Net group shapes (Simple, Level, Variation) share no base type, so the
    // seven kinds are enumerated by hand. Both tables matter, an in-place modification of a
    // base object needs the prefix exactly as much as a custom object does.
    private static IEnumerable<List<int>> SetPrefixes(object model) => model switch
    {
        UnitObjectData m => m.BaseUnits.Concat(m.NewUnits).Select(g => g.Unk),
        ItemObjectData m => m.BaseItems.Concat(m.NewItems).Select(g => g.Unk),
        AbilityObjectData m => m.BaseAbilities.Concat(m.NewAbilities).Select(g => g.Unk),
        DestructableObjectData m => m.BaseDestructables.Concat(m.NewDestructables).Select(g => g.Unk),
        DoodadObjectData m => m.BaseDoodads.Concat(m.NewDoodads).Select(g => g.Unk),
        BuffObjectData m => m.BaseBuffs.Concat(m.NewBuffs).Select(g => g.Unk),
        UpgradeObjectData m => m.BaseUpgrades.Concat(m.NewUpgrades).Select(g => g.Unk),
        _ => Enumerable.Empty<List<int>>(),
    };
}
