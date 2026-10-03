// tests/Wc3.Tests/JassReferencesTests.cs
using System.Diagnostics;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// "Who calls this", the question a merged arena script raises constantly. 3,009 functions over
/// 113,387 lines, where reading one function means asking what reaches it far more often than
/// asking where it is declared.
///
/// The case a plain text search gets wrong is the code reference. JASS passes a function as a
/// <c>code</c> value by naming it WITHOUT parentheses, so Condition, Filter and TriggerAddAction
/// are how most handlers are actually reached, and a search for "Foo(" misses every one of them.
/// On a trigger-driven map that is most of the answer.
/// </summary>
public class JassReferencesTests
{
    private readonly ITestOutputHelper _out;
    public JassReferencesTests(ITestOutputHelper output) => _out = output;

    private const string Script = """
        globals
            trigger gg_trg_Hero = null
        endglobals

        function Trig_Hero_Conditions takes nothing returns boolean
            return GetSpellAbilityId() == 'A00R'
        endfunction

        function Trig_Hero_Actions takes nothing returns nothing
            call BJDebugMsg("cast")
        endfunction

        function Helper takes nothing returns nothing
            call Trig_Hero_Actions()
        endfunction

        function InitTrig_Hero takes nothing returns nothing
            set gg_trg_Hero = CreateTrigger()
            call TriggerAddCondition(gg_trg_Hero, Condition(function Trig_Hero_Conditions))
            call TriggerAddAction(gg_trg_Hero, function Trig_Hero_Actions)
            // call Trig_Hero_Actions()   commented out, must not count
        endfunction
        """;

    [Fact]
    public void A_declaration_is_reported_as_a_declaration()
    {
        var refs = JassReferences.Find(Script, "Trig_Hero_Actions");
        var decl = refs.Where(r => r.Kind == JassReferenceKind.Declaration).ToList();

        // Against the index rather than a hardcoded number, so editing the sample above cannot
        // break this for the wrong reason. The first version said 10 and the answer was 9.
        var declared = JassFunctionIndex.Parse(Script).Single(f => f.Name == "Trig_Hero_Actions");

        Assert.Single(decl);
        Assert.Equal(declared.StartLine, decl[0].Line);
        Assert.Equal("Trig_Hero_Actions", decl[0].InFunction);   // inside its own body
    }

    [Fact]
    public void A_direct_call_is_found_with_its_owning_function()
    {
        var refs = JassReferences.FindUses(Script, "Trig_Hero_Actions");
        var call = refs.Single(r => r.Kind == JassReferenceKind.Call);

        _out.WriteLine($"line {call.Line} in {call.InFunction}: {call.Text}");
        Assert.Equal("Helper", call.InFunction);
        Assert.Contains("call Trig_Hero_Actions()", call.Text);
    }

    [Fact]
    public void A_function_passed_as_code_is_found_too()
    {
        // The half a text search for "Foo(" cannot see, and on a trigger-driven map the important
        // half. TriggerAddAction(t, function Foo) is how the handler is actually reached.
        var refs = JassReferences.FindUses(Script, "Trig_Hero_Actions");
        var codeRef = refs.Single(r => r.Kind == JassReferenceKind.CodeReference);

        _out.WriteLine($"line {codeRef.Line} in {codeRef.InFunction}: {codeRef.Text}");
        Assert.Equal("InitTrig_Hero", codeRef.InFunction);
        Assert.Contains("TriggerAddAction", codeRef.Text);
    }

    [Fact]
    public void A_condition_function_reference_is_a_code_reference_not_a_declaration()
    {
        var refs = JassReferences.Find(Script, "Trig_Hero_Conditions");
        _out.WriteLine(string.Join("\n", refs.Select(r => $"  {r.Line} {r.Kind} in {r.InFunction}")));

        Assert.Single(refs, r => r.Kind == JassReferenceKind.Declaration);
        var used = refs.Single(r => r.Kind == JassReferenceKind.CodeReference);
        Assert.Equal("InitTrig_Hero", used.InFunction);
        Assert.Contains("Condition(function Trig_Hero_Conditions)", used.Text);
    }

    [Fact]
    public void A_commented_out_call_is_not_a_reference()
    {
        var refs = JassReferences.FindUses(Script, "Trig_Hero_Actions");
        _out.WriteLine($"{refs.Count} use(s): "
                     + string.Join(", ", refs.Select(r => $"{r.Line}/{r.Kind}")));

        // One real call and one code reference. The commented third would make three.
        Assert.Equal(2, refs.Count);
        Assert.DoesNotContain(refs, r => r.Text.TrimStart().StartsWith("//", StringComparison.Ordinal));
    }

    [Fact]
    public void A_global_is_found_at_its_declaration_and_its_uses()
    {
        var refs = JassReferences.Find(Script, "gg_trg_Hero");
        _out.WriteLine(string.Join("\n",
            refs.Select(r => $"  {r.Line} {r.Kind} in {r.InFunction ?? "(file scope)"}")));

        // Declared in the globals block, at file scope rather than inside any function.
        Assert.Contains(refs, r => r.Line == 2 && r.InFunction is null);
        // And used three times inside InitTrig_Hero.
        Assert.Equal(3, refs.Count(r => r.InFunction == "InitTrig_Hero"));
    }

    [Fact]
    public void Matching_is_whole_word()
    {
        const string Tricky = """
            function Foo takes nothing returns nothing
            endfunction
            function Bar takes nothing returns nothing
                call FooBar()
                call MyFoo()
                call Foo()
            endfunction
            """;
        var refs = JassReferences.FindUses(Tricky, "Foo");
        _out.WriteLine(string.Join(", ", refs.Select(r => $"{r.Line}:{r.Text}")));

        // Only the real one. FooBar and MyFoo are different names.
        Assert.Single(refs);
        Assert.Contains("call Foo()", refs[0].Text);
    }

    [Theory]
    [InlineData("\n")]
    [InlineData("\r\n")]
    [InlineData("\r")]
    public void References_are_found_whatever_the_line_terminator(string eol)
    {
        var refs = JassReferences.FindUses(Script.Replace("\n", eol), "Trig_Hero_Actions");
        Assert.Equal(2, refs.Count);
    }

    [Fact]
    public void A_name_that_is_not_an_identifier_matches_nothing()
    {
        // A blank or punctuation query must not match everything, which is the failure mode of a
        // naive substring search.
        Assert.Empty(JassReferences.Find(Script, ""));
        Assert.Empty(JassReferences.Find(Script, "("));
        Assert.Empty(JassReferences.Find(Script, "Trig Hero"));
        Assert.Empty(JassReferences.Find("", "Foo"));
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Finding_references_in_a_real_eight_megabyte_script_is_fast_enough_to_be_a_keystroke()
    {
        var path = Path.Combine(Path.GetTempPath(), "wc3ctl-corpus-war3map.j");
        if (!File.Exists(path)) { _out.WriteLine("no corpus script, skipped"); return; }

        var source = File.ReadAllText(path, System.Text.Encoding.Latin1);
        var functions = JassFunctionIndex.Parse(source);

        // The most-called function in the script, so the worst case for result volume.
        var probe = functions.Select(f => f.Name)
            .OrderByDescending(n => source.Split(n).Length)
            .First();

        var sw = Stopwatch.StartNew();
        var refs = JassReferences.FindUses(source, probe);
        var took = sw.Elapsed;

        _out.WriteLine($"{source.Length:N0} chars, {functions.Count:N0} functions");
        _out.WriteLine($"'{probe}' has {refs.Count:N0} use(s), found in {took.TotalMilliseconds:F0}ms");
        foreach (var r in refs.Take(5))
            _out.WriteLine($"   {r.Line,7} {r.Kind,-14} {r.InFunction}");

        Assert.NotEmpty(refs);
        // This runs when a reader asks, so it shares the budget of a keypress rather than a load.
        Assert.True(took.TotalMilliseconds < 2000,
            $"finding references took {took.TotalMilliseconds:F0}ms, too slow to hang off a key");
    }
}
