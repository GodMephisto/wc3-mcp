// tests/Wc3.Tests/DdsPreviewConvertTests.cs
using System.Buffers.Binary;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Wc3.Commands;
using Wc3.Model;
using Wc3.Render;

namespace Wc3.Tests;

/// <summary>
/// Hermetic coverage of the .dds wiring into file preview and image convert.
/// Reforged maps commonly import DDS textures, so a .dds map entry must preview
/// as a PNG image and convert to PNG exactly like .blp does. Uses a synthetic
/// solid-red DXT1 file (BC1 endpoint expansion is exact, no lossy step).
/// </summary>
public class DdsPreviewConvertTests
{
    /// <summary>A minimal valid 4x4 DXT1 .dds where every texel is opaque red.</summary>
    private static byte[] SolidRedDxt1()
    {
        var dds = new byte[128 + 8];
        Encoding.ASCII.GetBytes("DDS ").CopyTo(dds, 0);
        BinaryPrimitives.WriteInt32LittleEndian(dds.AsSpan(4), 124);   // header size
        BinaryPrimitives.WriteInt32LittleEndian(dds.AsSpan(12), 4);    // height
        BinaryPrimitives.WriteInt32LittleEndian(dds.AsSpan(16), 4);    // width
        BinaryPrimitives.WriteInt32LittleEndian(dds.AsSpan(76), 32);   // pixelformat size
        BinaryPrimitives.WriteUInt32LittleEndian(dds.AsSpan(80), 0x4); // DDPF_FOURCC
        Encoding.ASCII.GetBytes("DXT1").CopyTo(dds, 84);
        // One BC1 block: c0 = red 565 (0xF800) > c1 = 0 selects 4-color mode,
        // all indices 0 so every texel is c0.
        dds[128] = 0x00;
        dds[129] = 0xF8;
        return dds;
    }

    private static MapDocument Map() => MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
    {
        ["war3mapImported\\tex.dds"] = SolidRedDxt1(),
        ["war3mapImported\\broken.dds"] = new byte[] { (byte)'D', (byte)'D', (byte)'S', (byte)' ', 0, 1, 2, 255 },
    }));

    private static void AssertSolidRedPng(byte[] png)
    {
        Assert.Equal(new byte[] { 0x89, 0x50, 0x4E, 0x47 }, png[..4]);
        using var image = Image.Load<Rgba32>(png);
        Assert.Equal(4, image.Width);
        Assert.Equal(4, image.Height);
        Assert.Equal(new Rgba32(255, 0, 0, 255), image[0, 0]);
        Assert.Equal(new Rgba32(255, 0, 0, 255), image[3, 3]);
    }

    [Fact]
    public void Dds_file_previews_as_a_png_image()
    {
        var p = FilePreviewCommand.Execute(Map(), "war3mapImported\\tex.dds");
        Assert.Equal("image", p.Kind);
        Assert.Contains("4x4 DDS", p.Info);
        Assert.NotNull(p.Png);
        AssertSolidRedPng(p.Png!);
    }

    [Fact]
    public void Undecodable_dds_degrades_to_a_hex_dump_instead_of_throwing()
    {
        var p = FilePreviewCommand.Execute(Map(), "war3mapImported\\broken.dds");
        Assert.Equal("binary", p.Kind);
        Assert.Null(p.Png);
        Assert.NotNull(p.Text);
    }

    [Fact]
    public void Dds_bytes_convert_to_png()
    {
        var png = ConvertCommand.ConvertImage(SolidRedDxt1(), ".dds", ".png");
        AssertSolidRedPng(png);
    }

    [Fact]
    public void Map_internal_dds_converts_to_png()
    {
        var png = ConvertCommand.ConvertImageFile(Map(), "war3mapImported\\tex.dds", ".png");
        AssertSolidRedPng(png);
    }

    [Fact]
    public void Dds_bytes_convert_to_jpeg_too()
    {
        var jpg = TextureConvert.Convert(SolidRedDxt1(), ".dds", ".jpg");
        Assert.Equal(0xFF, jpg[0]);
        Assert.Equal(0xD8, jpg[1]); // SOI marker
    }
}
