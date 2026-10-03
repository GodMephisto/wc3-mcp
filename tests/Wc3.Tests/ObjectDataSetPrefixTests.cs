// tests/Wc3.Tests/ObjectDataSetPrefixTests.cs
using War3Net.Build.Object;
using Wc3.Commands;
using Wc3.Model;
using Probe = Wc3.Tests.ObjectDataBinaryProbe;

namespace Wc3.Tests;

/// <summary>
/// Pins the Reforged (object-data format version 3) per-object modification set prefix.
///
/// The regression these guard against: an object added to a version 3 war3map.w3u/w3a/w3h
/// was written with setCount 0 instead of the one-set prefix { 1, 0 } every other object in
/// the same file carries, so the game resumed parsing the next object from the middle of the
/// added one's data and Warcraft III crashed before main() ran. Nothing caught it for two
/// weeks because validate, lint, roundtrip and pjass all read the bytes back through the
/// same reader that wrote them, where the missing int32 cancelled out. So every assertion
/// here goes through <see cref="ObjectDataBinaryProbe"/>, an independent parser that
/// derails at the same byte the game derails at.
/// </summary>
public class ObjectDataSetPrefixTests
{
    /// <summary>Every kind, so all three War3Net group shapes (Simple, Level, Variation) and
    /// both the plain and the extended modification layouts are covered.</summary>
    public static TheoryData<ObjectKind, string, string> Kinds() => new()
    {
        { ObjectKind.Unit,         "war3map.w3u", "hfoo" },
        { ObjectKind.Item,         "war3map.w3t", "ratf" },
        { ObjectKind.Ability,      "war3map.w3a", "AHbz" },
        { ObjectKind.Destructable, "war3map.w3b", "ATtr" },
        { ObjectKind.Doodad,       "war3map.w3d", "AOhs" },
        { ObjectKind.Buff,         "war3map.w3h", "BSTN" },
        { ObjectKind.Upgrade,      "war3map.w3q", "Rhme" },
    };

    /// <summary>
    /// The core pin. A map whose version 3 object data already holds objects gains one more
    /// through the real command path, and the saved bytes must still parse cleanly end to
    /// end with EVERY object carrying the canonical one-set prefix. Before the fix the added
    /// object declared setCount 0 and the independent parser derailed here.
    /// </summary>
    [Theory]
    [MemberData(nameof(Kinds))]
    public void Added_object_keeps_every_v3_object_readable_by_an_independent_parser(
        ObjectKind kind, string file, string baseCode)
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [file] = ExistingV3File(file, baseCode),
        }));

        var result = ObjectNewCommand.Execute(doc, kind, baseCode);
        Assert.True(result.Ok, result.Message);

        // Give the new object a field too. An empty group is only 12 bytes, so a missing
        // prefix would merely leave a short tail; a group WITH modifications is the shape
        // that made the game resume parsing from the middle of this object's data.
        var set = ObjectSetCommand.Execute(doc, kind, result.NewRawcode!, "unam", "Added");
        Assert.True(set.Ok, set.Message);

        var saved = MapDocument.Load(doc.SaveToBytes()).GetFile(file)!.RawBytes;
        var parsed = Probe.Read(saved, file);   // throws if any object is malformed

        Assert.Equal(3, parsed.Version);
        Assert.Equal(0, parsed.Trailing);
        Assert.Equal(3, parsed.Objects.Count); // the two already there, plus the new one
        Assert.Contains(parsed.Objects, o => o.NewId == result.NewRawcode);
        foreach (var o in parsed.Objects)
        {
            Assert.Equal(1, o.SetCount);
            Assert.Equal(new[] { ObjectDataSets.DefaultSetFlags }, o.SetFlags);
        }
    }

    /// <summary>
    /// Fidelity: re-serializing version 3 object data that nobody touched must reproduce the
    /// original bytes. Seeding the set prefix must not disturb a group that already has one,
    /// otherwise every save would rewrite untouched object data.
    /// </summary>
    [Theory]
    [MemberData(nameof(Kinds))]
    public void Untouched_v3_object_data_round_trips_byte_identical(
        ObjectKind kind, string file, string baseCode)
    {
        _ = kind;
        var original = ExistingV3File(file, baseCode);
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]> { [file] = original }));

        doc.AddOrReplaceModelFile(file, doc.GetFile(file)!.Model!); // same model, pure round-trip
        var saved = MapDocument.Load(doc.SaveToBytes()).GetFile(file)!.RawBytes;

        Assert.Equal(original, saved);
    }

    /// <summary>
    /// Version 1 and 2 object data has no set prefix at all, so seeding must stay away from
    /// it. A stray { 1, 0 } in a classic map's file would corrupt it the same way the missing
    /// one corrupted a Reforged map's.
    /// </summary>
    [Theory]
    [MemberData(nameof(Kinds))]
    public void Added_object_in_a_v2_file_gets_no_set_prefix(ObjectKind kind, string file, string baseCode)
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [file] = Probe.Write(2, file, ExistingObjects(baseCode, file)),
        }));

        var added = ObjectNewCommand.Execute(doc, kind, baseCode);
        Assert.True(added.Ok, added.Message);
        Assert.True(ObjectSetCommand.Execute(doc, kind, added.NewRawcode!, "unam", "Added").Ok);

        var saved = MapDocument.Load(doc.SaveToBytes()).GetFile(file)!.RawBytes;
        var parsed = Probe.Read(saved, file);

        Assert.Equal(2, parsed.Version);
        Assert.Equal(0, parsed.Trailing);
        Assert.Equal(3, parsed.Objects.Count);
        Assert.All(parsed.Objects, o => Assert.Empty(o.SetFlags)); // no prefix in the bytes
    }

    /// <summary>Seeding is idempotent and reports honestly, so a caller could surface "this
    /// model needed N prefixes" without the count drifting on a second pass.</summary>
    [Fact]
    public void EnsureSetPrefixes_seeds_once_and_only_for_v3()
    {
        var v3 = new UnitObjectData(ObjectDataFormatVersion.v3);
        v3.NewUnits.Add(new SimpleObjectModification { OldId = 1, NewId = 2 });
        v3.BaseUnits.Add(new SimpleObjectModification { OldId = 3, NewId = 0 });

        Assert.Equal(2, ObjectDataSets.EnsureSetPrefixes(v3));
        Assert.Equal(0, ObjectDataSets.EnsureSetPrefixes(v3));
        Assert.All(v3.NewUnits.Concat(v3.BaseUnits),
            g => Assert.Equal(new[] { ObjectDataSets.DefaultSetFlags }, g.Unk));

        var v2 = new UnitObjectData(ObjectDataFormatVersion.v2);
        v2.NewUnits.Add(new SimpleObjectModification { OldId = 1, NewId = 2 });
        Assert.Equal(0, ObjectDataSets.EnsureSetPrefixes(v2));
        Assert.Empty(v2.NewUnits[0].Unk);

        Assert.Equal(0, ObjectDataSets.EnsureSetPrefixes("not object data"));
        Assert.Equal(0, ObjectDataSets.EnsureSetPrefixes(null));
    }

    // ---- fixtures -------------------------------------------------------------

    /// <summary>A believable "already saved by the World Editor" version 3 payload: one
    /// in-place modification of a base object and one custom object derived from it.</summary>
    private static byte[] ExistingV3File(string file, string baseCode) =>
        Probe.Write(3, file, ExistingObjects(baseCode, file));

    private static List<(int, string, string, IReadOnlyList<Probe.Mod>)> ExistingObjects(
        string baseCode, string file)
    {
        bool extended = Probe.IsExtended(file);
        IReadOnlyList<Probe.Mod> mods = new[]
        {
            new Probe.Mod("unam", 3, extended ? 0 : null, extended ? 0 : null, "Fixture", 0),
            new Probe.Mod("uhpm", 0, extended ? 1 : null, extended ? 0 : null, 500, 0),
        };
        return new()
        {
            (0, baseCode, "", mods),        // original-object table
            (1, baseCode, "X000", mods),    // custom-object table
        };
    }
}
