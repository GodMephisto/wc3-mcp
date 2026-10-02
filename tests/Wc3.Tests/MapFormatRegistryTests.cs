// tests/Wc3.Tests/MapFormatRegistryTests.cs
using Wc3.Model;
namespace Wc3.Tests;

public class MapFormatRegistryTests
{
    [Theory]
    [InlineData("war3map.w3i")]
    [InlineData("war3map.w3e")]
    [InlineData("war3map.doo")]
    [InlineData("war3mapUnits.doo")]
    [InlineData("war3map.imp")]
    public void Known_formats_are_recognized(string name) => Assert.True(MapFormatRegistry.IsKnown(name));

    [Fact]
    public void Unknown_formats_are_not_recognized() => Assert.False(MapFormatRegistry.IsKnown("mystery.bin"));
}
