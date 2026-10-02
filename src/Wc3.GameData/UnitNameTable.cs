namespace Wc3.GameData;

/// <summary>
/// Maps unit rawcodes (e.g. "hfoo") to localized display names by reading the per-race
/// unitstrings.txt files (INI sections [rawcode] with a Name= key). Missing files are skipped.
/// </summary>
public sealed class UnitNameTable
{
    private static readonly string[] Races = { "human", "orc", "undead", "nightelf", "neutral", "campaign" };

    private readonly Dictionary<string, string> _names;
    private UnitNameTable(Dictionary<string, string> names) => _names = names;

    public int Count => _names.Count;

    public static UnitNameTable FromSources(IGameDataSource src, string locale = "enus")
    {
        var names = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var race in Races)
        {
            var bytes = src.ReadFile($@"war3.w3mod:_locales\{locale}.w3mod:units\{race}unitstrings.txt");
            if (bytes != null) ParseInto(names, WorldEditStrings.DecodeUtf8(bytes));
        }
        return new UnitNameTable(names);
    }

    public bool TryGetName(string rawcode, out string name) => _names.TryGetValue(rawcode, out name!);

    private static void ParseInto(Dictionary<string, string> names, string text)
    {
        string? section = null;
        foreach (var raw in text.Split('\n'))
        {
            var line = raw.TrimEnd('\r').Trim();
            if (line.Length == 0) continue;
            if (line.StartsWith('[') && line.EndsWith(']'))
            {
                section = line[1..^1];
                continue;
            }
            if (section is null) continue;
            var eq = line.IndexOf('=');
            if (eq < 0 || !line[..eq].Trim().Equals("Name", StringComparison.OrdinalIgnoreCase)) continue;
            names[section] = WorldEditStrings.StripQuotes(line[(eq + 1)..].Trim());
        }
    }
}
