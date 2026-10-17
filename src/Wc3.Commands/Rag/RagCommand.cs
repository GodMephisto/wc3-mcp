// src/Wc3.Commands/Rag/RagCommand.cs
namespace Wc3.Commands.Rag;

/// <summary>
/// The `rag` command. `install` fetches the model and builds the index once, then the MCP
/// `docs_search` uses that index automatically, so a client gets semantic retrieval with no
/// configuration. `status` reports what is present, `search` queries the index directly.
/// </summary>
public static class RagCommand
{
    public sealed record RagStatus(
        string Root, bool ModelPresent, long ModelBytes, bool IndexPresent, long IndexBytes,
        int Chunks, string Corpus, string? BuiltUtc, string ModelRepo);

    public sealed record RagInstallResult(
        string Root, string Corpus, int Chunks, string IndexFile, long IndexBytes, string ModelRepo);

    public sealed record RagHit(string Document, string Section, float Score, string Passage);

    public sealed record RagSearchResult(string Query, int Matches, IReadOnlyList<RagHit> Results);

    /// <summary>The bundled documents, beside the exe. The default corpus.</summary>
    public static string BundledContextDir() => Path.Combine(AppContext.BaseDirectory, "context");

    public static RagStatus Status()
    {
        bool model = File.Exists(RagPaths.ModelOnnx) && File.Exists(RagPaths.VocabTxt);
        long modelBytes = model ? new FileInfo(RagPaths.ModelOnnx).Length + new FileInfo(RagPaths.VocabTxt).Length : 0;
        var index = RagIndex.Load(RagPaths.IndexFile);
        bool indexPresent = index is not null;
        return new RagStatus(
            RagPaths.Root, model, modelBytes, indexPresent,
            indexPresent ? new FileInfo(RagPaths.IndexFile).Length : 0,
            index?.Chunks.Count ?? 0, index?.Corpus ?? "", index?.BuiltUtc.ToString("o"), RagPaths.ModelRepo);
    }

    public sealed record RagDatasetResult(string Dir, int Documents, string Url);

    /// <summary>Download the corpus if absent, without building the index. `rag install` calls this too.</summary>
    public static RagDatasetResult FetchDataset(Action<string>? log = null)
    {
        string dir = RagInstaller.EnsureDatasetAsync(log).GetAwaiter().GetResult();
        int n = Directory.GetFiles(dir, "*.md", SearchOption.AllDirectories).Length;
        return new RagDatasetResult(dir, n, RagPaths.DatasetUrl);
    }

    public static RagInstallResult Install(string? corpus, int? threads = null, int? batch = null, Action<string>? log = null)
    {
        // No corpus given means the full public dataset, fetched from the release and verified,
        // so the package stays small and a client still gets everything with one command.
        string ctx = corpus ?? RagInstaller.EnsureDatasetAsync(log).GetAwaiter().GetResult();
        if (!Directory.Exists(ctx))
            throw new DirectoryNotFoundException($"no corpus at '{ctx}'.");
        RagInstaller.EnsureModelAsync(log).GetAwaiter().GetResult();
        using var embedder = new RagEmbedder(RagPaths.ModelOnnx, RagPaths.VocabTxt, threads);
        var index = RagIndex.Build(ctx, embedder, (n, doc) => log?.Invoke($"  {n,5} chunks, {doc}"), Math.Clamp(batch ?? 16, 1, 128));
        index.Save(RagPaths.IndexFile);
        // The graph is built from the same documents and embeddings, so it and the index agree.
        var graph = RagGraph.Build(ctx, index, (n, doc) => log?.Invoke($"  {n,5} graph nodes, {doc}"));
        graph.Save(RagPaths.GraphFile);
        log?.Invoke($"graph    {graph.Nodes.Count} nodes, {graph.Edges.Count} edges");
        return new RagInstallResult(
            RagPaths.Root, ctx, index.Chunks.Count, RagPaths.IndexFile, new FileInfo(RagPaths.IndexFile).Length, RagPaths.ModelRepo);
    }

    public static RagSearchResult Search(string query, int top = 8, int? threads = null)
    {
        var index = RagIndex.Load(RagPaths.IndexFile)
            ?? throw new InvalidOperationException("no index. Run 'wc3ctl rag install' first.");
        using var embedder = new RagEmbedder(RagPaths.ModelOnnx, RagPaths.VocabTxt, threads);
        var hits = index.Search(embedder, query, top)
            .Select(h => new RagHit(h.chunk.Document, h.chunk.Heading, MathF.Round(h.score, 3), Truncate(h.chunk.Text, 1200)))
            .ToArray();
        return new RagSearchResult(query, hits.Length, hits);
    }

    private static string Truncate(string s, int n) => s.Length <= n ? s : s[..n] + "\n... (truncated)";

    // ---- the graph half, over the same corpus and embeddings ----

    public sealed record GraphNodeResult(string Id, string Type, string Name, int Degree, string? Edge, float? Weight);
    public sealed record GraphPathResult(string From, string To, int Hops, IReadOnlyList<string> Path);

    private static RagGraph LoadGraph() =>
        RagGraph.Load(RagPaths.GraphFile)
        ?? throw new InvalidOperationException("no graph. Run 'wc3ctl rag install' first.");

    /// <summary>Nodes within a depth of a start node, over the link and similarity edges.</summary>
    public static IReadOnlyList<GraphNodeResult> GraphNeighbors(string node, int depth = 1, string? edge = null, int limit = 25)
    {
        var g = LoadGraph();
        return g.Neighbors(node, depth, edge, limit)
            .Select(n => new GraphNodeResult(n.Id, n.Type, n.Name, 0, n.Edge, n.Weight))
            .ToArray();
    }

    /// <summary>The most connected nodes, the hubs of the corpus.</summary>
    public static IReadOnlyList<GraphNodeResult> GraphHubs(int top = 20, string? type = null)
    {
        var g = LoadGraph();
        return g.Hubs(top, type).Select(n => new GraphNodeResult(n.Id, n.Type, n.Name, n.Degree, null, null)).ToArray();
    }

    /// <summary>The shortest edge path between two nodes.</summary>
    public static GraphPathResult GraphPath(string from, string to)
    {
        var g = LoadGraph();
        var path = g.ShortestPath(from, to);
        return new GraphPathResult(from, to, Math.Max(0, path.Count - 1), path);
    }
}
