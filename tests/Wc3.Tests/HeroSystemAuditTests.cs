// tests/Wc3.Tests/HeroSystemAuditTests.cs
using System.Text;
using Wc3.Commands;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// The kit ladder is one per-hero table out of several, and naming only it understated the work.
/// Measured in the running game on GGGA: the pick-to-spawn path is not broken for a ported hero.
/// The commit is accepted, the seat finalizes, CreateUnit produces her unit, and she arrives alive,
/// visible, owned and selected, exactly like a native hero. What she lacks is membership in the
/// other hand-written tables (damage-event routing, two death handlers, generated map state, the
/// carried port's own roster mirror). Half of those are not comparison ladders, so the ladder
/// detector in ContractCommand could never see them.
/// </summary>
public class HeroSystemAuditTests
{
    /// <summary>A roster big enough that a fifth of it is a meaningful share.</summary>
    private static readonly string[] Natives =
        Enumerable.Range(1, 30).Select(i => "H" + i.ToString("D3")).ToArray();

    private static string Table(string name, string body, params string[] codes)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"function {name} takes nothing returns nothing");
        foreach (var c in codes) sb.AppendLine("    " + string.Format(body, c));
        sb.AppendLine("endfunction");
        return sb.ToString();
    }

    private static IReadOnlyList<string> Roster() => Natives.Append("H0ZZ").ToList();

    [Fact]
    public void NamesTablesTheNewcomerIsMissingFrom()
    {
        // Neither shape is a "== 'XXXX'" ladder, which is the point.
        var script = Table("RouteDamage", "call SaveTriggerHandle(H, 0, ( '{0}' ), ( gg_trg_x ))", Natives)
                     + Table("DeathVoice", "if IsHeroType(GetDyingUnit(), '{0}') then", Natives);

        var missing = HeroSystemAudit.Run(script, Roster(), "H0ZZ");

        Assert.Contains(missing, s => s.Function == "RouteDamage");
        Assert.Contains(missing, s => s.Function == "DeathVoice");
        Assert.All(missing, s => Assert.Equal(30, s.HeroCount));
    }

    [Fact]
    public void SaysNothingAboutATableTheNewcomerIsAlreadyIn()
    {
        var script = Table("RouteDamage", "call SaveTriggerHandle(H, 0, ( '{0}' ), ( gg_trg_x ))",
            Natives.Append("H0ZZ").ToArray());

        Assert.Empty(HeroSystemAudit.Run(script, Roster(), "H0ZZ"));
    }

    /// <summary>
    /// Membership alone matched 3371 functions on GGGA, because one hero's own damage trigger
    /// legitimately tests seventeen others. Scale relative to the roster is what cut that to seven,
    /// so a function naming a handful of heroes must stay out of the report.
    /// </summary>
    [Fact]
    public void IgnoresGameplayLogicThatMerelyMentionsAFewHeroes()
    {
        var registry = Table("Registry", "call Register('{0}')", Natives);
        var logic = Table("OneHeroCounterplay", "if GetUnitTypeId(u) == '{0}' then",
            Natives.Take(5).ToArray());

        var missing = HeroSystemAudit.Run(registry + logic, Roster(), "H0ZZ");

        Assert.Contains(missing, s => s.Function == "Registry");
        Assert.DoesNotContain(missing, s => s.Function == "OneHeroCounterplay");
    }

    [Fact]
    public void IgnoresRawcodesThatAreNotOnTheRoster()
    {
        var items = Table("ItemRelatives", "call SetItemRelative('{0}')",
            Enumerable.Range(1, 20).Select(i => "I" + i.ToString("D3")).ToArray());

        Assert.Empty(HeroSystemAudit.Run(items, Roster(), "H0ZZ"));
    }

    [Fact]
    public void ReportsTheBiggestTablesFirstAndCapsTheList()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < 20; i++)
            sb.Append(Table("T" + i, "call Register('{0}')", Natives.Take(10 + i).ToArray()));

        var missing = HeroSystemAudit.Run(sb.ToString(), Roster(), "H0ZZ");

        Assert.Equal(12, missing.Count);
        Assert.Equal(missing.OrderByDescending(s => s.HeroCount).Select(s => s.Function),
            missing.Select(s => s.Function));
        Assert.Equal(29, missing[0].HeroCount);
    }

    /// <summary>
    /// A generated map comments retired lines out rather than deleting them, and a porter writes
    /// provenance comments naming the rawcodes it carried. Counting those charged a hero to a table
    /// whose live code never mentions her.
    /// </summary>
    [Fact]
    public void IgnoresRawcodesThatAppearOnlyInComments()
    {
        var script = "function Registry takes nothing returns nothing\n"
                     + string.Concat(Natives.Select(c => $"    // retired: call Register('{c}')\n"))
                     + "endfunction\n";

        Assert.Empty(HeroSystemAudit.Run(script, Roster(), "H0ZZ"));
    }

    [Fact]
    public void AMapWithNoRosterIsNotAudited()
    {
        var script = Table("RouteDamage", "call Register('{0}')", Natives);

        Assert.Empty(HeroSystemAudit.Run(script, new[] { "H001", "H0ZZ" }, "H0ZZ"));
    }

    /// <summary>The reported line opens the function's own declaration, not a line inside it.</summary>
    [Fact]
    public void ReportsTheDeclarationLine()
    {
        var script = "function Unrelated takes nothing returns nothing\r\nendfunction\r\n"
                     + Table("Registry", "call Register('{0}')", Natives).Replace("\n", "\r\n");

        var missing = HeroSystemAudit.Run(script, Roster(), "H0ZZ");

        var line = Assert.Single(missing).Line;
        Assert.Equal("function Registry takes nothing returns nothing",
            script.Split('\n')[line - 1].TrimEnd('\r'));
    }
}
