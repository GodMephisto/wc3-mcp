// src/Wc3.Commands/ObjectFidelityCommand.cs
using System.Globalization;
using War3Net.Common.Extensions;
using Wc3.GameData;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>Whether a fidelity finding is a real data loss (Error) or context worth seeing but not a
/// fault (Info), for instance a rawcode that legitimately changed on a porting collision.</summary>
public enum FidelitySeverity { Error, Info }

/// <summary>The shape of one fidelity finding, so a front end can group and label them.</summary>
public enum FidelityIssue
{
    ObjectAbsent,        // a source-closure object with no counterpart in the target
    ObjectRemapped,      // no rawcode match, but a same-base target object exists, likely a remap
    BaseChanged,         // same rawcode in both, but built from a different base object
    FieldMissing,        // a field the source sets, absent in the target (falls back to base)
    LevelEntryMissing,   // a per-level or per-variation value present in source, absent in target
    LevelsDropped,       // the source carries more levels or variations than the target
    NumericValueDiff,    // a numeric field differs, the change that actually weakens a hero
    ReferenceValueDiff,  // a rawcode-valued field differs, likely a legitimate remap
    ValueDiff,           // any other value difference (text, path, flag)
    FieldAddedInTarget,  // a field the target sets and the source does not
}

/// <summary>One difference between a source object and its target counterpart. <see cref="Field"/> is
/// the bare field code or "code:N" for a per-level or per-variation value, empty for object-level
/// findings.</summary>
public sealed record FidelityFinding(
    string SourceRawcode, string? TargetRawcode, ObjectKind Kind, string? Name,
    FidelityIssue Issue, FidelitySeverity Severity,
    string Field, string? SourceValue, string? TargetValue, string Detail);

public sealed record ObjectFidelityResult(
    string Root, ObjectKind RootKind, string? Name, int ObjectsCompared,
    IReadOnlyList<FidelityFinding> Findings, IReadOnlyList<string> Diagnostics)
{
    /// <summary>Real losses, the ones that make a ported hero weaker than its source.</summary>
    public int Errors => Findings.Count(f => f.Severity == FidelitySeverity.Error);
    /// <summary>Differences explained by a legitimate port transformation, shown but not faults.</summary>
    public int Infos => Findings.Count(f => f.Severity == FidelitySeverity.Info);
    public bool Faithful => Errors == 0;
    /// <summary>Distinct source objects that carried at least one real loss.</summary>
    public int ObjectsWithLosses => Findings
        .Where(f => f.Severity == FidelitySeverity.Error)
        .Select(f => f.SourceRawcode).Distinct(StringComparer.Ordinal).Count();
}

/// <summary>
/// Compares one object and its whole custom closure between a SOURCE map and a TARGET map, to answer
/// "did the port carry this object faithfully". Object data is per-level and per-variation, not flat,
/// so a port that flattened or dropped the per-level entries would silently weaken a hero while every
/// wiring check still passed. This reports fields the source set that the target lost, per-level
/// values that did not come across, levels dropped wholesale, and objects missing from the target.
///
/// The port legitimately rewrites two things, and neither is counted as a fault. Rawcodes are remapped
/// on collision, so a reference field or an object rawcode may differ, that is surfaced as information.
/// Trigger-string references (TRIGSTR_) are inlined to literal text, so names and tooltips are compared
/// by their resolved text through each map's own string table.
///
/// The comparison is over the maps' own object-data deltas (not merged with base game data), because
/// the deltas are exactly what the port transfers, and comparing them needs no game install.
/// </summary>
public static class ObjectFidelityCommand
{
    /// <summary>Compares using game data auto-detected from <paramref name="gameDir"/> (or the install)
    /// to sharpen the closure crawl. Falls back cleanly to a game-data-free crawl when none is found.</summary>
    public static ObjectFidelityResult Compare(
        MapDocument source, MapDocument target, string rootRawcode, string? gameDir)
    {
        GameData.GameData.TryOpen(gameDir, out var ctx, out _);
        return Compare(source, target, rootRawcode, ctx);
    }

    /// <summary>Core with an explicit game-data context (null = map deltas only, the crawl stays
    /// permissive but still complete). Kept internal so hermetic tests can pass ctx null.</summary>
    internal static ObjectFidelityResult Compare(
        MapDocument source, MapDocument target, string rootRawcode, GameDataContext? ctx)
    {
        var diagnostics = new List<string>();
        var findings = new List<FidelityFinding>();

        var srcIndex = IndexByKind(source);
        var tgtIndex = IndexByKind(target);
        var srcCustomIds = AllIds(srcIndex);
        var tgtCustomIds = AllIds(tgtIndex);
        var srcStrings = MapStrings.From(source);
        var tgtStrings = MapStrings.From(target);

        var kind = KindOf(srcIndex, rootRawcode);
        if (kind is null)
        {
            diagnostics.Add($"'{rootRawcode}' is not a custom object in the source map, nothing to compare");
            return new ObjectFidelityResult(rootRawcode, ObjectKind.Unit, null, 0, findings, diagnostics);
        }

        // The closure to check is the source object's custom dependency graph. Game data lets the crawl
        // follow only genuine reference fields, without it the crawl is permissive but still complete.
        var bundle = BundleCommand.ResolveObject(source, kind.Value, rootRawcode, ctx, Array.Empty<string>());
        diagnostics.AddRange(bundle.Diagnostics.Select(d => "closure, " + d));

        var nodes = bundle.Objects
            .Where(n => n.CustomToMap && n.Rawcode.Length == 4)
            .OrderBy(n => n.Kind).ThenBy(n => n.Rawcode, StringComparer.Ordinal)
            .ToList();
        foreach (var node in nodes)
            CompareObject(node, srcIndex, tgtIndex, srcCustomIds, tgtCustomIds,
                srcStrings, tgtStrings, findings);

        return new ObjectFidelityResult(rootRawcode, kind.Value, bundle.RootName, nodes.Count, findings, diagnostics);
    }

    private static void CompareObject(
        BundleNode node,
        Dictionary<ObjectKind, Dictionary<int, MapObjectEntry>> srcIndex,
        Dictionary<ObjectKind, Dictionary<int, MapObjectEntry>> tgtIndex,
        HashSet<string> srcCustomIds, HashSet<string> tgtCustomIds,
        MapStrings srcStrings, MapStrings tgtStrings,
        List<FidelityFinding> findings)
    {
        var kind = node.Kind;
        int id = node.Rawcode.FromRawcode();
        if (!srcIndex[kind].TryGetValue(id, out var srcEntry)) return; // the node came from the source
        var srcDict = ObjectKinds.ModsToDict(srcEntry.Mods);

        // Match the object in the target by rawcode. When it is not there, it was either dropped (a
        // real loss) or remapped on collision (not a loss). A same-base target object that no source
        // object of its own claims is the tell-tale of a remap, so report that as information rather
        // than crying "absent". Field comparison across a remap is skipped, the pairing is a guess.
        if (!tgtIndex[kind].TryGetValue(id, out var tgtEntry))
        {
            var remaps = tgtIndex[kind].Values
                .Where(e => e.OldId != 0 && e.OldId == srcEntry.OldId && !srcIndex[kind].ContainsKey(e.Id))
                .Select(e => e.Id.ToRawcode())
                .OrderBy(r => r, StringComparer.Ordinal)
                .ToList();
            if (remaps.Count > 0)
                findings.Add(new FidelityFinding(node.Rawcode,
                    remaps.Count == 1 ? remaps[0] : null, kind, node.Name,
                    FidelityIssue.ObjectRemapped, FidelitySeverity.Info, "", null, null,
                    $"no rawcode match in target, but {remaps.Count} target object(s) share its base '{srcEntry.OldId.ToRawcode()}' ({string.Join(", ", remaps)}), likely a remap, fields not compared"));
            else
                findings.Add(new FidelityFinding(node.Rawcode, null, kind, node.Name,
                    FidelityIssue.ObjectAbsent, FidelitySeverity.Error, "", null, null,
                    "present in the source closure but absent from the target map"));
            return;
        }

        var tgtDict = ObjectKinds.ModsToDict(tgtEntry.Mods);

        FidelityFinding F(FidelityIssue issue, FidelitySeverity sev, string field, string? sv, string? tv, string detail)
            => new(node.Rawcode, node.Rawcode, kind, node.Name, issue, sev, field, sv, tv, detail);

        if (srcEntry.OldId != 0 && tgtEntry.OldId != 0 && srcEntry.OldId != tgtEntry.OldId)
            findings.Add(F(FidelityIssue.BaseChanged, FidelitySeverity.Info, "",
                srcEntry.OldId.ToRawcode(), tgtEntry.OldId.ToRawcode(),
                "same rawcode in both maps but built from a different base object"));

        foreach (var (key, srcVal) in srcDict.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
        {
            bool leveled = key.Contains(':');
            if (!tgtDict.TryGetValue(key, out var tgtVal))
            {
                // A dropped field weakens a hero only when it carried a number (damage, cooldown,
                // duration) or a reference to another custom object (a buff or spell that no longer
                // applies). A dropped text field is a lost tooltip or name, cosmetic, so it is
                // information rather than a counted loss.
                bool weakening = AllNumeric(srcVal) || AllTokensIn(srcVal, srcCustomIds);
                findings.Add(F(
                    leveled ? FidelityIssue.LevelEntryMissing : FidelityIssue.FieldMissing,
                    weakening ? FidelitySeverity.Error : FidelitySeverity.Info, key, srcVal, null,
                    (leveled ? "per-level value present in source, absent in target"
                             : "field set in source, absent in target (falls back to the base default)")
                    + (weakening ? "" : ", a text or cosmetic field")));
                continue;
            }
            Classify(key, srcVal, tgtVal, srcCustomIds, tgtCustomIds, srcStrings, tgtStrings,
                node.Rawcode, kind, node.Name, findings);
        }

        foreach (var (key, tgtVal) in tgtDict.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
            if (!srcDict.ContainsKey(key))
                findings.Add(F(FidelityIssue.FieldAddedInTarget, FidelitySeverity.Info, key, null, tgtVal,
                    "field set in target but not in source"));

        // Levels carried, the highest per-level or per-variation index across every field. A source
        // that has more than the target lost whole levels, the plainest form of "works, but weaker".
        int srcLevels = MaxLevel(srcDict), tgtLevels = MaxLevel(tgtDict);
        if (srcLevels > tgtLevels)
            findings.Add(F(FidelityIssue.LevelsDropped, FidelitySeverity.Error, "",
                srcLevels.ToString(CultureInfo.InvariantCulture), tgtLevels.ToString(CultureInfo.InvariantCulture),
                $"source carries {srcLevels} level(s) or variation(s), target only {tgtLevels}"));
    }

    /// <summary>Classifies a field whose value differs between the two maps into a finding, or none
    /// when the difference is a faithful transformation (a trigger string inlined to the same text).</summary>
    private static void Classify(
        string key, string srcVal, string tgtVal,
        HashSet<string> srcCustomIds, HashSet<string> tgtCustomIds,
        MapStrings srcStrings, MapStrings tgtStrings,
        string rawcode, ObjectKind kind, string? name, List<FidelityFinding> findings)
    {
        if (string.Equals(srcVal, tgtVal, StringComparison.Ordinal)) return;

        void Add(FidelityIssue issue, FidelitySeverity sev, string detail, string? sv = null, string? tv = null)
            => findings.Add(new FidelityFinding(rawcode, rawcode, kind, name, issue, sev, key,
                sv ?? srcVal, tv ?? tgtVal, detail));

        // Names and tooltips are TRIGSTR_ references in the source and inlined literals after porting,
        // so compare the resolved text through each map's own string table. Equal text is faithful.
        if (srcVal.Contains("TRIGSTR_", StringComparison.Ordinal) || tgtVal.Contains("TRIGSTR_", StringComparison.Ordinal))
        {
            var sr = srcStrings.Resolve(srcVal.Trim());
            var tr = tgtStrings.Resolve(tgtVal.Trim());
            if (string.Equals(sr, tr, StringComparison.Ordinal)) return;
            Add(FidelityIssue.ValueDiff, FidelitySeverity.Info,
                "text differs after resolving trigger strings", Trunc(sr), Trunc(tr));
            return;
        }

        // A field whose tokens are all custom-object rawcodes in each map, differing because those
        // objects were remapped on collision. We cannot prove from the maps alone that it is not a
        // corruption, so per the conservative rule it is information, not a counted loss.
        if (AllTokensIn(srcVal, srcCustomIds) && AllTokensIn(tgtVal, tgtCustomIds))
        {
            Add(FidelityIssue.ReferenceValueDiff, FidelitySeverity.Info,
                "rawcode reference differs, likely a legitimate remap on collision");
            return;
        }

        // A numeric change is the one that actually weakens a hero, damage, cooldown, chance, duration.
        if (AllNumeric(srcVal) && AllNumeric(tgtVal))
        {
            Add(FidelityIssue.NumericValueDiff, FidelitySeverity.Error, "numeric value differs");
            return;
        }

        // Anything else, a flag, an art path, a data token. Shown for transparency, not a counted loss,
        // because without game data we cannot tell a meaningful change from a cosmetic one here.
        Add(FidelityIssue.ValueDiff, FidelitySeverity.Info, "value differs");
    }

    // ---- indexing and small predicates ---------------------------------------

    private static Dictionary<ObjectKind, Dictionary<int, MapObjectEntry>> IndexByKind(MapDocument doc)
    {
        var index = new Dictionary<ObjectKind, Dictionary<int, MapObjectEntry>>();
        foreach (var kind in ObjectKinds.All)
        {
            var byId = new Dictionary<int, MapObjectEntry>();
            foreach (var e in ObjectKinds.MergedEntries(doc, ObjectKinds.Info(kind)))
                byId[e.Id] = e; // MergedEntries already unions the map and skin layers per id
            index[kind] = byId;
        }
        return index;
    }

    private static HashSet<string> AllIds(Dictionary<ObjectKind, Dictionary<int, MapObjectEntry>> index) =>
        index.Values.SelectMany(m => m.Keys).Select(id => id.ToRawcode()).ToHashSet(StringComparer.Ordinal);

    private static ObjectKind? KindOf(Dictionary<ObjectKind, Dictionary<int, MapObjectEntry>> index, string rawcode)
    {
        if (rawcode.Length != 4) return null;
        int id = rawcode.FromRawcode();
        foreach (var kind in ObjectKinds.All)
            if (index[kind].ContainsKey(id)) return kind;
        return null;
    }

    private static bool AllTokensIn(string value, HashSet<string> ids)
    {
        var tokens = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return tokens.Length > 0 && tokens.All(ids.Contains);
    }

    private static bool AllNumeric(string value)
    {
        var tokens = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return tokens.Length > 0 && tokens.All(t =>
            double.TryParse(t.TrimEnd('.'), NumberStyles.Any, CultureInfo.InvariantCulture, out _));
    }

    /// <summary>The highest per-level or per-variation index ("code:N") set on the object, 0 if flat.</summary>
    private static int MaxLevel(Dictionary<string, string> dict)
    {
        int max = 0;
        foreach (var key in dict.Keys)
        {
            int c = key.IndexOf(':');
            if (c >= 0 && int.TryParse(key[(c + 1)..], out var n) && n > max) max = n;
        }
        return max;
    }

    private static string Trunc(string s) => s.Length <= 60 ? s : s[..57] + "...";
}
