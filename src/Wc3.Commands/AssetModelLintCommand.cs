// src/Wc3.Commands/AssetModelLintCommand.cs
using Wc3.Model;
using Wc3.Modeling;

namespace Wc3.Commands;

/// <summary>The problem a model scan reports. Each one is a shape the game renders wrongly or
/// drops in silence, so it never reaches a log the author reads.</summary>
public enum ModelLintKind
{
    /// <summary>A geoset whose material or texture does not resolve, so it draws nothing.</summary>
    GeosetWithoutMaterial,
    /// <summary>A material naming a texture path the map does not hold.</summary>
    MissingTexture,
    /// <summary>A sequence whose interval start and end are equal, so it plays for zero frames.</summary>
    ZeroDurationSequence,
    /// <summary>A bounding box with a non-finite or absurdly large coordinate.</summary>
    OutOfRangeBounds,
}

/// <summary>One reported problem, the model holding it and a short human detail.</summary>
public sealed record ModelLintFinding(ModelLintKind Kind, string Path, string Detail);

/// <summary>
/// The scan outcome. <see cref="ModelsScanned"/> counts the imported models that parsed,
/// <see cref="Findings"/> holds every problem in model order, and <see cref="Diagnostics"/>
/// carries the models that could not be read at all so a parse failure never hides a run.
/// </summary>
public sealed record AssetModelLintResult(
    int ModelsScanned,
    IReadOnlyList<ModelLintFinding> Findings,
    IReadOnlyList<string> Diagnostics)
{
    public int FindingCount => Findings.Count;
}

/// <summary>
/// Scans every imported model in a map for the four structural problems a static check can
/// find without a WC3 install. It reuses the existing MDX/MDL parser (<see cref="ModelParser"/>,
/// which dispatches to the binary MDX reader) and never re-implements a chunk reader.
///
/// The four checks. A geoset whose material and texture do not resolve, which the game draws
/// as nothing with no message. A material naming a texture path the map does not hold, which
/// renders flat or not at all depending on the client. A sequence whose interval start and end
/// are equal, a zero-frame animation an author usually created by hand and never noticed. And a
/// bounding box that is not finite or whose largest coordinate reaches past the Warcraft III
/// world half extent of 16384 units, the tell for a stray transform or a corrupt vertex.
///
/// Read-only. It parses bytes in memory and changes nothing.
/// </summary>
public static class AssetModelLintCommand
{
    /// <summary>The Warcraft III world half extent in map units. A model vertex beyond this is
    /// off any playable map, so the box is reported rather than trusted.</summary>
    public const float MaxCoordinate = 16384f;

    private static readonly HashSet<string> ModelExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".mdx", ".mdl" };

    public static AssetModelLintResult Execute(MapDocument doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var findings = new List<ModelLintFinding>();
        var diagnostics = new List<string>();
        int scanned = 0;

        foreach (var entry in AssetReferences.ImportedEntries(doc))
        {
            var name = entry.FileName!;
            if (!ModelExtensions.Contains(Path.GetExtension(name))) continue;
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
            Inspect(doc, name, model, findings);
        }

        findings.Sort((a, b) =>
        {
            int byPath = string.Compare(a.Path, b.Path, StringComparison.OrdinalIgnoreCase);
            return byPath != 0 ? byPath : a.Kind.CompareTo(b.Kind);
        });
        return new AssetModelLintResult(scanned, findings, diagnostics);
    }

    private static void Inspect(MapDocument doc, string name, Model3D model, List<ModelLintFinding> findings)
    {
        for (int i = 0; i < model.Geosets.Count; i++)
        {
            var geoset = model.Geosets[i];
            if (geoset.TextureId < 0)
            {
                findings.Add(new ModelLintFinding(
                    ModelLintKind.GeosetWithoutMaterial, name,
                    $"geoset {i} has no material, so it draws nothing"));
            }
            else if (geoset.TextureId < model.Textures.Count
                     && IsConcreteTexture(model.Textures[geoset.TextureId])
                     && RenderModelCommand.FindTextureEntry(doc, model.Textures[geoset.TextureId]) is null)
            {
                findings.Add(new ModelLintFinding(
                    ModelLintKind.MissingTexture, name,
                    $"geoset {i} names a texture the map does not hold, '{model.Textures[geoset.TextureId]}'"));
            }

            CheckBounds(name, i, geoset, findings);
        }

        if (model.Skeleton is { } skeleton)
        {
            foreach (var sequence in skeleton.Sequences)
            {
                if (sequence.IntervalEnd - sequence.IntervalStart <= 0)
                {
                    findings.Add(new ModelLintFinding(
                        ModelLintKind.ZeroDurationSequence, name,
                        $"sequence '{sequence.Name}' spans {sequence.IntervalStart} to {sequence.IntervalEnd}"));
                }
            }
        }
    }

    /// <summary>A texture a material can actually point at. A ReplaceableId entry (team colour)
    /// or an empty string names no file and is never a missing one.</summary>
    private static bool IsConcreteTexture(string path) =>
        !string.IsNullOrWhiteSpace(path)
        && !path.StartsWith("ReplaceableId:", StringComparison.OrdinalIgnoreCase);

    private static void CheckBounds(string name, int index, Geoset geoset, List<ModelLintFinding> findings)
    {
        if (geoset.Vertices.Length < 3) return;

        float minX = float.MaxValue, minY = float.MaxValue, minZ = float.MaxValue;
        float maxX = float.MinValue, maxY = float.MinValue, maxZ = float.MinValue;
        for (int i = 0; i + 2 < geoset.Vertices.Length; i += 3)
        {
            float x = geoset.Vertices[i], y = geoset.Vertices[i + 1], z = geoset.Vertices[i + 2];
            if (!float.IsFinite(x) || !float.IsFinite(y) || !float.IsFinite(z))
            {
                findings.Add(new ModelLintFinding(
                    ModelLintKind.OutOfRangeBounds, name,
                    $"geoset {index} has a vertex that is not a finite number, so its bounding box is undefined"));
                return;
            }
            if (x < minX) minX = x;
            if (y < minY) minY = y;
            if (z < minZ) minZ = z;
            if (x > maxX) maxX = x;
            if (y > maxY) maxY = y;
            if (z > maxZ) maxZ = z;
        }

        float reach = MathF.Max(
            MathF.Max(MathF.Abs(minX), MathF.Abs(maxX)),
            MathF.Max(
                MathF.Max(MathF.Abs(minY), MathF.Abs(maxY)),
                MathF.Max(MathF.Abs(minZ), MathF.Abs(maxZ))));
        if (reach > MaxCoordinate)
        {
            findings.Add(new ModelLintFinding(
                ModelLintKind.OutOfRangeBounds, name,
                $"geoset {index} bounds reach {reach:0} units, past the {MaxCoordinate:0} unit world extent"));
        }
    }
}
