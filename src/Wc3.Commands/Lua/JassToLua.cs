// src/Wc3.Commands/Lua/JassToLua.cs
// Ported from Devo's Map Doctor, doctor/script/jass2lua.py (commit 80efaf6), MIT, devoltzz.
// See THIRD-PARTY-NOTICES.md. The structure follows the Python on purpose, function for
// function, so a difference between the two outputs can be traced to one place.
using System.Globalization;
using System.Numerics;
using System.Text;

namespace Wc3.Commands.Lua;

/// <summary>A JASS script that the converter cannot read, with the reason and usually the line.</summary>
public sealed class JassToLuaError(string message) : Exception(message);

/// <summary>
/// What a conversion produced. <see cref="LuaFunctions"/> and <see cref="LuaGlobals"/> are the two
/// halves of the script, <see cref="ExsLiterals"/> are the EXExecuteScript texts the functions call
/// by index, <see cref="LuaChunks"/> the Lua text a map embedded through lua_load strings.
/// </summary>
public sealed record JassToLuaResult(
    string LuaFunctions,
    string LuaGlobals,
    IReadOnlyList<string> LuaChunks,
    IReadOnlyList<string> ExsLiterals,
    IReadOnlyDictionary<string, string> HandleGlobals,
    IReadOnlyList<string> FuncNames,
    IReadOnlyList<string> OverridesWithoutTarget,
    IReadOnlyList<string> Warnings,
    IReadOnlyDictionary<string, (IReadOnlyList<string> Params, string Ret)> MapNatives,
    bool StrictReals = false,
    int HashtablesRewritten = 0);

/// <summary>
/// Converts a JASS map script to Lua for Warcraft III 1.31 and later, keeping the JASS rules Lua
/// does not share. Integer arithmetic wraps at 32 bits, integer division truncates toward zero,
/// division by zero yields zero instead of raising, a null string concatenates as empty, and an
/// array reads its type's default for a slot never written.
/// </summary>
/// <remarks>
/// The script is handled as Latin-1 text, one char per byte, the same convention as the rest of
/// the toolkit. The Python reads UTF-8 with surrogateescape, which gives back the same bytes on
/// write, so the two produce byte identical Lua for the same input.
/// </remarks>
public static class JassToLua
{
    /// <summary>Lua refuses a function with more than 200 locals. Past this many, the rest spill into a table.</summary>
    public const int LocalsInLua = 180;

    internal static readonly HashSet<string> Keywords = new(StringComparer.Ordinal)
    {
        "globals", "endglobals", "native", "constant", "type", "extends", "function", "takes", "returns",
        "endfunction", "local", "set", "call", "if", "then", "else", "elseif", "endif", "loop", "endloop",
        "exitwhen", "return", "debug", "and", "or", "not", "null", "true", "false", "array", "nothing",
    };

    internal static readonly HashSet<string> LuaKeywords = new(StringComparer.Ordinal)
    {
        "and", "break", "do", "else", "elseif", "end", "false", "for", "function", "goto", "if", "in", "local",
        "nil", "not", "or", "repeat", "return", "then", "true", "until", "while",
    };

    internal static readonly HashSet<string> Primitives = new(StringComparer.Ordinal)
    {
        "integer", "real", "string", "boolean", "code", "nothing",
    };

    /// <summary>The hashtable natives the rewrite understands, mapped from the native's own suffix
    /// to the type it stores. Shared with the Lua to JASS direction, which inverts it to name the
    /// native back, so the two stay in step by construction.</summary>
    internal static readonly Dictionary<string, string> HashtableStore = new(StringComparer.Ordinal)
    {
        ["Integer"] = "integer", ["Real"] = "real", ["Boolean"] = "boolean", ["Str"] = "string",
        ["UnitHandle"] = "unit", ["ItemHandle"] = "item", ["DestructableHandle"] = "destructable",
        ["TimerHandle"] = "timer", ["TriggerHandle"] = "trigger", ["EffectHandle"] = "effect",
        ["GroupHandle"] = "group", ["LocationHandle"] = "location", ["RegionHandle"] = "region",
        ["LightningHandle"] = "lightning", ["FogModifierHandle"] = "fogmodifier",
        ["AbilityHandle"] = "ability",
    };

    /// <summary>The suffix a hashtable helper carries for the type it stores. It is the type's own
    /// name, so the Lua to JASS direction can name the native back. A Lua table is untyped, so
    /// without it a rewritten SaveInteger and SaveReal would be indistinguishable, and every handle
    /// kind would collapse to one name.</summary>
    internal static string HashtableSuffix(string type) => type;

    /// <summary>
    /// Converts <paramref name="mapSource"/>. <paramref name="commonJ"/> and <paramref name="blizzardJ"/>
    /// supply the native and Blizzard.j signatures, which decide where an integer operation needs
    /// wrapping and what a null argument becomes. Either may be empty, at the cost of an
    /// "unknown function" warning for each call into it.
    /// </summary>
    public static JassToLuaResult Transpile(
        string commonJ, string blizzardJ, string mapSource,
        IReadOnlyDictionary<string, string>? overrides = null,
        IEnumerable<string>? dropFunctions = null,
        IReadOnlyDictionary<string, string>? translate = null,
        bool strictReals = false,
        bool hashtables = false)
        => new Transpiler(commonJ, blizzardJ, mapSource, overrides, dropFunctions, translate, strictReals, hashtables).Run();

    /// <summary>
    /// The runtime the converted functions call into. The helpers keep the JASS rules, and the
    /// Lua library functions they need are captured first, since a map may declare a JASS function
    /// named select or load and that would replace the global.
    /// </summary>
    public const string Prelude = """
        local select, setmetatable, load, pcall, tostring, type, math, table = select, setmetatable, load, pcall, tostring, type, math, table
        local __mt_i = { __index = function() return 0 end }
        local __mt_r = { __index = function() return 0.0 end }
        local __mt_b = { __index = function() return false end }
        function __arr_i() return setmetatable({}, __mt_i) end
        function __arr_r() return setmetatable({}, __mt_r) end
        function __arr_b() return setmetatable({}, __mt_b) end
        function __cat(...)
          local n = select("#", ...)
          if n == 2 then
            local a, b = ...
            return (a == nil and "" or a) .. (b == nil and "" or b)
          end
          local t = {...}
          for i = 1, n do
            local v = t[i]
            if v == nil then t[i] = "" elseif type(v) ~= "string" then t[i] = tostring(v) end
          end
          return table.concat(t, "", 1, n)
        end
        function __idiv(a, b)
          if b == 0 or b == nil or a == nil then return 0 end
          if math.type(a) == "integer" and math.type(b) == "integer" then
            local q = a // b
            if q < 0 and q * b ~= a then q = q + 1 end
            return q
          end
          local q = a / b
          if q >= 0 then q = math.floor(q) else q = math.ceil(q) end
          return math.tointeger(q) or q
        end
        function __div(a, b)
          if b == 0 or b == nil or a == nil then return 0.0 end
          return a / b
        end
        local function __wrap32(r)
          if math.type(r) == "integer" and (r > 2147483647 or r < -2147483648) then
            r = ((r + 2147483648) & 0xFFFFFFFF) - 2147483648
          end
          return r
        end
        function __imul(a, b) return __wrap32(a * b) end
        function __iadd(a, b) return __wrap32(a + b) end
        function __isub(a, b) return __wrap32(a - b) end
        function __exs(n)
          local s = __exs_src[n]
          if s == nil then return "" end
          local f = load("return " .. s, "=exs", "t") or load(s, "=exs", "t")
          if f == nil then return "" end
          local ok, r = pcall(f)
          if not ok or r == nil then return "" end
          return tostring(r)
        end
        function __luachunk(n)
          local s = __luachunk_src[n]
          local f = s and load(s, "=luachunk", "t")
          if f then pcall(f) end
        end
        """;

    /// <summary>
    /// The helpers a strict-real conversion adds. JASS reals are 32-bit floats and Lua numbers are
    /// 64-bit doubles, so a converted <c>a + b</c> on reals drifts where the original did not.
    /// Each helper rounds its result back through an IEEE single, which is what the JASS runtime
    /// holds. Only appended when the conversion asked for it, so the default output stays byte
    /// identical to the upstream port.
    /// </summary>
    public const string RealPrelude = """
        local __pack, __unpack = string.pack, string.unpack
        local function __f32(x)
          if type(x) ~= "number" then return x end
          return (__unpack("f", __pack("f", x)))
        end
        function __fadd(a, b) return __f32((a or 0.0) + (b or 0.0)) end
        function __fsub(a, b) return __f32((a or 0.0) - (b or 0.0)) end
        function __fmul(a, b) return __f32((a or 0.0) * (b or 0.0)) end
        function __fdiv(a, b)
          if b == 0 or b == nil or a == nil then return 0.0 end
          return __f32(a / b)
        end
        """;

    /// <summary>
    /// The helpers a hashtable rewrite adds. A JASS hashtable is two level, a parent key to a child
    /// key, and the natives exist in Lua, so leaving them alone is correct, only slow and unidiomatic.
    /// These keep the same shape on a plain Lua table, and the default a load returns when the slot
    /// was never written matches the JASS type.
    /// </summary>
    public const string HashtablePrelude = """
        local __pairs = pairs
        local function __hs(t, k)
          local p = t[k]
          if p == nil then p = {} t[k] = p end
          return p
        end
        function __hs_save(t, k, c, v) __hs(t, k)[c] = v end
        function __hs_load(t, k, c, d)
          local p = t[k]
          if p == nil then return d end
          local v = p[c]
          if v == nil then return d end
          return v
        end
        local function __hs_has(t, k, c) local p = t[k] return p ~= nil and p[c] ~= nil end
        local function __hs_remove(t, k, c) local p = t[k] if p ~= nil then p[c] = nil end end
        function __hs_flush_child(t, k) t[k] = nil end
        function __hs_flush(t) for k in __pairs(t) do t[k] = nil end end
        -- One alias per stored type, so the Lua to JASS direction can name the native back. The
        -- suffix is the type's own name, integer, real, boolean, string and the handle kinds.
        __hs_save_integer, __hs_save_real, __hs_save_boolean, __hs_save_string, __hs_save_unit, __hs_save_item, __hs_save_destructable, __hs_save_timer, __hs_save_trigger, __hs_save_effect, __hs_save_group, __hs_save_location, __hs_save_region, __hs_save_lightning, __hs_save_fogmodifier, __hs_save_ability = __hs_save, __hs_save, __hs_save, __hs_save, __hs_save, __hs_save, __hs_save, __hs_save, __hs_save, __hs_save, __hs_save, __hs_save, __hs_save, __hs_save, __hs_save, __hs_save
        __hs_load_integer, __hs_load_real, __hs_load_boolean, __hs_load_string, __hs_load_unit, __hs_load_item, __hs_load_destructable, __hs_load_timer, __hs_load_trigger, __hs_load_effect, __hs_load_group, __hs_load_location, __hs_load_region, __hs_load_lightning, __hs_load_fogmodifier, __hs_load_ability = __hs_load, __hs_load, __hs_load, __hs_load, __hs_load, __hs_load, __hs_load, __hs_load, __hs_load, __hs_load, __hs_load, __hs_load, __hs_load, __hs_load, __hs_load, __hs_load
        __hs_has_integer, __hs_has_real, __hs_has_boolean, __hs_has_string, __hs_has_unit, __hs_has_item, __hs_has_destructable, __hs_has_timer, __hs_has_trigger, __hs_has_effect, __hs_has_group, __hs_has_location, __hs_has_region, __hs_has_lightning, __hs_has_fogmodifier, __hs_has_ability = __hs_has, __hs_has, __hs_has, __hs_has, __hs_has, __hs_has, __hs_has, __hs_has, __hs_has, __hs_has, __hs_has, __hs_has, __hs_has, __hs_has, __hs_has, __hs_has
        __hs_remove_integer, __hs_remove_real, __hs_remove_boolean, __hs_remove_string, __hs_remove_unit, __hs_remove_item, __hs_remove_destructable, __hs_remove_timer, __hs_remove_trigger, __hs_remove_effect, __hs_remove_group, __hs_remove_location, __hs_remove_region, __hs_remove_lightning, __hs_remove_fogmodifier, __hs_remove_ability = __hs_remove, __hs_remove, __hs_remove, __hs_remove, __hs_remove, __hs_remove, __hs_remove, __hs_remove, __hs_remove, __hs_remove, __hs_remove, __hs_remove, __hs_remove, __hs_remove, __hs_remove, __hs_remove
        """;

    /// <summary>
    /// The whole war3map.lua for a conversion. The prelude, the EXExecuteScript and lua_load texts,
    /// the functions, then the globals in a function main and config run before themselves, since
    /// JASS sets its globals before either runs and a native called while the file loads may not
    /// be ready yet.
    /// </summary>
    public static string BuildScript(JassToLuaResult r)
    {
        var sb = new StringBuilder();
        sb.Append("-- war3map.lua, converted from war3map.j by wc3ctl script to-lua\n");
        sb.Append(Prelude.Replace("\r\n", "\n")).Append('\n');
        if (r.StrictReals) sb.Append(RealPrelude.Replace("\r\n", "\n")).Append('\n');
        if (r.HashtablesRewritten > 0) sb.Append(HashtablePrelude.Replace("\r\n", "\n")).Append('\n');
        AppendTable(sb, "__exs_src", r.ExsLiterals);
        AppendTable(sb, "__luachunk_src", r.LuaChunks);
        sb.Append(r.LuaFunctions).Append('\n');
        sb.Append("function __jass_globals()\n");
        if (r.LuaGlobals.Length > 0) sb.Append(r.LuaGlobals).Append('\n');
        sb.Append("end\n");
        sb.Append("""
            do
              local __main, __config, __ready = main, config, false
              local function __init()
                if not __ready then
                  __ready = true
                  __jass_globals()
                end
              end
              main = function() __init() if __main then return __main() end end
              config = function() __init() if __config then return __config() end end
            end

            """.Replace("\r\n", "\n"));
        return sb.ToString();
    }

    private static void AppendTable(StringBuilder sb, string name, IReadOnlyList<string> items)
    {
        sb.Append(name).Append(" = {");
        for (int i = 0; i < items.Count; i++)
            sb.Append(i == 0 ? "\n" : ",\n").Append('[').Append(i).Append("] = ").Append(LuaString(items[i]));
        sb.Append(items.Count == 0 ? "}\n" : "\n}\n");
    }

    internal static long Wrap32(BigInteger v)
    {
        var r = ((v + 0x80000000L) & 0xFFFFFFFFL) - 0x80000000L;
        return (long)r;
    }

    // ---------------------------------------------------------------- lexer

    internal sealed class Tok(string kind, object? val, int line)
    {
        public readonly string Kind = kind;
        public readonly object? Val = val;
        public readonly int Line = line;
        public string? S => Val as string;
        public override string ToString() => $"{Kind}({Val})@{Line}";
    }

    private static bool IsDigit(char c) => c is >= '0' and <= '9';
    private static bool IsHex(char c) => IsDigit(c) || c is >= 'a' and <= 'f' or >= 'A' and <= 'F';
    private static bool IsIdStart(char c) => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_';
    private static bool IsIdChar(char c) => IsIdStart(c) || IsDigit(c);

    internal static List<Tok> Lex(string src)
    {
        var toks = new List<Tok>();
        int i = 0, n = src.Length, line = 1;
        while (i < n)
        {
            char c = src[i];
            if (c == '\n')
            {
                if (toks.Count > 0 && toks[^1].Kind != "NL")
                    toks.Add(new Tok("NL", null, line));
                line++;
                i++;
                continue;
            }
            if (c is ' ' or '\t' or '\r')
            {
                while (i < n && src[i] is ' ' or '\t' or '\r') i++;
                continue;
            }
            if (c == '/' && i + 1 < n && src[i + 1] == '/')
            {
                int j = src.IndexOf('\n', i);
                i = j < 0 ? n : j;
                continue;
            }
            if (c == '"')
            {
                int j = i + 1;
                var buf = new StringBuilder();
                while (true)
                {
                    if (j >= n) throw new JassToLuaError($"unterminated string on line {line}");
                    char ch = src[j];
                    if (ch == '\\')
                    {
                        string nx = j + 1 < n ? src[j + 1].ToString() : "";
                        switch (nx)
                        {
                            case "n": buf.Append('\n'); break;
                            case "t": buf.Append('\t'); break;
                            case "r": buf.Append('\r'); break;
                            case "\"": buf.Append('"'); break;
                            case "\\": buf.Append('\\'); break;
                            case "'": buf.Append('\''); break;
                            case "b": buf.Append('\b'); break;
                            default: buf.Append('\\').Append(nx); break;
                        }
                        j += 2;
                        continue;
                    }
                    if (ch == '"') break;
                    if (ch == '\n') line++;
                    buf.Append(ch);
                    j++;
                }
                toks.Add(new Tok("STR", buf.ToString(), line));
                i = j + 1;
                continue;
            }
            if (c == '\'')
            {
                int k = i + 1;
                var buf = new StringBuilder();
                while (true)
                {
                    if (k >= n) throw new JassToLuaError($"unterminated fourcc on line {line}");
                    char ch = src[k];
                    if (ch == '\\')
                    {
                        if (k + 1 >= n) throw new JassToLuaError($"unterminated fourcc on line {line}");
                        buf.Append(src[k + 1]);
                        k += 2;
                        continue;
                    }
                    if (ch == '\'') break;
                    buf.Append(ch);
                    k++;
                }
                // One char per byte, so the chars are the bytes the Python encodes back.
                string s = buf.ToString();
                long val;
                if (s.Length == 4 && s.Any(ch => ch > 127))
                {
                    val = 0;
                    foreach (char ch in s)
                        val = (val * 256 + (ch >= 128 ? ch - 256 : ch)) & 0xFFFFFFFFL;
                    if (val >= 0x80000000L) val -= 0x100000000L;
                }
                else if (s.Length == 4)
                {
                    val = ((long)s[0] << 24) | ((long)s[1] << 16) | ((long)s[2] << 8) | s[3];
                    if (val >= 0x80000000L) val -= 0x100000000L;
                }
                else if (s.Length == 1)
                    val = s[0];
                else
                    throw new JassToLuaError($"invalid fourcc '{s}' on line {line}");
                toks.Add(new Tok("INT", val, line));
                i = k + 1;
                continue;
            }
            if (IsDigit(c) || (c == '.' && i + 1 < n && IsDigit(src[i + 1])))
            {
                if (c == '0' && i + 2 < n && src[i + 1] is 'x' or 'X' && IsHex(src[i + 2]))
                {
                    int j = i + 2;
                    while (j < n && IsHex(src[j])) j++;
                    var v = BigInteger.Parse("0" + src[(i + 2)..j], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    toks.Add(new Tok("INT", Wrap32(v), line));
                    i = j;
                    continue;
                }
                // (\d+\.\d*|\.\d+|\d+)
                int e = i;
                while (e < n && IsDigit(src[e])) e++;
                bool real = false;
                if (e < n && src[e] == '.' && (e > i || (e + 1 < n && IsDigit(src[e + 1]))))
                {
                    real = true;
                    e++;
                    while (e < n && IsDigit(src[e])) e++;
                }
                string t = src[i..e];
                i = e;
                if (real)
                    toks.Add(new Tok("REAL", t, line));
                else if (t.Length > 1 && t[0] == '0' && t.All(ch => ch is >= '0' and <= '7'))
                {
                    BigInteger v = 0;
                    foreach (char ch in t) v = v * 8 + (ch - '0');
                    toks.Add(new Tok("INT", Wrap32(v), line));
                }
                else
                    toks.Add(new Tok("INT", Wrap32(BigInteger.Parse(t, CultureInfo.InvariantCulture)), line));
                continue;
            }
            if (c == '$' && i + 1 < n && IsHex(src[i + 1]))
            {
                int j = i + 1;
                while (j < n && IsHex(src[j])) j++;
                var v = BigInteger.Parse("0" + src[(i + 1)..j], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                toks.Add(new Tok("INT", Wrap32(v), line));
                i = j;
                continue;
            }
            if (IsIdStart(c))
            {
                int j = i;
                while (j < n && IsIdChar(src[j])) j++;
                string w = src[i..j];
                i = j;
                toks.Add(new Tok(Keywords.Contains(w) ? "KW" : "ID", w, line));
                continue;
            }
            if (i + 1 < n)
            {
                string two = src.Substring(i, 2);
                if (two is "==" or "!=" or "<=" or ">=")
                {
                    toks.Add(new Tok("OP", two, line));
                    i += 2;
                    continue;
                }
            }
            if ("+-*/<>=(),[]".Contains(c))
            {
                toks.Add(new Tok("OP", c.ToString(), line));
                i++;
                continue;
            }
            // A UTF-8 byte order mark, which the Python sees as one char after decoding.
            if (c == 'ï' && i + 2 < n && src[i + 1] == '»' && src[i + 2] == '¿')
            {
                i += 3;
                continue;
            }
            if (c == '﻿')
            {
                i++;
                continue;
            }
            throw new JassToLuaError($"unexpected character '{c}' on line {line}");
        }
        toks.Add(new Tok("NL", null, line));
        toks.Add(new Tok("EOF", null, line));
        return toks;
    }

    // ---------------------------------------------------------------- parser

    internal sealed class Node(string k, object? a = null, object? b = null, object? c = null, int line = 0)
    {
        public readonly string K = k;
        public readonly object? A = a;
        public readonly object? B = b;
        public readonly object? C = c;
        public readonly int Line = line;
        public Node BN => (Node)B!;
        public Node CN => (Node)C!;
        public string AS => (string)A!;
    }

    internal sealed record LocalInfo(bool IsArray, Node? Init);
    internal sealed record Branch(Node Cond, List<Node> Body);

    internal sealed class Func(string name, List<(string Type, string Name)> parameters, string ret, int line)
    {
        public readonly string Name = name;
        public readonly List<(string Type, string Name)> Params = parameters;
        public readonly string Ret = ret;
        public readonly int Line = line;
        public List<Node> Body = new();
        public bool IsNative;
    }

    internal sealed record GlobalDecl(string Type, string Name, bool IsArray, bool IsConst, Node? Init, int Line);

    internal sealed class Parser(List<Tok> toks)
    {
        private int _i;
        public readonly List<GlobalDecl> Globals = new();
        public readonly Dictionary<string, Func> Natives = new(StringComparer.Ordinal);
        public readonly List<Func> Functions = new();
        public readonly Dictionary<string, string> Types = new(StringComparer.Ordinal);

        private Tok Peek(int off = 0) => toks[_i + off];
        private Tok Next() => toks[_i++];

        private static bool Is(Tok t, string kind, string? val) =>
            t.Kind == kind && (val is null || Equals(t.Val, val));

        private Tok? Accept(string kind, string? val = null)
        {
            var t = toks[_i];
            if (Is(t, kind, val)) { _i++; return t; }
            return null;
        }

        private Tok Expect(string kind, string? val = null)
        {
            var t = toks[_i];
            if (Is(t, kind, val)) { _i++; return t; }
            throw new JassToLuaError($"expected {kind} {val ?? "None"}, found {t} on line {t.Line}");
        }

        private void SkipNl()
        {
            while (toks[_i].Kind == "NL") _i++;
        }

        public Parser Parse()
        {
            while (true)
            {
                SkipNl();
                var t = Peek();
                if (t.Kind == "EOF") break;
                if (Is(t, "KW", "globals"))
                {
                    Next();
                    ParseGlobals();
                }
                else if (Is(t, "KW", "type"))
                {
                    Next();
                    string name = Expect("ID").S!;
                    Expect("KW", "extends");
                    var b = Next().Val;
                    Types[name] = b as string ?? Convert.ToString(b, CultureInfo.InvariantCulture) ?? "";
                    Expect("NL");
                }
                else if (t.Kind == "KW" && t.S is "native" or "constant" or "function")
                    ParseFunction();
                else
                    throw new JassToLuaError($"unexpected token at the top level: {t}");
            }
            return this;
        }

        private string ParseType()
        {
            var t = Next();
            if (t.Kind == "ID" || Is(t, "KW", "nothing")) return t.S!;
            throw new JassToLuaError($"type expected, found {t} on line {t.Line}");
        }

        private void ParseGlobals()
        {
            while (true)
            {
                SkipNl();
                var t = Peek();
                if (Is(t, "KW", "endglobals"))
                {
                    Next();
                    Expect("NL");
                    return;
                }
                bool isConst = Accept("KW", "constant") is not null;
                string type = ParseType();
                bool isArray = Accept("KW", "array") is not null;
                string name = Expect("ID").S!;
                Node? init = null;
                if (Accept("OP", "=") is not null) init = ParseExpr();
                Expect("NL");
                Globals.Add(new GlobalDecl(type, name, isArray, isConst, init, t.Line));
            }
        }

        private List<(string, string)> ParseParams()
        {
            var ps = new List<(string, string)>();
            if (Accept("KW", "nothing") is not null) return ps;
            while (true)
            {
                string type = ParseType();
                string name = Expect("ID").S!;
                ps.Add((type, name));
                if (Accept("OP", ",") is null) break;
            }
            return ps;
        }

        private void ParseFunction()
        {
            Accept("KW", "constant");
            var t = Next();
            if (Is(t, "KW", "native"))
            {
                string nname = Expect("ID").S!;
                Expect("KW", "takes");
                var nps = ParseParams();
                Expect("KW", "returns");
                string nret = ParseType();
                Expect("NL");
                Natives[nname] = new Func(nname, nps, nret, t.Line) { IsNative = true };
                return;
            }
            if (!Is(t, "KW", "function"))
                throw new JassToLuaError($"function expected on line {t.Line}");
            string name = Expect("ID").S!;
            Expect("KW", "takes");
            var ps = ParseParams();
            Expect("KW", "returns");
            string ret = ParseType();
            Expect("NL");
            var f = new Func(name, ps, ret, t.Line);
            f.Body = ParseBlock("endfunction");
            Expect("KW", "endfunction");
            Accept("NL");
            Functions.Add(f);
        }

        private List<Node> ParseBlock(params string[] terminators)
        {
            var stmts = new List<Node>();
            while (true)
            {
                SkipNl();
                var t = Peek();
                if (t.Kind == "KW" && terminators.Contains(t.S)) return stmts;
                if (t.Kind == "EOF")
                    throw new JassToLuaError($"unexpected end, expected ({string.Join(", ", terminators)})");
                stmts.Add(ParseStmt());
            }
        }

        private Node ParseStmt()
        {
            var t = Next();
            int line = t.Line;
            if (Is(t, "KW", "debug")) t = Next();
            if (t.Kind != "KW")
                throw new JassToLuaError($"statement expected, found {t} on line {t.Line}");
            string v = t.S!;
            switch (v)
            {
                case "local":
                {
                    string type = ParseType();
                    bool isArray = Accept("KW", "array") is not null;
                    string name = Expect("ID").S!;
                    Node? init = null;
                    if (Accept("OP", "=") is not null) init = ParseExpr();
                    Expect("NL");
                    return new Node("LOCAL", type, name, new LocalInfo(isArray, init), line);
                }
                case "set":
                {
                    string name = Expect("ID").S!;
                    Node? idx = null;
                    if (Accept("OP", "[") is not null)
                    {
                        idx = ParseExpr();
                        Expect("OP", "]");
                    }
                    Expect("OP", "=");
                    var e = ParseExpr();
                    Expect("NL");
                    return new Node("SET", name, idx, e, line);
                }
                case "call":
                {
                    var e = ParseExpr();
                    Expect("NL");
                    return new Node("CALLS", e, null, null, line);
                }
                case "if":
                {
                    var branches = new List<Branch>();
                    var cond = ParseExpr();
                    Expect("KW", "then");
                    Expect("NL");
                    branches.Add(new Branch(cond, ParseBlock("elseif", "else", "endif")));
                    List<Node>? els = null;
                    while (true)
                    {
                        var t2 = Next();
                        if (Equals(t2.Val, "elseif"))
                        {
                            cond = ParseExpr();
                            Expect("KW", "then");
                            Expect("NL");
                            branches.Add(new Branch(cond, ParseBlock("elseif", "else", "endif")));
                        }
                        else if (Equals(t2.Val, "else"))
                        {
                            Expect("NL");
                            els = ParseBlock("endif");
                            Expect("KW", "endif");
                            Expect("NL");
                            break;
                        }
                        else if (Equals(t2.Val, "endif"))
                        {
                            Expect("NL");
                            break;
                        }
                        else
                            throw new JassToLuaError($"malformed if on line {t2.Line}");
                    }
                    return new Node("IF", branches, els, null, line);
                }
                case "loop":
                {
                    Expect("NL");
                    var body = ParseBlock("endloop");
                    Expect("KW", "endloop");
                    Expect("NL");
                    return new Node("LOOP", body, null, null, line);
                }
                case "exitwhen":
                {
                    var e = ParseExpr();
                    Expect("NL");
                    return new Node("EXIT", e, null, null, line);
                }
                case "return":
                {
                    if (Peek().Kind == "NL")
                    {
                        Next();
                        return new Node("RET", null, null, null, line);
                    }
                    var e = ParseExpr();
                    Expect("NL");
                    return new Node("RET", e, null, null, line);
                }
            }
            throw new JassToLuaError($"unknown statement '{v}' on line {line}");
        }

        private Node ParseExpr() => ParseOr();

        private Node ParseOr()
        {
            var e = ParseAnd();
            while (Is(Peek(), "KW", "or"))
            {
                var t = Next();
                e = new Node("BIN", "or", e, ParseAnd(), t.Line);
            }
            return e;
        }

        private Node ParseAnd()
        {
            var e = ParseCmp();
            while (Is(Peek(), "KW", "and"))
            {
                var t = Next();
                e = new Node("BIN", "and", e, ParseCmp(), t.Line);
            }
            return e;
        }

        private Node ParseCmp()
        {
            var e = ParseAdd();
            while (Peek().Kind == "OP" && Peek().S is "==" or "!=" or "<" or "<=" or ">" or ">=")
            {
                var t = Next();
                e = new Node("BIN", t.S, e, ParseAdd(), t.Line);
            }
            return e;
        }

        private Node ParseAdd()
        {
            var e = ParseMul();
            while (Peek().Kind == "OP" && Peek().S is "+" or "-")
            {
                var t = Next();
                e = new Node("BIN", t.S, e, ParseMul(), t.Line);
            }
            return e;
        }

        private Node ParseMul()
        {
            var e = ParseUnary();
            while (Peek().Kind == "OP" && Peek().S is "*" or "/")
            {
                var t = Next();
                e = new Node("BIN", t.S, e, ParseUnary(), t.Line);
            }
            return e;
        }

        private Node ParseUnary()
        {
            var t = Peek();
            if (Is(t, "OP", "-"))
            {
                Next();
                return new Node("UN", "-", ParseUnary(), null, t.Line);
            }
            if (Is(t, "OP", "+"))
            {
                Next();
                return ParseUnary();
            }
            if (Is(t, "KW", "not"))
            {
                Next();
                return new Node("UN", "not", ParseUnary(), null, t.Line);
            }
            return ParsePrimary();
        }

        private Node ParsePrimary()
        {
            var t = Next();
            switch (t.Kind)
            {
                case "INT": return new Node("INT", t.Val, null, null, t.Line);
                case "REAL": return new Node("REAL", t.Val, null, null, t.Line);
                case "STR": return new Node("STR", t.Val, null, null, t.Line);
                case "KW":
                    switch (t.S)
                    {
                        case "true": return new Node("BOOL", true, null, null, t.Line);
                        case "false": return new Node("BOOL", false, null, null, t.Line);
                        case "null": return new Node("NULL", null, null, null, t.Line);
                        case "function": return new Node("FREF", Expect("ID").S, null, null, t.Line);
                    }
                    throw new JassToLuaError($"unexpected keyword '{t.S}' on line {t.Line}");
            }
            if (Is(t, "OP", "("))
            {
                var e = ParseExpr();
                Expect("OP", ")");
                return e;
            }
            if (t.Kind == "ID")
            {
                string name = t.S!;
                if (Accept("OP", "(") is not null)
                {
                    var args = new List<Node>();
                    if (Accept("OP", ")") is null)
                    {
                        while (true)
                        {
                            args.Add(ParseExpr());
                            if (Accept("OP", ")") is not null) break;
                            Expect("OP", ",");
                        }
                    }
                    return new Node("CALL", name, args, null, t.Line);
                }
                if (Accept("OP", "[") is not null)
                {
                    var idx = ParseExpr();
                    Expect("OP", "]");
                    return new Node("IDX", name, idx, null, t.Line);
                }
                return new Node("ID", name, null, null, t.Line);
            }
            throw new JassToLuaError($"invalid expression {t} on line {t.Line}");
        }
    }

    // ---------------------------------------------------------------- emitter

    /// <summary>A Lua string literal for a JASS string, one char per byte.</summary>
    /// <remarks>
    /// One deliberate difference from the Python. A control char is written as a decimal escape,
    /// and the Python writes it without padding, so a control char followed by a digit, chr(1)
    /// then "2", came out as \12 and Lua read one char 12. Padding to three digits when a digit
    /// follows keeps the two chars apart.
    /// </remarks>
    public static string LuaString(string s)
    {
        var sb = new StringBuilder(s.Length + 2);
        sb.Append('"');
        for (int i = 0; i < s.Length; i++)
        {
            char ch = s[i];
            int o = ch;
            if (ch == '"') sb.Append("\\\"");
            else if (ch == '\\') sb.Append("\\\\");
            else if (ch == '\n') sb.Append("\\n");
            else if (ch == '\r') sb.Append("\\r");
            else if (ch == '\t') sb.Append("\\t");
            else if (o < 32 || o == 127)
            {
                bool digitFollows = i + 1 < s.Length && IsDigit(s[i + 1]);
                sb.Append('\\').Append(o.ToString(digitFollows ? "D3" : "D", CultureInfo.InvariantCulture));
            }
            else sb.Append(ch);
        }
        sb.Append('"');
        return sb.ToString();
    }

    internal static string RealLiteral(string text)
    {
        if (text.StartsWith('.')) text = "0" + text;
        if (text.EndsWith('.')) text += "0";
        return text;
    }

    private sealed class Transpiler(
        string commonSrc, string blizzardSrc, string mapSrc,
        IReadOnlyDictionary<string, string>? overrides,
        IEnumerable<string>? dropFunctions,
        IReadOnlyDictionary<string, string>? translate,
        bool strictReals,
        bool hashtables)
    {
        private const int CatMax = 50;

        private readonly bool _strictReals = strictReals;
        private readonly bool _hashtables = hashtables;
        private HashSet<string> _rewritable = new(StringComparer.Ordinal);
        private int _hashtablesRewritten;
        private readonly IReadOnlyDictionary<string, string> _overrides = overrides ?? new Dictionary<string, string>();
        private readonly IReadOnlyDictionary<string, string> _translate = translate ?? new Dictionary<string, string>();
        private readonly HashSet<string> _drop = new(dropFunctions ?? [], StringComparer.Ordinal);
        private readonly Dictionary<string, (List<string> Params, string Ret)> _sigs = new(StringComparer.Ordinal);
        private readonly Dictionary<string, (string Type, bool IsArray)> _gtypes = new(StringComparer.Ordinal);
        private readonly List<string> _warnings = new();
        private readonly List<string> _luachunks = new();
        private readonly List<string> _exsLiterals = new();
        private readonly Dictionary<string, Func> _mapNatives = new(StringComparer.Ordinal);
        private readonly List<string> _funcNames = new();
        private Dictionary<string, (string Type, bool IsArray)> _scope = new(StringComparer.Ordinal);
        private HashSet<string> _spill = new(StringComparer.Ordinal);
        private Func? _curFunc;

        private static List<string> ParamTypes(Func f) => f.Params.Select(p => p.Type).ToList();

        private void LoadReference(string src)
        {
            var p = new Parser(Lex(src)).Parse();
            foreach (var g in p.Globals) _gtypes[g.Name] = (g.Type, g.IsArray);
            foreach (var (name, f) in p.Natives) _sigs[name] = (ParamTypes(f), f.Ret);
            foreach (var f in p.Functions) _sigs[f.Name] = (ParamTypes(f), f.Ret);
        }

        public JassToLuaResult Run()
        {
            LoadReference(commonSrc);
            LoadReference(blizzardSrc);
            var p = new Parser(Lex(mapSrc)).Parse();
            foreach (var (name, f) in p.Natives)
            {
                _sigs[name] = (ParamTypes(f), f.Ret);
                _mapNatives[name] = f;
            }
            foreach (var f in p.Functions) _sigs[f.Name] = (ParamTypes(f), f.Ret);
            foreach (var g in p.Globals) _gtypes[g.Name] = (g.Type, g.IsArray);

            var funcsOut = new List<string>();
            foreach (var f in p.Functions)
            {
                if (_drop.Contains(f.Name)) continue;
                funcsOut.Add(EmitFunction(f));
                _funcNames.Add(f.Name);
            }
            var names = new HashSet<string>(_funcNames, StringComparer.Ordinal);
            var missing = _overrides.Keys.Where(k => !names.Contains(k)).OrderBy(k => k, StringComparer.Ordinal).ToList();
            if (missing.Count > 0)
                Warn("OVERRIDE without a matching function in the map: " + string.Join(", ", missing));
            string globalsOut = EmitGlobals(p.Globals);
            var handleGlobals = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var g in p.Globals)
                if (!Primitives.Contains(g.Type) && !g.IsArray)
                    handleGlobals[g.Name] = g.Type;
            return new JassToLuaResult(
                string.Join("\n", funcsOut), globalsOut, _luachunks, _exsLiterals, handleGlobals,
                _funcNames, missing, _warnings,
                _mapNatives.ToDictionary(kv => kv.Key,
                    kv => ((IReadOnlyList<string>)ParamTypes(kv.Value), kv.Value.Ret), StringComparer.Ordinal),
                _strictReals, _hashtablesRewritten);
        }

        private (string Type, bool IsArray) VarType(string name)
        {
            if (_scope.TryGetValue(name, out var t)) return t;
            if (_gtypes.TryGetValue(name, out var g)) return g;
            return ("unknown", false);
        }

        private string EType(Node e)
        {
            switch (e.K)
            {
                case "INT": return "integer";
                case "REAL": return "real";
                case "STR": return "string";
                case "BOOL": return "boolean";
                case "NULL": return "null";
                case "ID":
                {
                    var t = VarType(e.AS).Type;
                    if (t == "unknown") Warn($"unknown type for {e.AS} (line {e.Line})");
                    return t;
                }
                case "IDX":
                {
                    var t = VarType(e.AS).Type;
                    if (t == "unknown") Warn($"unknown type for array {e.AS} (line {e.Line})");
                    return t;
                }
                case "CALL":
                    if (!_sigs.TryGetValue(e.AS, out var s))
                    {
                        Warn($"unknown function {e.AS} (line {e.Line})");
                        return "unknown";
                    }
                    return s.Ret;
                case "FREF": return "code";
                case "UN":
                    return e.AS == "not" ? "boolean" : EType(e.BN);
                case "BIN":
                {
                    string op = e.AS;
                    if (op is "==" or "!=" or "<" or "<=" or ">" or ">=" or "and" or "or") return "boolean";
                    string ta = EType(e.BN), tb = EType(e.CN);
                    if (op == "+" && (ta == "string" || tb == "string")) return "string";
                    if (ta == "real" || tb == "real") return "real";
                    if (ta == "integer" && tb == "integer") return "integer";
                    if (ta == "unknown" || tb == "unknown") return "unknown";
                    return "real";
                }
            }
            return "unknown";
        }

        private void Warn(string msg)
        {
            if (_warnings.Count < 500) _warnings.Add(msg);
        }

        private string Name(string n)
        {
            string b = LuaKeywords.Contains(n) ? n + "_" : n;
            return _spill.Contains(n) ? "__L." + b : b;
        }

        private void FlattenCat(Node e, List<Node> output)
        {
            if (e.K == "BIN" && e.AS == "+" && IsStringAdd(e))
            {
                FlattenCat(e.BN, output);
                FlattenCat(e.CN, output);
            }
            else
                output.Add(e);
        }

        private bool IsStringAdd(Node e)
        {
            string ta = EType(e.BN), tb = EType(e.CN);
            return ta == "string" || tb == "string" || (ta == "null" && tb == "null");
        }

        private string TranslateLiteral(string txt) =>
            _translate.Count > 0 && _translate.TryGetValue(txt, out var t) ? t : txt;

        private string Expr(Node e)
        {
            switch (e.K)
            {
                case "INT":
                {
                    long v = (long)e.A!;
                    return v >= 0 ? v.ToString(CultureInfo.InvariantCulture) : $"({v.ToString(CultureInfo.InvariantCulture)})";
                }
                case "REAL": return RealLiteral(e.AS);
                case "STR": return LuaString(TranslateLiteral(e.AS));
                case "BOOL": return (bool)e.A! ? "true" : "false";
                case "NULL": return "nil";
                case "ID": return Name(e.AS);
                case "IDX": return $"{Name(e.AS)}[{Expr(e.BN)}]";
                case "FREF": return Name(e.AS);
                case "CALL": return CallExpr(e);
                case "UN":
                    return e.AS == "not" ? $"(not {Expr(e.BN)})" : $"(-{Expr(e.BN)})";
                case "BIN":
                    return BinExpr(e);
            }
            throw new JassToLuaError($"expression node {e.K}");
        }

        private string BinExpr(Node e)
        {
            string op = e.AS;
            switch (op)
            {
                case "+":
                {
                    if (IsStringAdd(e))
                    {
                        var parts = new List<Node>();
                        FlattenCat(e, parts);
                        return EmitCat(parts);
                    }
                    string ta = EType(e.BN), tb = EType(e.CN);
                    if (ta == "integer" && tb == "integer" && !(SmallLit(e.BN) || SmallLit(e.CN)))
                        return Two("__iadd({0}, {1})", e);
                    if (_strictReals && RealOperand(ta, tb)) return Two("__fadd({0}, {1})", e);
                    return Two("({0} + {1})", e);
                }
                case "-":
                {
                    string ta = EType(e.BN), tb = EType(e.CN);
                    if (ta == "integer" && tb == "integer" && !(SmallLit(e.BN) || SmallLit(e.CN)))
                        return Two("__isub({0}, {1})", e);
                    if (_strictReals && RealOperand(ta, tb)) return Two("__fsub({0}, {1})", e);
                    return Two("({0} - {1})", e);
                }
                case "*":
                {
                    string ta = EType(e.BN), tb = EType(e.CN);
                    if (ta == "integer" && tb == "integer") return Two("__imul({0}, {1})", e);
                    if (_strictReals && RealOperand(ta, tb)) return Two("__fmul({0}, {1})", e);
                    return Two("({0} * {1})", e);
                }
                case "/":
                {
                    string ta = EType(e.BN), tb = EType(e.CN);
                    if (ta == "integer" && tb == "integer") return Two("__idiv({0}, {1})", e);
                    // Before the nonzero-literal shortcut, because that shortcut skips the rounding
                    // a strict-real conversion needs on the result.
                    if (_strictReals && RealOperand(ta, tb)) return Two("__fdiv({0}, {1})", e);
                    if (e.CN.K is "INT" or "REAL" && NumberValue(e.CN) != 0.0) return Two("({0} / {1})", e);
                    return Two("__div({0}, {1})", e);
                }
                case "==" or "!=":
                {
                    string ta = EType(e.BN), tb = EType(e.CN);
                    string left = e.BN.K == "NULL" ? NullFor(tb) : Expr(e.BN);
                    string right = e.CN.K == "NULL" ? NullFor(ta) : Expr(e.CN);
                    return $"({left} {(op == "==" ? "==" : "~=")} {right})";
                }
                case "<" or "<=" or ">" or ">=":
                {
                    string l = Expr(e.BN);
                    return $"({l} {op} {Expr(e.CN)})";
                }
                case "and" or "or":
                {
                    string l = Expr(e.BN);
                    return $"({l} {op} {Expr(e.CN)})";
                }
            }
            throw new JassToLuaError($"operator '{op}'");
        }

        private static double NumberValue(Node n) =>
            n.K == "INT" ? (long)n.A! : double.Parse(n.AS, NumberStyles.Float, CultureInfo.InvariantCulture);

        // Left before right, the order the Python's % tuple evaluates them, which fixes the order
        // of any warnings the two sides raise.
        private string Two(string format, Node e)
        {
            string l = Expr(e.BN);
            string r = Expr(e.CN);
            return string.Format(CultureInfo.InvariantCulture, format, l, r);
        }

        private string EmitCat(List<Node> parts)
        {
            if (parts.Count <= CatMax)
                return "__cat(" + string.Join(", ", parts.Select(Expr)) + ")";
            var chunks = new List<string>();
            for (int i = 0; i < parts.Count; i += CatMax)
                chunks.Add("__cat(" + string.Join(", ", parts.Skip(i).Take(CatMax).Select(Expr)) + ")");
            return "__cat(" + string.Join(", ", chunks) + ")";
        }

        private static bool SmallLit(Node e) => e.K == "INT" && (long)e.A! is > -1048576 and < 1048576;

        /// <summary>Whether the operands make this a real (floating point) operation. JASS promotes
        /// an integer operand to real, so one real operand is enough. Only consulted under
        /// strict reals, where the result is rounded back to the 32-bit float the JASS runtime
        /// would have produced.</summary>
        private static bool RealOperand(string ta, string tb) => ta == "real" || tb == "real";

        private static string NullFor(string type) => type switch
        {
            "integer" => "0",
            "real" => "0.0",
            "boolean" => "false",
            _ => "nil",
        };

        private string ExprAs(Node e, string type) => e.K == "NULL" ? NullFor(type) : Expr(e);

        private string CallExpr(Node e)
        {
            string name = e.AS;
            var args = (List<Node>)e.B!;
            if (_hashtables && args.Count > 0 && args[0].K == "ID" && _rewritable.Contains(args[0].AS)
                && RewriteHashtableCall(name, args) is { } rewritten)
                return rewritten;
            if (name == "EXExecuteScript" && args.Count == 1 && args[0].K == "STR")
            {
                int idx = _exsLiterals.Count;
                _exsLiterals.Add(args[0].AS);
                return $"__exs({idx})";
            }
            bool hasSig = _sigs.TryGetValue(name, out var sig);
            var parts = new List<string>();
            for (int i = 0; i < args.Count; i++)
            {
                var a = args[i];
                if (a.K == "NULL" && hasSig && i < sig.Params.Count) parts.Add(NullFor(sig.Params[i]));
                else parts.Add(Expr(a));
            }
            return $"{Name(name)}({string.Join(", ", parts)})";
        }

        private static string? DefaultFor(string type) => type switch
        {
            "integer" => "0",
            "real" => "0.0",
            "boolean" => "false",
            _ => null,
        };

        private static string ArrayCtor(string type) => type switch
        {
            "integer" => "__arr_i()",
            "real" => "__arr_r()",
            "boolean" => "__arr_b()",
            _ => "{}",
        };

        /// <summary>The type a hashtable native stores or loads, read from its suffix, or null when
        /// the native is not one of the family this rewrite understands.</summary>
        private static string? HashtableType(string native)
        {
            string? suffix =
                native.StartsWith("Save", StringComparison.Ordinal) ? native[4..] :
                native.StartsWith("Load", StringComparison.Ordinal) ? native[4..] :
                native.StartsWith("HaveSaved", StringComparison.Ordinal) ? native[9..] :
                native.StartsWith("RemoveSaved", StringComparison.Ordinal) ? native[11..] :
                null;
            return suffix is not null && HashtableStore.TryGetValue(suffix, out var t) ? t : null;
        }

        private static bool IsHashtableNative(string native) =>
            native is "FlushChildHashtable" or "FlushParentHashtable" || HashtableType(native) is not null;

        /// <summary>The helper call a hashtable native becomes when its handle is a rewritten table,
        /// or null when the call must stay a native.</summary>
        private string? RewriteHashtableCall(string name, List<Node> args)
        {
            if (name == "FlushChildHashtable" && args.Count == 2)
                return $"__hs_flush_child({Expr(args[0])}, {Expr(args[1])})";
            if (name == "FlushParentHashtable" && args.Count == 1)
                return $"__hs_flush({Expr(args[0])})";

            string? type = HashtableType(name);
            if (type is null) return null;
            string suffix = HashtableSuffix(type);
            if (name.StartsWith("Save", StringComparison.Ordinal) && args.Count == 4)
                return $"__hs_save_{suffix}({Expr(args[0])}, {Expr(args[1])}, {Expr(args[2])}, {ExprAs(args[3], type)})";
            if (name.StartsWith("Load", StringComparison.Ordinal) && args.Count == 3)
                return $"__hs_load_{suffix}({Expr(args[0])}, {Expr(args[1])}, {Expr(args[2])}, {DefaultFor(type) ?? "nil"})";
            if (name.StartsWith("HaveSaved", StringComparison.Ordinal) && args.Count == 3)
                return $"__hs_has_{suffix}({Expr(args[0])}, {Expr(args[1])}, {Expr(args[2])})";
            if (name.StartsWith("RemoveSaved", StringComparison.Ordinal) && args.Count == 3)
                return $"__hs_remove_{suffix}({Expr(args[0])}, {Expr(args[1])}, {Expr(args[2])})";
            return null;
        }

        /// <summary>The local hashtables in this function that can become a Lua table. A local
        /// qualifies when it is declared hashtable, initialised from CreateHashtable, and every
        /// reference to its name is the first argument of a hashtable native. Anything else, a pass
        /// to another function, a comparison, a reassignment, leaves it a real hashtable, because a
        /// native still has to receive one.</summary>
        private HashSet<string> RewritableHashtables(List<Node> body)
        {
            var names = new HashSet<string>(StringComparer.Ordinal);
            foreach (var s in body)
            {
                if (s.K != "LOCAL" || s.AS != "hashtable" || ((LocalInfo)s.C!).IsArray) continue;
                if (((LocalInfo)s.C!).Init is { K: "CALL", AS: "CreateHashtable" }) names.Add((string)s.B!);
            }
            if (names.Count == 0) return names;

            var safe = new HashSet<string>(names, StringComparer.Ordinal);
            foreach (var s in body) ScanHashtableUses(s, names, safe);
            return safe;
        }

        /// <summary>Marks a name unsafe when it is used anywhere but as a hashtable native's first
        /// argument. An unrecognized node shape marks every name under it unsafe rather than being
        /// assumed harmless, which is what keeps this conservative.</summary>
        private void ScanHashtableUses(Node node, HashSet<string> names, HashSet<string> safe)
        {
            switch (node.K)
            {
                case "CALL":
                {
                    var args = (List<Node>)node.B!;
                    bool native = IsHashtableNative(node.AS);
                    for (int i = 0; i < args.Count; i++)
                    {
                        if (i == 0 && native && args[i].K == "ID") continue; // the handle position
                        ScanHashtableUses(args[i], names, safe);
                    }
                    return;
                }
                case "ID":
                    if (names.Contains(node.AS)) safe.Remove(node.AS);
                    return;
                case "LOCAL":
                    return; // the declaration itself, not a use
                case "SET":
                    if (names.Contains(node.AS)) safe.Remove(node.AS);
                    if (node.B is Node idx) ScanHashtableUses(idx, names, safe);
                    if (node.CN is Node value) ScanHashtableUses(value, names, safe);
                    return;
                default:
                    if (node.A is Node an) ScanHashtableUses(an, names, safe);
                    else if (node.A is List<Node> ln) foreach (var x in ln) ScanHashtableUses(x, names, safe);
                    else if (node.A is List<Branch> lb)
                        foreach (var b in lb)
                        {
                            ScanHashtableUses(b.Cond, names, safe);
                            foreach (var x in b.Body) ScanHashtableUses(x, names, safe);
                        }
                    if (node.B is Node bn) ScanHashtableUses(bn, names, safe);
                    else if (node.B is List<Node> ln2) foreach (var x in ln2) ScanHashtableUses(x, names, safe);
                    if (node.C is Node cn) ScanHashtableUses(cn, names, safe);
                    else if (node.C is List<Node> ln3) foreach (var x in ln3) ScanHashtableUses(x, names, safe);
                    return;
            }
        }

        // The spill set is deliberately left as the last function emitted it, as the Python does.
        private string EmitGlobals(List<GlobalDecl> globs)
        {
            _scope = new(StringComparer.Ordinal);
            var output = new List<string>();
            foreach (var g in globs)
            {
                string n = Name(g.Name);
                if (g.IsArray)
                {
                    output.Add($"{n} = {ArrayCtor(g.Type)}");
                    continue;
                }
                if (g.Init is not null)
                {
                    output.Add($"{n} = {ExprAs(g.Init, g.Type)}");
                    continue;
                }
                if (DefaultFor(g.Type) is { } d) output.Add($"{n} = {d}");
            }
            return string.Join("\n", output);
        }

        private string EmitFunction(Func f)
        {
            _scope = new(StringComparer.Ordinal);
            _curFunc = f;
            _spill = new(StringComparer.Ordinal);
            _rewritable = _hashtables ? RewritableHashtables(f.Body) : new(StringComparer.Ordinal);
            foreach (var (t, n) in f.Params) _scope[n] = (t, false);
            string ps = string.Join(", ", f.Params.Select(p => Name(p.Name)));
            string head = $"function {Name(f.Name)}({ps})";
            if (_overrides.TryGetValue(f.Name, out var body))
                return $"-- @jass {f.Line} (override)\n{head}\n{body}\nend";
            var lines = new List<string> { $"-- @jass {f.Line}", head };
            var localVars = f.Body.Where(s => s.K == "LOCAL").Select(s => (string)s.B!).ToList();
            if (f.Params.Count + localVars.Count > LocalsInLua)
            {
                _spill = new(localVars.Skip(Math.Max(0, LocalsInLua - f.Params.Count)), StringComparer.Ordinal);
                lines.Add("  local __L = {}");
            }
            EmitBlock(f.Body, lines, 1);
            lines.Add("end");
            return string.Join("\n", lines);
        }

        private void EmitBlock(List<Node> stmtsIn, List<string> lines, int depth)
        {
            string ind = new(' ', 2 * depth);
            var stmts = FoldLuachunks(stmtsIn);
            int n = stmts.Count;
            for (int i = 0; i < n; i++)
            {
                var s = stmts[i];
                bool last = i == n - 1;
                switch (s.K)
                {
                    case "LOCAL":
                    {
                        string type = s.AS, name = (string)s.B!;
                        var info = (LocalInfo)s.C!;
                        _scope[name] = (type, info.IsArray);
                        string ln = Name(name);
                        string decl = _spill.Contains(name) ? "" : "local ";
                        if (info.IsArray)
                            lines.Add($"{ind}{decl}{ln} = {ArrayCtor(type)}");
                        else if (info.Init is not null)
                        {
                            // A rewritable hashtable is a plain Lua table from the start, so the
                            // CreateHashtable call goes away with it.
                            bool table = _hashtables && type == "hashtable" && _rewritable.Contains(name);
                            if (table) _hashtablesRewritten++;
                            lines.Add($"{ind}{decl}{ln} = {(table ? "{}" : ExprAs(info.Init, type))}");
                        }
                        else if (DefaultFor(type) is { } d)
                            lines.Add($"{ind}{decl}{ln} = {d}");
                        else if (decl.Length > 0)
                            lines.Add($"{ind}local {ln}");
                        break;
                    }
                    case "SET":
                    {
                        string vtype = VarType(s.AS).Type;
                        if (s.B is null)
                            lines.Add($"{ind}{Name(s.AS)} = {ExprAs(s.CN, vtype)}");
                        else
                        {
                            string target = Name(s.AS);
                            string idx = Expr(s.BN);
                            lines.Add($"{ind}{target}[{idx}] = {ExprAs(s.CN, vtype)}");
                        }
                        break;
                    }
                    case "CALLS":
                    {
                        var e = (Node)s.A!;
                        if (e.K != "CALL") throw new JassToLuaError($"call without a function call on line {s.Line}");
                        lines.Add(ind + CallExpr(e));
                        break;
                    }
                    case "LUACHUNK":
                        lines.Add($"{ind}__luachunk({(int)s.A!})");
                        break;
                    case "IF":
                    {
                        bool first = true;
                        foreach (var br in (List<Branch>)s.A!)
                        {
                            string kw = first ? "if" : "elseif";
                            first = false;
                            lines.Add($"{ind}{kw} {Expr(br.Cond)} then");
                            EmitBlock(br.Body, lines, depth + 1);
                        }
                        if (s.B is List<Node> els)
                        {
                            lines.Add(ind + "else");
                            EmitBlock(els, lines, depth + 1);
                        }
                        lines.Add(ind + "end");
                        break;
                    }
                    case "LOOP":
                        lines.Add(ind + "while true do");
                        EmitBlock((List<Node>)s.A!, lines, depth + 1);
                        lines.Add(ind + "end");
                        break;
                    case "EXIT":
                        lines.Add($"{ind}if {Expr((Node)s.A!)} then break end");
                        break;
                    case "RET":
                    {
                        string r = s.A is null ? "return" : "return " + ExprAs((Node)s.A, _curFunc!.Ret);
                        lines.Add(last ? ind + r : $"{ind}do {r} end");
                        break;
                    }
                    default:
                        throw new JassToLuaError($"unknown statement {s.K}");
                }
            }
        }

        private static readonly (string Pre, string Suf)[] LuaLoadForms =
        {
            ("lua_load([=[", "]=]"),
            ("lua_load(string.sub([====[!", "!]====]"),
        };

        private static (string Var, bool End)? LuaLoadParts(Node e)
        {
            if (e.K != "CALL" || e.AS != "EXExecuteScript") return null;
            var args = (List<Node>)e.B!;
            if (args.Count != 1) return null;
            var parts = new List<Node>();
            void Flat(Node x)
            {
                if (x.K == "BIN" && x.AS == "+")
                {
                    Flat(x.BN);
                    Flat(x.CN);
                }
                else parts.Add(x);
            }
            Flat(args[0]);
            if (parts.Count != 3) return null;
            var (a, b, c) = (parts[0], parts[1], parts[2]);
            if (a.K != "STR" || b.K != "ID" || c.K != "STR") return null;
            foreach (var (pre, suf) in LuaLoadForms)
                if (a.AS.StartsWith(pre, StringComparison.Ordinal) && c.AS.StartsWith(suf, StringComparison.Ordinal))
                    return (b.AS, c.AS.Contains("true", StringComparison.Ordinal));
            return null;
        }

        private List<Node> FoldLuachunks(List<Node> stmts)
        {
            var output = new List<Node>();
            var pending = new Dictionary<string, string>(StringComparer.Ordinal);
            var pendingOrder = new List<string>();
            void Add(string var, string text)
            {
                if (pending.TryGetValue(var, out var had)) pending[var] = had + text;
                else
                {
                    pending[var] = text;
                    pendingOrder.Add(var);
                }
            }
            void Close(string var, int line)
            {
                int idx = _luachunks.Count;
                _luachunks.Add(pending[var]);
                pending.Remove(var);
                pendingOrder.Remove(var);
                output.Add(new Node("LUACHUNK", idx, null, null, line));
            }
            int i = 0, n = stmts.Count;
            while (i < n)
            {
                var s = stmts[i];
                var nxt = i + 1 < n ? stmts[i + 1] : null;
                if (s.K == "SET" && s.B is null && s.CN.K == "STR")
                {
                    string var = s.AS;
                    var lp = nxt is not null && nxt.K == "CALLS" ? LuaLoadParts((Node)nxt.A!) : null;
                    if (lp is { } p && p.Var == var)
                    {
                        Add(var, TranslateLiteral(s.CN.AS));
                        if (p.End) Close(var, s.Line);
                        i += 2;
                        continue;
                    }
                }
                if (s.K == "LOCAL" && s.C is LocalInfo { Init: { K: "STR" } init })
                {
                    string var = (string)s.B!;
                    var lp = nxt is not null && nxt.K == "CALLS" ? LuaLoadParts((Node)nxt.A!) : null;
                    if (lp is { } p && p.Var == var)
                    {
                        output.Add(new Node("LOCAL", s.A, s.B, new LocalInfo(false, null), s.Line));
                        Add(var, TranslateLiteral(init.AS));
                        if (p.End) Close(var, s.Line);
                        i += 2;
                        continue;
                    }
                }
                output.Add(s);
                i++;
            }
            foreach (var var in pendingOrder.ToList())
            {
                Warn($"lua_load without an end for {var} in function {_curFunc!.Name}");
                int idx = _luachunks.Count;
                _luachunks.Add(pending[var]);
                output.Add(new Node("LUACHUNK", idx, null, null, 0));
            }
            return output;
        }
    }
}
