// tests/Wc3.Tests/SoundOptimizeCommandTests.cs
using NAudio.Wave;
using Wc3.Commands;
using Xunit;

namespace Wc3.Tests;

/// <summary>Hermetic coverage of <see cref="SoundOptimizeCommand"/> and the <see cref="SoundConvert"/>
/// helper it wraps. A small .wav is generated in memory with NAudio, optimised, and the produced
/// .wav is read back to prove the rate, channel count and byte size it reports. No corpus files.</summary>
public class SoundOptimizeCommandTests
{
    /// <summary>A real .wav on disk, a 440 Hz tone of the given rate, channels and length.</summary>
    private static string NewWav(int rate, int channels, int milliseconds)
    {
        var path = Path.Combine(Path.GetTempPath(), $"wc3sound-{Guid.NewGuid():N}.wav");
        var format = new WaveFormat(rate, 16, channels);
        using var writer = new WaveFileWriter(path, format);
        int frames = rate * milliseconds / 1000;
        for (int i = 0; i < frames; i++)
            for (int c = 0; c < channels; c++)
                writer.WriteSample(0.2f * MathF.Sin(2 * MathF.PI * 440 * i / rate));
        return path;
    }

    private static string NewOut() =>
        Path.Combine(Path.GetTempPath(), $"wc3sound-out-{Guid.NewGuid():N}.wav");

    [Fact]
    public void Resamples_to_the_target_rate_and_downmixes_to_mono()
    {
        var src = NewWav(44100, 2, 200);
        var outPath = NewOut();
        try
        {
            var r = SoundOptimizeCommand.ExecutePath(src, 22050, mono: true, outPath);

            Assert.Equal(44100, r.SampleRateBefore);
            Assert.Equal(2, r.ChannelsBefore);
            Assert.Equal(new FileInfo(src).Length, r.BytesBefore);
            Assert.Equal(22050, r.SampleRateAfter);
            Assert.Equal(1, r.ChannelsAfter);
            Assert.Equal(new FileInfo(outPath).Length, r.BytesAfter);

            // The output must be a real, readable 16-bit PCM .wav at the new rate.
            using var read = new WaveFileReader(outPath);
            Assert.Equal(22050, read.WaveFormat.SampleRate);
            Assert.Equal(1, read.WaveFormat.Channels);
            Assert.Equal(16, read.WaveFormat.BitsPerSample);
            Assert.True(read.Length > 0);
        }
        finally
        {
            File.Delete(src);
            if (File.Exists(outPath)) File.Delete(outPath);
        }
    }

    [Fact]
    public void Keeps_stereo_when_mono_is_not_requested()
    {
        var src = NewWav(44100, 2, 200);
        var outPath = NewOut();
        try
        {
            var r = SoundOptimizeCommand.ExecutePath(src, 22050, mono: false, outPath);

            Assert.Equal(22050, r.SampleRateAfter);
            Assert.Equal(2, r.ChannelsAfter);

            using var read = new WaveFileReader(outPath);
            Assert.Equal(2, read.WaveFormat.Channels);
        }
        finally
        {
            File.Delete(src);
            if (File.Exists(outPath)) File.Delete(outPath);
        }
    }

    [Fact]
    public void A_same_rate_wav_passes_through_as_a_valid_wav()
    {
        var src = NewWav(22050, 1, 100);
        var outPath = NewOut();
        try
        {
            var r = SoundOptimizeCommand.ExecutePath(src, 22050, mono: false, outPath);

            Assert.Equal(22050, r.SampleRateAfter);
            Assert.Equal(1, r.ChannelsAfter);

            using var read = new WaveFileReader(outPath);
            Assert.Equal(22050, read.WaveFormat.SampleRate);
        }
        finally
        {
            File.Delete(src);
            if (File.Exists(outPath)) File.Delete(outPath);
        }
    }

    [Fact]
    public void Missing_input_throws_file_not_found()
    {
        Assert.Throws<FileNotFoundException>(() =>
            SoundOptimizeCommand.ExecutePath("Z:\\nowhere\\missing.wav", 22050, false, NewOut()));
    }

    [Fact]
    public void A_non_positive_rate_is_rejected()
    {
        var src = NewWav(22050, 1, 50);
        try
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                SoundOptimizeCommand.ExecutePath(src, 0, false, NewOut()));
        }
        finally { File.Delete(src); }
    }

    [Theory]
    [InlineData("out.wav", SoundFormat.Wav)]
    [InlineData("out.WAV", SoundFormat.Wav)]
    [InlineData("out.mp3", SoundFormat.Mp3)]
    [InlineData("out.MP3", SoundFormat.Mp3)]
    [InlineData("noextension", SoundFormat.Wav)]
    [InlineData("out.ogg", SoundFormat.Wav)]
    public void The_output_extension_selects_the_format(string path, SoundFormat expected)
        => Assert.Equal(expected, SoundConvert.FormatForPath(path));

    [Fact]
    public void The_wav_dispatch_writes_a_riff_container()
    {
        var src = NewWav(44100, 2, 120);
        var outPath = NewOut();
        try
        {
            var r = SoundOptimizeCommand.ExecutePath(src, 22050, channels: null, bitrateKbps: null, outPath);

            Assert.Equal("wav", r.Format);
            Assert.Equal(0, r.BitrateKbps);
            Assert.Null(r.Note);
            var head = File.ReadAllBytes(outPath);
            Assert.Equal((byte)'R', head[0]);
            Assert.Equal((byte)'I', head[1]);
            Assert.Equal((byte)'F', head[2]);
            Assert.Equal((byte)'F', head[3]);
        }
        finally
        {
            File.Delete(src);
            if (File.Exists(outPath)) File.Delete(outPath);
        }
    }

    [Fact]
    public void The_mp3_dispatch_encodes_mp3_or_falls_back_to_wav()
    {
        var src = NewWav(44100, 2, 200);
        var outPath = Path.Combine(Path.GetTempPath(), $"wc3sound-{Guid.NewGuid():N}.mp3");
        string? fallback = null;
        try
        {
            var r = SoundOptimizeCommand.ExecutePath(src, 22050, channels: null, bitrateKbps: 128, outPath);

            if (SoundConvert.Mp3EncoderAvailable())
            {
                Assert.Equal("mp3", r.Format);
                Assert.Equal(128, r.BitrateKbps);
                Assert.Null(r.Note);
                Assert.Equal(outPath, r.Path);

                var head = File.ReadAllBytes(outPath);
                Assert.True(head.Length > 0);
                bool id3 = head.Length >= 3 && head[0] == 'I' && head[1] == 'D' && head[2] == '3';
                bool frame = head.Length >= 2 && head[0] == 0xFF && (head[1] & 0xE0) == 0xE0;
                Assert.True(id3 || frame, "the output does not look like an mp3");

                using var read = new AudioFileReader(outPath);
                Assert.Equal(22050, read.WaveFormat.SampleRate);
                Assert.Equal(2, read.WaveFormat.Channels);
            }
            else
            {
                // No MP3 encoder on this machine is a platform limit, not a failure. The command
                // writes a WAV and says so rather than throwing, and this branch proves that.
                Assert.Equal("wav", r.Format);
                Assert.NotNull(r.Note);
                Assert.EndsWith(".wav", r.Path, StringComparison.OrdinalIgnoreCase);
                fallback = r.Path;
                Assert.True(File.Exists(r.Path));
                using var read = new WaveFileReader(r.Path);
                Assert.Equal(22050, read.WaveFormat.SampleRate);
            }
        }
        finally
        {
            File.Delete(src);
            if (File.Exists(outPath)) File.Delete(outPath);
            if (fallback is not null && File.Exists(fallback)) File.Delete(fallback);
        }
    }

    [Fact]
    public void Stereo_can_be_requested_on_a_mono_source()
    {
        var src = NewWav(22050, 1, 120);
        var outPath = NewOut();
        try
        {
            var r = SoundOptimizeCommand.ExecutePath(src, 22050, channels: 2, bitrateKbps: null, outPath);

            Assert.Equal(1, r.ChannelsBefore);
            Assert.Equal(2, r.ChannelsAfter);

            using var read = new WaveFileReader(outPath);
            Assert.Equal(2, read.WaveFormat.Channels);
            Assert.Equal(22050, read.WaveFormat.SampleRate);
        }
        finally
        {
            File.Delete(src);
            if (File.Exists(outPath)) File.Delete(outPath);
        }
    }

    [Fact]
    public void An_out_of_range_mp3_bitrate_is_rejected()
    {
        var src = NewWav(22050, 1, 50);
        var outPath = Path.Combine(Path.GetTempPath(), $"wc3sound-{Guid.NewGuid():N}.mp3");
        try
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                SoundOptimizeCommand.ExecutePath(src, 22050, channels: null, bitrateKbps: 900, outPath));
        }
        finally
        {
            File.Delete(src);
            if (File.Exists(outPath)) File.Delete(outPath);
        }
    }

    [Theory]
    [InlineData("source", null)]
    [InlineData("", null)]
    [InlineData("keep", null)]
    [InlineData("mono", 1)]
    [InlineData("Stereo", 2)]
    [InlineData("2", 2)]
    public void Channel_tokens_resolve_to_a_count(string token, int? expected)
        => Assert.Equal(expected, SoundOptimizeCommand.ParseChannels(token));

    [Fact]
    public void An_unknown_channel_token_is_rejected()
        => Assert.Throws<ArgumentException>(() => SoundOptimizeCommand.ParseChannels("quad"));
}
