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
                var category = Categorize(path);
                // Extensionless refs (the game appends .mdx/.mdl at load) categorize as
                // "other". If a stored model file actually backs the ref, treat it as a
                // model so the file itself AND its textures are captured, not dropped.
                var modelEntry = category is "model" or "other"
                    ? RenderModelCommand.FindModelEntry(doc, path)
                    : null;
                if (category == "other" && modelEntry is not null)
                    category = "model";
                bool present = category == "model"
                    ? modelEntry is not null
                    : FindFileEntry(doc, path) is not null;
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
                var model = ModelParser.Parse(entry.RawBytes, entry.FileName);
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
                    stringSet.Add(resolved);
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

        while (queue.Count > 0)
        {
            var (rawcode, merged) = queue.Dequeue();
            var kind = nodes[rawcode].Kind; // enqueue always records the node first
            foreach (var field in merged.Fields)
                ScanField(kind, rawcode, field);
        }

        if (capped) diagnostics.Add($"node cap ({MaxNodes}) reached — dependency closure truncated");

        // Script closure: the war3map.j functions that implement the bundle's custom
        // skills. Seeded by every rawcode being ported (root + custom objects) — those
        // appear in JASS as 'XXXX' literals in spell-handler conditions and the like.
        var seedRawcodes = nodes.Values.Where(n => n.CustomToMap).Select(n => n.Rawcode)
            .Where(rc => rc != rootRawcode)
            .OrderBy(rc => rc, StringComparer.Ordinal)
            .Prepend(rootRawcode)
            .ToList();
        var functions = ResolveScriptClosure(doc, seedRawcodes, diagnostics, AddFileRef);

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

    private static readonly Regex Identifier = new(@"[A-Za-z_][A-Za-z0-9_]*", RegexOptions.Compiled);

    /// <summary>A double-quoted JASS string literal (captures the inner text). Used to pull
    /// asset paths out of spell handlers - AddSpecialEffect("war3mapImported\\x.mdx") and the
    /// like - so trigger-driven skill models get carried by the port.</summary>
    private static readonly Regex StringLiteral = new("\"([^\"]*)\"", RegexOptions.Compiled);

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
    /// </summary>
    private static IReadOnlyList<BundleFunction> ResolveScriptClosure(
        MapDocument doc, IReadOnlyList<string> seedRawcodes, List<string> diagnostics,
        Action<string, string, string> addFileRef)
    {
        var entry = doc.GetFile("war3map.j") ?? doc.GetFile("scripts\\war3map.j");
        if (entry is null)
        {
            diagnostics.Add(doc.GetFile("war3map.lua") is not null
                ? "Lua scripts not analyzed (JASS only) — script closure skipped"
                : "map has no war3map.j — script closure skipped");
            return Array.Empty<BundleFunction>();
        }

        var source = Encoding.UTF8.GetString(entry.RawBytes);
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

        var aliases = new Dictionary<string, string>(StringComparer.Ordinal); // name → 'XXXX'
        for (int i = 0; i < lines.Length; i++)
        {
            if (inFunction[i]) continue;
            var line = StripLineComment(lines[i]);
            foreach (var lit in literals)
            {
                if (!line.Contains(lit, StringComparison.Ordinal)) continue;
                var m = GlobalInitializer.Match(line);
                if (m.Success) aliases.TryAdd(m.Groups[1].Value, lit);
            }
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

        // One token pass per body fills both the call graph and the alias mentions.
        // Callees in body order: an identifier counts as a call when it names another
        // indexed function and is either invoked ("Foo(") or passed by reference
        // ("function Foo" — TriggerAddAction/TimerStart/Condition and friends).
        var callees = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        var aliasHits = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        foreach (var (name, body) in bodies)
        {
            var calls = new List<string>();
            var mentions = new List<string>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            Match? prev = null;
            foreach (Match m in Identifier.Matches(body))
            {
                if (m.Value != name && byName.ContainsKey(m.Value)
                    && (FollowedByOpenParen(body, m) || IsFunctionReference(body, prev, m))
                    && seen.Add(m.Value))
                    calls.Add(m.Value);
                else if (aliases.ContainsKey(m.Value) && seen.Add("'" + m.Value))
                    mentions.Add(m.Value);
                prev = m;
            }
            callees[name] = calls;
            aliasHits[name] = mentions;
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

        // Custom skills are usually trigger-driven: the visual effect models live as string
        // literals inside the spell handlers (AddSpecialEffect("war3mapImported\\x.mdx"), dummy
        // unit model swaps, ...), NOT in the ability object fields. Scan every closure function
        // body for asset-path literals so those models/textures/sounds port along with the skill.
        foreach (var name in reasons.Keys)
            foreach (Match sl in StringLiteral.Matches(bodies[name]))
            {
                // Unescape JASS string escapes: a path in source is "war3mapImported\\x.mdx"
                // (doubled backslashes) but the map stores it single-slashed, so match that.
                var path = sl.Groups[1].Value.Replace(@"\\", @"\");
                if (LooksLikeAssetPath(path)) addFileRef(name, path, "script");
            }

        return reasons
            .Select(kv => new BundleFunction(kv.Key, byName[kv.Key].StartLine, byName[kv.Key].EndLine, kv.Value))
            .OrderBy(f => f.StartLine)
            .ToList();
    }

    private static bool FollowedByOpenParen(string body, Match m)
    {
        int i = m.Index + m.Length;
        while (i < body.Length && (body[i] == ' ' || body[i] == '\t')) i++;
        return i < body.Length && body[i] == '(';
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

    private static MapFileEntry? FindFileEntry(MapDocument doc, string path) =>
        doc.GetFile(path) ?? doc.GetFile(path.Replace('/', '\\')) ?? doc.GetFile(path.Replace('\\', '/'));
}
