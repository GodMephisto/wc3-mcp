// tests/Wc3.Tests/ObjectBatchSetCommandTests.cs
// The batch is the answer to the chained-edit problem, so the tests pin the two properties
// that make it safe. Every edit lands in one save, and a failing edit writes nothing.
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public sealed class ObjectBatchSetCommandTests
{
    private static byte[] Sample()
    {
        var w3u = new UnitObjectData(ObjectDataFormatVersion.v2);
        var custom = new SimpleObjectModification { OldId = "Hpal".FromRawcode(), NewId = "H000".FromRawcode() };
        custom.Modifications.Add(new SimpleObjectDataModification
        { Id = "uagi".FromRawcode(), Type = ObjectDataType.Int, Value = 20 });
        custom.Modifications.Add(new SimpleObjectDataModification
        { Id = "unam".FromRawcode(), Type = ObjectDataType.String, Value = "Dark Paladin" });
        w3u.NewUnits.Add(custom);

        return SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3u"] = Serialize(w => w.Write(w3u)),
        });
    }

    private static byte[] Serialize(Action<BinaryWriter> write)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true)) write(bw);
        return ms.ToArray();
    }

    private static SimpleObjectDataModification ModOf(MapDocument doc, string rawcode, string field)
    {
        var w3u = (UnitObjectData)doc.GetFile("war3map.w3u")!.Model!;
        var unit = w3u.NewUnits.Single(u => u.NewId == rawcode.FromRawcode());
        return unit.Modifications.Single(m => m.Id == field.FromRawcode());
    }

    [Fact]
    public void Two_edits_on_one_object_land_in_one_save()
    {
        var doc = MapDocument.Load(Sample());
        var r = ObjectBatchSetCommand.Execute(doc,
        [
            new ObjectBatchEdit(ObjectKind.Unit, "H000", "uagi", "35"),
            new ObjectBatchEdit(ObjectKind.Unit, "H000", "unam", "Batch Paladin"),
        ]);

        Assert.True(r.Ok, r.Message);
        Assert.Equal(2, r.Applied);
        Assert.Empty(r.Failed);

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        Assert.Equal(35, ModOf(reloaded, "H000", "uagi").Value);
        Assert.Equal("Batch Paladin", ModOf(reloaded, "H000", "unam").Value);
    }

    [Fact]
    public void A_later_edit_to_the_same_field_wins()
    {
        var doc = MapDocument.Load(Sample());
        var r = ObjectBatchSetCommand.Execute(doc,
        [
            new ObjectBatchEdit(ObjectKind.Unit, "H000", "uagi", "30"),
            new ObjectBatchEdit(ObjectKind.Unit, "H000", "uagi", "44"),
        ]);

        Assert.True(r.Ok, r.Message);
        Assert.Equal(44, ModOf(MapDocument.Load(doc.SaveToBytes()), "H000", "uagi").Value);
    }

    [Fact]
    public void A_failing_edit_stops_the_batch_and_reports_its_position()
    {
        var doc = MapDocument.Load(Sample());
        var r = ObjectBatchSetCommand.Execute(doc,
        [
            new ObjectBatchEdit(ObjectKind.Unit, "H000", "uagi", "35"),
            new ObjectBatchEdit(ObjectKind.Unit, "H000", "uagi", "not a number"),
        ]);

        Assert.False(r.Ok);
        Assert.Equal(1, r.Applied);
        var failed = Assert.Single(r.Failed);
        Assert.Equal(1, failed.Index);
        Assert.Equal("uagi", failed.Field);
        Assert.Contains("nothing was written", r.Message);
    }

    [Fact]
    public void An_empty_batch_is_refused()
    {
        var r = ObjectBatchSetCommand.Execute(MapDocument.Load(Sample()), Array.Empty<ObjectBatchEdit>());

        Assert.False(r.Ok);
        Assert.Contains("no edits", r.Message);
    }

    [Fact]
    public void A_batch_over_the_limit_is_refused_before_any_edit_runs()
    {
        var edits = Enumerable.Range(0, ObjectBatchSetCommand.MaxEdits + 1)
            .Select(_ => new ObjectBatchEdit(ObjectKind.Unit, "H000", "uagi", "1"))
            .ToList();

        var r = ObjectBatchSetCommand.Execute(MapDocument.Load(Sample()), edits);

        Assert.False(r.Ok);
        Assert.Contains("at most", r.Message);
        Assert.Empty(r.Failed);
    }
}
