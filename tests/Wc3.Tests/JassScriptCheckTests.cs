// tests/Wc3.Tests/JassScriptCheckTests.cs
// The compile gate. In JASS one undeclared variable fails the whole war3map.j, so config() never
// runs and a hosted map shows no player slots - the failure that repeatedly looked like "the lobby
// is bugged". These lock in both halves: catching it, and never crying wolf on a healthy script.
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

public class JassScriptCheckTests
{
    private static IReadOnlyList<JassIssue> Check(string jass) => JassScriptCheck.Check(jass);

    private static IEnumerable<JassIssueKind> Kinds(string jass) => Check(jass).Select(i => i.Kind);

    /// <summary>A minimal script that hosts, so a fixture only exercises the check under test.</summary>
    private const string Hostable =
        "function config takes nothing returns nothing\n" +
        "    call SetPlayers(1)\n" +
        "endfunction\n" +
        "function main takes nothing returns nothing\n" +
        "endfunction\n";

    [Fact]
    public void A_healthy_script_reports_nothing()
    {
        Assert.Empty(Check(Hostable));
    }

    [Fact]
    public void Assigning_an_undeclared_variable_is_an_error()
    {
        string jass = Hostable +
            "function Spell takes nothing returns nothing\n" +
            "    set angle=1.0\n" +
            "endfunction\n";

        var issue = Assert.Single(Check(jass));
        Assert.Equal(JassIssueKind.UndeclaredAssignmentTarget, issue.Kind);
        Assert.Equal(DiagnosticSeverity.Error, issue.Severity);
        Assert.Contains("angle", issue.Message);
    }

    [Fact]
    public void Locals_parameters_globals_and_bj_names_are_all_declared()
    {
        // Each assignment target here is legitimately declared, including bj_* which lives in
        // Blizzard.j rather than the map, so none of them may be reported.
        string jass = Hostable +
            "globals\n" +
            "    integer udg_score= 0\n" +
            "    real array udg_pos\n" +
            "endglobals\n" +
            "function Spell takes unit caster,real amount returns nothing\n" +
            "    local integer i= 0\n" +
            "    local real array temp\n" +
            "    set i=1\n" +
            "    set caster=null\n" +
            "    set amount=2.0\n" +
            "    set udg_score=3\n" +
            "    set udg_pos[0]=4.0\n" +
            "    set temp[1]=5.0\n" +
            "    set bj_lastPlayedSound=null\n" +
            "endfunction\n";

        Assert.Empty(Check(jass));
    }

    [Fact]
    public void A_commented_out_declaration_whose_variable_is_still_used_is_an_error()
    {
        // Exactly the shape the porter used to leave behind.
        string jass = Hostable +
            "function Spell takes nothing returns nothing\n" +
            "//[wc3ctl trimmed]     local real angle=AngleBetween(a, b)\n" +
            "    call BJDebugMsg(R2S(angle))\n" +
            "endfunction\n";

        var kinds = Kinds(jass).ToList();
        Assert.Contains(JassIssueKind.DroppedLocalDeclaration, kinds);
        Assert.False(JassScriptCheck.IsCompilable(Check(jass)));
    }

    [Fact]
    public void The_healthy_trimmed_form_that_keeps_the_declaration_is_not_reported()
    {
        // The initializer is dropped but the variable stays declared, which compiles. This must not
        // be mistaken for the broken form, or every correctly trimmed port would look broken.
        string jass = Hostable +
            "function Spell takes nothing returns nothing\n" +
            "    local real angle //[wc3ctl trimmed] (initializer dropped)\n" +
            "    call BJDebugMsg(R2S(angle))\n" +
            "endfunction\n";

        Assert.Empty(Check(jass));
    }

    [Fact]
    public void Repair_restores_the_declaration_and_keeps_the_line_count()
    {
        string jass = Hostable +
            "function Spell takes nothing returns nothing\n" +
            "//[wc3ctl trimmed]     local real angle=AngleBetween(a, b)\n" +
            "    call BJDebugMsg(R2S(angle))\n" +
            "endfunction\n";

        var repaired = JassScriptCheck.Repair(jass, out int count);

        Assert.Equal(1, count);
        Assert.Contains("local real angle //[wc3ctl trimmed] (initializer dropped)", repaired);
        // Line numbers elsewhere must survive a repair, so the count cannot change.
        Assert.Equal(jass.Split('\n').Length, repaired.Split('\n').Length);
        Assert.Empty(Check(repaired));
    }

    [Fact]
    public void Repair_is_idempotent_and_a_no_op_on_a_healthy_script()
    {
        JassScriptCheck.Repair(Hostable, out int none);
        Assert.Equal(0, none);

        string broken = Hostable +
            "function S takes nothing returns nothing\n" +
            "//[wc3ctl trimmed]     local integer id=NewTimer()\n" +
            "    set id=1\n" +
            "endfunction\n";
        var once = JassScriptCheck.Repair(broken, out int first);
        var twice = JassScriptCheck.Repair(once, out int second);

        Assert.Equal(1, first);
        Assert.Equal(0, second);
        Assert.Equal(once, twice);
    }

    [Fact]
    public void An_array_declaration_survives_repair()
    {
        string jass = Hostable +
            "function S takes nothing returns nothing\n" +
            "//[wc3ctl trimmed]     local real array buf\n" +
            "    set buf[0]=1.0\n" +
            "endfunction\n";

        var repaired = JassScriptCheck.Repair(jass, out int count);

        Assert.Equal(1, count);
        Assert.Contains("local real array buf", repaired);
        Assert.Empty(Check(repaired));
    }

    [Fact]
    public void Duplicate_functions_and_globals_are_errors()
    {
        string jass = Hostable +
            "globals\n    integer dup= 1\n    integer dup= 2\nendglobals\n" +
            "function Twice takes nothing returns nothing\nendfunction\n" +
            "function Twice takes nothing returns nothing\nendfunction\n";

        var kinds = Kinds(jass).ToList();
        Assert.Contains(JassIssueKind.DuplicateFunction, kinds);
        Assert.Contains(JassIssueKind.DuplicateGlobal, kinds);
    }

    [Fact]
    public void Unbalanced_blocks_are_errors()
    {
        string jass = Hostable +
            "function S takes nothing returns nothing\n" +
            "    if true then\n" +
            "        call BJDebugMsg(\"x\")\n" +
            "endfunction\n";

        Assert.Contains(JassIssueKind.UnbalancedBlock, Kinds(jass));
    }

    [Fact]
    public void Commented_and_string_content_is_not_mistaken_for_code()
    {
        // A "set" inside a line comment, a block comment, or this tool's own trimmed-statement
        // marker must never be read as a real assignment.
        string jass = Hostable +
            "function S takes nothing returns nothing\n" +
            "    // set ghost=1\n" +
            "/* set phantom=2\n" +
            "   set wraith=3 */\n" +
            "//[wc3ctl trimmed]     set dropped=SomeDroppedCall()\n" +
            "endfunction\n";

        Assert.Empty(Check(jass));
    }

    [Fact]
    public void A_missing_entry_point_is_reported_but_does_not_block_compilation()
    {
        // A script fragment legitimately has no config/main, so this must not stop a port from
        // splicing, while a real map still gets told it cannot host.
        string fragment =
            "function Spell takes nothing returns nothing\n" +
            "endfunction\n";

        var issues = Check(fragment);

        Assert.Contains(JassIssueKind.MissingEntryPoint, issues.Select(i => i.Kind));
        Assert.False(JassScriptCheck.IsClean(issues));      // a map validator objects
        Assert.True(JassScriptCheck.IsCompilable(issues));  // but the script still compiles
    }

    [Fact]
    public void A_payload_with_no_functions_is_not_analyzed()
    {
        // Binary or unparseable content is described by the loader and empty-file checks, so
        // claiming "no config function" on top of that would be noise.
        Assert.Empty(Check("not a script at all"));
        Assert.Empty(Check(""));
    }

    [Fact]
    public void Config_without_player_slots_is_a_warning_not_an_error()
    {
        string jass =
            "function config takes nothing returns nothing\n" +
            "endfunction\n" +
            "function main takes nothing returns nothing\n" +
            "endfunction\n";

        var issue = Assert.Single(Check(jass));
        Assert.Equal(JassIssueKind.NoPlayerSlots, issue.Kind);
        Assert.Equal(DiagnosticSeverity.Warning, issue.Severity);
        Assert.True(JassScriptCheck.IsClean(Check(jass)));  // warnings never flip the verdict
    }
}
