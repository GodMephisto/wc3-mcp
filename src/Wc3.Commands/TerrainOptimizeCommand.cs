// src/Wc3.Commands/TerrainOptimizeCommand.cs
using War3Net.Build.Environment;   // MapEnvironment, TerrainTile, TerrainType, CliffType
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>What a terrain optimise pass changed, before and after, for the report.</summary>
/// <param name="Ok">False when nothing was written, with <see cref="Message"/> saying why.</param>
/// <param name="Message">A one-line human summary of what happened.</param>
/// <param name="Path">The map the pass read, or the output path once <see cref="TerrainOptimizeCommand.ExecutePath"/> wrote one.</param>
/// <param name="GroundTypesBefore">Declared ground tile entries before compaction.</param>
/// <param name="GroundTypesAfter">Declared ground tile entries after compaction, every one referenced by a cell.</param>
/// <param name="CliffTypesBefore">Declared cliff tile entries before compaction.</param>
/// <param name="CliffTypesAfter">Declared cliff tile entries after compaction.</param>
/// <param name="BytesBefore">The <c>war3map.w3e</c> entry size in bytes before the pass.</param>
/// <param name="BytesAfter">The <c>war3map.w3e</c> entry size in bytes after the pass.</param>
/// <param name="CellsRemapped">Corners whose ground or cliff index was rewritten.</param>
/// <param name="Diagnostics">The scope statement and any reason a part of the grid was left alone.</param>
public sealed record TerrainOptimizeResult(
    bool Ok,
    string Message,
    string Path,
    int GroundTypesBefore,
    int GroundTypesAfter,
    int CliffTypesBefore,
    int CliffTypesAfter,
    long BytesBefore,
    long BytesAfter,
    int CellsRemapped,
    IReadOnlyList<string> Diagnostics)
{
    /// <summary>True when at least one declared tile entry was dropped.</summary>
    public bool Changed => GroundTypesRemoved > 0 || CliffTypesRemoved > 0;
    public int GroundTypesRemoved => GroundTypesBefore - GroundTypesAfter;
    public int CliffTypesRemoved => CliffTypesBefore - CliffTypesAfter;
    /// <summary>Bytes trimmed from the decompressed <c>war3map.w3e</c>, four per dropped entry.</summary>
    public long BytesSaved => BytesBefore - BytesAfter;
}

/// <summary>
/// Shrinks a map's terrain by dropping the declared ground and cliff tile types its terrain never
/// references and remapping every corner's ground <c>Texture</c> and <c>CliffTexture</c> index onto
/// the compacted list, then writing a new map through the ordinary <see cref="MapDocument"/> save
/// path.
///
/// <para><b>Why this is safe, and the exact bytes it touches.</b> In <c>war3map.w3e</c> the two tile
/// palettes are stored explicitly, each entry one little-endian <c>int32</c> rawcode (see the War3Net
/// reader and writer for <c>MapEnvironment</c>), and a corner addresses a tile by its ordinal index,
/// not by rawcode. Dropping an entry no corner references and shifting every greater index down by
/// one therefore leaves each corner pointing at the same tile rawcode it pointed at before, so no
/// rendering or pathing can change. This pass writes only the two header lists and, for a corner
/// whose index moved, the packed ground-texture and cliff-texture fields. Every other byte of the
/// <c>war3map.w3e</c> entry is unchanged, the War3Net environment writer being byte-faithful, and
/// every other file in the archive is written back byte for byte by <see cref="MapDocument.Save"/>.
/// A dropped entry is exactly four bytes, so the decompressed <c>war3map.w3e</c> shrinks by four
/// bytes per dropped entry and no more.</para>
///
/// <para><b>What this deliberately does NOT do, so a caller is not misled.</b> It does not touch the
/// <c>Tileset</c> byte or the <c>IsCustomTileset</c> flag. Those select the engine's default palette
/// and are not derivable from the per-corner data, so changing them is a separate, unprovable
/// statement about how Warcraft III should read the map. It does not reorder a retained tile, only
/// removes unreferenced ones, so the retained order is the original order. It refuses a map whose
/// corners already reference an index outside the declared list, because remapping a dangling index
/// is guesswork.</para>
/// </summary>
public static class TerrainOptimizeCommand
{
    public const string TerrainFile = TerrainCommand.TerrainFile;

    /// <summary>The one scope statement this pass always reports, so the boundary of the change is
    /// visible in the result rather than only in this doc comment.</summary>
    private const string ScopeNote =
        "touches only the two declared tile lists and the per-corner ground and cliff index fields; "
        + "the tileset id, the custom-tileset flag and every other file are written back unchanged";

    /// <summary>Loads <paramref name="path"/>, compacts its terrain, and writes the result to
    /// <paramref name="outPath"/>. The input map is never modified.</summary>
    public static TerrainOptimizeResult ExecutePath(string path, string outPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(outPath);
        if (!File.Exists(path) && !Directory.Exists(path))
            throw new FileNotFoundException($"map not found: {path}", path);
        if (string.Equals(Path.GetFullPath(path), Path.GetFullPath(outPath), StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("out path must not be the input map");

        var doc = MapDocument.Load(path);
        var result = Execute(doc, path);
        if (!result.Ok) return result;

        if (Path.GetDirectoryName(Path.GetFullPath(outPath)) is { Length: > 0 } dir)
            Directory.CreateDirectory(dir);
        doc.Save(outPath);
        return result with { Path = outPath };
    }

    /// <summary>
    /// Compacts the terrain of an already loaded map in place, marking the <c>war3map.w3e</c> entry
    /// dirty only when a declared tile is actually dropped. The caller saves the document.
    /// </summary>
    public static TerrainOptimizeResult Execute(MapDocument doc, string name)
    {
        ArgumentNullException.ThrowIfNull(doc);

        var entry = doc.GetFile(TerrainFile);
        if (entry?.Model is not MapEnvironment env)
            return Fail(name, entry?.CurrentSize ?? 0,
                $"map has no {TerrainFile} (terrain) to optimize");

        long bytes = entry.CurrentSize;
        int groundBefore = env.TerrainTypes.Count;
        int cliffBefore = env.CliffTypes.Count;
        int gw = (int)env.Width + 1, gh = (int)env.Height + 1;
        var tiles = env.TerrainTiles;

        if (tiles is null || tiles.Count != gw * gh)
            return Fail(name, bytes,
                $"terrain tile buffer inconsistent (Width+1={gw} Height+1={gh} Tiles={tiles?.Count ?? 0}), refusing to compact");

        // Which declared entries does any corner actually reference? An empty list has nothing to
        // drop and nothing to remap, so a corner's index into it is left alone rather than treated
        // as dangling, matching the audit's own rule for a zero-length palette.
        var usedGround = new bool[groundBefore];
        var usedCliff = new bool[cliffBefore];
        bool groundOutOfRange = false, cliffOutOfRange = false;
        foreach (var t in tiles)
        {
            if (groundBefore > 0)
            {
                if (t.Texture >= groundBefore) groundOutOfRange = true;
                else usedGround[t.Texture] = true;
            }
            if (cliffBefore > 0)
            {
                if (t.CliffTexture >= cliffBefore) cliffOutOfRange = true;
                else usedCliff[t.CliffTexture] = true;
            }
        }

        if (groundOutOfRange || cliffOutOfRange)
        {
            string which = groundOutOfRange ? "ground" : "cliff";
            return Fail(name, bytes,
                $"terrain references a {which} texture index out of range of its declared tile list, refusing to compact");
        }

        int groundAfter = 0, cliffAfter = 0;
        var groundRemap = new int[groundBefore];
        for (int i = 0; i < groundBefore; i++) groundRemap[i] = usedGround[i] ? groundAfter++ : -1;
        var cliffRemap = new int[cliffBefore];
        for (int i = 0; i < cliffBefore; i++) cliffRemap[i] = usedCliff[i] ? cliffAfter++ : -1;

        int removed = (groundBefore - groundAfter) + (cliffBefore - cliffAfter);
        if (removed == 0)
            return new TerrainOptimizeResult(true,
                $"terrain already uses every declared tile ({groundBefore} ground / {cliffBefore} cliff), nothing to compact",
                name, groundBefore, groundBefore, cliffBefore, cliffBefore, bytes, bytes, 0,
                new[] { ScopeNote });

        // Rewrite each corner's index. The setter validates its range, and every index only moves
        // down, so a valid corner stays valid. An empty declared list has no remap array, so a
        // corner's index into it is left untouched.
        int cells = 0;
        foreach (var t in tiles)
        {
            bool ch = false;
            if (groundBefore > 0)
            {
                int ng = groundRemap[t.Texture];
                if (ng != t.Texture) { t.Texture = ng; ch = true; }
            }
            if (cliffBefore > 0)
            {
                int nc = cliffRemap[t.CliffTexture];
                if (nc != t.CliffTexture) { t.CliffTexture = nc; ch = true; }
            }
            if (ch) cells++;
        }

        // Replace the two palettes with their compacted form, preserving the retained order.
        if (groundAfter != groundBefore)
        {
            var kept = new List<TerrainType>(groundAfter);
            for (int i = 0; i < groundBefore; i++)
                if (groundRemap[i] >= 0) kept.Add(env.TerrainTypes[i]);
            env.TerrainTypes.Clear();
            env.TerrainTypes.AddRange(kept);
        }
        if (cliffAfter != cliffBefore)
        {
            var kept = new List<CliffType>(cliffAfter);
            for (int i = 0; i < cliffBefore; i++)
                if (cliffRemap[i] >= 0) kept.Add(env.CliffTypes[i]);
            env.CliffTypes.Clear();
            env.CliffTypes.AddRange(kept);
        }

        doc.AddOrReplaceModelFile(TerrainFile, env);

        // Each dropped entry is one four-byte rawcode in the header. No corner field changes width,
        // so the decompressed entry shrinks by exactly four bytes per dropped entry.
        long bytesAfter = bytes - 4L * removed;
        return new TerrainOptimizeResult(true,
            $"dropped {groundBefore - groundAfter} ground and {cliffBefore - cliffAfter} cliff declared tile(s), "
            + $"remapped {cells} corner(s), {TerrainFile} {bytes:N0} -> {bytesAfter:N0} byte(s)",
            name, groundBefore, groundAfter, cliffBefore, cliffAfter,
            bytes, bytesAfter, cells, new[] { ScopeNote });
    }

    private static TerrainOptimizeResult Fail(string name, long bytes, string message) =>
        new(false, message, name, 0, 0, 0, 0, bytes, bytes, 0, new[] { message });
}
