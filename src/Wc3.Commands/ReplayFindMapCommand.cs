// src/Wc3.Commands/ReplayFindMapCommand.cs
namespace Wc3.Commands;

/// <summary>Where a replay's recorded map lands on disk. <see cref="RecordedMap"/> is the raw value
/// the settings blob carried, <see cref="CleanMapPath"/> is that value trimmed to the Maps root,
/// and <see cref="ResolvedPath"/> is the file found in the folder, null when none matched.
/// <see cref="Candidates"/> lists every file whose name matched, as paths relative to the folder.</summary>
public sealed record ReplayFindMapReport(
    string File, string? RecordedMap, string? CleanMapPath, string MapsFolder,
    string? ResolvedPath, bool Found, IReadOnlyList<string> Candidates, string? Problem);

/// <summary>
/// Resolve the map a replay records to a file in a Maps folder, default the user's
/// Documents\Warcraft III\Maps. The recorded path is matched on its file name first, since the
/// settings blob carries a short non text prefix before the path, then a full relative path match
/// is preferred over a bare name match. Both .w3x and .w3m are resolved. Read only.
/// </summary>
public static class ReplayFindMapCommand
{
    public static ReplayFindMapReport Execute(string replay, string? mapsFolder = null)
    {
        var g = ReplayCommand.ReadGame(replay);
        string folder = mapsFolder ?? DefaultFolder();
        string? cleaned = ReplayCommand.CleanMapPath(g.Map);
        var (resolved, candidates) = g.Problem is null
            ? Resolve(cleaned, folder)
            : (null, (IReadOnlyList<string>)Array.Empty<string>());
        return new(g.File, g.Map, cleaned, folder, resolved, resolved is not null,
            candidates, g.Problem);
    }

    private static string DefaultFolder() => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Warcraft III", "Maps");

    /// <summary>Pure resolution over a folder. Returns the matched file and every name match.</summary>
    /// <summary>
    /// True when <paramref name="tail"/> is the end of <paramref name="path"/> on a separator
    /// boundary, so "Download/X.w3x" matches "Maps/Download/X.w3x" but not "Other/Download/X.w3x"
    /// unless it is a real tail.
    /// </summary>
    private static bool IsTailOf(string tail, string path)
    {
        if (path.Equals(tail, StringComparison.OrdinalIgnoreCase)) return true;
        if (path.Length <= tail.Length) return false;
        if (!path.EndsWith(tail, StringComparison.OrdinalIgnoreCase)) return false;
        char boundary = path[path.Length - tail.Length - 1];
        return boundary == '/' || boundary == '\\';
    }

    internal static (string? Resolved, IReadOnlyList<string> Candidates) Resolve(
        string? cleanedMap, string folder)
    {
        if (string.IsNullOrEmpty(cleanedMap) || !Directory.Exists(folder))
            return (null, Array.Empty<string>());

        string wanted = cleanedMap.Replace('\\', '/');
        string wantedName = wanted[(wanted.LastIndexOf('/') + 1)..];

        IEnumerable<string> files;
        try { files = Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories); }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            return (null, Array.Empty<string>());
        }

        var matches = new List<(string Full, string Relative)>();
        foreach (var file in files)
        {
            string ext = Path.GetExtension(file);
            if (!ext.Equals(".w3x", StringComparison.OrdinalIgnoreCase)
                && !ext.Equals(".w3m", StringComparison.OrdinalIgnoreCase))
                continue;
            if (!Path.GetFileName(file).Equals(wantedName, StringComparison.OrdinalIgnoreCase))
                continue;
            matches.Add((file, Path.GetRelativePath(folder, file).Replace('\\', '/')));
        }

        // The replay records the map relative to the install, "Maps\Download\X.w3x", while a
        // candidate is relative to the search folder, "Download\X.w3x", so comparing the two
        // directly never matched. Resolution then fell through to the first enumerated candidate,
        // which was the right one on Windows by directory order and the wrong one on macOS. The
        // recorded path is matched on its tail, longest tail first so the most specific wins.
        var exact = matches
            .Where(m => IsTailOf(m.Relative, wanted))
            .OrderByDescending(m => m.Relative.Length)
            .ThenBy(m => m.Relative, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();
        string? resolved = exact.Relative is not null ? exact.Full : matches.FirstOrDefault().Full;
        var candidates = matches.Select(m => m.Relative)
            .OrderBy(r => r, StringComparer.OrdinalIgnoreCase).ToList();
        return (resolved, candidates);
    }
}
