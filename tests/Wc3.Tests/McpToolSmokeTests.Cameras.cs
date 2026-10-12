// tests/Wc3.Tests/McpToolSmokeTests.Cameras.cs
// Cameras. The worked example for the other families. Each write is read back with camera_list.
namespace Wc3.Tests;

public sealed class McpSmokeCamerasTests : McpSmokeBase
{
    public McpSmokeCamerasTests(SmokeFixture f) : base(f) { }

    [Fact, Covers("camera_list")]
    public async Task Camera_list_on_a_blank_map_is_empty()
    {
        var cameras = Rows(await Call("camera_list", Args(("map", Fixture))));
        Assert.Empty(cameras);
    }

    [Fact, Covers("camera_add", "camera_set", "camera_remove")]
    public async Task Camera_add_set_and_remove_each_show_in_camera_list()
    {
        var added = await Write("camera_add", Args(("name", "Cam1"), ("target_x", 128f), ("target_y", -256f)));
        var cam = Assert.Single(Rows(await Call("camera_list", Args(("map", added)))));
        Assert.Equal("Cam1", Prop(cam, "name").GetString());
        Assert.Equal(128f, Prop(cam, "targetX").GetSingle());
        Assert.Equal(-256f, Prop(cam, "targetY").GetSingle());

        var set = await Write("camera_set", Args(("name", "Cam1"), ("field", "FieldOfView"), ("value", "1.25")), map: added);
        cam = Assert.Single(Rows(await Call("camera_list", Args(("map", set)))));
        Assert.Equal(1.25f, Prop(cam, "fieldOfView").GetSingle(), 3);

        var removed = await Write("camera_remove", Args(("name", "Cam1")), map: set);
        Assert.Empty(Rows(await Call("camera_list", Args(("map", removed)))));
    }

    [Fact, Covers("camera_set")]
    public async Task Camera_set_on_a_missing_camera_is_refused()
    {
        await CallError("camera_set", Args(("map", Fixture), ("out_path", NewOut("cam")),
            ("name", "Nope"), ("field", "FieldOfView"), ("value", "1")));
    }

    [Fact, Covers("camera_tour")]
    public async Task Camera_tour_adds_interpolated_cameras()
    {
        var two = await Write("camera_add", Args(("name", "A"), ("target_x", 0f), ("target_y", 0f)));
        var three = await Write("camera_add", Args(("name", "B"), ("target_x", 1000f), ("target_y", 2000f)), two);

        var tour = await Write("camera_tour", Args(("from", "A"), ("to", "B"), ("count", 3)), three);

        var cams = Rows(await Call("camera_list", Args(("map", tour))));
        Assert.Equal(5, cams.Count);
        Assert.Contains(cams, c => Prop(c, "name").GetString() == "A-tour-1");
        Assert.Contains(cams, c => Prop(c, "name").GetString() == "A-tour-3");
        var mid = cams.Single(c => Prop(c, "name").GetString() == "A-tour-2");
        Assert.Equal(500f, Prop(mid, "targetX").GetSingle(), 3);
    }

    [Fact, Covers("camera_tour")]
    public async Task Camera_tour_on_a_missing_camera_is_refused()
    {
        await CallError("camera_tour", Args(("map", Fixture), ("out_path", NewOut("tour")),
            ("from", "Ghost"), ("to", "Nope"), ("count", 2)));
    }
}
