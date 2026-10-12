// src/Wc3.Commands/ObjectDeprecatedCommand.cs
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>A field the map still sets that the target build's schema does not define.
/// The game ignores such a field on that build, so the edit silently does nothing.</summary>
public sealed record DeprecatedField(string Kind, string Rawcode, string? Name, string FieldCode);

public sealed record ObjectDeprecatedResult(
    string TargetBuild, int FieldsChecked, IReadOnlyList<DeprecatedField> Fields,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Lists object fields the target build's schema does not define but the map still
/// sets. Read-only. The target resolves exactly the way the compat and target commands
/// resolve it, a schema folder path or a build name under the schemas directory. A
/// target with no object data section reports that rather than a false clean.
/// </summary>
public static class ObjectDeprecatedCommand
{
    public static ObjectDeprecatedResult Execute(MapDocument doc, string target, string? schemasDir)
    {
        var schema = TargetBuildSettings.LoadSchema(target, schemasDir);
        return Execute(doc, schema);
    }

    internal static ObjectDeprecatedResult Execute(MapDocument doc, GameSchema schema)
    {
        var diagnostics = new List<string>();
        if (schema.Objects is null)
        {
            diagnostics.Add($"target {schema.Build} has no object data section in its schema, "
                + "so no field can be judged deprecated against it.");
            return new ObjectDeprecatedResult(
                schema.Build, 0, Array.Empty<DeprecatedField>(), diagnostics);
        }

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

        var strings = MapStrings.From(doc);
        var deprecated = new List<DeprecatedField>();
        var uncheckedKinds = new List<string>();
        int checkedFields = 0;

        foreach (var kind in ObjectKinds.All)
        {
            var info = ObjectKinds.Info(kind);
            var entries = ObjectKinds.MergedEntries(doc, info);
            if (entries.Count == 0) continue;

            if (!validByKind.TryGetValue(kind, out var valid))
            {
                uncheckedKinds.Add(kind.ToString().ToLowerInvariant());
                continue;
            }

            foreach (var entry in entries)
            {
                string rawcode = entry.Id.ToRawcode();
                string? name = ObjectKinds.DeltaName(
                    ObjectKinds.ModsToDict(entry.Mods), info, strings);
                foreach (var (key, _) in entry.Mods)
                {
                    string bare = key;
                    int colon = bare.IndexOf(':');
                    if (colon > 0) bare = bare[..colon];
                    checkedFields++;
                    if (valid.Contains(bare)) continue;
                    deprecated.Add(new DeprecatedField(
                        kind.ToString().ToLowerInvariant(), rawcode, name, bare));
                }
            }
        }

        if (uncheckedKinds.Count > 0)
            diagnostics.Add("no object schema section for " + string.Join(", ", uncheckedKinds)
                + ", so their fields were not checked.");

        deprecated = deprecated
            .OrderBy(f => f.Kind, StringComparer.Ordinal)
            .ThenBy(f => f.Rawcode, StringComparer.Ordinal)
            .ThenBy(f => f.FieldCode, StringComparer.OrdinalIgnoreCase)
            .ToList();
        return new ObjectDeprecatedResult(schema.Build, checkedFields, deprecated, diagnostics);
    }
}
