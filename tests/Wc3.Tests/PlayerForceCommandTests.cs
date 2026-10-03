// tests/Wc3.Tests/PlayerForceCommandTests.cs
using War3Net.Build.Common;
using War3Net.Build.Info;
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

public class PlayerForceCommandTests
{
    private static readonly string CorpusMapPath =
        TestCorpus.Map(@"GGGA_V0.02a.w3x");

    /// <summary>Blank map whose parsed MapInfo is seeded with three players and two
    /// forces (players 0+1 in force 0, player 2 in force 1). BlankMap's own single
    /// player and force are cleared first, so the fixture controls every slot.</summary>
    private static MapDocument Fixture()
    {
        var doc = BlankMap.Create();
        var info = (MapInfo)doc.GetFile(MapInfoCommand.FileName)!.Model!;
        info.Players.Clear();
        info.Forces.Clear();

        for (int id = 0; id < 3; id++)
        {
            info.Players.Add(new PlayerData
            {
                Id = id,
                Name = $"Player {id + 1}",
                Controller = id == 2 ? PlayerController.Computer : PlayerController.User,
                Race = PlayerRace.Human,
                Flags = id == 0 ? PlayerFlags.FixedStartPosition : 0,
                StartPosition = default,
                // War3Net's writer dereferences these masks unconditionally.
                AllyLowPriorityFlags = new Bitmask32(),
                AllyHighPriorityFlags = new Bitmask32(),
                EnemyLowPriorityFlags = new Bitmask32(),
                EnemyHighPriorityFlags = new Bitmask32(),
            });
        }

        info.Forces.Add(new ForceData
        {
            Name = "Force 1",
            Flags = ForceFlags.Allied | ForceFlags.AlliedVictory,
            Players = new Bitmask32(0b011), // players 0 and 1
        });
        info.Forces.Add(new ForceData
        {
            Name = "Force 2",
            Flags = 0,
            Players = new Bitmask32(0b100), // player 2
        });

        return doc;
    }

    [Fact]
    public void GetPlayers_ReadsIdNameRaceControllerColorAndFlags()
    {
        var doc = Fixture();
        var players = PlayerForceCommand.GetPlayers(doc);

        Assert.Equal(3, players.Count);
        Assert.Equal(new[] { 0, 1, 2 }, players.Select(p => p.Id));
        Assert.Equal("Player 1", players[0].Name);
        Assert.Equal("Human", players[0].Race);
        Assert.Equal("User", players[0].Controller);
        Assert.Equal("Computer", players[2].Controller);
        Assert.True(players[0].FixedStartPosition);
        Assert.False(players[1].FixedStartPosition);
        Assert.Equal(PlayerColors.Color(0), players[0].Color); // red
        Assert.Equal(PlayerColors.Color(2), players[2].Color); // teal
    }

    [Fact]
    public void GetForces_ReadsMembershipAndFlags()
    {
        var doc = Fixture();
        var forces = PlayerForceCommand.GetForces(doc);

        Assert.Equal(2, forces.Count);

        Assert.Equal(0, forces[0].Index);
        Assert.Equal("Force 1", forces[0].Name);
        Assert.Equal(new[] { 0, 1 }, forces[0].PlayerIds);
        Assert.True(forces[0].Allied);
        Assert.True(forces[0].AlliedVictory);
        Assert.False(forces[0].SharedVision);

        Assert.Equal(1, forces[1].Index);
        Assert.Equal(new[] { 2 }, forces[1].PlayerIds);
        Assert.False(forces[1].Allied);
    }

    [Fact]
    public void GetPlayersAndForces_AreEmptyWhenInfoHasNone()
    {
        // A map whose info lists no players or forces must read as empty, not throw.
        var doc = BlankMap.Create();
        var info = (MapInfo)doc.GetFile(MapInfoCommand.FileName)!.Model!;
        info.Players.Clear();
        info.Forces.Clear();
        Assert.Empty(PlayerForceCommand.GetPlayers(doc));
        Assert.Empty(PlayerForceCommand.GetForces(doc));
    }

    [Fact]
    public void BlankMap_has_the_one_player_and_force_its_script_sets_up()
    {
        // The blank script runs SetPlayerController(Player(0), MAP_CONTROL_USER) and puts
        // player 0 in team 0, so the info file must say the same or the editors have nothing.
        var doc = BlankMap.Create();

        var player = Assert.Single(PlayerForceCommand.GetPlayers(doc));
        Assert.Equal(0, player.Id);
        Assert.Equal("User", player.Controller);
        var force = Assert.Single(PlayerForceCommand.GetForces(doc));
        Assert.Equal(new[] { 0 }, force.PlayerIds);

        // And it survives a save and reload.
        var reloaded = MapDocument.Load(doc.SaveToBytes());
        Assert.Single(PlayerForceCommand.GetPlayers(reloaded));
        Assert.Single(PlayerForceCommand.GetForces(reloaded));
    }

    [Fact]
    public void SetPlayerForce_MovesThePlayer_MarksDirty_AndSurvivesRoundTrip()
    {
        var doc = Fixture();

        var result = PlayerForceCommand.SetPlayerForce(doc, playerId: 1, forceIndex: 1);
        Assert.True(result.Ok, result.Message);
        Assert.True(doc.GetFile(MapInfoCommand.FileName)!.IsDirty);

        var forces = PlayerForceCommand.GetForces(doc);
        Assert.Equal(new[] { 0 }, forces[0].PlayerIds);      // player 1 left force 0
        Assert.Equal(new[] { 1, 2 }, forces[1].PlayerIds);   // ...and joined force 1

        // The whole w3i re-serializes; a real save + reload must reparse the move.
        var reloaded = MapDocument.Load(doc.SaveToBytes());
        var reloadedForces = PlayerForceCommand.GetForces(reloaded);
        Assert.Equal(new[] { 0 }, reloadedForces[0].PlayerIds);
        Assert.Equal(new[] { 1, 2 }, reloadedForces[1].PlayerIds);
        Assert.Equal(3, PlayerForceCommand.GetPlayers(reloaded).Count);
    }

    [Fact]
    public void SetForceFlags_UpdatesOnlyTheTargetForce_AndSurvivesRoundTrip()
    {
        var doc = Fixture();

        var result = PlayerForceCommand.SetForceFlags(doc, forceIndex: 1,
            allied: true, alliedVictory: false, sharedVision: true,
            sharedUnitControl: true, sharedAdvUnitControl: false);
        Assert.True(result.Ok, result.Message);

        var forces = PlayerForceCommand.GetForces(MapDocument.Load(doc.SaveToBytes()));
        Assert.True(forces[1].Allied);
        Assert.False(forces[1].AlliedVictory);
        Assert.True(forces[1].SharedVision);
        Assert.True(forces[1].SharedUnitControl);
        Assert.False(forces[1].SharedAdvUnitControl);

        // Force 0 keeps its original flags.
        Assert.True(forces[0].Allied);
        Assert.True(forces[0].AlliedVictory);
        Assert.False(forces[0].SharedVision);
    }

    [Fact]
    public void Editors_RejectUnknownPlayersAndForceIndices()
    {
        var doc = Fixture();

        Assert.False(PlayerForceCommand.SetPlayerForce(doc, playerId: 7, forceIndex: 0).Ok);
        Assert.False(PlayerForceCommand.SetPlayerForce(doc, playerId: 0, forceIndex: 2).Ok);
        Assert.False(PlayerForceCommand.SetPlayerForce(doc, playerId: 0, forceIndex: -1).Ok);
        Assert.False(PlayerForceCommand.SetForceFlags(doc, 5, true, true, true, true, true).Ok);

        // Nothing was marked dirty by the rejected edits.
        Assert.False(doc.GetFile(MapInfoCommand.FileName)!.IsDirty);
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void CorpusMap_ExposesPlayersAndForces()
    {
        if (!File.Exists(CorpusMapPath)) return;
        var doc = MapDocument.Load(CorpusMapPath);

        var players = PlayerForceCommand.GetPlayers(doc);
        var forces = PlayerForceCommand.GetForces(doc);
        Assert.NotEmpty(players);
        Assert.NotEmpty(forces);

        // Every player id maps to a color and a display name without throwing.
        foreach (var p in players)
        {
            Assert.Equal(PlayerColors.Color(p.Id), p.Color);
            Assert.False(string.IsNullOrEmpty(PlayerColors.DisplayName(p.Id)));
        }
    }

    // ---- PlayerColors (fully hermetic) -------------------------------------

    [Fact]
    public void PlayerColors_MatchTheKnownReforgedTable()
    {
        Assert.Equal(((byte)0xFF, (byte)0x03, (byte)0x03), PlayerColors.Color(0));  // red
        Assert.Equal(((byte)0x00, (byte)0x42, (byte)0xFF), PlayerColors.Color(1));  // blue
        Assert.Equal(((byte)0x1C, (byte)0xE6, (byte)0xB9), PlayerColors.Color(2));  // teal
        Assert.Equal(((byte)0x95, (byte)0x96, (byte)0x97), PlayerColors.Color(8));  // gray
        Assert.Equal(((byte)0xA4, (byte)0x6F, (byte)0x33), PlayerColors.Color(23)); // peanut
    }

    [Fact]
    public void PlayerColors_All28SlotsAreDistinct()
    {
        var colors = Enumerable.Range(0, PlayerColors.SlotCount).Select(PlayerColors.Color).ToList();
        Assert.Equal(PlayerColors.SlotCount, colors.Distinct().Count());
    }

    [Fact]
    public void PlayerColors_DisplayNames()
    {
        Assert.Equal("Player 1 (Red)", PlayerColors.DisplayName(0));
        Assert.Equal("Player 2 (Blue)", PlayerColors.DisplayName(1));
        Assert.Equal("Player 24 (Peanut)", PlayerColors.DisplayName(23));
        Assert.Equal("Neutral Hostile", PlayerColors.DisplayName(24));
        Assert.Equal("Neutral Victim", PlayerColors.DisplayName(25));
        Assert.Equal("Neutral Extra", PlayerColors.DisplayName(26));
        Assert.Equal("Neutral Passive", PlayerColors.DisplayName(27));
        Assert.Equal("Player 29", PlayerColors.DisplayName(28)); // out-of-range fallback
    }

    [Fact]
    public void PlayerColors_OutOfRangeIdsFallBackToGray()
    {
        Assert.Equal(PlayerColors.Color(8), PlayerColors.Color(-1));
        Assert.Equal(PlayerColors.Color(8), PlayerColors.Color(28));
    }
}
