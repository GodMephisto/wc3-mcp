using Wc3.Commands;
using Xunit;

namespace Wc3.Tests;

public class TriggerStringResolverTests
{
    [Fact]
    public void Resolve_KnownId_ReturnsMappedString()
    {
        var strings = new Dictionary<int, string> { [3] = "Gutsy Geoid" };
        Assert.Equal("Gutsy Geoid", TriggerStringResolver.Resolve("TRIGSTR_003", strings));
    }

    [Fact]
    public void Resolve_UnknownId_ReturnsRawUnchanged()
    {
        Assert.Equal("TRIGSTR_7", TriggerStringResolver.Resolve("TRIGSTR_7", new Dictionary<int, string>()));
    }

    [Fact]
    public void Resolve_NonTrigstrLiteral_ReturnsRawUnchanged()
    {
        var strings = new Dictionary<int, string> { [1] = "whatever" };
        Assert.Equal("Literal Name", TriggerStringResolver.Resolve("Literal Name", strings));
    }

    [Fact]
    public void Resolve_TrimsTrailingWhitespaceAndNewlines()
    {
        var strings = new Dictionary<int, string> { [1] = "Name\r\n" };
        Assert.Equal("Name", TriggerStringResolver.Resolve("TRIGSTR_1", strings));
    }

    [Fact]
    public void Resolve_LeadingZeros_ResolveToSameId()
    {
        var strings = new Dictionary<int, string> { [9] = "Nine" };
        Assert.Equal("Nine", TriggerStringResolver.Resolve("TRIGSTR_0009", strings));
    }

    [Fact]
    public void Resolve_NullStrings_ReturnsRawUnchanged()
    {
        Assert.Equal("TRIGSTR_3", TriggerStringResolver.Resolve("TRIGSTR_3", null));
    }
}
