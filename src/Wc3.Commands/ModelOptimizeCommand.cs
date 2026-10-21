// src/Wc3.Commands/ModelOptimizeCommand.cs
using System.Buffers.Binary;
using System.Numerics;
using Wc3.Modeling;

namespace Wc3.Commands;

/// <summary>What an optimise pass changed, before and after, for the report.</summary>
public sealed record ModelOptimizeResult(
    string Path,
    int Geosets,
    int VerticesBefore,
    int VerticesAfter,
    int FacesBefore,
    int FacesAfter,
    int EffectsRemoved,
    long BytesBefore,
    long BytesAfter,
    IReadOnlyList<string> Diagnostics)
{
    public int VerticesRemoved => VerticesBefore - VerticesAfter;
    public int FacesRemoved => FacesBefore - FacesAfter;
}

/// <summary>
/// Shrinks a model by decimating each geoset's geometry with the quadric error metric, and writes a
/// new MDX. It works at the chunk level through <see cref="MdxDocument"/> and <see cref="MdxGeoset"/>,
/// so materials, textures, sequences, bones, emitters and cameras are written back byte for byte and
/// only the VRTX, NRMS, PVTX, GNDX and UVBS arrays of a reduced geoset change.
///
/// A geoset with no VRTX or PVTX, or one that does not reduce, is left untouched. The result carries
/// a before and after count so the caller can report the saving.
///
/// With <c>stripEffects</c> on it also drops the effect chunks, particle emitters (PREM), particle
/// emitters 2 (PRE2), ribbons (RIBB) and corns (CORN), so a model can be shrunk further when its
/// effects are not wanted. This changes how the model looks, so it is off by default.
/// </summary>
public static class ModelOptimizeCommand
{
    public static ModelOptimizeResult ExecutePath(string path, float ratio, string outPath, bool stripEffects = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (!File.Exists(path)) throw new FileNotFoundException($"model not found: {path}", path);
        var result = Execute(File.ReadAllBytes(path), path, ratio, out var output, stripEffects);
        File.WriteAllBytes(outPath, output);
        return result with { Path = outPath };
    }

    public static ModelOptimizeResult Execute(byte[] bytes, string name, float ratio, out byte[] output,
        bool stripEffects = false)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        var diagnostics = new List<string>();

        MdxDocument doc;
        try
        {
            doc = MdxDocument.Parse(bytes);
        }
        catch (Exception ex)
        {
            output = bytes;
            return new ModelOptimizeResult(name, 0, 0, 0, 0, 0, 0, bytes.LongLength, bytes.LongLength,
                new[] { $"could not parse '{name}': {ex.Message}" });
        }

        uint version = ReadVersion(doc);

        int texturesRemoved = PruneTextures(doc);
        if (texturesRemoved > 0) diagnostics.Add($"pruned {texturesRemoved} duplicate texture(s)");

        int geosets = 0, vBefore = 0, vAfter = 0, fBefore = 0, fAfter = 0;
        foreach (int chunkIndex in doc.GeosetIndices())
        {
            var payload = doc.Chunks[chunkIndex].Payload;
            var rebuilt = OptimizeGeos(payload, version, ratio, ref geosets, ref vBefore, ref vAfter,
                ref fBefore, ref fAfter, diagnostics);
            if (rebuilt is not null) doc.Replace(chunkIndex, rebuilt);
        }

        int effectsRemoved = 0;
        if (stripEffects)
        {
            effectsRemoved = doc.StripEffects();
            if (effectsRemoved > 0) diagnostics.Add($"stripped {effectsRemoved} effect chunk(s)");
        }

        output = doc.Write();
        return new ModelOptimizeResult(name, geosets, vBefore, vAfter, fBefore, fAfter, effectsRemoved,
            bytes.LongLength, output.LongLength, diagnostics);
    }

    private static byte[]? OptimizeGeos(byte[] payload, uint version, float ratio,
        ref int geosets, ref int vBefore, ref int vAfter, ref int fBefore, ref int fAfter,
        List<string> diagnostics)
    {
        // The GEOS payload is a sequence of geosets, each preceded by its inclusiveSize.
        var rebuilt = new List<byte[]>();
        int p = 0;
        while (p + 4 <= payload.Length)
        {
            int inclusiveSize = checked((int)BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(p, 4)));
            if (inclusiveSize < 4 || p + inclusiveSize > payload.Length) break;
            var geosetBytes = payload[(p + 4)..(p + inclusiveSize)];
            var newGeoset = OptimizeGeoset(geosetBytes, version, ratio,
                ref geosets, ref vBefore, ref vAfter, ref fBefore, ref fAfter, diagnostics);
            rebuilt.Add(newGeoset);
            p += inclusiveSize;
        }
        if (rebuilt.Count == 0) return null;

        int total = rebuilt.Sum(g => 4 + g.Length);
        var outBytes = new byte[total];
        int q = 0;
        foreach (var g in rebuilt)
        {
            BinaryPrimitives.WriteUInt32LittleEndian(outBytes.AsSpan(q, 4), (uint)(g.Length + 4));
            g.CopyTo(outBytes, q + 4);
            q += 4 + g.Length;
        }
        return outBytes;
    }

    private static byte[] OptimizeGeoset(byte[] geosetBytes, uint version, float ratio,
        ref int geosets, ref int vBefore, ref int vAfter, ref int fBefore, ref int fAfter,
        List<string> diagnostics)
    {
        var geoset = MdxGeoset.Parse(geosetBytes, version);
        var vrtx = geoset.Segments.FirstOrDefault(s => s.Tag == "VRTX");
        var pvtx = geoset.Segments.FirstOrDefault(s => s.Tag == "PVTX");
        if (vrtx is null || pvtx is null || vrtx.Payload.Length < 4 || pvtx.Payload.Length < 4)
            return geoset.Write();

        var vertices = ReadFloats(vrtx.Payload, 4, 3);
        var indices = ReadU16(pvtx.Payload, 4);
        geosets++;
        vBefore += vertices.Length / 3;
        fBefore += indices.Length / 3;

        // Compact first, dropping the zero-area triangles the game draws as nothing.
        var compacted = CompactFaces(vertices, indices);
        if (compacted.Length != indices.Length)
        {
            geoset.Set("PVTX", PackU16(compacted));
            SetPrimitiveCount(geoset, compacted.Length / 3);
            indices = compacted;
        }

        var dec = QemDecimator.Decimate(vertices, indices, ratio, ReadFirstUvSet(geoset));
        if (dec.OutputFaces >= indices.Length / 3)
        {
            vAfter += vertices.Length / 3;
            fAfter += indices.Length / 3;
            return geoset.Write(); // nothing reduced, keep it untouched
        }

        geoset.Set("VRTX", PackFloats(dec.Vertices, 3));
        geoset.Set("PVTX", PackU16(dec.Indices));
        RemapFloat3(geoset, "NRMS", dec.Remap, dec.OutputVertices);
        RemapBytes(geoset, "GNDX", dec.Remap, dec.OutputVertices);
        RemapUvs(geoset, dec.Remap, dec.OutputVertices);

        vAfter += dec.OutputVertices;
        fAfter += dec.OutputFaces;
        return geoset.Write();
    }

    private static void RemapFloat3(MdxGeoset geoset, string tag, int[] remap, int newCount)
    {
        var seg = geoset.Segments.FirstOrDefault(s => s.Tag == tag);
        if (seg is null || seg.Payload.Length < 4) return;
        int count = checked((int)BinaryPrimitives.ReadUInt32LittleEndian(seg.Payload.AsSpan(0, 4)));
        if (count * 12 + 4 > seg.Payload.Length || count != remap.Length) return;
        var outBytes = new byte[4 + newCount * 12];
        BinaryPrimitives.WriteUInt32LittleEndian(outBytes.AsSpan(0, 4), (uint)newCount);
        for (int i = 0; i < remap.Length; i++)
            if (remap[i] >= 0)
                Buffer.BlockCopy(seg.Payload, 4 + i * 12, outBytes, 4 + remap[i] * 12, 12);
        geoset.Set(tag, outBytes);
    }

    private static void RemapBytes(MdxGeoset geoset, string tag, int[] remap, int newCount)
    {
        var seg = geoset.Segments.FirstOrDefault(s => s.Tag == tag);
        if (seg is null || seg.Payload.Length < 4) return;
        int count = checked((int)BinaryPrimitives.ReadUInt32LittleEndian(seg.Payload.AsSpan(0, 4)));
        if (count + 4 > seg.Payload.Length || count != remap.Length) return;
        var outBytes = new byte[4 + newCount];
        BinaryPrimitives.WriteUInt32LittleEndian(outBytes.AsSpan(0, 4), (uint)newCount);
        for (int i = 0; i < remap.Length; i++)
            if (remap[i] >= 0) outBytes[4 + remap[i]] = seg.Payload[4 + i];
        geoset.Set(tag, outBytes);
    }

    /// <summary>The first UV set, for the seam guard in the decimator. Null when the geoset has
    /// no UVAS or the first set is malformed, which leaves the weld on position alone as before.</summary>
    private static float[]? ReadFirstUvSet(MdxGeoset geoset)
    {
        var seg = geoset.Segments.FirstOrDefault(s => s.Tag == "UVAS");
        if (seg is null || seg.Payload.Length < 16) return null;
        var payload = seg.Payload;
        uint sets = BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(0, 4));
        if (sets < 1 || payload.Length < 16) return null;
        if (System.Text.Encoding.ASCII.GetString(payload, 4, 4) != "UVBS") return null;
        uint n = BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(8, 4));
        if (n == 0 || 12L + (long)n * 8 > payload.Length) return null;
        return ReadFloats(payload, 12, 2);
    }

    private static void RemapUvs(MdxGeoset geoset, int[] remap, int newCount)
    {
        var seg = geoset.Segments.FirstOrDefault(s => s.Tag == "UVAS");
        if (seg is null || seg.Payload.Length < 8) return;
        // UVAS payload: count(4) then one UVBS per set, each "UVBS" + count + floats. The tag is
        // part of the set and must be rewritten, not dropped, or the reader stops at the count and
        // the model loses every UV. Every set is reduced, so all sets stay sized to the new vertex
        // count (the reader keeps the first, but a stale set would corrupt a multi-UV model).
        var payload = seg.Payload;
        int setCount = checked((int)BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(0, 4)));
        var outBytes = new List<byte>(4 + setCount * (8 + newCount * 8));
        var head = new byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(head, (uint)setCount);
        outBytes.AddRange(head);

        int p = 4;
        for (int s = 0; s < setCount; s++)
        {
            if (p + 8 > payload.Length
                || System.Text.Encoding.ASCII.GetString(payload, p, 4) != "UVBS") return; // malformed, leave untouched
            int n = checked((int)BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(p + 4, 4)));
            if (p + 8 + n * 8 > payload.Length || n != remap.Length) return; // unexpected size, leave untouched
            var newUv = new byte[8 + newCount * 8];
            System.Text.Encoding.ASCII.GetBytes("UVBS", newUv.AsSpan(0, 4));
            BinaryPrimitives.WriteUInt32LittleEndian(newUv.AsSpan(4, 4), (uint)newCount);
            for (int i = 0; i < remap.Length; i++)
                if (remap[i] >= 0)
                    Buffer.BlockCopy(payload, p + 8 + i * 8, newUv, 8 + remap[i] * 8, 8);
            outBytes.AddRange(newUv);
            p += 8 + n * 8;
        }
        geoset.Set("UVAS", outBytes.ToArray());
    }

    /// <summary>Drops triangles whose three corners are collinear, so they draw nothing.</summary>
    private static int[] CompactFaces(float[] vertices, int[] indices)
    {
        var kept = new List<int>(indices.Length);
        for (int i = 0; i + 2 < indices.Length; i += 3)
        {
            int a = indices[i] * 3, b = indices[i + 1] * 3, c = indices[i + 2] * 3;
            if (a + 2 >= vertices.Length || b + 2 >= vertices.Length || c + 2 >= vertices.Length) continue;
            var pa = new Vector3(vertices[a], vertices[a + 1], vertices[a + 2]);
            var pb = new Vector3(vertices[b], vertices[b + 1], vertices[b + 2]);
            var pc = new Vector3(vertices[c], vertices[c + 1], vertices[c + 2]);
            if (Vector3.Cross(pb - pa, pc - pa).LengthSquared() > 1e-12f)
            { kept.Add(indices[i]); kept.Add(indices[i + 1]); kept.Add(indices[i + 2]); }
        }
        return kept.ToArray();
    }

    /// <summary>Updates the PCNT primitive count to the new triangle count.</summary>
    private static void SetPrimitiveCount(MdxGeoset geoset, int triangleCount)
    {
        var seg = geoset.Segments.FirstOrDefault(s => s.Tag == "PCNT");
        if (seg is null || seg.Payload.Length < 8) return;
        var payload = (byte[])seg.Payload.Clone();
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(4, 4), (uint)triangleCount);
        geoset.Set("PCNT", payload);
    }

    /// <summary>Removes duplicate texture paths from TEXS and remaps the material texture ids.</summary>
    private static int PruneTextures(MdxDocument doc)
    {
        int texsIndex = IndexOf(doc, "TEXS");
        if (texsIndex < 0) return 0;
        var payload = doc.Chunks[texsIndex].Payload;
        int count = payload.Length / 268;
        if (count < 2) return 0;

        var seen = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var remap = new int[count];
        var unique = new List<int>();
        for (int i = 0; i < count; i++)
        {
            var path = System.Text.Encoding.ASCII.GetString(payload, i * 268 + 4, 260).TrimEnd('\0');
            if (seen.TryGetValue(path, out int nid)) remap[i] = nid;
            else { nid = unique.Count; seen[path] = nid; remap[i] = nid; unique.Add(i); }
        }
        if (unique.Count == count) return 0;

        var newPayload = new byte[unique.Count * 268];
        for (int i = 0; i < unique.Count; i++)
            Buffer.BlockCopy(payload, unique[i] * 268, newPayload, i * 268, 268);
        doc.Replace(texsIndex, newPayload);
        RemapMaterialTextures(doc, remap);
        return count - unique.Count;
    }

    private static void RemapMaterialTextures(MdxDocument doc, int[] remap)
    {
        int mtlsIndex = IndexOf(doc, "MTLS");
        if (mtlsIndex < 0) return;
        var payload = (byte[])doc.Chunks[mtlsIndex].Payload.Clone();
        int p = 0;
        while (p + 12 <= payload.Length)
        {
            int matSize = BinaryPrimitives.ReadInt32LittleEndian(payload.AsSpan(p, 4));
            int matEnd = p + matSize;
            if (matSize < 12 || matEnd > payload.Length) break;
            int q = p + 12;
            if (ReadTag(payload, q) != "LAYS" && q + 84 <= matEnd && ReadTag(payload, q + 80) == "LAYS") q += 80;
            if (ReadTag(payload, q) == "LAYS")
            {
                int layerCount = BinaryPrimitives.ReadInt32LittleEndian(payload.AsSpan(q + 4, 4));
                int lp = q + 8;
                for (int l = 0; l < layerCount && lp + 28 <= matEnd; l++)
                {
                    int layerSize = BinaryPrimitives.ReadInt32LittleEndian(payload.AsSpan(lp, 4));
                    if (layerSize < 28) break;
                    int texId = BinaryPrimitives.ReadInt32LittleEndian(payload.AsSpan(lp + 12, 4));
                    if (texId >= 0 && texId < remap.Length && remap[texId] != texId)
                        BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(lp + 12, 4), remap[texId]);
                    lp += layerSize;
                }
            }
            p = matEnd;
        }
        doc.Replace(mtlsIndex, payload);
    }

    private static int IndexOf(MdxDocument doc, string tag)
    {
        for (int i = 0; i < doc.Chunks.Count; i++) if (doc.Chunks[i].Tag == tag) return i;
        return -1;
    }

    private static string ReadTag(byte[] b, int pos) =>
        pos + 4 <= b.Length ? System.Text.Encoding.ASCII.GetString(b, pos, 4) : "";

    private static uint ReadVersion(MdxDocument doc)
    {
        var vers = doc.Chunks.FirstOrDefault(c => c.Tag == "VERS");
        return vers is { Payload.Length: >= 4 }
            ? BinaryPrimitives.ReadUInt32LittleEndian(vers.Payload.AsSpan(0, 4)) : 800u;
    }

    private static float[] ReadFloats(byte[] payload, int offset, int perVertex)
    {
        int count = checked((int)BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(offset - 4, 4)));
        var result = new float[count * perVertex];
        for (int i = 0; i < result.Length; i++)
            result[i] = BinaryPrimitives.ReadSingleLittleEndian(payload.AsSpan(offset + i * 4, 4));
        return result;
    }

    private static int[] ReadU16(byte[] payload, int offset)
    {
        int count = checked((int)BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(offset - 4, 4)));
        var result = new int[count];
        for (int i = 0; i < count; i++)
            result[i] = BinaryPrimitives.ReadUInt16LittleEndian(payload.AsSpan(offset + i * 2, 2));
        return result;
    }

    private static byte[] PackFloats(float[] values, int perVertex)
    {
        var outBytes = new byte[4 + values.Length * 4];
        BinaryPrimitives.WriteUInt32LittleEndian(outBytes.AsSpan(0, 4), (uint)(values.Length / perVertex));
        for (int i = 0; i < values.Length; i++)
            BinaryPrimitives.WriteSingleLittleEndian(outBytes.AsSpan(4 + i * 4, 4), values[i]);
        return outBytes;
    }

    private static byte[] PackU16(int[] values)
    {
        var outBytes = new byte[4 + values.Length * 2];
        BinaryPrimitives.WriteUInt32LittleEndian(outBytes.AsSpan(0, 4), (uint)values.Length);
        for (int i = 0; i < values.Length; i++)
            BinaryPrimitives.WriteUInt16LittleEndian(outBytes.AsSpan(4 + i * 2, 2), (ushort)values[i]);
        return outBytes;
    }
}
