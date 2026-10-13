// tests/Wc3.Tests/ScriptSemanticDiffTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// script semantic-diff, on two scripts that differ by a pure local rename and one real change.
/// </summary>
public class ScriptSemanticDiffTests
{
    private static MapDocument Doc(string jass) =>
        MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = Encoding.UTF8.GetBytes(jass),
        }));

    [Fact]
    public void A_pure_local_rename_is_ignored_and_a_real_line_is_reported()
    {
        var a = Doc(string.Join("\n",
            "function Foo takes nothing returns nothing",  // 1
            "    local integer i = 0",                     // 2
            "    set i = i + 1",                           // 3
            "    call BJDebugMsg(\"hi\")",                 // 4
            "endfunction",                                 // 5
            "function main takes nothing returns nothing", // 6
            "endfunction"));                               // 7

        var b = Doc(string.Join("\n",
            "function Foo takes nothing returns nothing",  // 1
            "    local integer j = 0",                     // 2 (renamed i to j)
            "    set j = j + 1",                           // 3
            "    call BJDebugMsg(\"bye\")",                // 4 (real change)
            "endfunction",                                 // 5
            "function main takes nothing returns nothing", // 6
            "endfunction"));                               // 7

        var r = ScriptSemanticDiffCommand.Execute(a, b);

        Assert.Equal("jass", r.Language);
        Assert.Equal(1, r.Changes);
        var one = Assert.Single(r.Lines);
        Assert.Equal("changed", one.Kind);
        Assert.Equal(4, one.LineA);
        Assert.Equal(4, one.LineB);
        Assert.Contains("bye", one.Text);
    }

    private static MapDocument LuaDoc(string lua) =>
        MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.lua"] = Encoding.UTF8.GetBytes(lua),
        }));

    [Fact]
    public void A_pure_chunk_level_local_rename_is_ignored()
    {
        // A Lua file-scope local, renamed, must be normalized away exactly as a function-local
        // rename is. It used to read as a real change, because the canonicalizer only knew a
        // function's OWN locals, so the use inside f() looked like a global reference.
        var a = LuaDoc("local cache = {}\nfunction f()\n    return cache\nend\n");
        var b = LuaDoc("local store = {}\nfunction f()\n    return store\nend\n");

        var r = ScriptSemanticDiffCommand.Execute(a, b);

        Assert.Equal("lua", r.Language);
        Assert.Equal(0, r.Changes);
    }

    [Fact]
    public void An_inserted_function_is_reported_whole()
    {
        var a = Doc("function main takes nothing returns nothing\nendfunction\n");
        var b = Doc("function main takes nothing returns nothing\nendfunction\n"
            + "function Extra takes nothing returns nothing\nendfunction\n");

        var r = ScriptSemanticDiffCommand.Execute(a, b);

        var one = Assert.Single(r.Lines);
        Assert.Equal("function-added", one.Kind);
        Assert.Contains("Extra", one.Text);
    }
}
