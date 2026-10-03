// tests/Wc3.Tests/BundleFilterTests.cs
using Wc3.Commands;

namespace Wc3.Tests;

/// <summary>
/// Hermetic coverage for the exclusion filter, hand-built <see cref="UnitBundle"/> fixtures
/// (no synthetic map needed, the filter never touches map bytes) exercise the identity fast
/// path, the downstream cascade, the guard against sweeping away script-only nodes, the
/// dangling-reference warning, and the script function ownership cascade.
/// </summary>
public class BundleFilterTests
{
    private static BundleNode Obj(string rawcode, ObjectKind kind, string name) =>
        new(rawcode, kind, name, CustomToMap: true);

    [Fact]
    public void Excluding_nothing_is_identity()
    {
        var bundle = new UnitBundle(
            "H001", "Hero",
            new[] { Obj("H001", ObjectKind.Unit, "Hero") },
            Array.Empty<BundleFile>(), Array.Empty<string>(),
            new[] { new BundleEdge("H001", "H001", "self") }, // any content, never inspected on this path
            Array.Empty<string>(), Array.Empty<BundleFunction>());

        // An empty set, and a set containing only the (unexcludable) root, both leave
        // nothing to do, the very same bundle instance comes back either way.
        Assert.Same(bundle, BundleFilter.Apply(bundle, new HashSet<string>()));
        Assert.Same(bundle, BundleFilter.Apply(bundle, new HashSet<string> { "H001" }));
    }

    [Fact]
    public void Excluding_a_leaf_narrows_objects_and_edges_without_touching_others()
    {
        var bundle = new UnitBundle(
            "H001", "Hero",
            new[]
            {
                Obj("H001", ObjectKind.Unit, "Hero"),
                Obj("A0QO", ObjectKind.Ability, "Q"),
                Obj("A0QP", ObjectKind.Ability, "W"),
            },
            Array.Empty<BundleFile>(), Array.Empty<string>(),
            new[]
            {
                new BundleEdge("H001", "A0QO", "uabi"),
                new BundleEdge("H001", "A0QP", "uabi"),
            },
            Array.Empty<string>(), Array.Empty<BundleFunction>());

        var result = BundleFilter.Apply(bundle, new HashSet<string> { "A0QO" });

        Assert.Equal(new[] { "H001", "A0QP" }, result.Objects.Select(o => o.Rawcode));
        Assert.Equal(new[] { new BundleEdge("H001", "A0QP", "uabi") }, result.Edges);
    }

    [Fact]
    public void Cascade_drops_a_dependent_used_only_by_the_excluded_node_but_keeps_a_shared_one()
    {
        // A0QO -> B001 (only A0QO reaches it, drops with A0QO) and A0QO/A0QP -> B002
        // (A0QP still needs it, stays). Same pattern repeated for a file dependency.
        var bundle = new UnitBundle(
            "H001", "Hero",
            new[]
            {
                Obj("H001", ObjectKind.Unit, "Hero"),
                Obj("A0QO", ObjectKind.Ability, "Q"),
                Obj("A0QP", ObjectKind.Ability, "W"),
                Obj("B001", ObjectKind.Buff, "Q-only buff"),
                Obj("B002", ObjectKind.Buff, "shared buff"),
            },
            new[]
            {
                new BundleFile(@"war3mapImported\Qonly.mdx", "model", PresentInMap: true),
                new BundleFile(@"war3mapImported\shared.mdx", "model", PresentInMap: true),
            },
            Array.Empty<string>(),
            new[]
            {
                new BundleEdge("H001", "A0QO", "uabi"),
                new BundleEdge("H001", "A0QP", "uabi"),
                new BundleEdge("A0QO", "B001", "abuf:1"),
                new BundleEdge("A0QO", "B002", "abuf:1"),
                new BundleEdge("A0QP", "B002", "abuf:1"),
                new BundleEdge("A0QO", @"war3mapImported\Qonly.mdx", "aeat"),
                new BundleEdge("A0QO", @"war3mapImported\shared.mdx", "aeat"),
                new BundleEdge("A0QP", @"war3mapImported\shared.mdx", "aeat"),
            },
            Array.Empty<string>(), Array.Empty<BundleFunction>());

        var result = BundleFilter.Apply(bundle, new HashSet<string> { "A0QO" });

        Assert.Equal(new[] { "H001", "A0QP", "B002" }, result.Objects.Select(o => o.Rawcode));
        Assert.Equal(new[] { @"war3mapImported\shared.mdx" }, result.Files.Select(f => f.Path));
        // The cascade is disclosed, not silent.
        Assert.Contains(result.Diagnostics, d => d.Contains("excluded") && d.Contains("automatically"));
    }

    [Fact]
    public void A_node_reachable_only_through_a_script_edge_is_never_swept_by_cascade()
    {
        // e025 has no edge from any object or file, only from a FUNCTION NAME (a script
        // closure carry), so it must survive even though its ability gets excluded, the
        // cascade only ever removes a node whose every recorded referrer got excluded.
        var bundle = new UnitBundle(
            "H001", "Hero",
            new[]
            {
                Obj("H001", ObjectKind.Unit, "Hero"),
                Obj("A0QO", ObjectKind.Ability, "Q"),
                Obj("e025", ObjectKind.Buff, "shared effect dummy"),
            },
            Array.Empty<BundleFile>(), Array.Empty<string>(),
            new[]
            {
                new BundleEdge("H001", "A0QO", "uabi"),
                new BundleEdge("Trig_CastA0QO", "e025", "script closure"),
            },
            Array.Empty<string>(),
            new[] { new BundleFunction("Trig_CastA0QO", 1, 5, "references 'A0QO'") });

        var result = BundleFilter.Apply(bundle, new HashSet<string> { "A0QO" });

        Assert.Contains("e025", result.Objects.Select(o => o.Rawcode));
    }

    [Fact]
    public void Excluding_a_node_a_kept_object_still_references_warns_instead_of_rewriting()
    {
        var bundle = new UnitBundle(
            "H001", "Hero",
            new[]
            {
                Obj("H001", ObjectKind.Unit, "Hero"),
                Obj("A0QO", ObjectKind.Ability, "Q"),
            },
            Array.Empty<BundleFile>(), Array.Empty<string>(),
            new[] { new BundleEdge("H001", "A0QO", "uabi") },
            Array.Empty<string>(), Array.Empty<BundleFunction>());

        var result = BundleFilter.Apply(bundle, new HashSet<string> { "A0QO" });

        // The root can never be excluded, so its uabi field still names A0QO in the
        // source map, that dangling reference must be disclosed, not silently dropped.
        Assert.Equal(new[] { "H001" }, result.Objects.Select(o => o.Rawcode));
        Assert.Contains(result.Diagnostics, d => d.Contains("H001") && d.Contains("A0QO") && d.Contains("uabi"));
    }

    [Fact]
    public void Excluding_an_ability_drops_its_own_script_but_keeps_shared_and_hero_wide_functions()
    {
        var bundle = new UnitBundle(
            "H001", "Hero",
            new[]
            {
                Obj("H001", ObjectKind.Unit, "Hero"),
                Obj("A0QO", ObjectKind.Ability, "Q"),
                Obj("A0QP", ObjectKind.Ability, "W"),
            },
            Array.Empty<BundleFile>(), Array.Empty<string>(),
            new[]
            {
                new BundleEdge("H001", "A0QO", "uabi"),
                new BundleEdge("H001", "A0QP", "uabi"),
                new BundleEdge("Trig_CastA0QO", @"war3mapImported\q.mdx", "script"),
            },
            Array.Empty<string>(),
            new[]
            {
                new BundleFunction("Trig_CastA0QO", 1, 5, "references 'A0QO'"),
                new BundleFunction("Helper_A0QO", 6, 8, "called by Trig_CastA0QO"),
                new BundleFunction("Trig_CastA0QP", 9, 13, "references 'A0QP'"),
                new BundleFunction("Trig_Shared", 14, 18, "references 'A0QO', 'A0QP'"),
                new BundleFunction("Trig_HeroWide", 19, 22, "references 'H001'"),
            });

        var result = BundleFilter.Apply(bundle, new HashSet<string> { "A0QO" });

        Assert.Equal(
            new[] { "Trig_CastA0QP", "Trig_HeroWide", "Trig_Shared" },
            result.Functions.Select(f => f.Name).OrderBy(n => n, StringComparer.Ordinal));
        // The dropped function's own script-carried asset edge goes with it.
        Assert.DoesNotContain(result.Edges, e => e.From == "Trig_CastA0QO");
    }
}
