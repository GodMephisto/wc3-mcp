// tests/Wc3.Tests/SemanticWriteTests.cs
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Build.Script;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Semantic write checks. A write is not proven by the map loading again, because the defects
/// already recorded here both load, one with missing UVs and one with a wrong trigger subtree.
/// Each fact below asserts the two things a loading map does not say on its own. The intended
/// change is present, and data the edit never targeted is unchanged.
///
/// Every assertion reads a reloaded map, never the document that was mutated. The edit could be
/// held only in memory and every one of these would still fail on the read-back.
/// </summary>
public class SemanticWriteTests
{
    private static MapDocument Reload(MapDocument doc) => MapDocument.Load(doc.SaveToBytes());

    private static byte[] Serialize(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }

    // --- 1. object_set ---------------------------------------------------------

    /// <summary>A w3u whose custom unit H000 carries uagi (Int 20) and unam ("Dark Paladin").</summary>
    private static byte[] ObjectMap()
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var custom = new SimpleObjectModification
        {
            OldId = "Hpal".FromRawcode(),
            NewId = "H000".FromRawcode(),
        };
        custom.Modifications.Add(new SimpleObjectDataModification
        {
            Id = "uagi".FromRawcode(),
            Type = ObjectDataType.Int,
            Value = 20,
        });
        custom.Modifications.Add(new SimpleObjectDataModification
        {
            Id = "unam".FromRawcode(),
            Type = ObjectDataType.String,
            Value = "Dark Paladin",
        });
        w3u.NewUnits.Add(custom);
        return Serialize(w => w.Write(w3u));
    }

    private static MapDocument ObjectDoc() =>
        MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = ObjectMap(),
        }));

    [Fact]
    public void Object_set_changes_the_named_field_and_leaves_the_sibling_field_alone()
    {
        var doc = ObjectDoc();
        var result = ObjectSetCommand.Execute(doc, "H000", "uagi", "35");
        Assert.True(result.Ok, result.Message);

        var w3u = (UnitObjectData)Reload(doc).GetFile("war3map.w3u")!.Model!;
        Assert.NotNull(w3u);
        var unit = w3u.NewUnits.Single(u => u.NewId == "H000".FromRawcode());

        // The intended change.
        var edited = unit.Modifications.Single(m => m.Id == "uagi".FromRawcode());
        Assert.Equal(ObjectDataType.Int, edited.Type);
        Assert.Equal(35, edited.Value);

        // The field next to it, which the edit never named.
        var sibling = unit.Modifications.Single(m => m.Id == "unam".FromRawcode());
        Assert.Equal(ObjectDataType.String, sibling.Type);
        Assert.Equal("Dark Paladin", sibling.Value);
        Assert.Equal(2, unit.Modifications.Count);
    }

    // --- 2. terrain corner -----------------------------------------------------

    [Fact]
    public void Terrain_corner_set_moves_the_edited_corner_and_leaves_its_neighbour_alone()
    {
        var doc = BlankMap.Create();
        float neighbourBefore = TerrainEditCommand.GetCorner(doc, 5, 4)!.GroundHeight;
        float target = TerrainEditCommand.GetCorner(doc, 4, 4)!.GroundHeight + 2.5f;

        var result = TerrainEditCommand.SetGroundHeight(doc, 4, 4, target);
        Assert.True(result.Ok, result.Message);
        Assert.Equal(1, result.CornersChanged);

        var reloaded = Reload(doc);

        // The intended change, on the corner the edit addressed.
        Assert.Equal(target, TerrainEditCommand.GetCorner(reloaded, 4, 4)!.GroundHeight);

        // The neighbour one column over, on the same row.
        Assert.Equal(neighbourBefore, TerrainEditCommand.GetCorner(reloaded, 5, 4)!.GroundHeight);
    }

    // --- 3. strings set --------------------------------------------------------

    private const string Wts =
        "STRING 1\r\n{\r\nFirst\r\n}\r\n\r\n" +
        "STRING 7\r\n{\r\nSecond\r\n}\r\n";

    [Fact]
    public void Strings_set_changes_one_entry_and_leaves_the_other_verbatim()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.wts"] = Encoding.UTF8.GetBytes(Wts),
        }));

        StringsCommand.Set(doc, 7, "Replaced");

        var reloaded = Reload(doc);
        var entries = StringsCommand.List(reloaded).Entries;

        // The intended change.
        Assert.Equal("Replaced", entries.Single(e => e.Id == 7).Text);

        // The other entry, which the edit never named.
        Assert.Equal("First", entries.Single(e => e.Id == 1).Text);

        // And the file itself, byte for byte, so nothing outside the body moved.
        Assert.Equal(
            "STRING 1\r\n{\r\nFirst\r\n}\r\n\r\n" +
            "STRING 7\r\n{\r\nReplaced\r\n}\r\n",
            Encoding.UTF8.GetString(reloaded.GetFile("war3map.wts")!.CurrentBytes));
    }

    // --- 4. trigger edit -------------------------------------------------------

    private const string TargetBody = "function TargetBody takes nothing returns nothing\nendfunction";
    private const string SiblingBody = "function SiblingBody takes nothing returns nothing\nendfunction";

    /// <summary>A wtg with two custom-text triggers, each paired with its own wct body.</summary>
    private static byte[] TriggerMap()
    {
        var wtg = new MapTriggers(MapTriggersFormatVersion.v7, MapTriggersSubVersion.v4);
        wtg.TriggerItems.Add(new TriggerDefinition(TriggerItemType.Script)
        {
            Id = 0x03000001, ParentId = -1, Name = "Target", Description = string.Empty,
            IsEnabled = true, IsInitiallyOn = true, IsCustomTextTrigger = true,
        });
        wtg.TriggerItems.Add(new TriggerDefinition(TriggerItemType.Script)
        {
            Id = 0x03000002, ParentId = -1, Name = "Sibling", Description = string.Empty,
            IsEnabled = true, IsInitiallyOn = true, IsCustomTextTrigger = true,
        });
        foreach (var group in wtg.TriggerItems.GroupBy(i => i.Type))
            wtg.TriggerItemCounts[group.Key] = group.Count();

        var wct = new MapCustomTextTriggers(MapCustomTextTriggersFormatVersion.v1, null)
        {
            GlobalCustomScriptComment = string.Empty,
            GlobalCustomScriptCode = new CustomTextTrigger { Code = string.Empty },
        };
        wct.CustomTextTriggers.Add(new CustomTextTrigger { Code = TargetBody });
        wct.CustomTextTriggers.Add(new CustomTextTrigger { Code = SiblingBody });

        return SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [TriggerCommand.FileName] = TriggerCommand.Serialize(wtg),
            [TriggerCommand.CustomTextFileName] = Serialize(w => w.Write(wct)),
        });
    }

    [Fact]
    public void Trigger_rename_changes_the_target_and_leaves_a_sibling_name_and_body_intact()
    {
        var doc = MapDocument.Load(TriggerMap());
        var before = TriggerReadCommand.GetTriggers(doc);
        Assert.Equal(2, before.Triggers.Count);

        var result = TriggerCommand.Rename(doc, 0x03000001, "Renamed Target");
        Assert.True(result.Ok, result.Message);

        var after = TriggerReadCommand.GetTriggers(Reload(doc));

        // The intended change, and its body still paired with it.
        var target = after.Triggers.Single(t => t.Id == 0x03000001);
        Assert.Equal("Renamed Target", target.Name);
        Assert.Equal(TargetBody, target.CustomText);

        // The sibling, name and body both.
        var sibling = after.Triggers.Single(t => t.Id == 0x03000002);
        Assert.Equal("Sibling", sibling.Name);
        Assert.Equal(SiblingBody, sibling.CustomText);
        Assert.True(sibling.Enabled);
        Assert.True(sibling.InitiallyOn);
        Assert.Equal(before.Triggers.Count, after.Triggers.Count);
    }

    // --- 5. placed unit --------------------------------------------------------

    [Fact]
    public void Placed_unit_set_moves_the_target_and_leaves_the_other_instance_in_place()
    {
        var doc = BlankMap.Create();
        var first = PlacementCommand.PlaceUnit(doc, "hfoo", ownerId: 0, x: 128f, y: -256f);
        var second = PlacementCommand.PlaceUnit(doc, "hfoo", ownerId: 0, x: 512f, y: 512f);
        Assert.True(first.Ok, first.Message);
        Assert.True(second.Ok, second.Message);

        var result = UnitInstanceCommand.SetPosition(doc, first.CreationNumber, 1024f, 1024f);
        Assert.True(result.Ok, result.Message);

        var reloaded = Reload(doc);

        // The intended change.
        var moved = UnitInstanceCommand.Get(reloaded, first.CreationNumber)!;
        Assert.Equal(1024f, moved.X);
        Assert.Equal(1024f, moved.Y);

        // The other placed instance, which the edit never named.
        var still = UnitInstanceCommand.Get(reloaded, second.CreationNumber)!;
        Assert.Equal(512f, still.X);
        Assert.Equal(512f, still.Y);
    }

    // --- 6. refusal ------------------------------------------------------------

    [Fact]
    public void A_rejected_object_set_leaves_the_document_unmutated()
    {
        var doc = ObjectDoc();
        byte[] original = doc.GetFile("war3map.w3u")!.RawBytes;

        // A malformed field code, rejected before anything is looked up.
        var badField = ObjectSetCommand.Execute(doc, "H000", "zz", "1");
        Assert.False(badField.Ok);
        Assert.Contains("field code", badField.Message);

        // A value that does not parse as the existing Int type, rejected at the edit itself.
        var badValue = ObjectSetCommand.Execute(doc, "H000", "uagi", "not-a-number");
        Assert.False(badValue.Ok);

        Assert.False(doc.GetFile("war3map.w3u")!.IsDirty);

        var reloaded = Reload(doc);

        // The file is the byte-exact original, so neither refusal wrote anything.
        Assert.Equal(original, reloaded.GetFile("war3map.w3u")!.RawBytes);

        // And the values on the reloaded map are the ones the fixture supplied.
        var w3u = (UnitObjectData)reloaded.GetFile("war3map.w3u")!.Model!;
        var unit = w3u.NewUnits.Single(u => u.NewId == "H000".FromRawcode());
        Assert.Equal(20, unit.Modifications.Single(m => m.Id == "uagi".FromRawcode()).Value);
        Assert.Equal("Dark Paladin", unit.Modifications.Single(m => m.Id == "unam".FromRawcode()).Value);
    }
}
