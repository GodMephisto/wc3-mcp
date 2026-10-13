// tests/Wc3.Tests/QemDecimatorTests.cs
using Wc3.Modeling;

namespace Wc3.Tests;

public sealed class QemDecimatorTests
{
    /// <summary>A flat grid of (n-1)^2*2 triangles, all in the z=0 plane.</summary>
    private static (float[] Vertices, int[] Indices) Grid(int n)
    {
        var v = new List<float>();
        for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++) { v.Add(x); v.Add(y); v.Add(0); }
        int V(int x, int y) => y * n + x;
        var idx = new List<int>();
        for (int y = 0; y < n - 1; y++)
            for (int x = 0; x < n - 1; x++)
            {
                idx.Add(V(x, y)); idx.Add(V(x + 1, y)); idx.Add(V(x, y + 1));
                idx.Add(V(x + 1, y)); idx.Add(V(x + 1, y + 1)); idx.Add(V(x, y + 1));
            }
        return (v.ToArray(), idx.ToArray());
    }

    [Fact]
    public void Ratio_one_returns_the_input_unchanged()
    {
        var (v, idx) = Grid(5);
        var r = QemDecimator.Decimate(v, idx, 1.0f);
        Assert.Equal(idx, r.Indices);
        Assert.Equal(v, r.Vertices);
    }

    [Fact]
    public void Decimation_reduces_the_face_count_and_stays_indexable()
    {
        var (v, idx) = Grid(9);
        var r = QemDecimator.Decimate(v, idx, 0.5f);
        Assert.True(r.OutputFaces < idx.Length / 3);
        Assert.True(r.OutputFaces > 0);
        Assert.All(r.Indices, i => Assert.InRange(i, 0, r.OutputVertices - 1));
    }

    [Fact]
    public void The_remap_covers_every_input_vertex()
    {
        var (v, idx) = Grid(9);
        var r = QemDecimator.Decimate(v, idx, 0.5f);
        Assert.Equal(v.Length / 3, r.Remap.Length);
        Assert.All(r.Remap, m => Assert.InRange(m, 0, r.OutputVertices - 1));
    }
}
