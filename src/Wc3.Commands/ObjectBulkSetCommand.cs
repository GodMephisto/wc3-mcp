// src/Wc3.Commands/ObjectBulkSetCommand.cs
using War3Net.Common.Extensions;
using Wc3.GameData;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One where clause, a field code and the value an object must carry to match.</summary>
public sealed record BulkSetWhere(string Code, string Value);

/// <summary>The rawcode of an object the bulk set matched, or failed to write, and why.</summary>
public sealed record BulkSetEntry(string Rawcode, bool Changed, string Detail);

/// <summary>
/// Outcome of a bulk set. Matched counts objects passing every where clause, Changed counts
/// the writes that succeeded, and Skipped counts matches the write refused (a field the
/// object's declared type could not accept). Entries records each match so a refusal is not
/// silent.
/// </summary>
public sealed record ObjectBulkSetResult(
    bool Ok, string Message, string Kind, string Field, string Value,
    int Matched, int Changed, int Skipped, IReadOnlyList<BulkSetEntry> Entries,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Sets one field on every object of a kind that matches every where clause. The clauses test
/// the object's merged value, base game data overlaid by the map's own deltas, with numbers
/// compared numerically so "420" matches "420.0". Each matching object is written through the
/// same path as object set, so it lands in the layer the map keeps the field in. Read-only on
/// its input, the caller saves the edited copy.
/// </summary>
public static class ObjectBulkSetCommand
{
    public static ObjectBulkSetResult Execute(
        MapDocument doc, ObjectKind kind, string field, string value,
        IReadOnlyList<BulkSetWhere> where, string? gameDirOverride)
    {
        if (GameData.GameData.TryOpen(gameDirOverride, out var ctx, out var diagnostic))
            return Execute(doc, kind, field, value, where, ctx, Array.Empty<string>());
        return Execute(doc, kind, field, value, where, null, new[] { diagnostic });
    }

    internal static ObjectBulkSetResult Execute(
        MapDocument doc, ObjectKind kind, string field, string value,
        IReadOnlyList<BulkSetWhere> where, GameDataContext? ctx, IReadOnlyList<string> diagnostics)
    {
        var info = ObjectKinds.Info(kind);
        string kindName = kind.ToString().ToLowerInvariant();
        var entries = ObjectKinds.MergedEntries(doc, info);

        var results = new List<BulkSetEntry>();
        int matched = 0, changed = 0, skipped = 0;

        foreach (var entry in entries)
        {
            string rawcode = entry.Id.ToRawcode();
            if (where.Count > 0 && !MatchesAll(ctx, kind, entry, where)) continue;
            matched++;

            var set = ObjectSetCommand.Execute(doc, kind, rawcode, field, value);
            if (set.Ok)
            {
                changed++;
                results.Add(new BulkSetEntry(rawcode, true, set.Message));
            }
            else
            {
                skipped++;
                results.Add(new BulkSetEntry(rawcode, false, set.Message));
            }
        }

        var notes = new List<string>(diagnostics);
        if (ctx is null && where.Count > 0)
            notes.Add("no game install, the where clauses tested the map's own deltas only");

        return new ObjectBulkSetResult(
            Ok: true,
            Message: $"set {field}={value} on {changed} of {matched} matching {kindName} object(s)",
            Kind: kindName, Field: field, Value: value,
            Matched: matched, Changed: changed, Skipped: skipped, Entries: results, Diagnostics: notes);
    }

    /// <summary>Every where clause equal to the object's merged value of the same bare code.</summary>
    private static bool MatchesAll(
        GameDataContext? ctx, ObjectKind kind, MapObjectEntry entry, IReadOnlyList<BulkSetWhere> where)
    {
        var merged = MergedBare(ctx, kind, entry);
        foreach (var clause in where)
        {
            if (!merged.TryGetValue(BareCode(clause.Code), out var actual)) return false;
            if (!ObjectNoopFieldsCommand.ValuesEqual(actual, clause.Value)) return false;
        }
        return true;
    }

    /// <summary>Base fields overlaid by the map's deltas, keyed by bare field code, last wins.</summary>
    private static Dictionary<string, string> MergedBare(
        GameDataContext? ctx, ObjectKind kind, MapObjectEntry entry)
    {
        var merged = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (ctx is not null && entry.OldId != 0
            && ObjectKinds.TryGetBaseFields(ctx, kind, entry.OldId.ToRawcode(), out var baseFields))
            foreach (var (code, v) in baseFields) merged[BareCode(code)] = v;
        foreach (var (code, v) in entry.Mods) merged[BareCode(code)] = v;
        return merged;
    }

    private static string BareCode(string code)
    {
        int colon = code.IndexOf(':');
        return colon < 0 ? code : code[..colon];
    }
}
