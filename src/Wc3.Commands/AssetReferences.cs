// src/Wc3.Commands/AssetReferences.cs
using System.Buffers.Binary;
using System.Text;
using System.Text.RegularExpressions;
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
        foreach (var value in ModelTextureValues(doc)) Add(value);
        foreach (var value in UiFileValues(doc)) Add(value);
        foreach (var value in StructuralFileValues(doc)) Add(value);
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

    /// <summary>
    /// Every texture path an imported model names in its own texture chunk. A texture reached only
    /// from a model is live even though no object-data field and no script literal names it, so a
    /// reference set without these would tell asset_unused to delete a texture a model still draws.
    /// An .mdx is read by walking its chunk list to TEXS; an .mdl is read from its Bitmap blocks.
    /// </summary>
    public static IEnumerable<string> ModelTextureValues(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        foreach (var entry in ImportedEntries(doc))
        {
            var name = entry.FileName!;
            var ext = Path.GetExtension(name);
            var bytes = entry.CurrentBytes;
            if (ext.Equals(".mdx", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var texture in MdxTexturePaths(bytes))
                    yield return texture;
            }
            else if (ext.Equals(".mdl", StringComparison.OrdinalIgnoreCase))
            {
                if (bytes.Length == 0) continue;
                var text = ScriptText.GetString(bytes);
                // MDL writes it as Bitmap { Image "path" }, so the quoted path follows the keyword.
                foreach (Match m in Regex.Matches(text,
                             @"\b(?:Image|Texture)\s+""([^""]+)""", RegexOptions.IgnoreCase))
                {
                    var value = m.Groups[1].Value.Trim();
                    if (value.Length > 0) yield return value;
                }
            }

            // Catch any other path the file embeds, a particle emitter 1 spawn model being the one
            // that matters. PREM carries a model path outside the texture chunk, and the emitter
            // reader does not expose it, so scanning the bytes is what keeps asset_unused from
            // offering a model a live emitter still spawns.
            foreach (var embedded in EmbeddedAssetPaths(bytes))
                yield return embedded;
        }
    }

    /// <summary>
    /// Every asset path named in a map-format file rather than in object data or the script. The
    /// w3i names the loading screen model, the w3e names every terrain tile texture, and the w3s
    /// names every sound. None of these is an object-data field, so without this a loading screen
    /// or a custom terrain tile reads as unused and a prune deletes a file the game still loads.
    /// Every structural entry is scanned, which over-approximates on purpose.
    /// </summary>
    public static IEnumerable<string> StructuralFileValues(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        foreach (var file in doc.Files)
        {
            var name = file.FileName;
            if (name is null || IsImportedEntry(name)) continue;
            // The MPQ bookkeeping entries, (listfile) above all, name every file in the archive, so
            // scanning one would mark every import used. The script is already read as literals.
            if (name.StartsWith('(')) continue;
            if (name.EndsWith(".j", StringComparison.OrdinalIgnoreCase)
                || name.EndsWith(".lua", StringComparison.OrdinalIgnoreCase)) continue;
            foreach (var value in EmbeddedAssetPaths(file.CurrentBytes))
                yield return value;
        }
    }

    /// <summary>
    /// Every path-like string inside a binary, so a reference the structured readers do not model
    /// is still seen. Conservative on purpose, since a false "used" only keeps a file.
    /// </summary>
    internal static IEnumerable<string> EmbeddedAssetPaths(byte[] data)
    {
        if (data.Length == 0) yield break;
        var text = Encoding.Latin1.GetString(data);
        foreach (Match m in EmbeddedAsset.Matches(text))
        {
            var value = m.Value.TrimStart('\0', ' ', '\t', '\r', '\n');
            if (value.Length > 0) yield return value;
        }
    }

    private static readonly Regex EmbeddedAsset = new(
        @"[A-Za-z0-9_\\/ .\-\[\]()]{1,180}\.(?:mdx|mdl|blp|tga|dds|png|jpg|jpeg|fdf|toc|wav|mp3)\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Every asset path named in the map's own UI files, war3mapSkin.txt, war3mapMisc.txt, and the
    /// .fdf and .toc files a custom interface carries. An interface file can name a texture no
    /// object field and no script literal does, so it belongs in the reference set.
    /// </summary>
    public static IEnumerable<string> UiFileValues(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        foreach (var entry in ImportedEntries(doc))
        {
            var name = entry.FileName!;
            var ext = Path.GetExtension(name);
            if (!ext.Equals(".fdf", StringComparison.OrdinalIgnoreCase)
                && !ext.Equals(".toc", StringComparison.OrdinalIgnoreCase)) continue;
            foreach (var value in EmbeddedAssetPaths(entry.CurrentBytes))
                yield return value;
        }
    }

    /// <summary>Texture paths from an MDX TEXS chunk. Each entry is replaceableId, char[260] path,
    /// flags. A replaceable entry has an empty path and is skipped.</summary>
    private static IEnumerable<string> MdxTexturePaths(byte[] data)
    {
        if (data.Length < 8 || data[0] != (byte)'M' || data[1] != (byte)'D'
            || data[2] != (byte)'L' || data[3] != (byte)'X')
            yield break;
        int offset = 4;
        while (offset + 8 <= data.Length)
        {
            int size = BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(offset + 4, 4));
            if (size < 0 || offset + 8 + size > data.Length) yield break;
            if (data[offset] == (byte)'T' && data[offset + 1] == (byte)'E'
                && data[offset + 2] == (byte)'X' && data[offset + 3] == (byte)'S')
            {
                int count = size / 268;
                for (int i = 0; i < count; i++)
                {
                    int entry = offset + 8 + i * 268;
                    if (entry + 264 > data.Length) break;
                    int end = entry + 4;
                    while (end < entry + 264 && data[end] != 0) end++;
                    if (end <= entry + 4) continue;
                    yield return Encoding.ASCII.GetString(data, entry + 4, end - entry - 4);
                }
            }
            offset += 8 + size;
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
