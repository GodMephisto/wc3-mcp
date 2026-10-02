using Wc3.Commands;

namespace Wc3.Tests;

/// <summary>
/// Pure-logic tests for HexDumpCommand.Format / RowCount, the windowed hex-dump
/// formatting a virtualized binary viewer relies on to show just the visible rows
/// of a large file. Hermetic — known byte arrays, exact expected lines.
/// </summary>
public class HexDumpTests
{
    [Fact]
    public void Format_FullRow_ExactLine()
    {
        var data = "Hello World!"u8.ToArray()
            .Concat(new byte[] { 0x00, 0x41, 0x7F, 0xFF })
            .ToArray();

        var lines = HexDumpCommand.Format(data);

        var line = Assert.Single(lines);
        Assert.Equal("00000000  48 65 6C 6C 6F 20 57 6F 72 6C 64 21 00 41 7F FF  |Hello World!.A..|", line);
    }

    [Fact]
    public void Format_WholeArrayConvenience_MatchesWindowed()
    {
        var data = Enumerable.Range(0, 40).Select(i => (byte)i).ToArray();
        Assert.Equal(HexDumpCommand.Format(data, 0, data.Length), HexDumpCommand.Format(data));
    }

    [Fact]
    public void Format_PartialLastRow_PadsHexColumnSoGutterAligns()
    {
        var lines = HexDumpCommand.Format(new byte[] { 0x41, 0x42 }, 0, 2, bytesPerRow: 4);

        var line = Assert.Single(lines);
        // Hex column stays 4*3-1 = 11 chars wide: "41 42" + 6 pad spaces, then the 2-space gap.
        Assert.Equal("00000000  41 42        |AB|", line);
    }

    [Fact]
    public void Format_MultiRow_PartialRowGutterStartsInSameColumn()
    {
        var data = Enumerable.Range(0, 17).Select(i => (byte)('A' + i)).ToArray(); // 'A'..'Q'

        var lines = HexDumpCommand.Format(data);

        Assert.Equal(2, lines.Count);
        Assert.Equal("00000000  41 42 43 44 45 46 47 48 49 4A 4B 4C 4D 4E 4F 50  |ABCDEFGHIJKLMNOP|", lines[0]);
        Assert.Equal("00000010  51" + new string(' ', 45) + "  |Q|", lines[1]);
        Assert.Equal(lines[0].IndexOf('|'), lines[1].IndexOf('|')); // gutter column aligned
    }

    [Fact]
    public void Format_Window_UsesAbsoluteOffsets()
    {
        var data = Enumerable.Range(0, 40).Select(i => (byte)i).ToArray();

        var lines = HexDumpCommand.Format(data, 16, 16);

        var line = Assert.Single(lines);
        Assert.Equal("00000010  10 11 12 13 14 15 16 17 18 19 1A 1B 1C 1D 1E 1F  |................|", line);
    }

    [Fact]
    public void Format_UnalignedWindow_OffsetLabelIsExact()
    {
        var data = Enumerable.Range(0, 16).Select(i => (byte)i).ToArray();

        var lines = HexDumpCommand.Format(data, 3, 4, bytesPerRow: 4);

        var line = Assert.Single(lines);
        Assert.Equal("00000003  03 04 05 06  |....|", line);
    }

    [Fact]
    public void Format_AsciiGutter_PrintableBoundaries()
    {
        // 0x1F just below printable, 0x20 space, 0x7E '~' last printable, 0x7F just above.
        var lines = HexDumpCommand.Format(new byte[] { 0x1F, 0x20, 0x7E, 0x7F }, 0, 4, bytesPerRow: 4);

        var line = Assert.Single(lines);
        Assert.Equal("00000000  1F 20 7E 7F  |. ~.|", line);
    }

    [Fact]
    public void Format_EmptyArray_Empty()
    {
        Assert.Empty(HexDumpCommand.Format(Array.Empty<byte>()));
        Assert.Empty(HexDumpCommand.Format(Array.Empty<byte>(), 0, 16));
    }

    [Fact]
    public void Format_OffsetBeyondEnd_Empty()
    {
        var data = new byte[] { 1, 2, 3, 4 };
        Assert.Empty(HexDumpCommand.Format(data, 4, 1));
        Assert.Empty(HexDumpCommand.Format(data, 10, 5));
    }

    [Fact]
    public void Format_NegativeOffsetOrLength_ClampsWithoutThrowing()
    {
        var data = new byte[] { 1, 2, 3, 4 };
        Assert.Empty(HexDumpCommand.Format(data, 0, 0));
        Assert.Empty(HexDumpCommand.Format(data, 0, -1));
        // Window end stays offset+length: [-8, -4) misses the array entirely...
        Assert.Empty(HexDumpCommand.Format(data, -8, 4));
        // ...while [-2, 4) clamps to the whole array.
        Assert.Equal(HexDumpCommand.Format(data), HexDumpCommand.Format(data, -2, 6));
    }

    [Fact]
    public void Format_LengthOverrunsArray_ClampsToEnd()
    {
        var data = new byte[] { 0x10, 0x20, 0x41, 0x42 };

        var lines = HexDumpCommand.Format(data, 2, 100, bytesPerRow: 2);

        var line = Assert.Single(lines);
        Assert.Equal("00000002  41 42  |AB|", line);
    }

    [Fact]
    public void Format_HugeOffsetAndLength_NoOverflow()
    {
        var data = new byte[] { 0x41, 0x42 };

        Assert.Empty(HexDumpCommand.Format(data, int.MaxValue, int.MaxValue));

        var lines = HexDumpCommand.Format(data, 1, int.MaxValue, bytesPerRow: 1);
        var line = Assert.Single(lines);
        Assert.Equal("00000001  42  |B|", line);
    }

    [Fact]
    public void Format_BytesPerRowBelowOne_ClampsToOne()
    {
        var lines = HexDumpCommand.Format(new byte[] { 0x41, 0x42 }, 0, 2, bytesPerRow: 0);

        Assert.Equal(2, lines.Count);
        Assert.Equal("00000000  41  |A|", lines[0]);
        Assert.Equal("00000001  42  |B|", lines[1]);
    }

    [Fact]
    public void RowCount_DefaultWidth_Math()
    {
        Assert.Equal(0, HexDumpCommand.RowCount(0));
        Assert.Equal(1, HexDumpCommand.RowCount(1));
        Assert.Equal(1, HexDumpCommand.RowCount(16));
        Assert.Equal(2, HexDumpCommand.RowCount(17));
        Assert.Equal(2, HexDumpCommand.RowCount(32));
    }

    [Fact]
    public void RowCount_CustomWidth_Math()
    {
        Assert.Equal(2, HexDumpCommand.RowCount(8, bytesPerRow: 4));
        Assert.Equal(3, HexDumpCommand.RowCount(9, bytesPerRow: 4));
    }

    [Fact]
    public void RowCount_NonPositiveInputs_Graceful()
    {
        Assert.Equal(0, HexDumpCommand.RowCount(-5));
        Assert.Equal(16, HexDumpCommand.RowCount(16, bytesPerRow: 0)); // width clamps to 1
    }

    [Fact]
    public void RowCount_MatchesFormattedLineCount()
    {
        var data = Enumerable.Range(0, 37).Select(i => (byte)i).ToArray();
        Assert.Equal(HexDumpCommand.RowCount(data.Length), HexDumpCommand.Format(data).Count);
        Assert.Equal(HexDumpCommand.RowCount(data.Length, 8), HexDumpCommand.Format(data, 0, data.Length, 8).Count);
    }
}
