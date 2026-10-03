// tests/Wc3.Tests/MapLoadPerfTests.cs
// Pins the laziness contract behind MapDocument.Load's deferred entry decompression.
// The motivating measurement, on a 240 MB real map Load spent 2.7 of its 3.3 seconds
// decompressing all 7914 entries (339 MB raw, mostly models and music) while parsing
// the known formats took 118 ms. Load now decompresses a known entry eagerly (its
// parse needs the bytes anyway) and defers every other entry to first access. These
// tests are deliberately not wall-clock timings, they assert the observable contract
// (what materializes when, and that fidelity survives never reading an entry), which
// is what a timing regression would break first.
using Wc3.Model;

namespace Wc3.Tests;

public class MapLoadPerfTests
{
    private static byte[] SampleMap() => SyntheticMap.Build(new Dictionary<string, byte[]>
    {
        ["war3map.w3i"] = new byte[] { 1, 2, 3, 4 },
        ["war3map.j"] = System.Text.Encoding.UTF8.GetBytes("function main takes nothing returns nothing\nendfunction\n"),
        ["mystery.bin"] = new byte[] { 42, 0, 255, 7 },
    });

    [Fact]
    public void Load_defers_decompression_of_unknown_entries()
    {
        var doc = MapDocument.Load(SampleMap());

        // Known entries were parsed at Load, so their bytes are in memory.
        Assert.True(doc.GetFile("war3map.j")!.IsMaterialized);

        // The asset-shaped entry was not read at all.
        Assert.False(doc.GetFile("mystery.bin")!.IsMaterialized);
    }

    [Fact]
    public void Deferred_bytes_match_the_original_content_on_first_access()
    {
        var doc = MapDocument.Load(SampleMap());
        var entry = doc.GetFile("mystery.bin")!;

        Assert.Equal(new byte[] { 42, 0, 255, 7 }, entry.RawBytes);
        Assert.True(entry.IsMaterialized);

        // Second access serves the same array, the read ran exactly once.
        Assert.Same(entry.RawBytes, entry.RawBytes);
    }

    [Fact]
    public void RawSize_answers_without_materializing()
    {
        var doc = MapDocument.Load(SampleMap());
        var entry = doc.GetFile("mystery.bin")!;

        Assert.Equal(4, entry.RawSize);
        Assert.False(entry.IsMaterialized);

        // And it agrees with the real bytes once they do materialize.
        Assert.Equal(entry.RawBytes.Length, entry.RawSize);
    }

    [Fact]
    public void RawSize_reflects_directly_assigned_bytes()
    {
        var doc = MapDocument.Load(SampleMap());
        var added = doc.AddOrReplaceRawFile("war3mapImported\\new.txt", new byte[] { 9, 9 });
        Assert.Equal(2, added.RawSize);
    }

    [Fact]
    public void Never_read_entry_still_writes_its_original_bytes()
    {
        var doc = MapDocument.Load(SampleMap());

        // Dirty an unrelated file so Save has real work to do, then save while
        // mystery.bin was never decompressed in this document.
        doc.AddOrReplaceRawFile("war3map.j", System.Text.Encoding.UTF8.GetBytes("// replaced\n"));
        Assert.False(doc.GetFile("mystery.bin")!.IsMaterialized);
        var saved = doc.SaveToBytes();
        Assert.False(doc.GetFile("mystery.bin")!.IsMaterialized);

        var reloaded = MapDocument.Load(saved);
        Assert.Equal(new byte[] { 42, 0, 255, 7 }, reloaded.GetFile("mystery.bin")!.RawBytes);
        Assert.Equal("// replaced\n", System.Text.Encoding.UTF8.GetString(reloaded.GetFile("war3map.j")!.RawBytes));
    }

    [Fact]
    public void Parsed_but_unmodified_entry_still_writes_its_original_bytes()
    {
        var original = MapDocument.Load(SampleMap());
        var doc = MapDocument.Load(SampleMap());

        // war3map.j is parse-registered, so Load materialized and parsed it. It is
        // not dirty, so Save must carry the original bytes through the archive rebuild.
        var script = doc.GetFile("war3map.j")!;
        Assert.True(script.IsMaterialized);
        Assert.True(script.IsParsed);
        Assert.False(script.IsDirty);

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        Assert.Equal(original.GetFile("war3map.j")!.RawBytes, reloaded.GetFile("war3map.j")!.RawBytes);
    }
}
