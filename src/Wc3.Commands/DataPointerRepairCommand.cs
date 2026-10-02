// src/Wc3.Commands/DataPointerRepairCommand.cs
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One field whose levels disagreed about which Data column they mean.</summary>
public sealed record DataPointerFix(
    string Kind, string Rawcode, string Field, IReadOnlyList<int> Levels, int Pointer);

public sealed record DataPointerRepairResult(
    int ObjectsScanned, int FieldsScanned, int FieldsFixed, int LevelsFixed,
    IReadOnlyList<DataPointerFix> Fixes, IReadOnlyList<string> Diagnostics);

/// <summary>
/// Repairs a levelled modification whose data pointer is 0 while other levels of the SAME field
/// carry a real one. The pointer is the DataA to DataF selector, so a level carrying 0 against
/// siblings carrying 1 names a different column and the engine does not read it as the field the
/// author meant.
///
/// Why this exists as a repair rather than only as a writer fix. The writer hardcoded the
/// pointer to 0 when ADDING a level, which is exactly what filling a missing top level does, so
/// maps were already written and shipped with the defect in them. Fixing the writer stops new
/// ones. This fixes the ones that exist.
///
/// Measured on one real map. The untouched original carries ZERO such fields across 897
/// abilities, so the convention is unambiguous and a mismatch is never the author's intent. The
/// build produced from it carried exactly five, every one a level a repair had added, and one of
/// them was a hero's level 6 passive that a player reported as doing nothing.
///
/// A field where EVERY level carries 0 is left alone. That is an ordinary non-Data field, and 0
/// is what it is supposed to hold, so touching it would invent a defect rather than fix one.
/// </summary>
public static class DataPointerRepairCommand
{
    public static DataPointerRepairResult Execute(MapDocument doc, bool apply)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var fixes = new List<DataPointerFix>();
        var diagnostics = new List<string>();
        int objects = 0, fields = 0, levelsFixed = 0;
        var touched = new HashSet<ObjectKind>();

        foreach (var kind in ObjectKinds.All)
        {
            if (ObjectDataWriter.ShapeOf(kind) != ObjectDataShape.Level) continue;
            var model = ObjectDataWriter.GetOrCreateMapModel(doc, kind);
            if (model is null) continue;

            foreach (var group in LevelGroups(model))
            {
                objects++;
                string raw = (group.NewId != 0 ? group.NewId : group.OldId).ToRawcode();
                foreach (var byField in group.Modifications.GroupBy(m => m.Id))
                {
                    fields++;
                    var mods = byField.ToList();
                    int pointer = mods.Select(m => m.Pointer).FirstOrDefault(p => p != 0);
                    if (pointer == 0) continue;               // an ordinary field, leave it

                    var wrong = mods.Where(m => m.Pointer == 0).ToList();
                    if (wrong.Count == 0) continue;

                    fixes.Add(new DataPointerFix(kind.ToString(), raw,
                        byField.Key.ToRawcode(),
                        wrong.Select(m => m.Level).OrderBy(l => l).ToList(), pointer));
                    levelsFixed += wrong.Count;
                    if (!apply) continue;

                    foreach (var m in wrong) m.Pointer = pointer;
                    touched.Add(kind);
                }
            }

            // Written back once per kind rather than per field, because the whole model is
            // re-serialized either way and doing it per field would rewrite it hundreds of times.
            if (apply && touched.Contains(kind))
                doc.AddOrReplaceModelFile(ObjectKinds.Info(kind).MapFile, model);
        }

        diagnostics.Add($"{objects} levelled object(s) and {fields} field(s) scanned, "
            + $"{fixes.Count} field(s) carrying {levelsFixed} level(s) with a data pointer that "
            + $"disagrees with the rest of the field {(apply ? "corrected" : "would be corrected")}");
        if (fixes.Count == 0)
            diagnostics.Add("note: nothing to do, which on an untouched map is the expected "
                + "result, since the convention is that a field's levels all name one column");

        return new DataPointerRepairResult(objects, fields, fixes.Count, levelsFixed,
            fixes.OrderBy(f => f.Rawcode, StringComparer.Ordinal).ToList(), diagnostics);
    }

    private static IEnumerable<LevelObjectModification> LevelGroups(object model)
    {
        switch (model)
        {
            case AbilityObjectData a:
                foreach (var g in a.BaseAbilities) yield return g;
                foreach (var g in a.NewAbilities) yield return g;
                break;
            case UpgradeObjectData u:
                foreach (var g in u.BaseUpgrades) yield return g;
                foreach (var g in u.NewUpgrades) yield return g;
                break;
        }
    }
}
