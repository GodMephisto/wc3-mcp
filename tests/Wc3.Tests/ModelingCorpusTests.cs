// tests/Wc3.Tests/ModelingCorpusTests.cs
using Wc3.Model;
using Wc3.Modeling;

namespace Wc3.Tests;

/// <summary>
/// Real-data coverage: .mdx models pulled straight out of the Anime corpus map
/// and .blp textures from the extracted samples directory. Both self-skip when
/// the local data is absent.
/// </summary>
public class ModelingCorpusTests
{
    private static readonly string AnimeMap =
        TestCorpus.Map(@"Anime_WOS2_0.25c1.w3x");

    private static readonly string BlpSamplesDir =
        System.IO.Path.Combine(System.IO.Path.GetTempPath(), @"wc3x_modelsamples\blp");

    [Fact]
    [Trait("Category", "Corpus")]
    public void Anime_map_mdx_models_parse_to_renderable_geometry()
    {
        if (!File.Exists(AnimeMap)) return; // corpus-optional

        var doc = MapDocument.Load(AnimeMap);
        var mdxFiles = doc.Files
            .Where(f => f.FileName?.EndsWith(".mdx", StringComparison.OrdinalIgnoreCase) == true)
            .Take(10)
            .ToList();
        Assert.NotEmpty(mdxFiles);

        int parsed = 0, totalVertices = 0, totalTriangles = 0;
        foreach (var file in mdxFiles)
        {
            var model = ModelParser.Parse(file.RawBytes, file.FileName!);
            if (model.Geosets.Count == 0) continue; // some models are emitter-only

            parsed++;
            foreach (var g in model.Geosets)
            {
                Assert.True(g.Vertices.Length % 3 == 0, $"{file.FileName}: vertex floats not xyz-aligned");
                Assert.True(g.Indices.Length % 3 == 0, $"{file.FileName}: indices not triangles");
                Assert.Equal(g.Vertices.Length, g.Normals.Length);
                Assert.Equal(g.Vertices.Length / 3 * 2, g.Uvs.Length);
                foreach (var index in g.Indices)
                    Assert.InRange(index, 0, g.Vertices.Length / 3 - 1);
                Assert.InRange(g.TextureId, -1, model.Textures.Count - 1);
                totalVertices += g.Vertices.Length / 3;
                totalTriangles += g.Indices.Length / 3;
            }
        }

        Assert.True(parsed > 0, "no .mdx with geometry parsed");
        Assert.True(totalVertices > 0);
        Assert.True(totalTriangles > 0);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Real_blp_samples_decode_to_rgba()
    {
        if (!Directory.Exists(BlpSamplesDir)) return; // samples-optional

        var samples = Directory.EnumerateFiles(BlpSamplesDir, "*.blp", SearchOption.AllDirectories)
            .Take(25)
            .ToList();
        Assert.NotEmpty(samples);

        foreach (var path in samples)
        {
            var image = BlpDecoder.Decode(File.ReadAllBytes(path));
            Assert.True(image.Width > 0, $"{path}: zero width");
            Assert.True(image.Height > 0, $"{path}: zero height");
            Assert.Equal(image.Width * image.Height * 4, image.Rgba.Length);
        }
    }
}
