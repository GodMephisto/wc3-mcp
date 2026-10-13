// tests/Wc3.Tests/TextureAtlasTests.cs
using Wc3.Commands;
using Wc3.Modeling;
using Wc3.Render;

namespace Wc3.Tests;

/// <summary>
/// The atlas packer and its command, proved on hermetic solid-colour PNGs. The load-bearing checks
/// are that the rectangles tile the atlas without overlapping and that each input's colour is the
/// one read back at its own rectangle, so a copy from the wrong offset or a wrong stride fails here
/// rather than in a renderer much later.
/// </summary>
public class TextureAtlasTests
{
    private static byte[] SolidPng(int w, int h, byte r, byte g, byte b)
    {
        var rgba = new byte[w * h * 4];
        for (int i = 0; i < w * h; i++)
        {
            rgba[i * 4] = r;
            rgba[i * 4 + 1] = g;
            rgba[i * 4 + 2] = b;
            rgba[i * 4 + 3] = 255;
        }
        return TexturePng.Encode(new TextureImage(w, h, rgba));
    }

    private static TextureImage Decode(byte[] bytes, string ext) => TextureConvert.DecodeRgba(bytes, ext);

    private static void AssertPixel(TextureImage tex, int x, int y, byte r, byte g, byte b)
    {
        int i = (y * tex.Width + x) * 4;
        Assert.Equal((r, g, b), (tex.Rgba[i], tex.Rgba[i + 1], tex.Rgba[i + 2]));
    }

    private static bool Overlap(AtlasPlacement a, AtlasPlacement b) =>
        a.X < b.X + b.Width && b.X < a.X + a.Width
        && a.Y < b.Y + b.Height && b.Y < a.Y + a.Height;

    [Fact]
    public void Three_solid_images_pack_with_contained_non_overlapping_rectangles()
    {
        var inputs = new List<AtlasInput>
        {
            new("red.png", 8, 8, Fill(8, 8, 255, 0, 0)),
            new("green.png", 16, 16, Fill(16, 16, 0, 255, 0)),
            new("blue.png", 8, 8, Fill(8, 8, 0, 0, 255)),
        };

        var pack = AtlasPacker.Pack(inputs, 512);

        Assert.Equal(pack.Width, pack.Height);
        Assert.True((pack.Width & (pack.Width - 1)) == 0, $"atlas side {pack.Width} is not a power of two");
        Assert.Equal(inputs.Count, pack.Placements.Count);

        // Every rectangle lies wholly inside the atlas.
        foreach (var p in pack.Placements)
        {
            Assert.True(p.X >= 0 && p.Y >= 0, $"{p.Name} starts outside the atlas at ({p.X}, {p.Y})");
            Assert.True(p.X + p.Width <= pack.Width && p.Y + p.Height <= pack.Height,
                $"{p.Name} runs past the {pack.Width}x{pack.Height} atlas");
        }

        // No two rectangles overlap.
        for (int i = 0; i < pack.Placements.Count; i++)
            for (int j = i + 1; j < pack.Placements.Count; j++)
                Assert.False(Overlap(pack.Placements[i], pack.Placements[j]),
                    $"{pack.Placements[i].Name} overlaps {pack.Placements[j].Name}");

        // Each input's colour survives at the centre of its own rectangle.
        var atlas = Decode(TextureConvert.EncodeRgba(new TextureImage(pack.Width, pack.Height, pack.Rgba), ".png"), ".png");
        var expected = new Dictionary<string, (byte R, byte G, byte B)>
        {
            ["red.png"] = (255, 0, 0),
            ["green.png"] = (0, 255, 0),
            ["blue.png"] = (0, 0, 255),
        };
        foreach (var p in pack.Placements)
        {
            var (r, g, b) = expected[p.Name];
            AssertPixel(atlas, p.X + p.Width / 2, p.Y + p.Height / 2, r, g, b);
        }
    }

    [Fact]
    public void Packing_the_same_inputs_twice_is_deterministic()
    {
        var inputs = new List<AtlasInput>
        {
            new("a", 5, 9, Fill(5, 9, 1, 2, 3)),
            new("b", 12, 4, Fill(12, 4, 4, 5, 6)),
            new("c", 7, 7, Fill(7, 7, 7, 8, 9)),
        };

        var first = AtlasPacker.Pack(inputs, 512);
        var second = AtlasPacker.Pack(inputs, 512);

        Assert.Equal(first.Width, second.Width);
        Assert.Equal(first.Rgba, second.Rgba);
        Assert.Equal(first.Placements, second.Placements);
    }

    [Fact]
    public void An_image_larger_than_the_maximum_is_refused()
    {
        var inputs = new List<AtlasInput> { new("big", 64, 64, Fill(64, 64, 0, 0, 0)) };
        Assert.Throws<InvalidOperationException>(() => AtlasPacker.Pack(inputs, 32));
    }

    [Fact]
    public void The_command_writes_an_atlas_in_the_first_input_format_and_reports_rectangles()
    {
        var dir = Path.Combine(Path.GetTempPath(), "wc3atlas", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            string a = Path.Combine(dir, "a.png");
            string b = Path.Combine(dir, "b.png");
            string c = Path.Combine(dir, "c.png");
            File.WriteAllBytes(a, SolidPng(8, 8, 255, 0, 0));
            File.WriteAllBytes(b, SolidPng(16, 16, 0, 255, 0));
            File.WriteAllBytes(c, SolidPng(8, 8, 0, 0, 255));
            string outPath = Path.Combine(dir, "atlas.png");

            var r = TextureAtlasCommand.ExecutePaths(new[] { a, b, c }, 512, outPath);

            Assert.True(File.Exists(outPath));
            Assert.Equal("png", r.Format);
            Assert.Equal(3, r.Entries.Count);
            Assert.True((r.AtlasWidth & (r.AtlasWidth - 1)) == 0);
            Assert.Equal(r.AtlasWidth, r.AtlasHeight);

            var atlas = Decode(File.ReadAllBytes(outPath), ".png");
            Assert.Equal(r.AtlasWidth, atlas.Width);
            foreach (var e in r.Entries)
                Assert.True(e.X + e.Width <= r.AtlasWidth && e.Y + e.Height <= r.AtlasHeight,
                    $"{e.Name} runs past the atlas");

            Assert.Equal((255, 0, 0), ColourAt(atlas, r.Entries[0]));
            Assert.Equal((0, 255, 0), ColourAt(atlas, r.Entries[1]));
            Assert.Equal((0, 0, 255), ColourAt(atlas, r.Entries[2]));
        }
        finally
        {
            try { Directory.Delete(dir, recursive: true); } catch (IOException) { }
        }
    }

    [Fact]
    public void The_command_refuses_a_single_input()
    {
        Assert.Throws<ArgumentException>(
            () => TextureAtlasCommand.ExecutePaths(new[] { "only.png" }, 512, "out.png"));
    }

    private static (byte R, byte G, byte B) ColourAt(TextureImage atlas, TextureAtlasEntry e)
    {
        int i = ((e.Y + e.Height / 2) * atlas.Width + (e.X + e.Width / 2)) * 4;
        return (atlas.Rgba[i], atlas.Rgba[i + 1], atlas.Rgba[i + 2]);
    }

    private static byte[] Fill(int w, int h, byte r, byte g, byte b)
    {
        var rgba = new byte[w * h * 4];
        for (int i = 0; i < w * h; i++)
        {
            rgba[i * 4] = r;
            rgba[i * 4 + 1] = g;
            rgba[i * 4 + 2] = b;
            rgba[i * 4 + 3] = 255;
        }
        return rgba;
    }
}
