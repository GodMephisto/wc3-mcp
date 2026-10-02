// tests/Wc3.Tests/ObjectNewCommandTests.cs
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public class ObjectNewCommandTests
{
    // A map already using H000/H001 (units) and A000 (ability) so allocation has
    // real collisions to skip, plus bystanders to prove nothing else is touched.
    private static byte[] Sample()
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        foreach (var code in new[] { "H000", "H001" })
        {
            var unit = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = code.FromRawcode() };
            unit.Modifications.Add(new SimpleObjectDataModification
            { Id = "uagi".FromRawcode(), Type = ObjectDataType.Int, Value = 20 });
            w3u.NewUnits.Add(unit);
        }

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        var abil = new LevelObjectModification { OldId = "AHbz".FromRawcode(), NewId = "A000".FromRawcode() };
        abil.Modifications.Add(new LevelObjectDataModification
        { Level = 1, Pointer = 0, Id = "adur".FromRawcode(), Type = ObjectDataType.Unreal, Value = 3.5f });
        w3a.NewAbilities.Add(abil);

        return SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Serialize(w => w.Write(w3u)),
            ["war3map.w3a"] = Serialize(w => w.Write(w3a)),
            ["war3map.j"] = System.Text.Encoding.UTF8.GetBytes(
                "function main takes nothing returns nothing\nendfunction\n"),
            ["mystery.bin"] = new byte[] { 42, 0, 255, 7 },
        });
    }

    [Fact]
    public void New_unit_allocates_a_fresh_code_skipping_used_ones()
    {
        var doc = MapDocument.Load(Sample());
        var r = ObjectNewCommand.Execute(doc, ObjectKind.Unit, "H000");

        Assert.True(r.Ok, r.Message);
        // H000 and H001 are taken; the allocator preserves the base's leading char.
        Assert.Equal("H002", r.NewRawcode);

        var w3u = (UnitObjectData)MapDocument.Load(doc.SaveToBytes()).GetFile("war3map.w3u")!.Model!;
        var group = w3u.NewUnits.Single(u => u.NewId == "H002".FromRawcode());
        Assert.Equal("H000".FromRawcode(), group.OldId); // derives from the given base
        Assert.Empty(group.Modifications);               // no mods = full inheritance
    }

    [Fact]
    public void New_object_survives_reload_and_lists_as_a_custom_of_its_kind()
    {
        var doc = MapDocument.Load(Sample());
        var r = ObjectNewCommand.Execute(doc, ObjectKind.Ability, "A000");
        Assert.True(r.Ok, r.Message);
        Assert.Equal("A001", r.NewRawcode);

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        var listed = ObjectListCommand.Execute(reloaded, ObjectKind.Ability, ctx: null).Items;
        var item = listed.Single(i => i.Rawcode == "A001");
        Assert.Equal("A000", item.BaseRawcode);
    }

    [Fact]
    public void New_object_of_a_kind_the_map_has_no_file_for_creates_that_file_only()
    {
        var original = MapDocument.Load(Sample());
        var doc = MapDocument.Load(Sample());
        var r = ObjectNewCommand.Execute(doc, ObjectKind.Item, "afac"); // map has no w3t

        Assert.True(r.Ok, r.Message);
        Assert.NotNull(r.NewRawcode);
        Assert.StartsWith("a", r.NewRawcode); // keeps the base's category char

        var rebuilt = MapDocument.Load(doc.SaveToBytes());
        var w3t = (ItemObjectData?)rebuilt.GetFile("war3map.w3t")?.Model;
        Assert.NotNull(w3t);
        Assert.Equal("afac".FromRawcode(), w3t!.NewItems.Single().OldId);

        // Everything that existed before is byte-identical; only w3t is new.
        var before = ContentFilesByName(original);
        var after = ContentFilesByName(rebuilt);
        Assert.Equal(before.Keys.Append("war3map.w3t").OrderBy(k => k), after.Keys.OrderBy(k => k));
        foreach (var (name, bytes) in before)
            Assert.True(after[name].SequenceEqual(bytes), $"unexpected change in {name}");
    }

    [Fact]
    public void Fresh_code_never_equals_the_base_even_when_the_base_is_not_in_the_map()
    {
        var doc = MapDocument.Load(Sample());
        // "h000" is unused by the map; without the guard the allocator's first
        // candidate for base 'h' would be h000 itself.
        var r = ObjectNewCommand.Execute(doc, ObjectKind.Unit, "h000");
        Assert.True(r.Ok, r.Message);
        Assert.NotEqual("h000", r.NewRawcode);
    }

    [Fact]
    public void Successive_creates_allocate_distinct_codes()
    {
        var doc = MapDocument.Load(Sample());
        var first = ObjectNewCommand.Execute(doc, ObjectKind.Unit, "Hpal");
        var second = ObjectNewCommand.Execute(doc, ObjectKind.Unit, "Hpal");

        Assert.True(first.Ok && second.Ok);
        Assert.NotEqual(first.NewRawcode, second.NewRawcode);

        var w3u = (UnitObjectData)MapDocument.Load(doc.SaveToBytes()).GetFile("war3map.w3u")!.Model!;
        Assert.Equal(4, w3u.NewUnits.Count); // H000, H001 + the two new ones
    }

    [Fact]
    public void Invalid_base_rawcode_is_rejected_without_dirtying_the_map()
    {
        var doc = MapDocument.Load(Sample());
        var r = ObjectNewCommand.Execute(doc, ObjectKind.Unit, "toolong");
        Assert.False(r.Ok);
        Assert.False(doc.GetFile("war3map.w3u")!.IsDirty);
    }

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
