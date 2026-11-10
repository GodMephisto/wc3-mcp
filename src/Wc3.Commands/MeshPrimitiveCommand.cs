// src/Wc3.Commands/MeshPrimitiveCommand.cs
using System.Numerics;

namespace Wc3.Commands;

/// <summary>The primitive shapes <see cref="MeshPrimitiveCommand"/> can generate.</summary>
public enum MeshPrimitiveKind
{
    /// <summary>A single quad in the XY plane, centred on the origin and facing +Z.</summary>
    Card,
    /// <summary>A flat annulus in the XY plane, an inner hole inside an outer radius.</summary>
    Ring,
    /// <summary>A cone along Z, its base disc at the bottom and its apex at the top.</summary>
    Cone,
    /// <summary>A cylinder along Z closed by a hemisphere at each end.</summary>
    Capsule,
}

/// <summary>
/// What a primitive generation wrote. <see cref="Vertices"/> is the deduplicated vertex count the
/// geoset carries, which is what <see cref="Faces"/> triangles index, and <see cref="Bytes"/> is the
/// MDX length. <see cref="Diagnostics"/> is the builder's notes, empty for a well formed primitive.
/// </summary>
public sealed record MeshPrimitiveResult(
    string Path,
    MeshPrimitiveKind Kind,
    int Vertices,
    int Faces,
    long Bytes,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Generates a primitive mesh and writes a real v800 MDX for it, through the same
/// <see cref="MdxMeshBuilder"/> every format importer feeds, so a generated card, ring, cone or
/// capsule is loadable by the game and not a stub.
///
/// Every primitive is centred on the origin, is built in the XY plane or along Z, and carries a
/// per-vertex normal plus a UV in the WC3 top-down sense, so <see cref="MeshData.FlipV"/> stays
/// false and the texture is not mirrored. A curved primitive shares its ring vertices where the UV
/// allows and duplicates only the seam, so a texture wraps once around the shape. <c>size</c> is the
/// card's half-extent and the outer radius of the ring, cone and capsule. <c>length</c> is the cone
/// height and the capsule's cylinder length, defaulting to <c>size</c>, so an unset length gives a
/// cone as tall as it is wide. <c>innerRadius</c> is the ring's hole and defaults to half of
/// <c>size</c>. <c>texturePath</c> is the map path (for example
/// <c>Textures\white.blp</c>) written into TEXS, and no texture bytes are produced, so the path must
/// already exist in the map. A curved primitive whose segment count is below
/// <see cref="MinSegments"/> is refused rather than written as a collapsed sliver.
/// </summary>
public static class MeshPrimitiveCommand
{
    /// <summary>The fewest radial segments a curved primitive accepts. Two would fold the surface
    /// onto itself and three is the first count that encloses an area.</summary>
    public const int MinSegments = 3;

    /// <summary>
    /// Generates <paramref name="kind"/> at <paramref name="size"/> and writes it to
    /// <paramref name="outPath"/>, creating the folder when it does not exist. Throws
    /// <see cref="ArgumentOutOfRangeException"/> when a size is not positive and finite, when a
    /// curved kind gets fewer than <see cref="MinSegments"/> segments, or when the ring hole is not
    /// inside its outer radius.
    /// </summary>
    public static MeshPrimitiveResult Execute(
        MeshPrimitiveKind kind,
        string outPath,
        float size = 32f,
        float length = 0f,
        float innerRadius = 0f,
        int segments = 16,
        string? texturePath = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outPath);
        if (!float.IsFinite(size) || size <= 0f)
            throw new ArgumentOutOfRangeException(nameof(size), size, "size must be a positive, finite number");

        bool curved = kind is MeshPrimitiveKind.Ring or MeshPrimitiveKind.Cone or MeshPrimitiveKind.Capsule;
        if (curved && segments < MinSegments)
            throw new ArgumentOutOfRangeException(nameof(segments), segments,
                $"{kind} needs at least {MinSegments} segments, a lower count collapses it to a sliver");

        float usedLength = length;
        if (kind is MeshPrimitiveKind.Cone or MeshPrimitiveKind.Capsule)
        {
            if (usedLength <= 0f) usedLength = size;
            if (!float.IsFinite(usedLength))
                throw new ArgumentOutOfRangeException(nameof(length), length,
                    "length must be a positive, finite number, or 0 to use size");
        }

        float usedInner = innerRadius;
        if (kind is MeshPrimitiveKind.Ring)
        {
            if (usedInner <= 0f) usedInner = size * 0.5f;
            if (!float.IsFinite(usedInner) || usedInner >= size)
                throw new ArgumentOutOfRangeException(nameof(innerRadius), innerRadius,
                    "the ring hole must be positive and smaller than size");
        }

        MeshData mesh = kind switch
        {
            MeshPrimitiveKind.Card => Card(size),
            MeshPrimitiveKind.Ring => Ring(size, usedInner, segments),
            MeshPrimitiveKind.Cone => Cone(size, usedLength, segments),
            MeshPrimitiveKind.Capsule => Capsule(size, usedLength, segments),
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "unknown primitive kind"),
        };

        // PVTX indexes with a u16, so a primitive that would overflow it is refused before the
        // builder silently truncates an index.
        if (mesh.Positions.Count >= ushort.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(segments), segments,
                "the mesh exceeds 65534 vertices, lower the segment count");

        string name = Path.GetFileNameWithoutExtension(outPath);
        var diagnostics = new List<string>();
        var built = MdxMeshBuilder.Build(
            new[] { mesh }, name, texturePath, Array.Empty<MeshSequence>(), diagnostics,
            out byte[] mdx, out _);

        string? dir = Path.GetDirectoryName(Path.GetFullPath(outPath));
        if (dir is { Length: > 0 }) Directory.CreateDirectory(dir);
        File.WriteAllBytes(outPath, mdx);

        return new MeshPrimitiveResult(
            outPath, kind, built.Vertices, built.Faces, mdx.LongLength, diagnostics);
    }

    /// <summary>A quad of side <c>2 * size</c> facing +Z, wound counter-clockwise seen from the
    /// front so the face normal and the game's culling agree.</summary>
    private static MeshData Card(float size)
    {
        var mesh = new PrimitiveMesh();
        int v0 = mesh.Add(new Vector3(-size, -size, 0f), Vector3.UnitZ, new Vector2(0f, 1f));
        int v1 = mesh.Add(new Vector3(size, -size, 0f), Vector3.UnitZ, new Vector2(1f, 1f));
        int v2 = mesh.Add(new Vector3(size, size, 0f), Vector3.UnitZ, new Vector2(1f, 0f));
        int v3 = mesh.Add(new Vector3(-size, size, 0f), Vector3.UnitZ, new Vector2(0f, 0f));
        mesh.Quad(v0, v1, v2, v3);
        return mesh.Build();
    }

    /// <summary>A flat annulus of <paramref name="segments"/> quads around the Z axis. The planar UV
    /// fits the outer disc into the 0..1 square, so no seam duplicate is needed and the ring stays at
    /// twice the segment count in vertices.</summary>
    private static MeshData Ring(float outer, float inner, int segments)
    {
        var mesh = new PrimitiveMesh();
        var innerIds = new int[segments];
        var outerIds = new int[segments];

        for (int i = 0; i < segments; i++)
        {
            float angle = MathF.Tau * i / segments;
            float cos = MathF.Cos(angle), sin = MathF.Sin(angle);
            var innerPosition = new Vector3(inner * cos, inner * sin, 0f);
            var outerPosition = new Vector3(outer * cos, outer * sin, 0f);
            innerIds[i] = mesh.Add(innerPosition, Vector3.UnitZ, PlanarUv(innerPosition, outer));
            outerIds[i] = mesh.Add(outerPosition, Vector3.UnitZ, PlanarUv(outerPosition, outer));
        }

        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;
            mesh.Quad(innerIds[i], outerIds[i], outerIds[next], innerIds[next]);
        }
        return mesh.Build();
    }

    /// <summary>
    /// A cone of base <paramref name="radius"/> and <paramref name="height"/>, its base disc at
    /// -height/2 and its apex at +height/2. The side normal slopes by the same ratio as the surface,
    /// <c>(height * cos, height * sin, radius)</c>, so shading follows the cone rather than the
    /// cylinder its base ring resembles. The side unwraps once around with the apex duplicated per
    /// segment so each triangle gets a sane apex UV, and the base disc is a fan facing -Z.
    /// </summary>
    private static MeshData Cone(float radius, float height, int segments)
    {
        var mesh = new PrimitiveMesh();
        float half = height * 0.5f;

        // Side ring, v = 1 at the base and v = 0 at the apex, so the texture runs bottom to top.
        int[] sideRing = mesh.AddRing(segments, radius, -half, normalH: height, normalZ: radius, v: 1f);
        for (int i = 0; i < segments; i++)
        {
            float mid = MathF.Tau * (i + 0.5f) / segments;
            var normal = new Vector3(height * MathF.Cos(mid), height * MathF.Sin(mid), radius);
            int apex = mesh.Add(new Vector3(0f, 0f, half), normal, new Vector2((i + 0.5f) / segments, 0f));
            mesh.Tri(sideRing[i], sideRing[i + 1], apex);
        }

        // Base disc facing -Z, a fan from the centre wound the opposite way to the +Z ring.
        int centre = mesh.Add(new Vector3(0f, 0f, -half), -Vector3.UnitZ, new Vector2(0.5f, 0.5f));
        var cap = new int[segments];
        for (int i = 0; i < segments; i++)
        {
            float angle = MathF.Tau * i / segments;
            var position = new Vector3(radius * MathF.Cos(angle), radius * MathF.Sin(angle), -half);
            cap[i] = mesh.Add(position, -Vector3.UnitZ, PlanarUv(position, radius));
        }
        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;
            mesh.Tri(centre, cap[next], cap[i]);
        }

        return mesh.Build();
    }

    /// <summary>
    /// A capsule of <paramref name="radius"/> whose cylinder runs <paramref name="length"/> along Z,
    /// closed at each end by a hemisphere, so the total height is length + 2 * radius and it spans
    /// that height from -half to +half. The profile is a stack of rings from the bottom pole to the
    /// top pole, the cylinder ring included, joined by quads with the normal of the surface the ring
    /// sits on. V is the fraction of the profile arc travelled, so the unwrap is continuous over the
    /// caps and the shaft rather than restarting at the equator.
    /// </summary>
    private static MeshData Capsule(float radius, float length, int segments)
    {
        var mesh = new PrimitiveMesh();
        float half = length * 0.5f;
        int capRings = Math.Max(2, segments / 2);
        float quarter = MathF.PI * 0.5f;
        float totalArc = length + MathF.PI * radius;

        int bottomPole = mesh.Add(
            new Vector3(0f, 0f, -half - radius), -Vector3.UnitZ, new Vector2(0.5f, 0f));

        var rings = new List<int[]>(capRings * 2);
        // Bottom hemisphere, from the ring nearest the pole down to the equator at -half.
        for (int k = capRings - 1; k >= 0; k--)
        {
            float phi = k * quarter / capRings;
            float arc = radius * (quarter - phi);
            rings.Add(mesh.AddRing(
                segments, radius * MathF.Cos(phi), -half - radius * MathF.Sin(phi),
                normalH: MathF.Cos(phi), normalZ: -MathF.Sin(phi), v: arc / totalArc));
        }

        // The top equator, which is the cylinder's far rim, then the top hemisphere rings.
        rings.Add(mesh.AddRing(
            segments, radius, half, normalH: 1f, normalZ: 0f,
            v: (radius * quarter + length) / totalArc));
        for (int k = 1; k < capRings; k++)
        {
            float phi = k * quarter / capRings;
            float arc = radius * quarter + length + radius * phi;
            rings.Add(mesh.AddRing(
                segments, radius * MathF.Cos(phi), half + radius * MathF.Sin(phi),
                normalH: MathF.Cos(phi), normalZ: MathF.Sin(phi), v: arc / totalArc));
        }

        int topPole = mesh.Add(
            new Vector3(0f, 0f, half + radius), Vector3.UnitZ, new Vector2(0.5f, 1f));

        int[] first = rings[0];
        for (int i = 0; i < segments; i++) mesh.Tri(bottomPole, first[i + 1], first[i]);

        for (int r = 0; r + 1 < rings.Count; r++)
        {
            int[] lower = rings[r];
            int[] upper = rings[r + 1];
            for (int i = 0; i < segments; i++)
                mesh.Quad(lower[i], lower[i + 1], upper[i + 1], upper[i]);
        }

        int[] last = rings[^1];
        for (int i = 0; i < segments; i++) mesh.Tri(last[i], last[i + 1], topPole);

        return mesh.Build();
    }

    /// <summary>The planar UV of a point on a disc of <paramref name="extent"/> radius, fitted into
    /// the 0..1 square with V measured downward as WC3 expects.</summary>
    private static Vector2 PlanarUv(Vector3 position, float extent) =>
        new(0.5f + position.X / (2f * extent), 0.5f - position.Y / (2f * extent));

    /// <summary>
    /// Position, normal and UV arrays kept parallel, so one vertex index addresses all three and a
    /// <see cref="MeshCorner"/> is the same index three times. A seam vertex is a separate entry
    /// with the same position and a different U, which the MDX builder keeps apart because it keys
    /// on the position index.
    /// </summary>
    private sealed class PrimitiveMesh
    {
        private readonly List<Vector3> _positions = new();
        private readonly List<Vector3> _normals = new();
        private readonly List<Vector2> _uvs = new();
        private readonly List<MeshCorner[]> _faces = new();

        public int Add(Vector3 position, Vector3 normal, Vector2 uv)
        {
            _positions.Add(position);
            _normals.Add(normal.LengthSquared() > 1e-20f ? Vector3.Normalize(normal) : Vector3.UnitZ);
            _uvs.Add(uv);
            return _positions.Count - 1;
        }

        /// <summary>One ring of <paramref name="segments"/> points plus a seam duplicate at u = 1.
        /// <paramref name="normalH"/> and <paramref name="normalZ"/> scale the horizontal and
        /// vertical normal components, so the caller expresses the surface slope and the normal is
        /// normalized on add. <paramref name="v"/> is the ring's V coordinate.</summary>
        public int[] AddRing(int segments, float radius, float z, float normalH, float normalZ, float v)
        {
            var ring = new int[segments + 1];
            for (int i = 0; i <= segments; i++)
            {
                float angle = MathF.Tau * i / segments;
                float cos = MathF.Cos(angle), sin = MathF.Sin(angle);
                var position = new Vector3(radius * cos, radius * sin, z);
                var normal = new Vector3(normalH * cos, normalH * sin, normalZ);
                ring[i] = Add(position, normal, new Vector2((float)i / segments, v));
            }
            return ring;
        }

        public void Tri(int a, int b, int c) => _faces.Add(Corners(a, b, c));

        public void Quad(int a, int b, int c, int d) => _faces.Add(Corners(a, b, c, d));

        public MeshData Build() => new(
            _positions, _normals, _uvs, _faces, FlipV: false);

        private static MeshCorner[] Corners(params int[] indices)
        {
            var corners = new MeshCorner[indices.Length];
            for (int i = 0; i < indices.Length; i++)
                corners[i] = new MeshCorner(indices[i], indices[i], indices[i]);
            return corners;
        }
    }
}
