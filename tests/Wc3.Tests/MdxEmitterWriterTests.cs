// tests/Wc3.Tests/MdxEmitterWriterTests.cs
using System.Buffers.Binary;
using System.Text;
using Wc3.Modeling;

namespace Wc3.Tests;

public sealed class MdxEmitterWriterTests
{
    private const string AuraPath = @"Textures\rock.blp";
    private const string SparkPath = @"Textures\spark.blp";

    /// <summary>
    /// The 424 byte PREM payload lifted from fireblast.mdx, the same record
    /// <see cref="MdxEmitterReaderTests"/> embeds, repeated here so this writer is pinned to real
    /// data rather than to its own reader. Its shape fixes every PREM offset, 4 bytes of record
    /// size, a 96 byte node, 284 bytes of fixed fields, then one 40 byte visibility track.
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

    private static MdxEmitterParameters Prem(
        int objectId = 3,
        int parentId = 0,
        string texturePath = AuraPath) => new()
        {
            Name = "dust",
            TexturePath = texturePath,
            Kind = MdxEmitterKind.ParticleEmitter1,
            ObjectId = objectId,
            ParentId = parentId,
            SpawnModelPath = @"sharedmodels\rocks1.mdl",
            Lifetime = 1f,
            EmissionRate = 20f,
            Speed = 40f,
            Gravity = 100f,
            Latitude = 45f,
            Longitude = 90f,
        };

    private static MdxEmitterParameters Pre2(
        int objectId = 5,
        int parentId = 0,
        string texturePath = AuraPath) => new()
        {
            Name = "spark",
            TexturePath = texturePath,
            Kind = MdxEmitterKind.ParticleEmitter2,
            ObjectId = objectId,
            ParentId = parentId,
            Lifetime = 0.75f,
            EmissionRate = 30f,
            Speed = 120f,
            Gravity = 0f,
            Latitude = 20f,
            Longitude = 0f,
            Variation = 10f,
            Width = 8f,
            Length = 8f,
            StartColour = new MdxEmitterColour(1f, 0.9f, 0.4f, 255),
            EndColour = new MdxEmitterColour(0.6f, 0.1f, 0f, 0),
            StartSize = 2f,
            EndSize = 0.5f,
            BlendMode = MdxEmitterBlend.Additive,
        };

    /// <summary>
    /// A document built from one PREM emitter is a real chunked MDX, VERS 800, a SEQS chunk, the
    /// texture and material chunks, then the emitter. The emitter reader reads it back and reports
    /// the node id from the payload, so the writer and the shipped reader agree by construction.
    /// </summary>
    [Fact]
    public void A_prem_document_round_trips_through_the_document_and_emitter_readers()
    {
        var document = MdxEmitterWriter.BuildDocument(new[] { Prem() });
        var bytes = document.Write();

        Assert.True(MdxDocument.HasMagic(bytes));

        var parsed = MdxDocument.Parse(bytes);
        Assert.Equal(
            new[] { "VERS", "SEQS", "TEXS", "MTLS", "PREM" },
            parsed.Chunks.Select(c => c.Tag));
        Assert.Equal(MdxEmitterWriter.ModelVersion,
            BinaryPrimitives.ReadUInt32LittleEndian(parsed.Chunks[0].Payload));
        Assert.Equal(bytes, parsed.Write());

        var emitter = Assert.Single(MdxEmitterReader.Read(parsed));
        Assert.Equal("PREM", emitter.Tag);
        Assert.Equal(3, emitter.NodeId);
        Assert.Null(emitter.TextureId);
    }

    /// <summary>
    /// The node header sits where the reader says it does, record size at 0, node size at 4, name at
    /// 8, objectId at 88, parentId at 92 and flags at 96, and the record states its own length, so a
    /// reader that walks by record size lands exactly on the next record. The parent id is read here
    /// because the emitter reader exposes only the node id.
    /// </summary>
    [Fact]
    public void A_prem_record_states_its_size_and_its_ids_at_the_reader_offsets()
    {
        var payload = MdxEmitterWriter.BuildPremPayload(Prem(objectId: 11, parentId: 4));

        Assert.Equal(MdxEmitterWriter.PremRecordSize, payload.Length);
        Assert.Equal(384, payload.Length);
        Assert.Equal(384, BinaryPrimitives.ReadInt32LittleEndian(payload.AsSpan(0, 4)));
        Assert.Equal(96, BinaryPrimitives.ReadInt32LittleEndian(payload.AsSpan(4, 4)));
        Assert.Equal("dust", Encoding.ASCII.GetString(payload, 8, 4));
        Assert.Equal(11, BinaryPrimitives.ReadInt32LittleEndian(payload.AsSpan(88, 4)));
        Assert.Equal(4, BinaryPrimitives.ReadInt32LittleEndian(payload.AsSpan(92, 4)));
        Assert.Equal(
            MdxEmitterWriter.ParticleEmitterFlag | MdxEmitterWriter.EmitterUsesMdlFlag,
            BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(96, 4)));
    }

    /// <summary>
    /// The encoder reproduces a real emitter. The record has no track, so it is the first 384 bytes
    /// of the 424 byte fireblast record, and every field that record fixes is compared byte for
    /// byte, the sizes, the name, the ids, the flags, the emission rate, the gravity, the 260 byte
    /// spawn model path at 16 past the node header, the lifetime and the speed.
    ///
    /// The two angle fields are compared by value instead of by byte, because the recorded values
    /// are 0.907571 and 1.09956 radians, the 6 decimal forms of 52 and 63 degrees, already rounded
    /// before they were written. Converting degrees here lands within a float rounding step of them.
    /// </summary>
    [Fact]
    public void A_prem_record_matches_the_real_fireblast_payload_field_for_field()
    {
        var real = Convert.FromHexString(RealPremPayloadHex);
        Assert.Equal(424, real.Length);

        var parameters = new MdxEmitterParameters
        {
            Name = "SuperSpray04",
            TexturePath = AuraPath,
            Kind = MdxEmitterKind.ParticleEmitter1,
            ObjectId = 17,
            ParentId = -1,
            Flags = 0x9000,
            SpawnModelPath = @"sharedmodels\rocks1.mdl",
            EmissionRate = 9f,
            Gravity = 360f,
            Longitude = 52f,
            Latitude = 63f,
            Lifetime = BitConverter.Int32BitsToSingle(unchecked((int)0x3F8888A4)),
            Speed = BitConverter.Int32BitsToSingle(unchecked((int)0x42ED872B)),
        };

        var payload = MdxEmitterWriter.BuildPremPayload(parameters);
        Assert.Equal(424 - 40, payload.Length); // the real record's one track is not written

        int fields = MdxEmitterWriter.FieldsOffset;
        Assert.Equal(100, fields);

        // Everything before the two angle fields, then everything after them. The record size field
        // is the one intended difference, since the real record also carries a visibility track.
        Assert.Equal(384, BinaryPrimitives.ReadInt32LittleEndian(payload.AsSpan(0, 4)));
        Assert.Equal(424, BinaryPrimitives.ReadInt32LittleEndian(real.AsSpan(0, 4)));
        Assert.Equal(real.AsSpan(4, fields + 4).ToArray(), payload.AsSpan(4, fields + 4).ToArray());
        int tail = payload.Length - (fields + 16);
        Assert.Equal(
            real.AsSpan(fields + 16, tail).ToArray(),
            payload.AsSpan(fields + 16, tail).ToArray());

        float realLongitude = BinaryPrimitives.ReadSingleLittleEndian(
            real.AsSpan(fields + MdxEmitterWriter.PremLongitudeOffset, 4));
        float realLatitude = BinaryPrimitives.ReadSingleLittleEndian(
            real.AsSpan(fields + MdxEmitterWriter.PremLatitudeOffset, 4));
        float longitude = BinaryPrimitives.ReadSingleLittleEndian(
            payload.AsSpan(fields + MdxEmitterWriter.PremLongitudeOffset, 4));
        float latitude = BinaryPrimitives.ReadSingleLittleEndian(
            payload.AsSpan(fields + MdxEmitterWriter.PremLatitudeOffset, 4));

        Assert.Equal(0.907571f, realLongitude, 5); // 52 degrees
        Assert.Equal(1.09956f, realLatitude, 5);   // 63 degrees
        Assert.Equal(realLongitude, longitude, 5);
        Assert.Equal(realLatitude, latitude, 5);
    }

    /// <summary>
    /// A PRE2 record is 271 bytes, 100 for the record size and the node and 171 for the fixed
    /// fields, and its texture id sits at byte 155 past the node header, the same offset
    /// <see cref="MdxEmitterReader"/> reads. That offset is unaligned because the three alpha bytes
    /// before it are bytes, not floats, so a writer that rounded it to 156 would move the id.
    /// </summary>
    [Fact]
    public void A_pre2_record_carries_its_texture_id_at_the_offset_the_reader_reads()
    {
        var payload = MdxEmitterWriter.BuildPre2Payload(Pre2(objectId: 9, parentId: 2), textureId: 1);

        Assert.Equal(MdxEmitterWriter.Pre2RecordSize, payload.Length);
        Assert.Equal(271, payload.Length);
        Assert.Equal(271, BinaryPrimitives.ReadInt32LittleEndian(payload.AsSpan(0, 4)));
        Assert.Equal(96, BinaryPrimitives.ReadInt32LittleEndian(payload.AsSpan(4, 4)));
        Assert.Equal(9, BinaryPrimitives.ReadInt32LittleEndian(payload.AsSpan(88, 4)));
        Assert.Equal(2, BinaryPrimitives.ReadInt32LittleEndian(payload.AsSpan(92, 4)));

        int nodeEnd = 4 + 96;
        Assert.Equal(155, MdxEmitterWriter.Pre2TextureIdOffset);
        Assert.Equal(1, BinaryPrimitives.ReadInt32LittleEndian(
            payload.AsSpan(nodeEnd + MdxEmitterWriter.Pre2TextureIdOffset, 4)));

        var document = MdxEmitterWriter.BuildDocument(new[] { Pre2(objectId: 9, parentId: 2) });
        var emitter = Assert.Single(MdxEmitterReader.Read(MdxDocument.Parse(document.Write())));
        Assert.Equal("PRE2", emitter.Tag);
        Assert.Equal(9, emitter.NodeId);
        Assert.Equal(0, emitter.TextureId);
    }

    /// <summary>
    /// The emitter's texture path is not only a parameter on the record, it reaches the document's
    /// texture table, so the material can name it and a texture audit sees it. Each distinct path is
    /// one TEXS entry, in the order the emitters name them.
    /// </summary>
    [Fact]
    public void The_texture_path_of_an_emitter_reaches_the_texture_chunk()
    {
        var document = MdxEmitterWriter.BuildDocument(new[] { Pre2(texturePath: SparkPath) });
        var parsed = MdxDocument.Parse(document.Write());

        var textures = MdxMaterialWriter.ReadTextures(parsed);
        Assert.Equal(SparkPath, Assert.Single(textures).Path);

        var materials = MdxMaterialWriter.ReadMaterials(parsed);
        var material = Assert.Single(materials);
        Assert.Equal(0, material.Layer.TextureId);
        Assert.Equal(FilterMode.Additive, material.Layer.FilterMode);

        var texs = parsed.Chunks.Single(c => c.Tag == "TEXS").Payload;
        Assert.Equal(MdxMaterialWriter.TextureEntrySize, texs.Length);
    }

    /// <summary>
    /// Two emitters in one document are two nodes, so their object ids must be distinct and their
    /// texture ids must index their own entries in the shared texture table. The reader returns both
    /// records in document order, one per chunk.
    /// </summary>
    [Fact]
    public void Two_emitters_in_one_document_keep_distinct_object_ids_and_textures()
    {
        var parameters = new[]
        {
            Pre2(objectId: 3, texturePath: AuraPath),
            Pre2(objectId: 7, texturePath: SparkPath),
        };

        var document = MdxEmitterWriter.BuildDocument(parameters);
        var parsed = MdxDocument.Parse(document.Write());

        Assert.Equal(2, MdxMaterialWriter.ReadTextures(parsed).Count);
        Assert.Equal(
            new[] { AuraPath, SparkPath },
            MdxMaterialWriter.ReadTextures(parsed).Select(t => t.Path));

        var emitters = MdxEmitterReader.Read(parsed);
        Assert.Equal(2, emitters.Count);
        Assert.Equal(new[] { 3, 7 }, emitters.Select(e => e.NodeId));
        Assert.Equal(2, emitters.Select(e => e.NodeId).Distinct().Count());
        Assert.Equal(new int?[] { 0, 1 }, emitters.Select(e => e.TextureId));
    }

    /// <summary>
    /// A duplicate object id would put two nodes at one index in the node array, so it is refused
    /// rather than written, and a document with no emitter at all is refused as a mistake.
    /// </summary>
    [Fact]
    public void A_duplicate_object_id_is_refused()
    {
        Assert.Throws<ArgumentException>(() => MdxEmitterWriter.BuildDocument(new[]
        {
            Prem(objectId: 4),
            Pre2(objectId: 4),
        }));

        Assert.Throws<ArgumentException>(() =>
            MdxEmitterWriter.BuildDocument(Array.Empty<MdxEmitterParameters>()));
    }

    /// <summary>
    /// A parameter outside its range is refused with an exception, never clamped. An emission rate
    /// of zero emits nothing and a negative one is meaningless, a lifetime must be positive, and the
    /// node flags must carry the bit that marks the node as an emitter.
    /// </summary>
    [Fact]
    public void An_invalid_emission_rate_or_a_zero_lifetime_is_refused()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            MdxEmitterWriter.BuildPremPayload(Prem() with { EmissionRate = 0f }));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            MdxEmitterWriter.BuildPremPayload(Prem() with { EmissionRate = -5f }));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            MdxEmitterWriter.BuildPremPayload(Prem() with { EmissionRate = float.NaN }));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            MdxEmitterWriter.BuildPre2Payload(Pre2() with { EmissionRate = 0f }, 0));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            MdxEmitterWriter.BuildPremPayload(Prem() with { Lifetime = 0f }));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            MdxEmitterWriter.BuildPremPayload(Prem() with { Lifetime = -1f }));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            MdxEmitterWriter.BuildPremPayload(Prem() with { Flags = 0 }));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            MdxEmitterWriter.BuildPremPayload(Prem() with { ObjectId = -1 }));
    }

    /// <summary>
    /// The whole model reader, not only the chunk reader, reads the document the writer builds. It
    /// skips the emitter chunk by size, keeps the texture path, and reports no geometry, which is
    /// what an effect model with an emitter and no mesh is.
    /// </summary>
    [Fact]
    public void The_model_parser_reads_the_whole_document()
    {
        var document = MdxEmitterWriter.BuildDocument(new[] { Pre2(texturePath: SparkPath) });

        var model = MdxParser.Parse(document.Write());

        Assert.Equal(SparkPath, Assert.Single(model.Textures));
        Assert.Empty(model.Geosets);
        Assert.Null(model.Skeleton);
    }

    /// <summary>
    /// The sequence chunk the document carries is the one the sequence writer produces, so a caller
    /// can state the model's own sequences and a caller who states none gets one standing sequence.
    /// </summary>
    [Fact]
    public void The_document_carries_the_stated_sequences()
    {
        var sequences = new[] { new MdxSequenceWriter.Sequence("Birth", 0, 500) };
        var document = MdxEmitterWriter.BuildDocument(new[] { Pre2() }, sequences);
        var parsed = MdxDocument.Parse(document.Write());

        var seqs = parsed.Chunks.Single(c => c.Tag == "SEQS").Payload;
        Assert.Equal(MdxSequenceWriter.RecordSize, seqs.Length);
        Assert.Equal("Birth", Encoding.ASCII.GetString(seqs, 0, 5));
        Assert.Equal(0u, BinaryPrimitives.ReadUInt32LittleEndian(seqs.AsSpan(80, 4)));
        Assert.Equal(500u, BinaryPrimitives.ReadUInt32LittleEndian(seqs.AsSpan(84, 4)));
    }
}
