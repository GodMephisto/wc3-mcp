// src/Wc3.Commands/Lua/LuaScriptIndex.cs
using Wc3.Model;

namespace Wc3.Commands.Lua;

/// <summary>
/// The function list and the name references of a Lua map script, in the same records the JASS
/// index uses (<see cref="JassFunction"/>, <see cref="JassReference"/>), so every caller that lists
/// functions, finds uses or jumps to a definition works on a Lua map without a second code path.
/// Built on the real parser, so a function is found by the grammar and not by a line pattern.
/// </summary>
public static class LuaScriptIndex
{
    /// <summary>
    /// Every function a script declares, in source order. That is each function statement,
    /// global, local or a.b:c method, at any depth, and each name = function or local name =
    /// function at any depth. Throws <see cref="LuaSyntaxError"/> when the script does not parse.
    /// </summary>
    public static IReadOnlyList<JassFunction> Functions(string source) => Functions(LuaParser.Parse(source), source);

    public static IReadOnlyList<JassFunction> Functions(LuaChunk chunk, string source)
    {
        var lines = new LineMap(source);
        var output = new List<JassFunction>();
        Walk(chunk.Body, null, (stmt, _) =>
        {
            switch (stmt)
            {
                case FunctionStmt f:
                    output.Add(Make(f.Name, f.IsLocal ? "local function " : "function ", f.Params, f.IsVararg, stmt, lines));
                    break;
                case AssignStmt a when a.Targets.Count == 1 && a.Values.Count == 1 && a.Values[0] is FunctionExpr fe
                                       && TargetName(a.Targets[0]) is { } name:
                    output.Add(Make(name, name + " = function", fe.Params, fe.IsVararg, stmt, lines, prefixIsHead: true));
                    break;
                case LocalStmt l when l.Names.Count == 1 && l.Values.Count == 1 && l.Values[0] is FunctionExpr le:
                    output.Add(Make(l.Names[0], $"local {l.Names[0]} = function", le.Params, le.IsVararg, stmt, lines, prefixIsHead: true));
                    break;
            }
        });
        return output.OrderBy(f => f.StartLine).ToList();
    }

    /// <summary>
    /// Every use of <paramref name="name"/>, its declarations included. A name followed by call
    /// arguments is a Call, a bare name passed or assigned as a value is a CodeReference (how a
    /// Lua map hands a function to TriggerAddAction, ForGroup and the like), and a function
    /// statement or name = function for it is a Declaration. A dotted name such as "a.b" matches
    /// field access on that path.
    /// </summary>
    public static IReadOnlyList<JassReference> References(string source, string name)
    {
        var chunk = LuaParser.Parse(source);
        var lines = new LineMap(source);
        var output = new List<JassReference>();
        void Add(int line, string? fn, JassReferenceKind kind) =>
            output.Add(new JassReference(line, fn, lines.Text(line), kind));

        Walk(chunk.Body, null, (stmt, fn) =>
        {
            if (stmt is FunctionStmt f && f.Name == name) Add(f.Line, fn, JassReferenceKind.Declaration);
            if (stmt is AssignStmt a && a.Targets.Count == 1 && a.Values.Count == 1 && a.Values[0] is FunctionExpr
                && TargetName(a.Targets[0]) == name)
                Add(stmt.Line, fn, JassReferenceKind.Declaration);
            foreach (var e in TopExprs(stmt))
                VisitExpr(e, fn, name, false, Add, isDeclTarget: stmt is AssignStmt da && da.Values.Count == 1
                    && da.Values[0] is FunctionExpr && ReferenceEquals(e, da.Targets[0]));
        });
        return output.OrderBy(r => r.Line).ToList();
    }

    /// <summary>The dotted name a target spells, "x" or "a.b.c", or null for anything else.</summary>
    internal static string? TargetName(LuaExpr e) => e switch
    {
        NameExpr n => n.Name,
        IndexExpr { Dot: true, Key: LiteralExpr { Value: string k } } ix when TargetName(ix.Base) is { } b => b + "." + k,
        _ => null,
    };

    private static JassFunction Make(string name, string head, List<string> pars, bool vararg, LuaStmt stmt,
        LineMap lines, bool prefixIsHead = false)
    {
        var all = vararg ? pars.Append("...") : pars;
        var signature = (prefixIsHead ? head : head + name) + "(" + string.Join(", ", all) + ")";
        int end = lines.LineAt(Math.Max(stmt.Span.Start, stmt.Span.End - 1));
        return new JassFunction(name, stmt.Line, end, signature);
    }

    // ---- traversal ---------------------------------------------------------------------

    /// <summary>Visits every statement, depth first, with the name of the function it sits in.</summary>
    internal static void Walk(LuaBlock block, string? inFunction, Action<LuaStmt, string?> visit)
    {
        foreach (var s in block)
        {
            visit(s, inFunction);
            string? here = s switch
            {
                FunctionStmt f => f.Name,
                AssignStmt a when a.Values.Count == 1 && a.Values[0] is FunctionExpr && a.Targets.Count == 1 => TargetName(a.Targets[0]) ?? inFunction,
                LocalStmt l when l.Values.Count == 1 && l.Values[0] is FunctionExpr && l.Names.Count == 1 => l.Names[0],
                _ => inFunction,
            };
            foreach (var child in ChildBlocks(s)) Walk(child, s is FunctionStmt ? here : inFunction, visit);
            foreach (var e in TopExprs(s))
                foreach (var fe in FunctionExprs(e))
                    Walk(fe.Body, here, visit);
        }
    }

    private static IEnumerable<LuaBlock> ChildBlocks(LuaStmt s) => s switch
    {
        FunctionStmt f => new[] { f.Body },
        IfStmt i => i.Branches.Select(b => b.Body),
        WhileStmt w => new[] { w.Body },
        NumericForStmt n => new[] { n.Body },
        GenericForStmt g => new[] { g.Body },
        RepeatStmt r => new[] { r.Body },
        DoStmt d => new[] { d.Body },
        _ => Array.Empty<LuaBlock>(),
    };

    /// <summary>The expressions a statement holds directly, not those inside nested blocks.</summary>
    internal static IEnumerable<LuaExpr> TopExprs(LuaStmt s) => s switch
    {
        LocalStmt l => l.Values,
        AssignStmt a => a.Targets.Concat(a.Values),
        CallStmt c => new LuaExpr[] { c.Call },
        IfStmt i => i.Branches.Where(b => b.Cond is not null).Select(b => b.Cond!),
        WhileStmt w => new[] { w.Cond },
        NumericForStmt n => n.Step is null ? new[] { n.Start, n.Stop } : new[] { n.Start, n.Stop, n.Step },
        GenericForStmt g => g.Exprs,
        RepeatStmt r => new[] { r.Cond },
        ReturnStmt r => r.Values,
        _ => Array.Empty<LuaExpr>(),
    };

    /// <summary>The function expressions inside an expression that are not inside another one.</summary>
    private static IEnumerable<FunctionExpr> FunctionExprs(LuaExpr e)
    {
        var stack = new Stack<LuaExpr>();
        stack.Push(e);
        while (stack.Count > 0)
        {
            var x = stack.Pop();
            if (x is FunctionExpr fe) { yield return fe; continue; }
            foreach (var c in Children(x)) stack.Push(c);
        }
    }

    internal static IEnumerable<LuaExpr> Children(LuaExpr e) => e switch
    {
        IndexExpr ix => new[] { ix.Base, ix.Key },
        CallExpr c => c.Args.Prepend(c.Func),
        UnaryExpr u => new[] { u.Operand },
        BinaryExpr b => new[] { b.Left, b.Right },
        ParenExpr p => new[] { p.Inner },
        TableExpr t => t.Fields.SelectMany(f => f.Key is LuaExpr k ? new[] { k, f.Value } : new[] { f.Value }),
        _ => Array.Empty<LuaExpr>(),
    };

    private static void VisitExpr(LuaExpr e, string? fn, string name, bool isCallee,
        Action<int, string?, JassReferenceKind> add, bool isDeclTarget)
    {
        if (isDeclTarget) return;
        if (e is FunctionExpr) return;   // its body is walked as statements, with its own function name
        var spelled = TargetName(e);
        if (spelled == name)
        {
            add(e.Line, fn, isCallee ? JassReferenceKind.Call : JassReferenceKind.CodeReference);
            return;
        }
        if (e is CallExpr c)
        {
            VisitExpr(c.Func, fn, name, c.Method is null, add, false);
            foreach (var a in c.Args) VisitExpr(a, fn, name, false, add, false);
            return;
        }
        foreach (var child in Children(e)) VisitExpr(child, fn, name, false, add, false);
    }

    /// <summary>Line numbers for byte offsets, counting line breaks the way Lua does.</summary>
    private sealed class LineMap
    {
        private readonly string _source;
        private readonly List<int> _starts = new() { 0 };

        public LineMap(string source)
        {
            _source = source;
            for (int i = 0; i < source.Length; i++)
            {
                char c = source[i];
                if (c != '\n' && c != '\r') continue;
                if (i + 1 < source.Length && (source[i + 1] == '\n' || source[i + 1] == '\r') && source[i + 1] != c) i++;
                _starts.Add(i + 1);
            }
        }

        public int LineAt(int offset)
        {
            int k = _starts.BinarySearch(offset);
            return (k >= 0 ? k : ~k - 1) + 1;
        }

        public string Text(int line)
        {
            if (line < 1 || line > _starts.Count) return "";
            int start = _starts[line - 1];
            int end = line < _starts.Count ? _starts[line] : _source.Length;
            return _source[start..end].TrimEnd('\r', '\n').Trim();
        }
    }
}
