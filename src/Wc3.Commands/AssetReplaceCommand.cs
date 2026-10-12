// src/Wc3.Commands/AssetReplaceCommand.cs
using System.Text.RegularExpressions;
using War3Net.Build.Import;
using War3Net.Build.Object;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Outcome of an asset path replacement, with a count per place it was found. <see cref="Ok"/>
/// is false and nothing is changed when the old path is not referenced anywhere.
/// </summary>
public sealed record AssetReplaceResult(
    bool Ok,
    string Message,
    string OldPath,
    string NewPath,
    int ObjectFieldsChanged,
    int ScriptLiteralsChanged,
    int ImportTableEntriesChanged,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// Repoints every reference to one asset path at another, in the three places a map names a
/// file, which are object-data field values across all seven kinds and both Reforged layers,
/// script string literals, and the war3map.imp import table. Refuses when the old path is named
/// nowhere, so a typo cannot produce a saved copy that silently changed nothing.
///
/// Matching mirrors the read side rather than a byte compare. A field holding the path
/// extensionless, or under its ".mdl" spelling while the archive stores ".mdx", still matches,
/// because <see cref="AssetReferences.TokenNames"/> expands a token the same way the unused and
/// duplicate scans judge an archive entry.
///
/// The archive entry itself is NOT renamed. The map model cannot remove an entry, so a rename
/// would leave the old bytes in place beside any new ones, and this command deliberately changes
/// references only. Move the file (or re-import it under the new path) when the reference must
/// resolve.
/// </summary>
public static class AssetReplaceCommand
{
    // The same literal shape the name harvester scans, a quoted run with no line break, three to
    // 260 characters, so "what counts as a path literal" is one rule across the toolkit.
    private static readonly Regex StringLiteral = new("\"([^\"\r\n]{3,260})\"", RegexOptions.Compiled);

    public static AssetReplaceResult Execute(MapDocument doc, string oldPath, string newPath)
    {
        ArgumentNullException.ThrowIfNull(doc);
        var diagnostics = new List<string>();
        if (string.IsNullOrWhiteSpace(oldPath) || string.IsNullOrWhiteSpace(newPath))
            return Refuse(oldPath, newPath, "both the old and the new path are required");

        int objectFields = RewriteObjectData(doc, oldPath, newPath);
        int scriptLiterals = RewriteScript(doc, oldPath, newPath);
        int importEntries = RewriteImportTable(doc, oldPath, newPath, diagnostics);

        int total = objectFields + scriptLiterals + importEntries;
        if (total == 0)
            return Refuse(oldPath, newPath,
                $"'{oldPath}' is not referenced by any object-data field, script string literal "
                + "or import table entry, so nothing was changed");

        return new AssetReplaceResult(
            Ok: true,
            Message: $"replaced '{oldPath}' with '{newPath}' in {objectFields} object field(s), "
                + $"{scriptLiterals} script literal(s) and {importEntries} import entry(ies). "
                + "The archive entry is not renamed, move the file if the new path must resolve.",
            oldPath, newPath, objectFields, scriptLiterals, importEntries, diagnostics);
    }

    private static AssetReplaceResult Refuse(string oldPath, string newPath, string message) =>
        new(false, message, oldPath, newPath, 0, 0, 0, Array.Empty<string>());

    /// <summary>Every object-data modification whose string value names the old path.</summary>
    private static int RewriteObjectData(MapDocument doc, string oldPath, string newPath)
    {
        int changed = 0;
        foreach (var kind in ObjectKinds.All)
        {
            var info = ObjectKinds.Info(kind);
            foreach (var file in new[] { info.MapFile, info.SkinFile })
            {
                var model = doc.GetFile(file)?.Model;
                if (model is null) continue;
                int n = RewriteModel(model, oldPath, newPath);
                if (n > 0)
                {
                    doc.AddOrReplaceModelFile(file, model);
                    changed += n;
                }
            }
        }
        return changed;
    }

    private static int RewriteModel(object model, string oldPath, string newPath)
    {
        int changed = 0;
        foreach (var mod in AllModifications(model))
        {
            if (mod.Value is not string value) continue;
            var rewritten = RewriteTokenList(value, oldPath, newPath);
            if (!ReferenceEquals(rewritten, value))
            {
                mod.Value = rewritten;
                changed++;
            }
        }
        return changed;
    }

    /// <summary>Rewrites the old path inside a possibly comma-separated field value, returning
    /// the SAME string instance when nothing matched so callers can count with ReferenceEquals.</summary>
    private static string RewriteTokenList(string value, string oldPath, string newPath)
    {
        bool changed = false;
        var parts = value.Split(',');
        for (int i = 0; i < parts.Length; i++)
        {
            if (!AssetReferences.TokenNames(parts[i].Trim(), oldPath)) continue;
            parts[i] = newPath;
            changed = true;
        }
        return changed ? string.Join(",", parts) : value;
    }

    /// <summary>Rewrites the old path inside every script string literal that names it,
    /// preserving the literal's quotes and re-escaping the new path for the script source.</summary>
    private static int RewriteScript(MapDocument doc, string oldPath, string newPath)
    {
        if (AssetReferences.ScriptEntry(doc) is not { FileName: { } scriptName } entry) return 0;

        var text = ScriptText.GetString(entry.CurrentBytes);
        int changed = 0;
        var rewritten = StringLiteral.Replace(text, match =>
        {
            var unescaped = AssetPathCandidates.Unescape(match.Groups[1].Value);
            if (!AssetReferences.TokenNames(unescaped, oldPath)) return match.Value;
            changed++;
            return "\"" + newPath.Replace("\\", "\\\\") + "\"";
        });
        if (changed > 0)
            doc.AddOrReplaceRawFile(scriptName, ScriptText.GetBytes(rewritten));
        return changed;
    }

    /// <summary>Rewrites the old path in the war3map.imp manifest, matching the separator and
    /// default-prefix spellings the import listing already tolerates.</summary>
    private static int RewriteImportTable(
        MapDocument doc, string oldPath, string newPath, List<string> diagnostics)
    {
        var entry = doc.GetFile(ImportsCommand.ImpFileName);
        if (entry is null) return 0;
        if (entry.Model is not ImportedFiles imports)
        {
            diagnostics.Add("war3map.imp is present but could not be parsed, left unchanged");
            return 0;
        }

        int changed = 0;
        foreach (var file in imports.Files)
        {
            if (!PathEqualsOld(file.FullPath, oldPath)) continue;
            file.FullPath = newPath;
            changed++;
        }
        if (changed > 0) doc.AddOrReplaceModelFile(ImportsCommand.ImpFileName, imports);
        return changed;
    }

    private static bool PathEqualsOld(string candidate, string oldPath)
    {
        var c = AssetReferences.Normalize(candidate);
        var o = AssetReferences.Normalize(oldPath);
        if (c == o) return true;
        // WorldEdit often stores a non-custom-path import without the war3mapImported\ prefix,
        // which the listing resolves the same way, so the manifest spelling is matched too.
        return AssetReferences.Normalize(ImportsCommand.DefaultImportPrefix + candidate) == o
            || AssetReferences.Normalize(ImportsCommand.DefaultImportPrefix + oldPath) == c;
    }

    private static IEnumerable<ObjectDataModification> AllModifications(object model) => model switch
    {
        UnitObjectData m => Simple(m.BaseUnits, m.NewUnits),
        ItemObjectData m => Simple(m.BaseItems, m.NewItems),
        DestructableObjectData m => Simple(m.BaseDestructables, m.NewDestructables),
        BuffObjectData m => Simple(m.BaseBuffs, m.NewBuffs),
        AbilityObjectData m => Level(m.BaseAbilities, m.NewAbilities),
        UpgradeObjectData m => Level(m.BaseUpgrades, m.NewUpgrades),
        DoodadObjectData m => Variation(m.BaseDoodads, m.NewDoodads),
        _ => Enumerable.Empty<ObjectDataModification>(),
    };

    private static IEnumerable<ObjectDataModification> Simple(
        IEnumerable<SimpleObjectModification> bases, IEnumerable<SimpleObjectModification> news) =>
        bases.Concat(news).SelectMany(g => g.Modifications);

    private static IEnumerable<ObjectDataModification> Level(
        IEnumerable<LevelObjectModification> bases, IEnumerable<LevelObjectModification> news) =>
        bases.Concat(news).SelectMany(g => g.Modifications);

    private static IEnumerable<ObjectDataModification> Variation(
        IEnumerable<VariationObjectModification> bases, IEnumerable<VariationObjectModification> news) =>
        bases.Concat(news).SelectMany(g => g.Modifications);
}
