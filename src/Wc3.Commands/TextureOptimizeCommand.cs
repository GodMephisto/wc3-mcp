// src/Wc3.Commands/TextureOptimizeCommand.cs
using Wc3.Render;

namespace Wc3.Commands;

/// <summary>What a texture optimise changed, before and after, for the report.</summary>
public sealed record TextureOptimizeResult(
    string Path,
    string Format,
    int WidthBefore,
    int HeightBefore,
    int WidthAfter,
    int HeightAfter,
    long BytesBefore,
    long BytesAfter);

/// <summary>
/// Shrinks a texture to fit a maximum side, preserving aspect ratio, and writes it back in the same
/// format. A WC3 texture is usually far larger than the engine needs, and it is the other half of
/// making a model compact, since the model only references it. Read the texture through the same
/// decoder the renderer uses, so .blp and .dds are handled alongside the standard formats.
/// </summary>
public static class TextureOptimizeCommand
{
    public static TextureOptimizeResult ExecutePath(string path, int maxDimension, string outPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (!File.Exists(path)) throw new FileNotFoundException($"texture not found: {path}", path);
        var ext = Path.GetExtension(path).ToLowerInvariant();
        var result = Execute(File.ReadAllBytes(path), outPath, ext, maxDimension, out var output);
        File.WriteAllBytes(outPath, output);
        return result;
    }

    /// <summary>
    /// The in-memory half, used when the texture lives inside an archive rather than on disk. The
    /// caller names the entry and its extension, because a map entry's bytes carry no path.
    /// </summary>
    public static TextureOptimizeResult Execute(
        byte[] bytes, string name, string extension, int maxDimension, out byte[] output)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        var ext = string.IsNullOrEmpty(extension) || extension.StartsWith('.')
            ? extension : "." + extension;
        ext = ext.ToLowerInvariant();

        var (w0, h0) = TextureConvert.Dimensions(bytes, ext);
        output = TextureConvert.ConvertScaled(bytes, ext, ext, maxDimension);
        var (w1, h1) = TextureConvert.Dimensions(output, ext);

        return new TextureOptimizeResult(name, ext.TrimStart('.'),
            w0, h0, w1, h1, bytes.LongLength, output.LongLength);
    }
}
