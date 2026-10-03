// tests/Wc3.Tests/UnitAbilitiesCommandTests.cs
using System.Numerics;
using System.Text;
using War3Net.Build.Common;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Build.Widget;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Tests for UnitAbilitiesCommand covering ScriptIndex.Build, LevelledValues,
/// synthetic map execution, and corpus map inspection.
/// </summary>
public class UnitAbilitiesCommandTests
{
    // ------------------------------------------------------------------------
    // Tests 1-9: ScriptIndex.Build pure string tests, no map needed.
    // ------------------------------------------------------------------------

    [Fact]
    public void ScriptIndex_Build_rule_a_ties_unit_and_ability_on_same_line()
    {
        string script =
            "globals\n"
            + "integer Hero_ID= 'H000'\n"
            + "integer HeroF_ID= 'A0GF'\n"
            + "endglobals\n"
            + "function Helper takes unit u returns nothing\n"
            + "    call HeroData_create(u, Hero_ID, HeroF_ID)\n"
            + "endfunction\n";

        var units = new HashSet<string>(StringComparer.Ordinal) { "H000" };
        var abilities = new HashSet<string>(StringComparer.Ordinal) { "A0GF" };

        var index = UnitAbilitiesCommand.ScriptIndex.Build(script, units, abilities);
        Assert.Single(index.ByUnit);
        Assert.True(index.ByUnit.ContainsKey("H000"));
        var hits = index.ByUnit["H000"];
        Assert.Single(hits);
        Assert.Equal("A0GF", hits[0].Ability);
        Assert.Equal(6, hits[0].Line);
    }

    [Fact]
    public void ScriptIndex_Build_hex_rawcode_global_resolves_to_same_ability()
    {
        string script =
            "globals\n"
            + "integer Hero_ID= $48303030\n"
            + "integer HeroF_ID= 'A0GF'\n"
            + "endglobals\n"
            + "function Helper takes unit u returns nothing\n"
            + "    call HeroData_create(u, Hero_ID, HeroF_ID)\n"
            + "endfunction\n";

        var units = new HashSet<string>(StringComparer.Ordinal) { "H000" };
        var abilities = new HashSet<string>(StringComparer.Ordinal) { "A0GF" };

        var index = UnitAbilitiesCommand.ScriptIndex.Build(script, units, abilities);
        var hits = index.ByUnit["H000"];
        Assert.Single(hits);
        Assert.Equal("A0GF", hits[0].Ability);
    }

    [Fact]
    public void ScriptIndex_Build_rule_b_ties_ability_inside_if_branch()
    {
        string script =
            "function OnUnitEntered takes nothing returns nothing\n"
            + "    local unit u= GetTriggerUnit()\n"
            + "    if GetUnitTypeId(u) == 'H01Z' then\n"
            + "        call MyHelper(u, 'ky01')\n"
            + "    endif\n"
            + "endfunction\n";

        var units = new HashSet<string>(StringComparer.Ordinal) { "H01Z" };
        var abilities = new HashSet<string>(StringComparer.Ordinal) { "ky01" };

        var index = UnitAbilitiesCommand.ScriptIndex.Build(script, units, abilities);
        Assert.Single(index.ByUnit);
        Assert.True(index.ByUnit.ContainsKey("H01Z"));
        var hits = index.ByUnit["H01Z"];
        Assert.Single(hits);
        Assert.Equal("ky01", hits[0].Ability);
        Assert.Equal(4, hits[0].Line);
    }

    [Fact]
    public void ScriptIndex_Build_else_branch_does_not_tie_ability()
    {
        string script =
            "function OnUpdate takes nothing returns nothing\n"
            + "    if GetUnitTypeId(u) == 'H000' then\n"
            + "        call Nothing()\n"
            + "    else\n"
            + "        call UnitAddAbility(u, 'A0ZZ')\n"
            + "    endif\n"
            + "endfunction\n";

        var units = new HashSet<string>(StringComparer.Ordinal) { "H000" };
        var abilities = new HashSet<string>(StringComparer.Ordinal) { "A0ZZ" };

        var index = UnitAbilitiesCommand.ScriptIndex.Build(script, units, abilities);
        Assert.False(index.ByUnit.ContainsKey("H000"));
    }

    [Fact]
    public void ScriptIndex_Build_elseif_replaces_tested_unit()
    {
        string script =
            "function Check takes nothing returns nothing\n"
            + "    if id == 'H000' then\n"
            + "        call Nothing()\n"
            + "    elseif id == 'H001' then\n"
            + "        call UnitAddAbility(u, 'A111')\n"
            + "    endif\n"
            + "endfunction\n";

        var units = new HashSet<string>(StringComparer.Ordinal) { "H000", "H001" };
        var abilities = new HashSet<string>(StringComparer.Ordinal) { "A111" };

        var index = UnitAbilitiesCommand.ScriptIndex.Build(script, units, abilities);
        Assert.True(index.ByUnit.ContainsKey("H001"));
        Assert.False(index.ByUnit.ContainsKey("H000"));
        var hits = index.ByUnit["H001"];
        Assert.Single(hits);
        Assert.Equal("A111", hits[0].Ability);
    }

    [Fact]
    public void ScriptIndex_Build_unrelated_lines_do_not_tie()
    {
        string script =
            "function Loop takes nothing returns nothing\n"
            + "    local integer i\n"
            + "    set i = 'H000'\n"
            + "    call UnitAddAbility(u, 'A0ZZ')\n"
            + "endfunction\n";

        var units = new HashSet<string>(StringComparer.Ordinal) { "H000" };
        var abilities = new HashSet<string>(StringComparer.Ordinal) { "A0ZZ" };

        var index = UnitAbilitiesCommand.ScriptIndex.Build(script, units, abilities);
        Assert.False(index.ByUnit.ContainsKey("H000"));
    }

    [Fact]
    public void ScriptIndex_Build_function_boundary_clears_branches()
    {
        string script =
            "function First takes nothing returns nothing\n"
            + "    if id == 'H000' then\n"
            + "        call Nothing()\n"
            + "    endif\n"
            + "endfunction\n"
            + "function Second takes nothing returns nothing\n"
            + "    call UnitAddAbility(u, 'A0ZZ')\n"
            + "endfunction\n";

        var units = new HashSet<string>(StringComparer.Ordinal) { "H000" };
        var abilities = new HashSet<string>(StringComparer.Ordinal) { "A0ZZ" };

        var index = UnitAbilitiesCommand.ScriptIndex.Build(script, units, abilities);
        Assert.False(index.ByUnit.ContainsKey("H000"));
    }

    [Fact]
    public void ScriptIndex_Build_comment_rawcodes_are_ignored()
    {
        string script =
            "function Example takes nothing returns nothing\n"
            + "    call UnitAddAbility(u, 'H000') // 'A0ZZ' is just a comment\n"
            + "endfunction\n";

        var units = new HashSet<string>(StringComparer.Ordinal) { "H000" };
        var abilities = new HashSet<string>(StringComparer.Ordinal) { "A0ZZ" };

        var index = UnitAbilitiesCommand.ScriptIndex.Build(script, units, abilities);
        Assert.False(index.ByUnit.ContainsKey("H000"));
    }

    [Fact]
    public void ScriptIndex_Build_unknown_rawcodes_do_not_count()
    {
        string script =
            "function Test takes nothing returns nothing\n"
            + "    call UnitAddAbility(u, 'A0ZZ')\n"
            + "    if id == 'HZZZ' then\n"
            + "        call Nothing()\n"
            + "    endif\n"
            + "endfunction\n";

        var units = new HashSet<string>(StringComparer.Ordinal) { "H000" };
        var abilities = new HashSet<string>(StringComparer.Ordinal) { "A0ZZ" };

        var index = UnitAbilitiesCommand.ScriptIndex.Build(script, units, abilities);
        Assert.False(index.ByUnit.ContainsKey("H000"));
    }

    // ------------------------------------------------------------------------
    // Tests 10-12: LevelledValues using hand-crafted MergedObjectResult.
    // ------------------------------------------------------------------------

    private static MergedObjectResult MakeAbility(
        string rawcode, string? baseRawcode, string? spb1,
        IEnumerable<KeyValuePair<string, string>>? leveled)
    {
        var fields = new List<MergedField>();
        if (spb1 is not null)
            fields.Add(new MergedField("spb1", "spb1", spb1, "map"));
        foreach (var kv in leveled ?? Array.Empty<KeyValuePair<string, string>>())
            fields.Add(new MergedField(kv.Key, kv.Key, kv.Value, "map"));
        return new MergedObjectResult(rawcode, true, baseRawcode, rawcode, fields, Array.Empty<string>());
    }

    [Fact]
    public void LevelledValues_uses_levelled_entries_when_they_exist()
    {
        var ability = MakeAbility(
            "S001", "Aspb", "A002,A003",
            new KeyValuePair<string, string>[]
            {
                new("spb1:1", "A002"),
                new("spb1:2", "A003"),
                new("alev", "2"),
            });

        var values = UnitAbilitiesCommand.LevelledValues(ability, "spb1").ToList();
        Assert.Equal(new[] { "A002", "A003" }, values);
    }

    [Fact]
    public void LevelledValues_falls_back_to_bare_when_no_levelled_entries_exist()
    {
        var ability = MakeAbility(
            "S000", "Aspb", "A001", null);

        var values = UnitAbilitiesCommand.LevelledValues(ability, "spb1").ToList();
        Assert.Single(values);
        Assert.Equal("A001", values[0]);
    }

    [Fact]
    public void LevelledValues_skips_empty_and_dash_values()
    {
        var ability = MakeAbility(
            "S001", "Aspb", null,
            new KeyValuePair<string, string>[]
            {
                new("spb1:1", "A001"),
                new("spb1:2", "-"),
                new("spb1:3", "_"),
                new("spb1:4", ""),
                new("alev", "4"),
            });

        var values = UnitAbilitiesCommand.LevelledValues(ability, "spb1").ToList();
        Assert.Single(values);
        Assert.Equal("A001", values[0]);
    }

    // ------------------------------------------------------------------------
    // Tests 13-17: Synthetic map tests.
    // ------------------------------------------------------------------------

    private static MapDocument BuildHeroMap(string script = "", int dummyCount = 0,
        string? placedUnitType = null)
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var hero = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        hero.Modifications.Add(new SimpleObjectDataModification
        { Id = "uhab".FromRawcode(), Type = ObjectDataType.String, Value = "A000,A001" });
        w3u.NewUnits.Add(hero);

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        var ab0 = new LevelObjectModification { OldId = "AHbz".FromRawcode(), NewId = "A000".FromRawcode() };
        w3a.NewAbilities.Add(ab0);
        var ab1 = new LevelObjectModification { OldId = "AHbz".FromRawcode(), NewId = "A001".FromRawcode() };
        w3a.NewAbilities.Add(ab1);

        var files = new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = SerializeObject(w3u),
            ["war3map.w3a"] = SerializeObject(w3a),
        };

        if (script.Length > 0)
            files["war3map.j"] = Encoding.UTF8.GetBytes(script);

        var doc = MapDocument.Load(SyntheticMap.Build(files));

        if (placedUnitType is not null)
        {
            var ability = new ModifiedAbilityData
            {
                AbilityId = "A009".FromRawcode(),
                HeroAbilityLevel = 3,
                IsAutocastActive = true,
            };
            // Place dummy units first so the special unit gets creation number == dummyCount.
            for (int i = 0; i < dummyCount; i++)
            {
                PlacementCommand.PlaceUnit(doc, placedUnitType, 0, 0f, 0f);
            }
            var result = PlacementCommand.PlaceUnit(doc, placedUnitType, 0, 0f, 0f);
            var units = PlacementCommand.GetOrCreateUnits(doc);
            var placed = units.Units.FirstOrDefault(u => u.CreationNumber == result.CreationNumber);
            if (placed is not null)
            {
                placed.AbilityData.Add(new ModifiedAbilityData
                {
                    AbilityId = ability.AbilityId,
                    HeroAbilityLevel = ability.HeroAbilityLevel,
                    IsAutocastActive = ability.IsAutocastActive,
                });
                doc.AddOrReplaceModelFile(PlacementCommand.UnitsFile, units);
            }
        }

        return doc;
    }

    private static byte[] SerializeObject(UnitObjectData w3u)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true);
        bw.Write(w3u);
        return ms.ToArray();
    }

    private static byte[] SerializeObject(AbilityObjectData w3a)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true);
        bw.Write(w3a);
        return ms.ToArray();
    }

    [Fact]
    public void Execute_ObjectHero_lists_uhab_abilities_with_source_ObjectHero()
    {
        var doc = BuildHeroMap();
        var result = UnitAbilitiesCommand.Execute(doc, "H000", gameDir: null);

        Assert.True(result.Found);
        Assert.Equal("H000", result.Unit);

        var heroAbilities = result.Abilities
            .Where(a => a.Source == AbilitySource.ObjectHero)
            .ToList();
        Assert.Equal(2, heroAbilities.Count);
        Assert.Contains("A000", heroAbilities.Select(a => a.Rawcode));
        Assert.Contains("A001", heroAbilities.Select(a => a.Rawcode));
        Assert.All(heroAbilities, a => { Assert.False(a.Inferred); });
    }

    [Fact]
    public void Execute_Spellbook_lists_inner_abilities_with_source_Spellbook()
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var hero = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        hero.Modifications.Add(new SimpleObjectDataModification
        { Id = "uhab".FromRawcode(), Type = ObjectDataType.String, Value = "S000" });
        w3u.NewUnits.Add(hero);

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        var book = new LevelObjectModification { OldId = "Aspb".FromRawcode(), NewId = "S000".FromRawcode() };
        book.Modifications.Add(new LevelObjectDataModification
        { Level = 1, Pointer = 1, Id = "spb1".FromRawcode(), Type = ObjectDataType.String, Value = "A002" });
        book.Modifications.Add(new LevelObjectDataModification
        { Level = 1, Pointer = 1, Id = "alev".FromRawcode(), Type = ObjectDataType.Int, Value = 1 });
        w3a.NewAbilities.Add(book);

        var plain = new LevelObjectModification { OldId = "AHbz".FromRawcode(), NewId = "A002".FromRawcode() };
        w3a.NewAbilities.Add(plain);

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = SerializeObject(w3u),
            ["war3map.w3a"] = SerializeObject(w3a),
        }));

        var result = UnitAbilitiesCommand.Execute(doc, "H000", gameDir: null);
        Assert.True(result.Found);

        var bookAbilities = result.Abilities
            .Where(a => a.Source == AbilitySource.Spellbook)
            .ToList();
        Assert.Single(bookAbilities);
        Assert.Equal("A002", bookAbilities[0].Rawcode);
        Assert.Equal("S000", bookAbilities[0].Detail);
    }

    [Fact]
    public void Execute_Spellbook_cycle_completes_without_hanging()
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var hero = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        hero.Modifications.Add(new SimpleObjectDataModification
        { Id = "uhab".FromRawcode(), Type = ObjectDataType.String, Value = "S000" });
        w3u.NewUnits.Add(hero);

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        var bookA = new LevelObjectModification { OldId = "Aspb".FromRawcode(), NewId = "S000".FromRawcode() };
        bookA.Modifications.Add(new LevelObjectDataModification
        { Level = 1, Pointer = 1, Id = "spb1".FromRawcode(), Type = ObjectDataType.String, Value = "S001" });
        bookA.Modifications.Add(new LevelObjectDataModification
        { Level = 1, Pointer = 1, Id = "alev".FromRawcode(), Type = ObjectDataType.Int, Value = 1 });
        w3a.NewAbilities.Add(bookA);

        var bookB = new LevelObjectModification { OldId = "Aspb".FromRawcode(), NewId = "S001".FromRawcode() };
        bookB.Modifications.Add(new LevelObjectDataModification
        { Level = 1, Pointer = 1, Id = "spb1".FromRawcode(), Type = ObjectDataType.String, Value = "S000" });
        bookB.Modifications.Add(new LevelObjectDataModification
        { Level = 1, Pointer = 1, Id = "alev".FromRawcode(), Type = ObjectDataType.Int, Value = 1 });
        w3a.NewAbilities.Add(bookB);

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = SerializeObject(w3u),
            ["war3map.w3a"] = SerializeObject(w3a),
        }));

        var sw = System.Diagnostics.Stopwatch.StartNew();
        var result = UnitAbilitiesCommand.Execute(doc, "H000", gameDir: null);
        sw.Stop();
        Assert.True(result.Found);
        Assert.True(sw.ElapsedMilliseconds < 5000, "spellbook cycle did not complete in time");
    }

    [Fact]
    public void Execute_Script_ties_ability_from_war3map_j()
    {
        string script =
            "globals\n"
            + "integer MyHero_ID= 'H000'\n"
            + "integer Spell_ID= 'A005'\n"
            + "endglobals\n"
            + "function ApplyAbility takes nothing returns nothing\n"
            + "    call HeroData_create(MyHero_ID, Spell_ID)\n"
            + "endfunction\n";

        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var hero = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        // No uhab here: A005 only comes from script, proving Source = Script.
        w3u.NewUnits.Add(hero);

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        w3a.NewAbilities.Add(new LevelObjectModification { OldId = "AHbz".FromRawcode(), NewId = "A005".FromRawcode() });

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = SerializeObject(w3u),
            ["war3map.w3a"] = SerializeObject(w3a),
            ["war3map.j"] = Encoding.UTF8.GetBytes(script),
        }));

        var result = UnitAbilitiesCommand.Execute(doc, "H000", gameDir: null);
        Assert.True(result.Found);

        var scriptAbilities = result.Abilities
            .Where(a => a.Source == AbilitySource.Script)
            .ToList();
        Assert.Single(scriptAbilities);
        Assert.Equal("A005", scriptAbilities[0].Rawcode);
        Assert.True(scriptAbilities[0].Inferred);
    }

    [Fact]
    public void Execute_PlaceUnit_includes_ModifiedAbilityData_with_source_PlacedUnit()
    {
        // Place 7 dummy units so the special unit gets creation number 7.
        var doc = BuildHeroMap(dummyCount: 7, placedUnitType: "H000");

        var result = UnitAbilitiesCommand.Execute(doc, "H000", gameDir: null, placedCreationNumber: 7);

        Assert.True(result.Found);
        Assert.Equal(7, result.PlacedCreationNumber);

        var placedAbilities = result.Abilities
            .Where(a => a.Source == AbilitySource.PlacedUnit)
            .ToList();
        Assert.Single(placedAbilities);
        Assert.Equal("A009", placedAbilities[0].Rawcode);
        Assert.Equal(3, placedAbilities[0].Level);
        Assert.True(placedAbilities[0].Autocast);
    }

    [Fact]
    public void Execute_PlaceUnit_with_missing_creation_number_reports_not_found()
    {
        var doc = BuildHeroMap(dummyCount: 7, placedUnitType: "H000");

        var result = UnitAbilitiesCommand.Execute(doc, "H000", gameDir: null, placedCreationNumber: 99);
        Assert.False(result.Found);
    }

    // ------------------------------------------------------------------------
    // Test 18: Corpus test, skipped cleanly when the map is absent.
    // ------------------------------------------------------------------------

    [Fact]
    [Trait("Category", "Corpus")]
    public void Corpus_Anime_WOS2_0_32I_H000_reports_expected_abilities()
    {
        string path = TestCorpus.Map("Anime_WOS2_0.32I.w3x");
        if (!File.Exists(path)) return;

        var doc = MapDocument.Load(path);
        var result = UnitAbilitiesCommand.Execute(doc, "H000", gameDir: null);

        Assert.True(result.Found);
        var rawcodes = result.Abilities.Select(a => a.Rawcode).ToHashSet(StringComparer.Ordinal);
        Assert.Contains("A000", rawcodes);
        Assert.Contains("A001", rawcodes);
        Assert.Contains("A002", rawcodes);
        Assert.Contains("A003", rawcodes);
        Assert.Contains("A004", rawcodes);
        Assert.Contains("A005", rawcodes);
        Assert.Contains("A0GF", rawcodes);
        Assert.Contains("A0GD", rawcodes);
    }
}
