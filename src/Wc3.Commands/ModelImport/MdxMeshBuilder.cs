// src/Wc3.Commands/ModelImport/MdxMeshBuilder.cs
using System.Buffers.Binary;
using System.Numerics;
using System.Text;
using Wc3.Modeling;
using Wc3.Render;

namespace Wc3.Commands;

/// <summary>
/// The one path from a list of <see cref="MeshData"/> to a Warcraft III MDX. Every format importer,
/// OBJ included, ends here, so the geoset layout (VRTX, NRMS, PTYP, PCNT, PVTX, GNDX, MTGC, MATS,
/// the untagged tail and UVAS/UVBS), the TEXS texture table, the MTLS material table and the GEOS
/// chunk per primitive are written once. A model carrying animations also gets a SEQS chunk, one
/// named interval per sequence, written from the frame ranges the caller already converted.
///
/// One geoset is written per mesh, one material per mesh and one TEXS entry per distinct texture
/// path. Each primitive's material resolves to its own texture when the importer supplied one, so a
/// multi-material glTF keeps its materials instead of collapsing to the first. A primitive with no
/// material, and every primitive of a model that names no material at all, keeps the legacy single
/// default texture, which is the pre-materials behaviour.
///
/// Corners are deduplicated into per-vertex position, normal and UV arrays plus a triangle index
/// list. A corner with a real normal keys on it. A corner without one keys on its face, and the
/// face's computed normal is written into that vertex, so a face keeps flat shading and a shared
/// quad still collapses to its four corners. An n-gon face is split into a triangle fan.
/// </summary>
internal static class MdxMeshBuilder
{
    private const uint Version = 800u;

    /// <summary>A decoded texture written beside the MDX. <see cref="FileName"/> is the file on
    /// disk, <see cref="MapPath"/> is the TEXS path the game reads, and <see cref="Bytes"/> are the
    /// .blp bytes to write.</summary>
    internal sealed record BuiltTexture(string FileName, string MapPath, byte[] Bytes);

    public static ModelImportResult Build(
        IReadOnlyList<MeshData> meshes, string name, string? texturePath,
        IReadOnlyList<MeshSequence> sequences, IReadOnlyList<string> diagnostics,
        out byte[] output, out IReadOnlyList<BuiltTexture> textures)
    {
        ArgumentNullException.ThrowIfNull(sequences);
        ArgumentNullException.ThrowIfNull(meshes);

        var texturePaths = new List<string>(); // TEXS index -> map path
        var files = new List<BuiltTexture>();
        var idByKey = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var usedFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var diag = new List<string>(diagnostics);

        string legacyPath = string.IsNullOrWhiteSpace(texturePath) ? DefaultTexturePath(name) : texturePath!;
        bool forceSingle = !string.IsNullOrWhiteSpace(texturePath);

        int EnsurePath(string mapPath)
        {
            string key = "p:" + mapPath;
            if (idByKey.TryGetValue(key, out int existing)) return existing;
            int id = texturePaths.Count;
            texturePaths.Add(mapPath);
            idByKey[key] = id;
            return id;
        }

        int EnsureImage(MeshTexture tex)
        {
            string key = "t:" + tex.Name;
            if (idByKey.TryGetValue(key, out int existing)) return existing;

            byte[] blp;
            try
            {
                blp = TextureConvert.Convert(tex.Bytes, tex.Extension, ".blp");
            }
            catch (Exception ex)
            {
                // A texture that will not decode must not sink the whole import, the material stays
                // and points at no texture.
                diag.Add($"texture '{tex.Name}' could not be decoded ({ex.Message}), its material keeps no texture");
                idByKey[key] = -1;
                return -1;
            }

            string fileName = UniqueFileName(tex.Name);
            string mapPath = "war3mapImported\\" + fileName;
            files.Add(new BuiltTexture(fileName, mapPath, blp));
            int id = texturePaths.Count;
            texturePaths.Add(mapPath);
            idByKey[key] = id;
            return id;
        }

        int ResolveTextureId(MeshData mesh)
        {
            if (forceSingle) return EnsurePath(legacyPath);
            if (mesh.Material is null) return EnsurePath(legacyPath);
            return mesh.Material.Texture is { } tex ? EnsureImage(tex) : -1;
        }

        string UniqueFileName(string baseName)
        {
            foreach (char c in Path.GetInvalidFileNameChars()) baseName = baseName.Replace(c, '_');
            if (string.IsNullOrWhiteSpace(baseName)) baseName = "texture";
            string candidate = baseName + ".blp";
            int suffix = 1;
            while (!usedFileNames.Add(candidate)) candidate = baseName + "_" + suffix++ + ".blp";
            return candidate;
        }

        var geoChunks = new List<MdxDocument.Chunk>();
        var materialTextureIds = new List<int>();
        var allVertices = new List<float>();
        int vertexTotal = 0, faceTotal = 0;

        for (int i = 0; i < meshes.Count; i++)
        {
            var mesh = meshes[i];
            var (vList, nList, uvList, indices) = BuildMesh(mesh);
            int vertexCount = vList.Count / 3;
            int triangleCount = indices.Count / 3;
            vertexTotal += vertexCount;
            faceTotal += triangleCount;
            allVertices.AddRange(vList);

            materialTextureIds.Add(ResolveTextureId(mesh));

            var geosetBytes = BuildGeoset(vList, nList, uvList, indices, i).Write();
            var payload = new byte[4 + geosetBytes.Length];
            BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(0, 4), (uint)(geosetBytes.Length + 4));
            geosetBytes.CopyTo(payload, 4);
            geoChunks.Add(new MdxDocument.Chunk("GEOS", payload));
        }

        var chunks = new List<MdxDocument.Chunk>
        {
            new("VERS", U32(Version)),
        };
        // A model with animations carries a SEQS chunk, its extent taken from the same model bounds
        // the geosets carry so a sequence is not left with a zero radius. A model with none stays
        // byte-identical to the pre-animation build, so existing imports and goldens do not move.
        if (sequences.Count > 0)
        {
            var (radius, boundsMin, boundsMax) = Bounds(allVertices);
            chunks.Add(new("SEQS", BuildSeqs(sequences, radius, boundsMin, boundsMax)));
        }
        chunks.Add(new("TEXS", BuildTexs(texturePaths)));
        chunks.Add(new("MTLS", BuildMtls(materialTextureIds)));
        chunks.AddRange(geoChunks);

        var doc = MdxDocument.Create(chunks);
        output = doc.Write();
        textures = files;

        return new ModelImportResult(
            name, meshes.Count, vertexTotal, faceTotal, texturePaths.Count, meshes.Count,
            output.LongLength, output, files.Select(f => f.FileName).ToList(), diag,
            sequences.Count, sequences.Select(s => s.Name).ToList());
    }

    private static (List<float> Vertices, List<float> Normals, List<float> Uvs, List<int> Indices)
        BuildMesh(MeshData mesh)
    {
        var vertices = new List<float>();
        var vertexNormals = new List<float>();
        var uvs = new List<float>();
        var indices = new List<int>();
        var map = new Dictionary<(int V, int Uv, int Key), int>();

        int IndexOf(MeshCorner c, int face, Vector3 faceNormal)
        {
            int key = c.Normal >= 0 ? c.Normal : -(face + 1); // a face-local key when the source gives no normal
            if (map.TryGetValue((c.Position, c.Uv, key), out int existing)) return existing;

            int index = vertices.Count / 3;
            var p = mesh.Positions[c.Position];
            vertices.Add(p.X); vertices.Add(p.Y); vertices.Add(p.Z);

            var n = c.Normal >= 0 ? mesh.Normals[c.Normal] : faceNormal;
            n = n.LengthSquared() > 1e-20f ? Vector3.Normalize(n) : new Vector3(0, 0, 1);
            vertexNormals.Add(n.X); vertexNormals.Add(n.Y); vertexNormals.Add(n.Z);

            var uv = c.Uv >= 0 ? mesh.Uvs[c.Uv] : Vector2.Zero;
            uvs.Add(uv.X); uvs.Add(mesh.FlipV ? 1f - uv.Y : uv.Y);
            map[(c.Position, c.Uv, key)] = index;
            return index;
        }

        for (int f = 0; f < mesh.Faces.Count; f++)
        {
            var face = mesh.Faces[f];
            var faceNormal = FaceNormal(face, mesh.Positions);
            for (int k = 1; k + 1 < face.Length; k++) // triangle fan
            {
                indices.Add(IndexOf(face[0], f, faceNormal));
                indices.Add(IndexOf(face[k], f, faceNormal));
                indices.Add(IndexOf(face[k + 1], f, faceNormal));
            }
        }
        return (vertices, vertexNormals, uvs, indices);
    }

    /// <summary>The polygon normal by Newell's method, so a non-planar or n-gon face still gets one.</summary>
    private static Vector3 FaceNormal(MeshCorner[] face, IReadOnlyList<Vector3> positions)
    {
        var n = Vector3.Zero;
        for (int i = 0; i < face.Length; i++)
        {
            var p = positions[face[i].Position];
            var q = positions[face[(i + 1) % face.Length].Position];
            n.X += (p.Y - q.Y) * (p.Z + q.Z);
            n.Y += (p.Z - q.Z) * (p.X + q.X);
            n.Z += (p.X - q.X) * (p.Y + q.Y);
        }
        return n.LengthSquared() > 1e-20f ? Vector3.Normalize(n) : new Vector3(0, 0, 1);
    }

    private static MdxGeoset BuildGeoset(
        List<float> vertices, List<float> normals, List<float> uvs, List<int> indices, int materialId)
    {
        int vertexCount = vertices.Count / 3;
        int triangleCount = indices.Count / 3;
        var (radius, min, max) = Bounds(vertices);

        return MdxGeoset.Create(new[]
        {
            FloatSegment("VRTX", vertices, vertexCount),
            FloatSegment("NRMS", normals, vertexCount),
            U32Segment("PTYP", new[] { 4u }),        // 4 = triangle list
            U32Segment("PCNT", new[] { (uint)triangleCount }),
            U16Segment("PVTX", indices),
            BytesSegment("GNDX", new byte[vertexCount]), // every vertex sits in matrix group 0
            U32Segment("MTGC", new[] { 1u }),
            U32Segment("MATS", new[] { 0u }),
            Tail(radius, min, max, materialId),
            UvasSegment(uvs, vertexCount),
        }, Version);
    }

    /// <summary>count + x,y,z per vertex.</summary>
    private static MdxGeoset.Segment FloatSegment(string tag, List<float> values, int count)
    {
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        w.Write((uint)count);
        foreach (var f in values) w.Write(f);
        w.Flush();
        return new MdxGeoset.Segment(tag, ms.ToArray());
    }

    private static MdxGeoset.Segment U16Segment(string tag, List<int> values)
    {
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        w.Write((uint)values.Count);
        foreach (var v in values) w.Write((ushort)v);
        w.Flush();
        return new MdxGeoset.Segment(tag, ms.ToArray());
    }

    private static MdxGeoset.Segment U32Segment(string tag, uint[] values)
    {
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        w.Write((uint)values.Length);
        foreach (var v in values) w.Write(v);
        w.Flush();
        return new MdxGeoset.Segment(tag, ms.ToArray());
    }

    private static MdxGeoset.Segment BytesSegment(string tag, byte[] values)
    {
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        w.Write((uint)values.Length);
        w.Write(values);
        w.Flush();
        return new MdxGeoset.Segment(tag, ms.ToArray());
    }

    /// <summary>The untagged tail, material id, selection fields, extent and per-sequence extents.</summary>
    private static MdxGeoset.Segment Tail(float radius, Vector3 min, Vector3 max, int materialId)
    {
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        w.Write((uint)materialId);  // materialId
        w.Write(0u);            // selectionGroup
        w.Write(0u);            // selectionFlags
        w.Write(radius);        // extent radius
        w.Write(min.X); w.Write(min.Y); w.Write(min.Z);
        w.Write(max.X); w.Write(max.Y); w.Write(max.Z);
        w.Write(0u);            // per-sequence extent count
        w.Flush();
        return new MdxGeoset.Segment("", ms.ToArray());
    }

    /// <summary>UVAS, one UV set. Payload is the set count then a UVBS with count + u,v per vertex.</summary>
    private static MdxGeoset.Segment UvasSegment(List<float> uvs, int vertexCount)
    {
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        w.Write(1u);            // number of UV sets
        w.Write("UVBS"u8);
        w.Write((uint)vertexCount);
        foreach (var f in uvs) w.Write(f);
        w.Flush();
        return new MdxGeoset.Segment("UVAS", ms.ToArray());
    }

    private static (float Radius, Vector3 Min, Vector3 Max) Bounds(List<float> vertices)
    {
        if (vertices.Count == 0) return (0f, Vector3.Zero, Vector3.Zero);
        var min = new Vector3(float.MaxValue);
        var max = new Vector3(float.MinValue);
        for (int i = 0; i + 2 < vertices.Count; i += 3)
        {
            min = Vector3.Min(min, new Vector3(vertices[i], vertices[i + 1], vertices[i + 2]));
            max = Vector3.Max(max, new Vector3(vertices[i], vertices[i + 1], vertices[i + 2]));
        }
        var center = (min + max) * 0.5f;
        float radius = 0f;
        for (int i = 0; i + 2 < vertices.Count; i += 3)
            radius = MathF.Max(radius, Vector3.Distance(center, new Vector3(vertices[i], vertices[i + 1], vertices[i + 2])));
        return (radius, min, max);
    }

    /// <summary>One 268-byte TEXS row per path, replaceableId 0 then a fixed 260-byte path then flags 0.</summary>
    private static byte[] BuildTexs(IReadOnlyList<string> paths)
    {
        var payload = new byte[paths.Count * 268];
        for (int i = 0; i < paths.Count; i++)
        {
            var bytes = Encoding.ASCII.GetBytes(paths[i]);
            Array.Copy(bytes, 0, payload, i * 268 + 4, Math.Min(bytes.Length, 260));
        }
        return payload;
    }

    /// <summary>
    /// One 132-byte SEQS record per sequence, the layout the parser reads. char[80] name, u32 start,
    /// u32 end, float moveSpeed, u32 nonLooping flags, float rarity, u32 syncPoint, then the extent as
    /// a radius and a min/max corner. The extent is the whole model's bounds, so the sequence has a
    /// real culling volume rather than a zero radius. Move speed, flags, rarity and sync point are
    /// left at zero, the honest defaults for an interval the source did not describe.
    /// </summary>
    private static byte[] BuildSeqs(
        IReadOnlyList<MeshSequence> sequences, float radius, Vector3 min, Vector3 max)
    {
        var payload = new byte[sequences.Count * 132];
        for (int i = 0; i < sequences.Count; i++)
        {
            var sequence = sequences[i];
            int o = i * 132;
            var nameBytes = Encoding.UTF8.GetBytes(sequence.Name);
            Array.Copy(nameBytes, 0, payload, o, Math.Min(nameBytes.Length, 80));
            BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(o + 80, 4), (uint)Math.Max(0, sequence.StartFrame));
            BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(o + 84, 4), (uint)Math.Max(0, sequence.EndFrame));
            BinaryPrimitives.WriteSingleLittleEndian(payload.AsSpan(o + 88, 4), 0f);   // moveSpeed
            BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(o + 92, 4), 0u);   // nonLooping flags
            BinaryPrimitives.WriteSingleLittleEndian(payload.AsSpan(o + 96, 4), 0f);   // rarity
            BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(o + 100, 4), 0u);  // syncPoint
            BinaryPrimitives.WriteSingleLittleEndian(payload.AsSpan(o + 104, 4), radius);
            BinaryPrimitives.WriteSingleLittleEndian(payload.AsSpan(o + 108, 4), min.X);
            BinaryPrimitives.WriteSingleLittleEndian(payload.AsSpan(o + 112, 4), min.Y);
            BinaryPrimitives.WriteSingleLittleEndian(payload.AsSpan(o + 116, 4), min.Z);
            BinaryPrimitives.WriteSingleLittleEndian(payload.AsSpan(o + 120, 4), max.X);
            BinaryPrimitives.WriteSingleLittleEndian(payload.AsSpan(o + 124, 4), max.Y);
            BinaryPrimitives.WriteSingleLittleEndian(payload.AsSpan(o + 128, 4), max.Z);
        }
        return payload;
    }

    /// <summary>One material per texture id, each a single LAYS layer whose textureId is that entry,
    /// or 0xFFFFFFFF when the material has no texture.</summary>
    private static byte[] BuildMtls(IReadOnlyList<int> textureIds)
    {
        const uint layerSize = 28u;
        uint matSize = 4 + 4 + 4 + 8 + layerSize; // inclusiveSize, priorityPlane, flags, LAYS+count, layer
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        foreach (int textureId in textureIds)
        {
            w.Write(matSize);
            w.Write(0u);            // priorityPlane
            w.Write(0u);            // flags
            w.Write("LAYS"u8);
            w.Write(1u);            // layer count
            w.Write(layerSize);
            w.Write(0u);            // filterMode, opaque
            w.Write(0u);            // shadingFlags
            w.Write((uint)textureId); // textureId
            w.Write(0xFFFFFFFFu);   // textureAnimationId
            w.Write(0u);            // coordId
            w.Write(1.0f);          // alpha
        }
        w.Flush();
        return ms.ToArray();
    }

    private static string DefaultTexturePath(string name)
    {
        string baseName = Path.GetFileNameWithoutExtension(name);
        if (string.IsNullOrWhiteSpace(baseName)) baseName = "Imported";
        return "war3mapImported\\" + baseName + ".blp";
    }

    private static byte[] U32(uint value)
    {
        var bytes = new byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(bytes, value);
        return bytes;
    }
}
