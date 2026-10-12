// src/Wc3.Commands/ObjectImportCommand.cs
using System.Globalization;
using War3Net.Common.Extensions;
using Wc3.GameData;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Outcome of importing one kind's CSV. Added counts objects the map did not define, Updated
/// counts objects it already did, and FieldsSet counts individual field writes. A header with
/// a column the kind does not define refuses the whole import before anything is written.
/// </summary>
public sealed record ObjectImportResult(
    bool Ok, string Message, string Kind, int Added, int Updated, int FieldsSet,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Imports a CSV of one Object Editor kind back into a map, adding objects the map does not
/// yet define and updating those it does. The table is validated first, and an unknown column
/// refuses the import rather than dropping data. A blank cell leaves the field untouched, so
/// an export/import round trip restores exactly the fields the export carried. When a game
/// install is available, a column must be a field the kind's metadata declares; without one a
/// syntactically valid field token is accepted and the import says it could not verify codes.
/// </summary>
public static class ObjectImportCommand
{
    public static ObjectImportResult Execute(
        MapDocument doc, ObjectKind kind, string csv, string? gameDirOverride)
    {
        if (GameData.GameData.TryOpen(gameDirOverride, out var ctx, out var diagnostic))
            return Execute(doc, kind, csv, ctx, Array.Empty<string>());
        return Execute(doc, kind, csv, null, new[] { diagnostic });
    }

    internal static ObjectImportResult Execute(
        MapDocument doc, ObjectKind kind, string csv,
        GameDataContext? ctx, IReadOnlyList<string> diagnostics)
    {
        var info = ObjectKinds.Info(kind);
        string kindName = kind.ToString().ToLowerInvariant();
        var notes = new List<string>(diagnostics);

        var rows = ObjectCsv.Parse(csv);
        if (rows.Count == 0)
            return Refuse(kindName, "the CSV is empty", notes);

        var header = rows[0];
        if (header.Count == 0 || !header[0].Equals("rawcode", StringComparison.OrdinalIgnoreCase))
            return Refuse(kindName, "the first CSV column must be 'rawcode'", notes);

        var columns = header.Skip(1).ToList();
        if (columns.Count == 0)
            return Refuse(kindName, "the CSV has a rawcode column but no field columns", notes);

        // Validate every column before touching the map, so an unknown column changes nothing.
        var known = new List<string?>(columns.Count);
        foreach (var column in columns)
        {
            if (!IsFieldToken(column))
                return Refuse(kindName, $"column '{column}' is not a valid field code", notes);
            if (ctx is not null && ObjectKinds.FieldMeta(ctx, kind) is { } meta
                && !meta.TryGet(BareCode(column), out _))
                return Refuse(kindName,
                    $"column '{column}' is not a field the {kindName} kind defines", notes);
            known.Add(column);
        }
        if (ctx is null)
            notes.Add("no game install, field codes were checked for shape only");

        int added = 0, updated = 0, fields = 0;
        for (int r = 1; r < rows.Count; r++)
        {
            var row = rows[r];
            if (row.Count == 1 && row[0].Length == 0) continue; // blank line

            string rawcode = row[0].Trim();
            if (rawcode.Length != 4)
            {
                notes.Add($"row {r + 1}: '{rawcode}' is not a four-character rawcode, skipped");
                continue;
            }

            bool exists = ObjectKinds.MergedEntries(doc, info)
                .Any(e => e.Id == rawcode.FromRawcode());
            if (!exists)
            {
                if (!CreateObject(doc, kind, ctx, rawcode.FromRawcode(), out var error))
                {
                    notes.Add($"row {r + 1}: {error}");
                    continue;
                }
                added++;
            }
            else
            {
                updated++;
            }

            for (int c = 0; c < known.Count; c++)
            {
                int cell = c + 1;
                if (cell >= row.Count) break;
                string value = row[cell];
                if (value.Length == 0) continue; // blank leaves the field untouched
                var set = ObjectSetCommand.Execute(doc, kind, rawcode, known[c]!, value);
                if (set.Ok) fields++;
                else notes.Add($"row {r + 1} {rawcode} {known[c]}: {set.Message}");
            }
        }

        return new ObjectImportResult(
            Ok: true,
            Message: $"imported {kindName} CSV, {added} added, {updated} updated, {fields} field(s) set",
            Kind: kindName, Added: added, Updated: updated, FieldsSet: fields, Diagnostics: notes);
    }

    private static ObjectImportResult Refuse(string kindName, string message, List<string> notes) =>
        new(false, message, kindName, 0, 0, 0, notes);

    /// <summary>
    /// The object must exist before ObjectSetCommand can write its fields. A rawcode the base
    /// game data defines becomes a modified standard (OldId = rawcode), anything else a custom
    /// with no base, both in the kind's war3map.* file exactly as object_new does for a custom.
    /// </summary>
    private static bool CreateObject(
        MapDocument doc, ObjectKind kind, GameDataContext? ctx, int id, out string error)
    {
        var info = ObjectKinds.Info(kind);
        var model = ObjectDataWriter.GetOrCreateModel(doc, kind, ObjectLayer.Map);
        var access = model is null ? null : ObjectDataWriter.AccessFor(model);
        if (access is null)
        {
            error = $"map has no parseable {info.MapFile} for a new {kind} object";
            return false;
        }

        string rawcode = id.ToRawcode();
        bool standard = ctx is not null && ObjectKinds.TryGetBaseFields(ctx, kind, rawcode, out _);
        if (standard) access.CreateGroup(id, 0);   // modified standard
        else access.CreateGroup(0, id);            // custom with no base
        doc.AddOrReplaceModelFile(info.MapFile, model!);
        error = "";
        return true;
    }

    /// <summary>A bare four-character alphanumeric code, optionally ':N' for a level/variation.</summary>
    private static bool IsFieldToken(string token)
    {
        string code = BareCode(token);
        if (code.Length != 4 || !code.All(char.IsLetterOrDigit)) return false;
        int colon = token.IndexOf(':');
        return colon < 0
            || int.TryParse(token[(colon + 1)..], NumberStyles.None, CultureInfo.InvariantCulture, out _);
    }

    private static string BareCode(string code)
    {
        int colon = code.IndexOf(':');
        return colon < 0 ? code : code[..colon];
    }
}
