// src/Wc3.Commands/ConversionNotes.cs
namespace Wc3.Commands;

/// <summary>One JASS rule the converter either keeps or approximates, with the JASS construct, the
/// Lua the converter emits for it, and how faithful that is.</summary>
public sealed record ConversionRule(
    string Area,
    string Jass,
    string Lua,
    string Status,
    string Detail);

/// <summary>The rule set, plus a one line summary of how many are exact.</summary>
public sealed record ConversionNotesResult(
    IReadOnlyList<ConversionRule> Rules,
    string Summary);

/// <summary>
/// What the JASS to Lua converter does and where it diverges, as data rather than only as code.
/// A caller editing a converted script needs to know which operations run exactly as the JASS did
/// and which are approximated, and this is that list. The converter is a port of devoltzz's
/// devos-map-doctor jass2lua.py at 80efaf6, so the two share every approximation here except
/// strict reals, which this port adds.
/// </summary>
public static class ConversionNotes
{
    /// <summary>The construct converts to Lua that runs the same.</summary>
    public const string Faithful = "faithful";

    /// <summary>The construct converts but runs differently in some case, named in the detail.</summary>
    public const string Approximated = "approximated";

    /// <summary>The construct converts faithfully only when the conversion asks for it.</summary>
    public const string OptIn = "opt-in";

    public static ConversionNotesResult Rules()
    {
        var rules = new List<ConversionRule>
        {
            new("integer arithmetic",
                "integer + - *",
                "__iadd, __isub, __imul",
                Faithful,
                "Each helper wraps its result to a signed 32 bit value, the width a JASS integer has. "
                + "Applied when both operands are integer and neither is a small literal, which Lua "
                + "already evaluates exactly."),
            new("integer division",
                "integer / integer",
                "__idiv",
                Faithful,
                "Truncates toward zero, the JASS rule, not toward negative infinity, which is Lua's "
                + "floor division. A zero divisor yields 0."),
            new("real division by zero",
                "real / real",
                "__div",
                Faithful,
                "A zero or nil divisor yields 0.0 instead of raising, which is what the JASS runtime "
                + "does. A nonzero literal divisor is left as a plain Lua division."),
            new("real precision",
                "real + - * /",
                "plain Lua operators, or __fadd, __fsub, __fmul, __fdiv",
                OptIn,
                "JASS reals are 32 bit floats and Lua numbers are 64 bit doubles, so a converted real "
                + "expression accumulates a different rounding error than the original. Pass "
                + "strict_reals to round each result through an IEEE single, which is what the JASS "
                + "runtime holds. Off by default, so the default output matches the upstream port."),
            new("string concatenation",
                "string + anything",
                "__cat",
                Faithful,
                "A null operand concatenates as empty, a non-string operand is converted, and a chain "
                + "of 50 or more operands is folded through table.concat rather than nesting "
                + "parentheses past what Lua's parser accepts."),
            new("array defaults",
                "an array slot never written",
                "__arr_i, __arr_r, __arr_b",
                Faithful,
                "A JASS array reads 0, 0.0 or false from an unwritten slot rather than nil. The "
                + "helpers give an array a metatable whose __index returns its type's default."),
            new("hashtables",
                "CreateHashtable, SaveInteger, LoadInteger, FlushChildHashtable",
                "the same natives, unchanged",
                Approximated,
                "The hashtable natives exist in Lua, so a converted call runs, and no table rewrite is "
                + "done. A Lua table would be idiomatic and faster, but a hashtable passed to another "
                + "function or a native still has to be a real hashtable, so the rewrite is left to "
                + "the caller rather than guessed."),
            new("embedded script text",
                "EXExecuteScript, lua_load strings",
                "__exs, __luachunk",
                Faithful,
                "The text is collected and re-emitted, and the helper loads and runs it, so an index "
                + "into the map's own script table still resolves."),
            new("Lua back to JASS",
                "the reverse direction",
                "LuaToJassCommand",
                Approximated,
                "The conversion is not reversible in general, so only the constructs that map back "
                + "without guessing are converted and every one left is reported. The runtime helper "
                + "calls invert exactly, a table, closure, method call, generic for, repeat, goto or "
                + "typed local does not."),
        };

        int exact = rules.Count(r => r.Status == Faithful);
        string summary = $"{rules.Count} rule(s), {exact} faithful, "
            + $"{rules.Count(r => r.Status == Approximated)} approximated, "
            + $"{rules.Count(r => r.Status == OptIn)} faithful only when asked.";
        return new ConversionNotesResult(rules, summary);
    }
}
