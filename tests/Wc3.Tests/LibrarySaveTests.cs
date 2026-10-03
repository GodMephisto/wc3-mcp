// tests/Wc3.Tests/LibrarySaveTests.cs
using Wc3.Commands;
using Wc3.Model;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Whether the maps in the user's own Maps folder can actually be saved, byte-faithfully.
///
/// Nothing had ever swept the library for this, and 7 of 34 could not be saved at all, which for a
/// tool whose whole promise is byte-faithful editing is the most serious failure there is. Three
/// distinct causes, one of them ours.
/// </summary>
public class LibrarySaveTests
{
    private readonly ITestOutputHelper _out;
    public LibrarySaveTests(ITestOutputHelper output) => _out = output;

    private static string Dir => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "Warcraft III", "Maps", "Download");

    /// <summary>
    /// The four that failed with "Unable to re-encode the mpq file, because its stream cannot be
    /// read". Their archives use a block-size shift of 13 or 14, meaning 4 MB and 8 MB sectors,
    /// while the rebuild was creating a 4 KB one. That forced every file to be re-encoded, and the
    /// protected ones, whose encryption keys are unrecoverable, could not be.
    /// </summary>
    [Theory]
    [InlineData("Angel-samurai-Z-v332A.w3x")]
    [InlineData("Angel-samurai-Z-v338.w3x")]
    [InlineData("Angel_samurai_338A_Optimize.w3x")]
    [InlineData("LODSuperM2.8B1.w3x")]
    [Trait("Category", "Corpus")]
    public void A_map_with_an_unusual_sector_size_saves_byte_faithfully(string mapName)
    {
        var path = Path.Combine(Dir, mapName);
        if (!File.Exists(path)) { _out.WriteLine($"{mapName} absent, skipped"); return; }

        var doc = MapDocument.Load(path);
        var result = RoundtripCommand.Execute(doc);

        _out.WriteLine($"{mapName}: {doc.Files.Count:N0} entries, "
                     + $"{result.Mismatches.Count} mismatch(es)");
        foreach (var m in result.Mismatches.Take(5)) _out.WriteLine($"   {m}");

        Assert.True(result.Faithful,
            $"{mapName} does not round-trip: {string.Join(", ", result.Mismatches.Take(5))}");
    }

    /// <summary>
    /// The one whose save was never broken. It holds 14 duplicated internal names, and the
    /// COMPARISON threw on them, so a map that writes all 100,788,325 of its bytes correctly was
    /// reported as a round-trip failure.
    /// </summary>
    [Fact]
    [Trait("Category", "Corpus")]
    public void A_map_with_duplicated_internal_names_compares_rather_than_throwing()
    {
        var path = Path.Combine(Dir, "NCD S1 ENGv1b.w3x");
        if (!File.Exists(path)) { _out.WriteLine("absent, skipped"); return; }

        var doc = MapDocument.Load(path);
        var dupes = doc.Files.Where(f => f.FileName is not null)
            .GroupBy(f => f.FileName!, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1).ToList();
        _out.WriteLine($"{dupes.Count} duplicated name(s), e.g. "
                     + string.Join(", ", dupes.Take(3).Select(g => $"{g.Key} x{g.Count()}")));
        Assert.NotEmpty(dupes);   // the whole point of this map as a fixture

        var result = RoundtripCommand.Execute(doc);   // must not throw
        _out.WriteLine($"round-trip ok={result.Faithful}, {result.Mismatches.Count} mismatch(es)");
        Assert.True(result.Faithful,
            $"mismatches: {string.Join(", ", result.Mismatches.Take(5))}");
    }

    /// <summary>
    /// The two whose archive cannot be rebuilt, 22 and 8 of their 65,534 live entries carry
    /// block rows protection has corrupted, and MpqArchiveBuilder opens every entry eagerly.
    /// These used to be pinned as a refusal that explains itself. They now save through the
    /// in-place salvage patch instead, and what is pinned here is the strongest form of that
    /// claim, an UNMODIFIED save reproduces the input byte for byte, and the save says which
    /// path it took. The edit-and-reload fidelity lives in ProtectedMapSalvageSaveTests.
    /// </summary>
    [Theory]
    [InlineData("ORDR_S2_2.305[R]_english.w3x")]
    [InlineData("PumpkinTD_v2.3b.w3x")]
    [Trait("Category", "Corpus")]
    public void A_map_that_cannot_be_rebuilt_saves_via_the_salvage_patch(string mapName)
    {
        var path = Path.Combine(Dir, mapName);
        if (!File.Exists(path)) { _out.WriteLine($"{mapName} absent, skipped"); return; }

        var doc = MapDocument.Load(path);
        // Reading it still works. That is worth pinning too, because "salvage save" must not
        // quietly become "cannot open".
        Assert.NotEmpty(doc.Files);
        Assert.NotNull(doc.GetFile("war3map.w3i"));

        var original = File.ReadAllBytes(path);
        var saved = doc.SaveToBytes();
        _out.WriteLine($"{mapName}: {original.Length:N0} bytes in, {saved.Length:N0} out");

        Assert.True(saved.AsSpan().SequenceEqual(original),
            "an unmodified salvage save must reproduce the input byte for byte");

        var notice = doc.Diagnostics.FirstOrDefault(
            d => d.FileName == "(archive)" && d.Severity == DiagnosticSeverity.Info);
        Assert.NotNull(notice);   // the save must say it took the salvage path
        _out.WriteLine(notice!.Message);
        Assert.Contains("patch", notice.Message);
    }
}
