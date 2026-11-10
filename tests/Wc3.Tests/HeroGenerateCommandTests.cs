// tests/Wc3.Tests/HeroGenerateCommandTests.cs
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Proves the hero generator against a synthetic map that defines the two bases it needs, so the
/// tests need no game install and no corpus. Every expected artifact is read back through the same
/// readers the rest of the toolkit uses, never through the generator's own result object.
/// </summary>
public class HeroGenerateCommandTests
{
    private const string Spec = """
    {
      "name": "Kurosaki Ichigo",
      "base_unit": "Hpal",
      "forms": [
        { "id": "base", "abilities": [
          { "slot": "Q", "name": "Getsuga Tenshou", "base": "ANcl", "cooldown": [8,8,8,8], "cast": 0.35, "damage": 120 },
          { "slot": "R", "name": "Bankai", "kind": "transform", "to": "bankai" } ] },
        { "id": "bankai", "abilities": [
          { "slot": "Q", "name": "Black Getsuga", "base": "ANcl", "cooldown": [9,9,9,9], "cast": 0.35, "damage": 180 } ] } ]
    }
    """;

    [Fact]
    public void Generate_creates_a_unit_per_form_and_an_ability_per_ability()
    {
        var doc = MapDocument.Load(Sample());
        var result = HeroGenerateCommand.Execute(doc, Spec);

        Assert.True(result.Ok, result.Message);
        Assert.Equal(2, result.Units.Count);
        Assert.Equal(3, result.Abilities.Count);
        Assert.Equal(2, result.Units.Select(u => u.Rawcode).Distinct().Count());
        Assert.Equal(3, result.Abilities.Select(a => a.Rawcode).Distinct().Count());

        // Read back from the map's own object data, not from the result.
        var units = ObjectKinds.MergedEntries(doc, ObjectKinds.Info(ObjectKind.Unit))
            .Where(u => u.Id != "Hpal".FromRawcode() && u.OldId == "Hpal".FromRawcode())
            .ToList();
        var abilities = ObjectKinds.MergedEntries(doc, ObjectKinds.Info(ObjectKind.Ability))
            .Where(a => a.Id != "ANcl".FromRawcode() && a.OldId == "ANcl".FromRawcode())
            .ToList();
        Assert.Equal(2, units.Count);
        Assert.Equal(3, abilities.Count);
        foreach (var unit in result.Units)
            Assert.Contains(units, u => u.Id.ToRawcode() == unit.Rawcode);
        foreach (var ability in result.Abilities)
            Assert.Contains(abilities, a => a.Id.ToRawcode() == ability.Rawcode);
    }

    [Fact]
    public void Every_generated_channel_ability_gets_a_distinct_base_order_id()
    {
        var doc = MapDocument.Load(Sample());
        var result = HeroGenerateCommand.Execute(doc, Spec);
        Assert.True(result.Ok, result.Message);

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        var firstLevel = result.Abilities
            .Select(a => OrderIdAt(reloaded, a.Rawcode, 1))
            .ToList();

        Assert.Equal(3, firstLevel.Distinct(StringComparer.OrdinalIgnoreCase).Count());

        // Every level of one ability must agree, and every result order id must be what the map
        // actually holds, since a result that disagrees with the file is not evidence.
        foreach (var ability in result.Abilities)
        {
            Assert.Contains(ability.Rawcode, ability.OrderId);
            for (int level = 1; level <= ability.Levels; level++)
                Assert.Equal(ability.OrderId, OrderIdAt(reloaded, ability.Rawcode, level));
        }
    }

    [Fact]
    public void Ability_order_audit_finds_no_collision_among_the_generated_set()
    {
        var doc = MapDocument.Load(Sample());
        var result = HeroGenerateCommand.Execute(doc, Spec);
        Assert.True(result.Ok, result.Message);

        var audit = AbilityOrderAuditCommand.Execute(MapDocument.Load(doc.SaveToBytes()));

        // The base ANcl is itself Channel and contributes its own "channel" order, so the three
        // generated orders make four in total.
        Assert.Equal(4, audit.DistinctOrders);
        Assert.True(audit.ChannelAbilities >= 3);
        Assert.Empty(audit.Collisions);
        Assert.Empty(audit.Problems);
    }

    [Fact]
    public void Each_form_uabi_names_only_its_own_abilities()
    {
        var doc = MapDocument.Load(Sample());
        var result = HeroGenerateCommand.Execute(doc, Spec);
        Assert.True(result.Ok, result.Message);

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        var baseUnit = result.Units.Single(u => u.FormId == "base");
        var bankaiUnit = result.Units.Single(u => u.FormId == "bankai");
        var baseAbilities = result.Abilities.Where(a => a.FormId == "base")
            .Select(a => a.Rawcode).ToList();
        var bankaiAbilities = result.Abilities.Where(a => a.FormId == "bankai")
            .Select(a => a.Rawcode).ToList();

        var baseUabi = Uabi(reloaded, baseUnit.Rawcode);
        var bankaiUabi = Uabi(reloaded, bankaiUnit.Rawcode);

        Assert.Equal(baseAbilities.OrderBy(x => x), baseUabi.OrderBy(x => x));
        Assert.Equal(bankaiAbilities.OrderBy(x => x), bankaiUabi.OrderBy(x => x));
        Assert.DoesNotContain(bankaiAbilities[0], baseUabi);
        Assert.DoesNotContain(baseAbilities[0], bankaiUabi);
    }

    [Fact]
    public void Generated_jass_is_compilable()
    {
        var doc = MapDocument.Load(Sample());
        var result = HeroGenerateCommand.Execute(doc, Spec);
        Assert.True(result.Ok, result.Message);

        var issues = JassScriptCheck.Check(result.Jass);
        Assert.True(JassScriptCheck.IsCompilable(issues),
            string.Join("\n", issues.Select(i => $"{i.Kind} line {i.Line} {i.Message}")));

        // One transform function per form and one damage stub per damaging ability, which is the
        // shape the brief asked for. Two of the three abilities carry damage.
        Assert.Equal(2, Occurrences(result.Jass, "function HeroGen_Transform_"));
        Assert.Equal(2, Occurrences(result.Jass, "\nfunction HeroGen_Damage_"));
        Assert.Contains("HeroGen_Transform_bankai", result.Jass);
        Assert.Contains("HeroGen_Cast_" + result.Abilities.Single(a => a.IsTransform).Rawcode, result.Jass);
    }

    [Fact]
    public void Output_map_reloads_with_the_generated_objects()
    {
        string input = TempPath();
        string output = TempPath();
        try
        {
            File.WriteAllBytes(input, Sample());
            var result = HeroGenerateCommand.Execute(input, output, Spec);
            Assert.True(result.Ok, result.Message);
            Assert.True(File.Exists(output));

            var reloaded = MapDocument.Load(output);
            foreach (var unit in result.Units)
            {
                Assert.NotEqual(0, ObjectKinds.MergedEntries(reloaded, ObjectKinds.Info(ObjectKind.Unit))
                    .Count(u => u.Id.ToRawcode() == unit.Rawcode));
                Assert.NotEmpty(Uabi(reloaded, unit.Rawcode));
            }
            foreach (var ability in result.Abilities)
            {
                Assert.NotEqual(0, ObjectKinds.MergedEntries(reloaded, ObjectKinds.Info(ObjectKind.Ability))
                    .Count(a => a.Id.ToRawcode() == ability.Rawcode));
                Assert.Equal(ability.OrderId, OrderIdAt(reloaded, ability.Rawcode, 1));
            }
        }
        finally
        {
            if (File.Exists(input)) File.Delete(input);
            if (File.Exists(output)) File.Delete(output);
        }
    }

    [Fact]
    public void Unknown_ability_base_is_refused_without_writing_anything()
    {
        const string bad = """
        {
          "name": "Half Written",
          "base_unit": "Hpal",
          "forms": [
            { "id": "base", "abilities": [
              { "slot": "Q", "name": "Fine", "base": "ANcl", "cooldown": [1,1,1,1] },
              { "slot": "W", "name": "Broken", "base": "Azzz", "cooldown": [1] } ] } ]
        }
        """;

        var doc = MapDocument.Load(Sample());
        int abilitiesBefore = ObjectKinds
            .MergedEntries(doc, ObjectKinds.Info(ObjectKind.Ability)).Count;
        int unitsBefore = ObjectKinds
            .MergedEntries(doc, ObjectKinds.Info(ObjectKind.Unit)).Count;

        var result = HeroGenerateCommand.Execute(doc, bad);

        Assert.False(result.Ok);
        Assert.Contains("Azzz", result.Message);
        Assert.Empty(result.Units);
        Assert.Empty(result.Abilities);

        // Not one object may have been created, and the files must not even be marked dirty.
        Assert.Equal(abilitiesBefore,
            ObjectKinds.MergedEntries(doc, ObjectKinds.Info(ObjectKind.Ability)).Count);
        Assert.Equal(unitsBefore,
            ObjectKinds.MergedEntries(doc, ObjectKinds.Info(ObjectKind.Unit)).Count);
        Assert.False(doc.GetFile("war3map.w3a")!.IsDirty);
        Assert.False(doc.GetFile("war3map.w3u")!.IsDirty);
    }

    // A map that already defines both bases the brief derives from, so the generator needs no
    // game install and the synthetic map is the whole world it sees.
    private static byte[] Sample()
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        w3u.BaseUnits.Add(new SimpleObjectModification
        {
            OldId = "Hpal".FromRawcode(),
            NewId = 0,
        });

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        w3a.BaseAbilities.Add(new LevelObjectModification
        {
            OldId = "ANcl".FromRawcode(),
            NewId = 0,
        });

        return SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Serialize(w => w.Write(w3u)),
            ["war3map.w3a"] = Serialize(w => w.Write(w3a)),
            ["war3map.j"] = System.Text.Encoding.UTF8.GetBytes(
                "function main takes nothing returns nothing\nendfunction\n"),
            ["mystery.bin"] = new byte[] { 42, 0, 255, 7 },
        });
    }

    private static Dictionary<string, string> Mods(MapDocument doc, ObjectKind kind, string rawcode) =>
        ObjectKinds.ModsToDict(ObjectKinds.MergedEntries(doc, ObjectKinds.Info(kind))
            .Single(e => e.Id == rawcode.FromRawcode()).Mods);

    private static string OrderIdAt(MapDocument doc, string rawcode, int level) =>
        Mods(doc, ObjectKind.Ability, rawcode)[$"Ncl6:{level}"];

    private static List<string> Uabi(MapDocument doc, string rawcode) =>
        Mods(doc, ObjectKind.Unit, rawcode)["uabi"]
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

    private static int Occurrences(string text, string token)
    {
        int count = 0, index = 0;
        while ((index = text.IndexOf(token, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += token.Length;
        }
        return count;
    }

    private static string TempPath() =>
        Path.Combine(Path.GetTempPath(), $"wc3gen_{Guid.NewGuid():N}.w3x");

    private static byte[] Serialize(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }
}
