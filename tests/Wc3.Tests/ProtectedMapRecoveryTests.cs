// tests/Wc3.Tests/ProtectedMapRecoveryTests.cs
// Covers MapDocument.Load probing an archive's hash table for standard WC3 file names when
// its (listfile) is stripped or curated to hide most files, the shape a protected real-world
// map presents (see docs/... investigation, 25 of the user's custom maps load with 0 to a
// handful of named entries and nothing else readable, even though the hash table still
// resolves every standard name).
using Wc3.Model;

namespace Wc3.Tests;

public class ProtectedMapRecoveryTests
{
    [Fact]
    public void Recovers_standard_files_hidden_from_a_stripped_listfile()
    {
        var bytes = SyntheticMap.BuildProtected(
            visibleFiles: new Dictionary<string, byte[]> { ["(listfile)"] = new byte[] { 0 } },
            hiddenFiles: new Dictionary<string, byte[]>
            {
                ["war3map.j"] = System.Text.Encoding.UTF8.GetBytes("globals\r\nendglobals"),
                ["war3map.w3i"] = new byte[] { 1, 2, 3, 4 },
                ["mystery.bin"] = new byte[] { 9, 9, 9 }, // author asset, not a standard name, stays unnamed
            });

        var doc = MapDocument.Load(bytes);

        var script = doc.GetFile("war3map.j");
        Assert.NotNull(script);
        Assert.True(script!.IsKnown);
        Assert.Equal("globals\r\nendglobals", script.Model);

        var info = doc.GetFile("war3map.w3i");
        Assert.NotNull(info);
        Assert.True(info!.IsKnown);
        Assert.Equal(new byte[] { 1, 2, 3, 4 }, info.RawBytes);

        // A name with no place on the standard list is not, and should not be, recoverable
        // this way, it stays exactly as unnamed as it was before probing.
        Assert.DoesNotContain(doc.Files, f => f.FileName == "mystery.bin");
        Assert.Contains(doc.Files, f => f.FileName is null && f.RawBytes.SequenceEqual(new byte[] { 9, 9, 9 }));
    }

    [Fact]
    public void Recovers_the_script_variant_some_compilers_use()
    {
        var bytes = SyntheticMap.BuildProtected(
            visibleFiles: new Dictionary<string, byte[]> { ["(listfile)"] = new byte[] { 0 } },
            hiddenFiles: new Dictionary<string, byte[]>
            {
                ["scripts\\war3map.j"] = System.Text.Encoding.UTF8.GetBytes("globals\r\nendglobals"),
            });

        var doc = MapDocument.Load(bytes);

        var entry = doc.GetFile("scripts\\war3map.j");
        Assert.NotNull(entry);
        Assert.Equal("globals\r\nendglobals", System.Text.Encoding.UTF8.GetString(entry!.RawBytes));
    }

    [Fact]
    public void Healthy_listfile_is_unaffected_by_probing()
    {
        // A map whose (listfile) already names everything must load identically whether or
        // not the probe runs, AddFileNames only sets FileName on entries that are still
        // unnamed, this is the single invariant every already-working map depends on.
        var bytes = SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3i"] = new byte[] { 1, 2, 3, 4 },
            ["war3map.j"] = new byte[] { 5, 6 },
            ["mystery.bin"] = new byte[] { 7, 8, 9 },
        });

        var doc = MapDocument.Load(bytes);

        // Not asserting doc.Files.Count, MpqArchiveBuilder adds its own bookkeeping entries
        // ((listfile)/(attributes)) when none is supplied, unrelated to this feature. What
        // matters is that every name the map's own listfile already provided survives
        // probing completely unchanged.
        Assert.Equal(new byte[] { 1, 2, 3, 4 }, doc.GetFile("war3map.w3i")!.RawBytes);
        Assert.Equal(new byte[] { 5, 6 }, doc.GetFile("war3map.j")!.RawBytes);
        Assert.Equal(new byte[] { 7, 8, 9 }, doc.GetFile("mystery.bin")!.RawBytes);
        Assert.True(doc.GetFile("war3map.w3i")!.IsKnown);
        Assert.False(doc.GetFile("mystery.bin")!.IsKnown);
    }

    [Fact]
    public void Probing_a_recovered_name_does_not_mark_it_dirty_or_change_saved_bytes()
    {
        // Naming an entry is metadata only, it must not cause SaveToBytes to re-serialize a
        // file that was never edited, byte fidelity for untouched files is load bearing.
        var bytes = SyntheticMap.BuildProtected(
            visibleFiles: new Dictionary<string, byte[]> { ["(listfile)"] = new byte[] { 0 } },
            hiddenFiles: new Dictionary<string, byte[]>
            {
                ["war3map.w3i"] = new byte[] { 1, 2, 3, 4 },
            });

        var doc = MapDocument.Load(bytes);
        var entry = doc.GetFile("war3map.w3i");
        Assert.NotNull(entry);
        Assert.False(entry!.IsDirty);

        var saved = doc.SaveToBytes();
        var reloaded = MapDocument.Load(saved);
        Assert.Equal(new byte[] { 1, 2, 3, 4 }, reloaded.GetFile("war3map.w3i")!.RawBytes);
    }
}
