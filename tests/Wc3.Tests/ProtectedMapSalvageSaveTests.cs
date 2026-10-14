// tests/Wc3.Tests/ProtectedMapSalvageSaveTests.cs
using System.Buffers.Binary;
using System.Text;
using War3Net.IO.Mpq;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// The in-place salvage patch that saves maps whose archive cannot be rebuilt (see
/// MpqSalvagePatcher for the measurements that ruled the rebuild out). The hermetic tests
/// exercise the patcher against archives War3Net itself built, including one corrupted the
/// way protection corrupts them, so the whole path runs in CI. The corpus test is the real
/// success criterion, an edit to a protected map must not corrupt any content that was
/// intact before the save.
/// </summary>
public class ProtectedMapSalvageSaveTests
{
    private readonly ITestOutputHelper _out;
    public ProtectedMapSalvageSaveTests(ITestOutputHelper output) => _out = output;

    private static string Dir => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "Warcraft III", "Maps", "Download");

    private static readonly Dictionary<string, byte[]> SampleFiles = new()
    {
        ["war3map.j"] = Encoding.UTF8.GetBytes(
            "function main takes nothing returns nothing\nendfunction\n"),
        ["war3map.wts"] = Encoding.UTF8.GetBytes("STRING 1\r\n{\r\nHello\r\n}\r\n"),
        ["bystander.txt"] = Encoding.UTF8.GetBytes("must survive every save untouched"),
        ["mystery.bin"] = new byte[] { 42, 0, 255, 7 },
    };

    // ---- patcher-level, hermetic ----

    [Fact]
    public void Replacing_an_entry_survives_a_war3net_reload()
    {
        var mapBytes = SyntheticMap.Build(SampleFiles);
        int header = Wc3.Model.MpqHeader.FindArchiveOffset(mapBytes);
        int blockIndex = FindBlockIndex(mapBytes, "war3map.j");
        var payload = Encoding.UTF8.GetBytes("// replaced by the patcher\n");

        var result = MpqSalvagePatcher.PatchSave(mapBytes, header,
            new (int, string?, byte[])[] { (blockIndex, "war3map.j", payload) });

        Assert.Equal(1, result.ReplacedEntries);
        Assert.Equal(0, result.AddedEntries);
        Assert.True(ReadArchiveFile(result.Bytes, "war3map.j").SequenceEqual(payload));
        Assert.True(ReadArchiveFile(result.Bytes, "bystander.txt")
            .SequenceEqual(SampleFiles["bystander.txt"]));
    }

    [Fact]
    public void Adding_a_file_claims_a_free_hash_slot_and_reloads()
    {
        var mapBytes = SyntheticMap.Build(SampleFiles);
        int header = Wc3.Model.MpqHeader.FindArchiveOffset(mapBytes);
        var payload = Encoding.UTF8.GetBytes("added by the patcher");

        var result = MpqSalvagePatcher.PatchSave(mapBytes, header,
            new (int, string?, byte[])[] { (-1, "war3mapImported\\salvage_added.txt", payload) });

        Assert.Equal(0, result.ReplacedEntries);
        Assert.Equal(1, result.AddedEntries);
        Assert.True(ReadArchiveFile(result.Bytes, "war3mapImported\\salvage_added.txt")
            .SequenceEqual(payload));
        // Growing the block table must not disturb anything that was already there.
        Assert.True(ReadArchiveFile(result.Bytes, "war3map.j").SequenceEqual(SampleFiles["war3map.j"]));
    }

    [Fact]
    public void Adding_a_name_the_archive_already_holds_replaces_instead_of_shadowing()
    {
        // A protected map's entries are live in the hash table even when Load could not
        // recover their names. Adding such a name must patch the existing row, a second
        // row with the same name would sit behind the first forever.
        var mapBytes = SyntheticMap.Build(SampleFiles);
        int header = Wc3.Model.MpqHeader.FindArchiveOffset(mapBytes);
        var payload = Encoding.UTF8.GetBytes("// took over the existing row\n");

        var result = MpqSalvagePatcher.PatchSave(mapBytes, header,
            new (int, string?, byte[])[] { (-1, "war3map.j", payload) });

        Assert.Equal(1, result.ReplacedEntries);
        Assert.Equal(0, result.AddedEntries);
        Assert.True(ReadArchiveFile(result.Bytes, "war3map.j").SequenceEqual(payload));
    }

    [Fact]
    public void A_full_hash_table_refuses_additions_but_still_replaces()
    {
        // Four files into a four-slot table, the shape the protected maps have at 65,536.
        var builder = new MpqArchiveBuilder();
        foreach (var (name, data) in SampleFiles)
            builder.AddFile(MpqFile.New(new MemoryStream(data), name));
        using var mpq = new MemoryStream();
        builder.SaveTo(mpq, new MpqArchiveCreateOptions
        {
            HashTableSize = 4,
            ListFileCreateMode = MpqFileCreateMode.None,
            AttributesCreateMode = MpqFileCreateMode.None,
        }, leaveOpen: true);
        var mapBytes = mpq.ToArray();
        int header = Wc3.Model.MpqHeader.FindArchiveOffset(mapBytes);

        var ex = Assert.Throws<NotSupportedException>(() => MpqSalvagePatcher.PatchSave(
            mapBytes, header,
            new (int, string?, byte[])[] { (-1, "brand_new.txt", new byte[] { 1 }) }));
        Assert.Contains("added", ex.Message);

        var replaced = MpqSalvagePatcher.PatchSave(mapBytes, header,
            new (int, string?, byte[])[]
                { (FindBlockIndex(mapBytes, "mystery.bin"), "mystery.bin", new byte[] { 9, 9 }) });
        Assert.True(ReadArchiveFile(replaced.Bytes, "mystery.bin").SequenceEqual(new byte[] { 9, 9 }));
    }

    // ---- MapDocument-level, hermetic, on an archive corrupted the way protection corrupts ----

    [Fact]
    public void An_unmodified_salvage_save_is_byte_identical()
    {
        var fixture = CorruptBlockRow(SyntheticMap.Build(SampleFiles), "mystery.bin");
        var doc = MapDocument.Load(fixture);

        var saved = doc.SaveToBytes();

        Assert.True(saved.AsSpan().SequenceEqual(fixture));
        Assert.Contains(doc.Diagnostics,
            d => d.FileName == "(archive)" && d.Severity == DiagnosticSeverity.Info);
    }

    [Fact]
    public void A_corrupted_archive_salvage_saves_edits_and_additions_and_reloads()
    {
        var fixture = CorruptBlockRow(SyntheticMap.Build(SampleFiles), "mystery.bin");
        var doc = MapDocument.Load(fixture);
        // Load must have flagged the corrupted entry rather than aborting.
        Assert.Contains(doc.Diagnostics, d => d.Message.Contains("preserved on save"));

        var newScript = Encoding.UTF8.GetBytes(
            "function main takes nothing returns nothing\ncall DoNothing()\nendfunction\n");
        doc.AddOrReplaceRawFile("war3map.j", newScript);
        var added = Encoding.UTF8.GetBytes("added through the salvage path");
        doc.AddOrReplaceRawFile("war3mapImported\\salvage_added.txt", added);

        var saved = doc.SaveToBytes();
        var notice = doc.Diagnostics.Single(
            d => d.FileName == "(archive)" && d.Severity == DiagnosticSeverity.Info);
        _out.WriteLine(notice.Message);
        Assert.Contains("1 replaced", notice.Message);
        Assert.Contains("1 added", notice.Message);

        var reloaded = MapDocument.Load(saved);
        Assert.Equal(doc.Files.Count, reloaded.Files.Count);
        Assert.True(reloaded.GetFile("war3map.j")!.RawBytes.SequenceEqual(newScript));
        Assert.True(reloaded.GetFile("bystander.txt")!.RawBytes
            .SequenceEqual(SampleFiles["bystander.txt"]));
        // The added file's name is in no listfile, so it reopens unnamed. The archive
        // itself must still resolve it by name, which is what the game does.
        Assert.True(ReadArchiveFile(saved, "war3mapImported\\salvage_added.txt").SequenceEqual(added));
        // The corrupted entry is still present and still flagged, not silently dropped.
        Assert.Contains(reloaded.Diagnostics, d => d.Message.Contains("preserved on save"));
    }

    [Fact]
    public void An_archive_that_is_mostly_stuffing_is_patched_not_rebuilt()
    {
        // More unnamed block rows than named ones is the shape a protected map has, and War3Net's
        // builder enumerates one file per block row and re-emits each on save, which doubled the
        // row count on the real Naruto map (1623 rows over 93 names became 3146). This archive
        // builds cleanly, so the rebuild path would be taken, and the save must recognise the
        // stuffing and patch in place instead, keeping the row count.
        var fixture = BuildStuffed();

        var doc = MapDocument.Load(fixture);
        int rowsBefore = doc.Files.Count;
        int unnamedBefore = doc.Files.Count(f => f.FileName is null);
        Assert.True(unnamedBefore > doc.Files.Count - unnamedBefore, "fixture must be mostly stuffing");

        var edited = Encoding.UTF8.GetBytes("// edited through the stuffing route\n");
        doc.AddOrReplaceRawFile("war3map.j", edited);
        var saved = doc.SaveToBytes();

        var reloaded = MapDocument.Load(saved);
        Assert.Equal(rowsBefore, reloaded.Files.Count);   // no re-emitted stuffing
        Assert.True(reloaded.GetFile("war3map.j")!.RawBytes.SequenceEqual(edited));
        Assert.Equal(unnamedBefore, reloaded.Files.Count(f => f.FileName is null));
        Assert.Contains(doc.Diagnostics,
            d => d.FileName == "(archive)" && d.Severity == DiagnosticSeverity.Info);
    }

    [Fact]
    public void A_recovered_name_survives_a_salvage_save()
    {
        // A deprotect pass sets RecoveredFileName without marking the entry dirty, and the salvage
        // path wrote only dirty entries, so the name was dropped and the output came back byte
        // identical to the input. Measured on PumpkinTD, 1529 recovered names lost that way.
        var fixture = CorruptBlockRow(BuildWithoutListFile(), "war3map.wts");
        var doc = MapDocument.Load(fixture);
        var entry = doc.Files.First(f => f.FileName is null && f.RawSize > 0);
        int size = entry.RawSize;
        entry.RecoveredFileName = "war3mapImported\\recovered.bin";

        var saved = doc.SaveToBytes();

        Assert.False(saved.AsSpan().SequenceEqual(fixture));
        Assert.Equal(size, ReadArchiveFile(saved, "war3mapImported\\recovered.bin").Length);
        Assert.Contains(doc.Diagnostics, d => d.Message.Contains("1 named"));
    }

    [Fact]
    public void Naming_the_stuffing_does_not_re_enable_the_rebuild()
    {
        // The named-vs-unnamed check alone is defeated by a deprotect pass, which names the real
        // files and leaves the balance looking healthy, so the rebuild ran and re-emitted the rest.
        // Measured on Naruto Autobattle after a deprotect pass, 1623 rows became 1853 on a save
        // that added names and changed nothing else.
        var doc = MapDocument.Load(BuildStuffed());
        int rowsBefore = doc.Files.Count;
        foreach (var f in doc.Files.Where(f => f.FileName is null))
            f.RecoveredFileName = $"war3mapImported\\recovered_{f.BlockIndex}.bin";

        var saved = doc.SaveToBytes();

        // One row more, the (listfile) carrying the recovered names, not the twelve stuffing rows
        // re-emitted again.
        Assert.Equal(rowsBefore + 1, MapDocument.Load(saved).Files.Count);
    }

    /// <summary>An archive written with no listfile, so every non-standard name loads unnamed.</summary>
    private static byte[] BuildWithoutListFile()
    {
        var builder = new MpqArchiveBuilder();
        foreach (var (name, data) in SampleFiles)
            builder.AddFile(MpqFile.New(new MemoryStream(data), name));
        using var mpq = new MemoryStream();
        builder.SaveTo(mpq, new MpqArchiveCreateOptions
        {
            ListFileCreateMode = MpqFileCreateMode.None,
            AttributesCreateMode = MpqFileCreateMode.None,
        }, leaveOpen: true);
        return mpq.ToArray();
    }

    /// <summary>Four named files against eight stuffing rows, the shape a protected map has. The
    /// stuffing carries distinct names written with no listfile, so it survives War3Net's
    /// name-keyed dedup, unlike MpqFile.New(stream), whose every file hashes to 0.</summary>
    private static byte[] BuildStuffed()
    {
        var builder = new MpqArchiveBuilder();
        foreach (var (name, data) in SampleFiles)
            builder.AddFile(MpqFile.New(new MemoryStream(data), name));
        for (int i = 0; i < 8; i++)
            builder.AddFile(MpqFile.New(
                new MemoryStream(new byte[] { (byte)i, 0, 0, 0 }), $"stuff{i}.bin"));
        using var mpq = new MemoryStream();
        builder.SaveTo(mpq, new MpqArchiveCreateOptions
        {
            ListFileCreateMode = MpqFileCreateMode.None,
            AttributesCreateMode = MpqFileCreateMode.None,
        }, leaveOpen: true);
        return mpq.ToArray();
    }

    [Fact]
    public void An_unserializable_dirty_model_keeps_its_own_error_on_a_corrupted_archive()
    {
        // SerializeEntry's loud refusal must survive the salvage path unchanged. Blaming
        // a caller's unsupported model on map protection would send them debugging the
        // wrong thing.
        var fixture = CorruptBlockRow(SyntheticMap.Build(SampleFiles), "mystery.bin");
        var doc = MapDocument.Load(fixture);
        doc.AddOrReplaceModelFile("war3map.wts", new object());

        var ex = Assert.Throws<NotSupportedException>(() => doc.SaveToBytes());
        Assert.Contains("war3map.wts", ex.Message);
        Assert.Contains("byte-faithful", ex.Message);
    }

    // ---- corpus, the two protected maps this was built for ----

    /// <summary>
    /// The real success criterion from the brief. A salvaged save of a protected map must
    /// keep every named file that was readable before the save readable after it, byte for
    /// byte, with the one edited file carrying its new content, and the saved map must load.
    /// </summary>
    [Theory]
    [InlineData("ORDR_S2_2.305[R]_english.w3x")]
    [InlineData("PumpkinTD_v2.3b.w3x")]
    [Trait("Category", "Corpus")]
    public void A_protected_map_edit_preserves_every_intact_named_file(string mapName)
    {
        var path = Path.Combine(Dir, mapName);
        if (!File.Exists(path)) { _out.WriteLine($"{mapName} absent, skipped"); return; }

        var original = File.ReadAllBytes(path);
        var doc = MapDocument.Load(original);
        var intact = doc.Files
            .Where(f => f.FileName is not null && f.RawBytes.Length > 0)
            .ToDictionary(f => f.FileName!, f => f.RawBytes);
        _out.WriteLine($"{mapName}: {original.Length:N0} bytes, {doc.Files.Count:N0} entries, "
                     + $"{intact.Count} named readable files captured");

        var script = doc.GetFile("war3map.j");
        Assert.NotNull(script);
        var edited = script!.RawBytes
            .Concat(Encoding.ASCII.GetBytes("\n// edited through the salvage save\n")).ToArray();
        doc.AddOrReplaceRawFile(script.FileName!, edited);

        var saved = doc.SaveToBytes();
        var notice = doc.Diagnostics.Single(
            d => d.FileName == "(archive)" && d.Severity == DiagnosticSeverity.Info);
        _out.WriteLine($"saved {saved.Length:N0} bytes ({saved.Length - original.Length:+#,0;-#,0} "
                     + "over the original)");
        _out.WriteLine(notice.Message);

        var reloaded = MapDocument.Load(saved);
        Assert.Equal(doc.Files.Count, reloaded.Files.Count);

        int verified = 0;
        foreach (var (name, before) in intact)
        {
            var entry = reloaded.GetFile(name);
            Assert.True(entry is not null, $"{name} vanished from the salvaged map");
            var expected = string.Equals(name, script.FileName, StringComparison.OrdinalIgnoreCase)
                ? edited : before;
            Assert.True(entry!.RawBytes.SequenceEqual(expected),
                $"{name} does not round-trip through the salvage save");
            verified++;
        }
        _out.WriteLine($"verified {verified} named files byte for byte, 0 entries dropped");
    }

    // ---- helpers ----

    /// <summary>
    /// Corrupts one entry's block row the way the protected maps in the library are
    /// corrupted, a file offset past 2 GB, which makes every open of that entry throw
    /// ArgumentOutOfRangeException from the stream seek and makes MpqArchiveBuilder's
    /// eager constructor abort, so MapDocument.Save takes the salvage path.
    /// </summary>
    private static byte[] CorruptBlockRow(byte[] mapBytes, string victimName)
    {
        var bytes = (byte[])mapBytes.Clone();
        int header = Wc3.Model.MpqHeader.FindArchiveOffset(bytes);
        int victim = FindBlockIndex(bytes, victimName);
        Assert.True(victim >= 0, $"{victimName} not found to corrupt");

        uint tablePos = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(header + 0x14, 4));
        uint tableSize = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(header + 0x1C, 4));
        var table = new byte[tableSize * 16];
        Array.Copy(bytes, header + tablePos, table, 0, table.Length);
        MpqSalvagePatcher.DecryptBlock(table, MpqSalvagePatcher.BlockTableKey);
        BinaryPrimitives.WriteUInt32LittleEndian(table.AsSpan(victim * 16, 4), 0xF0000000u);
        MpqSalvagePatcher.EncryptBlock(table, MpqSalvagePatcher.BlockTableKey);
        Array.Copy(table, 0, bytes, header + tablePos, table.Length);
        return bytes;
    }

    private static int FindBlockIndex(byte[] mapBytes, string fileName)
    {
        using var archive = MpqArchive.Open(new MemoryStream(mapBytes), loadListFile: true);
        // A standard name resolves without a listfile, which is how MapDocument.Load sees it, so a
        // fixture written with no listfile still finds war3map.j and friends here.
        archive.AddFileNames(Wc3.Model.StandardMapFileNames.All);
        int i = 0;
        foreach (var entry in archive)
        {
            if (string.Equals(entry.FileName, fileName, StringComparison.OrdinalIgnoreCase))
                return i;
            i++;
        }
        return -1;
    }

    private static byte[] ReadArchiveFile(byte[] mapBytes, string fileName)
    {
        using var archive = MpqArchive.Open(new MemoryStream(mapBytes), loadListFile: true);
        archive.AddFileName(fileName);
        using var stream = archive.OpenFile(fileName, null, true);
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }
}
