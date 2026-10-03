// src/Wc3.Render/TerrainRenderer.cs
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using War3Net.Build.Environment;

namespace Wc3.Render;

public static class TerrainRenderer
{
    // Keep output viewable without being huge: integer nearest-neighbor upscale
    // until the longer side would exceed this.
    private const int MaxSide = 1024;

    /// <summary>
    /// Renders a top-down colored terrain map and returns it as PNG bytes.
    /// Each tilepoint is colored by its ground tile type (grass/dirt/rock/…),
    /// shaded by elevation, outlined at cliff steps, and tinted for water and
    /// blight. North is up (war3map.w3e stores tilepoint rows south-to-north,
    /// so rows are flipped). The caller (CLI) writes the bytes to disk.
    /// </summary>
    public static byte[] RenderTerrainPng(Wc3.Model.MapDocument doc)
    {
        if (doc.GetFile("war3map.w3e")?.Model is not MapEnvironment env)
            throw new InvalidDataException("no terrain (war3map.w3e) in map");

        // Width/Height count tiles; the tilepoint grid has one more point per axis.
        int w = (int)env.Width + 1;
        int h = (int)env.Height + 1;
        var tiles = env.TerrainTiles;
        if (w <= 0 || h <= 0 || tiles.Count != w * h)
            throw new InvalidDataException($"terrain grid {w}x{h} does not match {tiles.Count} tilepoints");

        // Precompute one base color per ground tile type so we only parse each
        // enum name once (a map has at most a handful of ground types). Blank
        // maps have an empty TerrainTypes list, so fall back to a neutral color.
        var typeColors = BuildTypeColors(doc, env);
        var fallback = new Rgba32(96, 108, 84); // muted grass-green

        // TerrainTile.Height excludes cliff level; one cliff step equals 1.0 in
        // War3Net's normalized height units. Combine them for elevation shading.
        var elevation = new float[tiles.Count];
        float min = float.MaxValue, max = float.MinValue;
        for (int i = 0; i < tiles.Count; i++)
        {
            float e = tiles[i].Height + tiles[i].CliffLevel;
            elevation[i] = e;
            if (e < min) min = e;
            if (e > max) max = e;
        }
        float range = max - min;

        int scale = Math.Max(1, MaxSide / Math.Max(w, h));
        using var image = new Image<Rgba32>(w * scale, h * scale);
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int idx = y * w + x;
                var tile = tiles[idx];

                // A boundary tile is outside the playable area. The game does not draw it and
                // its own minimap shows it as void, so painting it as ground was what made a
                // rendered map read as "not on a proper map", with real terrain marooned in a
                // field of dirt. Measured on Anime_WOS2_0.29d, a tile inside the arena carries
                // IsBoundary false and grass, while the surrounding field carries IsBoundary
                // true and dirt. This is the flag, not fog of war, which was the first guess.
                if (tile.IsBoundary)
                {
                    Paint(image, x, y, h, scale, VoidColor);
                    continue;
                }

                // Base color from the tile's ground type.
                Rgba32 c;
                if (typeColors.Length > 0 && tile.Texture >= 0 && tile.Texture < typeColors.Length)
                    c = typeColors[tile.Texture];
                else
                    c = fallback;

                // Elevation shading: darker in valleys, brighter on peaks.
                float n = range > 0 ? (elevation[idx] - min) / range : 0.5f;
                float shade = 0.65f + 0.55f * n;

                // Cliff outline: darken a tile whose east/south neighbor sits at a
                // different cliff level, so cliff faces read as crisp edges.
                int cl = tile.CliffLevel;
                bool edge =
                    (x + 1 < w && tiles[idx + 1].CliffLevel != cl) ||
                    (y + 1 < h && tiles[idx + w].CliffLevel != cl);
                if (edge) shade *= 0.72f;

                c = Scale(c, shade);

                // Water: blend toward blue; deeper ground under the water is darker.
                if (tile.IsWater)
                {
                    var deep = new Rgba32(28, 52, 104);
                    var shallow = new Rgba32(70, 116, 176);
                    var water = Lerp(deep, shallow, n);
                    c = Lerp(c, water, 0.68f);
                }

                // Blight: cracked reddish earth overlay.
                if (tile.IsBlighted)
                    c = Lerp(c, new Rgba32(104, 66, 54), 0.7f);

                Paint(image, x, y, h, scale, c);
            }
        }

        using var ms = new MemoryStream();
        image.SaveAsPng(ms);
        return ms.ToArray();
    }

    /// <summary>
    /// Renders the terrain heightmap in 3D perspective (WC3 Z-up) — the
    /// "rendered in game" look versus the flat top-down <see cref="RenderTerrainPng"/>
    /// minimap. The tilepoint grid becomes a directional-lit, per-terrain-type
    /// coloured mesh at real ground+cliff heights; water tiles get a translucent
    /// plane. Orbit camera: <paramref name="yawDegrees"/> spins around Z (up),
    /// <paramref name="pitchDegrees"/> tilts down, <paramref name="zoom"/> scales
    /// camera distance (larger = closer). Pure software rasterizer — no GPU.
    /// </summary>
    public static byte[] RenderPerspectivePng(
        Wc3.Model.MapDocument doc, int width, int height,
        float yawDegrees = 45f, float pitchDegrees = 30f, float zoom = 1f)
    {
        if (doc.GetFile("war3map.w3e")?.Model is not MapEnvironment env)
            throw new InvalidDataException("no terrain (war3map.w3e) in map");

        width = Math.Clamp(width, 16, 4096);
        height = Math.Clamp(height, 16, 4096);

        int w = (int)env.Width + 1;    // tilepoint grid dimensions (tiles + 1 per axis)
        int h = (int)env.Height + 1;
        var tiles = env.TerrainTiles;
        if (tiles is null || tiles.Count < w * h)
            throw new InvalidDataException("terrain tilepoint data is incomplete");

        var typeColors = BuildTypeColors(doc, env);

        const float TileWorld = TerrainTransform.TileWorld; // 128 world units per tile
        const float StepWorld = 128f;                        // one cliff step in world Z
        float originX = -(int)env.Width * (TileWorld / 2f);
        float originY = -(int)env.Height * (TileWorld / 2f);

        // Tilepoint vertices in world space (Z-up). Height + CliffLevel is in
        // cliff-step units; one step is 128 world units tall.
        int n = w * h;
        var verts = new Vector3[n];
        float minZ = float.MaxValue, maxZ = float.MinValue;
        double waterSum = 0; int waterCount = 0;
        for (int j = 0; j < h; j++)
            for (int i = 0; i < w; i++)
            {
                var t = tiles[j * w + i];
                float z = (t.Height + t.CliffLevel) * StepWorld;
                verts[j * w + i] = new Vector3(originX + i * TileWorld, originY + j * TileWorld, z);
                if (z < minZ) minZ = z;
                if (z > maxZ) maxZ = z;
                if (t.IsWater) { waterSum += z; waterCount++; }
            }
        float waterZ = waterCount > 0 ? (float)(waterSum / waterCount) + 0.35f * StepWorld : minZ;

        // Orbit camera around the scene centre; distance derived from bounds / zoom.
        var center = new Vector3(0f, 0f, (minZ + maxZ) * 0.5f);
        float span = MathF.Max(w * TileWorld, h * TileWorld);
        float radius = MathF.Max(span, maxZ - minZ) * 0.5f + TileWorld;

        float yaw = yawDegrees * (MathF.PI / 180f);
        float pitch = Math.Clamp(pitchDegrees, 2f, 89f) * (MathF.PI / 180f);
        var eyeDir = new Vector3(
            MathF.Cos(pitch) * MathF.Cos(yaw),
            MathF.Cos(pitch) * MathF.Sin(yaw),
            MathF.Sin(pitch));
        float dist = radius / MathF.Max(zoom, 0.05f) * 2.4f;
        var eye = center + eyeDir * dist;
        var forward = Vector3.Normalize(center - eye);
        var right = Vector3.Normalize(Vector3.Cross(forward, Vector3.UnitZ));
        var up = Vector3.Cross(right, forward);
        int W = width, H = height;
        float focal = 0.5f * H / MathF.Tan(0.5f * (48f * MathF.PI / 180f)); // 48° vertical FOV
        var light = Vector3.Normalize(eyeDir + 0.5f * Vector3.UnitZ - 0.3f * right + 0.15f * up);
        const float AmbientFloor = 0.35f;

        // Screen buffers.
        var col = new Rgba32[W * H];
        var zbuf = new float[W * H];
        var sky = new Rgba32((byte)28, (byte)32, (byte)38, (byte)255);
        for (int i = 0; i < col.Length; i++) { col[i] = sky; zbuf[i] = float.MaxValue; }

        // Project every tilepoint once (terrain), and the water plane if needed.
        var pv = new (float sx, float sy, float vz)[n];
        for (int i = 0; i < n; i++) pv[i] = Project(verts[i]);
        (float sx, float sy, float vz)[]? pw = null;
        if (waterCount > 0)
        {
            pw = new (float sx, float sy, float vz)[n];
            for (int j = 0; j < h; j++)
                for (int i = 0; i < w; i++)
                    pw[j * w + i] = Project(new Vector3(originX + i * TileWorld, originY + j * TileWorld, waterZ));
        }

        // Terrain pass: two triangles per cell, flat-shaded by face normal.
        for (int j = 0; j < h - 1; j++)
            for (int i = 0; i < w - 1; i++)
            {
                int a = j * w + i, b = a + 1, c = a + w, d = c + 1;
                var tile = tiles[j * w + i];
                Rgba32 baseCol = (tile.Texture >= 0 && tile.Texture < typeColors.Length)
                    ? typeColors[tile.Texture] : new Rgba32((byte)120, (byte)120, (byte)120, (byte)255);
                Raster(pv[a], pv[c], pv[b], Shade(baseCol, FaceShade(verts[a], verts[c], verts[b])), 1f, true);
                Raster(pv[b], pv[c], pv[d], Shade(baseCol, FaceShade(verts[b], verts[c], verts[d])), 1f, true);
            }

        // Water pass: translucent plane over water tiles (z-tested, no z-write).
        if (pw is not null)
        {
            var wcol = new Rgba32((byte)48, (byte)96, (byte)168, (byte)255);
            for (int j = 0; j < h - 1; j++)
                for (int i = 0; i < w - 1; i++)
                {
                    if (!tiles[j * w + i].IsWater) continue;
                    int a = j * w + i, b = a + 1, c = a + w, d = c + 1;
                    Raster(pw[a], pw[c], pw[b], wcol, 0.55f, false);
                    Raster(pw[b], pw[c], pw[d], wcol, 0.55f, false);
                }
        }

        var image = new Image<Rgba32>(W, H);
        for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
                image[x, y] = col[y * W + x];
        using var ms = new MemoryStream();
        image.SaveAsPng(ms);
        return ms.ToArray();

        // --- locals -------------------------------------------------------
        (float sx, float sy, float vz) Project(Vector3 p)
        {
            var rel = p - eye;
            float vz = MathF.Max(Vector3.Dot(rel, forward), 1e-4f); // in front of eye
            float vx = Vector3.Dot(rel, right);
            float vy = Vector3.Dot(rel, up);
            return (W * 0.5f + vx / vz * focal, H * 0.5f - vy / vz * focal, vz);
        }

        float FaceShade(Vector3 v0, Vector3 v1, Vector3 v2)
        {
            var nrm = Vector3.Cross(v1 - v0, v2 - v0);
            float len = nrm.Length();
            if (len < 1e-6f) return 1f;
            float d = MathF.Abs(Vector3.Dot(nrm / len, light));
            return AmbientFloor + (1f - AmbientFloor) * MathF.Min(d, 1f);
        }

        static Rgba32 Shade(Rgba32 c, float s) => new(
            (byte)MathF.Min(c.R * s, 255f), (byte)MathF.Min(c.G * s, 255f),
            (byte)MathF.Min(c.B * s, 255f), (byte)255);

        void Raster((float sx, float sy, float vz) p0, (float sx, float sy, float vz) p1,
                    (float sx, float sy, float vz) p2, Rgba32 c, float alpha, bool writeZ)
        {
            float area = (p1.sx - p0.sx) * (p2.sy - p0.sy) - (p1.sy - p0.sy) * (p2.sx - p0.sx);
            if (MathF.Abs(area) < 1e-6f) return;
            float inv = 1f / area;
            int xmin = Math.Max(0, (int)MathF.Floor(MathF.Min(p0.sx, MathF.Min(p1.sx, p2.sx))));
            int xmax = Math.Min(W - 1, (int)MathF.Ceiling(MathF.Max(p0.sx, MathF.Max(p1.sx, p2.sx))));
            int ymin = Math.Max(0, (int)MathF.Floor(MathF.Min(p0.sy, MathF.Min(p1.sy, p2.sy))));
            int ymax = Math.Min(H - 1, (int)MathF.Ceiling(MathF.Max(p0.sy, MathF.Max(p1.sy, p2.sy))));
            for (int py = ymin; py <= ymax; py++)
                for (int px = xmin; px <= xmax; px++)
                {
                    float fx = px + 0.5f, fy = py + 0.5f;
                    float b0 = ((p2.sx - p1.sx) * (fy - p1.sy) - (p2.sy - p1.sy) * (fx - p1.sx)) * inv;
                    float b1 = ((p0.sx - p2.sx) * (fy - p2.sy) - (p0.sy - p2.sy) * (fx - p2.sx)) * inv;
                    float b2 = 1f - b0 - b1;
                    if (b0 < -1e-4f || b1 < -1e-4f || b2 < -1e-4f) continue;
                    float z = b0 * p0.vz + b1 * p1.vz + b2 * p2.vz;
                    int idx = py * W + px;
                    if (z >= zbuf[idx]) continue;
                    if (alpha >= 1f)
                    {
                        col[idx] = c;
                        if (writeZ) zbuf[idx] = z;
                    }
                    else
                    {
                        var bgc = col[idx];
                        col[idx] = new Rgba32(
                            (byte)(c.R * alpha + bgc.R * (1f - alpha)),
                            (byte)(c.G * alpha + bgc.G * (1f - alpha)),
                            (byte)(c.B * alpha + bgc.B * (1f - alpha)), (byte)255);
                    }
                }
        }
    }

    /// <summary>
    /// Inverts the <see cref="RenderPerspectivePng"/> camera: given a pixel in the
    /// rendered 3D image, casts a ray from the eye through that pixel and marches it
    /// against the terrain heightmap, returning the world (x,y) it strikes. Kept in
    /// lockstep with the render loop's camera math so picks match what is drawn.
    /// <paramref name="width"/>/<paramref name="height"/> and the yaw/pitch/zoom must
    /// be the same values passed to the render that produced the image.
    /// </summary>
    public static (bool ok, float wx, float wy) PickTerrain(
        Wc3.Model.MapDocument doc, int width, int height,
        float yawDegrees, float pitchDegrees, float zoom,
        float px, float py)
    {
        if (doc.GetFile("war3map.w3e")?.Model is not MapEnvironment env)
            return (false, 0f, 0f);

        width = Math.Clamp(width, 16, 4096);
        height = Math.Clamp(height, 16, 4096);

        int w = (int)env.Width + 1;
        int h = (int)env.Height + 1;
        var tiles = env.TerrainTiles;
        if (tiles is null || tiles.Count < w * h)
            return (false, 0f, 0f);

        const float TileWorld = TerrainTransform.TileWorld;
        const float StepWorld = 128f;
        float originX = -(int)env.Width * (TileWorld / 2f);
        float originY = -(int)env.Height * (TileWorld / 2f);

        // Height grid (world Z per tilepoint) — identical to the render's vertex Z.
        var hz = new float[w * h];
        float minZ = float.MaxValue, maxZ = float.MinValue;
        for (int k = 0; k < w * h; k++)
        {
            float z = (tiles[k].Height + tiles[k].CliffLevel) * StepWorld;
            hz[k] = z;
            if (z < minZ) minZ = z;
            if (z > maxZ) maxZ = z;
        }

        // Rebuild the exact orbit camera used by RenderPerspectivePng.
        int W = width, H = height;
        var center = new Vector3(0f, 0f, (minZ + maxZ) * 0.5f);
        float span = MathF.Max(w * TileWorld, h * TileWorld);
        float radius = MathF.Max(span, maxZ - minZ) * 0.5f + TileWorld;
        float yaw = yawDegrees * (MathF.PI / 180f);
        float pitch = Math.Clamp(pitchDegrees, 2f, 89f) * (MathF.PI / 180f);
        var eyeDir = new Vector3(
            MathF.Cos(pitch) * MathF.Cos(yaw),
            MathF.Cos(pitch) * MathF.Sin(yaw),
            MathF.Sin(pitch));
        float dist = radius / MathF.Max(zoom, 0.05f) * 2.4f;
        var eye = center + eyeDir * dist;
        var forward = Vector3.Normalize(center - eye);
        var right = Vector3.Normalize(Vector3.Cross(forward, Vector3.UnitZ));
        var up = Vector3.Cross(right, forward);
        float focal = 0.5f * H / MathF.Tan(0.5f * (48f * MathF.PI / 180f));

        // World-space ray through the pixel: the algebraic inverse of Project().
        float a = (px - W * 0.5f) / focal;
        float b = (H * 0.5f - py) / focal;
        var dir = Vector3.Normalize(forward + a * right + b * up);

        // Only a descending ray can strike the terrain the camera looks down on.
        if (dir.Z >= -1e-4f)
            return (false, 0f, 0f);

        // Bilinear terrain height at world (x,y), clamped to the tilepoint grid.
        float HeightAt(float x, float y)
        {
            float gi = Math.Clamp((x - originX) / TileWorld, 0f, w - 1.0001f);
            float gj = Math.Clamp((y - originY) / TileWorld, 0f, h - 1.0001f);
            int i0 = (int)gi, j0 = (int)gj;
            int i1 = Math.Min(i0 + 1, w - 1), j1 = Math.Min(j0 + 1, h - 1);
            float fi = gi - i0, fj = gj - j0;
            float z0 = hz[j0 * w + i0] * (1 - fi) + hz[j0 * w + i1] * fi;
            float z1 = hz[j1 * w + i0] * (1 - fi) + hz[j1 * w + i1] * fi;
            return z0 * (1 - fj) + z1 * fj;
        }

        // March the segment where the ray crosses the terrain's Z band. Pad the band
        // so a perfectly flat map (minZ == maxZ) still has a non-degenerate range.
        float pad = (maxZ - minZ) * 0.05f + TileWorld;
        float tEnter = MathF.Max(0f, ((maxZ + pad) - eye.Z) / dir.Z);   // dir.Z < 0 here
        float tExit = ((minZ - pad) - eye.Z) / dir.Z;
        if (tExit <= tEnter)
            return (false, 0f, 0f);

        const int Steps = 256;
        float stepT = (tExit - tEnter) / Steps;
        float tPrev = tEnter;
        float fPrev = (eye + dir * tPrev) is var p0 ? p0.Z - HeightAt(p0.X, p0.Y) : 0f;
        for (int s = 1; s <= Steps; s++)
        {
            float t = tEnter + stepT * s;
            var p = eye + dir * t;
            float f = p.Z - HeightAt(p.X, p.Y);
            if (f <= 0f && fPrev > 0f)
            {
                // Sign change between tPrev..t — bisect for a tighter surface hit.
                float lo = tPrev, hi = t;
                for (int bi = 0; bi < 12; bi++)
                {
                    float mid = 0.5f * (lo + hi);
                    var pm = eye + dir * mid;
                    if (pm.Z - HeightAt(pm.X, pm.Y) > 0f) lo = mid; else hi = mid;
                }
                var hit = eye + dir * (0.5f * (lo + hi));
                float maxX = originX + (w - 1) * TileWorld;
                float maxY = originY + (h - 1) * TileWorld;
                return (true, Math.Clamp(hit.X, originX, maxX), Math.Clamp(hit.Y, originY, maxY));
            }
            tPrev = t; fPrev = f;
        }
        return (false, 0f, 0f);
    }

    /// <summary>
    /// World/pixel mapping for <see cref="RenderTerrainPng"/> so callers (e.g. the
    /// Studio's click-to-place) can turn a pixel in the rendered image back into a
    /// world (x,y). Kept in lockstep with the render loop's scale and north-up flip.
    /// </summary>
    public readonly record struct TerrainTransform(
        int PixelWidth, int PixelHeight, int Scale,
        int GridWidth, int GridHeight, float OriginX, float OriginY)
    {
        /// <summary>World-unit spacing between adjacent tilepoints.</summary>
        public const float TileWorld = 128f;

        /// <summary>Pixel in the rendered image → world (x,y) at that tilepoint.</summary>
        public (float X, float Y) PixelToWorld(double px, double py)
        {
            if (Scale <= 0)
                return (OriginX, OriginY);
            double tx = px / Scale;
            double gridY = (GridHeight - 1) - (py / Scale); // undo the north-up flip
            return (OriginX + (float)(tx * TileWorld),
                    OriginY + (float)(gridY * TileWorld));
        }
    }

    /// <summary>Computes the <see cref="TerrainTransform"/> without rendering.</summary>
    public static TerrainTransform GetTransform(Wc3.Model.MapDocument doc)
    {
        if (doc.GetFile("war3map.w3e")?.Model is not MapEnvironment env)
            throw new InvalidDataException("no terrain (war3map.w3e) in map");
        int w = (int)env.Width + 1;
        int h = (int)env.Height + 1;
        int scale = Math.Max(1, MaxSide / Math.Max(w, h));
        // World coords of tilepoint (0,0): WC3 centers the tile grid on the origin,
        // so the SW corner point sits at -(tiles*128)/2 on each axis. (Standard for
        // World-Editor maps; the 3D viewport pass can refine from the stored offset.)
        float originX = -(int)env.Width * (TerrainTransform.TileWorld / 2f);
        float originY = -(int)env.Height * (TerrainTransform.TileWorld / 2f);
        return new TerrainTransform(w * scale, h * scale, scale, w, h, originX, originY);
    }

    /// <summary>
    /// Maps a ground <see cref="TerrainType"/> to a representative color by
    /// inspecting its enum name (e.g. "L_GrassCliff" → green). Names carry a
    /// tileset-letter prefix and a descriptive suffix; we match the suffix.
    /// </summary>
    /// <summary>What a tile outside the playable area is drawn as. The game shows void there.</summary>
    internal static readonly Rgba32 VoidColor = new(10, 10, 12, 255);

    /// <summary>
    /// Fills one tilepoint's cell. Tilepoints are stored row-major from the south-west corner,
    /// so rows are flipped to put north at the top of the image.
    /// </summary>
    private static void Paint(Image<Rgba32> image, int x, int y, int h, int scale, Rgba32 c)
    {
        int px = x * scale, py = (h - 1 - y) * scale;
        for (int dy = 0; dy < scale; dy++)
            for (int dx = 0; dx < scale; dx++)
                image[px + dx, py + dy] = c;
    }

    /// <summary>
    /// One base colour per ground tile type, taken from the tileset's OWN art where the base
    /// game is available and from the built-in table otherwise.
    /// </summary>
    /// <remarks>
    /// Both renderers here used to build this from <see cref="ColorForTerrainType"/> alone,
    /// while the Studio viewport drew real tile art through <see cref="TerrainArtCatalog"/>.
    /// That is why a rendered map did not look like the map in game, and why the fix belongs
    /// in one shared place rather than copied into each renderer. With no game install
    /// sampling returns null and the built-in colours stand, so both paths stay headless.
    /// </remarks>
    internal static Rgba32[] BuildTypeColors(Wc3.Model.MapDocument doc, MapEnvironment env)
    {
        var colors = new Rgba32[env.TerrainTypes.Count];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = ColorForTerrainType(env.TerrainTypes[i]);

        var sampled = TerrainArtCatalog.AverageColorsForMap(doc);
        if (sampled is null) return colors;

        for (int i = 0; i < colors.Length && i < sampled.Length; i++)
            if (sampled[i] is { } s)
                colors[i] = new Rgba32(s.R, s.G, s.B);
        return colors;
    }

    internal static Rgba32 ColorForTerrainType(TerrainType type)
    {
        string name = type.ToString();
        bool Has(string s) => name.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0;

        // Order matters: check more specific terms before generic ones.
        if (Has("Lava")) return new Rgba32(150, 52, 30);
        if (Has("Abyss")) return new Rgba32(26, 22, 32);
        if (Has("Snow") || Has("Ice")) return new Rgba32(228, 232, 242);
        if (Has("Water") || Has("Shore")) return new Rgba32(60, 96, 156);
        if (Has("Sand") || Has("Desert") || Has("Dune") || Has("Waste"))
            return new Rgba32(198, 176, 120);
        if (Has("Rock") || Has("Stone")) return new Rgba32(112, 110, 106);
        if (Has("Brick") || Has("Tiles") || Has("Path") || Has("Cobble") || Has("Flag"))
            return new Rgba32(150, 140, 122);
        if (Has("Mud") || Has("Bog") || Has("Marsh") || Has("Swamp"))
            return new Rgba32(92, 78, 52);
        if (Has("Dirt") || Has("Ground") || Has("Earth")) return new Rgba32(124, 92, 58);
        if (Has("Leaves") || Has("Vines") || Has("Foliage") || Has("Grassy"))
            return new Rgba32(58, 88, 44);
        if (Has("Grass")) return new Rgba32(86, 125, 57);
        return new Rgba32(96, 108, 84); // unknown → muted grass-green
    }

    private static Rgba32 Scale(Rgba32 c, float f) => new(
        (byte)Math.Clamp(c.R * f, 0f, 255f),
        (byte)Math.Clamp(c.G * f, 0f, 255f),
        (byte)Math.Clamp(c.B * f, 0f, 255f));

    private static Rgba32 Lerp(Rgba32 a, Rgba32 b, float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        return new Rgba32(
            (byte)(a.R + (b.R - a.R) * t),
            (byte)(a.G + (b.G - a.G) * t),
            (byte)(a.B + (b.B - a.B) * t));
    }
}
