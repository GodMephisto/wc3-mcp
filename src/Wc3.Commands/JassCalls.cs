// src/Wc3.Commands/JassCalls.cs
using System.Text;
using System.Text.RegularExpressions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// The call edges a JASS script declares, shared by every pass that needs them, so the unused
/// function finder and the call graph cannot disagree about the syntax.
///
/// A call is a name followed by '(', which covers a statement call and a call nested inside an
/// expression (JASS uses the <c>call</c> keyword only for statement-level calls). A function
/// pointer is the word 'function' followed by a name, which is how a handler reaches the engine.
/// <c>ExecuteFunc("Name")</c> names a function in a string literal, the one route a caller-graph
/// pass cannot otherwise see. String literals are blanked before matching so a call-shaped word in
/// a message is not mistaken for a call.
/// </summary>
internal static class JassCalls
{
    private static readonly Regex Reference = new(
        @"\b([A-Za-z_][A-Za-z0-9_]*)\s*\(|\bfunction\s+([A-Za-z_][A-Za-z0-9_]*)",
        RegexOptions.Compiled);
    private static readonly Regex ExecuteFuncLiteral = new(
        @"\bExecuteFunc\s*\(\s*""([^""\r\n]+)""", RegexOptions.Compiled);
    private static readonly Regex Identifier = new(@"[A-Za-z_][A-Za-z0-9_]*", RegexOptions.Compiled);

    /// <summary>
    /// Every direct call edge between functions this script declares, caller to callee. A call to
    /// a name the script does not declare (a native, or a local holding a code value) is not an
    /// edge. <paramref name="executeFuncNames"/> are the names passed to ExecuteFunc.
    /// </summary>
    public static Dictionary<string, SortedSet<string>> Edges(
        string[] codeLines, IReadOnlyList<JassFunction> functions, out HashSet<string> executeFuncNames)
    {
        executeFuncNames = new HashSet<string>(StringComparer.Ordinal);
        for (int i = 0; i < codeLines.Length; i++)
            foreach (Match m in ExecuteFuncLiteral.Matches(codeLines[i]))
                executeFuncNames.Add(m.Groups[1].Value);

        var declared = new HashSet<string>(functions.Select(f => f.Name), StringComparer.Ordinal);
        var edges = new Dictionary<string, SortedSet<string>>(StringComparer.Ordinal);
        foreach (var f in functions)
        {
            var callees = new SortedSet<string>(StringComparer.Ordinal);
            for (int i = f.StartLine; i < f.EndLine - 1 && i < codeLines.Length; i++)
                foreach (Match m in Reference.Matches(Mask(codeLines[i])))
                {
                    var callee = m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value;
                    if (declared.Contains(callee)) callees.Add(callee);
                }
            edges[f.Name] = callees;
        }
        return edges;
    }

    /// <summary>Blank every string literal's contents in place, so a word inside a message is not
    /// read as a token. The quotes are kept so positions do not shift.</summary>
    public static string Mask(string s)
    {
        var sb = new StringBuilder(s);
        bool inString = false;
        for (int i = 0; i < sb.Length; i++)
        {
            if (inString && sb[i] == '\\') { sb[i] = ' '; if (i + 1 < sb.Length) sb[++i] = ' '; continue; }
            if (sb[i] == '"') { inString = !inString; continue; }
            if (inString) sb[i] = ' ';
        }
        return sb.ToString();
    }

    /// <summary>Every identifier in <paramref name="text"/>, string literals blanked first.</summary>
    public static IEnumerable<string> Identifiers(string text) =>
        Identifier.Matches(Mask(text)).Select(m => m.Value);
}
