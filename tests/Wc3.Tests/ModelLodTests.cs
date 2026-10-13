// tests/Wc3.Tests/ModelLodTests.cs
using Wc3.Commands;
using Wc3.Modeling;

namespace Wc3.Tests;

/// <summary>
/// The level-of-detail chain, hermetic. A grid model is decimated to three ratios and each output
/// is checked for descending face counts and a clean parse, and ratio 1 is checked byte identical
/// to the input. No corpus and no WC3 install.
/// </summary>
public sealed class ModelLodTests
{
    private static string TempDir()
    {
        var dir = Path.Combine(Path.GetTempPath(), "wc3lod-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        return dir;
    }

    [Fact]
    public void Three_levels_descend_in_faces_and_each_parses()
    {
        var dir = TempDir();
        var input = Path.Combine(dir, "grid.mdx");
        File.WriteAllBytes(input, SyntheticModel.GridMdx(9));
        var outDir = Path.Combine(dir, "lod");

        var r = ModelLodCommand.ExecutePath(input, "1,0.5,0.25", outDir);

        Assert.Equal(3, r.Levels.Count);
        Assert.True(r.Levels[0].Faces > r.Levels[1].Faces);
        Assert.True(r.Levels[1].Faces > r.Levels[2].Faces);

        foreach (var level in r.Levels)
        {
            Assert.True(File.Exists(level.Path), $"missing level file {level.Path}");
            var model = ModelParser.Parse(File.ReadAllBytes(level.Path), Path.GetFileName(level.Path));
            Assert.NotEmpty(model.Geosets);
            Assert.True(model.Geosets[0].Indices.Length > 0);
        }

        Assert.Equal(r.Levels.Sum(l => l.Bytes), r.TotalBytes);
        Assert.Equal(3, Directory.GetFiles(outDir, "*.mdx").Length);
    }

    [Fact]
    public void Ratio_one_is_byte_identical_to_the_input()
    {
        var dir = TempDir();
        var input = Path.Combine(dir, "grid.mdx");
        var bytes = SyntheticModel.GridMdx(9);
        File.WriteAllBytes(input, bytes);
        var outDir = Path.Combine(dir, "lod");

        var r = ModelLodCommand.ExecutePath(input, "1,0.5", outDir);

        Assert.Equal(1f, r.Levels[0].Ratio);
        Assert.Equal(bytes, File.ReadAllBytes(r.Levels[0].Path));
    }

    [Fact]
    public void Ratios_are_parsed_and_validated()
    {
        Assert.Equal(new[] { 1f, 0.5f, 0.25f }, ModelLodCommand.ParseRatios("1,0.5,0.25"));
        Assert.Equal(new[] { 0.5f }, ModelLodCommand.ParseRatios(" 0.5 "));
        Assert.Throws<ArgumentException>(() => ModelLodCommand.ParseRatios("1.5"));
        Assert.Throws<ArgumentException>(() => ModelLodCommand.ParseRatios("0"));
        Assert.Throws<ArgumentException>(() => ModelLodCommand.ParseRatios("banana"));
        Assert.Throws<ArgumentException>(() => ModelLodCommand.ParseRatios(""));
        Assert.Throws<ArgumentException>(() => ModelLodCommand.ParseRatios("   "));
    }
}
