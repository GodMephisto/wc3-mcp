// tests/Wc3.Tests/ProtectedMapLoadTests.cs
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// A heavily protected map must LOAD, not read as empty.
///
/// BleachVsOnepiece13 names 2 of its 851 archive entries. Before the internal-name recovery,
/// every <c>war3map.*</c> lookup missed, nothing was parsed, and the audit reported
/// "OK, 0 abilities checked, 0 error(s)" for a map holding 897 abilities and 20 real errors.
/// A clean pass on a map that was never read is the most dangerous result this toolkit can
/// produce, so both halves are pinned here.
/// </summary>
public class ProtectedMapLoadTests
{
    private static readonly string V13 =
        TestCorpus.Map(@"BleachVsOnepiece13.w3x");
    private static readonly string V15 =
        TestCorpus.Map(@"BleachVsOnepiece15.w3x");

    [Fact]
    [Trait("Category", "Corpus")]
    public void A_map_that_names_almost_nothing_still_parses_its_object_data()
    {
        if (!File.Exists(V13)) return;
        var doc = MapDocument.Load(V13);

        // The control. The premise is that this map really does name almost nothing.
        int named = doc.Files.Count(f => f.FileName is not null);
        Assert.True(named < 10,
            $"expected a heavily protected map, but {named} entries are already named");

        // And yet the object data is fully there.
        Assert.True(
            ObjectKinds.MergedEntries(doc, ObjectKinds.Info(ObjectKind.Ability)).Count() > 800,
            "abilities did not parse, so the map read as empty");
        Assert.True(
            ObjectKinds.MergedEntries(doc, ObjectKinds.Info(ObjectKind.Unit)).Count() > 600,
            "units did not parse");
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void The_script_is_found_under_the_scripts_folder_spelling_too()
    {
        if (!File.Exists(V13)) return;
        var doc = MapDocument.Load(V13);

        // v13 holds a 4.4 MB script at scripts\war3map.j and NOTHING at war3map.j. Reading only
        // the second spelling returned empty, and an empty script makes every ability look
        // unimplemented, which is how the orphan check reported 166 on a map whose real
        // number is one.
        Assert.False(doc.TryReadFileByName("war3map.j", out _));
        Assert.True(doc.TryReadFileByName(@"scripts\war3map.j", out var script));
        Assert.True(script.Length > 4_000_000);

        var r = AuditCommand.Execute(doc, null, new[] { "orphan-ability" });
        Assert.True(r.Issues.Count < 10,
            $"orphan-ability reported {r.Issues.Count}, which is the empty-script signature");
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void V13_and_v15_carry_identical_object_data()
    {
        if (!File.Exists(V13) || !File.Exists(V15)) return;
        var a = MapDocument.Load(V13);
        var b = MapDocument.Load(V15);

        foreach (var kind in ObjectKinds.All)
        {
            var x = ObjectKinds.MergedEntries(a, ObjectKinds.Info(kind))
                .ToDictionary(e => e.Id, e => e.Mods.Count);
            var y = ObjectKinds.MergedEntries(b, ObjectKinds.Info(kind))
                .ToDictionary(e => e.Id, e => e.Mods.Count);
            Assert.Equal(x.Count, y.Count);
            Assert.Equal(x.OrderBy(k => k.Key), y.OrderBy(k => k.Key));
        }

        // Which is why reverting to v13 fixes nothing. Both audit to the same issue count.
        var ra = AuditCommand.Execute(a);
        var rb = AuditCommand.Execute(b);
        Assert.Equal(rb.Issues.Count, ra.Issues.Count);
        Assert.Equal(rb.Issues.Count(i => i.Severity == DiagnosticSeverity.Error),
                     ra.Issues.Count(i => i.Severity == DiagnosticSeverity.Error));
    }
}
