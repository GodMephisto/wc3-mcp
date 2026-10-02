// tests/Wc3.Tests/JassFunctionIndexTests.cs
using Wc3.Model;
namespace Wc3.Tests;

public class JassFunctionIndexTests
{
    [Fact]
    public void Parses_functions_with_names_and_line_ranges()
    {
        var jass = string.Join("\n",
            "function A takes nothing returns nothing",
            "    call B()",
            "endfunction",
            "function Bar takes integer x returns integer",
            "    return x",
            "endfunction");
        var fns = JassFunctionIndex.Parse(jass);
        Assert.Equal(2, fns.Count);
        Assert.Equal("A", fns[0].Name);
        Assert.Equal(1, fns[0].StartLine);
        Assert.Equal(3, fns[0].EndLine);
        Assert.Equal("function A takes nothing returns nothing", fns[0].Signature);
        Assert.Equal("Bar", fns[1].Name);
        Assert.Equal(4, fns[1].StartLine);
        Assert.Equal(6, fns[1].EndLine);
        Assert.Equal("function Bar takes integer x returns integer", fns[1].Signature);
    }

    [Fact]
    public void Crlf_input_parses_the_same()
    {
        var jass = "function A takes nothing returns nothing\r\nendfunction\r\n";
        var f = Assert.Single(JassFunctionIndex.Parse(jass));
        Assert.Equal("A", f.Name);
        Assert.Equal(1, f.StartLine);
        Assert.Equal(2, f.EndLine);
    }

    [Fact]
    public void Function_interface_is_not_reported()
    {
        var fns = JassFunctionIndex.Parse("function interface Xyz takes nothing returns nothing\n");
        Assert.Empty(fns);
    }

    [Fact]
    public void Commented_declaration_is_ignored()
    {
        var jass = string.Join("\n",
            "// function Ghost takes nothing returns nothing",
            "function Real takes nothing returns nothing",
            "endfunction");
        var f = Assert.Single(JassFunctionIndex.Parse(jass));
        Assert.Equal("Real", f.Name);
        Assert.Equal(2, f.StartLine);
        Assert.Equal(3, f.EndLine);
    }

    [Fact]
    public void Empty_or_functionless_input_yields_empty_list()
    {
        Assert.Empty(JassFunctionIndex.Parse(""));
        Assert.Empty(JassFunctionIndex.Parse("call Foo()\n// nothing here\nendfunction"));
    }
}
