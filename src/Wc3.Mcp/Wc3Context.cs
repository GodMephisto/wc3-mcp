// src/Wc3.Mcp/Wc3Context.cs
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using ModelContextProtocol.Protocol;
using Wc3.Commands.Rag;

namespace Wc3.Mcp;

/// <summary>
/// Serves the Warcraft III reference documents under `context/` as MCP resources, and a
/// read-only search tool over them. The resources are the context injector. Per the MCP
/// specification a knowledge server is Resources and Prompts, so a client reads a whole
/// document deterministically rather than hoping a similarity search returns the right chunk.
/// The search tool is the convenience over the same text, for a client that wants a passage
/// instead of 320KB.
/// </summary>
[McpServerToolType]
public static class Wc3Context
{
    private static string Root => Path.Combine(AppContext.BaseDirectory, "context");

    // One heading rule shared with the index and the graph (see RagText.Heading).

    // The optional local retrieval index, built by `wc3ctl rag install`. Loaded once and kept, so
    // a search does not pay the model load again. When it is absent, docs_search is lexical only.
    private static readonly Lazy<RagIndex?> Index = new(() => RagIndex.Load(RagPaths.IndexFile));
    private static readonly Lazy<RagEmbedder?> Embedder = new(() =>
    {
        try { return File.Exists(RagPaths.ModelOnnx) ? new RagEmbedder(RagPaths.ModelOnnx, RagPaths.VocabTxt) : null; }
        catch { return null; }
    });

    /// <summary>One resource per document folder under `context/`, the markdown read whole.</summary>
    public static IReadOnlyList<McpServerResource> CreateResources()
    {
        var list = new List<McpServerResource>();
        if (!Directory.Exists(Root)) return list;
        foreach (var dir in Directory.GetDirectories(Root).OrderBy(d => d, StringComparer.Ordinal))
        {
            string id = Path.GetFileName(dir);
            string? md = Directory.GetFiles(dir, "*.md").FirstOrDefault();
            if (md is null) continue;
            string path = md;
            list.Add(McpServerResource.Create(
                () => File.ReadAllText(path),
                new McpServerResourceCreateOptions
                {
                    UriTemplate = "wc3://context/" + id,
                    Name = id.Replace('-', '_'),
                    Description = Describe(id),
                    MimeType = "text/markdown",
                }));
        }
        return list;
    }

    private static string Describe(string id) => id switch
    {
        "ui-frames" => "The Big UI-Frame Tutorial by Tasyen, in full. Frame creation, frame natives, positioning and the 4:3 limitation, FDF and TOC, every frame type and the SimpleFrame set. Use it for any UI, frame or FDF work, and for the frame errors it catalogues.",
        "abilities" => "The Warcraft 3 Editor Ability Insight reference, in full. Every ability id with its field behaviour, quirks, stacking rules and bugs, the reference for picking a dummy ability or explaining a field. Use it before guessing how an ability field behaves.",
        _ => "A Warcraft III reference document, read whole.",
    };

    [McpServerTool(Name = "docs_search", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("Search the bundled Warcraft III reference documents and return the matching passages with their document and nearest heading. Use this to find the exact rule, field or warning in the UI-Frame tutorial or the Ability Insight reference without loading the whole file. Terms are matched on word boundaries and ranked by how many the passage carries, with a bonus when the section heading names a term, so an ability name finds that ability's section.")]
    public static object DocsSearch(
        [Description("Words to search for.")] string query,
        [Description("Restrict to one document. ui-frames or abilities. Omit to search every document.")] string? doc = null,
        [Description("Maximum passages to return. Default 8.")] int max = 8)
    {
        var terms = Regex.Matches(query.ToLowerInvariant(), "[a-z0-9_]+")
            .Select(m => m.Value).Where(t => t.Length >= 3).Distinct().ToArray();
        if (terms.Length == 0) return new { error = "no searchable terms in the query (need words of 3 or more letters)" };

        // Semantic when the local index is installed, lexical otherwise. The index is built by
        // `wc3ctl rag install` and auto-connects here, so a client needs no configuration.
        if (Index.Value is { } idx && Embedder.Value is { } semantic)
        {
            // Search a wider candidate pool, filter by document, then take max, or the filter
            // could drop a real match that ranked below the global top max.
            var semanticHits = idx.Search(semantic, query, Math.Clamp(max * 8, 8, 50))
                .Where(h => doc is null || FirstSegment(h.chunk.Document).Equals(doc, StringComparison.OrdinalIgnoreCase))
                .Take(Math.Clamp(max, 1, 40))
                .Select(h => new { document = h.chunk.Document, section = h.chunk.Heading, score = Math.Round(h.score, 3), passage = Truncate(h.chunk.Text, 1600) })
                .ToArray();
            return new { query, mode = "semantic", documents = Documents().Select(d => d.id).ToArray(), matches = semanticHits.Length, results = semanticHits };
        }

        var termRx = terms.Select(t => new Regex(@"\b" + Regex.Escape(t), RegexOptions.Compiled)).ToArray();

        var hits = new List<(double score, string doc, string heading, string text)>();

        foreach (var (id, path) in Documents())
        {
            if (doc is not null && !id.Equals(doc, StringComparison.OrdinalIgnoreCase)) continue;
            string text = File.ReadAllText(path).Replace("\r\n", "\n").TrimStart('\uFEFF');
            string heading = "";
            var buf = new StringBuilder();
            foreach (var line in text.Split('\n'))
            {
                var m = RagText.Heading.Match(line);
                if (m.Success)
                {
                    Score(buf, heading, id);
                    buf.Clear();
                    heading = (m.Groups["h"].Success ? m.Groups["h"].Value : "").Trim();
                }
                buf.Append(line).Append('\n');
                if (buf.Length >= 1500) Score(buf, heading, id);
            }
            Score(buf, heading, id);
        }

        void Score(StringBuilder block, string section, string docId)
        {
            if (block.Length == 0) return;
            string para = block.ToString().Trim();
            block.Clear();
            if (para.Length < 20) return;
            string lower = para.ToLowerInvariant();
            int carried = 0;
            for (int i = 0; i < termRx.Length; i++) if (termRx[i].IsMatch(lower)) carried++;
            if (carried == 0) return;
            int inHeading = 0;
            string lowHead = section.ToLowerInvariant();
            for (int i = 0; i < termRx.Length; i++) if (termRx[i].IsMatch(lowHead)) inHeading++;
            double score = carried * 10.0 + inHeading * 15.0 + Math.Min(para.Length, 2000) / 500.0;
            hits.Add((score, docId, section, para));
        }

        var top = hits.OrderByDescending(h => h.score).Take(Math.Clamp(max, 1, 40))
            .Select(h => new { document = h.doc, section = h.heading, score = Math.Round(h.score, 2), passage = Truncate(h.text, 1600) })
            .ToArray();
        return new { query, mode = "lexical", documents = Documents().Select(d => d.id).ToArray(), matches = top.Length, results = top };
    }

    private static IEnumerable<(string id, string path)> Documents()
    {
        if (!Directory.Exists(Root)) yield break;
        foreach (var dir in Directory.GetDirectories(Root).OrderBy(d => d, StringComparer.Ordinal))
        {
            string? md = Directory.GetFiles(dir, "*.md").FirstOrDefault();
            if (md is not null) yield return (Path.GetFileName(dir), md);
        }
    }

    private static string Truncate(string s, int n) =>
        s.Length <= n ? s : s[..n] + "\n... (truncated, read the whole document with the resource wc3://context/)";

    /// <summary>The top-level folder of a document id, the directory under context/, or the id itself.</summary>
    private static string FirstSegment(string docId)
    {
        int slash = docId.IndexOf('/');
        return slash < 0 ? docId : docId[..slash];
    }

    /// <summary>
    /// Map the RAG store's own failures to a clean tool error. A missing or malformed index or
    /// graph, or a failed download, becomes an McpException the client shows, not a raw crash.
    /// </summary>
    private static T Rag<T>(Func<T> body)
    {
        try
        {
            return body();
        }
        catch (Exception ex) when (ex is InvalidOperationException or DirectoryNotFoundException or IOException)
        {
            throw new McpException(ex.Message);
        }
    }

    [McpServerTool(Name = "rag_status", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("Report whether the local semantic retrieval index is installed, with its chunk count, corpus and the embedding model. docs_search uses the index automatically once it is installed.")]
    public static object RagStatus() => Rag(() => (object)RagCommand.Status());

    [McpServerTool(Name = "rag_install", ReadOnly = false, Idempotent = true, OpenWorld = true)]
    [Description("Install the local semantic index and graph over the Warcraft III corpus. Fetches the embedding model and the corpus dataset, verifies each, and builds the index and the graph, after which docs_search is semantic and the graph tools work. Needs network on the first run. The corpus is fixed to the bundled dataset, never a folder named by the caller, so this cannot read arbitrary local files.")]
    public static object RagInstall(
        [Description("CPU threads for embedding. Default min(cores/2, 4), the low CPU setting. Raise it to build faster.")] int? threads = null,
        [Description("Chunks per inference, default 16. Larger batches finish sooner and use more memory.")] int? batch = null)
    {
        var r = Rag(() => RagCommand.Install(null, threads, batch));
        return new { r.Root, r.Corpus, r.Chunks, r.IndexFile, r.ModelRepo, note = "docs_search is now semantic" };
    }

    [McpServerTool(Name = "graph_neighbors", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("List the nodes within a depth of a start node in the corpus graph, over link edges and embedding-similarity edges. Use it to find the documents related to one you already have. The graph is built by rag install, beside the index.")]
    public static object GraphNeighbors(
        [Description("A node id or name, for example a document title or a path without the .md.")] string node,
        [Description("How many hops out to walk. Default 1.")] int depth = 1,
        [Description("Restrict to one edge type, HAS_SECTION, LINKS_TO or SIMILAR_TO. Omit for all.")] string? edge = null,
        [Description("Maximum nodes to return. Default 25.")] int limit = 25)
        => Rag(() => RagCommand.GraphNeighbors(node, depth, edge, limit));

    [McpServerTool(Name = "graph_hubs", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("The most connected nodes in the corpus graph, the documents and sections the corpus centres on. Use it to see what the corpus is really about.")]
    public static object GraphHubs(
        [Description("Maximum nodes to return. Default 20.")] int top = 20,
        [Description("Restrict to one node type, Document or Section. Omit for all.")] string? type = null)
        => Rag(() => RagCommand.GraphHubs(top, type));

    [McpServerTool(Name = "graph_path", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("The shortest chain of edges between two nodes in the corpus graph, so you can see how two documents or subjects connect.")]
    public static object GraphPath(
        [Description("The start node id or name.")] string from,
        [Description("The end node id or name.")] string to)
        => Rag(() => RagCommand.GraphPath(from, to));
}
