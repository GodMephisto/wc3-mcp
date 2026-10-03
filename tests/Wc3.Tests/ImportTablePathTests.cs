// tests/Wc3.Tests/ImportTablePathTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Matching a war3map.imp entry to the archive file behind it.
///
/// This was wrong twice, in opposite directions, in two implementations of the same thing.
///
/// LintCommand hand-rolled the .imp parse by scanning NUL-delimited runs, and the format is a flag
/// BYTE followed by a NUL-terminated path, so every path came out carrying a leading control
/// character and none ever resolved. On FgoRD_1.11 it reported 1,124 problems where the truth is
/// one, and the tell was that the number split as 562 "listed but absent" and the same 562 as
/// "present but unlisted", the same files counted from both directions.
///
/// ImportsCommand parsed the format properly through War3Net but matched only the literal spelling
/// and the war3mapImported-prefixed literal. A manifest routinely writes forward slashes where the
/// archive uses backslashes, so on GGGA_V0.04b it reported 2,113 the same way.
///
/// Each was correct exactly where the other was wrong, which is the argument for there being one.
/// </summary>
public class ImportTablePathTests
{
    private readonly ITestOutputHelper _out;
    public ImportTablePathTests(ITestOutputHelper output) => _out = output;

    // ---- the spelling rule, as a pure function ----

    [Fact]
    public void A_manifest_path_is_tried_with_both_separators_and_with_the_prefix()
    {
        var spellings = ImportsCommand.ArchiveSpellings("Archer/Emiya_death1.mp3").ToList();
        _out.WriteLine(string.Join("\n", spellings));

        Assert.Contains(@"Archer\Emiya_death1.mp3", spellings);
        Assert.Contains(@"war3mapImported\Archer\Emiya_death1.mp3", spellings);
        Assert.Contains("Archer/Emiya_death1.mp3", spellings);          // the literal, still first
        Assert.Equal("Archer/Emiya_death1.mp3", spellings[0]);
    }

    [Fact]
    public void A_path_that_needs_no_rewriting_yields_no_duplicates()
    {
        var spellings = ImportsCommand.ArchiveSpellings(@"war3mapImported\foo.blp").ToList();
        Assert.Equal(spellings.Distinct(StringComparer.OrdinalIgnoreCase).Count(), spellings.Count);
        Assert.Contains(@"war3mapImported\foo.blp", spellings);
    }

    // ---- the parse, against a real .imp laid out by hand ----

    /// <summary>
    /// A war3map.imp: uint32 version, uint32 count, then per entry a flag byte and a
    /// NUL-terminated path. The flag byte is the part the hand-rolled scan swallowed.
    /// </summary>
    private static byte[] ImpTable(params (byte Flag, string Path)[] entries)
    {
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms, Encoding.Latin1, leaveOpen: true);
        w.Write(1);                 // version
        w.Write(entries.Length);    // count
        foreach (var (flag, path) in entries)
        {
            w.Write(flag);
            w.Write(Encoding.Latin1.GetBytes(path));
            w.Write((byte)0);
        }
        w.Flush();
        return ms.ToArray();
    }

    private static MapDocument MapWithImports(
        (byte Flag, string Path)[] table, params string[] archiveNames)
    {
        var files = new Dictionary<string, byte[]>
        {
            ["war3map.w3i"] = BlankMap.Create().GetFile("war3map.w3i")!.CurrentBytes,
            ["war3map.w3e"] = BlankMap.Create().GetFile("war3map.w3e")!.CurrentBytes,
            ["war3map.j"] = Encoding.UTF8.GetBytes("function main takes nothing returns nothing\nendfunction\n"),
            ["war3map.imp"] = ImpTable(table),
        };
        foreach (var n in archiveNames) files[n] = new byte[] { 1, 2, 3, 4 };
        return MapDocument.Load(SyntheticMap.Build(files));
    }

    [Fact]
    public void The_flag_byte_is_not_part_of_the_path()
    {
        // Flag 8 is 0x08, a backspace. Reading it into the path is what made every entry unresolvable.
        var doc = MapWithImports(
            new[] { ((byte)8, "!0001_Caster.mdx") },
            @"war3mapImported\!0001_Caster.mdx");

        var listing = ImportsCommand.Execute(doc);
        var entry = listing.Entries.Single(e => e.InManifest);
        _out.WriteLine($"path '{entry.Path}', inArchive={entry.InArchive}");

        Assert.Equal("!0001_Caster.mdx", entry.Path);
        Assert.DoesNotContain('\b', entry.Path);
        Assert.True(entry.InArchive, "the archive holds it under the war3mapImported prefix");
    }

    [Fact]
    public void A_forward_slash_manifest_path_matches_a_backslash_archive_name()
    {
        var doc = MapWithImports(
            new[] { ((byte)8, "Archer/Emiya_death1.mp3") },
            @"war3mapImported\Archer\Emiya_death1.mp3");

        var entry = ImportsCommand.Execute(doc).Entries.Single(e => e.InManifest);
        Assert.True(entry.InArchive, "forward slashes in the manifest must still match");
    }

    [Fact]
    public void A_file_the_archive_really_lacks_is_still_reported()
    {
        // The fix must not resolve everything. This is the finding the noise was burying.
        var doc = MapWithImports(new[] { ((byte)8, "gone.mdx") });

        var entry = ImportsCommand.Execute(doc).Entries.Single(e => e.InManifest);
        Assert.False(entry.InArchive);
    }

    [Fact]
    public void The_matched_archive_file_is_not_also_reported_as_unlisted()
    {
        // The double-count that made 562 look like 1,124.
        var doc = MapWithImports(
            new[] { ((byte)8, "Archer/Emiya_death1.mp3") },
            @"war3mapImported\Archer\Emiya_death1.mp3");

        var entries = ImportsCommand.Execute(doc).Entries;
        Assert.DoesNotContain(entries, e => !e.InManifest && e.InArchive);
    }

    [Fact]
    public void Lint_reports_a_clean_import_table_as_ok()
    {
        var doc = MapWithImports(
            new[] { ((byte)8, "Archer/Emiya_death1.mp3"), ((byte)8, "!0001_Caster.mdx") },
            @"war3mapImported\Archer\Emiya_death1.mp3",
            @"war3mapImported\!0001_Caster.mdx");

        var check = LintCommand.Run(doc).Checks
            .Single(c => c.Name == "import-table");
        _out.WriteLine($"{check.Severity} {check.Summary}");
        Assert.Equal(LintSeverity.Ok, check.Severity);
    }

    /// <summary>
    /// A stale table entry is a warning, not an error, and the reasoning is evidence-based rather
    /// than optimistic. Across 34 real maps, 13 have one, the largest count is 2, and every one of
    /// those maps is playable. The runtime consequence, if any, is what asset-references reports.
    /// </summary>
    [Fact]
    public void A_stale_entry_warns_rather_than_failing_the_lint()
    {
        var doc = MapWithImports(new[] { ((byte)8, "gone.mdx") });

        var check = LintCommand.Run(doc).Checks.Single(c => c.Name == "import-table");
        _out.WriteLine($"{check.Severity} {check.Summary}");
        Assert.Equal(LintSeverity.Warning, check.Severity);
        Assert.Contains("still runs", check.Summary);
    }
}
