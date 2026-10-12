// tests/Wc3.Tests/ScriptToolkitTests.cs
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Hermetic tests for the five read-only script toolkit commands. Each builds a synthetic map
/// with a hand-written war3map.j and asserts the command reports the shape it names and stays
/// silent on the shape that is clean.
/// </summary>
public class ScriptToolkitTests
{
    // One script carrying every shape the five commands look for. Kept small enough to read.
    private const string Script =
        "globals\n"
        + "    integer udg_NeverRead = 0\n"
        + "    integer udg_ReadOnly = 0\n"
        + "    integer udg_WriteOnly = 0\n"
        + "    trigger gg_trg_Gen = null\n"
        + "    location udg_Kept = null\n"
        + "endglobals\n"
        + "\n"
        + "function UsesGlobal takes nothing returns nothing\n"
        + "    set udg_ReadOnly = GetRandomInt(1, 10)\n"
        + "    call BJDebugMsg(I2S(udg_ReadOnly))\n"
        + "    set udg_WriteOnly = 1\n"
        + "    call RemoveLocation(udg_Kept)\n"
        + "endfunction\n"
        + "\n"
        + "function Called takes nothing returns nothing\n"
        + "endfunction\n"
        + "\n"
        + "function NeverCalled takes nothing returns nothing\n"
        + "endfunction\n"
        + "\n"
        + "function InitTrig_Gen takes nothing returns nothing\n"
        + "    call Called()\n"
        + "endfunction\n"
        + "\n"
        + "function Leaky takes nothing returns nothing\n"
        + "    local group g = CreateGroup()\n"
        + "    local location l = GetRectCenter(GetPlayableMapRect())\n"
        + "    call DestroyGroup(g)\n"
        + "    call RemoveLocation(l)\n"
        + "    set l = null\n"
        + "endfunction\n"
        + "\n"
        + "function Complex takes integer n returns integer\n"
        + "    if n > 0 and n < 10 then\n"
        + "        loop\n"
        + "            set n = n - 1\n"
        + "            exitwhen n <= 0\n"
        + "        endloop\n"
        + "    endif\n"
        + "    return n\n"
        + "endfunction\n"
        + "\n"
        + "function main takes nothing returns nothing\n"
        + "    call UsesGlobal()\n"
        + "    call InitTrig_Gen()\n"
        + "    call Leaky()\n"
        + "    call Complex(3)\n"
        + "endfunction\n"
        + "\n"
        + "function config takes nothing returns nothing\n"
        + "endfunction\n"
        + "\n"
        + "function InitCustomTriggers takes nothing returns nothing\n"
        + "    call InitTrig_Gen()\n"
        + "endfunction\n";

    private static MapDocument Doc() =>
        MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = System.Text.Encoding.Latin1.GetBytes(Script),
        }));

    [Fact]
    public void Unused_globals_reports_write_only_and_never_read_and_skips_framework()
    {
        var r = ScriptUnusedGlobalsCommand.Execute(Doc());
        var names = r.Globals.Select(g => g.Name).ToList();
        Assert.Contains("udg_NeverRead", names);
        Assert.Contains("udg_WriteOnly", names);        // assigned, never read
        Assert.DoesNotContain("udg_ReadOnly", names);   // read back
        Assert.DoesNotContain("udg_Kept", names);       // read as an argument
        Assert.DoesNotContain("gg_trg_Gen", names);     // trigger-generated framework
        Assert.Equal(r.Globals.First(g => g.Name == "udg_NeverRead").Line, 2);
    }

    [Fact]
    public void Unused_functions_reports_only_the_one_nothing_calls()
    {
        var r = ScriptUnusedFunctionsCommand.Execute(Doc());
        var names = r.Functions.Select(f => f.Name).ToList();
        Assert.Contains("NeverCalled", names);
        Assert.DoesNotContain("Called", names);           // called by InitTrig_Gen and main
        Assert.DoesNotContain("UsesGlobal", names);
        Assert.DoesNotContain("main", names);             // entry points excluded
        Assert.DoesNotContain("config", names);
        Assert.DoesNotContain("InitCustomTriggers", names);
    }

    [Fact]
    public void Null_locals_reports_a_destroyed_handle_that_is_never_nulled()
    {
        var r = ScriptNullLocalsCommand.Execute(Doc());
        var f = Assert.Single(r.Locals);
        Assert.Equal(("Leaky", "g", "group"), (f.Function, f.Name, f.Type));
    }

    [Fact]
    public void Complexity_counts_if_loop_and_or_and_ranks_worst_first()
    {
        var r = ScriptComplexityCommand.Execute(Doc());
        var worst = r.Functions[0];
        Assert.Equal("Complex", worst.Function);
        Assert.Equal(3, worst.Complexity);   // if + and + loop
    }

    [Fact]
    public void Callgraph_lists_the_edges_between_declared_functions()
    {
        var r = ScriptCallGraphCommand.Execute(Doc());
        Assert.Contains(r.Calls, e => e.From == "main" && e.To == "Complex");
        Assert.Contains(r.Calls, e => e.From == "InitTrig_Gen" && e.To == "Called");
        Assert.DoesNotContain(r.Calls, e => e.To == "BJDebugMsg");   // natives are not map functions
    }
}
