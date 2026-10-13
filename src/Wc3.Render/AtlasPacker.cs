// src/Wc3.Render/AtlasPacker.cs
namespace Wc3.Render;

/// <summary>One image offered to the packer, already decoded to RGBA8. The name is carried
/// through untouched so the caller can map a rectangle back to the input it came from.</summary>
public sealed record AtlasInput(string Name, int Width, int Height, byte[] Rgba);

/// <summary>Where one input landed in the atlas, in pixels, with its original name.</summary>
public sealed record AtlasPlacement(string Name, int X, int Y, int Width, int Height);

/// <summary>The packed atlas as RGBA8 pixels plus each input's rectangle, in the order given.</summary>
public sealed record AtlasResult(int Width, int Height, byte[] Rgba, IReadOnlyList<AtlasPlacement> Placements);

/// <summary>
/// Packs several decoded RGBA images into one square power-of-two atlas with a shelf packer. The
/// inputs are sorted tallest first and placed left to right into rows, a new row starting when the
/// next image does not fit the remaining width. The atlas side is the smallest power of two at or
/// above the largest input dimension that fits every image within <c>maxSize</c>, so a pack either
/// succeeds at a named size or reports that it cannot fit, never silently crops.
///
/// The result is deterministic. Sorting is by height descending and then by the original index, so
/// the same inputs in the same order always produce the same rectangles, atlas size and bytes.
/// </summary>
public static class AtlasPacker
{
    /// <summary>
    /// Packs <paramref name="images"/> into a square power-of-two atlas no larger than
    /// <paramref name="maxSize"/> on a side. Throws when a single image exceeds the cap or the set
    /// cannot fit at the cap.
    /// </summary>
    public static AtlasResult Pack(IReadOnlyList<AtlasInput> images, int maxSize)
    {
        ArgumentNullException.ThrowIfNull(images);
        if (images.Count == 0) throw new ArgumentException("need at least one image to pack", nameof(images));
        if (maxSize <= 0) throw new ArgumentOutOfRangeException(nameof(maxSize));

        foreach (var img in images)
        {
            ArgumentNullException.ThrowIfNull(img);
            if (img.Width <= 0 || img.Height <= 0)
                throw new ArgumentException($"image '{img.Name}' has no area", nameof(images));
            if (img.Rgba is null || img.Rgba.Length != img.Width * img.Height * 4)
                throw new ArgumentException($"image '{img.Name}' does not carry {img.Width}x{img.Height} RGBA", nameof(images));
        }

        // Tallest first, then original order. The stable tie-break is what keeps the atlas
        // identical from run to run for the same inputs.
        var ordered = images
            .Select((img, index) => (img, index))
            .OrderByDescending(t => t.img.Height)
            .ThenByDescending(t => t.img.Width)
            .ThenBy(t => t.index)
            .ToList();

        int largest = ordered.Max(t => Math.Max(t.img.Width, t.img.Height));
        int side = NextPowerOfTwo(largest);
        while (side > maxSize || !Fits(ordered, side))
        {
            if (side > maxSize)
                throw new InvalidOperationException(
                    $"cannot fit {images.Count} image(s) in a power-of-two atlas of at most {maxSize} pixels "
                    + "on a side, raise the maximum or shrink an input");
            side <<= 1;
        }

        var placed = Place(ordered, side);
        var atlas = new byte[side * side * 4];
        var result = new AtlasPlacement[images.Count];
        for (int k = 0; k < ordered.Count; k++)
        {
            var (img, originalIndex) = ordered[k];
            Blit(img, placed[k], atlas, side);
            result[originalIndex] = placed[k] with { Name = img.Name };
        }
        return new AtlasResult(side, side, atlas, result);
    }

    /// <summary>True when the already-ordered images fit a square of the given side.</summary>
    private static bool Fits(IReadOnlyList<(AtlasInput img, int index)> ordered, int side)
    {
        int x = 0, y = 0, shelfHeight = 0;
        foreach (var (img, _) in ordered)
        {
            if (img.Width > side || img.Height > side) return false;
            if (x + img.Width > side)
            {
                y += shelfHeight;
                x = 0;
                shelfHeight = 0;
            }
            if (y + img.Height > side) return false;
            x += img.Width;
            shelfHeight = Math.Max(shelfHeight, img.Height);
        }
        return true;
    }

    /// <summary>The actual rectangles, laid out the same way <see cref="Fits"/> walks the images.</summary>
    private static List<AtlasPlacement> Place(IReadOnlyList<(AtlasInput img, int index)> ordered, int side)
    {
        var placements = new List<AtlasPlacement>(ordered.Count);
        int x = 0, y = 0, shelfHeight = 0;
        foreach (var (img, _) in ordered)
        {
            if (x + img.Width > side)
            {
                y += shelfHeight;
                x = 0;
                shelfHeight = 0;
            }
            placements.Add(new AtlasPlacement(img.Name, x, y, img.Width, img.Height));
            x += img.Width;
            shelfHeight = Math.Max(shelfHeight, img.Height);
        }
        return placements;
    }

    private static void Blit(AtlasInput img, AtlasPlacement at, byte[] atlas, int side)
    {
        for (int row = 0; row < img.Height; row++)
        {
            int src = row * img.Width * 4;
            int dst = ((at.Y + row) * side + at.X) * 4;
            Array.Copy(img.Rgba, src, atlas, dst, img.Width * 4);
        }
    }

    private static int NextPowerOfTwo(int value)
    {
        int p = 1;
        while (p < value) p <<= 1;
        return p;
    }
}
