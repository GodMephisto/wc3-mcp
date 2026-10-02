using Wc3.GameData;
namespace Wc3.Tests;
public class SlkTableTests
{
    // 2 cols (unitID, name), header row 1, one data row: hfoo, "Footman"
    private const string Slk =
        "ID;PWXL;N;E\r\nB;X2;Y2;D0\r\nC;X1;Y1;K\"unitID\"\r\nC;X2;K\"name\"\r\nC;X1;Y2;K\"hfoo\"\r\nC;X2;K\"Footman\"\r\nE\r\n";

    [Fact]
    public void Parses_headers_and_row_by_first_column()
    {
        var t = SlkTable.Parse(System.Text.Encoding.ASCII.GetBytes(Slk));
        Assert.Contains("unitid", t.Headers);
        Assert.Contains("name", t.Headers);
        Assert.True(t.TryGetRow("hfoo", out var row));
        Assert.Equal("Footman", row["name"]);
    }

    [Fact]
    public void Unknown_key_returns_false()
    {
        var t = SlkTable.Parse(System.Text.Encoding.ASCII.GetBytes(Slk));
        Assert.False(t.TryGetRow("zzzz", out _));
    }
}
