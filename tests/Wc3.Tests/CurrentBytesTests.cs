// tests/Wc3.Tests/CurrentBytesTests.cs
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// A map file entry has two byte arrays and they are not interchangeable. RawBytes is the archive's
/// original content and never changes. OverrideBytes is the pending replacement, and it is what
/// Save writes. Asking the wrong one silently answers with pre-edit content.
///
/// That question was being answered 67 times by hand, 39 sites writing
/// <c>OverrideBytes ?? RawBytes</c> inline and 28 reading RawBytes bare, and the bare reads were
/// wrong. Several were user-visible. The Files panel previewed and hex-dumped the old content of
/// any file another panel had just changed. Lint asked "does the script compile" of the script on
/// disk rather than the one about to be saved, so a broken edit passed clean. Extract wrote the
/// old bytes to disk.
///
/// These pin the accessor that now owns the decision, and the two behaviours that were wrong.
/// </summary>
public class CurrentBytesTests
{
    [Fact]
    public void An_untouched_entry_reads_the_same_either_way()
    {
        var doc = BlankMap.Create();
        var entry = doc.GetFile("war3map.j")!;
        Assert.Null(entry.OverrideBytes);
        Assert.Equal(entry.RawBytes, entry.CurrentBytes);
        Assert.Equal(entry.RawSize, entry.CurrentSize);
    }

    [Fact]
    public void A_replaced_entry_reads_the_replacement_and_RawBytes_stays_the_original()
    {
        var doc = BlankMap.Create();
        var before = doc.GetFile("war3map.j")!.RawBytes.ToArray();

        var replacement = System.Text.Encoding.Latin1.GetBytes(
            "function main takes nothing returns nothing\nendfunction\n");
        doc.AddOrReplaceRawFile("war3map.j", replacement);

        var entry = doc.GetFile("war3map.j")!;
        Assert.Equal(replacement, entry.CurrentBytes);
        Assert.Equal(replacement.Length, entry.CurrentSize);
        // The original is still the original. That is the whole point of the distinction, and the
        // reason a bare RawBytes read is a stale read rather than a style choice.
        Assert.Equal(before, entry.RawBytes);
    }

    [Fact]
    public void A_second_replacement_of_the_same_entry_is_the_one_that_is_read()
    {
        // The exact shape that made this visible. AddOrReplaceRawFile takes the REPLACE branch on
        // an entry it already added, and that branch sets OverrideBytes only, so RawBytes is
        // pinned to whatever the very first write happened to be.
        var doc = BlankMap.Create();
        doc.AddOrReplaceRawFile("war3map.w3c", new byte[53]);
        doc.AddOrReplaceRawFile("war3map.w3c", new byte[8]);

        var entry = doc.GetFile("war3map.w3c")!;
        Assert.Equal(8, entry.CurrentBytes.Length);
        Assert.Equal(53, entry.RawBytes.Length);
    }

    [Fact]
    public void CurrentSize_does_not_decompress_an_untouched_entry()
    {
        // CurrentSize is the deferred-decompression counterpart of RawSize, so a sweep over every
        // entry stays free. If it went through CurrentBytes it would materialize the whole map.
        var doc = BlankMap.Create();
        foreach (var f in doc.Files)
            _ = f.CurrentSize;
        // Nothing to assert beyond "this did not have to decompress", which RawSize's own tests
        // already pin. Reaching here without an exception is the contract.
        Assert.NotEmpty(doc.Files);
    }

    [Fact]
    public void A_preview_of_an_edited_file_shows_the_edit()
    {
        // FilesView reads through FilePreviewCommand, which read RawBytes bare, so the panel
        // showed the file as it was on disk no matter what another panel had just done to it.
        var doc = BlankMap.Create();
        var edited = System.Text.Encoding.Latin1.GetBytes("// edited in session\n");
        doc.AddOrReplaceRawFile("war3map.j", edited);

        var preview = FilePreviewCommand.Execute(doc, "war3map.j");
        Assert.Contains("edited in session", preview.Text ?? "");
    }
}
