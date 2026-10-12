// tests/Wc3.Tests/ScriptReachableTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// script reachable, watched on a script with a clean closure from main and an InitTrig, and a
/// dead island that nothing roots at.
/// </summary>
public class ScriptReachableTests
{
    private static ScriptReachableResult Run(string jass) =>
        ScriptReachableCommand.Execute(MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = Encoding.UTF8.GetBytes(jass),
        })));

    private static readonly string Script = string.Join("\n",
        "function main takes nothing returns nothing",   // 1
        "    call HelperA()",                            // 2
        "endfunction",                                   // 3
        "function HelperA takes nothing returns nothing",// 4
        "endfunction",                                   // 5
        "function InitTrig_Test takes nothing returns nothing", // 6
        "    call HelperB()",                            // 7
        "endfunction",                                   // 8
        "function HelperB takes nothing returns nothing",// 9
        "endfunction",                                   // 10
        "function Orphan takes nothing returns nothing", // 11
        "    call OrphanChild()",                        // 12
        "endfunction",                                   // 13
        "function OrphanChild takes nothing returns nothing", // 14
        "endfunction");                                  // 15

    [Fact]
    public void Reachable_from_main_and_IniTrig_are_listed_and_the_island_is_dead()
    {
        var r = Run(Script);

        Assert.Equal("war3map.j", r.ScriptFile);
        Assert.Equal("jass", r.Language);
        Assert.Equal(4, r.Reachable);
        Assert.Equal(2, r.Dead);

        Assert.Equal(new[] { "main", "HelperA", "InitTrig_Test", "HelperB" },
            r.ReachableFunctions.Select(f => f.Name).ToArray());
        Assert.Equal(new[] { "Orphan", "OrphanChild" },
            r.DeadFunctions.Select(f => f.Name).ToArray());
        Assert.Equal(11, r.DeadFunctions[0].Line);
    }

    [Fact]
    public void A_function_only_reached_by_a_string_literal_is_dead_here()
    {
        // The pass roots only at main and InitTrig*, so a string-dispatched name is dead here.
        // script roots is the tool that finds these; this test pins that the two differ.
        var r = Run(string.Join("\n",
            "function main takes nothing returns nothing",
            "    call ExecuteFunc(\"Hidden\")",
            "endfunction",
            "function Hidden takes nothing returns nothing",
            "endfunction"));

        Assert.Contains(r.DeadFunctions, f => f.Name == "Hidden");
        Assert.Equal(1, r.Dead);
    }
}
