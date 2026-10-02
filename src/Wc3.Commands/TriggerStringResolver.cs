using System.Text.RegularExpressions;

namespace Wc3.Commands;

/// <summary>
/// Resolves TRIGSTR_ references (e.g. "TRIGSTR_003") against the war3map.wts
/// string table. Pure over a plain dictionary so it's testable without War3Net.
/// </summary>
public static partial class TriggerStringResolver
{
    [GeneratedRegex(@"^TRIGSTR_(\d+)$")]
    private static partial Regex TrigStrPattern();

    /// <summary>
    /// If <paramref name="raw"/> is a TRIGSTR_ reference with a known id,
    /// returns the referenced string (trailing whitespace/newlines trimmed);
    /// otherwise returns <paramref name="raw"/> unchanged.
    /// </summary>
    public static string Resolve(string raw, IReadOnlyDictionary<int, string>? strings)
    {
        if (raw is null || strings is null || strings.Count == 0) return raw!;
        var match = TrigStrPattern().Match(raw);
        if (!match.Success || !int.TryParse(match.Groups[1].Value, out var id)) return raw;
        return strings.TryGetValue(id, out var value) ? value.TrimEnd('\r', '\n', ' ', '\t') : raw;
    }
}
