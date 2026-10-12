// tests/Wc3.Tests/ReplayFixtures.cs
using System.IO.Compression;
using System.Text;

namespace Wc3.Tests;

/// <summary>
/// A synthetic .w3g builder, so every replay report command can be tested without a real recorded
/// game. It mirrors the shapes the reader was checked against, the 0x38 metadata block of build
/// 10100 multiplayer games and the 0x20 chat record whose text follows a five byte prefix.
/// </summary>
public static class ReplayFixtures
{
    public sealed record Player(int Id, string Name);

    public abstract record Event(int AtMs);
    public sealed record Chat(int AtMs, int PlayerId, string Text) : Event(AtMs);
    public sealed record Leave(int AtMs, int PlayerId, int Reason, int Result) : Event(AtMs);

    public static readonly Player[] TwoPlayers =
    {
        new(1, "Host#1"),
        new(2, "Guest#2"),
    };

    /// <summary>Build one replay. The header length is the reported game length, which the events
    /// do not have to stay under, exactly as a real replay's chat can land just past its length.</summary>
    public static byte[] Build(
        string map = @"Maps\Download\BaseMap_Repro.w3x",
        uint lengthMs = 123_456,
        Player[]? players = null,
        Event[]? events = null,
        string password = "",
        byte meta = 0x38)
    {
        players ??= TwoPlayers;
        events ??= Array.Empty<Event>();
        var ordered = events.OrderBy(e => e.AtMs).ToArray();

        var s = new MemoryStream();
        var w = new BinaryWriter(s);
        w.Write(1u);                                             // unknown
        w.Write((byte)0x00); w.Write((byte)players[0].Id); CStr(w, players[0].Name);
        w.Write((byte)2); w.Write((ushort)0);                    // additional data
        CStr(w, "Test Game");
        CStr(w, password);
        var settings = new byte[13]
            .Concat(Encoding.ASCII.GetBytes(map + "\0Creator\0\0")).ToArray();
        w.Write(Encode(settings)); w.Write((byte)0);
        w.Write(2u); w.Write(0u); w.Write(0u);                   // players, game type, language
        foreach (var p in players.Skip(1))
        {
            w.Write((byte)0x16); w.Write((byte)p.Id); CStr(w, p.Name);
            w.Write((byte)2); w.Write((ushort)0); w.Write(0u);
        }
        w.Write(meta); w.Write((byte)3); w.Write(4u); w.Write(0u);
        w.Write((byte)0x19); w.Write((ushort)0);                 // game start

        int now = 0;
        foreach (var e in ordered)
        {
            Advance(w, e.AtMs - now);
            now = e.AtMs;
            switch (e)
            {
                case Chat c:
                    var text = Encoding.UTF8.GetBytes(c.Text + "\0");
                    w.Write((byte)0x20); w.Write((byte)c.PlayerId);
                    w.Write((ushort)(5 + text.Length));
                    w.Write((byte)0x20); w.Write(0u);            // the five byte prefix
                    w.Write(text);
                    break;
                case Leave l:
                    w.Write((byte)0x17); w.Write((uint)l.Reason); w.Write((byte)l.PlayerId);
                    w.Write((uint)l.Result); w.Write(0u);
                    break;
            }
        }
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
        h.Write(lengthMs); h.Write(0u);
        h.Write((uint)block.Length); h.Write((uint)data.Length); h.Write(0u);
        h.Write(block);
        h.Flush();
        return f.ToArray();
    }

    /// <summary>A .w3g on disk, for the commands that take a path. The caller deletes it.</summary>
    public static string Write(string path, byte[] bytes)
    {
        File.WriteAllBytes(path, bytes);
        return path;
    }

    // A timeslot record advances the game clock. Two bytes of payload plus the delta.
    private static void Advance(BinaryWriter w, int delta)
    {
        if (delta <= 0) return;
        w.Write((byte)0x1F); w.Write((ushort)2); w.Write((ushort)delta);
    }

    private static void CStr(BinaryWriter w, string s)
    {
        w.Write(Encoding.UTF8.GetBytes(s)); w.Write((byte)0);
    }

    /// <summary>The inverse of the settings encoding, the same routine the existing replay test uses.</summary>
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
