// src/Wc3.Commands/Rag/RagService.cs
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.Tokenizers;

namespace Wc3.Commands.Rag;

/// <summary>
/// A local, single install retrieval index over the bundled Warcraft III documents. The same
/// setup the house vault runs, BAAI/bge-small-en-v1.5 at 384 dimensions, windowed at about
/// 1800 characters, cosine over normalized vectors, so a client gets semantic retrieval with
/// no Python, no service and no key. The model is fetched once into a local folder.
/// </summary>
public static class RagPaths
{
    public static string Root =>
        Environment.GetEnvironmentVariable("W3C_RAG_HOME") is { Length: > 0 } custom
            ? custom
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "wc3ctl", "rag");
    public static string ModelDir => Path.Combine(Root, "model");
    public static string ModelOnnx => Path.Combine(ModelDir, "model.onnx");
    public static string VocabTxt => Path.Combine(ModelDir, "vocab.txt");
    public static string IndexFile => Path.Combine(Root, "index.bin");
    public static string GraphFile => Path.Combine(Root, "graph.bin");

    /// <summary>The corpus, downloaded from the release rather than shipped, so the package stays small.</summary>
    public static string DatasetDir => Path.Combine(Root, "corpus");

    public const string ModelRepo = "BAAI/bge-small-en-v1.5";
    public const string ModelUrl = "https://huggingface.co/BAAI/bge-small-en-v1.5/resolve/main/onnx/model.onnx";
    public const string VocabUrl = "https://huggingface.co/BAAI/bge-small-en-v1.5/resolve/main/vocab.txt";
    // Pinned digests, because the model is loaded by ONNX Runtime from a user-writable folder and
    // is the same trust concern as the corpus, which was already verified.
    public const string ModelSha256 = "828e1496d7fabb79cfa4dcd84fa38625c0d3d21da474a00f08db0f559940cf35";
    public const string VocabSha256 = "07eced375cec144d27c900241f3e339478dec958f92fddbc551f295c992038a3";

    public const string DatasetUrl = "https://github.com/GodMephisto/wc3-mcp/releases/download/dataset-v1/wc3-corpus.tgz";
    public const string DatasetSha256 = "32bf2562c3133f704a00208c60596e0450591c4be530d2db699f938a3ed5da16";
}

/// <summary>
/// One heading rule for the index, the graph and the MCP search, so a section means the same thing
/// in all three. Markdown hashes, the Ability Insight dash convention, and rule lines of equals.
/// </summary>
public static class RagText
{
    public static readonly Regex Heading =
        new(@"^\s*(?:#{1,6}\s+(?<h>.*)|-{1,2}(?<h>[A-Z(][^\n]{0,80})|={3,})$", RegexOptions.Compiled);
}

/// <summary>
/// bge-small-en-v1.5 through ONNX Runtime. CLS pooling then L2 normalize, matching the model card.
/// The CPU is kept modest on purpose. ONNX's OpenMP backend spins by default, so a batch job can
/// pin every core, and the fix is three things, PASSIVE wait so idle threads sleep, a small
/// intra-op cap rather than every core, and batched inference so the cost is one run per batch
/// instead of one run per chunk. Set W3C_RAG_THREADS to change the cap.
/// </summary>
public sealed class RagEmbedder : IDisposable
{
    public const int Dim = 384;
    private const int MaxTokens = 512;
    private readonly InferenceSession _session;
    private readonly BertTokenizer _tokenizer;

    public RagEmbedder(string modelPath, string vocabPath, int? threads = null)
    {
        // Must be set before the session is created, or the native pool already spins.
        Environment.SetEnvironmentVariable("OMP_WAIT_POLICY", "PASSIVE");
        int chosen = threads
            ?? (int.TryParse(Environment.GetEnvironmentVariable("W3C_RAG_THREADS"), out var env) && env > 0
                ? env
                : Math.Clamp(Environment.ProcessorCount / 2, 2, 4));
        var options = new SessionOptions
        {
            IntraOpNumThreads = Math.Clamp(chosen, 1, 64),
            InterOpNumThreads = 1,
            ExecutionMode = ExecutionMode.ORT_SEQUENTIAL,
        };
        _session = new InferenceSession(modelPath, options);
        _tokenizer = BertTokenizer.Create(vocabPath);
    }

    public float[] Embed(string text) => EmbedBatch(new[] { text })[0];

    /// <summary>One ONNX run for the whole batch, padded to the batch's longest sequence.</summary>
    public float[][] EmbedBatch(IReadOnlyList<string> texts)
    {
        int b = texts.Count;
        var idList = new List<long>[b];
        int maxLen = 1;
        for (int i = 0; i < b; i++)
        {
            var ids = _tokenizer.EncodeToIds(texts[i], addSpecialTokens: true).Select(x => (long)x).ToList();
            if (ids.Count > MaxTokens) ids = ids.Take(MaxTokens).ToList();
            idList[i] = ids;
            if (ids.Count > maxLen) maxLen = ids.Count;
        }

        var inputIds = new DenseTensor<long>(new[] { b, maxLen });
        var attn = new DenseTensor<long>(new[] { b, maxLen });
        var types = new DenseTensor<long>(new[] { b, maxLen });
        for (int i = 0; i < b; i++)
            for (int j = 0; j < maxLen; j++)
            {
                bool real = j < idList[i].Count;
                inputIds[i, j] = real ? idList[i][j] : 0;
                attn[i, j] = real ? 1 : 0;
                types[i, j] = 0;
            }

        var inputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("input_ids", inputIds),
            NamedOnnxValue.CreateFromTensor("attention_mask", attn),
            NamedOnnxValue.CreateFromTensor("token_type_ids", types),
        };
        using var results = _session.Run(inputs);
        var last = results.First(r => r.Name == "last_hidden_state").AsTensor<float>();

        var output = new float[b][];
        for (int i = 0; i < b; i++)
        {
            var v = new float[Dim];
            for (int d = 0; d < Dim; d++) v[d] = last[i, 0, d];
            float norm = 0; for (int d = 0; d < Dim; d++) norm += v[d] * v[d];
            norm = MathF.Sqrt(norm);
            if (norm > 0) for (int d = 0; d < Dim; d++) v[d] /= norm;
            output[i] = v;
        }
        return output;
    }

    public void Dispose() => _session.Dispose();
}

public sealed record RagChunk(string Document, string Heading, string Text, float[] Vector);

/// <summary>The windowed index. One vector per window, at about 1800 characters, heading tracked.</summary>
public sealed class RagIndex
{
    public IReadOnlyList<RagChunk> Chunks => _chunks;
    private readonly List<RagChunk> _chunks;
    public string Corpus { get; }
    public DateTime BuiltUtc { get; }

    public RagIndex(List<RagChunk> chunks, string corpus, DateTime builtUtc)
    {
        _chunks = chunks; Corpus = corpus; BuiltUtc = builtUtc;
    }

    public static RagIndex Build(string corpusDir, RagEmbedder embedder, Action<int, string>? progress = null, int batch = 16)
    {
        var chunks = new List<RagChunk>();
        var pending = new List<(string doc, string head, string text)>();
        var docs = Directory.Exists(corpusDir)
            ? Directory.GetFiles(corpusDir, "*.md", SearchOption.AllDirectories)
                .OrderBy(p => p, StringComparer.Ordinal).ToArray()
            : Array.Empty<string>();

        void Flush()
        {
            if (pending.Count == 0) return;
            var vectors = embedder.EmbedBatch(pending.Select(p => p.head + "\n" + p.text).ToArray());
            for (int i = 0; i < pending.Count; i++)
                chunks.Add(new RagChunk(pending[i].doc, pending[i].head, pending[i].text, vectors[i]));
            pending.Clear();
        }

        foreach (var doc in docs)
        {
            string id = DocIdOf(corpusDir, doc);
            string text = File.ReadAllText(doc).Replace("\r\n", "\n").TrimStart('\uFEFF');
            foreach (var (heading, window) in Window(text))
            {
                if (window.Trim().Length < 40) continue;
                pending.Add((id, heading, window));
                if (pending.Count >= batch) Flush();
            }
            Flush();
            progress?.Invoke(chunks.Count, id);
        }
        Flush();
        return new RagIndex(chunks, corpusDir, DateTime.UtcNow);
    }

    /// <summary>
    /// The one document id, the corpus-relative path without extension, used by both the index and
    /// the graph so a chunk and a node name the same document. A bare file name here and a relative
    /// path there is what left the similarity edges pointing at nodes that did not exist.
    /// </summary>
    public static string DocIdOf(string root, string path) =>
        Path.ChangeExtension(Path.GetRelativePath(root, path), null)!.Replace('\\', '/');

    /// <summary>Split at headings and at a soft 1800 character cap, keeping the nearest heading.</summary>
    public static IEnumerable<(string heading, string window)> Window(string text)
    {
        string heading = "";
        var buf = new StringBuilder();
        foreach (var line in text.Split('\n'))
        {
            var m = RagText.Heading.Match(line);
            if (m.Success)
            {
                if (buf.Length > 0) { yield return (heading, buf.ToString()); buf.Clear(); }
                heading = m.Groups["h"].Value.Trim();
            }
            buf.Append(line).Append('\n');
            if (buf.Length >= 1800) { yield return (heading, buf.ToString()); buf.Clear(); }
        }
        if (buf.Length > 0) yield return (heading, buf.ToString());
    }

    public IEnumerable<(RagChunk chunk, float score)> Search(RagEmbedder embedder, string query, int top = 8)
    {
        var q = embedder.Embed(query);
        return _chunks
            .Select(c => (c, Dot(q, c.Vector)))
            .OrderByDescending(x => x.Item2)
            .Take(Math.Clamp(top, 1, 50));
    }

    private static float Dot(float[] a, float[] b)
    {
        float s = 0; for (int i = 0; i < a.Length; i++) s += a[i] * b[i]; return s;
    }

    public void Save(string path)
    {
        string? dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        using var fs = File.Create(path);
        using var w = new BinaryWriter(fs, Encoding.UTF8);
        w.Write(3);
        w.Write(Corpus);
        w.Write(BuiltUtc.ToString("o"));
        w.Write(_chunks.Count);
        foreach (var c in _chunks)
        {
            w.Write(c.Document); w.Write(c.Heading); w.Write(c.Text);
            foreach (var f in c.Vector) w.Write(f);
        }
    }

    public static RagIndex? Load(string path)
    {
        if (!File.Exists(path)) return null;
        try
        {
            using var fs = File.OpenRead(path);
            using var r = new BinaryReader(fs, Encoding.UTF8);
            if (r.ReadInt32() != 3) return null;
            string corpus = r.ReadString();
            var built = DateTime.Parse(r.ReadString());
            int count = r.ReadInt32();
            // Bound the count against the stream before allocating, and a corrupt file degrades to
            // no index rather than crashing the server.
            if (count < 0 || (long)count * (RagEmbedder.Dim * 4 + 16) > fs.Length) return null;
            var chunks = new List<RagChunk>(count);
            for (int i = 0; i < count; i++)
            {
                string doc = r.ReadString(), head = r.ReadString(), txt = r.ReadString();
                var vec = new float[RagEmbedder.Dim];
                for (int d = 0; d < RagEmbedder.Dim; d++) vec[d] = r.ReadSingle();
                chunks.Add(new RagChunk(doc, head, txt, vec));
            }
            return new RagIndex(chunks, corpus, built);
        }
        catch (Exception ex) when (ex is IOException or EndOfStreamException or FormatException or ArgumentException)
        {
            return null;
        }
    }
}

public static class RagInstaller
{
    /// <summary>Fetch the model once if absent. An HF_TOKEN is used when set, else anonymous.</summary>
    public static async Task EnsureModelAsync(Action<string>? log = null, CancellationToken ct = default)
    {
        Directory.CreateDirectory(RagPaths.ModelDir);
        using var http = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
        string? token = Environment.GetEnvironmentVariable("HF_TOKEN");
        if (!string.IsNullOrWhiteSpace(token))
            http.DefaultRequestHeaders.Authorization = new("Bearer", token);
        await Download(http, RagPaths.ModelUrl, RagPaths.ModelOnnx, RagPaths.ModelSha256, log, ct).ConfigureAwait(false);
        await Download(http, RagPaths.VocabUrl, RagPaths.VocabTxt, RagPaths.VocabSha256, log, ct).ConfigureAwait(false);
    }

    /// <summary>Fetch the corpus once, verify the digest, and extract it. Idempotent per install.</summary>
    public static async Task<string> EnsureDatasetAsync(Action<string>? log = null, CancellationToken ct = default)
    {
        if (Directory.Exists(RagPaths.DatasetDir) && Directory.EnumerateFileSystemEntries(RagPaths.DatasetDir).Any())
        {
            log?.Invoke("dataset  present");
            return RagPaths.DatasetDir;
        }
        Directory.CreateDirectory(RagPaths.DatasetDir);
        Directory.CreateDirectory(RagPaths.Root);
        string tgz = Path.Combine(RagPaths.Root, "wc3-corpus.tgz");
        using (var http = new HttpClient { Timeout = TimeSpan.FromMinutes(30) })
        using (var resp = await http.GetAsync(RagPaths.DatasetUrl, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false))
        {
            resp.EnsureSuccessStatusCode();
            log?.Invoke("dataset  downloading ...");
            await using var src = await resp.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
            await using var dst = File.Create(tgz);
            await src.CopyToAsync(dst, ct).ConfigureAwait(false);
        }

        string actual;
        using (var sha = System.Security.Cryptography.SHA256.Create())
        await using (var fs = File.OpenRead(tgz))
            actual = Convert.ToHexString(await sha.ComputeHashAsync(fs, ct).ConfigureAwait(false)).ToLowerInvariant();
        if (!string.Equals(actual, RagPaths.DatasetSha256, StringComparison.OrdinalIgnoreCase))
        {
            File.Delete(tgz);
            throw new InvalidOperationException($"dataset checksum mismatch, expected {RagPaths.DatasetSha256}, got {actual}");
        }

        log?.Invoke("dataset  verified, extracting ...");
        await using (var fs = File.OpenRead(tgz))
        await using (var gz = new System.IO.Compression.GZipStream(fs, System.IO.Compression.CompressionMode.Decompress))
            System.Formats.Tar.TarFile.ExtractToDirectory(gz, RagPaths.DatasetDir, overwriteFiles: true);
        File.Delete(tgz);
        log?.Invoke("dataset  ready");
        return RagPaths.DatasetDir;
    }

    /// <summary>
    /// Stream to a .part file, verify the digest, then move it into place, so a killed download
    /// never leaves a partial file the loader would accept and a tampered one never loads.
    /// </summary>
    private static async Task Download(HttpClient http, string url, string dest, string sha256, Action<string>? log, CancellationToken ct)
    {
        if (File.Exists(dest) && new FileInfo(dest).Length > 0 &&
            string.Equals(Sha256Of(dest), sha256, StringComparison.OrdinalIgnoreCase))
        {
            log?.Invoke($"present  {Path.GetFileName(dest)}");
            return;
        }
        log?.Invoke($"fetching {Path.GetFileName(dest)} ...");
        string part = dest + ".part";
        using (var resp = await http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false))
        {
            resp.EnsureSuccessStatusCode();
            await using var src = await resp.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
            await using var dst = File.Create(part);
            await src.CopyToAsync(dst, ct).ConfigureAwait(false);
        }
        string actual = Sha256Of(part);
        if (!string.Equals(actual, sha256, StringComparison.OrdinalIgnoreCase))
        {
            File.Delete(part);
            throw new InvalidOperationException($"checksum mismatch for {Path.GetFileName(dest)}, expected {sha256}, got {actual}");
        }
        File.Move(part, dest, overwrite: true);
        log?.Invoke($"wrote    {Path.GetFileName(dest)} ({new FileInfo(dest).Length / 1024 / 1024} MB)");
    }

    private static string Sha256Of(string path)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        using var fs = File.OpenRead(path);
        return Convert.ToHexString(sha.ComputeHash(fs)).ToLowerInvariant();
    }
}
