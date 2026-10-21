// src/Wc3.Commands/ModelSquishCommand.cs
using Wc3.Modeling;

namespace Wc3.Commands;

/// <summary>One pass and what it cost. <see cref="Kept"/> is false when the render gate
/// refused the result, and <see cref="Note"/> says why.</summary>
public sealed record SquishStep(
    string Name,
    long BytesBefore,
    long BytesAfter,
    double Ssim,
    double SilhouetteIou,
    bool Kept,
    string? Note);

/// <summary>
/// The whole run. <see cref="BytesBefore"/> and <see cref="BytesAfter"/> are the model file,
/// the texture totals are the referenced textures the run wrote, and <see cref="Steps"/> is the
/// receipt, one row per pass, kept or refused.
/// </summary>
public sealed record ModelSquishResult(
    string Path,
    string OutPath,
    long BytesBefore,
    long BytesAfter,
    int TexturesSquished,
    long TextureBytesBefore,
    long TextureBytesAfter,
    IReadOnlyList<SquishStep> Steps,
    double FinalSsim,
    double FinalSilhouetteIou,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// A lossy run with a gate. Applies the passes cheapest first, the lossless key drop and then
/// quadric decimation, and after each one renders the original and the candidate from the
/// comparison angles and keeps the pass only when SSIM and silhouette stay above the floor.
/// Textures the model references are optionally shrunk into a side folder, since they live
/// beside the model rather than inside the MDX. Read-only over the input, writes only the
/// output model and the texture copies.
/// </summary>
public static class ModelSquishCommand
{
    public static ModelSquishResult Execute(
        string path,
        string outPath,
        float ratio = 0.5f,
        bool lossless = true,
        bool decimate = true,
        int textureSide = 0,
        string? textureOutDir = null,
        double minSsim = 0.90,
        double minSilhouette = 0.85,
        bool stripEffects = false,
        int gateSize = 128)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(outPath);
        if (textureSide != 0 && textureOutDir is null)
        {
            throw new ArgumentException("a texture side needs an output folder for the textures", nameof(textureOutDir));
        }

        var bytesBefore = new FileInfo(path).Length;
        var steps = new List<SquishStep>();
        var diagnostics = new List<string>();
        string best = path;
        double bestSsim = 1.0, bestIou = 1.0;
        long bestBytes = bytesBefore;

        var work = Path.Combine(Path.GetTempPath(), "wc3squish-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(work);
        try
        {
            if (lossless)
            {
                var candidate = Path.Combine(work, "lossless.mdx");
                try
                {
                    var r = ModelKeyOptimizeCommand.ExecutePath(best, candidate);
                    var (ssim, iou) = Gate(path, candidate, gateSize);
                    bool kept = ssim >= minSsim && iou >= minSilhouette;
                    steps.Add(new SquishStep("lossless keys", r.BytesBefore, r.BytesAfter,
                        ssim, iou, kept, kept ? null : "below the render gate"));
                    if (kept) { best = candidate; bestBytes = r.BytesAfter; bestSsim = ssim; bestIou = iou; }
                }
                catch (Exception ex)
                {
                    diagnostics.Add($"lossless keys failed, {ex.Message}");
                }
            }

            if (decimate)
            {
                var candidate = Path.Combine(work, "decimated.mdx");
                try
                {
                    var r = ModelOptimizeCommand.ExecutePath(best, ratio, candidate, stripEffects);
                    var (ssim, iou) = Gate(path, candidate, gateSize);
                    bool kept = ssim >= minSsim && iou >= minSilhouette;
                    steps.Add(new SquishStep("decimate", r.BytesBefore, r.BytesAfter,
                        ssim, iou, kept, kept ? null : "below the render gate"));
                    if (kept) { best = candidate; bestBytes = r.BytesAfter; bestSsim = ssim; bestIou = iou; }
                }
                catch (Exception ex)
                {
                    diagnostics.Add($"decimate failed, {ex.Message}");
                }
            }

            File.Copy(best, outPath, overwrite: true);

            int squished = 0;
            long texBefore = 0, texAfter = 0;
            if (textureSide > 0 && textureOutDir is not null)
            {
                Directory.CreateDirectory(textureOutDir);
                var folder = Path.GetDirectoryName(Path.GetFullPath(path))!;
                var model = ModelParser.Parse(File.ReadAllBytes(best), Path.GetFileName(best));
                foreach (var texture in model.Textures.Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(texture)
                        || texture.StartsWith("ReplaceableId:", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    var source = Path.Combine(folder, Path.GetFileName(texture));
                    if (!File.Exists(source))
                    {
                        diagnostics.Add($"texture '{texture}' not found beside the model");
                        continue;
                    }
                    try
                    {
                        var destination = Path.Combine(textureOutDir, Path.GetFileName(texture));
                        var r = TextureOptimizeCommand.ExecutePath(source, textureSide, destination);
                        texBefore += r.BytesBefore;
                        texAfter += r.BytesAfter;
                        squished++;
                    }
                    catch (Exception ex)
                    {
                        diagnostics.Add($"texture '{texture}' could not be shrunk, {ex.Message}");
                    }
                }
            }

            return new ModelSquishResult(
                path, outPath, bytesBefore, bestBytes,
                squished, texBefore, texAfter,
                steps, bestSsim, bestIou, diagnostics);
        }
        finally
        {
            try { Directory.Delete(work, true); } catch { /* best effort */ }
        }
    }

    private static (double Ssim, double Iou) Gate(string original, string candidate, int size)
    {
        var folder = Path.GetDirectoryName(Path.GetFullPath(original))!;
        var r = ModelCompareCommand.Execute(original, candidate, size, size, textureFolder: folder);
        if (r.Angles.Count == 0)
        {
            var reason = r.Diagnostics.Count > 0 ? r.Diagnostics[0] : "no angle rendered";
            throw new InvalidDataException($"the gate could not render the pair, {reason}");
        }
        return (r.MeanSsim, r.MinSilhouetteIou);
    }
}
