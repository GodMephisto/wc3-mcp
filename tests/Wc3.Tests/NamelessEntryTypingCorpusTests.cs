// tests/Wc3.Tests/NamelessEntryTypingCorpusTests.cs
using System.Diagnostics;
using Wc3.Model;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// The production sniffer against the whole map library, the enforcing counterpart of the
/// UnnamedEntryTypeProbe census. That probe measured (with a throwaway sniffer and full
/// decompression) that about 99 percent of the non-empty nameless entries are identifiable
/// from their leading bytes, which is what justified wiring content typing into the Files
/// panel and the ls verb. This test keeps that true, a change that drops the production
/// sniffer's identification rate below 90 percent fails loudly instead of silently turning
/// typed rows back into blanks. It also pins the cost contract, typing must not materialize
/// a single nameless entry, prefix reads only.
/// </summary>
public class NamelessEntryTypingCorpusTests
{
    private readonly ITestOutputHelper _out;
    public NamelessEntryTypingCorpusTests(ITestOutputHelper output) => _out = output;

    private static IEnumerable<string> Maps()
    {
        string dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Warcraft III", "Maps", "Download");
        if (!Directory.Exists(dir)) yield break;
        foreach (var p in Directory.EnumerateFiles(dir, "*.w3?")
                     .OrderBy(p => new FileInfo(p).Length))
            yield return p;
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void The_sniffer_identifies_over_ninety_percent_of_nonempty_nameless_entries()
    {
        var census = new Dictionary<string, int>(StringComparer.Ordinal);
        int nameless = 0, empty = 0, identified = 0, materialized = 0, unreadable = 0;
        var clock = Stopwatch.StartNew();

        foreach (var path in Maps())
        {
            if (new FileInfo(path).Length > 300L * 1024 * 1024) continue;

            MapDocument doc;
            try { doc = MapDocument.Load(path); }
            catch { continue; }
            // The probe's census counted post-harvest nameless entries, so this does too.
            try { doc.HarvestAssetNames(); } catch { }

            foreach (var f in doc.Files)
            {
                if (f.FileName is not null) continue;
                nameless++;
                var t = ContentTypeSniffer.Sniff(f);
                census[t.DisplayName] = census.TryGetValue(t.DisplayName, out int n) ? n + 1 : 1;
                if (t.IsEmpty) empty++;
                else if (ReferenceEquals(t, SniffedContentType.Unreadable)) unreadable++;
                else if (t.IsIdentified) identified++;
                if (f.IsMaterialized) materialized++;
            }
        }

        if (nameless == 0) { _out.WriteLine("no maps with nameless entries on this machine, skipped"); return; }

        _out.WriteLine($"{nameless:N0} nameless entr(ies) sniffed in {clock.ElapsedMilliseconds:N0}ms");
        foreach (var kv in census.OrderByDescending(k => k.Value))
            _out.WriteLine($"  {kv.Key,-22} {kv.Value,7:N0}");

        // Entries whose bytes could not be read are excluded from the denominator alongside the
        // empty ones. Failing to identify content that was never available is not a failure of
        // the sniffer, and counting it as one would make the rate a measure of how protected the
        // library is rather than of how well the signatures work.
        int substantive = nameless - empty - unreadable;
        double rate = substantive == 0 ? 0 : 100.0 * identified / substantive;
        _out.WriteLine($"\n{identified:N0} of {substantive:N0} non-empty ({rate:F1}%) identified, "
                     + $"{empty:N0} empty, {unreadable:N0} unreadable");

        Assert.Equal(0, materialized);
        Assert.True(rate > 90,
            $"the sniffer identified only {rate:F1}% of non-empty nameless entries, "
            + "the probe measured about 99% so something regressed");
    }
}
