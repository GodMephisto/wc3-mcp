// tests/Wc3.Tests/MapDoctorCommandTests.cs
using Wc3.Commands;

namespace Wc3.Tests;

/// <summary>
/// The wrapper around Devo's Map Doctor. These cover the guards that decide whether an external
/// program runs on the user's maps at all, so none of them needs the doctor installed. The live
/// calls are in the Corpus test at the end.
/// </summary>
public class MapDoctorCommandTests : IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), "wc3ctl-doctor-tests", Guid.NewGuid().ToString("N"));

    public MapDoctorCommandTests() => Directory.CreateDirectory(_dir);
    public void Dispose() { try { Directory.Delete(_dir, true); } catch { } }

    private string FakeExe(string content)
    {
        var d = Directory.CreateDirectory(Path.Combine(_dir, "doctor", "9.9.9")).FullName;
        var exe = Path.Combine(d, MapDoctorCommand.ExeName);
        File.WriteAllText(exe, content);
        return exe;
    }

    [Fact]
    public void An_unpinned_build_is_refused()
    {
        var exe = FakeExe("not the doctor");
        var ex = Assert.Throws<InvalidOperationException>(() =>
            MapDoctorCommand.Locate(k => k == "WC3_DOCTOR" ? exe : null));
        Assert.Contains("not a doctor build", ex.Message);
    }

    [Fact]
    public void An_unpinned_build_runs_only_when_the_override_is_set()
    {
        var exe = FakeExe("not the doctor");
        var (found, version, _) = MapDoctorCommand.Locate(
            k => k switch { "WC3_DOCTOR" => exe, "WC3_DOCTOR_UNPINNED" => "1", _ => null });
        Assert.Equal(exe, found);
        Assert.Equal("unpinned", version);
    }

    [Fact]
    public void The_default_folder_is_searched_when_WC3_DOCTOR_is_unset()
    {
        var local = Path.Combine(_dir, "local");
        var folder = Directory.CreateDirectory(Path.Combine(local, "wc3ctl", "doctor", "1.0")).FullName;
        File.WriteAllText(Path.Combine(folder, MapDoctorCommand.ExeName), "x");
        var (found, _, _) = MapDoctorCommand.Locate(k => k == "WC3_DOCTOR_UNPINNED" ? "1" : null, local);
        Assert.Equal(Path.Combine(folder, MapDoctorCommand.ExeName), found);
    }

    [Fact]
    public void A_missing_doctor_says_where_to_put_it()
    {
        var ex = Assert.Throws<FileNotFoundException>(() =>
            MapDoctorCommand.Locate(_ => null, Path.Combine(_dir, "empty")));
        Assert.Contains("devos-map-doctor/releases", ex.Message);
    }

    [Theory]
    [InlineData("listfile", "--listfile")]
    [InlineData("--portraits", "--portraits")]
    [InlineData("no-gui-triggers", "--no-gui-triggers")]
    public void Allowed_fix_options_pass_through(string given, string passed) =>
        Assert.Equal(new[] { passed }, MapDoctorCommand.FixArgs(new[] { given }).ToArray());

    [Theory]
    [InlineData("translation=C:/secrets.txt")]
    [InlineData("editor-version=1.27")]
    [InlineData("anything")]
    public void Other_fix_options_are_refused(string given) =>
        Assert.Throws<ArgumentException>(() => MapDoctorCommand.FixArgs(new[] { given }).ToArray());

    [Fact]
    public void Only_read_only_reports_run_through_inspect()
    {
        var map = Path.Combine(_dir, "m.w3x");
        File.WriteAllText(map, "");
        foreach (var verb in new[] { "fix", "cheat", "qol", "port", "editor" })
            Assert.Throws<ArgumentException>(() => MapDoctorCommand.ReadOnly(verb, map));
    }

    [Fact]
    public void Fix_never_writes_over_the_original_or_an_existing_output()
    {
        var map = Path.Combine(_dir, "m.w3x");
        File.WriteAllText(map, "original");
        Assert.Throws<ArgumentException>(() => MapDoctorCommand.Fix(map, output: map));

        var taken = Path.Combine(_dir, "m_doctor.w3x");
        File.WriteAllText(taken, "someone's earlier fix");
        Assert.Throws<IOException>(() => MapDoctorCommand.Fix(map));

        Assert.Equal("original", File.ReadAllText(map));
        Assert.Equal("someone's earlier fix", File.ReadAllText(taken));
    }

    [Fact]
    public void A_malformed_game_version_is_refused()
    {
        var map = Path.Combine(_dir, "m.w3x");
        File.WriteAllText(map, "");
        Assert.Throws<ArgumentException>(() => MapDoctorCommand.Check(map, "1.27 --qol"));
    }

    // ---- the answer cache, keyed by the map's bytes rather than its path ----

    /// <summary>One lowercase key for a read-only command, using test identities unless given.</summary>
    private static string KeyFor(string map, string command = "check", string tool = "0.4.13+test",
        string doctor = "1.7.6:abc", params string[] extra)
    {
        var args = new List<string> { command, map };
        args.AddRange(extra);
        var key = MapDoctorCommand.CacheKey(command, args, tool, doctor, MapDoctorCommand.FileDigest);
        Assert.NotNull(key);
        return key;
    }

    private static MapDoctorResult AnAnswer(string command = "check") =>
        new(command, 0, null, null, "1.7.6 sha256:abc", null);

    private string WriteMap(string bytes)
    {
        var map = Path.Combine(_dir, "cache-me.w3x");
        File.WriteAllText(map, bytes);
        return map;
    }

    [Fact]
    public void An_unchanged_map_is_a_hit()
    {
        var map = WriteMap("aaaa");
        var first = KeyFor(map);
        var second = KeyFor(map);
        Assert.Equal(first, second);

        var cache = new MapDoctorCommand.DoctorResultCache();
        var answer = AnAnswer();
        cache.Put(first, answer);
        Assert.Same(answer, cache.Get(second));
    }

    [Fact]
    public void One_changed_byte_at_the_same_path_is_a_miss()
    {
        var map = WriteMap("aaaa");
        var stamp = File.GetLastWriteTimeUtc(map);
        var first = KeyFor(map);

        // Same length, same timestamp, same path, different bytes. A key made of the time and the
        // size cannot see this, which is the defect this test pins.
        File.WriteAllText(map, "aaab");
        File.SetLastWriteTimeUtc(map, stamp);
        Assert.Equal(stamp, File.GetLastWriteTimeUtc(map));
        Assert.Equal(4, new FileInfo(map).Length);

        var second = KeyFor(map);
        Assert.NotEqual(first, second);

        var cache = new MapDoctorCommand.DoctorResultCache();
        cache.Put(first, AnAnswer());
        Assert.Null(cache.Get(second));
    }

    [Fact]
    public void A_version_bump_is_a_miss()
    {
        var map = WriteMap("aaaa");
        var old = KeyFor(map, tool: "0.4.13+aaaaaaa");
        var bumped = KeyFor(map, tool: "0.4.14+bbbbbbb");
        Assert.NotEqual(old, bumped);

        var cache = new MapDoctorCommand.DoctorResultCache();
        cache.Put(old, AnAnswer());
        Assert.Null(cache.Get(bumped));
    }

    [Fact]
    public void A_different_doctor_build_is_a_miss()
    {
        var map = WriteMap("aaaa");
        var cache = new MapDoctorCommand.DoctorResultCache();
        var first = KeyFor(map, doctor: "1.7.6:522f9bda");
        cache.Put(first, AnAnswer());
        Assert.Null(cache.Get(KeyFor(map, doctor: "1.7.7:00000000")));
    }

    [Fact]
    public void An_option_that_changes_the_verdict_is_a_miss()
    {
        var map = WriteMap("aaaa");
        var plain = KeyFor(map);
        var for127 = KeyFor(map, extra: "--game=1.27");
        var for128 = KeyFor(map, extra: "--game=1.28");
        Assert.NotEqual(plain, for127);
        Assert.NotEqual(for127, for128);
    }

    [Fact]
    public void A_command_that_writes_is_never_cached()
    {
        var map = WriteMap("aaaa");
        Assert.Null(MapDoctorCommand.CacheKey("fix",
            new List<string> { "fix", map }, "0.4.13", "1.7.6:abc", MapDoctorCommand.FileDigest));
    }

    [Fact]
    public void A_folder_is_handled_without_being_hashed_as_a_file()
    {
        var folder = Directory.CreateDirectory(Path.Combine(_dir, "maps")).FullName;
        File.WriteAllText(Path.Combine(folder, "inside.w3x"), "x");

        // No throw, and no cache entry keyed by a folder's name alone.
        Assert.Null(MapDoctorCommand.FileDigest(folder));
        Assert.Null(MapDoctorCommand.CacheKey("check",
            new List<string> { "check", folder }, "0.4.13", "1.7.6:abc", MapDoctorCommand.FileDigest));
    }

    [Fact]
    public void A_path_named_twice_in_one_call_is_read_once()
    {
        // A hit must not cost more than one pass over the map, so a repeated path in the arguments
        // is digested once. Compare takes two maps but is not a cached command, so this is pinned
        // on check.
        var map = WriteMap("aaaa");
        var reads = 0;
        string? Counting(string path) { reads++; return MapDoctorCommand.FileDigest(path); }

        var key = MapDoctorCommand.CacheKey("check",
            new List<string> { "check", map, map }, "0.4.13", "1.7.6:abc", Counting);
        Assert.NotNull(key);
        Assert.Equal(1, reads);
    }

    [Fact]
    public void The_cache_is_bounded()
    {
        var cache = new MapDoctorCommand.DoctorResultCache(limit: 4);
        for (var i = 0; i < 40; i++) cache.Put("key-" + i, AnAnswer());
        Assert.Equal(4, cache.Count);
        Assert.Null(cache.Get("key-0"));
        Assert.NotNull(cache.Get("key-39"));
    }

    [Fact]
    public void This_build_names_itself_in_the_key()
    {
        var identity = MapDoctorCommand.ToolIdentity();
        Assert.False(string.IsNullOrWhiteSpace(identity));
        Assert.Equal(identity, MapDoctorCommand.ToolIdentity());
    }

    [Fact]
    [Trait("Category", "Corpus")]
    public void Check_reports_the_half_written_button_positions_in_GGGA_0_07c()
    {
        var map = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Warcraft III", "Maps", "Download", "GGGA_V0.07c_slk.w3x");
        if (!File.Exists(map)) return;
        var r = MapDoctorCommand.Check(map);
        Assert.Equal(0, r.ExitCode);
        var codes = r.Result!.Value.GetProperty("items").EnumerateArray()
            .Select(i => i.GetProperty("code").GetString()).ToList();
        Assert.Contains("slk_button_position", codes);
    }
}
