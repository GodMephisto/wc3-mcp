// tests/Wc3.Tests/ObjectSetSlotInferenceTests.cs
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Which slot a brand-new modification lands at on a leveled kind.
///
/// A leveled table stores two different kinds of field side by side and records no flag
/// saying which a field is. Per-level data (Cast Range, Cooldown) starts at level 1, while
/// a whole-object field (Requirements, Levels, Hotkey) lives at level 0. Writing a
/// whole-object field to level 1 produces a modification the game never reads, and the
/// write still reports success, so nothing anywhere says the edit did nothing.
///
/// That is not hypothetical. Clearing an inherited Berserker Upgrade requirement across a
/// real map wrote 20 of them to level 1, every one reported "saved", and all 20 were inert
/// in game. The map's own usage of the same field is the signal, since it is available
/// without game-data metadata loaded.
/// </summary>
public class ObjectSetSlotInferenceTests
{
    // Two custom abilities. A001 establishes the map's convention for each field,
    // areq at level 0 (whole-object) and acdn at levels 1 and 2 (per-level). A000
    // carries neither, so a write to A000 has to infer its slot from A001.
    private static byte[] Sample()
    {
        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);

        var bare = new LevelObjectModification { OldId = "AHbz".FromRawcode(), NewId = "A000".FromRawcode() };
        bare.Modifications.Add(new LevelObjectDataModification
        { Level = 0, Pointer = 0, Id = "anam".FromRawcode(), Type = ObjectDataType.String, Value = "Blizzard X" });
        w3a.NewAbilities.Add(bare);

        var conv = new LevelObjectModification { OldId = "AHbz".FromRawcode(), NewId = "A001".FromRawcode() };
        conv.Modifications.Add(new LevelObjectDataModification
        { Level = 0, Pointer = 0, Id = "areq".FromRawcode(), Type = ObjectDataType.String, Value = "" });
        conv.Modifications.Add(new LevelObjectDataModification
        { Level = 1, Pointer = 0, Id = "acdn".FromRawcode(), Type = ObjectDataType.Unreal, Value = 3.5f });
        conv.Modifications.Add(new LevelObjectDataModification
        { Level = 2, Pointer = 0, Id = "acdn".FromRawcode(), Type = ObjectDataType.Unreal, Value = 4.5f });
        w3a.NewAbilities.Add(conv);

        return SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3a"] = Serialize(w => w.Write(w3a)),
            ["war3map.j"] = System.Text.Encoding.UTF8.GetBytes(
                "function main takes nothing returns nothing\nendfunction\n"),
        });
    }

    [Fact]
    public void New_whole_object_field_lands_at_level_0_when_the_map_stores_it_there()
    {
        var doc = MapDocument.Load(Sample());
        var r = ObjectSetCommand.Execute(doc, ObjectKind.Ability, "A000", "areq", "");
        Assert.True(r.Ok, r.Message);

        var mod = Assert.Single(Mods(MapDocument.Load(doc.SaveToBytes()), "A000", "areq"));
        Assert.Equal(0, mod.Level);   // level 1 here is the silent no-op the game never reads
        Assert.Equal("", mod.Value);
    }

    [Fact]
    public void New_per_level_field_still_lands_at_level_1()
    {
        var doc = MapDocument.Load(Sample());
        var r = ObjectSetCommand.Execute(doc, ObjectKind.Ability, "A000", "acdn", "7.0");
        Assert.True(r.Ok, r.Message);

        Assert.Equal(1, Assert.Single(Mods(MapDocument.Load(doc.SaveToBytes()), "A000", "acdn")).Level);
    }

    [Fact]
    public void Unknown_field_with_no_precedent_in_the_map_keeps_the_level_1_default()
    {
        var doc = MapDocument.Load(Sample());
        var r = ObjectSetCommand.Execute(doc, ObjectKind.Ability, "A000", "aran", "500.0");
        Assert.True(r.Ok, r.Message);

        Assert.Equal(1, Assert.Single(Mods(MapDocument.Load(doc.SaveToBytes()), "A000", "aran")).Level);
    }

    [Fact]
    public void Explicit_level_0_always_wins_over_the_inference()
    {
        var doc = MapDocument.Load(Sample());
        var r = ObjectSetCommand.Execute(doc, ObjectKind.Ability, "A000", "acdn:0", "7.0");
        Assert.True(r.Ok, r.Message);

        Assert.Equal(0, Assert.Single(Mods(MapDocument.Load(doc.SaveToBytes()), "A000", "acdn")).Level);
    }

    // ---- helpers -----------------------------------------------------------------

    private static List<LevelObjectDataModification> Mods(MapDocument doc, string rawcode, string field) =>
        ((AbilityObjectData)doc.GetFile("war3map.w3a")!.Model!)
            .NewAbilities.Single(a => a.NewId == rawcode.FromRawcode())
            .Modifications.Where(m => m.Id == field.FromRawcode())
            .Cast<LevelObjectDataModification>().ToList();

    private static byte[] Serialize(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }
}
