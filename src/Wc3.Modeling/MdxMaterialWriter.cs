// src/Wc3.Modeling/MdxMaterialWriter.cs
using System.Buffers.Binary;
using System.Text;

namespace Wc3.Modeling;

/// <summary>
/// One TEXS entry, the texture table a material layer names by index. The path is the map path the
/// game reads, ASCII, and is truncated to 260 bytes. Replaceable id 0 means a literal file path, and
/// a non-zero id names a built-in team colour or ground texture instead, in which case the path is
/// left empty.
/// </summary>
public readonly record struct MdxTextureEntry(string Path, uint ReplaceableId = 0, uint Flags = 0);

/// <summary>
/// One MTLS layer, the surface a material draws. <see cref="FilterMode"/> is the blend mode the game
/// applies, so <see cref="Wc3.Modeling.FilterMode.None"/> is opaque and
/// <see cref="Wc3.Modeling.FilterMode.Additive"/> is the glow a generated effect usually wants.
/// <see cref="TextureId"/> indexes the TEXS table, and -1 names no texture at all.
/// </summary>
public readonly record struct MdxLayer(
    FilterMode FilterMode,
    uint ShadingFlags,
    int TextureId,
    int TextureAnimationId = -1,
    uint CoordId = 0,
    float Alpha = 1.0f)
{
    /// <summary>The layer a generated effect wants, one texture drawn opaque at full alpha.</summary>
    public static MdxLayer Opaque(int textureId, FilterMode filterMode = FilterMode.None) =>
        new(filterMode, 0, textureId);
}

/// <summary>
/// One MTLS material, a single layer over a priority plane. Priority 0 is the default plane, and
/// flags 0 is the honest default for a material that is neither two sided nor unshaded.
/// </summary>
public readonly record struct MdxMaterialEntry(
    uint PriorityPlane,
    uint Flags,
    MdxLayer Layer)
{
    /// <summary>A single-layer material on the default priority plane.</summary>
    public static MdxMaterialEntry Single(MdxLayer layer) => new(0, 0, layer);
}

/// <summary>
/// Encodes the TEXS and MTLS chunks of a binary MDX, and the material and texture tables a
/// generated effect needs, one texture and one layer per material. The layout is the one
/// <see cref="MdxParser"/> reads and <c>MdxMeshBuilder</c> already writes, and it is stated here as
/// offsets so a caller never has to count by hand.
///
/// TEXS carries no count field. The entry count is the payload length divided by the 268 byte entry
/// size, which is how the engine, <see cref="MdxParser"/> and the round trip all read it, so a count
/// written here would shift every entry by four bytes and be misread. Within one entry the offsets
/// are 0 a u32 replaceable id, 4 a null padded 260 byte path, and 264 a u32 flags field.
///
/// MTLS carries one record per material, each with an inclusive size that covers its own four bytes.
/// A 48 byte record is 0 inclusive size, 4 priority plane, 8 flags, 12 the LAYS tag, 16 the layer
/// count, then the layer at 20. A 28 byte layer is 0 its own inclusive size, 4 the filter mode (the
/// blend mode), 8 shading flags, 12 the static texture id, 16 the animated texture id, 20 the coord
/// id and 24 a float alpha.
/// </summary>
public static class MdxMaterialWriter
{
    /// <summary>The version this writer stamps, the one <see cref="MdxParser"/> expects for a modern model.</summary>
    public const uint Version = 800;

    /// <summary>One TEXS entry, its replaceable id, path and flags.</summary>
    public const int TextureEntrySize = 268;

    /// <summary>Where the null padded path starts inside one TEXS entry.</summary>
    public const int TexturePathOffset = 4;

    /// <summary>The path field length in a TEXS entry, in bytes.</summary>
    public const int TexturePathLength = 260;

    /// <summary>Where the flags field starts inside one TEXS entry.</summary>
    public const int TextureFlagsOffset = 264;

    /// <summary>One MTLS material with exactly one layer, its inclusive size included.</summary>
    public const int MaterialSize = 48;

    /// <summary>Where the LAYS tag starts inside one material.</summary>
    public const int MaterialLayersOffset = 12;

    /// <summary>Where the u32 layer count starts inside one material.</summary>
    public const int MaterialLayerCountOffset = 16;

    /// <summary>Where the single layer starts inside one material.</summary>
    public const int MaterialFirstLayerOffset = 20;

    /// <summary>One MTLS layer, its inclusive size included.</summary>
    public const int LayerSize = 28;

    /// <summary>
    /// Encodes a TEXS payload, one 268 byte entry per texture in order. A path longer than 260 bytes
    /// is cut to the first 260, never allowed to spill into the next entry, and a path is ASCII so
    /// cutting it can never split a character. An empty texture list encodes to an empty payload,
    /// which is the chunk a model with no textures carries.
    /// </summary>
    public static byte[] EncodeTexs(IReadOnlyList<MdxTextureEntry> textures)
    {
        ArgumentNullException.ThrowIfNull(textures);

        var payload = new byte[textures.Count * TextureEntrySize];
        for (int i = 0; i < textures.Count; i++)
        {
            var texture = textures[i];
            int entry = i * TextureEntrySize;

            BinaryPrimitives.WriteUInt32LittleEndian(
                payload.AsSpan(entry, 4), texture.ReplaceableId);

            var path = Encoding.ASCII.GetBytes(texture.Path);
            if (path.Length > TexturePathLength) path = path[..TexturePathLength];
            path.CopyTo(payload, entry + TexturePathOffset);

            BinaryPrimitives.WriteUInt32LittleEndian(
                payload.AsSpan(entry + TextureFlagsOffset, 4), texture.Flags);
        }
        return payload;
    }

    /// <summary>
    /// Encodes an MTLS payload, one 48 byte material per entry, each with one 28 byte layer. The
    /// layer count is always 1 because that is the shape this writer produces and the shape a
    /// generated effect needs. An empty material list encodes to an empty payload.
    /// </summary>
    public static byte[] EncodeMtls(IReadOnlyList<MdxMaterialEntry> materials)
    {
        ArgumentNullException.ThrowIfNull(materials);

        var payload = new byte[materials.Count * MaterialSize];
        for (int i = 0; i < materials.Count; i++)
        {
            var material = materials[i];
            int start = i * MaterialSize;

            BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(start, 4), MaterialSize);
            BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(start + 4, 4), material.PriorityPlane);
            BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(start + 8, 4), material.Flags);
            "LAYS"u8.CopyTo(payload.AsSpan(start + MaterialLayersOffset, 4));
            BinaryPrimitives.WriteUInt32LittleEndian(
                payload.AsSpan(start + MaterialLayerCountOffset, 4), 1);

            var layer = material.Layer;
            int at = start + MaterialFirstLayerOffset;
            BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(at, 4), LayerSize);
            BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(at + 4, 4), (uint)layer.FilterMode);
            BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(at + 8, 4), layer.ShadingFlags);
            BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(at + 12, 4), unchecked((uint)layer.TextureId));
            BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(at + 16, 4), unchecked((uint)layer.TextureAnimationId));
            BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(at + 20, 4), layer.CoordId);
            BinaryPrimitives.WriteSingleLittleEndian(payload.AsSpan(at + 24, 4), layer.Alpha);
        }
        return payload;
    }

    /// <summary>
    /// Builds a complete document for the given tables, VERS 800 then TEXS then MTLS, in the order
    /// the engine and <c>MdxMeshBuilder</c> emit them. Writing it gives a header a parser or a game
    /// accepts, with no geometry until a geoset is added.
    /// </summary>
    public static MdxDocument CreateDocument(
        IReadOnlyList<MdxTextureEntry> textures,
        IReadOnlyList<MdxMaterialEntry> materials,
        uint version = Version)
    {
        ArgumentNullException.ThrowIfNull(textures);
        ArgumentNullException.ThrowIfNull(materials);

        var versionBytes = new byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(versionBytes, version);

        return MdxDocument.Create(new[]
        {
            new MdxDocument.Chunk("VERS", versionBytes),
            new MdxDocument.Chunk("TEXS", EncodeTexs(textures)),
            new MdxDocument.Chunk("MTLS", EncodeMtls(materials)),
        });
    }

    /// <summary>
    /// The common case for a generated effect, one texture and one single-layer material that names
    /// it. The material draws layer 0 of texture 0 with the given blend mode.
    /// </summary>
    public static MdxDocument CreateDocument(
        string texturePath,
        FilterMode filterMode = FilterMode.None,
        uint replaceableId = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(texturePath);
        return CreateDocument(
            new[] { new MdxTextureEntry(texturePath, replaceableId) },
            new[] { MdxMaterialEntry.Single(MdxLayer.Opaque(0, filterMode)) });
    }

    /// <summary>
    /// Reads a TEXS payload back into its entries, stopping at the last whole 268 byte row so a
    /// truncated tail is ignored rather than throwing. The inverse of <see cref="EncodeTexs"/>.
    /// </summary>
    public static IReadOnlyList<MdxTextureEntry> ReadTextures(byte[] texsPayload)
    {
        ArgumentNullException.ThrowIfNull(texsPayload);

        var textures = new List<MdxTextureEntry>(texsPayload.Length / TextureEntrySize);
        for (int p = 0; p + TextureEntrySize <= texsPayload.Length; p += TextureEntrySize)
        {
            uint replaceableId = BinaryPrimitives.ReadUInt32LittleEndian(texsPayload.AsSpan(p, 4));
            string path = ReadFixedString(texsPayload, p + TexturePathOffset, TexturePathLength);
            uint flags = BinaryPrimitives.ReadUInt32LittleEndian(
                texsPayload.AsSpan(p + TextureFlagsOffset, 4));
            textures.Add(new MdxTextureEntry(path, replaceableId, flags));
        }
        return textures;
    }

    /// <summary>
    /// Reads an MTLS payload back into its materials, one layer each. A record whose inclusive size
    /// is short or runs past the payload stops the walk, matching <see cref="MdxParser"/>'s
    /// tolerance. The inverse of <see cref="EncodeMtls"/>, for the byte layout this writer emits.
    /// </summary>
    public static IReadOnlyList<MdxMaterialEntry> ReadMaterials(byte[] mtlsPayload)
    {
        ArgumentNullException.ThrowIfNull(mtlsPayload);

        var materials = new List<MdxMaterialEntry>(mtlsPayload.Length / MaterialSize);
        int p = 0;
        while (p + MaterialSize <= mtlsPayload.Length)
        {
            uint inclusiveSize = BinaryPrimitives.ReadUInt32LittleEndian(mtlsPayload.AsSpan(p, 4));
            if (inclusiveSize < MaterialSize || p + (long)inclusiveSize > mtlsPayload.Length) break;

            uint priorityPlane = BinaryPrimitives.ReadUInt32LittleEndian(mtlsPayload.AsSpan(p + 4, 4));
            uint flags = BinaryPrimitives.ReadUInt32LittleEndian(mtlsPayload.AsSpan(p + 8, 4));

            // A material whose layer does not fit still reads back, naming no texture and drawing
            // nothing, rather than silently claiming texture 0 at zero alpha.
            var layer = new MdxLayer(FilterMode.None, 0, -1);
            int at = p + MaterialFirstLayerOffset;
            if (at + LayerSize <= p + (int)inclusiveSize)
            {
                layer = new MdxLayer(
                    (FilterMode)BinaryPrimitives.ReadUInt32LittleEndian(mtlsPayload.AsSpan(at + 4, 4)),
                    BinaryPrimitives.ReadUInt32LittleEndian(mtlsPayload.AsSpan(at + 8, 4)),
                    unchecked((int)BinaryPrimitives.ReadUInt32LittleEndian(mtlsPayload.AsSpan(at + 12, 4))),
                    unchecked((int)BinaryPrimitives.ReadUInt32LittleEndian(mtlsPayload.AsSpan(at + 16, 4))),
                    BinaryPrimitives.ReadUInt32LittleEndian(mtlsPayload.AsSpan(at + 20, 4)),
                    BinaryPrimitives.ReadSingleLittleEndian(mtlsPayload.AsSpan(at + 24, 4)));
            }

            materials.Add(new MdxMaterialEntry(priorityPlane, flags, layer));
            p += (int)inclusiveSize;
        }
        return materials;
    }

    /// <summary>The TEXS entries of a parsed document, empty when it carries no TEXS chunk.</summary>
    public static IReadOnlyList<MdxTextureEntry> ReadTextures(MdxDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        foreach (var chunk in document.Chunks)
            if (chunk.Tag == "TEXS") return ReadTextures(chunk.Payload);
        return Array.Empty<MdxTextureEntry>();
    }

    /// <summary>The materials of a parsed document, empty when it carries no MTLS chunk.</summary>
    public static IReadOnlyList<MdxMaterialEntry> ReadMaterials(MdxDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        foreach (var chunk in document.Chunks)
            if (chunk.Tag == "MTLS") return ReadMaterials(chunk.Payload);
        return Array.Empty<MdxMaterialEntry>();
    }

    /// <summary>
    /// Decodes a fixed field up to its first null byte, the way the path field is stored. A field
    /// with no null byte, which is what a truncated path leaves, decodes to its whole length.
    /// </summary>
    private static string ReadFixedString(byte[] bytes, int offset, int length)
    {
        int end = Array.IndexOf(bytes, (byte)0, offset, length);
        return Encoding.ASCII.GetString(bytes, offset, end < 0 ? length : end - offset);
    }
}
