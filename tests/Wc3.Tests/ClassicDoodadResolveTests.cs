// tests/Wc3.Tests/ClassicDoodadResolveTests.cs
using War3Net.Build.Widget;
using War3Net.Common.Extensions;
using Wc3.Commands;
using Wc3.Model;
using Xunit.Abstractions;

namespace Wc3.Tests;

/// <summary>
/// Classic-tileset doodad models (Ruins_Flower, Lords_Rock, AshenRock, ...) live in CASC
/// with a per-variation filename suffix and no bare file: dfil says
/// "Doodads\Ruins\Plants\Ruins_Flower\Ruins_Flower" but the storage only has
/// ruins_flower0.mdx..ruins_flower4.mdx under the plain war3.w3mod: layer (verified by
/// enumerating the CASC listfile). These tests pin the enumeration discovery, the
/// variation-0 resolution through <see cref="RenderModelCommand.Prepare"/>, and the
/// aggregate resolve rate over every doodad/destructable type placed in a real map.
/// </summary>
public class ClassicDoodadResolveTests
{
    private const string Install = @"C:\Warcraft III";
    private static readonly string MapPath =
        TestCorpus.Map(@"GGGA_V0.02a.w3x");

    private readonly ITestOutputHelper _output;
    public ClassicDoodadResolveTests(ITestOutputHelper output) => _output = output;

    private static MapDocument EmptyMap() =>
        MapDocument.Load(SyntheticMap.Build(new Dictionary<string, byte[]>
        { ["war3map.j"] = new byte[] { 1 } }));

    [Fact]
    [Trait("Category", "GameData")]
    public void Enumeration_locates_classic_doodads_under_the_plain_war3_layer()
    {
        if (!Directory.Exists(Install)) return;
        Assert.True(GameData.GameData.TryOpen(Install, out var ctx, out var diag), diag);

        var hits = ctx!.FindFiles("ruins_flower");
        Assert.Contains(@"war3.w3mod:doodads\ruins\plants\ruins_flower\ruins_flower0.mdx", hits);
        // And the bare (suffixless) model file genuinely does not exist in the listfile.
        Assert.DoesNotContain(hits, h => h.EndsWith(@"\ruins_flower.mdx", StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [Trait("Category", "GameData")]
    [InlineData(@"Doodads\Ruins\Plants\Ruins_Flower\Ruins_Flower")]
    [InlineData(@"Doodads\LordaeronSummer\Rocks\Lords_Rock\Lords_Rock")]
    [InlineData(@"Doodads\Ashenvale\Rocks\AshenRock\AshenRock")]
    [InlineData(@"Doodads\LordaeronSummer\Props\LanternPost\LanternPost")]
    [InlineData(@"Doodads\Ashenvale\Structures\AshenRubble\AshenRubble")]
    [InlineData(@"Doodads\LordaeronSummer\Plants\RiverRushes\RiverRushes")]
    [InlineData(@"Doodads\Terrain\LOSBlocker\LOSBlocker")] // bare losblocker.mdx exists — no suffix needed
    public void Classic_doodad_dfil_paths_prepare_from_casc(string dfil)
    {
        if (!Directory.Exists(Install)) return;
        Assert.True(GameData.GameData.TryOpen(Install, out var ctx, out var diag), diag);

        var prepared = RenderModelCommand.Prepare(EmptyMap(), dfil, ctx);
        Assert.NotEmpty(prepared.Model.Geosets);
    }

    /// <summary>
    /// Aggregate: every doodad/destructable type placed in the GGGA corpus map either
    /// resolves a model through Prepare or has no model path at all (a handful of types
    /// have an empty dfil/bfil — nothing to draw is correct for those, not a bug).
    /// Before the variation-0 fallback this map measured 103/125 with 18 Prepare failures.
    /// </summary>
    [Fact]
    [Trait("Category", "Corpus")]
    public void GGGA_every_placed_doodad_type_with_a_model_path_prepares()
    {
        if (!File.Exists(MapPath) || !Directory.Exists(Install)) return;
        Assert.True(GameData.GameData.TryOpen(Install, out var ctx, out var diag), diag);

        var doc = MapDocument.Load(MapPath);
        var doodads = Assert.IsAssignableFrom<MapDoodads>(doc.GetFile("war3map.doo")!.Model);
        var types = doodads.Doodads.Select(d => d.TypeId).Distinct()
            .Select(id => id.ToRawcode()).OrderBy(c => c, StringComparer.Ordinal).ToList();

        int ok = 0;
        var noPath = new List<string>();
        var failed = new List<string>();
        foreach (var rawcode in types)
        {
            var merged = ObjectGetCommand.Execute(doc, rawcode, ctx, Array.Empty<string>());
            string? path = null;
            foreach (var code in new[] { "dfil", "bfil" })
            {
                var field = merged.Fields.FirstOrDefault(
                    f => string.Equals(f.Code, code, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrWhiteSpace(field?.Value))
                {
                    path = field.Value.Split(',')[0].Trim();
                    break;
                }
            }
            path ??= RenderModelCommand.BaseModelPath(ObjectKind.Doodad, rawcode, Install)
                  ?? RenderModelCommand.BaseModelPath(ObjectKind.Destructable, rawcode, Install);

            if (string.IsNullOrWhiteSpace(path)) { noPath.Add(rawcode); continue; }
            try
            {
                RenderModelCommand.Prepare(doc, path, ctx);
                ok++;
            }
            catch (Exception ex)
            {
                failed.Add($"{rawcode} -> '{path}': {ex.Message}");
            }
        }

        _output.WriteLine($"types={types.Count} ok={ok} noPath={noPath.Count} failed={failed.Count}");
        if (noPath.Count > 0) _output.WriteLine("no model path: " + string.Join(", ", noPath));
        foreach (var f in failed) _output.WriteLine("FAIL " + f);
        // For anything still failing, ask the listfile where (or whether) the model lives.
        foreach (var f in failed.Take(5))
        {
            var stem = Path.GetFileName(f.Split('\'')[1].Split(',')[0].Trim().TrimEnd('\\'));
            foreach (var hit in ctx!.FindFiles(stem, max: 10))
                _output.WriteLine($"   listfile: {hit}");
        }

        Assert.Empty(failed);
        Assert.Equal(types.Count, ok + noPath.Count);
    }
}
