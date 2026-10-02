// tests/Wc3.Tests/PlacementSceneTests.cs
using System.Numerics;
using War3Net.Build.Widget;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;
using Wc3.Render;
using Xunit;

namespace Wc3.Tests;

public class PlacementSceneTests
{
    [Fact]
    public void Build_MapWithoutPlacements_ProducesEmptyScene()
    {
        var doc = BlankMap.Create(); // blank map has terrain but no widget files
        var scene = PlacementScene.Build(doc);

        Assert.Equal(0, scene.UnitCount);
        Assert.Equal(0, scene.DoodadCount);
        Assert.Empty(scene.Meshes);
        Assert.Empty(scene.Instances);
    }

    [Fact]
    public void Build_WithoutResolver_SharesKindColoredBoxMeshes()
    {
        var doc = BlankMap.Create();
        Assert.True(PlacementCommand.PlaceUnit(doc, "hfoo", ownerId: 0, x: 128f, y: -256f).Ok);
        Assert.True(PlacementCommand.PlaceUnit(doc, "hkni", ownerId: 0, x: 300f, y: 400f).Ok);
        Assert.True(PlacementCommand.PlaceDoodad(doc, "LTlt", x: -512f, y: 64f).Ok);

        var scene = PlacementScene.Build(doc);

        Assert.Equal(2, scene.UnitCount);
        Assert.Equal(1, scene.DoodadCount);
        Assert.Equal(3, scene.Instances.Count);

        // Two unit TYPES without models share ONE unit box mesh; the doodad gets the other.
        Assert.Equal(2, scene.Meshes.Count);
        Assert.Contains(scene.Meshes, m => m.Key == PlacementScene.UnitBoxKey);
        Assert.Contains(scene.Meshes, m => m.Key == PlacementScene.DoodadBoxKey);

        // Boxes are real 3D meshes (24 verts / 36 indices), untextured single section,
        // colored per kind, based at z=0.
        var unitBox = Assert.Single(scene.Meshes, m => m.Key == PlacementScene.UnitBoxKey);
        Assert.Equal(24, unitBox.VertexCount);
        Assert.Equal(36, unitBox.Indices.Length);
        var boxSection = Assert.Single(unitBox.Sections);
        Assert.Equal(-1, boxSection.TextureSlot);
        Assert.Equal(36, boxSection.IndexCount);
        Assert.Empty(unitBox.Textures);
        AssertMeshHasColor(unitBox, PlacementScene.UnitColor);
        var doodadBox = Assert.Single(scene.Meshes, m => m.Key == PlacementScene.DoodadBoxKey);
        AssertMeshHasColor(doodadBox, PlacementScene.DoodadColor);

        // Instances stand on the terrain at their placement (x, y).
        var heights = TerrainHeightField.TryCreate(doc);
        Assert.NotNull(heights);
        var unitInstance = scene.Instances[0];
        Assert.Equal(PlacementScene.UnitBoxKey, unitInstance.MeshKey);
        var t = unitInstance.World.Translation;
        Assert.Equal(128f, t.X, 2);
        Assert.Equal(-256f, t.Y, 2);
        Assert.Equal(heights!.Sample(128f, -256f), t.Z, 2);
    }

    [Fact]
    public void Build_WithResolver_UploadsUniqueModelOnce_AndInstancesEachPlacement()
    {
        var doc = BlankMap.Create();
        Assert.True(PlacementCommand.PlaceUnit(doc, "hfoo", ownerId: 0, x: 256f, y: 512f).Ok);
        Assert.True(PlacementCommand.PlaceUnit(doc, "hfoo", ownerId: 0, x: -640f, y: 0f).Ok);

        var pm = new PlacementModel(SingleTriangleModel(), new Dictionary<int, Wc3.Modeling.TextureImage>());
        int calls = 0;
        var scene = PlacementScene.Build(doc, (rawcode, isUnit) =>
        {
            calls++;
            Assert.Equal("hfoo", rawcode);
            Assert.True(isUnit);
            return pm;
        });

        // The unique model resolves ONCE and both placements share the one mesh.
        Assert.Equal(1, calls);
        var mesh = Assert.Single(scene.Meshes);
        Assert.Equal("unit:hfoo", mesh.Key);
        Assert.Equal(3, mesh.VertexCount);               // the triangle, not the 24-vert box
        Assert.Equal(new uint[] { 0, 1, 2 }, mesh.Indices);
        Assert.Equal(2, scene.Instances.Count);
        Assert.All(scene.Instances, i => Assert.Equal("unit:hfoo", i.MeshKey));

        // Mesh vertices stay in model space; the instance transform carries the position.
        Assert.Equal(0f, mesh.Vertices[0], 3);
        Assert.Equal(0f, mesh.Vertices[1], 3);
        var heights = TerrainHeightField.TryCreate(doc);
        Assert.NotNull(heights);
        var t = scene.Instances[0].World.Translation;
        Assert.Equal(256f, t.X, 2);
        Assert.Equal(512f, t.Y, 2);
        Assert.Equal(heights!.Sample(256f, 512f), t.Z, 2);
    }

    [Fact]
    public void BuildModelMesh_CarriesUvs_AndBindsGeosetTextures()
    {
        var red = new Wc3.Modeling.TextureImage(2, 2, new byte[]
        {
            255, 0, 0, 255,  255, 0, 0, 255,
            255, 0, 0, 255,  255, 0, 0, 255,
        });
        var pm = new PlacementModel(
            SingleTriangleModel(textureId: 0),
            new Dictionary<int, Wc3.Modeling.TextureImage> { [0] = red });

        var mesh = PlacementScene.BuildModelMesh("unit:test", pm);
        Assert.NotNull(mesh);

        // The geoset's texture lands in the mesh and its section points at it.
        var texture = Assert.Single(mesh!.Textures);
        Assert.Same(red, texture);
        var section = Assert.Single(mesh.Sections);
        Assert.Equal(0, section.TextureSlot);
        Assert.Equal(3, section.IndexCount);
        Assert.Equal(0, section.IndexOffset);

        // Vertices carry the geoset's UVs at stride offset 6..7 (pos3 + normal3 + uv2 + color3).
        Assert.Equal(11, PlacementMesh.Stride);
        Assert.Equal(0f, mesh.Vertices[6], 3);   // v0 uv = (0, 0)
        Assert.Equal(0f, mesh.Vertices[7], 3);
        Assert.Equal(1f, mesh.Vertices[PlacementMesh.Stride + 6], 3);  // v1 uv = (1, 0)
        Assert.Equal(0f, mesh.Vertices[PlacementMesh.Stride + 7], 3);
        Assert.Equal(0f, mesh.Vertices[2 * PlacementMesh.Stride + 6], 3); // v2 uv = (0, 1)
        Assert.Equal(1f, mesh.Vertices[2 * PlacementMesh.Stride + 7], 3);
    }

    [Fact]
    public void BuildModelMesh_GeosetWithoutTexture_GetsNeutralUntexturedSection()
    {
        var pm = new PlacementModel(SingleTriangleModel(), new Dictionary<int, Wc3.Modeling.TextureImage>());

        var mesh = PlacementScene.BuildModelMesh("unit:test", pm);
        Assert.NotNull(mesh);

        Assert.Empty(mesh!.Textures);
        var section = Assert.Single(mesh.Sections);
        Assert.Equal(-1, section.TextureSlot);
        AssertMeshHasColor(mesh, PlacementScene.UntexturedColor);
    }

    [Fact]
    public void Build_TeamColorEffectModel_IsSkipped()
    {
        var doc = BlankMap.Create();
        Assert.True(PlacementCommand.PlaceDoodad(doc, "LTlt", x: 0f, y: 0f).Ok);

        // A glow plane like GeneralHeroGlow: one geoset referencing a ReplaceableId
        // (team-color) texture, none decoded — nothing we can draw but a white quad, so skip.
        var geoset = new Wc3.Modeling.Geoset(
            Vertices: new float[] { 0f, 0f, 0f, 32f, 0f, 0f, 0f, 0f, 64f },
            Normals: new float[] { 0f, 0f, 1f, 0f, 0f, 1f, 0f, 0f, 1f },
            Uvs: new float[] { 0f, 0f, 1f, 0f, 0f, 1f },
            Indices: new[] { 0, 1, 2 },
            TextureId: 0);
        var glow = new PlacementModel(
            new Wc3.Modeling.Model3D(new[] { geoset }, new[] { "ReplaceableId:2" }),
            new Dictionary<int, Wc3.Modeling.TextureImage>()); // nothing decoded

        var scene = PlacementScene.Build(doc, (_, _) => glow);
        Assert.Empty(scene.Instances);
        Assert.Empty(scene.Meshes);
    }

    [Fact]
    public void IsTeamColorEffect_OnlyWhenNoRealTextureAnywhere()
    {
        Wc3.Modeling.Geoset Geo(int textureId) => new(
            Vertices: new float[] { 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f },
            Normals: new float[] { 0f, 0f, 1f, 0f, 0f, 1f, 0f, 0f, 1f },
            Uvs: new float[] { 0f, 0f, 1f, 0f, 0f, 1f },
            Indices: new[] { 0, 1, 2 },
            TextureId: textureId);

        // All-replaceable → an effect plane.
        var glow = new PlacementModel(
            new Wc3.Modeling.Model3D(new[] { Geo(0) }, new[] { "ReplaceableId:2" }),
            new Dictionary<int, Wc3.Modeling.TextureImage>());
        Assert.True(PlacementScene.IsTeamColorEffect(glow));

        // One real decoded texture alongside a replaceable one → a normal model, NOT an effect.
        var tex = new Wc3.Modeling.TextureImage(1, 1, new byte[] { 255, 255, 255, 255 });
        var mixed = new PlacementModel(
            new Wc3.Modeling.Model3D(new[] { Geo(0), Geo(1) },
                new[] { "units\\real.blp", "ReplaceableId:2" }),
            new Dictionary<int, Wc3.Modeling.TextureImage> { [0] = tex });
        Assert.False(PlacementScene.IsTeamColorEffect(mixed));

        // No textures at all (flat model) is not a team-color effect — it renders gray.
        var flat = new PlacementModel(
            new Wc3.Modeling.Model3D(new[] { Geo(-1) }, Array.Empty<string>()),
            new Dictionary<int, Wc3.Modeling.TextureImage>());
        Assert.False(PlacementScene.IsTeamColorEffect(flat));
    }

    [Fact]
    public void Build_PlacementRotation_LandsInTheInstanceTransform()
    {
        var doc = BlankMap.Create();
        Assert.True(PlacementCommand.PlaceUnit(
            doc, "hfoo", ownerId: 0, x: 0f, y: 0f, rotation: MathF.PI / 2f).Ok);

        var scene = PlacementScene.Build(doc);
        var world = Assert.Single(scene.Instances).World;

        // Row-vector CreateRotationZ(π/2): M11 = cos ≈ 0, M12 = sin ≈ 1.
        Assert.Equal(0f, world.M11, 3);
        Assert.Equal(1f, world.M12, 3);
    }

    [Fact]
    public void Build_ResolverReturningEmptyModel_SkipsThePlacement()
    {
        var doc = BlankMap.Create();
        Assert.True(PlacementCommand.PlaceUnit(doc, "hfoo", ownerId: 0, x: 0f, y: 0f).Ok);

        // A "model" with no triangles has nothing to draw (a particle/light-only widget like
        // a bubble geyser). With a real resolver supplied, it is SKIPPED — not boxed — so the
        // viewport never shows a placeholder where a model was actually resolved.
        var empty = new PlacementModel(
            new Wc3.Modeling.Model3D(
                new[]
                {
                    new Wc3.Modeling.Geoset(
                        Vertices: Array.Empty<float>(),
                        Normals: Array.Empty<float>(),
                        Uvs: Array.Empty<float>(),
                        Indices: Array.Empty<int>(),
                        TextureId: -1),
                },
                Array.Empty<string>()),
            new Dictionary<int, Wc3.Modeling.TextureImage>());

        var scene = PlacementScene.Build(doc, (_, _) => empty);
        Assert.Empty(scene.Meshes);
        Assert.Empty(scene.Instances);
    }

    [Fact]
    public void HeightField_FlatMap_SamplesUniformly_AndRaycastHitsGround()
    {
        var doc = BlankMap.Create();
        var heights = TerrainHeightField.TryCreate(doc);
        Assert.NotNull(heights);

        // A blank map is flat: any two samples agree.
        Assert.Equal(heights!.Sample(0f, 0f), heights.Sample(500f, -700f), 3);
        Assert.Equal(heights.MinZ, heights.MaxZ, 3);

        // A ray straight down from above the terrain strikes the ground beneath the eye.
        var eye = new Vector3(100f, 200f, heights.MaxZ + 2000f);
        var (ok, wx, wy) = heights.RaycastGround(eye, new Vector3(0f, 0f, -1f));
        Assert.True(ok);
        Assert.Equal(100f, wx, 1);
        Assert.Equal(200f, wy, 1);

        // An ascending ray can never hit the ground.
        var (miss, _, _) = heights.RaycastGround(eye, new Vector3(0f, 0f, 1f));
        Assert.False(miss);
    }

    [Fact]
    public void HeightField_MapWithoutTerrain_IsNull()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = new byte[] { 1, 2 },
        }));
        Assert.Null(TerrainHeightField.TryCreate(doc));
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Build_RealMap_ProducesInstancesForItsPlacements()
    {
        string path = TestCorpus.Map(@"ggg_en_1.11r_slk.w3x");
        if (!File.Exists(path)) return;

        var doc = MapDocument.Load(path);
        var unitList = (doc.GetFile("war3mapUnits.doo")?.Model as MapUnits)?.Units;
        int units = unitList?.Count ?? 0;
        int doodads = (doc.GetFile("war3map.doo")?.Model as MapDoodads)?.Doodads.Count ?? 0;
        // Start-location markers carry no model and are skipped, so they never become instances.
        int startLocs = unitList?.Count(u =>
            string.Equals(u.TypeId.ToRawcode(), PlacementScene.StartLocationRawcode,
                StringComparison.OrdinalIgnoreCase)) ?? 0;

        var scene = PlacementScene.Build(doc);
        Assert.Equal(units, scene.UnitCount);
        Assert.Equal(doodads, scene.DoodadCount);
        // No resolver: every non-start-location placement gets a box instance.
        Assert.Equal(units - startLocs + doodads, scene.Instances.Count);
        if (units + doodads > 0)
            Assert.NotEmpty(scene.Meshes);
    }

    /// <summary>A minimal model: one right triangle rising from the local origin, with
    /// simple corner UVs (0,0) (1,0) (0,1).</summary>
    private static Wc3.Modeling.Model3D SingleTriangleModel(int textureId = -1)
    {
        var geoset = new Wc3.Modeling.Geoset(
            Vertices: new float[] { 0f, 0f, 0f, 32f, 0f, 0f, 0f, 0f, 64f },
            Normals: new float[] { 0f, -1f, 0f, 0f, -1f, 0f, 0f, -1f, 0f },
            Uvs: new float[] { 0f, 0f, 1f, 0f, 0f, 1f },
            Indices: new[] { 0, 1, 2 },
            TextureId: textureId);
        return new Wc3.Modeling.Model3D(new[] { geoset }, Array.Empty<string>());
    }

    /// <summary>Asserts some vertex in the mesh carries exactly this color
    /// (stride layout pos3 + normal3 + uv2 + color3 → color at offsets 8..10).</summary>
    private static void AssertMeshHasColor(PlacementMesh mesh, Vector3 color)
    {
        for (int i = 0; i < mesh.Vertices.Length; i += PlacementMesh.Stride)
        {
            if (MathF.Abs(mesh.Vertices[i + 8] - color.X) < 1e-4f &&
                MathF.Abs(mesh.Vertices[i + 9] - color.Y) < 1e-4f &&
                MathF.Abs(mesh.Vertices[i + 10] - color.Z) < 1e-4f)
                return;
        }
        Assert.Fail($"mesh '{mesh.Key}' carries no vertex with color {color}");
    }
}
