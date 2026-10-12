// src/Wc3.Render/PathingPng.cs
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using War3Net.Build.Environment;

namespace Wc3.Render;

/// <summary>
/// Renders a war3map.wpm pathing grid to a colour-coded PNG, and decodes a colour-coded
/// PNG back to pathing flags.
///
/// <para>The pathing byte is encoded as a bit-plane palette so every flag combination is
/// representable and reversible. The low nibble of the flag byte becomes the red channel
/// and the high nibble the green channel, each stepped by 17 (15 * 17 = 255) so every bit
/// is visible. The <c>none</c> cell is black, Walk is dark red, Blight is dark green, and
/// a cell with every flag set is bright yellow. A decoder refuses any colour the palette
/// does not produce, so a hand-painted image with a stray colour is rejected rather than
/// silently read as blank.</para>
///
/// <para>The map grid is one pixel per pathing cell in the top-left corner, in the map's
/// own storage order (row 0 is the south edge). A legend strip of swatches sits to the
/// right, one swatch per known flag, so a rendered image is self-describing. A decoder
/// reads only the top-left grid region, so a legend never has to be cropped before
/// import.</para>
/// </summary>
public static class PathingPng
{
    /// <summary>Channel step for one flag-byte nibble; 15 * 17 = 255 covers a full nibble.</summary>
    private const int Step = 17;

    /// <summary>Legend swatch edge, in pixels.</summary>
    private const int Swatch = 8;

    /// <summary>Gap between swatches and between the map grid and the legend, in pixels.</summary>
    private const int Gap = 2;

    /// <summary>Legend border, chosen so it is not a valid palette colour and reads as a frame.</summary>
    private static readonly Rgba32 BorderColour = new(255, 255, 255);

    /// <summary>The OR of every named flag, so a decoded byte must not set any other bit.</summary>
    public static readonly PathingType ValidFlags = ComputeValidFlags();

    private static PathingType ComputeValidFlags()
    {
        int mask = 0;
        foreach (var flag in Enum.GetValues<PathingType>())
            mask |= (int)flag;
        return (PathingType)mask;
    }

    /// <summary>One legend row of a flag set, its name, and the colour that encodes it.</summary>
    public sealed record PathingLegend(string Name, PathingType Flags, Rgba32 Colour);

    /// <summary>The legend shown in every rendered image, none first then each named flag.</summary>
    public static IReadOnlyList<PathingLegend> Legend { get; } = BuildLegend();

    private static PathingLegend[] BuildLegend()
    {
        var list = new List<PathingLegend> { new("none", default, EncodeColor(default)) };
        foreach (var flag in Enum.GetValues<PathingType>())
            list.Add(new(flag.ToString(), flag, EncodeColor(flag)));
        return list.ToArray();
    }

    /// <summary>A pathing flag set to its palette colour.</summary>
    public static Rgba32 EncodeColor(PathingType flags)
    {
        int b = (int)flags;
        return new Rgba32((byte)((b & 0x0F) * Step), (byte)(((b >> 4) & 0x0F) * Step), 0);
    }

    /// <summary>A palette colour back to its flag set, or false for any colour the palette refuses.</summary>
    public static bool TryDecodeColor(Rgba32 colour, out PathingType flags)
    {
        flags = default;
        if (colour.B != 0 || colour.R % Step != 0 || colour.G % Step != 0)
            return false;
        int b = (colour.R / Step) | ((colour.G / Step) << 4);
        if ((b & ~(int)ValidFlags) != 0)
            return false;
        flags = (PathingType)b;
        return true;
    }

    /// <summary>Renders the pathing grid plus its legend as PNG bytes.</summary>
    /// <exception cref="InvalidDataException">The pathing map has no consistent cell grid.</exception>
    public static byte[] Encode(MapPathingMap map)
    {
        ArgumentNullException.ThrowIfNull(map);
        int w = (int)map.Width, h = (int)map.Height;
        var cells = map.Cells;
        if (cells is null || cells.Count != w * h)
            throw new InvalidDataException(
                $"pathing map cell buffer inconsistent (Width={w} Height={h} Cells={cells?.Count ?? 0})");

        var legend = Legend;
        int legendHeight = legend.Count == 0 ? 0 : legend.Count * (Swatch + Gap) - Gap;
        int imageWidth = w + Gap + Swatch;
        int imageHeight = Math.Max(h, legendHeight);

        using var image = new Image<Rgba32>(imageWidth, imageHeight);
        for (int y = 0; y < imageHeight; y++)
            for (int x = 0; x < imageWidth; x++)
                image[x, y] = new Rgba32(0, 0, 0, 255);

        for (int y = 0; y < h; y++)
        {
            int rowBase = y * w;
            for (int x = 0; x < w; x++)
                image[x, y] = EncodeColor(cells[rowBase + x]);
        }

        int legendX = w + Gap;
        for (int i = 0; i < legend.Count; i++)
        {
            int legendY = i * (Swatch + Gap);
            var colour = legend[i].Colour;
            for (int dy = 0; dy < Swatch; dy++)
                for (int dx = 0; dx < Swatch; dx++)
                {
                    bool border = dx == 0 || dy == 0 || dx == Swatch - 1 || dy == Swatch - 1;
                    image[legendX + dx, legendY + dy] = border ? BorderColour : colour;
                }
        }

        using var ms = new MemoryStream();
        image.SaveAsPng(ms);
        return ms.ToArray();
    }

    /// <summary>
    /// Decodes the top-left <paramref name="regionWidth"/> by <paramref name="regionHeight"/>
    /// pixels of a PNG into pathing flags. Refuses an image smaller than the region and any
    /// pixel the palette does not produce. The legend to the right of the grid is ignored.
    /// </summary>
    public static bool TryDecodeRegion(
        byte[] png, int regionWidth, int regionHeight,
        out PathingType[] cells, out string error)
    {
        cells = Array.Empty<PathingType>();
        error = string.Empty;
        if (png is null || png.Length == 0)
        {
            error = "pathing image data is empty";
            return false;
        }

        Image<Rgba32> image;
        try { image = Image.Load<Rgba32>(png); }
        catch (Exception ex) { error = $"could not decode pathing PNG ({ex.Message})"; return false; }

        using (image)
        {
            if (image.Width < regionWidth || image.Height < regionHeight)
            {
                error = $"pathing image is {image.Width}x{image.Height}, smaller than the map's "
                    + $"{regionWidth}x{regionHeight} cell grid";
                return false;
            }

            var result = new PathingType[regionWidth * regionHeight];
            for (int y = 0; y < regionHeight; y++)
                for (int x = 0; x < regionWidth; x++)
                {
                    var colour = image[x, y];
                    if (!TryDecodeColor(colour, out var flags))
                    {
                        error = $"unknown pathing colour rgb({colour.R},{colour.G},{colour.B}) at ({x},{y})";
                        return false;
                    }
                    result[y * regionWidth + x] = flags;
                }

            cells = result;
            return true;
        }
    }
}
