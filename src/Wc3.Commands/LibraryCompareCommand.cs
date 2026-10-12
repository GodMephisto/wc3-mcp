// src/Wc3.Commands/LibraryCompareCommand.cs
using System.Security.Cryptography;

namespace Wc3.Commands;

/// <summary>
/// The diff between two folders, by map file name. OnlyInA and OnlyInB name a map present in one
/// folder and not the other, Changed names a map in both whose content hash differs, and Same is
/// the count of maps present in both with an identical hash. AmbiguousNames lists a file name that
/// appears more than once inside one folder, where a file-name comparison cannot decide, so it is
/// reported rather than guessed at.
/// </summary>
public sealed record LibraryCompareResult(
    IReadOnlyList<string> OnlyInA,
    IReadOnlyList<string> OnlyInB,
    IReadOnlyList<string> Changed,
    IReadOnlyList<string> AmbiguousNames,
    int Same);

/// <summary>
/// Compare two folders of maps by file name and content hash. A map is identified by its file name,
/// so a map that moved between subfolders still matches, and compared by the SHA-256 of the whole
/// file, so a map with the same name but different bytes reads as changed. Read-only, it opens each
/// file only to hash it and never loads a map.
///
/// Files are hashed one at a time, so a comparison never holds a whole library in memory at once.
/// </summary>
public static class LibraryCompareCommand
{
    public static LibraryCompareResult Execute(string folderA, string folderB)
    {
        var a = Index(folderA);
        var b = Index(folderB);

        var onlyA = new List<string>();
        var onlyB = new List<string>();
        var changed = new List<string>();
        var ambiguous = new List<string>();
        int same = 0;

        var names = a.Keys.Union(b.Keys, StringComparer.OrdinalIgnoreCase)
            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase);
        foreach (var name in names)
        {
            a.TryGetValue(name, out var inA);
            b.TryGetValue(name, out var inB);
            int countA = inA?.Count ?? 0;
            int countB = inB?.Count ?? 0;

            // A file name that is not unique inside a folder cannot be compared by name without
            // guessing which copy the other folder means. Report it instead of picking one.
            if (countA > 1 || countB > 1)
            {
                ambiguous.Add(name);
                continue;
            }

            if (countA == 1 && countB == 0) onlyA.Add(name);
            else if (countA == 0 && countB == 1) onlyB.Add(name);
            else if (countA == 1 && countB == 1)
            {
                if (string.Equals(inA![0], inB![0], StringComparison.Ordinal)) same++;
                else changed.Add(name);
            }
        }

        return new LibraryCompareResult(onlyA, onlyB, changed, ambiguous, same);
    }

    /// <summary>Each map file name under a folder to the SHA-256 hashes of every file with it.</summary>
    private static Dictionary<string, List<string>> Index(string folder)
    {
        var index = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var path in BenchMapFolder.Enumerate(folder))
        {
            string name = Path.GetFileName(path);
            if (!index.TryGetValue(name, out var hashes)) index[name] = hashes = new List<string>();
            hashes.Add(Hash(path));
        }
        return index;
    }

    /// <summary>The lowercase SHA-256 hex of a file, read in one streaming pass.</summary>
    private static string Hash(string path)
    {
        using var stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant();
    }
}
