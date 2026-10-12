// src/Wc3.Commands/Lua/LuaScriptReader.cs
using Wc3.Model;

namespace Wc3.Commands.Lua;

/// <summary>
/// Shared reading and parsing for the Lua script commands. Language detection and the decoded text
/// come from <see cref="ScriptCommand"/>, the tree from <see cref="LuaParser"/>, exactly as the
/// existing Lua tools do, so a Lua command never reads the script a second way.
/// </summary>
internal static class LuaScriptReader
{
    /// <summary>
    /// The map's script file, its decoded text, and whether it is a Lua script. A map whose script
    /// is JASS is reported so a Lua command can say so rather than fail.
    /// </summary>
    internal static (string File, string Source, bool IsLua) Read(MapDocument doc)
    {
        var (file, source) = ScriptCommand.Read(doc);
        return (file, source, ScriptCommand.IsLua(file));
    }

    /// <summary>Parses a chunk, returning null and the syntax error when it does not parse.</summary>
    internal static LuaChunk? TryParse(string source, out LuaSyntaxError? error)
    {
        try { error = null; return LuaParser.Parse(source); }
        catch (LuaSyntaxError e) { error = e; return null; }
    }

    /// <summary>The character offset at the start of each 1-based line, following Lua's rule that
    /// \r\n and \n\r are one line break. Result[0] is 0.</summary>
    internal static int[] LineStarts(string source)
    {
        var starts = new List<int> { 0 };
        for (int i = 0; i < source.Length; i++)
        {
            char c = source[i];
            if (c != '\n' && c != '\r') continue;
            if (i + 1 < source.Length && (source[i + 1] == '\n' || source[i + 1] == '\r') && source[i + 1] != c) i++;
            starts.Add(i + 1);
        }
        return starts.ToArray();
    }

    /// <summary>The text of a 1-based line without its terminator, or the empty string when the
    /// line is out of range.</summary>
    internal static string LineText(string source, int line)
    {
        var starts = LineStarts(source);
        if (line < 1 || line > starts.Length) return string.Empty;
        int start = starts[line - 1];
        int end = line < starts.Length ? starts[line] : source.Length;
        return source[start..end].TrimEnd('\r', '\n');
    }

    /// <summary>Every call expression inside an expression, the root included.</summary>
    internal static IEnumerable<CallExpr> Calls(LuaExpr root)
    {
        var stack = new Stack<LuaExpr>();
        stack.Push(root);
        while (stack.Count > 0)
        {
            var e = stack.Pop();
            if (e is CallExpr c)
            {
                yield return c;
                foreach (var a in c.Args) stack.Push(a);
                stack.Push(c.Func);
                continue;
            }
            foreach (var child in LuaScriptIndex.Children(e)) stack.Push(child);
        }
    }

    /// <summary>The dotted name an expression names, "f" or "a.b", or null for anything else.</summary>
    internal static string? CalleeName(LuaExpr e) => e switch
    {
        NameExpr n => n.Name,
        IndexExpr { Dot: true, Key: LiteralExpr { Value: string k } } ix when CalleeName(ix.Base) is { } b => b + "." + k,
        _ => null,
    };

    /// <summary>Every statement in a block, including those of nested blocks.</summary>
    internal static IEnumerable<LuaStmt> StatementsOf(LuaBlock block)
    {
        foreach (var s in block)
        {
            yield return s;
            foreach (var child in ChildBlocks(s))
                foreach (var inner in StatementsOf(child))
                    yield return inner;
        }
    }

    private static IEnumerable<LuaBlock> ChildBlocks(LuaStmt s) => s switch
    {
        IfStmt i => i.Branches.Select(b => b.Body),
        WhileStmt w => new[] { w.Body },
        NumericForStmt n => new[] { n.Body },
        GenericForStmt g => new[] { g.Body },
        RepeatStmt r => new[] { r.Body },
        DoStmt d => new[] { d.Body },
        _ => Array.Empty<LuaBlock>(),
    };
}
