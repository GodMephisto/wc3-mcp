// src/Wc3.Commands/PlayerSetRaceCommand.cs
using War3Net.Build.Info;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>Sets one player's race in war3map.w3i, the World Editor's Scenario, Player
/// Properties "Race" field. The w3i stores a race id per player and the game reads it for the
/// slot's default race. The command's "random" maps to <see cref="PlayerRace.Selectable"/>,
/// the value the editor writes for a slot left on random.</summary>
public static class PlayerSetRaceCommand
{
    private static readonly IReadOnlyDictionary<string, PlayerRace> ByName =
        new Dictionary<string, PlayerRace>(StringComparer.OrdinalIgnoreCase)
        {
            ["human"] = PlayerRace.Human,
            ["orc"] = PlayerRace.Orc,
            ["undead"] = PlayerRace.Undead,
            ["nightelf"] = PlayerRace.NightElf,
            ["random"] = PlayerRace.Selectable,
        };

    /// <summary>The race names this command accepts, lowercase, in the editor's dropdown
    /// order.</summary>
    public static IReadOnlyList<string> RaceNames { get; } = new[]
    {
        "human", "orc", "undead", "nightelf", "random",
    };

    /// <summary>Resolves a race name, ignoring case and separators, so "Night Elf" and
    /// "nightelf" both resolve. False when the text names none of the five values.</summary>
    public static bool TryParse(string? text, out PlayerRace race)
    {
        race = PlayerRace.Selectable;
        if (string.IsNullOrWhiteSpace(text))
            return false;
        var key = new string(text.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
        return ByName.TryGetValue(key, out race);
    }

    public static PlayerForceEditResult Execute(MapDocument doc, int playerId, string race)
    {
        if (!TryParse(race, out var parsed))
            return new(false,
                $"unknown race '{race}', expected one of {string.Join(", ", RaceNames)}");
        return PlayerForceCommand.Edit(doc, info =>
        {
            var player = PlayerForceCommand.FindPlayer(info, playerId);
            if (player is null)
                return new(false, $"map has no player with id {playerId}");
            player.Race = parsed;
            return new(true,
                $"player {playerId} race set to {new string(race.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant()}");
        });
    }
}
