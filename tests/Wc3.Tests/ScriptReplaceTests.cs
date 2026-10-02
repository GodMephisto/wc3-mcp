using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Pure-logic tests for ScriptCommand.ReplaceFunction, the splice ScriptView uses to write
/// an edited function back into the full script without disturbing surrounding bytes.
/// </summary>
public class ScriptReplaceTests
{
    private static JassFunction Fn(int startLine, int endLine) =>
        new("f", startLine, endLine, "function f takes nothing returns nothing");

    [Fact]
    public void ReplaceFunction_SingleLine_SwapsOnlyThatLine()
    {
        const string src = "line1\nline2\nline3";
        var ls = ScriptCommand.ComputeLineStarts(src);
        Assert.Equal("line1\nNEW\nline3", ScriptCommand.ReplaceFunction(src, ls, Fn(2, 2), "NEW"));
    }

    [Fact]
    public void ReplaceFunction_MultiLineRange_SwapsInclusiveRange()
    {
        const string src = "line1\nline2\nline3\nline4";
        var ls = ScriptCommand.ComputeLineStarts(src);
        // Two lines replaced by three; surrounding lines and newlines untouched.
        Assert.Equal("line1\nA\nB\nC\nline4",
            ScriptCommand.ReplaceFunction(src, ls, Fn(2, 3), "A\nB\nC"));
    }

    [Fact]
    public void ReplaceFunction_CrlfSource_PreservesCrlfAndConvertsNewText()
    {
        const string src = "a\r\nfunction f\r\nendfunction\r\nz";
        var ls = ScriptCommand.ComputeLineStarts(src);
        // Surrounding CRLFs untouched; the LF-only newText is converted to CRLF.
        Assert.Equal("a\r\nX\r\nY\r\nz",
            ScriptCommand.ReplaceFunction(src, ls, Fn(2, 3), "X\nY"));
    }

    [Fact]
    public void ReplaceFunction_LastFunctionRunsToEof_ReplacesToEnd()
    {
        const string src = "line1\nfunction f\nendfunction";
        var ls = ScriptCommand.ComputeLineStarts(src);
        Assert.Equal("line1\nX\nY", ScriptCommand.ReplaceFunction(src, ls, Fn(2, 3), "X\nY"));
    }

    [Fact]
    public void ReplaceFunction_TrailingNewlineAfterLastFunction_IsPreserved()
    {
        const string src = "line1\nfunction f\nendfunction\n";
        var ls = ScriptCommand.ComputeLineStarts(src);
        Assert.Equal("line1\nX\n", ScriptCommand.ReplaceFunction(src, ls, Fn(2, 3), "X"));
    }

    [Fact]
    public void ReplaceFunction_FirstFunction_KeepsEverythingAfter()
    {
        const string src = "function f\nendfunction\nrest";
        var ls = ScriptCommand.ComputeLineStarts(src);
        Assert.Equal("NEW\nrest", ScriptCommand.ReplaceFunction(src, ls, Fn(1, 2), "NEW"));
    }

    [Fact]
    public void ReplaceFunction_StartLineOutOfRange_ReturnsSourceUnchanged()
    {
        const string src = "line1\nline2\nline3";
        var ls = ScriptCommand.ComputeLineStarts(src);
        Assert.Equal(src, ScriptCommand.ReplaceFunction(src, ls, Fn(0, 1), "NEW"));   // below 1
        Assert.Equal(src, ScriptCommand.ReplaceFunction(src, ls, Fn(4, 5), "NEW"));   // above line count
    }

    [Fact]
    public void ReplaceFunction_EndLineBeyondFile_ClampsToLastLine()
    {
        const string src = "line1\nline2\nline3";
        var ls = ScriptCommand.ComputeLineStarts(src);
        Assert.Equal("line1\nNEW", ScriptCommand.ReplaceFunction(src, ls, Fn(2, 99), "NEW"));
    }

    [Fact]
    public void ReplaceFunction_NewTextTrailingNewline_SameResultAsWithout()
    {
        const string src = "a\nfunction f\nendfunction\nz";
        var ls = ScriptCommand.ComputeLineStarts(src);
        var without = ScriptCommand.ReplaceFunction(src, ls, Fn(2, 3), "N1\nN2");
        var with = ScriptCommand.ReplaceFunction(src, ls, Fn(2, 3), "N1\nN2\n");
        Assert.Equal("a\nN1\nN2\nz", without);
        Assert.Equal(without, with);
    }

    [Fact]
    public void ReplaceFunction_NewTextTrailingNewlineAtEof_DoesNotAddOne()
    {
        // The file's trailing-newline state comes from the original source, not the new text.
        const string src = "a\nfunction f\nendfunction";
        var ls = ScriptCommand.ComputeLineStarts(src);
        Assert.Equal("a\nX", ScriptCommand.ReplaceFunction(src, ls, Fn(2, 3), "X\n"));
    }

    [Fact]
    public void ReplaceFunction_SliceThenReplace_IsIdentity()
    {
        const string src = "a\r\nfunction f\r\nendfunction\r\nz";
        var ls = ScriptCommand.ComputeLineStarts(src);
        var slice = ScriptCommand.SliceFunction(src, ls, 2, 3);
        Assert.Equal(src, ScriptCommand.ReplaceFunction(src, ls, Fn(2, 3), slice));
    }
}
