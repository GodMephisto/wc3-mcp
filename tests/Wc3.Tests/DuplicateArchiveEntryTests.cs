// tests/Wc3.Tests/DuplicateArchiveEntryTests.cs
using System.Reflection;
using System.Text;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// An MPQ addresses a file by the hash of its name PLUS its locale, so one map can legitimately
/// hold several entries that all report the same FileName. The Chinese and Korean editors behind
/// most of the affected maps do exactly that.
///
/// MapDocument.AddOrReplaceRawFile resolved the name with GetFile, which is FirstOrDefault, so it
/// attached the override to whichever entry happened to load first and left the rest carrying the
/// original bytes. Save then wrote every entry, and the name resolved to one of the untouched
/// copies, so the edit was reported as applied and was not there when the map was read back.
///
/// Measured on Naruto Autobattle ENGv3ch11.w3x. The Reforged 3.0.0 repair reported 973 button
/// positions completed, saved, and a second pass over its own output found the same 973 still
/// malformed. Six files in that map are duplicated this way. Every edit command in the toolkit
/// goes through this method, 25 call sites across 12 files, so this was never specific to the
/// repair that exposed it.
/// </summary>
public class DuplicateArchiveEntryTests
{
    private const string Name = @"Units\CampaignUnitStrings.txt";
    private static readonly Encoding Text = Encoding.Latin1;

    /// <summary>Loads a one file map, then seeds a second entry under the same name to stand in
    /// for the locale twin a real packer writes. Reflection because Files is read-only by design,
    /// and widening it for a test would be a worse trade than reaching past it here.</summary>
    private static MapDocument WithDuplicate(byte[] payload, out MapFileEntry twin)
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [Name] = payload,
        }));

        var first = doc.GetFile(Name)!;
        twin = new MapFileEntry
        {
            FileName = first.FileName,
            BlockIndex = 99,
            RawBytes = payload,
            IsKnown = first.IsKnown,
        };

        var field = typeof(MapDocument).GetField("_files",
            BindingFlags.NonPublic | BindingFlags.Instance)!;
        ((List<MapFileEntry>)field.GetValue(doc)!).Add(twin);
        return doc;
    }

    [Fact]
    public void An_override_reaches_every_entry_sharing_the_name()
    {
        var doc = WithDuplicate(Text.GetBytes("Buttonpos=,2\r\n"), out var twin);
        Assert.Equal(2, doc.Files.Count(f => f.FileName == Name));

        doc.AddOrReplaceRawFile(Name, Text.GetBytes("Buttonpos=0,2\r\n"));

        // Whichever entry the archive's hash table resolves to has to carry the edit. Asserting
        // only on GetFile would pass on the broken code, because GetFile returns the one entry
        // that was always updated correctly.
        foreach (var entry in doc.Files.Where(f => f.FileName == Name))
            Assert.Equal("Buttonpos=0,2\r\n", Text.GetString(entry.OverrideBytes ?? entry.RawBytes));

        Assert.True(twin.IsDirty, "the twin has to be marked dirty or Save writes its stale bytes");
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void A_repair_converges_on_a_real_map_that_has_duplicate_entries()
    {
        // The end to end property, and the one that actually broke. It is a corpus test rather
        // than a hermetic one on purpose. A reflection-seeded twin does not survive SaveToBytes,
        // so the hermetic version of this passed against the broken code, which makes it a
        // comment rather than a test. Only a real archive carries a real duplicate.
        string path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Warcraft III", "Maps", "Download", "Naruto Autobattle ENGv3ch11.w3x");
        if (!File.Exists(path)) return;

        var doc = MapDocument.Load(path);
        Assert.Contains(doc.Files.GroupBy(f => f.FileName ?? "", StringComparer.OrdinalIgnoreCase),
            g => g.Count() > 1);

        var first = Reforged3RepairCommand.Execute(doc, apply: true);
        Assert.True(first.ButtonPositionsCompleted > 0, "this map is the one with malformed buttons");

        var second = Reforged3RepairCommand.Execute(MapDocument.Load(doc.SaveToBytes()), apply: false);
        Assert.Equal(0, second.IssueCount);
    }

    [Fact]
    public void A_map_without_duplicates_is_unaffected()
    {
        // The fix walks every matching entry instead of the first. On the ordinary one entry map
        // that has to behave exactly as it did, or the fix has traded one silent defect for another.
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [Name] = Text.GetBytes("Buttonpos=,2\r\n"),
        }));

        var entry = doc.AddOrReplaceRawFile(Name, Text.GetBytes("Buttonpos=0,2\r\n"));

        Assert.Single(doc.Files.Where(f => f.FileName == Name));
        Assert.Equal("Buttonpos=0,2\r\n", Text.GetString(entry.OverrideBytes!));
        Assert.True(entry.IsDirty);
    }

    [Fact]
    public void Adding_a_genuinely_new_file_still_creates_one_entry()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [Name] = Text.GetBytes("x"),
        }));

        doc.AddOrReplaceRawFile(@"Units\UnitSkin.txt", Text.GetBytes("[h001]\r\n"));

        Assert.Single(doc.Files.Where(f => f.FileName == @"Units\UnitSkin.txt"));
    }
}
