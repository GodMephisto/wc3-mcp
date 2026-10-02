// tests/Wc3.Tests/ProtectedMapNameLookupTests.cs
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Pins the difference between a listfile lookup and an MPQ hash lookup on a real protected
/// map, because that difference silently cost an entire audit check its population.
///
/// A protected map has its <c>(listfile)</c> stripped. Measured on this map, 793 of its 851
/// entries carry no <see cref="MapFileEntry.FileName"/> at all, so <see cref="MapDocument.GetFile"/>
/// returns null for files the archive plainly holds. That is not an error and reads exactly
/// like absence, which is how portrait-risk reported 3 affected units when the real number was
/// 178. MPQ addresses a file by the HASH of its name, so a name lookup works with no listfile.
///
/// A hermetic version of this test is not possible, because the defect only exists when the
/// listfile is missing and a synthetic map is built with one.
/// </summary>
public class ProtectedMapNameLookupTests
{
    private static readonly string MapPath =
        TestCorpus.Map(@"BVO16g.w3x");

    [Fact]
    [Trait("Category", "Corpus")]
    public void Hash_lookup_reaches_imported_models_the_listfile_lookup_cannot()
    {
        if (!File.Exists(MapPath)) return;
        var doc = MapDocument.Load(MapPath);

        // The control. Most entries really are unnamed, so the premise of this test holds
        // rather than being assumed.
        int unnamed = doc.Files.Count(f => f.FileName is null);
        Assert.True(unnamed > doc.Files.Count / 2,
            $"expected a protected map, but only {unnamed} of {doc.Files.Count} entries are unnamed");

        // Both halves on one real file. GetFile must miss it and the hash lookup must find it.
        //
        // The .mdx spelling, deliberately. Units in this map declare "Ulquiorra.mdl" while the
        // archive stores "Ulquiorra.mdx", which is why the audit probes the declared path and
        // then both extensions. A test written against the declared spelling fails on a premise
        // rather than on the behaviour it is checking, which is how this one first failed.
        const string model = "Ulquiorra.mdx";
        Assert.Null(doc.GetFile(model));
        Assert.True(doc.TryReadFileByName(model, out var bytes),
            $"{model} is referenced by a unit and present in the archive, so a hash lookup must find it");
        Assert.True(bytes.Length > 12);
        Assert.Equal((byte)'M', bytes[0]);
        Assert.Equal((byte)'D', bytes[1]);
        Assert.Equal((byte)'L', bytes[2]);
        Assert.Equal((byte)'X', bytes[3]);

        // And a name the archive genuinely does not hold still reports absence.
        Assert.False(doc.TryReadFileByName("NoSuchModelAnywhere.mdx", out _));
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Portrait_risk_sees_the_whole_population_not_just_the_named_entries()
    {
        if (!File.Exists(MapPath)) return;

        // Reading models through GetFile reported 3. The hash lookup reports 178. This pins the
        // floor rather than the exact number, so a model edit that fixes some of them does not
        // fail the test, while a regression back to the listfile lookup does.
        var r = AuditCommand.Execute(MapDocument.Load(MapPath), null, new[] { "portrait-risk" });
        Assert.True(r.Issues.Count > 100,
            $"portrait-risk found {r.Issues.Count}, which is the listfile-lookup blindness back again");
    }
}
