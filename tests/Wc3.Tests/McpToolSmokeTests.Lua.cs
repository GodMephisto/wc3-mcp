// tests/Wc3.Tests/McpToolSmokeTests.Lua.cs
// JASS to Lua conversion through the MCP tool. The converted copy must carry a script that
// parses as Lua 5.3, and the input map must come back unchanged.
namespace Wc3.Tests;

public sealed class McpSmokeLuaTests : McpSmokeBase
{
    public McpSmokeLuaTests(SmokeFixture f) : base(f) { }

    /// <summary>A folder map whose script is the given Lua, so the Lua commands have a Lua map to
    /// read. The blank fixture's war3map.j is removed so the entry resolves to war3map.lua.</summary>
    private string LuaFolder(string lua)
    {
        var folder = FolderFixture();
        var jass = Path.Combine(folder, "war3map.j");
        if (File.Exists(jass)) File.Delete(jass);
        File.WriteAllText(Path.Combine(folder, "war3map.lua"), lua);
        return folder;
    }

    [Fact, Covers("script_lua_leaks")]
    public async Task Script_lua_leaks_reports_a_handle_created_on_a_period()
    {
        var folder = LuaFolder("OnTick = function()\n  local g = CreateGroup()\nend\n"
                               + "TimerStart(CreateTimer(), 1.0, true, OnTick)\n");
        var r = await Call("script_lua_leaks", Args(("map", folder)));
        Assert.True(Prop(r, "ok").GetBoolean());
        Assert.True(Prop(r, "leaks").GetArrayLength() >= 1);
        Assert.True(Prop(r, "periodic").GetArrayLength() >= 1);
    }

    [Fact, Covers("script_lua_globals")]
    public async Task Script_lua_globals_lists_a_defined_global()
    {
        var folder = LuaFolder("count = 0\nfunction main()\n  count = count + 1\nend\n");
        var r = await Call("script_lua_globals", Args(("map", folder)));
        Assert.True(Prop(r, "ok").GetBoolean());
        Assert.True(Prop(r, "globals").GetArrayLength() >= 1);
    }

    [Fact, Covers("script_lua_check")]
    public async Task Script_lua_check_passes_a_clean_file_and_fails_a_broken_one()
    {
        var good = await Call("script_lua_check", Args(("map", LuaFolder("function main()\nend\n"))));
        Assert.True(Prop(good, "ok").GetBoolean());

        var bad = await Call("script_lua_check", Args(("map", LuaFolder("function main()\n  local x = )\nend\n"))));
        Assert.False(Prop(bad, "ok").GetBoolean());
        Assert.Equal(1, Prop(bad, "errors").GetInt32());
    }

    [Fact, Covers("script_lua_api")]
    public async Task Script_lua_api_reports_a_missing_native()
    {
        var temp = Path.Combine(_f.Dir, "lua-api-schemas" + Guid.NewGuid().ToString("N"));
        ScriptTestSchema.Save(Path.Combine(temp, "old"), "TFT-v1.30.1.10211", new[] { "KnownNative" });
        ScriptTestSchema.Save(Path.Combine(temp, "new"), "TFT-v1.31.0.10420", new[] { "KnownNative", "NewNative" });

        var folder = LuaFolder("function Init()\n  KnownNative()\n  NewNative()\nend\n");
        var r = await Call("script_lua_api", Args(("map", folder), ("target", "TFT-v1.30.1.10211"), ("schemas_dir", temp)));
        Assert.Equal(1, Prop(r, "missing").GetInt32());
    }

    [Fact, Covers("script_lua_to_jass")]
    public async Task Script_lua_to_jass_writes_a_converted_copy_and_leaves_the_input()
    {
        // A file map whose script is Lua, produced by script_to_lua, so the write-on-a-copy helper
        // (which hashes the input bytes) has a file to read.
        var luaMap = NewOut("lua-src");
        await Call("script_to_lua", Args(("map", Fixture), ("out_path", luaMap)));
        var outPath = await Write("script_lua_to_jass", Args(), map: luaMap);
        var check = await Call("script_check", Args(("map", outPath)));
        Assert.Equal("jass", Prop(check, "language").GetString());
    }

    [Fact, Covers("script_to_lua")]
    public async Task Script_to_lua_writes_a_lua_map_and_refuses_one_already_lua()
    {
        var before = File.ReadAllBytes(Fixture);
        var outPath = NewOut("to-lua");
        var r = await Call("script_to_lua", Args(("map", Fixture), ("out_path", outPath)));
        Assert.True(Prop(r, "functions").GetInt32() > 0);
        Assert.Equal(before, File.ReadAllBytes(Fixture));

        var check = await Call("script_check", Args(("map", outPath)));
        Assert.Equal("lua", Prop(check, "language").GetString());
        Assert.True(Prop(check, "ok").GetBoolean());

        Assert.Contains("already Lua", await CallError("script_to_lua", Args(("map", outPath), ("out_path", NewOut("to-lua-again")))));
    }

    [Fact, Covers("script_decompile")]
    public async Task Script_decompile_turns_bytecode_back_into_source()
    {
        var none = await Call("script_decompile", Args(("map", Fixture), ("out_path", NewOut("decompile-none"))));
        Assert.Equal(0, Prop(none, "files").GetArrayLength());

        var folder = FolderFixture();
        File.Delete(Path.Combine(folder, "war3map.j"));
        File.Copy(Path.Combine(AppContext.BaseDirectory, "Fixtures", "Lua", "bytecode-ops.luac"), Path.Combine(folder, "war3map.lua"));
        var outPath = NewOut("decompile");
        var r = await Call("script_decompile", Args(("map", folder), ("out_path", outPath)));
        Assert.Equal("war3map.lua", Prop(Prop(r, "files")[0], "fileName").GetString());

        var check = await Call("script_check", Args(("map", outPath)));
        Assert.True(Prop(check, "ok").GetBoolean());
    }
}
