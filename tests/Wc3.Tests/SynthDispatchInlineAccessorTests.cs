// tests/Wc3.Tests/SynthDispatchInlineAccessorTests.cs
// A second real dispatcher shape, found while porting Anime Choice Arena's H0DA "Shadow Nanaya"
// with --synth-dispatch. Anime_WOS2 (the map SynthDispatchBuilder was built and tested against)
// caches the caster and the ability id into locals once near the top of the dispatcher and
// compares those locals from then on ("local unit c= GetSpellAbilityUnit()", then
// "GetUnitTypeId(c) == Hero_ID", then "id == HeroQ_ID"). Anime Choice Arena's own dispatcher
// instead calls the accessor natives inline at every comparison, with no caching local at all
// ("GetUnitTypeId(GetSpellAbilityUnit()) == Hero_ID", "GetSpellAbilityId() == HeroQ_ID"). Both
// call sites carry a nested pair of parentheses the old regex's "[^)]*" argument class could not
// cross, so the hero guard was never recognised and every ability guard resolved to nothing,
// and a real corpus map's --synth-dispatch request silently fell back to carrying the whole
// shared dispatcher (and, through it, the source map's entire framework) into the target.
using Wc3.Commands;

namespace Wc3.Tests;

public class SynthDispatchInlineAccessorTests
{
    // Anime Choice Arena's real shape for Shadow Nanaya (H0DA), reduced to two abilities.
    private const string InlineAccessorSource =
        "globals\n" +
        "    integer DarkShiki_ID= 'H0DA'\n" +
        "    integer DarkShikiQ_ID= 'A1QZ'\n" +
        "    integer DarkShikiW_ID= 'A1R1'\n" +
        "endglobals\n" +
        "function DarkShikiQ_Start takes unit c, real x, real y returns nothing\n" +
        "    call KillUnit(c)\n" +
        "endfunction\n" +
        "function DarkShikiW_Start takes unit c, unit td returns nothing\n" +
        "    call KillUnit(td)\n" +
        "endfunction\n" +
        "function CastHero takes nothing returns nothing\n" +
        "    if GetUnitTypeId(GetSpellAbilityUnit()) == DarkShiki_ID then\n" +
        "        if GetSpellAbilityId() == DarkShikiQ_ID then\n" +
        "            call DarkShikiQ_Start((GetSpellAbilityUnit()), ((GetSpellTargetX())*1.0), ((GetSpellTargetY())*1.0))\n" +
        "        elseif GetSpellAbilityId() == DarkShikiW_ID then\n" +
        "            call DarkShikiW_Start((GetSpellAbilityUnit()), (GetSpellTargetUnit()))\n" +
        "        endif\n" +
        "    endif\n" +
        "endfunction\n" +
        "function InitTrig_CastCheck takes nothing returns nothing\n" +
        "    call TriggerAddAction(CreateTrigger(), function CastHero)\n" +
        "endfunction\n";

    [Fact]
    public void HeroGuard_IsRecognisedWhenTheAccessorCallIsInlinedRatherThanCachedInALocal()
    {
        var branches = SynthDispatchBuilder.ExtractHeroCastBranches(InlineAccessorSource, "H0DA");

        Assert.NotNull(branches);
        Assert.Equal(2, branches!.Count);

        var q = branches.Single(b => b.AbilityRawcode == "A1QZ");
        Assert.Contains(q.Calls, c => c.StartsWith("call DarkShikiQ_Start(", StringComparison.Ordinal));
        Assert.Contains("DarkShikiQ_Start", q.Callees);

        var w = branches.Single(b => b.AbilityRawcode == "A1R1");
        Assert.Contains(w.Calls, c => c.StartsWith("call DarkShikiW_Start(", StringComparison.Ordinal));
        Assert.Contains("DarkShikiW_Start", w.Callees);
    }

    [Fact]
    public void AForeignHerosInlineBranchIsNotPickedUpEither()
    {
        string jass =
            "globals\n" +
            "    integer DarkShiki_ID= 'H0DA'\n" +
            "    integer Erza_ID= 'H0EE'\n" +
            "    integer ErzaQ_ID= 'A0EE'\n" +
            "endglobals\n" +
            "function ErzaQ_Start takes unit c returns nothing\n" +
            "    call KillUnit(c)\n" +
            "endfunction\n" +
            "function CastHero takes nothing returns nothing\n" +
            "    if GetUnitTypeId(GetSpellAbilityUnit()) == Erza_ID then\n" +
            "        if GetSpellAbilityId() == ErzaQ_ID then\n" +
            "            call ErzaQ_Start(GetSpellAbilityUnit())\n" +
            "        endif\n" +
            "    endif\n" +
            "endfunction\n";

        Assert.Null(SynthDispatchBuilder.ExtractHeroCastBranches(jass, "H0DA"));
    }

    [Fact]
    public void MixedMapWhereOneHeroCachesLocalsAndAnotherInlinesTheAccessor_BothResolve()
    {
        // The same source function can legitimately mix both idioms across different heroes'
        // branches (a big shared arena dispatcher accretes hero blocks written years apart).
        string jass =
            "globals\n" +
            "    integer Asta_ID= 'H028'\n" +
            "    integer AstaQ_ID= 'A0DL'\n" +
            "    integer DarkShiki_ID= 'H0DA'\n" +
            "    integer DarkShikiQ_ID= 'A1QZ'\n" +
            "endglobals\n" +
            "function AstaQ_Start takes unit c returns nothing\n" +
            "    call KillUnit(c)\n" +
            "endfunction\n" +
            "function DarkShikiQ_Start takes unit c returns nothing\n" +
            "    call KillUnit(c)\n" +
            "endfunction\n" +
            "function CastHero takes nothing returns nothing\n" +
            "    local unit c= GetSpellAbilityUnit()\n" +
            "    local integer id= GetSpellAbilityId()\n" +
            "    if GetUnitTypeId(c) == Asta_ID then\n" +
            "        if id == AstaQ_ID then\n" +
            "            call AstaQ_Start(c)\n" +
            "        endif\n" +
            "    endif\n" +
            "    if GetUnitTypeId(GetSpellAbilityUnit()) == DarkShiki_ID then\n" +
            "        if GetSpellAbilityId() == DarkShikiQ_ID then\n" +
            "            call DarkShikiQ_Start(GetSpellAbilityUnit())\n" +
            "        endif\n" +
            "    endif\n" +
            "endfunction\n";

        var astaBranches = SynthDispatchBuilder.ExtractHeroCastBranches(jass, "H028");
        Assert.NotNull(astaBranches);
        Assert.Single(astaBranches!);
        Assert.Equal("A0DL", astaBranches![0].AbilityRawcode);

        var darkShikiBranches = SynthDispatchBuilder.ExtractHeroCastBranches(jass, "H0DA");
        Assert.NotNull(darkShikiBranches);
        Assert.Single(darkShikiBranches!);
        Assert.Equal("A1QZ", darkShikiBranches![0].AbilityRawcode);
    }
}
