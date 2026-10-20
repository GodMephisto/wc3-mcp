// src/Wc3.Commands/AssetModelLintCommand.cs
using System.Numerics;
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
    /// <summary>A vertex, normal or UV that is not a finite number.</summary>
    NonFiniteGeometry,
    /// <summary>A normal whose length is zero, so light and culling are undefined.</summary>
    DegenerateNormal,
    /// <summary>A triangle index outside the vertex array, so the draw reads past the buffer.</summary>
    IndexOutOfRange,
    /// <summary>A triangle with zero area, which draws nothing and still costs a face.</summary>
    DegenerateTriangle,
    /// <summary>A texture the model holds but no geoset references.</summary>
    UnusedTexture,
    /// <summary>An animation key that is not a finite number.</summary>
    NonFiniteAnimation,
    /// <summary>A skin bone id the skeleton does not define.</summary>
    MissingSkinBone,
    /// <summary>A skin weight that is negative, or whose four slots do not sum to one.</summary>
    InvalidSkinWeight,
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

    internal static void Inspect(MapDocument? doc, string name, Model3D model, List<ModelLintFinding> findings)
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
            else if (doc is not null
                     && geoset.TextureId < model.Textures.Count
                     && IsConcreteTexture(model.Textures[geoset.TextureId])
                     && RenderModelCommand.FindTextureEntry(doc, model.Textures[geoset.TextureId]) is null)
            {
                findings.Add(new ModelLintFinding(
                    ModelLintKind.MissingTexture, name,
                    $"geoset {i} names a texture the map does not hold, '{model.Textures[geoset.TextureId]}'"));
            }

            CheckBounds(name, i, geoset, findings);
            CheckGeometry(name, i, geoset, model.Skeleton, findings);
        }

        CheckTextures(name, model, findings);

        if (model.Skeleton is { } skeleton)
        {
            CheckSkeleton(name, skeleton, findings);
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

    /// <summary>Finite checks on every geometry array, plus zero-length normals, an index past
    /// the vertex array, a zero-area triangle, and skin data that names a bone the skeleton
    /// does not define or a weight set that does not sum to one.</summary>
    private static void CheckGeometry(
        string name, int index, Geoset geoset, ModelSkeleton? skeleton, List<ModelLintFinding> findings)
    {
        if (HasNonFinite(geoset.Vertices))
        {
            findings.Add(new ModelLintFinding(
                ModelLintKind.NonFiniteGeometry, name,
                $"geoset {index} has a vertex that is not a finite number"));
        }
        if (HasNonFinite(geoset.Normals))
        {
            findings.Add(new ModelLintFinding(
                ModelLintKind.NonFiniteGeometry, name,
                $"geoset {index} has a normal that is not a finite number"));
        }
        if (HasNonFinite(geoset.Uvs))
        {
            findings.Add(new ModelLintFinding(
                ModelLintKind.NonFiniteGeometry, name,
                $"geoset {index} has a UV that is not a finite number"));
        }

        int zeroNormals = 0;
        for (int i = 0; i + 2 < geoset.Normals.Length; i += 3)
        {
            float x = geoset.Normals[i], y = geoset.Normals[i + 1], z = geoset.Normals[i + 2];
            if (float.IsFinite(x) && float.IsFinite(y) && float.IsFinite(z)
                && x * x + y * y + z * z < 1e-8f)
            {
                zeroNormals++;
            }
        }
        if (zeroNormals > 0)
        {
            findings.Add(new ModelLintFinding(
                ModelLintKind.DegenerateNormal, name,
                $"geoset {index} has {zeroNormals} zero-length normal(s), so light and culling are undefined"));
        }

        int vertices = geoset.Vertices.Length / 3;
        int badIndices = 0;
        foreach (int v in geoset.Indices)
        {
            if (v < 0 || v >= vertices) badIndices++;
        }
        if (badIndices > 0)
        {
            findings.Add(new ModelLintFinding(
                ModelLintKind.IndexOutOfRange, name,
                $"geoset {index} has {badIndices} index(es) outside the {vertices} vertex array"));
        }

        int zeroArea = 0;
        for (int i = 0; i + 2 < geoset.Indices.Length; i += 3)
        {
            int a = geoset.Indices[i], b = geoset.Indices[i + 1], c = geoset.Indices[i + 2];
            if (a < 0 || b < 0 || c < 0 || a >= vertices || b >= vertices || c >= vertices) continue;
            if (TriangleAreaSquared(geoset.Vertices, a, b, c) < 1e-12f) zeroArea++;
        }
        if (zeroArea > 0)
        {
            findings.Add(new ModelLintFinding(
                ModelLintKind.DegenerateTriangle, name,
                $"geoset {index} has {zeroArea} triangle(s) with zero area"));
        }

        if (skeleton is null || geoset.SkinBones is not { } bones) return;
        var ids = new HashSet<int>(skeleton.Nodes.Select(n => n.ObjectId));
        int missing = 0;
        foreach (int bone in bones)
        {
            if (bone >= 0 && !ids.Contains(bone)) missing++;
        }
        if (missing > 0)
        {
            findings.Add(new ModelLintFinding(
                ModelLintKind.MissingSkinBone, name,
                $"geoset {index} has {missing} skin slot(s) naming a bone the skeleton does not define"));
        }

        if (geoset.SkinWeights is not { } weights) return;
        int invalid = 0;
        for (int i = 0; i + 3 < weights.Length; i += 4)
        {
            float w0 = weights[i], w1 = weights[i + 1], w2 = weights[i + 2], w3 = weights[i + 3];
            bool negative = w0 < 0 || w1 < 0 || w2 < 0 || w3 < 0;
            bool sums = MathF.Abs(w0 + w1 + w2 + w3 - 1f) <= 0.01f;
            if (negative || !sums) invalid++;
        }
        if (invalid > 0)
        {
            findings.Add(new ModelLintFinding(
                ModelLintKind.InvalidSkinWeight, name,
                $"geoset {index} has {invalid} vertex weight set(s) that are negative or do not sum to one"));
        }
    }

    /// <summary>A texture the model carries but no geoset draws with. A ReplaceableId entry
    /// resolves at runtime and is never reported.</summary>
    private static void CheckTextures(string name, Model3D model, List<ModelLintFinding> findings)
    {
        var used = new HashSet<int>(model.Geosets.Where(g => g.TextureId >= 0).Select(g => g.TextureId));
        for (int i = 0; i < model.Textures.Count; i++)
        {
            if (used.Contains(i) || !IsConcreteTexture(model.Textures[i])) continue;
            findings.Add(new ModelLintFinding(
                ModelLintKind.UnusedTexture, name,
                $"texture {i} '{model.Textures[i]}' is not referenced by any geoset"));
        }
    }

    private static void CheckSkeleton(string name, ModelSkeleton skeleton, List<ModelLintFinding> findings)
    {
        foreach (var node in skeleton.Nodes)
        {
            CheckTrack(name, node.Name, node.Translation, findings);
            CheckTrack(name, node.Name, node.Rotation, findings);
            CheckTrack(name, node.Name, node.Scaling, findings);
        }
    }

    private static void CheckTrack<T>(
        string name, string node, AnimTrack<T>? track, List<ModelLintFinding> findings) where T : struct
    {
        if (track is null) return;
        foreach (var key in track.Keys)
        {
            bool finite = key.Value switch
            {
                Vector3 v => float.IsFinite(v.X) && float.IsFinite(v.Y) && float.IsFinite(v.Z),
                Quaternion q => float.IsFinite(q.X) && float.IsFinite(q.Y)
                    && float.IsFinite(q.Z) && float.IsFinite(q.W),
                _ => true,
            };
            if (finite) continue;
            findings.Add(new ModelLintFinding(
                ModelLintKind.NonFiniteAnimation, name,
                $"node '{node}' has a key at frame {key.Frame} that is not a finite number"));
            return;
        }
    }

    private static bool HasNonFinite(float[] values)
    {
        foreach (float v in values)
        {
            if (!float.IsFinite(v)) return true;
        }
        return false;
    }

    private static float TriangleAreaSquared(float[] vertices, int a, int b, int c)
    {
        float ax = vertices[a * 3], ay = vertices[a * 3 + 1], az = vertices[a * 3 + 2];
        float bx = vertices[b * 3], by = vertices[b * 3 + 1], bz = vertices[b * 3 + 2];
        float cx = vertices[c * 3], cy = vertices[c * 3 + 1], cz = vertices[c * 3 + 2];
        float ux = bx - ax, uy = by - ay, uz = bz - az;
        float vx = cx - ax, vy = cy - ay, vz = cz - az;
        float nx = uy * vz - uz * vy;
        float ny = uz * vx - ux * vz;
        float nz = ux * vy - uy * vx;
        return nx * nx + ny * ny + nz * nz;
    }

}
