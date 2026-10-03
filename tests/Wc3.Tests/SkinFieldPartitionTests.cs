// tests/Wc3.Tests/SkinFieldPartitionTests.cs
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// A Reforged map splits one kind's object data across war3map.* and war3mapSkin.*, and decides the
/// split per field code. These cover measuring that split from the map itself and routing an edit to
/// the layer it names, which is what makes a ported object's model, icon and name take effect
/// instead of being merged away.
/// </summary>
public class SkinFieldPartitionTests
{
    private const string Gameplay = "uhpm";   // hit points, a gameplay field
    private const string Art = "umdl";        // model file, a presentation field

    /// <summary>
    /// A two-layer map in the shape a Reforged map has: gameplay fields on the units in
    /// war3map.w3u, presentation fields on the same units in war3mapSkin.w3u. H000 is a custom
    /// derived from Hpal; the numbered filler units establish the per-field majority the way a real
    /// map's thousand-odd objects do.
    /// </summary>
    private static byte[] TwoLayerMap(int fillerUnits = 6, bool includeSkin = true)
    {
        var map = new UnitObjectData(ObjectDataFormatVersion.v3);
        var skin = new UnitObjectData(ObjectDataFormatVersion.v3);

        for (int i = 0; i < fillerUnits; i++)
        {
            var code = $"U00{i}".FromRawcode();
            map.NewUnits.Add(Group("Hpal".FromRawcode(), code, Gameplay, ObjectDataType.Int, 100 + i));
            skin.NewUnits.Add(Group("Hpal".FromRawcode(), code, Art, ObjectDataType.String, $"Units\\F{i}.mdl"));
        }

        // The object under test exists in the gameplay layer only, exactly as a freshly
        // installed object does.
        map.NewUnits.Add(Group("Hpal".FromRawcode(), "H000".FromRawcode(), Gameplay, ObjectDataType.Int, 3600));

        var files = new Dictionary<string, byte[]> { ["war3map.w3u"] = Serialize(w => w.Write(map)) };
        if (includeSkin) files["war3mapSkin.w3u"] = Serialize(w => w.Write(skin));
        return SyntheticMap.Build(files);
    }

    [Fact]
    public void Learns_which_layer_each_field_belongs_in()
    {
        var partition = SkinFieldPartition.Learn(MapDocument.Load(TwoLayerMap()), ObjectKind.Unit);

        Assert.True(partition.HasSkinLayer);
        Assert.Equal(ObjectLayer.Skin, partition.LayerFor(Art));
        Assert.Equal(ObjectLayer.Map, partition.LayerFor(Gameplay));
    }

    [Fact]
    public void A_field_code_the_map_never_uses_routes_to_the_gameplay_layer()
    {
        var partition = SkinFieldPartition.Learn(MapDocument.Load(TwoLayerMap()), ObjectKind.Unit);

        // Unknown to this map, so there is no evidence for the skin layer. The gameplay layer is
        // the safe default: it is where a classic map keeps everything.
        Assert.Equal(ObjectLayer.Map, partition.LayerFor("uxyz"));
    }

    [Fact]
    public void The_split_is_per_field_code_so_a_level_suffix_routes_the_same_way()
    {
        var partition = SkinFieldPartition.Learn(MapDocument.Load(TwoLayerMap()), ObjectKind.Unit);

        Assert.Equal(ObjectLayer.Skin, partition.LayerFor(Art + ":3"));
    }

    [Fact]
    public void A_map_with_no_skin_file_routes_everything_to_war3map()
    {
        var doc = MapDocument.Load(TwoLayerMap(includeSkin: false));
        var partition = SkinFieldPartition.Learn(doc, ObjectKind.Unit);

        Assert.False(partition.HasSkinLayer);
        Assert.Equal(ObjectLayer.Map, partition.LayerFor(Art));
    }

    [Fact]
    public void One_stray_object_does_not_outvote_the_map()
    {
        // The stray is this tool's own earlier output: a single object with the art field in the
        // gameplay layer. The majority has to hold, or the tool would learn its own mistake back.
        var map = new UnitObjectData(ObjectDataFormatVersion.v3);
        var skin = new UnitObjectData(ObjectDataFormatVersion.v3);
        for (int i = 0; i < 5; i++)
            skin.NewUnits.Add(Group("Hpal".FromRawcode(), $"U00{i}".FromRawcode(),
                Art, ObjectDataType.String, $"Units\\F{i}.mdl"));
        map.NewUnits.Add(Group("Hpal".FromRawcode(), "H000".FromRawcode(),
            Art, ObjectDataType.String, "Units\\Stray.mdl"));

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Serialize(w => w.Write(map)),
            ["war3mapSkin.w3u"] = Serialize(w => w.Write(skin)),
        }));

        Assert.Equal(ObjectLayer.Skin, SkinFieldPartition.Learn(doc, ObjectKind.Unit).LayerFor(Art));
    }

    [Fact]
    public void Setting_a_presentation_field_writes_it_into_the_skin_layer()
    {
        var doc = MapDocument.Load(TwoLayerMap());

        var r = ObjectSetCommand.Execute(doc, ObjectKind.Unit, "H000", Art, "war3mapImported\\Ported.mdl");
        Assert.True(r.Ok, r.Message);

        // Reloaded from bytes, so this asserts what the game would read, not in-memory state.
        var reloaded = MapDocument.Load(doc.SaveToBytes());
        Assert.Equal("war3mapImported\\Ported.mdl", Field(reloaded, "war3mapSkin.w3u", "H000", Art));
        Assert.Null(Field(reloaded, "war3map.w3u", "H000", Art));
    }

    [Fact]
    public void The_skin_group_it_creates_carries_the_object_base_so_the_two_layers_agree()
    {
        var doc = MapDocument.Load(TwoLayerMap());
        Assert.True(ObjectSetCommand.Execute(doc, ObjectKind.Unit, "H000", Art, "Units\\X.mdl").Ok);

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        var group = ((UnitObjectData)reloaded.GetFile("war3mapSkin.w3u")!.Model!)
            .NewUnits.Single(u => u.NewId == "H000".FromRawcode());
        Assert.Equal("Hpal".FromRawcode(), group.OldId);
    }

    [Fact]
    public void Setting_a_gameplay_field_still_writes_it_into_war3map()
    {
        var doc = MapDocument.Load(TwoLayerMap());
        Assert.True(ObjectSetCommand.Execute(doc, ObjectKind.Unit, "H000", Gameplay, "4200").Ok);

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        Assert.Equal("4200", Field(reloaded, "war3map.w3u", "H000", Gameplay));
        Assert.Null(Field(reloaded, "war3mapSkin.w3u", "H000", Gameplay));
    }

    [Fact]
    public void An_existing_skin_value_is_edited_in_place_not_shadowed_from_war3map()
    {
        // U000 already carries the art field in the skin layer. Writing the edit to war3map
        // instead would leave the old value on top, so the edit would do nothing in game.
        var doc = MapDocument.Load(TwoLayerMap());
        Assert.True(ObjectSetCommand.Execute(doc, ObjectKind.Unit, "U000", Art, "Units\\Changed.mdl").Ok);

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        Assert.Equal("Units\\Changed.mdl", Field(reloaded, "war3mapSkin.w3u", "U000", Art));
        Assert.Null(Field(reloaded, "war3map.w3u", "U000", Art));
    }

    [Fact]
    public void Editing_only_the_skin_layer_leaves_war3map_byte_identical()
    {
        var bytes = TwoLayerMap();
        var before = MapDocument.Load(bytes).GetFile("war3map.w3u")!.RawBytes;

        var doc = MapDocument.Load(bytes);
        Assert.True(ObjectSetCommand.Execute(doc, ObjectKind.Unit, "H000", Art, "Units\\X.mdl").Ok);

        var after = MapDocument.Load(doc.SaveToBytes()).GetFile("war3map.w3u")!.RawBytes;
        Assert.Equal(before, after);
    }

    private static SimpleObjectModification Group(
        int oldId, int newId, string field, ObjectDataType type, object value)
    {
        var g = new SimpleObjectModification { OldId = oldId, NewId = newId };
        g.Unk.Add(ObjectDataSets.DefaultSetFlags);
        g.Modifications.Add(new SimpleObjectDataModification
        { Id = field.FromRawcode(), Type = type, Value = value });
        return g;
    }

    /// <summary>The field's value in one specific layer, or null when that layer has no such
    /// modification. Deliberately not the merged view: the point is which file it landed in.</summary>
    private static string? Field(MapDocument doc, string file, string rawcode, string field)
    {
        if (doc.GetFile(file)?.Model is not UnitObjectData w3u) return null;
        var group = w3u.NewUnits.Concat(w3u.BaseUnits)
            .FirstOrDefault(u => u.NewId == rawcode.FromRawcode() || u.OldId == rawcode.FromRawcode());
        var mod = group?.Modifications.FirstOrDefault(m => m.Id == field.FromRawcode());
        return mod is null ? null : ObjectKinds.FormatValue(mod.Value);
    }

    private static byte[] Serialize(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }
}
