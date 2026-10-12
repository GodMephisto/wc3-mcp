// src/Wc3.Commands/AssetImportWireCommand.cs
using War3Net.Build.Import;
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// The outcome of an import-wire run. <see cref="Ok"/> is false only when nothing was imported.
/// <see cref="Imported"/> lists the internal paths added to the archive,
/// <see cref="FieldsSet"/> the object fields repointed at them, and
/// <see cref="Diagnostics"/> the files that were skipped or fields that could not be set.
/// </summary>
public sealed record AssetImportWireResult(
    bool Ok,
    string Message,
    IReadOnlyList<string> Imported,
    IReadOnlyList<string> FieldsSet,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Adds disk files to a map as imports and, when a file is a model or an icon and an object is
/// named, points that object's matching field at the new import. One step that otherwise takes
/// three, import the file, register it in war3map.imp, then hand-edit a field.
///
/// Each file lands at "war3mapImported\&lt;file name&gt;" and is registered in the war3map.imp
/// manifest the way the World Editor stores a custom-path import. When
/// <paramref name="objectRawcode"/> is given the kind is probed (or taken from
/// <paramref name="kind"/>), a .mdx/.mdl file sets the kind's model field and a texture file
/// (.blp/.dds/.tga/.png) sets its icon field, through <see cref="ObjectSetCommand"/> so the edit
/// is byte-faithful and layer-aware. Nothing is saved here, the front-end writes a copy and
/// refuses to overwrite the input map.
/// </summary>
public static class AssetImportWireCommand
{
    private static readonly HashSet<string> ModelExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".mdx", ".mdl" };

    private static readonly HashSet<string> IconExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".blp", ".dds", ".tga", ".png" };

    /// <summary>The model and icon field per kind, or null where the kind has no such field.
    /// Confirmed against the base game metadata (unit umdl/uico, item ifil/iico,
    /// destructable bfil/bico, doodad dfil with no icon).</summary>
    private static readonly IReadOnlyDictionary<ObjectKind, (string Model, string? Icon)> FieldsByKind =
        new Dictionary<ObjectKind, (string Model, string? Icon)>
        {
            [ObjectKind.Unit] = ("umdl", "uico"),
            [ObjectKind.Item] = ("ifil", "iico"),
            [ObjectKind.Destructable] = ("bfil", "bico"),
            [ObjectKind.Doodad] = ("dfil", null),
        };

    public static AssetImportWireResult Execute(
        MapDocument doc,
        IReadOnlyList<string> files,
        string? objectRawcode = null,
        ObjectKind? kind = null)
    {
        ArgumentNullException.ThrowIfNull(doc);
        ArgumentNullException.ThrowIfNull(files);
        var imported = new List<string>();
        var fieldsSet = new List<string>();
        var diagnostics = new List<string>();

        if (objectRawcode is { Length: > 0 } && objectRawcode.Length != 4)
            return new(false, $"invalid object rawcode '{objectRawcode}', expected 4 characters",
                imported, fieldsSet, diagnostics);

        var pairs = new List<(string File, string InternalPath)>();
        foreach (var file in files)
        {
            if (string.IsNullOrWhiteSpace(file)) continue;
            if (!File.Exists(file))
            {
                diagnostics.Add($"'{file}' does not exist, skipped");
                continue;
            }
            var internalPath = ImportsCommand.DefaultImportPrefix + Path.GetFileName(file);
            FileEditCommand.AddOrReplace(doc, internalPath, File.ReadAllBytes(file));
            imported.Add(internalPath);
            pairs.Add((file, internalPath));
        }

        if (imported.Count == 0)
            return new(false, "no importable files were given", imported, fieldsSet, diagnostics);

        RegisterImports(doc, imported, diagnostics);

        if (!string.IsNullOrWhiteSpace(objectRawcode))
            WireFields(doc, pairs, objectRawcode, kind, fieldsSet, diagnostics);

        var message = $"imported {imported.Count} file(s)"
            + (fieldsSet.Count > 0 ? $", set {fieldsSet.Count} field(s)" : "");
        return new(true, message, imported, fieldsSet, diagnostics);
    }

    /// <summary>Registers every new path in war3map.imp, creating the manifest when the map has
    /// none. An unparseable existing manifest is left alone and reported, since rebuilding it
    /// would silently drop every import the map already listed.</summary>
    private static void RegisterImports(MapDocument doc, IReadOnlyList<string> paths, List<string> diagnostics)
    {
        var entry = doc.GetFile(ImportsCommand.ImpFileName);
        if (entry is not null && entry.Model is not ImportedFiles)
        {
            diagnostics.Add("war3map.imp is present but could not be parsed, the manifest was left unchanged");
            return;
        }

        var imports = entry?.Model as ImportedFiles ?? new ImportedFiles(ImportedFilesFormatVersion.v1);
        var existing = imports.Files
            .Select(f => AssetReferences.Normalize(f.FullPath))
            .ToHashSet(StringComparer.Ordinal);

        bool changed = false;
        foreach (var path in paths)
        {
            if (!existing.Add(AssetReferences.Normalize(path))) continue;
            imports.Files.Add(new ImportedFile { Flags = (ImportedFileFlags)10, FullPath = path });
            changed = true;
        }
        if (changed)
            doc.AddOrReplaceModelFile(ImportsCommand.ImpFileName, imports);
    }

    private static void WireFields(
        MapDocument doc,
        IReadOnlyList<(string File, string InternalPath)> pairs,
        string rawcode,
        ObjectKind? kind,
        List<string> fieldsSet,
        List<string> diagnostics)
    {
        var resolved = kind ?? DetectKind(doc, rawcode);
        if (resolved is null)
        {
            diagnostics.Add($"object '{rawcode}' was not found in the map, no field was set");
            return;
        }
        if (!FieldsByKind.TryGetValue(resolved.Value, out var fields))
        {
            diagnostics.Add($"{resolved.Value.ToString().ToLowerInvariant()} objects have no model or icon field");
            return;
        }

        foreach (var (file, internalPath) in pairs)
        {
            var ext = Path.GetExtension(file);
            string? code = ModelExtensions.Contains(ext) ? fields.Model
                : IconExtensions.Contains(ext) ? fields.Icon
                : null;
            if (code is null)
            {
                diagnostics.Add($"'{Path.GetFileName(file)}' is neither a model nor an icon, no field set");
                continue;
            }

            var result = ObjectSetCommand.Execute(doc, resolved.Value, rawcode, code, internalPath);
            if (result.Ok)
                fieldsSet.Add($"{code}={internalPath}");
            else
                diagnostics.Add(result.Message);
        }
    }

    /// <summary>The first kind whose merged entries hold the rawcode, or null.</summary>
    private static ObjectKind? DetectKind(MapDocument doc, string rawcode)
    {
        int id = rawcode.FromRawcode();
        foreach (var candidate in ObjectKinds.All)
        {
            if (!FieldsByKind.ContainsKey(candidate)) continue;
            if (ObjectKinds.MergedEntries(doc, ObjectKinds.Info(candidate)).Any(e => e.Id == id))
                return candidate;
        }
        return null;
    }
}
