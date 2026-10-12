// tests/Wc3.Tests/GameBuildNameTests.cs
// Tests for GameBuildName parsing and ordering against real schema folder names.

using Wc3.Commands;
using Gbn = Wc3.Commands.GameBuildName;

namespace Wc3.Tests;

/// <summary>
/// Parses every build folder in data/schemas and asserts the sorted order
/// matches the expected chronological sequence.
/// </summary>
public class GameBuildNameTests
{
    private static readonly string SchemasDir = Path.Combine(
        AppContext.BaseDirectory ?? Directory.GetCurrentDirectory(),
        "data", "schemas");

    [Fact]
    public void ParseAllSchemaFolders_AssertSortOrder()
    {
        // Skip if data/schemas is not present (CI may not have it).
        if (!Directory.Exists(SchemasDir))
            return;

        // Read all schema folder names from index.json.
        var buildNames = ReadBuildNames(SchemasDir);
        Assert.True(buildNames.Count > 0, "expected at least one schema build in data/schemas");

        // Parse and sort.
        var parsed = buildNames.Select(Gbn.Parse).ToList();
        parsed.Sort((a, b) => Gbn.Compare(a, b));

        // Build a lookup for easy assertions.
        Dictionary<string, GameBuildName> byName = parsed
            .ToDictionary(p => p.Raw, StringComparer.Ordinal);

        Assert.True(byName.ContainsKey("ROC-v1.00-ru"),
            "ROC-v1.00-ru not found");
        Assert.True(byName.ContainsKey("TFT-v1.07-ru"),
            "TFT-v1.07-ru not found");

        // Beta-ROC ordering: Beta-ROC-v1.00 should come first among Beta builds.
        int betaRocFirst = parsed.FindIndex(p => p.Raw == "Beta-ROC-v1.00");
        Assert.True(betaRocFirst >= 0, "Beta-ROC-v1.00 not found");

        // Beta-ROC-v1.34 (last Beta-ROC) should be before ROC-v1.00-ru.
        int betaRocLast = parsed.FindLastIndex(p =>
            p.IsBeta && p.Prefix?.ToLowerInvariant() == "roc");
        int roc100 = byName.ContainsKey("ROC-v1.00-ru")
            ? parsed.FindIndex(p => p.Raw == "ROC-v1.00-ru") : -1;
        Assert.True(betaRocLast < roc100,
            "last Beta-ROC should sort before ROC-v1.00-ru");

        // Beta-TFT-v300 should be between ROC-v1.06-ru and TFT-v1.07-ru.
        int betaTft300 = byName.ContainsKey("Beta-TFT-v300")
            ? parsed.FindIndex(p => p.Raw == "Beta-TFT-v300") : -1;
        int roc106 = byName.ContainsKey("ROC-v1.06-ru")
            ? parsed.FindIndex(p => p.Raw == "ROC-v1.06-ru") : -1;
        int tft107 = byName.ContainsKey("TFT-v1.07-ru")
            ? parsed.FindIndex(p => p.Raw == "TFT-v1.07-ru") : -1;
        Assert.True(betaTft300 >= 0, "Beta-TFT-v300 not found");
        Assert.True(betaTft300 > roc106,
            "Beta-TFT-v300 should sort after ROC-v1.06-ru");
        Assert.True(betaTft300 < tft107,
            "Beta-TFT-v300 should sort before TFT-v1.07-ru");

        // Key ordering from the task spec.
        AssertBefore(byName, "TFT-v1.30.4.11274", "TFT-v1.31.0.11889-w3t");
        AssertBefore(byName, "TFT-v1.31.0.11889-w3t", "TFT-v1.31.1.12173");
        AssertBefore(byName, "TFT-v1.31.1.12173", "Reforged-v1.32.0.14411");
        AssertBefore(byName, "Reforged-v1.32.0.14411", "3.0.0.24268");

        // ROC-v1.00-ru before TFT-v1.07-ru.
        AssertBefore(byName, "ROC-v1.00-ru", "TFT-v1.07-ru");

        // Locale variants: same version sorted by locale (English first).
        AssertBefore(byName, "TFT-v1.28.2.7395", "TFT-v1.28.0.7205-zhtw");

        // Letter suffix: TFT-v1.24a-ru should sort after TFT-v1.24.x releases.
        int tft124a = byName.ContainsKey("TFT-v1.24a-ru")
            ? parsed.FindIndex(p => p.Raw == "TFT-v1.24a-ru") : -1;
        int tft124b = byName.ContainsKey("TFT-v1.24b-ru")
            ? parsed.FindIndex(p => p.Raw == "TFT-v1.24b-ru") : -1;
        Assert.True(tft124a < tft124b,
            "TFT-v1.24a-ru should sort before TFT-v1.24b-ru");

        // Parse individual names.
        GameBuildName bn;

        // Bare version.
        bn = Gbn.Parse("1.30.4.11274");
        Assert.Equal("1.30.4.11274", bn.Raw);
        Assert.Equal(1, bn.Version.Major);
        Assert.False(bn.IsTest);
        Assert.False(bn.IsBeta);
        Assert.Null(bn.Locale);
        Assert.Null(bn.Prefix);

        // TFT build with locale.
        bn = Gbn.Parse("TFT-v1.31.1.12173");
        Assert.Equal("TFT-v1.31.1.12173", bn.Raw);
        Assert.Equal("TFT", bn.Prefix);
        Assert.Equal("1.31.1.12173", bn.Version.ToString());
        Assert.False(bn.IsTest);
        Assert.Null(bn.Locale);

        // Test build (w3t).
        bn = Gbn.Parse("TFT-v1.31.0.11889-w3t");
        Assert.True(bn.IsTest);
        Assert.False(bn.IsBeta);
        Assert.Equal("TFT", bn.Prefix);
        Assert.True(bn.Line.Contains("-w3t", StringComparison.Ordinal));

        // Test build (w3b).
        bn = Gbn.Parse("Reforged-v1.32.0.13369-w3b-ab0c8ee");
        Assert.True(bn.IsTest);
        Assert.Equal("Reforged", bn.Prefix);

        // Beta ROC.
        bn = Gbn.Parse("Beta-ROC-v1.00");
        Assert.True(bn.IsBeta);
        Assert.False(bn.IsTest);
        Assert.Equal("ROC", bn.Prefix);

        // Beta TFT.
        bn = Gbn.Parse("Beta-TFT-v300");
        Assert.True(bn.IsBeta);
        Assert.False(bn.IsTest);
        Assert.Equal("TFT", bn.Prefix);

        // Locale variant.
        bn = Gbn.Parse("TFT-v1.28.0.7205-zhtw");
        Assert.Equal("zhtw", bn.Locale);
        Assert.False(bn.IsTest);
        Assert.False(bn.IsBeta);

        // ROC with locale.
        bn = Gbn.Parse("ROC-v1.00-ru");
        Assert.Equal("ru", bn.Locale);

        // Letter suffix version (1.24a).
        bn = Gbn.Parse("TFT-v1.24a-ru");
        Assert.Equal("1.24.0.0", bn.Version.ToString());
        // The letter suffix after the version digits should be captured.
        Assert.Equal("a", bn.LetterSuffix);
    }

    /// <summary>
    /// Verify that comparing Beta-TFT-v300 to nearby builds produces the right order.
    /// The previous fix had bare versions sort above every prefixed name, which
    /// was wrong. This test would go red with that hack restored.
    /// </summary>
    [Fact]
    public void BetaTFTOrdering_CorrectSlot()
    {
        // Beta-TFT-v300 between ROC-v1.06-ru and TFT-v1.07-ru.
        var betaTft = Gbn.Parse("Beta-TFT-v300");
        var roc106 = Gbn.Parse("ROC-v1.06-ru");
        var tft107 = Gbn.Parse("TFT-v1.07-ru");

        Assert.True(Gbn.Compare(betaTft, roc106) > 0,
            "Beta-TFT-v300 should sort after ROC-v1.06-ru");
        Assert.True(Gbn.Compare(betaTft, tft107) < 0,
            "Beta-TFT-v300 should sort before TFT-v1.07-ru");

        // Also verify that Beta-TFT comes before Beta-TFT-v315.
        var betaTftLater = Gbn.Parse("Beta-TFT-v315");
        Assert.True(Gbn.Compare(betaTft, betaTftLater) < 0,
            "Beta-TFT-v300 should sort before Beta-TFT-v315");
    }

    /// <summary>
    /// Verify that letter suffix ordering puts 1.24 before 1.24a, which sorts before 1.24b.
    /// </summary>
    [Fact]
    public void LetterSuffixOrdering_124aBefore124b()
    {
        var bn124 = Gbn.Parse("TFT-v1.24-ru");
        var bn124a = Gbn.Parse("TFT-v1.24a-ru");
        var bn124b = Gbn.Parse("TFT-v1.24b-ru");

        Assert.True(Gbn.Compare(bn124, bn124a) < 0,
            "TFT-v1.24-ru should sort before TFT-v1.24a-ru");
        Assert.True(Gbn.Compare(bn124a, bn124b) < 0,
            "TFT-v1.24a-ru should sort before TFT-v1.24b-ru");
    }

    /// <summary>
    /// Verify that release (bare, no test flag) sorts before test of the same
    /// version. With the fix bare 3.0.0.24268 and Reforged test both get
    /// category 4, same version 3.0.0.24268, but bare.IsTest=false < true.
    /// </summary>
    [Fact]
    public void ReleaseBeforeTest_SameVersion()
    {
        // Compare bare version 3.0.0.24268 vs a test build of similar version.
        var bn = Gbn.Parse("Reforged-v3.0.0.24268-w3-3a9d8f2");
        Assert.True(bn.IsTest);
        Assert.False(bn.IsBeta);

        var bare = Gbn.Parse("3.0.0.24268");
        Assert.False(bare.IsTest);
        Assert.False(bare.IsBeta);

        // With the fix bare 3.0.0.24268 (category 4, IsTest=false) sorts
        // before Reforged test (category 4, IsTest=true) because bare releases
        // come before test builds of the same version.
        Assert.True(Gbn.Compare(bare, bn) < 0,
            "bare version should sort before Reforged test build of same version");
    }

    private static List<string> ReadBuildNames(string schemasDir)
    {
        var indexFile = Path.Combine(schemasDir, "index.json");
        if (!File.Exists(indexFile)) return new();

        var content = File.ReadAllText(indexFile);
        // Simple extraction: "build": "..."
        var names = new List<string>();
        for (int i = 0; i < content.Length; i++)
        {
            if (i + 8 <= content.Length && content.Substring(i, 8) == "\"build\"")
            {
                int q1 = content.IndexOf('"', i + 8);
                int q2 = content.IndexOf('"', q1 + 1);
                if (q2 > q1)
                {
                    names.Add(content.Substring(q1 + 1, q2 - q1 - 1));
                }
            }
        }
        return names;
    }

    private static void AssertBefore(Dictionary<string, GameBuildName> byName, string a, string b)
    {
        Assert.True(byName.ContainsKey(a), a + " not found");
        Assert.True(byName.ContainsKey(b), b + " not found");
        int idxA = byName.Values.ToList().FindIndex(p => p.Raw == a);
        int idxB = byName.Values.ToList().FindIndex(p => p.Raw == b);
        Assert.True(idxA < idxB, $"{a} should sort before {b} (idx {idxA} < {idxB})");
    }

    /// <summary>
    /// Task 13e: bare version sorts between named builds of the same tier.
    /// 1.30.4.11274 (TFT tier) should sort after TFT-v1.30.3.11235 and
    /// before TFT-v1.31.0.11889-w3t.
    /// 3.0.0.24268 (Reforged tier) should sort after Reforged-v2.0.4.23745-w3-9a94ff7.
    /// </summary>
    [Fact]
    public void BareVersionSortsBetweenNamedBuildsOfSameTier()
    {
        var bare130 = Gbn.Parse("1.30.4.11274");
        var bare300 = Gbn.Parse("3.0.0.24268");
        var tft1303 = Gbn.Parse("TFT-v1.30.3.11235");
        var tft1310 = Gbn.Parse("TFT-v1.31.0.11889-w3t");
        var reforged204 = Gbn.Parse("Reforged-v2.0.4.23745-w3-9a94ff7");

        // 1.30.4.11274 after TFT-v1.30.3.11235
        Assert.True(Gbn.Compare(bare130, tft1303) > 0,
            "1.30.4.11274 should sort after TFT-v1.30.3.11235");
        // 1.30.4.11274 before TFT-v1.31.0.11889-w3t
        Assert.True(Gbn.Compare(bare130, tft1310) < 0,
            "1.30.4.11274 should sort before TFT-v1.31.0.11889-w3t");
        // 3.0.0.24268 after Reforged-v2.0.4.23745-w3-9a94ff7
        Assert.True(Gbn.Compare(bare300, reforged204) > 0,
            "3.0.0.24268 should sort after Reforged-v2.0.4.23745-w3-9a94ff7");
    }
}
