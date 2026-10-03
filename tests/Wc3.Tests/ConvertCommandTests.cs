// tests/Wc3.Tests/ConvertCommandTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;
using Wc3.Modeling;
using Wc3.Render;

namespace Wc3.Tests;

public class ConvertCommandTests
{
    /// <summary>Magos-style MDL: one textured triangle referencing Textures\Test.blp.</summary>
    private const string TriangleMdl = """
        Version {
            FormatVersion 800,
        }
        Model "Tri" {
            BlendTime 150,
        }
        Textures 1 {
            Bitmap {
                Image "Textures\Test.blp",
            },
        }
        Materials 1 {
            Material {
                Layer {
                    FilterMode None,
                    static TextureID 0,
                },
            },
        }
        Geoset {
            Vertices 3 {
                { 0, 0, 0 },
                { 1, 0, 0 },
                { 0, 1, 0 },
            },
            Normals 3 {
                { 0, 0, 1 },
                { 0, 0, 1 },
                { 0, 0, 1 },
            },
            TVertices 3 {
                { 0, 0 },
                { 1, 0 },
                { 0, 1 },
            },
            VertexGroup {
                0,
                0,
                0,
            },
            Faces 1 3 {
                Triangles {
                    { 0, 1, 2 },
                },
            },
            Groups 1 1 {
                Matrices { 0 },
            },
            MinimumExtent { -1, -1, 0 },
            MaximumExtent { 1, 1, 0 },
            BoundsRadius 2,
            MaterialID 0,
            SelectionGroup 0,
        }
        """;

    private static byte[] SolidBlp(int w, int h)
    {
        var rgba = new byte[w * h * 4];
        for (int i = 0; i < rgba.Length; i += 4)
        {
            rgba[i] = 200; rgba[i + 1] = 60; rgba[i + 2] = 20; rgba[i + 3] = 255;
        }
        var png = TexturePng.Encode(new TextureImage(w, h, rgba));
        return TextureConvert.Convert(png, ".png", ".blp");
    }

    private static MapDocument MapWithModel() => MapDocument.Load(SyntheticMap.Build(
        new Dictionary<string, byte[]>
        {
            [@"war3mapImported\tri.mdl"] = Encoding.UTF8.GetBytes(TriangleMdl),
            [@"Textures\Test.blp"] = SolidBlp(16, 16),
        }));

    [Fact]
    public void Map_model_exports_obj_mtl_and_png_texture()
    {
        var export = ConvertCommand.ExportModelToObj(MapWithModel(), @"war3mapImported\tri.mdl");

        Assert.Contains("mtllib tri.mtl", export.Obj);
        Assert.Contains("v ", export.Obj);
        Assert.Contains("vt ", export.Obj);
        Assert.Contains("vn ", export.Obj);
        Assert.Contains("f 1/1/1 2/2/2 3/3/3", export.Obj);

        Assert.Contains("newmtl tex0", export.Mtl);
        Assert.Contains("map_Kd Test.png", export.Mtl);

        Assert.True(export.Textures.TryGetValue("Test.png", out var png), "Test.png not exported");
        Assert.Equal(new byte[] { 0x89, 0x50, 0x4E, 0x47 }, png![..4]);
    }

    [Fact]
    public void Map_model_resolves_the_mdx_mdl_extension_swap()
    {
        // Map stores tri.mdl; ask for tri.mdx like object data often does.
        var export = ConvertCommand.ExportModelToObj(MapWithModel(), @"war3mapImported\tri.mdx");
        Assert.Contains("f 1/1/1 2/2/2 3/3/3", export.Obj);
    }

    [Fact]
    public void Missing_model_throws_with_the_path_in_the_message()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(
            new Dictionary<string, byte[]> { ["war3map.j"] = new byte[] { 1 } }));
        var ex = Assert.Throws<InvalidDataException>(
            () => ConvertCommand.ExportModelToObj(doc, @"war3mapImported\nope.mdx"));
        Assert.Contains("nope.mdx", ex.Message);
    }

    [Fact]
    public void Unresolvable_texture_keeps_material_without_map_kd()
    {
        // Same model, but the map does not contain the texture.
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [@"war3mapImported\tri.mdl"] = Encoding.UTF8.GetBytes(TriangleMdl),
        }));
        var export = ConvertCommand.ExportModelToObj(doc, @"war3mapImported\tri.mdl");

        Assert.Contains("newmtl tex0", export.Mtl);
        Assert.DoesNotContain("map_Kd", export.Mtl);
        Assert.Empty(export.Textures);
    }

    [Fact]
    public void Standalone_model_bytes_reference_textures_but_export_none()
    {
        var export = ConvertCommand.ExportModelToObj(Encoding.UTF8.GetBytes(TriangleMdl), "tri.mdl");

        Assert.Contains("mtllib tri.mtl", export.Obj);
        Assert.Contains("map_Kd Test.png", export.Mtl); // basename reference for a later convert
        Assert.Empty(export.Textures);
    }

    [Fact]
    public void ConvertImageFile_converts_a_map_internal_blp_to_png()
    {
        var png = ConvertCommand.ConvertImageFile(MapWithModel(), @"Textures\Test.blp", ".png");
        Assert.Equal(new byte[] { 0x89, 0x50, 0x4E, 0x47 }, png[..4]);
    }

    [Fact]
    public void ConvertImageFile_missing_file_throws_file_not_found()
    {
        Assert.Throws<FileNotFoundException>(
            () => ConvertCommand.ConvertImageFile(MapWithModel(), "nope.blp", ".png"));
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Corpus_model_exports_valid_obj_with_bounded_indices()
    {
        string map = CorpusMap.PathOrEmpty;
        if (!File.Exists(map)) return; // corpus-optional

        var doc = MapDocument.Load(map);
        var model = CorpusSubject.LargestModelAsMdl(doc);
        if (model is null) return;   // no imported models in this map to export

        var export = ConvertCommand.ExportModelToObj(doc, model);
        var lines = export.Obj.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        int vertices = lines.Count(l => l.StartsWith("v "));
        Assert.True(vertices > 0, "no vertices exported");
        Assert.Contains(lines, l => l.StartsWith("vt "));
        Assert.Contains(lines, l => l.StartsWith("vn "));

        var faceIndices = lines.Where(l => l.StartsWith("f "))
            .SelectMany(l => l[2..].Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .SelectMany(v => v.Split('/'))
            .Select(int.Parse)
            .ToList();
        Assert.NotEmpty(faceIndices);
        Assert.All(faceIndices, i => Assert.InRange(i, 1, vertices));

        // Every exported texture is a PNG the MTL actually references.
        foreach (var (name, bytes) in export.Textures)
        {
            Assert.Equal(new byte[] { 0x89, 0x50, 0x4E, 0x47 }, bytes[..4]);
            Assert.Contains($"map_Kd {name}", export.Mtl);
        }
    }
}
