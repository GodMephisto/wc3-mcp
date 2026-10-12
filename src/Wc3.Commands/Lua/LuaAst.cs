// src/Wc3.Commands/Lua/LuaAst.cs
namespace Wc3.Commands.Lua;

// The Lua 5.3 syntax tree, ported from lua_ast.py in Devo's Map Doctor (MIT). Every literal keeps
// its source text and every comment is kept on the statement it belongs to, so a script can be
// printed back with its comments.

public abstract class LuaNode
{
    public int Line { get; init; }
}

public abstract class LuaExpr : LuaNode { }

public abstract class LuaStmt : LuaNode
{
    /// <summary>A comment on the same line, after the statement.</summary>
    public LuaComment? Comment { get; internal set; }
    public IReadOnlyList<LuaComment> LeadingComments { get; internal set; } = Array.Empty<LuaComment>();
    public IReadOnlyList<LuaComment> InnerComments { get; internal set; } = Array.Empty<LuaComment>();
    /// <summary>Byte offsets of the statement in the source, start inclusive, end exclusive.</summary>
    public (int Start, int End) Span { get; internal set; }
}

/// <summary>A statement list, with the comments that close it.</summary>
public sealed class LuaBlock : List<LuaStmt>
{
    public IReadOnlyList<LuaComment> EndComments { get; internal set; } = Array.Empty<LuaComment>();
}

public sealed class LocalStmt : LuaStmt
{
    public required List<string> Names { get; init; }
    public required List<string?> Attribs { get; init; }
    public required List<LuaExpr> Values { get; init; }
}

public sealed class AssignStmt : LuaStmt
{
    public required List<LuaExpr> Targets { get; init; }
    public required List<LuaExpr> Values { get; init; }
}

public sealed class CallStmt : LuaStmt
{
    public required CallExpr Call { get; init; }
}

public sealed class FunctionStmt : LuaStmt
{
    /// <summary>The full name, "f", "a.b.c" or "a.b:m".</summary>
    public required string Name { get; init; }
    public required bool IsLocal { get; init; }
    public required List<string> Params { get; init; }
    public required bool IsVararg { get; init; }
    public required LuaBlock Body { get; init; }
}

public sealed class IfStmt : LuaStmt
{
    /// <summary>Each branch, the condition null for the else branch.</summary>
    public required List<(LuaExpr? Cond, LuaBlock Body)> Branches { get; init; }
}

public sealed class WhileStmt : LuaStmt
{
    public required LuaExpr Cond { get; init; }
    public required LuaBlock Body { get; init; }
}

public sealed class NumericForStmt : LuaStmt
{
    public required string Var { get; init; }
    public required LuaExpr Start { get; init; }
    public required LuaExpr Stop { get; init; }
    public LuaExpr? Step { get; init; }
    public required LuaBlock Body { get; init; }
}

public sealed class GenericForStmt : LuaStmt
{
    public required List<string> Names { get; init; }
    public required List<LuaExpr> Exprs { get; init; }
    public required LuaBlock Body { get; init; }
}

public sealed class RepeatStmt : LuaStmt
{
    public required LuaBlock Body { get; init; }
    public required LuaExpr Cond { get; init; }
}

public sealed class DoStmt : LuaStmt
{
    public required LuaBlock Body { get; init; }
}

public sealed class ReturnStmt : LuaStmt
{
    public required List<LuaExpr> Values { get; init; }
}

public sealed class BreakStmt : LuaStmt { }

public sealed class GotoStmt : LuaStmt
{
    public required string Label { get; init; }
}

public sealed class LabelStmt : LuaStmt
{
    public required string Name { get; init; }
}

public sealed class NameExpr : LuaExpr
{
    public required string Name { get; init; }
}

public sealed class IndexExpr : LuaExpr
{
    public required LuaExpr Base { get; init; }
    public required LuaExpr Key { get; init; }
    /// <summary>Written as base.name rather than base[key].</summary>
    public bool Dot { get; init; }
}

public sealed class CallExpr : LuaExpr
{
    public required LuaExpr Func { get; init; }
    public required List<LuaExpr> Args { get; init; }
    /// <summary>The method name of a call written obj:method(...), else null.</summary>
    public string? Method { get; init; }
}

public sealed class FunctionExpr : LuaExpr
{
    public required List<string> Params { get; init; }
    public required bool IsVararg { get; init; }
    public required LuaBlock Body { get; init; }
}

/// <summary>A literal. Kind is number, string, boolean, nil or vararg.</summary>
public sealed class LiteralExpr : LuaExpr
{
    public required string Kind { get; init; }
    /// <summary>The source text, or null for a literal built rather than parsed.</summary>
    public string? Text { get; init; }
    /// <summary>A long or double for a number, the decoded bytes for a string, a bool, or null.</summary>
    public object? Value { get; init; }
}

public sealed class UnaryExpr : LuaExpr
{
    public required string Op { get; init; }
    public required LuaExpr Operand { get; init; }
}

public sealed class BinaryExpr : LuaExpr
{
    public required string Op { get; init; }
    public required LuaExpr Left { get; init; }
    public required LuaExpr Right { get; init; }
}

public sealed class ParenExpr : LuaExpr
{
    public required LuaExpr Inner { get; init; }
}

public sealed class TableExpr : LuaExpr
{
    /// <summary>
    /// Each field. Key is null for a positional field, a string for name = value, and an
    /// expression for [key] = value.
    /// </summary>
    public required List<(object? Key, LuaExpr Value)> Fields { get; init; }
}

/// <summary>A parsed chunk. Functions holds the top-level function statements by full name.</summary>
public sealed record LuaChunk(LuaBlock Body, IReadOnlyDictionary<string, FunctionStmt> Functions,
    IReadOnlyList<LuaComment> Comments);
