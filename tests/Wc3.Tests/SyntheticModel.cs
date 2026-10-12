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
        bool includeBone = false)
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
        w.Flush();
        return ms.ToArray();
    }

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
