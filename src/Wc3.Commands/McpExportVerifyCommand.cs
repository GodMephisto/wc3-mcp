// src/Wc3.Commands/McpExportVerifyCommand.cs
using System.Text;
using System.Text.RegularExpressions;

namespace Wc3.Commands;

/// <summary>One file the two trees disagree on. Status is added, changed or removed.</summary>
public sealed record ExportDifference(string Status, string Path);

/// <summary>Whether the export is in step, the roots compared, and every difference found.</summary>
public sealed record McpExportVerifyResult(
    bool InStep,
    string SourceRoot,
    string TargetRoot,
    IReadOnlyList<ExportDifference> Differences,
    int CheckedFiles,
    string? Message);

/// <summary>
/// Verifies that a public wc3-mcp checkout holds the same server source that scripts/export-wc3-mcp.py
/// would export from this repository, reporting every file that differs and writing nothing. It
/// applies the same rules the export script does, so the comparison matches what a real export would
/// produce. The exported projects, the exported test folder, and the file the checkout owns all live
/// in the constants below and stay in step with the script's own lists.
/// </summary>
public static partial class McpExportVerifyCommand
{
    /// <summary>The projects copied into the wc3-mcp checkout, from the script's EXPORTED_PROJECTS.</summary>
    public static readonly string[] ExportedProjects =
    {
        "src/Wc3.MapDocument", "src/Wc3.GameData", "src/Wc3.Modeling",
        "src/Wc3.Render", "src/Wc3.Commands", "src/Wc3.Mcp",
    };

    public const string TestsDir = "tests/Wc3.Tests";
    public const string TestsProject = TestsDir + "/Wc3.Tests.csproj";

    /// <summary>Files the checkout owns and the export never writes, from the script's TARGET_OWNED.</summary>
    public static readonly string[] TargetOwned = { "src/Wc3.Mcp/README.md" };

    private static readonly HashSet<string> SkipDirectories =
        new(StringComparer.OrdinalIgnoreCase) { "bin", "obj", ".git", "node_modules" };

    [GeneratedRegex(@"\bWc3Ctl\b")]
    private static partial Regex CliUse();

    [GeneratedRegex("^\\s*<ProjectReference Include=\"[^\"]*wc3ctl\\.csproj\" />\\s*\\r?\\n", RegexOptions.Multiline)]
    private static partial Regex CliReference();

    /// <summary>Compare the source repository against a target wc3-mcp checkout, writing nothing.</summary>
    public static McpExportVerifyResult Execute(string sourceRoot, string targetRoot)
    {
        if (!Directory.Exists(sourceRoot))
            return Empty(sourceRoot, targetRoot, $"no directory at {sourceRoot}");
        if (!Directory.Exists(targetRoot))
            return Empty(sourceRoot, targetRoot, $"no directory at {targetRoot}");

        var source = BuildSource(sourceRoot);
        // A wrong source root (the home directory, say, when RepoLayout.FindRoot could not find a
        // wc3ctl checkout and the caller fell back to the process cwd) exists on disk but holds none
        // of the exported projects, so BuildSource yields nothing and every target file reads as
        // "removed". Refuse that rather than return a 700-file diff that compares against nothing.
        if (source.Count == 0)
            return Empty(sourceRoot, targetRoot,
                $"no wc3ctl source found under {sourceRoot}; expected {string.Join(", ", ExportedProjects)}. "
                + "Pass the wc3ctl repository with source_dir.");
        var target = BuildTarget(targetRoot);

        var differences = new List<ExportDifference>();
        foreach (var (path, bytes) in source.OrderBy(kv => kv.Key, StringComparer.Ordinal))
        {
            if (!target.TryGetValue(path, out var targetBytes))
                differences.Add(new ExportDifference("added", path));
            else if (!SameText(bytes, targetBytes))
                differences.Add(new ExportDifference("changed", path));
        }
        foreach (var path in target.Keys.OrderBy(p => p, StringComparer.Ordinal))
            if (!source.ContainsKey(path))
                differences.Add(new ExportDifference("removed", path));

        return new McpExportVerifyResult(differences.Count == 0, sourceRoot, targetRoot,
            differences, source.Count, null);
    }

    private static McpExportVerifyResult Empty(string sourceRoot, string targetRoot, string message) =>
        new(false, sourceRoot, targetRoot, Array.Empty<ExportDifference>(), 0, message);

    private static Dictionary<string, byte[]> BuildSource(string root)
    {
        var files = new Dictionary<string, byte[]>(StringComparer.Ordinal);
        foreach (var relative in Enumerate(root))
        {
            if (IsTargetOwned(relative)) continue;
            string full = Full(root, relative);
            if (string.Equals(relative, TestsProject, StringComparison.Ordinal))
            {
                string text = File.ReadAllText(full);
                files[relative] = Encoding.UTF8.GetBytes(CliReference().Replace(text, string.Empty));
                continue;
            }
            if (IsCliUsingTest(relative, full)) continue;
            files[relative] = File.ReadAllBytes(full);
        }
        return files;
    }

    private static Dictionary<string, byte[]> BuildTarget(string root)
    {
        var files = new Dictionary<string, byte[]>(StringComparer.Ordinal);
        foreach (var relative in Enumerate(root))
        {
            if (IsTargetOwned(relative)) continue;
            files[relative] = File.ReadAllBytes(Full(root, relative));
        }
        return files;
    }

    private static IEnumerable<string> Enumerate(string root)
    {
        foreach (var dir in ExportedProjects.Concat(new[] { TestsDir }))
        {
            string start = Full(root, dir);
            if (!Directory.Exists(start)) continue;
            foreach (var file in Directory.EnumerateFiles(start, "*", SearchOption.AllDirectories))
            {
                string relative = Path.GetRelativePath(root, file).Replace('\\', '/');
                if (relative.Split('/').Any(SkipDirectories.Contains)) continue;
                yield return relative;
            }
        }
    }

    private static bool IsTargetOwned(string relative) =>
        TargetOwned.Contains(relative, StringComparer.Ordinal);

    private static bool IsCliUsingTest(string relative, string fullPath) =>
        relative.StartsWith(TestsDir + "/", StringComparison.Ordinal)
        && relative.EndsWith(".cs", StringComparison.Ordinal)
        && CliUse().IsMatch(File.ReadAllText(fullPath));

    private static string Full(string root, string relative) =>
        Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));

    /// <summary>Byte comparison after normalizing line endings, the way the export script compares.</summary>
    private static bool SameText(byte[] a, byte[] b) =>
        string.Equals(Normalize(a), Normalize(b), StringComparison.Ordinal);

    private static string Normalize(byte[] bytes) =>
        Encoding.Latin1.GetString(bytes).Replace("\r\n", "\n");
}
