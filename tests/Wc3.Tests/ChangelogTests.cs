// tests/Wc3.Tests/ChangelogTests.cs
using System.IO;
using System.Text.RegularExpressions;
using Wc3.Commands;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// Guards the changelog layout of the wc3ctl repository: one file per release under
/// <c>changelog/</c>, with <c>CHANGELOG.md</c> showing the latest. The version this build carries
/// must have its own file, so bumping the version without adding one fails here rather than at
/// review time or, worse, at a release that ships with no notes for it.
///
/// The wc3-mcp mirror carries this file but owns no <c>Wc3.sln</c> and no changelog, so both
/// checks no-op there. The guard is about the wc3ctl release layout, which that checkout does not
/// have.
/// </summary>
public class ChangelogTests
{
    /// <summary>The wc3ctl repository root, or null when this is not the wc3ctl checkout.</summary>
    private static string? Root() => RepoLayout.FindRoot();

    private static string CurrentVersion(string root)
    {
        var match = Regex.Match(
            File.ReadAllText(Path.Combine(root, "Directory.Build.props")),
            @"<Version>([^<]+)</Version>");
        Assert.True(match.Success, "no <Version> element in Directory.Build.props");
        return match.Groups[1].Value;
    }

    [Fact]
    public void The_current_version_has_a_changelog_file()
    {
        if (Root() is not { } root) return;   // the wc3-mcp mirror has no Wc3.sln and no changelog
        string version = CurrentVersion(root);
        string file = Path.Combine(root, "changelog", version + ".md");
        Assert.True(File.Exists(file),
            $"version {version} has no changelog/{version}.md, so a release would ship without "
            + "its own notes file");
    }

    [Fact]
    public void The_root_changelog_shows_the_latest_version()
    {
        // CHANGELOG.md is the entry point, so it must carry a section for the version this build
        // is. If it drifts behind, the newest release is only visible one file deep.
        if (Root() is not { } root) return;
        string version = CurrentVersion(root);
        Assert.Contains($"## {version} - ", File.ReadAllText(Path.Combine(root, "CHANGELOG.md")));
    }
}
