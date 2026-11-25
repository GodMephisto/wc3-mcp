// src/Wc3.Render/SafeImages.cs
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.PixelFormats;

namespace Wc3.Render;

/// <summary>
/// The one decode configuration this library uses. Every call that turns bytes into an image
/// goes through here so a whole class of parser cannot be reached by a crafted map file.
/// </summary>
/// <remarks>
/// TIFF is deliberately absent. ImageSharp 3.1.12, the pinned version, carries open advisories in
/// its TIFF decoders and encoders (GHSA-v76p-62qx-wwq2, GHSA-j9gm-c75j-xc9q, GHSA-wmxv-xphr-5c9g,
/// GHSA-jjfr-hcj7-qf5w) and its ICC profile parser (GHSA-gwg2-r3hj-4w44), all of which are fixed
/// only in 4.1.1 and later, and that line requires a Six Labors licence key at build time.
/// Registering no TIFF module and skipping metadata keeps every one of those code paths out of the
/// product, whatever bytes arrive. A sixth advisory (GHSA-j3p4-wp97-rph4) sits in
/// HistogramEqualization, a processing operation nothing here calls.
/// </remarks>
public static class SafeImages
{
    /// <summary>
    /// PNG, JPEG, BMP, TGA and GIF only, with metadata skipped so no ICC profile is read.
    /// </summary>
    public static readonly DecoderOptions Decode = new()
    {
        Configuration = new Configuration(
            new PngConfigurationModule(), new JpegConfigurationModule(), new BmpConfigurationModule(),
            new TgaConfigurationModule(), new GifConfigurationModule()),
        SkipMetadata = true,
    };

    /// <summary>Decodes with <see cref="Decode"/>, so no caller can pick up the default sniffer.</summary>
    public static Image<TPixel> Load<TPixel>(byte[] data) where TPixel : unmanaged, IPixel<TPixel>
        => Image.Load<TPixel>(Decode, data);
}
