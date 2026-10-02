using Wc3.GameData;
namespace Wc3.Tests;

public class BaseAbilityStoreTests
{
    private sealed class FakeSource : IGameDataSource
    {
        private readonly Dictionary<string, byte[]> _f;
        public FakeSource(Dictionary<string, byte[]> f) => _f = f;
        public byte[]? ReadFile(string name) => _f.TryGetValue(name, out var b) ? b : null;
        public void Dispose() { }
    }

    private static byte[] A(string s) => System.Text.Encoding.ASCII.GetBytes(s);

    // Real shape: metadata row key = 4-char code; "field" = data-SLK column; "slk" = data SLK
    // name; repeat > 0 = leveled numbered columns; data > 0 = the ability data-column letter;
    // useSpecific scopes a field to the listed rawcodes and notSpecific excludes them.
    private const string Meta =
        "ID;PWXL;N;E\r\n" +
        "C;X1;Y1;K\"ID\"\r\nC;X2;K\"field\"\r\nC;X3;K\"slk\"\r\nC;X4;K\"displayName\"\r\nC;X5;K\"repeat\"\r\nC;X6;K\"data\"\r\nC;X7;K\"useSpecific\"\r\nC;X8;K\"notSpecific\"\r\n" +
        "C;X1;Y2;K\"aher\"\r\nC;X2;K\"hero\"\r\nC;X3;K\"AbilityData\"\r\nC;X4;K\"WESTRING_AEVAL_AHER\"\r\nC;X5;K0\r\nC;X6;K0\r\n" +
        "C;X1;Y3;K\"anam\"\r\nC;X2;K\"Name\"\r\nC;X3;K\"Profile\"\r\nC;X4;K\"WESTRING_AEVAL_ANAM\"\r\nC;X5;K0\r\nC;X6;K0\r\n" +
        "C;X1;Y4;K\"acdn\"\r\nC;X2;K\"Cool\"\r\nC;X3;K\"AbilityData\"\r\nC;X4;K\"WESTRING_AEVAL_ACDN\"\r\nC;X5;K4\r\nC;X6;K0\r\n" +
        "C;X1;Y5;K\"Hbz2\"\r\nC;X2;K\"Data\"\r\nC;X3;K\"AbilityData\"\r\nC;X4;K\"WESTRING_AEVAL_DATA\"\r\nC;X5;K4\r\nC;X6;K2\r\nC;X7;K\"AHbz\"\r\n" +
        "C;X1;Y6;K\"xnot\"\r\nC;X2;K\"hero\"\r\nC;X3;K\"AbilityData\"\r\nC;X4;K\"WESTRING_AEVAL_XNOT\"\r\nC;X5;K0\r\nC;X6;K0\r\nC;X8;K\"Adef\"\r\nE\r\n";

    // abilitydata.slk keyed by alias; AHbz is a 3-level hero ability whose 4th column set
    // is padding (the real files copy the last level), Adef is single-level.
    private const string Data =
        "ID;PWXL;N;E\r\n" +
        "C;X1;Y1;K\"alias\"\r\nC;X2;K\"hero\"\r\nC;X3;K\"levels\"\r\n" +
        "C;X4;K\"cool1\"\r\nC;X5;K\"cool2\"\r\nC;X6;K\"cool3\"\r\nC;X7;K\"cool4\"\r\n" +
        "C;X8;K\"datab1\"\r\nC;X9;K\"datab2\"\r\nC;X10;K\"datab3\"\r\nC;X11;K\"datab4\"\r\n" +
        "C;X1;Y2;K\"AHbz\"\r\nC;X2;K1\r\nC;X3;K3\r\n" +
        "C;X4;K6\r\nC;X5;K7\r\nC;X6;K8\r\nC;X7;K8\r\n" +
        "C;X8;K30\r\nC;X9;K40\r\nC;X10;K50\r\nC;X11;K50\r\n" +
        "C;X1;Y3;K\"Adef\"\r\nC;X2;K0\r\nC;X3;K1\r\nC;X4;K10\r\nC;X8;K99\r\nE\r\n";

    private static BaseAbilityStore BuildStore() => BaseAbilityStore.Build(new FakeSource(new()
    {
        [@"war3.w3mod:units\abilitymetadata.slk"] = A(Meta),
        [@"war3.w3mod:units\abilitydata.slk"] = A(Data),
    }));

    [Fact]
    public void Resolves_base_ability_field_via_metadata()
    {
        var store = BuildStore();
        Assert.True(store.TryGetAbility("AHbz", out var fields));
        Assert.Equal("1", fields["aher"]);
        // "anam" lives in Profile (a TXT, not one of the ability SLKs) — not resolvable in v1.
        Assert.False(fields.ContainsKey("anam"));
    }

    [Fact]
    public void Leveled_slk_field_expands_to_per_level_keys_clamped_at_level_count()
    {
        var store = BuildStore();
        Assert.True(store.TryGetAbility("AHbz", out var fields));
        // Bare code keeps level 1 so flat lookups stay stable.
        Assert.Equal("6", fields["acdn"]);
        Assert.Equal("6", fields["acdn:1"]);
        Assert.Equal("7", fields["acdn:2"]);
        Assert.Equal("8", fields["acdn:3"]);
        // levels=3 hides the padded 4th column set.
        Assert.False(fields.ContainsKey("acdn:4"));
    }

    [Fact]
    public void Data_letter_selects_the_lettered_column_family()
    {
        var store = BuildStore();
        Assert.True(store.TryGetAbility("AHbz", out var fields));
        // Hbz2 has metadata field=Data with data=2, so it reads datab1..datab4.
        Assert.Equal("30", fields["Hbz2"]);
        Assert.Equal("40", fields["Hbz2:2"]);
        Assert.Equal("50", fields["Hbz2:3"]);
    }

    [Fact]
    public void Single_level_ability_stays_bare()
    {
        var store = BuildStore();
        Assert.True(store.TryGetAbility("Adef", out var fields));
        Assert.Equal("10", fields["acdn"]);
        Assert.False(fields.ContainsKey("acdn:1"));
    }

    [Fact]
    public void Use_specific_scopes_data_fields_to_listed_abilities()
    {
        var store = BuildStore();
        // Adef has a datab1 cell but Hbz2 is scoped to AHbz, so it must not leak.
        Assert.True(store.TryGetAbility("Adef", out var fields));
        Assert.False(fields.ContainsKey("Hbz2"));
        Assert.False(fields.ContainsKey("Hbz2:1"));
    }

    [Fact]
    public void Not_specific_excludes_listed_abilities()
    {
        var store = BuildStore();
        Assert.True(store.TryGetAbility("AHbz", out var hbz));
        Assert.Equal("1", hbz["xnot"]);
        Assert.True(store.TryGetAbility("Adef", out var def));
        Assert.False(def.ContainsKey("xnot"));
    }

    [Fact]
    public void Unknown_rawcode_returns_false_and_empty()
    {
        var store = BuildStore();
        Assert.False(store.TryGetAbility("Axxx", out var fields));
        Assert.Empty(fields);
    }

    [Fact]
    public void Missing_metadata_throws()
    {
        var src = new FakeSource(new());
        Assert.Throws<InvalidDataException>(() => BaseAbilityStore.Build(src));
    }
}
