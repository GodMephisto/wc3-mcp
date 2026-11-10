// src/Wc3.Mcp/McpToolCatalog.cs
using System.Globalization;
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
    bool? OpenWorld,
    string? Family = null)
{
    /// <summary>Other names a caller may use for this tool. Empty until a tool declares one.</summary>
    public IReadOnlyList<string> Aliases { get; init; } = Array.Empty<string>();

    /// <summary>The JSON Schema of the tool arguments, copied from the served tool.</summary>
    public JsonElement? InputSchema { get; init; }

    public bool Annotated =>
        ReadOnly is not null || Destructive is not null || Idempotent is not null || OpenWorld is not null;
}

/// <summary>One annotation problem, naming the tool and what is wrong with it.</summary>
public sealed record McpAnnotationProblem(string Tool, string Problem);

/// <summary>
/// One prompt exactly as the server serves it, for the catalog resource. Name and title come from
/// the prompt attribute, the description from its Description attribute, and Arguments holds the
/// argument names a caller must or may pass.
/// </summary>
public sealed record McpPromptInfo(
    string Name,
    string? Title,
    string? Description,
    IReadOnlyList<string> Arguments);

/// <summary>
/// The whole catalog resource. The tools keep the array they already had, now under tools, and the
/// prompts sit beside them so a client that reads only wc3://tools can see what a prompt takes.
/// </summary>
public sealed record McpCatalog(
    IReadOnlyList<McpToolInfo> Tools,
    IReadOnlyList<McpPromptInfo> Prompts);

/// <summary>
/// SUR-02 and SUR-03. One renamed tool and the release that ends the window. The old name answers
/// to the current tool until the removal version is reached, so the period is stated as a version
/// rather than a date and every build decides for itself whether the window is still open. The
/// current name is the only name the surface lists, and the old name never becomes a tool.
/// </summary>
public sealed record ToolAlias(string OldName, string CurrentName, string RemovalVersion);

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
    /// <summary>
    /// SUR-02 and SUR-03. The alias window, old name to the tool that answers to it, each entry
    /// carrying the version that removes the old name. The table is empty while no name in this
    /// tree has changed, because every name the surface serves is still the name it shipped with.
    /// A rename adds one entry here and nothing else, since the call filter and the catalog both
    /// read this table rather than a second copy of it.
    /// </summary>
    public static IReadOnlyDictionary<string, ToolAlias> Renames { get; } = BuildRenames();

    private static IReadOnlyDictionary<string, ToolAlias> BuildRenames()
    {
        ToolAlias[] entries =
        {
            // The shape a rename takes, and the only place a period is stated.
            // new ToolAlias("triggers_read", "trigger_tree_get", "0.5.0"),
        };

        var table = new Dictionary<string, ToolAlias>(StringComparer.Ordinal);
        foreach (ToolAlias entry in entries) table[entry.OldName] = entry;
        return table;
    }

    /// <summary>
    /// SUR-02. The name the surface serves for a caller's name, read from the same table the call
    /// filter uses, so the rewrite and the catalog cannot disagree about a rename. A name outside
    /// the window comes back unchanged, which is what leaves an unknown tool to fail at the
    /// protocol layer as it always has.
    /// </summary>
    public static string ServedName(string name, IReadOnlyDictionary<string, ToolAlias>? renames = null) =>
        (renames ?? Renames).TryGetValue(name, out ToolAlias? alias) ? alias.CurrentName : name;

    /// <summary>
    /// SUR-03. True when this build is at or past the entry's removal version, which is when the
    /// old name stops answering. Three numeric parts compared left to right, so 0.4.13 is inside a
    /// window that ends at 0.5.0 and outside one that ended at 0.4.12. A removal version that does
    /// not parse counts as reached, so a broken entry can never keep a name alive, and
    /// <see cref="InspectRenames"/> names the same entry as a problem.
    /// </summary>
    public static bool RemovalReached(string removalVersion, string currentVersion)
    {
        if (!TryVersion(removalVersion, out int[] removal)) return true;
        if (!TryVersion(currentVersion, out int[] current)) return false;
        for (int i = 0; i < 3; i++)
        {
            if (current[i] != removal[i]) return current[i] > removal[i];
        }
        return true;
    }

    /// <summary>Three numeric parts, or false when the text is not a version. Missing parts are zero.</summary>
    public static bool TryVersion(string? text, out int[] parts)
    {
        parts = new int[3];
        if (string.IsNullOrWhiteSpace(text)) return false;
        string[] pieces = text.Trim().Split('.');
        if (pieces.Length > 3) return false;
        for (int i = 0; i < pieces.Length; i++)
        {
            if (!int.TryParse(pieces[i].Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out int value))
                return false;
            parts[i] = value;
        }
        return true;
    }

    /// <summary>
    /// The problems with an alias table, in the order they are found. An empty list means every
    /// entry names a period, a tool and a name that were not already served. When
    /// <paramref name="liveTools"/> is supplied the table is also checked against the surface, so
    /// an alias whose target is missing, or whose old name is still a live tool, is a problem
    /// rather than a silent second name. The server calls this at startup for a table it was given
    /// and fails loud, and a test calls it for the shipped table and the live surface.
    /// </summary>
    public static IReadOnlyList<string> InspectRenames(
        IReadOnlyDictionary<string, ToolAlias>? table = null, IReadOnlyList<string>? liveTools = null)
    {
        IReadOnlyDictionary<string, ToolAlias> entries = table ?? Renames;
        var problems = new List<string>();
        foreach (var pair in entries.OrderBy(p => p.Key, StringComparer.Ordinal))
        {
            ToolAlias entry = pair.Value;
            if (!string.Equals(pair.Key, entry.OldName, StringComparison.Ordinal))
                problems.Add("the table key \"" + pair.Key + "\" is not the old name \"" + entry.OldName + "\".");
            if (string.IsNullOrWhiteSpace(entry.OldName))
                problems.Add("an entry has no old name.");
            if (string.IsNullOrWhiteSpace(entry.CurrentName))
                problems.Add(entry.OldName + " names no current tool.");
            else if (string.Equals(entry.OldName, entry.CurrentName, StringComparison.Ordinal))
                problems.Add(entry.OldName + " is aliased to itself, which is a rename that did not happen.");
            if (!TryVersion(entry.RemovalVersion, out _))
                problems.Add(entry.OldName + " states the removal version \"" + entry.RemovalVersion
                    + "\", which is not a version, so the window has no stated end.");
            if (liveTools is null) continue;
            if (!liveTools.Contains(entry.CurrentName, StringComparer.Ordinal))
                problems.Add(entry.OldName + " points at \"" + entry.CurrentName + "\", which the surface does not serve.");
            if (liveTools.Contains(entry.OldName, StringComparer.Ordinal))
                problems.Add(entry.OldName + " is still a live tool, so it cannot also be an alias.");
        }
        return problems;
    }

    /// <summary>The old names that resolve to this tool, sorted, for the catalog entry.</summary>
    private static IReadOnlyDictionary<string, IReadOnlyList<string>> AliasesByTool(
        IReadOnlyDictionary<string, ToolAlias> table)
    {
        var byTool = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        foreach (ToolAlias entry in table.Values.OrderBy(e => e.OldName, StringComparer.Ordinal))
        {
            if (!byTool.TryGetValue(entry.CurrentName, out List<string>? names))
                byTool[entry.CurrentName] = names = new List<string>();
            names.Add(entry.OldName);
        }
        return byTool.ToDictionary(
            p => p.Key, p => (IReadOnlyList<string>)p.Value.ToArray(), StringComparer.Ordinal);
    }

    /// <summary>
    /// Every tool, ordinal-sorted, with its description and four annotation hints. The aliases a
    /// tool answers to are listed on that tool and never on a row of their own, which is what keeps
    /// the catalog to one entry per tool.
    /// </summary>
    public static IReadOnlyList<McpToolInfo> Describe(IReadOnlyDictionary<string, ToolAlias>? renames = null)
    {
        IReadOnlyDictionary<string, IReadOnlyList<string>> byTool = AliasesByTool(renames ?? Renames);
        return Wc3McpServer.CreateTools()
            .Select(t => ToInfo(t.ProtocolTool, byTool))
            .OrderBy(i => i.Name, StringComparer.Ordinal)
            .ToList();
    }

    internal static McpToolInfo ToInfo(
        Tool tool, IReadOnlyDictionary<string, IReadOnlyList<string>>? aliasesByTool = null)
    {
        var a = tool.Annotations;
        return new McpToolInfo(
            tool.Name, tool.Description,
            a?.ReadOnlyHint, a?.DestructiveHint, a?.IdempotentHint, a?.OpenWorldHint,
            McpToolFamilies.Of(tool.Name))
        {
            Aliases = aliasesByTool is not null && aliasesByTool.TryGetValue(tool.Name, out var names)
                ? names
                : Array.Empty<string>(),
            InputSchema = tool.InputSchema,
        };
    }

    /// <summary>Every prompt, ordinal-sorted, with its title, description and argument names.</summary>
    public static IReadOnlyList<McpPromptInfo> DescribePrompts() =>
        Wc3McpServer.CreatePrompts()
            .Select(p => ToPromptInfo(p.ProtocolPrompt))
            .OrderBy(p => p.Name, StringComparer.Ordinal)
            .ToList();

    internal static McpPromptInfo ToPromptInfo(Prompt prompt) =>
        new(prompt.Name, prompt.Title, prompt.Description,
            (prompt.Arguments ?? Array.Empty<PromptArgument>()).Select(a => a.Name).ToList());

    /// <summary>
    /// The catalog as one read-only MCP resource, so a client can pull the whole surface in a
    /// single read instead of listing tools or prompts. Same reflection as the tools and the
    /// prompts, so it cannot drift. The JSON is serialized once at start, and the resource carries
    /// its byte size and digest, so resources/list names what a read returns.
    /// </summary>
    public static McpServerResource CreateCatalogResource()
    {
        var catalog = new McpCatalog(Describe(), DescribePrompts());
        string json = JsonSerializer.Serialize(catalog, Wc3McpServer.JsonOptions);
        return new ServedTextResource(
            "wc3://tools",
            "tool_catalog",
            null,
            "The MCP surface as JSON. Every tool with its description, its family, the four annotations, its aliases and its input schema, and every prompt with its title, description and argument names.",
            "application/json",
            json);
    }
}

/// <summary>
/// Audits the MCP surface against six rules. Every tool must declare all four hints, because an
/// unset hint defaults to true and a client reads it as a claim the server never made. A tool
/// whose name begins with a mutating stem must not claim to be read-only. A tool that writes must
/// not say read-only in its description, in any scope, because a caller reads that word as the
/// whole answer. A tool cannot be both read-only and destructive, and a tool whose only effect is
/// to remove something must declare destructive. The prefix list is deliberately precise rather than broad. A blanket 'port_' or
/// 'terrain_' would flag port_deps and port_preview, which are genuinely read-only, so each
/// mutating stem is named instead.
/// </summary>
public static class McpToolAudit
{
    /// <summary>
    /// Tool names that discard content a caller cannot get back, so they must declare
    /// <c>destructiveHint</c> true. Kept deliberately precise: each name is a tool whose only
    /// effect is to remove something, and <c>mpq_strip_bookkeeping</c> is absent because the
    /// bookkeeping entries it drops are regenerated (SAF-03).
    /// </summary>
    private static readonly string[] DestructiveNames =
    {
        "camera_remove", "mpq_remove", "mpq_remove_many", "placed_doodad_remove",
        "placed_fix_refs", "placed_unit_remove", "region_remove", "sound_remove",
        "trigger_remove", "trigger_remove_eca", "trigger_var_remove",
    };

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
            var unset = new List<string>();
            if (tool.ReadOnly is null) unset.Add("readOnlyHint");
            if (tool.Destructive is null) unset.Add("destructiveHint");
            if (tool.Idempotent is null) unset.Add("idempotentHint");
            if (tool.OpenWorld is null) unset.Add("openWorldHint");
            if (unset.Count > 0)
                problems.Add(new McpAnnotationProblem(tool.Name, "leaves " + string.Join(", ", unset) + " unset"));
            if (tool.ReadOnly == true && MutatingPrefixes.Any(p => tool.Name.StartsWith(p, StringComparison.Ordinal)))
                problems.Add(new McpAnnotationProblem(tool.Name, "name looks mutating but it is marked read-only"));
            if (tool.ReadOnly != true && tool.Description is not null
                && tool.Description.Contains("read-only", StringComparison.OrdinalIgnoreCase))
                problems.Add(new McpAnnotationProblem(tool.Name, "description says read-only but the tool writes"));
            if (tool.ReadOnly == true && tool.Destructive == true)
                problems.Add(new McpAnnotationProblem(tool.Name, "declares both read-only and destructive"));
            if (tool.Destructive != true && DestructiveNames.Contains(tool.Name, StringComparer.Ordinal))
                problems.Add(new McpAnnotationProblem(tool.Name, "name looks destructive but it is marked non-destructive"));
        }
        return new McpAnnotationAuditResult(problems);
    }
}
