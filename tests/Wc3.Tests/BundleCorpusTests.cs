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
    private static readonly string MapPath =
        TestCorpus.Map(@"Anime_WOS2_0.25c1.w3x");

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

        // H000 "Raiden Ei" — her spell handlers live in war3map.j behind
        // rawcode-initialized globals (integer RaidenQ_ID= 'A000' ...).
        var bundle = BundleCommand.ResolveUnit(doc, "H000", ctx: null, preDiagnostics: Array.Empty<string>());

        Assert.NotEmpty(bundle.Functions);
        Assert.Contains(bundle.Functions, f => f.Reason.StartsWith("references ", StringComparison.Ordinal));
        Assert.Contains(bundle.Functions, f => f.Reason.StartsWith("called by ", StringComparison.Ordinal));
        // Deterministic: ordered by declaration position, no duplicate names.
        Assert.Equal(bundle.Functions.OrderBy(f => f.StartLine).Select(f => f.Name), bundle.Functions.Select(f => f.Name));
        Assert.Equal(bundle.Functions.Count, bundle.Functions.Select(f => f.Name).Distinct(StringComparer.Ordinal).Count());
    }
}
