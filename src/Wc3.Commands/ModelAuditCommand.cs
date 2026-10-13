// src/Wc3.Commands/ModelAuditCommand.cs
using System.Numerics;
using Wc3.Modeling;

namespace Wc3.Commands;

/// <summary>A resource problem in a model that wastes space or draws nothing.</summary>
public enum ModelIssueKind
{
    /// <summary>The same texture path appears more than once in the texture list.</summary>
    DuplicateTexture,
    /// <summary>A geoset with no resolvable texture, so it draws nothing or draws flat.</summary>
    OrphanGeoset,
    /// <summary>A geoset with no triangles.</summary>
    EmptyGeoset,
    /// <summary>A triangle whose three points are collinear, so it has no area.</summary>
    ZeroAreaFace,
    /// <summary>A skeleton node no geoset's skin data references.</summary>
    UnusedNode,
    /// <summary>A vertex coordinate that is not finite.</summary>
    NonFiniteVertex,
    /// <summary>A particle emitter whose node objectId is outside the model's node list.</summary>
    DanglingEmitterNode,
    /// <summary>A particle emitter 2 whose texture id is not a valid index into the texture list.</summary>
    DanglingEmitterTexture,
}

/// <summary>One reported issue, with a short human detail.</summary>
public sealed record ModelIssue(ModelIssueKind Kind, string Detail);

/// <summary>
/// What a model holds and what is wrong with it. Counts are always present. <see cref="Issues"/>
/// is empty when the model is clean, and <see cref="Diagnostics"/> carries a parse failure so a
/// bad file never reads as a clean one.
/// </summary>
public sealed record ModelAuditResult(
    string Path,
    string Format,
    int Geosets,
    int Vertices,
    int Faces,
    int Textures,
    int Sequences,
    int Nodes,
    int ParticleEmitters,
    int ParticleEmitters2,
    int Ribbons,
    int Corns,
    long Bytes,
    IReadOnlyList<ModelIssue> Issues,
    IReadOnlyList<string> Diagnostics)
{
    public int IssueCount => Issues.Count;

    /// <summary>Every effect chunk the model carries, particle emitters plus ribbons plus corns.</summary>
    public int Effects => ParticleEmitters + ParticleEmitters2 + Ribbons + Corns;
}

/// <summary>
/// Audits a single MDX or MDL model, by path or by bytes, for the resource problems that waste
/// space or draw nothing, and reports what the model holds. Read-only, it parses in memory and
/// changes nothing, and it never throws on a bad file, reporting the parse failure instead.
///
/// The checks. A duplicate texture path in the list, which is dead weight the optimiser can drop.
/// A geoset with no texture, which the game draws as nothing with no message. A geoset with no
/// triangles. A degenerate triangle with no area. A skeleton node no geoset skins to, a leftover
/// bone or helper. A non-finite vertex coordinate, the tell for a corrupt or badly transformed
/// file. This is the analysis half of the optimiser, the half that says what can safely go.
///
/// It also counts the effects the model carries, particle emitters, particle emitters 2, ribbons
/// and popcorns, from the binary chunk tags alone, and reads each particle emitter's payload to
/// check the two references it makes. An emitter whose node objectId is not a node the model
/// declares, checked against the PIVT node list, and a particle emitter 2 whose texture id is not a
/// valid index into the model's texture list. Both are dangling references the game draws as
/// nothing. A model with no PIVT chunk has no node list to check against, so the node check is
/// skipped there rather than flagging every emitter.
/// </summary>
public static class ModelAuditCommand
{
    /// <summary>The Warcraft III world half extent in map units, reused from the map lint.</summary>
    public const float MaxCoordinate = AssetModelLintCommand.MaxCoordinate;

    public static ModelAuditResult ExecutePath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (!File.Exists(path)) throw new FileNotFoundException($"model not found: {path}", path);
        return Execute(File.ReadAllBytes(path), path);
    }

    public static ModelAuditResult Execute(byte[] bytes, string name)
    {
        ArgumentNullException.ThrowIfNull(bytes);

        Model3D model;
        try
        {
            model = ModelParser.Parse(bytes, name);
        }
        catch (Exception ex)
        {
            return new ModelAuditResult(name, "unknown", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, bytes.LongLength,
                Array.Empty<ModelIssue>(), new[] { $"could not parse '{name}': {ex.Message}" });
        }

        string format = bytes.Length >= 4 && bytes[0] == 'M' && bytes[1] == 'D'
            && bytes[2] == 'L' && bytes[3] == 'X' ? "mdx" : "mdl";

        // Effects are counted from the binary chunk list by tag. The emitter payloads are read only
        // to check each emitter's node and texture references. A text MDL carries no chunks, so the
        // document is null and both the counts and the emitter checks fall away.
        var document = MdxDocument.HasMagic(bytes) ? MdxDocument.Parse(bytes) : null;
        var effects = document?.CountEffects() ?? EffectCounts.None;

        int vertices = 0, faces = 0;
        var issues = new List<ModelIssue>();

        for (int g = 0; g < model.Geosets.Count; g++)
        {
            var geoset = model.Geosets[g];
            vertices += geoset.Vertices.Length / 3;
            faces += geoset.Indices.Length / 3;

            if (geoset.Indices.Length == 0)
                issues.Add(new ModelIssue(ModelIssueKind.EmptyGeoset, $"geoset {g} has no triangles"));
            if (geoset.TextureId < 0)
                issues.Add(new ModelIssue(ModelIssueKind.OrphanGeoset, $"geoset {g} has no texture"));

            if (geoset.Vertices.Any(v => !float.IsFinite(v)))
                issues.Add(new ModelIssue(ModelIssueKind.NonFiniteVertex, $"geoset {g} has a non-finite vertex"));

            int degenerate = CountDegenerate(geoset);
            if (degenerate > 0)
                issues.Add(new ModelIssue(ModelIssueKind.ZeroAreaFace,
                    $"geoset {g} has {degenerate} zero-area triangle(s)"));
        }

        foreach (var dup in model.Textures
                     .GroupBy(t => t, StringComparer.OrdinalIgnoreCase)
                     .Where(grp => grp.Count() > 1))
            issues.Add(new ModelIssue(ModelIssueKind.DuplicateTexture,
                $"texture '{dup.Key}' listed {dup.Count()} times"));

        if (model.Skeleton is { } skeleton)
        {
            var referenced = new HashSet<int>();
            foreach (var geoset in model.Geosets)
                if (geoset.SkinBones is { } bones)
                    foreach (var b in bones) if (b >= 0) referenced.Add(b);

            foreach (var node in skeleton.Nodes)
                if (!referenced.Contains(node.ObjectId))
                    issues.Add(new ModelIssue(ModelIssueKind.UnusedNode,
                        $"node '{node.Name}' ({node.ObjectId}) is not skinned to any geoset"));
        }

        if (document is not null)
        {
            // The PIVT node list is what a node's objectId indexes. With no PIVT there is no list to
            // check against, so the node check is skipped rather than flagging every emitter.
            int nodeCount = document.NodeCount();
            foreach (var emitter in MdxEmitterReader.Read(document))
            {
                if (nodeCount > 0 && (emitter.NodeId < 0 || emitter.NodeId >= nodeCount))
                    issues.Add(new ModelIssue(ModelIssueKind.DanglingEmitterNode,
                        $"{emitter.Tag} emitter in chunk {emitter.ChunkIndex} references node "
                        + $"{emitter.NodeId}, outside the model's {nodeCount} node(s)"));

                if (emitter.TextureId is { } textureId
                    && (textureId < 0 || textureId >= model.Textures.Count))
                    issues.Add(new ModelIssue(ModelIssueKind.DanglingEmitterTexture,
                        $"{emitter.Tag} emitter in chunk {emitter.ChunkIndex} references texture "
                        + $"{textureId}, outside the model's {model.Textures.Count} texture(s)"));
            }
        }

        return new ModelAuditResult(
            name, format,
            model.Geosets.Count, vertices, faces, model.Textures.Count,
            model.Skeleton?.Sequences.Count ?? 0, model.Skeleton?.Nodes.Count ?? 0,
            effects.ParticleEmitters, effects.ParticleEmitters2, effects.Ribbons, effects.Corns,
            bytes.LongLength, issues, Array.Empty<string>());
    }

    private static int CountDegenerate(Geoset geoset)
    {
        int count = 0;
        var v = geoset.Vertices;
        var idx = geoset.Indices;
        for (int i = 0; i + 2 < idx.Length; i += 3)
        {
            int a = idx[i] * 3, b = idx[i + 1] * 3, c = idx[i + 2] * 3;
            if (a + 2 >= v.Length || b + 2 >= v.Length || c + 2 >= v.Length) continue;
            var pa = new Vector3(v[a], v[a + 1], v[a + 2]);
            var pb = new Vector3(v[b], v[b + 1], v[b + 2]);
            var pc = new Vector3(v[c], v[c + 1], v[c + 2]);
            if (Vector3.Cross(pb - pa, pc - pa).LengthSquared() <= 1e-12f) count++;
        }
        return count;
    }
}
