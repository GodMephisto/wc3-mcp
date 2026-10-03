// tests/Wc3.Tests/MapFilePresenceTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// Three catalog panels told the user a cause they could not know. The cameras panel said "Its
/// war3map.w3c holds a version and a count of zero, so there is nothing to list", and the regions
/// panel said the same of war3map.w3r, when the command layer returns an empty list for an ABSENT
/// file and for an UNREADABLE one too. MapDocument records a parse failure as a diagnostic and
/// leaves Model null rather than throwing, so from a count alone the three cases are identical.
///
/// A user whose map is damaged was therefore told their map was fine, which is worse than saying
/// nothing because it stops the investigation. These pin that the three cases now read
/// differently.
/// </summary>
public class MapFilePresenceTests
{
    private static MapDocument With(params (string Name, byte[] Bytes)[] files) =>
        MapDocument.Load(SyntheticMap.Build(
            files.ToDictionary(f => f.Name, f => f.Bytes)));

    [Fact]
    public void An_absent_file_says_absent()
    {
        var doc = With(("war3map.j", Encoding.Latin1.GetBytes("// nothing here")));

        Assert.Equal(MapFileState.Absent, MapFilePresence.Of(doc, "war3map.w3c"));
        var text = MapFilePresence.Describe(doc, "war3map.w3c", "cameras");
        Assert.Contains("no war3map.w3c", text);
        Assert.DoesNotContain("count of zero", text);
    }

    [Fact]
    public void A_present_but_unparseable_file_says_unknown_rather_than_none()
    {
        // Bytes that are not a valid w3c. The loader keeps the entry, records a diagnostic and
        // leaves Model null, which is exactly the state that used to be reported as emptiness.
        var doc = With(
            ("war3map.j", Encoding.Latin1.GetBytes("// nothing here")),
            ("war3map.w3c", new byte[] { 0xFF, 0xFE, 0xFD, 0xFC, 0x01, 0x02, 0x03 }));

        Assert.Equal(MapFileState.Unreadable, MapFilePresence.Of(doc, "war3map.w3c"));
        var text = MapFilePresence.Describe(doc, "war3map.w3c", "cameras");
        Assert.Contains("could not be read", text);
        Assert.Contains("unknown rather than absent", text);
    }

    [Fact]
    public void A_readable_but_empty_file_may_say_so_confidently()
    {
        // The only case where a definite claim is honest. A real empty w3c is a version and a
        // count of zero, which is what the old message asserted for all three cases.
        var w3c = new byte[8];
        BitConverter.GetBytes(0).CopyTo(w3c, 0);   // format version
        BitConverter.GetBytes(0).CopyTo(w3c, 4);   // camera count

        var doc = With(
            ("war3map.j", Encoding.Latin1.GetBytes("// nothing here")),
            ("war3map.w3c", w3c));

        Assert.Equal(MapFileState.Empty, MapFilePresence.Of(doc, "war3map.w3c"));
        var text = MapFilePresence.Describe(doc, "war3map.w3c", "cameras", "Most maps never define one.");
        Assert.Contains("was read and defines no cameras", text);
        Assert.Contains("Most maps never define one.", text);
    }

    [Fact]
    public void The_three_descriptions_are_actually_different()
    {
        // The whole point is that a user can tell them apart. If two ever collapsed to the same
        // sentence the fix would be undone without any test noticing.
        var absent = With(("war3map.j", Encoding.Latin1.GetBytes("//")));
        var broken = With(("war3map.j", Encoding.Latin1.GetBytes("//")),
                          ("war3map.w3c", new byte[] { 0xFF, 0xFE, 0xFD, 0xFC, 9, 9, 9 }));
        var empty = With(("war3map.j", Encoding.Latin1.GetBytes("//")),
                         ("war3map.w3c", new byte[8]));

        var texts = new[]
        {
            MapFilePresence.Describe(absent, "war3map.w3c", "cameras"),
            MapFilePresence.Describe(broken, "war3map.w3c", "cameras"),
            MapFilePresence.Describe(empty, "war3map.w3c", "cameras"),
        };
        Assert.Equal(3, texts.Distinct(StringComparer.Ordinal).Count());
    }
}
