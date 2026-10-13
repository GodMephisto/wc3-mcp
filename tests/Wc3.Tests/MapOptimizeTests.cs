// tests/Wc3.Tests/MapOptimizeTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;
using Wc3.Modeling;

namespace Wc3.Tests;

/// <summary>
/// Hermetic tests for the batch map optimiser, built on a synthetic map with one decimatable model,
/// one shrinkable texture and one untouched file. No corpus and no WC3 install.
/// </summary>
public sealed class MapOptimizeTests
{
    private static readonly byte[] UntouchedScript =
        Encoding.ASCII.GetBytes("function main takes nothing returns nothing\nendfunction\n");

    private static byte[] SyntheticMapBytes() => SyntheticMap.Build(new Dictionary<string, byte[]>
    {
        ["war3map.j"] = UntouchedScript,
        [@"war3mapImported\Grid.mdx"] = SyntheticModel.GridMdx(9),
        [@"war3mapImported\Tex.blp"] = SyntheticModel.Blp(64, 64),
    });

    [Fact]
    public void Optimize_shrinks_the_map_and_leaves_other_files_byte_identical()
    {
        var mapPath = Path.Combine(Path.GetTempPath(), $"wc3opt-{Guid.NewGuid():N}.w3x");
        var outPath = Path.Combine(Path.GetTempPath(), $"wc3opt-out-{Guid.NewGuid():N}.w3x");
        File.WriteAllBytes(mapPath, SyntheticMapBytes());
        var inputBefore = File.ReadAllBytes(mapPath);
        try
        {
            var r = MapOptimizeCommand.ExecutePath(mapPath, 0.5f, 32, outPath);

            // The input map is never modified in place.
            Assert.Equal(inputBefore, File.ReadAllBytes(mapPath));
            Assert.True(File.Exists(outPath));

            // The output map is smaller, and both the model and the texture were replaced.
            Assert.True(new FileInfo(outPath).Length < new FileInfo(mapPath).Length,
                $"output {new FileInfo(outPath).Length} not smaller than input {new FileInfo(mapPath).Length}");
            Assert.Equal(1, r.ModelsOptimized);
            Assert.Equal(1, r.TexturesOptimized);
            Assert.Equal(2, r.Changed);
            Assert.True(r.BytesSaved > 0);

            var outDoc = MapDocument.Load(outPath);

            // The optimised model still parses and still has geometry.
            var modelEntry = outDoc.GetFile(@"war3mapImported\Grid.mdx");
            Assert.NotNull(modelEntry);
            var model = ModelParser.Parse(modelEntry!.CurrentBytes, @"war3mapImported\Grid.mdx");
            Assert.NotEmpty(model.Geosets);
            Assert.True(model.Geosets[0].Indices.Length > 0);

            // The texture was scaled to the limit.
            var texEntry = outDoc.GetFile(@"war3mapImported\Tex.blp");
            Assert.NotNull(texEntry);
            var (w, h) = Wc3.Render.TextureConvert.Dimensions(texEntry!.CurrentBytes, ".blp");
            Assert.True(w <= 32 && h <= 32, $"texture stayed {w}x{h}");

            // An untouched file is byte for byte identical.
            var script = outDoc.GetFile("war3map.j");
            Assert.NotNull(script);
            Assert.Equal(UntouchedScript, script!.CurrentBytes);
        }
        finally
        {
            File.Delete(mapPath);
            File.Delete(outPath);
        }
    }

    [Fact]
    public void Ratio_one_and_an_in_range_texture_change_nothing()
    {
        var mapPath = Path.Combine(Path.GetTempPath(), $"wc3opt-{Guid.NewGuid():N}.w3x");
        var outPath = Path.Combine(Path.GetTempPath(), $"wc3opt-out-{Guid.NewGuid():N}.w3x");
        File.WriteAllBytes(mapPath, SyntheticMapBytes());
        try
        {
            // ratio 1 keeps every face, and a maximum of 1024 is above the 64px texture, so neither
            // file is replaced and the report is empty.
            var r = MapOptimizeCommand.ExecutePath(mapPath, 1.0f, 1024, outPath);

            Assert.Equal(0, r.ModelsOptimized);
            Assert.Equal(0, r.TexturesOptimized);
            Assert.Empty(r.Files);

            var outDoc = MapDocument.Load(outPath);
            Assert.Equal(UntouchedScript, outDoc.GetFile("war3map.j")!.CurrentBytes);
        }
        finally
        {
            File.Delete(mapPath);
            File.Delete(outPath);
        }
    }

    [Theory]
    [InlineData(0f)]
    [InlineData(1.5f)]
    public void An_out_of_range_ratio_is_refused(float ratio)
        => Assert.Throws<ArgumentOutOfRangeException>(
            () => MapOptimizeCommand.Execute(MapDocument.Load(SyntheticMapBytes()), ratio, 256));

    [Fact]
    public void A_non_positive_texture_side_is_refused()
        => Assert.Throws<ArgumentOutOfRangeException>(
            () => MapOptimizeCommand.Execute(MapDocument.Load(SyntheticMapBytes()), 0.5f, 0));
}
