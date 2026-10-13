// tests/Wc3.Tests/TerrainOptimizeCommandTests.cs
using War3Net.Build.Environment; // MapEnvironment, TerrainType, CliffType
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public sealed class TerrainOptimizeCommandTests
{
    private const int TileEdge = 8;   // 8x8 tiles -> 9x9 = 81 corners

    private static MapDocument Doc() => BlankMap.Create(new BlankMapOptions { TileEdge = TileEdge });

    private static MapEnvironment Env(MapDocument doc)
        => (MapEnvironment)doc.GetFile(TerrainOptimizeCommand.TerrainFile)!.Model!;

    private static byte[] W3e(MapDocument doc)
        => doc.GetFile(TerrainOptimizeCommand.TerrainFile)!.RawBytes;

    [Fact]
    public void Compacts_unused_ground_tiles_and_remaps_cells()
    {
        var doc = Doc();
        var env = Env(doc);

        // Declare four ground tiles, reference only index 0 and index 2.
        var all = Enum.GetValues<TerrainType>();
        env.TerrainTypes.Clear();
        env.TerrainTypes.Add(all[0]);
        env.TerrainTypes.Add(all[1]);
        env.TerrainTypes.Add(all[2]);
        env.TerrainTypes.Add(all[3]);

        var tiles = env.TerrainTiles;
        foreach (var t in tiles) t.Texture = 0;
        tiles[0].Texture = 2;
        tiles[10].Texture = 2;
        tiles[20].Texture = 2;

        var r = TerrainOptimizeCommand.Execute(doc, "synth.w3x");

        Assert.True(r.Ok, r.Message);
        Assert.True(r.Changed);
        Assert.Equal(4, r.GroundTypesBefore);
        Assert.Equal(2, r.GroundTypesAfter);
        Assert.Equal(2, r.GroundTypesRemoved);
        Assert.Equal(8, r.BytesSaved);           // two dropped four-byte entries
        Assert.Equal(8, r.BytesBefore - r.BytesAfter);

        // The palette keeps the original relative order of the retained entries.
        Assert.Equal(2, env.TerrainTypes.Count);
        Assert.Equal(all[0], env.TerrainTypes[0]);
        Assert.Equal(all[2], env.TerrainTypes[1]);

        // Every cell now points at the same tile identity it did before.
        Assert.Equal(0, tiles[1].Texture);
        Assert.Equal(1, tiles[0].Texture);
        Assert.Equal(1, tiles[10].Texture);
        Assert.Equal(1, tiles[20].Texture);
        Assert.Equal(3, r.CellsRemapped);
    }

    [Fact]
    public void Compacts_unused_cliff_tiles_and_remaps_cliff_texture()
    {
        var doc = Doc();
        var env = Env(doc);

        var cliffs = Enum.GetValues<CliffType>();
        env.CliffTypes.Clear();
        env.CliffTypes.Add(cliffs[0]);
        env.CliffTypes.Add(cliffs[1]);
        env.CliffTypes.Add(cliffs[2]);

        var tiles = env.TerrainTiles;
        foreach (var t in tiles) t.CliffTexture = 0;
        tiles[3].CliffTexture = 2;
        tiles[4].CliffTexture = 2;

        var r = TerrainOptimizeCommand.Execute(doc, "synth.w3x");

        Assert.True(r.Ok, r.Message);
        Assert.Equal(3, r.CliffTypesBefore);
        Assert.Equal(2, r.CliffTypesAfter);
        Assert.Equal(2, env.CliffTypes.Count);
        Assert.Equal(cliffs[0], env.CliffTypes[0]);
        Assert.Equal(cliffs[2], env.CliffTypes[1]);
        Assert.Equal(1, tiles[3].CliffTexture);
        Assert.Equal(0, tiles[0].CliffTexture);
    }

    [Fact]
    public void The_no_cliff_sentinel_is_not_treated_as_a_dangling_index()
    {
        // Every uncliffed corner carries cliff texture 0xF, which is above any real map's declared
        // cliff count (2 to 4). Treating it as an index made the pass refuse every real map with a
        // "cliff texture index out of range" message and compact nothing.
        var doc = Doc();
        var env = Env(doc);
        var cliffs = Enum.GetValues<CliffType>();
        env.CliffTypes.Clear();
        env.CliffTypes.Add(cliffs[0]);
        env.CliffTypes.Add(cliffs[1]);
        env.CliffTypes.Add(cliffs[2]);

        var tiles = env.TerrainTiles;
        foreach (var t in tiles) t.CliffTexture = TerrainEditCommand.CliffTextureNone;
        tiles[3].CliffTexture = 2;   // uses the third declared cliff
        tiles[4].CliffTexture = 0;   // and the first, leaving index 1 unused

        var r = TerrainOptimizeCommand.Execute(doc, "synth.w3x");

        Assert.True(r.Ok, r.Message);
        Assert.Equal(2, r.CliffTypesAfter);
        Assert.Equal(TerrainEditCommand.CliffTextureNone, tiles[0].CliffTexture); // sentinel untouched
        Assert.Equal(1, tiles[3].CliffTexture);                                   // remapped down
        Assert.Equal(0, tiles[4].CliffTexture);
    }

    [Fact]
    public void A_map_that_uses_every_declared_tile_is_left_unchanged()
    {
        var doc = Doc();
        var env = Env(doc);

        var all = Enum.GetValues<TerrainType>();
        env.TerrainTypes.Clear();
        env.TerrainTypes.Add(all[0]);
        env.TerrainTypes.Add(all[1]);
        var tiles = env.TerrainTiles;
        for (int i = 0; i < tiles.Count; i++) tiles[i].Texture = i % 2;   // both entries referenced

        byte[] before = W3e(doc).ToArray();
        var r = TerrainOptimizeCommand.Execute(doc, "synth.w3x");

        Assert.True(r.Ok, r.Message);
        Assert.False(r.Changed);
        Assert.Equal(0, r.BytesSaved);
        Assert.Equal(2, env.TerrainTypes.Count);

        // The untouched entry survived the save path byte for byte.
        var reload = MapDocument.Load(doc.SaveToBytes());
        Assert.Equal(before, W3e(reload));
    }

    [Fact]
    public void Refuses_a_map_whose_cells_reference_an_out_of_range_texture()
    {
        var doc = Doc();
        var env = Env(doc);

        var all = Enum.GetValues<TerrainType>();
        env.TerrainTypes.Clear();
        env.TerrainTypes.Add(all[0]);
        env.TerrainTypes.Add(all[1]);
        env.TerrainTiles[0].Texture = 5;   // beyond the two declared entries

        var r = TerrainOptimizeCommand.Execute(doc, "synth.w3x");

        Assert.False(r.Ok);
        Assert.Contains("range", r.Message);
    }
}
