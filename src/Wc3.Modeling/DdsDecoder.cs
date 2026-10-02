// src/Wc3.Modeling/DdsDecoder.cs
using System.Buffers.Binary;
using System.Text;

namespace Wc3.Modeling;

/// <summary>
/// Decodes .dds textures to RGBA8 — Reforged repacked the classic .blp textures as DDS
/// under the same CASC paths, so base-game model previews need it. Supports the formats
/// those files actually use: BC1/DXT1, BC2/DXT3, BC3/DXT5 (fourCC or DX10 header) and
/// uncompressed 32-bit RGBA/BGRA. Top mip only.
/// </summary>
public static class DdsDecoder
{
    private const uint Magic = 0x20534444;        // "DDS "
    private const uint PfFourCc = 0x4;
    private const uint PfRgb = 0x40;

    public static bool LooksLikeDds(byte[] bytes) =>
        bytes.Length >= 4 && BinaryPrimitives.ReadUInt32LittleEndian(bytes) == Magic;

    public static TextureImage Decode(byte[] dds)
    {
        ArgumentNullException.ThrowIfNull(dds);
        if (dds.Length < 128 || BinaryPrimitives.ReadUInt32LittleEndian(dds) != Magic)
            throw new InvalidDataException("not a DDS file");

        var span = dds.AsSpan();
        int height = BinaryPrimitives.ReadInt32LittleEndian(span[12..]);
        int width = BinaryPrimitives.ReadInt32LittleEndian(span[16..]);
        uint pfFlags = BinaryPrimitives.ReadUInt32LittleEndian(span[80..]);
        string fourCc = Encoding.ASCII.GetString(dds, 84, 4);
        uint bitCount = BinaryPrimitives.ReadUInt32LittleEndian(span[88..]);
        uint rMask = BinaryPrimitives.ReadUInt32LittleEndian(span[92..]);
        int dataOffset = 128;

        if ((pfFlags & PfFourCc) != 0 && fourCc == "DX10")
        {
            // DXGI_FORMAT: BC1_* = 70-72, BC2_* = 73-75, BC3_* = 76-78.
            int dxgi = BinaryPrimitives.ReadInt32LittleEndian(span[128..]);
            dataOffset += 20;
            fourCc = dxgi switch
            {
                >= 70 and <= 72 => "DXT1",
                >= 73 and <= 75 => "DXT3",
                >= 76 and <= 78 => "DXT5",
                28 or 87 => "RAW32", // R8G8B8A8_UNORM / B8G8R8A8_UNORM
                _ => throw new InvalidDataException($"unsupported DDS DXGI format {dxgi}"),
            };
        }
        else if ((pfFlags & PfFourCc) == 0)
        {
            if ((pfFlags & PfRgb) == 0 || bitCount != 32)
                throw new InvalidDataException("unsupported uncompressed DDS pixel format");
            fourCc = "RAW32";
        }

        return fourCc switch
        {
            "DXT1" => DecodeBlocks(dds, dataOffset, width, height, blockBytes: 8, DecodeBc1Block),
            "DXT3" => DecodeBlocks(dds, dataOffset, width, height, blockBytes: 16, DecodeBc2Block),
            "DXT5" => DecodeBlocks(dds, dataOffset, width, height, blockBytes: 16, DecodeBc3Block),
            "RAW32" => DecodeRaw32(dds, dataOffset, width, height, rMask),
            _ => throw new InvalidDataException($"unsupported DDS fourCC '{fourCc}'"),
        };
    }

    private static TextureImage DecodeRaw32(byte[] dds, int offset, int width, int height, uint rMask)
    {
        var rgba = new byte[width * height * 4];
        bool bgra = rMask == 0x00FF0000; // else assume RGBA layout
        for (int i = 0; i < width * height; i++)
        {
            int src = offset + i * 4;
            if (src + 4 > dds.Length) break;
            rgba[i * 4 + 0] = bgra ? dds[src + 2] : dds[src + 0];
            rgba[i * 4 + 1] = dds[src + 1];
            rgba[i * 4 + 2] = bgra ? dds[src + 0] : dds[src + 2];
            rgba[i * 4 + 3] = dds[src + 3];
        }
        return new TextureImage(width, height, rgba);
    }

    /// <summary>Walks the 4x4 block grid, letting the per-format decoder fill each block
    /// (as a 16-entry RGBA span), and scatters the texels into the full image.</summary>
    private static TextureImage DecodeBlocks(
        byte[] dds, int offset, int width, int height, int blockBytes,
        Action<byte[], int, byte[]> decodeBlock)
    {
        var rgba = new byte[width * height * 4];
        var block = new byte[16 * 4];
        int blocksWide = Math.Max(1, (width + 3) / 4);
        int blocksHigh = Math.Max(1, (height + 3) / 4);
        for (int by = 0; by < blocksHigh; by++)
            for (int bx = 0; bx < blocksWide; bx++)
            {
                int src = offset + (by * blocksWide + bx) * blockBytes;
                if (src + blockBytes > dds.Length) return new TextureImage(width, height, rgba);
                decodeBlock(dds, src, block);
                for (int py = 0; py < 4; py++)
                {
                    int y = by * 4 + py;
                    if (y >= height) break;
                    for (int px = 0; px < 4; px++)
                    {
                        int x = bx * 4 + px;
                        if (x >= width) break;
                        int dst = (y * width + x) * 4;
                        int s = (py * 4 + px) * 4;
                        rgba[dst + 0] = block[s + 0];
                        rgba[dst + 1] = block[s + 1];
                        rgba[dst + 2] = block[s + 2];
                        rgba[dst + 3] = block[s + 3];
                    }
                }
            }
        return new TextureImage(width, height, rgba);
    }

    private static void DecodeBc1Block(byte[] dds, int src, byte[] block) =>
        DecodeColorBlock(dds, src, block, opaqueMode: false);

    private static void DecodeBc2Block(byte[] dds, int src, byte[] block)
    {
        DecodeColorBlock(dds, src + 8, block, opaqueMode: true);
        // 4-bit explicit alpha, 16 texels over 8 bytes, low nibble first.
        for (int i = 0; i < 16; i++)
        {
            int nibble = (dds[src + i / 2] >> (i % 2 * 4)) & 0xF;
            block[i * 4 + 3] = (byte)(nibble * 17);
        }
    }

    private static void DecodeBc3Block(byte[] dds, int src, byte[] block)
    {
        DecodeColorBlock(dds, src + 8, block, opaqueMode: true);
        byte a0 = dds[src], a1 = dds[src + 1];
        // Interpolated alpha palette (8 entries), then 16 3-bit indices in 6 bytes.
        Span<byte> alpha = stackalloc byte[8];
        alpha[0] = a0;
        alpha[1] = a1;
        if (a0 > a1)
            for (int i = 1; i < 7; i++) alpha[1 + i] = (byte)(((7 - i) * a0 + i * a1) / 7);
        else
        {
            for (int i = 1; i < 5; i++) alpha[1 + i] = (byte)(((5 - i) * a0 + i * a1) / 5);
            alpha[6] = 0;
            alpha[7] = 255;
        }
        ulong bits = 0;
        for (int i = 0; i < 6; i++) bits |= (ulong)dds[src + 2 + i] << (8 * i);
        for (int i = 0; i < 16; i++)
            block[i * 4 + 3] = alpha[(int)((bits >> (3 * i)) & 0x7)];
    }

    /// <summary>The shared RGB565 palette block (8 bytes). BC2/BC3 always use the 4-color
    /// palette (<paramref name="opaqueMode"/>); standalone BC1 switches on c0 &lt;= c1 to
    /// the 3-color + transparent form.</summary>
    private static void DecodeColorBlock(byte[] dds, int src, byte[] block, bool opaqueMode)
    {
        ushort c0 = BinaryPrimitives.ReadUInt16LittleEndian(dds.AsSpan(src));
        ushort c1 = BinaryPrimitives.ReadUInt16LittleEndian(dds.AsSpan(src + 2));
        Span<byte> pal = stackalloc byte[16]; // 4 palette entries x RGBA
        Expand565(c0, pal);
        Expand565(c1, pal[4..]);
        if (opaqueMode || c0 > c1)
        {
            for (int ch = 0; ch < 3; ch++)
            {
                pal[8 + ch] = (byte)((2 * pal[ch] + pal[4 + ch]) / 3);
                pal[12 + ch] = (byte)((pal[ch] + 2 * pal[4 + ch]) / 3);
            }
            pal[11] = 255;
            pal[15] = 255;
        }
        else
        {
            for (int ch = 0; ch < 3; ch++)
                pal[8 + ch] = (byte)((pal[ch] + pal[4 + ch]) / 2);
            pal[11] = 255;
            pal[15] = 0; // transparent black
        }

        uint indices = BinaryPrimitives.ReadUInt32LittleEndian(dds.AsSpan(src + 4));
        for (int i = 0; i < 16; i++)
        {
            int p = (int)((indices >> (2 * i)) & 0x3) * 4;
            block[i * 4 + 0] = pal[p + 0];
            block[i * 4 + 1] = pal[p + 1];
            block[i * 4 + 2] = pal[p + 2];
            block[i * 4 + 3] = pal[p + 3];
        }
    }

    private static void Expand565(ushort c, Span<byte> rgba)
    {
        int r = (c >> 11) & 0x1F, g = (c >> 5) & 0x3F, b = c & 0x1F;
        rgba[0] = (byte)(r << 3 | r >> 2);
        rgba[1] = (byte)(g << 2 | g >> 4);
        rgba[2] = (byte)(b << 3 | b >> 2);
        rgba[3] = 255;
    }
}
