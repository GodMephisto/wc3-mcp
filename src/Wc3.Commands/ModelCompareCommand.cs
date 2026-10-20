// src/Wc3.Commands/ModelCompareCommand.cs
using Wc3.Modeling;
using Wc3.Render;

namespace Wc3.Commands;

/// <summary>One camera angle and how the two renders scored there.</summary>
public sealed record ModelAngleScore(float Yaw, float Pitch, double Ssim, double SilhouetteIou);

/// <summary>
/// The comparison. <see cref="MeanSsim"/> averages the angles, <see cref="MinSilhouetteIou"/>
/// is the worst silhouette overlap, and a low one at any angle is the signal that a model
/// changed shape rather than only shading.
/// </summary>
public sealed record ModelCompareResult(
    string A,
    string B,
    int Width,
    int Height,
    IReadOnlyList<ModelAngleScore> Angles,
    double MeanSsim,
    double MinSilhouetteIou,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// The perceptual gate the lossy pipeline is built on. Parses two models, resolves their
/// textures from the folder beside each (or one folder given for both), renders both from a
/// set of yaw and pitch angles with the same rasterizer that produces thumbnails, and scores
/// each pair by SSIM and silhouette overlap. Read-only, it writes nothing.
/// </summary>
public static class ModelCompareCommand
{
    public static readonly (float Yaw, float Pitch)[] DefaultAngles =
    {
        (0f, 15f), (90f, 15f), (180f, 15f), (270f, 15f),
    };

    public static ModelCompareResult Execute(
        string pathA,
        string pathB,
        int width = 256,
        int height = 256,
        IReadOnlyList<(float Yaw, float Pitch)>? angles = null,
        string? textureFolder = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pathA);
        ArgumentException.ThrowIfNullOrWhiteSpace(pathB);

        var modelA = ModelParser.Parse(File.ReadAllBytes(pathA), Path.GetFileName(pathA));
        var modelB = ModelParser.Parse(File.ReadAllBytes(pathB), Path.GetFileName(pathB));
        var folderA = textureFolder ?? Path.GetDirectoryName(Path.GetFullPath(pathA))!;
        var folderB = textureFolder ?? Path.GetDirectoryName(Path.GetFullPath(pathB))!;
        var texturesA = AssetThumbnailsCommand.ResolveFolderTextures(folderA, modelA);
        var texturesB = AssetThumbnailsCommand.ResolveFolderTextures(folderB, modelB);

        var scores = new List<ModelAngleScore>();
        var diagnostics = new List<string>();
        foreach (var (yaw, pitch) in angles ?? DefaultAngles)
        {
            try
            {
                var pngA = ModelRenderer.RenderPng(modelA, texturesA, width, height, yaw, pitch);
                var pngB = ModelRenderer.RenderPng(modelB, texturesB, width, height, yaw, pitch);
                var score = AppearanceMetric.Compare(pngA, pngB);
                scores.Add(new ModelAngleScore(yaw, pitch, score.Ssim, score.SilhouetteIou));
            }
            catch (Exception ex)
            {
                diagnostics.Add($"angle {yaw:0},{pitch:0} could not be compared, {ex.Message}");
            }
        }

        double meanSsim = scores.Count == 0 ? 0 : scores.Average(s => s.Ssim);
        double minIou = scores.Count == 0 ? 0 : scores.Min(s => s.SilhouetteIou);
        return new ModelCompareResult(
            pathA, pathB, width, height, scores, meanSsim, minIou, diagnostics);
    }
}
