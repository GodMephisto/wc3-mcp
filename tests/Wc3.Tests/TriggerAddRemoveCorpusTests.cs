// tests/Wc3.Tests/TriggerAddRemoveCorpusTests.cs
using War3Net.Build.Script;
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// The add and remove guards on a REAL map, because the synthetic ones use trees this code wrote
/// itself. A map from the library brings the things a fixture cannot fake: a populated item-count
/// header the writer emits, ids namespaced by item type with duplicates across types, and a
/// hundred-odd custom-text bodies whose pairing must still hold afterwards.
/// </summary>
public class TriggerAddRemoveCorpusTests
{
    private readonly ITestOutputHelper _out;
    public TriggerAddRemoveCorpusTests(ITestOutputHelper output) => _out = output;

    private static string Path_(string name) => System.IO.Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "Warcraft III", "Maps", "Download", name);

    // A sub-version map with GUI triggers AND many custom-text bodies, which is the combination
    // that broke the old pairing rule.
    private const string SubVersionMap = "Anime_WOS2_0.30d.w3x";

    [Fact]
    [Trait("Category", "Corpus")]
    public void Adding_a_category_to_a_real_map_leaves_every_code_body_where_it_was()
    {
        string path = Path_(SubVersionMap);
        if (!File.Exists(path)) { _out.WriteLine("map absent, skipped"); return; }

        var doc = MapDocument.Load(path);
        var before = TriggerReadCommand.GetTriggers(doc);
        var bodiesBefore = before.Triggers
            .Where(t => t.IsCustomText)
            .ToDictionary(t => t.Name + "#" + t.Id, t => t.CustomText ?? string.Empty);
        _out.WriteLine($"before: {before.Categories.Count} categor(ies), "
                     + $"{before.Triggers.Count} trigger(s), {bodiesBefore.Count} custom-text");

        var r = TriggerCommand.AddCategory(doc, "wc3ctl added category");
        Assert.True(r.Ok, r.Message);
        _out.WriteLine(r.Message);

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        var after = TriggerReadCommand.GetTriggers(reloaded);
        _out.WriteLine($"after:  {after.Categories.Count} categor(ies), "
                     + $"{after.Triggers.Count} trigger(s)");

        Assert.Equal(before.Categories.Count + 1, after.Categories.Count);
        Assert.Equal(before.Triggers.Count, after.Triggers.Count);
        Assert.Contains(after.Categories, c => c.Name == "wc3ctl added category");

        int moved = 0;
        foreach (var t in after.Triggers.Where(t => t.IsCustomText))
        {
            if (!bodiesBefore.TryGetValue(t.Name + "#" + t.Id, out string? was)) continue;
            if ((t.CustomText ?? string.Empty) != was) moved++;
        }
        _out.WriteLine($"custom-text bodies that changed: {moved}");
        Assert.Equal(0, moved);
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Adding_a_category_to_a_real_map_touches_only_the_wtg()
    {
        string path = Path_(SubVersionMap);
        if (!File.Exists(path)) { _out.WriteLine("map absent, skipped"); return; }

        var original = MapDocument.Load(path);
        var doc = MapDocument.Load(path);
        Assert.True(TriggerCommand.AddCategory(doc, "wc3ctl added category").Ok);
        var saved = MapDocument.Load(doc.SaveToBytes());

        var changed = new List<string>();
        foreach (var entry in original.Files)
        {
            string name = entry.FileName ?? string.Empty;
            if (name.Length == 0 || name.StartsWith("(")) continue;   // MPQ bookkeeping
            var other = saved.GetFile(name);
            if (other is null) { changed.Add(name + " (missing)"); continue; }
            if (!entry.CurrentBytes.AsSpan().SequenceEqual(other.CurrentBytes))
                changed.Add(name);
        }

        _out.WriteLine("changed: " + (changed.Count == 0 ? "(nothing)" : string.Join(", ", changed)));
        Assert.Equal(new[] { "war3map.wtg" }, changed.ToArray());
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void The_added_category_id_stays_in_the_category_namespace()
    {
        string path = Path_(SubVersionMap);
        if (!File.Exists(path)) { _out.WriteLine("map absent, skipped"); return; }

        var doc = MapDocument.Load(path);
        var wtg = (MapTriggers)doc.GetFile("war3map.wtg")!.Model!;
        int maxCategory = wtg.TriggerItems
            .Where(i => i.Type == TriggerItemType.Category).Max(i => i.Id);
        int maxAny = wtg.TriggerItems.Max(i => i.Id);
        _out.WriteLine($"max category id 0x{maxCategory:X}, max id of any type 0x{maxAny:X}");
        Assert.True(maxAny > maxCategory,
            "this map is supposed to have a higher id in another type's namespace, "
            + "otherwise the test proves nothing");

        Assert.True(TriggerCommand.AddCategory(doc, "wc3ctl added category").Ok);
        var added = MapDocument.Load(doc.SaveToBytes())
            .GetFile("war3map.wtg") is { Model: MapTriggers t }
            ? t.TriggerItems.First(i => i.Name == "wc3ctl added category")
            : throw new InvalidOperationException("wtg vanished");

        _out.WriteLine($"added id 0x{added.Id:X}");
        Assert.Equal(maxCategory + 1, added.Id);
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void A_real_map_full_of_code_refuses_a_removal_that_would_shift_bodies()
    {
        string path = Path_(SubVersionMap);
        if (!File.Exists(path)) { _out.WriteLine("map absent, skipped"); return; }

        var doc = MapDocument.Load(path);
        var wtg = (MapTriggers)doc.GetFile("war3map.wtg")!.Model!;
        var slots = WctPairing.SlotIndices(wtg);

        // The first custom-text trigger with a unique id and a body: removing it must be refused,
        // because a hundred bodies sit behind it.
        var victim = wtg.TriggerItems.OfType<TriggerDefinition>()
            .Where(d => slots[d] == 0)
            .FirstOrDefault(d => wtg.TriggerItems.Count(i => i.Id == d.Id) == 1);
        if (victim is null) { _out.WriteLine("no uniquely addressable slot-0 trigger, skipped"); return; }

        var r = TriggerCommand.Remove(doc, victim.Id);
        _out.WriteLine($"remove '{victim.Name}' (id 0x{victim.Id:X}) -> Ok={r.Ok}: {r.Message}");
        Assert.False(r.Ok);
        Assert.Contains("without moving code", r.Message);
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void A_gui_trigger_on_a_real_sub_version_map_can_be_removed()
    {
        string path = Path_(SubVersionMap);
        if (!File.Exists(path)) { _out.WriteLine("map absent, skipped"); return; }

        var doc = MapDocument.Load(path);
        var wtg = (MapTriggers)doc.GetFile("war3map.wtg")!.Model!;
        var slots = WctPairing.SlotIndices(wtg);

        // A GUI trigger holds no slot here, so it cannot move anything. It must also be childless
        // and uniquely addressable.
        var gui = wtg.TriggerItems.OfType<TriggerDefinition>()
            .Where(d => slots[d] == -1)
            .FirstOrDefault(d => wtg.TriggerItems.Count(i => i.Id == d.Id) == 1
                              && !wtg.TriggerItems.Any(i => i.ParentId == d.Id));
        if (gui is null) { _out.WriteLine("no removable GUI trigger on this map, skipped"); return; }

        var bodiesBefore = TriggerReadCommand.GetTriggers(doc).Triggers
            .Where(t => t.IsCustomText)
            .ToDictionary(t => t.Name + "#" + t.Id, t => t.CustomText ?? string.Empty);

        var r = TriggerCommand.Remove(doc, gui.Id);
        _out.WriteLine($"remove GUI trigger '{gui.Name}' -> Ok={r.Ok}: {r.Message}");
        Assert.True(r.Ok, r.Message);

        var after = TriggerReadCommand.GetTriggers(MapDocument.Load(doc.SaveToBytes()));
        Assert.DoesNotContain(after.Triggers, t => t.Id == gui.Id && t.Name == gui.Name);

        int moved = after.Triggers.Where(t => t.IsCustomText)
            .Count(t => bodiesBefore.TryGetValue(t.Name + "#" + t.Id, out string? was)
                     && (t.CustomText ?? string.Empty) != was);
        _out.WriteLine($"custom-text bodies that changed: {moved}");
        Assert.Equal(0, moved);
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void The_real_maps_item_count_header_stays_true_after_an_add()
    {
        string path = Path_(SubVersionMap);
        if (!File.Exists(path)) { _out.WriteLine("map absent, skipped"); return; }

        var doc = MapDocument.Load(path);
        Assert.True(TriggerCommand.AddCategory(doc, "wc3ctl added category").Ok);

        var wtg = (MapTriggers)MapDocument.Load(doc.SaveToBytes()).GetFile("war3map.wtg")!.Model!;
        foreach (var kv in wtg.TriggerItemCounts)
        {
            int actual = wtg.TriggerItems.Count(i => i.Type == kv.Key);
            _out.WriteLine($"  {kv.Key,-14} header={kv.Value,-5} actual={actual}");
            Assert.Equal(actual, kv.Value);
        }
    }
}
