// src/Wc3.Commands/ScriptCommand.cs
using System.Text;
using Wc3.Model;

namespace Wc3.Commands;

public sealed record ScriptFunctionsResult(string ScriptFile, IReadOnlyList<JassFunction> Functions);

/// <summary>Where a name is used in the map's script, and how many of those uses pass it as a
/// <c>code</c> value rather than calling it.</summary>
public sealed record ScriptReferencesResult(
    string ScriptFile, string Name, int AsCode, IReadOnlyList<JassReference> References);

public static class ScriptCommand
{
    /// <summary>
    /// Lists the functions declared in the map's script (war3map.j, else war3map.lua).
    /// Lua scripts go through the same JASS scanner best-effort — typically zero hits,
    /// but the result still names the file so the caller knows what was inspected.
    /// </summary>
    public static ScriptFunctionsResult Functions(MapDocument doc)
    {
        var entry = ScriptEntry(doc);

        // Prefer pending in-memory edits (OverrideBytes) over the original bytes so
        // re-listing after a script edit reflects the current document state.
        var source = ScriptText.GetString(entry.CurrentBytes);
        var functions = JassFunctionIndex.Parse(source).OrderBy(f => f.StartLine).ToList();
        return new ScriptFunctionsResult(entry.FileName!, functions);
    }

    /// <summary>
    /// The map's script file and its decoded text, without indexing it.
    /// </summary>
    /// <remarks>
    /// Split out from <see cref="Functions"/> for the Studio, which needs the text on the UI
    /// thread to put it in the editor but wants the indexing off it. Parsing 8.4 MB costs 131ms
    /// and deriving its externals another 161ms, and neither has to happen before the reader can
    /// see their script.
    /// </remarks>
    public static (string ScriptFile, string Source) Read(MapDocument doc)
    {
        var entry = ScriptEntry(doc);
        return (entry.FileName!, ScriptEncoding.GetString(entry.CurrentBytes));
    }

    /// <summary>
    /// Latin-1, deliberately, and it must match what Parsers.cs uses to register war3map.j.
    ///
    /// A map script is a byte stream with no declared encoding, and real maps carry bytes that are
    /// not valid UTF-8 (one map in this library carries about 52,000 of them, from text written in
    /// a legacy code page). Decoding those as UTF-8 turns each invalid sequence into U+FFFD, and
    /// re-encoding U+FFFD writes back the three bytes EF BF BD, so the original byte is gone and
    /// every later round trip looks clean.
    ///
    /// This path read UTF-8 and wrote UTF-8, and it is the one the Studio's Script panel uses to
    /// load and to save. Measured on a synthetic script, the five bytes E3 29 B5 F1 80 came back
    /// as EF BF BD 29 EF BF BD EF BF BD, a file five bytes longer with three bytes destroyed,
    /// from opening the panel and saving without typing anything.
    ///
    /// Latin-1 maps every byte 0 to 255 to the same code point and back, so it is lossless in both
    /// directions. The cost is that genuinely multi-byte text shows as mojibake in the editor,
    /// which is a display fault in rare string literals rather than permanent damage to the map.
    /// Parsers.cs and HeroWiringAudit already made this call, and this makes the third caller
    /// agree with them instead of quietly disagreeing.
    ///
    /// The same reasoning, and the same fix, as StringsCommand for war3map.wts.
    /// </summary>
    public static readonly Encoding ScriptEncoding = ScriptText.Encoding;

    /// <summary>
    /// Writes the script text back to the entry it came FROM, whatever that entry is named.
    /// </summary>
    /// <remarks>
    /// The name matters and it is not always "war3map.j". 13 of 34 maps measured keep the script
    /// at scripts\war3map.j, and three writers hardcoded the root name. On such a map, placing a
    /// unit read the nested script, appended its spawn code, and wrote the result to the ROOT name,
    /// leaving the map with TWO scripts, the original 249,947 byte one untouched under scripts\ and
    /// a 250,245 byte edited copy at the root. Verified on Tom_and_Jerry_2014_v1.05.w3x, 71 entries
    /// before and 73 after.
    ///
    /// ScriptPorter and ScriptRepairCommand already wrote back to entry.FileName, so the correct
    /// pattern existed. This makes it the only reachable one.
    /// </remarks>
    public static string Write(MapDocument doc, string text, Encoding? encoding = null)
    {
        var entry = ScriptEntry(doc);
        doc.AddOrReplaceRawFile(entry.FileName!, (encoding ?? ScriptEncoding).GetBytes(text));
        return entry.FileName!;
    }

    /// <summary>Writes already-encoded script bytes back to the entry they came from.</summary>
    public static string Write(MapDocument doc, byte[] bytes)
    {
        var entry = ScriptEntry(doc);
        doc.AddOrReplaceRawFile(entry.FileName!, bytes);
        return entry.FileName!;
    }

    /// <summary>
    /// Every use of <paramref name="name"/> in the map's script, declaration excluded.
    /// </summary>
    /// <remarks>
    /// "Who calls this", which go to definition does not answer and which a merged arena script
    /// raises constantly. The count of code references is surfaced separately because that is the
    /// half a text search cannot find, JASS passes a handler as a code value by naming it without
    /// parentheses, so TriggerAddAction(t, function Foo) is how most handlers are reached and
    /// grepping for "Foo(" misses every one.
    /// </remarks>
    public static ScriptReferencesResult References(MapDocument doc, string name)
    {
        var (file, source) = Read(doc);
        var uses = JassReferences.FindUses(source, name);
        return new ScriptReferencesResult(
            file, name,
            uses.Count(r => r.Kind == JassReferenceKind.CodeReference),
            uses);
    }

    /// <summary>The map's script entry, war3map.j first, else war3map.lua.</summary>
    private static MapFileEntry ScriptEntry(MapDocument doc) =>
        doc.GetFile("war3map.j")
        ?? doc.GetFile(@"scripts\war3map.j")
        ?? doc.GetFile("war3map.lua")
        ?? throw new FileNotFoundException(
            "map contains no war3map.j or war3map.lua", "war3map.j");

    /// <summary>
    /// Char offset of the start of each 1-based line, following JassFunctionIndex line numbering.
    /// Result[0] is always 0. Empty source yields a single entry [0].
    /// </summary>
    /// <remarks>
    /// Delegates to <see cref="JassLines.LineStarts"/> so the offsets and the line numbers cannot
    /// disagree. They did. This counted only LF while the index counted only LF too, which was
    /// consistent and both wrong, and on the 13 of 34 measured maps that separate their script
    /// with a bare CR it meant one enormous line. Slicing a function out of such a file returned
    /// the whole file, and replacing one would have written it back over everything.
    /// </remarks>
    public static int[] ComputeLineStarts(string source) => JassLines.LineStarts(source);

    /// <summary>
    /// Returns the substring of <paramref name="source"/> spanning the 1-based line range
    /// [<paramref name="startLine"/>, <paramref name="endLine"/>] (inclusive), with the
    /// trailing newline excluded (and a preceding '\r' trimmed for CRLF sources). The last
    /// line runs to EOF. Returns "" when startLine is out of range. endLine is clamped into
    /// [startLine, lineCount] so a function whose EndLine overruns the file is still safe.
    /// <paramref name="lineStarts"/> must come from <see cref="ComputeLineStarts"/>.
    /// </summary>
    public static string SliceFunction(string source, int[] lineStarts, int startLine, int endLine)
    {
        if (lineStarts is null || lineStarts.Length == 0) return string.Empty;
        if (startLine < 1 || startLine > lineStarts.Length) return string.Empty;

        var clampedEnd = Math.Clamp(endLine, startLine, lineStarts.Length);
        var start = lineStarts[startLine - 1];
        var end = clampedEnd < lineStarts.Length
            ? lineStarts[clampedEnd] - 1   // up to (not including) the '\n'
            : source.Length;               // last line runs to EOF
        if (end > start && source[end - 1] == '\r') end--;
        if (end < start) end = start;
        return source.Substring(start, end - start);
    }

    /// <summary>
    /// Returns the full <paramref name="source"/> with the 1-based line range
    /// [fn.StartLine, fn.EndLine] replaced by <paramref name="newFunctionText"/>.
    /// Byte-faithful everywhere else: bytes before StartLine and after EndLine — including
    /// EndLine's own line terminator, or its absence when the last line runs to EOF — are
    /// untouched, so the file keeps its original trailing-newline state. The replacement
    /// text's newlines are converted to the source's convention (CRLF when the source
    /// contains any CRLF, else LF), and a single trailing newline on
    /// <paramref name="newFunctionText"/> is dropped (the replaced range never includes
    /// EndLine's terminator, so "with" and "without" splice identically). Returns
    /// <paramref name="source"/> unchanged when the fn range is invalid (StartLine out of
    /// range); EndLine is clamped into [StartLine, lineCount] like
    /// <see cref="SliceFunction"/>, making replace the exact inverse of slice.
    /// <paramref name="lineStarts"/> must come from <see cref="ComputeLineStarts"/>.
    /// </summary>
    public static string ReplaceFunction(string source, int[] lineStarts, JassFunction fn, string newFunctionText)
    {
        if (fn is null) return source;
        if (lineStarts is null || lineStarts.Length == 0) return source;
        if (fn.StartLine < 1 || fn.StartLine > lineStarts.Length) return source;

        // Same range math as SliceFunction: [start, end) excludes EndLine's terminator.
        var clampedEnd = Math.Clamp(fn.EndLine, fn.StartLine, lineStarts.Length);
        var start = lineStarts[fn.StartLine - 1];
        var end = clampedEnd < lineStarts.Length
            ? lineStarts[clampedEnd] - 1   // up to (not including) the '\n'
            : source.Length;               // last line runs to EOF
        if (end > start && source[end - 1] == '\r') end--;
        if (end < start) end = start;

        // The script's own convention, including a bare CR, so an edited function is not
        // written back in a different one from the lines around it.
        var newline = JassLines.DominantTerminator(source);
        var replacement = (newFunctionText ?? string.Empty)
            .Replace("\r\n", "\n")
            .Replace("\n", newline);
        if (replacement.EndsWith(newline, StringComparison.Ordinal))
            replacement = replacement[..^newline.Length];

        return string.Concat(source.AsSpan(0, start), replacement, source.AsSpan(end));
    }
}
