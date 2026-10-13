// src/Wc3.Commands/ModelImport/FbxImporter.cs
using System.Globalization;
using System.Numerics;

namespace Wc3.Commands;

/// <summary>
/// A parsed ASCII FBX scene, one <see cref="MeshData"/> per Geometry node and any diagnostics raised
/// while resolving a model transform. Mirrors <c>GltfScene</c> so the importer can surface both.
/// </summary>
internal sealed record FbxScene(IReadOnlyList<MeshData> Meshes, IReadOnlyList<string> Diagnostics);

/// <summary>
/// Parses the ASCII form of an FBX file into one <see cref="MeshData"/> per Geometry node. FBX is a
/// node tree in text, so a small lexer and node parser read it with no dependency. Each Geometry's
/// Vertices are the position array, PolygonVertexIndex is the corner stream where the last index of
/// every polygon is bitwise-not'd to mark its end, LayerElementNormal's Normals are the normals and
/// LayerElementUV's UV are the texture coordinates. A polygon is kept whole and the MDX builder
/// splits it into a triangle fan. The FBX V axis is taken bottom-up like OBJ, so V is flipped back
/// to WC3's top-down sense.
///
/// Normals and UVs are resolved through each layer's MappingInformationType and
/// ReferenceInformationType, so ByPolygonVertex (Direct and IndexToDirect), ByPolygon and
/// ByVertice/ByVertex are all read. A layer that is absent gives its corners a missing attribute and
/// the builder computes a per-face normal or zeroes the UV.
///
/// The binary form is a compressed container and is refused with a clear message, no dependency is
/// taken for it. A Model's local transform is applied when it links to its Geometry through the
/// Connections section and carries only a Translation, Rotation and Scaling, so the common export is
/// placed correctly. A transform with pivots, offsets or pre/post rotation is left unapplied and
/// reported, because guessing it would move the geometry.
/// </summary>
internal static class FbxImporter
{
    public static FbxScene Parse(string text, string name)
    {
        ArgumentNullException.ThrowIfNull(text);
        if (text.StartsWith("Kaydara FBX Binary", StringComparison.Ordinal))
            throw new NotSupportedException(
                $"FBX '{name}' is the binary form (magic Kaydara FBX Binary), which is not supported. "
                + "Re-export the model as ASCII FBX.");

        var root = ParseTree(text);
        var objects = root.Child("Objects")
            ?? throw new ArgumentException($"FBX '{name}' has no Objects section");

        var diagnostics = new List<string>();
        var parents = BuildParentMap(root.Child("Connections"));
        var models = new Dictionary<long, FbxNode>();
        foreach (var model in objects.ChildrenNamed("Model"))
            if (model.Id() is { } id) models[id] = model;

        var meshes = new List<MeshData>();
        foreach (var geometry in objects.ChildrenNamed("Geometry"))
        {
            var mesh = ExtractGeometry(geometry, models, parents, name, diagnostics);
            if (mesh is not null) meshes.Add(mesh);
        }

        if (meshes.Count == 0)
            throw new ArgumentException($"FBX '{name}' holds no drawable geometry");
        return new FbxScene(meshes, diagnostics);
    }

    private static MeshData? ExtractGeometry(
        FbxNode geometry, Dictionary<long, FbxNode> models, Dictionary<long, long> parents,
        string name, List<string> diagnostics)
    {
        var positions = ToVec3(geometry.Child("Vertices")?.Child("a")?.Numbers());
        if (positions.Count == 0) return null;

        var rawIndices = ToInts(geometry.Child("PolygonVertexIndex")?.Child("a")?.Numbers());
        var polygons = SplitPolygons(rawIndices);
        if (polygons.Count == 0) return null;

        var (normals, normalMapping, normalIndex) = ReadNormalLayer(geometry);
        var (uvs, uvMapping, uvIndex) = ReadUvLayer(geometry);

        var faces = new List<MeshCorner[]>(polygons.Count);
        int corner = 0;
        for (int p = 0; p < polygons.Count; p++)
        {
            var poly = polygons[p];
            var corners = new MeshCorner[poly.Length];
            for (int k = 0; k < poly.Length; k++)
            {
                int vi = poly[k];
                if (vi < 0 || vi >= positions.Count)
                    throw new InvalidDataException(
                        $"FBX '{name}' polygon index {vi} is out of range (have {positions.Count})");
                int ni = ResolveLayerIndex(normalMapping, corner, p, vi, normals.Count, normalIndex);
                int ui = ResolveLayerIndex(uvMapping, corner, p, vi, uvs.Count, uvIndex);
                corners[k] = new MeshCorner(vi, ni, ui);
                corner++;
            }
            faces.Add(corners);
        }

        var transform = ModelTransform(geometry.Id(), models, parents, name, diagnostics);
        if (transform != Matrix4x4.Identity)
        {
            for (int i = 0; i < positions.Count; i++)
                positions[i] = Vector3.Transform(positions[i], transform);
            var normalMatrix = transform;
            normalMatrix.M41 = normalMatrix.M42 = normalMatrix.M43 = 0f;
            for (int i = 0; i < normals.Count; i++)
            {
                var n = Vector3.Transform(normals[i], normalMatrix);
                normals[i] = n.LengthSquared() > 1e-20f ? Vector3.Normalize(n) : normals[i];
            }
        }

        return new MeshData(positions, normals, uvs, faces, FlipV: true);
    }

    // ---- connections ----

    /// <summary>child id to parent id from the "OO" entries of the Connections section.</summary>
    private static Dictionary<long, long> BuildParentMap(FbxNode? connections)
    {
        var map = new Dictionary<long, long>();
        if (connections is null) return map;
        foreach (var c in connections.ChildrenNamed("C"))
        {
            if (c.Text(0) != "OO") continue;
            if (c.Number(1) is not { } child || c.Number(2) is not { } parent) continue;
            map[(long)child] = (long)parent;
        }
        return map;
    }

    // ---- layer elements ----

    private readonly record struct LayerMapping(string Mapping, string Reference);

    private static (List<Vector3> Data, LayerMapping Mapping, List<int>? Index) ReadNormalLayer(FbxNode geometry)
    {
        var layer = geometry.Child("LayerElementNormal");
        if (layer is null) return (new List<Vector3>(), default, null);
        var data = ToVec3(layer.Child("Normals")?.Child("a")?.Numbers());
        var index = ToInts(layer.Child("NormalsIndex")?.Child("a")?.Numbers());
        return (data, ReadMapping(layer), index.Count > 0 ? index : null);
    }

    private static (List<Vector2> Data, LayerMapping Mapping, List<int>? Index) ReadUvLayer(FbxNode geometry)
    {
        var layer = geometry.Child("LayerElementUV");
        if (layer is null) return (new List<Vector2>(), default, null);
        var data = ToVec2(layer.Child("UV")?.Child("a")?.Numbers());
        var index = ToInts(layer.Child("UVIndex")?.Child("a")?.Numbers());
        return (data, ReadMapping(layer), index.Count > 0 ? index : null);
    }

    private static LayerMapping ReadMapping(FbxNode layer)
    {
        string mapping = layer.Child("MappingInformationType")?.Text(0) ?? "ByPolygonVertex";
        string reference = layer.Child("ReferenceInformationType")?.Text(0) ?? "Direct";
        return new LayerMapping(mapping, reference);
    }

    /// <summary>
    /// The index into a layer's data array for one polygon corner, honouring the layer's mapping and
    /// reference type. Returns -1 for a corner with no attribute so the builder fills it in.
    /// </summary>
    private static int ResolveLayerIndex(
        LayerMapping mapping, int corner, int polygon, int vertex, int count, List<int>? indexArray)
    {
        if (count == 0) return -1;
        int i;
        switch (mapping.Mapping)
        {
            case "ByVertice" or "ByVertex":
                i = vertex;
                break;
            case "ByPolygon":
                i = polygon;
                break;
            default: // ByPolygonVertex
                i = mapping.Reference == "IndexToDirect" && indexArray is not null
                    ? (corner < indexArray.Count ? indexArray[corner] : -1)
                    : corner;
                break;
        }
        return i >= 0 && i < count ? i : -1;
    }

    // ---- transforms ----

    private static Matrix4x4 ModelTransform(
        long? geometryId, Dictionary<long, FbxNode> models, Dictionary<long, long> parents,
        string name, List<string> diagnostics)
    {
        if (geometryId is null || models.Count == 0) return Matrix4x4.Identity;
        var total = Matrix4x4.Identity;
        long current = geometryId.Value;
        var seen = new HashSet<long> { current };
        while (parents.TryGetValue(current, out long parent) && models.TryGetValue(parent, out var model))
        {
            if (!seen.Add(parent))
            {
                diagnostics.Add($"FBX '{name}' has a model transform cycle, so no transform was applied");
                return Matrix4x4.Identity;
            }
            if (!TryModelMatrix(model, name, diagnostics, out var matrix)) return Matrix4x4.Identity;
            total *= matrix;
            current = parent;
        }
        return total;
    }

    private static bool TryModelMatrix(FbxNode model, string name, List<string> diagnostics, out Matrix4x4 matrix)
    {
        matrix = Matrix4x4.Identity;
        var properties = model.Child("Properties70");
        if (properties is null) return true;

        var p = properties.ChildrenNamed("P").ToList();
        string? unsupported = UnsupportedProperty(p);
        if (unsupported is not null)
        {
            diagnostics.Add(
                $"FBX '{name}' model transform carries '{unsupported}', so it was not applied");
            return false;
        }

        var translation = FindVector(p, "Lcl Translation", new Vector3(0f, 0f, 0f));
        var rotation = FindVector(p, "Lcl Rotation", new Vector3(0f, 0f, 0f));
        var scaling = FindVector(p, "Lcl Scaling", new Vector3(1f, 1f, 1f));
        int order = (int)FindNumber(p, "RotationOrder", 0);
        if (order is < 0 or > 5)
        {
            diagnostics.Add($"FBX '{name}' model rotation order {order} is unknown, so it was not applied");
            return false;
        }

        matrix = Matrix4x4.CreateScale(scaling) * EulerRotation(rotation, order) * Matrix4x4.CreateTranslation(translation);
        return true;
    }

    private static readonly string[] UnsupportedProperties =
    {
        "PreRotation", "PostRotation", "RotationPivot", "ScalingPivot",
        "RotationOffset", "ScalingOffset", "GeometricTranslation",
        "GeometricRotation", "GeometricScaling",
    };

    private static string? UnsupportedProperty(List<FbxNode> p)
    {
        foreach (var node in p)
        {
            string? name = node.Text(0);
            if (name is null) continue;
            foreach (var candidate in UnsupportedProperties)
                if (name == candidate) return candidate;
        }
        return null;
    }

    /// <summary>The Euler rotation for a FBX order code, degrees, applied X then Y then Z for order 0.</summary>
    private static Matrix4x4 EulerRotation(Vector3 degrees, int order)
    {
        float x = degrees.X * MathF.PI / 180f;
        float y = degrees.Y * MathF.PI / 180f;
        float z = degrees.Z * MathF.PI / 180f;
        var rx = Matrix4x4.CreateRotationX(x);
        var ry = Matrix4x4.CreateRotationY(y);
        var rz = Matrix4x4.CreateRotationZ(z);
        return order switch
        {
            1 => rx * rz * ry, // XZY
            2 => ry * rz * rx, // YZX
            3 => ry * rx * rz, // YXZ
            4 => rz * rx * ry, // ZXY
            5 => rz * ry * rx, // ZYX
            _ => rx * ry * rz, // XYZ
        };
    }

    private static Vector3 FindVector(List<FbxNode> p, string name, Vector3 fallback)
    {
        var node = p.FirstOrDefault(n => n.Text(0) == name);
        var numbers = node?.Numbers();
        if (numbers is null || numbers.Length < 3) return fallback;
        return new Vector3((float)numbers[0], (float)numbers[1], (float)numbers[2]);
    }

    private static double FindNumber(List<FbxNode> p, string name, double fallback)
    {
        var node = p.FirstOrDefault(n => n.Text(0) == name);
        var numbers = node?.Numbers();
        return numbers is { Length: >= 1 } ? numbers[0] : fallback;
    }

    // ---- polygon stream ----

    /// <summary>
    /// The polygon corner stream, where the last index of every polygon is bitwise-not'd. Splitting
    /// where that marker lands reconstructs the polygons in winding order.
    /// </summary>
    private static List<int[]> SplitPolygons(List<int> raw)
    {
        var polygons = new List<int[]>();
        var current = new List<int>();
        foreach (int encoded in raw)
        {
            if (encoded < 0)
            {
                current.Add(-encoded - 1);
                polygons.Add(current.ToArray());
                current.Clear();
            }
            else
            {
                current.Add(encoded);
            }
        }
        if (current.Count > 0) polygons.Add(current.ToArray());
        return polygons;
    }

    // ---- value readers ----

    private static List<Vector3> ToVec3(double[]? values)
    {
        var list = new List<Vector3>();
        if (values is null) return list;
        for (int i = 0; i + 2 < values.Length; i += 3)
            list.Add(new Vector3((float)values[i], (float)values[i + 1], (float)values[i + 2]));
        return list;
    }

    private static List<Vector2> ToVec2(double[]? values)
    {
        var list = new List<Vector2>();
        if (values is null) return list;
        for (int i = 0; i + 1 < values.Length; i += 2)
            list.Add(new Vector2((float)values[i], (float)values[i + 1]));
        return list;
    }

    private static List<int> ToInts(double[]? values)
    {
        var list = new List<int>();
        if (values is null) return list;
        foreach (double v in values) list.Add((int)Math.Round(v));
        return list;
    }

    // ---- node tree ----

    private enum TokKind { Word, String, Number, Open, Close, Comma, Colon, Star, Newline }

    private readonly record struct Tok(TokKind Kind, string Text, double Num);

    private sealed class FbxNode
    {
        public FbxNode(string name) => Name = name;
        public string Name { get; }
        public long? ArrayCount { get; set; }
        public List<Tok> Values { get; } = new();
        public List<FbxNode> Children { get; } = new();

        public FbxNode? Child(string name) => Children.FirstOrDefault(c => c.Name == name);
        public IEnumerable<FbxNode> ChildrenNamed(string name) => Children.Where(c => c.Name == name);

        public string? Text(int index) =>
            index >= 0 && index < Values.Count && Values[index].Kind == TokKind.String ? Values[index].Text : null;

        public double? Number(int index) =>
            index >= 0 && index < Values.Count && Values[index].Kind == TokKind.Number ? Values[index].Num : null;

        public long? Id() => Number(0) is { } v ? (long)v : null;

        public double[] Numbers() =>
            Values.Where(v => v.Kind == TokKind.Number).Select(v => v.Num).ToArray();
    }

    private static FbxNode ParseTree(string text)
    {
        var tokens = Lex(text);
        var root = new FbxNode("(root)");
        int i = 0;
        while (i < tokens.Count)
        {
            SkipTrivia(tokens, ref i);
            if (i >= tokens.Count) break;
            if (tokens[i].Kind != TokKind.Word) { i++; continue; }
            root.Children.Add(ParseNode(tokens, ref i));
        }
        return root;
    }

    private static FbxNode ParseNode(List<Tok> tokens, ref int i)
    {
        var node = new FbxNode(tokens[i++].Text);
        if (i < tokens.Count && tokens[i].Kind == TokKind.Colon)
        {
            i++;
            ReadValues(node, tokens, ref i);
            ContinueAcrossNewlines(node, tokens, ref i);
        }
        SkipTrivia(tokens, ref i);
        if (i < tokens.Count && tokens[i].Kind == TokKind.Open)
        {
            i++;
            while (true)
            {
                SkipTrivia(tokens, ref i);
                if (i >= tokens.Count) break;
                if (tokens[i].Kind == TokKind.Close) { i++; break; }
                if (tokens[i].Kind == TokKind.Word) node.Children.Add(ParseNode(tokens, ref i));
                else i++;
            }
        }
        return node;
    }

    /// <summary>Reads the value list after a colon, stopping at a brace, a newline or the next node.</summary>
    private static void ReadValues(FbxNode node, List<Tok> tokens, ref int i)
    {
        while (i < tokens.Count)
        {
            var t = tokens[i];
            if (t.Kind == TokKind.Star)
            {
                i++;
                if (i < tokens.Count && tokens[i].Kind == TokKind.Number) { node.ArrayCount = (long)tokens[i].Num; i++; }
                continue;
            }
            if (t.Kind == TokKind.Comma) { i++; continue; }
            if (t.Kind is TokKind.Number or TokKind.String) { node.Values.Add(t); i++; continue; }
            break;
        }
    }

    /// <summary>Joins an array that was wrapped onto the next line, which starts with a value token.</summary>
    private static void ContinueAcrossNewlines(FbxNode node, List<Tok> tokens, ref int i)
    {
        while (i < tokens.Count && tokens[i].Kind == TokKind.Newline)
        {
            int j = i;
            while (j < tokens.Count && tokens[j].Kind == TokKind.Newline) j++;
            if (j >= tokens.Count) break;
            var kind = tokens[j].Kind;
            if (kind is not (TokKind.Number or TokKind.String or TokKind.Star or TokKind.Comma)) break;
            i = j;
            ReadValues(node, tokens, ref i);
        }
    }

    private static void SkipTrivia(List<Tok> tokens, ref int i)
    {
        while (i < tokens.Count && tokens[i].Kind is TokKind.Newline or TokKind.Comma) i++;
    }

    private static List<Tok> Lex(string text)
    {
        var tokens = new List<Tok>();
        int i = 0, n = text.Length;
        while (i < n)
        {
            char ch = text[i];
            if (ch == '\r') { i++; continue; }
            if (ch == '\n') { tokens.Add(new Tok(TokKind.Newline, "\n", 0)); i++; continue; }
            if (ch is ' ' or '\t') { i++; continue; }
            if (ch == ';') { while (i < n && text[i] != '\n') i++; continue; }
            switch (ch)
            {
                case '{': tokens.Add(new Tok(TokKind.Open, "{", 0)); i++; continue;
                case '}': tokens.Add(new Tok(TokKind.Close, "}", 0)); i++; continue;
                case ',': tokens.Add(new Tok(TokKind.Comma, ",", 0)); i++; continue;
                case ':': tokens.Add(new Tok(TokKind.Colon, ":", 0)); i++; continue;
                case '*': tokens.Add(new Tok(TokKind.Star, "*", 0)); i++; continue;
            }

            if (ch == '"')
            {
                int start = ++i;
                while (i < n && text[i] != '"') i++;
                tokens.Add(new Tok(TokKind.String, text[start..Math.Min(i, n)], 0));
                i++;
                continue;
            }

            if (ch is '-' or '+' or '.' || char.IsDigit(ch))
            {
                int start = i;
                i++;
                while (i < n)
                {
                    char c = text[i];
                    if (char.IsDigit(c) || c == '.') { i++; continue; }
                    if ((c == 'e' || c == 'E') && i + 1 < n
                        && (char.IsDigit(text[i + 1]) || ((text[i + 1] is '+' or '-') && i + 2 < n && char.IsDigit(text[i + 2]))))
                    {
                        i += text[i + 1] is '+' or '-' ? 2 : 1;
                        continue;
                    }
                    if ((c is '+' or '-') && i > start && (text[i - 1] is 'e' or 'E')) { i++; continue; }
                    break;
                }
                string s = text[start..i];
                double num = double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double d) ? d : 0;
                tokens.Add(new Tok(TokKind.Number, s, num));
                continue;
            }

            {
                int start = i;
                while (i < n && (char.IsLetterOrDigit(text[i]) || text[i] == '_')) i++;
                if (i == start) { i++; continue; }
                tokens.Add(new Tok(TokKind.Word, text[start..i], 0));
            }
        }
        return tokens;
    }
}
