// src/Wc3.Mcp/Wc3Prompts.cs
using System.ComponentModel;
using ModelContextProtocol.Server;
using Wc3.Commands;

namespace Wc3.Mcp;

/// <summary>
/// MCP prompts for the twelve quickstart workflows. A prompt is a ready-made instruction an assistant
/// can pull from the server instead of guessing a plan, and each one names the exact tools to call
/// so a client does not have to search a surface of more than two hundred tools first.
///
/// Every prompt is read-only guidance text. It never reads or writes a map, it only returns the
/// words a caller should follow, so serving one has no side effect on any file.
/// </summary>
[McpServerPromptType]
public static class Wc3Prompts
{
    /// <summary>
    /// The prompt that carries each quickstart workflow, keyed by the workflow id (PRM-02). It is
    /// derived from <see cref="QuickstartCommand.All"/> rather than written a second time, so a
    /// workflow cannot name a prompt that does not exist and the two lists cannot drift.
    ///
    /// <c>audit_hero_spells</c> is a prompt with no workflow. That is allowed, because the
    /// requirement asks that every workflow have a prompt, not the reverse.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, string> WorkflowPrompts =
        QuickstartCommand.All.ToDictionary(w => w.Id, w => w.Prompt, StringComparer.Ordinal);
    [McpServerPrompt(Name = "fix_black_portraits", Title = "Find and fix black portraits")]
    [Description("Produce a guided plan for a hero or unit portrait that renders as a black square. "
        + "Use when a caller sees a black portrait in game or in the editor.")]
    public static string FixBlackPortraits(
        [Description("Path to the .w3x/.w3m map. Optional.")] string? map = null)
    {
        string target = string.IsNullOrWhiteSpace(map) ? "<map>" : map;
        return
            "Find why a portrait on " + target + " renders as a black square, then fix it. Work in this order.\n"
            + "1. Call asset_model_lint for a model whose skin, portrait or team colour texture is missing.\n"
            + "2. Call asset_audit and model_audit to separate a missing texture from a missing model.\n"
            + "3. Call asset_thumbnails to render the portrait and confirm the square is gone from the pixels.\n"
            + "4. Call render_model for the model in isolation when the thumbnail still looks wrong.\n"
            + "Name the exact file that is missing. A black square is nearly always a texture path that "
            + "resolves inside the editor but not inside the map archive.";
    }

    [McpServerPrompt(Name = "hunt_leaks", Title = "Hunt script leaks")]
    [Description("Produce a guided plan for finding handles, timers and triggers that are created and "
        + "never destroyed. Use when a map slows down over a long game.")]
    public static string HuntLeaks(
        [Description("Path to the .w3x/.w3m map. Optional.")] string? map = null)
    {
        string target = string.IsNullOrWhiteSpace(map) ? "<map>" : map;
        return
            "Find leaked handles on " + target + ". Work in this order.\n"
            + "1. Call script_leaks for the JASS scan and script_lua_leaks for a map that moved to Lua.\n"
            + "2. Call leak_list for the entries the scan recorded, so the report and the script agree.\n"
            + "3. Call leak_model to score the leak against the arithmetic of a long game rather than a guess.\n"
            + "4. Call bench_leaks to confirm the count against the benchmark harness.\n"
            + "Report each leak with the line, the handle type and the fix. Change the script only when "
            + "the caller asks, because a leak fix is a behaviour change and needs a rerun of the map.";
    }

    [McpServerPrompt(Name = "edit_objects", Title = "Edit object data")]
    [Description("Produce a guided plan for reading and changing unit, ability, item or other object "
        + "fields, including creating a custom object. Use when a caller wants a value changed.")]
    public static string EditObjects(
        [Description("Path to the .w3x/.w3m map. Optional.")] string? map = null)
    {
        string target = string.IsNullOrWhiteSpace(map) ? "<map>" : map;
        return
            "Change object data on " + target + " without damaging the rest of the map. Work in this order.\n"
            + "1. Call object_list for the kind, then object_get for the one entry, so you edit against the "
            + "current values rather than a remembered ones.\n"
            + "2. Call object_field_options for the field you mean to change, to learn its type and its range.\n"
            + "3. Call object_search when you know the value you want but not the id that carries it.\n"
            + "4. Call object_set for an existing object, or object_new to create a custom one, always to a "
            + "new out_path. Pass overwrite only when the caller has agreed to replace that file.\n"
            + "Report the object, the field, the old value and the new value, and the digests the write "
            + "returned, so a caller can tell the edit landed on the file they expected.";
    }

    [McpServerPrompt(Name = "edit_terrain_and_placement", Title = "Edit terrain and place units or doodads")]
    [Description("Produce a guided plan for sculpting terrain, painting tiles and placing or moving "
        + "units, doodads and regions. Use when a caller wants the ground or the placement changed.")]
    public static string EditTerrainAndPlacement(
        [Description("Path to the .w3x/.w3m map. Optional.")] string? map = null)
    {
        string target = string.IsNullOrWhiteSpace(map) ? "<map>" : map;
        return
            "Edit the ground and the placement on " + target + ". Work in this order.\n"
            + "1. Call terrain_info for the dimensions, the tile set and the current height range.\n"
            + "2. Call terrain_paint for a tile change and terrain_fill for a height or cliff change, one "
            + "region at a time, and read the tile count each call reports.\n"
            + "3. Call place_unit or place_doodad, then placed_doodad_set or placed_unit_set to adjust what "
            + "you placed rather than placing a second copy.\n"
            + "4. Call terrain_export_heightmap or pathing_export_png when the caller wants to see the "
            + "result outside the editor. Those two write image files, not a map.\n"
            + "Write to a new out_path. Terrain edits are the ones a caller notices immediately, so report "
            + "the region you changed and the count, not only the path.";
    }

    [McpServerPrompt(Name = "convert_to_lua", Title = "Convert a JASS map to Lua")]
    [Description("Produce a guided plan for moving a map's script from JASS to Lua, or for checking "
        + "one that has already moved. Use when a caller asks about Lua conversion.")]
    public static string ConvertToLua(
        [Description("Path to the .w3x/.w3m map. Optional.")] string? map = null)
    {
        string target = string.IsNullOrWhiteSpace(map) ? "<map>" : map;
        return
            "Convert the script on " + target + " from JASS to Lua. Work in this order.\n"
            + "1. Call script_conversion_notes for the traps that apply to this map before changing anything.\n"
            + "2. Call script_to_lua to write the converted script, then script_lua_check to lint the result.\n"
            + "3. Call script_lua_to_jass only when the caller wants the reverse, since a round trip is how "
            + "a conversion is checked rather than a conversion in itself.\n"
            + "A conversion that lints clean can still change behaviour, so report what the notes flagged "
            + "and what the check could not see. Never claim the map runs without a test in game.";
    }

    [McpServerPrompt(Name = "check_patch_compat", Title = "Check a map against a target patch")]
    [Description("Produce a guided plan for what breaks when a map runs on a different game build. "
        + "Use before shipping a map to a host on another patch.")]
    public static string CheckPatchCompat(
        [Description("Path to the .w3x/.w3m map. Optional.")] string? map = null)
    {
        string target = string.IsNullOrWhiteSpace(map) ? "<map>" : map;
        return
            "Check " + target + " against a target build. Work in this order.\n"
            + "1. Call map_compat with the target build to get the verdict for this map.\n"
            + "2. Call compat_matrix when the caller is choosing between builds, so the answer covers more "
            + "than the one build they named.\n"
            + "3. Call compat_explain for the reason behind a failure, naming the field or the block.\n"
            + "4. Call gamedata_changelog for what moved between the two builds, and bench_compat to see "
            + "whether the difference is a load cost rather than a break.\n"
            + "Report the verdict per build and the reason for each failure.";
    }

    [McpServerPrompt(Name = "fix_trigger_wiring", Title = "Read and fix a trigger tree")]
    [Description("Produce a guided plan for a spell or a hero that does nothing in game because the "
        + "trigger behind it is dead, disabled or wired to an event that never fires. Use when "
        + "something that should fire does not.")]
    public static string FixTriggerWiring(
        [Description("Path to the .w3x/.w3m map. Optional.")] string? map = null)
    {
        string target = string.IsNullOrWhiteSpace(map) ? "<map>" : map;
        return
            "Find why a trigger on " + target + " never fires. Work in this order.\n"
            + "1. Call triggers_read for the tree, the variables and the categories, so you argue from "
            + "the map rather than from a remembered layout.\n"
            + "2. Call trigger_lint for the wiring faults the linter can prove, such as an event that "
            + "no player reaches or a handler with no live statement.\n"
            + "3. Call trigger_dead for the triggers that can never run, and trigger_diff when the "
            + "caller says the map worked before a change.\n"
            + "4. Call trigger_to_script or trigger_export to read the generated JASS when the tree "
            + "looks right and the map still fails, since the answer is often in the body.\n"
            + "Report the trigger, the missing link and the fix. Change a trigger only when the caller "
            + "asks, and write to a new out_path when you do.";
    }

    [McpServerPrompt(Name = "learn_the_surface", Title = "Learn what this server can do")]
    [Description("Produce a guided plan for a first session, finding the tool for a job and reading "
        + "the reference before calling anything that writes. Use when a caller is new to the server.")]
    public static string LearnTheSurface(
        [Description("What the caller wants to do, in their own words. Optional.")] string? goal = null)
    {
        string which = string.IsNullOrWhiteSpace(goal) ? "the job the caller names" : goal;
        return
            "Start from nothing and reach " + which + " without damaging a map. Work in this order.\n"
            + "1. Call quickstart with the goal in the caller's own words, to get the workflow and the "
            + "tools and commands that carry it.\n"
            + "2. Call version_info to learn the surface version, the protocol revision and the tool "
            + "count, so a later failure can be told apart from a version difference.\n"
            + "3. Call docs_search for the reference text on the tool you intend to call.\n"
            + "4. Call changelog when the caller is upgrading, so a moved flag is not read as a break.\n"
            + "Read the annotations before the first write. A tool that declares destructive needs a "
            + "deliberate call, and every write takes an out_path, so nothing replaces the input.";
    }

    [McpServerPrompt(Name = "inspect_map", Title = "Inspect a map")]
    [Description("Produce a guided plan for understanding a Warcraft III map before changing it. "
        + "Use when a caller has a .w3x or .w3m and wants to know what it holds.")]
    public static string InspectMap(
        [Description("Path to the .w3x/.w3m map to inspect. Optional.")] string? map = null)
    {
        string target = string.IsNullOrWhiteSpace(map) ? "<map>" : map;
        return
            "Inspect the Warcraft III map at " + target + " without editing it. Work in this order.\n"
            + "1. Call map_overview for metadata, the file list and the custom object counts in one read.\n"
            + "2. Call object_list for each object kind that matters, then object_get for one entry.\n"
            + "3. Call audit_map for structural findings (missing models, dangling references, faults).\n"
            + "4. Call triggers_read if the map carries a GUI trigger tree.\n"
            + "Report the map name, author, dimensions, the custom object counts, and the audit findings. "
            + "Change nothing. If the caller then wants a specific job, call the quickstart tool with a goal.";
    }

    [McpServerPrompt(Name = "port_hero", Title = "Port a hero between maps")]
    [Description("Produce a guided plan for porting a hero or unit and its dependencies from one map "
        + "into another. Use when a caller wants a hero or unit moved across maps.")]
    public static string PortHero(
        [Description("Path to the source map. Optional.")] string? source = null,
        [Description("Path to the target map. Optional.")] string? target = null,
        [Description("Hero or unit rawcode to port. Optional.")] string? unit = null)
    {
        string src = string.IsNullOrWhiteSpace(source) ? "<source>" : source;
        string dst = string.IsNullOrWhiteSpace(target) ? "<target>" : target;
        string which = string.IsNullOrWhiteSpace(unit) ? "<rawcode>" : unit;
        return
            "Port " + which + " from " + src + " into " + dst + ". Work in this order.\n"
            + "1. Call hero_roster on the source to confirm the hero and its rawcode.\n"
            + "2. Call port_deps to resolve the full dependency closure (objects, assets, strings, script).\n"
            + "3. Call port_preview for the porting preview and any rawcode remaps.\n"
            + "4. Call port_unit to write the ported map. The source and target maps are never modified in place.\n"
            + "5. Call audit_hero on the ported map to confirm the cast chain is wired end to end.\n"
            + "If the target is a different game version, use port_into_version instead of port_unit. "
            + "State plainly any dependency the port could not carry.";
    }

    [McpServerPrompt(Name = "repair_reforged_map", Title = "Repair a map broken by a Reforged patch")]
    [Description("Produce a guided plan for repairing a map that a Reforged patch broke or that holds "
        + "a stale generated block. Use when a map that loaded before now fails or misbehaves.")]
    public static string RepairReforgedMap(
        [Description("Path to the .w3x/.w3m map to repair. Optional.")] string? map = null)
    {
        string target = string.IsNullOrWhiteSpace(map) ? "<map>" : map;
        return
            "Repair the Reforged regression on " + target + ". Work in this order.\n"
            + "1. Call map_compat against the current target to list what the map now references that the "
            + "build no longer defines.\n"
            + "2. Call compat_explain on one finding the caller does not understand.\n"
            + "3. Call repair_generated when the map carries a wc3ctl generated helper block that is too "
            + "narrow. It refuses maps without one.\n"
            + "4. Call map_optimize to rebuild the archive and drop duplicated or unused files.\n"
            + "Always write to a new out_path and never overwrite the input map. Report which fixes applied "
            + "and which were refused, and say the result still needs a play test.";
    }

    [McpServerPrompt(Name = "optimize_map_size", Title = "Shrink a map's file size")]
    [Description("Produce a guided plan for reducing a map's size by removing duplicated and unused "
        + "assets. Use when a map is too large to host.")]
    public static string OptimizeMapSize(
        [Description("Path to the .w3x/.w3m map to shrink. Optional.")] string? map = null)
    {
        string target = string.IsNullOrWhiteSpace(map) ? "<map>" : map;
        return
            "Reduce the size of " + target + ". Work in this order.\n"
            + "1. Call asset_size_report for the imported assets ranked by size.\n"
            + "2. Call asset_duplicates for byte-identical imports that can be merged.\n"
            + "3. Call asset_unused for imports nothing references. Treat the list as a lead to review, "
            + "not an instruction to delete, because a reference can hide in a script or a model.\n"
            + "4. Call map_optimize to rebuild the archive, and mpq_verify on the result.\n"
            + "Write to a new out_path and never overwrite the input map.";
    }

    [McpServerPrompt(Name = "audit_hero_spells", Title = "Audit a placed hero's spells")]
    [Description("Produce a guided plan for checking whether a placed hero's abilities are wired up in "
        + "the map, not merely present as object data. Use when a hero's spells do not fire in game.")]
    public static string AuditHeroSpells(
        [Description("Path to the .w3x/.w3m map. Optional.")] string? map = null,
        [Description("Hero rawcode to audit. Omit to audit every placed hero.")] string? hero = null)
    {
        string target = string.IsNullOrWhiteSpace(map) ? "<map>" : map;
        string which = string.IsNullOrWhiteSpace(hero) ? "every placed hero" : hero;
        return
            "Audit the spell wiring for " + which + " on " + target + ". Work in this order.\n"
            + "1. Call audit_hero to verify the chain end to end (the handler is attached to a trigger, the "
            + "event reaches the hero's player, the body has live statements).\n"
            + "2. Call audit_readiness to check InitGlobals and RunInitializationTriggers are carried and "
            + "called, and that every global the hero reads is assigned.\n"
            + "3. Call audit_ability for the per-ability verdict when a hero reads wired but still fails.\n"
            + "Name the exact missing link for each failure, and recall that a missing link is the useful "
            + "result rather than a pass.";
    }
}
