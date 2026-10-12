// src/Wc3.Commands/BenchMapFolder.cs
namespace Wc3.Commands;

/// <summary>
/// Finds every .w3x and .w3m under a folder, recursively, in one deterministic order. Shared by
/// the bench commands that sweep a folder so they all agree on what a folder of maps means.
/// </summary>
internal static class BenchMapFolder
{
    /// <summary>Every map file under <paramref name="folder"/>, sorted case-insensitively by path.</summary>
    public static IReadOnlyList<string> Enumerate(string folder)
    {
        if (!Directory.Exists(folder))
            throw new DirectoryNotFoundException("map folder not found " + folder);

        return Directory.EnumerateFiles(folder, "*.w3?", SearchOption.AllDirectories)
            .Where(f => f.EndsWith(".w3x", StringComparison.OrdinalIgnoreCase)
                     || f.EndsWith(".w3m", StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
