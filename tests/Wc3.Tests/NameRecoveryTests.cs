// tests/Wc3.Tests/NameRecoveryTests.cs
using System.Text;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Name recovery, each claim watched both ways.
///
/// The hermetic half proves the spelling generator and the stamping. The corpus half proves
/// the whole pass on the real protected map, because the defect this exists for only appears
/// when the listfile is missing and a synthetic map is always built with one.
/// </summary>
public class NameRecoveryTests
{
    private static readonly string MapPath = TestCorpus.Map(@"BVO16g.w3x");

    [Fact]
    public void Spellings_covers_the_extension_and_prefix_a_map_actually_disagrees_on()
    {
        // The real case. A unit declares Ulquiorra.mdl and the archive stores Ulquiorra.mdx,
        // and an import may or may not carry the war3mapImported prefix.
        var s = NameRecoveryCommand.Spellings("Ulquiorra.mdl").ToList();
        Assert.Contains("Ulquiorra.mdl", s);
        Assert.Contains("Ulquiorra.mdx", s);
        Assert.Contains(@"war3mapImported\Ulquiorra.mdx", s);
        Assert.Contains(@"ReplaceableTextures\CommandButtons\Ulquiorra.blp", s);
    }

    [Fact]
    public void Spellings_strips_a_declared_directory_because_the_archive_may_store_it_flat()
    {
        var s = NameRecoveryCommand.Spellings(@"war3mapImported\Toushirou.mdl").ToList();
        Assert.Contains("Toushirou.mdx", s);
        Assert.Contains("Toushirou.mdl", s);
    }

    [Fact]
    public void Icon_twins_rewrite_the_stem_which_is_the_only_way_to_reach_a_disabled_icon()
    {
        // The engine derives the greyed out icon itself from the enabled one, so DISBTNFoo is
        // named by no field, no script literal, no model and no dictionary. On one real map
        // this single rule took recovery from 833 of 851 to 849.
        var t = NameRecoveryCommand.IconTwins(@"ReplaceableTextures\CommandButtons\BTNGarp.blp")
            .ToList();
        Assert.Contains(@"ReplaceableTextures\CommandButtonsDisabled\DISBTNGarp.blp", t);
        Assert.Contains(@"ReplaceableTextures\PassiveButtons\PASBTNGarp.blp", t);

        // And the other direction, because a map may declare either one.
        var back = NameRecoveryCommand.IconTwins("DISBTNGarp.blp").ToList();
        Assert.Contains(@"ReplaceableTextures\CommandButtons\BTNGarp.blp", back);

        // A folder-only variant can never produce these, which is why Spellings is not enough.
        Assert.DoesNotContain(
            @"ReplaceableTextures\CommandButtonsDisabled\DISBTNGarp.blp",
            NameRecoveryCommand.Spellings(@"ReplaceableTextures\CommandButtons\BTNGarp.blp"));
    }

    [Fact]
    public void Icon_twins_strips_the_longest_prefix_so_a_DISBTN_does_not_keep_a_stray_DIS()
    {
        var t = NameRecoveryCommand.IconTwins("DISBTNGarp.blp").ToList();
        Assert.Contains("BTNGarp.blp", t);
        Assert.DoesNotContain("BTNDISGarp.blp", t);
        Assert.DoesNotContain("DISBTNDISGarp.blp", t);
    }

    [Fact]
    public void Identify_reads_the_kind_from_the_bytes_and_an_mdx_states_its_own_name()
    {
        using var ms = new MemoryStream();
        using (var w = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
        {
            w.Write(Encoding.ASCII.GetBytes("MDLX"));
            w.Write(Encoding.ASCII.GetBytes("MODL"));
            w.Write(0x150u);
            var body = new byte[0x150];
            Encoding.ASCII.GetBytes("CloudOfFog").CopyTo(body, 0);
            w.Write(body);
        }
        var (kind, selfName) = MapDocument.IdentifyBytes(ms.ToArray());
        Assert.Equal("MDX model", kind);
        Assert.Equal("CloudOfFog", selfName);

        Assert.Equal("BLP texture", MapDocument.IdentifyBytes("BLP1"u8).Kind);
        Assert.Equal("empty", MapDocument.IdentifyBytes(Array.Empty<byte>()).Kind);
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Recovery_names_most_of_a_real_protected_map_and_says_what_it_could_not()
    {
        if (!File.Exists(MapPath)) return;
        var doc = MapDocument.Load(MapPath);
        var r = NameRecoveryCommand.Execute(doc);

        // The control. The premise is that this map really is protected.
        Assert.True(r.NamedBefore < r.TotalBlocks / 2,
            $"expected a protected map, {r.NamedBefore} of {r.TotalBlocks} were already named");

        // Without any dictionary the map's own data still names most of it.
        Assert.True(r.NamedAfter > r.NamedBefore * 5,
            $"recovery went from {r.NamedBefore} to {r.NamedAfter}, which is barely a recovery");

        // A floor, not the exact number, so adding a source does not fail the test while a
        // source silently breaking does. Measured at 849 of 851 with no dictionary at all.
        Assert.True(r.NamedAfter >= r.TotalBlocks * 0.95,
            $"named {r.NamedAfter} of {r.TotalBlocks}, below the 95% this map reaches");

        // And it never claims total coverage. What is left is reported with what it is, read
        // from the bytes, because War3Net key-detects an encrypted block without its name.
        Assert.NotEmpty(r.StillUnnamed);
        Assert.All(r.StillUnnamed, u => Assert.NotEqual("", u.Kind));
        Assert.Contains(r.StillUnnamed, u => u.Kind.Contains("BLP") || u.Kind.Contains("MDX"));
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Applied_names_survive_a_save_and_reload()
    {
        if (!File.Exists(MapPath)) return;
        var doc = MapDocument.Load(MapPath);
        var r = NameRecoveryCommand.Execute(doc);
        int stamped = NameRecoveryCommand.ApplyNames(doc, r);
        Assert.True(stamped > 0);

        string tmp = Path.Combine(Path.GetTempPath(), $"deprot-{Guid.NewGuid():N}.w3x");
        try
        {
            doc.Save(tmp);
            var back = MapDocument.Load(tmp);
            int namedAfter = back.Files.Count(f => f.FileName is not null);

            // This is the assertion that matters. Writing a (listfile) as an ordinary file
            // produced 58 named before and 58 after, a clean silent no-op, because
            // MpqArchiveBuilder regenerates the listfile from the names it knows.
            Assert.True(namedAfter > r.NamedBefore * 5,
                $"names did not survive the save, {r.NamedBefore} before and {namedAfter} after");

            // And the map itself still works.
            Assert.Equal(
                ObjectKinds.MergedEntries(doc, ObjectKinds.Info(ObjectKind.Ability)).Count(),
                ObjectKinds.MergedEntries(back, ObjectKinds.Info(ObjectKind.Ability)).Count());
        }
        finally
        {
            if (File.Exists(tmp)) File.Delete(tmp);
        }
    }
}
