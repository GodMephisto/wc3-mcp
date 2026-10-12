// tests/Wc3.Tests/CompatExplainTests.cs
// Hermetic tests for CompatExplainCommand. Builds two tiny schemas in a temp folder, one target
// that lacks an identifier and one newer build that declares it.
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public sealed class CompatExplainTests : IDisposable
{
    private readonly string _dir = Path.Combine(
        Path.GetTempPath(), "wc3ctl-compat-explain", Guid.NewGuid().ToString("N"));

    public CompatExplainTests() => Directory.CreateDirectory(_dir);

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch (IOException) { }
    }

    /// <summary>Write one schema folder declaring the given native names in common.j.</summary>
    private void Schema(string build, params string[] natives)
    {
        var jass = new JassApi(
            Array.Empty<JassTypeDecl>(),
            natives.Select(n => new JassNativeDecl(n, Array.Empty<JassParameter>(), "boolean", false)).ToArray(),
            Array.Empty<JassGlobalDecl>(),
            Array.Empty<JassFunctionDecl>());
        var schema = new GameSchema(1, build, build,
            Array.Empty<SchemaInput>(), new[] { "script" },
            null, (SchemaTriggers?)null,
            new Dictionary<string, JassApi>(StringComparer.Ordinal) { ["scripts/common.j"] = jass });
        var dir = Path.Combine(_dir, build);
        Directory.CreateDirectory(dir);
        GameSchemaCommand.Save(schema, dir);
    }

    [Fact]
    public void Explain_reports_declared_in_target_when_the_target_has_it()
    {
        Schema("TFT-v1.30.1.10211", "NewTargetNative");
        Schema("TFT-v1.31.0.10420", "NewTargetNative", "ExtraNative");

        var r = CompatExplainCommand.Execute("NewTargetNative", "TFT-v1.30.1.10211", _dir);

        Assert.True(r.IsIdentifier);
        Assert.Equal("native", r.Kind);
        Assert.True(r.DeclaredInTarget);
    }

    [Fact]
    public void Explain_names_the_build_that_added_a_missing_identifier()
    {
        Schema("TFT-v1.30.1.10211", "NewTargetNative");
        Schema("TFT-v1.31.0.10420", "NewTargetNative", "ExtraNative");

        var r = CompatExplainCommand.Execute("ExtraNative", "TFT-v1.30.1.10211", _dir);

        Assert.True(r.IsIdentifier);
        Assert.Equal("native", r.Kind);
        Assert.False(r.DeclaredInTarget);
        Assert.Equal("TFT-v1.31.0.10420", r.AddedIn);
        Assert.Contains("TFT-v1.31.0.10420", r.Builds);
    }

    [Fact]
    public void Explain_reports_an_unknown_name_as_not_an_identifier()
    {
        Schema("TFT-v1.30.1.10211", "NewTargetNative");

        var r = CompatExplainCommand.Execute("NoSuchIdentifier", "TFT-v1.30.1.10211", _dir);

        Assert.False(r.IsIdentifier);
        Assert.False(r.DeclaredInTarget);
        Assert.Empty(r.Builds);
    }
}
