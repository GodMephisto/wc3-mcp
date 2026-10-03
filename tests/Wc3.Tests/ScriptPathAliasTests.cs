// tests/Wc3.Tests/ScriptPathAliasTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// A map's script is not always at the archive root. Measured over 34 maps in the user's Maps
/// folder, 13 store it as <c>scripts\war3map.j</c>, which is what the widely used map optimizer
/// emits, and 2 hold both.
///
/// The prober already knew that and recovered the name, so <c>ls</c> listed the file. Every caller
/// that asked for "war3map.j" still got nothing, so <c>extract war3map.j</c> answered "file not
/// found" for a file it had listed on the line above, and <c>validate</c> answered "map has no
/// script file, it cannot run" and exited 2, on 13 working maps.
/// </summary>
public class ScriptPathAliasTests
{
    private const string Script =
        "function config takes nothing returns nothing\ncall SetPlayers(2)\nendfunction\n"
        + "function main takes nothing returns nothing\nendfunction\n";

    /// <summary>
    /// A map whose script sits under scripts\ and nowhere else, built rather than edited because
    /// a document has no remove and BlankMap always puts the script at the root.
    /// </summary>
    private static MapDocument NestedScriptMap() =>
        MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3i"] = BlankMap.Create().GetFile("war3map.w3i")!.CurrentBytes,
            ["war3map.w3e"] = BlankMap.Create().GetFile("war3map.w3e")!.CurrentBytes,
            ["scripts\\war3map.j"] = Encoding.UTF8.GetBytes(Script),
        }));

    [Fact]
    public void An_exact_match_always_wins_over_an_alias()
    {
        // Two maps in the corpus hold both. The root one is the one the game reads, so a fallback
        // must never shadow it.
        var doc = BlankMap.Create();
        doc.AddOrReplaceRawFile("war3map.j", Encoding.UTF8.GetBytes("// root\n"));
        doc.AddOrReplaceRawFile("scripts\\war3map.j", Encoding.UTF8.GetBytes("// nested\n"));

        var hit = doc.GetFile("war3map.j");
        Assert.NotNull(hit);
        Assert.Equal("war3map.j", hit!.FileName);
        Assert.Contains("root", Encoding.UTF8.GetString(hit.CurrentBytes));
    }

    [Fact]
    public void A_nested_script_resolves_by_its_root_name()
    {
        var doc = NestedScriptMap();
        var hit = doc.GetFile("war3map.j");
        Assert.NotNull(hit);
        Assert.Equal("scripts\\war3map.j", hit!.FileName);
    }

    [Fact]
    public void The_alias_is_symmetric()
    {
        // A caller that knows the nested name must resolve a root-stored script too, or the two
        // spellings would each work only half the time.
        var doc = BlankMap.Create();
        Assert.NotNull(doc.GetFile("war3map.j"));
        Assert.NotNull(doc.GetFile("scripts\\war3map.j"));
        Assert.Equal("war3map.j", doc.GetFile("scripts\\war3map.j")!.FileName);
    }

    [Fact]
    public void An_unrelated_name_has_no_alias()
    {
        // The list is narrow on purpose. Answering a question about war3map.j with somebody's
        // imported art would be worse than answering it with nothing.
        Assert.Empty(StandardMapFileNames.AliasesFor("war3map.w3e"));
        Assert.Empty(StandardMapFileNames.AliasesFor("war3mapImported\\thing.blp"));
        Assert.Empty(StandardMapFileNames.AliasesFor(""));
    }

    [Fact]
    public void Extract_finds_a_nested_script_by_its_root_name()
    {
        var doc = NestedScriptMap();
        var result = ExtractCommand.Execute(doc, new ExtractSelector { ExactName = "war3map.j" });

        Assert.Single(result.Items);
        Assert.Contains("function main", Encoding.UTF8.GetString(result.Items[0].Bytes));
    }

    [Fact]
    public void Validate_does_not_call_a_nested_script_map_unrunnable()
    {
        var doc = NestedScriptMap();
        var result = ValidateCommand.Execute(doc);

        Assert.DoesNotContain(result.Issues,
            i => i.Message.Contains("no script file", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void The_function_index_reads_a_nested_script()
    {
        var doc = NestedScriptMap();
        var result = ScriptCommand.Functions(doc);

        Assert.Equal("scripts\\war3map.j", result.ScriptFile);
        Assert.Equal(2, result.Functions.Count);
        Assert.Contains(result.Functions, f => f.Name == "config");
        Assert.Contains(result.Functions, f => f.Name == "main");
    }
}
