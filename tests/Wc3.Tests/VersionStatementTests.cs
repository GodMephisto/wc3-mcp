// tests/Wc3.Tests/VersionStatementTests.cs
// IDN-02, one product version. Directory.Build.props declares it, the build turns it into the
// assembly version that the exe, 'wc3ctl version', 'wc3ctl mcp version' and the MCP 'version_info'
// tool report, and every other file that states a version is compared with it here. A manifest
// edited by hand drifts the moment somebody forgets it, so the comparison fails the suite instead
// of shipping two numbers. The same file runs in both checkouts, wc3ctl and the wc3-mcp mirror,
// and each run also reads the other checkout when it sits beside this one, so either tree catches
// a disagreement across the pair.
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Wc3.Tests;

public class VersionStatementTests
{
    [Fact]
    public void Version_statements_agree_with_Directory_Build_Props()
    {
        string root = VersionStatements.RepoRoot();
        string props = VersionStatements.ReadProps(root)!;

        Assert.False(string.IsNullOrWhiteSpace(props), "Directory.Build.props states no Version");

        // The build bakes the props version into this assembly, and these two are what a caller
        // reads back. Comparing them here means the published exe cannot report a stale number.
        string mcpBuild = typeof(Wc3.Mcp.ServerIdentity).Assembly.GetName().Version?.ToString(3) ?? "0.0.0";
        Assert.True(
            mcpBuild == props,
            $"the Wc3.Mcp assembly reports {mcpBuild}, which is not the {props} Directory.Build.props declares. "
            + "Rebuild after changing the version.");
        Assert.True(
            Wc3.Mcp.ServerIdentity.Version == props,
            $"version_info reports {Wc3.Mcp.ServerIdentity.Version}, which is not the {props} Directory.Build.props declares.");

        var disagreements = VersionStatements.Compare(root);
        Assert.True(
            disagreements.Count == 0,
            "These version statements disagree with Directory.Build.props at " + root + Environment.NewLine
            + string.Join(Environment.NewLine, disagreements));
    }

    [Fact]
    public void Version_statement_check_reports_a_drifting_manifest()
    {
        // Proves the comparison above is not vacuous. A tree whose npm package carries an older
        // number must be named, with the file and both values, and the agreeing files must not be.
        using var tmp = new TempDir();
        File.WriteAllText(
            Path.Combine(tmp.Dir, "Directory.Build.props"),
            "<Project>\n  <PropertyGroup>\n    <Version>1.2.3</Version>\n  </PropertyGroup>\n</Project>\n");
        Directory.CreateDirectory(Path.Combine(tmp.Dir, "npm"));
        File.WriteAllText(
            Path.Combine(tmp.Dir, "npm", "package.json"),
            "{ \"name\": \"wc3-mcp\", \"version\": \"1.2.2\" }\n");
        Directory.CreateDirectory(Path.Combine(tmp.Dir, "mcpb"));
        File.WriteAllText(
            Path.Combine(tmp.Dir, "mcpb", "manifest.json"),
            "{ \"version\": \"1.2.3\" }\n");
        File.WriteAllText(
            Path.Combine(tmp.Dir, "server.json"),
            "{ \"version\": \"1.2.3\", \"packages\": [ { \"version\": \"1.2.3\" } ] }\n");

        var found = VersionStatements.Compare(tmp.Dir, lookForOther: false);

        Assert.Single(found);
        Assert.Contains("npm/package.json", found[0]);
        Assert.Contains("1.2.2", found[0]);
    }
}

/// <summary>
/// Reads every committed statement of the product version under a checkout and names each one that
/// differs from that checkout's Directory.Build.props. The exe and the tools are not files, they
/// read the assembly version, so the test compares those directly instead.
/// </summary>
internal static class VersionStatements
{
    // The files this product ships that state a version and cannot read the props file. A new one
    // belongs here as well, or it drifts unseen.
    private static readonly string[] ManifestPaths =
    {
        "npm/wc3-mcp/package.json",
        "npm/package.json",
        "mcpb/manifest.json",
        "server.json",
    };

    private static readonly Regex VersionElement = new(@"<Version>\s*([^<\s]+)\s*</Version>", RegexOptions.Compiled);

    /// <summary>Walk up from the test binaries to the checkout that holds Directory.Build.props.</summary>
    internal static string RepoRoot()
    {
        for (DirectoryInfo? dir = new(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
        {
            if (File.Exists(Path.Combine(dir.FullName, "Directory.Build.props")))
                return dir.FullName;
        }
        throw new InvalidOperationException("no Directory.Build.props above " + AppContext.BaseDirectory);
    }

    /// <summary>The Version element, or null when the file or the element is missing.</summary>
    internal static string? ReadProps(string root)
    {
        string path = Path.Combine(root, "Directory.Build.props");
        if (!File.Exists(path)) return null;
        var match = VersionElement.Match(File.ReadAllText(path));
        return match.Success ? match.Groups[1].Value : null;
    }

    /// <summary>Every statement in the checkout, and in the checkout beside it, that is not the reference.</summary>
    internal static List<string> Compare(string root, bool lookForOther = true)
    {
        var found = new List<string>();
        string? reference = ReadProps(root);
        if (string.IsNullOrWhiteSpace(reference))
        {
            found.Add("Directory.Build.props states no Version, so nothing can be compared");
            return found;
        }

        CompareTree(root, root, reference, found);

        string? sibling = lookForOther ? Sibling(root) : null;
        if (sibling is not null)
        {
            string? siblingVersion = ReadProps(sibling);
            if (siblingVersion is null)
                found.Add("the other checkout at " + sibling + " states no Version");
            else if (siblingVersion != reference)
                found.Add(Relative(root, Path.Combine(sibling, "Directory.Build.props"))
                    + " states " + siblingVersion + ", which is not " + reference);
            CompareTree(root, sibling, reference, found);
        }

        return found;
    }

    private static void CompareTree(string root, string tree, string reference, List<string> found)
    {
        foreach (string relative in ManifestPaths)
        {
            string path = Path.Combine(tree, relative);
            if (!File.Exists(path)) continue;
            foreach ((string where, string value) in ReadJsonVersions(path))
            {
                if (value == reference) continue;
                found.Add((tree == root ? relative : Relative(root, path)) + where
                    + " states " + value + ", which is not " + reference);
            }
        }
    }

    /// <summary>Every string under a key named version, labelled by its position in the document.</summary>
    private static IEnumerable<(string Where, string Value)> ReadJsonVersions(string path)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        var found = new List<(string, string)>();
        Walk(document.RootElement, string.Empty, found);
        return found;
    }

    private static void Walk(JsonElement element, string where, List<(string Where, string Value)> found)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    string path = where.Length == 0 ? property.Name : where + "." + property.Name;
                    if (property.NameEquals("version") && property.Value.ValueKind == JsonValueKind.String)
                        found.Add((path, property.Value.GetString()!));
                    else
                        Walk(property.Value, path, found);
                }
                break;
            case JsonValueKind.Array:
                int index = 0;
                foreach (var item in element.EnumerateArray())
                {
                    Walk(item, where + "[" + index + "]", found);
                    index++;
                }
                break;
        }
    }

    /// <summary>The wc3ctl checkout next to a wc3-mcp checkout, or the mirror next to wc3ctl.</summary>
    private static string? Sibling(string root)
    {
        var self = new DirectoryInfo(root);
        string wanted = string.Equals(self.Name, "Wc3_CLI", StringComparison.OrdinalIgnoreCase) ? "wc3-mcp" : "Wc3_CLI";
        if (self.Parent is null) return null;
        string candidate = Path.Combine(self.Parent.FullName, wanted);
        return File.Exists(Path.Combine(candidate, "Directory.Build.props")) ? candidate : null;
    }

    private static string Relative(string root, string path)
    {
        try
        {
            return Path.GetRelativePath(root, path).Replace('\\', '/');
        }
        catch (ArgumentException)
        {
            return path.Replace('\\', '/');
        }
    }
}
