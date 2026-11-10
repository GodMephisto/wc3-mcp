// tests/Wc3.Tests/SoundTrimCommandTests.cs
using Wc3.Commands;

namespace Wc3.Tests;

public sealed class SoundTrimCommandTests
{
    private static string Clip(string? path = null)
    {
        path ??= Path.Combine(Path.GetTempPath(), "wc3trim-" + Guid.NewGuid().ToString("N") + ".wav");
        const int rate = 44100;
        int lead = rate / 10, tone = rate * 3 / 10, tail = rate / 10, count = lead + tone + tail;
        using var w = new BinaryWriter(File.Create(path));
        w.Write("RIFF"u8); w.Write(36 + count * 2); w.Write("WAVE"u8);
        w.Write("fmt "u8); w.Write(16); w.Write((short)1); w.Write((short)1);
        w.Write(rate); w.Write(rate * 2); w.Write((short)2); w.Write((short)16);
        w.Write("data"u8); w.Write(count * 2);
        for (int i = 0; i < count; i++)
        {
            double s = i >= lead && i < lead + tone
                ? Math.Sin(2 * Math.PI * 440 * (i - lead) / rate) * 0.5 : 0;
            w.Write((short)(s * short.MaxValue));
        }
        return path;
    }

    [Fact]
    public void It_cuts_the_silence_keeps_a_margin_and_normalises_the_peak()
    {
        string src = Clip();
        string dst = Path.Combine(Path.GetTempPath(), "wc3trim-" + Guid.NewGuid().ToString("N") + ".wav");
        try
        {
            var r = SoundTrimCommand.Execute(src, dst, marginSeconds: 0.01, peakDb: -3);

            Assert.InRange(r.OutputSeconds, 0.31, 0.33);
            Assert.InRange(r.RemovedSeconds, 0.17, 0.19);
            Assert.InRange(r.PeakAfterDb, -3.2, -2.8);

            var check = SoundAnalyzeCommand.Execute(dst);
            Assert.InRange(check.LeadingSilenceSeconds, 0.0, 0.02);
            Assert.InRange(check.TrailingSilenceSeconds, 0.0, 0.02);
            Assert.InRange(check.PeakDb, -3.2, -2.8);
        }
        finally { File.Delete(src); if (File.Exists(dst)) File.Delete(dst); }
    }

    [Fact]
    public void A_silent_clip_is_refused_rather_than_written()
    {
        string src = Path.Combine(Path.GetTempPath(), "wc3silent-" + Guid.NewGuid().ToString("N") + ".wav");
        string dst = Path.Combine(Path.GetTempPath(), "wc3silent-out-" + Guid.NewGuid().ToString("N") + ".wav");
        try
        {
            const int rate = 8000, count = rate / 2;
            using (var w = new BinaryWriter(File.Create(src)))
            {
                w.Write("RIFF"u8); w.Write(36 + count * 2); w.Write("WAVE"u8);
                w.Write("fmt "u8); w.Write(16); w.Write((short)1); w.Write((short)1);
                w.Write(rate); w.Write(rate * 2); w.Write((short)2); w.Write((short)16);
                w.Write("data"u8); w.Write(count * 2);
                for (int i = 0; i < count; i++) w.Write((short)0);
            }

            Assert.Throws<InvalidOperationException>(() => SoundTrimCommand.Execute(src, dst));
            Assert.False(File.Exists(dst));
        }
        finally { File.Delete(src); if (File.Exists(dst)) File.Delete(dst); }
    }
}
