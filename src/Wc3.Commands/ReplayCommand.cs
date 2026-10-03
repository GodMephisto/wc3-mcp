// src/Wc3.Commands/ReplayCommand.cs
using System.IO.Compression;
using System.Text;

namespace Wc3.Commands;

/// <summary>How and when one player left a recorded game.</summary>
public sealed record ReplayLeave(
    int PlayerId, string? Name, TimeSpan At, int Reason, int Result, string Meaning);

/// <summary>One recorded game. <see cref="Disconnected"/> is true when any player left with the
/// disconnect result code, which is what a dropped or desynced connection records.</summary>
public sealed record ReplaySummary(
    string File,
    DateTime Saved,
    string? Map,
    TimeSpan Length,
    int Build,
    IReadOnlyList<string> Players,
    IReadOnlyList<ReplayLeave> Leaves,
    bool Disconnected,
    string? Problem);

public sealed record ReplayReport(IReadOnlyList<ReplaySummary> Games, IReadOnlyList<string> Diagnostics);

/// <summary>
/// Reads Warcraft III replays (.w3g) and reports, per game, the map, the length, and how each
/// player left. This is how a disconnect test is scored without anyone writing results down,
/// because Reforged autosaves every custom game under
/// Documents\Warcraft III\BattleNet\&lt;account&gt;\Replays\Autosaved\Custom.
///
/// Format, from the community w3g specification and checked against replays saved here on
/// build 10200. A 68 byte header gives the decompressed size, the block count and the game
/// length in milliseconds. The body is zlib blocks, each with a 12 byte header from version
/// 10032 on and 8 bytes before it. The decompressed stream holds the host record, the game
/// name, an encoded settings string that carries the map path, the player list, and then the
/// action stream, where a LeaveGame record (0x17) carries a reason and a result code per player.
///
/// Result 0x01 is "disconnected". The others are left, lost, won, draw and observer left. A
/// desync ends in players being dropped, which the saver's replay records as disconnects, but
/// the replay does not say desync in words, so the report states the codes it read.
/// </summary>
public static class ReplayCommand
{
    private static readonly byte[] Magic = Encoding.ASCII.GetBytes("Warcraft III recorded game\x1A\0");

    /// <summary>Every replay under <paramref name="path"/> (a file or a folder, searched
    /// recursively), oldest first, optionally only games whose map path contains
    /// <paramref name="mapFilter"/>.</summary>
    public static ReplayReport Execute(string? path = null, string? mapFilter = null)
    {
        var diagnostics = new List<string>();
        path ??= DefaultFolder();
        if (path is null)
            return new ReplayReport(Array.Empty<ReplaySummary>(),
                new[] { "no replay folder found under Documents\\Warcraft III\\BattleNet, pass a path" });

        if (!File.Exists(path) && !Directory.Exists(path))
            return new ReplayReport(Array.Empty<ReplaySummary>(), new[] { $"no file or folder at {path}" });

        IEnumerable<string> files = File.Exists(path)
            ? new[] { path }
            : Directory.Exists(path)
                ? Directory.EnumerateFiles(path, "*.w3g", SearchOption.AllDirectories)
                : Array.Empty<string>();
        var games = files.Select(Read)
            .Where(g => mapFilter is null
                || (g.Map ?? "").Contains(mapFilter, StringComparison.OrdinalIgnoreCase))
            .OrderBy(g => g.Saved).ToList();
        diagnostics.Add($"read {games.Count} replay(s) from {path}");
        return new ReplayReport(games, diagnostics);
    }

    private static string? DefaultFolder()
    {
        string root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Warcraft III", "BattleNet");
        return Directory.Exists(root) ? root : null;
    }

    public static ReplaySummary Read(string file)
    {
        var saved = File.GetLastWriteTime(file);
        try { return Parse(file, File.ReadAllBytes(file), saved); }
        catch (Exception e) when (e is InvalidDataException or IndexOutOfRangeException
                                     or ArgumentOutOfRangeException or EndOfStreamException)
        {
            return new ReplaySummary(file, saved, null, TimeSpan.Zero, 0,
                Array.Empty<string>(), Array.Empty<ReplayLeave>(), false, e.Message);
        }
    }

    internal static ReplaySummary Parse(string file, byte[] b, DateTime saved)
    {
        if (b.Length < 68 || !b.AsSpan(0, Magic.Length).SequenceEqual(Magic))
            throw new InvalidDataException("not a Warcraft III replay");
        int headerSize = BitConverter.ToInt32(b, 28);
        int blocks = BitConverter.ToInt32(b, 44);
        int version = BitConverter.ToInt32(b, 52);
        int build = BitConverter.ToUInt16(b, 56);
        var length = TimeSpan.FromMilliseconds(BitConverter.ToUInt32(b, 60));

        var data = Decompress(b, headerSize, blocks, blockHeader: version >= 10032 ? 12 : 8);
        var r = new Reader(data);
        var players = new Dictionary<int, string>();

        r.Skip(4);                                   // unknown
        ReadPlayer(r, players);                      // host, record 0x00
        r.CString();                                 // game name
        // The lobby password, empty for a public game. Specifications call this "a null byte",
        // which holds only until a private lobby. It failed 14 of 75 replays here. Never output.
        r.CBytes();
        string? map = MapFrom(Decode(r.CBytes()));
        r.Skip(12);                                  // player count, game type, language
        while (r.Peek() == 0x16) { ReadPlayer(r, players); r.Skip(4); }
        // Reforged metadata, a subtype byte and a u32 length. Build 10100 multiplayer replays mark
        // it 0x38 and build 10200 local ones 0x39, measured here, and reading only 0x39 failed
        // every one of 26 online games.
        while (r.Peek() is 0x38 or 0x39) { r.Skip(2); r.Skip((int)r.U32()); }
        if (r.Byte() != 0x19) throw new InvalidDataException("no game start record");
        r.Skip(r.U16());

        var leaves = new List<ReplayLeave>();
        long ms = 0;
        string? problem = null;
        while (!r.End)
        {
            byte id = r.Byte();
            switch (id)
            {
                case 0x17:
                    int reason = (int)r.U32(), pid = r.Byte(), result = (int)r.U32();
                    r.Skip(4);
                    leaves.Add(new(pid, players.GetValueOrDefault(pid), TimeSpan.FromMilliseconds(ms),
                        reason, result, Meaning(result)));
                    break;
                case 0x1A or 0x1B or 0x1C: r.Skip(4); break;
                case 0x1E or 0x1F:
                    int n = r.U16();
                    ms += r.U16();
                    r.Skip(n - 2);
                    break;
                case 0x20: r.Skip(1); r.Skip(r.U16()); break;
                case 0x22: r.Skip(r.Byte()); break;
                case 0x23: r.Skip(10); break;
                case 0x2F: r.Skip(8); break;
                case 0x00: r.ToEnd(); break;         // zero padding after the last block
                default:
                    problem = $"stopped at unknown record 0x{id:X2}, leaves after it are not listed";
                    r.ToEnd();
                    break;
            }
        }

        bool dropped = leaves.Any(l => l.Result == 0x01);
        return new ReplaySummary(file, saved, map, length, build,
            players.OrderBy(p => p.Key).Select(p => p.Value).ToList(), leaves, dropped, problem);
    }

    private static string Meaning(int result) => result switch
    {
        0x01 => "disconnected",
        0x07 => "left",
        0x08 => "lost",
        0x09 => "won",
        0x0A => "draw",
        0x0B => "observer left",
        // Not in the classic specification. On Reforged replays here it marks ordinary leaving,
        // the saver's own exit at game end included, so it is read as left. Inferred, not documented.
        0x0D => "left",
        _ => $"result 0x{result:X2}",
    };

    private static void ReadPlayer(Reader r, Dictionary<int, string> players)
    {
        r.Skip(1);                                   // record id
        int pid = r.Byte();
        string name = r.CString();
        r.Skip(r.Byte());                            // additional data
        if (name.Length > 0) players[pid] = name;
    }

    private static byte[] Decompress(byte[] b, int offset, int blocks, int blockHeader)
    {
        using var output = new MemoryStream();
        int p = offset;
        for (int i = 0; i < blocks && p + blockHeader <= b.Length; i++)
        {
            int compressed = blockHeader == 12 ? BitConverter.ToInt32(b, p) : BitConverter.ToUInt16(b, p);
            p += blockHeader;
            // zlib, so the 2 byte header is skipped and the rest is raw deflate.
            using var z = new DeflateStream(new MemoryStream(b, p + 2, compressed - 2), CompressionMode.Decompress);
            z.CopyTo(output);
            p += compressed;
        }
        return output.ToArray();
    }

    /// <summary>The settings string is encoded in groups of 8, a mask byte and 7 data bytes,
    /// where a clear mask bit means the stored byte is one more than the real one.</summary>
    internal static byte[] Decode(byte[] e)
    {
        var d = new List<byte>(e.Length);
        for (int i = 0; i < e.Length; i++)
        {
            if (i % 8 == 0) continue;
            byte mask = e[i - i % 8];
            d.Add((mask & (1 << (i % 8))) == 0 ? (byte)(e[i] - 1) : e[i]);
        }
        return d.ToArray();
    }

    private static string? MapFrom(byte[] settings)
    {
        string s = Encoding.UTF8.GetString(settings);
        int end = -1;
        foreach (var ext in new[] { ".w3x", ".w3m" })
        {
            int i = s.IndexOf(ext, StringComparison.OrdinalIgnoreCase);
            if (i >= 0 && (end < 0 || i < end)) end = i;
        }
        if (end < 0) return null;
        int start = s.LastIndexOf('\0', end) + 1;
        return s[start..(end + 4)];
    }

    private sealed class Reader
    {
        private readonly byte[] _b;
        private int _p;
        public Reader(byte[] b) => _b = b;
        public bool End => _p >= _b.Length;
        public byte Peek() => _p < _b.Length ? _b[_p] : (byte)0;
        public byte Byte() => _b[_p++];
        public int U16() { int v = BitConverter.ToUInt16(_b, _p); _p += 2; return v; }
        public uint U32() { uint v = BitConverter.ToUInt32(_b, _p); _p += 4; return v; }
        public void Skip(int n)
        {
            if (n < 0 || _p + n > _b.Length) throw new EndOfStreamException("replay ends early");
            _p += n;
        }
        public void ToEnd() => _p = _b.Length;
        public byte[] CBytes()
        {
            int e = Array.IndexOf(_b, (byte)0, _p);
            if (e < 0) throw new EndOfStreamException("unterminated string");
            var s = _b[_p..e]; _p = e + 1; return s;
        }
        public string CString() => Encoding.UTF8.GetString(CBytes());
    }
}
