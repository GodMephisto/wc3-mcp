// tests/Wc3.Tests/NameRecoveryProbe.cs
using War3Net.IO.Mpq;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// How many archive entries stay nameless, and how many more could be named.
///
/// An MPQ locates a file by the hash of its name and stores no name anywhere, so a stripped
/// (listfile) leaves entries that can be read only if their name can be guessed. Every capability
/// in this tool that needs a named file (object data, previews, imports, porting) is bounded by
/// this one number, and 25 of 37 maps are reported to have no readable object data.
///
/// MapDocument.HarvestAssetNames currently guesses from two sources, string literals in the map
/// script and path-like values in object data. It does NOT read war3map.imp, which is literally a
/// list of the paths of every imported file, and imported files are exactly the entries that end
/// up nameless. This measures what that omission costs.
/// </summary>
public class NameRecoveryProbe
{
    private readonly ITestOutputHelper _out;
    public NameRecoveryProbe(ITestOutputHelper output) => _out = output;

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
    public void What_would_reading_the_import_list_recover()
    {
        _out.WriteLine($"{"map",-44} {"entries",8} {"unnamed",8} {"harvest",8} "
                     + $"{"still",7} {"imp",6} {"+imp",6}");
        _out.WriteLine(new string('-', 96));

        int mapsHelped = 0, totalRecovered = 0, mapsSeen = 0, harvestFailures = 0;

        foreach (var path in Maps())
        {
            if (new FileInfo(path).Length > 300L * 1024 * 1024) continue;

            MapDocument doc;
            try { doc = MapDocument.Load(path); }
            catch { continue; }
            mapsSeen++;

            int total = doc.Files.Count;
            int unnamedBefore = doc.Files.Count(f => f.FileName is null);

            // Counted, not swallowed. This exact line, as a bare catch, recorded an
            // IndexOutOfRangeException from HarvestAssetNames as "recovered 0 names", and that
            // zero was reported as a fact about the map's script rather than a crash in the
            // scanner. A sweep that hides the failure it exists to find is worse than no sweep.
            int harvested = 0;
            string? harvestError = null;
            try { harvested = doc.HarvestAssetNames(); }
            catch (Exception ex) { harvestError = $"{ex.GetType().Name}: {ex.Message}"; }
            if (harvestError is not null) harvestFailures++;
            int unnamedAfter = doc.Files.Count(f => f.FileName is null);

            // What the import list offers, spelled the way the archive would store it.
            var impPaths = ImportPaths(doc).ToList();

            // How many of the STILL nameless entries those paths would resolve. Measured by
            // handing the candidates to a fresh archive and counting newly named blocks, which is
            // exactly what HarvestAssetNames does with its own candidates.
            int fromImp = 0;
            if (impPaths.Count > 0 && unnamedAfter > 0)
            {
                try { fromImp = CountNewlyNamed(path, doc, impPaths); } catch { }
            }

            if (fromImp > 0) { mapsHelped++; totalRecovered += fromImp; }

            _out.WriteLine($"{Trim(path),-44} {total,8:N0} {unnamedBefore,8:N0} "
                         + $"{(harvestError is null ? harvested.ToString("N0") : "THREW"),8} "
                         + $"{unnamedAfter,7:N0} {impPaths.Count,6:N0} {fromImp,6:N0}"
                         + (harvestError is null ? "" : "  " + harvestError));
        }

        _out.WriteLine($"\n{mapsSeen} map(s) examined, "
                     + $"{harvestFailures} where the harvest THREW rather than returning");
        Assert.True(harvestFailures == 0,
            $"name recovery threw on {harvestFailures} map(s); a crash here reads as "
            + "\"recovered 0 names\" to every caller that guards it");
        _out.WriteLine($"{mapsHelped} map(s) would gain names from war3map.imp, "
                     + $"{totalRecovered:N0} entr(ies) in total");
        _out.WriteLine(totalRecovered == 0
            ? "VERDICT: the import list adds nothing the existing two sources do not already find."
            : "VERDICT: the import list is an unused source worth wiring into HarvestAssetNames.");
    }

    /// <summary>
    /// Every spelling of every path in war3map.imp that the archive might store it under.
    ///
    /// The format is a uint32 version, a uint32 count, then per entry a FLAG BYTE followed by a
    /// NUL-terminated path. Flag 8 means the path needs the war3mapImported\ prefix. Both
    /// separators and both prefix states are tried, because real maps disagree about all of it.
    /// </summary>
    private static IEnumerable<string> ImportPaths(MapDocument doc)
    {
        var entry = doc.GetFile("war3map.imp");
        if (entry?.Model is not War3Net.Build.Import.ImportedFiles imports) yield break;

        foreach (var f in imports.Files)
        {
            string p = f.FullPath ?? string.Empty;
            if (p.Length == 0) continue;
            foreach (var spelling in Spellings(p)) yield return spelling;
        }
    }

    private static IEnumerable<string> Spellings(string path)
    {
        string back = path.Replace('/', '\\');
        string fwd = path.Replace('\\', '/');
        foreach (var v in new[] { back, fwd })
        {
            yield return v;
            yield return "war3mapImported\\" + v.TrimStart('\\', '/');
            yield return "war3mapimported\\" + v.TrimStart('\\', '/');
        }
        // And with any leading prefix removed, for a map that stored the bare name.
        int cut = back.LastIndexOf('\\');
        if (cut >= 0 && cut + 1 < back.Length) yield return back[(cut + 1)..];
    }

    /// <summary>Opens the archive again with the extra candidates and counts blocks that gain a
    /// name they did not already have.</summary>
    private static int CountNewlyNamed(string path, MapDocument doc, IEnumerable<string> candidates)
    {
        var alreadyNamed = doc.Files
            .Where(f => f.FileName is not null)
            .Select(f => f.BlockIndex)
            .ToHashSet();

        using var fs = File.OpenRead(path);
        using var archive = MpqArchive.Open(fs, loadListFile: true);
        archive.AddFileNames(StandardMapFileNames.All);
        archive.AddFileNames(candidates);

        int i = 0, gained = 0;
        foreach (var e in archive)
        {
            if (e.FileName is not null && !alreadyNamed.Contains(i)) gained++;
            i++;
        }
        return gained;
    }

    private static string Trim(string p)
    {
        string n = Path.GetFileName(p);
        return n.Length <= 42 ? n : n[..42];
    }

    [Theory]
    [InlineData("U9_PumpkinZ_v4.7d.w3x")]
    [InlineData("NCD S1 ENGv1b.w3x")]
    [InlineData("Angel-samurai-Z-v338.w3x")]   // the control: harvest works here
    [Trait("Category", "Corpus")]
    public void Why_does_the_harvest_recover_nothing_on_this_map(string name)
    {
        string path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Warcraft III", "Maps", "Download", name);
        if (!File.Exists(path)) { _out.WriteLine($"{name} absent, skipped"); return; }

        var doc = MapDocument.Load(path);
        _out.WriteLine($"=== {name} ===");
        _out.WriteLine($"{doc.Files.Count:N0} entr(ies), "
                     + $"{doc.Files.Count(f => f.FileName is null):N0} unnamed");

        _out.WriteLine("named entries:");
        foreach (var f in doc.Files.Where(f => f.FileName is not null)
                     .OrderBy(f => f.FileName, StringComparer.OrdinalIgnoreCase))
            _out.WriteLine($"   {f.FileName,-40} {f.RawSize,10:N0} bytes"
                         + (f.Model is null ? "" : $"  parsed as {f.Model.GetType().Name}"));

        // Source 1 of the harvest is string literals in the map script. If the script is missing,
        // unreadable or empty, that source contributes nothing and the whole harvest can only
        // fall back on object data.
        var script = doc.GetFile("war3map.j") ?? doc.GetFile(@"scripts\war3map.j")
                  ?? doc.GetFile("war3map.lua") ?? doc.GetFile(@"scripts\war3map.lua");
        _out.WriteLine(script is null
            ? "script: NOT FOUND under any of the four spellings, so harvest source 1 is dead"
            : $"script: {script.FileName}, {script.RawSize:N0} bytes");

        // Source 2 is path-like values in object data.
        var objectFiles = doc.Files.Where(f => f.Model is not null
            && f.Model.GetType().Name.EndsWith("ObjectData", StringComparison.Ordinal)).ToList();
        _out.WriteLine($"object data files parsed: {objectFiles.Count}"
                     + (objectFiles.Count == 0 ? ", so harvest source 2 is dead too" : ""));

        int harvested = doc.HarvestAssetNames();
        _out.WriteLine($"harvest recovered {harvested:N0} name(s)");
    }
}
