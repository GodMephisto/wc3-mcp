// tests/Wc3.Tests/ModelImportFormatTests.cs
using System.Buffers.Binary;
using System.Text;
using Wc3.Commands;
using Wc3.Modeling;

namespace Wc3.Tests;

/// <summary>
/// The foreign model importers beyond OBJ. Each format is built as a tiny hermetic fixture, written
/// to a temp file and imported through the real path, then the produced MDX is parsed and its vertex
/// and face counts checked. A detection test per magic format proves the extension is not trusted.
/// </summary>
public sealed class ModelImportFormatTests : IDisposable
{
    private readonly string _dir = Path.Combine(
        Path.GetTempPath(), "wc3ctl-import-fmt", Guid.NewGuid().ToString("N"));

    public ModelImportFormatTests() => Directory.CreateDirectory(_dir);

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch { /* best effort */ }
    }

    private string Write(string name, string text)
    {
        var path = Path.Combine(_dir, name);
        File.WriteAllText(path, text);
        return path;
    }

    private string Write(string name, byte[] bytes)
    {
        var path = Path.Combine(_dir, name);
        File.WriteAllBytes(path, bytes);
        return path;
    }

    private static (int Vertices, int Faces) ImportCounts(string inputPath, string outPath)
    {
        var r = ModelImportCommand.ExecutePath(inputPath, outPath);
        var model = ModelParser.Parse(File.ReadAllBytes(outPath), "imported.mdx");
        var geoset = Assert.Single(model.Geosets);
        Assert.Equal(3, geoset.Vertices.Length / 3);
        Assert.Equal(3, geoset.Indices.Length);
        return (r.Vertices, r.Faces);
    }

    [Fact]
    public void A_tiny_obj_still_imports()
    {
        var path = Write("tri.obj", "v 0 0 0\nv 1 0 0\nv 0 1 0\nf 1 2 3\n");
        var (vertices, faces) = ImportCounts(path, Path.Combine(_dir, "obj.mdx"));
        Assert.Equal(3, vertices);
        Assert.Equal(1, faces);
    }

    [Fact]
    public void A_gltf_json_imports()
    {
        var path = Write("tri.gltf", TriangleGltfJson());
        var (vertices, faces) = ImportCounts(path, Path.Combine(_dir, "gltf.mdx"));

        Assert.Equal(3, vertices);
        Assert.Equal(1, faces);
        var model = ModelParser.Parse(File.ReadAllBytes(Path.Combine(_dir, "gltf.mdx")), "gltf.mdx");
        var geoset = model.Geosets[0];
        Assert.Equal(9, geoset.Normals.Length); // NORMAL accessor carried through
        Assert.Equal(6, geoset.Uvs.Length);     // TEXCOORD_0 accessor carried through
    }

    [Fact]
    public void A_glb_container_imports()
    {
        var glb = BuildGlb(TriangleGltfJson(), TriangleBuffer());
        var path = Write("tri.glb", glb);
        var (vertices, faces) = ImportCounts(path, Path.Combine(_dir, "glb.mdx"));
        Assert.Equal(3, vertices);
        Assert.Equal(1, faces);
    }

    [Fact]
    public void A_binary_stl_imports_without_uvs()
    {
        var path = Write("tri.stl", BuildBinaryStl());
        var outPath = Path.Combine(_dir, "stl-bin.mdx");
        var (vertices, faces) = ImportCounts(path, outPath);
        Assert.Equal(3, vertices);
        Assert.Equal(1, faces);

        var geoset = ModelParser.Parse(File.ReadAllBytes(outPath), "stl.mdx").Geosets[0];
        Assert.Equal(9, geoset.Normals.Length);
        Assert.Equal(6, geoset.Uvs.Length); // UVAS is always written, the values are zero
        Assert.All(geoset.Uvs, u => Assert.Equal(0f, u, 5));
    }

    [Fact]
    public void An_ascii_stl_with_a_zero_facet_normal_gets_a_computed_one()
    {
        const string ascii = """
            solid tri
              facet normal 0 0 0
                outer loop
                  vertex 0 0 0
                  vertex 1 0 0
                  vertex 0 1 0
                endloop
              endfacet
            endsolid tri
            """;
        var path = Write("tri-ascii.stl", ascii);
        var outPath = Path.Combine(_dir, "stl-ascii.mdx");
        var (vertices, faces) = ImportCounts(path, outPath);
        Assert.Equal(3, vertices);
        Assert.Equal(1, faces);

        var normals = ModelParser.Parse(File.ReadAllBytes(outPath), "stl.mdx").Geosets[0].Normals;
        for (int v = 0; v < 3; v++)
        {
            Assert.Equal(0f, normals[v * 3], 5);
            Assert.Equal(0f, normals[v * 3 + 1], 5);
            Assert.Equal(1f, normals[v * 3 + 2], 5); // CCW in XY faces +Z
        }
    }

    [Fact]
    public void An_ascii_ply_imports()
    {
        const string ply = """
            ply
            format ascii 1.0
            element vertex 3
            property float x
            property float y
            property float z
            property float nx
            property float ny
            property float nz
            property float s
            property float t
            element face 1
            property list uchar int vertex_indices
            end_header
            0 0 0 0 0 1 0 0
            1 0 0 0 0 1 1 0
            0 1 0 0 0 1 0 1
            3 0 1 2
            """;
        var path = Write("tri.ply", ply);
        var outPath = Path.Combine(_dir, "ply.mdx");
        var (vertices, faces) = ImportCounts(path, outPath);
        Assert.Equal(3, vertices);
        Assert.Equal(1, faces);

        var geoset = ModelParser.Parse(File.ReadAllBytes(outPath), "ply.mdx").Geosets[0];
        Assert.Equal(9, geoset.Normals.Length);
        Assert.Equal(6, geoset.Uvs.Length);
    }

    [Fact]
    public void A_binary_ply_imports()
    {
        var path = Write("tri-bin.ply", BuildBinaryPly());
        var outPath = Path.Combine(_dir, "ply-bin.mdx");
        var (vertices, faces) = ImportCounts(path, outPath);
        Assert.Equal(3, vertices);
        Assert.Equal(1, faces);
    }

    [Fact]
    public void A_glb_renamed_to_obj_is_detected_by_its_magic()
    {
        var path = Write("lied.obj", BuildGlb(TriangleGltfJson(), TriangleBuffer()));
        var (vertices, faces) = ImportCounts(path, Path.Combine(_dir, "lied.mdx"));
        Assert.Equal(3, vertices);
        Assert.Equal(1, faces);
    }

    [Fact]
    public void A_binary_stl_renamed_to_obj_is_detected_by_its_length()
    {
        var path = Write("lied.obj", BuildBinaryStl());
        var (vertices, faces) = ImportCounts(path, Path.Combine(_dir, "stl-lied.mdx"));
        Assert.Equal(3, vertices);
        Assert.Equal(1, faces);
    }

    [Fact]
    public void A_ply_renamed_to_stl_is_detected_by_its_magic()
    {
        var path = Write("lied.stl", "ply\nformat ascii 1.0\nelement vertex 3\n"
            + "property float x\nproperty float y\nproperty float z\nelement face 1\n"
            + "property list uchar int vertex_indices\nend_header\n"
            + "0 0 0\n1 0 0\n0 1 0\n3 0 1 2\n");
        var (vertices, faces) = ImportCounts(path, Path.Combine(_dir, "ply-lied.mdx"));
        Assert.Equal(3, vertices);
        Assert.Equal(1, faces);
    }

    [Fact]
    public void An_ascii_fbx_with_two_triangles_imports()
    {
        var path = Write("quads.fbx", FbxFixture());
        var outPath = Path.Combine(_dir, "fbx.mdx");
        var r = ModelImportCommand.ExecutePath(path, outPath);
        Assert.Equal(6, r.Vertices);
        Assert.Equal(2, r.Faces);

        var geoset = ModelParser.Parse(File.ReadAllBytes(outPath), "fbx.mdx").Geosets[0];
        Assert.Equal(6, geoset.Vertices.Length / 3);
        Assert.Equal(6, geoset.Indices.Length);
        Assert.Equal(18, geoset.Normals.Length);
        Assert.Equal(12, geoset.Uvs.Length);
    }

    [Fact]
    public void An_ascii_fbx_renamed_to_dat_is_detected_by_its_header()
    {
        var path = Write("lied.dat", FbxFixture());
        var r = ModelImportCommand.ExecutePath(path, Path.Combine(_dir, "fbx-renamed.mdx"));
        Assert.Equal(6, r.Vertices);
        Assert.Equal(2, r.Faces);
    }

    [Fact]
    public void An_ascii_fbx_model_translation_moves_the_vertices()
    {
        var path = Write("move.fbx", FbxFixture(
            "            Properties70:  {\n"
            + "                P: \"Lcl Translation\", \"Lcl Translation\", \"\", \"A\",10,20,30\n"
            + "            }\n"));
        var outPath = Path.Combine(_dir, "fbx-move.mdx");
        ModelImportCommand.ExecutePath(path, outPath);

        var vertices = ModelParser.Parse(File.ReadAllBytes(outPath), "fbx.mdx").Geosets[0].Vertices;
        Assert.Equal(10f, vertices[0], 4);
        Assert.Equal(20f, vertices[1], 4);
        Assert.Equal(30f, vertices[2], 4);
    }

    [Fact]
    public void An_ascii_fbx_model_rotation_turns_the_winding()
    {
        var path = Write("turn.fbx", FbxFixture(
            "            Properties70:  {\n"
            + "                P: \"Lcl Rotation\", \"Lcl Rotation\", \"\", \"A\",0,0,90\n"
            + "            }\n"));
        var outPath = Path.Combine(_dir, "fbx-turn.mdx");
        ModelImportCommand.ExecutePath(path, outPath);

        // Vertex 1 was (1,0,0), a 90 degree turn about Z sends it to (0,1,0).
        var vertices = ModelParser.Parse(File.ReadAllBytes(outPath), "fbx.mdx").Geosets[0].Vertices;
        Assert.Equal(0f, vertices[3], 4);
        Assert.Equal(1f, vertices[4], 4);
        Assert.Equal(0f, vertices[5], 4);
    }

    [Fact]
    public void A_binary_fbx_reports_that_only_ascii_is_supported()
    {
        var bytes = new byte[64];
        Encoding.ASCII.GetBytes("Kaydara FBX Binary  ").CopyTo(bytes, 0);
        var path = Write("bin.fbx", bytes);
        var ex = Assert.Throws<NotSupportedException>(
            () => ModelImportCommand.ExecutePath(path, Path.Combine(_dir, "fbx-bin.mdx")));
        Assert.Contains("ASCII", ex.Message);
    }

    /// <summary>
    /// A tiny hermetic ASCII FBX, two triangles that share no vertices, with a per-corner normal
    /// layer and a UV layer that indexes a pool. <paramref name="modelBody"/> is inserted inside the
    /// Model node, so a test can add a Properties70 transform.
    /// </summary>
    private static string FbxFixture(string modelBody = "")
    {
        const string template = """
            ; FBX 7.4.0 project file
            ; Created by wc3ctl test
            ;------------------------------------------------------------------

            FBXHeaderExtension:  {
                FBXHeaderVersion: 1003
                FBXVersion: 7400
            }

            Objects:  {
                Geometry: 1000, "Geometry::tri", "Mesh" {
                    Vertices: *18 {
                        a: 0,0,0,1,0,0,0,1,0,2,0,0,3,0,0,2,1,0
                    }
                    PolygonVertexIndex: *6 {
                        a: 0,1,-3,3,4,-6
                    }
                    LayerElementNormal: 0 {
                        MappingInformationType: "ByPolygonVertex"
                        ReferenceInformationType: "Direct"
                        Normals: *18 {
                            a: 0,0,1,0,0,1,0,0,1,0,0,1,0,0,1,0,0,1
                        }
                    }
                    LayerElementUV: 0 {
                        MappingInformationType: "ByPolygonVertex"
                        ReferenceInformationType: "IndexToDirect"
                        UV: *8 {
                            a: 0,0,1,0,0,1,1,1
                        }
                        UVIndex: *6 {
                            a: 0,1,2,3,1,2
                        }
                    }
                }
                Model: 2000, "Model::tri", "Mesh" {
                    Version: 232
            __MODEL_BODY__
                }
            }

            Connections:  {
                C: "OO",1000,2000
                C: "OO",2000,0
            }
            """;
        return template.Replace("__MODEL_BODY__", modelBody);
    }

    // Position (3 x VEC3 float), normal (3 x VEC3 float), uv (3 x VEC2 float), indices (3 x u16).
    private static byte[] TriangleBuffer()
    {
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        foreach (var v in new[] { (0f, 0f, 0f), (1f, 0f, 0f), (0f, 1f, 0f) })
            { w.Write(v.Item1); w.Write(v.Item2); w.Write(v.Item3); }
        for (int i = 0; i < 3; i++) { w.Write(0f); w.Write(0f); w.Write(1f); }
        foreach (var uv in new[] { (0f, 0f), (1f, 0f), (0f, 1f) }) { w.Write(uv.Item1); w.Write(uv.Item2); }
        w.Write((ushort)0); w.Write((ushort)1); w.Write((ushort)2);
        w.Flush();
        return ms.ToArray();
    }

    private static string TriangleGltfJson()
    {
        var buffer = TriangleBuffer();
        var b64 = Convert.ToBase64String(buffer);
        return $$"""
        {
          "asset": { "version": "2.0" },
          "buffers": [ { "byteLength": {{buffer.Length}}, "uri": "data:application/octet-stream;base64,{{b64}}" } ],
          "bufferViews": [
            { "buffer": 0, "byteOffset": 0, "byteLength": 36, "byteStride": 12 },
            { "buffer": 0, "byteOffset": 36, "byteLength": 36 },
            { "buffer": 0, "byteOffset": 72, "byteLength": 24 },
            { "buffer": 0, "byteOffset": 96, "byteLength": 6 }
          ],
          "accessors": [
            { "bufferView": 0, "componentType": 5126, "count": 3, "type": "VEC3" },
            { "bufferView": 1, "componentType": 5126, "count": 3, "type": "VEC3" },
            { "bufferView": 2, "componentType": 5126, "count": 3, "type": "VEC2" },
            { "bufferView": 3, "componentType": 5123, "count": 3, "type": "SCALAR" }
          ],
          "meshes": [ { "primitives": [ {
            "attributes": { "POSITION": 0, "NORMAL": 1, "TEXCOORD_0": 2 },
            "indices": 3, "mode": 4
          } ] } ]
        }
        """;
    }

    private static byte[] BuildGlb(string json, byte[] bin)
    {
        var jsonBytes = Pad(Encoding.UTF8.GetBytes(json), 0x20);
        var binBytes = Pad(bin, 0x00);
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        int total = 12 + 8 + jsonBytes.Length + 8 + binBytes.Length;
        w.Write("glTF"u8);
        w.Write(2u);
        w.Write((uint)total);
        w.Write((uint)jsonBytes.Length);
        w.Write(0x4E4F534Au); // "JSON"
        w.Write(jsonBytes);
        w.Write((uint)binBytes.Length);
        w.Write(0x004E4942u); // "BIN\0"
        w.Write(binBytes);
        w.Flush();
        return ms.ToArray();
    }

    private static byte[] Pad(byte[] data, byte fill)
    {
        int padded = (data.Length + 3) & ~3;
        if (padded == data.Length) return data;
        var result = new byte[padded];
        data.CopyTo(result, 0);
        for (int i = data.Length; i < padded; i++) result[i] = fill;
        return result;
    }

    private static byte[] BuildBinaryStl()
    {
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        w.Write(new byte[80]);   // header
        w.Write(1u);             // triangle count
        w.Write(0f); w.Write(0f); w.Write(1f); // facet normal
        foreach (var v in new[] { (0f, 0f, 0f), (1f, 0f, 0f), (0f, 1f, 0f) })
            { w.Write(v.Item1); w.Write(v.Item2); w.Write(v.Item3); }
        w.Write((ushort)0);
        w.Flush();
        return ms.ToArray();
    }

    private static byte[] BuildBinaryPly()
    {
        const string header = "ply\n"
            + "format binary_little_endian 1.0\n"
            + "element vertex 3\n"
            + "property float x\nproperty float y\nproperty float z\n"
            + "element face 1\n"
            + "property list uchar int vertex_indices\n"
            + "end_header\n";
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        w.Write(Encoding.ASCII.GetBytes(header));
        foreach (var v in new[] { (0f, 0f, 0f), (1f, 0f, 0f), (0f, 1f, 0f) })
            { w.Write(v.Item1); w.Write(v.Item2); w.Write(v.Item3); }
        w.Write((byte)3);        // list count
        w.Write(0); w.Write(1); w.Write(2);
        w.Flush();
        return ms.ToArray();
    }
}
