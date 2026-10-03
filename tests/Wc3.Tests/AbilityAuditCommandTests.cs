// tests/Wc3.Tests/AbilityAuditCommandTests.cs
// Hermetic coverage for each of the eight per-ability checks. HeroWiringAuditTests already covers
// the dispatch-chain machinery this reuses in depth (including MultipleLiveDispatchers, the
// double-registration shape), these focus on the checks AbilityAuditCommand itself adds: handler
// gutted, loop never started, damage, pause balance, timer callbacks, state, and effects.
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

public class AbilityAuditCommandTests
{
    private static AbilityCheckResult CheckOf(AbilityAuditResult r, AbilityCheck check) =>
        r.Abilities.Single(a => a.Ability == "A0DL").Checks.Single(c => c.Check == check);

    [Fact]
    public void Handler_gutted_by_a_trimmed_line_reports_FAIL()
    {
        var doc = BuildMap(Scaffold(
            "    call StartSpellUnit(GetTriggerUnit())\n"
            + "    //[wc3ctl trimmed] call UnitDamageTarget(GetTriggerUnit(), GetTriggerUnit(), 10., true, false, null, null, null)\n"
            + "    call StopSpellUnit(GetTriggerUnit())\n"));

        var r = AbilityAuditCommand.Audit(doc, "H000", 0);
        var handler = CheckOf(r, AbilityCheck.Handler);

        Assert.Equal(CheckVerdict.Fail, handler.Verdict);
        Assert.Contains("gutted", handler.Detail);
        Assert.Contains("AstaQ_Start", handler.Detail);
    }

    [Fact]
    public void Handler_carried_with_no_trimmed_lines_reports_PASS()
    {
        var doc = BuildMap(Scaffold(
            "    call StartSpellUnit(GetTriggerUnit())\n"
            + "    call StopSpellUnit(GetTriggerUnit())\n"));

        var r = AbilityAuditCommand.Audit(doc, "H000", 0);
        var handler = CheckOf(r, AbilityCheck.Handler);

        Assert.Equal(CheckVerdict.Pass, handler.Verdict);
    }

    [Fact]
    public void Loop_TimerStart_trimmed_reports_FAIL()
    {
        // The exact shape that broke Q2/W2/R2/T2 in the real corpus: TimerStart's own line trimmed
        // because the closure never carried its callback.
        // ScriptPorter's own Trim prepends the marker at column 0, the original line's indentation
        // (and everything else) follows it unchanged, so a real trimmed line never has whitespace
        // BEFORE the marker, only after it.
        var doc = BuildMap(Scaffold(
            "//[wc3ctl trimmed]     call TimerStart(CreateTimer(), 0.03, true, function AstaQ_Loop)\n"));

        var r = AbilityAuditCommand.Audit(doc, "H000", 0);
        var loop = CheckOf(r, AbilityCheck.Loop);

        Assert.Equal(CheckVerdict.Fail, loop.Verdict);
        Assert.Contains("trimmed", loop.Detail);
        Assert.Contains("AstaQ_Loop", loop.Detail);
    }

    [Fact]
    public void Loop_handler_never_declared_reports_FAIL()
    {
        var doc = BuildMap(Scaffold(
            "    call TimerStart(CreateTimer(), 0.03, true, function AstaQ_Loop)\n"));
        // AstaQ_Loop is referenced but never declared anywhere in the script.

        var r = AbilityAuditCommand.Audit(doc, "H000", 0);
        var loop = CheckOf(r, AbilityCheck.Loop);

        Assert.Equal(CheckVerdict.Fail, loop.Verdict);
        Assert.Contains("never declared", loop.Detail);
    }

    [Fact]
    public void Loop_started_live_and_carried_reports_PASS()
    {
        var doc = BuildMap(Scaffold(
            "    call TimerStart(CreateTimer(), 0.03, true, function AstaQ_Loop)\n",
            extraFunctions: "function AstaQ_Loop takes nothing returns nothing\nendfunction\n"));

        var r = AbilityAuditCommand.Audit(doc, "H000", 0);
        var loop = CheckOf(r, AbilityCheck.Loop);

        Assert.Equal(CheckVerdict.Pass, loop.Verdict);
        Assert.Contains("AstaQ_Loop", loop.Detail);
    }

    [Fact]
    public void No_damage_call_reachable_reports_DAMAGE_NO()
    {
        var doc = BuildMap(Scaffold("    call StartSpellUnit(GetTriggerUnit())\n"));

        var r = AbilityAuditCommand.Audit(doc, "H000", 0);
        var dmg = CheckOf(r, AbilityCheck.Damage);

        Assert.Equal(CheckVerdict.Fail, dmg.Verdict);
        Assert.Contains("DAMAGE NO", dmg.Detail);
    }

    [Fact]
    public void A_live_damage_call_reports_DAMAGE_YES()
    {
        var doc = BuildMap(Scaffold(
            "    call UnitDamageTarget(GetTriggerUnit(), GetTriggerUnit(), 10., true, false, null, null, null)\n"));

        var r = AbilityAuditCommand.Audit(doc, "H000", 0);
        var dmg = CheckOf(r, AbilityCheck.Damage);

        Assert.Equal(CheckVerdict.Pass, dmg.Verdict);
        Assert.Contains("DAMAGE YES", dmg.Detail);
    }

    [Fact]
    public void A_trimmed_damage_call_reports_DAMAGE_NO_and_names_the_reason()
    {
        var doc = BuildMap(Scaffold(
            "//[wc3ctl trimmed]     call dmgphys(GetTriggerUnit(), GetTriggerUnit(), 10.)\n"));

        var r = AbilityAuditCommand.Audit(doc, "H000", 0);
        var dmg = CheckOf(r, AbilityCheck.Damage);

        Assert.Equal(CheckVerdict.Fail, dmg.Verdict);
        Assert.Contains("trimmed", dmg.Detail);
    }

    [Fact]
    public void Pause_without_a_matching_unpause_reports_FAIL()
    {
        var doc = BuildMap(Scaffold("    call StartSpellUnit(GetTriggerUnit())\n"));

        var r = AbilityAuditCommand.Audit(doc, "H000", 0);
        var pause = CheckOf(r, AbilityCheck.PauseBalance);

        Assert.Equal(CheckVerdict.Fail, pause.Verdict);
        Assert.Contains("1 pause(s) but only 0 live unpause(s)", pause.Detail);
    }

    [Fact]
    public void Balanced_pause_and_unpause_reports_PASS()
    {
        var doc = BuildMap(Scaffold(
            "    call StartSpellUnit(GetTriggerUnit())\n"
            + "    call StopSpellUnit(GetTriggerUnit())\n"));

        var r = AbilityAuditCommand.Audit(doc, "H000", 0);
        var pause = CheckOf(r, AbilityCheck.PauseBalance);

        Assert.Equal(CheckVerdict.Pass, pause.Verdict);
    }

    [Fact]
    public void An_undeclared_timer_callback_reports_FAIL()
    {
        var doc = BuildMap(Scaffold(
            "    set SomeCallback = function GhostHandler\n"));
        // GhostHandler is referenced but never declared anywhere in the script.

        var r = AbilityAuditCommand.Audit(doc, "H000", 0);
        var cb = CheckOf(r, AbilityCheck.TimerCallbacks);

        Assert.Equal(CheckVerdict.Fail, cb.Verdict);
        Assert.Contains("GhostHandler", cb.Detail);
        Assert.Contains("undeclared", cb.Detail);
    }

    [Fact]
    public void A_trimmed_callback_assignment_reports_FAIL()
    {
        var doc = BuildMap(Scaffold(
            "//[wc3ctl trimmed]     set SomeCallback = function RealHandler\n",
            extraFunctions: "function RealHandler takes nothing returns nothing\nendfunction\n"));

        var r = AbilityAuditCommand.Audit(doc, "H000", 0);
        var cb = CheckOf(r, AbilityCheck.TimerCallbacks);

        Assert.Equal(CheckVerdict.Fail, cb.Verdict);
        Assert.Contains("trimmed", cb.Detail);
    }

    [Fact]
    public void A_declared_live_callback_reports_PASS()
    {
        var doc = BuildMap(Scaffold(
            "    set SomeCallback = function RealHandler\n",
            extraFunctions: "function RealHandler takes nothing returns nothing\nendfunction\n"));

        var r = AbilityAuditCommand.Audit(doc, "H000", 0);
        var cb = CheckOf(r, AbilityCheck.TimerCallbacks);

        Assert.Equal(CheckVerdict.Pass, cb.Verdict);
        Assert.Contains("RealHandler", cb.Detail);
    }

    [Fact]
    public void A_global_never_assigned_anywhere_and_read_by_this_ability_reports_FAIL()
    {
        // udg_AstaQ_Range is declared but never assigned anywhere in the whole script (Shape A), and
        // this ability's own handler reads it, so the finding must attach to THIS ability's row.
        string extraGlobals = "\nglobals\n    real udg_AstaQ_Range= 0.\nendglobals\n";
        var doc = BuildMap(extraGlobals + Scaffold(
            "    call SetUnitX(GetTriggerUnit(), udg_AstaQ_Range)\n"));

        var r = AbilityAuditCommand.Audit(doc, "H000", 0);
        var state = CheckOf(r, AbilityCheck.State);

        Assert.Equal(CheckVerdict.Fail, state.Verdict);
        Assert.Contains("udg_AstaQ_Range", state.Detail);
    }

    [Fact]
    public void A_global_this_ability_never_reads_is_not_attributed_to_it()
    {
        // udg_OtherHero_Range is never assigned either (Shape A too), but this ability's own code
        // never mentions it, so it must not show up on A0DL's row (that would blame the wrong ability).
        string extraGlobals = "\nglobals\n    real udg_OtherHero_Range= 0.\nendglobals\n";
        var doc = BuildMap(extraGlobals + Scaffold(
            "    call StartSpellUnit(GetTriggerUnit())\n"
            + "    call StopSpellUnit(GetTriggerUnit())\n"));

        var r = AbilityAuditCommand.Audit(doc, "H000", 0);
        var state = CheckOf(r, AbilityCheck.State);

        Assert.Equal(CheckVerdict.Pass, state.Verdict);
    }

    [Fact]
    public void A_missing_custom_effect_asset_reports_FAIL()
    {
        var doc = BuildMap(Scaffold(
            "    call DestroyEffect(AddSpecialEffect(\"war3mapImported\\\\missing_fx.mdx\", 0., 0.))\n"));

        var r = AbilityAuditCommand.Audit(doc, "H000", 0);
        var fx = CheckOf(r, AbilityCheck.Effects);

        Assert.Equal(CheckVerdict.Fail, fx.Verdict);
        Assert.Contains("missing_fx.mdx", fx.Detail);
        Assert.Contains("absent", fx.Detail);
    }

    [Fact]
    public void A_base_game_effect_reference_is_never_flagged_absent()
    {
        // Never imported into the map (a real Blizzard path), so absence there means nothing.
        var doc = BuildMap(Scaffold(
            "    call DestroyEffect(AddSpecialEffect(\"Abilities\\\\Spells\\\\Human\\\\Something\\\\Something.mdl\", 0., 0.))\n"));

        var r = AbilityAuditCommand.Audit(doc, "H000", 0);
        var fx = CheckOf(r, AbilityCheck.Effects);

        Assert.Equal(CheckVerdict.Pass, fx.Verdict);
    }

    [Fact]
    public void No_effect_call_at_all_is_PASS_not_a_failure()
    {
        var doc = BuildMap(Scaffold(
            "    call StartSpellUnit(GetTriggerUnit())\n"
            + "    call StopSpellUnit(GetTriggerUnit())\n"));

        var r = AbilityAuditCommand.Audit(doc, "H000", 0);
        var fx = CheckOf(r, AbilityCheck.Effects);

        Assert.Equal(CheckVerdict.Pass, fx.Verdict);
    }

    [Fact]
    public void A_passive_ability_is_marked_NotApplicable_across_every_check()
    {
        // A1R5-shaped self aura: no script presence, no dispatch, exactly the "not this tool's
        // concern" case HeroWiringAudit itself already treats as informational, not broken.
        const string jass = "function main takes nothing returns nothing\nendfunction\n";
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var hero = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        hero.Modifications.Add(new SimpleObjectDataModification
        { Id = "uhab".FromRawcode(), Type = ObjectDataType.String, Value = "A1R5" });
        w3u.NewUnits.Add(hero);

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        var aura = new LevelObjectModification { OldId = "AOae".FromRawcode(), NewId = "A1R5".FromRawcode() };
        aura.Modifications.Add(new LevelObjectDataModification
        { Id = "atar".FromRawcode(), Type = ObjectDataType.String, Value = "self", Level = 1, Pointer = 0 });
        w3a.NewAbilities.Add(aura);

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Ser(w => w.Write(w3u)),
            ["war3map.w3a"] = Ser(w => w.Write(w3a)),
            ["war3map.j"] = Encoding.UTF8.GetBytes(jass),
        }));

        var r = AbilityAuditCommand.Audit(doc, "H000", 0);
        var row = r.Abilities.Single(a => a.Ability == "A1R5");

        Assert.True(row.Pass);
        Assert.All(row.Checks, c => Assert.Equal(CheckVerdict.NotApplicable, c.Verdict));
    }

    [Fact]
    public void An_unsupported_dispatch_shape_marks_every_other_check_NotApplicable_not_a_guess()
    {
        // A castable active (real target, real cooldown) that dispatches through neither a
        // hero-guarded shared dispatcher nor a wc3ctl_SynthCast_* function, an unsupported shape.
        // Dispatch itself still reads a real verdict (NoDispatch, from HeroWiringAudit), but nothing
        // downstream can be verified, and must say so rather than silently reading as PASS.
        const string jass = "function main takes nothing returns nothing\nendfunction\n";
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var hero = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        hero.Modifications.Add(new SimpleObjectDataModification
        { Id = "uhab".FromRawcode(), Type = ObjectDataType.String, Value = "A200" });
        w3u.NewUnits.Add(hero);

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        var spell = new LevelObjectModification { OldId = "ANcl".FromRawcode(), NewId = "A200".FromRawcode() };
        spell.Modifications.Add(new LevelObjectDataModification
        { Id = "atar".FromRawcode(), Type = ObjectDataType.String, Value = "enemies,ground", Level = 1, Pointer = 0 });
        spell.Modifications.Add(new LevelObjectDataModification
        { Id = "acdn".FromRawcode(), Type = ObjectDataType.String, Value = "8", Level = 1, Pointer = 0 });
        w3a.NewAbilities.Add(spell);

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Ser(w => w.Write(w3u)),
            ["war3map.w3a"] = Ser(w => w.Write(w3a)),
            ["war3map.j"] = Encoding.UTF8.GetBytes(jass),
        }));

        var r = AbilityAuditCommand.Audit(doc, "H000", 0);
        var row = r.Abilities.Single(a => a.Ability == "A200");

        Assert.False(row.Pass);
        Assert.Equal(CheckVerdict.Fail, row.Checks.Single(c => c.Check == AbilityCheck.Dispatch).Verdict);
        foreach (var c in row.Checks.Where(c => c.Check != AbilityCheck.Dispatch))
            Assert.Equal(CheckVerdict.NotApplicable, c.Verdict);
    }

    // ---- synthetic map construction --------------------------------------------

    /// <summary>The scaffolding every check-specific test shares: a bare (--synth-dispatch shaped)
    /// dispatcher for ability A0DL wired to a real, live, once-built trigger, calling AstaQ_Start,
    /// whose body is the one thing each test varies.</summary>
    private static string Scaffold(string handlerBody, string extraFunctions = "") =>
        "globals\n"
        + "    trigger gg_trg_X = null\n"
        + "endglobals\n"
        + "function wc3ctl_SynthCast_H000 takes nothing returns nothing\n"
        + "    local integer id = GetSpellAbilityId()\n"
        + "    if id == 'A0DL' then\n"
        + "        call AstaQ_Start()\n"
        + "    endif\n"
        + "endfunction\n"
        + "function AstaQ_Start takes nothing returns nothing\n"
        + handlerBody
        + "endfunction\n"
        + extraFunctions
        + "function InitTrig_X takes nothing returns nothing\n"
        + "    set gg_trg_X = CreateTrigger()\n"
        + "    call TriggerRegisterAnyUnitEventBJ(gg_trg_X, EVENT_PLAYER_UNIT_SPELL_EFFECT)\n"
        + "    call TriggerAddAction(gg_trg_X, function wc3ctl_SynthCast_H000)\n"
        + "endfunction\n"
        + "function main takes nothing returns nothing\n"
        + "    call InitTrig_X()\n"
        + "endfunction\n";

    /// <summary>Hero H000 with one custom active ability A0DL (a real target, a real cooldown, so
    /// HeroWiringAudit never reads it as a passive) and the given war3map.j.</summary>
    private static MapDocument BuildMap(string jass)
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var hero = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        hero.Modifications.Add(new SimpleObjectDataModification
        { Id = "uhab".FromRawcode(), Type = ObjectDataType.String, Value = "A0DL" });
        w3u.NewUnits.Add(hero);

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        var spell = new LevelObjectModification { OldId = "ANcl".FromRawcode(), NewId = "A0DL".FromRawcode() };
        spell.Modifications.Add(new LevelObjectDataModification
        { Id = "atar".FromRawcode(), Type = ObjectDataType.String, Value = "enemies,ground", Level = 1, Pointer = 0 });
        spell.Modifications.Add(new LevelObjectDataModification
        { Id = "acdn".FromRawcode(), Type = ObjectDataType.String, Value = "8", Level = 1, Pointer = 0 });
        w3a.NewAbilities.Add(spell);

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
