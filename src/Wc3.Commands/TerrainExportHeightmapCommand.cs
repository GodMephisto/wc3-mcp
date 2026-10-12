// src/Wc3.Commands/TerrainExportHeightmapCommand.cs
using War3Net.Build.Environment;   // MapEnvironment
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Exports a map's terrain height grid as a 16-bit grayscale PNG, one pixel per corner.
/// Read-only, nothing on the <see cref="MapDocument"/> is touched, so the caller can
/// write the returned bytes wherever it likes. The grid is the map's own
/// (Width+1) by (Height+1) corner array, which <see cref="TerrainImportHeightmapCommand"/>
/// checks against before it will read one back.
/// </summary>
public static class TerrainExportHeightmapCommand
{
    public sealed record ExportResult(
        bool Ok, string Message, byte[]? Png = null, int Width = 0, int Height = 0);

    /// <summary>Encodes the terrain heights as a 16-bit grayscale PNG.</summary>
    public static ExportResult Export(MapDocument doc)
    {
        if (doc.GetFile(TerrainCommand.TerrainFile)?.Model is not MapEnvironment env)
            return new(false, $"map has no {TerrainCommand.TerrainFile} (terrain) to export");

        int w = Wc3.Render.HeightmapPng.GridWidth(env);
        int h = Wc3.Render.HeightmapPng.GridHeight(env);
        try
        {
            byte[] png = Wc3.Render.HeightmapPng.Encode(env);
            return new(true, $"exported {w}x{h} heightmap", png, w, h);
        }
        catch (InvalidDataException ex)
        {
            return new(false, ex.Message);
        }
    }
}
