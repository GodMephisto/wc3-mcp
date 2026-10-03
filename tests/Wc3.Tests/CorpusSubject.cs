// tests/Wc3.Tests/CorpusSubject.cs
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Finds the thing a Corpus test acts on inside whatever map <see cref="CorpusMap"/> resolved.
///
/// Naming the subject is what broke the Corpus suite. Several tests hardcoded a model path from
/// one particular map, that map went away, and the tests skipped in silence rather than failing.
/// A test that names its subject is pinned to one file on one machine. A test that FINDS its
/// subject states the property it actually cares about, which is the point of the test in the
/// first place.
/// </summary>
internal static class CorpusSubject
{
    /// <summary>
    /// Imported models in the map, largest first. Largest first because a big model is a real unit
    /// with geometry, textures and usually a skeleton, while a small one is an effect that parses
    /// fine and proves very little.
    /// </summary>
    public static IEnumerable<MapFileEntry> ImportedModels(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        return doc.Files
            .Where(f => f.FileName is { } n
                        && n.EndsWith(".mdx", StringComparison.OrdinalIgnoreCase)
                        && n.StartsWith("war3mapImported", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(f => f.RawBytes.Length);
    }

    /// <summary>
    /// The largest imported model's path, spelled with the <c>.mdl</c> extension that object data
    /// uses even though the archive stores <c>.mdx</c>. Returning the <c>.mdl</c> form on purpose,
    /// since that mismatch is normal in this format and every lookup has to survive it.
    /// Null when the map has no imported models.
    /// </summary>
    public static string? LargestModelAsMdl(MapDocument doc) =>
        ImportedModels(doc).FirstOrDefault()?.FileName is { } n
            ? n[..^4] + ".mdl"
            : null;

    /// <summary>
    /// The first imported model that satisfies <paramref name="accept"/>, tried largest first.
    /// Lets a test say "any model with a skeleton" instead of naming one and hoping.
    /// A model that fails to parse is skipped rather than throwing, because one bad model in a
    /// third-party map is not what the caller is testing.
    /// </summary>
    public static MapFileEntry? FirstModelWhere(MapDocument doc, Func<MapFileEntry, bool> accept)
    {
        ArgumentNullException.ThrowIfNull(accept);
        foreach (var entry in ImportedModels(doc))
        {
            try
            {
                if (accept(entry)) return entry;
            }
            catch
            {
                // Unparseable or unexpected shape. Try the next one.
            }
        }
        return null;
    }
}
