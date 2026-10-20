// src/Wc3.Commands/ModelLintCommand.cs
using Wc3.Modeling;

namespace Wc3.Commands;

/// <summary>
/// The scan outcome for one model file. <see cref="Findings"/> holds every problem the shared
/// checks report, and <see cref="Geosets"/> and <see cref="Textures"/> say what was inspected so
/// a clean result is distinguishable from a model that parsed to nothing.
/// </summary>
public sealed record ModelLintResult(
    string Path,
    int Geosets,
    int Textures,
    int Vertices,
    IReadOnlyList<ModelLintFinding> Findings)
{
    public int FindingCount => Findings.Count;
}

/// <summary>
/// Lints a single .mdx or .mdl file with the same checks the map-wide scan runs, without a map
/// to resolve texture paths against. Geometry, UVs, normals, indices, degenerate triangles,
/// skin data, animation finiteness and unused textures all work standalone. The map-resolving
/// missing-texture check needs a map and is skipped here.
/// </summary>
public static class ModelLintCommand
{
    public static ModelLintResult ExecutePath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        var bytes = File.ReadAllBytes(path);
        var name = Path.GetFileName(path);
        var model = ModelParser.Parse(bytes, name);
        var findings = new List<ModelLintFinding>();
        AssetModelLintCommand.Inspect(null, name, model, findings);
        findings.Sort((a, b) => a.Kind.CompareTo(b.Kind));
        int vertices = model.Geosets.Sum(g => g.Vertices.Length / 3);
        return new ModelLintResult(path, model.Geosets.Count, model.Textures.Count, vertices, findings);
    }
}
