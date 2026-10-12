// src/Wc3.Commands/ObjectNoopFieldsCommand.cs
using System.Globalization;
using Wc3.GameData;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>A map delta that sets a field to the value the base game already has, so
/// the modification changes nothing in game. This is cleanup, not a defect.</summary>
public sealed record NoopField(
    string Kind, string Rawcode, string? Name, string FieldCode, string FieldName, string Value);

public sealed record ObjectNoopFieldsResult(
    int Count, IReadOnlyList<NoopField> Fields, IReadOnlyList<string> Diagnostics);

/// <summary>
/// Finds object fields whose map value equals the base game value, meaning the edit
/// changes nothing. Read-only. Needs a game directory for the base values and degrades
/// to a map-only answer with a diagnostic when none is available, because without the
/// base there is nothing to compare a delta against.
/// </summary>
public static class ObjectNoopFieldsCommand
{
    private const string NoBaseDataNote =
        "no game data, so there is no base value to compare against. Point --game-dir at a "
        + "Warcraft III install to find fields whose map value equals the base game's.";

    public static ObjectNoopFieldsResult Execute(
        MapDocument doc, ObjectKind? kind, string? gameDirOverride)
    {
        bool opened = GameData.GameData.TryOpen(gameDirOverride, out var ctx, out var diagnostic);
        var diagnostics = new List<string>();
        if (!opened || ctx is null)
        {
            diagnostics.Add(NoBaseDataNote);
            if (!string.IsNullOrWhiteSpace(diagnostic)) diagnostics.Add(diagnostic);
            ctx = null;
        }
        return Execute(doc, kind, ctx, diagnostics);
    }

    internal static ObjectNoopFieldsResult Execute(
        MapDocument doc, ObjectKind? kind, GameDataContext? ctx, IReadOnlyList<string> diagnostics)
    {
        var kinds = kind is null
            ? ObjectKinds.All
            : (IReadOnlyList<ObjectKind>)new[] { kind.Value };

        var fields = new List<NoopField>();
        foreach (var k in kinds)
        {
            var lookup = ObjectKinds.FieldNameLookup(ctx, k);
            foreach (var obj in ObjectScan.Kind(doc, ctx, k))
            {
                foreach (var (code, mapValue) in obj.DeltaFields)
                {
                    if (!obj.BaseFields.TryGetValue(code, out var baseValue)) continue;
                    if (!ValuesEqual(baseValue, mapValue)) continue;
                    fields.Add(new NoopField(
                        k.ToString().ToLowerInvariant(), obj.Rawcode, obj.Name,
                        code, lookup(code), mapValue));
                }
            }
        }

        fields = fields
            .OrderBy(f => f.Kind, StringComparer.Ordinal)
            .ThenBy(f => f.Rawcode, StringComparer.Ordinal)
            .ThenBy(f => f.FieldCode, StringComparer.OrdinalIgnoreCase)
            .ToList();
        return new ObjectNoopFieldsResult(fields.Count, fields, diagnostics);
    }

    /// <summary>
    /// Whether a map value is the same as the base value. Numbers compare numerically so
    /// 420 and 420.0 match, since the two sides are formatted by different code paths.
    /// Everything else compares trimmed and case-insensitively.
    /// </summary>
    internal static bool ValuesEqual(string baseValue, string mapValue)
    {
        if (double.TryParse(baseValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var b)
            && double.TryParse(mapValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var m))
            return b == m;
        return string.Equals(baseValue.Trim(), mapValue.Trim(), StringComparison.OrdinalIgnoreCase);
    }
}
