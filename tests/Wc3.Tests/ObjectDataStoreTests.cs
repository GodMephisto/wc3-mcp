using Wc3.GameData;
namespace Wc3.Tests;

public class ObjectDataStoreTests
{
    private sealed class FakeSource : IGameDataSource
    {
        private readonly Dictionary<string, byte[]> _f;
        public FakeSource(Dictionary<string, byte[]> f) => _f = f;
        public byte[]? ReadFile(string name) => _f.TryGetValue(name, out var b) ? b : null;
        public void Dispose() { }
    }

    private static byte[] A(string s) => System.Text.Encoding.ASCII.GetBytes(s);

    // Real shape (destructable-flavored): metadata row key = 4-char code;
    // "field" = data-SLK column; "slk" = data table name.
    private const string Meta =
        "ID;PWXL;N;E\r\n" +
        "C;X1;Y1;K\"ID\"\r\nC;X2;K\"field\"\r\nC;X3;K\"slk\"\r\nC;X4;K\"displayName\"\r\n" +
        "C;X1;Y2;K\"bhps\"\r\nC;X2;K\"HP\"\r\nC;X3;K\"DestructableData\"\r\nC;X4;K\"WESTRING_BEVAL_BHPS\"\r\n" +
        "C;X1;Y3;K\"bnam\"\r\nC;X2;K\"Name\"\r\nC;X3;K\"Profile\"\r\nC;X4;K\"WESTRING_BEVAL_BNAM\"\r\nE\r\n";

    // destructabledata.slk keyed by DestructableID; ATtr has HP=50.
    private const string Data =
        "ID;PWXL;N;E\r\n" +
        "C;X1;Y1;K\"DestructableID\"\r\nC;X2;K\"HP\"\r\n" +
        "C;X1;Y2;K\"ATtr\"\r\nC;X2;K50\r\nE\r\n";

    private static ObjectDataStore BuildStore() => ObjectDataStore.Build(
        new FakeSource(new()
        {
            [@"war3.w3mod:units\destructablemetadata.slk"] = A(Meta),
            [@"war3.w3mod:units\destructabledata.slk"] = A(Data),
        }),
        @"war3.w3mod:units\destructablemetadata.slk", ObjectDataStore.UnitsDirSlk);

    [Fact]
    public void Resolves_field_via_metadata_derived_slk_set()
    {
        var store = BuildStore();
        Assert.True(store.TryGet("ATtr", out var fields));
        Assert.Equal("50", fields["bhps"]);
        // "bnam" lives in Profile (a TXT, not an SLK) — not resolvable in v1.
        Assert.False(fields.ContainsKey("bnam"));
    }

    [Fact]
    public void Unknown_rawcode_returns_false_and_empty()
    {
        var store = BuildStore();
        Assert.False(store.TryGet("xxxx", out var fields));
        Assert.Empty(fields);
    }

    [Fact]
    public void Missing_metadata_throws()
    {
        var src = new FakeSource(new());
        Assert.Throws<InvalidDataException>(
            () => ObjectDataStore.Build(src, @"war3.w3mod:units\destructablemetadata.slk", ObjectDataStore.UnitsDirSlk));
    }

    [Fact]
    public void Custom_path_map_reaches_tables_outside_the_convention()
    {
        // Doodad-style layout: the "DoodadData" table's file is doodads\doodads.slk,
        // not the lowercased table name under units\.
        const string doodadMeta =
            "ID;PWXL;N;E\r\n" +
            "C;X1;Y1;K\"ID\"\r\nC;X2;K\"field\"\r\nC;X3;K\"slk\"\r\nC;X4;K\"displayName\"\r\n" +
            "C;X1;Y2;K\"dcat\"\r\nC;X2;K\"category\"\r\nC;X3;K\"DoodadData\"\r\nC;X4;K\"WESTRING_DEVAL_DCAT\"\r\nE\r\n";
        const string doodadData =
            "ID;PWXL;N;E\r\n" +
            "C;X1;Y1;K\"doodID\"\r\nC;X2;K\"category\"\r\n" +
            "C;X1;Y2;K\"AOhs\"\r\nC;X2;K\"E\"\r\nE\r\n";
        var src = new FakeSource(new()
        {
            [@"war3.w3mod:doodads\doodadmetadata.slk"] = A(doodadMeta),
            [@"war3.w3mod:doodads\doodads.slk"] = A(doodadData),
        });
        var store = ObjectDataStore.Build(src, @"war3.w3mod:doodads\doodadmetadata.slk",
            name => name.Equals("DoodadData", StringComparison.OrdinalIgnoreCase)
                ? @"war3.w3mod:doodads\doodads.slk" : ObjectDataStore.UnitsDirSlk(name));

        Assert.True(store.TryGet("AOhs", out var fields));
        Assert.Equal("E", fields["dcat"]);
    }

    [Fact]
    public void Explicit_slk_names_restrict_which_tables_load()
    {
        // Item-style layout: item fields share the unit metadata; restricting the table
        // set to ItemData keeps the store item-only even though unit SLKs are present.
        const string sharedMeta =
            "ID;PWXL;N;E\r\n" +
            "C;X1;Y1;K\"ID\"\r\nC;X2;K\"field\"\r\nC;X3;K\"slk\"\r\nC;X4;K\"displayName\"\r\n" +
            "C;X1;Y2;K\"ilev\"\r\nC;X2;K\"level\"\r\nC;X3;K\"ItemData\"\r\nC;X4;K\"WESTRING_IEVAL_ILEV\"\r\n" +
            "C;X1;Y3;K\"uhpm\"\r\nC;X2;K\"HP\"\r\nC;X3;K\"UnitBalance\"\r\nC;X4;K\"WESTRING_UEVAL_UHPM\"\r\nE\r\n";
        const string itemData =
            "ID;PWXL;N;E\r\n" +
            "C;X1;Y1;K\"itemID\"\r\nC;X2;K\"level\"\r\n" +
            "C;X1;Y2;K\"ratf\"\r\nC;X2;K7\r\nE\r\n";
        const string balance =
            "ID;PWXL;N;E\r\n" +
            "C;X1;Y1;K\"unitBalanceID\"\r\nC;X2;K\"HP\"\r\n" +
            "C;X1;Y2;K\"ratf\"\r\nC;X2;K420\r\nE\r\n";
        var src = new FakeSource(new()
        {
            [@"war3.w3mod:units\unitmetadata.slk"] = A(sharedMeta),
            [@"war3.w3mod:units\itemdata.slk"] = A(itemData),
            [@"war3.w3mod:units\unitbalance.slk"] = A(balance),
        });
        var store = ObjectDataStore.Build(src, @"war3.w3mod:units\unitmetadata.slk",
            ObjectDataStore.UnitsDirSlk, new[] { "ItemData" });

        Assert.True(store.TryGet("ratf", out var fields));
        Assert.Equal("7", fields["ilev"]);
        // unitbalance.slk was excluded, so the unit field must not leak in.
        Assert.False(fields.ContainsKey("uhpm"));
    }

    [Fact]
    public void Empty_store_resolves_nothing()
    {
        Assert.True(ObjectDataStore.Empty.IsEmpty);
        Assert.False(ObjectDataStore.Empty.TryGet("hfoo", out var fields));
        Assert.Empty(fields);
        Assert.Empty(ObjectDataStore.Empty.Metadata.Fields);
    }
}
