// tests/Wc3.Tests/RoundtripTests.cs
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public class RoundtripTests
{
    private static byte[] Sample() => SyntheticMap.Build(new Dictionary<string, byte[]>
    {
        ["war3map.w3i"] = Enumerable.Range(0, 300).Select(i => (byte)i).ToArray(),
        ["war3map.j"]   = System.Text.Encoding.UTF8.GetBytes("function main takes nothing returns nothing\nendfunction\n"),
        ["mystery.bin"] = new byte[] { 42, 0, 255, 7 },
    });

    private static Dictionary<string, byte[]> ContentFilesByName(MapDocument doc) =>
        doc.Files.Where(f => f.FileName != null && !RoundtripCommand.MpqSpecialFiles.Contains(f.FileName!))
                 .ToDictionary(f => f.FileName!, f => f.RawBytes);

    [Fact]
    public void No_edit_roundtrip_preserves_every_file_and_header()
    {
        var original = MapDocument.Load(Sample());
        var rebuilt = MapDocument.Load(original.SaveToBytes());

        Assert.Equal(original.PreArchiveData, rebuilt.PreArchiveData);

        var origByName = ContentFilesByName(original);
        var newByName = ContentFilesByName(rebuilt);

        Assert.Equal(origByName.Keys.OrderBy(k => k), newByName.Keys.OrderBy(k => k));
        foreach (var (name, bytes) in origByName)
            Assert.Equal(bytes, newByName[name]);
    }

    [Fact]
    public void Unnamed_entries_roundtrip_byte_identical()
    {
        // A protected map: entries present in the archive but missing from the
        // listfile, so they reopen with FileName == null.
        var unnamedPayloads = new List<byte[]>
        {
            System.Text.Encoding.UTF8.GetBytes("hidden script payload"),
            new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 },
        };
        var bytes = SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = System.Text.Encoding.UTF8.GetBytes("function main takes nothing returns nothing\nendfunction\n"),
        }, unnamedPayloads);

        var original = MapDocument.Load(bytes);

        // Fixture sanity: the unnamed entries must be present AND readable —
        // an empty-placeholder entry would make the comparison below vacuous.
        var origUnnamed = UnnamedContents(original);
        Assert.Equal(AsMultiset(unnamedPayloads), AsMultiset(origUnnamed));

        var rebuilt = MapDocument.Load(original.SaveToBytes());
        var roundUnnamed = UnnamedContents(rebuilt);
        Assert.Equal(AsMultiset(origUnnamed), AsMultiset(roundUnnamed));
    }

    private static List<byte[]> UnnamedContents(MapDocument doc) =>
        doc.Files.Where(f => f.FileName == null).Select(f => f.RawBytes).ToList();

    // Unnamed entries can't be keyed by name, so compare contents as a multiset.
    private static List<string> AsMultiset(IEnumerable<byte[]> blobs) =>
        blobs.Select(Convert.ToHexString).OrderBy(s => s, StringComparer.Ordinal).ToList();

    [Fact]
    [Trait("Category", "Corpus")]
    public void Real_map_roundtrips_every_file()
    {
        string path = TestCorpus.Map(@"ggg_en_1.11r_slk.w3x");
        if (!File.Exists(path)) return;

        var original = MapDocument.Load(path);
        var rebuilt = MapDocument.Load(original.SaveToBytes());

        var orig = ContentFilesByName(original);
        var round = ContentFilesByName(rebuilt);
        foreach (var (name, bytes) in orig)
            Assert.True(round.ContainsKey(name) && round[name].SequenceEqual(bytes), $"mismatch: {name}");
    }
}
