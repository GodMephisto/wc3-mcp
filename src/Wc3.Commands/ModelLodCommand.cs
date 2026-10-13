// src/Wc3.Commands/ModelLodCommand.cs
using System.Globalization;

namespace Wc3.Commands;

/// <summary>One level of a level-of-detail chain, the ratio it was built at and what it cost.</summary>
public sealed record ModelLodLevel(
    float Ratio,
    string Path,
    int Vertices,
    int Faces,
    long Bytes);

/// <summary>The whole chain, one level per requested ratio, with the combined output size.</summary>
public sealed record ModelLodResult(
    string Path,
    string OutputDirectory,
    IReadOnlyList<ModelLodLevel> Levels,
    long TotalBytes,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Builds a level-of-detail chain for a model, one MDX per ratio at descending detail, by running
/// <see cref="ModelOptimizeCommand"/> once per ratio. Each level is the full model decimated to the
/// fraction of faces that ratio names, so the materials, textures, sequences, bones, emitters and
/// cameras are written back byte for byte at every level and only the geometry shrinks.
///
/// The ratios are given as a comma separated list, for example 1,0.5,0.25, and are processed in the
/// order given so the chain descends when the caller lists them descending. Ratio 1 is the original
/// geometry and its file is byte identical to the input.
///
/// A ratio outside (0, 1] is refused before anything is written. The output directory is created
/// when missing. The input file is never modified.
/// </summary>
public static class ModelLodCommand
{
    /// <summary>
    /// Parses a comma separated ratio list such as 1,0.5,0.25 into the values to decimate by. Each
    /// must be greater than 0 and at most 1. An empty, non-numeric or out of range entry throws.
    /// </summary>
    public static IReadOnlyList<float> ParseRatios(string? ratios)
    {
        if (string.IsNullOrWhiteSpace(ratios))
            throw new ArgumentException("at least one ratio is required", nameof(ratios));

        var parts = ratios.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 0)
            throw new ArgumentException("at least one ratio is required", nameof(ratios));

        var values = new List<float>(parts.Length);
        foreach (var part in parts)
        {
            if (!float.TryParse(part, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                throw new ArgumentException($"'{part}' is not a number", nameof(ratios));
            if (!(value > 0f) || value > 1f)
                throw new ArgumentException(
                    $"ratio {part} is out of range, must be greater than 0 and at most 1", nameof(ratios));
            values.Add(value);
        }
        return values;
    }

    /// <summary>Reads the input, writes one decimated MDX per ratio into the output directory.</summary>
    public static ModelLodResult ExecutePath(string path, string ratios, string outPath) =>
        ExecutePath(path, ParseRatios(ratios), outPath);

    /// <summary>Reads the input, writes one decimated MDX per ratio into the output directory.</summary>
    public static ModelLodResult ExecutePath(string path, IReadOnlyList<float> ratios, string outPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (!File.Exists(path)) throw new FileNotFoundException($"model not found: {path}", path);
        ArgumentNullException.ThrowIfNull(ratios);
        if (ratios.Count == 0) throw new ArgumentException("at least one ratio is required", nameof(ratios));
        foreach (var ratio in ratios)
            if (!(ratio > 0f) || ratio > 1f)
                throw new ArgumentException(
                    $"ratio {ratio} is out of range, must be greater than 0 and at most 1", nameof(ratios));
        ArgumentException.ThrowIfNullOrWhiteSpace(outPath);

        string directory = Path.GetFullPath(outPath);
        Directory.CreateDirectory(directory);

        string stem = Path.GetFileNameWithoutExtension(path);
        if (string.IsNullOrWhiteSpace(stem)) stem = "model";

        var levels = new List<ModelLodLevel>(ratios.Count);
        var diagnostics = new List<string>();
        long totalBytes = 0;

        for (int index = 0; index < ratios.Count; index++)
        {
            float ratio = ratios[index];
            string name = $"{stem}_lod{index}_{RatioTag(ratio)}.mdx";
            string dest = Path.Combine(directory, name);
            var r = ModelOptimizeCommand.ExecutePath(path, ratio, dest);
            levels.Add(new ModelLodLevel(ratio, dest, r.VerticesAfter, r.FacesAfter, r.BytesAfter));
            totalBytes += r.BytesAfter;
            foreach (var d in r.Diagnostics) diagnostics.Add($"{name}: {d}");
        }

        return new ModelLodResult(path, directory, levels, totalBytes, diagnostics);
    }

    /// <summary>A filename safe tag for a ratio, the invariant decimal with the point as an underscore.</summary>
    private static string RatioTag(float ratio) =>
        ratio.ToString("0.###", CultureInfo.InvariantCulture).Replace('.', '_');
}
