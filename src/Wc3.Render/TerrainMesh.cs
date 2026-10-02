// src/Wc3.Render/TerrainMesh.cs
using System;
using System.IO;
using System.Numerics;
using War3Net.Build.Environment;

namespace Wc3.Render;

/// <summary>
/// GPU-ready terrain geometry derived from a map's <c>war3map.w3e</c> tilepoint grid.
/// Vertices are interleaved <c>position(3) · normal(3) · uv(2) · layers(4)</c> in WC3 world
/// space (Z-up). Geometry is emitted <b>per cell</b> (4 verts / cell, not shared) so each
/// cell can carry its four corner terrain-type layer indices for GPU texture-splat blending;
/// <c>uv</c> is the corner's 0/1 position within the cell (used both as the tiling texcoord
/// and as the bilinear blend weight). Height math matches
/// <see cref="TerrainRenderer.RenderPerspectivePng"/> and <see cref="TerrainRenderer.PickTerrain"/>.
/// </summary>
public readonly record struct TerrainMesh(
    float[] Vertices,
    uint[] Indices,
    Vector3 Center,
    float Radius)
{
    /// <summary>Floats per vertex: px,py,pz, nx,ny,nz, u,v, l00,l10,l01,l11.</summary>
    public const int Stride = 12;
}

/// <summary>Builds a <see cref="TerrainMesh"/> from a parsed map document.</summary>
public static class TerrainMeshBuilder
{
    public static TerrainMesh Build(Wc3.Model.MapDocument doc)
    {
        if (doc.GetFile("war3map.w3e")?.Model is not MapEnvironment env)
            throw new InvalidDataException("no terrain (war3map.w3e) in map");

        int w = (int)env.Width + 1;      // tilepoint grid = tiles + 1 per axis
        int h = (int)env.Height + 1;
        var tiles = env.TerrainTiles;
        if (tiles is null || tiles.Count < w * h)
            throw new InvalidDataException("terrain tilepoint data is incomplete");

        int typeCount = Math.Max(1, env.TerrainTypes.Count);

        const float TileWorld = TerrainRenderer.TerrainTransform.TileWorld; // 128 world units / tile
        const float StepWorld = 128f;                                        // one cliff step in world Z
        float originX = -(int)env.Width * (TileWorld / 2f);
        float originY = -(int)env.Height * (TileWorld / 2f);

        int n = w * h;
        var zg = new float[n];
        float minZ = float.MaxValue, maxZ = float.MinValue;
        for (int j = 0; j < h; j++)
            for (int i = 0; i < w; i++)
            {
                var t = tiles[j * w + i];
                float z = (t.Height + t.CliffLevel) * StepWorld;
                zg[j * w + i] = z;
                if (z < minZ) minZ = z;
                if (z > maxZ) maxZ = z;
            }

        // Per-grid-point normal (central difference, Z-up; borders clamp to the edge sample).
        var nrm = new Vector3[n];
        for (int j = 0; j < h; j++)
            for (int i = 0; i < w; i++)
            {
                float zL = zg[j * w + Math.Max(i - 1, 0)];
                float zR = zg[j * w + Math.Min(i + 1, w - 1)];
                float zD = zg[Math.Max(j - 1, 0) * w + i];
                float zU = zg[Math.Min(j + 1, h - 1) * w + i];
                nrm[j * w + i] = Vector3.Normalize(new Vector3(-(zR - zL), -(zU - zD), 2f * TileWorld));
            }

        int cells = (w - 1) * (h - 1);
        var verts = new float[cells * 4 * TerrainMesh.Stride];
        var idxs = new uint[cells * 6];
        int vp = 0, ip = 0;
        uint baseV = 0;

        for (int j = 0; j < h - 1; j++)
            for (int i = 0; i < w - 1; i++)
            {
                int ia = j * w + i;            // SW  uv(0,0)
                int ib = ia + 1;               // SE  uv(1,0)
                int ic = (j + 1) * w + i;      // NW  uv(0,1)
                int id = ic + 1;               // NE  uv(1,1)

                float la = LayerOf(tiles[ia].Texture, typeCount);
                float lb = LayerOf(tiles[ib].Texture, typeCount);
                float lc = LayerOf(tiles[ic].Texture, typeCount);
                float ld = LayerOf(tiles[id].Texture, typeCount);

                Emit(verts, ref vp, originX + i * TileWorld,       originY + j * TileWorld,       zg[ia], nrm[ia], 0f, 0f, la, lb, lc, ld);
                Emit(verts, ref vp, originX + (i + 1) * TileWorld, originY + j * TileWorld,       zg[ib], nrm[ib], 1f, 0f, la, lb, lc, ld);
                Emit(verts, ref vp, originX + i * TileWorld,       originY + (j + 1) * TileWorld, zg[ic], nrm[ic], 0f, 1f, la, lb, lc, ld);
                Emit(verts, ref vp, originX + (i + 1) * TileWorld, originY + (j + 1) * TileWorld, zg[id], nrm[id], 1f, 1f, la, lb, lc, ld);

                // Two triangles (SW,SE,NE) & (SW,NE,NW), wound CCW as seen from +Z.
                idxs[ip++] = baseV + 0; idxs[ip++] = baseV + 1; idxs[ip++] = baseV + 3;
                idxs[ip++] = baseV + 0; idxs[ip++] = baseV + 3; idxs[ip++] = baseV + 2;
                baseV += 4;
            }

        var center = new Vector3(0f, 0f, (minZ + maxZ) * 0.5f);
        float span = MathF.Max(w * TileWorld, h * TileWorld);
        float radius = MathF.Max(span, maxZ - minZ) * 0.5f + TileWorld;
        return new TerrainMesh(verts, idxs, center, radius);
    }

    private static float LayerOf(int tex, int typeCount)
        => tex < 0 ? 0f : (tex >= typeCount ? typeCount - 1 : tex);

    private static void Emit(float[] buf, ref int o, float px, float py, float pz, Vector3 nrm,
                             float u, float vv, float la, float lb, float lc, float ld)
    {
        buf[o++] = px; buf[o++] = py; buf[o++] = pz;
        buf[o++] = nrm.X; buf[o++] = nrm.Y; buf[o++] = nrm.Z;
        buf[o++] = u; buf[o++] = vv;
        buf[o++] = la; buf[o++] = lb; buf[o++] = lc; buf[o++] = ld;
    }
}
