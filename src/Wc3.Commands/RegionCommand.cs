// src/Wc3.Commands/RegionCommand.cs
using System.Globalization;
using Color = System.Drawing.Color; // avoids the Region name clash with War3Net.Build.Environment.Region
using War3Net.Build.Common;        // WeatherType
using War3Net.Build.Environment;   // MapRegions, Region
using War3Net.Build.Info;          // MapInfo
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>Outcome of <see cref="RegionCommand.Create"/>. <see cref="CreationNumber"/> is
/// the region id the map assigned (-1 when the op failed).</summary>
public sealed record RegionCreateResult(bool Ok, string Message, int CreationNumber = -1);

/// <summary>One region reduced to the four corner values a JASS trigger's <c>Rect</c> call
/// needs. <see cref="Jass"/> is a ready-to-copy <c>Rect( left, bottom, right, top )</c> line.</summary>
public sealed record RegionRectResult(
    bool Ok, string Message,
    int CreationNumber = -1, string Name = "",
    float Left = 0f, float Bottom = 0f, float Right = 0f, float Top = 0f,
    string Jass = "");

/// <summary>One problem <see cref="RegionCommand.Validate"/> flags. <see cref="Kind"/> is a
/// stable token (overlap, zero-area, out-of-bounds) and <see cref="Detail"/> is human text.</summary>
public sealed record RegionIssue(string Kind, string Name, string Detail);

/// <summary>Result of <see cref="RegionCommand.Validate"/>. <see cref="Ok"/> is true when the
/// check ran and found nothing wrong.</summary>
public sealed record RegionValidateResult(
    bool Ok, string Message, int RegionCount, IReadOnlyList<RegionIssue> Issues);

/// <summary>
/// Rectangular-region authoring and checking over a map's war3map.w3r. Creation reuses the
/// region section <see cref="PlacementCommand.GetOrCreateRegions"/> already owns, so a map
/// with no regions gets one on first use. The checks are pure over a loaded
/// <see cref="MapDocument"/> and write nothing.
/// </summary>
public static class RegionCommand
{
    public const string FileName = PlacementCommand.RegionsFile;

    /// <summary>The weather effect <c>--weather</c> applies. The World Editor's own default
    /// for a fresh region is the map tileset's light rain, so this uses the Lordaeron light
    /// rain, the common summer-tileset default.</summary>
    public static readonly WeatherType DefaultWeather = WeatherType.LordaeronLightRain;

    /// <summary>
    /// Adds an axis-aligned rectangular region whose south-west corner is (<paramref name="x"/>,
    /// <paramref name="y"/>) and whose width and height are <paramref name="w"/> and
    /// <paramref name="h"/>. Rejects a blank or duplicate name and a non-positive size.
    /// <paramref name="weather"/> attaches <see cref="DefaultWeather"/> to the region.
    /// </summary>
    public static RegionCreateResult Create(
        MapDocument doc, string name, float x, float y, float w, float h, bool weather = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new(false, "region name must not be blank.");
        if (w <= 0f || h <= 0f)
            return new(false, $"invalid region size, need w>0 and h>0, got w={w} h={h}.");

        var regions = PlacementCommand.GetOrCreateRegions(doc);
        if (regions.Regions.Any(r => string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase)))
            return new(false, $"a region named '{name}' already exists.");

        int creationNumber = regions.Regions.Count == 0
            ? 0
            : regions.Regions.Max(r => r.CreationNumber) + 1;

        regions.Regions.Add(new Region
        {
            Left = x,
            Bottom = y,
            Right = x + w,
            Top = y + h,
            Name = name,
            CreationNumber = creationNumber,
            WeatherType = weather ? DefaultWeather : WeatherType.None,
            AmbientSound = string.Empty,
            Color = Color.FromArgb(255, 76, 255, 76),
        });

        doc.AddOrReplaceModelFile(FileName, regions);
        return new(true,
            $"created region '{name}' [{x},{y} .. {x + w},{y + h}] as region #{creationNumber}",
            creationNumber);
    }

    /// <summary>
    /// Resolves one named region (case-insensitive) to the four corner values its trigger
    /// <c>Rect</c> call needs, ready to print for copy into the map script. Read-only.
    /// </summary>
    public static RegionRectResult ToRect(MapDocument doc, string name)
    {
        if (doc.GetFile(FileName)?.Model is not MapRegions regions)
            return new(false, $"map has no {FileName} (no regions).");

        var region = regions.Regions.FirstOrDefault(
            r => string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase));
        if (region is null)
            return new(false, $"no region named '{name}'.");

        string jass = string.Format(CultureInfo.InvariantCulture,
            "Rect( {0:0.##}, {1:0.##}, {2:0.##}, {3:0.##} )",
            region.Left, region.Bottom, region.Right, region.Top);

        return new(true, $"region '{name}' as a trigger rect",
            region.CreationNumber, region.Name,
            region.Left, region.Bottom, region.Right, region.Top, jass);
    }

    /// <summary>
    /// Flags overlapping regions, zero-area regions, and regions that fall outside the
    /// playable bounds. Two regions overlap only when their interiors intersect, so regions
    /// that merely share an edge do not count. Playable bounds come from war3map.w3i, centred
    /// on the origin at PlayableMapAreaWidth/Height tiles of 128 world units each. When a map
    /// has no map info the bounds check is skipped. Read-only.
    /// </summary>
    public static RegionValidateResult Validate(MapDocument doc)
    {
        if (doc.GetFile(FileName)?.Model is not MapRegions regions)
            return new(true, $"map has no {FileName}, nothing to check.", 0, Array.Empty<RegionIssue>());

        var list = regions.Regions;
        var issues = new List<RegionIssue>();

        foreach (var r in list)
        {
            float width = r.Right - r.Left;
            float height = r.Top - r.Bottom;
            if (width <= 0f || height <= 0f)
                issues.Add(new RegionIssue("zero-area", r.Name,
                    $"region '{r.Name}' has no area (w={width}, h={height})."));
        }

        for (int i = 0; i < list.Count; i++)
            for (int j = i + 1; j < list.Count; j++)
            {
                var a = list[i];
                var b = list[j];
                float overlapW = Math.Min(a.Right, b.Right) - Math.Max(a.Left, b.Left);
                float overlapH = Math.Min(a.Top, b.Top) - Math.Max(a.Bottom, b.Bottom);
                if (overlapW > 0f && overlapH > 0f)
                    issues.Add(new RegionIssue("overlap", a.Name,
                        $"regions '{a.Name}' and '{b.Name}' overlap by {overlapW} x {overlapH}."));
            }

        if (doc.GetFile(MapInfoCommand.FileName)?.Model is MapInfo info
            && info.PlayableMapAreaWidth > 0 && info.PlayableMapAreaHeight > 0)
        {
            float halfX = info.PlayableMapAreaWidth * 64f;
            float halfY = info.PlayableMapAreaHeight * 64f;
            foreach (var r in list)
            {
                if (r.Left < -halfX || r.Right > halfX || r.Bottom < -halfY || r.Top > halfY)
                    issues.Add(new RegionIssue("out-of-bounds", r.Name,
                        $"region '{r.Name}' [{r.Left},{r.Bottom} .. {r.Right},{r.Top}] is outside "
                        + $"the playable bounds [{-halfX},{-halfY} .. {halfX},{halfY}]."));
            }
        }

        string message = issues.Count == 0
            ? $"checked {list.Count} region(s), no problems found."
            : $"checked {list.Count} region(s), found {issues.Count} problem(s).";
        return new(issues.Count == 0, message, list.Count, issues);
    }
}
