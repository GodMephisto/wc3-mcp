// src/Wc3.Commands/LeakModelCommand.cs
using System.Security.Cryptography;
using Wc3.Model;
using Wc3.Modeling;

namespace Wc3.Commands;

/// <summary>The model-level leak or waste a map carries, the join between its imported models and
/// textures and what the map and those models actually reference.</summary>
public enum ModelLeakKind
{
    /// <summary>An imported model no object-data field value and no script literal names.</summary>
    LeakedImport,
    /// <summary>A model naming a concrete texture the map does not hold.</summary>
    MissingTexture,
    /// <summary>A model imported again under a different name with byte-identical content.</summary>
    DuplicateImport,
    /// <summary>An imported texture no imported model names.</summary>
    UnusedTexture,
}

/// <summary>One finding, the file it names and a short human reason.</summary>
public sealed record ModelLeakFinding(ModelLeakKind Kind, string Path, string Reason);

/// <summary>
/// The scan outcome. <see cref="ModelsScanned"/> counts the imported models that parsed,
/// <see cref="TexturesScanned"/> counts the imported textures considered, and
/// <see cref="Findings"/> holds every finding in file order. <see cref="Diagnostics"/> carries
/// the models that could not be read, so a parse failure never reads as a clean run. The named
/// totals are the per-kind counts a caller renders or gates on.
/// </summary>
public sealed record LeakModelResult(
    int ModelsScanned,
    int TexturesScanned,
    IReadOnlyList<ModelLeakFinding> Findings,
    IReadOnlyList<string> Diagnostics)
{
    public int FindingCount => Findings.Count;
    public int LeakedImports => Count(ModelLeakKind.LeakedImport);
    public int MissingTextures => Count(ModelLeakKind.MissingTexture);
    public int DuplicateImports => Count(ModelLeakKind.DuplicateImport);
    public int UnusedTextures => Count(ModelLeakKind.UnusedTexture);

    private int Count(ModelLeakKind kind) => Findings.Count(f => f.Kind == kind);
}

/// <summary>
/// Reports the model-level leaks and waste across a map's imported models and textures. This is
/// the join the model audit and the import table did not have, each imported model checked
/// against what the map references and each imported texture against what its models name.
///
/// The four checks. A model imported but named by no object-data field value and no script
/// literal, a leaked import the map ships but never loads. A model whose material names a
/// concrete texture the map does not hold, which draws flat or not at all. A model imported more
/// than once under different names but byte-identical, the duplicate the optimiser can drop. And
/// a texture imported but named by no imported model, the model-scoped half of an unused asset.
///
/// It reuses the existing MDX/MDL parser (<see cref="ModelParser"/>) and the map's own import and
/// reference primitives (<see cref="AssetReferences"/>), so it re-implements no chunk reader and
/// no MPQ reader. Read-only, it parses bytes in memory and changes nothing.
/// </summary>
public static class LeakModelCommand
{
    private static readonly HashSet<string> ModelExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".mdx", ".mdl" };

    private static readonly HashSet<string> TextureExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".blp", ".tga", ".dds" };

    public static LeakModelResult Execute(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var findings = new List<ModelLeakFinding>();
        var diagnostics = new List<string>();

        var entries = AssetReferences.ImportedEntries(doc);
        var modelEntries = entries
            .Where(e => ModelExtensions.Contains(Path.GetExtension(e.FileName!)))
            .ToList();
        var textureEntries = entries
            .Where(e => TextureExtensions.Contains(Path.GetExtension(e.FileName!)))
            .ToList();

        int scanned = 0;
        var modelTexturePaths = new List<(string Model, string Texture)>();
        foreach (var entry in modelEntries)
        {
            var name = entry.FileName!;
            if (entry.CurrentBytes.Length == 0)
            {
                diagnostics.Add($"'{name}' is empty, skipped");
                continue;
            }

            Model3D model;
            try
            {
                model = ModelParser.Parse(entry.CurrentBytes, name);
            }
            catch (Exception ex)
            {
                diagnostics.Add($"'{name}' could not be parsed, {ex.Message}");
                continue;
            }
            scanned++;
            foreach (var texture in UsedTextures(model))
                modelTexturePaths.Add((name, texture));
        }

        ReportLeakedImports(doc, modelEntries, findings);
        ReportMissingTextures(doc, modelTexturePaths, findings);
        ReportDuplicateImports(modelEntries, findings);
        ReportUnusedTextures(textureEntries, modelTexturePaths, findings);

        findings.Sort((a, b) =>
        {
            int byPath = string.Compare(a.Path, b.Path, StringComparison.OrdinalIgnoreCase);
            return byPath != 0 ? byPath : a.Kind.CompareTo(b.Kind);
        });
        return new LeakModelResult(scanned, textureEntries.Count, findings, diagnostics);
    }

    /// <summary>A model named by no object-data field value and no script literal, the reference
    /// set <see cref="AssetReferences.ReferencedSpellings"/> builds the way the engine loads it.</summary>
    private static void ReportLeakedImports(
        MapDocument doc, IReadOnlyList<MapFileEntry> modelEntries, List<ModelLeakFinding> findings)
    {
        var referenced = AssetReferences.ReferencedSpellings(doc);
        foreach (var entry in modelEntries)
        {
            var name = entry.FileName!;
            if (!referenced.Contains(AssetReferences.Normalize(name)))
            {
                findings.Add(new ModelLeakFinding(ModelLeakKind.LeakedImport, name,
                    "no object field or script literal names this imported model"));
            }
        }
    }

    /// <summary>A concrete texture a model uses that no map entry backs. A ReplaceableId entry
    /// (team colour) names no file and is never a missing one.</summary>
    private static void ReportMissingTextures(
        MapDocument doc, IReadOnlyList<(string Model, string Texture)> modelTexturePaths,
        List<ModelLeakFinding> findings)
    {
        foreach (var (model, texture) in modelTexturePaths)
        {
            if (RenderModelCommand.FindTextureEntry(doc, texture) is null)
            {
                findings.Add(new ModelLeakFinding(ModelLeakKind.MissingTexture, model,
                    $"names a texture the map does not hold, '{texture}'"));
            }
        }
    }

    /// <summary>Whole models whose content is identical, one finding per digest where two names
    /// share it. A zero-byte entry holds nothing and deduplicates nothing, so it is skipped.</summary>
    private static void ReportDuplicateImports(
        IReadOnlyList<MapFileEntry> modelEntries, List<ModelLeakFinding> findings)
    {
        var groups = modelEntries
            .Where(e => e.CurrentSize > 0)
            .GroupBy(e => Convert.ToHexString(SHA256.HashData(e.CurrentBytes)).ToLowerInvariant())
            .Select(g => g.Select(e => e.FileName!).Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToList())
            .Where(names => names.Count > 1);

        foreach (var names in groups)
        {
            findings.Add(new ModelLeakFinding(ModelLeakKind.DuplicateImport, names[0],
                $"byte-identical to {string.Join(", ", names.Skip(1))}"));
        }
    }

    /// <summary>An imported texture no imported model's material names, the model-scoped half of
    /// an unused asset. A texture reached only by an object field or a script literal (an icon)
    /// is still reported here, because the check is what the models name and nothing else.</summary>
    private static void ReportUnusedTextures(
        IReadOnlyList<MapFileEntry> textureEntries,
        IReadOnlyList<(string Model, string Texture)> modelTexturePaths,
        List<ModelLeakFinding> findings)
    {
        foreach (var entry in textureEntries)
        {
            var name = entry.FileName!;
            if (!modelTexturePaths.Any(t => AssetReferences.TokenNames(t.Texture, name)))
            {
                findings.Add(new ModelLeakFinding(ModelLeakKind.UnusedTexture, name,
                    "no imported model names this texture"));
            }
        }
    }

    /// <summary>The distinct concrete texture paths a model's geosets resolve to, in geoset order.
    /// A path is concrete when a material can point at a file. A ReplaceableId entry or an empty
    /// string names no file.</summary>
    private static IEnumerable<string> UsedTextures(Model3D model)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var geoset in model.Geosets)
        {
            if (geoset.TextureId < 0 || geoset.TextureId >= model.Textures.Count) continue;
            var texture = model.Textures[geoset.TextureId];
            if (!IsConcreteTexture(texture)) continue;
            if (seen.Add(texture)) yield return texture;
        }
    }

    private static bool IsConcreteTexture(string path) =>
        !string.IsNullOrWhiteSpace(path)
        && !path.StartsWith("ReplaceableId:", StringComparison.OrdinalIgnoreCase);
}
