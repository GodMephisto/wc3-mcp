// tests/Wc3.Tests/SchemasDirResolutionTests.cs
// How a command or MCP tool picks its schemas folder. WC3_SCHEMAS_DIR is process wide, so this
// class runs outside the parallel collections and restores the variable after each case.
using Wc3.Commands;

namespace Wc3.Tests;

[CollectionDefinition(nameof(SchemasDirResolutionTests), DisableParallelization = true)]
public sealed class SchemasDirResolutionCollection { }

[Collection(nameof(SchemasDirResolutionTests))]
public sealed class SchemasDirResolutionTests : IDisposable
{
    private readonly string? _saved = Environment.GetEnvironmentVariable(GameSchemaCommand.SchemasDirEnvVar);

    public void Dispose() => Environment.SetEnvironmentVariable(GameSchemaCommand.SchemasDirEnvVar, _saved);

    private static string NewDir()
    {
        string dir = Path.Combine(Path.GetTempPath(), "wc3schemas-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        return dir;
    }

    [Fact]
    public void The_variable_is_used_when_no_argument_is_given()
    {
        string dir = NewDir();
        try
        {
            Environment.SetEnvironmentVariable(GameSchemaCommand.SchemasDirEnvVar, dir);
            Assert.Equal(Path.GetFullPath(dir), GameSchemaCommand.ResolveSchemasDir(null));
        }
        finally { Directory.Delete(dir, true); }
    }

    [Fact]
    public void An_existing_explicit_path_wins_over_the_variable()
    {
        string arg = NewDir();
        string env = NewDir();
        try
        {
            Environment.SetEnvironmentVariable(GameSchemaCommand.SchemasDirEnvVar, env);
            Assert.Equal(Path.GetFullPath(arg), GameSchemaCommand.ResolveSchemasDir(arg));
        }
        finally { Directory.Delete(arg, true); Directory.Delete(env, true); }
    }

    [Theory]
    [InlineData("  ")]
    public void A_blank_variable_falls_through_to_the_dataset(string value)
    {
        Environment.SetEnvironmentVariable(GameSchemaCommand.SchemasDirEnvVar, value);
        string resolved = GameSchemaCommand.ResolveSchemasDir(null);
        Assert.True(Directory.Exists(resolved));
        Assert.NotEqual(value, resolved);
    }

    [Fact]
    public void A_missing_variable_directory_falls_through_to_the_dataset()
    {
        string missing = Path.Combine(Path.GetTempPath(), "wc3-nope-" + Guid.NewGuid().ToString("N"));
        Environment.SetEnvironmentVariable(GameSchemaCommand.SchemasDirEnvVar, missing);
        string resolved = GameSchemaCommand.ResolveSchemasDir(null);
        Assert.True(Directory.Exists(resolved));
        Assert.NotEqual(missing, resolved);
    }

    [Fact]
    public void The_variable_is_used_when_the_argument_names_a_missing_directory()
    {
        string env = NewDir();
        try
        {
            string missing = Path.Combine(env, "not-there");
            Assert.Equal(Path.GetFullPath(env), GameSchemaCommand.ResolveSchemasDir(missing, env));
        }
        finally { Directory.Delete(env, true); }
    }
}
