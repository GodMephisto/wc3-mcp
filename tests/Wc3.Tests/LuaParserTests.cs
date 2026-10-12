// tests/Wc3.Tests/LuaParserTests.cs
using System.Text.Json;
using Wc3.Commands.Lua;

namespace Wc3.Tests;

/// <summary>
/// The Lua 5.3 parser against luac 5.3.6 itself. Fixtures/Lua/doctor-cases.luac.json holds the 87
/// good and bad cases of lua_ast.py in Devo's Map Doctor (MIT), each with what luac -p from
/// lua.org's 5.3.6 said about it on 2026-10-12, so the expected result is Lua's, not the
/// doctor's. The one place they disagree is the Lua 5.4 local attributes, which the doctor
/// accepts and Lua 5.3 (the version Warcraft III runs) refuses.
/// </summary>
public class LuaParserTests
{
    public sealed record Expected(bool ok, int line, string? message);
    public sealed record Case(string name, string source, Expected luac);

    private static readonly Lazy<List<Case>> Cases = new(() =>
        JsonSerializer.Deserialize<List<Case>>(File.ReadAllText(
            Path.Combine(AppContext.BaseDirectory, "Fixtures", "Lua", "doctor-cases.luac.json")))!);

    public static IEnumerable<object[]> Names() => Cases.Value.Select(c => new object[] { c.name });

    [Theory]
    [MemberData(nameof(Names))]
    public void Agrees_with_luac(string name)
    {
        var c = Cases.Value.Single(x => x.name == name);
        var error = LuaParser.Check(c.source);
        if (c.luac.ok)
        {
            Assert.Null(error);
            return;
        }
        Assert.NotNull(error);
        Assert.Equal((c.luac.line, c.luac.message), (error!.Line, error.Detail));
    }

    [Fact]
    public void The_fixture_has_both_kinds()
    {
        Assert.Equal(87, Cases.Value.Count);
        Assert.Equal(34, Cases.Value.Count(c => c.luac.ok));
    }

    [Theory]
    [MemberData(nameof(Names))]
    public void Canonical_text_parses_back_to_the_same_canonical_text(string name)
    {
        var c = Cases.Value.Single(x => x.name == name);
        if (!c.luac.ok) return;
        var once = LuaPrinter.Canonical(c.source);
        Assert.Null(LuaParser.Check(once));
        Assert.Equal(once, LuaPrinter.Canonical(once));
    }

    [Theory]
    [MemberData(nameof(Names))]
    public void Pretty_text_keeps_the_canonical_form(string name)
    {
        var c = Cases.Value.Single(x => x.name == name);
        if (!c.luac.ok) return;
        var chunk = LuaParser.Parse(c.source);
        var pretty = LuaPrinter.Pretty(chunk);
        Assert.Equal(LuaPrinter.Canonical(chunk), LuaPrinter.Canonical(pretty));
    }

    [Fact]
    public void Comments_survive_pretty_printing()
    {
        var chunk = LuaParser.Parse("-- head\nlocal a = 1 -- tail\nfunction f()\n  -- inside\n  x = 1\nend\n-- eof");
        var pretty = LuaPrinter.Pretty(chunk);
        foreach (var c in new[] { "-- head", "-- tail", "-- inside", "-- eof" })
            Assert.Contains(c, pretty);
    }

    [Fact]
    public void Top_level_functions_are_indexed_by_full_name()
    {
        var chunk = LuaParser.Parse("function main() end\nfunction a.b:c() end\nlocal function h() end\nx = function() end");
        Assert.Equal(new[] { "a.b:c", "h", "main" }, chunk.Functions.Keys.OrderBy(k => k, StringComparer.Ordinal));
        Assert.True(chunk.Functions["h"].IsLocal);
    }
}
