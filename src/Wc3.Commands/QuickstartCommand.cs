// src/Wc3.Commands/QuickstartCommand.cs
namespace Wc3.Commands;

/// <summary>
/// One guided workflow. <see cref="Id"/> is a short stable key, <see cref="Title"/> is the human
/// name, <see cref="WhenToUse"/> says in one line when to reach for it, and the two lists name the
/// MCP tools and the wc3ctl commands that carry the workflow out. The lists are pointers rather
/// than logic, so a caller resolves each name through the surface it already has.
/// </summary>
public sealed record QuickstartWorkflow(
    string Id,
    string Title,
    string WhenToUse,
    IReadOnlyList<string> McpTools,
    IReadOnlyList<string> CliCommands);

/// <summary>
/// The workflow router behind the <c>quickstart</c> MCP tool and the <c>wc3ctl quickstart</c>
/// command. With more than two hundred tools on the surface, an assistant asked to do something
/// broad benefits from a short list of the ten routes that cover most real work, rather than from
/// guessing which tool to call first.
///
/// Nothing here touches a map. It returns the same fixed catalog on every machine, so it is a
/// read-only reference rather than a resolver, and a goal narrows or ranks the list without ever
/// hiding a route the caller asked for by name.
/// </summary>
public static class QuickstartCommand
{
    /// <summary>The ten core workflows, in the order a newcomer meets them.</summary>
    public static IReadOnlyList<QuickstartWorkflow> All { get; } = new[]
    {
        new QuickstartWorkflow(
            "inspect",
            "Inspect a map",
            "Learn what a map is before changing anything, its metadata, files and custom objects.",
            new[] { "map_overview", "map_info", "list_files", "object_list", "audit_map" },
            new[] { "overview", "info", "ls", "object list", "audit" }),

        new QuickstartWorkflow(
            "port-hero",
            "Port a hero or unit between maps",
            "Move a unit or hero and everything it depends on into another map.",
            new[]
            {
                "port_deps", "port_preview", "port_unit", "port_into_version",
                "hero_roster", "audit_hero",
            },
            new[]
            {
                "port deps", "port preview", "port unit", "port into-version",
                "hero roster", "audit hero",
            }),

        new QuickstartWorkflow(
            "reforged-3-fix",
            "Repair a map broken by a Reforged patch",
            "A map that loaded before a patch and no longer does, or whose generated blocks went stale.",
            new[] { "repair_generated", "map_optimize", "map_compat", "compat_explain" },
            new[] { "repair-generated", "map optimize", "compat", "compat explain" }),

        new QuickstartWorkflow(
            "black-portraits",
            "Find and fix black portraits",
            "Hero or unit portraits render as a black square because a model or its texture is missing.",
            new[]
            {
                "asset_model_lint", "asset_audit", "model_audit",
                "asset_thumbnails", "render_model",
            },
            new[]
            {
                "asset model-lint", "asset audit", "model audit",
                "asset thumbnails", "render-model", "repair portraits",
            }),

        new QuickstartWorkflow(
            "shrink-map",
            "Shrink a map's file size",
            "A map is too large to host, usually from duplicated or unused imported assets.",
            new[]
            {
                "map_optimize", "asset_size_report", "asset_duplicates",
                "asset_unused", "mpq_verify",
            },
            new[]
            {
                "map optimize", "asset size-report", "asset duplicates",
                "asset unused", "mpq verify",
            }),

        new QuickstartWorkflow(
            "hunt-leaks",
            "Hunt script leaks",
            "Find handles, timers and triggers created and never destroyed, the classic WC3 leak.",
            new[]
            {
                "script_leaks", "script_lua_leaks", "leak_list",
                "leak_model", "bench_leaks",
            },
            new[]
            {
                "script leaks", "script lua-leaks", "leak list",
                "leak model", "bench leaks",
            }),

        new QuickstartWorkflow(
            "edit-objects",
            "Edit object data",
            "Change a unit, ability, item or other object's fields, or create a custom one.",
            new[]
            {
                "object_list", "object_get", "object_set", "object_new",
                "object_field_options", "object_search",
            },
            new[]
            {
                "object list", "object get", "object set", "object new",
                "object options", "object search",
            }),

        new QuickstartWorkflow(
            "terrain-and-placement",
            "Edit terrain and place units or doodads",
            "Sculpt terrain, paint tiles, and place or move units, doodads and regions.",
            new[]
            {
                "terrain_info", "terrain_paint", "terrain_fill", "terrain_export_heightmap",
                "place_unit", "place_doodad", "pathing_export_png",
            },
            new[]
            {
                "terrain info", "terrain paint", "terrain fill", "terrain export-heightmap",
                "place unit", "place doodad", "pathing export-png",
            }),

        new QuickstartWorkflow(
            "lua-conversion",
            "Convert a JASS map to Lua",
            "Move a map's script from JASS to Lua, or check one that has already moved.",
            new[]
            {
                "script_to_lua", "script_lua_check", "script_lua_to_jass", "script_conversion_notes",
            },
            new[]
            {
                "script to-lua", "script lua-check", "script lua-to-jass", "script conversion-notes",
            }),

        new QuickstartWorkflow(
            "patch-compat",
            "Check a map against a target patch",
            "See what breaks if a map runs on a different game build, before shipping it.",
            new[] { "map_compat", "compat_matrix", "compat_explain", "bench_compat", "gamedata_changelog" },
            new[] { "compat", "compat matrix", "compat explain", "bench compat", "gamedata changelog" }),
    };

    /// <summary>
    /// The workflows, optionally narrowed by a goal substring. A null or blank goal returns all
    /// ten. A goal matches when it appears (case-insensitively) in the id, the title, the when-to-use
    /// line, or any tool or command name. When nothing matches the goal the full list is returned,
    /// so a caller always gets a usable answer rather than an empty one.
    /// </summary>
    public static IReadOnlyList<QuickstartWorkflow> Execute(string? goal = null)
    {
        if (string.IsNullOrWhiteSpace(goal)) return All;

        string needle = goal.Trim();
        var matches = All.Where(w => Matches(w, needle)).ToList();
        return matches.Count > 0 ? matches : All;
    }

    private static bool Matches(QuickstartWorkflow w, string needle) =>
        Contains(w.Id, needle) || Contains(w.Title, needle) || Contains(w.WhenToUse, needle)
        || w.McpTools.Any(t => Contains(t, needle))
        || w.CliCommands.Any(c => Contains(c, needle));

    private static bool Contains(string haystack, string needle) =>
        haystack.Contains(needle, StringComparison.OrdinalIgnoreCase);
}
