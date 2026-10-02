// tests/Wc3.Tests/TextureConvertTests.cs
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Wc3.Modeling;
using Wc3.Render;

namespace Wc3.Tests;

/// <summary>
/// Image conversion coverage, including the BLP channel-order proof: BLP1 JPEG
/// content stores BGRA, our encoder swaps RGBA→BGRA on write and BlpDecoder
/// swaps back on read, so colors survive a full round-trip (JPEG-lossy, hence
/// tolerance). A channel swap bug would show as a ~255 error on the red/blue
/// quadrants — far outside tolerance.
/// </summary>
public class TextureConvertTests
{
    private const int Tolerance = 20; // JPEG quality 90; quadrant centers stay well inside this

    /// <summary>RGBA quadrants: TL=red, TR=green, BL=blue, BR=white.</summary>
    private static byte[] QuadrantRgba(int w, int h)
    {
        var rgba = new byte[w * h * 4];
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                int i = (y * w + x) * 4;
                bool right = x >= w / 2, bottom = y >= h / 2;
                (rgba[i], rgba[i + 1], rgba[i + 2]) = (right, bottom) switch
                {
                    (false, false) => ((byte)255, (byte)0, (byte)0),
                    (true, false) => ((byte)0, (byte)255, (byte)0),
                    (false, true) => ((byte)0, (byte)0, (byte)255),
                    (true, true) => ((byte)255, (byte)255, (byte)255),
                };
                rgba[i + 3] = 255;
            }
        return rgba;
    }

    private static byte[] QuadrantPng(int w, int h) =>
        TexturePng.Encode(new TextureImage(w, h, QuadrantRgba(w, h)));

    private static void AssertPixelNear(TextureImage tex, int x, int y, byte r, byte g, byte b, int tolerance)
    {
        int i = (y * tex.Width + x) * 4;
        Assert.True(
            Math.Abs(tex.Rgba[i] - r) <= tolerance
            && Math.Abs(tex.Rgba[i + 1] - g) <= tolerance
            && Math.Abs(tex.Rgba[i + 2] - b) <= tolerance,
            $"pixel ({x},{y}) = ({tex.Rgba[i]},{tex.Rgba[i + 1]},{tex.Rgba[i + 2]}), expected ~({r},{g},{b})");
    }

    [Fact]
    public void Png_to_blp_roundtrips_colors_through_our_decoder()
    {
        const int w = 32, h = 32;
        var blp = TextureConvert.Convert(QuadrantPng(w, h), ".png", ".blp");

        var tex = BlpDecoder.Decode(blp);
        Assert.Equal(w, tex.Width);
        Assert.Equal(h, tex.Height);

        // Quadrant centers, away from JPEG block edges.
        AssertPixelNear(tex, 8, 8, 255, 0, 0, Tolerance);    // red — a R/B swap would read ~(0,0,255)
        AssertPixelNear(tex, 24, 8, 0, 255, 0, Tolerance);   // green
        AssertPixelNear(tex, 8, 24, 0, 0, 255, Tolerance);   // blue
        AssertPixelNear(tex, 24, 24, 255, 255, 255, Tolerance); // white
    }

    [Fact]
    public void Png_to_blp_produces_blp1_magic_and_decodable_file()
    {
        var blp = TextureConvert.Convert(QuadrantPng(16, 16), ".png", ".blp");
        Assert.Equal("BLP1"u8.ToArray(), blp[..4]);
        var tex = BlpDecoder.Decode(blp);
        Assert.Equal(16, tex.Width);
        Assert.Equal(16, tex.Height);
    }

    [Fact]
    public void Blp_to_png_produces_png_magic_with_same_dimensions()
    {
        var blp = TextureConvert.Convert(QuadrantPng(16, 16), ".png", ".blp");
        var png = TextureConvert.Convert(blp, ".blp", ".png");
        Assert.Equal(new byte[] { 0x89, 0x50, 0x4E, 0x47 }, png[..4]);
        using var image = Image.Load<Rgba32>(png);
        Assert.Equal(16, image.Width);
        Assert.Equal(16, image.Height);
    }

    [Fact]
    public void Lossless_formats_roundtrip_exactly()
    {
        const int w = 16, h = 16;
        var png = QuadrantPng(w, h);

        // png → bmp → tga → png stays pixel-identical (no JPEG involved).
        var bmp = TextureConvert.Convert(png, ".png", ".bmp");
        var tga = TextureConvert.Convert(bmp, ".bmp", ".tga");
        var back = TextureConvert.Convert(tga, ".tga", ".png");

        using var a = Image.Load<Rgba32>(png);
        using var b = Image.Load<Rgba32>(back);
        Assert.Equal(a.Width, b.Width);
        Assert.Equal(a.Height, b.Height);
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                Assert.Equal(a[x, y], b[x, y]);
    }

    [Fact]
    public void Jpeg_target_produces_jfif_bytes()
    {
        var jpg = TextureConvert.Convert(QuadrantPng(16, 16), ".png", ".jpg");
        Assert.Equal(0xFF, jpg[0]);
        Assert.Equal(0xD8, jpg[1]); // SOI marker
    }

    [Fact]
    public void Non_power_of_two_dimensions_survive_blp_roundtrip()
    {
        var rgba = QuadrantRgba(20, 13);
        var png = TexturePng.Encode(new TextureImage(20, 13, rgba));
        var blp = TextureConvert.Convert(png, ".png", ".blp");
        var tex = BlpDecoder.Decode(blp);
        Assert.Equal(20, tex.Width);
        Assert.Equal(13, tex.Height);
    }

    [Fact]
    public void Unknown_target_extension_is_rejected_with_a_clear_message()
    {
        var ex = Assert.Throws<NotSupportedException>(
            () => TextureConvert.Convert(QuadrantPng(4, 4), ".png", ".webp"));
        Assert.Contains(".webp", ex.Message);
        Assert.Contains(".blp", ex.Message);
    }

    [Fact]
    public void Garbage_source_bytes_are_rejected_with_a_clear_message()
    {
        Assert.Throws<NotSupportedException>(
            () => TextureConvert.Convert(new byte[] { 1, 2, 3, 4 }, ".xyz", ".png"));
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Real_jpeg_blp_decodes_with_true_rgba_channel_order()
    {
        // Ground truth for the channel order: a real map asset with skin tones.
        // Without the JPEG-content BGRA swap in BlpDecoder this face decodes blue.
        string path =
            System.IO.Path.Combine(System.IO.Path.GetTempPath(), @"wc3x_modelsamples\blp\war3mapImported\Emoji_KEKW.blp");
        if (!File.Exists(path)) return; // samples-optional

        var tex = BlpDecoder.Decode(File.ReadAllBytes(path));
        // Average the central region; flesh tones are strongly red-dominant.
        long r = 0, b = 0;
        int cx = tex.Width / 2, cy = tex.Height / 2, n = 0;
        for (int y = cy - 20; y < cy + 20; y++)
            for (int x = cx - 20; x < cx + 20; x++)
            {
                int i = (y * tex.Width + x) * 4;
                r += tex.Rgba[i];
                b += tex.Rgba[i + 2];
                n++;
            }
        Assert.True(r > b + 30L * n, $"expected red-dominant skin tones, got avg R={r / n} B={b / n}");
    }
}
