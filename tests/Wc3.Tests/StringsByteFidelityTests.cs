// tests/Wc3.Tests/StringsByteFidelityTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// StringsCommand claims to edit war3map.wts byte-faithfully, splicing only the target entry and
/// leaving every other character untouched. That claim was true at the CHARACTER level and false
/// at the byte level, because the splice decoded the whole file as UTF-8 and re-encoded it, which
/// is byte-identical only while every byte in the file is valid UTF-8.
///
/// Measured across the map library, three maps are not: all three GGGA versions carry one
/// truncated multi-byte sequence each. On those, editing any one string rewrote that unrelated
/// byte into the three bytes of U+FFFD, permanently, and nothing anywhere would have reported it.
/// </summary>
public class StringsByteFidelityTests
{
    private readonly ITestOutputHelper _out;
    public StringsByteFidelityTests(ITestOutputHelper output) => _out = output;

    /// <summary>A wts holding one byte that is not valid UTF-8, which is what the real maps have:
    /// a lead byte with no continuation.</summary>
    private static MapDocument MapWithABadByte()
    {
        var head = Encoding.UTF8.GetBytes(
            "﻿STRING 1\r\n{\r\nplain ascii\r\n}\r\n\r\nSTRING 2\r\n{\r\n");
        var tail = Encoding.UTF8.GetBytes("\r\n}\r\n");
        // 0xE3 starts a three-byte sequence; followed by ')' it can never be completed.
        var broken = new byte[] { 0xE3, (byte)')' };

        var bytes = head.Concat(broken).Concat(tail).ToArray();
        var doc = BlankMap.Create();
        doc.AddOrReplaceRawFile("war3map.wts", bytes);
        return MapDocument.Load(doc.SaveToBytes());
    }

    [Fact]
    public void Editing_one_string_leaves_every_other_byte_of_the_file_alone()
    {
        var doc = MapWithABadByte();
        byte[] before = doc.GetFile("war3map.wts")!.CurrentBytes.ToArray();

        StringsCommand.Set(doc, 1, "replaced");
        byte[] after = doc.GetFile("war3map.wts")!.CurrentBytes;

        _out.WriteLine($"{before.Length} -> {after.Length} bytes");

        // The undecodable byte is in STRING 2, which was not edited, so it must be present
        // unchanged. A UTF-8 round trip turns it into EF BF BD.
        Assert.Contains((byte)0xE3, after);
        Assert.False(ContainsSequence(after, new byte[] { 0xEF, 0xBF, 0xBD }),
            "the untouched byte was rewritten as the UTF-8 replacement character");

        // And the edit itself landed.
        var listed = StringsCommand.List(doc).Entries;
        Assert.Equal("replaced", listed.Single(e => e.Id == 1).Text.Trim());
    }

    [Fact]
    public void A_no_op_style_edit_does_not_change_the_files_length_elsewhere()
    {
        var doc = MapWithABadByte();
        byte[] before = doc.GetFile("war3map.wts")!.CurrentBytes.ToArray();

        StringsCommand.Set(doc, 1, "plain ascii");   // same text it already had
        byte[] after = doc.GetFile("war3map.wts")!.CurrentBytes;

        Assert.Equal(before.Length, after.Length);
        Assert.Equal(before, after);
    }

    [Fact]
    public void New_text_is_still_written_as_real_utf8()
    {
        // The carrier is Latin-1, so the obvious way to get this wrong is to write the caller's
        // text as Latin-1 too and mangle every non-ASCII character they supply.
        var doc = MapWithABadByte();
        StringsCommand.Set(doc, 1, "한국어 test");

        byte[] after = doc.GetFile("war3map.wts")!.CurrentBytes;
        Assert.Contains(Encoding.UTF8.GetBytes("한국어"), b => true);
        Assert.True(ContainsSequence(after, Encoding.UTF8.GetBytes("한국어")),
            "the replacement text was not written as UTF-8");

        Assert.Equal("한국어 test",
            StringsCommand.List(doc).Entries.Single(e => e.Id == 1).Text.Trim());
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Editing_a_real_map_with_a_bad_byte_preserves_it()
    {
        string path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Warcraft III", "Maps", "Download", "GGGA_V0.04g.w3x");
        if (!File.Exists(path)) { _out.WriteLine("map absent, skipped"); return; }

        var doc = MapDocument.Load(path);
        byte[] before = doc.GetFile("war3map.wts")!.CurrentBytes.ToArray();
        int replacementsBefore = CountSequence(before, new byte[] { 0xEF, 0xBF, 0xBD });

        var first = StringsCommand.List(doc).Entries.FirstOrDefault();
        if (first is null) { _out.WriteLine("no strings, skipped"); return; }

        StringsCommand.Set(doc, first.Id, first.Text);   // rewrite one entry with its own text
        byte[] after = doc.GetFile("war3map.wts")!.CurrentBytes;
        int replacementsAfter = CountSequence(after, new byte[] { 0xEF, 0xBF, 0xBD });

        _out.WriteLine($"{before.Length:N0} -> {after.Length:N0} bytes, "
                     + $"U+FFFD sequences {replacementsBefore} -> {replacementsAfter}");
        Assert.Equal(replacementsBefore, replacementsAfter);
    }

    private static bool ContainsSequence(byte[] haystack, byte[] needle) =>
        CountSequence(haystack, needle) > 0;

    private static int CountSequence(byte[] haystack, byte[] needle)
    {
        int n = 0;
        for (int i = 0; i + needle.Length <= haystack.Length; i++)
        {
            bool hit = true;
            for (int j = 0; j < needle.Length; j++)
                if (haystack[i + j] != needle[j]) { hit = false; break; }
            if (hit) n++;
        }
        return n;
    }
}
