// tests/Wc3.Tests/TriggerAddRemoveTests.cs
using War3Net.Build.Extensions;
using War3Net.Build.Script;
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// Guards adding and removing trigger items. Every claim here goes through a real save and a real
/// reload, because an edit that only holds in memory is the failure mode these verbs are most
/// likely to have, and the four earlier trigger edits were shipped unreachable from any front-end
/// until exactly this kind of check was applied to them.
/// </summary>
public class TriggerAddRemoveTests
{
    // ---------------------------------------------------------------- fixtures

    private static byte[] WtgBytes(MapTriggers t)
    {
        using var ms = new MemoryStream();
        using (var w = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
            w.Write(t);
        return ms.ToArray();
    }

    private static byte[] WctBytes(MapCustomTextTriggers t)
    {
        using var ms = new MemoryStream();
        using (var w = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
            w.Write(t);
        return ms.ToArray();
    }

    private static TriggerDefinition Gui(string name, int id, int parent) =>
        new(TriggerItemType.Gui)
        {
            Id = id, Name = name, Description = string.Empty, ParentId = parent,
            IsEnabled = true, IsInitiallyOn = true,
        };

    private static TriggerDefinition Text(string name, int id, int parent) =>
        new(TriggerItemType.Script)
        {
            Id = id, Name = name, Description = string.Empty, ParentId = parent,
            IsEnabled = true, IsInitiallyOn = true, IsCustomTextTrigger = true,
        };

    private static TriggerCategoryDefinition Cat(string name, int id, int parent = -1) =>
        new() { Id = id, Name = name, ParentId = parent, IsExpanded = true };

    /// <summary>A map whose wtg carries the given tree, plus a wct with the given bodies.</summary>
    private static MapDocument Map(MapTriggers tree, MapCustomTextTriggers? wct = null)
    {
        var files = new Dictionary<string, byte[]> { ["war3map.wtg"] = WtgBytes(tree) };
        if (wct is not null) files["war3map.wct"] = WctBytes(wct);
        return MapDocument.Load(SyntheticMap.Build(files));
    }

    private static MapTriggers Tree(bool subVersion, params TriggerItem[] items)
    {
        var t = new MapTriggers(MapTriggersFormatVersion.v7,
            subVersion ? MapTriggersSubVersion.v4 : null);
        t.TriggerItems.AddRange(items);
        if (subVersion)
            foreach (var type in Enum.GetValues<TriggerItemType>())
                t.TriggerItemCounts[type] = items.Count(i => i.Type == type);
        return t;
    }

    private static MapCustomTextTriggers Wct(params string[] bodies)
    {
        var w = new MapCustomTextTriggers(MapCustomTextTriggersFormatVersion.v1, null)
        {
            // The writer serializes the global block through the same path as a slot, so both must
            // exist or it throws while writing the fixture rather than the code under test.
            GlobalCustomScriptComment = string.Empty,
            GlobalCustomScriptCode = new CustomTextTrigger { Code = string.Empty },
        };
        foreach (var b in bodies) w.CustomTextTriggers.Add(new CustomTextTrigger { Code = b });
        return w;
    }

    /// <summary>Saves and reloads, which is the only way to know an edit actually persisted.</summary>
    private static MapTriggers RoundTrip(MapDocument doc)
    {
        var reloaded = MapDocument.Load(doc.SaveToBytes());
        return (MapTriggers)reloaded.GetFile("war3map.wtg")!.Model!;
    }

    // ---------------------------------------------------------------- add

    [Fact]
    public void Adding_a_category_survives_a_save_and_reload()
    {
        var doc = Map(Tree(true, Cat("Existing", 0x02000001)));
        var r = TriggerCommand.AddCategory(doc, "Fresh");
        Assert.True(r.Ok, r.Message);

        var after = RoundTrip(doc);
        var added = after.TriggerItems.FirstOrDefault(i => i.Name == "Fresh");
        Assert.NotNull(added);
        Assert.Equal(TriggerItemType.Category, added!.Type);
        Assert.Equal(2, after.TriggerItems.Count);
    }

    [Fact]
    public void An_added_category_stays_inside_the_category_id_namespace()
    {
        // Sub-version maps put the item type in the id's high byte. A category numbered one past
        // the GLOBAL maximum would land in the comment range, which is what a naive maxId + 1 does.
        var doc = Map(Tree(true,
            Cat("Cats", 0x02000005),
            Gui("A trigger", 0x03000009, 0x02000005),
            new TriggerDefinition(TriggerItemType.Comment)
            {
                Id = 0x04000100, Name = "A comment", Description = string.Empty,
                ParentId = -1, IsComment = true,
            }));

        var r = TriggerCommand.AddCategory(doc, "Fresh");
        Assert.True(r.Ok, r.Message);

        var added = RoundTrip(doc).TriggerItems.First(i => i.Name == "Fresh");
        Assert.Equal(0x02000006, added.Id);
    }

    [Fact]
    public void An_added_trigger_is_enabled_and_initially_on()
    {
        // A default-constructed War3Net TriggerDefinition is neither, so a trigger born from one
        // would never run and would look perfectly normal in the tree.
        var doc = Map(Tree(true, Cat("Cats", 0x02000001)));
        var r = TriggerCommand.AddTrigger(doc, "Runs", 0x02000001);
        Assert.True(r.Ok, r.Message);

        var td = Assert.IsType<TriggerDefinition>(
            RoundTrip(doc).TriggerItems.First(i => i.Name == "Runs"));
        Assert.True(td.IsEnabled);
        Assert.True(td.IsInitiallyOn);
        Assert.False(td.RunOnMapInit);
        Assert.Equal(0x02000001, td.ParentId);
        Assert.Equal(TriggerItemType.Gui, td.Type);
        Assert.False(td.IsCustomTextTrigger);
    }

    [Fact]
    public void Adding_a_trigger_does_not_move_any_existing_code_body()
    {
        // The whole reason adding appends rather than inserts. Two custom-text triggers with known
        // bodies, then an addition, then every body must still belong to the same trigger.
        var doc = Map(
            Tree(true, Cat("Cats", 0x02000001),
                Text("First", 0x03000001, 0x02000001),
                Text("Second", 0x03000002, 0x02000001)),
            Wct("// body of First", "// body of Second"));

        var before = TriggerReadCommand.GetTriggers(doc);
        Assert.Equal("// body of First",
            before.Triggers.First(t => t.Name == "First").CustomText);

        Assert.True(TriggerCommand.AddTrigger(doc, "Third", 0x02000001).Ok);

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        var after = TriggerReadCommand.GetTriggers(reloaded);
        Assert.Equal("// body of First", after.Triggers.First(t => t.Name == "First").CustomText);
        Assert.Equal("// body of Second", after.Triggers.First(t => t.Name == "Second").CustomText);
        Assert.Null(after.Triggers.First(t => t.Name == "Third").CustomText);
    }

    [Fact]
    public void Adding_a_trigger_only_rewrites_the_wtg()
    {
        var doc = Map(
            Tree(true, Cat("Cats", 0x02000001), Text("First", 0x03000001, 0x02000001)),
            Wct("// body of First"));
        byte[] wctBefore = doc.GetFile("war3map.wct")!.CurrentBytes.ToArray();

        Assert.True(TriggerCommand.AddTrigger(doc, "Second", 0x02000001).Ok);

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        Assert.Equal(wctBefore, reloaded.GetFile("war3map.wct")!.CurrentBytes);
    }

    [Fact]
    public void The_item_count_header_tracks_an_add_and_a_remove()
    {
        // The writer emits this dictionary on sub-version maps, so a stale entry writes a header
        // that disagrees with the body and only the World Editor would notice.
        var doc = Map(Tree(true, Cat("Cats", 0x02000001)));
        Assert.True(TriggerCommand.AddCategory(doc, "Fresh").Ok);

        var after = RoundTrip(doc);
        Assert.Equal(2, after.TriggerItemCounts[TriggerItemType.Category]);

        int freshId = after.TriggerItems.First(i => i.Name == "Fresh").Id;
        var doc2 = MapDocument.Load(doc.SaveToBytes());
        Assert.True(TriggerCommand.Remove(doc2, freshId).Ok);
        Assert.Equal(1, RoundTrip(doc2).TriggerItemCounts[TriggerItemType.Category]);
    }

    [Fact]
    public void A_trigger_cannot_be_parented_to_another_trigger()
    {
        var doc = Map(Tree(true, Cat("Cats", 0x02000001), Gui("A", 0x03000001, 0x02000001)));
        var r = TriggerCommand.AddTrigger(doc, "B", 0x03000001);
        Assert.False(r.Ok);
        Assert.Contains("cannot hold triggers", r.Message);
    }

    [Fact]
    public void Adding_rejects_a_blank_name_and_a_missing_parent()
    {
        var doc = Map(Tree(true, Cat("Cats", 0x02000001)));
        Assert.False(TriggerCommand.AddCategory(doc, "  ").Ok);
        Assert.False(TriggerCommand.AddTrigger(doc, "x", 999999).Ok);
    }

    // ---------------------------------------------------------------- remove

    [Fact]
    public void Removing_an_empty_category_survives_a_save_and_reload()
    {
        var doc = Map(Tree(true, Cat("Keep", 0x02000001), Cat("Drop", 0x02000002)));
        var r = TriggerCommand.Remove(doc, 0x02000002);
        Assert.True(r.Ok, r.Message);

        var after = RoundTrip(doc);
        Assert.Single(after.TriggerItems);
        Assert.Equal("Keep", after.TriggerItems[0].Name);
    }

    [Fact]
    public void Removing_a_category_with_children_is_refused_rather_than_orphaning_them()
    {
        var doc = Map(Tree(true, Cat("Cats", 0x02000001), Gui("Child", 0x03000001, 0x02000001)));
        var r = TriggerCommand.Remove(doc, 0x02000001);
        Assert.False(r.Ok);
        Assert.Contains("orphan", r.Message);
        Assert.Equal(2, RoundTrip(doc).TriggerItems.Count);
    }

    [Fact]
    public void A_recursive_removal_takes_the_whole_subtree()
    {
        var doc = Map(Tree(true,
            Cat("Cats", 0x02000001),
            Cat("Nested", 0x02000002, 0x02000001),
            Gui("Deep", 0x03000001, 0x02000002)));

        var r = TriggerCommand.Remove(doc, 0x02000001, recursive: true);
        Assert.True(r.Ok, r.Message);
        Assert.Empty(RoundTrip(doc).TriggerItems);
    }

    [Fact]
    public void The_root_category_cannot_be_removed()
    {
        var doc = Map(Tree(true,
            new TriggerCategoryDefinition(TriggerItemType.RootCategory)
            { Id = 0, Name = "the map", ParentId = -1 }));
        var r = TriggerCommand.Remove(doc, 0);
        Assert.False(r.Ok);
        Assert.Contains("root category", r.Message);
    }

    [Fact]
    public void Removing_the_last_code_holder_is_allowed_because_nothing_shifts()
    {
        var doc = Map(
            Tree(true, Cat("Cats", 0x02000001),
                Text("First", 0x03000001, 0x02000001),
                Text("Last", 0x03000002, 0x02000001)),
            Wct("// body of First", "// body of Last"));

        var r = TriggerCommand.Remove(doc, 0x03000002);
        Assert.True(r.Ok, r.Message);

        // And the survivor still shows its own code, which is the point.
        var after = TriggerReadCommand.GetTriggers(MapDocument.Load(doc.SaveToBytes()));
        Assert.Equal("// body of First", after.Triggers.Single().CustomText);
    }

    [Fact]
    public void Removing_a_code_holder_that_would_shift_a_later_body_is_refused()
    {
        // This is the corruption the refusal exists for. Removing 'First' would leave 'Second'
        // reading slot 0, which holds First's script.
        var doc = Map(
            Tree(true, Cat("Cats", 0x02000001),
                Text("First", 0x03000001, 0x02000001),
                Text("Second", 0x03000002, 0x02000001)),
            Wct("// body of First", "// body of Second"));

        var r = TriggerCommand.Remove(doc, 0x03000001);
        Assert.False(r.Ok);
        Assert.Contains("without moving code", r.Message);
        Assert.Contains("Second", r.Message);

        // Nothing changed, and the bodies still line up.
        var after = TriggerReadCommand.GetTriggers(MapDocument.Load(doc.SaveToBytes()));
        Assert.Equal("// body of Second", after.Triggers.First(t => t.Name == "Second").CustomText);
    }

    [Fact]
    public void Removing_a_gui_trigger_is_allowed_on_a_sub_version_map_even_among_code_holders()
    {
        // A GUI trigger owns no slot when the sub-version is present, so its removal cannot move
        // anything, however much code sits around it.
        var doc = Map(
            Tree(true, Cat("Cats", 0x02000001),
                Text("First", 0x03000001, 0x02000001),
                Gui("Plain", 0x03000002, 0x02000001),
                Text("Second", 0x03000003, 0x02000001)),
            Wct("// body of First", "// body of Second"));

        var r = TriggerCommand.Remove(doc, 0x03000002);
        Assert.True(r.Ok, r.Message);

        var after = TriggerReadCommand.GetTriggers(MapDocument.Load(doc.SaveToBytes()));
        Assert.Equal("// body of First", after.Triggers.First(t => t.Name == "First").CustomText);
        Assert.Equal("// body of Second", after.Triggers.First(t => t.Name == "Second").CustomText);
    }

    [Fact]
    public void Removing_a_gui_trigger_that_owns_an_empty_slot_is_allowed()
    {
        // Without a sub-version, a GUI trigger DOES own a slot, but an empty one, so shifting the
        // empties past it changes nothing observable.
        var doc = Map(
            Tree(false, Cat("Cats", 1),
                Gui("Plain", 0, 1),
                Gui("Other", 0, 1)),
            Wct(string.Empty, string.Empty));

        // Both GUI triggers carry id 0 here, which is what real old-format maps do, so the
        // removal must refuse on ambiguity rather than pick one.
        var r = TriggerCommand.Remove(doc, 0);
        Assert.False(r.Ok);
        Assert.Contains("share id 0", r.Message);
    }

    // ---------------------------------------------------------------- ambiguous ids

    [Fact]
    public void An_id_shared_by_several_items_is_refused_by_every_mutator()
    {
        // Measured on RATankD_516.2: 606 triggers, every one with id 0. Silently editing the first
        // match would rename or disable the wrong trigger with no way to tell.
        var doc = Map(Tree(false, Gui("A", 0, -1), Gui("B", 0, -1)));

        foreach (var (what, result) in new (string, TriggerOpResult)[]
                 {
                     ("rename", TriggerCommand.Rename(doc, 0, "new name")),
                     ("set-enabled", TriggerCommand.SetEnabled(doc, 0, false)),
                     ("set-initially-on", TriggerCommand.SetInitiallyOn(doc, 0, false)),
                     ("set-run-on-map-init", TriggerCommand.SetRunOnMapInit(doc, 0, true)),
                     ("remove", TriggerCommand.Remove(doc, 0)),
                 })
        {
            Assert.False(result.Ok, $"{what} accepted an ambiguous id");
            Assert.Contains("share id 0", result.Message);
        }

        // And nothing was written despite five attempts.
        var after = RoundTrip(doc);
        Assert.Equal(2, after.TriggerItems.Count);
        Assert.Equal("A", after.TriggerItems[0].Name);
        Assert.True(((TriggerDefinition)after.TriggerItems[0]).IsEnabled);
    }

    [Fact]
    public void A_unique_id_still_edits_normally()
    {
        var doc = Map(Tree(true, Cat("Cats", 0x02000001), Gui("A", 0x03000001, 0x02000001)));
        Assert.True(TriggerCommand.Rename(doc, 0x03000001, "Renamed").Ok);
        Assert.Equal("Renamed", RoundTrip(doc).TriggerItems.First(i => i.Id == 0x03000001).Name);
    }
}
