// src/Wc3.Commands/GamedataSourcesCommand.cs
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wc3.Commands;

/// <summary>One section file present in a schema folder, with its byte size and SHA-256.</summary>
public sealed record SchemaSectionHash(string Section, string File, long Bytes, string Sha256);

/// <summary>
/// One schema folder's provenance. Build and Era name what it is, Sections and SectionHashes describe
/// the section files on disk, and Inputs and Missing carry the manifest's own record of the game files
/// it was derived from.
/// </summary>
public sealed record GamedataSourceEntry(
    string Build, string Era, IReadOnlyList<string> Sections,
    IReadOnlyList<SchemaSectionHash> SectionHashes,
    IReadOnlyList<SchemaInput> Inputs, IReadOnlyList<string> Missing);

/// <summary>The dataset's provenance index, one entry per schema folder, ordered oldest build first.</summary>
public sealed record GamedataSourcesResult(
    int SchemaVersion, IReadOnlyList<GamedataSourceEntry> Sources);

/// <summary>
/// Builds a machine readable provenance index of the schema dataset. For every schema folder it records
/// the build, its era, the sections present, a hash of each section file, and the input hashes from the
/// folder's manifest, so the whole dataset can be traced back to the files it was derived from.
/// </summary>
public static class GamedataSourcesCommand
{
    private static readonly JsonSerializerOptions Json = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    /// <summary>The section files a schema folder may hold, each with the section it belongs to.</summary>
    private static readonly (string Section, string File)[] SectionFiles =
    {
        ("objects", "objects.json"),
        ("triggers", "triggers.json"),
        ("script", "script.json"),
    };

    /// <summary>The provenance index, without writing it, so a caller can render or return it directly.</summary>
    public static GamedataSourcesResult Build(string? schemasDir = null)
    {
        var schemas = GamedataSupport.LoadAll(schemasDir);
        return new GamedataSourcesResult(
            GameSchemaCommand.SchemaVersion, schemas.Select(BuildEntry).ToList());
    }

    private static GamedataSourceEntry BuildEntry(LoadedSchema one)
    {
        var hashes = new List<SchemaSectionHash>();
        foreach (var (section, file) in SectionFiles)
        {
            var path = Path.Combine(one.Dir, file);
            if (!File.Exists(path)) continue;
            var bytes = File.ReadAllBytes(path);
            hashes.Add(new SchemaSectionHash(section, file, bytes.Length,
                Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant()));
        }
        return new GamedataSourceEntry(
            one.Build, GamedataSupport.Era(one.Build), GameSchemaCommand.Sections(one.Schema),
            hashes, one.Schema.Inputs, one.Schema.Missing);
    }

    /// <summary>
    /// Builds the index and writes it as sources.json to <paramref name="outFile"/>. Returns the index
    /// so a caller can report it. The parent directory is created when it does not exist.
    /// </summary>
    public static GamedataSourcesResult Execute(string outFile, string? schemasDir = null)
    {
        var result = Build(schemasDir);
        var path = Path.GetFullPath(outFile);
        var parent = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(parent)) Directory.CreateDirectory(parent);
        File.WriteAllText(path, JsonSerializer.Serialize(result, Json) + "\n", new UTF8Encoding(false));
        return result;
    }
}
