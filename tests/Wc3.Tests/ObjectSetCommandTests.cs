// tests/Wc3.Tests/ObjectSetCommandTests.cs
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public class ObjectSetCommandTests
{
    // A synthetic map whose w3u defines custom unit H000 (base Hpal, uagi=20,
    // unam="Dark Paladin") plus a modified standard unit hfoo (uhpm=500), with
    // a bystander script file to prove edits don't leak into other files.
    private static byte[] Sample()
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var custom = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        custom.Modifications.Add(new SimpleObjectDataModification
        { Id = "uagi".FromRawcode(), Type = ObjectDataType.Int, Value = 20 });
        custom.Modifications.Add(new SimpleObjectDataModification
        { Id = "unam".FromRawcode(), Type = ObjectDataType.String, Value = "Dark Paladin" });
        w3u.NewUnits.Add(custom);
        var standard = new SimpleObjectModification { OldId = "hfoo".FromRawcode(), NewId = 0 };
        standard.Modifications.Add(new SimpleObjectDataModification
        { Id = "uhpm".FromRawcode(), Type = ObjectDataType.Int, Value = 500 });
        w3u.BaseUnits.Add(standard);

        return SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Serialize(w => w.Write(w3u)),
            ["war3map.j"] = System.Text.Encoding.UTF8.GetBytes(
                "function main takes nothing returns nothing\nendfunction\n"),
            ["mystery.bin"] = new byte[] { 42, 0, 255, 7 },
        });
    }

    [Fact]
    public void Set_existing_int_field_persists_through_save_and_reload()
    {
        var doc = MapDocument.Load(Sample());
        var r = ObjectSetCommand.Execute(doc, "H000", "uagi", "35");

        Assert.True(r.Ok, r.Message);
        Assert.Equal("set uagi=35 on H000", r.Message);

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        var mod = ModOf(reloaded, "H000", "uagi");
        Assert.Equal(ObjectDataType.Int, mod.Type);
        Assert.Equal(35, mod.Value);
    }

    [Fact]
    public void Set_existing_string_field_keeps_string_type()
    {
        var doc = MapDocument.Load(Sample());
        Assert.True(ObjectSetCommand.Execute(doc, "H000", "unam", "1234").Ok);

        // "1234" looks like an int, but the existing modification is String-typed.
        var mod = ModOf(MapDocument.Load(doc.SaveToBytes()), "H000", "unam");
        Assert.Equal(ObjectDataType.String, mod.Type);
        Assert.Equal("1234", mod.Value);
    }

    [Theory]
    [InlineData("umvs", "270", ObjectDataType.Int)]
    [InlineData("ustp", "1.75", ObjectDataType.Unreal)]
    [InlineData("utip", "Train a hero", ObjectDataType.String)]
    public void Set_new_field_infers_type_from_value_shape(string field, string value, ObjectDataType expected)
    {
        var doc = MapDocument.Load(Sample());
        Assert.True(ObjectSetCommand.Execute(doc, "H000", field, value).Ok);

        var mod = ModOf(MapDocument.Load(doc.SaveToBytes()), "H000", field);
        Assert.Equal(expected, mod.Type);
    }

    [Fact]
    public void Set_reaches_modified_standard_unit_in_base_units()
    {
        var doc = MapDocument.Load(Sample());
        Assert.True(ObjectSetCommand.Execute(doc, "hfoo", "uhpm", "750").Ok);

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        var w3u = (UnitObjectData)reloaded.GetFile("war3map.w3u")!.Model!;
        var unit = w3u.BaseUnits.Single(u => u.OldId == "hfoo".FromRawcode());
        Assert.Equal(750, unit.Modifications.Single(m => m.Id == "uhpm".FromRawcode()).Value);
    }

    [Fact]
    public void Unknown_unit_reports_not_found()
    {
        var doc = MapDocument.Load(Sample());
        var r = ObjectSetCommand.Execute(doc, "Xxxx", "uagi", "1");
        Assert.False(r.Ok);
        Assert.Equal("unit Xxxx not found in map", r.Message);
        Assert.False(doc.GetFile("war3map.w3u")!.IsDirty);
    }

    [Fact]
    public void Value_that_does_not_parse_as_the_existing_type_is_rejected()
    {
        var doc = MapDocument.Load(Sample());
        var r = ObjectSetCommand.Execute(doc, "H000", "uagi", "not-a-number");
        Assert.False(r.Ok);
        Assert.Contains("Int", r.Message);
        Assert.False(doc.GetFile("war3map.w3u")!.IsDirty);
    }

    [Fact]
    public void Editing_w3u_leaves_every_other_file_byte_identical()
    {
        var original = MapDocument.Load(Sample());
        var doc = MapDocument.Load(Sample());
        Assert.True(ObjectSetCommand.Execute(doc, "H000", "uagi", "99").Ok);

        var rebuilt = MapDocument.Load(doc.SaveToBytes());
        Assert.Equal(original.PreArchiveData, rebuilt.PreArchiveData);

        var before = ContentFilesByName(original);
        var after = ContentFilesByName(rebuilt);
        Assert.Equal(before.Keys.OrderBy(k => k), after.Keys.OrderBy(k => k));
        foreach (var (name, bytes) in before.Where(kv => kv.Key != "war3map.w3u"))
            Assert.True(after[name].SequenceEqual(bytes), $"unexpected change in {name}");
        Assert.False(after["war3map.w3u"].SequenceEqual(before["war3map.w3u"]),
            "w3u should differ after the edit");
    }

    /// <summary>
    /// The safety gate on a real map: one field edit must change war3map.w3u and
    /// NOTHING else — every other internal file (named or unnamed) stays
    /// byte-identical, proving an edit cannot corrupt the rest of the map.
    /// </summary>
    [Fact]
    [Trait("Category", "Corpus")]
    public void Real_map_edit_changes_only_w3u_and_the_edit_sticks()
    {
        string path = CorpusMap.PathOrEmpty;
        if (!File.Exists(path)) return;

        var original = MapDocument.Load(path);
        var doc = MapDocument.Load(path);
        var w3u = (UnitObjectData)doc.GetFile("war3map.w3u")!.Model!;

        // Pick a real custom unit and an Int field it already has; bump the value.
        var (unit, mod) = w3u.NewUnits
            .SelectMany(u => u.Modifications
                .Where(m => m.Type == ObjectDataType.Int)
                .Select(m => (u, m)))
            .First();
        string rawcode = unit.NewId.ToRawcode();
        string field = mod.Id.ToRawcode();
        int newValue = (int)mod.Value! + 1;

        var r = ObjectSetCommand.Execute(doc, rawcode, field, newValue.ToString());
        Assert.True(r.Ok, r.Message);

        var rebuilt = MapDocument.Load(doc.SaveToBytes());

        // (a) The edit stuck.
        var reloadedW3u = (UnitObjectData)rebuilt.GetFile("war3map.w3u")!.Model!;
        var reloadedUnit = reloadedW3u.NewUnits.Single(u => u.NewId == unit.NewId);
        Assert.Equal(newValue, reloadedUnit.Modifications.Single(m => m.Id == mod.Id).Value);

        // (b) Every other named file is byte-identical.
        var before = ContentFilesByName(original);
        var after = ContentFilesByName(rebuilt);
        Assert.Equal(before.Keys.OrderBy(k => k), after.Keys.OrderBy(k => k));
        foreach (var (name, bytes) in before.Where(kv => kv.Key != "war3map.w3u"))
            Assert.True(after[name].SequenceEqual(bytes), $"unexpected change in {name}");

        // (c) Unnamed (listfile-stripped) entries survive byte-identically too.
        Assert.Equal(UnnamedMultiset(original), UnnamedMultiset(rebuilt));

        // (d) Header before the archive is untouched.
        Assert.Equal(original.PreArchiveData, rebuilt.PreArchiveData);
    }

    private static SimpleObjectDataModification ModOf(MapDocument doc, string rawcode, string field)
    {
        var w3u = (UnitObjectData)doc.GetFile("war3map.w3u")!.Model!;
        var unit = w3u.NewUnits.Single(u => u.NewId == rawcode.FromRawcode());
        return unit.Modifications.Single(m => m.Id == field.FromRawcode());
    }

    private static Dictionary<string, byte[]> ContentFilesByName(MapDocument doc) =>
        doc.Files.Where(f => f.FileName != null && !RoundtripCommand.MpqSpecialFiles.Contains(f.FileName!))
                 .ToDictionary(f => f.FileName!, f => f.RawBytes);

    private static List<string> UnnamedMultiset(MapDocument doc) =>
        doc.Files.Where(f => f.FileName == null)
                 .Select(f => Convert.ToHexString(f.RawBytes))
                 .OrderBy(s => s, StringComparer.Ordinal).ToList();

    private static byte[] Serialize(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }
}
