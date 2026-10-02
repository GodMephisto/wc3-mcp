namespace Wc3.Tests;

public class SmokeTest
{
    [Fact]
    public void War3Net_types_are_referencable()
    {
        var t = typeof(War3Net.IO.Mpq.MpqArchive);
        Assert.Equal("MpqArchive", t.Name);
    }
}
