// src/Wc3.GameData/EditorEnumData.cs
namespace Wc3.GameData;

/// <summary>One legal value of an enumerated object-data field, and what it is called.</summary>
public sealed record EnumOption(string Value, string DisplayName)
{
    /// <summary>What a picker should show. Falls back to the raw value when the display name
    /// could not be resolved, since a token is more use than an empty row.</summary>
    public string Label => DisplayName.Length > 0 ? DisplayName : Value;

    public override string ToString() =>
        DisplayName.Length > 0 && DisplayName != Value ? $"{DisplayName} ({Value})" : Value;
}

/// <summary>
/// The enumerated field types the World Editor offers as dropdowns, read from the game's own
/// <c>UI\UnitEditorData.txt</c> and <c>UI\WorldEditData.txt</c>.
///
/// Each section is one field TYPE, as named by the <c>type</c> column of a metadata SLK, and holds
/// an ordered list of <c>NN=storedValue,WESTRING_KEY</c>. So this is simultaneously the authority
/// for which values a field may legally take and the only place the human-readable names live.
///
/// <code>
/// [attackType]
/// 00=unknown,WESTRING_NONE
/// 01=normal,WESTRING_UE_ATTACKTYPE_NORMAL
/// NumValues=8
/// </code>
///
/// WHY this exists. Options were previously derived by collecting the distinct values the base
/// game data happened to use for a field. That is wrong in two directions at once. It misses legal
/// values no stock object uses, and it produces raw tokens, so a team colour field offered
/// "-1, 0, 1, 2" where the editor offers "None, Red, Blue, Teal". Deriving a closed set by
/// observation, when the closed set is shipped in a file, is guesswork dressed as measurement.
/// </summary>
public sealed class EditorEnumData
{
    private readonly Dictionary<string, List<EnumOption>> _byType =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Nothing loaded. Every lookup misses, so callers fall back cleanly.</summary>
    public static EditorEnumData Empty { get; } = new();

    /// <summary>Field type names that resolve to a closed set of values.</summary>
    public IReadOnlyCollection<string> Types => _byType.Keys;

    /// <summary>
    /// The ordered legal values for a field type, or false when the type is not enumerated.
    /// </summary>
    public bool TryGet(string type, out IReadOnlyList<EnumOption> options)
    {
        if (type.Length > 0 && _byType.TryGetValue(type, out var list))
        {
            options = list;
            return true;
        }
        options = Array.Empty<EnumOption>();
        return false;
    }

    /// <summary>
    /// Parses one or more editor data files. <paramref name="strings"/> resolves the WESTRING
    /// display keys; pass the same table the rest of the game data uses so a name never differs
    /// between two parts of the UI.
    /// </summary>
    public static EditorEnumData FromByteSources(WorldEditStrings strings, params byte[]?[] files)
    {
        var data = new EditorEnumData();
        foreach (var bytes in files)
        {
            if (bytes is null || bytes.Length == 0) continue;
            data.Ingest(System.Text.Encoding.UTF8.GetString(bytes), strings);
        }
        return data;
    }

    private void Ingest(string text, WorldEditStrings strings)
    {
        string? section = null;
        List<EnumOption>? current = null;

        foreach (var raw in text.Replace("\r", "").Split('\n'))
        {
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith("//", StringComparison.Ordinal)) continue;

            if (line.StartsWith('[') && line.EndsWith(']'))
            {
                section = line[1..^1].Trim();
                // A later file may extend a section a earlier one opened, so reuse the list.
                if (!_byType.TryGetValue(section, out current))
                    _byType[section] = current = new List<EnumOption>();
                continue;
            }
            if (current is null) continue;

            int eq = line.IndexOf('=');
            if (eq <= 0) continue;
            var key = line[..eq].Trim();

            // Sort and NumValues are metadata about the section, not values in it.
            if (key.Equals("NumValues", StringComparison.OrdinalIgnoreCase)
                || key.Equals("Sort", StringComparison.OrdinalIgnoreCase)) continue;
            // The World Editor skips the "_Alt" duplicates, which are alternate labels.
            if (key.EndsWith("_Alt", StringComparison.OrdinalIgnoreCase)) continue;

            var rest = line[(eq + 1)..].Trim();
            if (rest.Length == 0) continue;

            // "storedValue,WESTRING_KEY". A value containing no comma is its own label.
            int comma = rest.IndexOf(',');
            var value = (comma < 0 ? rest : rest[..comma]).Trim();
            var nameKey = comma < 0 ? "" : rest[(comma + 1)..].Trim();

            var display = nameKey.Length > 0 && strings.TryGet(nameKey, out var resolved)
                ? resolved
                : nameKey;
            // An unresolved WESTRING is worse than nothing as a label, so drop back to the value.
            if (display.StartsWith("WESTRING", StringComparison.OrdinalIgnoreCase)) display = "";

            if (value.Length > 0 && !current.Any(o =>
                    string.Equals(o.Value, value, StringComparison.OrdinalIgnoreCase)))
                current.Add(new EnumOption(value, display));
        }
    }
}

/// <summary>
/// One row of a World Editor catalog. <see cref="Key"/> is the stored token on the left of
/// the equals sign (a tileset letter, a channel number, a setting name), <see cref="Values"/>
/// is every comma-separated payload field in file order and verbatim (WESTRING keys
/// included), and <see cref="DisplayName"/> is the resolved human name when any payload
/// field is a WESTRING key the shipped strings know, empty otherwise.
/// </summary>
public sealed record EditorCatalogEntry(string Key, IReadOnlyList<string> Values, string DisplayName)
{
    /// <summary>What a picker should show. Falls back to the key when no display name
    /// resolved, because the key is the token a map actually stores and a WESTRING key
    /// would be worse than either.</summary>
    public string Label => DisplayName.Length > 0 ? DisplayName : Key;
}

/// <summary>
/// The World Editor's catalogs, read from the game's own <c>UI\WorldEditData.txt</c>. That
/// file holds 41 sections the editor drives its pickers and defaults from (TileSets,
/// SkyModels, LoadingScreens, SoundChannels, brush palettes, editor settings and more).
///
/// WHY this is a second reader of the same file rather than a use of
/// <see cref="EditorEnumData"/>. The enum reader models one shape,
/// <c>NN=storedValue,WESTRING_KEY</c>, where the left-hand key is bookkeeping and equal
/// values are duplicates. Measured against the retail install, only SkyModels fits that
/// shape. Every other section breaks it somewhere. TileSets puts the WESTRING first and the
/// payload second, so both columns come out swapped. TerrainLights keys 18 tilesets to 6
/// model paths, so value dedup silently drops 12 rows and the tileset letters, the actual
/// data, are gone. LoadingScreens carries four payload fields and a NumScreens count line,
/// which the enum reader turns into 3 rows, one of them the count. A catalog therefore
/// keeps the key, keeps every payload field, keeps duplicates and keeps file order.
/// </summary>
public sealed class EditorCatalogData
{
    private readonly Dictionary<string, List<EditorCatalogEntry>> _byName =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> _order = new();

    /// <summary>Nothing loaded. Every lookup misses, so callers fall back cleanly.</summary>
    public static EditorCatalogData Empty { get; } = new();

    /// <summary>Catalog names in file order.</summary>
    public IReadOnlyList<string> Names => _order;

    /// <summary>The ordered entries of one catalog, or false when the name is unknown.
    /// Name matching is case insensitive, the file's own casing is what
    /// <see cref="Names"/> reports.</summary>
    public bool TryGet(string name, out IReadOnlyList<EditorCatalogEntry> entries)
    {
        if (name.Length > 0 && _byName.TryGetValue(name, out var list))
        {
            entries = list;
            return true;
        }
        entries = Array.Empty<EditorCatalogEntry>();
        return false;
    }

    /// <summary>
    /// Parses one or more catalog files. <paramref name="strings"/> resolves the WESTRING
    /// display keys, pass the same table the rest of the game data uses so a name never
    /// differs between two parts of the UI.
    /// </summary>
    public static EditorCatalogData FromByteSources(WorldEditStrings strings, params byte[]?[] files)
    {
        var data = new EditorCatalogData();
        foreach (var bytes in files)
        {
            if (bytes is null || bytes.Length == 0) continue;
            data.Ingest(System.Text.Encoding.UTF8.GetString(bytes), strings);
        }
        return data;
    }

    private void Ingest(string text, WorldEditStrings strings)
    {
        List<EditorCatalogEntry>? current = null;

        foreach (var raw in text.Replace("\r", "").Split('\n'))
        {
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith("//", StringComparison.Ordinal)) continue;

            if (line.StartsWith('[') && line.EndsWith(']'))
            {
                var section = line[1..^1].Trim();
                if (!_byName.TryGetValue(section, out current))
                {
                    _byName[section] = current = new List<EditorCatalogEntry>();
                    _order.Add(section);
                }
                continue;
            }
            if (current is null) continue;

            int eq = line.IndexOf('=');
            if (eq <= 0) continue;
            var key = line[..eq].Trim();
            var rest = line[(eq + 1)..].Trim();
            if (rest.Length == 0) continue;

            // Count lines (NumScreens, NumSizes, NumImages, NumSounds, NumValues) and Sort
            // describe the section, they are not rows in it. The integer check keeps the
            // rule from ever eating a real entry whose key merely starts with Num.
            if (key.Equals("Sort", StringComparison.OrdinalIgnoreCase)) continue;
            if (key.StartsWith("Num", StringComparison.OrdinalIgnoreCase)
                && int.TryParse(rest, out _)) continue;

            var values = rest.Split(',', StringSplitOptions.TrimEntries);

            // The human name rides in whichever payload field is a WESTRING key. An
            // unresolved key yields no name at all, leaking "WESTRING_X" into a picker
            // is worse than falling back to the stored token.
            var display = "";
            foreach (var v in values)
            {
                if (!v.StartsWith("WESTRING", StringComparison.OrdinalIgnoreCase)) continue;
                if (strings.TryGet(v, out var resolved))
                {
                    display = StripAccelerator(resolved);
                    break;
                }
            }

            current.Add(new EditorCatalogEntry(key, values, display));
        }
    }

    /// <summary>Editor strings mark a menu accelerator with a single ampersand
    /// ("A&amp;bilities" shows as "Abilities" with b underlined). Strip the marker, keep
    /// a doubled ampersand as one literal.</summary>
    private static string StripAccelerator(string s)
    {
        if (!s.Contains('&')) return s;
        var sb = new System.Text.StringBuilder(s.Length);
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == '&')
            {
                if (i + 1 < s.Length && s[i + 1] == '&') { sb.Append('&'); i++; }
                continue;
            }
            sb.Append(s[i]);
        }
        return sb.ToString();
    }
}
