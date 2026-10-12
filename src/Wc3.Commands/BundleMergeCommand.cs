// src/Wc3.Commands/BundleMergeCommand.cs
namespace Wc3.Commands;

/// <summary>One rawcode both bundles define, but differently. A merge refuses rather than
/// guess which definition the caller meant.</summary>
public sealed record BundleConflict(string Rawcode, string Kind, string Detail);

/// <summary>
/// The outcome of merging two bundles. Ok is false (and Merged is null) when a rawcode is
/// defined differently in the two, so a caller refuses rather than ships a silent choice.
/// </summary>
public sealed record BundleMergeResult(
    bool Ok,
    string Message,
    UnitBundle? Merged,
    IReadOnlyList<BundleConflict> Conflicts);

/// <summary>
/// Merges two dependency bundles into one. Objects and assets are unioned, and a rawcode both
/// bundles define must agree (same kind, same custom flag, and no contradicting display name),
/// otherwise the merge refuses. Two bundles that share a rawcode with identical definitions are
/// the common case (the same ability two heroes both use) and merge cleanly.
/// </summary>
public static class BundleMergeCommand
{
    public static BundleMergeResult Execute(UnitBundle a, UnitBundle b)
    {
        var conflicts = new List<BundleConflict>();

        var byCodeA = a.Objects.ToDictionary(o => o.Rawcode, o => o, StringComparer.Ordinal);
        var byCodeB = b.Objects.ToDictionary(o => o.Rawcode, o => o, StringComparer.Ordinal);

        foreach (var kv in byCodeB)
        {
            if (!byCodeA.TryGetValue(kv.Key, out var left)) continue;
            var right = kv.Value;
            if (left.Kind != right.Kind)
            {
                conflicts.Add(new BundleConflict(kv.Key,
                    right.Kind.ToString().ToLowerInvariant(),
                    $"defined as {left.Kind.ToString().ToLowerInvariant()} in the first bundle "
                    + $"and {right.Kind.ToString().ToLowerInvariant()} in the second"));
            }
            else if (left.CustomToMap != right.CustomToMap)
            {
                conflicts.Add(new BundleConflict(kv.Key,
                    right.Kind.ToString().ToLowerInvariant(),
                    "one bundle defines it as a custom object and the other treats it as base-game"));
            }
            else if (left.Name is not null && right.Name is not null
                && !string.Equals(left.Name, right.Name, StringComparison.Ordinal))
            {
                conflicts.Add(new BundleConflict(kv.Key,
                    right.Kind.ToString().ToLowerInvariant(),
                    $"named \"{left.Name}\" in the first bundle and \"{right.Name}\" in the second"));
            }
        }

        if (conflicts.Count > 0)
        {
            var ordered = conflicts.OrderBy(c => c.Rawcode, StringComparer.Ordinal).ToList();
            return new BundleMergeResult(false,
                $"refused, {ordered.Count} rawcode(s) are defined differently in the two bundles: "
                + string.Join(", ", ordered.Select(c => c.Rawcode)),
                null, ordered);
        }

        var objects = byCodeA.Values
            .Concat(byCodeB.Values.Where(o => !byCodeA.ContainsKey(o.Rawcode)))
            .OrderBy(o => o.Kind).ThenBy(o => o.Rawcode, StringComparer.Ordinal)
            .ToList();

        var files = MergeFiles(a.Files, b.Files);
        var strings = a.Strings.Concat(b.Strings).Distinct(StringComparer.Ordinal)
            .OrderBy(s => s, StringComparer.Ordinal).ToList();

        var edges = a.Edges.Concat(b.Edges)
            .GroupBy(e => (e.From, e.To, e.Via))
            .Select(g => g.First())
            .OrderBy(e => e.From, StringComparer.Ordinal)
            .ThenBy(e => e.To, StringComparer.Ordinal)
            .ThenBy(e => e.Via, StringComparer.Ordinal)
            .ToList();

        var functions = a.Functions.Concat(b.Functions)
            .GroupBy(f => f.Name, StringComparer.Ordinal)
            .Select(g => g.First())
            .OrderBy(f => f.StartLine).ThenBy(f => f.Name, StringComparer.Ordinal)
            .ToList();

        var diagnostics = a.Diagnostics
            .Concat(b.Diagnostics)
            .Append($"merged {a.RootRawcode} ({a.Objects.Count} object(s)) and {b.RootRawcode} "
                + $"({b.Objects.Count} object(s))")
            .ToList();

        var merged = new UnitBundle(
            a.RootRawcode, a.RootName ?? b.RootName,
            objects, files, strings, edges, diagnostics, functions);

        return new BundleMergeResult(true,
            $"merged into {merged.Objects.Count} object(s), {merged.Files.Count} asset(s), "
            + $"{merged.Strings.Count} string(s), {merged.Functions.Count} function(s)",
            merged, Array.Empty<BundleConflict>());
    }

    /// <summary>Union of two asset lists by path. A path both hold keeps the first bundle's row
    /// unless the first marked it absent and the second marks it present, which is the more
    /// useful fact (the asset is in a map somewhere).</summary>
    private static IReadOnlyList<BundleFile> MergeFiles(
        IReadOnlyList<BundleFile> a, IReadOnlyList<BundleFile> b)
    {
        var byPath = new Dictionary<string, BundleFile>(StringComparer.OrdinalIgnoreCase);
        foreach (var f in a) byPath[f.Path] = f;
        foreach (var f in b)
        {
            if (byPath.TryGetValue(f.Path, out var existing) && !existing.PresentInMap && f.PresentInMap)
                byPath[f.Path] = existing with { PresentInMap = true };
            else if (!byPath.ContainsKey(f.Path))
                byPath[f.Path] = f;
        }
        return byPath.Values.OrderBy(f => f.Path, StringComparer.Ordinal).ToList();
    }
}
