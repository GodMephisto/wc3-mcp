// src/Wc3.Commands/PathingCommand.cs
using War3Net.Build.Environment;   // MapPathingMap, PathingType
using War3Net.Build.Info;          // MapInfo
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One pathing problem. <see cref="Kind"/> is a stable token
/// (enclosed-unpathable, start-location-unpathable, unreachable-corner) and
/// <see cref="Detail"/> is human text.</summary>
public sealed record PathingIssue(string Kind, string Detail);

/// <summary>Result of <see cref="PathingCommand.Validate"/>. <see cref="Ok"/> is true when the
/// check ran and found nothing wrong. Width/Height are the pathing grid size in cells.</summary>
public sealed record PathingValidateResult(
    bool Ok, string Message, int Width, int Height, IReadOnlyList<PathingIssue> Issues);

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

    /// <summary>
    /// Flags three pathing problems in war3map.wpm. An enclosed unpathable pocket is a blob of
    /// Walk-restricted cells that touches no map border, so it sits entirely inside pathable
    /// ground. A start location is flagged when the tile it sits on is Walk-restricted. A map
    /// corner is unreachable when it is Walk-restricted, or when it is walkable but sealed off
    /// from the largest walkable region. Read-only.
    /// </summary>
    public static PathingValidateResult Validate(MapDocument doc)
    {
        if (doc.GetFile(PathingFile)?.Model is not MapPathingMap map)
            return new(false, $"map has no {PathingFile} (pathing map) to validate.",
                0, 0, Array.Empty<PathingIssue>());

        int w = (int)map.Width, h = (int)map.Height;
        if (w <= 0 || h <= 0 || map.Cells is null || map.Cells.Count != w * h)
            return new(false,
                $"pathing map cell buffer inconsistent (Width={w} Height={h} Cells={map.Cells?.Count ?? 0}).",
                w, h, Array.Empty<PathingIssue>());

        var issues = new List<PathingIssue>();
        var blocked = new bool[w * h];
        for (int i = 0; i < blocked.Length; i++)
            blocked[i] = map.Cells[i].HasFlag(PathingType.Walk);

        // 1. Enclosed unpathable pockets.
        var blockedSeen = new bool[w * h];
        for (int start = 0; start < blocked.Length; start++)
        {
            if (!blocked[start] || blockedSeen[start]) continue;
            var (size, touchesBorder, minIndex) = FloodBlocked(blocked, blockedSeen, w, h, start);
            if (!touchesBorder)
                issues.Add(new PathingIssue("enclosed-unpathable",
                    $"{size}-cell unpathable pocket at cell ({minIndex % w},{minIndex / w}) "
                    + "is fully enclosed by pathable tiles."));
        }

        // 2. Walkable components, so a corner can be told apart from the main walkable region.
        var openSeen = new bool[w * h];
        var compId = new int[w * h];
        Array.Fill(compId, -1);
        int nextId = 0, largestId = -1, largestSize = 0;
        for (int start = 0; start < blocked.Length; start++)
        {
            if (blocked[start] || openSeen[start]) continue;
            int id = nextId++;
            int size = FloodOpenAssign(blocked, openSeen, compId, w, h, start, id);
            if (size > largestSize) { largestSize = size; largestId = id; }
        }

        // 3. Unreachable map corners.
        var corners = new List<(int X, int Y)>();
        foreach (var c in new[] { (0, 0), (w - 1, 0), (0, h - 1), (w - 1, h - 1) })
            if (!corners.Contains(c)) corners.Add(c);

        foreach (var (cx, cy) in corners)
        {
            int idx = cy * w + cx;
            if (blocked[idx])
                issues.Add(new PathingIssue("unreachable-corner",
                    $"corner ({cx},{cy}) is unpathable."));
            else if (compId[idx] != largestId)
                issues.Add(new PathingIssue("unreachable-corner",
                    $"corner ({cx},{cy}) is sealed off from the main walkable area."));
        }

        // 4. Start locations on unpathable tiles. Needs war3map.w3i for the world-to-cell map.
        bool checkedStarts = false;
        if (doc.GetFile(MapInfoCommand.FileName)?.Model is MapInfo info
            && info.PlayableMapAreaWidth > 0 && info.PlayableMapAreaHeight > 0)
        {
            checkedStarts = true;
            float halfX = info.PlayableMapAreaWidth * 64f;
            float halfY = info.PlayableMapAreaHeight * 64f;
            foreach (var u in UnitInstanceCommand.List(doc))
            {
                if (!string.Equals(u.TypeRawcode, PlacementCommand.StartLocationRawcode, StringComparison.Ordinal))
                    continue;
                int col = Math.Clamp((int)Math.Floor((u.X + halfX) / (2 * halfX) * w), 0, w - 1);
                int row = Math.Clamp((int)Math.Floor((u.Y + halfY) / (2 * halfY) * h), 0, h - 1);
                if (blocked[row * w + col])
                    issues.Add(new PathingIssue("start-location-unpathable",
                        $"player {u.OwnerId} start location at ({u.X},{u.Y}) sits on "
                        + $"unpathable cell ({col},{row})."));
            }
        }

        string suffix = checkedStarts ? "" : " (no war3map.w3i, start locations not checked)";
        string message = issues.Count == 0
            ? $"checked the {w}x{h} pathing grid, no problems found{suffix}."
            : $"checked the {w}x{h} pathing grid, found {issues.Count} problem(s){suffix}.";
        return new(issues.Count == 0, message, w, h, issues);
    }

    /// <summary>4-connected flood over a boolean target set. Returns the component size,
    /// whether it touches any map border, and its lowest cell index.</summary>
    private static (int Size, bool TouchesBorder, int MinIndex) FloodBlocked(
        bool[] target, bool[] seen, int w, int h, int start)
    {
        var stack = new Stack<int>();
        stack.Push(start);
        seen[start] = true;
        int size = 0, min = start;
        bool border = false;
        while (stack.Count > 0)
        {
            int idx = stack.Pop();
            size++;
            if (idx < min) min = idx;
            int x = idx % w, y = idx / w;
            if (x == 0 || y == 0 || x == w - 1 || y == h - 1) border = true;
            Push(idx - 1, x > 0);
            Push(idx + 1, x < w - 1);
            Push(idx - w, y > 0);
            Push(idx + w, y < h - 1);
        }
        return (size, border, min);

        void Push(int n, bool inside)
        {
            if (inside && target[n] && !seen[n]) { seen[n] = true; stack.Push(n); }
        }
    }

    /// <summary>4-connected flood over the walkable (non-blocked) set, writing
    /// <paramref name="id"/> into <paramref name="compId"/> and returning the size.</summary>
    private static int FloodOpenAssign(
        bool[] blocked, bool[] seen, int[] compId, int w, int h, int start, int id)
    {
        var stack = new Stack<int>();
        stack.Push(start);
        seen[start] = true;
        int size = 0;
        while (stack.Count > 0)
        {
            int idx = stack.Pop();
            compId[idx] = id;
            size++;
            int x = idx % w, y = idx / w;
            Push(idx - 1, x > 0);
            Push(idx + 1, x < w - 1);
            Push(idx - w, y > 0);
            Push(idx + w, y < h - 1);
        }
        return size;

        void Push(int n, bool inside)
        {
            if (inside && !blocked[n] && !seen[n]) { seen[n] = true; stack.Push(n); }
        }
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
