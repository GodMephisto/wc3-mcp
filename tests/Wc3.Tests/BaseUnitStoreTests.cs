using Wc3.GameData;
namespace Wc3.Tests;

public class BaseUnitStoreTests
{
    private sealed class FakeSource : IGameDataSource
    {
        private readonly Dictionary<string, byte[]> _f;
        public FakeSource(Dictionary<string, byte[]> f) => _f = f;
        public byte[]? ReadFile(string name) => _f.TryGetValue(name, out var b) ? b : null;
        public void Dispose() { }
    }

    private static byte[] A(string s) => System.Text.Encoding.ASCII.GetBytes(s);

    // Real shape: metadata row key = 4-char code; "field" = data-SLK column; "slk" = data SLK name.
    private const string Meta =
        "ID;PWXL;N;E\r\n" +
        "C;X1;Y1;K\"ID\"\r\nC;X2;K\"field\"\r\nC;X3;K\"slk\"\r\nC;X4;K\"displayName\"\r\n" +
        "C;X1;Y2;K\"uhpm\"\r\nC;X2;K\"HP\"\r\nC;X3;K\"UnitBalance\"\r\nC;X4;K\"WESTRING_UEVAL_UHPM\"\r\n" +
        "C;X1;Y3;K\"unam\"\r\nC;X2;K\"Name\"\r\nC;X3;K\"Profile\"\r\nC;X4;K\"WESTRING_UEVAL_UNAM\"\r\nE\r\n";

    // unitbalance.slk keyed by unitBalanceID; hfoo has HP=420.
    private const string Balance =
        "ID;PWXL;N;E\r\n" +
        "C;X1;Y1;K\"unitBalanceID\"\r\nC;X2;K\"HP\"\r\n" +
        "C;X1;Y2;K\"hfoo\"\r\nC;X2;K420\r\nE\r\n";

    private static BaseUnitStore BuildStore() => BaseUnitStore.Build(new FakeSource(new()
    {
        [@"war3.w3mod:units\unitmetadata.slk"] = A(Meta),
        [@"war3.w3mod:units\unitbalance.slk"] = A(Balance),
    }));

    [Fact]
    public void Resolves_base_unit_field_via_metadata()
    {
        var store = BuildStore();
        Assert.True(store.TryGetUnit("hfoo", out var fields));
        Assert.Equal("420", fields["uhpm"]);
        // "unam" lives in Profile (a TXT, not one of the unit SLKs) — not resolvable in v1.
        Assert.False(fields.ContainsKey("unam"));
    }

    [Fact]
    public void Unknown_rawcode_returns_false_and_empty()
    {
        var store = BuildStore();
        Assert.False(store.TryGetUnit("xxxx", out var fields));
        Assert.Empty(fields);
    }

    [Fact]
    public void Missing_metadata_throws()
    {
        var src = new FakeSource(new());
        Assert.Throws<InvalidDataException>(() => BaseUnitStore.Build(src));
    }
}
