// tests/Wc3.Tests/TerrainAuditCommandTests.cs
using War3Net.Build.Environment; // MapEnvironment
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

public class TerrainAuditCommandTests
{
    private const int TileEdge = 8;         // 8x8 tiles -> 9x9 = 81 corner tiles
    private const int W = TileEdge + 1;     // grid width in corners
    private static int Idx(int x, int y) => y * W + x;

    private static MapDocument BlankTerrain() => BlankMap.Create(new BlankMapOptions { TileEdge = TileEdge });

    private static MapEnvironment Env(MapDocument doc)
        => (MapEnvironment)doc.GetFile(TerrainAuditCommand.TerrainFile)!.Model!;

    private static TerrainAuditResult Audit(MapDocument doc) => TerrainAuditCommand.Execute(doc, "synthetic.w3x");

    // --- grid size, tile count, byte size -----------------------------------

    [Fact]
    public void Blank_terrain_reports_the_grid_and_is_flagged_as_all_zero()
    {
        var r = Audit(BlankTerrain());

        Assert.Equal(W, r.GridWidth);
        Assert.Equal(W, r.GridHeight);
        Assert.Equal(W * W, r.TileCount);
        Assert.Equal(5, r.GroundTileTypes);   // the blank palette
        Assert.Equal(0, r.CliffTileTypes);
        Assert.Equal(1, r.DistinctGroundTextures);
        Assert.Equal(1, r.DistinctCliffTextures);
        Assert.True(r.TerrainFileBytes > 0, "the w3e entry must have a measurable size");

        Assert.Contains(r.Issues, i => i.Kind == TerrainIssueKind.AllZeroTerrain);
        // The blank grid seats every corner at height 0 and cliff 0, so it is all-zero, not merely uniform.
        Assert.DoesNotContain(r.Issues, i => i.Kind == TerrainIssueKind.UniformTerrain);
        Assert.Contains(r.Diagnostics, d => d.Contains("cliff-texture-range"));
    }

    [Fact]
    public void Terrain_file_bytes_matches_the_entry_size()
    {
        var doc = BlankTerrain();
        var r = Audit(doc);
        Assert.Equal(doc.GetFile(TerrainAuditCommand.TerrainFile)!.CurrentSize, r.TerrainFileBytes);
    }

    // --- cell counts --------------------------------------------------------

    [Fact]
    public void Cliff_ramp_water_and_blight_cells_are_counted()
    {
        var doc = BlankTerrain();
        // Four disjoint 3x3 rectangles, one per flag, so each count is exactly 9.
        TerrainCommand.CliffRect(doc, 0, 0, 2, 2, TerrainCommand.CliffOp.Set, 2);
        TerrainCommand.RampRect(doc, 3, 0, 5, 2, on: true);
        TerrainCommand.WaterRect(doc, 6, 0, 8, 2, TerrainCommand.WaterOp.Set, 5f);
        TerrainCommand.BlightRect(doc, 0, 3, 2, 5, on: true);

        var r = Audit(doc);

        Assert.Equal(9, r.CliffCells);
        Assert.Equal(9, r.RampCells);
        Assert.Equal(9, r.WaterCells);
        Assert.Equal(9, r.BlightCells);
        Assert.Equal(2, r.MaxCliff);
        Assert.Equal(0, r.MinCliff);
        // A map with cliff and flags set is no longer an unedited base grid.
        Assert.DoesNotContain(r.Issues, i => i.Kind == TerrainIssueKind.AllZeroTerrain);
        Assert.DoesNotContain(r.Issues, i => i.Kind == TerrainIssueKind.UniformTerrain);
    }

    // --- uniform / distinct textures ----------------------------------------

    private static MapDocument SeededTerrain(int groundTypes)
    {
        var doc = BlankTerrain();
        var env = Env(doc);
        env.TerrainTypes.Clear();
        var all = Enum.GetValues<TerrainType>();
        for (int i = 0; i < groundTypes && i < all.Length; i++)
            env.TerrainTypes.Add(all[i]);
        return doc;
    }

    [Fact]
    public void Distinct_ground_textures_counts_every_index_in_use()
    {
        var doc = SeededTerrain(4);
        TerrainCommand.PaintRect(doc, 0, 0, 2, 2, textureIndex: 1);   // 9 corners at 1
        TerrainCommand.PaintRect(doc, 6, 6, 8, 8, textureIndex: 2);   // 9 corners at 2, rest at 0

        var r = Audit(doc);

        Assert.Equal(4, r.GroundTileTypes);
        Assert.Equal(3, r.DistinctGroundTextures);   // 0, 1 and 2 are all referenced
    }

    [Fact]
    public void A_uniform_non_zero_grid_is_flagged_as_uniform_not_all_zero()
    {
        var doc = BlankTerrain();
        TerrainCommand.DeformRect(doc, 0, 0, W - 1, W - 1, TerrainCommand.HeightOp.Set, 3f);

        var r = Audit(doc);

        Assert.Contains(r.Issues, i => i.Kind == TerrainIssueKind.UniformTerrain);
        Assert.DoesNotContain(r.Issues, i => i.Kind == TerrainIssueKind.AllZeroTerrain);
        Assert.Equal(3f, r.MinHeight);
        Assert.Equal(3f, r.MaxHeight);
    }

    // --- out-of-range tileset index -----------------------------------------

    [Fact]
    public void A_ground_texture_index_past_the_declared_count_is_flagged()
    {
        var doc = BlankTerrain();
        var env = Env(doc);
        env.TerrainTypes.Clear();
        env.TerrainTypes.Add(Enum.GetValues<TerrainType>()[0]);   // exactly one declared ground tile
        env.TerrainTiles[Idx(4, 4)].Texture = 3;                  // index 3 is past a count of 1

        var r = Audit(doc);

        Assert.Equal(1, r.GroundTileTypes);
        Assert.Contains(r.Issues, i => i.Kind == TerrainIssueKind.GroundTextureOutOfRange);
        // The check ran, because the map declares a range to check against.
        Assert.DoesNotContain(r.Diagnostics, d => d.Contains("ground-texture-range"));
    }

    // --- missing terrain ----------------------------------------------------

    [Fact]
    public void A_map_without_a_terrain_file_reports_a_diagnostic()
    {
        var doc = BlankTerrain();
        string dir = Path.Combine(Path.GetTempPath(), $"wc3audit-{Guid.NewGuid():N}");
        Directory.CreateDirectory(dir);
        try
        {
            foreach (var f in doc.Files)
            {
                if (f.FileName is null) continue;
                if (string.Equals(f.FileName, TerrainAuditCommand.TerrainFile, StringComparison.OrdinalIgnoreCase))
                    continue;   // deliberately drop the terrain file
                string path = Path.Combine(dir, f.FileName.Replace('\\', Path.DirectorySeparatorChar));
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                File.WriteAllBytes(path, f.RawBytes);
            }

            var r = TerrainAuditCommand.ExecutePath(dir);

            Assert.Empty(r.Issues);
            Assert.Contains(r.Diagnostics, d => d.Contains("no war3map.w3e"));
            Assert.Equal(0, r.TileCount);
        }
        finally
        {
            try { Directory.Delete(dir, recursive: true); } catch (IOException) { }
        }
    }
}
