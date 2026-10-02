// tests/Wc3.Tests/MapFileEntryTests.cs
using Wc3.Model;
namespace Wc3.Tests;

public class MapFileEntryTests
{
    [Fact]
    public void Unparsed_entry_reports_not_parsed_and_not_dirty()
    {
        var e = new MapFileEntry { FileName = "x", BlockIndex = 0, RawBytes = new byte[] { 1 }, IsKnown = false };
        Assert.False(e.IsParsed);
        Assert.False(e.IsDirty);
    }

    [Fact]
    public void Setting_model_marks_parsed()
    {
        var e = new MapFileEntry { FileName = "x", BlockIndex = 0, RawBytes = System.Array.Empty<byte>(), IsKnown = true };
        e.Model = new object();
        Assert.True(e.IsParsed);
    }
}
