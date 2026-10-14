// src/Wc3.Commands/GamedataEnumCommand.cs
namespace Wc3.Commands;

/// <summary>One legal value of an enumerated field type, with the name the editor shows.</summary>
public sealed record GamedataEnumValue(string Value, string? DisplayName);

/// <summary>One build's option set for an enumerated field type.</summary>
public sealed record GamedataEnumEntry(string Build, IReadOnlyList<GamedataEnumValue> Values);

/// <summary>
/// Every build that defines an enumerated field type, with the closed set of values it may take.
/// Target names the one build searched, or is null when every build was searched. Entries is empty
/// when no build defines the type.
/// </summary>
public sealed record GamedataEnumResult(
    string Type, string? Target, IReadOnlyList<GamedataEnumEntry> Entries);

/// <summary>
/// The closed set of legal values for an enumerated object field type, read from each build's
/// <c>UI\UnitEditorData.txt</c>, in one named build or in every build that defines the type.
///
/// This is the authority for what a dropdown may offer and the only place the value names live, so
/// a front end can build a picker from the schema alone rather than from a live install. The type
/// is the metadata SLK <c>type</c> column, so a field code resolves to a set through
/// <c>gamedata bounds</c> and then here.
/// </summary>
public static class GamedataEnumCommand
{
    public static GamedataEnumResult Execute(
        string type, string? target = null, string? schemasDir = null)
    {
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("a field type is required", nameof(type));

        var schemas = target is null
            ? GamedataSupport.LoadAll(schemasDir, GameSchemaCommand.SchemaPart.Enums)
            : new[] { GamedataSupport.Find(schemasDir, target) };

        var entries = new List<GamedataEnumEntry>();
        foreach (var one in schemas)
        {
            if (one.Schema.Enums is null) continue;
            if (!one.Schema.Enums.TryGetValue(type, out var values)) continue;
            entries.Add(new GamedataEnumEntry(one.Build,
                values.Select(v => new GamedataEnumValue(v.Value, v.DisplayName)).ToList()));
        }

        return new GamedataEnumResult(type, target, entries);
    }

    /// <summary>Every enumerated field type at least one searched build defines, for discovery.</summary>
    public static IReadOnlyList<string> Types(string? target = null, string? schemasDir = null)
    {
        var schemas = target is null
            ? GamedataSupport.LoadAll(schemasDir, GameSchemaCommand.SchemaPart.Enums)
            : new[] { GamedataSupport.Find(schemasDir, target) };

        var set = new SortedSet<string>(StringComparer.Ordinal);
        foreach (var one in schemas)
            if (one.Schema.Enums is not null)
                foreach (var key in one.Schema.Enums.Keys) set.Add(key);
        return set.ToList();
    }
}
