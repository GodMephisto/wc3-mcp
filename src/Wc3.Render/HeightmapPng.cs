// src/Wc3.Render/HeightmapPng.cs
using War3Net.Build.Environment;

namespace Wc3.Render;

/// <summary>
/// Encodes a terrain height grid to a 16-bit grayscale PNG and decodes one back.
///
/// <para>One pixel per terrain corner in the map's own storage order. The grid is
/// (Width+1) by (Height+1) corners and row 0 is the south edge, so a decode writes the
/// same byte layout the renderer and the brush commands read. There is no flip, which
/// keeps export then import an exact inverse.</para>
///
/// <para>A tile's <see cref="TerrainTile.Height"/> is a float. It is quantised by a
/// fixed scale of <see cref="HeightScale"/> (128, one cliff step in world units) around
/// a midpoint of 32768, so the 16-bit range covers ground from -256 to +255.99 height
/// units at 1/128 resolution. Export then import reproduces a height to within half a
/// quantisation step (1/256), and a height outside the representable band clamps rather
/// than wrapping.</para>
///
/// <para>The 16-bit sample is carried exactly, which is why neither direction uses a
/// general purpose codec for a grayscale image. A decoder that routes 16 bits through an
/// 8-bit or half-float intermediate would move a height by up to half a world unit.</para>
/// </summary>
public static class HeightmapPng
{
    /// <summary>World units per stored height unit, and the quantisation step.</summary>
    public const float HeightScale = 128f;

    /// <summary>The encoded value that stands for a height of zero.</summary>
    public const ushort Midpoint = 32768;

    /// <summary>Corners per image row, i.e. the terrain grid width.</summary>
    public static int GridWidth(MapEnvironment env) => (int)env.Width + 1;

    /// <summary>Corners per image column, i.e. the terrain grid height.</summary>
    public static int GridHeight(MapEnvironment env) => (int)env.Height + 1;

    /// <summary>A raw tile height to its 16-bit grayscale sample.</summary>
    public static ushort Quantise(float height) =>
        (ushort)Math.Clamp(MathF.Round(height * HeightScale) + Midpoint, 0f, ushort.MaxValue);

    /// <summary>A 16-bit grayscale sample back to a raw tile height.</summary>
    public static float Dequantise(ushort value) => (value - Midpoint) / HeightScale;

    /// <summary>Encodes the terrain grid as 16-bit grayscale PNG bytes.</summary>
    /// <exception cref="InvalidDataException">The environment has no consistent tile grid.</exception>
    public static byte[] Encode(MapEnvironment env)
    {
        ArgumentNullException.ThrowIfNull(env);
        int w = GridWidth(env), h = GridHeight(env);
        var tiles = env.TerrainTiles;
        if (tiles is null || tiles.Count != w * h)
            throw new InvalidDataException(
                $"terrain tile buffer inconsistent (Width+1={w} Height+1={h} Tiles={tiles?.Count ?? 0})");

        var samples = new ushort[w * h];
        for (int y = 0; y < h; y++)
        {
            int rowBase = y * w;
            for (int x = 0; x < w; x++)
                samples[rowBase + x] = Quantise(tiles[rowBase + x].Height);
        }

        return SafeImages.EncodeGray16Png(w, h, samples);
    }

    /// <summary>Decodes any PNG into its luminance samples, whether 8 or 16 bit.</summary>
    public static HeightmapImage Decode(byte[] png)
    {
        ArgumentNullException.ThrowIfNull(png);

        // A grayscale PNG is read sample for sample, so a 16-bit heightmap survives untouched.
        if (SafeImages.TryDecodeGray16Png(png, out int exactWidth, out int exactHeight, out var exact))
            return new HeightmapImage(exactWidth, exactHeight, exact);

        // Anything else (a colour PNG, an interlaced one) goes through the general decoder and
        // loses the low bits, the same way the old luminance conversion did.
        using var image = SafeImages.Load(png);
        int w = image.Width, h = image.Height;
        var pixels = new ushort[w * h];
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                var pixel = image.GetPixel(x, y);
                double luma = 0.299 * pixel.Red + 0.587 * pixel.Green + 0.114 * pixel.Blue;
                pixels[y * w + x] = (ushort)Math.Clamp(Math.Round(luma) * 257, 0, 65535);
            }
        return new HeightmapImage(w, h, pixels);
    }
}

/// <summary>A decoded grayscale grid of dimensions and one 16-bit sample per pixel.</summary>
public sealed record HeightmapImage(int Width, int Height, ushort[] Pixels);
