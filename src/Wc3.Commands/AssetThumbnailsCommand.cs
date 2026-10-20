// src/Wc3.Commands/AssetThumbnailsCommand.cs
using Wc3.Model;
using Wc3.Modeling;
using Wc3.Render;

namespace Wc3.Commands;

/// <summary>One thumbnail, the file name it should land under, the model it came from and the
/// PNG bytes. Pure data, the front-end decides where the file is written.</summary>
public sealed record Thumbnail(string Name, string Source, byte[] Png, int Width, int Height);

/// <summary>The render outcome. <see cref="Rendered"/> is the number of PNGs produced and
/// <see cref="Diagnostics"/> names the models that could not be read or rendered.</summary>
public sealed record AssetThumbnailsResult(
    int Rendered,
    IReadOnlyList<Thumbnail> Thumbnails,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Renders a thumbnail PNG for every model in a folder, or for every imported model inside
/// every map in a folder. It reuses <see cref="ModelRenderer"/> and the map-side texture
/// resolution of <see cref="RenderModelCommand"/>, so a thumbnail is the same picture the
/// render-model command would draw.
///
/// A folder of models yields one PNG per .mdx/.mdl file, textures looked up beside it by base
/// name. A folder of maps yields one PNG per map-imported model, named
/// "&lt;map&gt;__&lt;model&gt;.png" so two maps with the same model do not collide. Read-only
/// over the source folder and over every map it holds, the caller writes the bytes.
/// </summary>
public static class AssetThumbnailsCommand
{
    public const int DefaultSize = 256;

    private static readonly HashSet<string> ModelExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".mdx", ".mdl" };

    private static readonly HashSet<string> MapExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".w3x", ".w3m" };

    public static AssetThumbnailsResult Execute(string folder, int width = DefaultSize, int height = DefaultSize)
    {
        if (string.IsNullOrWhiteSpace(folder))
            throw new ArgumentException("a folder is required", nameof(folder));
        if (!Directory.Exists(folder))
            throw new DirectoryNotFoundException($"folder not found: {folder}");
        if (width <= 0 || height <= 0)
            throw new ArgumentOutOfRangeException(nameof(width), "thumbnail dimensions must be positive");

        var thumbnails = new List<Thumbnail>();
        var diagnostics = new List<string>();
        var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var file in Directory.EnumerateFiles(folder).OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
        {
            var ext = Path.GetExtension(file);
            if (ModelExtensions.Contains(ext))
                RenderModelFile(file, folder, width, height, thumbnails, diagnostics, used);
            else if (MapExtensions.Contains(ext))
                RenderMapModels(file, width, height, thumbnails, diagnostics, used);
        }

        return new AssetThumbnailsResult(thumbnails.Count, thumbnails, diagnostics);
    }

    private static void RenderModelFile(
        string file, string folder, int width, int height,
        List<Thumbnail> thumbnails, List<string> diagnostics, HashSet<string> used)
    {
        var label = Path.GetFileName(file);
        try
        {
            var model = ModelParser.Parse(File.ReadAllBytes(file), label);
            var png = ModelRenderer.RenderPng(
                model, ResolveFolderTextures(folder, model), width, height);
            thumbnails.Add(new Thumbnail(Unique(Path.GetFileNameWithoutExtension(file) + ".png", used),
                label, png, width, height));
        }
        catch (Exception ex)
        {
            diagnostics.Add($"'{label}' could not be rendered, {ex.Message}");
        }
    }

    private static void RenderMapModels(
        string mapPath, int width, int height,
        List<Thumbnail> thumbnails, List<string> diagnostics, HashSet<string> used)
    {
        var mapLabel = Path.GetFileName(mapPath);
        MapDocument doc;
        try
        {
            doc = MapDocument.Load(mapPath);
        }
        catch (Exception ex)
        {
            diagnostics.Add($"'{mapLabel}' could not be opened, {ex.Message}");
            return;
        }

        foreach (var entry in AssetReferences.ImportedEntries(doc))
        {
            var name = entry.FileName!;
            if (!ModelExtensions.Contains(Path.GetExtension(name))) continue;
            var label = $"{mapLabel} {name}";
            try
            {
                var prepared = RenderModelCommand.Prepare(doc, name);
                var png = prepared.RenderPng(width, height);
                var stem = Path.GetFileNameWithoutExtension(mapPath) + "__" + Path.GetFileNameWithoutExtension(name);
                thumbnails.Add(new Thumbnail(Unique(stem + ".png", used), label, png, width, height));
            }
            catch (Exception ex)
            {
                diagnostics.Add($"'{label}' could not be rendered, {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Textures for a model read straight off disk. A texture path is matched to a sibling
    /// file by base name, ignoring the folder and the extension, so "Textures\Foo.blp" finds
    /// "Foo.blp" in the folder. Unmatched or undecodable textures are simply absent and the
    /// renderer shades those geosets flat.
    /// </summary>
    internal static IReadOnlyDictionary<int, TextureImage> ResolveFolderTextures(string folder, Model3D model)
    {
        var byBaseName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var file in Directory.EnumerateFiles(folder))
            byBaseName.TryAdd(Path.GetFileNameWithoutExtension(file), file);

        var textures = new Dictionary<int, TextureImage>();
        for (int i = 0; i < model.Textures.Count; i++)
        {
            var path = model.Textures[i];
            if (string.IsNullOrWhiteSpace(path)
                || path.StartsWith("ReplaceableId:", StringComparison.OrdinalIgnoreCase))
                continue;
            var baseName = Path.GetFileNameWithoutExtension(path.Replace('\\', '/'));
            if (!byBaseName.TryGetValue(baseName, out var file)) continue;
            if (RenderModelCommand.TryDecodeTexture(File.ReadAllBytes(file)) is { } decoded)
                textures[i] = decoded;
        }
        return textures;
    }

    private static string Unique(string name, HashSet<string> used)
    {
        if (used.Add(name)) return name;
        var stem = Path.GetFileNameWithoutExtension(name);
        var ext = Path.GetExtension(name);
        for (int i = 2; ; i++)
        {
            var candidate = $"{stem}_{i}{ext}";
            if (used.Add(candidate)) return candidate;
        }
    }
}
