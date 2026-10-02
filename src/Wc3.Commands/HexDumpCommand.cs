// src/Wc3.Commands/HexDumpCommand.cs
using System.Text;

namespace Wc3.Commands;

/// <summary>
/// Pure, windowed hex-dump formatter for a virtualized binary viewer. A UI sizes its
/// scrollbar with <see cref="RowCount"/> and calls <see cref="Format(byte[], int, int, int)"/>
/// for just the visible rows on scroll — nothing here ever formats more than the
/// requested window.
/// </summary>
public static class HexDumpCommand
{
    private const string HexDigits = "0123456789ABCDEF";

    /// <summary>
    /// Number of dump rows a whole file of <paramref name="totalBytes"/> bytes produces at
    /// <paramref name="bytesPerRow"/> — lets a UI size a virtualized scrollbar without
    /// formatting anything. Non-positive <paramref name="totalBytes"/> yields 0;
    /// <paramref name="bytesPerRow"/> below 1 is clamped to 1.
    /// </summary>
    public static int RowCount(int totalBytes, int bytesPerRow = 16)
    {
        if (totalBytes <= 0) return 0;
        if (bytesPerRow < 1) bytesPerRow = 1;
        return (totalBytes + bytesPerRow - 1) / bytesPerRow;
    }

    /// <summary>Dump lines for the whole array (convenience over the windowed overload).</summary>
    public static IReadOnlyList<string> Format(byte[] data)
        => data is null ? Array.Empty<string>() : Format(data, 0, data.Length);

    /// <summary>
    /// Dump lines for the window [<paramref name="offset"/>, offset+length) of
    /// <paramref name="data"/>. Each line is
    /// <c>OOOOOOOO  XX XX .. XX  |ascii|</c>: an 8-hex-digit absolute offset (the index of
    /// the line's first byte), <paramref name="bytesPerRow"/> bytes as 2-hex-digit
    /// space-separated uppercase pairs (a partial last row is space-padded to full column
    /// width so the ASCII gutter starts in the same column on every line), then the ASCII
    /// gutter between pipes — printable bytes 0x20–0x7E as-is, everything else '.'.
    /// Lines start exactly at the (clamped) offset; a caller wanting conventional
    /// row-aligned labels passes a multiple of <paramref name="bytesPerRow"/>
    /// (e.g. firstVisibleRow * bytesPerRow).
    /// Never throws: the window is intersected with [0, data.Length) — note the window end
    /// stays offset+length, so a negative offset shrinks the window rather than shifting
    /// it — and an empty intersection (offset past EOF, non-positive length, ...) yields an
    /// empty list. offset+length is computed in 64-bit, so int.MaxValue cannot overflow.
    /// <paramref name="bytesPerRow"/> below 1 is clamped to 1.
    /// </summary>
    public static IReadOnlyList<string> Format(byte[] data, int offset, int length, int bytesPerRow = 16)
    {
        if (data is null || data.Length == 0) return Array.Empty<string>();
        if (bytesPerRow < 1) bytesPerRow = 1;

        long start = Math.Clamp(offset, 0, data.Length);
        long end = Math.Clamp((long)offset + length, 0, data.Length);
        if (end <= start) return Array.Empty<string>();

        var lines = new List<string>((int)((end - start + bytesPerRow - 1) / bytesPerRow));
        // One builder reused per line: offset(8) + gap(2) + hex(3n-1) + gap(2) + |ascii|(n+2).
        var sb = new StringBuilder(8 + 2 + (bytesPerRow * 3 - 1) + 2 + bytesPerRow + 2);

        for (long row = start; row < end; row += bytesPerRow)
        {
            sb.Clear();

            // 8-hex-digit absolute offset (data.Length <= int.MaxValue, so 8 digits suffice).
            for (int shift = 28; shift >= 0; shift -= 4)
                sb.Append(HexDigits[(int)(row >> shift) & 0xF]);
            sb.Append(' ').Append(' ');

            long rowEnd = Math.Min(row + bytesPerRow, end);

            // Hex column, padded to full width so the ASCII gutter aligns on a partial row.
            for (int k = 0; k < bytesPerRow; k++)
            {
                if (k > 0) sb.Append(' ');
                long i = row + k;
                if (i < rowEnd)
                {
                    byte b = data[i];
                    sb.Append(HexDigits[b >> 4]).Append(HexDigits[b & 0xF]);
                }
                else
                {
                    sb.Append(' ').Append(' ');
                }
            }

            sb.Append(' ').Append(' ').Append('|');
            for (long i = row; i < rowEnd; i++)
            {
                byte b = data[i];
                sb.Append(b is >= 0x20 and <= 0x7E ? (char)b : '.');
            }
            sb.Append('|');

            lines.Add(sb.ToString());
        }

        return lines;
    }
}
