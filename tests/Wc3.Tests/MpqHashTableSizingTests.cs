// tests/Wc3.Tests/MpqHashTableSizingTests.cs
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// A rebuilt archive re-places every entry in the MPQ hash table, which resolves names by
/// linear probing. Inheriting a nearly-full table therefore produces probe chains the
/// original never had. The map still hosts (a lobby reads only a handful of files) and then
/// stalls forever on the loading screen, and no content check catches it because not one
/// file's bytes changed. GGGA ships at 96.5% occupancy and reproduced exactly that.
/// </summary>
public class MpqHashTableSizingTests
{
    [Theory]
    [InlineData(8192, 0)]      // empty
    [InlineData(8192, 1)]      // barely used
    [InlineData(8192, 4096)]   // half full
    [InlineData(8192, 6144)]   // exactly at the 3/4 threshold
    [InlineData(256, 140)]     // a map of ours that loads fine today
    public void LeavesSafeTablesAlone(uint originalSize, int fileCount)
    {
        // Null means "keep the source's own size". Resizing an archive that was never at
        // risk would change its bytes and cost the byte-faithful guarantee for nothing.
        Assert.Null(MapDocument.GrownHashTableSize(originalSize, fileCount));
    }

    [Theory]
    [InlineData(8192, 6145, 16384)]   // one entry past the threshold
    [InlineData(8192, 7906, 16384)]   // GGGA as shipped, 96.5%
    [InlineData(8192, 8087, 16384)]   // GGGA carrying the ported hero, 98.7%
    [InlineData(8192, 20000, 32768)]  // needs more than one doubling
    public void GrowsTablesThatWouldBeUnsafeAfterRebuild(uint originalSize, int fileCount, ushort expected)
    {
        Assert.Equal(expected, MapDocument.GrownHashTableSize(originalSize, fileCount));
    }

    [Fact]
    public void GrownTableLeavesTheArchiveAtMostThreeQuartersFull()
    {
        var grown = MapDocument.GrownHashTableSize(8192, 8087);
        Assert.NotNull(grown);
        Assert.True(8087 * 4 <= grown!.Value * 3,
            $"8087 entries in {grown} slots is still above the 3/4 occupancy ceiling.");
    }

    [Fact]
    public void KeepsPowerOfTwoSizing()
    {
        // MPQ requires a power-of-two table; a non-power-of-two size corrupts name lookup.
        var grown = MapDocument.GrownHashTableSize(8192, 8087);
        Assert.NotNull(grown);
        Assert.Equal(0, grown!.Value & (grown.Value - 1));
    }

    [Fact]
    public void DoesNotExceedTheFormatCeilingWhenAlreadyAtIt()
    {
        // 32768 is the largest power of two War3Net's ushort HashTableSize can express, so
        // there is nothing to grow into. Leave it rather than emit an unrepresentable size.
        Assert.Null(MapDocument.GrownHashTableSize(32768, 999_999));
    }

    [Fact]
    public void TreatsAnUnreadableHeaderAsNothingToDo()
    {
        // OriginalHashTableSize returns 0 when the MPQ magic is not where it should be.
        Assert.Null(MapDocument.GrownHashTableSize(0, 8087));
    }
}
