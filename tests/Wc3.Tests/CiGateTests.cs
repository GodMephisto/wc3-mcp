// tests/Wc3.Tests/CiGateTests.cs
// Hermetic tests for the ci gate commands. Every one drives a synthetic map or a temporary schema
// folder, so nothing here needs a Warcraft III install or a schemas corpus on disk.
using System.Text;
using War3Net.Build.Info;
using Wc3.Commands;

namespace Wc3.Tests;

public sealed class CiGateTests : IDisposable
{
    private readonly string _dir = Path.Combine(
        Path.GetTempPath(), "wc3ctl-ci-gate", Guid.NewGuid().ToString("N"));

    public CiGateTests() => Directory.CreateDirectory(_dir);

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch (IOException) { }
    }

    private static readonly byte[] Blob = { 1, 2, 3, 4 };

    /// <summary>Write a synthetic map from the given entries and return its path.</summary>
    private string Map(params (string Name, byte[] Bytes)[] entries)
    {
        var files = new Dictionary<string, byte[]>(StringComparer.Ordinal);
        foreach (var (name, bytes) in entries) files[name] = bytes;
        var path = Path.Combine(_dir, "map-" + Guid.NewGuid().ToString("N") + ".w3x");
        File.WriteAllBytes(path, SyntheticMap.Build(files));
        return path;
    }

    /// <summary>A map that plain-validates: the required data files plus a script payload.</summary>
    private string ValidMap() => Map(
        ("war3map.w3i", Blob), ("war3map.w3e", Blob), ("war3map.j", Blob));

    /// <summary>Two tiny schemas named old and new. new adds ExtraNative, which old lacks.</summary>
    private string TwoSchemas()
    {
        GamedataFixtures.Save(_dir, "TFT-v1.30.1.10211", natives: new[] { "NewTargetNative" });
        GamedataFixtures.Save(_dir, "TFT-v1.31.0.10420", natives: new[] { "NewTargetNative", "ExtraNative" });
        return _dir;
    }

    private string ScriptMap(string body) => Map(
        ("war3map.w3i", Blob), ("war3map.w3e", Blob),
        ("war3map.j", Encoding.ASCII.GetBytes(body)));

    // ---- ci lint ---------------------------------------------------------

    [Fact]
    public void Lint_gate_passes_when_only_warnings_are_reported()
    {
        // junk.xyz is an unloadable entry, a file-types warning, not an error.
        var map = Map(("war3map.j", Blob), ("junk.xyz", Blob));

        var r = CiLintCommand.Execute(map);

        Assert.True(r.Passed);
        Assert.Equal(0, r.ExitCode);
        var gate = Assert.Single(r.Gates);
        Assert.Equal("lint", gate.Name);
        Assert.True(gate.Passed);
        Assert.Empty(gate.Detail); // no error-level finding named
        Assert.True(r.Lint.Ok, "the lint verdict must agree with the gate pass");
    }

    [Fact]
    public void Lint_gate_with_a_baseline_fails_only_on_a_new_finding()
    {
        var map = Map(("war3map.j", Blob), ("junk.xyz", Blob));
        var baseline = Path.Combine(_dir, "baseline.json");
        LintBaselineCommand.Write(map, baseline);

        var unchanged = CiLintCommand.Execute(map, baseline);
        Assert.True(unchanged.Passed);
        Assert.Empty(unchanged.Added);
        Assert.Equal("lint-baseline", Assert.Single(unchanged.Gates).Name);

        // A second unloadable entry is a new file-types finding the baseline never held.
        var changed = Map(("war3map.j", Blob), ("junk.xyz", Blob), ("other.abc", Blob));
        var r = CiLintCommand.Execute(changed, baseline);

        Assert.False(r.Passed);
        Assert.Equal(1, r.ExitCode);
        Assert.Contains(r.Added, f => f.Check == "file-types");
        Assert.Contains("lint-baseline", r.Gates.Select(g => g.Name));
    }

    [Fact]
    public void Lint_gate_runs_compat_and_fails_on_an_error_finding()
    {
        TwoSchemas();
        var map = ScriptMap(
            "function InitTriggers takes nothing returns nothing\n"
            + "    call ExtraNative()\n"
            + "endfunction\n");

        // Assert the compat gate itself, not the overall verdict. The synthetic native is unknown
        // to the real common.j, so when a WC3 install makes pjass run the lint gate also fails,
        // and that outcome belongs to pjass rather than to this test.
        var failing = CiLintCommand.Execute(map, null, "TFT-v1.30.1.10211", _dir);
        Assert.NotNull(failing.Compat);
        var compatGate = failing.Gates.Single(g => g.Name == "compat");
        Assert.False(compatGate.Passed);
        Assert.Contains(compatGate.Detail, d => d.Contains("ExtraNative"));

        var passing = CiLintCommand.Execute(
            ScriptMap("function InitTriggers takes nothing returns nothing\n"
                      + "    call NewTargetNative()\n"
                      + "endfunction\n"),
            null, "TFT-v1.30.1.10211", _dir);
        Assert.True(passing.Gates.Single(g => g.Name == "compat").Passed);
    }

    // ---- ci validate -----------------------------------------------------

    [Fact]
    public void Validate_gate_passes_on_a_map_with_no_errors()
    {
        var r = CiValidateCommand.Execute(ValidMap());

        Assert.True(r.Passed);
        Assert.Equal(0, r.ExitCode);
        Assert.Equal("validate", Assert.Single(r.Gates).Name);
        Assert.Null(r.Strict);
    }

    [Fact]
    public void Validate_gate_fails_when_required_files_are_missing()
    {
        var r = CiValidateCommand.Execute(Map(("war3map.j", Blob)));

        Assert.False(r.Passed);
        Assert.Equal(1, r.ExitCode);
        Assert.False(r.Gates.Single().Passed);
        Assert.Contains(r.Gates.Single().Detail, d => d.Contains("war3map.w3i"));
    }

    [Fact]
    public void Validate_strict_gate_fails_on_a_script_with_no_entry_point()
    {
        // The script payload declares no function at all. Plain validate passes it, the strict
        // script-entry-point check does not.
        var map = ValidMap();
        var plain = CiValidateCommand.Execute(map, strict: false);
        Assert.True(plain.Passed);

        var strict = CiValidateCommand.Execute(map, strict: true);
        Assert.False(strict.Passed);
        Assert.Equal(1, strict.ExitCode);
        Assert.NotNull(strict.Strict);
        Assert.Equal("validate-strict", strict.Gates.Single().Name);
        Assert.Contains(strict.Gates.Single().Detail,
            d => d.Contains("script-entry-point") || d.Contains("entry point"));
    }

    // ---- ci compat -------------------------------------------------------

    [Fact]
    public void Compat_gate_fails_on_an_error_level_finding_and_passes_without_one()
    {
        TwoSchemas();
        var failing = CiCompatCommand.Execute(
            ScriptMap("function f takes nothing returns nothing\n"
                      + "    call ExtraNative()\n"
                      + "endfunction\n"),
            "TFT-v1.30.1.10211", _dir);
        Assert.False(failing.Passed);
        Assert.Equal(1, failing.ExitCode);
        Assert.Contains(failing.Gates.Single().Detail, d => d.Contains("ExtraNative"));

        var passing = CiCompatCommand.Execute(
            ScriptMap("function f takes nothing returns nothing\n"
                      + "    call NewTargetNative()\n"
                      + "endfunction\n"),
            "TFT-v1.30.1.10211", _dir);
        Assert.True(passing.Passed);
        Assert.Contains("TFT-v1.30.1.10211", passing.Gates.Single().Summary);
    }

    // ---- ci schema -------------------------------------------------------

    [Fact]
    public void Schema_gate_passes_when_every_build_parses_and_the_floor_is_met()
    {
        GamedataFixtures.Save(_dir, "TFT-v1.30.1.10211", "units/unitmetadata.slk",
            new[] { GamedataFixtures.Field("Cool1", "int", "0", "100", true) });
        GamedataFixtures.Save(_dir, "TFT-v1.31.1.12173", natives: new[] { "N" });

        var r = CiSchemaCommand.Execute(_dir, minReal: 1);

        Assert.True(r.Passed);
        Assert.Equal(0, r.ExitCode);
        Assert.Equal(2, r.Builds.Count);
        Assert.All(r.Builds, b => Assert.True(b.Parsed));
        Assert.Equal(1, r.Coverage.Totals.Real);
        Assert.Equal(2, r.Gates.Count);
    }

    [Fact]
    public void Schema_gate_fails_when_real_object_data_drops_below_the_floor()
    {
        GamedataFixtures.Save(_dir, "TFT-v1.30.1.10211", "units/unitmetadata.slk",
            new[] { GamedataFixtures.Field("Cool1", "int", "0", "100", true) });

        var r = CiSchemaCommand.Execute(_dir, minReal: 2);

        Assert.False(r.Passed);
        Assert.Equal(1, r.ExitCode);
        var realGate = r.Gates.Single(g => g.Name == "schema-real");
        Assert.False(realGate.Passed);
        Assert.Contains("minimum 2", realGate.Summary);
    }

    [Fact]
    public void Schema_gate_names_a_build_that_no_longer_parses()
    {
        GamedataFixtures.Save(_dir, "TFT-v1.30.1.10211", natives: new[] { "N" });
        var broken = Path.Combine(_dir, "Broken-v1.0");
        Directory.CreateDirectory(broken);
        File.WriteAllText(Path.Combine(broken, "manifest.json"), "{ this is not valid json");

        var r = CiSchemaCommand.Execute(_dir, minReal: 0);

        Assert.False(r.Passed);
        Assert.Equal(1, r.ExitCode);
        var parseGate = r.Gates.Single(g => g.Name == "schema-parse");
        Assert.False(parseGate.Passed);
        Assert.Contains(parseGate.Detail, d => d.Contains("Broken-v1.0"));
        Assert.Contains(r.Builds, b => !b.Parsed);
    }

    [Fact]
    public void Schema_gate_fails_when_no_schema_folder_is_present()
    {
        var r = CiSchemaCommand.Execute(_dir, minReal: 0);
        Assert.False(r.Passed);
        Assert.Contains("no schema folder found", r.Gates.Single(g => g.Name == "schema-parse").Summary);
    }

    // ---- ci all ----------------------------------------------------------

    [Fact]
    public void All_gate_runs_lint_and_validate_and_skips_compat_without_a_target()
    {
        var r = CiAllCommand.Execute(ValidMap());

        Assert.True(r.Passed);
        Assert.Equal(0, r.ExitCode);
        Assert.Equal(new[] { "lint", "validate", "compat" }, r.Gates.Select(g => g.Name));
        Assert.Null(r.Compat);
        Assert.Contains("skipped", r.Gates.Single(g => g.Name == "compat").Summary);
    }

    [Fact]
    public void All_gate_fails_when_any_gate_fails()
    {
        // Lint passes here (warnings only), validate fails (missing required files).
        var r = CiAllCommand.Execute(Map(("war3map.j", Blob), ("junk.xyz", Blob)));

        Assert.False(r.Passed);
        Assert.Equal(1, r.ExitCode);
        Assert.True(r.Gates.Single(g => g.Name == "lint").Passed);
        Assert.False(r.Gates.Single(g => g.Name == "validate").Passed);
    }

    [Fact]
    public void All_gate_includes_compat_when_a_target_is_given()
    {
        TwoSchemas();
        var map = ScriptMap("function f takes nothing returns nothing\n"
                            + "    call ExtraNative()\n"
                            + "endfunction\n");

        var r = CiAllCommand.Execute(map, "TFT-v1.30.1.10211", _dir);

        Assert.False(r.Passed);
        Assert.NotNull(r.Compat);
        Assert.Contains(r.Gates, g => g.Name == "compat" && !g.Passed);
    }
}
