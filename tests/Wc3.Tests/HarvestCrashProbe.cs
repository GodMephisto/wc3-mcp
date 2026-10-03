// tests/Wc3.Tests/HarvestCrashProbe.cs
using War3Net.IO.Mpq;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// MapDocument.HarvestAssetNames throws IndexOutOfRangeException on U9_PumpkinZ_v4.7d.w3x, from
/// inside War3Net's archive.AddFileNames(candidates).
///
/// It went unnoticed because the sweep that first measured harvesting wrapped the call in a bare
/// try/catch, so a crash was recorded as "recovered 0 names" and reported as a map whose script
/// yields no usable candidates. That is a different and much less alarming conclusion than the
/// truth, which is that name recovery takes the whole caller down on this map.
///
/// This narrows the cause rather than guessing at it, because the fix depends on whether the
/// offending thing is a candidate name this code produced or the archive's own tables.
/// </summary>
public class HarvestCrashProbe
{
    private readonly ITestOutputHelper _out;
    public HarvestCrashProbe(ITestOutputHelper output) => _out = output;

    private static string Path_(string name) => System.IO.Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "Warcraft III", "Maps", "Download", name);

    [Theory]
    [InlineData("U9_PumpkinZ_v4.7d.w3x")]     // throws
    [InlineData("NCD S1 ENGv1b.w3x")]         // control: recovers 0 without throwing
    [Trait("Category", "Corpus")]
    public void Which_candidate_or_which_table_breaks_the_harvest(string name)
    {
        string path = Path_(name);
        if (!File.Exists(path)) { _out.WriteLine($"{name} absent, skipped"); return; }

        _out.WriteLine($"=== {name} ===");

        // The archive's own shape first. A malformed hash table would implicate the map rather
        // than the candidates.
        byte[] bytes = File.ReadAllBytes(path);
        int off = Wc3.Model.MpqHeader.FindArchiveOffset(bytes);
        if (off >= 0)
        {
            uint hashSize = BitConverter.ToUInt32(bytes, off + 0x18);
            uint blockSize = BitConverter.ToUInt32(bytes, off + 0x1C);
            _out.WriteLine($"hash table {hashSize:N0} slot(s), block table {blockSize:N0} entr(ies)"
                         + $", power of two: {(hashSize & (hashSize - 1)) == 0}");
        }

        var doc = MapDocument.Load(path);
        _out.WriteLine($"{doc.Files.Count} entr(ies), "
                     + $"{doc.Files.Count(f => f.FileName is null)} unnamed");

        // Rebuild the candidate set the way HarvestAssetNames does, so it can be bisected.
        var candidates = CandidatesOf(doc).ToList();
        _out.WriteLine($"{candidates.Count:N0} candidate name(s)");
        if (candidates.Count == 0) { _out.WriteLine("no candidates, nothing to add"); return; }

        var odd = candidates.Where(c => c.Length == 0 || c.Length > 260
                                     || c.Any(ch => ch < 32)).Take(5).ToList();
        _out.WriteLine(odd.Count == 0
            ? "no obviously malformed candidate (empty, over 260 chars, or holding a control character)"
            : "malformed candidates: " + string.Join(" | ", odd.Select(c => $"[{c.Length}] {Safe(c)}")));

        // Add them one at a time against a fresh archive, so the exact candidate that throws is
        // named rather than inferred.
        using var stream = new MemoryStream(bytes);
        using var archive = MpqArchive.Open(stream, loadListFile: true);
        archive.AddFileNames(StandardMapFileNames.All);

        int added = 0;
        foreach (var c in candidates)
        {
            try { archive.AddFileNames(new[] { c }); added++; }
            catch (Exception ex)
            {
                _out.WriteLine($"THREW after {added:N0} candidate(s), on [{c.Length}] {Safe(c)}");
                _out.WriteLine($"   {ex.GetType().Name}: {ex.Message}");
                return;
            }
        }
        _out.WriteLine($"all {added:N0} candidate(s) added one at a time WITHOUT throwing, "
                     + "so the fault needs the batch call rather than any single name");

        try
        {
            using var s2 = new MemoryStream(bytes);
            using var a2 = MpqArchive.Open(s2, loadListFile: true);
            a2.AddFileNames(StandardMapFileNames.All);
            a2.AddFileNames(candidates);
            _out.WriteLine("the batch call also succeeded on a fresh archive");
        }
        catch (Exception ex)
        {
            _out.WriteLine($"the BATCH call threw: {ex.GetType().Name}: {ex.Message}");
        }
    }

    /// <summary>Mirrors HarvestAssetNames's candidate gathering closely enough to reproduce it.</summary>
    private static IEnumerable<string> CandidatesOf(MapDocument doc)
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var script = doc.GetFile("war3map.j") ?? doc.GetFile(@"scripts\war3map.j")
                  ?? doc.GetFile("war3map.lua") ?? doc.GetFile(@"scripts\war3map.lua");
        if (script is { RawSize: > 0 })
        {
            string text = ScriptText.GetString(script.RawBytes);
            foreach (System.Text.RegularExpressions.Match m in
                     System.Text.RegularExpressions.Regex.Matches(text, "\"([^\"]*)\""))
            {
                var raw = m.Groups[1].Value;
                if (!AssetPathCandidates.LooksLikeAssetPath(raw)) continue;
                foreach (var c in AssetPathCandidates.Expand(AssetPathCandidates.Unescape(raw)))
                    set.Add(c);
            }
        }
        return set;
    }

    private static string Safe(string s) =>
        new(s.Take(90).Select(c => c >= 32 && c < 127 ? c : '.').ToArray());
}
