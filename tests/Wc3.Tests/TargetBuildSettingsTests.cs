// tests/Wc3.Tests/TargetBuildSettingsTests.cs
// Tests for TargetBuildSettings, covering precedence, bad name suggestions, and clear.
using Wc3.Commands;

namespace Wc3.Tests;

public class TargetBuildSettingsTests
{
    private static string TempSettingsDir()
    {
        var tempDir = Path.GetTempPath();
        var settingsDir = Path.Combine(tempDir, "wc3ctl-test-settings-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(settingsDir);
        return settingsDir;
    }

    private static string? FindSchemasDir()
    {
        var schemasDir = Environment.GetEnvironmentVariable("WC3_SCHEMAS_DIR");
        if (!string.IsNullOrWhiteSpace(schemasDir) && Directory.Exists(schemasDir))
            return schemasDir;
        var schemasPath = Path.Combine(AppContext.BaseDirectory, "data", "schemas");
        if (Directory.Exists(schemasPath))
            return schemasPath;
        return null;
    }

    [Fact]
    public void Precedence_option_over_environment_over_settings()
    {
        TargetBuildSettings.ResetSettingsDir();
        var settingsDir = TempSettingsDir();
        try
        {
            // Write a settings file.
            File.WriteAllText(
                Path.Combine(settingsDir, "settings.json"),
                "{\"targetBuild\": \"TFT-v1.31.0.00001\"}");

            Environment.SetEnvironmentVariable("WC3_TARGET_BUILD", "TFT-v1.30.2.10200");
            Environment.SetEnvironmentVariable("WC3CTL_SETTINGS_DIR", settingsDir);

            try
            {
                // Option overrides everything.
                var result = TargetBuildSettings.Get(option: "TFT-v1.30.1.10211");
                Assert.Equal("TFT-v1.30.1.10211", result.Build);
                Assert.Equal(TargetSource.Option, result.Source);

                // Without option, falls through to env var.
                result = TargetBuildSettings.Get();
                Assert.Equal("TFT-v1.30.2.10200", result.Build);
                Assert.Equal(TargetSource.Environment, result.Source);

                // Without option and env var, falls through to settings file.
                Environment.SetEnvironmentVariable("WC3_TARGET_BUILD", null);
                result = TargetBuildSettings.Get();
                Assert.Equal("TFT-v1.31.0.00001", result.Build);
                Assert.Equal(TargetSource.Settings, result.Source);
            }
            finally
            {
                Environment.SetEnvironmentVariable("WC3_TARGET_BUILD", null);
            }
        }
        finally
        {
            if (Directory.Exists(settingsDir))
                Directory.Delete(settingsDir, true);
        }
    }

    [Fact]
    public void Precedence_settings_over_default_skips_without_schemas()
    {
        TargetBuildSettings.ResetSettingsDir();
        var settingsDir = TempSettingsDir();
        try
        {
            File.WriteAllText(
                Path.Combine(settingsDir, "settings.json"),
                "{\"targetBuild\": \"TFT-v1.30.0.00001\"}");

            Environment.SetEnvironmentVariable("WC3CTL_SETTINGS_DIR", settingsDir);

            var schemasDir = FindSchemasDir();
            if (schemasDir is null)
            {
                // Default resolution requires schemas, skip test.
                return;
            }

            var result = TargetBuildSettings.Get(schemasDir: schemasDir);
            Assert.Equal("TFT-v1.30.0.00001", result.Build);
            Assert.Equal(TargetSource.Settings, result.Source);
        }
        finally
        {
            if (Directory.Exists(settingsDir))
                Directory.Delete(settingsDir, true);
        }
    }

    [Fact]
    public void Set_with_bad_name_throws_with_suggestions()
    {
        TargetBuildSettings.ResetSettingsDir();
        var settingsDir = TempSettingsDir();
        try
        {
            Environment.SetEnvironmentVariable("WC3CTL_SETTINGS_DIR", settingsDir);
            var schemasDir = FindSchemasDir();

            // Skip if no schemas available.
            if (schemasDir is null)
                return;

            var ex = Assert.Throws<FileNotFoundException>(() =>
                TargetBuildSettings.Set("bogus-build-name-that-does-not-exist", schemasDir));

            Assert.Contains("not found", ex.Message);
            // Close names should be suggested.
            Assert.Contains("Close names:", ex.Message);
        }
        finally
        {
            if (Directory.Exists(settingsDir))
                Directory.Delete(settingsDir, true);
        }
    }

    [Fact]
    public void Clear_removes_the_setting()
    {
        TargetBuildSettings.ResetSettingsDir();
        var settingsDir = TempSettingsDir();
        try
        {
            // Write a settings file.
            File.WriteAllText(
                Path.Combine(settingsDir, "settings.json"),
                "{\"targetBuild\": \"TFT-v1.30.1.10211\"}");

            Environment.SetEnvironmentVariable("WC3CTL_SETTINGS_DIR", settingsDir);

            // Verify the setting is there.
            var result = TargetBuildSettings.Get();
            Assert.Equal("TFT-v1.30.1.10211", result.Build);

            // Clear it.
            TargetBuildSettings.Clear();

            // Now Get should return default (or throw if no schemas).
            // The source should no longer be Settings.
            try
            {
                var cleared = TargetBuildSettings.Get();
                Assert.NotEqual(TargetSource.Settings, cleared.Source);
            }
            catch (DirectoryNotFoundException)
            {
                // No schemas available, clear succeeded (file was deleted).
                Assert.False(File.Exists(Path.Combine(settingsDir, "settings.json")));
            }
        }
        finally
        {
            if (Directory.Exists(settingsDir))
                Directory.Delete(settingsDir, true);
        }
    }

    [Fact]
    public void Watched_failing_by_swapping_branches()
    {
        TargetBuildSettings.ResetSettingsDir();
        // This test verifies the precedence chain by temporarily swapping two branches.
        // Start: option overrides environment variable.
        // Swap: remove the option, so environment variable is used instead.
        // Verify: the build string changes.
        // Restore: put the option back.
        var settingsDir = TempSettingsDir();
        try
        {
            File.WriteAllText(
                Path.Combine(settingsDir, "settings.json"),
                "{\"targetBuild\": \"TFT-v1.32.0.00001\"}");

            Environment.SetEnvironmentVariable("WC3_TARGET_BUILD", "TFT-v1.30.3.10205");
            Environment.SetEnvironmentVariable("WC3CTL_SETTINGS_DIR", settingsDir);

            try
            {
                // Normal: option wins.
                var withOption = TargetBuildSettings.Get(option: "TFT-v1.30.1.10211");
                Assert.Equal("TFT-v1.30.1.10211", withOption.Build);

                // Swapped: no option, so env var wins.
                var noOption = TargetBuildSettings.Get(option: null);
                Assert.Equal("TFT-v1.30.3.10205", noOption.Build);
                Assert.Equal(TargetSource.Environment, noOption.Source);

                // Restore: option back.
                Assert.Equal("TFT-v1.30.1.10211", TargetBuildSettings.Get(option: "TFT-v1.30.1.10211").Build);
            }
            finally
            {
                Environment.SetEnvironmentVariable("WC3_TARGET_BUILD", null);
            }
        }
        finally
        {
            if (Directory.Exists(settingsDir))
                Directory.Delete(settingsDir, true);
        }
    }

    private static int CountOccurrences(string text, string needle)
    {
        int n = 0, i = 0;
        while ((i = text.IndexOf(needle, i, StringComparison.Ordinal)) >= 0) { n++; i += needle.Length; }
        return n;
    }

    // A schema folder is enough to make a path a valid target, the manifest content is not read here.
    private static string TempSchemaFolder()
    {
        var dir = Path.Combine(TempSettingsDir(), "TFT-v1.30.1.10211");
        Directory.CreateDirectory(dir);
        File.WriteAllText(Path.Combine(dir, "manifest.json"), "{}");
        return dir;
    }

    [Fact]
    public void Set_twice_keeps_other_keys_and_writes_one_target_build()
    {
        TargetBuildSettings.ResetSettingsDir();
        var settingsDir = TempSettingsDir();
        var schemasDir = TempSettingsDir();
        var buildDir = TempSchemaFolder();
        try
        {
            Environment.SetEnvironmentVariable("WC3CTL_SETTINGS_DIR", settingsDir);
            TargetBuildSettings.ResetSettingsDir();

            var settingsFile = Path.Combine(settingsDir, "settings.json");
            File.WriteAllText(settingsFile, "{\"other\":1,\"targetBuild\":\"x\"}");

            TargetBuildSettings.Set(buildDir, schemasDir);
            // The second Set is what produced a duplicate key before the fix.
            TargetBuildSettings.Set(buildDir, schemasDir);

            var text = File.ReadAllText(settingsFile);
            Assert.Contains("\"other\"", text);
            Assert.Equal(1, CountOccurrences(text, "targetBuild"));
            using var doc = System.Text.Json.JsonDocument.Parse(text); // throws if the file is not valid JSON
            Assert.Equal(buildDir, doc.RootElement.GetProperty("targetBuild").GetString());
        }
        finally
        {
            Environment.SetEnvironmentVariable("WC3CTL_SETTINGS_DIR", null);
            if (Directory.Exists(settingsDir)) Directory.Delete(settingsDir, true);
            if (Directory.Exists(schemasDir)) Directory.Delete(schemasDir, true);
            if (Directory.Exists(buildDir)) Directory.Delete(Path.GetDirectoryName(buildDir)!, true);
        }
    }

    [Fact]
    public void Set_with_a_schema_folder_path_persists_it()
    {
        TargetBuildSettings.ResetSettingsDir();
        var settingsDir = TempSettingsDir();
        var schemasDir = TempSettingsDir();
        var buildDir = TempSchemaFolder();
        try
        {
            Environment.SetEnvironmentVariable("WC3CTL_SETTINGS_DIR", settingsDir);
            TargetBuildSettings.ResetSettingsDir();

            TargetBuildSettings.Set(buildDir, schemasDir);

            var result = TargetBuildSettings.Get();
            Assert.Equal(buildDir, result.Build);
            Assert.Equal(TargetSource.Settings, result.Source);
        }
        finally
        {
            Environment.SetEnvironmentVariable("WC3CTL_SETTINGS_DIR", null);
            if (Directory.Exists(settingsDir)) Directory.Delete(settingsDir, true);
            if (Directory.Exists(schemasDir)) Directory.Delete(schemasDir, true);
            if (Directory.Exists(buildDir)) Directory.Delete(Path.GetDirectoryName(buildDir)!, true);
        }
    }

    // Plain strings, so the suggestion function is tested with no schemas folder at all.
    private static readonly string[] SampleBuilds =
    {
        "TFT-v1.30.1.10211", "TFT-v1.31.0.11889-w3t", "TFT-v1.31.1.12173",
        "1.30.4.11274", "3.0.0.24268", "Beta-ROC-v1.00",
    };

    [Fact]
    public void Suggest_finds_a_substring_match_over_a_distance_only_one()
    {
        // "1.30.1" is not a prefix of any build, but it is inside TFT-v1.30.1.10211.
        var s = TargetBuildSettings.Suggest("1.30.1", SampleBuilds);
        Assert.Equal("TFT-v1.30.1.10211", s[0]);
    }

    [Fact]
    public void Suggest_matches_a_prefix_case_insensitively_and_nearest_first()
    {
        var s = TargetBuildSettings.Suggest("tft-v1.31", SampleBuilds);
        Assert.Equal(2, s.Take(2).Count(b => b.StartsWith("TFT-v1.31", StringComparison.Ordinal)));
        Assert.Contains("TFT-v1.31.0.11889-w3t", s.Take(2));
        Assert.Contains("TFT-v1.31.1.12173", s.Take(2));
    }

    [Fact]
    public void Suggest_is_capped_at_max_and_never_empty_for_a_nonempty_list()
    {
        var builds = Enumerable.Range(0, 40).Select(i => $"TFT-v1.3{i}.1.10211").ToArray();
        var s = TargetBuildSettings.Suggest("bogus-name", builds);
        Assert.Equal(10, s.Count);
        Assert.Empty(TargetBuildSettings.Suggest("x", Array.Empty<string>()));
    }
}
