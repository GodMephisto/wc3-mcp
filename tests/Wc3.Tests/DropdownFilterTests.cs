using Wc3.Commands;

namespace Wc3.Tests;

/// <summary>
/// Pure-logic tests for DropdownFilter, the name+id matching/ranking behind
/// Studio's searchable dropdowns (SearchableComboBox and the object editor's
/// list search).
/// </summary>
public class DropdownFilterTests
{
    [Fact]
    public void Matches_NameSubstring_True()
    {
        Assert.True(DropdownFilter.Matches("ootma", "Footman", "hfoo"));
    }

    [Fact]
    public void Matches_IdSubstring_True()
    {
        Assert.True(DropdownFilter.Matches("foo", "Knight", "hfoo"));
    }

    [Fact]
    public void Matches_CaseInsensitive_OnNameAndId()
    {
        Assert.True(DropdownFilter.Matches("FOOTMAN", "footman", "hfoo"));
        Assert.True(DropdownFilter.Matches("HFOO", "Footman", "hfoo"));
    }

    [Fact]
    public void Matches_EmptyQuery_MatchesAll()
    {
        Assert.True(DropdownFilter.Matches("", "Footman", "hfoo"));
        Assert.True(DropdownFilter.Matches("   ", "Footman", "hfoo"));
        Assert.True(DropdownFilter.Matches(null, "Footman", "hfoo"));
        Assert.True(DropdownFilter.Matches("", null, null));
    }

    [Fact]
    public void Matches_NoMatch_False()
    {
        Assert.False(DropdownFilter.Matches("zzz", "Footman", "hfoo"));
    }

    [Fact]
    public void Matches_TrimsQueryWhitespace()
    {
        Assert.True(DropdownFilter.Matches("  Foot  ", "Footman", "hfoo"));
        Assert.True(DropdownFilter.Matches("\thfoo\n", "Footman", "hfoo"));
    }

    [Fact]
    public void Matches_NullNameOrId_NoThrow()
    {
        Assert.True(DropdownFilter.Matches("foo", null, "hfoo"));
        Assert.True(DropdownFilter.Matches("foo", "Footman", null));
        Assert.False(DropdownFilter.Matches("foo", null, null));
    }

    [Fact]
    public void Rank_ExactIdBeforeExactNameBeforePrefixBeforeSubstring()
    {
        var exactId = DropdownFilter.Rank("hfoo", "Footman", "hfoo");
        var exactName = DropdownFilter.Rank("hfoo", "hfoo", "u001");
        var namePrefix = DropdownFilter.Rank("hfoo", "hfoo the Brave", "u002");
        var idPrefix = DropdownFilter.Rank("hfoo", "Knight", "hfoo2");
        var nameSub = DropdownFilter.Rank("hfoo", "The hfoo", "u003");
        var idSub = DropdownFilter.Rank("hfoo", "Peasant", "xhfoo");
        var noMatch = DropdownFilter.Rank("hfoo", "Militia", "hmil");

        Assert.True(exactId < exactName);
        Assert.True(exactName < namePrefix);
        Assert.True(namePrefix < idPrefix);
        Assert.True(idPrefix < nameSub);
        Assert.True(nameSub < idSub);
        Assert.Equal(int.MaxValue, noMatch);
    }

    [Fact]
    public void Rank_EmptyQuery_RanksEverythingEqual()
    {
        Assert.Equal(0, DropdownFilter.Rank("", "Footman", "hfoo"));
        Assert.Equal(0, DropdownFilter.Rank("  ", "Knight", "hkni"));
        Assert.Equal(0, DropdownFilter.Rank(null, null, null));
    }

    [Fact]
    public void Rank_CaseInsensitive()
    {
        Assert.Equal(
            DropdownFilter.Rank("hfoo", "Footman", "hfoo"),
            DropdownFilter.Rank("HFOO", "Footman", "hfoo"));
    }

    [Fact]
    public void Rank_SortsBestMatchFirst_TiesKeepOriginalOrder()
    {
        var items = new[]
        {
            (Name: "Footman", Id: "hfoo"),
            (Name: "Foot Messenger", Id: "u000"),
            (Name: "Knight", Id: "foot"),
            (Name: "Militia", Id: "hmil"),
        };

        var ordered = items
            .Where(i => DropdownFilter.Matches("foot", i.Name, i.Id))
            .OrderBy(i => DropdownFilter.Rank("foot", i.Name, i.Id)) // LINQ OrderBy is stable
            .Select(i => i.Id)
            .ToArray();

        // Exact id first, then the two name-prefix hits in their original order;
        // the non-match is filtered out entirely.
        Assert.Equal(new[] { "foot", "hfoo", "u000" }, ordered);
    }
}
