// tests/Wc3.Tests/SoundAnalyzeCommandTests.cs
using Wc3.Commands;

namespace Wc3.Tests;

public sealed class SoundAnalyzeCommandTests
{
    /// <summary>A WAV with 100 ms of silence, 300 ms of a -6 dB sine, then 100 ms of silence.</summary>
    private static string Clip(string ext = ".wav")
    {
        const int rate = 44100;
        int lead = rate / 10, tone = rate * 3 / 10, tail = rate / 10;
        int count = lead + tone + tail;
        string path = Path.Combine(Path.GetTempPath(), "wc3clip-" + Guid.NewGuid().ToString("N") + ext);
        using var w = new BinaryWriter(File.Create(path));
        w.Write("RIFF"u8); w.Write(36 + count * 2); w.Write("WAVE"u8);
        w.Write("fmt "u8); w.Write(16); w.Write((short)1); w.Write((short)1);
        w.Write(rate); w.Write(rate * 2); w.Write((short)2); w.Write((short)16);
        w.Write("data"u8); w.Write(count * 2);
        for (int i = 0; i < count; i++)
        {
            double s = i >= lead && i < lead + tone
                ? Math.Sin(2 * Math.PI * 440 * (i - lead) / rate) * 0.5
                : 0;
            w.Write((short)(s * short.MaxValue));
        }
        return path;
    }

    [Fact]
    public void It_measures_leading_trailing_silence_and_peak()
    {
        string path = Clip();
        try
        {
            var r = SoundAnalyzeCommand.Execute(path);

            Assert.Equal(0.5, r.Seconds, 2);
            Assert.Equal(44100, r.SampleRate);
            Assert.Equal(1, r.Channels);
            Assert.Equal(0.1, r.LeadingSilenceSeconds, 2);
            Assert.Equal(0.1, r.TrailingSilenceSeconds, 2);
            Assert.InRange(r.PeakDb, -6.2, -5.8);
            Assert.Equal(0, r.ClippedSamples);
            Assert.InRange(r.SilentFraction, 0.35, 0.45);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void A_clip_that_starts_at_full_scale_has_no_leading_silence_and_counts_clipping()
    {
        string path = Path.Combine(Path.GetTempPath(), "wc3clip-" + Guid.NewGuid().ToString("N") + ".wav");
        try
        {
            const int rate = 8000, count = rate / 2;
            using (var w = new BinaryWriter(File.Create(path)))
            {
                w.Write("RIFF"u8); w.Write(36 + count * 2); w.Write("WAVE"u8);
                w.Write("fmt "u8); w.Write(16); w.Write((short)1); w.Write((short)1);
                w.Write(rate); w.Write(rate * 2); w.Write((short)2); w.Write((short)16);
                w.Write("data"u8); w.Write(count * 2);
                for (int i = 0; i < count; i++) w.Write(short.MaxValue);
            }

            var r = SoundAnalyzeCommand.Execute(path);

            Assert.Equal(0, r.LeadingSilenceSeconds);
            Assert.True(r.ClippedSamples > count / 2, "a full scale square should count as clipped");
        }
        finally { File.Delete(path); }
    }
}
