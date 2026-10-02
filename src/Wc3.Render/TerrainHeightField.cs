// src/Wc3.Render/TerrainHeightField.cs
using System;
using System.Numerics;
using War3Net.Build.Environment;

namespace Wc3.Render;

/// <summary>
/// Bilinear world-height sampler over a map's <c>war3map.w3e</c> tilepoint grid, plus a
/// ray-march ground pick against it. Height math is kept in lockstep with
/// <see cref="TerrainMeshBuilder"/> and <see cref="TerrainRenderer.PickTerrain"/>:
/// z = (Height + CliffLevel) * 128 on a grid centred on the world origin, so samples
/// land exactly on the GL terrain mesh.
/// </summary>
public sealed class TerrainHeightField
{
    private const float TileWorld = TerrainRenderer.TerrainTransform.TileWorld; // 128 world units / tile
    private const float StepWorld = 128f;                                       // one cliff step in world Z

    private readonly float[] _z;
    private readonly int _w, _h;
    private readonly float _originX, _originY;

    public float MinZ { get; }
    public float MaxZ { get; }

    private TerrainHeightField(float[] z, int w, int h, float originX, float originY, float minZ, float maxZ)
    {
        _z = z; _w = w; _h = h;
        _originX = originX; _originY = originY;
        MinZ = minZ; MaxZ = maxZ;
    }

    /// <summary>Builds the field from the map's terrain, or null when the map has none.</summary>
    public static TerrainHeightField? TryCreate(Wc3.Model.MapDocument doc)
    {
        if (doc.GetFile("war3map.w3e")?.Model is not MapEnvironment env)
            return null;
        int w = (int)env.Width + 1;      // tilepoint grid = tiles + 1 per axis
        int h = (int)env.Height + 1;
        var tiles = env.TerrainTiles;
        if (tiles is null || tiles.Count < w * h)
            return null;

        var z = new float[w * h];
        float minZ = float.MaxValue, maxZ = float.MinValue;
        for (int k = 0; k < w * h; k++)
        {
            float v = (tiles[k].Height + tiles[k].CliffLevel) * StepWorld;
            z[k] = v;
            if (v < minZ) minZ = v;
            if (v > maxZ) maxZ = v;
        }
        float originX = -(int)env.Width * (TileWorld / 2f);
        float originY = -(int)env.Height * (TileWorld / 2f);
        return new TerrainHeightField(z, w, h, originX, originY, minZ, maxZ);
    }

    /// <summary>Bilinear terrain height at world (x, y), clamped to the tilepoint grid.</summary>
    public float Sample(float x, float y)
    {
        float gi = Math.Clamp((x - _originX) / TileWorld, 0f, _w - 1.0001f);
        float gj = Math.Clamp((y - _originY) / TileWorld, 0f, _h - 1.0001f);
        int i0 = (int)gi, j0 = (int)gj;
        int i1 = Math.Min(i0 + 1, _w - 1), j1 = Math.Min(j0 + 1, _h - 1);
        float fi = gi - i0, fj = gj - j0;
        float z0 = _z[j0 * _w + i0] * (1 - fi) + _z[j0 * _w + i1] * fi;
        float z1 = _z[j1 * _w + i0] * (1 - fi) + _z[j1 * _w + i1] * fi;
        return z0 * (1 - fj) + z1 * fj;
    }

    /// <summary>
    /// Marches a world-space ray (<paramref name="eye"/> + t·<paramref name="dir"/>) against the
    /// height field and returns the ground (x, y) it strikes, clamped to the grid extents.
    /// Mirrors the march in <see cref="TerrainRenderer.PickTerrain"/> so GL-viewport picks land
    /// where the CPU pick would.
    /// </summary>
    public (bool ok, float wx, float wy) RaycastGround(Vector3 eye, Vector3 dir)
    {
        // Only a descending ray can strike terrain the camera looks down on.
        if (dir.Z >= -1e-4f)
            return (false, 0f, 0f);

        // March the segment where the ray crosses the terrain's Z band. Pad the band
        // so a perfectly flat map (MinZ == MaxZ) still has a non-degenerate range.
        float pad = (MaxZ - MinZ) * 0.05f + TileWorld;
        float tEnter = MathF.Max(0f, ((MaxZ + pad) - eye.Z) / dir.Z);   // dir.Z < 0 here
        float tExit = ((MinZ - pad) - eye.Z) / dir.Z;
        if (tExit <= tEnter)
            return (false, 0f, 0f);

        const int Steps = 256;
        float stepT = (tExit - tEnter) / Steps;
        float tPrev = tEnter;
        var p0 = eye + dir * tPrev;
        float fPrev = p0.Z - Sample(p0.X, p0.Y);
        for (int s = 1; s <= Steps; s++)
        {
            float t = tEnter + stepT * s;
            var p = eye + dir * t;
            float f = p.Z - Sample(p.X, p.Y);
            if (f <= 0f && fPrev > 0f)
            {
                // Sign change between tPrev..t — bisect for a tighter surface hit.
                float lo = tPrev, hi = t;
                for (int bi = 0; bi < 12; bi++)
                {
                    float mid = 0.5f * (lo + hi);
                    var pm = eye + dir * mid;
                    if (pm.Z - Sample(pm.X, pm.Y) > 0f) lo = mid; else hi = mid;
                }
                var hit = eye + dir * (0.5f * (lo + hi));
                float maxX = _originX + (_w - 1) * TileWorld;
                float maxY = _originY + (_h - 1) * TileWorld;
                return (true, Math.Clamp(hit.X, _originX, maxX), Math.Clamp(hit.Y, _originY, maxY));
            }
            tPrev = t; fPrev = f;
        }
        return (false, 0f, 0f);
    }
}
