// src/Wc3.Commands/ModelImport/PlyImporter.cs
using System.Buffers.Binary;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace Wc3.Commands;

/// <summary>
/// Parses a PLY (Stanford polygon) file into a <see cref="MeshData"/>, ASCII and binary
/// little-endian. The header declares each element, its record count and its properties. The vertex
/// element's x, y, z, nx, ny, nz and s, t are read when present, so a file with no normals gives
/// corners a missing normal and the MDX builder computes a face normal. A face element's index list
/// is read as its polygons, in winding order. Binary big-endian is refused, since no tool here
/// writes it and the reader would otherwise have to carry a second byte order.
/// </summary>
internal static class PlyImporter
{
    private sealed class Element
    {
        public string Name = "";
        public int Count;
        public readonly List<Prop> Props = new();
    }

    private sealed class Prop
    {
        public string Name = "";
        public string Type = "";
        public bool IsList;
        public string CountType = "";
        public string ValueType = "";
    }

    public static MeshData Parse(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        var (elements, ascii, little, dataOffset) = ParseHeader(bytes);
        if (!little)
            throw new NotSupportedException("PLY binary_big_endian is not supported");

        int vertexCount = elements.FirstOrDefault(e => e.Name == "vertex")?.Count ?? 0;
        var scalar = new Dictionary<string, List<float>>(StringComparer.Ordinal);
        var faces = new List<int[]>();

        if (ascii) ReadAscii(bytes, dataOffset, elements, scalar, faces);
        else ReadBinary(bytes, dataOffset, elements, scalar, faces);

        bool hasNormals = HasColumn(scalar, "vertex.nx", vertexCount) && HasColumn(scalar, "vertex.ny", vertexCount)
            && HasColumn(scalar, "vertex.nz", vertexCount);
        bool hasUvs = HasColumn(scalar, "vertex.s", vertexCount) && HasColumn(scalar, "vertex.t", vertexCount);

        var positions = new List<Vector3>(vertexCount);
        var normals = hasNormals ? new List<Vector3>(vertexCount) : new List<Vector3>();
        var uvs = hasUvs ? new List<Vector2>(vertexCount) : new List<Vector2>();
        for (int i = 0; i < vertexCount; i++)
        {
            positions.Add(new Vector3(scalar["vertex.x"][i], scalar["vertex.y"][i], scalar["vertex.z"][i]));
            if (hasNormals) normals.Add(new Vector3(scalar["vertex.nx"][i], scalar["vertex.ny"][i], scalar["vertex.nz"][i]));
            if (hasUvs) uvs.Add(new Vector2(scalar["vertex.s"][i], scalar["vertex.t"][i]));
        }

        var meshFaces = new List<MeshCorner[]>(faces.Count);
        foreach (var index in faces)
        {
            if (index.Length < 3) continue;
            var corners = new MeshCorner[index.Length];
            for (int k = 0; k < index.Length; k++)
            {
                int vi = index[k];
                if (vi < 0 || vi >= vertexCount)
                    throw new InvalidDataException($"PLY face index {vi} is out of range (have {vertexCount})");
                corners[k] = new MeshCorner(vi, hasNormals ? vi : -1, hasUvs ? vi : -1);
            }
            meshFaces.Add(corners);
        }

        if (positions.Count == 0 || meshFaces.Count == 0)
            throw new ArgumentException("PLY holds no drawable geometry");

        return new MeshData(positions, normals, uvs, meshFaces, FlipV: false);
    }

    private static bool HasColumn(Dictionary<string, List<float>> scalar, string key, int expected) =>
        scalar.TryGetValue(key, out var col) && col.Count == expected;

    private static (List<Element> Elements, bool Ascii, bool Little, int DataOffset) ParseHeader(byte[] bytes)
    {
        int marker = IndexOf(bytes, "end_header");
        if (marker < 0) throw new InvalidDataException("PLY has no end_header line");
        int lineEnd = marker;
        while (lineEnd < bytes.Length && bytes[lineEnd] != '\n') lineEnd++;
        int dataOffset = Math.Min(lineEnd + 1, bytes.Length);

        var header = Encoding.ASCII.GetString(bytes, 0, dataOffset);
        var lines = header.Split('\n');
        if (lines.Length == 0 || lines[0].Trim() != "ply")
            throw new InvalidDataException("PLY has no ply magic");

        bool ascii = false, little = true;
        var elements = new List<Element>();
        Element? current = null;
        for (int i = 1; i < lines.Length; i++)
        {
            var parts = lines[i].Trim().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) continue;
            switch (parts[0])
            {
                case "format" when parts.Length >= 2:
                    ascii = parts[1] == "ascii";
                    little = parts[1] != "binary_big_endian";
                    break;
                case "element" when parts.Length >= 3:
                    current = new Element { Name = parts[1], Count = int.Parse(parts[2], CultureInfo.InvariantCulture) };
                    elements.Add(current);
                    break;
                case "property" when parts.Length >= 3 && parts[1] == "list" && current is not null:
                    current.Props.Add(new Prop { Name = parts[4], IsList = true, CountType = parts[2], ValueType = parts[3] });
                    break;
                case "property" when parts.Length >= 3 && current is not null:
                    current.Props.Add(new Prop { Name = parts[2], Type = parts[1] });
                    break;
            }
        }
        return (elements, ascii, little, dataOffset);
    }

    private static void ReadAscii(byte[] bytes, int dataOffset, List<Element> elements,
        Dictionary<string, List<float>> scalar, List<int[]> faces)
    {
        var text = Encoding.UTF8.GetString(bytes, dataOffset, bytes.Length - dataOffset);
        var tokens = text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        int pos = 0;
        foreach (var el in elements)
        {
            for (int r = 0; r < el.Count; r++)
            {
                bool faceCaptured = false;
                foreach (var prop in el.Props)
                {
                    if (prop.IsList)
                    {
                        int n = ParseInt(tokens, ref pos);
                        if (el.Name == "face" && !faceCaptured)
                        {
                            var idx = new int[n];
                            for (int k = 0; k < n; k++) idx[k] = ParseInt(tokens, ref pos);
                            faces.Add(idx);
                            faceCaptured = true;
                        }
                        else
                        {
                            pos += n;
                        }
                    }
                    else
                    {
                        float v = ParseFloat(tokens, ref pos);
                        if (el.Name == "vertex") Append(scalar, el.Name + "." + prop.Name, v);
                    }
                }
            }
        }
    }

    private static void ReadBinary(byte[] bytes, int dataOffset, List<Element> elements,
        Dictionary<string, List<float>> scalar, List<int[]> faces)
    {
        int p = dataOffset;
        foreach (var el in elements)
        {
            for (int r = 0; r < el.Count; r++)
            {
                bool faceCaptured = false;
                foreach (var prop in el.Props)
                {
                    if (prop.IsList)
                    {
                        int n = (int)ReadScalar(bytes, ref p, prop.CountType);
                        if (el.Name == "face" && !faceCaptured)
                        {
                            var idx = new int[n];
                            for (int k = 0; k < n; k++) idx[k] = (int)ReadScalar(bytes, ref p, prop.ValueType);
                            faces.Add(idx);
                            faceCaptured = true;
                        }
                        else
                        {
                            p += n * TypeSize(prop.ValueType);
                        }
                    }
                    else
                    {
                        float v = (float)ReadScalar(bytes, ref p, prop.Type);
                        if (el.Name == "vertex") Append(scalar, el.Name + "." + prop.Name, v);
                    }
                }
            }
        }
    }

    private static void Append(Dictionary<string, List<float>> scalar, string key, float value)
    {
        if (!scalar.TryGetValue(key, out var col)) { col = new List<float>(); scalar[key] = col; }
        col.Add(value);
    }

    private static double ReadScalar(byte[] b, ref int p, string type)
    {
        switch (type)
        {
            case "char" or "int8": { sbyte v = unchecked((sbyte)b[p]); p += 1; return v; }
            case "uchar" or "uint8": { byte v = b[p]; p += 1; return v; }
            case "short" or "int16": { short v = BinaryPrimitives.ReadInt16LittleEndian(b.AsSpan(p, 2)); p += 2; return v; }
            case "ushort" or "uint16": { ushort v = BinaryPrimitives.ReadUInt16LittleEndian(b.AsSpan(p, 2)); p += 2; return v; }
            case "int" or "int32": { int v = BinaryPrimitives.ReadInt32LittleEndian(b.AsSpan(p, 4)); p += 4; return v; }
            case "uint" or "uint32": { uint v = BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(p, 4)); p += 4; return v; }
            case "float" or "float32": { float v = BinaryPrimitives.ReadSingleLittleEndian(b.AsSpan(p, 4)); p += 4; return v; }
            case "double" or "float64": { double v = BinaryPrimitives.ReadDoubleLittleEndian(b.AsSpan(p, 8)); p += 8; return v; }
            default: throw new InvalidDataException($"PLY has an unknown property type '{type}'");
        }
    }

    private static int TypeSize(string type) => type switch
    {
        "char" or "int8" or "uchar" or "uint8" => 1,
        "short" or "int16" or "ushort" or "uint16" => 2,
        "int" or "int32" or "uint" or "uint32" or "float" or "float32" => 4,
        "double" or "float64" => 8,
        _ => throw new InvalidDataException($"PLY has an unknown property type '{type}'"),
    };

    private static int ParseInt(string[] tokens, ref int pos) =>
        int.Parse(tokens[pos++], CultureInfo.InvariantCulture);

    private static float ParseFloat(string[] tokens, ref int pos) =>
        float.Parse(tokens[pos++], CultureInfo.InvariantCulture);

    private static int IndexOf(byte[] bytes, string needle)
    {
        var pattern = Encoding.ASCII.GetBytes(needle);
        for (int i = 0; i + pattern.Length <= bytes.Length; i++)
        {
            bool hit = true;
            for (int j = 0; j < pattern.Length; j++)
                if (bytes[i + j] != pattern[j]) { hit = false; break; }
            if (hit) return i;
        }
        return -1;
    }
}
