// tests/Wc3.Tests/HarvestRobustnessTests.cs
using System.Text;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Name recovery must not be taken down by the script it is reading.
///
/// MapDocument.HarvestAssetNames scanned the script with its own regex, new("\"([^\"]*)\""),
/// which had no length bound and allowed line breaks. A real map (U9_PumpkinZ_v4.7d) carries
/// 659 character multi-line UI strings full of colour codes, those became candidate "paths", and
/// War3Net's name hashing threw IndexOutOfRangeException on the first one. The harvest died after
/// six candidates, so that map recovered NO names, and any caller not wrapping the call in a
/// try/catch crashed outright.
///
/// It stayed hidden because the sweep that first measured harvesting did wrap it in a bare
/// try/catch, turning a crash into "recovered 0 names", which reads as a map whose script simply
/// has nothing useful in it. A swallowed exception reported as a measurement is worse than a
/// crash, because it gets written down as a fact.
///
/// AssetPathCandidates already had the correct scanner, bounded to three to 260 characters with
/// no line break, and documented why. Two implementations of one rule, one of them wrong, which
/// is the same shape as the script encoding disagreement fixed earlier.
/// </summary>
public class HarvestRobustnessTests
{
    private readonly ITestOutputHelper _out;
    public HarvestRobustnessTests(ITestOutputHelper output) => _out = output;

    /// <summary>A script holding exactly the shape that broke it: a very long, multi-line,
    /// colour-coded UI string sitting beside a genuine asset path.</summary>
    private static string ScriptWithProse()
    {
        var prose = "[" + new string('X', 200) + " ]\\n|cffD3AA88"
                  + new string('Y', 200) + "|r : [HP0% ]\\n"
                  + new string('Z', 200) + " [|cffffcc00-";
        return "function main takes nothing returns nothing\n"
             + $"    call BJDebugMsg(\"{prose}\")\n"
             + "    call AddSpecialEffect(\"war3mapImported\\\\RealAsset.mdx\", 0., 0.)\n"
             + "endfunction\n";
    }

    private static MapDocument MapWith(string script) =>
        MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = Encoding.Latin1.GetBytes(script),
        }));

    [Fact]
    public void A_long_multiline_string_literal_does_not_crash_the_harvest()
    {
        var doc = MapWith(ScriptWithProse());

        // The assertion is simply that this returns. It used to throw.
        int named = doc.HarvestAssetNames();
        _out.WriteLine($"harvest returned {named} name(s) without throwing");
    }

    [Fact]
    public void Prose_never_becomes_a_candidate_path_in_the_first_place()
    {
        // The real fix is upstream of the crash. A 600 character literal spanning lines is not a
        // path and must not be offered as one, whatever the consumer does with it.
        var candidates = AssetPathCandidates.NamedInScript(ScriptWithProse()).ToList();
        _out.WriteLine($"{candidates.Count} candidate(s): "
                     + string.Join(", ", candidates.Select(c => $"[{c.Length}] {c}")));

        Assert.All(candidates, c => Assert.True(c.Length <= 260,
            $"a {c.Length} character literal was offered as an asset path"));
        Assert.All(candidates, c => Assert.DoesNotContain('\n', c));
        Assert.All(candidates, c => Assert.DoesNotContain('\r', c));
    }

    [Fact]
    public void The_genuine_asset_path_beside_the_prose_still_survives()
    {
        // A filter that fixed the crash by discarding everything would pass the two tests above
        // and destroy the feature.
        var candidates = AssetPathCandidates.NamedInScript(ScriptWithProse()).ToList();
        Assert.Contains(candidates, c => c.Contains("RealAsset.mdx", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void The_map_that_crashed_now_recovers_names()
    {
        string path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Warcraft III", "Maps", "Download", "U9_PumpkinZ_v4.7d.w3x");
        if (!File.Exists(path)) { _out.WriteLine("map absent, skipped"); return; }

        var doc = MapDocument.Load(path);
        int before = doc.Files.Count(f => f.FileName is null);

        int named = doc.HarvestAssetNames();   // this threw IndexOutOfRangeException

        int after = doc.Files.Count(f => f.FileName is null);
        _out.WriteLine($"{doc.Files.Count} entr(ies), {before} unnamed before, "
                     + $"{named} recovered, {after} unnamed after");

        // The point is that it completes. Whether this particular map's obfuscated script yields
        // any names is a separate question, and the count is reported rather than asserted.
        Assert.True(after <= before);
    }
}
