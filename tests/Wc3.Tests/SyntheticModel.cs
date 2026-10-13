// tests/Wc3.Tests/SyntheticModel.cs
using System.Text;
using Wc3.Modeling;
using Wc3.Render;

namespace Wc3.Tests;

/// <summary>
/// Builders for small synthetic MDX models and BLP textures, so the model and asset command
/// tests exercise a real parse without a WC3 install or a corpus. The MDX layout mirrors the
/// one the binary parser reads (VERS, SEQS, BONE, TEXS, MTLS, GEOS), kept small and explicit.
/// </summary>
internal static class SyntheticModel
{
    /// <summary>A minimal v800 MDLX with one textured triangle and no animation.</summary>
    public static byte[] TinyMdx() => BuildMdx();

    /// <summary>
    /// A v800 MDLX with one triangle geoset and optional texture, material, sequences and a bone.
    /// A zero-duration sequence only reaches <see cref="Model3D.Skeleton"/> when a bone is present,
    /// so <paramref name="includeBone"/> is what makes the sequence visible to a scan.
    /// </summary>
    public static byte[] BuildMdx(
        bool includeTexture = true,
        bool includeMaterial = true,
        int materialId = 0,
        float[]? vertices = null,
        string texturePath = @"Textures\Test.blp",
        (string Name, int Start, int End)[]? sequences = null,
        bool includeBone = false,
        (string Tag, byte[] Payload)[]? extraChunks = null)
    {
        var verts = vertices ?? new float[] { 0, 0, 0, 1, 0, 0, 0, 1, 0 };
        int vertexCount = verts.Length / 3;

        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        w.Write("MDLX"u8);

        w.Write("VERS"u8);
        w.Write(4u);
        w.Write(800u);

        if (sequences is not null)
        {
            w.Write("SEQS"u8);
            w.Write((uint)(132 * sequences.Length));
            foreach (var (name, start, end) in sequences)
            {
                WriteFixed(w, name, 80);
                w.Write((uint)start);
                w.Write((uint)end);
                w.Write(0f);  // move speed
                w.Write(0u);  // flags
                w.Write(0f);  // rarity
                w.Write(0u);  // sync point
                for (int i = 0; i < 7; i++) w.Write(0f); // extent
            }
        }

        if (includeBone)
        {
            // One BONE node, an inclusiveSize of 96 plus the 8 trailing geoset id fields.
            w.Write("BONE"u8);
            w.Write(104u);
            w.Write(96u);                 // inclusiveSize
            WriteFixed(w, "root", 80);
            w.Write(0u);                  // objectId
            w.Write(unchecked((uint)-1)); // parentId = -1
            w.Write(0u);                  // flags
            w.Write(0u);                  // trailing geosetId
            w.Write(0u);                  // trailing geosetAnimId
        }

        if (includeTexture)
        {
            w.Write("TEXS"u8);
            w.Write(268u);
            w.Write(0u); // replaceableId
            var path = Encoding.ASCII.GetBytes(texturePath);
            w.Write(path);
            w.Write(new byte[260 - path.Length]);
            w.Write(0u); // flags
        }

        if (includeMaterial)
        {
            const uint layerSize = 28u;
            uint matSize = 4 + 4 + 4 + 8 + layerSize; // inclusiveSize, priorityPlane, flags, LAYS+count, layer
            w.Write("MTLS"u8);
            w.Write(matSize);
            w.Write(matSize);
            w.Write(0u); // priorityPlane
            w.Write(0u); // flags
            w.Write("LAYS"u8);
            w.Write(1u);
            w.Write(layerSize);
            w.Write(0u);           // filterMode
            w.Write(0u);           // shadingFlags
            w.Write(0u);           // textureId
            w.Write(0xFFFFFFFFu);  // textureAnimationId
            w.Write(0u);           // coordId
            w.Write(1.0f);         // alpha
        }

        using var gs = new MemoryStream();
        using var gw = new BinaryWriter(gs);
        gw.Write("VRTX"u8);
        gw.Write((uint)vertexCount);
        foreach (var f in verts) gw.Write(f);
        gw.Write("NRMS"u8);
        gw.Write((uint)vertexCount);
        for (int i = 0; i < vertexCount; i++) { gw.Write(0f); gw.Write(0f); gw.Write(1f); }
        gw.Write("PTYP"u8);
        gw.Write(1u);
        gw.Write(4u);
        gw.Write("PCNT"u8);
        gw.Write(1u);
        gw.Write(3u);
        gw.Write("PVTX"u8);
        gw.Write(3u);
        gw.Write((ushort)0);
        gw.Write((ushort)1);
        gw.Write((ushort)2);
        gw.Write("GNDX"u8);
        gw.Write((uint)vertexCount);
        gw.Write(new byte[vertexCount]);
        gw.Write("MTGC"u8);
        gw.Write(1u);
        gw.Write(1u);
        gw.Write("MATS"u8);
        gw.Write(1u);
        gw.Write(0u);
        gw.Write((uint)materialId); // materialId
        gw.Write(0u);               // selectionGroup
        gw.Write(0u);               // selectionFlags
        foreach (var f in new float[] { 2, -1, -1, 0, 1, 1, 0 }) gw.Write(f);
        gw.Write(0u);               // per-sequence extent count
        gw.Write("UVAS"u8);
        gw.Write(1u);
        gw.Write("UVBS"u8);
        gw.Write((uint)vertexCount);
        for (int i = 0; i < vertexCount; i++) { gw.Write(i * 0.1f); gw.Write(0f); }
        gw.Flush();

        var geoset = gs.ToArray();
        w.Write("GEOS"u8);
        w.Write((uint)(geoset.Length + 4));
        w.Write((uint)(geoset.Length + 4));
        w.Write(geoset);

        // Extra chunks written verbatim, so a test can add an effect tag (PREM, PRE2, RIBB, CORN)
        // that the parser skips by size and the audit counts by tag.
        if (extraChunks is not null)
            foreach (var (tag, payload) in extraChunks)
            {
                w.Write(Encoding.ASCII.GetBytes(tag));
                w.Write((uint)payload.Length);
                w.Write(payload);
            }
        w.Flush();
        return ms.ToArray();
    }

    /// <summary>
    /// A v800 MDX with one geoset holding an n by n grid, 2*(n-1)^2 triangles, enough geometry for a
    /// decimator to reduce. The vertices lie in the z=0 plane and the texture, material and UV blocks
    /// are minimal, so the grid is the whole payload. A degenerate collinear triangle is appended when
    /// <paramref name="degenerate"/> is set, for a test that a zero-area face is compacted away.
    /// </summary>
    public static byte[] GridMdx(int n, bool degenerate = false)
    {
        var verts = new List<float>();
        for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++) { verts.Add(x); verts.Add(y); verts.Add(0); }
        int V(int x, int y) => y * n + x;
        var idx = new List<int>();
        for (int y = 0; y < n - 1; y++)
            for (int x = 0; x < n - 1; x++)
            {
                idx.Add(V(x, y)); idx.Add(V(x + 1, y)); idx.Add(V(x, y + 1));
                idx.Add(V(x + 1, y)); idx.Add(V(x + 1, y + 1)); idx.Add(V(x, y + 1));
            }
        if (degenerate) { idx.Add(V(0, 0)); idx.Add(V(1, 0)); idx.Add(V(2, 0)); } // collinear, zero area
        int count = verts.Count / 3;

        using var gs = new MemoryStream();
        using var gw = new BinaryWriter(gs);
        gw.Write("VRTX"u8); gw.Write((uint)count); foreach (var f in verts) gw.Write(f);
        gw.Write("NRMS"u8); gw.Write((uint)count);
        for (int i = 0; i < count; i++) { gw.Write(0f); gw.Write(0f); gw.Write(1f); }
        gw.Write("PTYP"u8); gw.Write(1u); gw.Write(4u);
        gw.Write("PCNT"u8); gw.Write(1u); gw.Write((uint)idx.Count);
        gw.Write("PVTX"u8); gw.Write((uint)idx.Count); foreach (var i in idx) gw.Write((ushort)i);
        gw.Write("GNDX"u8); gw.Write((uint)count); gw.Write(new byte[count]);
        gw.Write("MTGC"u8); gw.Write(1u); gw.Write(1u);
        gw.Write("MATS"u8); gw.Write(1u); gw.Write(0u);
        gw.Write(0u); gw.Write(0u); gw.Write(0u); // materialId, selectionGroup, selectionFlags
        foreach (var f in new float[] { 2, -1, -1, 0, n, n, 0 }) gw.Write(f); // extent
        gw.Write(0u); // per-sequence extent count
        gw.Write("UVAS"u8); gw.Write(1u);
        gw.Write("UVBS"u8); gw.Write((uint)count);
        for (int i = 0; i < count; i++) { gw.Write(0f); gw.Write(0f); }
        gw.Flush();
        var geoset = gs.ToArray();

        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        w.Write("MDLX"u8);
        w.Write("VERS"u8); w.Write(4u); w.Write(800u);
        w.Write("GEOS"u8); w.Write((uint)(geoset.Length + 4)); w.Write((uint)(geoset.Length + 4)); w.Write(geoset);
        w.Flush();
        return ms.ToArray();
    }

    /// <summary>
    /// One PREM (particle emitter 1) record, its node header only, which is all the emitter reader
    /// reads. The record is a uint32 record size counted from the record start, then the node, which
    /// is size prefixed itself, so the record is recordSize, nodeSize, char[80] name, int objectId,
    /// int parentId, uint32 flags. The objectId sits at byte 88 and the node header ends at byte 4
    /// plus nodeSize. Particle emitter 1 carries no texture id, only a spawn model path.
    /// </summary>
    public static byte[] ParticleEmitter1Record(int objectId, int parentId = -1, string name = "emitter1")
    {
        const int nodeSize = 96; // size field plus the 92 byte node header, no transforms
        const int recordSize = 4 + nodeSize;
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        w.Write((uint)recordSize);
        w.Write((uint)nodeSize);
        WriteFixed(w, name, 80);
        w.Write((uint)objectId);
        w.Write(unchecked((uint)parentId));
        w.Write(0u);
        return ms.ToArray();
    }

    /// <summary>
    /// One PRE2 (particle emitter 2) record. The node header is as for a PREM, and the fixed emitter
    /// fields follow it, with the texture id at byte 155 past the end of the node header. The fields
    /// between are left zero, since the emitter reader does not read them.
    /// </summary>
    public static byte[] ParticleEmitter2Record(int objectId, int textureId, int parentId = -1, string name = "emitter2")
    {
        const int nodeSize = 96;
        const int nodeEnd = 4 + nodeSize;   // the record offset where the fixed fields begin
        const int textureOffset = 155;      // the texture id, past the end of the node header
        const int recordSize = nodeEnd + textureOffset + 4;
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        w.Write((uint)recordSize);
        w.Write((uint)nodeSize);
        WriteFixed(w, name, 80);
        w.Write((uint)objectId);
        w.Write(unchecked((uint)parentId));
        w.Write(0u);
        w.Write(new byte[textureOffset]);   // the PRE2 fixed fields up to the texture id
        w.Write((uint)textureId);
        return ms.ToArray();
    }

    /// <summary>A PIVT chunk payload of one zero pivot per node. Only the count is read.</summary>
    public static byte[] Pivots(int count) => new byte[count * 12];

    /// <summary>A small BLP1 texture of one flat colour, encoded by the shared converter.</summary>
    public static byte[] Blp(int width = 32, int height = 32)
    {
        var rgba = new byte[width * height * 4];
        for (int i = 0; i < rgba.Length; i += 4)
        {
            rgba[i] = 200; rgba[i + 1] = 100; rgba[i + 2] = 50; rgba[i + 3] = 255;
        }
        var png = TexturePng.Encode(new TextureImage(width, height, rgba));
        return TextureConvert.Convert(png, ".png", ".blp");
    }

    private static void WriteFixed(BinaryWriter w, string value, int length)
    {
        var bytes = Encoding.ASCII.GetBytes(value);
        w.Write(bytes, 0, Math.Min(bytes.Length, length));
        w.Write(new byte[Math.Max(0, length - bytes.Length)]);
    }
}
