using Wc3.GameData;
namespace Wc3.Tests;

public class UnitMetadataTests
{
    // Real unitmetadata.slk shape (confirmed against the live install):
    //   row key = "ID" column = the 4-char field code (matches w3u mod codes),
    //   "field"       = the column name inside the data SLK,
    //   "slk"         = which data SLK holds the value (e.g. UnitBalance, Profile),
    //   "displayName" = WESTRING key.
    private const string Meta =
        "ID;PWXL;N;E\r\n" +
        "C;X1;Y1;K\"ID\"\r\nC;X2;K\"field\"\r\nC;X3;K\"slk\"\r\nC;X4;K\"displayName\"\r\n" +
        "C;X1;Y2;K\"uhpm\"\r\nC;X2;K\"HP\"\r\nC;X3;K\"UnitBalance\"\r\nC;X4;K\"WESTRING_UEVAL_UHPM\"\r\n" +
        "C;X1;Y3;K\"unam\"\r\nC;X2;K\"Name\"\r\nC;X3;K\"Profile\"\r\nC;X4;K\"WESTRING_UEVAL_UNAM\"\r\nE\r\n";

    [Fact]
    public void Maps_field_code_to_slk_column_and_display()
    {
        var m = UnitMetadata.FromSlk(SlkTable.Parse(System.Text.Encoding.ASCII.GetBytes(Meta)));

        Assert.True(m.TryGet("uhpm", out var hp));
        Assert.Equal("uhpm", hp.Code);
        Assert.Equal("UnitBalance", hp.SlkName);
        Assert.Equal("HP", hp.Column);
        Assert.Equal("WESTRING_UEVAL_UHPM", hp.DisplayName);

        Assert.True(m.TryGet("unam", out var nam));
        Assert.Equal("Name", nam.Column);
        Assert.Equal("Profile", nam.SlkName);

        Assert.Equal(2, m.Fields.Count);
        Assert.False(m.TryGet("zzzz", out _));
    }

    [Fact]
    public void Lookup_is_case_insensitive()
    {
        var m = UnitMetadata.FromSlk(SlkTable.Parse(System.Text.Encoding.ASCII.GetBytes(Meta)));
        Assert.True(m.TryGet("UHPM", out var hp));
        Assert.Equal("uhpm", hp.Code);
    }
}
