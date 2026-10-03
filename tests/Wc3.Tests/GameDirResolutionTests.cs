// tests/Wc3.Tests/GameDirResolutionTests.cs
// How a tool call picks its Warcraft III folder. The env var is process wide, so this class
// runs outside the parallel collections and restores the variable after each case.
using Wc3.Mcp;

namespace Wc3.Tests;

[CollectionDefinition(nameof(GameDirResolutionTests), DisableParallelization = true)]
public sealed class GameDirResolutionCollection { }

[Collection(nameof(GameDirResolutionTests))]
public sealed class GameDirResolutionTests : IDisposable
{
    private readonly string? _saved = Environment.GetEnvironmentVariable(Wc3Tools.GameDirVariable);

    public void Dispose() => Environment.SetEnvironmentVariable(Wc3Tools.GameDirVariable, _saved);

    [Fact]
    public void The_argument_wins_over_the_variable()
    {
        Environment.SetEnvironmentVariable(Wc3Tools.GameDirVariable, @"D:\FromEnv");
        Assert.Equal(@"D:\FromArg", Wc3Tools.ResolveGameDir(@"D:\FromArg"));
    }

    [Fact]
    public void The_variable_is_used_when_no_argument_is_given()
    {
        Environment.SetEnvironmentVariable(Wc3Tools.GameDirVariable, @"D:\FromEnv");
        Assert.Equal(@"D:\FromEnv", Wc3Tools.ResolveGameDir(null));
    }

    [Theory]
    [InlineData("${user_config.game_dir}")]
    [InlineData("  ")]
    public void An_unfilled_placeholder_or_blank_variable_means_auto_detect(string value)
    {
        Environment.SetEnvironmentVariable(Wc3Tools.GameDirVariable, value);
        Assert.Null(Wc3Tools.ResolveGameDir(null));
        Assert.Null(Wc3Tools.ResolveGameDir("${user_config.game_dir}"));
    }
}
