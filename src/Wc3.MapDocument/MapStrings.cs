// src/Wc3.MapDocument/MapStrings.cs
using System.Text.RegularExpressions;
using War3Net.Build.Script;

namespace Wc3.Model;

/// <summary>
/// The map's own trigger-string table (war3map.wts). Resolves TRIGSTR_
/// references (e.g. "TRIGSTR_003") to their text so object/map names can be
/// displayed as authored instead of as raw references.
/// </summary>
public sealed partial class MapStrings
{
    [GeneratedRegex(@"^TRIGSTR_(\d+)$")]
    private static partial Regex TrigStrPattern();

    private readonly IReadOnlyDictionary<int, string> _strings;

    public MapStrings(IReadOnlyDictionary<int, string> strings) => _strings = strings;

    /// <summary>Builds the table from the document's war3map.wts (empty if absent/unparsed).</summary>
    public static MapStrings From(MapDocument doc)
    {
        var table = new Dictionary<int, string>();
        if (doc.GetFile("war3map.wts")?.Model is TriggerStrings wts)
            foreach (var s in wts.Strings)
                // Real maps carry entries with a null value (empty STRING block);
                // store "" so Resolve never dereferences null.
                table[(int)s.Key] = s.Value ?? "";
        return new MapStrings(table);
    }

    /// <summary>
    /// If <paramref name="raw"/> is a TRIGSTR_ reference (leading zeros tolerated)
    /// with a known id, returns the referenced string (trailing whitespace/newlines
    /// trimmed); otherwise returns <paramref name="raw"/> unchanged. Null/empty → "".
    /// </summary>
    public string Resolve(string? raw)
    {
        if (string.IsNullOrEmpty(raw)) return "";
        var match = TrigStrPattern().Match(raw);
        if (!match.Success || !int.TryParse(match.Groups[1].Value, out var id)) return raw;
        return _strings.TryGetValue(id, out var value) ? value.TrimEnd('\r', '\n', ' ', '\t') : raw;
    }
}
