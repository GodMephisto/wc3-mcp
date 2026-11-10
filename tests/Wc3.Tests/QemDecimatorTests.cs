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

    [Fact]
    public void A_seam_pair_survives_decimation()
    {
        // Two quads sharing the edge x=1, each with its own UV island, so the shared edge is two
        // coincident vertices with different UVs, a seam. The unguarded decimator welded them and
        // tore the seam, so this pins the guard and the fact that decimation still ran.
        var v = new[]
        {
            0f, 0, 0, 1, 0, 0, 0, 1, 0, 1, 1, 0,
            1f, 0, 0, 2, 0, 0, 1, 1, 0, 2, 1, 0,
        };
        var uv = new[]
        {
            0f, 0, 1, 0, 0, 1, 1, 1,
            2f, 0, 3, 0, 2, 1, 3, 1,
        };
        var idx = new[] { 0, 1, 2, 1, 3, 2, 4, 5, 6, 5, 7, 6 };

        var r = QemDecimator.Decimate(v, idx, 0.3f, uv);

        Assert.True(r.OutputFaces < idx.Length / 3, "decimation did not run, the test would pass vacuously");
        Assert.NotEqual(r.Remap[1], r.Remap[4]);
        Assert.NotEqual(r.Remap[3], r.Remap[6]);
    }
}
