namespace Wc3.GameData;

/// <summary>
/// Object Editor "Profile" data: INI-like TXT files ([rawcode] sections with Key=Value
/// lines) holding the fields whose metadata "slk" column says "Profile". Names,
/// tooltips and art references live here rather than in the SLKs. Non-localized values
/// sit under units\*func.txt and localized text in the enus locale's units\*strings.txt
/// twins. Missing files contribute nothing. Later files overlay earlier ones per key.
/// A value that is exactly a WESTRING_* key resolves against the editor strings at
/// parse time.
/// </summary>
public sealed class ProfileTxtStore
{
    private const string UnitsDir = @"war3.w3mod:units\";
    private const string LocaleUnitsDir = @"war3.w3mod:_locales\enus.w3mod:units\";

    // File groups confirmed against the live install's CASC listfile. The ability
    // profile files also carry the buff sections (e.g. BSTN lives in
    // commonabilitystrings.txt), so the buff store reuses the ability set.
    private static readonly string[] AbilityGroups =
        { "campaign", "common", "human", "item", "neutral", "nightelf", "orc", "undead" };
    private static readonly string[] UpgradeGroups =
        { "campaign", "human", "neutral", "nightelf", "orc", "undead" };

    private readonly Dictionary<string, Dictionary<string, string>> _sections; // rawcode -> key -> value
    private ProfileTxtStore(Dictionary<string, Dictionary<string, string>> sections) => _sections = sections;

    /// <summary>A store that resolves nothing, used when no profile files were loaded.</summary>
    public static ProfileTxtStore Empty { get; } = new(new(StringComparer.OrdinalIgnoreCase));

    public int Count => _sections.Count;

    public bool IsEmpty => _sections.Count == 0;

    public static ProfileTxtStore BuildItems(IGameDataSource src, WorldEditStrings? strings = null) =>
        FromSources(src, new[] { UnitsDir + "itemfunc.txt", LocaleUnitsDir + "itemstrings.txt" }, strings);

    /// <summary>Ability profile files. Shared with buffs, whose sections live in the same files.</summary>
    public static ProfileTxtStore BuildAbilities(IGameDataSource src, WorldEditStrings? strings = null) =>
        FromSources(src,
            AbilityGroups.Select(g => UnitsDir + g + "abilityfunc.txt")
                .Concat(AbilityGroups.Select(g => LocaleUnitsDir + g + "abilitystrings.txt")),
            strings);

    public static ProfileTxtStore BuildUpgrades(IGameDataSource src, WorldEditStrings? strings = null) =>
        FromSources(src,
            UpgradeGroups.Select(g => UnitsDir + g + "upgradefunc.txt")
                .Concat(UpgradeGroups.Select(g => LocaleUnitsDir + g + "upgradestrings.txt")),
            strings);

    /// <summary>Parses every readable path into one rawcode -> key -> value map. A key
    /// present in several files keeps the last file's value, so localized strings files
    /// should come after their func twins. Missing files are skipped.</summary>
    public static ProfileTxtStore FromSources(
        IGameDataSource src, IEnumerable<string> paths, WorldEditStrings? strings = null)
    {
        var sections = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var path in paths)
        {
            var bytes = src.ReadFile(path);
            if (bytes != null) ParseInto(sections, WorldEditStrings.DecodeUtf8(bytes), strings);
        }
        return new ProfileTxtStore(sections);
    }

    public bool TryGetValue(string rawcode, string key, out string value)
    {
        value = "";
        return _sections.TryGetValue(rawcode, out var section) && section.TryGetValue(key, out value!);
    }

    private static void ParseInto(
        Dictionary<string, Dictionary<string, string>> sections, string text, WorldEditStrings? strings)
    {
        Dictionary<string, string>? section = null;
        foreach (var raw in text.Split('\n'))
        {
            var line = raw.TrimEnd('\r').Trim();
            if (line.Length == 0 || line.StartsWith("//", StringComparison.Ordinal)) continue;
            if (line.StartsWith('[') && line.EndsWith(']'))
            {
                var rawcode = line[1..^1].Trim();
                if (rawcode.Length == 0) { section = null; continue; }
                if (!sections.TryGetValue(rawcode, out section))
                    sections[rawcode] = section = new(StringComparer.OrdinalIgnoreCase);
                continue;
            }
            if (section is null) continue;
            var eq = line.IndexOf('=');
            if (eq <= 0) continue;
            var key = line[..eq].Trim();
            // Reforged balance-variant overrides ("Ubertip:melee,V0=...") do not apply
            // to map editing, so only the plain key is kept.
            if (key.Length == 0 || key.Contains(':')) continue;
            var value = line[(eq + 1)..].Trim();
            if (strings != null && value.StartsWith("WESTRING", StringComparison.OrdinalIgnoreCase)
                && strings.TryGet(value, out var resolved))
                value = resolved;
            section[key] = value;
        }
    }

    /// <summary>
    /// Splits a profile value into its comma-separated elements. The format expresses
    /// per-level lists, x/y pairs and the like this way. A quoted element keeps its
    /// embedded commas and sheds the quotes.
    /// </summary>
    public static IReadOnlyList<string> SplitElements(string value)
    {
        var parts = new List<string>();
        var sb = new System.Text.StringBuilder();
        bool inQuotes = false;
        foreach (var ch in value)
        {
            if (ch == '"') { inQuotes = !inQuotes; continue; }
            if (ch == ',' && !inQuotes) { parts.Add(sb.ToString().Trim()); sb.Clear(); continue; }
            sb.Append(ch);
        }
        parts.Add(sb.ToString().Trim());
        return parts;
    }
}
