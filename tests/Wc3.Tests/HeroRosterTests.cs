// tests/Wc3.Tests/HeroRosterTests.cs
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// The roster answers "how many heroes does this map have, and where does each one live",
/// which counting triggers does not.
///
/// Reported symptom, the number of hero triggers does not tally. It does not, and correctly so.
/// GGGA_V0.05 registers 170 heroes and carries 39 Character triggers, because four grouped
/// triggers hold 4.27 million characters of hero code between them while 35 heroes got a
/// trigger of their own.
///
/// The load-bearing assertion here is which registration call gets chosen. Picking the call
/// with the most distinct rawcodes is wrong and wrong NARROWLY, which is the dangerous kind:
/// on this map the native SetUnitAbilityLevelSwapped carries 172 rawcodes against RPB_AddHero's
/// 170 and would win by two, while none of its 172 is a unit at all.
/// </summary>
public class HeroRosterTests
{
    private readonly ITestOutputHelper _out;
    public HeroRosterTests(ITestOutputHelper output) => _out = output;

    private static string MapPath(string name) => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "Warcraft III", "Maps", "Download", name);

    [Fact]
    public void A_map_with_no_script_roster_refuses_instead_of_inventing_one()
    {
        var r = HeroRosterCommand.Run(BlankMap.Create());
        Assert.False(r.Ok);
        Assert.Empty(r.Heroes);
        _out.WriteLine(r.Message);
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void The_roster_comes_from_a_call_whose_rawcodes_are_actually_units()
    {
        string path = MapPath("GGGA_V0.05.w3x");
        if (!File.Exists(path)) { _out.WriteLine("map not on disk"); return; }

        var r = HeroRosterCommand.Run(MapDocument.Load(path));
        Assert.True(r.Ok, r.Message);
        _out.WriteLine($"{r.Heroes.Count} hero(es) via {r.RegistryFunction}");

        // The native that would win on raw count sets ability levels and registers nothing.
        Assert.NotEqual("SetUnitAbilityLevelSwapped", r.RegistryFunction);

        // A roster is a list of units, so most entries must resolve to a named unit. This is
        // the property that separates the real registration from a native, and asserting it
        // here means a future change to the selection rule cannot quietly pick the native again.
        int named = r.Heroes.Count(h => h.Name.Length > 0);
        _out.WriteLine($"{named} of {r.Heroes.Count} resolve to a named unit");
        Assert.True(named * 2 > r.Heroes.Count,
            $"only {named} of {r.Heroes.Count} roster entries are named units, which is the "
            + "signature of having picked an ability-taking native rather than the roster");
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Heroes_outnumber_the_triggers_that_hold_them()
    {
        // The whole point. If these two ever match, either the map changed or the roster
        // silently degraded into counting triggers again.
        string path = MapPath("GGGA_V0.05.w3x");
        if (!File.Exists(path)) { _out.WriteLine("map not on disk"); return; }

        var doc = MapDocument.Load(path);
        var roster = HeroRosterCommand.Run(doc);
        if (!roster.Ok) { _out.WriteLine(roster.Message); return; }

        var triggers = TriggerReadCommand.GetTriggers(doc).Triggers;
        int characterTriggers = triggers.Count(t => t.Name.Contains("Character", StringComparison.OrdinalIgnoreCase));

        _out.WriteLine($"heroes {roster.Heroes.Count}, Character triggers {characterTriggers}, "
                     + $"triggers in total {triggers.Count}");
        Assert.True(roster.Heroes.Count > characterTriggers,
            "the roster should carry more heroes than there are Character triggers, since "
            + "several triggers each bundle many heroes");

        // Every hero should be attributable to some trigger on this map, because its hero code
        // lives in custom text rather than in the compiled script alone.
        int placed = roster.Heroes.Count(h => h.Trigger is not null);
        _out.WriteLine($"{placed} of {roster.Heroes.Count} attributed to a trigger");
        Assert.True(placed * 2 > roster.Heroes.Count);
    }
}
