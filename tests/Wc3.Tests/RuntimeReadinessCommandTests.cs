// tests/Wc3.Tests/RuntimeReadinessCommandTests.cs
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Hermetic coverage for the runtime readiness check. This exists because a real ported hero
/// audited at zero wiring problems (HeroWiringAudit passed every check) and still did nothing in
/// game, because InitGlobals was dropped by the port and every damage/range/duration value it
/// would have set silently read as its type default. HeroWiringAudit proves a cast reaches a
/// handler, never what that handler computes, this is the check that would have caught it.
/// </summary>
public class RuntimeReadinessCommandTests
{
    // ---- gating: no GUI content means nothing to check -----------------------

    [Fact]
    public void A_script_with_no_globals_and_no_gui_triggers_reports_nothing()
    {
        const string jass = "function main takes nothing returns nothing\nendfunction\n";
        var doc = BuildMap(jass);

        var result = RuntimeReadinessCommand.Check(doc, "H000", ownerId: 0);

        Assert.Empty(result.Findings);
        Assert.True(result.Ready);
    }

    // ---- rule 1: InitGlobals ---------------------------------------------------

    [Fact]
    public void GlobalInitMissing_fires_when_InitGlobals_is_not_defined_at_all()
    {
        const string jass = @"globals
    unit array udg_Foo
endglobals
function InitTrig_Sample takes nothing returns nothing
    call TriggerRegisterAnyUnitEventBJ( gg_trg_Sample, EVENT_PLAYER_UNIT_SPELL_EFFECT )
endfunction
function main takes nothing returns nothing
    call InitTrig_Sample()
endfunction
";
        var doc = BuildMap(jass);

        var result = RuntimeReadinessCommand.Check(doc, "H000", ownerId: 0);

        var f = Assert.Single(result.Findings, f => f.Issue == ReadinessIssue.GlobalInitMissing);
        Assert.Contains("no InitGlobals function at all", f.Detail);
        Assert.False(result.Ready);
    }

    [Fact]
    public void GlobalInitMissing_fires_when_InitGlobals_is_defined_but_never_called()
    {
        const string jass = @"globals
    unit array udg_Foo
endglobals
function InitGlobals takes nothing returns nothing
    set udg_Foo[0]=null
endfunction
function InitTrig_Sample takes nothing returns nothing
    call TriggerRegisterAnyUnitEventBJ( gg_trg_Sample, EVENT_PLAYER_UNIT_SPELL_EFFECT )
endfunction
function main takes nothing returns nothing
    call InitTrig_Sample()
endfunction
";
        var doc = BuildMap(jass);

        var result = RuntimeReadinessCommand.Check(doc, "H000", ownerId: 0);

        var f = Assert.Single(result.Findings, f => f.Issue == ReadinessIssue.GlobalInitMissing);
        Assert.Contains("nothing ever calls it", f.Detail);
    }

    [Fact]
    public void GlobalInitMissing_stays_quiet_when_InitGlobals_is_defined_and_called()
    {
        const string jass = @"globals
    unit array udg_Foo
endglobals
function InitGlobals takes nothing returns nothing
    set udg_Foo[0]=null
endfunction
function InitTrig_Sample takes nothing returns nothing
    call TriggerRegisterAnyUnitEventBJ( gg_trg_Sample, EVENT_PLAYER_UNIT_SPELL_EFFECT )
endfunction
function main takes nothing returns nothing
    call InitGlobals()
    call InitTrig_Sample()
endfunction
";
        var doc = BuildMap(jass);

        var result = RuntimeReadinessCommand.Check(doc, "H000", ownerId: 0);

        Assert.DoesNotContain(result.Findings, f => f.Issue == ReadinessIssue.GlobalInitMissing);
        // Nothing in this script references H000 by rawcode, so the closure is empty and RunInit
        // is still missing, this is the one true positive left, isolating rule 1 from rule 3.
        Assert.Single(result.Findings, f => f.Issue == ReadinessIssue.RunInitializationTriggersMissing);
    }

    // ---- rule 2: RunInitializationTriggers -------------------------------------

    [Fact]
    public void RunInitializationTriggersMissing_stays_quiet_once_it_is_defined_and_called()
    {
        // Same script as the InitGlobals control, plus RunInitializationTriggers wired up too, a
        // fully healthy script end to end. Nothing here references 'H000', so the closure is
        // empty and rule 2 (per-global) has nothing to say either, this is the all-clear case.
        const string jass = @"globals
    unit array udg_Foo
endglobals
function InitGlobals takes nothing returns nothing
    set udg_Foo[0]=null
endfunction
function InitTrig_Sample takes nothing returns nothing
    call TriggerRegisterAnyUnitEventBJ( gg_trg_Sample, EVENT_PLAYER_UNIT_SPELL_EFFECT )
endfunction
function RunInitializationTriggers takes nothing returns nothing
endfunction
function main takes nothing returns nothing
    call InitGlobals()
    call InitTrig_Sample()
    call RunInitializationTriggers()
endfunction
";
        var doc = BuildMap(jass);

        var result = RuntimeReadinessCommand.Check(doc, "H000", ownerId: 0);

        Assert.Empty(result.Findings);
        Assert.True(result.Ready);
    }

    // ---- rule 3: a global read by this hero's own closure, never assigned -----

    [Fact]
    public void GlobalNeverAssigned_fires_for_a_global_this_heros_closure_reads_and_nothing_assigns()
    {
        // The handler names 'H000' directly, so it is part of H000's own dependency closure (the
        // same one BundleCommand computes for porting). udg_TestVar is read there and set nowhere
        // in the whole script, exactly the shape of the real bug, a dropped InitGlobals value.
        const string jass = @"globals
    unit array udg_TestVar
endglobals
function TestHandler takes nothing returns nothing
    if ( GetUnitTypeId(GetTriggerUnit()) == 'H000' ) then
        call RemoveUnit(udg_TestVar[0])
    endif
endfunction
function main takes nothing returns nothing
endfunction
";
        var doc = BuildMap(jass);

        var result = RuntimeReadinessCommand.Check(doc, "H000", ownerId: 0);

        var f = Assert.Single(result.Findings, f => f.Issue == ReadinessIssue.GlobalNeverAssigned);
        Assert.Equal("udg_TestVar", f.Global);
        // Warning, not Error. The hero's own closure on a tightly-coupled arena routinely pulls in
        // shared framework code other heroes' closures reach too, so this can never be proven to
        // be THIS hero's own bug, only a lead worth checking, see Check's rule 3 comment. A map
        // that compiles and round trips byte faithful must not come out INVALID over this alone.
        Assert.Equal(DiagnosticSeverity.Warning, f.Severity);
        Assert.True(result.Ready, "a Warning-only finding must not flip the verdict to not-ready");
    }

    [Fact]
    public void GlobalNeverAssigned_stays_quiet_when_something_assigns_it_anywhere_in_the_script()
    {
        // The assignment lives in a completely different function than the one that reads it,
        // "assigned nowhere in the WHOLE script" is the bar, not "assigned in the same function".
        // main calls SomeOtherSetup so the assignment is LIVE, not merely present in the text, see
        // GlobalNeverAssigned_fires_when_the_assigning_function_is_never_called for the other half.
        const string jass = @"globals
    unit array udg_TestVar
endglobals
function TestHandler takes nothing returns nothing
    if ( GetUnitTypeId(GetTriggerUnit()) == 'H000' ) then
        call RemoveUnit(udg_TestVar[0])
    endif
endfunction
function SomeOtherSetup takes nothing returns nothing
    set udg_TestVar[0]=null
endfunction
function main takes nothing returns nothing
    call SomeOtherSetup()
endfunction
";
        var doc = BuildMap(jass);

        var result = RuntimeReadinessCommand.Check(doc, "H000", ownerId: 0);

        Assert.DoesNotContain(result.Findings, f => f.Issue == ReadinessIssue.GlobalNeverAssigned);
    }

    [Fact]
    public void GlobalNeverAssigned_stays_quiet_when_the_declaration_itself_constructs_a_real_value()
    {
        // A handle-typed global built right in its own declaration ("hashtable udg_TestVar=
        // InitHashtable()") is just as real an initialization as a later "set", this is exactly
        // what tripped up the first version of this check on a real, working map.
        const string jass = @"globals
    hashtable udg_TestVar=InitHashtable()
endglobals
function TestHandler takes nothing returns nothing
    if ( GetUnitTypeId(GetTriggerUnit()) == 'H000' ) then
        call FlushParentHashtable(udg_TestVar)
    endif
endfunction
function main takes nothing returns nothing
endfunction
";
        var doc = BuildMap(jass);

        var result = RuntimeReadinessCommand.Check(doc, "H000", ownerId: 0);

        Assert.DoesNotContain(result.Findings, f => f.Issue == ReadinessIssue.GlobalNeverAssigned);
    }

    [Fact]
    public void GlobalNeverAssigned_fires_for_a_non_udg_global_too()
    {
        // The general shape found on a real map: gg_rct_Base is not a udg_ variable at all (it is
        // the World Editor's own name for a placed region), CreateRegions (the function that would
        // set it) is not carried, and a gate reading GetRectMinX(gg_rct_Base) as (0,0) on a null
        // rect silently blocked every cast. Rule 3 used to only look at udg_ names and would have
        // missed this entirely.
        const string jass = @"globals
    rect gg_rct_Base
endglobals
function TestHandler takes nothing returns nothing
    if ( GetUnitTypeId(GetTriggerUnit()) == 'H000' ) then
        call RemoveUnit(GetRectCenterUnit(gg_rct_Base))
    endif
endfunction
function main takes nothing returns nothing
endfunction
";
        var doc = BuildMap(jass);

        var result = RuntimeReadinessCommand.Check(doc, "H000", ownerId: 0);

        var f = Assert.Single(result.Findings, f => f.Issue == ReadinessIssue.GlobalNeverAssigned);
        Assert.Equal("gg_rct_Base", f.Global);
    }

    [Fact]
    public void GlobalNeverAssigned_stays_quiet_for_a_non_udg_global_assigned_elsewhere()
    {
        // Same shape as GearTimer05 on a real map: a hand-written system's own timer, assigned in
        // its own Init function rather than the conventional InitGlobals. main calls Init, so the
        // assignment is LIVE, not merely present in the text, see
        // GlobalNeverAssigned_fires_when_the_assigning_function_is_never_called for the other half,
        // the shape that actually broke GearTimer03/05/10 once a framework prune stopped calling
        // Init at all.
        const string jass = @"globals
    timer GearTimer05
endglobals
function TestHandler takes nothing returns nothing
    if ( GetUnitTypeId(GetTriggerUnit()) == 'H000' ) then
        call TimerStart(GearTimer05, 1.0, true, null)
    endif
endfunction
function Init takes nothing returns nothing
    set GearTimer05=CreateTimer()
endfunction
function main takes nothing returns nothing
    call Init()
endfunction
";
        var doc = BuildMap(jass);

        var result = RuntimeReadinessCommand.Check(doc, "H000", ownerId: 0);

        Assert.DoesNotContain(result.Findings, f => f.Issue == ReadinessIssue.GlobalNeverAssigned);
    }

    [Fact]
    public void GlobalNeverAssigned_fires_when_the_assigning_function_is_never_called()
    {
        // The exact shape that hid GearTimer03/05/10 and NoDecor_Cond on the Asta corpus port. A
        // "set GearTimer05=CreateTimer()" statement DOES appear in the script text (so the plain
        // "assigned anywhere" scan above stays quiet), but Init, the only function that contains
        // it, is never called, not from main, not from anywhere. The assignment is live in the TEXT
        // and dead at RUNTIME, and this is the check built to tell the two apart.
        const string jass = @"globals
    timer GearTimer05
endglobals
function TestHandler takes nothing returns nothing
    if ( GetUnitTypeId(GetTriggerUnit()) == 'H000' ) then
        call TimerStart(GearTimer05, 1.0, true, null)
    endif
endfunction
function Init takes nothing returns nothing
    set GearTimer05=CreateTimer()
endfunction
function main takes nothing returns nothing
endfunction
";
        var doc = BuildMap(jass);

        var result = RuntimeReadinessCommand.Check(doc, "H000", ownerId: 0);

        var f = Assert.Single(result.Findings, f => f.Issue == ReadinessIssue.GlobalNeverAssigned);
        Assert.Equal("GearTimer05", f.Global);
        Assert.Contains("Init", f.Detail, StringComparison.Ordinal);
        Assert.Contains("never calls", f.Detail, StringComparison.Ordinal);
        // Still a Warning, the same severity rule as every other GlobalNeverAssigned finding.
        Assert.Equal(DiagnosticSeverity.Warning, f.Severity);
        Assert.True(result.Ready, "a Warning-only finding must not flip the verdict to not-ready");
    }

    [Fact]
    public void GlobalNeverAssigned_stays_quiet_when_the_assigning_function_is_reached_through_a_callback()
    {
        // The dead-function check must follow the SAME wiring forms the reachability closure
        // elsewhere in this codebase already recognizes, a TriggerAddAction registering "function
        // Setup" as a callback is just as live as a direct call, once InitCustomTriggers (which
        // main calls) creates the trigger that will eventually run it.
        const string jass = @"globals
    timer GearTimer05
    trigger gg_trg_Setup
endglobals
function TestHandler takes nothing returns nothing
    if ( GetUnitTypeId(GetTriggerUnit()) == 'H000' ) then
        call TimerStart(GearTimer05, 1.0, true, null)
    endif
endfunction
function Setup takes nothing returns nothing
    set GearTimer05=CreateTimer()
endfunction
function InitCustomTriggers takes nothing returns nothing
    set gg_trg_Setup = CreateTrigger()
    call TriggerAddAction(gg_trg_Setup, function Setup)
endfunction
function main takes nothing returns nothing
    call InitCustomTriggers()
endfunction
";
        var doc = BuildMap(jass);

        var result = RuntimeReadinessCommand.Check(doc, "H000", ownerId: 0);

        Assert.DoesNotContain(result.Findings, f => f.Issue == ReadinessIssue.GlobalNeverAssigned);
    }

    [Fact]
    public void GlobalNeverAssigned_ignores_a_variable_outside_this_heros_own_closure()
    {
        // udg_OtherHeroVar is genuinely never assigned anywhere, but nothing that reads it
        // mentions 'H000', so it is not part of THIS hero's closure. A prototype that skipped
        // this scoping found problems like this one on a map the user plays without issue, purely
        // from OTHER heroes' own leftovers, so this is the false positive that made scoping
        // mandatory rather than a nice-to-have.
        const string jass = @"globals
    unit array udg_OtherHeroVar
endglobals
function UnrelatedHandler takes nothing returns nothing
    call RemoveUnit(udg_OtherHeroVar[0])
endfunction
function main takes nothing returns nothing
endfunction
";
        var doc = BuildMap(jass);

        var result = RuntimeReadinessCommand.Check(doc, "H000", ownerId: 0);

        Assert.Empty(result.Findings);
    }

    // ---- synthetic map construction --------------------------------------------

    /// <summary>Hero H000 (a custom Hpal-derived unit, so BundleCommand's root resolves) plus the
    /// given war3map.j.</summary>
    private static MapDocument BuildMap(string jass)
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var hero = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        w3u.NewUnits.Add(hero);

        return MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Ser(w => w.Write(w3u)),
            ["war3map.j"] = Encoding.UTF8.GetBytes(jass),
        }));
    }

    private static byte[] Ser(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }
}
