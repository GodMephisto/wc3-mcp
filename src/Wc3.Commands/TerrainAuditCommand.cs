// src/Wc3.Commands/TerrainAuditCommand.cs
using War3Net.Build.Environment;   // MapEnvironment, TerrainTile, TileFlags
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>What kind of measured terrain waste or structural fault a finding describes.</summary>
public enum TerrainIssueKind
{
    /// <summary>Every corner is identical across height, cliff level, texture and flags.</summary>
    UniformTerrain,
    /// <summary>Every corner's ground height is 0 and its cliff level is 0.</summary>
    AllZeroTerrain,
    /// <summary>A corner's ground texture index is at or past the map's declared ground tile count.</summary>
    GroundTextureOutOfRange,
    /// <summary>A corner's cliff texture index is at or past the map's declared cliff tile count.</summary>
    CliffTextureOutOfRange,
    /// <summary>The tile buffer length does not match the grid the header declares.</summary>
    BufferInconsistent,
}

/// <summary>One reported issue, with a short human detail.</summary>
public sealed record TerrainIssue(TerrainIssueKind Kind, string Detail);

/// <summary>
/// What a map's <c>war3map.w3e</c> holds and what is measurably wasteful about it. Counts are
/// always present when the terrain parsed. <see cref="Issues"/> is empty for a map with no
/// measured waste, and <see cref="Diagnostics"/> carries a missing file, an inconsistent buffer
/// or a check the map's own data could not support, so a bad map never reads as a clean one.
/// </summary>
/// <param name="Path">The map path the audit ran against.</param>
/// <param name="GridWidth">Corner columns, that is tile columns plus one.</param>
/// <param name="GridHeight">Corner rows, that is tile rows plus one.</param>
/// <param name="TileCount">Corner tiles, <see cref="GridWidth"/> times <see cref="GridHeight"/>.</param>
/// <param name="GroundTileTypes">Declared ground tile entries in the map's tileset table.</param>
/// <param name="CliffTileTypes">Declared cliff tile entries in the map's tileset table.</param>
/// <param name="DistinctGroundTextures">Distinct ground texture indices actually referenced by a corner.</param>
/// <param name="DistinctCliffTextures">Distinct cliff texture indices actually referenced by a corner.</param>
/// <param name="CliffCells">Corners with a cliff level above 0.</param>
/// <param name="RampCells">Corners flagged as a ramp.</param>
/// <param name="WaterCells">Corners flagged as water.</param>
/// <param name="BlightCells">Corners flagged as blighted.</param>
/// <param name="BoundaryCells">Corners flagged as a boundary.</param>
/// <param name="EdgeCells">Corners flagged as an edge tile.</param>
/// <param name="MinHeight">Lowest ground height, in War3Net height units.</param>
/// <param name="MaxHeight">Highest ground height.</param>
/// <param name="MeanHeight">Mean ground height over every corner.</param>
/// <param name="MinCliff">Lowest cliff level.</param>
/// <param name="MaxCliff">Highest cliff level.</param>
/// <param name="TerrainFileBytes">Size of the <c>war3map.w3e</c> entry in bytes.</param>
/// <param name="Issues">Measured waste or structural faults, empty when none were found.</param>
/// <param name="Checks">The checks this audit performs, so a caller can see what was measured.</param>
/// <param name="Diagnostics">A missing terrain file, an inconsistent buffer, or a check the data could not support.</param>
public sealed record TerrainAuditResult(
    string Path,
    int GridWidth,
    int GridHeight,
    int TileCount,
    int GroundTileTypes,
    int CliffTileTypes,
    int DistinctGroundTextures,
    int DistinctCliffTextures,
    int CliffCells,
    int RampCells,
    int WaterCells,
    int BlightCells,
    int BoundaryCells,
    int EdgeCells,
    float MinHeight,
    float MaxHeight,
    float MeanHeight,
    int MinCliff,
    int MaxCliff,
    long TerrainFileBytes,
    IReadOnlyList<TerrainIssue> Issues,
    IReadOnlyList<string> Checks,
    IReadOnlyList<string> Diagnostics)
{
    public int IssueCount => Issues.Count;
}

/// <summary>
/// Audits a map's terrain, <c>war3map.w3e</c>, for its size and its measurable waste. Read-only,
/// it parses in memory and changes nothing.
///
/// <para><b>The checks, all computed from the parsed grid.</b>
/// <c>grid-dims</c> and <c>tile-count</c> come from <c>MapEnvironment.Width</c>/<c>Height</c> and the
/// tile buffer length. <c>tile-buffer-consistent</c> asserts the buffer holds
/// (Width+1)*(Height+1) corners. <c>uniform-terrain</c> reports a grid whose every corner is
/// identical across height, cliff level, ground and cliff texture and flags, which a placeholder or
/// flattened map is. <c>all-zero-terrain</c> is the specific case where every height and cliff level
/// is 0. <c>ground-texture-range</c> and <c>cliff-texture-range</c> report a corner whose texture
/// index reaches at or past the map's declared tile count. The cliff, ramp, water, blight, boundary
/// and edge totals, the height and cliff ranges, and the distinct texture indices are counted over
/// every corner.</para>
///
/// <para><b>What is NOT attempted, because it cannot be read off this data alone.</b> Geometric or
/// procedural redundancy (a ridge that could be a smaller set of tiles), how much an MPQ recompress
/// would save, and orphan analysis of a declared tile type no corner references (that would be a
/// different, palette-level statement, not a property of the grid). No byte saving is quoted,
/// because none is measurable from the grid alone.</para>
/// </summary>
public static class TerrainAuditCommand
{
    public const string TerrainFile = TerrainCommand.TerrainFile;

    /// <summary>The checks this audit runs. Every name here is evaluated against the parsed grid,
    /// except that a range check whose declared tile list is empty is reported in
    /// <see cref="TerrainAuditResult.Diagnostics"/> instead of silently passing.</summary>
    public static readonly IReadOnlyList<string> Checks = new[]
    {
        "grid-dims",
        "tile-count",
        "tile-buffer-consistent",
        "uniform-terrain",
        "all-zero-terrain",
        "ground-texture-range",
        "cliff-texture-range",
        "cliff-ramp-water-blight-boundary-edge-counts",
        "height-and-cliff-range",
        "distinct-ground-and-cliff-texture-indices",
        "terrain-file-bytes",
    };

    /// <summary>Loads the map at <paramref name="path"/> and audits its terrain.</summary>
    public static TerrainAuditResult ExecutePath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (!File.Exists(path) && !Directory.Exists(path))
            throw new FileNotFoundException($"map not found: {path}", path);
        return Execute(MapDocument.Load(path), path);
    }

    /// <summary>Audits the terrain of an already loaded map, named for the result.</summary>
    public static TerrainAuditResult Execute(MapDocument doc, string name)
    {
        ArgumentNullException.ThrowIfNull(doc);

        var entry = doc.GetFile(TerrainFile);
        if (entry?.Model is not MapEnvironment env)
            return Empty(name, entry?.CurrentSize ?? 0,
                $"map has no {TerrainFile} (terrain) to audit");

        long bytes = entry.CurrentSize;
        int gw = (int)env.Width + 1, gh = (int)env.Height + 1;
        int groundTypes = env.TerrainTypes?.Count ?? 0;
        int cliffTypes = env.CliffTypes?.Count ?? 0;
        var tiles = env.TerrainTiles;

        if (tiles is null || tiles.Count != gw * gh)
            return new TerrainAuditResult(
                name, gw, gh, tiles?.Count ?? 0, groundTypes, cliffTypes,
                0, 0, 0, 0, 0, 0, 0, 0, 0f, 0f, 0f, 0, 0, bytes,
                new[]
                {
                    new TerrainIssue(TerrainIssueKind.BufferInconsistent,
                        $"TerrainTiles holds {tiles?.Count ?? 0} corner(s) but the grid is {gw}x{gh} corners"),
                },
                Checks, Array.Empty<string>());

        float min = float.MaxValue, max = float.MinValue;
        double sum = 0;
        int cmin = int.MaxValue, cmax = int.MinValue;
        int cliffCells = 0, ramp = 0, water = 0, blight = 0, boundary = 0, edge = 0;
        int groundBad = 0, cliffBad = 0;
        bool allZero = true;
        var groundIndices = new HashSet<int>();
        var cliffIndices = new HashSet<int>();
        var signatures = new HashSet<TileSignature>();

        foreach (var t in tiles)
        {
            float h = t.Height;
            if (h < min) min = h;
            if (h > max) max = h;
            sum += h;

            int cl = t.CliffLevel;
            if (cl < cmin) cmin = cl;
            if (cl > cmax) cmax = cl;

            if (cl > 0) cliffCells++;
            if (t.IsRamp) ramp++;
            if (t.IsWater) water++;
            if (t.IsBlighted) blight++;
            if (t.IsBoundary) boundary++;
            if (t.IsEdgeTile) edge++;

            groundIndices.Add(t.Texture);
            cliffIndices.Add(t.CliffTexture);

            if (h != 0f || cl != 0) allZero = false;
            signatures.Add(new TileSignature(h, cl, t.Texture, t.CliffTexture, t.Flags));

            if (groundTypes > 0 && t.Texture >= groundTypes) groundBad++;
            if (cliffTypes > 0 && t.CliffTexture >= cliffTypes) cliffBad++;
        }

        float mean = tiles.Count > 0 ? (float)(sum / tiles.Count) : 0f;

        var issues = new List<TerrainIssue>();
        if (allZero)
            issues.Add(new TerrainIssue(TerrainIssueKind.AllZeroTerrain,
                $"all {tiles.Count} corner(s) have height 0 and cliff level 0, an unedited or flattened base grid"));
        else if (signatures.Count == 1)
            issues.Add(new TerrainIssue(TerrainIssueKind.UniformTerrain,
                $"all {tiles.Count} corner(s) are identical (height {min:0.###}, cliff {cmin}), so the grid carries no variation"));

        if (groundBad > 0)
            issues.Add(new TerrainIssue(TerrainIssueKind.GroundTextureOutOfRange,
                $"{groundBad} corner(s) use a ground texture index at or past the map's {groundTypes} declared ground tile(s)"));

        if (cliffBad > 0)
            issues.Add(new TerrainIssue(TerrainIssueKind.CliffTextureOutOfRange,
                $"{cliffBad} corner(s) use a cliff texture index at or past the map's {cliffTypes} declared cliff tile(s)"));

        var diagnostics = new List<string>();
        if (groundTypes == 0)
            diagnostics.Add("ground-texture-range not evaluated, the map declares no ground tile types to range-check against");
        if (cliffTypes == 0)
            diagnostics.Add("cliff-texture-range not evaluated, the map declares no cliff tile types to range-check against");

        return new TerrainAuditResult(
            name, gw, gh, tiles.Count, groundTypes, cliffTypes,
            groundIndices.Count, cliffIndices.Count,
            cliffCells, ramp, water, blight, boundary, edge,
            min, max, mean, cmin, cmax, bytes,
            issues, Checks, diagnostics);
    }

    /// <summary>Every field a corner carries, so "identical everywhere" is measured exactly.</summary>
    private readonly record struct TileSignature(
        float Height, int CliffLevel, int Texture, int CliffTexture, TileFlags Flags);

    private static TerrainAuditResult Empty(string name, long bytes, string diagnostic) =>
        new(name, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0f, 0f, 0f, 0, 0, bytes,
            Array.Empty<TerrainIssue>(), Checks, new[] { diagnostic });
}
