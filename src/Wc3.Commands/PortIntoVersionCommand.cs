// src/Wc3.Commands/PortIntoVersionCommand.cs
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>An object field the target build's schema does not declare, rewritten out of the
/// ported map. The game ignores such a field on that build, so removing it makes the port
/// honest rather than leaving an edit that silently does nothing.</summary>
public sealed record DowngradedField(string Kind, string Rawcode, string? Name, string FieldCode);

/// <summary>A script identifier the ported map uses that the target build's scripts do not
/// declare. It cannot be rewritten safely (which native replaces it is not knowable here), so
/// it is flagged for the author instead.</summary>
public sealed record UndeclaredScriptIdentifier(string Name, int FirstLine, int Count);

/// <summary>
/// Port a unit and then downgrade the result to a named target build's schema. Object fields
/// the build does not declare are removed (a rewrite, since the build would ignore them
/// anyway) and script identifiers the build lacks are flagged (no safe rewrite exists). The
/// port report is carried whole so a caller sees what was ported alongside what was downgraded.
/// </summary>
public sealed record PortIntoVersionResult(
    string TargetBuild,
    PortResult Port,
    IReadOnlyList<DowngradedField> FieldsRemoved,
    IReadOnlyList<UndeclaredScriptIdentifier> ScriptIdentifiersFlagged,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Downgrades a ported map to a target build. The target resolves the way the compat and
/// target commands resolve it, a schema folder path or a build name under the schemas
/// directory. Porting itself is <see cref="PortCommand.PortUnit"/> unchanged, so this only
/// adds the schema rewrite on top of a normal port.
/// </summary>
public static class PortIntoVersionCommand
{
    public static PortIntoVersionResult Execute(
        MapDocument source, string rawcode, MapDocument target,
        string targetBuild, string? schemasDir = null, string? gameDirOverride = null,
        bool includeScript = true)
    {
        var schema = TargetBuildSettings.LoadSchema(targetBuild, schemasDir);

        var bundle = BundleCommand.ResolveUnit(source, rawcode, gameDirOverride);
        var port = PortCommand.PortUnit(source, bundle, target, includeScript);

        var removed = DowngradeObjects(target, schema);
        var diagnostics = new List<string>();
        var flagged = FlagUndeclaredIdentifiers(target, schema, diagnostics);

        return new PortIntoVersionResult(schema.Build, port, removed, flagged, diagnostics);
    }

    // ---- object field downgrade --------------------------------------------

    /// <summary>
    /// Removes every object field the target schema does not declare, in the map and skin
    /// object layers alike, recording each removal. A kind the schema has no section for is
    /// left untouched, because an absent section is not the same as the build declaring
    /// nothing (see <see cref="GameSchema.Objects"/>).
    /// </summary>
    private static IReadOnlyList<DowngradedField> DowngradeObjects(MapDocument target, GameSchema schema)
    {
        var removed = new List<DowngradedField>();
        if (schema.Objects is null) return removed;

        var validByKind = new Dictionary<ObjectKind, HashSet<string>>();
        foreach (var (file, fields) in schema.Objects)
        {
            var kindName = TargetCompatCommand.SchemaKindFromPath(file);
            if (kindName is null
                || !Enum.TryParse<ObjectKind>(kindName, ignoreCase: true, out var kind)) continue;
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var f in fields) set.Add(f.Code);
            validByKind[kind] = set;
        }

        var strings = MapStrings.From(target);
        foreach (var kind in ObjectKinds.All)
        {
            if (!validByKind.TryGetValue(kind, out var valid)) continue;
            var info = ObjectKinds.Info(kind);
            foreach (var file in new[] { info.MapFile, info.SkinFile })
            {
                var entry = target.GetFile(file);
                if (entry?.Model is null) continue;
                var perFile = new List<DowngradedField>();
                PruneModel(entry.Model, info, valid, perFile, strings);
                if (perFile.Count > 0)
                {
                    target.AddOrReplaceModelFile(file, entry.Model);
                    removed.AddRange(perFile);
                }
            }
        }

        return removed
            .OrderBy(f => f.Kind, StringComparer.Ordinal)
            .ThenBy(f => f.Rawcode, StringComparer.Ordinal)
            .ThenBy(f => f.FieldCode, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static void PruneModel(
        object model, ObjectKindInfo info, HashSet<string> valid,
        List<DowngradedField> removed, MapStrings strings)
    {
        string word = info.Kind.ToString().ToLowerInvariant();
        switch (model)
        {
            case UnitObjectData m:
                PruneSimple(m.BaseUnits.Concat(m.NewUnits), word, info, valid, removed, strings); break;
            case ItemObjectData m:
                PruneSimple(m.BaseItems.Concat(m.NewItems), word, info, valid, removed, strings); break;
            case DestructableObjectData m:
                PruneSimple(m.BaseDestructables.Concat(m.NewDestructables), word, info, valid, removed, strings); break;
            case BuffObjectData m:
                PruneSimple(m.BaseBuffs.Concat(m.NewBuffs), word, info, valid, removed, strings); break;
            case AbilityObjectData m:
                PruneLevel(m.BaseAbilities.Concat(m.NewAbilities), word, info, valid, removed, strings); break;
            case UpgradeObjectData m:
                PruneLevel(m.BaseUpgrades.Concat(m.NewUpgrades), word, info, valid, removed, strings); break;
            case DoodadObjectData m:
                PruneVariation(m.BaseDoodads.Concat(m.NewDoodads), word, info, valid, removed, strings); break;
        }
    }

    private static void PruneSimple(
        IEnumerable<SimpleObjectModification> groups, string word, ObjectKindInfo info, HashSet<string> valid,
        List<DowngradedField> removed, MapStrings strings)
    {
        foreach (var g in groups)
        {
            var delta = g.Modifications.ToDictionary(
                x => x.Id.ToRawcode(), x => ObjectKinds.FormatValue(x.Value), StringComparer.OrdinalIgnoreCase);
            var dead = g.Modifications.Select(x => x.Id.ToRawcode())
                .Where(code => !valid.Contains(code))
                .Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (dead.Count == 0) continue;
            Record(removed, word, g.NewId != 0 ? g.NewId : g.OldId, delta, dead, strings, info);
            g.Modifications.RemoveAll(x => !valid.Contains(x.Id.ToRawcode()));
        }
    }

    private static void PruneLevel(
        IEnumerable<LevelObjectModification> groups, string word, ObjectKindInfo info, HashSet<string> valid,
        List<DowngradedField> removed, MapStrings strings)
    {
        foreach (var g in groups)
        {
            var delta = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var x in g.Modifications)
                delta[x.Level == 0 ? x.Id.ToRawcode() : $"{x.Id.ToRawcode()}:{x.Level}"] =
                    ObjectKinds.FormatValue(x.Value);
            var dead = g.Modifications.Select(x => x.Id.ToRawcode())
                .Where(code => !valid.Contains(code))
                .Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (dead.Count == 0) continue;
            Record(removed, word, g.NewId != 0 ? g.NewId : g.OldId, delta, dead, strings, info);
            g.Modifications.RemoveAll(x => !valid.Contains(x.Id.ToRawcode()));
        }
    }

    private static void PruneVariation(
        IEnumerable<VariationObjectModification> groups, string word, ObjectKindInfo info, HashSet<string> valid,
        List<DowngradedField> removed, MapStrings strings)
    {
        foreach (var g in groups)
        {
            var dead = g.Modifications.Select(x => x.Id.ToRawcode())
                .Where(code => !valid.Contains(code))
                .Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (dead.Count == 0) continue;
            var delta = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var x in g.Modifications)
                delta[x.Variation == 0 ? x.Id.ToRawcode() : $"{x.Id.ToRawcode()}:{x.Variation}"] =
                    ObjectKinds.FormatValue(x.Value);
            Record(removed, word, g.NewId != 0 ? g.NewId : g.OldId, delta, dead, strings, info);
            g.Modifications.RemoveAll(x => !valid.Contains(x.Id.ToRawcode()));
        }
    }

    private static void Record(
        List<DowngradedField> removed, string word, int id,
        IReadOnlyDictionary<string, string> delta, IReadOnlyList<string> deadCodes,
        MapStrings strings, ObjectKindInfo info)
    {
        string rawcode = id.ToRawcode();
        // Resolve the display name before the prune, using the kind's own name field. The name
        // is a label for the report only, so a missing or leveled key is fine.
        string? name = ObjectKinds.DeltaName(delta, info, strings);
        foreach (var code in deadCodes)
            removed.Add(new DowngradedField(word, rawcode, name, code));
    }

    // ---- script identifier flagging ----------------------------------------

    /// <summary>
    /// Every identifier the map's script uses that the target build's common.j and blizzard.j
    /// do not declare. Uses the same tokenizer the compat check uses, so a keyword or a numeric
    /// literal is never mistaken for an identifier, and an identifier the map itself declares is
    /// skipped. A target with no script section is reported rather than judged clean.
    /// </summary>
    private static IReadOnlyList<UndeclaredScriptIdentifier> FlagUndeclaredIdentifiers(
        MapDocument target, GameSchema schema, List<string> diagnostics)
    {
        if (schema.Script is null)
        {
            diagnostics.Add($"target {schema.Build} declares no script section in its schema, "
                + "so its script identifiers could not be checked.");
            return Array.Empty<UndeclaredScriptIdentifier>();
        }

        var known = new HashSet<string>(StringComparer.Ordinal);
        foreach (var (key, api) in schema.Script)
        {
            if (key != "scripts/common.j" && key != "scripts/blizzard.j") continue;
            foreach (var n in api.Natives) known.Add(n.Name);
            foreach (var g in api.Globals) known.Add(g.Name);
            foreach (var t in api.Types) known.Add(t.Name);
            foreach (var f in api.Functions) known.Add(f.Name);
        }

        var entry = target.GetFile("war3map.j") ?? target.GetFile(@"scripts\war3map.j");
        if (entry is null)
        {
            diagnostics.Add("the map has no war3map.j, so its script identifiers could not be checked.");
            return Array.Empty<UndeclaredScriptIdentifier>();
        }

        var source = ScriptCommand.ScriptEncoding.GetString(entry.CurrentBytes);
        return TargetCompatCommand.UsedIdentifiers(source)
            .Where(u => !known.Contains(u.Name))
            .Select(u => new UndeclaredScriptIdentifier(u.Name, u.FirstLine, u.Count))
            .OrderBy(u => u.FirstLine).ThenBy(u => u.Name, StringComparer.Ordinal)
            .ToList();
    }
}
