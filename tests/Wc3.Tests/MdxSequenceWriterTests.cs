// tests/Wc3.Tests/MdxSequenceWriterTests.cs
using System.Buffers.Binary;
using System.Text;
using Wc3.Modeling;

namespace Wc3.Tests;

public sealed class MdxSequenceWriterTests
{
    private static readonly MdxSequenceWriter.Sequence[] TwoSequences =
    {
        new("Stand", 0, 1000),
        new("Walk", 1000, 2000),
    };

    /// <summary>
    /// The written bytes are a real chunked MDX, so they parse back through the chunk reader with a
    /// VERS chunk first and one SEQS chunk holding a record per sequence. SEQS has no count field,
    /// so the count is the payload length over the record size, and it must be two.
    /// </summary>
    [Fact]
    public void A_document_with_two_sequences_round_trips_through_the_document_reader()
    {
        var bytes = MdxSequenceWriter.BuildDocument(TwoSequences).Write();
        Assert.True(MdxDocument.HasMagic(bytes));

        var document = MdxDocument.Parse(bytes);
        Assert.Equal(new[] { "VERS", "SEQS" }, document.Chunks.Select(c => c.Tag));
        Assert.Equal(MdxSequenceWriter.ModelVersion,
            BinaryPrimitives.ReadUInt32LittleEndian(document.Chunks[0].Payload));

        var seqs = document.Chunks[1].Payload;
        Assert.Equal(2 * MdxSequenceWriter.RecordSize, seqs.Length);
        Assert.Equal(2, seqs.Length / MdxSequenceWriter.RecordSize); // the sequence count
        Assert.Equal(bytes, document.Write());
    }

    /// <summary>
    /// The name starts each record at byte 0, and the frames sit at byte 80 and byte 84 of the same
    /// record, the documented offsets. The tail after the end frame is 44 zero bytes for both, so a
    /// reader that walked past the name and frames still finds a well formed record.
    /// </summary>
    [Fact]
    public void Names_and_frames_read_back_at_the_documented_offsets()
    {
        var payload = MdxSequenceWriter.BuildPayload(TwoSequences);
        Assert.Equal(2 * MdxSequenceWriter.RecordSize, payload.Length);

        Assert.Equal("Stand", ReadName(payload, 0));
        Assert.Equal(0u, BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(0 + 80, 4)));
        Assert.Equal(1000u, BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(0 + 84, 4)));
        Assert.Equal(new byte[44], payload.AsSpan(88, 44).ToArray());

        Assert.Equal("Walk", ReadName(payload, MdxSequenceWriter.RecordSize));
        Assert.Equal(1000u, BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(132 + 80, 4)));
        Assert.Equal(2000u, BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(132 + 84, 4)));
        Assert.Equal(new byte[44], payload.AsSpan(132 + 88, 44).ToArray());
    }

    /// <summary>
    /// A 100 character name has 20 bytes too many for the 80 byte field. It is cut at 80 with no
    /// null terminator and no spill, so the record keeps its size, its frames stay at byte 80 and
    /// the next record still starts at byte 132 with an intact name and frames. A writer that
    /// copied the whole string would push both frames and the second record out of line.
    /// </summary>
    [Fact]
    public void A_name_longer_than_eighty_bytes_is_truncated_in_place()
    {
        var payload = MdxSequenceWriter.BuildPayload(new[]
        {
            new MdxSequenceWriter.Sequence(new string('A', 100), 7, 9),
            new MdxSequenceWriter.Sequence("Walk", 1000, 2000),
        });

        Assert.Equal(2 * MdxSequenceWriter.RecordSize, payload.Length);
        Assert.Equal(new string('A', 80), Encoding.ASCII.GetString(payload, 0, 80));
        Assert.Equal(7u, BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(80, 4)));
        Assert.Equal(9u, BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(84, 4)));

        Assert.Equal("Walk", ReadName(payload, MdxSequenceWriter.RecordSize));
        Assert.Equal(1000u, BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(132 + 80, 4)));
        Assert.Equal(2000u, BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(132 + 84, 4)));
    }

    /// <summary>
    /// The payload is byte identical to the SEQS block the existing mesh importer already writes
    /// through <see cref="SyntheticModel"/>, which is the layout the corpus reader accepts. This
    /// pins the new encoder to that one layout rather than to a second interpretation.
    /// </summary>
    [Fact]
    public void The_payload_matches_the_sequence_block_the_existing_writer_emits()
    {
        var expected = MdxDocument.Parse(SyntheticModel.BuildMdx(
            includeTexture: false, includeMaterial: false,
            sequences: TwoSequences.Select(s => (s.Name, s.StartFrame, s.EndFrame)).ToArray()))
            .Chunks.Single(c => c.Tag == "SEQS").Payload;

        Assert.Equal(expected, MdxSequenceWriter.BuildPayload(TwoSequences));
    }

    /// <summary>
    /// The whole model reader, not only the chunk reader, reads the written sequences back. A
    /// skeleton needs a node, so a BONE chunk from the synthetic model is appended after SEQS and
    /// the resulting document is parsed through <see cref="MdxParser"/>.
    /// </summary>
    [Fact]
    public void The_model_parser_reads_the_written_sequences_back()
    {
        var bone = MdxDocument.Parse(SyntheticModel.BuildMdx(includeBone: true))
            .Chunks.Single(c => c.Tag == "BONE");

        var document = MdxSequenceWriter.BuildDocument(TwoSequences, followingChunks: new[] { bone });
        var model = MdxParser.Parse(document.Write());

        var skeleton = Assert.IsType<ModelSkeleton>(model.Skeleton);
        Assert.Equal(new[] { "Stand", "Walk" }, skeleton.Sequences.Select(s => s.Name));
        Assert.Equal(new[] { 0, 1000 }, skeleton.Sequences.Select(s => s.IntervalStart));
        Assert.Equal(new[] { 1000, 2000 }, skeleton.Sequences.Select(s => s.IntervalEnd));
    }

    /// <summary>
    /// Frames are written as uint32, so a negative frame is refused rather than wrapping to a huge
    /// interval, and an interval that ends before it starts is refused because it never plays.
    /// </summary>
    [Fact]
    public void Rejects_a_negative_frame_and_an_interval_that_runs_backwards()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => MdxSequenceWriter.BuildPayload(new[] { new MdxSequenceWriter.Sequence("Bad", -1, 10) }));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => MdxSequenceWriter.BuildPayload(new[] { new MdxSequenceWriter.Sequence("Bad", 0, -1) }));
        Assert.Throws<ArgumentException>(
            () => MdxSequenceWriter.BuildPayload(new[] { new MdxSequenceWriter.Sequence("Bad", 10, 5) }));
    }

    private static string ReadName(byte[] payload, int recordStart)
    {
        var field = payload.AsSpan(recordStart, MdxSequenceWriter.NameLength);
        int end = field.IndexOf((byte)0);
        return Encoding.ASCII.GetString(end < 0 ? field : field[..end]);
    }
}
