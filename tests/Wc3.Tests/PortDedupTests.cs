// tests/Wc3.Tests/PortDedupTests.cs
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Port idempotency: re-porting a unit into a target that already holds last
/// time's port must recognize the collision as a prior port (exact content
/// match) and reuse it — no duplicate objects under fresh rawcodes. Anything
/// actually different remains a true collision and still remaps.
/// </summary>
public class PortDedupTests
{
    // Source mirrors PortCommandTests.SourceMap: custom hero H000 (custom ability
    // A000, an imported icon, a TRIGSTR name) — dedup must match the prior port's
    // INLINED name against the source's raw TRIGSTR reference.
    private static MapDocument SourceMap()
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var hero = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        hero.Modifications.Add(Str("uhab", "A000"));
        hero.Modifications.Add(Str("uico", "war3mapImported\\raidenicon.blp"));
        hero.Modifications.Add(Str("unam", "TRIGSTR_100"));
        w3u.NewUnits.Add(hero);

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        var abil = new LevelObjectModification { OldId = "ANcl".FromRawcode(), NewId = "A000".FromRawcode() };
        abil.Modifications.Add(new LevelObjectDataModification
        { Level = 0, Pointer = 0, Id = "anam".FromRawcode(), Type = ObjectDataType.String, Value = "Naginata Combo" });
        w3a.NewAbilities.Add(abil);

        return MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Ser(w => w.Write(w3u)),
            ["war3map.w3a"] = Ser(w => w.Write(w3a)),
            ["war3map.wts"] = Encoding.UTF8.GetBytes("STRING 100\n{\nRaiden Ei\n}\n"),
            ["war3mapImported\\raidenicon.blp"] = new byte[] { 10, 20, 30, 40 },
        }));
    }

    // Target with NO colliding codes: the first port lands under identity codes,
    // so the second port collides with the first port's own output.
    private static MapDocument EmptyTarget() =>
        MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Ser(w => w.Write(new UnitObjectData(ObjectDataFormatVersion.v2))),
            ["war3map.j"] = Encoding.UTF8.GetBytes("function main takes nothing returns nothing\nendfunction\n"),
        }));

    [Fact]
    public void Re_porting_the_same_unit_is_a_no_op_not_a_duplicator()
    {
        var source = SourceMap();
        var target = EmptyTarget();
        var bundle = BundleCommand.ResolveUnit(source, "H000", gameDirOverride: null);

        var first = PortCommand.PortUnit(source, bundle, target);
        Assert.Equal("H000", first.RootPortedTo); // clean target — no remap needed

        var second = PortCommand.PortUnit(source, bundle, target);

        // The collision was recognized as the prior port: reused, not remapped.
        Assert.Equal("H000", second.RootPortedTo);
        Assert.Empty(second.Remaps);
        Assert.Empty(second.Objects); // nothing was (re-)injected

        // No duplicates landed anywhere in the target's object data.
        var reloaded = MapDocument.Load(target.SaveToBytes());
        var w3u = (UnitObjectData)reloaded.GetFile("war3map.w3u")!.Model!;
        Assert.Equal(1, w3u.NewUnits.Count(u => u.NewId == "H000".FromRawcode()));
        Assert.Equal(1, w3u.BaseUnits.Count + w3u.NewUnits.Count);
        var w3a = (AbilityObjectData)reloaded.GetFile("war3map.w3a")!.Model!;
        Assert.Equal(1, w3a.NewAbilities.Count(a => a.NewId == "A000".FromRawcode()));
        Assert.Equal(1, w3a.BaseAbilities.Count + w3a.NewAbilities.Count);
    }

    [Fact]
    public void An_edited_prior_port_is_a_true_collision_and_still_remaps()
    {
        var source = SourceMap();
        var target = EmptyTarget();
        var bundle = BundleCommand.ResolveUnit(source, "H000", gameDirOverride: null);
        PortCommand.PortUnit(source, bundle, target);

        // The user edits the ported hero in the target: no longer the same object.
        var w3u = (UnitObjectData)target.GetFile("war3map.w3u")!.Model!;
        var hero = w3u.NewUnits.Single(u => u.NewId == "H000".FromRawcode());
        hero.Modifications.Single(m => m.Id == "unam".FromRawcode()).Value = "Edited Raiden";
        target.AddOrReplaceModelFile("war3map.w3u", w3u);

        var second = PortCommand.PortUnit(source, bundle, target);

        // Content differs → fresh code for the hero; the edited copy survives untouched.
        Assert.NotEqual("H000", second.RootPortedTo);
        Assert.Contains(second.Remaps, r => r.From == "H000" && r.Kind == ObjectKind.Unit);
        var after = (UnitObjectData)target.GetFile("war3map.w3u")!.Model!;
        Assert.Equal(1, after.NewUnits.Count(u => u.NewId == "H000".FromRawcode()));
        Assert.Equal(1, after.NewUnits.Count(u => u.NewId == second.RootPortedTo!.FromRawcode()));

        // The un-edited ability WAS identical → deduped: no remap, still exactly one A000.
        Assert.DoesNotContain(second.Remaps, r => r.From == "A000");
        var w3a = (AbilityObjectData)target.GetFile("war3map.w3a")!.Model!;
        Assert.Equal(1, w3a.NewAbilities.Count(a => a.NewId == "A000".FromRawcode()));

        // The re-injected hero still points at the reused ability.
        var ported = after.NewUnits.Single(u => u.NewId == second.RootPortedTo!.FromRawcode());
        Assert.Equal("A000", (string)ported.Modifications.Single(m => m.Id == "uhab".FromRawcode()).Value!);
    }

    [Fact]
    public void Preview_reports_the_same_dedup_plan_without_writing()
    {
        var source = SourceMap();
        var target = EmptyTarget();
        var bundle = BundleCommand.ResolveUnit(source, "H000", gameDirOverride: null);
        PortCommand.PortUnit(source, bundle, target);

        var preview = PortCommand.PreviewPort(source, bundle, target);

        Assert.Equal("H000", preview.RootPortedTo);
        Assert.Empty(preview.Remaps);
        Assert.Empty(preview.Objects);
        // Nothing was written: still exactly one H000 in the target.
        var w3u = (UnitObjectData)target.GetFile("war3map.w3u")!.Model!;
        Assert.Equal(1, w3u.NewUnits.Count(u => u.NewId == "H000".FromRawcode()));
    }

    private static SimpleObjectDataModification Str(string code, string value) =>
        new() { Id = code.FromRawcode(), Type = ObjectDataType.String, Value = value };

    private static byte[] Ser(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }
}
