// src/Wc3.Render/WaterMesh.cs
using System;
using War3Net.Build.Environment;

namespace Wc3.Render;

/// <summary>
/// GPU-ready water-surface geometry derived from a map's <c>war3map.w3e</c> water
/// corners, in the same WC3 world space (Z-up) as <see cref="TerrainMesh"/>. Vertices
/// are bare <c>position(3)</c>. One quad is emitted per water cell (a cell whose
/// south-west corner is flagged water, the convention
/// <see cref="TerrainRenderer.RenderPerspectivePng"/>'s water pass uses) at the
/// corners' stored water levels, so per-tile sculpted levels shape the surface.
/// Empty (zero-length arrays) when the map has no water.
/// </summary>
public readonly record struct WaterMesh(float[] Vertices, uint[] Indices)
{
    /// <summary>Floats per vertex: px, py, pz.</summary>
    public const int Stride = 3;
}

/// <summary>Builds a <see cref="WaterMesh"/> from a parsed map document.</summary>
public static class WaterMeshBuilder
{
    /// <summary>One cliff step in world Z, the same conversion the terrain mesh uses.</summary>
    private const float StepWorld = 128f;

    /// <summary>
    /// World-Z offset that lands a corner's normalized water level on the terrain
    /// mesh's datum. Two parts combine: the terrain mesh computes ground Z as
    /// (Height + CliffLevel) * 128, skipping the w3e spec's "(layer - 2)" cliff
    /// normalization (which lifts its datum two steps above the game's), and the
    /// game seats the water table 89.6 world units below the stored level (the
    /// spec's water-level formula). Net shift: 2 * 128 - 89.6.
    /// </summary>
    public const float WaterZOffset = 2f * StepWorld - 89.6f;

    /// <summary>
    /// Builds the water surface for a map, or an empty mesh when the map has no
    /// terrain file, an inconsistent tile grid, or no water-flagged corners.
    /// World coordinates (origin, tile size, height conversion) match
    /// <see cref="TerrainMeshBuilder.Build"/> exactly so the sheet overlays the
    /// GL terrain mesh without any per-view fixups.
    /// </summary>
    public static WaterMesh Build(Wc3.Model.MapDocument doc)
    {
        if (doc.GetFile("war3map.w3e")?.Model is not MapEnvironment env)
            return new WaterMesh(Array.Empty<float>(), Array.Empty<uint>());

        int w = (int)env.Width + 1;      // tilepoint grid = tiles + 1 per axis
        int h = (int)env.Height + 1;
        var tiles = env.TerrainTiles;
        if (tiles is null || tiles.Count < w * h)
            return new WaterMesh(Array.Empty<float>(), Array.Empty<uint>());

        const float TileWorld = TerrainRenderer.TerrainTransform.TileWorld; // 128 world units / tile
        float originX = -(int)env.Width * (TileWorld / 2f);
        float originY = -(int)env.Height * (TileWorld / 2f);

        // Size the buffers exactly: one quad per cell whose SW corner is water.
        int waterCells = 0;
        for (int j = 0; j < h - 1; j++)
            for (int i = 0; i < w - 1; i++)
                if (tiles[j * w + i].IsWater)
                    waterCells++;
        if (waterCells == 0)
            return new WaterMesh(Array.Empty<float>(), Array.Empty<uint>());

        var verts = new float[waterCells * 4 * WaterMesh.Stride];
        var idxs = new uint[waterCells * 6];
        int vp = 0, ip = 0;
        uint baseV = 0;

        for (int j = 0; j < h - 1; j++)
            for (int i = 0; i < w - 1; i++)
            {
                int ia = j * w + i;            // SW, flags the cell as water
                if (!tiles[ia].IsWater)
                    continue;
                int ib = ia + 1;               // SE
                int ic = ia + w;               // NW
                int id = ic + 1;               // NE

                // Each corner contributes its own stored level so a sloped or
                // stepped water body renders faithfully. A corner not flagged
                // water carries a stale stored level, so it inherits the
                // anchoring SW corner's level instead.
                float anchor = tiles[ia].WaterHeight;
                float za = anchor * StepWorld + WaterZOffset;
                float zb = LevelOf(tiles[ib], anchor);
                float zc = LevelOf(tiles[ic], anchor);
                float zd = LevelOf(tiles[id], anchor);

                float x0 = originX + i * TileWorld, x1 = x0 + TileWorld;
                float y0 = originY + j * TileWorld, y1 = y0 + TileWorld;
                Emit(verts, ref vp, x0, y0, za);
                Emit(verts, ref vp, x1, y0, zb);
                Emit(verts, ref vp, x0, y1, zc);
                Emit(verts, ref vp, x1, y1, zd);

                // Same winding as the terrain mesh: (SW,SE,NE) and (SW,NE,NW), CCW from +Z.
                idxs[ip++] = baseV + 0; idxs[ip++] = baseV + 1; idxs[ip++] = baseV + 3;
                idxs[ip++] = baseV + 0; idxs[ip++] = baseV + 3; idxs[ip++] = baseV + 2;
                baseV += 4;
            }

        return new WaterMesh(verts, idxs);
    }

    /// <summary>World Z of one corner's water surface. Falls back to the anchoring
    /// water corner's level when this corner is not itself flagged water.</summary>
    private static float LevelOf(TerrainTile tile, float anchorLevel)
        => (tile.IsWater ? tile.WaterHeight : anchorLevel) * StepWorld + WaterZOffset;

    private static void Emit(float[] buf, ref int o, float px, float py, float pz)
    {
        buf[o++] = px; buf[o++] = py; buf[o++] = pz;
    }
}
