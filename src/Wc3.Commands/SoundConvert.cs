// src/Wc3.Commands/SoundConvert.cs
using NAudio.MediaFoundation;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Wc3.Commands;

/// <summary>The output container an optimise writes. The output path's extension selects it.</summary>
public enum SoundFormat
{
    /// <summary>Uncompressed 16-bit PCM in a RIFF/WAVE container. Lossless, and the default.</summary>
    Wav,

    /// <summary>Compressed MPEG Layer III through the Windows Media Foundation encoder.</summary>
    Mp3,
}

/// <summary>The sample rate, channel count and byte size of an audio clip, before or after a convert.</summary>
public sealed record SoundInfo(int SampleRate, int Channels, long Bytes);

/// <summary>
/// Decodes, resamples, mixes and encodes audio through NAudio. Shared by the sound optimiser so the
/// decode and encode live in one place rather than inline in the command. NAudio's AudioFileReader
/// handles .wav natively and .mp3 through the Windows codecs, the same route the Studio's
/// AudioPlayer uses. WdlResamplingSampleProvider does the resample with no codec dependency, and
/// WAV is written by NAudio's own writer while MP3 goes through Media Foundation.
/// </summary>
public static class SoundConvert
{
    /// <summary>NAudio's default MP3 rate, 128 kbit/s, in bits per second.</summary>
    public const int DefaultMp3Bitrate = 128000;

    /// <summary>
    /// Maps an output path or a bare extension to the format its extension selects. A .mp3
    /// extension means MP3, anything else (including no extension) means WAV, the default.
    /// </summary>
    public static SoundFormat FormatForPath(string? pathOrExtension)
    {
        var text = pathOrExtension ?? "";
        int dot = text.LastIndexOf('.');
        var ext = dot >= 0 ? text[(dot + 1)..] : "";
        return ext.Equals("mp3", StringComparison.OrdinalIgnoreCase) ? SoundFormat.Mp3 : SoundFormat.Wav;
    }

    /// <summary>
    /// True when this machine can encode MP3, false when it cannot. Media Foundation is absent on
    /// a Windows N edition or a Server Core without the Media Feature Pack, and there the client
    /// consults this to fall back to WAV rather than fail. Any probe failure reads as unavailable.
    /// </summary>
    public static bool Mp3EncoderAvailable()
    {
        try
        {
            MediaFoundationApi.Startup();
            try
            {
                return MediaFoundationEncoder.GetOutputMediaTypes(AudioSubtypes.MFAudioFormat_MP3).Length > 0;
            }
            finally
            {
                MediaFoundationApi.Shutdown();
            }
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Reads <paramref name="path"/> (any audio AudioFileReader can open), optionally folds it to
    /// mono, resamples it to <paramref name="targetRate"/> and returns 16-bit PCM WAV bytes. The
    /// <paramref name="before"/> info describes the source file, <paramref name="after"/> the WAV
    /// produced. Kept for the mono-or-keep callers; it forwards to <see cref="ToOptimized"/>.
    /// </summary>
    public static byte[] ToOptimizedWav(
        string path, int targetRate, bool mono, out SoundInfo before, out SoundInfo after)
        => ToOptimized(path, SoundFormat.Wav, targetRate, mono ? 1 : null, DefaultMp3Bitrate, out before, out after);

    /// <summary>
    /// Decodes <paramref name="path"/>, applies <paramref name="channels"/> (null keeps the source
    /// channel count, 1 folds to mono, 2 gives stereo) and resamples to <paramref name="targetRate"/>,
    /// then encodes in <paramref name="format"/>. <paramref name="bitrate"/> is bits per second and
    /// applies to MP3 only. The <paramref name="before"/> info describes the source file and
    /// <paramref name="after"/> the encoded result.
    /// </summary>
    public static byte[] ToOptimized(
        string path, SoundFormat format, int targetRate, int? channels, int bitrate,
        out SoundInfo before, out SoundInfo after)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (targetRate <= 0)
            throw new ArgumentOutOfRangeException(nameof(targetRate), targetRate, "sample rate must be positive");
        if (channels is not (null or 1 or 2))
            throw new ArgumentOutOfRangeException(nameof(channels), channels, "channels must be null, 1 or 2");
        if (format == SoundFormat.Mp3 && bitrate <= 0)
            throw new ArgumentOutOfRangeException(nameof(bitrate), bitrate, "an mp3 bitrate must be positive");

        using var reader = new AudioFileReader(path);
        before = new SoundInfo(reader.WaveFormat.SampleRate, reader.WaveFormat.Channels, new FileInfo(path).Length);

        // Mix channels first, so the resampler runs on the smaller signal.
        ISampleProvider samples = ApplyChannels(reader, channels);
        if (samples.WaveFormat.SampleRate != targetRate)
            samples = new WdlResamplingSampleProvider(samples, targetRate);

        var bytes = format == SoundFormat.Mp3 ? EncodeMp3(samples, bitrate) : EncodeWav(samples);
        after = new SoundInfo(targetRate, samples.WaveFormat.Channels, bytes.Length);
        return bytes;
    }

    /// <summary>Writes <paramref name="samples"/> as a 16-bit PCM WAV into a fresh byte array.</summary>
    private static byte[] EncodeWav(ISampleProvider samples)
    {
        using var ms = new MemoryStream();
        WaveFileWriter.WriteWavFileToStream(ms, samples.ToWaveProvider16());
        return ms.ToArray();
    }

    /// <summary>Encodes <paramref name="samples"/> to MP3 at <paramref name="bitrate"/> bits per second.</summary>
    private static byte[] EncodeMp3(ISampleProvider samples, int bitrate)
    {
        using var ms = new MemoryStream();
        MediaFoundationEncoder.EncodeToMp3(samples.ToWaveProvider16(), ms, bitrate);
        return ms.ToArray();
    }

    /// <summary>Applies a channel request, leaving the source untouched when it already matches.</summary>
    private static ISampleProvider ApplyChannels(ISampleProvider source, int? channels)
    {
        if (channels is null || channels == source.WaveFormat.Channels)
            return source;
        return channels == 1 ? Downmix(source) : UpmixToStereo(source);
    }

    /// <summary>
    /// Folds any channel count to mono. Stereo is averaged directly, more than two channels keep
    /// the first pair and average them, since a WC3 sound is at most stereo in practice.
    /// </summary>
    private static ISampleProvider Downmix(ISampleProvider source)
    {
        switch (source.WaveFormat.Channels)
        {
            case 1:
                return source;
            case 2:
                return new StereoToMonoSampleProvider(source) { LeftVolume = 0.5f, RightVolume = 0.5f };
            default:
                var firstPair = new MultiplexingSampleProvider(new[] { source }, 2);
                return new StereoToMonoSampleProvider(firstPair) { LeftVolume = 0.5f, RightVolume = 0.5f };
        }
    }

    /// <summary>
    /// Gives two channels. A mono source is duplicated, more than two channels keep the first pair,
    /// the mirror of <see cref="Downmix"/>.
    /// </summary>
    private static ISampleProvider UpmixToStereo(ISampleProvider source)
    {
        return source.WaveFormat.Channels switch
        {
            1 => new MonoToStereoSampleProvider(source),
            _ => new MultiplexingSampleProvider(new[] { source }, 2),
        };
    }
}
