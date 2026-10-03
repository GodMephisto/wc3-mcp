// tests/Wc3.Tests/ScriptEncodingTests.cs
using System.Text;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// Pins that a map script is decoded byte-preservingly.
///
/// A war3map.j is a byte stream with no declared encoding, and real maps carry bytes that are not
/// valid UTF-8 (one 246 MB map on hand has about 52,000 of them, author names and localised
/// strings). Decoding those as UTF-8 turns each into U+FFFD, and the damage is silent: nothing
/// throws, nothing logs, and a later text scan is simply looking at content the map does not
/// contain. This exact family of bug bit three separate places in this project, and in two of them
/// the symptom pointed somewhere else entirely (a lint check failing on untouched maps, and an
/// imported asset appearing to be missing when only its NAME had been mangled).
/// </summary>
public class ScriptEncodingTests
{
    // The default parsers are registered by MapDocument's static constructor, so a test that only
    // touches MapFormatRegistry would find an empty registry. Touching MapDocument once forces it.
    static ScriptEncodingTests() => DefaultParsers.RegisterDefaults();

    /// <summary>A byte sequence that is not valid UTF-8, so a wrong decode is detectable.</summary>
    private static byte[] ScriptWithHighBytes()
    {
        var prefix = Encoding.ASCII.GetBytes("function main takes nothing returns nothing\n    call BJDebugMsg(\"");
        var suffix = Encoding.ASCII.GetBytes("\")\nendfunction\n");
        // 0x92 and 0xE9 are legal single bytes in a WC3 script and illegal as standalone UTF-8.
        var middle = new byte[] { 0x92, 0xE9, 0xFF, 0x80 };
        return prefix.Concat(middle).Concat(suffix).ToArray();
    }

    [Fact]
    public void ScriptParserPreservesEveryByte()
    {
        var raw = ScriptWithHighBytes();
        Assert.True(MapFormatRegistry.TryGetParser("war3map.j", out var parse));

        var text = Assert.IsType<string>(parse!(raw));

        // Round-tripping through Latin-1 must return the original bytes exactly. Under UTF-8 the
        // four high bytes collapse to replacement characters and this comparison fails.
        Assert.Equal(raw, Encoding.Latin1.GetBytes(text));
        Assert.DoesNotContain('�', text);
    }

    [Fact]
    public void LuaScriptParserPreservesEveryByte()
    {
        var raw = ScriptWithHighBytes();
        Assert.True(MapFormatRegistry.TryGetParser("war3map.lua", out var parse));
        var text = Assert.IsType<string>(parse!(raw));
        Assert.Equal(raw, Encoding.Latin1.GetBytes(text));
    }

    [Fact]
    public void EveryByteValueSurvivesTheRoundTrip()
    {
        // All 256 byte values, because a decode that is lossy for any one of them is lossy for a
        // real map: this is what "byte-faithful" has to mean at the text layer.
        var raw = Enumerable.Range(0, 256).Select(i => (byte)i).ToArray();
        Assert.Equal(raw, Encoding.Latin1.GetBytes(Encoding.Latin1.GetString(raw)));
    }

    [Fact]
    public void Utf8WouldHaveLostThoseBytes()
    {
        // Documents WHY Latin-1 is required rather than merely asserting it, so a future change
        // back to UTF-8 fails with an explanation rather than a bare inequality.
        var raw = ScriptWithHighBytes();
        Assert.NotEqual(raw, Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(raw)));
    }
}
