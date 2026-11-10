// tests/Wc3.Tests/MdxMaterialWriterTests.cs
using System.Buffers.Binary;
using System.Text;
using Wc3.Modeling;

namespace Wc3.Tests;

public sealed class MdxMaterialWriterTests
{
    private const string AuraPath = @"war3mapImported\hero_aura.blp";

    [Fact]
    public void Document_carries_vers_800_then_texs_then_mtls()
    {
        var bytes = MdxMaterialWriter.CreateDocument(
            new[] { new MdxTextureEntry(AuraPath) },
            new[] { MdxMaterialEntry.Single(MdxLayer.Opaque(0)) }).Write();

        var doc = MdxDocument.Parse(bytes);

        Assert.Equal(new[] { "VERS", "TEXS", "MTLS" }, doc.Chunks.Select(c => c.Tag));
        Assert.Equal(800u, BinaryPrimitives.ReadUInt32LittleEndian(doc.Chunks[0].Payload));
        Assert.Equal(MdxMaterialWriter.TextureEntrySize, doc.Chunks[1].Payload.Length);
        Assert.Equal(MdxMaterialWriter.MaterialSize, doc.Chunks[2].Payload.Length);
    }

    [Fact]
    public void Texture_path_is_at_the_documented_offset_and_null_padded()
    {
        var doc = MdxMaterialWriter.CreateDocument(AuraPath);
        var texs = doc.Chunks.Single(c => c.Tag == "TEXS").Payload;

        // Entry 0. Replaceable id 0, then the path at 4, then flags at 264.
        Assert.Equal(0u, BinaryPrimitives.ReadUInt32LittleEndian(texs.AsSpan(0, 4)));

        string path = Encoding.ASCII.GetString(
            texs, MdxMaterialWriter.TexturePathOffset, AuraPath.Length);
        Assert.Equal(AuraPath, path);

        // The rest of the 260 byte field is padding, so the byte after the path is a null.
        Assert.Equal((byte)0, texs[MdxMaterialWriter.TexturePathOffset + AuraPath.Length]);
        Assert.Equal((byte)0, texs[MdxMaterialWriter.TextureFlagsOffset - 1]);
        Assert.Equal(0u, BinaryPrimitives.ReadUInt32LittleEndian(
            texs.AsSpan(MdxMaterialWriter.TextureFlagsOffset, 4)));

        Assert.Equal(AuraPath, MdxMaterialWriter.ReadTextures(doc).Single().Path);
    }

    [Fact]
    public void Blend_mode_and_static_texture_id_read_back()
    {
        var doc = MdxMaterialWriter.CreateDocument(AuraPath, FilterMode.Additive);
        var mtls = doc.Chunks.Single(c => c.Tag == "MTLS").Payload;
        int layer = MdxMaterialWriter.MaterialFirstLayerOffset;

        // LAYS at 12, one layer, then the layer record at 20.
        Assert.Equal("LAYS", Encoding.ASCII.GetString(
            mtls, MdxMaterialWriter.MaterialLayersOffset, 4));
        Assert.Equal(1u, BinaryPrimitives.ReadUInt32LittleEndian(
            mtls.AsSpan(MdxMaterialWriter.MaterialLayerCountOffset, 4)));
        Assert.Equal((uint)MdxMaterialWriter.MaterialSize, BinaryPrimitives.ReadUInt32LittleEndian(
            mtls.AsSpan(0, 4)));
        Assert.Equal((uint)MdxMaterialWriter.LayerSize, BinaryPrimitives.ReadUInt32LittleEndian(
            mtls.AsSpan(layer, 4)));

        // Filter mode is the blend mode at layer offset 4, the static texture id at layer offset 12.
        Assert.Equal((uint)FilterMode.Additive, BinaryPrimitives.ReadUInt32LittleEndian(
            mtls.AsSpan(layer + 4, 4)));
        Assert.Equal(0u, BinaryPrimitives.ReadUInt32LittleEndian(mtls.AsSpan(layer + 12, 4)));

        var material = Assert.Single(MdxMaterialWriter.ReadMaterials(doc));
        Assert.Equal(FilterMode.Additive, material.Layer.FilterMode);
        Assert.Equal(0, material.Layer.TextureId);
        Assert.Equal(-1, material.Layer.TextureAnimationId);
        Assert.Equal(0u, material.Layer.CoordId);
        Assert.Equal(1.0f, material.Layer.Alpha);
    }

    [Fact]
    public void Path_longer_than_260_bytes_is_truncated_safely()
    {
        string longPath = new('a', 400);
        var payload = MdxMaterialWriter.EncodeTexs(
            new[] { new MdxTextureEntry(longPath, 0, 7) });

        // Truncation must not grow the entry or push the flags field out of place.
        Assert.Equal(MdxMaterialWriter.TextureEntrySize, payload.Length);
        Assert.Equal(7u, BinaryPrimitives.ReadUInt32LittleEndian(
            payload.AsSpan(MdxMaterialWriter.TextureFlagsOffset, 4)));

        string read = MdxMaterialWriter.ReadTextures(payload).Single().Path;
        Assert.Equal(MdxMaterialWriter.TexturePathLength, read.Length);
        Assert.Equal(new string('a', MdxMaterialWriter.TexturePathLength), read);
    }

    [Fact]
    public void The_existing_parser_reads_the_written_texture_table()
    {
        var bytes = MdxMaterialWriter.CreateDocument(AuraPath, FilterMode.Additive).Write();

        var model = ModelParser.Parse(bytes, "aura.mdx");

        Assert.Equal(AuraPath, Assert.Single(model.Textures));
    }

    [Fact]
    public void Each_material_record_advances_by_its_inclusive_size()
    {
        var materials = new[]
        {
            MdxMaterialEntry.Single(MdxLayer.Opaque(0)),
            MdxMaterialEntry.Single(MdxLayer.Opaque(1, FilterMode.Blend)),
        };

        var payload = MdxMaterialWriter.EncodeMtls(materials);

        Assert.Equal(2 * MdxMaterialWriter.MaterialSize, payload.Length);
        Assert.Equal((uint)MdxMaterialWriter.MaterialSize, BinaryPrimitives.ReadUInt32LittleEndian(
            payload.AsSpan(MdxMaterialWriter.MaterialSize, 4)));

        var read = MdxMaterialWriter.ReadMaterials(payload);
        Assert.Equal(2, read.Count);
        Assert.Equal(0, read[0].Layer.TextureId);
        Assert.Equal(1, read[1].Layer.TextureId);
        Assert.Equal(FilterMode.Blend, read[1].Layer.FilterMode);
    }
}
