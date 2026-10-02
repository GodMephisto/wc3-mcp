using Wc3.Commands;

namespace Wc3.Tests;

/// <summary>
/// Pure-logic tests for ImportsCommand.BuildEntries — the manifest/archive merge behind
/// the read-only imports listing (Execute only adds MapDocument plumbing on top).
/// </summary>
public class ImportsCommandTests
{
    private static Dictionary<string, int> Archive(params (string Name, int Size)[] files)
    {
        var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var (name, size) in files) dict[name] = size;
        return dict;
    }

    [Fact]
    public void ManifestPathPresentInArchive_MergedWithSize()
    {
        var entries = ImportsCommand.BuildEntries(
            new[] { @"war3mapImported\a.blp" },
            Archive((@"war3mapImported\a.blp", 100)));

        var e = Assert.Single(entries);
        Assert.Equal(@"war3mapImported\a.blp", e.Path);
        Assert.True(e.InManifest);
        Assert.True(e.InArchive);
        Assert.Equal(100, e.SizeBytes);
    }

    [Fact]
    public void ManifestOnly_MarkedMissingFromArchive()
    {
        var entries = ImportsCommand.BuildEntries(new[] { @"missing.mdx" }, Archive());

        var e = Assert.Single(entries);
        Assert.True(e.InManifest);
        Assert.False(e.InArchive);
        Assert.Null(e.SizeBytes);
    }

    [Fact]
    public void ArchiveOrphan_ListedAsNotInManifest()
    {
        var entries = ImportsCommand.BuildEntries(
            Array.Empty<string>(), Archive((@"war3mapImported\orphan.wav", 42)));

        var e = Assert.Single(entries);
        Assert.False(e.InManifest);
        Assert.True(e.InArchive);
        Assert.Equal(42, e.SizeBytes);
    }

    [Fact]
    public void ManifestPathWithoutDefaultPrefix_MatchesPrefixedArchiveFile_NoOrphanRow()
    {
        // WorldEdit stores non-custom-path imports WITHOUT the folder in the manifest,
        // but the archive file lives under war3mapImported\.
        var entries = ImportsCommand.BuildEntries(
            new[] { "a.blp" },
            Archive((@"war3mapImported\a.blp", 7)));

        var e = Assert.Single(entries);   // the archive file must be claimed, not doubled
        Assert.Equal("a.blp", e.Path);
        Assert.True(e.InManifest);
        Assert.True(e.InArchive);
        Assert.Equal(7, e.SizeBytes);
    }

    [Fact]
    public void Match_IsCaseInsensitive()
    {
        var entries = ImportsCommand.BuildEntries(
            new[] { @"WAR3MAPIMPORTED\A.BLP" },
            Archive((@"war3mapimported\a.blp", 9)));

        var e = Assert.Single(entries);
        Assert.True(e.InArchive);
        Assert.Equal(9, e.SizeBytes);
    }

    [Fact]
    public void DuplicateManifestPaths_CollapsedToOneRow()
    {
        var entries = ImportsCommand.BuildEntries(
            new[] { "a.blp", "A.BLP" }, Archive());
        Assert.Single(entries);
    }

    [Fact]
    public void Result_SortedByPath()
    {
        var entries = ImportsCommand.BuildEntries(
            new[] { "b.mdx" },
            Archive(("c.wav", 1), ("a.blp", 2)));

        Assert.Equal(new[] { "a.blp", "b.mdx", "c.wav" }, entries.Select(e => e.Path));
    }
}
