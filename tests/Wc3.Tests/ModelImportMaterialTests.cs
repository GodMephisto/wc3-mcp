// tests/Wc3.Tests/ModelImportMaterialTests.cs
using System.Buffers.Binary;
using System.Text;
using Wc3.Commands;
using Wc3.Modeling;
using Wc3.Render;

namespace Wc3.Tests;

/// <summary>
/// The glTF material and texture path. A glTF with two primitives and two materials carrying
/// embedded base64 PNG textures must import to two geosets and two TEXS entries, and the .blp files
/// written beside the MDX must decode. A model with no materials keeps the legacy single default
/// texture, and an image that will not decode leaves its material with no texture and a diagnostic
/// rather than failing the import. Everything is built in-memory, so there is no corpus or network.
/// </summary>
public sealed class ModelImportMaterialTests : IDisposable
{
    private readonly string _dir = Path.Combine(
        Path.GetTempPath(), "wc3ctl-import-mat", Guid.NewGuid().ToString("N"));

    public ModelImportMaterialTests() => Directory.CreateDirectory(_dir);

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch { /* best effort */ }
    }

    [Fact]
    public void Two_primitives_and_two_materials_import_to_two_geosets_and_two_textures()
    {
        var gltf = TwoPrimitiveGltf(Png(16, 16, 255, 0, 0), Png(16, 16, 0, 0, 255));
        var input = Write("two.gltf", gltf);
        var outPath = Path.Combine(_dir, "two.mdx");

        var r = ModelImportCommand.ExecutePath(input, outPath);

        Assert.Equal(2, r.Geosets);
        Assert.Equal(2, r.Materials);
        Assert.Equal(2, r.Textures);
        Assert.Equal(6, r.Vertices); // three per triangle primitive
        Assert.Equal(2, r.Faces);
        Assert.Empty(r.Diagnostics);
        Assert.Equal(2, r.TextureFiles.Count);
        Assert.Contains("Red.blp", r.TextureFiles);
        Assert.Contains("Blue.blp", r.TextureFiles);

        var model = ModelParser.Parse(File.ReadAllBytes(outPath), "two.mdx");
        Assert.Equal(2, model.Geosets.Count);
        Assert.Equal(2, model.Textures.Count);
        Assert.Equal(new[] { 0, 1 }, model.Geosets.Select(g => g.TextureId).OrderBy(i => i).ToArray());
        Assert.Contains(model.Textures, t => t.EndsWith("Red.blp", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(model.Textures, t => t.EndsWith("Blue.blp", StringComparison.OrdinalIgnoreCase));

        foreach (var file in r.TextureFiles)
        {
            var path = Path.Combine(_dir, file);
            Assert.True(File.Exists(path), $"expected the written texture {path}");
            var tex = BlpDecoder.Decode(File.ReadAllBytes(path));
            Assert.Equal(16, tex.Width);
            Assert.Equal(16, tex.Height);
        }
    }

    [Fact]
    public void A_gltf_without_materials_keeps_the_single_default_texture()
    {
        var gltf = TwoPrimitiveGltf(Png(16, 16, 255, 0, 0), Png(16, 16, 0, 0, 255), withMaterials: false);
        var input = Write("plain.gltf", gltf);
        var outPath = Path.Combine(_dir, "plain.mdx");

        var r = ModelImportCommand.ExecutePath(input, outPath);

        Assert.Equal(2, r.Geosets);   // one geoset per primitive
        Assert.Equal(1, r.Textures);  // one shared default texture
        Assert.Equal(2, r.Materials);
        Assert.Empty(r.TextureFiles); // the default texture is named, not written

        var model = ModelParser.Parse(File.ReadAllBytes(outPath), "plain.mdx");
        Assert.Equal(2, model.Geosets.Count);
        Assert.Single(model.Textures);
        Assert.All(model.Geosets, g => Assert.Equal(0, g.TextureId));
    }

    [Fact]
    public void An_image_that_will_not_decode_keeps_the_material_without_a_texture()
    {
        var garbage = Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
        var gltf = TwoPrimitiveGltf(garbage, garbage, withMaterials: true, mimeSubtype: "png", singlePrimitive: true);
        var input = Write("broken.gltf", gltf);
        var outPath = Path.Combine(_dir, "broken.mdx");

        var r = ModelImportCommand.ExecutePath(input, outPath);

        Assert.Equal(1, r.Geosets);
        Assert.Equal(1, r.Materials);
        Assert.Equal(0, r.Textures);
        Assert.NotEmpty(r.Diagnostics);

        var model = ModelParser.Parse(File.ReadAllBytes(outPath), "broken.mdx");
        var geoset = Assert.Single(model.Geosets);
        Assert.Equal(-1, geoset.TextureId);
    }

    private string Write(string name, string text)
    {
        var path = Path.Combine(_dir, name);
        File.WriteAllText(path, text);
        return path;
    }

    private static string Png(int w, int h, byte r, byte g, byte b)
    {
        var rgba = new byte[w * h * 4];
        for (int i = 0; i < rgba.Length; i += 4)
            (rgba[i], rgba[i + 1], rgba[i + 2], rgba[i + 3]) = (r, g, b, 255);
        return Convert.ToBase64String(TexturePng.Encode(new TextureImage(w, h, rgba)));
    }

    private static byte[] TwoPrimitiveBuffer()
    {
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        foreach (var v in new[] { (0f, 0f, 0f), (1f, 0f, 0f), (0f, 1f, 0f) })
            { w.Write(v.Item1); w.Write(v.Item2); w.Write(v.Item3); }
        w.Write((ushort)0); w.Write((ushort)1); w.Write((ushort)2);
        w.Write((ushort)0); // pad to a 4-byte aligned offset for the next float array
        foreach (var v in new[] { (0f, 0f, 1f), (1f, 0f, 1f), (0f, 1f, 1f) })
            { w.Write(v.Item1); w.Write(v.Item2); w.Write(v.Item3); }
        w.Write((ushort)0); w.Write((ushort)1); w.Write((ushort)2);
        w.Flush();
        return ms.ToArray();
    }

    private static string TwoPrimitiveGltf(string redBase64, string blueBase64,
        bool withMaterials = true, string mimeSubtype = "png", bool singlePrimitive = false)
    {
        var buffer = TwoPrimitiveBuffer();
        string buffer64 = Convert.ToBase64String(buffer);
        int primitiveCount = singlePrimitive ? 1 : 2;

        var primitives = new StringBuilder();
        for (int i = 0; i < primitiveCount; i++)
        {
            if (i > 0) primitives.Append(',');
            primitives.Append("{\"attributes\":{\"POSITION\":").Append(i * 2)
                .Append("},\"indices\":").Append(i * 2 + 1).Append(",\"mode\":4");
            if (withMaterials) primitives.Append(",\"material\":").Append(i);
            primitives.Append('}');
        }

        var materials = withMaterials
            ? ",\"materials\":["
              + "{\"pbrMetallicRoughness\":{\"baseColorTexture\":{\"index\":0}}},"
              + "{\"pbrMetallicRoughness\":{\"baseColorTexture\":{\"index\":1}}}],"
              + "\"textures\":[{\"source\":0},{\"source\":1}]"
            : "";

        return $$"""
        {
          "asset": { "version": "2.0" },
          "buffers": [ { "byteLength": {{buffer.Length}}, "uri": "data:application/octet-stream;base64,{{buffer64}}" } ],
          "bufferViews": [
            { "buffer": 0, "byteOffset": 0, "byteLength": 36 },
            { "buffer": 0, "byteOffset": 36, "byteLength": 6 },
            { "buffer": 0, "byteOffset": 44, "byteLength": 36 },
            { "buffer": 0, "byteOffset": 80, "byteLength": 6 }
          ],
          "accessors": [
            { "bufferView": 0, "componentType": 5126, "count": 3, "type": "VEC3" },
            { "bufferView": 1, "componentType": 5123, "count": 3, "type": "SCALAR" },
            { "bufferView": 2, "componentType": 5126, "count": 3, "type": "VEC3" },
            { "bufferView": 3, "componentType": 5123, "count": 3, "type": "SCALAR" }
          ],
          "images": [
            { "name": "Red", "uri": "data:image/{{mimeSubtype}};base64,{{redBase64}}" },
            { "name": "Blue", "uri": "data:image/{{mimeSubtype}};base64,{{blueBase64}}" }
          ]{{materials}},
          "meshes": [ { "primitives": [ {{primitives}} ] } ]
        }
        """;
    }
}
