// tests/Wc3.Tests/BundleSummaryTests.cs
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// A dependency bundle has to say what it is before it says what is in it.
///
/// Resolving one hero on GGGA_V0.05 produces 9,145 edges over 5,478 lines. Roughly a quarter
/// are the kit and the rest are assets, display strings and script closure, so a reader looking
/// for what the hero depends on is searching for a thousand rows inside nine thousand.
///
/// Worse, the closure carries at most 512 objects and the overflow was reported in a note
/// printed AFTER everything else. A bundle could be silently incomplete and read as exhaustive,
/// which is the failure this repo keeps paying for. Truncation is now a field rather than
/// prose, and both front-ends lead with it.
/// </summary>
public class BundleSummaryTests
{
    private readonly ITestOutputHelper _out;
    public BundleSummaryTests(ITestOutputHelper output) => _out = output;

    private static UnitBundle Bundle(
        IEnumerable<BundleEdge> edges,
        IEnumerable<BundleFile>? files = null,
        IEnumerable<string>? diagnostics = null) =>
        new("H000", "Root",
            Array.Empty<BundleNode>(),
            (files ?? Array.Empty<BundleFile>()).ToList(),
            Array.Empty<string>(),
            edges.ToList(),
            (diagnostics ?? Array.Empty<string>()).ToList(),
            Array.Empty<BundleFunction>());

    [Fact]
    public void Each_edge_lands_in_exactly_one_bucket()
    {
        var files = new[] { new BundleFile("war3mapImported\\hero.mdx", "model", true) };
        var b = Bundle(new[]
        {
            new BundleEdge("H000", "A001", "uabi"),                          // kit
            new BundleEdge("H000", "A002", "uabi"),                          // kit
            new BundleEdge("H000", "war3mapImported\\hero.mdx", "umdl"),     // asset
            new BundleEdge("H000", "Arthas Menethil", BundleStructure.StringVia),
            new BundleEdge("H000", "X999", BundleStructure.ScriptClosureVia),
        }, files);

        var s = BundleStructure.Summarize(b);

        Assert.Equal(5, s.Edges);
        Assert.Equal(2, s.ObjectFieldEdges);
        Assert.Equal(1, s.AssetEdges);
        Assert.Equal(1, s.StringEdges);
        Assert.Equal(1, s.ScriptClosureEdges);

        // The buckets must partition the edges. If they ever overlap or leak, a total that
        // looks right will be hiding a miscount.
        Assert.Equal(s.Edges,
            s.ObjectFieldEdges + s.AssetEdges + s.StringEdges + s.ScriptClosureEdges);
    }

    [Fact]
    public void A_carry_cap_diagnostic_marks_the_bundle_incomplete()
    {
        var b = Bundle(Array.Empty<BundleEdge>(), diagnostics: new[]
        {
            "note: carried 512 object(s) a handler spawns or grants at runtime",
            "note: script-spawned object cap (512) reached, 295 more spawned/granted object(s) not carried",
        });

        Assert.True(BundleStructure.Summarize(b).Truncated);
    }

    [Fact]
    public void Ordinary_notes_do_not_mark_a_bundle_incomplete()
    {
        // The over-carry note mentions a number and a cap-like word without being a cap. If
        // this ever flips, every bundle reports INCOMPLETE and the warning stops meaning
        // anything, which is worse than not having it.
        var b = Bundle(Array.Empty<BundleEdge>(), diagnostics: new[]
        {
            "note: carried 512 object(s) a handler spawns or grants at runtime; some may belong "
            + "to another hero's kit (over-carry, safe to prune)",
            "note: skipped 243 asset path(s) in 2 shared asset bank function(s)",
        });

        Assert.False(BundleStructure.Summarize(b).Truncated);
    }

    [Fact]
    public void Both_front_ends_describe_the_edges_with_one_shared_sentence()
    {
        // The CLI and the Studio each rendered their own wording for this. Two phrasings for one
        // fact is how a reader learns to distrust both, so the sentence lives in one place.
        var s = BundleStructure.Summarize(Bundle(new[]
        {
            new BundleEdge("H000", "A001", "uabi"),
            new BundleEdge("H000", "X999", BundleStructure.ScriptClosureVia),
        }));

        string line = BundleStructure.DescribeEdges(s);
        Assert.Contains("2 edge(s)", line);
        Assert.Contains("1 object-field (the kit)", line);
        Assert.Contains("1 script-closure", line);
        Assert.DoesNotContain(":", line);   // the project's punctuation rule, UI text included
    }

    [Fact]
    public void The_incomplete_warning_leads_with_the_word_that_matters()
    {
        // A reader scanning a wall of diagnostics has to catch this one. It starts with the word.
        Assert.StartsWith("INCOMPLETE", BundleStructure.TruncatedWarning);
        Assert.DoesNotContain(":", BundleStructure.TruncatedWarning);
    }

    [Fact]
    public void An_empty_bundle_summarises_to_zeroes_rather_than_throwing()
    {
        var s = BundleStructure.Summarize(Bundle(Array.Empty<BundleEdge>()));
        Assert.Equal(0, s.Edges);
        Assert.False(s.Truncated);
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void The_real_bundle_is_mostly_not_the_kit()
    {
        string path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Warcraft III", "Maps", "Download", "GGGA_V0.05.w3x");
        if (!File.Exists(path)) { _out.WriteLine("map not on disk"); return; }

        var bundle = BundleCommand.ResolveUnit(MapDocument.Load(path), "H005", null);
        var s = BundleStructure.Summarize(bundle);

        _out.WriteLine($"{s.Edges} edges: {s.ObjectFieldEdges} kit, {s.AssetEdges} asset, "
                     + $"{s.StringEdges} string, {s.ScriptClosureEdges} closure. "
                     + $"truncated={s.Truncated}");

        Assert.Equal(s.Edges,
            s.ObjectFieldEdges + s.AssetEdges + s.StringEdges + s.ScriptClosureEdges);
        Assert.True(s.Edges > 1000, "this map's hero bundles are large, that is the point");
        // The kit is the minority of what a bundle shows. If this ever inverts, the summary is
        // solving a problem the resolver no longer has and the panel should say so differently.
        Assert.True(s.ObjectFieldEdges < s.Edges / 2,
            $"expected the kit to be a minority, got {s.ObjectFieldEdges} of {s.Edges}");
    }
}
