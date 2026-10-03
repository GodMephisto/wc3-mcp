// src/Wc3.MapDocument/ContentTypeSniffer.cs
namespace Wc3.Model;

/// <summary>
/// What an entry holds, judged from its leading bytes and nothing else. An MPQ stores no
/// file names, so a map with a stripped listfile leaves entries that can be read but not
/// named, and this is the answer to "what is it" that survives that. DisplayName is for a
/// person ("BLP texture"), Extension is the likely on-disk extension for a generated
/// filename ("blp", falling back to "bin" when nothing is known), IsEmpty flags a zero-byte
/// entry, and IsIdentified says whether the bytes matched a known signature (empty and
/// unknown both report false).
/// </summary>
public sealed record SniffedContentType(string DisplayName, string Extension, bool IsEmpty, bool IsIdentified)
{
    public static readonly SniffedContentType Empty = new("empty", "bin", true, false);
    public static readonly SniffedContentType Unknown = new("unknown", "bin", false, false);

    /// <summary>
    /// The entry declares a size but none of its bytes could be read.
    ///
    /// Distinct from <see cref="Empty"/> on purpose. A protected archive's stuffed entries pass
    /// the loader's open probe, so they carry a real declared size, and then fail on actual
    /// decompression, so the prefix read yields nothing. Reporting those as "empty" put entries
    /// of 43 MB and 104 MB in a listing under the word empty, and it is the same conflation that
    /// produced the claim these archives hold 50,000 empty padding entries when almost none of
    /// them are empty.
    ///
    /// IsEmpty stays false, because the entry is not empty. IsIdentified stays false, because
    /// nothing was identified. Callers measuring an identification RATE should exclude these
    /// from the denominator, since no content was available to identify.
    /// </summary>
    public static readonly SniffedContentType Unreadable = new("unreadable", "bin", false, false);
}

/// <summary>
/// Types a file entry from its first few hundred bytes. Only signatures that actually occur
/// in Warcraft III archives are recognised, measured across the map library by the census in
/// tests/Wc3.Tests/UnnamedEntryTypeProbe.cs (BLP, MDX, DDS, MP3, WAV and text cover about
/// 99 percent of the non-empty nameless entries). Anything else reports unknown, because a
/// plausible wrong answer is worse than no answer, a mislabeled entry would be extracted
/// under a lying extension.
///
/// The sniff is deliberately cheap. It needs at most <see cref="PrefixLength"/> bytes, and
/// the entry overload reads them through <see cref="MapFileEntry.ReadPrefix"/>, which pulls
/// only the leading sectors of a compressed entry. Some archives are 250 MB with tens of
/// thousands of entries, and Load defers full decompression on purpose, so typing every
/// entry must never force it.
/// </summary>
public static class ContentTypeSniffer
{
    /// <summary>How many leading bytes a caller should provide. More adds nothing, every
    /// signature below sits inside the first dozen bytes and the text heuristic looks at
    /// this many.</summary>
    public const int PrefixLength = 512;

    /// <summary>
    /// Types the entry's current content, reading at most <see cref="PrefixLength"/> bytes.
    /// The verdict is cached on the entry, so listing the same map twice sniffs once. Pending
    /// replacement bytes (<see cref="MapFileEntry.OverrideBytes"/>) are sniffed uncached, they
    /// are already in memory and can change again.
    /// </summary>
    public static SniffedContentType Sniff(MapFileEntry entry)
    {
        if (entry.OverrideBytes is { } pending)
            return Sniff(pending);
        if (entry.CurrentSize == 0) return entry.SniffCache ??= SniffedContentType.Empty;

        // A declared size with no readable bytes is not emptiness. The span overload cannot tell
        // the difference, because both arrive as zero bytes, so the distinction has to be drawn
        // here where the declared size is still in hand.
        var prefix = entry.ReadPrefix(PrefixLength);
        return entry.SniffCache ??= prefix.Length == 0
            ? SniffedContentType.Unreadable
            : Sniff(prefix);
    }

    /// <summary>Types content from its leading bytes. Pass the whole payload or any prefix
    /// of it, only the first <see cref="PrefixLength"/> bytes are examined.</summary>
    public static SniffedContentType Sniff(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length == 0) return SniffedContentType.Empty;

        if (Is(bytes, "BLP0") || Is(bytes, "BLP1") || Is(bytes, "BLP2"))
            return new SniffedContentType("BLP texture", "blp", false, true);
        if (Is(bytes, "MDLX"))
            return new SniffedContentType("MDX model", "mdx", false, true);
        if (Is(bytes, "DDS "))
            return new SniffedContentType("DDS texture", "dds", false, true);
        if (Is(bytes, "RIFF") && Is(bytes, "WAVE", 8))
            return new SniffedContentType("WAV audio", "wav", false, true);
        if (Is(bytes, "OggS"))
            return new SniffedContentType("OGG audio", "ogg", false, true);
        if (Is(bytes, "ID3") || IsMp3FrameHeader(bytes))
            return new SniffedContentType("MP3 audio", "mp3", false, true);
        if (bytes.Length >= 4 && bytes[0] == 0x89 && Is(bytes, "PNG", 1))
            return new SniffedContentType("PNG image", "png", false, true);
        if (bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
            return new SniffedContentType("JPEG image", "jpg", false, true);
        if (Is(bytes, "MPQ") && bytes.Length >= 4 && (bytes[3] == 0x1A || bytes[3] == 0x1B))
            return new SniffedContentType("nested MPQ archive", "mpq", false, true);
        if (Is(bytes, "HM3W"))
            return new SniffedContentType("nested map", "w3x", false, true);
        if (bytes.Length >= 4 && bytes[0] == 0 && bytes[1] == 1 && bytes[2] == 0 && bytes[3] == 0)
            return new SniffedContentType("TrueType font", "ttf", false, true);
        if (Is(bytes, "ttcf"))
            return new SniffedContentType("TrueType font", "ttf", false, true);
        if (Is(bytes, "OTTO"))
            return new SniffedContentType("OpenType font", "otf", false, true);
        // BMP's magic is only two bytes, so also require the header's reserved fields to be
        // zero, otherwise any text starting with "BM" would be claimed as an image.
        if (Is(bytes, "BM") && bytes.Length >= 14
            && bytes[6] == 0 && bytes[7] == 0 && bytes[8] == 0 && bytes[9] == 0)
            return new SniffedContentType("BMP image", "bmp", false, true);

        if (LooksLikeText(bytes))
            return new SniffedContentType("text", "txt", false, true);

        return SniffedContentType.Unknown;
    }

    private static bool Is(ReadOnlySpan<byte> bytes, string tag, int at = 0)
    {
        if (at + tag.Length > bytes.Length) return false;
        for (int i = 0; i < tag.Length; i++)
            if (bytes[at + i] != tag[i]) return false;
        return true;
    }

    /// <summary>
    /// A validated MPEG audio frame header, not just the two sync bytes. The version, layer,
    /// bitrate and sample-rate fields must all hold legal values, because a bare 0xFF 0xEx
    /// test claims one random binary in every few hundred as music.
    /// </summary>
    private static bool IsMp3FrameHeader(ReadOnlySpan<byte> b)
    {
        if (b.Length < 4) return false;
        if (b[0] != 0xFF || (b[1] & 0xE0) != 0xE0) return false;
        if (((b[1] >> 3) & 0x03) == 0x01) return false;   // reserved MPEG version
        if (((b[1] >> 1) & 0x03) == 0x00) return false;   // reserved layer
        if ((b[2] >> 4) == 0x0F) return false;            // invalid bitrate
        if (((b[2] >> 2) & 0x03) == 0x03) return false;   // reserved sample rate
        return true;
    }

    /// <summary>Mostly printable with no NUL in the first block reads as text, the same
    /// heuristic the file preview uses.</summary>
    private static bool LooksLikeText(ReadOnlySpan<byte> bytes)
    {
        int n = Math.Min(bytes.Length, PrefixLength), printable = 0;
        for (int i = 0; i < n; i++)
        {
            byte c = bytes[i];
            if (c == 0) return false;
            if (c is >= 32 and < 127 or 9 or 10 or 13) printable++;
        }
        return printable > n * 0.9;
    }
}
