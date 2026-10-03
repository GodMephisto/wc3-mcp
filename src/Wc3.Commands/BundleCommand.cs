// src/Wc3.Commands/BundleCommand.cs
using System.Text;
using System.Text.RegularExpressions;
using War3Net.Common.Extensions;
using Wc3.GameData;
using Wc3.Model;
using Wc3.Modeling;

namespace Wc3.Commands;

/// <summary>
/// Computes the full dependency closure of one object (rooted at any of the seven
/// Object Editor kinds): every object it references (abilities, buffs, items,
/// upgrades, ...), every asset file (models, textures, icons, sounds) and every
/// trigger string, so a later wave can graph the result and port it into another
/// map. Breadth-first reference crawl over merged object fields: 4-char tokens
/// that resolve as objects become nodes, path-like tokens become file deps, and
/// map-imported models contribute their textures. With game data available, only
/// fields whose metadata TYPE is an object-reference type (unitCode, abilList, ...)
/// may add object nodes — a value that merely looks like a rawcode elsewhere is a
/// false positive; without game data the crawl stays permissive (any 4-char token
/// is a candidate). Only map-defined (custom) objects are recursed — base-game
/// objects are recorded as leaf nodes since they already exist in any target map.
/// </summary>
public static class BundleCommand
{
    /// <summary>Safety cap on discovered objects (cycles are handled separately).</summary>
    private const int MaxNodes = 5000;

    private static readonly string[] AssetExtensions =
        { ".mdx", ".mdl", ".blp", ".tga", ".dds", ".mp3", ".wav", ".flac" };

    /// <summary>Unit-rooted closure (back-compat shorthand for <see cref="ResolveObject(MapDocument, ObjectKind, string, string?)"/>).</summary>
    public static UnitBundle ResolveUnit(MapDocument doc, string rootRawcode, string? gameDirOverride) =>
        ResolveObject(doc, ObjectKind.Unit, rootRawcode, gameDirOverride);

    /// <summary>Unit-rooted closure with an explicit game-data context (back-compat).</summary>
    internal static UnitBundle ResolveUnit(
        MapDocument doc, string rootRawcode, GameDataContext? ctx, IReadOnlyList<string> preDiagnostics) =>
        ResolveObject(doc, ObjectKind.Unit, rootRawcode, ctx, preDiagnostics);

    /// <summary>Closure rooted at an object of the given kind. The result is a
    /// <see cref="UnitBundle"/> for historical reasons — it is a generic object
    /// bundle and covers any root kind.</summary>
    public static UnitBundle ResolveObject(
        MapDocument doc, ObjectKind rootKind, string rootRawcode, string? gameDirOverride)
    {
        return GameData.GameData.TryOpen(gameDirOverride, out var ctx, out var diagnostic)
            ? ResolveObject(doc, rootKind, rootRawcode, ctx, Array.Empty<string>())
            : ResolveObject(doc, rootKind, rootRawcode, null, new[] { diagnostic });
    }

    /// <summary>Core with an optional game-data context (null = map deltas only:
    /// base-game references cannot resolve and are silently skipped).</summary>
    internal static UnitBundle ResolveObject(
        MapDocument doc, ObjectKind rootKind, string rootRawcode, GameDataContext? ctx,
        IReadOnlyList<string> preDiagnostics)
    {
        var diagnostics = new List<string>(preDiagnostics);
        var strings = MapStrings.From(doc);

        // Per-kind rawcode ids the map itself defines/modifies — the "custom to
        // this map" test (custom objects need porting; base ones exist anywhere).
        var mapIds = new Dictionary<ObjectKind, HashSet<int>>();
        foreach (var kind in ObjectKinds.All)
            mapIds[kind] = ObjectKinds.MergedEntries(doc, ObjectKinds.Info(kind)).Select(e => e.Id).ToHashSet();

        var nodes = new Dictionary<string, BundleNode>(StringComparer.Ordinal);
        var files = new Dictionary<string, BundleFile>(StringComparer.Ordinal); // key = normalized path
        var stringSet = new SortedSet<string>(StringComparer.Ordinal);
        var edges = new List<BundleEdge>();
        var edgeSeen = new HashSet<(string, string, string)>();
        var queue = new Queue<(string Rawcode, MergedObjectResult Merged)>();
        bool capped = false;

        bool IsCustom(ObjectKind kind, string rawcode) => mapIds[kind].Contains(rawcode.FromRawcode());

        // Per (kind, field-code) "may this field's value reference objects?" — memoized
        // because the metadata probe also enumerates the store's distinct field values.
        var refFieldCache = new Dictionary<(ObjectKind Kind, string Code), bool>();
        bool IsReferenceField(ObjectKind kind, string fieldCode)
        {
            // No game data → field types are unknowable; keep the historical permissive
            // crawl (any 4-char token is a candidate) rather than regress that path.
            if (ctx is null) return true;
            if (refFieldCache.TryGetValue((kind, fieldCode), out var known)) return known;
            bool isRef = ObjectKinds.TryGetFieldOptions(ctx, kind, fieldCode, out var type, out _, out _)
                && IsObjectReferenceType(type);
            refFieldCache[(kind, fieldCode)] = isRef;
            return isRef;
        }

        void AddEdge(string from, string to, string via)
        {
            if (edgeSeen.Add((from, to, via))) edges.Add(new BundleEdge(from, to, via));
        }

        // Map-defined entries of any kind win over base-game stores, mirroring
        // ObjectGetCommand's kind-agnostic probe but keeping the matching kind.
        (ObjectKind Kind, MergedObjectResult Merged)? Resolve(string rawcode)
        {
            int id = rawcode.FromRawcode();
            foreach (var kind in ObjectKinds.All)
                if (mapIds[kind].Contains(id))
                    return (kind, ObjectGetCommand.Execute(doc, kind, rawcode, ctx, Array.Empty<string>()));
            if (ctx is not null)
                foreach (var kind in ObjectKinds.All)
                    if (ObjectKinds.TryGetBaseFields(ctx, kind, rawcode, out _))
                        return (kind, ObjectGetCommand.Execute(doc, kind, rawcode, ctx, Array.Empty<string>()));
            return null;
        }

        void AddObjectRef(string from, string token, string via)
        {
            if (nodes.ContainsKey(token)) { AddEdge(from, token, via); return; }
            if (nodes.Count >= MaxNodes) { capped = true; return; }
            if (Resolve(token) is not { } hit) return; // not an object — a false-positive token
            bool custom = IsCustom(hit.Kind, token);
            nodes[token] = new BundleNode(token, hit.Kind, hit.Merged.Name, custom);
            AddEdge(from, token, via);
            // Base-game objects are leaves: whatever THEY reference is also base.
            if (custom) queue.Enqueue((token, hit.Merged));
        }

        void AddFileRef(string from, string path, string via)
        {
            var key = NormalizePath(path);
            if (!files.TryGetValue(key, out var file))
            {
                // One universal resolver for every asset kind. It matches the file the map really
                // stores whatever the reference spelling, a model, texture, icon or sound, with or
                // without an extension, either slash, any case. The category comes from the RESOLVED
                // file (so an extensionless "...\BTNFoo" that lands on ...BTNFoo.blp reads as an
                // icon, and "...\Hero_Foo_Q" that lands on ....mp3 reads as a sound), and falls back
                // to the reference spelling only when nothing in the map backs it (a base-game asset).
                var entry = RenderModelCommand.FindAssetEntry(doc, path)
                    ?? FindFileEntry(doc, path);
                bool present = entry is not null;
                var category = Categorize(entry?.FileName ?? path);
                file = new BundleFile(path, category, present);
                files[key] = file;
                if (category == "model" && present) AddModelTextures(path);
            }
            AddEdge(from, file.Path, via); // first-seen spelling keeps edges consistent
        }

        void AddModelTextures(string modelPath)
        {
            var entry = RenderModelCommand.FindModelEntry(doc, modelPath);
            if (entry?.FileName is null) return;
            try
            {
                var model = ModelParser.Parse(entry.CurrentBytes, entry.FileName);
                foreach (var texture in model.Textures)
                {
                    if (string.IsNullOrWhiteSpace(texture)
                        || texture.StartsWith("ReplaceableId:", StringComparison.OrdinalIgnoreCase))
                        continue;
                    AddFileRef(modelPath, texture, "texture");
                }
            }
            catch (Exception ex)
            {
                diagnostics.Add($"could not read textures of model '{modelPath}': {ex.Message}");
            }
        }

        void ScanField(ObjectKind kind, string from, MergedField field)
        {
            var value = field.Value;
            if (string.IsNullOrWhiteSpace(value)) return;

            if (value.Contains("TRIGSTR_", StringComparison.Ordinal))
            {
                var resolved = strings.Resolve(value.Trim());
                if (resolved.Length > 0 && !resolved.StartsWith("TRIGSTR_", StringComparison.Ordinal))
                {
                    stringSet.Add(resolved);
                    // Record WHICH object wants this string. The list itself stays flat (the porter
                    // inlines all of it), but a front end needs the owner to tell the root's own
                    // tooltips apart from the closure's, exactly as it does for files.
                    AddEdge(from, resolved, BundleStructure.StringVia);
                }
            }

            // Cross-references are 4-char rawcodes or comma-separated lists of them —
            // but only in fields whose metadata type says they hold object references;
            // asset references are file-path strings (variation fields may list several).
            // The field code rides on every edge as its Via (the WHY of the inclusion).
            bool mayReferenceObjects = IsReferenceField(kind, field.Code);
            foreach (var raw in value.Split(','))
            {
                var token = raw.Trim();
                if (token.Length == 0) continue;
                if (LooksLikeAssetPath(token)) AddFileRef(from, token, field.Code);
                else if (mayReferenceObjects && token.Length == 4 && token.All(c => c is >= ' ' and <= '~'))
                    AddObjectRef(from, token, field.Code);
            }
        }

        // Root: the caller-declared kind (the crawl below stays kind-agnostic).
        var kindWord = rootKind.ToString().ToLowerInvariant();
        var root = ObjectGetCommand.Execute(doc, rootKind, rootRawcode, ctx, Array.Empty<string>());
        if (!root.Found)
        {
            diagnostics.Add($"root {kindWord} '{rootRawcode}' not found in the map"
                + (ctx is null ? " (game data unavailable — base objects unresolvable)" : " or game data"));
            return new UnitBundle(rootRawcode, null, Array.Empty<BundleNode>(), Array.Empty<BundleFile>(),
                Array.Empty<string>(), Array.Empty<BundleEdge>(), diagnostics, Array.Empty<BundleFunction>());
        }

        bool rootCustom = rootRawcode.Length == 4 && IsCustom(rootKind, rootRawcode);
        nodes[rootRawcode] = new BundleNode(rootRawcode, rootKind, root.Name, rootCustom);
        if (rootCustom) queue.Enqueue((rootRawcode, root));
        else diagnostics.Add($"root {kindWord} '{rootRawcode}' is a base-game {kindWord} — nothing custom to port");

        void Drain()
        {
            while (queue.Count > 0)
            {
                var (rawcode, merged) = queue.Dequeue();
                var kind = nodes[rawcode].Kind; // enqueue always records the node first
                foreach (var field in merged.Fields)
                    ScanField(kind, rawcode, field);
            }
        }
        Drain();

        if (capped) diagnostics.Add($"node cap ({MaxNodes}) reached — dependency closure truncated");

        // Script closure: the war3map.j functions that implement the bundle's custom
        // skills. Seeded by every rawcode being ported (root + custom objects) — those
        // appear in JASS as 'XXXX' literals in spell-handler conditions and the like.
        var seedRawcodes = nodes.Values.Where(n => n.CustomToMap).Select(n => n.Rawcode)
            .Where(rc => rc != rootRawcode)
            .OrderBy(rc => rc, StringComparer.Ordinal)
            .Prepend(rootRawcode)
            .ToList();
        // Every custom object id in the map (any kind), so the script closure can tell a
        // dispatcher branch guarded by ANOTHER hero (a foreign custom object) from one guarded
        // by the object being ported, and refuse to follow the foreign branches.
        var allCustomIds = mapIds.Values.SelectMany(s => s).ToHashSet();
        var functions = ResolveScriptClosure(doc, seedRawcodes, allCustomIds, diagnostics, AddFileRef,
            rc => AddObjectRef(rootRawcode, rc, "script closure"));
        // The script pass may have carried objects referenced only in JASS (an ability added by
        // UnitAddAbility, never on the hero). They are new nodes but not yet crawled for their own
        // field references, so drain the queue once more to complete their closure.
        Drain();

        return new UnitBundle(
            rootRawcode,
            root.Name,
            nodes.Values.OrderBy(n => n.Kind).ThenBy(n => n.Rawcode, StringComparer.Ordinal).ToList(),
            files.Values.OrderBy(f => f.Path, StringComparer.Ordinal).ToList(),
            stringSet.ToList(),
            edges.OrderBy(e => e.From, StringComparer.Ordinal)
                 .ThenBy(e => e.To, StringComparer.Ordinal)
                 .ThenBy(e => e.Via, StringComparer.Ordinal).ToList(),
            diagnostics,
            functions);
    }

    /// <summary>Safety cap on script-closure functions (a real war3map.j tops out around
    /// a few thousand — beyond that the seed heuristic has almost certainly run away).</summary>
    private const int MaxFunctions = 4000;

    /// <summary>Safety cap on objects carried because a handler spawns or grants them. A hero's kit
    /// spawns on the order of tens of dummy types, so a far larger count means a shared dispatcher's
    /// spawns leaked in, and the cap truncates loudly (a diagnostic) rather than running away.</summary>
    private const int MaxScriptSpawnedObjects = 512;

    /// <summary>Above this many DISTINCT objects granted or spawned by one function, that function is
    /// a roster registry rather than a hero's handler, and its references say nothing about which
    /// hero we are porting. Measured on Anime_WOS2, MyHeroIdInit grants 183 abilities onto pick
    /// screen dummies while a real handler grants a handful, so the gap either side of this is
    /// enormous. The existing branch-guard scoping already excludes a shared if/elseif dispatcher's
    /// other-hero spawns, this catches the other shape, a flat loop over the whole roster.</summary>
    private const int RosterRegistryGrantCount = 24;

    /// <summary>Above this many DISTINCT asset paths named in one function, that function is a
    /// shared asset bank (a pick screen, a hero guide, a random-pick roller) rather than one hero's
    /// spell handler, so its models, textures and sounds belong to the whole roster. Measured on
    /// Anime_WOS2, OnClick names 155 assets and RandomPick 30, while Asta's busiest own handler
    /// names 12. Same shape and same justification as RosterRegistryGrantCount, and both are why a
    /// port of one hero stops importing every hero's art.</summary>
    internal const int SharedAssetBankCount = 24;

    private static readonly Regex Identifier = new(@"[A-Za-z_][A-Za-z0-9_]*", RegexOptions.Compiled);

    /// <summary>A double-quoted JASS string literal (captures the inner text). Used to pull
    /// asset paths out of spell handlers - AddSpecialEffect("war3mapImported\\x.mdx") and the
    /// like - so trigger-driven skill models get carried by the port.</summary>
    private static readonly Regex StringLiteral = new("\"([^\"]*)\"", RegexOptions.Compiled);

    /// <summary>A single-quoted four-character rawcode literal, e.g. 'H001'. Used to read the
    /// hero/ability id a dispatcher branch is guarded by ("if GetUnitTypeId(c) == 'H001'").</summary>
    private static readonly Regex RawcodeLiteral = new("'([^']{4})'", RegexOptions.Compiled);

    /// <summary>An ExecuteFunc("Name") native call, which runs the function named by the string.
    /// The name sits inside a string literal, so it is never a normal call token and would be
    /// missed by the call detector, dropping the function from the closure.</summary>
    private static readonly Regex ExecuteFuncCall =
        new("ExecuteFunc\\s*\\(\\s*\"([^\"]+)\"", RegexOptions.Compiled);

    /// <summary>A runtime spawn or grant whose argument names an object by rawcode, the unit type of
    /// a CreateUnit family call or the ability of a UnitAddAbility. Only the type or ability argument
    /// of one of these is read, so a bare rawcode elsewhere on a line is never mistaken for a spawn.
    /// The match ends at the opening paren, the argument list is read from there.</summary>
    private static readonly Regex SpawnGrantCall = new(
        @"\b(?:CreateUnit|CreateUnitAtLoc|CreateUnitAtLocSaveLast|CreateNUnitsAtLoc"
        + @"|CreateNUnitsAtLocFacingLocBJ|UnitAddAbility|UnitAddAbilityBJ)\s*\(",
        RegexOptions.Compiled);

    /// <summary>A global declaration with an initializer: "[constant] type Name = ...".</summary>
    private static readonly Regex GlobalInitializer =
        new(@"^\s*(?:constant\s+)?[A-Za-z_][A-Za-z0-9_]*\s+([A-Za-z_][A-Za-z0-9_]*)\s*=", RegexOptions.Compiled);

    private static string StripLineComment(string line)
    {
        int comment = line.IndexOf("//", StringComparison.Ordinal);
        return comment >= 0 ? line[..comment] : line;
    }

    /// <summary>
    /// JASS call-graph closure seeded by rawcode literals: every war3map.j function whose
    /// body mentions '&lt;seed rawcode&gt;' — directly, or through a global initialized
    /// with one (integer Raiden_ID= 'H000' makes any Raiden_ID reference a hit) — plus
    /// everything those functions transitively call. Best-effort text analysis on top of
    /// <see cref="JassFunctionIndex"/> — line comments are stripped, but /* */ block
    /// comments and string literals are not understood, so a rawcode or function name
    /// inside either still matches (over-inclusion, never under-inclusion). Lua maps
    /// are not analyzed.
    ///
    /// Dispatch scoping: these maps route every hero's spellcast through one shared function
    /// shaped as "if GetUnitTypeId(c) == Raiden_ID then ...Raiden calls... endif; if ... ==
    /// Natsu_ID then ...Natsu calls... endif; ...". A call that appears ONLY inside a branch
    /// guarded by another object's rawcode (<paramref name="allCustomObjectIds"/> minus the
    /// ported set) is that other hero's, not ours, so it is not followed. Branches guarded by
    /// our own rawcodes, and unguarded calls, are followed normally. Without this, one shared
    /// dispatcher pulls every hero's spell code (and assets) into every hero's bundle.
    /// </summary>
    private static IReadOnlyList<BundleFunction> ResolveScriptClosure(
        MapDocument doc, IReadOnlyList<string> seedRawcodes, HashSet<int> allCustomObjectIds,
        List<string> diagnostics, Action<string, string, string> addFileRef, Action<string> carryObject)
    {
        var entry = doc.GetFile("war3map.j") ?? doc.GetFile("scripts\\war3map.j");
        if (entry is null)
        {
            diagnostics.Add(doc.GetFile("war3map.lua") is not null
                ? "Lua scripts not analyzed (JASS only) — script closure skipped"
                : "map has no war3map.j — script closure skipped");
            return Array.Empty<BundleFunction>();
        }

        var source = ScriptText.GetString(entry.CurrentBytes);
        var index = JassFunctionIndex.Parse(source);
        if (index.Count == 0) return Array.Empty<BundleFunction>();

        var lines = source.Split('\n');
        var literals = seedRawcodes.Where(rc => rc.Length == 4)
            .Distinct(StringComparer.Ordinal).Select(rc => $"'{rc}'").ToList();

        // First declaration wins on duplicate names (illegal in JASS anyway).
        var byName = new Dictionary<string, JassFunction>(StringComparer.Ordinal);
        foreach (var f in index) byName.TryAdd(f.Name, f);

        // Rawcode aliases: maps commonly stash spell ids in globals and compare against
        // those, so a global whose initializer carries a seed literal (any line outside
        // every function body, e.g. "integer RaidenQ_ID= 'A000'") makes its name count
        // as a reference to that rawcode wherever a function mentions it.
        var inFunction = new bool[lines.Length];
        foreach (var f in index)
            for (int i = f.StartLine - 1; i < f.EndLine && i < lines.Length; i++)
                inFunction[i] = true;

        // ourIds = the rawcodes being ported (root + custom closure objects). A dispatch
        // branch guarded by one of these is ours to follow; one guarded by any OTHER custom
        // object (allCustomObjectIds minus these) belongs to a different hero.
        var ourIds = seedRawcodes.Where(rc => rc.Length == 4)
            .Select(rc => rc.FromRawcode()).ToHashSet();

        var aliases = new Dictionary<string, string>(StringComparer.Ordinal);  // name → 'XXXX' (seed only)
        var allAliases = new Dictionary<string, int>(StringComparer.Ordinal);  // name → rawcode id (every id global)
        for (int i = 0; i < lines.Length; i++)
        {
            if (inFunction[i]) continue;
            var line = StripLineComment(lines[i]);
            var g = GlobalInitializer.Match(line);
            if (g.Success && RawcodeLiteral.Match(line) is { Success: true } rl)
                allAliases.TryAdd(g.Groups[1].Value, rl.Groups[1].Value.FromRawcode());
            foreach (var lit in literals)
            {
                if (!line.Contains(lit, StringComparison.Ordinal)) continue;
                var m = GlobalInitializer.Match(line);
                if (m.Success) aliases.TryAdd(m.Groups[1].Value, lit);
            }
        }

        // True when an if/elseif condition dispatches on a foreign hero: it names a custom
        // object rawcode (literal or *_ID alias) that is NOT one of ours. A condition that
        // also names one of ours, or names no object at all, is not foreign.
        bool IsForeignGuard(string condition)
        {
            var refs = new List<int>();
            foreach (Match m in RawcodeLiteral.Matches(condition))
                refs.Add(m.Groups[1].Value.FromRawcode());
            foreach (Match m in Identifier.Matches(condition))
                if (allAliases.TryGetValue(m.Value, out var rc)) refs.Add(rc);
            if (refs.Count == 0 || refs.Any(ourIds.Contains)) return false;
            return refs.Any(allCustomObjectIds.Contains);
        }

        // Body text (signature line included) with // line comments stripped.
        var bodies = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var f in byName.Values)
        {
            var sb = new StringBuilder();
            for (int i = f.StartLine - 1; i < f.EndLine && i < lines.Length; i++)
                sb.Append(StripLineComment(lines[i])).Append('\n');
            bodies[f.Name] = sb.ToString();
        }

        // One line-by-line pass per function fills both the call graph and the alias mentions.
        // A call counts when the identifier names another indexed function and is either invoked
        // ("Foo(") or passed by reference ("function Foo" — TriggerAddAction/TimerStart/Condition
        // and friends). Calls that appear ONLY inside a foreign-hero dispatch branch are dropped
        // (guardStack tracks the open if/elseif branches; a call is followed only when no active
        // branch is foreign). Alias mentions, which drive SEEDING, are collected regardless of
        // guard so the dispatcher itself is still recognised as referencing our hero.
        // Carry a hero's script-added abilities (e.g. a Q dash-back added by UnitAddAbility, never on
        // the unit's ability list) by the arena's naming convention: each ability id sits in a global
        // named after the hero (DarkShiki_ID, DarkShikiQ_ID, DarkShikiQ2_ID, ...). From the root's own
        // id-global take the hero stem, then carry every custom sibling id-global's object. Scoped to
        // the hero's own naming group, so it never pulls another hero's abilities (a closure-wide
        // rawcode scan does — shared dispatchers name every hero, and each carried unit cascades).
        if (seedRawcodes.Count > 0)
        {
            int rootId = seedRawcodes[0].FromRawcode();
            var rootGlobal = allAliases.FirstOrDefault(kv => kv.Value == rootId).Key;
            if (rootGlobal is not null)
            {
                string stem = rootGlobal.EndsWith("_ID", StringComparison.OrdinalIgnoreCase)
                    ? rootGlobal[..^3] : rootGlobal;
                var seedIdSet = seedRawcodes.Where(rc => rc.Length == 4)
                    .Select(rc => rc.FromRawcode()).ToHashSet();
                if (stem.Length >= 4)
                    foreach (var kv in allAliases)
                        if (kv.Key.StartsWith(stem, StringComparison.Ordinal)
                            && allCustomObjectIds.Contains(kv.Value)
                            && !seedIdSet.Contains(kv.Value))
                            carryObject(kv.Value.ToRawcode());
            }
        }

        var callees = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        var aliasHits = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        var assetRefs = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        var spawnRefs = new Dictionary<string, List<string>>(StringComparer.Ordinal); // custom ids spawned/granted
        foreach (var f in byName.Values)
        {
            var calls = new List<string>();
            var mentions = new List<string>();
            var assetLits = new List<string>();  // asset paths named on our (non-foreign) lines
            var spawns = new List<string>();     // custom object rawcodes spawned/granted on our lines
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var guardStack = new List<bool>(); // one entry per open 'if', true = foreign branch

            for (int i = f.StartLine - 1; i < f.EndLine && i < lines.Length; i++)
            {
                var line = StripLineComment(lines[i]);
                var head = line.TrimStart();

                // Adjust the branch-guard stack from the leading control keyword BEFORE scanning
                // this line's calls (so an inline "then call Foo()" is judged under its guard).
                if (StartsWithWord(head, "elseif"))
                {
                    if (guardStack.Count > 0) guardStack[^1] = IsForeignGuard(line);
                }
                else if (StartsWithWord(head, "else"))
                {
                    if (guardStack.Count > 0) guardStack[^1] = false; // else branch is not a foreign hero
                }
                else if (StartsWithWord(head, "endif"))
                {
                    if (guardStack.Count > 0) guardStack.RemoveAt(guardStack.Count - 1);
                }
                else if (StartsWithWord(head, "if"))
                {
                    guardStack.Add(IsForeignGuard(line));
                }

                bool foreign = guardStack.Contains(true);

                Match? prev = null;
                foreach (Match m in Identifier.Matches(line))
                {
                    if (m.Value != f.Name && byName.ContainsKey(m.Value)
                        && (FollowedByOpenParen(line, m) || IsFunctionReference(line, prev, m)))
                    {
                        if (!foreign && seen.Add(m.Value)) calls.Add(m.Value);
                    }
                    else if (aliases.ContainsKey(m.Value) && seen.Add("'" + m.Value))
                    {
                        mentions.Add(m.Value);
                    }
                    prev = m;
                }

                // ExecuteFunc("Name") dispatches to a function by name, so the target sits in a
                // string literal and is not a normal call token. Feed it into the same call set,
                // under the same branch-guard scoping (a foreign branch's dispatch is not ours).
                if (!foreign)
                    foreach (Match ef in ExecuteFuncCall.Matches(line))
                    {
                        var target = ef.Groups[1].Value;
                        if (target != f.Name && byName.ContainsKey(target) && seen.Add(target))
                            calls.Add(target);
                    }

                // Asset paths (AddSpecialEffect/MakeSound literals) are only ours when the line is
                // not inside a foreign hero's dispatch branch. A shared death or effect dispatcher
                // lists every hero's models/sounds in elseif branches, so without this scoping the
                // bundle drags all of them in.
                if (!foreign)
                    foreach (Match sl in StringLiteral.Matches(line))
                    {
                        var path = AssetPathCandidates.Unescape(sl.Groups[1].Value);
                        if (LooksLikeAssetPath(path) && seen.Add("$" + path)) assetLits.Add(path);
                    }

                // Objects the handler SPAWNS (CreateUnit and kin) or GRANTS (UnitAddAbility) at
                // runtime, named by rawcode in the call's argument, not in the hero's object data.
                // Read only the type/ability argument of the spawn/grant call, and only on our
                // (non-foreign) branches, so a shared dispatcher's other-hero spawns stay out.
                if (!foreign)
                    foreach (Match sg in SpawnGrantCall.Matches(line))
                    {
                        var args = CallArgs(line, sg.Index + sg.Length - 1);
                        foreach (int rc in ArgRawcodeIds(args, allAliases))
                            if (allCustomObjectIds.Contains(rc) && seen.Add("@" + rc))
                                spawns.Add(rc.ToRawcode());
                    }
            }
            callees[f.Name] = calls;
            aliasHits[f.Name] = mentions;
            assetRefs[f.Name] = assetLits;
            spawnRefs[f.Name] = spawns;
        }

        // Seeds: functions referencing any ported rawcode — as a literal or via an alias.
        var reasons = new Dictionary<string, string>(StringComparer.Ordinal);
        var queue = new Queue<string>();
        bool capped = false;
        foreach (var fn in byName.Values.OrderBy(f => f.StartLine))
        {
            var matched = literals.Where(l => bodies[fn.Name].Contains(l, StringComparison.Ordinal))
                .Concat(aliasHits[fn.Name].Select(a => $"{aliases[a]} via {a}"))
                .ToList();
            if (matched.Count == 0) continue;
            if (reasons.Count >= MaxFunctions) { capped = true; break; }
            reasons[fn.Name] = "references " + string.Join(", ", matched);
            queue.Enqueue(fn.Name);
        }

        // BFS over caller→callee; the reason records the first discoverer.
        while (queue.Count > 0 && !capped)
        {
            var caller = queue.Dequeue();
            foreach (var callee in callees[caller])
            {
                if (reasons.ContainsKey(callee)) continue;
                if (reasons.Count >= MaxFunctions) { capped = true; break; }
                reasons[callee] = $"called by {caller}";
                queue.Enqueue(callee);
            }
        }
        if (capped) diagnostics.Add($"function cap ({MaxFunctions}) reached — script closure truncated");

        // Doer-dummy recovery. A carried handler often READS a per-player unit array (unit array
        // udg_X, indexed by a player slot) whose units are created in a SETUP function the closure
        // never reached, so those dummy types are invisible to the spawn harvest above. Find the one
        // setup function that assigns the array by creating units and carry it, so the harvest then
        // picks up the dummies it makes. Strictly gated and it DECLINES rather than guesses, because
        // this rule is inferred from a single map and a wrong guess on another of the user's maps is
        // worse than admitting it does not know.
        var unitArrays = Regex.Matches(source, @"^[ \t]*unit[ \t]+array[ \t]+([A-Za-z_][A-Za-z0-9_]*)",
                RegexOptions.Multiline)
            .Select(m => m.Groups[1].Value).ToHashSet(StringComparer.Ordinal);
        if (unitArrays.Count > 0)
        {
            bool Reads(string body, string arr) => Regex.IsMatch(body, @"\b" + Regex.Escape(arr) + @"\s*\[");
            bool Assigns(string body, string arr) =>
                Regex.IsMatch(body, @"\bset\s+" + Regex.Escape(arr) + @"\s*\[");
            string? Body(string n) => bodies.TryGetValue(n, out var b) ? b : null;

            // An acceptable setup function, takes a player, populates the array by creating units,
            // calls only natives, BJs or already-carried helpers, and does not branch on a rawcode
            // (which would mark it a per-hero dispatcher rather than a shared per-player setup).
            bool Qualifies(JassFunction f, string body) =>
                Regex.IsMatch(f.Signature, @"\btakes\b.*\bplayer\b")
                && (SpawnGrantCall.IsMatch(body) || body.Contains("bj_lastCreatedUnit", StringComparison.Ordinal))
                && !callees[f.Name].Any(c => byName.ContainsKey(c) && !reasons.ContainsKey(c))
                && !body.Split('\n').Any(l =>
                    (StartsWithWord(l.TrimStart(), "if") || StartsWithWord(l.TrimStart(), "elseif"))
                    && RawcodeLiteral.IsMatch(l));

            var readByCarried = unitArrays.Where(a =>
                reasons.Keys.Any(n => Body(n) is { } b && Reads(b, a))).ToList();
            int declined = 0;
            foreach (var arr in readByCarried)
            {
                if (reasons.Keys.Any(n => Body(n) is { } b && Assigns(b, arr))) continue; // already populated
                var assigners = byName.Values
                    .Where(f => !reasons.ContainsKey(f.Name) && Body(f.Name) is { } b
                                && Assigns(b, arr) && Qualifies(f, b))
                    .Select(f => f.Name).ToList();
                if (assigners.Count == 1)
                    reasons[assigners[0]] = $"populates {arr}, read by a carried handler";
                else
                    declined++; // zero or several qualify, decline rather than guess
            }
            if (declined > 0)
                diagnostics.Add($"{declined} per-player unit array(s) read by the closure are populated by a "
                    + "setup function that could not be carried safely (none qualified, or several did), so a "
                    + "few of their spawned units may be absent (prune noise, or add them by hand)");
        }

        // Custom skills are usually trigger-driven: the visual effect models live as string
        // literals inside the spell handlers (AddSpecialEffect("war3mapImported\\x.mdx"), dummy
        // unit model swaps, ...), NOT in the ability object fields. Scan every closure function
        // body for asset-path literals so those models/textures/sounds port along with the skill.
        int assetBanks = 0, bankAssets = 0;
        foreach (var name in reasons.Keys)
        {
            // A shared asset bank names every hero's art in one body (a pick screen, a guide), so
            // its paths attribute to nobody. Skipping it is what stops a port of one hero from
            // importing the whole roster's models and sounds. An asset a real handler also names is
            // still carried by that handler.
            if (assetRefs[name].Count > SharedAssetBankCount)
            {
                assetBanks++;
                bankAssets += assetRefs[name].Count;
                continue;
            }
            foreach (var path in assetRefs[name])
                addFileRef(name, path, "script");
        }
        if (assetBanks > 0)
            diagnostics.Add($"skipped {bankAssets} asset path(s) in {assetBanks} shared asset bank "
                + $"function(s) (over {SharedAssetBankCount} distinct assets named in one body, so "
                + "they belong to the whole roster rather than this unit)");

        // Objects a carried handler spawns (CreateUnit and kin) or grants (UnitAddAbility) at runtime.
        // The data-driven closure never reaches these (only the SCRIPT names them, by rawcode), so
        // without this a ported spell's spawned dummies and granted sub-abilities are simply absent
        // and it does nothing. This carries generously and reports the cost, rather than guessing.
        //
        // On a tightly-coupled arena the carried FUNCTION set contains other heroes' handlers too (a
        // shared flat death or kill dispatcher calls every hero's), so some carried objects may belong
        // to another hero's kit. Every static rule tried to separate them either dropped one of THIS
        // hero's own handlers (breaking an ability, the worse failure) or still leaked, because shared
        // objects mean no rawcode test tells the heroes apart. So the deliberate choice is to over-carry
        // (bloat, which the map size budget tolerates) and report it loudly, leaving deliberate pruning
        // to the user. The count below is that report.
        var spawned = new SortedSet<string>(StringComparer.Ordinal);
        int registries = 0, registryGrants = 0;
        foreach (var name in reasons.Keys)
        {
            // A roster registry grants every hero's abilities onto pick screen dummies in one flat
            // loop, so its grants attribute to nobody. Skipping it is what stops a port of one hero
            // from carrying the whole roster. Anything a real handler grants still comes through,
            // and a rawcode this registry shares with a real handler is still carried by that
            // handler, so the exclusion cannot cost the hero an ability it actually uses.
            if (spawnRefs[name].Count > RosterRegistryGrantCount)
            {
                registries++;
                registryGrants += spawnRefs[name].Count;
                continue;
            }
            foreach (var rc in spawnRefs[name])
                spawned.Add(rc);
        }
        if (registries > 0)
            diagnostics.Add($"skipped {registryGrants} grant(s) in {registries} roster registry "
                + $"function(s) (over {RosterRegistryGrantCount} distinct objects granted in one body, "
                + "so they belong to the whole roster rather than this unit)");

        int carried = 0;
        foreach (var rc in spawned)
        {
            if (carried >= MaxScriptSpawnedObjects) break;
            carryObject(rc);
            carried++;
        }
        if (carried > 0)
            diagnostics.Add($"carried {carried} object(s) a handler spawns or grants at runtime; "
                + "on a tightly-coupled map some may belong to another hero's kit (over-carry, safe to prune)");
        if (spawned.Count > carried)
            diagnostics.Add($"script-spawned object cap ({MaxScriptSpawnedObjects}) reached, "
                + $"{spawned.Count - carried} more spawned/granted object(s) not carried");

        return reasons
            .Select(kv => new BundleFunction(kv.Key, byName[kv.Key].StartLine, byName[kv.Key].EndLine, kv.Value))
            .OrderBy(f => f.StartLine)
            .ToList();
    }

    /// <summary>True when <paramref name="s"/> begins with the JASS keyword <paramref name="word"/>
    /// as a whole word (the next character is not part of an identifier), so "if" matches "if (x)"
    /// but not "iffy" and "else" does not swallow "elseif".</summary>
    private static bool StartsWithWord(string s, string word)
    {
        if (!s.StartsWith(word, StringComparison.Ordinal)) return false;
        if (s.Length == word.Length) return true;
        char c = s[word.Length];
        return !(char.IsLetterOrDigit(c) || c == '_');
    }

    private static bool FollowedByOpenParen(string body, Match m)
    {
        int i = m.Index + m.Length;
        while (i < body.Length && (body[i] == ' ' || body[i] == '\t')) i++;
        return i < body.Length && body[i] == '(';
    }

    /// <summary>The text of a call's argument list, from the opening paren at <paramref name="openParen"/>
    /// to its matching close (nested parens balanced, e.g. the Player(0) inside CreateUnit). Truncates
    /// at end of line when the call spans lines, which is fine, the type or ability argument comes first.</summary>
    private static string CallArgs(string line, int openParen)
    {
        int depth = 0;
        var sb = new StringBuilder();
        for (int i = openParen; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '(') { depth++; if (depth == 1) continue; }
            else if (c == ')') { depth--; if (depth == 0) break; }
            sb.Append(c);
        }
        return sb.ToString();
    }

    /// <summary>The object ids named in a call's argument text, as rawcode literals ('XXXX') or as an
    /// id global initialized with one. For a spawn or grant call only the type or ability argument is a
    /// rawcode, so this returns exactly the spawned or granted object(s), never a coordinate or player.</summary>
    private static IEnumerable<int> ArgRawcodeIds(string args, Dictionary<string, int> allAliases)
    {
        foreach (Match rl in RawcodeLiteral.Matches(args))
            yield return rl.Groups[1].Value.FromRawcode();
        foreach (Match id in Identifier.Matches(args))
            if (allAliases.TryGetValue(id.Value, out var rc)) yield return rc;
    }

    /// <summary>True when the matched identifier is a "function Foo" code reference —
    /// the "function" keyword immediately precedes it with only whitespace between.</summary>
    private static bool IsFunctionReference(string body, Match? prev, Match m)
    {
        if (prev is null || prev.Value != "function") return false;
        for (int i = prev.Index + prev.Length; i < m.Index; i++)
            if (!char.IsWhiteSpace(body[i])) return false;
        return true;
    }

    /// <summary>Metadata type tokens whose values are object rawcodes: a known object
    /// stem + "Code" (single) or "List" (comma-separated). Non-object lists like
    /// "targetList"/"stringList" stay out — the stem whitelist (the same one the
    /// Studio object editor uses) keeps this conservative.</summary>
    private static bool IsObjectReferenceType(string type)
    {
        var t = type.ToLowerInvariant();
        if (!t.EndsWith("code", StringComparison.Ordinal) && !t.EndsWith("list", StringComparison.Ordinal))
            return false;
        return t[..^4] is "unit" or "abil" or "ability" or "heroability" or "abilityskin"
            or "item" or "tech" or "upgrade" or "buff" or "effect";
    }

    /// <summary>A token references an asset iff it carries a known media extension
    /// anywhere (extensionless model refs get .mdx appended by the game) or a backslash.</summary>
    private static bool LooksLikeAssetPath(string token) =>
        token.Contains('\\')
        || AssetExtensions.Any(ext => token.Contains(ext, StringComparison.OrdinalIgnoreCase));

    private static string Categorize(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext switch
        {
            ".mdx" or ".mdl" => "model",
            ".blp" or ".tga" or ".dds" => IsIconPath(path) ? "icon" : "texture",
            ".mp3" or ".wav" or ".flac" => "sound",
            _ => "other",
        };
    }

    /// <summary>Icons live under CommandButtons/PassiveButtons or use the BTN naming scheme.</summary>
    private static bool IsIconPath(string path)
    {
        if (path.Contains("CommandButtons", StringComparison.OrdinalIgnoreCase)
            || path.Contains("PassiveButtons", StringComparison.OrdinalIgnoreCase))
            return true;
        var name = Path.GetFileName(path);
        return name.StartsWith("BTN", StringComparison.OrdinalIgnoreCase)
            || name.StartsWith("DISBTN", StringComparison.OrdinalIgnoreCase)
            || name.StartsWith("PASBTN", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizePath(string path) => path.Replace('/', '\\').ToLowerInvariant();

    // Requires non-empty bytes so discovery agrees with FindAssetEntry and with the port copy,
    // which skips a zero-length source file. Otherwise a zero-byte import reads present here but
    // is skipped at copy time, a false "present".
    private static MapFileEntry? FindFileEntry(MapDocument doc, string path)
    {
        foreach (var p in new[] { path, path.Replace('/', '\\'), path.Replace('\\', '/') }.Distinct())
            if (doc.GetFile(p) is { CurrentBytes.Length: > 0 } entry) return entry;
        return null;
    }
}
