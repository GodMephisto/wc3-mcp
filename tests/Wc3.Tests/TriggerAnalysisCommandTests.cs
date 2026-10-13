// tests/Wc3.Tests/TriggerAnalysisCommandTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;
using War3Net.Build.Extensions;
using War3Net.Build.Script;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// The five read-only trigger analysis commands (to-script, diff, lint, export, dead). Every
/// fixture is a synthetic map carrying a war3map.wtg built and serialized in-process, so these run
/// hermetically with no game install and no map library. Removing any of the commands makes this
/// file fail to compile, which is the cheapest possible "fails without the change".
/// </summary>
public class TriggerAnalysisCommandTests
{
    // ------------------------------------------------------------------ fixtures

    private static byte[] Bytes(MapTriggers t)
    {
        using var ms = new MemoryStream();
        using (var w = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
            w.Write(t);
        return ms.ToArray();
    }

    private static MapDocument Map(MapTriggers t, string? script = null)
    {
        var doc = BlankMap.Create();
        doc.AddOrReplaceRawFile("war3map.wtg", Bytes(t));
        if (script is not null)
            doc.AddOrReplaceRawFile("war3map.j", Encoding.UTF8.GetBytes(script));
        return MapDocument.Load(doc.SaveToBytes());
    }

    private static MapTriggers Tree(
        IReadOnlyList<TriggerDefinition> triggers,
        IReadOnlyList<VariableDefinition>? variables = null)
    {
        var t = new MapTriggers(MapTriggersFormatVersion.v7, MapTriggersSubVersion.v4);
        t.TriggerItems.Add(new TriggerCategoryDefinition
        { Id = 0x02000001, Name = "Systems", ParentId = -1, IsExpanded = true });
        foreach (var td in triggers) t.TriggerItems.Add(td);
        foreach (var v in variables ?? Array.Empty<VariableDefinition>()) t.Variables.Add(v);
        foreach (var type in Enum.GetValues<TriggerItemType>())
            t.TriggerItemCounts[type] = t.TriggerItems.Count(i => i.Type == type);
        return t;
    }

    private static TriggerDefinition Gui(
        int id, string name, bool enabled = true, bool runOnInit = false) =>
        new(TriggerItemType.Gui)
        {
            Id = id, Name = name, Description = string.Empty, ParentId = 0x02000001,
            IsEnabled = enabled, IsInitiallyOn = true, RunOnMapInit = runOnInit,
        };

    private static TriggerFunction Fn(TriggerFunctionType type, string name, params TriggerFunctionParameter[] args)
    {
        var f = new TriggerFunction { Type = type, Name = name, IsEnabled = true };
        f.Parameters.AddRange(args);
        return f;
    }

    private static TriggerFunctionParameter Preset(string value) =>
        new() { Type = TriggerFunctionParameterType.Preset, Value = value };

    private static TriggerFunctionParameter Text(string value) =>
        new() { Type = TriggerFunctionParameterType.String, Value = value };

    private static TriggerFunctionParameter Var(string value) =>
        new() { Type = TriggerFunctionParameterType.Variable, Value = value };

    private static VariableDefinition Variable(string name, string type, string? initial = null) =>
        new()
        {
            Id = 1, ParentId = -1, Name = name, Type = type,
            IsInitialized = initial is not null, InitialValue = initial ?? string.Empty,
        };

    // ------------------------------------------------------------------ to-script

    [Fact]
    public void To_script_renders_events_conditions_and_actions()
    {
        var trig = Gui(0x03000001, "Melee Init");
        trig.Functions.Add(Fn(TriggerFunctionType.Event, "MapInitializationEvent"));
        trig.Functions.Add(Fn(TriggerFunctionType.Condition, "OperatorCompareBoolean",
            Preset("GetPlayersAll"), Preset("EqualNotEqualOperator"), Preset("true")));
        trig.Functions.Add(Fn(TriggerFunctionType.Action, "DisplayTextToForce",
            Preset("GetPlayersAll"), Text("hello")));

        var r = TriggerToScriptCommand.Render(Map(Tree(new[] { trig })));

        Assert.False(r.CatalogApplied);
        Assert.Contains("function InitTrig_Melee_Init takes nothing returns nothing", r.Script);
        Assert.Contains("call MapInitializationEvent( gg_trg_Melee_Init )", r.Script);
        Assert.Contains("call DisplayTextToForce( GetPlayersAll, \"hello\" )", r.Script);
        Assert.Contains("TriggerAddCondition( gg_trg_Melee_Init,", r.Script);
        Assert.Contains("TriggerAddAction( gg_trg_Melee_Init,", r.Script);
        Assert.Contains("returns boolean", r.Script);
    }

    [Fact]
    public void To_script_resolves_a_preset_through_the_catalog_when_one_is_given()
    {
        var action = Fn(TriggerFunctionType.Action, "DisplayTextToForce", Preset("OperatorAdd"), Text("x"));
        var trig = Gui(0x03000001, "T");
        trig.Functions.Add(Fn(TriggerFunctionType.Event, "MapInitializationEvent"));
        trig.Functions.Add(action);
        var map = Map(Tree(new[] { trig }));

        var withoutCatalog = TriggerToScriptCommand.Render(map);
        Assert.Contains("OperatorAdd", withoutCatalog.Script);

        var catalog = Wc3.GameData.TriggerDataParser.Parse(
            "[TriggerParams]\nOperatorAdd=0,ArithmeticOperator,\"+\",WESTRING_ARITH_ADD\n");
        var withCatalog = TriggerToScriptCommand.Render(map, catalog);
        Assert.True(withCatalog.CatalogApplied);
        Assert.Contains("( +, \"x\" )", withCatalog.Script);
        Assert.DoesNotContain("OperatorAdd", withCatalog.Script);
    }

    // ------------------------------------------------------------------ export

    [Fact]
    public void Export_splits_each_trigger_into_event_condition_action_lists()
    {
        var trig = Gui(0x03000001, "Melee Init");
        trig.Functions.Add(Fn(TriggerFunctionType.Event, "MapInitializationEvent"));
        trig.Functions.Add(Fn(TriggerFunctionType.Condition, "OperatorCompareBoolean",
            Preset("GetPlayersAll"), Preset("EqualNotEqualOperator"), Preset("true")));
        trig.Functions.Add(Fn(TriggerFunctionType.Action, "DisplayTextToForce",
            Preset("GetPlayersAll"), Text("hi")));

        var r = TriggerExportCommand.Export(Map(Tree(new[] { trig }, new[] { Variable("count", "integer", "0") })));

        var item = Assert.Single(r.Triggers);
        Assert.Equal("Melee Init", item.Name);
        Assert.True(item.Enabled);
        Assert.True(item.InitiallyOn);
        Assert.False(item.RunOnMapInit);
        Assert.Equal("MapInitializationEvent", Assert.Single(item.Events).Name);
        Assert.Equal("OperatorCompareBoolean", Assert.Single(item.Conditions).Name);
        var action = Assert.Single(item.Actions);
        Assert.Equal("DisplayTextToForce", action.Name);
        Assert.Equal(new[] { "GetPlayersAll", "\"hi\"" }, action.Parameters);
        Assert.Equal("count", Assert.Single(r.Variables).Name);
    }

    // ------------------------------------------------------------------ lint

    [Fact]
    public void Lint_flags_unreachable_duplicate_event_and_no_action()
    {
        var good = Gui(0x03000001, "Good");
        good.Functions.Add(Fn(TriggerFunctionType.Event, "MapInitializationEvent"));
        good.Functions.Add(Fn(TriggerFunctionType.Action, "DoNothing"));

        var noEvent = Gui(0x03000002, "NoEvent");
        noEvent.Functions.Add(Fn(TriggerFunctionType.Action, "DoNothing"));

        var duplicate = Gui(0x03000003, "Duplicate");
        duplicate.Functions.Add(Fn(TriggerFunctionType.Event, "MapInitializationEvent"));
        duplicate.Functions.Add(Fn(TriggerFunctionType.Event, "MapInitializationEvent"));
        duplicate.Functions.Add(Fn(TriggerFunctionType.Action, "DoNothing"));

        var noAction = Gui(0x03000004, "NoAction");
        noAction.Functions.Add(Fn(TriggerFunctionType.Event, "MapInitializationEvent"));

        var disabled = Gui(0x03000005, "Off", enabled: false);

        var script = new TriggerDefinition(TriggerItemType.Script)
        {
            Id = 0x03000006, Name = "Custom Script", Description = string.Empty, ParentId = 0x02000001,
            IsEnabled = true, IsInitiallyOn = true, RunOnMapInit = false,
        };

        var map = Map(Tree(new[] { good, noEvent, duplicate, noAction, disabled, script }));
        var r = TriggerLintCommand.Lint(map);

        Assert.Equal(4, r.TriggersChecked);
        Assert.Equal(1, r.DisabledSkipped);
        Assert.Equal(1, r.CustomTextSkipped);   // the Script trigger, straight to wct, not linted
        Assert.DoesNotContain(r.Findings, f => f.TriggerName == "Good");
        Assert.Contains(r.Findings, f => f.Rule == TriggerLintCommand.RuleUnreachable && f.TriggerName == "NoEvent");
        Assert.Contains(r.Findings, f => f.Rule == TriggerLintCommand.RuleDuplicateEvent && f.TriggerName == "Duplicate");
        Assert.Contains(r.Findings, f => f.Rule == TriggerLintCommand.RuleNoActions && f.TriggerName == "NoAction");
        Assert.DoesNotContain(r.Findings, f => f.TriggerName == "Off");
    }

    // ------------------------------------------------------------------ dead

    [Fact]
    public void Dead_finds_no_action_unit_event_and_never_set_variable()
    {
        var noAction = Gui(0x03000001, "NoAction");
        noAction.Functions.Add(Fn(TriggerFunctionType.Event, "MapInitializationEvent"));

        var unitEvent = Gui(0x03000002, "UnitEvent");
        unitEvent.Functions.Add(Fn(TriggerFunctionType.Event, "TriggerRegisterUnitEvent",
            Var("udg_myUnit"), Text("EVENT_UNIT_SPELL_EFFECT")));
        unitEvent.Functions.Add(Fn(TriggerFunctionType.Action, "DoNothing"));

        var variableEvent = Gui(0x03000003, "VariableEvent");
        variableEvent.Functions.Add(Fn(TriggerFunctionType.Event, "TriggerRegisterVariableEvent",
            Var("udg_flag"), Text("=="), Text("0")));
        variableEvent.Functions.Add(Fn(TriggerFunctionType.Action, "DoNothing"));

        var alive = Gui(0x03000004, "Alive");
        alive.Functions.Add(Fn(TriggerFunctionType.Event, "MapInitializationEvent"));
        alive.Functions.Add(Fn(TriggerFunctionType.Action, "DoNothing"));

        var map = Map(Tree(new[] { noAction, unitEvent, variableEvent, alive }, new[]
        {
            Variable("myUnit", "unit"),
            Variable("flag", "boolean"),
        }));

        var r = TriggerDeadCommand.Dead(map);

        var byName = r.Dead.ToDictionary(d => d.Name);
        Assert.Contains(byName["NoAction"].Reasons, x => x.Kind == TriggerDeadCommand.ReasonNoActions);
        Assert.Contains(byName["UnitEvent"].Reasons, x => x.Kind == TriggerDeadCommand.ReasonUnitWithoutUnit);
        Assert.Contains(byName["VariableEvent"].Reasons, x => x.Kind == TriggerDeadCommand.ReasonNeverSetVariable);
        Assert.DoesNotContain(r.Dead, d => d.Name == "Alive");
    }

    [Fact]
    public void Dead_does_not_flag_a_unit_event_once_the_script_assigns_the_variable()
    {
        var unitEvent = Gui(0x03000001, "UnitEvent");
        unitEvent.Functions.Add(Fn(TriggerFunctionType.Event, "TriggerRegisterUnitEvent",
            Var("udg_myUnit"), Text("EVENT_UNIT_SPELL_EFFECT")));
        unitEvent.Functions.Add(Fn(TriggerFunctionType.Action, "DoNothing"));

        var map = Map(
            Tree(new[] { unitEvent }, new[] { Variable("myUnit", "unit") }),
            script: "function main takes nothing returns nothing\n    set udg_myUnit = CreateUnit(Player(0), 'hfoo', 0, 0, 0)\nendfunction\n");

        var r = TriggerDeadCommand.Dead(map);

        Assert.True(r.ScriptScanned);
        Assert.DoesNotContain(r.Dead, d => d.Name == "UnitEvent");
    }

    // ------------------------------------------------------------------ diff

    [Fact]
    public void Diff_reports_added_removed_renamed_and_changed()
    {
        var keepA = Gui(0x03000001, "Keep");
        keepA.Functions.Add(Fn(TriggerFunctionType.Event, "MapInitializationEvent"));
        keepA.Functions.Add(Fn(TriggerFunctionType.Action, "DoNothing"));

        var renameA = Gui(0x03000002, "RenameMe");
        renameA.Functions.Add(Fn(TriggerFunctionType.Event, "MapInitializationEvent"));
        renameA.Functions.Add(Fn(TriggerFunctionType.Action, "DoNothing"));

        var goneA = Gui(0x03000003, "Gone");
        goneA.Functions.Add(Fn(TriggerFunctionType.Event, "MapInitializationEvent"));

        var keepB = Gui(0x03000001, "Keep");
        keepB.Functions.Add(Fn(TriggerFunctionType.Event, "MapInitializationEvent"));
        keepB.Functions.Add(Fn(TriggerFunctionType.Action, "DoNothing"));
        keepB.Functions.Add(Fn(TriggerFunctionType.Action, "DisplayTextToForce", Preset("GetPlayersAll"), Text("hi")));

        var renamedB = Gui(0x03000002, "Renamed");
        renamedB.Functions.Add(Fn(TriggerFunctionType.Event, "MapInitializationEvent"));
        renamedB.Functions.Add(Fn(TriggerFunctionType.Action, "DoNothing"));

        var addedB = Gui(0x03000009, "Added");
        addedB.Functions.Add(Fn(TriggerFunctionType.Event, "MapInitializationEvent"));

        var mapA = Map(Tree(new[] { keepA, renameA, goneA }));
        var mapB = Map(Tree(new[] { keepB, renamedB, addedB }));

        var r = TriggerDiffCommand.Compare(mapA, mapB);

        Assert.Contains("Added", r.Added);
        Assert.Contains("Gone", r.Removed);
        var rename = Assert.Single(r.Renamed);
        Assert.Equal(0x03000002, rename.Id);
        Assert.Equal("RenameMe", rename.From);
        Assert.Equal("Renamed", rename.To);

        var changed = Assert.Single(r.Changed);
        Assert.Equal("Keep", changed.Name);
        var delta = Assert.Single(changed.Changes);
        Assert.Equal("Action", delta.Kind);
        Assert.Equal("added", delta.Change);
        Assert.Contains("DisplayTextToForce", delta.Description);
    }

    [Fact]
    public void Diff_of_a_map_against_itself_is_empty()
    {
        var trig = Gui(0x03000001, "Same");
        trig.Functions.Add(Fn(TriggerFunctionType.Event, "MapInitializationEvent"));
        trig.Functions.Add(Fn(TriggerFunctionType.Action, "DoNothing"));
        var map = Map(Tree(new[] { trig }));

        var r = TriggerDiffCommand.Compare(map, map);

        Assert.Empty(r.Added);
        Assert.Empty(r.Removed);
        Assert.Empty(r.Renamed);
        Assert.Empty(r.Changed);
    }
}
