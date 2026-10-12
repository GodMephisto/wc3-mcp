// src/Wc3.Commands/Lua/LuaPrinter.cs
using System.Globalization;
using System.Text;

namespace Wc3.Commands.Lua;

/// <summary>
/// Prints a Lua tree back to source, ported from lua_ast.py in Devo's Map Doctor (MIT).
/// <see cref="Pretty"/> indents four spaces and keeps every comment. <see cref="Canonical"/> is the
/// shortest form that parses back to the same tree, with no comments, so two scripts that differ
/// only in layout and comments print the same canonical text.
/// </summary>
public sealed class LuaPrinter
{
    private const string Indent = "    ";
    private readonly bool _pretty;
    private readonly string _comma;
    private readonly string _eq;

    private LuaPrinter(bool pretty)
    {
        _pretty = pretty;
        _comma = pretty ? ", " : ",";
        _eq = pretty ? " = " : "=";
    }

    public static string Pretty(LuaChunk chunk) => Pretty(chunk.Body);

    public static string Pretty(LuaBlock body)
    {
        var lines = new LuaPrinter(true).Lines(body, 0);
        return lines.Count == 0 ? "" : string.Join("\n", lines) + "\n";
    }

    public static string Canonical(LuaChunk chunk) => Canonical(chunk.Body);

    public static string Canonical(LuaBlock body)
    {
        var p = new LuaPrinter(false);
        return string.Join(";", body.Select(s => p.Stmt(s, 0)));
    }

    public static string Canonical(string source) => Canonical(LuaParser.Parse(source));

    public static string Expression(LuaExpr e, bool pretty = true) => new LuaPrinter(pretty).Expr(e, 0);

    public static string Statement(LuaStmt s, bool pretty = true)
    {
        var p = new LuaPrinter(pretty);
        return pretty ? string.Join("\n", p.Lines(new LuaBlock { s }, 0)) : p.Stmt(s, 0);
    }

    // ---- joining tokens -----------------------------------------------------------

    private static bool IsWord(char c) => LuaLexer.IsNameChar(c);

    /// <summary>Joins two pieces with a space only where they would otherwise fuse into other tokens.</summary>
    internal static string Cat(string a, string b)
    {
        if (a.Length == 0 || b.Length == 0) return a + b;
        char x = a[^1], y = b[0];
        bool space;
        if (IsWord(x)) space = IsWord(y) || y == '.';
        else if (x == '.') space = y == '.' || IsWord(y);
        else if (x == '-') space = y == '-';
        else if (x == '[') space = y == '[' || y == '=';
        else space = y == '=' && "<>=~".Contains(x);
        return space ? a + " " + b : a + b;
    }

    private string Words(params string[] parts)
    {
        if (_pretty) return string.Join(" ", parts.Where(p => p.Length > 0));
        var output = "";
        foreach (var p in parts) output = Cat(output, p);
        return output;
    }

    private static bool IsName(string s) =>
        s.Length > 0 && LuaLexer.IsNameStart(s[0]) && s.All(LuaLexer.IsNameChar) && !LuaLexer.Keywords.Contains(s);

    private static string? DotName(IndexExpr index)
    {
        if (!index.Dot) return null;
        var name = index.Key switch
        {
            LiteralExpr { Kind: "string", Value: string v } => v,
            NameExpr n => n.Name,
            _ => null,
        };
        return name is not null && IsName(name) ? name : null;
    }

    private static bool NeedsParen(LuaExpr child, int priority, bool left)
    {
        if (child is BinaryExpr b)
        {
            var (cl, cr) = LuaParser.BinaryPriority[b.Op];
            return left ? priority > cr : cl <= priority;
        }
        return child is UnaryExpr && left && priority > LuaParser.UnaryPriority;
    }

    /// <summary>Quotes a string value (one char per byte) as a Lua literal.</summary>
    internal static string Quote(string value)
    {
        var sb = new StringBuilder(value.Length + 2).Append('"');
        foreach (char c in value)
        {
            if (c is '"' or '\\') sb.Append('\\').Append(c);
            else if (c == '\n') sb.Append("\\n");
            else if (c < 32 || c == 127) sb.Append('\\').Append(((int)c).ToString("000", CultureInfo.InvariantCulture));
            else sb.Append(c);
        }
        return sb.Append('"').ToString();
    }

    internal static string LiteralText(LiteralExpr lit)
    {
        switch (lit.Kind)
        {
            case "string": return Quote((string)lit.Value!);
            case "number":
                if (lit.Value is long l) return l.ToString(CultureInfo.InvariantCulture);
                var d = (double)lit.Value!;
                if (double.IsNaN(d)) return "(0/0)";
                if (double.IsInfinity(d)) return d > 0 ? "1e999" : "-1e999";
                var r = d.ToString("R", CultureInfo.InvariantCulture).Replace("E", "e");
                return r.Contains('.') || r.Contains('e') ? r : r + ".0";
            case "boolean": return (bool)lit.Value! ? "true" : "false";
            case "vararg": return "...";
            default: return "nil";
        }
    }

    // ---- expressions ----------------------------------------------------------------

    private string Exprs(IEnumerable<LuaExpr> values, int level) =>
        string.Join(_comma, values.Select(v => Expr(v, level)));

    private string Expr(LuaExpr e, int level)
    {
        switch (e)
        {
            case NameExpr n: return n.Name;
            case LiteralExpr lit: return lit.Text ?? LiteralText(lit);
            case CallExpr or IndexExpr: return Suffixed(e, level);
            case BinaryExpr b: return Binary(b, level);
            case UnaryExpr u:
            {
                var s = Expr(u.Operand, level);
                if (u.Operand is BinaryExpr ob && LuaParser.BinaryPriority[ob.Op].Left <= LuaParser.UnaryPriority)
                    s = "(" + s + ")";
                if (u.Op == "not") return Words("not", s);
                return u.Op == "-" && s.StartsWith('-') ? u.Op + " " + s : u.Op + s;
            }
            case ParenExpr p: return "(" + Expr(p.Inner, level) + ")";
            case TableExpr t: return Table(t, level);
            case FunctionExpr f: return Function("function", f.Params, f.IsVararg, f.Body, level);
            default: throw new ArgumentException($"not a Lua expression node: {e.GetType().Name}");
        }
    }

    private string Suffixed(LuaExpr e, int level)
    {
        var chain = new List<LuaExpr>();
        while (true)
        {
            if (e is CallExpr c) { chain.Add(c); e = c.Func; }
            else if (e is IndexExpr ix) { chain.Add(ix); e = ix.Base; }
            else break;
        }
        var s = Expr(e, level);
        if (e is not NameExpr && e is not ParenExpr) s = "(" + s + ")";
        for (int k = chain.Count - 1; k >= 0; k--)
        {
            if (chain[k] is IndexExpr ix)
            {
                var name = DotName(ix);
                if (name is not null) s += "." + name;
                else
                {
                    var key = Expr(ix.Key, level);
                    s += "[" + (key.StartsWith('[') ? " " : "") + key + "]";
                }
            }
            else
            {
                var call = (CallExpr)chain[k];
                if (call.Method is not null) s += ":" + call.Method;
                s += "(" + Exprs(call.Args, level) + ")";
            }
        }
        return s;
    }

    private string Binary(BinaryExpr e, int level)
    {
        var spine = new List<BinaryExpr>();
        LuaExpr cur = e;
        while (cur is BinaryExpr b) { spine.Add(b); cur = b.Left; }
        var s = Expr(cur, level);
        var left = cur;
        for (int k = spine.Count - 1; k >= 0; k--)
        {
            var n = spine[k];
            var (lp, rp) = LuaParser.BinaryPriority[n.Op];
            if (NeedsParen(left, lp, true)) s = "(" + s + ")";
            var r = Expr(n.Right, level);
            if (NeedsParen(n.Right, rp, false)) r = "(" + r + ")";
            s = Words(s, n.Op, r);
            left = n;
        }
        return s;
    }

    private string Table(TableExpr e, int level)
    {
        var parts = new List<string>();
        foreach (var (key, value) in e.Fields)
        {
            var v = Expr(value, level);
            if (key is null) parts.Add(v);
            else if (key is string ks && IsName(ks)) parts.Add(ks + _eq + v);
            else
            {
                var k = key is string str ? Quote(str) : Expr((LuaExpr)key, level);
                parts.Add("[" + (k.StartsWith('[') ? " " : "") + k + "]" + _eq + v);
            }
        }
        return "{" + string.Join(_comma, parts) + "}";
    }

    private string Function(string head, List<string> pars, bool vararg, LuaBlock body, int level)
    {
        var all = vararg ? pars.Append("...") : pars;
        return Compound(head + "(" + string.Join(_comma, all) + ")", body, "end", level);
    }

    private string Compound(string head, LuaBlock body, string tail, int level)
    {
        if (!_pretty) return Cat(Cat(head, string.Join(";", body.Select(s => Stmt(s, 0)))), tail);
        var lines = Lines(body, level + 1);
        if (lines.Count == 0) return head + " " + tail;
        return head + "\n" + string.Join("\n", lines) + "\n" + Repeat(level) + tail;
    }

    private static string Repeat(int level) => string.Concat(Enumerable.Repeat(Indent, level));

    // ---- statements -------------------------------------------------------------------

    private string Stmt(LuaStmt s, int level)
    {
        switch (s)
        {
            case CallStmt c: return Expr(c.Call, level);
            case AssignStmt a: return Exprs(a.Targets, level) + _eq + Exprs(a.Values, level);
            case LocalStmt l:
            {
                var names = string.Join(_comma, l.Names.Zip(l.Attribs,
                    (n, at) => at is null ? n : n + (_pretty ? $" <{at}>" : $"<{at}>")));
                return l.Values.Count == 0 ? Words("local", names) : Words("local", names, "=", Exprs(l.Values, level));
            }
            case FunctionStmt f:
                return Function(Words(f.IsLocal ? "local function" : "function", f.Name), f.Params, f.IsVararg, f.Body, level);
            case IfStmt i: return If(i, level);
            case ReturnStmt r: return Words("return", Exprs(r.Values, level));
            case WhileStmt w: return Compound(Words("while", Expr(w.Cond, level), "do"), w.Body, "end", level);
            case NumericForStmt nf:
            {
                var range = Expr(nf.Start, level) + _comma + Expr(nf.Stop, level);
                if (nf.Step is not null) range += _comma + Expr(nf.Step, level);
                return Compound(Words("for", nf.Var, "=", range, "do"), nf.Body, "end", level);
            }
            case GenericForStmt gf:
                return Compound(Words("for", string.Join(_comma, gf.Names), "in", Exprs(gf.Exprs, level), "do"),
                    gf.Body, "end", level);
            case RepeatStmt rp: return Compound("repeat", rp.Body, Words("until", Expr(rp.Cond, level)), level);
            case DoStmt d: return Compound("do", d.Body, "end", level);
            case BreakStmt: return "break";
            case GotoStmt g: return Words("goto", g.Label);
            case LabelStmt lb: return "::" + lb.Name + "::";
            default: throw new ArgumentException($"not a Lua statement node: {s.GetType().Name}");
        }
    }

    private string If(IfStmt s, int level)
    {
        if (!_pretty)
        {
            var output = "";
            for (int n = 0; n < s.Branches.Count; n++)
            {
                var (cond, body) = s.Branches[n];
                output = cond is null ? Cat(output, "else") : Words(output, n > 0 ? "elseif" : "if", Expr(cond, level), "then");
                output = Cat(output, string.Join(";", body.Select(x => Stmt(x, 0))));
            }
            return Cat(output, "end");
        }
        var ind = Repeat(level);
        var parts = new List<string>();
        for (int n = 0; n < s.Branches.Count; n++)
        {
            var (cond, body) = s.Branches[n];
            var head = cond is null ? "else" : $"{(n > 0 ? "elseif" : "if")} {Expr(cond, level)} then";
            parts.Add(n > 0 ? ind + head : head);
            parts.AddRange(Lines(body, level + 1));
        }
        parts.Add(ind + "end");
        return string.Join("\n", parts);
    }

    private List<string> Lines(LuaBlock body, int level)
    {
        var ind = Repeat(level);
        var output = new List<string>();
        for (int n = 0; n < body.Count; n++)
        {
            var s = body[n];
            foreach (var c in s.LeadingComments) output.Add(ind + c.Text);
            foreach (var c in s.InnerComments) output.Add(ind + c.Text);
            var text = Stmt(s, level);
            if (n > 0 && text.StartsWith('(')) text = ";" + text;
            if (s.Comment is not null) text += " " + s.Comment.Text;
            output.Add(ind + text);
        }
        foreach (var c in body.EndComments) output.Add(ind + c.Text);
        return output;
    }
}
