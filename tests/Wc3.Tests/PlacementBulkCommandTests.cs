// tests/Wc3.Tests/PlacementBulkCommandTests.cs
using War3Net.Build.Widget;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// Bulk placement, place grid and place scatter. Both build their copies through the
/// single-placement plumbing, so these assert the copy count, the exact grid geometry,
/// the scatter's determinism and bounds, and that every rejection writes nothing.
/// </summary>
public class PlacementBulkCommandTests
{
    private static MapUnits Units(MapDocument doc) =>
        (MapUnits)doc.GetFile(PlacementCommand.UnitsFile)!.Model!;

    private static MapDoodads Doodads(MapDocument doc) =>
        (MapDoodads)doc.GetFile(PlacementCommand.DoodadsFile)!.Model!;

    // ---- place grid --------------------------------------------------------

    [Fact]
    public void Grid_places_rows_times_cols_units_at_the_expected_positions()
    {
        var doc = BlankMap.Create();

        var r = PlaceGridCommand.Execute(doc, "hfoo", rows: 2, cols: 3, spacing: 128f);

        Assert.True(r.Ok, r.Message);
        Assert.Equal(6, r.Placed);
        Assert.Equal(new[] { 0, 1, 2, 3, 4, 5 }, r.CreationNumbers);

        // Centred on the origin, x in {-128, 0, 128}, y in {-64, 64}.
        var units = Units(doc).Units;
        Assert.Equal(6, units.Count);
        var positions = units.Select(u => (u.Position.X, u.Position.Y)).ToHashSet();
        foreach (var y in new[] { -64f, 64f })
            foreach (var x in new[] { -128f, 0f, 128f })
                Assert.Contains((x, y), positions);
    }

    [Fact]
    public void Grid_places_doodads_with_kind_doodad()
    {
        var doc = BlankMap.Create();

        var r = PlaceGridCommand.Execute(doc, "LTlt", rows: 1, cols: 2, spacing: 64f, kind: "doodad");

        Assert.True(r.Ok, r.Message);
        Assert.Equal(2, r.Placed);
        Assert.Null(doc.GetFile(PlacementCommand.UnitsFile)); // doodads go to war3map.doo
        var doodads = Doodads(doc).Doodads;
        Assert.Equal(2, doodads.Count);
        Assert.All(doodads, d => Assert.Equal("LTlt", d.TypeId.ToRawcode()));
    }

    [Fact]
    public void Grid_survives_a_save_and_reload()
    {
        var doc = BlankMap.Create();
        Assert.True(PlaceGridCommand.Execute(doc, "hfoo", 2, 2, 128f).Ok);

        var reloaded = MapDocument.Load(doc.SaveToBytes());

        Assert.Equal(4, Units(reloaded).Units.Count);
    }

    [Theory]
    [InlineData("abc", 1, 1, 128f, 0, "unit")]     // rawcode too short
    [InlineData("hfoo", 0, 1, 128f, 0, "unit")]    // rows < 1
    [InlineData("hfoo", 1, 0, 128f, 0, "unit")]    // cols < 1
    [InlineData("hfoo", 1, 1, 0f, 0, "unit")]      // spacing <= 0
    [InlineData("hfoo", 1, 1, -8f, 0, "unit")]     // spacing < 0
    [InlineData("hfoo", 1, 1, 128f, -1, "unit")]   // bad owner
    [InlineData("hfoo", 1, 1, 128f, 0, "prop")]    // unknown kind
    public void Grid_rejects_bad_input_and_writes_nothing(
        string rawcode, int rows, int cols, float spacing, int owner, string kind)
    {
        var doc = BlankMap.Create();

        var r = PlaceGridCommand.Execute(doc, rawcode, rows, cols, spacing, owner, kind);

        Assert.False(r.Ok);
        Assert.Null(doc.GetFile(PlacementCommand.UnitsFile));
        Assert.Null(doc.GetFile(PlacementCommand.DoodadsFile));
    }

    // ---- place scatter -----------------------------------------------------

    [Fact]
    public void Scatter_is_deterministic_for_a_seed()
    {
        var a = BlankMap.Create();
        var b = BlankMap.Create();

        Assert.True(PlaceScatterCommand.Execute(a, "hfoo", count: 8, seed: 1234).Ok);
        Assert.True(PlaceScatterCommand.Execute(b, "hfoo", count: 8, seed: 1234).Ok);

        var pa = Units(a).Units.Select(u => (u.Position.X, u.Position.Y)).ToList();
        var pb = Units(b).Units.Select(u => (u.Position.X, u.Position.Y)).ToList();
        Assert.Equal(pa, pb);
    }

    [Fact]
    public void Scatter_different_seeds_move_the_layout()
    {
        var a = BlankMap.Create();
        var b = BlankMap.Create();

        Assert.True(PlaceScatterCommand.Execute(a, "hfoo", count: 6, seed: 1).Ok);
        Assert.True(PlaceScatterCommand.Execute(b, "hfoo", count: 6, seed: 2).Ok);

        var pa = Units(a).Units.Select(u => (u.Position.X, u.Position.Y)).ToList();
        var pb = Units(b).Units.Select(u => (u.Position.X, u.Position.Y)).ToList();
        Assert.NotEqual(pa, pb);
    }

    [Fact]
    public void Scatter_over_the_whole_map_stays_inside_the_playable_area()
    {
        var doc = BlankMap.Create();

        var r = PlaceScatterCommand.Execute(doc, "hfoo", count: 12, seed: 7);

        Assert.True(r.Ok, r.Message);
        Assert.Equal(12, r.Placed);
        // Blank map is 32 tiles a side, so half extent is 32 * 64 = 2048 world units.
        Assert.All(Units(doc).Units, u =>
        {
            Assert.InRange(u.Position.X, -2048f, 2048f);
            Assert.InRange(u.Position.Y, -2048f, 2048f);
        });
    }

    [Fact]
    public void Scatter_inside_a_region_stays_inside_its_bounds()
    {
        var doc = BlankMap.Create();
        Assert.True(PlacementCommand.PlaceRegion(doc, "arena", -300f, -200f, 300f, 200f).Ok);

        var r = PlaceScatterCommand.Execute(doc, "hfoo", count: 10, region: "arena", seed: 99);

        Assert.True(r.Ok, r.Message);
        Assert.All(Units(doc).Units, u =>
        {
            Assert.InRange(u.Position.X, -300f, 300f);
            Assert.InRange(u.Position.Y, -200f, 200f);
        });
    }

    [Fact]
    public void Scatter_refuses_an_unknown_region()
    {
        var doc = BlankMap.Create();

        var r = PlaceScatterCommand.Execute(doc, "hfoo", count: 3, region: "nowhere");

        Assert.False(r.Ok);
        Assert.Contains("nowhere", r.Message);
        Assert.Null(doc.GetFile(PlacementCommand.UnitsFile));
    }

    [Theory]
    [InlineData("abc", 1)]    // rawcode too short
    [InlineData("hfoo", 0)]   // count < 1
    [InlineData("hfoo", -5)]  // count < 0
    public void Scatter_rejects_bad_input_and_writes_nothing(string rawcode, int count)
    {
        var doc = BlankMap.Create();

        var r = PlaceScatterCommand.Execute(doc, rawcode, count);

        Assert.False(r.Ok);
        Assert.Null(doc.GetFile(PlacementCommand.UnitsFile));
    }

    [Fact]
    public void Scatter_survives_a_save_and_reload()
    {
        var doc = BlankMap.Create();
        Assert.True(PlaceScatterCommand.Execute(doc, "hfoo", count: 5, seed: 42).Ok);

        var reloaded = MapDocument.Load(doc.SaveToBytes());

        Assert.Equal(5, Units(reloaded).Units.Count);
    }
}
