// tests/Wc3.Tests/HeroStatConventionTests.cs
using System.Text;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// Pins the part of a target's hero contract that is about NUMBERS rather than wiring.
///
/// This exists because of a hero that passed everything else. Shadow Nanaya was installed into
/// GGGA, registered with the map's own roster call, shown under the right tab, and her unit was
/// verified in game to be creatable. She was still unplayable, because she brought her home map's
/// stat MODEL with her: strength 55, agility 72, intelligence 50 and a 100 point hit-point pool,
/// onto a map where every one of its 167 heroes pins those three attributes at 0 and states a flat
/// pool of a few thousand instead. Nothing reported a problem, because nothing was looking at the
/// numbers.
/// </summary>
public class HeroStatConventionTests
{
    private const string RosterFunction = "AddHeroToRoster";

    /// <summary>
    /// A target shaped like GGGA: heroes registered one per call with a role argument, all of them
    /// stating zero attributes and a flat pool.
    /// </summary>
    private static (MapDocument Doc, RosterRegistry Roster) BuildTarget(
        int stalkers = 10, int tankers = 10, int stalkerHp = 3600, int tankerHp = 4500)
    {
        var doc = BlankMap.Create();
        var script = new StringBuilder();
        script.AppendLine($"function {RosterFunction} takes integer unitCode, string role, "
                          + "string portrait returns nothing");
        script.AppendLine("endfunction");
        script.AppendLine("function BuildRoster takes nothing returns nothing");

        void AddHeroes(int count, string role, int hp)
        {
            for (int i = 0; i < count; i++)
            {
                var made = ObjectNewCommand.Execute(doc, ObjectKind.Unit, "Hpal");
                Assert.True(made.Ok);
                var code = made.NewRawcode!;
                foreach (var (field, value) in new[]
                         {
                             ("ustr", "0"), ("uagi", "0"), ("uint", "0"),
                             ("uhpm", hp.ToString()), ("umpm", "1000"),
                         })
                    Assert.True(ObjectSetCommand.Execute(doc, ObjectKind.Unit, code, field, value).Ok);
                script.AppendLine($"    call {RosterFunction}('{code}' , \"{role}\" , \"p.blp\")");
            }
        }

        AddHeroes(stalkers, "Stalker", stalkerHp);
        AddHeroes(tankers, "Tanker", tankerHp);
        script.AppendLine("endfunction");

        FileEditCommand.AddOrReplace(doc, "war3map.j", Encoding.Latin1.GetBytes(script.ToString()));
        var contract = ContractCommand.Run(doc);
        var roster = Assert.Single(contract.Registries, r => r.Function == RosterFunction);
        return (doc, roster);
    }

    [Fact]
    public void Derives_the_targets_zero_attribute_convention()
    {
        var (doc, roster) = BuildTarget();

        var convention = HeroStatConvention.Derive(doc, roster, role: null);

        Assert.NotNull(convention);
        foreach (var code in new[] { "ustr", "uagi", "uint" })
        {
            var f = Assert.Single(convention!.Fields, x => x.Code == code);
            Assert.Equal("0", f.Value);
            Assert.True(f.Unanimous);
        }
    }

    [Fact]
    public void Role_narrows_the_sample_to_heroes_of_the_same_role()
    {
        // Stalkers and Tankers state different pools, which is exactly why the whole roster is the
        // wrong thing to average. A Stalker must be measured against Stalkers.
        var (doc, roster) = BuildTarget(stalkers: 12, tankers: 12, stalkerHp: 3600, tankerHp: 4500);

        var stalker = HeroStatConvention.Derive(doc, roster, "Stalker");
        var tanker = HeroStatConvention.Derive(doc, roster, "Tanker");

        Assert.Equal("3600", Assert.Single(stalker!.Fields, f => f.Code == "uhpm").Value);
        Assert.Equal("4500", Assert.Single(tanker!.Fields, f => f.Code == "uhpm").Value);
    }

    [Fact]
    public void Falls_back_to_the_whole_roster_when_too_few_heroes_share_the_role()
    {
        // Two Tankers is not a convention. Rather than measure a role from a couple of examples,
        // the whole roster is used and the description says so.
        var (doc, roster) = BuildTarget(stalkers: 14, tankers: 2);

        var convention = HeroStatConvention.Derive(doc, roster, "Tanker");

        Assert.NotNull(convention);
        Assert.DoesNotContain("Tanker", convention!.SampleDescription, StringComparison.Ordinal);
    }

    [Fact]
    public void Refuses_to_invent_a_convention_from_too_few_measurable_heroes()
    {
        // A registry whose rawcodes are mostly not defined by the map (base-game units, or a
        // roster naming things it does not own) has nothing to measure. Answering anyway would
        // hand a hero numbers derived from two examples.
        var doc = BlankMap.Create();
        var script = new StringBuilder();
        script.AppendLine($"function {RosterFunction} takes integer unitCode, string role, "
                          + "string portrait returns nothing");
        script.AppendLine("endfunction");
        script.AppendLine("function BuildRoster takes nothing returns nothing");
        for (int i = 0; i < 9; i++)
            script.AppendLine($"    call {RosterFunction}('Z00{i}' , \"Stalker\" , \"p.blp\")");
        script.AppendLine("endfunction");
        FileEditCommand.AddOrReplace(doc, "war3map.j", Encoding.Latin1.GetBytes(script.ToString()));

        var roster = Assert.Single(ContractCommand.Run(doc).Registries, r => r.Function == RosterFunction);

        Assert.Null(HeroStatConvention.Derive(doc, roster, role: null));
    }

    [Fact]
    public void Ignores_a_field_the_targets_own_heroes_mostly_leave_alone()
    {
        // A field only a couple of heroes state is whatever their base unit came with, not a rule.
        var (doc, roster) = BuildTarget(stalkers: 10, tankers: 10);
        var only = roster.RegisteredRawcodes.Take(2).ToList();
        foreach (var code in only)
            Assert.True(ObjectSetCommand.Execute(doc, ObjectKind.Unit, code, "ulev", "7").Ok);

        var convention = HeroStatConvention.Derive(doc, roster, role: null);

        Assert.DoesNotContain(convention!.Fields, f => f.Code == "ulev");
    }

    [Fact]
    public void Install_brings_a_source_maps_stats_onto_the_targets_model()
    {
        var (doc, roster) = BuildTarget();
        var made = ObjectNewCommand.Execute(doc, ObjectKind.Unit, "Hpal");
        var hero = made.NewRawcode!;
        // The shape Nanaya arrived in: attribute-driven, with the pool her attributes inflated.
        foreach (var (field, value) in new[]
                 { ("ustr", "55"), ("uagi", "72"), ("uint", "50"), ("umpm", "100") })
            Assert.True(ObjectSetCommand.Execute(doc, ObjectKind.Unit, hero, field, value).Ok);

        var convention = HeroStatConvention.Derive(doc, roster, "Stalker")!;
        foreach (var f in convention.Fields)
            Assert.True(ObjectSetCommand.Execute(doc, ObjectKind.Unit, hero, f.Code, f.Value).Ok);

        var fields = Deltas(doc, hero);
        Assert.Equal("0", fields["ustr"]);
        Assert.Equal("0", fields["uagi"]);
        Assert.Equal("0", fields["uint"]);
        Assert.Equal("3600", fields["uhpm"]);
        Assert.Equal("1000", fields["umpm"]);
    }

    /// <summary>The map's own deltas for one object, read without needing a game install.</summary>
    private static Dictionary<string, string> Deltas(MapDocument doc, string rawcode)
    {
        int id = rawcode.FromRawcode();
        var entry = ObjectKinds.MergedEntries(doc, ObjectKinds.Info(ObjectKind.Unit))
            .Single(e => e.Id == id);
        return ObjectKinds.ModsToDict(entry.Mods);
    }

    [Fact]
    public void Contract_reports_the_convention_so_it_can_be_reviewed_before_installing()
    {
        var (doc, _) = BuildTarget();

        var contract = ContractCommand.Run(doc);

        Assert.NotNull(contract.StatConvention);
        Assert.Contains(contract.StatConvention!.Fields, f => f.Code == "ustr" && f.Value == "0");
    }
}
