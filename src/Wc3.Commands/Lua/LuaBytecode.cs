// src/Wc3.Commands/Lua/LuaBytecode.cs
// Ported from Devo's Map Doctor, doctor/script/lua_bytecode.py (commit 80efaf6), MIT, devoltzz.
// See THIRD-PARTY-NOTICES.md.
namespace Wc3.Commands.Lua;

/// <summary>A Lua chunk that cannot be read as Lua 5.3 bytecode, with the reason.</summary>
public sealed class LuaBytecodeError(string message) : Exception(message);

/// <summary>
/// One function of a precompiled Lua 5.3 chunk. Constants are null, bool, long (an integer),
/// double (a float) or byte[] (a string, kept as bytes since Lua strings are byte strings).
/// </summary>
public sealed class LuaProto
{
    public byte[]? Source { get; init; }
    public int Line { get; init; }
    public int LastLine { get; init; }
    public int Params { get; init; }
    public int Vararg { get; init; }
    public int Stack { get; init; }
    public uint[] Code { get; set; } = [];
    public List<object?> Consts { get; init; } = new();
    public List<(int InStack, int Index)> Upvals { get; init; } = new();
    public List<LuaProto> Protos { get; init; } = new();
    public List<int> LineInfo { get; init; } = new();
    public List<(byte[]? Name, int Start, int End)> LocVars { get; init; } = new();
    public List<byte[]?> UpNames { get; init; } = new();

    /// <summary>This function and every function nested in it, depth first.</summary>
    public IEnumerable<LuaProto> Walk()
    {
        yield return this;
        foreach (var p in Protos)
            foreach (var g in p.Walk())
                yield return g;
    }
}

/// <summary>Reads a precompiled Lua 5.3 chunk, the format luac writes and string.dump returns.</summary>
public static class LuaBytecode
{
    /// <summary>True when <paramref name="data"/> starts with the Lua 5.3 chunk signature, ESC "Lua" 0x53.</summary>
    public static bool IsLua53Chunk(ReadOnlySpan<byte> data) =>
        data.Length >= 5 && data[0] == 0x1B && data[1] == (byte)'L' && data[2] == (byte)'u' && data[3] == (byte)'a' && data[4] == 0x53;

    /// <summary>
    /// The main function of the chunk, and whether the chunk ended exactly where the main function
    /// did. Bytes left over mean the data is not one chunk.
    /// </summary>
    public static (LuaProto Main, bool Whole) Load(byte[] data)
    {
        var r = new Reader(data);
        var f = r.Function(null);
        return (f, r.Position == data.Length);
    }

    private sealed class Reader
    {
        private readonly byte[] _d;
        private readonly int _sint, _ssize, _sinstr, _sinteger;
        public int Position;

        public Reader(byte[] data)
        {
            if (data.Length < 18 || !IsLua53Chunk(data))
                throw new LuaBytecodeError($"not a Lua 5.3 chunk ({Convert.ToHexString(data.AsSpan(0, Math.Min(6, data.Length)))})");
            _d = data;
            _sint = data[12];
            _ssize = data[13];
            _sinstr = data[14];
            _sinteger = data[15];
            int snumber = data[16];
            // Past the sizes, the check integer and float, and the main function's upvalue count.
            Position = 17 + _sinteger + snumber + 1;
        }

        private int Byte()
        {
            if (Position >= _d.Length) throw new LuaBytecodeError($"the chunk ends early ({Position + 1} of {_d.Length})");
            return _d[Position++];
        }

        private ReadOnlySpan<byte> Raw(long n)
        {
            if (Position + n > _d.Length) throw new LuaBytecodeError($"the chunk ends early ({Position + n} of {_d.Length})");
            var v = _d.AsSpan(Position, (int)n);
            Position += (int)n;
            return v;
        }

        private static long Little(ReadOnlySpan<byte> b, bool signed)
        {
            ulong v = 0;
            for (int k = b.Length - 1; k >= 0; k--) v = (v << 8) | b[k];
            if (signed && b.Length is > 0 and < 8 && (b[^1] & 0x80) != 0)
                v |= ulong.MaxValue << (8 * b.Length);
            return (long)v;
        }

        private int Int() => checked((int)Little(Raw(_sint), signed: true));
        private long Size() => Little(Raw(_ssize), signed: false);

        private byte[]? String()
        {
            long n = Byte();
            if (n == 0) return null;
            if (n == 0xFF) n = Size();
            return Raw(n - 1).ToArray();
        }

        private int Count()
        {
            int n = Int();
            if (n < 0) throw new LuaBytecodeError($"negative count {n} at byte {Position}");
            return n;
        }

        public LuaProto Function(byte[]? parentSource)
        {
            var source = String();
            if (source is null || source.Length == 0) source = parentSource;
            int line = Int(), last = Int();
            int ps = Byte(), va = Byte(), st = Byte();
            int nc = Count();
            var code = new uint[nc];
            for (int k = 0; k < nc; k++) code[k] = (uint)Little(Raw(_sinstr), signed: false);
            var consts = new List<object?>();
            for (int k = 0, n = Count(); k < n; k++)
            {
                int t = Byte();
                consts.Add(t switch
                {
                    0 => null,
                    1 => Byte() != 0,
                    3 => BitConverter.ToDouble(Raw(8)),
                    0x13 => Little(Raw(_sinteger), signed: true),
                    4 or 0x14 => String(),
                    _ => throw new LuaBytecodeError($"constant of type {t} at byte {Position}"),
                });
            }
            var upvals = new List<(int, int)>();
            for (int k = 0, n = Count(); k < n; k++) upvals.Add((Byte(), Byte()));
            var protos = new List<LuaProto>();
            for (int k = 0, n = Count(); k < n; k++) protos.Add(Function(source));
            var lineinfo = new List<int>();
            for (int k = 0, n = Count(); k < n; k++) lineinfo.Add(Int());
            var locvars = new List<(byte[]?, int, int)>();
            for (int k = 0, n = Count(); k < n; k++) locvars.Add((String(), Int(), Int()));
            var upnames = new List<byte[]?>();
            for (int k = 0, n = Count(); k < n; k++) upnames.Add(String());
            return new LuaProto
            {
                Source = source, Line = line, LastLine = last, Params = ps, Vararg = va, Stack = st,
                Code = code, Consts = consts, Upvals = upvals, Protos = protos, LineInfo = lineinfo,
                LocVars = locvars, UpNames = upnames,
            };
        }
    }
}
