// tests/Wc3.Tests/PlacementPickTests.cs
using System.Numerics;
using Wc3.Commands;
using Wc3.Model;
using Wc3.Modeling;
using Wc3.Render;
using Xunit;

namespace Wc3.Tests;

/// <summary>Placement-scene metadata the Studio viewport needs beyond raw geometry:
/// per-instance identity (owner, creation number) for pick/tint, per-mesh local AABBs
/// for ray-pick, and per-section blend/team-color flags for the shader.</summary>
public class PlacementPickTests
{
    [Fact]
    public void Build_UnitInstance_CarriesOwnerAndCreationNumber()
    {
        var doc = BlankMap.Create();
        var placed = PlacementCommand.PlaceUnit(doc, "hfoo", ownerId: 3, x: 128f, y: -256f);
        Assert.True(placed.Ok);

        var scene = PlacementScene.Build(doc);

        var instance = Assert.Single(scene.Instances);
        Assert.Equal(3, instance.OwnerId);
        Assert.Equal(placed.CreationNumber, instance.CreationNumber);
        Assert.True(instance.IsUnit);
    }

    [Fact]
    public void Build_DoodadInstance_CarriesCreationNumber_ButNoOwner()
    {
        // Doodads now carry their war3map.doo creation number (so the viewport can pick and
        // edit them, like units) but have no owner and are flagged IsUnit = false.
        var doc = BlankMap.Create();
        var placed = PlacementCommand.PlaceDoodad(doc, "LTlt", x: -512f, y: 64f);
        Assert.True(placed.Ok);

        var scene = PlacementScene.Build(doc);

        var instance = Assert.Single(scene.Instances);
        Assert.Equal(-1, instance.OwnerId);
        Assert.Equal(placed.CreationNumber, instance.CreationNumber);
        Assert.False(instance.IsUnit);
    }

    [Fact]
    public void BuildModelMesh_BoundsEncloseTheVertices()
    {
        var pm = new PlacementModel(
            new Model3D(new[] { TriangleGeoset() }, System.Array.Empty<string>()),
            new Dictionary<int, TextureImage>());

        var mesh = PlacementScene.BuildModelMesh("unit:test", pm);
        Assert.NotNull(mesh);

        // Triangle corners (0,0,0) (32,0,0) (0,0,64) -> tight local AABB.
        Assert.Equal(Vector3.Zero, mesh!.BoundsMin);
        Assert.Equal(new Vector3(32f, 0f, 64f), mesh.BoundsMax);
    }

    [Fact]
    public void BuildBoxMesh_BoundsMatchItsVertexPositions()
    {
        var mesh = PlacementScene.BuildBoxMesh(PlacementScene.UnitBoxKey, PlacementScene.UnitColor);

        var min = new Vector3(float.MaxValue);
        var max = new Vector3(float.MinValue);
        for (int i = 0; i < mesh.Vertices.Length; i += PlacementMesh.Stride)
        {
            var p = new Vector3(mesh.Vertices[i], mesh.Vertices[i + 1], mesh.Vertices[i + 2]);
            min = Vector3.Min(min, p);
            max = Vector3.Max(max, p);
        }
        Assert.Equal(min, mesh.BoundsMin);
        Assert.Equal(max, mesh.BoundsMax);
        Assert.True(mesh.BoundsMax.X > mesh.BoundsMin.X); // a real volume, not degenerate
        Assert.Equal(0f, mesh.BoundsMin.Z, 3);            // box base sits on local Z=0
    }

    [Fact]
    public void BuildModelMesh_SectionCarriesTheGeosetFilterMode()
    {
        var pm = new PlacementModel(
            new Model3D(
                new[] { TriangleGeoset(filterMode: FilterMode.Additive) },
                System.Array.Empty<string>()),
            new Dictionary<int, TextureImage>());

        var mesh = PlacementScene.BuildModelMesh("unit:test", pm);
        Assert.NotNull(mesh);

        var section = Assert.Single(mesh!.Sections);
        Assert.Equal(FilterMode.Additive, section.FilterMode);
    }

    [Fact]
    public void BuildModelMesh_UndecodedReplaceableSection_IsTeamColor_TexturedIsNot()
    {
        // One team-color geoset (ReplaceableId, undecoded) alongside one real textured
        // geoset — a normal unit with team-colored trim, NOT a skipped effect plane.
        var tex = new TextureImage(1, 1, new byte[] { 255, 255, 255, 255 });
        var pm = new PlacementModel(
            new Model3D(
                new[] { TriangleGeoset(textureId: 0), TriangleGeoset(textureId: 1) },
                new[] { "ReplaceableId:1", "units\\real.blp" }),
            new Dictionary<int, TextureImage> { [1] = tex });
        Assert.False(PlacementScene.IsTeamColorEffect(pm));

        var mesh = PlacementScene.BuildModelMesh("unit:test", pm);
        Assert.NotNull(mesh);
        Assert.Equal(2, mesh!.Sections.Count);

        var teamColor = mesh.Sections[0];
        Assert.True(teamColor.IsTeamColor);
        Assert.Equal(-1, teamColor.TextureSlot); // undecoded — no texture bound

        var textured = mesh.Sections[1];
        Assert.False(textured.IsTeamColor);
        Assert.Equal(0, textured.TextureSlot);

        // Team-color vertices are white (not the untextured gray) so the owner tint lands pure.
        Assert.Equal(1f, mesh.Vertices[8], 3);
        Assert.Equal(1f, mesh.Vertices[9], 3);
        Assert.Equal(1f, mesh.Vertices[10], 3);
    }

    [Fact]
    public void BuildModelMesh_UntexturedNonReplaceableSection_IsNotTeamColor()
    {
        var pm = new PlacementModel(
            new Model3D(new[] { TriangleGeoset() }, System.Array.Empty<string>()),
            new Dictionary<int, TextureImage>());

        var mesh = PlacementScene.BuildModelMesh("unit:test", pm);
        Assert.NotNull(mesh);

        var section = Assert.Single(mesh!.Sections);
        Assert.False(section.IsTeamColor);
        Assert.Equal(FilterMode.None, section.FilterMode);
    }

    /// <summary>One right triangle rising from the local origin: corners
    /// (0,0,0) (32,0,0) (0,0,64), UVs (0,0) (1,0) (0,1).</summary>
    private static Geoset TriangleGeoset(int textureId = -1, FilterMode filterMode = FilterMode.None) => new(
        Vertices: new float[] { 0f, 0f, 0f, 32f, 0f, 0f, 0f, 0f, 64f },
        Normals: new float[] { 0f, -1f, 0f, 0f, -1f, 0f, 0f, -1f, 0f },
        Uvs: new float[] { 0f, 0f, 1f, 0f, 0f, 1f },
        Indices: new[] { 0, 1, 2 },
        TextureId: textureId,
        FilterMode: filterMode);
}
