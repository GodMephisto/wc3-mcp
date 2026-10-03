// tests/Wc3.Tests/WctPairingProbe.cs
using War3Net.Build.Script;
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Tests the rule TriggerReadCommand uses to pair war3map.wct code bodies with war3map.wtg
/// trigger definitions. That rule is stated in a comment as "one code body per TriggerDefinition,
/// in wtg document order (GUI triggers get an empty slot)", and the reader implements it as
/// wct.CustomTextTriggers[definitionOrdinal] where definitionOrdinal counts EVERY definition.
///
/// TriggerTreeShapeProbe measured something that rule cannot explain. Anime_WOS2_0.30d has 109
/// slots for 115 definitions and ALL 109 slots are non-empty. If GUI triggers really got an empty
/// slot there would be empty slots and the counts would match. So either the comment is wrong, or
/// the map is unusual.
///
/// If the true rule is "one slot per CUSTOM-TEXT definition", then the reader is off by the number
/// of GUI triggers that precede each custom-text one, and the Studio has been showing the wrong
/// code body for custom-text triggers. That is worth knowing precisely, because the trigger panel
/// is exactly what the current work is meant to make trustworthy.
///
/// The test is decisive rather than suggestive. For each candidate rule, pair the bodies and count
/// how many pairings put a body whose own text names the trigger it belongs to. Custom-text
/// trigger bodies are compiled as function Trig_&lt;SanitizedName&gt;_Actions, so the body names its
/// own trigger, which makes the correct pairing self-evident from the data.
/// </summary>
public class WctPairingProbe
{
    private readonly ITestOutputHelper _out;
    public WctPairingProbe(ITestOutputHelper output) => _out = output;

    public static IEnumerable<object[]> Candidates()
    {
        string dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Warcraft III", "Maps", "Download");
        foreach (var n in new[] { "Anime_WOS2_0.30d.w3x", "GGGA_V0.04g.w3x", "RATankD_516.2.w3x" })
            yield return new object[] { Path.Combine(dir, n) };
    }

    [Theory]
    [MemberData(nameof(Candidates))]
    [Trait("Category", "Corpus")]
    public void Which_pairing_rule_matches_the_data(string path)
    {
        if (!File.Exists(path)) { _out.WriteLine($"{Path.GetFileName(path)} absent, skipped"); return; }
        var doc = MapDocument.Load(path);
        if (doc.GetFile("war3map.wtg")?.Model is not MapTriggers wtg) { _out.WriteLine("no wtg"); return; }
        if (doc.GetFile("war3map.wct")?.Model is not MapCustomTextTriggers wct) { _out.WriteLine("no wct"); return; }

        var defs = wtg.TriggerItems.OfType<TriggerDefinition>().ToList();
        var custom = defs.Where(IsCustomText).ToList();

        _out.WriteLine($"{Path.GetFileName(path)}");
        _out.WriteLine($"  definitions {defs.Count}, custom-text {custom.Count}, "
                     + $"gui {defs.Count - custom.Count}, wct slots {wct.CustomTextTriggers.Count}");

        // Rule A, what the reader does today: index by ordinal over ALL definitions.
        int hitsA = 0, testedA = 0;
        int ordinal = 0;
        foreach (var d in defs)
        {
            if (IsCustomText(d) && ordinal < wct.CustomTextTriggers.Count)
            {
                testedA++;
                if (BodyNames(wct.CustomTextTriggers[ordinal].Code, d.Name)) hitsA++;
            }
            ordinal++;
        }

        // Rule B, one slot per CUSTOM-TEXT definition only.
        int hitsB = 0, testedB = 0;
        for (int i = 0; i < custom.Count && i < wct.CustomTextTriggers.Count; i++)
        {
            testedB++;
            if (BodyNames(wct.CustomTextTriggers[i].Code, custom[i].Name)) hitsB++;
        }

        _out.WriteLine($"  rule A (ordinal over all definitions): {hitsA}/{testedA} bodies name "
                     + "their own trigger");
        _out.WriteLine($"  rule B (ordinal over custom-text only): {hitsB}/{testedB} bodies name "
                     + "their own trigger");

        int empty = wct.CustomTextTriggers.Count(t => string.IsNullOrWhiteSpace(t.Code?.TrimEnd('\0')));
        _out.WriteLine($"  empty slots {empty}, so \"GUI triggers get an empty slot\" is "
                     + (empty >= defs.Count - custom.Count && defs.Count - custom.Count > 0
                        ? "consistent" : "NOT consistent")
                     + $" with {defs.Count - custom.Count} gui trigger(s)");

        _out.WriteLine(hitsB > hitsA
            ? "  VERDICT: rule B fits the data better. The reader is MISPAIRING code bodies."
            : hitsA > hitsB
            ? "  VERDICT: rule A fits, the reader is correct on this map."
            : "  VERDICT: inconclusive on this map (both rules score the same).");

        // Show one concrete mispairing so the verdict is checkable rather than a score.
        if (custom.Count > 1 && wct.CustomTextTriggers.Count > 1)
        {
            var second = custom.Count > 1 ? custom[1] : custom[0];
            int ordA = defs.IndexOf(second);
            _out.WriteLine($"  example: custom-text trigger '{second.Name}' is definition ordinal "
                         + $"{ordA}, custom-text index 1");
            if (ordA < wct.CustomTextTriggers.Count)
                _out.WriteLine($"    rule A gives it: {Head(wct.CustomTextTriggers[ordA].Code)}");
            _out.WriteLine($"    rule B gives it: {Head(wct.CustomTextTriggers[1].Code)}");
        }
    }

    private static bool IsCustomText(TriggerDefinition d) =>
        d.IsCustomTextTrigger || d.Type == TriggerItemType.Script;

    /// <summary>True when the code body mentions the trigger's own name, which the JASS compiler
    /// emits as part of the Trig_&lt;name&gt;_Actions function it generates.</summary>
    private static bool BodyNames(string? code, string? name)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name)) return false;
        string needle = new string(name.Where(char.IsLetterOrDigit).ToArray());
        if (needle.Length < 4) return false;
        string haystack = new string(code.Where(char.IsLetterOrDigit).ToArray());
        return haystack.Contains(needle, StringComparison.OrdinalIgnoreCase);
    }

    private static string Head(string? code)
    {
        if (string.IsNullOrEmpty(code)) return "(empty)";
        string one = code.TrimEnd('\0').Replace("\r", " ").Replace("\n", " ").Trim();
        return one.Length <= 90 ? one : one.Substring(0, 90) + "...";
    }
}
