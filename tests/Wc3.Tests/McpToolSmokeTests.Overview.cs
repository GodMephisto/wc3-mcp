// tests/Wc3.Tests/McpToolSmokeTests.Overview.cs
// Overview family. map_overview composes the info, file-list and object-count reads into one
// call, so an agent asks for a map's shape once instead of calling three tools and seven kinds.
using System.Text.Json;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public sealed class McpSmokeOverviewTests : McpSmokeBase
{
    public McpSmokeOverviewTests(SmokeFixture f) : base(f) { }

    [Fact, Covers("map_overview")]
    public async Task Map_overview_returns_info_files_counts_and_diagnostics_in_one_call()
    {
        var r = await Call("map_overview", Args(("map", Fixture)));

        // Metadata, the same shape map_info returns.
        var info = Prop(r, "info");
        Assert.Equal(JsonValueKind.Object, info.ValueKind);
        Assert.True(Prop(info, "name").ValueKind == JsonValueKind.String);

        // Every archive file, the same shape list_files returns.
        var files = Prop(r, "files");
        Assert.Equal(JsonValueKind.Array, files.ValueKind);
        Assert.NotEmpty(files.EnumerateArray());

        // One count per Object Editor kind, all zero on a blank map.
        var counts = Prop(r, "objectCounts").EnumerateArray().ToList();
        Assert.Equal(ObjectKinds.All.Count, counts.Count);
        Assert.All(counts, c => Assert.Equal(0, Prop(c, "count").GetInt32()));

        // The diagnostics array is present (empty for a healthy map).
        Assert.Equal(JsonValueKind.Array, Prop(r, "diagnostics").ValueKind);
    }

    [Fact, Covers("map_overview")]
    public async Task Map_overview_counts_a_placed_custom_unit_under_its_kind()
    {
        // One custom unit in war3map.w3u must show up as a unit-kind count of 1.
        var w3u = new War3Net.Build.Object.UnitObjectData(War3Net.Build.Object.ObjectDataFormatVersion.v2);
        w3u.NewUnits.Add(new War3Net.Build.Object.SimpleObjectModification
        {
            OldId = War3Net.Common.Extensions.StringExtensions.FromRawcode("hfoo"),
            NewId = War3Net.Common.Extensions.StringExtensions.FromRawcode("u000"),
        });
        byte[] bytes;
        using (var ms = new MemoryStream())
        {
            using (var bw = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
                War3Net.Build.Extensions.BinaryWriterExtensions.Write(bw, w3u);
            bytes = ms.ToArray();
        }
        var map = Path.Combine(_f.Dir, $"overview-{Guid.NewGuid():N}.w3x");
        File.WriteAllBytes(map, SyntheticMap.Build(new Dictionary<string, byte[]> { ["war3map.w3u"] = bytes }));

        var r = await Call("map_overview", Args(("map", map)));
        var units = Prop(r, "objectCounts").EnumerateArray()
            .First(c => string.Equals(Prop(c, "kind").GetString(), "Unit", StringComparison.Ordinal));
        Assert.Equal(1, Prop(units, "count").GetInt32());
    }
}
