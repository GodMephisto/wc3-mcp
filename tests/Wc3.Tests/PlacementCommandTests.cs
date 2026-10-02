// tests/Wc3.Tests/PlacementCommandTests.cs
using War3Net.Build.Environment;
using War3Net.Build.Widget;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

public class PlacementCommandTests
{
    [Fact]
    public void PlaceUnit_CreatesUnitsFileOnBlankMap_AndSurvivesRoundTrip()
    {
        var doc = BlankMap.Create();
        Assert.Null(doc.GetFile(PlacementCommand.UnitsFile)); // blank map has no placement file

        var result = PlacementCommand.PlaceUnit(doc, "hfoo", ownerId: 0, x: 128f, y: -256f);
        Assert.True(result.Ok, result.Message);
        Assert.Equal(0, result.CreationNumber);

        // The file must now exist as a parsed MapUnits model, not a raw blob.
        var entry = doc.GetFile(PlacementCommand.UnitsFile);
        Assert.NotNull(entry);
        Assert.IsType<MapUnits>(entry!.Model);

        // Save + reload proves the SerializeEntry write path handles MapUnits.
        byte[] saved = doc.SaveToBytes();
        var reloaded = MapDocument.Load(saved);

        var units = reloaded.GetFile(PlacementCommand.UnitsFile)?.Model as MapUnits;
        Assert.NotNull(units);
        var u = Assert.Single(units!.Units);
        Assert.Equal("hfoo".FromRawcode(), u.TypeId);
        Assert.Equal(0, u.OwnerId);
        Assert.Equal(128f, u.Position.X);
        Assert.Equal(-256f, u.Position.Y);
        Assert.Equal(0, u.CreationNumber);

        // A well-formed placement must not degrade the clean round-trip.
        Assert.DoesNotContain(reloaded.Diagnostics, d => d.Severity == DiagnosticSeverity.Warning);
    }

    [Fact]
    public void PlaceUnit_AssignsUniqueIncrementingCreationNumbers()
    {
        var doc = BlankMap.Create();

        var a = PlacementCommand.PlaceUnit(doc, "hfoo", 0, 0f, 0f);
        var b = PlacementCommand.PlaceUnit(doc, "hkni", 0, 64f, 0f);
        var c = PlacementCommand.PlaceUnit(doc, "hrif", 1, 128f, 0f);

        Assert.Equal(0, a.CreationNumber);
        Assert.Equal(1, b.CreationNumber);
        Assert.Equal(2, c.CreationNumber);

        byte[] saved = doc.SaveToBytes();
        var units = MapDocument.Load(saved).GetFile(PlacementCommand.UnitsFile)?.Model as MapUnits;
        Assert.NotNull(units);
        Assert.Equal(3, units!.Units.Count);
        Assert.Equal(new[] { 0, 1, 2 }, units.Units.Select(u => u.CreationNumber).OrderBy(n => n));
    }

    [Theory]
    [InlineData("foo", 0)]   // too short
    [InlineData("hfoot", 0)] // too long
    [InlineData("hfoo", -1)] // bad owner
    public void PlaceUnit_RejectsBadInput(string rawcode, int owner)
    {
        var doc = BlankMap.Create();
        var result = PlacementCommand.PlaceUnit(doc, rawcode, owner, 0f, 0f);
        Assert.False(result.Ok);
        Assert.Null(doc.GetFile(PlacementCommand.UnitsFile)); // nothing written on rejection
    }

    // ---- Start locations (sloc units in war3mapUnits.doo) -----------------

    [Fact]
    public void PlaceStartLocation_CreatesSlocUnitOwnedByPlayer_AndSurvivesRoundTrip()
    {
        var doc = BlankMap.Create();
        int slocType = PlacementCommand.StartLocationRawcode.FromRawcode();

        var result = PlacementCommand.PlaceStartLocation(doc, player: 3, x: 512f, y: -768f);
        Assert.True(result.Ok, result.Message);
        Assert.Equal(0, result.CreationNumber);

        byte[] saved = doc.SaveToBytes();
        var units = MapDocument.Load(saved).GetFile(PlacementCommand.UnitsFile)?.Model as MapUnits;
        Assert.NotNull(units);
        var sloc = Assert.Single(units!.Units);
        Assert.Equal(slocType, sloc.TypeId);
        Assert.Equal(3, sloc.OwnerId);
        Assert.Equal(512f, sloc.Position.X);
        Assert.Equal(-768f, sloc.Position.Y);
    }

    [Fact]
    public void PlaceStartLocation_MovesExistingRatherThanDuplicating()
    {
        var doc = BlankMap.Create();

        var first = PlacementCommand.PlaceStartLocation(doc, player: 0, x: 0f, y: 0f);
        var second = PlacementCommand.PlaceStartLocation(doc, player: 0, x: 256f, y: 128f);
        Assert.True(second.Ok, second.Message);

        // Same player -> moved in place: one unit total, its creation number preserved.
        Assert.Equal(first.CreationNumber, second.CreationNumber);

        var units = doc.GetFile(PlacementCommand.UnitsFile)?.Model as MapUnits;
        Assert.NotNull(units);
        var sloc = Assert.Single(units!.Units);
        Assert.Equal(256f, sloc.Position.X);
        Assert.Equal(128f, sloc.Position.Y);
    }

    [Fact]
    public void PlaceStartLocation_DifferentPlayersGetSeparateLocations()
    {
        var doc = BlankMap.Create();
        int slocType = PlacementCommand.StartLocationRawcode.FromRawcode();

        PlacementCommand.PlaceStartLocation(doc, player: 0, x: 0f, y: 0f);
        PlacementCommand.PlaceStartLocation(doc, player: 1, x: 1024f, y: 1024f);

        var units = doc.GetFile(PlacementCommand.UnitsFile)?.Model as MapUnits;
        Assert.NotNull(units);
        Assert.Equal(2, units!.Units.Count(u => u.TypeId == slocType));
        Assert.Equal(
            new[] { 0, 1 },
            units.Units.Where(u => u.TypeId == slocType).Select(u => u.OwnerId).OrderBy(n => n));
    }

    [Fact]
    public void PlaceStartLocation_RejectsNegativePlayer()
    {
        var doc = BlankMap.Create();
        var result = PlacementCommand.PlaceStartLocation(doc, player: -1, x: 0f, y: 0f);
        Assert.False(result.Ok);
        Assert.Null(doc.GetFile(PlacementCommand.UnitsFile)); // nothing written on rejection
    }

    // ---- Items (UnitData in war3mapUnits.doo) -----------------------------

    [Fact]
    public void PlaceItem_WritesItemEntryOwnedByItemSlot_AndSurvivesRoundTrip()
    {
        var doc = BlankMap.Create();

        // tdst = Boots of Speed, a standard item rawcode.
        var result = PlacementCommand.PlaceItem(doc, "bspd", x: 320f, y: -64f, rotation: 0.5f);
        Assert.True(result.Ok, result.Message);
        Assert.Equal(0, result.CreationNumber);

        byte[] saved = doc.SaveToBytes();
        var reloaded = MapDocument.Load(saved);

        var units = reloaded.GetFile(PlacementCommand.UnitsFile)?.Model as MapUnits;
        Assert.NotNull(units);
        var item = Assert.Single(units!.Units);
        Assert.Equal("bspd".FromRawcode(), item.TypeId);
        Assert.Equal(PlacementCommand.ItemOwnerId, item.OwnerId);   // items sit in the item slot, not a player
        Assert.Equal(320f, item.Position.X);
        Assert.Equal(-64f, item.Position.Y);
        Assert.Equal(0.5f, item.Rotation);

        Assert.DoesNotContain(reloaded.Diagnostics, d => d.Severity == DiagnosticSeverity.Warning);
    }

    [Fact]
    public void PlaceItem_SharesTheCreationNumberSequenceWithUnits()
    {
        var doc = BlankMap.Create();
        var u = PlacementCommand.PlaceUnit(doc, "hfoo", 0, 0f, 0f);
        var i = PlacementCommand.PlaceItem(doc, "bspd", 64f, 0f);
        Assert.Equal(0, u.CreationNumber);
        Assert.Equal(1, i.CreationNumber);   // one shared war3mapUnits.doo sequence

        var units = doc.GetFile(PlacementCommand.UnitsFile)?.Model as MapUnits;
        Assert.Equal(2, units!.Units.Count);
    }

    [Theory]
    [InlineData("abc")]    // too short
    [InlineData("abcde")]  // too long
    [InlineData(null)]
    public void PlaceItem_RejectsBadInput(string rawcode)
    {
        var doc = BlankMap.Create();
        var result = PlacementCommand.PlaceItem(doc, rawcode, 0f, 0f);
        Assert.False(result.Ok);
        Assert.Null(doc.GetFile(PlacementCommand.UnitsFile)); // nothing written on rejection
    }

    // ---- Doodads (war3map.doo) --------------------------------------------

    [Fact]
    public void PlaceDoodad_CreatesDoodadsFileOnBlankMap_AndSurvivesRoundTrip()
    {
        var doc = BlankMap.Create();
        Assert.Null(doc.GetFile(PlacementCommand.DoodadsFile)); // blank map has no doodad file

        var result = PlacementCommand.PlaceDoodad(doc, "LTlt", x: 64f, y: -128f, rotation: 1.5f, scale: 2f);
        Assert.True(result.Ok, result.Message);
        Assert.Equal(0, result.CreationNumber);

        var entry = doc.GetFile(PlacementCommand.DoodadsFile);
        Assert.NotNull(entry);
        Assert.IsType<MapDoodads>(entry!.Model);

        byte[] saved = doc.SaveToBytes();
        var reloaded = MapDocument.Load(saved);

        var doodads = reloaded.GetFile(PlacementCommand.DoodadsFile)?.Model as MapDoodads;
        Assert.NotNull(doodads);
        var d = Assert.Single(doodads!.Doodads);
        Assert.Equal("LTlt".FromRawcode(), d.TypeId);
        Assert.Equal(64f, d.Position.X);
        Assert.Equal(-128f, d.Position.Y);
        Assert.Equal(1.5f, d.Rotation);
        Assert.Equal(2f, d.Scale.X);
        Assert.Equal(0, d.CreationNumber);

        Assert.DoesNotContain(reloaded.Diagnostics, x => x.Severity == DiagnosticSeverity.Warning);
    }

    [Fact]
    public void PlaceDoodad_AssignsUniqueIncrementingCreationNumbers()
    {
        var doc = BlankMap.Create();
        var a = PlacementCommand.PlaceDoodad(doc, "LTlt", 0f, 0f);
        var b = PlacementCommand.PlaceDoodad(doc, "ATtr", 64f, 0f);
        var c = PlacementCommand.PlaceDoodad(doc, "BTtw", 128f, 0f);
        Assert.Equal(0, a.CreationNumber);
        Assert.Equal(1, b.CreationNumber);
        Assert.Equal(2, c.CreationNumber);

        byte[] saved = doc.SaveToBytes();
        var doodads = MapDocument.Load(saved).GetFile(PlacementCommand.DoodadsFile)?.Model as MapDoodads;
        Assert.NotNull(doodads);
        Assert.Equal(3, doodads!.Doodads.Count);
        Assert.Equal(new[] { 0, 1, 2 }, doodads.Doodads.Select(d => d.CreationNumber).OrderBy(n => n));
    }

    [Theory]
    [InlineData("abc")]    // too short
    [InlineData("abcde")]  // too long
    [InlineData(null)]
    public void PlaceDoodad_RejectsBadInput(string rawcode)
    {
        var doc = BlankMap.Create();
        var result = PlacementCommand.PlaceDoodad(doc, rawcode, 0f, 0f);
        Assert.False(result.Ok);
        Assert.Null(doc.GetFile(PlacementCommand.DoodadsFile)); // nothing written on rejection
    }

    // ---- Regions (war3map.w3r) --------------------------------------------

    [Fact]
    public void PlaceRegion_CreatesRegionsFileOnBlankMap_AndSurvivesRoundTrip()
    {
        var doc = BlankMap.Create();
        Assert.Null(doc.GetFile(PlacementCommand.RegionsFile)); // blank map has no region file

        var result = PlacementCommand.PlaceRegion(doc, "spawn", left: -256f, bottom: -256f, right: 256f, top: 256f);
        Assert.True(result.Ok, result.Message);
        Assert.Equal(0, result.CreationNumber);

        // Save + reload proves the SerializeEntry write path handles MapRegions.
        byte[] saved = doc.SaveToBytes();
        var reloaded = MapDocument.Load(saved);

        var regions = reloaded.GetFile(PlacementCommand.RegionsFile)?.Model as MapRegions;
        Assert.NotNull(regions);
        var r = Assert.Single(regions!.Regions);
        Assert.Equal("spawn", r.Name);
        Assert.Equal(-256f, r.Left);
        Assert.Equal(-256f, r.Bottom);
        Assert.Equal(256f, r.Right);
        Assert.Equal(256f, r.Top);
        Assert.Equal(0, r.CreationNumber);

        Assert.DoesNotContain(reloaded.Diagnostics, x => x.Severity == DiagnosticSeverity.Warning);
    }

    [Fact]
    public void PlaceRegion_AssignsUniqueIncrementingCreationNumbers()
    {
        var doc = BlankMap.Create();
        var a = PlacementCommand.PlaceRegion(doc, "a", 0f, 0f, 64f, 64f);
        var b = PlacementCommand.PlaceRegion(doc, "b", 64f, 0f, 128f, 64f);
        Assert.Equal(0, a.CreationNumber);
        Assert.Equal(1, b.CreationNumber);

        byte[] saved = doc.SaveToBytes();
        var regions = MapDocument.Load(saved).GetFile(PlacementCommand.RegionsFile)?.Model as MapRegions;
        Assert.NotNull(regions);
        Assert.Equal(2, regions!.Regions.Count);
    }

    [Theory]
    [InlineData("", 0f, 0f, 64f, 64f)]   // empty name
    [InlineData("z", 64f, 0f, 0f, 64f)]  // right <= left
    [InlineData("z", 0f, 64f, 64f, 0f)]  // top <= bottom
    public void PlaceRegion_RejectsBadInput(string name, float left, float bottom, float right, float top)
    {
        var doc = BlankMap.Create();
        var result = PlacementCommand.PlaceRegion(doc, name, left, bottom, right, top);
        Assert.False(result.Ok);
        Assert.Null(doc.GetFile(PlacementCommand.RegionsFile)); // nothing written on rejection
    }

    [Fact]
    public void ListRegions_ReturnsPlacedRegions_EmptyWhenNone()
    {
        var doc = BlankMap.Create();
        Assert.Empty(PlacementCommand.ListRegions(doc)); // no region file yet

        PlacementCommand.PlaceRegion(doc, "spawn", -256f, -256f, 256f, 256f);
        PlacementCommand.PlaceRegion(doc, "arena", 0f, 0f, 512f, 512f);

        var regions = PlacementCommand.ListRegions(doc);
        Assert.Equal(2, regions.Count);
        var spawn = regions.Single(r => r.Name == "spawn");
        Assert.Equal(-256f, spawn.Left);
        Assert.Equal(256f, spawn.Top);
    }

    [Fact]
    public void RemoveRegion_DeletesByName_AndReportsMisses()
    {
        var doc = BlankMap.Create();
        PlacementCommand.PlaceRegion(doc, "spawn", 0f, 0f, 64f, 64f);
        PlacementCommand.PlaceRegion(doc, "arena", 64f, 0f, 128f, 64f);

        var ok = PlacementCommand.RemoveRegion(doc, "SPAWN"); // case-insensitive
        Assert.True(ok.Ok, ok.Message);
        Assert.Equal(new[] { "arena" }, PlacementCommand.ListRegions(doc).Select(r => r.Name));

        var miss = PlacementCommand.RemoveRegion(doc, "nope");
        Assert.False(miss.Ok);

        // The edit persists through a save/reload.
        var reloaded = MapDocument.Load(doc.SaveToBytes());
        Assert.Equal(new[] { "arena" }, PlacementCommand.ListRegions(reloaded).Select(r => r.Name));
    }
}
