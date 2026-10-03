// tests/Wc3.Tests/CorpusMap.cs
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Resolves the real map the Corpus-category tests run against.
///
/// Every one of those tests used to hardcode the same file name and skip silently when it was
/// absent. That name went stale, the file was gone, and ten test files' worth of Corpus checks
/// quietly stopped running while still reporting green. A byte-faithfulness test that cannot find
/// its map does not fail, it evaporates, which is the worst possible way for a fidelity check to
/// behave.
///
/// Two later attempts at the resolver got it wrong in opposite directions, and both are worth
/// recording because both looked reasonable.
///
/// Picking the LARGEST map on disk maximised coverage in theory and in practice pushed the suite
/// past ten minutes, because every corpus test loads its map at least twice and the largest map
/// here is 250 MB.
///
/// Picking the SMALLEST map above a size floor was fast and picked a map with no imported models,
/// so the model tests skipped and the suite was fast for the same reason it had been fast before,
/// which is to say it was not testing anything.
///
/// Size is a proxy for content and a bad one. So candidates are now tried in ascending size and
/// the first one that actually CONTAINS what the corpus needs wins. Opening a small archive costs
/// well under a second, so verifying beats guessing.
/// </summary>
internal static class CorpusMap
{
    private const string EnvVar = "WC3CTL_CORPUS_MAP";

    // The same folders TestCorpus resolves, WC3_CORPUS_DIR first when set, then the
    // current user's Documents\Warcraft III\Maps, so no machine's own path is written here.
    private static readonly string[] SearchFolders =
        new[] { TestCorpus.Directory, TestCorpus.MapsRoot }.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

    /// <summary>Below this a map is a test fixture or a stub, not content worth exercising.</summary>
    private const long MinBytes = 1L * 1024 * 1024;

    /// <summary>Above this, loading it twice per test costs more than the coverage is worth. The
    /// env var overrides this for anyone who wants a specific large map.</summary>
    private const long MaxBytes = 40L * 1024 * 1024;

    private static readonly Lazy<string?> Resolved = new(Resolve);

    /// <summary>The map to test against, or null when this machine has no suitable one.</summary>
    public static string? Path => Resolved.Value;

    /// <summary>Empty rather than null, so an existing <c>File.Exists(CorpusPath)</c> guard keeps
    /// working unchanged and still skips when nothing was found.</summary>
    public static string PathOrEmpty => Resolved.Value ?? "";

    public static bool Available => Resolved.Value is not null;

    /// <summary>What a test run should say about its corpus, so a skip is never invisible.</summary>
    public static string Describe() =>
        Resolved.Value is { } p
            ? $"corpus map: {System.IO.Path.GetFileName(p)} "
              + $"({new FileInfo(p).Length / (1024 * 1024)} MB)"
            : $"no usable corpus map found. Set {EnvVar}, or place a .w3x between "
              + $"{MinBytes / (1024 * 1024)} and {MaxBytes / (1024 * 1024)} MB carrying terrain, "
              + $"unit object data and at least one imported model, in "
              + string.Join(" or ", SearchFolders);

    /// <summary>
    /// A map is usable as the corpus when it carries the three things the Corpus tests actually
    /// reach for: terrain, unit object data, and at least one imported model. Stated here rather
    /// than assumed, so a map that cannot exercise the suite is never selected.
    /// </summary>
    private static bool IsUsable(string path)
    {
        try
        {
            var doc = MapDocument.Load(path);
            if (doc.GetFile("war3map.w3e") is null) return false;
            if (doc.GetFile("war3map.w3u") is null) return false;
            return doc.Files.Any(f => f.FileName is { } n
                && n.EndsWith(".mdx", StringComparison.OrdinalIgnoreCase)
                && n.StartsWith("war3mapImported", StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            // A protected or truncated archive is simply not a candidate.
            return false;
        }
    }

    private static string? Resolve()
    {
        var pinned = Environment.GetEnvironmentVariable(EnvVar);
        if (!string.IsNullOrWhiteSpace(pinned) && File.Exists(pinned)) return pinned;

        foreach (var folder in SearchFolders)
        {
            if (!Directory.Exists(folder)) continue;
            var candidates = new DirectoryInfo(folder)
                .EnumerateFiles("*.w3x", SearchOption.TopDirectoryOnly)
                .Where(f => f.Length >= MinBytes && f.Length <= MaxBytes)
                .OrderBy(f => f.Length);

            foreach (var c in candidates)
                if (IsUsable(c.FullName))
                    return c.FullName;
        }
        return null;
    }
}
