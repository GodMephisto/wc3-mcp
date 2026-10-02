// src/Wc3.Render/TextureConvert.cs
using SixLabors.ImageSharp;
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
            return Image.Load<Rgba32>(data);
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
