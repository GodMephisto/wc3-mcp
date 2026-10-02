// src/Wc3.Commands/PlayerForceCommand.cs
using War3Net.Build.Common;
using War3Net.Build.Info;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One player slot from war3map.w3i. Name is display-resolved (TRIGSTR_ refs
/// via war3map.wts); Color always comes from <see cref="PlayerColors"/> by slot id —
/// the w3i model this War3Net version parses carries no per-player custom color.</summary>
public sealed record PlayerInfo(
    int Id,
    string Name,
    (byte R, byte G, byte B) Color,
    string Race,
    string Controller,
    bool FixedStartPosition);

/// <summary>One force (World Editor "team") from war3map.w3i. PlayerIds lists the ids
/// of the map's actual players whose bit is set in the force's player mask, in player
/// order (mask bits for non-existent slots are ignored for display but preserved in
/// the file).</summary>
public sealed record ForceInfo(
    int Index,
    string Name,
    IReadOnlyList<int> PlayerIds,
    bool Allied,
    bool AlliedVictory,
    bool SharedVision,
    bool SharedUnitControl,
    bool SharedAdvUnitControl);

public sealed record PlayerForceEditResult(bool Ok, string Message);

/// <summary>
/// Read/edit access to the map's players and forces (war3map.w3i / War3Net
/// <see cref="MapInfo"/>), the World Editor's Scenario → Players / Forces dialogs.
/// Edits mutate the parsed MapInfo and re-serialize the WHOLE file exactly the way
/// <see cref="MapInfoCommand.Set"/> does (raw bytes + restored Model, dirty on Save),
/// so untouched fields — including the Unk* unknowns — survive byte-for-byte.
/// </summary>
public static class PlayerForceCommand
{
    /// <summary>The map's player slots. Empty when the map has no parseable w3i.</summary>
    public static IReadOnlyList<PlayerInfo> GetPlayers(MapDocument doc)
    {
        if (doc.GetFile(MapInfoCommand.FileName)?.Model is not MapInfo info
            || info.Players is null)
            return Array.Empty<PlayerInfo>();

        var strings = MapStrings.From(doc);
        return info.Players.Select(p => new PlayerInfo(
            Id: p.Id,
            Name: strings.Resolve(p.Name),
            Color: PlayerColors.Color(p.Id),
            Race: p.Race.ToString(),
            Controller: p.Controller.ToString(),
            FixedStartPosition: p.Flags.HasFlag(PlayerFlags.FixedStartPosition))).ToList();
    }

    /// <summary>The map's forces (teams). Empty when the map has no parseable w3i.</summary>
    public static IReadOnlyList<ForceInfo> GetForces(MapDocument doc)
    {
        if (doc.GetFile(MapInfoCommand.FileName)?.Model is not MapInfo info
            || info.Forces is null)
            return Array.Empty<ForceInfo>();

        var strings = MapStrings.From(doc);
        var playerIds = (info.Players ?? new List<PlayerData>()).Select(p => p.Id).ToList();
        return info.Forces.Select((f, index) => new ForceInfo(
            Index: index,
            Name: strings.Resolve(f.Name),
            PlayerIds: playerIds.Where(id => InMask(f.Players, id)).ToList(),
            Allied: f.Flags.HasFlag(ForceFlags.Allied),
            AlliedVictory: f.Flags.HasFlag(ForceFlags.AlliedVictory),
            SharedVision: f.Flags.HasFlag(ForceFlags.ShareVision),
            SharedUnitControl: f.Flags.HasFlag(ForceFlags.ShareUnitControl),
            SharedAdvUnitControl: f.Flags.HasFlag(ForceFlags.ShareAdvancedUnitControl))).ToList();
    }

    /// <summary>Moves a player into the force at <paramref name="forceIndex"/>: its bit
    /// is cleared from every other force's player mask (a player belongs to exactly one
    /// force, as in the World Editor) and set on the target's.</summary>
    public static PlayerForceEditResult SetPlayerForce(MapDocument doc, int playerId, int forceIndex)
    {
        if (doc.GetFile(MapInfoCommand.FileName)?.Model is not MapInfo info)
            return new(false, $"map has no parseable {MapInfoCommand.FileName}");
        if (info.Players is null || info.Players.All(p => p.Id != playerId))
            return new(false, $"map has no player with id {playerId}");
        if (info.Forces is null || forceIndex < 0 || forceIndex >= info.Forces.Count)
            return new(false,
                $"invalid force index {forceIndex} — map has {info.Forces?.Count ?? 0} force(s)");
        // Bitmask32 addresses bits 0..31.
        if (playerId is < 0 or > 31)
            return new(false, $"invalid player id {playerId} — must be 0..31");

        for (int i = 0; i < info.Forces.Count; i++)
        {
            var force = info.Forces[i];
            force.Players ??= new Bitmask32();
            force.Players[playerId] = i == forceIndex;
        }

        WriteBack(doc, info);
        return new(true, $"player {playerId} moved to force {forceIndex}");
    }

    /// <summary>Sets the force's alliance/sharing flags (any unknown flag bits the file
    /// carried are preserved).</summary>
    public static PlayerForceEditResult SetForceFlags(
        MapDocument doc,
        int forceIndex,
        bool allied,
        bool alliedVictory,
        bool sharedVision,
        bool sharedUnitControl,
        bool sharedAdvUnitControl)
    {
        if (doc.GetFile(MapInfoCommand.FileName)?.Model is not MapInfo info)
            return new(false, $"map has no parseable {MapInfoCommand.FileName}");
        if (info.Forces is null || forceIndex < 0 || forceIndex >= info.Forces.Count)
            return new(false,
                $"invalid force index {forceIndex} — map has {info.Forces?.Count ?? 0} force(s)");

        const ForceFlags managed = ForceFlags.Allied | ForceFlags.AlliedVictory
            | ForceFlags.ShareVision | ForceFlags.ShareUnitControl
            | ForceFlags.ShareAdvancedUnitControl;

        var force = info.Forces[forceIndex];
        var flags = force.Flags & ~managed;
        if (allied) flags |= ForceFlags.Allied;
        if (alliedVictory) flags |= ForceFlags.AlliedVictory;
        if (sharedVision) flags |= ForceFlags.ShareVision;
        if (sharedUnitControl) flags |= ForceFlags.ShareUnitControl;
        if (sharedAdvUnitControl) flags |= ForceFlags.ShareAdvancedUnitControl;
        force.Flags = flags;

        WriteBack(doc, info);
        return new(true,
            $"force {forceIndex} flags set (allied={allied}, alliedVictory={alliedVictory}, " +
            $"vision={sharedVision}, unitControl={sharedUnitControl}, advUnitControl={sharedAdvUnitControl})");
    }

    private static bool InMask(Bitmask32? mask, int playerId) =>
        mask is not null && playerId is >= 0 and <= 31 && mask[playerId];

    /// <summary>Re-serializes the mutated MapInfo through War3Net's writer and stores it
    /// as the entry's raw payload + parsed model — the MapInfoCommand.Set write path.</summary>
    private static void WriteBack(MapDocument doc, MapInfo info)
    {
        var entry = doc.AddOrReplaceRawFile(MapInfoCommand.FileName, MapInfoCommand.Serialize(info));
        entry.Model = info; // raw bytes win on Save; keep in-memory readers on the mutated model
    }
}
