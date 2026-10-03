// tests/Wc3.Tests/ScriptPathAliasLuaTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// Settles whether a map whose Lua script is stored under scripts\ can be read.
///
/// An audit reported that ScriptCommand.ScriptEntry probes war3map.j, scripts\war3map.j and
/// war3map.lua but never scripts\war3map.lua, and concluded the Script panel would report "No
/// script in map" for a map that has one. The reasoning is sound from that method alone, and the
/// conclusion is wrong, because MapDocument.GetFile falls back to StandardMapFileNames.AliasesFor
/// and the alias table pairs war3map.lua with scripts\war3map.lua in both directions.
///
/// Reading two files and reasoning about them is how that gets missed, so this asserts the
/// behaviour instead of the code shape. All four spellings must be readable, and the pair of
/// probes for the nested spellings in ScriptEntry are redundant rather than load-bearing.
/// </summary>
public class ScriptPathAliasLuaTests
{
    private const string Lua = "function main()\n  print(\"hello\")\nend\n";
    private const string Jass = "function main takes nothing returns nothing\nendfunction\n";

    /// <summary>An archive holding exactly one script, under exactly one spelling. Built from
    /// scratch rather than from BlankMap, which ships a war3map.j that would answer every case
    /// and make all four spellings look like they work.</summary>
    private static MapDocument MapWith(string internalPath, string text) =>
        MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [internalPath] = Encoding.Latin1.GetBytes(text),
        }));

    [Theory]
    [InlineData("war3map.j")]
    [InlineData(@"scripts\war3map.j")]
    [InlineData("war3map.lua")]
    [InlineData(@"scripts\war3map.lua")]
    public void A_script_is_found_under_every_spelling_a_real_map_uses(string internalPath)
    {
        bool lua = internalPath.EndsWith(".lua", StringComparison.OrdinalIgnoreCase);
        var doc = MapWith(internalPath, lua ? Lua : Jass);

        var (file, source) = ScriptCommand.Read(doc);
        Assert.False(string.IsNullOrEmpty(source),
            $"a map whose only script is {internalPath} reported no script");
        Assert.Contains(lua ? "print" : "endfunction", source, StringComparison.Ordinal);
        Assert.NotNull(file);
    }
}
