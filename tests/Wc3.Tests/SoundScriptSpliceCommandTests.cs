// tests/Wc3.Tests/SoundScriptSpliceCommandTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// Coverage of <see cref="SoundScriptSpliceCommand"/>. The block under test is the one
/// <see cref="SoundImportCommand"/> actually generates, split into globals and functions by the
/// splice. The placement facts are hermetic, and the three <c>Pjass</c> facts hand the assembled
/// whole script to the real compiler, which is the only proof that the placement is legal.
/// </summary>
public class SoundScriptSpliceCommandTests
{
    private const string ScriptName = "war3map.j";
    private const string Marker = "wc3ctl sound import a13";
    private const string Install = @"C:\Warcraft III";

    /// <summary>A script that already declares a globals block, which is the case that broke the
    /// first whole-block append. Every line is CRLF, including the last.</summary>
    private const string CrlfScriptWithGlobals =
        "function config takes nothing returns nothing\r\n" +
        "    call SetPlayers(1)\r\n" +
        "endfunction\r\n" +
        "\r\n" +
        "globals\r\n" +
        "    integer udg_count = 0\r\n" +
        "endglobals\r\n" +
        "\r\n" +
        "function main takes nothing returns nothing\r\n" +
        "    set udg_count = 1\r\n" +
        "endfunction\r\n" +
        "\r\n" +
        "function Extra takes nothing returns integer\r\n" +
        "    return udg_count\r\n" +
        "endfunction\r\n";

    /// <summary>The same script with no globals block, so the splice has to create one.</summary>
    private const string CrlfScriptWithoutGlobals =
        "function config takes nothing returns nothing\r\n" +
        "    call SetPlayers(1)\r\n" +
        "endfunction\r\n" +
        "\r\n" +
        "function main takes nothing returns nothing\r\n" +
        "    call BJDebugMsg(\"hi\")\r\n" +
        "endfunction\r\n" +
        "\r\n" +
        "function Extra takes nothing returns integer\r\n" +
        "    return 7\r\n" +
        "endfunction\r\n";

    private static readonly string[] OriginalFunctions = { "config", "main", "Extra" };

    [Fact]
    public void Splice_merges_globals_into_an_existing_globals_block()
    {
        var doc = MapWithScript(CrlfScriptWithGlobals);
        var original = Encoding.UTF8.GetBytes(CrlfScriptWithGlobals);

        var result = SoundScriptSpliceCommand.Splice(doc, DispatcherBlock(), Marker);

        Assert.True(result.Ok, result.Message);
        Assert.False(result.AlreadyPresent);

        var spliced = FileEditCommand.ReadBytes(doc, ScriptName);
        Assert.Equal(original.Length + result.BytesAdded, spliced.Length);
        Assert.Equal(spliced.Length, result.NewLength);

        var text = Encoding.UTF8.GetString(spliced);
        Assert.Equal(1, CountLines(text, "globals"));
        Assert.Equal(1, CountLines(text, "endglobals"));

        // The declarations landed inside the block, before the script's own endglobals.
        int globalsAt = text.IndexOf("globals\r\n", StringComparison.Ordinal);
        int endglobalsAt = text.IndexOf("endglobals", StringComparison.Ordinal);
        int slotsAt = text.IndexOf("constant integer SND_VOICE_SLOTS", StringComparison.Ordinal);
        Assert.True(globalsAt >= 0 && slotsAt > globalsAt && slotsAt < endglobalsAt);

        // The untouched region before the insertion point is an exact byte prefix, and the
        // original bytes from the endglobals line onward survive contiguously.
        int cut = CrlfScriptWithGlobals.IndexOf("\r\nendglobals", StringComparison.Ordinal);
        Assert.Equal(original[..cut], spliced[..cut]);
        Assert.Contains(Encoding.Latin1.GetString(original[cut..]),
            Encoding.Latin1.GetString(spliced), StringComparison.Ordinal);

        AssertAllOriginalFunctionsPresent(text);
        AssertOnlyCrlf(text);
    }

    [Fact]
    public void Splice_creates_a_globals_block_before_the_first_function_when_none_exists()
    {
        var doc = MapWithScript(CrlfScriptWithoutGlobals);
        var original = Encoding.UTF8.GetBytes(CrlfScriptWithoutGlobals);

        var result = SoundScriptSpliceCommand.Splice(doc, DispatcherBlock(), Marker);

        Assert.True(result.Ok, result.Message);
        Assert.False(result.AlreadyPresent);

        var spliced = FileEditCommand.ReadBytes(doc, ScriptName);
        var text = Encoding.UTF8.GetString(spliced);

        Assert.Equal(1, CountLines(text, "globals"));
        Assert.Equal(1, CountLines(text, "endglobals"));

        int globalsAt = text.IndexOf("globals\r\n", StringComparison.Ordinal);
        int firstFunction = text.IndexOf("function config", StringComparison.Ordinal);
        int slotsAt = text.IndexOf("constant integer SND_VOICE_SLOTS", StringComparison.Ordinal);
        Assert.True(globalsAt >= 0 && globalsAt < firstFunction);
        Assert.True(slotsAt > globalsAt && slotsAt < text.IndexOf("endglobals", StringComparison.Ordinal));

        // Nothing was inserted before the globals block, so everything the script had before its
        // first function is an exact prefix of the result.
        int cut = CrlfScriptWithoutGlobals.IndexOf("function config", StringComparison.Ordinal);
        Assert.Equal(original[..cut], spliced[..cut]);

        AssertAllOriginalFunctionsPresent(text);
        AssertOnlyCrlf(text);
    }

    [Fact]
    public void Splice_is_idempotent_and_reports_already_present()
    {
        var doc = MapWithScript(CrlfScriptWithGlobals);
        var block = DispatcherBlock();
        var first = SoundScriptSpliceCommand.Splice(doc, block, Marker);
        Assert.True(first.Ok, first.Message);
        var afterFirst = FileEditCommand.ReadBytes(doc, ScriptName);

        var second = SoundScriptSpliceCommand.Splice(doc, block, Marker);

        Assert.True(second.Ok, second.Message);
        Assert.True(second.AlreadyPresent);
        Assert.Equal(0, second.BytesAdded);
        Assert.Equal(afterFirst.Length, second.NewLength);

        var afterSecond = FileEditCommand.ReadBytes(doc, ScriptName);
        Assert.Equal(afterFirst, afterSecond);
        Assert.Equal(1, CountLines(Encoding.UTF8.GetString(afterSecond), "globals"));
        AssertAllOriginalFunctionsPresent(Encoding.UTF8.GetString(afterSecond));
    }

    [Fact]
    public void Splice_refuses_a_map_with_no_script()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3i"] = new byte[] { 0 },
        }));

        var result = SoundScriptSpliceCommand.Splice(doc, DispatcherBlock(), Marker);

        Assert.False(result.Ok);
        Assert.False(result.AlreadyPresent);
        Assert.Equal(0, result.BytesAdded);
        Assert.Contains("war3map.j", result.Message);
    }

    [Fact]
    public void Splice_refuses_a_block_with_no_function()
    {
        var doc = MapWithScript(CrlfScriptWithGlobals);
        var before = FileEditCommand.ReadBytes(doc, ScriptName);
        const string notJass = "// a comment with endfunction in it\nset x = 1\n";

        var result = SoundScriptSpliceCommand.Splice(doc, notJass, Marker);

        Assert.False(result.Ok);
        Assert.False(result.AlreadyPresent);
        Assert.Equal(0, result.BytesAdded);
        Assert.Equal(before.Length, result.NewLength);
        Assert.Contains("function", result.Message);
        Assert.Equal(before, FileEditCommand.ReadBytes(doc, ScriptName));
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Pjass_accepts_the_whole_script_when_a_globals_block_exists()
    {
        var doc = MapWithScript(CrlfScriptWithGlobals);
        var result = SoundScriptSpliceCommand.Splice(doc, DispatcherBlock(), Marker);
        Assert.True(result.Ok, result.Message);

        var text = Encoding.UTF8.GetString(FileEditCommand.ReadBytes(doc, ScriptName));
        AssertCompiles(text);
        AssertAllOriginalFunctionsPresent(text);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Pjass_accepts_the_whole_script_when_no_globals_block_exists()
    {
        var doc = MapWithScript(CrlfScriptWithoutGlobals);
        var result = SoundScriptSpliceCommand.Splice(doc, DispatcherBlock(), Marker);
        Assert.True(result.Ok, result.Message);

        var text = Encoding.UTF8.GetString(FileEditCommand.ReadBytes(doc, ScriptName));
        AssertCompiles(text);
        AssertAllOriginalFunctionsPresent(text);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Pjass_accepts_the_whole_script_after_a_duplicate_splice()
    {
        var doc = MapWithScript(CrlfScriptWithGlobals);
        var block = DispatcherBlock();
        Assert.True(SoundScriptSpliceCommand.Splice(doc, block, Marker).Ok);
        var second = SoundScriptSpliceCommand.Splice(doc, block, Marker);
        Assert.True(second.AlreadyPresent);

        var text = Encoding.UTF8.GetString(FileEditCommand.ReadBytes(doc, ScriptName));
        AssertCompiles(text);
        AssertAllOriginalFunctionsPresent(text);
        Assert.Equal(1, CountOf(text, "function SND_EventPlay"));
    }

    /// <summary>
    /// Hands the whole assembled script to pjass through <see cref="PjassGate"/>. Returns early
    /// when the game install is absent, matching the other GameData facts, so a machine without
    /// Warcraft III fails nothing while still proving placement everywhere it can.
    /// </summary>
    private static void AssertCompiles(string script)
    {
        if (!Directory.Exists(Install)) return;

        var check = JassScriptCheck.Check(script);
        Assert.True(JassScriptCheck.IsCompilable(check),
            "JassScriptCheck rejected the assembled script: " + string.Join(" | ",
                check.Where(i => i.Severity == DiagnosticSeverity.Error).Select(i => i.Message)));

        var pjass = PjassGate.Check(script);
        Assert.True(pjass.Ran, pjass.Note);
        Assert.True(pjass.Passed,
            pjass.Note + " " + string.Join(" | ", pjass.Errors));
    }

    /// <summary>
    /// The block <see cref="SoundImportCommand"/> really generates, with no repairs. pjass used to
    /// reject it on two generator defects, a parameter named <c>event</c> and an unescaped path
    /// backslash, and both are fixed in the generator now, so this is the exact text a caller
    /// would splice.
    /// </summary>
    private static string DispatcherBlock()
    {
        var clips = Path.Combine(Path.GetTempPath(), "wc3ctl-splice-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(clips);
        File.WriteAllBytes(Path.Combine(clips, "a.wav"), new byte[] { 0 });
        try
        {
            const string json = """
            { "event.one": { "clips": ["a.wav"], "cooldown": 0.06, "maxVoices": 2, "priority": 10 } }
            """;
            var import = SoundImportCommand.Import(
                MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>())), json, clips);
            Assert.True(import.Ok, import.Message);
            Assert.Contains("globals", import.Jass);
            return import.Jass;
        }
        finally
        {
            try { Directory.Delete(clips, recursive: true); } catch { /* best effort */ }
        }
    }

    private static MapDocument MapWithScript(string script) =>
        MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [ScriptName] = Encoding.UTF8.GetBytes(script),
        }));

    private static void AssertAllOriginalFunctionsPresent(string text)
    {
        foreach (var name in OriginalFunctions)
        {
            Assert.Equal(1, CountOf(text, "function " + name + " "));
            Assert.Contains("endfunction", text);
        }
    }

    private static void AssertOnlyCrlf(string text) =>
        Assert.DoesNotContain("\n", text.Replace("\r\n", string.Empty));

    private static int CountLines(string text, string line)
    {
        int count = 0;
        foreach (var candidate in text.Split('\n'))
            if (candidate.TrimEnd('\r').Trim() == line) count++;
        return count;
    }

    private static int CountOf(string haystack, string needle)
    {
        int count = 0;
        for (int at = haystack.IndexOf(needle, StringComparison.Ordinal);
             at >= 0;
             at = haystack.IndexOf(needle, at + needle.Length, StringComparison.Ordinal))
            count++;
        return count;
    }
}
