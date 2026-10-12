// tests/Wc3.Tests/AuditTrendTests.cs
// Hermetic tests for AuditTrendCommand. Uses the level-gap check, which fires on a map whose
// ability declares more levels than it sets a value for and whose base is undefined, so no game
// install is needed.
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public sealed class AuditTrendTests : IDisposable
{
    private readonly string _dir = Path.Combine(
        Path.GetTempPath(), "wc3ctl-audit-trend", Guid.NewGuid().ToString("N"));

    public AuditTrendTests() => Directory.CreateDirectory(_dir);

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch (IOException) { }
    }

    /// <summary>A map holding one ability per requested rawcode, each with a real level gap.</summary>
    private string MapWithGaps(params string[] rawcodes)
    {
        var w3a = new AbilityObjectData(ObjectDataFormatVersion.v2);
        foreach (var code in rawcodes)
        {
            var a = new LevelObjectModification
            { OldId = "ZZZZ".FromRawcode(), NewId = code.FromRawcode() };
            a.Modifications.Add(new LevelObjectDataModification
            { Level = 0, Pointer = 0, Id = "alev".FromRawcode(), Type = ObjectDataType.Int, Value = 3 });
            a.Modifications.Add(new LevelObjectDataModification
            { Level = 1, Pointer = 0, Id = "acdn".FromRawcode(), Type = ObjectDataType.Unreal, Value = 5.0f });
            w3a.NewAbilities.Add(a);
        }

        var files = new Dictionary<string, byte[]>
        {
            ["war3map.w3a"] = Serialize(w => w.Write(w3a)),
            ["war3map.j"] = Encoding.UTF8.GetBytes(
                "function main takes nothing returns nothing\nendfunction\n"),
        };
        var path = Path.Combine(_dir, "map-" + Guid.NewGuid().ToString("N") + ".w3x");
        File.WriteAllBytes(path, SyntheticMap.Build(files));
        return path;
    }

    private static byte[] Serialize(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }

    [Fact]
    public void Trend_reports_a_finding_the_second_map_added()
    {
        var before = MapWithGaps("A000");
        var after = MapWithGaps("A000", "A001");

        var r = AuditTrendCommand.Execute(before, after);

        Assert.Equal(1, r.Added);
        Assert.Equal(0, r.Removed);
        var row = Assert.Single(r.Rows);
        Assert.Equal("level-gap", row.Check);
        Assert.Contains(row.AddedDetail, d => d.Contains("A001"));
    }

    [Fact]
    public void Trend_reports_a_finding_the_second_map_removed()
    {
        var before = MapWithGaps("A000", "A001");
        var after = MapWithGaps("A000");

        var r = AuditTrendCommand.Execute(before, after);

        Assert.Equal(0, r.Added);
        Assert.Equal(1, r.Removed);
        var row = Assert.Single(r.Rows);
        Assert.Equal("level-gap", row.Check);
        Assert.Contains(row.RemovedDetail, d => d.Contains("A001"));
    }

    [Fact]
    public void Trend_of_a_map_against_itself_is_empty()
    {
        var map = MapWithGaps("A000");

        var r = AuditTrendCommand.Execute(map, map);

        Assert.Equal(0, r.Added);
        Assert.Equal(0, r.Removed);
        Assert.Empty(r.Rows);
    }
}
