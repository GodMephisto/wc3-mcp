// tests/Wc3.Tests/ContractHeroKitTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// Covers the last two pieces of a target's hero contract that a ported hero used to hit blind.
///
/// The kit ladder was found by instrumenting GGGA and loading it. The installed hero passed every
/// gate the map has (registered in the roster, correct role flag, RPB_IsHeroCommitValid true) and
/// CreateUnit produced her unit, and WS_FinalizeWorkingSourceHero still returned false, because
/// that function gives each hero its spells in a hand-written branch on the hero's rawcode and had
/// none for her. Nothing in the tool said so, so the roster looked like the thing that was wrong.
/// </summary>
public class ContractHeroKitTests
{
    private static MapDocument MapWithScript(string script)
    {
        var doc = BlankMap.Create();
        FileEditCommand.AddOrReplace(doc, "war3map.j", Encoding.Latin1.GetBytes(script));
        return doc;
    }

    private static string KitLadder(params string[] codes)
    {
        var sb = new StringBuilder();
        sb.AppendLine("function FinalizeHeroKit takes player p, unit u, integer heroCode returns boolean");
        for (int i = 0; i < codes.Length; i++)
        {
            sb.AppendLine($"    {(i == 0 ? "if" : "elseif")} heroCode == '{codes[i]}' then");
            sb.AppendLine($"        call TriggerRegisterUnitEvent(gg_trg_{codes[i]}, u, EVENT_UNIT_SPELL_EFFECT)");
            sb.AppendLine("        return true");
        }
        sb.AppendLine("    endif");
        sb.AppendLine("    return false");
        sb.AppendLine("endfunction");
        return sb.ToString();
    }

    private static string Roster(string secondArgument)
    {
        var sb = new StringBuilder();
        sb.AppendLine("function AddHeroToRoster takes integer unitCode, string role returns nothing");
        sb.AppendLine("endfunction");
        sb.AppendLine("function BuildRoster takes nothing returns nothing");
        for (int i = 0; i < 9; i++)
            sb.AppendLine($"    call AddHeroToRoster('H00{i}' , \"{secondArgument}\")");
        sb.AppendLine("endfunction");
        return sb.ToString();
    }

    [Fact]
    public void Finds_the_function_that_gives_each_hero_its_kit()
    {
        var codes = new[] { "H001", "H002", "H003", "H004", "H005", "H006", "H007", "H008", "H009" };
        var doc = MapWithScript(KitLadder(codes));

        var contract = ContractCommand.Run(doc);

        var chain = Assert.Single(contract.HeroDispatchChains);
        Assert.Equal("FinalizeHeroKit", chain.Function);
        Assert.Equal(codes.Length, chain.Branches);
        Assert.Contains("H006", chain.Rawcodes);
    }

    [Fact]
    public void A_handful_of_rawcode_comparisons_is_ordinary_logic_not_a_kit_ladder()
    {
        var doc = MapWithScript(KitLadder("H001", "H002", "H003"));

        Assert.Empty(ContractCommand.Run(doc).HeroDispatchChains);
    }

    [Fact]
    public void A_hero_the_ladder_has_no_branch_for_is_visible_as_absent()
    {
        var codes = new[] { "H001", "H002", "H003", "H004", "H005", "H006", "H007", "H008" };
        var doc = MapWithScript(KitLadder(codes));

        var chain = Assert.Single(ContractCommand.Run(doc).HeroDispatchChains);

        Assert.DoesNotContain("H0ZZ", chain.Rawcodes);
        Assert.Contains("H001", chain.Rawcodes);
    }

    /// <summary>
    /// The contract scan decodes the target's script and install writes parts of it straight back
    /// (the sampled registration call, a roster template's lines). A UTF-8 decode turns every byte
    /// a real map carries that is not valid UTF-8 into U+FFFD, and the Latin-1 write-back then
    /// stores '?', so the byte has to survive the read.
    /// </summary>
    [Fact]
    public void Reads_the_script_as_Latin1_so_a_sampled_line_can_be_written_back_unchanged()
    {
        // 0x92 is a Windows-1252 curly apostrophe and is not valid UTF-8 on its own. Real maps
        // are full of these, in author names and localised strings.
        const char curly = (char)0x92;
        const char damaged = (char)0xFFFD;
        var doc = MapWithScript(Roster("Blackrock" + curly + "s Roar"));

        var registry = Assert.Single(ContractCommand.Run(doc).Registries);

        Assert.Contains(curly, registry.ExampleCall);
        Assert.DoesNotContain(damaged, registry.ExampleCall);
        // The byte survives a write-back, which is the whole point of reading it as Latin-1.
        Assert.Equal(0x92,
            Encoding.Latin1.GetBytes(registry.ExampleCall)[registry.ExampleCall.IndexOf(curly)]);
    }

    [Fact]
    public void Registry_keeps_every_real_call_so_a_caller_can_read_their_other_arguments()
    {
        var sb = new StringBuilder();
        sb.AppendLine("function AddHeroToRoster takes integer unitCode, string role returns nothing");
        sb.AppendLine("endfunction");
        sb.AppendLine("function BuildRoster takes nothing returns nothing");
        for (int i = 0; i < 9; i++)
            sb.AppendLine($"    call AddHeroToRoster('H00{i}' , \"{(i < 5 ? "Stalker" : "Tanker")}\")");
        sb.AppendLine("endfunction");
        var doc = MapWithScript(sb.ToString());

        var registry = Assert.Single(ContractCommand.Run(doc).Registries);

        Assert.Equal(9, registry.Calls.Count);
        Assert.Equal(5, registry.Calls.Count(c => c.Line.Contains("\"Stalker\"", StringComparison.Ordinal)));
        Assert.All(registry.Calls, c => Assert.Contains(c.Rawcode, c.Line, StringComparison.Ordinal));
    }
}
