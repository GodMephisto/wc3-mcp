// tests/Wc3.Tests/LeakModelCommandTests.cs
using System.Text;
using Wc3.Commands;
using Wc3.Model;

namespace Wc3.Tests;

/// <summary>
/// Hermetic coverage for the model-level leak report. Every map is built in memory and every
/// model is a small synthetic MDX, so no WC3 install and no corpus are needed. Each of the four
/// findings is pinned by a map that isolates it from the other three.
/// </summary>
public sealed class LeakModelCommandTests
{
    [Fact]
    public void Leak_model_reports_an_unreferenced_import()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [@"war3mapImported\Orphan.mdx"] = SyntheticModel.BuildMdx(texturePath: @"Textures\Test.blp"),
            [@"Textures\Test.blp"] = SyntheticModel.Blp(),
        }));

        var r = LeakModelCommand.Execute(doc);

        Assert.Equal(1, r.ModelsScanned);
        Assert.Equal(1, r.LeakedImports);
        var finding = Assert.Single(r.Findings, f => f.Kind == ModelLeakKind.LeakedImport);
        Assert.Equal(@"war3mapImported\Orphan.mdx", finding.Path);
        Assert.DoesNotContain(r.Findings, f => f.Kind == ModelLeakKind.MissingTexture);
    }

    [Fact]
    public void Leak_model_does_not_report_a_referenced_import()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [@"war3mapImported\Ref.mdx"] = SyntheticModel.BuildMdx(texturePath: @"Textures\Ref.blp"),
            [@"Textures\Ref.blp"] = SyntheticModel.Blp(),
            ["war3map.j"] = JassRef(@"war3mapImported\Ref.mdx"),
        }));

        var r = LeakModelCommand.Execute(doc);

        Assert.DoesNotContain(r.Findings, f => f.Kind == ModelLeakKind.LeakedImport);
    }

    [Fact]
    public void Leak_model_reports_a_model_naming_a_missing_texture()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [@"war3mapImported\MissTex.mdx"] = SyntheticModel.BuildMdx(texturePath: @"Textures\Missing.blp"),
            ["war3map.j"] = JassRef(@"war3mapImported\MissTex.mdx"),
        }));

        var r = LeakModelCommand.Execute(doc);

        Assert.Equal(1, r.MissingTextures);
        var finding = Assert.Single(r.Findings, f => f.Kind == ModelLeakKind.MissingTexture);
        Assert.Equal(@"war3mapImported\MissTex.mdx", finding.Path);
        Assert.Contains(@"Textures\Missing.blp", finding.Reason);
    }

    [Fact]
    public void Leak_model_reports_a_byte_identical_duplicate_import()
    {
        var bytes = SyntheticModel.BuildMdx(texturePath: @"Textures\Test.blp");
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [@"war3mapImported\A.mdx"] = bytes,
            [@"war3mapImported\B.mdx"] = bytes,
            [@"Textures\Test.blp"] = SyntheticModel.Blp(),
            ["war3map.j"] = JassRef(@"war3mapImported\A.mdx", @"war3mapImported\B.mdx"),
        }));

        var r = LeakModelCommand.Execute(doc);

        Assert.Equal(1, r.DuplicateImports);
        var finding = Assert.Single(r.Findings, f => f.Kind == ModelLeakKind.DuplicateImport);
        Assert.Equal(@"war3mapImported\A.mdx", finding.Path);
        Assert.Contains(@"war3mapImported\B.mdx", finding.Reason);
        Assert.DoesNotContain(r.Findings, f => f.Kind == ModelLeakKind.LeakedImport);
    }

    [Fact]
    public void Leak_model_reports_a_texture_no_model_names()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [@"war3mapImported\Ref.mdx"] = SyntheticModel.BuildMdx(texturePath: @"Textures\Ref.blp"),
            [@"Textures\Ref.blp"] = SyntheticModel.Blp(),
            [@"war3mapImported\Loose.blp"] = SyntheticModel.Blp(),
            ["war3map.j"] = JassRef(@"war3mapImported\Ref.mdx"),
        }));

        var r = LeakModelCommand.Execute(doc);

        Assert.Equal(2, r.TexturesScanned);
        var finding = Assert.Single(r.Findings, f => f.Kind == ModelLeakKind.UnusedTexture);
        Assert.Equal(@"war3mapImported\Loose.blp", finding.Path);
        Assert.DoesNotContain(r.Findings,
            f => f.Kind == ModelLeakKind.UnusedTexture && f.Path.Contains("Ref"));
    }

    [Fact]
    public void Leak_model_reports_a_parse_failure_as_a_diagnostic()
    {
        var doc = MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        {
            [@"war3mapImported\Broken.mdx"] = new byte[] { 0, 1, 2, 3 },
        }));

        var r = LeakModelCommand.Execute(doc);

        Assert.Equal(0, r.ModelsScanned);
        Assert.Contains(r.Diagnostics, d => d.Contains("Broken.mdx"));
    }

    /// <summary>A map script naming one or more asset paths in JASS string literals, with the
    /// backslashes doubled the way a script source spells a path separator.</summary>
    private static byte[] JassRef(params string[] paths)
    {
        var sb = new StringBuilder();
        foreach (var path in paths)
            sb.AppendLine($"call AddSpecialEffect(\"{path.Replace(@"\", @"\\")}\", 0, 0)");
        return Encoding.ASCII.GetBytes(sb.ToString());
    }
}
