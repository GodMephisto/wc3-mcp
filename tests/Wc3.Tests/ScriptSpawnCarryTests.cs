// tests/Wc3.Tests/ScriptSpawnCarryTests.cs
// Coverage for the script-spawn / grant object carry in BundleCommand: a unit a carried handler
// spawns (CreateUnit and kin) or an ability it grants (UnitAddAbility) joins the bundle, a spawn in
// another hero's dispatch branch does not, and the guarded assigner carry pulls in the one setup
// function that populates a per-player unit array a carried handler reads.
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public class ScriptSpawnCarryTests
{
    [Fact]
    public void A_unit_a_carried_handler_spawns_is_carried()
    {
        const string jass =
            "function Trig_Spell_Actions takes nothing returns nothing\n" +
            "    if GetSpellAbilityId() == 'A000' then\n" +
            "        call CreateUnit(Player(0), 'e001', 0., 0., 0.)\n" +
            "    endif\n" +
            "endfunction\n";
        var bundle = Resolve(jass, units: new[] { "e001" });

        Assert.Contains(bundle.Objects, o => o.Rawcode == "e001" && o.Kind == ObjectKind.Unit);
    }

    [Fact]
    public void An_ability_a_carried_handler_grants_is_carried()
    {
        const string jass =
            "function Trig_Spell_Actions takes nothing returns nothing\n" +
            "    if GetSpellAbilityId() == 'A000' then\n" +
            "        call UnitAddAbility(GetTriggerUnit(), 'A001')\n" +
            "    endif\n" +
            "endfunction\n";
        var bundle = Resolve(jass, abilities: new[] { "A001" });

        Assert.Contains(bundle.Objects, o => o.Rawcode == "A001" && o.Kind == ObjectKind.Ability);
    }

    [Fact]
    public void A_spawn_in_a_foreign_dispatch_branch_is_not_carried()
    {
        // A shared dispatcher spawns our dummy in our branch and another hero's dummy in the branch
        // guarded by that hero's rawcode. Only ours may be carried, the foreign branch is scoped out.
        const string jass =
            "function CastDispatch takes nothing returns nothing\n" +
            "    if GetUnitTypeId(GetSpellAbilityUnit()) == 'H000' then\n" +
            "        call CreateUnit(Player(0), 'e001', 0., 0., 0.)\n" +
            "    endif\n" +
            "    if GetUnitTypeId(GetSpellAbilityUnit()) == 'H001' then\n" +
            "        call CreateUnit(Player(0), 'e002', 0., 0., 0.)\n" +
            "    endif\n" +
            "endfunction\n";
        var bundle = ResolveTwoHeroes(jass, units: new[] { "e001", "e002" });

        Assert.Contains(bundle.Objects, o => o.Rawcode == "e001");    // our branch's spawn
        Assert.DoesNotContain(bundle.Objects, o => o.Rawcode == "e002"); // the foreign branch's spawn
    }

    [Fact]
    public void The_setup_function_that_populates_a_read_array_is_carried_with_its_dummy()
    {
        // A carried handler reads a per-player unit array it never assigns. The one qualifying setup
        // function (takes a player, creates the units, no rawcode branch) is carried, so its dummy
        // 'n001' comes along, the doer-dummy recovery.
        const string jass =
            "globals\n" +
            "    unit array udg_Dummy\n" +
            "endglobals\n" +
            "function Setup takes player p returns nothing\n" +
            "    set udg_Dummy[GetPlayerId(p)] = CreateUnit(p, 'n001', 0., 0., 0.)\n" +
            "endfunction\n" +
            "function Trig_Spell_Actions takes nothing returns nothing\n" +
            "    if GetSpellAbilityId() == 'A000' then\n" +
            "        call IssueTargetOrder(udg_Dummy[GetPlayerId(GetOwningPlayer(GetTriggerUnit()))], \"attack\", GetTriggerUnit())\n" +
            "    endif\n" +
            "endfunction\n";
        var bundle = Resolve(jass, units: new[] { "n001" });

        Assert.Contains(bundle.Functions, f => f.Name == "Setup");
        Assert.Contains(bundle.Objects, o => o.Rawcode == "n001" && o.Kind == ObjectKind.Unit);
    }

    [Fact]
    public void An_ambiguous_array_setup_is_declined_not_guessed()
    {
        // Two setup functions both qualify to populate the read array. The carry declines rather than
        // guess, so neither dummy is pulled in and a diagnostic explains why.
        const string jass =
            "globals\n" +
            "    unit array udg_Dummy\n" +
            "endglobals\n" +
            "function SetupA takes player p returns nothing\n" +
            "    set udg_Dummy[GetPlayerId(p)] = CreateUnit(p, 'n001', 0., 0., 0.)\n" +
            "endfunction\n" +
            "function SetupB takes player p returns nothing\n" +
            "    set udg_Dummy[GetPlayerId(p)] = CreateUnit(p, 'n002', 0., 0., 0.)\n" +
            "endfunction\n" +
            "function Trig_Spell_Actions takes nothing returns nothing\n" +
            "    if GetSpellAbilityId() == 'A000' then\n" +
            "        call KillUnit(udg_Dummy[GetPlayerId(GetOwningPlayer(GetTriggerUnit()))])\n" +
            "    endif\n" +
            "endfunction\n";
        var bundle = Resolve(jass, units: new[] { "n001", "n002" });

        Assert.DoesNotContain(bundle.Functions, f => f.Name == "SetupA" || f.Name == "SetupB");
        Assert.DoesNotContain(bundle.Objects, o => o.Rawcode == "n001" || o.Rawcode == "n002");
        Assert.Contains(bundle.Diagnostics, d => d.Contains("per-player unit array"));
    }

    // ---- synthetic map construction ------------------------------------------

    /// <summary>H000 (hero, uabi=A000) + A000 (custom ability) + the given custom units and abilities
    /// and script. Resolved with no game data so the crawl never touches the local install.</summary>
    private static UnitBundle Resolve(string jass, string[]? units = null, string[]? abilities = null)
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var hero = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        hero.Modifications.Add(new SimpleObjectDataModification
        { Id = "uabi".FromRawcode(), Type = ObjectDataType.String, Value = "A000" });
        w3u.NewUnits.Add(hero);
        foreach (var u in units ?? Array.Empty<string>())
            w3u.NewUnits.Add(new SimpleObjectModification { OldId = "hfoo".FromRawcode(), NewId = u.FromRawcode() });

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        w3a.NewAbilities.Add(new LevelObjectModification { OldId = "AHbz".FromRawcode(), NewId = "A000".FromRawcode() });
        foreach (var a in abilities ?? Array.Empty<string>())
            w3a.NewAbilities.Add(new LevelObjectModification { OldId = "AHbz".FromRawcode(), NewId = a.FromRawcode() });

        return BundleFrom(w3u, w3a, jass);
    }

    /// <summary>Like <see cref="Resolve"/> but also defines a second custom hero H001, so a dispatch
    /// branch guarded by 'H001' is recognised as a foreign hero and scoped out.</summary>
    private static UnitBundle ResolveTwoHeroes(string jass, string[]? units = null)
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var hero = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        hero.Modifications.Add(new SimpleObjectDataModification
        { Id = "uabi".FromRawcode(), Type = ObjectDataType.String, Value = "A000" });
        w3u.NewUnits.Add(hero);
        w3u.NewUnits.Add(new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H001".FromRawcode() });
        foreach (var u in units ?? Array.Empty<string>())
            w3u.NewUnits.Add(new SimpleObjectModification { OldId = "hfoo".FromRawcode(), NewId = u.FromRawcode() });

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        w3a.NewAbilities.Add(new LevelObjectModification { OldId = "AHbz".FromRawcode(), NewId = "A000".FromRawcode() });

        return BundleFrom(w3u, w3a, jass);
    }

    private static UnitBundle BundleFrom(UnitObjectData w3u, AbilityObjectData w3a, string jass)
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Ser(w => w.Write(w3u)),
            ["war3map.w3a"] = Ser(w => w.Write(w3a)),
            ["war3map.j"] = Encoding.UTF8.GetBytes(jass),
        }));
        return BundleCommand.ResolveUnit(doc, "H000", ctx: null, preDiagnostics: Array.Empty<string>());
    }

    private static byte[] Ser(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }
}
