// tests/Wc3.Tests/CompatMatrixTests.cs
// Hermetic tests for CompatMatrixCommand. Two maps by two target builds, checked against tiny
// schemas built in a temp folder.
using System.Text;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

public sealed class CompatMatrixTests : IDisposable
{
    private readonly string _dir = Path.Combine(
        Path.GetTempPath(), "wc3ctl-compat-matrix", Guid.NewGuid().ToString("N"));

    public CompatMatrixTests() => Directory.CreateDirectory(_dir);

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch (IOException) { }
    }

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

    private string Map(string script)
    {
        var path = Path.Combine(_dir, "map-" + Guid.NewGuid().ToString("N") + ".w3x");
        File.WriteAllBytes(path, SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            ["war3map.j"] = Encoding.UTF8.GetBytes(script),
        }));
        return path;
    }

    [Fact]
    public void Matrix_reports_pass_and_fail_per_map_and_target()
    {
        Schema("TFT-v1.30.1.10211");
        Schema("MyBuild", "MatrixNative");

        var caller = Map("function main takes nothing returns nothing\n"
            + "    call MatrixNative()\nendfunction\n");
        var plain = Map("function main takes nothing returns nothing\nendfunction\n");

        var r = CompatMatrixCommand.Execute(
            new[] { caller, plain }, new[] { "TFT-v1.30.1.10211", "MyBuild" }, _dir);

        Assert.Equal(2, r.Maps.Count);
        Assert.Equal(2, r.Targets.Count);
        Assert.Equal(4, r.Cells.Count);

        string Name(string path) => Path.GetFileName(path);
        var fail = r.Cells.Single(c => c.Map == Name(caller) && c.Target == "TFT-v1.30.1.10211");
        Assert.False(fail.Pass);
        Assert.Equal(1, fail.Errors);

        var pass = r.Cells.Single(c => c.Map == Name(caller) && c.Target == "MyBuild");
        Assert.True(pass.Pass);
        Assert.Equal(0, pass.Errors);

        Assert.All(r.Cells.Where(c => c.Map == Name(plain)), c => Assert.True(c.Pass));
    }

    [Fact]
    public void Matrix_requires_at_least_one_map_and_one_target()
    {
        Assert.Throws<ArgumentException>(() =>
            CompatMatrixCommand.Execute(Array.Empty<string>(), new[] { "x" }, _dir));
        Assert.Throws<ArgumentException>(() =>
            CompatMatrixCommand.Execute(new[] { "y" }, Array.Empty<string>(), _dir));
    }
}
