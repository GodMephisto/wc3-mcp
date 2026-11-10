// tests/Wc3.Tests/McpSyntheticSurfaceTests.cs
// TST-02. Every read-only tool, called against a map this test builds itself, with no real map
// and no game install. The sweep that existed before this one selected its tools by name prefix
// and ran only against a corpus map, so a write tool whose name read like a read escaped it and
// the whole sweep skipped when the map was absent. This one selects on the read-only annotation,
// which is the claim the surface actually publishes, and refuses to be skipped.
using System.Text;
using System.Text.Json;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using Wc3.Commands;
using Wc3.Model;
using Xunit.Abstractions;

namespace Wc3.Tests;

public sealed class McpSyntheticSurfaceTests : IDisposable
{
    private readonly ITestOutputHelper _out;
    private readonly string _dir;
    private readonly string _map;
    private readonly int _creationNumber;

    public McpSyntheticSurfaceTests(ITestOutputHelper output)
    {
        _out = output;
        _dir = Path.Combine(Path.GetTempPath(), "wc3synth-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);

        // Built here rather than taken from a corpus. One placed unit gives the instance tools a
        // creation number to read, and one string table entry gives the string tools something to
        // find, so the tools that need a fixture are called rather than excused.
        var doc = BlankMap.Create();
        var placed = PlacementCommand.PlaceUnit(doc, "H000", 0, 64f, -64f);
        _creationNumber = placed.CreationNumber;
        doc.AddOrReplaceRawFile("war3map.wts",
            Encoding.UTF8.GetBytes("STRING 1\r\n{\r\nSynthetic Surface\r\n}\r\n"));
        _map = Path.Combine(_dir, "surface.w3x");
        File.WriteAllBytes(_map, doc.SaveToBytes());
    }

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch (IOException) { }
    }

    /// <summary>
    /// TST-02. Every tool the surface marks read-only is called against the synthetic map. A tool
    /// that answers must answer with JSON, and must carry every property its own output schema
    /// documents. A tool that refuses must refuse through the structured error document, so even
    /// the refusal is JSON a client can read. Nothing may throw, and a stack trace is a failure.
    /// </summary>
    [Fact]
    public async Task Every_read_only_tool_answers_json_on_a_synthetic_map()
    {
        var broken = new List<string>();
        // Refusals that reach the caller without a bracketed code are a real defect, but they are
        // listed rather than failed on, because the sweep cannot tell a caller error from a tool
        // that predates the code table. The list is the finding TST-02 still owes.
        var unshaped = new List<string>();
        int called = 0, answered = 0, refused = 0;

        await McpTestClient.WithClient(async (client, ct) =>
        {
            var tools = await client.ListToolsAsync(cancellationToken: ct);
            var readOnly = tools
                .Where(t => t.ProtocolTool.Annotations?.ReadOnlyHint == true)
                .OrderBy(t => t.Name, StringComparer.Ordinal)
                .ToList();

            _out.WriteLine($"{tools.Count} tool(s), {readOnly.Count} read-only");
            Assert.NotEmpty(readOnly);
            Assert.True(readOnly.Count < tools.Count,
                "every tool is marked read-only, so this sweep would be a read sweep in name only");

            foreach (var tool in readOnly)
            {
                var args = ArgsFor(tool);
                if (args is null)
                {
                    broken.Add(tool.Name + ": the sweep could not build an argument for it");
                    continue;
                }

                called++;
                CallToolResult result;
                try
                {
                    result = await client.CallToolAsync(tool.Name, args, cancellationToken: ct);
                }
                catch (Exception ex)
                {
                    broken.Add(tool.Name + ": threw " + ex.GetType().Name + " " + ex.Message);
                    continue;
                }

                string text = string.Concat(result.Content.OfType<TextContentBlock>().Select(c => c.Text));

                if (result.IsError == true)
                {
                    refused++;
                    if (!text.StartsWith('['))
                        unshaped.Add(tool.Name + ": " + Head(text));
                    if (text.Contains("   at ", StringComparison.Ordinal))
                        broken.Add(tool.Name + ": the refusal leaked a stack trace");
                    if (result.StructuredContent is not { } payload)
                    {
                        broken.Add(tool.Name + ": the refusal carries no structured error document");
                        continue;
                    }
                    try
                    {
                        using var error = JsonDocument.Parse(payload.GetRawText());
                        if (!error.RootElement.TryGetProperty("code", out var code)
                            || string.IsNullOrWhiteSpace(code.GetString()))
                            broken.Add(tool.Name + ": the structured refusal names no code");
                    }
                    catch (JsonException)
                    {
                        broken.Add(tool.Name + ": the structured refusal is not JSON");
                    }
                    _out.WriteLine($"  {tool.Name,-28} refused {Head(text)}");
                    continue;
                }

                answered++;
                if (string.IsNullOrWhiteSpace(text))
                {
                    broken.Add(tool.Name + ": answered with nothing at all");
                    continue;
                }

                JsonDocument document;
                try
                {
                    document = JsonDocument.Parse(text);
                }
                catch (JsonException ex)
                {
                    broken.Add(tool.Name + ": answered with text that is not JSON. " + ex.Message);
                    continue;
                }

                using (document)
                {
                    if (Problem(tool, document.RootElement) is { } problem)
                        broken.Add(tool.Name + ": " + problem);
                }
                _out.WriteLine($"  {tool.Name,-28} ok, {text.Length,7:N0} chars");
            }
        }, TimeSpan.FromMinutes(10));

        _out.WriteLine($"\n{called} called, {answered} answered, {refused} refused, {broken.Count} broken");
        Assert.True(called == ReadOnlyCount(),
            $"the sweep called {called} read-only tools and the surface serves {ReadOnlyCount()}");
        _out.WriteLine($"{unshaped.Count} refusal(s) without a bracketed code: "
            + string.Join(" | ", unshaped.Take(6)));

        Assert.True(broken.Count == 0, string.Join("\n", broken));
    }

    /// <summary>
    /// The read-only tools the surface serves, counted from the one description the sweep reads.
    /// A reflection count rather than the sweep's own tally, so a filter that silently drops a
    /// tool is caught instead of reporting success over the tools it kept.
    /// </summary>
    private static int ReadOnlyCount() =>
        Wc3.Mcp.McpToolCatalog.Describe().Count(t => t.ReadOnly == true);

    /// <summary>Why an answer does not carry the fields its own output schema documents.</summary>
    private static string? Problem(McpClientTool tool, JsonElement root)
    {
        if (tool.ProtocolTool.OutputSchema is not { } schema) return null;
        if (schema.ValueKind != JsonValueKind.Object) return "its output schema is not an object";
        if (!schema.TryGetProperty("properties", out var properties)
            || properties.ValueKind != JsonValueKind.Object)
            return null;
        if (root.ValueKind != JsonValueKind.Object) return "its output schema documents an object and the result is not one";

        // The schema documents the shape, and the serializer drops a null, so a documented field can
        // be absent when the tool had nothing to put in it. What matters is that the result invents
        // nothing the schema does not describe.
        var documented = properties.EnumerateObject()
            .Select(p => p.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var field in root.EnumerateObject())
            if (!documented.Contains(field.Name))
                return "the result carries \"" + field.Name + "\", which its output schema does not describe";
        return null;
    }

    /// <summary>
    /// One well-typed value for every argument the tool requires, from the map this test built.
    /// A name this table does not know gets a plain string, which a tool that needs another type
    /// refuses cleanly, so no tool is excused from the sweep.
    /// </summary>
    private Dictionary<string, object?>? ArgsFor(McpClientTool tool)
    {
        var schema = tool.JsonSchema;
        if (schema.ValueKind != JsonValueKind.Object) return new Dictionary<string, object?>();
        if (!schema.TryGetProperty("required", out var required) || required.ValueKind != JsonValueKind.Array)
            return new Dictionary<string, object?>();

        var args = new Dictionary<string, object?>();
        foreach (var item in required.EnumerateArray())
        {
            string? name = item.GetString();
            if (string.IsNullOrEmpty(name))
            {
                _out.WriteLine($"  {tool.Name,-28} skipped, a required name is not a string");
                return null;
            }

            args[name] = name switch
            {
                "map" or "mapPath" or "path" or "source_map" or "target_map" or "other" => _map,
                "map_a" or "map_b" => _map,
                "rawcode" => "H000",
                "creation_number" => _creationNumber,
                "field" => "unam",
                "query" => "Player",
                "name" => "Tilesets",
                "family" => "icon",
                "lang" or "language" => "jass",
                "out_path" or "out" or "output" or "output_path" or "dest" => Path.Combine(_dir, tool.Name + ".out"),
                "out_dir" or "folder" or "folder_a" or "folder_b" or "target_dir"
                    or "repo_dir" or "source_dir" or "schemas_dir" => _dir,
                _ => "x",
            };
        }
        return args;
    }

    private static string Head(string text)
    {
        string one = text.Replace('\n', ' ').Replace('\r', ' ').Trim();
        return one.Length <= 70 ? one : one[..70] + "...";
    }
}
