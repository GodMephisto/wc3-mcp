// src/Wc3.Commands/DoodadInstanceCommand.cs
using War3Net.Build.Widget;
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One placed doodad/destructable from war3map.doo. Scale is per-axis;
/// LifePercent is the World Editor's 0..100 health override (the .doo stores it as a
/// byte). Name is the map's object-data name delta for the type when one exists —
/// war3map.doo holds both destructables (w3b deltas) and doodads (w3d deltas), so both
/// are consulted — else null; base-game names need a game-data context which this
/// hermetic read deliberately does not open.</summary>
public sealed record DoodadInstanceInfo(
    int CreationNumber,
    string TypeRawcode,
    string? Name,
    float X,
    float Y,
    float Z,
    float Rotation,
    (float Sx, float Sy, float Sz) Scale,
    int Variation,
    int LifePercent);

public sealed record DoodadEditResult(bool Ok, string Message);

/// <summary>
/// Read/edit access to the doodads/destructables already placed on the map
/// (war3map.doo), the way the World Editor's doodad-properties dialog exposes them.
/// The doodad analogue of <see cref="UnitInstanceCommand"/>: reads walk the parsed
/// War3Net <see cref="MapDoodads"/> model; edits mutate the matching
/// <see cref="DoodadData"/> in place and re-register the model via
/// <see cref="MapDocument.AddOrReplaceModelFile"/> (exactly the PlacementCommand
/// write path), so the file re-serializes on the next Save.
/// </summary>
public static class DoodadInstanceCommand
{
    /// <summary>All placed doodads/destructables, in file order. Empty when the map
    /// has no war3map.doo.</summary>
    public static IReadOnlyList<DoodadInstanceInfo> List(MapDocument doc)
    {
        if (doc.GetFile(PlacementCommand.DoodadsFile)?.Model is not MapDoodads doodads)
            return Array.Empty<DoodadInstanceInfo>();

        var names = BuildNameLookup(doc);
        return doodads.Doodads.Select(d => ToInfo(d, names)).ToList();
    }

    /// <summary>The placed doodad with this creation number, or null when absent.</summary>
    public static DoodadInstanceInfo? Get(MapDocument doc, int creationNumber)
    {
        if (doc.GetFile(PlacementCommand.DoodadsFile)?.Model is not MapDoodads doodads)
            return null;
        var doodad = doodads.Doodads.FirstOrDefault(d => d.CreationNumber == creationNumber);
        return doodad is null ? null : ToInfo(doodad, BuildNameLookup(doc));
    }

    public static DoodadEditResult SetPosition(MapDocument doc, int creationNumber, float x, float y, float z)
        => Mutate(doc, creationNumber,
            d => d.Position = new System.Numerics.Vector3(x, y, z),
            $"position set to ({x}, {y}, {z})");

    /// <summary>Sets the doodad's facing angle in radians (the .doo stores radians).</summary>
    public static DoodadEditResult SetRotation(MapDocument doc, int creationNumber, float radians)
        => Mutate(doc, creationNumber, d => d.Rotation = radians, $"rotation set to {radians} rad");

    public static DoodadEditResult SetScale(MapDocument doc, int creationNumber, float sx, float sy, float sz)
    {
        if (sx <= 0f || sy <= 0f || sz <= 0f)
            return new(false, $"invalid scale ({sx}, {sy}, {sz}) — all axes must be > 0");
        return Mutate(doc, creationNumber,
            d => d.Scale = new System.Numerics.Vector3(sx, sy, sz),
            $"scale set to ({sx}, {sy}, {sz})");
    }

    /// <summary>Sets which model variation the doodad displays (>= 0; how many exist
    /// is a property of the doodad type, which this hermetic edit cannot see).</summary>
    public static DoodadEditResult SetVariation(MapDocument doc, int creationNumber, int variation)
    {
        if (variation < 0)
            return new(false, $"invalid variation {variation} — must be >= 0");
        return Mutate(doc, creationNumber, d => d.Variation = variation, $"variation set to {variation}");
    }

    /// <summary>Sets the health percent (0..100). Unlike units, the .doo stores doodad
    /// life as a plain byte percent with no -1 "default" sentinel.</summary>
    public static DoodadEditResult SetLifePercent(MapDocument doc, int creationNumber, int percent)
    {
        if (percent is < 0 or > 100)
            return new(false, $"invalid life percent {percent} — must be 0..100");
        return Mutate(doc, creationNumber, d => d.Life = (byte)percent, $"life set to {percent}%");
    }

    /// <summary>Removes the placed doodad from the map (marks war3map.doo dirty).</summary>
    public static DoodadEditResult Delete(MapDocument doc, int creationNumber)
    {
        if (doc.GetFile(PlacementCommand.DoodadsFile)?.Model is not MapDoodads doodads)
            return new(false, $"map has no {PlacementCommand.DoodadsFile} — nothing is placed");
        var doodad = doodads.Doodads.FirstOrDefault(d => d.CreationNumber == creationNumber);
        if (doodad is null)
            return new(false, $"no placed doodad with creation number {creationNumber}");

        doodads.Doodads.Remove(doodad);
        doc.AddOrReplaceModelFile(PlacementCommand.DoodadsFile, doodads);
        return new(true, $"doodad #{creationNumber}: deleted ({doodad.TypeId.ToRawcode()})");
    }

    /// <summary>Finds the doodad, applies the edit, and re-registers the MapDoodads
    /// model (marks war3map.doo dirty so the change persists on Save).</summary>
    private static DoodadEditResult Mutate(
        MapDocument doc, int creationNumber, Action<DoodadData> edit, string appliedMessage)
    {
        if (doc.GetFile(PlacementCommand.DoodadsFile)?.Model is not MapDoodads doodads)
            return new(false, $"map has no {PlacementCommand.DoodadsFile} — nothing is placed");
        var doodad = doodads.Doodads.FirstOrDefault(d => d.CreationNumber == creationNumber);
        if (doodad is null)
            return new(false, $"no placed doodad with creation number {creationNumber}");

        edit(doodad);
        doc.AddOrReplaceModelFile(PlacementCommand.DoodadsFile, doodads);
        return new(true, $"doodad #{creationNumber}: {appliedMessage}");
    }

    private static DoodadInstanceInfo ToInfo(DoodadData d, IReadOnlyDictionary<int, string> names) => new(
        CreationNumber: d.CreationNumber,
        TypeRawcode: d.TypeId.ToRawcode(),
        Name: names.TryGetValue(d.TypeId, out var name) ? name : null,
        X: d.Position.X,
        Y: d.Position.Y,
        Z: d.Position.Z,
        Rotation: d.Rotation,
        Scale: (d.Scale.X, d.Scale.Y, d.Scale.Z),
        Variation: d.Variation,
        LifePercent: d.Life);

    /// <summary>TypeId → display name from the map's destructable (w3b) and doodad (w3d)
    /// object-data name deltas — war3map.doo mixes both kinds — with TRIGSTR_ refs
    /// resolved against war3map.wts.</summary>
    private static IReadOnlyDictionary<int, string> BuildNameLookup(MapDocument doc)
    {
        var names = new Dictionary<int, string>();
        var strings = MapStrings.From(doc);
        foreach (var kind in new[] { ObjectKind.Destructable, ObjectKind.Doodad })
        {
            var info = ObjectKinds.Info(kind);
            foreach (var entry in ObjectKinds.MergedEntries(doc, info))
            {
                if (ObjectKinds.DeltaName(ObjectKinds.ModsToDict(entry.Mods), info, strings) is { } name)
                    names[entry.Id] = name;
            }
        }
        return names;
    }
}
