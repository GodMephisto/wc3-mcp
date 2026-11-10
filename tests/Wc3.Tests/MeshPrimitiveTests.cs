// tests/Wc3.Tests/MeshPrimitiveTests.cs
// Hermetic tests for MeshPrimitiveCommand. Each kind is generated into a temp folder, the file is
// read back as a real MDX, and the geoset it carries is checked against the counts the command
// reported. No game install is needed.
using System.Buffers.Binary;
using System.Text;
using Wc3.Commands;
using Wc3.Modeling;

namespace Wc3.Tests;

public sealed class MeshPrimitiveTests : IDisposable
{
    private readonly string _dir = Path.Combine(
        Path.GetTempPath(), "wc3ctl-mesh-primitive", Guid.NewGuid().ToString("N"));

    public MeshPrimitiveTests() => Directory.CreateDirectory(_dir);

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch (IOException) { }
    }

    [Fact]
    public void Card_writes_a_quad_facing_positive_z()
    {
        var (result, bytes) = Generate(
            MeshPrimitiveKind.Card, size: 32f, texturePath: "Textures\\white.blp");

        Assert.Equal(4, result.Vertices);
        Assert.Equal(2, result.Faces); // one quad, split into two triangles

        // The map path reaches TEXS, at the 4-byte offset the 268-byte row reserves for it.
        var doc = MdxDocument.Parse(bytes);
        var texs = Assert.Single(doc.Chunks.Where(c => c.Tag == "TEXS"));
        Assert.Contains("Textures\\white.blp", Encoding.ASCII.GetString(texs.Payload));

        // Every normal is +Z, so the card faces the viewer rather than being culled.
        var (_, geoset) = Parse(bytes);
        var normals = Assert.Single(geoset.Segments.Where(s => s.Tag == "NRMS")).Payload;
        int count = (int)BinaryPrimitives.ReadUInt32LittleEndian(normals);
        Assert.Equal(4, count);
        for (int i = 0; i < count; i++)
        {
            Assert.Equal(0f, ReadSingle(normals, i * 12), 5);
            Assert.Equal(0f, ReadSingle(normals, i * 12 + 4), 5);
            Assert.Equal(1f, ReadSingle(normals, i * 12 + 8), 5);
        }
    }

    [Fact]
    public void Ring_writes_an_annulus_of_the_requested_segment_count()
    {
        var (result, bytes) = Generate(MeshPrimitiveKind.Ring, size: 32f, innerRadius: 16f, segments: 12);

        Assert.Equal(24, result.Vertices); // inner and outer ring, no seam duplicate
        Assert.Equal(24, result.Faces);    // one quad per segment, two triangles each

        var (minZ, maxZ) = ZRange(bytes);
        Assert.Equal(0f, minZ, 5); // flat in the XY plane
        Assert.Equal(0f, maxZ, 5);
    }

    [Fact]
    public void Cone_writes_a_side_and_a_base_disc()
    {
        var (result, bytes) = Generate(MeshPrimitiveKind.Cone, size: 20f, length: 60f, segments: 16);

        Assert.Equal(32, result.Faces); // 16 side triangles plus 16 base triangles

        // The base sits at -height/2 and the apex at +height/2.
        var (minZ, maxZ) = ZRange(bytes);
        Assert.Equal(-30f, minZ, 4);
        Assert.Equal(30f, maxZ, 4);
    }

    [Fact]
    public void Capsule_writes_a_shaft_closed_by_two_hemispheres()
    {
        var (result, bytes) = Generate(MeshPrimitiveKind.Capsule, size: 16f, length: 64f, segments: 16);

        Assert.True(result.Vertices > 0);
        Assert.True(result.Faces > 0);

        // Total height is the cylinder length plus a hemisphere radius at each end.
        var (minZ, maxZ) = ZRange(bytes);
        Assert.Equal(-48f, minZ, 3);
        Assert.Equal(48f, maxZ, 3);
    }

    [Fact]
    public void A_segment_count_below_the_minimum_is_refused()
    {
        foreach (var kind in new[]
                 {
                     MeshPrimitiveKind.Ring, MeshPrimitiveKind.Cone, MeshPrimitiveKind.Capsule,
                 })
        {
            string path = Path.Combine(_dir, $"refused-{kind}.mdx");
            Assert.Throws<ArgumentOutOfRangeException>(() => MeshPrimitiveCommand.Execute(
                kind, path, segments: MeshPrimitiveCommand.MinSegments - 1));
            Assert.False(File.Exists(path), $"{kind} wrote a file despite the refused segment count");
        }
    }

    [Fact]
    public void A_non_positive_size_is_refused()
    {
        string path = Path.Combine(_dir, "refused-size.mdx");
        Assert.Throws<ArgumentOutOfRangeException>(
            () => MeshPrimitiveCommand.Execute(MeshPrimitiveKind.Card, path, size: 0f));
        Assert.False(File.Exists(path));
    }

    /// <summary>Generates one kind, asserts the file and its MDLX header and GEOS chunk, and
    /// returns the result beside the bytes so a caller can assert the shape itself.</summary>
    private (MeshPrimitiveResult Result, byte[] Bytes) Generate(
        MeshPrimitiveKind kind, float size = 32f, float length = 0f, float innerRadius = 0f,
        int segments = 16, string? texturePath = null)
    {
        string path = Path.Combine(_dir, $"{kind.ToString().ToLowerInvariant()}.mdx");
        var result = MeshPrimitiveCommand.Execute(
            kind, path, size, length, innerRadius, segments, texturePath);

        Assert.Equal(path, result.Path);
        Assert.Equal(kind, result.Kind);
        Assert.True(File.Exists(path), $"{kind} wrote no file at {path}");

        var bytes = File.ReadAllBytes(path);
        Assert.True(MdxDocument.HasMagic(bytes), $"{kind} did not write MDLX bytes");
        Assert.Equal((byte)'M', bytes[0]);
        Assert.Equal((byte)'D', bytes[1]);
        Assert.Equal((byte)'L', bytes[2]);
        Assert.Equal((byte)'X', bytes[3]);
        Assert.Equal(bytes.LongLength, result.Bytes);
        Assert.Empty(result.Diagnostics);

        // One GEOS chunk, and its VRTX count is the count the command reported.
        var (_, geoset) = Parse(bytes);
        var vrtx = Assert.Single(geoset.Segments.Where(s => s.Tag == "VRTX")).Payload;
        Assert.Equal(result.Vertices, (int)BinaryPrimitives.ReadUInt32LittleEndian(vrtx));
        Assert.Equal(result.Vertices * 12, vrtx.Length - 4);

        var pvtx = Assert.Single(geoset.Segments.Where(s => s.Tag == "PVTX")).Payload;
        Assert.Equal(result.Faces * 3, (int)BinaryPrimitives.ReadUInt32LittleEndian(pvtx));

        return (result, bytes);
    }

    /// <summary>Parses the bytes and returns the single GEOS chunk as a geoset, skipping the
    /// inclusive size field so the segment walk starts on the VRTX tag.</summary>
    private static (MdxDocument Doc, MdxGeoset Geoset) Parse(byte[] bytes)
    {
        var doc = MdxDocument.Parse(bytes);
        int index = Assert.Single(doc.GeosetIndices());
        return (doc, MdxGeoset.Parse(doc.Chunks[index].Payload[4..], 800));
    }

    /// <summary>The lowest and highest vertex Z the geoset carries, which pins the primitive's
    /// extent along its axis.</summary>
    private static (float Min, float Max) ZRange(byte[] bytes)
    {
        var (_, geoset) = Parse(bytes);
        var vrtx = Assert.Single(geoset.Segments.Where(s => s.Tag == "VRTX")).Payload;
        int count = (int)BinaryPrimitives.ReadUInt32LittleEndian(vrtx);
        float min = float.MaxValue, max = float.MinValue;
        for (int i = 0; i < count; i++)
        {
            float z = ReadSingle(vrtx, i * 12 + 8);
            min = MathF.Min(min, z);
            max = MathF.Max(max, z);
        }
        return (min, max);
    }

    /// <summary>Reads a float at a byte offset after the segment's 4-byte count.</summary>
    private static float ReadSingle(byte[] segment, int offsetAfterCount) =>
        BinaryPrimitives.ReadSingleLittleEndian(segment.AsSpan(4 + offsetAfterCount));
}
