// tests/Wc3.Tests/PlacementResolveTests.cs
using War3Net.Build.Widget;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;
using Wc3.Render;
using Xunit;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// End-to-end placement rendering: with the shared <see cref="PlacementModelResolver"/> and
/// a real CASC context, every placed widget in a real map must render as its actual model —
/// no orange/blue fallback boxes. The two ways a box appeared for a real map were (a) classic
/// doodads with variation-suffixed model files (fixed by RenderModelCommand's variation-0
/// candidate) and (b) custom doodads that inherit their art from a base object without an own
/// model field (fixed by the resolver's base-rawcode inheritance). Start locations carry no
/// model and are skipped, not boxed.
/// </summary>
public class PlacementResolveTests
{
    private const string Install = @"C:\Warcraft III";
    private static readonly string MapPath =
        TestCorpus.Map(@"GGGA_V0.02a.w3x");

    private readonly ITestOutputHelper _output;
    public PlacementResolveTests(ITestOutputHelper output) => _output = output;

    /// <summary>The exact resolver Studio's viewport uses: shared path resolution +
    /// Prepare into a posed, textured <see cref="PlacementModel"/> (null on any miss).</summary>
    private static Func<string, bool, PlacementModel?> Resolver(
        MapDocument doc, GameData.GameDataContext? ctx, string? gameDir) =>
        (rawcode, isUnit) =>
        {
            var path = PlacementModelResolver.ResolveModelPath(doc, rawcode, isUnit, gameDir);
            if (path is null) return null;
            try
            {
                var prepared = RenderModelCommand.Prepare(doc, path, ctx);
                return new PlacementModel(prepared.Model.PosedAt("Stand"), prepared.Textures);
            }
            catch { return null; }
        };

    /// <summary>Custom doodads with no model field of their own inherit their base object's
    /// model (D00A→ARrk, D00V→LPcr, D02A→ZPfw) instead of degrading to a box.</summary>
    [Theory]
    [Trait("Category", "Corpus")]
    [InlineData("D00A")]
    [InlineData("D00V")]
    [InlineData("D02A")]
    public void Custom_doodad_inherits_base_model_path(string rawcode)
    {
        if (!File.Exists(MapPath) || !Directory.Exists(Install)) return;
        var doc = MapDocument.Load(MapPath);

        var path = PlacementModelResolver.ResolveModelPath(doc, rawcode, isUnit: false, Install);
        _output.WriteLine($"{rawcode} -> {path ?? "(null)"}");
        Assert.False(string.IsNullOrWhiteSpace(path),
            $"{rawcode} should inherit its base object's model, not resolve to nothing");
    }

    /// <summary>The whole-map gate: build the placement scene exactly as the viewport does
    /// and assert NOT ONE instance falls back to a box. Every placed unit/doodad renders its
    /// real model; model-less markers (start locations) and genuinely model-less widgets are
    /// skipped. This is the "100%, no boxes" guarantee.</summary>
    [Fact]
    [Trait("Category", "Corpus")]
    public void GGGA_scene_has_zero_fallback_boxes()
    {
        if (!File.Exists(MapPath) || !Directory.Exists(Install)) return;
        Assert.True(GameData.GameData.TryOpen(Install, out var ctx, out var diag), diag);

        var doc = MapDocument.Load(MapPath);
        var scene = PlacementScene.Build(doc, Resolver(doc, ctx, Install));

        var boxed = scene.Instances
            .Where(i => i.MeshKey is PlacementScene.UnitBoxKey or PlacementScene.DoodadBoxKey)
            .Select(i => i.MeshKey).Distinct().ToList();
        int boxInstances = scene.Instances.Count(i =>
            i.MeshKey is PlacementScene.UnitBoxKey or PlacementScene.DoodadBoxKey);

        // A mesh whose every section is untextured renders as a solid gray/white shape — the
        // "white box" symptom (e.g. GeneralHeroGlow's team-color glow plane). None should remain:
        // effect-only models are skipped, everything else is textured.
        var fullyUntextured = scene.Meshes
            .Where(m => m.Sections.Count > 0 && m.Sections.All(s => s.TextureSlot < 0))
            .Select(m => m.Key).ToList();

        _output.WriteLine(
            $"instances={scene.Instances.Count} meshes={scene.Meshes.Count} "
            + $"boxInstances={boxInstances} fullyUntexturedMeshes={fullyUntextured.Count}");
        if (boxInstances > 0)
        {
            var boxKeys = scene.Meshes.Where(m => boxed.Contains(m.Key)).Select(m => m.Key);
            _output.WriteLine("BOXED: " + string.Join(", ", boxKeys));
        }
        if (fullyUntextured.Count > 0)
            _output.WriteLine("WHITE (fully untextured): " + string.Join(", ", fullyUntextured));

        Assert.Empty(boxed);              // no fallback boxes
        Assert.Empty(fullyUntextured);    // no solid white/gray models
        Assert.NotEmpty(scene.Instances); // sanity: the map really does place things
    }
}
