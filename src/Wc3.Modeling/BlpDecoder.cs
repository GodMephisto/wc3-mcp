// src/Wc3.Modeling/BlpDecoder.cs
using War3Net.Drawing.Blp;

namespace Wc3.Modeling;

/// <summary>A decoded texture: RGBA8, row-major, top-left origin.</summary>
public sealed record TextureImage(int Width, int Height, byte[] Rgba);

/// <summary>
/// Decodes Warcraft III .blp textures (BLP0/BLP1 JPEG-content and palettized,
/// plus BLP2) by wrapping the War3Net.Drawing.Blp package.
/// </summary>
public static class BlpDecoder
{
    public static TextureImage Decode(byte[] blp)
    {
        ArgumentNullException.ThrowIfNull(blp);
        using var stream = new MemoryStream(blp, writable: false);
        using var file = new BlpFile(stream);
        var rgba = file.GetPixels(0, out int width, out int height, bgra: false);
        // GetPixels' bgra flag only applies to Direct (palettized/DXT) content; for
        // JPEG-content BLP0/BLP1 the raw JPEG channel order comes back untouched —
        // and WC3 stores those channels as BGRA. Swap to true RGBA here (verified
        // against real map assets: skin tones decode blue without this).
        if (IsJpegContent(blp))
            for (int i = 0; i < rgba.Length; i += 4)
                (rgba[i], rgba[i + 2]) = (rgba[i + 2], rgba[i]);
        return new TextureImage(width, height, rgba);
    }

    /// <summary>BLP0/BLP1 header: 4-byte magic, then uint32 content type (0 = JPEG).</summary>
    private static bool IsJpegContent(byte[] blp) =>
        blp.Length >= 8
        && blp[0] == (byte)'B' && blp[1] == (byte)'L' && blp[2] == (byte)'P'
        && (blp[3] == (byte)'0' || blp[3] == (byte)'1')
        && BitConverter.ToUInt32(blp, 4) == 0;
}
