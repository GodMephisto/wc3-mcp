// src/Wc3.Commands/ScriptRootsCommand.cs
using System.Text.RegularExpressions;
using Wc3.Model;

namespace Wc3.Commands;

public sealed record ScriptRoot(string Function, string Reason, int DeclaredAtLine);

public sealed record ScriptRootsResult(
    string ScriptFile,
    int FunctionsDeclared,
    int ReferencedByIdentifier,
    int NamedInStringLiteral,
    IReadOnlyList<ScriptRoot> Roots)
{
    /// <summary>Share of the script that a caller-graph pass alone would wrongly consider dead.</summary>
    public double RootPercent =>
        FunctionsDeclared == 0 ? 0 : Math.Round(100.0 * Roots.Count / FunctionsDeclared, 2);
}

/// <summary>
/// Finds the functions a dead-code pass must treat as ROOTS because nothing calls them by
/// identifier: they are invoked by name at runtime, <c>call ExecuteFunc("SomeFunc")</c>.
///
/// This is the prerequisite for cleaning a ported script by reachability instead of by the
/// porter's current line-by-line trimming. Trimming is what breaks ported maps: commenting out a
/// call it cannot resolve leaves a local declared and never assigned (the script then does not
/// compile, so a hosted map has no player slots), or strips an iterator's advance so
/// <c>exitwhen not X_hasNext(it)</c> can never change and the loop spins forever.
/// Carry complete, never trim, then strip what is genuinely unreachable - but a reachability pass
/// that cannot see string dispatch would delete these and break the map silently.
///
/// Measured on a 4,428-function arena map: 95 roots, 2.1%, all following a per-hero event
/// convention. Small and regular enough to hand to an optimizer, which is what makes the whole
/// strategy viable.
/// </summary>
public static class ScriptRootsCommand
{
    private static readonly Regex Declaration =
        new(@"^function\s+([A-Za-z_][A-Za-z0-9_]*)", RegexOptions.Multiline | RegexOptions.Compiled);
    /// <summary>A call or a function-pointer reference, i.e. reachable through the caller graph.</summary>
    private static readonly Regex IdentifierReference =
        new(@"\b(?:call|function)\s+([A-Za-z_][A-Za-z0-9_]*)", RegexOptions.Compiled);
    private static readonly Regex StringLiteral =
        new(@"""([^""\r\n]{1,120})""", RegexOptions.Compiled);
    /// <summary>The explicit form, worth reporting separately from an incidental name collision.</summary>
    private static readonly Regex ExecuteFuncLiteral =
        new(@"ExecuteFunc\s*\(\s*""([^""\r\n]+)""", RegexOptions.Compiled);

    public static ScriptRootsResult Run(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var entry = doc.GetFile("war3map.j") ?? doc.GetFile("scripts\\war3map.j");
        if (entry is null)
            return new ScriptRootsResult("(none)", 0, 0, 0, Array.Empty<ScriptRoot>());

        var text = ScriptText.GetString(entry.CurrentBytes);

        var declaredAt = new Dictionary<string, int>(StringComparer.Ordinal);
        int line = 1;
        foreach (var raw in text.Split('\n'))
        {
            if (Declaration.Match(raw) is { Success: true } m && !declaredAt.ContainsKey(m.Groups[1].Value))
                declaredAt[m.Groups[1].Value] = line;
            line++;
        }
        var declared = declaredAt.Keys.ToHashSet(StringComparer.Ordinal);

        // Blank the declaration headers so a function's own header is not counted as a reference
        // to itself, which would make every function look reachable.
        var body = Declaration.Replace(text, "function __DECL__");

        var byIdentifier = IdentifierReference.Matches(body).Select(m => m.Groups[1].Value)
            .ToHashSet(StringComparer.Ordinal);
        var inStrings = StringLiteral.Matches(body).Select(m => m.Groups[1].Value)
            .ToHashSet(StringComparer.Ordinal);
        var viaExecuteFunc = ExecuteFuncLiteral.Matches(body).Select(m => m.Groups[1].Value)
            .ToHashSet(StringComparer.Ordinal);

        var namedInString = declared.Intersect(inStrings).ToHashSet(StringComparer.Ordinal);

        var roots = namedInString
            .Where(f => !byIdentifier.Contains(f))
            .Select(f => new ScriptRoot(f,
                viaExecuteFunc.Contains(f)
                    ? "ExecuteFunc(\"...\") by name, no identifier reference anywhere"
                    : "appears only inside a string literal, no identifier reference anywhere",
                declaredAt.TryGetValue(f, out var l) ? l : 0))
            .OrderBy(r => r.Function, StringComparer.Ordinal)
            .ToList();

        return new ScriptRootsResult(entry.FileName ?? "war3map.j",
            declared.Count, declared.Intersect(byIdentifier).Count(), namedInString.Count, roots);
    }
}
