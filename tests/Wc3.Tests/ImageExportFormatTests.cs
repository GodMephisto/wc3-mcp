// tests/Wc3.Tests/ImageExportFormatTests.cs
using Wc3.Commands;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// The Files panel gated image export on ".blp" and then passed ".blp" to the converter as the
/// SOURCE format regardless of what the entry actually was. So a .dds entry fell through to the
/// raw export and handed back undecoded bytes, while the preview pane beside it rendered the very
/// same file, because FilePreviewCommand decodes DDS and the export gate did not.
///
/// TextureConvert has accepted .blp, .dds, .png, .jpg, .jpeg, .bmp, .tga and .gif as sources the
/// whole time. The panel was the only thing that did not know.
///
/// These pin the command-layer half, that the converter handles the formats the panel now offers
/// and that naming the wrong source format is a real error rather than something that silently
/// half-works. The panel half is pinned in Wc3.Studio.Tests.
/// </summary>
public class ImageExportFormatTests
{
    private readonly ITestOutputHelper _out;
    public ImageExportFormatTests(ITestOutputHelper output) => _out = output;

    /// <summary>A minimal but real DDS: the magic, a 124 byte header declaring an uncompressed
    /// 1x1 32-bit BGRA surface, and one pixel.</summary>
    private static byte[] OnePixelDds()
    {
        var b = new byte[128 + 4];
        System.Text.Encoding.ASCII.GetBytes("DDS ").CopyTo(b, 0);
        BitConverter.GetBytes(124).CopyTo(b, 4);              // header size
        BitConverter.GetBytes(0x0000100F).CopyTo(b, 8);       // caps|height|width|pitch|pixelformat
        BitConverter.GetBytes(1).CopyTo(b, 12);               // height
        BitConverter.GetBytes(1).CopyTo(b, 16);               // width
        BitConverter.GetBytes(4).CopyTo(b, 20);               // pitch
        BitConverter.GetBytes(32).CopyTo(b, 76);              // pixel format size
        BitConverter.GetBytes(0x41).CopyTo(b, 80);            // RGB | ALPHAPIXELS
        BitConverter.GetBytes(32).CopyTo(b, 88);              // bit count
        BitConverter.GetBytes(0x00FF0000u).CopyTo(b, 92);     // R mask
        BitConverter.GetBytes(0x0000FF00u).CopyTo(b, 96);     // G mask
        BitConverter.GetBytes(0x000000FFu).CopyTo(b, 100);    // B mask
        BitConverter.GetBytes(0xFF000000u).CopyTo(b, 104);    // A mask
        BitConverter.GetBytes(0x1000).CopyTo(b, 108);         // caps TEXTURE
        b[128] = 0x20; b[129] = 0x40; b[130] = 0x60; b[131] = 0xFF;   // one BGRA pixel
        return b;
    }

    [Fact]
    public void A_dds_source_converts_to_png_rather_than_being_handed_back_raw()
    {
        byte[] png = ConvertCommand.ConvertImage(OnePixelDds(), ".dds", ".png");
        _out.WriteLine($"{png.Length} bytes out");

        // A real PNG signature, so this is a decode rather than a passthrough.
        Assert.True(png.Length > 8);
        Assert.Equal(new byte[] { 0x89, 0x50, 0x4E, 0x47 }, png.Take(4).ToArray());
    }

    [Fact]
    public void Naming_the_wrong_source_format_fails_loudly()
    {
        // The old panel passed ".blp" for every entry it exported. Asking the converter to read
        // DDS bytes as BLP has to be an error rather than something that produces a plausible
        // wrong image, otherwise the bug would have been invisible.
        var ex = Record.Exception(() => ConvertCommand.ConvertImage(OnePixelDds(), ".blp", ".png"));
        Assert.NotNull(ex);
        _out.WriteLine($"{ex!.GetType().Name}: {ex.Message}");
    }

    [Theory]
    [InlineData(".png")]
    [InlineData(".tga")]
    [InlineData(".bmp")]
    public void Every_target_format_the_panel_offers_actually_works(string toExt)
    {
        byte[] outBytes = ConvertCommand.ConvertImage(OnePixelDds(), ".dds", toExt);
        Assert.NotEmpty(outBytes);
        _out.WriteLine($"{toExt} -> {outBytes.Length} bytes");
    }
}
