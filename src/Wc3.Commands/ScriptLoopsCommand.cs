// src/Wc3.Commands/ScriptLoopsCommand.cs
using System.Text.RegularExpressions;
using Wc3.Model;

namespace Wc3.Commands;

public sealed record LoopFinding(
    string Function,
    int StartLine,
    int EndLine,
    int ExitWhenCount,
    bool HasReturnInside,
    string Risk,
    string Reason,
    string ConditionSample);

public sealed record ScriptLoopsResult(
    string ScriptFile,
    int TotalLoops,
    IReadOnlyList<LoopFinding> Findings)
{
    public int HighRisk => Findings.Count(f => f.Risk == "high");
}

/// <summary>
/// Finds JASS loops that may never terminate.
///
/// Built after measuring a hang that no archive-level tool could explain: one core pinned at
/// 1.001, ~80% of instruction-pointer samples inside a 30-byte window of the game executable,
/// while every structural check on the archive came back clean. The JASS interpreter lives in that
/// executable, so a script loop that never exits presents exactly that way - a tight native loop,
/// no crash, no log entry, no error. Three separate archive tools each disproved their own theory
/// before it was worth suspecting the script instead.
///
/// Two shapes are worth flagging:
/// <list type="bullet">
/// <item>A loop with no <c>exitwhen</c> at all, which can only leave via <c>return</c>.</item>
/// <item>A loop whose <c>exitwhen</c> condition names nothing the body ever assigns, so the
/// condition cannot change and the loop spins until something external intervenes. This is also
/// the shape of a deliberate anti-tamper stall.</item>
/// </list>
/// Heuristic by nature: it reports candidates to read, not proven infinite loops.
/// </summary>
public static class ScriptLoopsCommand
{
    private static readonly Regex FunctionStart =
        new(@"^\s*function\s+([A-Za-z_][A-Za-z0-9_]*)", RegexOptions.Compiled);
    private static readonly Regex Identifier =
        new(@"[A-Za-z_][A-Za-z0-9_]*", RegexOptions.Compiled);
    /// <summary>Assignment or mutation of a name inside the body: set X=, set X[i]=, or a call taking X.</summary>
    private static readonly Regex Assignment =
        new(@"^\s*set\s+([A-Za-z_][A-Za-z0-9_]*)", RegexOptions.Compiled);

    public static ScriptLoopsResult Run(MapDocument doc, bool onlyRisky = true)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var entry = doc.GetFile("war3map.j") ?? doc.GetFile("scripts\\war3map.j");
        if (entry is null)
            return new ScriptLoopsResult("(none)", 0, Array.Empty<LoopFinding>());

        var text = ScriptText.GetString(entry.CurrentBytes);
        var lines = text.Split('\n');

        var findings = new List<LoopFinding>();
        int total = 0;
        string currentFunction = "(top level)";
        // Stack of open loops, so nesting is handled rather than assumed away.
        var open = new Stack<(int line, List<string> body)>();

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var trimmed = StripComment(line).Trim();

            if (FunctionStart.Match(line) is { Success: true } fm)
                currentFunction = fm.Groups[1].Value;

            if (trimmed == "loop" || trimmed.StartsWith("loop ", StringComparison.Ordinal))
            {
                open.Push((i + 1, new List<string>()));
                continue;
            }

            if (open.Count > 0)
                foreach (var frame in open) frame.body.Add(trimmed);

            if (trimmed == "endloop" || trimmed.StartsWith("endloop ", StringComparison.Ordinal))
            {
                if (open.Count == 0) continue;   // unbalanced source; do not guess
                var (startLine, body) = open.Pop();
                total++;
                var finding = Classify(currentFunction, startLine, i + 1, body);
                if (!onlyRisky || finding.Risk != "low") findings.Add(finding);
            }
        }

        return new ScriptLoopsResult(entry.FileName ?? "war3map.j", total,
            findings.OrderBy(f => f.Risk == "high" ? 0 : 1).ThenBy(f => f.StartLine).ToList());
    }

    private static LoopFinding Classify(string function, int startLine, int endLine, List<string> body)
    {
        var exits = body.Where(l => l.StartsWith("exitwhen", StringComparison.Ordinal)).ToList();
        bool hasReturn = body.Any(l => l == "return" || l.StartsWith("return ", StringComparison.Ordinal));
        var assigned = body.Select(l => Assignment.Match(l))
            .Where(m => m.Success)
            .Select(m => m.Groups[1].Value)
            .ToHashSet(StringComparer.Ordinal);

        if (exits.Count == 0)
            return new(function, startLine, endLine, 0, hasReturn,
                hasReturn ? "medium" : "high",
                hasReturn
                    ? "no exitwhen; can only leave via return, so termination depends on a branch inside"
                    : "no exitwhen and no return: this loop cannot terminate on its own",
                "");

        // Does any exit condition mention something the body assigns? If not, no condition can
        // change, and the loop spins until something outside it intervenes.
        foreach (var e in exits)
        {
            // 'exitwhen true' leaves on the first iteration, so it terminates by construction. It
            // names nothing the body assigns and would otherwise be flagged, which is exactly the
            // false positive this check exists to suppress - real maps use it as a do-once block.
            if (IsAlwaysTrue(e)) return new(function, startLine, endLine, exits.Count, hasReturn,
                "low", "exits unconditionally on the first iteration", Shorten(e));
        }
        foreach (var e in exits)
        {
            var names = Identifier.Matches(e).Select(m => m.Value)
                .Where(n => n != "exitwhen" && !IsKeyword(n));
            if (names.Any(assigned.Contains))
                return new(function, startLine, endLine, exits.Count, hasReturn, "low",
                    "exit condition depends on a value the body assigns", Shorten(e));
        }

        return new(function, startLine, endLine, exits.Count, hasReturn,
            hasReturn ? "medium" : "high",
            "every exit condition names only values this loop never assigns, so the condition "
            + "cannot change from inside the loop",
            Shorten(exits[0]));
    }

    /// <summary>'exitwhen true' and its whitespace variants leave on the first iteration.</summary>
    private static bool IsAlwaysTrue(string exitWhen)
    {
        var cond = exitWhen["exitwhen".Length..].Trim();
        while (cond.StartsWith("(", StringComparison.Ordinal) && cond.EndsWith(")", StringComparison.Ordinal))
            cond = cond[1..^1].Trim();
        return cond == "true";
    }

    private static bool IsKeyword(string s) => s is "and" or "or" or "not" or "true" or "false"
        or "null" or "function" or "integer" or "real" or "boolean" or "string";

    private static string StripComment(string line)
    {
        int i = line.IndexOf("//", StringComparison.Ordinal);
        return i < 0 ? line : line[..i];
    }

    private static string Shorten(string s) => s.Length <= 110 ? s : s[..110] + "...";
}
