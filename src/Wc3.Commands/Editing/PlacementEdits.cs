// src/Wc3.Commands/Editing/PlacementEdits.cs
using War3Net.Build.Environment; // MapRegions, Region
using War3Net.Build.Widget;      // MapDoodads, DoodadData, MapUnits, UnitData
using Wc3.Model;

namespace Wc3.Commands.Editing;

/// <summary>
/// Places a doodad (war3map.doo). The first <see cref="Apply"/> delegates to
/// <see cref="PlacementCommand.PlaceDoodad"/> so the World-Editor default fields
/// live in one place, then captures the created <see cref="DoodadData"/>.
/// <see cref="Revert"/> removes that exact instance; a later Apply (redo) re-adds
/// the very same instance, so the CreationNumber — and any trigger reference keyed
/// off it — is stable across undo/redo.
/// </summary>
public sealed class PlaceDoodadEdit : IMapEdit
{
    private readonly string _type;
    private readonly float _x, _y, _z, _rotation, _scale;
    private readonly int _variation;
    private DoodadData? _placed;

    public PlaceDoodadEdit(string typeRawcode, float x, float y,
        float z = 0f, float rotation = 0f, float scale = 1f, int variation = 0)
    {
        _type = typeRawcode;
        _x = x; _y = y; _z = z;
        _rotation = rotation; _scale = scale; _variation = variation;
    }

    /// <summary>CreationNumber assigned on first Apply (-1 before then).</summary>
    public int CreationNumber => _placed?.CreationNumber ?? -1;

    public string Describe => $"Place doodad {_type}";

    public void Apply(MapDocument doc)
    {
        if (_placed is null)
        {
            var r = PlacementCommand.PlaceDoodad(doc, _type, _x, _y, _z, _rotation, _scale, _variation);
            if (!r.Ok)
                throw new InvalidOperationException(r.Message);
            var doodads = ModelOf(doc);
            _placed = doodads.Doodads.First(d => d.CreationNumber == r.CreationNumber);
        }
        else
        {
            var doodads = PlacementCommand.GetOrCreateDoodads(doc);
            doodads.Doodads.Add(_placed);
            doc.AddOrReplaceModelFile(PlacementCommand.DoodadsFile, doodads);
        }
    }

    public void Revert(MapDocument doc)
    {
        var doodads = PlacementCommand.GetOrCreateDoodads(doc);
        if (_placed is not null)
            doodads.Doodads.Remove(_placed);
        doc.AddOrReplaceModelFile(PlacementCommand.DoodadsFile, doodads);
    }

    private static MapDoodads ModelOf(MapDocument doc) =>
        doc.GetFile(PlacementCommand.DoodadsFile)?.Model as MapDoodads
        ?? throw new InvalidOperationException("war3map.doo missing after placement");
}

/// <summary>
/// Shared undo/redo mechanics for anything that appends a <see cref="UnitData"/> to
/// war3mapUnits.doo, which holds units AND preplaced items alike. The first Apply
/// constructs the entry through the subclass (so the World-Editor defaults live in
/// PlacementCommand), captures it, and later Applies re-add that exact instance while
/// Revert removes it, keeping the CreationNumber (and any trigger reference keyed off
/// it) stable across undo/redo. Subclasses supply only the initial placement and label.
/// </summary>
public abstract class MapUnitsEdit : IMapEdit
{
    private UnitData? _placed;

    /// <summary>CreationNumber assigned on first Apply (-1 before then).</summary>
    public int CreationNumber => _placed?.CreationNumber ?? -1;

    public abstract string Describe { get; }

    /// <summary>Places the entry for the first time; returns the command outcome.</summary>
    protected abstract (bool Ok, string Message, int CreationNumber) PlaceFirst(MapDocument doc);

    public void Apply(MapDocument doc)
    {
        if (_placed is null)
        {
            var r = PlaceFirst(doc);
            if (!r.Ok)
                throw new InvalidOperationException(r.Message);
            _placed = ModelOf(doc).Units.First(u => u.CreationNumber == r.CreationNumber);
        }
        else
        {
            var units = PlacementCommand.GetOrCreateUnits(doc);
            units.Units.Add(_placed);
            PlacementCommand.CommitUnits(doc, units);
        }
    }

    public void Revert(MapDocument doc)
    {
        var units = PlacementCommand.GetOrCreateUnits(doc);
        if (_placed is not null)
            units.Units.Remove(_placed);
        PlacementCommand.CommitUnits(doc, units);
    }

    private static MapUnits ModelOf(MapDocument doc) =>
        doc.GetFile(PlacementCommand.UnitsFile)?.Model as MapUnits
        ?? throw new InvalidOperationException("war3mapUnits.doo missing after placement");
}

/// <summary>
/// Places a unit (war3mapUnits.doo) owned by a player. Mirrors <see cref="PlaceDoodadEdit"/>
/// via <see cref="MapUnitsEdit"/>.
/// </summary>
public sealed class PlaceUnitEdit : MapUnitsEdit
{
    private readonly string _type;
    private readonly int _ownerId;
    private readonly float _x, _y, _z, _rotation, _scale;

    public PlaceUnitEdit(string typeRawcode, int ownerId, float x, float y,
        float z = 0f, float rotation = 0f, float scale = 1f)
    {
        _type = typeRawcode;
        _ownerId = ownerId;
        _x = x; _y = y; _z = z;
        _rotation = rotation; _scale = scale;
    }

    public override string Describe => $"Place unit {_type}";

    protected override (bool, string, int) PlaceFirst(MapDocument doc)
    {
        var r = PlacementCommand.PlaceUnit(doc, _type, _ownerId, _x, _y, _z, _rotation, _scale);
        return (r.Ok, r.Message, r.CreationNumber);
    }
}

/// <summary>
/// Places a preplaced item (a <see cref="UnitData"/> in war3mapUnits.doo owned by the item
/// slot). Same undo/redo mechanics as <see cref="PlaceUnitEdit"/>; differs only in the
/// initial placement call, which pins the owner to <see cref="PlacementCommand.ItemOwnerId"/>.
/// </summary>
public sealed class PlaceItemEdit : MapUnitsEdit
{
    private readonly string _type;
    private readonly float _x, _y, _z, _rotation, _scale;

    public PlaceItemEdit(string typeRawcode, float x, float y,
        float z = 0f, float rotation = 0f, float scale = 1f)
    {
        _type = typeRawcode;
        _x = x; _y = y; _z = z;
        _rotation = rotation; _scale = scale;
    }

    public override string Describe => $"Place item {_type}";

    protected override (bool, string, int) PlaceFirst(MapDocument doc)
    {
        var r = PlacementCommand.PlaceItem(doc, _type, _x, _y, _z, _rotation, _scale);
        return (r.Ok, r.Message, r.CreationNumber);
    }
}

/// <summary>
/// Adds a rectangular region (war3map.w3r). Construct once via
/// <see cref="PlacementCommand.PlaceRegion"/>, then add/remove the captured
/// <see cref="Region"/> by reference.
/// </summary>
public sealed class PlaceRegionEdit : IMapEdit
{
    private readonly string _name;
    private readonly float _left, _bottom, _right, _top;
    private Region? _placed;

    public PlaceRegionEdit(string name, float left, float bottom, float right, float top)
    {
        _name = name;
        _left = left; _bottom = bottom; _right = right; _top = top;
    }

    /// <summary>CreationNumber assigned on first Apply (-1 before then).</summary>
    public int CreationNumber => _placed?.CreationNumber ?? -1;

    public string Describe => $"Place region {_name}";

    public void Apply(MapDocument doc)
    {
        if (_placed is null)
        {
            var r = PlacementCommand.PlaceRegion(doc, _name, _left, _bottom, _right, _top);
            if (!r.Ok)
                throw new InvalidOperationException(r.Message);
            var regions = ModelOf(doc);
            _placed = regions.Regions.First(x => x.CreationNumber == r.CreationNumber);
        }
        else
        {
            var regions = PlacementCommand.GetOrCreateRegions(doc);
            regions.Regions.Add(_placed);
            doc.AddOrReplaceModelFile(PlacementCommand.RegionsFile, regions);
        }
    }

    public void Revert(MapDocument doc)
    {
        var regions = PlacementCommand.GetOrCreateRegions(doc);
        if (_placed is not null)
            regions.Regions.Remove(_placed);
        doc.AddOrReplaceModelFile(PlacementCommand.RegionsFile, regions);
    }

    private static MapRegions ModelOf(MapDocument doc) =>
        doc.GetFile(PlacementCommand.RegionsFile)?.Model as MapRegions
        ?? throw new InvalidOperationException("war3map.w3r missing after placement");
}

/// <summary>
/// Deletes the doodad with a given CreationNumber (the inverse of
/// <see cref="PlaceDoodadEdit"/>). Captures the removed <see cref="DoodadData"/>
/// on Apply so Revert restores it unchanged. A no-op if no such doodad exists.
/// </summary>
public sealed class RemoveDoodadEdit : IMapEdit
{
    private readonly int _creationNumber;
    private DoodadData? _removed;

    public RemoveDoodadEdit(int creationNumber) => _creationNumber = creationNumber;

    public string Describe => $"Delete doodad #{_creationNumber}";

    public void Apply(MapDocument doc)
    {
        var doodads = PlacementCommand.GetOrCreateDoodads(doc);
        _removed = doodads.Doodads.FirstOrDefault(d => d.CreationNumber == _creationNumber);
        if (_removed is not null)
            doodads.Doodads.Remove(_removed);
        doc.AddOrReplaceModelFile(PlacementCommand.DoodadsFile, doodads);
    }

    public void Revert(MapDocument doc)
    {
        if (_removed is null)
            return;
        var doodads = PlacementCommand.GetOrCreateDoodads(doc);
        doodads.Doodads.Add(_removed);
        doc.AddOrReplaceModelFile(PlacementCommand.DoodadsFile, doodads);
    }
}

/// <summary>
/// Deletes the unit with a given CreationNumber (the inverse of
/// <see cref="PlaceUnitEdit"/>). Captures the removed <see cref="UnitData"/> on
/// Apply so Revert restores it unchanged. A no-op if no such unit exists.
/// </summary>
public sealed class RemoveUnitEdit : IMapEdit
{
    private readonly int _creationNumber;
    private UnitData? _removed;

    public RemoveUnitEdit(int creationNumber) => _creationNumber = creationNumber;

    public string Describe => $"Delete unit #{_creationNumber}";

    public void Apply(MapDocument doc)
    {
        var units = PlacementCommand.GetOrCreateUnits(doc);
        _removed = units.Units.FirstOrDefault(u => u.CreationNumber == _creationNumber);
        if (_removed is not null)
            units.Units.Remove(_removed);
        PlacementCommand.CommitUnits(doc, units);
    }

    public void Revert(MapDocument doc)
    {
        if (_removed is null)
            return;
        var units = PlacementCommand.GetOrCreateUnits(doc);
        units.Units.Add(_removed);
        PlacementCommand.CommitUnits(doc, units);
    }
}
