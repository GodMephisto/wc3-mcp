// src/Wc3.Commands/SoundOptimizeCommand.cs
namespace Wc3.Commands;

/// <summary>What a sound optimise changed, before and after, for the report.</summary>
public sealed record SoundOptimizeResult(
    string Path,
    string Format,
    int BitrateKbps,
    string? Note,
    int SampleRateBefore,
    int ChannelsBefore,
    long BytesBefore,
    int SampleRateAfter,
    int ChannelsAfter,
    long BytesAfter);

/// <summary>
/// Shrinks a sound by decoding it, resampling to a target rate, optionally setting the channel
/// count and encoding it as WAV or MP3. The format comes from the output path's extension, .mp3
/// selects MP3 and anything else WAV. The decode, mix and encode live in <see cref="SoundConvert"/>.
/// A WC3 map's imported audio is usually a full-rate stereo file far heavier than the engine needs,
/// and this is the audio half of making a map compact, beside <see cref="TextureOptimizeCommand"/>.
///
/// <para>MP3 needs the Windows Media Foundation encoder. When this machine has none the command
/// does not fail, it writes WAV instead and says so in <see cref="SoundOptimizeResult.Note"/>.</para>
/// </summary>
public static class SoundOptimizeCommand
{
    /// <summary>The default target rate, 22050 Hz, which most WC3 sounds already use.</summary>
    public const int DefaultRate = 22050;

    /// <summary>The default MP3 bitrate in kbit/s when the caller names none.</summary>
    public const int DefaultBitrateKbps = 128;

    public const int MinBitrateKbps = 8;
    public const int MaxBitrateKbps = 320;

    /// <summary>
    /// Resolves a channel token to a channel count, null meaning keep the source. Accepts the
    /// friendly words and the plain numbers, "source"/"keep", "mono"/"1" and "stereo"/"2".
    /// Shared by every front-end so the parsing lives in one place.
    /// </summary>
    public static int? ParseChannels(string? token) => (token ?? "").Trim().ToLowerInvariant() switch
    {
        "" or "source" or "keep" => null,
        "mono" or "1" => 1,
        "stereo" or "2" => 2,
        _ => throw new ArgumentException(
            $"channels must be source, mono or stereo, not '{token}'", nameof(token)),
    };

    /// <summary>Optimises to WAV, keeping the source channels unless <paramref name="mono"/> is set.</summary>
    public static SoundOptimizeResult ExecutePath(string path, int targetRate, bool mono, string outPath)
        => ExecutePath(path, targetRate, mono ? 1 : null, null, outPath);

    /// <summary>
    /// Optimises <paramref name="path"/> into <paramref name="outPath"/>. The output extension
    /// selects the format. <paramref name="channels"/> is null to keep the source count, 1 for mono
    /// or 2 for stereo. <paramref name="bitrateKbps"/> applies to MP3 and defaults to 128.
    /// </summary>
    public static SoundOptimizeResult ExecutePath(
        string path, int targetRate, int? channels, int? bitrateKbps, string outPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (!File.Exists(path)) throw new FileNotFoundException($"sound not found: {path}", path);
        if (string.IsNullOrWhiteSpace(outPath))
            throw new ArgumentException("output path must not be blank", nameof(outPath));
        if (channels is not (null or 1 or 2))
            throw new ArgumentOutOfRangeException(nameof(channels), channels, "channels must be null, 1 or 2");

        var requested = SoundConvert.FormatForPath(outPath);
        var format = requested;
        string target = outPath;
        string? note = null;

        // No MP3 encoder on this machine is a platform limit, not a bad request, so fall back to
        // WAV and record why rather than failing the whole run.
        if (format == SoundFormat.Mp3 && !SoundConvert.Mp3EncoderAvailable())
        {
            format = SoundFormat.Wav;
            target = Path.ChangeExtension(outPath, ".wav");
            note = "no MP3 encoder is available on this machine, wrote a WAV instead";
        }

        int kbps = bitrateKbps ?? DefaultBitrateKbps;
        if (format == SoundFormat.Mp3 && kbps is < MinBitrateKbps or > MaxBitrateKbps)
            throw new ArgumentOutOfRangeException(nameof(bitrateKbps), bitrateKbps,
                $"an mp3 bitrate must be {MinBitrateKbps}..{MaxBitrateKbps} kbps");

        int bitsPerSecond = format == SoundFormat.Mp3 ? kbps * 1000 : 0;
        var bytes = SoundConvert.ToOptimized(path, format, targetRate, channels, bitsPerSecond,
            out var before, out var after);
        File.WriteAllBytes(target, bytes);

        return new SoundOptimizeResult(target, FormatName(format),
            format == SoundFormat.Mp3 ? kbps : 0, note,
            before.SampleRate, before.Channels, before.Bytes,
            after.SampleRate, after.Channels, after.Bytes);
    }

    private static string FormatName(SoundFormat format) =>
        format == SoundFormat.Mp3 ? "mp3" : "wav";
}
