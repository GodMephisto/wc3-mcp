// tests/Wc3.Tests/ModelAnimationTests.cs
using System.Buffers.Binary;
using System.Text;
using Wc3.Commands;
using Wc3.Model;
using Wc3.Modeling;
using Wc3.Render;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Skeletal animation coverage: hermetic tests drive the MDX animation chunks
/// (SEQS/BONE/PIVT + KGRT + GNDX/MTGC/MATS) through a hand-built binary model
/// and check the posed math exactly; corpus tests confirm real map models parse
/// a skeleton and render their Stand pose.
/// </summary>
public class ModelAnimationTests(ITestOutputHelper output)
{
    private static readonly string AnimeMap =
        TestCorpus.Map(@"Anime_WOS2_0.25c1.w3x");

    private static readonly byte[] PngSignature = { 0x89, (byte)'P', (byte)'N', (byte)'G', 0x0D, 0x0A, 0x1A, 0x0A };

    private static readonly IReadOnlyDictionary<int, TextureImage> NoTextures =
        new Dictionary<int, TextureImage>();

    [Fact]
    public void Parses_sequences_bones_pivots_and_vertex_groups_from_mdx()
    {
        var model = ModelParser.Parse(BuildAnimatedMdx(), "synthetic.mdx");

        var skeleton = model.Skeleton;
        Assert.NotNull(skeleton);
        var sequence = Assert.Single(skeleton!.Sequences);
        Assert.Equal("Stand", sequence.Name);
        Assert.Equal(100, sequence.IntervalStart);
        Assert.Equal(200, sequence.IntervalEnd);

        var node = Assert.Single(skeleton.Nodes);
        Assert.Equal("bone", node.Name);
        Assert.Equal(0, node.ObjectId);
        Assert.Equal(-1, node.ParentId);
        Assert.NotNull(node.Rotation);
        var key = Assert.Single(node.Rotation!.Keys);
        Assert.Equal(100, key.Frame);

        var geoset = Assert.Single(model.Geosets);
        Assert.NotNull(geoset.SkinBones);
        Assert.NotNull(geoset.SkinWeights);
        Assert.Equal(12, geoset.SkinBones!.Length);  // 3 vertices x 4 slots
        Assert.Equal(0, geoset.SkinBones[0]);        // group 0 -> bone objectId 0
        Assert.Equal(1f, geoset.SkinWeights![0]);    // single-bone group -> full weight
    }

    [Fact]
    public void Stand_pose_rotates_vertices_around_the_bone()
    {
        var model = ModelParser.Parse(BuildAnimatedMdx(), "synthetic.mdx");

        var posed = model.PosedAt();
        Assert.NotSame(model, posed);
        Assert.Null(posed.Skeleton); // posed output is static — re-posing must be a no-op
        Assert.Same(posed, posed.PosedAt());

        // The bone's only key (at Stand's first frame) is a 90° rotation about Z:
        // (1,0,0)->(0,1,0), (2,0,0)->(0,2,0), (1,1,0)->(-1,1,0). Normals (0,0,1) stay put.
        var v = Assert.Single(posed.Geosets).Vertices;
        AssertVertex(v, 0, 0f, 1f, 0f);
        AssertVertex(v, 1, 0f, 2f, 0f);
        AssertVertex(v, 2, -1f, 1f, 0f);
        var n = posed.Geosets[0].Normals;
        AssertVertex(n, 0, 0f, 0f, 1f);

        // The renderer's default path renders the posed model without throwing.
        var png = ModelRenderer.RenderPng(model, NoTextures, 64, 64);
        Assert.Equal(PngSignature, png[..8]);
    }

    [Fact]
    public void Model_without_skeleton_renders_in_bind_pose()
    {
        // No SEQS/BONE/PIVT chunks at all — the pre-animation shape of every model.
        var vertices = new float[] { 0f, 0f, 1f, 1f, 0f, -1f, -1f, 0f, -1f };
        var normals = new float[] { 0f, 1f, 0f, 0f, 1f, 0f, 0f, 1f, 0f };
        var uvs = new float[] { 0f, 0f, 1f, 0f, 0f, 1f };
        var model = new Model3D(
            new[] { new Geoset(vertices, normals, uvs, new[] { 0, 1, 2 }, -1) },
            Array.Empty<string>());

        Assert.Same(model, model.PosedAt()); // nothing to pose — identical instance back

        var png = ModelRenderer.RenderPng(model, NoTextures, 64, 64);
        Assert.Equal(PngSignature, png[..8]);
    }

    [Fact]
    public void Skeleton_without_matching_skin_data_falls_back_to_bind_pose()
    {
        var parsed = ModelParser.Parse(BuildAnimatedMdx(), "synthetic.mdx");
        var bindGeoset = parsed.Geosets[0] with { SkinBones = null, SkinWeights = null };
        var model = new Model3D(new[] { bindGeoset }, parsed.Textures, parsed.Skeleton);

        Assert.Same(model, model.PosedAt()); // no skinnable geoset — bind pose, no throw
        var png = ModelRenderer.RenderPng(model, NoTextures, 64, 64);
        Assert.Equal(PngSignature, png[..8]);
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Anime_model_parses_skeleton_and_renders_stand_pose()
    {
        if (!File.Exists(AnimeMap)) return; // corpus-optional

        var doc = MapDocument.Load(AnimeMap);
        var entry = RenderModelCommand.FindModelEntry(doc, @"war3mapImported\wos_RaidenShogun.mdl");
        Assert.NotNull(entry);

        var model = ModelParser.Parse(entry!.RawBytes, entry.FileName!);
        var skeleton = model.Skeleton;
        Assert.NotNull(skeleton);
        Assert.True(skeleton!.Nodes.Count > 0, "no bones parsed");
        Assert.True(skeleton.Sequences.Count > 0, "no sequences parsed");
        Assert.Contains(skeleton.Sequences,
            s => s.Name.StartsWith("Stand", StringComparison.OrdinalIgnoreCase));

        var posed = model.PosedAt();
        Assert.NotSame(model, posed);
        bool moved = model.Geosets.Zip(posed.Geosets)
            .Any(pair => !pair.First.Vertices.SequenceEqual(pair.Second.Vertices));
        Assert.True(moved, "Stand pose left every vertex at bind position");

        var png = ModelRenderer.RenderPng(model, NoTextures);
        Assert.Equal(PngSignature, png[..8]);
        Assert.True(png.Length > 1_000, $"suspiciously small render ({png.Length} bytes)");
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Anime_map_models_pose_coverage()
    {
        if (!File.Exists(AnimeMap)) return; // corpus-optional

        var doc = MapDocument.Load(AnimeMap);
        var mdxFiles = doc.Files
            .Where(f => f.FileName?.EndsWith(".mdx", StringComparison.OrdinalIgnoreCase) == true)
            .ToList();
        Assert.NotEmpty(mdxFiles);

        int parsed = 0, withSkeleton = 0, skinnable = 0, posed = 0, renderable = 0;
        foreach (var file in mdxFiles)
        {
            Model3D model;
            try { model = ModelParser.Parse(file.RawBytes, file.FileName!); }
            catch { continue; }
            parsed++;
            if (model.Geosets.Any(g => g.Indices.Length >= 3 && g.Vertices.Length >= 9)) renderable++;
            if (model.Skeleton is null) continue;
            withSkeleton++;
            if (model.Geosets.Any(g => g.SkinBones is not null)) skinnable++;

            var standPosed = model.PosedAt();
            bool moved = !ReferenceEquals(model, standPosed) && model.Geosets.Zip(standPosed.Geosets)
                .Any(pair => !pair.First.Vertices.SequenceEqual(pair.Second.Vertices));
            if (moved) posed++;
        }

        // skinnable < withSkeleton counts emitter-only models (no geosets) and geosets
        // without matrix groups; posed < skinnable additionally covers models whose
        // bones simply hold still at Stand's first frame — both are bind-pose renders.
        output.WriteLine(
            $"mdx files: {mdxFiles.Count}, parsed: {parsed}, renderable: {renderable}, " +
            $"skeleton: {withSkeleton}, skinnable: {skinnable}, stand-posed (vertices moved): {posed}");
        Assert.True(posed > 0, "not a single map model produced a Stand pose");
    }

    private static void AssertVertex(float[] packed, int index, float x, float y, float z)
    {
        Assert.Equal(x, packed[index * 3], 1e-4f);
        Assert.Equal(y, packed[index * 3 + 1], 1e-4f);
        Assert.Equal(z, packed[index * 3 + 2], 1e-4f);
    }

    /// <summary>
    /// A complete minimal animated MDX: one triangle geoset whose vertices all sit
    /// in matrix group 0 → bone 0, one root bone with a single KGRT key at frame 100
    /// (90° about Z, pivot at origin), and a "Stand" sequence spanning [100, 200].
    /// </summary>
    private static byte[] BuildAnimatedMdx()
    {
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        w.Write("MDLX"u8.ToArray());

        WriteTag(w, "VERS");
        w.Write(4);
        w.Write(800u);

        WriteTag(w, "SEQS");
        w.Write(132);
        WriteFixedString(w, "Stand", 80);
        w.Write(100u); // interval start — the frame PosedAt samples
        w.Write(200u);
        w.Write(0f);   // moveSpeed
        w.Write(0u);   // flags
        w.Write(0f);   // rarity
        w.Write(0u);   // syncPoint
        for (int i = 0; i < 7; i++) w.Write(0f); // extent

        var geoset = BuildGeosetRecord();
        WriteTag(w, "GEOS");
        w.Write(geoset.Length);
        w.Write(geoset);

        WriteTag(w, "BONE");
        w.Write(140);  // node (132) + geosetId + geosetAnimId
        w.Write(132);  // node inclusiveSize: 96 header + 36 KGRT
        WriteFixedString(w, "bone", 80);
        w.Write(0u);   // objectId
        w.Write(-1);   // parentId (root)
        w.Write(0u);   // flags
        WriteTag(w, "KGRT");
        w.Write(1);    // numKeys
        w.Write(1);    // interpolation: linear
        w.Write(-1);   // globalSeqId: none
        w.Write(100);  // frame
        w.Write(0f); w.Write(0f); w.Write(MathF.Sin(MathF.PI / 4)); w.Write(MathF.Cos(MathF.PI / 4));
        w.Write(0u);   // geosetId
        w.Write(0u);   // geosetAnimId

        WriteTag(w, "PIVT");
        w.Write(12);
        w.Write(0f); w.Write(0f); w.Write(0f);

        w.Flush();
        return ms.ToArray();
    }

    /// <summary>One geoset record (leading inclusiveSize) in the classic v800 layout.</summary>
    private static byte[] BuildGeosetRecord()
    {
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        w.Write(0); // inclusiveSize placeholder, patched below

        WriteTag(w, "VRTX");
        w.Write(3);
        w.Write(1f); w.Write(0f); w.Write(0f);
        w.Write(2f); w.Write(0f); w.Write(0f);
        w.Write(1f); w.Write(1f); w.Write(0f);

        WriteTag(w, "NRMS");
        w.Write(3);
        for (int i = 0; i < 3; i++) { w.Write(0f); w.Write(0f); w.Write(1f); }

        WriteTag(w, "PTYP");
        w.Write(1);
        w.Write(4u); // triangles

        WriteTag(w, "PCNT");
        w.Write(1);
        w.Write(3u);

        WriteTag(w, "PVTX");
        w.Write(3);
        w.Write((ushort)0); w.Write((ushort)1); w.Write((ushort)2);

        WriteTag(w, "GNDX");
        w.Write(3);
        w.Write((byte)0); w.Write((byte)0); w.Write((byte)0);

        WriteTag(w, "MTGC");
        w.Write(1);
        w.Write(1u); // group 0 has one bone

        WriteTag(w, "MATS");
        w.Write(1);
        w.Write(0u); // ... which is objectId 0

        w.Write(0u); // materialId
        w.Write(0u); // selectionGroup
        w.Write(0u); // selectionFlags
        for (int i = 0; i < 7; i++) w.Write(0f); // extent
        w.Write(0u); // sequence extents count

        WriteTag(w, "UVAS");
        w.Write(1);
        WriteTag(w, "UVBS");
        w.Write(3);
        for (int i = 0; i < 3; i++) { w.Write(0f); w.Write(0f); }

        w.Flush();
        var bytes = ms.ToArray();
        BinaryPrimitives.WriteInt32LittleEndian(bytes, bytes.Length); // inclusiveSize includes itself
        return bytes;
    }

    private static void WriteTag(BinaryWriter w, string tag)
        => w.Write(Encoding.ASCII.GetBytes(tag));

    private static void WriteFixedString(BinaryWriter w, string value, int length)
    {
        var bytes = new byte[length];
        Encoding.ASCII.GetBytes(value).CopyTo(bytes, 0);
        w.Write(bytes);
    }
}
