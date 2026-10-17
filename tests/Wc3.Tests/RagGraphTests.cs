// tests/Wc3.Tests/RagGraphTests.cs
// The corpus graph, built once from the documents and used by the MCP graph tools. No model and no
// network, so it is a pure unit test. A module initializer points the RAG store at a fresh temp dir
// before any test runs, so the context tools in the MCP tests run with no index and no graph and the
// suite is deterministic rather than dependent on whatever a machine already has installed.
using System.Runtime.CompilerServices;
using Wc3.Commands.Rag;

namespace Wc3.Tests;

internal static class RagTestEnv
{
    [ModuleInitializer]
    internal static void Init() =>
        Environment.SetEnvironmentVariable(
            "W3C_RAG_HOME",
            Path.Combine(Path.GetTempPath(), "wc3-rag-test-" + Guid.NewGuid().ToString("N")));
}

public class RagGraphTests
{
    private static string Corpus()
    {
        string dir = Path.Combine(Path.GetTempPath(), "wc3-rag-corpus-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(dir, "a"));
        Directory.CreateDirectory(Path.Combine(dir, "b"));
        File.WriteAllText(Path.Combine(dir, "a", "one.md"),
            "# One\n\nSee [[Two]] for more.\n\n## Section A\n\ntext\n");
        File.WriteAllText(Path.Combine(dir, "b", "two.md"),
            "# Two\n\nbody\n\n## Section B\n\ntext\n");
        return dir;
    }

    [Fact]
    public void Build_uses_the_corpus_relative_id_and_resolves_a_link()
    {
        var g = RagGraph.Build(Corpus(), index: null);
        Assert.Contains(g.Nodes, n => n.Id == "doc:a/one");
        Assert.Contains(g.Nodes, n => n.Id == "doc:b/two");
        Assert.Contains(g.Edges, e => e is { From: "doc:a/one", Type: "LINKS_TO", To: "doc:b/two" });
        Assert.Contains(g.Edges, e => e is { From: "doc:a/one", Type: "HAS_SECTION" });
    }

    [Fact]
    public void Every_edge_points_at_a_real_node()
    {
        // The bug this pins. The index keyed chunks by bare file name while the graph keyed nodes by
        // relative path, so SIMILAR_TO edges named nodes that did not exist and were dropped.
        var g = RagGraph.Build(Corpus(), index: null);
        var ids = g.Nodes.Select(n => n.Id).ToHashSet(StringComparer.Ordinal);
        Assert.All(g.Edges, e => { Assert.Contains(e.From, ids); Assert.Contains(e.To, ids); });
    }

    [Fact]
    public void Save_then_load_round_trips_and_still_answers()
    {
        var g = RagGraph.Build(Corpus(), index: null);
        string path = Path.Combine(Path.GetTempPath(), "wc3-rag-graph-" + Guid.NewGuid().ToString("N") + ".bin");
        g.Save(path);
        var back = RagGraph.Load(path);
        Assert.NotNull(back);
        Assert.Equal(g.Nodes.Count, back!.Nodes.Count);
        Assert.Equal(g.Edges.Count, back.Edges.Count);
        Assert.NotEmpty(back.Neighbors("doc:a/one", 1));
        Assert.Equal(new[] { "doc:a/one", "doc:b/two" }, back.ShortestPath("doc:a/one", "doc:b/two"));
    }

    [Fact]
    public void Load_of_a_corrupt_file_degrades_to_null_rather_than_throwing()
    {
        string path = Path.Combine(Path.GetTempPath(), "wc3-rag-bad-" + Guid.NewGuid().ToString("N") + ".bin");
        File.WriteAllBytes(path, new byte[] { 1, 0, 0, 0, 0xFF, 0xFF, 0xFF, 0x7F });
        Assert.Null(RagGraph.Load(path));
    }
}
