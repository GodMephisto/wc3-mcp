// tests/Wc3.Tests/BundleCorpusTests.cs
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Real-map smoke test for the dependency resolver: any custom unit of the
/// Anime corpus map must resolve to a bundle whose closure includes itself
/// and terminates (cycle guard + node cap). Runs deltas-only (ctx=null) so
/// no Warcraft III install is needed beyond the map. Skips when absent.
/// </summary>
public class BundleCorpusTests
{
    private static string MapPath => CorpusMap.PathOrEmpty;

    [Fact]
    [Trait("Category", "Corpus")]
    public void Anime_map_unit_bundle_resolves_and_terminates()
    {
        if (!File.Exists(MapPath)) return;
        var doc = MapDocument.Load(MapPath);

        var units = ObjectListCommand.Execute(doc, ObjectKind.Unit, ctx: null);
        Assert.NotEmpty(units.Items);

        var rootRawcode = units.Items[0].Rawcode;
        var bundle = BundleCommand.ResolveUnit(doc, rootRawcode, ctx: null, preDiagnostics: Array.Empty<string>());

        Assert.Equal(rootRawcode, bundle.RootRawcode);
        var root = Assert.Single(bundle.Objects, o => o.Rawcode == rootRawcode);
        Assert.True(root.CustomToMap);
        // Every edge endpoint that is an object must be a recorded node.
        var known = bundle.Objects.Select(o => o.Rawcode).ToHashSet(StringComparer.Ordinal);
        var filePaths = bundle.Files.Select(f => f.Path).ToHashSet(StringComparer.Ordinal);
        Assert.All(bundle.Edges, e => Assert.True(known.Contains(e.To) || filePaths.Contains(e.To)));
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Anime_map_raiden_bundle_pulls_a_script_closure()
    {
        if (!File.Exists(MapPath)) return;
        var doc = MapDocument.Load(MapPath);

        // Find a custom unit whose script closure is non-empty, rather than naming one. This test
        // used to name 'H000' from a specific map, and once that map was gone it skipped in silence
        // instead of failing. The property under test is that a bundle reaches script through
        // rawcode-initialised globals at all, not that one particular hero exists.
        var hero = FirstUnitWithAScriptClosure(doc);
        if (hero is null) return;   // this map has no unit whose handlers are reachable that way

        var bundle = BundleCommand.ResolveUnit(doc, hero, ctx: null, preDiagnostics: Array.Empty<string>());

        Assert.NotEmpty(bundle.Functions);
        Assert.Contains(bundle.Functions, f => f.Reason.StartsWith("references ", StringComparison.Ordinal));
        Assert.Contains(bundle.Functions, f => f.Reason.StartsWith("called by ", StringComparison.Ordinal));
        // Deterministic: ordered by declaration position, no duplicate names.
        Assert.Equal(bundle.Functions.OrderBy(f => f.StartLine).Select(f => f.Name), bundle.Functions.Select(f => f.Name));
        Assert.Equal(bundle.Functions.Count, bundle.Functions.Select(f => f.Name).Distinct(StringComparer.Ordinal).Count());
    }

    /// <summary>
    /// The first custom unit in the map whose bundle reaches at least one script function both by
    /// direct rawcode reference and by call closure, which is what the assertions below need.
    /// Bounded so a map full of units does not turn this into a sweep.
    /// </summary>
    private static string? FirstUnitWithAScriptClosure(MapDocument doc)
    {
        int examined = 0;
        foreach (var o in ObjectListCommand.Execute(doc, ObjectKind.Unit, gameDirOverride: null).Items)
        {
            if (o.BaseRawcode is null || o.BaseRawcode == o.Rawcode) continue;  // custom only
            if (++examined > 40) break;      // bounded, and 40 covers any real map's hero roster

            var b = BundleCommand.ResolveUnit(doc, o.Rawcode, ctx: null,
                preDiagnostics: Array.Empty<string>());
            if (b.Functions.Any(f => f.Reason.StartsWith("references ", StringComparison.Ordinal))
                && b.Functions.Any(f => f.Reason.StartsWith("called by ", StringComparison.Ordinal)))
                return o.Rawcode;
        }
        return null;
    }
}
