// tests/Wc3.Tests/ParseCoverageTests.cs
using Wc3.Model;
namespace Wc3.Tests;

public class ParseCoverageTests
{
    // The real map lives outside the repo; skip cleanly if absent.
    private static readonly string CorpusMap = TestCorpus.Map(@"ggg_en_1.11r_slk.w3x");

    [Fact]
    [Trait("Category", "Corpus")]
    public void Every_known_file_parses_or_has_diagnostic_no_crash()
    {
        if (!File.Exists(CorpusMap)) return; // corpus-optional

        var doc = MapDocument.Load(CorpusMap);

        // Guard against a vacuous pass: a real map must surface known files.
        Assert.NotEmpty(doc.Files.Where(f => f.IsKnown));

        foreach (var f in doc.Files.Where(f => f.IsKnown))
        {
            bool parsed = f.IsParsed;
            bool hasDiag = doc.Diagnostics.Any(d => d.FileName == f.FileName);
            Assert.True(parsed || hasDiag, $"{f.FileName}: neither parsed nor diagnosed");
        }
    }
}
