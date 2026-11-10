// src/Wc3.Commands/LibraryScanCommand.cs
using War3Net.Build.Info;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>
/// One map's identity and load state from a library scan. Bytes is the file size on disk, not the
/// sum of entry sizes, because a maintainer comparing a folder wants the size that occupies the
/// disk. TargetBuild is the game version the map itself declares, and is null when the map's w3i
/// format predates the field or the w3i did not parse.
/// </summary>
public sealed record LibraryScanMap(
    string Map,
    long Bytes,
    string? TargetBuild,
    bool Loads,
    int Entries,
    int Diagnostics,
    string? Error);

/// <summary>A folder scan: every map's row plus the loaded and failed counts and the byte total.</summary>
public sealed record LibraryScanResult(
    IReadOnlyList<LibraryScanMap> Maps,
    int Loaded,
    int Failed,
    long TotalBytes);

/// <summary>
/// Open every map in a folder through <see cref="MapDocument.Load"/> and report per map its size,
/// the build it declares, whether it loads, its entry count and its diagnostic count. Read-only,
/// the same load the single-map commands use, over a whole folder rather than one map.
///
/// Maps are opened one at a time. Each document is released before the next is opened, so a sweep
/// never holds a whole library in memory at once, and the result carries only the small rows.
///
/// PRF-04. The sweep takes a cancellation token and reads it before each map opens, so a cancel
/// stops the run at the next file rather than at the end of the folder. The exception it throws
/// names how many maps were scanned and how many the folder holds, so a client that cancelled can
/// see how far the sweep got. The map that is already open when the token fires is finished, which
/// is the unit of work this command can honour.
/// </summary>
public static class LibraryScanCommand
{
    /// <param name="progress">PRF-04. Called once per map with how many are done and how many there
    /// are, so the tool in front of this command can report where the sweep is. Null reports
    /// nothing, which is what the CLI passes.</param>
    public static LibraryScanResult Execute(string folder, CancellationToken cancellationToken = default,
        Action<int, int>? progress = null)
    {
        var paths = BenchMapFolder.Enumerate(folder);
        var maps = new List<LibraryScanMap>(paths.Count);
        for (int i = 0; i < paths.Count; i++)
        {
            // PRF-04. The check point is the top of the loop, before the map opens, so the cancel
            // costs one map at most and never a whole folder.
            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException(
                    "library scan cancelled after " + i + " of " + paths.Count + " maps");

            string path = paths[i];
            string name = Path.GetFileName(path);
            long bytes = new FileInfo(path).Length;
            try
            {
                maps.Add(Probe(name, bytes, MapDocument.Load(path)));
            }
            catch (Exception e) when (e is IOException or InvalidDataException or UnauthorizedAccessException)
            {
                maps.Add(Fail(name, bytes, e.Message));
            }
            progress?.Invoke(maps.Count, paths.Count);
        }
        return Summarize(maps);
    }

    /// <summary>One already-loaded map's scan row. Pure, so the report can reuse one load.</summary>
    public static LibraryScanMap Probe(string name, long bytes, MapDocument doc)
    {
        string? build = (doc.GetFile("war3map.w3i")?.Model as MapInfo)?.GameVersion?.ToString();
        return new LibraryScanMap(name, bytes, build, true, doc.Files.Count, doc.Diagnostics.Count, null);
    }

    /// <summary>A map that could not be loaded, given its file name, size and the failure reason.</summary>
    public static LibraryScanMap Fail(string name, long bytes, string error) =>
        new(name, bytes, null, false, 0, 0, error);

    /// <summary>Fold the per-map rows into the loaded, failed and byte totals. Pure.</summary>
    public static LibraryScanResult Summarize(IReadOnlyList<LibraryScanMap> maps) =>
        new(maps, maps.Count(m => m.Loads), maps.Count(m => !m.Loads), maps.Sum(m => m.Bytes));
}
