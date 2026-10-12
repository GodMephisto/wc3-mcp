// tests/Wc3.Tests/PlayerForceEditCommandTests.cs
// Hermetic tests for the five player/force field editors. Each builds a synthetic map
// (BlankMap, which carries a war3map.w3i and a JASS script with the SetPlayerColor calls),
// applies one edit, and reloads the saved bytes so the assertion is against what the file
// actually stores rather than the in-memory model.
using War3Net.Build.Common;
using War3Net.Build.Info;
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

public class PlayerForceEditCommandTests
{
    /// <summary>Blank map trimmed to exactly three players (0, 1, 2) and two forces
    /// (force 0 holds 0 and 1, force 1 holds 2). The script keeps its SetPlayerColor calls.</summary>
    private static MapDocument Fixture()
    {
        var doc = BlankMap.Create();
        var info = (MapInfo)doc.GetFile(MapInfoCommand.FileName)!.Model!;
        info.Players.Clear();
        info.Forces.Clear();

        for (var id = 0; id < 3; id++)
            info.Players.Add(new PlayerData
            {
                Id = id,
                Name = $"Player {id + 1}",
                Controller = id == 2 ? PlayerController.Computer : PlayerController.User,
                Race = PlayerRace.Human,
                Flags = 0,
                StartPosition = default,
                AllyLowPriorityFlags = new Bitmask32(),
                AllyHighPriorityFlags = new Bitmask32(),
                EnemyLowPriorityFlags = new Bitmask32(),
                EnemyHighPriorityFlags = new Bitmask32(),
            });

        info.Forces.Add(new ForceData
        {
            Name = "Force 1",
            Flags = ForceFlags.Allied | ForceFlags.AlliedVictory,
            Players = new Bitmask32(0b011),
        });
        info.Forces.Add(new ForceData
        {
            Name = "Force 2",
            Flags = 0,
            Players = new Bitmask32(0b100),
        });

        return doc;
    }

    // ---- player set-name --------------------------------------------------

    [Fact]
    public void SetName_ChangesOnlyTheTargetName_AndRoundTrips()
    {
        var doc = Fixture();

        var r = PlayerSetNameCommand.Execute(doc, 1, "Grunt");
        Assert.True(r.Ok, r.Message);
        Assert.True(doc.GetFile(MapInfoCommand.FileName)!.IsDirty);

        var players = PlayerForceCommand.GetPlayers(MapDocument.Load(doc.SaveToBytes()));
        Assert.Equal("Player 1", players[0].Name);
        Assert.Equal("Grunt", players[1].Name);
        Assert.Equal("Player 3", players[2].Name);
    }

    [Fact]
    public void SetName_OnMissingPlayer_IsRefusedAndLeavesTheFileClean()
    {
        var doc = Fixture();
        Assert.False(PlayerSetNameCommand.Execute(doc, 9, "X").Ok);
        Assert.False(doc.GetFile(MapInfoCommand.FileName)!.IsDirty);
    }

    // ---- player set-race --------------------------------------------------

    [Theory]
    [InlineData("human", "Human")]
    [InlineData("ORC", "Orc")]
    [InlineData("Undead", "Undead")]
    [InlineData("nightelf", "NightElf")]
    [InlineData("Night Elf", "NightElf")]
    [InlineData("random", "Selectable")]
    public void SetRace_AcceptsTheRaceNames_AndRoundTrips(string input, string expected)
    {
        var doc = Fixture();
        var r = PlayerSetRaceCommand.Execute(doc, 0, input);
        Assert.True(r.Ok, r.Message);

        var players = PlayerForceCommand.GetPlayers(MapDocument.Load(doc.SaveToBytes()));
        Assert.Equal(expected, players[0].Race);
    }

    [Fact]
    public void SetRace_UnknownRace_IsRefusedAndLeavesTheFileClean()
    {
        var doc = Fixture();
        Assert.False(PlayerSetRaceCommand.Execute(doc, 0, "dragon").Ok);
        Assert.False(doc.GetFile(MapInfoCommand.FileName)!.IsDirty);
    }

    // ---- player set-color (script based, see the command doc) -------------

    [Fact]
    public void SetColor_RewritesTheColorIdInTheScriptCall_AndRoundTrips()
    {
        var doc = Fixture();

        var r = PlayerSetColorCommand.Execute(doc, 1, "teal");
        Assert.True(r.Ok, r.Message);

        var (file, source) = ScriptCommand.Read(MapDocument.Load(doc.SaveToBytes()));
        Assert.EndsWith("war3map.j", file);
        Assert.Contains("SetPlayerColor( Player(1), ConvertPlayerColor(2) )", source);
        // Player 0's call is untouched.
        Assert.Contains("SetPlayerColor( Player(0), ConvertPlayerColor(0) )", source);
    }

    [Fact]
    public void SetColor_WorksOnALuaScript()
    {
        var doc = Fixture();
        var info = (MapInfo)doc.GetFile(MapInfoCommand.FileName)!.Model!;
        info.ScriptLanguage = ScriptLanguage.Lua;
        doc.AddOrReplaceRawFile("war3map.lua", System.Text.Encoding.UTF8.GetBytes(
            "function InitCustomPlayerSlots()\n" +
            "    SetPlayerColor(Player(0), ConvertPlayerColor(0))\n" +
            "end\n"));

        var r = PlayerSetColorCommand.Execute(doc, 0, "blue");
        Assert.True(r.Ok, r.Message);

        var (file, source) = ScriptCommand.Read(doc);
        Assert.EndsWith("war3map.lua", file);
        Assert.Contains("SetPlayerColor(Player(0), ConvertPlayerColor(1))", source);
    }

    [Fact]
    public void SetColor_UnknownColor_IsRefused()
    {
        var doc = Fixture();
        var r = PlayerSetColorCommand.Execute(doc, 0, "chartreuse");
        Assert.False(r.Ok);
        Assert.Contains("expected one of", r.Message);
    }

    [Fact]
    public void SetColor_WithoutAColorCall_IsRefused()
    {
        var doc = Fixture();
        ScriptCommand.Write(doc,
            "function InitCustomPlayerSlots takes nothing returns nothing\nendfunction\n");

        var r = PlayerSetColorCommand.Execute(doc, 0, "blue");
        Assert.False(r.Ok);
        Assert.Contains("no SetPlayerColor call", r.Message);
    }

    // ---- force rename -----------------------------------------------------

    [Fact]
    public void ForceRename_ChangesOnlyTheTargetForce_AndRoundTrips()
    {
        var doc = Fixture();

        var r = ForceRenameCommand.Execute(doc, 1, "The Horde");
        Assert.True(r.Ok, r.Message);
        Assert.True(doc.GetFile(MapInfoCommand.FileName)!.IsDirty);

        var forces = PlayerForceCommand.GetForces(MapDocument.Load(doc.SaveToBytes()));
        Assert.Equal("Force 1", forces[0].Name);
        Assert.Equal("The Horde", forces[1].Name);
    }

    [Fact]
    public void ForceRename_InvalidIndex_IsRefusedAndLeavesTheFileClean()
    {
        var doc = Fixture();
        Assert.False(ForceRenameCommand.Execute(doc, 5, "X").Ok);
        Assert.False(doc.GetFile(MapInfoCommand.FileName)!.IsDirty);
    }

    // ---- force set-players ------------------------------------------------

    [Fact]
    public void ForceSetPlayers_AddClearsThePlayerFromEveryOtherForce()
    {
        var doc = Fixture();

        var r = ForceSetPlayersCommand.Add(doc, 1, 0); // move player 0 into force 1
        Assert.True(r.Ok, r.Message);

        var forces = PlayerForceCommand.GetForces(MapDocument.Load(doc.SaveToBytes()));
        Assert.Equal(new[] { 1 }, forces[0].PlayerIds);      // player 0 left force 0
        Assert.Equal(new[] { 0, 2 }, forces[1].PlayerIds);   // and joined force 1
    }

    [Fact]
    public void ForceSetPlayers_RemoveClearsOnlyTheTargetForce()
    {
        var doc = Fixture();

        var r = ForceSetPlayersCommand.Remove(doc, 0, 0);
        Assert.True(r.Ok, r.Message);

        var forces = PlayerForceCommand.GetForces(MapDocument.Load(doc.SaveToBytes()));
        Assert.Equal(new[] { 1 }, forces[0].PlayerIds);
        Assert.Equal(new[] { 2 }, forces[1].PlayerIds); // untouched
    }

    [Fact]
    public void ForceSetPlayers_RejectsMissingPlayerAndForce_LeavingTheFileClean()
    {
        var doc = Fixture();
        Assert.False(ForceSetPlayersCommand.Add(doc, 0, 9).Ok);
        Assert.False(ForceSetPlayersCommand.Add(doc, 5, 0).Ok);
        Assert.False(ForceSetPlayersCommand.Remove(doc, 5, 0).Ok);
        Assert.False(doc.GetFile(MapInfoCommand.FileName)!.IsDirty);
    }
}
