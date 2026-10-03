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
        "asset_list", "audit_ability", "audit_fidelity", "audit_hero", "audit_map",
        "audit_readiness", "bundle_unit", "camera_add", "camera_list", "camera_remove",
        "camera_set", "deprotect_map", "diff", "editor_catalog_get", "editor_catalog_list",
        "file_get_text", "file_list", "file_set", "force_list", "force_set_flags", "hero_install",
        "hero_lint", "hero_roster", "imports_list", "lint", "list_files", "map_info",
        "map_info_get", "map_info_set", "new_map", "object_field_options", "object_form",
        "object_get", "object_list", "object_new", "object_set", "palette_doodad", "pathing_paint",
        "place_doodad", "place_item", "place_region", "place_start_location", "place_unit",
        "placed_doodad_get", "placed_doodad_remove", "placed_doodad_set", "placed_doodads_list",
        "placed_unit_get", "placed_unit_remove", "placed_unit_set", "placed_units_list",
        "player_list", "player_set_force", "port_unit", "region_list", "region_remove",
        "render_model", "repair_generated", "replay_summary", "roundtrip", "script_functions",
        "script_leaks", "script_references", "search", "sound_add", "sound_list", "sound_remove",
        "sound_set", "strings_list", "terrain_blight", "terrain_cliff", "terrain_corner_get",
        "terrain_corner_set", "terrain_deform", "terrain_fill", "terrain_info", "terrain_paint",
        "terrain_ramp", "terrain_stats", "terrain_water", "trigger_add", "trigger_add_category",
        "trigger_add_eca", "trigger_catalog_describe", "trigger_catalog_list",
        "trigger_recover_from_script", "trigger_remove", "trigger_remove_eca", "trigger_rename",
        "trigger_set_eca_enabled", "trigger_set_enabled", "trigger_set_initially_on",
        "trigger_set_run_on_map_init", "triggers_read", "uabi_profile", "unit_abilities",
        "validate",
    };

    private static Task WithClient(Func<McpClient, CancellationToken, Task> body) => McpTestClient.WithClient(body);

    [Fact]
    public async Task Initialize_reports_server_identity_and_tools_list_exposes_every_tool()
    {
        await WithClient(async (client, ct) =>
        {
            Assert.Equal(ServerIdentity.ProductId, client.ServerInfo.Name);
            Assert.Equal(ServerIdentity.Version, client.ServerInfo.Version);
            var tools = await client.ListToolsAsync(cancellationToken: ct);
            Assert.Equal(ExpectedTools, tools.Select(t => t.Name).OrderBy(n => n, StringComparer.Ordinal).ToArray());
        });
    }

    [Fact]
    public void The_identity_comes_from_the_build_not_the_fallbacks()
    {
        // Both values must be written into the assembly by Wc3.Mcp.csproj. If the metadata went
        // missing, the fallbacks would quietly make the public build call itself wc3ctl.
        var metadata = typeof(ServerIdentity).Assembly
            .GetCustomAttributes(typeof(System.Reflection.AssemblyMetadataAttribute), false)
            .Cast<System.Reflection.AssemblyMetadataAttribute>()
            .ToDictionary(a => a.Key, a => a.Value);
        Assert.Equal(metadata["McpProductId"], ServerIdentity.ProductId);
        Assert.Equal(metadata["McpServerName"], ServerIdentity.ServerName);
        Assert.Equal("." + ServerIdentity.ProductId + ".bak", Wc3.Mcp.Setup.ConfigFile.BackupSuffix);
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
