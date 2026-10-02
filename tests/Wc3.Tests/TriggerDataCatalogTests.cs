using Wc3.GameData;
using Xunit;

namespace Wc3.Tests;

public class TriggerDataCatalogTests
{
    // A synthetic TriggerData.txt exercising every section + the tricky bits: a comment line,
    // a quoted script value containing a comma, a typed code with a base type, a Call with a
    // return type, an Event with a "nothing" (void) arg list, and meta lines (incl. one whose
    // name is a prefix of another, to prove exact-name association).
    private const string Sample = """
        // leading comment — ignored
        [TriggerCategories]
        TC_NOTHING=WESTRING_TRIGCAT_NOTHING,ReplaceableTextures\WorldEditUI\Actions-Nothing,1
        TC_UNSEL=WESTRING_X,icon\path,0

        [TriggerTypes]
        abilcode=0,1,1,WESTRING_TRIGTYPE_abilcode,integer
        boolean=0,1,1,WESTRING_TRIGTYPE_boolean

        [TriggerTypeDefaults]
        boolean=false,WESTRING_FALSE
        integer=0
        group=CreateGroup(),WESTRING_TRIGDEFAULT_GROUP

        [TriggerParams]
        OperatorAdd=0,ArithmeticOperator,"+",WESTRING_ARITH_ADD
        WithComma=1,string,"a,b",WESTRING_COMMA

        [TriggerEvents]
        MapInitializationEvent=0,nothing
        _MapInitializationEvent_DisplayName="Map Initialization"
        _MapInitializationEvent_Category=TC_NOTHING

        [TriggerConditions]
        OperatorCompareBoolean=0,boolean,EqualNotEqualOperator,boolean
        _OperatorCompareBoolean_DisplayName="Boolean Comparison"
        _OperatorCompareBoolean_Parameters=~Value," ",~Operator," ",~Value

        [TriggerActions]
        DoNothing=0,nothing
        _DoNothing_DisplayName="Do Nothing"
        _DoNothing_Defaults=_

        [TriggerCalls]
        ParseTags=1,1,string,string
        _ParseTags_DisplayName="Parse String with Tags"
        ParseTagsExtra=0,0,integer,unit
        _ParseTags_ShouldNotLeak="wrong entry"
        _ParseTagsExtra_Category=TC_UNSEL
        """;

    private static TriggerDataCatalog Parsed() => TriggerDataParser.Parse(Sample);

    [Fact]
    public void Parses_categories_with_selectable_flag()
    {
        var c = Parsed();
        Assert.Equal(2, c.Categories.Count);
        var nothing = Assert.Single(c.Categories, x => x.Name == "TC_NOTHING");
        Assert.Equal("WESTRING_TRIGCAT_NOTHING", nothing.DisplayNameKey);
        Assert.Equal(@"ReplaceableTextures\WorldEditUI\Actions-Nothing", nothing.IconPath);
        Assert.True(nothing.Selectable);
        Assert.False(Assert.Single(c.Categories, x => x.Name == "TC_UNSEL").Selectable);
    }

    [Fact]
    public void Parses_types_with_optional_base_type()
    {
        var c = Parsed();
        var abil = c.FindType("abilcode");
        Assert.NotNull(abil);
        Assert.True(abil!.CanBeParameter);
        Assert.True(abil.Displayed);
        Assert.Equal("integer", abil.BaseType);
        Assert.Null(c.FindType("boolean")!.BaseType);
    }

    [Fact]
    public void Parses_type_defaults_keeping_expression_before_westring()
    {
        var c = Parsed();
        Assert.Equal("false", c.TypeDefaults["boolean"]);
        Assert.Equal("0", c.TypeDefaults["integer"]);
        Assert.Equal("CreateGroup()", c.TypeDefaults["group"]);
    }

    [Fact]
    public void Parses_params_and_respects_quoted_commas()
    {
        var c = Parsed();
        var add = c.FindParam("OperatorAdd");
        Assert.NotNull(add);
        Assert.Equal("ArithmeticOperator", add!.TypeName);
        Assert.Equal("+", add.ScriptText);
        // The quoted "a,b" must survive as a single script value, not split into two fields.
        Assert.Equal("a,b", c.FindParam("WithComma")!.ScriptText);
    }

    [Fact]
    public void Event_with_nothing_arg_has_no_parameters()
    {
        var e = Parsed().FindFunction("MapInitializationEvent");
        Assert.NotNull(e);
        Assert.Equal(TriggerFunctionKind.Event, e!.Kind);
        Assert.False(e.HasParameters);
        Assert.Empty(e.ArgumentTypes);
        Assert.Equal("Map Initialization", e.DisplayName);
        Assert.Equal("TC_NOTHING", e.Category);
    }

    [Fact]
    public void Condition_implicitly_returns_boolean_and_keeps_arg_types()
    {
        var cond = Parsed().FindFunction("OperatorCompareBoolean");
        Assert.NotNull(cond);
        Assert.Equal(TriggerFunctionKind.Condition, cond!.Kind);
        Assert.Equal("boolean", cond.ReturnType);
        Assert.Equal(new[] { "boolean", "EqualNotEqualOperator", "boolean" }, cond.ArgumentTypes);
        Assert.Contains("~Operator", cond.ParametersLayout);
    }

    [Fact]
    public void Call_parses_usableInEvents_returnType_and_args()
    {
        var call = Parsed().FindFunction("ParseTags");
        Assert.NotNull(call);
        Assert.Equal(TriggerFunctionKind.Call, call!.Kind);
        Assert.Equal(1, call.GameVersion);
        Assert.True(call.UsableInEvents);
        Assert.Equal("string", call.ReturnType);
        Assert.Equal(new[] { "string" }, call.ArgumentTypes);
        Assert.Equal("Parse String with Tags", call.DisplayName);
    }

    [Fact]
    public void Meta_associates_only_with_exact_owner_not_a_prefix_sibling()
    {
        var c = Parsed();
        // "_ParseTags_ShouldNotLeak" must not attach to ParseTags (unknown field anyway),
        // and "_ParseTagsExtra_Category" must attach to ParseTagsExtra, not ParseTags.
        Assert.Null(c.FindFunction("ParseTags")!.Category);
        Assert.Equal("TC_UNSEL", c.FindFunction("ParseTagsExtra")!.Category);
        Assert.False(c.FindFunction("ParseTagsExtra")!.UsableInEvents);
    }

    [Fact]
    public void Malformed_lines_are_skipped_not_thrown()
    {
        // No '=', stray bracket, empty key — all tolerated.
        var c = TriggerDataParser.Parse("[TriggerActions]\nno equals here\n=orphan\nGood=0,unit\n");
        Assert.NotNull(c.FindFunction("Good"));
        Assert.Single(c.Functions);
    }

    [Fact]
    public void Real_install_triggerdata_parses_into_a_rich_catalog()
    {
        if (!Wc3.GameData.GameData.TryOpen(null, out var ctx, out _) || ctx is null)
            return; // no install on this machine — integration check skipped
        if (!ctx.TryReadFile(@"ui\triggerdata.txt", out var bytes))
            return;

        var text = System.Text.Encoding.UTF8.GetString(bytes);
        var cat = TriggerDataParser.Parse(text);

        // The real file is large; assert we found a healthy population in each section and that
        // some well-known entries resolve with the expected shape.
        Assert.True(cat.Categories.Count > 10);
        Assert.True(cat.Types.Count > 30);
        Assert.True(cat.Params.Count > 100);
        Assert.True(cat.Functions.Count > 500);

        var doNothing = cat.FindFunction("DoNothing");
        Assert.NotNull(doNothing);
        Assert.Equal(TriggerFunctionKind.Action, doNothing!.Kind);
        Assert.False(doNothing.HasParameters);

        Assert.NotNull(cat.FindFunction("MapInitializationEvent"));
        Assert.Equal("integer", cat.FindType("abilcode")?.BaseType);
    }
}
