using System.Drawing;
using System.Numerics;
using War3Net.Build.Common;
using War3Net.Build.Info;
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

public class ValidateCommandTests
{
    // Non-empty placeholder bytes: present (so not "missing") but they won't parse
    // into real models — good enough to exercise presence/absence checks.
    private static readonly byte[] Blob = { 1, 2, 3, 4 };

    [Fact]
    public void Missing_required_files_are_errors()
    {
        // Only a script; no war3map.w3i / war3map.w3e.
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = Blob,
        }));

        var r = ValidateCommand.Execute(doc);

        Assert.False(r.Valid);
        Assert.Contains(r.Issues, i =>
            i.Severity == DiagnosticSeverity.Error && i.Category == "missing-file" && i.FileName == "war3map.w3i");
        Assert.Contains(r.Issues, i =>
            i.Severity == DiagnosticSeverity.Error && i.Category == "missing-file" && i.FileName == "war3map.w3e");
        // A script IS present, so the "no script" error must NOT fire.
        Assert.DoesNotContain(r.Issues, i => i.Message.Contains("no script"));
        Assert.Equal(r.Errors, r.Issues.Count(i => i.Severity == DiagnosticSeverity.Error));
    }

    [Fact]
    public void Missing_script_is_an_error()
    {
        // Has the required data files but no war3map.j / war3map.lua.
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3i"] = Blob,
            ["war3map.w3e"] = Blob,
        }));

        var r = ValidateCommand.Execute(doc);

        Assert.False(r.Valid);
        Assert.Contains(r.Issues, i =>
            i.Severity == DiagnosticSeverity.Error && i.Message.Contains("no script"));
    }

    [Fact]
    public void Present_required_files_yield_no_errors()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3i"] = Blob,
            ["war3map.w3e"] = Blob,
            ["war3map.j"] = Blob,
        }));

        var r = ValidateCommand.Execute(doc);

        // Warnings (e.g. loader parse notes) are allowed; there must be no errors.
        Assert.Equal(0, r.Errors);
        Assert.True(r.Valid, string.Join("; ", r.Issues.Select(i => i.Message)));
    }

    [Fact]
    public void Empty_named_file_is_a_warning()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3i"] = Blob,
            ["war3map.w3e"] = Blob,
            ["war3map.j"] = Blob,
            ["empty.txt"] = System.Array.Empty<byte>(),
        }));

        // Only assert the empty-file rule when the 0-byte entry actually survived the
        // MPQ round-trip; otherwise the archive layer dropped it and there is nothing
        // for the validator to flag.
        if (doc.GetFile("empty.txt") is not null)
        {
            var r = ValidateCommand.Execute(doc);
            Assert.Contains(r.Issues, i =>
                i.Severity == DiagnosticSeverity.Warning && i.Category == "empty-file" && i.FileName == "empty.txt");
        }
    }

    [Fact]
    public void Loader_diagnostics_are_surfaced()
    {
        // An unnamed archive entry makes the loader emit a Warning diagnostic.
        var doc = MapDocument.Load(SyntheticMap.Build(
            new Dictionary<string, byte[]>
            {
                ["war3map.w3i"] = Blob,
                ["war3map.w3e"] = Blob,
                ["war3map.j"] = Blob,
            },
            new[] { new byte[] { 7, 7, 7 } }));

        var r = ValidateCommand.Execute(doc);

        Assert.Equal(doc.Diagnostics.Count, r.Issues.Count(i => i.Category == "loader"));
        Assert.Contains(r.Issues, i => i.Category == "loader");
    }

    [Fact]
    public void Counts_are_consistent_with_issues()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["mystery.bin"] = Blob, // no required files at all
        }));

        var r = ValidateCommand.Execute(doc);

        Assert.Equal(r.Errors, r.Issues.Count(i => i.Severity == DiagnosticSeverity.Error));
        Assert.Equal(r.Warnings, r.Issues.Count(i => i.Severity == DiagnosticSeverity.Warning));
        Assert.Equal(r.Valid, r.Errors == 0);
    }

    [Fact]
    public void Empty_critical_file_is_an_error()
    {
        // A present-but-0-byte required file means the map cannot run: Error, not Warning.
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3i"] = Blob,
            ["war3map.w3e"] = System.Array.Empty<byte>(),
            ["war3map.j"] = Blob,
        }));

        // Only assert when the 0-byte entry actually survived the MPQ round-trip.
        if (doc.GetFile("war3map.w3e") is { } e && e.RawBytes.Length == 0)
        {
            var r = ValidateCommand.Execute(doc);
            Assert.False(r.Valid);
            Assert.Contains(r.Issues, i =>
                i.Severity == DiagnosticSeverity.Error && i.Category == "empty-file"
                && i.FileName == "war3map.w3e");
        }
    }

    [Fact]
    public void Blank_map_name_is_a_warning()
    {
        var info = BuildInfo();
        info.MapName = "";

        var r = ValidateCommand.Execute(LoadWithInfo(info));

        Assert.Contains(r.Issues, i =>
            i.Severity == DiagnosticSeverity.Warning && i.Category == "map-info"
            && i.Message.Contains("map name is blank"));
    }

    [Fact]
    public void Duplicate_player_ids_are_a_warning()
    {
        var info = BuildInfo();
        info.Players.Add(NewPlayer(0, "Player 1 (dup)")); // second entry re-using slot id 0

        var r = ValidateCommand.Execute(LoadWithInfo(info));

        Assert.Contains(r.Issues, i =>
            i.Severity == DiagnosticSeverity.Warning && i.Category == "map-info"
            && i.Message.Contains("duplicate player id 0"));
    }

    [Fact]
    public void Well_formed_map_info_yields_no_map_info_warnings()
    {
        var r = ValidateCommand.Execute(LoadWithInfo(BuildInfo()));

        Assert.DoesNotContain(r.Issues, i => i.Category == "map-info");
    }

    private static MapDocument LoadWithInfo(MapInfo info) =>
        MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3i"] = MapInfoCommand.Serialize(info),
            ["war3map.w3e"] = Blob,
            ["war3map.j"] = Blob,
        }));

    private static PlayerData NewPlayer(int id, string name) => new()
    {
        Id = id,
        Controller = PlayerController.User,
        Race = PlayerRace.Human,
        Flags = 0,
        Name = name,
        StartPosition = new Vector2(0f, 0f),
        AllyLowPriorityFlags = new Bitmask32(0),
        AllyHighPriorityFlags = new Bitmask32(0),
        EnemyLowPriorityFlags = new Bitmask32(0),
        EnemyHighPriorityFlags = new Bitmask32(0),
    };

    /// <summary>Complete v25 MapInfo (every field the writer emits is set) so
    /// serialization never trips over a null — mirrors MapInfoCommandTests.</summary>
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
        info.Players.Add(NewPlayer(0, "Player 1"));
        info.Forces.Add(new ForceData { Flags = 0, Players = new Bitmask32(-1), Name = "Force 1" });
        return info;
    }
}
