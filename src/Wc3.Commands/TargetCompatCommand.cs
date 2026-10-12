// src/Wc3.Commands/TargetCompatCommand.cs
using System.Text;
using War3Net.Common.Extensions;
using Wc3.GameData;
using Wc3.Model;

namespace Wc3.Commands;

using Gbn = Wc3.Commands.GameBuildName;

/// <summary>One finding from checking a map against a target build.</summary>
public sealed record CompatFinding(string Severity, string Kind, string Name, string Where, string Detail);

/// <summary>
/// Result of checking a map against a target build. IdentifiersChecked and FieldsChecked
/// tell a reader how much was actually examined, so "checked and fine" is visible
/// rather than just "empty result."
/// </summary>
public sealed record CompatResult(
    string Map,
    string TargetBuild,
    IReadOnlyList<CompatFinding> Findings,
    IReadOnlyList<string> NotChecked,
    int IdentifiersChecked,
    int FieldsChecked);

/// <summary>
/// Check a map against a target game build (a schema folder or build name).
/// Reports compile-time incompatibilities (missing natives, globals, types, functions)
/// and display-time incompatibilities (modified fields on object data that the target
/// build no longer defines).
/// </summary>
/// <param name="mapPath">Path to a .w3x/.w3m map file.</param>
/// <param name="target">
/// Either a path to a schema folder, or a build name looked up in schemasDir.
/// </param>
/// <param name="schemasDir">
/// Directory containing schema folders (each holding manifest.json). When null,
/// search <AppContext.BaseDirectory>/schemas first, then walk up from the base
/// directory to find a data/schemas folder.
/// </param>
public static class TargetCompatCommand
{
    /// <summary>
    /// Execute compatibility check. target is a schema folder path or a build name
    /// looked up under schemasDir. When schemasDir is null, look in
    /// <AppContext.BaseDirectory>/schemas, then walk up from the base directory
    /// to a folder holding data/schemas.
    /// </summary>
    public static CompatResult Execute(string mapPath, string target, string? schemasDir = null)
    {
        if (!File.Exists(mapPath))
            throw new FileNotFoundException("map not found: " + mapPath, mapPath);

        var findings = new List<CompatFinding>();
        var notChecked = new List<string>();

        // Resolve schemasDir.
        var resolvedSchemas = ResolveSchemasDir(schemasDir);

        // Build a single identifier-to-builds index across all schemas.
        var allNatives = new Dictionary<string, JassNativeDecl>(StringComparer.Ordinal);
        var allGlobals = new Dictionary<string, JassGlobalDecl>(StringComparer.Ordinal);
        var allTypes = new Dictionary<string, JassTypeDecl>(StringComparer.Ordinal);
        var allFunctions = new Dictionary<string, JassFunctionDecl>(StringComparer.Ordinal);
        var identifierBuilds = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);

        BuildUnionIndex(resolvedSchemas, allNatives, allGlobals, allTypes, allFunctions,
            identifierBuilds);

        // Load the map.
        MapDocument doc = MapDocument.Load(mapPath);

        // 1. Script check (Compile).
        CheckScript(doc, target, resolvedSchemas,
            allNatives, allGlobals, allTypes, allFunctions,
            identifierBuilds, findings, notChecked);

        // 2. Display check (object fields).
        CheckObjects(doc, target, resolvedSchemas, findings, notChecked);

        // Find the target schema (for the result).
        GameSchema targetSchema = LoadTargetSchema(target, resolvedSchemas);

        return new CompatResult(
            Path.GetFullPath(mapPath),
            targetSchema.Build,
            findings.OrderBy(f => f.Severity).ThenBy(f => f.Kind).ThenBy(f => f.Name).ToList(),
            notChecked.OrderBy(n => n).ToList(),
            GetIdentifiersChecked(doc, targetSchema),
            GetFieldsChecked(doc, targetSchema));
    }

    /// <summary>
    /// Resolve the schemas directory from the argument, or walk up from AppContext.BaseDirectory.
    /// Delegates to <see cref="GameSchemaCommand.ResolveSchemasDir"/> so every caller shares one rule.
    /// </summary>
    private static string ResolveSchemasDir(string? schemasDir) =>
        GameSchemaCommand.ResolveSchemasDir(schemasDir);

    /// <summary>
    /// Build a union index across all schema folders. One dictionary maps each
    /// identifier name to all build names that declare it, regardless of kind.
    /// </summary>
    private static void BuildUnionIndex(
        string schemasDir,
        Dictionary<string, JassNativeDecl> allNatives,
        Dictionary<string, JassGlobalDecl> allGlobals,
        Dictionary<string, JassTypeDecl> allTypes,
        Dictionary<string, JassFunctionDecl> allFunctions,
        Dictionary<string, HashSet<string>> identifierBuilds)
    {
        foreach (var subDir in Directory.EnumerateDirectories(schemasDir))
        {
            string manifestPath = Path.Combine(subDir, "manifest.json");
            if (!File.Exists(manifestPath)) continue;

            GameSchema schema = GameSchemaCommand.Load(subDir);
            string build = schema.Build;

            if (schema.Script is null) continue;

            foreach (var kv in schema.Script)
            {
                // Only common.j and blizzard.j for the compile check. Skip AI scripts.
                string scriptKey = kv.Key;
                if (scriptKey != "scripts/common.j" && scriptKey != "scripts/blizzard.j")
                    continue;

                var api = kv.Value;

                foreach (var nat in api.Natives)
                {
                    if (!allNatives.ContainsKey(nat.Name))
                        allNatives[nat.Name] = nat;
                    AddBuild(identifierBuilds, nat.Name, build);
                }

                foreach (var gl in api.Globals)
                {
                    if (!allGlobals.ContainsKey(gl.Name))
                        allGlobals[gl.Name] = gl;
                    AddBuild(identifierBuilds, gl.Name, build);
                }

                foreach (var tp in api.Types)
                {
                    if (!allTypes.ContainsKey(tp.Name))
                        allTypes[tp.Name] = tp;
                    AddBuild(identifierBuilds, tp.Name, build);
                }

                foreach (var fn in api.Functions)
                {
                    if (!allFunctions.ContainsKey(fn.Name))
                        allFunctions[fn.Name] = fn;
                    AddBuild(identifierBuilds, fn.Name, build);
                }
            }
        }
    }

    /// <summary>
    /// Every identifier any schema under a schemas directory declares, with the kind it is and
    /// every build that declares it, oldest build first. Kind is inferred the way a compat finding
    /// infers it, native over global over type over function when a name is several at once.
    /// </summary>
    public sealed record IdentifierIndex(
        IReadOnlyDictionary<string, string> Kinds,
        IReadOnlyDictionary<string, IReadOnlyList<string>> Builds);

    /// <summary>
    /// Build the identifier index across every schema folder, the same union the compat check
    /// walks. Reused by compat explain so one identifier can be asked about without a map.
    /// </summary>
    public static IdentifierIndex LoadIdentifierIndex(string? schemasDir = null)
    {
        var resolved = ResolveSchemasDir(schemasDir);
        var natives = new Dictionary<string, JassNativeDecl>(StringComparer.Ordinal);
        var globals = new Dictionary<string, JassGlobalDecl>(StringComparer.Ordinal);
        var types = new Dictionary<string, JassTypeDecl>(StringComparer.Ordinal);
        var functions = new Dictionary<string, JassFunctionDecl>(StringComparer.Ordinal);
        var builds = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
        BuildUnionIndex(resolved, natives, globals, types, functions, builds);

        var kinds = new Dictionary<string, string>(StringComparer.Ordinal);
        var byBuild = new Dictionary<string, IReadOnlyList<string>>(StringComparer.Ordinal);
        foreach (var kv in builds)
        {
            kinds[kv.Key] = InferKind(kv.Key, natives, globals, types, functions);
            byBuild[kv.Key] = kv.Value
                .OrderBy(b => b, Comparer<string>.Create((a, b) => Gbn.Compare(Gbn.Parse(a), Gbn.Parse(b))))
                .ToList();
        }
        return new IdentifierIndex(kinds, byBuild);
    }

    /// <summary>
    /// True when the schema declares the name as a native, global, type or function in common.j
    /// or blizzard.j. The same membership test the compile check uses against a target.
    /// </summary>
    public static bool TargetDeclares(string name, GameSchema targetSchema)
    {
        var sets = TargetDeclaredSets(targetSchema);
        return CheckInTarget(name, sets.Natives, sets.Globals, sets.Types, sets.Functions);
    }

    /// <summary>
    /// The names a schema declares in common.j and blizzard.j, split by kind. Shared by the
    /// compile check and <see cref="TargetDeclares"/> so both agree on what the target declares.
    /// </summary>
    internal static (HashSet<string> Natives, HashSet<string> Globals,
        HashSet<string> Types, HashSet<string> Functions) TargetDeclaredSets(GameSchema schema)
    {
        var natives = new HashSet<string>(StringComparer.Ordinal);
        var globals = new HashSet<string>(StringComparer.Ordinal);
        var types = new HashSet<string>(StringComparer.Ordinal);
        var functions = new HashSet<string>(StringComparer.Ordinal);

        if (schema.Script is not null)
            foreach (var scriptFile in schema.Script)
            {
                string shortName = scriptFile.Key;
                // Only common.j and blizzard.j for the compile check.
                if (shortName != "scripts/common.j" && shortName != "scripts/blizzard.j") continue;

                var api = scriptFile.Value;
                foreach (var nat in api.Natives) natives.Add(nat.Name);
                foreach (var gl in api.Globals) globals.Add(gl.Name);
                foreach (var tp in api.Types) types.Add(tp.Name);
                foreach (var fn in api.Functions) functions.Add(fn.Name);
            }

        return (natives, globals, types, functions);
    }

    /// <summary>Add a build name to the set for an identifier.</summary>
    private static void AddBuild(
        Dictionary<string, HashSet<string>> identifierBuilds, string name, string build)
    {
        if (!identifierBuilds.TryGetValue(name, out var set))
        {
            set = new HashSet<string>(StringComparer.Ordinal);
            identifierBuilds[name] = set;
        }
        set.Add(build);
    }

    /// <summary>Load the target schema by name or path.</summary>
    private static GameSchema LoadTargetSchema(string target, string schemasDir)
    {
        // Try as a path first.
        if (Directory.Exists(target) && File.Exists(Path.Combine(target, "manifest.json")))
            return GameSchemaCommand.Load(target);

        // Try as a build name.
        foreach (var subDir in Directory.EnumerateDirectories(schemasDir))
        {
            string manifestPath = Path.Combine(subDir, "manifest.json");
            if (!File.Exists(manifestPath)) continue;
            var manifest = GameSchemaCommand.Load(subDir);
            if (manifest.Build == target)
                return manifest;
        }

        // Not found. List up to 10 close build names.
        var builds = new List<string>();
        foreach (var subDir in Directory.EnumerateDirectories(schemasDir))
        {
            string manifestPath = Path.Combine(subDir, "manifest.json");
            if (!File.Exists(manifestPath)) continue;
            builds.Add(GameSchemaCommand.Load(subDir).Build);
        }
        builds.Sort((a, b) => Gbn.Compare(Gbn.Parse(a), Gbn.Parse(b)));

        // Find close matches (simple substring / prefix).
        var close = builds.Where(b =>
            b.Contains(target, StringComparison.OrdinalIgnoreCase)
            || target.Contains(b.Split('-').FirstOrDefault() ?? "", StringComparison.OrdinalIgnoreCase)
        ).Take(10).ToList();

        throw new FileNotFoundException(
            "build '" + target + "' not found"
            + (close.Count > 0 ? (". Close names: " + string.Join(", ", close)) : "")
            + ". Pass --schemas <dir> to override the schemas location.");
    }

    /// <summary>
    /// Compute when an identifier was introduced relative to a target build.
    ///
    /// <param name="builds">Build names that declare the identifier (from all kinds).</param>
    /// <param name="target">The target build name string.</param>
    /// <returns>(AddedIn, FirstSeen). AddedIn is the earliest non-test/non-beta build
    /// strictly newer than the target. FirstSeen is the earliest build (tests included)
    /// strictly newer than the target, only when that build's Version is strictly lower
    /// than AddedIn's Version. If FirstSeen equals AddedIn, FirstSeen is null.
    /// If AddedIn is null but test builds exist, AddedIn stays null and FirstSeen is
    /// the earliest test build.</returns>
    /// </summary>
    public static (string? AddedIn, string? FirstSeen) Introduced(
        IEnumerable<string> builds, string target)
    {
        var targetParsed = Gbn.Parse(target);
        string? targetLocale = targetParsed.Locale;

        // Parse and filter: keep only builds strictly newer than the target.
        var newer = builds
            .Select(b => (Raw: b, Parsed: Gbn.Parse(b)))
            .Where(pair => Gbn.Compare(pair.Parsed, targetParsed) > 0)
            .ToList();

        if (newer.Count == 0)
            return (null, null);

        // When target has a locale, prefer builds of the same locale.
        var candidates = targetLocale is null
            ? newer
            : newer.Where(pair => pair.Parsed.Locale == targetLocale);
        if (targetLocale is not null && !candidates.Any())
            candidates = newer; // fallback: ignore locale

        var sorted = candidates.ToList();

        // Sort by parsed version (ascending).
        sorted.Sort((a, b) => Gbn.Compare(a.Parsed, b.Parsed));

        // Find earliest release (non-test, non-beta) and earliest overall.
        string? earliestRelease = null;
        string? earliestTest = null;
        foreach ((string raw, _) in sorted)
        {
            var parsed = Gbn.Parse(raw);
            if (parsed.IsTest || parsed.IsBeta)
            {
                if (earliestTest is null)
                    earliestTest = raw;
            }
            else
            {
                if (earliestRelease is null)
                    earliestRelease = raw;
            }
        }

        // When AddedIn is null but test builds exist, firstSeen is the earliest test.
        // When AddedIn equals FirstSeen, FirstSeen stays null.
        if (earliestRelease is null)
        {
            return (null, earliestTest);
        }

        // FirstSeen is only included when that build sorts before AddedIn,
        // meaning it "actually appeared" first. Two builds with the same
        // version are the same release for this purpose.
        if (earliestTest is not null && Gbn.Compare(Gbn.Parse(earliestTest), Gbn.Parse(earliestRelease)) < 0)
        {
            return (earliestRelease, earliestTest);
        }
        return (earliestRelease, null);
    }

    /// <summary>
    /// Check the map's script against the target build.
    /// Declared identifiers in the map's own script are skipped (declared = used by the map author).
    /// Every OTHER identifier that is a native, global, type or function in ANY schema but
    /// absent from the target's scripts/common.j and scripts/blizzard.j is a finding.
    /// </summary>
    private static void CheckScript(
        MapDocument doc,
        string target,
        string schemasDir,
        Dictionary<string, JassNativeDecl> allNatives,
        Dictionary<string, JassGlobalDecl> allGlobals,
        Dictionary<string, JassTypeDecl> allTypes,
        Dictionary<string, JassFunctionDecl> allFunctions,
        Dictionary<string, HashSet<string>> identifierBuilds,
        List<CompatFinding> findings,
        List<string> notChecked)
    {
        // Find the script entry.
        MapFileEntry? scriptEntry = doc.GetFile("war3map.j") ?? doc.GetFile(@"scripts\war3map.j");

        if (scriptEntry is null)
        {
            // Map has no script section (neither war3map.j nor scripts\war3map.j).
            // Put "script" in NotChecked and skip.
            if (!notChecked.Contains("script"))
                notChecked.Add("script");
            return;
        }

        string source = ScriptCommand.ScriptEncoding.GetString(scriptEntry.CurrentBytes);

        // Parse the target schema to know what the target declares.
        GameSchema targetSchema = LoadTargetSchema(target, schemasDir);

        // What the target defines in common.j and blizzard.j, shared with the explain command.
        if (targetSchema.Script is null && !notChecked.Contains("script"))
            notChecked.Add("script");
        var targetSets = TargetDeclaredSets(targetSchema);
        var targetNatives = targetSets.Natives;
        var targetGlobals = targetSets.Globals;
        var targetTypes = targetSets.Types;
        var targetFunctions = targetSets.Functions;

        // Parse the map's script.
        JassApi mapApi = JassApiIndex.Parse(source);

        // Build set of identifiers declared IN the map (skip these).
        var declaredInMap = new HashSet<string>(StringComparer.Ordinal);
        foreach (var nat in mapApi.Natives) declaredInMap.Add(nat.Name);
        foreach (var gl in mapApi.Globals) declaredInMap.Add(gl.Name);
        foreach (var tp in mapApi.Types) declaredInMap.Add(tp.Name);
        foreach (var fn in mapApi.Functions) declaredInMap.Add(fn.Name);

        // Collect ALL identifiers (one per unique name, with occurrence count).
        var identifiers = CollectAllIdentifiers(source, mapApi);

        // Check each unique identifier against the target and the union index.
        // One finding per identifier (first line + occurrence count in detail).

        foreach (var (name, line, count) in identifiers)
        {
            // Skip identifiers declared in the map itself.
            if (declaredInMap.Contains(name)) continue;

            // Only identifiers that SOME schema declares can be findings.
            // Unknown identifiers (map globals, keywords caught by mistake) are never a finding.
            if (!allNatives.ContainsKey(name) && !allGlobals.ContainsKey(name)
                && !allTypes.ContainsKey(name) && !allFunctions.ContainsKey(name))
                continue;

            (string kind, bool inTarget) = (name) switch
            {
                _ => (InferKind(name, allNatives, allGlobals, allTypes, allFunctions),
                    CheckInTarget(name, targetNatives, targetGlobals, targetTypes, targetFunctions))
            };

            if (!inTarget)
            {
                string countText = ", used " + count + " time" + (count > 1 ? "s" : "");
                string detail;

                // Get all builds for this identifier and compute Introduced.
                var builds = identifierBuilds.TryGetValue(name, out var buildSet)
                    ? buildSet.ToList()
                    : Enumerable.Empty<string>().ToList();

                (string? addedIn, string? firstSeen) = Introduced(builds, targetSchema.Build);

                // Build detail text.
                if (addedIn is not null && firstSeen is not null)
                {
                    // Both: "added in A (first seen in F), not in T, used N times"
                    detail = "added in " + addedIn
                        + " (first seen in " + firstSeen + ")"
                        + ", not in " + targetSchema.Build + countText;
                }
                else if (addedIn is not null)
                {
                    // Only A: "added in A, not in T, used N times"
                    detail = "added in " + addedIn + ", not in " + targetSchema.Build + countText;
                }
                else if (firstSeen is not null)
                {
                    // Only F: "only in test builds, first seen in F, not in T, used N times"
                    detail = "only in test builds, first seen in " + firstSeen
                        + ", not in " + targetSchema.Build + countText;
                }
                else
                {
                    detail = "not in " + targetSchema.Build + countText;
                }

                findings.Add(new CompatFinding(
                    "error",
                    kind,
                    name,
                    "line " + line,
                    detail));
            }
        }
    }

    /// <summary>
    /// Tokenize identifiers from the map script outside of string literals and comments.
    /// Returns one entry per unique identifier with (name, firstLine, occurrenceCount).
    /// Arrays like udg_Name[16] yield name udg_Name (strip the bracketed suffix).
    /// </summary>
    private static List<(string Name, int FirstLine, int Count)> CollectAllIdentifiers(
        string source, JassApi mapApi)
    {
        var declaredNames = new HashSet<string>(StringComparer.Ordinal);

        // Collect declared names (these are NOT "used" in the compat sense).
        foreach (var nat in mapApi.Natives) declaredNames.Add(nat.Name);
        foreach (var gl in mapApi.Globals) declaredNames.Add(gl.Name);
        foreach (var tp in mapApi.Types) declaredNames.Add(tp.Name);
        foreach (var fn in mapApi.Functions) declaredNames.Add(fn.Name);

        // Parse line by line, stripping comments and string literals.
        var lines = source.Split('\n');
        var found = new Dictionary<string, (int FirstLine, int Count)>(StringComparer.Ordinal);

        for (int lineNum = 0; lineNum < lines.Length; lineNum++)
        {
            string line = lines[lineNum];
            string cleaned = StripCommentAndStrings(line);
            var tokens = cleaned.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

            foreach (string token in tokens)
            {
                // Strip trailing non-identifier characters (parentheses, dots) so
                // ExtraNative matches the schema entry ExtraNative.
                string name = StripTrailingPunct(token);

                // Strip any bracketed suffix from array access.
                // udg_Name[16] yields udg_Name.
                int bracketIdx = name.IndexOf('[');
                if (bracketIdx > 0)
                {
                    name = name[..bracketIdx];
                }

                // Skip JASS keywords.
                if (IsKeyword(name)) continue;

                // Skip numeric literals.
                if (IsNumeric(token)) continue;

                // Skip empty strings from punctuation-only tokens.
                if (name.Length == 0) continue;

                // Skip identifiers declared in the map.
                if (declaredNames.Contains(name)) continue;

                if (!found.TryGetValue(name, out var entry))
                {
                    found[name] = (lineNum + 1, 1);
                }
                else
                {
                    found[name] = (entry.FirstLine, entry.Count + 1);
                }
            }
        }

        return found.Select(kv => (kv.Key, kv.Value.FirstLine, kv.Value.Count)).ToList();
    }

    /// <summary>
    /// Identifiers a JASS source uses but does not itself declare, skipping keywords and numeric
    /// literals. The same scanner the compat check runs, exposed so a caller that holds a source
    /// (the into-version downgrade) judges identifiers exactly as the compat check does.
    /// </summary>
    internal static IReadOnlyList<(string Name, int FirstLine, int Count)> UsedIdentifiers(string source) =>
        CollectAllIdentifiers(source, JassApiIndex.Parse(source));

    /// <summary>Strip comments (// style) and string literals from a single line.</summary>
    private static string StripCommentAndStrings(string line)
    {
        var result = new StringBuilder();
        bool inString = false;
        bool inComment = false;

        for (int i = 0; i < line.Length; i++)
        {
            if (inComment)
            {
                if (line[i] == '\n')
                {
                    inComment = false;
                    result.AppendLine();
                }
                continue;
            }

            if (inString)
            {
                if (line[i] == '"')
                {
                    inString = false;
                    result.Append('"');
                }
                else if (line[i] == '\\')
                {
                    // Skip escaped character.
                    i++;
                }
                continue;
            }

            // Check for string start.
            if (line[i] == '"')
            {
                inString = true;
                result.Append('"');
                continue;
            }

            // Check for comment start.
            if (i + 1 < line.Length && line[i] == '/' && line[i + 1] == '/')
            {
                break;
            }

            result.Append(line[i]);
        }

        return result.ToString();
    }

    /// <summary>Strip trailing non-identifier characters from a token.
    /// "ExtraNative()" becomes "ExtraNative", "foo.bar" becomes "foo".</summary>
    private static string StripTrailingPunct(string token)
    {
        int i = token.Length - 1;
        while (i >= 0 && !char.IsLetterOrDigit(token[i]) && token[i] != '_')
            i--;
        return i < 0 ? string.Empty : token[..(i + 1)];
    }

    /// <summary>JASS keywords that should not be treated as identifiers.</summary>
    private static bool IsKeyword(string token)
    {
        switch (token)
        {
            case "native": case "constant": case "function": case "globals": case "endglobals":
            case "endfunction": case "type": case "extends": case "endtype":
            case "takes": case "returns": case "local": case "integer": case "real":
            case "boolean": case "string": case "handle": case "code": case "unit":
            case "widget": case "trigger": case "event": case "condition": case "action":
            case "call": case "exitwhen": case "if": case "then": case "else":
            case "endif": case "do": case "loop": case "endloop":
            case "return": case "null": case "true": case "false":
            case "array": case "private": case "static": case "public":
            case "interface": case "implements": case "thisexport": case "export":
            case "debug": case "scope": case "endscope": case "declaration":
            case "variable": case "nothing": case "library": case "requires":
            case "optional": case "initorder": case "onDestroy":
                return true;
            default:
                return false;
        }
    }

    /// <summary>Check if a token looks like a numeric literal.</summary>
    private static bool IsNumeric(string token)
    {
        // Real numbers and integers.
        if (double.TryParse(token, out _)) return true;
        // Hex literals (0x...).
        if (token.StartsWith("0x", StringComparison.Ordinal) || token.StartsWith("0X", StringComparison.Ordinal)) return true;
        // Binary (0b...).
        if (token.StartsWith("0b", StringComparison.Ordinal) || token.StartsWith("0B", StringComparison.Ordinal)) return true;
        return false;
    }

    /// <summary>
    /// Infer the kind of an identifier by looking it up in the union index.
    /// Priority: native > global > type > function (a name can be in multiple).
    /// </summary>
    private static string InferKind(string name,
        Dictionary<string, JassNativeDecl> allNatives,
        Dictionary<string, JassGlobalDecl> allGlobals,
        Dictionary<string, JassTypeDecl> allTypes,
        Dictionary<string, JassFunctionDecl> allFunctions)
    {
        if (allNatives.ContainsKey(name)) return "native";
        if (allGlobals.ContainsKey(name)) return "global";
        if (allTypes.ContainsKey(name)) return "type";
        if (allFunctions.ContainsKey(name)) return "function";
        return "native";
    }

    /// <summary>Check if a name exists in the target's scripts/common.j and scripts/blizzard.j.</summary>
    private static bool CheckInTarget(string name,
        HashSet<string> targetNatives,
        HashSet<string> targetGlobals,
        HashSet<string> targetTypes,
        HashSet<string> targetFunctions)
    {
        string kind = name switch
        {
            _ when targetNatives.Contains(name) => "native",
            _ when targetGlobals.Contains(name) => "global",
            _ when targetTypes.Contains(name) => "type",
            _ when targetFunctions.Contains(name) => "function",
            _ => "identifier",
        };

        return kind switch
        {
            "native" => targetNatives.Contains(name),
            "global" => targetGlobals.Contains(name),
            "type" => targetTypes.Contains(name),
            "function" => targetFunctions.Contains(name),
            _ => false,
        };
    }

    /// <summary>
    /// Check object data fields that were modified in the map but do not exist
    /// in the target build.
    /// </summary>
    private static void CheckObjects(
        MapDocument doc,
        string target,
        string schemasDir,
        List<CompatFinding> findings,
        List<string> notChecked)
    {
        GameSchema targetSchema = LoadTargetSchema(target, schemasDir);

        if (targetSchema.Objects is null)
        {
            if (!notChecked.Contains("objects"))
                notChecked.Add("objects");
            return;
        }

        // Build a lookup from object kind to the field codes defined in that kind's
        // schema file (the objects section of the schema).
        var validFieldsByKind = new Dictionary<ObjectKind, HashSet<string>>(ObjectKindEqualityComparer.Instance);
        foreach (var (schemaFile, fields) in targetSchema.Objects!)
        {
            string? kindName = SchemaKindFromPath(schemaFile);
            if (!Enum.TryParse<ObjectKind>(kindName, out var kind)) continue;
            var fieldSet = new HashSet<string>(StringComparer.Ordinal);
            foreach (var field in fields)
                fieldSet.Add(field.Code);
            validFieldsByKind[kind] = fieldSet;
        }

        // Read map object modifications for each kind using the same method as
        // object list/get. ObjectKinds.MergedEntries reads war3map.* and
        // war3mapSkin.* overlays, yielding per-object field mods.
        foreach (ObjectKind kind in ObjectKinds.All)
        {
            if (!validFieldsByKind.TryGetValue(kind, out var validFields))
                continue;

            var info = ObjectKinds.Info(kind);
            var entries = ObjectKinds.MergedEntries(doc, info);

            foreach (var entry in entries)
            {
                foreach (var mod in entry.Mods)
                {
                    // Stripped field code. For leveled/variation kinds the key
                    // is "code:N", extract the bare code for schema lookup.
                    string bareCode = mod.Key;
                    int colon = bareCode.IndexOf(':');
                    if (colon > 0) bareCode = bareCode[..colon];

                    if (!validFields.Contains(bareCode))
                    {
                        findings.Add(new CompatFinding(
                            "warning",
                            "field",
                            entry.Id.ToRawcode() + "." + bareCode,
                            entry.Id.ToRawcode(),
                            "field '" + bareCode + "' not defined in target build"));
                    }
                }
            }
        }
    }

    /// <summary>Map a schema file path to an object kind name for the kind-lookup table.
    /// Shared with the object commands so a schema file is classified one way.</summary>
    internal static string? SchemaKindFromPath(string schemaPath)
    {
        string lower = schemaPath.ToLowerInvariant();
        if (lower.Contains("unitmetadata")) return "unit";
        if (lower.Contains("itemmetadata")) return "item";
        if (lower.Contains("abilitymetadata")) return "ability";
        if (lower.Contains("destructablemetadata")) return "destructable";
        if (lower.Contains("doodadmetadata")) return "doodad";
        if (lower.Contains("abilitybuffmetadata")) return "buff";
        if (lower.Contains("upgrademetadata")) return "upgrade";
        if (lower.Contains("upgradeeffectmetadata")) return "upgrade";
        if (lower.Contains("miscmetadata")) return null;
        if (lower.Contains("skinmetadata")) return null;
        return null;
    }

    /// <summary>ObjectKind equality for use as a dictionary key.</summary>
    private sealed class ObjectKindEqualityComparer : IEqualityComparer<ObjectKind>
    {
        public static readonly ObjectKindEqualityComparer Instance = new();
        public bool Equals(ObjectKind a, ObjectKind b) => a == b;
        public int GetHashCode(ObjectKind obj) => obj.GetHashCode();
    }

    /// <summary>Count identifiers examined by parsing the script and the target.</summary>
    private static int GetIdentifiersChecked(MapDocument doc, GameSchema targetSchema)
    {
        MapFileEntry? scriptEntry = doc.GetFile("war3map.j") ?? doc.GetFile(@"scripts\war3map.j");
        if (scriptEntry is null || scriptEntry.CurrentBytes.Length == 0)
            return 0;

        string source = ScriptCommand.ScriptEncoding.GetString(scriptEntry.CurrentBytes);
        JassApi api = JassApiIndex.Parse(source);

        // Count identifiers called in the script but not declared by the map.
        // This includes natives/globals/types/functions from the target (which are "checked"
        // because the command examined them against the target).
        var declaredInMap = new HashSet<string>(StringComparer.Ordinal);
        foreach (var nat in api.Natives) declaredInMap.Add(nat.Name);
        foreach (var gl in api.Globals) declaredInMap.Add(gl.Name);
        foreach (var tp in api.Types) declaredInMap.Add(tp.Name);
        foreach (var fn in api.Functions) declaredInMap.Add(fn.Name);

        var lines = source.Split('\n');
        var allUsed = new HashSet<string>(StringComparer.Ordinal);

        foreach (string line in lines)
        {
            string cleaned = StripCommentAndStrings(line);
            var tokens = cleaned.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

            foreach (string token in tokens)
            {
                if (IsKeyword(token) || IsNumeric(token)) continue;
                string name = StripTrailingPunct(token);
                int bracketIdx = name.IndexOf('[');
                if (bracketIdx > 0) name = name[..bracketIdx];
                if (name.Length == 0) continue;
                if (!declaredInMap.Contains(name))
                    allUsed.Add(name);
            }
        }

        return allUsed.Count;
    }

    /// <summary>Count object-data fields examined.</summary>
    private static int GetFieldsChecked(MapDocument doc, GameSchema targetSchema)
    {
        if (targetSchema.Objects is null) return 0;

        int totalFields = 0;

        foreach (ObjectKind kind in ObjectKinds.All)
        {
            var info = ObjectKinds.Info(kind);
            var entries = ObjectKinds.MergedEntries(doc, info);

            foreach (var entry in entries)
            {
                totalFields += entry.Mods.Count;
            }
        }

        return totalFields;
    }
}
