// src/Wc3.Commands/ModelPreviewCommand.cs
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Wc3.Modeling;

namespace Wc3.Commands;

/// <summary>
/// Lightweight static model preview: MDX/MDL bytes → orthographic wireframe PNG.
/// This is explicitly a first cut, NOT a full render — no textures, no skeleton or
/// animation (bind pose only), no lighting, no perspective, no hidden-line removal.
/// The bind-pose triangle edges are projected with a fixed 3/4 orthographic view
/// (45° yaw around +Z — WC3 models are Z-up — then a 30° downward pitch), uniformly
/// scaled to fit the canvas with padding, and drawn as a wireframe on a solid
/// background. Empty, degenerate, or unparseable input degrades to a small
/// placeholder PNG (background + diagonal cross); <see cref="RenderPreview(byte[], int, int)"/>
/// never throws. Output is deterministic for identical input (no timestamps, no
/// randomness).
/// </summary>
public static class ModelPreviewCommand
{
    /// <summary>Fraction of the smaller canvas side left as padding on each edge.</summary>
    private const float PaddingFraction = 0.05f;

    /// <summary>Fixed 3/4 view: yaw around +Z (WC3's up axis), then downward pitch.</summary>
    private const float YawDegrees = 45f;
    private const float PitchDegrees = 30f;

    private static readonly Rgba32 Background = new(24, 26, 32, 255);
    private static readonly Rgba32 Wire = new(140, 200, 255, 255);
    private static readonly Rgba32 PlaceholderMark = new(90, 96, 110, 255);

    /// <summary>
    /// Renders a static wireframe preview of a WC3 model. <paramref name="modelBytes"/>
    /// is dispatched by content: "MDLX" magic → binary MDX, otherwise text MDL.
    /// Never throws: null/empty/unparseable bytes or a model with no drawable
    /// triangles return a placeholder PNG of the requested size instead.
    /// </summary>
    public static byte[] RenderPreview(byte[] modelBytes, int width = 512, int height = 512)
    {
        (width, height) = ClampSize(width, height);

        Model3D? model = null;
        try
        {
            if (modelBytes is { Length: > 0 })
                // The parser checks MDLX magic first; the .mdl name routes the
                // remaining (text) case to the MDL reader.
                model = ModelParser.Parse(modelBytes, "preview.mdl");
        }
        catch
        {
            model = null; // unparseable → placeholder
        }

        return model is null ? PlaceholderPng(width, height) : RenderPreview(model, width, height);
    }

    /// <summary>Renders an already-parsed model. Internal seam for hermetic tests.</summary>
    internal static byte[] RenderPreview(Model3D model, int width, int height)
    {
        (width, height) = ClampSize(width, height);
        try
        {
            var (vertices, indices) = FlattenGeosets(model);
            if (indices.Length < 3)
                return PlaceholderPng(width, height);

            var xy = ProjectOrthographic(vertices);
            if (!ScaleToFit(xy, width, height, PaddingFraction))
                return PlaceholderPng(width, height);

            using var image = new Image<Rgba32>(width, height, Background);
            for (int i = 0; i + 2 < indices.Length; i += 3)
            {
                int a = indices[i] * 2, b = indices[i + 1] * 2, c = indices[i + 2] * 2;
                DrawLine(image, xy[a], xy[a + 1], xy[b], xy[b + 1], Wire);
                DrawLine(image, xy[b], xy[b + 1], xy[c], xy[c + 1], Wire);
                DrawLine(image, xy[c], xy[c + 1], xy[a], xy[a + 1], Wire);
            }
            return EncodePng(image);
        }
        catch
        {
            return PlaceholderPng(width, height); // any geometry surprise degrades, never throws
        }
    }

    /// <summary>
    /// Concatenates every geoset's triangle list into one packed x,y,z vertex array
    /// with offset-adjusted indices. Triangles with out-of-range indices or with any
    /// non-finite vertex coordinate are dropped. (First-cut simplification: vertices
    /// of dropped triangles still participate in the bounding box.)
    /// </summary>
    internal static (float[] Vertices, int[] Indices) FlattenGeosets(Model3D model)
    {
        var verts = new List<float>();
        var idx = new List<int>();
        foreach (var g in model.Geosets)
        {
            if (g.Vertices is not { Length: >= 3 } || g.Indices is not { Length: >= 3 })
                continue;

            int vertexCount = g.Vertices.Length / 3;
            int baseIndex = verts.Count / 3;
            verts.AddRange(g.Vertices[..(vertexCount * 3)]); // ignore a trailing partial triple

            bool Finite(int vi) =>
                float.IsFinite(g.Vertices[vi * 3])
                && float.IsFinite(g.Vertices[vi * 3 + 1])
                && float.IsFinite(g.Vertices[vi * 3 + 2]);

            for (int i = 0; i + 2 < g.Indices.Length; i += 3)
            {
                int a = g.Indices[i], b = g.Indices[i + 1], c = g.Indices[i + 2];
                if ((uint)a >= (uint)vertexCount || (uint)b >= (uint)vertexCount || (uint)c >= (uint)vertexCount)
                    continue;
                if (!Finite(a) || !Finite(b) || !Finite(c))
                    continue;
                idx.Add(baseIndex + a);
                idx.Add(baseIndex + b);
                idx.Add(baseIndex + c);
            }
        }
        return (verts.ToArray(), idx.ToArray());
    }

    /// <summary>
    /// Projects packed x,y,z model-space vertices to packed 2D plane coordinates with
    /// the fixed 3/4 orthographic view: yaw <see cref="YawDegrees"/> around +Z, then
    /// pitch <see cref="PitchDegrees"/> tilting the camera down; depth is discarded.
    /// Orthographic, so Project(k·v) = k·Project(v). +Y is "up" in the returned plane
    /// (<see cref="ScaleToFit"/> flips it to raster rows).
    /// </summary>
    internal static float[] ProjectOrthographic(float[] xyz)
    {
        float yaw = YawDegrees * MathF.PI / 180f;
        float pitch = PitchDegrees * MathF.PI / 180f;
        float cy = MathF.Cos(yaw), sy = MathF.Sin(yaw);
        float cp = MathF.Cos(pitch), sp = MathF.Sin(pitch);

        int count = xyz.Length / 3;
        var xy = new float[count * 2];
        for (int v = 0; v < count; v++)
        {
            float x = xyz[v * 3], y = xyz[v * 3 + 1], z = xyz[v * 3 + 2];
            float x1 = x * cy - y * sy;          // yaw around +Z
            float y1 = x * sy + y * cy;          // depth axis after yaw
            xy[v * 2] = x1;                      // screen horizontal
            xy[v * 2 + 1] = z * cp - y1 * sp;    // screen up after pitch; depth dropped
        }
        return xy;
    }

    /// <summary>
    /// Uniformly scales and centers packed 2D points (Y-up) in place into pixel
    /// coordinates (Y-down raster) inside <paramref name="width"/>×<paramref name="height"/>,
    /// keeping <paramref name="paddingFraction"/> of the smaller side clear on each
    /// edge. Aspect ratio is preserved. A zero-extent cloud (single point, or all
    /// points coincident) collapses to the canvas center; non-finite points are
    /// excluded from the fit and pinned to the center. Returns false when no finite
    /// point exists (nothing drawable).
    /// </summary>
    internal static bool ScaleToFit(float[] xy, int width, int height, float paddingFraction)
    {
        float minX = float.PositiveInfinity, minY = float.PositiveInfinity;
        float maxX = float.NegativeInfinity, maxY = float.NegativeInfinity;
        for (int i = 0; i + 1 < xy.Length; i += 2)
        {
            float x = xy[i], y = xy[i + 1];
            if (!float.IsFinite(x) || !float.IsFinite(y)) continue;
            if (x < minX) minX = x;
            if (x > maxX) maxX = x;
            if (y < minY) minY = y;
            if (y > maxY) maxY = y;
        }
        if (minX > maxX)
            return false; // no finite points

        float pad = MathF.Min(width, height) * paddingFraction;
        float availW = MathF.Max(1f, width - 2f * pad);
        float availH = MathF.Max(1f, height - 2f * pad);
        float extentX = maxX - minX, extentY = maxY - minY;
        float scaleX = extentX > 0f ? availW / extentX : float.PositiveInfinity;
        float scaleY = extentY > 0f ? availH / extentY : float.PositiveInfinity;
        float scale = MathF.Min(scaleX, scaleY);
        if (!float.IsFinite(scale)) scale = 0f; // point-like model → everything at center

        float cx = (minX + maxX) / 2f, cyv = (minY + maxY) / 2f;
        float ox = width / 2f, oy = height / 2f;
        for (int i = 0; i + 1 < xy.Length; i += 2)
        {
            float x = xy[i], y = xy[i + 1];
            if (!float.IsFinite(x) || !float.IsFinite(y))
            {
                xy[i] = ox;
                xy[i + 1] = oy;
                continue;
            }
            xy[i] = ox + (x - cx) * scale;
            xy[i + 1] = oy - (y - cyv) * scale; // flip: plane Y-up → raster Y-down
        }
        return true;
    }

    /// <summary>Solid background plus a diagonal cross — the "no geometry" marker.</summary>
    private static byte[] PlaceholderPng(int width, int height)
    {
        using var image = new Image<Rgba32>(width, height, Background);
        DrawLine(image, 0, 0, width - 1, height - 1, PlaceholderMark);
        DrawLine(image, 0, height - 1, width - 1, 0, PlaceholderMark);
        return EncodePng(image);
    }

    /// <summary>Integer Bresenham; pixels outside the canvas are clipped per-pixel.</summary>
    private static void DrawLine(Image<Rgba32> img, float x0f, float y0f, float x1f, float y1f, Rgba32 color)
    {
        int x0 = (int)MathF.Round(x0f), y0 = (int)MathF.Round(y0f);
        int x1 = (int)MathF.Round(x1f), y1 = (int)MathF.Round(y1f);
        int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = -Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = dx + dy;
        while (true)
        {
            if ((uint)x0 < (uint)img.Width && (uint)y0 < (uint)img.Height)
                img[x0, y0] = color;
            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; }
            if (e2 <= dx) { err += dx; y0 += sy; }
        }
    }

    private static byte[] EncodePng(Image<Rgba32> image)
    {
        using var ms = new MemoryStream();
        image.SaveAsPng(ms);
        return ms.ToArray();
    }

    private static (int Width, int Height) ClampSize(int width, int height) =>
        (Math.Clamp(width, 1, 4096), Math.Clamp(height, 1, 4096));
}
