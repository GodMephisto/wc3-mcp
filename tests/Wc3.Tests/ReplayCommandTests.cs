// tests/Wc3.Tests/ReplayCommandTests.cs
using System.IO.Compression;
using System.Text;
using Wc3.Commands;

namespace Wc3.Tests;

/// <summary>
/// Replay scoring. Each synthetic replay carries one of the shapes that broke the first reader
/// on the 75 real replays here, a private lobby password where the specification says "a null
/// byte" (14 failures), and the 0x38 metadata block of build 10100 multiplayer games (26).
/// </summary>
public class ReplayCommandTests
{
    [Fact]
    public void Private_lobby_with_0x38_metadata_reads_the_map_and_the_disconnect()
    {
        var g = ReplayCommand.Parse("x.w3g", Replay(password: "1234", meta: 0x38, disconnect: true), DateTime.Now);

        Assert.Null(g.Problem);
        Assert.Equal(@"Maps\Download\BaseMap_Repro.w3x", g.Map);
        Assert.Equal(TimeSpan.FromMilliseconds(123_456), g.Length);
        Assert.True(g.Disconnected);
        var leave = Assert.Single(g.Leaves);
        Assert.Equal("disconnected", leave.Meaning);
        Assert.Equal(TimeSpan.FromMilliseconds(300), leave.At);
        Assert.Equal("Guest#2", leave.Name);
    }

    [Fact]
    public void Public_lobby_with_0x39_metadata_and_a_normal_leave_is_clean()
    {
        var g = ReplayCommand.Parse("x.w3g", Replay(password: "", meta: 0x39, disconnect: false), DateTime.Now);

        Assert.Null(g.Problem);
        Assert.False(g.Disconnected);
        Assert.Equal("left", Assert.Single(g.Leaves).Meaning);
    }

    [Fact]
    public void A_file_that_is_not_a_replay_is_reported_not_thrown()
    {
        string path = Path.Combine(Path.GetTempPath(), $"wc3ctl-not-a-replay-{Guid.NewGuid():N}.w3g");
        File.WriteAllBytes(path, new byte[100]);
        try
        {
            var g = ReplayCommand.Read(path);
            Assert.Equal("not a Warcraft III replay", g.Problem);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void Decode_inverts_the_settings_encoding()
    {
        var raw = Encoding.ASCII.GetBytes("Maps\\Download\\Even and odd bytes 0123");
        Assert.Equal(raw, ReplayCommand.Decode(Encode(raw)));
    }

    /// <summary>Every replay on this machine reads, which is the check the synthetic cases
    /// were derived from.</summary>
    [Fact]
    [Trait("Category", "Corpus")]
    public void Every_saved_replay_reads()
    {
        var r = ReplayCommand.Execute();
        Assert.NotEmpty(r.Games);
        Assert.All(r.Games, g => Assert.Null(g.Problem));
    }

    // ---- a replay built from the format --------------------------------------------------------

    private static byte[] Replay(string password, byte meta, bool disconnect)
    {
        var s = new MemoryStream();
        var w = new BinaryWriter(s);
        w.Write(1u);                                             // unknown
        w.Write((byte)0x00); w.Write((byte)1); CStr(w, "Host#1"); w.Write((byte)2); w.Write((ushort)0);
        CStr(w, "Test Game");
        CStr(w, password);
        var settings = new byte[13].Concat(Encoding.ASCII.GetBytes("Maps\\Download\\BaseMap_Repro.w3x\0Creator\0\0")).ToArray();
        w.Write(Encode(settings)); w.Write((byte)0);
        w.Write(2u); w.Write(0u); w.Write(0u);                   // players, game type, language
        w.Write((byte)0x16); w.Write((byte)2); CStr(w, "Guest#2"); w.Write((byte)2); w.Write((ushort)0); w.Write(0u);
        w.Write(meta); w.Write((byte)3); w.Write(4u); w.Write(0u);
        w.Write((byte)0x19); w.Write((ushort)0);
        w.Write((byte)0x1A); w.Write(1u);
        for (int i = 0; i < 3; i++) { w.Write((byte)0x1F); w.Write((ushort)2); w.Write((ushort)100); }
        w.Write((byte)0x17); w.Write(0x0Cu); w.Write((byte)2); w.Write(disconnect ? 0x01u : 0x07u); w.Write(0u);
        w.Flush();
        byte[] data = s.ToArray();

        var z = new MemoryStream();
        using (var zl = new ZLibStream(z, CompressionLevel.Optimal, leaveOpen: true)) zl.Write(data);
        byte[] block = z.ToArray();

        var f = new MemoryStream();
        var h = new BinaryWriter(f);
        h.Write(Encoding.ASCII.GetBytes("Warcraft III recorded game\x1A\0"));
        h.Write(68u); h.Write((uint)(block.Length + 12)); h.Write(1u); h.Write((uint)data.Length); h.Write(1u);
        h.Write(Encoding.ASCII.GetBytes("PX3W")); h.Write(10200u); h.Write((ushort)7000); h.Write((ushort)0x8000);
        h.Write(123_456u); h.Write(0u);
        h.Write((uint)block.Length); h.Write((uint)data.Length); h.Write(0u);
        h.Write(block);
        h.Flush();
        return f.ToArray();
    }

    private static void CStr(BinaryWriter w, string s) { w.Write(Encoding.UTF8.GetBytes(s)); w.Write((byte)0); }

    /// <summary>The inverse of the settings encoding. Each group of up to 7 bytes gets a mask
    /// byte, and an even byte is stored plus one with its mask bit clear.</summary>
    private static byte[] Encode(byte[] raw)
    {
        var o = new List<byte>();
        for (int g = 0; g < raw.Length; g += 7)
        {
            var chunk = raw.Skip(g).Take(7).ToArray();
            byte mask = 1;
            var body = new byte[chunk.Length];
            for (int i = 0; i < chunk.Length; i++)
            {
                if (chunk[i] % 2 == 0) body[i] = (byte)(chunk[i] + 1);
                else { body[i] = chunk[i]; mask |= (byte)(1 << (i + 1)); }
            }
            o.Add(mask); o.AddRange(body);
        }
        return o.ToArray();
    }
}
