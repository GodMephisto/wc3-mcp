// tests/Wc3.Tests/ContentTypeSnifferTests.cs
using System.Text;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// The content sniffer against synthetic payloads, one per recognised signature, plus the
/// contract that matters most, bytes that match nothing report unknown instead of a guess.
/// The entry-level tests pin the laziness contract, sniffing a deferred entry must read a
/// prefix only and never materialize the whole entry.
/// </summary>
public class ContentTypeSnifferTests
{
    private static byte[] Bytes(params int[] values) => values.Select(v => (byte)v).ToArray();

    private static byte[] Tagged(string tag, int length = 64)
    {
        var b = new byte[length];
        Encoding.ASCII.GetBytes(tag).CopyTo(b, 0);
        return b;
    }

    public static TheoryData<byte[], string, string> RecognisedPayloads() => new()
    {
        { Tagged("BLP0"), "BLP texture", "blp" },
        { Tagged("BLP1"), "BLP texture", "blp" },
        { Tagged("BLP2"), "BLP texture", "blp" },
        { Tagged("MDLX"), "MDX model", "mdx" },
        { Tagged("DDS "), "DDS texture", "dds" },
        { Tagged("RIFF????WAVE").Select((b, i) => i is >= 4 and < 8 ? (byte)0x10 : b).ToArray(), "WAV audio", "wav" },
        { Tagged("OggS"), "OGG audio", "ogg" },
        { Tagged("ID3"), "MP3 audio", "mp3" },
        // A real MPEG frame header, 0xFF sync then valid version, layer, bitrate, sample rate.
        { Bytes(0xFF, 0xFB, 0x90, 0x44, 0, 0, 0, 0), "MP3 audio", "mp3" },
        { Bytes(0x89, 'P', 'N', 'G', 0x0D, 0x0A, 0x1A, 0x0A), "PNG image", "png" },
        { Bytes(0xFF, 0xD8, 0xFF, 0xE0, 0, 0x10, 'J', 'F', 'I', 'F'), "JPEG image", "jpg" },
        { Bytes('M', 'P', 'Q', 0x1A, 0, 0, 0, 0), "nested MPQ archive", "mpq" },
        { Tagged("HM3W"), "nested map", "w3x" },
        { Bytes(0x00, 0x01, 0x00, 0x00, 0x00, 0x0F), "TrueType font", "ttf" },
        { Tagged("ttcf"), "TrueType font", "ttf" },
        { Tagged("OTTO"), "OpenType font", "otf" },
        { Bytes('B', 'M', 0x36, 0x10, 0, 0, 0, 0, 0, 0, 0x36, 0, 0, 0), "BMP image", "bmp" },
        { Encoding.ASCII.GetBytes("function main takes nothing returns nothing\nendfunction\n"), "text", "txt" },
    };

    [Theory]
    [MemberData(nameof(RecognisedPayloads))]
    public void Each_signature_is_recognised(byte[] payload, string displayName, string extension)
    {
        var t = ContentTypeSniffer.Sniff(payload);
        Assert.Equal(displayName, t.DisplayName);
        Assert.Equal(extension, t.Extension);
        Assert.True(t.IsIdentified);
        Assert.False(t.IsEmpty);
    }

    [Fact]
    public void Unknown_bytes_report_unknown_not_a_guess()
    {
        // No known magic, and the NUL bytes fail the text heuristic.
        var t = ContentTypeSniffer.Sniff(Bytes(0xDE, 0xAD, 0x00, 0xBE, 0xEF, 0x00, 0x01, 0x02));
        Assert.Equal("unknown", t.DisplayName);
        Assert.Equal("bin", t.Extension);
        Assert.False(t.IsIdentified);
        Assert.False(t.IsEmpty);
    }

    [Fact]
    public void A_bare_sync_byte_pair_is_not_enough_to_claim_mp3()
    {
        // 0xFF 0xF0 has the sync bits but a reserved layer, random binary in disguise.
        var t = ContentTypeSniffer.Sniff(Bytes(0xFF, 0xF0, 0x00, 0x00, 0x00, 0x00));
        Assert.NotEqual("MP3 audio", t.DisplayName);
        Assert.False(t.IsIdentified);
    }

    [Fact]
    public void Empty_content_reports_empty()
    {
        var t = ContentTypeSniffer.Sniff(ReadOnlySpan<byte>.Empty);
        Assert.True(t.IsEmpty);
        Assert.False(t.IsIdentified);
        Assert.Equal("empty", t.DisplayName);
    }

    [Fact]
    public void Sniffing_a_deferred_entry_reads_a_prefix_without_materializing_it()
    {
        // Big enough that a full materialization would be several compressed sectors.
        var blp = Tagged("BLP1", 64 * 1024);
        var doc = Wc3.Model.MapDocument.Load(SyntheticMap.Build(
            new Dictionary<string, byte[]> { ["war3map.j"] = Encoding.ASCII.GetBytes("// x\n") },
            new[] { blp }));

        var entry = doc.Files.Single(f => f.FileName is null);
        Assert.False(entry.IsMaterialized);

        var t = ContentTypeSniffer.Sniff(entry);
        Assert.Equal("BLP texture", t.DisplayName);
        Assert.False(entry.IsMaterialized);
    }

    [Fact]
    public void A_pending_replacement_is_sniffed_not_the_original_bytes()
    {
        var doc = Wc3.Model.MapDocument.Load(SyntheticMap.Build(
            new Dictionary<string, byte[]> { ["war3map.j"] = Encoding.ASCII.GetBytes("// x\n") },
            new[] { Tagged("BLP1") }));

        var entry = doc.Files.Single(f => f.FileName is null);
        Assert.Equal("BLP texture", ContentTypeSniffer.Sniff(entry).DisplayName);

        entry.OverrideBytes = Tagged("MDLX");
        Assert.Equal("MDX model", ContentTypeSniffer.Sniff(entry).DisplayName);
    }

    [Fact]
    public void A_zero_size_entry_reports_empty_without_any_read()
    {
        var doc = Wc3.Model.MapDocument.Load(SyntheticMap.Build(
            new Dictionary<string, byte[]> { ["war3map.j"] = Encoding.ASCII.GetBytes("// x\n") },
            new[] { Array.Empty<byte>() }));

        var entry = doc.Files.Single(f => f.FileName is null);
        var t = ContentTypeSniffer.Sniff(entry);
        Assert.True(t.IsEmpty);
        Assert.False(entry.IsMaterialized);
    }
}
