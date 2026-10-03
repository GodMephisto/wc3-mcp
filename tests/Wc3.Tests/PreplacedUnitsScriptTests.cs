// tests/Wc3.Tests/PreplacedUnitsScriptTests.cs
// A map with a custom war3map.j spawns its preplaced widgets from generated JASS
// (CreateAllUnits/CreateAllItems), not from war3mapUnits.doo directly. These guard that
// placing through wc3ctl wires that script in, keeps it idempotent, and never clobbers a
// map that already ships its own creation script.
using System.Text;
using Wc3.Commands;
using Wc3.Commands.Editing;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

public class PreplacedUnitsScriptTests
{
    // war3map.j as it stands after a save/reload, byte-faithful (Latin1 round-trips exactly).
    private static string ScriptOf(MapDocument doc)
    {
        byte[] saved = doc.SaveToBytes();
        var reloaded = MapDocument.Load(saved);
        return Encoding.Latin1.GetString(reloaded.GetFile(PreplacedUnitsScript.ScriptFile)!.RawBytes);
    }

    private static int Count(string haystack, string needle)
    {
        int n = 0, i = 0;
        while ((i = haystack.IndexOf(needle, i, StringComparison.Ordinal)) >= 0) { n++; i += needle.Length; }
        return n;
    }

    [Fact]
    public void PlacingAUnit_WiresCreateAllUnitsIntoMain()
    {
        var doc = BlankMap.Create();
        PlacementCommand.PlaceUnit(doc, "hfoo", ownerId: 0, x: 128f, y: -256f);

        string j = ScriptOf(doc);
        Assert.Contains("function CreateAllUnits takes nothing returns nothing", j);
        Assert.Contains("CreateUnit(Player(0), 'hfoo', 128.0, -256.0", j);
        Assert.Contains("call CreateAllUnits(  )", j);
    }

    [Fact]
    public void PlacingTwoUnits_ProducesOneFunctionAndOneCall()
    {
        var doc = BlankMap.Create();
        PlacementCommand.PlaceUnit(doc, "hfoo", 0, 0f, 0f);
        PlacementCommand.PlaceUnit(doc, "hkni", 1, 64f, 0f);

        string j = ScriptOf(doc);
        Assert.Equal(1, Count(j, "function CreateAllUnits takes nothing"));
        Assert.Equal(1, Count(j, "call CreateAllUnits("));
        Assert.Contains("'hfoo'", j);
        Assert.Contains("Player(1), 'hkni'", j);
    }

    [Fact]
    public void PlacingAnItem_WiresCreateAllItemsNotUnits()
    {
        var doc = BlankMap.Create();
        PlacementCommand.PlaceItem(doc, "ratf", x: 100f, y: 200f);

        string j = ScriptOf(doc);
        Assert.Contains("function CreateAllItems takes nothing returns nothing", j);
        Assert.Contains("CreateItem('ratf', 100.0, 200.0)", j);
        Assert.Contains("call CreateAllItems(  )", j);
        // Only an item was placed, so no unit creation is emitted.
        Assert.DoesNotContain("function CreateAllUnits", j);
    }

    [Fact]
    public void RemovingTheLastUnit_ClearsTheGeneratedBlockAndCall()
    {
        var doc = BlankMap.Create();
        var r = PlacementCommand.PlaceUnit(doc, "hfoo", 0, 0f, 0f);
        Assert.Contains("call CreateAllUnits(  )", ScriptOf(doc));

        new RemoveUnitEdit(r.CreationNumber).Apply(doc);

        string j = ScriptOf(doc);
        Assert.DoesNotContain("call CreateAllUnits(", j);
        Assert.DoesNotContain("function CreateAllUnits", j);
    }

    [Fact]
    public void AMapThatAlreadyCreatesItsUnits_IsLeftUntouched()
    {
        var doc = BlankMap.Create();

        // Give the map its own (foreign) CreateAllUnits, as a real World-Editor map has.
        var entry = doc.GetFile(PreplacedUnitsScript.ScriptFile)!;
        string orig = Encoding.Latin1.GetString(entry.RawBytes);
        string foreign = orig.Replace(
            "function main takes nothing returns nothing",
            "function CreateAllUnits takes nothing returns nothing\n    local unit u\n    set u = null\nendfunction\n"
                + "function main takes nothing returns nothing");
        doc.AddOrReplaceRawFile(PreplacedUnitsScript.ScriptFile, Encoding.Latin1.GetBytes(foreign));

        var res = PreplacedUnitsScript.Sync(doc);
        Assert.False(res.Ok);
        Assert.Contains("untouched", res.Message);

        // Our generated block must never be spliced into a map that owns its creation.
        string j = ScriptOf(doc);
        Assert.DoesNotContain("wc3ctl preplaced widgets", j);
    }

    [Fact]
    public void SpellDispatchRegistration_IsDeferredUntilAfterTriggersExist()
    {
        // An arena registers its spell-dispatch trigger per player. A placed hero needs its player
        // registered too, but CreateAllUnits runs BEFORE InitCustomTriggers in main(), so the trigger
        // does not exist yet at unit-creation time. The registration must therefore be deferred to a
        // 0-second timer, never emitted inline in CreateAllUnits (that would register on a null trigger).
        var doc = BlankMap.Create();
        var entry = doc.GetFile(PreplacedUnitsScript.ScriptFile)!;
        string orig = Encoding.Latin1.GetString(entry.RawBytes);
        string withDispatcher = orig.Replace(
            "function main takes nothing returns nothing",
            "function ArenaCast takes nothing returns nothing\n"
            + "    call TriggerRegisterPlayerUnitEvent(gg_trg_GearCastCheck, GetOwningPlayer(GetTriggerUnit()), EVENT_PLAYER_UNIT_SPELL_EFFECT, null)\n"
            + "endfunction\n"
            + "function main takes nothing returns nothing");
        doc.AddOrReplaceRawFile(PreplacedUnitsScript.ScriptFile, Encoding.Latin1.GetBytes(withDispatcher));

        PlacementCommand.PlaceUnit(doc, "hfoo", ownerId: 0, x: 0f, y: 0f);

        string j = ScriptOf(doc);

        // Registered in a deferred function, for the placed unit's player.
        Assert.Contains("function wc3ctl_WirePlacedHeroSpells takes nothing returns nothing", j);
        Assert.Contains(
            "TriggerRegisterPlayerUnitEvent(gg_trg_GearCastCheck, Player(0), EVENT_PLAYER_UNIT_SPELL_EFFECT",
            j);
        // Fired by a 0-second timer from CreateAllUnits.
        Assert.Contains("call TimerStart(CreateTimer(), 0., false, function wc3ctl_WirePlacedHeroSpells)", j);

        // The ordering guard: CreateAllUnits' own body must not register any spell event inline.
        int cau = j.IndexOf("function CreateAllUnits takes nothing", StringComparison.Ordinal);
        int cauEnd = j.IndexOf("endfunction", cau, StringComparison.Ordinal);
        string cauBody = j.Substring(cau, cauEnd - cau);
        Assert.DoesNotContain("TriggerRegisterPlayerUnitEvent", cauBody);
    }

    [Fact]
    public void SynthDispatchedHero_IsNotAlsoRegisteredOnTheSharedDispatcher()
    {
        // A --synth-dispatch port gives a hero its own self-contained cast dispatcher, an any-unit
        // registration that needs no per-player wiring at all. If the deferred wiring here ALSO
        // registers that hero's owner on the source's shared dispatcher trigger too, every one of its
        // spells fires twice, once through each path, the exact double-damage and permanent-pause
        // bug this guards against.
        string j = ScriptOf(MapWithScript(
            "function ArenaCast takes nothing returns nothing\n"
            + "    call TriggerRegisterPlayerUnitEvent(gg_trg_GearCastCheck, GetOwningPlayer(GetTriggerUnit()), EVENT_PLAYER_UNIT_SPELL_EFFECT, null)\n"
            + "endfunction\n"
            + "function wc3ctl_SynthCast_Hpal takes nothing returns nothing\n"
            + "    local unit c = GetSpellAbilityUnit()\n"
            + "endfunction\n"));

        // The synth-covered hero's owner is excluded, so no registration line survives at all
        // (it was the map's only placed hero). Scoped to the GENERATED block only, the fixture's
        // own ArenaCast function (added just so DetectPerPlayerSpellTriggers finds a dispatcher)
        // legitimately names the same trigger.
        string block = GeneratedBlock(j);
        Assert.DoesNotContain("TriggerRegisterPlayerUnitEvent(gg_trg_GearCastCheck", block);
        // The deferred wiring function is still generated, a synth-dispatched hero still needs
        // the hero-array registration and other placement bookkeeping this block does.
        Assert.Contains("function wc3ctl_WirePlacedHeroSpells", block);
    }

    [Fact]
    public void APlainPortedHeroSharingTheMap_StillGetsTheSharedDispatcherRegistration()
    {
        // A player whose hero was ported WITHOUT --synth-dispatch still needs the shared dispatcher
        // (that IS its only entry point), so its owner must stay registered even though another
        // player's synth-dispatched hero on the same map is excluded.
        var doc = BlankMap.Create();
        var entry = doc.GetFile(PreplacedUnitsScript.ScriptFile)!;
        string orig = Encoding.Latin1.GetString(entry.RawBytes);
        string withDispatcher = orig.Replace(
            "function main takes nothing returns nothing",
            "function ArenaCast takes nothing returns nothing\n"
            + "    call TriggerRegisterPlayerUnitEvent(gg_trg_GearCastCheck, GetOwningPlayer(GetTriggerUnit()), EVENT_PLAYER_UNIT_SPELL_EFFECT, null)\n"
            + "endfunction\n"
            + "function wc3ctl_SynthCast_Hpal takes nothing returns nothing\n"
            + "    local unit c = GetSpellAbilityUnit()\n"
            + "endfunction\n"
            + "function main takes nothing returns nothing");
        doc.AddOrReplaceRawFile(PreplacedUnitsScript.ScriptFile, Encoding.Latin1.GetBytes(withDispatcher));

        PlacementCommand.PlaceUnit(doc, "Hpal", ownerId: 0, x: 0f, y: 0f);   // synth-dispatched
        PlacementCommand.PlaceUnit(doc, "Hkot", ownerId: 1, x: 64f, y: 0f); // plain port, no synth dispatcher

        string j = ScriptOf(doc);

        Assert.DoesNotContain(
            "TriggerRegisterPlayerUnitEvent(gg_trg_GearCastCheck, Player(0), EVENT_PLAYER_UNIT_SPELL_EFFECT",
            j);
        Assert.Contains(
            "TriggerRegisterPlayerUnitEvent(gg_trg_GearCastCheck, Player(1), EVENT_PLAYER_UNIT_SPELL_EFFECT",
            j);
    }

    [Fact]
    public void ATriggerTheMapAlreadyReachablyRegisters_IsNotAlsoRegisteredByUs()
    {
        // The field report this guards against, an UNTOUCHED map (no porting, no flags at all) with
        // a hero simply PLACED into it. WOS2's own native script already registers gg_trg_CastCheck
        // for a player, reached through main, InitCustomTriggers, InitTrig_LvlUpCheck, and its own
        // trigger action, once that player passes a one-time gate. If our own deferred wiring ALSO
        // registers that player unconditionally, every ability that player casts through it,
        // including a hero we never touched, fires twice.
        // Single-line anchors only, so this does not depend on which newline convention the fixture's
        // own embedded script happens to use between the two lines of an empty function body.
        var doc = BlankMap.Create();
        var entry = doc.GetFile(PreplacedUnitsScript.ScriptFile)!;
        string orig = Encoding.Latin1.GetString(entry.RawBytes);
        string withNative = orig.Replace(
            "function InitCustomTriggers takes nothing returns nothing",
            "function Trig_LvlUpCheck_Actions takes nothing returns nothing\n"
            + "    call TriggerRegisterPlayerUnitEvent(gg_trg_CastCheck, GetOwningPlayer(GetTriggerUnit()), EVENT_PLAYER_UNIT_SPELL_EFFECT, null)\n"
            + "endfunction\n"
            + "function InitTrig_LvlUpCheck takes nothing returns nothing\n"
            + "    call TriggerAddAction(gg_trg_LvlUpCheck, function Trig_LvlUpCheck_Actions)\n"
            + "endfunction\n"
            + "function InitCustomTriggers takes nothing returns nothing\n"
            + "    call InitTrig_LvlUpCheck()");
        Assert.NotEqual(orig, withNative); // the replace must actually have matched something
        doc.AddOrReplaceRawFile(PreplacedUnitsScript.ScriptFile, Encoding.Latin1.GetBytes(withNative));

        PlacementCommand.PlaceUnit(doc, "Hpal", ownerId: 0, x: 0f, y: 0f);

        string j = ScriptOf(doc);
        Assert.DoesNotContain(
            "TriggerRegisterPlayerUnitEvent(gg_trg_CastCheck, Player(0), EVENT_PLAYER_UNIT_SPELL_EFFECT",
            j);
    }

    [Fact]
    public void ATriggerRegisteredOnlyInsideAnUnreachableFunction_IsStillRegisteredByUs()
    {
        // The opposite, and equally important, case. A hero ported into a BLANK map carries the
        // source's own pick-hero flow as TEXT, so DetectPerPlayerSpellTriggers still finds the
        // trigger name in it, but nothing in a blank target ever CALLS that flow, so nothing else
        // registers the dispatcher there. This is the wiring this whole file exists for, and it
        // must not regress just because the new "already live" check exists.
        var doc = BlankMap.Create();
        var entry = doc.GetFile(PreplacedUnitsScript.ScriptFile)!;
        string orig = Encoding.Latin1.GetString(entry.RawBytes);
        string withUnreachable = orig.Replace(
            "function main takes nothing returns nothing",
            "function Trig_PickHero_Actions takes nothing returns nothing\n"
            + "    call TriggerRegisterPlayerUnitEvent(gg_trg_CastCheck, GetOwningPlayer(GetTriggerUnit()), EVENT_PLAYER_UNIT_SPELL_EFFECT, null)\n"
            + "endfunction\n"
            + "function main takes nothing returns nothing");
        doc.AddOrReplaceRawFile(PreplacedUnitsScript.ScriptFile, Encoding.Latin1.GetBytes(withUnreachable));

        PlacementCommand.PlaceUnit(doc, "Hpal", ownerId: 0, x: 0f, y: 0f);

        string j = ScriptOf(doc);
        Assert.Contains(
            "TriggerRegisterPlayerUnitEvent(gg_trg_CastCheck, Player(0), EVENT_PLAYER_UNIT_SPELL_EFFECT",
            j);
    }

    [Fact]
    public void AnAnyUnitRegistrationTheMapAlreadyReachablyHas_CoversEveryOwnerNotJustOne()
    {
        // The trigger only becomes a candidate at all through a TriggerRegisterPlayerUnitEvent
        // occurrence somewhere in the text (an unreachable pick-hero stub here, mirroring the
        // carried-but-dead source flow the blank-map case relies on), but the map ALSO reachably
        // registers it through TriggerRegisterAnyUnitEventBJ, which covers every player outright.
        // Once that form is live, no owner needs our own registration, not only the one whose hero
        // we happened to place.
        var doc = BlankMap.Create();
        var entry = doc.GetFile(PreplacedUnitsScript.ScriptFile)!;
        string orig = Encoding.Latin1.GetString(entry.RawBytes);
        // Single-line anchors only, same reasoning as the test above.
        string withNative = orig
            .Replace(
                "function InitCustomTriggers takes nothing returns nothing",
                "function InitCustomTriggers takes nothing returns nothing\n"
                + "    call TriggerRegisterAnyUnitEventBJ(gg_trg_CastCheck, EVENT_PLAYER_UNIT_SPELL_EFFECT)")
            .Replace(
                "function main takes nothing returns nothing",
                "function Trig_PickHero_Actions takes nothing returns nothing\n"
                + "    call TriggerRegisterPlayerUnitEvent(gg_trg_CastCheck, GetOwningPlayer(GetTriggerUnit()), EVENT_PLAYER_UNIT_SPELL_EFFECT, null)\n"
                + "endfunction\n"
                + "function main takes nothing returns nothing");
        Assert.NotEqual(orig, withNative);
        doc.AddOrReplaceRawFile(PreplacedUnitsScript.ScriptFile, Encoding.Latin1.GetBytes(withNative));

        PlacementCommand.PlaceUnit(doc, "Hpal", ownerId: 0, x: 0f, y: 0f);
        PlacementCommand.PlaceUnit(doc, "Hkot", ownerId: 1, x: 64f, y: 0f);

        string j = ScriptOf(doc);
        Assert.DoesNotContain("TriggerRegisterPlayerUnitEvent(gg_trg_CastCheck, Player(0)", j);
        Assert.DoesNotContain("TriggerRegisterPlayerUnitEvent(gg_trg_CastCheck, Player(1)", j);
    }

    [Fact]
    public void AWc3ctlWiredInitTrigCall_DoesNotCountAsTheTargetAlreadyHavingIt()
    {
        // A PLAIN port (no --synth-dispatch) hooks every carried InitTrig_* not already called by
        // another carried aggregator straight into InitCustomTriggers, marking the call it inserts
        // "// wc3ctl ported: ...", see ScriptPorter.HookInit. That includes a carried
        // InitTrig_LvlUpCheck just as readily as any other, so measured on a real port this made a
        // hero's OWN carried, merely-lazy, level-up-gated mechanism look "already reachable" in a
        // freshly ported BLANK target, wrongly suppressing the very registration a placed hero needs
        // to be immediately castable there. A wc3ctl-inserted call must never itself be the reason
        // something reads as already live, only a call the target already had before wc3ctl touched
        // it (the untouched-map case above) counts.
        var doc = BlankMap.Create();
        var entry = doc.GetFile(PreplacedUnitsScript.ScriptFile)!;
        string orig = Encoding.Latin1.GetString(entry.RawBytes);
        string withPortedInitTrig = orig.Replace(
            "function InitCustomTriggers takes nothing returns nothing",
            "function Trig_LvlUpCheck_Actions takes nothing returns nothing\n"
            + "    call TriggerRegisterPlayerUnitEvent(gg_trg_CastCheck, GetOwningPlayer(GetTriggerUnit()), EVENT_PLAYER_UNIT_SPELL_EFFECT, null)\n"
            + "endfunction\n"
            + "function InitTrig_LvlUpCheck takes nothing returns nothing\n"
            + "    call TriggerAddAction(gg_trg_LvlUpCheck, function Trig_LvlUpCheck_Actions)\n"
            + "endfunction\n"
            + "function InitCustomTriggers takes nothing returns nothing\n"
            + "    call InitTrig_LvlUpCheck() // wc3ctl ported: Asta (H028)");
        Assert.NotEqual(orig, withPortedInitTrig);
        doc.AddOrReplaceRawFile(PreplacedUnitsScript.ScriptFile, Encoding.Latin1.GetBytes(withPortedInitTrig));

        PlacementCommand.PlaceUnit(doc, "Hpal", ownerId: 0, x: 0f, y: 0f);

        string j = ScriptOf(doc);
        Assert.Contains(
            "TriggerRegisterPlayerUnitEvent(gg_trg_CastCheck, Player(0), EVENT_PLAYER_UNIT_SPELL_EFFECT",
            j);
    }

    /// <summary>Only the generated block, so assertions cannot be satisfied (or broken) by the
    /// fixture's own script text that happens to mention the same array.</summary>
    private static string GeneratedBlock(string jass)
    {
        int start = jass.IndexOf("//=== wc3ctl preplaced widgets", StringComparison.Ordinal);
        int end = jass.IndexOf("//=== end wc3ctl preplaced widgets", StringComparison.Ordinal);
        Assert.True(start >= 0 && end > start, "no generated block in the script");
        return jass[start..end];
    }

    /// <summary>Replaces the blank map's script, then places a hero-capable unit so the generated
    /// block has something to register.</summary>
    private static MapDocument MapWithScript(string extraJass)
    {
        var doc = BlankMap.Create();
        var entry = doc.GetFile(PreplacedUnitsScript.ScriptFile)!;
        string orig = Encoding.Latin1.GetString(entry.RawBytes);
        doc.AddOrReplaceRawFile(PreplacedUnitsScript.ScriptFile, Encoding.Latin1.GetBytes(
            orig.Replace("function main takes nothing returns nothing",
                extraJass + "function main takes nothing returns nothing")));
        PlacementCommand.PlaceUnit(doc, "Hpal", ownerId: 0, x: 0f, y: 0f);
        return doc;
    }

    [Fact]
    public void Every_real_per_player_hero_array_is_registered_including_one_based_ones()
    {
        // A map that received ports from several arenas has several hero arrays, with different index
        // conventions. Registering only one (or only 0-based ones) leaves the other source's heroes
        // unable to cast, which is exactly what happened to a merged map in practice.
        string j = ScriptOf(MapWithScript(
            "globals\n" +
            "    unit array Hero\n" +
            "    unit array udg_Player\n" +
            "endglobals\n" +
            "function CastA takes nothing returns boolean\n" +
            "    return GetSpellAbilityUnit() == Hero[GetPlayerId(GetTriggerPlayer())]\n" +
            "endfunction\n" +
            "function CastB takes nothing returns boolean\n" +
            "    return GetTriggerUnit() == udg_Player[( 1 + GetPlayerId(GetTriggerPlayer()) )]\n" +
            "endfunction\n"));

        Assert.Contains("set Hero[GetPlayerId(GetOwningPlayer(u))] = u", j);
        Assert.Contains("set udg_Player[1 + GetPlayerId(GetOwningPlayer(u))] = u", j);
    }

    [Fact]
    public void A_per_player_helper_array_is_never_written_to()
    {
        // udg_Dummy and udg_RevengeUnit are per-player but hold spawned helpers, not the player's
        // hero. Writing a real hero into one makes that system think it owns our hero, so it may
        // kill or recycle it. Only identity-carrying arrays may be touched.
        string j = GeneratedBlock(ScriptOf(MapWithScript(
            "globals\n" +
            "    unit array Hero\n" +
            "    unit array udg_Dummy\n" +
            "    unit array udg_RevengeUnit\n" +
            "endglobals\n" +
            "function Spawn takes nothing returns nothing\n" +
            "    set udg_Dummy[GetPlayerId(GetTriggerPlayer())]=bj_lastCreatedUnit\n" +
            "    set udg_RevengeUnit[( 1 + GetPlayerId(GetTriggerPlayer()) )]=bj_lastCreatedUnit\n" +
            "    call BJDebugMsg(I2S(GetPlayerId(GetTriggerPlayer())))\n" +
            "endfunction\n" +
            "function Cast takes nothing returns boolean\n" +
            "    return GetSpellAbilityUnit() == Hero[GetPlayerId(GetTriggerPlayer())]\n" +
            "endfunction\n")));

        Assert.Contains("set Hero[GetPlayerId(GetOwningPlayer(u))] = u", j);
        Assert.DoesNotContain("set udg_Dummy[", j);
        Assert.DoesNotContain("set udg_RevengeUnit[", j);
    }

    [Fact]
    public void The_generated_block_carries_its_generator_version()
    {
        var doc = BlankMap.Create();
        PlacementCommand.PlaceUnit(doc, "hfoo", 0, 0f, 0f);

        Assert.Contains($"[gen v{PreplacedUnitsScript.GeneratorVersion}]", ScriptOf(doc));
    }

    [Fact]
    public void A_block_from_a_newer_generator_is_left_alone_instead_of_downgraded()
    {
        // The regression that cost real debugging time: an older build regenerated a newer block and
        // silently stripped its spell wiring, leaving a map that compiles and hosts but whose heroes
        // are mute. A newer block must survive an older generator untouched.
        var doc = BlankMap.Create();
        PlacementCommand.PlaceUnit(doc, "hfoo", 0, 0f, 0f);
        var entry = doc.GetFile(PreplacedUnitsScript.ScriptFile)!;
        string bumped = Encoding.Latin1.GetString(entry.OverrideBytes ?? entry.RawBytes)
            .Replace($"[gen v{PreplacedUnitsScript.GeneratorVersion}]",
                     $"[gen v{PreplacedUnitsScript.GeneratorVersion + 1}]");
        doc.AddOrReplaceRawFile(PreplacedUnitsScript.ScriptFile, Encoding.Latin1.GetBytes(bumped));

        var res = PreplacedUnitsScript.Sync(doc);

        Assert.False(res.Ok);
        Assert.Contains("newer", res.Message);
        // Untouched, so the newer behaviour it carried is still there.
        Assert.Contains($"[gen v{PreplacedUnitsScript.GeneratorVersion + 1}]", ScriptOf(doc));
    }

    // A hero setup routine that registers events and dispatches on the placed hero 'Hpal', so the
    // generated block wires it. The (player, unit, integer) shape is what DetectHeroSetup looks for.
    private const string ArenaSetupJass =
        "function ArenaSetup takes player p, unit u, integer c returns boolean\n" +
        "    call TriggerRegisterUnitEvent(gg_trg_X, u, EVENT_UNIT_SPELL_EFFECT)\n" +
        "    if c == 'Hpal' then\n" +
        "    endif\n" +
        "    return true\n" +
        "endfunction\n";

    // A per-player doer-dummy setup, the general form of GGGA's WS_CreateWorkingSourceBagAndVendors,
    // spawns a unit and stores it into a per-player unit array. It takes (player, integer) but does not
    // dispatch on the integer, so it is a generic per-player routine, not a per-hero handler.
    private const string DoerAssignerJass =
        "function MakeDoer takes player p, integer c returns nothing\n" +
        "    call CreateNUnitsAtLoc(1, 'hpea', p, GetRectCenter(gg_rct_X), 0.)\n" +
        "    set udg_Doer[1 + GetPlayerId(p)] = bj_lastCreatedUnit\n" +
        "endfunction\n";

    [Fact]
    public void WiresThePerPlayerDoerDummySetupUnderANullGuard()
    {
        string block = GeneratedBlock(ScriptOf(MapWithScript(
            "globals\n    unit array udg_Doer\nendglobals\n" + ArenaSetupJass + DoerAssignerJass)));

        // The setup is called for the placed hero, then the doer-dummy routine is called once, guarded
        // on its own array still being null so a second placed hero cannot spawn a duplicate set.
        Assert.Contains("set ok = ArenaSetup(Player(0), hu, 'Hpal')", block);
        Assert.Contains("if udg_Doer[1 + GetPlayerId(Player(0))] == null then", block);
        Assert.Contains("call MakeDoer(Player(0), 'Hpal')", block);
    }

    [Fact]
    public void EmitsNoDoerDummyCallAndSaysSoWhenNoAssignerIsPresent()
    {
        // Degrade safely, no assigner in the script means no call is emitted (emitting one would call a
        // function that does not exist and fail the compile gate), and the result says so.
        var doc = MapWithScript("globals\n    unit array udg_Doer\nendglobals\n" + ArenaSetupJass);
        var res = PreplacedUnitsScript.Sync(doc);

        Assert.True(res.Ok);
        Assert.Contains("no per-player doer-dummy setup routine detected", res.Message);
        string block = GeneratedBlock(ScriptOf(doc));
        Assert.Contains("set ok = ArenaSetup(Player(0), hu, 'Hpal')", block); // the hero is still wired
        Assert.DoesNotContain("MakeDoer", block);
        Assert.DoesNotContain("== null then", block);
    }

    [Fact]
    public void DeclinesDoerDummyWiringWhenMoreThanOneRoutineQualifies()
    {
        // The rule is inferred from one map, so ambiguity is declined rather than guessed. Two qualifying
        // routines means neither is called, and the result reports the decline.
        var doc = MapWithScript(
            "globals\n    unit array udg_Doer\n    unit array udg_Doer2\nendglobals\n" + ArenaSetupJass +
            DoerAssignerJass +
            "function MakeDoer2 takes player p returns nothing\n" +
            "    call CreateNUnitsAtLoc(1, 'hpea', p, GetRectCenter(gg_rct_X), 0.)\n" +
            "    set udg_Doer2[1 + GetPlayerId(p)] = bj_lastCreatedUnit\n" +
            "endfunction\n");
        var res = PreplacedUnitsScript.Sync(doc);

        Assert.True(res.Ok);
        Assert.Contains("declined doer-dummy wiring", res.Message);
        Assert.Contains("2 candidate", res.Message);
        string block = GeneratedBlock(ScriptOf(doc));
        Assert.DoesNotContain("call MakeDoer", block);
    }

    // A level up handler shaped like Anime_WOS2's real Trig_LvlUpCheck_Actions: the caster and its
    // type id are read into locals ONCE near the top, and every hero's own branch compares that
    // local against its id constant rather than calling GetUnitTypeId again inline. This is exactly
    // the shape the hero guard matcher had to be relaxed for (see SynthDispatchLevelUpGrantTests
    // for the extractor itself), two heroes share the one function the way a real arena's does.
    private const string LevelUpHandlerJass =
        "globals\n" +
        "    integer Hpal_ID= 'Hpal'\n" +
        "    integer HpalG_ID= 'A0AA'\n" +
        "    integer Hkot_ID= 'Hkot'\n" +
        "    integer HkotQ_ID= 'A0BB'\n" +
        "endglobals\n" +
        "function Trig_LvlUpCheck_Actions takes nothing returns nothing\n" +
        "    local unit c= GetTriggerUnit()\n" +
        "    local integer id= GetUnitTypeId(c)\n" +
        "    if Hpal_ID == id then\n" +
        "        if GetUnitAbilityLevel(c, HpalG_ID) == 0 then\n" +
        "            call UnitAddAbility(c, HpalG_ID)\n" +
        "            call UnitMakeAbilityPermanent(c, true, HpalG_ID)\n" +
        "        endif\n" +
        "    endif\n" +
        "    if Hkot_ID == id then\n" +
        "        if GetUnitAbilityLevel(c, HkotQ_ID) == 0 then\n" +
        "            call UnitAddAbility(c, HkotQ_ID)\n" +
        "        endif\n" +
        "    endif\n" +
        "endfunction\n";

    [Fact]
    public void GrantsALevelUpAbility_AtSpawnForAPreplacedHero()
    {
        // A preplaced hero is created and levelled in one shot, so the level up event this handler
        // waits on never fires for it. The generated block must grant it the ability at spawn
        // instead, keeping the source's own idempotent guard and permanence call verbatim.
        string block = GeneratedBlock(ScriptOf(MapWithScript(LevelUpHandlerJass)));

        Assert.Contains("if GetUnitAbilityLevel(u, HpalG_ID) == 0 then", block);
        Assert.Contains("call UnitAddAbility(u, HpalG_ID)", block);
        Assert.Contains("call UnitMakeAbilityPermanent(u, true, HpalG_ID)", block);
        // MapWithScript places only 'Hpal', so Hkot's own branch must never leak onto it.
        Assert.DoesNotContain("HkotQ_ID", block);
    }

    [Fact]
    public void LevelUpGrant_ScopesToEachPlacedHerosOwnBranchOnly()
    {
        // Two different heroes placed by two different players, sharing the one handler function
        // above. Each must receive exactly its own grant, never the other's and never doubled.
        var doc = BlankMap.Create();
        var entry = doc.GetFile(PreplacedUnitsScript.ScriptFile)!;
        string orig = Encoding.Latin1.GetString(entry.RawBytes);
        doc.AddOrReplaceRawFile(PreplacedUnitsScript.ScriptFile, Encoding.Latin1.GetBytes(
            orig.Replace("function main takes nothing returns nothing",
                LevelUpHandlerJass + "function main takes nothing returns nothing")));

        PlacementCommand.PlaceUnit(doc, "Hpal", ownerId: 0, x: 0f, y: 0f);
        PlacementCommand.PlaceUnit(doc, "Hkot", ownerId: 1, x: 64f, y: 0f);

        string block = GeneratedBlock(ScriptOf(doc));

        Assert.Equal(1, Count(block, "call UnitAddAbility(u, HpalG_ID)"));
        Assert.Equal(1, Count(block, "call UnitAddAbility(u, HkotQ_ID)"));
    }

    [Fact]
    public void AHeroWithNoLevelUpBranch_GetsNoGrantLinesAtAll()
    {
        // A handler that only ever mentions ANOTHER hero must add nothing for the placed one, not
        // even the guard, rather than guess.
        string otherHeroOnly =
            "globals\n    integer Hkot_ID= 'Hkot'\n    integer HkotQ_ID= 'A0BB'\nendglobals\n" +
            "function Trig_LvlUpCheck_Actions takes nothing returns nothing\n" +
            "    local unit c= GetTriggerUnit()\n" +
            "    local integer id= GetUnitTypeId(c)\n" +
            "    if Hkot_ID == id and GetUnitAbilityLevel(c, HkotQ_ID) == 0 then\n" +
            "        call UnitAddAbility(c, HkotQ_ID)\n" +
            "    endif\n" +
            "endfunction\n";

        string block = GeneratedBlock(ScriptOf(MapWithScript(otherHeroOnly)));
        Assert.DoesNotContain("GetUnitAbilityLevel(u,", block);
        Assert.DoesNotContain("UnitAddAbility(u,", block);
    }

    [Fact]
    public void SyncWithNoPlacements_LeavesTheScriptUnchanged()
    {
        // A map with no preplaced widgets has nothing to wire, so the script stays byte-identical.
        var doc = BlankMap.Create();
        string before = ScriptOf(doc);
        var res = PreplacedUnitsScript.Sync(doc);
        Assert.True(res.Ok);
        Assert.Equal(0, res.Units);
        Assert.Equal(0, res.Items);
        Assert.Equal(before, ScriptOf(doc));
    }
}
