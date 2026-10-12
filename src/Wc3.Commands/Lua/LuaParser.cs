// src/Wc3.Commands/Lua/LuaParser.cs
namespace Wc3.Commands.Lua;

/// <summary>
/// Lua 5.3 parser, ported from lua_ast.py in Devo's Map Doctor (MIT,
/// github.com/devoltzz/devos-map-doctor at 80efaf6), which follows lparser.c of the reference
/// implementation. It refuses what luac refuses, with luac's message and line, including the
/// semantic checks (goto and label scope, break outside a loop, vararg outside a vararg
/// function, the 200 C level, 200 local and 255 upvalue limits).
/// </summary>
public sealed class LuaParser
{
    internal static readonly IReadOnlyDictionary<string, (int Left, int Right)> BinaryPriority =
        new Dictionary<string, (int, int)>(StringComparer.Ordinal)
        {
            ["+"] = (10, 10), ["-"] = (10, 10), ["*"] = (11, 11), ["%"] = (11, 11), ["^"] = (14, 13),
            ["/"] = (11, 11), ["//"] = (11, 11), ["&"] = (6, 6), ["|"] = (4, 4), ["~"] = (5, 5),
            ["<<"] = (7, 7), [">>"] = (7, 7), [".."] = (9, 8), ["=="] = (3, 3), ["<"] = (3, 3),
            ["<="] = (3, 3), ["~="] = (3, 3), [">"] = (3, 3), [">="] = (3, 3), ["and"] = (2, 2),
            ["or"] = (1, 1),
        };

    internal const int UnaryPriority = 12;
    private static readonly HashSet<string> Unary = new(StringComparer.Ordinal) { "not", "-", "#", "~" };
    private static readonly HashSet<string> BlockEnd = new(StringComparer.Ordinal) { "else", "elseif", "end", "until", "EOF" };
    private static readonly HashSet<string> LabelLast = new(StringComparer.Ordinal) { "else", "elseif", "end", "EOF" };
    private const int LevelLimit = 200;
    private const int LevelBase = 1;
    private const int MaxLocals = 200;
    private const int MaxUpvalues = 255;

    private sealed class LocalVar
    {
        public required string Name;
        public string? Attrib;
    }

    private static readonly LocalVar Global = new() { Name = "(global)" };

    private sealed class LabelDesc
    {
        public required string Name;
        public int Line;
        public int NActive;
    }

    private sealed class FuncState
    {
        public FuncState? Parent;
        public int Line;
        public bool Vararg;
        public readonly List<LocalVar> ActVar = new();
        public int NActive;
        public readonly Dictionary<string, List<int>> Vars = new(StringComparer.Ordinal);
        public readonly Dictionary<string, LocalVar> Upvals = new(StringComparer.Ordinal);
        public readonly List<(int NActive, bool IsLoop, int FirstLabel, int FirstGoto)> Blocks = new();
        public readonly List<LabelDesc> Labels = new();
        public readonly List<LabelDesc> Gotos = new();
    }

    private readonly List<string> _k;
    private readonly List<string> _t;
    private readonly List<int> _l;
    private readonly List<int> _p;
    private readonly List<LuaComment> _comments;
    private readonly List<int> _ctok;
    private readonly Dictionary<int, string> _strval;
    private int _i;
    private int _ci;
    private readonly int _nc;
    private int _level = LevelBase;
    private FuncState _fs = null!;
    private List<LuaComment> _inner = new();
    private readonly List<LabelDesc> _labelsPending = new();

    private LuaParser(string text)
    {
        var t = LuaLexer.Lex(text);
        (_k, _t, _l, _p, _comments, _ctok, _strval) =
            (t.Kinds, t.Texts, t.Lines, t.Offsets, t.Comments, t.CommentTokens, t.StringValues);
        _nc = _comments.Count;
    }

    /// <summary>Parses a chunk. Throws <see cref="LuaSyntaxError"/> as luac would.</summary>
    public static LuaChunk Parse(string text)
    {
        // Deep nesting is bounded by the 200 level limit, but each level is several frames, so
        // run on a thread with a stack that cannot overflow before the limit is reached.
        LuaChunk? result = null;
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try { result = new LuaParser(text).Chunk(); }
            catch (Exception ex) { failure = ex; }
        }, 64 * 1024 * 1024);
        thread.Start();
        thread.Join();
        if (failure is not null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure).Throw();
        return result!;
    }

    /// <summary>Parses source bytes, one char per byte.</summary>
    public static LuaChunk Parse(byte[] source) => Parse(LuaLexer.FromBytes(source));

    /// <summary>Null when the chunk parses, else its syntax error.</summary>
    public static LuaSyntaxError? Check(string text)
    {
        try { Parse(text); return null; }
        catch (LuaSyntaxError e) { return e; }
    }

    private LuaChunk Chunk()
    {
        var fs = _fs = new FuncState { Parent = null, Line = 0, Vararg = true };
        fs.Upvals["_ENV"] = new LocalVar { Name = "_ENV" };
        EnterBlock(false);
        var body = StatList();
        if (_k[_i] != "EOF") ErrorExpected("EOF");
        LeaveBlock();
        var functions = new Dictionary<string, FunctionStmt>(StringComparer.Ordinal);
        foreach (var s in body)
            if (s is FunctionStmt f) functions[f.Name] = f;
        return new LuaChunk(body, functions, _comments);
    }

    // ---- errors -------------------------------------------------------------

    private int EndLine(int i)
    {
        int line = _l[i];
        if (_k[i] == "STRING") line += LuaLexer.NewlineCount(_t[i]);
        return line;
    }

    private string Near()
    {
        var kind = _k[_i];
        if (kind == "EOF") return "<eof>";
        var s = _t[_i];
        if (kind == "CHAR")
        {
            int b = s[0];
            return b is >= 32 and < 127 ? $"'{s}'" : $"'<\\{b}>'";
        }
        return LuaLexer.NearText(s);
    }

    private LuaSyntaxError Raise(string message, bool near = true)
    {
        int i = _i;
        if (_k[i] == "ERROR") return new LuaSyntaxError(_l[i], _t[i]);
        if (near) message += " near " + Near();
        return new LuaSyntaxError(EndLine(i), message);
    }

    private LuaSyntaxError Error(string message) => Raise(message);
    private LuaSyntaxError SemError(string message) => Raise(message, near: false);

    private LuaSyntaxError ErrorExpected(string what) => throw Raise(what switch
    {
        "EOF" => "<eof>",
        "NAME" => "<name>",
        _ => $"'{what}'",
    } + " expected");

    private LuaSyntaxError Limit(string what, int limit, FuncState? fs = null)
    {
        fs ??= _fs;
        var where = fs.Line == 0 ? "main function" : $"function at line {fs.Line}";
        throw Raise($"too many {what} (limit is {limit}) in {where}");
    }

    private void CheckNext(string what)
    {
        if (_k[_i] != what) ErrorExpected(what);
        _i++;
    }

    private void CheckMatch(string what, string who, int line)
    {
        if (_k[_i] != what)
        {
            if (line == EndLine(_i)) ErrorExpected(what);
            throw Raise($"'{what}' expected (to close '{who}' at line {line})");
        }
        _i++;
    }

    private string CheckName()
    {
        int i = _i;
        if (_k[i] != "NAME") ErrorExpected("NAME");
        _i = i + 1;
        return _t[i];
    }

    // ---- statements -----------------------------------------------------------

    private List<LuaComment> Take(int j)
    {
        int start = _ci;
        while (_ci < _nc && _ctok[_ci] <= j) _ci++;
        return _comments.GetRange(start, _ci - start);
    }

    private LuaBlock StatList()
    {
        var body = new LuaBlock();
        var outer = _inner;
        if (_ci < _nc && _ctok[_ci] < _i) outer.AddRange(Take(_i - 1));
        while (true)
        {
            int i = _i;
            var kind = _k[i];
            if (_labelsPending.Count > 0 && kind != ";" && kind != "::") CloseLabels(kind);
            if (BlockEnd.Contains(kind)) break;
            int depth = 1 + _labelsPending.Count;
            if (kind == ";")
            {
                if (_level + depth > LevelLimit) Limit("C levels", LevelLimit);
                _i = i + 1;
                continue;
            }
            IReadOnlyList<LuaComment> lead = _ci < _nc && _ctok[_ci] <= i ? Take(i) : Array.Empty<LuaComment>();
            var inner = _inner = new List<LuaComment>();
            _level += depth;
            if (_level > LevelLimit) Limit("C levels", LevelLimit);
            var stmt = Statement(kind);
            _level -= depth;
            int last = _i - 1;
            if (_ci < _nc)
            {
                if (_ctok[_ci] <= last) inner.AddRange(Take(last));
                if (_ci < _nc)
                {
                    int j = _i;
                    while (_k[j] == ";") j++;
                    var c = _comments[_ci];
                    if (_ctok[_ci] <= j && c.Line == EndLine(last))
                    {
                        stmt.Comment = c;
                        _ci++;
                    }
                }
            }
            stmt.LeadingComments = lead;
            if (inner.Count > 0) stmt.InnerComments = inner;
            stmt.Span = (_p[i], _p[last] + _t[last].Length);
            body.Add(stmt);
            if (kind == "return") break;
        }
        _inner = outer;
        if (_ci < _nc && _ctok[_ci] <= _i) body.EndComments = Take(_i);
        return body;
    }

    private LuaStmt Statement(string kind) => kind switch
    {
        "if" => If(),
        "while" => While(),
        "do" => Do(),
        "for" => For(),
        "repeat" => Repeat(),
        "function" => Function(),
        "local" => Local(),
        "::" => Label(),
        "return" => Return(),
        "break" => Break(),
        "goto" => Goto(),
        _ => ExprStat(),
    };

    private LuaBlock Block()
    {
        EnterBlock(false);
        var body = StatList();
        LeaveBlock();
        return body;
    }

    private void EnterBlock(bool isLoop) =>
        _fs.Blocks.Add((_fs.NActive, isLoop, _fs.Labels.Count, _fs.Gotos.Count));

    private void LeaveBlock()
    {
        var fs = _fs;
        var (nact, isLoop, firstLabel, firstGoto) = fs.Blocks[^1];
        if (isLoop) FindGotos(fs, new LabelDesc { Name = "break", Line = 0, NActive = fs.NActive });
        fs.Blocks.RemoveAt(fs.Blocks.Count - 1);
        if (fs.NActive > nact)
        {
            for (int idx = fs.NActive - 1; idx >= nact; idx--)
            {
                var name = fs.ActVar[idx].Name;
                var stack = fs.Vars[name];
                stack.RemoveAt(stack.Count - 1);
                if (stack.Count == 0) fs.Vars.Remove(name);
            }
            fs.ActVar.RemoveRange(nact, fs.ActVar.Count - nact);
            fs.NActive = nact;
        }
        fs.Labels.RemoveRange(firstLabel, fs.Labels.Count - firstLabel);
        var gotos = fs.Gotos;
        if (gotos.Count > firstGoto)
        {
            if (fs.Blocks.Count == 0)
            {
                var gt = gotos[firstGoto];
                if (gt.Name == "break") throw SemError($"<break> at line {gt.Line} not inside a loop");
                throw SemError($"no visible label '{gt.Name}' for <goto> at line {gt.Line}");
            }
            int i = firstGoto;
            while (i < gotos.Count)
            {
                if (gotos[i].NActive > nact) gotos[i].NActive = nact;
                if (!FindLabel(fs, i)) i++;
            }
        }
    }

    private bool FindLabel(FuncState fs, int gi)
    {
        var name = fs.Gotos[gi].Name;
        for (int k = fs.Blocks[^1].FirstLabel; k < fs.Labels.Count; k++)
        {
            var lb = fs.Labels[k];
            if (lb.Name == name)
            {
                CloseGoto(fs, gi, lb);
                return true;
            }
        }
        return false;
    }

    private void FindGotos(FuncState fs, LabelDesc lb)
    {
        var gotos = fs.Gotos;
        int i = fs.Blocks[^1].FirstGoto;
        while (i < gotos.Count)
        {
            if (gotos[i].Name == lb.Name) CloseGoto(fs, i, lb);
            else i++;
        }
    }

    private void CloseGoto(FuncState fs, int gi, LabelDesc lb)
    {
        var gt = fs.Gotos[gi];
        if (gt.NActive < lb.NActive)
            throw SemError($"<goto {gt.Name}> at line {gt.Line} jumps into the scope of local '{fs.ActVar[gt.NActive].Name}'");
        fs.Gotos.RemoveAt(gi);
    }

    private void NewLocalVar(string name, string? attrib = null)
    {
        if (_fs.ActVar.Count >= MaxLocals) Limit("local variables", MaxLocals);
        _fs.ActVar.Add(new LocalVar { Name = name, Attrib = attrib });
    }

    private void AdjustLocalVars(int n)
    {
        var fs = _fs;
        for (int idx = fs.NActive; idx < fs.NActive + n; idx++)
        {
            var name = fs.ActVar[idx].Name;
            if (!fs.Vars.TryGetValue(name, out var stack)) fs.Vars[name] = new List<int> { idx };
            else stack.Add(idx);
        }
        fs.NActive += n;
    }

    private LocalVar Resolve(FuncState fs, string name)
    {
        if (fs.Vars.TryGetValue(name, out var stack) && stack.Count > 0) return fs.ActVar[stack[^1]];
        if (fs.Upvals.TryGetValue(name, out var up)) return up;
        if (fs.Parent is null) return Global;
        var v = Resolve(fs.Parent, name);
        if (ReferenceEquals(v, Global)) return Global;
        if (fs.Upvals.Count >= MaxUpvalues) Limit("upvalues", MaxUpvalues, fs);
        fs.Upvals[name] = v;
        return v;
    }

    private void SingleVar(string name)
    {
        if (!_fs.Vars.ContainsKey(name) && ReferenceEquals(Resolve(_fs, name), Global)) Resolve(_fs, "_ENV");
    }

    private void CheckAssignable(string name)
    {
        var fs = _fs;
        LocalVar? v = fs.Vars.TryGetValue(name, out var stack) && stack.Count > 0
            ? fs.ActVar[stack[^1]]
            : fs.Upvals.GetValueOrDefault(name);
        if (v?.Attrib is not null) throw SemError($"attempt to assign to const variable '{name}'");
    }

    private LuaStmt If()
    {
        int line = _l[_i];
        var branches = new List<(LuaExpr?, LuaBlock)>();
        while (true)
        {
            _i++;
            var cond = SubExpr(0);
            CheckNext("then");
            branches.Add((cond, Block()));
            if (_k[_i] != "elseif") break;
        }
        if (_k[_i] == "else")
        {
            _i++;
            branches.Add((null, Block()));
        }
        CheckMatch("end", "if", line);
        return new IfStmt { Branches = branches, Line = line };
    }

    private LuaStmt While()
    {
        int line = _l[_i];
        _i++;
        var cond = SubExpr(0);
        EnterBlock(true);
        CheckNext("do");
        var body = Block();
        CheckMatch("end", "while", line);
        LeaveBlock();
        return new WhileStmt { Cond = cond, Body = body, Line = line };
    }

    private LuaStmt Do()
    {
        int line = _l[_i];
        _i++;
        var body = Block();
        CheckMatch("end", "do", line);
        return new DoStmt { Body = body, Line = line };
    }

    private LuaStmt Repeat()
    {
        int line = _l[_i];
        _i++;
        EnterBlock(true);
        EnterBlock(false);
        var body = StatList();
        CheckMatch("until", "repeat", line);
        var cond = SubExpr(0);
        LeaveBlock();
        LeaveBlock();
        return new RepeatStmt { Body = body, Cond = cond, Line = line };
    }

    private LuaStmt For()
    {
        int line = _l[_i];
        _i++;
        EnterBlock(true);
        var name = CheckName();
        var kind = _k[_i];
        LuaStmt stmt;
        if (kind == "=")
        {
            foreach (var hidden in new[] { "(for index)", "(for limit)", "(for step)" }) NewLocalVar(hidden);
            NewLocalVar(name);
            _i++;
            var start = SubExpr(0);
            CheckNext(",");
            var stop = SubExpr(0);
            LuaExpr? step = null;
            if (_k[_i] == ",")
            {
                _i++;
                step = SubExpr(0);
            }
            stmt = new NumericForStmt { Var = name, Start = start, Stop = stop, Step = step, Body = ForBody(1), Line = line };
        }
        else if (kind == "," || kind == "in")
        {
            foreach (var hidden in new[] { "(for generator)", "(for state)", "(for control)" }) NewLocalVar(hidden);
            NewLocalVar(name);
            var names = new List<string> { name };
            while (_k[_i] == ",")
            {
                _i++;
                names.Add(CheckName());
                NewLocalVar(names[^1]);
            }
            CheckNext("in");
            var exprs = ExpList();
            stmt = new GenericForStmt { Names = names, Exprs = exprs, Body = ForBody(names.Count), Line = line };
        }
        else throw Error("'=' or 'in' expected");
        CheckMatch("end", "for", line);
        LeaveBlock();
        return stmt;
    }

    private LuaBlock ForBody(int nvars)
    {
        AdjustLocalVars(3);
        CheckNext("do");
        EnterBlock(false);
        AdjustLocalVars(nvars);
        var body = Block();
        LeaveBlock();
        return body;
    }

    private LuaStmt Function()
    {
        int line = _l[_i];
        _i++;
        var name = CheckName();
        SingleVar(name);
        var full = name;
        bool isMethod = false;
        while (_k[_i] == ".")
        {
            _i++;
            full += "." + CheckName();
        }
        if (_k[_i] == ":")
        {
            _i++;
            full += ":" + CheckName();
            isMethod = true;
        }
        var (pars, vararg, body) = FuncBody(line, isMethod);
        if (full == name) CheckAssignable(name);
        return new FunctionStmt { Name = full, IsLocal = false, Params = pars, IsVararg = vararg, Body = body, Line = line };
    }

    private LuaStmt Local()
    {
        int line = _l[_i];
        _i++;
        if (_k[_i] == "function")
        {
            _i++;
            var fname = CheckName();
            NewLocalVar(fname);
            AdjustLocalVars(1);
            var (pars, vararg, body) = FuncBody(_l[_i], false);
            return new FunctionStmt { Name = fname, IsLocal = true, Params = pars, IsVararg = vararg, Body = body, Line = line };
        }
        // Lua 5.4 local attributes (<const>, <close>) are not parsed. The doctor accepts them, but
        // Warcraft III runs Lua 5.3, and luac 5.3.6 refuses "local x <const>" with "unexpected
        // symbol near '<'", which is what falls out of leaving the '<' for the next statement.
        var names = new List<string>();
        var attribs = new List<string?>();
        while (true)
        {
            var name = CheckName();
            NewLocalVar(name);
            names.Add(name);
            attribs.Add(null);
            if (_k[_i] != ",") break;
            _i++;
        }
        var values = new List<LuaExpr>();
        if (_k[_i] == "=")
        {
            _i++;
            values = ExpList();
        }
        AdjustLocalVars(names.Count);
        return new LocalStmt { Names = names, Attribs = attribs, Values = values, Line = line };
    }

    private LuaStmt Label()
    {
        int line = _l[_i];
        _i++;
        var name = CheckName();
        var fs = _fs;
        for (int k = fs.Blocks[^1].FirstLabel; k < fs.Labels.Count; k++)
            if (fs.Labels[k].Name == name)
                throw SemError($"label '{name}' already defined on line {fs.Labels[k].Line}");
        CheckNext("::");
        var lb = new LabelDesc { Name = name, Line = line, NActive = fs.NActive };
        fs.Labels.Add(lb);
        _labelsPending.Add(lb);
        return new LabelStmt { Name = name, Line = line };
    }

    private void CloseLabels(string kind)
    {
        while (_labelsPending.Count > 0)
        {
            var lb = _labelsPending[^1];
            _labelsPending.RemoveAt(_labelsPending.Count - 1);
            if (LabelLast.Contains(kind)) lb.NActive = _fs.Blocks[^1].NActive;
            FindGotos(_fs, lb);
        }
    }

    private LuaStmt Return()
    {
        int line = _l[_i];
        _i++;
        var kind = _k[_i];
        var values = BlockEnd.Contains(kind) || kind == ";" ? new List<LuaExpr>() : ExpList();
        if (_k[_i] == ";") _i++;
        return new ReturnStmt { Values = values, Line = line };
    }

    private LuaStmt Break()
    {
        int line = _l[_i];
        _i++;
        _fs.Gotos.Add(new LabelDesc { Name = "break", Line = line, NActive = _fs.NActive });
        return new BreakStmt { Line = line };
    }

    private LuaStmt Goto()
    {
        int line = _l[_i];
        _i++;
        var name = CheckName();
        _fs.Gotos.Add(new LabelDesc { Name = name, Line = line, NActive = _fs.NActive });
        FindLabel(_fs, _fs.Gotos.Count - 1);
        return new GotoStmt { Label = name, Line = line };
    }

    private LuaStmt ExprStat()
    {
        int line = _l[_i];
        var e = SuffixedExp();
        var kind = _k[_i];
        if (kind == "=" || kind == ",")
        {
            var targets = new List<LuaExpr> { e };
            CheckTarget(e);
            while (_k[_i] == ",")
            {
                _i++;
                e = SuffixedExp();
                if (targets.Count + _level > LevelLimit) Limit("C levels", LevelLimit);
                targets.Add(e);
                CheckTarget(e);
            }
            CheckNext("=");
            return new AssignStmt { Targets = targets, Values = ExpList(), Line = line };
        }
        if (e is not CallExpr call) throw Error("syntax error");
        return new CallStmt { Call = call, Line = line };
    }

    private void CheckTarget(LuaExpr e)
    {
        if (e is NameExpr n) CheckAssignable(n.Name);
        else if (e is not IndexExpr) throw Error("syntax error");
    }

    // ---- expressions ------------------------------------------------------------

    private List<LuaExpr> ExpList()
    {
        var values = new List<LuaExpr> { SubExpr(0) };
        while (_k[_i] == ",")
        {
            _i++;
            values.Add(SubExpr(0));
        }
        return values;
    }

    private LuaExpr SubExpr(int limit)
    {
        _level++;
        if (_level > LevelLimit) Limit("C levels", LevelLimit);
        int i = _i;
        var kind = _k[i];
        LuaExpr e;
        if (Unary.Contains(kind))
        {
            _i = i + 1;
            e = new UnaryExpr { Op = kind, Operand = SubExpr(UnaryPriority), Line = _l[i] };
        }
        else e = SimpleExp();
        while (true)
        {
            var op = _k[_i];
            if (!BinaryPriority.TryGetValue(op, out var p) || p.Left <= limit) break;
            _i++;
            e = new BinaryExpr { Op = op, Left = e, Right = SubExpr(p.Right), Line = e.Line };
        }
        _level--;
        return e;
    }

    private LuaExpr SimpleExp()
    {
        int i = _i;
        var kind = _k[i];
        switch (kind)
        {
            case "NAME":
            case "(":
                return SuffixedExp();
            case "NUMBER":
            {
                _i = i + 1;
                var s = _t[i];
                return new LiteralExpr { Kind = "number", Text = s, Value = LuaLexer.NumberValue(s), Line = _l[i] };
            }
            case "STRING":
                _i = i + 1;
                return StringLit(i);
            case "nil":
                _i = i + 1;
                return new LiteralExpr { Kind = "nil", Text = "nil", Value = null, Line = _l[i] };
            case "true":
            case "false":
                _i = i + 1;
                return new LiteralExpr { Kind = "boolean", Text = kind, Value = kind == "true", Line = _l[i] };
            case "...":
                if (!_fs.Vararg) throw Error("cannot use '...' outside a vararg function");
                _i = i + 1;
                return new LiteralExpr { Kind = "vararg", Text = "...", Value = null, Line = _l[i] };
            case "{":
                return Table();
            case "function":
            {
                _i = i + 1;
                var (pars, vararg, body) = FuncBody(_l[i + 1], false);
                return new FunctionExpr { Params = pars, IsVararg = vararg, Body = body, Line = _l[i] };
            }
            default:
                return SuffixedExp();
        }
    }

    private LiteralExpr StringLit(int i)
    {
        var s = _t[i];
        if (!_strval.TryGetValue(i, out var value))
            value = s[0] == '[' ? LuaLexer.LongValue(s) : s[1..^1];
        return new LiteralExpr { Kind = "string", Text = s, Value = value, Line = _l[i] };
    }

    private LuaExpr SuffixedExp()
    {
        int i = _i;
        var kind = _k[i];
        int line = _l[i];
        LuaExpr e;
        if (kind == "NAME")
        {
            var name = _t[i];
            _i = i + 1;
            SingleVar(name);
            e = new NameExpr { Name = name, Line = line };
        }
        else if (kind == "(")
        {
            _i = i + 1;
            var inner = SubExpr(0);
            CheckMatch(")", "(", line);
            e = new ParenExpr { Inner = inner, Line = line };
        }
        else throw Error("unexpected symbol");
        while (true)
        {
            kind = _k[_i];
            switch (kind)
            {
                case ".":
                {
                    _i++;
                    int j = _i;
                    var name = CheckName();
                    e = new IndexExpr
                    {
                        Base = e,
                        Key = new LiteralExpr { Kind = "string", Text = $"\"{name}\"", Value = name, Line = _l[j] },
                        Dot = true,
                        Line = line,
                    };
                    break;
                }
                case "[":
                {
                    _i++;
                    var key = SubExpr(0);
                    CheckNext("]");
                    e = new IndexExpr { Base = e, Key = key, Dot = false, Line = line };
                    break;
                }
                case ":":
                {
                    _i++;
                    var name = CheckName();
                    e = new CallExpr { Func = e, Args = FuncArgs(line), Method = name, Line = line };
                    break;
                }
                case "(":
                case "STRING":
                case "{":
                    e = new CallExpr { Func = e, Args = FuncArgs(line), Method = null, Line = line };
                    break;
                default:
                    return e;
            }
        }
    }

    private List<LuaExpr> FuncArgs(int line)
    {
        int i = _i;
        var kind = _k[i];
        if (kind == "(")
        {
            _i = i + 1;
            var args = _k[i + 1] == ")" ? new List<LuaExpr>() : ExpList();
            CheckMatch(")", "(", line);
            return args;
        }
        if (kind == "STRING")
        {
            _i = i + 1;
            return new List<LuaExpr> { StringLit(i) };
        }
        if (kind == "{") return new List<LuaExpr> { Table() };
        throw Error("function arguments expected");
    }

    private LuaExpr Table()
    {
        int line = _l[_i];
        _i++;
        var fields = new List<(object?, LuaExpr)>();
        while (_k[_i] != "}")
        {
            var kind = _k[_i];
            if (kind == "NAME" && _k[_i + 1] == "=")
            {
                var name = _t[_i];
                _i += 2;
                fields.Add((name, SubExpr(0)));
            }
            else if (kind == "[")
            {
                _i++;
                var key = SubExpr(0);
                CheckNext("]");
                CheckNext("=");
                fields.Add((key, SubExpr(0)));
            }
            else fields.Add((null, SubExpr(0)));
            kind = _k[_i];
            if (kind != "," && kind != ";") break;
            _i++;
        }
        CheckMatch("}", "{", line);
        return new TableExpr { Fields = fields, Line = line };
    }

    private (List<string>, bool, LuaBlock) FuncBody(int line, bool isMethod)
    {
        var fs = _fs = new FuncState { Parent = _fs, Line = line, Vararg = false };
        EnterBlock(false);
        if (isMethod)
        {
            NewLocalVar("self");
            AdjustLocalVars(1);
        }
        CheckNext("(");
        var pars = new List<string>();
        bool vararg = false;
        if (_k[_i] != ")")
        {
            while (true)
            {
                var kind = _k[_i];
                if (kind == "NAME")
                {
                    pars.Add(_t[_i]);
                    _i++;
                    NewLocalVar(pars[^1]);
                }
                else if (kind == "...")
                {
                    _i++;
                    vararg = true;
                }
                else throw Error("<name> or '...' expected");
                if (vararg || _k[_i] != ",") break;
                _i++;
            }
        }
        AdjustLocalVars(pars.Count);
        fs.Vararg = vararg;
        CheckNext(")");
        var body = StatList();
        CheckMatch("end", "function", line);
        LeaveBlock();
        _fs = fs.Parent!;
        return (pars, vararg, body);
    }
}
