// tests/Wc3.Tests/ModelImportTests.cs
using Wc3.Commands;
using Wc3.Modeling;

namespace Wc3.Tests;

public sealed class ModelImportTests
{
    private const string QuadWithNormals = """
        # a unit quad in the XY plane
        v 0 0 0
        v 1 0 0
        v 1 1 0
        v 0 1 0
        vt 0 0
        vt 1 0
        vt 1 1
        vt 0 1
        vn 0 0 1
        f 1/1/1 2/2/1 3/3/1 4/4/1
        """;

    private const string TriangleNoNormals = """
        v 0 0 0
        v 1 0 0
        v 0 1 0
        vt 0 0
        vt 1 0
        vt 0 1
        f 1/1 2/2 3/3
        """;

    [Fact]
    public void Quad_with_normals_imports_to_a_parsable_mdx()
    {
        var r = ModelImportCommand.Execute(QuadWithNormals, "quad.obj", out var mdx);

        Assert.Equal(1, r.Geosets);
        Assert.Equal(4, r.Vertices);
        Assert.Equal(2, r.Faces); // the n-gon fan split the quad into two triangles
        Assert.Equal(1, r.Textures);
        Assert.Equal(mdx.LongLength, r.Bytes);

        var model = ModelParser.Parse(mdx, "quad.mdx");
        var geoset = Assert.Single(model.Geosets);
        Assert.Equal(4, geoset.Vertices.Length / 3);
        Assert.Equal(6, geoset.Indices.Length); // two triangles
        Assert.Equal(4, geoset.Uvs.Length / 2);
        Assert.Equal(4, geoset.Normals.Length / 3);
        Assert.Equal(0, geoset.TextureId);
        Assert.Single(model.Textures);
    }

    [Fact]
    public void A_face_without_normals_gets_a_computed_face_normal()
    {
        var r = ModelImportCommand.Execute(TriangleNoNormals, "tri.obj", out var mdx);

        Assert.Equal(3, r.Vertices);
        Assert.Equal(1, r.Faces);

        var model = ModelParser.Parse(mdx, "tri.mdx");
        var geoset = Assert.Single(model.Geosets);
        for (int v = 0; v < 3; v++)
        {
            Assert.Equal(0f, geoset.Normals[v * 3], 5);
            Assert.Equal(0f, geoset.Normals[v * 3 + 1], 5);
            Assert.Equal(1f, geoset.Normals[v * 3 + 2], 5); // CCW in XY faces +Z
        }
    }

    [Fact]
    public void An_ngon_fans_into_n_minus_two_triangles()
    {
        const string pentagon = """
            v 0 0 0
            v 1 0 0
            v 2 1 0
            v 1 2 0
            v 0 1 0
            f 1 2 3 4 5
            """;
        var r = ModelImportCommand.Execute(pentagon, "pent.obj", out var mdx);

        Assert.Equal(5, r.Vertices);
        Assert.Equal(3, r.Faces);
        var model = ModelParser.Parse(mdx, "pent.mdx");
        Assert.Equal(9, model.Geosets[0].Indices.Length);
    }

    [Fact]
    public void Negative_and_v_slash_vn_face_forms_resolve()
    {
        const string obj = """
            v 0 0 0
            v 1 0 0
            v 0 1 0
            vn 0 0 1
            f -3//1 -2//1 -1//1
            """;
        var r = ModelImportCommand.Execute(obj, "neg.obj", out var mdx);

        Assert.Equal(3, r.Vertices);
        Assert.Equal(1, r.Faces);
        var model = ModelParser.Parse(mdx, "neg.mdx");
        Assert.Equal(3, model.Geosets[0].Indices.Length);
    }

    [Fact]
    public void An_export_then_import_round_trips_the_counts()
    {
        var original = ModelParser.Parse(SyntheticModel.TinyMdx(), "tri.mdx");
        var obj = ObjExporter.ToObj(original, null);

        var r = ModelImportCommand.Execute(obj, "tri.obj", out var mdx);
        var model = ModelParser.Parse(mdx, "tri.mdx");

        var before = original.Geosets[0];
        var after = Assert.Single(model.Geosets);
        Assert.Equal(before.Vertices.Length / 3, after.Vertices.Length / 3);
        Assert.Equal(before.Indices.Length / 3, after.Indices.Length / 3);
        Assert.Equal(before.Vertices.Length / 3, r.Vertices);
        Assert.Equal(before.Indices.Length / 3, r.Faces);
    }

    [Fact]
    public void An_obj_with_no_faces_is_refused()
    {
        const string empty = """
            v 0 0 0
            v 1 0 0
            """;
        Assert.Throws<ArgumentException>(
            () => ModelImportCommand.Execute(empty, "empty.obj", out _));
    }

    [Fact]
    public void An_out_of_range_face_index_is_refused()
    {
        const string bad = """
            v 0 0 0
            v 1 0 0
            v 0 1 0
            f 1 2 9
            """;
        Assert.Throws<ArgumentException>(
            () => ModelImportCommand.Execute(bad, "bad.obj", out _));
    }
}
