// tests/Wc3.Tests/SchemaEnumTests.cs
// The enumerated field option sets, captured in the schema from UnitEditorData.txt so a picker
// can be built from the schema alone rather than from a live install.
using Wc3.Commands;
using Wc3.GameData;

namespace Wc3.Tests;

public sealed class SchemaEnumTests
{
    private static string WriteSchemaSource()
    {
        string dir = Path.Combine(Path.GetTempPath(), "wc3enum-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(dir, "ui"));
        File.WriteAllText(Path.Combine(dir, "ui", "uniteditordata.txt"),
            "[attackType]\n00=unknown,WESTRING_NONE\n01=normal,WESTRING_UE_ATTACKTYPE_NORMAL\nNumValues=2\n");
        return dir;
    }

    [Fact]
    public void Extract_captures_an_enumerated_field_type()
    {
        string dir = WriteSchemaSource();
        try
        {
            using var src = new FolderGameDataSource(dir);
            var schema = GameSchemaCommand.Extract(src, "test", "folder test");
            Assert.NotNull(schema.Enums);
            Assert.True(schema.Enums!.ContainsKey("attackType"));
            var values = schema.Enums["attackType"];
            Assert.Equal(2, values.Count);
            Assert.Equal("unknown", values[0].Value);
            Assert.Equal("normal", values[1].Value);
        }
        finally { Directory.Delete(dir, true); }
    }

    [Fact]
    public void Sections_reports_enums()
    {
        string dir = WriteSchemaSource();
        try
        {
            using var src = new FolderGameDataSource(dir);
            var schema = GameSchemaCommand.Extract(src, "test", "folder test");
            Assert.Contains("enums", GameSchemaCommand.Sections(schema));
        }
        finally { Directory.Delete(dir, true); }
    }

    [Fact]
    public void Enums_survive_a_save_and_load_round_trip()
    {
        string dir = WriteSchemaSource();
        string outDir = Path.Combine(Path.GetTempPath(), "wc3enumout-" + Guid.NewGuid().ToString("N"));
        try
        {
            using var src = new FolderGameDataSource(dir);
            var schema = GameSchemaCommand.Extract(src, "TFT-v1.00", "folder test");
            GameSchemaCommand.Save(schema, outDir);
            Assert.True(File.Exists(Path.Combine(outDir, "enums.json")));
            var loaded = GameSchemaCommand.Load(outDir);
            Assert.NotNull(loaded.Enums);
            Assert.Equal(2, loaded.Enums!["attackType"].Count);
        }
        finally { Directory.Delete(dir, true); Directory.Delete(outDir, true); }
    }

    [Fact]
    public void Enum_command_reads_a_builds_option_set()
    {
        string dir = WriteSchemaSource();
        string schemasDir = Path.Combine(Path.GetTempPath(), "wc3enumschemas-" + Guid.NewGuid().ToString("N"));
        try
        {
            using var src = new FolderGameDataSource(dir);
            var schema = GameSchemaCommand.Extract(src, "TFT-v1.00", "folder test");
            GameSchemaCommand.Save(schema, Path.Combine(schemasDir, "TFT-v1.00"));

            var r = GamedataEnumCommand.Execute("attackType", "TFT-v1.00", schemasDir);
            Assert.Single(r.Entries);
            Assert.Equal(2, r.Entries[0].Values.Count);
            Assert.Equal("unknown", r.Entries[0].Values[0].Value);

            Assert.Contains("attackType", GamedataEnumCommand.Types("TFT-v1.00", schemasDir));
        }
        finally { Directory.Delete(dir, true); Directory.Delete(schemasDir, true); }
    }
}
