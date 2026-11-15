// src/Wc3.Modeling/BlpAudit.cs
// The header checks Devo's Map Doctor runs over every texture, done here from the bytes rather
// than from a decode. On a 101 MB campaign map its report named 94 textures with a header or mip
// levels past the end of the file, a JPEG header above 624 bytes, missing mip levels or a header
// that does not parse, and the engine either reads outside its buffer or draws the missing level
// transparent black. Decoding such a file throws or silently pads, so the header has to be read
// directly to say which of those it is.
using System.Buffers.Binary;

namespace Wc3.Modeling;

/// <summary>Which check fired, so the report can group findings without parsing prose.</summary>
public enum BlpProblem
{
    None,
    NotBlp,
    TruncatedHeader,
    JpegHeaderTooLarge,
    MipPastEnd,
    NoMipLevels,
    MissingFirstMip,
    BadDimensions,
    NotPowerOfTwo,
}

/// <summary>One texture's header verdict.</summary>
public sealed record BlpFinding(string Name, string Problem, int Width, int Height, long Length);

/// <summary>
/// Reads a BLP header without decoding the pixels and reports the defects the engine mishandles.
/// </summary>
/// <remarks>
/// The layouts are the documented ones. BLP1 is magic, content type, and for JPEG content a
/// declared header size before sixteen mip offsets and sixteen mip sizes. BLP2 is magic, version,
/// four single byte fields, width and height, then the same two sixteen entry arrays. Everything is
/// little endian, and a file shorter than its own header cannot be trusted to describe itself.
/// </remarks>
public static class BlpAudit
{
    public const int MaxJpegHeaderBytes = 624;

    /// <summary>How many leading bytes <see cref="Inspect"/> needs, enough for any valid header.</summary>
    public const int PrefixLength = 1024;

    /// <summary>The reported problem, or null when the header holds together.</summary>
    public static string? Inspect(ReadOnlySpan<byte> bytes, long fileLength, out int width, out int height)
        => Inspect(bytes, fileLength, out width, out height, out _);

    /// <summary>The reported problem and its kind, or null when the header holds together.</summary>
    public static string? Inspect(ReadOnlySpan<byte> bytes, long fileLength, out int width, out int height,
        out BlpProblem kind)
    {
        width = 0;
        height = 0;
        kind = BlpProblem.None;
        if (bytes.Length < 8 || fileLength < 8)
        {
            kind = BlpProblem.TruncatedHeader;
            return "a header that does not parse, the file is shorter than 8 bytes";
        }

        var magic = System.Text.Encoding.ASCII.GetString(bytes[..4]);
        if (!magic.StartsWith("BLP", StringComparison.Ordinal) || magic is not ("BLP0" or "BLP1" or "BLP2"))
        {
            kind = BlpProblem.NotBlp;
            return "a header that does not parse, no BLP signature";
        }

        return magic == "BLP2"
            ? InspectBlp2(bytes, fileLength, ref width, ref height, ref kind)
            : InspectBlp1(bytes, fileLength, ref width, ref height, ref kind);
    }

    private static string? InspectBlp1(ReadOnlySpan<byte> bytes, long fileLength, ref int width, ref int height, ref BlpProblem kind)
    {
        // BLP1 content type 0 is palettized and 1 is JPEG, the opposite of the reading I first
        // wrote. A palettized header is 156 bytes, magic and five words then the two tables, and a
        // JPEG header is magic, type, a declared header size, that header, then the tables. Measured
        // against a 10 KB texture War3Net decodes cleanly, reading the palettized case as JPEG put
        // the tables 11 bytes early and reported all 142 of the map's textures as broken.
        uint contentType = BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(4, 4));
        int mipTableAt;
        if (contentType == 0)
        {
            const int palettizedHeader = 4 + 4 * 5 + 128;
            if (fileLength < palettizedHeader)
            {
                kind = BlpProblem.TruncatedHeader;
                return "a header that does not parse, the file is shorter than a BLP1 header";
            }
            mipTableAt = 4 + 4 * 5;
            uint w = BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(12, 4));
            uint h = BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(16, 4));
            if (w is > 0 and <= 4096 && h is > 0 and <= 4096)
            {
                width = (int)w;
                height = (int)h;
            }
        }
        else
        {
            if (fileLength < 12)
            {
                kind = BlpProblem.TruncatedHeader;
                return "a header that does not parse, the file is shorter than a BLP1 header";
            }
            uint jpegHeaderSize = BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(8, 4));
            if (jpegHeaderSize > MaxJpegHeaderBytes)
            {
                kind = BlpProblem.JpegHeaderTooLarge;
                return $"a JPEG header of {jpegHeaderSize} bytes, above the {MaxJpegHeaderBytes} the engine allows";
            }
            mipTableAt = 12 + (int)jpegHeaderSize;
            if (mipTableAt + 128 > fileLength || mipTableAt + 128 > bytes.Length)
            {
                kind = BlpProblem.TruncatedHeader;
                return "a header that does not parse, the JPEG header runs past the end of the file";
            }
        }

        return CheckMips(bytes, mipTableAt, fileLength, ref kind);
    }

    private static string? InspectBlp2(ReadOnlySpan<byte> bytes, long fileLength, ref int width, ref int height, ref BlpProblem kind)
    {
        // magic, version, compression, alphaDepth, alphaType, hasMips, width, height, 16 + 16 uint32.
        const int headerLength = 4 + 4 + 4 + 4 + 4 + 128;
        if (fileLength < headerLength)
        {
            kind = BlpProblem.TruncatedHeader;
            return "a header that does not parse, the file is shorter than a BLP2 header";
        }

        uint w = BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(12, 4));
        uint h = BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(16, 4));
        if (w == 0 || h == 0 || w > 4096 || h > 4096)
        {
            kind = BlpProblem.BadDimensions;
            return $"a header that does not parse, {w} by {h} pixels is outside what the engine reads";
        }
        width = (int)w;
        height = (int)h;
        if (!IsPowerOfTwo(w) || !IsPowerOfTwo(h))
        {
            kind = BlpProblem.NotPowerOfTwo;
            return $"{w} by {h} is not a power of two, which the engine samples as a smeared or black texture";
        }

        byte hasMips = bytes[11];
        var problem = CheckMips(bytes, 20, fileLength, ref kind);
        if (problem is null && hasMips != 0)
        {
            uint first = BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(84, 4));
            if (first == 0)
            {
                kind = BlpProblem.MissingFirstMip;
                return "missing mip levels, the header declares mips and the first one is empty";
            }
        }
        return problem;
    }

    /// <summary>Every declared mip must sit inside the file.</summary>
    private static string? CheckMips(ReadOnlySpan<byte> bytes, int tableAt, long fileLength, ref BlpProblem kind)
    {
        int declared = 0;
        for (int i = 0; i < 16; i++)
        {
            uint offset = BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(tableAt + i * 4, 4));
            uint size = BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(tableAt + 64 + i * 4, 4));
            if (size == 0) continue;
            declared++;
            if ((long)offset + size > fileLength)
            {
                kind = BlpProblem.MipPastEnd;
                return $"mip level {i} ends at {(long)offset + size} bytes in a file of {fileLength}, so the header points past the end";
            }
        }
        if (declared == 0)
        {
            kind = BlpProblem.NoMipLevels;
            return "missing mip levels, the header declares none";
        }
        return null;
    }

    private static bool IsPowerOfTwo(uint v) => v != 0 && (v & (v - 1)) == 0;
}
