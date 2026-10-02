using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using War3Net.Build.Extensions;
using War3Net.Build.Script;
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// Coverage for <see cref="TriggerCommand"/>: byte-faithful (.wtg) serialize/round-trip, the
/// read-only <c>List</c>/<c>ListVariables</c> projections, and the mutating rename/flag ops,
/// all driven through a real <see cref="MapDocument"/> so the wired <c>ReadMapTriggers</c>
/// parser and <c>AddOrReplaceRawFile</c> persistence path are exercised end-to-end.
/// </summary>
public class TriggerCommandTests
{
    // Newest format/sub versions (highest enum value = TFT), matching what the round-trip
    // probe proved. Referenced positionally so we do not depend on member spellings.
    private static readonly MapTriggersFormatVersion Fmt =
        Enum.GetValues<MapTriggersFormatVersion>()[^1];
    private static readonly MapTriggersSubVersion Sub =
        Enum.GetValues<MapTriggersSubVersion>()[^1];

    /// <summary>Builds a minimal but valid MapTriggers tree: a root category, one child
    /// category, one GUI trigger (enabled + initially-on), and one integer variable.</summary>
    private static MapTriggers NewTriggers()
    {
        var mt = (MapTriggers)Activator.CreateInstance(typeof(MapTriggers), Fmt, Sub)!;
        mt.TriggerItems.Add(new TriggerCategoryDefinition(TriggerItemType.RootCategory)
            { Id = 0, ParentId = -1, Name = "" });
        mt.TriggerItems.Add(new TriggerCategoryDefinition(TriggerItemType.Category)
            { Id = 1, ParentId = 0, Name = "Cat" });
        mt.TriggerItems.Add(new TriggerDefinition(TriggerItemType.Gui)
            { Id = 2, ParentId = 1, Name = "Trg", IsEnabled = true, IsInitiallyOn = true });
        mt.Variables.Add(new VariableDefinition
            { Id = 3, ParentId = -1, Name = "V", Type = "integer", InitialValue = "0" });
        foreach (var g in mt.TriggerItems.GroupBy(i => i.Type))
            mt.TriggerItemCounts[g.Key] = g.Count();
        return mt;
    }

    /// <summary>A MapDocument whose war3map.wtg is the freshly-built minimal tree, loaded
    /// through the real parser so <c>.Model</c> is a populated MapTriggers.</summary>
    private static MapDocument Doc() =>
        MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [TriggerCommand.FileName] = TriggerCommand.Serialize(NewTriggers()),
        }));

    private static byte[] Wtg(MapDocument doc) => doc.GetFile(TriggerCommand.FileName)!.OverrideBytes!;

    // ---- pure serialize round-trip (byte-faithful) ----

    [Fact]
    public void Serialize_round_trips_byte_identical()
    {
        var bytes = TriggerCommand.Serialize(NewTriggers());
        using var ms = new MemoryStream(bytes);
        using var r = new BinaryReader(ms);
        var reser = TriggerCommand.Serialize(r.ReadMapTriggers());
        Assert.Equal(bytes, reser);
    }

    // ---- read-only projections ----

    [Fact]
    public void List_returns_every_item_with_fields()
    {
        var items = TriggerCommand.List(Doc());
        Assert.Equal(3, items.Count);

        var trg = items.Single(i => i.Id == 2);
        Assert.Equal(1, trg.ParentId);
        Assert.Equal("Trg", trg.Name);
        Assert.Equal(TriggerItemType.Gui.ToString(), trg.Type);
        Assert.True(trg.IsEnabled);
        Assert.True(trg.IsInitiallyOn);
        Assert.False(trg.RunOnMapInit);
        Assert.False(trg.IsComment);
        Assert.Equal(0, trg.FunctionCount);

        // Category items carry no trigger-only flags.
        var cat = items.Single(i => i.Id == 1);
        Assert.Null(cat.IsEnabled);
        Assert.Null(cat.FunctionCount);
    }

    [Fact]
    public void ListVariables_returns_the_variable()
    {
        var v = Assert.Single(TriggerCommand.ListVariables(Doc()));
        Assert.Equal(3, v.Id);
        Assert.Equal("V", v.Name);
        Assert.Equal("integer", v.Type);
        Assert.False(v.IsArray);
        Assert.Equal("0", v.InitialValue);
    }

    // ---- mutating ops ----

    [Fact]
    public void Rename_changes_trigger_name()
    {
        var doc = Doc();
        var res = TriggerCommand.Rename(doc, 2, "Renamed");
        Assert.True(res.Ok, res.Message);
        Assert.Equal(3, res.Count);
        Assert.Equal("Renamed", TriggerCommand.List(doc).Single(i => i.Id == 2).Name);
    }

    [Fact]
    public void Rename_rejects_missing_id()
    {
        var res = TriggerCommand.Rename(Doc(), 99, "Nope");
        Assert.False(res.Ok);
        Assert.Contains("99", res.Message);
    }

    [Fact]
    public void Rename_accepts_a_category()
    {
        // Rename is documented to work on any item kind, categories included.
        var doc = Doc();
        Assert.True(TriggerCommand.Rename(doc, 1, "Renamed Cat").Ok);
        Assert.Equal("Renamed Cat", TriggerCommand.List(doc).Single(i => i.Id == 1).Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Rename_rejects_blank_name(string blank)
    {
        var res = TriggerCommand.Rename(Doc(), 2, blank);
        Assert.False(res.Ok);
        Assert.Contains("blank", res.Message);
    }

    [Fact]
    public void SetEnabled_rejects_a_category()
    {
        // Id 1 is a Category, which carries no enabled flag.
        var res = TriggerCommand.SetEnabled(Doc(), 1, false);
        Assert.False(res.Ok);
        Assert.Contains("not an editable trigger", res.Message);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SetEnabled_toggles_flag(bool enabled)
    {
        var doc = Doc();
        Assert.True(TriggerCommand.SetEnabled(doc, 2, enabled).Ok);
        Assert.Equal(enabled, TriggerCommand.List(doc).Single(i => i.Id == 2).IsEnabled);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SetInitiallyOn_toggles_flag(bool on)
    {
        var doc = Doc();
        Assert.True(TriggerCommand.SetInitiallyOn(doc, 2, on).Ok);
        Assert.Equal(on, TriggerCommand.List(doc).Single(i => i.Id == 2).IsInitiallyOn);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SetRunOnMapInit_toggles_flag(bool on)
    {
        var doc = Doc();
        Assert.True(TriggerCommand.SetRunOnMapInit(doc, 2, on).Ok);
        Assert.Equal(on, TriggerCommand.List(doc).Single(i => i.Id == 2).RunOnMapInit);
    }

    // ---- persistence: a mutation survives a save + reload through the parser ----

    [Fact]
    public void Mutation_round_trips_through_save_and_reload()
    {
        var doc = Doc();
        Assert.True(TriggerCommand.Rename(doc, 2, "Persisted").Ok);
        Assert.True(TriggerCommand.SetEnabled(doc, 2, false).Ok);

        var reloaded = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [TriggerCommand.FileName] = Wtg(doc),
        }));

        var trg = TriggerCommand.List(reloaded).Single(i => i.Id == 2);
        Assert.Equal("Persisted", trg.Name);
        Assert.False(trg.IsEnabled);
    }
}
