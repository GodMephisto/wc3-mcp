// tests/Wc3.Tests/StringsToolkitTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Hermetic tests for the five wc3ctl strings verbs. Each builds a synthetic map carrying a
/// war3map.wts (and a script where a reference is needed) so the scan, the diff and the edit are
/// exercised without a game install.
/// </summary>
public class StringsToolkitTests
{
    private const string Wts =
        "STRING 1\r\n{\r\nused by script\r\n}\r\n\r\n" +
        "STRING 2\r\n{\r\nunused entry\r\n}\r\n\r\n" +
        "STRING 3\r\n{\r\nfind me needle\r\n}\r\n";

    private static MapDocument Map(string wts, string? script = null)
    {
        var files = new Dictionary<string, byte[]> { ["war3map.wts"] = Encoding.UTF8.GetBytes(wts) };
        if (script is not null) files["war3map.j"] = Encoding.UTF8.GetBytes(script);
        return MapDocument.Load(SyntheticMap.Build(files));
    }

    private const string ScriptReferencingOneAndMissing =
        "function main takes nothing returns nothing\n" +
        "    call DisplayTextToForce(bj_FORCE_ALL_PLAYERS, \"TRIGSTR_1\")\n" +
        "    call DisplayTextToForce(bj_FORCE_ALL_PLAYERS, \"TRIGSTR_999\")\n" +
        "endfunction\n";

    [Fact]
    public void Missing_reports_script_reference_with_no_entry()
    {
        var r = StringsMissingCommand.Execute(Map(Wts, ScriptReferencingOneAndMissing));

        Assert.Equal(new[] { 999 }, r.MissingIds);
        Assert.Equal(3, r.DefinedEntries);
        Assert.Equal(2, r.ReferencedIds);
    }

    [Fact]
    public void Missing_on_a_map_without_script_is_empty()
    {
        var r = StringsMissingCommand.Execute(Map(Wts));

        Assert.Empty(r.MissingIds);
        Assert.Equal(0, r.ReferencedIds);
    }

    [Fact]
    public void Unused_reports_entries_nothing_references()
    {
        var r = StringsUnusedCommand.Execute(Map(Wts, ScriptReferencingOneAndMissing));

        Assert.Equal(new[] { 2, 3 }, r.Unused.Select(u => u.Id));
        Assert.Equal("unused entry", r.Unused[0].Text);
        Assert.Equal(3, r.DefinedEntries);
    }

    [Fact]
    public void Unused_is_empty_when_every_entry_is_referenced()
    {
        var wts = "STRING 1\r\n{\r\na\r\n}\r\nSTRING 2\r\n{\r\nb\r\n}\r\n";
        var script = "function main takes nothing returns nothing\n"
            + "    call X(\"TRIGSTR_1\")\n    call X(\"TRIGSTR_2\")\nendfunction\n";
        Assert.Empty(StringsUnusedCommand.Execute(Map(wts, script)).Unused);
    }

    [Fact]
    public void Diff_reports_added_removed_and_changed()
    {
        var a = Map(Wts);
        var b = Map(
            "STRING 1\r\n{\r\nused by script CHANGED\r\n}\r\n\r\n" +
            "STRING 3\r\n{\r\nfind me needle\r\n}\r\n\r\n" +
            "STRING 4\r\n{\r\nadded\r\n}\r\n");

        var r = StringsDiffCommand.Execute(a, b);

        Assert.Equal(new[] { 4 }, r.Added.Select(e => e.Id));
        Assert.Equal(new[] { 2 }, r.Removed.Select(e => e.Id));
        var change = Assert.Single(r.Changed);
        Assert.Equal(1, change.Id);
        Assert.Equal("used by script", change.OldText);
        Assert.Equal("used by script CHANGED", change.NewText);
    }

    [Fact]
    public void Diff_of_a_map_against_itself_is_empty()
    {
        var r = StringsDiffCommand.Execute(Map(Wts), Map(Wts));
        Assert.Empty(r.Added);
        Assert.Empty(r.Removed);
        Assert.Empty(r.Changed);
    }

    [Fact]
    public void Find_matches_case_insensitively_and_reports_the_id()
    {
        var r = StringsFindCommand.Execute(Map(Wts), "NEEDLE");

        var hit = Assert.Single(r.Matches);
        Assert.Equal(3, hit.Id);
        Assert.Equal("find me needle", hit.Text);
        Assert.Equal(1, r.MatchCount);
    }

    [Fact]
    public void Find_no_match_is_empty()
    {
        Assert.Empty(StringsFindCommand.Execute(Map(Wts), "absent").Matches);
    }

    [Fact]
    public void Set_replaces_an_existing_entry_and_writes_a_copy()
    {
        var dir = Path.Combine(Path.GetTempPath(), "wc3strings", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        var input = Path.Combine(dir, "in.w3x");
        File.WriteAllBytes(input, SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.wts"] = Encoding.UTF8.GetBytes(Wts),
        }));
        var output = Path.Combine(dir, "out.w3x");
        try
        {
            var r = StringsSetCommand.Execute(input, 3, "edited", output);

            Assert.False(r.Added);
            Assert.Equal(output, r.SavedTo);
            Assert.Equal("edited", StringsCommand.List(MapDocument.Load(output)).Entries.Single(e => e.Id == 3).Text);
            // The input is untouched.
            Assert.Equal("find me needle", StringsCommand.List(MapDocument.Load(input)).Entries.Single(e => e.Id == 3).Text);
        }
        finally { Directory.Delete(dir, recursive: true); }
    }

    [Fact]
    public void Set_adds_a_missing_entry()
    {
        var dir = Path.Combine(Path.GetTempPath(), "wc3strings", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        var input = Path.Combine(dir, "in.w3x");
        File.WriteAllBytes(input, SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.wts"] = Encoding.UTF8.GetBytes(Wts),
        }));
        var output = Path.Combine(dir, "out.w3x");
        try
        {
            var r = StringsSetCommand.Execute(input, 42, "brand new", output);

            Assert.True(r.Added);
            Assert.Equal("brand new", StringsCommand.List(MapDocument.Load(output)).Entries.Single(e => e.Id == 42).Text);
        }
        finally { Directory.Delete(dir, recursive: true); }
    }

    [Fact]
    public void Set_refuses_the_input_map_as_output()
    {
        var dir = Path.Combine(Path.GetTempPath(), "wc3strings", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        var input = Path.Combine(dir, "in.w3x");
        File.WriteAllBytes(input, SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.wts"] = Encoding.UTF8.GetBytes(Wts),
        }));
        try
        {
            Assert.Throws<InvalidOperationException>(() => StringsSetCommand.Execute(input, 1, "x", input));
        }
        finally { Directory.Delete(dir, recursive: true); }
    }
}
