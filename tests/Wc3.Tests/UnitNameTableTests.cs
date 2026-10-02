using Wc3.GameData;
namespace Wc3.Tests;

public class UnitNameTableTests
{
    private sealed class FakeSource : IGameDataSource
    {
        private readonly Dictionary<string, byte[]> _f;
        public FakeSource(Dictionary<string, byte[]> f) => _f = f;
        public byte[]? ReadFile(string name) => _f.TryGetValue(name, out var b) ? b : null;
        public void Dispose() { }
    }

    private static byte[] U(string s) => System.Text.Encoding.UTF8.GetBytes(s);

    private static UnitNameTable BuildTable() => UnitNameTable.FromSources(new FakeSource(new()
    {
        [@"war3.w3mod:_locales\enus.w3mod:units\humanunitstrings.txt"] =
            U("[hfoo]\nName=Footman\nTip=x\n[Hblm]\nName=Blood Mage\n"),
    }));

    [Fact]
    public void Resolves_names_from_ini_sections()
    {
        var table = BuildTable();
        Assert.True(table.TryGetName("hfoo", out var footman));
        Assert.Equal("Footman", footman);
        Assert.True(table.TryGetName("Hblm", out var mage));
        Assert.Equal("Blood Mage", mage);
    }

    [Fact]
    public void Rawcode_lookup_is_case_insensitive()
    {
        var table = BuildTable();
        Assert.True(table.TryGetName("HBLM", out var mage));
        Assert.Equal("Blood Mage", mage);
    }

    [Fact]
    public void Unknown_rawcode_returns_false()
    {
        var table = BuildTable();
        Assert.False(table.TryGetName("xxxx", out _));
    }

    [Fact]
    public void Missing_files_are_skipped()
    {
        var table = UnitNameTable.FromSources(new FakeSource(new()));
        Assert.False(table.TryGetName("hfoo", out _));
    }

    [Fact]
    public void Strips_quotes_and_ignores_other_keys()
    {
        var table = UnitNameTable.FromSources(new FakeSource(new()
        {
            [@"war3.w3mod:_locales\enus.w3mod:units\orcunitstrings.txt"] =
                U("[ogru]\nTip=\"Train Grunt\"\nName=\"Grunt\"\nUbertip=y\n"),
        }));
        Assert.True(table.TryGetName("ogru", out var name));
        Assert.Equal("Grunt", name);
    }
}
