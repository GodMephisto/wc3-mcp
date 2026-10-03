// src/Wc3.Commands/HeroExportCommand.cs
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wc3.Model;

namespace Wc3.Commands;

public sealed record HeroExportResult(
    string Directory,
    HeroDefinition Definition,
    int AssetsWritten,
    long AssetBytes,
    int ObjectsExcluded,
    int FilesExcluded);

/// <summary>
/// Exports a hero out of a map as a reviewable <see cref="HeroDefinition"/> plus its assets,
/// which is the <c>import</c> verb of the format.
///
/// The critical decision here is WHAT NOT TO CARRY. The porter's failure was pulling in whatever
/// a script closure could reach - on one hero that was 61 extra objects and thousands of foreign
/// functions belonging to other heroes - and then trimming the calls it could not resolve, which
/// is what produced uninitialized locals and non-terminating loops. So this exports the hero's
/// REAL dependencies (edges through actual object fields) and REPORTS the closure carry as review
/// notes instead of silently including it. A human decides.
///
/// Being incomplete is acceptable and expected. A definition is reviewed before it is installed,
/// so "found the kit, flagged 61 maybes" is a good result. That was never true of a porter whose
/// output went straight into a map someone then hosted.
/// </summary>
public static class HeroExportCommand
{
    private static readonly JsonSerializerOptions Json = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };




    /// <summary>
    /// The one place the per-hero namespace prefix is defined, so install's reverse rename cannot
    /// drift from export's forward one. Two independent copies of this rule would be a silent
    /// mismatch, and a rename that only half applies leaves references with nothing to resolve to.
    /// </summary>
    internal static string NamespacePrefix(string rootRawcode) =>
        "h" + new string(rootRawcode.Where(char.IsLetterOrDigit).ToArray()) + "__";

    /// <summary>
    /// A field's value with any TRIGSTR reference already resolved, so it carries meaning outside
    /// the source map. Falls back to the raw value when there is nothing to resolve.
    /// </summary>
    private static string ResolvedValue(MergedField f) =>
        f.Value.StartsWith("TRIGSTR_", StringComparison.Ordinal) ? f.Display : f.Value;

    /// <summary>
    /// A prefix for every carried function and global, so nothing the definition brings can clash
    /// with a name the target already uses. The prefix is derived from the hero's rawcode, which is
    /// unique per definition and stable across installs.
    /// </summary>
    private static Dictionary<string, string> BuildNamespace(
        string rootRawcode, IEnumerable<string> functions, IEnumerable<string> globalLines)
    {
        var prefix = NamespacePrefix(rootRawcode);
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var fn in functions) map[fn] = prefix + fn;

        var decl = new System.Text.RegularExpressions.Regex(
            @"^\s*(?:constant\s+)?[A-Za-z_][A-Za-z0-9_]*\s+(?:array\s+)?([A-Za-z_][A-Za-z0-9_]*)");
        foreach (var line in globalLines)
        {
            var m = decl.Match(line);
            if (m.Success) map[m.Groups[1].Value] = prefix + m.Groups[1].Value;
        }
        // 'main' and 'config' are the engine's, never ours to rename.
        map.Remove("main");
        map.Remove("config");
        return map;
    }

    /// <summary>
    /// Rewrites carried symbol names, including inside string literals, because a map can dispatch
    /// by name (<c>ExecuteFunc("Foo")</c>) and renaming the declaration without the literal would
    /// break that call at runtime with no compile error to warn about it.
    /// </summary>
    internal static string ApplyNamespace(string text, Dictionary<string, string> rename)
    {
        if (rename.Count == 0) return text;
        // Longest first, so a name that is a prefix of another cannot corrupt it.
        var pattern = string.Join("|", rename.Keys
            .OrderByDescending(k => k.Length)
            .Select(System.Text.RegularExpressions.Regex.Escape));
        // A match preceded by a backslash is part of an escape sequence, not an identifier.
        // Without this guard a carried global named 'n' turned every "\\n" in the script into
        // "\\hH0DA__n", which pjass rejects as an invalid escape sequence. A word boundary is
        // not enough here because the backslash IS a word boundary.
        return System.Text.RegularExpressions.Regex.Replace(
            text, @"(?<!\\)\b(?:" + pattern + @")\b",
            m => rename.TryGetValue(m.Value, out var to) ? to : m.Value);
    }

    /// <summary>
    /// Declarations from the source's <c>globals</c> block that the carried functions actually
    /// reference, returned verbatim so a type or initialiser is never re-derived. Only what is used
    /// is carried: copying the whole block would drag in thousands of unrelated declarations, which
    /// is the same over-carry mistake in a different place.
    /// </summary>
    private static List<string> GlobalsUsedBy(string[] lines, HashSet<string> functions,
        Dictionary<string, (int start, int end)> spans)
    {
        int blockStart = -1, blockEnd = -1;
        for (int i = 0; i < lines.Length; i++)
        {
            var t = lines[i].Trim();
            if (blockStart < 0 && t == "globals") { blockStart = i; continue; }
            if (blockStart >= 0 && t == "endglobals") { blockEnd = i; break; }
        }
        if (blockStart < 0 || blockEnd < 0) return new List<string>();

        // name -> its declaration line
        var declOf = new Dictionary<string, string>(StringComparer.Ordinal);
        var decl = new System.Text.RegularExpressions.Regex(
            @"^\s*(?:constant\s+)?[A-Za-z_][A-Za-z0-9_]*\s+(?:array\s+)?([A-Za-z_][A-Za-z0-9_]*)");
        for (int i = blockStart + 1; i < blockEnd; i++)
        {
            var line = lines[i].TrimEnd('\r');
            var m = decl.Match(line);
            if (m.Success && !declOf.ContainsKey(m.Groups[1].Value))
                declOf[m.Groups[1].Value] = line;
        }

        var word = new System.Text.RegularExpressions.Regex(@"[A-Za-z_][A-Za-z0-9_]*");
        var used = new HashSet<string>(StringComparer.Ordinal);
        foreach (var fn in functions)
        {
            if (!spans.TryGetValue(fn, out var span)) continue;
            for (int i = span.start; i <= span.end; i++)
                foreach (System.Text.RegularExpressions.Match m in word.Matches(lines[i]))
                    if (declOf.ContainsKey(m.Value)) used.Add(m.Value);
        }

        // A carried global's INITIALISER can name another global, so closing only over function
        // bodies leaves those dangling: 'integer A= B' carried without B fails with "Undeclared
        // variable B". Iterate until the set stops growing.
        bool grew = true;
        while (grew)
        {
            grew = false;
            foreach (var name in used.ToList())
                foreach (System.Text.RegularExpressions.Match m in word.Matches(declOf[name]))
                    if (m.Value != name && declOf.ContainsKey(m.Value) && used.Add(m.Value))
                        grew = true;
        }

        // Preserve the source's declaration order: a constant may initialise from an earlier one.
        return declOf.Where(kv => used.Contains(kv.Key)).Select(kv => kv.Value)
            .OrderBy(line => Array.IndexOf(lines, line)).ToList();
    }

    /// <summary>
    /// Every function declaration mapped to its line span, via the shared
    /// <see cref="JassFunctionIndex"/> rather than a local regex.
    /// </summary>
    /// <remarks>
    /// A hand-rolled matcher here required <c>function</c> at column zero, so an INDENTED
    /// declaration was invisible to it. Those functions never entered the closure and the emitted
    /// script failed with "Undeclared function MakeSound". <c>JassFunctionIndex</c> exists in this
    /// repo precisely because throwaway scans miss that (it also handles <c>constant function</c>,
    /// trailing line comments, and vJASS <c>function interface</c>), so the fix is to use it rather
    /// than to keep a second, weaker parser alongside it.
    /// </remarks>
    private static Dictionary<string, (int start, int end)> IndexFunctions(string[] lines)
    {
        var spans = new Dictionary<string, (int, int)>(StringComparer.Ordinal);
        foreach (var fn in JassFunctionIndex.Parse(string.Join("\n", lines)))
            spans[fn.Name] = (fn.StartLine - 1, fn.EndLine - 1);   // Parse reports 1-based lines
        return spans;
    }

    /// <summary>
    /// Transitive callees of the seed set. Matches a name followed by '(' as well as
    /// 'function X': JASS uses the <c>call</c> keyword ONLY for statement-level calls, so a call
    /// inside an expression like <c>if IsValid(u) then</c> carries no keyword. Keying on
    /// <c>call</c> alone misses most references, which is exactly how an earlier pass lost live
    /// code. Over-matching costs a few extra functions; under-matching breaks the script.
    /// </summary>
    /// <summary>
    /// A class that is a SIBLING of the exported unit rather than something it depends on. The
    /// convention in these maps is one class per character, so a name ending in "Spells" is
    /// another character's kit; reaching it means the walk passed through the shared dispatcher,
    /// which is the thing to declare instead (see the roster-registration requirement).
    /// </summary>
    private static bool IsPeerClass(string wurstClass) =>
        wurstClass.EndsWith("Spells", StringComparison.Ordinal)
        || wurstClass.EndsWith("Debuff", StringComparison.Ordinal);

    /// <summary>
    /// True when the carried globals include a PRIVATE copy of this Wurst class's instance tables,
    /// which is what makes its calls unbindable however complete the target's own copy is.
    /// </summary>
    /// <remarks>
    /// Wurst gives every class an allocator (<c>Foo_firstFree</c>, <c>Foo_maxIndex</c>,
    /// <c>Foo_nextFree</c>) and a <c>Foo_typeId</c> array, and its optimiser INLINES allocation into
    /// the caller. So a hero that constructs a CallbackSingle carries the allocator counters even
    /// though the class itself was excluded, and the instance ids she issues are indices into HER
    /// arrays. Binding her <c>dispatch_CallbackSingle_start</c> to the target's would then pass the
    /// target an id it never issued, and the target would read its own typeId array at that index:
    /// a wrong dispatch or none, with no compile error and no runtime message. Measured on Shadow
    /// Nanaya, CallbackSingle carried 4 such globals and ShopUI 12, while HashMap, Table,
    /// CallbackPeriodic, ForForceCallback and ShopButton carried none.
    /// </remarks>
    private static bool CarriesClassState(string wurstClass, IEnumerable<string> globalNames)
    {
        foreach (var prefix in new[] { "", "s__", "si__", "sc__" })
        {
            var head = prefix + wurstClass + "_";
            foreach (var n in globalNames)
                if (n.StartsWith(head, StringComparison.Ordinal)) return true;
        }
        return false;
    }

    /// <summary>Declared names out of raw <c>globals</c> lines, for <see cref="CarriesClassState"/>.</summary>
    private static List<string> GlobalNames(IEnumerable<string> globalLines)
    {
        var decl = new System.Text.RegularExpressions.Regex(
            @"^\s*(?:constant\s+)?[A-Za-z_][A-Za-z0-9_]*\s+(?:array\s+)?([A-Za-z_][A-Za-z0-9_]*)");
        var names = new List<string>();
        foreach (var line in globalLines)
        {
            var m = decl.Match(line);
            if (m.Success) names.Add(m.Groups[1].Value);
        }
        return names;
    }

    /// <summary>Wurst emits s__Class_method / si__Class_field / dispatch_Class_method.</summary>
    private static readonly System.Text.RegularExpressions.Regex WurstSymbol =
        new(@"^(?:s__|si__|sc__|dispatch_|init_)([A-Za-z0-9]+?)_", System.Text.RegularExpressions.RegexOptions.Compiled);

    /// <summary>
    /// Transitive callees of the seed set, STOPPING at the boundary of a Wurst class the hero does
    /// not belong to.
    /// </summary>
    /// <remarks>
    /// Unbounded call-following carried 1,138 functions for a 10-ability hero, of which only 30
    /// were hers. The rest were the source map's item shop and the Wurst standard library
    /// (ShopUI, ShopAppearance, ItemInShop, LinkedList, HashList, Table...), reached because one
    /// utility call leads into the generated class dispatcher and from there to everything the map
    /// ever compiled. That volume is what a target cannot absorb.
    ///
    /// Wurst's output is namespaced, so the seam is in the naming rather than in the call graph:
    /// the hero's own class is discoverable from the seeds, and any OTHER class is a separate
    /// concern to be declared as a requirement instead of swallowed. Hand-written JASS has no such
    /// marker and is still followed normally.
    ///
    /// Matching a name followed by '(' as well as 'function X' is deliberate: JASS uses the
    /// <c>call</c> keyword ONLY for statement-level calls, so a call inside an expression carries
    /// no keyword, and keying on <c>call</c> alone loses most references.
    /// </remarks>
    private static HashSet<string> Closure(
        Dictionary<string, (int start, int end)> spans, IEnumerable<string> seed,
        SortedSet<string> excludedClasses, out bool sawPeer, SortedSet<string> peerCalls)
    {
        sawPeer = false;
        var seedList = seed.Where(spans.ContainsKey).ToList();

        // Classes the hero's own functions belong to; everything else is somebody else's system.
        var ownClasses = new HashSet<string>(StringComparer.Ordinal);
        foreach (var name in seedList)
        {
            var m = WurstSymbol.Match(name);
            if (m.Success) ownClasses.Add(m.Groups[1].Value);
        }

        var reference = new System.Text.RegularExpressions.Regex(
            @"\b([A-Za-z_][A-Za-z0-9_]*)\s*\(|\bfunction\s+([A-Za-z_][A-Za-z0-9_]*)");
        var keep = new HashSet<string>(StringComparer.Ordinal);
        var queue = new Queue<string>(seedList);
        while (queue.Count > 0)
        {
            var name = queue.Dequeue();
            if (!keep.Add(name)) continue;
            var (from, to) = spans[name];
            for (int i = from; i <= to; i++)
                foreach (System.Text.RegularExpressions.Match m in reference.Matches(_scriptLines![i]))
                {
                    var callee = m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value;
                    if (!spans.ContainsKey(callee) || keep.Contains(callee)) continue;

                    var wm = WurstSymbol.Match(callee);
                    if (wm.Success && !ownClasses.Contains(wm.Groups[1].Value))
                    {
                        var cls = wm.Groups[1].Value;
                        // A PEER, not a dependency. 20 of 28 excluded classes were other heroes'
                        // spell classes (AlucardSpells, AsNodtSpells, ...), and Nanaya does not
                        // call Alucard's spells: all hero classes route through one shared Wurst
                        // dispatcher, so reaching it makes every hero look like a neighbour. One
                        // edge, hit 20 times. A peer is neither carried NOR declared - listing it
                        // as a requirement would ask a target to supply unrelated characters.
                        // Either way the DEFINITION is not carried, so the CALL must still resolve
                        // to something or the script will not compile. A peer is another character;
                        // an excluded class is the source map's infrastructure. Both get a stub, and
                        // both are reported - a peer via shared-spell-dispatcher, infrastructure via
                        // its own wurst-class requirement. Stubbing only peers left the
                        // infrastructure calls dangling (dispatch_HashMap_..., dispatch_ShopUI_...),
                        // which is the identical mistake one set over.
                        if (IsPeerClass(cls)) sawPeer = true; else excludedClasses.Add(cls);
                        peerCalls.Add(callee);
                        continue;
                    }
                    queue.Enqueue(callee);
                }
        }
        return keep;
    }

    /// <summary>
    /// The byte-preserving codec for script text, matching <c>HeroInstallCommand</c>. Latin-1 is a
    /// byte-to-code-point identity, so a read/modify/write round-trip is lossless whatever the
    /// script's real encoding is, and a definition carries the source's bytes rather than a guess
    /// at their meaning.
    /// </summary>
    private static readonly Encoding ScriptBytes = Encoding.Latin1;

    [ThreadStatic] private static string[]? _scriptLines;

    public static HeroExportResult Run(MapDocument doc, string rawcode, string sourceMapPath,
        string outputDirectory, string? gameDir = null)
    {
        ArgumentNullException.ThrowIfNull(doc);
        Directory.CreateDirectory(outputDirectory);
        var assetDir = Path.Combine(outputDirectory, "assets");

        var bundle = BundleCommand.ResolveUnit(doc, rawcode, gameDir);

        // The hero's own dependency set, not everything a script could reach.
        var realObjects = BundleStructure.RealObjects(bundle);
        var realFiles = BundleStructure.RealFiles(bundle);

        // Carry each object's MAP-LEVEL fields, so the definition stands alone. Base-game values
        // are deliberately excluded: they belong to the engine, not the hero, and baking them in
        // would make a definition wrong the moment a patch changes a default.
        var objects = new List<DefinitionObject>();
        foreach (var o in bundle.Objects.Where(o => realObjects.Contains(o.Rawcode)))
        {
            var merged = ObjectGetCommand.Execute(doc, o.Kind, o.Rawcode, gameDir);
            var fields = merged.Found
                // Resolve a TRIGSTR reference into its literal text. A raw value like
                // "TRIGSTR_6175" indexes the SOURCE map's war3map.wts, which the target has no
                // copy of, so carrying it verbatim gave the installed hero the literal name
                // "TRIGSTR_6175" in game. MergedField.Display is that same value with the source's
                // string table already applied, which is the only form that means anything in
                // another map. Keeping the raw reference is right for editing a map in place and
                // wrong for moving content between maps, and this format only does the latter.
                ? merged.Fields.Where(f => f.Source == "map")
                    .Select(f => new DefinitionField(f.Code, ResolvedValue(f))).ToList()
                : new List<DefinitionField>();
            objects.Add(new DefinitionObject(o.Rawcode, o.Kind.ToString().ToLowerInvariant(),
                o.Name, merged.BaseRawcode, o.CustomToMap,
                o.Rawcode == bundle.RootRawcode ? "root" : "dependency", fields));
        }

        // Asset paths named by the carried objects' OWN fields, which the script closure never
        // sees. A hero's model (umdl) and icon (uico/ussi) live only in object data, so a bundle
        // built from script references carried her ability icons and left her with no model and no
        // portrait in game. Those are exactly the fields a player notices first.
        var fieldAssets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var o in objects)
            foreach (var f in o.Fields)
                if (AssetPathCandidates.LooksLikeAssetPath(f.Value))
                    foreach (var cand in AssetPathCandidates.Expand(f.Value))
                        if (doc.GetFile(cand) is not null) { fieldAssets.Add(cand); break; }


        string? scriptFile = null;
        // The carried functions and their spans, kept so the asset pass below can scan the bodies
        // that actually ship rather than only the bundle's narrower seed set.
        var carriedSpans = new List<(string Name, int Start, int End)>();
        var entryPoints = new List<string>();
        var excludedClassNames = new SortedSet<string>(StringComparer.Ordinal);
        var peerCallTargets = new SortedSet<string>(StringComparer.Ordinal);
        bool sharedDispatcherSeen = false;
        var globals = new List<string>();
        var stubbed = new List<DefinitionStub>();
        if (bundle.Functions.Count > 0 && doc.GetFile("war3map.j") is { } js)
        {
            // Latin-1, so a carried line arrives byte for byte. Latin-1 maps every byte 0..255 to
            // the same code point and back, which UTF-8 does not: a script that is really UTF-8
            // decodes to code points above U+00FF, and everything downstream that has to write raw
            // bytes again (pjass's input, the target's war3map.j) then replaces each with '?'. A
            // script that is NOT valid UTF-8 loses those bytes to U+FFFD outright. Either way the
            // carried text stops being what the source map had, which is the one thing a port
            // cannot afford.
            var text = ScriptBytes.GetString(js.CurrentBytes);
            var lines = text.Split('\n');

            // Carry the hero's functions AND everything they call, transitively. Carrying only the
            // seed set produced 50 "undeclared function" errors: the opposite failure to the
            // porter's over-carry, but just as fatal, since a script that does not compile means a
            // hosted map has no player slots. Over-including here is cheap; under-including is not.
            _scriptLines = lines;
            var spans = IndexFunctions(lines);
            var wanted = Closure(spans, bundle.Functions.Select(f => f.Name), excludedClassNames,
                out sharedDispatcherSeen, peerCallTargets);
            foreach (var name in wanted)
                if (spans.TryGetValue(name, out var sp))
                    carriedSpans.Add((name, sp.start, sp.end));

            var reason = bundle.Functions.ToDictionary(f => f.Name, f => f.Reason, StringComparer.Ordinal);

            var sb = new StringBuilder();
            foreach (var name in wanted.OrderBy(n => spans[n].start))
            {
                var (start, end) = spans[name];
                sb.AppendLine($"// {name}  ({(reason.TryGetValue(name, out var r) ? r : "called by a carried function")})");
                for (int i = start; i <= end; i++) sb.AppendLine(lines[i].TrimEnd('\r'));
                sb.AppendLine();
                entryPoints.Add(name);
            }
            // Globals the carried code reads. A function without its globals is as broken as a
            // function without its callees, and fails the same way: the script will not compile.
            var rawGlobals = GlobalsUsedBy(lines, wanted, spans);

            // Namespace every carried symbol. Rawcodes are remapped on install and assets are
            // hash-checked, but script symbols had no collision handling, which produced
            // "Symbol s already defined as global variable" the moment globals started being
            // carried: the source declares 's', the target has a local 's'. Prefixing per hero
            // means a carried symbol cannot collide on ANY target, which is the same fix already
            // applied to the other two namespaces.
            var rename = BuildNamespace(bundle.RootRawcode, wanted.Concat(peerCallTargets), rawGlobals);
            var body = ApplyNamespace(sb.ToString(), rename);
            globals.AddRange(rawGlobals.Select(g => ApplyNamespace(g, rename)));
            entryPoints = entryPoints.Select(n => rename.TryGetValue(n, out var r) ? r : n).ToList();

            // Excluding a class removed its DEFINITIONS but not the CALLS to it, so the emitted
            // script failed with "Undeclared function s__AlucardSpells___...". That is the porter's
            // trimming mistake reached from the other direction: never leave a reference without
            // something to resolve it. Emit a no-op stub per called entry point, and RECORD each
            // one, because a stub is a placeholder and only the record makes it replaceable. The
            // stubs were previously namespaced and unrecorded, so a target that implements HashMap
            // itself could never be reached: the carried code called a private empty function that
            // returned 0. Install now binds what it safely can and reports the rest.
            var globalNames = GlobalNames(rawGlobals);
            if (peerCallTargets.Count > 0)
            {
                var stubs = new StringBuilder();
                stubs.AppendLine();
                stubs.AppendLine("// ==== stubs for called-but-not-carried functions ====");
                stubs.AppendLine("// The closure stopped at these classes, which are other characters' kits and the");
                stubs.AppendLine("// source map's own infrastructure. They are stubbed so this script compiles, and");
                stubs.AppendLine("// 'wc3ctl hero install' deletes a stub and binds the call to the target's own");
                stubs.AppendLine("// function where that is safe, reporting every one it could not, because a stub");
                stubs.AppendLine("// that survives means this hero is incomplete on that target.");
                foreach (var fn in peerCallTargets)
                {
                    if (!spans.TryGetValue(fn, out var sp)) continue;
                    var header = lines[sp.start].TrimEnd('\r');
                    stubs.AppendLine(ApplyNamespace(header, rename));
                    if (header.Contains("returns nothing", StringComparison.Ordinal))
                        stubs.AppendLine("    // stub");
                    else if (header.Contains("returns boolean", StringComparison.Ordinal))
                        stubs.AppendLine("    return false");
                    else if (header.Contains("returns integer", StringComparison.Ordinal))
                        stubs.AppendLine("    return 0");
                    else if (header.Contains("returns real", StringComparison.Ordinal))
                        stubs.AppendLine("    return 0.");
                    else if (header.Contains("returns string", StringComparison.Ordinal))
                        stubs.AppendLine("    return null");
                    else
                        stubs.AppendLine("    return null");
                    stubs.AppendLine("endfunction");

                    var wm = WurstSymbol.Match(fn);
                    var cls = wm.Success ? wm.Groups[1].Value : "(not a Wurst class)";
                    stubbed.Add(new DefinitionStub(fn,
                        rename.TryGetValue(fn, out var ns) ? ns : fn,
                        cls, header.Trim(),
                        Peer: wm.Success && IsPeerClass(cls),
                        ClassStateCarried: wm.Success && CarriesClassState(cls, globalNames)));
                }
                body = stubs.ToString() + body;
            }

            scriptFile = "script.j";
            // Latin-1 out as well as in, so script.j on disk holds the source map's own bytes and
            // install can put them back unchanged. Reading one way and writing the other is what
            // turned a target's 51,779 non-ASCII bytes into 103,572 on a previous install.
            File.WriteAllText(Path.Combine(outputDirectory, scriptFile), body, ScriptBytes);
        }

        // Effect art named by string literals inside the functions that ACTUALLY SHIP.
        //
        // The bundle runs its own per-function asset scan, but only over its SEED functions, the
        // ones that reference the hero directly. The script carried here is the transitive closure
        // of those, which on one hero was 246 functions against the bundle's 92. Anything named
        // only in a transitively-carried function is therefore invisible to the bundle, with no
        // entry in bundle.Files and no edge, so it is unreachable however the dependency walk is
        // seeded. Five effect models for one hero's passive were lost exactly that way and her
        // spells fired with no visual at all.
        //
        // The bank guard is kept, and keeping it is the whole difference between this and a
        // blanket rescan. A function naming more than SharedAssetBankCount distinct assets is a
        // roster-wide bank, a pick screen or a shared effect dispatcher, and its paths belong to
        // nobody in particular. Scanning the carried text WITHOUT that guard pulled in 134 files
        // that appear in no carried model and in no carried handler.
        var scriptAssets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        int assetBanksSkipped = 0, bankAssetsSkipped = 0;
        foreach (var (_, start, end) in carriedSpans)
        {
            var named = AssetPathCandidates
                .NamedInScript(string.Join("\n", _scriptLines![start..(end + 1)]))
                .ToList();
            if (named.Count > BundleCommand.SharedAssetBankCount)
            {
                assetBanksSkipped++;
                bankAssetsSkipped += named.Count;
                continue;
            }
            foreach (var path in named)
                foreach (var cand in AssetPathCandidates.Expand(path))
                    if (doc.GetFile(cand) is not null) { scriptAssets.Add(cand); break; }
        }

        var assets = new List<DefinitionAsset>();
        long bytes = 0;
        var wantedAssets = bundle.Files.Where(f => realFiles.Contains(f.Path)).Select(f => f.Path)
            .Concat(fieldAssets)
            .Concat(scriptAssets)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(p2 => bundle.Files.FirstOrDefault(bf =>
                string.Equals(bf.Path, p2, StringComparison.OrdinalIgnoreCase))
                ?? new BundleFile(p2, scriptAssets.Contains(p2) ? "effect" : "field", true));

        foreach (var f in wantedAssets)
        {
            var entry = doc.GetFile(f.Path);
            if (entry is null) continue;
            var payload = entry.CurrentBytes;
            var dest = Path.Combine(assetDir, f.Path.Replace('\\', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
            File.WriteAllBytes(dest, payload);
            assets.Add(new DefinitionAsset(f.Path, f.Category, payload.Length,
                Convert.ToHexString(SHA256.HashData(payload)).ToLowerInvariant()));
            bytes += payload.Length;
        }

        // What a target must already provide. Stated so an install can refuse rather than produce
        // a map that silently lacks the hero.
        var requires = new List<DefinitionRequirement>
        {
            new("roster-registration",
                "the target must register this rawcode with its own hero roster; find the call with "
                + "'wc3ctl contract <target>' (GGGA's is RPB_AddHero(rawcode, role, portrait))",
                Satisfiable: false),
            new("spell-dispatch",
                "the target must route this hero's ability ids to its cast handlers, or the abilities "
                + "will exist but do nothing",
                Satisfiable: false),
        };
        // Wurst classes the closure deliberately stopped at. These are the source map's own
        // systems and standard library, not the hero's, so the target must already provide them.
        // Swallowing them instead is what produced 1,138 functions for a 30-function hero.
        foreach (var cls in excludedClassNames)
        {
            // Say whether an install can actually bind this class, so the requirement is a decision
            // and not just a name. A class whose instance tables came along is unbindable however
            // complete the target's own copy is (see CarriesClassState), and saying so here is the
            // difference between "the target must have HashMap" and "even a target that has it
            // cannot be used for this one".
            int calls = stubbed.Count(s => s.WurstClass == cls);
            bool stateCarried = stubbed.Any(s => s.WurstClass == cls && s.ClassStateCarried);
            requires.Add(new DefinitionRequirement("wurst-class",
                $"the target must provide the Wurst class '{cls}', which this hero's code calls into "
                + $"({calls} function(s)). "
                + (stateCarried
                    ? "This definition carries the class's own instance tables, so install CANNOT "
                      + "bind to the target's copy and will keep no-op stubs: the calls need the "
                      + "class itself carried, or the calling code pruned."
                    : "Install will bind these calls to the target's own functions when it declares "
                      + "them with matching signatures."),
                Satisfiable: !stateCarried));
        }
        if (sharedDispatcherSeen)
            requires.Add(new DefinitionRequirement("shared-spell-dispatcher",
                "this hero's spells route through the source map's shared dispatcher, which every "
                + "character there shares. The target must provide an equivalent entry point, or the "
                + "spells will not fire. Peer characters' classes were deliberately NOT carried.",
                Satisfiable: false));

        int excludedObjects = bundle.Objects.Count - objects.Count;
        int excludedFiles = bundle.Files.Count - assets.Count;
        var notes = new List<string>();
        if (excludedObjects > 0)
            notes.Add($"{excludedObjects} object(s) reachable only through the script closure were "
                      + "EXCLUDED; they usually belong to other heroes. Review and add any that are "
                      + "genuinely this hero's.");
        if (excludedFiles > 0)
            notes.Add($"{excludedFiles} file(s) reachable only through the script closure were EXCLUDED.");
        if (stubbed.Count > 0)
            notes.Add($"{stubbed.Count} called function(s) are NO-OP STUBS, not implementations "
                      + $"({stubbed.Count(s => s.Peer)} belong to other characters and stay stubbed, "
                      + $"{stubbed.Count(s => !s.Peer && !s.ClassStateCarried)} can bind to a target "
                      + $"that declares them, {stubbed.Count(s => !s.Peer && s.ClassStateCarried)} "
                      + "cannot bind because this definition carries the class's own instance "
                      + "tables). Whatever install leaves stubbed does nothing at runtime.");
        if (assetBanksSkipped > 0)
            notes.Add($"skipped {bankAssetsSkipped} asset path(s) named in {assetBanksSkipped} "
                + $"carried function(s) that each name more than {BundleCommand.SharedAssetBankCount} "
                + "distinct assets. A body that names that many is a roster-wide bank (a pick "
                + "screen, a shared effect dispatcher) and its paths belong to no one hero. If this "
                + "hero is missing a visual, the path is probably in one of these and needs "
                + "carrying by hand.");
        if (scriptAssets.Count > 0)
            notes.Add($"carried {scriptAssets.Count} effect asset(s) named only by string literals "
                + "inside the carried handlers, which object fields never mention.");
        foreach (var d in bundle.Diagnostics) notes.Add(d);

        var def = new HeroDefinition(HeroDefinition.CurrentSchemaVersion, bundle.RootRawcode,
            bundle.RootName, Path.GetFileName(sourceMapPath), "wc3ctl hero export",
            objects, assets, bundle.Strings, scriptFile, entryPoints, globals, requires, notes,
            stubbed);

        File.WriteAllText(Path.Combine(outputDirectory, "hero.json"),
            JsonSerializer.Serialize(def, Json), new UTF8Encoding(false));

        return new HeroExportResult(outputDirectory, def, assets.Count, bytes,
            excludedObjects, excludedFiles);
    }
}
