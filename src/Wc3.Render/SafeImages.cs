// src/Wc3.Render/SafeImages.cs
using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;
using SkiaSharp;

namespace Wc3.Render;

/// <summary>
/// The one decode configuration this library uses, and the one encoder for the formats Skia
/// provides. Every call that turns bytes into an image goes through here so a whole class of
/// parser cannot be reached by a crafted map file.
/// </summary>
/// <remarks>
/// <para>The allow-list is explicit, PNG, JPEG, BMP, GIF and TGA, decided by a magic-number sniff
/// here rather than by whatever sniffer the imaging library ships. Anything else is refused before
/// a decoder is constructed, so TIFF, WebP, HEIF and the rest are out of reach by construction, not
/// by configuration.</para>
/// <para>Metadata is stripped before decode rather than skipped during it, PNG chunks other than
/// IHDR/PLTE/tRNS/IDAT/IEND and JPEG APP1..APP15/COM segments are removed first, so no ICC profile,
/// EXIF block or text comment reaches the decoder. SkiaSharp offers no switch to ignore chunks, so
/// taking them out of the buffer is what keeps the same guarantee.</para>
/// <para>TIFF stays out for the same reason it was out under ImageSharp. Its decoders carried open
/// advisories, and nothing here needs it. TGA has no Skia codec at all, so the reader below is ours.
/// It accepts only uncompressed or RLE true-colour 24/32-bit images, which is the whole of what WC3
/// assets use, and everything else is refused.</para>
/// </remarks>
public static class SafeImages
{
    /// <summary>
    /// Ceiling on decoded pixels, so a crafted header cannot ask for an unbounded allocation before
    /// any pixel data has been validated. 2^26 is 67 megapixels, far above any map texture.
    /// </summary>
    public const int MaxPixels = 1 << 26;

    private static readonly byte[] PngMagic = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

    /// <summary>A blank, fully transparent image in this library's pixel layout, straight RGBA8888.</summary>
    public static SKBitmap NewBitmap(int width, int height)
    {
        var bitmap = new SKBitmap(new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Unpremul));
        // Skia does not promise zeroed pixels, and every renderer here starts from transparent.
        bitmap.Erase(SKColors.Transparent);
        return bitmap;
    }

    /// <summary>Wraps a tightly packed RGBA8888 buffer as an image.</summary>
    public static SKBitmap FromRgba(byte[] rgba, int width, int height)
    {
        ArgumentNullException.ThrowIfNull(rgba);
        if (width <= 0 || height <= 0)
            throw new ArgumentOutOfRangeException(nameof(width), "image dimensions must be positive");
        if (rgba.Length < width * height * 4)
            throw new ArgumentException("pixel buffer is smaller than the image", nameof(rgba));
        var bitmap = NewBitmap(width, height);
        System.Runtime.InteropServices.Marshal.Copy(rgba, 0, bitmap.GetPixels(), width * height * 4);
        return bitmap;
    }

    /// <summary>The image as a tightly packed RGBA8888 buffer, the inverse of <see cref="FromRgba"/>.</summary>
    public static byte[] ToRgba(SKBitmap bitmap)
    {
        ArgumentNullException.ThrowIfNull(bitmap);
        var rgba = new byte[bitmap.Width * bitmap.Height * 4];
        System.Runtime.InteropServices.Marshal.Copy(bitmap.GetPixels(), rgba, 0, rgba.Length);
        return rgba;
    }

    /// <summary>
    /// Decodes <paramref name="data"/> through the allow-list, returning straight RGBA8888.
    /// </summary>
    /// <exception cref="NotSupportedException">
    /// The bytes are not one of the allowed formats, are malformed, or ask for more than
    /// <see cref="MaxPixels"/> pixels.
    /// </exception>
    public static SKBitmap Load(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        if (data.Length == 0)
            throw new NotSupportedException("could not decode image data (empty buffer)");

        // TGA carries no magic number, so it is recognised by its header instead. Its second byte
        // has to be zero, while a PNG, JPEG, BMP or GIF signature puts a non-zero byte there, so
        // the header check cannot claim a file that has a magic number of its own.
        if (TryDecodeTga(data, out var tga))
            return tga!;

        var format = Sniff(data)
            ?? throw new NotSupportedException(
                "could not decode image data (not .png, .jpg, .bmp, .gif or .tga)");

        byte[] clean = format is SKEncodedImageFormat.Png or SKEncodedImageFormat.Jpeg
            ? StripMetadata(data, format)
            : data;

        using var skData = SKData.CreateCopy(clean);
        using var codec = SKCodec.Create(skData);
        if (codec is null || codec.EncodedFormat != format)
            throw new NotSupportedException($"could not decode image data (malformed {format})");

        int width = codec.Info.Width, height = codec.Info.Height;
        if (width <= 0 || height <= 0 || (long)width * height > MaxPixels)
            throw new NotSupportedException(
                $"could not decode image data ({width}x{height} is outside 1x1..{MaxPixels} pixels)");

        var info = new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Unpremul);
        var bitmap = new SKBitmap(info);
        var result = codec.GetPixels(info, bitmap.GetPixels());
        // IncompleteInput still yields the rows that were present. The old decoder behaved the
        // same way, so a truncated texture decodes as far as it goes rather than throwing.
        if (result is not (SKCodecResult.Success or SKCodecResult.IncompleteInput))
        {
            bitmap.Dispose();
            throw new NotSupportedException($"could not decode image data ({result})");
        }
        return bitmap;
    }

    /// <summary>True when the bytes start with the PNG signature.</summary>
    public static bool IsPng(byte[] data) =>
        data is { Length: >= 8 } && data.AsSpan(0, 8).SequenceEqual(PngMagic);

    /// <summary>Encodes straight RGBA8888 as PNG bytes.</summary>
    public static byte[] EncodePng(SKBitmap bitmap) => Encode(bitmap, SKEncodedImageFormat.Png, 100);

    /// <summary>
    /// Encodes to a format Skia can write. <paramref name="quality"/> is the JPEG quality and is
    /// ignored by the lossless codecs.
    /// </summary>
    /// <exception cref="NotSupportedException">Skia has no encoder for the requested format.</exception>
    public static byte[] Encode(SKBitmap bitmap, SKEncodedImageFormat format, int quality = 75)
    {
        ArgumentNullException.ThrowIfNull(bitmap);
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(format, quality)
            ?? throw new NotSupportedException($"no {format} encoder in this build");
        return data.ToArray();
    }

    /// <summary>
    /// Encodes 16-bit luminance samples as a grayscale PNG, one sample per pixel, no alpha.
    /// Skia has no 16-bit grayscale colour type, so this is written by hand. The only consumers are
    /// terrain heightmaps, where the extra eight bits are the difference between an exact round trip
    /// and a rounded one.
    /// </summary>
    public static byte[] EncodeGray16Png(int width, int height, ReadOnlySpan<ushort> samples)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentOutOfRangeException(nameof(width), "image dimensions must be positive");
        if (samples.Length < width * height)
            throw new ArgumentException("sample buffer is smaller than the image", nameof(samples));

        using var output = new MemoryStream();
        output.Write(PngMagic);

        Span<byte> header = stackalloc byte[13];
        BinaryPrimitives.WriteInt32BigEndian(header, width);
        BinaryPrimitives.WriteInt32BigEndian(header[4..], height);
        header[8] = 16;   // bit depth
        header[9] = 0;    // grayscale colour type
        header[10] = 0;   // deflate
        header[11] = 0;   // adaptive filtering
        header[12] = 0;   // not interlaced
        WritePngChunk(output, "IHDR", header);

        var raw = new byte[height * (1 + width * 2)]; // filter byte + two bytes per sample
        for (int y = 0; y < height; y++)
        {
            int row = y * (1 + width * 2);
            raw[row] = 0; // filter type 0 means none
            for (int x = 0; x < width; x++)
                BinaryPrimitives.WriteUInt16BigEndian(
                    raw.AsSpan(row + 1 + x * 2, 2), samples[y * width + x]);
        }

        using var compressed = new MemoryStream();
        using (var deflate = new ZLibStream(compressed, CompressionLevel.Optimal, leaveOpen: true))
            deflate.Write(raw);
        WritePngChunk(output, "IDAT", compressed.ToArray());
        WritePngChunk(output, "IEND", ReadOnlySpan<byte>.Empty);
        return output.ToArray();
    }

    /// <summary>
    /// Reads a non-interlaced 8- or 16-bit grayscale PNG into 16-bit samples, exactly. Returns false
    /// for anything else, including every other PNG colour type, so the caller falls back to the
    /// general decoder.
    /// </summary>
    public static bool TryDecodeGray16Png(byte[] png, out int width, out int height, out ushort[] samples)
    {
        width = height = 0;
        samples = Array.Empty<ushort>();
        if (!IsPng(png))
            return false;

        int bitDepth = 0, colourType = -1, interlace = -1;
        using var idat = new MemoryStream();
        int offset = 8;
        bool sawHeader = false;
        while (offset + 12 <= png.Length)
        {
            int length = BinaryPrimitives.ReadInt32BigEndian(png.AsSpan(offset, 4));
            if (length < 0 || offset + 12 + length > png.Length)
                return false;
            string type = Encoding.ASCII.GetString(png, offset + 4, 4);
            if (type == "IHDR")
            {
                if (length != 13) return false;
                width = BinaryPrimitives.ReadInt32BigEndian(png.AsSpan(offset + 8, 4));
                height = BinaryPrimitives.ReadInt32BigEndian(png.AsSpan(offset + 12, 4));
                bitDepth = png[offset + 16];
                colourType = png[offset + 17];
                interlace = png[offset + 20];
                sawHeader = true;
            }
            else if (type == "IDAT")
            {
                idat.Write(png, offset + 8, length);
            }
            else if (type == "IEND") break;
            offset += 12 + length;
        }

        if (!sawHeader || colourType != 0 || interlace != 0 || bitDepth is not (8 or 16))
            return false;
        if (width <= 0 || height <= 0 || (long)width * height > MaxPixels)
            return false;

        int bytesPerSample = bitDepth / 8;
        int stride = width * bytesPerSample;
        var raw = new byte[height * (1 + stride)];
        idat.Position = 0;
        try
        {
            using var inflate = new ZLibStream(idat, CompressionMode.Decompress, leaveOpen: true);
            int read = 0;
            while (read < raw.Length)
            {
                int n = inflate.Read(raw, read, raw.Length - read);
                if (n == 0) break;
                read += n;
            }
            if (read < raw.Length) return false;
        }
        catch (InvalidDataException)
        {
            return false;
        }

        if (!Unfilter(raw, height, stride, bytesPerSample))
            return false;

        var result = new ushort[width * height];
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                int at = y * (1 + stride) + 1 + x * bytesPerSample;
                // An 8-bit source scales by 257 so 255 maps to 65535, matching the old conversion.
                result[y * width + x] = bitDepth == 16
                    ? BinaryPrimitives.ReadUInt16BigEndian(raw.AsSpan(at, 2))
                    : (ushort)(raw[at] * 257);
            }

        samples = result;
        return true;
    }

    /// <summary>The format a magic number says these bytes are, or null for anything else.</summary>
    private static SKEncodedImageFormat? Sniff(byte[] data)
    {
        if (data.Length >= 8 && data.AsSpan(0, 8).SequenceEqual(PngMagic))
            return SKEncodedImageFormat.Png;
        if (data.Length >= 3 && data[0] == 0xFF && data[1] == 0xD8 && data[2] == 0xFF)
            return SKEncodedImageFormat.Jpeg;
        if (data.Length >= 2 && data[0] == 'B' && data[1] == 'M')
            return SKEncodedImageFormat.Bmp;
        if (data.Length >= 6 && data[0] == 'G' && data[1] == 'I' && data[2] == 'F' && data[3] == '8')
            return SKEncodedImageFormat.Gif;
        return null;
    }

    /// <summary>
    /// Removes every PNG chunk except the ones that carry pixels or frame the file, and every JPEG
    /// APP1..APP15 and COM segment. The removal happens before the decoder sees the buffer.
    /// </summary>
    private static byte[] StripMetadata(byte[] data, SKEncodedImageFormat format) =>
        format == SKEncodedImageFormat.Png ? StripPngMetadata(data) : StripJpegMetadata(data);

    private static byte[] StripPngMetadata(byte[] png)
    {
        using var output = new MemoryStream(png.Length);
        output.Write(png, 0, 8);
        int offset = 8, kept = 0;
        while (offset + 12 <= png.Length)
        {
            int length = BinaryPrimitives.ReadInt32BigEndian(png.AsSpan(offset, 4));
            if (length < 0 || offset + 12 + length > png.Length)
                return png; // malformed, hand the original to the decoder, which will refuse it
            string type = Encoding.ASCII.GetString(png, offset + 4, 4);
            bool keep = type is "IHDR" or "PLTE" or "tRNS" or "IDAT" or "IEND";
            if (keep)
            {
                output.Write(png, offset, 12 + length);
                kept++;
            }
            offset += 12 + length;
        }
        return kept > 0 ? output.ToArray() : png;
    }

    private static byte[] StripJpegMetadata(byte[] jpg)
    {
        if (jpg.Length < 4)
            return jpg;
        using var output = new MemoryStream(jpg.Length);
        output.WriteByte(0xFF);
        output.WriteByte(0xD8);
        int offset = 2;
        while (offset + 4 <= jpg.Length)
        {
            if (jpg[offset] != 0xFF)
                return jpg;
            byte marker = jpg[offset + 1];
            if (marker == 0xDA) // start of scan, entropy data follows to end of file
            {
                output.Write(jpg, offset, jpg.Length - offset);
                return output.ToArray();
            }
            if (marker == 0xD9)
                break;
            int length = (jpg[offset + 2] << 8) | jpg[offset + 3];
            if (length < 2 || offset + 2 + length > jpg.Length)
                return jpg;
            // APP0 (JFIF) is a layout header, kept. Its neighbours carry ICC and EXIF, dropped.
            bool drop = (marker >= 0xE1 && marker <= 0xEF) || marker == 0xFE;
            if (!drop)
                output.Write(jpg, offset, 2 + length);
            offset += 2 + length;
        }
        return output.ToArray();
    }

    /// <summary>
    /// Reads a TGA true-colour image. Only types 2 (uncompressed) and 10 (RLE) at 24 or 32 bits per
    /// pixel with no colour map are accepted, and the header must also describe a plausible image, so a
    /// file of random bytes cannot be mistaken for one.
    /// </summary>
    private static bool TryDecodeTga(byte[] data, out SKBitmap? bitmap)
    {
        bitmap = null;
        if (data.Length < 18 || data[1] != 0 || data[2] is not (2 or 10))
            return false;

        int width = BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(12, 2));
        int height = BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(14, 2));
        int depth = data[16];
        int descriptor = data[17];
        if (width <= 0 || height <= 0 || depth is not (24 or 32) || (descriptor & 0x10) != 0)
            return false;
        if ((long)width * height > MaxPixels)
            return false;

        bool topDown = (descriptor & 0x20) != 0;
        int bytesPerPixel = depth / 8;
        int offset = 18 + data[0];
        if (offset >= data.Length)
            return false;

        // Every pixel is at least one byte, so a file shorter than this cannot be the image it claims.
        long pixels = (long)width * height;
        if (data.Length - offset < pixels)
            return false;

        var result = NewBitmap(width, height);
        int written = 0;
        try
        {
            while (written < pixels)
            {
                int runLength = 1;
                bool run = false;
                if (data[2] == 10) // RLE
                {
                    if (offset >= data.Length) { result.Dispose(); return false; }
                    byte packet = data[offset++];
                    run = (packet & 0x80) != 0;
                    runLength = (packet & 0x7F) + 1;
                }

                for (int i = 0; i < runLength; i++)
                {
                    int at = offset + (run ? 0 : i) * bytesPerPixel;
                    if (at + bytesPerPixel > data.Length) { result.Dispose(); return false; }
                    int px = written % width;
                    int py = written / width;
                    if (!topDown) py = height - 1 - py;
                    result.SetPixel(px, py, new SKColor(
                        data[at + 2], data[at + 1], data[at],
                        bytesPerPixel == 4 ? data[at + 3] : (byte)255));
                    written++;
                }
                if (!run) offset += runLength * bytesPerPixel;
                else offset += bytesPerPixel;
            }
        }
        catch (IndexOutOfRangeException)
        {
            result.Dispose();
            return false;
        }

        bitmap = result;
        return true;
    }

    /// <summary>Reverses the per-row PNG filters in place. Returns false on an unknown filter type.</summary>
    private static bool Unfilter(byte[] raw, int height, int stride, int bytesPerPixel)
    {
        int rowLength = 1 + stride;
        for (int y = 0; y < height; y++)
        {
            int row = y * rowLength;
            byte filter = raw[row];
            if (filter == 0) continue;
            if (filter > 4) return false;

            for (int x = 0; x < stride; x++)
            {
                int at = row + 1 + x;
                int left = x >= bytesPerPixel ? raw[at - bytesPerPixel] : 0;
                int up = y > 0 ? raw[at - rowLength] : 0;
                int upLeft = y > 0 && x >= bytesPerPixel ? raw[at - rowLength - bytesPerPixel] : 0;
                int value = raw[at];
                raw[at] = filter switch
                {
                    1 => (byte)(value + left),
                    2 => (byte)(value + up),
                    3 => (byte)(value + (left + up) / 2),
                    _ => (byte)(value + Paeth(left, up, upLeft)),
                };
            }
        }
        return true;
    }

    private static int Paeth(int a, int b, int c)
    {
        int p = a + b - c;
        int pa = Math.Abs(p - a), pb = Math.Abs(p - b), pc = Math.Abs(p - c);
        return pa <= pb && pa <= pc ? a : pb <= pc ? b : c;
    }

    private static void WritePngChunk(Stream output, string type, ReadOnlySpan<byte> data)
    {
        Span<byte> length = stackalloc byte[4];
        BinaryPrimitives.WriteInt32BigEndian(length, data.Length);
        output.Write(length);

        var typeBytes = Encoding.ASCII.GetBytes(type);
        output.Write(typeBytes);
        output.Write(data);

        uint crc = Crc32(typeBytes, data);
        Span<byte> crcBytes = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(crcBytes, crc);
        output.Write(crcBytes);
    }

    private static readonly uint[] CrcTable = BuildCrcTable();

    private static uint[] BuildCrcTable()
    {
        var table = new uint[256];
        for (uint n = 0; n < 256; n++)
        {
            uint c = n;
            for (int k = 0; k < 8; k++)
                c = (c & 1) != 0 ? 0xEDB88320u ^ (c >> 1) : c >> 1;
            table[n] = c;
        }
        return table;
    }

    private static uint Crc32(ReadOnlySpan<byte> type, ReadOnlySpan<byte> data)
    {
        uint c = 0xFFFFFFFFu;
        foreach (byte b in type) c = CrcTable[(c ^ b) & 0xFF] ^ (c >> 8);
        foreach (byte b in data) c = CrcTable[(c ^ b) & 0xFF] ^ (c >> 8);
        return c ^ 0xFFFFFFFFu;
    }
}
