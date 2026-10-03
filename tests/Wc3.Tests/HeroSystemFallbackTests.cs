// tests/Wc3.Tests/HeroSystemFallbackTests.cs
using Wc3.Commands;

namespace Wc3.Tests;

/// <summary>
/// A per-hero table the installed hero is absent from is not automatically a gap. GGGA's
/// skin-origin table returns 0 for anything unlisted and a new hero is not a skin of anything, so
/// absence there is correct. Its damage-registration table has no default at all, yet most of the
/// map's own units are absent from it too and the reader tolerates that. Both cases used to get
/// the same instruction, "Add one by hand", and following it would have been wrong.
/// </summary>
public class HeroSystemFallbackTests
{
    private static readonly string[] Roster =
        { "H001", "H002", "H004", "H005", "H006", "H007", "H008", "H009", "H00A", "H00B" };

    private static string Ladder(string body) =>
        "function TheTable takes integer heroCode returns integer\n" + body + "endfunction\n";

    private static string BranchesFor(IEnumerable<string> codes) =>
        string.Concat(codes.Select((c, i) =>
            (i == 0 ? "    if " : "    elseif ") + $"heroCode == '{c}' then\n        return {i + 1}\n"));

    private static HeroSystem Hit(string script) =>
        HeroSystemAudit.Run(script, Roster, "H003").Single(s => s.Function == "TheTable");

    [Fact]
    public void A_ladder_that_ends_in_a_default_is_reported_as_having_one()
    {
        var hit = Hit(Ladder(BranchesFor(Roster) + "    endif\n    return 0\n"));

        Assert.Equal(Roster.Length, hit.HeroCount);
        Assert.True(hit.HasFallback,
            "a return reached after the last endif is a default and must be reported as one");
    }

    [Fact]
    public void A_ladder_with_no_default_is_reported_as_having_none()
    {
        var hit = Hit(Ladder(BranchesFor(Roster) + "    endif\n"));

        Assert.False(hit.HasFallback,
            "nothing follows the ladder, so a lookup for an unlisted hero yields nothing");
    }

    [Fact]
    public void An_else_arm_counts_as_the_default()
    {
        Assert.True(Hit(Ladder(BranchesFor(Roster) + "    else\n        return 0\n    endif\n")).HasFallback);
    }

    [Fact]
    public void Coverage_is_reported_against_the_roster_size()
    {
        // How much of the roster a table lists is the evidence for whether an entry is expected.
        // Full coverage is strong evidence. Partial coverage means absence is already the norm.
        var hit = Hit(Ladder(BranchesFor(Roster) + "    endif\n"));

        Assert.Equal(Roster.Length, hit.RosterSize);
        Assert.Equal(100, hit.CoveragePercent);
    }

    [Fact]
    public void Coverage_falls_when_the_table_lists_only_part_of_the_roster()
    {
        var hit = Hit(Ladder(BranchesFor(Roster.Take(5)) + "    endif\n"));

        Assert.Equal(5, hit.HeroCount);
        Assert.Equal(50, hit.CoveragePercent);
    }

    [Fact]
    public void A_table_the_hero_is_already_in_is_not_reported_at_all()
    {
        var script = Ladder(BranchesFor(Roster)
            + "    elseif heroCode == 'H003' then\n        return 99\n    endif\n");

        Assert.DoesNotContain(HeroSystemAudit.Run(script, Roster, "H003"),
            s => s.Function == "TheTable");
    }
}
