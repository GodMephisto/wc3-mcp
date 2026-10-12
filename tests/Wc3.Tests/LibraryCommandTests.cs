// tests/Wc3.Tests/LibraryCommandTests.cs
// Hermetic tests for the folder-sweep library commands. Each builds a temporary folder of
// synthetic maps through SyntheticMap.Build, so no corpus map or game install is needed. The
// pure folds (scan totals, audit ranking and per-check totals, compare) are tested over
// in-memory rows as well, so the mechanics are pinned without loading a map.
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

public sealed class LibraryCommandTests : IDisposable
{
    private readonly string _dir = Path.Combine(
        Path.GetTempPath(), "wc3ctl-library", Guid.NewGuid().ToString("N"));

    public LibraryCommandTests() => Directory.CreateDirectory(_dir);

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch (IOException) { }
    }

    private static readonly byte[] Blob = { 1, 2, 3, 4 };
    private const string Script =
        "function config takes nothing returns nothing\nendfunction\n"
        + "function main takes nothing returns nothing\nendfunction\n";

    /// <summary>A loaded-map w3i, optionally at v28 so it declares the game version it targets.</summary>
    private static byte[] Info(bool withBuild)
    {
        var info = new MapInfo(withBuild ? MapInfoFormatVersion.v28 : MapInfoFormatVersion.v25)
        {
            MapVersion = 1,
            EditorVersion = (EditorVersion)6072,
            MapName = "Synthetic Library Map",
            MapAuthor = "wc3ctl",
            MapDescription = "A synthetic map for hermetic library tests.",
            RecommendedPlayers = "1-2",
            CameraBounds = new Quadrilateral(-2048f, 2048f, 2048f, -2048f),
            CameraBoundsComplements = new RectangleMargins(6, 6, 4, 8),
            PlayableMapAreaWidth = 52,
            PlayableMapAreaHeight = 52,
            MapFlags = MapFlags.MeleeMap,
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
        if (withBuild) info.GameVersion = new Version(1, 31, 1, 12173);
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
        return MapInfoCommand.Serialize(info);
    }

    /// <summary>A map that loads and validates, at v28 so it declares a target build, or v25.</summary>
    private static byte[] GoodMap(bool withBuild = false) =>
        SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3i"] = Info(withBuild),
            ["war3map.w3e"] = Blob,
            ["war3map.j"] = Encoding.UTF8.GetBytes(Script),
        });

    private string WriteMap(string folder, string name, byte[] bytes)
    {
        Directory.CreateDirectory(folder);
        string path = Path.Combine(folder, name);
        File.WriteAllBytes(path, bytes);
        return path;
    }

    // ---- scan ------------------------------------------------------------

    [Fact]
    public void Scan_reports_each_map_and_the_totals()
    {
        var folder = Path.Combine(_dir, "scan");
        WriteMap(folder, "good.w3x", GoodMap(withBuild: true));
        WriteMap(folder, "bad.w3x", Blob);

        var r = LibraryScanCommand.Execute(folder);

        Assert.Equal(2, r.Maps.Count);
        Assert.Equal(1, r.Loaded);
        Assert.Equal(1, r.Failed);
        Assert.Equal(r.Maps.Sum(m => m.Bytes), r.TotalBytes);

        var good = r.Maps.Single(m => m.Map == "good.w3x");
        Assert.True(good.Loads);
        Assert.Equal("1.31.1.12173", good.TargetBuild);
        Assert.True(good.Entries > 0);
        Assert.True(good.Bytes > 0);
        Assert.Null(good.Error);

        var bad = r.Maps.Single(m => m.Map == "bad.w3x");
        Assert.False(bad.Loads);
        Assert.Equal(0, bad.Entries);
        Assert.NotNull(bad.Error);
    }

    [Fact]
    public void Scan_without_a_declared_build_reads_the_target_as_unknown()
    {
        var folder = Path.Combine(_dir, "scan-nobuild");
        WriteMap(folder, "classic.w3x", GoodMap());

        var r = LibraryScanCommand.Execute(folder);

        Assert.Null(Assert.Single(r.Maps).TargetBuild);
    }

    [Fact]
    public void Scan_summarize_folds_in_memory_rows()
    {
        var r = LibraryScanCommand.Summarize(new[]
        {
            new LibraryScanMap("a.w3x", 100, "1.31", true, 6, 1, null),
            new LibraryScanMap("b.w3x", 40, null, false, 0, 0, "no magic"),
        });

        Assert.Equal(1, r.Loaded);
        Assert.Equal(1, r.Failed);
        Assert.Equal(140, r.TotalBytes);
    }

    // ---- audit -----------------------------------------------------------

    [Fact]
    public void Audit_sweeps_a_folder_and_counts_a_failed_map()
    {
        var folder = Path.Combine(_dir, "audit");
        WriteMap(folder, "good.w3x", GoodMap());
        WriteMap(folder, "bad.w3x", Blob);

        var r = LibraryAuditCommand.Execute(folder);

        Assert.Equal(2, r.Maps.Count);
        Assert.Contains(r.Maps, m => m.Map == "bad.w3x" && !m.Ok && m.Error is not null);
        // A synthetic map carries no object data, so the behavioural audit finds nothing.
        Assert.Equal(0, r.TotalFindings);
        Assert.Empty(r.Checks);
    }

    [Fact]
    public void Audit_summarize_groups_checks_and_ranks_maps_by_severity()
    {
        var clean = new LibraryAuditMap("clean.w3x", true, 0, 0,
            Array.Empty<AuditIssue>(), null);
        var warn = new LibraryAuditMap("warn.w3x", true, 0, 2, new[]
        {
            new AuditIssue(DiagnosticSeverity.Warning, "level-gap", "A000", null, "w1"),
            new AuditIssue(DiagnosticSeverity.Warning, "level-gap", "A001", null, "w2"),
        }, null);
        var error = new LibraryAuditMap("error.w3x", false, 1, 0, new[]
        {
            new AuditIssue(DiagnosticSeverity.Error, "requirement", "A002", null, "e1"),
        }, null);

        // Deliberately out of rank order in the input.
        var r = LibraryAuditCommand.Summarize(new[] { clean, warn, error });

        // Errors first, then the map with two warnings, then the clean one.
        Assert.Equal(new[] { "error.w3x", "warn.w3x", "clean.w3x" }, r.Maps.Select(m => m.Map));
        Assert.Equal(3, r.TotalFindings);
        Assert.Equal(2, r.MapsWithFindings);
        Assert.Equal(2, r.Checks.Single(c => c.Check == "level-gap").Count);
        Assert.Equal(1, r.Checks.Single(c => c.Check == "requirement").Count);
        // Ordered by count descending, so level-gap reads before requirement.
        Assert.Equal("level-gap", r.Checks[0].Check);
    }

    // ---- validate --------------------------------------------------------

    [Fact]
    public void Validate_lists_the_maps_that_fail_with_a_reason()
    {
        var folder = Path.Combine(_dir, "validate");
        WriteMap(folder, "good.w3x", GoodMap());
        WriteMap(folder, "broken.w3x", SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = Encoding.UTF8.GetBytes(Script),
        }));
        WriteMap(folder, "unreadable.w3x", Blob);

        var r = LibraryValidateCommand.Execute(folder);

        Assert.Equal(3, r.Maps.Count);
        var good = r.Maps.Single(m => m.Map == "good.w3x");
        Assert.True(good.Valid, "good map issues: "
            + string.Join(" | ", good.Issues.Select(i => i.Severity + " " + i.Category + " " + i.Message)));
        Assert.Equal(1, r.Valid);
        Assert.Equal(2, r.Invalid);

        var broken = r.Maps.Single(m => m.Map == "broken.w3x");
        Assert.False(broken.Valid);
        Assert.True(broken.Errors > 0);
        Assert.NotEmpty(broken.Issues);

        var unreadable = r.Maps.Single(m => m.Map == "unreadable.w3x");
        Assert.False(unreadable.Valid);
        Assert.NotNull(unreadable.Error);
    }

    // ---- compare ---------------------------------------------------------

    [Fact]
    public void Compare_lists_only_in_changed_and_same_by_name_and_hash()
    {
        var a = Path.Combine(_dir, "cmp-a");
        var b = Path.Combine(_dir, "cmp-b");
        var shared = GoodMap();
        WriteMap(a, "same.w3x", shared);
        WriteMap(b, "same.w3x", shared);
        WriteMap(a, "changed.w3x", SyntheticMap.Build(new Dictionary<string, byte[]> { ["war3map.j"] = Encoding.UTF8.GetBytes("AAA") }));
        WriteMap(b, "changed.w3x", SyntheticMap.Build(new Dictionary<string, byte[]> { ["war3map.j"] = Encoding.UTF8.GetBytes("BBB") }));
        WriteMap(a, "onlya.w3x", SyntheticMap.Build(new Dictionary<string, byte[]> { ["war3map.j"] = Encoding.UTF8.GetBytes("A") }));
        WriteMap(b, "onlyb.w3x", SyntheticMap.Build(new Dictionary<string, byte[]> { ["war3map.j"] = Encoding.UTF8.GetBytes("B") }));

        var r = LibraryCompareCommand.Execute(a, b);

        Assert.Equal(new[] { "onlya.w3x" }, r.OnlyInA);
        Assert.Equal(new[] { "onlyb.w3x" }, r.OnlyInB);
        Assert.Equal(new[] { "changed.w3x" }, r.Changed);
        Assert.Equal(1, r.Same);
        Assert.Empty(r.AmbiguousNames);
    }

    [Fact]
    public void Compare_reports_a_name_that_is_not_unique_inside_one_folder()
    {
        var a = Path.Combine(_dir, "dup-a");
        var b = Path.Combine(_dir, "dup-b");
        WriteMap(Path.Combine(a, "one"), "dupe.w3x", SyntheticMap.Build(new Dictionary<string, byte[]> { ["war3map.j"] = Encoding.UTF8.GetBytes("1") }));
        WriteMap(Path.Combine(a, "two"), "dupe.w3x", SyntheticMap.Build(new Dictionary<string, byte[]> { ["war3map.j"] = Encoding.UTF8.GetBytes("2") }));
        WriteMap(b, "dupe.w3x", SyntheticMap.Build(new Dictionary<string, byte[]> { ["war3map.j"] = Encoding.UTF8.GetBytes("1") }));

        var r = LibraryCompareCommand.Execute(a, b);

        Assert.Equal(new[] { "dupe.w3x" }, r.AmbiguousNames);
        Assert.Empty(r.OnlyInA);
        Assert.Empty(r.Changed);
    }

    // ---- report ----------------------------------------------------------

    [Fact]
    public void Report_writes_a_markdown_file_with_the_summary_rows_and_findings()
    {
        var folder = Path.Combine(_dir, "report");
        WriteMap(folder, "good.w3x", GoodMap(withBuild: true));
        WriteMap(folder, "bad.w3x", Blob);
        string outPath = Path.Combine(_dir, "out", "report.md");

        var r = LibraryReportCommand.Execute(folder, outPath);

        Assert.True(File.Exists(outPath));
        Assert.Equal(outPath, r.Path);
        Assert.Equal(2, r.Maps);
        Assert.Equal(1, r.Loaded);
        Assert.Equal(1, r.Failed);
        Assert.Equal(1, r.Invalid);

        string md = File.ReadAllText(outPath);
        Assert.Contains("# Map library report", md);
        Assert.Contains("Streaming is true.", md);
        Assert.Contains("## Maps", md);
        Assert.Contains("| good.w3x |", md);
        Assert.Contains("1.31.1.12173", md);
        Assert.Contains("## Findings by check", md);
        Assert.Contains("## Validation failures", md);
        Assert.Contains("bad.w3x", md);
    }

    [Fact]
    public void Report_build_renders_the_aggregated_findings()
    {
        var scans = new List<LibraryScanMap>
        {
            new("a.w3x", 10, "1.31", true, 6, 0, null),
        };
        var audits = new List<LibraryAuditMap>
        {
            new("a.w3x", false, 1, 0, new[]
            {
                new AuditIssue(DiagnosticSeverity.Error, "requirement", "A000", null, "locked"),
            }, null),
        };
        var validations = new List<LibraryValidateMap>
        {
            new("a.w3x", true, 0, 0, Array.Empty<ValidationIssue>(), null),
        };

        string md = LibraryReportCommand.Build(
            "some-folder", scans, audits, validations,
            LibraryScanCommand.Summarize(scans), LibraryAuditCommand.Summarize(audits));

        Assert.Contains("findings 1", md);
        Assert.Contains("| requirement | 1 |", md);
        Assert.Contains("No map failed validation.", md);
    }
}
