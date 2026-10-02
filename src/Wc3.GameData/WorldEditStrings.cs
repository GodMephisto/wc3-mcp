namespace Wc3.GameData;

/// <summary>
/// Localized World Editor UI strings (ui\worldeditstrings.txt): KEY=VALUE lines under a
/// [WorldEditStrings] header. Values may be quoted (quotes stripped) and may contain
/// formatting codes like |n and |cAARRGGBB..|r (kept as-is).
/// </summary>
public sealed class WorldEditStrings
{
    private const char Bom = (char)0xFEFF;

    private readonly Dictionary<string, string> _strings;
    private WorldEditStrings(Dictionary<string, string> strings) => _strings = strings;

    public int Count => _strings.Count;

    public static WorldEditStrings FromBytes(byte[] bytes) => Parse(DecodeUtf8(bytes));

    /// <summary>
    /// Parses and merges several editor-string files into one table (for example
    /// worldeditstrings.txt for UI labels plus worldeditgamestrings.txt for the
    /// WESTRING_DEST_* / WESTRING_DOODAD_* object names). Null or missing files are
    /// skipped; a key in a later file overrides the same key in an earlier one.
    /// </summary>
    public static WorldEditStrings FromByteSources(params byte[]?[] files)
    {
        var sb = new System.Text.StringBuilder();
        foreach (var f in files)
            if (f is not null) sb.Append(DecodeUtf8(f)).Append('\n');
        return Parse(sb.ToString());
    }

    public static WorldEditStrings Parse(string text)
    {
        var strings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var raw in text.Split('\n'))
        {
            var line = raw.TrimEnd('\r');
            if (line.Length == 0 || line.StartsWith('[')) continue;
            var eq = line.IndexOf('=');
            if (eq < 0) continue;
            var key = line[..eq].Trim();
            if (key.Length == 0) continue;
            strings[key] = StripQuotes(line[(eq + 1)..].Trim());
        }
        return new WorldEditStrings(strings);
    }

    public bool TryGet(string key, out string value) => _strings.TryGetValue(key, out value!);

    /// <summary>UTF-8 decode, dropping a leading BOM (U+FEFF) if present.</summary>
    internal static string DecodeUtf8(byte[] bytes) =>
        System.Text.Encoding.UTF8.GetString(bytes).TrimStart(Bom);

    internal static string StripQuotes(string value) =>
        value.Length >= 2 && value.StartsWith('"') && value.EndsWith('"') ? value[1..^1] : value;
}
