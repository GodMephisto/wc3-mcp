// tests/Wc3.Tests/TriggerWriteTests.cs
using War3Net.Build.Script;
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// The GUI trigger tree was readable and not writable, which made every script edit a one-way
/// operation for anyone who still opens the map in the World Editor: the tree goes stale, and the
/// next editor save regenerates the script from it and discards the edit.
///
/// These pin the write path. Byte-faithfulness is the whole question, because a writer that merely
/// produces a loadable wtg but not the SAME wtg would corrupt a tree on every save of an untouched
/// map.
/// </summary>
public class TriggerWriteTests
{
    private readonly ITestOutputHelper _out;
    public TriggerWriteTests(ITestOutputHelper output) => _out = output;

    private static string CorpusPath => CorpusMap.PathOrEmpty;

    [Fact]
    [Trait("Category", "Corpus")]
    public void The_corpus_actually_resolves_to_a_map_on_this_machine()
    {
        // Stated as its own test because the previous arrangement pinned a file name that went
        // stale, and every Corpus check then skipped while still reporting green.
        _out.WriteLine(CorpusMap.Describe());
        Assert.True(CorpusMap.Available, CorpusMap.Describe());
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Trigger_writer_is_byte_faithful_on_a_real_map()
    {
        if (!File.Exists(CorpusPath)) return;
        _out.WriteLine(CorpusMap.Describe());

        var original = MapDocument.Load(CorpusPath);
        var entry = original.GetFile("war3map.wtg");
        if (entry?.Model is null) return;    // this map has no GUI trigger tree

        var doc = MapDocument.Load(CorpusPath);
        var model = doc.GetFile("war3map.wtg")!.Model!;
        Assert.IsType<MapTriggers>(model);

        // Same model straight back out: a pure round-trip through the writer.
        doc.AddOrReplaceModelFile("war3map.wtg", model);
        var rebuilt = MapDocument.Load(doc.SaveToBytes());

        var before = entry.RawBytes;
        var after = rebuilt.GetFile("war3map.wtg")!.RawBytes;

        _out.WriteLine($"war3map.wtg {before.Length} bytes before, {after.Length} after");
        Assert.True(after.SequenceEqual(before),
            $"war3map.wtg writer is NOT byte-faithful. {before.Length} bytes in, {after.Length} out. "
            + "Editing the trigger tree is unsafe until this holds.");
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Custom_text_triggers_have_no_model_writer_because_it_is_not_faithful()
    {
        // War3Net ships Write(BinaryWriter, MapCustomTextTriggers), and it was added to the write
        // dispatch alongside the trigger tree, and it is NOT byte-faithful. A real map measured
        // 549,168 bytes in and 549,170 out, a two byte gain on a file nobody had touched. Shipping
        // that would grow war3map.wct a little on every save of an untouched map.
        //
        // So the case was removed, and this test pins the removal: a dirty wct must throw rather
        // than silently write something slightly wrong. A loud refusal beats quiet corruption, and
        // without a test the case would drift back in the next time someone notices the writer
        // exists.
        if (!File.Exists(CorpusPath)) return;

        var doc = MapDocument.Load(CorpusPath);
        var entry = doc.GetFile("war3map.wct");
        if (entry?.Model is null) return;   // this map has no custom-text triggers

        doc.AddOrReplaceModelFile("war3map.wct", entry.Model);

        var ex = Assert.Throws<NotSupportedException>(() => doc.SaveToBytes());
        Assert.Contains("war3map.wct", ex.Message);
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void The_tree_a_read_returns_survives_a_write_semantically()
    {
        // Byte equality is the strong claim. This is the weaker one that still matters: even if
        // the bytes shift, the tree must come back with the same categories and triggers, or the
        // write is losing content rather than merely reordering it.
        if (!File.Exists(CorpusPath)) return;

        var doc = MapDocument.Load(CorpusPath);
        if (doc.GetFile("war3map.wtg")?.Model is null) return;

        var before = TriggerReadCommand.GetTriggers(doc);
        doc.AddOrReplaceModelFile("war3map.wtg", doc.GetFile("war3map.wtg")!.Model!);
        var after = TriggerReadCommand.GetTriggers(MapDocument.Load(doc.SaveToBytes()));

        Assert.Equal(before.Categories.Count, after.Categories.Count);
        Assert.Equal(before.Triggers.Count, after.Triggers.Count);
        Assert.Equal(before.Variables.Count, after.Variables.Count);
        Assert.Equal(
            before.Triggers.Select(t => t.Name).OrderBy(n => n, StringComparer.Ordinal),
            after.Triggers.Select(t => t.Name).OrderBy(n => n, StringComparer.Ordinal));
    }
}
