// tests/Wc3.Tests/ScriptPortTests.cs
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public class ScriptPortTests
{
    // Source: hero H000 with hero-ability A000, and a war3map.j where a spell handler
    // and its InitTrig reference the ability rawcode via a global alias. An unrelated
    // function + global must NOT be carried.
    private const string SourceScript = @"globals
    integer udg_RaidenQ_ID= 'A000'
    integer udg_Unrelated= 5
endglobals
function Trig_RaidenQ_Actions takes nothing returns nothing
    if GetSpellAbilityId() == udg_RaidenQ_ID then
        call KillUnit(GetTriggerUnit())
    endif
endfunction
function InitTrig_RaidenQ takes nothing returns nothing
    local integer i= udg_RaidenQ_ID
    call TriggerAddAction(CreateTrigger(), function Trig_RaidenQ_Actions)
endfunction
function UnrelatedFunc takes nothing returns nothing
    set udg_Unrelated= 99
endfunction
";

    // Target: already has a DIFFERENT Trig_RaidenQ_Actions (name collision → rename) and
    // an InitCustomTriggers (the init-hook target), plus its own globals block. It also
    // already defines H000 and A000 (rawcode collisions → remap + rawcode rewrite).
    private const string TargetScript = @"globals
    integer udg_Foo= 1
endglobals
function Trig_RaidenQ_Actions takes nothing returns nothing
    call BJDebugMsg(""target's own"")
endfunction
function InitCustomTriggers takes nothing returns nothing
    call BJDebugMsg(""init"")
endfunction
function main takes nothing returns nothing
    call InitCustomTriggers()
endfunction
";

    private static MapDocument SourceMap()
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var hero = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        hero.Modifications.Add(new SimpleObjectDataModification
        { Id = "uhab".FromRawcode(), Type = ObjectDataType.String, Value = "A000" });
        w3u.NewUnits.Add(hero);

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        w3a.NewAbilities.Add(new LevelObjectModification { OldId = "ANcl".FromRawcode(), NewId = "A000".FromRawcode() });

        return MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Ser(w => w.Write(w3u)),
            ["war3map.w3a"] = Ser(w => w.Write(w3a)),
            ["war3map.j"] = Encoding.UTF8.GetBytes(SourceScript),
        }));
    }

    private static MapDocument TargetMap()
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        w3u.NewUnits.Add(new SimpleObjectModification { OldId = "Hamg".FromRawcode(), NewId = "H000".FromRawcode() });
        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        w3a.NewAbilities.Add(new LevelObjectModification { OldId = "AHbz".FromRawcode(), NewId = "A000".FromRawcode() });

        return MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Ser(w => w.Write(w3u)),
            ["war3map.w3a"] = Ser(w => w.Write(w3a)),
            ["war3map.j"] = Encoding.UTF8.GetBytes(TargetScript),
        }));
    }

    [Fact]
    public void Ports_the_script_closure_with_rename_rawcode_rewrite_and_init_hook()
    {
        var source = SourceMap();
        var target = TargetMap();
        var bundle = BundleCommand.ResolveUnit(source, "H000", gameDirOverride: null);

        // Wave-3 closure found the two spell functions (and not the unrelated one).
        Assert.Contains(bundle.Functions, f => f.Name == "Trig_RaidenQ_Actions");
        Assert.Contains(bundle.Functions, f => f.Name == "InitTrig_RaidenQ");
        Assert.DoesNotContain(bundle.Functions, f => f.Name == "UnrelatedFunc");

        var result = PortCommand.PortUnit(source, bundle, target);

        Assert.NotNull(result.Script);
        Assert.Equal(2, result.Script!.Functions);
        Assert.Equal(1, result.Script.Globals);          // udg_RaidenQ_ID carried, udg_Unrelated not
        Assert.True(result.Script.Renamed >= 1);          // Trig_RaidenQ_Actions collided → renamed
        Assert.True(result.Script.InitHooked);

        // A ported script only lives in war3map.j (not the wtg/wct trigger tree), so the
        // World Editor would wipe it on save — the result must carry the loud warning.
        Assert.Contains(PortCommand.ScriptDurabilityWarning, result.Warnings);

        var reloaded = MapDocument.Load(target.SaveToBytes());
        var j = Encoding.UTF8.GetString(reloaded.GetFile("war3map.j")!.RawBytes);

        // The ability rawcode was remapped (A000 collided) and rewritten in the ported global.
        string abilTo = result.Remaps.Single(r => r.From == "A000" && r.Kind == ObjectKind.Ability).To;
        Assert.Contains($"udg_RaidenQ_ID= '{abilTo}'", j);
        Assert.DoesNotContain("udg_RaidenQ_ID= 'A000'", j);

        // The unrelated global/function were not carried.
        Assert.DoesNotContain("UnrelatedFunc", j);

        // The colliding function name was renamed in the ported block, and the original
        // target function of that name survives.
        Assert.Contains("Trig_RaidenQ_Actions_p1", j);
        Assert.Contains("call BJDebugMsg(\"target's own\")", j); // original untouched

        // InitTrig was hooked into InitCustomTriggers.
        var initFn = JassFunctionIndex.Parse(j).Single(f => f.Name == "InitCustomTriggers");
        var initBody = string.Join('\n', j.Replace("\r\n", "\n").Split('\n')[(initFn.StartLine - 1)..initFn.EndLine]);
        Assert.Contains("call InitTrig_RaidenQ()", initBody);
    }

    [Fact]
    public void Script_port_is_skipped_cleanly_when_target_has_no_script()
    {
        var source = SourceMap();
        var target = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Ser(w => w.Write(new UnitObjectData(ObjectDataFormatVersion.v2))),
        }));
        var bundle = BundleCommand.ResolveUnit(source, "H000", gameDirOverride: null);
        var result = PortCommand.PortUnit(source, bundle, target);
        // No throw; script info reports it was not ported.
        Assert.True(result.Script is null || result.Script.Functions == 0);
        // Nothing was ported into war3map.j, so the durability warning must NOT appear.
        Assert.DoesNotContain(PortCommand.ScriptDurabilityWarning, result.Warnings);
    }

    [Fact]
    public void Durability_warning_is_absent_when_script_port_is_opted_out()
    {
        var source = SourceMap();
        var target = TargetMap();
        var bundle = BundleCommand.ResolveUnit(source, "H000", gameDirOverride: null);
        var result = PortCommand.PortUnit(source, bundle, target, includeScript: false);
        Assert.Null(result.Script);
        Assert.DoesNotContain(PortCommand.ScriptDurabilityWarning, result.Warnings);
    }

    private static byte[] Ser(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }
}
