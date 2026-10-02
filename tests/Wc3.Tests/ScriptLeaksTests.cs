// tests/Wc3.Tests/ScriptLeaksTests.cs
using Wc3.Commands;

namespace Wc3.Tests;

/// <summary>
/// script leaks, every rule watched firing on the shape it names and silent on the shape that
/// cleans up, plus the heat that ranks a leak by how often its code runs.
/// </summary>
public class ScriptLeaksTests
{
    private static ScriptLeaksResult Run(string body, string extra = "") =>
        ScriptLeaksCommand.Analyze(
            "function Tick takes nothing returns nothing\n" + body + "endfunction\n"
            + extra
            + "function main takes nothing returns nothing\n"
            + "    call TimerStart(CreateTimer(), 0.03, true, function Tick)\n"
            + "endfunction\n");

    [Fact]
    public void A_creator_called_as_a_statement_is_discarded()
    {
        var l = Assert.Single(Run("    call AddSpecialEffect(\"x.mdl\", 0, 0)\n").Leaks);
        Assert.Equal(("discarded", "effect", "hot"), (l.Rule, l.Handle, l.Heat));
        Assert.Empty(Run("    call DestroyEffect(AddSpecialEffect(\"x.mdl\", 0, 0))\n").Leaks);
    }

    [Fact]
    public void An_inline_location_is_reported_unless_its_destroyer_receives_it()
    {
        var l = Assert.Single(Run("    call SetUnitPositionLoc(u, GetUnitLoc(v))\n").Leaks);
        Assert.Equal(("inline", "location"), (l.Rule, l.Handle));
        Assert.Empty(Run("    call RemoveLocation(GetUnitLoc(v))\n").Leaks);
        Assert.Empty(Run("    call SaveLocationHandle(ht, 0, 1, GetUnitLoc(v))\n").Leaks);
    }

    [Fact]
    public void ForGroupBJ_after_wantDestroyGroup_is_not_a_leak()
    {
        Assert.Single(Run("    call ForGroupBJ(GetUnitsInRectAll(r), function Tick)\n").Leaks);
        Assert.Empty(Run("    set bj_wantDestroyGroup = true\n    call ForGroupBJ(GetUnitsInRectAll(r), function Tick)\n").Leaks);
    }

    [Fact]
    public void A_local_is_reported_only_when_nothing_destroys_returns_or_stores_it()
    {
        var l = Assert.Single(Run("    local group g = CreateGroup()\n    call GroupEnumUnitsInRange(g, 0, 0, 500, null)\n").Leaks);
        Assert.Equal(("never-destroyed", "group"), (l.Rule, l.Handle));
        Assert.Empty(Run("    local group g = CreateGroup()\n    call DestroyGroup(g)\n").Leaks);
        Assert.Empty(Run("    local group g = CreateGroup()\n    set udg_G = g\n").Leaks);
        Assert.Empty(ScriptLeaksCommand.Analyze(
            "function Make takes nothing returns location\n    local location l = Location(0, 0)\n    return l\nendfunction\n").Leaks);
        Assert.Empty(Run("    local timer t = CreateTimer()\n    call TimerStart(t, 1, false, function Tick)\n").Leaks);
        // Handed to a function of the map, which might destroy it, so the rule stays silent.
        Assert.Empty(Run("    local group g = CreateGroup()\n    call Cleanup(g)\n",
            "function Cleanup takes group g returns nothing\n    call DestroyGroup(g)\nendfunction\n").Leaks);
    }

    [Fact]
    public void Heat_ranks_periodic_code_above_callbacks_and_init()
    {
        var r = ScriptLeaksCommand.Analyze(
            "function Hot takes nothing returns nothing\n    call AddSpecialEffect(\"a.mdl\", 0, 0)\nendfunction\n"
            + "function OnEvent takes nothing returns nothing\n    call AddSpecialEffect(\"b.mdl\", 0, 0)\nendfunction\n"
            + "function Setup takes nothing returns nothing\n    call AddSpecialEffect(\"c.mdl\", 0, 0)\nendfunction\n"
            + "function main takes nothing returns nothing\n"
            + "    call TimerStart(CreateTimer(), 0.03, true, function Hot)\n"
            + "    call TriggerAddAction(CreateTrigger(), function OnEvent)\n"
            + "    call Setup()\n"
            + "endfunction\n");
        Assert.Equal(new[] { "hot", "repeat", "once" }, r.Leaks.Select(l => l.Heat));
        var p = Assert.Single(r.Periodic);
        Assert.Equal(("Hot", 0.03), (p.Function, p.Period));
    }

    [Fact]
    public void A_creator_inside_a_string_or_comment_is_not_code()
    {
        Assert.Empty(Run("    call BJDebugMsg(\"GetUnitLoc(u) leaks\") // call AddSpecialEffect(x)\n").Leaks);
    }
}
