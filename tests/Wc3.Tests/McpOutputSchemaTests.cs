// tests/Wc3.Tests/McpOutputSchemaTests.cs
// RES-02. A tool whose result is a wrapper record publishes the shape of that record, so a client
// can validate a result and discover its fields without calling the tool, and the same object
// travels as structuredContent beside the text block that a client which reads no structured
// content still needs. At SDK 1.4.1 the schema is generated from the method's return type by
// McpServerToolAttribute.UseStructuredContent, so nothing here is hand written.
using System.Text.Json;
using System.Text.Json.Nodes;
using ModelContextProtocol.Protocol;
using Wc3.Model;

namespace Wc3.Tests;

public sealed class McpOutputSchemaTests : IDisposable
{
    /// <summary>The twelve tools whose result is a CappedPage wrapper record (RES-02). The list is
    /// written out rather than derived from the return type, so a tool that stops publishing one
    /// fails here instead of shrinking the expected set along with the measured one.</summary>
    private static readonly string[] WrapperRecordTools =
    {
        "audit_ability", "audit_hero", "audit_readiness", "camera_list", "file_list", "force_list",
        "placed_doodads_list", "player_list", "region_list", "sound_list", "target_list",
        "trigger_var_list",
    };

    private readonly string _dir = Path.Combine(Path.GetTempPath(), "wc3out-" + Guid.NewGuid().ToString("N"));

    public McpOutputSchemaTests() => Directory.CreateDirectory(_dir);

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch (IOException) { }
    }

    [Fact]
    public async Task Exactly_the_wrapper_record_tools_publish_an_output_schema()
    {
        var carrying = new List<string>();
        var malformed = new List<string>();

        await McpTestClient.WithClient(async (client, ct) =>
        {
            foreach (var tool in await client.ListToolsAsync(cancellationToken: ct))
            {
                JsonElement? schema = tool.ProtocolTool.OutputSchema;
                if (schema is null)
                    continue;

                carrying.Add(tool.Name);
                if (Problem(schema.Value) is { } problem)
                    malformed.Add(tool.Name + " " + problem);
            }
        });

        Assert.Equal(WrapperRecordTools, carrying.OrderBy(n => n, StringComparer.Ordinal).ToArray());
        Assert.True(malformed.Count == 0,
            "output schemas a client cannot use. " + string.Join(", ", malformed));
    }

    [Fact]
    public async Task A_wrapper_record_result_carries_the_same_object_as_text_and_structured_content()
    {
        string map = Path.Combine(_dir, "blank.w3x");
        File.WriteAllBytes(map, BlankMap.CreateArchiveBytes());

        bool textPresent = false;
        JsonNode? fromText = null;
        JsonNode? fromStructured = null;

        await McpTestClient.WithClient(async (client, ct) =>
        {
            var r = await client.CallToolAsync("player_list",
                new Dictionary<string, object?> { ["map"] = map }, cancellationToken: ct);
            string text = r.Content.OfType<TextContentBlock>().FirstOrDefault()?.Text ?? "";
            Assert.True(r.IsError != true, "player_list returned an error. " + text);

            textPresent = !string.IsNullOrWhiteSpace(text);
            fromText = JsonNode.Parse(text);
            fromStructured = r.StructuredContent is JsonElement element
                ? JsonNode.Parse(element.GetRawText())
                : null;
        });

        Assert.True(textPresent, "the text content a client that reads no structured content needs is gone");
        Assert.NotNull(fromStructured);
        Assert.True(JsonNode.DeepEquals(fromText, fromStructured),
            "structuredContent and the text content are not the same object");

        var wrapper = Assert.IsType<JsonObject>(fromStructured);
        // player_list is a capped wrapper, so it carries the cap it was held to and the count it
        // found. The four page names belong to the paged tools and are checked on list_files.
        foreach (string field in new[] { "cap", "truncated", "total", "items" })
            Assert.True(wrapper.ContainsKey(field), "structuredContent has no " + field);
    }

    /// <summary>Why a published schema is unusable, or null when it describes the wrapper record.
    /// The MCP revision this server speaks requires an object schema, and a caller that reads the
    /// schema needs the fields it will find in the result, the four RES-10 page names, the array and
    /// the three names this release keeps as aliases.</summary>
    private static string? Problem(JsonElement schema)
    {
        if (schema.ValueKind != JsonValueKind.Object)
            return "schema is not an object";
        if (!schema.TryGetProperty("type", out JsonElement type) || type.GetString() != "object")
            return "schema type is not object";
        if (!schema.TryGetProperty("properties", out JsonElement properties)
            || properties.ValueKind != JsonValueKind.Object)
            return "schema has no properties";

        var named = properties.EnumerateObject().Select(p => p.Name).OrderBy(n => n, StringComparer.Ordinal).ToArray();
        string[] expected =
        {
            "cap", "hasMore", "items", "nextOffset", "returnedCount", "total", "totalCount", "truncated",
        };
        return named.SequenceEqual(expected)
            ? null
            : "schema properties are " + string.Join(",", named);
    }
}
