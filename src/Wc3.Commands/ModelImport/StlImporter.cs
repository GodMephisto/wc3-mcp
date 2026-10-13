// src/Wc3.Commands/ModelImport/StlImporter.cs
using System.Buffers.Binary;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace Wc3.Commands;

/// <summary>
/// Parses an STL into a <see cref="MeshData"/>, both the binary and the ASCII form. A binary STL
/// carries a uint32 triangle count at offset 80, so its length is exactly 84 plus fifty bytes per
/// triangle. ASCII STL begins with "solid" and carries "facet normal" blocks. STL has no texture
/// coordinates, so no corner has a UV. Each triangle is independent, and its facet normal is used
/// when it is non-degenerate, otherwise the corner is left without one and the MDX builder computes
/// the face normal from the winding.
/// </summary>
internal static class StlImporter
{
    public static MeshData Parse(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        if (StlIsBinary(bytes))
            return ParseBinary(bytes);
        return ParseAscii(Encoding.UTF8.GetString(bytes));
    }

    /// <summary>True when the byte length matches a binary header plus fifty bytes per triangle.</summary>
    public static bool StlIsBinary(byte[] bytes)
    {
        if (bytes.Length < 84) return false;
        uint triangles = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(80, 4));
        return 84L + 50L * triangles == bytes.Length;
    }

    private static MeshData ParseBinary(byte[] bytes)
    {
        uint triangles = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(80, 4));
        var positions = new List<Vector3>((int)triangles * 3);
        var normals = new List<Vector3>((int)triangles);
        var faces = new List<MeshCorner[]>((int)triangles);

        int p = 84;
        for (int t = 0; t < triangles; t++)
        {
            var facetNormal = ReadVector(bytes, p); p += 12;
            int baseIndex = positions.Count;
            for (int v = 0; v < 3; v++)
            {
                positions.Add(ReadVector(bytes, p));
                p += 12;
            }
            p += 2; // attribute byte count, ignored
            int normalIndex = AddNormal(normals, facetNormal);
            faces.Add(new[]
            {
                new MeshCorner(baseIndex, normalIndex, -1),
                new MeshCorner(baseIndex + 1, normalIndex, -1),
                new MeshCorner(baseIndex + 2, normalIndex, -1),
            });
        }
        return new MeshData(positions, normals, Array.Empty<Vector2>(), faces, FlipV: false);
    }

    private static MeshData ParseAscii(string text)
    {
        var positions = new List<Vector3>();
        var normals = new List<Vector3>();
        var faces = new List<MeshCorner[]>();
        var triangle = new List<Vector3>(3);
        var facetNormal = Vector3.Zero;

        foreach (var raw in text.Split('\n'))
        {
            var line = raw.Trim();
            var parts = line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) continue;
            switch (parts[0])
            {
                case "facet" when parts.Length >= 5 && parts[1] == "normal":
                    facetNormal = new Vector3(F(parts[2]), F(parts[3]), F(parts[4]));
                    break;
                case "vertex" when parts.Length >= 4:
                    triangle.Add(new Vector3(F(parts[1]), F(parts[2]), F(parts[3])));
                    break;
                case "endfacet" when triangle.Count == 3:
                    int baseIndex = positions.Count;
                    positions.AddRange(triangle);
                    int normalIndex = AddNormal(normals, facetNormal);
                    faces.Add(new[]
                    {
                        new MeshCorner(baseIndex, normalIndex, -1),
                        new MeshCorner(baseIndex + 1, normalIndex, -1),
                        new MeshCorner(baseIndex + 2, normalIndex, -1),
                    });
                    triangle.Clear();
                    break;
                case "endfacet":
                    triangle.Clear();
                    break;
            }
        }

        if (positions.Count == 0 || faces.Count == 0)
            throw new ArgumentException("ASCII STL holds no facets");
        return new MeshData(positions, normals, Array.Empty<Vector2>(), faces, FlipV: false);
    }

    /// <summary>Adds a facet normal when it is usable, returning its index, or -1 for a degenerate one.</summary>
    private static int AddNormal(List<Vector3> normals, Vector3 candidate)
    {
        if (candidate.LengthSquared() <= 1e-20f) return -1;
        normals.Add(candidate);
        return normals.Count - 1;
    }

    private static Vector3 ReadVector(byte[] b, int offset) => new(
        BinaryPrimitives.ReadSingleLittleEndian(b.AsSpan(offset, 4)),
        BinaryPrimitives.ReadSingleLittleEndian(b.AsSpan(offset + 4, 4)),
        BinaryPrimitives.ReadSingleLittleEndian(b.AsSpan(offset + 8, 4)));

    private static float F(string s) =>
        float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out float v) ? v : 0f;
}
