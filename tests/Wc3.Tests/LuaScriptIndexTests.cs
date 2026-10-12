// tests/Wc3.Tests/LuaScriptIndexTests.cs
using Wc3.Commands.Lua;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

public class LuaScriptIndexTests
{
    private const string Script =
        "function InitGlobals()\n" +                 // 1
        "  udg_x = 0\n" +                            // 2
        "end\n" +                                    // 3
        "local function helper(a, ...)\n" +          // 4
        "  return a\n" +                             // 5
        "end\n" +                                    // 6
        "Trig_A_Actions = function()\n" +            // 7
        "  helper(1)\n" +                            // 8
        "end\n" +                                    // 9
        "function M.sub:go(b)\n" +                   // 10
        "  local inner = function() helper(2) end\n" + // 11
        "end\n" +                                    // 12
        "function InitTrig_A()\n" +                  // 13
        "  TriggerAddAction(t, Trig_A_Actions)\n" +  // 14
        "end\n";                                     // 15

    [Fact]
    public void Functions_finds_every_declaration_form_with_spans()
    {
        var fns = LuaScriptIndex.Functions(Script);
        Assert.Equal(new[] { "InitGlobals", "helper", "Trig_A_Actions", "M.sub:go", "inner", "InitTrig_A" },
            fns.Select(f => f.Name));
        var helper = fns.Single(f => f.Name == "helper");
        Assert.Equal((4, 6), (helper.StartLine, helper.EndLine));
        Assert.Equal("local function helper(a, ...)", helper.Signature);
        Assert.Equal("Trig_A_Actions = function()", fns.Single(f => f.Name == "Trig_A_Actions").Signature);
        Assert.Equal((10, 12), (fns.Single(f => f.Name == "M.sub:go").StartLine, fns.Single(f => f.Name == "M.sub:go").EndLine));
    }

    [Fact]
    public void References_split_calls_from_handlers_passed_as_values()
    {
        var refs = LuaScriptIndex.References(Script, "Trig_A_Actions");
        Assert.Equal(new[] { (7, JassReferenceKind.Declaration), (14, JassReferenceKind.CodeReference) },
            refs.Select(r => (r.Line, r.Kind)));
        Assert.Equal("InitTrig_A", refs[1].InFunction);

        var calls = LuaScriptIndex.References(Script, "helper");
        Assert.Equal(new (int, string?)[] { (8, "Trig_A_Actions"), (11, "inner") },
            calls.Where(r => r.Kind == JassReferenceKind.Call).Select(r => (r.Line, r.InFunction)));
    }

    [Fact]
    public void A_broken_script_throws_with_luac_line()
    {
        var e = Assert.Throws<LuaSyntaxError>(() => LuaScriptIndex.Functions("function f()\n  x = \nend\n"));
        Assert.Equal(3, e.Line);
    }
}
