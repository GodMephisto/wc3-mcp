// tests/Wc3.Tests/EcaAuthoringTests.cs
using War3Net.Build.Extensions;
using War3Net.Build.Script;
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Authoring events, conditions and actions.
///
/// The whole feature turns on one measured fact: war3map.wtg stores a function's parameters but
/// NOT how many there are, so the count is taken from the World-Editor function table on read.
/// A function written with the wrong arity does not produce a wrong trigger, it produces a file
/// that throws on the next read, and the map still saves. Before this work, writing one action
/// with one parameter (where the table declares two) made the whole wtg unparseable, the Studio
/// showed zero triggers, and nothing reported a problem.
///
/// So every test here goes through a real save and reload, and the one that matters most is the
/// one asserting the map is still READABLE afterwards.
/// </summary>
public class EcaAuthoringTests
{
    private readonly ITestOutputHelper _out;
    public EcaAuthoringTests(ITestOutputHelper output) => _out = output;

    private const int CategoryId = 0x02000001;
    private const int TriggerId = 0x03000001;
    private const int TextTriggerId = 0x03000002;

    private static byte[] Bytes(MapTriggers t)
    {
        using var ms = new MemoryStream();
        using (var w = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
            w.Write(t);
        return ms.ToArray();
    }

    private static MapDocument Map()
    {
        var t = new MapTriggers(MapTriggersFormatVersion.v7, MapTriggersSubVersion.v4);
        t.TriggerItems.Add(new TriggerCategoryDefinition
        { Id = CategoryId, Name = "Cats", ParentId = -1 });
        t.TriggerItems.Add(new TriggerDefinition(TriggerItemType.Gui)
        {
            Id = TriggerId, Name = "Melee Init", Description = string.Empty,
            ParentId = CategoryId, IsEnabled = true, IsInitiallyOn = true,
        });
        t.TriggerItems.Add(new TriggerDefinition(TriggerItemType.Script)
        {
            Id = TextTriggerId, Name = "Scripted", Description = string.Empty,
            ParentId = CategoryId, IsEnabled = true, IsCustomTextTrigger = true,
        });
        foreach (var type in Enum.GetValues<TriggerItemType>())
            t.TriggerItemCounts[type] = t.TriggerItems.Count(i => i.Type == type);

        var doc = BlankMap.Create();
        doc.AddOrReplaceRawFile("war3map.wtg", Bytes(t));
        return MapDocument.Load(doc.SaveToBytes());
    }

    private static TriggerModel Reload(MapDocument doc) =>
        TriggerReadCommand.GetTriggers(MapDocument.Load(doc.SaveToBytes()));

    // ---------------------------------------------------------------- the load-bearing one

    [Fact]
    public void An_added_action_survives_a_save_and_the_map_is_still_readable()
    {
        var doc = Map();
        var r = TriggerCommand.AddFunction(doc, TriggerId, TriggerFunctionType.Action,
            "DisplayTextToForce", new[] { "GetPlayersAll", "hello world" });
        Assert.True(r.Ok, r.Message);
        _out.WriteLine(r.Message);

        var model = Reload(doc);
        var trig = model.Triggers.Single(t => t.Id == TriggerId);
        Assert.Single(trig.Functions);

        var fn = trig.Functions[0];
        _out.WriteLine($"read back: {fn.Kind} {fn.Name}({string.Join(", ", fn.Parameters)})");
        Assert.Equal("Action", fn.Kind);
        Assert.Equal("DisplayTextToForce", fn.Name);
        Assert.Equal(2, fn.Parameters.Count);
    }

    [Fact]
    public void An_event_with_no_parameters_round_trips()
    {
        var doc = Map();
        Assert.True(TriggerCommand.AddFunction(doc, TriggerId, TriggerFunctionType.Event,
            "MapInitializationEvent").Ok);

        var fn = Reload(doc).Triggers.Single(t => t.Id == TriggerId).Functions.Single();
        Assert.Equal("Event", fn.Kind);
        Assert.Equal("MapInitializationEvent", fn.Name);
        Assert.Empty(fn.Parameters);
    }

    [Fact]
    public void Omitted_parameters_are_filled_from_the_tables_own_defaults()
    {
        // DisplayTextToForce declares [force, StringExt] with a default of GetPlayersAll for the
        // first. Supplying neither must still produce two parameters, because two is what the
        // reader will expect.
        var doc = Map();
        var r = TriggerCommand.AddFunction(doc, TriggerId, TriggerFunctionType.Action,
            "DisplayTextToForce");
        Assert.True(r.Ok, r.Message);

        var fn = Reload(doc).Triggers.Single(t => t.Id == TriggerId).Functions.Single();
        Assert.Equal(2, fn.Parameters.Count);
    }

    [Fact]
    public void Several_functions_in_a_row_all_survive()
    {
        var doc = Map();
        Assert.True(TriggerCommand.AddFunction(doc, TriggerId, TriggerFunctionType.Event,
            "MapInitializationEvent").Ok);
        Assert.True(TriggerCommand.AddFunction(doc, TriggerId, TriggerFunctionType.Action,
            "DisplayTextToForce", new[] { "GetPlayersAll", "one" }).Ok);
        Assert.True(TriggerCommand.AddFunction(doc, TriggerId, TriggerFunctionType.Action,
            "DisplayTextToForce", new[] { "GetPlayersAll", "two" }).Ok);

        var trig = Reload(doc).Triggers.Single(t => t.Id == TriggerId);
        Assert.Equal(3, trig.Functions.Count);
        Assert.Equal(new[] { "Event", "Action", "Action" },
            trig.Functions.Select(f => f.Kind).ToArray());
    }

    // ---------------------------------------------------------------- refusals

    [Fact]
    public void An_unknown_function_is_refused_with_suggestions()
    {
        var doc = Map();
        var r = TriggerCommand.AddFunction(doc, TriggerId, TriggerFunctionType.Action,
            "DisplayText");
        Assert.False(r.Ok);
        _out.WriteLine(r.Message);
        Assert.Contains("declares no action", r.Message);
        Assert.Contains("Did you mean", r.Message);
    }

    [Fact]
    public void Too_many_parameters_are_refused_rather_than_truncated()
    {
        var doc = Map();
        var r = TriggerCommand.AddFunction(doc, TriggerId, TriggerFunctionType.Action,
            "DisplayTextToForce", new[] { "GetPlayersAll", "a", "b", "c" });
        Assert.False(r.Ok);
        _out.WriteLine(r.Message);
        Assert.Contains("takes 2 parameter(s)", r.Message);

        // And nothing was written.
        Assert.Empty(Reload(doc).Triggers.Single(t => t.Id == TriggerId).Functions);
    }

    [Fact]
    public void A_custom_text_trigger_is_refused_because_its_body_is_script()
    {
        var doc = Map();
        var r = TriggerCommand.AddFunction(doc, TextTriggerId, TriggerFunctionType.Action,
            "DisplayTextToForce");
        Assert.False(r.Ok);
        _out.WriteLine(r.Message);
        Assert.Contains("custom-text trigger", r.Message);
    }

    [Fact]
    public void A_category_is_refused()
    {
        var doc = Map();
        var r = TriggerCommand.AddFunction(doc, CategoryId, TriggerFunctionType.Action,
            "DisplayTextToForce");
        Assert.False(r.Ok);
        Assert.Contains("not a trigger", r.Message);
    }

    // ---------------------------------------------------------------- remove and toggle

    [Fact]
    public void A_function_can_be_removed_by_index()
    {
        var doc = Map();
        TriggerCommand.AddFunction(doc, TriggerId, TriggerFunctionType.Event, "MapInitializationEvent");
        TriggerCommand.AddFunction(doc, TriggerId, TriggerFunctionType.Action,
            "DisplayTextToForce", new[] { "GetPlayersAll", "keep me" });

        var r = TriggerCommand.RemoveFunction(doc, TriggerId, 0);
        Assert.True(r.Ok, r.Message);

        var trig = Reload(doc).Triggers.Single(t => t.Id == TriggerId);
        Assert.Single(trig.Functions);
        Assert.Equal("DisplayTextToForce", trig.Functions[0].Name);
    }

    [Fact]
    public void Removing_an_out_of_range_index_is_refused()
    {
        var doc = Map();
        var r = TriggerCommand.RemoveFunction(doc, TriggerId, 0);
        Assert.False(r.Ok);
        Assert.Contains("no events, conditions or actions", r.Message);
    }

    [Fact]
    public void A_single_function_can_be_disabled_without_disabling_the_trigger()
    {
        var doc = Map();
        TriggerCommand.AddFunction(doc, TriggerId, TriggerFunctionType.Action,
            "DisplayTextToForce", new[] { "GetPlayersAll", "hi" });

        Assert.True(TriggerCommand.SetFunctionEnabled(doc, TriggerId, 0, false).Ok);

        var trig = Reload(doc).Triggers.Single(t => t.Id == TriggerId);
        Assert.False(trig.Functions[0].Enabled);
        Assert.True(trig.Enabled);   // the trigger itself is untouched
    }

    // ---------------------------------------------------------------- the catalog

    [Fact]
    public void The_catalog_reports_real_signatures()
    {
        var sig = TriggerFunctionBuilder.Signature(TriggerFunctionType.Action, "DisplayTextToForce");
        Assert.NotNull(sig);
        Assert.Equal(2, sig!.ArgumentTypes.Count);
        _out.WriteLine($"{sig.Name}: {string.Join(", ", sig.ArgumentTypes)} "
                     + $"defaults [{string.Join(", ", sig.Defaults)}] '{sig.DisplayName}'");

        Assert.NotEmpty(TriggerFunctionBuilder.Catalog(TriggerFunctionType.Event));
        Assert.NotEmpty(TriggerFunctionBuilder.Catalog(TriggerFunctionType.Action));
    }
}
