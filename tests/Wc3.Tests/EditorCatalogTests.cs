// tests/Wc3.Tests/EditorCatalogTests.cs
using Wc3.Commands;
using Wc3.GameData;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// The World Editor's catalogs come from the game's own UI\WorldEditData.txt. Reading them
/// through the enum lens loses the data. The stored key vanishes, equal payloads dedup away
/// (18 tileset lights became 6), swapped columns put a WESTRING key where the value belongs
/// (TileSets), and count lines leaked in as rows (MapSizes offered "9"). The catalog lens
/// keeps the key, every payload field, duplicates and file order.
/// </summary>
public class EditorCatalogTests
{
    private const string Install = @"D:\Warcraft III";

    private static EditorCatalogData Parse(string text, string strings = "") =>
        EditorCatalogData.FromByteSources(WorldEditStrings.Parse(strings),
            System.Text.Encoding.UTF8.GetBytes(text));

    [Fact]
    public void A_tileset_keeps_its_key_display_name_and_payload()
    {
        // The TileSets shape puts the WESTRING first and the payload second, the reverse of
        // an enum section. The key letter is the token maps actually store.
        var data = Parse(
            "[TileSets]\nA=WESTRING_LOCALE_ASHENVALE,TerrainArt\\Blight\\Ashen_Blight\n",
            "WESTRING_LOCALE_ASHENVALE=Ashenvale\n");

        Assert.True(data.TryGet("TileSets", out var entries));
        var e = Assert.Single(entries);
        Assert.Equal("A", e.Key);
        Assert.Equal("Ashenvale", e.DisplayName);
        Assert.Equal("Ashenvale", e.Label);
        Assert.Equal(new[] { "WESTRING_LOCALE_ASHENVALE", @"TerrainArt\Blight\Ashen_Blight" },
            e.Values);
    }

    [Fact]
    public void Equal_payloads_stay_distinct_entries()
    {
        // TerrainLights keys 18 tilesets onto 6 model paths. Deduping by value, as the enum
        // lens does, silently threw 12 tilesets away.
        var data = Parse("[TerrainLights]\nL=Environment\\A.mdl\nF=Environment\\A.mdl\n");

        Assert.True(data.TryGet("TerrainLights", out var entries));
        Assert.Equal(new[] { "L", "F" }, entries.Select(e => e.Key));
        Assert.All(entries, e => Assert.Equal(@"Environment\A.mdl", e.Values[0]));
    }

    [Fact]
    public void Count_lines_are_bookkeeping_but_a_num_prefixed_entry_is_not()
    {
        // NumScreens describes the section. A key merely starting with Num whose value is
        // not an integer must survive, the integer check is what keeps the rule honest.
        var data = Parse("[LoadingScreens]\nNumScreens=2\nSort=1\n00=0,W_A,0,UI\\a.mdl\n"
                       + "NumbersFile=foo.txt\n");

        Assert.True(data.TryGet("LoadingScreens", out var entries));
        Assert.Equal(new[] { "00", "NumbersFile" }, entries.Select(e => e.Key));
    }

    [Fact]
    public void Every_comma_field_survives_in_order()
    {
        // A loading screen row carries four fields (version, WESTRING, sequence, model).
        // The enum lens kept one and mangled the rest.
        var data = Parse(
            "[LoadingScreens]\n00=0,WESTRING_LS_T01,3,UI\\Glues\\Loading\\Back.mdl\n",
            "WESTRING_LS_T01=Tutorial 01\n");

        Assert.True(data.TryGet("LoadingScreens", out var entries));
        var e = Assert.Single(entries);
        Assert.Equal(new[] { "0", "WESTRING_LS_T01", "3", @"UI\Glues\Loading\Back.mdl" },
            e.Values);
        Assert.Equal("Tutorial 01", e.DisplayName);
    }

    [Fact]
    public void An_unresolved_westring_never_leaks_into_the_label()
    {
        // The stored key is the honest fallback. A raw WESTRING key in a picker is worse
        // than either the key or nothing.
        var data = Parse("[ObjectEditorCategories]\nabil=WESTRING_DOES_NOT_EXIST\n");

        Assert.True(data.TryGet("ObjectEditorCategories", out var entries));
        Assert.Equal("abil", entries[0].Label);
        Assert.DoesNotContain("WESTRING", entries[0].Label);
    }

    [Fact]
    public void A_menu_accelerator_ampersand_is_stripped_from_the_display()
    {
        // The shipped string is "A&bilities", the ampersand is a Windows menu accelerator
        // marker, not part of the name. A doubled ampersand is one literal ampersand.
        var data = Parse("[ObjectEditorCategories]\nabil=WESTRING_OE_CAT_ABILITIES\n"
                       + "amp=WESTRING_AMP\n",
            "WESTRING_OE_CAT_ABILITIES=A&bilities\nWESTRING_AMP=Rock && Roll\n");

        Assert.True(data.TryGet("ObjectEditorCategories", out var entries));
        Assert.Equal("Abilities", entries[0].DisplayName);
        Assert.Equal("Rock & Roll", entries[1].DisplayName);
    }

    [Fact]
    public void Commented_out_entries_are_skipped()
    {
        // DayEnvironmentMap ships mostly commented out. The commented rows are Blizzard's
        // notes, not catalog data.
        var data = Parse("[DayEnvironmentMap]\n//X=Environment/X/day_ibl.tif\n"
                       + "Default=Environment/L/day_ibl.tif\n");

        Assert.True(data.TryGet("DayEnvironmentMap", out var entries));
        var e = Assert.Single(entries);
        Assert.Equal("Default", e.Key);
    }

    [Fact]
    public void Names_keep_file_order_and_lookup_is_case_insensitive()
    {
        var data = Parse("[TileSets]\nA=W_A,x\n[SkyModels]\n00=y,W_B\n");

        Assert.Equal(new[] { "TileSets", "SkyModels" }, data.Names);
        Assert.True(data.TryGet("tilesets", out var entries));
        Assert.Single(entries);
    }

    [Fact]
    public void Empty_is_safe_to_query()
    {
        Assert.False(EditorCatalogData.Empty.TryGet("TileSets", out var entries));
        Assert.Empty(entries);
        Assert.Empty(EditorCatalogData.Empty.Names);
        Assert.False(EditorCatalogData.Empty.TryGet("", out _));
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void The_retail_install_defines_the_catalogs_with_their_real_sizes()
    {
        if (!Directory.Exists(Install)) return;
        if (!GameData.GameData.TryOpen(Install, out var ctx, out _) || ctx is null) return;

        var cats = ctx.EditorCatalogs;
        // The counts the shipped file actually holds, measured 2026-08-28. The enum lens
        // reported 6, 2 and 3 for the last three, which is the bug this type exists to fix.
        Assert.True(cats.TryGet("TileSets", out var tiles));
        Assert.Equal(18, tiles.Count);
        Assert.True(cats.TryGet("TerrainLights", out var lights));
        Assert.Equal(18, lights.Count);
        Assert.True(cats.TryGet("SoundChannels", out var channels));
        Assert.Equal(15, channels.Count);
        Assert.True(cats.TryGet("LoadingScreens", out var screens));
        Assert.Equal(101, screens.Count);
        // MapSizes must not contain the NumSizes count line as a row.
        Assert.True(cats.TryGet("MapSizes", out var sizes));
        Assert.Equal(9, sizes.Count);
        Assert.DoesNotContain(sizes, s => s.Key.StartsWith("Num", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void Retail_entries_resolve_to_the_names_the_editor_shows()
    {
        if (!Directory.Exists(Install)) return;
        if (!GameData.GameData.TryOpen(Install, out var ctx, out _) || ctx is null) return;

        var cats = ctx.EditorCatalogs;
        Assert.True(cats.TryGet("TileSets", out var tiles));
        var ashenvale = tiles.First(t => t.Key == "A");
        Assert.Equal("Ashenvale", ashenvale.Label);
        Assert.EndsWith("Ashen_Blight", ashenvale.Values[^1]);

        Assert.True(cats.TryGet("SoundChannels", out var channels));
        var error = channels.First(c => c.Key == "06");
        Assert.Equal("Error", error.Label);
        Assert.Equal("0", error.Values[0]);

        // The accelerator marker is stripped, the editor shows "Abilities".
        Assert.True(cats.TryGet("ObjectEditorCategories", out var oeCats));
        Assert.Equal("Abilities", oeCats.First(c => c.Key == "abil").Label);

        // A settings section keeps its key and both comma fields.
        Assert.True(cats.TryGet("WorldEditMisc", out var misc));
        var mapSize = misc.First(m => m.Key == "DefaultMapSize");
        Assert.Equal(new[] { "64", "64" }, mapSize.Values);
    }

    [Fact]
    [Trait("Category", "GameData")]
    public void The_command_layer_lists_names_and_one_catalog()
    {
        if (!Directory.Exists(Install)) return;

        var names = EditorCatalogCommand.Names(Install);
        Assert.True(names.Total >= 41);
        Assert.Contains(names.Catalogs, c => c.Name == "TileSets" && c.Entries == 18);

        // Case insensitive in, the file's own casing out.
        var list = EditorCatalogCommand.List("tilesets", Install);
        Assert.Equal("TileSets", list.Catalog);
        Assert.Equal(18, list.Total);

        var ex = Assert.Throws<InvalidOperationException>(
            () => EditorCatalogCommand.List("NoSuchCatalog", Install));
        Assert.Contains("Unknown catalog", ex.Message);
    }
}
