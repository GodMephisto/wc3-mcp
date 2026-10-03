// tests/Wc3.Tests/ScriptByteFidelityTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Whether reading a map script and writing it straight back changes its bytes.
///
/// Three layers disagree about the encoding of war3map.j. Parsers.cs registers it as Latin-1, and
/// says why in a comment: a map script is a byte stream with no declared encoding, real maps carry
/// bytes that are not valid UTF-8, and one map in this library carries about 52,000 of them.
/// HeroWiringAudit reads it as Latin-1 too. ScriptCommand.Read decodes it as UTF-8 and
/// ScriptCommand.Write re-encodes as UTF-8, and that pair is the path the Studio's Script panel
/// uses to load and to save.
///
/// A UTF-8 decode of an invalid sequence yields U+FFFD, and re-encoding U+FFFD writes back the
/// three bytes EF BF BD. The round trip then looks clean forever while the original byte is gone.
/// That is the identical failure already found and fixed in StringsCommand for war3map.wts, where
/// editing one string rewrote an unrelated byte in another.
///
/// So this asserts the behaviour rather than arguing from the call sites, which is the only way
/// this kind of claim should be settled.
/// </summary>
public class ScriptByteFidelityTests
{
    private readonly ITestOutputHelper _out;
    public ScriptByteFidelityTests(ITestOutputHelper output) => _out = output;

    /// <summary>A script carrying bytes that are not valid UTF-8. 0xE3 starts a three-byte
    /// sequence and is followed here by a quote, which can never complete it. Real maps get here
    /// through Korean, Russian and Chinese text written in a legacy code page.</summary>
    private static byte[] ScriptWithHighBytes()
    {
        var head = Encoding.Latin1.GetBytes(
            "function main takes nothing returns nothing\n    call BJDebugMsg(\"");
        var high = new byte[] { 0xE3, 0x29, 0xB5, 0xF1, 0x80 };
        var tail = Encoding.Latin1.GetBytes("\")\nendfunction\n");
        return head.Concat(high).Concat(tail).ToArray();
    }

    private static MapDocument MapWithScript(byte[] script) =>
        MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = script,
        }));

    [Fact]
    public void Reading_a_script_and_writing_it_back_unchanged_preserves_every_byte()
    {
        byte[] original = ScriptWithHighBytes();
        var doc = MapWithScript(original);

        var (_, text) = ScriptCommand.Read(doc);
        ScriptCommand.Write(doc, text);

        byte[] after = MapDocument.Load(doc.SaveToBytes()).GetFile("war3map.j")!.CurrentBytes;

        _out.WriteLine($"{original.Length} bytes in, {after.Length} bytes out");
        _out.WriteLine("in  " + string.Join(" ", original.Select(b => b.ToString("X2"))));
        _out.WriteLine("out " + string.Join(" ", after.Select(b => b.ToString("X2"))));

        Assert.Equal(original, after);
    }

    [Fact]
    public void A_read_does_not_silently_replace_bytes_it_cannot_decode()
    {
        var doc = MapWithScript(ScriptWithHighBytes());
        var (_, text) = ScriptCommand.Read(doc);

        int replacements = text.Count(c => c == '�');
        _out.WriteLine($"{replacements} replacement character(s) in the decoded script");
        Assert.Equal(0, replacements);
    }

    [Fact]
    public void An_actual_edit_changes_only_what_was_edited()
    {
        // The realistic case. A user opens the Script panel on a map with legacy text, changes one
        // line, and saves. Everything they did not touch must survive.
        byte[] original = ScriptWithHighBytes();
        var doc = MapWithScript(original);

        var (_, text) = ScriptCommand.Read(doc);
        ScriptCommand.Write(doc, text.Replace("main", "renamed", StringComparison.Ordinal));

        byte[] after = MapDocument.Load(doc.SaveToBytes()).GetFile("war3map.j")!.CurrentBytes;

        // The high bytes are outside the edit and must be present, unchanged and un-replaced.
        Assert.Contains((byte)0xE3, after);
        Assert.Contains((byte)0xB5, after);
        Assert.Contains((byte)0xF1, after);
        Assert.False(ContainsSequence(after, new byte[] { 0xEF, 0xBF, 0xBD }),
            "an untouched byte was rewritten as the UTF-8 replacement character");
        Assert.Contains("renamed", Encoding.Latin1.GetString(after), StringComparison.Ordinal);
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void A_real_map_full_of_legacy_text_round_trips()
    {
        string dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Warcraft III", "Maps", "Download");
        string? path = Directory.Exists(dir)
            ? Directory.EnumerateFiles(dir, "Anime_WOS2_0.28a*.w3x").FirstOrDefault()
            : null;
        if (path is null) { _out.WriteLine("no suitable map, skipped"); return; }

        var doc = MapDocument.Load(path);
        var entry = doc.GetFile("war3map.j");
        if (entry is null) { _out.WriteLine("no script, skipped"); return; }

        byte[] before = entry.CurrentBytes.ToArray();
        int highBytes = before.Count(b => b >= 0x80);
        _out.WriteLine($"{Path.GetFileName(path)}: {before.Length:N0} script bytes, "
                     + $"{highBytes:N0} of them above 0x7F");

        var (_, text) = ScriptCommand.Read(doc);
        ScriptCommand.Write(doc, text);
        byte[] after = MapDocument.Load(doc.SaveToBytes()).GetFile("war3map.j")!.CurrentBytes;

        _out.WriteLine($"after a read and an unchanged write: {after.Length:N0} bytes");
        Assert.Equal(before.Length, after.Length);
        Assert.Equal(before, after);
    }

    private static bool ContainsSequence(byte[] haystack, byte[] needle)
    {
        for (int i = 0; i + needle.Length <= haystack.Length; i++)
        {
            bool hit = true;
            for (int j = 0; j < needle.Length; j++)
                if (haystack[i + j] != needle[j]) { hit = false; break; }
            if (hit) return true;
        }
        return false;
    }
}
