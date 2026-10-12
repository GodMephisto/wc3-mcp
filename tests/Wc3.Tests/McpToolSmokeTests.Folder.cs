// tests/Wc3.Tests/McpToolSmokeTests.Folder.cs
// The tools on a map saved in folder mode. Reading goes through map_info, writing through
// map_info_set, and the edited copy must come back as a folder holding the change.
using Wc3.Model;

namespace Wc3.Tests;

public sealed class McpSmokeFolderTests : McpSmokeBase
{
    public McpSmokeFolderTests(SmokeFixture f) : base(f) { }

    [Fact]
    public async Task A_folder_map_reads_and_edits_through_the_tools()
    {
        var folder = FolderFixture();
        var info = await Call("map_info", Args(("map", folder)));
        Assert.Equal("Smoke Fixture", Prop(info, "name").GetString());

        var outPath = NewOut("folder-edit");
        await Call("map_info_set", Args(("map", folder), ("out_path", outPath), ("field", "Author"), ("value", "Folder Tester")));
        Assert.True(Directory.Exists(outPath), "a folder map's edited copy should be a folder");
        var fields = await Call("map_info_get", Args(("map", outPath)));
        Assert.Equal("Folder Tester", Prop(fields, "author").GetString());
    }

    /// <summary>The fixture as a folder map whose script is Lua rather than JASS.</summary>
    private string LuaFolderFixture(string lua)
    {
        var folder = FolderFixture();
        File.Delete(Path.Combine(folder, "war3map.j"));
        File.WriteAllText(Path.Combine(folder, "war3map.lua"), lua);
        return folder;
    }

    [Fact, Covers("script_check")]
    public async Task Script_check_passes_jass_and_reports_a_lua_error_with_its_line()
    {
        var jass = await Call("script_check", Args(("map", Fixture)));
        Assert.Equal("jass", Prop(jass, "language").GetString());
        Assert.True(Prop(jass, "ok").GetBoolean());

        var bad = await Call("script_check", Args(("map", LuaFolderFixture("function main()\n  x = \nend\n"))));
        Assert.Equal("lua", Prop(bad, "language").GetString());
        Assert.False(Prop(bad, "ok").GetBoolean());
        Assert.Equal(3, Prop(Prop(bad, "issues")[0], "line").GetInt32());
    }

    [Fact, Covers("script_format")]
    public async Task Script_format_reprints_lua_and_refuses_jass()
    {
        var map = LuaFolderFixture("function main()  local x=1  if x then print( x ) end end\n");
        var outPath = NewOut("script-format");
        var r = await Call("script_format", Args(("map", map), ("out_path", outPath), ("style", "pretty")));
        Assert.True(Prop(r, "changed").GetBoolean());
        var check = await Call("script_check", Args(("map", outPath)));
        Assert.True(Prop(check, "ok").GetBoolean());
        Assert.Contains("\n", File.ReadAllText(Path.Combine(outPath, "war3map.lua")).Trim());

        Assert.Contains("JASS", await CallError("script_format", Args(("map", Fixture), ("out_path", NewOut("jass-format")))));
    }

    [Fact]
    public async Task A_folder_maps_copy_cannot_go_inside_it()
    {
        var folder = FolderFixture();
        var message = await CallError("map_info_set",
            Args(("map", folder), ("out_path", Path.Combine(folder, "inner.w3x")), ("field", "Author"), ("value", "x")));
        Assert.Contains("inside the input map folder", message);
    }
}
