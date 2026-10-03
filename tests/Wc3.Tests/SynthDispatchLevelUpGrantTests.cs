// tests/Wc3.Tests/SynthDispatchLevelUpGrantTests.cs
// A PREPLACED hero is created and levelled in one shot, so a level up event an arena's handler
// relies on to grant part of a hero's kit never fires for it. SynthDispatchBuilder.
// ExtractHeroLevelUpGrants reads that hero's own branch out of the handler so the grants can be
// replayed at spawn instead. These pin the exact shape a real map (Anime_WOS2's Asta) uses, the
// relaxed hero guard the blocker needed, and the scoping that keeps a foreign hero's grants out.
using Wc3.Commands;

namespace Wc3.Tests;

public class SynthDispatchLevelUpGrantTests
{
    // The real shape Anime_WOS2's Trig_LvlUpCheck_Actions uses: the caster is read into a local
    // ONCE near the top of the function ("local unit c= GetTriggerUnit()"), its type id resolved
    // into ANOTHER local ("local integer id= GetUnitTypeId(c)"), and every hero's own branch
    // compares that local against its id constant rather than calling GetUnitTypeId again inline.
    // This is exactly the shape SynthDispatchBuilder.TryFindHeroBranch could not see before this
    // fix (it required the literal text "GetUnitTypeId" inside the guard's own condition).
    private const string LevelUpHandlerHeader =
        "globals\n" +
        "    integer Asta_ID= 'H028'\n" +
        "    integer AstaG_ID= 'A0DQ'\n" +
        "    integer AstaSword_ID= 'A0DW'\n" +
        "    integer AstaF_ID= 'A0DR'\n" +
        "    integer Natsu_ID= 'H001'\n" +
        "    integer NatsuF_ID= 'A0AA'\n" +
        "endglobals\n" +
        "function Trig_LvlUpCheck_Actions takes nothing returns nothing\n" +
        "    local unit c= GetTriggerUnit()\n" +
        "    local integer id= GetUnitTypeId(c)\n";

    [Fact]
    public void ExtractsAWrappingGuardsGrants_UsingTheRelaxedHeroBranchMatch()
    {
        // Asta's own exact shape: a wrapping "if Asta_ID == id then" with two nested
        // "if GetUnitAbilityLevel(...) == 0" grants inside it, one of which (the G branch) also
        // rides a second, ungated ability along for free (the sword).
        string jass = LevelUpHandlerHeader +
            "    if Asta_ID == id then\n" +
            "        if GetUnitAbilityLevel(c, AstaG_ID) == 0 then\n" +
            "            call UnitAddAbility(c, AstaG_ID)\n" +
            "            call UnitMakeAbilityPermanent(c, true, AstaG_ID)\n" +
            "            call UnitAddAbility(c, AstaSword_ID)\n" +
            "        endif\n" +
            "        if GetUnitAbilityLevel(c, AstaF_ID) == 0 then\n" +
            "            call UnitAddAbility(c, AstaF_ID)\n" +
            "            call UnitMakeAbilityPermanent(c, true, AstaF_ID)\n" +
            "        endif\n" +
            "    endif\n" +
            "    set c= null\n" +
            "endfunction\n";

        var grants = SynthDispatchBuilder.ExtractHeroLevelUpGrants(jass, "H028");

        Assert.NotNull(grants);
        Assert.Equal(2, grants!.Count);

        var g = grants.Single(x => x.AbilityRawcode == "A0DQ");
        Assert.Equal("c", g.UnitParam);
        Assert.Equal("GetUnitAbilityLevel(c, AstaG_ID) == 0", g.Condition);
        Assert.Equal(new[]
        {
            "call UnitAddAbility(c, AstaG_ID)",
            "call UnitMakeAbilityPermanent(c, true, AstaG_ID)",
            "call UnitAddAbility(c, AstaSword_ID)",
        }, g.Calls);

        var f = grants.Single(x => x.AbilityRawcode == "A0DR");
        Assert.Equal(new[]
        {
            "call UnitAddAbility(c, AstaF_ID)",
            "call UnitMakeAbilityPermanent(c, true, AstaF_ID)",
        }, f.Calls);
    }

    [Fact]
    public void NeverLeaksAnotherHerosGrantFromTheSameSharedHandler()
    {
        // The trap the job called out: the handler carries EVERY hero's branch in one function, so
        // an unscoped scan for UnitAddAbility would hand Asta Natsu's passive too. Querying for
        // Natsu must return ONLY Natsu's own grant, never Asta's.
        string jass = LevelUpHandlerHeader +
            "    if Asta_ID == id then\n" +
            "        if GetUnitAbilityLevel(c, AstaG_ID) == 0 then\n" +
            "            call UnitAddAbility(c, AstaG_ID)\n" +
            "            call UnitMakeAbilityPermanent(c, true, AstaG_ID)\n" +
            "        endif\n" +
            "    endif\n" +
            "    if Natsu_ID == id and GetUnitAbilityLevel(c, NatsuF_ID) == 0 then\n" +
            "        call UnitAddAbility(c, NatsuF_ID)\n" +
            "        call UnitMakeAbilityPermanent(c, true, NatsuF_ID)\n" +
            "    endif\n" +
            "endfunction\n";

        var astaGrants = SynthDispatchBuilder.ExtractHeroLevelUpGrants(jass, "H028");
        Assert.NotNull(astaGrants);
        Assert.Single(astaGrants!);
        Assert.Equal("A0DQ", astaGrants![0].AbilityRawcode);
        Assert.DoesNotContain(astaGrants[0].Calls, c => c.Contains("Natsu", StringComparison.Ordinal));

        // Natsu's own combined-condition shape (the hero check and the ability guard share one
        // "if ... and ... then" line) is a DIFFERENT shape from Asta's wrapping one and is out of
        // scope for this fix, see the type's own doc comment. It must not silently pick up Asta's
        // grant either way.
        var natsuGrants = SynthDispatchBuilder.ExtractHeroLevelUpGrants(jass, "H001");
        Assert.True(natsuGrants is null || natsuGrants.All(g => g.AbilityRawcode != "A0DQ"));
    }

    [Fact]
    public void AHeroWithNoLevelUpBranchAtAll_YieldsNull()
    {
        string jass = LevelUpHandlerHeader +
            "    if Natsu_ID == id and GetUnitAbilityLevel(c, NatsuF_ID) == 0 then\n" +
            "        call UnitAddAbility(c, NatsuF_ID)\n" +
            "    endif\n" +
            "endfunction\n";

        Assert.Null(SynthDispatchBuilder.ExtractHeroLevelUpGrants(jass, "H028"));
    }

    [Fact]
    public void ALiteralGetUnitTypeIdGuard_IsStillRecognised()
    {
        // The relaxed local-alias matcher is additive, the pre-existing literal shape
        // ("GetUnitTypeId(...) == HeroId" with no local in between) must keep working too. The
        // ability guard's own unit argument still has to be a bare local (never a call expression,
        // there would be nothing to rename it to for a freshly created unit), just as the local
        // alias shape requires.
        string jass =
            "globals\n" +
            "    integer AstaG_ID= 'A0DQ'\n" +
            "endglobals\n" +
            "function Trig_LvlUpCheck_Actions takes nothing returns nothing\n" +
            "    local unit c= GetTriggerUnit()\n" +
            "    if GetUnitTypeId(c) == 'H028' then\n" +
            "        if GetUnitAbilityLevel(c, AstaG_ID) == 0 then\n" +
            "            call UnitAddAbility(c, AstaG_ID)\n" +
            "        endif\n" +
            "    endif\n" +
            "endfunction\n";

        var grants = SynthDispatchBuilder.ExtractHeroLevelUpGrants(jass, "H028");
        Assert.NotNull(grants);
        Assert.Single(grants!);
        Assert.Equal("A0DQ", grants![0].AbilityRawcode);
    }

    [Fact]
    public void ReplaceIdentifier_RetargetsOnlyTheWholeToken()
    {
        string retargeted = SynthDispatchBuilder.ReplaceIdentifier(
            "GetUnitAbilityLevel(c, AstaG_ID) == 0", "c", "u");
        Assert.Equal("GetUnitAbilityLevel(u, AstaG_ID) == 0", retargeted);
    }
}
