// tests/Wc3.Tests/SoundCommandTests.cs
using Wc3.Commands;
using Wc3.Model;
using War3Net.Build.Audio;
using War3Net.Build.Extensions;
using Xunit;

namespace Wc3.Tests;

/// <summary>Hermetic coverage of <see cref="SoundCommand"/>: list/add/set/remove over a
/// synthetic war3map.w3s, full Save→Load round-trips through the pinned MapSounds writer,
/// and the pure <see cref="SoundCommand.ApplyField"/> mapping.</summary>
public class SoundCommandTests
{
    // The w3s format version is discovered rather than hard-coded (the enum's members are
    // opaque in the probe dump); newest value = latest catalog format.
    private static readonly MapSoundsFormatVersion LatestVersion =
        (MapSoundsFormatVersion)Enum.GetValues<MapSoundsFormatVersion>().Cast<int>().Max();

    private static Sound Snd(string name, string file = "war3mapImported\\x.wav", int volume = 127) => new()
    {
        Name = name,
        FilePath = file,
        // Every string member must be non-null or the War3Net writer NREs on serialize.
        EaxSetting = string.Empty, SoundName = string.Empty,
        Unk2 = string.Empty, Unk5 = string.Empty, Unk6 = string.Empty, Unk7 = string.Empty,
        FacialAnimationLabel = string.Empty, FacialAnimationGroupLabel = string.Empty,
        FacialAnimationSetFilepath = string.Empty,
        Volume = volume,
    };

    private static MapSounds NewSounds(params Sound[] snds)
    {
        var m = new MapSounds(LatestVersion);
        foreach (var s in snds) m.Sounds.Add(s);
        return m;
    }

    private static byte[] SerializeSounds(MapSounds m)
    {
        using var ms = new MemoryStream();
        using (var w = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
            w.Write(m);
        return ms.ToArray();
    }

    private static byte[] SoundsBytes(params Sound[] snds) => SerializeSounds(NewSounds(snds));

    private static MapDocument Doc(params Sound[] snds) =>
        MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3s"] = SoundsBytes(snds),
        }));

    private static MapDocument EmptyMap() =>
        MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>()));

    [Fact]
    public void List_reads_sounds_in_order()
    {
        var doc = Doc(Snd("gg_snd_a", volume: 100), Snd("gg_snd_b", volume: 50));
        var snds = SoundCommand.List(doc);

        Assert.Equal(2, snds.Count);
        Assert.Equal("gg_snd_a", snds[0].Name);
        Assert.Equal(100, snds[0].Volume);
        Assert.Equal("gg_snd_b", snds[1].Name);
    }

    [Fact]
    public void Unchanged_reserialize_is_byte_identical()
    {
        var original = SoundsBytes(Snd("gg_snd_a"), Snd("gg_snd_b"));
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.w3s"] = original,
        }));

        var model = (MapSounds)doc.GetFile("war3map.w3s")!.Model!;
        Assert.Equal(original, SerializeSounds(model));
    }

    [Fact]
    public void Add_appends_sound_and_round_trips()
    {
        var doc = Doc(Snd("gg_snd_a"));
        var result = SoundCommand.Add(doc, "gg_snd_b", "war3mapImported\\b.wav");

        Assert.True(result.Ok, result.Message);
        Assert.Equal(2, result.Count);

        // AddOrReplaceModelFile marks the entry dirty, so SaveToBytes re-serializes the model
        // through the pinned MapSounds writer; reload and confirm the new sound survived.
        var reloaded = MapDocument.Load(doc.SaveToBytes());
        var b = SoundCommand.List(reloaded).Single(s => s.Name == "gg_snd_b");
        Assert.Equal("war3mapImported\\b.wav", b.FilePath);
    }

    [Fact]
    public void Add_rejects_blank_name()
    {
        var doc = Doc(Snd("gg_snd_a"));
        Assert.False(SoundCommand.Add(doc, "  ", "x.wav").Ok);
    }

    [Fact]
    public void Add_rejects_duplicate_name_case_insensitively()
    {
        var doc = Doc(Snd("gg_snd_a"));
        var r = SoundCommand.Add(doc, "GG_SND_A", "x.wav");

        Assert.False(r.Ok);
        Assert.Contains("already exists", r.Message);
    }

    [Fact]
    public void Add_into_map_without_catalog_creates_one()
    {
        var doc = EmptyMap();
        var r = SoundCommand.Add(doc, "gg_snd_a", "x.wav");
        Assert.True(r.Ok, r.Message);

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        Assert.Single(SoundCommand.List(reloaded));
    }

    [Fact]
    public void Set_changes_field_and_round_trips()
    {
        var doc = Doc(Snd("gg_snd_a", volume: 100));
        var r = SoundCommand.Set(doc, "gg_snd_a", "Volume", "42");
        Assert.True(r.Ok, r.Message);

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        Assert.Equal(42, SoundCommand.List(reloaded).Single().Volume);
    }

    [Fact]
    public void Set_missing_sound_errors()
    {
        var doc = Doc(Snd("gg_snd_a"));
        Assert.False(SoundCommand.Set(doc, "nope", "Volume", "1").Ok);
    }

    [Fact]
    public void Set_unknown_field_errors()
    {
        var doc = Doc(Snd("gg_snd_a"));
        var r = SoundCommand.Set(doc, "gg_snd_a", "Bogus", "1");

        Assert.False(r.Ok);
        Assert.Contains("Unknown sound field", r.Message);
    }

    [Fact]
    public void Set_rejects_rename_collision()
    {
        var doc = Doc(Snd("gg_snd_a"), Snd("gg_snd_b"));
        var r = SoundCommand.Set(doc, "gg_snd_a", "Name", "GG_SND_B");

        Assert.False(r.Ok);
        Assert.Contains("already exists", r.Message);
    }

    [Fact]
    public void Remove_deletes_sound_and_round_trips()
    {
        var doc = Doc(Snd("gg_snd_a"), Snd("gg_snd_b"));
        Assert.True(SoundCommand.Remove(doc, "gg_snd_a").Ok);

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        var names = SoundCommand.List(reloaded).Select(s => s.Name).ToList();
        Assert.DoesNotContain("gg_snd_a", names);
        Assert.Contains("gg_snd_b", names);
    }

    [Fact]
    public void Remove_missing_errors()
    {
        var doc = Doc(Snd("gg_snd_a"));
        Assert.False(SoundCommand.Remove(doc, "nope").Ok);
    }

    [Fact]
    public void List_returns_empty_when_catalog_absent()
    {
        // A map with no war3map.w3s just has no sounds; listing must not throw (a fresh map
        // or one that never defined a sound is a normal, editable state - Add creates the file).
        var doc = EmptyMap();
        Assert.Empty(SoundCommand.List(doc));
    }

    [Fact]
    public void ImportedAudioFiles_lists_only_audio_assets_sorted()
    {
        // A map can be full of imported audio yet define no sounds - this surfaces those files
        // (the World Editor's Import Manager) so they can be turned into definitions.
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [@"Voices\Reimu\Reimu_Book.mp3"] = new byte[] { 1 },
            [@"Units\Human\Attack1.wav"] = new byte[] { 2 },
            [@"war3map.j"] = new byte[] { 3 },              // not audio
            [@"Textures\foo.blp"] = new byte[] { 4 },        // not audio
        }));

        var audio = SoundCommand.ImportedAudioFiles(doc);
        Assert.Equal(new[] { @"Units\Human\Attack1.wav", @"Voices\Reimu\Reimu_Book.mp3" }, audio);
    }

    [Theory]
    [InlineData(@"Voices\Reimu\Reimu_Book.mp3", "gg_snd_Reimu_Book")]
    [InlineData(@"a b-c.wav", "gg_snd_a_b_c")]
    [InlineData("plain.ogg", "gg_snd_plain")]
    public void SoundNameForFile_derives_a_clean_label(string path, string expected)
    {
        Assert.Equal(expected, SoundCommand.SoundNameForFile(path));
    }

    // ---- pure ApplyField mapping ----

    [Fact]
    public void ApplyField_sets_string_routes()
    {
        var s = Snd("gg_snd_a");
        Assert.True(SoundCommand.ApplyField(s, "File", "war3mapImported\\y.wav", out _));
        Assert.Equal("war3mapImported\\y.wav", s.FilePath);
        Assert.True(SoundCommand.ApplyField(s, "Eax", "combat", out _));
        Assert.Equal("combat", s.EaxSetting);
    }

    [Fact]
    public void ApplyField_parses_numbers_and_enum_codes()
    {
        var s = Snd("gg_snd_a");
        Assert.True(SoundCommand.ApplyField(s, "Volume", "77", out _));
        Assert.Equal(77, s.Volume);
        Assert.True(SoundCommand.ApplyField(s, "Pitch", "1.5", out _));
        Assert.Equal(1.5f, s.Pitch);
        Assert.True(SoundCommand.ApplyField(s, "Channel", "1", out _));
        Assert.Equal(1, (int)s.Channel);
    }

    [Fact]
    public void ApplyField_accepts_enum_member_name()
    {
        var s = Snd("gg_snd_a");
        var name = Enum.GetNames<SoundChannel>()[0];
        Assert.True(SoundCommand.ApplyField(s, "Channel", name, out var err), err);
    }

    [Fact]
    public void ApplyField_rejects_bad_number()
    {
        var s = Snd("gg_snd_a");
        Assert.False(SoundCommand.ApplyField(s, "Volume", "abc", out var err));
        Assert.Contains("not a valid integer", err);
    }
}
