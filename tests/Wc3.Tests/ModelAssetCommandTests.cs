// tests/Wc3.Tests/ModelAssetCommandTests.cs
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Import;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Hermetic coverage for the model and batch asset commands. Every map is built in memory and
/// every model is a small synthetic MDX, so no WC3 install and no corpus are needed.
/// </summary>
public sealed class ModelAssetCommandTests : IDisposable
{
    private readonly string _dir = Path.Combine(
        Path.GetTempPath(), "wc3-model-assets", Guid.NewGuid().ToString("N"));

    public ModelAssetCommandTests() => Directory.CreateDirectory(_dir);

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch (IOException) { }
    }

    // ---- asset model-lint ----------------------------------------------------

    [Fact]
    public void Model_lint_reports_a_geoset_with_no_material()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [@"war3mapImported\NoMat.mdx"] = SyntheticModel.BuildMdx(includeMaterial: false),
        }));

        var r = AssetModelLintCommand.Execute(doc);

        Assert.Equal(1, r.ModelsScanned);
        Assert.Contains(r.Findings, f => f.Kind == ModelLintKind.GeosetWithoutMaterial
            && f.Path == @"war3mapImported\NoMat.mdx");
    }

    [Fact]
    public void Model_lint_reports_a_material_naming_a_missing_texture()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [@"war3mapImported\MissTex.mdx"] = SyntheticModel.BuildMdx(texturePath: @"Textures\Missing.blp"),
        }));

        var r = AssetModelLintCommand.Execute(doc);

        var finding = Assert.Single(r.Findings, f => f.Kind == ModelLintKind.MissingTexture);
        Assert.Contains(@"Textures\Missing.blp", finding.Detail);
        Assert.DoesNotContain(r.Findings, f => f.Kind == ModelLintKind.GeosetWithoutMaterial);
    }

    [Fact]
    public void Model_lint_reports_a_resolved_texture_as_clean()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [@"war3mapImported\Ok.mdx"] = SyntheticModel.BuildMdx(texturePath: @"Textures\Test.blp"),
            [@"Textures\Test.blp"] = SyntheticModel.Blp(),
        }));

        var r = AssetModelLintCommand.Execute(doc);

        Assert.Empty(r.Findings);
    }

    [Fact]
    public void Model_lint_reports_a_zero_duration_sequence()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [@"war3mapImported\ZeroSeq.mdx"] = SyntheticModel.BuildMdx(
                sequences: new[] { ("Stand", 0, 0) }, includeBone: true),
        }));

        var r = AssetModelLintCommand.Execute(doc);

        Assert.Contains(r.Findings, f => f.Kind == ModelLintKind.ZeroDurationSequence
            && f.Path == @"war3mapImported\ZeroSeq.mdx");
    }

    [Fact]
    public void Model_lint_reports_an_out_of_range_bounding_box()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [@"war3mapImported\Big.mdx"] = SyntheticModel.BuildMdx(
                vertices: new float[] { 0, 0, 0, 1_000_000, 0, 0, 0, 1, 0 }),
        }));

        var r = AssetModelLintCommand.Execute(doc);

        Assert.Contains(r.Findings, f => f.Kind == ModelLintKind.OutOfRangeBounds
            && f.Path == @"war3mapImported\Big.mdx");
    }

    // ---- asset thumbnails ----------------------------------------------------

    [Fact]
    public void Thumbnails_render_one_png_per_model_file()
    {
        File.WriteAllBytes(Path.Combine(_dir, "Tri.mdx"), SyntheticModel.TinyMdx());
        File.WriteAllBytes(Path.Combine(_dir, "Tri.blp"), SyntheticModel.Blp());

        var r = AssetThumbnailsCommand.Execute(_dir);

        var thumb = Assert.Single(r.Thumbnails);
        Assert.Equal("Tri.png", thumb.Name);
        Assert.True(thumb.Png.Length > 8);
        Assert.Equal(0x89, thumb.Png[0]);
        Assert.Equal((byte)'P', thumb.Png[1]);
    }

    // ---- asset convert-batch -------------------------------------------------

    [Fact]
    public void Convert_batch_converts_a_folder_of_textures()
    {
        File.WriteAllBytes(Path.Combine(_dir, "Tex.blp"), SyntheticModel.Blp());

        var r = AssetConvertBatchCommand.Execute(_dir, ".png");

        var converted = Assert.Single(r.Textures);
        Assert.Equal("Tex.png", converted.RelativePath);
        Assert.Equal(".png", r.ToExtension);
        Assert.Equal(0x89, converted.Bytes[0]);
        Assert.Equal((byte)'P', converted.Bytes[1]);
    }

    // ---- asset import-wire ---------------------------------------------------

    [Fact]
    public void Import_wire_adds_a_file_and_points_a_unit_field_at_it()
    {
        var modelFile = Path.Combine(_dir, "Hero.mdx");
        File.WriteAllBytes(modelFile, SyntheticModel.TinyMdx());

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Serialize(w => w.Write(CustomUnit("H000"))),
        }));

        var r = AssetImportWireCommand.Execute(doc, new[] { modelFile }, "H000", ObjectKind.Unit);

        Assert.True(r.Ok, r.Message);
        Assert.Contains(@"war3mapImported\Hero.mdx", r.Imported);
        Assert.Contains(@"umdl=war3mapImported\Hero.mdx", r.FieldsSet);

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        var fields = ObjectKinds.ModsToDict(
            ObjectKinds.MergedEntries(reloaded, ObjectKinds.Info(ObjectKind.Unit)).Single().Mods);
        Assert.Equal(@"war3mapImported\Hero.mdx", fields["umdl"]);

        var imports = (ImportedFiles)reloaded.GetFile("war3map.imp")!.Model!;
        Assert.Contains(imports.Files, f => f.FullPath == @"war3mapImported\Hero.mdx");
    }

    [Fact]
    public void Import_wire_imports_without_an_object()
    {
        var modelFile = Path.Combine(_dir, "Loose.mdx");
        File.WriteAllBytes(modelFile, SyntheticModel.TinyMdx());

        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Serialize(w => w.Write(CustomUnit("H000"))),
        }));

        var r = AssetImportWireCommand.Execute(doc, new[] { modelFile });

        Assert.True(r.Ok, r.Message);
        Assert.Empty(r.FieldsSet);
        Assert.Contains(@"war3mapImported\Loose.mdx", r.Imported);
    }

    // ---- model export-obj ----------------------------------------------------

    [Fact]
    public void Export_obj_returns_obj_mtl_and_converted_textures()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [@"war3mapImported\Tri.mdx"] = SyntheticModel.BuildMdx(texturePath: @"Textures\Tri.blp"),
            [@"Textures\Tri.blp"] = SyntheticModel.Blp(),
        }));

        var export = ModelExportObjCommand.Execute(doc, @"war3mapImported\Tri.mdx");

        Assert.Contains("v ", export.Obj);
        Assert.Contains("usemtl tex0", export.Obj);
        Assert.Contains("mtllib Tri.mtl", export.Obj);
        Assert.Contains("newmtl tex0", export.Mtl);
        Assert.True(export.Textures.ContainsKey("Tri.png"));
    }

    [Fact]
    public void Export_obj_references_the_mtl_name_the_caller_asks_for()
    {
        // A caller names the MTL from its own output path, so the pair sits at "<out>.obj" and
        // "<out>.mtl", which need not match the model's own name. Without threading the name
        // through, the OBJ pointed at "<model basename>.mtl" and no renderer found the materials.
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [@"war3mapImported\Tri.mdx"] = SyntheticModel.BuildMdx(texturePath: @"Textures\Tri.blp"),
            [@"Textures\Tri.blp"] = SyntheticModel.Blp(),
        }));

        var export = ModelExportObjCommand.Execute(doc, @"war3mapImported\Tri.mdx", "custom-out");

        Assert.Contains("mtllib custom-out.mtl", export.Obj);
    }

    [Fact]
    public void Export_obj_publishes_every_file_and_leaves_no_staging_folder()
    {
        var doc = TriMap();
        string objPath = Path.Combine(_dir, "Tri.obj");

        var r = ModelExportObjCommand.Write(doc, @"war3mapImported\Tri.mdx", objPath);

        Assert.Equal(objPath, r.SavedTo);
        Assert.Equal(Path.Combine(_dir, "Tri.mtl"), r.Mtl);
        Assert.Equal(new[] { "Tri.png" }, r.Textures);
        foreach (string path in new[] { r.SavedTo, r.Mtl }
                     .Concat(r.Textures.Select(t => Path.Combine(_dir, t))))
        {
            Assert.True(File.Exists(path), path);
            Assert.True(new FileInfo(path).Length > 0, path);
        }
        Assert.Contains("mtllib Tri.mtl", File.ReadAllText(r.SavedTo));
        Assert.Empty(r.Diagnostics);
        Assert.Empty(Staging());
        Assert.Empty(Directory.GetFiles(_dir, "*" + ".replaced-" + "*"));
    }

    [Fact]
    public void Export_obj_refuses_an_existing_target_and_leaves_it_unchanged()
    {
        var doc = TriMap();
        string objPath = Path.Combine(_dir, "Tri.obj");
        byte[] original = Encoding.UTF8.GetBytes("previous export");
        File.WriteAllBytes(objPath, original);

        var ex = Assert.Throws<IOException>(() =>
            ModelExportObjCommand.Write(doc, @"war3mapImported\Tri.mdx", objPath));

        Assert.Contains("overwrite", ex.Message);
        Assert.Equal(original, File.ReadAllBytes(objPath));
        Assert.False(File.Exists(Path.Combine(_dir, "Tri.mtl")));
        Assert.False(File.Exists(Path.Combine(_dir, "Tri.png")));
        Assert.Empty(Staging());
    }

    [Fact]
    public void Export_obj_overwrite_replaces_the_target_and_lists_the_new_files()
    {
        var doc = TriMap();
        string objPath = Path.Combine(_dir, "Tri.obj");
        string mtlPath = Path.Combine(_dir, "Tri.mtl");
        File.WriteAllText(objPath, "previous export");
        File.WriteAllText(mtlPath, "previous materials");

        var r = ModelExportObjCommand.Write(doc, @"war3mapImported\Tri.mdx", objPath, overwrite: true);

        Assert.Equal(new[] { "Tri.png" }, r.Textures);
        Assert.Contains("mtllib Tri.mtl", File.ReadAllText(objPath));
        Assert.Contains("newmtl tex0", File.ReadAllText(mtlPath));
        Assert.True(new FileInfo(Path.Combine(_dir, "Tri.png")).Length > 0);
        Assert.Empty(Staging());
        Assert.Empty(Directory.GetFiles(_dir, "*" + ".replaced-" + "*"));
    }

    [Fact]
    public void Export_obj_restores_the_previous_target_when_the_publish_fails_halfway()
    {
        var doc = TriMap();
        string objPath = Path.Combine(_dir, "Tri.obj");
        string mtlPath = Path.Combine(_dir, "Tri.mtl");
        byte[] obj = Encoding.UTF8.GetBytes("previous export");
        byte[] mtl = Encoding.UTF8.GetBytes("previous materials");
        File.WriteAllBytes(objPath, obj);
        File.WriteAllBytes(mtlPath, mtl);

        // A folder squatting on a texture name is an obstruction the export cannot replace, so the
        // texture move fails after the obj and the mtl have already been swapped in. The rollback
        // has to put both originals back byte for byte.
        Directory.CreateDirectory(Path.Combine(_dir, "Tri.png"));

        Assert.Throws<IOException>(() =>
            ModelExportObjCommand.Write(doc, @"war3mapImported\Tri.mdx", objPath, overwrite: true));

        Assert.Equal(obj, File.ReadAllBytes(objPath));
        Assert.Equal(mtl, File.ReadAllBytes(mtlPath));
        Assert.True(Directory.Exists(Path.Combine(_dir, "Tri.png")));
        Assert.Empty(Staging());
        Assert.Empty(Directory.GetFiles(_dir, "*" + ".replaced-" + "*"));
    }

    [Fact]
    public void Export_obj_leaves_no_staging_and_no_target_when_the_parent_is_a_file()
    {
        var doc = TriMap();
        string blocker = Path.Combine(_dir, "blocker");
        File.WriteAllText(blocker, "not a folder");
        string objPath = Path.Combine(blocker, "Tri.obj");

        Assert.Throws<IOException>(() =>
            ModelExportObjCommand.Write(doc, @"war3mapImported\Tri.mdx", objPath));

        Assert.Equal("not a folder", File.ReadAllText(blocker));
        Assert.False(File.Exists(objPath));
        Assert.Empty(Staging());
        Assert.Equal(new[] { blocker }, Directory.GetFileSystemEntries(_dir));
    }

    // ---- fixtures ------------------------------------------------------------

    private static MapDocument TriMap() => MapDocument.Load(SyntheticMap.Build(
        new Dictionary<string, byte[]>
        {
            [@"war3mapImported\Tri.mdx"] = SyntheticModel.BuildMdx(texturePath: @"Textures\Tri.blp"),
            [@"Textures\Tri.blp"] = SyntheticModel.Blp(),
        }));

    /// <summary>Every leftover staging folder under the test's own root, which must be none.</summary>
    private IEnumerable<string> Staging() => Directory.GetDirectories(
        _dir, "*" + ModelExportObjCommand.StagingInfix + "*", SearchOption.AllDirectories);

    private static UnitObjectData CustomUnit(string rawcode)
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var unit = new SimpleObjectModification { OldId = "hfoo".FromRawcode(), NewId = rawcode.FromRawcode() };
        unit.Modifications.Add(new SimpleObjectDataModification
        { Id = "unam".FromRawcode(), Type = ObjectDataType.String, Value = "Custom" });
        w3u.NewUnits.Add(unit);
        return w3u;
    }

    private static byte[] Serialize(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }
}
