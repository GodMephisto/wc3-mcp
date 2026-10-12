// tests/Wc3.Tests/TerrainSmoothCommandTests.cs
using War3Net.Build.Environment; // MapEnvironment
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// Whole-map smoothing. One pass averages each corner with its in-grid four neighbours,
/// and repeating it converges a noisy field. These pin the one-pass kernel, that more
/// passes keep going, and the guards.
/// </summary>
public class TerrainSmoothCommandTests
{
    private const int TileEdge = 8;
    private const int W = TileEdge + 1;
    private static int Idx(int x, int y) => y * W + x;

    private static MapDocument Blank() => BlankMap.Create(new BlankMapOptions { TileEdge = TileEdge });
    private static MapEnvironment Env(MapDocument d) => (MapEnvironment)d.GetFile(TerrainCommand.TerrainFile)!.Model!;

    private static MapDocument WithSpike()
    {
        var doc = Blank();
        TerrainCommand.DeformRect(doc, 3, 3, 5, 5, TerrainCommand.HeightOp.Set, 0f);
        TerrainCommand.Deform(doc, 4, 4, 0, TerrainCommand.HeightOp.Set, 10f, TerrainCommand.BrushShape.Square);
        return doc;
    }

    [Fact]
    public void One_pass_averages_the_spike_toward_its_neighbours()
    {
        var doc = WithSpike();
        var r = TerrainSmoothCommand.Smooth(doc, 1);
        Assert.True(r.Ok, r.Message);

        // self(10) plus four zero neighbours, over five samples.
        Assert.Equal(2f, Env(doc).TerrainTiles[Idx(4, 4)].Height, 3);
    }

    [Fact]
    public void Each_pass_persists_through_save()
    {
        var doc = WithSpike();
        TerrainSmoothCommand.Smooth(doc, 1);
        var reloaded = MapDocument.Load(doc.SaveToBytes());
        Assert.Equal(2f, Env(reloaded).TerrainTiles[Idx(4, 4)].Height, 3);
    }

    [Fact]
    public void More_passes_lower_the_spike_further()
    {
        var one = WithSpike();
        TerrainSmoothCommand.Smooth(one, 1);
        var three = WithSpike();
        TerrainSmoothCommand.Smooth(three, 3);

        Assert.True(Env(three).TerrainTiles[Idx(4, 4)].Height < Env(one).TerrainTiles[Idx(4, 4)].Height);
    }

    [Fact]
    public void Zero_passes_changes_nothing()
    {
        var doc = WithSpike();
        float before = Env(doc).TerrainTiles[Idx(4, 4)].Height;
        var r = TerrainSmoothCommand.Smooth(doc, 0);
        Assert.True(r.Ok, r.Message);
        Assert.Equal(0, r.TilesChanged);
        Assert.Equal(before, Env(doc).TerrainTiles[Idx(4, 4)].Height);
    }

    [Fact]
    public void Negative_passes_is_rejected()
    {
        var r = TerrainSmoothCommand.Smooth(Blank(), -1);
        Assert.False(r.Ok);
    }

    [Fact]
    public void Smooth_refuses_a_map_with_no_terrain()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = new byte[] { 0x2F, 0x2F },
        }));
        var r = TerrainSmoothCommand.Smooth(doc, 1);
        Assert.False(r.Ok);
    }
}
