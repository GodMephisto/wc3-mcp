// src/Wc3.Commands/MapOptimizeCommand.cs
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One imported file the batch optimiser replaced, with what it held before and after.</summary>
public sealed record MapOptimizeFile(
    string Path,
    string Kind,
    long BytesBefore,
    long BytesAfter,
    string Detail)
{
    public long BytesSaved => BytesBefore - BytesAfter;
}

/// <summary>
/// What a map optimise pass changed. <see cref="Files"/> holds one entry per replaced file,
/// <see cref="ModelsOptimized"/> and <see cref="TexturesOptimized"/> are the per-kind counts, and
/// <see cref="BytesBefore"/> and <see cref="BytesAfter"/> are the whole map's size on disk when it
/// was loaded from and saved to a path. <see cref="Diagnostics"/> lists the files that were skipped
/// or could not be optimised, so a skipped file never reads as a smaller one.
/// </summary>
public sealed record MapOptimizeResult(
    string Path,
    int ModelsOptimized,
    int TexturesOptimized,
    IReadOnlyList<MapOptimizeFile> Files,
    long BytesBefore,
    long BytesAfter,
    IReadOnlyList<string> Diagnostics)
{
    /// <summary>How many imported files were replaced.</summary>
    public int Changed => Files.Count;

    /// <summary>The bytes recovered across every replaced file, the honest saving the per-file
    /// numbers add up to.</summary>
    public long BytesSaved => Files.Sum(f => f.BytesSaved);
}

/// <summary>
/// Shrinks every imported model and texture inside a map in one pass and writes a new map with
/// only those files replaced, everything else byte for byte. It walks the map's own imported
/// entries through <see cref="AssetReferences.ImportedEntries"/> and hands each one to the existing
/// per-file command, <see cref="ModelOptimizeCommand"/> for a .mdx and
/// <see cref="TextureOptimizeCommand"/> for an image, so no decimation or scaling is re-implemented
/// here.
///
/// A model is decimated at <c>ratio</c>, the target fraction of its faces to keep, and a texture is
/// scaled to fit <c>maxTexture</c> on its longest side. A file whose optimised bytes are not smaller
/// is left untouched and the map keeps its original bytes, so the pass never inflates a file. The
/// change is applied through <see cref="MapDocument.AddOrReplaceRawFile"/>, which marks the entry
/// dirty, and the untouched entries keep their original archive bytes on save.
///
/// The input map is never modified here. The caller saves an edited copy, the same no-clobber
/// stance every other map command takes.
/// </summary>
public static class MapOptimizeCommand
{
    private static readonly HashSet<string> ModelExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".mdx", ".mdl" };

    /// <summary>The image formats the shared converter can both decode and re-encode, so a texture
    /// in any of them is a candidate. DDS is source-only and is left out, since the converter has no
    /// DDS encoder and a silent fallback would be a lie.</summary>
    private static readonly HashSet<string> TextureExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".blp", ".png", ".jpg", ".jpeg", ".bmp", ".tga", ".gif",
        };

    /// <summary>Loads the map, optimises its imported models and textures, and writes the edited
    /// copy to <paramref name="outPath"/>.</summary>
    public static MapOptimizeResult ExecutePath(string mapPath, float ratio, int maxTexture, string outPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mapPath);
        if (!File.Exists(mapPath)) throw new FileNotFoundException($"map not found: {mapPath}", mapPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(outPath);

        var doc = MapDocument.Load(mapPath);
        var result = Execute(doc, ratio, maxTexture);
        doc.Save(outPath);
        return result with
        {
            Path = outPath,
            BytesBefore = new FileInfo(mapPath).Length,
            BytesAfter = new FileInfo(outPath).Length,
        };
    }

    /// <summary>Optimises the map in memory and returns the report. The front-end saves.</summary>
    public static MapOptimizeResult Execute(MapDocument doc, float ratio, int maxTexture)
    {
        ArgumentNullException.ThrowIfNull(doc);
        if (ratio is <= 0f or > 1f)
            throw new ArgumentOutOfRangeException(nameof(ratio), ratio, "ratio must be in (0, 1]");
        if (maxTexture <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxTexture), maxTexture, "max texture side must be positive");

        var files = new List<MapOptimizeFile>();
        var diagnostics = new List<string>();
        int models = 0, textures = 0;

        foreach (var entry in AssetReferences.ImportedEntries(doc))
        {
            var name = entry.FileName!;
            var ext = Path.GetExtension(name);
            if (ModelExtensions.Contains(ext))
            {
                if (OptimizeModel(doc, entry, name, ratio, diagnostics) is { } changed)
                {
                    files.Add(changed);
                    models++;
                }
            }
            else if (TextureExtensions.Contains(ext))
            {
                if (OptimizeTexture(doc, entry, name, ext, maxTexture, diagnostics) is { } changed)
                {
                    files.Add(changed);
                    textures++;
                }
            }
        }

        files.Sort((a, b) => string.Compare(a.Path, b.Path, StringComparison.OrdinalIgnoreCase));
        return new MapOptimizeResult("", models, textures, files, 0, 0, diagnostics);
    }

    private static MapOptimizeFile? OptimizeModel(
        MapDocument doc, MapFileEntry entry, string name, float ratio, List<string> diagnostics)
    {
        var source = entry.CurrentBytes;
        if (source.Length == 0)
        {
            diagnostics.Add($"'{name}' is empty, skipped");
            return null;
        }

        ModelOptimizeResult r;
        byte[] output;
        try
        {
            r = ModelOptimizeCommand.Execute(source, name, ratio, out output);
        }
        catch (Exception ex)
        {
            diagnostics.Add($"'{name}' could not be optimised, {ex.Message}");
            return null;
        }
        foreach (var d in r.Diagnostics) diagnostics.Add($"{name}: {d}");
        if (output.Length >= source.LongLength) return null; // nothing reduced, keep the original bytes

        doc.AddOrReplaceRawFile(name, output);
        return new MapOptimizeFile(name, "model", source.LongLength, output.LongLength,
            $"vertices {r.VerticesBefore}->{r.VerticesAfter}, faces {r.FacesBefore}->{r.FacesAfter}");
    }

    private static MapOptimizeFile? OptimizeTexture(
        MapDocument doc, MapFileEntry entry, string name, string ext, int maxTexture, List<string> diagnostics)
    {
        var source = entry.CurrentBytes;
        if (source.Length == 0)
        {
            diagnostics.Add($"'{name}' is empty, skipped");
            return null;
        }

        TextureOptimizeResult r;
        byte[] output;
        try
        {
            r = TextureOptimizeCommand.Execute(source, name, ext, maxTexture, out output);
        }
        catch (Exception ex)
        {
            diagnostics.Add($"'{name}' could not be optimised, {ex.Message}");
            return null;
        }
        // Neither side was over the limit, so nothing was scaled and re-encoding would only change
        // the file, not shrink the map.
        if (r.WidthAfter == r.WidthBefore && r.HeightAfter == r.HeightBefore) return null;
        if (output.Length >= source.LongLength)
        {
            diagnostics.Add($"'{name}' did not shrink, left unchanged");
            return null;
        }

        doc.AddOrReplaceRawFile(name, output);
        return new MapOptimizeFile(name, "texture", source.LongLength, output.LongLength,
            $"{r.WidthBefore}x{r.HeightBefore} -> {r.WidthAfter}x{r.HeightAfter}");
    }
}
