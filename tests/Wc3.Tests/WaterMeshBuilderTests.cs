using Wc3.Commands;
using Wc3.Model;
using Wc3.Render;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// Hermetic tests for <see cref="WaterMeshBuilder"/>: water quads appear over sculpted
/// water cells in the terrain mesh's world coordinates at the stored water level plus
/// the datum offset, and the mesh is empty when the map has no water. Uses the same
/// 8x8-tile blank map as the water-brush tests.
/// </summary>
public class WaterMeshBuilderTests
{
    private const int TileEdge = 8;
    private const float StepWorld = 128f;

    private static MapDocument BlankTerrain()
        => BlankMap.Create(new BlankMapOptions { TileEdge = TileEdge });

    [Fact]
    public void No_water_yields_an_empty_mesh()
    {
        var mesh = WaterMeshBuilder.Build(BlankTerrain());
        Assert.Empty(mesh.Vertices);
        Assert.Empty(mesh.Indices);
    }

    [Fact]
    public void Sculpted_water_yields_a_quad_at_the_stored_level()
    {
        var doc = BlankTerrain();
        var r = TerrainCommand.Water(doc, 4, 4, radius: 0, TerrainCommand.WaterOp.Set, amount: 2f);
        Assert.True(r.Ok, r.Message);

        var mesh = WaterMeshBuilder.Build(doc);

        // One water corner anchors exactly one cell: a quad of 4 vertices and 6 indices.
        Assert.Equal(4 * WaterMesh.Stride, mesh.Vertices.Length);
        Assert.Equal(6, mesh.Indices.Length);

        // Every vertex Z sits at the stored level times the world step plus the datum
        // offset (the SW corner is the only water corner, so all four share its level).
        float expectedZ = 2f * StepWorld + WaterMeshBuilder.WaterZOffset;
        for (int v = 0; v < 4; v++)
            Assert.Equal(expectedZ, mesh.Vertices[v * WaterMesh.Stride + 2], 3);

        // The quad covers the cell anchored at corner (4,4): X and Y span one 128-unit
        // tile from that corner's world position (the grid is centred on the origin).
        float origin = -TileEdge * (StepWorld / 2f);
        float x0 = origin + 4 * StepWorld, y0 = origin + 4 * StepWorld;
        Assert.Equal(x0, mesh.Vertices[0], 3);
        Assert.Equal(y0, mesh.Vertices[1], 3);
        Assert.Equal(x0 + StepWorld, mesh.Vertices[WaterMesh.Stride + 0], 3);
        Assert.Equal(y0 + StepWorld, mesh.Vertices[3 * WaterMesh.Stride + 1], 3);
    }

    [Fact]
    public void Indices_stay_within_the_vertex_range()
    {
        var doc = BlankTerrain();
        TerrainCommand.Water(doc, 4, 4, radius: 2, TerrainCommand.WaterOp.Set, amount: 1f);
        var mesh = WaterMeshBuilder.Build(doc);

        Assert.True(mesh.Indices.Length > 0);
        Assert.Equal(0, mesh.Indices.Length % 6);
        uint vertexCount = (uint)(mesh.Vertices.Length / WaterMesh.Stride);
        Assert.All(mesh.Indices, i => Assert.True(i < vertexCount));
    }

    [Fact]
    public void Neighbouring_water_corners_carry_their_own_level()
    {
        var doc = BlankTerrain();
        // Two adjacent water corners at different levels: the cell anchored at (4,4)
        // reads level 1 at its SW corner and level 3 at its SE corner (5,4).
        TerrainCommand.Water(doc, 4, 4, 0, TerrainCommand.WaterOp.Set, 1f);
        TerrainCommand.Water(doc, 5, 4, 0, TerrainCommand.WaterOp.Set, 3f);

        var mesh = WaterMeshBuilder.Build(doc);

        // Two cells, one anchored by each watered corner.
        Assert.Equal(2 * 4 * WaterMesh.Stride, mesh.Vertices.Length);

        // The first emitted cell is the one anchored at (4,4): its SW vertex sits at
        // level 1 and its SE vertex picks up the neighbouring corner's own level 3.
        float z1 = 1f * StepWorld + WaterMeshBuilder.WaterZOffset;
        float z3 = 3f * StepWorld + WaterMeshBuilder.WaterZOffset;
        Assert.Equal(z1, mesh.Vertices[2], 3);
        Assert.Equal(z3, mesh.Vertices[WaterMesh.Stride + 2], 3);
    }

    [Fact]
    public void Terrain_relative_flood_sits_above_the_brushed_ground()
    {
        // The Studio water brush treats its Amount as a DEPTH above the ground and converts
        // to a stored level via WaterZOffset (see TerrainView.ApplySculptAt). Reproduce that
        // exact math and assert the rendered water surface lands depth*128 world units above
        // the ground, never below it. This is the regression guard for the bug where a fresh
        // Water stroke rendered under the terrain and looked like nothing happened.
        var doc = BlankTerrain();
        const int col = 4, row = 4;
        const float depth = 0.5f;

        float ground = TerrainCommand.GroundLevelAt(doc, col, row)!.Value;
        float waterLevel = ground + depth - WaterMeshBuilder.WaterZOffset / StepWorld;
        var r = TerrainCommand.Water(doc, col, row, radius: 0, TerrainCommand.WaterOp.Set, waterLevel);
        Assert.True(r.Ok, r.Message);

        var mesh = WaterMeshBuilder.Build(doc);
        Assert.NotEmpty(mesh.Indices);

        float groundZ = ground * StepWorld;                                  // terrain-mesh world Z
        float waterZ = mesh.Vertices[2];                                     // SW corner surface Z
        // Within one w3e height-quantum: WaterHeight stores on a 1/512 grid, so the surface
        // can land up to a fraction of a world unit off the exact target.
        Assert.True(Math.Abs(waterZ - (groundZ + depth * StepWorld)) < 1f,
            $"water surface {waterZ} should sit {depth * StepWorld} above ground {groundZ}");
        Assert.True(waterZ > groundZ, "flooded water must render above the ground it was brushed onto");
    }

    [Fact]
    public void Drained_water_empties_the_mesh_again()
    {
        var doc = BlankTerrain();
        TerrainCommand.Water(doc, 4, 4, 1, TerrainCommand.WaterOp.Set, 2f);
        Assert.NotEmpty(WaterMeshBuilder.Build(doc).Indices);

        TerrainCommand.Water(doc, 4, 4, 1, TerrainCommand.WaterOp.Remove);
        Assert.Empty(WaterMeshBuilder.Build(doc).Indices);
    }
}
