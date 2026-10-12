// tests/Wc3.Tests/PlacedRefsCommandTests.cs
using War3Net.Build.Widget;
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// Placed-object census and reference hygiene, placed stats, placed missing-refs and
/// placed fix-refs. All three share the same classifier, so these pin the grouping, the
/// definition test (map object data defines a rawcode, a stranger does not) and the
/// refusal to rewrite a healthy map.
/// </summary>
public class PlacedRefsCommandTests
{
    private static MapUnits Units(MapDocument doc) =>
        (MapUnits)doc.GetFile(PlacementCommand.UnitsFile)!.Model!;

    private static MapDoodads Doodads(MapDocument doc) =>
        (MapDoodads)doc.GetFile(PlacementCommand.DoodadsFile)!.Model!;

    /// <summary>A map with two hfoo units, one LTlt doodad and one start location.</summary>
    private static MapDocument Populated()
    {
        var doc = BlankMap.Create();
        Assert.True(PlacementCommand.PlaceUnit(doc, "hfoo", 0, 0f, 0f).Ok);
        Assert.True(PlacementCommand.PlaceUnit(doc, "hfoo", 0, 64f, 0f).Ok);
        Assert.True(PlacementCommand.PlaceDoodad(doc, "LTlt", 128f, 128f).Ok);
        Assert.True(PlacementCommand.PlaceStartLocation(doc, 0, -128f, -128f).Ok);
        return doc;
    }

    // ---- placed stats ------------------------------------------------------

    [Fact]
    public void Stats_counts_by_rawcode_and_kind()
    {
        var r = PlacedStatsCommand.Execute(Populated());

        Assert.True(r.Ok, r.Message);
        Assert.Equal(3, r.UnitTotal);   // two hfoo plus the sloc
        Assert.Equal(1, r.DoodadTotal);

        var hfoo = r.Types.Single(t => t.Kind == "unit" && t.Rawcode == "hfoo");
        Assert.Equal(2, hfoo.Count);

        var sloc = r.Types.Single(t => t.Kind == "unit" && t.Rawcode == PlacementCommand.StartLocationRawcode);
        Assert.Equal(1, sloc.Count);
        Assert.Equal("Start Location", sloc.Name); // resolved from the map's own pseudo-type

        var tree = r.Types.Single(t => t.Kind == "doodad" && t.Rawcode == "LTlt");
        Assert.Equal(1, tree.Count);
    }

    [Fact]
    public void Stats_on_an_empty_map_reports_no_placements()
    {
        var r = PlacedStatsCommand.Execute(BlankMap.Create());

        Assert.True(r.Ok);
        Assert.Equal(0, r.UnitTotal);
        Assert.Equal(0, r.DoodadTotal);
        Assert.Empty(r.Types);
    }

    // ---- placed missing-refs ----------------------------------------------

    [Fact]
    public void Missing_refs_flags_only_rawcodes_the_map_cannot_define()
    {
        var doc = BlankMap.Create();
        // A custom unit the map defines, so it must NOT be flagged.
        var custom = ObjectNewCommand.Execute(doc, ObjectKind.Unit, "hfoo");
        Assert.True(custom.Ok, custom.Message);
        Assert.True(PlacementCommand.PlaceUnit(doc, custom.NewRawcode!, 0, 0f, 0f).Ok);
        // A stranger the map does not define.
        Assert.True(PlacementCommand.PlaceUnit(doc, "zzzz", 0, 64f, 0f).Ok);
        // A start location is defined by the game itself.
        Assert.True(PlacementCommand.PlaceStartLocation(doc, 0, 128f, 0f).Ok);

        var r = PlacedMissingRefsCommand.Execute(doc, gameDir: null);

        Assert.True(r.Ok, r.Message);
        var miss = Assert.Single(r.Missing);
        Assert.Equal("zzzz", miss.Rawcode);
        Assert.Equal("unit", miss.Kind);
    }

    [Fact]
    public void Missing_refs_flags_a_doodad_type_the_map_cannot_define()
    {
        var doc = BlankMap.Create();
        Assert.True(PlacementCommand.PlaceDoodad(doc, "zzzz", 0f, 0f).Ok);

        var r = PlacedMissingRefsCommand.Execute(doc, gameDir: null);

        var miss = Assert.Single(r.Missing);
        Assert.Equal("doodad", miss.Kind);
        Assert.Equal("zzzz", miss.Rawcode);
    }

    // ---- placed fix-refs ---------------------------------------------------

    [Fact]
    public void Fix_refs_refuses_a_healthy_map()
    {
        var doc = BlankMap.Create();
        // Resolve the placement from the map's own object data, so this needs no base game install.
        var unit = ObjectNewCommand.Execute(doc, ObjectKind.Unit, "hfoo");
        Assert.True(unit.Ok, unit.Message);
        Assert.True(PlacementCommand.PlaceUnit(doc, unit.NewRawcode!, 0, 0f, 0f).Ok);

        var r = PlacedFixRefsCommand.Execute(doc);

        Assert.False(r.Ok);
        Assert.Contains("nothing to fix", r.Message);
        Assert.Empty(r.Changes);
    }

    [Fact]
    public void Fix_refs_drops_a_missing_unit_and_keeps_the_rest()
    {
        var doc = BlankMap.Create();
        // Define one unit in the map's object data so only the stranger is missing.
        var custom = ObjectNewCommand.Execute(doc, ObjectKind.Unit, "hfoo");
        Assert.True(custom.Ok, custom.Message);
        var good = PlacementCommand.PlaceUnit(doc, custom.NewRawcode!, 0, 0f, 0f);
        var bad = PlacementCommand.PlaceUnit(doc, "zzzz", 0, 64f, 0f);

        var r = PlacedFixRefsCommand.Execute(doc);

        Assert.True(r.Ok, r.Message);
        Assert.Equal(1, r.Changed);
        var change = Assert.Single(r.Changes);
        Assert.Equal("dropped", change.Action);
        Assert.Equal("zzzz", change.Rawcode);

        var ids = Units(doc).Units.Select(u => u.CreationNumber).ToList();
        Assert.Contains(good.CreationNumber, ids);
        Assert.DoesNotContain(bad.CreationNumber, ids);
    }

    [Fact]
    public void Fix_refs_rewrites_a_missing_unit_to_the_replacement()
    {
        var doc = BlankMap.Create();
        var bad = PlacementCommand.PlaceUnit(doc, "zzzz", 0, 32f, 32f);

        var r = PlacedFixRefsCommand.Execute(doc, gameDir: null, toRawcode: "hfoo");

        Assert.True(r.Ok, r.Message);
        Assert.Equal(1, r.Changed);
        Assert.Equal("rewrote", r.Changes[0].Action);
        Assert.Equal("hfoo", r.Changes[0].ToRawcode);
        Assert.Equal("hfoo", UnitInstanceCommand.Get(doc, bad.CreationNumber)!.TypeRawcode);
    }

    [Fact]
    public void Fix_refs_drops_a_missing_doodad()
    {
        var doc = BlankMap.Create();
        Assert.True(PlacementCommand.PlaceDoodad(doc, "zzzz", 0f, 0f).Ok);

        var r = PlacedFixRefsCommand.Execute(doc);

        Assert.True(r.Ok, r.Message);
        Assert.Equal(1, r.Changed);
        Assert.Equal("doodad", r.Changes[0].Kind);
        Assert.Empty(Doodads(doc).Doodads);
    }

    [Fact]
    public void Fix_refs_rejects_a_malformed_replacement_rawcode()
    {
        var doc = BlankMap.Create();
        Assert.True(PlacementCommand.PlaceUnit(doc, "zzzz", 0, 0f, 0f).Ok);

        var r = PlacedFixRefsCommand.Execute(doc, gameDir: null, toRawcode: "toolong");

        Assert.False(r.Ok);
        Assert.Contains("4 characters", r.Message);
    }

    [Fact]
    public void Fix_refs_change_survives_a_save_and_reload()
    {
        var doc = BlankMap.Create();
        Assert.True(PlacementCommand.PlaceUnit(doc, "zzzz", 0, 0f, 0f).Ok);

        Assert.True(PlacedFixRefsCommand.Execute(doc).Ok);
        var reloaded = MapDocument.Load(doc.SaveToBytes());

        Assert.Empty(Units(reloaded).Units);
    }
}
