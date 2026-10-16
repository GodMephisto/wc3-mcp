// tests/Wc3.Tests/ListCommandTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// The listing that also types a protected map's nameless entries. The typing pass is warmed by
/// <see cref="MapDocument.PreSniffUnnamedEntries"/> before the projection reads the cache, so these
/// pin that every nameless entry comes back typed and no named entry is ever assigned a type.
/// </summary>
public class ListCommandTests
{
    private static byte[] Blp()
    {
        var b = new byte[64];
        Encoding.ASCII.GetBytes("BLP1").CopyTo(b, 0);
        return b;
    }

    [Fact]
    public void Typing_unnamed_entries_reports_a_type_for_every_nameless_entry()
    {
        // 70 crosses the parallel threshold in MapDocument, so the warm-up takes the per-worker
        // archive path and the projection must still see every verdict from the cache.
        var unnamed = Enumerable.Range(0, 70).Select(_ => Blp()).ToArray();
        var doc = MapDocument.Load(SyntheticMap.Build(
            new Dictionary<string, byte[]> { ["war3map.j"] = new byte[] { 1, 2, 3 } }, unnamed));

        var typed = ListCommand.Execute(doc, typeUnnamed: true);

        var nameless = typed.Files.Where(f => f.Name is null).ToList();
        Assert.Equal(70, nameless.Count);
        Assert.All(nameless, f => Assert.Equal("BLP texture", f.ContentType));
        Assert.All(typed.Files.Where(f => f.Name is not null), f => Assert.Null(f.ContentType));
    }

    [Fact]
    public void A_plain_listing_reports_no_content_type()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(
            new Dictionary<string, byte[]> { ["war3map.j"] = new byte[] { 1, 2, 3 } },
            new[] { Blp() }));

        var plain = ListCommand.Execute(doc);

        Assert.All(plain.Files, f => Assert.Null(f.ContentType));
    }
}
