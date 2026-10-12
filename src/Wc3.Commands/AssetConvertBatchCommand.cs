// src/Wc3.Commands/AssetConvertBatchCommand.cs
using Wc3.Render;

namespace Wc3.Commands;

/// <summary>One converted texture, its path relative to the source folder with the target
/// extension, the source file it came from and the converted bytes.</summary>
public sealed record ConvertedTexture(string RelativePath, string SourcePath, byte[] Bytes);

/// <summary>The batch outcome. <see cref="Converted"/> is the number of textures produced,
/// <see cref="ToExtension"/> is the normalised target extension, and <see cref="Diagnostics"/>
/// lists the files that were skipped or could not be converted.</summary>
public sealed record AssetConvertBatchResult(
    int Converted,
    string ToExtension,
    IReadOnlyList<ConvertedTexture> Textures,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Converts every texture in a folder to one target format, reusing <see cref="TextureConvert"/>.
/// The scan is recursive, so a folder laid out like an import tree ("war3mapImported\a.blp")
/// converts in place. Each result keeps its path relative to the source folder with only the
/// extension changed, so the caller can mirror the tree into the output directory. Read-only
/// over the source, which is opened for reading and never written.
/// </summary>
public static class AssetConvertBatchCommand
{
    private static readonly HashSet<string> TextureExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".blp", ".dds", ".png", ".jpg", ".jpeg", ".bmp", ".tga", ".gif",
        };

    /// <summary>Targets <see cref="TextureConvert"/> can encode. DDS is source-only and is not
    /// offered, since the converter has no DDS encoder and a silent fallback would be a lie.</summary>
    private static readonly HashSet<string> EncodableTargets =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".png", ".jpg", ".jpeg", ".bmp", ".tga", ".gif", ".blp",
        };

    public static AssetConvertBatchResult Execute(string folder, string toExtension)
    {
        if (string.IsNullOrWhiteSpace(folder))
            throw new ArgumentException("a folder is required", nameof(folder));
        if (!Directory.Exists(folder))
            throw new DirectoryNotFoundException($"folder not found: {folder}");

        var to = NormalizeExtension(toExtension);
        if (!EncodableTargets.Contains(to))
            throw new ArgumentException(
                $"unsupported target format '{toExtension}', use .png, .jpg, .jpeg, .bmp, .tga, .gif or .blp",
                nameof(toExtension));

        var textures = new List<ConvertedTexture>();
        var diagnostics = new List<string>();
        foreach (var file in Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories)
                     .OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
        {
            var from = Path.GetExtension(file);
            if (!TextureExtensions.Contains(from)) continue;
            var relative = Path.GetRelativePath(folder, file);
            if (from.Equals(to, StringComparison.OrdinalIgnoreCase))
            {
                diagnostics.Add($"'{relative}' is already {to}, skipped");
                continue;
            }
            try
            {
                var bytes = TextureConvert.Convert(File.ReadAllBytes(file), from, to);
                textures.Add(new ConvertedTexture(
                    Path.ChangeExtension(relative, to), file, bytes));
            }
            catch (Exception ex)
            {
                diagnostics.Add($"'{relative}' could not convert to {to}, {ex.Message}");
            }
        }

        return new AssetConvertBatchResult(textures.Count, to, textures, diagnostics);
    }

    private static string NormalizeExtension(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
            throw new ArgumentException("a target extension is required", nameof(extension));
        return extension.StartsWith('.') ? extension.ToLowerInvariant() : "." + extension.ToLowerInvariant();
    }
}
