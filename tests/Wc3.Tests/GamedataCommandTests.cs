// tests/Wc3.Tests/GamedataCommandTests.cs
using Wc3.Commands;

namespace Wc3.Tests;

/// <summary>
/// Hermetic tests for the read-only gamedata commands. Each test builds temporary schema folders with
/// GameSchemaCommand.Save and runs the command against them, so nothing depends on data/schemas or a
/// game install.
/// </summary>
public sealed class GamedataCommandTests : IDisposable
{
    private readonly string _root = Path.Combine(
        Path.GetTempPath(), "wc3ctl-gamedata-" + Guid.NewGuid().ToString("N"));

    public GamedataCommandTests() => Directory.CreateDirectory(_root);

    public void Dispose()
    {
        try { Directory.Delete(_root, recursive: true); } catch (IOException) { }
    }

    [Fact]
    public void History_reports_each_kind_and_the_first_and_last_build()
    {
        GamedataFixtures.Save(_root, "TFT-v1.30.1.10211", "units/unitmetadata.slk",
            new[] { GamedataFixtures.Field("Cool1", "int", "0", "100", true) },
            natives: new[] { "NativeA" });
        GamedataFixtures.Save(_root, "TFT-v1.31.1.12173", "units/unitmetadata.slk",
            new[] { GamedataFixtures.Field("Cool1", "real", "0", "100", true) },
            functions: new[] { "FuncB" });

        var field = GamedataHistoryCommand.Execute("Cool1", _root);
        Assert.True(field.Found);
        var onlyKind = Assert.Single(field.Kinds);
        Assert.Equal("field", onlyKind.Kind);
        Assert.Equal(2, onlyKind.Builds.Count);
        Assert.Equal("TFT-v1.30.1.10211", field.FirstBuild);
        Assert.Equal("TFT-v1.31.1.12173", field.LastBuild);
        Assert.Equal(2, field.BuildsSearched);
        Assert.Contains("units/unitmetadata.slk", field.ObjectFiles);

        var native = GamedataHistoryCommand.Execute("NativeA", _root);
        Assert.Equal("native", Assert.Single(native.Kinds).Kind);
        Assert.Equal("TFT-v1.30.1.10211", native.FirstBuild);

        var missing = GamedataHistoryCommand.Execute("NoSuchName", _root);
        Assert.False(missing.Found);
        Assert.Empty(missing.Kinds);
        Assert.Null(missing.FirstBuild);
    }

    [Fact]
    public void Changelog_reports_added_removed_retyped_and_bounds_changes()
    {
        GamedataFixtures.Save(_root, "TFT-v1.30.1.10211", "units/unitmetadata.slk", new[]
        {
            GamedataFixtures.Field("Keep", "int", "0", "100", true),
            GamedataFixtures.Field("Gone", "int", "0", "5", true),
            GamedataFixtures.Field("Retype", "int", "0", "10", true),
        });
        GamedataFixtures.Save(_root, "TFT-v1.31.1.12173", "units/unitmetadata.slk", new[]
        {
            GamedataFixtures.Field("Keep", "int", "0", "200", true),
            GamedataFixtures.Field("Retype", "real", "0", "10", true),
            GamedataFixtures.Field("New", "int", "0", "1", false),
        });

        var r = GamedataChangelogCommand.Execute("TFT-v1.30.1.10211", "TFT-v1.31.1.12173", _root);

        var group = Assert.Single(r.Groups);
        Assert.Equal("units/unitmetadata.slk", group.Slk);
        Assert.Contains(group.Changes, c => c.Code == "Keep" && c.Change == "bounds");
        Assert.Contains(group.Changes, c => c.Code == "Gone" && c.Change == "removed");
        Assert.Contains(group.Changes, c => c.Code == "Retype" && c.Change == "retyped");
        Assert.Contains(group.Changes, c => c.Code == "New" && c.Change == "added");
        Assert.Equal(1, r.Added);
        Assert.Equal(1, r.Removed);
        Assert.Equal(1, r.Retyped);
        Assert.Equal(1, r.BoundsChanged);
    }

    [Fact]
    public void Field_lookup_matches_display_name_and_honours_the_target()
    {
        GamedataFixtures.Save(_root, "TFT-v1.30.1.10211", "units/unitmetadata.slk", new[]
        {
            GamedataFixtures.Field("Cool1", "int", "0", "100", true, display: "Attack Range"),
            GamedataFixtures.Field("Cool2", "int", "0", "100", true, display: "Attack Speed"),
        });
        GamedataFixtures.Save(_root, "TFT-v1.31.1.12173", "units/unitmetadata.slk", new[]
        {
            GamedataFixtures.Field("Cool1", "int", "0", "100", true, display: "Defense"),
        });

        var all = GamedataFieldLookupCommand.Execute("attack", null, _root);
        Assert.Equal(2, all.Matches.Count);
        Assert.All(all.Matches, m => Assert.StartsWith("Attack", m.DisplayName));

        var targeted = GamedataFieldLookupCommand.Execute("attack", "TFT-v1.31.1.12173", _root);
        Assert.Empty(targeted.Matches);

        var none = GamedataFieldLookupCommand.Execute("no such text", null, _root);
        Assert.Empty(none.Matches);
    }

    [Fact]
    public void Bounds_reports_type_bounds_and_the_empty_flag()
    {
        GamedataFixtures.Save(_root, "TFT-v1.30.1.10211", "units/unitmetadata.slk", new[]
        {
            GamedataFixtures.Field("Cool1", "real", "1.5", "9.5", canEmpty: true),
        });
        GamedataFixtures.Save(_root, "TFT-v1.31.1.12173", "units/unitmetadata.slk", new[]
        {
            GamedataFixtures.Field("Cool1", "real", "0", "20", canEmpty: false),
        });

        var r = GamedataBoundsCommand.Execute("Cool1", null, _root);
        Assert.Equal(2, r.Entries.Count);
        var first = r.Entries[0];
        Assert.Equal("TFT-v1.30.1.10211", first.Build);
        Assert.Equal("real", first.Type);
        Assert.Equal("1.5", first.MinValue);
        Assert.Equal("9.5", first.MaxValue);
        Assert.True(first.CanBeEmpty);
        Assert.Equal("units/unitmetadata.slk", first.Slk);

        var missing = GamedataBoundsCommand.Execute("NoSuchCode", null, _root);
        Assert.Empty(missing.Entries);
    }

    [Fact]
    public void Coverage_names_the_sections_and_era_of_each_build()
    {
        GamedataFixtures.Save(_root, "TFT-v1.30.1.10211", "units/unitmetadata.slk",
            new[] { GamedataFixtures.Field("Cool1", "int", "0", "100", true) },
            natives: new[] { "NativeA" });
        GamedataFixtures.Save(_root, "3.0.0.24268", functions: new[] { "FuncB" });

        var r = GamedataCoverageCommand.Execute(_root);
        Assert.Equal(2, r.Rows.Count);

        var full = r.Rows.Single(x => x.Build == "TFT-v1.30.1.10211");
        Assert.Contains("objects", full.Sections);
        Assert.Contains("script", full.Sections);
        Assert.Equal("TFT", full.Era);
        Assert.Equal(1, full.Fields);
        Assert.Equal(1, full.Natives);

        var scriptOnly = r.Rows.Single(x => x.Build == "3.0.0.24268");
        Assert.DoesNotContain("objects", scriptOnly.Sections);
        Assert.Contains("script", scriptOnly.Sections);
        Assert.Equal("Reforged", scriptOnly.Era);
    }
}
