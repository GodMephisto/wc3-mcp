using Wc3.GameData;
namespace Wc3.Tests;

/// <summary>
/// Hermetic coverage for the Profile TXT layer: the INI-like parser itself, the
/// element splitter, and the ObjectDataStore join that turns profile keys into
/// field codes (index picks, per-level expansion, buff name fallback, and the
/// SLK-catalog gate that keeps ability sections out of buff queries).
/// </summary>
public class ProfileTxtStoreTests
{
    private sealed class FakeSource : IGameDataSource
    {
        private readonly Dictionary<string, byte[]> _f;
        public FakeSource(Dictionary<string, byte[]> f) => _f = f;
        public byte[]? ReadFile(string name) => _f.TryGetValue(name, out var b) ? b : null;
        public void Dispose() { }
    }

    private static byte[] A(string s) => System.Text.Encoding.ASCII.GetBytes(s);

    [Fact]
    public void Parses_sections_keys_comments_and_variant_overrides()
    {
        var src = new FakeSource(new()
        {
            ["a.txt"] = A(
                "// header comment\r\n" +
                "[ratf]\r\n" +
                "Name=Claws of Attack +15\r\n" +
                "Ubertip=\"Boosts damage, a lot.\"\r\n" +
                "Ubertip:melee,V0=\"variant text\"\r\n" +
                "\r\n" +
                "[rhth]\r\n" +
                "Name=Periapt of Vitality\r\n"),
        });
        var store = ProfileTxtStore.FromSources(src, new[] { "a.txt", "missing.txt" });

        Assert.Equal(2, store.Count);
        Assert.True(store.TryGetValue("ratf", "Name", out var name));
        Assert.Equal("Claws of Attack +15", name);
        Assert.True(store.TryGetValue("RATF", "name", out _)); // lookups are case-insensitive
        Assert.True(store.TryGetValue("ratf", "Ubertip", out var tip));
        Assert.Equal("Boosts damage, a lot.", ProfileTxtStore.SplitElements(tip).Single());
        Assert.False(store.TryGetValue("ratf", "Ubertip:melee,V0", out _)); // variant key skipped
        Assert.True(store.TryGetValue("rhth", "Name", out _));
    }

    [Fact]
    public void Later_files_overlay_earlier_ones_per_key()
    {
        var src = new FakeSource(new()
        {
            ["func.txt"] = A("[ratf]\r\nArt=BTNClaws.blp\r\nName=placeholder\r\n"),
            ["strings.txt"] = A("[ratf]\r\nName=Claws of Attack +15\r\n"),
        });
        var store = ProfileTxtStore.FromSources(src, new[] { "func.txt", "strings.txt" });

        Assert.True(store.TryGetValue("ratf", "Name", out var name));
        Assert.Equal("Claws of Attack +15", name);
        Assert.True(store.TryGetValue("ratf", "Art", out var art));
        Assert.Equal("BTNClaws.blp", art);
    }

    [Fact]
    public void Westring_values_resolve_against_editor_strings()
    {
        var strings = WorldEditStrings.Parse("[WorldEditStrings]\nWESTRING_FOO=\"Resolved Name\"\n");
        var src = new FakeSource(new() { ["a.txt"] = A("[xxxx]\r\nName=WESTRING_FOO\r\n") });
        var store = ProfileTxtStore.FromSources(src, new[] { "a.txt" }, strings);

        Assert.True(store.TryGetValue("xxxx", "Name", out var name));
        Assert.Equal("Resolved Name", name);
    }

    [Fact]
    public void Split_elements_honors_quotes()
    {
        Assert.Equal(new[] { "a", "b, with comma", "c" },
            ProfileTxtStore.SplitElements("a,\"b, with comma\",c"));
        Assert.Equal(new[] { "single" }, ProfileTxtStore.SplitElements("\"single\""));
        Assert.Equal(new[] { "plain" }, ProfileTxtStore.SplitElements("plain"));
    }

    // Destructable-flavored metadata exercising every profile shape: a plain name,
    // a per-level list (repeat=1), an x/y pair (index=1), and a modelList that must
    // keep its comma list whole. The SLK row makes ATtr part of the kind's catalog.
    private const string Meta =
        "ID;PWXL;N;E\r\n" +
        "C;X1;Y1;K\"ID\"\r\nC;X2;K\"field\"\r\nC;X3;K\"slk\"\r\nC;X4;K\"index\"\r\nC;X5;K\"repeat\"\r\nC;X6;K\"type\"\r\n" +
        "C;X1;Y2;K\"bhps\"\r\nC;X2;K\"HP\"\r\nC;X3;K\"DestructableData\"\r\nC;X4;K0\r\nC;X5;K0\r\nC;X6;K\"int\"\r\n" +
        "C;X1;Y3;K\"bnam\"\r\nC;X2;K\"Name\"\r\nC;X3;K\"Profile\"\r\nC;X4;K0\r\nC;X5;K0\r\nC;X6;K\"string\"\r\n" +
        "C;X1;Y4;K\"btp1\"\r\nC;X2;K\"Tip\"\r\nC;X3;K\"Profile\"\r\nC;X4;K0\r\nC;X5;K1\r\nC;X6;K\"string\"\r\n" +
        "C;X1;Y5;K\"bbpy\"\r\nC;X2;K\"Buttonpos\"\r\nC;X3;K\"Profile\"\r\nC;X4;K1\r\nC;X5;K0\r\nC;X6;K\"int\"\r\n" +
        "C;X1;Y6;K\"bmdl\"\r\nC;X2;K\"Models\"\r\nC;X3;K\"Profile\"\r\nC;X4;K0\r\nC;X5;K0\r\nC;X6;K\"modelList\"\r\nE\r\n";

    private const string Data =
        "ID;PWXL;N;E\r\n" +
        "C;X1;Y1;K\"DestructableID\"\r\nC;X2;K\"HP\"\r\n" +
        "C;X1;Y2;K\"ATtr\"\r\nC;X2;K50\r\nE\r\n";

    private const string Profile =
        "[ATtr]\r\n" +
        "Name=\"Tree Wall\"\r\n" +
        "Tip=Level One,Level Two\r\n" +
        "Buttonpos=1,2\r\n" +
        "Models=a.mdl,b.mdl\r\n" +
        "[Xzzz]\r\n" +
        "Name=Profile Only Ghost\r\n";

    private static ObjectDataStore BuildJoinedStore()
    {
        var src = new FakeSource(new()
        {
            [@"war3.w3mod:units\destructablemetadata.slk"] = A(Meta),
            [@"war3.w3mod:units\destructabledata.slk"] = A(Data),
            ["profile.txt"] = A(Profile),
        });
        var profile = ProfileTxtStore.FromSources(src, new[] { "profile.txt" });
        return ObjectDataStore.Build(
            src, @"war3.w3mod:units\destructablemetadata.slk", ObjectDataStore.UnitsDirSlk,
            slkNames: null, profile: profile);
    }

    [Fact]
    public void Profile_fields_join_by_index_repeat_and_list_type()
    {
        var store = BuildJoinedStore();
        Assert.True(store.TryGet("ATtr", out var fields));
        Assert.Equal("50", fields["bhps"]);
        Assert.Equal("Tree Wall", fields["bnam"]);
        Assert.Equal("Level One", fields["btp1"]); // bare code keeps level 1
        Assert.Equal("Level One", fields["btp1:1"]);
        Assert.Equal("Level Two", fields["btp1:2"]);
        Assert.Equal("2", fields["bbpy"]); // index 1 picks the y element
        Assert.Equal("a.mdl,b.mdl", fields["bmdl"]); // list type stays whole
    }

    [Fact]
    public void Profile_only_rawcodes_stay_outside_the_catalog()
    {
        // The ability profile files carry buff sections too, so a rawcode absent
        // from the kind's data SLKs must not resolve through the shared TXT.
        var store = BuildJoinedStore();
        Assert.False(store.TryGet("Xzzz", out var fields));
        Assert.Empty(fields);
    }

    [Fact]
    public void Buff_name_falls_back_to_bufftip_when_editor_name_is_absent()
    {
        const string buffMeta =
            "ID;PWXL;N;E\r\n" +
            "C;X1;Y1;K\"ID\"\r\nC;X2;K\"field\"\r\nC;X3;K\"slk\"\r\nC;X4;K\"index\"\r\n" +
            "C;X1;Y2;K\"frac\"\r\nC;X2;K\"race\"\r\nC;X3;K\"AbilityBuffData\"\r\nC;X4;K0\r\n" +
            "C;X1;Y3;K\"fnam\"\r\nC;X2;K\"EditorName\"\r\nC;X3;K\"Profile\"\r\nC;X4;K-1\r\n" +
            "C;X1;Y4;K\"ftip\"\r\nC;X2;K\"Bufftip\"\r\nC;X3;K\"Profile\"\r\nC;X4;K-1\r\nE\r\n";
        const string buffData =
            "ID;PWXL;N;E\r\n" +
            "C;X1;Y1;K\"alias\"\r\nC;X2;K\"race\"\r\n" +
            "C;X1;Y2;K\"BSTN\"\r\nC;X2;K\"other\"\r\n" +
            "C;X1;Y3;K\"Bnam\"\r\nC;X2;K\"other\"\r\nE\r\n";
        const string buffProfile =
            "[BSTN]\r\nBufftip=Stunned\r\n" +
            "[Bnam]\r\nEditorName=Named Buff\r\nBufftip=Something Else\r\n";
        var src = new FakeSource(new()
        {
            [@"war3.w3mod:units\abilitybuffmetadata.slk"] = A(buffMeta),
            [@"war3.w3mod:units\abilitybuffdata.slk"] = A(buffData),
            ["profile.txt"] = A(buffProfile),
        });
        var profile = ProfileTxtStore.FromSources(src, new[] { "profile.txt" });
        var store = ObjectDataStore.BuildBuffs(src, profile);

        Assert.True(store.TryGet("BSTN", out var stun));
        Assert.Equal("Stunned", stun["fnam"]); // copied from ftip
        Assert.True(store.TryGet("Bnam", out var named));
        Assert.Equal("Named Buff", named["fnam"]); // EditorName wins when present
    }
}
