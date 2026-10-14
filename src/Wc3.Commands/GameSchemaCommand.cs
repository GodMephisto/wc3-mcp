// src/Wc3.Commands/GameSchemaCommand.cs
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wc3.GameData;
using Wc3.Model;

namespace Wc3.Commands;

using Gbn = GameBuildName;

/// <summary>One object field as a build's editor defines it, from that build's metadata SLK.</summary>
public sealed record SchemaField(
    string Code, string Slk, string Column, string Type, string DisplayNameKey, string? DisplayName,
    string Category, int Index, int Repeat, int Data, string UseSpecific, string NotSpecific,
    string MinValue, string MaxValue, bool CanBeEmpty,
    bool UseHero, bool UseUnit, bool UseBuilding, bool UseItem);

/// <summary>One legal value of an enumerated object field type, from the build's
/// <c>UI\UnitEditorData.txt</c>. The value is what a map stores, the display name is what the
/// World Editor shows, and the name is null when the WESTRING key could not be resolved.</summary>
public sealed record SchemaEnumValue(string Value, string? DisplayName);

/// <summary>A GUI trigger type, from TriggerData.txt.</summary>
public sealed record SchemaTriggerType(string Name, string? BaseType, bool CanBeGlobal, bool CanBeParameter);

/// <summary>A GUI trigger function, from TriggerData.txt. GameVersion is the file's own tag (0 for
/// Reign of Chaos, 1 for The Frozen Throne and later).</summary>
public sealed record SchemaTriggerFunction(
    string Name, string Kind, int GameVersion, string? ReturnType, IReadOnlyList<string> ArgumentTypes,
    string? Category, string? DisplayName);

public sealed record SchemaTriggers(
    IReadOnlyList<string> Categories, IReadOnlyList<SchemaTriggerType> Types,
    IReadOnlyList<SchemaTriggerFunction> Functions);

/// <summary>An input the schema was built from, so two schemas can be traced to their exact files.</summary>
public sealed record SchemaInput(string Name, long Bytes, string Sha256);

/// <summary>
/// What one game build defines, derived from its files and never copying them. Each section is
/// null when its files were not in the source, which is not the same as the build defining
/// nothing, and a comparison treats it that way.
/// </summary>
public sealed record GameSchema(
    int SchemaVersion, string Build, string Source,
    IReadOnlyList<SchemaInput> Inputs, IReadOnlyList<string> Missing,
    IReadOnlyDictionary<string, IReadOnlyList<SchemaField>>? Objects,
    SchemaTriggers? Triggers,
    IReadOnlyDictionary<string, JassApi>? Script,
    IReadOnlyDictionary<string, IReadOnlyList<SchemaEnumValue>>? Enums = null);

public sealed record SchemaWriteResult(
    string OutDir, string Build, string Source, IReadOnlyList<string> Sections,
    int ObjectFiles, int Fields, int TriggerFunctions, int ScriptFiles, int Natives,
    int EnumTypes, IReadOnlyList<string> Missing);

/// <summary>One difference between two builds. Section is objects, triggers or script, Group the
/// file or list inside it, Change is added, removed or changed.</summary>
public sealed record SchemaChange(string Section, string Group, string Name, string Change, string? Detail);

public sealed record SchemaDiffResult(
    string BuildA, string BuildB, IReadOnlyList<SchemaChange> Changes, IReadOnlyList<string> NotCompared);

/// <summary>An entry summarizing one schema build, as recorded in index.json.</summary>
public sealed record SchemaIndexEntry(string Build, string Source, IReadOnlyList<string> Sections,
    int ObjectFiles, int Fields, int TriggerFunctions,
    IReadOnlyDictionary<string, int> Natives,   // per script file, e.g. scripts/common.j -> 1274
    IReadOnlyDictionary<string, int> Functions, // per script file
    int EnumTypes, IReadOnlyList<string> Missing);

/// <summary>
/// Builds, saves, loads and compares per-build schemas. A schema answers "what does this build's
/// editor show, and what does its compiler accept", which is what decides whether a map made for
/// one build opens and compiles on another.
/// </summary>
public static class GameSchemaCommand
{
    public const int SchemaVersion = 1;

    /// <summary>The metadata SLKs that define the object editor's fields, by storage name.</summary>
    public static readonly IReadOnlyList<string> ObjectMetadataFiles = new[]
    {
        @"war3.w3mod:units\unitmetadata.slk",
        @"war3.w3mod:units\abilitymetadata.slk",
        @"war3.w3mod:units\abilitybuffmetadata.slk",
        @"war3.w3mod:units\destructablemetadata.slk",
        @"war3.w3mod:doodads\doodadmetadata.slk",
        @"war3.w3mod:units\upgrademetadata.slk",
        @"war3.w3mod:units\upgradeeffectmetadata.slk",
        @"war3.w3mod:units\miscmetadata.slk",
        @"war3.w3mod:ui\skinmetadata.slk",
    };

    public const string TriggerDataFile = @"war3.w3mod:ui\triggerdata.txt";

    /// <summary>The World Editor data files that state the closed set of values an enumerated field
    /// type may take, and the human names for them. One section per field type, named by the
    /// <c>type</c> column of a metadata SLK.</summary>
    public static readonly IReadOnlyList<string> EnumFiles = new[]
    {
        @"war3.w3mod:ui\uniteditordata.txt",
    };

    public static readonly IReadOnlyList<string> ScriptFiles = new[]
    {
        @"war3.w3mod:scripts\common.j",
        @"war3.w3mod:scripts\blizzard.j",
        @"war3.w3mod:scripts\common.ai",
    };

    private static readonly string[] StringFiles =
    {
        @"war3.w3mod:_locales\enus.w3mod:ui\worldeditstrings.txt",
        @"war3.w3mod:_locales\enus.w3mod:ui\worldeditgamestrings.txt",
    };

    private static readonly JsonSerializerOptions Json = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    /// <summary>The short name a storage name is recorded under, such as <c>units/unitmetadata.slk</c>.</summary>
    public static string ShortName(string storageName) =>
        storageName[(storageName.LastIndexOf(':') + 1)..].Replace('\\', '/').ToLowerInvariant();

    /// <summary>Reads every schema input the source has. Never throws for a missing file, it is listed instead.</summary>
    public static GameSchema Extract(IGameDataSource src, string build, string source)
    {
        var inputs = new List<SchemaInput>();
        var missing = new List<string>();
        byte[]? Read(string name)
        {
            var bytes = src.ReadFile(name);
            if (bytes is null) missing.Add(ShortName(name));
            else inputs.Add(new(ShortName(name), bytes.Length, Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant()));
            return bytes;
        }

        var strings = WorldEditStrings.FromByteSources(StringFiles.Select(Read).ToArray());
        string? Resolve(string key) => strings.TryGet(key, out var v) ? v : null;

        var objects = new SortedDictionary<string, IReadOnlyList<SchemaField>>(StringComparer.Ordinal);
        foreach (var name in ObjectMetadataFiles)
        {
            if (Read(name) is not { } bytes) continue;
            var meta = ObjectMetadata.FromSlk(SlkTable.Parse(bytes));
            objects[ShortName(name)] = meta.Fields
                .OrderBy(f => f.Code, StringComparer.Ordinal)
                .Select(f => new SchemaField(f.Code, f.SlkName, f.Column, f.Type, f.DisplayName, Resolve(f.DisplayName),
                    f.Category, f.Index, f.Repeat, f.Data, f.UseSpecific, f.NotSpecific, f.MinValue, f.MaxValue,
                    f.CanBeEmpty, f.UseHero, f.UseUnit, f.UseBuilding, f.UseItem))
                .ToList();
        }

        SchemaTriggers? triggers = null;
        if (Read(TriggerDataFile) is { } triggerBytes)
        {
            var catalog = TriggerDataParser.Parse(Encoding.UTF8.GetString(triggerBytes));
            triggers = new SchemaTriggers(
                catalog.Categories.Select(c => c.Name).OrderBy(n => n, StringComparer.Ordinal).ToList(),
                catalog.Types.OrderBy(t => t.Name, StringComparer.Ordinal)
                    .Select(t => new SchemaTriggerType(t.Name, t.BaseType, t.CanBeGlobal, t.CanBeParameter)).ToList(),
                catalog.Functions.OrderBy(f => f.Kind).ThenBy(f => f.Name, StringComparer.Ordinal)
                    .Select(f => new SchemaTriggerFunction(f.Name, f.Kind.ToString(), f.GameVersion, f.ReturnType,
                        f.ArgumentTypes, f.Category, f.DisplayName)).ToList());
        }

        var script = new SortedDictionary<string, JassApi>(StringComparer.Ordinal);
        foreach (var name in ScriptFiles)
            if (Read(name) is { } bytes)
                script[ShortName(name)] = JassApiIndex.Parse(Encoding.UTF8.GetString(bytes));

        // The enumerated field option sets. This is the closed set of legal values for a field
        // type, and the only place the human names live, so a picker can be built from the schema
        // alone rather than from a live install.
        var enums = new SortedDictionary<string, IReadOnlyList<SchemaEnumValue>>(StringComparer.OrdinalIgnoreCase);
        var enumData = EditorEnumData.FromByteSources(strings, EnumFiles.Select(Read).ToArray());
        foreach (var type in enumData.Types.OrderBy(t => t, StringComparer.Ordinal))
        {
            if (!enumData.TryGet(type, out var options) || options.Count == 0) continue;
            enums[type] = options
                .Select(o => new SchemaEnumValue(o.Value, o.DisplayName.Length > 0 ? o.DisplayName : null))
                .ToList();
        }

        return new GameSchema(SchemaVersion, build, source,
            inputs.OrderBy(i => i.Name, StringComparer.Ordinal).ToList(),
            missing.OrderBy(m => m, StringComparer.Ordinal).ToList(),
            objects.Count > 0 ? objects : null, triggers, script.Count > 0 ? script : null,
            enums.Count > 0 ? enums : null);
    }

    /// <summary>
    /// Extracts a schema from an install (CASC) or, when <paramref name="fromFolder"/> is given, from
    /// a folder of loose files, and writes it to <paramref name="outDir"/>. The build is read from the
    /// install's .build.info unless named.
    /// </summary>
    public static SchemaWriteResult Execute(string outDir, string? fromFolder, string? build, string? gameDir)
    {
        GameSchema schema;
        if (fromFolder is not null)
        {
            using var src = new FolderGameDataSource(fromFolder);
            schema = Extract(src, build ?? "unknown", "folder " + Path.GetFileName(Path.GetFullPath(fromFolder).TrimEnd('\\', '/')));
        }
        else
        {
            var install = GameInstall.Locate(gameDir)
                ?? throw new InvalidOperationException("Warcraft III install not found (pass --game-dir <path>, or --from <folder>)");
            if (!CascGameDataSource.TryOpen(install, out var casc, out var err))
                throw new InvalidOperationException($"could not open game data at {install}: {err}");
            using (casc!)
                schema = Extract(casc!, build ?? GameInstall.ReadBuildVersion(install) ?? "unknown", "install");
        }
        Save(schema, outDir);
        return new SchemaWriteResult(Path.GetFullPath(outDir), schema.Build, schema.Source, Sections(schema),
            schema.Objects?.Count ?? 0, schema.Objects?.Values.Sum(f => f.Count) ?? 0,
            schema.Triggers?.Functions.Count ?? 0, schema.Script?.Count ?? 0,
            schema.Script?.Values.Sum(a => a.Natives.Count) ?? 0, schema.Enums?.Count ?? 0, schema.Missing);
    }

    public static IReadOnlyList<string> Sections(GameSchema s) =>
        new[] { s.Objects is null ? null : "objects", s.Triggers is null ? null : "triggers",
                s.Script is null ? null : "script", s.Enums is null ? null : "enums" }
            .Where(x => x is not null).Select(x => x!).ToList();

    /// <summary>Writes manifest.json plus one file per section the schema has. Absent sections write
    /// nothing, and a stale file from an earlier run of the same folder is removed.</summary>
    public static void Save(GameSchema schema, string outDir)
    {
        Directory.CreateDirectory(outDir);
        void Write<T>(string file, T? value) where T : class
        {
            var path = Path.Combine(outDir, file);
            if (value is null) { if (File.Exists(path)) File.Delete(path); return; }
            File.WriteAllText(path, JsonSerializer.Serialize(value, Json) + "\n", new UTF8Encoding(false));
        }
        Write("manifest.json", schema with { Objects = null, Triggers = null, Script = null, Enums = null });
        Write("objects.json", schema.Objects);
        Write("triggers.json", schema.Triggers);
        Write("script.json", schema.Script);
        Write("enums.json", schema.Enums);
    }

    /// <summary>
    /// Which sections of a schema folder to read. The four payloads are about 2.5 MB per build and
    /// a command usually reads one of them, so reading all four to answer a question about objects
    /// parses more than twice what it needs. A section not asked for stays null, exactly as an
    /// absent file does.
    /// </summary>
    [Flags]
    public enum SchemaPart
    {
        None = 0,
        Objects = 1,
        Triggers = 2,
        Script = 4,
        Enums = 8,
        All = Objects | Triggers | Script | Enums,
    }

    public static GameSchema Load(string dir) => Load(dir, SchemaPart.All);

    /// <summary>Read the named sections. Omit a section and its payload is not read at all.</summary>
    public static GameSchema Load(string dir, SchemaPart want)
    {
        var manifestPath = Path.Combine(dir, "manifest.json");
        if (!File.Exists(manifestPath)) throw new FileNotFoundException(manifestPath, $"not a schema folder, no manifest.json in {dir}");
        T? Read<T>(string file) where T : class =>
            File.Exists(Path.Combine(dir, file)) ? JsonSerializer.Deserialize<T>(File.ReadAllText(Path.Combine(dir, file)), Json) : null;
        bool has(SchemaPart p) => (want & p) != 0;

        var manifest = Read<GameSchema>("manifest.json")!;
        return manifest with
        {
            Objects = has(SchemaPart.Objects)
                ? Read<Dictionary<string, List<SchemaField>>>("objects.json")?
                    .ToDictionary(kv => kv.Key, kv => (IReadOnlyList<SchemaField>)kv.Value)
                : null,
            Triggers = has(SchemaPart.Triggers) ? Read<SchemaTriggers>("triggers.json") : null,
            Script = has(SchemaPart.Script) ? Read<Dictionary<string, JassApi>>("script.json") : null,
            Enums = has(SchemaPart.Enums)
                ? Read<Dictionary<string, List<SchemaEnumValue>>>("enums.json")?
                    .ToDictionary(kv => kv.Key, kv => (IReadOnlyList<SchemaEnumValue>)kv.Value)
                : null,
        };
    }

    /// <summary>
    /// The manifest alone, without objects, triggers, script or enums. The manifest carries the
    /// build name, era, section list, inputs and missing list, which is everything a command that
    /// reports what a build IS needs. It is a few KB where the four sections are a few MB.
    /// </summary>
    public static GameSchema LoadManifest(string dir) => Load(dir, SchemaPart.None);

    /// <summary>
    /// The manifest and the script section only, skipping objects, triggers and enums.
    /// The identifier union scan reads every build's script.json and nothing else, and objects.json
    /// is larger than script.json, so loading the whole schema there parses 60% it never looks at.
    /// </summary>
    public static (string Build, IReadOnlyDictionary<string, JassApi>? Script) LoadScript(string dir)
    {
        var schema = Load(dir, SchemaPart.Script);
        return (schema.Build, schema.Script);
    }

    public static SchemaDiffResult Diff(string dirA, string dirB) => Diff(Load(dirA), Load(dirB));

    /// <summary>Filters a diff result to one section, or returns it unchanged when section is null.
    /// Filters both Changes and NotCompared. Throws ArgumentException for an invalid section name.</summary>
    public static SchemaDiffResult Filter(SchemaDiffResult r, string? section)
    {
        if (section is null) return r;
        switch (section)
        {
            case "objects": break;
            case "triggers": break;
            case "script": break;
            case "enums": break;
            default: throw new ArgumentException(
                $"invalid section '{section}' for schema-diff, expected objects, triggers, script or enums");
        }
        var filtered = new SchemaDiffResult(
            r.BuildA, r.BuildB,
            r.Changes.Where(c => c.Section == section).ToList(),
            r.NotCompared.Where(n => n.Split(' ')[0] == section).ToList());
        return filtered;
    }

    /// <summary>
    /// Every difference between two builds, in the sections both have. A section only one of them
    /// has is listed in NotCompared rather than reported as everything added or removed.
    /// </summary>
    public static SchemaDiffResult Diff(GameSchema a, GameSchema b)
    {
        var changes = new List<SchemaChange>();
        var notCompared = new List<string>();

        void Compare<T>(string section, string group, IEnumerable<T> left, IEnumerable<T> right,
            Func<T, string> key, Func<T, T, string?> describe)
        {
            var l = left.GroupBy(key).ToDictionary(g => g.Key, g => g.First(), StringComparer.Ordinal);
            var r = right.GroupBy(key).ToDictionary(g => g.Key, g => g.First(), StringComparer.Ordinal);
            foreach (var k in l.Keys.Union(r.Keys).OrderBy(k => k, StringComparer.Ordinal))
            {
                if (!r.ContainsKey(k)) changes.Add(new(section, group, k, "removed", null));
                else if (!l.ContainsKey(k)) changes.Add(new(section, group, k, "added", null));
                else if (describe(l[k], r[k]) is { } detail) changes.Add(new(section, group, k, "changed", detail));
            }
        }

        if (a.Objects is null || b.Objects is null) notCompared.Add(Missing("objects", a.Objects is null, b.Objects is null, a, b));
        else
            foreach (var file in a.Objects.Keys.Union(b.Objects.Keys).OrderBy(k => k, StringComparer.Ordinal))
            {
                if (!a.Objects.ContainsKey(file) || !b.Objects.ContainsKey(file))
                { notCompared.Add($"objects {file}, only in {(a.Objects.ContainsKey(file) ? a.Build : b.Build)}"); continue; }
                Compare("objects", file, a.Objects[file], b.Objects[file], f => f.Code, DescribeField);
            }

        if (a.Triggers is null || b.Triggers is null) notCompared.Add(Missing("triggers", a.Triggers is null, b.Triggers is null, a, b));
        else
        {
            Compare("triggers", "categories", a.Triggers.Categories, b.Triggers.Categories, c => c, (_, _) => null);
            Compare("triggers", "types", a.Triggers.Types, b.Triggers.Types, t => t.Name,
                (x, y) => x == y ? null : Changed(("base", x.BaseType, y.BaseType),
                    ("global", x.CanBeGlobal.ToString(), y.CanBeGlobal.ToString()),
                    ("parameter", x.CanBeParameter.ToString(), y.CanBeParameter.ToString())));
            Compare("triggers", "functions", a.Triggers.Functions, b.Triggers.Functions, f => f.Kind + " " + f.Name,
                (x, y) => Changed(("returns", x.ReturnType, y.ReturnType),
                    ("arguments", string.Join(", ", x.ArgumentTypes), string.Join(", ", y.ArgumentTypes)),
                    ("category", x.Category, y.Category), ("version", x.GameVersion.ToString(), y.GameVersion.ToString())));
        }

        if (a.Script is null || b.Script is null) notCompared.Add(Missing("script", a.Script is null, b.Script is null, a, b));
        else
            foreach (var file in a.Script.Keys.Union(b.Script.Keys).OrderBy(k => k, StringComparer.Ordinal))
            {
                if (!a.Script.ContainsKey(file) || !b.Script.ContainsKey(file))
                { notCompared.Add($"script {file}, only in {(a.Script.ContainsKey(file) ? a.Build : b.Build)}"); continue; }
                var x = a.Script[file]; var y = b.Script[file];
                Compare("script", file + " types", x.Types, y.Types, t => t.Name,
                    (p, q) => p.Parent == q.Parent ? null : $"extends {p.Parent} -> {q.Parent}");
                Compare("script", file + " natives", x.Natives, y.Natives, n => n.Name,
                    (p, q) => Changed(("signature", Signature(p.Parameters, p.Returns), Signature(q.Parameters, q.Returns))));
                Compare("script", file + " globals", x.Globals, y.Globals, g => g.Name,
                    (p, q) => Changed(("type", p.Type, q.Type), ("constant", p.IsConstant.ToString(), q.IsConstant.ToString()),
                        ("array", p.IsArray.ToString(), q.IsArray.ToString()), ("value", p.Value, q.Value)));
                Compare("script", file + " functions", x.Functions, y.Functions, f => f.Name,
                    (p, q) => Changed(("signature", Signature(p.Parameters, p.Returns), Signature(q.Parameters, q.Returns))));
            }

        if (a.Enums is null || b.Enums is null) notCompared.Add(Missing("enums", a.Enums is null, b.Enums is null, a, b));
        else
            foreach (var type in a.Enums.Keys.Union(b.Enums.Keys).OrderBy(k => k, StringComparer.Ordinal))
            {
                if (!a.Enums.ContainsKey(type) || !b.Enums.ContainsKey(type))
                { notCompared.Add($"enums {type}, only in {(a.Enums.ContainsKey(type) ? a.Build : b.Build)}"); continue; }
                Compare("enums", type, a.Enums[type], b.Enums[type], v => v.Value,
                    (p, q) => p.DisplayName == q.DisplayName ? null
                        : $"name {p.DisplayName ?? "(none)"} -> {q.DisplayName ?? "(none)"}");
            }

        return new SchemaDiffResult(a.Build, b.Build, changes, notCompared);
    }

    private static string Missing(string section, bool inA, bool inB, GameSchema a, GameSchema b) =>
        $"{section}, not in the schema of " + (inA && inB ? $"{a.Build} or {b.Build}" : inA ? a.Build : b.Build);

    private static string Signature(IReadOnlyList<JassParameter> ps, string returns) =>
        $"takes {(ps.Count == 0 ? "nothing" : string.Join(", ", ps.Select(p => p.Type + " " + p.Name)))} returns {returns}";

    private static string? DescribeField(SchemaField x, SchemaField y) => Changed(
        ("type", x.Type, y.Type), ("slk", x.Slk, y.Slk), ("column", x.Column, y.Column),
        ("name", x.DisplayName ?? x.DisplayNameKey, y.DisplayName ?? y.DisplayNameKey),
        ("category", x.Category, y.Category), ("index", x.Index.ToString(), y.Index.ToString()),
        ("levels", x.Repeat.ToString(), y.Repeat.ToString()), ("data", x.Data.ToString(), y.Data.ToString()),
        ("useSpecific", x.UseSpecific, y.UseSpecific), ("notSpecific", x.NotSpecific, y.NotSpecific),
        ("min", x.MinValue, y.MinValue), ("max", x.MaxValue, y.MaxValue),
        ("canBeEmpty", x.CanBeEmpty.ToString(), y.CanBeEmpty.ToString()),
        ("use", Use(x), Use(y)));

    private static string Use(SchemaField f) =>
        $"{(f.UseHero ? "H" : "")}{(f.UseUnit ? "U" : "")}{(f.UseBuilding ? "B" : "")}{(f.UseItem ? "I" : "")}";

    /// <summary>"name a -> b" for each pair that differs, joined, or null when none do.</summary>
    private static string? Changed(params (string Name, string? A, string? B)[] pairs)
    {
        var parts = pairs.Where(p => (p.A ?? "") != (p.B ?? "")).Select(p => $"{p.Name} {Show(p.A)} -> {Show(p.B)}").ToList();
        return parts.Count == 0 ? null : string.Join(", ", parts);
        static string Show(string? s) => string.IsNullOrEmpty(s) ? "(none)" : s;
    }

    /// <summary>The environment variable that points the toolkit at a schemas directory.</summary>
    public const string SchemasDirEnvVar = "WC3_SCHEMAS_DIR";

    /// <summary>
    /// Resolve the schemas directory. An explicit path that exists wins, then the
    /// <see cref="SchemasDirEnvVar"/> environment variable when it names an existing directory, then
    /// a <c>schemas</c> folder beside the executable, then a walk up from the base directory to a
    /// <c>data/schemas</c> folder. Throws <see cref="DirectoryNotFoundException"/> when none is
    /// found.
    /// </summary>
    public static string ResolveSchemasDir(string? schemasDir) =>
        ResolveSchemasDir(schemasDir, Environment.GetEnvironmentVariable(SchemasDirEnvVar));

    /// <summary>
    /// The resolution above with the environment value supplied by the caller, so the precedence is
    /// testable without mutating process environment state. A value that does not name a directory
    /// is ignored, exactly as an explicit path that does not exist is.
    /// </summary>
    internal static string ResolveSchemasDir(string? schemasDir, string? envSchemasDir)
    {
        if (schemasDir is not null && Directory.Exists(schemasDir))
            return Path.GetFullPath(schemasDir);

        if (!string.IsNullOrWhiteSpace(envSchemasDir) && Directory.Exists(envSchemasDir))
            return Path.GetFullPath(envSchemasDir!);

        string baseDir = AppContext.BaseDirectory;
        string candidate1 = Path.Combine(baseDir, "schemas");
        if (Directory.Exists(candidate1))
            return Path.GetFullPath(candidate1);

        for (string? dir = baseDir; dir != null; dir = Directory.GetParent(dir)?.FullName)
        {
            string candidate = Path.Combine(dir, "data", "schemas");
            if (Directory.Exists(candidate))
                return Path.GetFullPath(candidate);
        }

        throw new DirectoryNotFoundException(
            "could not find schemas directory. Pass --schemas <dir>, set " + SchemasDirEnvVar
            + ", place schemas under <AppContext.BaseDirectory>/schemas, or walk up from the base "
            + "directory to data/schemas.");
    }

    /// <summary>
    /// Loads every subfolder in <paramref name="schemasDir"/> holding a manifest.json, builds a
    /// <see cref="SchemaIndexEntry"/> for each one, and returns them sorted by Build with ordinal
    /// comparison. Throws <see cref="FileNotFoundException"/> when no schema folders exist.
    /// </summary>
    public static IReadOnlyList<SchemaIndexEntry> BuildIndex(string schemasDir)
    {
        var entries = new List<SchemaIndexEntry>();
        foreach (var sub in Directory.EnumerateDirectories(schemasDir).OrderBy(d => d, StringComparer.Ordinal))
        {
            var manifestPath = Path.Combine(sub, "manifest.json");
            if (!File.Exists(manifestPath)) continue;
            var schema = Load(sub);
            entries.Add(new SchemaIndexEntry(
                schema.Build,
                schema.Source,
                Sections(schema),
                schema.Objects?.Count ?? 0,
                schema.Objects?.Values.Sum(f => f.Count) ?? 0,
                schema.Triggers?.Functions.Count ?? 0,
                BuildNativesDict(schema),
                BuildFunctionsDict(schema),
                schema.Enums?.Count ?? 0,
                schema.Missing));
        }
        entries.Sort((a, b) => Gbn.Compare(Gbn.Parse(a.Build), Gbn.Parse(b.Build)));
        return entries;
    }

    /// <summary>
    /// Writes <c>index.json</c> into <paramref name="schemasDir"/> from the entries produced by
    /// <see cref="BuildIndex"/>. Returns the absolute path of the file written.
    /// </summary>
    public static string WriteIndex(string schemasDir)
    {
        var entries = BuildIndex(schemasDir);
        var path = Path.Combine(schemasDir, "index.json");
        File.WriteAllText(path, JsonSerializer.Serialize(entries, Json) + "\n", new UTF8Encoding(false));
        return Path.GetFullPath(path);
    }

    private static Dictionary<string, int> BuildNativesDict(GameSchema s)
    {
        if (s.Script is null) return new Dictionary<string, int>(StringComparer.Ordinal);
        var dict = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var kv in s.Script)
        {
            var count = 0;
            foreach (var api in kv.Value.Natives) count++;
            foreach (var api in kv.Value.Functions) count++;
            foreach (var api in kv.Value.Globals) count++;
            if (count > 0) dict[kv.Key] = count;
        }
        return dict;
    }

    private static Dictionary<string, int> BuildFunctionsDict(GameSchema s)
    {
        if (s.Script is null) return new Dictionary<string, int>(StringComparer.Ordinal);
        var dict = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var kv in s.Script)
        {
            var count = 0;
            foreach (var api in kv.Value.Functions) count++;
            if (count > 0) dict[kv.Key] = count;
        }
        return dict;
    }
}
