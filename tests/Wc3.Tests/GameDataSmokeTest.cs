namespace Wc3.Tests;

public class GameDataSmokeTest
{
    [Fact]
    public void CascLib_type_is_referencable()
        => Assert.Equal("CascStorage", typeof(CascLib.NET.CascStorage).Name);
}
