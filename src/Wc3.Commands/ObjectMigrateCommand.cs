// src/Wc3.Commands/ObjectMigrateCommand.cs
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One field removed from the map because the target build does not define it.</summary>
public sealed record MigrateDrop(string Kind, string Rawcode, string FieldCode);

/// <summary>
/// Outcome of migrating a map's object data to a target build's field set. FieldsDropped is
/// the count and Drops lists each removal, so a drop is never silent. UncheckedKinds are the
/// kinds the target schema has no object section for, which were left untouched rather than
/// stripped.
/// </summary>
public sealed record ObjectMigrateResult(
    bool Ok, string Message, string TargetBuild, int FieldsKept, int FieldsDropped,
    IReadOnlyList<MigrateDrop> Drops, IReadOnlyList<string> UncheckedKinds,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Rewrites a map's object data to a target build's field set. Every modification whose bare
/// field code the target build's schema does not define is removed from both Reforged layers,
/// and each removal is reported. A target with no object data section refuses rather than
/// stripping the whole map, since "no section" is "not measured", not "defines nothing".
/// Read-only on its input, the caller saves the edited copy.
/// </summary>
public static class ObjectMigrateCommand
{
    public static ObjectMigrateResult Execute(MapDocument doc, string target, string? schemasDir)
    {
        var schema = TargetBuildSettings.LoadSchema(target, schemasDir);
        return Execute(doc, schema);
    }

    internal static ObjectMigrateResult Execute(MapDocument doc, GameSchema schema)
    {
        var diagnostics = new List<string>();
        if (schema.Objects is null)
            return new ObjectMigrateResult(
                Ok: false,
                Message: $"target {schema.Build} has no object data section in its schema, so nothing "
                    + "could be migrated against it",
                TargetBuild: schema.Build, FieldsKept: 0, FieldsDropped: 0,
                Drops: Array.Empty<MigrateDrop>(), UncheckedKinds: Array.Empty<string>(),
                Diagnostics: diagnostics);

        // The field set the target defines per kind, keyed by the schema file path classifier.
        var validByKind = new Dictionary<ObjectKind, HashSet<string>>();
        foreach (var (file, fields) in schema.Objects)
        {
            string? kindName = TargetCompatCommand.SchemaKindFromPath(file);
            if (kindName is null
                || !Enum.TryParse<ObjectKind>(kindName, ignoreCase: true, out var kind)) continue;
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var field in fields) set.Add(field.Code);
            validByKind[kind] = set;
        }

        var drops = new List<MigrateDrop>();
        var uncheckedKinds = new List<string>();
        int kept = 0;

        foreach (var kind in ObjectKinds.All)
        {
            var info = ObjectKinds.Info(kind);
            string kindName = kind.ToString().ToLowerInvariant();
            if (!validByKind.TryGetValue(kind, out var valid))
            {
                uncheckedKinds.Add(kindName);
                continue;
            }

            foreach (var file in new[] { info.MapFile, info.SkinFile })
            {
                var model = doc.GetFile(file)?.Model;
                if (model is null) continue;
                var (removed, survived) = MigrateModel(model, valid, kindName, drops);
                kept += survived;
                if (removed > 0) doc.AddOrReplaceModelFile(file, model);
            }
        }

        if (uncheckedKinds.Count > 0)
            diagnostics.Add("no object schema section for " + string.Join(", ", uncheckedKinds)
                + ", their fields were left as-is.");

        drops = drops
            .OrderBy(d => d.Kind, StringComparer.Ordinal)
            .ThenBy(d => d.Rawcode, StringComparer.Ordinal)
            .ThenBy(d => d.FieldCode, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return new ObjectMigrateResult(
            Ok: true,
            Message: $"migrated to {schema.Build}, dropped {drops.Count} field(s)", 
            TargetBuild: schema.Build, FieldsKept: kept, FieldsDropped: drops.Count,
            Drops: drops, UncheckedKinds: uncheckedKinds, Diagnostics: diagnostics);
    }

    private static (int Removed, int Kept) MigrateModel(
        object model, HashSet<string> valid, string kindName, List<MigrateDrop> drops)
    {
        int removed = 0, kept = 0;
        switch (model)
        {
            case UnitObjectData m:
                FilterSimple(m.BaseUnits, m.NewUnits); break;
            case ItemObjectData m:
                FilterSimple(m.BaseItems, m.NewItems); break;
            case DestructableObjectData m:
                FilterSimple(m.BaseDestructables, m.NewDestructables); break;
            case BuffObjectData m:
                FilterSimple(m.BaseBuffs, m.NewBuffs); break;
            case AbilityObjectData m:
                FilterLevel(m.BaseAbilities, m.NewAbilities); break;
            case UpgradeObjectData m:
                FilterLevel(m.BaseUpgrades, m.NewUpgrades); break;
            case DoodadObjectData m:
                FilterVariation(m.BaseDoodads, m.NewDoodads); break;
            default:
                return (0, 0);
        }
        return (removed, kept);

        void FilterSimple(List<SimpleObjectModification> bases, List<SimpleObjectModification> news)
        {
            foreach (var g in bases.Concat(news))
                Count(g.NewId != 0 ? g.NewId : g.OldId, g.Modifications);
        }

        void FilterLevel(List<LevelObjectModification> bases, List<LevelObjectModification> news)
        {
            foreach (var g in bases.Concat(news))
                Count(g.NewId != 0 ? g.NewId : g.OldId, g.Modifications);
        }

        void FilterVariation(List<VariationObjectModification> bases, List<VariationObjectModification> news)
        {
            foreach (var g in bases.Concat(news))
                Count(g.NewId != 0 ? g.NewId : g.OldId, g.Modifications);
        }

        void Count<T>(int id, List<T> mods) where T : ObjectDataModification
        {
            for (int i = mods.Count - 1; i >= 0; i--)
            {
                string code = mods[i].Id.ToRawcode();
                if (valid.Contains(code)) { kept++; continue; }
                drops.Add(new MigrateDrop(kindName, id.ToRawcode(), code));
                mods.RemoveAt(i);
                removed++;
            }
        }
    }
}
