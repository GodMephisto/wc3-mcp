// src/Wc3.Commands/PlacedStatsCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One placed type's tally, its kind (unit or doodad), rawcode, resolved name
/// when one is available, and how many copies sit on the map.</summary>
public sealed record PlacedTypeCount(string Kind, string Rawcode, string? Name, int Count);

/// <summary>Placed-widget census with totals per kind and one row per distinct type.</summary>
public sealed record PlacedStatsResult(
    bool Ok, string Message, int UnitTotal, int DoodadTotal, IReadOnlyList<PlacedTypeCount> Types);

/// <summary>
/// Counts the units and doodads already on a map, grouped by rawcode and by kind. Names
/// resolve from the map's own object data (including the sloc pseudo-type), and from the
/// base game data when a game dir is supplied. Read-only.
/// </summary>
public static class PlacedStatsCommand
{
    public static PlacedStatsResult Execute(MapDocument doc, string? gameDir = null)
    {
        ArgumentNullException.ThrowIfNull(doc);

        var refs = PlacementRefs.All(doc);
        var names = BuildNames(doc, refs, gameDir);

        var types = refs
            .GroupBy(r => (r.Kind, r.Rawcode))
            .Select(g => new PlacedTypeCount(
                g.Key.Kind, g.Key.Rawcode,
                names.TryGetValue((g.Key.Kind, g.Key.Rawcode), out var n) ? n : null,
                g.Count()))
            .OrderBy(t => t.Kind, StringComparer.Ordinal)
            .ThenBy(t => t.Rawcode, StringComparer.Ordinal)
            .ToList();

        int unitTotal = refs.Count(r => r.Kind == PlacementRefs.UnitKind);
        int doodadTotal = refs.Count(r => r.Kind == PlacementRefs.DoodadKind);

        string message = refs.Count == 0
            ? "no placements on this map"
            : $"{unitTotal} unit(s) of {types.Count(t => t.Kind == PlacementRefs.UnitKind)} type(s), "
              + $"{doodadTotal} doodad(s) of {types.Count(t => t.Kind == PlacementRefs.DoodadKind)} type(s)";

        return new(true, message, unitTotal, doodadTotal, types);
    }

    /// <summary>TypeId to display name. The map's own name deltas come first (via the
    /// instance commands, so the sloc pseudo-type reads as "Start Location"). A type the
    /// map does not name falls back to the base game's name only when a game dir is given.</summary>
    private static Dictionary<(string Kind, string Rawcode), string> BuildNames(
        MapDocument doc, IReadOnlyList<PlacedRef> refs, string? gameDir)
    {
        var mapNames = new Dictionary<(string, string), string>();
        foreach (var u in UnitInstanceCommand.List(doc))
            if (!string.IsNullOrEmpty(u.Name))
                mapNames[(PlacementRefs.UnitKind, u.TypeRawcode)] = u.Name!;
        foreach (var d in DoodadInstanceCommand.List(doc))
            if (!string.IsNullOrEmpty(d.Name))
                mapNames[(PlacementRefs.DoodadKind, d.TypeRawcode)] = d.Name!;

        var names = new Dictionary<(string, string), string>();
        foreach (var (kind, rawcode) in refs.Select(r => (r.Kind, r.Rawcode)).Distinct())
        {
            if (mapNames.TryGetValue((kind, rawcode), out var map))
            {
                names[(kind, rawcode)] = map;
                continue;
            }
            if (gameDir is { Length: > 0 })
            {
                var merged = ObjectGetCommand.Execute(doc, rawcode, gameDir);
                if (!string.IsNullOrEmpty(merged.Name))
                    names[(kind, rawcode)] = merged.Name!;
            }
        }
        return names;
    }
}
