using System.Drawing;
using System.Numerics;
using War3Net.Build.Common;
using War3Net.Build.Info;
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

public class GeneratedMapRepairCommandTests
{
    private const string GeneratedScript = """
        function InitCustomPlayerSlots takes nothing returns nothing
            call SetPlayerStartLocation( Player(0), 0 )
            call SetPlayerColor( Player(0), ConvertPlayerColor(0) )
            call SetPlayerRacePreference( Player(0), RACE_PREF_HUMAN )
            call SetPlayerRaceSelectable( Player(0), true )
            call SetPlayerController( Player(0), MAP_CONTROL_USER )
            call SetPlayerStartLocation( Player(1), 1 )
            call SetPlayerColor( Player(1), ConvertPlayerColor(1) )
            call SetPlayerRacePreference( Player(1), RACE_PREF_HUMAN )
            call SetPlayerRaceSelectable( Player(1), true )
            call SetPlayerController( Player(1), MAP_CONTROL_USER )
        endfunction

        function InitCustomTeams takes nothing returns nothing
            // Force: Force 1
            call SetPlayerTeam( Player(0), 0 )
            call SetPlayerTeam( Player(1), 0 )
        endfunction

        function wc3ctl_WirePlacedHeroSpells takes nothing returns nothing
            local group g = CreateGroup()
            local unit hu
            local boolean ok
            call TriggerRegisterPlayerUnitEvent(gg_trg_GearCastCheck, Player(0), EVENT_PLAYER_UNIT_SPELL_EFFECT, null)
            call TriggerRegisterPlayerUnitEvent(gg_trg_OrbOfAvariceStaff, Player(0), EVENT_PLAYER_UNIT_SPELL_EFFECT, null)
            call GroupEnumUnitsOfPlayer(g, Player(0), null)
            loop
                set hu = FirstOfGroup(g)
                exitwhen hu == null
                call GroupRemoveUnit(g, hu)
                if GetUnitTypeId(hu) == 'H0DA' then
                    set ok = WS_FinalizeWorkingSourceHero(Player(0), hu, 'H0DA')
                endif
                if GetUnitTypeId(hu) == 'H000' then
                    set ok = WS_FinalizeWorkingSourceHero(Player(0), hu, 'H000')
                endif
                if GetUnitTypeId(hu) == 'H001' then
                    set ok = WS_FinalizeWorkingSourceHero(Player(0), hu, 'H001')
                endif
            endloop
            call DestroyGroup(g)
            set g = null
            set hu = null
            call DestroyTimer(GetExpiredTimer())
        endfunction

        function CreateAllUnits takes nothing returns nothing
            local unit u
            set u = CreateUnit(Player(0), 'H0DA', -400.0, 0.0, 0.0)
            call SetHeroLevel(u, 50, false)
            if IsUnitType(u, UNIT_TYPE_HERO) and udg_Player[1 + GetPlayerId(GetOwningPlayer(u))] == null then
                set udg_Player[1 + GetPlayerId(GetOwningPlayer(u))] = u
            endif
            if IsUnitType(u, UNIT_TYPE_HERO) and Hero[GetPlayerId(GetOwningPlayer(u))] == null then
                set Hero[GetPlayerId(GetOwningPlayer(u))] = u
            endif
            if IsUnitType(u, UNIT_TYPE_HERO) and udg_Hero2[GetPlayerId(GetOwningPlayer(u))] == null then
                set udg_Hero2[GetPlayerId(GetOwningPlayer(u))] = u
            endif
            set u = CreateUnit(Player(0), 'H000', 400.0, 0.0, 0.0)
            if IsUnitType(u, UNIT_TYPE_HERO) and udg_Hero2[GetPlayerId(GetOwningPlayer(u))] == null then
                set udg_Hero2[GetPlayerId(GetOwningPlayer(u))] = u
            endif
            set u = CreateUnit(Player(0), 'H001', 400.0, 300.0, 0.0)
            if IsUnitType(u, UNIT_TYPE_HERO) and udg_Hero2[GetPlayerId(GetOwningPlayer(u))] == null then
                set udg_Hero2[GetPlayerId(GetOwningPlayer(u))] = u
            endif
            call TimerStart(CreateTimer(), 0., false, function wc3ctl_WirePlacedHeroSpells)
            set u = null
        endfunction

        function config takes nothing returns nothing
            call SetMapName( "Shiki Arena" )
            call SetMapDescription( "Created with wc3ctl." )
            call SetPlayers( 2 )
            call SetTeams( 1 )
            call SetGamePlacement( MAP_PLACEMENT_USE_MAP_SETTINGS )

            call DefineStartLocation( 0, -128.0, 0.0 )
            call DefineStartLocation( 1, 128.0, 0.0 )

            // Player setup
            call InitCustomPlayerSlots(  )
            call InitCustomTeams(  )
        endfunction
        """;

    [Fact]
    public void RepairGeneratedHeroes_RewritesOwnersScriptSlotsAndMapInfo()
    {
        var doc = LoadGeneratedMap();

        var result = GeneratedMapRepairCommand.RepairGeneratedHeroes(doc);

        Assert.True(result.Ok, result.Message);
        Assert.Equal(2, result.PlayerSlotsBefore);
        Assert.Equal(3, result.PlayerSlotsAfter);
        Assert.True(result.ScriptUpdated);
        Assert.True(result.MapInfoUpdated);
        Assert.True(result.HeaderUpdated);
        Assert.Equal(new[] { 0, 1, 2 }, result.Heroes.Select(h => h.OwnerId));
        Assert.Equal(new[] { 0, 0, 0 }, result.Heroes.Select(h => h.OriginalOwnerId));

        var script = FileEditCommand.ReadText(doc, "war3map.j");
        Assert.Contains("call SetPlayers( 3 )", script);
        Assert.Contains("call SetPlayerStartLocation( Player(2), 2 )", script);
        Assert.Contains("call SetPlayerController( Player(2), MAP_CONTROL_COMPUTER )", script);
        Assert.Contains("call DefineStartLocation( 2, 400.0, 300.0 )", script);
        Assert.Contains("CreateUnit(Player(1), 'H000', 400.0, 0.0, 0.0)", script);
        Assert.Contains("CreateUnit(Player(2), 'H001', 400.0, 300.0, 0.0)", script);
        Assert.Contains("udg_Hero2[1 + GetPlayerId(GetOwningPlayer(u))]", script);
        Assert.Contains("set ok = WS_FinalizeWorkingSourceHero(GetOwningPlayer(hu), hu, 'H001')", script);
        Assert.Contains("local integer i = 0", script);
        Assert.Contains("call TriggerRegisterPlayerUnitEvent(gg_trg_GearCastCheck, Player(i), EVENT_PLAYER_UNIT_SPELL_EFFECT, null)", script);

        var info = (MapInfo)doc.GetFile(MapInfoCommand.FileName)!.Model!;
        Assert.Equal(3, info.Players.Max(p => p.Id) + 1);
        var player2 = Assert.Single(info.Players.Where(p => p.Id == 2));
        Assert.Equal(PlayerController.Computer, player2.Controller);
        Assert.Equal(new Vector2(400f, 300f), player2.StartPosition);

        Assert.Equal((uint)3, BitConverter.ToUInt32(doc.PreArchiveData, 13));

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        var reloadedScript = FileEditCommand.ReadText(reloaded, "war3map.j");
        Assert.Contains("call SetPlayers( 3 )", reloadedScript);
        var reloadedInfo = (MapInfo)reloaded.GetFile(MapInfoCommand.FileName)!.Model!;
        Assert.Equal(3, reloadedInfo.Players.Max(p => p.Id) + 1);
        Assert.Equal((uint)3, BitConverter.ToUInt32(reloaded.PreArchiveData, 13));
    }

    [Fact]
    public void RepairGeneratedHeroes_RejectsMapsWithoutTheGeneratedHelper()
    {
        var doc = BlankMap.Create();

        var result = GeneratedMapRepairCommand.RepairGeneratedHeroes(doc);

        Assert.False(result.Ok);
        Assert.Contains("generated wc3ctl preplaced-hero helper block", result.Message);
    }

    private static MapDocument LoadGeneratedMap() =>
        MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3i"] = MapInfoCommand.Serialize(BuildInfo()),
            ["war3map.j"] = System.Text.Encoding.UTF8.GetBytes(GeneratedScript),
        }));

    private static MapInfo BuildInfo()
    {
        var info = new MapInfo(MapInfoFormatVersion.v25)
        {
            MapVersion = 1,
            EditorVersion = (EditorVersion)6072,
            MapName = "Shiki Arena",
            MapAuthor = "wc3ctl",
            MapDescription = "Created with wc3ctl.",
            RecommendedPlayers = "1-2",
            CameraBounds = new Quadrilateral(-2048f, 2048f, 2048f, -2048f),
            CameraBoundsComplements = new RectangleMargins(6, 6, 4, 8),
            PlayableMapAreaWidth = 52,
            PlayableMapAreaHeight = 52,
            MapFlags = MapFlags.MeleeMap | MapFlags.HasMapPropertiesMenuBeenOpened,
            Tileset = Tileset.LordaeronSummer,
            LoadingScreenBackgroundNumber = -1,
            LoadingScreenPath = "",
            LoadingScreenText = "",
            LoadingScreenTitle = "",
            LoadingScreenSubtitle = "",
            GameDataSet = GameDataSet.Default,
            PrologueScreenPath = "",
            PrologueScreenText = "",
            PrologueScreenTitle = "",
            PrologueScreenSubtitle = "",
            FogStyle = FogStyle.Linear,
            FogStartZ = 3000f,
            FogEndZ = 5000f,
            FogDensity = 0.5f,
            FogColor = Color.FromArgb(255, 0, 0, 0),
            GlobalWeather = WeatherType.None,
            SoundEnvironment = "",
            LightEnvironment = Tileset.Unspecified,
            WaterTintingColor = Color.FromArgb(255, 255, 255, 255),
        };

        info.Players.Add(NewPlayer(0, "Player 1", PlayerController.User));
        info.Players.Add(NewPlayer(1, "Player 2", PlayerController.User));
        info.Forces.Add(new ForceData { Flags = 0, Players = new Bitmask32(-1), Name = "Force 1" });
        return info;
    }

    private static PlayerData NewPlayer(int id, string name, PlayerController controller) => new()
    {
        Id = id,
        Controller = controller,
        Race = PlayerRace.Human,
        Flags = 0,
        Name = name,
        StartPosition = new Vector2(0f, 0f),
        AllyLowPriorityFlags = new Bitmask32(0),
        AllyHighPriorityFlags = new Bitmask32(0),
        EnemyLowPriorityFlags = new Bitmask32(0),
        EnemyHighPriorityFlags = new Bitmask32(0),
    };
}
