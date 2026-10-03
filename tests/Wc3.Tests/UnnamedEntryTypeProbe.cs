// tests/Wc3.Tests/UnnamedEntryTypeProbe.cs
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// What is actually inside the entries whose names cannot be recovered.
///
/// NameRecoveryProbe measured that 17 of 37 maps still carry nameless entries after the harvest,
/// and two of them (U9_PumpkinZ_v4.7d, NCD S1 ENGv1b) recover NOTHING despite having a readable
/// multi-megabyte script and parsed object data. The likely reason is an obfuscated script that
/// builds asset paths at runtime instead of holding them as literals, so there is no literal to
/// harvest. Those names are gone and no amount of guessing brings them back.
///
/// But the name is only needed to LOCATE a file in an MPQ. wc3ctl already reads these entries
/// perfectly well by block index. So the question worth asking is not "what were they called" but
/// "what ARE they", which the first few bytes answer directly. If most nameless entries can be
/// typed, then the Files panel can describe them, previews can render them and extraction can name
/// them something sensible, on the 17 maps where today they are simply blank rows.
/// </summary>
public class UnnamedEntryTypeProbe
{
    private readonly ITestOutputHelper _out;
    public UnnamedEntryTypeProbe(ITestOutputHelper output) => _out = output;

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

    /// <summary>
    /// Type from the leading bytes. Only signatures that actually occur in Warcraft III archives,
    /// because a guess that is merely plausible is worse than "unknown".
    /// </summary>
    private static string Sniff(byte[] b)
    {
        if (b.Length < 4) return b.Length == 0 ? "empty" : "tiny";

        bool Is(string tag, int at = 0)
        {
            if (at + tag.Length > b.Length) return false;
            for (int i = 0; i < tag.Length; i++) if (b[at + i] != tag[i]) return false;
            return true;
        }

        if (Is("BLP0") || Is("BLP1") || Is("BLP2")) return "BLP texture";
        if (Is("MDLX")) return "MDX model";
        if (Is("RIFF")) return Is("WAVE", 8) ? "WAV audio" : "RIFF container";
        if (Is("OggS")) return "OGG audio";
        if (Is("ID3")) return "MP3 audio";
        if (b[0] == 0xFF && (b[1] & 0xE0) == 0xE0) return "MP3 audio";
        if (Is("DDS ")) return "DDS texture";
        if (b[0] == 0x89 && Is("PNG", 1)) return "PNG image";
        if (b[0] == 0xFF && b[1] == 0xD8) return "JPEG image";
        if (Is("MPQ")) return "nested MPQ";
        if (Is("HM3W")) return "map header";
        if (Is("\0\0\0")) return "TrueType font";
        if (Is("OTTO")) return "OpenType font";
        if (Is("BM")) return "BMP image";
        if (Is("")) return "SLK or binary table";

        // Text-ish: mostly printable with no NUL in the first block reads as text, the same test
        // FilePreviewCommand uses.
        int n = Math.Min(b.Length, 512), printable = 0, nul = 0;
        for (int i = 0; i < n; i++)
        {
            if (b[i] == 0) nul++;
            else if (b[i] >= 32 && b[i] < 127 || b[i] == 9 || b[i] == 10 || b[i] == 13) printable++;
        }
        if (nul == 0 && printable > n * 0.9) return "text";

        return "unknown";
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Can_the_nameless_entries_be_typed_by_their_contents()
    {
        var overall = new Dictionary<string, int>(StringComparer.Ordinal);
        long overallBytes = 0;
        int mapsWithNameless = 0, totalNameless = 0, totalTyped = 0, harvestFailures = 0;
        int unreadable = 0;

        foreach (var path in Maps())
        {
            if (new FileInfo(path).Length > 300L * 1024 * 1024) continue;

            MapDocument doc;
            try { doc = MapDocument.Load(path); }
            catch { continue; }
            // Reported rather than swallowed, for the reason recorded in NameRecoveryProbe.
            try { doc.HarvestAssetNames(); }
            catch (Exception ex)
            {
                harvestFailures++;
                _out.WriteLine($"{Path.GetFileName(path),-44} harvest THREW "
                             + $"{ex.GetType().Name}: {ex.Message}");
            }

            var nameless = doc.Files.Where(f => f.FileName is null).ToList();
            if (nameless.Count == 0) continue;
            mapsWithNameless++;

            var perMap = new Dictionary<string, int>(StringComparer.Ordinal);
            int typed = 0;
            foreach (var f in nameless)
            {
                // A read failure is counted separately from a genuinely empty entry. Folding
                // the two together is what produced the claim that these archives hold ~50,000
                // empty padding entries, when in fact only a handful are empty and the rest are
                // entries the loader cannot read at all. RawBytes returns an empty array for
                // both, so the distinction has to be made here or not at all.
                byte[] bytes;
                try { bytes = f.RawBytes; }
                catch { bytes = Array.Empty<byte>(); unreadable++; }
                string kind = Sniff(bytes);
                perMap[kind] = perMap.TryGetValue(kind, out int a) ? a + 1 : 1;
                overall[kind] = overall.TryGetValue(kind, out int b) ? b + 1 : 1;
                overallBytes += bytes.Length;
                if (kind is not ("unknown" or "empty" or "tiny")) typed++;
            }

            totalNameless += nameless.Count;
            totalTyped += typed;
            _out.WriteLine($"{Path.GetFileName(path),-44} {nameless.Count,6:N0} nameless, "
                         + $"{typed,6:N0} typed ({(nameless.Count == 0 ? 0 : 100.0 * typed / nameless.Count):F0}%)  "
                         + string.Join(" ", perMap.OrderByDescending(k => k.Value)
                             .Take(4).Select(k => $"{k.Key}={k.Value}")));
        }

        _out.WriteLine($"\n{harvestFailures} map(s) where the harvest threw, "
                     + $"{unreadable:N0} entr(ies) whose bytes could not be read at all");
        _out.WriteLine($"{mapsWithNameless} map(s) carry nameless entries, "
                     + $"{totalNameless:N0} entr(ies) in total, {overallBytes / 1024 / 1024:N0} MB");
        _out.WriteLine($"{totalTyped:N0} of them ({(totalNameless == 0 ? 0 : 100.0 * totalTyped / totalNameless):F1}%) "
                     + "can be typed from their leading bytes");
        _out.WriteLine("\nby type across the library:");
        foreach (var kv in overall.OrderByDescending(k => k.Value))
            _out.WriteLine($"  {kv.Key,-22} {kv.Value,7:N0}");

        // Judged against entries that yielded bytes, not against all of them. A first pass
        // compared with the raw total and concluded sniffing was not worth wiring in, which was
        // an artifact: 100,194 of the 142,051 nameless entries report zero bytes, about 50,000 in
        // each of the two protected maps. Counting those as a failure to identify CONTENT
        // measures the wrong thing, and the wrong verdict would have killed a feature that works.
        //
        // A SECOND correction, because the first was still wrong about why. Zero bytes here does
        // not mean the entry holds nothing. MapFileEntry.RawBytes returns an empty array both for
        // a genuinely zero-length entry and for one the loader could not read, since an unreadable
        // entry never gets a deferred reader and RawSize reports zero for it too. This probe
        // cannot tell those apart, by construction.
        //
        // Measured against the archives directly rather than through MapDocument, only 5 entries
        // in ORDR_S2 and 1 in PumpkinTD are literally zero length. The other ~50,000 per map are
        // entries War3Net cannot read, roughly 41,000 unreadable plus 8,200 offset-encrypted in
        // ORDR, whose block rows carry junk sizes that alias real data regions. Calling them
        // padding, as the first correction did, understates them badly: they are precisely why a
        // from-scratch rebuild of those archives is unsound and why the save path patches the
        // original bytes in place instead. See MpqSalvagePatcher.
        int empties = overall.TryGetValue("empty", out int e) ? e : 0;
        int substantive = totalNameless - empties;
        double rate = substantive == 0 ? 0 : 100.0 * totalTyped / substantive;
        _out.WriteLine($"\nof {substantive:N0} nameless entr(ies) that yielded bytes, "
                     + $"{totalTyped:N0} ({rate:F1}%) are identified. The other {empties:N0} "
                     + "yielded no bytes, which means unreadable far more often than empty.");
        _out.WriteLine(rate > 90
            ? "VERDICT: nearly every nameless entry that holds anything can be described without "
            + "knowing its name, so the Files panel, previews and extraction can stop treating "
            + "these as blank rows."
            : "VERDICT: content sniffing does not identify enough of them to be worth wiring in.");
    }
}
