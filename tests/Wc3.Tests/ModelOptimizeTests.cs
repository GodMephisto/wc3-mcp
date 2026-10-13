// tests/Wc3.Tests/ModelOptimizeTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Modeling;

namespace Wc3.Tests;

public sealed class ModelOptimizeTests
{
    /// <summary>A v800 MDX with one geoset holding an n by n grid, delegated to the shared builder.</summary>
    private static byte[] GridMdx(int n, bool degenerate = false) => SyntheticModel.GridMdx(n, degenerate);

    /// <summary>A v800 MDX with two identical TEXS entries and one material referencing the second.</summary>
    private static byte[] DupTextureMdx()
    {
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        w.Write("MDLX"u8);
        w.Write("VERS"u8); w.Write(4u); w.Write(800u);
        w.Write("TEXS"u8); w.Write(536u);
        for (int t = 0; t < 2; t++)
        {
            w.Write(0u);
            var path = Encoding.ASCII.GetBytes(@"Textures\Test.blp");
            w.Write(path); w.Write(new byte[260 - path.Length]); w.Write(0u);
        }
        const uint layerSize = 28u;
        uint matSize = 4 + 4 + 4 + 8 + layerSize;
        w.Write("MTLS"u8); w.Write(matSize); w.Write(matSize); w.Write(0u); w.Write(0u);
        w.Write("LAYS"u8); w.Write(1u); w.Write(layerSize);
        w.Write(0u); w.Write(0u); w.Write(1u); w.Write(0xFFFFFFFFu); w.Write(0u); w.Write(1.0f);
        w.Flush();
        return ms.ToArray();
    }

    [Fact]
    public void Duplicate_textures_are_pruned()
    {
        var bytes = DupTextureMdx();
        ModelOptimizeCommand.Execute(bytes, "dup.mdx", 1.0f, out var output);
        Assert.True(output.Length < bytes.Length);
        var doc = MdxDocument.Parse(output);
        Assert.Equal(268, doc.Chunks.First(c => c.Tag == "TEXS").Payload.Length); // one entry left
    }

    [Fact]
    public void Ratio_one_leaves_the_model_unchanged()
    {
        var bytes = SyntheticModel.GridMdx(9);
        ModelOptimizeCommand.Execute(bytes, "grid.mdx", 1.0f, out var output);
        Assert.Equal(bytes, output);
    }

    [Fact]
    public void Decimation_shrinks_the_geoset_and_stays_parseable()
    {
        var bytes = SyntheticModel.GridMdx(9);
        var r = ModelOptimizeCommand.Execute(bytes, "grid.mdx", 0.5f, out var output);
        Assert.Equal(1, r.Geosets);
        Assert.True(r.FacesAfter < r.FacesBefore);
        Assert.True(r.VerticesAfter < r.VerticesBefore);
        Assert.True(output.Length <= bytes.Length);

        var model = ModelParser.Parse(output, "grid.mdx");
        Assert.NotEmpty(model.Geosets);
        Assert.True(model.Geosets[0].Indices.Length > 0);

        // UVs must survive decimation and stay sized to the new vertex count. The rewritten UVAS
        // must keep its per-set UVBS tag; dropping it makes the reader stop at the count and the
        // model loses every UV, which a textured render then trips over. This pins that contract.
        var g = model.Geosets[0];
        Assert.True(g.Uvs.Length > 0, "UVs were dropped by decimation");
        Assert.Equal(g.Vertices.Length / 3, g.Uvs.Length / 2);
    }

    [Fact]
    public void A_zero_area_face_is_compacted_away()
    {
        var bytes = SyntheticModel.GridMdx(5, degenerate: true);
        var r = ModelOptimizeCommand.Execute(bytes, "grid.mdx", 1.0f, out var output);
        Assert.Equal(r.FacesBefore - 1, r.FacesAfter); // the collinear triangle is dropped
        var model = ModelParser.Parse(output, "grid.mdx");
        Assert.NotEmpty(model.Geosets);
    }

    [Fact]
    public void Effects_are_kept_when_stripping_is_off()
    {
        var bytes = SyntheticModel.BuildMdx(extraChunks: new[] { ("PREM", new byte[8]) });
        var r = ModelOptimizeCommand.Execute(bytes, "fx.mdx", 1.0f, out var output, stripEffects: false);

        Assert.Equal(0, r.EffectsRemoved);
        var parsed = MdxDocument.Parse(output);
        Assert.Equal(1, parsed.CountEffects().ParticleEmitters);
        Assert.Equal(bytes, output); // nothing reduced, nothing dropped, byte for byte

        var model = ModelParser.Parse(output, "fx.mdx");
        Assert.NotEmpty(model.Geosets);
    }

    [Fact]
    public void Effects_are_dropped_when_stripping_is_on_and_output_still_parses()
    {
        var bytes = SyntheticModel.BuildMdx(extraChunks: new[] { ("PREM", new byte[8]) });
        var r = ModelOptimizeCommand.Execute(bytes, "fx.mdx", 1.0f, out var output, stripEffects: true);

        Assert.Equal(1, r.EffectsRemoved);
        Assert.True(output.Length < bytes.Length);
        Assert.Equal(0, MdxDocument.Parse(output).CountEffects().ParticleEmitters);

        var model = ModelParser.Parse(output, "fx.mdx");
        Assert.NotEmpty(model.Geosets);
        Assert.True(model.Geosets[0].Indices.Length > 0);
    }
}
