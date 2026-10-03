using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Finds a string anywhere a map keeps text a person or an agent would look for, which is the
/// archive's file names, the script's lines, the string table, and object data (rawcodes and
/// field values).
/// </summary>
/// <remarks>
/// It used to compare file names only, while the MCP tool described a search of script text,
/// object data and strings. So an agent asking where an ability id or a hero name is used got
/// "no hits" for something the map plainly contained, which reads exactly like absence.
/// Matching ignores case. Output is capped at <see cref="MaxHits"/> so a common word cannot
/// flood the caller, and <see cref="SearchResult.Truncated"/> says when that happened.
/// </remarks>
public static class SearchCommand
{
    public const int MaxHits = 500;
    private const int SnippetLength = 160;

    public static SearchResult Execute(MapDocument doc, string query)
    {
        var hits = new List<SearchHit>();
        bool Add(string file, string context)
        {
            if (hits.Count >= MaxHits) return false;
            hits.Add(new SearchHit(file, context));
            return true;
        }
        if (string.IsNullOrEmpty(query)) return new SearchResult(hits);

        foreach (var f in doc.Files)
        {
            string? name = f.FileName ?? f.RecoveredFileName;
            if (name is not null && name.Contains(query, StringComparison.OrdinalIgnoreCase)
                && !Add(name, "filename"))
                return new SearchResult(hits, Truncated: true);
        }

        foreach (var scriptName in new[] { "war3map.j", "war3map.lua" })
        {
            if (doc.GetFile(scriptName) is not { } script) continue;
            var lines = ScriptText.GetString(script.CurrentBytes).Split('\n');
            for (int i = 0; i < lines.Length; i++)
                if (lines[i].Contains(query, StringComparison.OrdinalIgnoreCase)
                    && !Add(script.FileName ?? scriptName, $"line {i + 1}: {Snippet(lines[i])}"))
                    return new SearchResult(hits, Truncated: true);
        }

        var strings = StringsCommand.List(doc);
        foreach (var s in strings.Entries)
            if (s.Text.Contains(query, StringComparison.OrdinalIgnoreCase)
                && !Add(strings.FileName ?? "war3map.wts", $"TRIGSTR_{s.Id:000}: {Snippet(s.Text)}"))
                return new SearchResult(hits, Truncated: true);

        foreach (var kind in ObjectKinds.All)
        {
            var info = ObjectKinds.Info(kind);
            foreach (var entry in ObjectKinds.MergedEntries(doc, info))
            {
                string rawcode = entry.Id.ToRawcode();
                string baseRawcode = entry.OldId != 0 ? entry.OldId.ToRawcode() : "";
                if ((rawcode.Contains(query, StringComparison.OrdinalIgnoreCase)
                     || baseRawcode.Contains(query, StringComparison.OrdinalIgnoreCase))
                    && !Add(info.MapFile, $"{kind} {rawcode}" + (baseRawcode.Length > 0 ? $" (based on {baseRawcode})" : "")))
                    return new SearchResult(hits, Truncated: true);
                foreach (var (field, value) in entry.Mods)
                    if (value.Contains(query, StringComparison.OrdinalIgnoreCase)
                        && !Add(info.MapFile, $"{kind} {rawcode} {field}: {Snippet(value)}"))
                        return new SearchResult(hits, Truncated: true);
            }
        }
        return new SearchResult(hits);
    }

    private static string Snippet(string text)
    {
        var t = text.Trim();
        return t.Length <= SnippetLength ? t : t[..SnippetLength] + "...";
    }
}
