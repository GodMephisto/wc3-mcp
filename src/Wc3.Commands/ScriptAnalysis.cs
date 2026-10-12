// src/Wc3.Commands/ScriptAnalysis.cs
using System.Text;
using System.Text.RegularExpressions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Shared reading, indexing and text normalization for the script analysis commands reachable,
/// api-usage, duplicate-bodies, semantic-diff and modernize.
///
/// Nothing here re-implements a pass that already exists. Language detection and the decoded text
/// come from <see cref="ScriptCommand"/>, the JASS function index from
/// <see cref="JassFunctionIndex"/>, the Lua function index from the Lua parser, the call edges and
/// the string dispatch from <see cref="JassCalls"/>, and the comment view from
/// <see cref="JassComments"/>. What this file adds is a language-aware comment strip for Lua and a
/// single identifier rewriter both modernize and the local-rename normalizer share.
/// </summary>
internal static class ScriptAnalysis
{
    private static readonly Regex WhitespaceRun = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex Identifier = new(@"[A-Za-z_]\w*", RegexOptions.Compiled);

    /// <summary>A JASS local declaration, the name is the captured group. A leading
    /// <c>constant</c> and an <c>array</c> modifier are both optional.</summary>
    private static readonly Regex JassLocal = new(
        @"^\s*local\s+(?:constant\s+)?[A-Za-z_]\w*\s+(?:array\s+)?([A-Za-z_]\w*)",
        RegexOptions.Multiline | RegexOptions.Compiled);

    /// <summary>The map's script file, its decoded text, and whether it is Lua rather than JASS.</summary>
    internal static (string File, string Source, bool IsLua) Read(MapDocument doc)
    {
        var (file, source) = ScriptCommand.Read(doc);
        return (file, source, ScriptCommand.IsLua(file));
    }

    /// <summary>Every function a script declares, by the language's own index. A Lua script that
    /// does not parse has no functions rather than an exception, so a caller reports zero.</summary>
    internal static IReadOnlyList<JassFunction> Functions(string source, bool isLua)
    {
        if (!isLua) return JassFunctionIndex.Parse(source);
        try { return Lua.LuaScriptIndex.Functions(source); }
        catch (Lua.LuaSyntaxError) { return Array.Empty<JassFunction>(); }
    }

    /// <summary>Comment-free lines, same count and same indices as the source. JASS and Lua have
    /// different comment markers, so each gets its own stripper.</summary>
    internal static string[] CodeLines(string source, bool isLua) =>
        isLua ? StripLuaComments(JassLines.Split(source)) : JassComments.Strip(JassLines.Split(source));

    /// <summary>The call edges between declared functions, shared with the call graph and the
    /// unused-function finder so no two passes disagree about what calls what.</summary>
    internal static Dictionary<string, SortedSet<string>> Edges(string[] code, IReadOnlyList<JassFunction> functions) =>
        JassCalls.Edges(code, functions, out _);

    /// <summary>Whitespace collapsed to single spaces and trimmed, so indentation is invisible.</summary>
    internal static string Normalize(string line) => WhitespaceRun.Replace(line, " ").Trim();

    /// <summary>
    /// One function's body as normalized lines, with comments dropped, whitespace collapsed and
    /// every local variable replaced by its declaration position. Two functions that differ only by
    /// a pure rename of their locals canonicalize to the same text, which is what makes a rename
    /// invisible to the semantic diff and lets duplicate body detection see a copied function.
    /// <paramref name="dropSignature"/> is true for duplicate bodies, where the signature line would
    /// otherwise make every function unique.
    /// </summary>
    internal static string[] CanonicalLines(
        string source, int[] lineStarts, JassFunction f, bool isLua, bool dropSignature)
    {
        int start = dropSignature ? f.StartLine + 1 : f.StartLine;
        var slice = ScriptCommand.SliceFunction(source, lineStarts, start, f.EndLine);
        var code = string.Join("\n", CodeLines(slice, isLua));
        var map = LocalMap(code, isLua);
        if (map.Count > 0) code = RewriteIdentifiers(code, isLua, map).Text;
        return JassLines.Split(code).Select(Normalize).ToArray();
    }

    /// <summary>The whole canonical body joined into one string, for grouping duplicate bodies.</summary>
    internal static string CanonicalBody(string source, int[] lineStarts, JassFunction f, bool isLua) =>
        string.Join("\n", CanonicalLines(source, lineStarts, f, isLua, dropSignature: true));

    /// <summary>Local variable names a function declares, in first-declaration order. A name
    /// declared twice keeps its first position, so the canonical token is stable.</summary>
    private static Dictionary<string, string> LocalMap(string bodyText, bool isLua)
    {
        var names = new List<string>();
        if (isLua)
        {
            foreach (var raw in JassLines.Split(bodyText))
            {
                var trimmed = raw.TrimStart();
                if (!trimmed.StartsWith("local ", StringComparison.Ordinal)) continue;
                var rest = trimmed["local ".Length..];
                if (rest.StartsWith("function", StringComparison.Ordinal)) continue;   // local function f
                int eq = rest.IndexOf('=');
                if (eq >= 0) rest = rest[..eq];
                foreach (var part in rest.Split(','))
                {
                    var m = Identifier.Match(part);
                    if (m.Success) names.Add(m.Value);
                }
            }
        }
        else
        {
            foreach (Match m in JassLocal.Matches(bodyText)) names.Add(m.Groups[1].Value);
        }

        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        int i = 0;
        foreach (var n in names)
            if (!map.ContainsKey(n)) map[n] = "\u0001L" + i++;
        return map;
    }

    /// <summary>
    /// Rewrites identifier tokens that are in <paramref name="map"/>, outside string literals and
    /// comments, and reports each hit with its 1-based line. Both the modernize pass and the local
    /// rename normalizer go through here so a name inside a string or a comment is never touched.
    /// Lua long strings and long comments use the simple <c>[[ ]]</c> form, a level-delimited
    /// <c>[=[</c> is not tracked.
    /// </summary>
    internal static (string Text, List<(string From, string To, int Line)> Hits) RewriteIdentifiers(
        string text, bool isLua, IReadOnlyDictionary<string, string> map)
    {
        var hits = new List<(string, string, int)>();
        if (map.Count == 0) return (text, hits);

        var sb = new StringBuilder(text.Length);
        int line = 1;
        int i = 0;
        bool inString = false, lineComment = false, blockComment = false;
        while (i < text.Length)
        {
            char c = text[i];
            if (c == '\n' || c == '\r')
            {
                line++; inString = false; lineComment = false; sb.Append(c); i++;
                continue;
            }
            if (lineComment) { sb.Append(c); i++; continue; }
            if (blockComment)
            {
                if (!isLua && c == '*' && Peek(text, i + 1) == '/') { sb.Append("*/"); i += 2; blockComment = false; continue; }
                if (isLua && c == ']' && Peek(text, i + 1) == ']') { sb.Append("]]"); i += 2; blockComment = false; continue; }
                sb.Append(c); i++; continue;
            }
            if (inString)
            {
                if (c == '\\' && i + 1 < text.Length) { sb.Append(c); sb.Append(text[i + 1]); i += 2; continue; }
                if (c == '"') inString = false;
                sb.Append(c); i++; continue;
            }

            if (c == '"') { inString = true; sb.Append(c); i++; continue; }
            if (!isLua && c == '/' && Peek(text, i + 1) == '/') { lineComment = true; sb.Append("//"); i += 2; continue; }
            if (!isLua && c == '/' && Peek(text, i + 1) == '*') { blockComment = true; sb.Append("/*"); i += 2; continue; }
            if (isLua && c == '-' && Peek(text, i + 1) == '-')
            {
                if (Peek(text, i + 2) == '[' && Peek(text, i + 3) == '[')
                { blockComment = true; sb.Append("--[["); i += 4; continue; }
                lineComment = true; sb.Append("--"); i += 2; continue;
            }

            if (char.IsLetter(c) || c == '_')
            {
                int j = i + 1;
                while (j < text.Length && (char.IsLetterOrDigit(text[j]) || text[j] == '_')) j++;
                var token = text[i..j];
                if (map.TryGetValue(token, out var to)) { sb.Append(to); hits.Add((token, to, line)); }
                else sb.Append(token);
                i = j; continue;
            }
            sb.Append(c); i++;
        }
        return (sb.ToString(), hits);
    }

    private static char Peek(string s, int i) => i < s.Length ? s[i] : '\0';

    /// <summary>Blanks Lua line and long comments out of already-split lines, keeping each line's
    /// position so a reported line number still points at the right source line.</summary>
    private static string[] StripLuaComments(string[] lines)
    {
        var result = new string[lines.Length];
        bool inLong = false;
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var sb = new StringBuilder(line.Length);
            for (int c = 0; c < line.Length; c++)
            {
                if (inLong)
                {
                    if (c + 1 < line.Length && line[c] == ']' && line[c + 1] == ']') { inLong = false; c++; }
                    continue;
                }
                if (c + 1 < line.Length && line[c] == '-' && line[c + 1] == '-')
                {
                    if (c + 3 < line.Length && line[c + 2] == '[' && line[c + 3] == '[')
                    { inLong = true; c += 3; continue; }
                    break;
                }
                sb.Append(line[c]);
            }
            result[i] = sb.ToString();
        }
        return result;
    }
}
