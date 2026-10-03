// tests/Wc3.Tests/JassLinesTests.cs
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// Line splitting, and why it is not the one-liner it looks like.
///
/// Measured over 34 maps in the user's Maps folder, 13 store their war3map.j with a BARE CARRIAGE
/// RETURN as the line separator, which is what the widely used map optimizer emits. Every one of
/// those 13 reported EXACTLY ZERO functions, because the index split on a newline. Between them
/// they hold about 125,000 function declarations, one of them 17,304 on its own.
///
/// Nothing failed while that was true. The function list was empty, the compile check found no
/// functions and therefore no problems, and lint reported "war3map.j compiles" for a script it had
/// not read a statement of. A green result that means nothing is the worst shape a bug can take.
/// </summary>
public class JassLinesTests
{
    private const string Fn =
        "function A takes nothing returns nothing\ncall BJDebugMsg(\"x\")\nendfunction\n";

    [Theory]
    [InlineData("\n")]
    [InlineData("\r\n")]
    [InlineData("\r")]
    public void All_three_terminators_split_the_same_way(string eol)
    {
        var text = string.Join(eol, "one", "two", "three");
        Assert.Equal(new[] { "one", "two", "three" }, JassLines.Split(text));
    }

    [Fact]
    public void CRLF_is_one_terminator_and_not_two()
    {
        // Counting it as two would put every line number past the first off by the number of
        // lines above it, which is worse than not splitting at all because it looks plausible.
        Assert.Equal(new[] { "a", "b" }, JassLines.Split("a\r\nb"));
        Assert.Equal(2, JassLines.LineStarts("a\r\nb").Length);
    }

    [Fact]
    public void A_mixed_script_splits_on_whichever_terminator_appears()
    {
        // Not hypothetical. Tom_and_Jerry_2014_v1.05.w3x has 5,802 bare CR, 1,380 bare LF and
        // 2 CRLF in one file.
        Assert.Equal(new[] { "a", "b", "c", "d" }, JassLines.Split("a\rb\nc\r\nd"));
    }

    [Fact]
    public void Empty_and_terminator_only_input_behave_like_string_Split()
    {
        Assert.Equal(new[] { "" }, JassLines.Split(""));
        Assert.Equal(new[] { "", "" }, JassLines.Split("\n"));
        Assert.Equal(new[] { "", "" }, JassLines.Split("\r"));
        Assert.Equal(new[] { "", "" }, JassLines.Split("\r\n"));
    }

    [Theory]
    [InlineData("\n")]
    [InlineData("\r\n")]
    [InlineData("\r")]
    public void LineStarts_agrees_with_Split_on_every_terminator(string eol)
    {
        var text = string.Join(eol, "alpha", "beta", "gamma") + eol;
        var lines = JassLines.Split(text);
        var starts = JassLines.LineStarts(text);

        Assert.Equal(lines.Length, starts.Length);
        for (int i = 0; i < lines.Length; i++)
            Assert.Equal(lines[i], text.Substring(starts[i], lines[i].Length));
    }

    [Theory]
    [InlineData("\n")]
    [InlineData("\r\n")]
    [InlineData("\r")]
    public void A_function_is_found_whatever_the_script_is_separated_with(string eol)
    {
        var text = Fn.Replace("\n", eol);
        var fns = JassFunctionIndex.Parse(text);

        Assert.Single(fns);
        Assert.Equal("A", fns[0].Name);
        Assert.Equal(1, fns[0].StartLine);
        Assert.Equal(3, fns[0].EndLine);
    }

    [Theory]
    [InlineData("\n")]
    [InlineData("\r\n")]
    [InlineData("\r")]
    public void Slice_and_replace_stay_inverses_on_every_terminator(string eol)
    {
        var text = ("header" + eol) + Fn.Replace("\n", eol) + ("footer" + eol);
        var starts = ScriptCommand.ComputeLineStarts(text);
        var fn = JassFunctionIndex.Parse(text).Single();

        var body = ScriptCommand.SliceFunction(text, starts, fn.StartLine, fn.EndLine);
        Assert.StartsWith("function A", body);
        Assert.EndsWith("endfunction", body);

        // Putting back exactly what was taken out must change nothing. On a CR script this used to
        // be catastrophic rather than merely wrong, the whole file read as one line, so slicing a
        // function returned everything and replacing it wrote over everything.
        var same = ScriptCommand.ReplaceFunction(text, starts, fn, body);
        Assert.Equal(text, same);
    }

    [Theory]
    [InlineData("\n")]
    [InlineData("\r\n")]
    [InlineData("\r")]
    public void A_replacement_is_written_in_the_scripts_own_convention(string eol)
    {
        var text = ("header" + eol) + Fn.Replace("\n", eol) + ("footer" + eol);
        var starts = ScriptCommand.ComputeLineStarts(text);
        var fn = JassFunctionIndex.Parse(text).Single();

        var edited = ScriptCommand.ReplaceFunction(text, starts, fn,
            "function A takes nothing returns nothing\ncall BJDebugMsg(\"y\")\nendfunction");

        Assert.Contains("BJDebugMsg(\"y\")", edited);
        Assert.Equal(eol, JassLines.DominantTerminator(edited));
        // Same line count in and out, so the index that follows the edit still lines up.
        Assert.Equal(JassLines.Split(text).Length, JassLines.Split(edited).Length);
    }

    [Fact]
    public void DominantTerminator_picks_the_one_that_actually_dominates()
    {
        Assert.Equal("\r", JassLines.DominantTerminator("a\rb\rc\rd\ne"));
        Assert.Equal("\n", JassLines.DominantTerminator("a\nb\nc\nd\re"));
        Assert.Equal("\r\n", JassLines.DominantTerminator("a\r\nb\r\nc\nd"));
        Assert.Equal("\n", JassLines.DominantTerminator("no terminators at all"));
        Assert.Equal("\n", JassLines.DominantTerminator(""));
    }

    [Theory]
    [InlineData("\n")]
    [InlineData("\r\n")]
    [InlineData("\r")]
    public void The_compile_check_sees_a_break_whatever_the_terminator(string eol)
    {
        // The vacuous pass, pinned. An undeclared assignment target fails the whole war3map.j, and
        // on a CR script the checker used to find nothing at all and report clean.
        var broken = string.Join(eol,
            "function config takes nothing returns nothing",
            "set udg_NeverDeclared = 1",
            "endfunction",
            "function main takes nothing returns nothing",
            "endfunction") + eol;

        var issues = JassScriptCheck.Check(broken);
        Assert.Contains(issues, i => i.Severity == DiagnosticSeverity.Error);
    }

    [Theory]
    [InlineData("\n")]
    [InlineData("\r\n")]
    [InlineData("\r")]
    public void External_calls_are_found_whatever_the_terminator(string eol)
    {
        var text = Fn.Replace("\n", eol);
        var ext = JassSyntax.ExternalCalls(text);
        Assert.Contains("BJDebugMsg", ext);
        Assert.DoesNotContain("A", ext);
    }
}
