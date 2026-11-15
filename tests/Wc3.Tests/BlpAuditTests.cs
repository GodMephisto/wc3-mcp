// tests/Wc3.Tests/BlpAuditTests.cs
// The header rules the texture audit rests on. Each case builds the smallest file that exercises
// one rule, so a change to an offset or a bound fails here rather than in a report about a map.
using Wc3.Modeling;

namespace Wc3.Tests;

public class BlpAuditTests
{
    private const string Bs = "\\";

    /// <summary>A palettized BLP1, the layout this map's textures actually use.</summary>
    private static byte[] Palettized(int width, int height, int levels, int declaredLength)
    {
        var b = new byte[declaredLength];
        "BLP1"u8.CopyTo(b);
        BitConverter.GetBytes(0u).CopyTo(b, 4);   // content type 0 = palettized
        BitConverter.GetBytes(8u).CopyTo(b, 8);   // alpha bits
        BitConverter.GetBytes((uint)width).CopyTo(b, 12);
        BitConverter.GetBytes((uint)height).CopyTo(b, 16);
        BitConverter.GetBytes(3u).CopyTo(b, 20);  // picture type
        BitConverter.GetBytes(1u).CopyTo(b, 24);  // picture sub type
        int at = 156;
        int w = width, h = height;
        for (int i = 0; i < levels; i++)
        {
            int size = Math.Max(1, w * h / 4);
            if (at + size > b.Length) break;
            BitConverter.GetBytes((uint)at).CopyTo(b, 28 + i * 4);
            BitConverter.GetBytes((uint)size).CopyTo(b, 92 + i * 4);
            at += size;
            w = Math.Max(1, w / 2);
            h = Math.Max(1, h / 2);
        }
        return b;
    }

    [Fact]
    public void A_palettized_header_with_a_contiguous_chain_is_clean()
    {
        var b = Palettized(64, 64, 7, 4000);
        Assert.Null(BlpAudit.Inspect(b, b.Length, out _, out _, out var kind));
        Assert.Equal(BlpProblem.None, kind);
    }

    [Fact]
    public void A_chain_that_runs_past_the_end_is_reported()
    {
        var b = Palettized(64, 64, 7, 4000);
        var cut = b.AsSpan(0, 1200).ToArray();  // inside mip 1, which ends at 1436
        var problem = BlpAudit.Inspect(cut, cut.Length, out _, out _, out var kind);
        Assert.Equal(BlpProblem.MipPastEnd, kind);
        Assert.Contains("past the end", problem);
    }

    [Fact]
    public void A_file_without_a_signature_is_reported()
    {
        var b = "not a texture at all"u8.ToArray();
        Assert.Equal(BlpProblem.NotBlp, _kind(BlpAudit.Inspect(b, b.Length, out _, out _, out var k), k));
    }

    [Fact]
    public void A_file_shorter_than_a_header_is_reported()
    {
        var b = "BLP1"u8.ToArray();
        Assert.Equal(BlpProblem.TruncatedHeader, _kind(BlpAudit.Inspect(b, b.Length, out _, out _, out var k), k));
    }

    [Fact]
    public void A_jpeg_header_above_the_engine_limit_is_reported()
    {
        var b = new byte[2000];
        "BLP1"u8.CopyTo(b);
        BitConverter.GetBytes(1u).CopyTo(b, 4);   // content type 1 = JPEG
        BitConverter.GetBytes(1024u).CopyTo(b, 8); // declared header, above 624
        Assert.Equal(BlpProblem.JpegHeaderTooLarge,
            _kind(BlpAudit.Inspect(b, b.Length, out _, out _, out var k), k));
    }

    [Fact]
    public void A_blp2_with_no_dimensions_is_reported()
    {
        var b = new byte[200];
        "BLP2"u8.CopyTo(b);
        BitConverter.GetBytes(1u).CopyTo(b, 4);
        Assert.Equal(BlpProblem.BadDimensions,
            _kind(BlpAudit.Inspect(b, b.Length, out _, out _, out var k), k));
    }

    [Fact]
    public void A_blp2_that_is_not_a_power_of_two_is_reported()
    {
        var b = new byte[400];
        "BLP2"u8.CopyTo(b);
        BitConverter.GetBytes(1u).CopyTo(b, 4);
        b[8] = 1; b[9] = 8; b[10] = 1; b[11] = 1;
        BitConverter.GetBytes(100u).CopyTo(b, 12);
        BitConverter.GetBytes(64u).CopyTo(b, 16);
        BitConverter.GetBytes((uint)200).CopyTo(b, 20);
        BitConverter.GetBytes((uint)150).CopyTo(b, 84);
        Assert.Equal(BlpProblem.NotPowerOfTwo,
            _kind(BlpAudit.Inspect(b, b.Length, out _, out _, out var k), k));
    }

    private static BlpProblem _kind(string? problem, BlpProblem kind)
    {
        Assert.NotNull(problem);
        return kind;
    }
}
