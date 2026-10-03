using Wc3.GameData;
using Wc3.Modeling;
using War3Net.Build.Environment;

namespace Wc3.Render;

/// <summary>
/// Resolves WC3 ground tiles to decoded RGBA tile textures from the base-game CASC:
/// tileId (4CC, e.g. "Lgrs") -> Terrain.slk row (dir/file) -> DDS/BLP -> RGBA cropped to a cell.
/// Never throws; yields null when a tile is unavailable so callers fall back to flat colours.
/// </summary>
public sealed class TerrainArtCatalog
{
    /// <summary>Variation-cell size we crop to. Reforged tiles are 512x256 variation atlases;
    /// the top-left <see cref="Cell"/>x<see cref="Cell"/> region is variation 0.</summary>
    public const int Cell = 128;

    private readonly GameDataContext _ctx;
    private readonly SlkTable? _slk;
    private readonly Dictionary<string, TextureImage?> _cache = new(StringComparer.OrdinalIgnoreCase);

    private TerrainArtCatalog(GameDataContext ctx, SlkTable? slk) { _ctx = ctx; _slk = slk; }

    /// <summary>Opens Terrain.slk from the given context. Never throws.</summary>
    public static TerrainArtCatalog Open(GameDataContext ctx)
    {
        SlkTable? slk = null;
        if (ctx.TryReadFile("war3.w3mod:terrainart\\terrain.slk", out var bytes))
        {
            try { slk = SlkTable.Parse(bytes); } catch { slk = null; }
        }
        return new TerrainArtCatalog(ctx, slk);
    }

    /// <summary>Whether Terrain.slk was found (else every Resolve returns null).</summary>
    public bool HasCatalog => _slk != null;

    /// <summary>
    /// Maps a War3Net <see cref="TerrainType"/> to its 4CC tile id (e.g. "Lgrs"). WC3 stores
    /// 4CCs byte-reversed vs the packed int, so we try the reversed order first and fall back
    /// to whichever order actually exists in Terrain.slk.
    /// </summary>
    public string TileIdOf(TerrainType type)
    {
        var raw = BitConverter.GetBytes((int)type);
        string fwd = new(new[] { (char)raw[0], (char)raw[1], (char)raw[2], (char)raw[3] });
        string rev = new(new[] { (char)raw[3], (char)raw[2], (char)raw[1], (char)raw[0] });
        if (_slk != null)
        {
            if (_slk.TryGetRow(rev, out _)) return rev;
            if (_slk.TryGetRow(fwd, out _)) return fwd;
        }
        return rev;
    }

    public TextureImage? Resolve(TerrainType type) => Resolve(TileIdOf(type));

    /// <summary>Resolves a tile id to a <see cref="Cell"/>x<see cref="Cell"/> RGBA image (cached).</summary>
    public TextureImage? Resolve(string tileId)
    {
        if (_cache.TryGetValue(tileId, out var cached)) return cached;
        var img = Load(tileId);
        _cache[tileId] = img;
        return img;
    }

    private TextureImage? Load(string tileId)
    {
        if (_slk == null || !_slk.TryGetRow(tileId, out var row)) return null;
        row.TryGetValue("dir", out var dir);
        row.TryGetValue("file", out var file);
        if (string.IsNullOrWhiteSpace(file)) return null;
        var rel = string.IsNullOrWhiteSpace(dir) ? file! : dir!.TrimEnd('\\') + "\\" + file;

        // Reforged ships .dds; older/classic installs .blp.
        foreach (var (ext, isDds) in new[] { (".dds", true), (".blp", false) })
        {
            var path = ("war3.w3mod:" + rel + ext).ToLowerInvariant();
            if (!_ctx.TryReadFile(path, out var tex)) continue;
            try
            {
                var full = isDds ? DdsDecoder.Decode(tex) : BlpDecoder.Decode(tex);
                return Crop(full, Cell);
            }
            catch { /* corrupt/unsupported — try next extension, else null */ }
        }
        return null;
    }

    /// <summary>Crops the top-left cell (variation 0) as an RGBA <see cref="TextureImage"/>.</summary>
    private static TextureImage Crop(TextureImage src, int cell)
    {
        int w = Math.Min(cell, src.Width);
        int h = Math.Min(cell, src.Height);
        var dst = new byte[w * h * 4];
        int srcStride = src.Width * 4;
        int dstStride = w * 4;
        for (int y = 0; y < h; y++)
            Array.Copy(src.Rgba, y * srcStride, dst, y * dstStride, dstStride);
        return new TextureImage(w, h, dst);
    }

    /// <summary>
    /// Builds a contiguous RGBA layer array (<c>layerCount * cell * cell * 4</c>) for a map's
    /// terrain types: real tile textures where available, flat terrain-type colours as fallback.
    /// Opens the base-game CASC internally (cached); if unavailable, every layer is a flat colour.
    /// </summary>
    public static byte[] BuildLayersForMap(Wc3.Model.MapDocument doc, out int layerCount, out int cell)
    {
        cell = Cell;
        var env = doc.GetFile("war3map.w3e")?.Model as MapEnvironment;
        var types = env?.TerrainTypes;
        layerCount = Math.Max(1, types?.Count ?? 1);

        TerrainArtCatalog? cat = null;
        if (Wc3.GameData.GameData.TryOpen(null, out var ctx, out _) && ctx != null)
            cat = Open(ctx);

        int layerBytes = cell * cell * 4;
        var data = new byte[layerCount * layerBytes];
        for (int i = 0; i < layerCount; i++)
        {
            byte[]? rgba = null;
            if (types != null && i < types.Count)
            {
                var img = cat?.Resolve(types[i]);
                rgba = (img != null && img.Width == cell && img.Height == cell && img.Rgba.Length == layerBytes)
                    ? img.Rgba
                    : FlatColor(types[i], cell);
            }
            rgba ??= Fill(cell, 120, 120, 120);
            Array.Copy(rgba, 0, data, i * layerBytes, layerBytes);
        }
        return data;
    }

    /// <summary>
    /// One representative RGB per terrain type, taken from that type's REAL tile texture.
    /// Returns null when the base game is unavailable, so a caller keeps its own fallback.
    /// </summary>
    /// <remarks>
    /// The PNG renderer draws a whole tile into a handful of pixels, so blitting the 128x128
    /// texture and letting it downsample would arrive at this average anyway. Computing it
    /// directly keeps that path cheap and, more to the point, makes the colours the tileset's
    /// own rather than a hardcoded guess. An entry is null where the texture did not resolve,
    /// which a caller must read as "keep the old colour" rather than as black.
    /// </remarks>
    public static (byte R, byte G, byte B)?[]? AverageColorsForMap(Wc3.Model.MapDocument doc)
    {
        var env = doc.GetFile("war3map.w3e")?.Model as MapEnvironment;
        var types = env?.TerrainTypes;
        if (types is null || types.Count == 0) return null;

        if (!Wc3.GameData.GameData.TryOpen(null, out var ctx, out _) || ctx is null) return null;
        var cat = Open(ctx);
        if (!cat.HasCatalog) return null;

        var result = new (byte, byte, byte)?[types.Count];
        for (int i = 0; i < types.Count; i++)
        {
            var img = cat.Resolve(types[i]);
            if (img is not null) result[i] = Average(img);
        }
        return result;
    }

    /// <summary>Mean RGB over the opaque pixels of a decoded texture.</summary>
    private static (byte R, byte G, byte B)? Average(TextureImage img)
    {
        long r = 0, g = 0, b = 0, n = 0;
        var px = img.Rgba;
        for (int p = 0; p + 3 < px.Length; p += 4)
        {
            if (px[p + 3] == 0) continue;   // a fully transparent pixel contributes nothing
            r += px[p]; g += px[p + 1]; b += px[p + 2]; n++;
        }
        if (n == 0) return null;
        return ((byte)(r / n), (byte)(g / n), (byte)(b / n));
    }

    private static byte[] FlatColor(TerrainType type, int cell)
    {
        var c = TerrainRenderer.ColorForTerrainType(type);
        return Fill(cell, c.R, c.G, c.B);
    }

    private static byte[] Fill(int cell, byte r, byte g, byte b)
    {
        var buf = new byte[cell * cell * 4];
        for (int p = 0; p < buf.Length; p += 4) { buf[p] = r; buf[p + 1] = g; buf[p + 2] = b; buf[p + 3] = 255; }
        return buf;
    }
}
