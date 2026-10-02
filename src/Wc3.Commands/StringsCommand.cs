// src/Wc3.Commands/StringsCommand.cs
using System.Globalization;
using System.Text;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// One war3map.wts entry: <c>STRING &lt;Id&gt;</c> with the block body as <see cref="Text"/>.
/// Text keeps the body's own line terminators as authored but excludes the final terminator
/// that precedes the closing <c>}</c>; an empty block yields "".
/// </summary>
public sealed record WtsEntry(int Id, string Text);

/// <summary>war3map.wts listing; <see cref="FileName"/> is null when the map has no wts file.</summary>
public sealed record StringsListResult(string? FileName, IReadOnlyList<WtsEntry> Entries);

/// <summary>
/// Reads/edits the map's trigger-string table (war3map.wts) as TEXT, byte-faithfully:
/// <see cref="SetEntry"/> splices only the target entry's body and leaves every other
/// character of the file untouched (headers, braces, comments, blank lines, other entries).
/// War3Net's TriggerStrings model stays parse-only here — re-serializing it would rewrite
/// the whole file and lose the original spacing.
/// </summary>
public static class StringsCommand
{
    public const string WtsFileName = "war3map.wts";

    /// <summary>Body span of one entry: [BodyStart, BodyEnd) covers the raw body characters
    /// including the terminator before the '}' line; BodyEnd is the '}' line's start.</summary>
    private readonly record struct EntrySpan(int Id, int BodyStart, int BodyEnd);

    /// <summary>
    /// Parses wts text into entries. Lenient: a header without a following '{' line, or a
    /// block never closed by a '}' line, is skipped rather than throwing (real maps carry
    /// oddities). '{' / '}' must sit on their own lines, as WorldEdit writes them; optional
    /// "//" comment lines between the header and '{' are tolerated.
    /// </summary>
    public static IReadOnlyList<WtsEntry> Parse(string wtsSource)
    {
        ArgumentNullException.ThrowIfNull(wtsSource);
        return ParseSpans(wtsSource)
            .Select(s => new WtsEntry(s.Id, ExtractText(wtsSource, s)))
            .ToList();
    }

    /// <summary>The entry with the given id, or null when absent. First match wins if a
    /// malformed file carries duplicate ids.</summary>
    public static WtsEntry? Get(string wtsSource, int id)
        => Parse(wtsSource).FirstOrDefault(e => e.Id == id);

    /// <summary>
    /// Returns <paramref name="wtsSource"/> with ONLY the body of entry <paramref name="id"/>
    /// replaced by <paramref name="newText"/>; every character outside that body is preserved
    /// verbatim. newText's line breaks are normalized to the entry's own terminator style
    /// (CRLF/LF, detected from the existing body, else from the '{' line) and a single
    /// trailing terminator is appended; empty text produces an empty block. Round-trip
    /// property: SetEntry(src, id, Get(src, id).Text) == src.
    /// Throws <see cref="KeyNotFoundException"/> when the id is absent.
    /// </summary>
    public static string SetEntry(string wtsSource, int id, string newText)
    {
        ArgumentNullException.ThrowIfNull(wtsSource);
        ArgumentNullException.ThrowIfNull(newText);

        var span = ParseSpans(wtsSource).Cast<EntrySpan?>().FirstOrDefault(s => s!.Value.Id == id)
            ?? throw new KeyNotFoundException($"{WtsFileName} has no STRING {id} entry.");

        var rawBody = wtsSource[span.BodyStart..span.BodyEnd];
        var terminator =
            rawBody.EndsWith("\r\n", StringComparison.Ordinal) ? "\r\n" :
            rawBody.EndsWith("\n", StringComparison.Ordinal) ? "\n" :
            // Empty block: mirror the terminator of the '{' line just before the body.
            span.BodyStart >= 2 && wtsSource[span.BodyStart - 2] == '\r' ? "\r\n" : "\n";

        var newBody = newText.Length == 0
            ? string.Empty
            : NormalizeLineBreaks(newText, terminator) + terminator;

        return wtsSource[..span.BodyStart] + newBody + wtsSource[span.BodyEnd..];
    }

    /// <summary>Lists the document's trigger strings (empty result with null FileName when
    /// the map has no war3map.wts). Reads any pending in-memory edit, not the original bytes.</summary>
    public static StringsListResult List(MapDocument doc)
    {
        var entry = doc.GetFile(WtsFileName);
        if (entry is null) return new StringsListResult(null, Array.Empty<WtsEntry>());
        return new StringsListResult(entry.FileName, Parse(CurrentSource(entry)));
    }

    /// <summary>
    /// Applies <see cref="SetEntry"/> to the document's war3map.wts and stores the result
    /// via <see cref="MapDocument.AddOrReplaceRawFile"/> (written verbatim on Save). Edits
    /// stack: each call re-reads the pending override, so successive Sets all survive.
    /// </summary>
    public static void Set(MapDocument doc, int id, string newText)
    {
        var entry = doc.GetFile(WtsFileName)
            ?? throw new FileNotFoundException($"map contains no {WtsFileName}", WtsFileName);
        var updated = SetEntry(CurrentSource(entry), id, newText);
        doc.AddOrReplaceRawFile(WtsFileName, Encoding.UTF8.GetBytes(updated));
    }

    /// <summary>Current text of the entry: the pending raw override when one exists, else the
    /// original bytes. UTF-8 keeps a BOM as U+FEFF through GetString/GetBytes, so decode +
    /// re-encode of untouched text is byte-identical.</summary>
    private static string CurrentSource(MapFileEntry entry)
        => Encoding.UTF8.GetString(entry.OverrideBytes ?? entry.RawBytes);

    private static List<EntrySpan> ParseSpans(string src)
    {
        var spans = new List<EntrySpan>();
        var pos = 0;
        while (pos < src.Length)
        {
            var next = NextLine(src, pos, out var content);
            if (!TryParseHeader(content, out var id)) { pos = next; continue; }

            // Skip optional "//" comment and blank lines between the header and '{'.
            var cursor = next;
            var foundOpen = false;
            var afterOpen = cursor;
            while (cursor < src.Length)
            {
                afterOpen = NextLine(src, cursor, out var line);
                if (line.StartsWith("//", StringComparison.Ordinal) || string.IsNullOrWhiteSpace(line))
                {
                    cursor = afterOpen;
                    continue;
                }
                foundOpen = line == "{";
                break;
            }
            // No '{': resume scanning AT the unexpected line (it may be the next header).
            if (!foundOpen) { pos = cursor; continue; }

            // Body runs until the first line that is exactly "}".
            var bodyStart = afterOpen;
            var scan = afterOpen;
            var closeLineStart = -1;
            while (scan < src.Length)
            {
                var lineStart = scan;
                var afterLine = NextLine(src, scan, out var bodyLine);
                if (bodyLine == "}") { closeLineStart = lineStart; scan = afterLine; break; }
                scan = afterLine;
            }
            if (closeLineStart < 0) { pos = next; continue; }   // unterminated — skip entry

            spans.Add(new EntrySpan(id, bodyStart, closeLineStart));
            pos = scan;
        }
        return spans;
    }

    /// <summary>Reads the line starting at <paramref name="start"/>; returns the next line's
    /// start offset (or src.Length). <paramref name="content"/> excludes the '\n' / "\r\n"
    /// terminator but is otherwise verbatim.</summary>
    private static int NextLine(string src, int start, out string content)
    {
        var nl = src.IndexOf('\n', start);
        if (nl < 0) { content = src[start..]; return src.Length; }
        var end = nl;
        if (end > start && src[end - 1] == '\r') end--;
        content = src[start..end];
        return nl + 1;
    }

    private static bool TryParseHeader(string line, out int id)
    {
        id = 0;
        var s = line.TrimStart('\uFEFF');   // tolerate a UTF-8 BOM before the first header
        if (!s.StartsWith("STRING ", StringComparison.Ordinal)) return false;
        return int.TryParse(s["STRING ".Length..].Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out id);
    }

    private static string ExtractText(string src, EntrySpan span)
    {
        var raw = src[span.BodyStart..span.BodyEnd];
        if (raw.EndsWith("\r\n", StringComparison.Ordinal)) return raw[..^2];
        if (raw.EndsWith("\n", StringComparison.Ordinal)) return raw[..^1];
        return raw;   // empty block: '{' line directly followed by the '}' line
    }

    /// <summary>Rewrites \r\n, \n, and lone \r line breaks to <paramref name="terminator"/> so
    /// an edited body doesn't mix terminator styles with the rest of the file.</summary>
    private static string NormalizeLineBreaks(string text, string terminator)
    {
        var sb = new StringBuilder(text.Length);
        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            if (c == '\r')
            {
                if (i + 1 < text.Length && text[i + 1] == '\n') i++;
                sb.Append(terminator);
            }
            else if (c == '\n') sb.Append(terminator);
            else sb.Append(c);
        }
        return sb.ToString();
    }
}
