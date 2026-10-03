// tests/Wc3.Tests/TerrainArtFallbackProbe.cs
using War3Net.Build.Environment;
using Wc3.Model;
using Wc3.Render;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Why does a rendered map look nothing like the map in game.
///
/// The user reported that the Studio's map picture "looks different from the map in game,
/// like it is not on a proper map". Rendering two real maps showed why it reads that way,
/// both come back in a handful of flat colours with no tile texture at all.
///
/// TerrainArtCatalog is supposed to resolve each terrain type to its real tile texture from
/// the base game and only fall back to a flat colour when that fails. The fallback is silent
/// by design, "never throws, yields null when a tile is unavailable", which is correct
/// behaviour for a renderer and terrible for diagnosis, because a fully flat map and a fully
/// textured one take exactly the same code path and report the same success.
///
/// So this counts the fallbacks and separates the two reasons they happen, the texture could
/// not be found at all, or it was found and then REJECTED for being the wrong size. The
/// second is the interesting one, because Crop returns min(Cell, src.Width) while the caller
/// demands exactly Cell, so any tile smaller than 128 pixels is silently discarded.
/// </summary>
public class TerrainArtFallbackProbe
{
    private readonly ITestOutputHelper _out;
    public TerrainArtFallbackProbe(ITestOutputHelper output) => _out = output;

    private static IEnumerable<string> Maps()
    {
        string dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Warcraft III", "Maps", "Download");
        if (!Directory.Exists(dir)) yield break;
        foreach (var p in Directory.EnumerateFiles(dir, "*.w3?")
                     .OrderBy(p => new FileInfo(p).Length).Take(10))
            yield return p;
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void How_many_terrain_types_fall_back_to_a_flat_colour()
    {
        if (!Wc3.GameData.GameData.TryOpen(null, out var ctx, out var why) || ctx is null)
        {
            _out.WriteLine($"no game data, nothing measured: {why}");
            return;
        }

        var cat = TerrainArtCatalog.Open(ctx);
        _out.WriteLine($"Terrain.slk found: {cat.HasCatalog}");
        if (!cat.HasCatalog)
        {
            _out.WriteLine("VERDICT: Terrain.slk is missing, so EVERY tile on EVERY map is a flat "
                         + "colour. That alone explains the reported appearance.");
            return;
        }

        int totalTypes = 0, resolved = 0, nullMiss = 0, wrongSize = 0;
        var sizes = new Dictionary<string, int>(StringComparer.Ordinal);
        var badIds = new List<string>();

        foreach (var path in Maps())
        {
            MapDocument doc;
            try { doc = MapDocument.Load(path); } catch { continue; }
            var env = doc.GetFile("war3map.w3e")?.Model as MapEnvironment;
            var types = env?.TerrainTypes;
            if (types is null || types.Count == 0) continue;

            var perMap = new List<string>();
            foreach (var t in types)
            {
                totalTypes++;
                string id = cat.TileIdOf(t);
                var img = cat.Resolve(t);

                if (img is null)
                {
                    nullMiss++;
                    perMap.Add($"{id}=missing");
                    if (badIds.Count < 20) badIds.Add(id + " (not found)");
                    continue;
                }

                string dim = $"{img.Width}x{img.Height}";
                sizes[dim] = sizes.TryGetValue(dim, out int c) ? c + 1 : 1;

                // The exact predicate BuildLayersForMap uses to accept a texture.
                bool accepted = img.Width == TerrainArtCatalog.Cell
                             && img.Height == TerrainArtCatalog.Cell
                             && img.Rgba.Length == TerrainArtCatalog.Cell * TerrainArtCatalog.Cell * 4;
                if (accepted) { resolved++; perMap.Add($"{id}=ok"); }
                else
                {
                    wrongSize++;
                    perMap.Add($"{id}={dim} REJECTED");
                    if (badIds.Count < 20) badIds.Add($"{id} ({dim}, rejected for size)");
                }
            }
            _out.WriteLine($"{Path.GetFileName(path),-40} {types.Count,2} type(s)  "
                         + string.Join(" ", perMap.Take(6)));
        }

        _out.WriteLine($"\n{totalTypes} terrain type(s) examined across the sample");
        _out.WriteLine($"  {resolved} resolved to a real texture and were ACCEPTED");
        _out.WriteLine($"  {nullMiss} could not be found at all");
        _out.WriteLine($"  {wrongSize} WERE found and then rejected for being the wrong size");

        _out.WriteLine("\ndecoded texture sizes seen:");
        foreach (var kv in sizes.OrderByDescending(k => k.Value))
            _out.WriteLine($"  {kv.Key,-12} {kv.Value}");

        if (badIds.Count > 0)
        {
            _out.WriteLine("\nfirst failures:");
            foreach (var b in badIds) _out.WriteLine("  " + b);
        }

        double flatRate = totalTypes == 0 ? 0 : 100.0 * (nullMiss + wrongSize) / totalTypes;
        _out.WriteLine($"\n{flatRate:F1}% of terrain types render as a FLAT COLOUR rather than a texture");
        _out.WriteLine(flatRate > 50
            ? "VERDICT: most tiles are flat colours, which is exactly why a rendered map does "
            + "not look like the map in game."
            : "VERDICT: most tiles do resolve, so flat colour is not the main cause and the "
            + "difference lies elsewhere.");
    }
}
