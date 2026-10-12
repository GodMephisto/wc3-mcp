// src/Wc3.Commands/RepoLayout.cs
namespace Wc3.Commands;

/// <summary>
/// Locates the wc3ctl repository from a starting directory. Release tooling reads and writes files
/// that live beside the solution, and the published binaries run outside the checkout, so this walks
/// up looking for the solution file rather than assuming a fixed relative path.
/// </summary>
public static class RepoLayout
{
    public const string SolutionFileName = "Wc3.sln";
    public const string BuildPropsFileName = "Directory.Build.props";

    /// <summary>The repository root, found by walking up for the solution file, or null when none.</summary>
    public static string? FindRoot(string? start = null)
    {
        foreach (var seed in Seeds(start))
        {
            var dir = seed;
            while (dir is { Length: > 0 })
            {
                if (File.Exists(Path.Combine(dir, SolutionFileName))) return dir;
                dir = Directory.GetParent(dir)?.FullName;
            }
        }
        return null;
    }

    /// <summary>The repository Directory.Build.props, where the one product version lives, or null.</summary>
    public static string? FindBuildProps(string? start = null) =>
        FindRoot(start) is { } root ? Path.Combine(root, BuildPropsFileName) : null;

    private static IEnumerable<string> Seeds(string? start)
    {
        if (start is { Length: > 0 }) yield return Path.GetFullPath(start);
        yield return Directory.GetCurrentDirectory();
        yield return AppContext.BaseDirectory;
    }
}
