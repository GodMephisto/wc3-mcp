// src/Wc3.Mcp/McpToolCatalog.cs
using System.Text.Json;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace Wc3.Mcp;

/// <summary>
/// One tool exactly as the server serves it, for the read-only catalog dump and the audit.
/// A null hint means the tool left that annotation at its default, which is what the SDK does
/// when the attribute does not set the property. <see cref="Annotated"/> is true when at least
/// one hint is present, since the SDK omits the whole annotations object when every hint is a
/// default.
/// </summary>
public sealed record McpToolInfo(
    string Name,
    string? Description,
    bool? ReadOnly,
    bool? Destructive,
    bool? Idempotent,
    bool? OpenWorld)
{
    public bool Annotated =>
        ReadOnly is not null || Destructive is not null || Idempotent is not null || OpenWorld is not null;
}

/// <summary>One annotation problem, naming the tool and what is wrong with it.</summary>
public sealed record McpAnnotationProblem(string Tool, string Problem);

/// <summary>The audit outcome. An empty problem list means the surface is clean.</summary>
public sealed record McpAnnotationAuditResult(IReadOnlyList<McpAnnotationProblem> Problems)
{
    public int Count => Problems.Count;
}

/// <summary>
/// The read-only view of the tools the server builds. Both the catalog dump and the audit read
/// through <see cref="Wc3McpServer.CreateTools"/>, the same reflection the stdio server uses, so
/// neither can drift from what a client is actually offered.
/// </summary>
public static class McpToolCatalog
{
    /// <summary>Every tool, ordinal-sorted, with its description and four annotation hints.</summary>
    public static IReadOnlyList<McpToolInfo> Describe() =>
        Wc3McpServer.CreateTools()
            .Select(t => ToInfo(t.ProtocolTool))
            .OrderBy(i => i.Name, StringComparer.Ordinal)
            .ToList();

    internal static McpToolInfo ToInfo(Tool tool)
    {
        var a = tool.Annotations;
        return new McpToolInfo(
            tool.Name, tool.Description,
            a?.ReadOnlyHint, a?.DestructiveHint, a?.IdempotentHint, a?.OpenWorldHint);
    }

    /// <summary>
    /// The catalog as one read-only MCP resource, so a client can pull the whole surface in a
    /// single read instead of listing tools. Same reflection as the tools, so it cannot drift.
    /// </summary>
    public static McpServerResource CreateCatalogResource() =>
        McpServerResource.Create(
            () => JsonSerializer.Serialize(Describe(), Wc3McpServer.JsonOptions),
            new McpServerResourceCreateOptions
            {
                UriTemplate = "wc3://tools",
                Name = "tool_catalog",
                Description = "The MCP tool surface as JSON, every tool with its description and the four annotations.",
                MimeType = "application/json",
            });
}

/// <summary>
/// Audits the MCP surface against a naming convention. A tool whose name begins with a mutating
/// stem must not claim to be read-only, and every tool must declare at least one annotation.
/// The prefix list is deliberately precise rather than broad. A blanket 'port_' or 'terrain_'
/// would flag port_deps, port_preview and terrain_export_heightmap, which are genuinely
/// read-only, so each mutating stem is named instead.
/// </summary>
public static class McpToolAudit
{
    /// <summary>Name stems that only ever prefix a tool that writes. Kept in step with the surface.</summary>
    private static readonly string[] MutatingPrefixes =
    {
        "asset_replace", "camera_add", "camera_remove", "camera_set", "camera_tour",
        "file_set", "force_set_flags", "gamedata_export_schemas", "hero_install",
        "leak_harness", "lint_baseline", "map_info_set", "new_map",
        "object_clone", "object_new", "object_set",
        "pathing_import_png", "pathing_paint",
        "place_",
        "placed_doodad_remove", "placed_doodad_set", "placed_fix_refs",
        "placed_unit_remove", "placed_unit_set",
        "player_set_force", "port_into_version", "port_unit",
        "region_create", "region_remove", "script_format",
        "sound_add", "sound_remove", "sound_set", "strings_set", "target_set",
        "terrain_blight", "terrain_cliff", "terrain_corner_set", "terrain_deform",
        "terrain_fill", "terrain_import_heightmap", "terrain_paint", "terrain_ramp",
        "terrain_smooth", "terrain_water",
        "trigger_add", "trigger_recover_from_script", "trigger_remove", "trigger_rename",
        "trigger_set_code", "trigger_set_eca_enabled", "trigger_set_enabled",
        "trigger_set_initially_on",
        "trigger_set_run_on_map_init",
        "trigger_var_add", "trigger_var_remove", "trigger_var_set",
    };

    /// <summary>Audit the live server surface.</summary>
    public static McpAnnotationAuditResult InspectServer() => Inspect(McpToolCatalog.Describe());

    /// <summary>Audit a tool list, so the rules can be tested without touching the real surface.</summary>
    public static McpAnnotationAuditResult Inspect(IReadOnlyList<McpToolInfo> tools)
    {
        var problems = new List<McpAnnotationProblem>();
        foreach (var tool in tools.OrderBy(t => t.Name, StringComparer.Ordinal))
        {
            if (!tool.Annotated)
            {
                problems.Add(new McpAnnotationProblem(tool.Name, "declares no annotations"));
                continue;
            }
            if (tool.ReadOnly == true && MutatingPrefixes.Any(p => tool.Name.StartsWith(p, StringComparison.Ordinal)))
                problems.Add(new McpAnnotationProblem(tool.Name, "name looks mutating but it is marked read-only"));
        }
        return new McpAnnotationAuditResult(problems);
    }
}
