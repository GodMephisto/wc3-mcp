// src/Wc3.Commands/ObjectSearchCommand.cs
using Wc3.GameData;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One field value that contains the query. BaseValue and MapValue are null
/// when the field exists only on the other side, so a reader can tell a delta that
/// matches from a base value that was overridden away.</summary>
public sealed record ObjectSearchHit(
    string Kind, string Rawcode, string? Name,
    string FieldCode, string FieldName, string? BaseValue, string? MapValue);

public sealed record ObjectSearchResult(
    string Query, int Count, IReadOnlyList<ObjectSearchHit> Hits, IReadOnlyList<string> Diagnostics);

/// <summary>
/// Full-text search across every custom or modified object's field values, in both the
/// base game data and the map's own deltas. Read-only. A leveled key carries its level
/// suffix so a hit is traceable to the level it came from.
/// </summary>
public static class ObjectSearchCommand
{
    private const string EmptyQueryNote =
        "query is empty, so nothing was searched. Pass text to find in object field values.";

    public static ObjectSearchResult Execute(
        MapDocument doc, string query, ObjectKind? kind, string? field, string? gameDirOverride)
    {
        GameData.GameData.TryOpen(gameDirOverride, out var ctx, out _);
        return Execute(doc, query, kind, field, ctx);
    }

    internal static ObjectSearchResult Execute(
        MapDocument doc, string query, ObjectKind? kind, string? field, GameDataContext? ctx)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new ObjectSearchResult(
                query ?? "", 0, Array.Empty<ObjectSearchHit>(), new[] { EmptyQueryNote });

        var kinds = kind is null
            ? ObjectKinds.All
            : (IReadOnlyList<ObjectKind>)new[] { kind.Value };

        var hits = new List<ObjectSearchHit>();
        foreach (var k in kinds)
        {
            var fieldLookup = ObjectKinds.FieldNameLookup(ctx, k);
            foreach (var obj in ObjectScan.Kind(doc, ctx, k))
            {
                var codes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var c in obj.BaseFields.Keys) codes.Add(c);
                foreach (var c in obj.DeltaFields.Keys) codes.Add(c);

                foreach (var code in codes)
                {
                    if (field is not null && !BareCodeEquals(code, field)) continue;
                    obj.BaseFields.TryGetValue(code, out var baseValue);
                    obj.DeltaFields.TryGetValue(code, out var mapValue);
                    if (!Matches(baseValue, query) && !Matches(mapValue, query)) continue;
                    hits.Add(new ObjectSearchHit(
                        k.ToString().ToLowerInvariant(), obj.Rawcode, obj.Name,
                        code, fieldLookup(code), baseValue, mapValue));
                }
            }
        }

        hits = hits
            .OrderBy(h => h.Kind, StringComparer.Ordinal)
            .ThenBy(h => h.Rawcode, StringComparer.Ordinal)
            .ThenBy(h => h.FieldCode, StringComparer.OrdinalIgnoreCase)
            .ToList();
        return new ObjectSearchResult(query, hits.Count, hits, Array.Empty<string>());
    }

    private static bool Matches(string? value, string query) =>
        value is not null && value.Contains(query, StringComparison.OrdinalIgnoreCase);

    private static bool BareCodeEquals(string code, string field)
    {
        int colon = code.IndexOf(':');
        var bare = colon < 0 ? code : code[..colon];
        return bare.Equals(field, StringComparison.OrdinalIgnoreCase);
    }
}
