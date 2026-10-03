// src/Wc3.MapDocument/JassRawcodeAliases.cs
using System.Text.RegularExpressions;

namespace Wc3.Model;

/// <summary>
/// Globals that stand in for a rawcode, the <c>integer DarkShikiQ_ID= 'A1QZ'</c> idiom these maps
/// use everywhere. A handler almost never compares against the literal, it compares against the
/// global, so any analysis that only looks for <c>'A1QZ'</c> misses the dispatch entirely.
///
/// Shared between the porter (which seeds its script closure from these) and the wiring audit
/// (which resolves a dispatch through them), so the two cannot disagree about what an id means.
/// </summary>
public static class JassRawcodeAliases
{
    /// <summary>A global declaration with an initializer, "[constant] type Name = ...".</summary>
    private static readonly Regex GlobalInitializer = new(
        @"^\s*(?:constant\s+)?[A-Za-z_][A-Za-z0-9_]*\s+([A-Za-z_][A-Za-z0-9_]*)\s*=",
        RegexOptions.Compiled);

    private static readonly Regex RawcodeLiteral = new("'([^']{4})'", RegexOptions.Compiled);

    /// <summary>
    /// Global name to the 4-character rawcode it is initialized with. Only declarations OUTSIDE any
    /// function body count, so a local of the same name cannot be mistaken for a global alias.
    /// First declaration wins.
    /// </summary>
    public static Dictionary<string, string> Parse(string jass)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        if (string.IsNullOrEmpty(jass)) return result;

        // Normalizing CRLF was not enough, a bare CR is a line terminator too.
        var lines = JassLines.Split(jass);
        var inFunction = new bool[lines.Length];
        foreach (var f in JassFunctionIndex.Parse(jass))
            for (int i = f.StartLine - 1; i < f.EndLine && i < lines.Length; i++)
                inFunction[i] = true;

        for (int i = 0; i < lines.Length; i++)
        {
            if (inFunction[i]) continue;
            var line = StripLineComment(lines[i]);
            var g = GlobalInitializer.Match(line);
            if (!g.Success) continue;
            var lit = RawcodeLiteral.Match(line);
            if (lit.Success) result.TryAdd(g.Groups[1].Value, lit.Groups[1].Value);
        }
        return result;
    }

    private static string StripLineComment(string line)
    {
        int comment = line.IndexOf("//", StringComparison.Ordinal);
        return comment >= 0 ? line[..comment] : line;
    }
}
