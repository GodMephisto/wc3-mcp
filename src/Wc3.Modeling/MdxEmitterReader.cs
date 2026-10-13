// src/Wc3.Modeling/MdxEmitterReader.cs
using System.Buffers.Binary;

namespace Wc3.Modeling;

/// <summary>
/// One particle emitter read from a PREM (particle emitter 1) or PRE2 (particle emitter 2) chunk.
/// <see cref="NodeId"/> is the emitter node's objectId, the index the emitter occupies in the
/// model's node array. <see cref="TextureId"/> is the PRE2 emitter's texture index, and is null
/// for a PREM emitter, because particle emitter 1 has no texture field, only a spawn model path.
/// </summary>
/// <param name="ChunkIndex">Index of the source chunk in the document's chunk list.</param>
/// <param name="Tag">The chunk tag, PREM or PRE2.</param>
public readonly record struct MdxEmitter(int ChunkIndex, string Tag, int NodeId, int? TextureId);

/// <summary>
/// Reads the particle emitters an MDX carries, so the audit can check the node and texture each one
/// references rather than only counting the effect chunks. The renderer parser skips PREM and PRE2
/// by chunk size, so this walks their payloads directly.
///
/// The layout is confirmed against the FastMDX reader that War3Net uses, and the PREM objectId
/// offset of 88 is confirmed against a real populated PREM record, the single emitter SuperSpray04
/// at objectId 17 in an imported fireblast.mdx, where nodes 0 to 16 are its bones and helpers.
/// A PREM or PRE2 chunk payload is a sequence of emitter records. Each record begins with a
/// uint32 record size counted from the record start, then the node, which is itself size prefixed,
/// so a record is
/// recordSize, nodeSize, char[80] name, int objectId, int parentId, uint32 flags, node transforms.
/// The objectId therefore sits at byte 88 of the record, and the node header ends at
/// recordStart plus 4 plus nodeSize. A PRE2 record then carries fixed emitter fields, with the
/// texture id at byte 155 past the end of the node header. A PREM record carries no texture id.
/// Only these confirmed fields are read. Everything else in the record is skipped by its size, so a
/// node transform (which grows nodeSize) shifts the PRE2 texture id automatically.
/// </summary>
public static class MdxEmitterReader
{
    private const int SizeField = 4;
    private const int NameLength = 80;
    private const int NodeHeaderSize = 92; // name, objectId, parentId, flags
    private const int MinimumRecordSize = SizeField + SizeField + NodeHeaderSize;
    private const int ObjectIdOffset = SizeField + SizeField + NameLength; // 88
    private const int ParticleEmitter2TextureIdOffset = 155; // past the end of the node header

    /// <summary>
    /// The emitters in every PREM and PRE2 chunk, in chunk order and record order. A chunk whose
    /// records do not parse is left out rather than throwing, matching the parser's tolerance.
    /// </summary>
    public static IReadOnlyList<MdxEmitter> Read(MdxDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        var emitters = new List<MdxEmitter>();
        var chunks = document.Chunks;
        for (int i = 0; i < chunks.Count; i++)
        {
            var chunk = chunks[i];
            if (chunk.Tag is "PREM" or "PRE2")
                ReadChunk(chunk, i, emitters);
        }
        return emitters;
    }

    private static void ReadChunk(MdxDocument.Chunk chunk, int chunkIndex, List<MdxEmitter> emitters)
    {
        byte[] b = chunk.Payload;
        int pos = 0;
        while (pos + MinimumRecordSize <= b.Length)
        {
            int recordSize = ReadI32(b, pos);
            if (recordSize < MinimumRecordSize || pos + (long)recordSize > b.Length)
                break; // malformed or truncated record, keep what we have

            int nodeSize = ReadI32(b, pos + SizeField);
            int nodeEnd = pos + SizeField + nodeSize;
            if (nodeSize < SizeField + NodeHeaderSize || nodeEnd > pos + recordSize)
                break;

            int nodeId = ReadI32(b, pos + ObjectIdOffset);
            int? textureId = null;
            if (chunk.Tag == "PRE2")
            {
                int textureOffset = nodeEnd + ParticleEmitter2TextureIdOffset;
                if (textureOffset + 4 <= pos + recordSize)
                    textureId = ReadI32(b, textureOffset);
            }
            emitters.Add(new MdxEmitter(chunkIndex, chunk.Tag, nodeId, textureId));
            pos += recordSize;
        }
    }

    private static int ReadI32(byte[] b, int pos)
        => BinaryPrimitives.ReadInt32LittleEndian(b.AsSpan(pos, 4));
}
