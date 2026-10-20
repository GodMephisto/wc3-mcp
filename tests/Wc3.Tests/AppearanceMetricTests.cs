// tests/Wc3.Tests/AppearanceMetricTests.cs
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Wc3.Render;

namespace Wc3.Tests;

public sealed class AppearanceMetricTests
{
    [Fact]
    public void An_image_against_itself_is_a_perfect_match()
    {
        using var image = Solid(64, 64, new Rgba32(200, 100, 50, 255), 10, 10, 30, 30);
        var score = AppearanceMetric.Compare(image, image);
        Assert.Equal(1.0, score.Ssim, 6);
        Assert.Equal(1.0, score.SilhouetteIou, 6);
        Assert.True(score.CoverageA > 0);
    }

    [Fact]
    public void A_shifted_silhouette_lowers_the_overlap()
    {
        using var a = Solid(64, 64, new Rgba32(200, 100, 50, 255), 10, 10, 30, 30);
        using var b = Solid(64, 64, new Rgba32(200, 100, 50, 255), 30, 10, 30, 30);
        var score = AppearanceMetric.Compare(a, b);
        Assert.InRange(score.SilhouetteIou, 0.0001, 0.9999);
    }

    [Fact]
    public void A_different_colour_lowers_ssim_but_not_the_silhouette()
    {
        using var a = Solid(32, 32, new Rgba32(240, 240, 240, 255), 4, 4, 20, 20);
        using var b = Solid(32, 32, new Rgba32(20, 20, 20, 255), 4, 4, 20, 20);
        var score = AppearanceMetric.Compare(a, b);
        Assert.True(score.Ssim < 0.5);
        Assert.Equal(1.0, score.SilhouetteIou, 6);
    }

    [Fact]
    public void An_empty_frame_against_itself_is_a_match()
    {
        using var empty = new Image<Rgba32>(32, 32);
        var score = AppearanceMetric.Compare(empty, empty);
        Assert.Equal(1.0, score.SilhouetteIou, 6);
        Assert.Equal(0.0, score.CoverageA, 6);
    }

    private static Image<Rgba32> Solid(int w, int h, Rgba32 colour, int x0, int y0, int rw, int rh)
    {
        var image = new Image<Rgba32>(w, h);
        for (int y = y0; y < y0 + rh; y++)
        {
            for (int x = x0; x < x0 + rw; x++)
            {
                image[x, y] = colour;
            }
        }
        return image;
    }
}
