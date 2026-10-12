// src/Wc3.Commands/PlacementRefs.cs
using War3Net.Build.Widget;
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// One placed widget (a unit, item, start location or doodad) reduced to what the
/// reference-analysis commands need. Which file holds it, which family it belongs to,
/// its creation number, its rawcode and where it sits. Kind is the placement file's
/// own family, "unit" for war3mapUnits.doo and "doodad" for war3map.doo, matching the
/// names the placed_* MCP tools and the unit/doodad CLI groups already use.
/// </summary>
public sealed record PlacedRef(
    string File, string Kind, int CreationNumber, string Rawcode, float X, float Y);

/// <summary>
/// Shared reference analysis over a map's placements, so placed_stats,
/// placed missing-refs and placed fix-refs classify and resolve a placement the same
/// way. A rawcode is "defined" when the map's own object data (any kind) defines it, or,
/// when a game dir resolves, the base game data does. A start location marker (sloc) is
/// a widget the game itself defines, so it always counts as defined.
/// </summary>
public static class PlacementRefs
{
    public const string UnitKind = "unit";
    public const string DoodadKind = "doodad";

    /// <summary>Every placement, war3mapUnits.doo first, each file in its own order.</summary>
    public static IReadOnlyList<PlacedRef> All(MapDocument doc)
    {
        var list = new List<PlacedRef>();
        if (doc.GetFile(PlacementCommand.UnitsFile)?.Model is MapUnits units)
            foreach (var u in units.Units)
                list.Add(new PlacedRef(PlacementCommand.UnitsFile, UnitKind, u.CreationNumber,
                    u.TypeId.ToRawcode(), u.Position.X, u.Position.Y));
        if (doc.GetFile(PlacementCommand.DoodadsFile)?.Model is MapDoodads doodads)
            foreach (var d in doodads.Doodads)
                list.Add(new PlacedRef(PlacementCommand.DoodadsFile, DoodadKind, d.CreationNumber,
                    d.TypeId.ToRawcode(), d.Position.X, d.Position.Y));
        return list;
    }

    /// <summary>
    /// Whether a rawcode is defined somewhere the game can resolve it, the map's own
    /// object data (probed across every kind) or the base game data when a game dir
    /// resolves. Absent game data the base check is skipped, so a rawcode only the base
    /// game defines reads as undefined and the caller has to say so.
    /// </summary>
    public static bool IsDefined(MapDocument doc, string rawcode, string? gameDir)
    {
        if (string.Equals(rawcode, PlacementCommand.StartLocationRawcode, StringComparison.Ordinal))
            return true;
        if (rawcode.Length != 4)
            return false;
        return ObjectGetCommand.Execute(doc, rawcode, gameDir).Found;
    }
}
