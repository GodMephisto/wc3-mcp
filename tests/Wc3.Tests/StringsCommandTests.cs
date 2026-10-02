using Wc3.Commands;

namespace Wc3.Tests;

/// <summary>
/// Pure-logic tests for StringsCommand.Parse/Get/SetEntry — the war3map.wts text splicing
/// that StringImportView relies on. Byte-faithfulness is the core contract: SetEntry must
/// touch ONLY the target entry's body and leave every other character verbatim.
/// </summary>
public class StringsCommandTests
{
    // WorldEdit-style CRLF file: single-line, multi-line, and empty bodies,
    // plus a blank line between entries.
    private const string Crlf =
        "STRING 1\r\n{\r\nFirst\r\n}\r\n\r\n" +
        "STRING 7\r\n{\r\nSecond line one\r\nSecond line two\r\n}\r\n\r\n" +
        "STRING 12\r\n{\r\n}\r\n";

    [Fact]
    public void Parse_MultiEntry_ReturnsAllIdsAndTexts()
    {
        var entries = StringsCommand.Parse(Crlf);

        Assert.Equal(new[] { 1, 7, 12 }, entries.Select(e => e.Id));
        Assert.Equal("First", entries[0].Text);
        Assert.Equal("Second line one\r\nSecond line two", entries[1].Text);
        Assert.Equal("", entries[2].Text);
    }

    [Fact]
    public void Parse_LfFile_Works()
    {
        var entries = StringsCommand.Parse("STRING 3\n{\nhello\n}\n");
        var e = Assert.Single(entries);
        Assert.Equal(3, e.Id);
        Assert.Equal("hello", e.Text);
    }

    [Fact]
    public void Parse_CommentLineBetweenHeaderAndBrace_Tolerated()
    {
        var entries = StringsCommand.Parse("STRING 2\n// a WorldEdit comment\n{\nX\n}\n");
        var e = Assert.Single(entries);
        Assert.Equal(2, e.Id);
        Assert.Equal("X", e.Text);
    }

    [Fact]
    public void Parse_LeadingBom_FirstEntryStillParsed()
    {
        var entries = StringsCommand.Parse("\uFEFFSTRING 1\n{\nA\n}\n");
        var e = Assert.Single(entries);
        Assert.Equal(1, e.Id);
        Assert.Equal("A", e.Text);
    }

    [Fact]
    public void Parse_LeadingZeros_ParsedAsSameId()
    {
        var entries = StringsCommand.Parse("STRING 007\n{\nbond\n}\n");
        Assert.Equal(7, Assert.Single(entries).Id);
    }

    [Fact]
    public void Parse_UnterminatedBlock_SkippedWithoutThrowing()
    {
        Assert.Empty(StringsCommand.Parse("STRING 1\n{\nnever closed"));
    }

    [Fact]
    public void Parse_HeaderWithoutBrace_SkippedAndNextEntryStillFound()
    {
        var entries = StringsCommand.Parse("STRING 1\njunk line\nSTRING 2\n{\nok\n}\n");
        var e = Assert.Single(entries);
        Assert.Equal(2, e.Id);
        Assert.Equal("ok", e.Text);
    }

    [Fact]
    public void Get_ById_ReturnsEntry()
    {
        var e = StringsCommand.Get(Crlf, 7);
        Assert.NotNull(e);
        Assert.Equal("Second line one\r\nSecond line two", e!.Text);
    }

    [Fact]
    public void Get_MissingId_ReturnsNull()
    {
        Assert.Null(StringsCommand.Get(Crlf, 99));
    }

    [Fact]
    public void SetEntry_ReplacesOnlyTargetBody_OthersByteIdentical()
    {
        var result = StringsCommand.SetEntry(Crlf, 7, "Replaced");

        // Full-string equality against a hand-built expectation IS the byte-faithfulness
        // assertion: every character outside STRING 7's body must be verbatim.
        Assert.Equal(
            "STRING 1\r\n{\r\nFirst\r\n}\r\n\r\n" +
            "STRING 7\r\n{\r\nReplaced\r\n}\r\n\r\n" +
            "STRING 12\r\n{\r\n}\r\n",
            result);
    }

    [Fact]
    public void SetEntry_SameText_IsByteIdentical()
    {
        foreach (var id in new[] { 1, 7, 12 })
            Assert.Equal(Crlf, StringsCommand.SetEntry(Crlf, id, StringsCommand.Get(Crlf, id)!.Text));
    }

    [Fact]
    public void SetEntry_CrlfFile_NormalizesNewTextLineBreaksToCrlf()
    {
        var result = StringsCommand.SetEntry(Crlf, 1, "a\nb");
        Assert.Contains("STRING 1\r\n{\r\na\r\nb\r\n}\r\n", result);
    }

    [Fact]
    public void SetEntry_LfFile_KeepsLf()
    {
        var result = StringsCommand.SetEntry("STRING 3\n{\nold\n}\n", 3, "new1\r\nnew2");
        Assert.Equal("STRING 3\n{\nnew1\nnew2\n}\n", result);
    }

    [Fact]
    public void SetEntry_EmptyText_ProducesEmptyBlock()
    {
        var result = StringsCommand.SetEntry(Crlf, 1, "");
        Assert.StartsWith("STRING 1\r\n{\r\n}\r\n\r\nSTRING 7\r\n", result);
    }

    [Fact]
    public void SetEntry_EmptyBlock_GetsTerminatorMatchingFile()
    {
        // STRING 12's body is empty — the terminator must be inferred from the '{' line.
        var result = StringsCommand.SetEntry(Crlf, 12, "filled");
        Assert.EndsWith("STRING 12\r\n{\r\nfilled\r\n}\r\n", result);
    }

    [Fact]
    public void SetEntry_PreservesLeadingBom()
    {
        var result = StringsCommand.SetEntry("\uFEFFSTRING 1\n{\nA\n}\n", 1, "B");
        Assert.Equal("\uFEFFSTRING 1\n{\nB\n}\n", result);
    }

    [Fact]
    public void SetEntry_MissingId_Throws()
    {
        Assert.Throws<KeyNotFoundException>(() => StringsCommand.SetEntry(Crlf, 99, "x"));
    }

    [Fact]
    public void SetEntry_EditsStack()
    {
        var result = StringsCommand.SetEntry(StringsCommand.SetEntry(Crlf, 1, "x"), 12, "y");
        Assert.Equal(
            "STRING 1\r\n{\r\nx\r\n}\r\n\r\n" +
            "STRING 7\r\n{\r\nSecond line one\r\nSecond line two\r\n}\r\n\r\n" +
            "STRING 12\r\n{\r\ny\r\n}\r\n",
            result);
    }
}
