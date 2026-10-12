// src/Wc3.Commands/BundleJson.cs
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wc3.Commands;

/// <summary>
/// The on-disk form of a dependency bundle. A bundle resolved from a map is not only a
/// preview to read, it is an artifact worth keeping, so it can be written once and read
/// back later, compared against another, or merged with one. One definition for the writer
/// and the reader so the two cannot drift, and case-insensitive so a bundle produced by the
/// CLI (PascalCase) or the MCP server (camelCase) reopens here either way.
/// </summary>
public static class BundleJson
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    /// <summary>The bundle as indented JSON.</summary>
    public static string Serialize(UnitBundle bundle) =>
        JsonSerializer.Serialize(bundle, Options);

    /// <summary>Parse a bundle. Throws <see cref="InvalidDataException"/> when the text is not a bundle.</summary>
    public static UnitBundle Deserialize(string json) =>
        JsonSerializer.Deserialize<UnitBundle>(json, Options)
        ?? throw new InvalidDataException("the file is not a dependency bundle (JSON parsed to null)");

    /// <summary>Write the bundle to a file, LF-terminated and without a BOM.</summary>
    public static void Save(UnitBundle bundle, string path) =>
        File.WriteAllText(path, Serialize(bundle) + "\n", new UTF8Encoding(false));

    /// <summary>Read a bundle from a file.</summary>
    public static UnitBundle Load(string path) => Deserialize(File.ReadAllText(path));
}
