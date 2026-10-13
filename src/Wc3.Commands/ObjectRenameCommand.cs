// src/Wc3.Commands/ObjectRenameCommand.cs
using System.Text.RegularExpressions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// Outcome of renaming an object rawcode. FieldsRewritten counts object-data field values that
/// named the old rawcode, ScriptLiteralsRewritten counts script string literals, and both are
/// zero when the map referenced it nowhere or rewrite-refs was not asked for.
/// </summary>
public sealed record ObjectRenameResult(
    bool Ok, string Message, string Kind, string OldRawcode, string NewRawcode,
    int FieldsRewritten, int ScriptLiteralsRewritten, IReadOnlyList<string> Diagnostics);

/// <summary>
/// Renames one object's rawcode in a kind and, when asked, repoints every reference to it.
/// The object's own group is re-keyed (a custom's NewId, a modified standard's OldId), and with
/// --rewrite-refs every object-data field value and script string literal that names the old
/// rawcode as a token is rewritten, so an ability list or a JASS reference does not dangle.
/// Refuses a new rawcode already taken by the map or one equal to the old. Read-only on its
/// input, the caller saves the edited copy.
/// </summary>
public static class ObjectRenameCommand
{
    private static readonly Regex DoubleQuoted = new("\"([^\"\r\n]*)\"", RegexOptions.Compiled);
    private static readonly Regex SingleQuoted = new("'([^'\r\n]*)'", RegexOptions.Compiled);

    public static ObjectRenameResult Execute(
        MapDocument doc, ObjectKind kind, string oldRawcode, string newRawcode, bool rewriteRefs)
    {
        string kindName = kind.ToString().ToLowerInvariant();
        if (oldRawcode.Length != 4)
            return Refuse(kindName, oldRawcode, newRawcode, $"invalid rawcode '{oldRawcode}', expected 4 characters");
        if (newRawcode.Length != 4)
            return Refuse(kindName, oldRawcode, newRawcode, $"invalid new rawcode '{newRawcode}', expected 4 characters");
        if (string.Equals(oldRawcode, newRawcode, StringComparison.OrdinalIgnoreCase))
            return Refuse(kindName, oldRawcode, newRawcode, "the new rawcode is the same as the old");

        var info = ObjectKinds.Info(kind);
        int oldId = oldRawcode.FromRawcode();
        int newId = newRawcode.FromRawcode();

        if (ObjectKinds.MergedEntries(doc, info).All(e => e.Id != oldId))
            return Refuse(kindName, oldRawcode, newRawcode, $"{kindName} {oldRawcode} not found in map");
        if (RawcodeAllocator.UsedRawcodes(doc).Contains(newId))
            return Refuse(kindName, oldRawcode, newRawcode,
                $"{kindName} {newRawcode} already exists in the map");

        // Re-key the object's own group in EVERY layer that holds it, not just the first. A
        // Reforged map can split one object across war3map.* and war3mapSkin.*; renaming only the
        // map layer left the skin group keyed to the old rawcode, so the old code still resolved
        // and the renamed one was missing the skin half (name, model, icon).
        bool renamed = false;
        foreach (var file in new[] { info.MapFile, info.SkinFile })
        {
            var model = doc.GetFile(file)?.Model;
            if (model is null) continue;
            if (!RenameGroup(model, oldId, newId)) continue;
            doc.AddOrReplaceModelFile(file, model);
            renamed = true;
        }
        if (!renamed)
            return Refuse(kindName, oldRawcode, newRawcode,
                $"{kindName} {oldRawcode} has no field data to rename");

        int fields = 0, literals = 0;
        if (rewriteRefs)
        {
            fields = RewriteObjectReferences(doc, oldRawcode, newRawcode);
            literals = RewriteScriptLiterals(doc, oldRawcode, newRawcode);
        }

        string refs = rewriteRefs
            ? $", rewrote {fields} field(s) and {literals} script literal(s)"
            : "";
        return new ObjectRenameResult(
            Ok: true,
            Message: $"renamed {kindName} {oldRawcode} to {newRawcode}{refs}",
            Kind: kindName, OldRawcode: oldRawcode, NewRawcode: newRawcode,
            FieldsRewritten: fields, ScriptLiteralsRewritten: literals,
            Diagnostics: Array.Empty<string>());
    }

    private static ObjectRenameResult Refuse(
        string kind, string oldRawcode, string newRawcode, string message) =>
        new(false, message, kind, oldRawcode, newRawcode, 0, 0, Array.Empty<string>());

    /// <summary>Changes the object's own key in a model. Returns true when the group was found.</summary>
    private static bool RenameGroup(object model, int oldId, int newId)
    {
        switch (model)
        {
            case UnitObjectData m: return Simple(m.BaseUnits, m.NewUnits);
            case ItemObjectData m: return Simple(m.BaseItems, m.NewItems);
            case DestructableObjectData m: return Simple(m.BaseDestructables, m.NewDestructables);
            case BuffObjectData m: return Simple(m.BaseBuffs, m.NewBuffs);
            case AbilityObjectData m: return Level(m.BaseAbilities, m.NewAbilities);
            case UpgradeObjectData m: return Level(m.BaseUpgrades, m.NewUpgrades);
            case DoodadObjectData m: return Variation(m.BaseDoodads, m.NewDoodads);
            default: return false;
        }

        bool Simple(List<SimpleObjectModification> bases, List<SimpleObjectModification> news)
        {
            foreach (var g in news) if (g.NewId == oldId) { g.NewId = newId; return true; }
            foreach (var g in bases) if (g.OldId == oldId) { g.OldId = newId; return true; }
            return false;
        }

        bool Level(List<LevelObjectModification> bases, List<LevelObjectModification> news)
        {
            foreach (var g in news) if (g.NewId == oldId) { g.NewId = newId; return true; }
            foreach (var g in bases) if (g.OldId == oldId) { g.OldId = newId; return true; }
            return false;
        }

        bool Variation(List<VariationObjectModification> bases, List<VariationObjectModification> news)
        {
            foreach (var g in news) if (g.NewId == oldId) { g.NewId = newId; return true; }
            foreach (var g in bases) if (g.OldId == oldId) { g.OldId = newId; return true; }
            return false;
        }
    }

    /// <summary>Rewrites the old rawcode inside every string field value of every kind and layer.</summary>
    private static int RewriteObjectReferences(MapDocument doc, string oldRawcode, string newRawcode)
    {
        int changed = 0;
        foreach (var kind in ObjectKinds.All)
        {
            var info = ObjectKinds.Info(kind);
            foreach (var file in new[] { info.MapFile, info.SkinFile })
            {
                var model = doc.GetFile(file)?.Model;
                if (model is null) continue;
                int n = RewriteModel(model, oldRawcode, newRawcode);
                if (n > 0) { doc.AddOrReplaceModelFile(file, model); changed += n; }
            }
        }
        return changed;
    }

    private static int RewriteModel(object model, string oldRawcode, string newRawcode)
    {
        int changed = 0;
        foreach (var mod in AllModifications(model))
        {
            if (mod.Value is not string value) continue;
            var rewritten = RewriteTokenList(value, oldRawcode, newRawcode);
            if (!ReferenceEquals(rewritten, value)) { mod.Value = rewritten; changed++; }
        }
        return changed;
    }

    /// <summary>Rewrites the old rawcode inside a comma-separated field value, returning the
    /// SAME string instance when nothing matched so callers can count with ReferenceEquals.</summary>
    private static string RewriteTokenList(string value, string oldRawcode, string newRawcode)
    {
        bool changed = false;
        var parts = value.Split(',');
        for (int i = 0; i < parts.Length; i++)
        {
            if (!parts[i].Trim().Equals(oldRawcode, StringComparison.OrdinalIgnoreCase)) continue;
            parts[i] = newRawcode;
            changed = true;
        }
        return changed ? string.Join(",", parts) : value;
    }

    /// <summary>Rewrites the old rawcode inside every script string literal that names it as a
    /// standalone token, preserving the literal's quotes.</summary>
    private static int RewriteScriptLiterals(MapDocument doc, string oldRawcode, string newRawcode)
    {
        if (AssetReferences.ScriptEntry(doc) is not { FileName: { } scriptName } entry) return 0;

        string text = ScriptText.GetString(entry.CurrentBytes);
        int literals = 0;

        string Rewrite(string literal, char quote)
        {
            if (!literal.Contains(oldRawcode, StringComparison.OrdinalIgnoreCase)) return quote + literal + quote;
            var rewritten = ReplaceToken(literal, oldRawcode, newRawcode);
            if (!ReferenceEquals(rewritten, literal)) literals++;
            return quote + rewritten + quote;
        }

        var output = DoubleQuoted.Replace(text, m => Rewrite(m.Groups[1].Value, '"'));
        output = SingleQuoted.Replace(output, m => Rewrite(m.Groups[1].Value, '\''));

        if (literals > 0) doc.AddOrReplaceRawFile(scriptName, ScriptText.GetBytes(output));
        return literals;
    }

    /// <summary>Replaces the old rawcode where it appears delimited by anything that is not a
    /// letter or digit, returning the SAME string instance when nothing matched.</summary>
    private static string ReplaceToken(string text, string oldRawcode, string newRawcode)
    {
        var result = new System.Text.StringBuilder(text.Length);
        bool changed = false;
        int i = 0;
        while (i < text.Length)
        {
            if (i + oldRawcode.Length <= text.Length
                && string.Compare(text, i, oldRawcode, 0, oldRawcode.Length, StringComparison.OrdinalIgnoreCase) == 0
                && (i == 0 || !char.IsLetterOrDigit(text[i - 1]))
                && (i + oldRawcode.Length == text.Length || !char.IsLetterOrDigit(text[i + oldRawcode.Length])))
            {
                result.Append(newRawcode);
                i += oldRawcode.Length;
                changed = true;
            }
            else
            {
                result.Append(text[i]);
                i++;
            }
        }
        return changed ? result.ToString() : text;
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
