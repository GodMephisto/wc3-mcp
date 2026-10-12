// src/Wc3.Commands/LuaCheckCommand.cs
using System.Text.RegularExpressions;
using Wc3.Commands.Lua;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One Lua file and whether it parses. When it does not, Line and Column point at the
/// error (Column is 0 when it could not be located).</summary>
public sealed record LuaFileCheck(
    string FileName, int Bytes, int Lines, bool Ok, int Line, int Column, string Message);

/// <summary>The result of parsing every Lua file a map carries.</summary>
public sealed record LuaCheckResult(bool Ok, int Files, int Errors, IReadOnlyList<LuaFileCheck> Checks);

/// <summary>
/// Parses every Lua file in a map and reports the syntax errors with the line and column, the way
/// the game's loader and luac 5.3 would refuse them. A Lua script that does not parse cannot run at
/// all, and the game shows no player slots and says nothing in its log, so the error has to be
/// found here.
///
/// This is a wider check than <c>script check</c>, which parses only the one entry the map runs as
/// its script. A map can carry several Lua modules, and a broken <c>require</c> target is just as
/// fatal as a broken main file. The parser is <see cref="LuaParser"/>, the same one every other Lua
/// command uses, so this cannot disagree with them about what parses.
/// </summary>
public static class LuaCheckCommand
{
    private static readonly Regex NearQuoted = new(@"near '(.*)'$", RegexOptions.Compiled);

    public static LuaCheckResult Execute(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var files = doc.Files
            .Where(f => f.FileName is not null
                        && f.FileName.EndsWith(".lua", StringComparison.OrdinalIgnoreCase))
            .GroupBy(f => f.FileName!, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .Select(f => (f.FileName!, f.CurrentBytes))
            .OrderBy(f => f.Item1, StringComparer.Ordinal)
            .ToList();
        return Analyze(files);
    }

    internal static LuaCheckResult Analyze(IReadOnlyList<(string Name, byte[] Bytes)> files)
    {
        var checks = new List<LuaFileCheck>();
        foreach (var (name, bytes) in files)
        {
            string source = ScriptCommand.ScriptEncoding.GetString(bytes);
            int lines = CountLines(source);
            var error = LuaParser.Check(source);
            if (error is null)
            {
                checks.Add(new LuaFileCheck(name, bytes.Length, lines, true, 0, 0, ""));
                continue;
            }
            int column = ColumnOf(source, error.Line, error.Detail);
            checks.Add(new LuaFileCheck(name, bytes.Length, lines, false, error.Line, column, error.Detail));
        }
        int errors = checks.Count(c => !c.Ok);
        return new LuaCheckResult(errors == 0, checks.Count, errors, checks);
    }

    /// <summary>Parses one source string and returns its single-file check, for a front end that
    /// holds unsaved editor text.</summary>
    public static LuaFileCheck Check(string fileName, string source)
    {
        var error = LuaParser.Check(source);
        return error is null
            ? new LuaFileCheck(fileName, ScriptCommand.ScriptEncoding.GetByteCount(source), CountLines(source), true, 0, 0, "")
            : new LuaFileCheck(fileName, ScriptCommand.ScriptEncoding.GetByteCount(source), CountLines(source),
                false, error.Line, ColumnOf(source, error.Line, error.Detail), error.Detail);
    }

    /// <summary>
    /// The 1-based column the parser's "near" token sits at on the reported line, or 0 when it
    /// cannot be located.
    ///
    /// The Lua parser reports line and message only, the way luac does, and luac's message names
    /// the token it stopped at ("unexpected symbol near ')'"). The column here is that token's
    /// first occurrence on the line, which is the error point for a token luac had not consumed.
    /// It is deliberately best effort, so it is 0 rather than a guess when the token is not on the
    /// line it names, and it is the line length plus one for the end of file.
    /// </summary>
    internal static int ColumnOf(string source, int line, string detail)
    {
        string text = LuaReaderLine(source, line);
        if (detail.EndsWith("near <eof>", StringComparison.Ordinal)) return text.Length + 1;
        var m = NearQuoted.Match(detail);
        if (!m.Success) return 0;
        string token = m.Groups[1].Value;
        if (token.Length > 3 && token.EndsWith("...", StringComparison.Ordinal))
            token = token[..^3];
        if (token.Length == 0) return 0;
        int at = text.IndexOf(token, StringComparison.Ordinal);
        return at < 0 ? 0 : at + 1;
    }

    private static string LuaReaderLine(string source, int line) =>
        LuaScriptReader.LineText(source, line);

    private static int CountLines(string source)
    {
        if (source.Length == 0) return 0;
        return LuaScriptReader.LineStarts(source).Length;
    }
}
