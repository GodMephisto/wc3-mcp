// tests/Wc3.Tests/SafeImagesTests.cs
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.PixelFormats;
using Wc3.Render;

namespace Wc3.Tests;

/// <summary>
/// Every decode in Wc3.Render goes through <see cref="SafeImages"/>, so the advisories against
/// ImageSharp 3.1.12's TIFF and ICC code are unreachable from a crafted map file. One test per
/// entry point, because a single caller left on the default configuration reopens the whole set.
/// </summary>
public class SafeImagesTests
{
    private static byte[] TiffBytes()
    {
        using var img = new Image<Rgba32>(4, 4);
        using var ms = new MemoryStream();
        img.Save(ms, new TiffEncoder());
        return ms.ToArray();
    }

    private static byte[] PngBytes()
    {
        using var img = new Image<Rgba32>(4, 4);
        img[0, 0] = new Rgba32(255, 0, 0, 255);
        using var ms = new MemoryStream();
        img.SaveAsPng(ms);
        return ms.ToArray();
    }

    [Fact]
    public void A_png_still_decodes_so_the_restricted_set_is_not_too_narrow()
    {
        using var img = SafeImages.Load<Rgba32>(PngBytes());
        Assert.Equal(4, img.Width);
        Assert.Equal(new Rgba32(255, 0, 0, 255), img[0, 0]);
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
