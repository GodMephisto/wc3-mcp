// src/Wc3.Render/TextureConvert.cs
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.PixelFormats;
using War3Net.Drawing.Blp;
using Wc3.Modeling;

namespace Wc3.Render;

/// <summary>
/// Converts image bytes between WC3's .blp/.dds and standard formats
/// (.png/.jpg/.jpeg/.bmp/.tga, plus any source format ImageSharp sniffs).
/// DDS is source-only (no encoder), decoded via <see cref="DdsDecoder"/>.
/// Produced .blp files are BLP1 JPEG-content with mipmaps; WC3 stores the JPEG
/// channels as BGRA, so RGBA pixels are swapped before encoding (round-trip
/// through <see cref="BlpDecoder"/> is proven in tests).
/// </summary>
public static class TextureConvert
{
    /// <summary>
    /// Decodes only the five formats this converter documents, and skips embedded metadata.
    /// ImageSharp 3.1.12, the pinned version, has advisories in its TIFF decoder and its ICC
    /// profile parser (GHSA-wmxv-xphr-5c9g, GHSA-gwg2-r3hj-4w44) that are fixed only in 4.1.2,
    /// which carries a license gate. The default configuration sniffs TIFF and reads ICC
    /// profiles from any input, so neither is registered or read here.
    /// </summary>
    internal static readonly DecoderOptions SafeDecode = new()
    {
        Configuration = new Configuration(
            new PngConfigurationModule(), new JpegConfigurationModule(), new BmpConfigurationModule(),
            new TgaConfigurationModule(), new GifConfigurationModule()),
        SkipMetadata = true,
    };

    public static byte[] Convert(byte[] data, string fromExt, string toExt)
    {
        ArgumentNullException.ThrowIfNull(data);
        using var image = LoadRgba(data, fromExt);
        return Save(image, toExt);
    }

    private static Image<Rgba32> LoadRgba(byte[] data, string fromExt)
    {
        if (Normalize(fromExt) == ".blp")
        {
            var tex = BlpDecoder.Decode(data);
            return Image.LoadPixelData<Rgba32>(tex.Rgba, tex.Width, tex.Height);
        }
        if (Normalize(fromExt) == ".dds")
        {
            var tex = DdsDecoder.Decode(data);
            return Image.LoadPixelData<Rgba32>(tex.Rgba, tex.Width, tex.Height);
        }
        try
        {
            return Image.Load<Rgba32>(SafeDecode, data);
        }
        catch (UnknownImageFormatException ex)
        {
            throw new NotSupportedException(
                $"unsupported source image format '{fromExt}' — use .blp, .dds, .png, .jpg, .jpeg, .bmp, .tga or .gif", ex);
        }
    }

    private static byte[] Save(Image<Rgba32> image, string toExt)
    {
        using var ms = new MemoryStream();
        switch (Normalize(toExt))
        {
            case ".png": image.SaveAsPng(ms); break;
            case ".jpg" or ".jpeg": image.SaveAsJpeg(ms); break;
            case ".bmp": image.SaveAsBmp(ms); break;
            case ".tga": image.SaveAsTga(ms); break;
            case ".gif": image.SaveAsGif(ms); break;
            case ".blp": EncodeBlp(image, ms); break;
            default:
                throw new NotSupportedException(
                    $"unsupported target image format '{toExt}' — use .png, .jpg, .jpeg, .bmp, .tga, .gif or .blp");
        }
        return ms.ToArray();
    }

    private static void EncodeBlp(Image<Rgba32> image, Stream output)
    {
        var pixels = new byte[image.Width * image.Height * 4];
        image.CopyPixelDataTo(pixels);
        // BLP1 JPEG content stores BGRA (verified against real WC3 assets) — swap from RGBA.
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
