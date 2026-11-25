// src/Wc3.Render/TexturePng.cs
using Wc3.Modeling;

namespace Wc3.Render;

/// <summary>Encodes a decoded <see cref="TextureImage"/> (RGBA8) to PNG bytes.</summary>
public static class TexturePng
{
    public static byte[] Encode(TextureImage texture)
    {
        ArgumentNullException.ThrowIfNull(texture);
        using var image = SafeImages.FromRgba(texture.Rgba, texture.Width, texture.Height);
        return SafeImages.EncodePng(image);
    }
}
