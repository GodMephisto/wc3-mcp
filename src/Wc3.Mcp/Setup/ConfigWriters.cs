// src/Wc3.Mcp/Setup/ConfigWriters.cs
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Wc3.Mcp.Setup;

/// <summary>The server entry written into a client, meaning how to start this exe and the optional game folder.</summary>
public sealed record ServerEntry(string Name, string Command, IReadOnlyList<string> Args, string? GameDir)
{
    /// <summary>
    /// The entry for the running process. A published exe starts itself. Under <c>dotnet Wc3.Mcp.dll</c>
    /// the host is dotnet, so the entry starts dotnet with this assembly's path.
    /// </summary>
    public static ServerEntry ForThisProcess(string name, string? gameDir, IReadOnlyList<string>? serveArgs = null)
    {
        var exe = Environment.ProcessPath ?? throw new InvalidOperationException("cannot tell where this program is");
        var tail = serveArgs ?? Array.Empty<string>();
        if (Path.GetFileNameWithoutExtension(exe).Equals("dotnet", StringComparison.OrdinalIgnoreCase))
        {
            // Under dotnet the host is dotnet, so the entry starts dotnet with the entry assembly.
            var assembly = System.Reflection.Assembly.GetEntryAssembly()?.Location ?? typeof(ServerEntry).Assembly.Location;
            return new ServerEntry(name, exe, new[] { assembly }.Concat(tail).ToArray(), gameDir);
        }
        return new ServerEntry(name, exe, tail.ToArray(), gameDir);
    }

    /// <summary>The JSON object most clients use for one stdio server.</summary>
    public JsonObject ToJson(bool writeType)
    {
        var o = new JsonObject();
        if (writeType) o["type"] = "stdio";
        o["command"] = Command;
        o["args"] = new JsonArray(Args.Select(a => (JsonNode?)JsonValue.Create(a)).ToArray());
        if (GameDir is not null) o["env"] = new JsonObject { ["WC3_GAME_DIR"] = GameDir };
        return o;
    }
}

/// <summary>What a setup step did to one config file.</summary>
public enum WriteOutcome { Written, Unchanged, Removed, NotPresent, Refused }

/// <summary>One step's result, with a sentence a person can read.</summary>
public sealed record WriteResult(WriteOutcome Outcome, string Path, string Message);

/// <summary>
/// Adds or removes one server in a JSON config without disturbing anything else in the file.
/// A commented file has only the one entry span spliced at its byte offset, so every comment
/// and every other byte survives. When the file cannot be scanned or the splice would break it,
/// the file is left alone and the caller gets the snippet to paste instead.
/// </summary>
public static class JsonConfig
{
    private const string CommentRefusal = "this file has comments, which rewriting would delete, so add the snippet by hand";

    private static readonly JsonSerializerOptions Pretty = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private static readonly JsonDocumentOptions Lenient = new()
    {
        CommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    /// <summary>Whether the text contains a // or /* */ comment outside strings.</summary>
    public static bool HasComments(string text)
    {
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(text), new JsonReaderOptions
        {
            CommentHandling = JsonCommentHandling.Allow,
            AllowTrailingCommas = true,
        });
        try
        {
            while (reader.Read())
                if (reader.TokenType == JsonTokenType.Comment) return true;
        }
        catch (JsonException)
        {
            // Broken JSON. The parse that follows refuses it with a clearer message.
        }
        return false;
    }

    /// <summary>The snippet a person pastes when the file cannot be edited automatically.</summary>
    public static string Snippet(string serversKey, ServerEntry entry, bool writeType) =>
        new JsonObject { [serversKey] = new JsonObject { [entry.Name] = entry.ToJson(writeType) } }.ToJsonString(Pretty);

    /// <summary>Adds or replaces the entry under <paramref name="serversKey"/>.</summary>
    public static WriteResult Upsert(string path, string serversKey, ServerEntry entry, bool writeType)
    {
        var wanted = entry.ToJson(writeType);
        if (File.Exists(path))
        {
            var text = File.ReadAllText(path);
            if (!string.IsNullOrWhiteSpace(text) && HasComments(text))
            {
                if (!TrySpliceCommentedJson(text, serversKey, entry.Name, wanted, out var spliced))
                    return new WriteResult(WriteOutcome.Refused, path, CommentRefusal);
                if (spliced == text) return new WriteResult(WriteOutcome.Unchanged, path, "already set up");
                ConfigFile.ReplaceWithBackup(path, spliced);
                return new WriteResult(WriteOutcome.Written, path, "added");
            }
        }

        if (!TryLoad(path, out var root, out var refusal)) return refusal!;
        var servers = root![serversKey] as JsonObject;
        if (root[serversKey] is not null && servers is null)
            return new WriteResult(WriteOutcome.Refused, path, $"\"{serversKey}\" in this file is not an object, so it was left alone");
        if (servers is null) { servers = new JsonObject(); root[serversKey] = servers; }

        if (servers[entry.Name] is JsonNode existing && JsonNode.DeepEquals(existing, wanted))
            return new WriteResult(WriteOutcome.Unchanged, path, "already set up");
        servers[entry.Name] = wanted;
        Save(path, root);
        return new WriteResult(WriteOutcome.Written, path, "added");
    }

    /// <summary>Removes the entry if present.</summary>
    public static WriteResult Remove(string path, string serversKey, string name)
    {
        if (!File.Exists(path)) return new WriteResult(WriteOutcome.NotPresent, path, "no config file");
        var text = File.ReadAllText(path);
        if (HasComments(text))
        {
            if (!TrySpliceCommentedJson(text, serversKey, name, null, out var spliced))
                return new WriteResult(WriteOutcome.Refused, path, CommentRefusal);
            if (spliced == text) return new WriteResult(WriteOutcome.NotPresent, path, "was not set up");
            ConfigFile.ReplaceWithBackup(path, spliced);
            return new WriteResult(WriteOutcome.Removed, path, "removed");
        }

        if (!TryLoad(path, out var root, out var refusal)) return refusal!;
        if (root![serversKey] is not JsonObject servers || !servers.ContainsKey(name))
            return new WriteResult(WriteOutcome.NotPresent, path, "was not set up");
        servers.Remove(name);
        Save(path, root);
        return new WriteResult(WriteOutcome.Removed, path, "removed");
    }

    /// <summary>Whether the file already lists the server.</summary>
    public static bool Contains(string path, string serversKey, string name)
    {
        try
        {
            if (!File.Exists(path)) return false;
            var root = JsonNode.Parse(File.ReadAllText(path), documentOptions: Lenient);
            return root?[serversKey] is JsonObject s && s.ContainsKey(name);
        }
        catch (JsonException) { return false; }
    }

    private static bool TryLoad(string path, out JsonObject? root, out WriteResult? refusal)
    {
        root = null; refusal = null;
        if (!File.Exists(path)) { root = new JsonObject(); return true; }
        var text = File.ReadAllText(path);
        if (string.IsNullOrWhiteSpace(text)) { root = new JsonObject(); return true; }
        if (HasComments(text))
        {
            refusal = new WriteResult(WriteOutcome.Refused, path, CommentRefusal);
            return false;
        }
        try
        {
            root = JsonNode.Parse(text, documentOptions: Lenient) as JsonObject;
        }
        catch (JsonException ex)
        {
            refusal = new WriteResult(WriteOutcome.Refused, path, $"this file is not valid JSON ({ex.Message}), so it was left alone");
            return false;
        }
        if (root is null)
        {
            refusal = new WriteResult(WriteOutcome.Refused, path, "this file does not hold a JSON object, so it was left alone");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Splices one entry into a file that holds comments, changing only the byte range of that
    /// entry so every comment and every other byte survives. Returns false when the text cannot
    /// be scanned or the result would not parse, so the caller can refuse instead.
    /// </summary>
    private static bool TrySpliceCommentedJson(string text, string serversKey, string entryName, JsonObject? newEntry, out string spliced)
    {
        spliced = text;
        byte[] bytes;
        var tokens = new List<JsonToken>();
        try
        {
            bytes = Encoding.UTF8.GetBytes(text);
            var reader = new Utf8JsonReader(bytes, new JsonReaderOptions
            {
                CommentHandling = JsonCommentHandling.Allow,
                AllowTrailingCommas = true,
            });
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.Comment) continue;
                tokens.Add(new JsonToken
                {
                    Type = reader.TokenType,
                    Start = (int)reader.TokenStartIndex,
                    End = (int)reader.BytesConsumed,
                    Depth = reader.CurrentDepth,
                    Name = reader.TokenType == JsonTokenType.PropertyName ? reader.GetString() : null,
                });
            }
        }
        catch (JsonException)
        {
            return false;
        }

        if (tokens.Count == 0 || tokens[0].Type != JsonTokenType.StartObject || tokens[0].Depth != 0)
            return false;

        int rootCloseIdx = -1;
        for (int i = tokens.Count - 1; i >= 0; i--)
        {
            if (tokens[i].Type == JsonTokenType.EndObject && tokens[i].Depth == 0) { rootCloseIdx = i; break; }
        }
        if (rootCloseIdx < 0) return false;

        int MatchEnd(int i)
        {
            int d = tokens[i].Depth;
            for (int j = i + 1; j < tokens.Count; j++)
                if ((tokens[j].Type == JsonTokenType.EndObject || tokens[j].Type == JsonTokenType.EndArray) && tokens[j].Depth == d)
                    return j;
            return -1;
        }

        int ValueEnd(int i)
        {
            var t = tokens[i];
            if (t.Type == JsonTokenType.StartObject || t.Type == JsonTokenType.StartArray)
            {
                int m = MatchEnd(i);
                return m < 0 ? -1 : tokens[m].End;
            }
            return t.End;
        }

        bool CommaAfter(int pos)
        {
            int p = pos;
            while (p < bytes.Length && IsJsonSpace(bytes[p])) p++;
            return p < bytes.Length && bytes[p] == (byte)',';
        }

        int serversNameIdx = -1;
        bool rootHasChild = false, rootLastComma = false;
        for (int i = 1; i < rootCloseIdx; i++)
        {
            if (tokens[i].Type != JsonTokenType.PropertyName || tokens[i].Depth != 1) continue;
            rootHasChild = true;
            int vend = ValueEnd(i + 1);
            rootLastComma = vend >= 0 && CommaAfter(vend);
            if (tokens[i].Name == serversKey) serversNameIdx = i;
        }

        if (serversNameIdx >= 0)
        {
            int sv = serversNameIdx + 1;
            if (tokens[sv].Type != JsonTokenType.StartObject || tokens[sv].Depth != 1) return false;
            int serversCloseIdx = MatchEnd(sv);
            if (serversCloseIdx < 0) return false;
            int serversClose = tokens[serversCloseIdx].Start;

            int entryNameIdx = -1;
            bool serversHasChild = false, serversLastComma = false;
            for (int i = sv + 1; i < serversCloseIdx; i++)
            {
                if (tokens[i].Type != JsonTokenType.PropertyName || tokens[i].Depth != 2) continue;
                serversHasChild = true;
                int vend = ValueEnd(i + 1);
                serversLastComma = vend >= 0 && CommaAfter(vend);
                if (tokens[i].Name == entryName) entryNameIdx = i;
            }

            if (entryNameIdx >= 0)
            {
                int ev = entryNameIdx + 1;
                int valueEnd = ValueEnd(ev);
                if (valueEnd < 0) return false;
                if (newEntry is null)
                {
                    int end = valueEnd;
                    if (CommaAfter(valueEnd))
                    {
                        end = valueEnd;
                        while (end < bytes.Length && IsJsonSpace(bytes[end])) end++;
                        end++;
                    }
                    spliced = Splice(bytes, tokens[entryNameIdx].Start, end, Array.Empty<byte>());
                }
                else
                {
                    if (NodesEqual(bytes, tokens[ev].Start, valueEnd, newEntry)) return true;
                    spliced = Splice(bytes, tokens[ev].Start, valueEnd, Encoding.UTF8.GetBytes(newEntry.ToJsonString(Pretty)));
                }
            }
            else if (newEntry is null)
            {
                return true;
            }
            else
            {
                var insert = (serversHasChild && !serversLastComma ? "," : "")
                    + JsonSerializer.Serialize(entryName, Pretty) + ": " + newEntry.ToJsonString(Pretty);
                spliced = Splice(bytes, serversClose, serversClose, Encoding.UTF8.GetBytes(insert));
            }
        }
        else if (newEntry is null)
        {
            return true;
        }
        else
        {
            var value = new JsonObject { [entryName] = newEntry }.ToJsonString(Pretty);
            var insert = (rootHasChild && !rootLastComma ? "," : "")
                + JsonSerializer.Serialize(serversKey, Pretty) + ": " + value;
            spliced = Splice(bytes, tokens[rootCloseIdx].Start, tokens[rootCloseIdx].Start, Encoding.UTF8.GetBytes(insert));
        }

        try
        {
            return JsonNode.Parse(spliced, documentOptions: Lenient) is not null;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool IsJsonSpace(byte b) =>
        b == (byte)' ' || b == (byte)'\t' || b == (byte)'\r' || b == (byte)'\n';

    private static bool NodesEqual(byte[] bytes, int start, int end, JsonObject expected)
    {
        try
        {
            var existing = JsonNode.Parse(Encoding.UTF8.GetString(bytes, start, end - start));
            return existing is not null && JsonNode.DeepEquals(existing, expected);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static string Splice(byte[] bytes, int start, int end, byte[] replacement)
    {
        var result = new byte[bytes.Length - (end - start) + replacement.Length];
        Array.Copy(bytes, 0, result, 0, start);
        Array.Copy(replacement, 0, result, start, replacement.Length);
        Array.Copy(bytes, end, result, start + replacement.Length, bytes.Length - end);
        return Encoding.UTF8.GetString(result);
    }

    private readonly struct JsonToken
    {
        public JsonTokenType Type { get; init; }
        public int Start { get; init; }
        public int End { get; init; }
        public int Depth { get; init; }
        public string? Name { get; init; }
    }

    private static void Save(string path, JsonObject root) =>
        ConfigFile.ReplaceWithBackup(path, root.ToJsonString(Pretty) + Environment.NewLine);
}

/// <summary>
/// Adds or removes one <c>[mcp_servers.NAME]</c> table in Codex's config.toml. Only the lines of that
/// table (and its <c>.env</c> sub-table) are touched, so comments and every other setting survive.
/// </summary>
public static class CodexToml
{
    /// <summary>The TOML block for the entry. Literal strings keep Windows backslashes as typed.</summary>
    public static string Block(ServerEntry entry)
    {
        var sb = new StringBuilder();
        sb.Append("[mcp_servers.").Append(entry.Name).Append("]\n");
        sb.Append("command = ").Append(Literal(entry.Command)).Append('\n');
        sb.Append("args = [").Append(string.Join(", ", entry.Args.Select(Literal))).Append("]\n");
        if (entry.GameDir is not null)
        {
            sb.Append('\n').Append("[mcp_servers.").Append(entry.Name).Append(".env]\n");
            sb.Append("WC3_GAME_DIR = ").Append(Literal(entry.GameDir)).Append('\n');
        }
        return sb.ToString();
    }

    public static WriteResult Upsert(string path, ServerEntry entry)
    {
        var lines = File.Exists(path) ? File.ReadAllText(path).Replace("\r\n", "\n").Split('\n').ToList() : new List<string>();
        var without = Strip(lines, entry.Name, out _);
        var block = Block(entry).TrimEnd('\n').Split('\n');
        while (without.Count > 0 && without[^1].Length == 0) without.RemoveAt(without.Count - 1);
        if (without.Count > 0) without.Add("");
        without.AddRange(block);
        var text = string.Join("\n", without) + "\n";
        if (File.Exists(path) && File.ReadAllText(path).Replace("\r\n", "\n") == text)
            return new WriteResult(WriteOutcome.Unchanged, path, "already set up");
        ConfigFile.ReplaceWithBackup(path, text);
        return new WriteResult(WriteOutcome.Written, path, "added");
    }

    public static WriteResult Remove(string path, string name)
    {
        if (!File.Exists(path)) return new WriteResult(WriteOutcome.NotPresent, path, "no config file");
        var lines = File.ReadAllText(path).Replace("\r\n", "\n").Split('\n').ToList();
        var without = Strip(lines, name, out var found);
        if (!found) return new WriteResult(WriteOutcome.NotPresent, path, "was not set up");
        while (without.Count > 0 && without[^1].Length == 0) without.RemoveAt(without.Count - 1);
        ConfigFile.ReplaceWithBackup(path, string.Join("\n", without) + "\n");
        return new WriteResult(WriteOutcome.Removed, path, "removed");
    }

    public static bool Contains(string path, string name) =>
        File.Exists(path) && File.ReadAllLines(path).Any(l => IsOwnHeader(l.Trim(), name));

    // Drops the server's own table and its sub-tables, up to the next unrelated table header.
    private static List<string> Strip(List<string> lines, string name, out bool found)
    {
        var kept = new List<string>();
        bool inside = false;
        found = false;
        foreach (var line in lines)
        {
            var t = line.Trim();
            if (t.StartsWith('['))
            {
                inside = IsOwnHeader(t, name);
                if (inside) found = true;
            }
            if (!inside) kept.Add(line);
        }
        return kept;
    }

    private static bool IsOwnHeader(string trimmed, string name) =>
        trimmed == $"[mcp_servers.{name}]" || trimmed.StartsWith($"[mcp_servers.{name}.", StringComparison.Ordinal);

    // A TOML literal string cannot hold a single quote, so fall back to a basic string with escapes.
    private static string Literal(string s) =>
        s.Contains('\'') ? "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"" : "'" + s + "'";
}

/// <summary>Writes a config file safely, keeping the previous version beside it.</summary>
public static class ConfigFile
{
    /// <summary>The suffix of the copy kept before each change, <c>.wc3ctl.bak</c> or <c>.wc3-mcp.bak</c>.</summary>
    public static string BackupSuffix { get; } = "." + ServerIdentity.ProductId + ".bak";

    /// <summary>
    /// Copies the current file to NAME plus <see cref="BackupSuffix"/>, writes the new text to a temp file in the
    /// same folder, then moves it into place, so a crash never leaves a half-written config.
    /// </summary>
    public static void ReplaceWithBackup(string path, string text)
    {
        var dir = Path.GetDirectoryName(Path.GetFullPath(path))!;
        Directory.CreateDirectory(dir);
        if (File.Exists(path)) File.Copy(path, path + BackupSuffix, overwrite: true);
        var tmp = Path.Combine(dir, "." + Path.GetFileName(path) + "." + Guid.NewGuid().ToString("N") + ".tmp");
        File.WriteAllText(tmp, text, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        File.Move(tmp, path, overwrite: true);
    }
}
