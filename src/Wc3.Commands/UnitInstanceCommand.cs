// src/Wc3.Commands/UnitInstanceCommand.cs
using War3Net.Build.Widget;
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One placed unit from war3mapUnits.doo. Scale is per-axis; HpPercent /
/// ManaPercent are the World Editor's percent overrides where -1 means "use the
/// object's default". Name is the map's object-data name delta for the type when one
/// exists ("Start Location" for sloc), else null — base-game names need a game-data
/// context which this hermetic read deliberately does not open.</summary>
public sealed record UnitInstanceInfo(
    int CreationNumber,
    string TypeRawcode,
    string? Name,
    int OwnerId,
    float X,
    float Y,
    float Rotation,
    (float Sx, float Sy, float Sz) Scale,
    int HeroLevel,
    int HeroStrength,
    int HeroAgility,
    int HeroIntelligence,
    int HpPercent,
    int ManaPercent,
    int GoldAmount,
    float TargetAcquisition);

public sealed record UnitEditResult(bool Ok, string Message);

/// <summary>
/// Read/edit access to the units already placed on the map (war3mapUnits.doo), the
/// way the World Editor's unit-properties dialog exposes them. Reads walk the parsed
/// War3Net <see cref="MapUnits"/> model; edits mutate the matching
/// <see cref="UnitData"/> in place and re-register the model via
/// <see cref="MapDocument.AddOrReplaceModelFile"/> (exactly the PlacementCommand
/// write path), so the file re-serializes on the next Save.
/// </summary>
public static class UnitInstanceCommand
{
    /// <summary>All placed units, start locations (sloc) included, in file order.
    /// Empty when the map has no war3mapUnits.doo.</summary>
    public static IReadOnlyList<UnitInstanceInfo> List(MapDocument doc)
    {
        if (doc.GetFile(PlacementCommand.UnitsFile)?.Model is not MapUnits units)
            return Array.Empty<UnitInstanceInfo>();

        var names = BuildNameLookup(doc);
        return units.Units.Select(u => ToInfo(u, names)).ToList();
    }

    /// <summary>The placed unit with this creation number, or null when absent.</summary>
    public static UnitInstanceInfo? Get(MapDocument doc, int creationNumber)
    {
        if (doc.GetFile(PlacementCommand.UnitsFile)?.Model is not MapUnits units)
            return null;
        var unit = units.Units.FirstOrDefault(u => u.CreationNumber == creationNumber);
        return unit is null ? null : ToInfo(unit, BuildNameLookup(doc));
    }

    public static UnitEditResult SetOwner(MapDocument doc, int creationNumber, int ownerId)
    {
        if (ownerId < 0)
            return new(false, $"invalid owner id {ownerId} — must be >= 0");
        return Mutate(doc, creationNumber, u => u.OwnerId = ownerId,
            $"owner set to {ownerId} ({PlayerColors.DisplayName(ownerId)})");
    }

    public static UnitEditResult SetHeroLevel(MapDocument doc, int creationNumber, int level)
    {
        if (level < 1)
            return new(false, $"invalid hero level {level} — must be >= 1");
        return Mutate(doc, creationNumber, u => u.HeroLevel = level, $"hero level set to {level}");
    }

    /// <summary>Sets the hero's Strength bonus (heroes only, 0 or more, matches the World Editor).</summary>
    public static UnitEditResult SetHeroStrength(MapDocument doc, int creationNumber, int value)
    {
        if (value < 0)
            return new(false, $"invalid strength {value} — must be >= 0");
        return Mutate(doc, creationNumber, u => u.HeroStrength = value, $"strength set to {value}");
    }

    /// <summary>Sets the hero's Agility bonus (heroes only, 0 or more).</summary>
    public static UnitEditResult SetHeroAgility(MapDocument doc, int creationNumber, int value)
    {
        if (value < 0)
            return new(false, $"invalid agility {value} — must be >= 0");
        return Mutate(doc, creationNumber, u => u.HeroAgility = value, $"agility set to {value}");
    }

    /// <summary>Sets the hero's Intelligence bonus (heroes only, 0 or more).</summary>
    public static UnitEditResult SetHeroIntelligence(MapDocument doc, int creationNumber, int value)
    {
        if (value < 0)
            return new(false, $"invalid intelligence {value} — must be >= 0");
        return Mutate(doc, creationNumber, u => u.HeroIntelligence = value, $"intelligence set to {value}");
    }

    /// <summary>Sets the target acquisition range in world units, or -1 for the object's default.</summary>
    public static UnitEditResult SetTargetAcquisition(MapDocument doc, int creationNumber, float range)
    {
        if (range < 0f && range != -1f)
            return new(false, $"invalid target acquisition {range} — must be >= 0 (or -1 for default)");
        return Mutate(doc, creationNumber, u => u.TargetAcquisition = range,
            $"target acquisition set to {(range == -1f ? "default" : range.ToString("0.##"))}");
    }

    /// <summary>Sets the HP percent override (0..100, or -1 = the object's default).</summary>
    public static UnitEditResult SetHpPercent(MapDocument doc, int creationNumber, int percent)
    {
        if (percent is not (-1) and (< 0 or > 100))
            return new(false, $"invalid HP percent {percent} — must be 0..100 (or -1 for default)");
        return Mutate(doc, creationNumber, u => u.HP = percent, $"HP set to {Pct(percent)}");
    }

    /// <summary>Sets the mana percent override (0..100, or -1 = the object's default).</summary>
    public static UnitEditResult SetManaPercent(MapDocument doc, int creationNumber, int percent)
    {
        if (percent is not (-1) and (< 0 or > 100))
            return new(false, $"invalid mana percent {percent} — must be 0..100 (or -1 for default)");
        return Mutate(doc, creationNumber, u => u.MP = percent, $"mana set to {Pct(percent)}");
    }

    public static UnitEditResult SetScale(MapDocument doc, int creationNumber, float sx, float sy, float sz)
    {
        if (sx <= 0f || sy <= 0f || sz <= 0f)
            return new(false, $"invalid scale ({sx}, {sy}, {sz}) — all axes must be > 0");
        return Mutate(doc, creationNumber,
            u => u.Scale = new System.Numerics.Vector3(sx, sy, sz),
            $"scale set to ({sx}, {sy}, {sz})");
    }

    /// <summary>Sets the unit's facing angle in radians (the .doo stores radians).</summary>
    public static UnitEditResult SetFacing(MapDocument doc, int creationNumber, float radians)
        => Mutate(doc, creationNumber, u => u.Rotation = radians, $"facing set to {radians} rad");

    /// <summary>Moves the unit to map coordinates (x, y), keeping its stored Z. For
    /// drag-to-move in the viewport.</summary>
    public static UnitEditResult SetPosition(MapDocument doc, int creationNumber, float x, float y)
        => Mutate(doc, creationNumber, u => u.Position = new System.Numerics.Vector3(x, y, u.Position.Z),
            $"moved to ({x:0}, {y:0})");

    /// <summary>Sets the gold amount carried by a gold-mine-style unit.</summary>
    public static UnitEditResult SetGold(MapDocument doc, int creationNumber, int amount)
    {
        if (amount < 0)
            return new(false, $"invalid gold amount {amount} — must be >= 0");
        return Mutate(doc, creationNumber, u => u.GoldAmount = amount, $"gold set to {amount}");
    }

    /// <summary>Removes the placed unit with this creation number. Ok=false when absent.</summary>
    public static UnitEditResult Delete(MapDocument doc, int creationNumber)
        => DeleteMany(doc, new[] { creationNumber });

    /// <summary>Removes every placed unit whose creation number is in the set (the
    /// multi-select delete). Ok when at least one was removed.</summary>
    public static UnitEditResult DeleteMany(MapDocument doc, IEnumerable<int> creationNumbers)
    {
        if (doc.GetFile(PlacementCommand.UnitsFile)?.Model is not MapUnits units)
            return new(false, $"map has no {PlacementCommand.UnitsFile} — nothing is placed");
        var set = new HashSet<int>(creationNumbers);
        if (set.Count == 0)
            return new(false, "no units selected");
        int removed = units.Units.RemoveAll(u => set.Contains(u.CreationNumber));
        if (removed == 0)
            return new(false, "none of the selected units were found");
        PlacementCommand.CommitUnits(doc, units);
        return new(true, removed == 1 ? "removed 1 unit" : $"removed {removed} units");
    }

    /// <summary>Sets the owner on every unit in the set in one write (multi-select
    /// set-team). Ok when at least one was changed.</summary>
    public static UnitEditResult SetOwnerMany(MapDocument doc, IEnumerable<int> creationNumbers, int ownerId)
    {
        if (ownerId < 0)
            return new(false, $"invalid owner id {ownerId} — must be >= 0");
        if (doc.GetFile(PlacementCommand.UnitsFile)?.Model is not MapUnits units)
            return new(false, $"map has no {PlacementCommand.UnitsFile} — nothing is placed");
        var set = new HashSet<int>(creationNumbers);
        int changed = 0;
        foreach (var u in units.Units)
            if (set.Contains(u.CreationNumber)) { u.OwnerId = ownerId; changed++; }
        if (changed == 0)
            return new(false, "none of the selected units were found");
        PlacementCommand.CommitUnits(doc, units);
        return new(true, $"set {changed} unit(s) to {PlayerColors.DisplayName(ownerId)}");
    }

    /// <summary>Finds the unit, applies the edit, and re-registers the MapUnits model
    /// (marks war3mapUnits.doo dirty so the change persists on Save).</summary>
    private static UnitEditResult Mutate(
        MapDocument doc, int creationNumber, Action<UnitData> edit, string appliedMessage)
    {
        if (doc.GetFile(PlacementCommand.UnitsFile)?.Model is not MapUnits units)
            return new(false, $"map has no {PlacementCommand.UnitsFile} — nothing is placed");
        var unit = units.Units.FirstOrDefault(u => u.CreationNumber == creationNumber);
        if (unit is null)
            return new(false, $"no placed unit with creation number {creationNumber}");

        edit(unit);
        // CommitUnits (not a plain model write) so the runtime creation script re-applies the change,
        // a scripted map spawns from CreateAllUnits, not from the .doo, so an edit that only touched
        // the .doo would never show up in game.
        PlacementCommand.CommitUnits(doc, units);
        return new(true, $"unit #{creationNumber}: {appliedMessage}");
    }

    private static UnitInstanceInfo ToInfo(UnitData u, IReadOnlyDictionary<int, string> names) => new(
        CreationNumber: u.CreationNumber,
        TypeRawcode: u.TypeId.ToRawcode(),
        Name: names.TryGetValue(u.TypeId, out var name) ? name : null,
        OwnerId: u.OwnerId,
        X: u.Position.X,
        Y: u.Position.Y,
        Rotation: u.Rotation,
        Scale: (u.Scale.X, u.Scale.Y, u.Scale.Z),
        HeroLevel: u.HeroLevel,
        HeroStrength: u.HeroStrength,
        HeroAgility: u.HeroAgility,
        HeroIntelligence: u.HeroIntelligence,
        HpPercent: u.HP,
        ManaPercent: u.MP,
        GoldAmount: u.GoldAmount,
        TargetAcquisition: u.TargetAcquisition);

    /// <summary>TypeId → display name from the map's unit object-data name deltas
    /// (TRIGSTR_ refs resolved against war3map.wts), plus the sloc pseudo-type.</summary>
    private static IReadOnlyDictionary<int, string> BuildNameLookup(MapDocument doc)
    {
        var names = new Dictionary<int, string>
        {
            [PlacementCommand.StartLocationRawcode.FromRawcode()] = "Start Location",
        };
        var info = ObjectKinds.Info(ObjectKind.Unit);
        var strings = MapStrings.From(doc);
        foreach (var entry in ObjectKinds.MergedEntries(doc, info))
        {
            if (ObjectKinds.DeltaName(ObjectKinds.ModsToDict(entry.Mods), info, strings) is { } name)
                names[entry.Id] = name;
        }
        return names;
    }

    private static string Pct(int percent) => percent == -1 ? "default" : $"{percent}%";
}
