// tests/Wc3.Tests/LuaScriptCommandsTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Hermetic tests for the five Lua script commands. Each builds a synthetic map with a hand-written
/// war3map.lua and asserts the command reports the shape it names. No game install and no real map.
/// </summary>
public class LuaScriptCommandsTests
{
    private static MapDocument LuaDoc(string lua) =>
        MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.lua"] = Encoding.UTF8.GetBytes(lua),
        }));

    // ---- lua-leaks --------------------------------------------------------

    private static readonly string LeakScript = string.Join("\n",
        "OnTick = function()",                  // 1
        "  local g = CreateGroup()",            // 2
        "end",                                  // 3
        "TimerStart(CreateTimer(), 1.0, true, OnTick)", // 4
        "G = {}",                                // 5
        "function Init()",                      // 6
        "  G[1] = CreateGroup()",               // 7
        "end",                                  // 8
        "function Drop()",                      // 9
        "  CreateGroup()",                      // 10
        "end");                                 // 11

    [Fact]
    public void Lua_leaks_reports_a_dropped_creator_a_hot_creator_and_a_retained_table()
    {
        var r = LuaScriptLeaksCommand.Execute(LuaDoc(LeakScript));

        Assert.True(r.Ok, r.Message);
        var discarded = Assert.Single(r.Leaks.Where(l => l.Rule == "discarded"));
        Assert.Equal(("Drop", 10, "group"), (discarded.Function, discarded.Line, discarded.Handle));

        var hot = Assert.Single(r.Leaks.Where(l => l.Rule == "hot-creator"));
        Assert.Equal(("OnTick", "hot", 2), (hot.Function, hot.Heat, hot.Line));

        var table = Assert.Single(r.Leaks.Where(l => l.Rule == "table-retains"));
        Assert.Equal(("Init", "group", 7), (table.Function, table.Handle, table.Line));
        Assert.Contains("'G'", table.Why);

        Assert.Single(r.Periodic);
        Assert.Equal("OnTick", r.Periodic[0].Function);
        Assert.Equal(1.0, r.Periodic[0].Period);
    }

    [Fact]
    public void Lua_leaks_refuses_a_jass_script_with_a_message()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = Encoding.UTF8.GetBytes("function main takes nothing returns nothing\nendfunction\n"),
        }));
        var r = LuaScriptLeaksCommand.Execute(doc);
        Assert.False(r.Ok);
        Assert.Contains("JASS", r.Message);
    }

    // ---- lua-globals ------------------------------------------------------

    [Fact]
    public void Lua_globals_lists_reads_and_writes_and_flags_a_write_never_read()
    {
        var lua = string.Join("\n",
            "count = 0",              // 1
            "function bump()",        // 2
            "  count = count + 1",    // 3
            "end",                    // 4
            "function reset()",       // 5
            "  unused = 5",           // 6
            "end",                    // 7
            "function main()",        // 8
            "  bump()",               // 9
            "  reset()",              // 10
            "end");                   // 11

        var r = LuaGlobalsCommand.Execute(LuaDoc(lua));
        Assert.True(r.Ok, r.Message);

        var count = Assert.Single(r.Globals.Where(g => g.Name == "count"));
        Assert.Equal(2, count.Writes);       // line 1 and line 3
        Assert.Equal(1, count.Reads);        // line 3
        Assert.False(count.Framework);

        var unused = Assert.Single(r.Globals.Where(g => g.Name == "unused"));
        Assert.Equal(1, unused.Writes);
        Assert.Equal(0, unused.Reads);

        var main = Assert.Single(r.Globals.Where(g => g.Name == "main"));
        Assert.True(main.Framework);

        Assert.Equal(1, r.NeverRead);        // unused, main is an entry point
    }

    [Fact]
    public void Lua_globals_does_not_list_a_native_the_map_only_calls()
    {
        var r = LuaGlobalsCommand.Execute(LuaDoc("function main()\n  CreateUnit(nil, 0, 0, 0)\nend\n"));
        Assert.DoesNotContain(r.Globals, g => g.Name == "CreateUnit");
        Assert.Contains(r.Globals, g => g.Name == "main");
    }

    // ---- lua-check --------------------------------------------------------

    [Fact]
    public void Lua_check_reports_a_syntax_error_with_line_and_column()
    {
        var r = LuaCheckCommand.Execute(LuaDoc("function main()\n  local x = )\nend\n"));
        Assert.False(r.Ok);
        Assert.Equal(1, r.Errors);
        var file = Assert.Single(r.Checks);
        Assert.False(file.Ok);
        Assert.Equal(2, file.Line);
        Assert.Equal(13, file.Column);   // "  local x = )" puts ) at column 13
    }

    [Fact]
    public void Lua_check_parses_a_clean_file_and_a_second_broken_one()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.lua"] = Encoding.UTF8.GetBytes("function main()\nend\n"),
            [@"scripts\mod.lua"] = Encoding.UTF8.GetBytes("function f()\n  x = \nend\n"),
        }));
        var r = LuaCheckCommand.Execute(doc);
        Assert.Equal(2, r.Files);
        Assert.Equal(1, r.Errors);
        Assert.False(r.Ok);
        Assert.Single(r.Checks.Where(c => c.FileName == "war3map.lua" && c.Ok));
        Assert.Single(r.Checks.Where(c => c.FileName == @"scripts\mod.lua" && !c.Ok));
    }

    // ---- lua-api ----------------------------------------------------------

    [Fact]
    public void Lua_api_reports_a_native_the_target_lacks_and_skips_fourcc_and_own_functions()
    {
        var temp = Path.Combine(Path.GetTempPath(), "wc3ctl-lua-api-" + Guid.NewGuid().ToString("N"));
        try
        {
            ScriptTestSchema.Save(Path.Combine(temp, "old"), "TFT-v1.30.1.10211", new[] { "KnownNative" });
            ScriptTestSchema.Save(Path.Combine(temp, "new"), "TFT-v1.31.0.10420",
                new[] { "KnownNative", "NewNative" });

            var lua = string.Join("\n",
                "function Local()",          // 1
                "  KnownNative()",           // 2
                "  NewNative()",             // 3
                "  FourCC(\"hfoo\")",        // 4
                "  Local()",                 // 5
                "end");                      // 6

            var r = LuaApiCommand.Execute(LuaDoc(lua), "TFT-v1.30.1.10211", temp);
            Assert.Equal("TFT-v1.30.1.10211", r.TargetBuild);
            Assert.Equal(2, r.Used);                 // KnownNative and NewNative are both known natives
            var one = Assert.Single(r.Usages);       // only NewNative is missing from the target
            Assert.Equal(("NewNative", "native", 3), (one.Name, one.Kind, one.Line));
            Assert.Contains("identity", r.Mapping);
        }
        finally
        {
            if (Directory.Exists(temp)) Directory.Delete(temp, recursive: true);
        }
    }

    // ---- lua-to-jass ------------------------------------------------------

    [Fact]
    public void Lua_to_jass_converts_a_typed_function_and_reports_what_it_left()
    {
        var lua = string.Join("\n",
            "count = 0",                 // 1
            "function Tick()",           // 2
            "  local x = 1",             // 3
            "  x = x + 1",               // 4
            "  if x > 2 then",           // 5
            "    count = 0",             // 6
            "  end",                     // 7
            "  while x > 0 do",          // 8
            "    x = x - 1",             // 9
            "  end",                     // 10
            "end",                       // 11
            "function WithParam(a)",     // 12
            "end");                      // 13

        var doc = LuaDoc(lua);
        var r = LuaToJassCommand.Execute(doc);

        Assert.True(r.Ok, string.Join(" ", r.Diagnostics));
        Assert.Equal(1, r.FunctionsConverted);
        Assert.Equal(1, r.GlobalsConverted);
        Assert.Equal(1, r.FunctionsSkipped);
        Assert.Contains("function Tick takes nothing returns nothing", r.Jass);
        Assert.Contains("local integer x = 1", r.Jass);
        Assert.Contains("set count = 0", r.Jass);
        Assert.Contains("exitwhen not (x > 0)", r.Jass);
        Assert.Contains(r.Unconverted, u => u.Construct == "function" && u.Detail.Contains("WithParam"));

        // The whole converted map gained a war3map.j, which the game runs instead of the Lua.
        Assert.NotNull(doc.GetFile("war3map.j"));
        Assert.Contains("function Tick", ScriptCommand.Read(doc).Source);
    }

    [Fact]
    public void Lua_to_jass_inverts_the_iadd_helper_and_cat()
    {
        var lua = "function F()\n  local s = __cat(\"a\", \"b\")\n  local n = __iadd(1, 2)\nend\n";
        var r = LuaToJassCommand.Execute(LuaDoc(lua));
        Assert.True(r.Ok);
        Assert.Contains("local string s = (\"a\" + \"b\")", r.Jass);
        Assert.Contains("local integer n = (1 + 2)", r.Jass);
    }

    [Fact]
    public void Lua_to_jass_refuses_a_jass_script()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = Encoding.UTF8.GetBytes("function main takes nothing returns nothing\nendfunction\n"),
        }));
        var r = LuaToJassCommand.Execute(doc);
        Assert.False(r.Ok);
    }
}
