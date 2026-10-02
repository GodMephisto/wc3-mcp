using System;
using System.IO;
using System.Linq;
using Wc3.Model;
using Wc3.Render;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Machine-gated probe: confirms <see cref="TerrainArtCatalog.BuildLayersForMap"/> assembles a
/// real map's terrain-type layers end-to-end (env extraction → CASC → BLP → RGBA) and that at
/// least one layer is a real, non-uniform texture rather than a flat-colour fallback. Self-locates
/// the first loadable map under the local WC3 Maps directory, so it needs no hard-coded map path.
/// </summary>
public class TerrainLayersProbe
{
    private static readonly string MapsRoot = TestCorpus.MapsRoot;
    private readonly ITestOutputHelper _out;

    public TerrainLayersProbe(ITestOutputHelper output) => _out = output;

    [Fact]
    public void BuildLayersForMap_resolves_real_textures()
    {
        if (!Directory.Exists(MapsRoot)) return; // gated: only runs where local maps are present

        foreach (var path in Directory.EnumerateFiles(MapsRoot, "*.w3x", SearchOption.AllDirectories).Take(12))
        {
            MapDocument doc;
            try { doc = MapDocument.Load(path); }
            catch { continue; }
            if (doc.GetFile("war3map.w3e") is null) continue;

            var data = TerrainArtCatalog.BuildLayersForMap(doc, out int count, out int cell);
            Assert.True(count >= 1, "at least one terrain layer");
            Assert.Equal(count * cell * cell * 4, data.Length);

            int layerBytes = cell * cell * 4;
            int nonFlat = 0;
            for (int i = 0; i < count; i++)
            {
                int off = i * layerBytes;
                bool uniform = true;
                for (int p = 4; p < layerBytes; p += 4)
                    if (data[off + p] != data[off] || data[off + p + 1] != data[off + 1] || data[off + p + 2] != data[off + 2])
                    { uniform = false; break; }
                if (!uniform) nonFlat++;
            }

            _out.WriteLine($"[TerrainLayersProbe] map={Path.GetFileName(path)} layers={count} cell={cell} nonFlat={nonFlat}");
            Assert.True(nonFlat > 0, $"expected >=1 real texture layer, got {nonFlat}/{count} non-flat");
            return; // one real map is enough
        }

        Assert.Fail("no loadable map with terrain found under " + MapsRoot);
    }
}
