// src/Wc3.Commands/PlacementCommand.cs
using Color = System.Drawing.Color; // alias only Color — avoids Region clash with War3Net.Build.Environment.Region
using System.Numerics;
using War3Net.Build.Common;      // RandomItemSet lives here (not in .Widget)
using War3Net.Build.Environment; // MapRegions, Region, WeatherType, MapRegionsFormatVersion
using War3Net.Build.Widget;
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Programmatic unit placement onto a map's war3mapUnits.doo. Built primarily so
/// automated tests can seed a map with known units and assert the load/edit/save
/// round-trip, but it is a general placement API. The war3mapUnits.doo file is
/// created (as a modern v8/subversion-11 widget file) when the map has none — a
/// blank map has no placement file at all.
/// </summary>
public static class PlacementCommand
{
    public const string UnitsFile = "war3mapUnits.doo";
    public const string DoodadsFile = "war3map.doo";
    public const string RegionsFile = "war3map.w3r";

    // Modern (1.32+/Reforged) widget-file version. War3Net writes the extended
    // fields (SkinId, etc.) when UseNewFormat is set, which subversion 11 implies.
    private const MapWidgetsFormatVersion DefaultFormat = MapWidgetsFormatVersion.v8;
    private const MapWidgetsSubVersion DefaultSubVersion = MapWidgetsSubVersion.v11;

    public sealed record PlaceUnitResult(bool Ok, string Message, int CreationNumber = -1);

    /// <summary>
    /// Places a unit of type <paramref name="typeRawcode"/> owned by
    /// <paramref name="ownerId"/> at map coordinates (<paramref name="x"/>,
    /// <paramref name="y"/>). Returns the assigned CreationNumber (the id triggers
    /// use to reference the unit); it is unique within the file (max existing + 1).
    /// </summary>
    public static PlaceUnitResult PlaceUnit(
        MapDocument doc,
        string typeRawcode,
        int ownerId,
        float x,
        float y,
        float z = 0f,
        float rotation = 0f,
        float scale = 1f)
    {
        if (typeRawcode is null || typeRawcode.Length != 4)
            return new(false, $"invalid unit rawcode '{typeRawcode}' — expected 4 characters");
        if (ownerId < 0)
            return new(false, $"invalid owner id {ownerId} — must be >= 0");

        var units = GetOrCreateUnits(doc);
        int creationNumber = NextCreationNumber(units);
        units.Units.Add(NewUnitData(typeRawcode.FromRawcode(), ownerId, x, y, z, rotation, scale, creationNumber));

        CommitUnits(doc, units);
        return new(true, $"placed {typeRawcode} (owner {ownerId}) at ({x}, {y}) as unit #{creationNumber}", creationNumber);
    }

    /// <summary>The owner id the World Editor writes for a preplaced item in
    /// war3mapUnits.doo. Items have no real owning player, so they occupy the slot just
    /// past neutral passive (15). Kept as a named constant so item placement matches what
    /// the editor produces rather than scattering a magic 16.</summary>
    public const int ItemOwnerId = 16;

    public sealed record PlaceItemResult(bool Ok, string Message, int CreationNumber = -1);

    /// <summary>
    /// Places a preplaced item of type <paramref name="itemRawcode"/> at map coordinates
    /// (<paramref name="x"/>, <paramref name="y"/>). Items share war3mapUnits.doo with units
    /// (the rawcode's presence in the item catalog is what makes WC3 spawn it as a ground
    /// item), so this mirrors <see cref="PlaceUnit"/> exactly but pins the owner to
    /// <see cref="ItemOwnerId"/>. Returns the assigned CreationNumber.
    /// </summary>
    public static PlaceItemResult PlaceItem(
        MapDocument doc,
        string itemRawcode,
        float x,
        float y,
        float z = 0f,
        float rotation = 0f,
        float scale = 1f)
    {
        if (itemRawcode is null || itemRawcode.Length != 4)
            return new(false, $"invalid item rawcode '{itemRawcode}' — expected 4 characters");

        var units = GetOrCreateUnits(doc);
        int creationNumber = NextCreationNumber(units);
        units.Units.Add(NewUnitData(itemRawcode.FromRawcode(), ItemOwnerId, x, y, z, rotation, scale, creationNumber));

        CommitUnits(doc, units);
        return new(true, $"placed item {itemRawcode} at ({x}, {y}) as widget #{creationNumber}", creationNumber);
    }

    /// <summary>Builds a war3mapUnits.doo entry with the World-Editor defaults, shared by
    /// unit and item placement so the default set (skin, hero fields, -1 sentinels, the
    /// lists War3Net's writer dereferences unconditionally) lives in exactly one place.</summary>
    private static UnitData NewUnitData(
        int typeId, int ownerId, float x, float y, float z, float rotation, float scale, int creationNumber) =>
        new()
        {
            TypeId = typeId,
            OwnerId = ownerId,
            Flags = 2,
            Position = new Vector3(x, y, z),
            Rotation = rotation,
            Scale = new Vector3(scale, scale, scale),
            // -1 means "use the object's default" for these; matches the World Editor.
            HP = -1,
            MP = -1,
            GoldAmount = 0,
            TargetAcquisition = -1f,
            HeroLevel = 1,
            HeroStrength = 0,
            HeroAgility = 0,
            HeroIntelligence = 0,
            CustomPlayerColorId = -1,
            WaygateDestinationRegionId = -1,
            SkinId = typeId,      // no custom Reforged skin -> skin id equals the type id
            Variation = 0,
            MapItemTableId = -1,
            CreationNumber = creationNumber,
            // War3Net's writer dereferences these lists unconditionally.
            InventoryData = new List<InventoryItemData>(),
            AbilityData = new List<ModifiedAbilityData>(),
            ItemTableSets = new List<RandomItemSet>(),
        };

    /// <summary>The 4-character type id the World Editor uses for a start location inside
    /// war3mapUnits.doo. A start location is stored as a preplaced "unit" of this type owned
    /// by the player it belongs to; the unit's (x, y) is that player's spawn point.</summary>
    public const string StartLocationRawcode = "sloc";

    /// <summary>
    /// Places (or moves) player <paramref name="player"/>'s start location at map coordinates
    /// (<paramref name="x"/>, <paramref name="y"/>). The World Editor permits exactly one start
    /// location per player, so if this player already has one it is moved rather than duplicated
    /// (preserving its CreationNumber). Otherwise a fresh <c>sloc</c> unit is added. Returns the
    /// start location's CreationNumber.
    /// </summary>
    public static PlaceUnitResult PlaceStartLocation(MapDocument doc, int player, float x, float y)
    {
        if (player < 0)
            return new(false, $"invalid player {player} — must be >= 0");

        var units = GetOrCreateUnits(doc);
        int slocType = StartLocationRawcode.FromRawcode();

        // One start location per player: move the existing one in place if present so its
        // CreationNumber (and any trigger references keyed off it) survive the edit.
        var existing = units.Units.FirstOrDefault(u => u.TypeId == slocType && u.OwnerId == player);
        if (existing is not null)
        {
            existing.Position = new Vector3(x, y, existing.Position.Z);
            CommitUnits(doc, units);
            return new(true,
                $"moved player {player} start location to ({x}, {y}) (unit #{existing.CreationNumber})",
                existing.CreationNumber);
        }

        // None yet: delegate to PlaceUnit so the UnitData defaults (skin, hero fields, inventory
        // lists) live in exactly one place.
        var result = PlaceUnit(doc, StartLocationRawcode, player, x, y);
        return result.Ok
            ? new(true,
                $"placed player {player} start location at ({x}, {y}) (unit #{result.CreationNumber})",
                result.CreationNumber)
            : result;
    }

    /// <summary>Returns the map's parsed MapUnits, creating an empty modern one if absent.</summary>
    internal static MapUnits GetOrCreateUnits(MapDocument doc)
    {
        if (doc.GetFile(UnitsFile)?.Model is MapUnits existing)
            return existing;
        return new MapUnits(DefaultFormat, DefaultSubVersion, useNewFormat: true);
    }

    /// <summary>
    /// Persists a war3mapUnits.doo change and regenerates the runtime creation script so the
    /// preplaced widgets actually spawn in game. Every unit and item write funnels through here
    /// so the .doo and its generated <c>CreateAllUnits</c>/<c>CreateAllItems</c> never drift
    /// apart. See <see cref="PreplacedUnitsScript"/> for why the script (not the .doo) is what
    /// spawns widgets in a map that ships a custom war3map.j.
    /// </summary>
    internal static void CommitUnits(MapDocument doc, MapUnits units)
    {
        doc.AddOrReplaceModelFile(UnitsFile, units);
        PreplacedUnitsScript.Sync(doc);
    }

    private static int NextCreationNumber(MapUnits units) =>
        units.Units.Count == 0 ? 0 : units.Units.Max(u => u.CreationNumber) + 1;

    // ---- Doodads (war3map.doo / MapDoodads) --------------------------------

    public sealed record PlaceDoodadResult(bool Ok, string Message, int CreationNumber = -1);

    /// <summary>
    /// Places a doodad of <paramref name="typeRawcode"/> at (x, y[, z]) and returns
    /// its creation number. Mirrors <see cref="PlaceUnit"/>; writes war3map.doo.
    /// </summary>
    public static PlaceDoodadResult PlaceDoodad(
        MapDocument doc,
        string typeRawcode,
        float x,
        float y,
        float z = 0f,
        float rotation = 0f,
        float scale = 1f,
        int variation = 0)
    {
        if (typeRawcode is null || typeRawcode.Length != 4)
            return new(false, $"invalid doodad rawcode '{typeRawcode}' — expected 4 characters");
        if (variation < 0)
            return new(false, $"invalid variation {variation} — must be >= 0");

        var doodads = GetOrCreateDoodads(doc);

        int typeId = typeRawcode.FromRawcode();
        int creationNumber = NextDoodadCreationNumber(doodads);

        doodads.Doodads.Add(new DoodadData
        {
            TypeId = typeId,
            Variation = variation,
            Position = new Vector3(x, y, z),
            Rotation = rotation,
            Scale = new Vector3(scale, scale, scale),
            // Normal = visible + solid (flag byte 2), the World Editor default.
            State = DoodadState.Normal,
            Life = 100,             // 100% health, matches the World Editor
            SkinId = typeId,        // no custom Reforged skin -> skin id equals the type id
            MapItemTableId = -1,
            CreationNumber = creationNumber,
            // War3Net's writer dereferences this list unconditionally.
            ItemTableSets = new List<RandomItemSet>(),
        });

        doc.AddOrReplaceModelFile(DoodadsFile, doodads);
        return new(true, $"placed doodad {typeRawcode} at ({x}, {y}) as doodad #{creationNumber}", creationNumber);
    }

    /// <summary>Returns the map's parsed MapDoodads, creating an empty modern one if absent.</summary>
    internal static MapDoodads GetOrCreateDoodads(MapDocument doc)
    {
        if (doc.GetFile(DoodadsFile)?.Model is MapDoodads existing)
            return existing;
        return new MapDoodads(DefaultFormat, DefaultSubVersion, useNewFormat: true);
    }

    private static int NextDoodadCreationNumber(MapDoodads doodads) =>
        doodads.Doodads.Count == 0 ? 0 : doodads.Doodads.Max(d => d.CreationNumber) + 1;

    // ---- Regions (war3map.w3r / MapRegions) --------------------------------

    public sealed record PlaceRegionResult(bool Ok, string Message, int CreationNumber = -1);

    /// <summary>
    /// Adds a rectangular region [<paramref name="left"/>, <paramref name="bottom"/>] ..
    /// [<paramref name="right"/>, <paramref name="top"/>] named <paramref name="name"/>
    /// and returns its creation number (the region id). Writes war3map.w3r.
    /// </summary>
    public static PlaceRegionResult PlaceRegion(
        MapDocument doc,
        string name,
        float left,
        float bottom,
        float right,
        float top)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new(false, "invalid region name — must be non-empty");
        if (right <= left || top <= bottom)
            return new(false, $"invalid region bounds — need right>left and top>bottom, got L={left} B={bottom} R={right} T={top}");

        var regions = GetOrCreateRegions(doc);
        int creationNumber = NextRegionCreationNumber(regions);

        regions.Regions.Add(new Region
        {
            Left = left,
            Bottom = bottom,
            Right = right,
            Top = top,
            Name = name,
            CreationNumber = creationNumber,
            WeatherType = WeatherType.None,  // no weather effect
            AmbientSound = string.Empty,     // no ambient sound
            // The World Editor assigns each region a display colour; any valid colour works.
            Color = Color.FromArgb(255, 76, 255, 76),
        });

        doc.AddOrReplaceModelFile(RegionsFile, regions);
        return new(true, $"placed region '{name}' [{left},{bottom} .. {right},{top}] as region #{creationNumber}", creationNumber);
    }

    /// <summary>Returns the map's parsed MapRegions, creating an empty modern one if absent.</summary>
    internal static MapRegions GetOrCreateRegions(MapDocument doc)
    {
        if (doc.GetFile(RegionsFile)?.Model is MapRegions existing)
            return existing;
        return new MapRegions(MapRegionsFormatVersion.v5);
    }

    private static int NextRegionCreationNumber(MapRegions regions) =>
        regions.Regions.Count == 0 ? 0 : regions.Regions.Max(r => r.CreationNumber) + 1;

    /// <summary>One region's identity and bounds, for listing/editing front-ends.</summary>
    public sealed record RegionInfo(
        int CreationNumber, string Name, float Left, float Bottom, float Right, float Top);

    /// <summary>Lists the map's regions (war3map.w3r) in file order. Empty when the map has none.</summary>
    public static IReadOnlyList<RegionInfo> ListRegions(MapDocument doc)
    {
        if (doc.GetFile(RegionsFile)?.Model is not MapRegions regions)
            return Array.Empty<RegionInfo>();
        return regions.Regions
            .Select(r => new RegionInfo(r.CreationNumber, r.Name, r.Left, r.Bottom, r.Right, r.Top))
            .ToList();
    }

    /// <summary>Removes the first region with the given name (case-insensitive) and writes
    /// war3map.w3r. A miss returns Ok=false so the caller can report it.</summary>
    public static PlaceRegionResult RemoveRegion(MapDocument doc, string name)
    {
        if (doc.GetFile(RegionsFile)?.Model is not MapRegions regions)
            return new(false, $"map has no {RegionsFile} (no regions to remove)");
        var match = regions.Regions.FirstOrDefault(
            r => string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase));
        if (match is null)
            return new(false, $"no region named '{name}'");
        regions.Regions.Remove(match);
        doc.AddOrReplaceModelFile(RegionsFile, regions);
        return new(true, $"removed region '{name}'", match.CreationNumber);
    }
}
