// src/Wc3.Commands/BundleFilter.cs
using System.Text.RegularExpressions;

namespace Wc3.Commands;

/// <summary>
/// Narrows a <see cref="UnitBundle"/> to what a caller still wants ported, given a set of
/// rawcodes and/or file paths the user excluded (the Studio dependency graph's per-node
/// toggle). Pure and side-effect free, the source bundle is never mutated, so it can sit
/// ahead of <see cref="PortCommand.PortUnit"/> without either of them knowing about the
/// other.
///
/// Two different problems get two different answers here, both explained where they are
/// decided below. An excluded object's OWN dependents that nothing else in the port would
/// reach are dropped automatically (a fixpoint over the object/file edge graph), that
/// mirrors what would happen had the excluded node simply never existed and needs no
/// warning. A still-kept object whose OWN data keeps naming the excluded rawcode (the
/// hero's ability list still lists the ability the user unticked) is left exactly as is,
/// not rewritten, and reported as a warning instead. Automatically stripping a source
/// object's field values is exactly the kind of automatic closure surgery this project's
/// porting work has already tried and rejected three times, each attempt quietly broke a
/// working ability (see BundleCommand's seeding comment), so the safer choice is to leave
/// the reference literal and let the user decide by hand whether to also port the excluded
/// object, exclude the referencing one too, or accept the dangling id.
/// </summary>
public static class BundleFilter
{
    private static readonly Regex QuotedRawcode = new(@"'([^']{4})'", RegexOptions.Compiled);
    private const string CalledByPrefix = "called by ";

    /// <summary>
    /// Returns a bundle with every rawcode/path in <paramref name="excludedKeys"/> removed,
    /// its own dependents cascaded out when nothing else in the kept set still needs them,
    /// and its script functions dropped when every ability they are attributed to was
    /// excluded. The root can never be excluded, even if it appears in
    /// <paramref name="excludedKeys"/>, it is the object being ported. Excluding nothing
    /// (after removing a stray root entry) returns <paramref name="bundle"/> itself
    /// unchanged, so callers never pay for a filter pass that would not change anything.
    /// </summary>
    public static UnitBundle Apply(UnitBundle bundle, IReadOnlySet<string> excludedKeys)
    {
        var excluded = new HashSet<string>(StringComparer.Ordinal);
        foreach (var key in excludedKeys)
            if (key != bundle.RootRawcode)
                excluded.Add(key);
        if (excluded.Count == 0)
            return bundle;

        var objByCode = new Dictionary<string, BundleNode>(StringComparer.Ordinal);
        foreach (var o in bundle.Objects) objByCode[o.Rawcode] = o;
        var fileByPath = new Dictionary<string, BundleFile>(StringComparer.Ordinal);
        foreach (var f in bundle.Files) fileByPath[f.Path] = f;
        bool IsCandidate(string key) => objByCode.ContainsKey(key) || fileByPath.ContainsKey(key);

        // Baseline in-degree, edges between two candidate nodes only. A script edge (its
        // From is a function name, never a rawcode or file path this bundle tracks as a
        // node) does not count, so a trigger-carried asset that has no OTHER referrer stays
        // untouched by the cascade below rather than vanishing the moment anything at all
        // gets excluded.
        var incoming = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        foreach (var e in bundle.Edges)
        {
            if (!IsCandidate(e.From) || !IsCandidate(e.To)) continue;
            if (!incoming.TryGetValue(e.To, out var list)) incoming[e.To] = list = new();
            list.Add(e.From);
        }

        // Downstream cascade to a fixpoint. A candidate node with at least one recorded
        // referrer joins the excluded set once EVERY one of its original referrers is
        // excluded (directly or by an earlier round of this same cascade). A node with zero
        // recorded referrers (the root, or something the script pass carried directly) is
        // never swept in by this loop, only a direct exclusion removes it.
        int directCount = excluded.Count;
        bool changed = true;
        int guard = objByCode.Count + fileByPath.Count + 1;
        while (changed && guard-- > 0)
        {
            changed = false;
            foreach (var key in objByCode.Keys.Concat(fileByPath.Keys))
            {
                if (key == bundle.RootRawcode || excluded.Contains(key)) continue;
                if (incoming.TryGetValue(key, out var from) && from.Count > 0 && from.All(excluded.Contains))
                {
                    excluded.Add(key);
                    changed = true;
                }
            }
        }
        int cascadedCount = excluded.Count - directCount;

        var keptObjects = bundle.Objects.Where(o => !excluded.Contains(o.Rawcode)).ToList();
        var keptFiles = bundle.Files.Where(f => !excluded.Contains(f.Path)).ToList();
        var keptFunctions = FilterFunctions(bundle.Functions, excluded);
        var keptFunctionNames = keptFunctions.Select(f => f.Name).ToHashSet(StringComparer.Ordinal);

        bool FromSurvives(string from) =>
            IsCandidate(from) ? !excluded.Contains(from) : keptFunctionNames.Contains(from);
        var keptEdges = bundle.Edges
            .Where(e => FromSurvives(e.From) && !excluded.Contains(e.To))
            .ToList();

        var diagnostics = new List<string>(bundle.Diagnostics);
        int droppedObjects = bundle.Objects.Count - keptObjects.Count;
        int droppedFiles = bundle.Files.Count - keptFiles.Count;
        int droppedFunctions = bundle.Functions.Count - keptFunctions.Count;
        if (droppedObjects > 0 || droppedFiles > 0)
            diagnostics.Add($"excluded {droppedObjects} object(s) and {droppedFiles} file(s) by selection"
                + (cascadedCount > 0 ? $", {cascadedCount} of those automatically (nothing else in the "
                    + "kept selection still referenced them)" : ""));
        if (droppedFunctions > 0)
            diagnostics.Add($"excluded {droppedFunctions} script function(s) attributable only to "
                + "excluded object(s)");

        // Dangling-reference warning, a KEPT node whose own data still names an EXCLUDED
        // one. Grouped per (referrer, excluded target) so a field repeated across several
        // ability levels prints once, not once per level.
        foreach (var g in bundle.Edges
                     .Where(e => IsCandidate(e.From) && !excluded.Contains(e.From) && excluded.Contains(e.To))
                     .GroupBy(e => (e.From, e.To)))
        {
            var vias = string.Join(", ", g.Select(e => e.Via).Distinct());
            var targetLabel = objByCode.TryGetValue(g.Key.To, out var node)
                ? $"{g.Key.To} \"{node.Name ?? node.Kind.ToString()}\""
                : g.Key.To;
            diagnostics.Add($"{g.Key.From} still references excluded {targetLabel} via {vias}, "
                + "that field was not rewritten, the ported object will name an id that was not carried "
                + "unless it is already present in the target or ported separately");
        }

        return new UnitBundle(
            bundle.RootRawcode, bundle.RootName, keptObjects, keptFiles, bundle.Strings,
            keptEdges, diagnostics, keptFunctions);
    }

    /// <summary>
    /// Drops a script function once every rawcode it (or its call chain) is attributed to
    /// has been excluded. Attribution follows <see cref="BundleFunction.Reason"/>, which
    /// BundleCommand's BFS assigns exactly once per function, so it is a clean forest, a
    /// seed function ("references '...'") owns whatever rawcodes its own reason quotes, and
    /// a discovered one ("called by Name") simply inherits its caller's owning set. A
    /// function with no resolvable owner (a caller outside this bundle, or a defensive cycle
    /// guard) is never dropped, this project's standing rule is to over-carry a script
    /// closure rather than guess at a narrower one.
    /// </summary>
    private static IReadOnlyList<BundleFunction> FilterFunctions(
        IReadOnlyList<BundleFunction> functions, HashSet<string> excluded)
    {
        if (functions.Count == 0) return functions;

        var byName = new Dictionary<string, BundleFunction>(StringComparer.Ordinal);
        foreach (var f in functions) byName[f.Name] = f;

        var seedRawcodes = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
        var parent = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var f in functions)
        {
            if (f.Reason.StartsWith(CalledByPrefix, StringComparison.Ordinal))
            {
                parent[f.Name] = f.Reason[CalledByPrefix.Length..];
            }
            else
            {
                var set = new HashSet<string>(StringComparer.Ordinal);
                foreach (Match m in QuotedRawcode.Matches(f.Reason)) set.Add(m.Groups[1].Value);
                seedRawcodes[f.Name] = set;
            }
        }

        var owners = new Dictionary<string, HashSet<string>?>(StringComparer.Ordinal);
        HashSet<string>? Resolve(string name, HashSet<string> visiting)
        {
            if (owners.TryGetValue(name, out var cached)) return cached;
            if (seedRawcodes.TryGetValue(name, out var own)) return owners[name] = own;
            if (!parent.TryGetValue(name, out var callerName)
                || !byName.ContainsKey(callerName)
                || !visiting.Add(name))
            {
                return owners[name] = null; // no recorded parent, a foreign caller, or a cycle
            }
            var result = Resolve(callerName, visiting);
            visiting.Remove(name);
            return owners[name] = result;
        }

        var kept = new List<BundleFunction>(functions.Count);
        foreach (var f in functions)
        {
            var owner = Resolve(f.Name, new HashSet<string>(StringComparer.Ordinal));
            bool drop = owner is { Count: > 0 } && owner.All(excluded.Contains);
            if (!drop) kept.Add(f);
        }
        return kept;
    }
}
