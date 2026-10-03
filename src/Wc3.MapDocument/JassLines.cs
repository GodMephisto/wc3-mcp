// src/Wc3.MapDocument/JassLines.cs
namespace Wc3.Model;

/// <summary>
/// Splits a map script into lines, treating CRLF, LF and a bare CR as line terminators.
/// </summary>
/// <remarks>
/// This exists because the obvious spelling is wrong for a third of the real corpus. Measured over
/// 34 maps in the user's own Maps folder, 13 store their war3map.j with a BARE CARRIAGE RETURN as
/// the line separator, the old Mac convention, because that is what the widely used map optimizer
/// emits along with moving the script into scripts\. Those 13 hold roughly 125,000 function
/// declarations between them, one of them 17,304 on its own.
///
/// Every one of those maps reported EXACTLY ZERO functions, because JassFunctionIndex.Parse split
/// on '\n' and a CR separated script has almost none. Nothing failed. The function list was empty,
/// the compile check found no functions and therefore no problems, and lint reported "war3map.j
/// compiles" on a script it had not read a single statement of. That is the worst possible failure
/// shape, a green result that means nothing, and it is the one this file exists to stop.
///
/// Normalizing the text instead was considered and rejected. The Studio writes the editor's text
/// back into the map, so rewriting 5,802 terminators on read would rewrite them on save too, and
/// turn opening a script into a 250 KB diff. The content is left exactly as it is and the SPLITTER
/// is taught the three terminators. That also matches AvaloniaEdit's own line model, so the
/// editor's gutter and a diagnostic's line number agree, which they would not if only one of them
/// counted a bare CR as a line.
/// </remarks>
public static class JassLines
{
    /// <summary>The script's lines, without their terminators. Empty input yields one empty line,
    /// matching <c>string.Split</c> so callers that index by line need no special case.</summary>
    public static string[] Split(string text)
    {
        if (string.IsNullOrEmpty(text)) return new[] { string.Empty };

        var lines = new List<string>();
        int start = 0;
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (c != '\r' && c != '\n') continue;
            lines.Add(text[start..i]);
            // CRLF is ONE terminator, not two. Counting it as two would put every line number
            // past the first line of a Windows-written script off by the number of lines above it.
            if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n') i++;
            start = i + 1;
        }
        lines.Add(text[start..]);
        return lines.ToArray();
    }

    /// <summary>
    /// Char offset where each 1-based line starts. <c>Result[0]</c> is always 0, and the result
    /// always has one entry per line that <see cref="Split"/> returns, so the two agree by
    /// construction rather than by both being written carefully.
    /// </summary>
    public static int[] LineStarts(string text)
    {
        if (string.IsNullOrEmpty(text)) return new[] { 0 };

        var starts = new List<int> { 0 };
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (c != '\r' && c != '\n') continue;
            if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n') i++;
            starts.Add(i + 1);
        }
        // A trailing terminator opens a final empty line, which Split also returns. If the text
        // does not end in one, the last start is already the last line's.
        if (starts[^1] > text.Length) starts.RemoveAt(starts.Count - 1);
        return starts.ToArray();
    }

    /// <summary>
    /// The terminator the text mostly uses, for writing lines back in the script's own style.
    /// Defaults to <c>"\n"</c> for text with no terminator at all.
    /// </summary>
    public static string DominantTerminator(string text)
    {
        if (string.IsNullOrEmpty(text)) return "\n";

        int crlf = 0, lf = 0, cr = 0;
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '\r')
            {
                if (i + 1 < text.Length && text[i + 1] == '\n') { crlf++; i++; }
                else cr++;
            }
            else if (text[i] == '\n') lf++;
        }
        if (crlf >= lf && crlf >= cr && crlf > 0) return "\r\n";
        if (cr > lf) return "\r";
        return "\n";
    }
}
