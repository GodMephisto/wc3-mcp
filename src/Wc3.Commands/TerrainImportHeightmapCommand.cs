// src/Wc3.Commands/TerrainImportHeightmapCommand.cs
using War3Net.Build.Environment;   // MapEnvironment
using Wc3.Model;
using Wc3.Render;

namespace Wc3.Commands;

/// <summary>
/// <para>Sets a map's terrain tile heights from a grayscale PNG produced by
/// <see cref="TerrainExportHeightmapCommand"/>. The PNG dimensions must match the map's
/// (Width+1) by (Height+1) corner grid exactly, and a mismatch is refused rather than
/// stretched or cropped, because a resized heightmap silently shifts every corner.</para>
///
/// <para>Only <see cref="TerrainTile.Height"/> is written, and cliff level, texture, water
/// and the flags are untouched. The input map is never written here, the caller saves the
/// returned (dirty) document to a copy.</para>
/// </summary>
public static class TerrainImportHeightmapCommand
{
    public sealed record ImportResult(bool Ok, string Message, int TilesChanged = 0);

    /// <summary>Reads <paramref name="png"/> and applies it to the terrain grid.</summary>
    public static ImportResult Import(MapDocument doc, byte[] png)
    {
        if (doc.GetFile(TerrainCommand.TerrainFile)?.Model is not MapEnvironment env)
            return new(false, $"map has no {TerrainCommand.TerrainFile} (terrain) to import into");

        int w = HeightmapPng.GridWidth(env);
        int h = HeightmapPng.GridHeight(env);
        var tiles = env.TerrainTiles;
        if (tiles is null || tiles.Count != w * h)
            return new(false,
                $"terrain tile buffer inconsistent (Width+1={w} Height+1={h} Tiles={tiles?.Count ?? 0})");

        HeightmapImage image;
        try { image = HeightmapPng.Decode(png); }
        catch (Exception ex) { return new(false, $"could not decode heightmap PNG ({ex.Message})"); }

        if (image.Width != w || image.Height != h)
            return new(false,
                $"heightmap is {image.Width}x{image.Height}, but the map's grid is {w}x{h}");

        int changed = 0;
        for (int i = 0; i < tiles.Count; i++)
        {
            // Clamp and quantise to the storable ground range. A 16-bit sample decodes to a height
            // that can sit just past the maximum the 16-bit raw field holds (the top sample is
            // 112.0 against a 111.998 ceiling), and the tile setter wraps an out-of-range value, so
            // the highest exported height came back as the -16 minimum. ClampGroundHeight floors it
            // at the ceiling instead, so export then import is the inverse the class promises.
            float height = TerrainEditCommand.ClampGroundHeight(HeightmapPng.Dequantise(image.Pixels[i]));
            if (tiles[i].Height != height)
            {
                tiles[i].Height = height;
                changed++;
            }
        }

        if (changed > 0)
            doc.AddOrReplaceModelFile(TerrainCommand.TerrainFile, env);
        return new(true, $"set {changed} corner heights from a {w}x{h} heightmap", changed);
    }
}
