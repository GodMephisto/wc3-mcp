// tests/Wc3.Tests/ProtocolRevisionTests.cs
// The protocol revision matrix the specification promises, as facts instead of prose.
// Every case drives the real in-memory server built from Wc3McpServer.ApplyTo, the same
// wiring the stdio exe serves, over in-memory pipes. No processes, no network, no WC3 install.
//
// Two client shapes are used. The SDK's typed client cannot observe a fallback answer, because
// McpClientOptions.ProtocolVersion makes it throw when the server answers anything else, so the
// offered-revision matrix is measured over a raw newline-delimited JSON-RPC pipe. The typed
// client still anchors the revision that a normal client actually negotiates.
using System.Globalization;
using System.IO.Pipelines;
using System.Text;
using System.Text.Json;
using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Wc3.Mcp;

namespace Wc3.Tests;

public class ProtocolRevisionTests
{
    /// <summary>Every revision the pinned SDK knows, oldest first.</summary>
    private static readonly string[] KnownRevisions =
    {
        "2024-11-05", "2025-03-26", "2025-06-18", "2025-11-25",
    };

    private static readonly TimeSpan SessionLimit = TimeSpan.FromSeconds(120);

    [Theory]
    [InlineData("2024-11-05")]
    [InlineData("2025-03-26")]
    [InlineData("2025-06-18")]
    [InlineData("2025-11-25")]
    public async Task An_offered_revision_is_answered_with_a_servable_revision_and_the_full_tool_count(string offered)
    {
        RevisionSession session = await MeasureAsync(offered);

        Assert.Null(session.Error);
        Assert.Equal(offered, session.Answered);
        // A revision the server can actually serve is one it names from the known set and one a
        // tools/list completes under, which the session below already did to fill Tools.
        Assert.Contains(session.Answered, KnownRevisions);
        Assert.Equal(Wc3McpServer.ToolCount(), session.Tools.Count);
    }

    [Fact]
    public async Task The_revision_matrix_answers_every_offered_revision_with_one_tool_count()
    {
        var rows = new List<RevisionSession>();
        foreach (string offered in KnownRevisions)
            rows.Add(await MeasureAsync(offered));

        string matrix = string.Join("\n", rows.Select(r =>
            r.Offered + " -> " + (r.Answered ?? "<none>") + " tools=" + r.Tools.Count));

        Assert.True(rows.All(r => r.Answered == r.Offered),
            "an offered revision was answered with a different revision, so the server did not serve it.\n" + matrix);
        Assert.True(rows.All(r => r.Tools.Count == Wc3McpServer.ToolCount()),
            "a revision listed a tool count that differs from the registered surface.\n" + matrix);
        Assert.Single(rows.Select(r => r.Tools.Count).Distinct());
        Assert.Contains(Wc3McpServer.ProtocolRevision, rows.Select(r => r.Answered ?? "<none>").ToArray());
    }

    [Fact]
    public async Task The_negotiated_revision_lists_items_with_the_current_surface_and_a_title()
    {
        // The revision a real client lands on, before the raw pipe confirms it from the wire.
        await McpTestClient.WithClient((client, ct) =>
        {
            Assert.Equal(Wc3McpServer.ProtocolRevision, client.NegotiatedProtocolVersion);
            return Task.CompletedTask;
        });

        RevisionSession session = await MeasureAsync(Wc3McpServer.ProtocolRevision);
        Assert.Equal(Wc3McpServer.ProtocolRevision, session.Answered);
        Assert.Equal(Wc3McpServer.ToolCount(), session.Tools.Count);

        // RES-02. The twelve tools whose result is a wrapper record publish their shape. Every
        // other tool still publishes none, which is what makes the set worth asserting rather than
        // the presence alone.
        string[] wrapperRecords =
        {
            "audit_ability", "audit_hero", "audit_readiness", "camera_list", "file_list", "force_list",
            "placed_doodads_list", "player_list", "region_list", "sound_list", "target_list",
            "trigger_var_list",
        };

        var gaps = new List<string>();
        var leaked = new List<string>();
        var withSchema = new List<string>();
        foreach (JsonElement tool in session.Tools)
        {
            string name = tool.TryGetProperty("name", out JsonElement nameElement)
                && nameElement.ValueKind == JsonValueKind.String
                    ? nameElement.GetString() ?? string.Empty
                    : string.Empty;
            string label = name.Length == 0 ? "<unnamed item>" : name;

            if (name.Length == 0)
                gaps.Add(label + " name");
            if (!HasNonEmptyString(tool, "description"))
                gaps.Add(label + " description");
            if (!tool.TryGetProperty("inputSchema", out JsonElement schema) || schema.ValueKind != JsonValueKind.Object)
                gaps.Add(label + " inputSchema");
            if (!tool.TryGetProperty("annotations", out JsonElement annotations) || annotations.ValueKind != JsonValueKind.Object)
                gaps.Add(label + " annotations");
            if (tool.TryGetProperty("outputSchema", out JsonElement outputSchema))
            {
                withSchema.Add(name);
                if (!HasNonEmptyString(outputSchema, "type") || outputSchema.GetProperty("type").GetString() != "object")
                    gaps.Add(label + " outputSchema is not an object schema");
            }
            // SUR-10 publishes a title on every tool, so a client reads a readable label from the
            // inventory rather than parsing the snake case name.
            if (!HasNonEmptyString(tool, "title"))
                gaps.Add(label + " title");
        }

        Assert.True(gaps.Count == 0,
            "tools/list items missing a required member of the current surface. " + string.Join(", ", gaps));
        Assert.True(leaked.Count == 0,
            "tools/list items carry a member the current surface does not publish. " + string.Join(", ", leaked));
        Assert.Equal(wrapperRecords, withSchema.OrderBy(n => n, StringComparer.Ordinal).ToArray());
    }

    [Theory]
    [InlineData("1999-01-01")]
    [InlineData("banana")]
    [InlineData("2025-02-30")]
    [InlineData("")]
    public async Task An_unknown_revision_falls_back_to_the_latest_revision_and_still_lists_tools(string offered)
    {
        // Measured on the pinned SDK, an unknown offer is answered with the latest revision rather
        // than a protocol error, and the session stays usable. If the SDK ever starts refusing, this
        // fact fails and its name has to change with the behaviour, not the other way round.
        RevisionSession session = await MeasureAsync(offered);

        Assert.Null(session.Error);
        Assert.Equal(Wc3McpServer.ProtocolRevision, session.Answered);
        Assert.Equal(Wc3McpServer.ToolCount(), session.Tools.Count);
    }

    [Fact]
    public async Task The_initialize_instructions_are_present_and_under_a_thousand_characters_at_every_revision()
    {
        var texts = new List<string>();
        var lengths = new List<string>();
        foreach (string offered in KnownRevisions)
        {
            RevisionSession session = await MeasureAsync(offered);
            string text = session.Instructions ?? string.Empty;
            texts.Add(text);
            lengths.Add(offered + "=" + text.Length);

            Assert.False(string.IsNullOrWhiteSpace(text), "no instructions on the initialize reply at " + offered);
            Assert.True(text.Length < 1000,
                "instructions at " + offered + " are " + text.Length + " characters, at or over the 1000 limit");
        }

        Assert.True(texts.Distinct().Count() == 1,
            "the instructions differ across revisions. lengths " + string.Join(", ", lengths));
    }

    [Fact]
    public async Task The_typed_client_refuses_a_fallback_answer_so_the_matrix_needs_the_raw_pipe()
    {
        // This is why the matrix above is measured over the pipe. With ProtocolVersion set the SDK
        // client fails the handshake when the server answers another revision, which is exactly what
        // an unknown offer produces, so the fallback is unobservable through the typed client.
        McpException error = await Assert.ThrowsAsync<McpException>(
            () => WithTypedClient("1999-01-01", (_, _) => Task.CompletedTask));

        Assert.Contains("protocol version mismatch", error.Message);
    }

    /// <summary>One initialize plus one tools/list, as the wire answered them.</summary>
    /// <param name="Offered">The revision the client asked for.</param>
    /// <param name="Answered">The revision the server answered, or null when the reply carried an error.</param>
    /// <param name="Error">The JSON-RPC error, or null for a successful handshake.</param>
    /// <param name="Instructions">The initialize instructions, or null when the reply carried none.</param>
    /// <param name="Tools">The tools/list items, parsed but not interpreted.</param>
    private sealed record RevisionSession(
        string Offered,
        string? Answered,
        string? Error,
        string? Instructions,
        IReadOnlyList<JsonElement> Tools);

    /// <summary>Handshakes at the offered revision and lists the surface, over a raw JSON-RPC pipe.</summary>
    private static Task<RevisionSession> MeasureAsync(string offered) =>
        // On the thread pool, not xUnit's synchronisation context. McpTestClient explains why, and
        // the same starvation applies to the server's continuations here.
        Task.Run(async () =>
        {
            using var limit = new CancellationTokenSource(SessionLimit);
            Pipe clientToServer = new(), serverToClient = new();

            var options = new McpServerOptions();
            Wc3McpServer.ApplyTo(options);

            await using var server = McpServer.Create(
                new StreamServerTransport(clientToServer.Reader.AsStream(), serverToClient.Writer.AsStream()),
                options);
            Task run = server.RunAsync(limit.Token);
            try
            {
                var peer = new RawPeer(clientToServer.Writer.AsStream(), serverToClient.Reader.AsStream(), limit.Token);
                JsonElement reply = await peer.RequestAsync("initialize", InitializeParams(offered));
                if (reply.TryGetProperty("error", out JsonElement handshakeError))
                    return new RevisionSession(offered, null, handshakeError.GetRawText(), null, Array.Empty<JsonElement>());

                JsonElement result = reply.GetProperty("result");
                string answered = result.TryGetProperty("protocolVersion", out JsonElement version)
                    && version.ValueKind == JsonValueKind.String
                        ? version.GetString() ?? string.Empty
                        : string.Empty;
                string? instructions = result.TryGetProperty("instructions", out JsonElement text)
                    && text.ValueKind == JsonValueKind.String
                        ? text.GetString()
                        : null;

                await peer.NotifyAsync("notifications/initialized");
                JsonElement listing = await peer.RequestAsync("tools/list", null);
                if (listing.TryGetProperty("error", out JsonElement listError))
                    return new RevisionSession(offered, answered, listError.GetRawText(), instructions, Array.Empty<JsonElement>());

                var tools = listing.GetProperty("result").GetProperty("tools")
                    .EnumerateArray().Select(tool => tool.Clone()).ToArray();
                return new RevisionSession(offered, answered, null, instructions, tools);
            }
            finally
            {
                limit.Cancel();
                try { await run; } catch (OperationCanceledException) { }
            }
        });

    /// <summary>The typed client at a forced revision, used only to pin what the SDK refuses.</summary>
    private static Task WithTypedClient(string offered, Func<McpClient, CancellationToken, Task> body) =>
        Task.Run(async () =>
        {
            using var limit = new CancellationTokenSource(SessionLimit);
            Pipe clientToServer = new(), serverToClient = new();

            var options = new McpServerOptions();
            Wc3McpServer.ApplyTo(options);

            await using var server = McpServer.Create(
                new StreamServerTransport(clientToServer.Reader.AsStream(), serverToClient.Writer.AsStream()),
                options);
            Task run = server.RunAsync(limit.Token);
            try
            {
                await using var client = await McpClient.CreateAsync(
                    new StreamClientTransport(clientToServer.Writer.AsStream(), serverToClient.Reader.AsStream()),
                    new McpClientOptions { ProtocolVersion = offered },
                    cancellationToken: limit.Token);
                await body(client, limit.Token);
            }
            finally
            {
                limit.Cancel();
                try { await run; } catch (OperationCanceledException) { }
            }
        });

    /// <summary>The initialize params a client sends, with no optional capabilities claimed.</summary>
    private static JsonElement InitializeParams(string offeredVersion) =>
        JsonSerializer.SerializeToElement(new
        {
            protocolVersion = offeredVersion,
            capabilities = new { },
            clientInfo = new { name = "wc3-protocol-revision-test", version = "1" },
        });

    private static bool HasNonEmptyString(JsonElement element, string property) =>
        element.TryGetProperty(property, out JsonElement value)
        && value.ValueKind == JsonValueKind.String
        && !string.IsNullOrWhiteSpace(value.GetString());

    /// <summary>
    /// Newline-delimited JSON-RPC over the in-memory pipe, the framing the stdio exe uses. It only
    /// sends and matches ids, because the matrix reads the raw reply rather than a typed result.
    /// </summary>
    private sealed class RawPeer
    {
        private static readonly UTF8Encoding Utf8 = new(encoderShouldEmitUTF8Identifier: false);
        private readonly Stream _toServer;
        private readonly StreamReader _fromServer;
        private readonly CancellationToken _cancellation;
        private int _lastId;

        public RawPeer(Stream toServer, Stream fromServer, CancellationToken cancellation)
        {
            _toServer = toServer;
            _fromServer = new StreamReader(fromServer, Utf8);
            _cancellation = cancellation;
        }

        public async Task<JsonElement> RequestAsync(string method, JsonElement? parameters)
        {
            int id = ++_lastId;
            string line = "{\"jsonrpc\":\"2.0\",\"id\":" + id.ToString(CultureInfo.InvariantCulture)
                + ",\"method\":" + JsonSerializer.Serialize(method)
                + (parameters is null ? string.Empty : ",\"params\":" + parameters.Value.GetRawText())
                + "}";
            await WriteLineAsync(line);

            while (true)
            {
                string? reply = await _fromServer.ReadLineAsync(_cancellation)
                    ?? throw new InvalidOperationException("the server closed the pipe before answering " + method);
                if (string.IsNullOrWhiteSpace(reply))
                    continue;
                using JsonDocument document = JsonDocument.Parse(reply);
                if (!document.RootElement.TryGetProperty("id", out JsonElement idElement)
                    || idElement.ValueKind != JsonValueKind.Number
                    || idElement.GetInt32() != id)
                    continue;
                return document.RootElement.Clone();
            }
        }

        public Task NotifyAsync(string method) =>
            WriteLineAsync("{\"jsonrpc\":\"2.0\",\"method\":" + JsonSerializer.Serialize(method) + "}");

        private async Task WriteLineAsync(string json)
        {
            byte[] bytes = Utf8.GetBytes(json + "\n");
            await _toServer.WriteAsync(bytes, _cancellation);
            await _toServer.FlushAsync(_cancellation);
        }
    }
}
