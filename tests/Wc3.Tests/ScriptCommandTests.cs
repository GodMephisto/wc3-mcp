// tests/Wc3.Tests/ScriptCommandTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;
namespace Wc3.Tests;

public class ScriptCommandTests
{
    private static MapDocument DocWith(string fileName, string source) =>
        MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [fileName] = Encoding.UTF8.GetBytes(source),
        }));

    [Fact]
    public void Functions_lists_jass_functions_sorted_by_start_line()
    {
        var jass = string.Join("\n",
            "function InitTrig takes nothing returns nothing",
            "    call DoNothing()",
            "endfunction",
            "function main takes nothing returns nothing",
            "endfunction");
        var r = ScriptCommand.Functions(DocWith("war3map.j", jass));

        Assert.Equal("war3map.j", r.ScriptFile);
        Assert.Equal(2, r.Functions.Count);
        Assert.Equal("InitTrig", r.Functions[0].Name);
        Assert.Equal("main", r.Functions[1].Name);
        Assert.True(r.Functions[0].StartLine < r.Functions[1].StartLine);
    }

    [Fact]
    public void Functions_falls_back_to_lua_and_names_the_file()
    {
        var r = ScriptCommand.Functions(DocWith("war3map.lua", "local x = 1\nfunction main()\nend\n"));
        Assert.Equal("war3map.lua", r.ScriptFile);
        // Lua goes through the JASS scanner best-effort; no JASS-shaped functions here.
        Assert.Empty(r.Functions);
    }

    [Fact]
    public void Functions_throws_when_map_has_no_script()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = new byte[] { 1 },
        }));
        Assert.Throws<FileNotFoundException>(() => ScriptCommand.Functions(doc));
    }
}
