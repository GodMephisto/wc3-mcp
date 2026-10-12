// tests/Wc3.Tests/GamedataSchemaShipTests.cs
using System.IO.Compression;
using System.Text.Json;
using Wc3.Commands;

namespace Wc3.Tests;

/// <summary>
/// Hermetic tests for the schema dataset shipping commands. Each builds temporary schema folders with
/// GamedataFixtures and runs the command against them, so nothing depends on data/schemas, a game
/// install or the network.
/// </summary>
public sealed class GamedataSchemaShipTests : IDisposable
{
    private readonly string _root = Path.Combine(
        Path.GetTempPath(), "wc3ctl-schemaship-" + Guid.NewGuid().ToString("N"));

    public GamedataSchemaShipTests() => Directory.CreateDirectory(_root);

    public void Dispose()
    {
        try { Directory.Delete(_root, recursive: true); } catch (IOException) { }
    }

    /// <summary>Two builds, one carrying objects and script, one carrying only script.</summary>
    private string SchemasDir()
    {
        var dir = Path.Combine(_root, "schemas");
        Directory.CreateDirectory(dir);
        GamedataFixtures.Save(dir, "TFT-v1.30.1.10211", "units/unitmetadata.slk",
            new[] { GamedataFixtures.Field("Cool1", "int", "0", "100", true) },
            natives: new[] { "NativeA" });
        GamedataFixtures.Save(dir, "3.0.0.24268", functions: new[] { "FuncB" });
        return dir;
    }

    [Fact]
    public void Sources_records_build_era_sections_and_section_hashes()
    {
        var schemas = SchemasDir();
        var outFile = Path.Combine(_root, "sources.json");

        var r = GamedataSourcesCommand.Execute(outFile, schemas);

        Assert.True(File.Exists(outFile));
        Assert.Equal(GameSchemaCommand.SchemaVersion, r.SchemaVersion);
        Assert.Equal(2, r.Sources.Count);

        var full = r.Sources.Single(s => s.Build == "TFT-v1.30.1.10211");
        Assert.Equal("TFT", full.Era);
        Assert.Contains("objects", full.Sections);
        Assert.Contains("script", full.Sections);
        Assert.Contains(full.SectionHashes,
            h => h.Section == "objects" && h.File == "objects.json" && h.Sha256.Length == 64);
        Assert.Contains(full.SectionHashes, h => h.Section == "script" && h.File == "script.json");

        var scriptOnly = r.Sources.Single(s => s.Build == "3.0.0.24268");
        Assert.Equal("Reforged", scriptOnly.Era);
        Assert.DoesNotContain("objects", scriptOnly.Sections);
        Assert.DoesNotContain(scriptOnly.SectionHashes, h => h.Section == "objects");

        using var doc = JsonDocument.Parse(File.ReadAllText(outFile));
        Assert.Equal(2, doc.RootElement.GetProperty("sources").GetArrayLength());
    }

    [Fact]
    public void Sources_carries_the_manifest_input_hashes()
    {
        var dir = Path.Combine(_root, "schemas");
        Directory.CreateDirectory(dir);
        var schema = new GameSchema(GameSchemaCommand.SchemaVersion, "1.30.1.10211", "folder x",
            new[] { new SchemaInput("scripts/common.j", 123, "abc123") }, Array.Empty<string>(),
            null, null, null);
        GameSchemaCommand.Save(schema, Path.Combine(dir, "1.30.1.10211"));

        var r = GamedataSourcesCommand.Execute(Path.Combine(_root, "sources.json"), dir);

        var entry = Assert.Single(r.Sources);
        var input = Assert.Single(entry.Inputs);
        Assert.Equal("scripts/common.j", input.Name);
        Assert.Equal(123, input.Bytes);
        Assert.Equal("abc123", input.Sha256);
    }

    [Fact]
    public void Export_copies_the_whole_dataset_to_a_directory()
    {
        var schemas = SchemasDir();
        var target = Path.Combine(_root, "out");

        var r = GamedataExportSchemasCommand.Execute(target, schemas);

        Assert.False(r.Zipped);
        Assert.Equal(new[] { "3.0.0.24268", "TFT-v1.30.1.10211" }, r.Builds);
        Assert.True(File.Exists(Path.Combine(target, "TFT-v1.30.1.10211", "manifest.json")));
        Assert.True(File.Exists(Path.Combine(target, "TFT-v1.30.1.10211", "objects.json")));
        Assert.True(File.Exists(Path.Combine(target, "3.0.0.24268", "manifest.json")));
        Assert.Equal(r.Files, Directory.EnumerateFiles(target, "*", SearchOption.AllDirectories).Count());
    }

    [Fact]
    public void Export_packs_the_dataset_into_a_single_zip()
    {
        var schemas = SchemasDir();
        var zip = Path.Combine(_root, "wc3-schemas.zip");

        var r = GamedataExportSchemasCommand.Execute(zip, schemas);

        Assert.True(r.Zipped);
        Assert.True(File.Exists(zip));
        using var archive = ZipFile.OpenRead(zip);
        Assert.Contains(archive.Entries, e => e.FullName == "TFT-v1.30.1.10211/manifest.json");
        Assert.Contains(archive.Entries, e => e.FullName == "TFT-v1.30.1.10211/objects.json");
        Assert.Contains(archive.Entries, e => e.FullName == "3.0.0.24268/script.json");
    }
}
