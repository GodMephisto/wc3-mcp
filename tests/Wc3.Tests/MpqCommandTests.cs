// tests/Wc3.Tests/MpqCommandTests.cs
// Hermetic coverage for the mpq container commands, built on SyntheticMap so no real
// map and no Warcraft III install is needed. Each test pins one behaviour that the
// command would not provide if it were missing or wrong.
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public class MpqCommandTests
{
    private static readonly Dictionary<string, byte[]> Basic = new()
    {
        ["war3map.j"] = new byte[] { 1, 2, 3, 4 },
        ["mystery.bin"] = new byte[] { 7, 8, 9 },
    };

    private static string Write(byte[] bytes, string ext = ".w3x")
    {
        var path = Path.Combine(Path.GetTempPath(), $"wc3mpq-{Guid.NewGuid():N}{ext}");
        File.WriteAllBytes(path, bytes);
        return path;
    }

    private static string WriteText(string text, string ext = ".txt")
    {
        var path = Path.Combine(Path.GetTempPath(), $"wc3mpq-{Guid.NewGuid():N}{ext}");
        File.WriteAllText(path, text);
        return path;
    }

    [Fact]
    public void Verify_reports_a_healthy_archive_as_fully_readable()
    {
        var path = Write(SyntheticMap.Build(Basic));
        try
        {
            var r = MpqVerifyCommand.Execute(path);
            // war3map.j, mystery.bin and the generated (listfile).
            Assert.True(r.TotalEntries >= 3, $"expected at least 3 entries, saw {r.TotalEntries}");
            Assert.Equal(0, r.UnreadableEntries);
            Assert.Empty(r.Unreadable);
            Assert.Equal(r.TotalEntries, r.ReadableEntries + r.UnreadableEntries);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void Listfile_splits_listfile_names_probe_recoveries_and_unnamed()
    {
        // Two hidden files, one carrying a standard name the probe should recover and the other a
        // name nothing can guess, so it must stay unnamed.
        var path = Write(SyntheticMap.BuildProtected(
            new Dictionary<string, byte[]> { ["war3map.j"] = new byte[] { 1 } },
            new Dictionary<string, byte[]>
            {
                ["war3map.w3e"] = new byte[] { 2 },
                ["custom_asset.bin"] = new byte[] { 3 },
            }));
        try
        {
            var r = MpqListfileCommand.Execute(path);
            Assert.True(r.NamedByListfile >= 1, "war3map.j is named by the listfile");
            Assert.Equal(1, r.RecoveredByStandardProbe);
            Assert.Equal(1, r.WithoutName);
            Assert.Equal(r.TotalEntries, r.NamedByListfile + r.RecoveredByStandardProbe + r.WithoutName);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void Name_recover_reports_a_dictionary_hit_on_an_unnamed_block()
    {
        var path = Write(SyntheticMap.BuildProtected(
            new Dictionary<string, byte[]> { ["war3map.j"] = new byte[] { 1 } },
            new Dictionary<string, byte[]> { ["hidden.w3e"] = new byte[] { 2 } }));
        // A dictionary naming the hidden file and an already-named one. Only the hidden file
        // may be reported, because RecoverNames skips blocks that already carry a name.
        var dict = WriteText("hidden.w3e\nwar3map.j\n\n");
        try
        {
            var doc = MapDocument.Load(path);
            var r = MpqNameRecoverCommand.Execute(doc, dict);
            var recovered = Assert.Single(r.Names);
            Assert.Equal("hidden.w3e", recovered.Name);
            Assert.True(recovered.SizeBytes > 0);
            Assert.True(r.UnnamedBefore >= 1);
            Assert.DoesNotContain(r.Names, n => n.Name == "war3map.j");
        }
        finally { File.Delete(path); File.Delete(dict); }
    }

    [Fact]
    public void Strip_bookkeeping_drops_listfile_reopens_and_leaves_the_input_untouched()
    {
        var path = Write(SyntheticMap.Build(Basic));
        var outPath = Path.Combine(Path.GetDirectoryName(path)!,
            Path.GetFileNameWithoutExtension(path) + ".edited.w3x");
        var inputBefore = File.ReadAllBytes(path);
        try
        {
            Assert.True(MapDocument.Load(path).HasFileByName("(listfile)"),
                "the fixture must carry a (listfile) for the drop to be observable");

            var r = MpqStripBookkeepingCommand.Execute(path, outPath);

            Assert.Equal(outPath, r.SavedTo);
            Assert.True(r.Reopens, r.ReopenError);
            Assert.False(r.ListfilePresent, "the listfile should be gone from the output");
            Assert.False(r.AttributesPresent);
            Assert.True(File.Exists(outPath));
            Assert.False(MapDocument.Load(outPath).HasFileByName("(listfile)"));
            Assert.Equal(inputBefore, File.ReadAllBytes(path));
        }
        finally { File.Delete(path); if (File.Exists(outPath)) File.Delete(outPath); }
    }

    [Fact]
    public void Strip_bookkeeping_defaults_to_an_edited_copy_and_refuses_in_place()
    {
        var path = Write(SyntheticMap.Build(Basic));
        var expected = Path.Combine(Path.GetDirectoryName(path)!,
            Path.GetFileNameWithoutExtension(path) + ".edited.w3x");
        try
        {
            var r = MpqStripBookkeepingCommand.Execute(path, null);
            Assert.Equal(expected, r.SavedTo);

            var same = Assert.Throws<InvalidOperationException>(
                () => MpqStripBookkeepingCommand.Execute(path, path));
            Assert.Contains("never overwritten", same.Message);
        }
        finally { File.Delete(path); if (File.Exists(expected)) File.Delete(expected); }
    }

    [Fact]
    public void Bytes_diff_separates_identical_changed_and_one_sided_entries()
    {
        var a = Write(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = new byte[] { 1, 2, 3 },
            ["keep.bin"] = new byte[] { 7, 7 },
            ["onlyA.bin"] = new byte[] { 1 },
        }));
        var b = Write(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = new byte[] { 1, 2, 3 },
            ["keep.bin"] = new byte[] { 7, 8 },
            ["onlyB.bin"] = new byte[] { 2 },
        }));
        try
        {
            var d = MpqBytesDiffCommand.Execute(a, b);
            Assert.False(d.Identical);
            Assert.Contains(d.Differences, x => x.Name == "keep.bin");
            Assert.Contains("onlyA.bin", d.OnlyInA);
            Assert.Contains("onlyB.bin", d.OnlyInB);
            Assert.DoesNotContain(d.Differences, x => x.Name == "war3map.j");

            var same = MpqBytesDiffCommand.Execute(a, a);
            Assert.True(same.Identical);
            Assert.Equal(0, same.Differ);
            Assert.Empty(same.OnlyInA);
            Assert.Empty(same.OnlyInB);
        }
        finally { File.Delete(a); File.Delete(b); }
    }
}
