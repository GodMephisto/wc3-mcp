// tests/Wc3.Tests/ImportsClassificationTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Two defects in how an archive entry is classified as a user import, both measured on maps
/// Blizzard ships inside build 3.0.0.24268 rather than on fixtures.
///
/// The engine files the 3.0.0 editor writes are neither registered nor on the exclusion list, so
/// a map's own lighting, its conversation data and its per-locale string tables are reported as
/// things the author imported. On (2)EchoIsles.w3x and FaceFXCinematicsTest.w3x, which have no
/// war3mapImported folder at all, that produced 14 and 15 phantom import rows.
///
/// Separately, war3map.imp writes its paths with forward slashes while the archive stores
/// backslashes, and the two were compared literally. The same file then appeared twice, once as
/// manifest-only with no size and once as an unlisted archive orphan, so a caller reading the
/// list saw a missing import and a stray file where there was one correctly imported file.
/// </summary>
public class ImportsClassificationTests
{
    [Theory]
    [InlineData(@"war3map.w3l")]                    // lights, new in 3.0.0
    [InlineData(@"war3mapPostProcessing.txt")]      // post-processing, new in 3.0.0
    [InlineData(@"war3map.w3grp")]
    [InlineData(@"war3map.soundasset")]
    [InlineData(@"conversation.json")]
    [InlineData(@"_Locales\deDE.w3mod\war3map.wts")]
    [InlineData(@"_Locales\zhCN.w3mod\war3map.wts")]
    public void Engine_files_are_not_user_imports(string name)
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [name] = Encoding.Latin1.GetBytes("payload"),
        }));

        Assert.Empty(ImportsCommand.Execute(doc).Entries);
    }

    [Fact]
    public void A_real_import_is_still_reported()
    {
        // The fix must not swallow actual imports, which is the failure mode that would make
        // the list useless in the other direction.
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [@"war3mapImported\custom.blp"] = Encoding.Latin1.GetBytes("blp"),
            [@"war3map.w3l"] = Encoding.Latin1.GetBytes("lights"),
        }));

        var entries = ImportsCommand.Execute(doc).Entries;
        Assert.Single(entries);
        Assert.Equal(@"war3mapImported\custom.blp", entries[0].Path);
    }

    [Fact]
    public void A_manifest_path_matches_its_archive_entry_across_slash_styles()
    {
        // war3map.imp says forward, the archive says back. One file, one row, present in both.
        var archive = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            [@"_Locales\deDE.w3mod\war3map.wts"] = 503,
        };

        var entries = ImportsCommand.BuildEntries(
            new[] { "_Locales/deDE.w3mod/war3map.wts" }, archive);

        var row = Assert.Single(entries);
        Assert.True(row.InManifest, "the manifest lists it");
        Assert.True(row.InArchive, "and the archive holds it, under the other slash");
        Assert.Equal(503, row.SizeBytes);
    }

    [Fact]
    public void A_manifest_path_that_is_genuinely_absent_still_reports_absent()
    {
        // Normalising slashes must not turn a real missing-file finding into a false match.
        var entries = ImportsCommand.BuildEntries(
            new[] { "war3mapImported/gone.blp" },
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase));

        var row = Assert.Single(entries);
        Assert.True(row.InManifest);
        Assert.False(row.InArchive);
        Assert.Null(row.SizeBytes);
    }
}
