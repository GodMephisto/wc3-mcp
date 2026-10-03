// src/Wc3.Commands/SkinFieldPartition.cs
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>Which of a kind's two object-data files a field belongs in.</summary>
public enum ObjectLayer { Map, Skin }

/// <summary>
/// Which object-data layer each field code belongs in, learned from the map itself.
///
/// Reforged splits one kind's object data across two files. <c>war3map.w3u</c> holds the gameplay
/// fields and <c>war3mapSkin.w3u</c> holds the presentation fields, and the editor decides the split
/// per field code, not per object. The two are merged at load time with the skin layer on top, so a
/// presentation field written into the base layer while the skin layer holds a value for it is
/// silently discarded, and a presentation field written into the base layer of an object with no
/// skin entry at all is (on the maps measured) simply not honoured.
///
/// Nothing in the file format states the split, so it is measured instead of assumed. Every field
/// code is counted in both layers across every object of the kind, and the layer holding more of
/// them wins. On a real Reforged map the margin is not close: of 2207 units in one map, 1551 put the
/// model field in the skin layer and one put it in the base layer, and that one was an object this
/// tool had installed. The vote is therefore robust to a handful of objects that break the pattern,
/// and to this tool's own earlier mistakes being present in the map it is learning from.
///
/// A map with no skin file at all is a classic (non Reforged) map, where the base layer is the whole
/// truth and every field routes there.
/// </summary>
public sealed class SkinFieldPartition
{
    private readonly HashSet<string> _skinFields;

    private SkinFieldPartition(bool hasSkinLayer, HashSet<string> skinFields)
    {
        HasSkinLayer = hasSkinLayer;
        _skinFields = skinFields;
    }

    /// <summary>False for a classic map, where everything routes to war3map.*.</summary>
    public bool HasSkinLayer { get; }

    /// <summary>Field codes this map keeps in its skin layer.</summary>
    public IReadOnlyCollection<string> SkinFields => _skinFields;

    /// <summary>
    /// Measures the split for one kind by counting each field code in both of the kind's files.
    /// </summary>
    public static SkinFieldPartition Learn(MapDocument doc, ObjectKind kind)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var info = ObjectKinds.Info(kind);
        var skinModel = doc.GetFile(info.SkinFile)?.Model;
        if (skinModel is null)
            return new SkinFieldPartition(false, new HashSet<string>(StringComparer.OrdinalIgnoreCase));

        var mapCounts = Count(doc.GetFile(info.MapFile)?.Model);
        var skinCounts = Count(skinModel);

        var skinFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var (code, skinUses) in skinCounts)
            if (skinUses > mapCounts.GetValueOrDefault(code))
                skinFields.Add(code);

        return new SkinFieldPartition(true, skinFields);
    }

    /// <summary>
    /// The layer a field belongs in. <paramref name="field"/> may be a bare code or "code:N",
    /// since the split is per field code and does not vary by level or variation.
    /// </summary>
    public ObjectLayer LayerFor(string field)
    {
        if (!HasSkinLayer) return ObjectLayer.Map;
        int colon = field.IndexOf(':');
        var code = colon < 0 ? field : field[..colon];
        return _skinFields.Contains(code) ? ObjectLayer.Skin : ObjectLayer.Map;
    }

    private static Dictionary<string, int> Count(object? model)
    {
        var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var id in FieldIds(model))
        {
            var code = id.ToRawcode();
            counts[code] = counts.GetValueOrDefault(code) + 1;
        }
        return counts;
    }

    // Every modification's field id, across both tables and all three modification shapes.
    private static IEnumerable<int> FieldIds(object? model) => model switch
    {
        UnitObjectData m => Simple(m.BaseUnits, m.NewUnits),
        ItemObjectData m => Simple(m.BaseItems, m.NewItems),
        DestructableObjectData m => Simple(m.BaseDestructables, m.NewDestructables),
        BuffObjectData m => Simple(m.BaseBuffs, m.NewBuffs),
        AbilityObjectData m => Level(m.BaseAbilities, m.NewAbilities),
        UpgradeObjectData m => Level(m.BaseUpgrades, m.NewUpgrades),
        DoodadObjectData m => Variation(m.BaseDoodads, m.NewDoodads),
        _ => Enumerable.Empty<int>(),
    };

    private static IEnumerable<int> Simple(
        List<SimpleObjectModification> a, List<SimpleObjectModification> b) =>
        a.Concat(b).SelectMany(g => g.Modifications).Select(m => m.Id);

    private static IEnumerable<int> Level(
        List<LevelObjectModification> a, List<LevelObjectModification> b) =>
        a.Concat(b).SelectMany(g => g.Modifications).Select(m => m.Id);

    private static IEnumerable<int> Variation(
        List<VariationObjectModification> a, List<VariationObjectModification> b) =>
        a.Concat(b).SelectMany(g => g.Modifications).Select(m => m.Id);
}
