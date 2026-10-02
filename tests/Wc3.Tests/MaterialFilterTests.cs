// tests/Wc3.Tests/MaterialFilterTests.cs
using System.Numerics;
using System.Text;
using Wc3.Modeling;

namespace Wc3.Tests;

/// <summary>
/// Hermetic coverage for per-geoset material filter modes: MDL FilterMode keywords
/// and MDX layer filter-mode dwords land on <see cref="Geoset.FilterMode"/>, the
/// base (first) layer decides multi-layer materials, material-less geosets stay
/// opaque, and posing preserves the mode.
/// </summary>
public class MaterialFilterTests
{
    [Theory]
    [InlineData("None", FilterMode.None)]
    [InlineData("Transparent", FilterMode.Transparent)]
    [InlineData("Blend", FilterMode.Blend)]
    [InlineData("Additive", FilterMode.Additive)]
    [InlineData("AddAlpha", FilterMode.AddAlpha)]
    [InlineData("Modulate", FilterMode.Modulate)]
    [InlineData("Modulate2x", FilterMode.Modulate2x)]
    public void Mdl_layer_filter_mode_lands_on_the_geoset(string keyword, FilterMode expected)
    {
        var model = ParseMdl(BuildMdl($"Layer {{ FilterMode {keyword}, static TextureID 0, }},"));
        Assert.Equal(expected, Assert.Single(model.Geosets).FilterMode);
    }

    [Fact]
    public void Mdl_maps_each_geoset_to_its_own_material_mode()
    {
        var model = ParseMdl(BuildMdl(
            "Layer { FilterMode None, static TextureID 0, },",
            "Layer { FilterMode Additive, static TextureID 0, },",
            "Layer { FilterMode Transparent, static TextureID 0, },"));

        Assert.Equal(3, model.Geosets.Count);
        Assert.Equal(FilterMode.None, model.Geosets[0].FilterMode);
        Assert.Equal(FilterMode.Additive, model.Geosets[1].FilterMode);
        Assert.Equal(FilterMode.Transparent, model.Geosets[2].FilterMode);
    }

    [Fact]
    public void Mdl_multi_layer_material_uses_the_base_layer()
    {
        // An opaque base with an additive overlay (glow) must stay opaque.
        var model = ParseMdl(BuildMdl(
            """
            Layer { FilterMode None, static TextureID 0, },
            Layer { FilterMode Additive, static TextureID 0, },
            """));
        Assert.Equal(FilterMode.None, Assert.Single(model.Geosets).FilterMode);
    }

    [Fact]
    public void Mdl_unknown_filter_word_degrades_to_none()
    {
        var model = ParseMdl(BuildMdl("Layer { FilterMode Sparkly, static TextureID 0, },"));
        Assert.Equal(FilterMode.None, Assert.Single(model.Geosets).FilterMode);
    }

    [Fact]
    public void Mdl_geoset_without_material_stays_none()
    {
        // No Materials block, no MaterialID — the round-trip default is opaque.
        var mdl = """
            Geoset {
                Vertices 3 { { 0, 0, 0 }, { 1, 0, 0 }, { 0, 1, 0 }, },
                Normals 3 { { 0, 0, 1 }, { 0, 0, 1 }, { 0, 0, 1 }, },
                TVertices 3 { { 0, 0 }, { 1, 0 }, { 0, 1 }, },
                Faces 1 3 { Triangles { { 0, 1, 2 }, }, },
            }
            """;
        var model = ParseMdl(mdl);
        var g = Assert.Single(model.Geosets);
        Assert.Equal(FilterMode.None, g.FilterMode);
        Assert.Equal(-1, g.TextureId);
    }

    [Theory]
    [InlineData(0u, FilterMode.None)]
    [InlineData(1u, FilterMode.Transparent)]
    [InlineData(2u, FilterMode.Blend)]
    [InlineData(3u, FilterMode.Additive)]
    [InlineData(4u, FilterMode.AddAlpha)]
    [InlineData(5u, FilterMode.Modulate)]
    [InlineData(6u, FilterMode.Modulate2x)]
    [InlineData(99u, FilterMode.None)] // out-of-range dword degrades to opaque
    public void Mdx_layer_filter_mode_lands_on_the_geoset(uint raw, FilterMode expected)
    {
        var model = ModelParser.Parse(BuildMdx(raw), "tri.mdx");
        Assert.Equal(expected, Assert.Single(model.Geosets).FilterMode);
    }

    [Fact]
    public void Mdx_multi_layer_material_uses_the_base_layer()
    {
        var model = ModelParser.Parse(BuildMdx(0u, 3u), "tri.mdx");
        var g = Assert.Single(model.Geosets);
        Assert.Equal(FilterMode.None, g.FilterMode);
        Assert.Equal(0, g.TextureId); // base-layer texture mapping is undisturbed
    }

    [Fact]
    public void Geoset_filter_mode_defaults_to_none()
    {
        // Existing call sites construct Geoset without the new parameter.
        var g = new Geoset([], [], [], [], -1);
        Assert.Equal(FilterMode.None, g.FilterMode);
    }

    [Fact]
    public void Posing_preserves_the_geoset_filter_mode()
    {
        // One bone, 90° about Z at Stand's first frame — enough for the poser to
        // actually rebuild the geoset rather than hand back the same instance.
        var rotation = new AnimTrack<Quaternion>(
            new[] { new AnimKey<Quaternion>(0, Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI / 2)) }, -1);
        var skeleton = new ModelSkeleton(
            new[] { new ModelSequence("Stand", 0, 100) },
            new[] { new ModelNode("bone", 0, -1, Vector3.Zero, null, rotation, null) });
        var geoset = new Geoset(
            new float[] { 1, 0, 0, 2, 0, 0, 1, 1, 0 },
            new float[] { 0, 0, 1, 0, 0, 1, 0, 0, 1 },
            new float[] { 0, 0, 1, 0, 0, 1 },
            new[] { 0, 1, 2 },
            -1,
            SkinBones: new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            SkinWeights: new float[] { 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0 },
            FilterMode: FilterMode.Additive);
        var model = new Model3D(new[] { geoset }, Array.Empty<string>(), skeleton);

        var posed = model.PosedAt();

        Assert.NotSame(model, posed); // the pose ran and rebuilt the geoset
        var posedGeoset = Assert.Single(posed.Geosets);
        Assert.False(posedGeoset.Vertices.SequenceEqual(geoset.Vertices));
        Assert.Equal(FilterMode.Additive, posedGeoset.FilterMode);
    }

    private static Model3D ParseMdl(string text)
        => ModelParser.Parse(Encoding.UTF8.GetBytes(text), "test.mdl");

    /// <summary>One texture, one material per layer-list, one triangle geoset per material.</summary>
    private static string BuildMdl(params string[] materialLayers)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Version { FormatVersion 800, }");
        sb.AppendLine("Textures 1 { Bitmap { Image \"Textures\\Test.blp\", }, }");
        sb.AppendLine($"Materials {materialLayers.Length} {{");
        foreach (var layers in materialLayers)
            sb.AppendLine($"    Material {{ {layers} }},");
        sb.AppendLine("}");
        for (int i = 0; i < materialLayers.Length; i++)
        {
            sb.AppendLine($$"""
                Geoset {
                    Vertices 3 { { 0, 0, 0 }, { 1, 0, 0 }, { 0, 1, 0 }, },
                    Normals 3 { { 0, 0, 1 }, { 0, 0, 1 }, { 0, 0, 1 }, },
                    TVertices 3 { { 0, 0 }, { 1, 0 }, { 0, 1 }, },
                    Faces 1 3 { Triangles { { 0, 1, 2 }, }, },
                    MaterialID {{i}},
                }
                """);
        }
        return sb.ToString();
    }

    /// <summary>
    /// Minimal v800 MDLX (VERS + TEXS + MTLS + GEOS, one triangle referencing
    /// material 0) whose single material carries one 28-byte layer per entry in
    /// <paramref name="layerFilterModes"/>.
    /// </summary>
    private static byte[] BuildMdx(params uint[] layerFilterModes)
    {
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        w.Write("MDLX"u8);

        w.Write("VERS"u8);
        w.Write(4u);
        w.Write(800u);

        // TEXS: one 268-byte entry
        w.Write("TEXS"u8);
        w.Write(268u);
        w.Write(0u); // replaceableId
        var path = Encoding.ASCII.GetBytes(@"Textures\Test.blp");
        w.Write(path);
        w.Write(new byte[260 - path.Length]);
        w.Write(0u); // flags

        // MTLS: one material (layer = 28 bytes incl. its size field)
        const uint layerSize = 28u;
        uint matSize = 4 + 4 + 4 + 8 + layerSize * (uint)layerFilterModes.Length;
        w.Write("MTLS"u8);
        w.Write(matSize);
        w.Write(matSize);        // material inclusiveSize
        w.Write(0u);             // priorityPlane
        w.Write(0u);             // flags
        w.Write("LAYS"u8);
        w.Write((uint)layerFilterModes.Length);
        foreach (var mode in layerFilterModes)
        {
            w.Write(layerSize);      // layer inclusiveSize
            w.Write(mode);           // filterMode
            w.Write(0u);             // shadingFlags
            w.Write(0u);             // textureId
            w.Write(0xFFFFFFFFu);    // textureAnimationId (none)
            w.Write(0u);             // coordId
            w.Write(1.0f);           // alpha
        }

        // GEOS: one geoset built in its own buffer so the inclusive size is exact.
        using var gs = new MemoryStream();
        using var gw = new BinaryWriter(gs);
        gw.Write("VRTX"u8);
        gw.Write(3u);
        foreach (var f in new float[] { 0, 0, 0, 1, 0, 0, 0, 1, 0 }) gw.Write(f);
        gw.Write("NRMS"u8);
        gw.Write(3u);
        foreach (var f in new float[] { 0, 0, 1, 0, 0, 1, 0, 0, 1 }) gw.Write(f);
        gw.Write("PTYP"u8);
        gw.Write(1u);
        gw.Write(4u); // triangles
        gw.Write("PCNT"u8);
        gw.Write(1u);
        gw.Write(3u);
        gw.Write("PVTX"u8);
        gw.Write(3u);
        gw.Write((ushort)0);
        gw.Write((ushort)1);
        gw.Write((ushort)2);
        gw.Write("GNDX"u8);
        gw.Write(3u);
        gw.Write(new byte[] { 0, 0, 0 });
        gw.Write("MTGC"u8);
        gw.Write(1u);
        gw.Write(1u);
        gw.Write("MATS"u8);
        gw.Write(1u);
        gw.Write(0u);
        gw.Write(0u);            // materialId
        gw.Write(0u);            // selectionGroup
        gw.Write(0u);            // selectionFlags
        foreach (var f in new float[] { 2, -1, -1, 0, 1, 1, 0 }) gw.Write(f); // radius + min + max
        gw.Write(0u);            // per-sequence extent count
        gw.Write("UVAS"u8);
        gw.Write(1u);            // UV set count
        gw.Write("UVBS"u8);
        gw.Write(3u);
        foreach (var f in new float[] { 0, 0, 1, 0, 0, 1 }) gw.Write(f);
        gw.Flush();

        var geoset = gs.ToArray();
        w.Write("GEOS"u8);
        w.Write((uint)(geoset.Length + 4));
        w.Write((uint)(geoset.Length + 4)); // geoset inclusiveSize
        w.Write(geoset);

        w.Flush();
        return ms.ToArray();
    }
}
