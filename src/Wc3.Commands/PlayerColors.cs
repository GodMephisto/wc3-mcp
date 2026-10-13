// src/Wc3.Commands/PlayerColors.cs
namespace Wc3.Commands;

/// <summary>
/// One team color as a named record rather than a <c>(byte R, byte G, byte B)</c> tuple. A
/// ValueTuple serializes to an EMPTY JSON object under System.Text.Json, because its components are
/// fields and fields are excluded by default, so player_list reported <c>"color":{}</c> for every
/// slot while the value was present the whole time. A record exposes R, G and B as properties, so
/// the color survives serialization on both the CLI and the MCP surface.
/// </summary>
public sealed record PlayerColorRgb(byte R, byte G, byte B);

/// <summary>
/// The Reforged 28-slot player-color table: ids 0..23 are the playable slots
/// (TeamColor00..TeamColor23 — classic 12 plus the twelve added in patch 1.29),
/// ids 24..27 are the neutral slots (24 Neutral Hostile, 25 Neutral Victim,
/// 26 Neutral Extra, 27 Neutral Passive — the JASS PLAYER_NEUTRAL_* ids).
/// The game tints all four neutral slots with the black team color; we give them
/// slightly separated near-black shades so UI swatches stay distinguishable
/// (approximation — the neutrals have no distinct canonical colors).
/// </summary>
public static class PlayerColors
{
    /// <summary>Total slots (24 playable + 4 neutral).</summary>
    public const int SlotCount = 28;

    /// <summary>Highest playable slot id (0-based); 24..27 are the neutral slots.</summary>
    public const int MaxPlayableId = 23;

    public const int NeutralHostileId = 24;
    public const int NeutralVictimId = 25;
    public const int NeutralExtraId = 26;
    public const int NeutralPassiveId = 27;

    private static readonly (byte R, byte G, byte B)[] Table =
    {
        (0xFF, 0x03, 0x03), // 0  Red
        (0x00, 0x42, 0xFF), // 1  Blue
        (0x1C, 0xE6, 0xB9), // 2  Teal
        (0x54, 0x00, 0x81), // 3  Purple
        (0xFF, 0xFC, 0x01), // 4  Yellow
        (0xFE, 0xBA, 0x0E), // 5  Orange
        (0x20, 0xC0, 0x00), // 6  Green
        (0xE5, 0x5B, 0xB0), // 7  Pink
        (0x95, 0x96, 0x97), // 8  Gray
        (0x7E, 0xBF, 0xF1), // 9  Light Blue
        (0x10, 0x62, 0x46), // 10 Dark Green
        (0x4E, 0x2A, 0x04), // 11 Brown
        (0x9B, 0x00, 0x00), // 12 Maroon
        (0x00, 0x00, 0xC3), // 13 Navy
        (0x00, 0xEA, 0xFF), // 14 Turquoise
        (0xBE, 0x00, 0xFE), // 15 Violet
        (0xEB, 0xCD, 0x87), // 16 Wheat
        (0xF8, 0xA4, 0x8B), // 17 Peach
        (0xBF, 0xFF, 0x80), // 18 Mint
        (0xDC, 0xB9, 0xEB), // 19 Lavender
        (0x28, 0x28, 0x28), // 20 Coal
        (0xEB, 0xF0, 0xFF), // 21 Snow
        (0x00, 0x78, 0x1E), // 22 Emerald
        (0xA4, 0x6F, 0x33), // 23 Peanut
        (0x0D, 0x0D, 0x0D), // 24 Neutral Hostile  (near-black, see class doc)
        (0x1A, 0x1A, 0x1A), // 25 Neutral Victim
        (0x30, 0x30, 0x30), // 26 Neutral Extra
        (0x45, 0x45, 0x45), // 27 Neutral Passive
    };

    private static readonly string[] Names =
    {
        "Red", "Blue", "Teal", "Purple", "Yellow", "Orange", "Green", "Pink",
        "Gray", "Light Blue", "Dark Green", "Brown", "Maroon", "Navy",
        "Turquoise", "Violet", "Wheat", "Peach", "Mint", "Lavender",
        "Coal", "Snow", "Emerald", "Peanut",
    };

    /// <summary>The slot's team color. Out-of-range ids fall back to the gray
    /// slot's color rather than throwing — safe for rendering weird owner ids.</summary>
    public static PlayerColorRgb Color(int id)
    {
        var t = id >= 0 && id < SlotCount ? Table[id] : Table[8];
        return new PlayerColorRgb(t.R, t.G, t.B);
    }

    /// <summary>"Player 1 (Red)" .. "Player 24 (Peanut)", the neutral slot names for
    /// 24..27, and a bare "Player N" for out-of-range ids.</summary>
    public static string DisplayName(int id) => id switch
    {
        >= 0 and <= MaxPlayableId => $"Player {id + 1} ({Names[id]})",
        NeutralHostileId => "Neutral Hostile",
        NeutralVictimId => "Neutral Victim",
        NeutralExtraId => "Neutral Extra",
        NeutralPassiveId => "Neutral Passive",
        _ => $"Player {id + 1}",
    };

    /// <summary>The 24 playable color names in id order, matching the World Editor's
    /// player-properties color list (ids 12..23 arrived with Reforged). The four neutral
    /// slots have no canonical name and are deliberately excluded.</summary>
    public static IReadOnlyList<string> ColorNames { get; } = Array.AsReadOnly(Names);

    /// <summary>Resolves a color name to its 0-based team color id the way the World Editor
    /// spells it, ignoring case and the separators in "Light Blue" or "dark_green", so
    /// "lightblue", "Light Blue" and "LIGHT-BLUE" all resolve to id 9. False when the text
    /// names none of the 24 playable slots.</summary>
    public static bool TryParseId(string? name, out int id)
    {
        id = -1;
        if (string.IsNullOrWhiteSpace(name))
            return false;
        var wanted = Normalize(name);
        for (var i = 0; i < Names.Length; i++)
        {
            if (Normalize(Names[i]) != wanted)
                continue;
            id = i;
            return true;
        }
        return false;
    }

    private static string Normalize(string text)
    {
        var buffer = new System.Text.StringBuilder(text.Length);
        foreach (var c in text)
            if (char.IsLetterOrDigit(c))
                buffer.Append(char.ToLowerInvariant(c));
        return buffer.ToString();
    }
}
