// src/Wc3.Mcp/Wc3Tools.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Mcp;

/// <summary>
/// The MCP tool surface: one thin wrapper per Wc3.Commands handler. Each tool
/// loads the map(s), calls the shared command layer and returns its result POCO
/// (the SDK serializes it to JSON), no map parsing or logic lives here.
/// <para>
/// RES-02. The twelve tools whose result is a wrapper record, <see cref="CappedPage{T}"/>
/// with its page counts and items, also publish an output schema. The attribute's
/// <c>UseStructuredContent</c> is what turns it on, and SDK 1.4.1 then generates the schema from
/// the method's return type and sends the same object as <c>structuredContent</c> beside the text
/// block, so no schema is hand written here. At SDK 1.4.1 the attribute value wins over
/// <c>McpServerToolCreateOptions.UseStructuredContent</c>, which is why this is declared per tool
/// rather than once in the surface wiring. The text block still carries the same JSON, so a client
/// that reads both pays for it twice, which is why the capped wrapper records are the set that
/// carries one rather than every tool on the surface.
/// </para>
/// </summary>
[McpServerToolType]
public static class Wc3Tools
{
    private const string KindValues = "unit|item|ability|destructable|doodad|buff|upgrade";

    /// <summary>The overwrite argument's wording, identical on every writing tool, so a caller
    /// reads one contract across the whole surface.</summary>
    private const string OverwriteDoc =
        "Replace an existing output. Default false, and a rerun that would overwrite is refused with "
        + "OUTPUT_EXISTS and the output is left as it was (SAF-05).";

    /// <summary>The expected digest argument's wording, identical on every writing tool.</summary>
    private const string ExpectShaDoc =
        "The SHA-256 of the input as you last read it. A mismatch is refused with STALE_INPUT, so a "
        + "write cannot land on content that changed underneath it (SAF-07).";

    [McpServerTool(Name = "map_info", Title = "Map metadata", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Show a Warcraft III map's metadata: name, author, player count, playable dimensions and load diagnostics.")]
    public static MapInfoResult MapInfo(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() =>
        {
            try { return InfoCommand.Execute(LoadMap(map)); }
            catch (McpException ex) when (ex.Message.StartsWith("map must be a .w3x or .w3m file", StringComparison.Ordinal)
                || ex.Message.StartsWith("map not found", StringComparison.Ordinal))
            {
                throw Wc3Errors.Refuse(Wc3ErrorCodes.MapUnreadable, ex.Message
                    + ". An asset path is listed by asset_list, which takes the map and an icon or model family", map);
            }
        });

    [McpServerTool(Name = "list_files", Title = "List map files", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("List the map archive's internal files, each with its name (null = unnamed/protected entry), size in bytes, whether wc3ctl knows/parses the format, and for unnamed entries the content type sniffed from their leading bytes ('BLP texture', 'MDX model', 'empty', ...). Pass brief true to get the total and only the first 50 files when the full list would be too large for the context.")]
    public static PagedFiles ListFiles(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Recover unnamed entries' names from the map's own script and object data before listing (worth paying for on a protected map). Recovered names are flagged nameFromHarvest.")] bool harvest = false,
        [Description("Return the total file count and only the first 50 files instead of the full list. Default false.")] bool brief = false)
        => Run(() =>
        {
            var doc = LoadMap(map);
            if (harvest) doc.HarvestAssetNames();
            if (!brief)
            {
                var all = ListCommand.Execute(doc, typeUnnamed: true).Files;
                return new PagedFiles(all.Count, all.Count, false, null, all, false, all.Count);
            }
            var page = ListCommand.ExecuteBrief(doc, typeUnnamed: true);
            return new PagedFiles(page.Files.Count, page.Total, page.Truncated,
                page.Truncated ? page.Files.Count : null, page.Files, page.Truncated, page.Total);
        });

    [McpServerTool(Name = "map_overview", Title = "Map overview", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("One call per map: metadata (name, author, player count, dimensions), every archive file, the custom or modified object counts by Object Editor kind, and the load diagnostics. Use this when you need a map's overall shape instead of calling map_info, list_files and object_list separately.")]
    public static MapOverviewResult MapOverview(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => MapOverviewCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "mcp_audit_annotations", Title = "Audit tool annotations", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Audit the MCP tool surface itself. Reports every tool whose annotations disagree with its name (a mutating name marked read-only) and every tool that declares no annotations, as a list of problems plus a count. An empty list means the surface is clean. Read-only.")]
    public static McpAnnotationAuditResult McpAuditAnnotations()
        => Run(() => McpToolAudit.InspectServer());

    [McpServerTool(Name = "error_codes", Title = "List the error codes", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("List every stable error code this server can return, each with the one line that says what it means. A refusal carries one of these codes as the first bracketed word of its message and as the code field of the error result, so a caller can branch on the code instead of reading prose. Read-only, it touches no map.")]
    public static ErrorCodesResult ErrorCodes() => Run(() => new ErrorCodesResult(Wc3ErrorCodes.Count, Wc3ErrorCodes.All));

    [McpServerTool(Name = "version_info", Title = "Build and version info", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Report the product version, the git short hash and UTC build time stamped into this build, the name and version of the MCP server it carries, the product id, the newest MCP protocol revision the build speaks and the SDK version it was compiled against. Read-only.")]
    public static VersionInfoReport VersionInfo()
        => Run(() =>
        {
            var v = ReleaseVersionCommand.Read(typeof(Wc3Tools).Assembly,
                ServerIdentity.ServerName, ServerIdentity.Version);
            return new VersionInfoReport(
                v.ProductVersion, v.GitHash, v.BuildTimeUtc, v.McpServerName, v.McpVersion,
                ServerIdentity.ProductId, Wc3McpServer.ProtocolRevision, ServerIdentity.SdkVersion);
        });

    [McpServerTool(Name = "changelog", Title = "Release notes from git", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Generate release notes from git log between two refs, grouped by Conventional Commit type (feat, fix, perf, docs, chore and the rest), a heading per group with its subjects under it. Read-only over git.")]
    public static ChangelogResult Changelog(
        [Description("Git ref to start after. Omit for the most recent tag reachable from the end ref.")] string? from_ref = null,
        [Description("Git ref to end at. Omit for HEAD.")] string? to_ref = null,
        [Description("Repository directory. Omit to locate the wc3ctl repository.")] string? repo_dir = null)
        => Run(() => ChangelogCommand.Execute(
            repo_dir ?? RepoLayout.FindRoot() ?? Directory.GetCurrentDirectory(), from_ref, to_ref));

    [McpServerTool(Name = "quickstart", Title = "Pick a workflow", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Route a broad request to the right workflow. Returns the ten core workflows a map task usually falls into (inspect, port-hero, reforged-3-fix, black-portraits, shrink-map, hunt-leaks, edit-objects, terrain-and-placement, lua-conversion, patch-compat), each naming the MCP tools and the wc3ctl commands that carry it out. Pass a goal (a substring such as 'portrait', 'leak' or 'shrink') to narrow the list. With more than two hundred tools on this server, call this first when the caller states an outcome rather than a tool. Read-only, it touches no map.")]
    public static QuickstartPage Quickstart(
        [Description("Optional goal substring used to narrow the workflows, for example 'portrait', 'leak' or 'shrink'.")] string? goal = null)
        => Run(() =>
        {
            var items = QuickstartCommand.Execute(goal);
            return new QuickstartPage(items.Count, items.Count, false, null, items);
        });

    [McpServerTool(Name = "mcp_export_verify", Title = "Verify MCP export", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Verify that a wc3-mcp checkout holds the same server source scripts/export-wc3-mcp.py would export from this repository, reporting every file that differs without writing. Read-only.")]
    public static McpExportVerifyResult McpExportVerify(
        [Description("Path to the wc3-mcp checkout to check.")] string target_dir,
        [Description("Source repository directory. Omit to locate the wc3ctl repository.")] string? source_dir = null)
        => Run(() => McpExportVerifyCommand.Execute(
            source_dir ?? RepoLayout.FindRoot() ?? Directory.GetCurrentDirectory(), target_dir));

    [McpServerTool(Name = "script_leaks", Title = "Find script handle leaks", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Find handle leaks in a map's war3map.j (locations, groups, forces, effects, timers, "
        + "text tags, lightning and triggers created and never destroyed), each ranked by how often "
        + "its code runs, hot (periodic), repeat (any callback) or once (init), plus every periodic "
        + "timer and trigger with its period. Read-only.")]
    public static ScriptLeaksResult ScriptLeaks(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => ScriptLeaksCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "script_bench", Title = "Rank script hot spots", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Rank every script function by static engine cost per second, per-tick CreateUnit, "
        + "special effects, group enumeration, condition allocation and damage, plus the leak shape "
        + "each one carries. Headless and needs no game client, so an ability can be triaged before "
        + "a play test. Read-only.")]
    public static ScriptBenchResult ScriptBench(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => ScriptBenchCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "script_verify", Title = "Verify script logic", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Static verifier for a map script. Reachability-aware checks for pause without a "
        + "reachable restore, SetUnitTimeScale(0), periodic timers never stopped, group and location "
        + "leaks, ability-strip, ability-hide, GetLocalPlayer desync, sleeps and GetTriggerUnit in a "
        + "timer callback, and constant-exit loops. It follows direct calls and TimerStart callback "
        + "edges, so a correct channel ability with the pause in _Start and resume in _Loop is not "
        + "flagged. Headless, no game client. Read-only.")]
    public static ScriptVerifyResult ScriptVerify(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => ScriptVerifyCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "leak_list", Title = "Ranked leak list", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("The ranked static handle-leak list for a map's script plus the periodic timers and "
        + "triggers that feed it, and every hot leak target by function. Read-only, a presentation over "
        + "script_leaks for the leak workflow.")]
    public static LeakListResult LeakList(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => LeakListCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "leak_harness", Title = "Emit a leak harness", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Emit a small in-map harness in the map's own language (jass or lua) that drives the "
        + "named hot leak candidates on a short timer so the runtime handle and memory slope is measurable "
        + "in seconds rather than an hour, plus a handle count sampler. Writes the harness to out_path as "
        + "valid syntax for the language. Does not start the game.")]
    public static object LeakHarness(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Harness language, jass or lua.")] string lang,
        [Description("A hot leak candidate function name, or all.")] string target,
        [Description("Path to write the harness text to.")] string out_path,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run<object>(() =>
        {
            var doc = LoadMap(map);
            return WriteWith(out_path, Inputs(map), overwrite, expect_input_sha256,
                temp => LeakHarnessCommand.Execute(doc, lang, target, temp));
        });

    [McpServerTool(Name = "leak_join", Title = "Join leak list to session", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Join a map's static leak list to a wc3watch session folder. Reads samples.csv and "
        + "events.csv, fits the handle and memory slopes from the session's forced garbage collection "
        + "when one was marked, and joins each rising window to the periodic candidates from the static "
        + "list and the session events and game log lines near it. Names the likely leaking trigger. "
        + "Read-only. A folder with no samples.csv is reported clearly.")]
    public static LeakJoinResult LeakJoin(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("A wc3watch session folder holding samples.csv and, optionally, events.csv.")] string session_folder)
        => Run(() => LeakJoinCommand.Execute(LoadMap(map), session_folder));

    [McpServerTool(Name = "leak_ab", Title = "Compare two leak sessions", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Diff two wc3watch sessions and report whether the handle and memory slope fell, each "
        + "measured from its own forced garbage collection. The usual shape is one session with a "
        + "trigger live and one with it disabled. With a map, name the periodic candidate whose "
        + "predicted rate is nearest the drop, labelled an inference. Read-only.")]
    public static LeakAbResult LeakAb(
        [Description("First wc3watch session folder.")] string session_a,
        [Description("Second wc3watch session folder.")] string session_b,
        [Description("Optional map, a .w3x/.w3m file or an unpacked map folder, to attribute the drop to a periodic candidate.")] string? map = null)
        => Run(() => LeakAbCommand.Execute(session_a, session_b, map is null ? null : LoadMap(map)));

    [McpServerTool(Name = "leak_benchmark", Title = "Leak detector benchmark", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Run the leak detector over a set of synthetic maps, each with a known injected leak of "
        + "one creator kind plus a clean control, and report true positive and false positive counts so "
        + "the detector's accuracy is a measured number. Read-only.")]
    public static LeakBenchmarkResult LeakBenchmark()
        => Run(() => LeakBenchmarkCommand.Execute());

    [McpServerTool(Name = "leak_model", Title = "Model and texture leaks", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Report the model-level leaks and waste across a map's imported models and textures, "
        + "the join between the model audit and the import table. Finds an imported model no field or "
        + "script names, a model naming a texture the map does not hold, a model imported more than once "
        + "under different names with identical bytes, and an imported texture no imported model names. "
        + "Reuses the existing model parser and import primitives. Read-only.")]
    public static LeakModelResult LeakModel(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => LeakModelCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "script_unused_globals", Title = "Find unused globals", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Find the globals a map's war3map.j declares and never reads, the variable half of "
        + "dead code. A global that is only ever assigned and never appears on the right of anything "
        + "is reported too, since its value is never used. The World Editor's generated globals are "
        + "excluded (every gg_ handle, every bj_ name, and the literal worldedit). Read-only.")]
    public static ScriptUnusedGlobalsResult ScriptUnusedGlobals(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => ScriptUnusedGlobalsCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "script_unused_functions", Title = "Find unused functions", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Find the functions a map's war3map.j declares that no other function calls and "
        + "nothing else reaches, excluding the engine's entry points (main, config, "
        + "InitCustomTriggers and the map initialization functions) and anything an InitTrig_* "
        + "function or an ExecuteFunc(\"...\") string names. Read-only.")]
    public static ScriptUnusedFunctionsResult ScriptUnusedFunctions(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => ScriptUnusedFunctionsCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "script_null_locals", Title = "Find nulled handle locals", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Find the classic JASS handle leak: a local of a handle type (location, group, "
        + "force, effect, timer, trigger, text tag, lightning) that the function destroys with the "
        + "matching Remove or Destroy but never sets to null, which keeps the handle id reserved for "
        + "the rest of the function. A local that is never destroyed is a different leak and is not "
        + "reported here. Read-only.")]
    public static ScriptNullLocalsResult ScriptNullLocals(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => ScriptNullLocalsCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "script_complexity", Title = "Measure function complexity", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Measure each function's cyclomatic complexity as the number of if, loop, and and "
        + "or branch keywords in its body, ranked worst first with the line. Answers where the "
        + "complexity is on a large script. Read-only.")]
    public static ScriptComplexityResult ScriptComplexity(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => ScriptComplexityCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "script_callgraph", Title = "Script call graph", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("The call edges between the functions a map's war3map.j declares, caller to "
        + "callee. Only edges between functions this script declares are included, so a native call "
        + "or a call through a local code value is not an edge. Read-only.")]
    public static ScriptCallGraphResult ScriptCallGraph(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => ScriptCallGraphCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "uabi_profile", Title = "Profile ability lists", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Measure unit ability lists (uabi) across many maps at once, one row per map: unit "
        + "types, references, distinct ids, hero and item abilities in normal lists, ids neither the "
        + "map nor the game defines, duplicates, and a per-race breakdown. Built to compare the "
        + "variants of a disconnect bisection without starting the game.")]
    public static UabiProfileReport UabiProfile(
        [Description("Maps, or folders searched recursively. At most 1000, and a longer list is refused with a tool error naming the limit.")] string[] paths,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Without it hero, item, race and dangling columns use the map's own fields only.")] string? game_dir = null)
        => Run(() =>
        {
            RequireAtMost(paths.Length, "paths");
            return UabiProfileCommand.Execute(paths, ResolveGameDir(game_dir));
        });

    [McpServerTool(Name = "replay_summary", Title = "Replay summary", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Read recorded games (.w3g) and report each one's map, length, players, and how "
        + "and when every player left. A game where any player left with result 0x01 is flagged "
        + "as a disconnect, which is how a dropped or desynced game is scored without anyone "
        + "writing results down. Reforged autosaves every game under Documents\\Warcraft III\\"
        + "BattleNet\\<account>\\Replays\\Autosaved.")]
    public static ReplayReport ReplaySummary(
        [Description("A .w3g file, or a folder searched recursively. Default: every Battle.net account's replays.")] string? path = null,
        [Description("Only games whose map path contains this text. This is a filter, not a path to read, so a folder cannot be passed here.")] string? map = null)
        => Run(() =>
        {
            if (path is not null && !File.Exists(path) && !Directory.Exists(path))
                throw Wc3Errors.Refuse(Wc3ErrorCodes.InputNotFound, $"no .w3g file or folder at {path}", path);
            return ReplayCommand.Execute(path, map);
        });

    [McpServerTool(Name = "replay_diff", Title = "Compare two replays", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Compare two recorded games (.w3g). Reports whether they played the same map, the "
        + "length difference, and each player's leave time difference, keyed by slot. Read-only.")]
    public static ReplayDiffReport ReplayDiff(
        [Description("Path to the first .w3g recorded game.")] string a,
        [Description("Path to the second .w3g recorded game.")] string b)
        => Run(() => ReplayDiffCommand.Execute(RequireReplay(a), RequireReplay(b)));

    [McpServerTool(Name = "replay_players", Title = "Replay player table", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Per-player table for one recorded game (.w3g). Reports name, slot, result, "
        + "leave time and leave reason. Team and race are not carried by this replay parser, so "
        + "both are omitted rather than guessed. Read-only.")]
    public static ReplayPlayersReport ReplayPlayers(
        [Description("Path to a .w3g recorded game.")] string replay)
        => Run(() => ReplayPlayersCommand.Execute(RequireReplay(replay)));

    [McpServerTool(Name = "replay_timeline", Title = "Replay event timeline", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Ordered event list for one recorded game (.w3g). Reports the game start, each "
        + "chat line and player leave with its reason, and the game end time. Read-only.")]
    public static ReplayTimelineReport ReplayTimeline(
        [Description("Path to a .w3g recorded game.")] string replay)
        => Run(() => ReplayTimelineCommand.Execute(RequireReplay(replay)));

    [McpServerTool(Name = "replay_find_map", Title = "Find a replay's map", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("The map path a replay (.w3g) records, and whether that map file exists in the "
        + "Maps folder, resolving the .w3x or .w3m. Default folder is the user's "
        + "Documents\\Warcraft III\\Maps. Read-only.")]
    public static ReplayFindMapReport ReplayFindMap(
        [Description("Path to a .w3g recorded game.")] string replay,
        [Description("Folder to search for the recorded map. Default Documents\\Warcraft III\\Maps.")] string? maps = null)
        => Run(() => ReplayFindMapCommand.Execute(RequireReplay(replay), maps));

    [McpServerTool(Name = "replay_stats", Title = "Replay statistics", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Aggregate stats for one recorded game (.w3g). Reports duration, player count, "
        + "actions per minute per player when the record carries them, and the disconnect count. Read-only.")]
    public static ReplayStatsReport ReplayStats(
        [Description("Path to a .w3g recorded game.")] string replay)
        => Run(() => ReplayStatsCommand.Execute(RequireReplay(replay)));

    /// <summary>Validate a replay path, the same clean refusal replay_summary gives.</summary>
    private static string RequireReplay(string replay)
    {
        if (string.IsNullOrWhiteSpace(replay))
            throw Missing("replay path");
        if (!File.Exists(replay))
            throw Wc3Errors.Refuse(Wc3ErrorCodes.InputNotFound, $"no .w3g file at {replay}", replay);
        return replay;
    }

    [McpServerTool(Name = "object_list", Title = "List custom objects", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("List the map's custom/modified objects of one Object Editor kind: rawcode, base rawcode (null = created from scratch) and resolved name. Pass brief true to get the total and only the first 50 objects when the full list would be too large for the context.")]
    public static PagedObjects ObjectList(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Object kind: " + KindValues + ".")] string kind = "unit",
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Used to resolve base-game names.")] string? game_dir = null,
        [Description("Return the total object count and only the first 50 objects instead of the full list. Default false.")] bool brief = false)
        => Run(() =>
        {
            ObjectKind parsed;
            try { parsed = ParseKind(kind); }
            catch (McpException ex) when (kind.Equals("trigger", StringComparison.OrdinalIgnoreCase)
                || kind.Equals("script", StringComparison.OrdinalIgnoreCase))
            {
                throw Wc3Errors.Refuse(Wc3ErrorCodes.InvalidArgument, ex.Message
                    + ". A trigger and a script are not object kinds, and triggers_read returns both", kind);
            }
            if (!brief)
            {
                var all = ObjectListCommand.Execute(LoadMap(map), parsed, ResolveGameDir(game_dir)).Items;
                return new PagedObjects(all.Count, all.Count, false, null, all, false, all.Count);
            }
            var page = ObjectListCommand.ExecuteBrief(LoadMap(map), parsed, ResolveGameDir(game_dir));
            return new PagedObjects(page.Items.Count, page.Total, page.Truncated,
                page.Truncated ? page.Items.Count : null, page.Items, page.Truncated, page.Total);
        });

    [McpServerTool(Name = "object_get", Title = "Get an object", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Get an object's merged fields (base game data overlaid by the map's deltas), each labeled 'base' or 'map', with field names resolved. Omit kind to auto-detect it from the rawcode.")]
    public static MergedObjectResult ObjectGet(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Four-character object rawcode, e.g. 'hfoo' or 'u000'.")] string rawcode,
        [Description("Object kind: " + KindValues + ". Default: probe every kind (map deltas first, then base game data).")] string? kind = null,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Without it only the map's own deltas resolve.")] string? game_dir = null)
        => Run(() =>
        {
            if (kind is null)
                return ObjectGetCommand.Execute(LoadMap(map), rawcode, ResolveGameDir(game_dir));
            ObjectKind parsed;
            try { parsed = ParseKind(kind); }
            catch (McpException ex) when (kind.Equals("trigger", StringComparison.OrdinalIgnoreCase)
                || kind.Equals("script", StringComparison.OrdinalIgnoreCase))
            {
                throw Wc3Errors.Refuse(Wc3ErrorCodes.InvalidArgument, ex.Message
                    + ". A trigger and a script are not object kinds, and triggers_read returns both", kind);
            }
            return ObjectGetCommand.Execute(LoadMap(map), parsed, rawcode, ResolveGameDir(game_dir));
        });

    [McpServerTool(Name = "object_form", Title = "Object as an edit form", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Get an object as an editable FORM rather than a flat field dump: fields grouped and ordered the way the World Editor groups them (Art, Combat, Stats, ...), fields that do not apply to this object hidden, each field's legal range and whether a blank is allowed, and which object-data layer it is written to (war3map.* or the war3mapSkin.* twin). Prefer this over object_get when deciding what to edit.")]
    public static ObjectForm ObjectFormTool(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Four-character object rawcode, e.g. 'hfoo' or 'H000'.")] string rawcode,
        [Description("Object kind: " + KindValues + ".")] string kind = "unit",
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Without it fields cannot be grouped, ordered or bounds-checked, and the layer falls back to a measurement of the map itself.")] string? game_dir = null,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => ObjectFormCommand.Execute(LoadMap(map), ParseKind(kind), rawcode, ResolveGameDir(game_dir)));

    private const string UnitInstanceFields =
        "Owner|X|Y|Position|Facing|Scale|HeroLevel|Strength|Agility|Intelligence|"
        + "HpPercent|ManaPercent|Gold|TargetAcquisition";

    private const string DoodadInstanceFields =
        "X|Y|Z|Position|Rotation|Scale|Variation|LifePercent";

    /// <summary>Save-and-report for a placed-instance edit. One helper rather than one per tool,
    /// so every mutating tool refuses and reports identically.</summary>
    private static EditToolResult SaveEdit(
        MapDocument doc, string map, string? outPath, bool overwrite, string? expectInputSha256,
        bool ok, string message)
    {
        if (!ok) throw Rejected(message);
        var w = WriteGate.Save(doc, map, outPath, overwrite, expectInputSha256, LoadedShaOf(doc));
        return new EditToolResult(w.SavedTo, message, w.InputSha256, w.OutputSha256,
            w.FilesRewritten, w.BytesWritten);
    }

    /// <summary>
    /// The inputs a file level write names, so the gate can refuse an output that is one of them.
    /// A null or blank path is dropped rather than passed through.
    /// </summary>
    private static string[] Inputs(params string?[] paths) =>
        paths.Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p!).ToArray();

    /// <summary>
    /// Write a loaded document whose source may be an archive or a folder map. An archive is
    /// staged as one file and renamed. A folder map is staged as one folder and renamed, since a
    /// folder cannot be published as a file (SAF-15).
    /// </summary>
    private static FileWriteOutcome SaveDoc(
        MapDocument doc, string outPath, IReadOnlyList<string> inputs, bool overwrite, string? expectInputSha256)
    {
        // PRF-08. The staged copy lands beside the destination, so the temp budget is that volume.
        RequireTempDisk(outPath);
        return doc.SourceFolder is null
            ? WriteGate.WriteFile(outPath, inputs, overwrite, expectInputSha256, temp => doc.Save(temp))
            : WriteGate.WriteFolder(outPath, inputs, overwrite, expectInputSha256, temp => doc.Save(temp));
    }

    /// <summary>
    /// Gated save of a loaded map to a path the tool computed itself, through the file level gate.
    /// The map gate resolves its own destination, so a tool that names one directly (a report, a
    /// ported copy, a decompiled map) needs this twin to take the same refusals and digests.
    /// </summary>
    private static FileWriteOutcome SaveDocAt(
        MapDocument doc, string outPath, string? map, bool overwrite, string? expectInputSha256)
        => SaveDoc(doc, outPath, Inputs(map), overwrite, expectInputSha256);

    /// <summary>
    /// Run a command that writes its own output, staging the destination first. The command's
    /// report is kept, the gate outcome is merged in, and any path field the report holds is
    /// pointed at the published file rather than the staging path (SAF-15).
    /// </summary>
    private static Dictionary<string, object?> WriteWith<T>(
        string outPath,
        IReadOnlyList<string> inputs,
        bool overwrite,
        string? expectInputSha256,
        Func<string, T> write,
        params string[] pathKeys) where T : class
    {
        // PRF-08. Every staged write in this file passes through here, so the temp disk limit is
        // checked once, at the point the staging folder is about to be created.
        RequireTempDisk(outPath);
        T? report = null;
        var w = WriteGate.WriteFile(outPath, inputs, overwrite, expectInputSha256, temp =>
        {
            report = write(temp);

            // A command that reports a refusal rather than throwing must not have its output
            // published, so the refusal is raised inside the staging step and the temp is dropped.
            if (report.GetType().GetProperty("Ok")?.GetValue(report) is false)
                throw Rejected(report.GetType().GetProperty("Message")?.GetValue(report) as string
                    ?? "the command refused to write the output");
        });
        var gated = Gated(report!, w);
        foreach (string key in pathKeys) gated[key] = w.SavedTo;
        return gated;
    }

    /// <summary>Pairs of staged file and final file for a command that wrote into a staging folder.
    /// The primary is ordered first, so it becomes the reported savedTo (SAF-15).</summary>
    private static List<(string Source, string Target)> StagedPairs(
        string staging, string destDir, string? primaryName = null) =>
        Directory.EnumerateFiles(staging, "*", SearchOption.AllDirectories)
            .Select(src => (Source: src,
                Target: Path.Combine(destDir, Path.GetRelativePath(staging, src))))
            .OrderBy(p => primaryName is not null &&
                          string.Equals(Path.GetFileName(p.Target), primaryName, StringComparison.OrdinalIgnoreCase)
                ? 0 : 1)
            .ToList();

    /// <summary>A staging folder beside the destination, so a publish is a rename on one volume.
    /// PRF-08 checks the temp disk on that same volume, since the staged copy lands beside it.</summary>
    private static string StagingFolder(string finalPath)
    {
        RequireTempDisk(finalPath);
        return Path.Combine(Path.GetDirectoryName(Path.GetFullPath(finalPath)) is { Length: > 0 } d ? d : ".",
            "." + Path.GetFileNameWithoutExtension(finalPath) + "." + Guid.NewGuid().ToString("N") + ".stage");
    }

    /// <summary>
    /// A command's own report with the gate outcome merged in. The five gate fields sit at the top
    /// level beside the report's fields, under the same camelCase names the serializer gives a
    /// record, so every writing tool answers the same way whatever type its report is (SAF-15).
    /// </summary>
    private static Dictionary<string, object?> Gated(object report, FileWriteOutcome w)
    {
        var map = new Dictionary<string, object?>(StringComparer.Ordinal);
        foreach (var p in report.GetType().GetProperties(
                     System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            if (p.GetIndexParameters().Length == 0)
                map[JsonNamingPolicy.CamelCase.ConvertName(p.Name)] = p.GetValue(report);
        map["savedTo"] = w.SavedTo;
        map["inputSha256"] = w.InputSha256;
        map["outputSha256"] = w.OutputSha256;
        map["filesRewritten"] = w.FilesRewritten;
        map["bytesWritten"] = w.BytesWritten;
        if (w.Files.Count > 1)
            map["files"] = WithRoles(w.Files);
        return map;
    }

    /// <summary>
    /// Name every file a multi file write published, keeping the gate's order so the primary output
    /// stays first behind savedTo (RES-11).
    /// </summary>
    private static IReadOnlyList<OutputFile> WithRoles(IReadOnlyList<WrittenFile> files) =>
        files.Select(f => new OutputFile(RoleOf(f.Path), f.Path, f.Sha256)).ToList();

    /// <summary>The role a published file plays, taken from what the file is rather than where it
    /// sits, so a texture beside an export is named as a texture.</summary>
    private static string RoleOf(string path) => Path.GetExtension(path).ToLowerInvariant() switch
    {
        ".obj" => "obj mesh",
        ".mtl" => "material",
        ".blp" or ".dds" or ".png" or ".tga" or ".jpg" or ".jpeg" => "texture",
        ".mdx" or ".mdl" => "model",
        ".w3x" or ".w3m" => "map",
        ".json" => "json report",
        ".csv" => "csv table",
        ".j" or ".lua" => "script",
        _ => "output",
    };

    /// <summary>
    /// The files a port read and the one it wrote, each with the role it plays, so the two source
    /// maps are not lost behind the single savedTo (RES-11).
    /// </summary>
    private static IReadOnlyList<OutputFile> PortFiles(
        string sourceMap, string targetMap, FileWriteOutcome w, string outputRole) =>
        new[]
        {
            new OutputFile("source map", Path.GetFullPath(sourceMap), ShaOrEmpty(sourceMap)),
            new OutputFile("target map", Path.GetFullPath(targetMap), ShaOrEmpty(targetMap)),
            new OutputFile(outputRole, w.SavedTo, w.OutputSha256),
        };

    /// <summary>The digest of a file on disk, or an empty string for a folder map, which has no
    /// single archive to hash.</summary>
    private static string ShaOrEmpty(string path) => File.Exists(path) ? WriteGate.Sha256File(path) : "";


    private const string TerrainCornerFieldNames =
        "GroundHeight|AddGroundHeight|WaterHeight|GroundTexture|CliffLevel";

    [McpServerTool(Name = "object_field_options", Title = "List legal field values", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("The legal values for one object-data field, taken from what the base game data actually uses for it. Ask this BEFORE object_set on an enumerated field, so the value written is one the game already accepts rather than a guess.")]
    public static ObjectFieldOptionsResult ObjectFieldOptions(
        [Description("Object kind: " + KindValues + ".")] string kind,
        [Description("The field's four-character code, e.g. 'utyp' or 'ua1t'.")] string field,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Required for this to return anything, since the options come from the base data.")] string? game_dir = null)
        => Run(() => ObjectFieldOptionsCommand.Execute(ParseKind(kind), field, ResolveGameDir(game_dir)));

    [McpServerTool(Name = "asset_list", Title = "List valid asset paths", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("List the asset paths an icon or model object field can actually be set to, from the map's own imported files and from the base game. Ask this BEFORE setting a model or icon field, because a field naming a path that does not resolve renders as nothing and reports no error anywhere.")]
    public static AssetListResult AssetList(
        [Description("Asset family: icon or model. It matches the field's metadata type token.")] string family,
        [Description("Optional path to a .w3x/.w3m map, or to an unpacked map folder. Without one, only the base game's paths are listed.")] string? map = null,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Without it only the map's own paths are listed.")] string? game_dir = null)
        => Run(() => AssetListCommand.Execute(
            map is null ? null : LoadMap(map),
            AssetListCommand.ParseFamily(family),
            ResolveGameDir(game_dir)));

    [McpServerTool(Name = "asset_unused", Title = "Find unused imports", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("List the map's imported files that nothing references, each with its size. A file counts as referenced when an object field, a war3mapSkin.txt or war3mapMisc.txt value, a script literal, an imported model's texture chunk, a path embedded in a model's bytes, an imported .fdf or .toc, or a map format file names it. The map format scan matters, since the w3i names the loading screen, the w3e every terrain tile and the w3s every sound, so without it those read as unused. Treat the list as leads, as the base game can load a file none of these name.")]
    public static AssetUnusedResult AssetUnused(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => AssetUnusedCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "asset_report", Title = "Full asset report", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("One report over every imported file in a map. Lists each entry with its size and whether anything references it, plus every fault the asset checks and the model scan find, an empty entry, an oversized or non power of two texture, a model without source, an import referenced nowhere, byte identical duplicate groups, and model lint faults (geoset without material, missing texture, zero duration sequence, bounding box out of range). The engine logs nothing about what it loads, so this is the full answer a map gives. ModelWithoutSource fires for nearly every compiled .mdx, so read it as a lead.")]
    public static AssetReportResult AssetReport(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Run the per-model structural scan, the slowest part. Default true.")] bool model_lint = true,
        [Description("Also read War3Log.txt and fold the engine's own faults into the report, a texture it replaced with a solid colour, a model it could not create, a UI frame or frame string defined twice, and any other line it wrote as an error. Default false.")] bool log = false,
        [Description("Path to War3Log.txt. Default Documents\\Warcraft III\\Logs\\War3Log.txt.")] string? log_path = null)
        => Run(() =>
        {
            var logPath = log_path ?? (log
                ? War3LogFaults.DefaultPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
                : null);
            return AssetReportCommand.Execute(LoadMap(map), map, includeModelLint: model_lint, logPath: logPath);
        });

    [McpServerTool(Name = "asset_duplicates", Title = "Find duplicate imports", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Group the map's imported files whose bytes are identical (by sha256) and report the space each duplicate group wastes. Zero-byte entries are excluded. Read-only.")]
    public static AssetDuplicatesResult AssetDuplicates(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => AssetDuplicatesCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "asset_audit", Title = "Flag suspicious assets", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Flag suspicious imported assets. It reports entries larger than max_bytes, BLP textures whose width or height is not a power of two, a binary .mdx with no .mdl source beside it, and zero-byte entries. Read-only.")]
    public static AssetAuditResult AssetAudit(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Flag imported entries larger than this many bytes. Default 4194304 (4 MiB).")] long max_bytes = AssetAuditCommand.DefaultMaxBytes)
        => Run(() => AssetAuditCommand.Execute(LoadMap(map), max_bytes));

    [McpServerTool(Name = "asset_size_report", Title = "Imported bytes by folder", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Break the map's imported bytes down by parent folder, largest total first, so the folders that dominate the map's weight are obvious. Read-only.")]
    public static AssetSizeReportResult AssetSizeReport(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => AssetSizeReportCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "asset_replace", Title = "Replace an asset path", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Replace an asset path everywhere it is referenced (object-data field values, script string literals and the war3map.imp import table) and save the edited copy to out_path. Refuses when the old path is referenced nowhere. The input map is NEVER modified, and the archive entry itself is NOT renamed, move the file if the new path must resolve.")]
    public static object AssetReplace(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("The asset path currently referenced.")] string old_path,
        [Description("The asset path to reference instead.")] string new_path,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run<object>(() =>
        {
            string full = ResolveOutPath(map, out_path);
            var doc = LoadMap(map);
            var r = AssetReplaceCommand.Execute(doc, old_path, new_path);
            if (!r.Ok) throw Rejected(r.Message);
            return Gated(r, SaveDocAt(doc, full, map, overwrite, expect_input_sha256));
        });

    [McpServerTool(Name = "asset_model_lint", Title = "Lint imported models", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Scan every imported model (.mdx/.mdl) in the map and report the structural problems a static check can find, a geoset with no material, a material naming a texture the map does not hold, a sequence with a zero duration, and an out of range bounding box. Read-only, it parses bytes in memory and changes nothing.")]
    public static AssetModelLintResult AssetModelLint(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => AssetModelLintCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "asset_thumbnails", Title = "Render model thumbnails", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Render a thumbnail PNG for every model in a folder, or for every imported model inside every map in the folder, and write them under out_dir. Reuses the model renderer. The source folder and every map it holds are never modified. Returns the output directory and the file names.")]
    public static AssetThumbnailsToolResult AssetThumbnails(
        [Description("Folder holding model (.mdx/.mdl) files, or map (.w3x/.w3m) files whose imported models are rendered.")] string folder,
        [Description("Directory the PNG thumbnails are written under. Created if absent.")] string out_dir,
        [Description("Thumbnail edge length in pixels. Default 256.")] int size = AssetThumbnailsCommand.DefaultSize,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null,
        [Description("Cancels the call at the next check point. The render itself is not interruptible once it starts.")] CancellationToken cancellationToken = default)
        => Run(() =>
        {
            if (string.IsNullOrWhiteSpace(out_dir)) throw Missing("out_dir");
            RequireTempDisk(out_dir);
            CheckPoint(cancellationToken, "the render start");
            var r = AssetThumbnailsCommand.Execute(folder, size, size);
            CheckPoint(cancellationToken, "the publish");
            var w = WriteGate.WriteFolder(out_dir, Inputs(folder), overwrite, expect_input_sha256, temp =>
            {
                Directory.CreateDirectory(temp);
                foreach (var t in r.Thumbnails) File.WriteAllBytes(Path.Combine(temp, t.Name), t.Png);
            });
            return new AssetThumbnailsToolResult(w.SavedTo, r.Rendered, r.Thumbnails.Select(t => t.Name).ToList(),
                r.Diagnostics, w.InputSha256, w.OutputSha256, w.FilesRewritten, w.BytesWritten);
        });

    [McpServerTool(Name = "asset_convert_batch", Title = "Convert textures in bulk", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Convert every texture in a folder to one target format, reusing the shared converter, and write the copies under out_dir with their relative paths preserved. The source folder is never modified. Returns the output directory, the count and the file names.")]
    public static AssetConvertBatchToolResult AssetConvertBatch(
        [Description("Folder holding texture files.")] string folder,
        [Description("Target format extension, e.g. .png or .blp.")] string to,
        [Description("Directory the converted textures are written under. Created if absent.")] string out_dir,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null,
        [Description("Cancels the call at the next check point. The conversion itself is not interruptible once it starts.")] CancellationToken cancellationToken = default)
        => Run(() =>
        {
            if (string.IsNullOrWhiteSpace(out_dir)) throw Missing("out_dir");
            RequireTempDisk(out_dir);
            CheckPoint(cancellationToken, "the conversion start");
            var r = AssetConvertBatchCommand.Execute(folder, to);
            CheckPoint(cancellationToken, "the publish");
            var w = WriteGate.WriteFolder(out_dir, Inputs(folder), overwrite, expect_input_sha256, temp =>
            {
                Directory.CreateDirectory(temp);
                foreach (var t in r.Textures)
                {
                    var dest = Path.Combine(temp, t.RelativePath);
                    if (Path.GetDirectoryName(dest) is { Length: > 0 } dir) Directory.CreateDirectory(dir);
                    File.WriteAllBytes(dest, t.Bytes);
                }
            });
            var failed = SkippedItems(r.Diagnostics);
            return new AssetConvertBatchToolResult(w.SavedTo, r.Converted, r.ToExtension,
                r.Textures.Select(t => t.RelativePath).ToList(), r.Diagnostics,
                w.InputSha256, w.OutputSha256, w.FilesRewritten, w.BytesWritten,
                failed, failed.Count > 0);
        });

    [McpServerTool(Name = "asset_import_wire", Title = "Import and wire assets", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Add disk files to a map as imports and, when a file is a model or an icon and object names an object, point its matching model or icon field at the new import. Saves the edited copy to out_path. The input map is NEVER modified. Returns the imported paths and the fields set.")]
    public static AssetImportWireToolResult AssetImportWire(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("One or more disk files to add as imports. At most 1000, and a longer list is refused with a tool error naming the limit.")] string[] files,
        [Description("Four-character rawcode of the object whose model or icon field is pointed at each imported file. Omit to import without setting a field.")] string? obj = null,
        [Description("Object type, one of unit|item|destructable|doodad. Defaults to auto-detect from the object.")] string? kind = null,
        [Description("Keep each file's path relative to this directory instead of flattening to a single folder. A war3mapimported folder is normalised to war3mapImported. Use this for a sound or asset pack that must land on its original subfolders.")] string? base_dir = null,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run(() =>
        {
            RequireAtMost(files.Length, "files");
            string full = ResolveOutPath(map, out_path);
            var doc = LoadMap(map);
            var r = AssetImportWireCommand.Execute(doc, files, obj, kind is null ? null : ParseKind(kind), base_dir);
            if (!r.Ok) throw Rejected(r.Message);
            var w = SaveDocAt(doc, full, map, overwrite, expect_input_sha256);
            return new AssetImportWireToolResult(w.SavedTo, r.Imported, r.FieldsSet, r.Diagnostics,
                w.InputSha256, w.OutputSha256, w.FilesRewritten, w.BytesWritten);
        });

    [McpServerTool(Name = "model_squish", Title = "Shrink a model with a render check", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("A lossy model run with a render gate. Applies the lossless key drop, then "
        + "quadric decimation, renders the original and each candidate from four angles after "
        + "every pass, and keeps a pass only while SSIM and silhouette stay above the floor. "
        + "Writes a receipt per pass. Optionally shrinks the model's own textures into a side "
        + "folder. The input is never modified.")]
    public static object ModelSquish(
        [Description("Path to a .mdx or .mdl file.")] string model,
        [Description("Output model path.")] string out_path,
        [Description("Target face fraction for decimation, 0 to 1.")] float ratio = 0.5f,
        [Description("Shrink the model's textures to this maximum side. 0 leaves them alone.")]
        int texture_side = 0,
        [Description("Folder for the shrunk textures. Required with texture_side.")]
        string? texture_out = null,
        [Description("Render gate, the minimum mean SSIM.")] double min_ssim = 0.90,
        [Description("Render gate, the minimum silhouette overlap.")] double min_silhouette = 0.85,
        [Description("Skip the lossless key drop.")] bool no_lossless = false,
        [Description("Skip the decimation pass.")] bool no_decimate = false,
        [Description("Strip effect chunks during decimation.")] bool strip_effects = false,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run<object>(() =>
        {
            string full = Path.GetFullPath(out_path);
            string staging = StagingFolder(full);
            string texStaging = staging + "-tex";
            Directory.CreateDirectory(staging);
            try
            {
                if (texture_side != 0)
                {
                    if (string.IsNullOrWhiteSpace(texture_out))
                        throw Invalid("a texture side needs texture_out, the folder the copies land in", "texture_out");
                    Directory.CreateDirectory(texStaging);
                }

                var r = ModelSquishCommand.Execute(model, Path.Combine(staging, Path.GetFileName(full)), ratio,
                    !no_lossless, !no_decimate, texture_side, texture_side != 0 ? texStaging : null,
                    min_ssim, min_silhouette, strip_effects);

                var pairs = new List<(string Source, string Target)>
                {
                    (Path.Combine(staging, Path.GetFileName(full)), full),
                };
                if (texture_side != 0)
                    pairs.AddRange(StagedPairs(texStaging, Path.GetFullPath(texture_out!)));

                var w = WriteGate.Publish(pairs, Inputs(model), overwrite, expect_input_sha256);
                var gated = Gated(r, w);
                gated["outPath"] = w.SavedTo;
                return gated;
            }
            finally
            {
                try { Directory.Delete(staging, recursive: true); } catch { /* best effort */ }
                try { if (Directory.Exists(texStaging)) Directory.Delete(texStaging, recursive: true); }
                catch { /* best effort */ }
            }
        });

    [McpServerTool(Name = "model_primitive", Title = "Generate a primitive mesh", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Generate a primitive mesh and write a real MDX at out_path. Kinds are card, ring, cone and capsule. The output has a geoset, a material and a texture reference, so it loads like any imported model. Use it for effects and placeholders without an external asset. A curved kind with fewer than three segments is refused.")]
    public static object ModelPrimitive(
        [Description("Primitive kind: card|ring|cone|capsule.")] string kind,
        [Description("Output .mdx path.")] string out_path,
        [Description("Overall size, radius or edge length depending on the kind. Default 32.")] float size = 32f,
        [Description("Length along Z for cone and capsule. Zero uses the size.")] float length = 0f,
        [Description("Inner radius for a ring. Zero uses half the size.")] float inner_radius = 0f,
        [Description("Segments around the curve. Default 16, below 3 is refused.")] int segments = 16,
        [Description("Texture map path written into the model, for example war3mapImported\\glow.blp.")] string? texture_path = null,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run<object>(() => WriteWith(out_path, Array.Empty<string>(), overwrite, expect_input_sha256,
            temp => MeshPrimitiveCommand.Execute(
                ParseEnum<MeshPrimitiveKind>(kind, nameof(kind)),
                temp, size, length, inner_radius, segments, texture_path), "path"));

    [McpServerTool(Name = "model_compare", Title = "Compare two model renders", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Render two models from four angles and score how close they look, by SSIM on "
        + "the shaded frame and silhouette intersection-over-union, per angle and as a mean and "
        + "worst case. The gate a lossy model step has to pass. Read-only.")]
    public static ModelCompareResult ModelCompare(
        [Description("Path to the first .mdx or .mdl file.")] string a,
        [Description("Path to the second .mdx or .mdl file.")] string b,
        [Description("Render width in pixels.")] int width = 256,
        [Description("Render height in pixels.")] int height = 256,
        [Description("Folder to resolve textures from for both. Omit to use each model's own folder.")]
        string? textures = null)
        => Run(() => ModelCompareCommand.Execute(a, b, width, height, textureFolder: textures));

    [McpServerTool(Name = "model_lint", Title = "Lint one model", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Lint one .mdx or .mdl for the structural problems the game renders wrongly or "
        + "drops in silence: non-finite vertices, normals or UVs, zero-length normals, indices past "
        + "the vertex array, zero-area triangles, skin bones the skeleton does not define, weight "
        + "sets that do not sum to one, non-finite animation keys and textures no geoset references. "
        + "Read-only.")]
    public static ModelLintResult ModelLint(
        [Description("Path to a .mdx or .mdl file.")] string model)
        => Run(() => ModelLintCommand.ExecutePath(model));

    [McpServerTool(Name = "model_audit", Title = "Audit model waste", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Audit a .mdx or .mdl model for the resource waste and draw-nothing problems an optimiser can remove, and report what it holds. Finds duplicate textures, geosets with no texture or no triangles, zero-area faces, unused skeleton nodes and non-finite vertices. Counts the effects an MDX carries, particle emitters, particle emitters 2, ribbons and popcorns, from the binary chunk list, and reads each particle emitter's payload to report one that references a node the model does not define or a texture outside its texture list. Read-only.")]
    public static ModelAuditResult ModelAudit(
        [Description("Path to a .mdx or .mdl model file.")] string model)
        => Run(() => ModelAuditCommand.ExecutePath(model));

    [McpServerTool(Name = "model_optimize", Title = "Decimate model geometry", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Decimate a .mdx or .mdl model's geometry with the quadric error metric, drop zero-area faces and duplicate textures, and write a smaller MDX at out_path, leaving the other chunks untouched. ratio is the target fraction of faces to keep. Set strip_effects to also drop the model's effect chunks, particle emitters (PREM), particle emitters 2 (PRE2), ribbons (RIBB) and corns (CORN), which changes how the model looks, so it defaults to false. Returns the before and after vertex and face counts and the number of effect chunks removed. The input is never modified.")]
    public static object ModelOptimize(
        [Description("Path to a .mdx or .mdl model file.")] string model,
        [Description("Output .mdx path. The input is never modified.")] string out_path,
        [Description("Target fraction of faces to keep, between 0 and 1.")] float ratio = 0.5f,
        [Description("Drop the model's effects (particle emitters, particle emitters 2, ribbons and corns) when true. Changes how the model looks. Default false.")] bool strip_effects = false,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run<object>(() => WriteWith(out_path, Inputs(model), overwrite, expect_input_sha256,
            temp => ModelOptimizeCommand.ExecutePath(model, ratio, temp, strip_effects), "path", "outPath"));

    [McpServerTool(Name = "model_lod", Title = "Build an LOD chain", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Generate a level-of-detail chain for a .mdx or .mdl model, writing one decimated MDX per ratio into out_dir. ratios is a comma separated list of face fractions to keep, each between 0 and 1, processed in order, so a descending list gives a descending chain. Every level keeps the model's materials, textures, sequences, bones, emitters and cameras byte for byte and shrinks only geometry. Ratio 1 is the original and its file is byte identical to the input. The input is never modified. Returns each level's counts and byte size.")]
    public static object ModelLod(
        [Description("Path to a .mdx or .mdl model file.")] string model,
        [Description("Comma-separated target fractions of faces to keep, each between 0 and 1, for example 1,0.5,0.25. At most 1000 ratios, and a longer list is refused with a tool error naming the limit.")] string ratios,
        [Description("Output directory for the LOD chain. Created when missing. The input is never modified.")] string out_dir,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run<object>(() =>
        {
            RequireAtMost(ratios.Split(',').Count(part => part.Trim().Length > 0), "ratios");
            ModelLodResult? r = null;
            var w = WriteGate.WriteFolder(out_dir, Inputs(model), overwrite, expect_input_sha256,
                temp => r = ModelLodCommand.ExecutePath(model, ratios, temp));
            var gated = Gated(r!, w);
            gated["outputDirectory"] = w.SavedTo;
            gated["levels"] = r!.Levels
                .Select(l => l with { Path = Path.Combine(w.SavedTo, Path.GetFileName(l.Path)) })
                .ToList();
            return gated;
        });

    [McpServerTool(Name = "texture_optimize", Title = "Shrink a texture", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Shrink a .blp, .dds or standard image texture to fit a maximum side, preserving aspect ratio, and write it at out_path in the same format. The input is never modified. Returns the before and after dimensions and byte sizes.")]
    public static object TextureOptimize(
        [Description("Path to a texture file.")] string texture,
        [Description("Output path. The input is never modified.")] string out_path,
        [Description("Maximum side length in pixels.")] int max = 256,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run<object>(() => WriteWith(out_path, Inputs(texture), overwrite, expect_input_sha256,
            temp => TextureOptimizeCommand.ExecutePath(texture, max, temp), "path"));

    [McpServerTool(Name = "texture_atlas", Title = "Pack textures into an atlas", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Pack two or more .blp, .dds or standard image textures into one power-of-two atlas and write it at out_path in the first input's format. Every input is decoded, placed deterministically by a shelf packer, and left unmodified. Returns the atlas size, its byte count and each input's pixel rectangle, so a caller can build the UV mapping.")]
    public static object TextureAtlas(
        [Description("Two or more paths to texture files. At most 1000, and a longer list is refused with a tool error naming the limit.")] string[] images,
        [Description("Output atlas path. The inputs are never modified.")] string out_path,
        [Description("Maximum atlas side in pixels. The atlas is the smallest power of two that fits and does not exceed this.")] int max = 512,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run<object>(() =>
        {
            RequireAtMost(images.Length, "images");
            return WriteWith(out_path, images, overwrite, expect_input_sha256,
                temp => TextureAtlasCommand.ExecutePaths(images, max, temp), "output");
        });

    [McpServerTool(Name = "map_optimize", Title = "Shrink textures in a map", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Shrink every imported texture in a map in one pass and write a new map to out_path, replacing only those files and keeping the rest byte for byte. Models are left untouched by default and each texture is scaled to fit max_texture on its longest side. Pass ratio below 1 to decimate models too, which is unsafe by default because a decimated model once crashed the game on load with an ACCESS_VIOLATION, so load-test the result. A file whose optimised bytes are not smaller is left as it was. The input map is never modified.")]
    public static object MapOptimize(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map path. The input is never modified.")] string out_path,
        [Description("Target fraction of a model's faces to keep, between 0 and 1. Default 1.0, which leaves every model byte for byte and shrinks only textures. A value below 1 decimates models, which needs a load test because it can produce a model the game crashes on.")] float ratio = 1.0f,
        [Description("Maximum texture side in pixels. A texture larger than this on either side is scaled down. Default 256.")] int max_texture = 256,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null,
        [Description("Cancels the call at the next check point. The per texture optimisation inside the map is not interruptible.")] CancellationToken cancellationToken = default)
        => Run<object>(() =>
        {
            string full = ResolveOutPath(map, out_path);
            CheckPoint(cancellationToken, "the optimisation start");
            return WriteWith(full, Inputs(map), overwrite, expect_input_sha256,
                temp =>
                {
                    CheckPoint(cancellationToken, "the staging copy");
                    return MapOptimizeCommand.ExecutePath(map, ratio, max_texture, temp);
                }, "path");
        });

    [McpServerTool(Name = "model_export_obj", Title = "Export a model to OBJ", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Export a map-imported model to Wavefront OBJ at out_path, writing an MTL and any map-resolvable textures converted to PNG beside it. The map itself is never modified. Every file is staged beside the target and published only when all of them are complete, so a failure leaves no partial export. An existing .obj, .mtl or texture is refused and left unchanged unless overwrite is true. Returns the .obj path, the .mtl path, the texture names and any publish note.")]
    public static object ModelExportObj(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Internal model path (.mdx/.mdl) inside the map.")] string model,
        [Description("Output .obj file path. A .mtl and the converted textures are written beside it.")] string out_path,
        [Description("Replace an .obj, .mtl or texture that already exists beside out_path. Default false, which refuses an existing target and leaves it exactly as it was.")] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null,
        [Description("Cancels the call at the next check point. The export itself is not interruptible, and a cancel after it finishes still publishes nothing.")] CancellationToken cancellationToken = default)
        => Run<object>(() =>
        {
            var doc = LoadMap(map);
            string full = Path.GetFullPath(out_path);
            string staging = StagingFolder(full);
            Directory.CreateDirectory(staging);
            try
            {
                CheckPoint(cancellationToken, "the export start");
                var r = ModelExportObjCommand.Write(doc, model, Path.Combine(staging, Path.GetFileName(full)), false);
                CheckPoint(cancellationToken, "the publish");
                var w = WriteGate.Publish(
                    StagedPairs(staging, Path.GetDirectoryName(full) ?? ".", Path.GetFileName(full)),
                    Inputs(map), overwrite, expect_input_sha256);
                var gated = Gated(r, w);
                gated["mtl"] = Path.Combine(Path.GetDirectoryName(w.SavedTo)!,
                    r.Mtl is null ? Path.GetFileNameWithoutExtension(w.SavedTo) + ".mtl" : Path.GetFileName(r.Mtl));
                return gated;
            }
            finally
            {
                try { Directory.Delete(staging, recursive: true); } catch { /* best effort */ }
            }
        });

    [McpServerTool(Name = "model_import", Title = "Import a foreign model", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Import a foreign model into a Warcraft III MDX at out_path. Reads OBJ, glTF and GLB, STL, PLY and ASCII FBX, detecting the format from content so the extension does not matter, and refuses binary FBX. One geoset is built per source primitive with normals and UVs, each glTF base colour texture becomes a .blp beside the MDX with one entry per distinct texture, and each glTF animation becomes a named sequence. Bone keyframes are not written. The input is never modified, and an existing output is refused without overwrite.")]
    public static object ModelImport(
        [Description("Path to a foreign model file, one of OBJ, glTF, GLB, STL, PLY or ASCII FBX.")] string obj,
        [Description("Output .mdx path. The input is never modified.")] string out_path,
        [Description("Optional texture path to embed, relative to the map, overriding every material. Omit to use the model's own materials, or war3mapImported\\<name>.blp when it names none.")] string? texture = null,
        [Description("Frames per second used to convert a glTF animation's time range into a sequence frame interval. The Warcraft III default is 30.")] double fps = 30,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null,
        [Description("Cancels the call at the next check point. The import reads the token before the file it writes, so it stops at that file, removes what it wrote and publishes nothing.")] CancellationToken cancellationToken = default,
        IProgress<ProgressNotificationValue>? progress = null)
        => Run<object>(() =>
        {
            string full = Path.GetFullPath(out_path);
            string staging = StagingFolder(full);
            Directory.CreateDirectory(staging);
            try
            {
                CheckPoint(cancellationToken, "the import start");
                ServerRuntime.ReportProgress(progress, "model_import", "started, reading the input");
                var r = ModelImportCommand.ExecutePath(obj, Path.Combine(staging, Path.GetFileName(full)),
                    texture, fps, cancellationToken,
                    (done, total) => ServerRuntime.ReportProgress(
                        progress, "model_import", "wrote " + done + " of " + total + " files", done, total));
                CheckPoint(cancellationToken, "the publish");
                var w = WriteGate.Publish(
                    StagedPairs(staging, Path.GetDirectoryName(full) ?? ".", Path.GetFileName(full)),
                    Inputs(obj), overwrite, expect_input_sha256);
                return Gated(r, w);
            }
            finally
            {
                try { Directory.Delete(staging, recursive: true); } catch { /* best effort */ }
            }
        });

    [McpServerTool(Name = "lint", Title = "Pre-flight a map", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Pre-flight the map: does its script compile, does war3map.imp agree with what the archive holds, do referenced asset paths resolve, and is every entry a loadable type. This is the 'is this map going to work' check and is the right thing to run before handing a map to anyone. A failure here usually means the map will not host or will hang on load with no error of its own.")]
    public static LintResult Lint(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Optional path to the ORIGINAL map this one was built from. Supplied, the lint also reports what a rebuild lost or clobbered.")] string? against = null)
        => Run(() => LintCommand.Run(LoadMap(map), against is null ? null : LoadMap(against)));

    [McpServerTool(Name = "validate", Title = "Validate map structure", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Check the map for structural problems: missing or empty files, entries that fail to load, and a script that cannot compile. Narrower than lint and a good first question when a map behaves as though something is absent.")]
    public static ValidateResult Validate(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => ValidateCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "validate_strict", Title = "Strict map validation", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("A stricter validate. Runs the default checks and adds three that are too strict to be a default, a map with no (listfile), a script that declares no entry point function, and a placement whose type neither the map nor the installed game defines. The undefined-type check needs the installed game and reports a diagnostic rather than a clean result when it cannot run. The default validate is unchanged.")]
    public static ValidateStrictResult ValidateStrict(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Without it the undefined-type check is skipped, with a diagnostic.")] string? game_dir = null)
        => Run(() => ValidateStrictCommand.Execute(LoadMap(map), ResolveGameDir(game_dir)));

    [McpServerTool(Name = "lint_baseline", Title = "Write a lint baseline", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Write the map's current lint findings to a baseline JSON file. A later gate, the CLI 'lint <map> --baseline <file>', then fails only on a finding this file does not already hold, so known and accepted warnings do not block CI. Writes a local file, never the map.")]
    public static object LintBaseline(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("File to write the baseline JSON to.")] string out_path,
        [Description("Optional path to the ORIGINAL map this one was built from, so the baseline records the before/after checks too.")] string? against = null,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run<object>(() => WriteWith(out_path, Inputs(map, against), overwrite, expect_input_sha256,
            temp => LintBaselineCommand.Write(map, temp, against), "outPath"));

    [McpServerTool(Name = "roundtrip", Title = "Check a byte faithful roundtrip", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Verify the map survives a load and save byte for byte. Any file that differs is one this toolkit does not reproduce faithfully, which is worth knowing BEFORE editing that map, since a save would then rewrite it. Archive bookkeeping the container regenerates is excluded by design.")]
    public static RoundtripResult Roundtrip(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => RoundtripCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "search", Title = "Search map contents", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Search the map's contents for a string, ignoring case: archive file names, script lines (with the line number), string-table entries, and object data (custom object rawcodes and every field value). Each hit names the file and where in it. Capped at 500 hits, with truncated set when the cap was reached. Use this to find where a name, an ability id or an asset path is actually referenced, rather than guessing which file holds it.")]
    public static SearchResult Search(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Text to look for.")] string query)
        => Run(() =>
        {
            try { return SearchCommand.Execute(LoadMap(map), query); }
            catch (McpException ex) when (ex.Message.StartsWith("map must be a .w3x or .w3m file", StringComparison.Ordinal))
            {
                throw Wc3Errors.Refuse(Wc3ErrorCodes.MapUnreadable, ex.Message
                    + ". A recorded game is read by replay_summary, which takes the .w3g path directly", map);
            }
        });

    [McpServerTool(Name = "diff", Title = "Diff two maps", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Compare two maps and report which internal files were added, removed or changed. The direct way to see what an edit actually did to a map, and to check that it touched only what was intended.")]
    public static DiffResult Diff(
        [Description("Path to the first .w3x/.w3m map, or to an unpacked map folder.")] string map,
        [Description("Path to the second .w3x/.w3m map.")] string other)
        => Run(() => DiffCommand.Execute(LoadMap(map), LoadMap(other)));

    [McpServerTool(Name = "script_functions", Title = "List script functions", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("List every function declared in the map's compiled script, with where it starts and ends. Indentation-tolerant, so it finds declarations a naive line scan misses. Use it to locate a handler before reading or editing the script.")]
    public static ScriptFunctionsResult ScriptFunctions(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => ScriptCommand.Functions(LoadMap(map)));

    [McpServerTool(Name = "script_references", Title = "Find references to a name", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Find every use of a name in the map's compiled script: direct calls, and the places it is passed as a code value (Condition, Filter, TriggerAddAction). The second kind is how most trigger handlers are actually reached and a text search for 'name(' cannot see it, so use this rather than search when asking what reaches a function. Each result carries its line, the function containing it, and the line text.")]
    public static ScriptReferencesResult ScriptReferences(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Function or global name to find uses of.")] string name)
        => Run(() => ScriptCommand.References(LoadMap(map), name));

    [McpServerTool(Name = "script_check", Title = "Check script syntax", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Check the map script for errors that stop it loading, war3map.j or war3map.lua. Lua is checked against Lua 5.3 exactly as luac 5.3.6 would, with its line and message, JASS by the built-in JASS checks. A script that fails here cannot run, and the game gives no reason, it just shows no slots. A Lua script that carries the Wurst package runtime is reported as a warning, because a JASS-to-Lua conversion of a Wurst map does not preserve package initialization and a spell can then fail in game with \"Could not initialize package\".")]
    public static ScriptCheckResult ScriptCheck(
        [Description("Path to a .w3x/.w3m map file or map folder.")] string map)
        => Run(() => ScriptCommand.Check(LoadMap(map)));

    [McpServerTool(Name = "script_format", Title = "Reformat a Lua script", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Reprint a Lua map script and save the map to out_path. pretty is indented with comments kept, canonical is one fixed form for diffing two builds. The result compiles to the same Lua instructions as the original. JASS maps are refused. The input map is NEVER modified, and nothing is written when the script is already in that form.")]
    public static object ScriptFormat(
        [Description("Path to a .w3x/.w3m map file or map folder.")] string map,
        [Description("Where to save the reformatted map.")] string out_path,
        [Description("pretty or canonical.")] string style = "pretty",
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run(() =>
        {
            var full = ResolveOutPath(map, out_path);
            var doc = LoadMap(map);
            var r = ScriptCommand.Format(doc, style);
            var w = r.Changed ? SaveDocAt(doc, full, map, overwrite, expect_input_sha256) : null;
            return new { r.ScriptFile, r.Style, r.Changed, r.BytesBefore, r.BytesAfter,
                SavedTo = w?.SavedTo, InputSha256 = w?.InputSha256, OutputSha256 = w?.OutputSha256 ?? "",
                FilesRewritten = w?.FilesRewritten ?? 0, BytesWritten = w?.BytesWritten ?? 0 };
        });

    [McpServerTool(Name = "script_to_lua", Title = "Convert JASS to Lua", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Convert a JASS map to a Lua map and save it to out_path. The script becomes war3map.lua and keeps the JASS rules Lua lacks (32 bit integer wrap, integer division toward zero, divide by zero gives zero, a null string concatenates as empty, unset array slots read their type default), war3map.j becomes a stub, and the w3i language is set to Lua so the game runs it. Lua maps are refused, as is a WurstScript script, whose package runtime does not survive conversion, unless allow_wurst is true. Pass strict_reals to round real arithmetic back to 32 bit.")]
    public static object ScriptToLua(
        [Description("Path to a .w3x/.w3m map file or map folder.")] string map,
        [Description("Where to save the converted map.")] string out_path,
        [Description("Round real (floating point) arithmetic to the 32 bit float JASS uses, so a converted expression does not drift from the original. Off by default, which reproduces the upstream converter byte for byte.")] bool strict_reals = false,
        [Description("Rewrite a hashtable that is local to one function and used only through the hashtable natives into a plain Lua table, which is what Lua should use. A hashtable passed to another function or a native stays a real hashtable, since a native still has to receive one. Off by default.")] bool hashtables = false,
        [Description("Convert even when the script was generated by WurstScript. Wurst's package runtime does not survive the conversion, and the map fails in game with \"Could not initialize package\" when a spell runs. Off by default. Wurst compiles to Lua directly, so rebuild the Wurst project with its own Lua target instead.")] bool allow_wurst = false,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run<object>(() =>
        {
            var full = ResolveOutPath(map, out_path);
            var doc = LoadMap(map);
            var r = JassToLuaCommand.Convert(doc, strictReals: strict_reals, hashtables: hashtables,
                allowWurst: allow_wurst);
            return Gated(r, SaveDocAt(doc, full, map, overwrite, expect_input_sha256));
        });

    [McpServerTool(Name = "script_decompile", Title = "Decompile Lua bytecode", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Turn every Lua entry of a map that ships as precompiled Lua 5.3 bytecode (war3map.lua or a module it requires, common in YDWE maps) back into Lua source that runs the same, and save the map to out_path. The source is a transcription of the bytecode, one statement per instruction with gotos, not the author's original text. Both standard Lua 5.3 and the KK YDWE engine's opcode numbering are read. Nothing is written when the map holds no bytecode. The input map is NEVER modified.")]
    public static object ScriptDecompile(
        [Description("Path to a .w3x/.w3m map file or map folder.")] string map,
        [Description("Where to save the decompiled map.")] string out_path,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run(() =>
        {
            var full = ResolveOutPath(map, out_path);
            var doc = LoadMap(map);
            var r = LuaDecompileCommand.Decompile(doc);
            var w = r.Files.Count > 0 ? SaveDocAt(doc, full, map, overwrite, expect_input_sha256) : null;
            return new { r.Files, r.Failed, SavedTo = w?.SavedTo, InputSha256 = w?.InputSha256,
                OutputSha256 = w?.OutputSha256 ?? "", FilesRewritten = w?.FilesRewritten ?? 0,
                BytesWritten = w?.BytesWritten ?? 0 };
        });

    [McpServerTool(Name = "script_lua_leaks", Title = "Find Lua handle leaks", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Find handle leaks in a Lua map script. Lua collects the wrapper but the engine still owns the handle, so a creator dropped as a statement, a creator on a period that is never released, and a handle kept in a table the script never clears are the shapes that grow memory. Each finding is ranked hot, repeat or once. Read-only.")]
    public static LuaScriptLeaksResult ScriptLuaLeaks(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => LuaScriptLeaksCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "script_lua_globals", Title = "List Lua globals", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("List the globals a Lua map defines, where each is read and written, and any global that is written but never read. The Lua twin of script_unused_globals, parser based rather than a line scan. Read-only.")]
    public static LuaGlobalsResult ScriptLuaGlobals(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => LuaGlobalsCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "script_lua_check", Title = "Check Lua syntax", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Parse every Lua file in a map and report the syntax errors with their line and column, the way luac 5.3 refuses them. Wider than script_check, which parses only the entry the map runs. Read-only.")]
    public static LuaCheckResult ScriptLuaCheck(
        [Description("Path to a .w3x/.w3m map file or map folder.")] string map)
        => Run(() => LuaCheckCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "script_lua_api", Title = "Check Lua API usage", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("List the natives and functions a Lua script calls that a target game build does not declare. Uses the same schema membership test as the compat check, and reads the calls from the parsed tree. Read-only.")]
    public static LuaApiResult ScriptLuaApi(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Target build name or schema folder path.")] string target,
        [Description("Directory of schema folders with manifest.json. Optional, the default search is used when omitted.")] string? schemas_dir = null)
        => Run(() => LuaApiCommand.Execute(LoadMap(map), target, schemas_dir));

    [McpServerTool(Name = "script_lua_to_jass", Title = "Convert Lua back to JASS", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Best effort conversion of a Lua map back to JASS, saved to out_path. The JASS to Lua conversion is not reversible in general, so only the constructs that map back without guessing are converted and every construct left is reported. Nothing is written when the script does not convert. The input map is NEVER modified.")]
    public static object ScriptLuaToJass(
        [Description("Path to a .w3x/.w3m map file or map folder.")] string map,
        [Description("Where to save the converted map.")] string out_path,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run(() =>
        {
            var full = ResolveOutPath(map, out_path);
            var doc = LoadMap(map);
            var r = LuaToJassCommand.Execute(doc);
            if (!r.Ok) throw Rejected(string.Join(" ", r.Diagnostics));
            var w = SaveDocAt(doc, full, map, overwrite, expect_input_sha256);
            return new { r.ScriptFile, r.FunctionsConverted, r.FunctionsSkipped, r.GlobalsConverted, r.Unconverted, r.Diagnostics,
                SavedTo = w.SavedTo, w.InputSha256, w.OutputSha256, w.FilesRewritten, w.BytesWritten };
        });

    [McpServerTool(Name = "script_conversion_notes", Title = "JASS to Lua conversion rules", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("The rules the JASS to Lua converter applies and where it diverges, one row per construct, with the JASS form, the Lua it emits and whether that runs the same. Read it before editing a converted script or trusting a conversion, since two rules are approximated, real arithmetic precision and hashtables, and the first is fixed by passing strict_reals to script_to_lua. No map needed. Read-only.")]
    public static ConversionNotesResult ScriptConversionNotes() => ConversionNotes.Rules();

    [McpServerTool(Name = "script_reachable", Title = "List reachable functions", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("List the functions reachable from the map script's entry points, main and the InitTrig functions, and the dead functions nothing reaches, each with its declaration line. A function reached only through a string (ExecuteFunc) is reported dead here, which is the honest answer to what main reaches. Read-only.")]
    public static ScriptReachableResult ScriptReachable(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => ScriptReachableCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "script_api_usage", Title = "Find undeclared natives", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("List the natives and functions the map script uses that a target game build does not declare, the compile-time side of a port. Uses the same schema membership test as the compat check. Read-only.")]
    public static ScriptApiUsageResult ScriptApiUsage(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Target build name or schema folder path.")] string target,
        [Description("Directory of schema folders with manifest.json. Optional, the default search is used when omitted.")] string? schemas_dir = null)
        => Run(() => ScriptApiUsageCommand.Execute(LoadMap(map), target, schemas_dir));

    [McpServerTool(Name = "script_semantic_diff", Title = "Diff scripts semantically", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Compare two map scripts line by line after normalizing whitespace, dropping comments and ignoring pure renames of local variables, and report the real changes. Use this rather than diff for a before/after of a port or an edit where a variable rename or a reindent should not drown the intended change. Read-only.")]
    public static ScriptSemanticDiffResult ScriptSemanticDiff(
        [Description("Path to the first .w3x/.w3m map, or to an unpacked map folder.")] string map,
        [Description("Path to the second .w3x/.w3m map.")] string other)
        => Run(() => ScriptSemanticDiffCommand.Execute(LoadMap(map), LoadMap(other)));

    [McpServerTool(Name = "script_duplicate_bodies", Title = "Find duplicate functions", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Find functions whose normalized bodies are identical, up to comments, whitespace and local variable names. A copied function is a maintenance trap, a fix in one copy silently misses the others. Read-only.")]
    public static ScriptDuplicateBodiesResult ScriptDuplicateBodies(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => ScriptDuplicateBodiesCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "script_modernize", Title = "Modernize deprecated natives", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Rewrite deprecated or renamed natives in the map script to the target build's spelling where a known mapping exists, and save the map to out_path. A rewrite is only applied when the target build declares the new name, and names inside strings or comments are never touched. Nothing is written when there is nothing to rewrite. The input map is NEVER modified.")]
    public static object ScriptModernize(
        [Description("Path to a .w3x/.w3m map file or map folder.")] string map,
        [Description("Where to save the rewritten map.")] string out_path,
        [Description("Target build name or schema folder path.")] string target,
        [Description("Directory of schema folders with manifest.json. Optional, the default search is used when omitted.")] string? schemas_dir = null,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run(() =>
        {
            var full = ResolveOutPath(map, out_path);
            var doc = LoadMap(map);
            var r = ScriptModernizeCommand.Execute(doc, target, schemas_dir);
            var w = r.Rewrites > 0 ? SaveDocAt(doc, full, map, overwrite, expect_input_sha256) : null;
            return new { r.ScriptFile, r.TargetBuild, r.Rewrites, r.Changes, SavedTo = w?.SavedTo,
                InputSha256 = w?.InputSha256, OutputSha256 = w?.OutputSha256 ?? "",
                FilesRewritten = w?.FilesRewritten ?? 0, BytesWritten = w?.BytesWritten ?? 0 };
        });

    [McpServerTool(Name = "terrain_info", Title = "Terrain grid info", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("The terrain grid's extents in corners, plus the map's ground and cliff tile lists. Texture fields on a corner are indexes into those lists, so read this first to know what an index means.")]
    public static TerrainEditCommand.TerrainInfo TerrainInfoTool(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => TerrainEditCommand.GetInfo(LoadMap(map))
            ?? throw Wc3Errors.Refuse(Wc3ErrorCodes.ObjectNotFound, "this map has no war3map.w3e terrain file", "war3map.w3e"));

    [McpServerTool(Name = "terrain_corner_get", Title = "Get one terrain corner", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Show one terrain corner's full state: ground height, cliff level, water, ground and cliff texture indexes with variations, and the ramp, blight, boundary and edge flags.")]
    public static TerrainEditCommand.CornerInfo TerrainCornerGet(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Corner column, 0-based. Bounds come from terrain_info.")] int col,
        [Description("Corner row, 0-based.")] int row)
        => Run(() => TerrainEditCommand.GetCorner(LoadMap(map), col, row)
            ?? throw Wc3Errors.Refuse(Wc3ErrorCodes.ObjectNotFound, $"no terrain corner at ({col}, {row})", $"{col},{row}"));

    [McpServerTool(Name = "terrain_corner_set", Title = "Set one terrain corner", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Set one field on ONE terrain corner and save the edited copy to out_path. The input map is NEVER modified in place. For area work prefer terrain_deform, terrain_paint and the other brush tools.")]
    public static EditToolResult TerrainCornerSet(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Corner column, 0-based.")] int col,
        [Description("Corner row, 0-based.")] int row,
        [Description("Field: " + TerrainCornerFieldNames + ".")] string field,
        [Description("New value.")] string value,
        [Description("Where to write the edited map. Defaults to a sibling '.edited' copy.")] string? out_path = null,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() =>
        {
            var doc = LoadMap(map);
            var r = TerrainCornerFields.SetField(doc, col, row, field, value);
            return SaveEdit(doc, map, out_path, overwrite, expect_input_sha256, r.Ok, r.Message);
        });

    [McpServerTool(Name = "placed_units_list", Title = "List placed units", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("List every unit already PLACED on the map (war3mapUnits.doo), each with the creation number needed to edit or remove it, plus its type rawcode, owner, position and hero stats. Start locations appear here too. Call this before placed_unit_set or placed_unit_remove. Pass brief true to get the total and only the first 50 units when the full list would be too large for the context.")]
    public static PagedUnits PlacedUnitsList(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Return the total unit count and only the first 50 units instead of the full list. Default false.")] bool brief = false)
        => Run(() =>
        {
            if (!brief)
            {
                var all = UnitInstanceCommand.List(LoadMap(map));
                return new PagedUnits(all.Count, all.Count, false, null, all, false, all.Count);
            }
            var page = UnitInstanceCommand.ListBrief(LoadMap(map));
            return new PagedUnits(page.Units.Count, page.Total, page.Truncated,
                page.Truncated ? page.Units.Count : null, page.Units, page.Truncated, page.Total);
        });

    [McpServerTool(Name = "placed_unit_get", Title = "Get a placed unit", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Show one placed unit's full state by creation number: type, owner, position, facing, scale, hero level and attributes, hit points and mana percentages, gold and target acquisition range.")]
    public static UnitInstanceInfo PlacedUnitGet(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("The unit's creation number, from placed_units_list.")] int creation_number)
        => Run(() => UnitInstanceCommand.Get(LoadMap(map), creation_number)
            ?? throw Wc3Errors.Refuse(Wc3ErrorCodes.ObjectNotFound, $"no placed unit with creation number {creation_number}", creation_number.ToString()));

    [McpServerTool(Name = "placed_unit_set", Title = "Edit a placed unit", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Set one field on a unit already placed on the map and save the edited copy to out_path. The input map is NEVER modified in place. A position field takes 'x,y', a scale field takes one number (uniform) or 'sx,sy,sz', and Facing is in RADIANS (pi/2 faces north), the unit the map file stores.")]
    public static EditToolResult PlacedUnitSet(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("The unit's creation number, from placed_units_list.")] int creation_number,
        [Description("Field: " + UnitInstanceFields + ".")] string field,
        [Description("New value.")] string value,
        [Description("Where to write the edited map. Defaults to a sibling '.edited' copy.")] string? out_path = null,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() =>
        {
            var doc = LoadMap(map);
            var r = PlacedInstanceFields.SetUnitField(doc, creation_number, field, value);
            return SaveEdit(doc, map, out_path, overwrite, expect_input_sha256, r.Ok, r.Message);
        });

    [McpServerTool(Name = "placed_unit_remove", Title = "Remove a placed unit", ReadOnly = false, Destructive = true, Idempotent = true, OpenWorld = false)]
    [Description("Remove a unit already placed on the map and save the edited copy to out_path. The input map is NEVER modified in place.")]
    public static EditToolResult PlacedUnitRemove(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("The unit's creation number, from placed_units_list.")] int creation_number,
        [Description("Where to write the edited map. Defaults to a sibling '.edited' copy.")] string? out_path = null,
        [Description("State the intent to destroy data. Without it the call is refused with CONFIRM_REQUIRED, naming what would be removed.")] bool confirm = false,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => RunConfirmed(confirm, "removes placed unit " + creation_number + "", () =>
        {
            var doc = LoadMap(map);
            var r = UnitInstanceCommand.Delete(doc, creation_number);
            return SaveEdit(doc, map, out_path, overwrite, expect_input_sha256, r.Ok, r.Message);
        });

    [McpServerTool(Name = "placed_doodads_list", Title = "List placed doodads", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("List every doodad and destructable already PLACED on the map (war3map.doo), each with the creation number needed to edit or remove it, plus its type rawcode, position, rotation, scale and variation.")]
    public static CappedPage<DoodadInstanceInfo> PlacedDoodadsList(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => Page(Wc3Limit.Cap(DoodadInstanceCommand.List(LoadMap(map)))));

    [McpServerTool(Name = "placed_doodad_get", Title = "Get a placed doodad", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Show one placed doodad's full state by creation number: type, position, rotation, scale, variation and life percentage.")]
    public static DoodadInstanceInfo PlacedDoodadGet(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("The doodad's creation number, from placed_doodads_list.")] int creation_number)
        => Run(() => DoodadInstanceCommand.Get(LoadMap(map), creation_number)
            ?? throw Wc3Errors.Refuse(Wc3ErrorCodes.ObjectNotFound, $"no placed doodad with creation number {creation_number}", creation_number.ToString()));

    [McpServerTool(Name = "placed_doodad_set", Title = "Edit a placed doodad", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Set one field on a doodad already placed on the map and save the edited copy to out_path. The input map is NEVER modified in place. A position field takes 'x,y' or 'x,y,z'.")]
    public static EditToolResult PlacedDoodadSet(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("The doodad's creation number, from placed_doodads_list.")] int creation_number,
        [Description("Field: " + DoodadInstanceFields + ".")] string field,
        [Description("New value.")] string value,
        [Description("Where to write the edited map. Defaults to a sibling '.edited' copy.")] string? out_path = null,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() =>
        {
            var doc = LoadMap(map);
            var r = PlacedInstanceFields.SetDoodadField(doc, creation_number, field, value);
            return SaveEdit(doc, map, out_path, overwrite, expect_input_sha256, r.Ok, r.Message);
        });

    [McpServerTool(Name = "placed_doodad_remove", Title = "Remove a placed doodad", ReadOnly = false, Destructive = true, Idempotent = true, OpenWorld = false)]
    [Description("Remove a doodad already placed on the map and save the edited copy to out_path. The input map is NEVER modified in place.")]
    public static EditToolResult PlacedDoodadRemove(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("The doodad's creation number, from placed_doodads_list.")] int creation_number,
        [Description("Where to write the edited map. Defaults to a sibling '.edited' copy.")] string? out_path = null,
        [Description("State the intent to destroy data. Without it the call is refused with CONFIRM_REQUIRED, naming what would be removed.")] bool confirm = false,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => RunConfirmed(confirm, "removes placed doodad " + creation_number + "", () =>
        {
            var doc = LoadMap(map);
            var r = DoodadInstanceCommand.Delete(doc, creation_number);
            return SaveEdit(doc, map, out_path, overwrite, expect_input_sha256, r.Ok, r.Message);
        });

    [McpServerTool(Name = "place_grid", Title = "Place a grid of units", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Place a regular rows x cols grid of copies of one unit or doodad type and save the edited copy to out_path. The grid is centred on the map origin. The input map is NEVER modified in place.")]
    public static BulkPlacementToolResult PlaceGrid(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Four-character unit or doodad type rawcode.")] string type_rawcode,
        [Description("Number of grid rows (>= 1).")] int rows,
        [Description("Number of grid columns (>= 1).")] int cols,
        [Description("Spacing between adjacent cells in world units. Default 128 (one tile).")] float spacing = PlaceGridCommand.DefaultSpacing,
        [Description("Owning player id for a unit grid (0-based). Ignored for doodads.")] int owner_id = 0,
        [Description("Placement kind, unit or doodad.")] string kind = "unit",
        [Description("Where to write the edited map. Defaults to a sibling '.edited' copy.")] string? out_path = null,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run(() =>
        {
            var doc = LoadMap(map);
            var r = PlaceGridCommand.Execute(doc, type_rawcode, rows, cols, spacing, owner_id, kind);
            if (!r.Ok) throw Rejected(r.Message);
            string full = ResolveOutPath(map, out_path);
            var w = SaveDocAt(doc, full, map, overwrite, expect_input_sha256);
            return new BulkPlacementToolResult(w.SavedTo, r.Message, r.Placed, r.CreationNumbers,
                w.InputSha256, w.OutputSha256, w.FilesRewritten, w.BytesWritten);
        });

    [McpServerTool(Name = "place_scatter", Title = "Scatter units randomly", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Place N copies of a unit type at deterministic random positions and save the edited copy to out_path. Positions are uniform inside a named region or across the whole playable area, and the same seed reproduces the same layout. The input map is NEVER modified in place.")]
    public static BulkPlacementToolResult PlaceScatter(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Four-character unit type rawcode.")] string type_rawcode,
        [Description("Number of copies to place (>= 1).")] int count,
        [Description("Region name to scatter inside. Omit to scatter across the whole playable map.")] string? region = null,
        [Description("Deterministic random seed. Default 0.")] int seed = PlaceScatterCommand.DefaultSeed,
        [Description("Where to write the edited map. Defaults to a sibling '.edited' copy.")] string? out_path = null,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run(() =>
        {
            var doc = LoadMap(map);
            var r = PlaceScatterCommand.Execute(doc, type_rawcode, count, region, seed);
            if (!r.Ok) throw Rejected(r.Message);
            string full = ResolveOutPath(map, out_path);
            var w = SaveDocAt(doc, full, map, overwrite, expect_input_sha256);
            return new BulkPlacementToolResult(w.SavedTo, r.Message, r.Placed, r.CreationNumbers,
                w.InputSha256, w.OutputSha256, w.FilesRewritten, w.BytesWritten);
        });

    [McpServerTool(Name = "placed_stats", Title = "Placed object counts", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Count the units and doodads already placed on the map, grouped by rawcode and by kind, with each type's resolved name where one is available. Read-only.")]
    public static PlacedStatsResult PlacedStats(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Without it only the map's own object-data names resolve.")] string? game_dir = null)
        => Run(() => PlacedStatsCommand.Execute(LoadMap(map), ResolveGameDir(game_dir)));

    [McpServerTool(Name = "placed_missing_refs", Title = "Find placements with no type", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("List placements whose rawcode is defined neither in the map's object data nor in the base game data. Without a game_dir only the map's object data can be consulted, so stock base-game types read as unresolved too and the message says so. Read-only.")]
    public static PlacedMissingRefsResult PlacedMissingRefs(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Without it the base-game check is skipped.")] string? game_dir = null)
        => Run(() => PlacedMissingRefsCommand.Execute(LoadMap(map), ResolveGameDir(game_dir)));

    [McpServerTool(Name = "placed_fix_refs", Title = "Drop or fix broken placements", ReadOnly = false, Destructive = true, Idempotent = false, OpenWorld = false)]
    [Description("Drop (or, with to_rawcode, rewrite) placements that reference a missing rawcode and save the edited copy to out_path, reporting each change. Refuses when nothing is wrong. The input map is NEVER modified in place.")]
    public static object PlacedFixRefs(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Where to write the edited map. Defaults to a sibling '.edited' copy.")] string? out_path = null,
        [Description("Rewrite each missing placement's rawcode to this four-character code instead of dropping it.")] string? to_rawcode = null,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Without it the base-game check is skipped.")] string? game_dir = null,
        [Description("State the intent to destroy data. Without it the call is refused with CONFIRM_REQUIRED, naming what would be removed.")] bool confirm = false,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => RunConfirmed(confirm, "rewrites or drops every placement whose type is missing", () =>
        {
            var doc = LoadMap(map);
            var r = PlacedFixRefsCommand.Execute(doc, ResolveGameDir(game_dir), to_rawcode);
            if (!r.Ok) throw Rejected(r.Message);
            string full = ResolveOutPath(map, out_path);
            return Gated(r, SaveDocAt(doc, full, map, overwrite, expect_input_sha256));
        });

    [McpServerTool(Name = "triggers_read", Title = "Read the trigger tree", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Read the map's GUI trigger tree (war3map.wtg) plus the custom-text bodies (war3map.wct) and the script language: every category, every trigger with its event/condition/action tree, and every GUI variable. Read-only, because this format cannot yet be written back. A map with no GUI triggers returns empty lists rather than failing.")]
    public static TriggerModel TriggersRead(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => TriggerReadCommand.GetTriggers(LoadMap(map)));

    [McpServerTool(Name = "trigger_to_script", Title = "Render triggers as script", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Render the map's GUI trigger tree (war3map.wtg) to JASS-style text, the way the World Editor's 'convert to custom text' presents one trigger. Read-only, it never edits the map. Preset parameter values are resolved to their script form through the World-Editor catalog when the installed game provides one, and otherwise keep their raw names. This is a rendering for reading rather than a replacement for the compiled script (war3map.j), so it is not guaranteed to compile.")]
    public static TriggerScriptResult TriggerToScript(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Used only to resolve preset values, and its absence is not an error.")] string? game_dir = null)
        => Run(() => TriggerToScriptCommand.Render(
            LoadMap(map), TriggerCatalogCommand.TryLoad(null, ResolveGameDir(game_dir), out _)));

    [McpServerTool(Name = "trigger_diff", Title = "Diff two trigger trees", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Compare two maps' GUI trigger trees and report triggers added, removed and renamed, plus event, condition or action changes within a trigger shared by both maps. Triggers are matched first by a unique id (so a rename reads as a rename) and then by name. Read-only.")]
    public static TriggerDiffResult TriggerDiff(
        [Description("Path to the first .w3x/.w3m map file, or to an unpacked map folder.")] string map_a,
        [Description("Path to the second .w3x/.w3m map file.")] string map_b)
        => Run(() => TriggerDiffCommand.Compare(LoadMap(map_a), LoadMap(map_b)));

    [McpServerTool(Name = "trigger_lint", Title = "Lint the trigger tree", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Check a map's GUI trigger tree for three structural faults, a trigger that can never fire (no event and not run on map initialization), a trigger that registers the same event twice, and a trigger with no action. Disabled and custom-text triggers are skipped and the counts say so. Read-only.")]
    public static TriggerLintResult TriggerLint(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => TriggerLintCommand.Lint(LoadMap(map)));

    [McpServerTool(Name = "trigger_export", Title = "Export the trigger tree", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Export a map's full GUI trigger tree as structured output. Each trigger carries its name, its enabled, initially-on and run-on-map-init flags, and its event, condition and action lists with the catalog function name, enabled flag, arguments and nested blocks for every node. Read-only. A map with no trigger tree returns empty lists.")]
    public static TriggerExportResult TriggerExport(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => TriggerExportCommand.Export(LoadMap(map)));

    [McpServerTool(Name = "trigger_dead", Title = "Find dead triggers", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Find triggers that cannot do anything, listing each reason. Triggers with no action, and triggers whose events never fire, which is an event depending on a variable nothing sets (a unit event holding an unassigned unit variable, or a variable event watching a variable that never changes). A variable counts as set when the tree initialises it or, when the compiled script is present, when the script assigns it. Read-only.")]
    public static TriggerDeadResult TriggerDead(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => TriggerDeadCommand.Dead(LoadMap(map)));

    [McpServerTool(Name = "hero_roster", Title = "List the map's heroes", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("List the playable heroes a map registers, each with its rawcode, display name, the string tags from its registration call (typically a role and a portrait path), and which trigger's custom-text body holds its code. A hero is not a trigger, and counting one does not count the other, a measured map held 170 registered heroes across 39 Character triggers because a few grouped triggers bundle many heroes. Use this rather than counting triggers when asked how many heroes a map has. A map that creates heroes inline returns Ok=false.")]
    public static HeroRosterCommand.RosterResult HeroRoster(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => HeroRosterCommand.Run(LoadMap(map)));

    [McpServerTool(Name = "strings_list", Title = "List the string table", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("List the map's string table (war3map.wts). Every TRIGSTR_ reference in object data and in scripts resolves through here, so this is how to read what a tooltip or a unit name actually says.")]
    public static StringsListResult StringsList(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => StringsCommand.List(LoadMap(map)));

    [McpServerTool(Name = "strings_missing", Title = "Find missing strings", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("List the TRIGSTR_ keys the map script and the GUI trigger tree reference that have no war3map.wts entry behind them. A missing key shows the literal token TRIGSTR_nnn in game where a name or a message was meant. Read-only.")]
    public static StringsMissingResult StringsMissing(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => StringsMissingCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "strings_unused", Title = "Find unused strings", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("List the war3map.wts entries no object-data field, map-info field, GUI trigger or script line references, each with its text. Read-only. An unreferenced entry is dead weight rather than a fault, so treat the list as a lead to review and not an instruction to delete.")]
    public static StringsUnusedResult StringsUnused(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => StringsUnusedCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "strings_diff", Title = "Diff string tables", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Compare two maps' string tables (war3map.wts) and report entries added, removed and changed. Entries are matched by id, so a text change on the same key reads as a change rather than a remove plus an add. Read-only.")]
    public static StringsDiffResult StringsDiff(
        [Description("Path to the first .w3x/.w3m map file, or to an unpacked map folder.")] string map_a,
        [Description("Path to the second .w3x/.w3m map file.")] string map_b)
        => Run(() => StringsDiffCommand.Execute(LoadMap(map_a), LoadMap(map_b)));

    [McpServerTool(Name = "strings_find", Title = "Search string text", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Find war3map.wts entries whose text contains the given text, case-insensitive, each with its id. Read-only. An empty query matches every entry.")]
    public static StringsFindResult StringsFind(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Text to find in entry text, a case-insensitive substring.")] string text)
        => Run(() => StringsFindCommand.Execute(LoadMap(map), text));

    [McpServerTool(Name = "strings_set", Title = "Set a string entry", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Set one war3map.wts entry's text, or add the entry when the id is absent, and save the edited map to out_path. The file's own encoding and line endings are preserved and the input map is NEVER modified. out_path defaults to a sibling '<map>.edited.<ext>' and must differ from the input.")]
    public static object StringsSet(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only. The edited copy is written to out_path.")] string map,
        [Description("war3map.wts entry id to set or add, e.g. 4084.")] int id,
        [Description("New text for the entry. Empty text clears the entry to an empty block.")] string text,
        [Description("Output map path. Defaults to a sibling '<map>.edited.<ext>'. Must differ from the input.")] string? out_path = null,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run<object>(() =>
        {
            string dest = MpqStripBookkeepingCommand.ResolveOutput(RequireMapPath(map), out_path);
            StringsSetResult? r = null;
            var w = WriteGate.WriteFile(dest, Inputs(map), overwrite, expect_input_sha256,
                temp => r = StringsSetCommand.Execute(map, id, text, temp));
            return Gated(r!, w);
        });

    [McpServerTool(Name = "imports_list", Title = "Audit the import table", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("List the map's import table (war3map.imp) against what the archive actually holds, so a table entry with no file behind it and a file absent from the table both show up.")]
    public static ImportsListResult ImportsList(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => ImportsCommand.Execute(LoadMap(map)));

    [McpServerTool(Name = "object_set", Title = "Set an object field", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Set a field on an object of one Object Editor kind and save the edited map to out_path (only that kind's war3map.* file is re-serialized). The input map is NEVER modified in place. Field syntax: a bare 4-char field code, or 'code:N' to select level N (ability/upgrade) or variation N (doodad). Returns both SHA-256 digests and the count of files rewritten, refuses an existing output unless overwrite is true, and refuses a stale expect_input_sha256.")]
    public static ObjectSetToolResult ObjectSet(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Four-character object rawcode, e.g. 'hfoo' or 'u000'.")] string rawcode,
        [Description("Field to set: a 4-char field code, optionally ':N' for level/variation.")] string field,
        [Description("New value (parsed to the field's type: int/real/bool/string).")] string value,
        [Description("Object kind (selects which war3map.* file is edited): " + KindValues + ".")] string kind = "unit",
        [Description("Replace out_path when it already exists. Default false, which refuses rather than silently overwriting a previous output.")] bool overwrite = false,
        [Description("The SHA-256 the caller believes the input map has, from a previous result. A mismatch refuses the call as STALE_INPUT, which stops two edits from one base losing the first.")] string? expect_input_sha256 = null)
        => Run(() =>
        {
            var doc = LoadMap(map);
            var r = ObjectSetCommand.Execute(doc, ParseKind(kind), rawcode, field, value);
            if (!r.Ok) throw Rejected(r.Message);
            var w = WriteGate.Save(doc, map, out_path, overwrite, expect_input_sha256, LoadedShaOf(doc));
            return new ObjectSetToolResult(w.SavedTo, w.InputSha256, w.OutputSha256, w.FilesRewritten,
                w.BytesWritten, r.Message, r.Warning);
        });

    [McpServerTool(Name = "object_batch_set", Title = "Set several object fields", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Apply several object edits in one load and save once to out_path. The input map is NEVER modified in place. Edits are all or nothing, so the first failure stops the batch and nothing is written. Use this instead of two object_set calls from one base, which loses the first edit. Returns both digests, the rewritten count and the number applied.")]
    public static ObjectBatchSetToolResult ObjectBatchSet(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("The edits, applied in order. At most 500. A later edit to the same field wins.")] BatchEditInput[] edits,
        [Description("Replace out_path when it already exists. Default false, which refuses rather than silently overwriting a previous output.")] bool overwrite = false,
        [Description("The SHA-256 the caller believes the input map has, from a previous result. A mismatch refuses the call as STALE_INPUT.")] string? expect_input_sha256 = null)
        => Run(() =>
        {
            if (edits is null || edits.Length == 0)
                throw Invalid("at least one edit is required", "edits");
            if (edits.Length > ObjectBatchSetCommand.MaxEdits)
                throw Invalid($"at most {ObjectBatchSetCommand.MaxEdits} edits are allowed in one call, got {edits.Length}", "edits");

            var parsed = new List<ObjectBatchEdit>(edits.Length);
            for (int i = 0; i < edits.Length; i++)
            {
                var e = edits[i];
                if (string.IsNullOrWhiteSpace(e.Rawcode) || string.IsNullOrWhiteSpace(e.Field))
                    throw Invalid($"edit {i + 1}: rawcode and field are required", "rawcode");
                parsed.Add(new ObjectBatchEdit(ParseKind(e.Kind), e.Rawcode, e.Field, e.Value ?? ""));
            }

            var doc = LoadMap(map);
            var r = ObjectBatchSetCommand.Execute(doc, parsed);
            if (!r.Ok)
            {
                string detail = r.Failed.Count == 0 ? "" : $" ({r.Failed[0].Message})";
                throw Rejected(r.Message + detail);
            }

            var w = WriteGate.Save(doc, map, out_path, overwrite, expect_input_sha256, LoadedShaOf(doc));
            return new ObjectBatchSetToolResult(w.SavedTo, w.InputSha256, w.OutputSha256, w.FilesRewritten,
                w.BytesWritten, r.Applied, r.Message, r.Warnings.ToArray());
        });

    [McpServerTool(Name = "object_new", Title = "Create a custom object", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Create a new custom object derived from a base rawcode (a fresh unused rawcode is allocated) and save the edited map to out_path. The input map is NEVER modified in place. Returns the new rawcode.")]
    public static ObjectNewToolResult ObjectNew(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Four-character base object rawcode to derive from, e.g. 'hfoo'.")] string base_rawcode,
        [Description("Object kind: " + KindValues + ".")] string kind = "unit",
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run(() =>
        {
            string full = ResolveOutPath(map, out_path);
            var doc = LoadMap(map);
            var r = ObjectNewCommand.Execute(doc, ParseKind(kind), base_rawcode);
            if (!r.Ok) throw Rejected(r.Message);
            var w = SaveDocAt(doc, full, map, overwrite, expect_input_sha256);
            return new ObjectNewToolResult(w.SavedTo, r.Message, r.NewRawcode,
                w.InputSha256, w.OutputSha256, w.FilesRewritten, w.BytesWritten);
        });

    [McpServerTool(Name = "object_search", Title = "Search object fields", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Full-text search across every custom or modified object's field values, in both the base game data and the map's own deltas. Read-only. Reports the kind, rawcode, resolved name, field code, and the base and map values, so a hit is traceable. Pass kind to search one Object Editor type or field to restrict to one field code.")]
    public static ObjectSearchResult ObjectSearch(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Text to find in field values, a case-insensitive substring.")] string query,
        [Description("Object kind: " + KindValues + ". Omit to search every kind.")] string? kind = null,
        [Description("Restrict the search to one field code, e.g. 'uhpm'.")] string? field = null,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Without it only the map's own deltas are searched.")] string? game_dir = null)
        => Run(() => ObjectSearchCommand.Execute(
            LoadMap(map), query, kind is null ? null : ParseKind(kind), field, ResolveGameDir(game_dir)));

    [McpServerTool(Name = "object_noop_fields", Title = "Find no-op object fields", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("List object fields whose map value equals the base game value, so the edit changes nothing. Read-only. Needs a game_dir for the base values; without one the answer is map-only and says so.")]
    public static ObjectNoopFieldsResult ObjectNoopFields(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Object kind: " + KindValues + ". Omit to check every kind.")] string? kind = null,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Without it there is no base value to compare against.")] string? game_dir = null)
        => Run(() => ObjectNoopFieldsCommand.Execute(
            LoadMap(map), kind is null ? null : ParseKind(kind), ResolveGameDir(game_dir)));

    [McpServerTool(Name = "object_deprecated", Title = "Find deprecated fields", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("List object fields the target build's schema does not define but the map still sets, meaning the edit is ignored on that build. Read-only. The target is a schema folder path or a build name under the schemas directory. A target with no object data section is reported as such rather than as a clean result.")]
    public static ObjectDeprecatedResult ObjectDeprecated(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Target build name or path to a schema folder.")] string target,
        [Description("Optional path to a schemas directory. When omitted, searches <AppContext.BaseDirectory>/schemas then walks up to data/schemas.")] string? schemas_dir = null)
        => Run(() => ObjectDeprecatedCommand.Execute(LoadMap(map), target, schemas_dir));

    [McpServerTool(Name = "object_fix_model_ext", Title = "Repair model path extensions", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Repair object-data model paths in one pass and save to out_path. A .mdl is rewritten to the .mdx that exists, a doubled war3mapImported prefix is collapsed, and any path still unresolved is redirected to redirect_missing when that file resolves. Base-game roots are left alone. The input map is NEVER modified in place.")]
    public static object ObjectFixModelExt(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Internal path that unresolved model references are pointed at, for example war3mapImported/dummy.mdx. Omit to leave unresolved references untouched.")] string? redirect_missing = null,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run<object>(() =>
        {
            string full = ResolveOutPath(map, out_path);
            var doc = LoadMap(map);
            var r = ModelExtFixCommand.Execute(doc, apply: true, redirectMissing: redirect_missing);
            return Gated(r, SaveDocAt(doc, full, map, overwrite, expect_input_sha256));
        });

    [McpServerTool(Name = "object_rawcode_suggest", Title = "Suggest free rawcodes", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Propose free four-character rawcodes for one Object Editor kind that collide with neither the map's objects nor the base game data. Read-only. Without a game_dir the suggestions are checked against the map only.")]
    public static ObjectRawcodeSuggestResult ObjectRawcodeSuggest(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Object kind: " + KindValues + ".")] string kind,
        [Description("How many rawcodes to propose. Default 10.")] int count = 10,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Without it base game data is not excluded.")] string? game_dir = null)
        => Run(() => ObjectRawcodeSuggestCommand.Execute(
            LoadMap(map), ParseKind(kind), count, ResolveGameDir(game_dir)));

    [McpServerTool(Name = "object_clone", Title = "Clone an object", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Deep-clone a map object under a new rawcode and save the edited map to out_path. The input map is NEVER modified in place. The clone copies the source's field deltas and, for a leveled kind, its per-level data too, keeping the source's base. Refuses a rawcode already taken by the map.")]
    public static ObjectCloneToolResult ObjectClone(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Four-character source rawcode to clone.")] string rawcode,
        [Description("Four-character rawcode for the clone.")] string new_rawcode,
        [Description("Object kind: " + KindValues + ".")] string kind = "unit",
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run(() =>
        {
            string full = ResolveOutPath(map, out_path);
            var doc = LoadMap(map);
            var r = ObjectCloneCommand.Execute(doc, ParseKind(kind), rawcode, new_rawcode);
            if (!r.Ok) throw Rejected(r.Message);
            var w = SaveDocAt(doc, full, map, overwrite, expect_input_sha256);
            return new ObjectCloneToolResult(w.SavedTo, r.Message, r.NewRawcode, r.FieldsCopied,
                w.InputSha256, w.OutputSha256, w.FilesRewritten, w.BytesWritten);
        });

    [McpServerTool(Name = "object_migrate", Title = "Migrate objects to a build", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Rewrite a map's object data to a target build's field set, dropping every field the target does not define, and save the edited copy to out_path. The input map is NEVER modified in place. Every drop is reported, and a target with no object data section refuses rather than stripping everything.")]
    public static ObjectMigrateToolResult ObjectMigrate(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Target build name or path to a schema folder, read the same way the compat command reads it.")] string to,
        [Description("Optional path to a schemas directory. When omitted, searches <AppContext.BaseDirectory>/schemas then walks up to data/schemas.")] string? schemas_dir = null,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run(() =>
        {
            string full = ResolveOutPath(map, out_path);
            var doc = LoadMap(map);
            var r = ObjectMigrateCommand.Execute(doc, to, schemas_dir);
            if (!r.Ok) throw Rejected(r.Message);
            var w = SaveDocAt(doc, full, map, overwrite, expect_input_sha256);
            return new ObjectMigrateToolResult(
                w.SavedTo, r.Message, r.TargetBuild, r.FieldsKept, r.FieldsDropped, r.Drops.Count,
                w.InputSha256, w.OutputSha256, w.FilesRewritten, w.BytesWritten);
        });

    [McpServerTool(Name = "object_bulk_set", Title = "Set a field on many objects", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Set one field on every object of a kind that matches every where clause, and save the edited copy to out_path. The input map is NEVER modified in place. A where clause is code=value and tests the object's merged value (base game data overlaid by the map). Reports how many objects changed.")]
    public static ObjectBulkSetToolResult ObjectBulkSet(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("The field and value to write, as code=value (e.g. uhpm=1000).")] string set,
        [Description("Object kind: " + KindValues + ".")] string kind = "unit",
        [Description("Zero or more match clauses code=value. An object must match every clause to be written. At most 1000, and a longer list is refused with a tool error naming the limit.")] string[]? where = null,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Used to test where clauses against base game values.")] string? game_dir = null,
        [Description("Replace out_path when it already exists. Default false, which refuses rather than silently overwriting a previous output.")] bool overwrite = false,
        [Description("The SHA-256 the caller believes the input map has, from a previous result. A mismatch refuses the call as STALE_INPUT.")] string? expect_input_sha256 = null)
        => Run(() =>
        {
            RequireAtMost(where?.Length ?? 0, "where");
            if (!TrySplitPair(set, out var field, out var value))
                throw Invalid($"expected set as code=value, got '{set}'", set);
            var clauses = new List<BulkSetWhere>();
            foreach (var raw in where ?? Array.Empty<string>())
            {
                if (!TrySplitPair(raw, out var wc, out var wv))
                    throw Invalid($"expected a where clause as code=value, got '{raw}'", raw);
                clauses.Add(new BulkSetWhere(wc, wv));
            }
            var doc = LoadMap(map);
            var r = ObjectBulkSetCommand.Execute(doc, ParseKind(kind), field, value, clauses, ResolveGameDir(game_dir));
            if (!r.Ok) throw Rejected(r.Message);
            var w = WriteGate.Save(doc, map, out_path, overwrite, expect_input_sha256, LoadedShaOf(doc));
            var failed = r.Entries.Where(e => !e.Changed)
                .Select(e => new SkippedItem(e.Rawcode, e.Detail)).ToList();
            return new ObjectBulkSetToolResult(
                w.SavedTo, w.InputSha256, w.OutputSha256, w.FilesRewritten, w.BytesWritten,
                r.Message, r.Kind, r.Matched, r.Changed, r.Skipped, failed, failed.Count > 0);
        });

    [McpServerTool(Name = "object_export", Title = "Export objects to CSV", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Export the objects of one kind (base game data overlaid by the map's deltas) to a CSV file at out_path, one row per object, one column per field code, with a header. Read-only on the map. Feed the CSV back with object_import.")]
    public static ObjectExportToolResult ObjectExport(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("CSV output file path.")] string out_path,
        [Description("Object kind: " + KindValues + ".")] string kind = "unit",
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Without it the export carries the map's own deltas only.")] string? game_dir = null)
        => Run(() =>
        {
            var r = ObjectExportCommand.Execute(LoadMap(map), ParseKind(kind), ResolveGameDir(game_dir));
            string full = Path.GetFullPath(out_path);
            if (Path.GetDirectoryName(full) is { Length: > 0 } dir) Directory.CreateDirectory(dir);
            File.WriteAllText(full, r.Csv);
            return new ObjectExportToolResult(full, r.Message, r.Kind, r.ObjectCount, r.Columns.Count);
        });

    [McpServerTool(Name = "object_import", Title = "Import objects from CSV", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Import a CSV of one kind back into the map, adding objects the map does not define and updating those it does, then save the edited copy to out_path. The input map is NEVER modified in place. An unknown column refuses the whole import. A blank cell leaves that field untouched.")]
    public static ObjectImportToolResult ObjectImport(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Path to the CSV file to import.")] string csv_path,
        [Description("Object kind: " + KindValues + ".")] string kind = "unit",
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Used to verify a column is a field the kind defines.")] string? game_dir = null,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run(() =>
        {
            if (!File.Exists(csv_path)) throw Wc3Errors.Refuse(Wc3ErrorCodes.InputNotFound, $"no CSV file at {csv_path}", csv_path);
            string full = ResolveOutPath(map, out_path);
            var doc = LoadMap(map);
            var r = ObjectImportCommand.Execute(doc, ParseKind(kind), File.ReadAllText(csv_path), ResolveGameDir(game_dir));
            if (!r.Ok) throw Rejected(r.Message);
            var w = SaveDocAt(doc, full, map, overwrite, expect_input_sha256);
            return new ObjectImportToolResult(w.SavedTo, r.Message, r.Kind, r.Added, r.Updated, r.FieldsSet,
                w.InputSha256, w.OutputSha256, w.FilesRewritten, w.BytesWritten);
        });

    [McpServerTool(Name = "object_rename", Title = "Rename an object rawcode", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Rename an object rawcode and save the edited copy to out_path. The input map is NEVER modified in place. With rewrite_refs true, every object-data field value and script string literal that names the old rawcode is rewritten too, so references do not dangle. Refuses a new rawcode already taken by the map.")]
    public static ObjectRenameToolResult ObjectRename(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("The rawcode to rename.")] string old_rawcode,
        [Description("The new rawcode.")] string new_rawcode,
        [Description("Object kind: " + KindValues + ".")] string kind = "unit",
        [Description("Also rewrite every field value and script literal that references the old rawcode. Default false.")] bool rewrite_refs = false,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run(() =>
        {
            string full = ResolveOutPath(map, out_path);
            var doc = LoadMap(map);
            var r = ObjectRenameCommand.Execute(doc, ParseKind(kind), old_rawcode, new_rawcode, rewrite_refs);
            if (!r.Ok) throw Rejected(r.Message);
            var w = SaveDocAt(doc, full, map, overwrite, expect_input_sha256);
            return new ObjectRenameToolResult(
                w.SavedTo, r.Message, r.Kind, r.OldRawcode, r.NewRawcode, r.FieldsRewritten,
                r.ScriptLiteralsRewritten, w.InputSha256, w.OutputSha256, w.FilesRewritten, w.BytesWritten);
        });

    [McpServerTool(Name = "unit_abilities", Title = "List a unit's abilities", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Every ability a unit has, each tagged with where it comes from. ObjectNormal and ObjectHero are the unit type's own uabi and uhab lists. Spellbook is inside a spellbook the unit holds, where passives often hide. PlacedUnit is set on one placed unit, with its level. MorphForm comes from a form the unit turns into. Script is given by the script at runtime, found where a statement names both the unit type and the ability, and those entries are inferred and carry a line number. Use this rather than object_get when asking what a unit really has.")]
    public static UnitAbilitiesResult UnitAbilities(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Four-character unit type rawcode, e.g. 'H000'. Ignored when creation_number is given.")] string rawcode,
        [Description("Creation number of a placed unit, to include the abilities and levels set on that one unit.")] int? creation_number = null,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Used to resolve ability names.")] string? game_dir = null)
        => Run(() =>
        {
            var doc = LoadMap(map);
            return UnitAbilitiesCommand.Execute(doc, rawcode, ResolveGameDir(game_dir), creation_number);
        });

    [McpServerTool(Name = "bundle_unit", Title = "Resolve a unit's bundle", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Resolve everything a unit depends on - the porting preview: referenced objects (with custom-to-map flags), asset files, trigger strings, dependency edges and the JASS trigger-function closure.")]
    public static UnitBundle BundleUnit(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Four-character unit rawcode, e.g. 'u000'.")] string rawcode,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Used to tell base-game references from custom ones.")] string? game_dir = null)
        => Run(() => BundleCommand.ResolveUnit(LoadMap(map), rawcode, ResolveGameDir(game_dir)));

    /// <summary>PNGs up to this size also return inline as MCP image content.</summary>
    private const int InlineImageLimit = 1_000_000;

    [McpServerTool(Name = "render_model", Title = "Render a model to PNG", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Render an object's model (map-imported .mdx/.mdl) to a PNG written at out_path. Returns the saved path and byte size; small images also come back inline as MCP image content. The map itself is never modified.")]
    public static CallToolResult RenderModel(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Four-character object rawcode (e.g. 'u000'), or an internal model path (contains '\\', '/' or '.').")] string rawcode,
        [Description("Output PNG file path. Created/overwritten; parent directories are created as needed.")] string out_path,
        [Description("Object kind: unit|item|destructable|doodad. Default: auto-detect from the rawcode.")] string? kind = null,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var).")] string? game_dir = null,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run(() =>
        {
            if (string.IsNullOrWhiteSpace(out_path))
                throw Missing("out_path");
            var doc = LoadMap(map);

            // Anything that can't be a rawcode is treated as an internal model path (CLI parity).
            bool isPath = rawcode.Length != 4 || rawcode.IndexOfAny(new[] { '\\', '/', '.' }) >= 0;
            byte[] png = isPath
                ? RenderModelCommand.Execute(doc, rawcode)
                : kind is null
                    ? RenderModelCommand.Execute(doc, rawcode, ResolveGameDir(game_dir))
                    : RenderModelCommand.Execute(doc, ParseKind(kind), rawcode, ResolveGameDir(game_dir));

            var w = WriteGate.WriteBytes(out_path, png, Inputs(map), overwrite, expect_input_sha256);

            bool inline = png.Length <= InlineImageLimit;
            var info = new RenderModelToolResult(w.SavedTo, png.Length, inline,
                w.InputSha256, w.OutputSha256, w.FilesRewritten, w.BytesWritten);
            var content = new List<ContentBlock>
            {
                new TextContentBlock { Text = JsonSerializer.Serialize(info, Wc3McpServer.JsonOptions) },
            };
            if (inline)
                // FromBytes, not the Data setter. Data holds the BASE64-ENCODED bytes, so
                // assigning the raw PNG to it put binary where base64 text belongs, and the
                // reply did not survive the round trip. An MCP client failed to deserialize
                // it with "The JSON value could not be converted to ContentBlock.
                // Path: $.content[1]", so every agent calling render_model got that instead
                // of an image. FromBytes is the SDK's factory for decoded bytes and does the
                // encoding itself.
                content.Add(ImageContentBlock.FromBytes(png, "image/png"));
            return new CallToolResult { Content = content };
        });

    [McpServerTool(Name = "port_unit", Title = "Port a unit", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Port a unit (its custom-object closure + imported assets + strings, and best-effort its JASS trigger closure) from a source map into a target map, auto-remapping rawcode collisions. WRITES A NEW FILE '<target>.ported.<ext>' next to the target map (overwritten if it exists from a previous run) - the source and target maps themselves are NEVER modified. Returns the full port report: remaps, ported objects, copied files, inlined strings, script info and warnings.")]
    public static PortUnitToolResult PortUnit(
        [Description("Map to port FROM (.w3x/.w3m). Read-only.")] string source_map,
        [Description("Four-character rawcode of the unit to port, e.g. 'u000'.")] string rawcode,
        [Description("Map to port INTO (.w3x/.w3m). Read-only - the result is saved as a sibling '<target>.ported.<ext>' file.")] string target_map,
        [Description("Also carry the unit's JASS trigger-function closure into the target script (best effort). Default true.")] bool include_script = true,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var).")] string? game_dir = null,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run(() =>
        {
            var source = LoadMap(source_map);
            var target = LoadMap(target_map); // fresh load - ported output never feeds back into inputs
            var bundle = BundleCommand.ResolveUnit(source, rawcode, ResolveGameDir(game_dir));
            var report = PortCommand.PortUnit(source, bundle, target, includeScript: include_script);

            string targetFull = Path.GetFullPath(target_map);
            string outPath = Path.Combine(Path.GetDirectoryName(targetFull) ?? ".",
                Path.GetFileNameWithoutExtension(targetFull) + ".ported" + Path.GetExtension(targetFull));
            var w = SaveDoc(target, outPath, Inputs(source_map, target_map), overwrite, expect_input_sha256);
            return new PortUnitToolResult(w.SavedTo, report,
                w.InputSha256, w.OutputSha256, w.FilesRewritten, w.BytesWritten,
                PortFiles(source_map, target_map, w, "ported map"));
        });

    [McpServerTool(Name = "port_deps", Title = "Show a unit's dependencies", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Report the full dependency closure of a rawcode, its objects grouped by kind, its assets with their byte sizes, its display strings and its script functions. The readable shape of a dependency bundle, before porting it.")]
    public static PortDepsResult PortDeps(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Four-character object rawcode, e.g. 'H000'.")] string rawcode,
        [Description("Root object kind: unit|item|ability|destructable|doodad|buff|upgrade. Default unit.")] string? kind = null,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var).")] string? game_dir = null)
        => Run(() => PortDepsCommand.Execute(LoadMap(map), rawcode,
            kind is null ? ObjectKind.Unit : ParseKind(kind), ResolveGameDir(game_dir)));

    [McpServerTool(Name = "port_preview", Title = "Preview a port", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Dry run, listing exactly what a port of the rawcode into the target map would add or change, writing nothing. Same code path as port_unit, so the preview and the port cannot drift.")]
    public static PortResult PortPreview(
        [Description("Map to port FROM (.w3x/.w3m). Read-only.")] string source_map,
        [Description("Four-character rawcode of the unit to preview, e.g. 'H000'.")] string rawcode,
        [Description("Map to port INTO (.w3x/.w3m). Read-only - nothing is written.")] string target_map,
        [Description("Also report the JASS trigger-function closure that would be carried. Default true.")] bool include_script = true,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var).")] string? game_dir = null)
        => Run(() => PortPreviewCommand.Execute(
            LoadMap(source_map), rawcode, LoadMap(target_map), ResolveGameDir(game_dir), include_script));

    [McpServerTool(Name = "port_into_version", Title = "Port to an older build", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Port a unit, then downgrade the result to a target build's schema. Object fields the build does not declare are removed and script identifiers it lacks are flagged. Writes a NEW file at out_path - the source and target maps are NEVER modified.")]
    public static PortIntoVersionToolResult PortIntoVersion(
        [Description("Map to port FROM (.w3x/.w3m). Read-only.")] string source_map,
        [Description("Four-character rawcode of the unit to port, e.g. 'H000'.")] string rawcode,
        [Description("Map to port INTO (.w3x/.w3m). Read-only - the result is saved to out_path.")] string target_map,
        [Description("Target build name or schema folder path.")] string target,
        [Description("Path to write the downgraded map. Must differ from the source and target maps.")] string out_path,
        [Description("Schemas directory, when target is a build name rather than a folder path.")] string? schemas_dir = null,
        [Description("Also carry the unit's JASS trigger-function closure before downgrading. Default true.")] bool include_script = true,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var).")] string? game_dir = null,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run(() =>
        {
            string full = ResolveOutPath(target_map, out_path);
            var targetDoc = LoadMap(target_map);
            var report = PortIntoVersionCommand.Execute(
                LoadMap(source_map), rawcode, targetDoc, target, schemas_dir,
                ResolveGameDir(game_dir), include_script);
            var w = SaveDoc(targetDoc, full, Inputs(source_map, target_map), overwrite, expect_input_sha256);
            return new PortIntoVersionToolResult(w.SavedTo, report,
                w.InputSha256, w.OutputSha256, w.FilesRewritten, w.BytesWritten,
                PortFiles(source_map, target_map, w, "downgraded map"));
        });

    [McpServerTool(Name = "bundle_diff", Title = "Diff two bundles", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Compare two dependency bundles (the JSON written by 'bundle unit -o') by their object and asset members, listing what each adds, removes or renames.")]
    public static BundleDiffResult BundleDiff(
        [Description("Path to the first bundle JSON file.")] string bundle_a,
        [Description("Path to the second bundle JSON file.")] string bundle_b)
        => Run(() => BundleDiffCommand.Execute(LoadBundle(bundle_a), LoadBundle(bundle_b)));

    [McpServerTool(Name = "bundle_merge", Title = "Merge two bundles", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Merge two dependency bundles into one, written as JSON at out_path. Refuses when the same rawcode is defined differently in the two bundles (a different kind, custom-vs-base, or a contradicting name).")]
    public static object BundleMerge(
        [Description("Path to the first bundle JSON file.")] string bundle_a,
        [Description("Path to the second bundle JSON file.")] string bundle_b,
        [Description("Path to write the merged bundle JSON.")] string out_path,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run<object>(() =>
        {
            if (string.IsNullOrWhiteSpace(out_path)) throw Missing("out_path");
            var result = BundleMergeCommand.Execute(LoadBundle(bundle_a), LoadBundle(bundle_b));
            if (!result.Ok)
                throw Rejected(result.Message + " " + string.Join("; ",
                    result.Conflicts.Select(c => $"{c.Rawcode} {c.Detail}")));
            var gated = WriteWith(out_path, Inputs(bundle_a, bundle_b), overwrite, expect_input_sha256,
                temp => { BundleJson.Save(result.Merged!, temp); return result; });
            gated["files"] = new[]
            {
                new OutputFile("first bundle", Path.GetFullPath(bundle_a), ShaOrEmpty(bundle_a)),
                new OutputFile("second bundle", Path.GetFullPath(bundle_b), ShaOrEmpty(bundle_b)),
                new OutputFile("merged bundle", (string)gated["savedTo"]!, (string)gated["outputSha256"]!),
            };
            return gated;
        });

    /// <summary>Read a bundle JSON file, surfacing a clean MCP error when it cannot be read.</summary>
    private static UnitBundle LoadBundle(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) throw Missing("bundle path");
        if (!File.Exists(path)) throw Wc3Errors.Refuse(Wc3ErrorCodes.InputNotFound, $"bundle not found: {path}", path);
        try { return BundleJson.Load(path); }
        catch (Exception ex) { throw Wc3Errors.Refuse(Wc3ErrorCodes.InputUnreadable, $"could not read bundle '{path}': {ex.Message}", path); }
    }

    // ---- audits (diagnostics over data already in the map) ---------------

    [McpServerTool(Name = "audit_hero", Title = "Audit hero wiring", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("Say whether a placed hero's abilities are wired up in the script, not merely present as object data. For every ability it walks the chain end to end, the ability object exists, something dispatches a cast on its id, that handler is attached to a trigger, the trigger's event reaches the hero's player, the handler body still has live statements, any identity array it gates on lists this hero, and the init that builds the trigger runs exactly once. A missing link is named, so a broken port can be fixed rather than only detected. Omit hero to audit every placed hero.")]
    public static CappedPage<HeroWiringResult> AuditHero(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Hero rawcode to audit, e.g. 'H001'. Omit to audit every hero placed on the map.")] string? hero = null)
        => Run(() =>
        {
            var doc = LoadMap(map);
            IReadOnlyList<HeroWiringResult> rows = hero is null
                ? HeroWiringAudit.AuditPlacedHeroes(doc)
                : new[] { HeroWiringAudit.Audit(doc, hero, ownerId: 0) };
            return Page(Wc3Limit.Cap(rows));
        });

    [McpServerTool(Name = "audit_fidelity", Title = "Audit port fidelity", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Tells you whether a ported object's data came across faithfully, by comparing it and its whole custom-object dependency closure between the SOURCE map it was ported from and the TARGET map it was ported into. Reports fields the source set that the target dropped, per-level or per-variation values missing, whole levels lost, and objects present in the source closure but absent from the target, each labeled a real loss or merely informational. A rawcode remapped on a porting collision and a trigger string inlined to literal text are recognized as legitimate port transformations, not faults.")]
    public static ObjectFidelityResult AuditFidelity(
        [Description("Path to the SOURCE map the object was ported FROM.")] string source_map,
        [Description("Path to the TARGET map the object was ported INTO.")] string target_map,
        [Description("Root object rawcode to compare, the same rawcode in both maps.")] string rawcode,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Sharpens the dependency closure crawl. Without it the crawl is still complete, only more permissive.")] string? game_dir = null)
        => Run(() => ObjectFidelityCommand.Compare(LoadMap(source_map), LoadMap(target_map), rawcode, ResolveGameDir(game_dir)));

    [McpServerTool(Name = "audit_readiness", Title = "Audit script readiness", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("Say whether a placed hero's script would run correctly, not merely fire, the gap audit_hero cannot see since it only proves a cast reaches a handler. Checks that InitGlobals and RunInitializationTriggers, the two World Editor entry points that set every custom starting value and run any Map Initialization trigger, are carried and actually called, and that every udg_ global the hero's carried code reads is assigned somewhere rather than left at its type default. A hero can audit clean on audit_hero and still do nothing worth playing. Omit hero to check every hero placed.")]
    public static CappedPage<RuntimeReadinessResult> AuditReadiness(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Hero rawcode to check, e.g. 'H001'. Omit to check every hero placed on the map.")] string? hero = null)
        => Run(() =>
        {
            var doc = LoadMap(map);
            IReadOnlyList<RuntimeReadinessResult> rows = hero is null
                ? RuntimeReadinessCommand.CheckPlacedHeroes(doc)
                : new[] { RuntimeReadinessCommand.Check(doc, hero, ownerId: 0) };
            return Page(Wc3Limit.Cap(rows));
        });

    [McpServerTool(Name = "audit_ability", Title = "Audit ability runtime chain", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("Report PASS or the exact broken link, per ability, across a placed hero's runtime chain, deeper than audit_hero, which only proves a cast reaches a live trigger. Checks that the dispatch is reached exactly once (two live triggers make every affected spell fire twice), that handlers survive a port, that a follow-up TimerStart loop starts and its callback is declared, that every StartSpellUnit pause is matched by a live StopSpellUnit, that callbacks name a declared function, that globals read are assigned, and that named special effects still exist. Omit hero to audit every placed hero.")]
    public static CappedPage<AbilityAuditResult> AuditAbility(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Hero rawcode to audit, e.g. 'H001'. Omit to audit every hero placed on the map.")] string? hero = null)
        => Run(() =>
        {
            var doc = LoadMap(map);
            IReadOnlyList<AbilityAuditResult> rows = hero is null
                ? AbilityAuditCommand.AuditPlacedHeroes(doc)
                : new[] { AbilityAuditCommand.Audit(doc, hero, ownerId: 0) };
            return Page(Wc3Limit.Cap(rows));
        });

    [McpServerTool(Name = "palette_doodad", Title = "List placeable doodads", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("List the doodad types placeable on a map - the palette to consult before place_doodad. Unions the base-game doodad catalog (installed GameData) with the map's own object-data: custom New* doodads and modified Base* doodads. Each entry carries its four-char rawcode, a resolved display name (null when unresolvable), its source (base|map-custom|map-modified) and the base it derives from. Without a WC3 install the palette is map-only. The map is never modified.")]
    public static DoodadPaletteResult PaletteDoodad(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var).")] string? game_dir = null)
        => Run(() => PaletteCommand.DoodadPalette(LoadMap(map), ResolveGameDir(game_dir)));

    [McpServerTool(Name = "place_doodad", Title = "Place a doodad", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Place a doodad instance at (x, y) on the map's doodad layer (war3map.doo) and save the edited map to out_path. The input map is NEVER modified in place, so out_path must differ from it. Returns the assigned creation number. Pass the placement as one object, or the flattened type_rawcode, x, y, z, rotation, scale and variation arguments this tool shipped with, which stay for one version (SUR-08).")]
    public static PlacementToolResult PlaceDoodad(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). Created/overwritten; parent directories are created as needed. To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("The placement as one object. typeRawcode, x and y are required inside it, z defaults 0, rotation 0, scale 1 and variation 0. Pass either this or the flattened arguments, never both (SUR-08).")] DoodadPlacement? placement = null,
        [Description("Legacy flattened form of placement.typeRawcode. Required when placement is omitted. Default null, meaning placement was given instead.")] string? type_rawcode = null,
        [Description("Legacy flattened form of placement.x. Required when placement is omitted. Default null, meaning placement was given instead.")] float? x = null,
        [Description("Legacy flattened form of placement.y. Required when placement is omitted. Default null, meaning placement was given instead.")] float? y = null,
        [Description("Legacy flattened form of placement.z. Default null, which means 0.")] float? z = null,
        [Description("Legacy flattened form of placement.rotation. Default null, which means 0.")] float? rotation = null,
        [Description("Legacy flattened form of placement.scale. Default null, which means 1.")] float? scale = null,
        [Description("Legacy flattened form of placement.variation. Default null, which means 0.")] int? variation = null,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() =>
        {
            DoodadPlacement spec = DoodadPlacementOf(placement, type_rawcode, x, y, z, rotation, scale, variation);
            return SavePlacement(map, out_path, overwrite, expect_input_sha256, doc =>
            {
                var r = PlacementCommand.PlaceDoodad(doc, spec.TypeRawcode, spec.X, spec.Y, spec.Z,
                    spec.Rotation, spec.Scale, spec.Variation);
                return (r.Ok, r.Message, r.CreationNumber);
            });
        });

    [McpServerTool(Name = "place_region", Title = "Create a region", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Define a rectangular region on the map's region layer (war3map.w3r) and save the edited map to out_path. The input map is NEVER modified in place - out_path must differ from it. Bounds need right>left and top>bottom. Returns the assigned creation number.")]
    public static PlacementToolResult PlaceRegion(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). Created/overwritten; parent directories are created as needed. To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Region name (non-empty).")] string name,
        [Description("West edge, min X (world coordinate).")] float left,
        [Description("South edge, min Y (world coordinate).")] float bottom,
        [Description("East edge, max X (world coordinate).")] float right,
        [Description("North edge, max Y (world coordinate).")] float top,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SavePlacement(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = PlacementCommand.PlaceRegion(doc, name, left, bottom, right, top);
            return (r.Ok, r.Message, r.CreationNumber);
        }));

    [McpServerTool(Name = "place_unit", Title = "Place a unit", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Place a unit of the given type, owned by a player, at world (x, y) and save the edited map to out_path. The input map is NEVER modified in place, so out_path must differ from it. Returns the assigned creation number (the id triggers use to reference the unit). Pass the placement as one object, or the flattened type_rawcode, owner_id, x, y, z, rotation and scale arguments this tool shipped with, which stay for one version (SUR-08).")]
    public static PlacementToolResult PlaceUnit(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). Created/overwritten; parent directories are created as needed. To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("The placement as one object. typeRawcode, ownerId, x and y are required inside it, z defaults 0, rotation 0 and scale 1. Pass either this or the flattened arguments, never both (SUR-08).")] UnitPlacement? placement = null,
        [Description("Legacy flattened form of placement.typeRawcode. Required when placement is omitted. Default null, meaning placement was given instead.")] string? type_rawcode = null,
        [Description("Legacy flattened form of placement.ownerId. Required when placement is omitted. Default null, meaning placement was given instead.")] int? owner_id = null,
        [Description("Legacy flattened form of placement.x. Required when placement is omitted. Default null, meaning placement was given instead.")] float? x = null,
        [Description("Legacy flattened form of placement.y. Required when placement is omitted. Default null, meaning placement was given instead.")] float? y = null,
        [Description("Legacy flattened form of placement.z. Default null, which means 0.")] float? z = null,
        [Description("Legacy flattened form of placement.rotation. Default null, which means 0.")] float? rotation = null,
        [Description("Legacy flattened form of placement.scale. Default null, which means 1.")] float? scale = null,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() =>
        {
            UnitPlacement spec = UnitPlacementOf(placement, type_rawcode, owner_id, x, y, z, rotation, scale);
            return SavePlacement(map, out_path, overwrite, expect_input_sha256, doc =>
            {
                var r = PlacementCommand.PlaceUnit(doc, spec.TypeRawcode, spec.OwnerId, spec.X, spec.Y,
                    spec.Z, spec.Rotation, spec.Scale);
                return (r.Ok, r.Message, r.CreationNumber);
            });
        });

    [McpServerTool(Name = "place_start_location", Title = "Place a start location", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Place or move a player's start location at world (x, y) and save the edited map to out_path. The input map is NEVER modified in place. A start location is stored as a preplaced 'sloc' unit owned by the player; the World Editor permits exactly one per player, so if this player already has one it is moved (its creation number preserved) rather than duplicated. Returns the start location's creation number.")]
    public static PlacementToolResult PlaceStartLocation(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). Created/overwritten; parent directories are created as needed. To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Player whose start location this is (0-based; 0 = red).")] int player,
        [Description("World X coordinate.")] float x,
        [Description("World Y coordinate.")] float y,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SavePlacement(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = PlacementCommand.PlaceStartLocation(doc, player, x, y);
            return (r.Ok, r.Message, r.CreationNumber);
        }));

    [McpServerTool(Name = "place_item", Title = "Place an item", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Place a preplaced item of the given type at world (x, y) and save the edited map to out_path. Items share war3mapUnits.doo with units (WC3 spawns a ground item because the rawcode is an item), so the item has no owning player. The input map is NEVER modified in place. Returns the assigned creation number.")]
    public static PlacementToolResult PlaceItem(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). Created/overwritten; parent directories are created as needed. To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Four-character item type rawcode, e.g. 'bspd' (Boots of Speed).")] string type_rawcode,
        [Description("World X coordinate.")] float x,
        [Description("World Y coordinate.")] float y,
        [Description("Height above ground in world units. Default 0.")] float z = 0f,
        [Description("Facing angle in radians. Default 0.")] float rotation = 0f,
        [Description("Uniform scale. Default 1.")] float scale = 1f,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SavePlacement(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = PlacementCommand.PlaceItem(doc, type_rawcode, x, y, z, rotation, scale);
            return (r.Ok, r.Message, r.CreationNumber);
        }));

    // ---- terrain editing -------------------------------------------------

    private const string ShapeValues = "circle|square";

    // Compile-time mirror of SoundCommand.EditableFields (attribute args must be const).
    private const string SoundFieldValues =
        "Name|File|Eax|Volume|Pitch|PitchVariance|FadeIn|FadeOut|Priority|Channel|Flags|" +
        "MinDistance|MaxDistance|DistanceCutoff|ConeInside|ConeOutside|ConeOutsideVolume";

    [McpServerTool(Name = "terrain_stats", Title = "Terrain summary", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Summarize a map's terrain (war3map.w3e): tile count, min/mean/max ground height and min/max cliff level. Read-only.")]
    public static TerrainCommand.StatsResult TerrainStats(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => TerrainCommand.Stats(LoadMap(map)));

    [McpServerTool(Name = "terrain_audit", Title = "Audit terrain waste", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Audit a map's terrain (war3map.w3e) for its size and its measurable waste: grid and tile counts, distinct ground and cliff textures in use, cliff/ramp/water/blight totals, the war3map.w3e byte size, and any uniform, all-zero or out-of-range-tileset pattern found. Reports which checks it ran. Read-only.")]
    public static TerrainAuditResult TerrainAudit(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => TerrainAuditCommand.ExecutePath(map));

    [McpServerTool(Name = "terrain_optimize", Title = "Shrink the terrain file", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Shrink a map's war3map.w3e by dropping the declared ground and cliff tile types the terrain never references and remapping every corner's ground and cliff texture index onto the compacted list, saving the edited copy to out_path. Every other file is written back byte for byte. Reports the before and after declared tile counts and the war3map.w3e byte saving. The input map is never modified.")]
    public static object TerrainOptimize(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). Created/overwritten; parent directories are created as needed.")] string out_path,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run<object>(() => WriteWith(ResolveOutPath(map, out_path), Inputs(map), overwrite, expect_input_sha256,
            temp => TerrainOptimizeCommand.ExecutePath(map, temp), "path"));

    [McpServerTool(Name = "terrain_deform", Title = "Deform terrain height", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Raise/lower/set/flatten/smooth ground height over a circular or square brush, saving the edited map to out_path. The input map is NEVER modified in place.")]
    public static TerrainToolResult TerrainDeform(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). Created/overwritten; parent directories are created as needed. To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Brush centre tile X (0-based column into war3map.w3e).")] int center_x,
        [Description("Brush centre tile Y (0-based row into war3map.w3e).")] int center_y,
        [Description("Brush radius in tiles (>= 0).")] int radius,
        [Description("Height operation: raise|lower|set|flatten|smooth.")] string op = "raise",
        [Description("Step for raise/lower; target for set; ignored by flatten/smooth. WC3 world units.")] float amount = 1f,
        [Description("Brush footprint: " + ShapeValues + ".")] string shape = "circle",
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveTerrain(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = TerrainCommand.Deform(doc, center_x, center_y, radius,
                ParseEnum<TerrainCommand.HeightOp>(op, "op"), amount,
                ParseEnum<TerrainCommand.BrushShape>(shape, "shape"));
            return (r.Ok, r.Message, r.TilesChanged);
        }));

    [McpServerTool(Name = "terrain_cliff", Title = "Edit cliff levels", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Raise/lower/set the cliff (stepped-terrain) level over a brush, saving the edited map to out_path. The input map is NEVER modified in place.")]
    public static TerrainToolResult TerrainCliff(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Brush centre tile X.")] int center_x,
        [Description("Brush centre tile Y.")] int center_y,
        [Description("Brush radius in tiles (>= 0).")] int radius,
        [Description("Cliff operation: raise|lower|set.")] string op = "raise",
        [Description("Step count for raise/lower; the absolute cliff level for set.")] int level = 1,
        [Description("Brush footprint: " + ShapeValues + ".")] string shape = "circle",
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveTerrain(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = TerrainCommand.Cliff(doc, center_x, center_y, radius,
                ParseEnum<TerrainCommand.CliffOp>(op, "op"), level,
                ParseEnum<TerrainCommand.BrushShape>(shape, "shape"));
            return (r.Ok, r.Message, r.TilesChanged);
        }));

    [McpServerTool(Name = "terrain_ramp", Title = "Toggle terrain ramps", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Toggle the ramp (sloped cliff transition) flag over a brush, saving the edited map to out_path. The input map is NEVER modified in place.")]
    public static TerrainToolResult TerrainRamp(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Brush centre tile X.")] int center_x,
        [Description("Brush centre tile Y.")] int center_y,
        [Description("Brush radius in tiles (>= 0).")] int radius,
        [Description("true = set the ramp flag; false = clear it.")] bool on = true,
        [Description("Brush footprint: " + ShapeValues + ".")] string shape = "circle",
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveTerrain(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = TerrainCommand.Ramp(doc, center_x, center_y, radius, on,
                ParseEnum<TerrainCommand.BrushShape>(shape, "shape"));
            return (r.Ok, r.Message, r.TilesChanged);
        }));

    [McpServerTool(Name = "terrain_paint", Title = "Paint a ground texture", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Paint a ground texture over a brush, saving the edited map to out_path. texture_index selects a slot in the map's tileset table. The input map is NEVER modified in place.")]
    public static TerrainToolResult TerrainPaint(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Brush centre tile X.")] int center_x,
        [Description("Brush centre tile Y.")] int center_y,
        [Description("Brush radius in tiles (>= 0).")] int radius,
        [Description("Ground texture slot index in the map's tileset table.")] int texture_index,
        [Description("Optional tile variation; omit to choose the default.")] int? variation = null,
        [Description("Brush footprint: " + ShapeValues + ".")] string shape = "circle",
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveTerrain(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = TerrainCommand.Paint(doc, center_x, center_y, radius, texture_index, variation,
                ParseEnum<TerrainCommand.BrushShape>(shape, "shape"));
            return (r.Ok, r.Message, r.TilesChanged);
        }));

    [McpServerTool(Name = "terrain_water", Title = "Edit water level", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Set/raise/lower/remove water over a brush, saving the edited map to out_path. The input map is NEVER modified in place.")]
    public static TerrainToolResult TerrainWater(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Brush centre tile X.")] int center_x,
        [Description("Brush centre tile Y.")] int center_y,
        [Description("Brush radius in tiles (>= 0).")] int radius,
        [Description("Water operation: set|raise|lower|remove. set/raise/lower flag the tile as water; remove clears the flag.")] string op = "set",
        [Description("Absolute water height for set; delta for raise/lower; ignored by remove. WC3 world units.")] float amount = 0f,
        [Description("Brush footprint: " + ShapeValues + ".")] string shape = "circle",
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveTerrain(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = TerrainCommand.Water(doc, center_x, center_y, radius,
                ParseEnum<TerrainCommand.WaterOp>(op, "op"), amount,
                ParseEnum<TerrainCommand.BrushShape>(shape, "shape"));
            return (r.Ok, r.Message, r.TilesChanged);
        }));

    [McpServerTool(Name = "terrain_blight", Title = "Set or clear blight", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Set or clear the blight (corrupted ground) flag over a brush, saving the edited map to out_path. The input map is NEVER modified in place.")]
    public static TerrainToolResult TerrainBlight(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Brush centre tile X.")] int center_x,
        [Description("Brush centre tile Y.")] int center_y,
        [Description("Brush radius in tiles (>= 0).")] int radius,
        [Description("true = blight the tiles; false = clear blight.")] bool on = true,
        [Description("Brush footprint: " + ShapeValues + ".")] string shape = "circle",
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveTerrain(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = TerrainCommand.Blight(doc, center_x, center_y, radius, on,
                ParseEnum<TerrainCommand.BrushShape>(shape, "shape"));
            return (r.Ok, r.Message, r.TilesChanged);
        }));

    [McpServerTool(Name = "terrain_export_heightmap", Title = "Export a heightmap PNG", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Export a map's terrain heights as a 16-bit grayscale PNG of the (Width+1) by (Height+1) corner grid, one pixel per corner, to out_path. The input map is never modified, and the same file re-imports through terrain_import_heightmap.")]
    public static ImageExportToolResult TerrainExportHeightmap(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Output PNG path. Parent directories are created as needed.")] string out_path,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run(() =>
        {
            var r = TerrainExportHeightmapCommand.Export(LoadMap(map));
            if (!r.Ok || r.Png is null) throw Rejected(r.Message);
            var w = WriteGate.WriteBytes(ResolveOutPath(map, out_path), r.Png, Inputs(map),
                overwrite, expect_input_sha256);
            return new ImageExportToolResult(w.SavedTo, r.Message, r.Width, r.Height, r.Png.Length,
                w.InputSha256, w.OutputSha256, w.FilesRewritten, w.BytesWritten);
        });

    [McpServerTool(Name = "terrain_import_heightmap", Title = "Import a heightmap PNG", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Set the terrain tile heights from a grayscale PNG and save the edited map to out_path. The PNG dimensions must match the map's corner grid exactly or the call is refused. Only ground height is written, and cliff level, texture, water and flags are untouched. The input map is NEVER modified in place.")]
    public static TerrainToolResult TerrainImportHeightmap(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Path to the grayscale heightmap PNG, as produced by terrain_export_heightmap.")] string png,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveTerrain(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = TerrainImportHeightmapCommand.Import(doc, ReadPngFile(png));
            return (r.Ok, r.Message, r.TilesChanged);
        }));

    [McpServerTool(Name = "terrain_smooth", Title = "Smooth the whole map", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Smooth the whole map: each pass replaces every corner height with the mean of itself and its in-grid four neighbours, saving the edited map to out_path. The input map is NEVER modified in place.")]
    public static TerrainToolResult TerrainSmooth(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Number of smoothing passes to run. 0 makes no change. Default 1.")] int passes = 1,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveTerrain(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = TerrainSmoothCommand.Smooth(doc, passes);
            return (r.Ok, r.Message, r.TilesChanged);
        }));

    // Not ReadOnly, because passing out_path writes a copy. The annotation is per tool, not per
    // call, so a tool that can write must not claim to be read-only.
    [McpServerTool(Name = "deprotect_map", Title = "Recover protected file names", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Recover the file names a protected map stripped. A protected map ships no "
        + "(listfile), so most entries have no name and cannot be looked up by name at all. "
        + "Measured on one real map, 793 of 851 entries were nameless and recovery reached 832. "
        + "Names come from the map's own object data, its script literals, the texture paths its "
        + "models declare, any listfiles supplied, and an asset-name sweep of every block. "
        + "Without out_path nothing is written. With it, the recovered names go into a copy at "
        + "out_path and the input map is never modified in place.")]
    public static object DeprotectMap(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Optional output map path. When given, the recovered names are written into a copy; the input map is NEVER modified in place. Named entries are recompressed, every other entry stays byte-faithful.")] string? out_path = null,
        [Description("Paths to extra name dictionaries, one name per line. Community listfiles and sibling maps' listfiles both work. At most 1000, and a longer list is refused with a tool error naming the limit.")] string[]? listfiles = null,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run<object>(() =>
        {
            RequireAtMost(listfiles?.Length ?? 0, "listfiles");
            var doc = LoadMap(map);
            var r = NameRecoveryCommand.Execute(doc, listfiles ?? Array.Empty<string>());
            if (string.IsNullOrWhiteSpace(out_path)) return r;
            NameRecoveryCommand.ApplyNames(doc, r);
            return Gated(r, SaveDocAt(doc, ResolveOutPath(map, out_path), map, overwrite, expect_input_sha256));
        });

    // ---- mpq container tools ---------------------------------------------

    [McpServerTool(Name = "mpq_verify", Title = "Verify archive entries", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Open every entry of a map archive and read it to its end, reporting any that "
        + "fail with the error raised, plus a summary count. Use this to prove an archive is fully "
        + "usable rather than merely addressable. Read-only.")]
    public static MpqVerifyResult MpqVerify(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => MpqVerifyCommand.Execute(RequireMapPath(map)));

    [McpServerTool(Name = "mpq_listfile", Title = "Report listfile coverage", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Report an archive's listfile coverage in three parts, the entries named by the "
        + "listfile it ships, the entries recovered by the standard-name probe, and the entries "
        + "with no name at all. Read-only.")]
    public static MpqListfileResult MpqListfile(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => MpqListfileCommand.Execute(RequireMapPath(map)));

    [McpServerTool(Name = "mpq_name_recover", Title = "Recover archive names", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Resolve a dictionary of candidate names against the archive's hash table and "
        + "report the names that hit unnamed blocks. Narrower than deprotect_map, which harvests "
        + "its own candidates from the map. Read-only.")]
    public static MpqNameRecoverResult MpqNameRecover(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Path to a dictionary file, one candidate name per line.")] string dictionary)
        => Run(() => MpqNameRecoverCommand.Execute(LoadMap(map), dictionary));

    [McpServerTool(Name = "mpq_strip_bookkeeping", Title = "Strip bookkeeping entries", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Save a copy of a map with the (listfile) and (attributes) bookkeeping entries "
        + "dropped, then confirm the copy opens again. The input map is never overwritten. "
        + "out_path defaults to a sibling '<map>.edited.<ext>'.")]
    public static object MpqStripBookkeeping(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Output map path. Defaults to a sibling '<map>.edited.<ext>'. Must differ from the input.")] string? out_path = null,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run<object>(() =>
        {
            string full = ResolveOutPath(RequireMapPath(map), out_path);
            return WriteWith(full, Inputs(map), overwrite, expect_input_sha256,
                temp => MpqStripBookkeepingCommand.Execute(map, temp));
        });

    [McpServerTool(Name = "mpq_bytes_diff", Title = "Diff archives entry by entry", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Compare two archives entry by entry by name. For each name present in both, "
        + "report whether the payload SHA256 matches, and list names only one side carries. The "
        + "byte-level counterpart of the structural mpq-diff. Read-only.")]
    public static MpqBytesDiffResult MpqBytesDiff(
        [Description("Path to the first .w3x/.w3m map, or to an unpacked map folder.")] string map,
        [Description("Path to the second .w3x/.w3m map.")] string other)
        => Run(() => MpqBytesDiffCommand.Execute(RequireMapPath(map), RequireMapPath(other)));

    [McpServerTool(Name = "mpq_rebuild", Title = "Rebuild the archive", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Rebuild a map archive from scratch into a healthy one, re-placing every readable "
        + "entry and dropping the blocks protection made unreadable or left nameless. Reports the "
        + "entry counts before and after and the names dropped. Writes a copy, the input map is "
        + "never modified in place.")]
    public static object MpqRebuild(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Output map path. The input map is never overwritten.")] string? out_path,
        [Description("Force a larger hash table. Doubles the source capacity, then raises it until 3/4 occupancy fits. Default false.")] bool grow_hash = false,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run<object>(() =>
        {
            string full = ResolveOutPath(RequireMapPath(map), out_path);
            return WriteWith(full, Inputs(map), overwrite, expect_input_sha256,
                temp => MpqRebuildCommand.Execute(map, temp, grow_hash));
        });

    [McpServerTool(Name = "mpq_merge", Title = "Merge two archives", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Merge two map archives into one, taking every readable named entry from both. A "
        + "name in both is resolved by the preferred side and reported as a conflict. Writes a copy "
        + "of the first map plus both maps' entries, the input maps are never modified.")]
    public static object MpqMerge(
        [Description("Path to the first .w3x/.w3m map, or to an unpacked map folder.")] string map,
        [Description("Path to the second .w3x/.w3m map.")] string other,
        [Description("Output map path. The input maps are never overwritten.")] string? out_path,
        [Description("Which side wins a name both maps carry, 'a' or 'b'. Default 'a'.")] string prefer = "a",
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run<object>(() =>
        {
            string full = ResolveOutPath(RequireMapPath(map), out_path);
            return WriteWith(full, Inputs(map, other), overwrite, expect_input_sha256,
                temp => MpqMergeCommand.Execute(map, other, temp, prefer));
        });

    [McpServerTool(Name = "mpq_add", Title = "Add an archive file", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Add or replace one internal archive file from a file on disk, byte-faithfully. "
        + "Reports whether the name was added or replaced and whether the payload round-trips. "
        + "Writes a copy, the input map is never modified in place.")]
    public static object MpqAdd(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Internal archive path, for example 'war3mapImported\\\\model.mdx'.")] string name,
        [Description("Path to the file whose bytes become the internal file.")] string from,
        [Description("Output map path. The input map is never overwritten.")] string? out_path,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run<object>(() =>
        {
            string full = ResolveOutPath(RequireMapPath(map), out_path);
            return WriteWith(full, Inputs(map, from), overwrite, expect_input_sha256,
                temp => MpqAddCommand.Execute(map, name, from, temp));
        });

    [McpServerTool(Name = "mpq_remove", Title = "Remove an archive file", ReadOnly = false, Destructive = true, Idempotent = false, OpenWorld = false)]
    [Description("Remove one internal file by rebuilding the archive without it, and report "
        + "whether the rebuild actually dropped the name. Writes a copy, the input map is never "
        + "modified in place.")]
    public static object MpqRemove(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Internal archive path to remove, for example 'war3map.j'.")] string name,
        [Description("Output map path. The input map is never overwritten.")] string? out_path,
        [Description("State the intent to destroy data. Without it the call is refused with CONFIRM_REQUIRED, naming what would be removed.")] bool confirm = false,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => RunConfirmed<object>(confirm, "removes '" + name + "' from the archive", () =>
        {
            string full = ResolveOutPath(RequireMapPath(map), out_path);
            return WriteWith(full, Inputs(map), overwrite, expect_input_sha256,
                temp => MpqRemoveCommand.Execute(map, name, temp));
        });

    [McpServerTool(Name = "mpq_remove_many", Title = "Remove several archive files", ReadOnly = false, Destructive = true, Idempotent = false, OpenWorld = false)]
    [Description("Remove many internal files in one rebuild and report how many were dropped. Writes a copy, the input map is never modified in place. The one-rebuild form of mpq_remove, for pruning the files asset_unused reports as unreferenced.")]
    public static object MpqRemoveMany(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Internal archive paths to remove, for example one asset_unused path per element. At most 1000, and a longer list is refused with a tool error naming the limit.")] string[] names,
        [Description("Output map path. The input map is never overwritten.")] string out_path,
        [Description("State the intent to destroy data. Without it the call is refused with CONFIRM_REQUIRED, naming what would be removed.")] bool confirm = false,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => RunConfirmed<object>(confirm, "removes " + names.Length + " entries from the archive", () =>
        {
            RequireAtMost(names.Length, "names");
            string full = ResolveOutPath(RequireMapPath(map), out_path);
            return WriteWith(full, Inputs(map), overwrite, expect_input_sha256,
                temp => MpqRemoveCommand.ExecuteMany(map, names, temp));
        });

    [McpServerTool(Name = "mpq_extract_all", Title = "Extract every entry", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Extract every entry of a map archive to a directory, including unnamed protected "
        + "blocks, through the same path-safety rule 'extract' uses. The map is never modified. "
        + "Returns the output directory, the counts and the written file names.")]
    public static MpqExtractAllToolResult MpqExtractAll(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Directory the entries are written under. Created if absent.")] string? out_dir,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run(() =>
        {
            if (string.IsNullOrWhiteSpace(out_dir)) throw Missing("out_dir");
            var r = MpqExtractAllCommand.Execute(LoadMap(map));
            var w = WriteGate.WriteFolder(out_dir, Inputs(map), overwrite, expect_input_sha256, temp =>
            {
                var rootPrefix = temp.EndsWith(Path.DirectorySeparatorChar) ? temp : temp + Path.DirectorySeparatorChar;
                foreach (var item in r.Items)
                {
                    var relative = ExtractPaths.RelativePathFor(item);
                    var dest = Path.GetFullPath(Path.Combine(temp, relative));
                    if (!dest.StartsWith(rootPrefix, StringComparison.OrdinalIgnoreCase))
                        throw Wc3Errors.Refuse(Wc3ErrorCodes.OutsideOutputDirectory, $"refusing to write outside the output directory, {item.Name}", item.Name);
                    Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
                    File.WriteAllBytes(dest, item.Bytes);
                }
            });
            return new MpqExtractAllToolResult(w.SavedTo, r.TotalEntries, r.NamedEntries,
                r.UnnamedEntries, r.TotalBytes, r.Items.Select(ExtractPaths.RelativePathFor).ToList(),
                w.InputSha256, w.OutputSha256, w.FilesRewritten, w.BytesWritten);
        });

    [McpServerTool(Name = "file_list", Title = "List internal files", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("List the archive's internal entries with their CURRENT size, which reflects a "
        + "pending replacement rather than the stale original. Name is null for an unnamed entry, "
        + "which is what a protected map produces.")]
    public static CappedPage<FileState> FileList(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => Page(Wc3Limit.Cap(FileEditCommand.ListFiles(LoadMap(map)))));

    [McpServerTool(Name = "file_get_text", Title = "Read an internal text file", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Read one internal file as text, naming the encoding it used and whether a byte order mark declared that encoding or it was assumed. Reports how many bytes the decoder had to replace and refuses a lossy decode with LOSSY_DECODE unless allow_lossy is true, so a binary entry is never handed back as corrupt text. Use file_extract to write the raw bytes instead.")]
    public static FileTextResult FileGetText(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Internal archive path, for example 'war3mapSkin.txt'.")] string internal_path,
        [Description("Return the text even when the decode replaced bytes, which hides the damage. Default false.")] bool allow_lossy = false)
        => Run(() =>
        {
            var doc = LoadMap(map);
            var entry = doc.GetFile(internal_path)
                ?? throw Wc3Errors.Refuse(Wc3ErrorCodes.ObjectNotFound, $"the map has no internal file '{internal_path}'", internal_path);
            var (text, encoding, declared, replaced) = DecodeText(entry.CurrentBytes);
            if (replaced > 0 && !allow_lossy)
                throw new ToolError(Wc3ErrorCodes.LossyDecode.Code,
                    $"'{internal_path}' does not decode as {encoding} without replacing {replaced} byte(s)",
                    "pass allow_lossy true to read it anyway, or use file_extract to write the raw bytes");
            return new FileTextResult(internal_path, text, encoding, declared, replaced, replaced > 0);
        });

    [McpServerTool(Name = "file_extract", Title = "Extract an internal file", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Write one internal archive entry to out_path exactly as the archive holds it, so a binary entry, or one whose text decode would be lossy, can be read without corruption. The map is never modified. Refuses an out_path equal to the input map, and an out_path that already exists without overwrite. Returns where it landed, the entry name, its size and its digest.")]
    public static FileExtractResult FileExtract(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Internal archive path to extract, for example 'war3map.w3e'.")] string internal_path,
        [Description("Disk file to write the entry's bytes to.")] string out_path,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run(() =>
        {
            var doc = LoadMap(map);
            var entry = doc.GetFile(internal_path)
                ?? throw Wc3Errors.Refuse(Wc3ErrorCodes.ObjectNotFound, $"the map has no internal file '{internal_path}'", internal_path);
            byte[] bytes = entry.CurrentBytes;
            var w = WriteGate.WriteBytes(out_path, bytes, Inputs(map), overwrite, expect_input_sha256);
            return new FileExtractResult(w.SavedTo, internal_path, bytes.Length, w.OutputSha256,
                w.InputSha256, w.FilesRewritten, w.BytesWritten);
        });

    /// <summary>
    /// Decode an internal file as text, naming the encoding it used and counting the bytes the
    /// decoder had to replace. A byte order mark declares the encoding, otherwise UTF-8 is assumed,
    /// so a caller can tell a measured answer from a guess (RES-04).
    /// </summary>
    private static (string Text, string Encoding, bool Declared, int Replaced) DecodeText(byte[] bytes)
    {
        string name = "utf-8";
        bool declared = false;
        Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        int bom = 0;
        if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
        {
            declared = true;
            bom = 3;
        }
        else if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
        {
            encoding = new UnicodeEncoding(bigEndian: false, byteOrderMark: false);
            name = "utf-16le";
            declared = true;
            bom = 2;
        }
        else if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
        {
            encoding = new UnicodeEncoding(bigEndian: true, byteOrderMark: false);
            name = "utf-16be";
            declared = true;
            bom = 2;
        }

        var counted = new CountingFallback();
        var decoder = (Encoding)encoding.Clone();
        decoder.DecoderFallback = counted;
        string text = decoder.GetString(bytes, bom, bytes.Length - bom);
        if (text.Length > 0 && text[0] == '\uFEFF') text = text[1..];
        return (text, name, declared, counted.Bytes);
    }

    /// <summary>
    /// Counts the bytes a decode replaced rather than throwing on the first bad sequence or
    /// substituting them unseen, so a lossy read is reported instead of hidden (RES-04).
    /// </summary>
    private sealed class CountingFallback : DecoderFallback
    {
        public int Bytes { get; private set; }

        public override int MaxCharCount => 1;

        public override DecoderFallbackBuffer CreateFallbackBuffer() => new Buffer(this);

        private sealed class Buffer : DecoderFallbackBuffer
        {
            private readonly CountingFallback _owner;
            private bool _pending;

            public Buffer(CountingFallback owner) => _owner = owner;

            public override int Remaining => _pending ? 1 : 0;

            public override bool Fallback(byte[] bytesUnknown, int index)
            {
                _owner.Bytes += bytesUnknown.Length;
                _pending = true;
                return true;
            }

            public override char GetNextChar()
            {
                if (!_pending) return '\0';
                _pending = false;
                return '\uFFFD';
            }

            public override bool MovePrevious() => false;
        }
    }

    [McpServerTool(Name = "file_set", Title = "Write an internal file", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Replace an internal file's bytes verbatim from a disk file and save the edited "
        + "map to out_path. The input map is NEVER modified in place. Bytes are staged unchanged, "
        + "so a file carrying a BOM or CRLF round-trips exactly, which is what keeps the archive "
        + "byte-faithful. Content comes from a disk path rather than an argument because these "
        + "payloads are multi-line and full of pipes and colour codes.")]
    public static FileSetResult FileSet(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Internal archive path to replace or add, for example 'war3mapSkin.txt'.")] string internal_path,
        [Description("Disk file whose exact bytes become the new payload.")] string from,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run(() =>
        {
            string full = ResolveOutPath(map, out_path);
            var doc = LoadMap(map);
            var r = FileEditCommand.AddOrReplace(doc, internal_path, File.ReadAllBytes(from));
            var w = SaveDocAt(doc, full, map, overwrite, expect_input_sha256);
            return new FileSetResult(w.SavedTo, r.Name, r.Replaced, r.SizeBytes,
                w.InputSha256, w.OutputSha256, w.FilesRewritten, w.BytesWritten);
        });

    [McpServerTool(Name = "audit_map", Title = "Audit object behaviour", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Behavioural audit of a map's object data, read-only. Where validate asks whether a map loads, this asks whether its abilities do what they claim, by comparing the author's tooltips against the data and resolving every object reference. Checks level gap, level tooltip, tooltip claim, orphan ability, requirement, portrait risk, dangling reference and missing model, the last being a path that loads no file, which the engine logs and retries on every creation. Requirement and dangling reference need an installed game and report a diagnostic rather than a false clean.")]
    public static AuditResult AuditMap(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Restrict to one check by name. Default: run all eight.")] string? check = null,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Without it the requirement and dangling-reference checks are skipped, with a diagnostic.")] string? game_dir = null)
        => Run(() => AuditCommand.Execute(LoadMap(map), ResolveGameDir(game_dir),
            check is null ? null : new[] { check }));

    [McpServerTool(Name = "audit_ability_orders", Title = "Audit shared order ids", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Find Channel-based abilities that share a Base Order ID (Ncl6). Channel dispatches by order string, so two abilities on one unit with the same string can both fire from a single key press. Read-only, no game install, no script run. With a hero rawcode it checks only that hero's uabi list, which is the precise case. Also reports levels that disagree and empty order ids.")]
    public static AbilityOrderAuditResult AuditAbilityOrders(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Optional four-character hero rawcode whose uabi list bounds the check.")] string? hero = null)
        => Run(() => AbilityOrderAuditCommand.Execute(LoadMap(map), hero));

    [McpServerTool(Name = "audit_trend", Title = "Diff two audit runs", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Run the behavioural audit against two maps and report the findings the second added and removed, grouped by check. Where audit_map answers what is wrong now, this answers what an edit or a port changed. Issues are matched by check, rawcode and message.")]
    public static AuditTrendResult AuditTrend(
        [Description("Path to the first .w3x/.w3m map file, or to an unpacked map folder.")] string map,
        [Description("Path to the second .w3x/.w3m map file, the one treated as the newer state.")] string other,
        [Description("Restrict to one check by name. Default: run all eight.")] string? check = null,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var). Without it the requirement and dangling-reference checks are skipped, with a diagnostic.")] string? game_dir = null)
        => Run(() => AuditTrendCommand.Execute(map, other, ResolveGameDir(game_dir),
            check is null ? null : new[] { check }));

    [McpServerTool(Name = "sound_import_events", Title = "Import sound events", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Import a sound event set into a map and save the edited copy. Each event maps to clips with a cooldown, a voice cap and a priority. Writes one sound catalog entry per clip, refuses an event whose clip is missing, and returns a generated JASS dispatcher that should be appended to the map's script. The input map is never modified.")]
    public static SoundImportToolResult SoundImportEvents(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder.")] string map,
        [Description("Output map path. Defaults to a sibling .edited copy when omitted.")] string? out_path,
        [Description("JSON mapping event names to clips, cooldown, maxVoices and priority.")] string events_json,
        [Description("Folder holding the clip files.")] string clips_dir,
        [Description("Replace out_path when it already exists.")] bool overwrite = false,
        [Description("The SHA-256 the caller believes the input map has. A mismatch refuses as STALE_INPUT.")] string? expect_input_sha256 = null)
        => Run(() =>
        {
            var doc = LoadMap(map);
            var r = SoundImportCommand.Import(doc, events_json, clips_dir);
            if (!r.Ok) throw Rejected(r.Message);
            var w = WriteGate.Save(doc, map, out_path, overwrite, expect_input_sha256, LoadedShaOf(doc));
            return new SoundImportToolResult(w.SavedTo, w.InputSha256, w.OutputSha256,
                r.Message, r.Events.Count, r.CatalogEntries.Count, r.MissingClips, r.Refused, r.Jass);
        });

    [McpServerTool(Name = "sound_list", Title = "List sound definitions", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("List the map's sound catalog (war3map.w3s): each definition's name, file path, channel, flags, volume, pitch, priority and min/max distance.")]
    public static CappedPage<SoundFields> SoundList(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => Page(Wc3Limit.Cap(SoundCommand.List(LoadMap(map)))));

    [McpServerTool(Name = "sound_add", Title = "Add a sound definition", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Add a new sound definition (keyed by name) to the map's sound catalog and save the edited map to out_path. The input map is NEVER modified in place.")]
    public static SoundToolResult SoundAdd(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Sound definition name (the label triggers/UI reference). Must be unique.")] string name,
        [Description("Sound file path, e.g. 'Sound\\Ambient\\...'. Optional.")] string? file = null,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run(() => SaveSound(map, out_path, overwrite, expect_input_sha256,
            doc => SoundCommand.Add(doc, name, file)));

    [McpServerTool(Name = "sound_set", Title = "Edit a sound definition", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Set a field on a sound definition and save the edited map to out_path. The input map is NEVER modified in place. Editable fields: " + SoundFieldValues + ".")]
    public static SoundToolResult SoundSet(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Sound definition name to edit.")] string name,
        [Description("Field to set: " + SoundFieldValues + ".")] string field,
        [Description("New value (parsed to the field's type: int/real/bool/string).")] string value,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run(() => SaveSound(map, out_path, overwrite, expect_input_sha256,
            doc => SoundCommand.Set(doc, name, field, value)));

    [McpServerTool(Name = "sound_remove", Title = "Remove a sound definition", ReadOnly = false, Destructive = true, Idempotent = false, OpenWorld = false)]
    [Description("Remove a sound definition from the map's sound catalog and save the edited map to out_path. The input map is NEVER modified in place.")]
    public static SoundToolResult SoundRemove(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Sound definition name to remove.")] string name,
        [Description("State the intent to destroy data. Without it the call is refused with CONFIRM_REQUIRED, naming what would be removed.")] bool confirm = false,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => RunConfirmed(confirm, "removes sound definition '" + name + "'",
            () => SaveSound(map, out_path, overwrite, expect_input_sha256, doc => SoundCommand.Remove(doc, name)));

    [McpServerTool(Name = "sound_optimize", Title = "Shrink a sound file", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Shrink a sound file by decoding it, resampling to a target rate (default 22050 Hz), optionally setting the channel count and encoding it. The format comes from out_path's extension, .mp3 selects MP3 and anything else WAV. The input is never modified. Reads .wav natively and .mp3 through the Windows codecs. When no MP3 encoder is present the command writes WAV and reports a note rather than failing. Returns the before and after format, sample rate, channel count and byte size.")]
    public static object SoundOptimize(
        [Description("Path to a .wav or .mp3 sound file.")] string sound,
        [Description("Output path. Its extension picks the format, .mp3 for MP3, anything else for WAV. The input is never modified.")] string out_path,
        [Description("Target sample rate in Hz. Default 22050.")] int rate = SoundOptimizeCommand.DefaultRate,
        [Description("Output channels, source, mono or stereo. Default source keeps the source channel count.")] string channels = "source",
        [Description("MP3 bitrate in kbps, default 128. Applies to .mp3 output only, ignored for WAV.")] int bitrate = SoundOptimizeCommand.DefaultBitrateKbps,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run<object>(() => WriteWith(out_path, Inputs(sound), overwrite, expect_input_sha256,
            temp => SoundOptimizeCommand.ExecutePath(
                sound, rate, SoundOptimizeCommand.ParseChannels(channels), bitrate, temp), "path"));

    // ---- camera catalog (war3map.w3c) ------------------------------------

    // Compile-time mirror of CameraCommand.EditableFields (attribute args must be const).
    private const string CameraFieldValues =
        "Name|TargetX|TargetY|ZOffset|Rotation|AngleOfAttack|TargetDistance|Roll|" +
        "FieldOfView|FarClippingPlane|NearClippingPlane|LocalPitch|LocalYaw|LocalRoll";

    [McpServerTool(Name = "camera_list", Title = "List cameras", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("List the map's cameras (war3map.w3c): each camera's name, target position, rotation, angle of attack, distance and field of view.")]
    public static CappedPage<CameraFields> CameraList(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => Page(Wc3Limit.Cap(CameraCommand.List(LoadMap(map)))));

    [McpServerTool(Name = "camera_add", Title = "Add a camera", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Add a camera at a target position and save the edited map to out_path. The input map is NEVER modified in place. Rejects a blank or duplicate name.")]
    public static EditToolResult CameraAdd(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Camera name (unique).")] string name,
        [Description("Camera target X (world coordinate).")] float target_x,
        [Description("Camera target Y (world coordinate).")] float target_y,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = CameraCommand.Add(doc, name, target_x, target_y);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "camera_set", Title = "Edit a camera", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Set a field on a camera and save the edited map to out_path. The input map is NEVER modified in place. Editable fields: " + CameraFieldValues + ".")]
    public static EditToolResult CameraSet(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Camera name to edit.")] string name,
        [Description("Field to set: " + CameraFieldValues + ".")] string field,
        [Description("New value (parsed to the field's type).")] string value,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = CameraCommand.Set(doc, name, field, value);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "camera_remove", Title = "Remove a camera", ReadOnly = false, Destructive = true, Idempotent = false, OpenWorld = false)]
    [Description("Remove a camera and save the edited map to out_path. The input map is NEVER modified in place.")]
    public static EditToolResult CameraRemove(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Camera name to remove.")] string name,
        [Description("State the intent to destroy data. Without it the call is refused with CONFIRM_REQUIRED, naming what would be removed.")] bool confirm = false,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => RunConfirmed(confirm, "removes camera '" + name + "'", () => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = CameraCommand.Remove(doc, name);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "camera_tour", Title = "Build a camera tour", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Add N cameras interpolated between two existing cameras as a smooth tour path, and save the edited map to out_path. Every float field is blended linearly and the generated cameras sit strictly between the endpoints, named <from>-tour-i. The input map is NEVER modified in place.")]
    public static CameraTourToolResult CameraTour(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder. The file is read-only and the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Name of the camera the tour starts at.")] string from,
        [Description("Name of the camera the tour ends at.")] string to,
        [Description("How many cameras to interpolate between the two. Default 3.")] int count = 3,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run(() =>
        {
            var doc = LoadMap(map);
            var r = CameraCommand.Tour(doc, from, to, count);
            if (!r.Ok) throw Rejected(r.Message);
            string full = ResolveOutPath(map, out_path);
            var w = SaveDocAt(doc, full, map, overwrite, expect_input_sha256);
            return new CameraTourToolResult(w.SavedTo, r.Message, r.Names, r.Count,
                w.InputSha256, w.OutputSha256, w.FilesRewritten, w.BytesWritten);
        });

    // ---- pathing map (war3map.wpm) ---------------------------------------

    private const string PathingFlagValues = "Walk|Fly|Build|Blight|Water";

    [McpServerTool(Name = "pathing_paint", Title = "Paint pathing bits", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Paint pathing bits over a circular or square brush and save the edited map to out_path. The input map is NEVER modified in place. Flags (comma-separated): " + PathingFlagValues + " - a SET bit RESTRICTS that capability (Walk set = ground units cannot walk there). op: set|clear|toggle.")]
    public static EditToolResult PathingPaint(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Brush centre pathing-cell X (4 cells per terrain tile).")] int center_x,
        [Description("Brush centre pathing-cell Y.")] int center_y,
        [Description("Brush radius in pathing cells (>= 0).")] int radius,
        [Description("Comma-separated pathing bits: " + PathingFlagValues + ".")] string flags,
        [Description("How the bits combine: set|clear|toggle. Default set.")] string op = "set",
        [Description("Brush footprint: circle|square. Default circle.")] string shape = "circle",
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var flagText = (flags ?? "").Replace(" ", "");
            if (!Enum.TryParse<War3Net.Build.Environment.PathingType>(flagText, ignoreCase: true, out var parsed))
                throw Invalid($"invalid pathing flag(s) '{flags}'; expected a comma-separated subset of: " + PathingFlagValues, flags);
            var r = PathingCommand.Paint(doc, center_x, center_y, radius, parsed,
                ParseEnum<PathingCommand.BrushOp>(op, "op"),
                ParseEnum<PathingCommand.BrushShape>(shape, "shape"));
            return (r.Ok, r.Ok ? $"{r.Message} ({r.CellsChanged} cells)" : r.Message);
        }));

    [McpServerTool(Name = "pathing_export_png", Title = "Export pathing as PNG", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Render a map's war3map.wpm pathing grid to a colour-coded PNG at out_path, one pixel per pathing cell with a legend strip of the pathing flags. The input map is never modified. The same file re-imports through pathing_import_png.")]
    public static PathingExportToolResult PathingExportPng(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Output PNG path. Parent directories are created as needed.")] string out_path,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run(() =>
        {
            var r = PathingExportPngCommand.Export(LoadMap(map));
            if (!r.Ok || r.Png is null) throw Rejected(r.Message);
            var w = WriteGate.WriteBytes(ResolveOutPath(map, out_path), r.Png, Inputs(map),
                overwrite, expect_input_sha256);
            return new PathingExportToolResult(w.SavedTo, r.Message, r.Width, r.Height, r.Png.Length,
                r.Legend ?? [], w.InputSha256, w.OutputSha256, w.FilesRewritten, w.BytesWritten);
        });

    [McpServerTool(Name = "pathing_import_png", Title = "Import pathing from PNG", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Set the pathing flags from a colour-coded PNG and save the edited map to out_path. Each cell's flags are replaced by the value its pixel decodes to. The image must be at least the map's Width by Height cell grid (the top-left region is read, so a legend to the right is ignored) and any unknown colour is refused. The input map is NEVER modified in place.")]
    public static EditToolResult PathingImportPng(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Path to the colour-coded pathing PNG, as produced by pathing_export_png.")] string png,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = PathingImportPngCommand.Import(doc, ReadPngFile(png));
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "pathing_validate", Title = "Validate pathing", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Flag pathing problems in a map's war3map.wpm. It reports a pocket of unpathable tiles fully enclosed by pathable ones, a start location sitting on an unpathable tile, and a map corner that is unpathable or sealed off from the main walkable area. Read-only.")]
    public static PathingValidateResult PathingValidate(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => PathingCommand.Validate(LoadMap(map)));

    // ---- scenario: map-info, players, forces (war3map.w3i) ---------------

    // Compile-time mirror of MapInfoCommand.EditableFields (attribute args must be const).
    private const string MapInfoFieldValues =
        "MapName|Author|Description|RecommendedPlayers|Tileset|LightEnvironment|GlobalWeather|" +
        "SoundEnvironment|WaterTintColor|FogStyle|FogStartZ|FogEndZ|FogDensity|FogColor";

    [McpServerTool(Name = "map_info_get", Title = "Read scenario fields", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Read the map's editable scenario fields (war3map.w3i): name, author, description, recommended players, tileset, environment, fog and the map-option flags. Richer than map_info; this is the field set map_info_set edits.")]
    public static MapInfoFields MapInfoGet(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => MapInfoCommand.Read(LoadMap(map)));

    [McpServerTool(Name = "map_info_set", Title = "Set a scenario field", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Set a scenario field and save the edited map to out_path. The input map is NEVER modified in place. Editable fields: " + MapInfoFieldValues + " (and the map-option booleans).")]
    public static EditToolResult MapInfoSet(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Field to set: " + MapInfoFieldValues + ".")] string field,
        [Description("New value (parsed to the field's type).")] string value,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            MapInfoCommand.Set(doc, field, value); // throws on bad field -> clean MCP error via Run
            return (true, $"set {field} = {value}");
        }));

    [McpServerTool(Name = "player_list", Title = "List player slots", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("List the map's player slots (war3map.w3i): each player's id, name, color, race, controller and whether its start position is fixed.")]
    public static CappedPage<PlayerInfo> PlayerList(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => Page(Wc3Limit.Cap(PlayerForceCommand.GetPlayers(LoadMap(map)))));

    [McpServerTool(Name = "player_set_force", Title = "Move a player to a force", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Move a player into a force (team) and save the edited map to out_path. The input map is NEVER modified in place.")]
    public static EditToolResult PlayerSetForce(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Player id (0-based).")] int player_id,
        [Description("Force index (0-based).")] int force_index,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = PlayerForceCommand.SetPlayerForce(doc, player_id, force_index);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "force_list", Title = "List forces", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("List the map's forces/teams (war3map.w3i): each force's index, name, member player ids and alliance/sharing flags.")]
    public static CappedPage<ForceInfo> ForceList(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => Page(Wc3Limit.Cap(PlayerForceCommand.GetForces(LoadMap(map)))));

    [McpServerTool(Name = "force_set_flags", Title = "Set force alliance flags", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Set a force's alliance/sharing flags and save the edited map to out_path. The input map is NEVER modified in place.")]
    public static EditToolResult ForceSetFlags(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Force index (0-based).")] int force_index,
        [Description("Allied.")] bool allied,
        [Description("Allied victory.")] bool allied_victory,
        [Description("Shared vision.")] bool shared_vision,
        [Description("Shared unit control.")] bool shared_unit_control,
        [Description("Shared advanced unit control.")] bool shared_adv_unit_control,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = PlayerForceCommand.SetForceFlags(doc, force_index, allied, allied_victory,
                shared_vision, shared_unit_control, shared_adv_unit_control);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "player_set_name", Title = "Set a player name", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Set a player's display name (war3map.w3i) and save the edited map to out_path. The input map is NEVER modified in place.")]
    public static EditToolResult PlayerSetName(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Player id (0-based).")] int player_id,
        [Description("New display name.")] string name,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = PlayerSetNameCommand.Execute(doc, player_id, name);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "player_set_color", Title = "Set a player colour", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Set a player's team color and save the edited map to out_path. war3map.w3i has no per-player color field, so this rewrites the SetPlayerColor call the map script already carries. The input map is NEVER modified in place.")]
    public static EditToolResult PlayerSetColor(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Player id (0-based).")] int player_id,
        [Description("Team color name, for example red, lightblue or darkgreen.")] string color,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = PlayerSetColorCommand.Execute(doc, player_id, color);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "player_set_race", Title = "Set a player race", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Set a player's race (war3map.w3i) and save the edited map to out_path. Accepted values are human, orc, undead, nightelf and random. The input map is NEVER modified in place.")]
    public static EditToolResult PlayerSetRace(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Player id (0-based).")] int player_id,
        [Description("One of human, orc, undead, nightelf or random.")] string race,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = PlayerSetRaceCommand.Execute(doc, player_id, race);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "force_rename", Title = "Rename a force", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Rename a force (team) in war3map.w3i and save the edited map to out_path. The input map is NEVER modified in place.")]
    public static EditToolResult ForceRename(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Force index (0-based).")] int force_index,
        [Description("New force name.")] string name,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = ForceRenameCommand.Execute(doc, force_index, name);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "force_set_players", Title = "Add or remove force members", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Add or remove a player from a force (team) in war3map.w3i and save the edited map to out_path. Pass exactly one of add or remove. Adding clears the player from every other force so the masks stay consistent. The input map is NEVER modified in place.")]
    public static EditToolResult ForceSetPlayers(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Force index (0-based).")] int force_index,
        [Description("Add this player id to the force. Pass exactly one of add or remove.")] int? add = null,
        [Description("Remove this player id from the force. Pass exactly one of add or remove.")] int? remove = null,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            if ((add is null) == (remove is null))
                return (false, "pass exactly one of add or remove");
            var r = add is not null
                ? ForceSetPlayersCommand.Add(doc, force_index, add.Value)
                : ForceSetPlayersCommand.Remove(doc, force_index, remove!.Value);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "new_map", Title = "Create a blank map", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Create a blank, World-Editor-openable map (.w3x) at out_path. No input map. Returns where it was written.")]
    public static EditToolResult NewMap(
        [Description("Output map file path to create.")] string out_path,
        [Description("Map name. Default 'Blank Map'.")] string? name = null,
        [Description("Playable size in tiles per edge. Default 32.")] int? tiles = null,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run(() =>
        {
            if (string.IsNullOrWhiteSpace(out_path)) throw Missing("out_path");
            var opts = new BlankMapOptions();
            if (!string.IsNullOrEmpty(name)) opts = opts with { MapName = name! };
            if (tiles is { } t) opts = opts with { TileEdge = t };
            byte[] bytes;
            try { bytes = BlankMap.CreateArchiveBytes(opts); }
            catch (Exception ex) { throw Failed(ex); }
            var w = WriteGate.WriteBytes(out_path, bytes, Array.Empty<string>(), overwrite, expect_input_sha256);
            return new EditToolResult(w.SavedTo,
                $"created \"{opts.MapName}\" ({opts.TileEdge}x{opts.TileEdge} tiles, {bytes.Length} bytes)",
                w.InputSha256, w.OutputSha256, w.FilesRewritten, w.BytesWritten);
        });

    [McpServerTool(Name = "region_list", Title = "List regions", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("List the map's rectangular regions (war3map.w3r), each with its creation number, name, and bounds. Add regions with place_region.")]
    public static CappedPage<PlacementCommand.RegionInfo> RegionList(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => Page(Wc3Limit.Cap(PlacementCommand.ListRegions(LoadMap(map)))));

    [McpServerTool(Name = "region_remove", Title = "Remove a region", ReadOnly = false, Destructive = true, Idempotent = false, OpenWorld = false)]
    [Description("Remove a region by name and save the edited map to out_path. The input map is NEVER modified in place.")]
    public static EditToolResult RegionRemove(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Region name to remove.")] string name,
        [Description("State the intent to destroy data. Without it the call is refused with CONFIRM_REQUIRED, naming what would be removed.")] bool confirm = false,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => RunConfirmed(confirm, "removes region '" + name + "'", () => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = PlacementCommand.RemoveRegion(doc, name);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "region_create", Title = "Create a region from a corner", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Create a rectangular region from its south-west corner (x, y) plus a width w and height h, and save the edited map to out_path. The input map is NEVER modified in place. Rejects a blank or duplicate name and a non-positive size. Set weather to attach the default weather effect. Returns the assigned creation number.")]
    public static PlacementToolResult RegionCreate(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder. The file is read-only and the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). Created or overwritten, and parent directories are created as needed. To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Region name (non-empty, unique).")] string name,
        [Description("South-west corner X (world coordinate).")] float x,
        [Description("South-west corner Y (world coordinate).")] float y,
        [Description("Region width in world units (must be > 0).")] float w,
        [Description("Region height in world units (must be > 0).")] float h,
        [Description("Attach the default weather effect to the region. Default false.")] bool weather = false,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SavePlacement(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = RegionCommand.Create(doc, name, x, y, w, h, weather);
            return (r.Ok, r.Message, r.CreationNumber);
        }));

    [McpServerTool(Name = "region_to_rect", Title = "Resolve a region to a rect", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Resolve one named region to the four corner values a JASS trigger's Rect call needs (left, bottom, right, top), with a ready-to-copy Rect(...) line. Read-only.")]
    public static RegionRectResult RegionToRect(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Region name to convert.")] string region)
        => Run(() => RegionCommand.ToRect(LoadMap(map), region));

    [McpServerTool(Name = "region_validate", Title = "Validate regions", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Flag overlapping regions, zero-area regions and regions that fall outside the playable bounds in a map's war3map.w3r. Regions sharing only an edge do not overlap. Read-only.")]
    public static RegionValidateResult RegionValidate(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => RegionCommand.Validate(LoadMap(map)));

    [McpServerTool(Name = "trigger_add_eca", Title = "Add a trigger line", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Add an event, condition or action to a GUI trigger and save the edited map to out_path. Parameters follow the order the function declares, and any left off are filled from the World Editor table's own defaults. war3map.wtg stores a function's parameters but not how many there are, so a line written with the wrong count yields a map nothing can read afterwards. Custom-text triggers are refused. This edits the World Editor's trigger source, not the compiled script the game runs, so the line is inert until the map is opened and saved there.")]
    public static EditToolResult TriggerAddEca(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Trigger item id, from triggers_read.")] int id,
        [Description("One of: event, condition, action.")] string kind,
        [Description("Function name from the World-Editor table, for example DisplayTextToForce.")] string name,
        [Description("Parameter values in declared order. Omit to take the table's defaults. At most 1000, and a longer list is refused with a tool error naming the limit.")] string[]? parameters = null,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() =>
        {
            RequireAtMost(parameters?.Length ?? 0, "parameters");
            return SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
            {
                if (!TryParseEcaKind(kind, out var parsed))
                    return (false, "kind must be event, condition or action.");
                var r = TriggerCommand.AddFunction(doc, id, parsed, name, parameters);
                return (r.Ok, r.Message);
            });
        });

    [McpServerTool(Name = "trigger_remove_eca", Title = "Remove a trigger line", ReadOnly = false, Destructive = true, Idempotent = false, OpenWorld = false)]
    [Description("Remove one event, condition or action from a GUI trigger by its zero-based position, and save the edited map to out_path. Positions come from triggers_read, which lists a trigger's functions in order. The input map is NEVER modified in place.")]
    public static EditToolResult TriggerRemoveEca(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Trigger item id, from triggers_read.")] int id,
        [Description("Zero-based position of the function within the trigger.")] int index,
        [Description("State the intent to destroy data. Without it the call is refused with CONFIRM_REQUIRED, naming what would be removed.")] bool confirm = false,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => RunConfirmed(confirm, "removes function " + index + " from trigger " + id + "", () => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = TriggerCommand.RemoveFunction(doc, id, index);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "trigger_set_eca_enabled", Title = "Toggle one trigger line", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Enable or disable ONE event, condition or action within a trigger, the World Editor's per-line toggle rather than the whole-trigger one, and save the edited map to out_path. The input map is NEVER modified in place.")]
    public static EditToolResult TriggerSetEcaEnabled(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Trigger item id, from triggers_read.")] int id,
        [Description("Zero-based position of the function within the trigger.")] int index,
        [Description("True to enable, false to disable.")] bool on,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = TriggerCommand.SetFunctionEnabled(doc, id, index, on);
            return (r.Ok, r.Message);
        }));

    private static bool TryParseEcaKind(string text, out War3Net.Build.Script.TriggerFunctionType kind)
    {
        switch ((text ?? string.Empty).Trim().ToLowerInvariant())
        {
            case "event": kind = War3Net.Build.Script.TriggerFunctionType.Event; return true;
            case "condition": kind = War3Net.Build.Script.TriggerFunctionType.Condition; return true;
            case "action": kind = War3Net.Build.Script.TriggerFunctionType.Action; return true;
            default: kind = default; return false;
        }
    }

    [McpServerTool(Name = "hero_lint", Title = "Lint a hero definition", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Validate a hero definition folder before installing it: assets present and hash-matched, objects installable, declared script functions present, no dangling references. Reads a definition and returns findings, it writes nothing. Run this before hero_install, because an install that fails halfway leaves a map holding some of a hero.")]
    public static HeroLintResult HeroLint(
        [Description("Path to a hero definition directory, the folder holding hero.json.")] string definition)
        => Run(() => HeroLintCommand.Run(definition));

    [McpServerTool(Name = "hero_install", Title = "Install a hero", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Install a hero definition into a map and save the result to out_path. Remaps rawcodes that would collide, writes the hero's assets, and reports what the target map must still be wired with before the hero actually works, since a definition carries objects and script but not the target's own integration (roster registration, spell dispatch, per-player hero arrays). Refuses asset collisions unless force is set. The input map is NEVER modified in place.")]
    public static EditToolResult HeroInstall(
        [Description("Path to a hero definition directory, the folder holding hero.json.")] string definition,
        [Description("Path to the target .w3x/.w3m map file, or to an unpacked map folder. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Overwrite target assets that collide with the definition's, instead of refusing.")] bool force = false,
        [Description("Warcraft III install directory, when the base game data is needed and auto-detection fails.")] string? game_dir = null,
        [Description("Role to install the hero as, when the definition supports more than one.")] string? role = null,
        [Description("Keep the source map's stat values rather than adapting them to the target.")] bool keep_source_stats = false,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = HeroInstallCommand.Run(definition, doc, force, game_dir, role, keep_source_stats);
            string detail = r.Ok
                ? $"{r.Message} {r.ObjectsCreated} object(s), {r.FieldsApplied} field(s), "
                  + $"{r.AssetsWritten} asset(s) written, {r.AssetsSkippedIdentical} identical and skipped."
                  + (r.UnmetRequirements.Count == 0
                     ? string.Empty
                     : " STILL UNWIRED: " + string.Join("; ", r.UnmetRequirements))
                  + (r.NextSteps.Count == 0
                     ? string.Empty
                     : " Next: " + string.Join("; ", r.NextSteps))
                : r.Message
                  + (r.Collisions.Count == 0
                     ? string.Empty
                     : " Collisions: " + string.Join("; ", r.Collisions.Take(8)));
            return (r.Ok, detail);
        }));

    [McpServerTool(Name = "hero_generate", Title = "Generate a multi-form hero", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Generate a multi-form hero from a JSON brief and save the result to out_path, with one unit object per form derived from one base unit, one ability object per slot carrying its own Base Order ID, cooldown, cast time and per level tooltip, each form's ability list in uabi, and a self contained JASS block with one transform function per form and one timer driven damage stub per damaging ability. The whole brief is validated before the first object is created, so a brief naming an unknown base is refused with nothing written.")]
    public static object HeroGenerate(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder, that holds the base unit and ability bases. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument.")] string out_path,
        [Description("The hero brief as a JSON string. Shape, { \"name\": \"Kurosaki Ichigo\", \"base_unit\": \"Hpal\", \"forms\": [ { \"id\": \"base\", \"abilities\": [ { \"slot\": \"Q\", \"name\": \"Getsuga Tenshou\", \"base\": \"ANcl\", \"cooldown\": [8,8,8,8], \"cast\": 0.35, \"damage\": 120 }, { \"slot\": \"R\", \"name\": \"Bankai\", \"kind\": \"transform\", \"to\": \"bankai\" } ] }, { \"id\": \"bankai\", \"abilities\": [ ... ] } ] }. Every form id must be a JASS identifier, and a transform must name a form in the same brief.")] string spec_json,
        [Description("Warcraft III install directory, when the base unit or an ability base is not defined by the map itself and auto-detection fails.")] string? game_dir = null,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run<object>(() =>
        {
            string full = ResolveOutPath(map, out_path);
            return WriteWith(full, Inputs(map), overwrite, expect_input_sha256,
                temp => HeroGenerateCommand.Execute(map, temp, spec_json, game_dir), "path");
        });

    [McpServerTool(Name = "content_scaffold", Title = "Scaffold a content package", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Generate a playable content package into out_dir from a name alone: a body capsule model, a particle effect model, a 64 by 64 icon, a short 16 bit PCM impact sound, and a role manifest (package.json). Every file is loadable rather than a stub, and both models name the package's own icon, so the package has no dependency on art outside itself. Gameplay binds a role such as hero.alpha.body, never a filename, so replacing the placeholder with final art is one binding change. Returns the root, the name, every file with its role and size, and the manifest path.")]
    public static object ContentScaffold(
        [Description("Package name. Reduced to letters, digits, underscores and hyphens for file and role names, which is reported as a diagnostic when that changes anything.")] string name,
        [Description("Folder to write the package into. Created when it does not exist.")] string out_dir,
        [Description("Texture map path both models name, for example war3mapImported\\glow.blp. Omit to use the package's own icon.")] string? texture_path = null,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run<object>(() =>
        {
            ContentScaffoldResult? r = null;
            var w = WriteGate.WriteFolder(out_dir, Array.Empty<string>(), overwrite, expect_input_sha256,
                temp => r = ContentScaffoldCommand.Execute(name, temp, texture_path));
            var gated = Gated(r!, w);
            gated["root"] = w.SavedTo;
            gated["files"] = r!.Files
                .Select(f => f with { Path = Path.Combine(w.SavedTo, Path.GetFileName(f.Path)) })
                .ToList();
            gated["manifestPath"] = Path.Combine(w.SavedTo, ContentScaffoldCommand.ManifestFileName);
            return gated;
        });

    [McpServerTool(Name = "repair_generated", Title = "Repair a generated map", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Repair a map that wc3ctl itself generated, whose preplaced-hero helper block is too narrow for the heroes installed into it, and save the repaired map to out_path. Widens the player slots, rewrites hero owners in the script, and brings war3map.w3i and the map header into line. This is NOT a general map repair: it looks for wc3ctl's own generated helper function and refuses any map that does not carry one. The input map is NEVER modified in place.")]
    public static EditToolResult RepairGenerated(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only; the repaired copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = GeneratedMapRepairCommand.RepairGeneratedHeroes(doc);
            return (r.Ok, r.Ok
                ? $"{r.Message} Player slots {r.PlayerSlotsBefore} to {r.PlayerSlotsAfter}, "
                  + $"{r.Heroes.Count} hero(es) reassigned."
                : r.Message);
        }));

    [McpServerTool(Name = "repair_portraits", Title = "Fix portrait rendering", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Remove the pre-900 camera chunk from every at-risk model so the portrait pane renders under 3.0.0 and later, and save the repaired map to out_path. Camera removal only. The version re-stamp is refused because it was measured to crash the map on load. The input map is NEVER modified in place.")]
    public static EditToolResult RepairPortraits(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the repaired copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = PortraitRepairCommand.Execute(doc, apply: true);
            return (r.ModelsRepaired > 0 || r.ModelsAtRisk == 0,
                $"{r.ModelsRepaired} of {r.ModelsAtRisk} at-risk model(s) repaired.");
        }));

    [McpServerTool(Name = "repair_audit_errors", Title = "Repair audited defects", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Repair the object-data defects audit reports. Fills ability level gaps (a field with no value at a declared level, copied from the nearest valid level), clears dangling references and impossible requirements, and saves to out_path. The input map is NEVER modified in place.")]
    public static EditToolResult RepairAuditErrors(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the repaired copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = AuditRepairCommand.Execute(doc, apply: true);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "terrain_fill", Title = "Fill a terrain rectangle", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Apply a terrain tool over an inclusive rectangle of corners and save the edited map to out_path, instead of one brush dab at a time. Corners may be given in any order and a rectangle that runs off the grid is clipped rather than refused. tool is one of Raise, Lower, SetHeight, Flatten, Paint, CliffRaise, CliffLower, CliffSet, Ramp, RampOff, Water, WaterRemove, Blight, BlightOff. value is a height for Raise, Lower, SetHeight and Water, a tile index for Paint (see terrain_info), and a step count for the cliff tools, and is ignored by Flatten, Ramp, RampOff, WaterRemove, Blight and BlightOff.")]
    public static EditToolResult TerrainFill(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("First corner column, 0-based.")] int x0,
        [Description("First corner row, 0-based.")] int y0,
        [Description("Second corner column, 0-based and inclusive.")] int x1,
        [Description("Second corner row, 0-based and inclusive.")] int y1,
        [Description("Raise, Lower, SetHeight, Flatten, Paint, CliffRaise, CliffLower, CliffSet, Ramp, RampOff, Water, WaterRemove, Blight or BlightOff.")] string tool,
        [Description("Height, ground tile index or cliff step count, depending on the tool. Defaults to 1.")] float value = 1f,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            if (!Enum.TryParse<TerrainFillCommand.FillTool>(tool, ignoreCase: true, out var parsed))
                return (false, $"unknown tool '{tool}'. Valid tools are "
                             + string.Join(", ", Enum.GetNames<TerrainFillCommand.FillTool>()) + ".");
            var r = TerrainFillCommand.Fill(doc, x0, y0, x1, y1, parsed, value);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "trigger_recover_from_script", Title = "Rebuild a trigger tree from script", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Rebuild a browsable GUI trigger tree (war3map.wtg) for a map that has none, by decompiling the script the game runs, and save it to out_path. It only ever adds a tree and leaves the script alone. A map that already carries a tree is refused, because a decompiled tree cannot replace a real one, the decompiler reported success on a map holding 626 trigger items while producing 1 at zero name recall. Use it on protected or stripped maps whose tree was removed but whose script survives. The tree describes the script and does not drive it.")]
    public static EditToolResult TriggerRecoverFromScript(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only; the result is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = TriggerRecoverCommand.Recover(doc);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "trigger_add_category", Title = "Add a trigger category", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Add a category to the map's GUI trigger tree and save the edited map to out_path. A category is a folder in the trigger tree, so this is always safe: categories hold no script and adding one moves nothing. The input map is NEVER modified in place. Writes war3map.wtg only.")]
    public static EditToolResult TriggerAddCategory(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Name for the new category.")] string name,
        [Description("Id of the item to nest it under, or -1 for the top level. Ids come from triggers_read.")] int parent_id = -1,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = TriggerCommand.AddCategory(doc, name, parent_id);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "trigger_add", Title = "Add a trigger", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Add a trigger to a category in the map's GUI trigger tree and save the edited map to out_path. The new trigger is created enabled and initially on, matching the World Editor, with no events, conditions or actions yet. This edits war3map.wtg, the World Editor's trigger source, not the compiled war3map.j the game runs, so the trigger is inert until the map is opened and saved in the World Editor. A custom-text trigger cannot be added, since its body lives in war3map.wct. The input map is never modified in place.")]
    public static EditToolResult TriggerAdd(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Name for the new trigger.")] string name,
        [Description("Id of the category to add it to, or -1 for the top level. Ids come from triggers_read.")] int parent_id,
        [Description("Create a comment rather than a GUI trigger.")] bool comment = false,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = TriggerCommand.AddTrigger(doc, name, parent_id,
                comment ? TriggerCommand.NewTriggerKind.Comment : TriggerCommand.NewTriggerKind.Gui);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "trigger_remove", Title = "Remove a trigger item", ReadOnly = false, Destructive = true, Idempotent = false, OpenWorld = false)]
    [Description("Remove a trigger item (a category, a trigger or a comment) from the GUI trigger tree and save the edited map to out_path. Refuses rather than risk two silent corruptions: removing a category that still holds children would orphan them (pass recursive to remove the subtree instead), and removing a trigger whose script body sits ahead of other bodies in war3map.wct would shift every later body onto the wrong trigger, since that file cannot be rewritten. The input map is NEVER modified in place.")]
    public static EditToolResult TriggerRemove(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Trigger item id, from triggers_read.")] int id,
        [Description("Remove the item's descendants too, rather than refusing to orphan them.")] bool recursive = false,
        [Description("State the intent to destroy data. Without it the call is refused with CONFIRM_REQUIRED, naming what would be removed.")] bool confirm = false,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => RunConfirmed(confirm, "removes trigger item " + id + (recursive ? " and its descendants" : "") + "", () => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = TriggerCommand.Remove(doc, id, recursive);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "trigger_rename", Title = "Rename a trigger item", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Rename a trigger item (a category, a trigger or a deleted stub) in the GUI trigger tree and save the edited map to out_path. The input map is NEVER modified in place. Ids come from triggers_read. Writes war3map.wtg only.")]
    public static EditToolResult TriggerRename(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Trigger item id, from triggers_read.")] int id,
        [Description("New name.")] string name,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = TriggerCommand.Rename(doc, id, name);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "trigger_move", Title = "Move a trigger item", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Move a trigger item under a parent category in the GUI trigger tree and save the edited map to out_path. This is how you organise the tree into folders. Only ParentId changes, so document order and every war3map.wct code body stay put, and a category cannot be moved inside its own subtree. The input map is NEVER modified in place.")]
    public static EditToolResult TriggerMove(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument.")] string out_path,
        [Description("Trigger item id, from triggers_read.")] int id,
        [Description("Parent category id, or -1 for the root.")] int parent,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = TriggerCommand.Move(doc, id, parent);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "trigger_set_enabled", Title = "Enable or disable a trigger", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Enable or disable a GUI trigger and save the edited map to out_path. A disabled trigger is not compiled into the map script at all, so this is how you take one out of play without deleting it. The input map is NEVER modified in place.")]
    public static EditToolResult TriggerSetEnabled(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Trigger item id, from triggers_read.")] int id,
        [Description("True to enable, false to disable.")] bool on,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = TriggerCommand.SetEnabled(doc, id, on);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "trigger_set_initially_on", Title = "Set a trigger's initial state", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Set whether a GUI trigger starts switched on, and save the edited map to out_path. A trigger that is off at map start never fires until something turns it on, which is a common reason a trigger looks correct and does nothing. The input map is NEVER modified in place.")]
    public static EditToolResult TriggerSetInitiallyOn(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Trigger item id, from triggers_read.")] int id,
        [Description("True to start on, false to start off.")] bool on,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = TriggerCommand.SetInitiallyOn(doc, id, on);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "trigger_set_run_on_map_init", Title = "Set a trigger to run on init", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Set whether a GUI trigger runs on map initialization, and save the edited map to out_path. The input map is NEVER modified in place.")]
    public static EditToolResult TriggerSetRunOnMapInit(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk and writes a new file, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Trigger item id, from triggers_read.")] int id,
        [Description("True to run on map init.")] bool on,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = TriggerCommand.SetRunOnMapInit(doc, id, on);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "trigger_sync_bodies", Title = "Sync trigger bodies", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Reconcile war3map.wct with the war3map.wtg trigger tree and save the edited map to out_path. A map can arrive with more code bodies than the tree accounts for (a protected or ported build keeps orphaned bodies), or fewer, and the World Editor then cannot trust which body belongs to which trigger and refuses some trigger removals. This drops the orphaned tail or appends empty slots, copying every surviving byte verbatim. It is the repair for the trigger-bodies lint warning. The input map is NEVER modified in place.")]
    public static EditToolResult TriggerSyncBodies(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map).")] string out_path,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = TriggerCommand.SyncBodies(doc);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "trigger_var_list", Title = "List global variables", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("List the map's global variables (the .wtg Variables list), each with its name, type, array flag, declared size and initial value. Read-only.")]
    public static CappedPage<TriggerVariableFields> TriggerVarList(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map)
        => Run(() => Page(Wc3Limit.Cap(TriggerCommand.ListVariables(LoadMap(map)))));

    [McpServerTool(Name = "trigger_var_add", Title = "Add a global variable", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Add a global variable to the map's wtg and save the edited map to out_path. Provides a name, a type (for example integer, real, string, boolean, unit, group, trigger) and, for an array, the array flag and size. An initial value marks the variable initialized. Refuses a blank name or type and a name that already exists, because the wtg keys variables by name. IMPORTANT: this edits war3map.wtg, the World Editor's trigger SOURCE, not war3map.j, so a variable it defines is visible in the World Editor and inert in game until that editor saves the map. The input map is NEVER modified in place.")]
    public static EditToolResult TriggerVarAdd(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Variable name.")] string name,
        [Description("Variable type, for example integer, real, string, boolean, unit, group or trigger.")] string type,
        [Description("Declare it as an array. Default false.")] bool is_array = false,
        [Description("Array size when is_array is true. Default 1.")] int array_size = 1,
        [Description("Initial value. Providing one marks the variable initialized. Omit to leave it uninitialized.")] string? initial_value = null,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = TriggerCommand.AddVariable(doc, name, type, is_array, array_size, initial_value);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "trigger_var_set", Title = "Update a global variable", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Update an existing global variable in the map's wtg by name and save the edited map to out_path. Any argument left off keeps its current value, so one field can change without restating the rest. This does not rewrite references to the variable inside trigger bodies. The input map is NEVER modified in place.")]
    public static EditToolResult TriggerVarSet(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Name of the variable to update.")] string name,
        [Description("New type. Omit to keep the current one.")] string? type = null,
        [Description("New array flag. Omit to keep the current one.")] bool? is_array = null,
        [Description("New array size. Omit to keep the current one.")] int? array_size = null,
        [Description("New initial value, which also marks the variable initialized. Omit to keep the current one.")] string? initial_value = null,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = TriggerCommand.SetVariable(doc, name, type, is_array, array_size, initial_value);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "trigger_var_remove", Title = "Remove a global variable", ReadOnly = false, Destructive = true, Idempotent = false, OpenWorld = false)]
    [Description("Remove a global variable from the map's wtg by name and save the edited map to out_path. Any trigger that referenced the variable is left pointing at nothing, so check references afterwards. The input map is NEVER modified in place.")]
    public static EditToolResult TriggerVarRemove(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Name of the variable to remove.")] string name,
        [Description("State the intent to destroy data. Without it the call is refused with CONFIRM_REQUIRED, naming what would be removed.")] bool confirm = false,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => RunConfirmed(confirm, "removes trigger variable '" + name + "'", () => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = TriggerCommand.RemoveVariable(doc, name);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "trigger_set_code", Title = "Set a trigger's custom code", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Set the custom-text JASS body of a custom-text trigger and save the edited map to out_path. Only that trigger's slot in war3map.wct is spliced, so every other slot's bytes survive verbatim, which is why this one write into war3map.wct is safe where the rest of the trigger surface refuses it. A GUI trigger is refused. This edits the World Editor's trigger source, not the compiled war3map.j the game runs, so the body is inert until the map is opened and saved in the World Editor.")]
    public static EditToolResult TriggerSetCode(
        [Description("Path to the source .w3x/.w3m map file, or to an unpacked map folder holding the same entries. Read-only, the edited copy is written to out_path.")] string map,
        [Description("Output map file path (must differ from the input map). To make SEVERAL edits, pass this file as the next call's map argument. Each tool loads the map fresh from disk, so two edits both reading the ORIGINAL map produce two separate outputs and the first edit is lost.")] string out_path,
        [Description("Trigger item id of the custom-text trigger, from triggers_read.")] int id,
        [Description("The JASS body to store in this trigger's war3map.wct slot.")] string code,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null)
        => Run(() => SaveEdit(map, out_path, overwrite, expect_input_sha256, doc =>
        {
            var r = TriggerCommand.SetCustomText(doc, id, code);
            return (r.Ok, r.Message);
        }));

    [McpServerTool(Name = "trigger_catalog_list", Title = "List trigger functions", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("List GUI-trigger functions from the World-Editor catalog (UI\\TriggerData.txt), optionally filtered by kind and/or a name/display-name search. The catalog is read from an explicit file when given, otherwise from the installed game.")]
    public static TriggerCatalogListResult TriggerCatalogList(
        [Description("Path to an explicit TriggerData.txt. When omitted, the catalog is read from the installed game.")] string? file = null,
        [Description("Warcraft III install directory. When omitted, uses the WC3_GAME_DIR env var, else auto-detects.")] string? game_dir = null,
        [Description("Filter by kind: event|condition|action|call. Optional.")] string? kind = null,
        [Description("Case-insensitive substring filter over function name and display name. Optional.")] string? search = null)
        => Run(() =>
        {
            var catalog = TriggerCatalogCommand.Load(file, ResolveGameDir(game_dir), out var source);
            return TriggerCatalogCommand.List(catalog, source, kind, search);
        });

    [McpServerTool(Name = "trigger_catalog_describe", Title = "Describe a trigger function", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Show full detail for one GUI-trigger function by name (e.g. 'DoNothing'): kind, game version, return type, argument types, display name, parameters layout and defaults. The catalog is read from an explicit file when given, otherwise from the installed game.")]
    public static TriggerCatalogDetail TriggerCatalogDescribe(
        [Description("Function name (e.g. DoNothing).")] string name,
        [Description("Path to an explicit TriggerData.txt. When omitted, the catalog is read from the installed game.")] string? file = null,
        [Description("Warcraft III install directory. When omitted, uses the WC3_GAME_DIR env var, else auto-detects.")] string? game_dir = null)
        => Run(() =>
        {
            var catalog = TriggerCatalogCommand.Load(file, ResolveGameDir(game_dir), out _);
            return TriggerCatalogCommand.Describe(catalog, name)
                ?? throw Wc3Errors.Refuse(Wc3ErrorCodes.ObjectNotFound, $"No such function: {name}", name);
        });

    [McpServerTool(Name = "editor_catalog_list", Title = "List editor catalogs", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("List the World Editor's catalog names from UI\\WorldEditData.txt with entry counts (TileSets, SkyModels, LoadingScreens, SoundChannels, MapSizes, the brush palettes and more). These are the catalogs the editor drives its own pickers from, ask here instead of inventing a token.")]
    public static EditorCatalogNamesResult EditorCatalogList(
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var).")] string? game_dir = null)
        => Run(() => EditorCatalogCommand.Names(ResolveGameDir(game_dir)));

    [McpServerTool(Name = "editor_catalog_get", Title = "Show one editor catalog", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Show one World Editor catalog's entries, each with its stored key (the token a map file actually carries), its resolved display name and every payload field. TileSets, for example, maps the letter A to Ashenvale plus its blight texture path.")]
    public static EditorCatalogListResult EditorCatalogGet(
        [Description("Catalog name, e.g. TileSets. Case insensitive, see editor_catalog_list for the names.")] string name,
        [Description("Warcraft III install directory (overrides auto-detection and the WC3_GAME_DIR env var).")] string? game_dir = null)
        => Run(() => EditorCatalogCommand.List(name, ResolveGameDir(game_dir)));

    // ---- Devo's Map Doctor, run as an external program --------------------

    private const string DoctorNote = " Runs Devo's Map Doctor (MIT, github.com/devoltzz/devos-map-doctor), "
        + "found through WC3_DOCTOR or %LOCALAPPDATA%\\wc3ctl\\doctor\\<version>\\, and only a pinned build. "
        + "Returns the doctor's own JSON unchanged.";

    [McpServerTool(Name = "doctor_check", Title = "Check a map for Reforged 3.0", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Will this map run on Reforged 3.0, and if not, why. A verdict plus findings, each with a "
        + "code, a severity (blocker, warning, info), the affected files or models, and whether the doctor "
        + "can fix it. Catches what stops a map loading that wc3ctl's own checks do not, such as half "
        + "written Buttonpos in SLK profile tables and models with more than 255 matrix groups. Never "
        + "starts the game." + DoctorNote)]
    public static MapDoctorResult DoctorCheck(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Check against an older game instead, 1.24a to 1.28.5 or 1.29.2, such as 1.27.")] string? game = null)
        => Run(() => MapDoctorCommand.Check(map, game));

    [McpServerTool(Name = "doctor_inspect", Title = "Inspect a map's health", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Read-only doctor reports on one map. diag is what protects the map and what the World "
        + "Editor needs, including whether it runs in SLK mode and the fix steps that apply. files lists "
        + "the archive's files with recovered names, card the name, author, players and loading screen, "
        + "rawcodes the object ids with names, recipes the item recipes the script makes." + DoctorNote)]
    public static MapDoctorResult DoctorInspect(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("One of diag, files, card, rawcodes, recipes.")] string report = "diag")
        => Run(() => MapDoctorCommand.ReadOnly(report, map));

    [McpServerTool(Name = "doctor_compare", Title = "Compare two map versions", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("What differs between two versions of a map, file by file (added, removed, changed) "
        + "with names recovered on protected archives, plus object, script and string summaries. The "
        + "fastest way to see what someone else's fix actually changed." + DoctorNote)]
    public static MapDoctorResult DoctorCompare(
        [Description("The first map, a .w3x/.w3m file or an unpacked map folder.")] string map,
        [Description("The map to compare it with.")] string other)
        => Run(() => MapDoctorCommand.Compare(map, other));

    [McpServerTool(Name = "doctor_fix", Title = "Fix a copy for 3.0", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false)]
    [Description("Repair a COPY of a map for Reforged 3.0 with the doctor's Fix map: protection, fake "
        + "file tables, scrambled ids, data the 3.0 engine refuses, broken SLK tables, crashing models, "
        + "portrait cameras, data pointers, ability lists. The original is never written and an existing "
        + "output is never overwritten. Run doctor_inspect (diag) first to see which steps apply." + DoctorNote)]
    public static MapDoctorResult DoctorFix(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Where to write the repaired copy. Default <map>_doctor.<ext> beside the input.")] string? out_path = null,
        [Description("Steps and extras, without dashes, such as listfile, data, portraits, uabi. Prefix no- to "
            + "drop a default step. Empty applies the steps the doctor picks for this map. Allowed: unprotect, "
            + "fake-files, listfile, ids, data, script-back, editor-files, counts, doodads, gui-triggers, "
            + "script-objects, safe-units, models, model-names, portraits, data-pointers, kk-textures, "
            + "green-icons, uabi, preload, single-player, shrink, strip-indent. At most 1000, and a longer list is refused with a tool error naming the limit.")] string[]? options = null,
        [Description("Replace an existing output file. Default false, and a rerun that would overwrite is refused with OUTPUT_EXISTS (SAF-05).")] bool overwrite = false,
        [Description("The SHA-256 of the input map as you last read it. A mismatch is refused with STALE_INPUT, so an edit cannot land on a file that changed underneath it (SAF-07).")] string? expect_input_sha256 = null,
        [Description("Cancels the call at the next check point. The repair itself is not interruptible once it starts.")] CancellationToken cancellationToken = default)
        => Run(() =>
        {
            RequireAtMost(options?.Length ?? 0, "options");
            CheckPoint(cancellationToken, "the repair start");
            var result = MapDoctorCommand.Fix(map, options, out_path);
            CheckPoint(cancellationToken, "the result");
            return result;
        });

    // ---- shared plumbing -------------------------------------------------

    /// <summary>Expected failures become clean MCP tool errors, never stack traces.</summary>
    /// <summary>
    /// SAF-10. A destructive call states its intent, and the refusal names what would be removed.
    /// The flag proves the caller included it, not that a person approved it, which is SAF-18.
    /// </summary>
    private static T RunConfirmed<T>(bool confirm, string what, Func<T> body) =>
        Run(() =>
        {
            if (!confirm)
            {
                throw new ToolError(Wc3ErrorCodes.ConfirmRequired.Code, what + ", pass confirm true",
                    "the input is never modified, the call still writes a new file, so the flag only states intent");
            }
            return body();
        });

    /// <summary>PAR-08. The most entries one array argument may carry. A longer list is refused
    /// with a tool error that names the limit, never truncated, so a caller never believes a
    /// complete list was applied when part of it was dropped.</summary>
    private const int MaxArrayItems = 1000;

    // ---- PRF-08. The four resource limits, in one place ------------------
    //
    // The corpus measurement these are set against is the largest map this toolkit loads,
    // ORDR_S2_2.323[R]_english.w3x, 7914 archive entries and 339 MB decompressed (see the
    // comment on MapFileEntry.RawBytes). Each limit sits a few times above that baseline, so a
    // real map and a bigger one both pass, and a decrypt bomb or a runaway entry count does not.

    /// <summary>PRF-08. The most entries one archive may hold. 20000 is 2.5x the corpus map's
    /// 7914 entries. A protected map pads its hash table with decoys, so the count is high by
    /// nature and a low bound would refuse legitimate maps.</summary>
    private const int MaxArchiveEntries = 20000;

    /// <summary>PRF-08. The most bytes one entry may decompress to, read from the block table so
    /// nothing is decompressed to measure it. 256 MiB is far above the largest real entry, a
    /// music or model file of a few MiB, and far below a single-sector bomb.</summary>
    private const long MaxEntryDecompressedBytes = 256L * 1024 * 1024;

    /// <summary>PRF-08. The most bytes one call may hold decompressed across every entry of a
    /// map. 2 GiB is 6x the corpus map's 339 MB, which leaves room for a bigger campaign map and
    /// still bounds a 64 bit process.</summary>
    private const long MaxCallDecompressedBytes = 2L * 1024 * 1024 * 1024;

    /// <summary>PRF-08. The temporary disk a call may assume. A staged write puts a whole second
    /// copy of its output beside the destination before it publishes, so the volume holding that
    /// destination must have this much free. 2 GiB covers a copy of the corpus map plus its
    /// intermediates, and matches the per call decompressed ceiling. The measured value is the
    /// free space, since the staged bytes do not exist yet when the call starts.</summary>
    private const long MaxTempBytes = 2L * 1024 * 1024 * 1024;

    /// <summary>PAR-08. Refuse an array argument above its maximum, naming the limit and the
    /// count received.</summary>
    private static void RequireAtMost(int count, string what)
    {
        if (count > MaxArrayItems)
            throw Invalid($"{what} takes at most {MaxArrayItems} entries, got {count}", what);
    }

    /// <summary>
    /// PRF-04. The check point every slow tool runs, before it starts the command and again
    /// before it publishes. Two of the commands behind these tools now take a token of their own
    /// (the library scan and the model import), so those two stop at the next file inside the run.
    /// The rest take none, so a cancel can only be read at the boundary the tool owns. This makes
    /// the cancel observed there rather than swallowed by the catch in <see cref="Run"/> and
    /// reported as success.
    /// </summary>
    private static void CheckPoint(CancellationToken cancellationToken, string step)
    {
        if (cancellationToken.IsCancellationRequested)
            throw new OperationCanceledException(
                $"cancelled at {step}. The command already running is not interruptible, so no further step is started.");
    }

    /// <summary>
    /// PRF-08. The three archive limits, measured from the block table so policing them costs no
    /// decompression. The entry count, the largest single entry and the running total are all
    /// refused by name with the value measured, never truncated.
    /// </summary>
    private static void RequireArchiveWithinLimits(MapDocument doc, string map)
    {
        int entries = doc.Files.Count;
        if (entries > MaxArchiveEntries)
            throw Wc3Errors.Refuse(Wc3ErrorCodes.ArchiveEntries,
                $"{map} holds {entries} archive entries, the limit is {MaxArchiveEntries}", map);

        long total = 0;
        foreach (var entry in doc.Files)
        {
            long size = entry.RawSize;
            if (size > MaxEntryDecompressedBytes)
                throw Wc3Errors.Refuse(Wc3ErrorCodes.EntryBytes,
                    $"{map} entry '{entry.FileName ?? "(unnamed)"}' decompresses to {size} bytes, "
                    + $"the limit per entry is {MaxEntryDecompressedBytes}", entry.FileName ?? "(unnamed)");
            total += size;
            if (total > MaxCallDecompressedBytes)
                throw Wc3Errors.Refuse(Wc3ErrorCodes.CallBytes,
                    $"{map} decompresses past {total} bytes across {entries} entries, "
                    + $"the limit per call is {MaxCallDecompressedBytes}", map);
        }
    }

    /// <summary>
    /// PRF-08. The temporary disk limit, measured on the volume the staged write will land on. A
    /// volume that cannot be measured, a UNC share with no drive info, is let through rather than
    /// refused, because the limit must never be the thing that breaks a working call.
    /// </summary>
    private static void RequireTempDisk(string destinationPath)
    {
        string? root;
        try { root = Path.GetPathRoot(Path.GetFullPath(destinationPath)); }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException) { return; }
        if (string.IsNullOrEmpty(root)) return;
        long free;
        try
        {
            var drive = new DriveInfo(root);
            if (!drive.IsReady) return;
            free = drive.AvailableFreeSpace;
        }
        catch (Exception ex) when (ex is ArgumentException or IOException or UnauthorizedAccessException) { return; }
        if (free < MaxTempBytes)
            throw Wc3Errors.Refuse(Wc3ErrorCodes.TempDisk,
                $"{root} has {free} bytes free, the temporary disk limit is {MaxTempBytes}", root);
    }

    /// <summary>ERR-05. The skip lines a batch command records, split into the item it named and
    /// the reason beside it, so a partial success is reported as one.</summary>
    private static IReadOnlyList<SkippedItem> SkippedItems(IReadOnlyList<string> diagnostics)
    {
        const char quote = '\'';
        var skipped = new List<SkippedItem>();
        foreach (var line in diagnostics)
        {
            if (line.Length < 3 || line[0] != quote) continue;
            int close = line.IndexOf(quote, 1);
            if (close <= 1) continue;
            skipped.Add(new SkippedItem(line[1..close], line[(close + 1)..].TrimStart(',', ' ').Trim()));
        }
        return skipped;
    }

    /// <summary>
    /// The settings file TargetBuildSettings writes, rebuilt here because the path property that
    /// names it is internal to Wc3.Commands. WC3CTL_SETTINGS_DIR overrides the folder exactly as it
    /// does there, so the CLI, the tests and this tool all name the same file.
    /// </summary>
    private static string TargetSettingsPath()
    {
        string? dir = Environment.GetEnvironmentVariable("WC3CTL_SETTINGS_DIR");
        if (!string.IsNullOrWhiteSpace(dir)) return Path.Combine(dir, "settings.json");
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "wc3ctl", "settings.json");
    }

    /// <summary>
    /// The validation TargetBuildSettings.Set performs before it writes, split out so the write
    /// itself can go through the gate. A schema folder holding a manifest is a target in its own
    /// right, any other value must name a build the schemas folder declares, and the unknown case
    /// is left to LoadSchema, which throws the FileNotFoundException with the close build names.
    /// </summary>
    private static void RequireKnownBuild(string build, string? schemasDir)
    {
        if (Directory.Exists(build) && File.Exists(Path.Combine(build, "manifest.json"))) return;
        foreach (var info in TargetBuildSettings.List(schemasDir))
            if (string.Equals(info.Build, build, StringComparison.Ordinal)) return;
        TargetBuildSettings.LoadSchema(build, schemasDir);
    }

    /// <summary>
    /// The settings JSON with targetBuild replaced and every other key kept. The property is
    /// written once and an existing targetBuild is skipped rather than named twice, which is what
    /// produced a duplicate key before. A file that cannot be parsed starts fresh.
    /// </summary>
    private static string TargetSettingsJson(string path, string build)
    {
        JsonDocument existing;
        try
        {
            existing = File.Exists(path)
                ? JsonDocument.Parse(File.ReadAllText(path))
                : JsonDocument.Parse("{}");
        }
        catch (JsonException)
        {
            existing = JsonDocument.Parse("{}");
        }

        using (existing)
        {
            using var buffer = new MemoryStream();
            using (var writer = new Utf8JsonWriter(buffer))
            {
                writer.WriteStartObject();
                foreach (var prop in existing.RootElement.EnumerateObject())
                {
                    if (prop.Name == "targetBuild") continue;
                    prop.WriteTo(writer);
                }
                writer.WriteString("targetBuild", build);
                writer.WriteEndObject();
            }
            // UTF-8 with no byte order mark, which is what WriteGate.WriteText publishes.
            return System.Text.Encoding.UTF8.GetString(buffer.ToArray());
        }
    }

    /// <summary>ERR-02. A required argument is absent. The message is the sentence the surface
    /// already used, and the argument name is the offending value the error result reports.</summary>
    private static McpException Missing(string argument) =>
        Wc3Errors.Refuse(Wc3ErrorCodes.InvalidArgument, argument + " is required", argument);

    /// <summary>ERR-02. A value is outside its legal set, or a limit on the argument was broken.
    /// The message names the set or the limit, so the caller needs no other source.</summary>
    private static McpException Invalid(string message, string? value = null) =>
        Wc3Errors.Refuse(Wc3ErrorCodes.InvalidArgument, message, value);

    /// <summary>ERR-02. The shared command refused the request and named its own reason.
    /// The tool layer did not author the wording, so it reports the shared refusal as one code.</summary>
    private static McpException Rejected(string message) =>
        Wc3Errors.Refuse(Wc3ErrorCodes.CommandRejected, message);

    /// <summary>ERR-02. The map path does not exist. Every loader entry point uses this, so the
    /// text is the one sentence the surface has always returned.</summary>
    private static McpException MapNotFound(string map) =>
        Wc3Errors.Refuse(Wc3ErrorCodes.MapNotFound, "map not found: " + map, map);

    /// <summary>ERR-02. An unexpected failure, with the message the runtime gave, reported as
    /// <see cref="Wc3ErrorCodes.Internal"/> so a caller never reads it as its own mistake.</summary>
    private static McpException Failed(Exception ex) =>
        Wc3Errors.Refuse(Wc3ErrorCodes.Internal, ex.Message);

    private static T Run<T>(Func<T> body)
    {
        try { return body(); }
        catch (McpException) { throw; }
        // PRF-04. A cancel is reported as a cancel. The generic arm below would otherwise turn
        // OperationCanceledException into an McpException and the client could not tell a
        // cancelled call from a failed one.
        catch (OperationCanceledException) { throw; }
        catch (ToolError ex)
        {
            throw Wc3Errors.Refuse(ex.Code, ex.Message, next: ex.Next);
        }
        catch (Exception ex) { throw Failed(ex); }
    }

    /// <summary>RES-10. One capped collection under the page names every list result shares, with
    /// the three names these twelve tools served before kept as aliases for one minor release. A
    /// capped list has no offset argument, so <c>nextOffset</c> is the index the next page would
    /// start at when the cap cut anything, and null when it did not.</summary>
    private static CappedPage<T> Page<T>(CappedItems<T> capped) =>
        new(capped.Items.Count, capped.Total, capped.Truncated,
            capped.Truncated ? capped.Cap : null, capped.Items, capped.Cap, capped.Truncated, capped.Total);

    private static MapDocument LoadMap(string map)
    {
        if (string.IsNullOrWhiteSpace(map))
            throw Missing("map path");
        // A folder map is a real Studio workflow, so `map` accepts one archive file or a folder.
        // The extension check only applies to a file. PAR-01 reopens as a wording problem: every
        // parameter says which of the two it takes, rather than the loader refusing one of them.
        bool folder = Directory.Exists(map);
        if (!File.Exists(map) && !folder)
            throw MapNotFound(map);
        if (!folder)
        {
            string extension = System.IO.Path.GetExtension(map);
            if (!extension.Equals(".w3x", StringComparison.OrdinalIgnoreCase)
                && !extension.Equals(".w3m", StringComparison.OrdinalIgnoreCase))
                throw Wc3Errors.Refuse(Wc3ErrorCodes.MapUnreadable, $"map must be a .w3x or .w3m file: {map}", map);
        }
        MapDocument doc;
        string? sha = null;
        try
        {
            if (File.Exists(map))
            {
                // Hash the exact snapshot the document is built from, so a later write reports the
                // digest of what it read rather than the digest of whatever replaced the file.
                byte[] bytes = File.ReadAllBytes(map);
                doc = MapDocument.Load(bytes);
                sha = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
            }
            else
            {
                doc = MapDocument.Load(map);
            }
        }
        catch (Exception ex) { throw Wc3Errors.Refuse(Wc3ErrorCodes.MapUnreadable, $"could not load '{map}': {ex.Message}", map); }

        // PRF-08. Outside the catch above, so a refused archive names the limit it broke rather
        // than being reworded as "could not load". Measured from the block table, no decompression.
        RequireArchiveWithinLimits(doc, map);
        if (sha is not null) LoadedSha.AddOrUpdate(doc, sha);
        return doc;
    }

    /// <summary>Digests of the bytes each loaded document was built from, keyed by the document.</summary>
    private static readonly ConditionalWeakTable<MapDocument, string> LoadedSha = new();

    private static string? LoadedShaOf(MapDocument doc) =>
        LoadedSha.TryGetValue(doc, out var sha) ? sha : null;

    /// <summary>Validate a map path without parsing it, for the container tools that read the
    /// archive directly. Same "map not found" message LoadMap gives.</summary>
    private static string RequireMapPath(string map)
    {
        if (string.IsNullOrWhiteSpace(map))
            throw Missing("map path");
        if (!File.Exists(map))
            throw MapNotFound(map);
        return map;
    }

    /// <summary>SUR-08. The refusal a placement call gets when it passes the nested object and one
    /// of the flattened arguments together. Half applying one form over the other is the failure
    /// this refuses, because the caller cannot tell which field won.</summary>
    private const string MixedPlacementForm =
        "pass either placement or the flattened placement arguments, not both. The flattened form is "
        + "kept for one version (SUR-08)";

    /// <summary>SUR-08. The unit placement from either form. The nested object is returned as it
    /// arrived, the flattened arguments are read into the same record, and a call that supplies
    /// parts of both is refused rather than half applied. A missing required field is named, so a
    /// caller that drops one learns which.</summary>
    private static UnitPlacement UnitPlacementOf(
        UnitPlacement? placement, string? typeRawcode, int? ownerId, float? x, float? y,
        float? z, float? rotation, float? scale)
    {
        if (placement is not null)
        {
            if (typeRawcode is not null || ownerId is not null || x is not null || y is not null
                || z is not null || rotation is not null || scale is not null)
                throw Invalid(MixedPlacementForm, "placement");
            return placement;
        }

        if (string.IsNullOrWhiteSpace(typeRawcode)) throw Missing("type_rawcode");
        if (ownerId is null) throw Missing("owner_id");
        if (x is null) throw Missing("x");
        if (y is null) throw Missing("y");
        return new UnitPlacement(typeRawcode, ownerId.Value, x.Value, y.Value,
            z ?? 0f, rotation ?? 0f, scale ?? 1f);
    }

    /// <summary>SUR-08. The doodad placement from either form, the same contract as
    /// <see cref="UnitPlacementOf"/> with the variation index in place of an owner.</summary>
    private static DoodadPlacement DoodadPlacementOf(
        DoodadPlacement? placement, string? typeRawcode, float? x, float? y,
        float? z, float? rotation, float? scale, int? variation)
    {
        if (placement is not null)
        {
            if (typeRawcode is not null || x is not null || y is not null || z is not null
                || rotation is not null || scale is not null || variation is not null)
                throw Invalid(MixedPlacementForm, "placement");
            return placement;
        }

        if (string.IsNullOrWhiteSpace(typeRawcode)) throw Missing("type_rawcode");
        if (x is null) throw Missing("x");
        if (y is null) throw Missing("y");
        return new DoodadPlacement(typeRawcode, x.Value, y.Value,
            z ?? 0f, rotation ?? 0f, scale ?? 1f, variation ?? 0);
    }

    /// <summary>Load the map, apply a placement mutation, and save the edited copy to out_path.
    /// The input map is never modified in place (out_path must differ) - mirroring the no-clobber
    /// stance of render_model and port_unit. A rejected placement surfaces as a clean MCP error.</summary>
    private static PlacementToolResult SavePlacement(
        string map, string outPath, bool overwrite, string? expectInputSha256,
        Func<MapDocument, (bool Ok, string Message, int CreationNumber)> place)
    {
        if (string.IsNullOrWhiteSpace(outPath))
            throw Missing("out_path");

        var doc = LoadMap(map);
        var (ok, message, creationNumber) = place(doc);
        if (!ok) throw Rejected(message);

        // Every placement write takes the same gate as every other edit, so it reports the same
        // digests and refuses to replace a file the caller did not agree to replace (SAF-05, SAF-06).
        var w = WriteGate.Save(doc, map, outPath, overwrite, expectInputSha256, LoadedShaOf(doc));
        return new PlacementToolResult(w.SavedTo, message, creationNumber,
            w.InputSha256, w.OutputSha256, w.FilesRewritten, w.BytesWritten);
    }

    /// <summary>Load the map, apply a terrain edit, and save the edited copy to out_path.</summary>
    private static TerrainToolResult SaveTerrain(
        string map, string outPath, bool overwrite, string? expectInputSha256,
        Func<MapDocument, (bool Ok, string Message, int TilesChanged)> edit)
    {
        if (string.IsNullOrWhiteSpace(outPath))
            throw Missing("out_path");

        var doc = LoadMap(map);
        var (ok, message, tilesChanged) = edit(doc);
        if (!ok) throw Rejected(message);

        var w = WriteGate.Save(doc, map, outPath, overwrite, expectInputSha256, LoadedShaOf(doc));
        return new TerrainToolResult(w.SavedTo, message, tilesChanged, w.InputSha256, w.OutputSha256,
            w.FilesRewritten, w.BytesWritten);
    }

    /// <summary>Load the map, apply a sound-catalog mutation, and save the edited copy to out_path.</summary>
    private static SoundToolResult SaveSound(
        string map, string outPath, bool overwrite, string? expectInputSha256,
        Func<MapDocument, SoundOpResult> edit)
    {
        if (string.IsNullOrWhiteSpace(outPath))
            throw Missing("out_path");

        var doc = LoadMap(map);
        var r = edit(doc);
        if (!r.Ok) throw Rejected(r.Message);

        var w = WriteGate.Save(doc, map, outPath, overwrite, expectInputSha256, LoadedShaOf(doc));
        return new SoundToolResult(w.SavedTo, r.Message,
            w.InputSha256, w.OutputSha256, w.FilesRewritten, w.BytesWritten);
    }

    /// <summary>Load the map, apply an (Ok, Message) mutation, and save the edited copy to
    /// out_path. Shared by the camera, pathing, map-info, player and force write tools.</summary>
    private static EditToolResult SaveEdit(
        string map, string outPath, bool overwrite, string? expectInputSha256,
        Func<MapDocument, (bool Ok, string Message)> edit)
    {
        var doc = LoadMap(map);
        var (ok, message) = edit(doc);
        if (!ok) throw Rejected(message);
        var w = WriteGate.Save(doc, map, outPath, overwrite, expectInputSha256, LoadedShaOf(doc));
        return new EditToolResult(w.SavedTo, message, w.InputSha256, w.OutputSha256,
            w.FilesRewritten, w.BytesWritten);
    }

    /// <summary>Parse a string into an enum (case-insensitive); clean MCP error on failure.</summary>
    private static TEnum ParseEnum<TEnum>(string value, string paramName) where TEnum : struct, Enum
    {
        if (Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed) && Enum.IsDefined(parsed))
            return parsed;
        throw Invalid($"invalid {paramName} '{value}'; expected one of: " +
            string.Join(", ", Enum.GetNames<TEnum>()).ToLowerInvariant(), value);
    }

    /// <summary>Validate an out_path (required, must differ from the input) and return its full
    /// path. The parameter is nullable because every mutating tool takes out_path as an optional
    /// argument and this method is what refuses a missing one. Declaring it non-null made the
    /// callers warn instead, which is the same defect one call site further out.</summary>
    private static string ResolveOutPath(string map, string? outPath)
    {
        string full;
        if (string.IsNullOrWhiteSpace(outPath))
        {
            // The documented default every mutating tool states, a sibling "<map>.edited.<ext>"
            // copy so the input is never overwritten. Without it a caller that omits the optional
            // out_path was refused with "out_path is required" even though the schema and the
            // description both say it may be omitted.
            string dir = Path.GetDirectoryName(Path.GetFullPath(map)) ?? ".";
            full = Path.TrimEndingDirectorySeparator(Path.Combine(dir,
                Path.GetFileNameWithoutExtension(map) + ".edited" + Path.GetExtension(map)));
        }
        else
        {
            full = Path.TrimEndingDirectorySeparator(Path.GetFullPath(outPath));
        }
        string input = Path.TrimEndingDirectorySeparator(Path.GetFullPath(map));
        if (string.Equals(full, input, StringComparison.OrdinalIgnoreCase))
            throw Wc3Errors.Refuse(Wc3ErrorCodes.OutPathIsInput, "out_path must not be the input map file itself", "out_path");
        // A folder map's copy cannot live inside the folder it was read from.
        if (Directory.Exists(input) && full.StartsWith(input + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            throw Wc3Errors.Refuse(Wc3ErrorCodes.OutPathInsideInput, "out_path must not be inside the input map folder", "out_path");
        return full;
    }

    private static ObjectKind ParseKind(string kind)
    {
        try { return ObjectKinds.Parse(kind); }
        catch (ArgumentException ex) { throw Wc3Errors.Refuse(Wc3ErrorCodes.InvalidArgument, ex.Message, kind); }
    }

    /// <summary>Split a "code=value" pair on its first '=', the value may itself contain '='.</summary>
    private static bool TrySplitPair(string token, out string code, out string value)
    {
        int eq = token.IndexOf('=');
        if (eq <= 0) { code = ""; value = ""; return false; }
        code = token[..eq];
        value = token[(eq + 1)..];
        return true;
    }

    /// <summary>Read a PNG the caller named; a missing file is a clean MCP error.</summary>
    private static byte[] ReadPngFile(string png)
    {
        if (string.IsNullOrWhiteSpace(png))
            throw Missing("png path");
        if (!File.Exists(png))
            throw Wc3Errors.Refuse(Wc3ErrorCodes.InputNotFound, $"png not found {png}", png);
        return File.ReadAllBytes(png);
    }

    /// <summary>Write PNG bytes to a caller-named path (never the input map), creating dirs.</summary>
    private static string WritePngFile(string map, string outPath, byte[] bytes)
    {
        string full = ResolveOutPath(map, outPath);
        if (Path.GetDirectoryName(full) is { Length: > 0 } dir) Directory.CreateDirectory(dir);
        File.WriteAllBytes(full, bytes);
        return full;
    }

    internal const string GameDirVariable = "WC3_GAME_DIR";

    // ---- gamedata schema tools --------------------------------------------

    [McpServerTool(Name = "map_compat", Title = "Check build compatibility", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Check a map against a target build, given as a schema folder path or a build name. Reports compile time incompatibilities (natives, globals, types or functions the map uses and the target scripts do not declare) and display time ones (object fields the map modifies and the target does not define). IdentifiersChecked and FieldsChecked tell checked and fine apart from nothing to check. Omit target to use the configured default from settings, then the environment, then the newest schema. Never writes.")]
    public static CompatResult MapCompat(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Target build name or schema folder path to check compatibility against. Omit to use the configured default from settings, environment, or the newest available schema.")] string? target = null,
        [Description("Optional path to schemas directory containing schema folders with manifest.json. When omitted, searches <AppContext.BaseDirectory>/schemas first, then walks up from the base directory.")] string? schemas_dir = null)
        => Run(() => TargetCompatCommand.Execute(map, ResolveTarget(target, schemas_dir), schemas_dir));

    [McpServerTool(Name = "compat_matrix", Title = "Compatibility matrix", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Check several maps against several target builds and return a grid of pass or fail per cell, with the finding count in each cell. Pass means the target build raised no error-level finding, so a map that only warns still passes. The whole-map by whole-build view the single map_compat call cannot give.")]
    public static CompatMatrixResult CompatMatrix(
        [Description("One or more .w3x/.w3m map paths. At most 1000, and a longer list is refused with a tool error naming the limit.")] string[] maps,
        [Description("One or more target build names or schema folder paths. At most 1000, and a longer list is refused with a tool error naming the limit.")] string[] targets,
        [Description("Optional path to schemas directory containing schema folders with manifest.json. When omitted, searches <AppContext.BaseDirectory>/schemas first, then walks up from the base directory.")] string? schemas_dir = null)
        => Run(() =>
        {
            RequireAtMost(maps.Length, "maps");
            RequireAtMost(targets.Length, "targets");
            return CompatMatrixCommand.Execute(maps, targets, schemas_dir);
        });

    [McpServerTool(Name = "compat_explain", Title = "Explain one identifier", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Explain one identifier against a target build. Reports whether the target declares it as a native, global, type or function, and if not, the earliest build that added it plus the test build it was first seen in, and every build that declares it. Reuses the compat union index, so its answer about when a name appeared matches a compat finding.")]
    public static CompatExplainResult CompatExplain(
        [Description("Identifier to explain, a native, global, type or function name.")] string name,
        [Description("Target build name or schema folder path to explain against. Omit to use the configured default from settings, environment, or the newest available schema.")] string? target = null,
        [Description("Optional path to schemas directory containing schema folders with manifest.json.")] string? schemas_dir = null)
        => Run(() => CompatExplainCommand.Execute(name, ResolveTarget(target, schemas_dir), schemas_dir));

    [McpServerTool(Name = "target_get", Title = "Show the target build", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("View the current target build setting: which build maps are checked against when compat or lint omit --target, where it comes from (option, environment, settings, default), and which schema sections are available. Also shows the installed game build version.")]
    public static TargetGetResult TargetGet(
        [Description("Optional path to schemas directory containing schema folders with manifest.json.")] string? schemas_dir = null)
        => Run(() =>
        {
            var result = TargetBuildSettings.Get(option: null, schemasDir: schemas_dir);
            string? installBuild = null;
            try
            {
                string? installDir = Wc3.GameData.GameInstall.Locate();
                if (installDir is not null)
                    installBuild = Wc3.GameData.GameInstall.ReadBuildVersion(installDir);
            }
            catch { /* no game install */ }
            // Lowercase, because the description names the values option, environment, settings and
            // default, and a client matching those strings got the enum's PascalCase instead.
            return new TargetGetResult(result.Build, result.Source.ToString().ToLowerInvariant(), installBuild,
                result.Sections ?? Array.Empty<string>());
        });

    [McpServerTool(Name = "target_set", Title = "Set the target build", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Set the target build that compat and lint use when --target is omitted. Validates the build exists in the schemas folder, then writes the settings file through the write gate, which refuses an existing settings file without overwrite and refuses a path that crosses a symbolic link. The write is staged beside the settings file and renamed into place, and the answer carries savedTo, inputSha256, outputSha256, filesRewritten and bytesWritten.")]
    public static TargetSetResult TargetSet(
        [Description("Target build name or schema folder path to set as the default.")] string build,
        [Description("Optional path to schemas directory containing schema folders with manifest.json.")] string? schemas_dir = null,
        [Description(OverwriteDoc)] bool overwrite = false)
        => Run(() =>
        {
            string path = TargetSettingsPath();
            try
            {
                RequireKnownBuild(build, schemas_dir);
            }
            catch (FileNotFoundException ex)
            {
                return new TargetSetResult(false, ex.Message, SavedTo: null);
            }

            // The settings file is both the input and the output of this write, so naming it as a
            // gate input would trip OUT_PATH_IS_INPUT and no write would ever land. Its digest
            // before the write is what the outcome reports, and the gate is handed no input.
            string? inputSha = File.Exists(path) ? WriteGate.Sha256File(path) : null;
            var w = WriteGate.WriteText(path, TargetSettingsJson(path, build), Array.Empty<string>(),
                overwrite, expectInputSha256: null);
            return new TargetSetResult(true, "set target " + build, w.SavedTo,
                inputSha, w.OutputSha256, w.FilesRewritten, w.BytesWritten);
        });

    [McpServerTool(Name = "target_list", Title = "List available builds", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("List all available builds in the schemas directory, with their sections. Use --full_only to skip scripts-only builds.")]
    public static CappedPage<BuildInfo> TargetList(
        [Description("Optional path to schemas directory containing schema folders with manifest.json.")] string? schemas_dir = null,
        [Description("When true, skip scripts-only builds that have no object data.")] bool full_only = false)
        => Run(() => Page(Wc3Limit.Cap(TargetBuildSettings.List(schemasDir: schemas_dir, fullOnly: full_only))));

    [McpServerTool(Name = "gamedata_schema_diff", Title = "Diff two schema folders", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Compare two schema folders (produced by gamedata schema) and report added, removed and changed object fields, trigger functions, script types and natives, and enumerated field option sets. Groups output by section (objects, triggers, script, enums) then by file or category. A section only one build has is listed as not compared rather than everything added or removed. Use this to track what a patch changes across builds, or to check whether two maps targeting different patches are compatible.")]
    public static SchemaDiffResult GameDataSchemaDiff(
        [Description("Path to the first schema folder (from gamedata schema).")] string a,
        [Description("Path to the second schema folder (from gamedata schema).")] string b,
        [Description("Show changes for one section only: objects, triggers, or script. Omit for all.")] string? section = null)
        => Run(() => GameSchemaCommand.Filter(GameSchemaCommand.Diff(a, b), section));

    [McpServerTool(Name = "gamedata_history", Title = "Trace a name across builds", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Report whether a name is an object field code, a native, a type, a global or a function across every recorded build. Lists every build each kind appears in, plus the first and last build that carry the name, and the object metadata files that define it as a field.")]
    public static GamedataHistoryResult GameDataHistory(
        [Description("The name to look for, a field code or a JASS identifier.")] string name,
        [Description("Optional path to schemas directory containing schema folders with manifest.json.")] string? schemas_dir = null)
        => Run(() => GamedataHistoryCommand.Execute(name, schemas_dir));

    [McpServerTool(Name = "gamedata_changelog", Title = "Diff object fields between builds", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Diff two builds' object data field by field. Reports fields added, removed, retyped, or whose minimum or maximum bound changed, grouped by object metadata file. A file only one build carries is listed as not compared rather than every field added or removed.")]
    public static GamedataChangelogResult GameDataChangelog(
        [Description("The older build name or schema folder path.")] string build_a,
        [Description("The newer build name or schema folder path.")] string build_b,
        [Description("Optional path to schemas directory containing schema folders with manifest.json.")] string? schemas_dir = null)
        => Run(() => GamedataChangelogCommand.Execute(build_a, build_b, schemas_dir));

    [McpServerTool(Name = "gamedata_field_lookup", Title = "Find object field codes", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Find object field codes whose display name contains the query, in one build or all builds. Reports the code, display name, type and build for each match.")]
    public static GamedataFieldLookupResult GameDataFieldLookup(
        [Description("Text to find inside a field display name, case-insensitive.")] string query,
        [Description("Build name or schema folder path to search. Omit to search every build.")] string? target = null,
        [Description("Optional path to schemas directory containing schema folders with manifest.json.")] string? schemas_dir = null)
        => Run(() => GamedataFieldLookupCommand.Execute(query, target, schemas_dir));

    [McpServerTool(Name = "gamedata_bounds", Title = "Show field bounds", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Report a field code's type, minimum and maximum bounds, whether it can be empty, and the object metadata file that defines it, in one build or every build where it exists.")]
    public static GamedataBoundsResult GameDataBounds(
        [Description("The object field code to inspect.")] string code,
        [Description("Build name or schema folder path to search. Omit to search every build.")] string? target = null,
        [Description("Optional path to schemas directory containing schema folders with manifest.json.")] string? schemas_dir = null)
        => Run(() => GamedataBoundsCommand.Execute(code, target, schemas_dir));

    [McpServerTool(Name = "gamedata_enum", Title = "Show enum values", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Report the closed set of legal values for an enumerated object field type, from the build's UnitEditorData.txt, in one build or every build where it exists. The type is the metadata SLK type column, the one gamedata_bounds reports for a field code. This is the authority a picker should offer, not the values a stock object happens to use.")]
    public static GamedataEnumResult GameDataEnum(
        [Description("The enumerated field type to inspect, the metadata SLK type column.")] string type,
        [Description("Build name or schema folder path to search. Omit to search every build.")] string? target = null,
        [Description("Optional path to schemas directory containing schema folders with manifest.json.")] string? schemas_dir = null)
        => Run(() => GamedataEnumCommand.Execute(type, target, schemas_dir));

    [McpServerTool(Name = "gamedata_coverage", Title = "Schema coverage per build", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("List one row per schema folder with the build, its era, and which sections it holds (objects, triggers, script, enums), plus field and native counts, so a reader can tell a build carrying real object data from one carrying only its script.")]
    public static GamedataCoverageResult GameDataCoverage(
        [Description("Optional path to schemas directory containing schema folders with manifest.json.")] string? schemas_dir = null)
        => Run(() => GamedataCoverageCommand.Execute(schemas_dir));

    // ---- bench tools -----------------------------------------------------

    [McpServerTool(Name = "bench_schema_coverage", Title = "Measure schema data", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Measure what object data a schemas corpus carries. One row per schema folder names the build, its era and whether the object data is real (the folder holds its own fields), era-derived (no fields here but a sibling build in the same era carries them) or absent, plus the real field count, and the totals. Reuses the compat schema resolution and the gamedata era labels.")]
    public static SchemaCoverageResult BenchSchemaCoverage(
        [Description("Optional path to schemas directory containing schema folders with manifest.json.")] string? schemas_dir = null)
        => Run(() => BenchSchemaCoverageCommand.Execute(schemas_dir));

    [McpServerTool(Name = "bench_compat", Title = "Benchmark build compatibility", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Check every map against every target build and return a pass or fail matrix with the finding counts in each cell, plus the aggregate over the whole grid. Pass means the target build raised no error-level finding. The measured sweep behind compat_matrix, with the totals a benchmark report needs.")]
    public static BenchCompatResult BenchCompat(
        [Description("One or more .w3x/.w3m map paths. At most 1000, and a longer list is refused with a tool error naming the limit.")] string[] maps,
        [Description("One or more target build names or schema folder paths. At most 1000, and a longer list is refused with a tool error naming the limit.")] string[] targets,
        [Description("Optional path to schemas directory containing schema folders with manifest.json.")] string? schemas_dir = null)
        => Run(() =>
        {
            RequireAtMost(maps.Length, "maps");
            RequireAtMost(targets.Length, "targets");
            return BenchCompatCommand.Execute(maps, targets, schemas_dir);
        });

    [McpServerTool(Name = "bench_leaks", Title = "Leak counts for a folder", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Run the static leak detector over every map in a folder and report the counts per map and per leak kind, with the hot and repeat totals. The folder sweep the single script_leaks call cannot give, so a whole map set can be ranked by how much leaking code it carries.")]
    public static BenchLeaksResult BenchLeaks(
        [Description("Folder of .w3x/.w3m maps, searched recursively.")] string folder)
        => Run(() => BenchLeaksCommand.Execute(folder));

    [McpServerTool(Name = "bench_open", Title = "Time archive loading", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Open every map in a folder through the loader and record the per-map open time in milliseconds, entry count and total decompressed bytes, reporting the slowest map and the median open time. The load is read-only and never writes a map back.")]
    public static BenchOpenResult BenchOpen(
        [Description("Folder of .w3x/.w3m maps, searched recursively.")] string folder)
        => Run(() => BenchOpenCommand.Execute(folder));

    [McpServerTool(Name = "bench_perf_gate", Title = "Check benchmarks for regressions", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Compare two recorded benchmark result JSON files and report whether any metric regressed by more than a threshold, for use as a CI gate. Each file is an object with a name, an optional generated stamp, a metrics map of name to value, and an optional higherIsBetter list naming metrics where an increase is an improvement. A metric in only one file is reported rather than counted. The comparison is pure data, it runs no benchmark.")]
    public static PerfGateResult BenchPerfGate(
        [Description("Path to the baseline benchmark JSON file.")] string baseline,
        [Description("Path to the benchmark JSON file to compare against the baseline.")] string run,
        [Description("Fail when any metric regressed by more than this percent. Default 10.")] double max_regression_pct = 10)
        => Run(() => BenchPerfGateCommand.Execute(baseline, run, max_regression_pct));

    // ---- ci gate tools ---------------------------------------------------

    [McpServerTool(Name = "ci_lint", Title = "CI gate for lint", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Run the lint as a CI gate. Returns one pass or fail per check with what was examined and an exit code (0 pass, 1 fail). Fails on an error-level finding, or, with baseline, only on a finding the baseline does not already hold, so a map with known and accepted warnings can still be gated on regressions. With target it also runs the compat check and fails on an error-level finding. Read-only, never writes the map or the baseline.")]
    public static CiLintResult CiLint(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Baseline file written by lint_baseline. When given, the gate fails only on a finding the baseline does not already hold.")] string? baseline = null,
        [Description("Target build name or schema folder path. When given, a compat check runs and fails on an error-level finding.")] string? target = null,
        [Description("Optional path to schemas directory containing schema folders with manifest.json.")] string? schemas_dir = null,
        [Description("Optional path to the ORIGINAL map this one was built from, so the lint also reports what a rebuild lost or clobbered.")] string? against = null)
        => Run(() => CiLintCommand.Execute(map, baseline, target, schemas_dir, against));

    [McpServerTool(Name = "ci_validate", Title = "CI gate for validate", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Run the validation as a CI gate. Returns one pass or fail with what was examined and an exit code (0 pass, 1 fail). Fails when the map cannot load or reports an error-level issue. With strict the extra strict checks (missing listfile, a script with no entry point, an undefined placement type) are included. Read-only.")]
    public static CiValidateResult CiValidate(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Run the stricter validation, whose extra checks are too strict to be a default.")] bool strict = false,
        [Description("Warcraft III install directory for the strict undefined-type check. Overrides auto-detection and the WC3_GAME_DIR env var.")] string? game_dir = null)
        => Run(() => CiValidateCommand.Execute(map, strict, ResolveGameDir(game_dir)));

    [McpServerTool(Name = "ci_compat", Title = "CI gate for compat", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Run the compat check as a CI gate. Returns one pass or fail with what was examined and an exit code (0 pass, 1 fail). Fails on an error-level finding, a native, global, type or function the map uses that the target build does not declare. Warnings about object fields the target no longer defines do not fail the gate. Read-only.")]
    public static CiCompatResult CiCompat(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Target build name or schema folder path.")] string target,
        [Description("Optional path to schemas directory containing schema folders with manifest.json.")] string? schemas_dir = null)
        => Run(() => CiCompatCommand.Execute(map, target, schemas_dir));

    [McpServerTool(Name = "ci_schema", Title = "CI gate for schema data", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Check the schema dataset for regressions as a CI gate. Returns one pass or fail per check with what was examined and an exit code (0 pass, 1 fail). Fails when any build stops parsing or fewer than min_real builds carry real object data. Each folder is loaded on its own so a broken build is named rather than aborting the whole check. Read-only.")]
    public static CiSchemaResult CiSchema(
        [Description("Optional path to schemas directory containing schema folders with manifest.json.")] string? schemas_dir = null,
        [Description("Least number of builds with real object data that passes. 0 means no floor.")] int min_real = 0)
        => Run(() => CiSchemaCommand.Execute(schemas_dir, min_real));

    [McpServerTool(Name = "ci_all", Title = "Run all CI gates", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Run lint, validate and, when target is given, compat in one pass. Returns one summary line per gate and the overall pass or fail with an exit code (0 pass, 1 fail), so a pipeline gets the whole map picture from one call. Reuses the lint and validate gates, so a gate means here exactly what ci_lint and ci_validate mean on their own. Read-only.")]
    public static CiAllResult CiAll(
        [Description("Path to a .w3x/.w3m map file, or to an unpacked map folder holding the same entries.")] string map,
        [Description("Target build name or schema folder path. When given, the compat gate runs. When omitted the gate is reported as skipped.")] string? target = null,
        [Description("Optional path to schemas directory containing schema folders with manifest.json.")] string? schemas_dir = null)
        => Run(() => CiAllCommand.Execute(map, target, schemas_dir));

    // ---- library tools ---------------------------------------------------

    [McpServerTool(Name = "library_scan", Title = "Scan a folder of maps", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Open every map in a folder through the loader and report per map its size in bytes, the build it targets if known, whether it loads, its archive entry count and its diagnostic count, plus the loaded, failed and byte totals. Opens one map at a time. Read-only.")]
    public static LibraryScanResult LibraryScan(
        [Description("Folder of .w3x/.w3m maps, searched recursively.")] string folder,
        [Description("Cancels the call at the next check point. The sweep reads the token before each map opens, so it stops at the next file rather than at the end of the folder.")] CancellationToken cancellationToken = default,
        IProgress<ProgressNotificationValue>? progress = null)
        => Run(() =>
        {
            CheckPoint(cancellationToken, "the scan start");
            ServerRuntime.ReportProgress(progress, "library_scan", "started, sweeping the folder");
            return LibraryScanCommand.Execute(folder, cancellationToken,
                (done, total) => ServerRuntime.ReportProgress(
                    progress, "library_scan", "scanned " + done + " of " + total + " maps", done, total));
        });

    [McpServerTool(Name = "library_audit", Title = "Audit a folder of maps", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Run the behavioural audit over every map in a folder and report the findings per map and the totals per check, so a whole library is ranked by how much it needs fixing. Opens one map at a time. Read-only.")]
    public static LibraryAuditResult LibraryAudit(
        [Description("Folder of .w3x/.w3m maps, searched recursively.")] string folder,
        [Description("Warcraft III install directory for the checks that need game data. Omit to auto-detect.")] string? game_dir = null,
        [Description("Cancels the call at the next check point. The audit itself is not interruptible once it starts.")] CancellationToken cancellationToken = default)
        => Run(() =>
        {
            CheckPoint(cancellationToken, "the audit start");
            return LibraryAuditCommand.Execute(folder, ResolveGameDir(game_dir));
        });

    [McpServerTool(Name = "library_validate", Title = "Validate a folder of maps", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Run the load validation over every map in a folder and list the ones that fail, with the reason. A map that cannot be opened counts as invalid. Opens one map at a time. Read-only.")]
    public static LibraryValidateResult LibraryValidate(
        [Description("Folder of .w3x/.w3m maps, searched recursively.")] string folder,
        [Description("Cancels the call at the next check point. The validation itself is not interruptible once it starts.")] CancellationToken cancellationToken = default)
        => Run(() =>
        {
            CheckPoint(cancellationToken, "the validation start");
            return LibraryValidateCommand.Execute(folder);
        });

    [McpServerTool(Name = "library_compare", Title = "Compare two map folders", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Compare two folders of maps by file name and content hash, listing the maps only in the first folder, only in the second, and changed between them, plus the identical count and any file name that is not unique inside a folder. Read-only.")]
    public static LibraryCompareResult LibraryCompare(
        [Description("The first folder of maps.")] string folder_a,
        [Description("The second folder of maps.")] string folder_b,
        [Description("Cancels the call at the next check point. The comparison itself is not interruptible once it starts.")] CancellationToken cancellationToken = default)
        => Run(() =>
        {
            CheckPoint(cancellationToken, "the comparison start");
            return LibraryCompareCommand.Execute(folder_a, folder_b);
        });

    [McpServerTool(Name = "library_report", Title = "Write a library report", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Write one Markdown report over a folder of maps, a summary line, a per-map table and the aggregated findings, to out_path. The report is built from the same probes the scan, audit and validate sweeps use. Writes only out_path and never a map.")]
    public static object LibraryReport(
        [Description("Folder of .w3x/.w3m maps, searched recursively.")] string folder,
        [Description("Path to write the Markdown report to.")] string out_path,
        [Description("Warcraft III install directory for the checks that need game data. Omit to auto-detect.")] string? game_dir = null,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null,
        [Description("Cancels the call at the next check point. The report sweep itself is not interruptible, and a cancel during it still writes nothing.")] CancellationToken cancellationToken = default)
        => Run<object>(() =>
        {
            CheckPoint(cancellationToken, "the report start");
            return WriteWith(out_path, Inputs(folder), overwrite, expect_input_sha256,
                temp =>
                {
                    CheckPoint(cancellationToken, "the sweep");
                    return LibraryReportCommand.Execute(folder, temp, ResolveGameDir(game_dir));
                }, "path");
        });
    [McpServerTool(Name = "gamedata_sources", Title = "Show dataset provenance", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Return the dataset's provenance index. For every schema folder it reports the build, its era, the sections present, the SHA-256 of each section file, and the input hashes from the folder's manifest, so the whole dataset can be traced back to the game files it was derived from.")]
    public static GamedataSourcesResult GameDataSources(
        [Description("Optional path to schemas directory containing schema folders with manifest.json.")] string? schemas_dir = null)
        => Run(() => GamedataSourcesCommand.Build(schemas_dir));

    [McpServerTool(Name = "gamedata_export_schemas", Title = "Export the schema dataset", ReadOnly = false, Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Export the schema dataset. Copies every schema folder into a target directory, or packs the whole dataset into a single zip when out_path ends in .zip, for attaching to a release. Writes only under out_path. Returns the build folder names, file count and byte total.")]
    public static object GameDataExportSchemas(
        [Description("Target directory for the dataset, or a path ending in .zip to pack it into one archive.")] string out_path,
        [Description("Optional path to schemas directory containing schema folders with manifest.json.")] string? schemas_dir = null,
        [Description(OverwriteDoc)] bool overwrite = false,
        [Description(ExpectShaDoc)] string? expect_input_sha256 = null)
        => Run<object>(() =>
        {
            GamedataExportResult? r = null;
            bool zipped = out_path.EndsWith(".zip", StringComparison.OrdinalIgnoreCase);
            FileWriteOutcome w = zipped
                ? WriteGate.WriteFile(out_path, Inputs(schemas_dir), overwrite, expect_input_sha256,
                    temp => r = GamedataExportSchemasCommand.Execute(temp, schemas_dir))
                : WriteGate.WriteFolder(out_path, Inputs(schemas_dir), overwrite, expect_input_sha256,
                    temp => r = GamedataExportSchemasCommand.Execute(temp, schemas_dir));
            var gated = Gated(r!, w);
            gated["target"] = w.SavedTo;
            return gated;
        });
    /// <summary>
    /// A tool's game_dir argument, else WC3_GAME_DIR, else null (auto-detect). A value still holding
    /// an unfilled placeholder such as ${user_config.game_dir}, which an MCPB host may pass through
    /// when the optional setting was left empty, counts as unset.
    /// </summary>
    internal static string? ResolveGameDir(string? game_dir) =>
        IsSet(game_dir) ? game_dir
        : Environment.GetEnvironmentVariable(GameDirVariable) is var env && IsSet(env) ? env
        : null;

    /// <summary>Resolve the target build: use the explicit value or fall through to settings.</summary>
    internal static string ResolveTarget(string? target, string? schemas_dir)
    {
        if (target is not null)
            return target;
        var settings = TargetBuildSettings.Get(option: null, schemasDir: schemas_dir);
        return settings.Build;
    }

    private static bool IsSet(string? value) =>
        !string.IsNullOrWhiteSpace(value) && !value.Contains("${", StringComparison.Ordinal);

    // ---- target_* result POCOs ------------------------------------------

    /// <summary>Target get outcome: the build, its source, and the installed game build.</summary>
    public sealed record TargetGetResult(string Build, string Source, string? InstallBuild, IReadOnlyList<string> Sections);

    /// <summary>target_set outcome, whether the build validated and the settings file was written,
    /// where it landed, and the digests and size a caller can check (SAF-06).</summary>
    public sealed record TargetSetResult(
        bool Ok, string Message, string? SavedTo,
        string? InputSha256 = null, string OutputSha256 = "", int FilesRewritten = 0, long BytesWritten = 0);
}

/// <summary>render_model outcome: where the PNG landed and whether it was also inlined.</summary>
public sealed record RenderModelToolResult(string SavedTo, int SizeBytes, bool ImageInline,
    string? InputSha256 = null, string OutputSha256 = "", int FilesRewritten = 0, long BytesWritten = 0);

/// <summary>asset_thumbnails outcome: where the PNGs landed, how many, their names and any skips.</summary>
public sealed record AssetThumbnailsToolResult(
    string OutDir, int Rendered, IReadOnlyList<string> Files, IReadOnlyList<string> Diagnostics,
    string? InputSha256 = null, string OutputSha256 = "", int FilesRewritten = 0, long BytesWritten = 0);

/// <summary>mpq_extract_all outcome: where the entries landed, the counts, and the relative file names.</summary>
public sealed record MpqExtractAllToolResult(
    string OutDir, int TotalEntries, int NamedEntries, int UnnamedEntries, long TotalBytes,
    IReadOnlyList<string> Files,
    string? InputSha256 = null, string OutputSha256 = "", int FilesRewritten = 0, long BytesWritten = 0);

/// <summary>ERR-05. One item a batch skipped, named, with the reason it was skipped.</summary>
public sealed record SkippedItem(string Item, string Reason);

/// <summary>asset_convert_batch outcome: where the copies landed, how many, the target extension,
/// their relative paths, any skips, and each skipped file with its reason. Partial is true when
/// anything was skipped, so a run that converted some of a folder is not reported as success.</summary>
public sealed record AssetConvertBatchToolResult(
    string OutDir, int Converted, string To, IReadOnlyList<string> Files, IReadOnlyList<string> Diagnostics,
    string? InputSha256 = null, string OutputSha256 = "", int FilesRewritten = 0, long BytesWritten = 0,
    IReadOnlyList<SkippedItem>? Failed = null, bool Partial = false);

/// <summary>asset_import_wire outcome: where the edited copy landed, the imported paths, the
/// fields set and any skips.</summary>
public sealed record AssetImportWireToolResult(
    string SavedTo, IReadOnlyList<string> Imported, IReadOnlyList<string> FieldsSet,
    IReadOnlyList<string> Diagnostics,
    string? InputSha256 = null, string OutputSha256 = "", int FilesRewritten = 0, long BytesWritten = 0);

/// <summary>model_export_obj outcome: where the .obj landed, its .mtl path and the texture names.</summary>
public sealed record ModelExportObjToolResult(
    string SavedTo, string Mtl, IReadOnlyList<string> Textures, IReadOnlyList<string> Diagnostics);

/// <summary>model_import outcome: where the .mdx landed, the counts it holds, the sequence count and
/// names, and the texture files written.</summary>
public sealed record ModelImportToolResult(
    string SavedTo, int Geosets, int Vertices, int Faces, int Textures, int Materials,
    int Sequences, IReadOnlyList<string> SequenceNames, IReadOnlyList<string> TextureFiles);

/// <summary>port_unit outcome: where the new .ported map was written plus the full port report.</summary>
public sealed record PortUnitToolResult(string SavedTo, PortResult Report,
    string? InputSha256 = null, string OutputSha256 = "", int FilesRewritten = 0, long BytesWritten = 0,
    IReadOnlyList<OutputFile>? Files = null);

/// <summary>port_into_version outcome: where the downgraded map was written plus the report.</summary>
public sealed record PortIntoVersionToolResult(string SavedTo, PortIntoVersionResult Report,
    string? InputSha256 = null, string OutputSha256 = "", int FilesRewritten = 0, long BytesWritten = 0,
    IReadOnlyList<OutputFile>? Files = null);

/// <summary>bundle_merge outcome: where the merged bundle was written plus the merge result.</summary>
public sealed record BundleMergeToolResult(string SavedTo, BundleMergeResult Merge);

/// <summary>place_doodad / place_region outcome: where the edited map was written, a
/// human-readable message, and the creation number assigned to the new instance.</summary>
public sealed record PlacementToolResult(string SavedTo, string Message, int CreationNumber,
    string? InputSha256 = null, string OutputSha256 = "", int FilesRewritten = 0, long BytesWritten = 0);

/// <summary>SUR-08. The placement half of <c>place_unit</c> as one object, so the tool keeps five
/// top-level arguments instead of eleven. Every field carries its own description and its own
/// default, which is what the schema a caller reads is built from. The flattened arguments the
/// tool shipped with stay on the tool for one version and are refused alongside this object.</summary>
public sealed record UnitPlacement(
    [property: Description("Four-character unit type rawcode, e.g. 'hfoo' or 'u000'.")] string TypeRawcode,
    [property: Description("Owning player id, 0-based, where 0 is red. Neutral players use the high ids, for example 24 is neutral hostile.")] int OwnerId,
    [property: Description("World X coordinate.")] float X,
    [property: Description("World Y coordinate.")] float Y,
    [property: Description("Height above ground in world units. Default 0.")] float Z = 0f,
    [property: Description("Facing angle in radians. Default 0.")] float Rotation = 0f,
    [property: Description("Uniform scale. Default 1.")] float Scale = 1f);

/// <summary>SUR-08. The placement half of <c>place_doodad</c> as one object. Same contract as
/// <see cref="UnitPlacement"/>, with the variation index a doodad carries in place of an owner.</summary>
public sealed record DoodadPlacement(
    [property: Description("Four-character doodad type rawcode from palette_doodad, e.g. 'ATtr'.")] string TypeRawcode,
    [property: Description("World X coordinate.")] float X,
    [property: Description("World Y coordinate.")] float Y,
    [property: Description("World Z height offset. Default 0.")] float Z = 0f,
    [property: Description("Facing angle in radians. Default 0.")] float Rotation = 0f,
    [property: Description("Uniform scale. Default 1.")] float Scale = 1f,
    [property: Description("Doodad variation index. Default 0.")] int Variation = 0);

/// <summary>camera_tour outcome, where the edited map was written, the generated camera names
/// in order, and the camera total after the op.</summary>
public sealed record CameraTourToolResult(
    string SavedTo, string Message, IReadOnlyList<string> Names, int Count,
    string? InputSha256 = null, string OutputSha256 = "", int FilesRewritten = 0, long BytesWritten = 0);

/// <summary>place_grid / place_scatter outcome, where the edited map was written, how many
/// copies were placed, and each copy's creation number in placement order.</summary>
public sealed record BulkPlacementToolResult(
    string SavedTo, string Message, int Placed, IReadOnlyList<int> CreationNumbers,
    string? InputSha256 = null, string OutputSha256 = "", int FilesRewritten = 0, long BytesWritten = 0);

/// <summary>terrain_* edit outcome: where the edited map was written, a message, and tiles changed.</summary>
public sealed record TerrainToolResult(
    string SavedTo, string Message, int TilesChanged,
    string? InputSha256 = null, string OutputSha256 = "", int FilesRewritten = 0, long BytesWritten = 0);

/// <summary>terrain_export_heightmap outcome: where the PNG landed, its grid, and its byte size.</summary>
public sealed record ImageExportToolResult(string SavedTo, string Message, int Width, int Height, int SizeBytes,
    string? InputSha256 = null, string OutputSha256 = "", int FilesRewritten = 0, long BytesWritten = 0);

/// <summary>pathing_export_png outcome: where the PNG landed, its grid and size, and the flag legend.</summary>
public sealed record PathingExportToolResult(
    string SavedTo, string Message, int Width, int Height, int SizeBytes,
    IReadOnlyList<PathingExportPngCommand.LegendEntry> Legend,
    string? InputSha256 = null, string OutputSha256 = "", int FilesRewritten = 0, long BytesWritten = 0);

/// <summary>object_set outcome: where the edited map was written, its digests, what was rewritten, a message, and any non-fatal warning.</summary>
public sealed record ObjectSetToolResult(
    string SavedTo, string? InputSha256, string OutputSha256, int FilesRewritten, long BytesWritten,
    string Message, string? Warning);

/// <summary>One edit in an object_batch_set request. Kind defaults to unit.</summary>
public sealed record BatchEditInput(string Rawcode, string Field, string? Value, string Kind = "unit");

/// <summary>object_batch_set outcome: where the map was written, its digests, how many edits
/// applied, the message and any non-fatal warnings.</summary>
public sealed record ObjectBatchSetToolResult(
    string SavedTo, string? InputSha256, string OutputSha256, int FilesRewritten, long BytesWritten,
    int Applied, string Message, string[] Warnings);

/// <summary>object_new outcome: where the edited map was written, a message, and the newly allocated rawcode.</summary>
public sealed record ObjectNewToolResult(string SavedTo, string Message, string? NewRawcode,
    string? InputSha256 = null, string OutputSha256 = "", int FilesRewritten = 0, long BytesWritten = 0);

/// <summary>object_clone outcome: where the edited map was written, a message, the new
/// rawcode, and how many field values the clone received.</summary>
public sealed record ObjectCloneToolResult(string SavedTo, string Message, string? NewRawcode, int FieldsCopied,
    string? InputSha256 = null, string OutputSha256 = "", int FilesRewritten = 0, long BytesWritten = 0);

/// <summary>object_migrate outcome: the saved map, the target build, and the field counts.</summary>
public sealed record ObjectMigrateToolResult(
    string SavedTo, string Message, string TargetBuild, int FieldsKept, int FieldsDropped, int Drops,
    string? InputSha256 = null, string OutputSha256 = "", int FilesRewritten = 0, long BytesWritten = 0);

/// <summary>object_bulk_set outcome: the saved map, how many objects matched and changed, and each
/// object the write skipped with its reason. Partial is true when anything was skipped, so a run
/// that changed some of the matches is not reported as success.</summary>
public sealed record ObjectBulkSetToolResult(
    string SavedTo, string? InputSha256, string OutputSha256, int FilesRewritten, long BytesWritten,
    string Message, string Kind, int Matched, int Changed, int Skipped,
    IReadOnlyList<SkippedItem> Failed, bool Partial);

/// <summary>object_export outcome: the CSV path, the kind, and the row and column counts.</summary>
public sealed record ObjectExportToolResult(
    string CsvPath, string Message, string Kind, int ObjectCount, int Columns);

/// <summary>object_import outcome: the saved map and the added, updated and field counts.</summary>
public sealed record ObjectImportToolResult(
    string SavedTo, string Message, string Kind, int Added, int Updated, int FieldsSet,
    string? InputSha256 = null, string OutputSha256 = "", int FilesRewritten = 0, long BytesWritten = 0);

/// <summary>object_rename outcome: the saved map, the old and new rawcodes, and the reference counts.</summary>
public sealed record ObjectRenameToolResult(
    string SavedTo, string Message, string Kind, string OldRawcode, string NewRawcode,
    int FieldsRewritten, int ScriptLiteralsRewritten,
    string? InputSha256 = null, string OutputSha256 = "", int FilesRewritten = 0, long BytesWritten = 0);

/// <summary>sound_* write outcome: where the edited map was written plus a human-readable message.</summary>
/// <summary>sound_import_events outcome: where the map landed, its digests, how many events and
/// catalog entries were written, and the generated dispatcher text.</summary>
public sealed record SoundImportToolResult(
    string SavedTo, string? InputSha256, string OutputSha256, string Message,
    int Events, int CatalogEntries, IReadOnlyList<string> MissingClips,
    IReadOnlyList<SoundImportRefusal> Refused, string Jass);

public sealed record SoundToolResult(string SavedTo, string Message,
    string? InputSha256 = null, string OutputSha256 = "", int FilesRewritten = 0, long BytesWritten = 0);

/// <summary>One file a multi file write published, named by the role it plays, so a caller can
/// tell the primary output from its companions and check each digest (RES-11).</summary>
public sealed record OutputFile(string Role, string Path, string Sha256);

/// <summary>file_get_text outcome. Text is the decoded payload, Encoding is what was used,
/// EncodingDeclared says a byte order mark declared it rather than it being assumed, and
/// BytesReplaced with Lossy reports a decode that had to substitute bytes (RES-04).</summary>
public sealed record FileTextResult(
    string Name, string Text, string Encoding = "utf-8", bool EncodingDeclared = true,
    int BytesReplaced = 0, bool Lossy = false);

/// <summary>file_extract outcome. SavedTo is the file written, Name the internal entry, and
/// Sha256 the digest of the bytes as they landed (RES-04).</summary>
public sealed record FileExtractResult(string SavedTo, string Name, int SizeBytes, string Sha256,
    string? InputSha256 = null, int FilesRewritten = 0, long BytesWritten = 0);

/// <summary>file_set outcome. Replaced is true when an existing entry's payload was
/// superseded and false when a new entry was added, which the caller cannot otherwise tell.</summary>
public sealed record FileSetResult(string SavedTo, string Name, bool Replaced, int SizeBytes,
    string? InputSha256 = null, string OutputSha256 = "", int FilesRewritten = 0, long BytesWritten = 0);

/// <summary>Generic write outcome for camera/pathing/map-info/player/force/new tools:
/// where the edited (or created) map was written, the digests and counts where a write gate ran,
/// and a human-readable message.</summary>
public sealed record EditToolResult(
    string SavedTo, string Message,
    string? InputSha256 = null, string OutputSha256 = "", int FilesRewritten = 0, long BytesWritten = 0);

/// <summary>
/// RES-10. The page a capped collection returns. Every paged result names the page size
/// <c>returnedCount</c>, the size of the whole list <c>totalCount</c>, whether anything is left
/// <c>hasMore</c> and where a next page would start <c>nextOffset</c>, and never a bare
/// <c>count</c>. This is the shape the twelve <c>UseStructuredContent</c> list tools return.
/// <para>
/// The last three fields are the names those twelve tools served before this version, kept for one
/// minor release so a caller that already reads them keeps working. <c>truncated</c> is the same
/// fact as <c>hasMore</c> and <c>total</c> the same fact as <c>totalCount</c>; <c>cap</c> is how
/// many items the result may carry. A tool with no offset argument reports
/// <c>nextOffset</c> as the index the next page would start at, so a caller learns where the cut
/// fell even though this version offers no argument to ask for it.
/// </para>
/// </summary>
public sealed record CappedPage<T>(
    int ReturnedCount, int TotalCount, bool HasMore, int? NextOffset,
    IReadOnlyList<T> Items, int Cap, bool Truncated, int Total);

/// <summary>RES-10. The <c>list_files</c> page. The array keeps the name it has always had,
/// <c>files</c>, and the counts use the names every paged result uses.</summary>
public sealed record PagedFiles(
    int ReturnedCount, int TotalCount, bool HasMore, int? NextOffset,
    IReadOnlyList<FileEntryInfo> Files, bool Truncated, int Total);

/// <summary>RES-10. The <c>object_list</c> page, the same shape as <see cref="PagedFiles"/> with
/// the array under <c>items</c>.</summary>
public sealed record PagedObjects(
    int ReturnedCount, int TotalCount, bool HasMore, int? NextOffset,
    IReadOnlyList<ObjectListItem> Items, bool Truncated, int Total);

/// <summary>RES-10. The <c>placed_units_list</c> page, the same shape with the array under
/// <c>units</c>.</summary>
public sealed record PagedUnits(
    int ReturnedCount, int TotalCount, bool HasMore, int? NextOffset,
    IReadOnlyList<UnitInstanceInfo> Units, bool Truncated, int Total);

/// <summary>The quickstart tool's answer. A read tool returns an object, not a bare array, so the
/// workflows ride under items with the RES-10 page names beside them. The twelve workflows are
/// always returned whole, so <c>hasMore</c> is false and <c>nextOffset</c> is null. The <c>count</c>
/// field this tool used to serve is gone rather than aliased, because RES-10 forbids a bare
/// <c>count</c> on a page and this is the tool the CMP-04 renames list records the removal for.
/// </summary>
public sealed record QuickstartPage(
    int ReturnedCount, int TotalCount, bool HasMore, int? NextOffset,
    IReadOnlyList<QuickstartWorkflow> Items);
