// src/Wc3.Modeling/QemDecimator.cs
using System.Numerics;

namespace Wc3.Modeling;

/// <summary>
/// The reduced geometry plus a map from every input vertex to its output vertex, or -1 when the
/// vertex was merged away. The caller remaps its per-vertex arrays (normals, UVs, matrix groups)
/// through <see cref="Remap"/>.
/// </summary>
public sealed record DecimateResult(float[] Vertices, int[] Indices, int[] Remap)
{
    public int InputVertices => Remap.Length;
    public int OutputVertices => Vertices.Length / 3;
    public int OutputFaces => Indices.Length / 3;
}

/// <summary>
/// Mesh decimation by greedy edge collapse under a quadric error metric, the standard from Garland
/// and Heckbert, following the implementation guide in arXiv 2512.19959. Each vertex accumulates a
/// quadric from the planes of the faces that touch it. An edge's cost is the quadric error at its
/// midpoint, and the cheapest edges are collapsed first until the target face count is reached.
///
/// Deterministic. Vertices are welded by exact position, edges are ordered by cost then by index,
/// and there is no random tie-breaking, so the same input always yields the same output.
///
/// Non-manifold and multi-component input, the Warcraft III case, is handled because the weld is by
/// position alone and the collapse is a union-find over welded vertices, not a half-edge walk.
/// </summary>
public static class QemDecimator
{
    public static DecimateResult Decimate(float[] vertices, int[] indices, float ratio)
    {
        ArgumentNullException.ThrowIfNull(vertices);
        ArgumentNullException.ThrowIfNull(indices);
        if (ratio >= 1f || vertices.Length < 9 || indices.Length < 3)
            return Identity(vertices, indices);

        int faceCount = indices.Length / 3;
        int targetFaces = Math.Max(1, (int)MathF.Round(faceCount * Math.Clamp(ratio, 0.01f, 1f)));

        // 1. Weld by exact position so shared corners share a quadric.
        var weld = new Dictionary<(float, float, float), int>();
        var canonical = new int[vertices.Length / 3];
        var positions = new List<Vector3>();
        for (int i = 0; i < canonical.Length; i++)
        {
            var key = (vertices[i * 3], vertices[i * 3 + 1], vertices[i * 3 + 2]);
            if (!weld.TryGetValue(key, out int c))
            {
                c = positions.Count;
                weld[key] = c;
                positions.Add(new Vector3(key.Item1, key.Item2, key.Item3));
            }
            canonical[i] = c;
        }

        // 2. Triangles on welded vertices, dropping any that are already degenerate.
        var tris = new List<(int A, int B, int C)>();
        for (int t = 0; t + 2 < indices.Length; t += 3)
        {
            int a = canonical[indices[t]], b = canonical[indices[t + 1]], c = canonical[indices[t + 2]];
            if (a != b && b != c && a != c) tris.Add((a, b, c));
        }

        // 3. One quadric per welded vertex, accumulated from incident face planes.
        int vn = positions.Count;
        var quad = new float[vn][];
        for (int i = 0; i < vn; i++) quad[i] = new float[10];
        foreach (var (a, b, c) in tris)
            AccumulatePlane(quad[a], quad[b], quad[c], positions[a], positions[b], positions[c]);

        // 4. Unique edges and their cost at the midpoint.
        var edgeSet = new HashSet<(int, int)>();
        var edges = new List<(float Cost, int A, int B)>();
        foreach (var (a, b, c) in tris)
            foreach (var (u, v) in new[] { (a, b), (b, c), (a, c) })
            {
                int lo = Math.Min(u, v), hi = Math.Max(u, v);
                if (!edgeSet.Add((lo, hi))) continue;
                var mid = (positions[lo] + positions[hi]) * 0.5f;
                edges.Add((Cost(quad[lo], quad[hi], mid), lo, hi));
            }
        edges.Sort((x, y) => x.Cost != y.Cost ? x.Cost.CompareTo(y.Cost) : (x.A != y.A ? x.A.CompareTo(y.A) : x.B.CompareTo(y.B)));

        // 5. Greedy collapse with union-find, the surviving vertex moving to the midpoint.
        var parent = Enumerable.Range(0, vn).ToArray();
        int Find(int x) { while (parent[x] != x) { parent[x] = parent[parent[x]]; x = parent[x]; } return x; }
        var live = new bool[vn];
        Array.Fill(live, true);

        int remaining = tris.Count;
        foreach (var (_, a, b) in edges)
        {
            if (remaining <= targetFaces) break;
            int ra = Find(a), rb = Find(b);
            if (ra == rb) continue;
            int keep = Math.Min(ra, rb), drop = Math.Max(ra, rb);
            parent[drop] = keep;
            positions[keep] = (positions[keep] + positions[drop]) * 0.5f;
            for (int q = 0; q < 10; q++) quad[keep][q] += quad[drop][q];
            live[drop] = false;
            remaining = CountLive(tris, parent);
        }

        // 6. Rebuild, dropping triangles whose corners merged together.
        var outTris = new List<(int A, int B, int C)>();
        foreach (var (a, b, c) in tris)
        {
            int ra = Find(a), rb = Find(b), rc = Find(c);
            if (ra != rb && rb != rc && ra != rc) outTris.Add((ra, rb, rc));
        }

        // 7. Compact surviving vertices and build the remap.
        var newIndex = new int[vn];
        Array.Fill(newIndex, -1);
        var outPositions = new List<float>();
        foreach (var (a, b, c) in outTris)
            foreach (var r in new[] { a, b, c })
                if (newIndex[r] < 0)
                {
                    newIndex[r] = outPositions.Count / 3;
                    outPositions.Add(positions[r].X);
                    outPositions.Add(positions[r].Y);
                    outPositions.Add(positions[r].Z);
                }
        var outIndices = new List<int>(outTris.Count * 3);
        foreach (var (a, b, c) in outTris) { outIndices.Add(newIndex[a]); outIndices.Add(newIndex[b]); outIndices.Add(newIndex[c]); }

        var remap = new int[canonical.Length];
        for (int i = 0; i < canonical.Length; i++)
            remap[i] = newIndex[Find(canonical[i])];

        return new DecimateResult(outPositions.ToArray(), outIndices.ToArray(), remap);
    }

    private static DecimateResult Identity(float[] vertices, int[] indices)
    {
        var remap = new int[vertices.Length / 3];
        for (int i = 0; i < remap.Length; i++) remap[i] = i;
        return new DecimateResult((float[])vertices.Clone(), (int[])indices.Clone(), remap);
    }

    private static int CountLive(List<(int A, int B, int C)> tris, int[] parent)
    {
        int Find(int x) { while (parent[x] != x) x = parent[x]; return x; }
        int n = 0;
        foreach (var (a, b, c) in tris)
        {
            int ra = Find(a), rb = Find(b), rc = Find(c);
            if (ra != rb && rb != rc && ra != rc) n++;
        }
        return n;
    }

    private static void AccumulatePlane(float[] qa, float[] qb, float[] qc, Vector3 a, Vector3 b, Vector3 c)
    {
        var n = Vector3.Cross(b - a, c - a);
        float len = n.Length();
        if (len <= 1e-12f) return; // degenerate face, no plane
        n /= len;
        float d = -Vector3.Dot(n, a);
        float[] p = { n.X, n.Y, n.Z, d };
        for (int i = 0; i < 4; i++)
            for (int j = i; j < 4; j++)
            {
                float term = p[i] * p[j];
                qa[Idx(i, j)] += term; qb[Idx(i, j)] += term; qc[Idx(i, j)] += term;
            }
    }

    private static float Cost(float[] qa, float[] qb, Vector3 v)
    {
        float[] q = new float[10];
        for (int i = 0; i < 10; i++) q[i] = qa[i] + qb[i];
        float x = v.X, y = v.Y, z = v.Z;
        return q[0] * x * x + 2 * q[1] * x * y + 2 * q[2] * x * z + 2 * q[3] * x
             + q[4] * y * y + 2 * q[5] * y * z + 2 * q[6] * y
             + q[7] * z * z + 2 * q[8] * z + q[9];
    }

    private static int Idx(int i, int j) => i * 4 + j - (i * (i + 1)) / 2;
}
