// tests/Wc3.Tests/MapInfoCommandTests.cs
using System.Drawing;
using System.Numerics;
using System.Text;
using War3Net.Build.Common;
using War3Net.Build.Extensions;
using War3Net.Build.Info;
using War3Net.Build.Script;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Hermetic coverage of MapInfoCommand: the war3map.w3i is constructed
/// programmatically (a complete v25 MapInfo), serialized with the same War3Net
/// writer the command uses, and wrapped in an in-memory synthetic MPQ — no
/// corpus map or game install required.
/// </summary>
public class MapInfoCommandTests
{
    /// <summary>Minimal but complete v25 MapInfo: every field the v25 writer emits is
    /// set, so serialization never trips over a null.</summary>
    private static MapInfo BuildInfo()
    {
        var info = new MapInfo(MapInfoFormatVersion.v25)
        {
            MapVersion = 1,
            EditorVersion = (EditorVersion)6072,
            MapName = "Synthetic Test Map",
            MapAuthor = "wc3ctl",
            MapDescription = "A synthetic map for hermetic tests.",
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
        info.Players.Add(new PlayerData
        {
            Id = 0,
            Controller = PlayerController.User,
            Race = PlayerRace.Human,
            Flags = 0,
            Name = "Player 1",
            StartPosition = new Vector2(0f, 0f),
            AllyLowPriorityFlags = new Bitmask32(0),
            AllyHighPriorityFlags = new Bitmask32(0),
            EnemyLowPriorityFlags = new Bitmask32(0),
            EnemyHighPriorityFlags = new Bitmask32(0),
        });
        info.Forces.Add(new ForceData { Flags = 0, Players = new Bitmask32(-1), Name = "Force 1" });
        return info;
    }

    private static MapDocument LoadSynthetic(MapInfo info, IDictionary<string, byte[]>? extraFiles = null)
    {
        var files = new Dictionary<string, byte[]> { ["war3map.w3i"] = MapInfoCommand.Serialize(info) };
        foreach (var (name, bytes) in extraFiles ?? new Dictionary<string, byte[]>())
            files[name] = bytes;
        return MapDocument.Load(SyntheticMap.Build(files));
    }

    private static byte[] Wts(params (uint Key, string Value)[] entries)
    {
        var wts = new TriggerStrings();
        foreach (var (key, value) in entries)
            wts.Strings.Add(new TriggerString { Key = key, Value = value });
        using var ms = new MemoryStream();
        using (var sw = new StreamWriter(ms, Encoding.UTF8, leaveOpen: true)) sw.WriteTriggerStrings(wts);
        return ms.ToArray();
    }

    [Fact]
    public void Read_resolves_trigstr_names_against_the_wts()
    {
        // The World Editor stores the map title as a TRIGSTR_ reference into war3map.wts.
        // Read must resolve it so the panel shows real text ("Gutsy Geoid Game"), not the raw
        // key - the reason Map Info looked erased. Unresolved keys just fall through.
        var info = BuildInfo();
        info.MapName = "TRIGSTR_100";
        info.MapAuthor = "TRIGSTR_101";
        var doc = LoadSynthetic(info, new Dictionary<string, byte[]>
        {
            ["war3map.wts"] = Wts((100u, "Gutsy Geoid Game"), (101u, "GodMephisto")),
        });

        var fields = MapInfoCommand.Read(doc);
        Assert.Equal("Gutsy Geoid Game", fields.MapName);
        Assert.Equal("GodMephisto", fields.Author);
    }

    [Fact]
    public void Read_returns_the_w3i_fields()
    {
        var fields = MapInfoCommand.Read(LoadSynthetic(BuildInfo()));

        Assert.Equal("Synthetic Test Map", fields.MapName);
        Assert.Equal("wc3ctl", fields.Author);
        Assert.Equal("A synthetic map for hermetic tests.", fields.Description);
        Assert.Equal("1-2", fields.RecommendedPlayers);
        Assert.Equal(52, fields.PlayableWidth);
        Assert.Equal(52, fields.PlayableHeight);
        Assert.Equal(1, fields.Players);

        // The extended w3i surface: environment, fog, screens and flag toggles.
        Assert.StartsWith("BL (", fields.CameraBounds);
        Assert.Equal("LordaeronSummer", fields.Tileset);
        Assert.Equal("Unspecified", fields.LightEnvironment);
        Assert.Equal("None", fields.GlobalWeather);
        Assert.Equal("", fields.SoundEnvironment);
        Assert.Equal("255,255,255,255", fields.WaterTintColor);
        Assert.Equal("Linear", fields.FogStyle);
        Assert.Equal(3000f, fields.FogStartZ);
        Assert.Equal(5000f, fields.FogEndZ);
        Assert.Equal(0.5f, fields.FogDensity);
        Assert.Equal("0,0,0,255", fields.FogColor);
        Assert.Equal(-1, fields.LoadingScreenIndex);
        Assert.Equal("", fields.LoadingScreenPath);
        Assert.Equal("", fields.PrologueTitle);
        Assert.True(fields.MeleeMap);
        Assert.False(fields.UseCustomForces);
        Assert.False(fields.HasTerrainFog);
        Assert.False(fields.HasWaterTint);
    }

    [Fact]
    public void Set_map_name_round_trips_through_save_and_reload()
    {
        var doc = LoadSynthetic(BuildInfo());
        MapInfoCommand.Set(doc, "MapName", "Renamed Map");

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        var fields = MapInfoCommand.Read(reloaded);

        Assert.Equal("Renamed Map", fields.MapName);
        // Every unrelated field survives the rename untouched.
        Assert.Equal("wc3ctl", fields.Author);
        Assert.Equal("A synthetic map for hermetic tests.", fields.Description);
        Assert.Equal("1-2", fields.RecommendedPlayers);
        Assert.Equal(52, fields.PlayableWidth);
        Assert.Equal(52, fields.PlayableHeight);
        Assert.Equal(1, fields.Players);
    }

    [Fact]
    public void Set_is_visible_in_memory_without_a_save()
    {
        var doc = LoadSynthetic(BuildInfo());

        var returned = MapInfoCommand.Set(doc, "Author", "someone else");

        Assert.Equal("someone else", returned.Author);
        // The entry's Model is restored after the raw write-back, so a fresh Read
        // (and InfoCommand) sees the mutation immediately.
        Assert.Equal("someone else", MapInfoCommand.Read(doc).Author);
        Assert.Equal("Synthetic Test Map", MapInfoCommand.Read(doc).MapName);
    }

    [Fact]
    public void Field_names_match_case_insensitively()
    {
        var doc = LoadSynthetic(BuildInfo());
        MapInfoCommand.Set(doc, "recommendedplayers", "2-4");
        Assert.Equal("2-4", MapInfoCommand.Read(doc).RecommendedPlayers);
    }

    /// <summary>The fidelity invariant behind "untouched fields preserved": parsing a
    /// w3i through MapDocument and re-serializing it UNCHANGED reproduces the original
    /// bytes exactly, so a field edit can only ever affect that field's encoding.</summary>
    [Fact]
    public void Unchanged_reserialize_is_byte_identical()
    {
        var original = MapInfoCommand.Serialize(BuildInfo());
        var doc = MapDocument.Load(SyntheticMap.Build(
            new Dictionary<string, byte[]> { ["war3map.w3i"] = original }));

        var reparsed = Assert.IsType<MapInfo>(doc.GetFile("war3map.w3i")!.Model);

        Assert.Equal(original, MapInfoCommand.Serialize(reparsed));
    }

    [Fact]
    public void Set_leaves_other_archive_files_untouched()
    {
        var script = Encoding.UTF8.GetBytes(
            "function main takes nothing returns nothing\nendfunction\n");
        var doc = LoadSynthetic(BuildInfo(),
            new Dictionary<string, byte[]> { ["war3map.j"] = script });

        MapInfoCommand.Set(doc, "Description", "changed description");
        var reloaded = MapDocument.Load(doc.SaveToBytes());

        Assert.Equal(script, reloaded.GetFile("war3map.j")!.RawBytes);
        Assert.Equal("changed description", MapInfoCommand.Read(reloaded).Description);
    }

    [Fact]
    public void Set_rejects_unknown_and_read_only_fields()
    {
        var doc = LoadSynthetic(BuildInfo());
        Assert.Throws<ArgumentException>(() => MapInfoCommand.Set(doc, "PlayableWidth", "10"));
        Assert.Throws<ArgumentException>(() => MapInfoCommand.Set(doc, "CameraBounds", "0,0"));
        Assert.Throws<ArgumentException>(() => MapInfoCommand.Set(doc, "NoSuchField", "x"));
        // Malformed values for the typed fields are rejected the same way.
        Assert.Throws<ArgumentException>(() => MapInfoCommand.Set(doc, "FogColor", "purple"));
        Assert.Throws<ArgumentException>(() => MapInfoCommand.Set(doc, "FogStartZ", "far"));
        Assert.Throws<ArgumentException>(() => MapInfoCommand.Set(doc, "MeleeMap", "maybe"));
        Assert.Throws<ArgumentException>(() => MapInfoCommand.Set(doc, "Tileset", "??"));
        // Rejection happens before any mutation or write-back.
        Assert.Equal("Synthetic Test Map", MapInfoCommand.Read(doc).MapName);
    }

    [Fact]
    public void Set_flag_round_trips_and_leaves_every_other_field_unchanged()
    {
        var doc = LoadSynthetic(BuildInfo());
        var before = MapInfoCommand.Read(doc);
        Assert.False(before.UseCustomForces);

        MapInfoCommand.Set(doc, "UseCustomForces", "true");

        var saved = MapDocument.Load(doc.SaveToBytes());
        var after = MapInfoCommand.Read(saved);
        Assert.True(after.UseCustomForces);
        // Record equality pins every other exposed field as unchanged.
        Assert.Equal(before with { UseCustomForces = true }, after);
        // Unexposed flag bits (HasMapPropertiesMenuBeenOpened) survive the toggle too.
        var reparsed = Assert.IsType<MapInfo>(saved.GetFile("war3map.w3i")!.Model);
        Assert.True(reparsed.MapFlags.HasFlag(MapFlags.HasMapPropertiesMenuBeenOpened));
        Assert.True(reparsed.MapFlags.HasFlag(MapFlags.MeleeMap));
    }

    [Fact]
    public void Set_fog_color_round_trips_through_save_and_reload()
    {
        var doc = LoadSynthetic(BuildInfo());
        var before = MapInfoCommand.Read(doc);

        MapInfoCommand.Set(doc, "FogColor", "10,20,30,40");

        var after = MapInfoCommand.Read(MapDocument.Load(doc.SaveToBytes()));
        Assert.Equal("10,20,30,40", after.FogColor);
        Assert.Equal(before with { FogColor = "10,20,30,40" }, after);
    }

    [Fact]
    public void Set_loading_screen_title_round_trips_through_save_and_reload()
    {
        var doc = LoadSynthetic(BuildInfo());
        var before = MapInfoCommand.Read(doc);

        MapInfoCommand.Set(doc, "LoadingScreenTitle", "TRIGSTR_001");

        var after = MapInfoCommand.Read(MapDocument.Load(doc.SaveToBytes()));
        Assert.Equal("TRIGSTR_001", after.LoadingScreenTitle);
        Assert.Equal(before with { LoadingScreenTitle = "TRIGSTR_001" }, after);
    }

    /// <summary>Enum-backed fields accept the War3Net name, the WE tileset letter,
    /// and the raw 4-letter weather code interchangeably.</summary>
    [Fact]
    public void Enum_fields_accept_names_letters_and_weather_codes()
    {
        var doc = LoadSynthetic(BuildInfo());

        MapInfoCommand.Set(doc, "Tileset", "N");
        MapInfoCommand.Set(doc, "GlobalWeather", "RAlr");
        MapInfoCommand.Set(doc, "FogStyle", "Exponential1");

        var after = MapInfoCommand.Read(MapDocument.Load(doc.SaveToBytes()));
        Assert.Equal("Northrend", after.Tileset);
        Assert.Equal("AshenvaleLightRain", after.GlobalWeather);
        Assert.Equal("Exponential1", after.FogStyle);
    }

    [Fact]
    public void Missing_or_unparseable_w3i_throws()
    {
        var noW3i = MapDocument.Load(SyntheticMap.Build(
            new Dictionary<string, byte[]> { ["readme.txt"] = new byte[] { 9 } }));
        Assert.Throws<InvalidOperationException>(() => MapInfoCommand.Read(noW3i));
        Assert.Throws<InvalidOperationException>(() => MapInfoCommand.Set(noW3i, "MapName", "x"));

        // Garbage w3i: load degrades to raw (Model == null) and the command refuses.
        var badW3i = MapDocument.Load(SyntheticMap.Build(
            new Dictionary<string, byte[]> { ["war3map.w3i"] = new byte[] { 1, 2, 3 } }));
        Assert.Throws<InvalidOperationException>(() => MapInfoCommand.Read(badW3i));
    }
}
