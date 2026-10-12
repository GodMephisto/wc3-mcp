// src/Wc3.Commands/Lua/Lua53Translate.cs
// Ported from Devo's Map Doctor, doctor/script/lua53_translate.py (commit 80efaf6), MIT, devoltzz.
// See THIRD-PARTY-NOTICES.md. Kept function for function with the Python so the outputs compare.
using System.Globalization;
using System.Text;

namespace Wc3.Commands.Lua;

/// <summary>Bytecode the translator cannot turn back into source, with the reason and the pc.</summary>
public sealed class LuaTranslateError(string message) : Exception(message);

/// <summary>
/// Turns a precompiled Lua 5.3 chunk back into Lua source that runs the same, one statement per
/// instruction. Every register is a local, every jump a goto, and a register a closure captures
/// lives in a one slot table so the closure and the function share it. The result is not the
/// source the author wrote, it is a faithful transcription of what the bytecode does.
/// </summary>
/// <remarks>
/// The translated text is a chunk that returns function(ENV, ...), which runs the original main
/// function with ENV as its _ENV. Both the standard Lua 5.3 opcode numbering and the shuffled one
/// of the KK YDWE engine are read, told apart by the opcode every function ends with.
/// </remarks>
public static class Lua53Translate
{
    private const int FieldsPerFlush = 50;
    private const int MaxLocalRegs = 190;

    internal const int MOVE = 0, LOADK = 1, LOADKX = 2, LOADBOOL = 3, LOADNIL = 4, GETUPVAL = 5, GETTABUP = 6,
        GETTABLE = 7, SETTABUP = 8, SETUPVAL = 9, SETTABLE = 10, NEWTABLE = 11, SELF = 12, ADD = 13, SUB = 14,
        MUL = 15, MOD = 16, POW = 17, DIV = 18, IDIV = 19, BAND = 20, BOR = 21, BXOR = 22, SHL = 23, SHR = 24,
        UNM = 25, BNOT = 26, NOT = 27, LEN = 28, CONCAT = 29, JMP = 30, EQ = 31, LT = 32, LE = 33, TEST = 34,
        TESTSET = 35, CALL = 36, TAILCALL = 37, RETURN = 38, FORLOOP = 39, FORPREP = 40, TFORCALL = 41,
        TFORLOOP = 42, SETLIST = 43, CLOSURE = 44, VARARG = 45, EXTRAARG = 46;

    /// <summary>The KK YDWE engine's opcode number to the standard one.</summary>
    private static readonly int[] KkYdweOpcodes = BuildKk();

    private static int[] BuildKk()
    {
        var pairs = new (int, int)[]
        {
            (27, 0), (26, 1), (29, 2), (28, 3), (31, 4), (30, 5), (33, 6), (32, 7), (35, 8), (34, 9), (37, 10),
            (36, 11), (39, 12), (38, 13), (41, 14), (40, 15), (43, 16), (42, 17), (45, 18), (44, 19), (1, 20),
            (2, 21), (0, 22), (4, 23), (3, 24), (6, 25), (5, 26), (8, 27), (7, 28), (10, 29), (9, 30), (12, 31),
            (11, 32), (14, 33), (13, 34), (16, 35), (15, 36), (18, 37), (17, 38), (20, 39), (19, 40), (21, 41),
            (22, 42), (24, 43), (23, 44), (46, 45), (25, 46),
        };
        var t = new int[47];
        foreach (var (k, v) in pairs) t[k] = v;
        return t;
    }

    private static readonly Dictionary<int, string> Arith = new()
    {
        [ADD] = "+", [SUB] = "-", [MUL] = "*", [MOD] = "%", [POW] = "^", [DIV] = "/", [IDIV] = "//",
        [BAND] = "&", [BOR] = "|", [BXOR] = "~", [SHL] = "<<", [SHR] = ">>",
    };

    private static readonly Dictionary<int, string> Unary = new() { [UNM] = "-", [BNOT] = "~", [NOT] = "not ", [LEN] = "#" };
    private static readonly Dictionary<int, string> Compare = new() { [EQ] = "==", [LT] = "<", [LE] = "<=" };

    public const string Prelude = """
        local __mt, __tonumber, __floor, __ceil, __maxi, __mini, __error, __select =
          math.type, tonumber, math.floor, math.ceil, math.maxinteger, math.mininteger, error, select
        local function __toint(v, mode)
          -- luaV_tointeger: an integer, or a float/string with an exact integer value (mode 1: floor, 2: ceil)
          if __mt(v) == 'integer' then return v end
          local n = __tonumber(v)
          if n == nil then return nil end
          if __mt(n) == 'integer' then return n end
          if n ~= n then return nil end
          local f = (mode == 2) and __ceil(n) or __floor(n)
          if f ~= f or f >= 2^63 or f < -2^63 then return nil end
          return __mt(f) == 'integer' and f or math.tointeger(f)
        end
        local function __forprep(init, limit, step)
          if __mt(init) == 'integer' and __mt(step) == 'integer' then
            local stop = false
            local il = __toint(limit, step < 0 and 2 or 1)
            if il == nil then
              local n = __tonumber(limit)
              if n ~= nil then
                if 0 < n then il = __maxi; if step < 0 then stop = true end
                else il = __mini; if step >= 0 then stop = true end end
              end
            end
            if il ~= nil then
              if stop then init = 0 end
              return init - step, il, step
            end
          end
          local nl = __tonumber(limit)
          if nl == nil then __error("'for' limit must be a number", 2) end
          local ns = __tonumber(step)
          if ns == nil then __error("'for' step must be a number", 2) end
          local ni = __tonumber(init)
          if ni == nil then __error("'for' initial value must be a number", 2) end
          return (ni + 0.0) - (ns + 0.0), nl + 0.0, ns + 0.0
        end
        local function __setlist(t, base, ...)
          for i = 1, __select('#', ...) do t[base + i] = (__select(i, ...)) end
        end
        local F = {}

        """;

    /// <summary>The closing line of a translation, which hands the main function back to the caller.</summary>
    public const string ReturnLine = "return function(ENV, ...) return F[0]({{ENV}})(...) end\n";

    /// <summary>"standard", "kk_ydwe", or null when the last opcodes fit neither numbering.</summary>
    public static string? OpcodeTable(LuaProto main)
    {
        var last = main.Walk().Where(g => g.Code.Length > 0).Select(g => (int)(g.Code[^1] & 0x3F)).ToHashSet();
        if (last.SetEquals([38])) return "standard";
        if (last.SetEquals([17])) return "kk_ydwe";
        return null;
    }

    private static void Renumber(LuaProto main, string table)
    {
        if (table == "standard") return;
        foreach (var g in main.Walk())
            g.Code = g.Code.Select(i => (i & ~0x3Fu) | (uint)KkYdweOpcodes[i & 0x3F]).ToArray();
    }

    private readonly record struct Ins(int Op, int A, int B, int C, int Bx, int SBx, int Ax);

    private static Ins Decode(uint i)
    {
        int bx = (int)((i >> 14) & 0x3FFFF);
        return new Ins((int)(i & 0x3F), (int)((i >> 6) & 0xFF), (int)((i >> 23) & 0x1FF), (int)((i >> 14) & 0x1FF),
            bx, bx - 131071, (int)(i >> 6));
    }

    /// <summary>A constant as Lua source.</summary>
    public static string Literal(object? v)
    {
        switch (v)
        {
            case null: return "nil";
            case bool b: return b ? "true" : "false";
            case long n:
                if (n == long.MinValue) return "math.mininteger";
                if (!(n > -2147483648L && n < 2147483648L)) return "0x" + ((ulong)n).ToString("x", CultureInfo.InvariantCulture);
                return n >= 0 ? n.ToString(CultureInfo.InvariantCulture) : $"({n.ToString(CultureInfo.InvariantCulture)})";
            case double d:
                if (double.IsNaN(d)) return "(0/0)";
                if (double.IsPositiveInfinity(d)) return "(1/0)";
                if (double.IsNegativeInfinity(d)) return "(-1/0)";
                if (d == 0.0) return double.IsNegative(d) ? "(-0.0)" : "0.0";
                {
                    string s = PythonRepr(d);
                    if (!s.Contains('e') && !s.Contains('.')) s += ".0";
                    return d > 0 ? s : $"({s})";
                }
            case byte[] bytes:
            {
                var sb = new StringBuilder(bytes.Length + 2);
                sb.Append('"');
                foreach (byte ch in bytes)
                {
                    if (ch == 0x22) sb.Append("\\\"");
                    else if (ch == 0x5C) sb.Append("\\\\");
                    else if (ch is >= 0x20 and < 0x7F) sb.Append((char)ch);
                    else if (ch == 0x0A) sb.Append("\\n");
                    else sb.Append('\\').Append(ch.ToString("D3", CultureInfo.InvariantCulture));
                }
                sb.Append('"');
                return sb.ToString();
            }
        }
        throw new ArgumentException($"constant of type {v.GetType().Name}");
    }

    /// <summary>
    /// A finite nonzero double as Python's repr writes it. The shortest round trip digits, fixed
    /// notation when the decimal point falls between 10^-4 and 10^16, otherwise d.ddde+XX.
    /// </summary>
    internal static string PythonRepr(double d)
    {
        string r = Math.Abs(d).ToString("R", CultureInfo.InvariantCulture);
        int e = 0;
        int ei = r.IndexOfAny(['E', 'e']);
        if (ei >= 0)
        {
            e = int.Parse(r[(ei + 1)..], NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
            r = r[..ei];
        }
        int dot = r.IndexOf('.');
        string ip = dot < 0 ? r : r[..dot], fp = dot < 0 ? "" : r[(dot + 1)..];
        string digits = ip + fp;
        int decpt = ip.Length + e;
        int lead = 0;
        while (lead < digits.Length - 1 && digits[lead] == '0') lead++;
        digits = digits[lead..];
        decpt -= lead;
        digits = digits.TrimEnd('0');
        if (digits.Length == 0) digits = "0";
        string sign = d < 0 ? "-" : "";
        if (decpt > -4 && decpt <= 16)
        {
            if (decpt <= 0) return sign + "0." + new string('0', -decpt) + digits;
            if (decpt >= digits.Length) return sign + digits + new string('0', decpt - digits.Length) + ".0";
            return sign + digits[..decpt] + "." + digits[decpt..];
        }
        int x = decpt - 1;
        string mant = digits.Length == 1 ? digits : digits[0] + "." + digits[1..];
        return sign + mant + "e" + (x < 0 ? "-" : "+") + Math.Abs(x).ToString("D2", CultureInfo.InvariantCulture);
    }

    private sealed record Proto(LuaProto F, int Id, List<int> ChildIds);

    private static List<Proto> Number(LuaProto main)
    {
        var output = new List<Proto?>();
        int Visit(LuaProto f)
        {
            int me = output.Count;
            output.Add(null);
            var kids = f.Protos.Select(Visit).ToList();
            output[me] = new Proto(f, me, kids);
            return me;
        }
        Visit(main);
        return output.Select(p => p!).ToList();
    }

    /// <summary>Translates a chunk. <paramref name="name"/> only labels the header and errors.</summary>
    public static string Translate(byte[] data, string name = "chunk")
    {
        var (main, whole) = LuaBytecode.Load(data);
        if (!whole) throw new LuaTranslateError($"{name}: bytes left after the chunk");
        var table = OpcodeTable(main)
            ?? throw new LuaTranslateError($"{name}: the opcode numbering is neither standard Lua 5.3 nor the KK YDWE engine one");
        Renumber(main, table);
        var protos = Number(main);
        var parts = new List<string>
        {
            $"-- {name}: translated from Lua 5.3 bytecode ({protos.Count} functions)",
            Prelude.Replace("\r\n", "\n"),
        };
        foreach (var pr in protos) parts.Add(Function(pr));
        if (main.Upvals.Count != 1)
            throw new LuaTranslateError($"{name}: the main function has {main.Upvals.Count} upvalues (one, _ENV, expected)");
        parts.Add(ReturnLine.TrimEnd('\n'));
        return string.Join("\n", parts) + "\n";
    }

    private static string Function(Proto pr)
    {
        var f = pr.F;
        var code = f.Code;
        var consts = f.Consts;
        int n = code.Length;
        int stack = Math.Max(Math.Max(f.Stack, f.Params), 2);
        var captured = new HashSet<int>();
        foreach (var i in code)
        {
            var d = Decode(i);
            if (d.Op == CLOSURE)
                foreach (var (instack, idx) in f.Protos[d.Bx].Upvals)
                    if (instack != 0) captured.Add(idx);
        }

        string R(int k) => captured.Contains(k) ? $"b{k}[1]" : k >= MaxLocalRegs ? $"X[{k}]" : $"r{k}";
        string K(int k) => Literal(consts[k]);
        string RK(int x) => (x & 0x100) != 0 ? K(x & 0xFF) : R(x);
        List<string> Regs(int a, int count) => Enumerable.Range(a, Math.Max(0, count)).Select(R).ToList();

        var targets = new HashSet<int>();
        for (int pc = 0; pc < n; pc++)
        {
            var d = Decode(code[pc]);
            if (d.Op is JMP or FORLOOP or FORPREP or TFORLOOP) targets.Add(pc + 1 + d.SBx);
            else if (d.Op is EQ or LT or LE or TEST or TESTSET) targets.Add(pc + 2);
            else if (d.Op == LOADBOOL && d.C != 0) targets.Add(pc + 2);
        }
        var output = new List<string>();
        var ps = Enumerable.Range(0, f.Params).Select(k => captured.Contains(k) ? $"p{k}" : $"r{k}").ToList();
        if (f.Vararg != 0) ps.Add("...");
        output.Add($"F[{pr.Id}] = function(U) return function({string.Join(", ", ps)})");
        var localRegs = Enumerable.Range(f.Params, Math.Max(0, Math.Min(stack, MaxLocalRegs) - f.Params))
            .Where(k => !captured.Contains(k)).ToList();
        for (int k = 0; k < localRegs.Count; k += 60)
            output.Add("local " + string.Join(", ", localRegs.Skip(k).Take(60).Select(r => $"r{r}")));
        if (stack > MaxLocalRegs) output.Add("local X = {}");
        var boxes = captured.OrderBy(k => k).ToList();
        foreach (var k in boxes)
            output.Add(k < f.Params ? $"local b{k} = {{p{k}}}" : $"local b{k} = {{}}");
        (int Start, string Expr)? pending = null;
        var skip = new HashSet<int>();

        string CloseFrom(int a) => string.Join(" ", boxes.Where(k => k >= a).Select(k => $"b{k} = {{b{k}[1]}}"));

        for (int pc = 0; pc < n; pc++)
        {
            if (skip.Contains(pc)) continue;
            var (op, a, b, c, bx, sbx, ax) = Decode(code[pc]);
            if (targets.Contains(pc))
            {
                if (pending is not null) throw new LuaTranslateError($"a jump lands inside an open result list at pc {pc}");
                output.Add($"::L{pc}::");
            }

            List<string> Args(int first, int? countOrOpen)
            {
                if (countOrOpen is null)
                {
                    if (pending is not { } p) throw new LuaTranslateError($"open argument list without a pending result at pc {pc}");
                    var l = Enumerable.Range(first, Math.Max(0, p.Start - first)).Select(R).ToList();
                    l.Add(p.Expr);
                    return l;
                }
                return Enumerable.Range(first, Math.Max(0, countOrOpen.Value)).Select(R).ToList();
            }

            string s;
            switch (op)
            {
                case MOVE: s = $"{R(a)} = {R(b)}"; break;
                case LOADK: s = $"{R(a)} = {K(bx)}"; break;
                case LOADKX:
                {
                    if (pc + 1 >= n || Decode(code[pc + 1]).Op != EXTRAARG) throw new LuaTranslateError($"LOADKX without EXTRAARG at pc {pc}");
                    s = $"{R(a)} = {K(Decode(code[pc + 1]).Ax)}";
                    skip.Add(pc + 1);
                    break;
                }
                case LOADBOOL:
                    s = $"{R(a)} = {(b != 0 ? "true" : "false")}";
                    if (c != 0) s += $" goto L{pc + 2}";
                    break;
                case LOADNIL: s = string.Join(" ", Enumerable.Range(0, b + 1).Select(k => $"{R(a + k)} = nil")); break;
                case GETUPVAL: s = $"{R(a)} = U[{b + 1}][1]"; break;
                case GETTABUP: s = $"{R(a)} = U[{b + 1}][1][{RK(c)}]"; break;
                case GETTABLE: s = $"{R(a)} = {R(b)}[{RK(c)}]"; break;
                case SETTABUP: s = $"U[{a + 1}][1][{RK(b)}] = {RK(c)}"; break;
                case SETUPVAL: s = $"U[{b + 1}][1] = {R(a)}"; break;
                case SETTABLE: s = $"{R(a)}[{RK(b)}] = {RK(c)}"; break;
                case NEWTABLE: s = $"{R(a)} = {{}}"; break;
                case SELF: s = $"{R(a + 1)} = {R(b)} {R(a)} = {R(a + 1)}[{RK(c)}]"; break;
                case >= ADD and <= SHR: s = $"{R(a)} = {RK(b)} {Arith[op]} {RK(c)}"; break;
                case >= UNM and <= LEN: s = $"{R(a)} = {Unary[op]}{R(b)}"; break;
                case CONCAT: s = $"{R(a)} = {string.Join(" .. ", Enumerable.Range(b, Math.Max(0, c - b + 1)).Select(R))}"; break;
                case JMP:
                {
                    string pre = a != 0 ? CloseFrom(a - 1) : "";
                    s = (pre.Length > 0 ? pre + " " : "") + $"goto L{pc + 1 + sbx}";
                    break;
                }
                case EQ or LT or LE:
                {
                    string cond = $"{RK(b)} {Compare[op]} {RK(c)}";
                    s = a != 0 ? $"if not ({cond}) then goto L{pc + 2} end" : $"if {cond} then goto L{pc + 2} end";
                    break;
                }
                case TEST:
                    s = c != 0 ? $"if not {R(a)} then goto L{pc + 2} end" : $"if {R(a)} then goto L{pc + 2} end";
                    break;
                case TESTSET:
                    s = c != 0
                        ? $"if {R(b)} then {R(a)} = {R(b)} else goto L{pc + 2} end"
                        : $"if not {R(b)} then {R(a)} = {R(b)} else goto L{pc + 2} end";
                    break;
                case CALL or TAILCALL:
                {
                    var argl = Args(a + 1, b == 0 ? null : b - 1);
                    if (b == 0) pending = null;
                    string call = $"{R(a)}({string.Join(", ", argl)})";
                    if (op == TAILCALL)
                    {
                        s = $"do return {call} end";
                        if (pc + 1 < n && Decode(code[pc + 1]).Op == RETURN && !targets.Contains(pc + 1)) skip.Add(pc + 1);
                    }
                    else if (c == 0)
                    {
                        pending = (a, call);
                        continue;
                    }
                    else if (c == 1) s = call;
                    else s = $"{string.Join(", ", Regs(a, c - 1))} = {call}";
                    break;
                }
                case RETURN:
                {
                    List<string> vals;
                    if (b == 0)
                    {
                        vals = Args(a, null);
                        pending = null;
                    }
                    else vals = Regs(a, b - 1);
                    s = vals.Count > 0 ? $"do return {string.Join(", ", vals)} end" : "do return end";
                    break;
                }
                case FORPREP:
                    s = $"{R(a)}, {R(a + 1)}, {R(a + 2)} = __forprep({R(a)}, {R(a + 1)}, {R(a + 2)}) goto L{pc + 1 + sbx}";
                    break;
                case FORLOOP:
                {
                    string ra = R(a), lim = R(a + 1), step = R(a + 2), ext = R(a + 3);
                    s = $"{ra} = {ra} + {step} if (0 < {step} and {ra} <= {lim}) or (not (0 < {step}) and {lim} <= {ra}) then {ext} = {ra} goto L{pc + 1 + sbx} end";
                    break;
                }
                case TFORCALL: s = $"{string.Join(", ", Regs(a + 3, c))} = {R(a)}({R(a + 1)}, {R(a + 2)})"; break;
                case TFORLOOP: s = $"if {R(a + 1)} ~= nil then {R(a)} = {R(a + 1)} goto L{pc + 1 + sbx} end"; break;
                case SETLIST:
                {
                    if (c == 0)
                    {
                        if (pc + 1 >= n || Decode(code[pc + 1]).Op != EXTRAARG) throw new LuaTranslateError($"SETLIST without EXTRAARG at pc {pc}");
                        c = Decode(code[pc + 1]).Ax;
                        skip.Add(pc + 1);
                    }
                    int bas = (c - 1) * FieldsPerFlush;
                    if (b == 0)
                    {
                        var vals = Args(a + 1, null);
                        pending = null;
                        s = $"__setlist({R(a)}, {bas}, {string.Join(", ", vals)})";
                    }
                    else s = string.Join(" ", Enumerable.Range(1, b).Select(k => $"{R(a)}[{bas + k}] = {R(a + k)}"));
                    break;
                }
                case CLOSURE:
                {
                    var child = f.Protos[bx];
                    var ups = child.Upvals.Select(u => u.InStack != 0 ? $"b{u.Index}" : $"U[{u.Index + 1}]");
                    s = $"{R(a)} = F[{pr.ChildIds[bx]}]({{{string.Join(", ", ups)}}})";
                    break;
                }
                case VARARG:
                    if (b == 0)
                    {
                        pending = (a, "...");
                        continue;
                    }
                    s = $"{string.Join(", ", Regs(a, b - 1))} = ...";
                    break;
                case EXTRAARG: throw new LuaTranslateError($"EXTRAARG alone at pc {pc}");
                default: throw new LuaTranslateError($"unknown opcode {op} at pc {pc}");
            }
            if (pending is not null)
                throw new LuaTranslateError($"an open result list at pc {pc - 1} is not taken by the next instruction");
            output.Add(s);
        }
        if (pending is not null) throw new LuaTranslateError("the function ends with an open result list");
        if (n > 0 && Decode(code[^1]).Op is not (RETURN or TAILCALL or JMP)) output.Add("do return end");
        output.Add("end end");
        return string.Join("\n", output);
    }
}
