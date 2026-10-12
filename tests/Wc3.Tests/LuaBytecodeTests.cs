// tests/Wc3.Tests/LuaBytecodeTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Commands.Lua;
using Wc3.Model;
using static Wc3.Commands.Lua.Lua53Translate;

namespace Wc3.Tests;

/// <summary>
/// The Lua 5.3 chunk reader and the translator back to source, ported from lua_bytecode.py and
/// lua53_translate.py in Devo's Map Doctor. Chunks are assembled here byte by byte so each test
/// controls exactly one instruction, and one real chunk from luac 5.3.6 is checked against the
/// doctor's own translation of it.
/// </summary>
public class LuaBytecodeTests
{
    // ---------------------------------------------------------------- a minimal luac writer

    private static uint ABC(int op, int a, int b, int c) => (uint)(op | a << 6 | c << 14 | b << 23);
    private static uint ABx(int op, int a, int bx) => (uint)(op | a << 6 | bx << 14);
    private static uint AsBx(int op, int a, int sbx) => ABx(op, a, sbx + 131071);
    private static int Kst(int k) => k | 0x100;

    private sealed class Fn
    {
        public int Params, Vararg, Stack = 8;
        public List<uint> Code = new();
        public List<object?> Consts = new();
        public List<(int, int)> Upvals = new();
        public List<Fn> Protos = new();
    }

    private static void Int(List<byte> o, int v) => o.AddRange(BitConverter.GetBytes(v));

    private static void Str(List<byte> o, byte[]? s)
    {
        if (s is null) { o.Add(0); return; }
        if (s.Length + 1 < 0xFF) o.Add((byte)(s.Length + 1));
        else
        {
            o.Add(0xFF);
            o.AddRange(BitConverter.GetBytes((long)s.Length + 1));
        }
        o.AddRange(s);
    }

    private static void Write(List<byte> o, Fn f, bool main)
    {
        Str(o, main ? Encoding.ASCII.GetBytes("@test.lua") : null);
        Int(o, 0);
        Int(o, 0);
        o.Add((byte)f.Params);
        o.Add((byte)f.Vararg);
        o.Add((byte)f.Stack);
        Int(o, f.Code.Count);
        foreach (var i in f.Code) o.AddRange(BitConverter.GetBytes(i));
        Int(o, f.Consts.Count);
        foreach (var c in f.Consts)
        {
            switch (c)
            {
                case null: o.Add(0); break;
                case bool b: o.Add(1); o.Add((byte)(b ? 1 : 0)); break;
                case double d: o.Add(3); o.AddRange(BitConverter.GetBytes(d)); break;
                case long n: o.Add(0x13); o.AddRange(BitConverter.GetBytes(n)); break;
                case string s:
                    var bytes = Encoding.Latin1.GetBytes(s);
                    o.Add(bytes.Length > 40 ? (byte)0x14 : (byte)4);
                    Str(o, bytes);
                    break;
            }
        }
        Int(o, f.Upvals.Count);
        foreach (var (instack, idx) in f.Upvals) { o.Add((byte)instack); o.Add((byte)idx); }
        Int(o, f.Protos.Count);
        foreach (var p in f.Protos) Write(o, p, false);
        Int(o, 0); // lineinfo
        Int(o, 0); // locvars
        Int(o, 0); // upnames
    }

    private static byte[] Chunk(Fn main)
    {
        var o = new List<byte> { 0x1B, (byte)'L', (byte)'u', (byte)'a', 0x53, 0 };
        o.AddRange(new byte[] { 0x19, 0x93, 0x0D, 0x0A, 0x1A, 0x0A });
        o.AddRange(new byte[] { 4, 8, 4, 8, 8 });
        o.AddRange(BitConverter.GetBytes(0x5678L));
        o.AddRange(BitConverter.GetBytes(370.5));
        o.Add((byte)main.Upvals.Count);
        Write(o, main, true);
        return o.ToArray();
    }

    /// <summary>A main function with _ENV as its upvalue, running <paramref name="code"/> then returning.</summary>
    private static Fn Main(object?[] consts, params uint[] code)
    {
        var f = new Fn { Vararg = 1, Consts = consts.ToList(), Upvals = { (1, 0) } };
        f.Code.AddRange(code);
        f.Code.Add(ABC(RETURN, 0, 1, 0));
        return f;
    }

    private static string Body(Fn main) => Translate(Chunk(main), "t");

    // ---------------------------------------------------------------- the reader

    [Fact]
    public void The_reader_takes_the_header_and_every_constant_type()
    {
        string longText = new('x', 300);
        var (main, whole) = LuaBytecode.Load(Chunk(Main([null, true, false, 5L, -2.5, "hi", longText])));
        Assert.True(whole);
        Assert.Equal(Encoding.ASCII.GetBytes("@test.lua"), main.Source);
        Assert.Equal(1, main.Vararg);
        Assert.Null(main.Consts[0]);
        Assert.Equal(true, main.Consts[1]);
        Assert.Equal(false, main.Consts[2]);
        Assert.Equal(5L, main.Consts[3]);
        Assert.Equal(-2.5, main.Consts[4]);
        Assert.Equal("hi"u8.ToArray(), (byte[])main.Consts[5]!);
        Assert.Equal(300, ((byte[])main.Consts[6]!).Length);
    }

    [Fact]
    public void The_reader_takes_upvalues_and_nested_functions_and_spots_trailing_bytes()
    {
        var inner = new Fn { Upvals = { (1, 2), (0, 0) }, Code = { ABC(RETURN, 0, 1, 0) } };
        var middle = new Fn { Protos = { inner }, Code = { ABC(RETURN, 0, 1, 0) } };
        var main = Main([], ABx(CLOSURE, 0, 0));
        main.Protos.Add(middle);
        var bytes = Chunk(main);
        var (m, whole) = LuaBytecode.Load(bytes);
        Assert.True(whole);
        Assert.Equal(3, m.Walk().Count());
        Assert.Equal([(1, 2), (0, 0)], m.Protos[0].Protos[0].Upvals);
        Assert.Equal(Encoding.ASCII.GetBytes("@test.lua"), m.Protos[0].Protos[0].Source);

        Assert.False(LuaBytecode.Load([.. bytes, 0]).Whole);
        Assert.Throws<LuaTranslateError>(() => Translate([.. bytes, 0]));
        Assert.Throws<LuaBytecodeError>(() => LuaBytecode.Load(bytes[..^5]));
        Assert.Throws<LuaBytecodeError>(() => LuaBytecode.Load("print(1)"u8.ToArray()));
    }

    // ---------------------------------------------------------------- opcode families

    [Fact]
    public void Loads_and_moves()
    {
        var s = Body(Main([7L],
            ABx(LOADK, 0, 0), ABC(MOVE, 1, 0, 0), ABC(LOADBOOL, 2, 1, 1), ABC(LOADNIL, 3, 1, 0)));
        Assert.Contains("\nr0 = 7\n", s);
        Assert.Contains("\nr1 = r0\n", s);
        Assert.Contains("\nr2 = true goto L4\n", s);
        Assert.Contains("\nr3 = nil r4 = nil\n::L4::\n", s);
    }

    [Fact]
    public void Tables_upvalues_and_self()
    {
        var s = Body(Main(["print", "x"],
            ABC(GETTABUP, 0, 0, Kst(0)), ABC(SETTABUP, 0, Kst(1), 0), ABC(NEWTABLE, 1, 0, 0),
            ABC(GETTABLE, 2, 1, Kst(1)), ABC(SETTABLE, 1, Kst(1), 2), ABC(SELF, 3, 1, Kst(0)),
            ABC(GETUPVAL, 5, 0, 0), ABC(SETUPVAL, 5, 0, 0)));
        Assert.Contains("\nr0 = U[1][1][\"print\"]\n", s);
        Assert.Contains("\nU[1][1][\"x\"] = r0\n", s);
        Assert.Contains("\nr1 = {}\n", s);
        Assert.Contains("\nr2 = r1[\"x\"]\n", s);
        Assert.Contains("\nr1[\"x\"] = r2\n", s);
        Assert.Contains("\nr4 = r1 r3 = r4[\"print\"]\n", s);
        Assert.Contains("\nr5 = U[1][1]\n", s);
        Assert.Contains("\nU[1][1] = r5\n", s);
    }

    [Fact]
    public void Arithmetic_unary_and_concat()
    {
        var s = Body(Main([2L],
            ABC(ADD, 0, 1, Kst(0)), ABC(IDIV, 0, 1, 2), ABC(SHR, 0, 1, 2), ABC(POW, 0, 1, 2),
            ABC(UNM, 0, 1, 0), ABC(BNOT, 0, 1, 0), ABC(NOT, 0, 1, 0), ABC(LEN, 0, 1, 0), ABC(CONCAT, 0, 1, 3)));
        Assert.Contains("\nr0 = r1 + 2\n", s);
        Assert.Contains("\nr0 = r1 // r2\n", s);
        Assert.Contains("\nr0 = r1 >> r2\n", s);
        Assert.Contains("\nr0 = r1 ^ r2\n", s);
        Assert.Contains("\nr0 = -r1\n", s);
        Assert.Contains("\nr0 = ~r1\n", s);
        Assert.Contains("\nr0 = not r1\n", s);
        Assert.Contains("\nr0 = #r1\n", s);
        Assert.Contains("\nr0 = r1 .. r2 .. r3\n", s);
    }

    [Fact]
    public void Jumps_comparisons_and_tests()
    {
        var s = Body(Main([1L],
            ABC(EQ, 1, 0, Kst(0)), AsBx(JMP, 0, 1), ABC(LT, 0, 0, 1), AsBx(JMP, 0, 0),
            ABC(TEST, 0, 0, 1), ABC(TESTSET, 1, 0, 0), AsBx(JMP, 1, -3)));
        Assert.Contains("\nif not (r0 == 1) then goto L2 end\n", s);
        Assert.Contains("\ngoto L3\n", s);
        Assert.Contains("\nif r0 < r1 then goto L4 end\n", s);
        Assert.Contains("\nif not r0 then goto L6 end\n", s);
        Assert.Contains("\nif not r0 then r1 = r0 else goto L7 end\n", s);
        Assert.Contains("\ngoto L4\n", s);
    }

    [Fact]
    public void Calls_returns_and_varargs()
    {
        var callee = new Fn { Vararg = 1, Code = { ABC(VARARG, 0, 0, 0), ABC(RETURN, 0, 0, 0) } };
        var main = Main([],
            ABC(CALL, 0, 3, 2), ABC(VARARG, 2, 0, 0), ABC(CALL, 1, 0, 1), ABC(CALL, 0, 1, 0), ABC(CALL, 3, 0, 1),
            ABC(TAILCALL, 0, 2, 0), ABC(RETURN, 0, 0, 0));
        main.Protos.Add(callee);
        var s = Body(main);
        Assert.Contains("\nr0 = r0(r1, r2)\n", s);
        Assert.Contains("\nr1(...)\n", s);
        Assert.Contains("\nr3(r0())\n", s);
        Assert.Contains("\ndo return r0(r1) end\n", s);
        Assert.Contains("F[1] = function(U) return function(...)\nlocal r0, r1", s);
        Assert.Contains("\ndo return ... end\n", s);
    }

    [Fact]
    public void Numeric_and_generic_for_loops()
    {
        var s = Body(Main([],
            AsBx(FORPREP, 0, 0), AsBx(FORLOOP, 0, -1), AsBx(JMP, 0, 0), ABC(TFORCALL, 4, 0, 2), AsBx(TFORLOOP, 6, -2)));
        Assert.Contains("\nr0, r1, r2 = __forprep(r0, r1, r2) goto L1\n", s);
        Assert.Contains("\nr0 = r0 + r2 if (0 < r2 and r0 <= r1) or (not (0 < r2) and r1 <= r0) then r3 = r0 goto L1 end\n", s);
        Assert.Contains("\nr7, r8 = r4(r5, r6)\n", s);
        Assert.Contains("\nif r7 ~= nil then r6 = r7 goto L3 end\n", s);
        Assert.Contains("local function __forprep(init, limit, step)", s);
    }

    [Fact]
    public void Setlist_closures_and_captured_registers()
    {
        var inner = new Fn { Upvals = { (1, 1), (0, 0) }, Code = { ABC(GETUPVAL, 0, 0, 0), ABC(RETURN, 0, 2, 0) } };
        var main = Main([],
            ABC(NEWTABLE, 0, 3, 0), ABC(SETLIST, 0, 2, 1), ABC(SETLIST, 0, 2, 3), ABC(VARARG, 1, 0, 0), ABC(SETLIST, 0, 0, 1),
            ABx(CLOSURE, 2, 0), AsBx(JMP, 2, 0));
        main.Protos.Add(inner);
        var s = Body(main);
        Assert.Contains("\nr0[1] = b1[1] r0[2] = r2\n", s);
        Assert.Contains("\nr0[101] = b1[1] r0[102] = r2\n", s);
        Assert.Contains("\n__setlist(r0, 0, ...)\n", s);
        Assert.Contains("\nlocal b1 = {}\n", s);
        Assert.Contains("\nr2 = F[1]({b1, U[1]})\n", s);
        Assert.Contains("\nb1 = {b1[1]} goto L7\n", s);
        Assert.Contains("F[1] = function(U) return function()\n", s);
    }

    [Fact]
    public void The_kk_ydwe_numbering_translates_like_the_standard_one()
    {
        var bytes = File.ReadAllBytes(Fixture("bytecode-ops.luac"));
        var (main, _) = LuaBytecode.Load(bytes);
        Assert.Equal("standard", OpcodeTable(main));
        // Rewrite every opcode into the KK YDWE numbering, which the translator must undo.
        var toKk = new int[47];
        int[] kkToStd = [22, 20, 21, 24, 23, 26, 25, 28, 27, 30, 29, 32, 31, 34, 33, 36, 35, 38, 37, 40, 39, 41, 42, 44, 43,
                         46, 1, 0, 3, 2, 5, 4, 7, 6, 9, 8, 11, 10, 13, 12, 15, 14, 17, 16, 19, 18, 45];
        for (int k = 0; k < 47; k++) toKk[kkToStd[k]] = k;
        var kk = (byte[])bytes.Clone();
        Patch(kk, main, toKk);
        var (kkMain, _) = LuaBytecode.Load(kk);
        Assert.Equal("kk_ydwe", OpcodeTable(kkMain));
        Assert.Equal(Translate(bytes, "x"), Translate(kk, "x"));
    }

    private static void Patch(byte[] chunk, LuaProto main, int[] map)
    {
        var offsets = CodeOffsets(chunk).ToList();
        Assert.Equal(main.Walk().Sum(f => f.Code.Length), offsets.Count);
        foreach (var off in offsets)
        {
            uint w = BitConverter.ToUInt32(chunk, off);
            BitConverter.GetBytes((w & ~0x3Fu) | (uint)map[w & 0x3F]).CopyTo(chunk, off);
        }
    }

    // The byte offset of every instruction, found by walking the chunk the way luac lays it out.
    private static IEnumerable<int> CodeOffsets(byte[] d)
    {
        int p = 17 + 8 + 8 + 1;
        var list = new List<int>();
        void SkipStr()
        {
            int n = d[p++];
            if (n == 0xFF) { n = (int)BitConverter.ToInt64(d, p); p += 8; }
            if (n > 0) p += n - 1;
        }
        int I() { int v = BitConverter.ToInt32(d, p); p += 4; return v; }
        void Fn()
        {
            SkipStr(); p += 8; p += 3;
            int nc = I();
            for (int k = 0; k < nc; k++) { list.Add(p); p += 4; }
            int nk = I();
            for (int k = 0; k < nk; k++)
            {
                int t = d[p++];
                if (t == 1) p += 1; else if (t is 3 or 0x13) p += 8; else if (t is 4 or 0x14) SkipStr();
            }
            // Read each count before adding, since p += I() reads p before I() moves it.
            int nup = I();
            p += 2 * nup;
            int np = I();
            for (int k = 0; k < np; k++) Fn();
            int nli = I();
            p += 4 * nli;
            int nl = I();
            for (int k = 0; k < nl; k++) { SkipStr(); p += 8; }
            int nu = I();
            for (int k = 0; k < nu; k++) SkipStr();
        }
        Fn();
        return list;
    }

    [Fact]
    public void Constants_print_as_the_doctor_prints_them()
    {
        Assert.Equal("0.1", Literal(0.1));
        Assert.Equal("1e+16", Literal(1e16));
        Assert.Equal("1000000000000000.0", Literal(1e15));
        Assert.Equal("1e-05", Literal(0.00001));
        Assert.Equal("0.0001", Literal(0.0001));
        Assert.Equal("(-2.5)", Literal(-2.5));
        Assert.Equal("1.2345678901234568e+17", Literal(123456789012345678.0));
        Assert.Equal("(-0.0)", Literal(-0.0));
        Assert.Equal("(0/0)", Literal(double.NaN));
        Assert.Equal("(-1/0)", Literal(double.NegativeInfinity));
        Assert.Equal("math.mininteger", Literal(long.MinValue));
        Assert.Equal("0x80000000", Literal(2147483648L));
        Assert.Equal("(-5)", Literal(-5L));
        Assert.Equal("\"a\\\"\\\\\\n\\000\\255\"", Literal(new byte[] { 0x61, 0x22, 0x5C, 0x0A, 0, 0xFF }));
    }

    // ---------------------------------------------------------------- real luac output

    private static string Fixture(string name) => Path.Combine(AppContext.BaseDirectory, "Fixtures", "Lua", name);

    [Fact]
    public void A_luac_chunk_translates_exactly_as_the_doctor_translates_it()
    {
        // bytecode-ops.luac is luac 5.3.6 run on bytecode-ops.lua, and the .doctor.lua beside it is
        // lua53_translate.py's output for it. Running both under lua 5.3.6 printed the same line.
        var expected = File.ReadAllText(Fixture("bytecode-ops.doctor.lua"), Encoding.Latin1).Replace("\r\n", "\n");
        var actual = Translate(File.ReadAllBytes(Fixture("bytecode-ops.luac")), "ops");
        Assert.Equal(expected, actual);
        Assert.Null(LuaParser.Check(actual));
    }

    [Fact]
    public void The_command_replaces_bytecode_entries_and_leaves_source_alone()
    {
        var doc = MapDocument.Load(BlankMap.CreateArchiveBytes());
        Assert.Empty(LuaDecompileCommand.Decompile(doc).Files);

        doc.AddOrReplaceRawFile("war3map.lua", File.ReadAllBytes(Fixture("bytecode-ops.luac")));
        doc.AddOrReplaceRawFile(@"scripts\util.lua", "return 1\n"u8.ToArray());
        var r = LuaDecompileCommand.Decompile(doc);
        var file = Assert.Single(r.Files);
        Assert.Equal("war3map.lua", file.FileName);
        Assert.Equal("standard", file.OpcodeTable);
        Assert.Empty(r.Failed);

        var saved = MapDocument.Load(doc.SaveToBytes());
        Assert.True(saved.TryReadFileByName("war3map.lua", out var lua));
        var text = Encoding.Latin1.GetString(lua);
        Assert.EndsWith(LuaDecompileCommand.RunLine, text);
        Assert.Null(LuaParser.Check(text));
        Assert.True(saved.TryReadFileByName(@"scripts\util.lua", out var util));
        Assert.Equal("return 1\n", Encoding.ASCII.GetString(util));
    }

    [Fact]
    public void The_reader_refuses_a_code_count_larger_than_the_chunk()
    {
        // A function declaring 2,000,000,000 instructions with no code bytes after it. The count is
        // read straight from the file, so it is checked against the bytes left before the array is
        // sized, the way MdxParser.ReadTrack already guards its key count.
        var o = new List<byte> { 0x1B, (byte)'L', (byte)'u', (byte)'a', 0x53, 0 };
        o.AddRange(new byte[] { 0x19, 0x93, 0x0D, 0x0A, 0x1A, 0x0A });
        o.AddRange(new byte[] { 4, 8, 4, 8, 8 });
        o.AddRange(BitConverter.GetBytes(0x5678L));            // header check integer
        o.AddRange(BitConverter.GetBytes(370.5));              // header check float
        o.Add(0);                                              // main upvalue count
        o.Add(0);                                              // source, 0 means inherit
        o.AddRange(BitConverter.GetBytes(0));                  // line defined
        o.AddRange(BitConverter.GetBytes(0));                  // last line defined
        o.Add(0); o.Add(0); o.Add(8);                          // params, vararg, max stack
        o.AddRange(BitConverter.GetBytes(2_000_000_000));      // code count

        var ex = Assert.Throws<LuaBytecodeError>(() => LuaBytecode.Load(o.ToArray()));
        Assert.Contains("instructions", ex.Message);
    }
}
