// src/Wc3.Commands/JassRawcodes.cs
using System.Text.RegularExpressions;
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Finding object rawcodes in a JASS script. A rawcode is written 'n015', as the same four bytes
/// in hex ($6E303135, how YDWE and optimised maps print ids), or as an integer global that holds
/// either. Shared by the preload repair and the ability inspector so both read a script alike.
/// </summary>
internal static class JassRawcodes
{
    /// <summary>One token per match. Group 1 a quoted rawcode, 2 a hex rawcode, 3 an identifier.</summary>
    internal static readonly Regex Token = new(@"'([^']{4})'|\$([0-9A-Fa-f]{8})\b|\b([A-Za-z_]\w*)\b", RegexOptions.Compiled);

    private static readonly Regex IntegerGlobal = new(@"^\s*(?:constant\s+)?integer\s+(\w+)\s*=\s*'([^']{4})'",
        RegexOptions.Compiled);
    private static readonly Regex IntegerGlobalHex = new(@"^\s*(?:constant\s+)?integer\s+(\w+)\s*=\s*\$([0-9A-Fa-f]{8})\b",
        RegexOptions.Compiled);

    /// <summary>The integer globals that hold a rawcode, by name. The first assignment wins.</summary>
    internal static Dictionary<string, string> Globals(IEnumerable<string> lines)
    {
        var globals = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var line in lines)
        {
            if (IntegerGlobal.Match(line) is { Success: true } g) globals.TryAdd(g.Groups[1].Value, g.Groups[2].Value);
            else if (IntegerGlobalHex.Match(line) is { Success: true } h && FromHex(h.Groups[2].Value) is { } hx)
                globals.TryAdd(h.Groups[1].Value, hx);
        }
        return globals;
    }

    /// <summary>Every rawcode one line of code names, directly or through a global.</summary>
    internal static IEnumerable<string> In(string code, IReadOnlyDictionary<string, string> globals)
    {
        foreach (Match t in Token.Matches(code))
        {
            if (t.Groups[1].Success) yield return t.Groups[1].Value;
            else if (t.Groups[2].Success) { if (FromHex(t.Groups[2].Value) is { } hx) yield return hx; }
            else if (globals.TryGetValue(t.Groups[3].Value, out var raw)) yield return raw;
        }
    }

    /// <summary>$6E303135 as the rawcode 'n015', when all four bytes are printable.</summary>
    internal static string? FromHex(string hex)
    {
        var chars = new char[4];
        for (int i = 0; i < 4; i++)
        {
            int b = Convert.ToInt32(hex.Substring(i * 2, 2), 16);
            if (b < 0x20 || b > 0x7E) return null;
            chars[i] = (char)b;
        }
        return new string(chars);
    }

    /// <summary>The line without its // comment. A // inside a string literal is kept.</summary>
    internal static string StripComment(string line)
    {
        bool inString = false;
        for (int i = 0; i < line.Length - 1; i++)
        {
            char c = line[i];
            if (c == '\\' && inString) { i++; continue; }
            if (c == '"') inString = !inString;
            else if (!inString && c == '/' && line[i + 1] == '/') return line[..i];
        }
        return line;
    }

    /// <summary>
    /// The rawcodes the map itself defines for a kind. An optimised map keeps its objects in SLK
    /// tables inside the archive instead of in war3map.w3u or war3map.w3a, so both are read.
    /// </summary>
    internal static HashSet<string> MapDefined(MapDocument doc, ObjectKind kind, string slkName) =>
        ObjectKinds.MergedEntries(doc, ObjectKinds.Info(kind))
            .Select(e => e.Id.ToRawcode()).Concat(SlkIds(doc, slkName))
            .ToHashSet(StringComparer.Ordinal);

    internal static IEnumerable<string> SlkIds(MapDocument doc, string name)
    {
        if (!doc.TryReadFileByName(name, out var bytes) || bytes.Length == 0) return Array.Empty<string>();
        try { return GameData.SlkTable.Parse(bytes).RowKeys.Where(k => k.Length == 4).ToList(); }
        catch (Exception) { return Array.Empty<string>(); }
    }
}
