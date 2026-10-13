// tests/Wc3.Tests/MdxGeosetTests.cs
using Wc3.Modeling;

namespace Wc3.Tests;

public sealed class MdxGeosetTests
{
    private static (MdxGeoset Geoset, byte[] Payload) Synthetic()
    {
        var doc = MdxDocument.Parse(SyntheticModel.TinyMdx());
        var geos = doc.Chunks.First(c => c.Tag == "GEOS").Payload;
        var geosetPayload = geos[4..]; // after the inclusiveSize field
        return (MdxGeoset.Parse(geosetPayload, 800), geosetPayload);
    }

    [Fact]
    public void A_geoset_round_trips_exactly()
    {
        var (geoset, payload) = Synthetic();
        Assert.Equal(payload, geoset.Write());
    }

    [Fact]
    public void The_expected_sub_chunks_are_present()
    {
        var (geoset, _) = Synthetic();
        var tags = geoset.Segments.Select(s => s.Tag).ToList();
        Assert.Contains("VRTX", tags);
        Assert.Contains("PVTX", tags);
        Assert.Contains("UVAS", tags);
        Assert.Contains("", tags); // the untagged section after MATS
    }

    [Fact]
    public void A_sub_chunk_can_be_replaced()
    {
        var (geoset, _) = Synthetic();
        var payload = new byte[4 + 12];
        BitConverter.TryWriteBytes(payload.AsSpan(0, 4), 1u); // count
        payload[4] = 9;
        geoset.Set("VRTX", payload);
        var bytes = geoset.Write();
        var reparsed = MdxGeoset.Parse(bytes, 800);
        Assert.Equal(payload, reparsed.Segments.First(s => s.Tag == "VRTX").Payload);
    }
}
