// tests/Wc3.Tests/McpToolSmokeTests.Triggers.cs
// The trigger tools. A blank map has no war3map.wtg, so the fixture writes one standard
// World-Editor trigger into the script and recovers the trigger tree from it, all through MCP,
// which also exercises trigger_recover_from_script on a script it can actually read.
using System.Text.Json;

namespace Wc3.Tests;

public sealed partial class McpToolSmokeTests
{
    private const string HelloTrigger = """
        function Trig_Hello_Actions takes nothing returns nothing
            call DisplayTextToForce( GetPlayersAll(), "hello" )
        endfunction

        function InitTrig_Hello takes nothing returns nothing
            set gg_trg_Hello = CreateTrigger(  )
            call TriggerRegisterTimerEventSingle( gg_trg_Hello, 1.00 )
            call TriggerAddAction( gg_trg_Hello, function Trig_Hello_Actions )
        endfunction

        function InitCustomTriggers takes nothing returns nothing
            call InitTrig_Hello(  )
        endfunction
        """;

    /// <summary>A map whose war3map.wtg holds one GUI trigger, Hello, with one event and one action.</summary>
    private async Task<(string Map, int CategoryId, int TriggerId)> TriggerMap()
    {
        var script = Prop(await Call("file_get_text", Args(("map", Fixture), ("internal_path", "war3map.j"))), "text").GetString()!
            .Replace("\r\n", "\n");
        script = script.Replace("globals\nendglobals", "globals\n    trigger gg_trg_Hello = null\nendglobals");
        script = script.Replace("function InitCustomTriggers takes nothing returns nothing\nendfunction", HelloTrigger.Replace("\r\n", "\n"));
        Assert.Contains("InitTrig_Hello", script);
        var scriptFile = NewOut("hello", ".j");
        File.WriteAllText(scriptFile, script, Wc3.Model.ScriptText.Encoding);

        var withScript = await Write("file_set", Args(("internal_path", "war3map.j"), ("from", scriptFile)));
        var recovered = await Write("trigger_recover_from_script", Args(), withScript);
        var model = await Call("triggers_read", Args(("map", recovered)));
        var trigger = Assert.Single(Rows(Prop(model, "triggers")));
        return (recovered, Prop(trigger, "parentCategoryId").GetInt32(), Prop(trigger, "id").GetInt32());
    }

    private static JsonElement Trigger(JsonElement model, int id) =>
        Rows(Prop(model, "triggers")).Single(t => Prop(t, "id").GetInt32() == id);

    [Fact, Covers("trigger_recover_from_script", "triggers_read")]
    public async Task Recovering_from_script_gives_the_trigger_its_event_and_action()
    {
        var (map, _, id) = await TriggerMap();
        var hello = Trigger(await Call("triggers_read", Args(("map", map))), id);
        Assert.Equal("Hello", Prop(hello, "name").GetString());
        var functions = Rows(Prop(hello, "functions"));
        Assert.Contains(functions, f => Prop(f, "kind").GetString() == "Event" && Prop(f, "name").GetString() == "TriggerRegisterTimerEventSingle");
        Assert.Contains(functions, f => Prop(f, "kind").GetString() == "Action" && Prop(f, "name").GetString() == "DisplayTextToForce");

        // A script with no World-Editor triggers in it gives nothing to recover, and says so.
        var refusal = await CallError("trigger_recover_from_script", Args(("map", Fixture), ("out_path", NewOut("none"))));
        Assert.Contains("empty", refusal);
    }

    [Fact, Covers("trigger_add_category", "trigger_add")]
    public async Task Adding_a_category_and_a_trigger_in_it()
    {
        var (map, _, _) = await TriggerMap();
        var withCat = await Write("trigger_add_category", Args(("name", "Setup")), map);
        var cat = Rows(Prop(await Call("triggers_read", Args(("map", withCat))), "categories"))
            .Single(c => Prop(c, "name").GetString() == "Setup");
        int catId = Prop(cat, "id").GetInt32();

        var withTrig = await Write("trigger_add", Args(("name", "Spawn Wave"), ("parent_id", catId)), withCat);
        var added = Rows(Prop(await Call("triggers_read", Args(("map", withTrig))), "triggers"))
            .Single(t => Prop(t, "name").GetString() == "Spawn Wave");
        Assert.Equal(catId, Prop(added, "parentCategoryId").GetInt32());
    }

    [Fact, Covers("trigger_rename", "trigger_set_enabled", "trigger_set_initially_on", "trigger_set_run_on_map_init")]
    public async Task Rename_and_flags_reach_the_trigger()
    {
        var (map, _, id) = await TriggerMap();
        var m1 = await Write("trigger_rename", Args(("id", id), ("name", "Greeting")), map);
        var m2 = await Write("trigger_set_enabled", Args(("id", id), ("on", false)), m1);
        var m3 = await Write("trigger_set_initially_on", Args(("id", id), ("on", false)), m2);
        var m4 = await Write("trigger_set_run_on_map_init", Args(("id", id), ("on", true)), m3);

        var t = Trigger(await Call("triggers_read", Args(("map", m4))), id);
        Assert.Equal("Greeting", Prop(t, "name").GetString());
        Assert.False(Prop(t, "enabled").GetBoolean());
        Assert.False(Prop(t, "initiallyOn").GetBoolean());
    }

    [Fact, Covers("trigger_add_eca", "trigger_set_eca_enabled", "trigger_remove_eca")]
    public async Task Events_conditions_and_actions_are_added_toggled_and_removed()
    {
        var (map, _, id) = await TriggerMap();
        int before = Rows(Prop(Trigger(await Call("triggers_read", Args(("map", map))), id), "functions")).Count;

        var added = await Write("trigger_add_eca", Args(("id", id), ("kind", "action"), ("name", "DisplayTextToForce")), map);
        var functions = Rows(Prop(Trigger(await Call("triggers_read", Args(("map", added))), id), "functions"));
        Assert.Equal(before + 1, functions.Count);

        var off = await Write("trigger_set_eca_enabled", Args(("id", id), ("index", 0), ("on", false)), added);
        Assert.False(Prop(Rows(Prop(Trigger(await Call("triggers_read", Args(("map", off))), id), "functions"))[0], "enabled").GetBoolean());

        var removed = await Write("trigger_remove_eca", Args(("id", id), ("index", 0)), added);
        Assert.Equal(before, Rows(Prop(Trigger(await Call("triggers_read", Args(("map", removed))), id), "functions")).Count);

        var unknown = await CallError("trigger_add_eca", Args(("map", map), ("out_path", NewOut("bad")), ("id", id), ("kind", "action"), ("name", "NoSuchFunction")));
        Assert.Contains("NoSuchFunction", unknown);
    }

    [Fact, Covers("trigger_remove")]
    public async Task Removing_a_category_refuses_to_orphan_unless_recursive()
    {
        var (map, catId, id) = await TriggerMap();
        await CallError("trigger_remove", Args(("map", map), ("out_path", NewOut("orphan")), ("id", catId)));

        var gone = await Write("trigger_remove", Args(("id", catId), ("recursive", true)), map);
        var model = await Call("triggers_read", Args(("map", gone)));
        Assert.DoesNotContain(Rows(Prop(model, "triggers")), t => Prop(t, "id").GetInt32() == id);
        Assert.DoesNotContain(Rows(Prop(model, "categories")), c => Prop(c, "id").GetInt32() == catId);
    }
}
