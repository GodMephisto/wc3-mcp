// tests/Wc3.Tests/ScriptGlobalsTests.cs
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// GUI variables and script globals are two different populations, and a panel that reads
/// only the first reports nothing on a map whose variables all live in the second.
///
/// Measured on Anime_WOS2_0.29d, war3map.wtg declares ZERO variables and the script's globals
/// block declares 12,543. The trigger panel omitted its Variables node entirely at zero, so a
/// map with twelve thousand variables showed no variables section at all.
/// </summary>
public class ScriptGlobalsTests
{
    private readonly ITestOutputHelper _out;
    public ScriptGlobalsTests(ITestOutputHelper output) => _out = output;

    private static MapDocument WithScript(string jass)
    {
        var doc = BlankMap.Create();
        doc.AddOrReplaceRawFile("war3map.j", ScriptText.GetBytes(jass));
        return doc;
    }

    [Fact]
    public void Declarations_are_read_with_their_type_array_and_constant_flags()
    {
        var doc = WithScript(
            "globals\n"
            + "    integer plain= 0\n"
            + "    constant string SAVE_FOLDER= \"save\\\\\"\n"
            + "    unit array Heroes\n"
            + "    real noInit\n"
            + "endglobals\n"
            + "function Foo takes nothing returns nothing\n"
            + "endfunction\n");

        var r = ScriptGlobalsCommand.List(doc);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(4, r.Globals.Count);

        var plain = r.Globals.Single(g => g.Name == "plain");
        Assert.Equal("integer", plain.Type);
        Assert.False(plain.IsArray);
        Assert.False(plain.IsConstant);
        Assert.Equal("0", plain.InitialValue);

        var folder = r.Globals.Single(g => g.Name == "SAVE_FOLDER");
        Assert.True(folder.IsConstant);
        Assert.Equal("string", folder.Type);

        var heroes = r.Globals.Single(g => g.Name == "Heroes");
        Assert.True(heroes.IsArray);
        Assert.Equal("unit", heroes.Type);

        Assert.Null(r.Globals.Single(g => g.Name == "noInit").InitialValue);
    }

    [Fact]
    public void Nothing_outside_the_globals_block_is_counted()
    {
        // A function body full of local declarations must not leak into the count, and the
        // block must end at endglobals rather than running to the end of the file.
        var doc = WithScript(
            "globals\n"
            + "    integer one= 1\n"
            + "endglobals\n"
            + "function Foo takes nothing returns nothing\n"
            + "    local integer notAGlobal= 2\n"
            + "    local unit array alsoNot\n"
            + "endfunction\n");

        var r = ScriptGlobalsCommand.List(doc);
        Assert.Single(r.Globals);
        Assert.Equal("one", r.Globals[0].Name);
    }

    [Fact]
    public void A_script_with_no_globals_block_is_reported_rather_than_failing()
    {
        var doc = WithScript("function Foo takes nothing returns nothing\nendfunction\n");
        var r = ScriptGlobalsCommand.List(doc);
        Assert.True(r.Ok);
        Assert.Empty(r.Globals);
        Assert.Contains("no globals block", r.Message);
    }

    [Fact]
    public void A_synthesised_blank_map_reports_no_globals_without_failing()
    {
        // A blank map DOES carry a war3map.j, so this is not the "no script" path. It is the
        // ordinary case of a script that declares nothing, and it must come back Ok with an
        // empty list rather than as an error.
        var r = ScriptGlobalsCommand.List(BlankMap.Create());
        Assert.True(r.Ok, r.Message);
        Assert.Empty(r.Globals);
        Assert.Equal(0, ScriptGlobalsCommand.Count(BlankMap.Create()));
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void The_reported_symptom_reproduces_and_the_two_populations_differ()
    {
        string path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Warcraft III", "Maps", "Download", "Anime_WOS2_0.29d.w3x");
        if (!File.Exists(path)) { _out.WriteLine("map not on disk"); return; }

        var doc = MapDocument.Load(path);
        var model = TriggerReadCommand.GetTriggers(doc);
        var globals = ScriptGlobalsCommand.List(doc);

        _out.WriteLine($"GUI variables in the trigger tree: {model.Variables.Count}");
        _out.WriteLine($"globals declared in the script:    {globals.Globals.Count:N0}");

        Assert.Empty(model.Variables);
        Assert.True(globals.Globals.Count > 1000,
            $"expected thousands of script globals, found {globals.Globals.Count}");
    }
}
