// tests/Wc3.Tests/DdsDecoderTests.cs
using System.Buffers.Binary;
using System.Text;
using Wc3.Modeling;

namespace Wc3.Tests;

/// <summary>
/// Hermetic checks of the hand-rolled DDS decoder (Reforged repacks classic .blp
/// textures as DDS, so base-game model previews depend on it): BC1 palette math in
/// both modes, BC3 interpolated alpha, and container sniffing.
/// </summary>
public class DdsDecoderTests
{
    private const ushort Red565 = 0xF800;   // → (255, 0, 0)
    private const ushort Blue565 = 0x001F;  // → (0, 0, 255)

    private static byte[] Header(string fourCc, int width, int height)
    {
        var h = new byte[128];
        Encoding.ASCII.GetBytes("DDS ").CopyTo(h, 0);
        BinaryPrimitives.WriteInt32LittleEndian(h.AsSpan(4), 124);      // header size
        BinaryPrimitives.WriteInt32LittleEndian(h.AsSpan(12), height);
        BinaryPrimitives.WriteInt32LittleEndian(h.AsSpan(16), width);
        BinaryPrimitives.WriteInt32LittleEndian(h.AsSpan(76), 32);      // pixelformat size
        BinaryPrimitives.WriteUInt32LittleEndian(h.AsSpan(80), 0x4);    // DDPF_FOURCC
        Encoding.ASCII.GetBytes(fourCc).CopyTo(h, 84);
        return h;
    }

    private static (byte R, byte G, byte B, byte A) Texel(TextureImage img, int x, int y)
    {
        int i = (y * img.Width + x) * 4;
        return (img.Rgba[i], img.Rgba[i + 1], img.Rgba[i + 2], img.Rgba[i + 3]);
    }

    [Fact]
    public void Bc1_four_color_mode_decodes_endpoints_and_interpolants()
    {
        // One 4x4 block: c0 red > c1 blue → 4-color mode. Texels 0..3 use indices
        // 0,1,2,3 (packed 0b11100100 = 0xE4); the rest index 0.
        var dds = Header("DXT1", 4, 4).Concat(new byte[]
        {
            0x00, 0xF8,   // c0 = red
            0x1F, 0x00,   // c1 = blue
            0xE4, 0, 0, 0,
        }).ToArray();

        var img = DdsDecoder.Decode(dds);
        Assert.Equal((4, 4), (img.Width, img.Height));
        Assert.Equal(((byte)255, (byte)0, (byte)0, (byte)255), Texel(img, 0, 0));   // c0
        Assert.Equal(((byte)0, (byte)0, (byte)255, (byte)255), Texel(img, 1, 0));   // c1
        Assert.Equal(((byte)170, (byte)0, (byte)85, (byte)255), Texel(img, 2, 0));  // 2/3 c0
        Assert.Equal(((byte)85, (byte)0, (byte)170, (byte)255), Texel(img, 3, 0));  // 1/3 c0
        Assert.Equal(((byte)255, (byte)0, (byte)0, (byte)255), Texel(img, 3, 3));
    }

    [Fact]
    public void Bc1_three_color_mode_makes_index3_transparent()
    {
        // c0 <= c1 → 3-color + transparent mode; texel 0 uses index 3.
        var dds = Header("DXT1", 4, 4).Concat(new byte[]
        {
            0x1F, 0x00,   // c0 = blue (smaller)
            0x00, 0xF8,   // c1 = red
            0x03, 0, 0, 0,
        }).ToArray();

        var img = DdsDecoder.Decode(dds);
        Assert.Equal((byte)0, Texel(img, 0, 0).A);       // transparent black
        Assert.Equal((byte)255, Texel(img, 1, 0).A);     // the rest opaque
    }

    [Fact]
    public void Bc3_interpolates_alpha_and_decodes_color()
    {
        // Alpha block: a0=255 > a1=0 → 7-step interpolation; texel 0 index 0 (255),
        // texel 1 index 1 (0) → 3-bit indices 0b001_000 = 0x08. Color: all red.
        var dds = Header("DXT5", 4, 4).Concat(new byte[]
        {
            255, 0,                    // a0, a1
            0x08, 0, 0, 0, 0, 0,       // alpha indices
            0x00, 0xF8, 0x1F, 0x00,    // c0 red, c1 blue
            0, 0, 0, 0,                // all color index 0
        }).ToArray();

        var img = DdsDecoder.Decode(dds);
        Assert.Equal(((byte)255, (byte)0, (byte)0, (byte)255), Texel(img, 0, 0));
        Assert.Equal((byte)0, Texel(img, 1, 0).A);
        Assert.Equal((byte)255, Texel(img, 1, 0).R); // color plane independent of alpha
    }

    [Fact]
    public void Sniffs_dds_magic_and_rejects_non_dds()
    {
        Assert.True(DdsDecoder.LooksLikeDds(Header("DXT1", 4, 4)));
        Assert.False(DdsDecoder.LooksLikeDds(new byte[] { 1, 2, 3, 4, 5 }));
        Assert.Throws<InvalidDataException>(() => DdsDecoder.Decode(new byte[] { 1, 2, 3, 4 }));
    }
}
