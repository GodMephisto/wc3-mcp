// tests/Wc3.Tests/ObjectFidelityTests.cs
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Build.Script;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Hermetic coverage for the object-data fidelity comparison: a faithful carry produces no loss, a
/// dropped per-level value or dropped level counts as a loss, a weakened numeric value is a loss, a
/// dropped numeric field is a loss, and the two legitimate port transformations (a remapped rawcode
/// reference and a trigger string inlined to the same text) are not losses. Runs with no game data
/// (ctx null) so the closure crawl never touches the local install.
/// </summary>
public class ObjectFidelityTests
{
    [Fact]
    public void Identical_source_and_target_report_faithful()
    {
        var abil = new Abil("A000", "ANcl", new AF("Idam", 1, "100"), new AF("Idam", 2, "200"));
        var src = Map("A000", new[] { abil }, Array.Empty<Buff>());
        var tgt = Map("A000", new[] { abil }, Array.Empty<Buff>());

        var r = ObjectFidelityCommand.Compare(src, tgt, "H000", ctx: null);

        Assert.True(r.Faithful, string.Join(" | ", r.Findings.Select(f => f.Detail)));
        Assert.Equal(0, r.Errors);
        Assert.True(r.ObjectsCompared >= 2); // the hero and its ability
    }

    [Fact]
    public void A_dropped_level_is_a_loss()
    {
        var srcAbil = new Abil("A000", "ANcl",
            new AF("Idam", 1, "100"), new AF("Idam", 2, "200"), new AF("Idam", 3, "300"));
        var tgtAbil = new Abil("A000", "ANcl",
            new AF("Idam", 1, "100"), new AF("Idam", 2, "200")); // level 3 dropped
        var src = Map("A000", new[] { srcAbil }, Array.Empty<Buff>());
        var tgt = Map("A000", new[] { tgtAbil }, Array.Empty<Buff>());

        var r = ObjectFidelityCommand.Compare(src, tgt, "H000", ctx: null);

        Assert.False(r.Faithful);
        Assert.Contains(r.Findings, f => f.SourceRawcode == "A000"
            && f.Issue == FidelityIssue.LevelEntryMissing && f.Field == "Idam:3"
            && f.Severity == FidelitySeverity.Error);
        Assert.Contains(r.Findings, f => f.SourceRawcode == "A000"
            && f.Issue == FidelityIssue.LevelsDropped && f.Severity == FidelitySeverity.Error);
    }

    [Fact]
    public void A_weakened_numeric_value_is_a_loss()
    {
        var src = Map("A000", new[] { new Abil("A000", "ANcl", new AF("Idam", 1, "300")) }, Array.Empty<Buff>());
        var tgt = Map("A000", new[] { new Abil("A000", "ANcl", new AF("Idam", 1, "150")) }, Array.Empty<Buff>());

        var r = ObjectFidelityCommand.Compare(src, tgt, "H000", ctx: null);

        Assert.False(r.Faithful);
        var f = Assert.Single(r.Findings, x => x.Issue == FidelityIssue.NumericValueDiff);
        Assert.Equal("Idam:1", f.Field);
        Assert.Equal("300", f.SourceValue);
        Assert.Equal("150", f.TargetValue);
        Assert.Equal(FidelitySeverity.Error, f.Severity);
    }

    [Fact]
    public void A_dropped_numeric_field_is_a_loss_but_a_dropped_text_field_is_not()
    {
        var src = Map("A000", new[]
        {
            new Abil("A000", "ANcl", new AF("acdn", 0, "8"), new AF("ansf", 0, "custom suffix")),
        }, Array.Empty<Buff>());
        var tgt = Map("A000", new[] { new Abil("A000", "ANcl") }, Array.Empty<Buff>()); // both fields gone

        var r = ObjectFidelityCommand.Compare(src, tgt, "H000", ctx: null);

        Assert.False(r.Faithful);
        // The numeric cooldown is a real loss.
        Assert.Contains(r.Findings, f => f.Field == "acdn"
            && f.Issue == FidelityIssue.FieldMissing && f.Severity == FidelitySeverity.Error);
        // The text suffix is cosmetic, reported for information only.
        Assert.Contains(r.Findings, f => f.Field == "ansf"
            && f.Issue == FidelityIssue.FieldMissing && f.Severity == FidelitySeverity.Info);
    }

    [Fact]
    public void A_remapped_rawcode_reference_is_informational_not_a_loss()
    {
        // The ability points at buff B000 in the source and B001 in the target, both custom buffs that
        // exist identically in both maps. That is what a rawcode remap on collision looks like, so it
        // is information, not a loss.
        var buffs = new[]
        {
            new Buff("B000", "BSTN", new BF("fnam", "Poison")),
            new Buff("B001", "BSTN", new BF("fnam", "Poison")),
        };
        var src = Map("A000", new[] { new Abil("A000", "ANcl", new AF("abuf", 1, "B000")) }, buffs);
        var tgt = Map("A000", new[] { new Abil("A000", "ANcl", new AF("abuf", 1, "B001")) }, buffs);

        var r = ObjectFidelityCommand.Compare(src, tgt, "H000", ctx: null);

        Assert.True(r.Faithful, string.Join(" | ", r.Findings.Select(f => $"{f.Issue} {f.Field}")));
        Assert.Contains(r.Findings, f => f.Field == "abuf:1"
            && f.Issue == FidelityIssue.ReferenceValueDiff && f.Severity == FidelitySeverity.Info);
    }

    [Fact]
    public void A_trigger_string_inlined_to_the_same_text_is_not_a_difference()
    {
        // Source names the ability with a TRIGSTR_ reference resolving to "Fireball"; the port inlined
        // that to the literal "Fireball" in the target. Same text, so there must be no finding at all.
        var src = Map("A000", new[] { new Abil("A000", "ANcl", new AF("anam", 0, "TRIGSTR_005")) },
            Array.Empty<Buff>(), (5u, "Fireball"));
        var tgt = Map("A000", new[] { new Abil("A000", "ANcl", new AF("anam", 0, "Fireball")) },
            Array.Empty<Buff>());

        var r = ObjectFidelityCommand.Compare(src, tgt, "H000", ctx: null);

        Assert.True(r.Faithful);
        Assert.DoesNotContain(r.Findings, f => f.Field == "anam");
    }

    [Fact]
    public void An_object_in_the_source_closure_absent_from_the_target_is_a_loss()
    {
        // The ability references buff B000 in both maps, but the target never defines B000 as an object.
        var src = Map("A000", new[] { new Abil("A000", "ANcl", new AF("abuf", 1, "B000")) },
            new[] { new Buff("B000", "BSTN", new BF("fnam", "Poison")) });
        var tgt = Map("A000", new[] { new Abil("A000", "ANcl", new AF("abuf", 1, "B000")) },
            Array.Empty<Buff>()); // B000 not defined here

        var r = ObjectFidelityCommand.Compare(src, tgt, "H000", ctx: null);

        Assert.False(r.Faithful);
        var f = Assert.Single(r.Findings, x => x.Issue == FidelityIssue.ObjectAbsent);
        Assert.Equal("B000", f.SourceRawcode);
        Assert.Equal(ObjectKind.Buff, f.Kind);
        Assert.Equal(FidelitySeverity.Error, f.Severity);
    }

    // ---- synthetic map construction ------------------------------------------

    private sealed record AF(string Field, int Level, string Value);   // one ability field delta
    private sealed record BF(string Field, string Value);              // one buff field delta
    private sealed record Abil(string Rawcode, string Base, params AF[] Fields);
    private sealed record Buff(string Rawcode, string Base, params BF[] Fields);

    /// <summary>Hero H000 (uabi = <paramref name="uabi"/>) plus the given custom abilities and buffs,
    /// and an optional war3map.wts for TRIGSTR resolution.</summary>
    private static MapDocument Map(string uabi, IEnumerable<Abil> abilities, IEnumerable<Buff> buffs,
        params (uint Key, string Value)[] wts)
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var hero = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        hero.Modifications.Add(new SimpleObjectDataModification
        { Id = "uabi".FromRawcode(), Type = ObjectDataType.String, Value = uabi });
        w3u.NewUnits.Add(hero);

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        foreach (var a in abilities)
        {
            var mod = new LevelObjectModification { OldId = a.Base.FromRawcode(), NewId = a.Rawcode.FromRawcode() };
            foreach (var f in a.Fields)
                mod.Modifications.Add(new LevelObjectDataModification
                { Id = f.Field.FromRawcode(), Type = ObjectDataType.String, Value = f.Value, Level = f.Level, Pointer = 0 });
            w3a.NewAbilities.Add(mod);
        }

        var w3h = new BuffObjectData(ObjectDataFormatVersion.v2);
        foreach (var b in buffs)
        {
            var mod = new SimpleObjectModification { OldId = b.Base.FromRawcode(), NewId = b.Rawcode.FromRawcode() };
            foreach (var f in b.Fields)
                mod.Modifications.Add(new SimpleObjectDataModification
                { Id = f.Field.FromRawcode(), Type = ObjectDataType.String, Value = f.Value });
            w3h.NewBuffs.Add(mod);
        }

        var files = new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Ser(w => w.Write(w3u)),
            ["war3map.w3a"] = Ser(w => w.Write(w3a)),
            ["war3map.w3h"] = Ser(w => w.Write(w3h)),
        };
        if (wts.Length > 0) files["war3map.wts"] = WtsBytes(wts);
        return MapDocument.Load(SyntheticMap.Build(files));
    }

    private static byte[] WtsBytes((uint Key, string Value)[] entries)
    {
        var wts = new TriggerStrings();
        foreach (var (key, value) in entries)
            wts.Strings.Add(new TriggerString { Key = key, Value = value });
        using var ms = new MemoryStream();
        using (var sw = new StreamWriter(ms, Encoding.UTF8, leaveOpen: true)) sw.WriteTriggerStrings(wts);
        return ms.ToArray();
    }

    private static byte[] Ser(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }
}
