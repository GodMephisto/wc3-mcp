using Wc3.Commands;
using Xunit;

namespace Wc3.Tests;

public class TriggerCatalogCommandTests : IDisposable
{
    private readonly string _file;

    public TriggerCatalogCommandTests()
    {
        _file = Path.Combine(Path.GetTempPath(), $"triggerdata-{Guid.NewGuid():N}.txt");
        File.WriteAllText(_file, """
            [TriggerTypes]
            boolean=0,1,1,WESTRING_BOOL

            [TriggerEvents]
            MapInitializationEvent=0,nothing
            _MapInitializationEvent_DisplayName="Map Initialization"
            _MapInitializationEvent_Category=TC_NOTHING

            [TriggerActions]
            DoNothing=0,nothing
            _DoNothing_DisplayName="Do Nothing"

            [TriggerConditions]
            OperatorCompareBoolean=0,boolean,EqualNotEqualOperator,boolean
            _OperatorCompareBoolean_DisplayName="Boolean Comparison"

            [TriggerCalls]
            ParseTags=1,1,string,string
            _ParseTags_DisplayName="Parse String with Tags"
            """);
    }

    public void Dispose() { try { File.Delete(_file); } catch { /* best effort */ } }

    [Fact]
    public void Load_from_file_reports_source_and_parses()
    {
        var cat = TriggerCatalogCommand.Load(_file, gameDir: null, out var source);
        Assert.Equal(_file, source);
        Assert.NotNull(cat.FindFunction("DoNothing"));
    }

    [Fact]
    public void Load_missing_file_throws_actionable()
    {
        var ex = Assert.Throws<InvalidOperationException>(
            () => TriggerCatalogCommand.Load(@"Z:\nope\triggerdata.txt", null, out _));
        Assert.Contains("not found", ex.Message);
    }

    [Fact]
    public void List_unfiltered_returns_all_sorted_by_name()
    {
        var cat = TriggerCatalogCommand.Load(_file, null, out var src);
        var res = TriggerCatalogCommand.List(cat, src, kind: null, search: null);
        Assert.Equal(4, res.Total);
        Assert.Equal(src, res.Source);
        var names = res.Functions.Select(f => f.Name).ToList();
        Assert.Equal(names.OrderBy(n => n, StringComparer.OrdinalIgnoreCase), names);
    }

    [Fact]
    public void List_kind_filter_is_case_insensitive()
    {
        var cat = TriggerCatalogCommand.Load(_file, null, out var src);
        var acts = TriggerCatalogCommand.List(cat, src, kind: "ACTION", search: null);
        Assert.Equal(1, acts.Total);
        Assert.Equal("DoNothing", acts.Functions[0].Name);
        Assert.Equal("Action", acts.Functions[0].Kind);
    }

    [Fact]
    public void List_search_matches_name_or_display_name()
    {
        var cat = TriggerCatalogCommand.Load(_file, null, out var src);
        // "comparison" appears only in the display name of OperatorCompareBoolean.
        var res = TriggerCatalogCommand.List(cat, src, kind: null, search: "comparison");
        Assert.Equal(1, res.Total);
        Assert.Equal("OperatorCompareBoolean", res.Functions[0].Name);
    }

    [Fact]
    public void List_unknown_kind_throws()
    {
        var cat = TriggerCatalogCommand.Load(_file, null, out var src);
        var ex = Assert.Throws<InvalidOperationException>(
            () => TriggerCatalogCommand.List(cat, src, kind: "banana", search: null));
        Assert.Contains("Unknown kind", ex.Message);
    }

    [Fact]
    public void Describe_returns_detail_for_a_call()
    {
        var cat = TriggerCatalogCommand.Load(_file, null, out _);
        var d = TriggerCatalogCommand.Describe(cat, "ParseTags");
        Assert.NotNull(d);
        Assert.Equal("Call", d!.Kind);
        Assert.True(d.UsableInEvents);
        Assert.Equal("string", d.ReturnType);
        Assert.Equal(new[] { "string" }, d.ArgumentTypes);
        Assert.Equal("Parse String with Tags", d.DisplayName);
    }

    [Fact]
    public void Describe_missing_returns_null()
    {
        var cat = TriggerCatalogCommand.Load(_file, null, out _);
        Assert.Null(TriggerCatalogCommand.Describe(cat, "NoSuchFunction"));
    }
}
