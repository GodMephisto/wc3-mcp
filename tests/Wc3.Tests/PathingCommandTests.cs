// tests/Wc3.Tests/PathingCommandTests.cs
using War3Net.Build.Environment; // MapPathingMap, PathingType, MapPathingMapFormatVersion
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

public class PathingCommandTests
{
    // Distinct width/height so any x/y transposition bug shows up. Replaces the
    // blank doc's default wpm with a fresh, all-clear W x H pathing map.
    private const int W = 16;
    private const int H = 12;

    private static MapDocument BlankWithPathing(int w = W, int h = H)
    {
        var doc = BlankMap.Create();
        var map = new MapPathingMap(MapPathingMapFormatVersion.v0)
        {
            Width = (uint)w,
            Height = (uint)h,
            Cells = Enumerable.Repeat(default(PathingType), w * h).ToList(),
        };
        doc.AddOrReplaceModelFile(PathingCommand.PathingFile, map);
        return doc;
    }

    private static MapPathingMap Reload(MapDocument doc)
    {
        byte[] saved = doc.SaveToBytes();
        var reloaded = MapDocument.Load(saved);
        // A well-formed pathing edit must not degrade the clean round-trip.
        Assert.DoesNotContain(reloaded.Diagnostics, d => d.Severity == DiagnosticSeverity.Warning);
        var map = reloaded.GetFile(PathingCommand.PathingFile)?.Model as MapPathingMap;
        Assert.NotNull(map);
        return map!;
    }

    [Fact]
    public void Paint_SingleCell_HitsExactIndex_AndSurvivesRoundTrip()
    {
        var doc = BlankWithPathing();
        // radius 0 square = exactly one cell.
        var r = PathingCommand.Paint(doc, centerX: 3, centerY: 5, radius: 0,
            flags: PathingType.Walk | PathingType.Build, op: PathingCommand.BrushOp.Set);

        Assert.True(r.Ok, r.Message);
        Assert.Equal(1, r.CellsChanged);

        var map = Reload(doc);
        int hit = 5 * W + 3;
        for (int i = 0; i < map.Cells.Count; i++)
        {
            var expected = i == hit ? (PathingType.Walk | PathingType.Build) : default;
            Assert.Equal(expected, map.Cells[i]);
        }
    }

    [Fact]
    public void Paint_Square_SetsFullFootprint()
    {
        var doc = BlankWithPathing();
        var r = PathingCommand.Paint(doc, 8, 6, radius: 2, flags: PathingType.Water,
            shape: PathingCommand.BrushShape.Square);

        Assert.True(r.Ok, r.Message);
        Assert.Equal(25, r.CellsChanged); // (2*2+1)^2

        var map = Reload(doc);
        for (int y = 4; y <= 8; y++)
            for (int x = 6; x <= 10; x++)
                Assert.True(map.Cells[y * W + x].HasFlag(PathingType.Water), $"({x},{y}) should be water");
        // A cell just outside the square is untouched.
        Assert.Equal(default, map.Cells[6 * W + 5]);
    }

    [Fact]
    public void Paint_Circle_ExcludesCornersOutsideRadius()
    {
        var doc = BlankWithPathing();
        var r = PathingCommand.Paint(doc, 8, 6, radius: 2, flags: PathingType.Fly,
            shape: PathingCommand.BrushShape.Circle);

        Assert.True(r.Ok, r.Message);
        // Circle r=2 (Euclidean, cell-center, dx²+dy²≤4): 5 + 3 + 3 + 1 + 1 = 13 cells.
        // (25-cell bounding square minus the 12 offsets with dx²+dy² > 4.)
        Assert.Equal(13, r.CellsChanged);

        var map = Reload(doc);
        Assert.True(map.Cells[6 * W + 8].HasFlag(PathingType.Fly));   // center
        Assert.True(map.Cells[6 * W + 10].HasFlag(PathingType.Fly));  // edge on axis (dist 2)
        Assert.Equal(default, map.Cells[4 * W + 6]);                  // corner (dx=-2,dy=-2 -> 8 > 4)
        Assert.Equal(default, map.Cells[8 * W + 10]);                 // corner
    }

    [Fact]
    public void Paint_Clear_RemovesOnlyNamedBits()
    {
        var doc = BlankWithPathing();
        PathingCommand.Paint(doc, 8, 6, 1, PathingType.Walk | PathingType.Fly);
        var r = PathingCommand.Paint(doc, 8, 6, 1, PathingType.Walk, op: PathingCommand.BrushOp.Clear);

        Assert.True(r.Ok, r.Message);
        Assert.Equal(9, r.CellsChanged);

        var map = Reload(doc);
        var cell = map.Cells[6 * W + 8];
        Assert.False(cell.HasFlag(PathingType.Walk)); // cleared
        Assert.True(cell.HasFlag(PathingType.Fly));   // untouched
    }

    [Fact]
    public void Paint_Toggle_FlipsNamedBits()
    {
        var doc = BlankWithPathing();
        PathingCommand.Paint(doc, 8, 6, 0, PathingType.Blight);           // set Blight at one cell
        var r = PathingCommand.Paint(doc, 8, 6, 0, PathingType.Blight | PathingType.Water,
            op: PathingCommand.BrushOp.Toggle);

        Assert.True(r.Ok, r.Message);
        var map = Reload(doc);
        var cell = map.Cells[6 * W + 8];
        Assert.False(cell.HasFlag(PathingType.Blight)); // was set -> toggled off
        Assert.True(cell.HasFlag(PathingType.Water));   // was clear -> toggled on
    }

    [Fact]
    public void Paint_ClampsToBounds_NoOverflowNearCorner()
    {
        var doc = BlankWithPathing();
        // Center at (0,0) with a big radius: only the in-bounds quadrant is touched.
        var r = PathingCommand.Paint(doc, 0, 0, radius: 100, flags: PathingType.Build,
            shape: PathingCommand.BrushShape.Square);

        Assert.True(r.Ok, r.Message);
        Assert.Equal(W * H, r.CellsChanged); // whole grid is within a 100-cell square of origin

        var map = Reload(doc);
        Assert.All(map.Cells, c => Assert.True(c.HasFlag(PathingType.Build)));
    }

    [Fact]
    public void PaintRect_FillsInclusiveRectangle_CornersAnyOrder()
    {
        var doc = BlankWithPathing();
        // Corners given max-first to prove ordering normalization.
        var r = PathingCommand.PaintRect(doc, x0: 10, y0: 8, x1: 2, y1: 1, flags: PathingType.Walk);

        Assert.True(r.Ok, r.Message);
        Assert.Equal((10 - 2 + 1) * (8 - 1 + 1), r.CellsChanged); // 9 * 8

        var map = Reload(doc);
        for (int y = 1; y <= 8; y++)
            for (int x = 2; x <= 10; x++)
                Assert.True(map.Cells[y * W + x].HasFlag(PathingType.Walk), $"({x},{y})");
        Assert.Equal(default, map.Cells[0]);            // (0,0) outside rect
        Assert.Equal(default, map.Cells[9 * W + 11]);   // (11,9) outside rect
    }

    [Fact]
    public void Paint_NoOp_WhenFlagsAlreadySet_ReportsZeroChanged()
    {
        var doc = BlankWithPathing();
        PathingCommand.Paint(doc, 8, 6, 1, PathingType.Walk);
        var again = PathingCommand.Paint(doc, 8, 6, 1, PathingType.Walk); // already set

        Assert.True(again.Ok, again.Message);
        Assert.Equal(0, again.CellsChanged);
    }

    [Fact]
    public void Paint_ReturnsError_WhenMapHasNoPathingFile()
    {
        // BlankMap now ships a wpm, so build a doc without one from scratch.
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = new byte[] { 0x2F, 0x2F }, // "//"
        }));
        Assert.Null(doc.GetFile(PathingCommand.PathingFile));

        var r = PathingCommand.Paint(doc, 0, 0, 1, PathingType.Walk);
        Assert.False(r.Ok);
        Assert.Equal(0, r.CellsChanged);
    }

    [Fact]
    public void Paint_RejectsNegativeRadius()
    {
        var doc = BlankWithPathing();
        var r = PathingCommand.Paint(doc, 0, 0, radius: -1, flags: PathingType.Walk);
        Assert.False(r.Ok);
    }
}
