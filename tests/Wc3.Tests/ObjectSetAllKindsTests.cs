// tests/Wc3.Tests/ObjectSetAllKindsTests.cs
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// ObjectSetCommand across the non-unit kinds and the three modification shapes:
/// item/buff (Simple), ability (Level), doodad (Variation) — the unit path keeps
/// its own suite in ObjectSetCommandTests.
/// </summary>
public class ObjectSetAllKindsTests
{
    // A synthetic map with one custom object per shape family plus bystander files:
    //   w3a: A000 (base AHbz)  anam@0 "Blizzard X", adur@1 3.5, adur@2 4.5
    //   w3t: I000 (base afac)  igol 150
    //   w3h: B000 (base Bblo)  ftip "Bloodlust X"
    //   w3d: D000 (base YOtf)  dnam@var0 "Torch X", dvar@var2 7
    private static byte[] Sample()
    {
        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        var abil = new LevelObjectModification { OldId = "AHbz".FromRawcode(), NewId = "A000".FromRawcode() };
        abil.Modifications.Add(new LevelObjectDataModification
        { Level = 0, Pointer = 0, Id = "anam".FromRawcode(), Type = ObjectDataType.String, Value = "Blizzard X" });
        abil.Modifications.Add(new LevelObjectDataModification
        { Level = 1, Pointer = 0, Id = "adur".FromRawcode(), Type = ObjectDataType.Unreal, Value = 3.5f });
        abil.Modifications.Add(new LevelObjectDataModification
        { Level = 2, Pointer = 0, Id = "adur".FromRawcode(), Type = ObjectDataType.Unreal, Value = 4.5f });
        w3a.NewAbilities.Add(abil);

        var w3t = new ItemObjectData(ObjectDataFormatVersion.v2);
        var item = new SimpleObjectModification { OldId = "afac".FromRawcode(), NewId = "I000".FromRawcode() };
        item.Modifications.Add(new SimpleObjectDataModification
        { Id = "igol".FromRawcode(), Type = ObjectDataType.Int, Value = 150 });
        w3t.NewItems.Add(item);

        var w3h = new BuffObjectData(ObjectDataFormatVersion.v2);
        var buff = new SimpleObjectModification { OldId = "Bblo".FromRawcode(), NewId = "B000".FromRawcode() };
        buff.Modifications.Add(new SimpleObjectDataModification
        { Id = "ftip".FromRawcode(), Type = ObjectDataType.String, Value = "Bloodlust X" });
        w3h.NewBuffs.Add(buff);

        var w3d = new DoodadObjectData(ObjectDataFormatVersion.v2);
        var dood = new VariationObjectModification { OldId = "YOtf".FromRawcode(), NewId = "D000".FromRawcode() };
        dood.Modifications.Add(new VariationObjectDataModification
        { Variation = 0, Pointer = 0, Id = "dnam".FromRawcode(), Type = ObjectDataType.String, Value = "Torch X" });
        dood.Modifications.Add(new VariationObjectDataModification
        { Variation = 2, Pointer = 0, Id = "dvar".FromRawcode(), Type = ObjectDataType.Int, Value = 7 });
        w3d.NewDoodads.Add(dood);

        return SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3a"] = Serialize(w => w.Write(w3a)),
            ["war3map.w3t"] = Serialize(w => w.Write(w3t)),
            ["war3map.w3h"] = Serialize(w => w.Write(w3h)),
            ["war3map.w3d"] = Serialize(w => w.Write(w3d)),
            ["war3map.j"] = System.Text.Encoding.UTF8.GetBytes(
                "function main takes nothing returns nothing\nendfunction\n"),
            ["mystery.bin"] = new byte[] { 42, 0, 255, 7 },
        });
    }

    // ---- ability (Level shape) ----------------------------------------------

    [Fact]
    public void Ability_set_with_explicit_level_edits_that_level_only()
    {
        var doc = MapDocument.Load(Sample());
        var r = ObjectSetCommand.Execute(doc, ObjectKind.Ability, "A000", "adur:2", "9.25");

        Assert.True(r.Ok, r.Message);
        Assert.Null(r.Warning); // existing modification: type preserved, no inference

        var abil = Ability(MapDocument.Load(doc.SaveToBytes()));
        Assert.Equal(9.25f, LevelMod(abil, "adur", 2).Value);
        Assert.Equal(ObjectDataType.Unreal, LevelMod(abil, "adur", 2).Type);
        Assert.Equal(3.5f, LevelMod(abil, "adur", 1).Value); // level 1 untouched
    }

    [Fact]
    public void Ability_set_bare_code_defaults_to_level_1()
    {
        var doc = MapDocument.Load(Sample());
        Assert.True(ObjectSetCommand.Execute(doc, ObjectKind.Ability, "A000", "adur", "8.0").Ok);

        var abil = Ability(MapDocument.Load(doc.SaveToBytes()));
        Assert.Equal(8f, LevelMod(abil, "adur", 1).Value);
        Assert.Equal(4.5f, LevelMod(abil, "adur", 2).Value); // other levels untouched
    }

    [Fact]
    public void Ability_set_bare_code_prefers_existing_level_0_mod()
    {
        var doc = MapDocument.Load(Sample());
        Assert.True(ObjectSetCommand.Execute(doc, ObjectKind.Ability, "A000", "anam", "Frost Nova").Ok);

        var abil = Ability(MapDocument.Load(doc.SaveToBytes()));
        var mods = abil.Modifications.Where(m => m.Id == "anam".FromRawcode()).ToList();
        Assert.Single(mods); // edited in place at level 0, not duplicated at level 1
        Assert.Equal(0, mods[0].Level);
        Assert.Equal("Frost Nova", mods[0].Value);
    }

    [Fact]
    public void Ability_new_field_at_explicit_level_is_created_with_inferred_type_and_warning()
    {
        var doc = MapDocument.Load(Sample());
        var r = ObjectSetCommand.Execute(doc, ObjectKind.Ability, "A000", "ahdu:3", "2.5");

        Assert.True(r.Ok, r.Message);
        Assert.NotNull(r.Warning);

        var mod = LevelMod(Ability(MapDocument.Load(doc.SaveToBytes())), "ahdu", 3);
        Assert.Equal(ObjectDataType.Unreal, mod.Type);
        Assert.Equal(2.5f, mod.Value);
    }

    // ---- item / buff (Simple shape) ------------------------------------------

    [Fact]
    public void Item_set_existing_int_field_persists_and_keeps_type()
    {
        var doc = MapDocument.Load(Sample());
        var r = ObjectSetCommand.Execute(doc, ObjectKind.Item, "I000", "igol", "300");
        Assert.True(r.Ok, r.Message);

        var w3t = (ItemObjectData)MapDocument.Load(doc.SaveToBytes()).GetFile("war3map.w3t")!.Model!;
        var mod = w3t.NewItems.Single(i => i.NewId == "I000".FromRawcode())
            .Modifications.Single(m => m.Id == "igol".FromRawcode());
        Assert.Equal(ObjectDataType.Int, mod.Type);
        Assert.Equal(300, mod.Value);
    }

    [Fact]
    public void Simple_kind_rejects_level_syntax()
    {
        var doc = MapDocument.Load(Sample());
        var r = ObjectSetCommand.Execute(doc, ObjectKind.Item, "I000", "igol:2", "300");
        Assert.False(r.Ok);
        Assert.False(doc.GetFile("war3map.w3t")!.IsDirty);
    }

    [Fact]
    public void Buff_set_existing_string_field_persists()
    {
        var doc = MapDocument.Load(Sample());
        Assert.True(ObjectSetCommand.Execute(doc, ObjectKind.Buff, "B000", "ftip", "Bloodlust Y").Ok);

        var w3h = (BuffObjectData)MapDocument.Load(doc.SaveToBytes()).GetFile("war3map.w3h")!.Model!;
        var mod = w3h.NewBuffs.Single(b => b.NewId == "B000".FromRawcode())
            .Modifications.Single(m => m.Id == "ftip".FromRawcode());
        Assert.Equal("Bloodlust Y", mod.Value);
    }

    // ---- doodad (Variation shape) ---------------------------------------------

    [Fact]
    public void Doodad_set_bare_code_targets_variation_0()
    {
        var doc = MapDocument.Load(Sample());
        Assert.True(ObjectSetCommand.Execute(doc, ObjectKind.Doodad, "D000", "dnam", "Torch Y").Ok);

        var dood = Doodad(MapDocument.Load(doc.SaveToBytes()));
        var mods = dood.Modifications.Where(m => m.Id == "dnam".FromRawcode()).ToList();
        Assert.Single(mods);
        Assert.Equal(0, mods[0].Variation);
        Assert.Equal("Torch Y", mods[0].Value);
    }

    [Fact]
    public void Doodad_set_with_explicit_variation_edits_that_variation()
    {
        var doc = MapDocument.Load(Sample());
        Assert.True(ObjectSetCommand.Execute(doc, ObjectKind.Doodad, "D000", "dvar:2", "9").Ok);

        var mod = Doodad(MapDocument.Load(doc.SaveToBytes()))
            .Modifications.Single(m => m.Id == "dvar".FromRawcode() && m.Variation == 2);
        Assert.Equal(ObjectDataType.Int, mod.Type);
        Assert.Equal(9, mod.Value);
    }

    // ---- fidelity / layers ------------------------------------------------------

    [Fact]
    public void Editing_an_ability_changes_only_w3a_and_leaves_every_other_file_byte_identical()
    {
        var original = MapDocument.Load(Sample());
        var doc = MapDocument.Load(Sample());
        Assert.True(ObjectSetCommand.Execute(doc, ObjectKind.Ability, "A000", "adur:1", "6.5").Ok);

        var rebuilt = MapDocument.Load(doc.SaveToBytes());
        Assert.Equal(original.PreArchiveData, rebuilt.PreArchiveData);

        var before = ContentFilesByName(original);
        var after = ContentFilesByName(rebuilt);
        Assert.Equal(before.Keys.OrderBy(k => k), after.Keys.OrderBy(k => k));
        foreach (var (name, bytes) in before.Where(kv => kv.Key != "war3map.w3a"))
            Assert.True(after[name].SequenceEqual(bytes), $"unexpected change in {name}");
        Assert.False(after["war3map.w3a"].SequenceEqual(before["war3map.w3a"]),
            "w3a should differ after the edit");
    }

    [Fact]
    public void Skin_only_object_gets_its_group_mirrored_into_the_map_layer()
    {
        // I001 exists only in the Reforged skin twin; the edit must create
        // war3map.w3t with the mirrored group and never rewrite the skin file.
        var skin = new ItemObjectData(ObjectDataFormatVersion.v2);
        var item = new SimpleObjectModification { OldId = "afac".FromRawcode(), NewId = "I001".FromRawcode() };
        item.Modifications.Add(new SimpleObjectDataModification
        { Id = "unam".FromRawcode(), Type = ObjectDataType.String, Value = "Skin Orb" });
        skin.NewItems.Add(item);
        var map = SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3mapSkin.w3t"] = Serialize(w => w.Write(skin)),
            ["war3map.j"] = System.Text.Encoding.UTF8.GetBytes("// noop\n"),
        });

        var original = MapDocument.Load(map);
        var doc = MapDocument.Load(map);
        var r = ObjectSetCommand.Execute(doc, ObjectKind.Item, "I001", "igol", "50");
        Assert.True(r.Ok, r.Message);

        var rebuilt = MapDocument.Load(doc.SaveToBytes());
        var w3t = (ItemObjectData?)rebuilt.GetFile("war3map.w3t")?.Model;
        Assert.NotNull(w3t);
        var group = w3t!.NewItems.Single(i => i.NewId == "I001".FromRawcode());
        Assert.Equal("afac".FromRawcode(), group.OldId);
        Assert.Equal(50, group.Modifications.Single(m => m.Id == "igol".FromRawcode()).Value);

        Assert.True(rebuilt.GetFile("war3mapSkin.w3t")!.RawBytes
            .SequenceEqual(original.GetFile("war3mapSkin.w3t")!.RawBytes), "skin layer must stay untouched");
    }

    [Fact]
    public void Field_the_skin_layer_holds_is_edited_in_the_skin_layer()
    {
        // The skin layer wins field by field, so a value it holds shadows any edit to
        // war3map.*. Anime WOS2 0.32d keeps destructable B017's model as ".mdl .mdl" in
        // war3mapSkin.w3b, and writing bfil to the map layer saved, reported success,
        // and left the merged value exactly as broken as before.
        var skin = new DestructableObjectData(ObjectDataFormatVersion.v2);
        var dest = new SimpleObjectModification { OldId = "YTct".FromRawcode(), NewId = "B017".FromRawcode() };
        dest.Modifications.Add(new SimpleObjectDataModification
        { Id = "bfil".FromRawcode(), Type = ObjectDataType.String, Value = ".mdl .mdl" });
        skin.NewDestructables.Add(dest);
        var map = SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3mapSkin.w3b"] = Serialize(w => w.Write(skin)),
            ["war3map.j"] = System.Text.Encoding.UTF8.GetBytes("// noop\n"),
        });

        var doc = MapDocument.Load(map);
        var r = ObjectSetCommand.Execute(doc, ObjectKind.Destructable, "B017", "bfil", "war3mapImported\\x.mdl");
        Assert.True(r.Ok, r.Message);

        var rebuilt = MapDocument.Load(doc.SaveToBytes());
        var merged = ObjectKinds.MergedEntries(rebuilt, ObjectKinds.Info(ObjectKind.Destructable))
            .Single(e => e.Id == "B017".FromRawcode());
        Assert.Equal("war3mapImported\\x.mdl", ObjectKinds.ModsToDict(merged.Mods)["bfil"]);
    }

    [Fact]
    public void Unknown_object_reports_kind_specific_not_found()
    {
        var doc = MapDocument.Load(Sample());
        var r = ObjectSetCommand.Execute(doc, ObjectKind.Ability, "Xxxx", "adur", "1");
        Assert.False(r.Ok);
        Assert.Equal("ability Xxxx not found in map", r.Message);
        Assert.False(doc.GetFile("war3map.w3a")!.IsDirty);
    }

    // ---- helpers -----------------------------------------------------------------

    private static LevelObjectModification Ability(MapDocument doc) =>
        ((AbilityObjectData)doc.GetFile("war3map.w3a")!.Model!)
            .NewAbilities.Single(a => a.NewId == "A000".FromRawcode());

    private static VariationObjectModification Doodad(MapDocument doc) =>
        ((DoodadObjectData)doc.GetFile("war3map.w3d")!.Model!)
            .NewDoodads.Single(d => d.NewId == "D000".FromRawcode());

    private static LevelObjectDataModification LevelMod(
        LevelObjectModification group, string code, int level) =>
        group.Modifications.Single(m => m.Id == code.FromRawcode() && m.Level == level);

    private static Dictionary<string, byte[]> ContentFilesByName(MapDocument doc) =>
        doc.Files.Where(f => f.FileName != null && !RoundtripCommand.MpqSpecialFiles.Contains(f.FileName!))
                 .ToDictionary(f => f.FileName!, f => f.RawBytes);

    private static byte[] Serialize(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }
}
