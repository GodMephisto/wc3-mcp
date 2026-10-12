// tests/Wc3.Tests/JassToLuaTests.cs
using Wc3.Commands;
using Wc3.Commands.Lua;
using Wc3.Model;
using War3Net.Build.Info;

namespace Wc3.Tests;

/// <summary>
/// The JASS to Lua converter, ported from jass2lua.py in Devo's Map Doctor. Each test pins one
/// rule where JASS and Lua disagree, by the Lua it must emit.
/// </summary>
public class JassToLuaTests
{
    private const string Natives = """
        native GetRandomInt takes integer lowBound, integer highBound returns integer
        native I2S takes integer i returns string
        native CreateTimer takes nothing returns timer
        native TimerStart takes timer whichTimer, real timeout, boolean periodic, code handlerFunc returns nothing
        native SetPlayerState takes integer whichPlayer, integer amount, real r returns nothing
        """;

    private static JassToLuaResult Convert(string body) => JassToLua.Transpile(Natives, "", body);

    private static string Fn(string decls, string stmts) =>
        $"globals\n{decls}\nendglobals\nfunction F takes nothing returns nothing\n{stmts}\nendfunction\n";

    [Fact]
    public void Integer_division_truncates_toward_zero_and_real_division_guards_zero()
    {
        var r = Convert(Fn("integer a = 7\ninteger b = 2\nreal x = 1.0\nreal y = 2.0",
            "set a = a / b\nset x = x / y\nset x = x / 2.0\nset x = x / 0.0"));
        Assert.Contains("  a = __idiv(a, b)", r.LuaFunctions);
        Assert.Contains("  x = __div(x, y)", r.LuaFunctions);
        Assert.Contains("  x = (x / 2.0)", r.LuaFunctions);
        Assert.Contains("  x = __div(x, 0.0)", r.LuaFunctions);
    }

    [Fact]
    public void Integer_arithmetic_wraps_at_32_bits()
    {
        var r = Convert(Fn("integer a = 0\ninteger b = 0",
            "set a = a + b\nset a = a - b\nset a = a * b\nset a = a + 1\nset a = 4294967295\nset b = 0x80000000\nset b = 'A000'"));
        Assert.Contains("  a = __iadd(a, b)", r.LuaFunctions);
        Assert.Contains("  a = __isub(a, b)", r.LuaFunctions);
        Assert.Contains("  a = __imul(a, b)", r.LuaFunctions);
        // A small literal cannot overflow alone, so it keeps the plain operator.
        Assert.Contains("  a = (a + 1)", r.LuaFunctions);
        Assert.Contains("  a = (-1)", r.LuaFunctions);
        Assert.Contains("  b = (-2147483648)", r.LuaFunctions);
        Assert.Contains("  b = 1093677104", r.LuaFunctions);
    }

    [Fact]
    public void String_concatenation_goes_through_the_null_safe_cat()
    {
        var r = Convert(Fn("string s = null", "set s = \"a\" + s + I2S(1)\nset s = null"));
        Assert.Contains("  s = __cat(\"a\", s, I2S(1))", r.LuaFunctions);
        Assert.Contains("  s = nil", r.LuaFunctions);
        Assert.Contains("s = nil", r.LuaGlobals);
        Assert.Contains("local n = select(\"#\", ...)", JassToLua.Prelude);
        Assert.Contains("if v == nil then t[i] = \"\"", JassToLua.Prelude);
    }

    [Fact]
    public void Arrays_read_their_type_default()
    {
        var r = Convert(Fn("integer array ia\nreal array ra\nboolean array ba\ntimer array ta\nstring array sa",
            "local integer array li\nlocal timer array lt"));
        Assert.Contains("ia = __arr_i()", r.LuaGlobals);
        Assert.Contains("ra = __arr_r()", r.LuaGlobals);
        Assert.Contains("ba = __arr_b()", r.LuaGlobals);
        Assert.Contains("ta = {}", r.LuaGlobals);
        Assert.Contains("sa = {}", r.LuaGlobals);
        Assert.Contains("  local li = __arr_i()", r.LuaFunctions);
        Assert.Contains("  local lt = {}", r.LuaFunctions);
    }

    [Fact]
    public void Locals_past_180_spill_into_a_table()
    {
        var decls = string.Join("\n", Enumerable.Range(0, 185).Select(i => $"local integer l{i} = {i}"));
        var r = JassToLua.Transpile("", "",
            $"function F takes integer p returns integer\n{decls}\nreturn l184 + l0\nendfunction\n");
        Assert.Contains("  local __L = {}", r.LuaFunctions);
        // One parameter, so 179 locals stay real and the 180th onward spill.
        Assert.Contains("  local l178 = 178", r.LuaFunctions);
        Assert.Contains("  __L.l179 = 179", r.LuaFunctions);
        Assert.Contains("  return __iadd(__L.l184, l0)", r.LuaFunctions);
        Assert.Null(LuaParser.Check(JassToLua.BuildScript(r)));
    }

    [Fact]
    public void Code_values_nulls_and_lua_keywords_follow_the_signature()
    {
        var r = Convert("""
            function end takes nothing returns nothing
            endfunction
            function F takes nothing returns boolean
            local timer t = CreateTimer()
            call TimerStart(t, 1.0, false, function end)
            call SetPlayerState(null, null, null)
            call Missing(null)
            if t == null then
            return false
            set t = null
            endif
            return null
            endfunction
            """);
        Assert.Contains("function end_()", r.LuaFunctions);
        Assert.Contains("  TimerStart(t, 1.0, false, end_)", r.LuaFunctions);
        Assert.Contains("  SetPlayerState(0, 0, 0.0)", r.LuaFunctions);
        Assert.Contains("  Missing(nil)", r.LuaFunctions);
        Assert.Contains("  if (t == nil) then", r.LuaFunctions);
        Assert.Contains("    do return false end", r.LuaFunctions);
        Assert.Contains("  return false", r.LuaFunctions);
        Assert.Equal(["end", "F"], r.FuncNames);
    }

    [Fact]
    public void Warnings_name_unknown_functions_and_unknown_variables()
    {
        var r = Convert(Fn("integer a = 0", "set a = Missing() + b"));
        Assert.Contains("unknown function Missing (line 5)", r.Warnings);
        Assert.Contains("unknown type for b (line 5)", r.Warnings);
    }

    [Fact]
    public void Execute_script_literals_and_lua_load_chunks_are_lifted_out()
    {
        var r = JassToLua.Transpile("", "", """
            function F takes nothing returns nothing
            local string s = "print(1)"
            call EXExecuteScript("lua_load([=[" + s + "]=], true)")
            call EXExecuteScript("1 + 2")
            endfunction
            """);
        Assert.Equal(["print(1)"], r.LuaChunks);
        Assert.Equal(["1 + 2"], r.ExsLiterals);
        Assert.Contains("  local s", r.LuaFunctions);
        Assert.Contains("  __luachunk(0)", r.LuaFunctions);
        Assert.Contains("  __exs(0)", r.LuaFunctions);
        Assert.Null(LuaParser.Check(JassToLua.BuildScript(r)));
    }

    [Fact]
    public void A_control_char_before_a_digit_keeps_a_three_digit_escape()
    {
        Assert.Equal("\"\\1x\"", JassToLua.LuaString("\u0001x"));
        Assert.Equal("\"\\0012\"", JassToLua.LuaString("\u00012"));
    }

    [Fact]
    public void Malformed_jass_is_refused_with_its_line()
    {
        var e = Assert.Throws<JassToLuaError>(() => JassToLua.Transpile("", "", "function F takes nothing returns nothing\nset = 1\nendfunction\n"));
        Assert.Contains("line 2", e.Message);
    }

    [Fact]
    public void The_command_writes_lua_sets_the_w3i_flag_and_stubs_the_jass()
    {
        var doc = MapDocument.Load(BlankMap.CreateArchiveBytes());
        var (file, _) = ScriptCommand.Read(doc);
        Assert.Equal("war3map.j", file);

        var r = JassToLuaCommand.Convert(doc, Natives, "");
        Assert.True(r.Functions > 0);

        var saved = MapDocument.Load(doc.SaveToBytes());
        var (after, lua) = ScriptCommand.Read(saved);
        Assert.Equal("war3map.lua", after);
        Assert.Null(LuaParser.Check(lua));
        Assert.Contains("function main()", lua);
        Assert.Equal(ScriptLanguage.Lua, (saved.GetFile("war3map.w3i")!.Model as MapInfo)!.ScriptLanguage);
        Assert.True(saved.TryReadFileByName("war3map.j", out var stub));
        Assert.Equal(JassToLuaCommand.StubText, ScriptCommand.ScriptEncoding.GetString(stub));

        Assert.Throws<InvalidOperationException>(() => JassToLuaCommand.Convert(saved, "", ""));
    }

    [Fact]
    public void A_classic_w3i_is_raised_to_v28_to_carry_the_lua_flag()
    {
        var doc = MapDocument.Load(BlankMap.CreateArchiveBytes());
        var info = (MapInfo)doc.GetFile("war3map.w3i")!.Model!;
        info.FormatVersion = MapInfoFormatVersion.v25;
        doc.AddOrReplaceRawFile("war3map.w3i", MapInfoCommand.Serialize(info)).Model = info;

        var r = JassToLuaCommand.Convert(doc, Natives, "");
        Assert.Contains(r.Warnings, w => w.Contains("raised to v28"));

        var saved = (MapInfo)MapDocument.Load(doc.SaveToBytes()).GetFile("war3map.w3i")!.Model!;
        Assert.Equal(MapInfoFormatVersion.v28, saved.FormatVersion);
        Assert.Equal(ScriptLanguage.Lua, saved.ScriptLanguage);
    }
}
