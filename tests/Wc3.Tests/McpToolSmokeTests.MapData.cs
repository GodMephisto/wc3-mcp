// tests/Wc3.Tests/McpToolSmokeTests.MapData.cs
// MapData family. Covers sound_list, sound_add, sound_set, sound_remove, map_info,
// map_info_get, map_info_set, file_list, file_get_text, file_set, list_files, new_map.
namespace Wc3.Tests;

public sealed partial class McpToolSmokeTests
{
    [Fact, Covers("sound_list")]
    public async Task Sound_list_on_a_blank_map_is_empty()
    {
        var sounds = Rows(await Call("sound_list", Args(("map", Fixture))));
        Assert.Empty(sounds);
    }

    [Fact, Covers("sound_add", "sound_list")]
    public async Task Sound_add_then_list_shows_new_sound()
    {
        var added = await Write("sound_add", Args(("name", "TestSound"), ("file", "Sound\\Ambient\\test.wav")));
        var sounds = Rows(await Call("sound_list", Args(("map", added))));
        var snd = Assert.Single(sounds);
        Assert.Equal("TestSound", Prop(snd, "name").GetString());
        Assert.Equal("Sound\\Ambient\\test.wav", Prop(snd, "filePath").GetString());
    }

    [Fact, Covers("sound_set", "sound_list")]
    public async Task Sound_set_field_then_list_shows_updated_value()
    {
        var added = await Write("sound_add", Args(("name", "VolSound"), ("file", "Sound\\Ambient\\vol.wav")));
        var set = await Write("sound_set", Args(("name", "VolSound"), ("field", "Volume"), ("value", "128")), map: added);
        var sounds = Rows(await Call("sound_list", Args(("map", set))));
        var snd = Assert.Single(sounds);
        Assert.Equal(128, Prop(snd, "volume").GetInt32());
    }

    [Fact, Covers("sound_remove", "sound_list")]
    public async Task Sound_remove_then_list_is_empty()
    {
        var added = await Write("sound_add", Args(("name", "ToRemove"), ("file", "Sound\\Ambient\\to.wav")));
        var removed = await Write("sound_remove", Args(("name", "ToRemove")), map: added);
        var sounds = Rows(await Call("sound_list", Args(("map", removed))));
        Assert.Empty(sounds);
    }

    [Fact, Covers("sound_set")]
    public async Task Sound_set_on_missing_sound_is_refused()
    {
        await CallError("sound_set", Args(("map", Fixture), ("out_path", NewOut("snd")),
            ("name", "Nope"), ("field", "Volume"), ("value", "64")));
    }

    [Fact]
    [Covers("map_info")]
    public async Task Map_info_on_blank_fixture_matches_expectations()
    {
        var info = await Call("map_info", Args(("map", Fixture)));
        Assert.Equal("Smoke Fixture", Prop(info, "name").GetString());
        Assert.Equal(32, Prop(info, "width").GetInt32());
        Assert.Equal(32, Prop(info, "height").GetInt32());
        Assert.Equal(BlankPlayers, Prop(info, "players").GetInt32());
    }

    [Fact]
    [Covers("map_info_get")]
    public async Task Map_info_get_on_blank_map()
    {
        var fields = await Call("map_info_get", Args(("map", Fixture)));
        Assert.Equal("Smoke Fixture", Prop(fields, "mapName").GetString());
        Assert.Equal(BlankPlayers, Prop(fields, "players").GetInt32());
        Assert.Equal(32, Prop(fields, "playableWidth").GetInt32());
        Assert.Equal(32, Prop(fields, "playableHeight").GetInt32());
    }

    [Fact, Covers("map_info_set", "map_info_get")]
    public async Task Map_info_set_then_get_shows_updated_field()
    {
        var set = await Write("map_info_set", Args(("field", "Author"), ("value", "Smoke Tester")));
        var fields = await Call("map_info_get", Args(("map", set)));
        Assert.Equal("Smoke Tester", Prop(fields, "author").GetString());
    }

    [Fact, Covers("map_info_set", "map_info_get")]
    public async Task Map_info_set_description_then_get_shows_it()
    {
        var set = await Write("map_info_set", Args(("field", "Description"), ("value", "Test description for smoke")));
        var fields = await Call("map_info_get", Args(("map", set)));
        Assert.Equal("Test description for smoke", Prop(fields, "description").GetString());
    }

    [Fact, Covers("file_get_text")]
    public async Task File_get_text_reads_the_blank_maps_script()
    {
        var result = await Call("file_get_text", Args(("map", Fixture), ("internal_path", "war3map.j")));
        Assert.Contains("function main takes nothing returns nothing", Prop(result, "text").GetString());
    }

    [Fact, Covers("file_get_text")]
    public async Task File_get_text_refuses_a_file_the_map_does_not_have()
    {
        var message = await CallError("file_get_text", Args(("map", Fixture), ("internal_path", "war3mapSkin.txt")));
        Assert.Contains("war3mapSkin.txt", message);
    }

    [Fact, Covers("file_list")]
    public async Task File_list_on_blank_map_has_entries()
    {
        var result = await Call("file_list", Args(("map", Fixture)));
        var files = Rows(result);
        Assert.NotEmpty(files);
    }

    [Fact, Covers("list_files")]
    public async Task List_files_on_blank_map_has_entries()
    {
        var result = await Call("list_files", Args(("map", Fixture)));
        var files = Rows(result);
        Assert.NotEmpty(files);
    }

    [Fact, Covers("file_set", "file_get_text")]
    public async Task File_set_adds_entry_then_get_text_shows_it()
    {
        var tempFile = NewOut("testdata", ".txt");
        await File.WriteAllTextAsync(tempFile, "Hello from smoke test");
        var set = await Write("file_set", Args(("internal_path", "smoke/test.txt"), ("from", tempFile)));
        var text = await Call("file_get_text", Args(("map", set), ("internal_path", "smoke/test.txt")));
        Assert.Equal("Hello from smoke test", Prop(text, "text").GetString());
    }

    [Fact]
    [Covers("new_map")]
    public async Task New_map_returns_path_and_maps()
    {
        var result = await Call("new_map", Args(("out_path", NewOut("newmap")), ("name", "Smoke Test"), ("tiles", 32)));
        var savedTo = Prop(result, "savedTo").GetString();
        Assert.NotNull(savedTo);
        Assert.True(File.Exists(savedTo));
        var info = await Call("map_info", Args(("map", savedTo)));
        Assert.Equal("Smoke Test", Prop(info, "name").GetString());
        Assert.Equal(32, Prop(info, "width").GetInt32());
    }
}
