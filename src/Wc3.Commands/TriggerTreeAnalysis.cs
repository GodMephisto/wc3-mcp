// src/Wc3.Commands/TriggerTreeAnalysis.cs
using System.Text;
using System.Text.RegularExpressions;

namespace Wc3.Commands;

/// <summary>
/// Small shared helpers for the read-only trigger analysis commands (to-script, diff, lint,
/// export, dead). Everything here works on the already-projected
/// <see cref="TriggerFunctionInfo"/> model from <see cref="TriggerReadCommand"/>, so a single
/// parameter rendering is the one the reader already produces rather than a second one that
/// could drift from it.
/// </summary>
internal static class TriggerTreeAnalysis
{
    /// <summary>Matches a global-variable reference (a World-Editor variable is named
    /// <c>udg_&lt;name&gt;</c> wherever it appears, including inside a nested call's argument
    /// list).</summary>
    private static readonly Regex VariableRef =
        new(@"\budg_[A-Za-z0-9_]+", RegexOptions.Compiled);

    /// <summary>The variable names referenced anywhere among these rendered parameter strings,
    /// de-duplicated and with the leading <c>udg_</c> kept so callers can match them against the
    /// wtg variable list either way.</summary>
    public static IReadOnlyList<string> VariableRefs(IEnumerable<string> parameters) =>
        parameters
            .SelectMany(p => VariableRef.Matches(p).Select(m => m.Value))
            .Distinct(StringComparer.Ordinal)
            .ToList();

    /// <summary>Drops a leading <c>udg_</c> so a parameter reference and a wtg variable
    /// definition, which may or may not carry the prefix, compare equal.</summary>
    public static string StripUdg(string name) =>
        name.StartsWith("udg_", StringComparison.Ordinal) ? name[4..] : name;

    /// <summary>A canonical, deterministic description of one ECA node and its nested blocks,
    /// used as the unit of comparison when two trigger trees are diffed.</summary>
    public static string Describe(TriggerFunctionInfo function)
    {
        var sb = new StringBuilder();
        DescribeInto(sb, function, includeKind: false);
        return sb.ToString();
    }

    private static void DescribeInto(StringBuilder sb, TriggerFunctionInfo f, bool includeKind)
    {
        if (includeKind) sb.Append(f.Kind).Append(' ');
        sb.Append(f.Name).Append('(').Append(string.Join(", ", f.Parameters)).Append(')');
        if (f.Children.Count == 0) return;
        sb.Append(" {");
        for (int i = 0; i < f.Children.Count; i++)
        {
            if (i > 0) sb.Append("; ");
            DescribeInto(sb, f.Children[i], includeKind: true);
        }
        sb.Append('}');
    }
}
