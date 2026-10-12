// tests/Wc3.Tests/LintBaselineTests.cs
// Hermetic tests for LintBaselineCommand, the write mode and the CI gate. The finding used is
// file-types, which fires on a map carrying an entry the game does not load and needs no install.
using System.Text;
using Wc3.Commands;

namespace Wc3.Tests;

public sealed class LintBaselineTests : IDisposable
{
    private readonly string _dir = Path.Combine(
        Path.GetTempPath(), "wc3ctl-lint-baseline", Guid.NewGuid().ToString("N"));

    public LintBaselineTests() => Directory.CreateDirectory(_dir);

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch (IOException) { }
    }

    /// <summary>A map whose script has an entry point, plus any odd-extension entries named.</summary>
    private string Map(params string[] oddFiles)
    {
        var files = new Dictionary<string, byte[]>
        {
            ["war3map.j"] = Encoding.ASCII.GetBytes(
                "function main takes nothing returns nothing\nendfunction\n"),
        };
        foreach (var f in oddFiles) files[f] = new byte[] { 1, 2, 3 };

        var path = Path.Combine(_dir, "map-" + Guid.NewGuid().ToString("N") + ".w3x");
        File.WriteAllBytes(path, SyntheticMap.Build(files));
        return path;
    }

    [Fact]
    public void Write_records_a_baseline_and_gate_accepts_the_same_map()
    {
        var map = Map("junk.xyz");
        var baselinePath = Path.Combine(_dir, "baseline.json");

        var write = LintBaselineCommand.Write(map, baselinePath);

        Assert.True(write.Findings >= 1, "a map with an unloadable entry should record a finding");
        Assert.True(File.Exists(baselinePath));

        var gate = LintBaselineCommand.Gate(map, baselinePath);
        Assert.True(gate.Ok);
        Assert.Empty(gate.Added);
    }

    [Fact]
    public void Gate_fails_only_on_a_new_finding()
    {
        var map = Map("junk.xyz");
        var baselinePath = Path.Combine(_dir, "baseline.json");
        LintBaselineCommand.Write(map, baselinePath);

        // One more unloadable entry is a new file-types finding the baseline never held.
        var changed = Map("junk.xyz", "other.abc");
        var gate = LintBaselineCommand.Gate(changed, baselinePath);

        Assert.False(gate.Ok);
        Assert.NotEmpty(gate.Added);
        Assert.Contains(gate.Added, f => f.Check == "file-types");
    }

    [Fact]
    public void Gate_reports_a_resolved_finding_and_stays_ok()
    {
        var map = Map("junk.xyz");
        var baselinePath = Path.Combine(_dir, "baseline.json");
        LintBaselineCommand.Write(map, baselinePath);

        // The odd entry is gone, so the baseline finding is resolved and nothing is added.
        var clean = Map();
        var gate = LintBaselineCommand.Gate(clean, baselinePath);

        Assert.True(gate.Ok);
        Assert.Empty(gate.Added);
        Assert.Contains(gate.Resolved, f => f.Check == "file-types");
    }

    [Fact]
    public void Gate_throws_when_the_baseline_is_missing()
    {
        var map = Map();
        Assert.Throws<FileNotFoundException>(() =>
            LintBaselineCommand.Gate(map, Path.Combine(_dir, "does-not-exist.json")));
    }
}
