// tests/Wc3.Tests/McpSchemaFailureTests.cs
// ERR-06, the naming half. A refusal for a missing required argument says which argument was
// missing, because a model that reads only the tool name retries the same call and loses the turn.
// The check runs at the surface, where the schema is still in hand, so the name the client read in
// the schema is the name the refusal carries, in the text and in the value field of the payload.
using System.Text.Json;
using ModelContextProtocol.Protocol;

namespace Wc3.Tests;

public class McpSchemaFailureTests
{
    [Fact]
    public async Task A_schema_failure_names_what_was_missing()
    {
        string requiredNames = "";
        string message = "";
        string? value = null;

        await McpTestClient.WithClient(async (client, ct) =>
        {
            // map_info has one parameter and no default for it, so the schema names the argument
            // the caller had to pass.
            var mapInfo = (await client.ListToolsAsync(cancellationToken: ct)).First(t => t.Name == "map_info");
            requiredNames = string.Join(", ", mapInfo.JsonSchema.GetProperty("required")
                .EnumerateArray().Select(e => e.GetString()));

            // The call a model makes when it forgets the one argument the tool cannot run without.
            var r = await client.CallToolAsync("map_info",
                new Dictionary<string, object?>(), cancellationToken: ct);
            Assert.True(r.IsError == true, "a call with no arguments was accepted");
            message = r.Content.OfType<TextContentBlock>().FirstOrDefault()?.Text ?? "";

            // The same name travels as the value field, so a caller branches on it without reading
            // the sentence. The read is case folded, so a naming policy change cannot make this
            // test pass for the wrong reason.
            Assert.True(r.StructuredContent is not null, "the error result carried no structured content");
            value = r.StructuredContent!.Value.EnumerateObject()
                .First(p => string.Equals(p.Name, "value", StringComparison.OrdinalIgnoreCase))
                .Value.GetString();
        });

        Assert.Equal("map", requiredNames);
        Assert.False(string.IsNullOrWhiteSpace(message), "the refusal carried no text");

        // The message names the tool the caller used and the argument it left out, so the retry is
        // a different call rather than the same one again.
        Assert.StartsWith("[INVALID_ARGUMENT]", message, StringComparison.Ordinal);
        Assert.Contains("map_info", message);
        Assert.Contains("\"map\"", message);
        Assert.Equal("map", value);
    }
}
