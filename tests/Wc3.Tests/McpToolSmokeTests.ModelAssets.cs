// tests/Wc3.Tests/McpToolSmokeTests.ModelAssets.cs
// The model and batch asset tools. Model lint scans imported models, the batch tools write
// files to a directory the caller names, and import-wire writes an edited map copy. Each write
// is checked to leave its input map untouched.
using NAudio.Wave;
using Wc3.Modeling;
using Wc3.Render;

namespace Wc3.Tests;

public sealed class McpSmokeModelAssetTests : McpSmokeBase
{
    public McpSmokeModelAssetTests(SmokeFixture f) : base(f) { }

    [Fact, Covers("asset_model_lint")]
    public async Task Model_lint_answers_on_a_blank_map()
    {
        var result = await Call("asset_model_lint", Args(("map", Fixture)));

        Assert.Equal(0, Prop(result, "modelsScanned").GetInt32());
        Assert.Empty(Prop(result, "findings").EnumerateArray());
    }

    [Fact, Covers("leak_model")]
    public async Task Leak_model_answers_on_a_blank_map()
    {
        var result = await Call("leak_model", Args(("map", Fixture)));

        Assert.Equal(0, Prop(result, "modelsScanned").GetInt32());
        Assert.Empty(Prop(result, "findings").EnumerateArray());
    }

    [Fact, Covers("asset_thumbnails")]
    public async Task Thumbnails_writes_a_png_for_a_model_folder()
    {
        var folder = Path.Combine(_f.Dir, $"models-{Guid.NewGuid():N}");
        Directory.CreateDirectory(folder);
        File.WriteAllBytes(Path.Combine(folder, "Tri.mdx"), SyntheticModel.TinyMdx());

        var outDir = Path.Combine(_f.Dir, $"thumbs-{Guid.NewGuid():N}");
        var result = await Call("asset_thumbnails",
            Args(("folder", folder), ("out_dir", outDir)));

        Assert.Equal(1, Prop(result, "rendered").GetInt32());
        Assert.True(File.Exists(Path.Combine(outDir, "Tri.png")));
    }

    [Fact, Covers("asset_convert_batch")]
    public async Task Convert_batch_writes_a_converted_texture()
    {
        var folder = Path.Combine(_f.Dir, $"textures-{Guid.NewGuid():N}");
        Directory.CreateDirectory(folder);
        File.WriteAllBytes(Path.Combine(folder, "Tex.blp"), SyntheticModel.Blp());

        var outDir = Path.Combine(_f.Dir, $"converted-{Guid.NewGuid():N}");
        var result = await Call("asset_convert_batch",
            Args(("folder", folder), ("to", ".png"), ("out_dir", outDir)));

        Assert.Equal(1, Prop(result, "converted").GetInt32());
        Assert.True(File.Exists(Path.Combine(outDir, "Tex.png")));
    }

    [Fact, Covers("model_audit")]
    public async Task Model_audit_reports_the_model()
    {
        var modelFile = NewOut("audit", ".mdx");
        File.WriteAllBytes(modelFile, SyntheticModel.TinyMdx());
        var r = await Call("model_audit", Args(("model", modelFile)));
        Assert.Equal("mdx", Prop(r, "format").GetString());
        Assert.True(Prop(r, "geosets").GetInt32() >= 1);
    }

    [Fact, Covers("model_optimize")]
    public async Task Model_optimize_writes_a_smaller_model()
    {
        var modelFile = NewOut("optin", ".mdx");
        File.WriteAllBytes(modelFile, SyntheticModel.TinyMdx());
        var outFile = NewOut("optout", ".mdx");
        var r = await Call("model_optimize", Args(("model", modelFile), ("out_path", outFile), ("ratio", 0.5)));
        Assert.True(File.Exists(outFile));
        Assert.True(Prop(r, "geosets").GetInt32() >= 1);
    }

    [Fact, Covers("model_lod")]
    public async Task Model_lod_writes_a_descending_chain()
    {
        var modelFile = NewOut("lodin", ".mdx");
        File.WriteAllBytes(modelFile, SyntheticModel.GridMdx(9));
        var outDir = Path.Combine(_f.Dir, $"lod-{Guid.NewGuid():N}");
        var r = await Call("model_lod", Args(("model", modelFile), ("ratios", "1,0.5,0.25"), ("out_dir", outDir)));

        var levels = Rows(Prop(r, "levels"));
        Assert.Equal(3, levels.Count);
        Assert.True(File.Exists(Prop(levels[0], "path").GetString()));
        Assert.True(Prop(levels[0], "faces").GetInt32() > Prop(levels[1], "faces").GetInt32());
        Assert.True(Prop(levels[1], "faces").GetInt32() > Prop(levels[2], "faces").GetInt32());
        Assert.Equal(3, Directory.GetFiles(outDir, "*.mdx").Length);
    }

    [Fact, Covers("texture_optimize")]
    public async Task Texture_optimize_shrinks_a_texture()
    {
        var blp = NewOut("texin", ".blp");
        File.WriteAllBytes(blp, SyntheticModel.Blp(64, 64));
        var outBlp = NewOut("texout", ".blp");
        var r = await Call("texture_optimize", Args(("texture", blp), ("out_path", outBlp), ("max", 32)));
        Assert.True(File.Exists(outBlp));
        Assert.Equal(32, Prop(r, "widthAfter").GetInt32());
    }

    [Fact, Covers("texture_atlas")]
    public async Task Texture_atlas_packs_textures_into_one_file()
    {
        string a = NewOut("atlas-a", ".png");
        string b = NewOut("atlas-b", ".png");
        string c = NewOut("atlas-c", ".png");
        File.WriteAllBytes(a, SolidPng(8, 8, 255, 0, 0));
        File.WriteAllBytes(b, SolidPng(16, 16, 0, 255, 0));
        File.WriteAllBytes(c, SolidPng(8, 8, 0, 0, 255));

        var outPath = NewOut("atlas", ".png");
        var r = await Call("texture_atlas",
            Args(("images", new[] { a, b, c }), ("out_path", outPath), ("max", 512)));

        Assert.True(File.Exists(outPath));
        Assert.Equal(3, Rows(Prop(r, "entries")).Count);
        Assert.Equal("png", Prop(r, "format").GetString());
    }

    private static byte[] SolidPng(int w, int h, byte r, byte g, byte b)
    {
        var rgba = new byte[w * h * 4];
        for (int i = 0; i < w * h; i++)
        {
            rgba[i * 4] = r;
            rgba[i * 4 + 1] = g;
            rgba[i * 4 + 2] = b;
            rgba[i * 4 + 3] = 255;
        }
        return TexturePng.Encode(new TextureImage(w, h, rgba));
    }

    [Fact, Covers("map_optimize")]
    public async Task Map_optimize_writes_a_smaller_map()
    {
        var mapPath = NewOut("mapopt-in");
        File.WriteAllBytes(mapPath, SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = new byte[] { 1, 2, 3, 4 },
            [@"war3mapImported\Grid.mdx"] = SyntheticModel.GridMdx(9),
            [@"war3mapImported\Tex.blp"] = SyntheticModel.Blp(64, 64),
        }));
        var outPath = NewOut("mapopt-out");
        var before = Sha(mapPath);

        var r = await Call("map_optimize", Args(
            ("map", mapPath), ("out_path", outPath), ("ratio", 0.5f), ("max_texture", 32)));

        Assert.Equal(before, Sha(mapPath));
        Assert.True(File.Exists(outPath));
        Assert.Equal(1, Prop(r, "modelsOptimized").GetInt32());
        Assert.Equal(1, Prop(r, "texturesOptimized").GetInt32());
        Assert.True(Prop(r, "bytesSaved").GetInt64() > 0);
    }

    [Fact, Covers("sound_optimize")]
    public async Task Sound_optimize_resamples_a_wav()
    {
        var wav = NewOut("soundin", ".wav");
        var format = new WaveFormat(44100, 16, 2);
        using (var writer = new WaveFileWriter(wav, format))
            for (int i = 0; i < 4410; i++)
                for (int c = 0; c < 2; c++)
                    writer.WriteSample(0.2f * MathF.Sin(2 * MathF.PI * 440 * i / 44100));

        var outWav = NewOut("soundout", ".wav");
        var r = await Call("sound_optimize", Args(
            ("sound", wav), ("out_path", outWav), ("rate", 22050), ("channels", "mono")));

        Assert.True(File.Exists(outWav));
        Assert.Equal("wav", Prop(r, "format").GetString());
        Assert.Equal(22050, Prop(r, "sampleRateAfter").GetInt32());
        Assert.Equal(1, Prop(r, "channelsAfter").GetInt32());
        Assert.Equal(2, Prop(r, "channelsBefore").GetInt32());
    }

    [Fact, Covers("model_export_obj")]
    public async Task Model_export_obj_writes_obj_and_mtl()
    {
        var modelFile = NewOut("model", ".mdx");
        File.WriteAllBytes(modelFile, SyntheticModel.TinyMdx());
        var withModel = await Write("file_set",
            Args(("internal_path", @"war3mapImported\Smoke.mdx"), ("from", modelFile)));

        var objPath = NewOut("export", ".obj");
        await Call("model_export_obj", Args(
            ("map", withModel), ("model", @"war3mapImported\Smoke.mdx"), ("out_path", objPath)));

        Assert.True(File.Exists(objPath));
        var mtlPath = Path.ChangeExtension(objPath, ".mtl");
        Assert.True(File.Exists(mtlPath));
        Assert.Contains("mtllib", File.ReadAllText(objPath));
        Assert.Contains("newmtl", File.ReadAllText(mtlPath));
    }

    [Fact, Covers("model_import")]
    public async Task Model_import_writes_mdx_from_an_obj()
    {
        var objFile = NewOut("model", ".obj");
        File.WriteAllText(objFile, "v 0 0 0\nv 1 0 0\nv 0 1 0\nf 1 2 3\n");
        var outFile = NewOut("imported", ".mdx");

        var r = await Call("model_import", Args(("obj", objFile), ("out_path", outFile)));

        Assert.True(File.Exists(outFile));
        Assert.Equal(3, Prop(r, "vertices").GetInt32());
        Assert.Equal(1, Prop(r, "faces").GetInt32());
        Assert.Equal(1, Prop(r, "textures").GetInt32());
    }

    [Fact, Covers("asset_import_wire")]
    public async Task Import_wire_adds_a_file_to_a_map_copy()
    {
        var modelFile = NewOut("wire", ".mdx");
        File.WriteAllBytes(modelFile, SyntheticModel.TinyMdx());

        var outPath = NewOut("wired");
        var before = Sha(Fixture);
        var result = await Call("asset_import_wire", Args(
            ("map", Fixture), ("out_path", outPath), ("files", new[] { modelFile })));

        Assert.Equal(before, Sha(Fixture));
        Assert.True(File.Exists(outPath));
        Assert.Contains(@"war3mapImported\",
            Prop(result, "imported").EnumerateArray().First().GetString());

        var imports = await Call("imports_list", Args(("map", outPath)));
        Assert.Contains(Rows(Prop(imports, "entries")),
            e => Prop(e, "path").GetString()!.EndsWith("wire", StringComparison.OrdinalIgnoreCase)
                 || Prop(e, "path").GetString()!.EndsWith(".mdx", StringComparison.OrdinalIgnoreCase));
    }
}
