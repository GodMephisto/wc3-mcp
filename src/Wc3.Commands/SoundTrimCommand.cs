// src/Wc3.Commands/SoundTrimCommand.cs
using NAudio.Wave;

namespace Wc3.Commands;

/// <summary>What a trim changed, in the units a sound designer works in.</summary>
public sealed record SoundTrimResult(
    string Path, string OutPath, double SourceSeconds, double OutputSeconds,
    double RemovedSeconds, double TrimmedLeadSeconds, double TrimmedTailSeconds,
    double PeakBeforeDb, double PeakAfterDb);

/// <summary>
/// Cuts the silence from both ends of a clip, keeps a chosen margin, and optionally
/// normalises the peak. A sourced anime impact often carries a quarter second of silence
/// before the hit, which reads in game as input lag, and a voice line may sit far below the
/// rest of the set. Both are fixed here rather than in the map.
///
/// Output is 16 bit PCM WAV at the source rate and channel count. Convert to MP3 afterwards
/// with the sound optimiser when size matters. Read-only on the input.
/// </summary>
public static class SoundTrimCommand
{
    public static SoundTrimResult Execute(
        string path, string outPath, double thresholdDb = SoundAnalyzeCommand.DefaultThresholdDb,
        double marginSeconds = 0.01, double? peakDb = null)
    {
        if (!File.Exists(path)) throw new FileNotFoundException($"audio file not found: {path}", path);

        using var reader = new AudioFileReader(path);
        int rate = reader.WaveFormat.SampleRate;
        int channels = reader.WaveFormat.Channels;
        int bits = 16;

        var all = new List<float>(rate * channels * 2);
        var buffer = new float[rate * channels];
        int read;
        while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
            for (int i = 0; i < read; i++) all.Add(buffer[i]);

        int frames = all.Count / channels;
        if (frames == 0) throw new InvalidOperationException($"'{path}' has no audio");

        double linear = Math.Pow(10, thresholdDb / 20.0);
        int first = -1, last = -1;
        for (int f = 0; f < frames; f++)
        {
            for (int c = 0; c < channels; c++)
            {
                if (Math.Abs(all[f * channels + c]) >= linear)
                {
                    if (first < 0) first = f;
                    last = f;
                    break;
                }
            }
        }
        if (first < 0) throw new InvalidOperationException($"'{path}' is silent at {thresholdDb} dB");

        int margin = (int)Math.Round(Math.Max(0, marginSeconds) * rate);
        int start = Math.Max(0, first - margin);
        int end = Math.Min(frames, last + 1 + margin);
        int kept = end - start;

        double peakBefore = 0, peakKept = 0;
        for (int f = 0; f < frames; f++)
            for (int c = 0; c < channels; c++)
                peakBefore = Math.Max(peakBefore, Math.Abs(all[f * channels + c]));
        for (int f = start; f < end; f++)
            for (int c = 0; c < channels; c++)
                peakKept = Math.Max(peakKept, Math.Abs(all[f * channels + c]));

        double gain = 1.0;
        if (peakDb is { } target && peakKept > 0)
            gain = Math.Pow(10, target / 20.0) / peakKept;

        WriteWav(outPath, all, start, kept, channels, rate, bits, gain);

        double sourceSeconds = (double)frames / rate;
        double outputSeconds = (double)kept / rate;
        return new(path, outPath, Math.Round(sourceSeconds, 3), Math.Round(outputSeconds, 3),
            Math.Round(sourceSeconds - outputSeconds, 3),
            Math.Round((double)start / rate, 3),
            Math.Round((double)(frames - end) / rate, 3),
            peakBefore > 0 ? Math.Round(20 * Math.Log10(peakBefore), 2) : double.NegativeInfinity,
            peakKept * gain > 0 ? Math.Round(20 * Math.Log10(peakKept * gain), 2) : double.NegativeInfinity);
    }

    private static void WriteWav(string outPath, List<float> samples, int start, int kept,
        int channels, int rate, int bits, double gain)
    {
        string? dir = Path.GetDirectoryName(Path.GetFullPath(outPath));
        if (dir is { Length: > 0 }) Directory.CreateDirectory(dir);

        int dataBytes = kept * channels * (bits / 8);
        using var w = new BinaryWriter(File.Create(outPath));
        w.Write("RIFF"u8); w.Write(36 + dataBytes); w.Write("WAVE"u8);
        w.Write("fmt "u8); w.Write(16); w.Write((short)1); w.Write((short)channels);
        w.Write(rate); w.Write(rate * channels * (bits / 8));
        w.Write((short)(channels * (bits / 8))); w.Write((short)bits);
        w.Write("data"u8); w.Write(dataBytes);

        for (int i = 0; i < kept * channels; i++)
        {
            double s = Math.Clamp(samples[start * channels + i] * gain, -1, 1);
            w.Write((short)Math.Round(s * short.MaxValue));
        }
    }
}
