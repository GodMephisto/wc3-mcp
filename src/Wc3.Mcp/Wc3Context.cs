// src/Wc3.Mcp/Wc3Context.cs
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using ModelContextProtocol.Protocol;
using Wc3.Commands.Rag;

namespace Wc3.Mcp;

/// <summary>
/// A resource backed by one string read at server start. The byte size and the SHA256 of the bytes
/// are fixed once, so resources/list reports them without touching the disk again. The SDK options
/// object carries Meta but no Size, so this subclass overrides ProtocolResource to set the protocol
/// Size field and to put the digest in _meta under the key sha256.
/// </summary>
internal sealed class ServedTextResource : McpServerResource
{
    private readonly string _uri;
    private readonly string _name;
    private readonly string? _title;
    private readonly string? _description;
    private readonly string _mimeType;
    private readonly string _text;
    private readonly long _size;
    private readonly string _sha256;

    internal ServedTextResource(
        string uri,
        string name,
        string? title,
        string? description,
        string mimeType,
        string text,
        byte[]? bytes = null)
    {
        _uri = uri;
        _name = name;
        _title = title;
        _description = description;
        _mimeType = mimeType;
        _text = text;
        byte[] content = bytes ?? Encoding.UTF8.GetBytes(text);
        _size = content.LongLength;
        _sha256 = Digest(content);
    }

    internal string Sha256 => _sha256;

    internal static string Digest(byte[] bytes) =>
        Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();

    private JsonObject Meta() => new() { ["sha256"] = _sha256 };

    public override ResourceTemplate ProtocolResourceTemplate => new()
    {
        UriTemplate = _uri,
        Name = _name,
        Title = _title,
        Description = _description,
        MimeType = _mimeType,
        Meta = Meta(),
    };

    public override Resource? ProtocolResource => new Resource
    {
        Uri = _uri,
        Name = _name,
        Title = _title,
        Description = _description,
        MimeType = _mimeType,
        Size = _size,
        Meta = Meta(),
    };

    public override IReadOnlyList<object> Metadata => Array.Empty<object>();

    public override bool IsMatch(string uri) =>
        string.Equals(uri, _uri, StringComparison.OrdinalIgnoreCase);

    public override ValueTask<ReadResourceResult> ReadAsync(
        RequestContext<ReadResourceRequestParams> request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(new ReadResourceResult
        {
            Contents = new List<ResourceContents>
            {
                new TextResourceContents { Uri = _uri, MimeType = _mimeType, Text = _text },
            },
        });
    }
}

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

    /// <summary>
    /// One resource per document folder under `context/`, the markdown read whole, and one resource
    /// per top level section of that markdown. Every resource carries the byte size and the digest
    /// of what it serves, fixed at start so resources/list needs no disk read.
    /// </summary>
    public static IReadOnlyList<McpServerResource> CreateResources()
    {
        var list = new List<McpServerResource>();
        if (!Directory.Exists(Root)) return list;
        foreach (var dir in Directory.GetDirectories(Root).OrderBy(d => d, StringComparer.Ordinal))
        {
            string id = Path.GetFileName(dir);
            if (IsSkillFolder(id)) continue;
            string? md = Directory.GetFiles(dir, "*.md").FirstOrDefault();
            if (md is null) continue;

            // Read the exact bytes once. The size and the digest describe the file on disk, and the
            // served text is decoded from those same bytes, so the three can never disagree.
            byte[] bytes = File.ReadAllBytes(md);
            string text = Encoding.UTF8.GetString(bytes);
            list.Add(new ServedTextResource(
                "wc3://context/" + id,
                id.Replace('-', '_'),
                null,
                Describe(id),
                "text/markdown",
                text,
                bytes));

            var seen = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var (heading, body) in Sections(text))
            {
                string slug = Slug(heading);
                int count = seen.TryGetValue(slug, out int prior) ? prior : 0;
                seen[slug] = count + 1;
                string unique = count == 0 ? slug : slug + "-" + (count + 1);
                list.Add(new ServedTextResource(
                    "wc3://context/" + id + "/" + unique,
                    id.Replace('-', '_') + "_" + unique.Replace('-', '_'),
                    heading,
                    DescribeSection(heading, id),
                    "text/markdown",
                    body));
            }
        }
        return list;
    }

    /// <summary>
    /// The top level sections of one document. Two shapes exist in the corpus. The UI-Frame
    /// tutorial marks a section with a line of two hashes and puts the heading on the next line,
    /// and the Ability Insight reference marks each entry with a dash before a capital letter.
    /// A document is split by one shape only, so Lua comment lines that begin with two dashes in
    /// the tutorial never become sections. Where it applies, the dash rule is
    /// <see cref="RagText.Heading"/>, so a section means the same thing here as it does in the
    /// index, the graph and docs_search.
    /// </summary>
    private static IEnumerable<(string Heading, string Text)> Sections(string text)
    {
        string[] lines = text.Split('\n');
        for (int i = 0; i < lines.Length; i++) lines[i] = lines[i].TrimEnd((char)13);

        // The tutorial uses a hash line as its section marker. The ability reference has no hash
        // lines at all, so its entries are the dash headings.
        bool marked = lines.Any(line => line.Trim() == "##");

        var marks = new List<(int Start, string Heading)>();
        for (int i = 0; i < lines.Length; i++)
        {
            if (marked)
            {
                if (lines[i].Trim() != "##") continue;
                int j = i + 1;
                while (j < lines.Length && lines[j].Trim().Length == 0) j++;
                if (j < lines.Length && lines[j].Trim().Length > 0)
                    marks.Add((i, lines[j].Trim()));
                continue;
            }

            // Skip the document title and any hashed subheading. Only the dash entries are sections.
            if (lines[i].TrimStart().StartsWith('#')) continue;
            Match m = RagText.Heading.Match(lines[i]);
            if (m.Success && m.Groups["h"].Success && m.Groups["h"].Value.Trim().Length > 0)
                marks.Add((i, m.Groups["h"].Value.Trim()));
        }

        for (int k = 0; k < marks.Count; k++)
        {
            int end = k + 1 < marks.Count ? marks[k + 1].Start : lines.Length;
            yield return (marks[k].Heading, string.Join("\n", lines, marks[k].Start, end - marks[k].Start));
        }
    }

    /// <summary>A URI path segment for a heading, lowercase words joined by single hyphens.</summary>
    private static string Slug(string heading)
    {
        var sb = new StringBuilder(heading.Length);
        foreach (char c in heading.Trim().ToLowerInvariant())
        {
            if (c is >= 'a' and <= 'z' or >= '0' and <= '9') sb.Append(c);
            else if (sb.Length > 0 && sb[^1] != '-') sb.Append('-');
        }
        string slug = sb.ToString().Trim('-');
        return slug.Length == 0 ? "section" : slug;
    }

    private static string DescribeSection(string heading, string id) =>
        heading + ". One top level section of " + LongTitle(id)
            + ", served on its own so a client reads one part instead of the whole document.";

    private static string LongTitle(string id) => id switch
    {
        "ui-frames" => "the Big UI-Frame Tutorial",
        "abilities" => "the Warcraft 3 Editor Ability Insight reference",
        _ => "the " + id + " reference document",
    };

    private static string Describe(string id) => id switch
    {
        "ui-frames" => "The Big UI-Frame Tutorial by Tasyen, in full. Frame creation, frame natives, positioning and the 4:3 limitation, FDF and TOC, every frame type and the SimpleFrame set. Use it for any UI, frame or FDF work, and for the frame errors it catalogues.",
        "abilities" => "The Warcraft 3 Editor Ability Insight reference, in full. Every ability id with its field behaviour, quirks, stacking rules and bugs, the reference for picking a dummy ability or explaining a field. Use it before guessing how an ability field behaves.",
        _ => "A Warcraft III reference document, read whole.",
    };

    [McpServerTool(Name = "docs_search", Title = "Search the reference documents", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
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
            string id = Path.GetFileName(dir);
            if (IsSkillFolder(id)) continue;
            string? md = Directory.GetFiles(dir, "*.md").FirstOrDefault();
            if (md is not null) yield return (id, md);
        }
    }

    /// <summary>
    /// The skills live under `context/skills` only so the reference folder and the skills stay one
    /// corpus on disk. They are served as `wc3://skills/*`, not as a context document, and the
    /// search must not index three files as one more document, so the folder is skipped here.
    /// </summary>
    private static bool IsSkillFolder(string id) =>
        id.Equals("skills", StringComparison.OrdinalIgnoreCase);

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

    [McpServerTool(Name = "rag_status", Title = "Report the retrieval index state", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Report whether the local semantic retrieval index is installed, with its chunk count, corpus and the embedding model. docs_search uses the index automatically once it is installed.")]
    public static object RagStatus() => Rag(() => (object)RagCommand.Status());

    [McpServerTool(Name = "rag_install", Title = "Install the retrieval index", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = true)]
    [Description("Install the local semantic index and graph over the Warcraft III corpus. Fetches the embedding model and the corpus dataset, verifies each, and builds the index and the graph, after which docs_search is semantic and the graph tools work. Needs network on the first run. It writes many files under the user's local app data, in the rag root the answer names, and never into a map folder, so it is not gated like a single artifact and reports the root and how many files it wrote instead. The corpus is the bundled dataset, never a caller named folder, so this cannot read arbitrary local files.")]
    public static object RagInstall(
        [Description("CPU threads for embedding. Default min(cores/2, 4), the low CPU setting. Raise it to build faster.")] int? threads = null,
        [Description("Chunks per inference, default 16. Larger batches finish sooner and use more memory.")] int? batch = null)
    {
        // An install writes a corpus, a model, an index and a graph, so it cannot be gated as one
        // artifact. It declares what it wrote instead, and the count is measured from the folder
        // before and after rather than taken from the install's own success message.
        var before = RagFileState(RagPaths.Root);
        var r = Rag(() => RagCommand.Install(null, threads, batch));
        int? filesWritten = RagFilesWritten(RagPaths.Root, before);
        return new { r.Root, r.Corpus, r.Chunks, r.IndexFile, r.ModelRepo, FilesWritten = filesWritten, note = "docs_search is now semantic" };
    }

    /// <summary>
    /// Every file under the rag root, keyed by its relative path, with the length and the last
    /// write time that identify one version of it. Null means the folder could not be read, which
    /// is different from an empty folder, so the count can say unknown rather than zero.
    /// </summary>
    private static Dictionary<string, (long Length, DateTime WrittenAt)>? RagFileState(string root)
    {
        var state = new Dictionary<string, (long Length, DateTime WrittenAt)>(StringComparer.OrdinalIgnoreCase);
        if (!Directory.Exists(root)) return state;
        try
        {
            foreach (string file in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
            {
                var info = new FileInfo(file);
                state[Path.GetRelativePath(root, file)] = (info.Length, info.LastWriteTimeUtc);
            }
            return state;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return null;
        }
    }

    /// <summary>
    /// How many files the install created or updated, or null when either picture of the folder
    /// could not be read. A counting failure must not turn a finished install into an error, and
    /// it must not be reported as zero either.
    /// </summary>
    private static int? RagFilesWritten(
        string root, IReadOnlyDictionary<string, (long Length, DateTime WrittenAt)>? before)
    {
        if (before is null) return null;
        var after = RagFileState(root);
        if (after is null) return null;
        int written = 0;
        foreach (var (path, now) in after)
        {
            if (!before.TryGetValue(path, out var then)
                || then.Length != now.Length
                || then.WrittenAt != now.WrittenAt)
                written++;
        }
        return written;
    }

    [McpServerTool(Name = "graph_neighbors", Title = "List graph neighbours of a node", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("List the nodes within a depth of a start node in the corpus graph, over link edges and embedding-similarity edges. Use it to find the documents related to one you already have. The graph is built by rag install, beside the index.")]
    public static object GraphNeighbors(
        [Description("A node id or name, for example a document title or a path without the .md.")] string node,
        [Description("How many hops out to walk. Default 1.")] int depth = 1,
        [Description("Restrict to one edge type, HAS_SECTION, LINKS_TO or SIMILAR_TO. Omit for all.")] string? edge = null,
        [Description("Maximum nodes to return. Default 25.")] int limit = 25)
        => Rag(() => RagCommand.GraphNeighbors(node, depth, edge, limit));

    [McpServerTool(Name = "graph_hubs", Title = "Show the most connected nodes", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("The most connected nodes in the corpus graph, the documents and sections the corpus centres on. Use it to see what the corpus is really about.")]
    public static object GraphHubs(
        [Description("Maximum nodes to return. Default 20.")] int top = 20,
        [Description("Restrict to one node type, Document or Section. Omit for all.")] string? type = null)
        => Rag(() => RagCommand.GraphHubs(top, type));

    [McpServerTool(Name = "graph_path", Title = "Find the shortest graph path", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("The shortest chain of edges between two nodes in the corpus graph, so you can see how two documents or subjects connect.")]
    public static object GraphPath(
        [Description("The start node id or name.")] string from,
        [Description("The end node id or name.")] string to)
        => Rag(() => RagCommand.GraphPath(from, to));
}
