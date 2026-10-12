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
