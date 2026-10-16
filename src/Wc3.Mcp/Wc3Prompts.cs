// src/Wc3.Mcp/Wc3Prompts.cs
using System.ComponentModel;
using ModelContextProtocol.Server;

namespace Wc3.Mcp;

/// <summary>
/// MCP prompts for the five most common jobs. A prompt is a ready-made instruction an assistant
/// can pull from the server instead of guessing a plan, and each one names the exact tools to call
/// so a client does not have to search a surface of more than two hundred tools first.
///
/// Every prompt is read-only guidance text. It never reads or writes a map, it only returns the
/// words a caller should follow, so serving one has no side effect on any file.
/// </summary>
[McpServerPromptType]
public static class Wc3Prompts
{
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
