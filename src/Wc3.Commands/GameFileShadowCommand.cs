// src/Wc3.Commands/GameFileShadowCommand.cs
// A map that carries a file at the same path as a file of the game overrides the game's copy.
// Devo's Map Doctor reports two maps that this closed in LAN and HD graphics (UI\MiniMap textures,
// and a port with 1,486 repacked game textures), and removing the files fixed both. This detects
// the same condition against the installed game's own file list, with no external tool.
using Wc3.GameData;
using Wc3.Model;

namespace Wc3.Commands;

/// <summary>One map entry whose path is also a path of the installed game.</summary>
public sealed record GameFileShadow(string Name, long Size, string Folder);

public sealed record GameFileShadowResult(
    bool Ok,
    string Message,
    string? GameDir,
    int MapEntries,
    int GameFiles,
    int Conflicts,
    IReadOnlyList<GameFileShadow> Files,
    IReadOnlyDictionary<string, int> Folders);

public sealed record GameFileShadowApplyResult(
    bool Ok,
    string Message,
    string SavedTo,
    int Requested,
    int Dropped,
    int InputEntries,
    int OutputEntries,
    long OutputBytes);

/// <summary>
/// Finds and removes map files that sit on a path the game already ships, which makes the engine
/// read the map's copy in place of its own.
/// </summary>
/// <remarks>
/// Removal cannot go through <see cref="Wc3.Model.MapDocument"/>, whose save only adds or shadows
/// entries and has no delete, so the apply path rebuilds the archive through
/// <see cref="MpqRemoveCommand.ExecuteMany"/> from the file on disk.
/// A shadow file may be intentional, a retexture for instance, which is why detection is separate
/// and the description says so.
/// </remarks>
public static class GameFileShadowCommand
{
    /// <summary>Game folders a map is allowed to shadow without that being worth reporting.</summary>
    private static readonly string[] IgnoredPrefixes = [];

    /// <summary>
    /// The World Editor's own data files, which a SLK-mode map must carry to define its custom
    /// objects. They share a path with the game's files by construction, so reporting them as
    /// shadows would be noise and removing them would break the map.
    /// </summary>
    private static readonly string[] EditorDataFolders =
    [
        "units\\", "doodads\\", "abilities\\", "objects\\", "misc\\", "splats\\", "campaign\\",
        "human\\", "orc\\", "undead\\", "nightelf\\", "neutral\\", "terrainart\\",
    ];

    private static readonly string[] ArtExtensions =
    [
        ".mdx", ".mdl", ".blp", ".dds", ".tga", ".fdf", ".wav", ".mp3", ".dls", ".ttf",
    ];

    /// <summary>
    /// True for a path the map legitimately carries at a game path, which is the editor's object
    /// data rather than an asset the engine would otherwise read from its own archive.
    /// </summary>
    internal static bool IsEditorData(string normalized)
    {
        if (normalized.StartsWith("war3map", StringComparison.OrdinalIgnoreCase)) return true;

        var dot = normalized.LastIndexOf('.');
        var ext = dot >= 0 ? normalized[dot..] : string.Empty;

        // An SLK or a plain text table is object data wherever it sits.
        if (ext is ".slk" or ".txt") return true;

        // Anything that is not art under an object-data folder is a table fragment.
        if (Array.IndexOf(ArtExtensions, ext) < 0
            && EditorDataFolders.Any(f => normalized.StartsWith(f, StringComparison.OrdinalIgnoreCase)))
            return true;

        return false;
    }

    /// <summary>True for an extension the game ships as art, the only files worth probing one by one.</summary>
    internal static bool LooksLikeArt(string normalized)
    {
        var dot = normalized.LastIndexOf('.');
        if (dot < 0) return false;
        return Array.IndexOf(ArtExtensions, normalized[dot..]) >= 0;
    }

    public static GameFileShadowResult Detect(MapDocument doc, string? gameDir)
    {
        ArgumentNullException.ThrowIfNull(doc);

        if (string.IsNullOrWhiteSpace(gameDir))
        {
            return new GameFileShadowResult(false,
                "no Warcraft III install given, so the game's file list cannot be read. Pass --game-dir "
                + "or set WC3_GAME_DIR.", null, doc.Files.Count, 0, 0, [], new Dictionary<string, int>());
        }

        if (!CascGameDataSource.TryOpen(gameDir!, out var casc, out var error) || casc is null)
        {
            return new GameFileShadowResult(false,
                $"the game store at '{gameDir}' did not open, so the game's file list cannot be read. " + error,
                gameDir, doc.Files.Count, 0, 0, [], new Dictionary<string, int>());
        }

        using var store = casc;
        var game = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in store.EnumerateFileNames())
        {
            var key = Normalize(name);
            if (key.Length > 0) game.Add(key);
        }

        var hits = new List<GameFileShadow>();
        var folders = new SortedDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var probed = 0;
        foreach (var entry in doc.Files)
        {
            if (string.IsNullOrEmpty(entry.FileName)) continue;
            var key = Normalize(entry.FileName);
            if (key.Length == 0) continue;
            if (IgnoredPrefixes.Any(p => key.StartsWith(p, StringComparison.OrdinalIgnoreCase))) continue;
            if (IsEditorData(key)) continue;

            var inGame = game.Contains(key);
            if (!inGame)
            {
                // The listfile does not name every file the store can open, so ask per path. Only
                // art is worth a probe, which keeps this to a few dozen opens rather than thousands.
                if (!LooksLikeArt(key)) continue;
                probed++;
                inGame = store.HasFile(entry.FileName!)
                         || store.HasFile(entry.FileName!.Replace('\\', '/'))
                         || store.HasFile("war3.w3mod:" + entry.FileName)
                         || store.HasFile("war3.w3mod:" + entry.FileName!.Replace('\\', '/'));
            }
            if (!inGame) continue;

            var folder = FolderOf(entry.FileName);
            folders.TryGetValue(folder, out var n);
            folders[folder] = n + 1;
            hits.Add(new GameFileShadow(entry.FileName, entry.RawBytes.LongLength, folder));
        }

        hits.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
        var message = hits.Count == 0
            ? $"no map entry that is not editor data sits on a path of the installed game, checked against "
              + $"{game.Count} game file names"
            : $"{hits.Count} map files sit on a path of the installed game and are not editor data, checked "
              + $"against {game.Count} game file names. The engine reads the map's copy instead of its own. "
              + "The list is what this build ships, so a map aimed at a classic build can override paths "
              + "this build has renamed";

        return new GameFileShadowResult(true, message, gameDir, doc.Files.Count, game.Count,
            hits.Count, hits, folders);
    }

    /// <summary>
    /// Rebuilds the map from disk without the named entries. Everything else is carried over
    /// byte for byte by the archive rebuild.
    /// </summary>
    /// <remarks>
    /// A protected map keeps entries the archive no longer names, and the rebuild works by name, so
    /// those entries cannot be written back. Measured on a 101 MB campaign map, a rebuild that
    /// ignored this took 1578 entries to 1000 and produced a smaller, broken map. The pre-flight
    /// count refuses instead, because a silent loss of 578 assets is worse than no repair.
    /// </remarks>
    public static GameFileShadowApplyResult Apply(string mapPath, IReadOnlyList<string> names, string outPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mapPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(outPath);
        ArgumentNullException.ThrowIfNull(names);

        if (names.Count == 0)
            return new GameFileShadowApplyResult(false, "no file names were given, so nothing was removed",
                outPath, 0, 0, 0, 0, 0);

        var doc = MapDocument.Load(mapPath);
        var unnamed = doc.Files.Count(f => f.FileName is null && f.RecoveredFileName is null);
        if (unnamed > 0)
        {
            return new GameFileShadowApplyResult(false,
                $"refused, this map holds {unnamed} entries the archive does not name and the rebuild works by "
                + "name, so the copy would lose them. Devo's Map Doctor keeps those blocks and is the path to "
                + "use on a protected map. The list above is still what would need removing",
                outPath, names.Count, 0, doc.Files.Count, doc.Files.Count, 0);
        }

        var r = MpqRemoveCommand.ExecuteMany(mapPath, names, outPath);
        var message = r.Dropped == 0
            ? $"none of the {r.Requested} name(s) were present in the archive, so the copy is unchanged"
            : $"removed {r.Dropped} of {r.Requested} name(s), {r.InputEntries} to {r.OutputEntries} entries";

        // A rebuild that loses entries it was not asked to drop is a broken map, so say so.
        var expected = r.InputEntries - r.Dropped;
        if (r.Reopens && r.OutputEntries != expected)
        {
            return new GameFileShadowApplyResult(false,
                message + $", which is {expected - r.OutputEntries} fewer than the input minus the dropped names. "
                + "The copy is not a repair, do not ship it",
                r.SavedTo, r.Requested, r.Dropped, r.InputEntries, r.OutputEntries, r.OutputBytes);
        }

        return new GameFileShadowApplyResult(r.Reopens, message, r.SavedTo, r.Requested, r.Dropped,
            r.InputEntries, r.OutputEntries, r.OutputBytes);
    }

    /// <summary>
    /// Lowercase, forward slashes collapsed to backslashes, and every CASC layer prefix dropped.
    /// Names come out of the store as <c>war3.w3mod:_de.w3mod:replaceabletextures\...</c>, so more
    /// than one prefix has to come off before a map path can be compared with a game path.
    /// </summary>
    internal static string Normalize(string name)
    {
        var s = name.Trim().Replace('/', '\\').TrimStart('\\');
        while (true)
        {
            var colon = s.IndexOf(':');
            if (colon <= 0) break;
            var prefix = s[..colon];
            if (!prefix.Contains("w3mod", StringComparison.OrdinalIgnoreCase)) break;
            s = s[(colon + 1)..].TrimStart('\\');
        }
        return s.ToLowerInvariant();
    }

    /// <summary>The first two path segments, which is how the reports group shadowed files.</summary>
    private static string FolderOf(string name)
    {
        var parts = name.Replace('/', '\\').Split('\\', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length switch
        {
            0 => string.Empty,
            1 => "(root)",
            _ => parts[0] + "\\" + parts[1],
        };
    }
}
