// src/Wc3.Commands/ScriptComplexityCommand.cs
using System.Text.RegularExpressions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One function's decision points. Line is the function's 1-based declaration line.</summary>
public sealed record FunctionComplexity(string Function, int Complexity, int Line, int EndLine);

public sealed record ScriptComplexityResult(
    string ScriptFile,
    int FunctionCount,
    int TotalDecisions,
    IReadOnlyList<FunctionComplexity> Functions);

/// <summary>
/// Measures a function's cyclomatic complexity as the number of branching keywords in its body,
/// then ranks the whole script worst first.
///
/// The keywords are <c>if</c>, <c>loop</c>, <c>and</c> and <c>or</c>, each counted at word
/// boundaries so <c>elseif</c> is one keyword, not two, and <c>endloop</c>/<c>endif</c> are none.
/// A function with no branch has complexity zero; the classic +1 baseline is not added, so the
/// number is the count of decisions rather than the count of paths. It is the same ordering
/// either way, and the raw count is the one to compare across functions.
///
/// Built to answer "where is the complexity" on a large script. The worst function is the one a
/// reader, or a fix, should look at first.
/// </summary>
public static class ScriptComplexityCommand
{
    private static readonly Regex Branch = new(@"\b(?:if|loop|and|or)\b", RegexOptions.Compiled);

    public static ScriptComplexityResult Execute(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        if (!UabiRuntimeRepairCommand.TryReadScript(doc, out var name, out var script, out _))
            return new(name, 0, 0, Array.Empty<FunctionComplexity>());
        return Analyze(name, script);
    }

    internal static ScriptComplexityResult Analyze(string scriptFile, string script)
    {
        var code = JassComments.Strip(JassLines.Split(script));
        var functions = JassFunctionIndex.Parse(script);

        var rows = new List<FunctionComplexity>(functions.Count);
        int total = 0;
        foreach (var f in functions)
        {
            int c = 0;
            for (int i = f.StartLine; i < f.EndLine - 1 && i < code.Length; i++)
                c += Branch.Matches(JassCalls.Mask(code[i])).Count;
            total += c;
            rows.Add(new FunctionComplexity(f.Name, c, f.StartLine, f.EndLine));
        }

        rows = rows.OrderByDescending(r => r.Complexity).ThenBy(r => r.Line).ToList();
        return new(scriptFile, functions.Count, total, rows);
    }
}
