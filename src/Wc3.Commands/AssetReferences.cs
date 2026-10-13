// src/Wc3.Commands/AssetReferences.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// The two questions every asset command shares, answered once here rather than five times.
///
/// First, which archive entries are the map's OWN imported files rather than its structural
/// ones. Second, which asset path spellings the map's object data and its script string
/// literals name, expanded to the variants the engine actually loads. Both come from the same
/// primitives the name harvester (<see cref="MapDocument.HarvestAssetNames"/>) and the porter
/// (<see cref="BundleCommand"/>) use, so a path one recognises is recognised by all, and none
/// of the five commands can drift from the others about what "an import" or "a reference" is.
/// </summary>
internal static class AssetReferences
{
    /// <summary>
    /// True when an archive entry is a user import. Everything the map format registry knows,
    /// the importer's engine-file list, and MPQ bookkeeping is structural and excluded, so the
    /// minimap, locale tables and the map's own war3map.* files never read as user assets.
    /// </summary>
    public static bool IsImportedEntry(string fileName) =>
        !fileName.StartsWith('(') && !ImportsCommand.IsEngineFile(fileName);

    /// <summary>The archive's imported (non-structural) entries, in archive order.</summary>
    public static IReadOnlyList<MapFileEntry> ImportedEntries(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        return doc.Files
            .Where(f => f.FileName is not null && IsImportedEntry(f.FileName!))
            .ToList();
    }

    /// <summary>A path in the one form comparisons use, backslashes and case folded away, the
    /// way the engine resolves a name.</summary>
    public static string Normalize(string path) => path.Replace('/', '\\').ToLowerInvariant();

    /// <summary>
    /// Every spelling the map's object data and script string literals name, expanded to the
    /// load-time variants (extension siblings, icon prefixes, either slash) and normalised. A
    /// path named in any accepted form, "war3mapImported\Foo" for a stored "war3mapImported\Foo.blp"
    /// included, is in this set, so membership answers "is this archive entry referenced".
    /// </summary>
    public static HashSet<string> ReferencedSpellings(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var set = new HashSet<string>(StringComparer.Ordinal);
        foreach (var value in ObjectFieldValues(doc)) Add(value);
        foreach (var value in OverrideFileValues(doc)) Add(value);
        if (ScriptEntry(doc) is { } script)
            foreach (var literal in AssetPathCandidates.NamedInScript(ScriptText.GetString(script.CurrentBytes)))
                Add(literal);
        return set;

        void Add(string? raw)
        {
            if (string.IsNullOrEmpty(raw) || !AssetPathCandidates.LooksLikeAssetPath(raw)) return;
            foreach (var candidate in AssetPathCandidates.Expand(raw))
                set.Add(Normalize(candidate));
        }
    }

    /// <summary>Every field value any object-data kind carries, both Reforged layers merged.</summary>
    public static IEnumerable<string> ObjectFieldValues(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        foreach (var kind in ObjectKinds.All)
            foreach (var entry in ObjectKinds.MergedEntries(doc, ObjectKinds.Info(kind)))
                foreach (var mod in entry.Mods)
                    yield return mod.Value;
    }

    /// <summary>The map-local Reforged override files, whose values are asset paths.</summary>
    private static readonly string[] OverrideFiles = { "war3mapSkin.txt", "war3mapMisc.txt" };

    /// <summary>
    /// Every value in the map-local override files, the right-hand side of each key=value, split
    /// on commas. war3mapSkin.txt alone names icons, models and sounds the object data never
    /// repeats (InfoPanelIconArmorSmall=Gem\Armor\Red.blp), so a file it names is used even when no
    /// object field and no script literal names it. Harvesting it keeps asset_unused from flagging
    /// a skin-referenced import as unused, which would be a false lead to delete a live file.
    /// </summary>
    public static IEnumerable<string> OverrideFileValues(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        foreach (var name in OverrideFiles)
        {
            if (doc.GetFile(name) is not { CurrentBytes.Length: > 0 } entry) continue;
            foreach (var raw in ScriptText.GetString(entry.CurrentBytes).Split('\n'))
            {
                var line = raw.Trim();
                if (line.Length == 0 || line[0] is '[' or '/' or ';') continue;
                int eq = line.IndexOf('=');
                if (eq <= 0) continue;
                foreach (var token in line[(eq + 1)..].Split(',', StringSplitOptions.TrimEntries))
                    if (token.Length > 0) yield return token;
            }
        }
    }

    /// <summary>The map's script entry, JASS first then Lua, at the root or under scripts\.</summary>
    public static MapFileEntry? ScriptEntry(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        return doc.GetFile("war3map.j") ?? doc.GetFile("scripts\\war3map.j")
            ?? doc.GetFile("war3map.lua") ?? doc.GetFile("scripts\\war3map.lua");
    }

    /// <summary>
    /// True when a single path token names <paramref name="path"/> in any spelling the engine
    /// loads. The same expansion the reference set uses, so a token and an archive entry are
    /// judged by one rule.
    /// </summary>
    public static bool TokenNames(string token, string path)
    {
        if (token.Length == 0) return false;
        var wanted = Normalize(path);
        if (Normalize(token) == wanted) return true;
        if (!AssetPathCandidates.LooksLikeAssetPath(token)) return false;
        foreach (var candidate in AssetPathCandidates.Expand(token))
            if (Normalize(candidate) == wanted) return true;
        return false;
    }

    /// <summary>
    /// The parent folder of an internal path, "" becoming "(root)" so the size report has one
    /// label for files at the archive top rather than a blank.
    /// </summary>
    public static string FolderOf(string path)
    {
        int slash = path.LastIndexOfAny(new[] { '\\', '/' });
        return slash < 0 ? "(root)" : path[..slash];
    }
}
