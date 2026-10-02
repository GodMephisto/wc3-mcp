// src/Wc3.Commands/TerrainCommand.cs
using War3Net.Build.Environment;   // MapEnvironment, TerrainTile
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// HiveWE-style terrain height brush over a map's war3map.w3e. The environment is a
/// (Width+1)×(Height+1) grid of corner tiles (<c>env.TerrainTiles</c>, row-major from the
/// map's south-west corner, index = y*(Width+1) + x — the same convention
/// <see cref="Wc3.Render.TerrainRenderer"/> reads). Each tile's <c>Height</c> is its
/// ground elevation excluding cliff level; <b>raise</b>/<b>lower</b> shift it, <b>set</b>
/// assigns an absolute value, <b>flatten</b> levels a region to its own mean, and
/// <b>smooth</b> averages each tile toward its neighbours.
///
/// Deforms operate on raw tile <c>Height</c> so there is no unit ambiguity: callers pass
/// amounts in the same units the tiles already store (use <see cref="Stats"/> to inspect
/// the live range before brushing). Built primarily so automated tests can raise a known
/// region and assert the load/edit/save round-trip, but it is a general API.
/// </summary>
public static partial class TerrainCommand
{
    public const string TerrainFile = "war3map.w3e";

    /// <summary>How a brush changes each affected tile's height.</summary>
    public enum HeightOp { Raise, Lower, Set, Flatten, Smooth }

    /// <summary>Brush footprint.</summary>
    public enum BrushShape { Square, Circle }

    /// <summary>How a brush changes each affected tile's integer <c>CliffLevel</c> — the
    /// discrete plateau layer Warcraft III renders vertical cliff faces between (added to
    /// <c>Height</c> for elevation; a blank map's base layer is 0).</summary>
    public enum CliffOp { Raise, Lower, Set }

    public sealed record DeformResult(bool Ok, string Message, int TilesChanged = 0);

    public sealed record PaintResult(bool Ok, string Message, int TilesChanged = 0);

    public sealed record StatsResult(
        bool Ok, string Message, int TileCount = 0,
        float MinHeight = 0, float MaxHeight = 0, float MeanHeight = 0,
        int MinCliff = 0, int MaxCliff = 0);

    /// <summary>
    /// Deforms every tile within <paramref name="radius"/> corners of
    /// (<paramref name="centerX"/>, <paramref name="centerY"/>). Square uses a (2r+1)²
    /// footprint; Circle keeps tiles whose centre is within the radius. Tiles outside the
    /// grid are skipped. <paramref name="amount"/> is the step for Raise/Lower and the
    /// target for Set (ignored by Flatten/Smooth). Returns the number of tiles changed.
    /// </summary>
    public static DeformResult Deform(
        MapDocument doc,
        int centerX,
        int centerY,
        int radius,
        HeightOp op,
        float amount = 1f,
        BrushShape shape = BrushShape.Circle)
    {
        if (radius < 0)
            return new(false, $"radius must be >= 0 (got {radius})");

        long r2 = (long)radius * radius;
        return Apply(
            doc,
            x0: centerX - radius, y0: centerY - radius,
            x1: centerX + radius, y1: centerY + radius,
            keep: (x, y) =>
            {
                if (shape != BrushShape.Circle) return true;
                long dx = x - centerX, dy = y - centerY;
                return dx * dx + dy * dy <= r2;
            },
            op, amount,
            describe: $"{op} {FmtAmount(op, amount)}over {shape.ToString().ToLowerInvariant()} r{radius} at ({centerX},{centerY})");
    }

    /// <summary>
    /// Deforms every tile in the inclusive axis-aligned rectangle [x0,x1]×[y0,y1]
    /// (corner coordinates; corners may be given in any order). Tiles outside the grid are
    /// skipped. A rectangle spanning the whole grid is a full-map deform.
    /// </summary>
    public static DeformResult DeformRect(
        MapDocument doc,
        int x0, int y0, int x1, int y1,
        HeightOp op,
        float amount = 1f)
    {
        int minX = Math.Min(x0, x1), minY = Math.Min(y0, y1);
        int maxX = Math.Max(x0, x1), maxY = Math.Max(y0, y1);
        return Apply(
            doc,
            x0: minX, y0: minY, x1: maxX, y1: maxY,
            keep: static (_, _) => true,
            op, amount,
            describe: $"{op} {FmtAmount(op, amount)}over rect ({minX},{minY})-({maxX},{maxY})");
    }

    /// <summary>
    /// Paints ground <paramref name="textureIndex"/> onto every tile within
    /// <paramref name="radius"/> corners of (<paramref name="centerX"/>,
    /// <paramref name="centerY"/>), using the same footprint rules as <see cref="Deform"/>
    /// (Square = (2r+1)², Circle = tiles whose centre is within the radius). The index is an
    /// offset into the map's ground tile-type list (<c>env.TerrainTypes</c>); it is validated
    /// against that list's length. <paramref name="variation"/>, when non-null, also sets each
    /// tile's texture variation. Returns the number of tiles actually changed.
    /// </summary>
    public static PaintResult Paint(
        MapDocument doc,
        int centerX,
        int centerY,
        int radius,
        int textureIndex,
        int? variation = null,
        BrushShape shape = BrushShape.Circle)
    {
        if (radius < 0)
            return new(false, $"radius must be >= 0 (got {radius})");

        long r2 = (long)radius * radius;
        return ApplyPaint(
            doc,
            x0: centerX - radius, y0: centerY - radius,
            x1: centerX + radius, y1: centerY + radius,
            keep: (x, y) =>
            {
                if (shape != BrushShape.Circle) return true;
                long dx = x - centerX, dy = y - centerY;
                return dx * dx + dy * dy <= r2;
            },
            textureIndex, variation,
            describe: $"paint tex {textureIndex}{FmtVariation(variation)}over {shape.ToString().ToLowerInvariant()} r{radius} at ({centerX},{centerY})");
    }

    /// <summary>
    /// Paints ground <paramref name="textureIndex"/> onto every tile in the inclusive
    /// axis-aligned rectangle [x0,x1]×[y0,y1] (corners in any order; a full-grid rectangle
    /// repaints the whole map). See <see cref="Paint"/> for index/variation semantics.
    /// </summary>
    public static PaintResult PaintRect(
        MapDocument doc,
        int x0, int y0, int x1, int y1,
        int textureIndex,
        int? variation = null)
    {
        int minX = Math.Min(x0, x1), minY = Math.Min(y0, y1);
        int maxX = Math.Max(x0, x1), maxY = Math.Max(y0, y1);
        return ApplyPaint(
            doc,
            x0: minX, y0: minY, x1: maxX, y1: maxY,
            keep: static (_, _) => true,
            textureIndex, variation,
            describe: $"paint tex {textureIndex}{FmtVariation(variation)}over rect ({minX},{minY})-({maxX},{maxY})");
    }

    private static string FmtVariation(int? variation) =>
        variation.HasValue ? $" var {variation.Value} " : " ";

    /// <summary>
    /// Reports the height range (min/max/mean) and cliff-level range over the whole
    /// terrain grid, without modifying anything. Use this to discover the units a map's
    /// tiles store before choosing a raise/lower step.
    /// </summary>
    public static StatsResult Stats(MapDocument doc)
    {
        var entry = doc.GetFile(TerrainFile);
        if (entry?.Model is not MapEnvironment env)
            return new(false, $"map has no {TerrainFile} (terrain) to inspect");

        var tiles = env.TerrainTiles;
        if (tiles is null || tiles.Count == 0)
            return new(false, "terrain has no tiles");

        float min = float.MaxValue, max = float.MinValue;
        double sum = 0;
        int cmin = int.MaxValue, cmax = int.MinValue;
        foreach (var t in tiles)
        {
            float hgt = t.Height;
            if (hgt < min) min = hgt;
            if (hgt > max) max = hgt;
            sum += hgt;
            int cl = t.CliffLevel;
            if (cl < cmin) cmin = cl;
            if (cl > cmax) cmax = cl;
        }

        float mean = (float)(sum / tiles.Count);
        return new(true,
            $"{tiles.Count} tiles: height {min:0.###}..{max:0.###} (mean {mean:0.###}), cliff {cmin}..{cmax}",
            tiles.Count, min, max, mean, cmin, cmax);
    }

    private static string FmtAmount(HeightOp op, float amount) => op switch
    {
        HeightOp.Raise or HeightOp.Lower => $"{amount:0.###} ",
        HeightOp.Set => $"{amount:0.###} ",
        _ => "",
    };

    private static DeformResult Apply(
        MapDocument doc,
        int x0, int y0, int x1, int y1,
        Func<int, int, bool> keep,
        HeightOp op,
        float amount,
        string describe)
    {
        var entry = doc.GetFile(TerrainFile);
        if (entry?.Model is not MapEnvironment env)
            return new(false, $"map has no {TerrainFile} (terrain) to deform");

        int w = (int)env.Width + 1, h = (int)env.Height + 1;
        var tiles = env.TerrainTiles;
        if (tiles is null || tiles.Count != w * h)
            return new(false,
                $"terrain tile buffer inconsistent (Width+1={w} Height+1={h} Tiles={tiles?.Count ?? 0})");

        // Snapshot every tile's height up-front so Flatten/Smooth read a stable source
        // that is independent of write order, and so change-counting compares against the
        // true pre-edit value.
        var src = new float[tiles.Count];
        for (int i = 0; i < tiles.Count; i++)
            src[i] = tiles[i].Height;

        // Clamp the affected rectangle to the grid, then apply the shape predicate.
        int loX = Math.Max(0, x0), hiX = Math.Min(w - 1, x1);
        int loY = Math.Max(0, y0), hiY = Math.Min(h - 1, y1);
        if (loX > hiX || loY > hiY)
            return new(true, $"{describe} — 0 tiles changed (region off-grid)", 0);

        // Flatten needs the region mean before writing anything.
        float flattenTarget = 0f;
        if (op == HeightOp.Flatten)
        {
            double s = 0; int n = 0;
            for (int y = loY; y <= hiY; y++)
                for (int x = loX; x <= hiX; x++)
                    if (keep(x, y)) { s += src[y * w + x]; n++; }
            if (n == 0)
                return new(true, $"{describe} — 0 tiles changed (empty footprint)", 0);
            flattenTarget = (float)(s / n);
        }

        int changed = 0;
        for (int y = loY; y <= hiY; y++)
        {
            int rowBase = y * w;
            for (int x = loX; x <= hiX; x++)
            {
                if (!keep(x, y)) continue;
                int idx = rowBase + x;
                float before = src[idx];
                float after = op switch
                {
                    HeightOp.Raise => before + amount,
                    HeightOp.Lower => before - amount,
                    HeightOp.Set => amount,
                    HeightOp.Flatten => flattenTarget,
                    HeightOp.Smooth => SmoothedHeight(src, w, h, x, y),
                    _ => before,
                };

                if (after != before)
                {
                    tiles[idx].Height = after;
                    changed++;
                }
            }
        }

        // Only mark the subfile dirty when something actually changed, so a no-op deform
        // doesn't force a re-serialize on Save.
        if (changed > 0)
            doc.AddOrReplaceModelFile(TerrainFile, env);
        return new(true, $"{describe} — {changed} tiles changed", changed);
    }

    private static PaintResult ApplyPaint(
        MapDocument doc,
        int x0, int y0, int x1, int y1,
        Func<int, int, bool> keep,
        int textureIndex,
        int? variation,
        string describe)
    {
        var entry = doc.GetFile(TerrainFile);
        if (entry?.Model is not MapEnvironment env)
            return new(false, $"map has no {TerrainFile} (terrain) to paint");

        int groundTypes = env.TerrainTypes?.Count ?? 0;
        if (groundTypes == 0)
            return new(false, "map has no ground tile-types to paint with");
        if (textureIndex < 0 || textureIndex >= groundTypes)
            return new(false,
                $"texture index {textureIndex} out of range (map has {groundTypes} ground tile-types; valid 0..{groundTypes - 1})");
        if (variation.HasValue && variation.Value < 0)
            return new(false, $"variation must be >= 0 (got {variation.Value})");

        int w = (int)env.Width + 1, h = (int)env.Height + 1;
        var tiles = env.TerrainTiles;
        if (tiles is null || tiles.Count != w * h)
            return new(false,
                $"terrain tile buffer inconsistent (Width+1={w} Height+1={h} Tiles={tiles?.Count ?? 0})");

        // Clamp the affected rectangle to the grid, then apply the shape predicate.
        int loX = Math.Max(0, x0), hiX = Math.Min(w - 1, x1);
        int loY = Math.Max(0, y0), hiY = Math.Min(h - 1, y1);
        if (loX > hiX || loY > hiY)
            return new(true, $"{describe} — 0 tiles changed (region off-grid)", 0);

        int changed = 0;
        for (int y = loY; y <= hiY; y++)
        {
            int rowBase = y * w;
            for (int x = loX; x <= hiX; x++)
            {
                if (!keep(x, y)) continue;
                // TerrainTile is a reference type (the height Apply mutates tiles[idx]
                // in place), so editing the captured element updates the buffer.
                var tile = tiles[rowBase + x];
                bool tileChanged = false;
                if (tile.Texture != textureIndex) { tile.Texture = textureIndex; tileChanged = true; }
                if (variation.HasValue && tile.Variation != variation.Value)
                {
                    tile.Variation = variation.Value;
                    tileChanged = true;
                }
                if (tileChanged) changed++;
            }
        }

        // Only mark the subfile dirty when something actually changed, so a no-op paint
        // doesn't force a re-serialize on Save.
        if (changed > 0)
            doc.AddOrReplaceModelFile(TerrainFile, env);
        return new(true, $"{describe} — {changed} tiles changed", changed);
    }

    // --- cliff level / ramp geometry ----------------------------------------

    /// <summary>
    /// Raises, lowers, or sets the integer <c>CliffLevel</c> of every tile within
    /// <paramref name="radius"/> of (<paramref name="centerX"/>,<paramref name="centerY"/>),
    /// using the same footprint rules as <see cref="Deform"/> (Square = (2r+1)², Circle =
    /// tiles whose centre is within the radius). Cliff levels are the discrete plateau
    /// layers Warcraft III renders vertical faces between; the result is floored at 0 (the
    /// format's base layer). <paramref name="level"/> is the absolute layer for
    /// <see cref="CliffOp.Set"/> and the shift magnitude for raise/lower, and must be &gt;= 0.
    /// Returns the number of tiles actually changed.
    /// </summary>
    public static DeformResult Cliff(
        MapDocument doc,
        int centerX,
        int centerY,
        int radius,
        CliffOp op,
        int level,
        BrushShape shape = BrushShape.Circle)
    {
        if (radius < 0)
            return new(false, $"radius must be >= 0 (got {radius})");

        long r2 = (long)radius * radius;
        return ApplyCliff(
            doc,
            x0: centerX - radius, y0: centerY - radius,
            x1: centerX + radius, y1: centerY + radius,
            keep: (x, y) =>
            {
                if (shape != BrushShape.Circle) return true;
                long dx = x - centerX, dy = y - centerY;
                return dx * dx + dy * dy <= r2;
            },
            op, level,
            describe: $"cliff {op.ToString().ToLowerInvariant()} {level} over {shape.ToString().ToLowerInvariant()} r{radius} at ({centerX},{centerY})");
    }

    /// <summary>
    /// Cliff-level variant of <see cref="DeformRect"/>: applies <paramref name="op"/> over
    /// the inclusive rectangle [x0,x1]×[y0,y1] (corners in any order). See <see cref="Cliff"/>.
    /// </summary>
    public static DeformResult CliffRect(
        MapDocument doc,
        int x0, int y0, int x1, int y1,
        CliffOp op,
        int level)
    {
        int minX = Math.Min(x0, x1), minY = Math.Min(y0, y1);
        int maxX = Math.Max(x0, x1), maxY = Math.Max(y0, y1);
        return ApplyCliff(
            doc,
            x0: minX, y0: minY, x1: maxX, y1: maxY,
            keep: static (_, _) => true,
            op, level,
            describe: $"cliff {op.ToString().ToLowerInvariant()} {level} over rect ({minX},{minY})-({maxX},{maxY})");
    }

    /// <summary>
    /// Sets (<paramref name="on"/> = true) or clears the <c>IsRamp</c> flag on every tile
    /// within <paramref name="radius"/> of (<paramref name="centerX"/>,<paramref name="centerY"/>),
    /// using the same footprint rules as <see cref="Deform"/>. Ramp tiles are the sloped
    /// transitions Warcraft III draws between adjacent cliff levels. Returns the number of
    /// tiles whose flag actually changed.
    /// </summary>
    public static DeformResult Ramp(
        MapDocument doc,
        int centerX,
        int centerY,
        int radius,
        bool on,
        BrushShape shape = BrushShape.Circle)
    {
        if (radius < 0)
            return new(false, $"radius must be >= 0 (got {radius})");

        long r2 = (long)radius * radius;
        return ApplyRamp(
            doc,
            x0: centerX - radius, y0: centerY - radius,
            x1: centerX + radius, y1: centerY + radius,
            keep: (x, y) =>
            {
                if (shape != BrushShape.Circle) return true;
                long dx = x - centerX, dy = y - centerY;
                return dx * dx + dy * dy <= r2;
            },
            on,
            describe: $"ramp {(on ? "on" : "off")} over {shape.ToString().ToLowerInvariant()} r{radius} at ({centerX},{centerY})");
    }

    /// <summary>
    /// Rectangle variant of <see cref="Ramp"/>: sets or clears <c>IsRamp</c> over the
    /// inclusive rectangle [x0,x1]×[y0,y1] (corners in any order).
    /// </summary>
    public static DeformResult RampRect(
        MapDocument doc,
        int x0, int y0, int x1, int y1,
        bool on)
    {
        int minX = Math.Min(x0, x1), minY = Math.Min(y0, y1);
        int maxX = Math.Max(x0, x1), maxY = Math.Max(y0, y1);
        return ApplyRamp(
            doc,
            x0: minX, y0: minY, x1: maxX, y1: maxY,
            keep: static (_, _) => true,
            on,
            describe: $"ramp {(on ? "on" : "off")} over rect ({minX},{minY})-({maxX},{maxY})");
    }

    private static DeformResult ApplyCliff(
        MapDocument doc,
        int x0, int y0, int x1, int y1,
        Func<int, int, bool> keep,
        CliffOp op,
        int level,
        string describe)
    {
        var entry = doc.GetFile(TerrainFile);
        if (entry?.Model is not MapEnvironment env)
            return new(false, $"map has no {TerrainFile} (terrain) to deform");

        if (level < 0)
            return new(false, $"cliff level must be >= 0 (got {level})");

        int w = (int)env.Width + 1, h = (int)env.Height + 1;
        var tiles = env.TerrainTiles;
        if (tiles is null || tiles.Count != w * h)
            return new(false,
                $"terrain tile buffer inconsistent (Width+1={w} Height+1={h} Tiles={tiles?.Count ?? 0})");

        // Clamp the affected rectangle to the grid, then apply the shape predicate.
        int loX = Math.Max(0, x0), hiX = Math.Min(w - 1, x1);
        int loY = Math.Max(0, y0), hiY = Math.Min(h - 1, y1);
        if (loX > hiX || loY > hiY)
            return new(true, $"{describe} — 0 tiles changed (region off-grid)", 0);

        int changed = 0;
        for (int y = loY; y <= hiY; y++)
        {
            int rowBase = y * w;
            for (int x = loX; x <= hiX; x++)
            {
                if (!keep(x, y)) continue;
                var tile = tiles[rowBase + x];
                int before = tile.CliffLevel;
                int after = op switch
                {
                    CliffOp.Raise => before + level,
                    CliffOp.Lower => before - level,
                    CliffOp.Set => level,
                    _ => before,
                };
                if (after < 0) after = 0;   // floor at the format's base layer
                if (after != before) { tile.CliffLevel = after; changed++; }
            }
        }

        if (changed > 0)
            doc.AddOrReplaceModelFile(TerrainFile, env);
        return new(true, $"{describe} — {changed} tiles changed", changed);
    }

    private static DeformResult ApplyRamp(
        MapDocument doc,
        int x0, int y0, int x1, int y1,
        Func<int, int, bool> keep,
        bool on,
        string describe)
    {
        var entry = doc.GetFile(TerrainFile);
        if (entry?.Model is not MapEnvironment env)
            return new(false, $"map has no {TerrainFile} (terrain) to deform");

        int w = (int)env.Width + 1, h = (int)env.Height + 1;
        var tiles = env.TerrainTiles;
        if (tiles is null || tiles.Count != w * h)
            return new(false,
                $"terrain tile buffer inconsistent (Width+1={w} Height+1={h} Tiles={tiles?.Count ?? 0})");

        int loX = Math.Max(0, x0), hiX = Math.Min(w - 1, x1);
        int loY = Math.Max(0, y0), hiY = Math.Min(h - 1, y1);
        if (loX > hiX || loY > hiY)
            return new(true, $"{describe} — 0 tiles changed (region off-grid)", 0);

        int changed = 0;
        for (int y = loY; y <= hiY; y++)
        {
            int rowBase = y * w;
            for (int x = loX; x <= hiX; x++)
            {
                if (!keep(x, y)) continue;
                var tile = tiles[rowBase + x];
                if (tile.IsRamp != on) { tile.IsRamp = on; changed++; }
            }
        }

        if (changed > 0)
            doc.AddOrReplaceModelFile(TerrainFile, env);
        return new(true, $"{describe} — {changed} tiles changed", changed);
    }

    /// <summary>Mean of a tile and its in-grid 4-neighbours, read from the snapshot.</summary>
    private static float SmoothedHeight(float[] src, int w, int h, int x, int y)
    {
        float sum = src[y * w + x];
        int n = 1;
        if (x > 0) { sum += src[y * w + (x - 1)]; n++; }
        if (x < w - 1) { sum += src[y * w + (x + 1)]; n++; }
        if (y > 0) { sum += src[(y - 1) * w + x]; n++; }
        if (y < h - 1) { sum += src[(y + 1) * w + x]; n++; }
        return sum / n;
    }
}
