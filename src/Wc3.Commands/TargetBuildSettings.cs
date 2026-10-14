// src/Wc3.Commands/TargetBuildSettings.cs
using System.Text.Json;

namespace Wc3.Commands;

/// <summary>
/// Where a target setting comes from, for display and debugging.
/// </summary>
public enum TargetSource
{
    /// <summary>An explicit --target flag on the command line.</summary>
    Option,
    /// <summary>The WC3_TARGET_BUILD environment variable.</summary>
    Environment,
    /// <summary>The persisted settings file on disk.</summary>
    Settings,
    /// <summary>No user setting, derived from the installed game build or newest schema.</summary>
    Default,
}

/// <summary>
/// A target build that also reports where it was resolved from.
/// </summary>
public sealed record TargetBuildResult(string Build, TargetSource Source, IReadOnlyList<string>? Sections = null);

/// <summary>
/// Per-build schema metadata, used by target list.
/// </summary>
public sealed record BuildInfo(string Build, IReadOnlyList<string> Sections);

/// <summary>
/// Reads and writes the user's target build setting. The setting controls which
/// game build (schema) the toolkit checks maps against by default.
///
/// The file lives at %APPDATA%/wc3ctl/settings.json on Windows and
/// ~/.config/wc3ctl/settings.json on macOS (ApplicationData maps to ~/.config).
/// An environment variable WC3_TARGET_BUILD overrides the file. An explicit
/// --target overrides both.
/// </summary>
public static class TargetBuildSettings
{
    private const string SettingsKey = "targetBuild";

    private static Lazy<string?> SettingsDir = new(() =>
    {
        var envDir = Environment.GetEnvironmentVariable("WC3CTL_SETTINGS_DIR");
        return string.IsNullOrWhiteSpace(envDir) ? null : envDir;
    });

    /// <summary>Reset the cached settings directory. For testing only.</summary>
    internal static void ResetSettingsDir() => SettingsDir = new(() =>
    {
        var envDir = Environment.GetEnvironmentVariable("WC3CTL_SETTINGS_DIR");
        return string.IsNullOrWhiteSpace(envDir) ? null : envDir;
    });

    /// <summary>
    /// The file path where settings are stored. Tests can override this through
    /// the WC3CTL_SETTINGS_DIR environment variable at runtime.
    /// </summary>
    internal static string FilePath
    {
        get
        {
            string? baseDir = SettingsDir.Value;
            if (baseDir is not null)
                return Path.Combine(baseDir, "settings.json");
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "wc3ctl", "settings.json");
        }
    }

    /// <summary>
    /// Get the current target build, resolving through the precedence chain:
    /// option (explicit --target) > environment (WC3_TARGET_BUILD) > settings
    /// file > default (installed game build or newest schema).
    /// </summary>
    /// <param name="option">
    /// An explicit --target value, or null to fall through to lower tiers.
    /// </param>
    /// <param name="schemasDir">
    /// Path to a schemas directory, or null to use the default location.
    /// </param>
    public static TargetBuildResult Get(string? option = null, string? schemasDir = null)
    {
        // Every tier carries the resolved build's schema sections, so a caller can see what the
        // target actually offers without a second call. The description promises this and it used
        // to be dropped on every path.
        TargetBuildResult Result(string build, TargetSource source) =>
            new(build, source, SectionsFor(build, schemasDir));

        // Tier 1: explicit option.
        if (option is not null && option.Length > 0)
            return Result(option, TargetSource.Option);

        // Tier 2: environment variable.
        var envBuild = Environment.GetEnvironmentVariable("WC3_TARGET_BUILD");
        if (envBuild is not null && envBuild.Length > 0)
            return Result(envBuild, TargetSource.Environment);

        // Tier 3: settings file.
        try
        {
            string path = FilePath;
            if (File.Exists(path))
            {
                var content = File.ReadAllText(path);
                var obj = JsonDocument.Parse(content);
                if (obj.RootElement.TryGetProperty(SettingsKey, out JsonElement kb)
                    && kb.GetString() is { } stored && stored.Length > 0)
                {
                    return Result(stored, TargetSource.Settings);
                }
            }
        }
        catch { /* corrupt or unreadable, fall through to default */ }

        // Tier 4 derives default from installed game or newest schema.
        return Result(ResolveDefaultBuild(schemasDir), TargetSource.Default);
    }

    /// <summary>The sections of the named build's schema, or empty when no schema matches it.</summary>
    private static IReadOnlyList<string> SectionsFor(string build, string? schemasDir)
    {
        try
        {
            foreach (var info in List(schemasDir))
                if (string.Equals(info.Build, build, StringComparison.OrdinalIgnoreCase))
                    return info.Sections;
        }
        catch { /* schemas unavailable, sections stay empty */ }
        return Array.Empty<string>();
    }

    /// <summary>
    /// Persist the given build as the user's target. Validates it against the
    /// schemas folder before writing.
    /// </summary>
    /// <param name="build">
    /// A build name (e.g. "TFT-v1.30.1.10211") or a schema folder path.
    /// </param>
    /// <param name="schemasDir">
    /// Path to a schemas directory, or null to use the default location.
    /// </param>
    public static void Set(string build, string? schemasDir = null)
    {
        var resolvedSchemas = ResolveSchemasDir(schemasDir);

        // A schema folder is a valid target in its own right, so persist the path unchanged. Any
        // other value must name a build the schemas folder declares.
        bool isPath = Directory.Exists(build) && File.Exists(Path.Combine(build, "manifest.json"));
        if (!isPath)
        {
            var names = BuildNames(resolvedSchemas);
            if (!names.Contains(build, StringComparer.Ordinal))
            {
                var close = Suggest(build, names);
                throw new FileNotFoundException(
                    "build '" + build + "' not found"
                    + (close.Count > 0 ? (". Close names: " + string.Join(", ", close)) : ""));
            }
        }

        WriteTarget(build);
    }

    /// <summary>
    /// Resolve one target to its loaded <see cref="GameSchema"/>, the same way compat and the
    /// target commands resolve it. The target is either a schema folder path (holding a
    /// manifest.json) or a build name under the schemas directory. Throws
    /// <see cref="FileNotFoundException"/> with close build names when the target is unknown.
    /// </summary>
    public static GameSchema LoadSchema(string target, string? schemasDir = null)
    {
        if (Directory.Exists(target) && File.Exists(Path.Combine(target, "manifest.json")))
            return GameSchemaCommand.Load(target);

        // The manifest to find the match, then the one full schema. Loading every build's four
        // sections to locate one of them parsed hundreds of MB and was most of compat explain's
        // six seconds, measured.
        var resolvedSchemas = ResolveSchemasDir(schemasDir);
        var names = new List<string>();
        foreach (var subDir in Directory.EnumerateDirectories(resolvedSchemas))
        {
            if (!File.Exists(Path.Combine(subDir, "manifest.json"))) continue;
            var build = GameSchemaCommand.LoadManifest(subDir).Build;
            if (build == target) return GameSchemaCommand.Load(subDir);
            names.Add(build);
        }

        var close = Suggest(target, names);
        throw new FileNotFoundException(
            "build '" + target + "' not found"
            + (close.Count > 0 ? (". Close names: " + string.Join(", ", close)) : "")
            + ". Pass --schemas <dir> to override the schemas location.");
    }

    /// <summary>Every build name the schemas folder declares.</summary>
    private static List<string> BuildNames(string schemasDir)
    {
        var names = new List<string>();
        foreach (var subDir in Directory.EnumerateDirectories(schemasDir))
        {
            if (!File.Exists(Path.Combine(subDir, "manifest.json"))) continue;
            names.Add(GameSchemaCommand.LoadManifest(subDir).Build);
        }
        return names;
    }

    /// <summary>
    /// The up-to-<paramref name="max"/> closest build names to an unknown input, nearest first.
    /// A build that starts with the input ranks above one that merely contains it, and both rank
    /// above the rest, which are ordered by Levenshtein distance over the lowercased names. So
    /// "1.30.1" suggests TFT-v1.30.1.10211 and "tft-v1.31" suggests the 1.31 builds. It works on
    /// plain strings, so it is unit testable with no schemas folder.
    /// </summary>
    internal static IReadOnlyList<string> Suggest(
        string input, IReadOnlyList<string> builds, int max = 10)
    {
        string needle = (input ?? string.Empty).Trim();
        if (needle.Length == 0 || builds.Count == 0 || max <= 0) return Array.Empty<string>();

        string lowered = needle.ToLowerInvariant();
        return builds
            .Select(b => (
                Build: b,
                Rank: b.StartsWith(needle, StringComparison.OrdinalIgnoreCase) ? 0
                    : b.Contains(needle, StringComparison.OrdinalIgnoreCase) ? 1
                    : 2,
                Distance: Levenshtein(lowered, b.ToLowerInvariant())))
            .OrderBy(r => r.Rank)
            .ThenBy(r => r.Distance)
            .ThenBy(r => r.Build, StringComparer.Ordinal)
            .Take(max)
            .Select(r => r.Build)
            .ToList();
    }

    /// <summary>Plain Levenshtein edit distance between two short strings.</summary>
    private static int Levenshtein(string a, string b)
    {
        var previous = new int[b.Length + 1];
        var current = new int[b.Length + 1];
        for (int j = 0; j <= b.Length; j++) previous[j] = j;
        for (int i = 1; i <= a.Length; i++)
        {
            current[0] = i;
            for (int j = 1; j <= b.Length; j++)
            {
                int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                current[j] = Math.Min(
                    Math.Min(current[j - 1] + 1, previous[j] + 1), previous[j - 1] + cost);
            }
            (previous, current) = (current, previous);
        }
        return previous[b.Length];
    }

    /// <summary>
    /// Writes targetBuild and keeps every other key. <c>JsonProperty.WriteTo</c> writes a property's
    /// name and value together, so the replaced key is skipped entirely rather than named a second
    /// time, which is what produced a duplicate targetBuild. A file that cannot be parsed starts
    /// fresh rather than aborting the set.
    /// </summary>
    private static void WriteTarget(string build)
    {
        string? dir = Path.GetDirectoryName(FilePath);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

        JsonDocument existingDoc;
        try
        {
            existingDoc = File.Exists(FilePath)
                ? JsonDocument.Parse(File.ReadAllText(FilePath))
                : JsonDocument.Parse("{}");
        }
        catch (JsonException)
        {
            existingDoc = JsonDocument.Parse("{}");
        }

        using (existingDoc)
        {
            // The stream is disposed explicitly, Utf8JsonWriter does not release it, and leaving it
            // open kept settings.json locked so a second Set in the same process failed to open it.
            using var stream = System.IO.File.Open(
                FilePath, FileMode.Create, FileAccess.Write, FileShare.Read);
            using var writer = new Utf8JsonWriter(stream);
            writer.WriteStartObject();
            foreach (var prop in existingDoc.RootElement.EnumerateObject())
            {
                if (prop.Name == SettingsKey) continue;
                prop.WriteTo(writer);
            }
            writer.WriteString(SettingsKey, build);
            writer.WriteEndObject();
            writer.Flush();
        }
    }

    /// <summary>
    /// Clear (delete) the user's target setting, returning to default behavior.
    /// </summary>
    public static void Clear()
    {
        if (File.Exists(FilePath))
            File.Delete(FilePath);
    }

    /// <summary>
    /// List every build in the schemas index with its sections.
    /// </summary>
    /// <param name="schemasDir">
    /// Path to a schemas directory, or null to use the default location.
    /// </param>
    /// <param name="fullOnly">
    /// When true, skip builds that are scripts-only (no objects).
    /// </param>
    public static IReadOnlyList<BuildInfo> List(string? schemasDir = null, bool fullOnly = false)
    {
        var resolvedSchemas = ResolveSchemasDir(schemasDir);
        string cacheKey = $"{resolvedSchemas}|{fullOnly}";

        // Cached, because answering it parses every schema in the directory in full just to read
        // its build name and section list. Measured at 5,256ms for 159 builds, and the Studio's
        // object panel asks on every ShowMap, which made opening that tab hold the UI thread for
        // five seconds. The schemas are a shipped asset, so the answer only changes with the
        // directory, which is part of the key.
        lock (ListCache)
            if (ListCache.TryGetValue(cacheKey, out var cached)) return cached;

        var result = new List<BuildInfo>();
        foreach (var subDir in Directory.EnumerateDirectories(resolvedSchemas))
        {
            string manifestPath = Path.Combine(subDir, "manifest.json");
            if (!File.Exists(manifestPath)) continue;

            // The manifest and the section files, not the sections themselves. This loop asks only
            // for each build's name and which sections it holds, and loading the payloads to answer
            // that parsed 400 MB. It is the reason a fresh CLI process spent five seconds before it
            // reached any work, measured on compat explain.
            var manifest = GameSchemaCommand.LoadManifest(subDir);
            if (fullOnly && !GameSchemaCommand.HasObjectData(subDir)) continue;

            result.Add(new BuildInfo(manifest.Build, GameSchemaCommand.SectionsOnDisk(subDir)));
        }

        result.Sort((a, b) => Wc3.Commands.GameBuildName.Compare(
            Wc3.Commands.GameBuildName.Parse(a.Build),
            Wc3.Commands.GameBuildName.Parse(b.Build)));

        lock (ListCache) ListCache[cacheKey] = result;
        return result;
    }

    private static readonly Dictionary<string, IReadOnlyList<BuildInfo>> ListCache =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Drop the <see cref="List"/> cache. For testing only.</summary>
    public static void ResetListCache()
    {
        lock (ListCache) ListCache.Clear();
    }

    /// <summary>
    /// Resolve the schemas directory, matching the same logic as TargetCompatCommand.
    /// Delegates to <see cref="GameSchemaCommand.ResolveSchemasDir"/> so every caller shares one rule.
    /// </summary>
    private static string ResolveSchemasDir(string? schemasDir) =>
        GameSchemaCommand.ResolveSchemasDir(schemasDir);

    /// <summary>
    /// Derive the default build by checking if a schema exists matching the installed
    /// game version, or fall back to the newest schema by version number.
    /// </summary>
    private static string ResolveDefaultBuild(string? schemasDir)
    {
        var resolvedSchemas = ResolveSchemasDir(schemasDir);
        string? defaultBuild = null;
        string? newestBuild = null;
        var newestParsed = default(Wc3.Commands.GameBuildName);

        // Resolve the installed build once. Reading it inside the loop opened the registry and the
        // install folder once per schema, which is what made every MCP session take seconds when a
        // schemas dataset is present.
        string? installBuild = null;
        try
        {
            string? installDir = Wc3.GameData.GameInstall.Locate();
            if (installDir is not null) installBuild = Wc3.GameData.GameInstall.ReadBuildVersion(installDir);
        }
        catch { /* no install, leave the default null */ }

        foreach (var subDir in Directory.EnumerateDirectories(resolvedSchemas))
        {
            string manifestPath = Path.Combine(subDir, "manifest.json");
            if (!File.Exists(manifestPath)) continue;

            // The build name only, so the manifest alone.
            var schema = GameSchemaCommand.LoadManifest(subDir);

            if (installBuild is not null && installBuild == schema.Build)
                defaultBuild = schema.Build;

            // Track the newest schema by version.
            var parsed = Wc3.Commands.GameBuildName.Parse(schema.Build);
            if (default(Wc3.Commands.GameBuildName) == newestParsed
                || Wc3.Commands.GameBuildName.Compare(parsed, newestParsed) > 0)
            {
                newestBuild = schema.Build;
                newestParsed = parsed;
            }
        }

        return defaultBuild ?? (newestBuild ?? "unknown");
    }
}
