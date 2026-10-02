// tests/Wc3.Tests/UnitInstanceCommandTests.cs
using War3Net.Build.Object;
using War3Net.Build.Widget;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

public class UnitInstanceCommandTests
{
    /// <summary>Blank map with one footman (owner 0) and player 1's start location.</summary>
    private static (MapDocument Doc, int FootmanCn, int SlocCn) Fixture()
    {
        var doc = BlankMap.Create();
        var footman = PlacementCommand.PlaceUnit(doc, "hfoo", ownerId: 0, x: 128f, y: -256f, rotation: 1.5f);
        Assert.True(footman.Ok, footman.Message);
        var sloc = PlacementCommand.PlaceStartLocation(doc, player: 1, x: 512f, y: 512f);
        Assert.True(sloc.Ok, sloc.Message);
        return (doc, footman.CreationNumber, sloc.CreationNumber);
    }

    [Fact]
    public void List_ReturnsAllPlacedUnits_IncludingStartLocations()
    {
        var (doc, footmanCn, slocCn) = Fixture();

        var all = UnitInstanceCommand.List(doc);
        Assert.Equal(2, all.Count);

        var footman = Assert.Single(all, u => u.CreationNumber == footmanCn);
        Assert.Equal("hfoo", footman.TypeRawcode);
        Assert.Equal(0, footman.OwnerId);
        Assert.Equal(128f, footman.X);
        Assert.Equal(-256f, footman.Y);
        Assert.Equal(1.5f, footman.Rotation);
        Assert.Equal((1f, 1f, 1f), footman.Scale);
        Assert.Equal(1, footman.HeroLevel);
        Assert.Equal(-1, footman.HpPercent);   // -1 = the object's default
        Assert.Equal(-1, footman.ManaPercent);
        Assert.Equal(0, footman.GoldAmount);
        Assert.Null(footman.Name);             // no object-data delta, no game data opened

        var sloc = Assert.Single(all, u => u.CreationNumber == slocCn);
        Assert.Equal(PlacementCommand.StartLocationRawcode, sloc.TypeRawcode);
        Assert.Equal("Start Location", sloc.Name);
        Assert.Equal(1, sloc.OwnerId);
    }

    [Fact]
    public void List_IsEmptyOnBlankMap_AndGetMissesReturnNull()
    {
        var doc = BlankMap.Create(); // no war3mapUnits.doo at all
        Assert.Empty(UnitInstanceCommand.List(doc));
        Assert.Null(UnitInstanceCommand.Get(doc, 0));

        var (withUnits, _, _) = Fixture();
        Assert.Null(UnitInstanceCommand.Get(withUnits, 999));
    }

    [Fact]
    public void SetOwner_MutatesUnit_MarksDirty_AndSurvivesRoundTrip()
    {
        var (doc, cn, _) = Fixture();

        var result = UnitInstanceCommand.SetOwner(doc, cn, 3);
        Assert.True(result.Ok, result.Message);
        Assert.Equal(3, UnitInstanceCommand.Get(doc, cn)!.OwnerId);
        Assert.True(doc.GetFile(PlacementCommand.UnitsFile)!.IsDirty);

        // The in-memory MapUnits model itself carries the change...
        var units = (MapUnits)doc.GetFile(PlacementCommand.UnitsFile)!.Model!;
        Assert.Equal(3, units.Units.Single(u => u.CreationNumber == cn).OwnerId);

        // ...and it persists through a real save + reload.
        var reloaded = MapDocument.Load(doc.SaveToBytes());
        Assert.Equal(3, UnitInstanceCommand.Get(reloaded, cn)!.OwnerId);
    }

    [Fact]
    public void PropertyEdits_ApplyToTheMatchingUnit()
    {
        var (doc, cn, slocCn) = Fixture();

        Assert.True(UnitInstanceCommand.SetHeroLevel(doc, cn, 5).Ok);
        Assert.True(UnitInstanceCommand.SetHpPercent(doc, cn, 50).Ok);
        Assert.True(UnitInstanceCommand.SetManaPercent(doc, cn, 25).Ok);
        Assert.True(UnitInstanceCommand.SetScale(doc, cn, 2f, 2.5f, 3f).Ok);
        Assert.True(UnitInstanceCommand.SetFacing(doc, cn, 3.14f).Ok);
        Assert.True(UnitInstanceCommand.SetGold(doc, cn, 12500).Ok);

        var info = UnitInstanceCommand.Get(doc, cn)!;
        Assert.Equal(5, info.HeroLevel);
        Assert.Equal(50, info.HpPercent);
        Assert.Equal(25, info.ManaPercent);
        Assert.Equal((2f, 2.5f, 3f), info.Scale);
        Assert.Equal(3.14f, info.Rotation);
        Assert.Equal(12500, info.GoldAmount);

        // The other placed unit is untouched.
        var sloc = UnitInstanceCommand.Get(doc, slocCn)!;
        Assert.Equal(1, sloc.HeroLevel);
        Assert.Equal((1f, 1f, 1f), sloc.Scale);
    }

    [Fact]
    public void HpAndMana_AcceptMinusOneAsDefault()
    {
        var (doc, cn, _) = Fixture();
        Assert.True(UnitInstanceCommand.SetHpPercent(doc, cn, 50).Ok);
        Assert.True(UnitInstanceCommand.SetHpPercent(doc, cn, -1).Ok);
        Assert.Equal(-1, UnitInstanceCommand.Get(doc, cn)!.HpPercent);
        Assert.True(UnitInstanceCommand.SetManaPercent(doc, cn, -1).Ok);
    }

    [Fact]
    public void Edits_RejectInvalidValues_WithoutMutating()
    {
        var (doc, cn, _) = Fixture();
        var before = UnitInstanceCommand.Get(doc, cn)!;

        Assert.False(UnitInstanceCommand.SetOwner(doc, cn, -1).Ok);
        Assert.False(UnitInstanceCommand.SetHeroLevel(doc, cn, 0).Ok);
        Assert.False(UnitInstanceCommand.SetHpPercent(doc, cn, 101).Ok);
        Assert.False(UnitInstanceCommand.SetHpPercent(doc, cn, -2).Ok);
        Assert.False(UnitInstanceCommand.SetManaPercent(doc, cn, 101).Ok);
        Assert.False(UnitInstanceCommand.SetScale(doc, cn, 0f, 1f, 1f).Ok);
        Assert.False(UnitInstanceCommand.SetGold(doc, cn, -5).Ok);

        Assert.Equal(before, UnitInstanceCommand.Get(doc, cn));
    }

    [Fact]
    public void Edits_FailCleanly_WhenUnitOrFileIsMissing()
    {
        var blank = BlankMap.Create();
        var noFile = UnitInstanceCommand.SetOwner(blank, 0, 1);
        Assert.False(noFile.Ok);

        var (doc, _, _) = Fixture();
        var noUnit = UnitInstanceCommand.SetOwner(doc, 999, 1);
        Assert.False(noUnit.Ok);
        Assert.Contains("999", noUnit.Message);
    }

    [Fact]
    public void Delete_RemovesTheUnit_AndPersists()
    {
        var (doc, footmanCn, slocCn) = Fixture();

        var r = UnitInstanceCommand.Delete(doc, footmanCn);
        Assert.True(r.Ok, r.Message);
        Assert.Null(UnitInstanceCommand.Get(doc, footmanCn));

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        Assert.Null(UnitInstanceCommand.Get(reloaded, footmanCn));
        Assert.NotNull(UnitInstanceCommand.Get(reloaded, slocCn)); // the other unit survives
    }

    [Fact]
    public void Delete_MissingUnit_FailsCleanly()
    {
        var (doc, _, _) = Fixture();
        Assert.False(UnitInstanceCommand.Delete(doc, 999).Ok);
    }

    [Fact]
    public void DeleteMany_RemovesTheSelection_InOneWrite()
    {
        var (doc, footmanCn, slocCn) = Fixture();

        var r = UnitInstanceCommand.DeleteMany(doc, new[] { footmanCn, slocCn });
        Assert.True(r.Ok, r.Message);
        Assert.Contains("2", r.Message);
        Assert.Empty(UnitInstanceCommand.List(doc));
    }

    [Fact]
    public void SetOwnerMany_RetargetsTheWholeSelection()
    {
        var (doc, footmanCn, slocCn) = Fixture();

        var r = UnitInstanceCommand.SetOwnerMany(doc, new[] { footmanCn, slocCn }, ownerId: 3);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(3, UnitInstanceCommand.Get(doc, footmanCn)!.OwnerId);
        Assert.Equal(3, UnitInstanceCommand.Get(doc, slocCn)!.OwnerId);
    }

    [Fact]
    public void SetOwnerMany_NoneMatched_FailsCleanly()
    {
        var (doc, _, _) = Fixture();
        Assert.False(UnitInstanceCommand.SetOwnerMany(doc, new[] { 998, 999 }, 2).Ok);
    }

    [Fact]
    public void Name_ResolvesFromMapObjectDataDelta()
    {
        var (doc, cn, _) = Fixture();

        // Give the placed type a map-local name delta (what the Object Editor writes).
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var delta = new SimpleObjectModification { OldId = "hfoo".FromRawcode(), NewId = 0 };
        delta.Modifications.Add(new SimpleObjectDataModification
        { Id = "unam".FromRawcode(), Type = ObjectDataType.String, Value = "Elite Footman" });
        w3u.BaseUnits.Add(delta);
        doc.AddOrReplaceModelFile("war3map.w3u", w3u);

        Assert.Equal("Elite Footman", UnitInstanceCommand.Get(doc, cn)!.Name);
    }
}
