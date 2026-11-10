// src/Wc3.Commands/SoundScriptSpliceCommand.cs
using System.Text;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Outcome of a splice. <see cref="BytesAdded"/> is the payload appended by this call and is zero
/// for a refusal and for an idempotent no-op. <see cref="NewLength"/> is the script's length after
/// the call, so a caller can show it without a second read. <see cref="AlreadyPresent"/> is true
/// only when both marker lines were already in the script and nothing was written.
/// </summary>
public sealed record SoundScriptSpliceResult(
    bool Ok,
    string Message,
    int BytesAdded,
    int NewLength,
    bool AlreadyPresent);

/// <summary>
/// Installs the JASS dispatcher that <see cref="SoundImportCommand"/> returns into a map's
/// war3map.j. The import command deliberately never edits the script, and this is the one place
/// that does, so the whole operation is a byte splice rather than a decode and re-encode.
///
/// Why the block is split. The generated text declares a <c>globals</c> block and JASS permits
/// exactly one, so appending the block whole is a hard compile error on any script that already
/// has one. Measured with the installed pjass on 2026-11-08, appending whole to a script with a
/// globals block reported a syntax error and 76 "Statement outside of function" errors. The
/// block is therefore split, its declarations go inside the script's existing globals block (or a
/// new block is created before the first function), and only its functions are appended.
///
/// Contracts:
/// <list type="bullet">
/// <item>Every original byte is preserved. The two insertion points are line starts, the text
/// outside them is byte-identical and keeps its order, and the bytes before the first insertion
/// are an exact prefix of the result. Nothing is decoded, Latin1 is used only as a byte-transparent
/// view to find line boundaries.</item>
/// <item>Line endings follow the script. A CRLF script gets CRLF even though the generator emits LF.</item>
/// <item>Two one line comments name <c>marker</c>, one at each insertion point. Each part is
/// independently idempotent, so a second splice of the same block reports
/// <see cref="SoundScriptSpliceResult.AlreadyPresent"/> and writes nothing.</item>
/// <item>A map without war3map.j is refused, and a block with no <c>function</c> keyword is
/// refused, because appending something that is not JASS is worse than not appending it.</item>
/// </list>
/// </summary>
public static class SoundScriptSpliceCommand
{
    /// <summary>The marker used when a caller passes none.</summary>
    public const string DefaultMarker = "wc3ctl sound import";

    /// <summary>Reforged stores the script under scripts on some maps, so both names are read,
    /// matching the lookup the other script commands use.</summary>
    private static readonly string[] ScriptNames = { "war3map.j", @"scripts\war3map.j" };

    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);

    /// <summary>
    /// Splices <paramref name="jass"/> into the map's war3map.j, merging its globals declarations
    /// into the script's globals block and appending its functions, then stages the result through
    /// <see cref="FileEditCommand.AddOrReplace"/> so the document owns the write.
    /// </summary>
    /// <param name="doc">The map to splice into. Nothing is saved here.</param>
    /// <param name="jass">The block from <see cref="SoundImportCommand"/>, LF or CRLF.</param>
    /// <param name="marker">A short label for the two comment lines. Blank means
    /// <see cref="DefaultMarker"/>.</param>
    public static SoundScriptSpliceResult Splice(MapDocument doc, string jass, string? marker = null)
    {
        ArgumentNullException.ThrowIfNull(doc);

        var entry = FindScript(doc);
        if (entry?.FileName is null)
            return new(false, "map contains no war3map.j script", 0, 0, false);

        var original = FileEditCommand.ReadBytes(doc, entry.FileName);
        int before = original.Length;

        if (!ContainsFunctionKeyword(jass))
            return new(false, "the block contains no JASS function, nothing was spliced", 0, before, false);

        var newline = NewLineOf(original);
        var parts = SplitBlock(jass!, newline);
        var label = OneLine(marker);
        var globalsMarker = "// " + label + " globals";
        var functionsMarker = "// " + label + " functions";

        bool wantGlobals = parts.Globals.Length > 0;
        bool globalsPresent = wantGlobals && Contains(original, globalsMarker);
        bool functionsPresent = Contains(original, functionsMarker);

        if (functionsPresent && (!wantGlobals || globalsPresent))
            return new(true, $"the block is already in {entry.FileName}, nothing was spliced",
                0, before, true);

        // Every offset is a position in the original script, so the append cannot shift the
        // earlier globals offset. Line scanning reads the original only, never the payload that
        // already carries the appended functions.
        var view = Encoding.Latin1.GetString(original);
        var spans = LineSpans(view);

        byte[] payload = original;
        int functionsStart = -1;
        string detail;

        if (!functionsPresent)
        {
            var text = new StringBuilder();
            bool separator = before > 0 && original[^1] != (byte)'\n' && original[^1] != (byte)'\r';
            if (separator) text.Append(newline);
            functionsStart = before + (separator ? newline.Length : 0);
            text.Append(functionsMarker).Append(newline).Append(parts.Functions).Append(newline);
            payload = Concat(payload, Utf8NoBom.GetBytes(text.ToString()));
            detail = "functions appended";
        }
        else
        {
            detail = "functions already present";
        }

        if (wantGlobals && !globalsPresent)
        {
            int endglobalsAt = FirstLine(spans, view, t => t == "endglobals");
            int functionAt = FirstLine(spans, view, t => StartsWithFunction(t));

            if (endglobalsAt >= 0)
            {
                var text = globalsMarker + newline + parts.Globals + newline;
                payload = InsertAt(payload, spans[endglobalsAt].Start, Utf8NoBom.GetBytes(text));
                detail = "globals merged into the existing block, " + detail;
            }
            else
            {
                var builder = new StringBuilder();
                builder.Append(globalsMarker).Append(newline)
                       .Append("globals").Append(newline)
                       .Append(parts.Globals).Append(newline)
                       .Append("endglobals").Append(newline).Append(newline);
                int at = functionAt >= 0 ? spans[functionAt].Start
                       : functionsStart >= 0 ? functionsStart
                       : payload.Length;
                payload = InsertAt(payload, at, Utf8NoBom.GetBytes(builder.ToString()));
                detail = "globals block created before the first function, " + detail;
            }
        }

        int added = payload.Length - before;
        FileEditCommand.AddOrReplace(doc, entry.FileName, payload);
        return new(true, $"spliced {added} byte(s) into {entry.FileName}, {detail}",
            added, payload.Length, false);
    }

    /// <summary>The two halves of the generated block. <see cref="Globals"/> is the declaration
    /// lines without the <c>globals</c>/<c>endglobals</c> brackets, empty when the block declares
    /// none. <see cref="Functions"/> is everything else, comments included.</summary>
    private readonly record struct BlockParts(string Globals, string Functions);

    /// <summary>
    /// Splits a block into its globals body and its functions. The first <c>globals</c> line and
    /// the <c>endglobals</c> after it bound the body. A block with no such pair is all functions,
    /// which keeps a hand written function-only block legal.
    /// </summary>
    private static BlockParts SplitBlock(string jass, string newline)
    {
        var text = ToNewLine(jass, "\n");
        var lines = text.Split('\n');

        int start = Array.FindIndex(lines, l => l.Trim() == "globals");
        int end = start < 0 ? -1 : Array.FindIndex(lines, start + 1, l => l.Trim() == "endglobals");
        if (start < 0 || end < 0)
            return new(string.Empty, TrimBlankLines(lines, newline));

        var body = lines[(start + 1)..end]
            .Where(l => l.Trim().Length > 0)
            .ToList();
        var functions = lines[..start].Concat(lines[(end + 1)..]).ToList();

        return new(
            string.Join(newline, body).Trim(),
            TrimBlankLines(functions, newline));
    }

    private static string TrimBlankLines(IReadOnlyList<string> lines, string newline)
    {
        int first = 0;
        int last = lines.Count - 1;
        while (first <= last && lines[first].Trim().Length == 0) first++;
        while (last >= first && lines[last].Trim().Length == 0) last--;
        return first > last ? string.Empty : string.Join(newline, lines.Skip(first).Take(last - first + 1));
    }

    private static MapFileEntry? FindScript(MapDocument doc)
    {
        foreach (var name in ScriptNames)
        {
            var entry = doc.GetFile(name);
            if (entry?.FileName is not null) return entry;
        }
        return null;
    }

    /// <summary>True when the text carries the <c>function</c> keyword as its own token, so
    /// <c>endfunction</c> alone or an identifier like <c>myfunction</c> does not qualify.</summary>
    private static bool ContainsFunctionKeyword(string? text)
    {
        if (string.IsNullOrEmpty(text)) return false;
        const string keyword = "function";
        int from = 0;
        while (true)
        {
            int at = text.IndexOf(keyword, from, StringComparison.Ordinal);
            if (at < 0) return false;
            bool leftOk = at == 0 || !IsIdentifierChar(text[at - 1]);
            int end = at + keyword.Length;
            bool rightOk = end >= text.Length || !IsIdentifierChar(text[end]);
            if (leftOk && rightOk) return true;
            from = at + 1;
        }
    }

    private static bool IsIdentifierChar(char c) => char.IsLetterOrDigit(c) || c == '_';

    /// <summary>A line that opens a top level function declaration.</summary>
    private static bool StartsWithFunction(string text)
    {
        var t = text.TrimStart('\uFEFF').TrimStart();
        if (!t.StartsWith("function", StringComparison.Ordinal)) return false;
        return t.Length == 8 || !IsIdentifierChar(t[8]);
    }

    /// <summary>The span of every line in the byte-transparent view, terminator excluded.</summary>
    private static List<(int Start, int End)> LineSpans(string text)
    {
        var spans = new List<(int, int)>();
        int start = 0;
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] != '\n') continue;
            int end = i > start && text[i - 1] == '\r' ? i - 1 : i;
            spans.Add((start, end));
            start = i + 1;
        }
        if (start <= text.Length) spans.Add((start, text.Length));
        return spans;
    }

    private static int FirstLine(
        IReadOnlyList<(int Start, int End)> spans, string view, Func<string, bool> match)
    {
        for (int i = 0; i < spans.Count; i++)
            if (match(view[spans[i].Start..spans[i].End])) return i;
        return -1;
    }

    private static byte[] InsertAt(byte[] target, int offset, byte[] insert)
    {
        var result = new byte[target.Length + insert.Length];
        Buffer.BlockCopy(target, 0, result, 0, offset);
        Buffer.BlockCopy(insert, 0, result, offset, insert.Length);
        Buffer.BlockCopy(target, offset, result, offset + insert.Length, target.Length - offset);
        return result;
    }

    private static byte[] Concat(byte[] left, byte[] right)
    {
        var result = new byte[left.Length + right.Length];
        Buffer.BlockCopy(left, 0, result, 0, left.Length);
        Buffer.BlockCopy(right, 0, result, left.Length, right.Length);
        return result;
    }

    /// <summary>The first line terminator the script uses. A script with no terminator at all
    /// gets CRLF, which is the convention every Warcraft III script ships with.</summary>
    private static string NewLineOf(byte[] bytes)
    {
        for (int i = 0; i < bytes.Length; i++)
        {
            if (bytes[i] != (byte)'\n') continue;
            return i > 0 && bytes[i - 1] == (byte)'\r' ? "\r\n" : "\n";
        }
        return "\r\n";
    }

    private static string ToNewLine(string text, string newline)
    {
        var normalized = text.Replace("\r\n", "\n").Replace('\r', '\n');
        return newline == "\n" ? normalized : normalized.Replace("\n", newline);
    }

    /// <summary>A marker is one comment line by construction, so any terminator a caller put in
    /// it is collapsed to a space rather than allowed to break the line.</summary>
    private static string OneLine(string? marker)
    {
        var text = string.IsNullOrWhiteSpace(marker) ? DefaultMarker : marker;
        var sb = new StringBuilder(text!.Length);
        foreach (var c in text)
            sb.Append(c is '\r' or '\n' ? ' ' : c);
        return sb.ToString().Trim();
    }

    /// <summary>True when the exact marker line is already in the script. Plain byte search, so it
    /// stays valid whatever encoding the map author used.</summary>
    private static bool Contains(byte[] haystack, string line) =>
        IndexOf(haystack, Utf8NoBom.GetBytes(line)) >= 0;

    private static int IndexOf(byte[] haystack, byte[] needle)
    {
        if (needle.Length == 0 || haystack.Length < needle.Length) return -1;
        for (int i = 0; i <= haystack.Length - needle.Length; i++)
        {
            int j = 0;
            while (j < needle.Length && haystack[i + j] == needle[j]) j++;
            if (j == needle.Length) return i;
        }
        return -1;
    }
}
