// tests/Wc3.Tests/ObjExporterTests.cs
using Wc3.Modeling;

namespace Wc3.Tests;

public class ObjExporterTests
{
    /// <summary>One triangle at z=0, normals +Z, UVs at the corners.</summary>
    private static Geoset Triangle(int textureId) => new(
        Vertices: new float[] { 0, 0, 0, 1, 0, 0, 0, 1, 0 },
        Normals: new float[] { 0, 0, 1, 0, 0, 1, 0, 0, 1 },
        Uvs: new float[] { 0, 0, 1, 0, 0, 1 },
        Indices: new[] { 0, 1, 2 },
        TextureId: textureId);

    private static string[] Lines(string text) =>
        text.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(l => l.TrimEnd('\r')).ToArray();

    [Fact]
    public void Single_geoset_emits_v_vt_vn_f_and_material_references()
    {
        var model = new Model3D(new[] { Triangle(0) }, new[] { @"Textures\Test.blp" });
        var obj = ObjExporter.ToObj(model, "tri.mtl");
        var lines = Lines(obj);

        Assert.Contains("mtllib tri.mtl", lines);
        Assert.Contains("o geoset0", lines);
        Assert.Contains("usemtl tex0", lines);
        Assert.Equal(3, lines.Count(l => l.StartsWith("v ")));
        Assert.Equal(3, lines.Count(l => l.StartsWith("vn ")));
        Assert.Equal(3, lines.Count(l => l.StartsWith("vt ")));
        Assert.Contains("f 1/1/1 2/2/2 3/3/3", lines);
    }

    [Fact]
    public void Uv_v_coordinate_is_flipped_for_obj()
    {
        var model = new Model3D(new[] { Triangle(0) }, new[] { "t.blp" });
        var lines = Lines(ObjExporter.ToObj(model, null));

        // Input UVs: (0,0), (1,0), (0,1) → OBJ vt with V flipped: (0,1), (1,1), (0,0).
        var vt = lines.Where(l => l.StartsWith("vt ")).ToArray();
        Assert.Equal(new[] { "vt 0 1", "vt 1 1", "vt 0 0" }, vt);
    }

    [Fact]
    public void Without_mtl_no_mtllib_or_usemtl_lines_appear()
    {
        var model = new Model3D(new[] { Triangle(0) }, new[] { "t.blp" });
        var obj = ObjExporter.ToObj(model, null);
        Assert.DoesNotContain("mtllib", obj);
        Assert.DoesNotContain("usemtl", obj);
    }

    [Fact]
    public void Multiple_geosets_use_a_global_one_based_vertex_index()
    {
        var model = new Model3D(new[] { Triangle(0), Triangle(1) }, new[] { "a.blp", "b.blp" });
        var lines = Lines(ObjExporter.ToObj(model, "m.mtl"));

        Assert.Contains("o geoset0", lines);
        Assert.Contains("o geoset1", lines);
        Assert.Contains("usemtl tex1", lines);
        Assert.Contains("f 1/1/1 2/2/2 3/3/3", lines);
        Assert.Contains("f 4/4/4 5/5/5 6/6/6", lines); // offset by geoset0's 3 vertices

        // Every face index stays within the global vertex count.
        int totalVertices = lines.Count(l => l.StartsWith("v "));
        var indices = lines.Where(l => l.StartsWith("f "))
            .SelectMany(l => l[2..].Split(' '))
            .SelectMany(v => v.Split('/'))
            .Select(int.Parse);
        Assert.All(indices, i => Assert.InRange(i, 1, totalVertices));
    }

    [Fact]
    public void Unresolved_texture_id_gets_no_usemtl_and_no_material()
    {
        var model = new Model3D(new[] { Triangle(-1) }, Array.Empty<string>());
        var obj = ObjExporter.ToObj(model, "m.mtl");
        Assert.DoesNotContain("usemtl", obj);
        Assert.DoesNotContain("newmtl", ObjExporter.ToMtl(model, new Dictionary<int, string>()));
    }

    [Fact]
    public void Mtl_lists_each_distinct_texture_id_once_with_map_kd_when_provided()
    {
        var model = new Model3D(
            new[] { Triangle(0), Triangle(1), Triangle(0) },
            new[] { "a.blp", "b.blp" });
        var mtl = ObjExporter.ToMtl(model, new Dictionary<int, string> { [0] = "a.png" });
        var lines = Lines(mtl);

        Assert.Single(lines, l => l == "newmtl tex0");
        Assert.Single(lines, l => l == "newmtl tex1");
        Assert.Contains("map_Kd a.png", lines);
        Assert.Single(lines, l => l.StartsWith("map_Kd")); // tex1 has no file → no map_Kd
    }

    [Fact]
    public void Floats_are_invariant_culture_fixed_point()
    {
        var g = new Geoset(
            Vertices: new float[] { 1.5f, -2.25f, 0.000001f },
            Normals: new float[] { 0, 0, 1 },
            Uvs: new float[] { 0.5f, 0.5f },
            Indices: new[] { 0, 0, 0 },
            TextureId: -1);
        var obj = ObjExporter.ToObj(new Model3D(new[] { g }, Array.Empty<string>()), null);
        Assert.Contains("v 1.5 -2.25 0.000001", obj);
        Assert.DoesNotContain("E-", obj); // no scientific notation
        Assert.DoesNotContain(",", obj.Replace(", 1 unit", "")); // no comma decimal separators (header aside)
    }
}
