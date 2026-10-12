// src/Wc3.Commands/ScriptSemanticDiffCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// One real difference between two scripts. Kind is changed, added, removed, function-added or
/// function-removed. LineA and LineB are 1-based in their own file, 0 when the line does not exist
/// there. Text is the human-readable form of the change.
/// </summary>
public sealed record SemanticDiffLine(string Kind, int LineA, int LineB, string Text);

/// <summary>The real changes between two scripts, ignoring whitespace and pure local renames.</summary>
public sealed record ScriptSemanticDiffResult(
    string ScriptA,
    string ScriptB,
    string Language,
    int Changes,
    IReadOnlyList<SemanticDiffLine> Lines);

/// <summary>
/// Compares two map scripts function by function, line by line, after normalizing away noise that
/// is not a real change.
///
/// Whitespace is collapsed, comments are dropped, and every local variable is replaced by its
/// declaration position, so renaming <c>i</c> to <c>j</c> throughout a function is not reported.
/// What remains is compared line by line inside each function, with functions added or removed
/// reported whole. The point is a before/after of a port or an edit where the only intended changes
/// are the ones shown, and a variable rename or a reindent does not drown them.
/// </summary>
public static class ScriptSemanticDiffCommand
{
    public static ScriptSemanticDiffResult Execute(MapDocument a, MapDocument b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);
        var (fileA, sourceA, luaA) = ScriptAnalysis.Read(a);
        var (fileB, sourceB, luaB) = ScriptAnalysis.Read(b);
        return Analyze(fileA, sourceA, luaA, fileB, sourceB, luaB);
    }

    internal static ScriptSemanticDiffResult Analyze(
        string fileA, string sourceA, bool luaA,
        string fileB, string sourceB, bool luaB)
    {
        string language = luaA == luaB ? (luaA ? "lua" : "jass") : "mixed";

        var funcsA = ScriptAnalysis.Functions(sourceA, luaA);
        var funcsB = ScriptAnalysis.Functions(sourceB, luaB);
        var startsA = ScriptCommand.ComputeLineStarts(sourceA);
        var startsB = ScriptCommand.ComputeLineStarts(sourceB);
        var textA = JassLines.Split(sourceA);
        var textB = JassLines.Split(sourceB);

        var byNameA = FirstByName(funcsA);
        var byNameB = FirstByName(funcsB);

        var changes = new List<SemanticDiffLine>();
        var handled = new HashSet<string>(StringComparer.Ordinal);

        foreach (var fA in funcsA)
        {
            if (!byNameB.TryGetValue(fA.Name, out var fB)) continue;
            handled.Add(fA.Name);

            var ca = ScriptAnalysis.CanonicalLines(sourceA, startsA, fA, luaA, dropSignature: false);
            var cb = ScriptAnalysis.CanonicalLines(sourceB, startsB, fB, luaB, dropSignature: false);
            int max = Math.Max(ca.Length, cb.Length);
            for (int i = 0; i < max; i++)
            {
                bool hasA = i < ca.Length, hasB = i < cb.Length;
                string va = hasA ? ca[i] : "";
                string vb = hasB ? cb[i] : "";
                if (hasA && hasB && va == vb) continue;

                int lineA = hasA ? fA.StartLine + i : 0;
                int lineB = hasB ? fB.StartLine + i : 0;
                string rawA = hasA ? LineAt(textA, lineA) : "";
                string rawB = hasB ? LineAt(textB, lineB) : "";

                if (hasA && hasB)
                    changes.Add(new SemanticDiffLine("changed", lineA, lineB, rawA + "  becomes  " + rawB));
                else if (hasA)
                    changes.Add(new SemanticDiffLine("removed", lineA, 0, rawA));
                else
                    changes.Add(new SemanticDiffLine("added", 0, lineB, rawB));
            }
        }

        foreach (var fA in funcsA)
            if (!handled.Contains(fA.Name))
                changes.Add(new SemanticDiffLine("function-removed", fA.StartLine, 0, fA.Signature));

        foreach (var fB in funcsB)
            if (!byNameA.ContainsKey(fB.Name))
                changes.Add(new SemanticDiffLine("function-added", 0, fB.StartLine, fB.Signature));

        changes = changes
            .OrderBy(c => c.LineB == 0 ? c.LineA : c.LineB)
            .ThenBy(c => c.Kind, StringComparer.Ordinal)
            .ToList();

        return new(fileA, fileB, language, changes.Count, changes);
    }

    private static Dictionary<string, JassFunction> FirstByName(IReadOnlyList<JassFunction> functions)
    {
        var map = new Dictionary<string, JassFunction>(StringComparer.Ordinal);
        foreach (var f in functions)
            if (!map.ContainsKey(f.Name)) map[f.Name] = f;
        return map;
    }

    private static string LineAt(string[] lines, int n) =>
        n >= 1 && n <= lines.Length ? lines[n - 1].Trim() : "";
}
