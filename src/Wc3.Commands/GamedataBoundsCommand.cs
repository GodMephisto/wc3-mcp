// src/Wc3.Commands/GamedataBoundsCommand.cs
namespace Wc3.Commands;

/// <summary>One build's definition of a field code, with the bounds the object editor enforces.</summary>
public sealed record GamedataBoundsEntry(
    string Build, string Code, string Slk, string Type,
    string MinValue, string MaxValue, bool CanBeEmpty);

/// <summary>
/// Every build that defines a field code, with its type and bounds. Target names the one build
/// searched, or is null when every build was searched. Entries is empty when no build defines the code.
/// </summary>
public sealed record GamedataBoundsResult(
    string Code, string? Target, IReadOnlyList<GamedataBoundsEntry> Entries);

/// <summary>
/// Reports a field code's type, minimum and maximum bounds, whether it can be empty, and the object
/// metadata file that defines it, in one named build or in every build where it exists. A code may be
/// defined in more than one file, so one build can contribute more than one entry.
/// </summary>
public static class GamedataBoundsCommand
{
    public static GamedataBoundsResult Execute(
        string code, string? target = null, string? schemasDir = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("a field code is required", nameof(code));

        var schemas = target is null
            ? GamedataSupport.LoadAll(schemasDir, GameSchemaCommand.SchemaPart.Objects)
            : new[] { GamedataSupport.Find(schemasDir, target) };

        var entries = new List<GamedataBoundsEntry>();
        foreach (var one in schemas)
        {
            if (one.Schema.Objects is null) continue;
            foreach (var kv in one.Schema.Objects.OrderBy(k => k.Key, StringComparer.Ordinal))
            {
                foreach (var field in kv.Value)
                {
                    if (!string.Equals(field.Code, code, StringComparison.Ordinal)) continue;
                    entries.Add(new GamedataBoundsEntry(
                        one.Build, field.Code, kv.Key, field.Type,
                        field.MinValue, field.MaxValue, field.CanBeEmpty));
                }
            }
        }

        return new GamedataBoundsResult(code, target, entries);
    }
}
