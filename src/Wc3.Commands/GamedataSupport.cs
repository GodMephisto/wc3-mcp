// src/Wc3.Commands/GamedataSupport.cs
namespace Wc3.Commands;

/// <summary>One schema folder loaded from disk, with the build it names and the directory it came from.</summary>
public sealed record LoadedSchema(string Dir, string Build, GameSchema Schema);

/// <summary>
/// Shared loading, build lookup and era labelling for the read-only gamedata commands. Every command
/// resolves the schemas folder, loads each build once into memory, and works on those schemas, so
/// none of them depends on the raw game install.
/// </summary>
public static class GamedataSupport
{
    /// <summary>The schemas directory, resolved the same way as compat and target.</summary>
    public static string ResolveDir(string? schemasDir) => GameSchemaCommand.ResolveSchemasDir(schemasDir);

    /// <summary>Every schema folder under the directory, ordered oldest build first.</summary>
    public static IReadOnlyList<LoadedSchema> LoadAll(string? schemasDir) =>
        LoadAll(schemasDir, GameSchemaCommand.SchemaPart.All);

    /// <summary>
    /// Every schema folder under the directory, ordered oldest build first, reading only the named
    /// sections. Each folder is read and parsed independently, in parallel, because the parse is the
    /// whole cost and it was serial. Measured cold on the shipped 157 folders, the manifest-only
    /// load behind gamedata sources went from 5.9 s to 2.1 s and the full load to about 2 s.
    /// </summary>
    public static IReadOnlyList<LoadedSchema> LoadAll(string? schemasDir, GameSchemaCommand.SchemaPart want)
    {
        var dir = ResolveDir(schemasDir);
        var list = Directory.EnumerateDirectories(dir)
            .Where(sub => File.Exists(Path.Combine(sub, "manifest.json")))
            .OrderBy(d => d, StringComparer.Ordinal)
            .AsParallel().AsOrdered()
            .Select(sub =>
            {
                var schema = GameSchemaCommand.Load(sub, want);
                return new LoadedSchema(sub, schema.Build, schema);
            })
            .ToList();

        // Parse each build name once. The comparer used to parse both sides of every comparison.
        var parsed = list.ToDictionary(o => o.Build, o => GameBuildName.Parse(o.Build), StringComparer.Ordinal);
        list.Sort((a, b) => GameBuildName.Compare(parsed[a.Build], parsed[b.Build]));
        return list;
    }

    /// <summary>
    /// Every schema folder's manifest, without its section payloads, ordered oldest build first.
    /// For a command that reports what a build is rather than what it contains, loading every
    /// build's objects and script parses hundreds of MB for fields the manifest already carries.
    /// </summary>
    public static IReadOnlyList<LoadedSchema> LoadAllManifests(string? schemasDir) =>
        LoadAll(schemasDir, GameSchemaCommand.SchemaPart.None);

    /// <summary>
    /// One build by name or by schema folder path. A directory holding a manifest.json is loaded
    /// directly, otherwise the build name is matched against every folder. Throws
    /// <see cref="FileNotFoundException"/> when neither matches, listing any close names.
    /// </summary>
    public static LoadedSchema Find(string? schemasDir, string build)
    {
        if (Directory.Exists(build) && File.Exists(Path.Combine(build, "manifest.json")))
        {
            var schema = GameSchemaCommand.Load(build);
            return new LoadedSchema(Path.GetFullPath(build), schema.Build, schema);
        }

        // Read each folder's manifest, not its four sections, to find the match. Loading every
        // build's objects and script to locate one of them parsed 355 MB for nothing.
        var dir = ResolveDir(schemasDir);
        var named = Directory.EnumerateDirectories(dir)
            .Where(sub => File.Exists(Path.Combine(sub, "manifest.json")))
            .OrderBy(d => d, StringComparer.Ordinal)
            .Select(sub => (Dir: sub, Build: GameSchemaCommand.LoadScript(sub).Build))
            .ToList();

        foreach (var (sub, name) in named)
            if (string.Equals(name, build, StringComparison.Ordinal))
            {
                var schema = GameSchemaCommand.Load(sub);
                return new LoadedSchema(sub, schema.Build, schema);
            }

        var close = named.Select(o => o.Build)
            .Where(b => b.Contains(build, StringComparison.OrdinalIgnoreCase)
                        || build.Contains(b, StringComparison.OrdinalIgnoreCase))
            .Take(10).ToList();
        throw new FileNotFoundException(
            "build '" + build + "' not found in the schemas directory"
            + (close.Count > 0 ? (", close names " + string.Join(", ", close)) : ""));
    }

    /// <summary>
    /// A short era label from the parsed build name, such as ROC, TFT, Beta ROC, Reforged. A bare
    /// name takes its era from the version, below 1.07 is ROC, through 1.31 is TFT, 1.32 and above
    /// is Reforged.
    /// </summary>
    public static string Era(string build)
    {
        var parsed = GameBuildName.Parse(build);
        if (parsed.IsBeta)
        {
            return parsed.Prefix?.ToUpperInvariant() switch
            {
                "ROC" => "Beta ROC",
                "TFT" => "Beta TFT",
                _ => "Beta",
            };
        }
        if (parsed.Prefix is not null)
        {
            return parsed.Prefix.ToLowerInvariant() switch
            {
                "roc" => "ROC",
                "tft" => "TFT",
                "reforged" => "Reforged",
                _ => parsed.Prefix,
            };
        }
        var version = parsed.Version;
        if (version.Major < 1 || (version.Major == 1 && version.Minor < 7)) return "ROC";
        if (version.Major == 1 && version.Minor < 32) return "TFT";
        return "Reforged";
    }

    /// <summary>The display name a field is shown under, its resolved text or its raw key.</summary>
    public static string DisplayOf(SchemaField field) =>
        string.IsNullOrEmpty(field.DisplayName) ? field.DisplayNameKey : field.DisplayName!;
}
