// src/Wc3.Commands/TerrainSmoothCommand.cs
using War3Net.Build.Environment;   // MapEnvironment
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Whole-map height smoothing. One pass replaces every corner's height with the mean of
/// itself and its in-grid four neighbours, read from a snapshot so a pass is independent
/// of write order. Repeating <c>passes</c> times converges a noisy height field toward a
/// smooth one. This is the map-wide counterpart to the single-dab
/// <see cref="TerrainCommand.HeightOp.Smooth"/> brush, which smooths only the tiles inside
/// one footprint.
/// </summary>
public static class TerrainSmoothCommand
{
    public sealed record SmoothResult(bool Ok, string Message, int TilesChanged = 0);

    /// <summary>Smooths every terrain corner <paramref name="passes"/> times.</summary>
    public static SmoothResult Smooth(MapDocument doc, int passes)
    {
        if (passes < 0)
            return new(false, $"passes must be >= 0 (got {passes})");

        if (doc.GetFile(TerrainCommand.TerrainFile)?.Model is not MapEnvironment env)
            return new(false, $"map has no {TerrainCommand.TerrainFile} (terrain) to smooth");

        int w = (int)env.Width + 1, h = (int)env.Height + 1;
        var tiles = env.TerrainTiles;
        if (tiles is null || tiles.Count != w * h)
            return new(false,
                $"terrain tile buffer inconsistent (Width+1={w} Height+1={h} Tiles={tiles?.Count ?? 0})");

        var original = new float[tiles.Count];
        for (int i = 0; i < tiles.Count; i++)
            original[i] = tiles[i].Height;

        // A snapshot in and a fresh buffer out per pass, so every corner of one pass reads
        // the same pre-pass state and the kernel does not smear in write order.
        var current = (float[])original.Clone();
        var next = new float[tiles.Count];
        for (int pass = 0; pass < passes; pass++)
        {
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    next[y * w + x] = TerrainCommand.SmoothedHeight(current, w, h, x, y);
            (current, next) = (next, current);
        }

        int changed = 0;
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].Height != current[i])
            {
                tiles[i].Height = current[i];
                changed++;
            }
        }

        if (changed > 0)
            doc.AddOrReplaceModelFile(TerrainCommand.TerrainFile, env);
        return new(true, $"smooth {passes} pass(es), {changed} corners changed", changed);
    }
}
