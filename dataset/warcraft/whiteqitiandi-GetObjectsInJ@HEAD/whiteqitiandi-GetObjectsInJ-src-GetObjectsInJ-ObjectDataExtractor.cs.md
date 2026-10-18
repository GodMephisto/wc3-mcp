---
type: vendor-doc
source_repo: "https://github.com/whiteqitiandi/GetObjectsInJ"
source_path: "src/GetObjectsInJ/ObjectDataExtractor.cs"
ref: HEAD
---

using System.Text;
using System.Text.RegularExpressions;

namespace GetObjectsInJ;

public enum ObjectKind
{
    Ability,
    Buff,
    Item,
    Unit
}

public sealed record ObjectSource(ObjectKind Kind, string IniPath);
public sealed record ObjectRow(string Type, string Id, string Name);

public sealed record ExtractionResult(
    IReadOnlyList<ObjectRow> MatchedRows,
    IReadOnlyList<string> UnmatchedIds,
    IReadOnlyDictionary<ObjectKind, int> MatchedCounts);

public static partial class ObjectDataExtractor
{
    static ObjectDataExtractor() => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

    public static ExtractionResult Extract(
        string jPath,
        IEnumerable<ObjectSource> sources,
        ISet<ObjectKind> selectedKinds)
    {
        var ids = FindReferencedIds(ReadTextWithEncodingDetection(jPath));
        var nameMaps = sources.ToDictionary(
            source => source.Kind,
            source => ReadIniNames(ReadTextWithEncodingDetection(source.IniPath), source.Kind));

        var rows = new List<ObjectRow>();
        var counts = Enum.GetValues<ObjectKind>().ToDictionary(kind => kind, _ => 0);
        var unmatched = new List<string>();

        foreach (var id in ids)
        {
            var foundAnywhere = false;
            foreach (var kind in Enum.GetValues<ObjectKind>())
            {
                if (!nameMaps.TryGetValue(kind, out var names) || !names.TryGetValue(id, out var name)) continue;
                foundAnywhere = true;
                counts[kind]++;
                if (selectedKinds.Contains(kind))
                    rows.Add(new ObjectRow(GetKindName(kind), id, name));
            }

            if (!foundAnywhere) unmatched.Add(id);
        }

        return new ExtractionResult(rows, unmatched, counts);
    }

    public static IReadOnlyList<string> FindReferencedIds(string jass)
    {
        var result = new List<string>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        for (var i = 0; i < jass.Length;)
        {
            if (jass[i] == '/' && i + 1 < jass.Length && jass[i + 1] == '/')
            {
                i += 2;
                while (i < jass.Length && jass[i] is not ('\r' or '\n')) i++;
                continue;
            }

            if (jass[i] == '/' && i + 1 < jass.Length && jass[i + 1] == '*')
            {
                i += 2;
                while (i + 1 < jass.Length && !(jass[i] == '*' && jass[i + 1] == '/')) i++;
                i = Math.Min(i + 2, jass.Length);
                continue;
            }

            if (jass[i] == '"')
            {
                i = SkipQuotedString(jass, i, '"');
                continue;
            }

            if (jass[i] == '\'' && i + 5 < jass.Length && jass[i + 5] == '\'' &&
                IsRawCodeCharacter(jass[i + 1]) && IsRawCodeCharacter(jass[i + 2]) &&
                IsRawCodeCharacter(jass[i + 3]) && IsRawCodeCharacter(jass[i + 4]))
            {
                var id = jass.Substring(i + 1, 4);
                if (seen.Add(id)) result.Add(id);
                i += 6;
                continue;
            }

            if (jass[i] == '\'')
            {
                i = SkipQuotedString(jass, i, '\'');
                continue;
            }

            i++;
        }

        return result;
    }

    public static Dictionary<string, string> ReadIniNames(string ini, ObjectKind kind = ObjectKind.Unit)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        var selectedPriorities = new Dictionary<string, int>(StringComparer.Ordinal);
        var lines = ini.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        string? section = null;

        for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            var trimmed = lines[lineIndex].Trim();
            if (trimmed.Length == 0 || trimmed.StartsWith(';') || trimmed.StartsWith('#')) continue;

            var sectionMatch = SectionRegex().Match(trimmed);
            if (sectionMatch.Success)
            {
                section = sectionMatch.Groups[1].Value;
                // ID 是否存在與是否有顯示名稱是兩件事。尤其 buff.ini 內部分
                // section 只有 EditorName，甚至沒有任何非空的名稱欄位。
                result.TryAdd(section, string.Empty);
                selectedPriorities.TryAdd(section, 0);
                continue;
            }

            if (section is null) continue;
            var equals = trimmed.IndexOf('=');
            if (equals < 0) continue;
            var key = trimmed[..equals].Trim();
            var priority = GetDisplayNamePriority(kind, key);
            if (priority == 0) continue;

            var rawValue = trimmed[(equals + 1)..].TrimStart();
            string value;
            var longOpen = LuaLongStringOpenRegex().Match(rawValue);
            if (longOpen.Success)
            {
                var closing = "]" + longOpen.Groups[1].Value + "]";
                var content = new StringBuilder();
                var remainder = rawValue[longOpen.Length..];
                var closeAt = remainder.IndexOf(closing, StringComparison.Ordinal);

                if (closeAt >= 0)
                {
                    content.Append(remainder[..closeAt]);
                }
                else
                {
                    if (remainder.Length > 0) content.Append(remainder);
                    var closed = false;
                    while (++lineIndex < lines.Length)
                    {
                        var next = lines[lineIndex];
                        closeAt = next.IndexOf(closing, StringComparison.Ordinal);
                        if (content.Length > 0) content.Append('\n');
                        if (closeAt >= 0)
                        {
                            content.Append(next[..closeAt]);
                            closed = true;
                            break;
                        }
                        content.Append(next);
                    }

                    if (!closed)
                        throw new FormatException($"INI 區段 [{section}] 的 Name 含有未結束的 Lua 長字串。");
                }
                value = content.ToString();
            }
            else
            {
                value = StripIniComment(rawValue).Trim();
                if (value.Length >= 2 && value[0] == '"' && value[^1] == '"')
                    value = UnescapeQuotedValue(value[1..^1]);
                else if (value.Length >= 2 && value[0] == '\'' && value[^1] == '\'')
                    value = value[1..^1];
            }

            var currentPriority = selectedPriorities[section];
            var currentValue = result[section];
            if ((priority > currentPriority && (value.Length > 0 || currentValue.Length == 0)) ||
                (currentValue.Length == 0 && value.Length > 0))
            {
                result[section] = value;
                selectedPriorities[section] = priority;
            }
        }

        return result;
    }

    public static void WriteCsv(string outputPath, IEnumerable<ObjectRow> rows, IEnumerable<string>? unmatchedIds = null)
    {
        using var writer = new StreamWriter(outputPath, false, new UTF8Encoding(true));
        foreach (var row in rows)
            writer.WriteLine($"{CsvEscape(row.Id)},{CsvEscape(row.Name)},{CsvEscape(row.Type)}");
        if (unmatchedIds is not null)
            foreach (var id in unmatchedIds)
                writer.WriteLine($"{CsvEscape(id)},,未匹配");
    }

    public static string GetKindName(ObjectKind kind) => kind switch
    {
        ObjectKind.Ability => "技能",
        ObjectKind.Buff => "增幅",
        ObjectKind.Item => "物品",
        ObjectKind.Unit => "部隊",
        _ => throw new ArgumentOutOfRangeException(nameof(kind))
    };

    private static int GetDisplayNamePriority(ObjectKind kind, string key)
    {
        if (kind == ObjectKind.Buff)
        {
            if (key.Equals("Bufftip", StringComparison.OrdinalIgnoreCase)) return 30;
            if (key.Equals("EditorName", StringComparison.OrdinalIgnoreCase)) return 20;
            if (key.Equals("Name", StringComparison.OrdinalIgnoreCase)) return 10;
            return 0;
        }

        if (key.Equals("Name", StringComparison.OrdinalIgnoreCase)) return 30;
        if (key.Equals("EditorName", StringComparison.OrdinalIgnoreCase)) return 20;
        if (key.Equals("Tip", StringComparison.OrdinalIgnoreCase)) return 10;
        return 0;
    }

    private static string ReadTextWithEncodingDetection(string path)
    {
        var bytes = File.ReadAllBytes(path);
        if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            return Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
        if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
            return Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2);
        if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
            return Encoding.BigEndianUnicode.GetString(bytes, 2, bytes.Length - 2);

        try { return new UTF8Encoding(false, true).GetString(bytes); }
        catch (DecoderFallbackException) { return Encoding.GetEncoding(950).GetString(bytes); }
    }

    private static int SkipQuotedString(string source, int start, char quote)
    {
        for (var i = start + 1; i < source.Length; i++)
        {
            if (source[i] == '\\') i++;
            else if (source[i] == quote) return i + 1;
        }
        return source.Length;
    }

    private static string StripIniComment(string value)
    {
        var quote = '\0';
        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if (quote != '\0')
            {
                if (c == '\\') i++;
                else if (c == quote) quote = '\0';
                continue;
            }
            if (c is '"' or '\'') quote = c;
            else if (c is ';' or '#') return value[..i].TrimEnd();
        }
        return value;
    }

    private static string UnescapeQuotedValue(string value) => value
        .Replace("\\\"", "\"")
        .Replace("\\n", "\n")
        .Replace("\\r", "\r")
        .Replace("\\\\", "\\");

    private static string CsvEscape(string value) =>
        value.IndexOfAny([',', '"', '\r', '\n']) >= 0 ? $"\"{value.Replace("\"", "\"\"")}\"" : value;

    private static bool IsRawCodeCharacter(char c) => c is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9';

    [GeneratedRegex(@"^\[([A-Za-z0-9]{4})\](?:\s*[;#].*)?$")]
    private static partial Regex SectionRegex();

    [GeneratedRegex(@"^\[(=*)\[")]
    private static partial Regex LuaLongStringOpenRegex();
}
