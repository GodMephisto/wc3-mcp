// src/Wc3.Commands/PathingExportPngCommand.cs
using SixLabors.ImageSharp.PixelFormats;   // Rgba32
using War3Net.Build.Environment;            // MapPathingMap
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Exports a map's war3map.wpm pathing grid as a colour-coded PNG with a legend strip.
/// Read-only. One pixel per pathing cell in the top-left corner, in the map's storage
/// order, with a swatch per known flag to the right. The structured legend is returned so
/// a front-end can print flag names and hex colours beside the image path, which is how
/// the image stays self-describing without drawing text into it.
/// </summary>
public static class PathingExportPngCommand
{
    /// <summary>One legend row for a front-end, a flag name, the raw flag value, and its colour.</summary>
    public sealed record LegendEntry(string Name, string Flags, string Colour);

    public sealed record ExportResult(
        bool Ok, string Message, byte[]? Png = null, int Width = 0, int Height = 0,
        IReadOnlyList<LegendEntry>? Legend = null);

    /// <summary>Renders the pathing grid and legend as PNG bytes.</summary>
    public static ExportResult Export(MapDocument doc)
    {
        if (doc.GetFile(PathingCommand.PathingFile)?.Model is not MapPathingMap map)
            return new(false, $"map has no {PathingCommand.PathingFile} (pathing map) to export");

        int w = (int)map.Width, h = (int)map.Height;
        try
        {
            byte[] png = Wc3.Render.PathingPng.Encode(map);
            var legend = Wc3.Render.PathingPng.Legend
                .Select(l => new LegendEntry(l.Name, l.Flags.ToString(), Hex(l.Colour)))
                .ToList();
            return new(true, $"exported {w}x{h} pathing map", png, w, h, legend);
        }
        catch (InvalidDataException ex)
        {
            return new(false, ex.Message);
        }
    }

    private static string Hex(Rgba32 c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
}
