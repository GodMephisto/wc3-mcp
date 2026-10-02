// src/Wc3.Commands/TerrainEditCommand.cs
using War3Net.Build.Environment;   // MapEnvironment, TerrainTile
using War3Net.Common.Extensions;   // ToRawcode
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Per-corner terrain editing primitives over a map's <c>war3map.w3e</c> — the building
/// blocks a sculpting brush UI composes (read a corner, nudge its height, repaint its
/// tile, step its cliff layer). The environment is a (Width+1)×(Height+1) grid of corner
/// tiles (<c>env.TerrainTiles</c>, row-major from the map's south-west corner, index =
/// row*(Width+1) + col — the same convention <see cref="TerrainCommand"/> and
/// <see cref="Wc3.Render.TerrainRenderer"/> use). Complements the area brushes in
/// <see cref="TerrainCommand"/> with exact single-corner reads/writes plus one
/// falloff-weighted area raise for the raise/lower brush.
///
/// <para><b>Height units:</b> heights are War3Net's normalized units — 1.0 equals one
/// cliff step (512 raw file units, 128 world-Z units; world Z = (Height + CliffLevel)
/// × 128). The file stores ground height as a ushort (raw = height×512 + 8192), so
/// writes are quantized to 1/512 steps and clamped to the representable range
/// [<see cref="MinGroundHeight"/>, <see cref="MaxGroundHeight"/>] up front — what
/// <see cref="GetCorner"/> reports after an edit is exactly what a save/load round-trip
/// preserves. Water level shares the encoding but keeps only 14 bits
/// ([<see cref="MinWaterHeight"/>, <see cref="MaxWaterHeight"/>]); cliff level is the
/// file's 4-bit plateau layer (0..<see cref="MaxCliffLevel"/>).</para>
/// </summary>
public static class TerrainEditCommand
{
    public const string TerrainFile = "war3map.w3e";

    /// <summary>Lowest storable ground/water height (raw 0 → (0−8192)/512).</summary>
    public const float MinGroundHeight = -16f;

    /// <summary>Highest storable ground height (raw ushort max → (65535−8192)/512).</summary>
    public const float MaxGroundHeight = (65535 - 8192) / 512f;

    /// <summary>Lowest storable water level (same raw zero-point as ground).</summary>
    public const float MinWaterHeight = -16f;

    /// <summary>Highest storable water level — the file keeps water in a 14-bit field
    /// (raw 0x3FFF max), the top bits being flags.</summary>
    public const float MaxWaterHeight = (16383 - 8192) / 512f;

    /// <summary>Highest cliff plateau layer — a 4-bit nibble in the file (War3Net throws
    /// on serialize past it).</summary>
    public const int MaxCliffLevel = 15;

    /// <summary>Terrain grid summary. <paramref name="Width"/>/<paramref name="Height"/>
    /// are corner counts (tiles + 1 per axis); the tile lists are fourCC ids in map
    /// order, so a corner's texture index points into <paramref name="GroundTiles"/>.</summary>
    public sealed record TerrainInfo(
        int Width,
        int Height,
        IReadOnlyList<string> GroundTiles,
        IReadOnlyList<string> CliffTiles);

    /// <summary>Snapshot of one corner tile. <paramref name="GroundHeight"/> excludes the
    /// cliff level (elevation = (GroundHeight + CliffLevel) × 128 world units); texture
    /// indices point into the map's ground/cliff tile lists (<see cref="GetInfo"/>).</summary>
    public sealed record CornerInfo(
        int Col,
        int Row,
        float GroundHeight,
        float WaterHeight,
        int GroundTexture,
        int TextureVariation,
        int CliffLevel,
        int CliffTexture,
        int CliffVariation,
        bool Water,
        bool Ramp,
        bool Blighted,
        bool Boundary,
        bool EdgeTile);

    public sealed record TerrainEditResult(bool Ok, string Message, int CornersChanged = 0);

    /// <summary>Grid dimensions (in corners) plus the map's ground/cliff tile ids, or
    /// null when the map has no terrain file.</summary>
    public static TerrainInfo? GetInfo(MapDocument doc)
    {
        if (doc.GetFile(TerrainFile)?.Model is not MapEnvironment env)
            return null;
        var ground = (env.TerrainTypes ?? []).Select(t => ((int)t).ToRawcode()).ToList();
        var cliff = (env.CliffTypes ?? []).Select(t => ((int)t).ToRawcode()).ToList();
        return new TerrainInfo((int)env.Width + 1, (int)env.Height + 1, ground, cliff);
    }

    /// <summary>Reads one corner, or null when the map has no terrain or
    /// (<paramref name="col"/>,<paramref name="row"/>) is off the grid.</summary>
    public static CornerInfo? GetCorner(MapDocument doc, int col, int row)
    {
        if (TryGetGrid(doc, out var env, out int w, out int h) is not null)
            return null;
        if (col < 0 || col >= w || row < 0 || row >= h)
            return null;
        var t = env!.TerrainTiles[row * w + col];
        return new CornerInfo(
            col, row,
            t.Height, t.WaterHeight,
            t.Texture, t.Variation,
            t.CliffLevel, t.CliffTexture, t.CliffVariation,
            t.IsWater, t.IsRamp, t.IsBlighted, t.IsBoundary, t.IsEdgeTile);
    }

    /// <summary>Sets a corner's ground height (normalized units; quantized to 1/512 and
    /// clamped to the storable range — see the class remarks).</summary>
    public static TerrainEditResult SetGroundHeight(MapDocument doc, int col, int row, float height)
    {
        if (!float.IsFinite(height))
            return new(false, $"height must be finite (got {height})");
        if (Locate(doc, col, row, out var env, out var tile) is { } error)
            return new(false, error);

        float target = Quantize(Math.Clamp(height, MinGroundHeight, MaxGroundHeight));
        int changed = 0;
        if (tile!.Height != target)
        {
            tile.Height = target;
            doc.AddOrReplaceModelFile(TerrainFile, env!);
            changed = 1;
        }
        return new(true, $"corner ({col},{row}) ground height = {target:0.###}", changed);
    }

    /// <summary>Adds <paramref name="delta"/> to a corner's ground height (normalized
    /// units; the result is quantized and clamped like <see cref="SetGroundHeight"/>).</summary>
    public static TerrainEditResult AddGroundHeight(MapDocument doc, int col, int row, float delta)
    {
        if (!float.IsFinite(delta))
            return new(false, $"delta must be finite (got {delta})");
        if (Locate(doc, col, row, out _, out var tile) is { } error)
            return new(false, error);
        return SetGroundHeight(doc, col, row, tile!.Height + delta);
    }

    /// <summary>Sets a corner's water level (normalized units, 14-bit storable range) and
    /// flags the corner as water — the primitive a water brush calls per corner.</summary>
    public static TerrainEditResult SetWaterHeight(MapDocument doc, int col, int row, float waterHeight)
    {
        if (!float.IsFinite(waterHeight))
            return new(false, $"water height must be finite (got {waterHeight})");
        if (Locate(doc, col, row, out var env, out var tile) is { } error)
            return new(false, error);

        float target = Quantize(Math.Clamp(waterHeight, MinWaterHeight, MaxWaterHeight));
        int changed = 0;
        if (tile!.WaterHeight != target || !tile.IsWater)
        {
            tile.WaterHeight = target;
            tile.IsWater = true;
            doc.AddOrReplaceModelFile(TerrainFile, env!);
            changed = 1;
        }
        return new(true, $"corner ({col},{row}) water height = {target:0.###}", changed);
    }

    /// <summary>Paints a corner's ground texture. <paramref name="tileIndex"/> points into
    /// the map's ground tile list (<see cref="GetInfo"/>) and is validated against it (the
    /// file stores the index as a nibble, so 15 is the format ceiling regardless).</summary>
    public static TerrainEditResult SetGroundTexture(MapDocument doc, int col, int row, int tileIndex)
    {
        if (Locate(doc, col, row, out var env, out var tile) is { } error)
            return new(false, error);

        int groundTypes = env!.TerrainTypes?.Count ?? 0;
        if (groundTypes == 0)
            return new(false, "map has no ground tile-types to paint with");
        int max = Math.Min(groundTypes - 1, 15);
        if (tileIndex < 0 || tileIndex > max)
            return new(false,
                $"texture index {tileIndex} out of range (map has {groundTypes} ground tile-types; valid 0..{max})");

        int changed = 0;
        if (tile!.Texture != tileIndex)
        {
            tile.Texture = tileIndex;
            doc.AddOrReplaceModelFile(TerrainFile, env);
            changed = 1;
        }
        return new(true, $"corner ({col},{row}) ground texture = {tileIndex}", changed);
    }

    /// <summary>Sets a corner's cliff plateau layer (0..<see cref="MaxCliffLevel"/> — the
    /// file's 4-bit range; War3Net refuses to serialize anything past it).</summary>
    public static TerrainEditResult SetCliffLevel(MapDocument doc, int col, int row, int level)
    {
        if (level < 0 || level > MaxCliffLevel)
            return new(false, $"cliff level {level} out of range (valid 0..{MaxCliffLevel})");
        if (Locate(doc, col, row, out var env, out var tile) is { } error)
            return new(false, error);

        int changed = 0;
        if (tile!.CliffLevel != level)
        {
            tile.CliffLevel = level;
            doc.AddOrReplaceModelFile(TerrainFile, env!);
            changed = 1;
        }
        return new(true, $"corner ({col},{row}) cliff level = {level}", changed);
    }

    /// <summary>
    /// Raise/lower-brush primitive: adds <paramref name="delta"/> to every corner within
    /// <paramref name="radius"/> corners of (<paramref name="centerCol"/>,
    /// <paramref name="centerRow"/>) — a circular footprint (dist ≤ radius; radius 0 is
    /// just the centre). With <paramref name="falloff"/> the delta is weighted
    /// 1 − dist/(radius+1), so the centre moves by the full delta and the rim by a
    /// fraction, giving a smooth mound instead of a plateau. Corners off the grid are
    /// skipped (an entirely off-grid footprint is a no-op, not an error). Each corner is
    /// quantized/clamped like <see cref="SetGroundHeight"/>.
    /// </summary>
    public static TerrainEditResult AddGroundHeightArea(
        MapDocument doc, int centerCol, int centerRow, int radius, float delta, bool falloff)
    {
        if (radius < 0)
            return new(false, $"radius must be >= 0 (got {radius})");
        if (!float.IsFinite(delta))
            return new(false, $"delta must be finite (got {delta})");
        if (TryGetGrid(doc, out var env, out int w, out int h) is { } error)
            return new(false, error);

        int loCol = Math.Max(0, centerCol - radius), hiCol = Math.Min(w - 1, centerCol + radius);
        int loRow = Math.Max(0, centerRow - radius), hiRow = Math.Min(h - 1, centerRow + radius);
        long r2 = (long)radius * radius;

        int changed = 0;
        var tiles = env!.TerrainTiles;
        for (int row = loRow; row <= hiRow; row++)
        {
            for (int col = loCol; col <= hiCol; col++)
            {
                long dc = col - centerCol, dr = row - centerRow;
                long d2 = dc * dc + dr * dr;
                if (d2 > r2) continue;

                float weight = falloff ? 1f - MathF.Sqrt(d2) / (radius + 1) : 1f;
                var tile = tiles[row * w + col];
                float target = Quantize(Math.Clamp(
                    tile.Height + delta * weight, MinGroundHeight, MaxGroundHeight));
                if (tile.Height != target)
                {
                    tile.Height = target;
                    changed++;
                }
            }
        }

        if (changed > 0)
            doc.AddOrReplaceModelFile(TerrainFile, env);
        return new(true,
            $"add {delta:0.###}{(falloff ? " (falloff)" : "")} over r{radius} at ({centerCol},{centerRow}) — {changed} corners changed",
            changed);
    }

    /// <summary>Snaps a normalized height onto the file's 1/512-step lattice so the
    /// in-memory value equals what a save/load round-trip yields (the serializer
    /// truncates; pre-rounding makes the truncation exact).</summary>
    private static float Quantize(float height) => MathF.Round(height * 512f) / 512f;

    /// <summary>Fetches the terrain grid; returns an error message or null on success.</summary>
    private static string? TryGetGrid(MapDocument doc, out MapEnvironment? env, out int w, out int h)
    {
        env = doc.GetFile(TerrainFile)?.Model as MapEnvironment;
        w = h = 0;
        if (env is null)
            return $"map has no {TerrainFile} (terrain) to edit";
        w = (int)env.Width + 1;
        h = (int)env.Height + 1;
        if (env.TerrainTiles is null || env.TerrainTiles.Count != w * h)
            return $"terrain tile buffer inconsistent (Width+1={w} Height+1={h} Tiles={env.TerrainTiles?.Count ?? 0})";
        return null;
    }

    /// <summary>Bounds-checked corner lookup; returns an error message or null on success.</summary>
    private static string? Locate(MapDocument doc, int col, int row, out MapEnvironment? env, out TerrainTile? tile)
    {
        tile = null;
        if (TryGetGrid(doc, out env, out int w, out int h) is { } error)
            return error;
        if (col < 0 || col >= w || row < 0 || row >= h)
            return $"corner ({col},{row}) out of range (grid is {w}x{h} corners)";
        tile = env!.TerrainTiles[row * w + col];
        return null;
    }
}
