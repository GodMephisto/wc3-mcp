// tests/Wc3.Tests/JassSyntaxTests.cs
using System.Diagnostics;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// The shared JASS vocabulary. It lives in this layer, beside JassFunctionIndex and
/// JassScriptCheck, because nothing about "which words are JASS keywords" belongs to a front-end.
/// The Studio builds its syntax highlighting from it, and a terminal could colour the same tokens
/// from the same list without owning a second copy.
/// </summary>
public class JassSyntaxTests
{
    private readonly ITestOutputHelper _out;
    public JassSyntaxTests(ITestOutputHelper output) => _out = output;

    private const string Sample = """
        globals
            integer udg_Count = 0
        endglobals

        function Helper takes nothing returns nothing
            call BJDebugMsg("hi")
        endfunction

        function Trig_Actions takes nothing returns nothing
            local unit u = CreateUnit(Player(0), 'hfoo', 0., 0., 270.)
            call Helper()
            call UnitAddAbility(u, 'A00R')
        endfunction
        """;

    [Fact]
    public void Keywords_and_types_are_reserved_and_a_function_name_is_not()
    {
        Assert.True(JassSyntax.IsReserved("function"));
        Assert.True(JassSyntax.IsReserved("endfunction"));
        Assert.True(JassSyntax.IsReserved("integer"));
        Assert.True(JassSyntax.IsReserved("unit"));
        Assert.False(JassSyntax.IsReserved("CreateUnit"));
        Assert.False(JassSyntax.IsReserved("Trig_Actions"));
        // Case matters in JASS, so a capitalised keyword is not one.
        Assert.False(JassSyntax.IsReserved("Function"));
    }

    [Fact]
    public void External_calls_are_the_ones_the_script_does_not_declare()
    {
        var ext = JassSyntax.ExternalCalls(Sample);

        // Called and not declared here, so external.
        Assert.Contains("BJDebugMsg", ext);
        Assert.Contains("CreateUnit", ext);
        Assert.Contains("Player", ext);
        Assert.Contains("UnitAddAbility", ext);

        // Declared in this very script, so its own, not external. This is the distinction the
        // highlighting rests on, a native and a local helper must not look the same.
        Assert.DoesNotContain("Helper", ext);
        Assert.DoesNotContain("Trig_Actions", ext);

        // Not a call site, so not a call.
        Assert.DoesNotContain("udg_Count", ext);
        Assert.DoesNotContain("u", ext);
        // A keyword is never reported, even where one is followed by a paren.
        Assert.DoesNotContain("if", ext);
        Assert.DoesNotContain("integer", ext);
    }

    [Fact]
    public void A_call_with_space_before_the_paren_still_counts()
    {
        var ext = JassSyntax.ExternalCalls("function F takes nothing returns nothing\n"
                                         + "call SomeNative (1)\nendfunction\n");
        Assert.Contains("SomeNative", ext);
    }

    [Fact]
    public void Empty_and_paren_free_input_yield_nothing_rather_than_throwing()
    {
        Assert.Empty(JassSyntax.ExternalCalls(""));
        Assert.Empty(JassSyntax.ExternalCalls("// just a comment\n"));
        Assert.Empty(JassSyntax.ExternalCalls("globals\n integer x = 0\nendglobals\n"));
        Assert.Empty(JassSyntax.NativesIn(""));
        Assert.Empty(JassSyntax.DeclaredTypes(""));
    }

    [Fact]
    public void NativesIn_reads_a_common_j_style_declaration()
    {
        const string CommonJ = """
            native CreateUnit takes player id, integer unitid returns unit
            constant native GetPlayerId takes player whichPlayer returns integer
            function BJDebugMsg takes string msg returns nothing
            function interface HandlerFunc takes nothing returns nothing
            // native Commented takes nothing returns nothing
            type unitpool extends handle
            """;
        var natives = JassSyntax.NativesIn(CommonJ);

        Assert.Contains("CreateUnit", natives);
        Assert.Contains("GetPlayerId", natives);       // "constant native"
        Assert.Contains("BJDebugMsg", natives);        // Blizzard.j is external to a map too
        Assert.DoesNotContain("HandlerFunc", natives); // "function interface" has no body
        Assert.DoesNotContain("Commented", natives);
        Assert.DoesNotContain("unitpool", natives);    // a type, not a callable
    }

    [Fact]
    public void DeclaredTypes_finds_a_scripts_own_handle_types()
    {
        var types = JassSyntax.DeclaredTypes(
            "type mything extends handle\ntype other extends agent\nlocal integer x = 0\n");
        Assert.Equal(new[] { "mything", "other" }, types.OrderBy(t => t).ToArray());
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void External_calls_over_a_real_eight_megabyte_script_is_fast_enough_for_a_load()
    {
        var path = Path.Combine(Path.GetTempPath(), "wc3ctl-corpus-war3map.j");
        if (!File.Exists(path)) { _out.WriteLine("no corpus script, skipped"); return; }

        var source = File.ReadAllText(path, System.Text.Encoding.Latin1);

        var sw = Stopwatch.StartNew();
        var ext = JassSyntax.ExternalCalls(source);
        var took = sw.Elapsed;

        _out.WriteLine($"{source.Length:N0} chars, {ext.Count:N0} externals, "
                     + $"{took.TotalMilliseconds:F0}ms");

        // This runs once per script load in the editor, so it shares the load budget. A regex
        // was the obvious way to write it and a manual character pass is why this is affordable.
        Assert.True(took.TotalMilliseconds < 1500,
            $"deriving externals took {took.TotalMilliseconds:F0}ms, which is too much to spend "
            + "on highlighting during a panel load");
        Assert.NotEmpty(ext);
        // A real map script calls the obvious ones. If none of these are present the pass is
        // finding nothing useful and the highlighting would be no better than keywords only.
        Assert.Contains("CreateUnit", ext);
    }
}
