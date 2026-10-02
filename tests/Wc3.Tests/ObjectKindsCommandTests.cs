// tests/Wc3.Tests/ObjectKindsCommandTests.cs
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Build.Script;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Hermetic coverage (ctx=null → deltas-only) for the multi-kind object commands:
/// one test per modification shape (Simple via items, Level via upgrades, Variation
/// via doodads), plus the war3mapSkin.* overlay and TRIGSTR name resolution.
/// </summary>
public class ObjectKindsCommandTests
{
    [Fact]
    public void Item_deltas_resolve_from_w3t_with_trigstr_name()
    {
        var w3t = new ItemObjectData(ObjectDataFormatVersion.v2);
        var item = new SimpleObjectModification { OldId = "ratf".FromRawcode(), NewId = "I000".FromRawcode() };
        item.Modifications.Add(new SimpleObjectDataModification
        { Id = "unam".FromRawcode(), Type = ObjectDataType.String, Value = "TRIGSTR_3" });
        item.Modifications.Add(new SimpleObjectDataModification
        { Id = "ilev".FromRawcode(), Type = ObjectDataType.Int, Value = 5 });
        w3t.NewItems.Add(item);

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3t"] = Serialize(w => w.Write(w3t)),
            ["war3map.wts"] = Wts((3u, "Bandit Blade")),
        }));

        var got = ObjectGetCommand.Execute(doc, ObjectKind.Item, "I000", ctx: null, preDiagnostics: Array.Empty<string>());
        Assert.True(got.Found);
        Assert.Equal("ratf", got.BaseRawcode);
        Assert.Equal("Bandit Blade", got.Name);
        Assert.Equal("5", got.Fields.Single(f => f.Code == "ilev").Value);
        Assert.All(got.Fields, f => Assert.Equal("map", f.Source));

        var listed = ObjectListCommand.Execute(doc, ObjectKind.Item, ctx: null);
        Assert.Equal(new ObjectListItem("I000", "ratf", "Bandit Blade"), Assert.Single(listed.Items));

        // Strict kind: the item rawcode is invisible through the unit lens…
        Assert.False(ObjectGetCommand.Execute(doc, ObjectKind.Unit, "I000", ctx: null,
            preDiagnostics: Array.Empty<string>()).Found);
        // …but the kind-agnostic overload auto-detects it.
        Assert.True(ObjectGetCommand.Execute(doc, "I000", ctx: null,
            preDiagnostics: Array.Empty<string>()).Found);
    }

    [Fact]
    public void Upgrade_level_deltas_key_as_code_colon_level()
    {
        var w3q = new UpgradeObjectData(ObjectDataFormatVersion.v2);
        var upgrade = new LevelObjectModification { OldId = "Rhme".FromRawcode(), NewId = "R000".FromRawcode() };
        upgrade.Modifications.Add(new LevelObjectDataModification
        { Id = "gnam".FromRawcode(), Type = ObjectDataType.String, Value = "Sharper Blades", Level = 0, Pointer = 0 });
        upgrade.Modifications.Add(new LevelObjectDataModification
        { Id = "gtim".FromRawcode(), Type = ObjectDataType.Int, Value = 45, Level = 2, Pointer = 0 });
        w3q.NewUpgrades.Add(upgrade);

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        { ["war3map.w3q"] = Serialize(w => w.Write(w3q)) }));

        var got = ObjectGetCommand.Execute(doc, ObjectKind.Upgrade, "R000", ctx: null, preDiagnostics: Array.Empty<string>());
        Assert.True(got.Found);
        Assert.Equal("Rhme", got.BaseRawcode);
        Assert.Equal("Sharper Blades", got.Name);
        Assert.Equal("45", got.Fields.Single(f => f.Code == "gtim:2").Value);

        var listed = ObjectListCommand.Execute(doc, ObjectKind.Upgrade, ctx: null);
        Assert.Equal(new ObjectListItem("R000", "Rhme", "Sharper Blades"), Assert.Single(listed.Items));
    }

    [Fact]
    public void Doodad_variation_deltas_key_as_code_colon_variation()
    {
        var w3d = new DoodadObjectData(ObjectDataFormatVersion.v2);
        var doodad = new VariationObjectModification { OldId = "AOhs".FromRawcode(), NewId = "D000".FromRawcode() };
        doodad.Modifications.Add(new VariationObjectDataModification
        { Id = "dnam".FromRawcode(), Type = ObjectDataType.String, Value = "Odd Stump", Variation = 0, Pointer = 0 });
        doodad.Modifications.Add(new VariationObjectDataModification
        { Id = "dvis".FromRawcode(), Type = ObjectDataType.Unreal, Value = 250f, Variation = 2, Pointer = 0 });
        w3d.NewDoodads.Add(doodad);

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        { ["war3map.w3d"] = Serialize(w => w.Write(w3d)) }));

        var got = ObjectGetCommand.Execute(doc, ObjectKind.Doodad, "D000", ctx: null, preDiagnostics: Array.Empty<string>());
        Assert.True(got.Found);
        Assert.Equal("AOhs", got.BaseRawcode);
        Assert.Equal("Odd Stump", got.Name);
        Assert.Equal("250", got.Fields.Single(f => f.Code == "dvis:2").Value);
    }

    [Fact]
    public void Buff_and_destructable_simple_deltas_resolve()
    {
        // A modified standard buff (base-list entry: NewId=0, its own base) and a
        // from-scratch custom destructable derived from a standard tree.
        var w3h = new BuffObjectData(ObjectDataFormatVersion.v2);
        var buff = new SimpleObjectModification { OldId = "BSTN".FromRawcode(), NewId = 0 };
        buff.Modifications.Add(new SimpleObjectDataModification
        { Id = "fnam".FromRawcode(), Type = ObjectDataType.String, Value = "Super Stun" });
        w3h.BaseBuffs.Add(buff);

        var w3b = new DestructableObjectData(ObjectDataFormatVersion.v2);
        var tree = new SimpleObjectModification { OldId = "ATtr".FromRawcode(), NewId = "B000".FromRawcode() };
        tree.Modifications.Add(new SimpleObjectDataModification
        { Id = "bnam".FromRawcode(), Type = ObjectDataType.String, Value = "Iron Tree" });
        w3b.NewDestructables.Add(tree);

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3h"] = Serialize(w => w.Write(w3h)),
            ["war3map.w3b"] = Serialize(w => w.Write(w3b)),
        }));

        var gotBuff = ObjectGetCommand.Execute(doc, ObjectKind.Buff, "BSTN", ctx: null, preDiagnostics: Array.Empty<string>());
        Assert.True(gotBuff.Found);
        Assert.Equal("BSTN", gotBuff.BaseRawcode);
        Assert.Equal("Super Stun", gotBuff.Name);

        var gotTree = ObjectGetCommand.Execute(doc, ObjectKind.Destructable, "B000", ctx: null, preDiagnostics: Array.Empty<string>());
        Assert.True(gotTree.Found);
        Assert.Equal("ATtr", gotTree.BaseRawcode);
        Assert.Equal("Iron Tree", gotTree.Name);

        Assert.Equal("BSTN", Assert.Single(ObjectListCommand.Execute(doc, ObjectKind.Buff, ctx: null).Items).Rawcode);
        Assert.Equal("B000", Assert.Single(ObjectListCommand.Execute(doc, ObjectKind.Destructable, ctx: null).Items).Rawcode);
    }

    [Fact]
    public void Skin_layer_overrides_map_fields_and_adds_objects()
    {
        var map = new UnitObjectData(ObjectDataFormatVersion.v2);
        var mapUnit = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        mapUnit.Modifications.Add(new SimpleObjectDataModification
        { Id = "unam".FromRawcode(), Type = ObjectDataType.String, Value = "Dark Paladin" });
        mapUnit.Modifications.Add(new SimpleObjectDataModification
        { Id = "uhpm".FromRawcode(), Type = ObjectDataType.Int, Value = 100 });
        map.NewUnits.Add(mapUnit);

        var skin = new UnitObjectData(ObjectDataFormatVersion.v2);
        var skinUnit = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        skinUnit.Modifications.Add(new SimpleObjectDataModification
        { Id = "uhpm".FromRawcode(), Type = ObjectDataType.Int, Value = 999 });
        skin.NewUnits.Add(skinUnit);
        var skinOnly = new SimpleObjectModification { OldId = "hfoo".FromRawcode(), NewId = "H001".FromRawcode() };
        skinOnly.Modifications.Add(new SimpleObjectDataModification
        { Id = "unam".FromRawcode(), Type = ObjectDataType.String, Value = "Skin Knight" });
        skin.NewUnits.Add(skinOnly);

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Serialize(w => w.Write(map)),
            ["war3mapSkin.w3u"] = Serialize(w => w.Write(skin)),
        }));

        // Union by rawcode: the skin's uhpm wins per-field, the map-only unam survives.
        var got = ObjectGetCommand.Execute(doc, ObjectKind.Unit, "H000", ctx: null, preDiagnostics: Array.Empty<string>());
        Assert.Equal("999", got.Fields.Single(f => f.Code == "uhpm").Value);
        Assert.Equal("Dark Paladin", got.Name);

        var listed = ObjectListCommand.Execute(doc, ObjectKind.Unit, ctx: null);
        Assert.Equal(2, listed.Items.Count);
        Assert.Contains(new ObjectListItem("H000", "Hpal", "Dark Paladin"), listed.Items);
        Assert.Contains(new ObjectListItem("H001", "hfoo", "Skin Knight"), listed.Items);
    }

    private static byte[] Serialize(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
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
}
