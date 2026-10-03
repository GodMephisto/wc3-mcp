// src/Wc3.Commands/PlacedInstanceFields.cs
using System.Globalization;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Field-name dispatch for editing a placed unit or doodad, so the three front ends reach the
/// per-property setters through one named surface instead of each wiring fourteen verbs.
///
/// The setters themselves are typed and stay typed. This adds the string-keyed door the CLI and
/// MCP need, the same way <c>SoundCommand.Set</c> and <c>CameraCommand.Set</c> already work, and
/// keeps the field list in one place so a front end cannot advertise a field the layer does not
/// have or miss one it does.
/// </summary>
public static class PlacedInstanceFields
{
    /// <summary>Editable fields on a placed unit, in the order a person would look for them.</summary>
    public static readonly IReadOnlyList<string> UnitFields = new[]
    {
        "Owner", "X", "Y", "Position", "Facing", "Scale",
        "HeroLevel", "Strength", "Agility", "Intelligence",
        "HpPercent", "ManaPercent", "Gold", "TargetAcquisition",
    };

    /// <summary>Editable fields on a placed doodad.</summary>
    public static readonly IReadOnlyList<string> DoodadFields = new[]
    {
        "X", "Y", "Z", "Position", "Rotation", "Scale", "Variation", "LifePercent",
    };

    /// <summary>
    /// Sets one field on a placed unit. <paramref name="value"/> is parsed to the field's own
    /// type, and a positional or scale field accepts a comma-separated tuple.
    /// </summary>
    public static UnitEditResult SetUnitField(
        MapDocument doc, int creationNumber, string field, string value)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var current = UnitInstanceCommand.Get(doc, creationNumber);
        if (current is null)
            return new UnitEditResult(false, $"no placed unit with creation number {creationNumber}");

        switch (field.ToLowerInvariant())
        {
            case "owner":
                return Int(value, out var owner)
                    ? UnitInstanceCommand.SetOwner(doc, creationNumber, owner)
                    : Bad(field, value, "a player index");
            case "herolevel":
                return Int(value, out var lvl)
                    ? UnitInstanceCommand.SetHeroLevel(doc, creationNumber, lvl)
                    : Bad(field, value, "an integer level");
            case "strength":
                return Int(value, out var str)
                    ? UnitInstanceCommand.SetHeroStrength(doc, creationNumber, str)
                    : Bad(field, value, "an integer");
            case "agility":
                return Int(value, out var agi)
                    ? UnitInstanceCommand.SetHeroAgility(doc, creationNumber, agi)
                    : Bad(field, value, "an integer");
            case "intelligence":
                return Int(value, out var itl)
                    ? UnitInstanceCommand.SetHeroIntelligence(doc, creationNumber, itl)
                    : Bad(field, value, "an integer");
            case "hppercent":
                return Int(value, out var hp)
                    ? UnitInstanceCommand.SetHpPercent(doc, creationNumber, hp)
                    : Bad(field, value, "a percentage");
            case "manapercent":
                return Int(value, out var mana)
                    ? UnitInstanceCommand.SetManaPercent(doc, creationNumber, mana)
                    : Bad(field, value, "a percentage");
            case "gold":
                return Int(value, out var gold)
                    ? UnitInstanceCommand.SetGold(doc, creationNumber, gold)
                    : Bad(field, value, "an integer");
            case "targetacquisition":
                return Real(value, out var acq)
                    ? UnitInstanceCommand.SetTargetAcquisition(doc, creationNumber, acq)
                    : Bad(field, value, "a range");
            case "facing":
                return Real(value, out var facing)
                    ? UnitInstanceCommand.SetFacing(doc, creationNumber, facing)
                    : Bad(field, value, "an angle in radians");

            // A single axis keeps the other one, so moving along X does not silently zero Y.
            case "x":
                return Real(value, out var x)
                    ? UnitInstanceCommand.SetPosition(doc, creationNumber, x, current.Y)
                    : Bad(field, value, "a coordinate");
            case "y":
                return Real(value, out var y)
                    ? UnitInstanceCommand.SetPosition(doc, creationNumber, current.X, y)
                    : Bad(field, value, "a coordinate");
            case "position":
                return Tuple(value, 2, out var pos)
                    ? UnitInstanceCommand.SetPosition(doc, creationNumber, pos[0], pos[1])
                    : Bad(field, value, "x,y");
            case "scale":
                // One number scales uniformly, which is what a person almost always means.
                return Tuple(value, 3, out var s3)
                    ? UnitInstanceCommand.SetScale(doc, creationNumber, s3[0], s3[1], s3[2])
                    : Real(value, out var s1)
                        ? UnitInstanceCommand.SetScale(doc, creationNumber, s1, s1, s1)
                        : Bad(field, value, "one number or sx,sy,sz");

            default:
                return new UnitEditResult(false, Unknown(field, UnitFields));
        }
    }

    /// <summary>Sets one field on a placed doodad.</summary>
    public static DoodadEditResult SetDoodadField(
        MapDocument doc, int creationNumber, string field, string value)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var current = DoodadInstanceCommand.Get(doc, creationNumber);
        if (current is null)
            return new DoodadEditResult(false, $"no placed doodad with creation number {creationNumber}");

        switch (field.ToLowerInvariant())
        {
            case "rotation":
                return Real(value, out var rot)
                    ? DoodadInstanceCommand.SetRotation(doc, creationNumber, rot)
                    : new DoodadEditResult(false, BadText(field, value, "an angle in radians"));
            case "variation":
                return Int(value, out var v)
                    ? DoodadInstanceCommand.SetVariation(doc, creationNumber, v)
                    : new DoodadEditResult(false, BadText(field, value, "an integer variation"));
            case "lifepercent":
                return Int(value, out var life)
                    ? DoodadInstanceCommand.SetLifePercent(doc, creationNumber, life)
                    : new DoodadEditResult(false, BadText(field, value, "a percentage"));

            case "x":
                return Real(value, out var x)
                    ? DoodadInstanceCommand.SetPosition(doc, creationNumber, x, current.Y, current.Z)
                    : new DoodadEditResult(false, BadText(field, value, "a coordinate"));
            case "y":
                return Real(value, out var y)
                    ? DoodadInstanceCommand.SetPosition(doc, creationNumber, current.X, y, current.Z)
                    : new DoodadEditResult(false, BadText(field, value, "a coordinate"));
            case "z":
                return Real(value, out var z)
                    ? DoodadInstanceCommand.SetPosition(doc, creationNumber, current.X, current.Y, z)
                    : new DoodadEditResult(false, BadText(field, value, "a coordinate"));
            case "position":
                return Tuple(value, 3, out var p3)
                    ? DoodadInstanceCommand.SetPosition(doc, creationNumber, p3[0], p3[1], p3[2])
                    : Tuple(value, 2, out var p2)
                        ? DoodadInstanceCommand.SetPosition(doc, creationNumber, p2[0], p2[1], current.Z)
                        : new DoodadEditResult(false, BadText(field, value, "x,y or x,y,z"));
            case "scale":
                return Tuple(value, 3, out var s3)
                    ? DoodadInstanceCommand.SetScale(doc, creationNumber, s3[0], s3[1], s3[2])
                    : Real(value, out var s1)
                        ? DoodadInstanceCommand.SetScale(doc, creationNumber, s1, s1, s1)
                        : new DoodadEditResult(false, BadText(field, value, "one number or sx,sy,sz"));

            default:
                return new DoodadEditResult(false, Unknown(field, DoodadFields));
        }
    }

    private static bool Int(string s, out int v) =>
        int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out v);

    private static bool Real(string s, out float v) =>
        float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out v);

    private static bool Tuple(string s, int arity, out float[] v)
    {
        var parts = s.Split(',', StringSplitOptions.TrimEntries);
        v = new float[arity];
        if (parts.Length != arity) return false;
        for (int i = 0; i < arity; i++)
            if (!Real(parts[i], out v[i])) return false;
        return true;
    }

    private static UnitEditResult Bad(string field, string value, string expected) =>
        new(false, BadText(field, value, expected));

    private static string BadText(string field, string value, string expected) =>
        $"'{value}' is not valid for {field}, expected {expected}";

    private static string Unknown(string field, IReadOnlyList<string> fields) =>
        $"unknown field '{field}', expected one of: {string.Join("|", fields)}";
}
