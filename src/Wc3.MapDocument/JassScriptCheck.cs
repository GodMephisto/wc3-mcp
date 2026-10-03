// src/Wc3.MapDocument/JassScriptCheck.cs
using System.Text;
using System.Text.RegularExpressions;

namespace Wc3.Model;

public enum JassIssueKind
{
    /// <summary>An assignment to a name that is not declared anywhere. A hard compile error.</summary>
    UndeclaredAssignmentTarget,
    /// <summary>A local declaration this tool commented out whose variable is still read. A hard
    /// compile error, and the one that repeatedly emptied host lobbies. Mechanically repairable.</summary>
    DroppedLocalDeclaration,
    DuplicateFunction,
    DuplicateGlobal,
    UnbalancedBlock,
    /// <summary>No <c>config</c> or no <c>main</c>. The game calls both, so the map cannot host.</summary>
    MissingEntryPoint,
    /// <summary><c>config</c> exists but declares no player slots, so the lobby shows none.</summary>
    NoPlayerSlots,
}

/// <summary>One problem found in a script. <paramref name="Line"/> is 1-based, 0 when not line-bound.</summary>
public sealed record JassIssue(JassIssueKind Kind, DiagnosticSeverity Severity, int Line, string Message);

/// <summary>
/// Checks whether a JASS script would still compile, and repairs the damage this tool itself can do.
///
/// Why this exists: in JASS a single undeclared variable fails the whole <c>war3map.j</c>, so
/// <c>config()</c> never runs and a hosted map shows no player slots. A port that trims calls to
/// functions it did not carry can leave exactly that behind, and until now nothing verified the
/// result, so a broken map saved silently and only failed in the lobby.
///
/// Every check here is chosen to need no list of natives, so a missing game install can never turn
/// into a false positive. The strongest signal is an assignment target, because a native can never
/// be assigned to, so an undeclared one is unambiguous. <see cref="PjassGate"/> is the deeper,
/// optional pass that does use the real parser.
/// </summary>
public static class JassScriptCheck
{
    /// <summary>Marker this tool writes when it comments a statement out. Owned here (the lowest
    /// layer that reasons about it) so the porter and the checker share one spelling.</summary>
    public const string TrimMarker = "//[wc3ctl trimmed] ";

    /// <summary>A local declaration that was commented out entirely, so its variable is gone.
    /// The repairable break. The healthy form keeps the declaration and puts the marker AFTER it
    /// ("local real x //[wc3ctl trimmed] (initializer dropped)"), which this deliberately misses.</summary>
    private static readonly Regex DroppedLocal = new(
        @"^(\s*)//\[wc3ctl trimmed\]\s*(local\b[^=\r\n]*)(=.*)?$", RegexOptions.Compiled);

    private static readonly Regex SetTarget = new(
        @"^set\s+([A-Za-z_][A-Za-z0-9_]*)", RegexOptions.Compiled);

    private static readonly Regex Ident = new(@"[A-Za-z_][A-Za-z0-9_]*", RegexOptions.Compiled);

    /// <summary>
    /// Finds the problems in <paramref name="jass"/>. <paramref name="externalGlobals"/> optionally
    /// adds globals declared outside the map (common.j / Blizzard.j), which the caller can supply
    /// from the game install. Without it, names beginning <c>bj_</c> are still treated as external,
    /// since that is the Blizzard.j convention, so their assignments never read as undeclared.
    /// </summary>
    public static IReadOnlyList<JassIssue> Check(string jass, IReadOnlySet<string>? externalGlobals = null)
    {
        var issues = new List<JassIssue>();
        if (string.IsNullOrWhiteSpace(jass)) return issues;

        // Normalizing CRLF was not enough, a bare CR is a line terminator too, and a script
        // that split into almost no lines produced almost no findings, which read as clean.
        var lines = JassLines.Split(jass);
        var code = StripComments(lines);                 // comment-free view, same indices
        var (globals, _) = JassGlobals.Parse(lines);
        var functions = JassFunctionIndex.Parse(jass);

        bool Known(string name) =>
            globals.ContainsKey(name)
            || name.StartsWith("bj_", StringComparison.Ordinal)
            || (externalGlobals?.Contains(name) ?? false);

        // Duplicate definitions. Two of the same name is a compile error, and a merge (a port
        // splicing a second script in) is exactly how one appears.
        foreach (var dup in functions.GroupBy(f => f.Name, StringComparer.Ordinal).Where(g => g.Count() > 1))
            issues.Add(new(JassIssueKind.DuplicateFunction, DiagnosticSeverity.Error,
                dup.Skip(1).First().StartLine, $"function '{dup.Key}' is defined {dup.Count()} times"));

        foreach (var dupGlobal in DuplicateGlobalNames(lines))
            issues.Add(new(JassIssueKind.DuplicateGlobal, DiagnosticSeverity.Error,
                dupGlobal.Line, $"global '{dupGlobal.Name}' is declared more than once"));

        foreach (var f in functions)
        {
            var declared = new HashSet<string>(ParamNames(f.Signature), StringComparer.Ordinal);
            // JASS requires every local up front, so one pass over the body collects them all
            // before any statement that could reference one.
            for (int i = f.StartLine - 1; i < f.EndLine && i < code.Length; i++)
            {
                var t = code[i].TrimStart();
                if (!t.StartsWith("local ", StringComparison.Ordinal)) continue;
                if (JassGlobals.NameOf(t["local ".Length..]) is { } local) declared.Add(local);
            }

            int ifDepth = 0, loopDepth = 0;
            for (int i = f.StartLine - 1; i < f.EndLine && i < code.Length; i++)
            {
                var t = code[i].Trim();
                if (t.Length == 0) continue;

                if (StartsWithWord(t, "if")) ifDepth++;
                else if (StartsWithWord(t, "endif")) ifDepth--;
                else if (StartsWithWord(t, "loop")) loopDepth++;
                else if (StartsWithWord(t, "endloop")) loopDepth--;

                if (SetTarget.Match(t) is { Success: true } m)
                {
                    var target = m.Groups[1].Value;
                    if (!declared.Contains(target) && !Known(target))
                        issues.Add(new(JassIssueKind.UndeclaredAssignmentTarget, DiagnosticSeverity.Error,
                            i + 1, $"'{target}' is assigned in {f.Name} but never declared"));
                }
            }

            if (ifDepth != 0)
                issues.Add(new(JassIssueKind.UnbalancedBlock, DiagnosticSeverity.Error, f.StartLine,
                    $"{f.Name} has unbalanced if/endif (depth {ifDepth} at endfunction)"));
            if (loopDepth != 0)
                issues.Add(new(JassIssueKind.UnbalancedBlock, DiagnosticSeverity.Error, f.StartLine,
                    $"{f.Name} has unbalanced loop/endloop (depth {loopDepth} at endfunction)"));
        }

        issues.AddRange(DroppedLocalIssues(lines, code, functions));

        // Host prerequisites. The game calls config() to build the lobby and main() to start the
        // map, so a script missing either cannot be hosted at all. Only meaningful once the script
        // declares at least one function, a payload with none is not JASS to begin with (or is
        // unreadable), which the loader and empty-file checks describe far better than this would.
        if (functions.Count > 0)
            foreach (var entry in new[] { "config", "main" })
                if (!functions.Any(f => f.Name == entry))
                    issues.Add(new(JassIssueKind.MissingEntryPoint, DiagnosticSeverity.Error, 0,
                        $"no '{entry}' function, the map cannot host"));

        if (functions.FirstOrDefault(f => f.Name == "config") is { } cfg)
        {
            bool slots = false;
            for (int i = cfg.StartLine - 1; i < cfg.EndLine && i < code.Length && !slots; i++)
                slots = code[i].Contains("SetPlayers", StringComparison.Ordinal)
                     || code[i].Contains("InitCustomPlayerSlots", StringComparison.Ordinal)
                     || code[i].Contains("DefineStartLocation", StringComparison.Ordinal);
            if (!slots)
                issues.Add(new(JassIssueKind.NoPlayerSlots, DiagnosticSeverity.Warning, cfg.StartLine,
                    "config() declares no player slots, the host lobby may show none"));
        }

        return issues.OrderBy(i => i.Line).ToList();
    }

    /// <summary>True when the script has no issue of <see cref="DiagnosticSeverity.Error"/>.</summary>
    public static bool IsClean(IEnumerable<JassIssue> issues) =>
        !issues.Any(i => i.Severity == DiagnosticSeverity.Error);

    /// <summary>
    /// Whether this kind stops the script from compiling, as opposed to describing what the finished
    /// map needs in order to host. The distinction matters because the two have different audiences,
    /// a map validator cares that <c>config</c> exists, while splicing a script fragment must only
    /// insist that the result still compiles, and a fragment legitimately has no entry points.
    /// </summary>
    public static bool BlocksCompilation(JassIssueKind kind) => kind
        is JassIssueKind.UndeclaredAssignmentTarget
        or JassIssueKind.DroppedLocalDeclaration
        or JassIssueKind.DuplicateFunction
        or JassIssueKind.DuplicateGlobal
        or JassIssueKind.UnbalancedBlock;

    /// <summary>True when nothing would stop the script from compiling. Host-readiness findings
    /// (a missing entry point, no player slots) are deliberately not considered here.</summary>
    public static bool IsCompilable(IEnumerable<JassIssue> issues) =>
        !issues.Any(i => i.Severity == DiagnosticSeverity.Error && BlocksCompilation(i.Kind));

    /// <summary>
    /// Repairs the mechanically fixable breakage, a local declaration this tool commented out, by
    /// restoring the declaration in place and keeping only its initializer dropped. The variable
    /// takes its type default (null/0/false) instead of a value from a function that was not
    /// carried, which compiles and is what the trimming intended in the first place. Line count and
    /// indentation are preserved, so every other line number stays valid.
    /// </summary>
    public static string Repair(string jass, out int repairs)
    {
        repairs = 0;
        if (string.IsNullOrEmpty(jass)) return jass;

        string nl = jass.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";
        // Normalizing CRLF was not enough, a bare CR is a line terminator too, and a script
        // that split into almost no lines produced almost no findings, which read as clean.
        var lines = JassLines.Split(jass);
        for (int i = 0; i < lines.Length; i++)
        {
            var m = DroppedLocal.Match(lines[i]);
            if (!m.Success) continue;
            lines[i] = m.Groups[1].Value + m.Groups[2].Value.TrimEnd()
                     + " " + TrimMarker + "(initializer dropped)";
            repairs++;
        }
        return repairs == 0 ? jass : string.Join(nl, lines);
    }

    /// <summary>A commented-out local whose variable is still referenced later in the same function.</summary>
    private static IEnumerable<JassIssue> DroppedLocalIssues(
        string[] lines, string[] code, IReadOnlyList<JassFunction> functions)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            var m = DroppedLocal.Match(lines[i]);
            if (!m.Success) continue;
            var name = JassGlobals.NameOf(m.Groups[2].Value["local ".Length..].TrimStart());
            if (name is null) continue;

            var owner = functions.FirstOrDefault(f => i + 1 >= f.StartLine && i + 1 <= f.EndLine);
            int end = owner?.EndLine ?? lines.Length;
            int uses = 0;
            for (int j = i + 1; j < end && j < code.Length; j++)
                foreach (Match id in Ident.Matches(code[j]))
                    if (id.Value == name) uses++;

            if (uses > 0)
                yield return new(JassIssueKind.DroppedLocalDeclaration, DiagnosticSeverity.Error, i + 1,
                    $"declaration of '{name}' was commented out but it is still used {uses} time(s)"
                    + (owner is null ? "" : $" in {owner.Name}") + ", so the script cannot compile");
        }
    }

    private static IEnumerable<(string Name, int Line)> DuplicateGlobalNames(string[] lines)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        bool inBlock = false;
        for (int i = 0; i < lines.Length; i++)
        {
            var t = lines[i].Trim();
            if (!inBlock) { if (t == "globals") inBlock = true; continue; }
            if (t == "endglobals") { inBlock = false; continue; }
            if (t.Length == 0 || t.StartsWith("//", StringComparison.Ordinal)) continue;
            if (JassGlobals.NameOf(t) is { } name && !seen.Add(name))
                yield return (name, i + 1);
        }
    }

    /// <summary>The parameter names out of "function F takes unit u,real x returns nothing".</summary>
    private static IEnumerable<string> ParamNames(string signature)
    {
        int takes = signature.IndexOf(" takes ", StringComparison.Ordinal);
        if (takes < 0) yield break;
        int returns = signature.IndexOf(" returns ", takes, StringComparison.Ordinal);
        var list = returns < 0 ? signature[(takes + 7)..] : signature[(takes + 7)..returns];
        if (list.Trim() is "nothing" or "") yield break;
        foreach (var part in list.Split(','))
        {
            var toks = part.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            if (toks.Length >= 2) yield return toks[^1];
        }
    }

    /// <summary>The script with comments blanked out, same line count and indices, so a "set" or an
    /// identifier inside a comment (or a commented-out statement this tool wrote) is never mistaken
    /// for code. Block comments are tracked across lines; string literals are left alone, which is
    /// safe because no check keys off a statement that could hide inside one.</summary>
    private static string[] StripComments(string[] lines)
    {
        var result = new string[lines.Length];
        bool inBlock = false;
        for (int i = 0; i < lines.Length; i++)
        {
            var sb = new StringBuilder(lines[i].Length);
            var line = lines[i];
            for (int c = 0; c < line.Length; c++)
            {
                if (inBlock)
                {
                    if (c + 1 < line.Length && line[c] == '*' && line[c + 1] == '/') { inBlock = false; c++; }
                    else sb.Append(' ');
                    continue;
                }
                if (c + 1 < line.Length && line[c] == '/' && line[c + 1] == '/') break;      // line comment
                if (c + 1 < line.Length && line[c] == '/' && line[c + 1] == '*')
                { inBlock = true; c++; continue; }
                sb.Append(line[c]);
            }
            result[i] = sb.ToString();
        }
        return result;
    }

    private static bool StartsWithWord(string s, string word)
    {
        if (!s.StartsWith(word, StringComparison.Ordinal)) return false;
        if (s.Length == word.Length) return true;
        char c = s[word.Length];
        return !(char.IsLetterOrDigit(c) || c == '_');
    }
}
