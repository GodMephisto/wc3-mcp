// src/Wc3.Commands/ObjectDataWriter.cs
using War3Net.Build.Object;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>The three War3Net modification shapes (see ObjectKinds for the mapping).</summary>
internal enum ObjectDataShape { Simple, Level, Variation }

/// <summary>
/// Write-side adapter over one kind's object-data model, hiding the three group/mod
/// type families (Simple/Level/Variation) behind delegates so ObjectSetCommand and
/// ObjectNewCommand can mutate any kind uniformly. FindGroup resolves like the read
/// side: customs by NewId first, then modified standards by OldId. The int slot in
/// FindMod/AddMod is the mod's Level (ability/upgrade) or Variation (doodad); simple
/// kinds ignore it.
/// </summary>
internal sealed record ObjectDataAccess(
    object Model,
    Func<int, object?> FindGroup,
    Func<int, int, object> CreateGroup,                        // (oldId, newId) → group in the right list
    Func<object, int, int, ObjectDataModification?> FindMod,   // (group, fieldId, slot)
    Action<object, int, int, ObjectDataType, object> AddMod);  // (group, fieldId, slot, type, value)

internal static class ObjectDataWriter
{
    internal static ObjectDataShape ShapeOf(ObjectKind kind) => kind switch
    {
        ObjectKind.Ability or ObjectKind.Upgrade => ObjectDataShape.Level,
        ObjectKind.Doodad => ObjectDataShape.Variation,
        _ => ObjectDataShape.Simple,
    };

    /// <summary>The file backing one layer of a kind's object data.</summary>
    internal static string FileFor(ObjectKind kind, ObjectLayer layer)
    {
        var info = ObjectKinds.Info(kind);
        return layer == ObjectLayer.Skin ? info.SkinFile : info.MapFile;
    }

    /// <summary>
    /// The kind's war3map.* model, or a fresh empty one when the map has no such file
    /// yet (format version mirrored from the skin twin when present, else v2). Null
    /// when the file exists but has no parsed model — rebuilding it blind would drop
    /// the original data, so callers must refuse to edit.
    /// </summary>
    internal static object? GetOrCreateMapModel(MapDocument doc, ObjectKind kind) =>
        GetOrCreateModel(doc, kind, ObjectLayer.Map);

    /// <summary>
    /// As <see cref="GetOrCreateMapModel"/>, for either layer. The version of a newly created
    /// file is mirrored from the kind's other layer, so the two halves of one kind never disagree
    /// about the format they are written in.
    /// </summary>
    internal static object? GetOrCreateModel(MapDocument doc, ObjectKind kind, ObjectLayer layer)
    {
        var wanted = FileFor(kind, layer);
        var twin = FileFor(kind, layer == ObjectLayer.Skin ? ObjectLayer.Map : ObjectLayer.Skin);
        if (doc.GetFile(wanted) is { } entry)
            return entry.Model;
        var version = ObjectDataSets.FormatVersionOf(doc.GetFile(twin)?.Model)
            ?? ObjectDataFormatVersion.v2;
        return kind switch
        {
            ObjectKind.Unit => new UnitObjectData(version),
            ObjectKind.Item => new ItemObjectData(version),
            ObjectKind.Ability => new AbilityObjectData(version),
            ObjectKind.Destructable => new DestructableObjectData(version),
            ObjectKind.Doodad => new DoodadObjectData(version),
            ObjectKind.Buff => new BuffObjectData(version),
            ObjectKind.Upgrade => new UpgradeObjectData(version),
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
    }

    /// <summary>Adapter for a model of any of the seven kinds; null for anything else
    /// (e.g. a file that parsed as an unexpected type).</summary>
    internal static ObjectDataAccess? AccessFor(object model) => model switch
    {
        UnitObjectData m => Simple(m, m.BaseUnits, m.NewUnits),
        ItemObjectData m => Simple(m, m.BaseItems, m.NewItems),
        DestructableObjectData m => Simple(m, m.BaseDestructables, m.NewDestructables),
        BuffObjectData m => Simple(m, m.BaseBuffs, m.NewBuffs),
        AbilityObjectData m => Level(m, m.BaseAbilities, m.NewAbilities),
        UpgradeObjectData m => Level(m, m.BaseUpgrades, m.NewUpgrades),
        DoodadObjectData m => Variation(m, m.BaseDoodads, m.NewDoodads),
        _ => null,
    };

    /// <summary>
    /// The distinct slots (ability/upgrade Level, doodad Variation) at which
    /// <paramref name="fieldId"/> already appears anywhere in this model, across every
    /// object, empty when the map never sets it.
    ///
    /// This exists because a leveled table stores two different kinds of field side by
    /// side. A per-level field (Cast Range, Cooldown) starts at level 1, while a field
    /// that is one value for the whole object (Requirements, Levels, Hotkey) is stored
    /// at level 0, and the binary records no flag saying which a field is. Writing a
    /// whole-object field to level 1 produces a modification the game never reads, and
    /// nothing reports a problem, so the map's own usage of the same field is the most
    /// reliable signal available without game-data metadata loaded.
    /// </summary>
    internal static IReadOnlyCollection<int> SlotsUsedFor(object model, int fieldId)
    {
        var slots = new HashSet<int>();
        switch (model)
        {
            case AbilityObjectData m:
                Scan(m.BaseAbilities, m.NewAbilities); break;
            case UpgradeObjectData m:
                Scan(m.BaseUpgrades, m.NewUpgrades); break;
            case DoodadObjectData m:
                foreach (var g in m.BaseDoodads.Concat(m.NewDoodads))
                    foreach (var mod in g.Modifications)
                        if (mod.Id == fieldId) slots.Add(mod.Variation);
                break;
        }
        return slots;

        void Scan(List<LevelObjectModification> bases, List<LevelObjectModification> news)
        {
            foreach (var g in bases.Concat(news))
                foreach (var mod in g.Modifications)
                    if (mod.Id == fieldId) slots.Add(mod.Level);
        }
    }

    private static ObjectDataAccess Simple(
        object model, List<SimpleObjectModification> bases, List<SimpleObjectModification> news) => new(
        model,
        id => news.FirstOrDefault(g => g.NewId == id) ?? bases.FirstOrDefault(g => g.OldId == id),
        (oldId, newId) =>
        {
            var g = new SimpleObjectModification { OldId = oldId, NewId = newId };
            (newId != 0 ? news : bases).Add(g);
            return g;
        },
        (group, fieldId, _) =>
            ((SimpleObjectModification)group).Modifications.FirstOrDefault(m => m.Id == fieldId),
        (group, fieldId, _, type, value) => ((SimpleObjectModification)group).Modifications
            .Add(new SimpleObjectDataModification { Id = fieldId, Type = type, Value = value }));

    private static ObjectDataAccess Level(
        object model, List<LevelObjectModification> bases, List<LevelObjectModification> news) => new(
        model,
        id => news.FirstOrDefault(g => g.NewId == id) ?? bases.FirstOrDefault(g => g.OldId == id),
        (oldId, newId) =>
        {
            var g = new LevelObjectModification { OldId = oldId, NewId = newId };
            (newId != 0 ? news : bases).Add(g);
            return g;
        },
        (group, fieldId, level) => ((LevelObjectModification)group).Modifications
            .FirstOrDefault(m => m.Id == fieldId && m.Level == level),
        (group, fieldId, level, type, value) =>
        {
            var mods = ((LevelObjectModification)group).Modifications;
            mods.Add(new LevelObjectDataModification
            {
                Level = level,
                Pointer = PointerFor(mods, fieldId),
                Id = fieldId,
                Type = type,
                Value = value,
            });
        });

    /// <summary>
    /// The data pointer an added level must carry, taken from the SAME field at any other level.
    /// </summary>
    /// <remarks>
    /// This is the DataA to DataF selector, and it was hardcoded to 0 here. Editing an existing
    /// level kept its pointer because the lookup found the entry, while ADDING a level wrote 0,
    /// so a repair that filled a missing top level produced a field whose levels disagree about
    /// which column they mean.
    ///
    /// Measured on one real map. Across 897 abilities the original carries ZERO Data fields that
    /// mix pointer 0 with a real one, and the build this writer produced carried exactly five,
    /// every one of them the level the repair had added. The player found it before the toolkit
    /// did, reporting a level 6 passive that still did nothing, because nothing in this
    /// repository ever read Pointer back. A read-back through a route that ignores the field
    /// deciding the outcome is not a verification.
    ///
    /// Falls back to 0 only when the field has no other level, which is the genuinely new field
    /// case where there is nothing to copy and 0 is what a fresh non-Data field carries anyway.
    /// </remarks>
    private static int PointerFor(IEnumerable<LevelObjectDataModification> mods, int fieldId)
    {
        foreach (var m in mods)
            if (m.Id == fieldId && m.Pointer != 0)
                return m.Pointer;
        return 0;
    }

    private static ObjectDataAccess Variation(
        object model, List<VariationObjectModification> bases, List<VariationObjectModification> news) => new(
        model,
        id => news.FirstOrDefault(g => g.NewId == id) ?? bases.FirstOrDefault(g => g.OldId == id),
        (oldId, newId) =>
        {
            var g = new VariationObjectModification { OldId = oldId, NewId = newId };
            (newId != 0 ? news : bases).Add(g);
            return g;
        },
        (group, fieldId, variation) => ((VariationObjectModification)group).Modifications
            .FirstOrDefault(m => m.Id == fieldId && m.Variation == variation),
        (group, fieldId, variation, type, value) => ((VariationObjectModification)group).Modifications
            .Add(new VariationObjectDataModification
            { Variation = variation, Pointer = 0, Id = fieldId, Type = type, Value = value }));
}
