using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Info;
using War3Net.Build.Script;
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Coverage for <see cref="TriggerReadCommand.GetTriggers"/>: the empty-map contract
/// (never throws, carries the script language), the wtg/wct parser path for the tree +
/// custom-text pairing, and the ECA function/parameter rendering (model-injected — the
/// binary function format needs TriggerData-driven parameter counts, which the corpus
/// test exercises against a real map).
/// </summary>
public class TriggerReadCommandTests
{
    private readonly ITestOutputHelper _output;
    public TriggerReadCommandTests(ITestOutputHelper output) => _output = output;

    // Newest format/sub versions (highest enum value = TFT), same recipe as
    // TriggerCommandTests — the round-trip probe proved this combination serializes.
    private static readonly MapTriggersFormatVersion Fmt =
        Enum.GetValues<MapTriggersFormatVersion>()[^1];
    private static readonly MapTriggersSubVersion Sub =
        Enum.GetValues<MapTriggersSubVersion>()[^1];

    // ---- empty-map contract ----

    [Fact]
    public void Blank_map_yields_empty_model_with_script_language()
    {
        var model = TriggerReadCommand.GetTriggers(BlankMap.Create());

        Assert.NotNull(model);
        Assert.Empty(model.Categories);
        Assert.Empty(model.Triggers);
        Assert.Empty(model.Variables);
        Assert.Equal(ScriptLanguage.Jass.ToString(), model.ScriptLanguage);
    }

    [Fact]
    public void Script_language_comes_from_map_info()
    {
        var doc = BlankMap.Create();
        ((MapInfo)doc.GetFile("war3map.w3i")!.Model!).ScriptLanguage = ScriptLanguage.Lua;

        Assert.Equal("Lua", TriggerReadCommand.GetTriggers(doc).ScriptLanguage);
    }

    // ---- wtg/wct parser path: tree structure + custom-text pairing ----

    /// <summary>Root category, one child category, one GUI trigger, one custom-text
    /// trigger, one initialized and one uninitialized variable. Functions stay empty here:
    /// serialized function bodies need TriggerData-consistent parameter counts to parse
    /// back, so the rendering tests below inject the model directly instead.</summary>
    private static MapTriggers NewTriggers()
    {
        var mt = (MapTriggers)Activator.CreateInstance(typeof(MapTriggers), Fmt, Sub)!;
        mt.TriggerItems.Add(new TriggerCategoryDefinition(TriggerItemType.RootCategory)
            { Id = 0, ParentId = -1, Name = "" });
        mt.TriggerItems.Add(new TriggerCategoryDefinition(TriggerItemType.Category)
            { Id = 1, ParentId = 0, Name = "Cat" });
        mt.TriggerItems.Add(new TriggerDefinition(TriggerItemType.Gui)
            { Id = 2, ParentId = 1, Name = "Trg", Description = "A gui trigger.",
              IsEnabled = true, IsInitiallyOn = true });
        mt.TriggerItems.Add(new TriggerDefinition(TriggerItemType.Script)
            { Id = 3, ParentId = 1, Name = "Cust", IsEnabled = true,
              IsCustomTextTrigger = true });
        mt.Variables.Add(new VariableDefinition
            { Id = 4, ParentId = -1, Name = "Count", Type = "integer",
              IsInitialized = true, InitialValue = "7" });
        mt.Variables.Add(new VariableDefinition
            { Id = 5, ParentId = -1, Name = "Units", Type = "unit",
              IsArray = true, ArraySize = 8 });
        foreach (var g in mt.TriggerItems.GroupBy(i => i.Type))
            mt.TriggerItemCounts[g.Key] = g.Count();
        return mt;
    }

    private const string CustomBody = "function Foo takes nothing returns nothing\nendfunction";

    /// <summary>One wct body per TriggerDefinition in wtg order: empty for the GUI
    /// trigger (ordinal 0), real code for the custom-text one (ordinal 1).</summary>
    private static byte[] CustomTextBytes()
    {
        var wct = new MapCustomTextTriggers(MapCustomTextTriggersFormatVersion.v1, null)
        {
            GlobalCustomScriptComment = string.Empty,
            GlobalCustomScriptCode = new CustomTextTrigger { Code = string.Empty },
        };
        wct.CustomTextTriggers.Add(new CustomTextTrigger { Code = string.Empty });
        wct.CustomTextTriggers.Add(new CustomTextTrigger { Code = CustomBody + "\0" });

        using var ms = new MemoryStream();
        using (var w = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
            w.Write(wct);
        return ms.ToArray();
    }

    private static MapDocument ParsedDoc() =>
        MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [TriggerReadCommand.TriggersFileName] = TriggerCommand.Serialize(NewTriggers()),
            [TriggerReadCommand.CustomTextFileName] = CustomTextBytes(),
        }));

    [Fact]
    public void Reads_categories_triggers_and_variables_through_the_parser()
    {
        var model = TriggerReadCommand.GetTriggers(ParsedDoc());

        Assert.Equal(2, model.Categories.Count);
        var cat = model.Categories.Single(c => c.Id == 1);
        Assert.Equal("Cat", cat.Name);
        Assert.Equal(TriggerItemType.Category.ToString(), cat.Kind);

        Assert.Equal(2, model.Triggers.Count);
        var trg = model.Triggers.Single(t => t.Id == 2);
        Assert.Equal(1, trg.ParentCategoryId);
        Assert.Equal("Trg", trg.Name);
        Assert.Equal("A gui trigger.", trg.Description);
        Assert.True(trg.Enabled);
        Assert.True(trg.InitiallyOn);
        Assert.False(trg.IsCustomText);
        Assert.Empty(trg.Functions);
        Assert.Null(trg.CustomText);

        var vars = model.Variables;
        Assert.Equal(2, vars.Count);
        Assert.Equal("7", vars.Single(v => v.Name == "Count").InitialValue);
        var units = vars.Single(v => v.Name == "Units");
        Assert.True(units.IsArray);
        Assert.Null(units.InitialValue); // not initialized
    }

    [Fact]
    public void Custom_text_trigger_gets_its_wct_body()
    {
        var model = TriggerReadCommand.GetTriggers(ParsedDoc());

        var cust = model.Triggers.Single(t => t.Id == 3);
        Assert.True(cust.IsCustomText);
        Assert.Equal(CustomBody, cust.CustomText); // trailing NUL trimmed
    }

    [Fact]
    public void Missing_wct_leaves_custom_text_null_without_throwing()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [TriggerReadCommand.TriggersFileName] = TriggerCommand.Serialize(NewTriggers()),
        }));

        var cust = TriggerReadCommand.GetTriggers(doc).Triggers.Single(t => t.Id == 3);
        Assert.True(cust.IsCustomText);
        Assert.Null(cust.CustomText);
    }

    // ---- ECA function/parameter rendering (model-injected) ----

    private static TriggerFunctionParameter Str(string value) =>
        new() { Type = TriggerFunctionParameterType.String, Value = value };

    private static TriggerFunctionParameter Call(string name, params TriggerFunctionParameter[] args)
    {
        var fn = new TriggerFunction
            { Type = TriggerFunctionType.Call, Name = name, IsEnabled = true };
        fn.Parameters.AddRange(args);
        return new TriggerFunctionParameter
            { Type = TriggerFunctionParameterType.Function, Value = name, Function = fn };
    }

    /// <summary>A doc whose wtg model is installed directly (no serialize/parse round
    /// trip) so function trees can be shaped freely.</summary>
    private static MapDocument DocWithFunctions(params TriggerFunction[] functions)
    {
        var mt = NewTriggers();
        var trg = (TriggerDefinition)mt.TriggerItems.Single(i => i.Id == 2);
        trg.Functions.AddRange(functions);

        var doc = BlankMap.Create();
        doc.AddOrReplaceModelFile(TriggerReadCommand.TriggersFileName, mt);
        return doc;
    }

    [Fact]
    public void Renders_event_condition_and_action_kinds()
    {
        var evt = new TriggerFunction
            { Type = TriggerFunctionType.Event, Name = "TriggerRegisterTimerEventPeriodic", IsEnabled = true };
        evt.Parameters.Add(Str("5.00"));
        var cond = new TriggerFunction
            { Type = TriggerFunctionType.Condition, Name = "OperatorCompareBoolean", IsEnabled = true };
        var act = new TriggerFunction
            { Type = TriggerFunctionType.Action, Name = "DoNothing", IsEnabled = false };

        var trg = TriggerReadCommand.GetTriggers(DocWithFunctions(evt, cond, act))
            .Triggers.Single(t => t.Id == 2);

        Assert.Equal(new[] { "Event", "Condition", "Action" }, trg.Functions.Select(f => f.Kind));
        Assert.Equal("TriggerRegisterTimerEventPeriodic", trg.Functions[0].Name);
        Assert.False(trg.Functions[2].Enabled);
    }

    [Fact]
    public void Renders_parameters_readably()
    {
        var act = new TriggerFunction
            { Type = TriggerFunctionType.Action, Name = "SetVariable", IsEnabled = true };
        act.Parameters.Add(new TriggerFunctionParameter
        {
            Type = TriggerFunctionParameterType.Variable,
            Value = "MyArray",
            ArrayIndexer = Str("3"),
        });
        act.Parameters.Add(Str("hello world"));
        act.Parameters.Add(new TriggerFunctionParameter
            { Type = TriggerFunctionParameterType.Preset, Value = "Player00" });
        act.Parameters.Add(Call("IsUnitOwnedByPlayer", Call("GetTriggerUnit"), Str("42")));

        var fn = TriggerReadCommand.GetTriggers(DocWithFunctions(act))
            .Triggers.Single(t => t.Id == 2).Functions.Single();

        Assert.Equal(
            new[]
            {
                "MyArray[3]",              // variable with array indexer; numeric index bare
                "\"hello world\"",         // non-numeric string literal quoted
                "Player00",                // preset by raw name
                "IsUnitOwnedByPlayer(GetTriggerUnit(), 42)", // nested calls inline
            },
            fn.Parameters);
    }

    [Fact]
    public void Renders_child_function_blocks()
    {
        var ifThen = new TriggerFunction
            { Type = TriggerFunctionType.Action, Name = "IfThenElseMultiple", IsEnabled = true };
        var child = new TriggerFunction
            { Type = TriggerFunctionType.Action, Name = "DisplayTextToForce",
              IsEnabled = true, Branch = 1 };
        child.Parameters.Add(Call("GetPlayersAll"));
        child.Parameters.Add(Str("Hello"));
        ifThen.ChildFunctions.Add(child);

        var fn = TriggerReadCommand.GetTriggers(DocWithFunctions(ifThen))
            .Triggers.Single(t => t.Id == 2).Functions.Single();

        var c = Assert.Single(fn.Children);
        Assert.Equal("DisplayTextToForce", c.Name);
        Assert.Equal(new[] { "GetPlayersAll()", "\"Hello\"" }, c.Parameters);
        Assert.Empty(c.Children);
    }

    [Fact]
    public void Pathological_nesting_is_depth_guarded()
    {
        // A parameter chain nested far beyond the guard: f(f(f(...))). Must terminate
        // and render a truncated stub instead of recursing forever.
        var inner = Str("0");
        for (int i = 0; i < 200; i++) inner = Call("Deep", inner);
        var act = new TriggerFunction
            { Type = TriggerFunctionType.Action, Name = "SetVariable", IsEnabled = true };
        act.Parameters.Add(inner);

        var fn = TriggerReadCommand.GetTriggers(DocWithFunctions(act))
            .Triggers.Single(t => t.Id == 2).Functions.Single();

        var rendered = Assert.Single(fn.Parameters);
        Assert.Contains("...", rendered);
        Assert.StartsWith("Deep(", rendered);
    }

    // ---- corpus: a real map's trigger file through the full load path ----

    private static readonly string CorpusMap =
        TestCorpus.Map(@"GGGA_V0.02a.w3x");

    [Fact]
    [Trait("Category", "Corpus")]
    public void Corpus_map_triggers_read_without_throwing()
    {
        if (!File.Exists(CorpusMap)) return;

        var model = TriggerReadCommand.GetTriggers(MapDocument.Load(CorpusMap));

        Assert.NotNull(model);
        _output.WriteLine(
            $"categories={model.Categories.Count} triggers={model.Triggers.Count} " +
            $"variables={model.Variables.Count} language={model.ScriptLanguage}");
        Assert.True(
            model.Categories.Count + model.Triggers.Count + model.Variables.Count > 0,
            "expected at least one category, trigger or variable in the corpus map");

        foreach (var t in model.Triggers.Take(5))
            _output.WriteLine(
                $"  [{t.Id}] '{t.Name}' functions={t.Functions.Count} custom={t.IsCustomText}");
    }
}
