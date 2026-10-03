// tests/Wc3.Tests/HeroWiringAuditTests.cs
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Hermetic coverage for the two false-positive classes the audit used to report:
/// a dispatch found in an inline condition helper (attached only through a caller), and
/// an unreferenced passive or aura (pure object data, no script) wrongly called inert.
/// </summary>
public class HeroWiringAuditTests
{
    // ---- Bug 1: dispatch attached through a caller, not directly --------------

    [Fact]
    public void Dispatch_in_an_inline_condition_helper_is_wired_via_its_attached_caller()
    {
        // The id check lives in Trig_ChangeWay2_Func001C, which nothing attaches to a trigger. Its
        // caller Trig_ChangeWay2_Actions is the one on gg_trg_ChangeWay2, so the spell is fully wired.
        // The old audit stopped at "the helper is not attached" and reported NotAttachedToTrigger.
        const string jass = @"globals
    trigger gg_trg_ChangeWay2= null
endglobals
function Trig_ChangeWay2_Func001C takes nothing returns boolean
    return ( GetSpellAbilityId() == 'A000' )
endfunction
function Trig_ChangeWay2_Actions takes nothing returns nothing
    if ( not Trig_ChangeWay2_Func001C() ) then
        return
    endif
    call KillUnit(GetTriggerUnit())
endfunction
function InitTrig_ChangeWay2 takes nothing returns nothing
    set gg_trg_ChangeWay2= CreateTrigger()
    call TriggerRegisterAnyUnitEventBJ( gg_trg_ChangeWay2, EVENT_PLAYER_UNIT_SPELL_EFFECT )
    call TriggerAddAction( gg_trg_ChangeWay2, function Trig_ChangeWay2_Actions )
endfunction
function main takes nothing returns nothing
    call InitTrig_ChangeWay2()
endfunction
";
        var doc = BuildMap("A000", jass, new AbilitySpec("A000", "ANcl"));

        var result = HeroWiringAudit.Audit(doc, "H000", ownerId: 0);
        var a000 = result.Abilities.Single(a => a.Ability == "A000");

        Assert.Equal(WiringStatus.Ok, a000.Status);
        // The detail names the function that actually carries the trigger, not just the helper.
        Assert.Contains("via 'Trig_ChangeWay2_Actions'", a000.Detail);
        Assert.Empty(result.Problems);
    }

    [Fact]
    public void Dispatch_no_caller_of_which_is_attached_still_reports_NotAttachedToTrigger()
    {
        // Same inline-helper shape, but nothing in the caller chain is ever attached to a trigger,
        // so the spell genuinely never fires. The caller walk must not paper over that.
        const string jass = @"function Trig_Solo_Func001C takes nothing returns boolean
    return ( GetSpellAbilityId() == 'A000' )
endfunction
function Trig_Solo_Actions takes nothing returns nothing
    if ( not Trig_Solo_Func001C() ) then
        return
    endif
    call KillUnit(GetTriggerUnit())
endfunction
";
        var doc = BuildMap("A000", jass, new AbilitySpec("A000", "ANcl"));

        var result = HeroWiringAudit.Audit(doc, "H000", ownerId: 0);
        var a000 = result.Abilities.Single(a => a.Ability == "A000");

        Assert.Equal(WiringStatus.NotAttachedToTrigger, a000.Status);
        Assert.Contains(result.Problems, p => p.Ability == "A000");
    }

    // ---- Bug 2: unreferenced passives are informational, not inert ------------

    [Fact]
    public void Unreferenced_self_target_aura_is_informational_not_inert()
    {
        // A custom aura (Targets Allowed = self, no cast cost) that the script never names. It works
        // purely from object data, so it must read as passive, not as a NoDispatch fault. This is the
        // A1R5 ("Eye Of Death Perception", based on an Endurance Aura) case from the real map.
        const string jass = "function main takes nothing returns nothing\nendfunction\n";
        var aura = new AbilitySpec("A1R5", "AOae",
            new FieldMod("atar", 1, "self"),
            new FieldMod("acdn", 1, "0"));
        var doc = BuildMap("A1R5", jass, aura);

        var result = HeroWiringAudit.Audit(doc, "H000", ownerId: 0);
        var a = result.Abilities.Single(x => x.Ability == "A1R5");

        Assert.Equal(WiringStatus.NotCastDispatched, a.Status);
        Assert.Empty(result.Problems);
    }

    [Fact]
    public void Unreferenced_base_game_passive_the_map_does_not_define_is_informational()
    {
        // Ault (Ultravision) is a base-game code the map never customizes and the script never names.
        // The engine drives it from the unit's ability list alone, so it is passive, not inert.
        const string jass = "function main takes nothing returns nothing\nendfunction\n";
        var doc = BuildMap("Ault", jass); // no custom ability objects at all

        var result = HeroWiringAudit.Audit(doc, "H000", ownerId: 0);
        var a = result.Abilities.Single(x => x.Ability == "Ault");

        Assert.Equal(WiringStatus.NotCastDispatched, a.Status);
        Assert.Empty(result.Problems);
    }

    [Fact]
    public void Unreferenced_active_spell_with_a_real_target_is_reported_inert()
    {
        // A custom active (Targets Allowed names enemies) that nothing in the script dispatches is a
        // spell the map defines but forgot to wire. That is the genuine fault NoDispatch is for.
        const string jass = "function main takes nothing returns nothing\nendfunction\n";
        var spell = new AbilitySpec("A200", "ANcl",
            new FieldMod("atar", 1, "enemies,ground"),
            new FieldMod("acdn", 1, "8"));
        var doc = BuildMap("A200", jass, spell);

        var result = HeroWiringAudit.Audit(doc, "H000", ownerId: 0);
        var a = result.Abilities.Single(x => x.Ability == "A200");

        Assert.Equal(WiringStatus.NoDispatch, a.Status);
        Assert.Contains(result.Problems, p => p.Ability == "A200");
    }

    // ---- Bug 3: an ability keyed to several triggers, one an inert stub -------

    [Fact]
    public void Ability_wired_by_one_of_two_triggers_reads_as_wired_not_faulted_by_the_stub()
    {
        // Two triggers share the same id condition. gg_trg_Stub has that condition and an empty action
        // but no event registered, an inert duplicate. gg_trg_Real has the same condition, a live
        // action, and a spell event. Keying the ability to a single arbitrary candidate could land on
        // the stub and report NoEventRegistered, the A0K1-on-Tohno false positive. The ability works
        // through gg_trg_Real, so it must read as wired.
        const string jass = @"globals
    trigger gg_trg_Stub= null
    trigger gg_trg_Real= null
endglobals
function Trig_Stub_Conditions takes nothing returns boolean
    return ( GetSpellAbilityId() == 'A000' )
endfunction
function Trig_Stub_Actions takes nothing returns nothing
endfunction
function Trig_Real_Conditions takes nothing returns boolean
    return ( GetSpellAbilityId() == 'A000' )
endfunction
function Trig_Real_Actions takes nothing returns nothing
    call KillUnit(GetTriggerUnit())
endfunction
function InitTrig_Stub takes nothing returns nothing
    set gg_trg_Stub= CreateTrigger()
    call TriggerAddCondition( gg_trg_Stub, Condition( function Trig_Stub_Conditions ) )
    call TriggerAddAction( gg_trg_Stub, function Trig_Stub_Actions )
endfunction
function InitTrig_Real takes nothing returns nothing
    set gg_trg_Real= CreateTrigger()
    call TriggerRegisterAnyUnitEventBJ( gg_trg_Real, EVENT_PLAYER_UNIT_SPELL_EFFECT )
    call TriggerAddCondition( gg_trg_Real, Condition( function Trig_Real_Conditions ) )
    call TriggerAddAction( gg_trg_Real, function Trig_Real_Actions )
endfunction
function main takes nothing returns nothing
    call InitTrig_Stub()
    call InitTrig_Real()
endfunction
";
        var doc = BuildMap("A000", jass, new AbilitySpec("A000", "ANcl"));

        var result = HeroWiringAudit.Audit(doc, "H000", ownerId: 0);
        var a000 = result.Abilities.Single(a => a.Ability == "A000");

        Assert.Equal(WiringStatus.Ok, a000.Status);
        Assert.Contains("gg_trg_Real", a000.Detail);   // credited to the working trigger, not the stub
        Assert.Empty(result.Problems);
    }

    [Fact]
    public void A_doubled_candidate_is_reported_even_when_another_candidate_is_clean()
    {
        // gg_trg_Once completes the chain and builds once. gg_trg_Twice also completes but its init is
        // called twice, so the ability fires twice, a genuine double-fire. A healthy candidate must not
        // mask that, so InitCalledTwice takes precedence over the clean Ok on the other candidate.
        const string jass = @"globals
    trigger gg_trg_Once= null
    trigger gg_trg_Twice= null
endglobals
function Trig_Once_Conditions takes nothing returns boolean
    return ( GetSpellAbilityId() == 'A000' )
endfunction
function Trig_Once_Actions takes nothing returns nothing
    call KillUnit(GetTriggerUnit())
endfunction
function Trig_Twice_Conditions takes nothing returns boolean
    return ( GetSpellAbilityId() == 'A000' )
endfunction
function Trig_Twice_Actions takes nothing returns nothing
    call KillUnit(GetTriggerUnit())
endfunction
function InitTrig_Once takes nothing returns nothing
    set gg_trg_Once= CreateTrigger()
    call TriggerRegisterAnyUnitEventBJ( gg_trg_Once, EVENT_PLAYER_UNIT_SPELL_EFFECT )
    call TriggerAddCondition( gg_trg_Once, Condition( function Trig_Once_Conditions ) )
    call TriggerAddAction( gg_trg_Once, function Trig_Once_Actions )
endfunction
function InitTrig_Twice takes nothing returns nothing
    set gg_trg_Twice= CreateTrigger()
    call TriggerRegisterAnyUnitEventBJ( gg_trg_Twice, EVENT_PLAYER_UNIT_SPELL_EFFECT )
    call TriggerAddCondition( gg_trg_Twice, Condition( function Trig_Twice_Conditions ) )
    call TriggerAddAction( gg_trg_Twice, function Trig_Twice_Actions )
endfunction
function main takes nothing returns nothing
    call InitTrig_Once()
    call InitTrig_Twice()
    call InitTrig_Twice()
endfunction
";
        var doc = BuildMap("A000", jass, new AbilitySpec("A000", "ANcl"));

        var result = HeroWiringAudit.Audit(doc, "H000", ownerId: 0);
        var a000 = result.Abilities.Single(a => a.Ability == "A000");

        Assert.Equal(WiringStatus.InitCalledTwice, a000.Status);
        Assert.Contains("gg_trg_Twice", a000.Detail);
        Assert.Contains(result.Problems, p => p.Ability == "A000");
    }

    [Fact]
    public void Two_different_triggers_each_fully_wired_reports_MultipleLiveDispatchers()
    {
        // gg_trg_Old and gg_trg_wc3ctl_SynthCast_H000 each independently complete the WHOLE chain,
        // own trigger, own event, own init called exactly once, so InitCalledTwice (one trigger built
        // twice) never trips on either. This is the real shape deliverable 1 fixed, a --synth-dispatch
        // hero's own synthesized dispatcher AND the source's shared dispatcher both being live for the
        // same hero, every affected spell fires once per trigger, doubled damage and a caster left
        // permanently paused. A per-candidate "does this ONE chain complete" check reports Ok on the
        // first one it tries and never notices the second, live, path, exactly the blind spot this
        // status exists to close. The synth-named trigger is load-bearing here, this status is scoped
        // to require one (see the guard comment above), a source map's own two native triggers on the
        // same ability are a different, legitimate shape covered by the ChangeWay2-style test below.
        const string jass = @"globals
    trigger gg_trg_Old= null
    trigger gg_trg_wc3ctl_SynthCast_H000= null
endglobals
function Trig_Old_Actions takes nothing returns nothing
    if GetSpellAbilityId() == 'A000' then
        call KillUnit(GetTriggerUnit())
    endif
endfunction
function InitTrig_Old takes nothing returns nothing
    set gg_trg_Old= CreateTrigger()
    call TriggerRegisterAnyUnitEventBJ( gg_trg_Old, EVENT_PLAYER_UNIT_SPELL_EFFECT )
    call TriggerAddAction( gg_trg_Old, function Trig_Old_Actions )
endfunction
function Trig_wc3ctl_SynthCast_H000_Actions takes nothing returns nothing
    if GetSpellAbilityId() == 'A000' then
        call KillUnit(GetTriggerUnit())
    endif
endfunction
function InitTrig_wc3ctl_SynthCast_H000 takes nothing returns nothing
    set gg_trg_wc3ctl_SynthCast_H000= CreateTrigger()
    call TriggerRegisterAnyUnitEventBJ( gg_trg_wc3ctl_SynthCast_H000, EVENT_PLAYER_UNIT_SPELL_EFFECT )
    call TriggerAddAction( gg_trg_wc3ctl_SynthCast_H000, function Trig_wc3ctl_SynthCast_H000_Actions )
endfunction
function main takes nothing returns nothing
    call InitTrig_Old()
    call InitTrig_wc3ctl_SynthCast_H000()
endfunction
";
        var doc = BuildMap("A000", jass, new AbilitySpec("A000", "ANcl"));

        var result = HeroWiringAudit.Audit(doc, "H000", ownerId: 0);
        var a000 = result.Abilities.Single(a => a.Ability == "A000");

        Assert.Equal(WiringStatus.MultipleLiveDispatchers, a000.Status);
        Assert.Contains("gg_trg_Old", a000.Detail);
        Assert.Contains("gg_trg_wc3ctl_SynthCast_H000", a000.Detail);
        Assert.Contains(result.Problems, p => p.Ability == "A000");
    }

    [Fact]
    public void Two_native_triggers_on_the_same_ability_with_no_synth_dispatcher_is_not_flagged()
    {
        // Trig_ChangeWay2 and Trig_Alternate_Start both independently complete the whole chain for
        // A04Z, same event verb, neither testing a foreign ability, exactly the shape the guard above
        // used to misread as a double dispatch. Measured on a real GGGA map, this is Tohno's OWN two
        // complementary "Change Way" triggers (one a plain ability swap, one an elaborate cinematic),
        // present since before any porting touched the map, so a user playing the unported source sees
        // the exact same two firings. Neither trigger is a wc3ctl_SynthCast_* dispatcher, so this must
        // stay Ok, not MultipleLiveDispatchers, a source map's own pre-existing design is not a fault
        // porting introduced.
        const string jass = @"globals
    trigger gg_trg_ChangeWay2= null
    trigger gg_trg_Alternate_Start= null
endglobals
function Trig_ChangeWay2_Actions takes nothing returns nothing
    if GetSpellAbilityId() == 'A04Z' then
        call KillUnit(GetTriggerUnit())
    endif
endfunction
function InitTrig_ChangeWay2 takes nothing returns nothing
    set gg_trg_ChangeWay2= CreateTrigger()
    call TriggerRegisterAnyUnitEventBJ( gg_trg_ChangeWay2, EVENT_PLAYER_UNIT_SPELL_EFFECT )
    call TriggerAddAction( gg_trg_ChangeWay2, function Trig_ChangeWay2_Actions )
endfunction
function Trig_Alternate_Start_Actions takes nothing returns nothing
    if GetSpellAbilityId() == 'A04Z' then
        call KillUnit(GetTriggerUnit())
    endif
endfunction
function InitTrig_Alternate_Start takes nothing returns nothing
    set gg_trg_Alternate_Start= CreateTrigger()
    call TriggerRegisterAnyUnitEventBJ( gg_trg_Alternate_Start, EVENT_PLAYER_UNIT_SPELL_EFFECT )
    call TriggerAddAction( gg_trg_Alternate_Start, function Trig_Alternate_Start_Actions )
endfunction
function main takes nothing returns nothing
    call InitTrig_ChangeWay2()
    call InitTrig_Alternate_Start()
endfunction
";
        var doc = BuildMap("A04Z", jass, new AbilitySpec("A04Z", "ANcl"));

        var result = HeroWiringAudit.Audit(doc, "H000", ownerId: 0);
        var a04z = result.Abilities.Single(a => a.Ability == "A04Z");

        Assert.Equal(WiringStatus.Ok, a04z.Status);
        Assert.DoesNotContain(result.Problems, p => p.Ability == "A04Z");
    }

    [Fact]
    public void A_shared_condition_that_also_tests_a_foreign_ability_is_not_a_double_dispatch()
    {
        // Trig_Shared tests OUR ability (A000) alongside a FOREIGN one (B999, not in this hero's own
        // ability set), the signature of a different hero's ability-mimicry or class-change system
        // that merely happens to also test our id, not a genuine second live path for our own cast.
        // Measured on a real GGGA port, Trig_Battle_Mage_Spell_start (a different hero's ability-copy
        // system) and Trig_ChangeWay2 (a shared stance-swap trigger) both looked like Tohno's OWN
        // A01F/A04Z firing twice until this guard, a false alarm the event-verb check alone did not
        // catch (both were genuinely registered on EVENT_..._SPELL_EFFECT).
        const string jass = @"globals
    trigger gg_trg_Own= null
    trigger gg_trg_Shared= null
endglobals
function Trig_Own_Actions takes nothing returns nothing
    if GetSpellAbilityId() == 'A000' then
        call KillUnit(GetTriggerUnit())
    endif
endfunction
function InitTrig_Own takes nothing returns nothing
    set gg_trg_Own= CreateTrigger()
    call TriggerRegisterAnyUnitEventBJ( gg_trg_Own, EVENT_PLAYER_UNIT_SPELL_EFFECT )
    call TriggerAddAction( gg_trg_Own, function Trig_Own_Actions )
endfunction
function Trig_Shared_Conditions takes nothing returns boolean
    return ( GetSpellAbilityId() == 'A000' ) or ( GetSpellAbilityId() == 'B999' )
endfunction
function Trig_Shared_Actions takes nothing returns nothing
    call KillUnit(GetTriggerUnit())
endfunction
function InitTrig_Shared takes nothing returns nothing
    set gg_trg_Shared= CreateTrigger()
    call TriggerRegisterAnyUnitEventBJ( gg_trg_Shared, EVENT_PLAYER_UNIT_SPELL_EFFECT )
    call TriggerAddCondition( gg_trg_Shared, Condition( function Trig_Shared_Conditions ) )
    call TriggerAddAction( gg_trg_Shared, function Trig_Shared_Actions )
endfunction
function main takes nothing returns nothing
    call InitTrig_Own()
    call InitTrig_Shared()
endfunction
";
        var doc = BuildMap("A000", jass, new AbilitySpec("A000", "ANcl"));

        var result = HeroWiringAudit.Audit(doc, "H000", ownerId: 0);
        var a000 = result.Abilities.Single(a => a.Ability == "A000");

        Assert.Equal(WiringStatus.Ok, a000.Status);
        Assert.Empty(result.Problems);
    }

    [Fact]
    public void Assign_then_compare_with_a_spell_id_local_reads_as_wired()
    {
        const string jass = @"globals
    trigger gg_trg_CastingCheck= null
    integer SomeSpell_ID= 'A010'
endglobals
function Trig_CastingCheck_Actions takes nothing returns nothing
    local integer id = GetSpellAbilityId()
    if id == SomeSpell_ID then
        call KillUnit(GetTriggerUnit())
    endif
endfunction
function InitTrig_CastingCheck takes nothing returns nothing
    set gg_trg_CastingCheck= CreateTrigger()
    call TriggerRegisterAnyUnitEventBJ( gg_trg_CastingCheck, EVENT_PLAYER_UNIT_SPELL_EFFECT )
    call TriggerAddAction( gg_trg_CastingCheck, function Trig_CastingCheck_Actions )
endfunction
function main takes nothing returns nothing
    call InitTrig_CastingCheck()
endfunction
";
        var doc = BuildMap("A010", jass, new AbilitySpec("A010", "ANcl"));

        var result = HeroWiringAudit.Audit(doc, "H000", ownerId: 0);
        var a010 = result.Abilities.Single(a => a.Ability == "A010");

        Assert.Equal(WiringStatus.Ok, a010.Status);
        Assert.Empty(result.Problems);
    }

    [Fact]
    public void Placeholder_then_real_spell_id_assignment_is_tracked_across_all_assignments()
    {
        const string jass = @"globals
    trigger gg_trg_CastingCheck= null
    integer LaterSpell_ID= 'A011'
endglobals
function Trig_CastingCheck_Actions takes nothing returns nothing
    local integer id = 0
    set id = GetSpellAbilityId()
    if LaterSpell_ID == id then
        call KillUnit(GetTriggerUnit())
    endif
endfunction
function InitTrig_CastingCheck takes nothing returns nothing
    set gg_trg_CastingCheck= CreateTrigger()
    call TriggerRegisterAnyUnitEventBJ( gg_trg_CastingCheck, EVENT_PLAYER_UNIT_SPELL_EFFECT )
    call TriggerAddAction( gg_trg_CastingCheck, function Trig_CastingCheck_Actions )
endfunction
function main takes nothing returns nothing
    call InitTrig_CastingCheck()
endfunction
";
        var doc = BuildMap("A011", jass, new AbilitySpec("A011", "ANcl"));

        var result = HeroWiringAudit.Audit(doc, "H000", ownerId: 0);
        var a011 = result.Abilities.Single(a => a.Ability == "A011");

        Assert.Equal(WiringStatus.Ok, a011.Status);
        Assert.Empty(result.Problems);
    }

    [Fact]
    public void Direct_GetSpellAbilityId_literal_dispatch_still_reads_as_wired()
    {
        const string jass = @"globals
    trigger gg_trg_Literal= null
endglobals
function Trig_Literal_Actions takes nothing returns nothing
    if GetSpellAbilityId() == 'A012' then
        call KillUnit(GetTriggerUnit())
    endif
endfunction
function InitTrig_Literal takes nothing returns nothing
    set gg_trg_Literal= CreateTrigger()
    call TriggerRegisterAnyUnitEventBJ( gg_trg_Literal, EVENT_PLAYER_UNIT_SPELL_EFFECT )
    call TriggerAddAction( gg_trg_Literal, function Trig_Literal_Actions )
endfunction
function main takes nothing returns nothing
    call InitTrig_Literal()
endfunction
";
        var doc = BuildMap("A012", jass, new AbilitySpec("A012", "ANcl"));

        var result = HeroWiringAudit.Audit(doc, "H000", ownerId: 0);
        var a012 = result.Abilities.Single(a => a.Ability == "A012");

        Assert.Equal(WiringStatus.Ok, a012.Status);
        Assert.Empty(result.Problems);
    }

    [Fact]
    public void Unrelated_integer_local_does_not_count_as_a_dispatch()
    {
        const string jass = @"globals
    trigger gg_trg_Unrelated= null
    integer UnrelatedSpell_ID= 'A013'
endglobals
function Trig_Unrelated_Actions takes nothing returns nothing
    local integer id = GetUnitTypeId(GetTriggerUnit())
    if id == UnrelatedSpell_ID then
        call KillUnit(GetTriggerUnit())
    endif
endfunction
function InitTrig_Unrelated takes nothing returns nothing
    set gg_trg_Unrelated= CreateTrigger()
    call TriggerRegisterAnyUnitEventBJ( gg_trg_Unrelated, EVENT_PLAYER_UNIT_SPELL_EFFECT )
    call TriggerAddAction( gg_trg_Unrelated, function Trig_Unrelated_Actions )
endfunction
function main takes nothing returns nothing
    call InitTrig_Unrelated()
endfunction
";
        var spell = new AbilitySpec("A013", "ANcl",
            new FieldMod("atar", 1, "enemies,ground"),
            new FieldMod("acdn", 1, "8"));
        var doc = BuildMap("A013", jass, spell);

        var result = HeroWiringAudit.Audit(doc, "H000", ownerId: 0);
        var a013 = result.Abilities.Single(a => a.Ability == "A013");

        Assert.Equal(WiringStatus.NoDispatch, a013.Status);
        Assert.Contains(result.Problems, p => p.Ability == "A013");
    }

    // ---- synthetic map construction ------------------------------------------

    private sealed record FieldMod(string Field, int Level, string Value);
    private sealed record AbilitySpec(string Rawcode, string Base, params FieldMod[] Fields);

    /// <summary>Hero H000 whose hero-ability list is <paramref name="uhab"/>, the given war3map.j,
    /// and one custom ability object per <paramref name="abilities"/> spec (with its field deltas).</summary>
    private static MapDocument BuildMap(string uhab, string jass, params AbilitySpec[] abilities)
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var hero = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        hero.Modifications.Add(new SimpleObjectDataModification
        { Id = "uhab".FromRawcode(), Type = ObjectDataType.String, Value = uhab });
        w3u.NewUnits.Add(hero);

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        foreach (var spec in abilities)
        {
            var mod = new LevelObjectModification
            { OldId = spec.Base.FromRawcode(), NewId = spec.Rawcode.FromRawcode() };
            foreach (var f in spec.Fields)
                mod.Modifications.Add(new LevelObjectDataModification
                { Id = f.Field.FromRawcode(), Type = ObjectDataType.String, Value = f.Value, Level = f.Level, Pointer = 0 });
            w3a.NewAbilities.Add(mod);
        }

        return MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Ser(w => w.Write(w3u)),
            ["war3map.w3a"] = Ser(w => w.Write(w3a)),
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
