// tests/Wc3.Tests/ProtectedMapNameRecoveryTests.cs
// Measures the recovery gap between wc3ctl's current standard-name probe and a dedicated
// extractor for a protected MPQ archive (its (listfile) stripped or curated down to a
// handful of decoy names). An MPQ never stores a file's real name, only a hash of it, so
// no tool can invert the hash, every recovery method (a known-name probe, a cross-map name
// dictionary, scraping names out of readable content, brute force) works by GUESSING a
// candidate and testing whether its hash already matches an entry in the archive.
//
// This does not touch MapDocument.cs or the standard-name list it already probes (owned by
// another agent's in-flight work on the content-scrape half of this same problem), it reads
// that list and measures one more technique against it, a name dictionary harvested from
// the user's own already-healthy maps (listfile intact) and tested against the protected
// ones. Imported assets circulate widely across maps in this genre, so a name recovered
// from one map's own listfile is a real candidate for another map that hid its own.
//
// Needs the real map library, so Category=Corpus, skips silently when absent. Writes a
// report next to the maps and, if the dictionary earns its keep, the harvested name list
// itself as a plain text artifact for another agent to wire in.
using System.Text;
using War3Net.IO.Mpq;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

public class ProtectedMapNameRecoveryTests
{
    private static readonly string MapDir = TestCorpus.Directory;

    private readonly ITestOutputHelper _out;
    public ProtectedMapNameRecoveryTests(ITestOutputHelper output) => _out = output;

    private sealed record MapScan(string Path, int Total, int NamedByOwnListfile, IReadOnlyList<string> OwnNames);

    [Fact]
    [Trait("Category", "Corpus")]
    public void Measure_standard_probe_and_cross_map_dictionary_recovery()
    {
        if (!Directory.Exists(MapDir)) return;

        var files = AllMapFiles(MapDir);
        if (files.Count == 0) return;

        var scans = new List<MapScan>();
        var loadErrors = new List<(string Name, string Message)>();
        foreach (var path in files)
        {
            try { scans.Add(ScanOwnListfile(path)); }
            catch (Exception ex) { loadErrors.Add((Path.GetFileName(path), ex.Message)); }
        }

        // A map counts as healthy, and so a dictionary source, only when its OWN listfile
        // named every single entry with zero help from anything wc3ctl adds. Anything short
        // of that is a measurement target instead, whether it is missing one file or all of
        // them, so a map is never counted as both.
        var healthy = scans.Where(s => s.Total > 0 && s.NamedByOwnListfile == s.Total).ToList();
        var protectedMaps = scans.Where(s => s.Total > 0 && s.NamedByOwnListfile < s.Total).ToList();

        var dictionary = healthy.SelectMany(s => s.OwnNames)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var report = new StringBuilder();
        report.AppendLine("# MPQ name recovery measurement");
        report.AppendLine();
        report.AppendLine($"map files scanned = {files.Count}, load errors = {loadErrors.Count}, "
            + $"healthy (own listfile complete) = {healthy.Count}, protected (own listfile incomplete) = {protectedMaps.Count}");
        report.AppendLine($"dictionary harvested from the {healthy.Count} healthy maps, "
            + $"{dictionary.Count} distinct names");
        report.AppendLine();

        long totalEntries = 0, namedStandard = 0, namedDict = 0;
        var sigCounts = new Dictionary<string, int>(StringComparer.Ordinal);

        report.AppendLine("# Per protected map, worst gap first");
        report.AppendLine();
        foreach (var s in protectedMaps.OrderBy(s => (double)s.NamedByOwnListfile / s.Total))
        {
            var (standardNamed, dictNamed, stillUnnamedSample) = ProbeGains(s.Path, s.Total, dictionary);
            totalEntries += s.Total; namedStandard += standardNamed; namedDict += dictNamed;

            string name = Path.GetFileName(s.Path);
            report.AppendLine($"## {name}");
            report.AppendLine($"  total entries {s.Total}");
            report.AppendLine($"  named by its own listfile alone {s.NamedByOwnListfile} "
                + $"({Pct(s.NamedByOwnListfile, s.Total)})");
            report.AppendLine($"  named after the standard-name probe (current production) {standardNamed} "
                + $"({Pct(standardNamed, s.Total)}), gain over own listfile +{standardNamed - s.NamedByOwnListfile}");
            report.AppendLine($"  named after standard probe + cross-map dictionary {dictNamed} "
                + $"({Pct(dictNamed, s.Total)}), dictionary's own incremental gain +{dictNamed - standardNamed}");
            report.AppendLine($"  still unnamed after both {s.Total - dictNamed} ({Pct(s.Total - dictNamed, s.Total)})");
            report.AppendLine();

            foreach (var sig in stillUnnamedSample)
                sigCounts[sig] = sigCounts.GetValueOrDefault(sig) + 1;
        }

        report.AppendLine("# Totals across all protected maps");
        report.AppendLine($"  total entries {totalEntries}");
        report.AppendLine($"  named after standard probe alone {namedStandard} ({Pct(namedStandard, totalEntries)})");
        report.AppendLine($"  named after standard probe + dictionary {namedDict} ({Pct(namedDict, totalEntries)})");
        report.AppendLine($"  dictionary's own incremental gain {namedDict - namedStandard} entries "
            + $"({PctOfGap(namedDict - namedStandard, totalEntries - namedStandard)} of the post-standard gap)");
        report.AppendLine($"  still unnamed after both {totalEntries - namedDict} ({Pct(totalEntries - namedDict, totalEntries)})");
        report.AppendLine();

        report.AppendLine("# Content signature of entries still unnamed after standard probe + dictionary");
        report.AppendLine("(first bytes of each remaining unnamed entry, a best-effort characterisation, not an identification)");
        foreach (var kv in sigCounts.OrderByDescending(kv => kv.Value))
            report.AppendLine($"  {kv.Key}  {kv.Value}");
        report.AppendLine();

        if (loadErrors.Count > 0)
        {
            report.AppendLine("# Map files that failed to open");
            foreach (var (n, m) in loadErrors) report.AppendLine($"  {n}  {m}");
            report.AppendLine();
        }

        string outPath = Path.Combine(Path.GetTempPath(), "wc3ctl-mpq-name-recovery.md");
        File.WriteAllText(outPath, report.ToString());
        _out.WriteLine($"report: {outPath}");
        _out.WriteLine($"totals: entries={totalEntries} standard={namedStandard} dict={namedDict}");

        // A data artifact, not a code change, so it does not touch anything under
        // src/Wc3.MapDocument. Written unconditionally so the file this test's own report
        // describes always exists next to it; whether it is worth wiring in is the
        // measurement's job, not this test's.
        string dictPath = Path.Combine(Path.GetTempPath(), "wc3ctl-cross-map-dictionary.txt");
        File.WriteAllLines(dictPath, dictionary);
        _out.WriteLine($"dictionary: {dictPath}");

        // Reporting is the deliverable, nothing here is a pass/fail condition over the
        // user's map library.
        Assert.True(true);
    }

    /// <summary>Opens the archive with no help at all (no standard-name probe, no dictionary),
    /// the rawest possible reading, so "healthy" reflects only what the map's own listfile
    /// names, never anything wc3ctl itself already contributes.</summary>
    private static MapScan ScanOwnListfile(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        using var stream = new MemoryStream(bytes);
        using var archive = MpqArchive.Open(stream, loadListFile: true);
        int total = 0, named = 0;
        var names = new List<string>();
        foreach (var entry in archive)
        {
            total++;
            if (entry.FileName is { } n) { named++; names.Add(n); }
        }
        return new MapScan(path, total, named, names);
    }

    /// <summary>Two fresh opens of the same bytes, one with the standard-name probe alone
    /// (current production behaviour, see MapDocument.Load), one with the standard names
    /// plus the cross-map dictionary. Separate archive instances so neither pass can leak
    /// state into the other. Also samples a content signature off whatever is still unnamed
    /// after both, capped, these maps run into the thousands of imported assets and only a
    /// characterisation is wanted, not a byte-for-byte census.</summary>
    private static (int StandardNamed, int DictNamed, List<string> StillUnnamedSignatures) ProbeGains(
        string path, int total, IReadOnlyList<string> dictionary)
    {
        byte[] bytes = File.ReadAllBytes(path);

        int standardNamed;
        using (var stream = new MemoryStream(bytes))
        using (var archive = MpqArchive.Open(stream, loadListFile: true))
        {
            archive.AddFileNames(StandardMapFileNames.All);
            standardNamed = archive.Count(e => e.FileName is not null);
        }

        int dictNamed;
        var signatures = new List<string>();
        using (var stream = new MemoryStream(bytes))
        using (var archive = MpqArchive.Open(stream, loadListFile: true))
        {
            archive.AddFileNames(StandardMapFileNames.All);
            archive.AddFileNames(dictionary);
            dictNamed = 0;
            const int sigCap = 40; // per map, plenty for a genre-level characterisation
            foreach (var entry in archive)
            {
                if (entry.FileName is not null) { dictNamed++; continue; }
                if (signatures.Count >= sigCap) continue;
                signatures.Add(Sniff(archive, entry));
            }
        }

        return (standardNamed, dictNamed, signatures);
    }

    /// <summary>Best-effort content bucket from an unnamed entry's own bytes, since its name
    /// is not recoverable this way but its type often still is. Not exhaustive, unmatched
    /// bytes are reported as such rather than guessed at.</summary>
    private static string Sniff(MpqArchive archive, MpqEntry entry)
    {
        byte[] head;
        try
        {
            using var fs = archive.OpenFile(entry);
            using var ms = new MemoryStream();
            fs.CopyTo(ms, 512);
            head = ms.ToArray();
        }
        catch (Exception ex)
        {
            return $"unreadable ({ex.GetType().Name})";
        }

        if (head.Length == 0) return "empty";
        if (StartsWith(head, "MDLX")) return "model (mdx)";
        if (StartsWith(head, "BLP1") || StartsWith(head, "BLP2")) return "texture (blp)";
        if (StartsWith(head, "RIFF")) return "audio (riff/wav)";
        if (StartsWith(head, "MPQ\x1A") || StartsWith(head, "MPQ\r")) return "nested mpq";
        if (head.Length >= 2 && head[0] == 0x00 && head[1] == 0x00) return "possible tga (unconfirmed, no header magic)";
        if (LooksLikeText(head)) return "plausible text";
        return "unknown/binary";
    }

    private static bool StartsWith(byte[] head, string magic)
    {
        if (head.Length < magic.Length) return false;
        for (int i = 0; i < magic.Length; i++)
            if (head[i] != (byte)magic[i]) return false;
        return true;
    }

    private static bool LooksLikeText(byte[] head)
    {
        int sample = Math.Min(head.Length, 64);
        int printable = 0;
        for (int i = 0; i < sample; i++)
        {
            byte b = head[i];
            if (b == '\t' || b == '\r' || b == '\n' || (b >= 0x20 && b < 0x7F)) printable++;
        }
        return sample > 0 && printable >= sample * 9 / 10;
    }

    private static string Pct(long part, long whole) =>
        whole == 0 ? "n/a" : $"{100.0 * part / whole:F1}%";

    private static string PctOfGap(long gain, long gap) =>
        gap <= 0 ? "n/a" : $"{100.0 * gain / gap:F1}%";

    /// <summary>Every .w3x/.w3m under the library, at the file level, not deduplicated to one
    /// per title. This measurement is about individual archives, a version-2 copy of a map
    /// is its own protected-or-not archive regardless of what its title shares with another
    /// file. The same junk filters as the port triage sweep (ladder melee, our own scratch
    /// and repaired outputs) still apply, those are never real inputs to either sweep.</summary>
    private static List<string> AllMapFiles(string dir)
    {
        var result = new List<string>();
        foreach (var path in Directory.EnumerateFiles(dir, "*.w3?", SearchOption.AllDirectories))
        {
            var ext = Path.GetExtension(path);
            if (!ext.Equals(".w3x", StringComparison.OrdinalIgnoreCase)
                && !ext.Equals(".w3m", StringComparison.OrdinalIgnoreCase)) continue;

            string file = Path.GetFileNameWithoutExtension(path);
            if (file.StartsWith("(", StringComparison.Ordinal)) continue;
            if (file.Contains("NewMap", StringComparison.OrdinalIgnoreCase)) continue;
            if (file.EndsWith(".ported", StringComparison.OrdinalIgnoreCase)
                || file.EndsWith(".edited", StringComparison.OrdinalIgnoreCase)
                || file.EndsWith(".repaired", StringComparison.OrdinalIgnoreCase)) continue;

            result.Add(path);
        }
        // Bounded for the same reason as the port triage sweep, and CorpusSweep.Describe reports
        // what was left out so a sampled run never reads as a complete one. See CorpusSweep.
        return CorpusSweep.Bound(result);
    }
}
