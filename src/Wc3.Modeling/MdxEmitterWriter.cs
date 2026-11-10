// src/Wc3.Modeling/MdxEmitterWriter.cs
using System.Buffers.Binary;
using System.Text;

namespace Wc3.Modeling;

/// <summary>Which emitter chunk encodes a set of parameters.</summary>
public enum MdxEmitterKind
{
    /// <summary>Particle emitter 1, the PREM chunk. No texture id, one spawn model path.</summary>
    ParticleEmitter1,

    /// <summary>Particle emitter 2, the PRE2 chunk. One texture id, plus segment colour, alpha and size.</summary>
    ParticleEmitter2,
}

/// <summary>
/// The blend a PRE2 emitter applies to its texture. These are the values the emitter chunk stores,
/// 0 to 4, and they are NOT the material layer values in <see cref="FilterMode"/>, which run 0 to 6
/// with a different meaning at every index. The two spaces are mapped by name when a material is
/// written, never by casting one to the other.
/// </summary>
public enum MdxEmitterBlend
{
    /// <summary>Alpha blended, the usual smoke and dust.</summary>
    Blend = 0,

    /// <summary>Additive, the glow a generated effect usually wants.</summary>
    Additive = 1,

    /// <summary>Multiplied, a darkener.</summary>
    Modulate = 2,

    /// <summary>Doubled multiply.</summary>
    Modulate2x = 3,

    /// <summary>Additive with an alpha channel.</summary>
    AddAlpha = 4,
}

/// <summary>How a PRE2 emitter orients the sprite along its velocity, the chunk's head or tail field.</summary>
public enum MdxEmitterHeadTail
{
    /// <summary>A head sprite, the usual dot or puff.</summary>
    Head = 0,

    /// <summary>A tail sprite.</summary>
    Tail = 1,

    /// <summary>Head and tail together.</summary>
    Both = 2,
}

/// <summary>
/// One colour stop of a PRE2 emitter, three floats in the 0 to 1 range and a byte alpha. The alpha
/// rides here because the emitter chunk keeps its three colours as floats and its three alphas as
/// bytes in separate blocks, and a caller setting a fade wants to write it once.
/// </summary>
public readonly record struct MdxEmitterColour(float Red, float Green, float Blue, byte Alpha = 255)
{
    /// <summary>Opaque white, the neutral stop.</summary>
    public static MdxEmitterColour White { get; } = new(1f, 1f, 1f, 255);
}

/// <summary>One sprite sheet interval, a start frame, an end frame and a repeat count, all u32.</summary>
public readonly record struct MdxEmitterUvInterval(uint Start, uint End, uint Repeat);

/// <summary>
/// The four sprite sheet intervals a PRE2 emitter carries, head lifetime, head decay, tail lifetime
/// and tail decay. Every interval zeroed is frame 0 for the whole particle life, which is what a
/// single frame texture wants and what a caller gets by default.
/// </summary>
public sealed record MdxEmitterSheetUv(
    MdxEmitterUvInterval HeadLifetime,
    MdxEmitterUvInterval HeadDecay,
    MdxEmitterUvInterval TailLifetime,
    MdxEmitterUvInterval TailDecay)
{
    /// <summary>Frame 0 for the whole life, the single frame sheet.</summary>
    public static MdxEmitterSheetUv SingleFrame { get; } =
        new(default, default, default, default);
}

/// <summary>
/// One particle emitter as a caller states it, shared by both chunk layouts. Every value is checked
/// before a byte is written and an illegal one is refused with an exception naming the field, never
/// clamped, so a caller cannot ship an emitter whose stated parameters are not the ones it holds.
///
/// <see cref="Kind"/> picks the chunk. A PREM emitter encodes lifetime, emission rate, speed,
/// gravity, latitude, longitude and <see cref="SpawnModelPath"/>, and the chunk has no field for a
/// texture, a colour, a size, a sprite sheet or a priority plane, so those parameters are simply not
/// part of that chunk. A PRE2 emitter encodes all of them except the spawn model path, which PRE2
/// does not carry. The parameters a given kind cannot hold are documented on each property rather
/// than dropped in silence.
/// </summary>
public sealed record MdxEmitterParameters
{
    /// <summary>The node name, ASCII, cut at 80 bytes because that is the node name field.</summary>
    public required string Name { get; init; }

    /// <summary>
    /// The texture the emitter draws, a map path. It reaches TEXS and the material, and for a PRE2
    /// emitter its TEXS index is the chunk's texture id. A PREM emitter still lists it, since the
    /// document keeps one texture table, but the PREM chunk itself has no texture id field.
    /// </summary>
    public required string TexturePath { get; init; }

    /// <summary>Which chunk encodes this emitter. PRE2 by default, because that is the textured one.</summary>
    public MdxEmitterKind Kind { get; init; } = MdxEmitterKind.ParticleEmitter2;

    /// <summary>The node's objectId, the index it occupies in the model's node array. Not negative.</summary>
    public int ObjectId { get; init; }

    /// <summary>The parent node id, -1 for a root node, as every node in the format states it.</summary>
    public int ParentId { get; init; } = -1;

    /// <summary>
    /// The model a PREM emitter spawns at each particle, for example <c>sharedmodels\rocks1.mdl</c>.
    /// Written into the PREM chunk's 256 byte path field, and PRE2 has no equivalent field.
    /// </summary>
    public string SpawnModelPath { get; init; } = string.Empty;

    /// <summary>How long one particle lives, in seconds. Must be greater than zero.</summary>
    public float Lifetime { get; init; } = 1f;

    /// <summary>
    /// Particles per second. Must be greater than zero. A real PRE2 emitter may hold a zero here and
    /// drive the rate from a KP2E track, which is what the Blizzard emitters in freezingring.mdx do,
    /// but this writer has no track support, so a zero base would write an emitter that never emits
    /// and it is refused rather than accepted as dead content.
    /// </summary>
    public float EmissionRate { get; init; } = 10f;

    /// <summary>Initial particle speed, in units per second. Zero is allowed, a negative speed is not.</summary>
    public float Speed { get; init; }

    /// <summary>Downward acceleration in units per second squared. Negative lifts the particles.</summary>
    public float Gravity { get; init; }

    /// <summary>The emission cone half angle in degrees, 0 to 180. Stored as radians.</summary>
    public float Latitude { get; init; }

    /// <summary>The emission direction around the vertical in degrees, 0 to 360. Stored as radians.</summary>
    public float Longitude { get; init; }

    /// <summary>The colour at birth. A PRE2 field only, PREM has no colour.</summary>
    public MdxEmitterColour StartColour { get; init; } = MdxEmitterColour.White;

    /// <summary>The colour at death. A PRE2 field only.</summary>
    public MdxEmitterColour EndColour { get; init; } = MdxEmitterColour.White;

    /// <summary>The particle size multiplier at birth. A PRE2 field only.</summary>
    public float StartSize { get; init; } = 1f;

    /// <summary>The particle size multiplier at death. A PRE2 field only.</summary>
    public float EndSize { get; init; } = 1f;

    /// <summary>The blend the emitter draws with. A PRE2 field only.</summary>
    public MdxEmitterBlend BlendMode { get; init; } = MdxEmitterBlend.Additive;

    /// <summary>
    /// The node flags. The <c>ParticleEmitter</c> bit, 0x1000, must be set, which is what marks the
    /// node as an emitter at all. The default carries that bit and nothing else.
    /// </summary>
    public uint Flags { get; init; } = MdxEmitterWriter.ParticleEmitterFlag;

    /// <summary>Speed variation, plus or minus, in units per second. A PRE2 field only.</summary>
    public float Variation { get; init; }

    /// <summary>The emitter area across, in units. A PRE2 field only.</summary>
    public float Width { get; init; }

    /// <summary>The emitter area along, in units. A PRE2 field only.</summary>
    public float Length { get; init; }

    /// <summary>Sprite sheet rows, at least 1. A PRE2 field only.</summary>
    public uint Rows { get; init; } = 1;

    /// <summary>Sprite sheet columns, at least 1. A PRE2 field only.</summary>
    public uint Columns { get; init; } = 1;

    /// <summary>Whether the sprite is a head, a tail or both. A PRE2 field only.</summary>
    public MdxEmitterHeadTail HeadOrTail { get; init; } = MdxEmitterHeadTail.Head;

    /// <summary>The tail length in units. A PRE2 field only.</summary>
    public float TailLength { get; init; }

    /// <summary>How long the tail lasts, in seconds. A PRE2 field only.</summary>
    public float TailTime { get; init; }

    /// <summary>The sprite sheet intervals. A PRE2 field only.</summary>
    public MdxEmitterSheetUv Sheet { get; init; } = MdxEmitterSheetUv.SingleFrame;

    /// <summary>The Reforged squirt flag, written as 1 or 0. A PRE2 field only.</summary>
    public bool Squirt { get; init; }

    /// <summary>The draw priority plane. A PRE2 field only.</summary>
    public uint PriorityPlane { get; init; }

    /// <summary>The replaceable texture id, 0 for a literal texture path. A PRE2 field only.</summary>
    public int ReplaceableId { get; init; }
}

/// <summary>
/// Encodes the PREM (particle emitter 1) and PRE2 (particle emitter 2) chunks of a binary MDX, and a
/// complete effect document around them, so a particle effect can be generated from parameters
/// rather than inherited from an exporter.
///
/// The layout is not recalled, it is derived from three sources that agree. The shipped reader
/// <see cref="MdxEmitterReader"/> states the node header offsets. The 424 byte PREM record that
/// <c>MdxEmitterReaderTests</c> embeds, a real emitter lifted from fireblast.mdx, fixes every
/// remaining offset arithmetically, since its record size 424 is the 4 byte record size plus a 96
/// byte node plus 284 bytes of fixed fields plus a 40 byte visibility track, and its spawn model path
/// text sits at byte 116, which is byte 16 past the node header. The fixed PREM fields are then a
/// float emission rate, gravity, longitude, latitude, a 256 byte spawn model path padded to 260, a
/// float lifetime and a float speed. The PRE2 order is the one the FastMDX and war3mdlx readers both
/// read, and its texture id lands at byte 155 past the node header, which is exactly what
/// <see cref="MdxEmitterReader"/> reads, because the three segment colours, the three alpha bytes and
/// the four sheet intervals before it add up to 155. Six real PRE2 records in the Blizzard model
/// freezingring.mdx decode whole at those offsets, every field in range and every texture id inside
/// the model's six entry texture table, so the PRE2 order is measured and not only read.
///
/// Every record is one emitter, a start counted size, a start counted node with no transform tracks,
/// then the fixed fields. A chunk carries the records back to back, and this writer emits one chunk
/// per emitter, the way the reference exporter does.
///
/// Latitude and longitude are stated in degrees and stored in radians, which the real record proves,
/// since its two angle fields decode to 0.907571 and 1.09956 radians, the 6 decimal radian forms of
/// 52 and 63 degrees. A value converted here from degrees therefore differs from that record in the
/// last bits, which is expected, since the recorded value was already rounded to 6 decimals before it
/// was written.
/// </summary>
public static class MdxEmitterWriter
{
    /// <summary>The MDX version the built document declares, the classic v800 layout.</summary>
    public const uint ModelVersion = 800;

    /// <summary>The chunk tag of a particle emitter 1.</summary>
    public const string PremTag = "PREM";

    /// <summary>The chunk tag of a particle emitter 2.</summary>
    public const string Pre2Tag = "PRE2";

    /// <summary>The node flag that marks a node as a particle emitter.</summary>
    public const uint ParticleEmitterFlag = 0x1000;

    /// <summary>The node flag that says a PREM spawn model path is a .mdl rather than a .mdx.</summary>
    public const uint EmitterUsesMdlFlag = 0x8000;

    /// <summary>The node flag that says a PREM spawn model is a .tga rather than a .mdx or .mdl.</summary>
    public const uint EmitterUsesTgaFlag = 0x10000;

    // Node header offsets, counted from the record start. The reader documents the same shape, a
    // record size, then a node whose own size covers the name, the ids and the flags, then the
    // emitter fields, so the node header ends at recordStart plus 4 plus the node size.

    /// <summary>The size field that opens a record, counted from the record start.</summary>
    public const int RecordSizeField = 4;

    /// <summary>Where the node's own size field starts, the second field of a record.</summary>
    public const int NodeSizeField = 4;

    /// <summary>Where the node name starts, a null padded ASCII field.</summary>
    public const int NodeNameOffset = 8;

    /// <summary>The length of the null padded node name field.</summary>
    public const int NodeNameLength = 80;

    /// <summary>Where the objectId sits in a record, past the record and node size fields and the name.</summary>
    public const int ObjectIdOffset = 88;

    /// <summary>Where the parentId sits in a record, four bytes past the objectId.</summary>
    public const int ParentIdOffset = 92;

    /// <summary>Where the node flags sit in a record, four bytes past the parentId.</summary>
    public const int NodeFlagsOffset = 96;

    /// <summary>The node header after its size field, the name, the objectId, the parentId and the flags.</summary>
    public const int NodeHeaderSize = 92;

    /// <summary>The node size this writer states, the header and its size field and no transform tracks.</summary>
    public const int NodeSize = 4 + NodeHeaderSize;

    /// <summary>Where the emitter's own fixed fields begin, four bytes past a record start plus the node size.</summary>
    public const int FieldsOffset = RecordSizeField + NodeSize;

    // PREM fixed fields, counted from the end of the node header. A real 424 byte record confirms
    // all of them, see the type summary.

    /// <summary>The emission rate, in particles per second.</summary>
    public const int PremEmissionRateOffset = 0;

    /// <summary>The gravity, in units per second squared.</summary>
    public const int PremGravityOffset = 4;

    /// <summary>The longitude, in radians.</summary>
    public const int PremLongitudeOffset = 8;

    /// <summary>The latitude, in radians.</summary>
    public const int PremLatitudeOffset = 12;

    /// <summary>Where the spawn model path starts, the text field a PREM emitter carries.</summary>
    public const int PremSpawnModelPathOffset = 16;

    /// <summary>The spawn model path field, 256 characters and 4 bytes of padding the format never names.</summary>
    public const int PremSpawnModelPathLength = 260;

    /// <summary>The path text limit, the 256 bytes before the unread padding.</summary>
    public const int PremSpawnModelPathTextLength = 256;

    /// <summary>The particle lifetime, in seconds.</summary>
    public const int PremLifetimeOffset = 276;

    /// <summary>The initial speed, in units per second.</summary>
    public const int PremSpeedOffset = 280;

    /// <summary>The PREM fixed fields, before any track, the path and the lifetime and speed floats.</summary>
    public const int PremFixedSize = PremSpawnModelPathOffset + PremSpawnModelPathLength + 8;

    /// <summary>A whole PREM record with no track, the record size it states included.</summary>
    public const int PremRecordSize = RecordSizeField + NodeSize + PremFixedSize;

    // PRE2 fixed fields, counted from the end of the node header. The order is the one FastMDX and
    // war3mdlx both read, and the texture id offset is the one MdxEmitterReader reads.

    /// <summary>The initial speed, in units per second.</summary>
    public const int Pre2SpeedOffset = 0;

    /// <summary>The speed variation, plus or minus, in units per second.</summary>
    public const int Pre2VariationOffset = 4;

    /// <summary>The latitude, in radians.</summary>
    public const int Pre2LatitudeOffset = 8;

    /// <summary>The gravity, in units per second squared.</summary>
    public const int Pre2GravityOffset = 12;

    /// <summary>The particle lifetime, in seconds.</summary>
    public const int Pre2LifetimeOffset = 16;

    /// <summary>The emission rate, in particles per second.</summary>
    public const int Pre2EmissionRateOffset = 20;

    /// <summary>The emitter length, in units.</summary>
    public const int Pre2LengthOffset = 24;

    /// <summary>The emitter width, in units.</summary>
    public const int Pre2WidthOffset = 28;

    /// <summary>The blend mode, one of the <see cref="MdxEmitterBlend"/> values.</summary>
    public const int Pre2BlendOffset = 32;

    /// <summary>The sprite sheet rows.</summary>
    public const int Pre2RowsOffset = 36;

    /// <summary>The sprite sheet columns.</summary>
    public const int Pre2ColumnsOffset = 40;

    /// <summary>The head or tail field, one of the <see cref="MdxEmitterHeadTail"/> values.</summary>
    public const int Pre2HeadOrTailOffset = 44;

    /// <summary>The tail length, in units.</summary>
    public const int Pre2TailLengthOffset = 48;

    /// <summary>The tail lifetime, in seconds.</summary>
    public const int Pre2TailTimeOffset = 52;

    /// <summary>The first of three segment colours, each three floats.</summary>
    public const int Pre2FirstSegmentColourOffset = 56;

    /// <summary>The first of three segment alpha bytes, which is why the texture id is unaligned.</summary>
    public const int Pre2FirstSegmentAlphaOffset = 92;

    /// <summary>The first of three segment size multipliers, each a float.</summary>
    public const int Pre2FirstSegmentScalingOffset = 95;

    /// <summary>The head lifetime sheet interval, three u32 values.</summary>
    public const int Pre2HeadLifetimeOffset = 107;

    /// <summary>The head decay sheet interval.</summary>
    public const int Pre2HeadDecayOffset = 119;

    /// <summary>The tail lifetime sheet interval.</summary>
    public const int Pre2TailLifetimeOffset = 131;

    /// <summary>The tail decay sheet interval.</summary>
    public const int Pre2TailDecayOffset = 143;

    /// <summary>The texture id, the index into TEXS. The reader reads this same byte offset.</summary>
    public const int Pre2TextureIdOffset = 155;

    /// <summary>The Reforged squirt flag, a u32 that is 1 or 0.</summary>
    public const int Pre2SquirtOffset = 159;

    /// <summary>The draw priority plane.</summary>
    public const int Pre2PriorityPlaneOffset = 163;

    /// <summary>The replaceable texture id.</summary>
    public const int Pre2ReplaceableIdOffset = 167;

    /// <summary>The PRE2 fixed fields, before any track, ending four bytes past the replaceable id.</summary>
    public const int Pre2FixedSize = Pre2ReplaceableIdOffset + 4;

    /// <summary>A whole PRE2 record with no track, the record size it states included.</summary>
    public const int Pre2RecordSize = RecordSizeField + NodeSize + Pre2FixedSize;

    /// <summary>The SEQS chunk a document carries when the caller states none, one standing sequence.</summary>
    public static IReadOnlyList<MdxSequenceWriter.Sequence> DefaultSequences { get; } =
        new[] { new MdxSequenceWriter.Sequence("Stand", 0, 1000) };

    /// <summary>The chunk tag a kind is written under.</summary>
    public static string ChunkTag(MdxEmitterKind kind) => kind switch
    {
        MdxEmitterKind.ParticleEmitter1 => PremTag,
        MdxEmitterKind.ParticleEmitter2 => Pre2Tag,
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "unknown emitter kind"),
    };

    /// <summary>
    /// The chunk payload for one emitter, the record its <see cref="MdxEmitterParameters.Kind"/>
    /// names. This is the payload alone, without a tag or a size, so a caller can drop it into a
    /// chunk list or into a map's model of its own making. <paramref name="textureId"/> is the
    /// emitter's index in the texture table the caller writes, and a PREM emitter has no field that
    /// could hold it.
    /// </summary>
    public static byte[] BuildPayload(MdxEmitterParameters emitter, int textureId = 0)
    {
        ArgumentNullException.ThrowIfNull(emitter);
        return emitter.Kind switch
        {
            MdxEmitterKind.ParticleEmitter1 => BuildPremPayload(emitter),
            MdxEmitterKind.ParticleEmitter2 => BuildPre2Payload(emitter, textureId),
            _ => throw new ArgumentOutOfRangeException(
                nameof(emitter), emitter.Kind, "unknown emitter kind"),
        };
    }

    /// <summary>
    /// One PREM record, a start counted record size, the node header, then the emission rate,
    /// gravity, longitude, latitude, the spawn model path, the lifetime and the speed. Node flags
    /// gain the spawn model bit when the path ends in .mdl or .tga, which is what the real record
    /// shows, where a .mdl path carries flag 0x8000 beside the 0x1000 emitter bit.
    /// </summary>
    public static byte[] BuildPremPayload(MdxEmitterParameters emitter)
    {
        ArgumentNullException.ThrowIfNull(emitter);
        Validate(emitter, MdxEmitterKind.ParticleEmitter1);

        var record = new byte[PremRecordSize];
        WriteRecordHeader(record, emitter, PremFlags(emitter));

        int o = FieldsOffset;
        BinaryPrimitives.WriteSingleLittleEndian(
            record.AsSpan(o + PremEmissionRateOffset, 4), emitter.EmissionRate);
        BinaryPrimitives.WriteSingleLittleEndian(
            record.AsSpan(o + PremGravityOffset, 4), emitter.Gravity);
        BinaryPrimitives.WriteSingleLittleEndian(
            record.AsSpan(o + PremLongitudeOffset, 4), ToRadians(emitter.Longitude));
        BinaryPrimitives.WriteSingleLittleEndian(
            record.AsSpan(o + PremLatitudeOffset, 4), ToRadians(emitter.Latitude));

        var path = Encoding.ASCII.GetBytes(emitter.SpawnModelPath);
        path.CopyTo(record.AsSpan(o + PremSpawnModelPathOffset, path.Length));

        BinaryPrimitives.WriteSingleLittleEndian(
            record.AsSpan(o + PremLifetimeOffset, 4), emitter.Lifetime);
        BinaryPrimitives.WriteSingleLittleEndian(
            record.AsSpan(o + PremSpeedOffset, 4), emitter.Speed);
        return record;
    }

    /// <summary>
    /// One PRE2 record, a start counted record size, the node header, then the eight fixed floats,
    /// the blend, the sheet and tail fields, the three segment colours, alphas and size
    /// multipliers, the four sheet intervals, and the texture id, squirt, priority plane and
    /// replaceable id. The middle colour and the middle alpha and size are the midpoint between the
    /// start and end values, the natural sample of the three stop ramp the chunk stores.
    /// </summary>
    public static byte[] BuildPre2Payload(MdxEmitterParameters emitter, int textureId)
    {
        ArgumentNullException.ThrowIfNull(emitter);
        if (textureId < 0)
            throw new ArgumentOutOfRangeException(
                nameof(textureId), textureId, "a texture id cannot be negative");

        Validate(emitter, MdxEmitterKind.ParticleEmitter2);

        var record = new byte[Pre2RecordSize];
        WriteRecordHeader(record, emitter, emitter.Flags);

        int o = FieldsOffset;
        BinaryPrimitives.WriteSingleLittleEndian(record.AsSpan(o + Pre2SpeedOffset, 4), emitter.Speed);
        BinaryPrimitives.WriteSingleLittleEndian(
            record.AsSpan(o + Pre2VariationOffset, 4), emitter.Variation);
        BinaryPrimitives.WriteSingleLittleEndian(
            record.AsSpan(o + Pre2LatitudeOffset, 4), ToRadians(emitter.Latitude));
        BinaryPrimitives.WriteSingleLittleEndian(
            record.AsSpan(o + Pre2GravityOffset, 4), emitter.Gravity);
        BinaryPrimitives.WriteSingleLittleEndian(
            record.AsSpan(o + Pre2LifetimeOffset, 4), emitter.Lifetime);
        BinaryPrimitives.WriteSingleLittleEndian(
            record.AsSpan(o + Pre2EmissionRateOffset, 4), emitter.EmissionRate);
        BinaryPrimitives.WriteSingleLittleEndian(
            record.AsSpan(o + Pre2LengthOffset, 4), emitter.Length);
        BinaryPrimitives.WriteSingleLittleEndian(
            record.AsSpan(o + Pre2WidthOffset, 4), emitter.Width);

        BinaryPrimitives.WriteUInt32LittleEndian(
            record.AsSpan(o + Pre2BlendOffset, 4), (uint)emitter.BlendMode);
        BinaryPrimitives.WriteUInt32LittleEndian(record.AsSpan(o + Pre2RowsOffset, 4), emitter.Rows);
        BinaryPrimitives.WriteUInt32LittleEndian(
            record.AsSpan(o + Pre2ColumnsOffset, 4), emitter.Columns);
        BinaryPrimitives.WriteUInt32LittleEndian(
            record.AsSpan(o + Pre2HeadOrTailOffset, 4), (uint)emitter.HeadOrTail);

        BinaryPrimitives.WriteSingleLittleEndian(
            record.AsSpan(o + Pre2TailLengthOffset, 4), emitter.TailLength);
        BinaryPrimitives.WriteSingleLittleEndian(
            record.AsSpan(o + Pre2TailTimeOffset, 4), emitter.TailTime);

        WriteColour(record, o + Pre2FirstSegmentColourOffset, emitter.StartColour);
        WriteColour(record, o + Pre2FirstSegmentColourOffset + 12, Midpoint(emitter.StartColour, emitter.EndColour));
        WriteColour(record, o + Pre2FirstSegmentColourOffset + 24, emitter.EndColour);

        record[o + Pre2FirstSegmentAlphaOffset] = emitter.StartColour.Alpha;
        record[o + Pre2FirstSegmentAlphaOffset + 1] = MidpointAlpha(emitter.StartColour, emitter.EndColour);
        record[o + Pre2FirstSegmentAlphaOffset + 2] = emitter.EndColour.Alpha;

        BinaryPrimitives.WriteSingleLittleEndian(
            record.AsSpan(o + Pre2FirstSegmentScalingOffset, 4), emitter.StartSize);
        BinaryPrimitives.WriteSingleLittleEndian(
            record.AsSpan(o + Pre2FirstSegmentScalingOffset + 4, 4), (emitter.StartSize + emitter.EndSize) / 2f);
        BinaryPrimitives.WriteSingleLittleEndian(
            record.AsSpan(o + Pre2FirstSegmentScalingOffset + 8, 4), emitter.EndSize);

        WriteInterval(record, o + Pre2HeadLifetimeOffset, emitter.Sheet.HeadLifetime);
        WriteInterval(record, o + Pre2HeadDecayOffset, emitter.Sheet.HeadDecay);
        WriteInterval(record, o + Pre2TailLifetimeOffset, emitter.Sheet.TailLifetime);
        WriteInterval(record, o + Pre2TailDecayOffset, emitter.Sheet.TailDecay);

        BinaryPrimitives.WriteInt32LittleEndian(
            record.AsSpan(o + Pre2TextureIdOffset, 4), textureId);
        BinaryPrimitives.WriteUInt32LittleEndian(
            record.AsSpan(o + Pre2SquirtOffset, 4), emitter.Squirt ? 1u : 0u);
        BinaryPrimitives.WriteUInt32LittleEndian(
            record.AsSpan(o + Pre2PriorityPlaneOffset, 4), emitter.PriorityPlane);
        BinaryPrimitives.WriteInt32LittleEndian(
            record.AsSpan(o + Pre2ReplaceableIdOffset, 4), emitter.ReplaceableId);
        return record;
    }

    /// <summary>
    /// A complete effect document, VERS then SEQS then TEXS then MTLS then one chunk per emitter in
    /// the order given. The texture table holds each distinct texture path once, in the order the
    /// emitters name them, so the first emitter that names a path decides its id, and the material
    /// for a path takes its blend from the first emitter that named it.
    ///
    /// Two emitters cannot share an objectId, because an objectId is an index in the node array and
    /// two nodes at one index is a broken model, so a duplicate is refused rather than written. No
    /// PIVT chunk is written, so <see cref="MdxDocument.NodeCount"/> reads zero and a caller who
    /// composes a skeleton supplies the pivot table that matches its node array.
    /// </summary>
    public static MdxDocument BuildDocument(
        IReadOnlyList<MdxEmitterParameters> emitters,
        IReadOnlyList<MdxSequenceWriter.Sequence>? sequences = null,
        uint version = ModelVersion)
    {
        ArgumentNullException.ThrowIfNull(emitters);
        if (emitters.Count == 0)
            throw new ArgumentException("a document needs at least one emitter", nameof(emitters));

        var seen = new HashSet<int>();
        foreach (var emitter in emitters)
        {
            if (emitter is null)
                throw new ArgumentException("an emitter cannot be null", nameof(emitters));
            if (!seen.Add(emitter.ObjectId))
                throw new ArgumentException(
                    $"two emitters share node id {emitter.ObjectId}, which cannot index one node array",
                    nameof(emitters));
        }

        var textures = new List<string>();
        foreach (var emitter in emitters)
            if (!textures.Any(t => string.Equals(t, emitter.TexturePath, StringComparison.OrdinalIgnoreCase)))
                textures.Add(emitter.TexturePath);

        var textureEntries = new List<MdxTextureEntry>(textures.Count);
        var materials = new List<MdxMaterialEntry>(textures.Count);
        for (int i = 0; i < textures.Count; i++)
        {
            textureEntries.Add(new MdxTextureEntry(textures[i]));
            var owner = emitters.First(e =>
                string.Equals(e.TexturePath, textures[i], StringComparison.OrdinalIgnoreCase));
            materials.Add(MdxMaterialEntry.Single(MdxLayer.Opaque(i, ToLayerFilterMode(owner.BlendMode))));
        }

        var chunks = new List<MdxDocument.Chunk>(4 + emitters.Count)
        {
            new("VERS", VersionPayload(version)),
            new("SEQS", MdxSequenceWriter.BuildPayload(sequences ?? DefaultSequences)),
            new("TEXS", MdxMaterialWriter.EncodeTexs(textureEntries)),
            new("MTLS", MdxMaterialWriter.EncodeMtls(materials)),
        };

        foreach (var emitter in emitters)
        {
            int textureId = textures.FindIndex(t =>
                string.Equals(t, emitter.TexturePath, StringComparison.OrdinalIgnoreCase));
            byte[] payload = emitter.Kind switch
            {
                MdxEmitterKind.ParticleEmitter1 => BuildPremPayload(emitter),
                MdxEmitterKind.ParticleEmitter2 => BuildPre2Payload(emitter, textureId),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(emitters), emitter.Kind, "unknown emitter kind"),
            };
            chunks.Add(new MdxDocument.Chunk(ChunkTag(emitter.Kind), payload));
        }

        return MdxDocument.Create(chunks);
    }

    private static byte[] VersionPayload(uint version)
    {
        var payload = new byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(payload, version);
        return payload;
    }

    /// <summary>
    /// The record size field, the node size field and the node header, the bytes both chunks share.
    /// The node size states the header and its own field and no transform tracks, so the node is 96
    /// bytes and the emitter fields begin at byte 100.
    /// </summary>
    private static void WriteRecordHeader(byte[] record, MdxEmitterParameters emitter, uint flags)
    {
        BinaryPrimitives.WriteInt32LittleEndian(record.AsSpan(0, 4), record.Length);
        BinaryPrimitives.WriteInt32LittleEndian(record.AsSpan(NodeSizeField, 4), NodeSize);

        var name = Encoding.ASCII.GetBytes(emitter.Name);
        int nameLength = Math.Min(name.Length, NodeNameLength);
        name.AsSpan(0, nameLength).CopyTo(record.AsSpan(NodeNameOffset, NodeNameLength));

        BinaryPrimitives.WriteInt32LittleEndian(record.AsSpan(ObjectIdOffset, 4), emitter.ObjectId);
        BinaryPrimitives.WriteInt32LittleEndian(record.AsSpan(ParentIdOffset, 4), emitter.ParentId);
        BinaryPrimitives.WriteUInt32LittleEndian(record.AsSpan(NodeFlagsOffset, 4), flags);
    }

    /// <summary>
    /// The PREM node flags, the stated flags plus the spawn model bit the path extension implies,
    /// the .mdl and .tga bits the node flag table defines for a particle emitter 1.
    /// </summary>
    private static uint PremFlags(MdxEmitterParameters emitter)
    {
        uint flags = emitter.Flags;
        if (emitter.SpawnModelPath.EndsWith(".mdl", StringComparison.OrdinalIgnoreCase))
            flags |= EmitterUsesMdlFlag;
        if (emitter.SpawnModelPath.EndsWith(".tga", StringComparison.OrdinalIgnoreCase))
            flags |= EmitterUsesTgaFlag;
        return flags;
    }

    private static void WriteColour(byte[] record, int offset, MdxEmitterColour colour)
    {
        BinaryPrimitives.WriteSingleLittleEndian(record.AsSpan(offset, 4), colour.Red);
        BinaryPrimitives.WriteSingleLittleEndian(record.AsSpan(offset + 4, 4), colour.Green);
        BinaryPrimitives.WriteSingleLittleEndian(record.AsSpan(offset + 8, 4), colour.Blue);
    }

    private static void WriteInterval(byte[] record, int offset, MdxEmitterUvInterval interval)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(record.AsSpan(offset, 4), interval.Start);
        BinaryPrimitives.WriteUInt32LittleEndian(record.AsSpan(offset + 4, 4), interval.End);
        BinaryPrimitives.WriteUInt32LittleEndian(record.AsSpan(offset + 8, 4), interval.Repeat);
    }

    private static MdxEmitterColour Midpoint(MdxEmitterColour a, MdxEmitterColour b) =>
        new((a.Red + b.Red) / 2f, (a.Green + b.Green) / 2f, (a.Blue + b.Blue) / 2f);

    private static byte MidpointAlpha(MdxEmitterColour a, MdxEmitterColour b) =>
        (byte)((a.Alpha + b.Alpha) / 2);

    private static float ToRadians(float degrees) => (float)(degrees * Math.PI / 180.0);

    /// <summary>
    /// Maps the emitter blend to the material layer blend of the same name. The two value spaces are
    /// different, so this is a mapping and never a cast.
    /// </summary>
    private static FilterMode ToLayerFilterMode(MdxEmitterBlend blend) => blend switch
    {
        MdxEmitterBlend.Blend => FilterMode.Blend,
        MdxEmitterBlend.Additive => FilterMode.Additive,
        MdxEmitterBlend.Modulate => FilterMode.Modulate,
        MdxEmitterBlend.Modulate2x => FilterMode.Modulate2x,
        MdxEmitterBlend.AddAlpha => FilterMode.AddAlpha,
        _ => throw new ArgumentOutOfRangeException(nameof(blend), blend, "unknown emitter blend"),
    };

    /// <summary>
    /// Refuses every value outside the range the chunk can hold or the format defines, naming the
    /// field, so a caller learns which parameter is wrong rather than receiving a silent correction.
    /// </summary>
    private static void Validate(MdxEmitterParameters emitter, MdxEmitterKind kind)
    {
        if (string.IsNullOrWhiteSpace(emitter.Name))
            throw new ArgumentException("an emitter needs a name", nameof(emitter));
        if (string.IsNullOrWhiteSpace(emitter.TexturePath))
            throw new ArgumentException("an emitter needs a texture path", nameof(emitter));
        if (Encoding.ASCII.GetByteCount(emitter.TexturePath) > MdxMaterialWriter.TexturePathLength)
            throw new ArgumentOutOfRangeException(
                nameof(emitter),
                emitter.TexturePath.Length,
                $"a texture path longer than {MdxMaterialWriter.TexturePathLength} bytes cannot fit TEXS");

        if (emitter.ObjectId < 0)
            throw new ArgumentOutOfRangeException(
                nameof(emitter), emitter.ObjectId, "an emitter node id cannot be negative");
        if (emitter.ParentId < -1)
            throw new ArgumentOutOfRangeException(
                nameof(emitter), emitter.ParentId, "a parent id is -1 for a root node and not lower");
        if (emitter.ParentId == emitter.ObjectId)
            throw new ArgumentException(
                $"emitter '{emitter.Name}' names itself as its parent", nameof(emitter));

        if ((emitter.Flags & ParticleEmitterFlag) == 0)
            throw new ArgumentOutOfRangeException(
                nameof(emitter),
                emitter.Flags,
                $"node flags must carry the ParticleEmitter bit 0x{ParticleEmitterFlag:X}");

        RequirePositive(emitter.Lifetime, "lifetime", nameof(emitter));
        RequirePositive(emitter.EmissionRate, "emission rate", nameof(emitter));
        RequireNonNegative(emitter.Speed, "speed", nameof(emitter));
        RequireFinite(emitter.Gravity, "gravity", nameof(emitter));
        RequireAngle(emitter.Latitude, 180f, "latitude", nameof(emitter));
        RequireAngle(emitter.Longitude, 360f, "longitude", nameof(emitter));

        if (Encoding.ASCII.GetByteCount(emitter.SpawnModelPath) > PremSpawnModelPathTextLength)
            throw new ArgumentOutOfRangeException(
                nameof(emitter),
                emitter.SpawnModelPath.Length,
                $"a spawn model path longer than {PremSpawnModelPathTextLength} bytes cannot fit the PREM path field");

        if (kind != MdxEmitterKind.ParticleEmitter2) return;

        RequireNonNegative(emitter.Variation, "variation", nameof(emitter));
        RequireNonNegative(emitter.Width, "width", nameof(emitter));
        RequireNonNegative(emitter.Length, "length", nameof(emitter));
        RequireNonNegative(emitter.StartSize, "start size", nameof(emitter));
        RequireNonNegative(emitter.EndSize, "end size", nameof(emitter));
        RequireNonNegative(emitter.TailLength, "tail length", nameof(emitter));
        RequireNonNegative(emitter.TailTime, "tail time", nameof(emitter));

        if (emitter.Rows == 0)
            throw new ArgumentOutOfRangeException(
                nameof(emitter), emitter.Rows, "a sprite sheet needs at least one row");
        if (emitter.Columns == 0)
            throw new ArgumentOutOfRangeException(
                nameof(emitter), emitter.Columns, "a sprite sheet needs at least one column");
        if (!Enum.IsDefined(emitter.HeadOrTail))
            throw new ArgumentOutOfRangeException(
                nameof(emitter), emitter.HeadOrTail, "unknown head or tail value");
        if (!Enum.IsDefined(emitter.BlendMode))
            throw new ArgumentOutOfRangeException(
                nameof(emitter), emitter.BlendMode, "unknown emitter blend");
        if (emitter.ReplaceableId < 0)
            throw new ArgumentOutOfRangeException(
                nameof(emitter), emitter.ReplaceableId, "a replaceable id cannot be negative");

        RequireColour(emitter.StartColour, "start colour", nameof(emitter));
        RequireColour(emitter.EndColour, "end colour", nameof(emitter));
    }

    private static void RequireColour(MdxEmitterColour colour, string field, string paramName)
    {
        RequireUnit(colour.Red, field, paramName);
        RequireUnit(colour.Green, field, paramName);
        RequireUnit(colour.Blue, field, paramName);
    }

    private static void RequireUnit(float value, string field, string paramName)
    {
        if (!float.IsFinite(value) || value < 0f || value > 1f)
            throw new ArgumentOutOfRangeException(
                paramName, value, $"the {field} must be between 0 and 1");
    }

    private static void RequireFinite(float value, string field, string paramName)
    {
        if (!float.IsFinite(value))
            throw new ArgumentOutOfRangeException(
                paramName, value, $"the {field} must be a finite number");
    }

    private static void RequirePositive(float value, string field, string paramName)
    {
        if (!float.IsFinite(value) || value <= 0f)
            throw new ArgumentOutOfRangeException(
                paramName, value, $"the {field} must be greater than zero");
    }

    private static void RequireNonNegative(float value, string field, string paramName)
    {
        if (!float.IsFinite(value) || value < 0f)
            throw new ArgumentOutOfRangeException(
                paramName, value, $"the {field} cannot be negative");
    }

    private static void RequireAngle(float degrees, float maximum, string field, string paramName)
    {
        if (!float.IsFinite(degrees) || degrees < 0f || degrees > maximum)
            throw new ArgumentOutOfRangeException(
                paramName, degrees, $"the {field} must be between 0 and {maximum} degrees");
    }
}
