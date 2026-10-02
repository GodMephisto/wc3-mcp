// tests/Wc3.Tests/MapWriteTests.cs
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Byte-faithfulness of the write-support layer (wave 5a): re-serializing a file
/// through its model must reproduce the original bytes, and adding/replacing files
/// must persist through a save+reload without disturbing the rest of the archive.
/// </summary>
public class MapWriteTests
{
    private static readonly string CorpusPath =
        TestCorpus.Map(@"Anime_WOS2_0.25c1.w3x");

    // Object-data + imports: formats with a byte-faithful War3Net model writer.
    private static readonly string[] ModelSerializableFiles =
    {
        "war3map.w3u", "war3map.w3a", "war3map.w3t", "war3map.w3b",
        "war3map.w3d", "war3map.w3h", "war3map.w3q", "war3map.imp",
        "war3mapSkin.w3u", "war3mapSkin.w3a", "war3mapSkin.w3t", "war3mapSkin.w3b",
        "war3mapSkin.w3d", "war3mapSkin.w3h", "war3mapSkin.w3q",
    };

    /// <summary>
    /// The core invariant: for every model-serializable file the real map actually
    /// contains, re-serializing it (via AddOrReplaceModelFile with its own model,
    /// unchanged) reproduces the original bytes exactly. A single save dirties them
    /// all at once; each is asserted independently so a lossy writer is pinpointed.
    /// </summary>
    [Fact]
    [Trait("Category", "Corpus")]
    public void Reserializing_each_object_data_file_is_byte_identical()
    {
        if (!File.Exists(CorpusPath)) return;

        var original = MapDocument.Load(CorpusPath);
        var doc = MapDocument.Load(CorpusPath);

        var touched = new List<string>();
        foreach (var name in ModelSerializableFiles)
        {
            var entry = doc.GetFile(name);
            if (entry?.Model is null) continue; // format absent in this map
            doc.AddOrReplaceModelFile(name, entry.Model); // same model → pure round-trip
            touched.Add(name);
        }

        Assert.NotEmpty(touched); // the corpus map must exercise at least some formats

        var rebuilt = MapDocument.Load(doc.SaveToBytes());
        var before = ContentFilesByName(original);
        var after = ContentFilesByName(rebuilt);

        foreach (var name in touched)
            Assert.True(after[name].SequenceEqual(before[name]),
                $"{name} writer is NOT byte-faithful (round-trip differs)");

        // Nothing outside the touched set changed.
        foreach (var (name, bytes) in before.Where(kv => !touched.Contains(kv.Key)))
            Assert.True(after[name].SequenceEqual(bytes), $"unexpected change in {name}");
        Assert.Equal(original.PreArchiveData, rebuilt.PreArchiveData);
    }

    /// <summary>
    /// The terrain environment (war3map.w3e) writer must be byte-faithful: re-serializing
    /// the real map's own environment model (unchanged) must reproduce the original bytes
    /// exactly. This pins the War3Net MapEnvironment writer that TerrainCommand relies on;
    /// a non-faithful writer would gratuitously rewrite untouched terrain on every edit.
    /// </summary>
    [Fact]
    [Trait("Category", "Corpus")]
    public void Environment_writer_is_byte_faithful_on_real_map()
    {
        if (!File.Exists(CorpusPath)) return;

        var original = MapDocument.Load(CorpusPath);
        var doc = MapDocument.Load(CorpusPath);

        var entry = doc.GetFile("war3map.w3e");
        Assert.NotNull(entry);          // the corpus map must contain terrain
        Assert.NotNull(entry!.Model);   // and the reader must have parsed it into a model

        // Same model, re-serialized: a pure round-trip through the new w3e writer path.
        doc.AddOrReplaceModelFile("war3map.w3e", entry.Model!);

        var rebuilt = MapDocument.Load(doc.SaveToBytes());
        var before = ContentFilesByName(original)["war3map.w3e"];
        var after = ContentFilesByName(rebuilt)["war3map.w3e"];

        Assert.True(after.SequenceEqual(before),
            "war3map.w3e writer is NOT byte-faithful (round-trip differs)");
    }

    /// <summary>
    /// The sound catalog (war3map.w3s) writer must be byte-faithful: re-serializing the
    /// real map's own sounds model (unchanged) must reproduce the original bytes exactly.
    /// This pins the War3Net MapSounds writer that SoundCommand relies on; a non-faithful
    /// writer would gratuitously rewrite the untouched sound catalog on every edit.
    /// </summary>
    [Fact]
    [Trait("Category", "Corpus")]
    public void Sounds_writer_is_byte_faithful_on_real_map()
    {
        if (!File.Exists(CorpusPath)) return;

        var original = MapDocument.Load(CorpusPath);
        var doc = MapDocument.Load(CorpusPath);

        var entry = doc.GetFile("war3map.w3s");
        Assert.NotNull(entry);          // the corpus map must contain a sound catalog
        Assert.NotNull(entry!.Model);   // and the reader must have parsed it into a model

        // Same model, re-serialized: a pure round-trip through the new w3s writer path.
        doc.AddOrReplaceModelFile("war3map.w3s", entry.Model!);

        var rebuilt = MapDocument.Load(doc.SaveToBytes());
        var before = ContentFilesByName(original)["war3map.w3s"];
        var after = ContentFilesByName(rebuilt)["war3map.w3s"];

        Assert.True(after.SequenceEqual(before),
            "war3map.w3s writer is NOT byte-faithful (round-trip differs)");
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Adding_a_raw_import_persists_and_leaves_others_untouched()
    {
        if (!File.Exists(CorpusPath)) return;

        var original = MapDocument.Load(CorpusPath);
        var doc = MapDocument.Load(CorpusPath);

        const string name = "war3mapImported\\wc3ctl_write_test.bin";
        var payload = new byte[] { 1, 2, 3, 4, 250, 251, 252 };
        doc.AddOrReplaceRawFile(name, payload);

        var rebuilt = MapDocument.Load(doc.SaveToBytes());

        Assert.True(rebuilt.GetFile(name)?.RawBytes.SequenceEqual(payload),
            "added import missing or wrong bytes after save+reload");

        // Every pre-existing named file is byte-identical.
        var before = ContentFilesByName(original);
        var after = ContentFilesByName(rebuilt);
        foreach (var (n, bytes) in before)
            Assert.True(after[n].SequenceEqual(bytes), $"unexpected change in {n}");
        Assert.Equal(original.PreArchiveData, rebuilt.PreArchiveData);
    }

    [Fact]
    public void Replacing_the_script_text_persists()
    {
        var doc = MapDocument.Load(Sample());
        const string newScript = "function main takes nothing returns nothing\n" +
                                 "    call BJDebugMsg(\"ported\")\nendfunction\n";
        doc.AddOrReplaceRawFile("war3map.j", Encoding.UTF8.GetBytes(newScript));

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        Assert.Equal(newScript, Encoding.UTF8.GetString(reloaded.GetFile("war3map.j")!.RawBytes));
        // The bystander object-data file is untouched.
        Assert.NotNull(reloaded.GetFile("war3map.w3u")?.Model);
    }

    [Fact]
    public void Adding_a_brand_new_file_appears_after_reload()
    {
        var doc = MapDocument.Load(Sample());
        var payload = Encoding.UTF8.GetBytes("hello");
        doc.AddOrReplaceRawFile("war3mapImported\\note.txt", payload);

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        Assert.True(reloaded.GetFile("war3mapImported\\note.txt")?.RawBytes.SequenceEqual(payload));
    }

    [Fact]
    public void Injecting_an_ability_object_data_file_into_a_map_without_one()
    {
        var doc = MapDocument.Load(Sample()); // Sample has no war3map.w3a
        Assert.Null(doc.GetFile("war3map.w3a"));

        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        doc.AddOrReplaceModelFile("war3map.w3a", w3a);

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        Assert.IsType<AbilityObjectData>(reloaded.GetFile("war3map.w3a")?.Model);
    }

    [Fact]
    public void Unsupported_dirty_model_throws_rather_than_dropping_changes()
    {
        var doc = MapDocument.Load(Sample());
        // war3map.wts has no byte-faithful model writer; a model-based dirty must fail
        // loudly (the porter appends strings as raw text instead).
        doc.AddOrReplaceModelFile("war3map.wts", new object());
        Assert.Throws<NotSupportedException>(() => doc.SaveToBytes());
    }

    // A synthetic map: a w3u with one custom unit, plus a script and a bystander file.
    private static byte[] Sample()
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        return SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Serialize(w => w.Write(w3u)),
            ["war3map.j"] = Encoding.UTF8.GetBytes(
                "function main takes nothing returns nothing\nendfunction\n"),
            ["mystery.bin"] = new byte[] { 42, 0, 255, 7 },
        });
    }

    private static Dictionary<string, byte[]> ContentFilesByName(MapDocument doc) =>
        doc.Files.Where(f => f.FileName != null && !RoundtripCommand.MpqSpecialFiles.Contains(f.FileName!))
                 .ToDictionary(f => f.FileName!, f => f.RawBytes);

    private static byte[] Serialize(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }
}
