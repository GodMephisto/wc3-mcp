using Wc3.Commands;
using Wc3.Model;
namespace Wc3.Tests;

public class ExtractCommandTests
{
    // Files shaped like the spec's example map: a model, a texture, a sound, a script,
    // plus one unnamed (protected-style) entry.
    private static MapDocument Doc() => MapDocument.Load(SyntheticMap.Build(
        new Dictionary<string, byte[]>
        {
            [@"war3mapImported\a.mdx"] = new byte[] { 1, 2 },
            ["b.blp"] = new byte[] { 3 },
            [@"sound\c.mp3"] = new byte[] { 4, 5, 6 },
            ["war3map.j"] = new byte[] { 7 },
        },
        new[] { new byte[] { 9, 9 } }));

    [Theory]
    [InlineData("*.mdx", @"war3mapImported\a.mdx", true)]
    [InlineData("*.mdx", @"war3mapImported\A.MDX", true)]   // case-insensitive
    [InlineData("*.mdx", "b.blp", false)]
    [InlineData("war3map*", "war3map.j", true)]
    [InlineData("war3map*", @"war3mapImported\a.mdx", true)]
    [InlineData("*.j", "war3map.j", true)]
    [InlineData("war3map.j", "war3map.j", true)]            // no wildcard = exact
    [InlineData("war3map.j", "war3map.jj", false)]
    public void Glob_matches_star_wildcard_case_insensitive(string pattern, string name, bool expected) =>
        Assert.Equal(expected, ExtractCommand.GlobMatch(pattern, name));

    [Fact]
    public void Models_group_selects_only_mdx()
    {
        var r = ExtractCommand.Execute(Doc(), new ExtractSelector { Patterns = ExtractSelector.GroupPatterns("models") });
        var item = Assert.Single(r.Items);
        Assert.Equal(@"war3mapImported\a.mdx", item.Name);
        Assert.Equal(new byte[] { 1, 2 }, item.Bytes);
    }

    [Fact]
    public void Sounds_group_selects_only_mp3()
    {
        var r = ExtractCommand.Execute(Doc(), new ExtractSelector { Patterns = ExtractSelector.GroupPatterns("sounds") });
        var item = Assert.Single(r.Items);
        Assert.Equal(@"sound\c.mp3", item.Name);
    }

    [Fact]
    public void Exact_name_selects_one_file_case_insensitive()
    {
        var r = ExtractCommand.Execute(Doc(), new ExtractSelector { ExactName = "WAR3MAP.J" });
        var item = Assert.Single(r.Items);
        Assert.Equal("war3map.j", item.Name);
        Assert.Equal(new byte[] { 7 }, item.Bytes);
    }

    [Fact]
    public void All_selects_every_entry_including_unnamed()
    {
        // The archive also carries MPQ bookkeeping files ((listfile), (attributes)),
        // so compare against doc.Files rather than a hardcoded count.
        var doc = Doc();
        var r = ExtractCommand.Execute(doc, new ExtractSelector { All = true });
        Assert.Equal(doc.Files.Count, r.Items.Count);
        Assert.Contains(r.Items, i => i.Name == null);
    }

    [Fact]
    public void Pattern_selection_skips_unnamed_entries()
    {
        var doc = Doc();
        var r = ExtractCommand.Execute(doc, new ExtractSelector { Patterns = new[] { "*" } });
        Assert.Equal(doc.Files.Count(f => f.FileName != null), r.Items.Count);
        Assert.All(r.Items, i => Assert.NotNull(i.Name));
        Assert.Contains(r.Items, i => i.Name == @"war3mapImported\a.mdx");
    }
}
