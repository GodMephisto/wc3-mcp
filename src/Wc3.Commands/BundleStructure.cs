using System;
using System.Collections.Generic;
using System.Linq;

namespace Wc3.Commands;

/// <summary>
/// The one rule that separates a hero's real dependencies from the script closure
/// over-carry, shared by every front end so they cannot drift. An edge is a real object
/// reference when its Via is anything other than ScriptClosureVia. Objects the root reaches
/// only through that synthetic seed edge are carried, and belong in their own group, never
/// under the hero.
/// </summary>
public static class BundleStructure
{
    /// <summary>The Via that BundleCommand stamps on the synthetic root to object edge it adds
    /// for every object the trigger script drags in. Any other Via is a genuine field code.</summary>
    public const string ScriptClosureVia = "script closure";

    /// <summary>The Via on an object to display-string edge. Its To is prose, not a rawcode or a
    /// path, so the object and file rules must both skip it. Without that guard a four character
    /// string ("Bash", say) would forge an object edge purely by looking like a rawcode.</summary>
    public const string StringVia = "string";

    /// <summary>Real object to object references, the referencing rawcode to its distinct
    /// referenced rawcodes, dropping the script closure seed edges and any endpoint that is
    /// not an object in this bundle.</summary>
    public static Dictionary<string, List<string>> RealAdjacency(UnitBundle bundle)
    {
        var codes = bundle.Objects.Select(o => o.Rawcode).ToHashSet(StringComparer.Ordinal);
        return RealObjectEdges(bundle, codes)
            .GroupBy(e => e.From, StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => g.Select(e => e.To).Distinct().ToList(),
                StringComparer.Ordinal);
    }

    /// <summary>Real object to object edges grouped by the referencing rawcode, keeping each
    /// edge so a caller can show the field code it came through. Same rule as RealAdjacency.</summary>
    public static Dictionary<string, List<BundleEdge>> RealChildEdges(UnitBundle bundle)
    {
        var codes = bundle.Objects.Select(o => o.Rawcode).ToHashSet(StringComparer.Ordinal);
        return RealObjectEdges(bundle, codes)
            .GroupBy(e => e.From, StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.Ordinal);
    }

    private static IEnumerable<BundleEdge> RealObjectEdges(UnitBundle bundle, HashSet<string> codes) =>
        bundle.Edges.Where(e => e.Via != ScriptClosureVia && e.Via != StringVia
            && codes.Contains(e.From) && codes.Contains(e.To));

    /// <summary>The root plus every object it reaches through real field references. The
    /// complement of CarriedByScriptClosure, and the seed set for the file and string rules,
    /// because an asset is only genuinely the root's if a genuinely reachable object asks for it.</summary>
    public static HashSet<string> RealObjects(UnitBundle bundle)
    {
        var adjacency = RealAdjacency(bundle);
        var reachable = new HashSet<string>(StringComparer.Ordinal) { bundle.RootRawcode };
        var queue = new Queue<string>();
        queue.Enqueue(bundle.RootRawcode);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (!adjacency.TryGetValue(current, out var targets))
                continue;
            foreach (var to in targets)
                if (reachable.Add(to))
                    queue.Enqueue(to);
        }
        return reachable;
    }

    /// <summary>Rawcodes the root reaches only through the script closure seed edge, never
    /// through a real reference. These are the deliberate over-carry, other heroes' kits and
    /// the like, and are shown in their own group rather than under the hero.</summary>
    public static HashSet<string> CarriedByScriptClosure(UnitBundle bundle)
    {
        var reachable = RealObjects(bundle);
        return bundle.Objects
            .Where(o => o.Rawcode != bundle.RootRawcode && !reachable.Contains(o.Rawcode))
            .Select(o => o.Rawcode)
            .ToHashSet(StringComparer.Ordinal);
    }

    /// <summary>Files the root really needs, the assets its real objects point at plus the
    /// textures those models pull in. A hero resolves to hundreds of files because the closure
    /// carries every other hero's art, and every one of those arrives through a perfectly valid
    /// art field, so an edge walk that ignores WHICH object asked cannot tell them apart.</summary>
    public static HashSet<string> RealFiles(UnitBundle bundle)
    {
        var paths = bundle.Files.Select(f => f.Path).ToHashSet(StringComparer.Ordinal);
        return ReachedFrom(bundle, RealObjects(bundle), paths);
    }

    /// <summary>Display strings the root really needs, the ones on its real objects' fields.
    /// Same rule and same reason as RealFiles, a foreign hero's tooltips are not the root's.</summary>
    public static HashSet<string> RealStrings(UnitBundle bundle)
    {
        var strings = bundle.Strings.ToHashSet(StringComparer.Ordinal);
        var real = RealObjects(bundle);
        return bundle.Edges
            .Where(e => e.Via == StringVia && real.Contains(e.From) && strings.Contains(e.To))
            .Select(e => e.To)
            .ToHashSet(StringComparer.Ordinal);
    }

    /// <summary>Walks from a seed set to every target in <paramref name="targets"/>, continuing
    /// THROUGH the targets themselves so a model's textures come along (BundleCommand records
    /// those as model path to texture path edges, not as object to texture).</summary>
    private static HashSet<string> ReachedFrom(
        UnitBundle bundle, HashSet<string> seeds, HashSet<string> targets)
    {
        var adjacency = bundle.Edges
            .Where(e => e.Via != StringVia && targets.Contains(e.To))
            .GroupBy(e => e.From, StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => g.Select(e => e.To).Distinct().ToList(),
                StringComparer.Ordinal);
        var reached = new HashSet<string>(StringComparer.Ordinal);
        var queue = new Queue<string>(seeds);
        while (queue.Count > 0)
        {
            if (!adjacency.TryGetValue(queue.Dequeue(), out var hits))
                continue;
            foreach (var to in hits)
                if (reached.Add(to))
                    queue.Enqueue(to);
        }
        return reached;
    }

    /// <summary>
    /// What a bundle is made of, before anybody reads nine thousand edges one at a time.
    /// </summary>
    /// <remarks>
    /// Measured on GGGA_V0.05 hero H005, resolving one hero produces 9,145 edges and 5,478 lines
    /// of human output, made of 2,168 object-field edges (the kit), 4,334 assets, 2,131 display
    /// strings and 512 script closure. A reader wanting to know what that hero depends on has to
    /// find two thousand relevant rows inside nine thousand. BundleSummaryTests re-measures the
    /// split against that map and prints it, so a drift is visible rather than left to a comment
    /// nobody re-checks.
    ///
    /// The truncation matters more than the volume. The closure carries at most 512 objects and
    /// the overflow is reported in a note printed AFTER everything else, so a bundle can be
    /// silently incomplete while looking exhaustive. That belongs at the top, not the bottom.
    /// </remarks>
    public sealed record BundleSummary(
        int Objects,
        int Files,
        int Strings,
        int Functions,
        int Edges,
        /// <summary>Edges through a real object field code, which is the kit itself.</summary>
        int ObjectFieldEdges,
        /// <summary>Edges to a model, texture, icon or sound.</summary>
        int AssetEdges,
        /// <summary>Edges to a display string.</summary>
        int StringEdges,
        /// <summary>Edges the script closure added. Carried, and not the kit.</summary>
        int ScriptClosureEdges,
        /// <summary>True when a diagnostic says a cap was hit, so the bundle is INCOMPLETE.</summary>
        bool Truncated,
        IReadOnlyList<string> Notes);

    /// <summary>The sentence every front end leads with when a carry cap truncated the closure.
    /// One constant rather than one phrasing per front end, because the whole point is that a
    /// reader recognises it wherever it appears.</summary>
    public const string TruncatedWarning =
        "INCOMPLETE, a carry cap was reached so some dependencies are missing.";

    /// <summary>One line naming what the edges actually are, so a nine thousand edge total stops
    /// reading as a dependency count.</summary>
    public static string DescribeEdges(BundleSummary s) =>
        $"{s.Edges} edge(s), {s.ObjectFieldEdges} object-field (the kit), "
        + $"{s.AssetEdges} asset, {s.StringEdges} string, {s.ScriptClosureEdges} script-closure";

    /// <summary>Summarises a resolved bundle. Never throws, because a summary is a reading aid.</summary>
    public static BundleSummary Summarize(UnitBundle bundle)
    {
        var files = bundle.Files.Select(f => f.Path).ToHashSet(StringComparer.OrdinalIgnoreCase);

        int closure = 0, strings = 0, assets = 0, fields = 0;
        foreach (var e in bundle.Edges)
        {
            if (e.Via == ScriptClosureVia) closure++;
            else if (e.Via == StringVia) strings++;
            else if (files.Contains(e.To)) assets++;
            else fields++;
        }

        // A cap is reported in prose by the resolver, so this looks for that rather than
        // re-deriving a limit which would then have two places to drift apart.
        bool truncated = bundle.Diagnostics.Any(d =>
            d.Contains("cap", StringComparison.OrdinalIgnoreCase)
            && d.Contains("not carried", StringComparison.OrdinalIgnoreCase));

        return new BundleSummary(
            bundle.Objects.Count, bundle.Files.Count, bundle.Strings.Count,
            bundle.Functions.Count, bundle.Edges.Count,
            fields, assets, strings, closure,
            truncated, bundle.Diagnostics);
    }
}
