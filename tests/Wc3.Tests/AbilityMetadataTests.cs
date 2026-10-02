using Wc3.GameData;
namespace Wc3.Tests;

public class AbilityMetadataTests
{
    // Real abilitymetadata.slk shape (confirmed against the live install, same as units):
    //   row key = "ID" column = the 4-char field code (matches w3a mod codes),
    //   "field"       = the column name inside the data SLK,
    //   "slk"         = which data SLK holds the value (AbilityData, or Profile),
    //   "displayName" = WESTRING key.
    private const string Meta =
        "ID;PWXL;N;E\r\n" +
        "C;X1;Y1;K\"ID\"\r\nC;X2;K\"field\"\r\nC;X3;K\"slk\"\r\nC;X4;K\"displayName\"\r\n" +
        "C;X1;Y2;K\"aher\"\r\nC;X2;K\"hero\"\r\nC;X3;K\"AbilityData\"\r\nC;X4;K\"WESTRING_AEVAL_AHER\"\r\n" +
        "C;X1;Y3;K\"anam\"\r\nC;X2;K\"Name\"\r\nC;X3;K\"Profile\"\r\nC;X4;K\"WESTRING_AEVAL_ANAM\"\r\nE\r\n";

    [Fact]
    public void Maps_field_code_to_slk_column_and_display()
    {
        var m = AbilityMetadata.FromSlk(SlkTable.Parse(System.Text.Encoding.ASCII.GetBytes(Meta)));

        Assert.True(m.TryGet("aher", out var hero));
        Assert.Equal("aher", hero.Code);
        Assert.Equal("AbilityData", hero.SlkName);
        Assert.Equal("hero", hero.Column);
        Assert.Equal("WESTRING_AEVAL_AHER", hero.DisplayName);

        Assert.True(m.TryGet("anam", out var nam));
        Assert.Equal("Name", nam.Column);
        Assert.Equal("Profile", nam.SlkName);

        Assert.Equal(2, m.Fields.Count);
        Assert.False(m.TryGet("zzzz", out _));
    }

    [Fact]
    public void Lookup_is_case_insensitive()
    {
        var m = AbilityMetadata.FromSlk(SlkTable.Parse(System.Text.Encoding.ASCII.GetBytes(Meta)));
        Assert.True(m.TryGet("AHER", out var hero));
        Assert.Equal("aher", hero.Code);
    }
}
