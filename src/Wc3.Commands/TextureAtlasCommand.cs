// src/Wc3.Commands/TextureAtlasCommand.cs
using Wc3.Modeling;
using Wc3.Render;

namespace Wc3.Commands;

/// <summary>Where one input image landed in the atlas, by path and by pixel rectangle.</summary>
public sealed record TextureAtlasEntry(string Path, string Name, int X, int Y, int Width, int Height);

/// <summary>What the atlas pack produced, for the report and for the JSON mapping.</summary>
public sealed record TextureAtlasResult(
    string Output,
    string Format,
    int AtlasWidth,
    int AtlasHeight,
    long Bytes,
    IReadOnlyList<TextureAtlasEntry> Entries);

/// <summary>
/// Packs two or more textures into one power-of-two atlas and reports each input's rectangle in
/// pixels. The atlas is written in the first input's format, so a caller packs a set of .blp faces
/// and gets a .blp back. Every input is read through the same decoder the renderer uses, so .blp,
/// .dds and the standard formats are all accepted and never modified. The packing itself is
/// deterministic, see <see cref="AtlasPacker"/>.
/// </summary>
public static class TextureAtlasCommand
{
    public static TextureAtlasResult ExecutePaths(IReadOnlyList<string> paths, int maxSize, string outPath)
    {
        ArgumentNullException.ThrowIfNull(paths);
        if (paths.Count < 2) throw new ArgumentException("atlas needs at least two input images", nameof(paths));
        ArgumentException.ThrowIfNullOrWhiteSpace(outPath);

        var inputs = new List<AtlasInput>(paths.Count);
        foreach (var path in paths)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            if (!File.Exists(path)) throw new FileNotFoundException($"texture not found: {path}", path);
            var tex = TextureConvert.DecodeRgba(File.ReadAllBytes(path), Path.GetExtension(path));
            inputs.Add(new AtlasInput(Path.GetFileName(path), tex.Width, tex.Height, tex.Rgba));
        }

        var format = Path.GetExtension(paths[0]).ToLowerInvariant();
        var pack = AtlasPacker.Pack(inputs, maxSize);
        var atlas = TextureConvert.EncodeRgba(new TextureImage(pack.Width, pack.Height, pack.Rgba), format);
        File.WriteAllBytes(outPath, atlas);

        var entries = new List<TextureAtlasEntry>(pack.Placements.Count);
        for (int i = 0; i < pack.Placements.Count; i++)
        {
            var p = pack.Placements[i];
            entries.Add(new TextureAtlasEntry(paths[i], p.Name, p.X, p.Y, p.Width, p.Height));
        }
        return new TextureAtlasResult(outPath, format.TrimStart('.'), pack.Width, pack.Height, atlas.LongLength, entries);
    }
}
