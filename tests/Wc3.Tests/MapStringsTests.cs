// tests/Wc3.Tests/MapStringsTests.cs
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Script;
using Wc3.Model;

namespace Wc3.Tests;

public class MapStringsTests
{
    // Builds a map whose war3map.wts is a real TriggerStrings serialized with
    // War3Net's own writer, so From() exercises the full load+parse path.
    private static MapDocument WithWts(params (uint Key, string Value)[] entries)
    {
        var wts = new TriggerStrings();
        foreach (var (key, value) in entries)
            wts.Strings.Add(new TriggerString { Key = key, Value = value });
        using var ms = new MemoryStream();
        using (var sw = new StreamWriter(ms, Encoding.UTF8, leaveOpen: true))
            sw.WriteTriggerStrings(wts);
        return MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.wts"] = ms.ToArray(),
        }));
    }

    [Fact]
    public void Resolves_trigstr_reference_from_map_wts()
    {
        var strings = MapStrings.From(WithWts((3u, "Gutsy Geoid")));
        Assert.Equal("Gutsy Geoid", strings.Resolve("TRIGSTR_3"));
    }

    [Fact]
    public void Tolerates_leading_zeros_in_reference()
    {
        var strings = MapStrings.From(WithWts((9u, "Nine")));
        Assert.Equal("Nine", strings.Resolve("TRIGSTR_0009"));
    }

    [Fact]
    public void Non_trigstr_input_returns_unchanged()
    {
        var strings = MapStrings.From(WithWts((3u, "Gutsy Geoid")));
        Assert.Equal("Literal Name", strings.Resolve("Literal Name"));
    }

    [Fact]
    public void Unknown_id_returns_raw_reference()
    {
        var strings = MapStrings.From(WithWts((3u, "Gutsy Geoid")));
        Assert.Equal("TRIGSTR_42", strings.Resolve("TRIGSTR_42"));
    }

    [Fact]
    public void Null_or_empty_resolves_to_empty_string()
    {
        var strings = new MapStrings(new Dictionary<int, string>());
        Assert.Equal("", strings.Resolve(null));
        Assert.Equal("", strings.Resolve(""));
    }

    [Fact]
    public void Map_without_wts_yields_empty_table()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = Encoding.UTF8.GetBytes(
                "function main takes nothing returns nothing\nendfunction\n"),
        }));
        Assert.Equal("TRIGSTR_3", MapStrings.From(doc).Resolve("TRIGSTR_3"));
    }

    [Fact]
    public void Null_valued_wts_entry_resolves_to_empty_not_throw()
    {
        // Real maps (e.g. Anime_WOS2) carry an empty STRING block, which War3Net
        // parses to a TriggerString with a NULL Value; From must store "" so
        // Resolve never dereferences null.
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.wts"] = Encoding.UTF8.GetBytes(
                "STRING 0\r\n{\r\n}\r\n\r\nSTRING 1\r\n{\r\nHello\r\n}\r\n"),
        }));
        var strings = MapStrings.From(doc);
        Assert.Equal("", strings.Resolve("TRIGSTR_0"));
        Assert.Equal("Hello", strings.Resolve("TRIGSTR_1"));
    }

    [Fact]
    public void Trailing_newlines_are_trimmed_from_resolved_value()
    {
        var strings = new MapStrings(new Dictionary<int, string> { [1] = "Name\r\n" });
        Assert.Equal("Name", strings.Resolve("TRIGSTR_1"));
    }
}
