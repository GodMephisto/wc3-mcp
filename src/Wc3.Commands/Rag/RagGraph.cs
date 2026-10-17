// src/Wc3.Commands/Rag/RagGraph.cs
using System.Text;
using System.Text.RegularExpressions;

namespace Wc3.Commands.Rag;

public sealed record RagNode(string Id, string Type, string Name);
public sealed record RagEdge(string From, string Type, string To, float Weight);

/// <summary>
/// The graph half of the retrieval store, built once from the corpus and the embeddings, with the
/// shape kept independent of any engine that might hold it. Nodes are documents and their sections.
/// Edges are a document to its section, a document to a document it links, and a document to a
/// document it is close to by embedding. Queried in process over dictionaries, no server. A Neo4j
/// store, when it is wanted, is fed from these same nodes and edges rather than rebuilding them.
/// </summary>
public sealed class RagGraph
{
    public IReadOnlyList<RagNode> Nodes => _nodes;
    public IReadOnlyList<RagEdge> Edges => _edges;
    public DateTime BuiltUtc { get; }

    private readonly List<RagNode> _nodes;
    private readonly List<RagEdge> _edges;
    private readonly Dictionary<string, int> _id;
    private readonly List<List<(int To, float Weight, string Type)>> _adj;

    private RagGraph(List<RagNode> nodes, List<RagEdge> edges, DateTime builtUtc)
    {
        _nodes = nodes; _edges = edges; BuiltUtc = builtUtc;
        _id = new Dictionary<string, int>(nodes.Count, StringComparer.Ordinal);
        for (int i = 0; i < nodes.Count; i++) _id[nodes[i].Id] = i;
        _adj = new List<List<(int, float, string)>>(nodes.Count);
        for (int i = 0; i < nodes.Count; i++) _adj.Add(new List<(int, float, string)>());
        foreach (var e in edges)
        {
            if (!_id.TryGetValue(e.From, out int a) || !_id.TryGetValue(e.To, out int b)) continue;
            _adj[a].Add((b, e.Weight, e.Type));
            _adj[b].Add((a, e.Weight, e.Type));
        }
    }

    /// <summary>Documents and their sections, plus link and similarity edges between documents.</summary>
    public static RagGraph Build(string corpusDir, RagIndex? index, Action<int, string>? progress = null,
        float similarityFloor = 0.80f, int similarityTop = 6)
    {
        var nodes = new List<RagNode>();
        var edges = new List<RagEdge>();
        var docs = Directory.Exists(corpusDir)
            ? Directory.GetFiles(corpusDir, "*.md", SearchOption.AllDirectories).OrderBy(p => p, StringComparer.Ordinal).ToArray()
            : Array.Empty<string>();

        var titles = new Dictionary<string, string>(StringComparer.Ordinal);   // doc id -> title
        var links = new List<(string from, string target)>();

        foreach (var path in docs)
        {
            string id = RagIndex.DocIdOf(corpusDir, path);
            string text = File.ReadAllText(path).Replace("\r\n", "\n").TrimStart('\uFEFF');
            string title = FirstHeading(text) ?? Path.GetFileNameWithoutExtension(path);
            titles[id] = title;
            nodes.Add(new RagNode("doc:" + id, "Document", title));

            int section = 0;
            string heading = title;
            var buf = new StringBuilder();
            void Flush()
            {
                section++;
                string secId = "sec:" + id + "#" + section;
                var name = string.IsNullOrWhiteSpace(heading) ? title : heading;
                nodes.Add(new RagNode(secId, "Section", name));
                edges.Add(new RagEdge("doc:" + id, "HAS_SECTION", secId, 1f));
                foreach (Match m in Regex.Matches(buf.ToString(), @"\[\[([^\]\|]+)"))
                    links.Add((id, m.Groups[1].Value.Trim()));
                buf.Clear();
            }
            foreach (var line in text.Split('\n'))
            {
                var hm = RagText.Heading.Match(line);
                if (hm.Success) { if (buf.Length > 0) Flush(); heading = hm.Groups["h"].Value.Trim(); }
                buf.Append(line).Append('\n');
            }
            if (buf.Length > 0) Flush();
            progress?.Invoke(nodes.Count, id);
        }

        // Document to document, by title a link names.
        var byTitle = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var (id, t) in titles) byTitle.TryAdd(t, id);
        foreach (var (from, target) in links)
            if (byTitle.TryGetValue(target, out string? to) && !string.Equals(to, from, StringComparison.Ordinal))
                edges.Add(new RagEdge("doc:" + from, "LINKS_TO", "doc:" + to, 1f));

        // Document to document, by embedding closeness. One mean vector per document.
        if (index is not null && index.Chunks.Count > 0)
        {
            var byDoc = new Dictionary<string, (float[] sum, int n)>(StringComparer.Ordinal);
            foreach (var c in index.Chunks)
            {
                if (!byDoc.TryGetValue(c.Document, out var acc)) acc = (new float[RagEmbedder.Dim], 0);
                for (int d = 0; d < RagEmbedder.Dim; d++) acc.sum[d] += c.Vector[d];
                byDoc[c.Document] = (acc.sum, acc.n + 1);
            }
            var ids = byDoc.Keys.OrderBy(k => k, StringComparer.Ordinal).ToArray();
            var vecs = ids.ToDictionary(k => k, k =>
            {
                var (sum, n) = byDoc[k]; var v = new float[RagEmbedder.Dim];
                for (int d = 0; d < RagEmbedder.Dim; d++) v[d] = sum[d] / n;
                float norm = 0; for (int d = 0; d < RagEmbedder.Dim; d++) norm += v[d] * v[d];
                norm = MathF.Sqrt(norm); if (norm > 0) for (int d = 0; d < RagEmbedder.Dim; d++) v[d] /= norm;
                return v;
            });
            for (int i = 0; i < ids.Length; i++)
            {
                var near = new List<(string id, float s)>();
                for (int j = 0; j < ids.Length; j++)
                {
                    if (i == j) continue;
                    float s = 0; for (int d = 0; d < RagEmbedder.Dim; d++) s += vecs[ids[i]][d] * vecs[ids[j]][d];
                    if (s >= similarityFloor) near.Add((ids[j], s));
                }
                foreach (var (other, s) in near.OrderByDescending(x => x.s).Take(similarityTop))
                    edges.Add(new RagEdge("doc:" + ids[i], "SIMILAR_TO", "doc:" + other, MathF.Round(s, 3)));
            }
        }

        return new RagGraph(nodes, edges, DateTime.UtcNow);
    }

    private static string? FirstHeading(string text)
    {
        var m = Regex.Match(text, @"^\s*#\s+(?<h>.+)$", RegexOptions.Multiline);
        return m.Success ? m.Groups["h"].Value.Trim() : null;
    }

    public sealed record Neighbor(string Id, string Type, string Name, string Edge, float Weight, int Depth, int Degree);
    public sealed record NodeSummary(string Id, string Type, string Name, int Degree);

    /// <summary>Breadth first over the edges, up to a depth.</summary>
    public IReadOnlyList<Neighbor> Neighbors(string start, int depth = 1, string? edgeType = null, int limit = 50)
    {
        int root = Resolve(start);
        if (root < 0) return Array.Empty<Neighbor>();
        var seen = new HashSet<int> { root };
        var frontier = new Queue<(int node, int d)>();
        frontier.Enqueue((root, 0));
        var outp = new List<Neighbor>();
        while (frontier.Count > 0 && outp.Count < limit)
        {
            var (cur, d) = frontier.Dequeue();
            if (d >= depth) continue;
            foreach (var (to, w, t) in _adj[cur])
            {
                if (edgeType is not null && !t.Equals(edgeType, StringComparison.OrdinalIgnoreCase)) continue;
                if (!seen.Add(to)) continue;
                var n = _nodes[to];
                outp.Add(new Neighbor(n.Id, n.Type, n.Name, t, w, d + 1, _adj[to].Count));
                frontier.Enqueue((to, d + 1));
            }
        }
        return outp;
    }

    /// <summary>Nodes by degree, the hubs of the corpus.</summary>
    public IReadOnlyList<NodeSummary> Hubs(int top = 20, string? type = null) =>
        Enumerable.Range(0, _nodes.Count)
            .Where(i => type is null || _nodes[i].Type.Equals(type, StringComparison.OrdinalIgnoreCase))
            .Select(i => new NodeSummary(_nodes[i].Id, _nodes[i].Type, _nodes[i].Name, _adj[i].Count))
            .OrderByDescending(n => n.Degree)
            .ThenBy(n => n.Id, StringComparer.Ordinal)
            .Take(Math.Clamp(top, 1, 100))
            .ToArray();

    /// <summary>Shortest edge path between two nodes, by breadth first search.</summary>
    public IReadOnlyList<string> ShortestPath(string from, string to)
    {
        int a = Resolve(from), b = Resolve(to);
        if (a < 0 || b < 0) return Array.Empty<string>();
        var prev = new int[_nodes.Count];
        Array.Fill(prev, -1);
        var seen = new bool[_nodes.Count];
        var q = new Queue<int>();
        q.Enqueue(a); seen[a] = true;
        while (q.Count > 0)
        {
            int cur = q.Dequeue();
            if (cur == b) break;
            foreach (var (next, _, _) in _adj[cur])
                if (!seen[next]) { seen[next] = true; prev[next] = cur; q.Enqueue(next); }
        }
        if (!seen[b]) return Array.Empty<string>();
        var path = new List<string>();
        for (int cur = b; cur != -1; cur = prev[cur]) path.Add(_nodes[cur].Id);
        path.Reverse();
        return path;
    }

    /// <summary>Name or id to a node index, exact first then case-insensitive contains.</summary>
    public int Resolve(string nameOrId)
    {
        if (_id.TryGetValue(nameOrId, out int exact)) return exact;
        string dotted = "doc:" + nameOrId;
        if (_id.TryGetValue(dotted, out int doc)) return doc;
        for (int i = 0; i < _nodes.Count; i++)
            if (_nodes[i].Name.Equals(nameOrId, StringComparison.OrdinalIgnoreCase)) return i;
        for (int i = 0; i < _nodes.Count; i++)
            if (_nodes[i].Name.Contains(nameOrId, StringComparison.OrdinalIgnoreCase)) return i;
        return -1;
    }

    public void Save(string path)
    {
        string? dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        using var fs = File.Create(path);
        using var w = new BinaryWriter(fs, Encoding.UTF8);
        w.Write(1);
        w.Write(BuiltUtc.ToString("o"));
        w.Write(_nodes.Count);
        foreach (var n in _nodes) { w.Write(n.Id); w.Write(n.Type); w.Write(n.Name); }
        w.Write(_edges.Count);
        foreach (var e in _edges) { w.Write(e.From); w.Write(e.Type); w.Write(e.To); w.Write(e.Weight); }
    }

    public static RagGraph? Load(string path)
    {
        if (!File.Exists(path)) return null;
        try
        {
            using var fs = File.OpenRead(path);
            using var r = new BinaryReader(fs, Encoding.UTF8);
            if (r.ReadInt32() != 1) return null;
            var built = DateTime.Parse(r.ReadString());
            int nc = r.ReadInt32();
            // Bound each count against the stream before allocating, a corrupt file degrades to no
            // graph rather than a large allocation or a crash on the MCP path.
            if (nc < 0 || (long)nc * 12 > fs.Length) return null;
            var nodes = new List<RagNode>(nc);
            for (int i = 0; i < nc; i++) nodes.Add(new RagNode(r.ReadString(), r.ReadString(), r.ReadString()));
            int ec = r.ReadInt32();
            if (ec < 0 || (long)ec * 16 > fs.Length) return null;
            var edges = new List<RagEdge>(ec);
            for (int i = 0; i < ec; i++) edges.Add(new RagEdge(r.ReadString(), r.ReadString(), r.ReadString(), r.ReadSingle()));
            return new RagGraph(nodes, edges, built);
        }
        catch (Exception ex) when (ex is IOException or EndOfStreamException or FormatException or ArgumentException)
        {
            return null;
        }
    }
}
