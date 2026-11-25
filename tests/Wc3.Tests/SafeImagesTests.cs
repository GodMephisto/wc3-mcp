// tests/Wc3.Tests/SafeImagesTests.cs
using SkiaSharp;
using Wc3.Render;

namespace Wc3.Tests;

/// <summary>
/// Every decode in Wc3.Render goes through <see cref="SafeImages"/>, which decides the format from
/// a magic-number allow-list before any decoder exists. One test per entry point, because a single
/// caller left on a general-purpose decoder reopens the whole set of format parsers a crafted map
/// file could reach.
/// </summary>
public class SafeImagesTests
{
    /// <summary>
    /// A minimal but well-formed little-endian TIFF: header, then one IFD with width and length. No
    /// library here can write TIFF any more, which is the point, so the bytes are spelled out.
    /// </summary>
    private static byte[] TiffBytes()
    {
        var bytes = new byte[8 + 2 + 2 * 12 + 4];
        bytes[0] = (byte)'I';
        bytes[1] = (byte)'I';
        bytes[2] = 42;                                      // magic, so the file announces itself
        bytes[3] = 0;
        BitConverter.GetBytes(8).CopyTo(bytes, 4);           // offset of the first IFD
        BitConverter.GetBytes((ushort)2).CopyTo(bytes, 8);   // IFD entry count
        // ImageWidth (256), type SHORT (3), count 1, value 4
        BitConverter.GetBytes((ushort)256).CopyTo(bytes, 10);
        BitConverter.GetBytes((ushort)3).CopyTo(bytes, 12);
        BitConverter.GetBytes(1u).CopyTo(bytes, 14);
        BitConverter.GetBytes((ushort)4).CopyTo(bytes, 18);
        // ImageLength (257), type SHORT (3), count 1, value 4
        BitConverter.GetBytes((ushort)257).CopyTo(bytes, 22);
        BitConverter.GetBytes((ushort)3).CopyTo(bytes, 24);
        BitConverter.GetBytes(1u).CopyTo(bytes, 26);
        BitConverter.GetBytes((ushort)4).CopyTo(bytes, 30);
        return bytes;
    }

    private static byte[] PngBytes()
    {
        using var img = SafeImages.NewBitmap(4, 4);
        img.SetPixel(0, 0, new SKColor(255, 0, 0, 255));
        return SafeImages.EncodePng(img);
    }

    [Fact]
    public void A_png_still_decodes_so_the_restricted_set_is_not_too_narrow()
    {
        using var img = SafeImages.Load(PngBytes());
        Assert.Equal(4, img.Width);
        Assert.Equal(4, img.Height);
        Assert.Equal(new SKColor(255, 0, 0, 255), img.GetPixel(0, 0));
    }

    [Fact]
    public void Appearance_metric_refuses_a_tiff()
    {
        var png = PngBytes();
        Assert.ThrowsAny<Exception>(() => AppearanceMetric.Compare(TiffBytes(), png));
    }

    [Fact]
    public void Heightmap_decode_refuses_a_tiff()
    {
        Assert.ThrowsAny<Exception>(() => HeightmapPng.Decode(TiffBytes()));
    }

    [Fact]
    public void Pathing_decode_refuses_a_tiff_and_says_so()
    {
        bool ok = PathingPng.TryDecodeRegion(TiffBytes(), 2, 2, out _, out var error);
        Assert.False(ok);
        Assert.Contains("could not decode", error);
    }
}
