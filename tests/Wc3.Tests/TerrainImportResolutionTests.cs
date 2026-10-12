// tests/Wc3.Tests/TerrainImportResolutionTests.cs
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using War3Net.Build.Environment;
using Wc3.Model;
using Wc3.Modeling;
using Wc3.Render;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// Map imports that override terrain tile textures must be found before the
/// base game CASC, matching the game's own precedence.
///
/// The old behavior was to only check the base game CASC (war3.w3mod:...),
/// so imported .blp / .dds files were silently ignored and the renderer
/// fell back to flat colours.
///
/// This test creates a map with a .blp import at the exact path
/// Terrain.slk references for a known terrain type, then asserts the
/// catalog resolves that import, not the base game file.
/// </summary>
public class TerrainImportResolutionTests
{
    /// <summary>
    /// A map with an imported .blp at the path Terrain.slk references
    /// must resolve the import, not the base game file.
    /// </summary>
    [Fact]
    [Trait("Category", "GameData")]
    public void Map_imported_blp_resolved_over_base_game()
    {
        // Skip when game data (CASC) is not available
        if (!Wc3.GameData.GameData.TryOpen(null, out var ctx, out _)) return;

        // Step 1: create a valid 128x128 .blp file (the cell size)
        const int size = TerrainArtCatalog.Cell;
        var blp = CreateQuadBlp(size);

        // Step 2: build a blank map that has terrain types
        var doc = BlankMap.Create();

        // Step 3: find what path Terrain.slk assigns to the first terrain type
        var cat = TerrainArtCatalog.Open(ctx!, doc);
        Assert.True(cat.HasCatalog, "Terrain.slk missing");

        var env = doc.GetFile("war3map.w3e")?.Model as MapEnvironment;
        Assert.NotNull(env);
        Assert.NotEmpty(env.TerrainTypes);

        var firstType = env.TerrainTypes[0];
        string tileId = cat.TileIdOf(firstType);
        Assert.NotNull(tileId);

        // Get the relative path Terrain.slk assigns to this tile
        string rel = cat.TilePath(tileId)!;

        // Step 4: add the .blp to the map at that path
        doc.AddOrReplaceRawFile(rel + ".blp", blp);

        // Step 5: resolve via a catalog that was opened with the map
        var mapCat = TerrainArtCatalog.Open(ctx!, doc);
        var img = mapCat.Resolve(tileId);

        // The map's .blp should be found (not null from the CASC miss)
        // Without the fix, this returns null and falls back to flat colour.
        Assert.NotNull(img);
        Assert.Equal(size, img.Width);
        Assert.Equal(size, img.Height);

        // Verify the decoded image matches our quadrant pattern (proves
        // we decoded the map's .blp, not the base game file)
        AssertPixelNear(img, 32, 32, 255, 0, 0, 20);    // red
        AssertPixelNear(img, 96, 32, 0, 255, 0, 20);    // green
        AssertPixelNear(img, 32, 96, 0, 0, 255, 20);    // blue
        AssertPixelNear(img, 96, 96, 255, 255, 255, 20); // white
    }

    /// <summary>
    /// Resolved layers from a map with an imported .blp must come from
    /// the import, not flat colours or the base game file.
    /// </summary>
    [Fact]
    [Trait("Category", "GameData")]
    public void BuildLayersForMap_respects_map_import()
    {
        // Skip when game data (CASC) is not available
        if (!Wc3.GameData.GameData.TryOpen(null, out var ctx, out _)) return;

        // Create a 128x128 solid-teal .blp file (0, 255, 255)
        const int size = TerrainArtCatalog.Cell;
        var blp = CreateTealBlp(size);

        var doc = BlankMap.Create();

        var cat = TerrainArtCatalog.Open(ctx!, doc);
        Assert.True(cat.HasCatalog);

        var env = (MapEnvironment)doc.GetFile("war3map.w3e")!.Model!;
        Assert.NotEmpty(env.TerrainTypes);

        string tileId = cat.TileIdOf(env.TerrainTypes[0]);
        string rel = cat.TilePath(tileId)!;

        // Add a teal .blp at the path Terrain.slk references
        doc.AddOrReplaceRawFile(rel + ".blp", blp);

        // Build layers. The import should be used for the first terrain type.
        var layers = TerrainArtCatalog.BuildLayersForMap(doc, out var layerCount, out var cell);

        Assert.Equal(size, cell);

        // A map with multiple terrain types has multiple layers (the blank map
        // has 5). The test must check that the layer for the overridden type
        // contains the imported pixels, not that layerCount equals 1.
        var types = env.TerrainTypes;
        int overriddenIndex = 0; // index 0 = first terrain type
        int layerBytes = cell * cell * 4;

        // Spot check the first 4 pixels of the overridden layer (solid teal).
        int layerStart = overriddenIndex * layerBytes;
        int px0 = layerStart;
        Assert.Equal((byte)0, layers[px0]);
        Assert.Equal((byte)255, layers[px0 + 1]);
        Assert.Equal((byte)255, layers[px0 + 2]);
        Assert.Equal((byte)255, layers[px0 + 3]);

        // Spot check the first 4 pixels of the second layer (if any), which
        // should NOT be teal (proves the import does not bleed into other layers)
        if (types!.Count > 1)
        {
            int baseOther = 1 * layerBytes;
            int pxOther = baseOther;
            Assert.True(
                layers[pxOther] != 0 || layers[pxOther + 1] != 255 || layers[pxOther + 2] != 255,
                "second layer should not be teal (import only applies to the overridden type)");
        }
    }

    /// <summary>
    /// Creates a 128x128 .blp file with four colour quadrants (TL=red, TR=green,
    /// BL=blue, BR=white). The colours let us verify that the test's .blp, not
    /// the base game file, was decoded.
    ///
    /// Must swap the red and blue channels before encoding, matching
    /// <see cref="Wc3.Render.TextureConvert.EncodeBlp"/> which swaps RGBA
    /// into the BGRA byte order that War3Net's BLP encoder expects.
    /// Without this swap the decoder reads back swapped colours.
    /// </summary>
    private static byte[] CreateQuadBlp(int size)
    {
        var rgba = new byte[size * size * 4];
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                int i = (y * size + x) * 4;
                bool right = x >= size / 2, bottom = y >= size / 2;
                (rgba[i], rgba[i + 1], rgba[i + 2]) = (right, bottom) switch
                {
                    (false, false) => ((byte)255, (byte)0, (byte)0),
                    (true, false) => ((byte)0, (byte)255, (byte)0),
                    (false, true) => ((byte)0, (byte)0, (byte)255),
                    (true, true) => ((byte)255, (byte)255, (byte)255),
                };
                rgba[i + 3] = 255;
            }

        // Swap R and B to match EncodeBlp which swaps RGBA into BGRA order
        // before passing to BlpEncoder (colours survive a full
        // round-trip through EncodeBlp + BlpDecoder when swapped this way).
        for (int i = 0; i < rgba.Length; i += 4)
            (rgba[i], rgba[i + 2]) = (rgba[i + 2], rgba[i]);

        var image = Image.LoadPixelData<Rgba32>(rgba, size, size);
        using var ms = new System.IO.MemoryStream();
        new War3Net.Drawing.Blp.BlpEncoder(
            new War3Net.Drawing.Blp.Blp1EncodingOptions
            {
                GenerateMipmaps = true,
                JpegQuality = 90,
            })
            .Encode(ms, size, size, rgba);
        return ms.ToArray();
    }

    /// <summary>
    /// Creates a 128x128 solid-teal .blp file (0, 255, 255).
    ///
    /// Must swap the red and blue channels before encoding, matching
    /// <see cref="Wc3.Render.TextureConvert.EncodeBlp"/>.
    /// </summary>
    private static byte[] CreateTealBlp(int size)
    {
        var rgba = new byte[size * size * 4];
        for (int i = 0; i < rgba.Length; i += 4)
        {
            // R G B A
            rgba[i] = 0;
            rgba[i + 1] = 255;
            rgba[i + 2] = 255;
            rgba[i + 3] = 255;
        }

        // Swap R and B to match EncodeBlp which swaps RGBA into BGRA order
        // before passing to BlpEncoder (colours survive a full
        // round-trip through EncodeBlp + BlpDecoder when swapped this way).
        for (int i = 0; i < rgba.Length; i += 4)
            (rgba[i], rgba[i + 2]) = (rgba[i + 2], rgba[i]);

        using var ms = new System.IO.MemoryStream();
        new War3Net.Drawing.Blp.BlpEncoder(
            new War3Net.Drawing.Blp.Blp1EncodingOptions
            {
                GenerateMipmaps = true,
                JpegQuality = 90,
            })
            .Encode(ms, size, size, rgba);
        return ms.ToArray();
    }

    private static void AssertPixelNear(TextureImage tex, int x, int y, byte r, byte g, byte b, int tolerance)
    {
        int i = (y * tex.Width + x) * 4;
        Assert.True(
            Math.Abs(tex.Rgba[i] - r) <= tolerance
            && Math.Abs(tex.Rgba[i + 1] - g) <= tolerance
            && Math.Abs(tex.Rgba[i + 2] - b) <= tolerance,
            $"pixel ({x},{y}) = ({tex.Rgba[i]},{tex.Rgba[i + 1]},{tex.Rgba[i + 2]}), expected ~({r},{g},{b})");
    }
}
