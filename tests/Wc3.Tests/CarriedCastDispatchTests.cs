// tests/Wc3.Tests/CarriedCastDispatchTests.cs
using Wc3.Commands;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// Export carries a hero's cast dispatcher and cannot carry the TRIGGER that drove it, because a
/// trigger is a runtime handle rather than a function in the closure. Measured on GGGA: the
/// installed hero's dispatcher was declared, referenced by nothing, and every one of her
/// spell-start functions was unreachable, so she spawned correctly and her abilities did nothing.
/// </summary>
public class CarriedCastDispatchTests
{
    private const string Head = "function main takes nothing returns nothing\r\n"
                                + "    call InitGlobals()\r\n"
                                + "endfunction\r\n";

    private static string Dispatcher(string name, string gate) =>
        $"function {name} takes nothing returns boolean\n"
        + $"    if GetUnitTypeId(GetSpellAbilityUnit()) == {gate} then\n"
        + "        if GetSpellAbilityId() == 'A00c' then\n"
        + "            call DarkShikiQ_Start(GetSpellAbilityUnit())\n"
        + "        endif\n"
        + "    endif\n"
        + "    return false\n"
        + "endfunction\n";

    [Fact]
    public void WiresADispatcherNothingDrives()
    {
        var body = Dispatcher("CarriedDispatch", "'H003'");

        var w = Assert.Single(CarriedCastDispatch.Wire(Head, body, "H003"));

        Assert.Equal("CarriedDispatch", w.ConditionFunction);
        Assert.Contains("TriggerRegisterAnyUnitEventBJ(t, EVENT_PLAYER_UNIT_SPELL_EFFECT)", w.GeneratedScript);
        Assert.Contains("return CarriedDispatch()", w.GeneratedScript);
        Assert.Contains("call ExecuteFunc(", w.InitCall);
    }

    /// <summary>
    /// The gate is the whole safety argument. A carried dispatcher branches on the SOURCE map's
    /// other heroes too, and those rawcodes can be real units in the target, so registering it
    /// ungated would make the target's own hero run carried spell code.
    /// </summary>
    [Fact]
    public void GatesTheDispatcherToTheInstalledHero()
    {
        var body = Dispatcher("CarriedDispatch", "'H003'");

        var w = Assert.Single(CarriedCastDispatch.Wire(Head, body, "H003"));

        Assert.Contains("if GetUnitTypeId(GetSpellAbilityUnit()) != 'H003' then", w.GeneratedScript);
        Assert.Contains("return false", w.GeneratedScript);
    }

    /// <summary>A carried dispatcher compares against a global, not a literal, in the real case.</summary>
    [Fact]
    public void ResolvesTheHeroThroughACarriedRawcodeGlobal()
    {
        var body = "integer DarkShiki_ID= 'H003'\n" + Dispatcher("CarriedDispatch", "DarkShiki_ID");

        Assert.Single(CarriedCastDispatch.Wire(Head, body, "H003"));
    }

    /// <summary>
    /// A definition declares its rawcode globals in hero.json, and install writes those into the
    /// TARGET's globals block, not into the carried body. Looking for the alias only in the body
    /// found nothing and skipped the real dispatcher on the first run against GGGA.
    /// </summary>
    [Fact]
    public void ResolvesTheHeroThroughAGlobalInstallPutInTheTargetScript()
    {
        var head = "globals\r\ninteger DarkShiki_ID= 'H003'\r\nendglobals\r\n" + Head;

        Assert.Single(CarriedCastDispatch.Wire(head, Dispatcher("CarriedDispatch", "DarkShiki_ID"), "H003"));
    }

    [Fact]
    public void LeavesADispatcherSomethingAlreadyDrives()
    {
        var body = Dispatcher("CarriedDispatch", "'H003'")
                   + "function InitIt takes nothing returns nothing\n"
                   + "    call TriggerAddCondition(t, Condition(function CarriedDispatch))\n"
                   + "endfunction\n";

        Assert.Empty(CarriedCastDispatch.Wire(Head, body, "H003"));
    }

    [Fact]
    public void LeavesADispatcherTheTargetAlreadyNames()
    {
        var head = Head + "function Hook takes nothing returns nothing\r\n"
                        + "    call TriggerAddCondition(t, Condition(function CarriedDispatch))\r\n"
                        + "endfunction\r\n";

        Assert.Empty(CarriedCastDispatch.Wire(head, Dispatcher("CarriedDispatch", "'H003'"), "H003"));
    }

    [Fact]
    public void IgnoresBooleanFunctionsThatAreNotCastDispatchers()
    {
        var body = "function SomeFilter takes nothing returns boolean\n"
                   + "    return GetUnitTypeId(GetFilterUnit()) == 'H003'\n"
                   + "endfunction\n";

        Assert.Empty(CarriedCastDispatch.Wire(Head, body, "H003"));
    }

    [Fact]
    public void IgnoresADispatcherThatNeverMentionsThisHero()
    {
        Assert.Empty(CarriedCastDispatch.Wire(Head, Dispatcher("CarriedDispatch", "'H099'"), "H003"));
    }

    /// <summary>
    /// The carried body is appended AFTER main, so a direct call would not resolve in vanilla JASS.
    /// The call goes inside main, never after its endfunction, which would be file scope.
    /// </summary>
    [Fact]
    public void PutsTheInitCallInsideMain()
    {
        var result = CarriedCastDispatch.InsertIntoMain(Head, new[] { "    call ExecuteFunc(\"W\")" });

        var lines = result.Split('\n').Select(l => l.TrimEnd('\r')).ToArray();
        int call = Array.IndexOf(lines, "    call ExecuteFunc(\"W\")");
        int end = Array.IndexOf(lines, "endfunction");
        Assert.True(call > 0);
        Assert.True(call < end);
    }

    [Fact]
    public void AScriptWithNoMainIsLeftAlone()
    {
        const string head = "function other takes nothing returns nothing\r\nendfunction\r\n";

        Assert.Equal(head, CarriedCastDispatch.InsertIntoMain(head, new[] { "    call ExecuteFunc(\"W\")" }));
    }
}
