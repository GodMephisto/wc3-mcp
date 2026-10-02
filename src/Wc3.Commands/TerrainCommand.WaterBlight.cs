using War3Net.Build.Environment;   // MapEnvironment, TerrainTile
using Wc3.Model;                   // MapDocument

namespace Wc3.Commands;

/// <summary>
/// Water-height and blight area brushes for <see cref="TerrainCommand"/>, kept in their own
/// partial-class file. Both operate on the same (Width+1)×(Height+1) corner-tile grid as the
/// height/cliff/texture brushes and use the identical circle/square + radius footprint
/// convention, so a caller who knows <see cref="Deform"/> already knows these. Writes back
/// to <c>war3map.w3e</c> only when at least one tile actually changed.
/// </summary>
public static partial class TerrainCommand
{
    // ---- Ground query -------------------------------------------------------

    /// <summary>
    /// The ground surface level at one corner, in cliff-step units: <c>Height + CliffLevel</c>,
    /// the exact quantity the terrain mesh multiplies by 128 for world Z. Returns null when the
    /// map has no terrain, the grid is inconsistent, or (col,row) is off-grid. A caller that
    /// wants water to sit a set depth above the brushed ground reads this, adds the depth, and
    /// converts to a stored water level, so the water plane never renders under the terrain.
    /// </summary>
    public static float? GroundLevelAt(MapDocument doc, int col, int row)
    {
        if (doc.GetFile(TerrainFile)?.Model is not MapEnvironment env)
            return null;
        int w = (int)env.Width + 1, h = (int)env.Height + 1;
        var tiles = env.TerrainTiles;
        if (tiles is null || tiles.Count != w * h) return null;
        if (col < 0 || col >= w || row < 0 || row >= h) return null;
        var t = tiles[row * w + col];
        return t.Height + t.CliffLevel;
    }

    // ---- Water --------------------------------------------------------------

    /// <summary>How a water brush stroke changes a tile's water level.</summary>
    public enum WaterOp
    {
        /// <summary>Set the water height to an absolute value and flag the tile as water.</summary>
        Set,
        /// <summary>Add to the existing water height and flag the tile as water.</summary>
        Raise,
        /// <summary>Subtract from the existing water height (the tile stays flagged as water).</summary>
        Lower,
        /// <summary>Clear the water flag (drain); the stored height is left untouched.</summary>
        Remove,
    }

    /// <summary>
    /// Applies a water brush over a circular (default) or square footprint of the given
    /// <paramref name="radius"/> centred at (<paramref name="centerX"/>,<paramref name="centerY"/>).
    /// <paramref name="amount"/> is the absolute level for <see cref="WaterOp.Set"/> and the
    /// delta for Raise/Lower; it is ignored for <see cref="WaterOp.Remove"/>.
    /// </summary>
    public static DeformResult Water(
        MapDocument doc,
        int centerX,
        int centerY,
        int radius,
        WaterOp op,
        float amount = 0f,
        BrushShape shape = BrushShape.Circle)
    {
        if (radius < 0)
            return new(false, $"radius must be >= 0 (got {radius})");

        long r2 = (long)radius * radius;
        return ApplyWater(
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
            describe: $"water {DescribeWater(op, amount)} over {shape.ToString().ToLowerInvariant()} r{radius} at ({centerX},{centerY})");
    }

    /// <summary>
    /// Rectangle variant of <see cref="Water"/>: applies the op to every tile in the inclusive
    /// axis-aligned rectangle [x0,x1]×[y0,y1] (corners may be given in any order).
    /// </summary>
    public static DeformResult WaterRect(
        MapDocument doc,
        int x0, int y0, int x1, int y1,
        WaterOp op,
        float amount = 0f)
    {
        int lx = Math.Min(x0, x1), hx = Math.Max(x0, x1);
        int ly = Math.Min(y0, y1), hy = Math.Max(y0, y1);
        return ApplyWater(
            doc, lx, ly, hx, hy,
            keep: static (_, _) => true,
            op, amount,
            describe: $"water {DescribeWater(op, amount)} over rect [{lx},{ly}]-[{hx},{hy}]");
    }

    private static string DescribeWater(WaterOp op, float amount) =>
        op == WaterOp.Remove ? "remove" : $"{op.ToString().ToLowerInvariant()} {amount:0.###}";

    private static DeformResult ApplyWater(
        MapDocument doc,
        int x0, int y0, int x1, int y1,
        Func<int, int, bool> keep,
        WaterOp op,
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

                bool wasWater = tile.IsWater;
                float wasHeight = tile.WaterHeight;

                switch (op)
                {
                    case WaterOp.Set:    tile.WaterHeight = amount; tile.IsWater = true;  break;
                    case WaterOp.Raise:  tile.WaterHeight += amount; tile.IsWater = true; break;
                    case WaterOp.Lower:  tile.WaterHeight -= amount; tile.IsWater = true; break;
                    case WaterOp.Remove: tile.IsWater = false;                            break;
                }

                if (tile.IsWater != wasWater || tile.WaterHeight != wasHeight) changed++;
            }
        }

        if (changed > 0)
            doc.AddOrReplaceModelFile(TerrainFile, env);
        return new(true, $"{describe} — {changed} tiles changed", changed);
    }

    // ---- Blight -------------------------------------------------------------

    /// <summary>
    /// Sets (<paramref name="on"/> = true) or clears the blight flag on every tile within a
    /// circular (default) or square footprint. Mirrors the ramp-flag brush exactly.
    /// </summary>
    public static DeformResult Blight(
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
        return ApplyBlight(
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
            describe: $"{(on ? "blight" : "unblight")} over {shape.ToString().ToLowerInvariant()} r{radius} at ({centerX},{centerY})");
    }

    /// <summary>Rectangle variant of <see cref="Blight"/> (corners may be given in any order).</summary>
    public static DeformResult BlightRect(
        MapDocument doc,
        int x0, int y0, int x1, int y1,
        bool on)
    {
        int lx = Math.Min(x0, x1), hx = Math.Max(x0, x1);
        int ly = Math.Min(y0, y1), hy = Math.Max(y0, y1);
        return ApplyBlight(
            doc, lx, ly, hx, hy,
            keep: static (_, _) => true,
            on,
            describe: $"{(on ? "blight" : "unblight")} over rect [{lx},{ly}]-[{hx},{hy}]");
    }

    private static DeformResult ApplyBlight(
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
                if (tile.IsBlighted != on) { tile.IsBlighted = on; changed++; }
            }
        }

        if (changed > 0)
            doc.AddOrReplaceModelFile(TerrainFile, env);
        return new(true, $"{describe} — {changed} tiles changed", changed);
    }
}
