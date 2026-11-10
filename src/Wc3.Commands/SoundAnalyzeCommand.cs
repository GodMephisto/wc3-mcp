// src/Wc3.Commands/SoundAnalyzeCommand.cs
using NAudio.Wave;

namespace Wc3.Commands;

/// <summary>What a clip measures, before any processing decision.</summary>
public sealed record SoundAnalyzeResult(
    string Path, double Seconds, int SampleRate, int Channels, int BitsPerSample,
    double PeakDb, double RmsDb, int ClippedSamples,
    double LeadingSilenceSeconds, double TrailingSilenceSeconds, double SilentFraction,
    double ThresholdDb);

/// <summary>
/// Measures an audio clip so a sourced file can be judged before it is used. Reports the
/// duration, format, peak and RMS loudness, clipped samples, and how much silence sits at the
/// head and tail. Leading silence is the one that matters most, because a clip that starts
/// late makes a correctly timed attack feel unresponsive.
///
/// Decoding is NAudio, the same path the sound optimiser uses, so WAV and MP3 measure the same
/// way they convert. Read-only.
/// </summary>
public static class SoundAnalyzeCommand
{
    /// <summary>The default silence threshold, in dBFS. -60 dB is below audible room tone and
    /// above a true digital zero, so a fade to silence is measured as silence.</summary>
    public const double DefaultThresholdDb = -60.0;

    public static SoundAnalyzeResult Execute(string path, double thresholdDb = DefaultThresholdDb)
    {
        if (!File.Exists(path)) throw new FileNotFoundException($"audio file not found: {path}", path);

        using var reader = new AudioFileReader(path);
        int rate = reader.WaveFormat.SampleRate;
        int channels = reader.WaveFormat.Channels;
        int bits = reader.WaveFormat.BitsPerSample;

        var buffer = new float[rate * channels];
        double peak = 0, sumSquares = 0;
        long total = 0;
        int clipped = 0;

        // Silence is measured from the decoded float stream at the threshold, so it is the same
        // unit for a quiet MP3 and a loud WAV. A sample is silent when its absolute value is
        // below the linear threshold.
        double linear = Math.Pow(10, thresholdDb / 20.0);
        long firstAudible = -1, lastAudible = -1;
        long frames = 0;

        int read;
        while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
        {
            for (int i = 0; i < read; i++)
            {
                double s = buffer[i];
                double a = Math.Abs(s);
                if (a > peak) peak = a;
                sumSquares += s * s;
                total++;
                if (a >= 0.999) clipped++;

                if (a >= linear)
                {
                    long frame = frames + i / channels;
                    if (firstAudible < 0) firstAudible = frame;
                    lastAudible = frame;
                }
            }
            frames += read / channels;
        }

        double seconds = total > 0 ? (double)frames / rate : 0;
        double peakDb = peak > 0 ? 20 * Math.Log10(peak) : double.NegativeInfinity;
        double rms = total > 0 ? Math.Sqrt(sumSquares / total) : 0;
        double rmsDb = rms > 0 ? 20 * Math.Log10(rms) : double.NegativeInfinity;

        double leading = firstAudible < 0 ? seconds : (double)firstAudible / rate;
        double trailing = lastAudible < 0 ? seconds : seconds - (double)(lastAudible + 1) / rate;
        long audibleFrames = lastAudible < 0 ? 0 : lastAudible - firstAudible + 1;
        double silentFraction = frames > 0 ? 1.0 - (double)audibleFrames / frames : 1.0;

        return new(path, Math.Round(seconds, 3), rate, channels, bits,
            Math.Round(peakDb, 2), Math.Round(rmsDb, 2), clipped,
            Math.Round(leading, 3), Math.Round(trailing, 3),
            Math.Round(silentFraction, 3), thresholdDb);
    }
}
