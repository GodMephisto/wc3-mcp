// src/Wc3.Commands/ZoomMath.cs
namespace Wc3.Commands;

/// <summary>Pure viewport math for panels that pan/zoom fixed-size content (e.g. a rendered terrain bitmap).</summary>
public static class ZoomMath
{
    /// <summary>
    /// Zoom-to-cursor: multiplies <paramref name="zoom"/> by <paramref name="zoomFactor"/> (clamped to
    /// [<paramref name="minZoom"/>, <paramref name="maxZoom"/>]) and adjusts the offsets so the content
    /// point currently under the cursor stays under the cursor.
    ///
    /// <para><b>Convention (matches TerrainView):</b> the content is laid out at <c>contentPoint * zoom</c>
    /// layout pixels (TerrainView sizes its Image to <c>bitmapPixelSize * zoom</c>), and
    /// <paramref name="offsetX"/>/<paramref name="offsetY"/> are ScrollViewer <b>scroll offsets</b> — the
    /// layout-pixel distance the viewport has been scrolled right/down. A content point <c>p</c> is visible
    /// at viewport position <c>p * zoom - offset</c>, so the content point under the cursor is
    /// <c>(offset + cursor) / zoom</c> and the invariant-preserving new offset is
    /// <c>newOffset = (offset + cursor) * (newZoom / zoom) - cursor</c>.</para>
    ///
    /// <para>This is the standard translate-transform formula
    /// <c>newOffset = cursor - (cursor - oldOffset) * (newZoom / oldZoom)</c> with the offset sign flipped
    /// (a scroll offset is the negated translation). If the clamped zoom equals the current zoom the
    /// offsets are returned unchanged.</para>
    ///
    /// <para>Offsets are not clamped to the content extent — the ScrollViewer coerces its own Offset.</para>
    /// </summary>
    /// <param name="offsetX">Current horizontal scroll offset in layout pixels.</param>
    /// <param name="offsetY">Current vertical scroll offset in layout pixels.</param>
    /// <param name="zoom">Current zoom (layout pixels per content pixel); must be positive.</param>
    /// <param name="cursorX">Cursor X relative to the viewport, in layout pixels.</param>
    /// <param name="cursorY">Cursor Y relative to the viewport, in layout pixels.</param>
    /// <param name="zoomFactor">Multiplier applied to <paramref name="zoom"/>; must be positive.</param>
    /// <param name="minZoom">Inclusive lower zoom bound.</param>
    /// <param name="maxZoom">Inclusive upper zoom bound.</param>
    /// <returns>The adjusted scroll offsets and the clamped new zoom.</returns>
    public static (double OffsetX, double OffsetY, double Zoom) ZoomAtPoint(
        double offsetX, double offsetY, double zoom,
        double cursorX, double cursorY,
        double zoomFactor, double minZoom, double maxZoom)
    {
        if (zoom <= 0)
            throw new ArgumentOutOfRangeException(nameof(zoom), zoom, "Zoom must be positive.");
        if (zoomFactor <= 0)
            throw new ArgumentOutOfRangeException(nameof(zoomFactor), zoomFactor, "Zoom factor must be positive.");

        var newZoom = Math.Clamp(zoom * zoomFactor, minZoom, maxZoom);
        var scale = newZoom / zoom;
        return (
            OffsetX: (offsetX + cursorX) * scale - cursorX,
            OffsetY: (offsetY + cursorY) * scale - cursorY,
            Zoom: newZoom);
    }
}
