// tests/Wc3.Tests/DoodadInstanceCommandTests.cs
using War3Net.Build.Object;
using War3Net.Build.Widget;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

public class DoodadInstanceCommandTests
{
    /// <summary>Blank map with a tree (variation 3) and a rock, both via PlaceDoodad.</summary>
    private static (MapDocument Doc, int TreeCn, int RockCn) Fixture()
    {
        var doc = BlankMap.Create();
        var tree = PlacementCommand.PlaceDoodad(doc, "LTlt", x: 128f, y: -256f, rotation: 1.5f, variation: 3);
        Assert.True(tree.Ok, tree.Message);
        var rock = PlacementCommand.PlaceDoodad(doc, "LOrc", x: 512f, y: 512f);
        Assert.True(rock.Ok, rock.Message);
        return (doc, tree.CreationNumber, rock.CreationNumber);
    }

    [Fact]
    public void List_ReturnsAllPlacedDoodads()
    {
        var (doc, treeCn, rockCn) = Fixture();

        var all = DoodadInstanceCommand.List(doc);
        Assert.Equal(2, all.Count);

        var tree = Assert.Single(all, d => d.CreationNumber == treeCn);
        Assert.Equal("LTlt", tree.TypeRawcode);
        Assert.Equal(128f, tree.X);
        Assert.Equal(-256f, tree.Y);
        Assert.Equal(0f, tree.Z);
        Assert.Equal(1.5f, tree.Rotation);
        Assert.Equal((1f, 1f, 1f), tree.Scale);
        Assert.Equal(3, tree.Variation);
        Assert.Equal(100, tree.LifePercent);  // PlaceDoodad's World-Editor default
        Assert.Null(tree.Name);               // no object-data delta, no game data opened

        var rock = Assert.Single(all, d => d.CreationNumber == rockCn);
        Assert.Equal("LOrc", rock.TypeRawcode);
        Assert.Equal(0, rock.Variation);
    }

    [Fact]
    public void List_IsEmptyOnBlankMap_AndGetMissesReturnNull()
    {
        var doc = BlankMap.Create(); // no war3map.doo at all
        Assert.Empty(DoodadInstanceCommand.List(doc));
        Assert.Null(DoodadInstanceCommand.Get(doc, 0));

        var (withDoodads, _, _) = Fixture();
        Assert.Null(DoodadInstanceCommand.Get(withDoodads, 999));
    }

    [Fact]
    public void SetPosition_MutatesDoodad_MarksDirty_AndSurvivesRoundTrip()
    {
        var (doc, cn, _) = Fixture();

        var result = DoodadInstanceCommand.SetPosition(doc, cn, 300f, -400f, 25f);
        Assert.True(result.Ok, result.Message);
        var info = DoodadInstanceCommand.Get(doc, cn)!;
        Assert.Equal((300f, -400f, 25f), (info.X, info.Y, info.Z));
        Assert.True(doc.GetFile(PlacementCommand.DoodadsFile)!.IsDirty);

        // The in-memory MapDoodads model itself carries the change...
        var doodads = (MapDoodads)doc.GetFile(PlacementCommand.DoodadsFile)!.Model!;
        Assert.Equal(300f, doodads.Doodads.Single(d => d.CreationNumber == cn).Position.X);

        // ...and it persists through a real save + reload.
        var reloaded = MapDocument.Load(doc.SaveToBytes());
        var back = DoodadInstanceCommand.Get(reloaded, cn)!;
        Assert.Equal((300f, -400f, 25f), (back.X, back.Y, back.Z));
    }

    [Fact]
    public void PropertyEdits_ApplyToTheMatchingDoodad_AndSurviveRoundTrip()
    {
        var (doc, cn, rockCn) = Fixture();

        Assert.True(DoodadInstanceCommand.SetRotation(doc, cn, 3.14f).Ok);
        Assert.True(DoodadInstanceCommand.SetScale(doc, cn, 2f, 2.5f, 3f).Ok);
        Assert.True(DoodadInstanceCommand.SetVariation(doc, cn, 7).Ok);
        Assert.True(DoodadInstanceCommand.SetLifePercent(doc, cn, 55).Ok);

        var info = DoodadInstanceCommand.Get(doc, cn)!;
        Assert.Equal(3.14f, info.Rotation);
        Assert.Equal((2f, 2.5f, 3f), info.Scale);
        Assert.Equal(7, info.Variation);
        Assert.Equal(55, info.LifePercent);

        // The other placed doodad is untouched.
        var rock = DoodadInstanceCommand.Get(doc, rockCn)!;
        Assert.Equal((1f, 1f, 1f), rock.Scale);
        Assert.Equal(100, rock.LifePercent);

        // Everything persists through a real save + reload.
        var back = DoodadInstanceCommand.Get(MapDocument.Load(doc.SaveToBytes()), cn)!;
        Assert.Equal(3.14f, back.Rotation);
        Assert.Equal((2f, 2.5f, 3f), back.Scale);
        Assert.Equal(7, back.Variation);
        Assert.Equal(55, back.LifePercent);
    }

    [Fact]
    public void Edits_RejectInvalidValues_WithoutMutating()
    {
        var (doc, cn, _) = Fixture();
        var before = DoodadInstanceCommand.Get(doc, cn)!;

        Assert.False(DoodadInstanceCommand.SetScale(doc, cn, 0f, 1f, 1f).Ok);
        Assert.False(DoodadInstanceCommand.SetScale(doc, cn, 1f, -1f, 1f).Ok);
        Assert.False(DoodadInstanceCommand.SetVariation(doc, cn, -1).Ok);
        Assert.False(DoodadInstanceCommand.SetLifePercent(doc, cn, -1).Ok);
        Assert.False(DoodadInstanceCommand.SetLifePercent(doc, cn, 101).Ok);

        Assert.Equal(before, DoodadInstanceCommand.Get(doc, cn));
    }

    [Fact]
    public void Edits_FailCleanly_WhenDoodadOrFileIsMissing()
    {
        var blank = BlankMap.Create();
        var noFile = DoodadInstanceCommand.SetRotation(blank, 0, 1f);
        Assert.False(noFile.Ok);
        Assert.False(DoodadInstanceCommand.Delete(blank, 0).Ok);

        var (doc, _, _) = Fixture();
        var noDoodad = DoodadInstanceCommand.SetRotation(doc, 999, 1f);
        Assert.False(noDoodad.Ok);
        Assert.Contains("999", noDoodad.Message);
        Assert.False(DoodadInstanceCommand.Delete(doc, 999).Ok);
    }

    [Fact]
    public void Delete_RemovesTheDoodad_AndSurvivesRoundTrip()
    {
        var (doc, treeCn, rockCn) = Fixture();

        var result = DoodadInstanceCommand.Delete(doc, treeCn);
        Assert.True(result.Ok, result.Message);
        Assert.True(doc.GetFile(PlacementCommand.DoodadsFile)!.IsDirty);

        Assert.Null(DoodadInstanceCommand.Get(doc, treeCn));
        var remaining = Assert.Single(DoodadInstanceCommand.List(doc));
        Assert.Equal(rockCn, remaining.CreationNumber);

        // The removal persists through a real save + reload.
        var reloaded = MapDocument.Load(doc.SaveToBytes());
        Assert.Null(DoodadInstanceCommand.Get(reloaded, treeCn));
        Assert.Equal(rockCn, Assert.Single(DoodadInstanceCommand.List(reloaded)).CreationNumber);
    }

    [Fact]
    public void Name_ResolvesFromDestructableAndDoodadDeltas()
    {
        var (doc, treeCn, rockCn) = Fixture();

        // The tree type gets a destructable (w3b, simple-shape) name delta...
        var w3b = new DestructableObjectData(ObjectDataFormatVersion.v2);
        var treeDelta = new SimpleObjectModification { OldId = "LTlt".FromRawcode(), NewId = 0 };
        treeDelta.Modifications.Add(new SimpleObjectDataModification
        { Id = "bnam".FromRawcode(), Type = ObjectDataType.String, Value = "Iron Tree" });
        w3b.BaseDestructables.Add(treeDelta);
        doc.AddOrReplaceModelFile("war3map.w3b", w3b);

        // ...and the rock type a doodad (w3d, variation-shape) name delta.
        var w3d = new DoodadObjectData(ObjectDataFormatVersion.v2);
        var rockDelta = new VariationObjectModification { OldId = "LOrc".FromRawcode(), NewId = 0 };
        rockDelta.Modifications.Add(new VariationObjectDataModification
        { Id = "dnam".FromRawcode(), Type = ObjectDataType.String, Value = "Odd Rock", Variation = 0, Pointer = 0 });
        w3d.BaseDoodads.Add(rockDelta);
        doc.AddOrReplaceModelFile("war3map.w3d", w3d);

        Assert.Equal("Iron Tree", DoodadInstanceCommand.Get(doc, treeCn)!.Name);
        Assert.Equal("Odd Rock", DoodadInstanceCommand.Get(doc, rockCn)!.Name);
    }
}
