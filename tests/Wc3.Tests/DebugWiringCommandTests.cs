// tests/Wc3.Tests/DebugWiringCommandTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Hermetic coverage for the cast-chain instrumentation. The fixture below is the real shape
/// found on a ported arena map (an "Anime_WOS2"-style hero dispatcher): one gg_trg_* trigger
/// registered per player for EVENT_PLAYER_UNIT_SPELL_EFFECT, a single boolean-returning condition
/// function that gates on IsUnitType/CheckCoordsInRect/SpellExtension before branching per hero on
/// GetUnitTypeId, and the generated wc3ctl_WirePlacedHeroSpells that defers the per-player
/// registration until after InitCustomTriggers has run.
/// </summary>
public class DebugWiringCommandTests
{
    private const string Script = """
        globals
            trigger gg_trg_CastCheck= null
            integer Asta_ID= 'H028'
            integer Raiden_ID= 'H001'
            integer AstaQ_ID= 'A0DL'
        endglobals

        function CastHero_Conditions takes nothing returns boolean
            local integer check= 0
            local unit c= GetSpellAbilityUnit()
            local integer id= GetSpellAbilityId()
            local boolean b= true
        if IsUnitType(c, UNIT_TYPE_HERO) and b then
            if ( CheckCoordsInRect(gg_rct_Base , GetUnitX(c) , GetUnitY(c)) == false and CheckCoordsInRect(gg_rct_Cage , GetUnitX(c) , GetUnitY(c)) == false ) or SpellExtension(c , id) then
            if GetUnitTypeId(c) == Raiden_ID then
            if id == RaidenQ_ID then
            set check=1
            endif
            endif
            if GetUnitTypeId(c) == Asta_ID then
            if id == AstaQ_ID then
            set check=1
            call AstaQ_Start(c)
            endif
            endif
            endif
            endif
            set c=null
            return false
        endfunction

        function InitTrig_CastCheck takes nothing returns nothing
            set gg_trg_CastCheck=CreateTrigger()
            call TriggerAddCondition(gg_trg_CastCheck, Condition(function CastHero_Conditions))
        endfunction

        function wc3ctl_WirePlacedHeroSpells takes nothing returns nothing
            call TriggerRegisterPlayerUnitEvent(gg_trg_CastCheck, Player(0), EVENT_PLAYER_UNIT_SPELL_EFFECT, null)
            call DestroyTimer(GetExpiredTimer())
        endfunction

        function main takes nothing returns nothing
            call InitTrig_CastCheck()
        endfunction
        """;

    [Fact]
    public void Instrument_adds_every_checkpoint_for_the_requested_hero()
    {
        var doc = BuildMap(Script);

        var result = DebugWiringCommand.Instrument(doc, "H028");

        Assert.True(result.Ok, result.Message);
        Assert.Contains(result.Targets, t => t.Function == "CastHero_Conditions");
        Assert.Contains(result.Targets, t => t.Function == PreplacedUnitsScript.WireSpellsFunc);

        string script = Encoding.Latin1.GetString(doc.GetFile("war3map.j")!.OverrideBytes!);

        Assert.Contains("call BJDebugMsg(\"[wc3ctl-debug] CastHero_Conditions entered, ability=\" "
            + "+ I2S(GetSpellAbilityId()) + \", casterType=\" + I2S(GetUnitTypeId(c)))", script);
        Assert.Contains("call BJDebugMsg(\"[wc3ctl-debug] IsUnitType(HERO)=true\")", script);
        Assert.Contains("call BJDebugMsg(\"[wc3ctl-debug] b=true\")", script);
        Assert.Contains("CheckCoordsInRect(gg_rct_Base , GetUnitX(c) , GetUnitY(c))=true", script);
        Assert.Contains("SpellExtension(c , id)=true", script);
        Assert.Contains("[wc3ctl-debug] area/extension gate=true", script);
        Assert.Contains("entered Asta_ID branch for H028, ability=\" + I2S(GetSpellAbilityId())", script);
        Assert.Contains("[wc3ctl-debug] CastHero_Conditions finished, check=\" + I2S(check)", script);
        Assert.Contains("[wc3ctl-debug] wc3ctl_WirePlacedHeroSpells fired, gg_trg_CastCheck is non-null", script);
        Assert.Contains("[wc3ctl-debug] registered gg_trg_CastCheck EVENT_PLAYER_UNIT_SPELL_EFFECT for Player(0)", script);

        // The instrumentation is additive, every original function is still there, none dropped.
        var names = JassFunctionIndex.Parse(script).Select(f => f.Name).ToHashSet(StringComparer.Ordinal);
        Assert.Equal(
            new HashSet<string> { "CastHero_Conditions", "InitTrig_CastCheck", PreplacedUnitsScript.WireSpellsFunc, "main" },
            names);
    }

    [Fact]
    public void Instrument_without_a_hero_still_covers_the_shared_checkpoints()
    {
        var doc = BuildMap(Script);

        var result = DebugWiringCommand.Instrument(doc);

        Assert.True(result.Ok, result.Message);
        string script = Encoding.Latin1.GetString(doc.GetFile("war3map.j")!.OverrideBytes!);
        Assert.Contains("CastHero_Conditions entered", script);
        Assert.DoesNotContain("Asta_ID branch", script); // no hero requested, no per-hero checkpoint
    }

    [Fact]
    public void Instrument_reports_a_hero_with_no_dispatch_branch_at_all()
    {
        // Sylas_ID exists as an alias but the dispatcher never branches on it, the class of bug
        // this tool exists to surface, a hero that audits clean yet has no branch to enter.
        string script = Script.Replace(
            "integer AstaQ_ID= 'A0DL'",
            "integer AstaQ_ID= 'A0DL'\n    integer Sylas_ID= 'H0ZZ'");
        var doc = BuildMap(script);

        var result = DebugWiringCommand.Instrument(doc, "H0ZZ");

        Assert.True(result.Ok, result.Message);
        Assert.Contains(result.Diagnostics, d =>
            d.Contains("Sylas_ID") && d.Contains("no dispatch branch in this function at all"));
    }

    [Fact]
    public void Instrument_reports_no_dispatch_trigger_rather_than_failing_oddly()
    {
        const string noDispatch = "function main takes nothing returns nothing\nendfunction\n";
        var doc = BuildMap(noDispatch);

        var result = DebugWiringCommand.Instrument(doc, "H028");

        Assert.False(result.Ok);
        Assert.Contains("nothing here to instrument", result.Message);
        Assert.Empty(result.Targets);
    }

    private static MapDocument BuildMap(string jass) =>
        MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = Encoding.UTF8.GetBytes(jass),
        }));
}
