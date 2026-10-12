// src/Wc3.Render/HeightmapPng.cs
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
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

        using var image = new Image<L16>(w, h);
        for (int y = 0; y < h; y++)
        {
            int rowBase = y * w;
            for (int x = 0; x < w; x++)
                image[x, y] = new L16(Quantise(tiles[rowBase + x].Height));
        }

        using var ms = new MemoryStream();
        image.SaveAsPng(ms);
        return ms.ToArray();
    }

    /// <summary>Decodes any PNG into its luminance samples, whether 8 or 16 bit.</summary>
    public static HeightmapImage Decode(byte[] png)
    {
        ArgumentNullException.ThrowIfNull(png);
        using var image = Image.Load<L16>(png);
        int w = image.Width, h = image.Height;
        var pixels = new ushort[w * h];
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                pixels[y * w + x] = image[x, y].PackedValue;
        return new HeightmapImage(w, h, pixels);
    }
}

/// <summary>A decoded grayscale grid of dimensions and one 16-bit sample per pixel.</summary>
public sealed record HeightmapImage(int Width, int Height, ushort[] Pixels);
