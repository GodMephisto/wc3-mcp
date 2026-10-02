// src/Wc3.Modeling/MdxParser.cs
using System.Buffers.Binary;
using System.Numerics;
using System.Text;

namespace Wc3.Modeling;

/// <summary>
/// Reader for the binary MDX format ("MDLX" chunked). Extracts per-geoset
/// positions/normals/uvs/triangles plus the material→texture mapping a static
/// render needs, and — best effort — the animation data a posed render needs:
/// sequences (SEQS), the bone/helper hierarchy with keyframe tracks
/// (BONE/HELP + KGTR/KGRT/KGSC), pivots (PIVT) and per-vertex bone attachments
/// (GNDX/MTGC/MATS, or the v900+ SKIN weights). Animation parsing is strictly
/// non-fatal: any inconsistency drops the skeleton and the model still loads as
/// static geometry. Particles, ribbons, cameras etc. are skipped via chunk sizes.
/// Handles classic v800 and the Reforged v900/v1000 layout differences
/// (shader name, LOD name, TANG/SKIN).
/// </summary>
internal static class MdxParser
{
    public static Model3D Parse(byte[] bytes)
    {
        if (bytes.Length < 8 || ReadTag(bytes, 0) != "MDLX")
            throw new InvalidDataException("Not an MDX file (missing MDLX magic).");

        uint version = 800;
        var textures = new List<string>();
        var materials = new List<(int TextureId, FilterMode Filter)>(); // material index -> base-layer texture + blend mode
        var raw = new List<RawGeoset>();

        var sequences = new List<ModelSequence>();
        var rawNodes = new List<RawNode>();
        var pivots = new List<Vector3>();
        bool animBroken = false; // any animation-parse surprise → no skeleton, geometry unaffected

        int pos = 4;
        while (pos + 8 <= bytes.Length)
        {
            string tag = ReadTag(bytes, pos);
            int size = checked((int)ReadU32(bytes, pos + 4));
            int payload = pos + 8;
            int end = payload + size;
            if (size < 0 || end > bytes.Length)
                break; // truncated trailing chunk — keep what we already have

            switch (tag)
            {
                case "VERS": if (size >= 4) version = ReadU32(bytes, payload); break;
                case "TEXS": ParseTexs(bytes, payload, end, textures); break;
                case "MTLS": ParseMtls(bytes, payload, end, materials); break;
                case "GEOS": ParseGeos(bytes, payload, end, version, raw); break;
                case "SEQS":
                    try { ParseSeqs(bytes, payload, end, sequences); } catch { animBroken = true; }
                    break;
                case "BONE":
                    try { ParseNodeChunk(bytes, payload, end, trailingBytes: 8, rawNodes); } catch { animBroken = true; }
                    break;
                case "HELP":
                    try { ParseNodeChunk(bytes, payload, end, trailingBytes: 0, rawNodes); } catch { animBroken = true; }
                    break;
                case "PIVT":
                    try { ParsePivots(bytes, payload, end, pivots); } catch { animBroken = true; }
                    break;
            }
            pos = end;
        }

        ModelSkeleton? skeleton = null;
        if (!animBroken && rawNodes.Count > 0 && sequences.Count > 0)
        {
            var nodes = new List<ModelNode>(rawNodes.Count);
            foreach (var n in rawNodes)
            {
                var pivot = n.ObjectId >= 0 && n.ObjectId < pivots.Count ? pivots[n.ObjectId] : Vector3.Zero;
                nodes.Add(new ModelNode(n.Name, n.ObjectId, n.ParentId, pivot, n.Translation, n.Rotation, n.Scaling));
            }
            skeleton = new ModelSkeleton(sequences, nodes);
        }

        var geosets = new List<Geoset>(raw.Count);
        foreach (var g in raw)
        {
            var (textureId, filter) = g.MaterialId >= 0 && g.MaterialId < materials.Count
                ? materials[g.MaterialId]
                : (-1, FilterMode.None);
            if (textureId < 0 || textureId >= textures.Count) textureId = -1;

            int[]? skinBones = null;
            float[]? skinWeights = null;
            if (skeleton is not null)
                (skinBones, skinWeights) = BuildSkin(g); // never throws; null on inconsistency
            geosets.Add(new Geoset(g.Vertices, g.Normals, g.Uvs, g.Indices, textureId, skinBones, skinWeights, filter));
        }
        return new Model3D(geosets, textures, skeleton);
    }

    private sealed record RawGeoset(
        float[] Vertices, float[] Normals, float[] Uvs, int[] Indices, int MaterialId,
        byte[]? VertexGroups, int[]? MatrixGroupCounts, int[]? MatrixIndices, byte[]? SkinBytes);

    private sealed record RawNode(
        string Name, int ObjectId, int ParentId,
        AnimTrack<Vector3>? Translation, AnimTrack<Quaternion>? Rotation, AnimTrack<Vector3>? Scaling);

    /// <summary>SEQS: fixed 132-byte records (char[80] name, u32 start, u32 end, moveSpeed, flags, rarity, syncPoint, extent).</summary>
    private static void ParseSeqs(byte[] b, int pos, int end, List<ModelSequence> sequences)
    {
        while (pos + 132 <= end)
        {
            string name = ReadFixedString(b, pos, 80);
            int start = checked((int)ReadU32(b, pos + 80));
            int stop = checked((int)ReadU32(b, pos + 84));
            sequences.Add(new ModelSequence(name, start, stop));
            pos += 132;
        }
    }

    /// <summary>PIVT: float3 per node, indexed by objectId.</summary>
    private static void ParsePivots(byte[] b, int pos, int end, List<Vector3> pivots)
    {
        while (pos + 12 <= end)
        {
            pivots.Add(new Vector3(ReadF32(b, pos), ReadF32(b, pos + 4), ReadF32(b, pos + 8)));
            pos += 12;
        }
    }

    /// <summary>
    /// BONE/HELP: repeated generic node structs — inclusiveSize, char[80] name,
    /// u32 objectId, i32 parentId, u32 flags, then KG** tracks up to inclusiveSize.
    /// BONE trails each node with u32 geosetId + u32 geosetAnimId
    /// (<paramref name="trailingBytes"/> = 8), which we skip.
    /// </summary>
    private static void ParseNodeChunk(byte[] b, int pos, int end, int trailingBytes, List<RawNode> nodes)
    {
        while (pos + 96 <= end)
        {
            int inclusiveSize = checked((int)ReadU32(b, pos));
            int nodeEnd = pos + inclusiveSize;
            if (inclusiveSize < 96 || nodeEnd > end)
                break; // malformed node — keep the ones already parsed

            string name = ReadFixedString(b, pos + 4, 80);
            int objectId = checked((int)ReadU32(b, pos + 84));
            int parentId = unchecked((int)ReadU32(b, pos + 88));
            // flags at pos + 92 — not needed for posing

            AnimTrack<Vector3>? translation = null, scaling = null;
            AnimTrack<Quaternion>? rotation = null;
            int p = pos + 96;
            while (p + 16 <= nodeEnd)
            {
                string tag = ReadTag(b, p);
                var track = tag is "KGTR" or "KGRT" or "KGSC"
                    ? ReadTrack(b, ref p, nodeEnd, floatsPerValue: tag == "KGRT" ? 4 : 3)
                    : null;
                if (track is null)
                    break; // unknown/overrunning sub-chunk: its size is unknowable — stop tracks, node still usable

                switch (tag)
                {
                    case "KGTR": translation = ToVec3Track(track); break;
                    case "KGSC": scaling = ToVec3Track(track); break;
                    case "KGRT": rotation = ToQuatTrack(track); break;
                }
            }

            nodes.Add(new RawNode(name, objectId, parentId, translation, rotation, scaling));
            pos = nodeEnd + trailingBytes;
        }
    }

    private sealed record RawTrack(int[] Frames, float[][] Values, int GlobalSequenceId);

    /// <summary>
    /// KG** track: tag, u32 numKeys, u32 interpolation (0 none, 1 linear, 2 hermite,
    /// 3 bezier), i32 globalSeqId, then per key: i32 frame + value floats
    /// (+ inTan/outTan of the same width when interpolation &gt; 1 — read past, unused:
    /// the poser approximates everything linearly). Null when the track overruns.
    /// </summary>
    private static RawTrack? ReadTrack(byte[] b, ref int p, int limit, int floatsPerValue)
    {
        int numKeys = checked((int)ReadU32(b, p + 4));
        int interpolation = checked((int)ReadU32(b, p + 8));
        int globalSeqId = unchecked((int)ReadU32(b, p + 12));
        int keySize = 4 + floatsPerValue * 4 + (interpolation > 1 ? floatsPerValue * 8 : 0);
        int data = p + 16;
        if (numKeys < 0 || keySize <= 0 || data + (long)numKeys * keySize > limit)
            return null;

        var frames = new int[numKeys];
        var values = new float[numKeys][];
        for (int i = 0; i < numKeys; i++)
        {
            int k = data + i * keySize;
            frames[i] = unchecked((int)ReadU32(b, k));
            var v = new float[floatsPerValue];
            for (int f = 0; f < floatsPerValue; f++)
                v[f] = ReadF32(b, k + 4 + f * 4);
            values[i] = v;
        }
        p = data + numKeys * keySize;
        return new RawTrack(frames, values, globalSeqId);
    }

    private static AnimTrack<Vector3> ToVec3Track(RawTrack t)
    {
        var keys = new AnimKey<Vector3>[t.Frames.Length];
        for (int i = 0; i < keys.Length; i++)
            keys[i] = new AnimKey<Vector3>(t.Frames[i], new Vector3(t.Values[i][0], t.Values[i][1], t.Values[i][2]));
        return new AnimTrack<Vector3>(keys, t.GlobalSequenceId);
    }

    private static AnimTrack<Quaternion> ToQuatTrack(RawTrack t)
    {
        var keys = new AnimKey<Quaternion>[t.Frames.Length];
        for (int i = 0; i < keys.Length; i++)
            keys[i] = new AnimKey<Quaternion>(
                t.Frames[i], new Quaternion(t.Values[i][0], t.Values[i][1], t.Values[i][2], t.Values[i][3]));
        return new AnimTrack<Quaternion>(keys, t.GlobalSequenceId);
    }

    /// <summary>
    /// Normalizes a geoset's bone attachments to 4 weighted slots per vertex.
    /// Prefers v900+ SKIN (true blend weights: 4 node ids + 4 byte weights per
    /// vertex); classic GNDX/MTGC/MATS matrix groups become a rigid average over
    /// the group's first ≤4 bones (classic MDX stores no per-vertex weights).
    /// Returns (null, null) when the data is absent or inconsistent — bind pose.
    /// </summary>
    private static (int[]?, float[]?) BuildSkin(RawGeoset g)
    {
        int vertexCount = g.Vertices.Length / 3;
        if (vertexCount == 0) return (null, null);

        if (g.SkinBytes is { } skin && skin.Length == vertexCount * 8)
        {
            var bones = new int[vertexCount * 4];
            var weights = new float[vertexCount * 4];
            for (int v = 0; v < vertexCount; v++)
            {
                int o = v * 8;
                float sum = skin[o + 4] + skin[o + 5] + skin[o + 6] + skin[o + 7];
                for (int s = 0; s < 4; s++)
                {
                    bones[v * 4 + s] = skin[o + s];
                    weights[v * 4 + s] = sum > 0f ? skin[o + 4 + s] / sum : 0f; // all-zero → bind-pose vertex
                }
            }
            return (bones, weights);
        }

        if (g.VertexGroups is { } groups && groups.Length == vertexCount
            && g.MatrixGroupCounts is { } counts && g.MatrixIndices is { } matrices)
        {
            var offsets = new int[counts.Length];
            long running = 0;
            for (int i = 0; i < counts.Length; i++)
            {
                if (counts[i] < 0) return (null, null);
                offsets[i] = (int)running;
                running += counts[i];
            }
            if (running > matrices.Length) return (null, null);

            var bones = new int[vertexCount * 4];
            var weights = new float[vertexCount * 4];
            for (int v = 0; v < vertexCount; v++)
            {
                int group = groups[v];
                if (group >= counts.Length) return (null, null); // corrupt grouping — whole geoset to bind
                int take = Math.Min(counts[group], 4);
                for (int s = 0; s < take; s++)
                {
                    bones[v * 4 + s] = matrices[offsets[group] + s];
                    weights[v * 4 + s] = 1f / take;
                }
            }
            return (bones, weights);
        }

        return (null, null);
    }

    /// <summary>TEXS: fixed 268-byte entries (u32 replaceableId, char[260] path, u32 flags).</summary>
    private static void ParseTexs(byte[] b, int pos, int end, List<string> textures)
    {
        while (pos + 268 <= end)
        {
            uint replaceableId = ReadU32(b, pos);
            string path = ReadFixedString(b, pos + 4, 260);
            textures.Add(path.Length > 0 ? path : $"ReplaceableId:{replaceableId}");
            pos += 268;
        }
    }

    /// <summary>
    /// MTLS: per-material inclusiveSize records; we only want each material's base
    /// (first) layer — its texture id and filter mode. Layer 0 is the surface the
    /// game draws first; overlay layers (team glow etc.) don't decide the geoset's
    /// blend mode.
    /// </summary>
    private static void ParseMtls(byte[] b, int pos, int end, List<(int TextureId, FilterMode Filter)> materials)
    {
        while (pos + 8 <= end)
        {
            int inclusiveSize = checked((int)ReadU32(b, pos));
            int matEnd = pos + inclusiveSize;
            if (inclusiveSize < 12 || matEnd > end) break;

            int p = pos + 12; // inclusiveSize, priorityPlane, flags
            // v900/v1000 insert a char[80] shader name before LAYS.
            if (p + 4 <= matEnd && ReadTag(b, p) != "LAYS" && p + 84 <= matEnd && ReadTag(b, p + 80) == "LAYS")
                p += 80;

            int texId = -1;
            var filter = FilterMode.None;
            if (p + 8 <= matEnd && ReadTag(b, p) == "LAYS")
            {
                uint layerCount = ReadU32(b, p + 4);
                int lp = p + 8;
                // Layer: inclusiveSize, filterMode, shadingFlags, textureId, ...
                if (layerCount > 0 && lp + 16 <= matEnd)
                {
                    filter = ToFilterMode(ReadU32(b, lp + 4));
                    texId = checked((int)ReadU32(b, lp + 12));
                }
            }
            materials.Add((texId, filter));
            pos = matEnd;
        }
    }

    /// <summary>MDX filter modes are u32 0-6; anything else (corrupt/future) degrades to opaque.</summary>
    private static FilterMode ToFilterMode(uint value)
        => value <= (uint)FilterMode.Modulate2x ? (FilterMode)value : FilterMode.None;

    private static void ParseGeos(byte[] b, int pos, int end, uint version, List<RawGeoset> geosets)
    {
        while (pos + 4 <= end)
        {
            int inclusiveSize = checked((int)ReadU32(b, pos));
            int gEnd = pos + inclusiveSize;
            if (inclusiveSize < 4 || gEnd > end) break;
            geosets.Add(ParseGeoset(b, pos + 4, gEnd, version));
            pos = gEnd;
        }
    }

    private static RawGeoset ParseGeoset(byte[] b, int pos, int gEnd, uint version)
    {
        float[] vertices = [], normals = [], uvs = [];
        int[] indices = [];
        int materialId = -1;
        byte[]? vertexGroups = null, skinBytes = null;
        int[]? matrixGroupCounts = null, matrixIndices = null;

        int p = pos;
        while (p + 8 <= gEnd)
        {
            string tag = ReadTag(b, p);
            int count = checked((int)ReadU32(b, p + 4));
            int data = p + 8;
            if (count < 0) throw new InvalidDataException($"Corrupt geoset: negative {tag} count.");

            switch (tag)
            {
                case "VRTX": vertices = ReadFloats(b, data, count * 3, gEnd); p = data + count * 12; break;
                case "NRMS": normals = ReadFloats(b, data, count * 3, gEnd); p = data + count * 12; break;
                case "PTYP": p = data + count * 4; break; // primitive types (4 = triangles; WC3 only uses triangles)
                case "PCNT": p = data + count * 4; break;
                case "PVTX":
                    if (data + count * 2 > gEnd) throw new InvalidDataException("Corrupt geoset: PVTX overruns.");
                    indices = new int[count];
                    for (int i = 0; i < count; i++) indices[i] = ReadU16(b, data + i * 2);
                    p = data + count * 2;
                    break;
                case "GNDX": // one uint8 matrix-group index per vertex
                    if (data + count <= gEnd) vertexGroups = b[data..(data + count)];
                    p = data + count;
                    break;
                case "MTGC": // per-group count of bone indices
                    matrixGroupCounts = TryReadInts(b, data, count, gEnd);
                    p = data + count * 4;
                    break;
                case "MATS":
                    matrixIndices = TryReadInts(b, data, count, gEnd); // flat bone objectId list
                    p = data + count * 4;
                    if (p + 4 <= gEnd) materialId = checked((int)ReadU32(b, p));
                    // Skip the untagged section (materialId, selectionGroup, selectionFlags,
                    // v900+ lod + char[80] name, extents) to reach the next tagged chunk.
                    p = SkipUntaggedGeosetSection(b, p, gEnd, version);
                    break;
                case "TANG": p = data + count * 16; break; // v900+: float[4] per vertex
                case "SKIN": // v900+: per-vertex 4 bone ids + 4 byte weights
                    if (data + count <= gEnd) skinBytes = b[data..(data + count)];
                    p = data + count;
                    break;
                case "UVAS":
                    // count = number of UV sets, each stored as a following UVBS chunk.
                    p = data;
                    for (int s = 0; s < count && p + 8 <= gEnd; s++)
                    {
                        if (ReadTag(b, p) != "UVBS") break;
                        int n = checked((int)ReadU32(b, p + 4));
                        if (n < 0) throw new InvalidDataException("Corrupt geoset: negative UVBS count.");
                        if (s == 0) uvs = ReadFloats(b, p + 8, n * 2, gEnd);
                        p = p + 8 + n * 8;
                    }
                    p = gEnd; // nothing after the UV sets that we need
                    break;
                default:
                    p = gEnd; // unknown tag — stop rather than misparse
                    break;
            }
        }
        return new RawGeoset(
            vertices, normals, uvs, indices, materialId,
            vertexGroups, matrixGroupCounts, matrixIndices, skinBytes);
    }

    /// <summary>
    /// After MATS + materialId the geoset carries selection info, (v900+) LOD name,
    /// and per-sequence extents with no tags. Compute the exact skip for the known
    /// layouts, falling back to a byte scan for 'UVAS'/'TANG'/'SKIN' if that lands
    /// somewhere unrecognizable.
    /// </summary>
    private static int SkipUntaggedGeosetSection(byte[] b, int p, int gEnd, uint version)
    {
        int q = p + 12; // materialId, selectionGroup, selectionFlags
        if (version >= 900) q += 84; // u32 lod + char[80] lod name
        q += 28; // extent: radius + min[3] + max[3]
        if (q + 4 <= gEnd)
        {
            int seqExtents = checked((int)ReadU32(b, q));
            if (seqExtents >= 0) q += 4 + seqExtents * 28;
        }
        if (q + 4 <= gEnd && ReadTag(b, q) is "UVAS" or "TANG" or "SKIN")
            return q;

        // Fallback: scan for the next known tag within the geoset.
        for (int i = p; i + 4 <= gEnd; i++)
        {
            if (ReadTag(b, i) is "UVAS" or "TANG" or "SKIN")
                return i;
        }
        return gEnd;
    }

    private static float[] ReadFloats(byte[] b, int pos, int count, int limit)
    {
        if (pos + count * 4 > limit) throw new InvalidDataException("Corrupt geoset: float array overruns.");
        var result = new float[count];
        for (int i = 0; i < count; i++)
            result[i] = BinaryPrimitives.ReadSingleLittleEndian(b.AsSpan(pos + i * 4, 4));
        return result;
    }

    /// <summary>Defensive u32→int array read for animation data: null (not throw) on overrun.</summary>
    private static int[]? TryReadInts(byte[] b, int pos, int count, int limit)
    {
        if (count < 0 || pos + (long)count * 4 > limit) return null;
        var result = new int[count];
        for (int i = 0; i < count; i++)
            result[i] = unchecked((int)ReadU32(b, pos + i * 4));
        return result;
    }

    private static string ReadTag(byte[] b, int pos)
        => Encoding.ASCII.GetString(b, pos, 4);

    private static uint ReadU32(byte[] b, int pos)
        => BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(pos, 4));

    private static float ReadF32(byte[] b, int pos)
        => BinaryPrimitives.ReadSingleLittleEndian(b.AsSpan(pos, 4));

    private static ushort ReadU16(byte[] b, int pos)
        => BinaryPrimitives.ReadUInt16LittleEndian(b.AsSpan(pos, 2));

    private static string ReadFixedString(byte[] b, int pos, int length)
    {
        int nul = Array.IndexOf(b, (byte)0, pos, length);
        int len = nul < 0 ? length : nul - pos;
        return Encoding.UTF8.GetString(b, pos, len);
    }
}
