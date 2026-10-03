// tests/Wc3.Tests/McpToolSweepTests.cs
using System.IO.Pipelines;
using System.Text.Json;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Wc3.Mcp;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Every read-only MCP tool, actually called, against a real map.
///
/// The server exposes 73 tools and NONE of them was ever invoked by a test. The parity test checks
/// the surface by reflection, that a CLI verb and an MCP tool exist in pairs, and the server test
/// calls two tools to prove the protocol wiring. What nobody checked is whether the other 71
/// return anything sensible, or throw, on a real map. MCP is one of the three front-ends this
/// project ships, and it was the only one with no behavioural coverage.
///
/// Read-only tools only. A sweep must not mutate the user's maps, so anything whose name implies a
/// write is excluded by name and the exclusion list is asserted to be non-empty, since a filter
/// that silently matches nothing is how a sweep reports success over an empty set.
/// </summary>
public class McpToolSweepTests
{
    private readonly ITestOutputHelper _out;
    public McpToolSweepTests(ITestOutputHelper output) => _out = output;

    private static string Dir => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "Warcraft III", "Maps", "Download");

    /// <summary>Verbs that change something. Never called here.</summary>
    private static readonly string[] MutatingPrefixes =
    {
        "camera_add", "camera_remove", "camera_set", "force_set", "map_info_set", "new_map",
        "object_set", "object_new", "place_", "port_", "hero_install", "script_repair",
        "script_strip", "strings_set", "sound_", "region_remove", "unit_set", "unit_remove",
        "doodad_set", "doodad_remove", "terrain_", "pathing_", "file_set", "convert",
        // These slipped through on the first run because the prefix list keyed off the
        // wrong stem, and they refused cleanly for want of an out_path rather than doing
        // anything. Named explicitly so the filter does not depend on a naming accident.
        "placed_unit_remove", "placed_unit_set", "placed_doodad_remove", "placed_doodad_set",
        "test_load", "trace_load", "debug_wiring", "repair", "bundle_",
    };

    /// <summary>
    /// Whether the sweep may call this tool. render_model stays in, deliberately. It writes a PNG
    /// to a temp path but never touches the map, and it is the tool that caught the image content
    /// block being emitted as raw bytes where base64 belongs.
    /// </summary>
    private static bool IsReadOnly(string tool) =>
        !MutatingPrefixes.Any(p => tool.StartsWith(p, StringComparison.Ordinal));

    private static async Task WithClient(Func<McpClient, CancellationToken, Task> body)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        Pipe clientToServer = new(), serverToClient = new();
        await using var server = McpServer.Create(
            new StreamServerTransport(clientToServer.Reader.AsStream(), serverToClient.Writer.AsStream()),
            Wc3McpServer.CreateOptions());
        Task run = server.RunAsync(timeout.Token);
        try
        {
            await using var client = await McpClient.CreateAsync(
                new StreamClientTransport(clientToServer.Writer.AsStream(), serverToClient.Reader.AsStream()),
                cancellationToken: timeout.Token);
            await body(client, timeout.Token);
        }
        finally
        {
            timeout.Cancel();
            try { await run; } catch (OperationCanceledException) { }
        }
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public async Task Every_read_only_tool_answers_on_a_real_map()
    {
        var map = Path.Combine(Dir, "Gem TD Inw Blitz 1.1.w3x");
        if (!File.Exists(map)) { _out.WriteLine("map absent, skipped"); return; }

        await WithClient(async (client, ct) =>
        {
            var tools = await client.ListToolsAsync(cancellationToken: ct);
            var readOnly = tools.Where(t => IsReadOnly(t.Name))
                .OrderBy(t => t.Name, StringComparer.Ordinal).ToList();

            _out.WriteLine($"{tools.Count} tool(s), {readOnly.Count} read-only");
            Assert.NotEmpty(readOnly);
            Assert.True(readOnly.Count < tools.Count,
                "the mutating filter matched nothing, so this would be sweeping writes too");

            var fx = FixturesFor(map);
            _out.WriteLine($"fixtures: rawcode={fx.Rawcode}, creation={fx.CreationNumber}");

            var broke = new List<string>();
            int answered = 0, refused = 0;

            foreach (var tool in readOnly)
            {
                // Supply only the arguments the schema actually declares, so a tool needing a
                // rawcode or a kind is skipped rather than called with nonsense.
                var args = ArgsFor(tool, map, fx, out var why);
                if (args is null)
                {
                    _out.WriteLine($"  {tool.Name,-24} skipped, {why}");
                    continue;
                }

                CallToolResult result;
                try
                {
                    result = await client.CallToolAsync(tool.Name, args, cancellationToken: ct);
                }
                catch (Exception ex)
                {
                    _out.WriteLine($"  {tool.Name,-24} THREW {ex.GetType().Name}: {ex.Message}");
                    broke.Add($"{tool.Name}: threw {ex.GetType().Name} {ex.Message}");
                    continue;
                }

                var text = string.Concat(result.Content.OfType<TextContentBlock>().Select(c => c.Text));
                bool isError = result.IsError == true;
                if (isError)
                {
                    refused++;
                    _out.WriteLine($"  {tool.Name,-24} error: {Trim(text)}");
                    // A clean refusal is acceptable, a stack trace is not. That distinction is the
                    // whole point of an error channel.
                    if (text.Contains("   at ", StringComparison.Ordinal))
                        broke.Add($"{tool.Name}: leaked a stack trace");
                    continue;
                }

                answered++;
                if (string.IsNullOrWhiteSpace(text))
                    broke.Add($"{tool.Name}: answered with nothing at all");
                _out.WriteLine($"  {tool.Name,-24} ok, {text.Length,7:N0} chars  {Trim(text)}");
            }

            _out.WriteLine($"\n{answered} answered, {refused} clean refusal(s), {broke.Count} broken");
            Assert.True(broke.Count == 0, string.Join("\n", broke));
        });
    }

    private static string Trim(string s)
    {
        var one = s.Replace('\n', ' ').Replace('\r', ' ').Trim();
        return one.Length <= 58 ? one : one[..58] + "...";
    }

    /// <summary>
    /// Arguments for a tool, filled from its own declared schema with real values taken from the
    /// map. Only REQUIRED arguments are supplied.
    /// </summary>
    /// <remarks>
    /// Passing optional arguments too was the first version's mistake and it manufactured a
    /// failure. "kind" means unit|item|ability to the object tools and event|condition|action to
    /// the trigger catalog, so blanket-assigning "unit" made trigger_catalog_list refuse. The tool
    /// was right to refuse, and the sweep was wrong to ask. Required-only also exercises each
    /// tool's default path, which is the one an agent hits first.
    /// </remarks>
    private Dictionary<string, object?>? ArgsFor(
        McpClientTool tool, string map, Fixtures fx, out string why)
    {
        why = "";
        var schema = tool.JsonSchema;
        if (schema.ValueKind != JsonValueKind.Object)
            return new Dictionary<string, object?>();

        var required = new List<string>();
        if (schema.TryGetProperty("required", out var req) && req.ValueKind == JsonValueKind.Array)
            foreach (var r in req.EnumerateArray())
                if (r.GetString() is { } name) required.Add(name);

        var args = new Dictionary<string, object?>();
        var unmet = new List<string>();
        foreach (var name in required)
        {
            object? value = name switch
            {
                "map" or "mapPath" or "path" or "source_map" or "target_map" or "other" => map,
                "rawcode" => fx.Rawcode,
                // The doodad tools index a different sequence from the unit tools.
                "creation_number" => tool.Name.Contains("doodad", StringComparison.Ordinal)
                    ? fx.DoodadCreationNumber
                    : fx.CreationNumber,
                "field" => "ugol",
                "query" => fx.Query,
                "family" => "icon",
                // "name" means a catalog to editor_catalog_get and a trigger function to
                // trigger_catalog_describe.
                "name" => tool.Name.StartsWith("trigger_catalog", StringComparison.Ordinal)
                    ? fx.TriggerFunction
                    : fx.CatalogName,
                "out_path" => Path.Combine(Path.GetTempPath(),
                                           "wc3ctl-mcp-sweep-" + tool.Name + ".png"),
                _ => null,
            };
            if (value is null) unmet.Add(name);
            else args[name] = value;
        }

        if (unmet.Count > 0)
        {
            why = "needs " + string.Join(", ", unmet);
            return null;
        }
        return args;
    }

    /// <summary>Real values from the map, so a required argument is answered with something the
    /// map actually contains rather than a guess.</summary>
    private sealed record Fixtures(
        string? Rawcode, int? CreationNumber, int? DoodadCreationNumber,
        string Query, string CatalogName, string TriggerFunction);

    private static Fixtures FixturesFor(string map)
    {
        var doc = Wc3.Model.MapDocument.Load(map);
        var rawcode = Wc3.Commands.ObjectListCommand
            .Execute(doc, Wc3.Commands.ObjectKind.Unit, (string?)null)
            .Items.FirstOrDefault()?.Rawcode;
        var creation = Wc3.Commands.UnitInstanceCommand.List(doc)
            .Select(u => (int?)u.CreationNumber).FirstOrDefault();
        // A doodad creation number is its OWN sequence. Feeding a unit's number to
        // placed_doodad_get made it refuse with "no placed doodad", correctly, and that refusal
        // was the fixture's fault rather than a finding.
        var doodad = Wc3.Commands.DoodadInstanceCommand.List(doc)
            .Select(d => (int?)d.CreationNumber).FirstOrDefault();
        return new Fixtures(rawcode, creation, doodad, "Player", "TileSets", "DoNothing");
    }
}
