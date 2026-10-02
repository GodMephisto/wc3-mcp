using Wc3.GameData;
using Wc3.Modeling;
using Wc3.Render;
using Xunit;

namespace Wc3.Tests;

// Depends on the local WC3 CASC install (same assumption as the object-data / model-render tests).
[Trait("Category", "GameData")]
public class TerrainArtCatalogTests
{
    [Fact]
    public void Resolves_LordaeronSummer_grass_to_rgba_cell()
    {
        Assert.True(Wc3.GameData.GameData.TryOpen(null, out var ctx, out var diag), $"GameData open failed: {diag}");
        Assert.NotNull(ctx);

        var cat = TerrainArtCatalog.Open(ctx!);
        Assert.True(cat.HasCatalog, "Terrain.slk not found in CASC");

        var img = cat.Resolve("Lgrs"); // Lordaeron Summer Grass
        Assert.NotNull(img);
        Assert.Equal(TerrainArtCatalog.Cell, img!.Width);
        Assert.Equal(TerrainArtCatalog.Cell, img.Height);
        Assert.Equal(TerrainArtCatalog.Cell * TerrainArtCatalog.Cell * 4, img.Rgba.Length);

        bool anyNonZero = false;
        foreach (var b in img.Rgba) if (b != 0) { anyNonZero = true; break; }
        Assert.True(anyNonZero, "decoded grass tile is all-zero");
    }

    [Fact]
    public void Caches_and_returns_same_instance()
    {
        Assert.True(Wc3.GameData.GameData.TryOpen(null, out var ctx, out _));
        var cat = TerrainArtCatalog.Open(ctx!);
        var a = cat.Resolve("Ldrt");
        var b = cat.Resolve("Ldrt");
        Assert.Same(a, b);
    }
}
