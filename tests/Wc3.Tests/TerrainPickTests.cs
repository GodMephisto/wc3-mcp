// tests/Wc3.Tests/TerrainPickTests.cs
using Wc3.Model;
using Wc3.Render;

namespace Wc3.Tests;

/// <summary>
/// <see cref="TerrainRenderer.PickTerrain"/> must invert the perspective camera used by
/// <see cref="TerrainRenderer.RenderPerspectivePng"/>: the centre pixel casts a ray
/// straight at the scene centre, so on a flat map it recovers world (0,0) regardless of
/// pitch, and a skyward ray must report a miss rather than a bogus placement.
/// </summary>
public class TerrainPickTests
{
    private const int W = 320, H = 240;

    [Fact]
    public void Pick_centre_pixel_recovers_scene_centre()
    {
        var doc = BlankMap.Create(new BlankMapOptions { TileEdge = 8 });

        var (ok, wx, wy) = TerrainRenderer.PickTerrain(
            doc, W, H, yawDegrees: 45f, pitchDegrees: 30f, zoom: 1f, px: W / 2f, py: H / 2f);

        Assert.True(ok, "centre pixel should strike the terrain");
        Assert.True(Math.Abs(wx) < 64f, $"world x should be ~0, got {wx}");
        Assert.True(Math.Abs(wy) < 64f, $"world y should be ~0, got {wy}");
    }

    [Theory]
    [InlineData(20f)]
    [InlineData(45f)]
    [InlineData(75f)]
    public void Pick_centre_pixel_is_pitch_stable(float pitch)
    {
        var doc = BlankMap.Create(new BlankMapOptions { TileEdge = 8 });

        var (ok, wx, wy) = TerrainRenderer.PickTerrain(doc, W, H, 45f, pitch, 1f, W / 2f, H / 2f);

        Assert.True(ok, $"centre should hit at pitch {pitch}");
        Assert.True(Math.Abs(wx) < 64f && Math.Abs(wy) < 64f, $"centre ~0 at pitch {pitch}, got ({wx},{wy})");
    }

    [Fact]
    public void Pick_stays_within_the_map_bounds()
    {
        var doc = BlankMap.Create(new BlankMapOptions { TileEdge = 8 });
        // World half-extent for an 8-tile map: 8 * 128 / 2 = 512.
        var (ok, wx, wy) = TerrainRenderer.PickTerrain(doc, W, H, 45f, 30f, 1f, W * 0.75f, H * 0.75f);

        Assert.True(ok, "an on-terrain pixel should hit");
        Assert.InRange(wx, -512f, 512f);
        Assert.InRange(wy, -512f, 512f);
    }

    [Fact]
    public void Pick_skyward_pixel_reports_a_miss()
    {
        var doc = BlankMap.Create(new BlankMapOptions { TileEdge = 8 });
        // Far above the frame: the ray points up, away from the ground.
        var (ok, _, _) = TerrainRenderer.PickTerrain(doc, W, H, 45f, 30f, 1f, px: W / 2f, py: -4000f);

        Assert.False(ok, "a skyward ray must not hit the terrain");
    }

    [Fact]
    public void Pick_rejects_a_map_with_no_terrain()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        { ["war3map.j"] = new byte[] { 1 } }));

        var (ok, _, _) = TerrainRenderer.PickTerrain(doc, W, H, 45f, 30f, 1f, W / 2f, H / 2f);

        Assert.False(ok, "a map with no war3map.w3e cannot be picked");
    }
}
