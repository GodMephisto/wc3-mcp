// src/Wc3.Commands/ConvertCommand.cs
using Wc3.Model;
using Wc3.Modeling;
using Wc3.Render;

namespace Wc3.Commands;

/// <summary>
/// A model exported to OBJ: the .obj text, the .mtl text it references (as
/// "&lt;model basename&gt;.mtl"), and the referenced textures converted to PNG,
/// keyed by the file name the MTL's map_Kd lines use. Pure data — the
/// front-end decides where (and whether) files land on disk.
/// </summary>
public sealed record ModelExport(string Obj, string Mtl, IReadOnlyDictionary<string, byte[]> Textures);

/// <summary>Format conversions: images (blp/dds/png/jpg/bmp/tga/gif) and model→OBJ.</summary>
public static class ConvertCommand
{
    /// <summary>Converts image bytes between formats; extensions pick the codecs.</summary>
    public static byte[] ConvertImage(byte[] input, string fromExt, string toExt) =>
        TextureConvert.Convert(input, fromExt, toExt);

    /// <summary>Converts a map-internal image (both slash conventions tried).</summary>
    public static byte[] ConvertImageFile(MapDocument doc, string internalName, string toExt)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var entry = FindFile(doc, internalName)
            ?? throw new FileNotFoundException($"'{internalName}' not found in map", internalName);
        return TextureConvert.Convert(entry.RawBytes, Path.GetExtension(internalName), toExt);
    }

    /// <summary>
    /// Exports a map-imported model to OBJ + MTL, converting every map-resolvable
    /// referenced texture to PNG (named "&lt;texture basename&gt;.png").
    /// ReplaceableId/base-game/undecodable textures keep their material but get
    /// no map_Kd. The .mdx/.mdl path swap is handled like everywhere else.
    /// </summary>
    public static ModelExport ExportModelToObj(MapDocument doc, string modelInternalPath)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var entry = RenderModelCommand.FindModelEntry(doc, modelInternalPath)
            ?? throw new InvalidDataException(
                $"model '{modelInternalPath}' is not in the map — only map-imported models export");
        var model = ModelParser.Parse(entry.RawBytes, entry.FileName!);
        return Export(model, Path.GetFileNameWithoutExtension(entry.FileName!), doc);
    }

    /// <summary>
    /// Exports standalone model bytes (a .mdx/.mdl on disk) to OBJ + MTL. With no
    /// map to pull textures from, materials reference "&lt;texture basename&gt;.png"
    /// so separately converted textures line up; the Textures dictionary is empty.
    /// </summary>
    public static ModelExport ExportModelToObj(byte[] modelBytes, string fileName)
    {
        ArgumentNullException.ThrowIfNull(modelBytes);
        var model = ModelParser.Parse(modelBytes, fileName);
        return Export(model, Path.GetFileNameWithoutExtension(fileName), doc: null);
    }

    private static ModelExport Export(Model3D model, string baseName, MapDocument? doc)
    {
        var fileByTexId = new Dictionary<int, string>();
        var textures = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);

        foreach (var id in model.Geosets.Select(g => g.TextureId)
                     .Where(id => id >= 0 && id < model.Textures.Count).Distinct())
        {
            var path = model.Textures[id];
            if (string.IsNullOrWhiteSpace(path)
                || path.StartsWith("ReplaceableId:", StringComparison.OrdinalIgnoreCase))
                continue;

            var pngName = PngNameFor(path, id, textures);
            if (doc is null)
            {
                fileByTexId[id] = pngName; // referenced, not exported
                continue;
            }

            var entry = FindFile(doc, path);
            if (entry is null || entry.RawBytes.Length == 0)
                continue; // base-game/CASC texture — material stays plain
            try
            {
                textures[pngName] = TextureConvert.Convert(
                    entry.RawBytes, Path.GetExtension(path), ".png");
                fileByTexId[id] = pngName;
            }
            catch (Exception)
            {
                // Undecodable import, material stays plain.
            }
        }

        var obj = ObjExporter.ToObj(model, baseName + ".mtl");
        var mtl = ObjExporter.ToMtl(model, fileByTexId);
        return new ModelExport(obj, mtl, textures);
    }

    /// <summary>Basename + .png, suffixed with the texture id on a rare name clash.</summary>
    private static string PngNameFor(string texturePath, int texId, IReadOnlyDictionary<string, byte[]> taken)
    {
        var baseName = Path.GetFileNameWithoutExtension(texturePath.Replace('\\', '/'));
        var name = baseName + ".png";
        return taken.ContainsKey(name) ? $"{baseName}_{texId}.png" : name;
    }

    private static MapFileEntry? FindFile(MapDocument doc, string path) =>
        doc.GetFile(path)
        ?? doc.GetFile(path.Replace('/', '\\'))
        ?? doc.GetFile(path.Replace('\\', '/'));
}
