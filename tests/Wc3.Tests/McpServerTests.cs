// tests/Wc3.Tests/McpServerTests.cs
// Hermetic MCP protocol tests: the real server wiring (Wc3McpServer.CreateOptions,
// exactly what the stdio exe serves) driven by the SDK's own client over in-memory
// pipes — no processes, no network, no WC3 install.
using System.IO.Pipelines;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Wc3.Mcp;

namespace Wc3.Tests;

public class McpServerTests
{
    // Ordinal-sorted: the test compares against tools.OrderBy(..., StringComparer.Ordinal).
    // Underscore (0x5F) sorts before lowercase letters, so families group naturally.
    private static readonly string[] ExpectedTools =
    {
        "audit_map",
        "bundle_unit", "camera_add", "camera_list", "camera_remove",
        "camera_set", "deprotect_map", "file_get_text", "file_list", "file_set",
        "force_list", "force_set_flags", "list_files",
        "map_info", "map_info_get", "map_info_set", "new_map",
        "object_get", "object_list", "object_new", "object_set",
        "palette_doodad", "pathing_paint", "place_doodad", "place_item",
        "place_region", "place_start_location", "place_unit", "player_list",
        "player_set_force", "port_unit", "render_model", "replay_summary", "script_leaks", "sound_add",
        "sound_list", "sound_remove", "sound_set", "terrain_blight",
        "terrain_cliff", "terrain_deform", "terrain_paint", "terrain_ramp",
        "terrain_stats", "terrain_water", "trigger_catalog_describe", "trigger_catalog_list",
        "uabi_profile", "unit_abilities",
    };

    private static Task WithClient(Func<McpClient, CancellationToken, Task> body) => McpTestClient.WithClient(body);

    [Fact]
    public async Task Initialize_reports_server_identity_and_tools_list_exposes_every_tool()
    {
        await WithClient(async (client, ct) =>
        {
            Assert.Equal("wc3-mcp", client.ServerInfo.Name);
            var tools = await client.ListToolsAsync(cancellationToken: ct);
            Assert.Equal(ExpectedTools, tools.Select(t => t.Name).OrderBy(n => n, StringComparer.Ordinal).ToArray());
        });
    }

    [Fact]
    public async Task List_files_tool_call_round_trips_a_real_map_through_the_protocol()
    {
        var mapPath = Path.Combine(Path.GetTempPath(), $"wc3mcp-{Guid.NewGuid():N}.w3x");
        File.WriteAllBytes(mapPath, SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = new byte[] { 1, 2, 3 },
            ["mystery.bin"] = new byte[] { 9 },
        }));
        try
        {
            await WithClient(async (client, ct) =>
            {
                var result = await client.CallToolAsync("list_files",
                    new Dictionary<string, object?> { ["map"] = mapPath }, cancellationToken: ct);
                Assert.NotEqual(true, result.IsError);
                var text = Assert.IsType<TextContentBlock>(Assert.Single(result.Content)).Text;
                Assert.Contains("war3map.j", text);
                Assert.Contains("mystery.bin", text);
            });
        }
        finally
        {
            File.Delete(mapPath);
        }
    }

    [Fact]
    public async Task Missing_map_surfaces_as_clean_tool_error_not_a_stack_trace()
    {
        await WithClient(async (client, ct) =>
        {
            var result = await client.CallToolAsync("map_info",
                new Dictionary<string, object?> { ["map"] = @"Z:\nowhere\missing.w3x" }, cancellationToken: ct);
            Assert.Equal(true, result.IsError);
            var text = Assert.IsType<TextContentBlock>(Assert.Single(result.Content)).Text;
            Assert.Contains("map not found", text);
            Assert.DoesNotContain(" at ", text); // no stack frames leak to the client
        });
    }
}
