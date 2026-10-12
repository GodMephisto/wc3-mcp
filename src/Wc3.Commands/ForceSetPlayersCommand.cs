// src/Wc3.Commands/ForceSetPlayersCommand.cs
using War3Net.Build.Common;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>Adds or removes one player from one force (team) in war3map.w3i.
///
/// Adding also clears the player's bit from every other force, because the World Editor keeps a
/// player in exactly one force so the force masks stay consistent. Removing clears the bit from
/// the target force only, which leaves the player ungrouped, a state the editor permits.</summary>
public static class ForceSetPlayersCommand
{
    public static PlayerForceEditResult Add(MapDocument doc, int forceIndex, int playerId)
        => PlayerForceCommand.Edit(doc, info =>
        {
            if (PlayerForceCommand.CheckForceIndex(info, forceIndex) is { } failure)
                return failure;
            if (PlayerForceCommand.FindPlayer(info, playerId) is null)
                return new(false, $"map has no player with id {playerId}");
            if (playerId is < 0 or > 31)
                return new(false, $"invalid player id {playerId}, must be 0..31");

            for (var i = 0; i < info.Forces!.Count; i++)
            {
                var force = info.Forces[i];
                force.Players ??= new Bitmask32();
                force.Players[playerId] = i == forceIndex;
            }
            return new(true, $"player {playerId} added to force {forceIndex}");
        });

    public static PlayerForceEditResult Remove(MapDocument doc, int forceIndex, int playerId)
        => PlayerForceCommand.Edit(doc, info =>
        {
            if (PlayerForceCommand.CheckForceIndex(info, forceIndex) is { } failure)
                return failure;
            if (PlayerForceCommand.FindPlayer(info, playerId) is null)
                return new(false, $"map has no player with id {playerId}");
            if (playerId is < 0 or > 31)
                return new(false, $"invalid player id {playerId}, must be 0..31");

            var force = info.Forces![forceIndex];
            force.Players ??= new Bitmask32();
            force.Players[playerId] = false;
            return new(true, $"player {playerId} removed from force {forceIndex}");
        });
}
