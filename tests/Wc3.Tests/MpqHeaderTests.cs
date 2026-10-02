// tests/Wc3.Tests/MpqHeaderTests.cs
using Wc3.Model;
namespace Wc3.Tests;

public class MpqHeaderTests
{
    private static readonly byte[] Magic = { (byte)'M', (byte)'P', (byte)'Q', 0x1A };

    [Fact]
    public void Finds_offset_zero_when_no_preheader()
    {
        var bytes = new byte[1024];
        Magic.CopyTo(bytes, 0);
        Assert.Equal(0, MpqHeader.FindArchiveOffset(bytes));
    }

    [Fact]
    public void Finds_offset_after_512_byte_wc3_header()
    {
        var bytes = new byte[2048];
        bytes[0] = (byte)'H'; bytes[1] = (byte)'M'; bytes[2] = (byte)'3'; bytes[3] = (byte)'W';
        Magic.CopyTo(bytes, 0x200);
        Assert.Equal(0x200, MpqHeader.FindArchiveOffset(bytes));
    }

    [Fact]
    public void Returns_minus_one_when_absent()
    {
        Assert.Equal(-1, MpqHeader.FindArchiveOffset(new byte[600]));
    }
}
