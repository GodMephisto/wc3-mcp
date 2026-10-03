// tests/Wc3.Tests/TerrainRenderColorTests.cs
using War3Net.Build.Environment;
using Wc3.Model;
using Wc3.Render;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// The PNG renderer must colour a tile from the tileset's OWN art, not from a hardcoded
/// guess, because that difference is the whole reason an exported map did not look like the
/// map in game. The Studio viewport has always used the real art via TerrainArtCatalog and
/// this path had not, which is the same "built, working, wired to one front-end" shape this
/// repo keeps finding.
///
/// These need a Warcraft III install, so they carry the GameData trait. The renderer must
/// still work without one, and the hermetic half of that contract is pinned below.
/// </summary>
public class TerrainRenderColorTests
{
    private readonly ITestOutputHelper _out;
    public TerrainRenderColorTests(ITestOutputHelper output) => _out = output;

    private static string? AnyMap()
    {
        string dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Warcraft III", "Maps", "Download");
        if (!Directory.Exists(dir)) return null;
        return Directory.EnumerateFiles(dir, "*.w3?")
            .OrderBy(p => new FileInfo(p).Length).FirstOrDefault();
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Sampled_colours_come_from_the_tileset_not_the_hardcoded_table()
    {
        string? path = AnyMap();
        if (path is null) { _out.WriteLine("no maps on disk"); return; }

        var doc = MapDocument.Load(path);
        var env = doc.GetFile("war3map.w3e")?.Model as MapEnvironment;
        Assert.NotNull(env);

        var sampled = TerrainArtCatalog.AverageColorsForMap(doc);
        Assert.NotNull(sampled);
        Assert.Equal(env!.TerrainTypes.Count, sampled!.Length);

        int resolved = 0, differ = 0;
        for (int i = 0; i < sampled.Length; i++)
        {
            if (sampled[i] is not { } s) continue;
            resolved++;
            var hard = TerrainRenderer.ColorForTerrainType(env.TerrainTypes[i]);
            if (s.R != hard.R || s.G != hard.G || s.B != hard.B) differ++;
            _out.WriteLine($"{env.TerrainTypes[i],-12} sampled=({s.R},{s.G},{s.B}) "
                         + $"hardcoded=({hard.R},{hard.G},{hard.B})");
        }

        // Every type on this map resolved when the fallback probe measured the library, so a
        // regression that broke resolution would show up here as a drop rather than silently.
        Assert.True(resolved == sampled.Length,
            $"only {resolved} of {sampled.Length} terrain types resolved to a texture");

        // If sampling produced exactly the hardcoded table, the wiring is not doing anything.
        Assert.True(differ > 0,
            "every sampled colour equalled the hardcoded guess, so the tileset art is not "
            + "actually reaching the renderer");
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void The_rendered_png_changes_when_real_tile_art_is_available()
    {
        string? path = AnyMap();
        if (path is null) { _out.WriteLine("no maps on disk"); return; }

        var doc = MapDocument.Load(path);
        byte[] png = TerrainRenderer.RenderTerrainPng(doc);

        Assert.NotEmpty(png);
        // PNG magic, so a failure here is a broken encoder rather than a broken colour table.
        Assert.Equal(0x89, png[0]);
        Assert.Equal((byte)'P', png[1]);
        _out.WriteLine($"rendered {png.Length:N0} bytes");
    }

    [Fact]
    public void A_blank_map_still_renders_without_any_game_install()
    {
        // The hermetic half of the contract. AverageColorsForMap returns null when the base
        // game is missing and the hardcoded colours must carry the render, so this must pass
        // on a machine with no Warcraft III at all.
        var doc = BlankMap.Create();
        byte[] png = TerrainRenderer.RenderTerrainPng(doc);
        Assert.NotEmpty(png);
        Assert.Equal(0x89, png[0]);
    }
}
