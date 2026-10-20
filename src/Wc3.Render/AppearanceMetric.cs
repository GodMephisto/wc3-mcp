// src/Wc3.Render/AppearanceMetric.cs
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Wc3.Render;

/// <summary>
/// How close two renders of the same subject are. <see cref="Ssim"/> is luminance similarity
/// over the pixels either image covers, <see cref="SilhouetteIou"/> is the overlap of their
/// opaque masks, and the two coverage numbers say how much of the frame each filled, so a
/// model that shrank out of view cannot score well by being empty.
/// </summary>
public sealed record AppearanceScore(
    double Ssim, double SilhouetteIou, double CoverageA, double CoverageB);

/// <summary>
/// A small perceptual gate: global SSIM and silhouette intersection-over-union between two
/// PNG renders. Both are cheap, both are deterministic, and together they catch the two ways
/// a lossy model step goes wrong, shading that drifts and geometry that shrinks or tears.
/// Computed only over the union of the two masks so an empty background does not flatter the
/// score.
/// </summary>
public static class AppearanceMetric
{
    /// <summary>An alpha value above this counts as covered. The rasterizer writes 0 or 255.</summary>
    public const byte CoverageThreshold = 8;

    public static AppearanceScore Compare(byte[] pngA, byte[] pngB)
    {
        using var a = Image.Load<Rgba32>(pngA);
        using var b = Image.Load<Rgba32>(pngB);
        return Compare(a, b);
    }

    public static AppearanceScore Compare(Image<Rgba32> a, Image<Rgba32> b)
    {
        if (a.Width != b.Width || a.Height != b.Height)
        {
            throw new InvalidDataException(
                $"images differ in size, {a.Width}x{a.Height} against {b.Width}x{b.Height}");
        }

        int w = a.Width, h = a.Height;
        var la = new List<double>(w * h);
        var lb = new List<double>(w * h);
        long both = 0, either = 0, coveredA = 0, coveredB = 0;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                var pa = a[x, y];
                var pb = b[x, y];
                bool sa = pa.A > CoverageThreshold;
                bool sb = pb.A > CoverageThreshold;
                if (sa) coveredA++;
                if (sb) coveredB++;
                if (sa || sb) either++;
                if (!sa && !sb) continue;
                if (sa && sb) both++;
                la.Add(Luma(pa));
                lb.Add(Luma(pb));
            }
        }

        double iou = either == 0 ? 1 : (double)both / either;
        double ssim = Ssim(la, lb);
        double frame = (double)w * h;
        return new AppearanceScore(ssim, iou, coveredA / frame, coveredB / frame);
    }

    private static double Luma(Rgba32 p) => 0.299 * p.R + 0.587 * p.G + 0.114 * p.B;

    private static double Ssim(IReadOnlyList<double> a, IReadOnlyList<double> b)
    {
        int n = a.Count;
        if (n == 0) return 1;
        if (n != b.Count) throw new InvalidDataException("sample sets differ in length");

        double meanA = 0, meanB = 0;
        for (int i = 0; i < n; i++) { meanA += a[i]; meanB += b[i]; }
        meanA /= n;
        meanB /= n;

        double varA = 0, varB = 0, cov = 0;
        for (int i = 0; i < n; i++)
        {
            double da = a[i] - meanA;
            double db = b[i] - meanB;
            varA += da * da;
            varB += db * db;
            cov += da * db;
        }
        varA /= n;
        varB /= n;
        cov /= n;

        // 0..255 luma, so the standard dynamic range constants scale by 255 squared.
        const double range = 255.0;
        double c1 = 0.01 * range * 0.01 * range;
        double c2 = 0.03 * range * 0.03 * range;
        return ((2 * meanA * meanB + c1) * (2 * cov + c2))
            / ((meanA * meanA + meanB * meanB + c1) * (varA + varB + c2));
    }
}
