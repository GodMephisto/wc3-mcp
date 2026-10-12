// src/Wc3.Commands/PathingImportPngCommand.cs
using War3Net.Build.Environment;   // MapPathingMap, PathingType
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Sets a map's war3map.wpm pathing flags from a colour-coded PNG produced by
/// <see cref="PathingExportPngCommand"/>. Each cell's flags are replaced by the value its
/// pixel decodes to, the top-left (Width by Height) region of the image, so an exported
/// image with a legend to its right can be re-imported without cropping. A pixel whose
/// colour the palette does not produce is refused rather than read as blank.
///
/// <para>The input map is never written here, the caller saves the returned (dirty)
/// document to a copy.</para>
/// </summary>
public static class PathingImportPngCommand
{
    public sealed record ImportResult(bool Ok, string Message, int CellsChanged = 0);

    /// <summary>Reads <paramref name="png"/> and applies it to the pathing grid.</summary>
    public static ImportResult Import(MapDocument doc, byte[] png)
    {
        if (doc.GetFile(PathingCommand.PathingFile)?.Model is not MapPathingMap map)
            return new(false, $"map has no {PathingCommand.PathingFile} (pathing map) to import into");

        int w = (int)map.Width, h = (int)map.Height;
        var cells = map.Cells;
        if (cells is null || cells.Count != w * h)
            return new(false,
                $"pathing map cell buffer inconsistent (Width={w} Height={h} Cells={cells?.Count ?? 0})");

        if (!Wc3.Render.PathingPng.TryDecodeRegion(png, w, h, out var decoded, out var error))
            return new(false, error);

        int changed = 0;
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i] != decoded[i])
            {
                cells[i] = decoded[i];
                changed++;
            }
        }

        if (changed > 0)
            doc.AddOrReplaceModelFile(PathingCommand.PathingFile, map);
        return new(true, $"set {changed} pathing cells from a {w}x{h} image", changed);
    }
}
