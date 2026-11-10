// tests/Wc3.Tests/AbilityOrderAuditCommandTests.cs
// The audit exists because Channel dispatches by order string, so two abilities that share
// one can fire together. These facts pin the grouping, the hero scope and the two malformed
// cases that would otherwise pass silently.
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public sealed class AbilityOrderAuditCommandTests
{
    private static byte[] Ser(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }

    private static void Channel(AbilityObjectData w3a, string rawcode, params (int Level, string Order)[] orders)
    {
        var ability = new LevelObjectModification { OldId = "ANcl".FromRawcode(), NewId = rawcode.FromRawcode() };
        ability.Modifications.Add(new LevelObjectDataModification
        {
            Id = "anam".FromRawcode(), Type = ObjectDataType.String, Value = rawcode, Level = 1, Pointer = 0,
        });
        foreach (var (level, order) in orders)
        {
            ability.Modifications.Add(new LevelObjectDataModification
            {
                Id = "Ncl6".FromRawcode(), Type = ObjectDataType.String, Value = order, Level = level, Pointer = 0,
            });
        }
        w3a.NewAbilities.Add(ability);
    }

    private static MapDocument Sample()
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var hero = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        hero.Modifications.Add(new SimpleObjectDataModification
        { Id = "uabi".FromRawcode(), Type = ObjectDataType.String, Value = "A100,A101,A102" });
        w3u.NewUnits.Add(hero);

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        Channel(w3a, "A100", (1, "absorb"));
        Channel(w3a, "A101", (1, "absorb"));
        Channel(w3a, "A102", (1, "thunderclap"));
        Channel(w3a, "A103");                                   // inherits "channel"
        Channel(w3a, "A104", (1, "absorb"), (2, "channel"));    // levels disagree
        Channel(w3a, "A105", (1, ""));                          // empty order

        return MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Ser(w => w.Write(w3u)),
            ["war3map.w3a"] = Ser(w => w.Write(w3a)),
        }));
    }

    [Fact]
    public void A_shared_order_string_is_reported_with_every_member()
    {
        var r = AbilityOrderAuditCommand.Execute(Sample());

        Assert.Equal(6, r.ChannelAbilities);
        var absorb = Assert.Single(r.Collisions.Where(g => g.Order == "absorb"));
        Assert.Equal(new[] { "A100", "A101", "A104" },
            absorb.Members.Select(m => m.Rawcode).OrderBy(x => x).ToArray());
    }

    [Fact]
    public void A_hero_scope_checks_only_the_abilities_that_hero_lists()
    {
        var r = AbilityOrderAuditCommand.Execute(Sample(), "H000");

        Assert.Equal(3, r.ChannelAbilities);
        var absorb = Assert.Single(r.Collisions);
        Assert.Equal("absorb", absorb.Order);
        Assert.Equal(new[] { "A100", "A101" }, absorb.Members.Select(m => m.Rawcode).OrderBy(x => x).ToArray());
    }

    [Fact]
    public void Levels_that_disagree_and_an_empty_order_are_reported_separately()
    {
        var r = AbilityOrderAuditCommand.Execute(Sample());

        Assert.Contains(r.Problems, p => p.Kind == "levels-disagree" && p.Rawcode == "A104");
        Assert.Contains(r.Problems, p => p.Kind == "empty-order" && p.Rawcode == "A105");
    }

    [Fact]
    public void A_hero_whose_abilities_are_all_distinct_has_no_collision()
    {
        var r = AbilityOrderAuditCommand.Execute(Sample(), "H000");

        Assert.DoesNotContain(r.Collisions, g => g.Members.Any(m => m.Rawcode == "A102"));
    }

    [Fact]
    public void A_hero_missing_from_the_map_data_is_reported_rather_than_guessed()
    {
        var r = AbilityOrderAuditCommand.Execute(Sample(), "H999");

        Assert.Empty(r.Collisions);
        Assert.Contains("not in the map", string.Join(" ", r.Diagnostics));
    }
}
