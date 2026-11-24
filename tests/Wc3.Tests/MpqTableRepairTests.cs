namespace Wc3.Tests;

using Wc3.Model;
using Xunit;

/// <summary>
/// The decision rule behind the hash table repair, on the decrypted words, with no archive and no
/// encryption involved. One entry is four words, name1, name2, locale and platform, block index.
/// The end to end proof lives on the 350 MB BWOS map, where three deleted entries turned a
/// MAP_UNREADABLE into a loaded document.
/// </summary>
public sealed class MpqTableRepairTests
{
    private const uint Empty = 0xFFFFFFFF;
    private const uint Deleted = 0xFFFFFFFE;

    private static uint[] Entry(uint name1, uint name2, uint locale, uint block)
        => new[] { name1, name2, locale, block };

    [Fact]
    public void An_entry_that_names_a_block_is_left_alone()
    {
        uint[] words = Entry(0x1111, 0x2222, 0, 5);

        int cleared = MpqTableRepair.Normalize(words, blockCount: 8);

        Assert.Equal(0, cleared);
        Assert.Equal(new uint[] { 0x1111, 0x2222, 0, 5 }, words);
    }

    [Fact]
    public void An_empty_entry_is_left_alone()
    {
        uint[] words = Entry(Empty, Empty, Empty, Empty);

        int cleared = MpqTableRepair.Normalize(words, blockCount: 8);

        Assert.Equal(0, cleared);
    }

    [Fact]
    public void A_deleted_entry_is_written_as_empty()
    {
        uint[] words = Entry(0xAAAA, 0xBBBB, 0, Deleted);

        int cleared = MpqTableRepair.Normalize(words, blockCount: 8);

        Assert.Equal(1, cleared);
        Assert.Equal(new uint[] { Empty, Empty, Empty, Empty }, words);
    }

    [Fact]
    public void An_entry_past_the_block_table_is_written_as_empty()
    {
        uint[] words = Entry(0xAAAA, 0xBBBB, 0, 9);

        int cleared = MpqTableRepair.Normalize(words, blockCount: 8);

        Assert.Equal(1, cleared);
        Assert.Equal(Empty, words[3]);
    }

    [Fact]
    public void Running_the_repair_twice_changes_nothing_the_second_time()
    {
        uint[] words = Entry(0xAAAA, 0xBBBB, 0, Deleted);
        MpqTableRepair.Normalize(words, blockCount: 8);

        int again = MpqTableRepair.Normalize(words, blockCount: 8);

        Assert.Equal(0, again);
    }

    [Fact]
    public void Five_entries_are_each_judged_on_their_own()
    {
        uint[] words = new uint[4 * 5];
        uint[] cases = { 0, Deleted, Empty, 3, 99 };
        for (int i = 0; i < cases.Length; i++)
        {
            words[i * 4] = 0x1234;
            words[i * 4 + 3] = cases[i];
        }

        int cleared = MpqTableRepair.Normalize(words, blockCount: 8);

        Assert.Equal(2, cleared);
        Assert.Equal(0x1234u, words[0]);
        Assert.Equal(Empty, words[4]);
        Assert.Equal(0x1234u, words[8]);
        Assert.Equal(0x1234u, words[12]);
        Assert.Equal(Empty, words[16]);
    }

    [Fact]
    public void Bytes_that_are_not_an_archive_are_refused()
    {
        Assert.Equal(0, MpqTableRepair.TryNormalize(new byte[] { 1, 2, 3, 4 }));
        Assert.Equal(0, MpqTableRepair.TryNormalize(null!));
    }
}
