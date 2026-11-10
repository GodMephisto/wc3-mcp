// tests/Wc3.Tests/SoundImportCommandTests.cs
using Wc3.Commands;
using Wc3.Model;
using Xunit;

namespace Wc3.Tests;

/// <summary>
/// Hermetic coverage of <see cref="SoundImportCommand"/>. Event JSON becomes war3map.w3s catalog
/// entries, clip resolution against a clip folder, and the generated JASS dispatcher. The map is
/// synthetic and the clips are empty placeholder files, so nothing here needs a game install.
/// </summary>
public class SoundImportCommandTests
{
    /// <summary>A throwaway clip folder, deleted even when a fact fails.</summary>
    private sealed class Clips : IDisposable
    {
        public string Path { get; }

        public Clips(params string[] names)
        {
            Path = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(), "wc3ctl-soundimport-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path);
            foreach (var name in names)
            {
                var full = System.IO.Path.Combine(Path, name.Replace('\\', System.IO.Path.DirectorySeparatorChar));
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(full)!);
                File.WriteAllBytes(full, new byte[] { 0 });
            }
        }

        public void Dispose()
        {
            try { Directory.Delete(Path, recursive: true); } catch { /* best effort */ }
        }
    }

    private static MapDocument EmptyMap() =>
        MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>()));

    /// <summary>The function body of <paramref name="name"/> in <paramref name="jass"/>, or empty
    /// when no such function exists.</summary>
    private static string FunctionText(string jass, string name)
    {
        var lines = JassLines.Split(jass);
        var fn = JassFunctionIndex.Parse(jass).FirstOrDefault(f => f.Name == name);
        if (fn is null) return string.Empty;
        return string.Join("\n", lines[(fn.StartLine - 1)..fn.EndLine]);
    }

    [Fact]
    public void Import_writes_one_catalog_entry_per_clip_and_dispatches_each_event()
    {
        using var clips = new Clips("a.wav", "b.wav");
        var doc = EmptyMap();
        const string json = """
        {
          "event.one": { "clips": ["a.wav"], "cooldown": 0.06, "maxVoices": 3, "priority": 10 },
          "event.two": { "clips": ["b.wav"], "cooldown": 0.25, "maxVoices": 2, "priority": 5 }
        }
        """;

        var result = SoundImportCommand.Import(doc, json, clips.Path);

        Assert.True(result.Ok, result.Message);
        Assert.Equal(2, result.Events.Count);
        Assert.Equal(2, result.CatalogEntries.Count);
        Assert.Empty(result.MissingClips);
        Assert.Empty(result.Refused);

        // One entry per clip, pointing into war3mapImported, and it survives the real writer.
        var reloaded = MapDocument.Load(doc.SaveToBytes());
        var sounds = SoundCommand.List(reloaded);
        Assert.Equal(2, sounds.Count);
        Assert.Contains(sounds, s => s.Name == "gg_snd_a" && s.FilePath == @"war3mapImported\a.wav");
        Assert.Contains(sounds, s => s.Name == "gg_snd_b" && s.FilePath == @"war3mapImported\b.wav");

        Assert.Contains("function SND_Event_event_one_Play takes nothing returns boolean", result.Jass);
        Assert.Contains("function SND_Event_event_two_Play takes nothing returns boolean", result.Jass);
        Assert.Contains("function SND_EventPlay takes integer ev, real cooldown returns boolean", result.Jass);
        Assert.Contains("function SND_Init takes nothing returns nothing", result.Jass);

        // The path literal is escaped for JASS, a bare backslash is an invalid escape under pjass.
        Assert.Contains(@"CreateSound(""war3mapImported\\a.wav""", result.Jass);

        // The generated block is a legal JASS fragment, every global it assigns is declared.
        Assert.True(JassScriptCheck.IsCompilable(JassScriptCheck.Check(result.Jass)),
            string.Join("\n", JassScriptCheck.Check(result.Jass).Select(i => i.Message)));
    }

    [Fact]
    public void Missing_clip_is_reported_and_nothing_is_written_for_that_event()
    {
        using var clips = new Clips("good.wav");
        var doc = EmptyMap();
        const string json = """
        {
          "ok":  { "clips": ["good.wav"], "cooldown": 0.10, "maxVoices": 1, "priority": 0 },
          "bad": { "clips": ["good.wav", "gone.wav"], "cooldown": 0.10, "maxVoices": 1, "priority": 0 }
        }
        """;

        var result = SoundImportCommand.Import(doc, json, clips.Path);

        Assert.False(result.Ok);
        Assert.Equal(new[] { "gone.wav" }, result.MissingClips);
        var refused = Assert.Single(result.Refused);
        Assert.Equal("bad", refused.EventName);
        Assert.Contains("gone.wav", refused.Reason);

        // The refused event contributed no catalog entry and no function.
        var entry = Assert.Single(result.CatalogEntries);
        Assert.Equal("gg_snd_good", entry.SoundName);
        Assert.Single(result.Events);
        Assert.DoesNotContain("SND_Event_bad_Play", result.Jass);
        Assert.Contains("SND_Event_ok_Play", result.Jass);

        var reloaded = MapDocument.Load(doc.SaveToBytes());
        Assert.Equal(new[] { "gg_snd_good" }, SoundCommand.List(reloaded).Select(s => s.Name));
    }

    [Fact]
    public void Generated_jass_has_one_function_per_event_and_names_its_cooldown()
    {
        using var clips = new Clips("a.wav", "b.wav");
        var doc = EmptyMap();
        const string json = """
        {
          "fast": { "clips": ["a.wav"], "cooldown": 0.06, "maxVoices": 3, "priority": 10 },
          "slow": { "clips": ["b.wav"], "cooldown": 0.25, "maxVoices": 1, "priority": 2 }
        }
        """;

        var result = SoundImportCommand.Import(doc, json, clips.Path);
        Assert.True(result.Ok, result.Message);

        var dispatchers = JassFunctionIndex.Parse(result.Jass)
            .Where(f => f.Name.StartsWith("SND_Event_", StringComparison.Ordinal) && f.Name != "SND_EventPlay")
            .ToList();
        Assert.Equal(2, dispatchers.Count);

        string fast = FunctionText(result.Jass, "SND_Event_fast_Play");
        string slow = FunctionText(result.Jass, "SND_Event_slow_Play");
        Assert.False(string.IsNullOrEmpty(fast));
        Assert.False(string.IsNullOrEmpty(slow));

        Assert.Contains("0.06", fast);
        Assert.DoesNotContain("0.25", fast);
        Assert.Contains("0.25", slow);
        Assert.DoesNotContain("0.06", slow);

        // The per-event voice limit and priority are emitted into the shared init, one per event.
        Assert.Contains("set SND_EVENT_VOICES[0] = 3", result.Jass);
        Assert.Contains("set SND_EVENT_VOICES[1] = 1", result.Jass);
        Assert.Contains("set SND_EVENT_PRIORITY[0] = 10", result.Jass);
        Assert.Contains("set SND_EVENT_PRIORITY[1] = 2", result.Jass);
    }

    [Fact]
    public void Event_with_no_clips_is_refused()
    {
        using var clips = new Clips();
        var doc = EmptyMap();
        const string json = """
        { "silent": { "clips": [], "cooldown": 0.10, "maxVoices": 1, "priority": 0 } }
        """;

        var result = SoundImportCommand.Import(doc, json, clips.Path);

        Assert.False(result.Ok);
        var refused = Assert.Single(result.Refused);
        Assert.Equal("silent", refused.EventName);
        Assert.Contains("no clips", refused.Reason);
        Assert.Empty(result.Events);
        Assert.Empty(result.CatalogEntries);
        Assert.Equal(string.Empty, result.Jass);

        // Nothing was written, so the map never grew a sound catalog for it.
        Assert.Null(doc.GetFile("war3map.w3s"));
    }
}
