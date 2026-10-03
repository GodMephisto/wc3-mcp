// tests/Wc3.Tests/WctPairingTests.cs
using War3Net.Build.Script;
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// Guards the war3map.wct to war3map.wtg pairing rule, both branches, on synthetic trees that make
/// the answer unambiguous. The measurement that produced the rule lives in WctPairingRuleSweep and
/// needs the map library; these run anywhere and fail loudly if the branch is ever flattened back
/// into a single unconditional ordinal.
/// </summary>
public class WctPairingTests
{
    private static TriggerDefinition Gui(string name) =>
        new() { Name = name, Description = string.Empty, IsEnabled = true };

    private static TriggerDefinition Text(string name) =>
        new(TriggerItemType.Script)
        { Name = name, Description = string.Empty, IsEnabled = true, IsCustomTextTrigger = true };

    private static MapTriggers Tree(bool subVersion, params TriggerItem[] items)
    {
        var t = new MapTriggers(
            MapTriggersFormatVersion.v7,
            subVersion ? MapTriggersSubVersion.v4 : null);
        t.TriggerItems.AddRange(items);
        return t;
    }

    private static MapCustomTextTriggers Bodies(params string[] code) => Bodies(null, code);

    private static MapCustomTextTriggers Bodies(MapCustomTextTriggersSubVersion? sub, params string[] code)
    {
        var wct = new MapCustomTextTriggers(MapCustomTextTriggersFormatVersion.v1, sub);
        foreach (var c in code)
            wct.CustomTextTriggers.Add(new CustomTextTrigger { Code = c });
        return wct;
    }

    [Fact]
    public void Without_a_sub_version_a_gui_trigger_holds_an_empty_slot_of_its_own()
    {
        // Measured on RATankD_516.2: 606 slots for 606 definitions, 42 empty for 42 GUI triggers.
        var a = Text("first");
        var g = Gui("a gui trigger");
        var b = Text("second");
        var wtg = Tree(subVersion: false, a, g, b);
        var wct = Bodies("// first", string.Empty, "// second");

        Assert.Equal(3, WctPairing.ExpectedSlotCount(wtg));
        Assert.Equal("// first", WctPairing.BodyFor(wtg, wct, a));
        Assert.Equal("// second", WctPairing.BodyFor(wtg, wct, b));
        // The GUI trigger owns slot 1, and still reports no body, because it is not custom text.
        Assert.Null(WctPairing.BodyFor(wtg, wct, g));
        Assert.Equal(1, WctPairing.SlotIndices(wtg)[g]);
    }

    [Fact]
    public void With_a_sub_version_a_gui_trigger_holds_no_slot_at_all()
    {
        // Measured on Anime_WOS2_0.30d: 109 slots for 115 definitions, 6 GUI, zero empty. This is
        // the case the reader used to get wrong, and it got it wrong in the most misleading
        // possible way, by showing 'second' the body belonging to 'first'.
        var a = Text("first");
        var g = Gui("a gui trigger");
        var b = Text("second");
        var wtg = Tree(subVersion: true, a, g, b);
        var wct = Bodies("// first", "// second");

        Assert.Equal(2, WctPairing.ExpectedSlotCount(wtg));
        Assert.Equal("// first", WctPairing.BodyFor(wtg, wct, a));
        Assert.Equal("// second", WctPairing.BodyFor(wtg, wct, b));
        Assert.Null(WctPairing.BodyFor(wtg, wct, g));
        Assert.Equal(-1, WctPairing.SlotIndices(wtg)[g]);
    }

    [Fact]
    public void The_old_unconditional_rule_would_have_mispaired_and_this_says_so_out_loud()
    {
        var a = Text("first");
        var g = Gui("gui");
        var b = Text("second");
        var wtg = Tree(subVersion: true, a, g, b);
        var wct = Bodies("// first", "// second");

        // Ordinal over ALL definitions puts 'second' at index 2, which does not exist, so the old
        // reader showed it as having NO script at all. With one more custom-text trigger after it,
        // the same arithmetic shows it someone else's script instead.
        int oldOrdinalForB = wtg.TriggerItems.OfType<TriggerDefinition>().ToList().IndexOf(b);
        Assert.Equal(2, oldOrdinalForB);
        Assert.Equal(1, WctPairing.SlotIndices(wtg)[b]);
        Assert.NotEqual(oldOrdinalForB, WctPairing.SlotIndices(wtg)[b]);
    }

    [Fact]
    public void Definitions_sharing_an_id_still_get_distinct_slots()
    {
        // Sub-version maps namespace ids by item type, so duplicate ids across the tree are normal
        // (24 of them on Anime_WOS2_0.30d). A pairing keyed by id rather than identity would
        // collapse these two onto one slot.
        var a = Text("same id A");
        var b = Text("same id B");
        a.Id = 42; b.Id = 42;
        var wtg = Tree(subVersion: true, a, b);
        var wct = Bodies("// A", "// B");

        Assert.Equal("// A", WctPairing.BodyFor(wtg, wct, a));
        Assert.Equal("// B", WctPairing.BodyFor(wtg, wct, b));
    }

    [Fact]
    public void A_short_slot_list_yields_null_rather_than_throwing()
    {
        var a = Text("first");
        var b = Text("second");
        var wtg = Tree(subVersion: true, a, b);
        var wct = Bodies("// only one");

        Assert.Equal("// only one", WctPairing.BodyFor(wtg, wct, a));
        Assert.Null(WctPairing.BodyFor(wtg, wct, b));
    }

    [Fact]
    public void Reading_a_real_map_pairs_bodies_that_name_their_own_trigger()
    {
        // The end-to-end version of the same claim, through the real reader. A custom-text body is
        // compiled as Trig_<name>_Actions or written by hand with the name in a header comment, so
        // a correct pairing shows most bodies naming their own trigger and a wrong one does not.
        string path = CorpusMap.PathOrEmpty;
        if (!File.Exists(path)) return;

        var doc = MapDocument.Load(path);
        var model = TriggerReadCommand.GetTriggers(doc);
        var withText = model.Triggers
            .Where(t => t.IsCustomText && !string.IsNullOrWhiteSpace(t.CustomText))
            .ToList();
        if (withText.Count < 8) return;   // too few to be a meaningful ratio

        int named = withText.Count(t => Mentions(t.CustomText!, t.Name));
        Assert.True(named * 2 >= withText.Count,
            $"only {named} of {withText.Count} custom-text bodies name their own trigger, which "
            + "is the signature of a mispaired wct");
    }

    private static bool Mentions(string code, string name)
    {
        string needle = new(name.Where(char.IsLetterOrDigit).ToArray());
        if (needle.Length < 4) return true;   // too short to judge, do not count against
        string hay = new(code.Where(char.IsLetterOrDigit).ToArray());
        return hay.Contains(needle, StringComparison.OrdinalIgnoreCase);
    }
}
