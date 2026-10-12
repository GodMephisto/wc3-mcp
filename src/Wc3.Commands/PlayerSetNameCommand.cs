// src/Wc3.Commands/PlayerSetNameCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>Sets one player's display name in war3map.w3i, the World Editor's Scenario,
/// Player Properties "Name" field. The text is stored verbatim, so a previous TRIGSTR_
/// reference is replaced by the literal. Every other player field survives byte-for-byte
/// because the whole file re-serializes through the same path as
/// <see cref="PlayerForceCommand.SetPlayerForce"/>.</summary>
public static class PlayerSetNameCommand
{
    public static PlayerForceEditResult Execute(MapDocument doc, int playerId, string name)
        => PlayerForceCommand.Edit(doc, info =>
        {
            var player = PlayerForceCommand.FindPlayer(info, playerId);
            if (player is null)
                return new(false, $"map has no player with id {playerId}");
            if (name is null)
                return new(false, "a player name is required");
            player.Name = name;
            return new(true, $"player {playerId} name set to \"{name}\"");
        });
}
