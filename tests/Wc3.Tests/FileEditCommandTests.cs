// tests/Wc3.Tests/FileEditCommandTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Hermetic coverage of FileEditCommand over an in-memory synthetic MPQ. The core
/// contract under test: after a write, every read helper reflects the pending
/// OverrideBytes payload — never the stale (immutable) RawBytes original.
/// </summary>
public class FileEditCommandTests
{
    private const string ScriptName = "war3map.j";
    private const string ScriptText = "call DoNothing()";
    private const string ImportName = @"war3mapImported\note.bin";

    private static MapDocument LoadSynthetic(IDictionary<string, byte[]>? extraFiles = null)
    {
        var files = new Dictionary<string, byte[]> { [ScriptName] = Encoding.ASCII.GetBytes(ScriptText) };
        foreach (var (name, bytes) in extraFiles ?? new Dictionary<string, byte[]>())
            files[name] = bytes;
        return MapDocument.Load(SyntheticMap.Build(files));
    }

    // ---------- AddOrReplace ----------

    [Fact]
    public void AddOrReplace_NewFile_AppearsInListingWithCorrectBytes()
    {
        var doc = LoadSynthetic();
        var payload = new byte[] { 1, 2, 3, 4, 250 };

        var result = FileEditCommand.AddOrReplace(doc, ImportName, payload);

        Assert.False(result.Replaced);
        Assert.Equal(payload.Length, result.SizeBytes);
        var row = Assert.Single(FileEditCommand.ListFiles(doc), f => f.Name == ImportName);
        Assert.Equal(payload.Length, row.SizeBytes);
        Assert.True(row.IsDirty);
        Assert.True(row.HasOverride);
        Assert.Equal(payload, FileEditCommand.ReadBytes(doc, ImportName));
    }

    [Fact]
    public void AddOrReplace_ExistingName_Replaces_AndReadAfterWriteReturnsNewBytes()
    {
        var doc = LoadSynthetic();
        var newBytes = Encoding.ASCII.GetBytes("call KillUnit(GetTriggerUnit())");

        var result = FileEditCommand.AddOrReplace(doc, ScriptName, newBytes);

        // Contract: an already-existing name is REPLACED (mirrors AddOrReplaceRawFile).
        Assert.True(result.Replaced);
        // Read-after-write returns the NEW payload (OverrideBytes path) while the
        // immutable original stays untouched — the classic stale-RawBytes trap.
        Assert.Equal(newBytes, FileEditCommand.ReadBytes(doc, ScriptName));
        Assert.Equal(Encoding.ASCII.GetBytes(ScriptText), doc.GetFile(ScriptName)!.RawBytes);
    }

    [Fact]
    public void ListFiles_And_Stat_ReflectOverrideSize_NotStaleOriginal()
    {
        var doc = LoadSynthetic();
        var bigger = new byte[ScriptText.Length + 100];

        FileEditCommand.AddOrReplace(doc, ScriptName, bigger);

        Assert.Equal(bigger.Length, FileEditCommand.Stat(doc, ScriptName).SizeBytes);
        var row = Assert.Single(FileEditCommand.ListFiles(doc), f => f.Name == ScriptName);
        Assert.Equal(bigger.Length, row.SizeBytes);
    }

    [Fact]
    public void AddOrReplace_SurvivesSaveAndReload()
    {
        var doc = LoadSynthetic();
        var payload = Encoding.ASCII.GetBytes("persisted");
        FileEditCommand.AddOrReplace(doc, ImportName, payload);

        var reloaded = MapDocument.Load(doc.SaveToBytes());

        Assert.Equal(payload, FileEditCommand.ReadBytes(reloaded, ImportName));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AddOrReplace_BlankName_Throws(string name)
    {
        var doc = LoadSynthetic();
        Assert.Throws<ArgumentException>(() => FileEditCommand.AddOrReplace(doc, name, new byte[] { 1 }));
    }

    // ---------- Remove ----------

    [Fact]
    public void Remove_IsNotSupportedByTheModel_FailsLoudly()
    {
        // MapDocument exposes no removal API and its save pipeline only adds/shadows
        // dirty entries when rebuilding the original archive, so FileEditCommand.Remove
        // documents the limitation and fails loudly instead of silently no-opping.
        var doc = LoadSynthetic();
        var ex = Assert.Throws<NotSupportedException>(() => FileEditCommand.Remove(doc, ScriptName));
        Assert.Contains(ScriptName, ex.Message);
    }

    // ---------- text round-trip ----------

    [Fact]
    public void WriteText_ThenReadText_RoundTrips_IncludingNonAscii()
    {
        var doc = LoadSynthetic();
        const string text = "Grunt ready! Ünïcødé — züg-züg";

        FileEditCommand.WriteText(doc, @"war3mapImported\readme.txt", text);

        Assert.Equal(text, FileEditCommand.ReadText(doc, @"war3mapImported\readme.txt"));
    }

    [Fact]
    public void ReadText_ReflectsPriorWriteText_OnExistingFile()
    {
        var doc = LoadSynthetic();

        FileEditCommand.WriteText(doc, ScriptName, "call ReplacedBody()");

        Assert.Equal("call ReplacedBody()", FileEditCommand.ReadText(doc, ScriptName));
    }

    [Fact]
    public void WriteText_EmitsNoBom()
    {
        var doc = LoadSynthetic();

        FileEditCommand.WriteText(doc, @"war3mapImported\plain.txt", "hello");

        Assert.Equal(Encoding.ASCII.GetBytes("hello"),
            FileEditCommand.ReadBytes(doc, @"war3mapImported\plain.txt"));
    }

    [Fact]
    public void ReadText_StripsLeadingBomFromOriginalFile()
    {
        var bommed = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes("STRING 1")).ToArray();
        var doc = LoadSynthetic(new Dictionary<string, byte[]> { ["war3map.wts"] = bommed });

        Assert.Equal("STRING 1", FileEditCommand.ReadText(doc, "war3map.wts"));
    }

    [Fact]
    public void WriteText_CustomEncoding_RoundTrips()
    {
        var doc = LoadSynthetic();
        const string text = "Zwölf Boxkämpfer";

        FileEditCommand.WriteText(doc, @"war3mapImported\utf16.txt", text, Encoding.Unicode);

        Assert.Equal(text, FileEditCommand.ReadText(doc, @"war3mapImported\utf16.txt", Encoding.Unicode));
    }

    // ---------- read helpers ----------

    [Fact]
    public void ReadBytes_MissingFile_Throws()
    {
        var doc = LoadSynthetic();
        Assert.Throws<FileNotFoundException>(() => FileEditCommand.ReadBytes(doc, "no-such-file.txt"));
    }

    [Fact]
    public void ReadBytes_ReturnsDefensiveCopy()
    {
        var doc = LoadSynthetic();

        var copy = FileEditCommand.ReadBytes(doc, ScriptName);
        copy[0] = (byte)'X';

        Assert.Equal((byte)ScriptText[0], FileEditCommand.ReadBytes(doc, ScriptName)[0]);
    }
}
