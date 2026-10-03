// tests/Wc3.Tests/ScriptEffectAssetCarryTests.cs
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// A custom skill's visual effect is almost always a string literal inside its handler rather
/// than a value on an ability field, so carrying a hero without scanning the shipped script
/// leaves her spells firing with nothing visible. These pin the two halves of that scan, the
/// literal shape and the roster-wide bank guard, which is the half a first attempt omitted and
/// which is why that attempt over-carried by 134 files.
/// </summary>
public class ScriptEffectAssetCarryTests
{
    private const string Effect = @"war3mapImported\Gear_Clock_2.mdx";

    [Fact]
    public void An_effect_path_in_a_handler_is_found_and_unescaped()
    {
        // A JASS source writes one separator as two backslash characters. A caller that skips
        // the un-escaping looks up a name the archive does not hold and calls the file missing
        // while it is sitting right there.
        var body = "function CastIt takes nothing returns nothing\n"
                 + "    call AddSpecialEffect(\"war3mapImported\\\\Gear_Clock_2.mdx\", 0., 0.)\n"
                 + "endfunction\n";

        Assert.Contains(Effect, AssetPathCandidates.NamedInScript(body));
    }

    [Fact]
    public void A_path_is_reported_once_however_often_it_is_cast()
    {
        var body = "call AddSpecialEffect(\"war3mapImported\\\\Same.mdx\", 0., 0.)\n"
                 + "call AddSpecialEffect(\"war3mapImported\\\\Same.mdx\", 1., 1.)\n";

        Assert.Single(AssetPathCandidates.NamedInScript(body));
    }

    [Fact]
    public void Prose_and_short_literals_are_not_treated_as_paths()
    {
        var body = "call BJDebugMsg(\"Shadow Nanaya has entered the arena\")\n"
                 + "call MakeSound(\"Q\")\n";

        Assert.Empty(AssetPathCandidates.NamedInScript(body));
    }

    [Fact]
    public void A_handler_naming_a_few_effects_stays_under_the_bank_guard()
    {
        // Her passive named 7 in one body and 3 in another. Both must be carried, so the guard
        // has to sit well above a legitimately effect-heavy spell.
        var body = string.Join("\n", Enumerable.Range(0, 7).Select(i =>
            $"    call AddSpecialEffect(\"war3mapImported\\\\Fx_{i}.mdx\", 0., 0.)"));

        var named = AssetPathCandidates.NamedInScript(body).ToList();
        Assert.Equal(7, named.Count);
        Assert.True(named.Count <= BundleCommand.SharedAssetBankCount,
            "a seven-effect spell handler must not read as a roster-wide asset bank");
    }

    [Fact]
    public void A_roster_wide_bank_is_over_the_guard_and_must_be_skipped()
    {
        // The real one named 171 paths in a single body. Attributing those to whichever hero is
        // being exported is what imports the whole roster's art.
        var body = string.Join("\n", Enumerable.Range(0, 171).Select(i =>
            $"    if s == \"Hero{i}\" then\n        call MakeSound(\"war3mapImported\\\\Voice_{i}.mp3\")"));

        var named = AssetPathCandidates.NamedInScript(body).ToList();
        Assert.Equal(171, named.Count);
        Assert.True(named.Count > BundleCommand.SharedAssetBankCount,
            "a body naming 171 paths must read as a bank and be skipped");
    }

    [Fact]
    public void The_guard_threshold_has_one_definition()
    {
        // Two copies of this number would let the bundle and the export disagree about what a
        // bank is, and the disagreement would be silent.
        Assert.Equal(24, BundleCommand.SharedAssetBankCount);
    }

    [Fact]
    public void A_literal_naming_mdl_resolves_to_the_mdx_actually_stored()
    {
        // Her first clock model is named .mdl in the script and stored as .mdx. Resolution has to
        // try the family or that one file is silently dropped while the other four arrive.
        var candidates = AssetPathCandidates.Expand(@"war3mapImported\Gear_Clock_1.mdl").ToList();
        Assert.Contains(@"war3mapImported\Gear_Clock_1.mdx", candidates);
    }
}
