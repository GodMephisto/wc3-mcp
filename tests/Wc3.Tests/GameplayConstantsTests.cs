// tests/Wc3.Tests/GameplayConstantsTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

public class GameplayConstantsTests
{
    private static readonly Encoding Enc = Encoding.Latin1;

    private static MapDocument MapWithMisc(string? miscBody)
    {
        var doc = BlankMap.Create();
        if (miscBody is not null)
            doc.AddOrReplaceRawFile(GameplayConstants.MiscFile, Enc.GetBytes(miscBody));
        return doc;
    }

    private static string? MiscText(MapDocument doc)
    {
        var e = doc.GetFile(GameplayConstants.MiscFile);
        return e is null ? null : Enc.GetString(e.OverrideBytes ?? e.RawBytes);
    }

    [Fact]
    public void Target_without_misc_copies_the_source_file_wholesale()
    {
        var source = MapWithMisc("[Misc]\nMaxHeroLevel=35\nMaxUnitLevel=100\nStrHitPointBonus=30\n");
        var target = MapWithMisc(null);

        var summary = GameplayConstants.CarryLevelCaps(source, target);

        Assert.NotNull(summary);
        var text = MiscText(target);
        Assert.NotNull(text);
        Assert.Contains("MaxHeroLevel=35", text);
        // wholesale copy brings the source's other tuning too
        Assert.Contains("StrHitPointBonus=30", text);
    }

    [Fact]
    public void Target_with_lower_cap_is_raised_but_other_tuning_is_untouched()
    {
        var source = MapWithMisc("[Misc]\nMaxHeroLevel=35\nMaxUnitLevel=100\n");
        var target = MapWithMisc("[Misc]\nMaxHeroLevel=10\nMaxUnitLevel=20\nDamageBonusSpells=1.0\n");

        var summary = GameplayConstants.CarryLevelCaps(source, target);

        Assert.NotNull(summary);
        var text = MiscText(target)!;
        Assert.Contains("MaxHeroLevel=35", text);
        Assert.Contains("MaxUnitLevel=100", text);
        // the target's own unrelated tuning survives
        Assert.Contains("DamageBonusSpells=1.0", text);
        Assert.DoesNotContain("MaxHeroLevel=10", text);
    }

    [Fact]
    public void Higher_target_cap_is_never_lowered()
    {
        var source = MapWithMisc("[Misc]\nMaxHeroLevel=35\n");
        var target = MapWithMisc("[Misc]\nMaxHeroLevel=99\n");

        var summary = GameplayConstants.CarryLevelCaps(source, target);

        Assert.Null(summary);
        Assert.Contains("MaxHeroLevel=99", MiscText(target)!);
    }

    [Fact]
    public void Carry_is_idempotent()
    {
        var source = MapWithMisc("[Misc]\nMaxHeroLevel=35\n");
        var target = MapWithMisc("[Misc]\nMaxHeroLevel=10\n");

        var first = GameplayConstants.CarryLevelCaps(source, target);
        var second = GameplayConstants.CarryLevelCaps(source, target);

        Assert.NotNull(first);
        Assert.Null(second); // nothing left to raise
    }

    [Fact]
    public void Missing_key_in_target_is_appended()
    {
        var source = MapWithMisc("[Misc]\nMaxHeroLevel=35\n");
        var target = MapWithMisc("[Misc]\nDamageBonusSpells=1.0\n"); // no MaxHeroLevel at all

        var summary = GameplayConstants.CarryLevelCaps(source, target);

        Assert.NotNull(summary);
        Assert.Contains("MaxHeroLevel=35", MiscText(target)!);
    }

    [Fact]
    public void Source_without_misc_is_a_no_op()
    {
        var source = MapWithMisc(null);
        var target = MapWithMisc("[Misc]\nMaxHeroLevel=10\n");

        var summary = GameplayConstants.CarryLevelCaps(source, target);

        Assert.Null(summary);
        Assert.Contains("MaxHeroLevel=10", MiscText(target)!);
    }
}
