// src/Wc3.Modeling/MdxSequenceWriter.cs
using System.Buffers.Binary;
using System.Text;

namespace Wc3.Modeling;

/// <summary>
/// Encodes the SEQS chunk of a binary MDX, the animation sequence list a model declares, so a model
/// built from scratch can name its sequences rather than inheriting them from an exporter.
///
/// The payload is a flat array of fixed 132 byte records with no count field, which is the layout
/// <see cref="MdxParser"/> reads and the reference exporter writes. The count is the payload length
/// divided by <see cref="RecordSize"/>. A record is a char[80] name, then uint32 start frame, then
/// uint32 end frame, then the 44 byte animation tail (float moveSpeed, uint32 nonLooping flags,
/// float rarity, uint32 syncPoint, then the extent as a float radius and six corner floats). The
/// tail is written zero, the honest default for a sequence whose source described no bounds, which
/// matches what the mesh importer in the command layer already emits for the same fields.
///
/// A name is ASCII, null padded to 80 bytes, and truncated at 80 bytes rather than spilling into
/// the next record's offset. Frames are unsigned, so a negative frame is refused instead of wrapping
/// to a huge interval that would never play.
/// </summary>
public static class MdxSequenceWriter
{
    /// <summary>The size of one sequence record, name and frames plus the zeroed animation tail.</summary>
    public const int RecordSize = 132;

    /// <summary>The length of the null padded name field, in bytes.</summary>
    public const int NameLength = 80;

    /// <summary>The offset of the start frame from the record start.</summary>
    public const int StartOffset = 80;

    /// <summary>The offset of the end frame from the record start.</summary>
    public const int EndOffset = 84;

    /// <summary>The MDX version the built document declares, the classic v800 layout.</summary>
    public const uint ModelVersion = 800;

    private const uint VersChunkSize = 4;

    /// <summary>
    /// One animation sequence as the writer takes it, a name and an inclusive frame interval.
    /// <see cref="StartFrame"/> and <see cref="EndFrame"/> are written as uint32, so both must be
    /// non-negative and the interval must not end before it starts.
    /// </summary>
    public sealed record Sequence(string Name, int StartFrame, int EndFrame);

    /// <summary>
    /// The SEQS chunk payload for <paramref name="sequences"/>, one record each in list order. The
    /// trailing animation tail of every record is left zero, and a name longer than
    /// <see cref="NameLength"/> bytes is cut at that boundary, so the record stays in place.
    /// </summary>
    public static byte[] BuildPayload(IReadOnlyList<Sequence> sequences)
    {
        ArgumentNullException.ThrowIfNull(sequences);

        var payload = new byte[sequences.Count * RecordSize];
        for (int i = 0; i < sequences.Count; i++)
        {
            var sequence = sequences[i]
                ?? throw new ArgumentException("a sequence cannot be null", nameof(sequences));
            if (sequence.StartFrame < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(sequences), sequence.StartFrame, "a sequence start frame cannot be negative");
            if (sequence.EndFrame < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(sequences), sequence.EndFrame, "a sequence end frame cannot be negative");
            if (sequence.EndFrame < sequence.StartFrame)
                throw new ArgumentException(
                    $"sequence '{sequence.Name}' ends at frame {sequence.EndFrame} before it starts at {sequence.StartFrame}",
                    nameof(sequences));

            int o = i * RecordSize;
            var name = Encoding.ASCII.GetBytes(sequence.Name ?? string.Empty);
            name.AsSpan(0, Math.Min(name.Length, NameLength)).CopyTo(payload.AsSpan(o, NameLength));
            BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(o + StartOffset, 4), (uint)sequence.StartFrame);
            BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(o + EndOffset, 4), (uint)sequence.EndFrame);
        }
        return payload;
    }

    /// <summary>
    /// A complete MDX document carrying a VERS chunk at <paramref name="version"/> and the SEQS
    /// chunk for <paramref name="sequences"/>, ready for <see cref="MdxDocument.Write"/>. Chunks in
    /// <paramref name="followingChunks"/> are appended after SEQS, so a caller composing a whole
    /// model (a geoset, a bone, an emitter) builds it in one document instead of patching a second.
    /// </summary>
    public static MdxDocument BuildDocument(
        IReadOnlyList<Sequence> sequences,
        uint version = ModelVersion,
        IReadOnlyList<MdxDocument.Chunk>? followingChunks = null)
    {
        ArgumentNullException.ThrowIfNull(sequences);

        var chunks = new List<MdxDocument.Chunk>(2 + (followingChunks?.Count ?? 0))
        {
            new("VERS", VersionPayload(version)),
            new("SEQS", BuildPayload(sequences)),
        };
        if (followingChunks is not null)
            chunks.AddRange(followingChunks);
        return MdxDocument.Create(chunks);
    }

    private static byte[] VersionPayload(uint version)
    {
        var payload = new byte[VersChunkSize];
        BinaryPrimitives.WriteUInt32LittleEndian(payload, version);
        return payload;
    }
}
