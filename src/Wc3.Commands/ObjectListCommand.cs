using War3Net.Common.Extensions;
using Wc3.GameData;
using Wc3.Model;

namespace Wc3.Commands;

public static class ObjectListCommand
{
    /// <summary>Backward-compatible unit-only overload (CLI default, Studio).</summary>
    public static ObjectListResult Execute(MapDocument doc, string? gameDirOverride) =>
        Execute(doc, ObjectKind.Unit, gameDirOverride);

    /// <summary>
    /// Enumerate the map's entries of one Object Editor kind from war3map.* overlaid
    /// by the Reforged war3mapSkin.* twin: modified standard objects (Base*, identified
    /// by OldId — their own base) and custom objects (New*, identified by NewId; OldId 0
    /// = base-less). No file → empty list. Names resolve from the map's name-field
    /// delta (TRIGSTR_ refs via war3map.wts), else the base game's name (null when
    /// unavailable); the game data is only opened when the map actually has entries.
    /// </summary>
    public static ObjectListResult Execute(MapDocument doc, ObjectKind kind, string? gameDirOverride)
    {
        var entries = ObjectKinds.MergedEntries(doc, ObjectKinds.Info(kind));
        if (entries.Count == 0) return new ObjectListResult(Array.Empty<ObjectListItem>());

        GameData.GameData.TryOpen(gameDirOverride, out var ctx, out _);
        return Execute(doc, kind, entries, ctx);
    }

    /// <summary>Core with an optional game-data context (null = no name fallback).</summary>
    internal static ObjectListResult Execute(MapDocument doc, ObjectKind kind, GameDataContext? ctx) =>
        Execute(doc, kind, ObjectKinds.MergedEntries(doc, ObjectKinds.Info(kind)), ctx);

    private static ObjectListResult Execute(
        MapDocument doc, ObjectKind kind, IReadOnlyList<MapObjectEntry> entries, GameDataContext? ctx)
    {
        var info = ObjectKinds.Info(kind);
        var strings = MapStrings.From(doc);
        var items = new List<ObjectListItem>(entries.Count);
        foreach (var e in entries)
        {
            string? baseRawcode = e.OldId == 0 ? null : e.OldId.ToRawcode();
            string? name = ObjectKinds.DeltaName(ObjectKinds.ModsToDict(e.Mods), info, strings)
                ?? ObjectKinds.BaseName(ctx, kind, baseRawcode);
            items.Add(new ObjectListItem(e.Id.ToRawcode(), baseRawcode, name));
        }
        return new ObjectListResult(items);
    }
}
