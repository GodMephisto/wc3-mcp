// tests/Wc3.Tests/PreloadRepairTests.cs
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// repair preload finds what a map loads in its first seconds and moves it under the loading
/// screen. Each case checks the side it must reach and the side it must leave alone.
/// </summary>
public class PreloadRepairTests
{
    // H000 and A000 are defined by the map and reached from the 1 second trigger, through an
    // integer global and a literal. H001 and A001 are defined too but only a 30 second trigger
    // reaches them, so they are not early.
    private const string Script =
        "globals\n"
        + "integer Hero_ID= 'H000'\n"
        + "trigger gg_trg_Early= null\n"
        + "trigger gg_trg_Late= null\n"
        + "endglobals\n"
        + "function Helper takes nothing returns nothing\n"
        + "    local unit u= CreateUnit(Player(12), Hero_ID, 0, 0, 0) // 'H001' in a comment is not a use\n"
        + "    call UnitAddAbility(u, 'A000')\n"
        + "endfunction\n"
        + "function Early takes nothing returns nothing\n"
        + "    call Helper()\n"
        + "endfunction\n"
        + "function Late takes nothing returns nothing\n"
        + "    call UnitAddAbility(CreateUnit(Player(12), 'H001', 0, 0, 0), 'A001')\n"
        + "endfunction\n"
        + "function InitTrig takes nothing returns nothing\n"
        + "    set gg_trg_Early=CreateTrigger()\n"
        + "    call TriggerRegisterTimerEventSingle(gg_trg_Early, 1)\n"
        + "    call TriggerAddAction(gg_trg_Early, function Early)\n"
        + "    set gg_trg_Late=CreateTrigger()\n"
        + "    call TriggerRegisterTimerEventSingle(gg_trg_Late, 30.00)\n"
        + "    call TriggerAddAction(gg_trg_Late, function Late)\n"
        + "endfunction\n"
        + "function main takes nothing returns nothing\n"
        + "    local integer i= 0\n"
        + "    call SetCameraBounds(0, 0, 0, 0, 0, 0, 0, 0)\n"
        + "    call InitBlizzard()\n"
        + "    call InitTrig()\n"
        + "endfunction\n";

    private static MapDocument Map(string script = Script)
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        foreach (var id in new[] { "H000", "H001" })
            w3u.NewUnits.Add(new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = id.FromRawcode() });
        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        foreach (var id in new[] { "A000", "A001" })
            w3a.NewAbilities.Add(new LevelObjectModification { OldId = "AHbz".FromRawcode(), NewId = id.FromRawcode() });
        return MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Serialize(w => w.Write(w3u)),
            ["war3map.w3a"] = Serialize(w => w.Write(w3a)),
            ["war3map.j"] = Encoding.UTF8.GetBytes(script),
        }));
    }

    private static byte[] Serialize(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }

    private static string ScriptOf(MapDocument doc)
    {
        Assert.True(doc.TryReadFileByName("war3map.j", out var bytes));
        return Encoding.Latin1.GetString(bytes);
    }

    [Fact]
    public void Collects_what_an_early_timer_reaches_and_nothing_a_late_one_does()
    {
        var r = PreloadRepairCommand.Execute(Map(), apply: false);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(new[] { "H000" }, r.UnitTypes);
        Assert.Equal(new[] { "A000" }, r.Abilities);
        Assert.Equal("Early", Assert.Single(r.Entries).Function);
    }

    [Fact]
    public void A_wider_window_or_a_named_function_reaches_further()
    {
        Assert.Contains("H001", PreloadRepairCommand.Execute(Map(), false, withinSeconds: 60).UnitTypes);
        Assert.Contains("A001", PreloadRepairCommand.Execute(Map(), false, functions: new[] { "Late" }).Abilities);
    }

    [Fact]
    public void The_preload_runs_before_InitBlizzard_and_the_map_code_is_untouched()
    {
        var doc = Map();
        var r = PreloadRepairCommand.Execute(doc, apply: true);
        Assert.True(r.Ok, r.Message);
        var lines = ScriptOf(doc).Split('\n').ToList();
        int call = lines.FindIndex(l => l.Contains("ExecuteFunc(\"wc3ctl_preload_1\")"));
        int blizz = lines.FindIndex(l => l.Trim() == "call InitBlizzard()");
        int local = lines.FindIndex(l => l.Trim() == "local integer i= 0");
        Assert.True(local < call && call < blizz, $"local {local}, call {call}, InitBlizzard {blizz}");
        Assert.Contains(lines, l => l.Contains("CreateUnit(Player(PLAYER_NEUTRAL_PASSIVE), 'H000'"));
        Assert.Contains(lines, l => l.Contains("call UnitAddAbility(u, 'A000')"));
        // Every original line is still there, in order.
        var original = Script.Split('\n');
        int at = 0;
        foreach (var l in lines) if (at < original.Length && l == original[at]) at++;
        Assert.Equal(original.Length, at);
    }

    [Fact]
    public void A_second_run_is_refused_and_a_dry_run_writes_nothing()
    {
        var doc = Map();
        PreloadRepairCommand.Execute(doc, apply: false);
        Assert.Equal(Script, ScriptOf(doc));
        PreloadRepairCommand.Execute(doc, apply: true);
        Assert.False(PreloadRepairCommand.Execute(doc, apply: true).Ok);
    }

    [Fact]
    public void Keeps_the_scripts_own_line_ending()
    {
        var doc = Map(Script.Replace("\n", "\r\n"));
        PreloadRepairCommand.Execute(doc, apply: true);
        string s = ScriptOf(doc);
        Assert.DoesNotContain("\n", s.Replace("\r\n", ""));
    }
}
