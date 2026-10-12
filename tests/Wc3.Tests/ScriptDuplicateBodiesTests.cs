// tests/Wc3.Tests/ScriptDuplicateBodiesTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// script duplicate-bodies, where two functions share a body up to a local rename and a third does not.
/// </summary>
public class ScriptDuplicateBodiesTests
{
    private static ScriptDuplicateBodiesResult Run(string jass) =>
        ScriptDuplicateBodiesCommand.Execute(MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = Encoding.UTF8.GetBytes(jass),
        })));

    [Fact]
    public void Two_functions_with_the_same_body_are_grouped_and_a_different_one_is_not()
    {
        var r = Run(string.Join("\n",
            "function One takes nothing returns nothing",    // 1
            "    local integer i = 0",                       // 2
            "    call BJDebugMsg(\"x\")",                    // 3
            "endfunction",                                   // 4
            "function Two takes nothing returns nothing",    // 5
            "    local integer j = 0",                       // 6 (renamed i to j)
            "    call BJDebugMsg(\"x\")",                    // 7
            "endfunction",                                   // 8
            "function Three takes nothing returns nothing",  // 9
            "    call BJDebugMsg(\"y\")",                    // 10
            "endfunction"));                                 // 11

        Assert.Equal(3, r.Functions);
        Assert.Equal(1, r.DuplicateGroups);
        Assert.Equal(2, r.DuplicateFunctions);

        var group = Assert.Single(r.Groups);
        Assert.Equal(2, group.Size);
        Assert.Equal(new[] { "One", "Two" }, group.Functions.Select(f => f.Name).ToArray());
    }

    [Fact]
    public void A_script_with_no_repeated_body_has_no_groups()
    {
        var r = Run("function main takes nothing returns nothing\nendfunction\n"
            + "function Helper takes nothing returns nothing\n    call main()\nendfunction\n");
        Assert.Equal(0, r.DuplicateGroups);
        Assert.Empty(r.Groups);
    }
}
