// tests/Wc3.Tests/CameraCommandTests.cs
using System.Numerics;
using Wc3.Commands;
using Wc3.Model;
using War3Net.Build.Environment;
using Xunit;

namespace Wc3.Tests;

/// <summary>Hermetic coverage of <see cref="CameraCommand"/>: byte-faithful re-serialize,
/// list/add/set/remove, and the pure <see cref="CameraCommand.ApplyField"/> mapping.</summary>
public class CameraCommandTests
{
    private static Camera Cam(string name, float x = 0f, float y = 0f) => new()
    {
        Name = name,
        TargetPosition = new Vector2(x, y),
        ZOffset = 0f, Rotation = 90f, AngleOfAttack = 304f, TargetDistance = 1650f,
        Roll = 0f, FieldOfView = 70f, FarClippingPlane = 5000f, NearClippingPlane = 16f,
        LocalPitch = 0f, LocalYaw = 0f, LocalRoll = 0f,
    };

    private static byte[] CamerasBytes(params Camera[] cams)
    {
        var m = new MapCameras(MapCamerasFormatVersion.v0, useNewFormat: false);
        foreach (var c in cams) m.Cameras.Add(c);
        return CameraCommand.Serialize(m);
    }

    private static MapDocument Doc(params Camera[] cams) =>
        MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3c"] = CamerasBytes(cams),
        }));

    [Fact]
    public void List_returns_empty_when_catalog_absent()
    {
        // A map with no war3map.w3c just has no cameras; listing must not throw.
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>()));
        Assert.Empty(CameraCommand.List(doc));
    }

    [Fact]
    public void List_reads_cameras_in_order()
    {
        var doc = Doc(Cam("Alpha", 128f, 256f), Cam("Beta", 512f, 64f));
        var cams = CameraCommand.List(doc);

        Assert.Equal(2, cams.Count);
        Assert.Equal("Alpha", cams[0].Name);
        Assert.Equal(128f, cams[0].TargetX);
        Assert.Equal(256f, cams[0].TargetY);
        Assert.Equal("Beta", cams[1].Name);
        Assert.Equal(512f, cams[1].TargetX);
    }

    [Fact]
    public void Unchanged_reserialize_is_byte_identical()
    {
        var original = CamerasBytes(Cam("Alpha", 128f, 256f), Cam("Beta", 512f, 64f));
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3c"] = original,
        }));

        var model = (MapCameras)doc.GetFile("war3map.w3c")!.Model!;
        Assert.Equal(original, CameraCommand.Serialize(model));
    }

    [Fact]
    public void Add_appends_camera_and_persists()
    {
        var doc = Doc(Cam("Alpha"));
        var result = CameraCommand.Add(doc, "Beta", 300f, 400f);

        Assert.True(result.Ok, result.Message);
        Assert.Equal(2, result.Count);

        // Re-read from the mutated in-memory model.
        var beta = CameraCommand.List(doc).Single(c => c.Name == "Beta");
        Assert.Equal(300f, beta.TargetX);
        Assert.Equal(400f, beta.TargetY);

        // The staged override bytes round-trip: reload the re-serialized w3c and confirm Beta
        // survived. AddOrReplaceRawFile on an existing entry stages into OverrideBytes (RawBytes
        // keeps the original payload), so that is the persisted-on-Save payload to reload.
        var reloaded = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3c"] = doc.GetFile("war3map.w3c")!.OverrideBytes!,
        }));
        Assert.Contains(CameraCommand.List(reloaded), c => c.Name == "Beta" && c.TargetX == 300f);
    }

    [Fact]
    public void Add_rejects_blank_name()
    {
        var doc = Doc(Cam("Alpha"));
        Assert.False(CameraCommand.Add(doc, "  ", 0f, 0f).Ok);
        Assert.Single(CameraCommand.List(doc));
    }

    [Fact]
    public void Add_rejects_duplicate_name_case_insensitively()
    {
        var doc = Doc(Cam("Alpha"));
        Assert.False(CameraCommand.Add(doc, "alpha", 0f, 0f).Ok);
        Assert.Single(CameraCommand.List(doc));
    }

    [Fact]
    public void Set_updates_field_and_persists()
    {
        var doc = Doc(Cam("Alpha", 128f, 256f));
        var result = CameraCommand.Set(doc, "Alpha", "FieldOfView", "55");

        Assert.True(result.Ok, result.Message);
        Assert.Equal(55f, CameraCommand.List(doc).Single().FieldOfView);
    }

    [Fact]
    public void Set_can_move_target_component()
    {
        var doc = Doc(Cam("Alpha", 128f, 256f));
        Assert.True(CameraCommand.Set(doc, "Alpha", "TargetX", "999").Ok);

        var cam = CameraCommand.List(doc).Single();
        Assert.Equal(999f, cam.TargetX);
        Assert.Equal(256f, cam.TargetY); // untouched component preserved
    }

    [Fact]
    public void Set_can_rename()
    {
        var doc = Doc(Cam("Alpha"));
        Assert.True(CameraCommand.Set(doc, "Alpha", "Name", "Renamed").Ok);
        Assert.Equal("Renamed", CameraCommand.List(doc).Single().Name);
    }

    [Fact]
    public void Set_rejects_unknown_field()
    {
        var doc = Doc(Cam("Alpha"));
        Assert.False(CameraCommand.Set(doc, "Alpha", "Nope", "1").Ok);
    }

    [Fact]
    public void Set_rejects_unparseable_value()
    {
        var doc = Doc(Cam("Alpha"));
        Assert.False(CameraCommand.Set(doc, "Alpha", "Rotation", "abc").Ok);
        Assert.Equal(90f, CameraCommand.List(doc).Single().Rotation); // unchanged
    }

    [Fact]
    public void Set_rejects_missing_camera()
    {
        var doc = Doc(Cam("Alpha"));
        Assert.False(CameraCommand.Set(doc, "Ghost", "Roll", "1").Ok);
    }

    [Fact]
    public void Set_rejects_rename_onto_existing()
    {
        var doc = Doc(Cam("Alpha"), Cam("Beta"));
        Assert.False(CameraCommand.Set(doc, "Alpha", "Name", "beta").Ok);
        Assert.Equal("Alpha", CameraCommand.List(doc)[0].Name); // unchanged
    }

    [Fact]
    public void Remove_deletes_camera()
    {
        var doc = Doc(Cam("Alpha"), Cam("Beta"));
        var result = CameraCommand.Remove(doc, "alpha");

        Assert.True(result.Ok, result.Message);
        Assert.Equal(1, result.Count);
        Assert.Equal("Beta", CameraCommand.List(doc).Single().Name);
    }

    [Fact]
    public void Remove_rejects_missing_camera()
    {
        var doc = Doc(Cam("Alpha"));
        Assert.False(CameraCommand.Remove(doc, "Ghost").Ok);
        Assert.Single(CameraCommand.List(doc));
    }

    [Fact]
    public void List_returns_empty_when_w3c_missing_but_other_files_present()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = new byte[] { 1, 2, 3 },
        }));
        Assert.Empty(CameraCommand.List(doc));
    }

    // ---- pure ApplyField coverage (no MapDocument) ----

    [Theory]
    [InlineData("ZOffset", "12.5")]
    [InlineData("AngleOfAttack", "310")]
    [InlineData("LocalYaw", "-45")]
    public void ApplyField_sets_float_fields(string field, string value)
    {
        var cam = Cam("Alpha");
        Assert.True(CameraCommand.ApplyField(cam, field, value, out var err), err);
    }

    [Fact]
    public void ApplyField_rejects_blank_name()
    {
        var cam = Cam("Alpha");
        Assert.False(CameraCommand.ApplyField(cam, "Name", "   ", out _));
        Assert.Equal("Alpha", cam.Name);
    }

    [Fact]
    public void ApplyField_rejects_unknown_field()
    {
        var cam = Cam("Alpha");
        Assert.False(CameraCommand.ApplyField(cam, "Bogus", "1", out var err));
        Assert.Contains("Unknown camera field", err);
    }

    [Fact]
    public void ApplyField_rejects_bad_number()
    {
        var cam = Cam("Alpha");
        Assert.False(CameraCommand.ApplyField(cam, "Roll", "notnum", out var err));
        Assert.Contains("not a valid number", err);
    }
}
