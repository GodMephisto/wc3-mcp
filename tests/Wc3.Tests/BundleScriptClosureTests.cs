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
