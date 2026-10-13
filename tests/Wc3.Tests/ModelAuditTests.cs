// tests/Wc3.Tests/ModelAuditTests.cs
using System.Text;
using Wc3.Commands;

namespace Wc3.Tests;

public sealed class ModelAuditTests
{
    private static string Mdl(int textureCount, string vertices)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Version { FormatVersion 800, }");
        sb.AppendLine($"Textures {textureCount} {{");
        for (int i = 0; i < textureCount; i++)
            sb.AppendLine("  Bitmap { Image \"Textures\\Test.blp\", },");
        sb.AppendLine("}");
        sb.AppendLine("Materials 1 { Material { Layer { FilterMode None, static TextureID 0, }, }, }");
        sb.AppendLine($$"""
            Geoset {
                Vertices 3 { {{vertices}}, },
                Normals 3 { { 0, 0, 1 }, { 0, 0, 1 }, { 0, 0, 1 }, },
                TVertices 3 { { 0, 0 }, { 1, 0 }, { 0, 1 }, },
                Faces 1 3 { Triangles { { 0, 1, 2 }, }, },
                MaterialID 0,
            }
            """);
        return sb.ToString();
    }

    private static ModelAuditResult Audit(string mdl)
        => ModelAuditCommand.Execute(Encoding.UTF8.GetBytes(mdl), "test.mdl");

    [Fact]
    public void A_clean_model_reports_no_issues()
    {
        var r = Audit(Mdl(1, "{ 0, 0, 0 }, { 1, 0, 0 }, { 0, 1, 0 }"));
        Assert.Equal("mdl", r.Format);
        Assert.Equal(1, r.Geosets);
        Assert.Equal(3, r.Vertices);
        Assert.Equal(1, r.Faces);
        Assert.Empty(r.Issues);
    }

    [Fact]
    public void A_duplicate_texture_is_reported()
    {
        var r = Audit(Mdl(2, "{ 0, 0, 0 }, { 1, 0, 0 }, { 0, 1, 0 }"));
        Assert.Contains(r.Issues, i => i.Kind == ModelIssueKind.DuplicateTexture);
    }

    [Fact]
    public void A_degenerate_triangle_is_reported()
    {
        var r = Audit(Mdl(1, "{ 0, 0, 0 }, { 1, 0, 0 }, { 2, 0, 0 }"));
        Assert.Contains(r.Issues, i => i.Kind == ModelIssueKind.ZeroAreaFace);
    }

    [Fact]
    public void Unparseable_bytes_report_a_diagnostic_not_an_issue()
    {
        var r = ModelAuditCommand.Execute(new byte[] { 1, 2, 3, 4 }, "broken.mdx");
        Assert.Empty(r.Issues);
        Assert.NotEmpty(r.Diagnostics);
    }

    [Fact]
    public void A_particle_emitter_chunk_is_counted_from_the_chunk_list()
    {
        var bytes = SyntheticModel.BuildMdx(
            extraChunks: new[] { ("PREM", new byte[8]) });
        var r = ModelAuditCommand.Execute(bytes, "fx.mdx");
        Assert.Equal("mdx", r.Format);
        Assert.Equal(1, r.ParticleEmitters);
        Assert.Equal(0, r.ParticleEmitters2);
        Assert.Equal(0, r.Ribbons);
        Assert.Equal(0, r.Corns);
        Assert.Equal(1, r.Effects);
    }

    [Fact]
    public void Every_effect_tag_is_counted_and_summed()
    {
        var bytes = SyntheticModel.BuildMdx(extraChunks: new[]
        {
            ("PREM", new byte[8]),
            ("PREM", new byte[16]),
            ("PRE2", new byte[12]),
            ("RIBB", new byte[4]),
            ("CORN", new byte[4]),
        });
        var r = ModelAuditCommand.Execute(bytes, "fx.mdx");
        Assert.Equal(2, r.ParticleEmitters);
        Assert.Equal(1, r.ParticleEmitters2);
        Assert.Equal(1, r.Ribbons);
        Assert.Equal(1, r.Corns);
        Assert.Equal(5, r.Effects);
    }

    [Fact]
    public void A_model_without_effects_reports_zero()
    {
        var r = ModelAuditCommand.Execute(SyntheticModel.TinyMdx(), "plain.mdx");
        Assert.Equal(0, r.Effects);
        Assert.Equal(0, r.ParticleEmitters);
        Assert.Equal(0, r.Corns);
    }

    [Fact]
    public void An_emitter_node_outside_the_model_is_reported()
    {
        var bytes = SyntheticModel.BuildMdx(extraChunks: new[]
        {
            ("PIVT", SyntheticModel.Pivots(2)),
            ("PREM", SyntheticModel.ParticleEmitter1Record(objectId: 7)),
        });
        var r = ModelAuditCommand.Execute(bytes, "fx.mdx");
        Assert.Contains(r.Issues, i => i.Kind == ModelIssueKind.DanglingEmitterNode);
    }

    [Fact]
    public void An_emitter_node_inside_the_model_is_clean()
    {
        var bytes = SyntheticModel.BuildMdx(extraChunks: new[]
        {
            ("PIVT", SyntheticModel.Pivots(2)),
            ("PREM", SyntheticModel.ParticleEmitter1Record(objectId: 1)),
        });
        var r = ModelAuditCommand.Execute(bytes, "fx.mdx");
        Assert.DoesNotContain(r.Issues, i => i.Kind == ModelIssueKind.DanglingEmitterNode);
    }

    [Fact]
    public void A_model_without_a_node_list_skips_the_emitter_node_check()
    {
        // No PIVT, so there is no node list to check against and the check is skipped rather than
        // flagging an emitter whose node id cannot be validated.
        var bytes = SyntheticModel.BuildMdx(extraChunks: new[]
        {
            ("PREM", SyntheticModel.ParticleEmitter1Record(objectId: 7)),
        });
        var r = ModelAuditCommand.Execute(bytes, "fx.mdx");
        Assert.DoesNotContain(r.Issues, i => i.Kind == ModelIssueKind.DanglingEmitterNode);
    }

    [Fact]
    public void An_emitter_texture_outside_the_texture_list_is_reported()
    {
        var bytes = SyntheticModel.BuildMdx(extraChunks: new[]
        {
            ("PRE2", SyntheticModel.ParticleEmitter2Record(objectId: 0, textureId: 5)),
        });
        var r = ModelAuditCommand.Execute(bytes, "fx.mdx");
        Assert.Contains(r.Issues, i => i.Kind == ModelIssueKind.DanglingEmitterTexture);
    }

    [Fact]
    public void An_emitter_texture_inside_the_texture_list_is_clean()
    {
        var bytes = SyntheticModel.BuildMdx(extraChunks: new[]
        {
            ("PRE2", SyntheticModel.ParticleEmitter2Record(objectId: 0, textureId: 0)),
        });
        var r = ModelAuditCommand.Execute(bytes, "fx.mdx");
        Assert.DoesNotContain(r.Issues, i => i.Kind == ModelIssueKind.DanglingEmitterTexture);
    }

    [Fact]
    public void Every_emitter_in_a_multi_record_chunk_is_checked()
    {
        var payload = SyntheticModel.ParticleEmitter2Record(objectId: 0, textureId: 0)
            .Concat(SyntheticModel.ParticleEmitter2Record(objectId: 1, textureId: 9))
            .ToArray();
        var bytes = SyntheticModel.BuildMdx(extraChunks: new[] { ("PRE2", payload) });
        var r = ModelAuditCommand.Execute(bytes, "fx.mdx");
        Assert.Equal(1, r.ParticleEmitters2);
        Assert.Contains(r.Issues, i => i.Kind == ModelIssueKind.DanglingEmitterTexture);
    }
}
