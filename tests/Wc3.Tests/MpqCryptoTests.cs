// tests/Wc3.Tests/MpqCryptoTests.cs
using Wc3.Commands;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// Pins MPQ's cipher against known answers, because getting it wrong does not fail loudly - it
/// returns plausible-looking noise. A hand-rolled version of this built the crypt table from the
/// HIGH 16 bits of each seed instead of the low ones, and the result was a decrypt that produced
/// roughly half the table set for every mutually exclusive flag. That read as a discovery about
/// the archive rather than as a broken decrypt, and it cost real debugging time.
/// </summary>
public class MpqCryptoTests
{
    [Fact]
    public void CryptTableMatchesKnownTableKeys()
    {
        // These are the canonical StormLib values for the two internal tables. If the crypt table
        // is built wrongly these will not match, and nothing downstream can be trusted.
        Assert.True(MpqCrypto.SelfCheck());
    }

    [Theory]
    [InlineData("(hash table)", 3, 0xC3AF3770u)]
    [InlineData("(block table)", 3, 0xEC83B3A3u)]
    public void HashStringReproducesCanonicalKeys(string name, int type, uint expected) =>
        Assert.Equal(expected, MpqCrypto.HashString(name, type));

    [Fact]
    public void HashStringIsCaseInsensitive() =>
        Assert.Equal(MpqCrypto.HashString("war3map.j", 3), MpqCrypto.HashString("WAR3MAP.J", 3));

    [Fact]
    public void HashStringTreatsForwardSlashAsBackslash()
    {
        // MPQ normalises separators, and an import table routinely spells a path with forward
        // slashes while the archive stores backslashes. Treating those as different names is how a
        // present file gets reported as absent.
        Assert.Equal(
            MpqCrypto.HashString(@"war3mapImported\thing.blp", 3),
            MpqCrypto.HashString("war3mapImported/thing.blp", 3));
    }

    [Fact]
    public void DifferentNamesHashDifferently() =>
        Assert.NotEqual(MpqCrypto.HashString("a.blp", 3), MpqCrypto.HashString("b.blp", 3));

    [Fact]
    public void DecryptIsDeterministicAndKeyDependent()
    {
        var payload = new byte[64];
        for (int i = 0; i < payload.Length; i++) payload[i] = (byte)(i * 7);

        var a = (byte[])payload.Clone();
        var b = (byte[])payload.Clone();
        var c = (byte[])payload.Clone();
        MpqCrypto.DecryptInPlace(a, 0x1234u);
        MpqCrypto.DecryptInPlace(b, 0x1234u);
        MpqCrypto.DecryptInPlace(c, 0x1235u);

        Assert.Equal(a, b);                 // same key, same result
        Assert.NotEqual(a, c);              // key actually participates
        Assert.NotEqual(payload, a);        // and something happened
    }

    [Fact]
    public void DecryptLeavesATrailingPartialWordAlone()
    {
        // Tables are whole 4-byte words; a stray tail must not be mangled or throw.
        var data = new byte[] { 1, 2, 3, 4, 9, 9 };
        MpqCrypto.DecryptInPlace(data, 0xABCDu);
        Assert.Equal(9, data[4]);
        Assert.Equal(9, data[5]);
    }
}
