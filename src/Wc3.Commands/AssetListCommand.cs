// src/Wc3.Commands/AssetListCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>Media family an asset-path field holds, dispatched from the field's metadata type
/// token exactly as the World Editor and HiveWE dispatch it.</summary>
public enum AssetFamily { Icon, Model }

/// <summary>
/// Asset paths available to an object field, split by where they come from. A map path is an
/// archive entry name, which is exactly the string an object field must hold. A game path is the
/// logical path after the w3mod storage prefixes, likewise.
/// </summary>
public sealed record AssetListResult(
    AssetFamily Family,
    IReadOnlyList<string> MapPaths,
    IReadOnlyList<string> GamePaths,
    string? Diagnostic = null)
{
    /// <summary>Every path, the map's first because a map's own import shadows a base-game file
    /// of the same name, then the base game's, duplicates folded.</summary>
    public IReadOnlyList<string> All => MapPaths
        .Concat(GamePaths)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToList();
}

/// <summary>
/// Lists the asset paths an icon or model field can actually be set to, rather than leaving
/// someone to type one and find out later that nothing resolves.
///
/// This lives in the command layer, not in the GUI, because the GUI is not the only thing that
/// needs it. It began as a Studio helper and a picker is only one use of the answer, an agent
/// choosing a model through MCP and a person checking what a map ships from a terminal need the
/// same list.
///
/// Family membership is derived through <see cref="AssetPathCandidates"/> rather than a second
/// extension table, so this and the archive prober cannot disagree about what counts as a model.
/// </summary>
public static class AssetListCommand
{
    /// <summary>Enumeration cap. Reforged's listfile holds a few tens of thousands per family, so
    /// this is unreachable in practice and exists only so a pathological listfile cannot balloon
    /// the result without bound.</summary>
    private const int MaxGamePaths = 200_000;

    /// <summary>The family for a metadata type token, or null when the field is not an asset
    /// path. Nothing outside the two the metadata names is guessed at.</summary>
    public static AssetFamily? FamilyForFieldType(string type) => type.ToLowerInvariant() switch
    {
        "icon" => AssetFamily.Icon,
        "model" => AssetFamily.Model,
        _ => null,
    };

    /// <summary>Parses a family name as a caller would type it.</summary>
    public static AssetFamily ParseFamily(string token) =>
        Enum.TryParse<AssetFamily>(token, ignoreCase: true, out var f)
            ? f
            : throw new ArgumentException(
                $"unknown asset family '{token}', expected icon or model");

    /// <summary>
    /// The paths of a family, from the map when one is given and from the base game when an
    /// install answers. Never throws for a missing install, it reports the reason instead, because
    /// a machine with no Warcraft III must still be able to list what a map itself holds.
    /// </summary>
    public static AssetListResult Execute(MapDocument? doc, AssetFamily family, string? gameDirOverride)
    {
        var mapPaths = doc is null ? Array.Empty<string>() : MapPaths(doc, family);

        if (!GameData.GameData.TryOpen(gameDirOverride, out var ctx, out var diagnostic) || ctx is null)
            return new AssetListResult(family, mapPaths, Array.Empty<string>(), diagnostic);

        return new AssetListResult(family, mapPaths, GamePaths(ctx, family));
    }

    /// <summary>The map's own entries of the family, sorted, duplicates folded.</summary>
    public static IReadOnlyList<string> MapPaths(MapDocument doc, AssetFamily family)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var exts = FamilyExtensions(family);
        return doc.Files
            .Select(f => f.FileName)
            .Where(n => n is not null
                        && exts.Contains(Path.GetExtension(n), StringComparer.OrdinalIgnoreCase))
            .Select(n => n!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>
    /// The base game's paths of the family. One listfile pass, narrowed before the extension
    /// filter so a full enumeration is not walked twice.
    /// </summary>
    public static IReadOnlyList<string> GamePaths(GameData.GameDataContext ctx, AssetFamily family)
    {
        ArgumentNullException.ThrowIfNull(ctx);

        // Icons narrow by directory, since every icon field the game reads points into
        // ReplaceableTextures\CommandButtons or its Disabled sibling, and both contain this
        // needle. Models narrow by extension, since the base game ships binary .mdx only.
        var needle = family == AssetFamily.Icon
            ? @"replaceabletextures\commandbuttons"
            : ".mdx";
        var exts = FamilyExtensions(family);
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var result = new List<string>();

        foreach (var name in ctx.FindFiles(needle, MaxGamePaths))
        {
            // A storage path carries w3mod layer prefixes ("war3.w3mod:_hd.w3mod:units\..."). An
            // object field stores the logical path after the last prefix, and HD and SD ship the
            // same logical names, so the dedup folds the layers to one entry.
            var logical = name[(name.LastIndexOf(':') + 1)..];
            if (!exts.Contains(Path.GetExtension(logical), StringComparer.OrdinalIgnoreCase))
                continue;
            if (seen.Add(logical))
                result.Add(logical);
        }

        result.Sort(StringComparer.OrdinalIgnoreCase);
        return result;
    }

    /// <summary>
    /// The family's extensions, read out of <see cref="AssetPathCandidates.Expand"/> by expanding
    /// one seed per family. Expand answers "which sibling spellings might this file exist under",
    /// which for a bare seed is precisely the extension family, so the path rules stay defined in
    /// exactly one place.
    /// </summary>
    public static IReadOnlyList<string> FamilyExtensions(AssetFamily family) =>
        AssetPathCandidates.Expand(family == AssetFamily.Model ? "seed.mdx" : "seed.blp")
            .Select(Path.GetExtension)
            .Where(e => e is { Length: > 0 })
            .Select(e => e!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
}
