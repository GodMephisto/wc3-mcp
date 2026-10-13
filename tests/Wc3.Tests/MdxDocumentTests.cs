// tests/Wc3.Tests/MdxDocumentTests.cs
using Wc3.Modeling;

namespace Wc3.Tests;

public sealed class MdxDocumentTests
{
    [Fact]
    public void Round_trips_bytes_exactly()
    {
        var bytes = SyntheticModel.TinyMdx();
        var doc = MdxDocument.Parse(bytes);
        Assert.Equal(bytes, doc.Write());
    }

    [Fact]
    public void Rejects_bytes_without_the_mdlx_magic()
    {
        Assert.False(MdxDocument.HasMagic(new byte[] { 1, 2, 3, 4 }));
        Assert.Throws<InvalidDataException>(() => MdxDocument.Parse(new byte[] { 1, 2, 3, 4 }));
    }

    [Fact]
    public void Lists_the_geoset_chunks()
    {
        var doc = MdxDocument.Parse(SyntheticModel.TinyMdx());
        Assert.NotEmpty(doc.GeosetIndices());
        Assert.Contains(doc.Chunks, c => c.Tag == "GEOS");
    }
}
