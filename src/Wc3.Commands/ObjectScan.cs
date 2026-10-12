// src/Wc3.Commands/ObjectScan.cs
using War3Net.Common.Extensions;
using Wc3.GameData;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One map-defined object normalized for read-only scans. Carries the kind,
/// the object's rawcode and base, its resolved name, the base game fields (empty when
/// no game data is available) and the map's own field deltas with the Reforged skin
/// layer already folded in (skin wins per field).</summary>
internal sealed record ScannedObject(
    ObjectKind Kind, string Rawcode, string? BaseRawcode, string? Name,
    IReadOnlyDictionary<string, string> BaseFields,
    IReadOnlyDictionary<string, string> DeltaFields);

/// <summary>
/// Walks a map's Object Editor data one kind at a time, resolving each object's base
/// fields from the game data when a context is supplied and the map's own deltas
/// always. Shared by the read-only object commands (search, noop-fields, deprecated)
/// so they read the object model exactly the way <see cref="ObjectGetCommand"/> does.
/// </summary>
internal static class ObjectScan
{
    private static readonly IReadOnlyDictionary<string, string> None =
        new Dictionary<string, string>();

    internal static IEnumerable<ScannedObject> Kind(
        MapDocument doc, GameDataContext? ctx, ObjectKind kind)
    {
        var info = ObjectKinds.Info(kind);
        var strings = MapStrings.From(doc);
        foreach (var entry in ObjectKinds.MergedEntries(doc, info))
        {
            string rawcode = entry.Id.ToRawcode();
            string? baseRawcode = entry.OldId == 0 ? null : entry.OldId.ToRawcode();
            var delta = ObjectKinds.ModsToDict(entry.Mods);

            IReadOnlyDictionary<string, string> baseFields = None;
            if (baseRawcode is not null && ctx is not null
                && ObjectKinds.TryGetBaseFields(ctx, kind, baseRawcode, out var resolved))
                baseFields = resolved;

            string? name = ObjectKinds.DeltaName(delta, info, strings)
                ?? ObjectKinds.BaseName(ctx, kind, baseRawcode);

            yield return new ScannedObject(kind, rawcode, baseRawcode, name, baseFields, delta);
        }
    }

    internal static IEnumerable<ScannedObject> All(MapDocument doc, GameDataContext? ctx)
    {
        foreach (var kind in ObjectKinds.All)
            foreach (var obj in Kind(doc, ctx, kind))
                yield return obj;
    }
}
