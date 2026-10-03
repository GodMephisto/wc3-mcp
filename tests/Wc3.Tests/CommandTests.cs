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

    [Fact]
    public void List_types_nameless_entries_when_asked_and_never_otherwise()
    {
        var blp = new byte[64];
        System.Text.Encoding.ASCII.GetBytes("BLP1").CopyTo(blp, 0);
        var doc = MapDocument.Load(SyntheticMap.Build(
            new Dictionary<string, byte[]> { ["war3map.j"] = new byte[] { 1, 2, 3 } },
            new[] { blp }));

        var typed = ListCommand.Execute(doc, typeUnnamed: true);
        var unnamed = typed.Files.Single(f => f.Name is null);
        Assert.Equal("BLP texture", unnamed.ContentType);
        // A named entry's name already answers the question, no type is reported for it.
        Assert.All(typed.Files.Where(f => f.Name is not null), f => Assert.Null(f.ContentType));

        // The default stays type-free, the cheap sweep the Studio status line relies on.
        var plain = ListCommand.Execute(doc);
        Assert.All(plain.Files, f => Assert.Null(f.ContentType));
    }

    [Fact]
    public void List_marks_names_the_harvest_recovered()
    {
        var script = "globals\r\nendglobals\r\nfunction Cast takes nothing returns nothing\r\n"
            + "    call AddSpecialEffect(\"war3mapImported\\\\Foo.mdx\", 0, 0)\r\n"
            + "endfunction\r\n";
        var doc = MapDocument.Load(SyntheticMap.BuildProtected(
            visibleFiles: new Dictionary<string, byte[]> { ["(listfile)"] = new byte[] { 0 } },
            hiddenFiles: new Dictionary<string, byte[]>
            {
                ["war3map.j"] = System.Text.Encoding.UTF8.GetBytes(script),
                [@"war3mapImported\Foo.mdx"] = new byte[] { 1, 2, 3 },
            }));
        doc.HarvestAssetNames();

        var r = ListCommand.Execute(doc);
        var recovered = r.Files.Single(f => f.Name == @"war3mapImported\Foo.mdx");
        Assert.True(recovered.NameFromHarvest);
        // The script's own name came from Load's standard-name probe, not from the harvest.
        var scriptEntry = r.Files.Single(f => f.Name == "war3map.j");
        Assert.False(scriptEntry.NameFromHarvest);
    }
}
