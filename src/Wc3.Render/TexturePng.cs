// src/Wc3.Render/TexturePng.cs
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Wc3.Modeling;

namespace Wc3.Render;

/// <summary>Encodes a decoded <see cref="TextureImage"/> (RGBA8) to PNG bytes.</summary>
public static class TexturePng
{
    public static byte[] Encode(TextureImage texture)
    {
        ArgumentNullException.ThrowIfNull(texture);
        using var image = Image.LoadPixelData<Rgba32>(texture.Rgba, texture.Width, texture.Height);
        using var ms = new MemoryStream();
        image.SaveAsPng(ms);
        return ms.ToArray();
    }
}
