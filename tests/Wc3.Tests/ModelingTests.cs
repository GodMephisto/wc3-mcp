// tests/Wc3.Tests/ModelingTests.cs
using System.Text;
using Wc3.Modeling;

namespace Wc3.Tests;

/// <summary>Hermetic tests for the MDL/MDX parsers and the ModelParser dispatcher.</summary>
public class ModelingTests
{
    private const string TinyMdl = """
        // Magos-style MDL snippet: one textured triangle.
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

    [Fact]
    public void Mdl_text_parses_single_triangle_geoset()
    {
        var model = ModelParser.Parse(Encoding.UTF8.GetBytes(TinyMdl), "tri.mdl");

        Assert.Single(model.Textures);
        Assert.Equal(@"Textures\Test.blp", model.Textures[0]);

        var g = Assert.Single(model.Geosets);
        Assert.Equal(new float[] { 0, 0, 0, 1, 0, 0, 0, 1, 0 }, g.Vertices);
        Assert.Equal(new float[] { 0, 0, 1, 0, 0, 1, 0, 0, 1 }, g.Normals);
        Assert.Equal(new float[] { 0, 0, 1, 0, 0, 1 }, g.Uvs);
        Assert.Equal(new[] { 0, 1, 2 }, g.Indices);
        Assert.Equal(0, g.TextureId);
    }

    [Fact]
    public void Mdx_binary_parses_single_triangle_geoset()
    {
        var model = ModelParser.Parse(BuildTinyMdx(), "tri.mdx");

        Assert.Single(model.Textures);
        Assert.Equal(@"Textures\Test.blp", model.Textures[0]);

        var g = Assert.Single(model.Geosets);
        Assert.Equal(new float[] { 0, 0, 0, 1, 0, 0, 0, 1, 0 }, g.Vertices);
        Assert.Equal(new float[] { 0, 0, 1, 0, 0, 1, 0, 0, 1 }, g.Normals);
        Assert.Equal(new float[] { 0, 0, 1, 0, 0, 1 }, g.Uvs);
        Assert.Equal(new[] { 0, 1, 2 }, g.Indices);
        Assert.Equal(0, g.TextureId);
    }

    [Fact]
    public void Mdx_extension_without_magic_is_rejected()
    {
        var ex = Assert.Throws<InvalidDataException>(
            () => ModelParser.Parse(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, "junk.mdx"));
        Assert.Contains("MDLX", ex.Message);
    }

    [Fact]
    public void Unknown_extension_is_rejected()
    {
        Assert.Throws<NotSupportedException>(
            () => ModelParser.Parse(new byte[] { 1, 2, 3, 4 }, "model.obj"));
    }

    /// <summary>Builds a minimal v800 MDLX: VERS + TEXS + MTLS + GEOS with one triangle.</summary>
    private static byte[] BuildTinyMdx()
    {
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        w.Write("MDLX"u8);

        // VERS
        w.Write("VERS"u8);
        w.Write(4u);
        w.Write(800u);

        // TEXS: one 268-byte entry
        w.Write("TEXS"u8);
        w.Write(268u);
        w.Write(0u); // replaceableId
        var path = Encoding.ASCII.GetBytes(@"Textures\Test.blp");
        w.Write(path);
        w.Write(new byte[260 - path.Length]);
        w.Write(0u); // flags

        // MTLS: one material, one layer (layer = 28 bytes incl. its size field)
        var layerSize = 28u;
        var matSize = 4 + 4 + 4 + 8 + layerSize; // inclusiveSize, priorityPlane, flags, LAYS+count, layer
        w.Write("MTLS"u8);
        w.Write(matSize);
        w.Write(matSize);        // material inclusiveSize
        w.Write(0u);             // priorityPlane
        w.Write(0u);             // flags
        w.Write("LAYS"u8);
        w.Write(1u);             // layer count
        w.Write(layerSize);      // layer inclusiveSize
        w.Write(0u);             // filterMode
        w.Write(0u);             // shadingFlags
        w.Write(0u);             // textureId
        w.Write(0xFFFFFFFFu);    // textureAnimationId (none)
        w.Write(0u);             // coordId
        w.Write(1.0f);           // alpha

        // GEOS: one geoset built in its own buffer so the inclusive size is exact.
        using var gs = new MemoryStream();
        using var gw = new BinaryWriter(gs);
        gw.Write("VRTX"u8);
        gw.Write(3u);
        foreach (var f in new float[] { 0, 0, 0, 1, 0, 0, 0, 1, 0 }) gw.Write(f);
        gw.Write("NRMS"u8);
        gw.Write(3u);
        foreach (var f in new float[] { 0, 0, 1, 0, 0, 1, 0, 0, 1 }) gw.Write(f);
        gw.Write("PTYP"u8);
        gw.Write(1u);
        gw.Write(4u); // triangles
        gw.Write("PCNT"u8);
        gw.Write(1u);
        gw.Write(3u);
        gw.Write("PVTX"u8);
        gw.Write(3u);
        gw.Write((ushort)0);
        gw.Write((ushort)1);
        gw.Write((ushort)2);
        gw.Write("GNDX"u8);
        gw.Write(3u);
        gw.Write(new byte[] { 0, 0, 0 });
        gw.Write("MTGC"u8);
        gw.Write(1u);
        gw.Write(1u);
        gw.Write("MATS"u8);
        gw.Write(1u);
        gw.Write(0u);
        gw.Write(0u);            // materialId
        gw.Write(0u);            // selectionGroup
        gw.Write(0u);            // selectionFlags
        foreach (var f in new float[] { 2, -1, -1, 0, 1, 1, 0 }) gw.Write(f); // radius + min + max
        gw.Write(0u);            // per-sequence extent count
        gw.Write("UVAS"u8);
        gw.Write(1u);            // UV set count
        gw.Write("UVBS"u8);
        gw.Write(3u);
        foreach (var f in new float[] { 0, 0, 1, 0, 0, 1 }) gw.Write(f);
        gw.Flush();

        var geoset = gs.ToArray();
        w.Write("GEOS"u8);
        w.Write((uint)(geoset.Length + 4));
        w.Write((uint)(geoset.Length + 4)); // geoset inclusiveSize
        w.Write(geoset);

        w.Flush();
        return ms.ToArray();
    }
}
