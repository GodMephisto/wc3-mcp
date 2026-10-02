// tests/Wc3.Tests/MapDocumentParseTests.cs
using Wc3.Model;
namespace Wc3.Tests;

public class MapDocumentParseTests
{
    [Fact]
    public void Parser_that_throws_degrades_to_raw_with_diagnostic()
    {
        // The registry is process-global; restore the real parsers afterwards
        // so other tests stay order-independent.
        try
        {
            MapFormatRegistry.Register("war3map.w3i", _ => throw new InvalidDataException("boom"));
            var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
            {
                ["war3map.w3i"] = new byte[] { 1, 2, 3 },
            }));

            var entry = doc.GetFile("war3map.w3i")!;
            Assert.False(entry.IsParsed);
            Assert.NotEmpty(entry.RawBytes);
            Assert.Contains(doc.Diagnostics, d => d.FileName == "war3map.w3i" && d.Severity == DiagnosticSeverity.Warning);
        }
        finally
        {
            DefaultParsers.Reregister();
        }
    }
}
