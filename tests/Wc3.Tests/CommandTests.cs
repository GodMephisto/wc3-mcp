using Wc3.Commands;
using Wc3.Model;
namespace Wc3.Tests;

public class CommandTests
{
    private static MapDocument Doc() => MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
    {
        ["war3map.j"] = new byte[] { 1, 2, 3 },
        ["mystery.bin"] = new byte[] { 9 },
    }));

    [Fact]
    public void List_reports_all_files_with_metadata()
    {
        var result = ListCommand.Execute(Doc());
        Assert.Contains(result.Files, f => f.Name == "war3map.j" && f.Known);
        Assert.Contains(result.Files, f => f.Name == "mystery.bin" && !f.Known);
    }

    [Fact]
    public void Diff_detects_added_and_removed_files()
    {
        var a = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]> { ["a.txt"] = new byte[]{1}, ["shared.txt"] = new byte[]{2} }));
        var b = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]> { ["b.txt"] = new byte[]{1}, ["shared.txt"] = new byte[]{2} }));
        var diff = Wc3.Commands.DiffCommand.Execute(a, b);
        Assert.Contains(diff.Entries, e => e.Name == "a.txt" && e.Change == "removed");
        Assert.Contains(diff.Entries, e => e.Name == "b.txt" && e.Change == "added");
        Assert.DoesNotContain(diff.Entries, e => e.Name == "shared.txt");
    }

    [Fact]
    public void Roundtrip_command_reports_faithful_for_unedited_map()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]> { ["war3map.j"] = new byte[]{1,2,3}, ["x.bin"] = new byte[]{4} }));
        var result = Wc3.Commands.RoundtripCommand.Execute(doc);
        Assert.True(result.Faithful, string.Join(",", result.Mismatches));
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Roundtrip_command_reports_faithful_for_unedited_real_map()
    {
        string path = TestCorpus.Map(@"ggg_en_1.11r_slk.w3x");
        if (!File.Exists(path)) return;

        var result = Wc3.Commands.RoundtripCommand.Execute(MapDocument.Load(path));
        Assert.True(result.Faithful, string.Join(",", result.Mismatches));
    }
}
