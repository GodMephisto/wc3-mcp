// src/Wc3.Commands/PathingCommand.cs
using War3Net.Build.Environment;   // MapPathingMap, PathingType
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// HiveWE-style pathing brush over a map's war3map.wpm. The pathing map is a
/// Width×Height grid of <see cref="PathingType"/> flag bytes (4 pathing cells per
/// terrain tile). A <b>set</b> bit marks that capability as <i>restricted</i> for the
/// cell — e.g. Walk set = ground units cannot walk, Build set = cannot build, Water
/// set = the cell is water. This operates on the raw flags so there is no
/// inverted-semantics ambiguity: callers name the exact <see cref="PathingType"/>
/// bits to Set / Clear / Toggle.
///
/// Cells are addressed in pathing-cell coordinates with index = y*Width + x, matching
/// War3Net's <c>Cells</c> ordering (row-major from the map's south-west corner).
/// World-coordinate convenience overloads can layer on top later. Built primarily so
/// automated tests can paint a known region and assert the load/edit/save round-trip,
/// but it is a general API.
/// </summary>
public static class PathingCommand
{
    public const string PathingFile = "war3map.wpm";

    /// <summary>How brush flags combine with each cell's existing flags.</summary>
    public enum BrushOp { Set, Clear, Toggle }

    /// <summary>Brush footprint.</summary>
    public enum BrushShape { Square, Circle }

    public sealed record PaintResult(bool Ok, string Message, int CellsChanged = 0);

    /// <summary>
    /// Paints <paramref name="flags"/> onto every cell within <paramref name="radius"/>
    /// cells of (<paramref name="centerX"/>, <paramref name="centerY"/>). Square uses a
    /// (2r+1)² footprint; Circle keeps cells whose center is within the radius. Cells
    /// outside the map bounds are skipped. Returns the number of cells actually changed.
    /// </summary>
    public static PaintResult Paint(
        MapDocument doc,
        int centerX,
        int centerY,
        int radius,
        PathingType flags,
        BrushOp op = BrushOp.Set,
        BrushShape shape = BrushShape.Square)
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
            flags, op,
            describe: $"{op} {flags} over {shape.ToString().ToLowerInvariant()} r{radius} at ({centerX},{centerY})");
    }

    /// <summary>
    /// Paints <paramref name="flags"/> onto every cell in the inclusive axis-aligned
    /// rectangle [x0,x1]×[y0,y1] (pathing-cell coordinates; corners may be given in any
    /// order). Cells outside the map bounds are skipped. A rectangle spanning the whole
    /// grid is a full-map fill.
    /// </summary>
    public static PaintResult PaintRect(
        MapDocument doc,
        int x0, int y0, int x1, int y1,
        PathingType flags,
        BrushOp op = BrushOp.Set)
    {
        int minX = Math.Min(x0, x1), minY = Math.Min(y0, y1);
        int maxX = Math.Max(x0, x1), maxY = Math.Max(y0, y1);
        return Apply(
            doc,
            x0: minX, y0: minY, x1: maxX, y1: maxY,
            keep: static (_, _) => true,
            flags, op,
            describe: $"{op} {flags} over rect ({minX},{minY})-({maxX},{maxY})");
    }

    private static PaintResult Apply(
        MapDocument doc,
        int x0, int y0, int x1, int y1,
        Func<int, int, bool> keep,
        PathingType flags,
        BrushOp op,
        string describe)
    {
        var entry = doc.GetFile(PathingFile);
        if (entry?.Model is not MapPathingMap map)
            return new(false, $"map has no {PathingFile} (pathing map) to paint");

        int w = (int)map.Width, h = (int)map.Height;
        if (map.Cells is null || map.Cells.Count != w * h)
            return new(false,
                $"pathing map cell buffer inconsistent (Width={w} Height={h} Cells={map.Cells?.Count ?? 0})");

        // Clamp the affected rectangle to the grid, then apply the shape predicate.
        int loX = Math.Max(0, x0), hiX = Math.Min(w - 1, x1);
        int loY = Math.Max(0, y0), hiY = Math.Min(h - 1, y1);

        int changed = 0;
        for (int y = loY; y <= hiY; y++)
        {
            int rowBase = y * w;
            for (int x = loX; x <= hiX; x++)
            {
                if (!keep(x, y)) continue;
                int idx = rowBase + x;
                PathingType before = map.Cells[idx];
                PathingType after = op switch
                {
                    BrushOp.Set => before | flags,
                    BrushOp.Clear => before & ~flags,
                    BrushOp.Toggle => before ^ flags,
                    _ => before,
                };
                if (after != before)
                {
                    map.Cells[idx] = after;
                    changed++;
                }
            }
        }

        // Only mark the subfile dirty when something actually changed, so a no-op
        // paint doesn't force a re-serialize on Save.
        if (changed > 0)
            doc.AddOrReplaceModelFile(PathingFile, map);
        return new(true, $"{describe} — {changed} cells changed", changed);
    }
}
