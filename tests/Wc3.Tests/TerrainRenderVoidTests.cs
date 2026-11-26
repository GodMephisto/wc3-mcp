// tests/Wc3.Tests/TerrainRenderVoidTests.cs
using SkiaSharp;
using War3Net.Build.Environment;
using Wc3.Model;
using Wc3.Render;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// A tile outside the playable area must render as void, not as ground.
///
/// This is what made a rendered map read as "not on a proper map". The playable arena sat in
/// one corner of a field of dirt, because every boundary tile was painted with its ground
/// texture like any other. The game does not draw those tiles and the map's own minimap shows
/// them black.
///
/// The cause was measured rather than guessed, and the first guess was wrong. Fog of war
/// looked like the explanation until two corners of Anime_WOS2_0.29d were compared, one inside
/// the arena carrying IsBoundary false with grass, and one outside carrying IsBoundary true
/// with dirt. The flag is in the file, so the renderer had the answer all along.
/// </summary>
public class TerrainRenderVoidTests
{
    private readonly ITestOutputHelper _out;
    public TerrainRenderVoidTests(ITestOutputHelper output) => _out = output;

    private const int TileEdge = 8;
    private const int W = TileEdge + 1;

    private static MapEnvironment Env(MapDocument doc)
        => (MapEnvironment)doc.GetFile("war3map.w3e")!.Model!;

    [Fact]
    public void A_boundary_tile_renders_as_void_and_an_ordinary_tile_does_not()
    {
        var doc = BlankMap.Create(new BlankMapOptions { TileEdge = TileEdge });
        var env = Env(doc);

        // Mark one corner as boundary and leave its neighbour alone, so a single render
        // carries both cases and neither can pass by accident.
        var tiles = env.TerrainTiles;
        int boundaryIdx = 2 * W + 2;
        int ordinaryIdx = 2 * W + 3;
        tiles[boundaryIdx].IsBoundary = true;
        tiles[ordinaryIdx].IsBoundary = false;

        byte[] png = TerrainRenderer.RenderTerrainPng(doc);
        using var img = SafeImages.Load(png);

        int scale = img.Width / W;
        Assert.True(scale >= 1, $"unexpected scale from {img.Width}px over {W} corners");

        // Rows are flipped so north is up, which the renderer does when painting.
        SKColor At(int x, int y) => img.GetPixel(x * scale + scale / 2, (W - 1 - y) * scale + scale / 2);

        var atBoundary = At(2, 2);
        var atOrdinary = At(3, 2);
        _out.WriteLine($"boundary pixel ({atBoundary.Red},{atBoundary.Green},{atBoundary.Blue}) "
                     + $"ordinary pixel ({atOrdinary.Red},{atOrdinary.Green},{atOrdinary.Blue})");

        Assert.Equal(TerrainRenderer.VoidColor.Alpha, atBoundary.Alpha);
        Assert.Equal(TerrainRenderer.VoidColor.Red, atBoundary.Red);
        Assert.Equal(TerrainRenderer.VoidColor.Green, atBoundary.Green);
        Assert.Equal(TerrainRenderer.VoidColor.Blue, atBoundary.Blue);

        Assert.False(
            atOrdinary.Red == TerrainRenderer.VoidColor.Red
            && atOrdinary.Green == TerrainRenderer.VoidColor.Green
            && atOrdinary.Blue == TerrainRenderer.VoidColor.Blue,
            "a tile that is NOT a boundary was painted as void, so the flag is being ignored "
            + "in the other direction");
    }

    [Fact]
    public void Void_wins_over_water_and_blight()
    {
        // A boundary tile is not drawn at all, so an overlay on it must not leak through. The
        // easy way to get this wrong is to paint void first and then blend water over it.
        var doc = BlankMap.Create(new BlankMapOptions { TileEdge = TileEdge });
        var tiles = Env(doc).TerrainTiles;

        int idx = 4 * W + 4;
        tiles[idx].IsBoundary = true;
        tiles[idx].IsWater = true;
        tiles[idx].IsBlighted = true;

        byte[] png = TerrainRenderer.RenderTerrainPng(doc);
        using var img = SafeImages.Load(png);
        int scale = img.Width / W;
        var px = img.GetPixel(4 * scale + scale / 2, (W - 1 - 4) * scale + scale / 2);

        Assert.Equal(TerrainRenderer.VoidColor.Red, px.Red);
        Assert.Equal(TerrainRenderer.VoidColor.Green, px.Green);
        Assert.Equal(TerrainRenderer.VoidColor.Blue, px.Blue);
    }
}
