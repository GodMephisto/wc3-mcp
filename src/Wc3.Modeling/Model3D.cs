// src/Wc3.Modeling/Model3D.cs
namespace Wc3.Modeling;

/// <summary>
/// Renderer-neutral model: geometry plus the texture paths it references, and —
/// when the source file carried parseable animation data — a skeleton
/// (sequences + bone hierarchy) that <see cref="PosedAt"/> can apply.
/// </summary>
public sealed record Model3D(
    IReadOnlyList<Geoset> Geosets,
    IReadOnlyList<string> Textures,
    ModelSkeleton? Skeleton = null)
{
    /// <summary>
    /// Returns a copy of the model with vertices/normals transformed to the first
    /// frame of the named sequence ("Stand" by default: exact match preferred, then
    /// any Stand* variant, then the model's first sequence). Returns <c>this</c>
    /// unchanged when there is no skeleton, no sequences, or no skinnable geometry —
    /// the bind pose is always the safe fallback and this method never throws.
    /// The returned model has no skeleton (its geometry is already posed).
    /// </summary>
    public Model3D PosedAt(string? sequenceName = "Stand")
    {
        try
        {
            return ModelPoser.Pose(this, sequenceName);
        }
        catch
        {
            return this; // any pose-evaluation surprise degrades to bind pose
        }
    }
}

/// <summary>
/// One drawable mesh chunk. <paramref name="Vertices"/> and <paramref name="Normals"/>
/// are packed x,y,z triples; <paramref name="Uvs"/> packed u,v pairs; <paramref name="Indices"/>
/// triangle-list indices into the vertex arrays. <paramref name="TextureId"/> indexes
/// <see cref="Model3D.Textures"/>, or -1 when the geoset has no resolvable texture.
/// <paramref name="SkinBones"/>/<paramref name="SkinWeights"/> are optional skinning
/// data: 4 slots per vertex, bone node ObjectIds with normalized weights (unused
/// slots have weight 0). Null when the geoset carries no usable attachment data.
/// <paramref name="FilterMode"/> is the blend mode of the geoset's material (its
/// base layer); <see cref="FilterMode.None"/> when the geoset has no material.
/// </summary>
public sealed record Geoset(
    float[] Vertices,
    float[] Normals,
    float[] Uvs,
    int[] Indices,
    int TextureId,
    int[]? SkinBones = null,
    float[]? SkinWeights = null,
    FilterMode FilterMode = FilterMode.None);

/// <summary>
/// WC3 material-layer blend mode, as stored in MDX layer records (u32 0-6) and MDL
/// <c>FilterMode</c> keywords. <see cref="None"/> renders opaque (a texture's alpha
/// channel is utility data — team colour, reflections — not transparency);
/// <see cref="Transparent"/> is an alpha-tested cutout; the rest blend against the
/// frame buffer (<see cref="Additive"/> glows being the common case).
/// </summary>
public enum FilterMode
{
    None = 0,
    Transparent = 1,
    Blend = 2,
    Additive = 3,
    AddAlpha = 4,
    Modulate = 5,
    Modulate2x = 6,
}
