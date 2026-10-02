// src/Wc3.Render/ModelRenderer.cs
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Wc3.Modeling;

namespace Wc3.Render;

/// <summary>
/// Headless CPU rasterizer for models: z-buffered triangle fill with
/// directional-light shading and nearest-neighbour texturing. No GPU/OpenGL —
/// runs anywhere the CLI does. Models with a parsed skeleton render posed at
/// the first frame of their "Stand" sequence (bind pose when there is no usable
/// skeleton), from an orbit camera with WC3's Z-up convention (models stand
/// upright); the default angles give the classic 3/4 view.
/// </summary>
public static class ModelRenderer
{
    // Camera orbit for the default 3/4 view: yaw around the Z (up) axis, then pitch down.
    public const float DefaultYawDegrees = 45f;
    public const float DefaultPitchDegrees = 30f;
    public const float DefaultZoom = 1f;
    private const float FillFraction = 0.9f;    // model's share of the frame at zoom 1
    private const float AmbientLight = 0.3f;    // shading floor so backfaces stay visible
    private const byte AlphaTestThreshold = 128; // cutout transparency (hair, foliage)
    private const float DistanceFactor = 3.5f;  // eye distance in model radii → mild perspective

    /// <summary>
    /// Renders <paramref name="model"/> to PNG bytes over a transparent background.
    /// <paramref name="textures"/> maps indices of <see cref="Model3D.Textures"/> to
    /// decoded images; geosets whose texture is absent shade flat gray (covers
    /// ReplaceableId entries and textures the map does not contain).
    /// <paramref name="yawDegrees"/>/<paramref name="pitchDegrees"/> orbit the camera
    /// around Z-up; pitch is clamped just short of the poles to keep the basis stable.
    /// <paramref name="zoom"/> magnifies the view (1 = fit-to-frame). Framing is derived
    /// from the model's bounding sphere so the model holds a steady size as it rotates,
    /// and a mild perspective (with perspective-correct interpolation) gives real depth.
    /// <paramref name="sequenceName"/> selects the animation pose ("Stand" by default;
    /// see <see cref="Model3D.PosedAt"/> for the match rules); pass null to force the
    /// bind pose. Models without a usable skeleton always render in bind pose.
    /// </summary>
    public static byte[] RenderPng(
        Model3D model,
        IReadOnlyDictionary<int, TextureImage> textures,
        int width = 512,
        int height = 512,
        float yawDegrees = DefaultYawDegrees,
        float pitchDegrees = DefaultPitchDegrees,
        float zoom = DefaultZoom,
        string? sequenceName = "Stand")
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(textures);
        if (width <= 0 || height <= 0)
            throw new ArgumentOutOfRangeException(nameof(width), "image dimensions must be positive");

        if (sequenceName is not null && model.Skeleton is not null)
            model = model.PosedAt(sequenceName); // never throws; bind pose on any failure

        var geosets = model.Geosets.Where(g => g.Indices.Length >= 3 && g.Vertices.Length >= 9).ToList();
        if (geosets.Count == 0)
            throw new InvalidDataException("model has no renderable geometry (no geosets with triangles)");

        // Camera basis from yaw/pitch on the unit sphere (Z-up). A pitch at ±90° would
        // make forward parallel to Z and the right-vector cross product degenerate, so
        // stop just short of the poles.
        float yaw = yawDegrees * MathF.PI / 180f;
        float pitch = Math.Clamp(pitchDegrees, -89.9f, 89.9f) * MathF.PI / 180f;
        var eyeDir = new Vector3(
            MathF.Cos(pitch) * MathF.Cos(yaw),
            MathF.Cos(pitch) * MathF.Sin(yaw),
            MathF.Sin(pitch));
        var forward = -eyeDir;
        var right = Vector3.Normalize(Vector3.Cross(forward, Vector3.UnitZ));
        var up = Vector3.Cross(right, forward);

        // Bounding sphere (view-independent) → a stable frame: the model keeps its size
        // no matter how it is spun. Center on the box midpoint; radius covers every vertex.
        Vector3 min = new(float.MaxValue), max = new(float.MinValue);
        foreach (var g in geosets)
            for (int i = 0; i + 2 < g.Vertices.Length; i += 3)
            {
                var p = new Vector3(g.Vertices[i], g.Vertices[i + 1], g.Vertices[i + 2]);
                min = Vector3.Min(min, p);
                max = Vector3.Max(max, p);
            }
        var center = (min + max) / 2f;
        float radius = 1e-6f;
        foreach (var g in geosets)
            for (int i = 0; i + 2 < g.Vertices.Length; i += 3)
                radius = MathF.Max(radius,
                    (new Vector3(g.Vertices[i], g.Vertices[i + 1], g.Vertices[i + 2]) - center).Length());

        // Eye pulled back along the view direction; focal chosen so the sphere fills the
        // frame at zoom 1. Perspective divide uses the per-vertex distance along forward.
        float dist = radius * DistanceFactor;
        var eye = center + eyeDir * dist;
        float focal = FillFraction * 0.5f * MathF.Min(width, height) * dist / radius * zoom;

        // Project every vertex once: screen x/y (perspective) + inverse depth for
        // perspective-correct attribute interpolation, + view depth for the z-buffer.
        var sx = new Vector2[geosets.Count][];
        var invW = new float[geosets.Count][];
        var depth = new float[geosets.Count][];
        for (int gi = 0; gi < geosets.Count; gi++)
        {
            var v = geosets[gi].Vertices;
            int n = v.Length / 3;
            var scr = new Vector2[n];
            var iw = new float[n];
            var dep = new float[n];
            for (int i = 0; i < n; i++)
            {
                var rel = new Vector3(v[i * 3], v[i * 3 + 1], v[i * 3 + 2]) - eye;
                float vx = Vector3.Dot(rel, right);
                float vy = Vector3.Dot(rel, up);
                float vz = MathF.Max(Vector3.Dot(rel, forward), 1e-4f); // distance in front of eye
                scr[i] = new Vector2(width / 2f + vx / vz * focal, height / 2f - vy / vz * focal);
                iw[i] = 1f / vz;
                dep[i] = vz;
            }
            sx[gi] = scr; invW[gi] = iw; depth[gi] = dep;
        }

        // Light biased toward the camera and above-left so the visible side reads.
        var light = Vector3.Normalize(eyeDir + 0.6f * up - 0.4f * right);

        var zbuffer = new float[width * height];
        Array.Fill(zbuffer, float.MaxValue);
        using var image = new Image<Rgba32>(width, height); // zeroed → transparent

        for (int gi = 0; gi < geosets.Count; gi++)
        {
            var geoset = geosets[gi];
            var scr = sx[gi];
            var iw = invW[gi];
            var dep = depth[gi];
            textures.TryGetValue(geoset.TextureId, out var texture);

            for (int t = 0; t + 2 < geoset.Indices.Length; t += 3)
            {
                int i0 = geoset.Indices[t], i1 = geoset.Indices[t + 1], i2 = geoset.Indices[t + 2];
                float ax = scr[i0].X, ay = scr[i0].Y;
                float bx = scr[i1].X, by = scr[i1].Y;
                float cx = scr[i2].X, cy = scr[i2].Y;

                // Signed doubled area; near-zero → degenerate sliver, skip.
                float areaFull = (bx - ax) * (cy - ay) - (by - ay) * (cx - ax);
                if (MathF.Abs(areaFull) < 1e-9f) continue;

                int pxMin = Math.Max(0, (int)MathF.Floor(MathF.Min(ax, MathF.Min(bx, cx))));
                int pxMax = Math.Min(width - 1, (int)MathF.Ceiling(MathF.Max(ax, MathF.Max(bx, cx))));
                int pyMin = Math.Max(0, (int)MathF.Floor(MathF.Min(ay, MathF.Min(by, cy))));
                int pyMax = Math.Min(height - 1, (int)MathF.Ceiling(MathF.Max(ay, MathF.Max(by, cy))));
                if (pxMin > pxMax || pyMin > pyMax) continue;

                float iw0 = iw[i0], iw1 = iw[i1], iw2 = iw[i2];
                for (int py = pyMin; py <= pyMax; py++)
                {
                    float py5 = py + 0.5f;
                    for (int px = pxMin; px <= pxMax; px++)
                    {
                        float px5 = px + 0.5f;
                        // Screen-space barycentric weights (winding-independent inside test —
                        // no backface culling, WC3 materials are frequently two-sided).
                        float w0 = ((bx - px5) * (cy - py5) - (by - py5) * (cx - px5)) / areaFull;
                        float w1 = ((cx - px5) * (ay - py5) - (cy - py5) * (ax - px5)) / areaFull;
                        float w2 = 1f - w0 - w1;
                        if (w0 < 0f || w1 < 0f || w2 < 0f) continue;

                        // Perspective-correct weights (divide by interpolated 1/z) so
                        // textures don't warp across large near-far triangles.
                        float denom = w0 * iw0 + w1 * iw1 + w2 * iw2;
                        if (denom < 1e-12f) continue;
                        float p0 = w0 * iw0 / denom, p1 = w1 * iw1 / denom, p2 = w2 * iw2 / denom;

                        float d = p0 * dep[i0] + p1 * dep[i1] + p2 * dep[i2];
                        int zi = py * width + px;
                        if (d >= zbuffer[zi]) continue;

                        var rgba = SampleColor(geoset, texture, i0, i1, i2, p0, p1, p2);
                        if (rgba.A < AlphaTestThreshold) continue; // cutout: keep depth open

                        float shade = ShadeAt(geoset, i0, i1, i2, p0, p1, p2, light);
                        zbuffer[zi] = d;
                        image[px, py] = new Rgba32(
                            (byte)(rgba.R * shade), (byte)(rgba.G * shade), (byte)(rgba.B * shade), 255);
                    }
                }
            }
        }

        using var ms = new MemoryStream();
        image.SaveAsPng(ms);
        return ms.ToArray();
    }

    /// <summary>Nearest-neighbour texture sample at the interpolated UV, or flat gray.</summary>
    private static Rgba32 SampleColor(
        Geoset geoset, TextureImage? texture, int i0, int i1, int i2, float w0, float w1, float w2)
    {
        if (texture is null) return new Rgba32(190, 190, 190, 255);

        float u = w0 * geoset.Uvs[i0 * 2] + w1 * geoset.Uvs[i1 * 2] + w2 * geoset.Uvs[i2 * 2];
        float v = w0 * geoset.Uvs[i0 * 2 + 1] + w1 * geoset.Uvs[i1 * 2 + 1] + w2 * geoset.Uvs[i2 * 2 + 1];
        u -= MathF.Floor(u); // WC3 UVs wrap
        v -= MathF.Floor(v);

        int tx = Math.Clamp((int)(u * texture.Width), 0, texture.Width - 1);
        int ty = Math.Clamp((int)(v * texture.Height), 0, texture.Height - 1);
        int o = (ty * texture.Width + tx) * 4;
        return new Rgba32(texture.Rgba[o], texture.Rgba[o + 1], texture.Rgba[o + 2], texture.Rgba[o + 3]);
    }

    /// <summary>Directional-light intensity from the interpolated normal, with an ambient floor.</summary>
    private static float ShadeAt(
        Geoset geoset, int i0, int i1, int i2, float w0, float w1, float w2, Vector3 light)
    {
        var n = new Vector3(
            w0 * geoset.Normals[i0 * 3] + w1 * geoset.Normals[i1 * 3] + w2 * geoset.Normals[i2 * 3],
            w0 * geoset.Normals[i0 * 3 + 1] + w1 * geoset.Normals[i1 * 3 + 1] + w2 * geoset.Normals[i2 * 3 + 1],
            w0 * geoset.Normals[i0 * 3 + 2] + w1 * geoset.Normals[i1 * 3 + 2] + w2 * geoset.Normals[i2 * 3 + 2]);
        float lengthSq = n.LengthSquared();
        if (lengthSq < 1e-12f) return 1f; // missing/zero normals: render unshaded

        // |dot|: two-sided lighting so form reads regardless of winding.
        float intensity = MathF.Abs(Vector3.Dot(n / MathF.Sqrt(lengthSq), light));
        return AmbientLight + (1f - AmbientLight) * MathF.Min(intensity, 1f);
    }
}
