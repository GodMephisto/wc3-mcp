// src/Wc3.Commands/ObjectExportCommand.cs
using War3Net.Common.Extensions;
using Wc3.GameData;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// The CSV for one Object Editor kind: one row per object the map defines (merged base game
/// data overlaid by the map's deltas), one column per field code, a header row first. The
/// columns are the union of every field any exported object carries, ordinally sorted, so a
/// round trip through <see cref="ObjectImportCommand"/> restores the same set.
/// </summary>
public sealed record ObjectExportResult(
    bool Ok, string Message, string Kind, int ObjectCount,
    IReadOnlyList<string> Columns, string Csv, IReadOnlyList<string> Diagnostics);

/// <summary>
/// Exports the objects of one kind to CSV. Read-only, no map bytes change. Base game fields
/// merge in when a game install is available, so a field the map did not touch still exports
/// its base value; without one the export is map-deltas only and says so. A field whose
/// delta equals the base value is exported once, at its merged value.
/// </summary>
public static class ObjectExportCommand
{
    public static ObjectExportResult Execute(MapDocument doc, ObjectKind kind, string? gameDirOverride)
    {
        if (GameData.GameData.TryOpen(gameDirOverride, out var ctx, out var diagnostic))
            return Execute(doc, kind, ctx, Array.Empty<string>());
        return Execute(doc, kind, null, new[] { diagnostic });
    }

    internal static ObjectExportResult Execute(
        MapDocument doc, ObjectKind kind, GameDataContext? ctx, IReadOnlyList<string> diagnostics)
    {
        var info = ObjectKinds.Info(kind);
        string kindName = kind.ToString().ToLowerInvariant();
        var entries = ObjectKinds.MergedEntries(doc, info);

        // Merged fields per object: base game fields overlaid by the map's deltas.
        var rows = entries.Select(e => (Entry: e, Fields: MergedFields(ctx, kind, e))).ToList();

        var columns = rows
            .SelectMany(r => r.Fields.Keys)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(c => c, StringComparer.Ordinal)
            .ToList();

        var lines = new List<string>(rows.Count + 1)
        {
            ObjectCsv.Row(new[] { "rawcode" }.Concat(columns)),
        };
        foreach (var (entry, fields) in rows)
        {
            var cells = new List<string>(columns.Count + 1) { entry.Id.ToRawcode() };
            foreach (var column in columns)
                cells.Add(fields.TryGetValue(column, out var v) ? v : "");
            lines.Add(ObjectCsv.Row(cells));
        }

        string csv = string.Join("\n", lines) + "\n";
        var notes = new List<string>(diagnostics);
        if (ctx is null)
            notes.Add("no game install, the export carries the map's own deltas only");

        return new ObjectExportResult(
            Ok: true,
            Message: $"exported {rows.Count} {kindName} object(s) across {columns.Count} field(s)",
            Kind: kindName,
            ObjectCount: rows.Count,
            Columns: columns,
            Csv: csv,
            Diagnostics: notes);
    }

    /// <summary>Base fields for the object's base rawcode, overlaid by its map/skin deltas
    /// (last wins), mirroring the read side of object_get.</summary>
    private static Dictionary<string, string> MergedFields(
        GameDataContext? ctx, ObjectKind kind, MapObjectEntry entry)
    {
        var merged = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (ctx is not null && entry.OldId != 0)
        {
            string baseRawcode = entry.OldId.ToRawcode();
            if (ObjectKinds.TryGetBaseFields(ctx, kind, baseRawcode, out var baseFields))
                foreach (var (code, value) in baseFields) merged[code] = value;
        }
        foreach (var (code, value) in entry.Mods) merged[code] = value;
        return merged;
    }
}
