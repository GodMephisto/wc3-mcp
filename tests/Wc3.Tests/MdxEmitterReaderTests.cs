// tests/Wc3.Tests/MdxEmitterReaderTests.cs
using System.Buffers.Binary;
using System.Text;
using Wc3.Modeling;

namespace Wc3.Tests;

public sealed class MdxEmitterReaderTests
{
    /// <summary>
    /// The exact 424 byte PREM payload lifted from fireblast.mdx, an imported model in
    /// GGGA_V0.07b_fixed.w3x that really carries a populated particle emitter. The model's sha256 is
    /// 3406ecd2420f8d4a576bde7c0959d43c3fd6ed2c846a44c668876e005f1cc5ad. Its one emitter is the
    /// node SuperSpray04 at objectId 17, the only PREM in a 24 node model where nodes 0 to 16 are
    /// bones and helpers and nodes 18 to 23 are PRE2, ribbon and event nodes. Embedding the raw
    /// bytes pins the objectId offset against real data, not only a synthetic layout.
    /// </summary>
    private const string RealPremPayloadHex =
        "A801000060000000537570657253707261793034000000000000000000000000" +
        "0000000000000000000000000000000000000000000000000000000000000000" +
        "00000000000000000000000000000000000000000000000011000000FFFFFFFF" +
        "00900000000010410000B4439356683F62BE8C3F7368617265646D6F64656C73" +
        "5C726F636B73312E6D646C000000000000000000000000000000000000000000" +
        "0000000000000000000000000000000000000000000000000000000000000000" +
        "0000000000000000000000000000000000000000000000000000000000000000" +
        "0000000000000000000000000000000000000000000000000000000000000000" +
        "0000000000000000000000000000000000000000000000000000000000000000" +
        "0000000000000000000000000000000000000000000000000000000000000000" +
        "0000000000000000000000000000000000000000000000000000000000000000" +
        "000000000000000000000000000000000000000000000000A488883F2B87ED42" +
        "4B5045560300000000000000FFFFFFFF2100000000000000A60000000000803F" +
        "9001000000000000";

    /// <summary>
    /// The PREM node header is recordSize(4) nodeSize(4) name[80] objectId(4) parentId(4) flags(4),
    /// so the name ends at byte 88 and the objectId is read there. A synthetic record built to that
    /// layout pins the reader to byte 88, with a distinctive parentId so a reader that landed on the
    /// wrong field reports a different value than 17.
    /// </summary>
    [Fact]
    public void The_prem_object_id_is_read_from_byte_88_of_the_record()
    {
        var record = SyntheticModel.ParticleEmitter1Record(objectId: 17, parentId: 42, name: "SuperSpray04");

        Assert.Equal(100, record.Length);
        Assert.Equal(100, BinaryPrimitives.ReadInt32LittleEndian(record.AsSpan(0, 4)));  // recordSize
        Assert.Equal(96, BinaryPrimitives.ReadInt32LittleEndian(record.AsSpan(4, 4)));   // nodeSize
        Assert.Equal("SuperSpray04", Encoding.ASCII.GetString(record, 8, 12));           // name at 8
        Assert.Equal(17, BinaryPrimitives.ReadInt32LittleEndian(record.AsSpan(88, 4)));  // objectId at 88
        Assert.Equal(42, BinaryPrimitives.ReadInt32LittleEndian(record.AsSpan(92, 4)));  // parentId at 92

        var doc = MdxDocument.Parse(SyntheticModel.BuildMdx(extraChunks: new[] { ("PREM", record) }));
        var emitter = Assert.Single(MdxEmitterReader.Read(doc));
        Assert.Equal(17, emitter.NodeId); // byte 88, not 84 (name tail, zero) and not 92 (42)
    }

    /// <summary>
    /// Decoding the real recorded payload yields the emitter's objectId, so the byte 88 offset holds
    /// on a populated PREM an exporter really wrote, which the source alone did not prove.
    /// </summary>
    [Fact]
    public void A_real_prem_payload_decodes_to_its_object_id()
    {
        byte[] payload = Convert.FromHexString(RealPremPayloadHex);
        Assert.Equal(424, payload.Length);
        Assert.Equal("SuperSpray04", Encoding.ASCII.GetString(payload, 8, 12));
        Assert.Equal(17, BinaryPrimitives.ReadInt32LittleEndian(payload.AsSpan(88, 4)));

        var doc = MdxDocument.Parse(SyntheticModel.BuildMdx(extraChunks: new[] { ("PREM", payload) }));
        var emitter = Assert.Single(MdxEmitterReader.Read(doc));
        Assert.Equal("PREM", emitter.Tag);
        Assert.Equal(17, emitter.NodeId);
        Assert.Null(emitter.TextureId);
    }

    [Fact]
    public void Reads_the_node_id_and_absent_texture_of_a_prem()
    {
        var bytes = SyntheticModel.BuildMdx(extraChunks: new[]
        {
            ("PREM", SyntheticModel.ParticleEmitter1Record(objectId: 3)),
        });
        var emitter = Assert.Single(MdxEmitterReader.Read(MdxDocument.Parse(bytes)));
        Assert.Equal("PREM", emitter.Tag);
        Assert.Equal(3, emitter.NodeId);
        Assert.Null(emitter.TextureId);
    }

    [Fact]
    public void Reads_the_node_id_and_texture_id_of_a_pre2()
    {
        var bytes = SyntheticModel.BuildMdx(extraChunks: new[]
        {
            ("PRE2", SyntheticModel.ParticleEmitter2Record(objectId: 5, textureId: 1)),
        });
        var emitter = Assert.Single(MdxEmitterReader.Read(MdxDocument.Parse(bytes)));
        Assert.Equal("PRE2", emitter.Tag);
        Assert.Equal(5, emitter.NodeId);
        Assert.Equal(1, emitter.TextureId);
    }

    [Fact]
    public void Reads_every_record_in_a_chunk()
    {
        var payload = SyntheticModel.ParticleEmitter2Record(objectId: 5, textureId: 0)
            .Concat(SyntheticModel.ParticleEmitter2Record(objectId: 6, textureId: 2))
            .ToArray();
        var doc = MdxDocument.Parse(SyntheticModel.BuildMdx(extraChunks: new[] { ("PRE2", payload) }));
        var emitters = MdxEmitterReader.Read(doc);
        Assert.Equal(2, emitters.Count);
        Assert.Equal(new[] { 5, 6 }, emitters.Select(e => e.NodeId));
        Assert.Equal(new int?[] { 0, 2 }, emitters.Select(e => e.TextureId));
    }

    [Fact]
    public void An_empty_effect_chunk_yields_no_emitters()
    {
        var doc = MdxDocument.Parse(SyntheticModel.BuildMdx(extraChunks: new[] { ("PREM", new byte[8]) }));
        Assert.Empty(MdxEmitterReader.Read(doc));
    }

    [Fact]
    public void Node_count_is_taken_from_the_pivt_chunk()
    {
        var withPivots = MdxDocument.Parse(
            SyntheticModel.BuildMdx(extraChunks: new[] { ("PIVT", SyntheticModel.Pivots(4)) }));
        Assert.Equal(4, withPivots.NodeCount());
        Assert.Equal(0, MdxDocument.Parse(SyntheticModel.BuildMdx()).NodeCount());
    }
}
