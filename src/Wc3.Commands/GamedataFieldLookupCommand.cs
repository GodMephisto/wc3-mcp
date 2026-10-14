// src/Wc3.Commands/GamedataFieldLookupCommand.cs
namespace Wc3.Commands;

/// <summary>One object field whose display name matched, and the build that defines it.</summary>
public sealed record GamedataFieldMatch(
    string Build, string Code, string Slk, string DisplayName, string Type);

/// <summary>
/// Every object field whose display name contains the query. Target names the one build searched, or
/// is null when every build was searched.
/// </summary>
public sealed record GamedataFieldLookupResult(
    string Query, string? Target, IReadOnlyList<GamedataFieldMatch> Matches);

/// <summary>
/// Finds object field codes whose display name contains the query, in one named build or in every
/// build. The match is case-insensitive over the resolved display name, and falls back to the raw
/// display key when a build carries no translated text for the field.
/// </summary>
public static class GamedataFieldLookupCommand
{
    public static GamedataFieldLookupResult Execute(
        string query, string? target = null, string? schemasDir = null)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw new ArgumentException("a query is required", nameof(query));

        var schemas = target is null
            ? GamedataSupport.LoadAll(schemasDir, GameSchemaCommand.SchemaPart.Objects)
            : new[] { GamedataSupport.Find(schemasDir, target) };

        var matches = new List<GamedataFieldMatch>();
        foreach (var one in schemas)
        {
            if (one.Schema.Objects is null) continue;
            foreach (var kv in one.Schema.Objects.OrderBy(k => k.Key, StringComparer.Ordinal))
            {
                foreach (var field in kv.Value)
                {
                    if (!GamedataSupport.DisplayOf(field).Contains(query, StringComparison.OrdinalIgnoreCase))
                        continue;
                    matches.Add(new GamedataFieldMatch(
                        one.Build, field.Code, kv.Key, GamedataSupport.DisplayOf(field), field.Type));
                }
            }
        }

        return new GamedataFieldLookupResult(query, target, matches);
    }
}
