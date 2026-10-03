using System.Text.RegularExpressions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Which internal files to extract. Exactly one selection mode applies:
/// All, an exact internal name, or one or more glob patterns.
/// </summary>
public sealed record ExtractSelector
{
    public string? ExactName { get; init; }
    public IReadOnlyList<string> Patterns { get; init; } = Array.Empty<string>();
    public bool All { get; init; }

    /// <summary>Expands a convenience group (models/textures/sounds) into glob patterns.</summary>
    public static IReadOnlyList<string> GroupPatterns(string group) => group.ToLowerInvariant() switch
    {
        "models" => new[] { "*.mdx", "*.mdl" },
        "textures" => new[] { "*.blp", "*.tga", "*.dds" },
        "sounds" => new[] { "*.mp3", "*.wav" },
        _ => throw new ArgumentException($"unknown file group '{group}'", nameof(group)),
    };
}

public static class ExtractCommand
{
    /// <summary>Case-insensitive glob match ('*' wildcard) against a full internal path.</summary>
    public static bool GlobMatch(string pattern, string name) =>
        Regex.IsMatch(name, "^" + Regex.Escape(pattern).Replace(@"\*", ".*") + "$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    /// <summary>
    /// Selects matching entries (with their bytes) — no filesystem access here;
    /// the CLI layer is responsible for writing.
    /// </summary>
    public static ExtractResult Execute(MapDocument doc, ExtractSelector selector)
    {
        IEnumerable<MapFileEntry> picked;
        if (selector.All)
            picked = doc.Files;                              // includes unnamed (protected) entries
        else if (selector.ExactName is { } byBlock && byBlock.StartsWith('#')
                 && int.TryParse(byBlock.AsSpan(1), out int block))
            // "#1155" is block 1155, the way deprotect lists an entry no dictionary could name.
            picked = doc.Files.Where(f => f.BlockIndex == block);
        else if (selector.ExactName is { } exact)
            // Through GetFile, so an exact name resolves the same way everywhere. Matching
            // doc.Files directly here meant `extract war3map.j` answered "file not found" on the
            // 13 of 34 measured maps that store it as scripts\war3map.j, a file `ls` had just
            // listed on the line above.
            picked = doc.GetFile(exact) is { } hit
                ? new[] { hit }
                : Enumerable.Empty<MapFileEntry>();
        else
            picked = doc.Files.Where(f => f.FileName != null && selector.Patterns.Any(p => GlobMatch(p, f.FileName!)));

        return new ExtractResult(picked.Select(f => new ExtractedItem(f.FileName, f.BlockIndex, f.CurrentBytes)).ToList());
    }
}
