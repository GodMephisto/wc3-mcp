// tests/Wc3.Tests/ObjectDataPointerTests.cs
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// The data pointer on a LEVELLED object-data modification, which is the DataA to DataF selector.
///
/// This exists because the writer hardcoded it to 0 when ADDING a level, and nothing in the
/// repository ever read it back. Editing an existing level kept the right pointer because the
/// lookup found the entry, so the defect appeared only when a MISSING level was filled, which is
/// exactly what a level-gap repair does.
///
/// Measured on one real map. The untouched original carries ZERO Data fields mixing pointer 0
/// with a real one across 897 abilities, and the repaired build carried exactly five, every one
/// a level the repair had added. <c>object get</c> reported the value correctly in all five,
/// because it never looks at the pointer, so the toolkit was structurally blind to its own bug
/// and a player found it instead by noticing a level 6 passive that still did nothing.
///
/// The assertion is on the pointer as it survives a real serialize and reload, not on the
/// in-memory object, because a check that never crosses the format proves nothing about the
/// bytes the game reads.
/// </summary>
public class ObjectDataPointerTests
{
    private const string Ability = "A000";

    private static byte[] Serialize(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }

    /// <summary>A map with one 3 level ability carrying <paramref name="field"/> at the given levels.</summary>
    private static MapDocument MapWith(string field, int pointer, params int[] levels)
    {
        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        var a = new LevelObjectModification
        { OldId = "ANca".FromRawcode(), NewId = Ability.FromRawcode() };
        a.Modifications.Add(new LevelObjectDataModification
        {
            Level = 0,
            Pointer = 0,
            Id = "alev".FromRawcode(),
            Type = ObjectDataType.Int,
            Value = 3,
        });
        foreach (int lv in levels)
            a.Modifications.Add(new LevelObjectDataModification
            {
                Level = lv,
                Pointer = pointer,
                Id = field.FromRawcode(),
                Type = ObjectDataType.Unreal,
                Value = 0.1f * lv,
            });
        w3a.NewAbilities.Add(a);

        return MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3a"] = Serialize(w => w.Write(w3a)),
        }));
    }

    /// <summary>Every (level, pointer) that field carries, read back out of the SAVED bytes.</summary>
    private static Dictionary<int, int> PointersAfterSave(MapDocument doc, string field)
    {
        var reloaded = MapDocument.Load(doc.SaveToBytes());
        var file = reloaded.GetFile("war3map.w3a")
                   ?? throw new InvalidOperationException("no war3map.w3a after save");
        var raw = file.OverrideBytes ?? file.RawBytes;
        using var reader = new BinaryReader(new MemoryStream(raw));
        var w3a = reader.ReadAbilityObjectData();
        var entry = w3a.NewAbilities.Single(x => x.NewId == Ability.FromRawcode());
        return entry.Modifications
            .Where(m => m.Id == field.FromRawcode())
            .ToDictionary(m => m.Level, m => m.Pointer);
    }

    private static void Set(MapDocument doc, string field, string value)
    {
        var r = ObjectSetCommand.Execute(doc, ObjectKind.Ability, Ability, field, value);
        Assert.True(r.Ok, r.Message);
    }

    [Fact]
    public void The_fixture_really_does_start_with_a_consistent_non_zero_pointer()
    {
        // The control. If the fixture wrote pointer 0 to begin with, every test below would
        // pass on broken code, which is the tautology this file exists to avoid.
        var before = PointersAfterSave(MapWith("nca1", pointer: 1, 1, 2), "nca1");
        Assert.Equal(new[] { 1, 2 }, before.Keys.OrderBy(k => k));
        Assert.All(before.Values, p => Assert.Equal(1, p));
    }

    [Fact]
    public void An_ADDED_level_inherits_the_pointer_the_field_already_uses()
    {
        // The defect itself. Levels 1 and 2 use DataA (pointer 1), level 3 is missing, and a
        // repair fills it. The added level has to mean the same column as its siblings.
        var doc = MapWith("nca1", pointer: 1, 1, 2);
        Set(doc, "nca1:3", "0.8");

        var after = PointersAfterSave(doc, "nca1");
        Assert.Equal(new[] { 1, 2, 3 }, after.Keys.OrderBy(k => k));
        Assert.Equal(1, after[3]);
        Assert.Single(after.Values.Distinct());          // no level disagrees with the rest
    }

    [Fact]
    public void A_field_using_a_later_Data_column_keeps_that_column_too()
    {
        // Pointer 5 is DataE, and one real ability on the map in question uses it. Copying
        // "the first pointer seen anywhere" rather than "this field's pointer" would put it
        // back to 1 here, so the fixture deliberately does not use 1.
        var doc = MapWith("Nbf5", pointer: 5, 1, 2);
        Set(doc, "Nbf5:3", "42");

        Assert.Equal(5, PointersAfterSave(doc, "Nbf5")[3]);
    }

    [Fact]
    public void Editing_an_existing_level_still_leaves_its_pointer_alone()
    {
        // The half that already worked, pinned so the fix cannot regress it.
        var doc = MapWith("nca1", pointer: 1, 1, 2);
        Set(doc, "nca1:2", "0.9");

        Assert.Equal(1, PointersAfterSave(doc, "nca1")[2]);
    }

    [Fact]
    public void A_genuinely_new_field_with_no_sibling_gets_zero()
    {
        // Nothing to copy, and 0 is what an ordinary non-Data field carries, so 0 is correct
        // here rather than a fallback quietly hiding a miss.
        var doc = MapWith("nca1", pointer: 1, 1, 2);
        Set(doc, "acdn:1", "12");

        Assert.Equal(0, PointersAfterSave(doc, "acdn")[1]);
    }
}
