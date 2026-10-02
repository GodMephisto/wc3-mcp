using Wc3.Commands;

namespace Wc3.Tests;

/// <summary>
/// Pure-logic tests for ZoomMath.ZoomAtPoint, the zoom-to-cursor math TerrainView uses for its
/// mouse-wheel zoom. Convention: offsets are ScrollViewer scroll offsets, a content point p is
/// visible at viewport position p * zoom - offset.
/// </summary>
public class ZoomMathTests
{
    private const double MinZoom = 0.1;
    private const double MaxZoom = 10.0;
    private const double Tolerance = 1e-9;

    /// <summary>Content point (in content/bitmap pixels) visible at a viewport position.</summary>
    private static (double X, double Y) ContentPointAt(
        double offsetX, double offsetY, double zoom, double viewportX, double viewportY) =>
        ((offsetX + viewportX) / zoom, (offsetY + viewportY) / zoom);

    [Theory]
    [InlineData(2.0)]  // zoom in
    [InlineData(0.5)]  // zoom out
    public void ZoomAtPoint_PointUnderCursor_IsInvariant(double factor)
    {
        const double offsetX = 30, offsetY = 40, zoom = 1.5;
        const double cursorX = 100, cursorY = 150;

        var before = ContentPointAt(offsetX, offsetY, zoom, cursorX, cursorY);
        var (ox, oy, newZoom) = ZoomMath.ZoomAtPoint(
            offsetX, offsetY, zoom, cursorX, cursorY, factor, MinZoom, MaxZoom);
        var after = ContentPointAt(ox, oy, newZoom, cursorX, cursorY);

        Assert.Equal(zoom * factor, newZoom, Tolerance);
        Assert.Equal(before.X, after.X, Tolerance);
        Assert.Equal(before.Y, after.Y, Tolerance);
    }

    [Fact]
    public void ZoomAtPoint_AtMax_ClampsAndKeepsOffsets()
    {
        var (ox, oy, zoom) = ZoomMath.ZoomAtPoint(
            30, 40, MaxZoom, 100, 150, 1.1, MinZoom, MaxZoom);

        Assert.Equal(MaxZoom, zoom);
        Assert.Equal(30, ox, Tolerance);
        Assert.Equal(40, oy, Tolerance);
    }

    [Fact]
    public void ZoomAtPoint_AtMin_ClampsAndKeepsOffsets()
    {
        var (ox, oy, zoom) = ZoomMath.ZoomAtPoint(
            30, 40, MinZoom, 100, 150, 0.9, MinZoom, MaxZoom);

        Assert.Equal(MinZoom, zoom);
        Assert.Equal(30, ox, Tolerance);
        Assert.Equal(40, oy, Tolerance);
    }

    [Fact]
    public void ZoomAtPoint_PartialClamp_StillKeepsPointUnderCursor()
    {
        // 8 * 2 would be 16; clamped to 10 — the invariant must hold for the clamped zoom.
        const double offsetX = 500, offsetY = 250, zoom = 8.0;
        const double cursorX = 64, cursorY = 32;

        var before = ContentPointAt(offsetX, offsetY, zoom, cursorX, cursorY);
        var (ox, oy, newZoom) = ZoomMath.ZoomAtPoint(
            offsetX, offsetY, zoom, cursorX, cursorY, 2.0, MinZoom, MaxZoom);
        var after = ContentPointAt(ox, oy, newZoom, cursorX, cursorY);

        Assert.Equal(MaxZoom, newZoom);
        Assert.Equal(before.X, after.X, Tolerance);
        Assert.Equal(before.Y, after.Y, Tolerance);
    }

    [Fact]
    public void ZoomAtPoint_CursorAtOrigin_ScalesOffsetsOnly()
    {
        // Cursor at the viewport origin: newOffset = offset * (newZoom / zoom).
        var (ox, oy, zoom) = ZoomMath.ZoomAtPoint(
            30, 40, 1.0, 0, 0, 2.0, MinZoom, MaxZoom);

        Assert.Equal(2.0, zoom, Tolerance);
        Assert.Equal(60, ox, Tolerance);
        Assert.Equal(80, oy, Tolerance);
    }

    [Fact]
    public void ZoomAtPoint_ZeroOffsetAtOrigin_StaysAtOrigin()
    {
        var (ox, oy, zoom) = ZoomMath.ZoomAtPoint(
            0, 0, 1.0, 0, 0, 2.0, MinZoom, MaxZoom);

        Assert.Equal(2.0, zoom, Tolerance);
        Assert.Equal(0, ox, Tolerance);
        Assert.Equal(0, oy, Tolerance);
    }

    [Fact]
    public void ZoomAtPoint_SymmetricInOut_ReturnsNearOriginal()
    {
        const double offsetX = 123.4, offsetY = 56.7, zoom = 1.5;
        const double cursorX = 200, cursorY = 90;

        var (ox1, oy1, z1) = ZoomMath.ZoomAtPoint(
            offsetX, offsetY, zoom, cursorX, cursorY, 1.25, MinZoom, MaxZoom);
        var (ox2, oy2, z2) = ZoomMath.ZoomAtPoint(
            ox1, oy1, z1, cursorX, cursorY, 1.0 / 1.25, MinZoom, MaxZoom);

        Assert.Equal(zoom, z2, Tolerance);
        Assert.Equal(offsetX, ox2, Tolerance);
        Assert.Equal(offsetY, oy2, Tolerance);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    public void ZoomAtPoint_NonPositiveZoom_Throws(double zoom)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ZoomMath.ZoomAtPoint(0, 0, zoom, 0, 0, 1.1, MinZoom, MaxZoom));
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-0.5)]
    public void ZoomAtPoint_NonPositiveFactor_Throws(double factor)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ZoomMath.ZoomAtPoint(0, 0, 1.0, 0, 0, factor, MinZoom, MaxZoom));
    }
}
