// tests/Wc3.Tests/McpEnvPrecedenceTests.cs
// ENV-02. The environment precedence is the one part of the configuration contract a user can
// change without editing a file, so each tier is pinned here, including the placeholder case that
// an MCPB host leaves in its config when it has no game folder to give.
using Wc3.Commands;
using Wc3.Mcp;

namespace Wc3.Tests;

[Collection("Serial")]
public sealed class McpEnvPrecedenceTests : IDisposable
{
    private const string Placeholder = "${user_config.game_dir}";

    private readonly string _dir;
    private readonly string? _target;
    private readonly string? _gameDir;
    private readonly string? _settingsDir;

    public McpEnvPrecedenceTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "wc3env-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);

        _target = Environment.GetEnvironmentVariable("WC3_TARGET_BUILD");
        _gameDir = Environment.GetEnvironmentVariable("WC3_GAME_DIR");
        _settingsDir = Environment.GetEnvironmentVariable("WC3CTL_SETTINGS_DIR");

        Environment.SetEnvironmentVariable("WC3CTL_SETTINGS_DIR", _dir);
        TargetBuildSettings.ResetSettingsDir();
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable("WC3_TARGET_BUILD", _target);
        Environment.SetEnvironmentVariable("WC3_GAME_DIR", _gameDir);
        Environment.SetEnvironmentVariable("WC3CTL_SETTINGS_DIR", _settingsDir);
        TargetBuildSettings.ResetSettingsDir();
        try { Directory.Delete(_dir, recursive: true); } catch { /* best effort */ }
    }

    private void WriteSettings(string build) =>
        File.WriteAllText(Path.Combine(_dir, "settings.json"), "{\"targetBuild\":\"" + build + "\"}");

    [Fact]
    public void The_game_dir_argument_beats_the_environment()
    {
        Environment.SetEnvironmentVariable("WC3_GAME_DIR", @"C:\from-env");
        Assert.Equal(@"C:\from-arg", Wc3Tools.ResolveGameDir(@"C:\from-arg"));
    }

    [Fact]
    public void The_environment_is_used_when_no_argument_is_given()
    {
        Environment.SetEnvironmentVariable("WC3_GAME_DIR", @"C:\from-env");
        Assert.Equal(@"C:\from-env", Wc3Tools.ResolveGameDir(null));
    }

    [Fact]
    public void A_placeholder_argument_falls_through_to_the_environment()
    {
        // An unfilled ${...} is what a host that was never configured writes. It is not a path,
        // and treating it as one is how a tool ends up reading a folder literally called that.
        Environment.SetEnvironmentVariable("WC3_GAME_DIR", @"C:\from-env");
        Assert.Equal(@"C:\from-env", Wc3Tools.ResolveGameDir(Placeholder));
    }

    [Fact]
    public void A_placeholder_environment_value_counts_as_unset()
    {
        Environment.SetEnvironmentVariable("WC3_GAME_DIR", Placeholder);
        Assert.Null(Wc3Tools.ResolveGameDir(null));
    }

    [Fact]
    public void The_target_argument_beats_the_environment_and_says_so()
    {
        Environment.SetEnvironmentVariable("WC3_TARGET_BUILD", "FromEnv");
        var r = TargetBuildSettings.Get(option: "FromArg");
        Assert.Equal("FromArg", r.Build);
        Assert.Equal(TargetSource.Option, r.Source);
    }

    [Fact]
    public void The_environment_beats_the_settings_file_and_says_so()
    {
        WriteSettings("FromSettings");
        Environment.SetEnvironmentVariable("WC3_TARGET_BUILD", "FromEnv");
        var r = TargetBuildSettings.Get();
        Assert.Equal("FromEnv", r.Build);
        Assert.Equal(TargetSource.Environment, r.Source);
    }

    [Fact]
    public void The_settings_file_is_used_when_the_environment_is_clear_and_says_so()
    {
        WriteSettings("FromSettings");
        Environment.SetEnvironmentVariable("WC3_TARGET_BUILD", null);
        var r = TargetBuildSettings.Get();
        Assert.Equal("FromSettings", r.Build);
        Assert.Equal(TargetSource.Settings, r.Source);
    }
}
