// tests/Wc3.Tests/McpToolSmokeTests.Brief.cs
// Token-lean brief modes. A brief listing answers with the total and a first page rather than the
// whole list, so an agent learns how big a list is without paying for every entry.
using System.Text.Json;

namespace Wc3.Tests;

public sealed class McpSmokeBriefTests : McpSmokeBase
{
    public McpSmokeBriefTests(SmokeFixture f) : base(f) { }

    /// <summary>A map with the given number of named member files, enough to cross the page size.</summary>
    private string MapWithFiles(int count)
    {
        var files = new Dictionary<string, byte[]>();
        for (int i = 0; i < count; i++) files[$"file{i:D3}.bin"] = new byte[] { (byte)i };
        var map = Path.Combine(_f.Dir, $"brief-{Guid.NewGuid():N}.w3x");
        File.WriteAllBytes(map, SyntheticMap.Build(files));
        return map;
    }

    [Fact, Covers("list_files")]
    public async Task List_files_brief_returns_the_total_and_a_first_page()
    {
        var map = MapWithFiles(60);

        var full = Rows(await Call("list_files", Args(("map", map)))).Count;
        Assert.True(full > 50, $"expected more than 50 files, got {full}");

        var brief = await Call("list_files", Args(("map", map), ("brief", true)));
        Assert.Equal(full, Prop(brief, "total").GetInt32());
        Assert.True(Prop(brief, "truncated").GetBoolean());
        Assert.Equal(50, Rows(brief).Count);
    }

    [Fact, Covers("list_files")]
    public async Task List_files_brief_on_a_short_map_is_not_truncated()
    {
        var map = MapWithFiles(5);
        var full = Rows(await Call("list_files", Args(("map", map)))).Count;

        var brief = await Call("list_files", Args(("map", map), ("brief", true)));
        Assert.Equal(full, Prop(brief, "total").GetInt32());
        Assert.False(Prop(brief, "truncated").GetBoolean());
        Assert.Equal(full, Rows(brief).Count);
    }

    [Fact, Covers("object_list")]
    public async Task Object_list_brief_returns_the_total_and_a_first_page()
    {
        // 60 custom units in war3map.w3u.
        var w3u = new War3Net.Build.Object.UnitObjectData(War3Net.Build.Object.ObjectDataFormatVersion.v2);
        for (int i = 0; i < 60; i++)
            w3u.NewUnits.Add(new War3Net.Build.Object.SimpleObjectModification
            {
                OldId = War3Net.Common.Extensions.StringExtensions.FromRawcode("hfoo"),
                NewId = War3Net.Common.Extensions.StringExtensions.FromRawcode($"u{i:D3}"),
            });
        byte[] bytes;
        using (var ms = new MemoryStream())
        {
            using (var bw = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
                War3Net.Build.Extensions.BinaryWriterExtensions.Write(bw, w3u);
            bytes = ms.ToArray();
        }
        var map = Path.Combine(_f.Dir, $"brief-obj-{Guid.NewGuid():N}.w3x");
        File.WriteAllBytes(map, SyntheticMap.Build(new Dictionary<string, byte[]> { ["war3map.w3u"] = bytes }));

        var args = Args(("map", map), ("kind", "unit"), ("game_dir", "Z:\\no_such_install"));
        var fullCount = Rows(await Call("object_list", args)).Count;
        Assert.Equal(60, fullCount);

        args["brief"] = true;
        var brief = await Call("object_list", args);
        Assert.Equal(60, Prop(brief, "total").GetInt32());
        Assert.True(Prop(brief, "truncated").GetBoolean());
        Assert.Equal(50, Rows(brief).Count);
    }

    [Fact, Covers("placed_units_list")]
    public async Task Placed_units_list_brief_returns_the_total_and_a_first_page()
    {
        // 11 x 5 = 55 units on a copy of the fixture.
        var map = await Write("place_grid", Args(("type_rawcode", "hfoo"), ("rows", 11), ("cols", 5)));

        var full = Rows(await Call("placed_units_list", Args(("map", map)))).Count;
        Assert.Equal(55, full);

        var brief = await Call("placed_units_list", Args(("map", map), ("brief", true)));
        Assert.Equal(55, Prop(brief, "total").GetInt32());
        Assert.True(Prop(brief, "truncated").GetBoolean());
        Assert.Equal(50, Rows(brief).Count);
    }

    [Fact, Covers("placed_units_list")]
    public async Task Placed_units_list_default_is_still_the_full_list()
    {
        var map = await Write("place_grid", Args(("type_rawcode", "hfoo"), ("rows", 2), ("cols", 3)));
        var full = await Call("placed_units_list", Args(("map", map)));
        Assert.Equal(JsonValueKind.Array, full.ValueKind);
        Assert.Equal(6, full.GetArrayLength());
    }
}
