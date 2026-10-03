// src/Wc3.MapDocument/JassGlobals.cs
using System.Globalization;
using System.Text.RegularExpressions;

namespace Wc3.Model;

/// <summary>
/// The declarations inside a script's <c>globals</c> block(s). Pure text scan, the companion of
/// <see cref="JassFunctionIndex"/>. Shared so the script porter and the script checker can never
/// disagree about what counts as a global (they used to hold separate copies of this logic).
/// </summary>
public static class JassGlobals
{
    /// <summary>Global name to its full declaration line, plus the declaration order.
    /// First declaration wins on a duplicate name (illegal in JASS anyway).</summary>
    public static (Dictionary<string, string> ByName, List<string> Order) Parse(string[] lines)
    {
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        var order = new List<string>();
        bool inBlock = false;
        foreach (var raw in lines)
        {
            var t = raw.Trim();
            if (!inBlock) { if (t == "globals") inBlock = true; continue; }
            if (t == "endglobals") { inBlock = false; continue; }
            if (t.Length == 0 || t.StartsWith("//", StringComparison.Ordinal)) continue;
            var name = NameOf(t);
            if (name is not null && !map.ContainsKey(name)) { map[name] = t; order.Add(name); }
        }
        return (map, order);
    }

    /// <summary>The declared name in "[constant] type [array] name[= init]". Also serves a local
    /// declaration once its leading "local " is removed, the tail shape is identical.</summary>
    public static string? NameOf(string decl)
    {
        var toks = decl.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        int i = 0;
        if (i < toks.Length && toks[i] == "constant") i++;
        i++; // type
        if (i < toks.Length && toks[i] == "array") i++;
        if (i >= toks.Length) return null;
        var name = toks[i].Split('=')[0].Trim();
        return name.Length == 0 ? null : name;
    }

    /// <summary>The declared type and array-ness of a "[constant] type [array] name[= init]"
    /// declaration, null when it cannot be parsed (same leniency as <see cref="NameOf"/>). Used by
    /// the porter's bootstrap-state feature to decide how (or whether) a carried-but-never-assigned
    /// global can be safely constructed, "timer" and "timer array" must not be treated alike, an
    /// array has no single instance to construct.</summary>
    public static (string Type, bool IsArray)? TypeOf(string decl)
    {
        var toks = decl.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        int i = 0;
        if (i < toks.Length && toks[i] == "constant") i++;
        if (i >= toks.Length) return null;
        string type = toks[i];
        i++;
        bool isArray = i < toks.Length && toks[i] == "array";
        return (type, isArray);
    }

    private static readonly Regex SetStatement = new(@"\bset\s+([A-Za-z_][A-Za-z0-9_]*)\b", RegexOptions.Compiled);

    /// <summary>Every name in <paramref name="declared"/> that <paramref name="scopeText"/> assigns,
    /// either through a "set NAME=" statement anywhere in it, or through a non-default value on the
    /// declaration itself (see <see cref="HasRealInitializer"/>, a handle constructed right there,
    /// "hashtable udg_X=InitHashtable()", is just as real an initialization as a later "set"). The
    /// general "has this global's value ever been set" test, shared so the runtime readiness check
    /// and the porter's bootstrap-state feature can never disagree about what counts as an
    /// assignment.
    ///
    /// Comments are stripped from <paramref name="scopeText"/> first, so a "set" the porter itself
    /// commented out (its own trim marker prefixes one, "//[wc3ctl trimmed] set X=...", because X
    /// turned out to call a function this port could not carry) does not read as a real assignment.
    /// Missing this once let a "code" global (a timer callback trimmed the same way GearTimer03's
    /// was) go unreported, the commented-out "set" was still visible to a plain regex, so the
    /// global looked assigned when the actual assignment never runs.</summary>
    public static HashSet<string> Assigned(IReadOnlyDictionary<string, string> declared, string scopeText)
    {
        string stripped = string.Join('\n', JassComments.Strip(JassLines.Split(scopeText)));
        var assigned = new HashSet<string>(StringComparer.Ordinal);
        foreach (Match m in SetStatement.Matches(stripped))
            if (declared.ContainsKey(m.Groups[1].Value)) assigned.Add(m.Groups[1].Value);
        foreach (var (name, decl) in declared)
            if (HasRealInitializer(decl)) assigned.Add(name);
        return assigned;
    }

    /// <summary>True when a "[constant] type [array] name = rhs" declaration's rhs is something
    /// other than the type's own zero value. A bare array declaration or one with no "=" at all has
    /// no rhs and returns false, exactly like a trivial default, since nothing here was actually
    /// initialized either way.</summary>
    public static bool HasRealInitializer(string declLine)
    {
        int eq = declLine.IndexOf('=');
        if (eq < 0) return false;
        string rhs = declLine[(eq + 1)..].Trim();
        if (rhs is "false" or "null" or "") return false;
        return !double.TryParse(rhs.TrimEnd('.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var d)
            || d != 0;
    }
}
