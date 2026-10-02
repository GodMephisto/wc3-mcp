// src/Wc3.Commands/NameRecoveryCommand.cs
using System.Text;
using System.Text.RegularExpressions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One archive block that had no name and now has one, with where the name came from.</summary>
public sealed record RecoveredName(int BlockIndex, string Name, string Source, int SizeBytes);

/// <summary>
/// A block still carrying no name, so the report never implies more coverage than it has.
/// <see cref="Kind"/> and <see cref="SelfName"/> come from the bytes, because War3Net already
/// key-detects an encrypted block without its name, so content is readable even when the name
/// is not recoverable. On one real map all 793 unnamed entries read fine.
/// </summary>
public sealed record UnnamedBlock(int BlockIndex, int SizeBytes, string Kind, string SelfName);

public sealed record NameRecoveryResult(
    int TotalBlocks,
    int NamedBefore,
    int NamedAfter,
    IReadOnlyList<RecoveredName> Recovered,
    IReadOnlyList<UnnamedBlock> StillUnnamed,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Recovers file names for a protected map, which is the readable half of deprotection.
///
/// A protected map ships no <c>(listfile)</c>, so most of its entries have no name and
/// <see cref="MapDocument.GetFile"/> can never find them. Measured on one real 9.5 MB map, 793
/// of 851 entries carried no name and 833 were encrypted.
///
/// READING them is not the problem, and an earlier version of this note said it was. An MPQ
/// block's key is derived from its own basename, but a compressed block also stores a sector
/// offset table whose first entry is always that table's own size, which is known plaintext, so
/// the key is recoverable from the file itself. War3Net already does this, and all 793 unnamed
/// entries on that map read correctly. Content is therefore always available and only the NAME
/// is at stake, which is what this command recovers.
///
/// Measured contributions on that map. The map's own object data reached 456 of 851, its script
/// literals 598, a community listfile 808, the texture paths its models declare 831, and an
/// asset-name sweep of every block's bytes 833. The remaining 18 are named nowhere inside the
/// map, so the engine cannot load them by name either.
///
/// The order of sources matters for the report rather than for the result, because a block is
/// named if ANY source names it. Reporting the contribution per source is what makes a source
/// that adds nothing visible instead of assumed useful.
/// </summary>
public static class NameRecoveryCommand
{
    // Names every map has, whether or not it admits to them.
    private static readonly string[] Internal =
    {
        "war3map.j", "war3map.lua", "war3map.w3a", "war3map.w3u", "war3map.w3t", "war3map.w3h",
        "war3map.w3b", "war3map.w3d", "war3map.w3q", "war3map.w3e", "war3map.w3i", "war3map.wts",
        "war3map.doo", "war3map.w3r", "war3map.w3c", "war3map.w3s", "war3map.wpm", "war3map.shd",
        "war3mapUnits.doo", "war3mapMap.blp", "war3mapMap.tga", "war3mapMap.j", "war3mapPreview.tga",
        "war3mapSkin.txt", "war3map.imp", "war3mapMisc.txt", "war3mapExtra.txt", "war3map.mmp",
        "war3mapPath.tga", "war3map.wtg", "war3map.wct", "war3map.w3o", "war3map.w3v",
        "conversation.json", "(listfile)", "(attributes)", "(signature)",
    };

    // Object fields that hold a model, icon or effect path.
    private static readonly string[] ArtFields =
    {
        "umdl", "uico", "ussi", "uspa", "uubs", "ushu", "amat", "aart", "auar", "acat",
        "aeat", "atat", "bart", "ftat", "fart", "dfil", "bfil", "gfil", "iart", "ifil",
    };

    private static readonly string[] Extensions =
    {
        ".mdx", ".mdl", ".blp", ".tga", ".dds", ".wav", ".mp3", ".txt", ".slk", ".fdf", ".toc",
    };

    private static readonly string[] Prefixes =
    {
        "", @"war3mapImported\", @"Textures\", @"ReplaceableTextures\", @"Imported\",
        @"Models\", @"UI\", @"Sound\",
        // Icons live two levels down and nothing else reaches them. These two recovered
        // BTN_cr_RIGHTbooK.blp and BTNTheYellow.blp, which every other source missed.
        @"ReplaceableTextures\CommandButtons\", @"ReplaceableTextures\CommandButtonsDisabled\",
        @"ReplaceableTextures\PassiveButtons\",
    };

    private static readonly Regex Quoted = new(@"""([^""\r\n]{2,120})""", RegexOptions.Compiled);

    private static readonly Regex AssetName = new(
        @"[A-Za-z0-9_!.,'()\[\]&+ -]{1,90}\.(?:blp|tga|dds|mdx|mdl|wav|mp3)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Every spelling of one seed worth trying. A map routinely declares <c>Ulquiorra.mdl</c>
    /// while the archive stores <c>Ulquiorra.mdx</c>, and an import may or may not carry the
    /// <c>war3mapImported</c> prefix, so the declared string alone misses real files.
    /// </summary>
    internal static IEnumerable<string> Spellings(string seed)
    {
        yield return seed;
        string baseName = seed.Replace('/', '\\').Split('\\').Last();
        int dot = baseName.LastIndexOf('.');
        string stem = dot > 0 ? baseName[..dot] : baseName;
        if (stem.Length == 0) yield break;
        foreach (var prefix in Prefixes)
        {
            yield return prefix + baseName;
            foreach (var ext in Extensions)
                yield return prefix + stem + ext;
        }
    }

    /// <param name="dictionaries">
    /// Paths to listfiles, one name per line. Community listfiles and the listfiles of sibling
    /// maps both work. Missing files are reported as a diagnostic rather than thrown.
    /// </param>
    public static NameRecoveryResult Execute(
        MapDocument doc, IReadOnlyList<string>? dictionaries = null)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var diagnostics = new List<string>();
        int total = doc.Files.Count;
        int namedBefore = doc.Files.Count(f => f.FileName is not null);

        var recovered = new Dictionary<int, RecoveredName>();
        var unnamedSizes = doc.UnnamedBlocks();

        void Try(IEnumerable<string> seeds, string source, bool spread,
            Func<string, IEnumerable<string>>? extra = null)
        {
            var candidates = extra is not null ? seeds.SelectMany(extra)
                : spread ? seeds.SelectMany(Spellings)
                : seeds;
            foreach (var (block, name) in doc.RecoverNames(candidates))
                if (!recovered.ContainsKey(block))
                    recovered[block] = new RecoveredName(
                        block, name, source, unnamedSizes.GetValueOrDefault(block));
        }

        Try(Internal, "internal", spread: false);
        Try(ArtPaths(doc), "object data", spread: true);
        Try(ScriptLiterals(doc), "script literal", spread: true);

        foreach (var path in dictionaries ?? Array.Empty<string>())
        {
            if (!File.Exists(path))
            {
                diagnostics.Add($"dictionary not found, skipped: {path}");
                continue;
            }
            // Streamed, because a community listfile runs to tens of megabytes and holding one
            // in memory alongside the map is pure waste.
            Try(ReadLines(path), $"dictionary {Path.GetFileName(path)}", spread: false);
        }

        // Models name their textures. Every MDX declares what it uses in its TEXS chunk, 268
        // bytes per record holding a 260 byte path, and a BLP carries no internal name of its
        // own, so this is the only thing in the archive that can name one. It recovered 21
        // blocks on the map above, more than any dictionary managed at that point.
        Try(TexturePaths(doc), "model TEXS", spread: true);

        // A model states its own name in its MODL chunk, which is the only thing that can name
        // a block nothing else references. It recovered the one nameless MDX on the map above,
        // which calls itself CloudOfFog.
        Try(SelfDeclaredNames(doc), "self-declared", spread: true);

        // Finally an asset-name sweep of every block's bytes, because files name each other.
        Try(AssetNamesInContent(doc), "content sweep", spread: true);

        // One more pass, spelling variants of everything recovered so far. A texture usually
        // sits beside the model that uses it under a sibling name.
        Try(recovered.Values.Select(r => r.Name).ToList(), "variant", spread: true);

        // Icon twins, and this one is worth understanding because every other source is blind
        // to it BY CONSTRUCTION. An object's icon field names the enabled icon, BTNFoo.blp. The
        // engine then derives the greyed out twin ITSELF, as DISBTNFoo.blp in a sibling folder,
        // and the passive twin as PASBTNFoo.blp. Neither twin is named by any field, any script
        // literal, any model, or any dictionary, so nothing that reads the map can ever see them
        // while the game uses them normally.
        //
        // That is not a corner case here. On one real map the last 18 nameless entries were 16
        // DISBTN twins plus two unrelated files, and this single rule took recovery from 833 of
        // 851 to 849. The rule rewrites the STEM, which is why folder-only variants miss it.
        Try(recovered.Values.Select(r => r.Name)
                .Concat(doc.Files.Where(f => f.FileName is not null).Select(f => f.FileName!))
                .ToList(),
            "icon twin", spread: false, extra: IconTwins);

        var bytesByBlock = doc.Files.ToDictionary(f => f.BlockIndex, f => f.RawBytes);
        var still = unnamedSizes
            .Where(kv => !recovered.ContainsKey(kv.Key))
            .Select(kv =>
            {
                var (kind, selfName) = MapDocument.IdentifyBytes(
                    bytesByBlock.GetValueOrDefault(kv.Key, Array.Empty<byte>()));
                return new UnnamedBlock(kv.Key, kv.Value, kind, selfName);
            })
            .OrderByDescending(u => u.SizeBytes)
            .ToList();

        diagnostics.Add($"{total} blocks, {namedBefore} named before, "
            + $"{namedBefore + recovered.Count} after, {still.Count} still unnamed");

        return new NameRecoveryResult(
            total, namedBefore, namedBefore + recovered.Count,
            recovered.Values.OrderBy(r => r.BlockIndex).ToList(), still, diagnostics);
    }

    /// <summary>
    /// Stamps the recovered names onto their entries so the next Save writes them into the
    /// archive's listfile. Returns how many entries were named.
    /// </summary>
    /// <remarks>
    /// Those entries are re-added under their new name and therefore recompressed, so they are
    /// NOT byte-faithful afterwards. Every other entry is untouched. Writing a (listfile) as an
    /// ordinary file does not work at all, because MpqArchiveBuilder regenerates it from the
    /// names it knows, which was measured as 58 named entries before and 58 after.
    /// </remarks>
    public static int ApplyNames(MapDocument doc, NameRecoveryResult result)
    {
        ArgumentNullException.ThrowIfNull(doc);
        ArgumentNullException.ThrowIfNull(result);
        var byBlock = result.Recovered.ToDictionary(r => r.BlockIndex, r => r.Name);
        int n = 0;
        foreach (var entry in doc.Files)
            if (entry.FileName is null && byBlock.TryGetValue(entry.BlockIndex, out var name))
            {
                entry.RecoveredFileName = name;
                n++;
            }
        return n;
    }

    // The button folders and the stem prefixes the engine's own convention uses.
    private static readonly string[] IconFolders =
    {
        "", @"ReplaceableTextures\CommandButtons\",
        @"ReplaceableTextures\CommandButtonsDisabled\",
        @"ReplaceableTextures\PassiveButtons\", @"war3mapImported\",
    };

    private static readonly string[] IconStemPrefixes =
    {
        "", "BTN", "DISBTN", "PASBTN", "PAS", "ATC", "DISPAS", "DISATC",
    };

    /// <summary>
    /// Every icon name the engine's convention relates to this one, in both directions. From
    /// <c>BTNFoo.blp</c> it produces <c>DISBTNFoo.blp</c> and <c>PASBTNFoo.blp</c>, and from a
    /// bare <c>Foo</c> it produces all three, because a map may declare any of them.
    /// </summary>
    internal static IEnumerable<string> IconTwins(string seed)
    {
        string baseName = seed.Replace('/', '\\').Split('\\').Last();
        int dot = baseName.LastIndexOf('.');
        string stem = dot > 0 ? baseName[..dot] : baseName;
        if (stem.Length < 2) yield break;

        // Strip any known prefix so the core can be re-prefixed every other way. Longest first,
        // or DISBTNFoo would match BTN's rule and keep a stray DIS.
        var cores = new List<string> { stem };
        foreach (var p in new[] { "DISBTN", "DISPAS", "DISATC", "PASBTN", "BTN", "PAS", "ATC" })
            if (stem.StartsWith(p, StringComparison.OrdinalIgnoreCase) && stem.Length > p.Length)
            {
                cores.Add(stem[p.Length..]);
                break;
            }

        foreach (var core in cores)
            foreach (var prefix in IconStemPrefixes)
                foreach (var folder in IconFolders)
                {
                    yield return folder + prefix + core + ".blp";
                    yield return folder + prefix + core + ".tga";
                }
    }

    private static IEnumerable<string> TexturePaths(MapDocument doc)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in doc.Files)
        {
            var data = entry.OverrideBytes ?? entry.RawBytes;
            if (data.Length < 8 || data[0] != 'M' || data[1] != 'D' || data[2] != 'L' || data[3] != 'X')
                continue;
            int p = 4;
            while (p + 8 <= data.Length)
            {
                uint size = BitConverter.ToUInt32(data, p + 4);
                if (data[p] == 'T' && data[p + 1] == 'E' && data[p + 2] == 'X' && data[p + 3] == 'S')
                {
                    int limit = Math.Min(p + 8 + (int)size, data.Length);
                    for (int i = p + 8; i + 268 <= limit; i += 268)
                    {
                        int start = i + 4, end = start;
                        while (end < start + 260 && end < data.Length && data[end] != 0) end++;
                        if (end <= start) continue;
                        var s = Encoding.Latin1.GetString(data, start, end - start).Trim();
                        if (s.Length > 4 && seen.Add(s)) yield return s;
                    }
                }
                if (size == 0) break;
                p += 8 + (int)size;
            }
        }
    }

    /// <summary>Names entries state about themselves, which today means an MDX MODL chunk.</summary>
    private static IEnumerable<string> SelfDeclaredNames(MapDocument doc)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in doc.Files.Where(f => f.FileName is null))
        {
            var (_kind, selfName) = MapDocument.IdentifyBytes(entry.OverrideBytes ?? entry.RawBytes);
            if (selfName.Length > 1 && seen.Add(selfName)) yield return selfName;
        }
    }

    /// <summary>Anything asset-shaped in any block's bytes. The broadest and last source.</summary>
    private static IEnumerable<string> AssetNamesInContent(MapDocument doc)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in doc.Files)
        {
            var data = entry.OverrideBytes ?? entry.RawBytes;
            if (data.Length == 0) continue;
            foreach (Match m in AssetName.Matches(Encoding.Latin1.GetString(data)))
            {
                string s = m.Value.Trim();
                if (s.Length > 4 && seen.Add(s)) yield return s;
            }
        }
    }

    private static IEnumerable<string> ReadLines(string path)
    {
        using var reader = new StreamReader(path);
        while (reader.ReadLine() is { } line)
        {
            line = line.Trim();
            if (line.Length > 1) yield return line;
        }
    }

    private static IEnumerable<string> ArtPaths(MapDocument doc)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kind in ObjectKinds.All)
            foreach (var entry in ObjectKinds.MergedEntries(doc, ObjectKinds.Info(kind)))
                foreach (var (key, value) in entry.Mods)
                {
                    string code = key.Split(':')[0];
                    if (!ArtFields.Contains(code, StringComparer.OrdinalIgnoreCase)) continue;
                    foreach (var part in (value?.ToString() ?? "").Split(','))
                        if (part.Trim().Length > 2 && seen.Add(part.Trim()))
                            yield return part.Trim();
                }
    }

    private static IEnumerable<string> ScriptLiterals(MapDocument doc)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in new[] { "war3map.j", "war3map.lua", "war3mapSkin.txt",
                                     "war3mapMisc.txt", "war3mapExtra.txt" })
        {
            if (!doc.TryReadFileByName(name, out var bytes) || bytes.Length == 0) continue;
            // Latin-1 so no byte sequence can throw, and a path is ASCII anyway.
            string text = Encoding.Latin1.GetString(bytes);
            foreach (Match m in Quoted.Matches(text))
            {
                string s = m.Groups[1].Value.Trim();
                if (s.Length > 1 && seen.Add(s)) yield return s;
            }
        }
    }
}
