using Wc3.Commands;

namespace Wc3.Tests;

/// <summary>
/// Pure-logic tests for ScriptCommand.ComputeLineStarts / SliceFunction, the source-slicing
/// that ScriptView relies on to show a single function without laying out a multi-MB script.
/// </summary>
public class ScriptSliceTests
{
    [Fact]
    public void ComputeLineStarts_Empty_SingleZeroEntry()
    {
        Assert.Equal(new[] { 0 }, ScriptCommand.ComputeLineStarts(""));
    }

    [Fact]
    public void ComputeLineStarts_NoNewline_SingleEntry()
    {
        Assert.Equal(new[] { 0 }, ScriptCommand.ComputeLineStarts("abc"));
    }

    [Fact]
    public void ComputeLineStarts_Lf_OffsetsAfterEachNewline()
    {
        Assert.Equal(new[] { 0, 2, 4 }, ScriptCommand.ComputeLineStarts("a\nb\nc"));
    }

    [Fact]
    public void ComputeLineStarts_TrailingNewline_AddsEmptyFinalLine()
    {
        Assert.Equal(new[] { 0, 2 }, ScriptCommand.ComputeLineStarts("a\n"));
    }

    [Fact]
    public void ComputeLineStarts_Crlf_CountsLfPosition()
    {
        // '\n' is at index 2 in "a\r\nb"; the next line starts at 3.
        Assert.Equal(new[] { 0, 3 }, ScriptCommand.ComputeLineStarts("a\r\nb"));
    }

    [Fact]
    public void SliceFunction_SingleLine_ReturnsThatLine()
    {
        const string src = "line1\nline2\nline3";
        var ls = ScriptCommand.ComputeLineStarts(src);
        Assert.Equal("line1", ScriptCommand.SliceFunction(src, ls, 1, 1));
        Assert.Equal("line3", ScriptCommand.SliceFunction(src, ls, 3, 3));
    }

    [Fact]
    public void SliceFunction_MultiLine_ReturnsInclusiveRange()
    {
        const string src = "line1\nline2\nline3";
        var ls = ScriptCommand.ComputeLineStarts(src);
        Assert.Equal("line2\nline3", ScriptCommand.SliceFunction(src, ls, 2, 3));
    }

    [Fact]
    public void SliceFunction_Crlf_TrimsTrailingCarriageReturn()
    {
        const string src = "a\r\nb\r\nc";
        var ls = ScriptCommand.ComputeLineStarts(src);
        Assert.Equal("a", ScriptCommand.SliceFunction(src, ls, 1, 1));
        Assert.Equal("b", ScriptCommand.SliceFunction(src, ls, 2, 2));
        Assert.Equal("c", ScriptCommand.SliceFunction(src, ls, 3, 3));
    }

    [Fact]
    public void SliceFunction_LastLineNoNewline_RunsToEof()
    {
        const string src = "onlyline";
        var ls = ScriptCommand.ComputeLineStarts(src);
        Assert.Equal("onlyline", ScriptCommand.SliceFunction(src, ls, 1, 1));
    }

    [Fact]
    public void SliceFunction_EmptySource_ReturnsEmpty()
    {
        var ls = ScriptCommand.ComputeLineStarts("");
        Assert.Equal("", ScriptCommand.SliceFunction("", ls, 1, 1));
    }

    [Fact]
    public void SliceFunction_EndLineBeyondFile_ClampsToLastLine()
    {
        const string src = "line1\nline2\nline3";
        var ls = ScriptCommand.ComputeLineStarts(src);
        Assert.Equal("line1\nline2\nline3", ScriptCommand.SliceFunction(src, ls, 1, 99));
    }

    [Fact]
    public void SliceFunction_StartLineOutOfRange_ReturnsEmpty()
    {
        const string src = "line1\nline2\nline3";
        var ls = ScriptCommand.ComputeLineStarts(src);
        Assert.Equal("", ScriptCommand.SliceFunction(src, ls, 0, 1));   // below 1
        Assert.Equal("", ScriptCommand.SliceFunction(src, ls, 4, 4));   // above line count
    }

    [Fact]
    public void SliceFunction_EndBeforeStart_ClampsWithoutThrowing()
    {
        const string src = "line1\nline2\nline3";
        var ls = ScriptCommand.ComputeLineStarts(src);
        // endLine < startLine is clamped up to startLine, so it yields just that line.
        Assert.Equal("line2", ScriptCommand.SliceFunction(src, ls, 2, 1));
    }
}
