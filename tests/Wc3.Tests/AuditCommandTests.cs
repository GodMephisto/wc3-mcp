// tests/Wc3.Tests/AuditCommandTests.cs
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Every audit check, proven to fire on a map that carries exactly its defect, and proven
/// silent on one that does not.
///
/// This two-sided shape is the whole point. A check that has only ever been seen returning
/// nothing has told you about the check, not about the map, and this project has already paid
/// for that twice. A drift sweep reported zero because its column lookup was wrong, and was
/// only caught by running it against a map known to contain nine defects. An orphan check
/// reported 167 because it compared the wrong byte order of an ability id, and a tooltip
/// comparison reported clean on tooltips it could not read at all.
///
/// So each check here gets a positive case, watched firing, and a negative case, watched
/// silent. A check with only one side is a comment.
/// </summary>
public class AuditCommandTests
{
    // ---- fixtures ----------------------------------------------------------------

    /// <summary>A map holding exactly the abilities given, plus a trivial script.</summary>
    private static byte[] Map(
        Action<AbilityObjectData> abilities,
        Action<UnitObjectData>? units = null,
        string script = "function main takes nothing returns nothing\nendfunction\n",
        (string Name, byte[] Bytes)? extraFile = null)
    {
        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        abilities(w3a);
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        units?.Invoke(w3u);

        var files = new Dictionary<string, byte[]>
        {
            ["war3map.w3a"] = Serialize(w => w.Write(w3a)),
            ["war3map.w3u"] = Serialize(w => w.Write(w3u)),
            ["war3map.j"] = Encoding.UTF8.GetBytes(script),
        };
        if (extraFile is { } f) files[f.Name] = f.Bytes;
        return SyntheticMap.Build(files);
    }

    private static LevelObjectModification Ability(string code, string baseCode = "AHbz")
        => new() { OldId = baseCode.FromRawcode(), NewId = code.FromRawcode() };

    private static void Set(LevelObjectModification a, string field, int level, object value,
        ObjectDataType type = ObjectDataType.String)
        => a.Modifications.Add(new LevelObjectDataModification
        { Level = level, Pointer = 0, Id = field.FromRawcode(), Type = type, Value = value });

    private static void Levels(LevelObjectModification a, int n)
        => Set(a, "alev", 0, n, ObjectDataType.Int);

    private static SimpleObjectModification Unit(string code, string baseCode = "hfoo")
        => new() { OldId = baseCode.FromRawcode(), NewId = code.FromRawcode() };

    private static void SetU(SimpleObjectModification u, string field, object value,
        ObjectDataType type = ObjectDataType.String)
        => u.Modifications.Add(new SimpleObjectDataModification
        { Id = field.FromRawcode(), Type = type, Value = value });

    private static AuditResult Run(byte[] map, string check)
        => AuditCommand.Execute(MapDocument.Load(map), gameDirOverride: null, only: new[] { check });

    private static byte[] Serialize(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }

    /// <summary>A minimal MDX: MDLX, a VERS chunk, and optionally a CAMS chunk.</summary>
    private static byte[] Mdx(int version, bool camera)
    {
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        w.Write(Encoding.ASCII.GetBytes("MDLX"));
        w.Write(Encoding.ASCII.GetBytes("VERS"));
        w.Write(4u);
        w.Write(version);
        if (camera)
        {
            w.Write(Encoding.ASCII.GetBytes("CAMS"));
            w.Write(8u);
            w.Write(0L);
        }
        w.Flush();
        return ms.ToArray();
    }

    // ---- level-gap ---------------------------------------------------------------

    [Fact]
    public void Level_gap_fires_when_a_declared_level_has_no_value()
    {
        // The base is a rawcode nothing defines, so no fallback exists and the gap is real.
        // An earlier version of this test used AHbz, which HAS acdn at levels 1 to 3, so it
        // asserted a defect the engine does not have and only passed while the check was
        // blind to the base. Naming the arm is the fix.
        var map = Map(w3a =>
        {
            var a = Ability("A000", baseCode: "ZZZZ");
            Levels(a, 3);
            Set(a, "acdn", 1, 5.0f, ObjectDataType.Unreal);   // levels 2 and 3 missing
            w3a.NewAbilities.Add(a);
        });
        var r = Run(map, "level-gap");
        var i = Assert.Single(r.Issues);
        Assert.Equal("level-gap", i.Check);
        Assert.Equal("A000", i.Rawcode);
        Assert.Contains("acdn", i.Message);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Level_gap_is_silent_when_the_base_ability_supplies_the_missing_levels()
    {
        // AHbz carries acdn at levels 1, 2 and 3, so a map setting only level 1 is ORDINARY
        // authoring and the other two resolve. Reporting it flagged 333 fields on one real map
        // where the real number was 99, and noise at that ratio teaches a reader to skip the
        // check entirely.
        var map = Map(w3a =>
        {
            var a = Ability("A000", baseCode: "AHbz");
            Levels(a, 3);
            Set(a, "acdn", 1, 5.0f, ObjectDataType.Unreal);
            w3a.NewAbilities.Add(a);
        });
        Assert.Empty(Run(map, "level-gap").Issues);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Level_gap_fires_past_the_end_of_a_real_base_abilitys_own_levels()
    {
        // AHbz carries acdn at levels 1 to 3 only. An ability declaring 5 therefore resolves 2
        // and 3 and has nothing at 4 and 5, which is the exact defect a player reports as "the
        // level 5 spell has no cooldown".
        //
        // This case exists because the sibling test uses a base nothing defines, so it returns
        // before the per-level lookup runs and could never catch a bug in that lookup. It was
        // added after the bare-key sabotage stayed green against it.
        var map = Map(w3a =>
        {
            var a = Ability("A000", baseCode: "AHbz");
            Levels(a, 5);
            Set(a, "acdn", 1, 5.0f, ObjectDataType.Unreal);
            w3a.NewAbilities.Add(a);
        });
        var i = Assert.Single(Run(map, "level-gap").Issues);
        Assert.Contains("[4,5]", i.Message);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Level_gap_ignores_a_data_field_because_absence_there_is_how_the_author_writes_off()
    {
        // Measured on one real map, 26 of the 35 abilities setting Osh2 start at level 2 with a
        // flat sentinel, and Osh2 is written as an explicit zero exactly once in the whole map.
        var map = Map(w3a =>
        {
            var a = Ability("A000", baseCode: "ZZZZ");
            Levels(a, 5);
            Set(a, "Osh2", 2, 99999.0f, ObjectDataType.Unreal);
            Set(a, "Osh2", 3, 99999.0f, ObjectDataType.Unreal);
            w3a.NewAbilities.Add(a);
        });
        Assert.Empty(Run(map, "level-gap").Issues);
    }

    [Fact]
    public void Level_gap_is_silent_when_every_declared_level_has_a_value()
    {
        var map = Map(w3a =>
        {
            var a = Ability("A000");
            Levels(a, 3);
            for (int lv = 1; lv <= 3; lv++) Set(a, "acdn", lv, 5.0f, ObjectDataType.Unreal);
            w3a.NewAbilities.Add(a);
        });
        Assert.Empty(Run(map, "level-gap").Issues);
    }

    // ---- level-tooltip -----------------------------------------------------------

    [Fact]
    public void Level_tooltip_fires_when_a_REACHABLE_level_has_no_tooltip_of_its_own()
    {
        // The defect a player sees. Levels 1 and 2 read as the custom spell and level 3
        // silently reverts to whatever the base ability calls itself.
        var map = Map(w3a =>
        {
            var a = Ability("A000");
            Levels(a, 3);
            Set(a, "atp1", 1, "Blast - Level 1");
            Set(a, "atp1", 2, "Blast - Level 2");
            w3a.NewAbilities.Add(a);
        });
        var i = Assert.Single(Run(map, "level-tooltip").Issues);
        Assert.Contains("declares 3", i.Message);
        Assert.Contains("[3]", i.Message);
        Assert.Contains("fall back", i.Message);
    }

    [Fact]
    public void Level_tooltip_does_NOT_fire_for_text_above_the_reachable_level_count()
    {
        // Unreachable text is untidy, not broken, and warning about it fired 38 times on one
        // real map for a condition no player can observe. It is reported as a note instead,
        // and this test is the guard against it quietly becoming a warning again.
        var map = Map(w3a =>
        {
            var a = Ability("A000");
            Levels(a, 2);
            Set(a, "atp1", 1, "Blast - Level 1");
            Set(a, "atp1", 2, "Blast - Level 2");
            Set(a, "atp1", 3, "Blast - Level 3");    // dead, the ability stops at 2
            w3a.NewAbilities.Add(a);
        });
        var r = Run(map, "level-tooltip");
        Assert.Empty(r.Issues);
        Assert.Contains(r.Diagnostics, d => d.Contains("A000") && d.Contains("above the level"));
    }

    [Fact]
    public void Level_tooltip_is_silent_when_the_ability_authors_no_tooltip_at_all()
    {
        // Authoring none is a plain use of the base ability, which works. Only a PARTIAL set
        // is a defect, because that is what proves the author meant to override the text.
        var map = Map(w3a =>
        {
            var a = Ability("A000");
            Levels(a, 3);
            w3a.NewAbilities.Add(a);
        });
        Assert.Empty(Run(map, "level-tooltip").Issues);
    }

    [Fact]
    public void Level_tooltip_is_silent_when_the_counts_agree()
    {
        var map = Map(w3a =>
        {
            var a = Ability("A000");
            Levels(a, 2);
            Set(a, "atp1", 1, "Blast - Level 1");
            Set(a, "atp1", 2, "Blast - Level 2");
            w3a.NewAbilities.Add(a);
        });
        Assert.Empty(Run(map, "level-tooltip").Issues);
    }

    // ---- tooltip-claim -----------------------------------------------------------

    [Fact]
    public void Tooltip_claim_fires_when_the_prose_states_a_cooldown_the_data_contradicts()
    {
        var map = Map(w3a =>
        {
            var a = Ability("A000");
            Levels(a, 1);
            Set(a, "aub1", 1, "Hits hard. |cffc3dbffCooldown:|r 9 seconds.");
            Set(a, "acdn", 1, 13.0f, ObjectDataType.Unreal);
            w3a.NewAbilities.Add(a);
        });
        var i = Assert.Single(Run(map, "tooltip-claim").Issues);
        Assert.Contains("cooldown", i.Message);
        Assert.Contains("9", i.Message);
        Assert.Contains("13", i.Message);
    }

    [Fact]
    public void Tooltip_claim_is_silent_when_the_prose_and_the_data_agree()
    {
        var map = Map(w3a =>
        {
            var a = Ability("A000");
            Levels(a, 1);
            Set(a, "aub1", 1, "Hits hard. |cffc3dbffCooldown:|r 13 seconds.");
            Set(a, "acdn", 1, 13.0f, ObjectDataType.Unreal);
            w3a.NewAbilities.Add(a);
        });
        Assert.Empty(Run(map, "tooltip-claim").Issues);
    }

    [Fact]
    public void Tooltip_claim_does_not_fire_when_the_field_is_inherited_rather_than_set()
    {
        // No acdn on the object at all means it uses the base ability's cooldown, which this
        // check cannot see and must not guess about.
        var map = Map(w3a =>
        {
            var a = Ability("A000");
            Levels(a, 1);
            Set(a, "aub1", 1, "Hits hard. |cffc3dbffCooldown:|r 9 seconds.");
            w3a.NewAbilities.Add(a);
        });
        Assert.Empty(Run(map, "tooltip-claim").Issues);
    }

    [Fact]
    public void Tooltip_claim_reads_Area_of_Effect_as_a_radius()
    {
        // The control for the test below. Without it, that one proves only that the check
        // stopped working rather than that it stopped misreading one phrase.
        var map = Map(w3a =>
        {
            var a = Ability("A000");
            Levels(a, 1);
            Set(a, "aare", 1, 340.0f, ObjectDataType.Unreal);
            Set(a, "aub1", 1, "Slams. |cffc3dbffArea of Effect:|r 500.");
            w3a.NewAbilities.Add(a);
        });
        var i = Assert.Single(Run(map, "tooltip-claim").Issues);
        Assert.Contains("area of effect 500", i.Message);
    }

    [Fact]
    public void Tooltip_claim_does_NOT_read_Area_of_Damage_as_a_radius()
    {
        // "Area of Damage" is the damage dealt inside the area, not the area. Reading it as a
        // radius produced ten false claims on one real map, where the field carrying exactly
        // the stated number was the base ability's own column named "AOE Damage".
        var map = Map(w3a =>
        {
            var a = Ability("A000");
            Levels(a, 1);
            Set(a, "aare", 1, 340.0f, ObjectDataType.Unreal);
            Set(a, "aub1", 1, "Slams. |cffc3dbffArea of Damage:|r 500.");
            w3a.NewAbilities.Add(a);
        });
        Assert.Empty(Run(map, "tooltip-claim").Issues);
    }

    [Fact]
    public void Tooltip_claim_becomes_a_note_when_the_script_names_the_ability()
    {
        // A script that names the ability can set the value at runtime, so the stored field is
        // not what the player gets. Still reported, because the other reading is a stale
        // tooltip and only the author can tell those apart.
        var map = Map(w3a =>
        {
            var a = Ability("A000");
            Levels(a, 1);
            Set(a, "aare", 1, 100.0f, ObjectDataType.Unreal);
            Set(a, "aub1", 1, "Walls up. |cffc3dbffArea of Effect:|r 350.");
            w3a.NewAbilities.Add(a);
        },
        script: "function main takes nothing returns nothing\n"
                + "call DoNothing('A000')\nendfunction\n");
        var r = Run(map, "tooltip-claim");
        Assert.Empty(r.Issues);
        Assert.Contains(r.Diagnostics, d => d.Contains("A000") && d.Contains("set aside"));
    }

    // ---- orphan-ability ----------------------------------------------------------

    [Fact]
    public void Orphan_fires_when_effects_are_zeroed_and_no_trigger_mentions_the_ability()
    {
        var map = Map(
            w3a =>
            {
                var a = Ability("A000");
                Levels(a, 1);
                Set(a, "Hbz1", 1, 0.0f, ObjectDataType.Unreal);   // neutralised for a trigger
                w3a.NewAbilities.Add(a);
            },
            w3u =>
            {
                var u = Unit("U000");
                SetU(u, "uabi", "A000");
                w3u.NewUnits.Add(u);
            });
        var i = Assert.Single(Run(map, "orphan-ability").Issues);
        Assert.Equal("A000", i.Rawcode);
    }

    [Fact]
    public void Orphan_is_silent_when_the_script_names_the_ability_as_a_big_endian_integer()
    {
        // 'A000' is 1093677056 big-endian. War3Net's FromRawcode yields the OTHER byte order,
        // and comparing against that reported 167 false orphans on a real map.
        long beA000 = AuditCommand.BigEndian("A000");
        var map = Map(
            w3a =>
            {
                var a = Ability("A000");
                Levels(a, 1);
                Set(a, "Hbz1", 1, 0.0f, ObjectDataType.Unreal);
                w3a.NewAbilities.Add(a);
            },
            w3u =>
            {
                var u = Unit("U000");
                SetU(u, "uabi", "A000");
                w3u.NewUnits.Add(u);
            },
            script: $"function main takes nothing returns nothing\n"
                  + $"    call SetUnitAbilityLevel(null, {beA000}, 1)\n"
                  + $"endfunction\n");
        Assert.Empty(Run(map, "orphan-ability").Issues);
    }

    [Fact]
    public void Orphan_is_silent_when_the_ability_overrides_no_effect_at_all()
    {
        // Overriding nothing means it is a plain use of its base ability, which works.
        var map = Map(
            w3a =>
            {
                var a = Ability("A000");
                Levels(a, 1);
                w3a.NewAbilities.Add(a);
            },
            w3u =>
            {
                var u = Unit("U000");
                SetU(u, "uabi", "A000");
                w3u.NewUnits.Add(u);
            });
        Assert.Empty(Run(map, "orphan-ability").Issues);
    }

    [Fact]
    public void Orphan_attributes_an_item_granted_ability_to_its_item()
    {
        // Abilities granted by items were invisible to this check until items were included.
        var w3t = new ItemObjectData(ObjectDataFormatVersion.v2);
        var item = new SimpleObjectModification { OldId = "ratf".FromRawcode(), NewId = "I000".FromRawcode() };
        item.Modifications.Add(new SimpleObjectDataModification
        { Id = "iabi".FromRawcode(), Type = ObjectDataType.String, Value = "A000" });
        item.Modifications.Add(new SimpleObjectDataModification
        { Id = "unam".FromRawcode(), Type = ObjectDataType.String, Value = "Orb of Testing" });
        w3t.NewItems.Add(item);

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        var a = Ability("A000");
        Levels(a, 1);
        Set(a, "Hbz1", 1, 0.0f, ObjectDataType.Unreal);
        w3a.NewAbilities.Add(a);

        var map = SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3a"] = Serialize(w => w.Write(w3a)),
            ["war3map.w3t"] = Serialize(w => w.Write(w3t)),
            ["war3map.j"] = Encoding.UTF8.GetBytes("function main takes nothing returns nothing\nendfunction\n"),
        });
        var i = Assert.Single(Run(map, "orphan-ability").Issues);
        Assert.Equal("A000", i.Rawcode);
        Assert.Contains("Orb of Testing", i.Owner);
    }

    // ---- portrait-risk -----------------------------------------------------------

    [Fact]
    public void Portrait_risk_fires_for_a_model_with_a_camera_below_version_900()
    {
        var map = Map(
            w3a => { },
            w3u =>
            {
                var u = Unit("U000");
                SetU(u, "umdl", "Hero.mdx");
                SetU(u, "unam", "Test Hero");
                w3u.NewUnits.Add(u);
            },
            extraFile: ("Hero.mdx", Mdx(800, camera: true)));
        var i = Assert.Single(Run(map, "portrait-risk").Issues);
        Assert.Equal("U000", i.Rawcode);
        Assert.Contains("800", i.Message);
    }

    [Fact]
    public void Portrait_risk_is_silent_for_the_same_model_without_a_camera()
    {
        var map = Map(
            w3a => { },
            w3u =>
            {
                var u = Unit("U000");
                SetU(u, "umdl", "Hero.mdx");
                w3u.NewUnits.Add(u);
            },
            extraFile: ("Hero.mdx", Mdx(800, camera: false)));
        Assert.Empty(Run(map, "portrait-risk").Issues);
    }

    [Fact]
    public void Portrait_risk_is_silent_for_a_modern_model_that_has_a_camera()
    {
        var map = Map(
            w3a => { },
            w3u =>
            {
                var u = Unit("U000");
                SetU(u, "umdl", "Hero.mdx");
                w3u.NewUnits.Add(u);
            },
            extraFile: ("Hero.mdx", Mdx(1800, camera: true)));
        Assert.Empty(Run(map, "portrait-risk").Issues);
    }

    // ---- dangling-reference ------------------------------------------------------
    //
    // Both sides need the installed game, because the check's whole job is to tell a stale id
    // apart from a legitimate base game one, and only the game data knows the difference. A
    // hermetic version of this test would report every base game buff as dangling.

    [Fact]
    [Trait("Category", "GameData")]
    public void Dangling_reference_fires_for_a_buff_id_nothing_defines()
    {
        var map = Map(w3a =>
        {
            var a = Ability("A000");
            Levels(a, 1);
            Set(a, "abuf", 1, "ZZZZ");
            w3a.NewAbilities.Add(a);
        });
        var i = Assert.Single(Run(map, "dangling-reference").Issues);
        Assert.Equal("A000", i.Rawcode);
        Assert.Contains("ZZZZ", i.Message);
        Assert.Equal(DiagnosticSeverity.Error, i.Severity);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Dangling_reference_is_silent_for_a_base_game_buff_the_map_never_defines()
    {
        // Bfro is Frost Armor, shipped with the game and entirely correct to name here.
        // Checking only what the map defines reported 284 of these on one real map.
        var map = Map(w3a =>
        {
            var a = Ability("A000");
            Levels(a, 1);
            Set(a, "abuf", 1, "Bfro");
            w3a.NewAbilities.Add(a);
        });
        Assert.Empty(Run(map, "dangling-reference").Issues);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Dangling_reference_is_silent_for_a_buff_the_game_defines_only_in_its_skin_data()
    {
        // BHtb has no row in the buff table. AbilitySkin.txt defines it with skinType=buff, and
        // the base ability ACcb names it as BuffID1. Reporting it made the repair's own restored
        // value look dangling on 25 bisection maps.
        var map = Map(w3a =>
        {
            var a = Ability("A000", "ACcb");
            Levels(a, 1);
            Set(a, "abuf", 1, "BHtb");
            w3a.NewAbilities.Add(a);
        });
        Assert.Empty(Run(map, "dangling-reference").Issues);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Dangling_reference_is_silent_for_an_ability_the_map_itself_defines()
    {
        var map = Map(w3a =>
        {
            var a = Ability("A000");
            Levels(a, 1);
            Set(a, "abuf", 1, "A001");
            w3a.NewAbilities.Add(a);
            var b = Ability("A001");
            Levels(b, 1);
            w3a.NewAbilities.Add(b);
        });
        Assert.Empty(Run(map, "dangling-reference").Issues);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Dangling_reference_states_the_population_it_walked_even_when_it_finds_nothing()
    {
        var map = Map(w3a =>
        {
            var a = Ability("A000");
            Levels(a, 1);
            Set(a, "abuf", 1, "Bfro");
            w3a.NewAbilities.Add(a);
        });
        var r = Run(map, "dangling-reference");
        Assert.Empty(r.Issues);
        Assert.Contains(r.Diagnostics, d => d.Contains("examined 1 rawcode reference"));
    }

    // ---- repair audit-errors -----------------------------------------------------
    //
    // The repair shares the audit's detection, so each case is proven by the audit going
    // silent afterwards, and the entries that were not dangling are proven kept verbatim.

    private static (MapDocument Doc, AuditRepairResult R) Repair(byte[] map, bool apply = true)
    {
        var doc = MapDocument.Load(map);
        return (doc, AuditRepairCommand.Execute(doc, apply));
    }

    private static byte[] Saved(MapDocument doc) => doc.SaveToBytes();

    [Fact]
    [Trait("Category", "GameData")]
    public void Repair_removes_a_dead_uabi_id_and_keeps_every_other_entry()
    {
        var map = Map(_ => { }, w3u =>
        {
            var u = Unit("h000");
            SetU(u, "uabi", "Aloc,ZZZZ,AHbz");
            w3u.NewUnits.Add(u);
        });
        var (doc, r) = Repair(map);
        Assert.True(r.Ok, r.Message);
        var e = Assert.Single(r.Edits);
        Assert.Equal(("uabi", "Aloc,AHbz"), (e.Field, e.After));
        var fixedMap = Saved(doc);
        Assert.Empty(Run(fixedMap, "dangling-reference").Issues);
        var u = ObjectGetCommand.Execute(MapDocument.Load(fixedMap), ObjectKind.Unit, "h000", null);
        Assert.Contains(u.Fields, f => f.Value == "Aloc,AHbz");
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Repair_restores_a_missing_buff_from_the_base_ability()
    {
        // GGGA_V0.07b's A1A3 is exactly this, a Storm Bolt copy naming B06H on every level.
        var map = Map(w3a =>
        {
            var a = Ability("A000", "AHtb");
            Levels(a, 5);
            for (int l = 1; l <= 5; l++) Set(a, "abuf", l, "ZZZZ");
            w3a.NewAbilities.Add(a);
        });
        var (doc, r) = Repair(map);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(5, r.Edits.Count);
        Assert.All(r.Edits, e => Assert.Equal("BPSE", e.After));
        Assert.Empty(Run(Saved(doc), "dangling-reference").Issues);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Repair_clears_an_inherited_requirement_at_level_zero()
    {
        // Absk inherits the Berserker upgrade Robk, which this map never defines.
        var map = Map(w3a =>
        {
            var a = Ability("A000", "Absk");
            Levels(a, 1);
            w3a.NewAbilities.Add(a);
        });
        Assert.NotEmpty(Run(map, "requirement").Issues);
        var (doc, r) = Repair(map);
        Assert.True(r.Ok, r.Message);
        var e = Assert.Single(r.Edits);
        Assert.Equal(("areq:0", "_"), (e.Field, e.After));
        Assert.Empty(Run(Saved(doc), "requirement").Issues);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Repair_dry_run_lists_the_edits_and_changes_nothing()
    {
        var map = Map(_ => { }, w3u =>
        {
            var u = Unit("h000");
            SetU(u, "uabi", "ZZZZ");
            w3u.NewUnits.Add(u);
        });
        var (doc, r) = Repair(map, apply: false);
        Assert.Single(r.Edits);
        Assert.NotEmpty(Run(Saved(doc), "dangling-reference").Issues);
    }

    [Fact]
    public void Mdx_header_reader_reports_version_and_camera_presence()
    {
        Assert.Equal((800, true), AuditCommand.ReadMdxHeader(Mdx(800, true)));
        Assert.Equal((1800, false), AuditCommand.ReadMdxHeader(Mdx(1800, false)));
        Assert.Equal((null, false), AuditCommand.ReadMdxHeader(Encoding.ASCII.GetBytes("not a model")));
    }
}
