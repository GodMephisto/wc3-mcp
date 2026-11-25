// src/Wc3.Render/TextureConvert.cs
using System.Buffers.Binary;
using SkiaSharp;
using War3Net.Drawing.Blp;
using Wc3.Modeling;

namespace Wc3.Render;

/// <summary>
/// Converts image bytes between WC3's .blp/.dds and standard formats
/// (.png/.jpg/.jpeg/.bmp/.tga/.gif, plus any source format <see cref="SafeImages"/> sniffs).
/// DDS is source-only (no encoder), decoded via <see cref="DdsDecoder"/>.
/// Produced .blp files are BLP1 JPEG-content with mipmaps. WC3 stores the JPEG
/// channels as BGRA, so RGBA pixels are swapped before encoding (round-trip
/// through <see cref="BlpDecoder"/> is proven in tests).
/// </summary>
public static class TextureConvert
{
    public static byte[] Convert(byte[] data, string fromExt, string toExt)
    {
        ArgumentNullException.ThrowIfNull(data);
        using var image = LoadRgba(data, fromExt);
        return Save(image, toExt);
    }

    /// <summary>
    /// Converts and shrinks in one step. When the source is larger than <paramref name="maxDimension"/>
    /// on either side, it is scaled down to fit, preserving aspect ratio, then saved. This is the
    /// texture half of making a model compact, since a WC3 texture is usually far larger than the
    /// engine needs and a smaller power-of-two size keeps it valid.
    /// </summary>
    public static byte[] ConvertScaled(byte[] data, string fromExt, string toExt, int maxDimension)
    {
        ArgumentNullException.ThrowIfNull(data);
        using var image = LoadRgba(data, fromExt);
        if (maxDimension > 0 && (image.Width > maxDimension || image.Height > maxDimension))
        {
            int w = image.Width >= image.Height
                ? maxDimension
                : Math.Max(1, (int)Math.Round(image.Width * (double)maxDimension / image.Height));
            int h = image.Height >= image.Width
                ? maxDimension
                : Math.Max(1, (int)Math.Round(image.Height * (double)maxDimension / image.Width));
            using var scaled = image.Resize(
                new SKImageInfo(w, h, SKColorType.Rgba8888, SKAlphaType.Unpremul),
                new SKSamplingOptions(SKCubicResampler.Mitchell));
            return Save(scaled, toExt);
        }
        return Save(image, toExt);
    }

    /// <summary>The pixel dimensions of a texture, decoded without re-encoding.</summary>
    public static (int Width, int Height) Dimensions(byte[] data, string fromExt)
    {
        ArgumentNullException.ThrowIfNull(data);
        using var image = LoadRgba(data, fromExt);
        return (image.Width, image.Height);
    }

    /// <summary>
    /// Decodes any supported source to raw RGBA8, for a caller that needs the pixels rather than a
    /// re-encoded file (an atlas packer, most of all). The same decoder backs every other method, so
    /// .blp and .dds are handled alongside the standard formats.
    /// </summary>
    public static TextureImage DecodeRgba(byte[] data, string fromExt)
    {
        ArgumentNullException.ThrowIfNull(data);
        using var image = LoadRgba(data, fromExt);
        return new TextureImage(image.Width, image.Height, SafeImages.ToRgba(image));
    }

    /// <summary>Encodes a raw RGBA8 buffer to the named target format, the inverse of DecodeRgba.</summary>
    public static byte[] EncodeRgba(TextureImage image, string toExt)
    {
        ArgumentNullException.ThrowIfNull(image);
        using var img = SafeImages.FromRgba(image.Rgba, image.Width, image.Height);
        return Save(img, toExt);
    }

    private static SKBitmap LoadRgba(byte[] data, string fromExt)
    {
        if (Normalize(fromExt) == ".blp")
        {
            var tex = BlpDecoder.Decode(data);
            return SafeImages.FromRgba(tex.Rgba, tex.Width, tex.Height);
        }
        if (Normalize(fromExt) == ".dds")
        {
            var tex = DdsDecoder.Decode(data);
            return SafeImages.FromRgba(tex.Rgba, tex.Width, tex.Height);
        }
        try
        {
            return SafeImages.Load(data);
        }
        catch (NotSupportedException ex)
        {
            throw new NotSupportedException(
                $"unsupported source image format '{fromExt}', use .blp, .dds, .png, .jpg, .jpeg, .bmp, .tga or .gif", ex);
        }
    }

    private static byte[] Save(SKBitmap image, string toExt)
    {
        switch (Normalize(toExt))
        {
            case ".png":
                return SafeImages.EncodePng(image);
            case ".jpg" or ".jpeg":
                return SafeImages.Encode(image, SKEncodedImageFormat.Jpeg, 75);
            case ".bmp":
                return ImageWriters.Bmp(image);
            case ".tga":
                return ImageWriters.Tga(image);
            case ".gif":
                return ImageWriters.Gif(image);
            case ".blp":
            {
                using var ms = new MemoryStream();
                EncodeBlp(image, ms);
                return ms.ToArray();
            }
            default:
                throw new NotSupportedException(
                    $"unsupported target image format '{toExt}', use .png, .jpg, .jpeg, .bmp, .tga, .gif or .blp");
        }
    }

    private static void EncodeBlp(SKBitmap image, Stream output)
    {
        var pixels = SafeImages.ToRgba(image);
        // BLP1 JPEG content stores BGRA (verified against real WC3 assets), swap from RGBA.
        for (int i = 0; i < pixels.Length; i += 4)
            (pixels[i], pixels[i + 2]) = (pixels[i + 2], pixels[i]);
        new BlpEncoder(new Blp1EncodingOptions { GenerateMipmaps = true, JpegQuality = 90 })
            .Encode(output, image.Width, image.Height, pixels);
    }

    private static string Normalize(string ext) =>
        string.IsNullOrEmpty(ext)
            ? ext
            : (ext.StartsWith('.') ? ext : "." + ext).ToLowerInvariant();
}

/// <summary>
/// The encoders Skia does not ship. Skia writes PNG, JPEG and WebP only, so BMP, TGA and GIF are
/// written here. Each one takes straight RGBA8888 and produces the simplest valid file of its
/// format, because the consumers are game clients and texture tools rather than archival storage.
/// </summary>
internal static class ImageWriters
{
    /// <summary>
    /// 24-bit BI_RGB BMP, bottom-up rows, each row padded to four bytes. 24 bits is what the format
    /// is universally read as; a 32-bit BI_RGB header leaves the fourth byte officially unused, so a
    /// reader is free to ignore it and a round trip through BMP would silently drop alpha.
    /// </summary>
    internal static byte[] Bmp(SKBitmap bitmap)
    {
        ArgumentNullException.ThrowIfNull(bitmap);
        int width = bitmap.Width, height = bitmap.Height;
        int stride = (width * 3 + 3) & ~3;
        int imageSize = stride * height;
        var bytes = new byte[54 + imageSize];

        bytes[0] = (byte)'B';
        bytes[1] = (byte)'M';
        BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(2, 4), bytes.Length);
        BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(10, 4), 54);
        BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(14, 4), 40);   // BITMAPINFOHEADER
        BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(18, 4), width);
        BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(22, 4), height);
        BinaryPrimitives.WriteInt16LittleEndian(bytes.AsSpan(26, 2), 1);    // planes
        BinaryPrimitives.WriteInt16LittleEndian(bytes.AsSpan(28, 2), 24);   // bits per pixel
        BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(34, 4), imageSize);
        BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(38, 4), 2835); // 72 dpi
        BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(42, 4), 2835);

        for (int y = 0; y < height; y++)
        {
            int row = 54 + (height - 1 - y) * stride; // BMP rows run bottom-up
            for (int x = 0; x < width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);
                int at = row + x * 3;
                bytes[at] = pixel.Blue;
                bytes[at + 1] = pixel.Green;
                bytes[at + 2] = pixel.Red;
            }
        }
        return bytes;
    }

    /// <summary>
    /// Uncompressed true-colour TGA, 32 bits per pixel, top-left origin. The alpha bits in the image
    /// descriptor are set to 8 so readers keep the alpha channel.
    /// </summary>
    internal static byte[] Tga(SKBitmap bitmap)
    {
        ArgumentNullException.ThrowIfNull(bitmap);
        int width = bitmap.Width, height = bitmap.Height;
        var bytes = new byte[18 + width * height * 4];

        bytes[2] = 2;                                            // uncompressed true-colour
        BinaryPrimitives.WriteUInt16LittleEndian(bytes.AsSpan(12, 2), (ushort)width);
        BinaryPrimitives.WriteUInt16LittleEndian(bytes.AsSpan(14, 2), (ushort)height);
        bytes[16] = 32;                                          // bits per pixel
        bytes[17] = 0x28;                                        // 8 alpha bits, top-left origin

        int at = 18;
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);
                bytes[at++] = pixel.Blue;
                bytes[at++] = pixel.Green;
                bytes[at++] = pixel.Red;
                bytes[at++] = pixel.Alpha;
            }
        return bytes;
    }

    /// <summary>
    /// Single-frame GIF87a. Skia decodes GIF but does not encode it, so this writes the container,
    /// a 6x6x6 colour cube plus a grey ramp for the palette, and LZW-compressed indices. The
    /// quantiser is coarser than a median-cut one, which is a deliberate trade for a few dozen lines
    /// that cannot drift with a library upgrade.
    /// </summary>
    internal static byte[] Gif(SKBitmap bitmap)
    {
        ArgumentNullException.ThrowIfNull(bitmap);
        int width = bitmap.Width, height = bitmap.Height;

        const int Transparent = 255;
        var palette = new byte[256 * 3];
        int index = 0;
        for (int r = 0; r < 6; r++)
            for (int g = 0; g < 6; g++)
                for (int b = 0; b < 6; b++)
                {
                    palette[index * 3] = (byte)(r * 51);
                    palette[index * 3 + 1] = (byte)(g * 51);
                    palette[index * 3 + 2] = (byte)(b * 51);
                    index++;
                }
        for (int grey = 216; grey < Transparent; grey++)
        {
            byte v = (byte)((grey - 216) * 255 / 38);
            palette[grey * 3] = palette[grey * 3 + 1] = palette[grey * 3 + 2] = v;
        }

        var indices = new byte[width * height];
        var nearest = new Dictionary<int, byte>();
        int position = 0;
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);
                if (pixel.Alpha < 128)
                {
                    indices[position++] = Transparent;
                    continue;
                }
                int key = (pixel.Red << 16) | (pixel.Green << 8) | pixel.Blue;
                if (!nearest.TryGetValue(key, out byte mapped))
                {
                    int r = Math.Min(5, (pixel.Red + 25) / 51);
                    int g = Math.Min(5, (pixel.Green + 25) / 51);
                    int b = Math.Min(5, (pixel.Blue + 25) / 51);
                    mapped = (byte)(r * 36 + g * 6 + b);
                    nearest[key] = mapped;
                }
                indices[position++] = mapped;
            }

        using var output = new MemoryStream();
        output.Write(new[] { (byte)'G', (byte)'I', (byte)'F', (byte)'8', (byte)'7', (byte)'a' });
        Span<byte> screen = stackalloc byte[7];
        BinaryPrimitives.WriteUInt16LittleEndian(screen, (ushort)width);
        BinaryPrimitives.WriteUInt16LittleEndian(screen[2..], (ushort)height);
        screen[4] = 0xF7; // global colour table, 8-bit, 256 entries
        screen[5] = 0;    // background colour index
        screen[6] = 0;    // pixel aspect ratio
        output.Write(screen);
        output.Write(palette);

        output.WriteByte(0x21); // extension
        output.WriteByte(0xF9); // graphic control
        output.WriteByte(4);
        output.WriteByte(0x01); // transparent colour given
        output.WriteByte(0);
        output.WriteByte(0);
        output.WriteByte(Transparent);
        output.WriteByte(0);

        output.WriteByte(0x2C); // image descriptor
        Span<byte> frame = stackalloc byte[9];
        BinaryPrimitives.WriteUInt16LittleEndian(frame, 0);
        BinaryPrimitives.WriteUInt16LittleEndian(frame[2..], 0);
        BinaryPrimitives.WriteUInt16LittleEndian(frame[4..], (ushort)width);
        BinaryPrimitives.WriteUInt16LittleEndian(frame[6..], (ushort)height);
        frame[8] = 0; // no local colour table, not interlaced
        output.Write(frame);
        output.WriteByte(8); // LZW minimum code size

        using var compressed = new MemoryStream();
        WriteLzw(compressed, indices, 8);
        var payload = compressed.ToArray();
        for (int at = 0; at < payload.Length; at += 255)
        {
            int count = Math.Min(255, payload.Length - at);
            output.WriteByte((byte)count);
            output.Write(payload, at, count);
        }
        output.WriteByte(0);   // block terminator
        output.WriteByte(0x3B); // trailer
        return output.ToArray();
    }

    private static void WriteLzw(Stream output, byte[] indices, int minimumCodeSize)
    {
        int clearCode = 1 << minimumCodeSize;
        int endCode = clearCode + 1;
        int codeSize = minimumCodeSize + 1;
        int nextCode = endCode + 1;
        var table = new Dictionary<int, int>();
        var bits = new BitWriter(output);

        bits.Write(clearCode, codeSize);
        int prefix = indices[0];
        for (int i = 1; i < indices.Length; i++)
        {
            int suffix = indices[i];
            int key = (prefix << 8) | suffix;
            if (table.TryGetValue(key, out int known))
            {
                prefix = known;
                continue;
            }

            bits.Write(prefix, codeSize);
            if (nextCode < 4096)
            {
                table[key] = nextCode++;
                if (nextCode > (1 << codeSize) && codeSize < 12)
                    codeSize++;
            }
            else
            {
                bits.Write(clearCode, codeSize);
                table.Clear();
                codeSize = minimumCodeSize + 1;
                nextCode = endCode + 1;
            }
            prefix = suffix;
        }

        bits.Write(prefix, codeSize);
        bits.Write(endCode, codeSize);
        bits.Flush();
    }

    /// <summary>Little-endian bit packer, the order GIF's LZW stream expects.</summary>
    private sealed class BitWriter(Stream output)
    {
        private int _buffer;
        private int _count;

        public void Write(int value, int bits)
        {
            _buffer |= value << _count;
            _count += bits;
            while (_count >= 8)
            {
                output.WriteByte((byte)(_buffer & 0xFF));
                _buffer >>= 8;
                _count -= 8;
            }
        }

        public void Flush()
        {
            if (_count > 0)
            {
                output.WriteByte((byte)(_buffer & 0xFF));
                _buffer = 0;
                _count = 0;
            }
        }
    }
}
