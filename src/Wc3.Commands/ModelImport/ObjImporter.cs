// src/Wc3.Commands/ModelImport/ObjImporter.cs
using System.Globalization;
using System.Numerics;

namespace Wc3.Commands;

/// <summary>
/// Parses a Wavefront OBJ into a <see cref="MeshData"/>. Reads position, texture and normal
/// declarations and the face lines in their three forms (v, v/vt, v/vt/vn, and v//vn). OBJ indices
/// are 1-based and a negative index counts back from the current end. OBJ V runs bottom-up, so
/// <see cref="MeshData.FlipV"/> is true and the MDX builder flips it back to WC3's top-down sense,
/// which makes an ObjExporter export followed by an import round-trip the geometry.
/// </summary>
internal static class ObjImporter
{
    public static MeshData Parse(string text, string name)
    {
        ArgumentNullException.ThrowIfNull(text);

        var positions = new List<Vector3>();
        var texcoords = new List<Vector2>();
        var normals = new List<Vector3>();
        var faces = new List<MeshCorner[]>();

        foreach (var raw in text.Split('\n'))
        {
            var line = raw.Trim();
            if (line.Length == 0 || line[0] == '#') continue;
            int sp = line.IndexOf(' ');
            if (sp < 0) continue;
            string keyword = line[..sp];
            var parts = line[(sp + 1)..].Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            switch (keyword)
            {
                case "v" when parts.Length >= 3:
                    positions.Add(new Vector3(F(parts[0]), F(parts[1]), F(parts[2])));
                    break;
                case "vt" when parts.Length >= 2:
                    texcoords.Add(new Vector2(F(parts[0]), F(parts[1])));
                    break;
                case "vn" when parts.Length >= 3:
                    normals.Add(new Vector3(F(parts[0]), F(parts[1]), F(parts[2])));
                    break;
                case "f" when parts.Length >= 3:
                    faces.Add(ParseFace(parts, positions.Count, texcoords.Count, normals.Count));
                    break;
            }
        }

        if (positions.Count == 0 || faces.Count == 0)
            throw new ArgumentException($"OBJ '{name}' holds no drawable geometry (no v/f lines)");

        return new MeshData(positions, normals, texcoords, faces, FlipV: true);
    }

    private static MeshCorner[] ParseFace(string[] tokens, int vertexCount, int texCount, int normalCount)
    {
        var corners = new MeshCorner[tokens.Length];
        for (int i = 0; i < tokens.Length; i++)
        {
            var fields = tokens[i].Split('/');
            int v = Resolve(fields[0], vertexCount, "vertex");
            int vt = fields.Length >= 2 && fields[1].Length > 0 ? Resolve(fields[1], texCount, "texture") : -1;
            int vn = fields.Length >= 3 && fields[2].Length > 0 ? Resolve(fields[2], normalCount, "normal") : -1;
            corners[i] = new MeshCorner(v, vn, vt);
        }
        return corners;
    }

    private static int Resolve(string field, int count, string kind)
    {
        if (!int.TryParse(field, NumberStyles.Integer, CultureInfo.InvariantCulture, out int index))
            throw new ArgumentException($"OBJ face has a non-numeric {kind} index '{field}'");
        int resolved = index > 0 ? index - 1 : count + index;
        if (resolved < 0 || resolved >= count)
            throw new ArgumentException($"OBJ face {kind} index {index} is out of range (have {count})");
        return resolved;
    }

    private static float F(string s) =>
        float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out float v) ? v : 0f;
}
