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

    public static RagInstallResult Install(string? corpus, int? threads = null, int? batch = null, Action<string>? log = null)
    {
        string ctx = corpus ?? BundledContextDir();
        if (!Directory.Exists(ctx))
            throw new DirectoryNotFoundException(
                $"no corpus at '{ctx}'. Pass --corpus <dir> or run from an install that carries the context folder.");
        RagInstaller.EnsureModelAsync(log).GetAwaiter().GetResult();
        using var embedder = new RagEmbedder(RagPaths.ModelOnnx, RagPaths.VocabTxt, threads);
        var index = RagIndex.Build(ctx, embedder, (n, doc) => log?.Invoke($"  {n,5} chunks, {doc}"), Math.Clamp(batch ?? 16, 1, 128));
        index.Save(RagPaths.IndexFile);
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
}
