// tests/Wc3.Tests/McpServerTests.cs
// Hermetic MCP protocol tests: the real server wiring (Wc3McpServer.CreateOptions,
// exactly what the stdio exe serves) driven by the SDK's own client over in-memory
// pipes, no processes, no network, no WC3 install.
using System.IO.Pipelines;
using System.Text.Json;
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
        "asset_audit", "asset_convert_batch", "asset_duplicates", "asset_import_wire", "asset_list",
        "asset_model_lint", "asset_replace", "asset_report", "asset_size_report", "asset_thumbnails", "asset_unused",
        "audit_ability", "audit_ability_orders", "audit_fidelity", "audit_hero", "audit_map",
        "audit_readiness", "audit_trend", "bench_compat", "bench_leaks", "bench_open",
        "bench_perf_gate", "bench_schema_coverage", "bundle_diff", "bundle_merge", "bundle_unit",
        "camera_add", "camera_list", "camera_remove", "camera_set", "camera_tour", "changelog",
        "ci_all", "ci_compat", "ci_lint", "ci_schema", "ci_validate",
        "compat_explain",
        "compat_matrix", "content_scaffold", "deprotect_map", "diff", "docs_search", "doctor_check", "doctor_compare", "doctor_fix",
        "doctor_inspect", "editor_catalog_get", "editor_catalog_list", "error_codes",
        "file_extract", "file_get_text", "file_list", "file_set", "force_list", "force_rename",
        "force_set_flags", "force_set_players",
        "gamedata_bounds", "gamedata_changelog", "gamedata_coverage", "gamedata_enum", "gamedata_export_schemas",
        "gamedata_field_lookup", "gamedata_history", "gamedata_schema_diff", "gamedata_sources",
        "graph_hubs", "graph_neighbors", "graph_path",
        "hero_generate", "hero_install", "hero_lint", "hero_roster",
        "imports_list", "leak_ab", "leak_benchmark", "leak_harness", "leak_join", "leak_list",
        "leak_model",
        "library_audit", "library_compare", "library_report", "library_scan", "library_validate",
        "lint", "lint_baseline", "list_files", "map_compat", "map_info",
        "map_info_get", "map_info_set", "map_optimize", "map_overview",
        "mcp_audit_annotations",
        "mcp_export_verify",
        "model_audit",
        "model_compare",
        "model_export_obj",
        "model_import",
        "model_lint",
        "model_lod",
        "model_optimize", "model_primitive",
        "model_squish",
        "mpq_add", "mpq_bytes_diff", "mpq_extract_all", "mpq_listfile", "mpq_merge",
        "mpq_name_recover", "mpq_rebuild", "mpq_remove", "mpq_remove_many",
        "mpq_strip_bookkeeping", "mpq_verify", "new_map", "object_batch_set", "object_bulk_set", "object_clone", "object_deprecated",
        "object_export", "object_field_options", "object_fix_model_ext", "object_form", "object_get", "object_import", "object_list",
        "object_migrate", "object_new", "object_noop_fields", "object_rawcode_suggest", "object_rename",
        "object_search", "object_set",
        "palette_doodad", "pathing_export_png", "pathing_import_png", "pathing_paint",
        "pathing_validate",
        "place_doodad", "place_grid", "place_item", "place_region", "place_scatter",
        "place_start_location", "place_unit",
        "placed_doodad_get", "placed_doodad_remove", "placed_doodad_set", "placed_doodads_list",
        "placed_fix_refs", "placed_missing_refs", "placed_stats",
        "placed_unit_get", "placed_unit_remove", "placed_unit_set", "placed_units_list",
        "player_list", "player_set_color", "player_set_force", "player_set_name", "player_set_race",
        "port_deps", "port_into_version", "port_preview",
        "port_unit", "quickstart", "rag_install", "rag_status", "region_create", "region_list", "region_remove",
        "region_to_rect",
        "region_validate",
        "render_model", "repair_audit_errors", "repair_generated", "repair_portraits",
        "replay_diff", "replay_find_map", "replay_players",
        "replay_stats", "replay_summary", "replay_timeline", "roundtrip",
        "script_api_usage", "script_bench", "script_callgraph",
        "script_check", "script_complexity", "script_conversion_notes", "script_decompile",
        "script_duplicate_bodies",
        "script_format", "script_functions",
        "script_leaks", "script_lua_api", "script_lua_check", "script_lua_globals",
        "script_lua_leaks", "script_lua_to_jass", "script_modernize", "script_null_locals",
        "script_reachable", "script_references", "script_semantic_diff", "script_to_lua",
        "script_unused_functions", "script_unused_globals", "script_verify", "search", "sound_add", "sound_import_events", "sound_list",
        "sound_optimize", "sound_remove", "sound_set", "strings_diff", "strings_find", "strings_list",
        "strings_missing", "strings_set", "strings_unused",
        "target_get", "target_list", "target_set",
        "terrain_audit", "terrain_blight", "terrain_cliff", "terrain_corner_get", "terrain_corner_set",
        "terrain_deform", "terrain_export_heightmap", "terrain_fill", "terrain_import_heightmap",
        "terrain_info", "terrain_optimize", "terrain_paint", "terrain_ramp", "terrain_smooth",
        "terrain_stats", "terrain_water", "texture_atlas", "texture_optimize", "trigger_add", "trigger_add_category",
        "trigger_add_eca", "trigger_catalog_describe", "trigger_catalog_list", "trigger_dead",
        "trigger_diff", "trigger_export", "trigger_lint", "trigger_move", "trigger_recover_from_script",
        "trigger_remove", "trigger_remove_eca", "trigger_rename",
        "trigger_set_code", "trigger_set_eca_enabled", "trigger_set_enabled", "trigger_set_initially_on",
        "trigger_set_run_on_map_init", "trigger_sync_bodies", "trigger_to_script", "trigger_var_add", "trigger_var_list",
        "trigger_var_remove", "trigger_var_set", "triggers_read", "uabi_profile",
        "unit_abilities",
        "validate", "validate_strict", "version_info",
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
    public async Task Initialize_carries_the_server_instructions_with_the_target_line()
    {
        // The initialize reply must carry BuildInstructions (the "target ..." line), which only
        // reaches a client when Program copies ServerInstructions onto the host options. Before
        // that copy the field was empty.
        await WithClient(async (client, ct) =>
        {
            Assert.False(string.IsNullOrWhiteSpace(client.ServerInstructions));
            Assert.Contains("target", client.ServerInstructions);
            await Task.CompletedTask;
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
    public async Task Tool_catalog_resource_is_served_as_json()
    {
        // The resource exposes the same catalog the audit and the dump use, so a client can pull
        // the whole surface in one read instead of listing tools.
        await WithClient(async (client, ct) =>
        {
            var resources = await client.ListResourcesAsync(cancellationToken: ct);
            Assert.Contains(resources, r => r.Uri == "wc3://tools");

            var contents = await client.ReadResourceAsync("wc3://tools", cancellationToken: ct);
            var text = Assert.IsType<TextResourceContents>(Assert.Single(contents.Contents)).Text;
            Assert.Contains("\"name\":\"map_overview\"", text);
            Assert.Contains("\"name\":\"mcp_audit_annotations\"", text);
        });
    }

    [Fact]
    public async Task The_catalog_lists_every_prompt_with_its_title_and_arguments()
    {
        var expected = Wc3McpServer.CreatePrompts()
            .Select(p => p.ProtocolPrompt.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToArray();

        await WithClient(async (client, ct) =>
        {
            var contents = await client.ReadResourceAsync("wc3://tools", cancellationToken: ct);
            var text = Assert.IsType<TextResourceContents>(Assert.Single(contents.Contents)).Text;
            using var doc = JsonDocument.Parse(text);
            var prompts = doc.RootElement.GetProperty("prompts").EnumerateArray().ToList();

            Assert.Equal(expected, prompts.Select(p => p.GetProperty("name").GetString())
                .OrderBy(n => n, StringComparer.Ordinal).ToArray());
            Assert.All(prompts, p =>
            {
                Assert.False(string.IsNullOrWhiteSpace(p.GetProperty("title").GetString()),
                    "a prompt was listed without a title");
                Assert.False(string.IsNullOrWhiteSpace(p.GetProperty("description").GetString()),
                    "a prompt was listed without a description");
                var arguments = p.GetProperty("arguments").EnumerateArray().Select(a => a.GetString()).ToList();
                Assert.NotEmpty(arguments);
                Assert.All(arguments, a => Assert.False(string.IsNullOrWhiteSpace(a)));
            });
        });
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

    [Fact]
    public async Task Built_in_prompts_are_served_and_one_round_trips_through_the_protocol()
    {
        await WithClient(async (client, ct) =>
        {
            var prompts = await client.ListPromptsAsync(cancellationToken: ct);
            var names = prompts.Select(p => p.Name).OrderBy(n => n, StringComparer.Ordinal).ToArray();
            // The original five must stay served. The set may grow, and a prompt that
            // disappears breaks a caller's saved workflow, so assert inclusion.
            var required = new[]
            {
                "audit_hero_spells", "inspect_map", "optimize_map_size",
                "port_hero", "repair_reforged_map",
            };
            Assert.All(required, name => Assert.Contains(name, names));

            var result = await client.GetPromptAsync("inspect_map",
                new Dictionary<string, object?> { ["map"] = "sample.w3x" }, cancellationToken: ct);
            var text = Assert.IsType<TextContentBlock>(Assert.Single(result.Messages).Content).Text;
            Assert.Contains("map_overview", text);
            Assert.Contains("sample.w3x", text);
        });
    }

    [Fact]
    public async Task Context_documents_are_served_as_resources_and_searchable()
    {
        // The two community references are the context injector. A resource returns a document
        // whole, and docs_search finds a passage in it, so a client configures nothing.
        await WithClient(async (client, ct) =>
        {
            var resources = await client.ListResourcesAsync(cancellationToken: ct);
            Assert.Contains(resources, r => r.Uri == "wc3://context/abilities");
            Assert.Contains(resources, r => r.Uri == "wc3://context/ui-frames");

            var contents = await client.ReadResourceAsync("wc3://context/abilities", cancellationToken: ct);
            var text = Assert.IsType<TextResourceContents>(Assert.Single(contents.Contents)).Text;
            Assert.Contains("Aerial Shackles", text);

            var result = await client.CallToolAsync("docs_search",
                new Dictionary<string, object?> { ["query"] = "Aerial Shackles", ["max"] = 3 }, cancellationToken: ct);
            Assert.NotEqual(true, result.IsError);
            var body = Assert.IsType<TextContentBlock>(Assert.Single(result.Content)).Text;
            Assert.Contains("Aerial Shackles", body);
        });
    }

    [Fact]
    public async Task Every_resource_reports_a_byte_size_and_the_sha256_of_the_file_it_serves()
    {
        // RSC-01, RSC-05 and RSC-08. The listing must name the size and the digest of what a read
        // returns, computed at start from the installed file. A section serves a slice of a
        // document, so its reference is the slice itself rather than the file.
        await WithClient(async (client, ct) =>
        {
            var resources = await client.ListResourcesAsync(cancellationToken: ct);
            Assert.NotEmpty(resources);

            foreach (var resource in resources)
            {
                var protocol = resource.ProtocolResource;
                Assert.NotNull(protocol);
                Assert.True(protocol!.Size > 0, resource.Uri + " reports no byte size");
                Assert.NotNull(protocol.Meta);
                string sha = protocol.Meta!["sha256"]!.GetValue<string>();
                Assert.Matches("^[0-9a-f]{64}$", sha);

                if (resource.Uri == "wc3://tools")
                {
                    string json = await ReadResourceText(client, resource.Uri, ct);
                    var bytes = System.Text.Encoding.UTF8.GetBytes(json);
                    Assert.Equal(Sha256Hex(bytes), sha);
                    Assert.Equal((long)bytes.Length, protocol!.Size!.Value);
                    continue;
                }

                if (resource.Uri.StartsWith("wc3://skills/", StringComparison.Ordinal))
                {
                    string id = resource.Uri["wc3://skills/".Length..];
                    string path = Wc3Skills.Locate(id);
                    byte[] bytes = File.ReadAllBytes(path);
                    Assert.Equal(Sha256Hex(bytes), sha);
                    Assert.Equal((long)bytes.Length, protocol!.Size!.Value);
                    Assert.Equal(File.ReadAllText(path), await ReadResourceText(client, resource.Uri, ct));
                    continue;
                }

                string[] parts = resource.Uri.Split('/');
                Assert.True(parts.Length is 4 or 5, resource.Uri + " is not a document or section uri");
                Assert.Equal("context", parts[2]);
                string doc = parts[3];
                if (parts.Length == 4)
                {
                    string file = Assert.Single(Directory.GetFiles(
                        Path.Combine(AppContext.BaseDirectory, "context", doc), "*.md"));
                    byte[] bytes = File.ReadAllBytes(file);
                    Assert.Equal(Sha256Hex(bytes), sha);
                    Assert.Equal((long)bytes.Length, protocol!.Size!.Value);
                }
                else
                {
                    string section = await ReadResourceText(client, resource.Uri, ct);
                    var bytes = System.Text.Encoding.UTF8.GetBytes(section);
                    Assert.Equal(Sha256Hex(bytes), sha);
                    Assert.Equal((long)bytes.Length, protocol!.Size!.Value);
                }
            }
        });
    }

    [Fact]
    public async Task A_section_resource_returns_that_section_only()
    {
        // RSC-02. A client that wants one ability or one introductory section reads the section
        // uri, not the whole 300KB document.
        await WithClient(async (client, ct) =>
        {
            var resources = await client.ListResourcesAsync(cancellationToken: ct);
            Assert.Contains(resources, r => r.Uri == "wc3://context/abilities/aerial-shackles");

            string section = await ReadResourceText(client, "wc3://context/abilities/aerial-shackles", ct);
            Assert.Contains("Aerial Shackles", section);
            Assert.DoesNotContain("Barrage", section);
            Assert.DoesNotContain("Warcraft 3 Editor Ability Insight", section);

            string whole = await ReadResourceText(client, "wc3://context/abilities", ct);
            Assert.Contains("Barrage", whole);
            Assert.Contains("Warcraft 3 Editor Ability Insight", whole);
            Assert.True(whole.Length > section.Length * 10);

            Assert.Contains(resources, r => r.Uri == "wc3://context/ui-frames/introduction");
            string intro = await ReadResourceText(client, "wc3://context/ui-frames/introduction", ct);
            Assert.Contains("Frames can display Text", intro);
            Assert.DoesNotContain("The Big UI-Frame Tutorial", intro);
            Assert.DoesNotContain("The nativ GUI-TriggerData", intro);
        });
    }

    [Fact]
    public async Task The_catalog_carries_an_aliases_array_and_an_input_schema_for_every_tool()
    {
        // RSC-03. A client that reads only wc3://tools can see each tool's arguments, so it need
        // not call tools/list to learn the shape of a call.
        await WithClient(async (client, ct) =>
        {
            var tools = await client.ListToolsAsync(cancellationToken: ct);
            string text = await ReadResourceText(client, "wc3://tools", ct);

            using var doc = JsonDocument.Parse(text);
            var catalog = doc.RootElement.GetProperty("tools").EnumerateArray().ToList();
            Assert.Equal(tools.Count, catalog.Count);
            Assert.NotEmpty(catalog);

            Assert.All(catalog, tool =>
            {
                var aliases = tool.GetProperty("aliases");
                Assert.Equal(JsonValueKind.Array, aliases.ValueKind);

                // SUR-03. A tool lists the old names that still answer to it, and only those. The
                // shipped table is empty, so every entry is empty today and a rename adds its old
                // name here rather than a row of its own.
                string name = tool.GetProperty("name").GetString() ?? "";
                var expected = McpToolCatalog.Renames.Values
                    .Where(a => a.CurrentName == name)
                    .Select(a => a.OldName)
                    .OrderBy(n => n, StringComparer.Ordinal)
                    .ToArray();
                Assert.Equal(expected, aliases.EnumerateArray()
                    .Select(a => a.GetString() ?? "")
                    .OrderBy(n => n, StringComparer.Ordinal)
                    .ToArray());

                var schema = tool.GetProperty("inputSchema");
                Assert.Equal(JsonValueKind.Object, schema.ValueKind);
                Assert.True(schema.TryGetProperty("type", out var type));
                Assert.Equal("object", type.GetString());
            });
        });
    }

    private static async Task<string> ReadResourceText(McpClient client, string uri, CancellationToken ct)
    {
        var contents = await client.ReadResourceAsync(uri, cancellationToken: ct);
        return Assert.IsType<TextResourceContents>(Assert.Single(contents.Contents)).Text;
    }

    private static string Sha256Hex(byte[] bytes) =>
        Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(bytes)).ToLowerInvariant();
}
