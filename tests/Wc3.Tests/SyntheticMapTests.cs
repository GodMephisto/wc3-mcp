// tests/Wc3.Tests/SyntheticMapTests.cs
using Wc3.Model;
namespace Wc3.Tests;

public class SyntheticMapTests
{
    [Fact]
    public void Built_file_has_mpq_at_offset_0x200()
    {
        var bytes = SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3i"] = new byte[] { 1, 2, 3 },
            ["readme.txt"] = new byte[] { 9 },
        });
        Assert.Equal(0x200, MpqHeader.FindArchiveOffset(bytes));
    }
}
