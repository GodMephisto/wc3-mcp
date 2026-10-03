// tests/Wc3.Tests/BundleScriptClosureTests.cs
using System.Text;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Hermetic coverage for the JASS script closure inside the unit bundle:
/// rawcode-literal seeding, global-alias seeding, transitive call collection
/// (both "Foo(" invocations and "function Foo" references), line-comment
/// immunity, exclusion of unrelated functions, and the empty-list-plus-
/// diagnostic contract for Lua-only and script-less maps.
/// </summary>
public class BundleScriptClosureTests
{
    [Fact]
    public void Seed_function_pulls_transitive_callees_and_skips_unrelated()
    {
        const string jass =
            "function Helper takes nothing returns nothing\n" +
            "endfunction\n" +
            "\n" +
            "function Timer_Cb takes nothing returns nothing\n" +
            "endfunction\n" +
            "\n" +
            "function Unrelated takes nothing returns nothing\n" +
            "    call KillUnit(GetTriggerUnit())\n" +
            "endfunction\n" +
            "\n" +
            "function Trig_Spell_Actions takes nothing returns nothing\n" +
            "    // call Unrelated() — commented out, must not count\n" +
            "    if GetSpellAbilityId() == 'A000' then\n" +
            "        call Helper()\n" +
            "        call TimerStart(CreateTimer(), 1.0, false, function Timer_Cb)\n" +
            "    endif\n" +
            "endfunction\n";

        var bundle = ResolveH000WithAbility(jass);

        // Ordered by StartLine; the seed's reason names the literal, the callees
        // name their discoverer — including the "function Timer_Cb" reference form.
        Assert.Equal(new[]
        {
            new BundleFunction("Helper", 1, 2, "called by Trig_Spell_Actions"),
            new BundleFunction("Timer_Cb", 4, 5, "called by Trig_Spell_Actions"),
            new BundleFunction("Trig_Spell_Actions", 11, 17, "references 'A000'"),
        }, bundle.Functions);
        Assert.DoesNotContain(bundle.Functions, f => f.Name == "Unrelated");
    }

    [Fact]
    public void Global_initialized_with_seed_rawcode_aliases_into_the_closure()
    {
        const string jass =
            "globals\n" +
            "integer MyHero_ID= 'H000'\n" +
            "endglobals\n" +
            "\n" +
            "function CheckHero takes nothing returns boolean\n" +
            "    return GetUnitTypeId(GetTriggerUnit()) == MyHero_ID\n" +
            "endfunction\n" +
            "\n" +
            "function Other takes nothing returns nothing\n" +
            "endfunction\n";

        var bundle = ResolveH000WithAbility(jass);

        var check = Assert.Single(bundle.Functions);
        Assert.Equal(new BundleFunction("CheckHero", 5, 7, "references 'H000' via MyHero_ID"), check);
    }

    [Fact]
    public void Lua_only_map_yields_empty_functions_with_diagnostic()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = HeroW3u(),
            ["war3map.lua"] = Encoding.UTF8.GetBytes("function foo() end\n"),
        }));

        var bundle = BundleCommand.ResolveUnit(doc, "H000", ctx: null, preDiagnostics: Array.Empty<string>());

        Assert.Empty(bundle.Functions);
        Assert.Contains(bundle.Diagnostics, d => d.Contains("Lua"));
    }

    [Fact]
    public void Scriptless_map_yields_empty_functions_with_diagnostic()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = HeroW3u(),
        }));

        var bundle = BundleCommand.ResolveUnit(doc, "H000", ctx: null, preDiagnostics: Array.Empty<string>());

        Assert.Empty(bundle.Functions);
        Assert.Contains(bundle.Diagnostics, d => d.Contains("war3map.j"));
    }

    [Fact]
    public void Foreign_hero_dispatch_branch_is_not_followed()
    {
        // The shared spellcast dispatcher pattern: our hero H000's branch calls RaidenSpell,
        // another custom hero H001's branch calls NatsuSpell. Seeding on H000 pulls in the
        // dispatcher (it names 'H000') and RaidenSpell, but NOT NatsuSpell, which lives only
        // inside a branch guarded by a foreign hero id.
        const string jass =
            "function RaidenSpell takes nothing returns nothing\n" +
            "endfunction\n" +
            "function NatsuSpell takes nothing returns nothing\n" +
            "endfunction\n" +
            "function CastDispatch takes nothing returns nothing\n" +
            "    if GetUnitTypeId(GetSpellAbilityUnit()) == 'H000' then\n" +
            "        call RaidenSpell()\n" +
            "    endif\n" +
            "    if GetUnitTypeId(GetSpellAbilityUnit()) == 'H001' then\n" +
            "        call NatsuSpell()\n" +
            "    endif\n" +
            "endfunction\n";

        var names = ResolveWithTwoHeroes(jass).Functions.Select(f => f.Name).ToHashSet();
        Assert.Contains("CastDispatch", names);      // seed, references 'H000'
        Assert.Contains("RaidenSpell", names);       // our branch, followed
        Assert.DoesNotContain("NatsuSpell", names);  // foreign branch, cut
    }

    [Fact]
    public void Foreign_dispatch_via_id_alias_and_elseif_is_not_followed()
    {
        // Same, but the guards use *_ID globals and an elseif chain (the real maps' form).
        const string jass =
            "globals\n" +
            "integer Raiden_ID= 'H000'\n" +
            "integer Natsu_ID= 'H001'\n" +
            "endglobals\n" +
            "function RaidenSpell takes nothing returns nothing\n" +
            "endfunction\n" +
            "function NatsuSpell takes nothing returns nothing\n" +
            "endfunction\n" +
            "function SharedFx takes nothing returns nothing\n" +
            "endfunction\n" +
            "function CastDispatch takes nothing returns nothing\n" +
            "    if GetUnitTypeId(GetSpellAbilityUnit()) == Raiden_ID then\n" +
            "        call RaidenSpell()\n" +
            "        call SharedFx()\n" +
            "    elseif GetUnitTypeId(GetSpellAbilityUnit()) == Natsu_ID then\n" +
            "        call NatsuSpell()\n" +
            "        call SharedFx()\n" +
            "    endif\n" +
            "endfunction\n";

        var names = ResolveWithTwoHeroes(jass).Functions.Select(f => f.Name).ToHashSet();
        Assert.Contains("RaidenSpell", names);       // our branch
        Assert.Contains("SharedFx", names);          // also called in our branch, so kept
        Assert.DoesNotContain("NatsuSpell", names);  // only in the foreign branch, cut
    }

    [Fact]
    public void Asset_literals_in_a_foreign_dispatch_branch_are_not_bundled()
    {
        // A shared effect dispatcher names our hero's model in its branch and another hero's in
        // the foreign branch. Only ours may be bundled, the foreign asset never loads for us.
        const string jass =
            "function CastFx takes nothing returns nothing\n" +
            "    if GetUnitTypeId(GetSpellAbilityUnit()) == 'H000' then\n" +
            "        call AddSpecialEffect(\"war3mapImported\\\\ours.mdx\", 0., 0.)\n" +
            "    endif\n" +
            "    if GetUnitTypeId(GetSpellAbilityUnit()) == 'H001' then\n" +
            "        call AddSpecialEffect(\"war3mapImported\\\\theirs.mdx\", 0., 0.)\n" +
            "    endif\n" +
            "endfunction\n";

        var files = ResolveWithTwoHeroes(jass).Files.Select(f => f.Path).ToList();
        Assert.Contains(files, p => p.EndsWith("ours.mdx", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(files, p => p.EndsWith("theirs.mdx", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ExecuteFunc_named_function_is_pulled_into_the_closure()
    {
        // A seed function dispatches by name via ExecuteFunc("Storm_Actions"). The target sits in
        // a string literal, not a normal call, but it must still be carried, else the spell's body
        // never runs in the ported map.
        const string jass =
            "function Storm_Actions takes nothing returns nothing\n" +
            "endfunction\n" +
            "function Trig_Cast takes nothing returns nothing\n" +
            "    if GetSpellAbilityId() == 'A000' then\n" +
            "        call ExecuteFunc(\"Storm_Actions\")\n" +
            "    endif\n" +
            "endfunction\n";

        var names = ResolveH000WithAbility(jass).Functions.Select(f => f.Name).ToHashSet();
        Assert.Contains("Trig_Cast", names);      // seed (references 'A000')
        Assert.Contains("Storm_Actions", names);  // reached only via ExecuteFunc
    }

    [Fact]
    public void ExecuteFunc_naming_a_nonexistent_function_is_ignored()
    {
        const string jass =
            "function Trig_Cast takes nothing returns nothing\n" +
            "    if GetSpellAbilityId() == 'A000' then\n" +
            "        call ExecuteFunc(\"NotAFunction\")\n" +
            "    endif\n" +
            "endfunction\n";

        var names = ResolveH000WithAbility(jass).Functions.Select(f => f.Name).ToHashSet();
        Assert.Contains("Trig_Cast", names);
        Assert.DoesNotContain("NotAFunction", names); // unknown name adds nothing, no throw
    }

    [Fact]
    public void ExecuteFunc_in_a_foreign_dispatch_branch_is_not_pulled_in()
    {
        // ExecuteFunc callees honor the same branch scoping as ordinary calls.
        const string jass =
            "function RaidenFx takes nothing returns nothing\n" +
            "endfunction\n" +
            "function NatsuFx takes nothing returns nothing\n" +
            "endfunction\n" +
            "function CastDispatch takes nothing returns nothing\n" +
            "    if GetUnitTypeId(GetSpellAbilityUnit()) == 'H000' then\n" +
            "        call ExecuteFunc(\"RaidenFx\")\n" +
            "    endif\n" +
            "    if GetUnitTypeId(GetSpellAbilityUnit()) == 'H001' then\n" +
            "        call ExecuteFunc(\"NatsuFx\")\n" +
            "    endif\n" +
            "endfunction\n";

        var names = ResolveWithTwoHeroes(jass).Functions.Select(f => f.Name).ToHashSet();
        Assert.Contains("RaidenFx", names);       // our branch's ExecuteFunc target
        Assert.DoesNotContain("NatsuFx", names);  // foreign branch's target, cut
    }

    [Fact]
    public void Script_added_sibling_ability_is_carried_but_a_foreign_heros_is_not()
    {
        // A hero's ability ids sit in globals named after the hero (MyHero_ID, MyHeroQ_ID,
        // MyHeroQ2_ID). A000 (MyHeroQ_ID) is on the unit's ability list and seeds normally. A001
        // (MyHeroQ2_ID) is a dash-back added at runtime by UnitAddAbility, on no ability list and
        // reachable only through the script — it must be carried by the sibling-global rule. A foreign
        // hero's sibling (NatsuQ2_ID -> A002) shares no stem with our hero and must NOT be carried.
        const string jass =
            "globals\n" +
            "integer MyHero_ID= 'H000'\n" +
            "integer MyHeroQ_ID= 'A000'\n" +
            "integer MyHeroQ2_ID= 'A001'\n" +
            "integer Natsu_ID= 'H001'\n" +
            "integer NatsuQ2_ID= 'A002'\n" +
            "endglobals\n" +
            "function Cast takes nothing returns nothing\n" +
            "    if GetUnitTypeId(GetSpellAbilityUnit()) == MyHero_ID then\n" +
            "        call UnitAddAbility(GetSpellAbilityUnit(), MyHeroQ2_ID)\n" +
            "    endif\n" +
            "endfunction\n";

        var rawcodes = ResolveWithSiblingAbilities(jass).Objects.Select(o => o.Rawcode).ToHashSet();
        Assert.Contains("A000", rawcodes);      // on the hero's ability list (a normal seed)
        Assert.Contains("A001", rawcodes);      // sibling id-global, script-added -> carried
        Assert.DoesNotContain("A002", rawcodes); // a foreign hero's sibling -> not carried
    }

    /// <summary>H000 (hero, uabi=A000) with a script-only sibling ability A001 and a foreign hero
    /// H001 whose sibling is A002. A000/A001/A002 all exist as custom abilities.</summary>
    private static UnitBundle ResolveWithSiblingAbilities(string jass)
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var h000 = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        h000.Modifications.Add(new SimpleObjectDataModification
        { Id = "uabi".FromRawcode(), Type = ObjectDataType.String, Value = "A000" });
        w3u.NewUnits.Add(h000);
        w3u.NewUnits.Add(new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H001".FromRawcode() });

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        foreach (var rc in new[] { "A000", "A001", "A002" })
            w3a.NewAbilities.Add(new LevelObjectModification
            { OldId = "AHbz".FromRawcode(), NewId = rc.FromRawcode() });

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Serialize(w => War3Net.Build.Extensions.BinaryWriterExtensions.Write(w, w3u)),
            ["war3map.w3a"] = Serialize(w => War3Net.Build.Extensions.BinaryWriterExtensions.Write(w, w3a)),
            ["war3map.j"] = Encoding.UTF8.GetBytes(jass),
        }));
        return BundleCommand.ResolveUnit(doc, "H000", ctx: null, preDiagnostics: Array.Empty<string>());
    }

    /// <summary>H000 (ported hero, uabi=A000) alongside a second custom hero H001 (foreign),
    /// plus A000 and the given script. Bundling H000 sees ported ids H000/A000, and H001 as a
    /// known foreign custom object, so a branch guarded by 'H001' is dropped.</summary>
    private static UnitBundle ResolveWithTwoHeroes(string jass)
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var h000 = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        h000.Modifications.Add(new SimpleObjectDataModification
        { Id = "uabi".FromRawcode(), Type = ObjectDataType.String, Value = "A000" });
        w3u.NewUnits.Add(h000);
        w3u.NewUnits.Add(new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H001".FromRawcode() });

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        w3a.NewAbilities.Add(new LevelObjectModification
        { OldId = "AHbz".FromRawcode(), NewId = "A000".FromRawcode() });

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Serialize(w => War3Net.Build.Extensions.BinaryWriterExtensions.Write(w, w3u)),
            ["war3map.w3a"] = Serialize(w => War3Net.Build.Extensions.BinaryWriterExtensions.Write(w, w3a)),
            ["war3map.j"] = Encoding.UTF8.GetBytes(jass),
        }));
        return BundleCommand.ResolveUnit(doc, "H000", ctx: null, preDiagnostics: Array.Empty<string>());
    }

    /// <summary>H000 (custom hero, uabi=A000) + A000 (custom ability) + the given script —
    /// the seed rawcodes the closure sees are therefore H000 and A000.</summary>
    private static UnitBundle ResolveH000WithAbility(string jass)
    {
        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        w3a.NewAbilities.Add(new LevelObjectModification
        { OldId = "AHbz".FromRawcode(), NewId = "A000".FromRawcode() });

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = HeroW3u(),
            ["war3map.w3a"] = Serialize(w => War3Net.Build.Extensions.BinaryWriterExtensions.Write(w, w3a)),
            ["war3map.j"] = Encoding.UTF8.GetBytes(jass),
        }));

        return BundleCommand.ResolveUnit(doc, "H000", ctx: null, preDiagnostics: Array.Empty<string>());
    }

    private static byte[] HeroW3u()
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var unit = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        unit.Modifications.Add(new SimpleObjectDataModification
        { Id = "uabi".FromRawcode(), Type = ObjectDataType.String, Value = "A000" });
        w3u.NewUnits.Add(unit);
        return Serialize(w => War3Net.Build.Extensions.BinaryWriterExtensions.Write(w, w3u));
    }

    private static byte[] Serialize(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }
}
