// src/Wc3.Commands/ModelImport/MeshData.cs
using System.Numerics;

namespace Wc3.Commands;

/// <summary>
/// One face corner of a <see cref="MeshData"/>, indexing the mesh's position, normal and UV
/// arrays. A negative normal or UV index means that attribute is absent for the corner, so the
/// shared builder computes a per-face normal or zeroes the UV. This is the one vertex shape every
/// format importer produces, so OBJ, glTF, STL and PLY all feed the same MDX path.
/// </summary>
internal readonly record struct MeshCorner(int Position, int Normal, int Uv);

/// <summary>
/// One texture an importer resolved from the source. <see cref="Name"/> has no extension and names
/// the file the import publishes, <see cref="Bytes"/> are the source image bytes and
/// <see cref="Extension"/> is the source format so the converter picks the right decoder. Two
/// primitives that share a source image share one instance, so the file is written once.
/// </summary>
internal sealed record MeshTexture(string Name, byte[] Bytes, string Extension);

/// <summary>
/// The material a mesh primitive uses. A null <see cref="Texture"/> means the source named a
/// material but its image could not be read, so the material is kept with no texture and the
/// geoset's layer references none.
/// </summary>
internal sealed record MeshMaterial(MeshTexture? Texture);

/// <summary>
/// One animation a source carried, its time range in seconds. glTF is the only importer that fills
/// this today, reading each animation's sampler input accessors so the MDX builder can turn the
/// range into a sequence frame interval at the model's fps. The keyframe tracks are deliberately
/// not carried, only the named interval.
/// </summary>
internal sealed record MeshAnimation(string Name, float StartSeconds, float EndSeconds);

/// <summary>
/// A named MDX sequence after the frame conversion, one SEQS entry. <see cref="StartFrame"/> and
/// <see cref="EndFrame"/> are integer frames on the model's global timeline, derived from an
/// animation's time range at the chosen fps.
/// </summary>
internal sealed record MeshSequence(string Name, int StartFrame, int EndFrame);

/// <summary>
/// A format-neutral mesh, the shape every importer returns and
/// <see cref="MdxMeshBuilder.Build"/> consumes. <see cref="Positions"/> is required.
/// <see cref="Normals"/> and <see cref="Uvs"/> may be empty when the source format carries none,
/// and a corner referencing a missing attribute (index -1) gets a computed normal or a zero UV.
/// <see cref="Faces"/> are vertex-index polygons in winding order, split into a triangle fan when
/// longer than a triangle. <see cref="FlipV"/> is true only for formats whose V axis runs bottom-up
/// like OBJ, so glTF (top-left origin) and STL (no UVs) leave V untouched. <see cref="Material"/> is
/// the primitive's resolved material, or null when the source carried none, which keeps the legacy
/// single default texture.
/// </summary>
internal sealed record MeshData(
    IReadOnlyList<Vector3> Positions,
    IReadOnlyList<Vector3> Normals,
    IReadOnlyList<Vector2> Uvs,
    IReadOnlyList<MeshCorner[]> Faces,
    bool FlipV,
    MeshMaterial? Material = null);
