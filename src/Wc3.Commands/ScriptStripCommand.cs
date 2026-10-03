// src/Wc3.Commands/ScriptStripCommand.cs
using System.Text;
using System.Text.RegularExpressions;
using Wc3.Model;

namespace Wc3.Commands;

public sealed record StripResult(
    int FunctionsBefore,
    int FunctionsAfter,
    int FunctionsRemoved,
    int LinesBefore,
    int LinesAfter,
    int EntryPoints,
    int StringRoots,
    IReadOnlyList<string> KeptRoots,
    string NewScript)
{
    public double PercentRemoved =>
        FunctionsBefore == 0 ? 0 : Math.Round(100.0 * FunctionsRemoved / FunctionsBefore, 1);
}

/// <summary>
/// Removes functions nothing can reach, by call-graph closure from the script's real entry
/// points plus every function invoked by name at runtime.
///
/// This is the alternative to the porter's line-by-line trimming, which is what actually breaks
/// ported maps: commenting out an unresolvable call leaves a local declared and never assigned
/// (so the script does not compile and a hosted map shows no player slots), or strips an
/// iterator's advance so <c>exitwhen not X_hasNext(it)</c> can never change and the loop spins
/// forever. Both were measured on a real port: 8 compile errors and 38 non-terminating loops that
/// the source map does not have.
///
/// Deleting a whole unreachable function cannot produce either failure, because nothing that
/// remains refers to it. The safety of the pass rests entirely on the root set being complete,
/// which is why string-dispatched names are collected first (see <see cref="ScriptRootsCommand"/>)
/// and why this is deliberately conservative: globals are never touched, and any function named
/// anywhere in a string literal is kept whether or not it looks reachable.
/// </summary>
public static class ScriptStripCommand
{
    /// <summary>Functions Warcraft III calls itself. Everything live hangs off one of these.</summary>
    private static readonly string[] EngineEntryPoints =
    {
        "main", "config", "InitGlobals", "InitCustomTriggers", "InitCustomTeams",
        "InitAllyPriorities", "CreateAllUnits", "CreateNeutralPassiveBuildings",
        "CreateNeutralHostileBuildings", "CreatePlayerBuildings", "CreatePlayerUnits",
        "CreateAllItems", "CreateRegions", "CreateCameras", "InitSounds", "InitUpgrades",
        "InitTechTree", "InitBlizzard", "RunInitializationTriggers",
    };

    private static readonly Regex Declaration =
        new(@"^function\s+([A-Za-z_][A-Za-z0-9_]*)", RegexOptions.Compiled);
    private static readonly Regex EndFunction = new(@"^endfunction", RegexOptions.Compiled);
    /// <summary>
    /// Any reference that can reach a function: a name followed by '(' , or 'function X' for a
    /// function pointer. JASS uses the <c>call</c> keyword ONLY for statement-level calls; a call
    /// inside an expression, <c>if IsValid(u) then</c>, carries no keyword at all.
    /// Requiring <c>call</c> here was a genuine semantics error that made this pass delete live
    /// code and took a script from 8 compile errors to 50. For a pass that DELETES, over-matching
    /// is free and under-matching is catastrophic, so every name-then-paren counts as a reference
    /// even though many will be natives or locals.
    /// </summary>
    private static readonly Regex IdentifierReference =
        new(@"\b([A-Za-z_][A-Za-z0-9_]*)\s*\(|\bfunction\s+([A-Za-z_][A-Za-z0-9_]*)",
            RegexOptions.Compiled);

    private static string CalleeOf(Match m) =>
        m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value;

    private static readonly Regex StringLiteral =
        new(@"""([^""\r\n]{1,120})""", RegexOptions.Compiled);

    public static StripResult Run(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var entry = doc.GetFile("war3map.j") ?? doc.GetFile("scripts\\war3map.j")
            ?? throw new InvalidOperationException("Map has no war3map.j to strip.");

        var text = ScriptText.GetString(entry.CurrentBytes);
        var lines = text.Split('\n');

        // Function spans. Everything outside them (globals, comments, the header) is preserved
        // verbatim: stripping a global is far riskier than stripping a function, and functions are
        // where the bulk of a bloated script lives anyway.
        var spans = new List<(string name, int start, int end)>();
        string? open = null; int openAt = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            if (open is null)
            {
                if (Declaration.Match(lines[i]) is { Success: true } m)
                { open = m.Groups[1].Value; openAt = i; }
            }
            else if (EndFunction.IsMatch(lines[i]))
            {
                spans.Add((open, openAt, i));
                open = null;
            }
        }

        var bodyOf = spans.ToDictionary(s => s.name, s => string.Join("\n",
            lines.Skip(s.start).Take(s.end - s.start + 1)), StringComparer.Ordinal);
        var declared = bodyOf.Keys.ToHashSet(StringComparer.Ordinal);

        // Roots. Anything named in ANY string literal is kept, not just ExecuteFunc arguments,
        // because a name can reach the engine through a variable or a helper and the cost of
        // keeping a few extra functions is nothing next to deleting a live one.
        var stringNamed = StringLiteral.Matches(text).Select(m => m.Groups[1].Value)
            .Where(declared.Contains).ToHashSet(StringComparer.Ordinal);

        // References that live OUTSIDE any function body, chiefly the globals block, e.g.
        // 'code onCast = function Foo'. Omitting these was a real bug in the first version of this
        // pass: it scanned only reachable function bodies, so a function referenced solely from a
        // global initialiser looked dead and was deleted, taking a compiling script from 8 errors
        // to 50. Any pass that deletes code has to see EVERY reference, not most of them.
        var insideFunction = new bool[lines.Length];
        foreach (var (_, start, end) in spans)
            for (int i = start; i <= end; i++) insideFunction[i] = true;
        var outsideText = new StringBuilder();
        for (int i = 0; i < lines.Length; i++)
            if (!insideFunction[i]) outsideText.Append(lines[i]).AppendLine();
        var referencedOutside = IdentifierReference.Matches(outsideText.ToString())
            .Select(m => m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value)
            .Where(declared.Contains)
            .ToHashSet(StringComparer.Ordinal);

        var roots = new HashSet<string>(StringComparer.Ordinal);
        roots.UnionWith(referencedOutside);
        // Exact match only. A prefix rule here was a real mistake: "InitTrig_" as a root pattern
        // kept 397 of them, every other hero's trigger initialiser, each dragging in its own
        // closure. An InitTrig_ function is live only if something actually calls it, and a port
        // that trimmed InitCustomTriggers to one line calls almost none of them.
        foreach (var name in declared)
            if (EngineEntryPoints.Contains(name, StringComparer.Ordinal))
                roots.Add(name);
        int engineRoots = roots.Count;
        roots.UnionWith(stringNamed);

        // Closure over identifier references inside kept bodies.
        var reachable = new HashSet<string>(StringComparer.Ordinal);
        var queue = new Queue<string>(roots);
        while (queue.Count > 0)
        {
            var fn = queue.Dequeue();
            if (!reachable.Add(fn) || !bodyOf.TryGetValue(fn, out var body)) continue;
            foreach (Match m in IdentifierReference.Matches(body))
            {
                var callee = m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value;
                if (declared.Contains(callee) && !reachable.Contains(callee)) queue.Enqueue(callee);
            }
        }

        var drop = new bool[lines.Length];
        int removed = 0;
        foreach (var (name, start, end) in spans)
        {
            if (reachable.Contains(name)) continue;
            for (int i = start; i <= end; i++) drop[i] = true;
            removed++;
        }

        var kept = new StringBuilder();
        int keptLines = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            if (drop[i]) continue;
            kept.Append(lines[i]);
            if (i < lines.Length - 1) kept.Append('\n');
            keptLines++;
        }

        return new StripResult(spans.Count, spans.Count - removed, removed,
            lines.Length, keptLines, engineRoots, stringNamed.Count,
            roots.OrderBy(r => r, StringComparer.Ordinal).Take(25).ToList(), kept.ToString());
    }
}
