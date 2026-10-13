// src/Wc3.Commands/ObjectCloneCommand.cs
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

public sealed record ObjectCloneResult(
    bool Ok, string Message, string? NewRawcode = null, int FieldsCopied = 0);

/// <summary>
/// Deep-clones a map object under a new rawcode. It copies every field delta the source
/// carries, and for a leveled kind (ability, upgrade) or a variation kind (doodad) the
/// per-level and per-variation data too, since those are modifications on the same group. The clone
/// keeps the source's base, so a clone of a custom derives from the same base and a
/// clone of a modified standard becomes a custom derived from that standard, exactly
/// the way the World Editor copies an object. Refuses when the new rawcode is already
/// taken by the map. The clone lands in the kind's war3map.* file.
///
/// The modification objects are copied field for field rather than through the
/// normalized read model, because the read model keeps only the value and would drop
/// the per-level Pointer (the DataA to DataF selector) that decides which column a
/// level writes into.
/// </summary>
public static class ObjectCloneCommand
{
    public static ObjectCloneResult Execute(
        MapDocument doc, ObjectKind kind, string rawcode, string newRawcode)
    {
        string kindName = kind.ToString().ToLowerInvariant();
        if (rawcode.Length != 4)
            return new(false, $"invalid rawcode '{rawcode}', expected 4 characters");
        if (newRawcode.Length != 4)
            return new(false, $"invalid new rawcode '{newRawcode}', expected 4 characters");
        if (string.Equals(rawcode, newRawcode, StringComparison.Ordinal))
            return new(false, $"new rawcode '{newRawcode}' is the same as the source rawcode");

        var info = ObjectKinds.Info(kind);
        int sourceId = rawcode.FromRawcode();
        int newId = newRawcode.FromRawcode();

        if (RawcodeAllocator.UsedRawcodes(doc).Contains(newId))
            return new(false, $"{kindName} {newRawcode} already exists in the map");

        if (ObjectKinds.MergedEntries(doc, info).All(e => e.Id != sourceId))
            return new(false, $"{kindName} {rawcode} not found in map");

        // Clone every layer that holds the object. A Reforged map can split one object across
        // war3map.* and war3mapSkin.* (measured: a unit with 16 fields in the map file and 7 in the
        // skin file, name, model and icon among the skin half). Cloning only the map layer dropped
        // that half, so the clone inherited the base unit's name, model and icon rather than the
        // source's. GetOrCreateModel for an absent skin file returns an empty model whose FindGroup
        // is null, so nothing is written there unless the source really has a skin group.
        int copied = 0;
        bool clonedAnyLayer = false;
        foreach (var layer in new[] { ObjectLayer.Map, ObjectLayer.Skin })
        {
            var model = ObjectDataWriter.GetOrCreateModel(doc, kind, layer);
            var access = model is null ? null : ObjectDataWriter.AccessFor(model);
            if (access is null) continue;
            if (access.FindGroup(sourceId) is not { } sourceGroup) continue;

            var clone = access.CreateGroup(BaseIdOf(sourceGroup), newId);
            copied += CopyModifications(sourceGroup, clone);
            doc.AddOrReplaceModelFile(ObjectDataWriter.FileFor(kind, layer), model!);
            clonedAnyLayer = true;
        }
        if (!clonedAnyLayer)
            return new(false, $"{kindName} {rawcode} has no field data to clone");

        return new(true,
            $"cloned {kindName} {rawcode} to {newRawcode} "
            + $"({copied} field value{(copied == 1 ? "" : "s")})",
            newRawcode, copied);
    }

    private static int BaseIdOf(object group) => group switch
    {
        SimpleObjectModification g => g.OldId,
        LevelObjectModification g => g.OldId,
        VariationObjectModification g => g.OldId,
        _ => 0,
    };

    private static int CopyModifications(object source, object target)
    {
        switch (source, target)
        {
            case (SimpleObjectModification s, SimpleObjectModification t):
                foreach (var m in s.Modifications)
                    t.Modifications.Add(new SimpleObjectDataModification
                    { Id = m.Id, Type = m.Type, Value = m.Value });
                return s.Modifications.Count;
            case (LevelObjectModification s, LevelObjectModification t):
                foreach (var m in s.Modifications)
                    t.Modifications.Add(new LevelObjectDataModification
                    { Level = m.Level, Pointer = m.Pointer, Id = m.Id, Type = m.Type, Value = m.Value });
                return s.Modifications.Count;
            case (VariationObjectModification s, VariationObjectModification t):
                foreach (var m in s.Modifications)
                    t.Modifications.Add(new VariationObjectDataModification
                    { Variation = m.Variation, Pointer = m.Pointer, Id = m.Id, Type = m.Type, Value = m.Value });
                return s.Modifications.Count;
            default:
                return 0;
        }
    }
}
